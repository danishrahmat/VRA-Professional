
namespace VRA.App.Services;

public class SimulatedReinforcementService : IReinforcementService
{
    private static readonly string[] ValidDisplays =
        ["Left", "Centre", "Right"];

    public void ShowAnimation(string display)
    {
        ValidateDisplay(display);

        System.Diagnostics.Debug.WriteLine(
            $"[SIMULATION] Show animation on {display} display.");
    }

    public void PlaySound(string display)
    {
        ValidateDisplay(display);

        System.Diagnostics.Debug.WriteLine(
            $"[SIMULATION] Play sound for {display} display.");
    }

    public void StopAll()
    {
        System.Diagnostics.Debug.WriteLine(
            "[SIMULATION] Stop all reinforcement outputs.");
    }

    private static void ValidateDisplay(string display)
    {
        if (!ValidDisplays.Contains(display))
        {
            throw new ArgumentException(
                $"Invalid display '{display}'. Use Left, Centre, or Right.",
                nameof(display));
        }
    }
}
