
namespace VRA.App.Configuration;

public class ReinforcementConfigurationException : InvalidOperationException
{
    public ReinforcementConfigurationException(string message)
        : base(message)
    {
    }

    public ReinforcementConfigurationException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}
