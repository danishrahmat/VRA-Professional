
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VRA.App.Configuration;
using VRA.App.Display;
using VRA.App.Views;
using VRA.App.VideoLibrary;


namespace VRA.App.Services;

public class WpfReinforcementService : IReinforcementService
{
    private readonly DisplayDiscoveryService _displayDiscoveryService;
    private readonly DisplayMappingSettingsService _mappingSettingsService;
    private readonly ReinforcementWindowPositioningService _positioningService;
    private readonly ReinforcementVideoLibraryService _videoLibraryService;

    private readonly Dictionary<string, ReinforcementWindow> _windows =
        new(StringComparer.OrdinalIgnoreCase);

    public WpfReinforcementService(
    DisplayDiscoveryService displayDiscoveryService,
    DisplayMappingSettingsService mappingSettingsService,
    ReinforcementWindowPositioningService positioningService,
    ReinforcementVideoLibraryService videoLibraryService)
    {
        _displayDiscoveryService = displayDiscoveryService;
        _mappingSettingsService = mappingSettingsService;
        _positioningService = positioningService;
        _videoLibraryService = videoLibraryService;
    }

    public void ShowAnimation(string display)
    {
        var settings = _mappingSettingsService.Load();

        string? deviceName = display.ToLowerInvariant() switch
        {
            "left" => settings.LeftDeviceName,
            "centre" => settings.CentreDeviceName,
            "right" => settings.RightDeviceName,
            _ => throw new ArgumentException(
                $"Unknown reinforcement target: {display}", nameof(display))
        };

        if (string.IsNullOrWhiteSpace(deviceName))
        {
            throw new InvalidOperationException(
                $"No monitor is assigned to the {display} target.");
        }

        var monitor = _displayDiscoveryService.GetDisplays()
            .FirstOrDefault(d => string.Equals(
                d.DeviceName,
                deviceName,
                StringComparison.OrdinalIgnoreCase));

        if (monitor is null)
        {
            throw new InvalidOperationException(
                $"The monitor assigned to {display} is not currently detected.");
        }

        if (!_windows.TryGetValue(display, out var window))
        {
            window = new ReinforcementWindow();
            _windows[display] = window;
        }

        window.SetTarget(display);
        _positioningService.PositionWindow(window, monitor);

        if (!window.IsVisible)
        {
            window.Show();
        }
    }

    public void PlayRandomVideo(string display, string folderId)
    {
        var video = _videoLibraryService.SelectRandomVideo(folderId);

        if (video is null)
        {
            throw new InvalidOperationException(
                $"No usable MP4 videos were found in folder '{folderId}'. " +
                "Add MP4 files to the folder and refresh the video library.");
        }

        // Reuse the existing monitor mapping and positioning logic.
        ShowAnimation(display);

        var window = _windows[display];
        window.PlayVideo(video.FilePath);

        Debug.WriteLine(
            $"Playing random reinforcement video '{video.FileName}' " +
            $"from folder '{folderId}' on {display}.");
    }

    public void PlaySound(string display)
    {
        Debug.WriteLine(
            $"Sound request for {display}. Audio output is not implemented yet.");
    }

    public void StopAll()
    {
        foreach (var window in _windows.Values.ToArray())
        {
            window.Close();
        }

        _windows.Clear();
        Debug.WriteLine("All reinforcement windows closed.");
    }
}
