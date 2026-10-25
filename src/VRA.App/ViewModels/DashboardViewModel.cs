
using CommunityToolkit.Mvvm.ComponentModel;
using VRA.App.Display;
using VRA.App.Infrastructure;
using VRA.Core.Models;

namespace VRA.App.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly CurrentPatientService _currentPatientService;
    private readonly DisplayDiscoveryService _displayDiscoveryService;

    [ObservableProperty]
    private Patient? currentPatient;

    [ObservableProperty]
    private int connectedDisplayCount;

    public string DisplayCountText => $"{ConnectedDisplayCount} / 3";

    public DashboardViewModel(
        CurrentPatientService currentPatientService,
        DisplayDiscoveryService displayDiscoveryService)
    {
        _currentPatientService = currentPatientService;
        _displayDiscoveryService = displayDiscoveryService;

        // Get the patient that is already selected.
        CurrentPatient = _currentPatientService.CurrentPatient;

        // Listen for future patient changes.
        _currentPatientService.CurrentPatientChanged += patient =>
        {
            CurrentPatient = patient;
        };

        RefreshDisplayCount();
    }

    public void RefreshDisplayCount()
    {
        ConnectedDisplayCount = _displayDiscoveryService.GetDisplays().Count;
        OnPropertyChanged(nameof(DisplayCountText));
    }
}
