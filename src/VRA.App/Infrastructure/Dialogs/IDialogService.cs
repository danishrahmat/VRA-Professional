using VRA.Core.Models;

namespace VRA.App.Infrastructure.Dialogs;

public interface IDialogService
{
    bool? ShowNewPatientDialog();
    bool? ShowEditPatientDialog(Patient patient);
}