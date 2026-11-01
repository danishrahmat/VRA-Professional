
using System.Windows.Controls;
using VRA.App.ViewModels;

namespace VRA.App.Views.Pages;

public partial class AudioSettingsView : UserControl
{
    public AudioSettingsView(AudioSettingsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
