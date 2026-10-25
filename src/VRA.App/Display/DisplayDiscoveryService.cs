
using System.Runtime.InteropServices;

namespace VRA.App.Display;

public class DisplayDiscoveryService
{
    private const int ENUM_CURRENT_SETTINGS = -1;
    private const int DISPLAY_DEVICE_PRIMARY_DEVICE = 0x4;

    public IReadOnlyList<DisplayInfo> GetDisplays()
    {
        var displays = new List<DisplayInfo>();

        for (uint index = 0; ; index++)
        {
            var device = new DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf<DISPLAY_DEVICE>();

            if (!EnumDisplayDevices(
                    null, index, ref device, 0))
            {
                break;
            }

            // Ignore inactive display adapters.
            if ((device.StateFlags & 0x1) == 0)
                continue;

            var settings = new DEVMODE();
            settings.dmSize = (ushort)Marshal.SizeOf<DEVMODE>();

            if (!EnumDisplaySettings(
                    device.DeviceName,
                    ENUM_CURRENT_SETTINGS,
                    ref settings))
            {
                continue;
            }

            var monitorInfo = new MONITORINFOEX();
            monitorInfo.cbSize = Marshal.SizeOf<MONITORINFOEX>();

            bool isPrimary =
                (device.StateFlags & DISPLAY_DEVICE_PRIMARY_DEVICE) != 0;

            displays.Add(new DisplayInfo
            {
                DeviceName = device.DeviceName,
                DisplayName = string.IsNullOrWhiteSpace(device.DeviceString)
                    ? device.DeviceName
                    : device.DeviceString,
                Width = (int)settings.dmPelsWidth,
                Height = (int)settings.dmPelsHeight,
                IsPrimary = isPrimary,

                PositionX = settings.dmPositionX,
                PositionY = settings.dmPositionY
            });
        }

        return displays;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool EnumDisplayDevices(
        string? lpDevice,
        uint iDevNum,
        ref DISPLAY_DEVICE lpDisplayDevice,
        uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool EnumDisplaySettings(
        string deviceName,
        int modeNum,
        ref DEVMODE devMode);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DISPLAY_DEVICE
    {
        public int cb;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;

        public int StateFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;

        public ushort dmSpecVersion;
        public ushort dmDriverVersion;
        public ushort dmSize;
        public ushort dmDriverExtra;
        public uint dmFields;

        public int dmPositionX;
        public int dmPositionY;
        public uint dmDisplayOrientation;
        public uint dmDisplayFixedOutput;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;

        public ushort dmLogPixels;
        public uint dmBitsPerPel;
        public uint dmPelsWidth;
        public uint dmPelsHeight;
        public uint dmDisplayFlags;
        public uint dmDisplayFrequency;

        public uint dmICMMethod;
        public uint dmICMIntent;
        public uint dmMediaType;
        public uint dmDitherType;
        public uint dmReserved1;
        public uint dmReserved2;
        public uint dmPanningWidth;
        public uint dmPanningHeight;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MONITORINFOEX
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDevice;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
