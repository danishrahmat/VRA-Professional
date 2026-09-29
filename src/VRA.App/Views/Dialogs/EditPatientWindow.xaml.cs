using VRA.App.ViewModels;

namespace VRA.App.Views.Dialogs;

public partial class EditPatientWindow
{
    private readonly EditPatientViewModel _viewModel;

    public EditPatientWindow(EditPatientViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        DataContext = viewModel;

        Content = new EditPatientView
        {
            DataContext = viewModel
        };

        _viewModel.RequestClose += OnRequestClose;
    }

    private void OnRequestClose(bool? result)
    {
        DialogResult = result;
    }
}