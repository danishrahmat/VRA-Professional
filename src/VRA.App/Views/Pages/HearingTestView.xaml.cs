
using System.Windows.Controls;
using VRA.App.ViewModels;

namespace VRA.App.Views.Pages;

public partial class HearingTestView : UserControl
{
    public HearingTestView(HearingTestViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
