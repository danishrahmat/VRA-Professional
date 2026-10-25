
using VRA.App.Configuration;

namespace VRA.App.Configuration;

public static class DefaultReinforcementConfiguration
{
    public static ReinforcementConfiguration Create()
    {
        return new ReinforcementConfiguration
        {
            Displays =
            [
                new DisplayOutputConfiguration
                {
                    Name = "Left",
                    MonitorNumber = null
                },
                new DisplayOutputConfiguration
                {
                    Name = "Centre",
                    MonitorNumber = null
                },
                new DisplayOutputConfiguration
                {
                    Name = "Right",
                    MonitorNumber = null
                }
            ],
            Audio = new AudioOutputConfiguration
            {
                DeviceNumber = null,
                ChannelCount = 4
            }
        };
    }
}
