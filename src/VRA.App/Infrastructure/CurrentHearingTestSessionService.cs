
using VRA.Common.Enums;
using VRA.Core.Models;

namespace VRA.App.Infrastructure;

public class CurrentHearingTestSessionService
{
    public HearingTestSession? CurrentSession { get; private set; }

    public event Action<HearingTestSession?>? SessionChanged;

    public void SetSession(HearingTestSession session)
    {
        CurrentSession = session;
        SessionChanged?.Invoke(session);
    }

    public void ClearSession()
    {
        CurrentSession = null;
        SessionChanged?.Invoke(null);
    }

    public void RefreshSession(HearingTestSession session)
    {
        CurrentSession = session;
        SessionChanged?.Invoke(session);
    }
}
