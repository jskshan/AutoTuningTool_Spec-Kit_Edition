using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Security;

namespace FingerAutoTuning
{
    /*
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
        public static extern int ChangeDisplaySettings([In, Out]ref DEVMODE lpDevMode,
                                                        [param: MarshalAs(UnmanagedType.U4)]
                                                        uint dwflags);
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_BADMODE = -2;
        public const int DISP_CHANGE_FAILED = -1;
        public const int DISP_CHANGE_RESTART = 1;
    }
    */

    class Win32
    {
        public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const uint MOUSEEVENTF_LEFTUP = 0x0004;
        public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        public const uint MOUSEEVENTF_MOVE = 0x0001;
        public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        public const uint MOUSEEVENTF_XDOWN = 0x0080;
        public const uint MOUSEEVENTF_XUP = 0x0100;
        public const uint MOUSEEVENTF_WHEEL = 0x0800;
        public const uint MOUSEEVENTF_HWHEEL = 0x01000;

        public const int LOCALE_SDECIMAL = 0x0000000E;
        public const int LOCALE_USER_DEFAULT = 0x0400;
        public const int LOCALE_SYSTEM_DEFAULT = 0x0800;

        public const uint HWND_BROADCAST = 0xFFFF;
        public const uint WM_USER = 0x0400;

        //Power Status
        public const uint WM_POWERBROADCAST = 0x218;
        public const uint PBT_APMSUSPEND = 0x04;
        public const uint PBT_APMRESUMEAUTOMATIC = 0x12;
        public const uint WM_QUERYENDSESSION = 0x11;

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
        public const int WM_POINTERUPDATE = 0x0245;
        public const int WM_POINTERDOWN = 0x0246;

        /// <summary>
        /// Serves as a standard header for information related to a device event reported through the WM_DEVICECHANGE message.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HDR
        {
            public uint dbch_Size;
            public uint dbch_DeviceType;
            public uint dbch_Reserved;
        }

        /// <summary>
        /// Contains information about a class of devices.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string dbcc_name;
        }

        #region Constants
        /// <summary>Windows message sent when a device is inserted or removed</summary>
        public const int WM_DEVICECHANGE = 0x0219;
        /// <summary>WParam for above : A device was inserted</summary>
        public const int DEVICE_ARRIVAL = 0x8000;
        /// <summary>WParam for above : A device was removed</summary>
        public const int DEVICE_REMOVECOMPLETE = 0x8004;
        /// <summary>Used in SetupDiClassDevs to get devices present in the system</summary>
        public const int DIGCF_PRESENT = 0x02;
        /// <summary>Used in SetupDiClassDevs to get device interface details</summary>
        public const int DIGCF_DEVICEINTERFACE = 0x10;
        /// <summary>Used when registering for device insert/remove messages : specifies the type of device</summary>
        public const int DEVTYP_DEVICEINTERFACE = 0x05;
        /// <summary>Used when registering for device insert/remove messages : we're giving the API call a window handle</summary>
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        /// <summary>Purges Win32 transmit buffer by aborting the current transmission.</summary>
        public const uint PURGE_TXABORT = 0x01;
        /// <summary>Purges Win32 receive buffer by aborting the current receive.</summary>
        public const uint PURGE_RXABORT = 0x02;
        /// <summary>Purges Win32 transmit buffer by clearing it.</summary>
        public const uint PURGE_TXCLEAR = 0x04;
        /// <summary>Purges Win32 receive buffer by clearing it.</summary>
        public const uint PURGE_RXCLEAR = 0x08;
        /// <summary>CreateFile : Open file for read</summary>
        public const uint GENERIC_READ = 0x80000000;
        /// <summary>CreateFile : Open file for write</summary>
        public const uint GENERIC_WRITE = 0x40000000;
        /// <summary>CreateFile : file share for write</summary>
        public const uint FILE_SHARE_WRITE = 0x2;
        /// <summary>CreateFile : file share for read</summary>
        public const uint FILE_SHARE_READ = 0x1;
        /// <summary>CreateFile : Open handle for overlapped operations</summary>
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        //20120529
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        /// <summary>CreateFile : Resource to be "created" must exist</summary>
        public const uint OPEN_EXISTING = 3;
        /// <summary>CreateFile : Resource will be "created" or existing will be used</summary>
        public const uint OPEN_ALWAYS = 4;
        /// <summary>ReadFile/WriteFile : Overlapped operation is incomplete.</summary>
        public const uint ERROR_IO_PENDING = 997;
        /// <summary>Infinite timeout</summary>
        public const uint INFINITE = 0xFFFFFFFF;
        /// <summary>Simple representation of a null handle : a closed stream will get this handle. Note it is public for comparison by higher level classes.</summary>
        public static IntPtr NullHandle = IntPtr.Zero;
        /// <summary>Simple representation of the handle returned when CreateFile fails.</summary>
        public static IntPtr InvalidHandleValue = new IntPtr(-1);

        #endregion

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// Used when registering a window to receive messages about devices added or removed from the system.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public class DeviceBroadcastInterface
        {
            public int Size;
            public int DeviceType;
            public int Reserved;
            public Guid ClassGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Name;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Registers a window for device insert/remove messages
        /// </summary>
        /// <param name="hwnd">Handle to the window that will receive the messages</param>
        /// <param name="oInterface">DeviceBroadcastInterrface structure</param>
        /// <param name="nFlags">set to DEVICE_NOTIFY_WINDOW_HANDLE</param>
        /// <returns>A handle used when unregistering</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hwnd, DeviceBroadcastInterface oInterface, uint nFlags);

        /// <summary>
        /// Unregister from above.
        /// </summary>
        /// <param name="hHandle">Handle returned in call to RegisterDeviceNotification</param>
        /// <returns>True if success</returns>
        [DllImport("user32.dll", SetLastError = true)]
        protected static extern bool UnregisterDeviceNotification(IntPtr hHandle);

        /// <summary>
        /// Gets the GUID that Windows uses to represent HID class devices
        /// </summary>
        /// <param name="gHid">An out parameter to take the Guid</param>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern void HidD_GetHidGuid(out Guid gHid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("WtsApi32.dll")]
        private static extern bool WTSRegisterSessionNotification(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)]int dwFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("WtsApi32.dll")]
        private static extern bool WTSUnRegisterSessionNotification(IntPtr hWnd);

        public const int NOTIFY_FOR_THIS_SESSION = 0;
        public const int WM_WTSSESSION_CHANGE = 0x2b1;

        // WParam values that can be received: 
        public const int WTS_CONSOLE_CONNECT = 0x1; // A session was connected to the console terminal.
        public const int WTS_CONSOLE_DISCONNECT = 0x2; // A session was disconnected from the console terminal.
        public const int WTS_REMOTE_CONNECT = 0x3; // A session was connected to the remote terminal.
        public const int WTS_REMOTE_DISCONNECT = 0x4; // A session was disconnected from the remote terminal.
        public const int WTS_SESSION_LOGON = 0x5; // A user has logged on to the session.
        public const int WTS_SESSION_LOGOFF = 0x6; // A user has logged off the session.
        public const int WTS_SESSION_LOCK = 0x7; // A session has been locked.
        public const int WTS_SESSION_UNLOCK = 0x8; // A session has been unlocked.
        public const int WTS_SESSION_REMOTE_CONTROL = 0x9; // A session has changed its remote controlled status.

        public static bool RegisterWTS(IntPtr Handle)
        {
            if (!WTSRegisterSessionNotification(Handle, NOTIFY_FOR_THIS_SESSION))
                return false;

            return true;
        }

        public static void UnregisterWTS(IntPtr Handle)
        {
            // unregister the handle before it gets destroyed
            WTSUnRegisterSessionNotification(Handle);
        }

        public static Guid HIDGuid
        {
            get
            {
                Guid gHid;
                HidD_GetHidGuid(out gHid);
                return gHid;
                //return new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); //gHid;
            }
        }

        public static IntPtr RegisterForUsbEvents(IntPtr hWnd, Guid gClass)
        {
            DeviceBroadcastInterface oInterfaceIn = new DeviceBroadcastInterface();
            oInterfaceIn.Size = Marshal.SizeOf(oInterfaceIn);
            oInterfaceIn.ClassGuid = gClass;
            oInterfaceIn.DeviceType = DEVTYP_DEVICEINTERFACE;
            oInterfaceIn.Reserved = 0;
            return RegisterDeviceNotification(hWnd, oInterfaceIn, DEVICE_NOTIFY_WINDOW_HANDLE);
        }

        public static bool UnregisterForUsbEvents(IntPtr hHandle)
        {
            return UnregisterDeviceNotification(hHandle);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetLocaleInfo(uint Locale, uint LCType, [Out] StringBuilder lpLCData, int cchData);

        /// <summary>TimeBeginPeriod(). See the Windows API documentation for details.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
        public static extern uint TimeBeginPeriod(uint uMilliseconds);

        /// <summary>TimeEndPeriod(). See the Windows API documentation for details.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
        public static extern uint TimeEndPeriod(uint uMilliseconds);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcmp(byte[] b1, byte[] b2, long count);

        [DllImport("user32")]
        public static extern uint GetMessageExtraInfo();

        [DllImport("user32")]
        public static extern bool GetCursorPos(out Point lpPoint);

        //public const int LOCALE_SDECIMAL = 0x0000000E;
        //public const int LOCALE_USER_DEFAULT = 0x0400;
        //public const int LOCALE_SYSTEM_DEFAULT = 0x0800;

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint MM_BeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint MM_EndPeriod(uint uMilliseconds);

        [DllImport("user32.dll")]
        public static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        //宣告與ini讀寫相關API宣告
        /*
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetLocaleInfo(uint Locale, uint LCType, [Out] StringBuilder lpLCData, int cchData);
        */

        #region HID API Declare
        //public const int WM_DEVICECHANGE = 0x0219;   //Device Change Command
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVTYP_DEVICEINTERFACE = 0x0005;
        //public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x0000;
        //public const int DIGCF_PRESENT = 0x0002;
        //public const int DIGCF_DEVICEINTERFACE = 0x0010;
        public const UInt32 INVALID_HANDLE_VALUE = 0xffffffff;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

        /*
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint UnregisterDeviceNotification(IntPtr hHandle);
        */

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, UInt32 iEnumerator, IntPtr hParent, UInt32 nFlags);

        public static Guid GUID_DEVINTERFACE_HID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");

        /*
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
            public byte[] dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HDR
        {
            public UInt32 dbch_size;
            public UInt32 dbch_devicetype;
            public UInt32 dbch_reserved;
        }
        */

        /*
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        */

        #endregion

    }// class Win32
}