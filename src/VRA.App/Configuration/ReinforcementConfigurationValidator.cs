
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
            throw new ReinforcementConfigurationException(
                "The Displays list is required.");
        }

        var names = configuration.Displays
            .Select(display => display?.Name?.Trim() ?? string.Empty)
            .ToList();

        if (names.Any(string.IsNullOrWhiteSpace))
        {
            throw new ReinforcementConfigurationException(
                "Every display must have a name.");
        }

        if (names.Distinct(StringComparer.OrdinalIgnoreCase).Count()
            != names.Count)
        {
            throw new ReinforcementConfigurationException(
                "Display names must be unique.");
        }

        foreach (string requiredName in RequiredDisplays)
        {
            if (!names.Contains(
                    requiredName,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new ReinforcementConfigurationException(
                    $"Required logical display '{requiredName}' is missing.");
            }
        }

        if (configuration.Audio is null)
        {
            throw new ReinforcementConfigurationException(
                "The Audio configuration is required.");
        }


        if (configuration.Audio.ChannelCount is < 1 or > 32)
        {
            throw new ReinforcementConfigurationException(
                "Audio ChannelCount must be between 1 and 32.");
        }


        if (configuration.Displays.Any(display =>
                display.MonitorNumber is not null &&
                display.MonitorNumber < 1))
        {
            throw new ReinforcementConfigurationException(
                "MonitorNumber must be positive when specified.");
        }

        if (configuration.Audio.DeviceNumber is not null &&
            configuration.Audio.DeviceNumber < 0)
        {
            throw new ReinforcementConfigurationException(
                "Audio DeviceNumber cannot be negative when specified.");
        }
    }
}
