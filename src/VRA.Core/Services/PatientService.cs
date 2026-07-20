using Microsoft.EntityFrameworkCore;
using VRA.Core.Data;
using VRA.Core.Interfaces;
using VRA.Core.Models;

namespace VRA.Core.Services;

public class PatientService : IPatientService
{
    private readonly VraDbContext _context;

    public PatientService(VraDbContext context)
    {
        _context = context;
    }

    public async Task<List<Patient>> GetAllAsync()
    {
        return await _context.Patients
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        return await _context.Patients.FindAsync(id);
    }

    public async Task AddAsync(Patient patient)
    {
        patient.CreatedAt = DateTime.Now;
        patient.UpdatedAt = DateTime.Now;

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Patient patient)
    {
        patient.UpdatedAt = DateTime.Now;

        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
            return;

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
    }
}