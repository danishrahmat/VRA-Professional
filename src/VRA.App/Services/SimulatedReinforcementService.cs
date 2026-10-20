
namespace VRA.App.Services;

public class SimulatedReinforcementService : IReinforcementService
{
    public void ShowAnimation(string display)
    {
        System.Diagnostics.Debug.WriteLine(
            $"[SIMULATION] Show animation on {display} display.");
    }

    public void PlaySound(string display)
    {
        System.Diagnostics.Debug.WriteLine(
            $"[SIMULATION] Play sound for {display} display.");
    }

    public void StopAll()
    {
        System.Diagnostics.Debug.WriteLine(
            "[SIMULATION] Stop all reinforcement outputs.");
    }
}
