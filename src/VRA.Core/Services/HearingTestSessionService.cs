
using Microsoft.EntityFrameworkCore;
using VRA.Core.Data;
using VRA.Core.Interfaces;
using VRA.Core.Models;
using VRA.Common.Enums;

namespace VRA.Core.Services;

public class HearingTestSessionService : IHearingTestSessionService
{
    private readonly VraDbContext _context;

    public HearingTestSessionService(VraDbContext context)
    {
        _context = context;
    }

    public async Task<HearingTestSession> CreateAsync(Guid patientId)
    {
        var patientExists = await _context.Patients
            .AnyAsync(patient => patient.Id == patientId);

        if (!patientExists)
        {
            throw new InvalidOperationException(
                "Cannot create a hearing test session for a patient that does not exist.");
        }

        var session = new HearingTestSession
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            StartedAt = DateTime.Now,
            Status = HearingTestStatus.InProgress,
            Notes = string.Empty
        };

        _context.HearingTestSessions.Add(session);
        await _context.SaveChangesAsync();

        return session;
    }

    public async Task<HearingTestSession?> GetByIdAsync(Guid id)
    {
        return await _context.HearingTestSessions
            .Include(session => session.Patient)
            .FirstOrDefaultAsync(session => session.Id == id);
    }

    public async Task<List<HearingTestSession>> GetByPatientIdAsync(Guid patientId)
    {
        return await _context.HearingTestSessions
            .Where(session => session.PatientId == patientId)
            .OrderByDescending(session => session.StartedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(HearingTestSession session)
    {
        var existingSession = await _context.HearingTestSessions
            .FirstOrDefaultAsync(item => item.Id == session.Id);

        if (existingSession is null)
        {
            throw new InvalidOperationException(
                "The hearing test session could not be found.");
        }

        existingSession.Status = session.Status;
        existingSession.CompletedAt = session.CompletedAt;
        existingSession.Notes = session.Notes;

        await _context.SaveChangesAsync();
    }
}
