using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRA.App.Infrastructure.Navigation;
using VRA.App.Views;
using VRA.App.Views.Pages;

namespace VRA.App.ViewModels;

public partial class SidebarViewModel : ObservableObject
{
    private readonly NavigationService _navigation;

    public SidebarViewModel(NavigationService navigation)
    {
        _navigation = navigation;
    }

    [RelayCommand]
    private void Dashboard()
    {
        _navigation.Navigate<DashboardView>();
    }

    [RelayCommand]
    private void Patients()
    {
        _navigation.Navigate<PatientsView>();
    }
}