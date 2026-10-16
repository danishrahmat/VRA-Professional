
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRA.App.Infrastructure;
using VRA.Core.Models;

namespace VRA.App.ViewModels;

public partial class HearingTestViewModel : ObservableObject
{
    private readonly CurrentPatientService _currentPatientService;

    [ObservableProperty]
    private Patient? currentPatient;

    [ObservableProperty]
    private string statusMessage = "Ready to start a hearing test.";

    public HearingTestViewModel(
        CurrentPatientService currentPatientService)
    {
        _currentPatientService = currentPatientService;

        CurrentPatient = _currentPatientService.CurrentPatient;

        _currentPatientService.CurrentPatientChanged += patient =>
        {
            CurrentPatient = patient;
        };
    }


    [RelayCommand]
    private void StartTest()
    {
        if (CurrentPatient is null)
        {
            StatusMessage =
                "Please select a patient before starting a hearing test.";
            return;
        }

        StatusMessage =
            $"Ready to start a hearing test for {CurrentPatient.FullName}.";
    }
}
