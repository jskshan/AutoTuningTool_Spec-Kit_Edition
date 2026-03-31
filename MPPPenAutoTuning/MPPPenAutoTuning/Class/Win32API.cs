using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace MPPPenAutoTuning
{
    //Win32 API Declare
    public class Win32
    {
        // Message Signature & Mask
        public const uint MI_WP_SIGNATURE = 0xFF515700;
        public const uint SIGNATURE_MASK = 0xFFFFFF00;
        public const uint MI_WP_SIGNATURE_FROM_PEN = 0xFF515700;
        public const uint MI_WP_SIGNATURE_FROM_TOUCH = 0xFF515780;
        public const uint SIGNATURE_MASK_TOUCH_PEN = 0xFFFFFF80;

        // Type of Input Event Provider
        public const int INPUT_EVENT_PROVIDER_TOUCH = 2;
        public const int INPUT_EVENT_PROVIDER_PEN = 1;
        public const int INPUT_EVENT_PROVIDER_MOUSE = 0;

        // Event Code
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;

        public const int LOCALE_SDECIMAL = 0x0000000E;
        public const int LOCALE_USER_DEFAULT = 0x0400;
        public const int LOCALE_SYSTEM_DEFAULT = 0x0800;

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint MM_BeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint MM_EndPeriod(uint uMilliseconds);

        [DllImport("user32.dll")]
        public static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetLocaleInfo(uint Locale, uint LCType, [Out] StringBuilder lpLCData, int cchData);

        //宣告與ini讀寫相關API宣告
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("user32")]
        public static extern uint GetMessageExtraInfo();

        [DllImport("user32")]
        public static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(
              string deviceName, int modeNum, ref DEVMODE devMode);
        public const int ENUM_CURRENT_SETTINGS = -1;

        public const int ENUM_REGISTRY_SETTINGS = -2;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ChangeDisplaySettings([In, Out]ref DEVMODE lpDevMode, [param: MarshalAs(UnmanagedType.U4)]uint dwflags);
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_BADMODE = -2;
        public const int DISP_CHANGE_FAILED = -1;
        public const int DISP_CHANGE_RESTART = 1;
    }
}
