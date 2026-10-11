
using VRA.Core.Models;

namespace VRA.Core.Interfaces;

public interface IPatientService
{
    Task<List<Patient>> GetAllAsync();

    Task<Patient?> GetByIdAsync(Guid id);

    Task AddAsync(Patient patient);

    Task UpdateAsync(Patient patient);

    Task DeleteAsync(Guid id);

    Task<bool> MedicalRecordNumberExistsAsync(
        string medicalRecordNumber,
        Guid? excludePatientId = null);

    Task<bool> IcPassportNumberExistsAsync(
        string icPassportNumber,
        Guid? excludePatientId = null);
}
