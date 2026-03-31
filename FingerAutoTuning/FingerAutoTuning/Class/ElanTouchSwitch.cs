using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    class ElanTouchSwitch
    {
        public const int m_nPARAMETER_PH1           = 1;
        public const int m_nPARAMETER_PH2           = 2;
        public const int m_nPARAMETER_PH3           = 3;
        public const int m_nPARAMETER_SUM           = 4;
        public const int m_nPARAMETER_TPPARAMETER   = 5;

        public const int m_nOPTION_PROJECTOPTION    = 1;
        public const int m_nOPTION_FWIPOPTION       = 2;

        public const int m_nDATA_BASE               = 1;
        public const int m_nDATA_ADC                = 2;
        public const int m_nDATA_DV                 = 3;
        public const int m_nDATA_Noise              = 4;

        private static int m_nTimeout = ParamFingerAutoTuning.m_nGetParameterTimeout;

        public static void SetGetParameterTimeout()
        {
            //if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                m_nTimeout = ParamFingerAutoTuning.m_nSSHSocketServerGetParameterTimeout;
            else
                m_nTimeout = ParamFingerAutoTuning.m_nGetParameterTimeout;
        }

        public static int SetInterface(bool bSocket)
        {
            int nInterface = ElanTouch.INTERFACE_WIN_HID;

            if (((int)UserInterfaceDefine.InterfaceType.IF_I2C == ParamFingerAutoTuning.m_nInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_I2C_TDDI == ParamFingerAutoTuning.m_nInterfaceType))
            {
                if (bSocket == false)
                    nInterface = ElanTouch.INTERFACE_WIN_BRIDGE_I2C;
                else
                    nInterface = ElanTouch_Socket.INTERFACE_WIN_BRIDGE_I2C;
            }
            else if (((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE == ParamFingerAutoTuning.m_nInterfaceType) ||
                     ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING_HALF_CYCLE == ParamFingerAutoTuning.m_nInterfaceType))
            {
                if (bSocket == false)
                    nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_RISING_HALF << 4);
                else
                    nInterface = (ElanTouch_Socket.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch_Socket.INTF_TYPE_SPI_MA_RISING_HALF << 4);
            }
            else if (((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE == ParamFingerAutoTuning.m_nInterfaceType) ||
                     ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING_HALF_CYCLE == ParamFingerAutoTuning.m_nInterfaceType))
            {
                if (bSocket == false)
                    nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_FALLING_HALF << 4);
                else
                    nInterface = (ElanTouch_Socket.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch_Socket.INTF_TYPE_SPI_MA_FALLING_HALF << 4);
            }
            else if (((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING == ParamFingerAutoTuning.m_nInterfaceType) ||
                     ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING == ParamFingerAutoTuning.m_nInterfaceType))
            {
                if (bSocket == false)
                    nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_RISING << 4);
                else
                    nInterface = (ElanTouch_Socket.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch_Socket.INTF_TYPE_SPI_MA_RISING << 4);
            }
            else if (((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING == ParamFingerAutoTuning.m_nInterfaceType) ||
                     ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING == ParamFingerAutoTuning.m_nInterfaceType))
            {
                if (bSocket == false)
                    nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_FALLING << 4);
                else
                    nInterface = (ElanTouch_Socket.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch_Socket.INTF_TYPE_SPI_MA_FALLING << 4);
            }
            else
            {
                if (bSocket == false)
                    nInterface = ElanTouch.INTERFACE_WIN_HID;
                else
                    nInterface = ElanTouch_Socket.INTERFACE_WIN_HID;
            }

            return nInterface;
        }

        public static bool Connect(int nAssignVID, int nAssignPID, int nInterface, int nDVDD, int nVIO, int nI2CAddress, int nNormalLength, bool bSocket)
        {
            if (bSocket == false)
            {
                if (ElanTouch.Connect(nAssignVID, nAssignPID, nInterface, nDVDD:nDVDD, nVIO:nVIO, nI2CAddr:nI2CAddress, nI2CLength:nNormalLength) != ElanTouch.TP_SUCCESS)
                    return false;
                else
                    return true;
            }
            else
            {
                if (ElanTouch_Socket.Connect(nAssignVID, nAssignPID, nInterface, nDVDD: nDVDD, nVIO: nVIO, nI2CAddr: nI2CAddress, nI2CLength: nNormalLength) != ElanTouch_Socket.TP_SUCCESS)
                    return false;
                else
                    return true;
            }
        }

        public static int GetDeviceCount(bool bSocket)
        {
            if (bSocket == false)
                return ElanTouch.GetDeviceCount();
            else
                return ElanTouch_Socket.GetDeviceCount();
        }

        public static int SetSPICmdLength(bool bSocket, int nCmdLength = 0x06)
        {
            if (bSocket == false)
                return ElanTouch.SetSPICmdLength(nCmdLength);
            else
                return ElanTouch_Socket.SetSPICmdLength(nCmdLength);
        }

        public static bool GetHIDDevPath(IntPtr nPair, int nLength, int nDeviceIndex, bool bSocket)
        {
            int nResultFlag;

            if (bSocket == false)
            {
                nResultFlag = ElanTouch.GetHIDDevPath(nPair, nLength, nDeviceIndex);

                if (nResultFlag == ElanTouch.TP_SUCCESS)
                    return true;
                else
                    return false;
            }
            else
            {
                nResultFlag = ElanTouch_Socket.GetHIDDevPath(nPair, nLength, nDeviceIndex);

                if (nResultFlag == ElanTouch_Socket.TP_SUCCESS)
                    return true;
                else
                    return false;
            }
        }

        public static int GetRXTXTrace(bool bRXTrace, int nDeviceIndex, bool bSocket)
        {
            if (bSocket == false)
            {
                if (bRXTrace == true)
                    return ElanTouch.GetRXTrace(2, m_nTimeout, nDeviceIndex);
                else
                    return ElanTouch.GetTXTrace(2, m_nTimeout, nDeviceIndex);
            }
            else
            {
                if (bRXTrace == true)
                    return ElanTouch_Socket.GetRXTrace(2, m_nTimeout, nDeviceIndex);
                else
                    return ElanTouch_Socket.GetTXTrace(2, m_nTimeout, nDeviceIndex);
            }
        }

        public static bool GetFWVersion(ref UInt32 nFWVersion, int nDeviceIndex, bool bSocket)
        {
            if (bSocket == false)
            {
                int nResultFlag = ElanTouch.GetFWVer(ref nFWVersion, m_nTimeout, nDeviceIndex);

                if (nResultFlag == ElanTouch.TP_SUCCESS)
                    return true;
                else
                    return false;
            }
            else
            {
                int nResultFlag = ElanTouch_Socket.GetFWVer(ref nFWVersion, m_nTimeout, nDeviceIndex);

                if (nResultFlag == ElanTouch_Socket.TP_SUCCESS)
                    return true;
                else
                    return false;
            }
        }

        public static void SendDevCommand(byte[] byteCommand_Array, int nDeviceIndex, bool bSocket)
        {
            if (bSocket == false)
                ElanTouch.SendDevCommand(byteCommand_Array, byteCommand_Array.Length, 1000, nDeviceIndex);
            else
                ElanTouch_Socket.SendDevCommand(byteCommand_Array, byteCommand_Array.Length, 1000, nDeviceIndex);
        }

        public static int ReadDevData(byte[] byteData_Array, int nDeviceIndex, bool bSocket, int nTimeout = 1000)
        {
            if (bSocket == false)
                return ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, nTimeout, nDeviceIndex);
            else
                return ElanTouch_Socket.ReadDevData(byteData_Array, byteData_Array.Length, nTimeout, nDeviceIndex);
        }

        public static bool CheckTPState(int nTPStateFlag, bool bSocket)
        {
            if (bSocket == false)
            {
                if (nTPStateFlag == ElanTouch.TP_SUCCESS)
                    return true;
                else
                    return false;
            }
            else
            {
                if (nTPStateFlag == ElanTouch_Socket.TP_SUCCESS)
                    return true;
                else
                    return false;
            }
        }

        public static void DisableTPReport(bool bDisable, int nDeviceIndex, bool bSocket)
        {
            if (bSocket == false)
                ElanTouch.DisableTPReport(bDisable, 1000, nDeviceIndex);
            else
                ElanTouch_Socket.DisableTPReport(bDisable, 1000, nDeviceIndex);
        }

        public static void EnableTestMode(bool bEnable, int nDeviceIndex, bool bSocket)
        {
            if (bSocket == false)
                ElanTouch.EnableTestMode(bEnable, 1000, nDeviceIndex);
            else
                ElanTouch_Socket.EnableTestMode(bEnable, 1000, nDeviceIndex);
        }

        public static void SetAnalogParameter(int nParameterType, int nValue, int nDeviceIndex, bool bSocket)
        {
            switch (nParameterType)
            {
                case m_nPARAMETER_PH1:
                    if (bSocket == false)
                        ElanTouch.SetPH1(nValue, 1000, nDeviceIndex);
                    else
                        ElanTouch_Socket.SetPH1(nValue, 1000, nDeviceIndex);

                    break;
                case m_nPARAMETER_PH2:
                    if (bSocket == false)
                        ElanTouch.SetPH2(nValue, 1000, nDeviceIndex);
                    else
                        ElanTouch_Socket.SetPH2(nValue, 1000, nDeviceIndex);

                    break;
                case m_nPARAMETER_PH3:
                    if (bSocket == false)
                        ElanTouch.SetPH3(nValue, 1000, nDeviceIndex);
                    else
                        ElanTouch_Socket.SetPH3(nValue, 1000, nDeviceIndex);

                    break;
                case m_nPARAMETER_SUM:
                    if (bSocket == false)
                        ElanTouch.SetSUM(nValue, 1000, nDeviceIndex);
                    else
                        ElanTouch_Socket.SetSUM(nValue, 1000, nDeviceIndex);

                    break;
                default:
                    break;
            }
        }

        public static int GetAnalogParameter(int nParameterType, int nDeviceIndex, bool bSocket)
        {
            int nValue = -1;

            switch (nParameterType)
            {
                case m_nPARAMETER_PH1:
                    if (bSocket == false)
                        nValue = ElanTouch.GetPH1(m_nTimeout, nDeviceIndex);
                    else
                        nValue = ElanTouch_Socket.GetPH1(m_nTimeout, nDeviceIndex);

                    break;
                case m_nPARAMETER_PH2:
                    if (bSocket == false)
                        nValue = ElanTouch.GetPH2(m_nTimeout, nDeviceIndex);
                    else
                        nValue = ElanTouch_Socket.GetPH2(m_nTimeout, nDeviceIndex);

                    break;
                case m_nPARAMETER_PH3:
                    if (bSocket == false)
                        nValue = ElanTouch.GetPH3(m_nTimeout, nDeviceIndex);
                    else
                        nValue = ElanTouch_Socket.GetPH3(m_nTimeout, nDeviceIndex);

                    break;
                case m_nPARAMETER_SUM:
                    if (bSocket == false)
                        nValue = ElanTouch.GetSUM(m_nTimeout, nDeviceIndex);
                    else
                        nValue = ElanTouch_Socket.GetSUM(m_nTimeout, nDeviceIndex);

                    break;
                default:
                    break;
            }

            return nValue;
        }

        public static void SetFWParameter(int nParameterType, int nDeviceIndex, bool bSocket)
        {
            switch (nParameterType)
            {
                case m_nPARAMETER_TPPARAMETER:
                    if (bSocket == false)
                        ElanTouch.SetTPParameter(1000, nDeviceIndex);
                    else
                        ElanTouch_Socket.SetTPParameter(1000, nDeviceIndex);

                    break;
                default:
                    break;
            }
        }

        public static bool SetReK(int nTimeout, int nDeviceIndex, bool bSocket)
        {
            int nResultFlag;

            if (bSocket == false)
            {
                nResultFlag = ElanTouch.ReK(nTimeout, false, nDeviceIndex);

                if (nResultFlag == ElanTouch.TP_SUCCESS)
                    return true;
                else
                    return false;
            }
            else
            {
                nResultFlag = ElanTouch_Socket.ReK(nTimeout, false, nDeviceIndex);

                if (nResultFlag == ElanTouch_Socket.TP_SUCCESS)
                    return true;
                else
                    return false;
            }
        }

        public static short GetProjOption(int nDeviceIndex, bool bSocket)
        {
            if (bSocket == false)
                return (short)ElanTouch.GetProjOption(m_nTimeout, nDeviceIndex);
            else
                return (short)ElanTouch_Socket.GetProjOption(m_nTimeout, nDeviceIndex);
        }

        public static int GetFWIPOption(int nDeviceIndex, bool bSocket)
        {
            int nValue = 0;

            if (bSocket == false)
                ElanTouch.GetFWIPOption(ref nValue, m_nTimeout, nDeviceIndex);
            else
                ElanTouch_Socket.GetFWIPOption(ref nValue, m_nTimeout, nDeviceIndex);

            return nValue;
        }

        public static void SetOption(int nOptionType, short nValue, int nDeviceIndex, bool bSocket)
        {
            switch (nOptionType)
            {
                case m_nOPTION_PROJECTOPTION:
                    if (bSocket == false)
                        ElanTouch.SetProjOption(nValue, 1000, nDeviceIndex);
                    else
                        ElanTouch_Socket.SetProjOption(nValue, 1000, nDeviceIndex);

                    break;
                case m_nOPTION_FWIPOPTION:
                    if (bSocket == false)
                        ElanTouch.SetFWIPOption(nValue, 1000, nDeviceIndex);
                    else
                        ElanTouch_Socket.SetFWIPOption(nValue, 1000, nDeviceIndex);

                    break;
            }
        }

        public static int GetFrameData(int nDataType, ref int[] nDataBuffer_Array, int nRXTraceNumber, int nTXTraceNumber, byte bTraceType, int nTimeout,
                                       int nDeviceIndex, bool bSocket, bool bGetSelf)
        {
            int nResultFlag = 0;
            int nBaseLength = nRXTraceNumber + 1;

            if (bGetSelf == true)
                nBaseLength = nRXTraceNumber + 2;

            switch (nDataType)
            {
                case m_nDATA_BASE:
                    if (bSocket == false)
                        nResultFlag = ElanTouch.GetBase1DArray(nDataBuffer_Array, nRXTraceNumber, nTXTraceNumber, nBaseLength, bTraceType, nTimeout, nDeviceIndex);
                    else
                        nResultFlag = ElanTouch_Socket.GetBase1DArray(nDataBuffer_Array, nRXTraceNumber, nTXTraceNumber, nBaseLength, bTraceType, nTimeout, nDeviceIndex);
                    break;
                case m_nDATA_ADC:
                    if (bSocket == false)
                        nResultFlag = ElanTouch.GetADC1DArray(nDataBuffer_Array, nRXTraceNumber, nTXTraceNumber, nBaseLength, bTraceType, nTimeout, nDeviceIndex);
                    else
                        nResultFlag = ElanTouch_Socket.GetADC1DArray(nDataBuffer_Array, nRXTraceNumber, nTXTraceNumber, nBaseLength, bTraceType, nTimeout, nDeviceIndex);
                    break;
                case m_nDATA_DV:
                    if (bSocket == false)
                        nResultFlag = ElanTouch.GetDV1DArray(nDataBuffer_Array, nRXTraceNumber, nTXTraceNumber, nBaseLength, bTraceType, nTimeout, nDeviceIndex);
                    else
                        nResultFlag = ElanTouch_Socket.GetDV1DArray(nDataBuffer_Array, nRXTraceNumber, nTXTraceNumber, nBaseLength, bTraceType, nTimeout, nDeviceIndex);
                    break;
                case m_nDATA_Noise:
                    if (bSocket == false)
                        nResultFlag = ElanTouch.GetNoise1DArray(nDataBuffer_Array, nRXTraceNumber, nTXTraceNumber, nBaseLength, bTraceType, nTimeout, nDeviceIndex);
                    else
                        nResultFlag = ElanTouch_Socket.GetNoise1DArray(nDataBuffer_Array, nRXTraceNumber, nTXTraceNumber, nBaseLength, bTraceType, nTimeout, nDeviceIndex);
                    break;
                default:
                    break;
            }

            return nResultFlag;
        }

        public static void TransferTestModeViaHID(int nDeviceIndex, bool bSocket)
        {
            if (bSocket == false)
                ElanTouch.TransferTestModeViaHID(nDeviceIndex);
        }

        public static void Disconnect(bool bSocket)
        {
            if (ParamFingerAutoTuning.m_nDisableDisconnect == 1)
                return;

            if (bSocket == false)
                ElanTouch.Disconnect();
            else
                ElanTouch_Socket.Disconnect();
        }

#if _USE_9F07_SOCKET
        public static bool CheckOutReportCallBackIsNull(bool bSocket)
        {
            if (bSocket == false)
            {
                if (null == ElanTouch.m_OutReportCallBack)
                    return true;
                else
                    return false;
            }
            else
            {
                if (null == ElanTouch_Socket.m_OutReportCallBack)
                    return true;
                else
                    return false;
            }
        }

        public static void SetOutReportRegCallback(AppCore cAppCore, bool bSocket)
        {
            if (bSocket == false)
            {
                ElanTouch.m_OutReportCallBack = new ElanTouch.PFUNC_OUT_REPORT_CALLBACK(cAppCore.OutReportCallbackFunc);
                ElanTouch.OutReportRegCallback(ElanTouch.m_OutReportCallBack);
            }
            else
            {
                ElanTouch_Socket.m_OutReportCallBack = new ElanTouch_Socket.PFUNC_OUT_REPORT_CALLBACK(cAppCore.OutReportCallbackFunc);
                ElanTouch_Socket.OutReportRegCallback(ElanTouch_Socket.m_OutReportCallBack);
            }
        }

        public static bool CheckInReportCallBackIsNull(bool bSocket)
        {
            if (bSocket == false)
            {
                if (null == ElanTouch.m_InReportCallBack)
                    return true;
                else
                    return false;
            }
            else
            {
                if (null == ElanTouch_Socket.m_InReportCallBack)
                    return true;
                else
                    return false;
            }
        }

        public static void SetInReportRegCallback(AppCore cAppCore, bool bSocket)
        {
            if (bSocket == false)
            {
                ElanTouch.m_InReportCallBack = new ElanTouch.PFUNC_IN_REPORT_CALLBACK(cAppCore.InReportCallbackFunc);
                ElanTouch.InReportRegCallback(ElanTouch.m_InReportCallBack);
            }
            else
            {
                ElanTouch_Socket.m_InReportCallBack = new ElanTouch_Socket.PFUNC_IN_REPORT_CALLBACK(cAppCore.InReportCallbackFunc);
                ElanTouch_Socket.InReportRegCallback(ElanTouch_Socket.m_InReportCallBack);
            }
        }

        public static int ConnectSocket(int nAddress, int nPort, bool bSocket)
        {
            if (bSocket == false)
                return ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);
            else
                return ElanTouch_Socket.ConnectSocket(nAddress, nPort, ElanTouch_Socket.m_SocketEventCallBack);
        }
#endif
    }
}
