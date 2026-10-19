
using VRA.Core.Models;

namespace VRA.Core.Interfaces;

public interface IHearingTestSessionService
{
    Task<HearingTestSession> CreateAsync(Guid patientId);

    Task<HearingTestSession?> GetByIdAsync(Guid id);

    Task<List<HearingTestSession>> GetByPatientIdAsync(Guid patientId);

    Task UpdateAsync(HearingTestSession session);
}
