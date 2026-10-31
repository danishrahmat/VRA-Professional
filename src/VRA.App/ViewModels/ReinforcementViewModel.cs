
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRA.App.Configuration;
using VRA.App.Display;
using VRA.App.Infrastructure;
using VRA.App.Models;
using VRA.App.Services;
using VRA.App.ViewModels;
using VRA.Common.Enums;
using VRA.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using VRA.App.VideoLibrary;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace VRA.App.ViewModels;

public partial class ReinforcementViewModel : ObservableObject
{
    private readonly CurrentPatientService _currentPatientService;
    private readonly CurrentHearingTestSessionService _currentSessionService;
    private readonly IReinforcementService _reinforcementService;
    private readonly ReinforcementConfiguration _configuration;
    private readonly DisplayDiscoveryService _displayDiscoveryService;
    private readonly DisplayMappingSettingsService _displayMappingSettingsService;
    private readonly ReinforcementVideoLibraryService _videoLibraryService;


    public ReinforcementViewModel(
        CurrentPatientService currentPatientService,
        CurrentHearingTestSessionService currentSessionService,
        IReinforcementService reinforcementService,
        ReinforcementConfiguration configuration,
        DisplayDiscoveryService displayDiscoveryService,
        DisplayMappingSettingsService displayMappingSettingsService,
        ReinforcementVideoLibraryService videoLibraryService)
    {
        _currentPatientService = currentPatientService;
        _currentSessionService = currentSessionService;
        _reinforcementService = reinforcementService;
        _configuration = configuration;
        _displayDiscoveryService = displayDiscoveryService;

        CurrentPatient = _currentPatientService.CurrentPatient;
        CurrentSession = _currentSessionService.CurrentSession;

        UpdateHearingSessionStatus();

        _currentPatientService.CurrentPatientChanged += HandleCurrentPatientChanged;
        _currentSessionService.SessionChanged += HandleSessionChanged;
        _displayMappingSettingsService = displayMappingSettingsService;
        _videoLibraryService = videoLibraryService;

        RefreshDisplays();
        RefreshVideoFolders();
    }


    [ObservableProperty]
    private Patient? currentPatient;

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string selectedDisplay = "Centre";

    [ObservableProperty]
    private string activeReinforcement = "None";

    [ObservableProperty]
    private string activeTarget = "None";

    [ObservableProperty]
    private HearingTestSession? currentSession;

    [ObservableProperty]
    private string hearingSessionStatus = "No active hearing test";

    [ObservableProperty]
    private ReinforcementState outputState = ReinforcementState.Idle;

    [ObservableProperty]
    private ObservableCollection<DisplayInfo> detectedDisplays = [];

    [ObservableProperty]
    private DisplayMapping? leftDisplayMapping;

    [ObservableProperty]
    private DisplayMapping? centreDisplayMapping;

    [ObservableProperty]
    private DisplayMapping? rightDisplayMapping;

    [ObservableProperty]
    private ObservableCollection<ReinforcementVideoFolder> videoFolders = [];

    [ObservableProperty]
    private ReinforcementVideoFolder? selectedVideoFolder;

    [ObservableProperty]
    private ObservableCollection<ReinforcementVideo> availableVideos = [];

    [ObservableProperty]
    private ReinforcementVideo? selectedVideo;




    [RelayCommand]
    private void SelectDisplay(string display)
    {
        bool isConfigured = _configuration.Displays.Any(
            item => string.Equals(
                item.Name,
                display,
                StringComparison.OrdinalIgnoreCase));

        if (!isConfigured)
        {
            StatusMessage = $"Display '{display}' is not configured.";
            return;
        }

        SelectedDisplay = _configuration.Displays
            .First(item => string.Equals(
                item.Name,
                display,
                StringComparison.OrdinalIgnoreCase))
            .Name;

        StatusMessage = $"{SelectedDisplay} display selected";
    }





    [RelayCommand(CanExecute = nameof(CanRunReinforcement))]
    private void ShowAnimation()
    {
        if (!CanRunReinforcement()) return;

        try
        {
            if (SelectedVideoFolder is null)
            {
                StatusMessage = "Please select a video folder first.";
                return;
            }

            _reinforcementService.PlayRandomVideo(
                SelectedDisplay,
                SelectedVideoFolder.Id);

            OutputState = ReinforcementState.AnimationRequested;
            ActiveReinforcement = "Animation";
            ActiveTarget = SelectedDisplay;
            StatusMessage =
                $"Playing random video from {SelectedVideoFolder.Name} " +
    $"on {SelectedDisplay} display.";
        }
        catch (ArgumentException ex)
        {
            OutputState = ReinforcementState.Error;
            ActiveReinforcement = "None";
            ActiveTarget = "None";
            StatusMessage = $"Animation request failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            OutputState = ReinforcementState.Error;
            ActiveReinforcement = "None";
            ActiveTarget = "None";
            StatusMessage = "Animation request failed. Check Debug Output.";
        }
    }





    [RelayCommand(CanExecute = nameof(CanRunReinforcement))]
    private void PlaySound()
    {
        if (!CanRunReinforcement()) return;

        try
        {
            _reinforcementService.PlaySound(SelectedDisplay);

            OutputState = ReinforcementState.SoundRequested;
            ActiveReinforcement = "Sound";
            ActiveTarget = SelectedDisplay;
            StatusMessage =
                $"Sound requested for {SelectedDisplay} display (UI test only)";
        }
        catch (ArgumentException ex)
        {
            OutputState = ReinforcementState.Error;
            ActiveReinforcement = "None";
            ActiveTarget = "None";
            StatusMessage = $"Sound request failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            OutputState = ReinforcementState.Error;
            ActiveReinforcement = "None";
            ActiveTarget = "None";
            StatusMessage = "Sound request failed. Check Debug Output.";
        }
    }





    [RelayCommand]
    private void StopAll()
    {
        try
        {
            _reinforcementService.StopAll();

            OutputState = ReinforcementState.Idle;
            ActiveReinforcement = "None";
            ActiveTarget = "None";
            StatusMessage = "All reinforcement outputs stopped (UI test only)";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            OutputState = ReinforcementState.Error;
            ActiveReinforcement = "None";
            ActiveTarget = "None";
            StatusMessage = "Stop request failed. Check Debug Output.";
        }
    }





    [RelayCommand]
    private void RefreshDisplays()
    {
        var displays = _displayDiscoveryService.GetDisplays();

        DetectedDisplays = new ObservableCollection<DisplayInfo>(displays);

        // Load saved assignments.
        var savedSettings = _displayMappingSettingsService.Load();

        // Restore saved assignments when available.
        LeftDisplayMapping ??= new DisplayMapping
        {
            TargetName = "Left",
            DeviceName = savedSettings.LeftDeviceName
        };

        CentreDisplayMapping ??= new DisplayMapping
        {
            TargetName = "Centre",
            DeviceName = savedSettings.CentreDeviceName
        };

        RightDisplayMapping ??= new DisplayMapping
        {
            TargetName = "Right",
            DeviceName = savedSettings.RightDeviceName
        };

        // Apply defaults only when no saved assignment exists.
        LeftDisplayMapping.DeviceName ??=
            displays.ElementAtOrDefault(0)?.DeviceName;

        CentreDisplayMapping.DeviceName ??=
            displays.ElementAtOrDefault(1)?.DeviceName;

        RightDisplayMapping.DeviceName ??=
            displays.ElementAtOrDefault(2)?.DeviceName;

        StatusMessage = $"Detected {DetectedDisplays.Count} active display(s).";
    }



    [RelayCommand]
    private void AssignLeftDisplay(string? deviceName)
    {
        TryAssignDisplay("Left", deviceName);
    }

    [RelayCommand]
    private void AssignCentreDisplay(string? deviceName)
    {
        TryAssignDisplay("Centre", deviceName);
    }

    [RelayCommand]
    private void AssignRightDisplay(string? deviceName)
    {
        TryAssignDisplay("Right", deviceName);
    }



    private void HandleCurrentPatientChanged(Patient? patient)
    {
        CurrentPatient = patient;
        UpdateHearingSessionStatus();
    }

    private void HandleSessionChanged(HearingTestSession? session)
    {
        CurrentSession = session;
        UpdateHearingSessionStatus();
    }

    private void UpdateHearingSessionStatus()
    {
        HearingSessionStatus = CurrentSession?.Status switch
        {
            HearingTestStatus.InProgress => "In Progress",
            HearingTestStatus.Completed => "Completed",
            HearingTestStatus.Cancelled => "Cancelled",
            _ => "No active hearing test"
        };
    }


    partial void OnCurrentPatientChanged(Patient? value)
    {
        ShowAnimationCommand.NotifyCanExecuteChanged();
        PlaySoundCommand.NotifyCanExecuteChanged();
    }

    partial void OnCurrentSessionChanged(HearingTestSession? value)
    {
        ShowAnimationCommand.NotifyCanExecuteChanged();
        PlaySoundCommand.NotifyCanExecuteChanged();

        if (value?.Status != HearingTestStatus.InProgress)
        {
            StopAll();
        }
    }

    private bool CanRunReinforcement()
    {
        return CurrentPatient is not null
            && CurrentSession?.Status == HearingTestStatus.InProgress;
    }


    public bool IsDisplayAlreadyAssigned(
        string? deviceName,
        string targetName)
    {
        if (string.IsNullOrWhiteSpace(deviceName))
            return false;

        var mappings = new[]
        {
        LeftDisplayMapping,
        CentreDisplayMapping,
        RightDisplayMapping
    };

        return mappings.Any(mapping =>
            mapping is not null
            && !string.Equals(
                mapping.TargetName,
                targetName,
                StringComparison.OrdinalIgnoreCase)
            && string.Equals(
                mapping.DeviceName,
                deviceName,
                StringComparison.OrdinalIgnoreCase));
    }



    public bool TryAssignDisplay(
        string targetName,
        string? deviceName)
    {
        if (IsDisplayAlreadyAssigned(deviceName, targetName))
        {
            StatusMessage =
                $"This display is already assigned to another target. " +
                $"Choose a different display for {targetName}.";

            return false;
        }

        var mapping = targetName switch
        {
            "Left" => LeftDisplayMapping,
            "Centre" => CentreDisplayMapping,
            "Right" => RightDisplayMapping,
            _ => null
        };

        if (mapping is null)
            return false;

        mapping.DeviceName = deviceName;

        SaveDisplayMappings();

        StatusMessage = $"{targetName} display mapping updated.";

        return true;
    }



    private void SaveDisplayMappings()
    {
        var settings = new DisplayMappingSettings
        {
            LeftDeviceName = LeftDisplayMapping?.DeviceName,
            CentreDeviceName = CentreDisplayMapping?.DeviceName,
            RightDeviceName = RightDisplayMapping?.DeviceName
        };

        try
        {
            _displayMappingSettingsService.Save(settings);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            StatusMessage =
                "Display mapping changed, but saving failed. Check Debug Output.";
        }
    }

    [RelayCommand]
    private void RefreshVideoFolders()
    {
        string? selectedId = SelectedVideoFolder?.Id;

        VideoFolders = new ObservableCollection<ReinforcementVideoFolder>(
            _videoLibraryService.GetFolders());

        SelectedVideoFolder =
            VideoFolders.FirstOrDefault(folder => folder.Id == selectedId)
            ?? VideoFolders.FirstOrDefault();

        RefreshVideos();
    }

    [RelayCommand]
    private void RefreshVideos()
    {
        if (SelectedVideoFolder is null)
        {
            AvailableVideos = [];
            SelectedVideo = null;
            return;
        }

        AvailableVideos = new ObservableCollection<ReinforcementVideo>(
            _videoLibraryService.GetVideos(SelectedVideoFolder.Id));

        SelectedVideo = AvailableVideos.FirstOrDefault();

        StatusMessage =
            $"Found {AvailableVideos.Count} video(s) in {SelectedVideoFolder.Name}.";
    }

    [RelayCommand]
    private void BrowseVideoFolder()
    {
        if (SelectedVideoFolder is null)
        {
            StatusMessage = "Please select a video folder first.";
            return;
        }

        var dialog = new OpenFolderDialog
        {
            Title = $"Select folder for {SelectedVideoFolder.Name}",
            Multiselect = false
        };

        if (Directory.Exists(SelectedVideoFolder.FolderPath))
        {
            dialog.InitialDirectory = SelectedVideoFolder.FolderPath;
        }

        try
        {
            if (dialog.ShowDialog() != true)
                return;

            _videoLibraryService.SetFolderPath(
                SelectedVideoFolder.Id,
                dialog.FolderName);

            string selectedId = SelectedVideoFolder.Id;

            RefreshVideoFolders();

            StatusMessage =
                $"{SelectedVideoFolder?.Name} folder location updated.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            StatusMessage =
                "Unable to change video folder. Check Debug Output.";
        }
    }

    [RelayCommand]
    private void OpenVideoFolder()
    {
        if (SelectedVideoFolder is null)
        {
            StatusMessage = "Please select a video folder first.";
            return;
        }

        try
        {
            string folderPath = SelectedVideoFolder.FolderPath;

            Directory.CreateDirectory(folderPath);

            Process.Start(new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true
            });

            StatusMessage =
                $"Opened {SelectedVideoFolder.Name}.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            StatusMessage =
                "Unable to open video folder. Check Debug Output.";
        }
    }
    partial void OnSelectedVideoFolderChanged(
        ReinforcementVideoFolder? value)
    {
        RefreshVideos();
    }

}
