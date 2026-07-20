using VRA.Core.Models;

namespace VRA.Core.Interfaces;

public interface IPatientService
{
    Task<List<Patient>> GetAllAsync();

    Task<Patient?> GetByIdAsync(Guid id);

    Task AddAsync(Patient patient);

    Task UpdateAsync(Patient patient);

    Task DeleteAsync(Guid id);
}