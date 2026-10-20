using System.Windows.Controls;
using VRA.App.ViewModels;

namespace VRA.App.Views.Pages;

public partial class ReinforcementView : UserControl
{
    public ReinforcementView(ReinforcementViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}