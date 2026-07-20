using CommunityToolkit.Mvvm.ComponentModel;

namespace VRA.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "VRA Professional";

    [ObservableProperty]
    private string status = "System Ready";

    [ObservableProperty]
    private string currentPatient = "No patient selected";
}