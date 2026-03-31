/*
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Elan
{
    public class ElanTouch
    {
        //Declare the Error code
        public static int TP_SUCCESS = 0x0000;
        public static int TP_ERR_COMMAND_NOT_SUPPORT = 0x0001;
        public static int TP_ERR_DEVICE_BUSY = 0x0002;
        public static int TP_ERR_IO_PENDING = 0x0003;
        public static int TP_ERR_DATA_PATTEN = 0x0005;
        public static int TP_ERR_CONNECT_NO_HELLO_PACKEY = 0x1002;
        public static int TP_ERR_NOT_FOUND_DEVICE = 0x1004;
        public static int TP_TESTMODE_GET_RAWDATA_FAIL = 0x3001;
        public static int TP_ERR_CHK_MSG = 0xFFFF;
        public static string m_sDLLPath = "LibTouch.dll";

        //Declare the CTL Type
        public static byte CTL_1 = 0xb8;
        public static byte CTL_2 = 0xb9;

        protected static int RETRY_COUNT = 50;

        /// <summary>
        /// The data length that read from TP
        /// </summary>
        protected static int IN_DATA_LENGTH = 65;

        /// <summary>
        /// Define the interface of connecting.
        /// </summary>
        public const int INTERFACE_WIN_HID = 1;

        public enum InterfaceType
        {
            IF_USB = 0,
            IF_HID_OVER_I2C,
            IF_I2C,
            IF_SPI,
            IF_SPI_ITOUCH,
            IF_SPI_PRECISE,
        }

        /// <summary>
        /// Declare the pen report data
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EMC_REPORT_PEN
        {
            public UInt32 uiReportID;

            [MarshalAs(UnmanagedType.U1)]
            public bool bInRange;
            [MarshalAs(UnmanagedType.U1)]
            public bool bTip;
            [MarshalAs(UnmanagedType.U1)]
            public bool bBarrel;
            [MarshalAs(UnmanagedType.U1)]
            public bool bInvert;

            [MarshalAs(UnmanagedType.U1)]
            public bool bErase;
            [MarshalAs(UnmanagedType.U1)]
            public bool bButton1;
            [MarshalAs(UnmanagedType.U1)]
            public bool bButton2;
            [MarshalAs(UnmanagedType.U1)]
            public bool bReserved;
            public UInt32 uiBattery;
            public UInt32 uiPosX;
            public UInt32 uiPosY;
            public UInt32 uiTipPressure;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EMC_FINGER
        {
            public UInt32 uiContactID;
            [MarshalAs(UnmanagedType.U1)]
            public bool bTip;
            [MarshalAs(UnmanagedType.U1)]
            public bool bReserved1;
            [MarshalAs(UnmanagedType.U1)]
            public bool bReserved2;
            [MarshalAs(UnmanagedType.U1)]
            public bool bReserved3;

            public UInt32 uiContactWidth;
            public UInt32 uiContactHeight;
            public UInt32 uiPosX;
            public UInt32 uiPosY;
            public UInt32 uiPosCenterX;
            public UInt32 uiPosCenterY;

        };

        public struct EMC_REPORT_FINGER
        {
            public UInt32 uiReportID;
            public UInt32 uiActiveTouchCount;
            public UInt32 uiScanTime;
            public EMC_FINGER finger1;
            public EMC_FINGER finger2;
            public EMC_FINGER finger3;
            public EMC_FINGER finger4;
            public EMC_FINGER finger5;
            public EMC_FINGER finger6;
            public EMC_FINGER finger7;
            public EMC_FINGER finger8;
            public EMC_FINGER finger9;
            public EMC_FINGER finger10;
        };


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EMC_VALUE_CAPS
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool bSupport;
            [MarshalAs(UnmanagedType.U1)]
            public bool bReserved1;
            [MarshalAs(UnmanagedType.U1)]
            public bool bReserved2;
            [MarshalAs(UnmanagedType.U1)]
            public bool bReserved3;

            public int iLocialMax;
            public int iDataIdx;
            public int iUsage;
            public int iUsagePage;
            public int iReportCnt;
            public int iBitSz;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PEN_PROFILE
        {
            public int ReportID;

            // button indicator index
            public int TipIdx;
            public int TipUsage;
            public int TipUsagePage;

            public int BarrelSwitchIdx;
            public int BarrelSwitchUsage;
            public int BarrelSwitchUsagePage;

            public int EraserIdx;
            public int EraserUsage;
            public int EraserUsagePage;

            public int InvertIdx;
            public int InvertUsage;
            public int InvertUsagePage;

            public int InRangeIdx;
            public int InRangeUsage;
            public int InRangeUsagePage;

            public EMC_VALUE_CAPS CapsLogicalX;
            public EMC_VALUE_CAPS CapsLogicalY;
            public EMC_VALUE_CAPS CapsTipPressure;
            public EMC_VALUE_CAPS CapsBatteryStrength;
            public EMC_VALUE_CAPS CapsVendor;
            public EMC_VALUE_CAPS CapsReportTest;
            public EMC_VALUE_CAPS CapsTiltX;		// The range between the Y-Z planeand the pointer device plane. Positive is toward the user right hand.
            public EMC_VALUE_CAPS CapsTiltY;      // The range between the X-Z planeand the pointer device plane. Positive is toward the user.
            public EMC_VALUE_CAPS CapsAzimuth;	// The counter-clockwise rotation of the cursor about the Z-axis.

            public int inputReportLength;				// Pen Report Length
        };

        //
        // HID device "Finger" information
        // 
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FINGER_PROFILE
        {
            //m_nContactNumIdx = m_nContactMaxNumEachReport * m_nContactDataSize + m_nScanTimeSize + 1;	
            public int nContactNumIdx;
            //
            // HID device information
            // 
            public int ReportID;
            public int maxLogicalX;
            public int maxLogicalY;
            public int maxContactCnt;					// 10 fingers max. 
            public int inputReportLength;				// Touch report length is 116 bytes.

            public int nScanTimeByteNum;				// Take 4 bytes space in every touch report
            public int nContactDataByteNum;			// Take 11 byte for every contacted finger data
            public int nContactMaxNumEachReport;		// 2 for hybrid mode.. Calculate - every input report carray 5 contact finger report

            public int byteSizeLogicalX;
            public int byteSizeScanTime;
            public int byteSizeContactWidth;

            public int idxTip;
            public int idxX;
            public int idxY;
            public int idxContactID;
            public int idxContactHeight;
            public int idxContactWidth;
            public int idxScanTime;
            public int idxCurContactCnt;

            //J2++
            public EMC_VALUE_CAPS CapsContactID;
            public EMC_VALUE_CAPS CapsWidth;
            public EMC_VALUE_CAPS CapsHeight;
            public EMC_VALUE_CAPS CapsLogicalX;
            public EMC_VALUE_CAPS CapsLogicalY;
            public EMC_VALUE_CAPS CapsScanTime;
            public EMC_VALUE_CAPS CapsActualCount;
            //J2--
        };

        //
        // HID device "Finger" information
        // 
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LINETEST_PROFILE
        {
            //
            // HID device information
            // 
            public int ReportID;
            public int inputReportLength;				// Line Test reprot length is 32 bytes.
            public EMC_VALUE_CAPS CapsLineTest;

        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PROFILE_HEADER
        {
            public uint Length;
            public uint ContentTag;
        };

        // EMC_PROFILE_TOUCH
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TOUCH_PROFILE
        {
            public PROFILE_HEADER pProfileHeader;

            public int nDevIdx;
            [MarshalAs(UnmanagedType.U1)]
            public bool bSupportFinger;
            [MarshalAs(UnmanagedType.U1)]
            public bool bSupportPen;
            [MarshalAs(UnmanagedType.U1)]
            public bool bSupportLineTest;
            [MarshalAs(UnmanagedType.U1)]
            public bool bIsPreciseTouch;
            public int ProfFingerNum;
            public FINGER_PROFILE ProfFinger1;
            public FINGER_PROFILE ProfFinger2;
            public FINGER_PROFILE ProfFinger3;
            public FINGER_PROFILE ProfFinger4;
            public PEN_PROFILE ProfPen;
            public LINETEST_PROFILE ProfLineTest;
        };

        [DllImport("LibTouch.dll")]
        public static extern int ParserReportPen(byte[] ReportData, int iDataLen, IntPtr p_rpPen, int nDevIdx = 0);

        [DllImport("LibTouch.dll")]
        public static extern int ParserReportFinger(byte[] ReportData, int iDataLen, IntPtr p_rpFinger, int nDevIdx = 0);

        [DllImport("LibTouch.dll")]
        public static extern int GetProfile(IntPtr pProfile, int nDevIdx);

        ////////////////////////////////////////////////////////
        //Callback function pointer declare
        ////////////////////////////////////////////////////////
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void PFUNC_OUT_REPORT_CALLBACK(IntPtr pReportBuffer, int nReportLen);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void PFUNC_IN_REPORT_CALLBACK(IntPtr pReportBuffer, int nReportLen);

        //J2++
        /// <summary>
        /// The callback functon to notify ap that something happen when starting a socket server.
        /// </summary>
        /// <param name="nEventID"></param>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void PFUNC_SOCKET_EVENT_CALLBACK(int nEventID);
        //J2--

        public static PFUNC_OUT_REPORT_CALLBACK m_OutReportCallBack;
        public static PFUNC_IN_REPORT_CALLBACK m_InReportCallBack;
        //J2++
        public static PFUNC_SOCKET_EVENT_CALLBACK m_SocketEventCallBack = null;
        //J2--

        // Alan 20161111
        public struct SYSDELTA_TIME
        {
            public Int16 wYear;
            public Int16 wMonth;
            public Int16 wDayOfWeek;
            public Int16 wDay;
            public Int16 wHour;
            public Int16 wMinute;
            public Int16 wSecond;
            public Int16 wMilliseconds;
            public long lDeltaMicroSeconds;

            public SYSDELTA_TIME(bool bInitial = true)
            {
                this.wYear = this.wMonth = this.wDayOfWeek = this.wDay = this.wHour = 0;
                this.wMinute = this.wSecond = this.wMilliseconds = 0;
                this.lDeltaMicroSeconds = 0;
            }
        };

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void PFUNC_REPORT_RECEIVE(IntPtr pReportBuffer, int nReportLen, int nReportCount, SYSDELTA_TIME timeStamp);

        public static PFUNC_REPORT_RECEIVE m_InHIDReceiveCallBack = null;

        [DllImport("LibTouch.dll")]
        public static extern int InputRawRegHIDDevice(IntPtr hWnd, int nDevIdx = 0, int nUsagePage = 0x0D, int Usage = 0x00);

        [DllImport("LibTouch.dll")]
        public static extern int InputRawRegHIDCallback(PFUNC_REPORT_RECEIVE pFuncReportRcv,
                                                        int nDevIdx = 0,
                                                        int nUsagePage = 0x0D,
                                                        int Usage = 0x00);

        [DllImport("LibTouch.dll")]
        public static extern int InputRawUnRegHIDCallback(int nDevIdx = 0);
        //~Alan 20161111

        [DllImport("LibTouch.dll")]
        public static extern int OutReportRegCallback(PFUNC_OUT_REPORT_CALLBACK pFuncReportRcv);
        [DllImport("LibTouch.dll")]
        public static extern int OutReportUnRegCallback();
        [DllImport("LibTouch.dll")]
        public static extern int InReportRegCallback(PFUNC_IN_REPORT_CALLBACK pFuncReportRcv);
        [DllImport("LibTouch.dll")]
        public static extern int InReportUnRegCallback();

        ////////////////////////////////////////////////////////
        //Basic communication functions with TP Device
        ////////////////////////////////////////////////////////

        /// <summary>
        /// Connect to TP Device
        /// </summary>
        /// <param name="nVID"></param>
        /// <param name="nPID"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int Connect(int nVID = 0x4f3,
                                         int nPID = 0x00,
                                         int nInterface = INTERFACE_WIN_HID,
                                         int nDVDD = 33,
                                         int nVIO = 33,
                                         int nI2CAddr = 0x20,
                                         int nI2CLength = 0x3f);


        //J2++
        /// <summary>
        /// Connect to TP via Socket
        /// </summary>
        /// <param name="nIPAddress">The IP address. Input format is 0x7f000001 (127.0.0.1). Set to 0 run the socket server</param>
        /// <param name="nPort">port number</param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int ConnectSocket(int nIPAddress, int nPort, PFUNC_SOCKET_EVENT_CALLBACK pFuncSocketEvent = null);
        //J2--

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int ConnectBridge(int nVID, int nPID, InterfaceType eInterfaceType);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern void Disconnect();

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetDeviceCount();

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetHIDDevPairNum(IntPtr pszDevPairNum, int strlen, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetHIDDevPath(char[] pszDevPath, int strlen, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetHIDDevType(char[] pszDevType, int strlen, int nDevIdx);


        ////////////////////////////////////////////////////////
        //Get basic information form TP Device
        ////////////////////////////////////////////////////////

        /// <summary>
        /// Get the PID and VID
        /// </summary>
        /// <param name="p_nVID"></param>
        /// <param name="p_nPID"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetID(ref UInt32 p_nVID, ref UInt32 p_nPID, int nDevIdx);

        /// <summary>
        /// Get the number of IC
        /// </summary>
        /// <param name="nPartial"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetChipNum(int nPartial, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetBridgeVersion(ref UInt32 p_nbridgeVer, int nDevIdx);

        /// <summary>
        /// Get the TX Trace number
        /// </summary>
        /// <param name="nPartial"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetTXTrace(int nPartial, int nTimeoutMS, int nDevIdx);

        /// <summary>
        /// Get the RX Trace Number
        /// </summary>
        /// <param name="nPartial"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetRXTrace(int nPartial, int nTimeoutMS, int nDevIdx);

        /// <summary>
        /// Get the trace number of each ic
        /// </summary>
        /// <param name="RXTraceArray"></param>
        /// <param name="nPartial"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetRXTraceArray(int[] RXTraceArray, int nPartial, int nTimeoutMS, int nDevIdx);

        /// <summary>
        /// Fw get multi-frame and compute the average.
        /// </summary>
        /// <param name="pFrame"></param>
        /// <param name="nXLen"></param>
        /// <param name="nYLen"></param>
        /// <param name="nBaseLen"></param>
        /// <param name="nFrameCount"></param>
        /// <param name="nTimeout"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetMultiADCFarmes(int[] pFrame,
                                                   int nXLen,
                                                   int nYLen,
                                                   int nBaseLen,
                                                   int nFrameCount,
                                                   int nTimeout,
                                                   int nDevIdx);

        /// <summary>
        /// Get the ADC data and convert the 2d array to 1d array 
        /// </summary>
        /// <param name="pFrame"></param>
        /// <param name="nXLen"></param>
        /// <param name="nYLen"></param>
        /// <param name="nBaseLen"></param>
        /// <param name="nTimeout"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll")]
        public static extern int GetADC1DArray(int[] pFrame,
                                                int nXLen,
                                                int nYLen,
                                                int nBaseLen,
                                                int nTimeout,
                                                int nDevIdx);

        /// <summary>
        /// Get the noise data and conver the 2d array to 1d array
        /// </summary>
        /// <param name="pFrame"></param>
        /// <param name="nXLen"></param>
        /// <param name="nYLen"></param>
        /// <param name="nBaseLen"></param>
        /// <param name="nTimeout"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll")]
        public static extern int GetNoise1DArray(int[] pFrame,
                                                 int nXLen,
                                                 int nYLen,
                                                 int nBaseLen,
                                                 int nTimeout,
                                                 int nDevIdx);

        /// <summary>
        /// Get the base data and store to 1D Array 
        /// </summary>
        /// <param name="pBaseData"></param>
        /// <param name="nXLen"></param>
        /// <param name="nYLen"></param>
        /// <param name="nBaseLen"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll")]
        public static extern int GetBase1DArray(int[] pBaseData,
                                                int nXLen,
                                                int nYLen,
                                                int nBaseLen,
                                                int nTimeoutMS,
                                                int nDevIdx);

        /// <summary>
        /// Get the dV data and convert the 2d array to 1d array 
        /// </summary>
        /// <param name="pDVData"></param>
        /// <param name="nXLen"></param>
        /// <param name="nYLen"></param>
        /// <param name="nBaseLen"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll")]
        public static extern int GetDV1DArray(int[] pDVData, int nXLen, int nYLen, int nBaseLen, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll")]
        public static extern int GetOtherRawData1DArray(int[] pOtherData, int nXLen, int nYLen, int nDataType, int nTimeoutMS, int nDevIdx);

        /// <summary>
        /// Enable/Disable Test Mode
        /// </summary>
        /// <param name="bEnable"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int EnableTestMode(bool bEnable, int nTimeoutMS, int nDevIdx);

        ///////////////////////////////////////////////////////////////////////////
        // Declare the functions to get TP version information
        ///////////////////////////////////////////////////////////////////////////
        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetFwIDEx(ref UInt32 fwID, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetFwVerEx(ref UInt32 fwVer, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetTestSolVerEx(ref UInt32 pTestSolVer, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetTestVerEx(ref uint pTestVer, int nTimeoutMS = 1000, int nDevIdx = 0);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetSolVerEx(ref UInt32 nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetBCVerEx(ref UInt32 nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetWHCKVerEx(ref UInt32 nValue, int nTimeoutMS, int nDevIdx);

        /// <summary>
        /// Get FW ID
        /// </summary>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        public static int GetFWID(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            int nRetryCount = RETRY_COUNT;
            int nRet = GetFwIDEx(ref nValue, nTimeoutMS, nDevIdx);
            if (nRet == TP_ERR_DATA_PATTEN)
            {
                while (nRetryCount > 0)
                {
                    byte[] DataArray = new byte[IN_DATA_LENGTH];
                    nRet = ReadDevData(DataArray, DataArray.Length, nTimeoutMS, nDevIdx);
                    if (nRet != TP_SUCCESS)
                        break;

                    if (DataArray[0] != 0x52 || (DataArray[1] & 0xf0) != 0xf0)
                    {
                        nRetryCount--;
                        continue;
                    }
                    else
                    {
                        nValue = (uint)(((DataArray[1] & 0x0f) << 12) |
                                        ((DataArray[2] & 0xff) << 4) |
                                        ((DataArray[3] & 0xf0) >> 4));
                        break;
                    }
                }
            }

            return nRet;
        }

        /// <summary>
        /// Get FW Version
        /// </summary>
        /// <param name="nValue"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        public static int GetFWVer(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            int nRetryCount = RETRY_COUNT;
            int nRet = GetFwVerEx(ref nValue, nTimeoutMS, nDevIdx);
            if (nRet == TP_ERR_DATA_PATTEN)
            {
                while (nRetryCount > 0)
                {
                    byte[] DataArray = new byte[IN_DATA_LENGTH];
                    nRet = ReadDevData(DataArray, DataArray.Length, nTimeoutMS, nDevIdx);
                    if (nRet != TP_SUCCESS)
                        break;

                    if (DataArray[0] != 0x52 || (DataArray[1] & 0x00) != 0xf0)
                    {
                        nRetryCount--;
                        continue;
                    }
                    else
                    {
                        nValue = (uint)(((DataArray[1] & 0x0f) << 12) |
                                        ((DataArray[2] & 0xff) << 4) |
                                        ((DataArray[3] & 0xf0) >> 4));
                        break;
                    }
                }
            }

            return nRet;
        }

        /// <summary>
        /// Get Test Solution Version
        /// </summary>
        /// <param name="nValue"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        public static int GetTestSolVer(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            int nRetryCount = RETRY_COUNT;
            int nRet = GetTestSolVerEx(ref nValue, nTimeoutMS, nDevIdx);
            if (nRet == TP_ERR_DATA_PATTEN)
            {
                while (nRetryCount > 0)
                {
                    byte[] DataArray = new byte[IN_DATA_LENGTH];
                    nRet = ReadDevData(DataArray, DataArray.Length, nTimeoutMS, nDevIdx);
                    if (nRet != TP_SUCCESS)
                        break;

                    if (DataArray[0] != 0x52 || (DataArray[1] & 0xF0) != 0xE0)
                    {
                        nRetryCount--;
                        Thread.Sleep(10);
                        continue;
                    }
                    else
                    {
                        nValue = (uint)(((DataArray[1] & 0x0f) << 12) | (DataArray[2] << 4) | ((DataArray[3] & 0xf0) >> 4));
                        break;
                    }
                }
            }

            return nRet;
        }

        /// <summary>
        /// Get Test Version
        /// </summary>
        /// <param name="nValue"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        public static int GetTestVer(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            int nRetryCount = RETRY_COUNT;
            int nRet = GetTestVerEx(ref nValue, nTimeoutMS, nDevIdx);
            if (nRet == TP_ERR_DATA_PATTEN)
            {
                while (nRetryCount > 0)
                {
                    byte[] DataArray = new byte[IN_DATA_LENGTH];
                    nRet = ReadDevData(DataArray, DataArray.Length, nTimeoutMS, nDevIdx);
                    if (nRet != TP_SUCCESS)
                        break;

                    if (DataArray[0] != 0x52 || (DataArray[1] & 0xF0) != 0xE0)
                    {
                        nRetryCount--;
                        Thread.Sleep(10);
                        continue;
                    }
                    else
                    {
                        nValue = (uint)(((DataArray[1] & 0x0f) << 4) | ((DataArray[2] & 0xf0) >> 4));
                        break;
                    }
                }
            }

            return nRet;
        }

        /// <summary>
        /// Get Solution Version
        /// </summary>
        /// <param name="nValue"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        public static int GetSolutionVersion(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            int nRetryCount = RETRY_COUNT;
            int nRet = GetSolVerEx(ref nValue, nTimeoutMS, nDevIdx);
            if (nRet == TP_ERR_DATA_PATTEN)
            {
                while (nRetryCount > 0)
                {
                    byte[] DataArray = new byte[IN_DATA_LENGTH];
                    nRet = ReadDevData(DataArray, DataArray.Length, nTimeoutMS, nDevIdx);
                    if (nRet != TP_SUCCESS)
                        break;

                    if (DataArray[0] != 0x52 || (DataArray[1] & 0xF0) != 0xE0)
                    {
                        nRetryCount--;
                        continue;
                    }
                    else
                    {
                        nValue = (uint)(((DataArray[2] & 0x0f) << 4) | ((DataArray[3] & 0xf0) >> 4));
                        break;
                    }
                }
            }

            return nRet;
        }

        /// <summary>
        /// Get Bootcode Version
        /// </summary>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        public static int GetBootcodeVersion(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            int nRetryCount = RETRY_COUNT;
            int nRet = GetBCVerEx(ref nValue, nTimeoutMS, nDevIdx);
            if (nRet == TP_ERR_DATA_PATTEN)
            {
                while (nRetryCount > 0)
                {
                    byte[] DataArray = new byte[IN_DATA_LENGTH];
                    nRet = ReadDevData(DataArray, DataArray.Length, nTimeoutMS, nDevIdx);
                    if (nRet != TP_SUCCESS)
                        break;

                    if (DataArray[0] != 0x52 || (DataArray[1] & 0xf0) != 0x10)
                    {
                        nRetryCount--;
                        continue;
                    }
                    else
                    {
                        nValue = (uint)(((DataArray[1] & 0x0f) << 12) |
                                        ((DataArray[2] & 0xff) << 4) |
                                        ((DataArray[3] & 0xf0) >> 4));
                        break;
                    }
                }
            }

            return nRet;
        }

        /// <summary>
        /// Get WHCK Version
        /// </summary>
        /// <param name="nValue"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        public static int GetWHCKVersion(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            int nRetryCount = RETRY_COUNT;
            int nRet = GetWHCKVerEx(ref nValue, nTimeoutMS, nDevIdx);
            if (nRet == TP_ERR_DATA_PATTEN)
            {
                while (nRetryCount > 0)
                {
                    byte[] DataArray = new byte[IN_DATA_LENGTH];
                    nRet = ReadDevData(DataArray, DataArray.Length, nTimeoutMS, nDevIdx);
                    if (nRet != TP_SUCCESS)
                        break;

                    if (DataArray[0] != 0x52 || DataArray[1] != 0xD2)
                    {
                        nRetryCount--;
                        continue;
                    }
                    else
                    {
                        nValue = (uint)(DataArray[2] << 8 | DataArray[3]);
                        break;
                    }
                }
            }

            return nRet;
        }

        ///////////////////////////////////////////////////////////////////////////
        // Declare the function to get/get the analog parameter
        ///////////////////////////////////////////////////////////////////////////
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetPH1(int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetPH1(int nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetPH2(int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetPH2(int nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetPH3(int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetPH3(int nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetTPParameter(int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int ReK(int nTimeout, bool bOldRek, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern bool GetPage(int[] PageBuffer, int nAddr, int nSize, int nTimeoutMS = -1, int nDevIdx = 0);
        //[DllImport("ElanTPDLL.dll", SetLastError = true)]
        //public static extern int ReadRomValue(ushort nAddr, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetSUM(int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetSUM(int nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetRawMode(bool bEnable, byte nScanTXIdx, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int DisableTPReport(bool bEnable, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int EnableCCVMode(bool bEnable,
                                                bool bEnableSwitch,
                                                int nDelay,
                                                int nTimeoutMS,
                                                int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetCCV(byte[] pCCVRawData,
                                        int nTPMasterNum,
                                        int nTPSlaveNum,
                                        int nSampleNum,
                                        int nBaseLen,
                                        int nTimeoutMS,
                                        int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetProjOption(int nTimeout, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetProjOption(short nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int ClearFR(int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetANA_CTL(byte nCTLType, ref int nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetANA_CTL(byte nCTLType, int nVlaue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetBSH(ref int nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetBSH(int nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetRGM(ref byte nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetRGM(byte nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetPage(ushort[] PageBuffer, ushort nAddr, int nSize, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int DisableAlgorithm(bool bDisableOBL, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SendDevCommand(byte[] pszCommandBuf, int nCommandLen, int nTimeout, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SendBridgeCommand(byte[] pszCommandBuf, int nCommandLen, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetFWIPOption(ref int nValue, int nTimeoutMS = 1000, int nDevIdx = 0);

        /// <summary>
        /// Get the FW IP Option
        /// </summary>
        /// <returns></returns>
        public static int GetFWIPOption()
        {
            int nRetryCount = RETRY_COUNT;
            int nValue = 0;
            while (nRetryCount > 0)
            {
                int nRet = GetFWIPOption(ref nValue, 1000, 0);
                if (nRet == TP_SUCCESS)
                    break;
                else if (nRet == TP_ERR_DATA_PATTEN)
                {
                    nRetryCount--;
                    continue;
                }
                else
                {
                    nValue = -1;
                    break;
                }
            }

            return nValue;
        }

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetFWIPOption(int nValue, int nTimeoutMS = 1000, int nDevIdx = 0);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int ReadDevData(byte[] pszDataBuf, int nDataLen, int nTimeoutMS, int nDevIdx);

        public static bool GetPhysicalSize(ref int nX, ref int nY, int nTimeout)
        {
            nX = xGetPhysicalSize(true, nTimeout);
            nY = xGetPhysicalSize(false, nTimeout);

            if (nX <= 0 || nY <= 0)
                return false;

            return true;
        }

        protected static short xGetPhysicalSize(bool bScan, int nTimeout)
        {
            short nRet = 0;
            int nRetryCount = 50;
            byte[] cmd = { 0x53, 0xD0, 0x00, 0x01 };
            byte[] Buf = new byte[65];
            if (bScan == true)
                cmd[1] = 0xD7;
            else
                cmd[1] = 0xD8;

            SendDevCommand(cmd, cmd.Length, 1000, 0);

            while (nRetryCount > 0)
            {
                if (ReadDevData(Buf, Buf.Length, 1000, 0) != TP_SUCCESS)
                    break;

                if (Buf[1] != cmd[1])
                {
                    nRetryCount--;
                    continue;
                }

                nRet = (short)(Buf[2] << 8 | Buf[3]);

                break;
            }

            return nRet;
        }

        /// <summary>
        /// Get the CTL value with specific type
        /// Type : CTL_1 or CTL_2
        /// </summary>
        /// <param name="nCTLType"></param>
        /// <returns></returns>
        public static int GetCTL(byte nCTLType)
        {
            int nRetryCount = RETRY_COUNT;
            int nValue = 0;
            while (nRetryCount > 0)
            {
                int nRet = GetANA_CTL(nCTLType, ref nValue, 1000, 0);
                if (nRet == TP_SUCCESS)
                    break;
                else if (nRet == TP_ERR_DATA_PATTEN)
                {
                    nRetryCount--;
                    continue;
                }
                else
                {
                    nValue = -1;
                    break;
                }
            }

            return nValue;
        }

        /// <summary>
        /// Get the BSH Value
        /// </summary>
        /// <returns></returns>
        public static int GetBSH()
        {
            int nRetryCount = RETRY_COUNT;
            int nValue = 0;
            while (nRetryCount > 0)
            {
                int nRet = GetBSH(ref nValue, 1000, 0);
                if (nRet == TP_SUCCESS)
                    break;
                else if (nRet == TP_ERR_DATA_PATTEN)
                {
                    nRetryCount--;
                    continue;
                }
                else
                {
                    nValue = -1;
                    break;
                }
            }

            return nValue;
        }

        // Below functions for ParameterTuning CSV Parsing
        public const int MAX_PARAM_VAR_LENGTH = 128;

        public enum ParamVarField
        {
            Index = 0,
            Address,
            Classification,
            Name,
            DefaultValue,
            Range,
            AssociatedExpression,
            FunctionDescriptor,
            Public,
            SmallValueSideEffect,
            LargeValueSideEffect,
            Tag1,
            Tag2,
            Tag3,
            Max_ParamVar_Count
        };

        public const int MAX_PARAM_VAR_COUNT = (int)ParamVarField.Max_ParamVar_Count; //14

        public struct CSV_PARAM
        {
            public char[,] cParam;//[MAX_PARAM_VAR_COUNT][MAX_PARAM_VAR_LENGTH];

            public UInt32 uiIndex;
            public UInt32 uiAddress;

            public UInt32 uiValue;

            public UInt32 uiMax;
            public UInt32 uiMin;

            public bool bExprCheck;	// For C# dll import issue, the struct length
            public byte[] ucReservedByte;	// should be the multiple size of 4 byts. ucReservedByte[3]

            public CSV_PARAM(bool bInitial = true)
            {
                this.cParam = new char[MAX_PARAM_VAR_COUNT, MAX_PARAM_VAR_LENGTH];
                this.uiIndex = this.uiAddress = this.uiValue = this.uiMax = this.uiMin = 0;
                this.bExprCheck = false;
                this.ucReservedByte = new byte[3];
            }
        };

        // For parsing CSV Param file
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int LoadFromCsvFile(char[] sCsvFileName, ref int p_iTitleCount, ref int p_iParamCount);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetCsvFileInfo(ref int p_iSolutionID, ref int p_iInterface, ref int p_iParameterValue);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetCsvTitle(char[] sTitle, int iLength, int index);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetCsvParam(CSV_PARAM[] p_eParam, int iParamCount);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetCsvParam(CSV_PARAM[] p_eParam, int iParamCount);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int CsvExportEktFile(char[] sEktFileName);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int CsvExportEktFileWithLastPage(char[] sEktFileName, byte[] pucLastPage);

        // For parsing ini param file
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int UnicodeFileToAnsiFile(char[] cUnicodeInIFileName, char[] cAnsiIniFileName);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern bool LoadFromInIFile(char[] sInIFileName);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern void InIExportEktFile(char[] sEktFileName);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern void InIExportEktFileWithLastPage(char[] sEktFileName, byte[] pucLastPage);

        // Read / Write EEPROM
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern bool GetEEPROMData(byte[] pROMBuffer, int nSize, int nTimeoutMS = -1, int nDevIdx = 0);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetEEPROMData(byte[] pROMBuffer, int nSize, int nTimeoutMS = -1, int nDevIdx = 0);

    }
}
*/
