



namespace VRA.App.Display;

public class DisplayDiscoveryService
{
    public IReadOnlyList<DisplayInfo> GetDisplays()
    {
        // Windows display discovery will be implemented
        // using native Windows APIs in the next step.
        return Array.Empty<DisplayInfo>();
    }
}
