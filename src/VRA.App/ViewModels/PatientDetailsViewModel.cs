
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRA.App.Infrastructure;
using VRA.App.Infrastructure.Dialogs;
using VRA.App.Infrastructure.Navigation;
using VRA.Core.Interfaces;
using VRA.Core.Models;

namespace VRA.App.ViewModels;

public partial class PatientDetailsViewModel : ObservableObject
{
    private readonly CurrentPatientService _currentPatientService;
    private readonly NavigationService _navigation;
    private readonly IDialogService _dialogService;
    private readonly IPatientService _patientService;

    [ObservableProperty]
    private Patient? currentPatient;

    public PatientDetailsViewModel(
        CurrentPatientService currentPatientService,
        NavigationService navigation,
        IDialogService dialogService,
        IPatientService patientService)
    {
        _currentPatientService = currentPatientService;
        _navigation = navigation;
        _dialogService = dialogService;
        _patientService = patientService;

        CurrentPatient = _currentPatientService.CurrentPatient;

        _currentPatientService.CurrentPatientChanged += patient =>
        {
            CurrentPatient = patient;
        };
    }

    [RelayCommand]
    private void Back()
    {
        _navigation.Navigate<Views.Pages.PatientsView>();
    }


    [RelayCommand(CanExecute = nameof(CanEditPatient))]
    private async Task EditPatientAsync()
    {
        if (CurrentPatient is null)
            return;

        var patientId = CurrentPatient.Id;

        var result = _dialogService.ShowEditPatientDialog(CurrentPatient);

        if (result != true)
            return;

        // Fetch the saved record from SQLite.
        var updatedPatient = await _patientService.GetByIdAsync(patientId);

        if (updatedPatient is null)
            return;

        // Update the shared patient first so other views receive the change.
        _currentPatientService.SetPatient(updatedPatient);

        // Update this page's displayed patient.
        CurrentPatient = updatedPatient;
    }


    private bool CanEditPatient()
    {
        return CurrentPatient is not null;
    }

    partial void OnCurrentPatientChanged(Patient? value)
    {
        EditPatientCommand.NotifyCanExecuteChanged();
    }
}
