using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using VRA.App.Views;
using VRA.App.Views.Pages;
using VRA.App.Infrastructure.Navigation;

namespace VRA.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly NavigationService _navigation;
    public SidebarViewModel Sidebar { get; }

    [ObservableProperty]
    private string title = "VRA Professional";

    [ObservableProperty]
    private string status = "System Ready";

    [ObservableProperty]
    private string currentPatient = "No patient selected";

    [ObservableProperty]
    private UserControl currentView;

    public MainWindowViewModel(
    NavigationService navigation,
    SidebarViewModel sidebar)
    {
        _navigation = navigation;

        Sidebar = sidebar;

        _navigation.ViewChanged += view =>
        {
            CurrentView = view;
        };

        _navigation.Navigate<DashboardView>();
    }
}