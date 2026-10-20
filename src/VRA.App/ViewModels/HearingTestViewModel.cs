
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRA.App.Infrastructure;
using VRA.Common.Enums;
using VRA.Core.Interfaces;
using VRA.Core.Models;

namespace VRA.App.ViewModels;

public partial class HearingTestViewModel : ObservableObject
{
    private readonly CurrentPatientService _currentPatientService;
    private readonly IHearingTestSessionService _sessionService;
    private readonly CurrentHearingTestSessionService _currentSessionService;


    [ObservableProperty]
    private Patient? currentPatient;

    [ObservableProperty]
    private HearingTestSession? currentSession;

    [ObservableProperty]
    private string sessionStatusText = "Not started";

    [ObservableProperty]
    private string statusMessage = "Ready to start a hearing test.";

    [ObservableProperty]
    private bool isStartingTest;



    public HearingTestViewModel(
        CurrentPatientService currentPatientService,
        IHearingTestSessionService sessionService,
        CurrentHearingTestSessionService currentSessionService)
    {
        _currentPatientService = currentPatientService;
        _sessionService = sessionService;
        _currentSessionService = currentSessionService;


        CurrentPatient = _currentPatientService.CurrentPatient;
        CurrentSession = _currentSessionService.CurrentSession;

        if (CurrentSession is not null)
        {
            SessionStatusText = CurrentSession.Status switch
            {
                HearingTestStatus.InProgress => "In Progress",
                HearingTestStatus.Completed => "Completed",
                HearingTestStatus.Cancelled => "Cancelled",
                _ => "Not started"
            };
        }
        _currentSessionService.SessionChanged += session =>
        {
            CurrentSession = session;

            SessionStatusText = session?.Status switch
            {
                HearingTestStatus.InProgress => "In Progress",
                HearingTestStatus.Completed => "Completed",
                HearingTestStatus.Cancelled => "Cancelled",
                _ => "Not started"
            };
        };

        _currentPatientService.CurrentPatientChanged += patient =>
        {
            CurrentPatient = patient;
        };
    }

    private bool CanStartTest()
    {
        return CurrentPatient is not null
            && CurrentSession?.Status != HearingTestStatus.InProgress
            && !IsStartingTest;
    }

    [RelayCommand(CanExecute = nameof(CanStartTest))]
    private async Task StartTestAsync()
    {
        if (CurrentPatient is null)
        {
            StatusMessage = "Please select a patient before starting a hearing test.";
            return;
        }

        try
        {
            IsStartingTest = true;
            StatusMessage = "Creating hearing test session...";

            var session = await _sessionService.CreateAsync(CurrentPatient.Id);

            CurrentSession = session;
            _currentSessionService.SetSession(session);
            SessionStatusText = "In Progress";

            StatusMessage =
                $"Hearing test session started for {CurrentPatient.FullName}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Unable to start hearing test: {ex.Message}";
        }
        finally
        {
            IsStartingTest = false;
        }
    }

    private bool CanFinishTest()
    {
        return CurrentSession?.Status == HearingTestStatus.InProgress
            && !IsStartingTest;
    }

    [RelayCommand(CanExecute = nameof(CanFinishTest))]
    private async Task CompleteTestAsync()
    {
        await FinishTestAsync(HearingTestStatus.Completed);
    }


    [RelayCommand(CanExecute = nameof(CanFinishTest))]
    private async Task CancelTestAsync()
    {
        var result = System.Windows.MessageBox.Show(
            "Are you sure you want to cancel this hearing test?\n\n" +
            "The session will be saved with Cancelled status.",
            "Confirm Cancellation",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning,
            System.Windows.MessageBoxResult.No);

        if (result != System.Windows.MessageBoxResult.Yes)
            return;

        await FinishTestAsync(HearingTestStatus.Cancelled);
    }



    private async Task FinishTestAsync(HearingTestStatus finalStatus)
    {
        if (CurrentSession is null ||
            CurrentSession.Status != HearingTestStatus.InProgress)
        {
            return;
        }

        var session = CurrentSession;

        try
        {
            IsStartingTest = true;

            StatusMessage = finalStatus == HearingTestStatus.Completed
                ? "Completing hearing test..."
                : "Cancelling hearing test...";

            session.Status = finalStatus;
            session.CompletedAt = DateTime.Now;

            await _sessionService.UpdateAsync(session);

            // Refresh the session object so WPF updates its bindings.
            CurrentSession = new HearingTestSession
            {
                Id = session.Id,
                PatientId = session.PatientId,
                Patient = session.Patient,
                StartedAt = session.StartedAt,
                CompletedAt = session.CompletedAt,
                Status = session.Status,
                Notes = session.Notes
            };

            SessionStatusText = finalStatus switch
            {
                HearingTestStatus.InProgress => "In Progress",
                HearingTestStatus.Completed => "Completed",
                HearingTestStatus.Cancelled => "Cancelled",
                _ => finalStatus.ToString()
            };
            _currentSessionService.RefreshSession(CurrentSession);
            StatusMessage = finalStatus == HearingTestStatus.Completed
                ? "Hearing test completed successfully."
                : "Hearing test cancelled.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Unable to update hearing test: {ex.Message}";
        }
        finally
        {
            IsStartingTest = false;
        }
    }

    

    partial void OnCurrentPatientChanged(Patient? value)
    {
        RefreshCommands();
    }

    partial void OnCurrentSessionChanged(HearingTestSession? value)
    {
        RefreshCommands();
    }

    partial void OnIsStartingTestChanged(bool value)
    {
        RefreshCommands();
    }

    private void RefreshCommands()
    {
        StartTestCommand.NotifyCanExecuteChanged();
        CompleteTestCommand.NotifyCanExecuteChanged();
        CancelTestCommand.NotifyCanExecuteChanged();
    }


}
