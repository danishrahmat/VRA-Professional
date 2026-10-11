using System.Windows.Controls;
using VRA.App.ViewModels;

namespace VRA.App.Views.Pages;

public partial class PatientDetailsView : UserControl
{
    public PatientDetailsView(PatientDetailsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}