
using System;
using System.Windows;
using VRA.App.Display;
using VRA.App.Views;

namespace VRA.App.Services;

public class ReinforcementWindowPositioningService
{
    public void PositionWindow(
        ReinforcementWindow window,
        DisplayInfo display)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(display);

        if (display.Width <= 0 || display.Height <= 0)
        {
            throw new InvalidOperationException(
                $"Invalid resolution for display {display.DeviceName}.");
        }

        window.WindowStartupLocation = WindowStartupLocation.Manual;
        window.WindowState = WindowState.Normal;

        window.Left = display.PositionX;
        window.Top = display.PositionY;
        window.Width = display.Width;
        window.Height = display.Height;
    }
}
