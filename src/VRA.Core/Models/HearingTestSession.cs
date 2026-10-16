
using VRA.Common.Enums;

namespace VRA.Core.Models;

public class HearingTestSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Patient associated with this session
    public Guid PatientId { get; set; }

    public Patient? Patient { get; set; }

    // Session timing
    public DateTime StartedAt { get; set; } = DateTime.Now;

    public DateTime? CompletedAt { get; set; }

    // Current session status
    public HearingTestStatus Status { get; set; }
        = HearingTestStatus.InProgress;

    // Optional notes for the session
    public string Notes { get; set; } = string.Empty;
}
