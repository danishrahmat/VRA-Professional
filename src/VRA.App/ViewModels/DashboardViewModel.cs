using CommunityToolkit.Mvvm.ComponentModel;
using VRA.App.Infrastructure;
using VRA.Core.Models;

namespace VRA.App.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly CurrentPatientService _currentPatientService;

    [ObservableProperty]
    private Patient? currentPatient;

    public DashboardViewModel(CurrentPatientService currentPatientService)
    {
        _currentPatientService = currentPatientService;

        // Get the patient that is already selected
        CurrentPatient = _currentPatientService.CurrentPatient;

        // Listen for future patient changes
        _currentPatientService.CurrentPatientChanged += patient =>
        {
            CurrentPatient = patient;
        };
    }
}