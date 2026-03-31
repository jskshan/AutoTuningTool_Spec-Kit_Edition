using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using Microsoft.Win32;
using System.Diagnostics;

namespace Elan
{
    public sealed class InputDevice
    {
        #region const definitions

        // The following constants are defined in Windows.h

        private const int RIDEV_INPUTSINK = 0x00000100;
        private const int RID_INPUT = 0x10000003;
        private const int RIDEV_PAGEONLY = 0x00000020;

        private const int FAPPCOMMAND_MASK = 0xF000;
        private const int FAPPCOMMAND_MOUSE = 0x8000;
        private const int FAPPCOMMAND_OEM = 0x1000;

        private const int RIM_TYPEMOUSE = 0;
        private const int RIM_TYPEKEYBOARD = 1;
        private const int RIM_TYPEHID = 2;

        private const int RIDI_DEVICENAME = 0x20000007;
        private const int RIDI_DEVICEINFO = 0x2000000b;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_INPUT = 0x00FF;
        private const int VK_OEM_CLEAR = 0xFE;
        private const int VK_LAST_KEY = VK_OEM_CLEAR; // this is a made up value used as a sentinel

        #endregion const definitions

        #region structs & enums

        /// <summary>
        /// An enum representing the different types of input devices.
        /// </summary>
        public enum DeviceType
        {
            Key,
            Mouse,
            OEM
        }

        /// <summary>
        /// Class encapsulating the information about a
        /// keyboard event, including the device it
        /// originated with and what key was pressed
        /// </summary>
        public class DeviceInfo
        {
            public string deviceName;
            public string deviceType;
            public IntPtr deviceHandle;
            public string Name;
            public string source;
            public ushort key;
            public string vKey;
        }

        #region Windows.h structure declarations

        // The following structures are defined in Windows.h

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTDEVICELIST
        {
            public IntPtr hDevice;
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
        }

        /*[StructLayout(LayoutKind.Explicit)]
        internal struct RAWINPUT
        {
            [FieldOffset(0)]
            public RAWINPUTHEADER header;
            [FieldOffset(16)]
            public RAWMOUSE mouse;
            [FieldOffset(16)]
            public RAWKEYBOARD keyboard;
            [FieldOffset(16)]
            public RAWHID hid;
        }*/
        /// <summary>
        /// Contains the raw input from a device. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUT
        {
            /// <summary>
            /// Header for the data.
            /// </summary>
            public RAWINPUTHEADER header;
            public Union Data;
            [StructLayout(LayoutKind.Explicit)]
            public struct Union
            {
                /// <summary>
                /// Mouse raw input data.
                /// </summary>
                [FieldOffset(0)]
                public RAWMOUSE mouse;
                /// <summary>
                /// Keyboard raw input data.
                /// </summary>
                [FieldOffset(0)]
                public RAWKEYBOARD keyboard;
                /// <summary>
                /// HID raw input data.
                /// </summary>
                [FieldOffset(0)]
                public RAWHID hid;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTHEADER
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSize;
            public IntPtr hDevice;
            [MarshalAs(UnmanagedType.U4)]
            public int wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWHID
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizHid;
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct BUTTONSSTR
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonFlags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonData;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct RAWMOUSE
        {
            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(0)]
            public ushort usFlags;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            public uint ulButtons;
            [FieldOffset(4)]
            public BUTTONSSTR buttonsStr;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(8)]
            public uint ulRawButtons;
            [FieldOffset(12)]
            public int lLastX;
            [FieldOffset(16)]
            public int lLastY;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(20)]
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWKEYBOARD
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort MakeCode;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Flags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Reserved;
            [MarshalAs(UnmanagedType.U2)]
            public ushort VKey;
            [MarshalAs(UnmanagedType.U4)]
            public uint Message;
            [MarshalAs(UnmanagedType.U4)]
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTDEVICE
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsagePage;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsage;
            [MarshalAs(UnmanagedType.U4)]
            public int dwFlags;
            public IntPtr hwndTarget;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RID_DEVICE_INFO_HID
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwVendorId;
            [MarshalAs(UnmanagedType.U4)]
            public int dwProductId;
            [MarshalAs(UnmanagedType.U4)]
            public int dwVersionNumber;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsagePage;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsage;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RID_DEVICE_INFO_KEYBOARD
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSubType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwKeyboardMode;
            [MarshalAs(UnmanagedType.U4)]
            public int dwNumberOfFunctionKeys;
            [MarshalAs(UnmanagedType.U4)]
            public int dwNumberOfIndicators;
            [MarshalAs(UnmanagedType.U4)]
            public int dwNumberOfKeysTotal;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RID_DEVICE_INFO_MOUSE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwId;
            [MarshalAs(UnmanagedType.U4)]
            public int dwNumberOfButtons;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSampleRate;
            [MarshalAs(UnmanagedType.U4)]
            public int fHasHorizontalWheel;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct RID_DEVICE_INFO
        {
            [FieldOffset(0)]
            public int cbSize;
            [FieldOffset(4)]
            public int dwType;
            [FieldOffset(8)]
            public RID_DEVICE_INFO_MOUSE mouse;
            [FieldOffset(8)]
            public RID_DEVICE_INFO_KEYBOARD keyboard;
            [FieldOffset(8)]
            public RID_DEVICE_INFO_HID hid;
        }

        #endregion Windows.h structure declarations


        #endregion structs & enums

        #region DllImports

        [DllImport("User32.dll")]
        extern static uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

        [DllImport("User32.dll")]
        extern static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

        [DllImport("User32.dll")]
        extern static uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        #endregion DllImports

        #region Variables and event handling

        /// <summary>
        /// List of keyboard devices. Key: the device handle
        /// Value: the device info class
        /// </summary>
        private System.Collections.Hashtable deviceList = new Hashtable();

        /// <summary>
        /// The delegate to handle KeyPressed events.
        /// </summary>
        /// <param name="sender">The object sending the event.</param>
        /// <param name="e">A set of KeyControlEventArgs information about the key that was pressed and the device it was on.</param>
        public delegate void DeviceEventHandler(object sender, KeyControlEventArgs e);
        public delegate void HIDDeviceEventHandler(object sender, HIDDeviceEventArgs e);

        /// <summary>
        /// The event raised when InputDevice detects that a key was pressed.
        /// </summary>
        public event DeviceEventHandler KeyPressed;
        public event HIDDeviceEventHandler HIDHandler;

        private IntPtr m_hWnd = IntPtr.Zero;

        /// <summary>
        /// Arguments provided by the handler for the HIDDeviceEventArgs
        /// event.
        /// </summary>
        public class HIDDeviceEventArgs : EventArgs
        {
            public byte[] m_Buffer;

            public HIDDeviceEventArgs(byte[] Buffer)
            {
                m_Buffer = Buffer;
            }
        }

        /// <summary>
        /// Arguments provided by the handler for the KeyPressed
        /// event.
        /// </summary>
        public class KeyControlEventArgs : EventArgs
        {
            private DeviceInfo m_deviceInfo;
            private DeviceType m_device;

            public KeyControlEventArgs(DeviceInfo dInfo, DeviceType device)
            {
                m_deviceInfo = dInfo;
                m_device = device;
            }

            public KeyControlEventArgs()
            {
            }

            public DeviceInfo Keyboard
            {
                get { return m_deviceInfo; }
                set { m_deviceInfo = value; }
            }

            public DeviceType Device
            {
                get { return m_device; }
                set { m_device = value; }
            }
        }

        #endregion Variables and event handling

        #region InputDevice( IntPtr hwnd )

        /// <summary>
        /// InputDevice constructor; registers the raw input devices
        /// for the calling window.
        /// </summary>
        /// <param name="hwnd">Handle of the window listening for key presses</param>
        public InputDevice(IntPtr hwnd)
        {
            m_hWnd = hwnd;
        }

        #endregion InputDevice( IntPtr hwnd )

        /// <summary>
        /// 尋找指定的VID與PID的HID Device，並註冊該Device以取得Raw Data
        /// </summary>
        /// <param name="nVID">VID</param>
        /// <param name="nPID">PID</param>
        /// <returns>success return true else return false</returns>
        public bool RegisterHIDDevice(int nVID, int nPID, ushort nCurUsagePage = 0x0d)
        {
            //Get the Device List
            uint nInputDeviceCount = 0;
            int nSize = Marshal.SizeOf(typeof(RAWINPUTDEVICELIST));
            bool bFindDevice = false;
            IntPtr pRawInputDeviceList = IntPtr.Zero;
            int nResult = -1;

            if (GetRawInputDeviceList(IntPtr.Zero, ref nInputDeviceCount, (uint)nSize) != 0)
            {
                return false;
            }

            pRawInputDeviceList = Marshal.AllocHGlobal((int)(nSize * nInputDeviceCount));
            nResult = (int)GetRawInputDeviceList(pRawInputDeviceList, ref nInputDeviceCount, (uint)nSize);

            if (nResult == -1)
            {
                return false;
            }

            ushort uGetUsePage = 0;
            ushort uGetUsage = 0;

            //Check the VID and PID
            for (int i = 0; i < nInputDeviceCount; i++)
            {
                IntPtr pCurDevice = new IntPtr(pRawInputDeviceList.ToInt32() + (i * nSize));
                RAWINPUTDEVICELIST InputDevice = (RAWINPUTDEVICELIST)Marshal.PtrToStructure(pCurDevice, typeof(RAWINPUTDEVICELIST));

                if (pCurDevice == IntPtr.Zero)
                    continue;

                //Get the Device Information 
                uint nRIDDevInfoSize = (uint)Marshal.SizeOf(typeof(RID_DEVICE_INFO));
                IntPtr pDevInfo = Marshal.AllocHGlobal((int)nRIDDevInfoSize);
                RID_DEVICE_INFO devInfo = new RID_DEVICE_INFO();

                //Assign the structure size
                devInfo.cbSize = (int)nRIDDevInfoSize;
                Marshal.StructureToPtr(devInfo, pDevInfo, true);

                nResult = (int)GetRawInputDeviceInfo(InputDevice.hDevice, RIDI_DEVICEINFO, pDevInfo, ref nRIDDevInfoSize);

                if (nResult == -1)
                {
                    Marshal.FreeHGlobal(pDevInfo);
                    continue;
                }

                devInfo = (RID_DEVICE_INFO)Marshal.PtrToStructure(pDevInfo, typeof(RID_DEVICE_INFO));

                if (devInfo.dwType == RIM_TYPEHID)
                {
                    if (nVID > 0 && devInfo.hid.dwVendorId != nVID)
                    {
                        Marshal.FreeHGlobal(pDevInfo);
                        continue;
                    }

                    if (nPID > 0 && devInfo.hid.dwProductId != nPID)
                    {
                        Marshal.FreeHGlobal(pDevInfo);
                        continue;
                    }

                    uGetUsePage = devInfo.hid.usUsagePage;
                    uGetUsage = devInfo.hid.usUsage;
                    Marshal.FreeHGlobal(pDevInfo);
                    bFindDevice = true;
                    break;

                    /*
                    if ((devInfo.hid.dwVendorId == nVID) && (devInfo.hid.dwProductId == nPID))
                    {
                        uGetUsePage = devInfo.hid.usUsagePage;
                        uGetUsage = devInfo.hid.usUsage;
                        Marshal.FreeHGlobal(pDevInfo);
                        bFindDevice = true;
                        break;
                    }
                    */
                }

                Marshal.FreeHGlobal(pDevInfo);
            }

            Marshal.FreeHGlobal(pRawInputDeviceList);

            if (bFindDevice == false)
            {
                return false;
            }

            RAWINPUTDEVICE[] RawInputDevice = new RAWINPUTDEVICE[1];
            RawInputDevice[0].usUsagePage = nCurUsagePage;
            RawInputDevice[0].dwFlags = RIDEV_INPUTSINK | RIDEV_PAGEONLY;
            RawInputDevice[0].usUsage = 0x00;
            RawInputDevice[0].hwndTarget = m_hWnd;

            // listen to digitizer events
            if (RegisterRawInputDevices(RawInputDevice, 1, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE))) == false)
            {
                return false;
            }

            return true;
        }

        #region ReadReg( string item, ref bool isKeyboard )

        /// <summary>
        /// Reads the Registry to retrieve a friendly description
        /// of the device, and determine whether it is a keyboard.
        /// </summary>
        /// <param name="item">The device name to search for, as provided by GetRawInputDeviceInfo.</param>
        /// <param name="isKeyboard">Determines whether the device's class is "Keyboard".</param>
        /// <returns>The device description stored in the Registry entry's DeviceDesc value.</returns>
        private string ReadReg(string item, ref bool isKeyboard)
        {
            // Example Device Identification string
            // @"\??\ACPI#PNP0303#3&13c0b0c5&0#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}";

            // remove the \??\
            item = item.Substring(4);

            string[] split = item.Split('#');

            string id_01 = split[0];    // ACPI (Class code)
            string id_02 = split[1];    // PNP0303 (SubClass code)
            string id_03 = split[2];    // 3&13c0b0c5&0 (Protocol code)
            //The final part is the class GUID and is not needed here

            //Open the appropriate key as read-only so no permissions
            //are needed.
            RegistryKey OurKey = Registry.LocalMachine;

            string findme = string.Format(@"System\CurrentControlSet\Enum\{0}\{1}\{2}", id_01, id_02, id_03);

            OurKey = OurKey.OpenSubKey(findme, false);

            //Retrieve the desired information and set isKeyboard
            string deviceDesc = (string)OurKey.GetValue("DeviceDesc");
            string deviceClass = (string)OurKey.GetValue("Class");

            if (deviceClass.ToUpper().Equals("KEYBOARD"))
            {
                isKeyboard = true;
            }
            else
            {
                isKeyboard = false;
            }
            return deviceDesc;
        }

        #endregion ReadReg( string item, ref bool isKeyboard )

        #region int EnumerateDevices()

        /// <summary>
        /// Iterates through the list provided by GetRawInputDeviceList,
        /// counting keyboard devices and adding them to deviceList.
        /// </summary>
        /// <returns>The number of keyboard devices found.</returns>
        public int EnumerateDevices()
        {

            int NumberOfDevices = 0;
            uint deviceCount = 0;
            int dwSize = (Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));

            // Get the number of raw input devices in the list,
            // then allocate sufficient memory and get the entire list
            if (GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) == 0)
            {
                IntPtr pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
                GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

                // Iterate through the list, discarding undesired items
                // and retrieving further information on keyboard devices
                for (int i = 0; i < deviceCount; i++)
                {
                    DeviceInfo dInfo;
                    string deviceName;
                    uint pcbSize = 0;

                    RAWINPUTDEVICELIST rid = (RAWINPUTDEVICELIST)Marshal.PtrToStructure(
                                               new IntPtr((pRawInputDeviceList.ToInt32() + (dwSize * i))),
                                               typeof(RAWINPUTDEVICELIST));

                    GetRawInputDeviceInfo(rid.hDevice, RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

                    if (pcbSize > 0)
                    {
                        IntPtr pData = Marshal.AllocHGlobal((int)pcbSize);
                        GetRawInputDeviceInfo(rid.hDevice, RIDI_DEVICENAME, pData, ref pcbSize);
                        deviceName = (string)Marshal.PtrToStringAnsi(pData);

                        // Drop the "root" keyboard and mouse devices used for Terminal 
                        // Services and the Remote Desktop
                        if (deviceName.ToUpper().Contains("ROOT"))
                        {
                            continue;
                        }

                        // If the device is identified in the list as a keyboard or 
                        // HID device, create a DeviceInfo object to store information 
                        // about it
                        if (rid.dwType == RIM_TYPEKEYBOARD || rid.dwType == RIM_TYPEHID)
                        {
                            dInfo = new DeviceInfo();

                            dInfo.deviceName = (string)Marshal.PtrToStringAnsi(pData);
                            dInfo.deviceHandle = rid.hDevice;
                            dInfo.deviceType = GetDeviceType(rid.dwType);

                            // Check the Registry to see whether this is actually a 
                            // keyboard, and to retrieve a more friendly description.
                            bool IsKeyboardDevice = false;
                            string DeviceDesc = ReadReg(deviceName, ref IsKeyboardDevice);
                            dInfo.Name = DeviceDesc;

                            // If it is a keyboard and it isn't already in the list,
                            // add it to the deviceList hashtable and increase the
                            // NumberOfDevices count
                            if (!deviceList.Contains(rid.hDevice) && IsKeyboardDevice)
                            {
                                NumberOfDevices++;
                                deviceList.Add(rid.hDevice, dInfo);
                            }
                        }
                        Marshal.FreeHGlobal(pData);
                    }
                }


                Marshal.FreeHGlobal(pRawInputDeviceList);

                return NumberOfDevices;

            }
            else
            {
                throw new ApplicationException("An error occurred while retrieving the list of devices.");
            }

        }

        #endregion EnumerateDevices()

        #region ProcessInputCommand( Message message )

        /// <summary>
        /// Processes WM_INPUT messages to retrieve information about any
        /// keyboard events that occur.
        /// </summary>
        /// <param name="message">The WM_INPUT message to process.</param>
        public void ProcessInputCommand(Message message)
        {
            uint dwSize = 0;

            // First call to GetRawInputData sets the value of dwSize,
            // which can then be used to allocate the appropriate amount of memory,
            // storing the pointer in "buffer".
            GetRawInputData(message.LParam, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));

            IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);

            try
            {
                // Check that buffer points to something, and if so,
                // call GetRawInputData again to fill the allocated memory
                // with information about the input
                if (buffer != IntPtr.Zero &&
                    GetRawInputData(message.LParam,
                                     RID_INPUT,
                                     buffer,
                                     ref dwSize,
                                     (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize)
                {
                    // Store the message information in "raw", then check
                    // that the input comes from a keyboard device before
                    // processing it to raise an appropriate KeyPressed event.

                    RAWINPUT raw = (RAWINPUT)Marshal.PtrToStructure(buffer, typeof(RAWINPUT));

                    #region RIM_TYPEKEYBOARD
                    if (raw.header.dwType == RIM_TYPEKEYBOARD)
                    {
                        // Filter for Key Down events and then retrieve information 
                        // about the keystroke
                        if (raw.Data.keyboard.Message == WM_KEYDOWN || raw.Data.keyboard.Message == WM_SYSKEYDOWN)
                        {
                            ushort key = raw.Data.keyboard.VKey;

                            // On most keyboards, "extended" keys such as the arrow or 
                            // page keys return two codes - the key's own code, and an
                            // "extended key" flag, which translates to 255. This flag
                            // isn't useful to us, so it can be disregarded.
                            if (key > VK_LAST_KEY)
                            {
                                return;
                            }

                            // Retrieve information about the device and the
                            // key that was pressed.
                            DeviceInfo dInfo = null;

                            if (deviceList.Contains(raw.header.hDevice))
                            {
                                Keys myKey;

                                dInfo = (DeviceInfo)deviceList[raw.header.hDevice];

                                myKey = (Keys)Enum.Parse(typeof(Keys), Enum.GetName(typeof(Keys), key));
                                dInfo.vKey = myKey.ToString();
                                dInfo.key = key;
                            }
                            else
                            {
                                string errMessage = String.Format("Handle :{0} was not in hashtable. The device may support more than one handle or usage page, and is probably not a standard keyboard.", raw.header.hDevice);
                                throw new ApplicationException(errMessage);
                            }

                            // If the key that was pressed is valid and there
                            // was no problem retrieving information on the device,
                            // raise the KeyPressed event.
                            if (KeyPressed != null && dInfo != null)
                            {
                                KeyPressed(this, new KeyControlEventArgs(dInfo, GetDevice(message.LParam.ToInt32())));
                            }
                            else
                            {
                                string errMessage = String.Format("Received Unknown Key: {0}. Possibly an unknown device", key);
                                throw new ApplicationException(errMessage);
                            }
                        }
                    }
                    #endregion
                    else if (raw.header.dwType == RIM_TYPEHID)
                    {
                        for (int index = 0; index < raw.Data.hid.dwCount; ++index)
                        {
                            byte[] pBuffer = new byte[raw.Data.hid.dwSizHid];
                            IntPtr pReportStart = new IntPtr(buffer.ToInt32() + Marshal.SizeOf(typeof(RAWINPUTHEADER)) + Marshal.SizeOf(typeof(RAWHID)));
                            Marshal.Copy(pReportStart, pBuffer, 0, pBuffer.Length);
                            if (HIDHandler != null)
                                HIDHandler(this, new HIDDeviceEventArgs(pBuffer));
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        #endregion ProcessInputCommand( Message message )

        #region DeviceType GetDevice( int param )

        /// <summary>
        /// Determines what type of device triggered a WM_INPUT message.
        /// (Used in the ProcessInputCommand method).
        /// </summary>
        /// <param name="param">The LParam from a WM_INPUT message.</param>
        /// <returns>A DeviceType enum value.</returns>
        private DeviceType GetDevice(int param)
        {
            DeviceType deviceType;

            switch ((int)(((ushort)(param >> 16)) & FAPPCOMMAND_MASK))
            {
                case FAPPCOMMAND_OEM:
                    deviceType = DeviceType.OEM;
                    break;
                case FAPPCOMMAND_MOUSE:
                    deviceType = DeviceType.Mouse;
                    break;
                default:
                    deviceType = DeviceType.Key;
                    break;
            }

            return deviceType;
        }

        #endregion DeviceType GetDevice( int param )

        #region ProcessMessage( Message message )

        /// <summary>
        /// Filters Windows messages for WM_INPUT messages and calls
        /// ProcessInputCommand if necessary.
        /// </summary>
        /// <param name="message">The Windows message.</param>
        public void ProcessMessage(Message message)
        {
            switch (message.Msg)
            {
                case WM_INPUT:
                    {
                        ProcessInputCommand(message);
                    }
                    break;
            }
        }

        #endregion ProcessMessage( Message message )

        #region GetDeviceType( int device )

        /// <summary>
        /// Converts a RAWINPUTDEVICELIST dwType value to a string
        /// describing the device type.
        /// </summary>
        /// <param name="device">A dwType value (RIM_TYPEMOUSE, 
        /// RIM_TYPEKEYBOARD or RIM_TYPEHID).</param>
        /// <returns>A string representation of the input value.</returns>
        private string GetDeviceType(int device)
        {
            string deviceType;
            switch (device)
            {
                case RIM_TYPEMOUSE: deviceType = "MOUSE"; break;
                case RIM_TYPEKEYBOARD: deviceType = "KEYBOARD"; break;
                case RIM_TYPEHID: deviceType = "HID"; break;
                default: deviceType = "UNKNOWN"; break;
            }
            return deviceType;
        }

        #endregion GetDeviceType( int device )

    }
}
