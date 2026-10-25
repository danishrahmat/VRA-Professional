
namespace VRA.App.Display;

public class DisplayInfo
{
    public string DeviceName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsPrimary { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }

    public string DisplayLabel =>
        $"{DeviceName.Replace(@"\\.\", "")} — {Width} × {Height}" +
        (IsPrimary ? " (Primary)" : "");

    public override string ToString()
    {
        return DisplayLabel;
    }
}
