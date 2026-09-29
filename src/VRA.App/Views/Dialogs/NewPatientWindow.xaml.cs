using VRA.App.ViewModels;

namespace VRA.App.Views.Dialogs;

public partial class NewPatientWindow
{
    private readonly NewPatientViewModel _viewModel;

    public NewPatientWindow(NewPatientViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        DataContext = viewModel;

        Content = new NewPatientView
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