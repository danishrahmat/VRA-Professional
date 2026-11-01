
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRA.App.Services.Audio;

namespace VRA.App.ViewModels;

public partial class AudioSettingsViewModel : ObservableObject
{
    private readonly AudioDeviceDiscoveryService _audioDeviceService;
    private readonly AudioPlaybackService _audioPlaybackService;

    private readonly string _settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VRA Professional",
        "audio-device-mappings.json");

    [ObservableProperty]
    private ObservableCollection<AudioDeviceInfo> audioDevices = [];

    [ObservableProperty]
    private ObservableCollection<AudioDeviceInfo> usbAudioDevices = [];

    [ObservableProperty]
    private string? leftDeviceId;

    [ObservableProperty]
    private string? centreDeviceId;

    [ObservableProperty]
    private string? rightDeviceId;

    [ObservableProperty]
    private double leftVolume = 0.15;

    [ObservableProperty]
    private double centreVolume = 0.15;

    [ObservableProperty]
    private double rightVolume = 0.15;

    [ObservableProperty]
    private string statusMessage = "Ready to detect audio devices.";

    public AudioSettingsViewModel(
    AudioDeviceDiscoveryService audioDeviceService,
    AudioPlaybackService audioPlaybackService)
    {
        _audioDeviceService = audioDeviceService;
        _audioPlaybackService = audioPlaybackService;

        RefreshDevices();
        LoadAssignments();
    }

    [RelayCommand]
    private void RefreshDevices()
    {
        try
        {
            var devices = _audioDeviceService.GetPlaybackDevices();

            AudioDevices = new ObservableCollection<AudioDeviceInfo>(
                devices);

            UsbAudioDevices = new ObservableCollection<AudioDeviceInfo>(
                devices.Where(device =>
                    device.Name.Contains(
                        "USB PnP Sound Device",
                        StringComparison.OrdinalIgnoreCase)));

            StatusMessage =
                $"Detected {AudioDevices.Count} active playback device(s). " +
                $"USB audio adapters: {UsbAudioDevices.Count}.";
        }
        catch (Exception ex)
        {
            AudioDevices.Clear();
            UsbAudioDevices.Clear();

            StatusMessage =
                $"Unable to detect audio devices: {ex.Message}";
        }
    }



    private void ValidateDeviceAssignment(
        string position,
        string? deviceId,
        string currentPosition)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            return;

        bool alreadyAssigned =
            currentPosition != "Left" &&
            string.Equals(deviceId, LeftDeviceId,
                StringComparison.OrdinalIgnoreCase)
            ||
            currentPosition != "Centre" &&
            string.Equals(deviceId, CentreDeviceId,
                StringComparison.OrdinalIgnoreCase)
            ||
            currentPosition != "Right" &&
            string.Equals(deviceId, RightDeviceId,
                StringComparison.OrdinalIgnoreCase);

        if (!alreadyAssigned)
            return;

        switch (currentPosition)
        {
            case "Left":
                LeftDeviceId = null;
                break;

            case "Centre":
                CentreDeviceId = null;
                break;

            case "Right":
                RightDeviceId = null;
                break;
        }

        StatusMessage =
            $"{position}: This audio device is already assigned " +
            "to another speaker position.";
    }

    [RelayCommand]
    private void TestLeftAudio()
    {
        TestAudio(LeftDeviceId, "LEFT", LeftVolume);
    }

    [RelayCommand]
    private void TestCentreAudio()
    {
        TestAudio(CentreDeviceId, "CENTRE", CentreVolume);
    }

    [RelayCommand]
    private void TestRightAudio()
    {
        TestAudio(RightDeviceId, "RIGHT", RightVolume);
    }

    [RelayCommand]
    private void StopAudio()
    {
        _audioPlaybackService.Stop();
        StatusMessage = "Audio playback stopped.";
    }

    private void TestAudio(
    string? deviceId,
    string position,
    double volume)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            StatusMessage = $"Please assign an audio device to {position}.";
            return;
        }

        try
        {
            _audioPlaybackService.PlayTestTone(deviceId, volume);

            StatusMessage =
                $"Playing {position} audio at {volume:P0} volume.";
        }
        catch (Exception ex)
        {
            StatusMessage =
                $"Unable to play {position} audio: {ex.Message}";
        }
    }
    [RelayCommand]
    private void SaveAssignments()
    {
        // Check for duplicate audio device assignments
        var selectedIds = new[]
        {
        LeftDeviceId,
        CentreDeviceId,
        RightDeviceId
    };

        var assignedIds = selectedIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();

        if (assignedIds.Count != assignedIds
            .Distinct(StringComparer.OrdinalIgnoreCase).Count())
        {
            StatusMessage =
                "Error: The same audio device cannot be assigned to multiple speakers.";

            return;
        }

        try
        {
            var settings = new AudioDeviceMappingSettings
            {
                LeftDeviceId = LeftDeviceId,
                CentreDeviceId = CentreDeviceId,
                RightDeviceId = RightDeviceId,

                LeftVolume = LeftVolume,
                CentreVolume = CentreVolume,
                RightVolume = RightVolume
            };

            string? directory = Path.GetDirectoryName(_settingsPath);

            if (directory is null)
                throw new InvalidOperationException(
                    "Unable to determine audio settings directory.");

            Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(
                settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(_settingsPath, json);

            StatusMessage =
                $"Saved - LEFT: {LeftDeviceId ?? "NULL"} | " +
                $"CENTRE: {CentreDeviceId ?? "NULL"} | " +
                $"RIGHT: {RightDeviceId ?? "NULL"}";
        }
        catch (Exception ex)
        {
            StatusMessage =
                $"Unable to save audio assignments: {ex.Message}";
        }
    }

    private void LoadAssignments()
    {
        try
        {
            if (!File.Exists(_settingsPath))
                return;

            string json = File.ReadAllText(_settingsPath);

            var settings =
                JsonSerializer.Deserialize<AudioDeviceMappingSettings>(json);

            if (settings is null)
                return;

            LeftDeviceId = settings.LeftDeviceId;
            CentreDeviceId = settings.CentreDeviceId;
            RightDeviceId = settings.RightDeviceId;

            LeftVolume = Math.Clamp(settings.LeftVolume ?? 0.15, 0.0, 1.0);
            CentreVolume = Math.Clamp(settings.CentreVolume ?? 0.15, 0.0, 1.0);
            RightVolume = Math.Clamp(settings.RightVolume ?? 0.15, 0.0, 1.0);
        }
        catch (Exception ex)
        {
            StatusMessage =
                $"Unable to load saved audio assignments: {ex.Message}";
        }
    }
}

public sealed class AudioDeviceMappingSettings
{
    public string? LeftDeviceId { get; set; }
    public string? CentreDeviceId { get; set; }
    public string? RightDeviceId { get; set; }

    public double? LeftVolume { get; set; }
    public double? CentreVolume { get; set; }
    public double? RightVolume { get; set; }
}
