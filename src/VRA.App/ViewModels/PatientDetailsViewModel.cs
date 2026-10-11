using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRA.App.Infrastructure;
using VRA.App.Infrastructure.Navigation;
using VRA.Core.Models;

namespace VRA.App.ViewModels;

public partial class PatientDetailsViewModel : ObservableObject
{
    private readonly CurrentPatientService _currentPatientService;
    private readonly NavigationService _navigation;

    [ObservableProperty]
    private Patient? currentPatient;

    public PatientDetailsViewModel(
        CurrentPatientService currentPatientService,
        NavigationService navigation)
    {
        _currentPatientService = currentPatientService;
        _navigation = navigation;

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
}