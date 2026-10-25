
using VRA.App.Configuration;

namespace VRA.App.Services;

public class SimulatedReinforcementService : IReinforcementService
{
    private readonly ReinforcementConfiguration _configuration;

    public SimulatedReinforcementService(
        ReinforcementConfiguration configuration)
    {
        _configuration = configuration;
    }

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

    private void ValidateDisplay(string display)
    {
        bool isConfigured = _configuration.Displays.Any(
            item => string.Equals(
                item.Name,
                display,
                StringComparison.OrdinalIgnoreCase));

        if (!isConfigured)
        {
            throw new ArgumentException(
                $"Display '{display}' is not configured.",
                nameof(display));
        }
    }
}
