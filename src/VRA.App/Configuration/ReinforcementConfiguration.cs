
namespace VRA.App.Configuration;

public class ReinforcementConfiguration
{
    public List<DisplayOutputConfiguration> Displays { get; set; } = [];

    public AudioOutputConfiguration Audio { get; set; } = new();
}

public class DisplayOutputConfiguration
{
    public string Name { get; set; } = string.Empty;

    public int? MonitorNumber { get; set; }
}

public class AudioOutputConfiguration
{
    public int? DeviceNumber { get; set; }

    public int ChannelCount { get; set; } = 4;
}
