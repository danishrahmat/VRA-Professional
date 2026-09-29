using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using VRA.App.Infrastructure.Dialogs;
using VRA.Core.Interfaces;
using VRA.Core.Models;

namespace VRA.App.ViewModels;

public partial class PatientsViewModel : ObservableObject
{
    private readonly IPatientService _patientService;
    private readonly IDialogService _dialogService;

    public ObservableCollection<Patient> Patients { get; }
        = new();

    public ICollectionView PatientsView { get; }

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private Patient? selectedPatient;

    public PatientsViewModel(
        IPatientService patientService,
        IDialogService dialogService)
    {
        _patientService = patientService;
        _dialogService = dialogService;

        PatientsView = CollectionViewSource.GetDefaultView(Patients);
        PatientsView.Filter = FilterPatient;

        _ = LoadPatientsAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        PatientsView.Refresh();
    }

    private bool FilterPatient(object obj)
    {
        if (obj is not Patient patient)
            return false;

        if (string.IsNullOrWhiteSpace(SearchText))
            return true;

        var search = SearchText.Trim();

        return
            patient.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            patient.IcPassportNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            patient.ParentGuardian.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            patient.Diagnosis.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    private async Task LoadPatientsAsync()
    {
        Patients.Clear();

        var patients = await _patientService.GetAllAsync();

        foreach (var patient in patients)
        {
            Patients.Add(patient);
        }

        PatientsView.Refresh();
    }

    [RelayCommand]
    private async Task NewPatientAsync()
    {
        var result = _dialogService.ShowNewPatientDialog();

        if (result == true)
        {
            await LoadPatientsAsync();
        }
    }

    [RelayCommand(CanExecute = nameof(CanEditPatient))]
    private void EditPatient()
    {
        if (SelectedPatient is null)
            return;

        // We will open the Edit Patient dialog here next.
    }

    private bool CanEditPatient()
    {
        return SelectedPatient is not null;
    }

    partial void OnSelectedPatientChanged(Patient? value)
    {
        EditPatientCommand.NotifyCanExecuteChanged();
    }
}