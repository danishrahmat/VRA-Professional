using System.Windows;

using Microsoft.Extensions.DependencyInjection;
using VRA.App.Views.Dialogs;
using VRA.App.ViewModels;
using VRA.Core.Interfaces;
using VRA.Core.Models;
namespace VRA.App.Infrastructure.Dialogs;

public class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;

    public DialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool? ShowNewPatientDialog()
    {
        var window = _serviceProvider
            .GetRequiredService<NewPatientWindow>();

        window.Owner = Application.Current.MainWindow;

        return window.ShowDialog();
    }

    public bool? ShowEditPatientDialog(Patient patient)
    {
        var patientService =
            _serviceProvider.GetRequiredService<IPatientService>();

        var viewModel = new EditPatientViewModel(
            patientService,
            patient);

        var window = new EditPatientWindow(viewModel);

        window.Owner = Application.Current.MainWindow;

        return window.ShowDialog();
    }

    public bool ConfirmDeletePatient(string patientName)
    {
        var result = MessageBox.Show(
            $"Are you sure you want to delete patient '{patientName}'?\n\n" +
            "This action cannot be undone.",
            "Confirm Delete Patient",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        return result == MessageBoxResult.Yes;
    }
}