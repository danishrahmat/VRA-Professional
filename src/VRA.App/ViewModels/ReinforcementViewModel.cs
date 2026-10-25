
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRA.App.Infrastructure;
using VRA.Core.Models;
using VRA.App.Services;
using VRA.Common.Enums;
using VRA.App.Models;

namespace VRA.App.ViewModels;

public partial class ReinforcementViewModel : ObservableObject
{
    private readonly CurrentPatientService _currentPatientService;
    private readonly CurrentHearingTestSessionService _currentSessionService;
    private readonly IReinforcementService _reinforcementService;

    public ReinforcementViewModel(
    CurrentPatientService currentPatientService,
    CurrentHearingTestSessionService currentSessionService,
    IReinforcementService reinforcementService)
    {
        _currentPatientService = currentPatientService;
        _currentSessionService = currentSessionService;
        _reinforcementService = reinforcementService;

        CurrentPatient = _currentPatientService.CurrentPatient;
        CurrentSession = _currentSessionService.CurrentSession;

        UpdateHearingSessionStatus();

        _currentPatientService.CurrentPatientChanged += HandleCurrentPatientChanged;
        _currentSessionService.SessionChanged += HandleSessionChanged;
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



    [RelayCommand]
    private void SelectDisplay(string display)
    {
        if (display is not ("Left" or "Centre" or "Right"))
            return;

        SelectedDisplay = display;
        StatusMessage = $"{display} display selected";
    }




    [RelayCommand(CanExecute = nameof(CanRunReinforcement))]
    private void ShowAnimation()
    {
        if (!CanRunReinforcement()) return;

        try
        {
            _reinforcementService.ShowAnimation(SelectedDisplay);

            OutputState = ReinforcementState.AnimationRequested;
            ActiveReinforcement = "Animation";
            ActiveTarget = SelectedDisplay;
            StatusMessage =
                $"Animation requested on {SelectedDisplay} display (UI test only)";
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


}
