using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO;

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
        public const int INTERFACE_WIN_BRIDGE_I2C = 8;
        public const int INTERFACE_WIN_BRIDGE_SPI = 9;

        public const int INTF_TYPE_SPI_MA_FALLING = 0;
        public const int INTF_TYPE_SPI_MA_RISING  = 2;
        public const int INTF_TYPE_SPI_MA_RISING_HALF = 4;
        public const int INTF_TYPE_SPI_MA_FALLING_HALF = 6;

        public enum TP_INTERFACE_TYPE
        {
            IF_USB = 0,
            IF_HID_OVER_I2C,
            IF_I2C,
            IF_SPI_MA_RISING_HALF_CYCLE,
            IF_SPI_MA_FALLING_HALF_CYCLE,
            IF_SPI_MA_RISING,
            IF_SPI_MA_FALLING,
            IF_SPI_PRECISE
        }


        ////////////////////////////////////////////////////////
        //Callback function pointer declare
        ////////////////////////////////////////////////////////
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void PFUNC_OUT_REPORT_CALLBACK(IntPtr pReportBuffer, int nReportLen);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void PFUNC_IN_REPORT_CALLBACK(IntPtr pReportBuffer, int nReportLen);

        /// <summary>
        /// The callback functon to notify ap that something happen when starting a socket server.
        /// </summary>
        /// <param name="nEventID"></param>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void PFUNC_SOCKET_EVENT_CALLBACK(int nEventID);

        public static PFUNC_OUT_REPORT_CALLBACK m_OutReportCallBack = null;
        public static PFUNC_IN_REPORT_CALLBACK m_InReportCallBack = null;
        public static PFUNC_SOCKET_EVENT_CALLBACK m_SocketEventCallBack = null;

        public struct SYSDELTA_TIME {
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

        public const int MAX_CHIP_NUM = 4;
        public struct TraceInfo
        {
            public int nChipNum;
            public int nXTotal;
            public int nYTotal;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHIP_NUM)]
            public int[] XAxis;
            public int nPartialNum;       

            public TraceInfo(int nMaxChipNum)
            {
                nChipNum = 0;
                nXTotal = 0;
                nYTotal = 0;
                nPartialNum = 0;
                XAxis = new int[nMaxChipNum];
                Array.Clear(XAxis, 0, XAxis.Length);
            }

            public int GetRXTraceNum(TraceMode Mode)
            {
                int nTraceNum = 0;
                if((Mode & TraceMode.Mutual) != 0)
                {
                    if (nChipNum == 2)
                        nTraceNum = nXTotal - nPartialNum;
                    else if (nChipNum == 3)
                        nTraceNum = nXTotal - (nPartialNum * 2);
                    else
                        nTraceNum = nXTotal;
                }

                if (ContainSelf(Mode) == true)
                {
                    if ((Mode & TraceMode.Combo2Self) != 0)
                        nTraceNum += 2;
                    else
                        nTraceNum += 1;
                }

                if ((Mode & TraceMode.Partial) != 0)
                {
                    if (nChipNum == 2)
                        nTraceNum += nPartialNum;
                    else if (nChipNum == 3)
                        nTraceNum +=  (nPartialNum * 2);
                }

                return nTraceNum;
            }

            public int GetTXTraceNum(TraceMode Mode)
            {
                if (ContainSelf(Mode) == true)
                    return nYTotal + 1;
                else
                    return nYTotal;
            }            
        } ;
        //J2--

        // Alan 20210128
        private static bool m_bEnableBulkMode = false;
        //~Alan 20210128

        /// <summary>
        /// The trace mode that user wants to get
        /// </summary>
        public enum TraceMode
        {
            Mutual = 0x01,
            Self = 0x02,
            Partial = 0x04,
            ComboSelf = 0x08,
            Combo2Self = 0x10
        }

        /// <summary>
        /// Check the inputed mode. It's has self or not.
        /// </summary>
        /// <param name="Mode"></param>
        /// <returns></returns>
        public static bool ContainSelf(ElanTouch.TraceMode Mode)
        {
            if ((Mode & ElanTouch.TraceMode.Self) != 0 || (Mode & ElanTouch.TraceMode.ComboSelf) != 0 || (Mode & ElanTouch.TraceMode.Combo2Self) != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get the trace mode base on the input IC Type.
        /// </summary>
        /// <param name="nICType">Input the IC Type</param>
        /// <param name="bSelf">The return mode needs to include the self or not. Default is set to true</param>
        /// <returns></returns>
        public static TraceMode GetTraceMode(int nICType, bool bSelf = true)
        {
            TraceMode Mode = TraceMode.Mutual | TraceMode.Partial;

            //Bulk mode only support the mutual data
            // Alan 20210128
            if (IsSupportBulk() == true)
                return Mode;
            //~Alan 20210128

            if (bSelf == true)
            {
                switch (nICType)
                {
                    case ElanDefine.Gen53_ICType:
                        Mode |= TraceMode.Self;
                        break;
                    case ElanDefine.Gen63_ICType:
                    case ElanDefine.Gen5M_ICType:
                    case ElanDefine.Gen39P_ICType:
                    case ElanDefine.Gen902_ICType: // 902 Series
                        Mode |= TraceMode.ComboSelf;
                        break;
                    case ElanDefine.Gen7315_2chips_ICType:
                    case ElanDefine.Gen7318_2chips_ICType:
                        Mode |= TraceMode.Combo2Self;
                        break;
                    default:
                        break;
                }
            }

            return Mode;
        }
        //J2--

        // Alan 20210128
        public static bool EnableBulkMode
        {
            set { m_bEnableBulkMode = value; }
            get { return m_bEnableBulkMode; }
        }
        //~Alan 20210128

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void PFUNC_REPORT_RECEIVE(IntPtr pReportBuffer, int nReportLen, int nReportCount, SYSDELTA_TIME timeStamp);

        public static PFUNC_REPORT_RECEIVE m_InHIDReceiveCallBack = null;

        [DllImport("LibTouch.dll")]
        public static extern int InputRawRegHIDCallback(PFUNC_REPORT_RECEIVE pFuncReportRcv, 
                                                        int nDevIdx = 0, 
                                                        int nUsagePage = 0x0D, 
                                                        int Usage = 0x00);

        [DllImport("LibTouch.dll")]
        public static extern int InputRawUnRegHIDCallback(int nDevIdx = 0);

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
        public static extern int Connect(int nVID, 
                                         int nPID, 
                                         int nInterface = INTERFACE_WIN_HID, 
                                         int nDVDD = 33, 
                                         int nVIO = 33, 
                                         int nI2CAddr = 0x20,
                                         int nI2CLength = 0x3f,
                                         int nTDDISPICLKRate = 0);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetSPICmdLength(int nCmdLength = 0x06);

        /// <summary>
        /// Connect to TP via Socket
        /// </summary>
        /// <param name="nIPAddress">The IP address. Input format is 0x7f000001 (127.0.0.1). Set to 0 run the socket server</param>
        /// <param name="nPort">port number</param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int ConnectSocket(int nIPAddress, int nPort, PFUNC_SOCKET_EVENT_CALLBACK pFuncSocketEvent = null);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int ConnectBridge(int nVID, int nPID, TP_INTERFACE_TYPE eInterfaceType);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int ResetBridge(int nVID, int nPID, TP_INTERFACE_TYPE eInterfaceType);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int Disconnect();

        /// <summary>
        /// Use this function to check the HID Over I2c Status(0x07)
        ///  Normal Mode : TP_SUCCESS
        ///  Other       : Recovery mode or other error
        /// </summary>
        /// <param name="nTimeout"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int IsHIDI2CConnected(int nTimeout, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetDeviceCount();

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetHIDDevPairNum(IntPtr pszDevPairNum, int strlen, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetHIDDevPath(IntPtr pszDevPath, int strlen, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetHIDDevType(IntPtr pszDevType, int strlen, int nDevIdx);


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
        /// Get trace information from TP
        /// </summary>
        /// <param name="pTraceInfo"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetTraceInfo(IntPtr pTraceInfo, int nDefaultPartial, int nTimeoutMS, int nDevIdx = 0);

        /// <summary>
        /// Set the user customize information
        /// </summary>
        /// <param name="pTraceInfo"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetTraceInfo(IntPtr pTraceInfo);

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
        /// Get TX Trace number directory.(without send command to read tp information)
        /// </summary>
        /// <param name="p_nTxTrace"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetTXTraceEx(ref Int32 nTxTrace, int nTimeoutMS = -1, int nDevIdx = 0);

        /// <summary>
        /// Get RX Trace number directory.(without send command to read tp information)
        /// </summary>
        /// <param name="nRxTrace"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetRXTraceEx(ref Int32 nRxTrace, int nTimeoutMS = -1, int nDevIdx = 0);

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
                                                byte nGetTraceType,
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
                                                 byte nGetTraceType,
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
                                                byte nGetTraceType,
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
        public static extern int GetDV1DArray(int[] pDVData, int nXLen, int nYLen, int nBaseLen, byte nGetTraceType, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll")]
        public static extern int GetOffset1DArray(int[] pDVData, int nXLen, int nYLen, int nBaseLen, byte nGetTraceType, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll")]
        public static extern int GetMMTPGA1DArray(int[] pDVData, int nXLen, int nYLen, int nBaseLen, byte nGetTraceType, int nTimeoutMS, int nDevIdx);        


        /// <summary>
        /// Enable/Disable Test Mode
        /// </summary>
        /// <param name="bEnable"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int EnableTestMode(bool bEnable, int nTimeoutMS, int nDevIdx);

        /// <summary>
        /// Switch ADC to sensor check mode
        /// </summary>
        /// <param name="bEnable"></param>
        /// <param name="nTimeoutMS"></param>
        /// <param name="nDevIdx"></param>
        /// <returns></returns>
        public static int EnableSensorTest(bool bEnable, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            if(bEnable == true)
                return SendDevCommand(new byte[] { 0x54, 0x8d, 0x01, 0x01 }, 4, nTimeoutMS, nDevIdx);
            else
                return SendDevCommand(new byte[] { 0x54, 0x8d, 0x00, 0x01 }, 4, nTimeoutMS, nDevIdx);
        }

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

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetParamTuneVersionEx(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetICOtherInfoEx(ref UInt32 nValue, int nAddress, int nTimeoutMS = 1000, int nDevIdx = 0);

        [DllImport("LibTouch.dll", SetLastError = true)]
        protected static extern int GetSNVerEx(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0);


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

        public static int GetParamTuneVer(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            int nRetryCount = RETRY_COUNT;
            int nRet = GetParamTuneVersionEx(ref nValue, nTimeoutMS, nDevIdx);
            if (nRet == TP_ERR_DATA_PATTEN)
            {
                while (nRetryCount > 0)
                {
                    byte[] DataArray = new byte[IN_DATA_LENGTH];
                    nRet = ReadDevData(DataArray, DataArray.Length, nTimeoutMS, nDevIdx);
                    if (nRet != TP_SUCCESS)
                        break;

                    if (DataArray[0] != 0x52 || DataArray[1] != 0xD3)
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

        public static int GetICOtherInfo(ref UInt32 nValue, int nAddr, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            int nRetryCount = RETRY_COUNT;
            int nRet = GetICOtherInfoEx(ref nValue, nAddr, nTimeoutMS, nDevIdx);
            if (nRet == TP_ERR_DATA_PATTEN)
            {
                while (nRetryCount > 0)
                {
                    byte[] DataArray = new byte[IN_DATA_LENGTH];
                    byte	AddrHByte	= (byte)((nAddr & 0xff00) >> 8);
                    byte	AddrLByte	= (byte)(nAddr & 0x00ff);
                    nRet = ReadDevData(DataArray, DataArray.Length, nTimeoutMS, nDevIdx);
                    if (nRet != TP_SUCCESS)
                        break;

                    if (DataArray[0] != 0x95 || DataArray[1] != AddrHByte || DataArray[2] != AddrLByte)
                    {
                        nRetryCount--;
                        continue;
                    }
                    else
                    {
                        nValue = (uint)(DataArray[3] << 8 | DataArray[4]);
                        break;
                    }
                }
            }

            return nRet;
        }

        public static int GetSNVer(ref UInt32 nValue, int nTimeoutMS = 1000, int nDevIdx = 0)
        {
            int nRetryCount = RETRY_COUNT;
            int nRet = GetSNVerEx(ref nValue, nTimeoutMS, nDevIdx);
            if (nRet == TP_ERR_DATA_PATTEN)
            {
                while (nRetryCount > 0)
                {
                    byte[] DataArray = new byte[IN_DATA_LENGTH];
                    nRet = ReadDevData(DataArray, DataArray.Length, nTimeoutMS, nDevIdx);
                    if (nRet != TP_SUCCESS)
                        break;

                    if (DataArray[0] != 0x52 || DataArray[1] != 0xD3)
                    {
                        nRetryCount--;
                        continue;
                    }
                    else
                    {
                        nValue = (uint)(DataArray[3] << 8 | DataArray[4]);
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
        public static extern int GetSUM(int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetSUM(int nValue, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetRawMode(bool bEnable, byte nScanTXIdx, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetDiscMode(bool bEnable, int nTimeoutMS, int nDevIdx);

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
        public static extern bool GetPage(ushort[] PageBuffer, ushort nAddr, int nSize, int nTimeoutMS, int nDevIdx, bool bFor63XXInfoPage);

         [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int DisableAlgorithm(bool bDisableOBL, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SendDevCommand(byte[] pszCommandBuf, int nCommandLen,  int nTimeout, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SendBridgeCommand(byte[] pszCommandBuf, int nCommandLen, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetFWIPOption(ref int nValue, int nTimeoutMS = 1000, int nDevIdx = 0);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetSensorID(ref int nValue, int nTimeoutMS = 1000, int nDevIdx = 0);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetSensorIDRAM(ref int nValue, ushort nAddr, int nTimeoutMS = 1000, int nDevIdx = 0);

        #region Bulk Functions
        [DllImport("LibTouch.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsSupportBulk();

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SendBulkCommand(byte[] pszCommandBuf, int nCommandLen, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll")]
        public static extern int GetBulkTestMode1DArrayData(int[] pFrame, int nXLen, int nYLen, int nBaseLen, int nDataType, byte nGetTraceType, int nTimeout, int nDevIdx);
        #endregion

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

        #region Bulk Mode Function
        public static void TransferTestModeViaBulk(int nRXTraceNum, int nTXTraceNum, int nDevIdx)
        {
            byte[] Cmd = { 0x2D, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            int nDataLen = nRXTraceNum * nTXTraceNum * 2;

            Cmd[3] = (byte)((nDataLen & 0xff00) >> 8);
            Cmd[4] = (byte)(nDataLen & 0xff);

            SendBridgeCommand(Cmd, Cmd.Length, ElanDefine.TIME_1SEC, nDevIdx);
        }

        public static void TransferTestModeViaHID(int nDevIdx)
        {
            byte[] Cmd = { 0x2D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            SendBridgeCommand(Cmd, Cmd.Length, ElanDefine.TIME_1SEC, nDevIdx);
            
        }
        #endregion

        // For parsing CSV Param file
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int LoadFromCsvFile(char[] sCsvFileName, ref int p_iTitleCount, ref int p_iParamCount);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetCsvFileInfo(ref int p_iSolutionID, ref int p_iInterface, ref int p_iParameterValue);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetCsvTitle(StringBuilder sbTitle, int iLength, int index);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int GetCsvParam(IntPtr p_eParam, int iParamCount);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetCsvParam(IntPtr p_eParam, int iParamCount);

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
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetEEPROMData(byte[] pROMBuffer, int nSize, int nTimeoutMS = -1, int nDevIdx = 0);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetEEPROMData(byte[] pROMBuffer, int nSize, int nTimeoutMS = -1, int nDevIdx = 0);

        // set 0 for Normal Type and set 3 for Combo Type
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int SetRawDataCount(int nCount, int nTimeoutMS, int nDevIdx);

        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern string GetErrMessage();

        #region Parameter Tuning function declare
        [DllImport("LibTouch.dll", SetLastError = true)]
        public static extern int LoadFromCsvFile(char[] sCsvFileName, IntPtr p_iTitleCount, IntPtr p_iParamCount);
        #endregion
    }


}
