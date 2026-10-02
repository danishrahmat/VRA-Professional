using VRA.App.ViewModels;
using System.Windows.Controls;

namespace VRA.App.Views;

public partial class DashboardView : UserControl
{
    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}