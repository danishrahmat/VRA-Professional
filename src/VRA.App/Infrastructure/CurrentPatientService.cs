using VRA.Core.Models;

//namespace VRA.App.Infrastructure.Patients;
namespace VRA.App.Infrastructure;

public class CurrentPatientService
{
    private Patient? _currentPatient;

    public Patient? CurrentPatient
    {
        get => _currentPatient;
        private set
        {
            _currentPatient = value;
            CurrentPatientChanged?.Invoke(value);
        }
    }

    public event Action<Patient?>? CurrentPatientChanged;

    public void SetPatient(Patient patient)
    {
        CurrentPatient = patient;
    }

    public void ClearPatient()
    {
        CurrentPatient = null;
    }
}