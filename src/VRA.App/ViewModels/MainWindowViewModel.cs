using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using VRA.App.Views;
using VRA.App.Views.Pages;
using VRA.App.Infrastructure.Navigation;
//using VRA.App.Infrastructure.Patients;
using VRA.App.Infrastructure;
using VRA.Core.Models;

namespace VRA.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly NavigationService _navigation;
    private readonly CurrentPatientService _currentPatientService;
    public SidebarViewModel Sidebar { get; }

    [ObservableProperty]
    private string title = "VRA Professional";

    [ObservableProperty]
    private string status = "System Ready";

    [ObservableProperty]
    private Patient? currentPatient;

    [ObservableProperty]
    private UserControl currentView;



    public MainWindowViewModel(
        NavigationService navigation,
        SidebarViewModel sidebar,
        CurrentPatientService currentPatientService)
    {
        _navigation = navigation;
        Sidebar = sidebar;

        _currentPatientService = currentPatientService;

        _currentPatientService.CurrentPatientChanged += patient =>
        {
            CurrentPatient = patient;
        };

        _navigation.ViewChanged += view =>
        {
            CurrentView = view;
        };

        _navigation.Navigate<DashboardView>();
    }
}