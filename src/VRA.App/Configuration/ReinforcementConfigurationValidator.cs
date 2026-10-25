
namespace VRA.App.Configuration;

public static class ReinforcementConfigurationValidator
{
    private static readonly string[] RequiredDisplays =
        ["Left", "Centre", "Right"];

    public static void Validate(ReinforcementConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (configuration.Displays is null)
        {
            throw new InvalidOperationException(
                "The Displays list is required.");
        }

        var names = configuration.Displays
            .Select(display => display?.Name?.Trim() ?? string.Empty)
            .ToList();

        if (names.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidOperationException(
                "Every display must have a name.");
        }

        if (names.Distinct(StringComparer.OrdinalIgnoreCase).Count()
            != names.Count)
        {
            throw new InvalidOperationException(
                "Display names must be unique.");
        }

        foreach (string requiredName in RequiredDisplays)
        {
            if (!names.Contains(
                    requiredName,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Required logical display '{requiredName}' is missing.");
            }
        }

        if (configuration.Audio is null)
        {
            throw new InvalidOperationException(
                "The Audio configuration is required.");
        }

        if (configuration.Audio.ChannelCount < 1)
        {
            throw new InvalidOperationException(
                "Audio ChannelCount must be greater than zero.");
        }

        if (configuration.Displays.Any(display =>
                display.MonitorNumber is not null &&
                display.MonitorNumber < 1))
        {
            throw new InvalidOperationException(
                "MonitorNumber must be positive when specified.");
        }

        if (configuration.Audio.DeviceNumber is not null &&
            configuration.Audio.DeviceNumber < 0)
        {
            throw new InvalidOperationException(
                "Audio DeviceNumber cannot be negative when specified.");
        }
    }
}
