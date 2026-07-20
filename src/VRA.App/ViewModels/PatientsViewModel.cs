using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using VRA.Core.Interfaces;
using VRA.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace VRA.App.ViewModels;

public partial class PatientsViewModel : ObservableObject
{
    private readonly IPatientService _patientService;

    public ObservableCollection<Patient> Patients { get; }
        = new();

    public PatientsViewModel(IPatientService patientService)
    {
        _patientService = patientService;

        _ = LoadPatientsAsync();
    }

    private async Task LoadPatientsAsync()
    {
        Patients.Clear();

        var patients = await _patientService.GetAllAsync();

        foreach (var patient in patients)
        {
            Patients.Add(patient);
        }
    }
}