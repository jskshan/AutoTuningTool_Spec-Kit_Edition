
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FingerAutoTuning;

namespace Elan
{
    class ElanCommand_Gen8
    {
        public enum ParameterType
        {
            Write_MS_PH1,
            Write_MS_PH2,
            Write_MS_PH3,
            Write_MS_DFT_NUM,
            Read_MS_PH1,
            Read_MS_PH2,
            Read_MS_PH3,
            Read_MS_DFT_NUM
        }

        private enum CommandType
        {
            WriteControlFlag,
            WriteParamAddrControl,
            WriteParameterOffsetIndex,
            WriteParameterValue,
            ReadParamterValue
        }

        private frmMain m_frmParent = null;
        public static int m_nIN_DATA_LENGTH = 65;

        private int m_nDeviceIndex = 0;
        private bool m_bSocketConnectType = false;

        private class CommandInfo
        {
            public byte byteAuto_Set_Flag_H = 0x00;
            public byte byteAuto_Set_Flag_L = 0x01;
            public byte byteAFE_Param_Index = 0x00;
            public byte byteScan_Mode = 0x00;
            public byte byteScan_Mode_Sub = 0x00;
            public byte byteWR_Value_H = 0x00;
            public byte byteWR_Value_L = 0x00;
        }

        private CommandInfo m_cCommandInfo = new CommandInfo();

        public ElanCommand_Gen8(frmMain frmParent , int nDeviceIndex, bool bSocket)
        {
            m_frmParent = frmParent;
            m_nDeviceIndex = nDeviceIndex;
            m_bSocketConnectType = bSocket;
        }

        private void SetCommandInfo(ParameterType eParameterType, int nValue = 0)
        {
            byte byteHighByte = 0x00;
            byte byteLowByte = 0x00;

            switch (eParameterType)
            {
                case ParameterType.Write_MS_PH1:
                    m_cCommandInfo.byteAuto_Set_Flag_H = 0x00;
                    m_cCommandInfo.byteAuto_Set_Flag_L = 0x01;
                    m_cCommandInfo.byteAFE_Param_Index = 0x9E;
                    m_cCommandInfo.byteScan_Mode = 0x01;
                    m_cCommandInfo.byteScan_Mode_Sub = 0x01;

                    byteHighByte = (byte)((nValue & 0xFF00) >> 8);
                    byteLowByte = (byte)(nValue & 0xFF);

                    m_cCommandInfo.byteWR_Value_H = byteHighByte;
                    m_cCommandInfo.byteWR_Value_L = byteLowByte;
                    break;
                case ParameterType.Write_MS_PH2:
                    m_cCommandInfo.byteAuto_Set_Flag_H = 0x00;
                    m_cCommandInfo.byteAuto_Set_Flag_L = 0x01;
                    m_cCommandInfo.byteAFE_Param_Index = 0x9F;
                    m_cCommandInfo.byteScan_Mode = 0x01;
                    m_cCommandInfo.byteScan_Mode_Sub = 0x01;

                    byteHighByte = (byte)((nValue & 0xFF00) >> 8);
                    byteLowByte = (byte)(nValue & 0xFF);

                    m_cCommandInfo.byteWR_Value_H = byteHighByte;
                    m_cCommandInfo.byteWR_Value_L = byteLowByte;
                    break;
                case ParameterType.Write_MS_PH3:
                    m_cCommandInfo.byteAuto_Set_Flag_H = 0x00;
                    m_cCommandInfo.byteAuto_Set_Flag_L = 0x01;
                    m_cCommandInfo.byteAFE_Param_Index = 0xA0;
                    m_cCommandInfo.byteScan_Mode = 0x01;
                    m_cCommandInfo.byteScan_Mode_Sub = 0x01;

                    byteHighByte = (byte)((nValue & 0xFF00) >> 8);
                    byteLowByte = (byte)(nValue & 0xFF);

                    m_cCommandInfo.byteWR_Value_H = byteHighByte;
                    m_cCommandInfo.byteWR_Value_L = byteLowByte;
                    break;
                case ParameterType.Write_MS_DFT_NUM:
                    m_cCommandInfo.byteAuto_Set_Flag_H = 0x00;
                    m_cCommandInfo.byteAuto_Set_Flag_L = 0x01;
                    m_cCommandInfo.byteAFE_Param_Index = 0x9C;
                    m_cCommandInfo.byteScan_Mode = 0x01;
                    m_cCommandInfo.byteScan_Mode_Sub = 0x01;

                    byteHighByte = (byte)((nValue & 0xFF00) >> 8);
                    byteLowByte = (byte)(nValue & 0xFF);

                    m_cCommandInfo.byteWR_Value_H = byteHighByte;
                    m_cCommandInfo.byteWR_Value_L = byteLowByte;
                    break;
                case ParameterType.Read_MS_PH1:
                    m_cCommandInfo.byteAuto_Set_Flag_H = 0x00;
                    m_cCommandInfo.byteAuto_Set_Flag_L = 0x01;
                    m_cCommandInfo.byteAFE_Param_Index = 0x9E;
                    m_cCommandInfo.byteScan_Mode = 0x01;
                    m_cCommandInfo.byteScan_Mode_Sub = 0x01;
                    break;
                case ParameterType.Read_MS_PH2:
                    m_cCommandInfo.byteAuto_Set_Flag_H = 0x00;
                    m_cCommandInfo.byteAuto_Set_Flag_L = 0x01;
                    m_cCommandInfo.byteAFE_Param_Index = 0x9F;
                    m_cCommandInfo.byteScan_Mode = 0x01;
                    m_cCommandInfo.byteScan_Mode_Sub = 0x01;
                    break;
                case ParameterType.Read_MS_PH3:
                    m_cCommandInfo.byteAuto_Set_Flag_H = 0x00;
                    m_cCommandInfo.byteAuto_Set_Flag_L = 0x01;
                    m_cCommandInfo.byteAFE_Param_Index = 0xA0;
                    m_cCommandInfo.byteScan_Mode = 0x01;
                    m_cCommandInfo.byteScan_Mode_Sub = 0x01;
                    break;
                case ParameterType.Read_MS_DFT_NUM:
                    m_cCommandInfo.byteAuto_Set_Flag_H = 0x00;
                    m_cCommandInfo.byteAuto_Set_Flag_L = 0x01;
                    m_cCommandInfo.byteAFE_Param_Index = 0x9C;
                    m_cCommandInfo.byteScan_Mode = 0x01;
                    m_cCommandInfo.byteScan_Mode_Sub = 0x01;
                    break;
                default:
                    break;
            }
        }

        public void SendWriteCommand(ParameterType eParameterType, int nValue = 0)
        {
            SetCommandInfo(eParameterType, nValue);

            SendCommand(CommandType.WriteParamAddrControl);
            SendCommand(CommandType.WriteParameterOffsetIndex);
            SendCommand(CommandType.WriteParameterValue);
            SendCommand(CommandType.WriteControlFlag);
        }

        public void SendRawDataCountCommand()
        {
            byte[] arrbyteCommand = new byte[] { 0x54, 0xBC, 0x00, 0x02 };
            ElanTouchSwitch.SendDevCommand(arrbyteCommand, m_nDeviceIndex, m_bSocketConnectType);
            Thread.Sleep(10);
        }

        public bool CheckEnterTestMode(int nTimeout = 1000)
        {
            int nRetryCount = 50;
            bool bEnterTestMode = false;

            byte[] arrbyteCommand = new byte[] { 0x53, 0xB5, 0x00, 0x01 };

            ElanTouchSwitch.SendDevCommand(arrbyteCommand, m_nDeviceIndex, m_bSocketConnectType);

            while (nRetryCount > 0)
            {
                byte[] arrbyteData = new byte[m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouchSwitch.ReadDevData(arrbyteData, m_nDeviceIndex, m_bSocketConnectType);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                {
                    m_frmParent.DebugLogOutput(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(nTimeout);
                    break;
                }

                string sGetACK = "Get ACK :";
                for (int nByteIndex = 0; nByteIndex < arrbyteData.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", arrbyteData[nByteIndex].ToString("X2"));
                m_frmParent.DebugLogOutput(sGetACK);

                if (arrbyteData[0] == 0x52 && arrbyteData[1] == 0xB5)
                {
                    if (arrbyteData[3] == 0x01)
                    {
                        bEnterTestMode = true;
                        break;
                    }
                }

                nRetryCount--;
            }

            if (bEnterTestMode == true)
                return true;
            else
                return false;
        }

        public int SendReadCommand(ParameterType eParameterType, int nTimeout = 1000)
        {
            int nGetValue = -1;
            int nRetryCount = 50;

            SetCommandInfo(eParameterType);

            SendCommand(CommandType.WriteParamAddrControl);
            SendCommand(CommandType.WriteParameterOffsetIndex);
            SendCommand(CommandType.ReadParamterValue);

            while (nRetryCount > 0)
            {
                byte[] arrbyteData = new byte[m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouchSwitch.ReadDevData(arrbyteData, m_nDeviceIndex, m_bSocketConnectType);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                {
                    m_frmParent.DebugLogOutput(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(nTimeout);
                    break;
                }

                string sGetACK = "Get ACK :";
                for (int nByteIndex = 0; nByteIndex < arrbyteData.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", arrbyteData[nByteIndex].ToString("X2"));
                m_frmParent.DebugLogOutput(sGetACK);

                if (arrbyteData[0] == 0x52)
                {
                    if (eParameterType == ParameterType.Read_MS_PH1 ||
                        eParameterType == ParameterType.Read_MS_PH2 ||
                        eParameterType == ParameterType.Read_MS_PH3 ||
                        eParameterType == ParameterType.Read_MS_DFT_NUM)
                    {
                        if (arrbyteData[1] == 0xB4)
                        {
                            nGetValue = (ushort)(arrbyteData[2] << 8 | arrbyteData[3]);
                            break;
                        }
                    }
                }

                nRetryCount--;
            }

            Thread.Sleep(20);
            return nGetValue;
        }

        private void SendCommand(CommandType eCommandType)
        {
            byte[] arrbyteCommand;

            switch(eCommandType)
            {
                case CommandType.WriteControlFlag:
                    arrbyteCommand = new byte[] { 0x54, 0xF0, m_cCommandInfo.byteAuto_Set_Flag_H, m_cCommandInfo.byteAuto_Set_Flag_L };
                    ElanTouchSwitch.SendDevCommand(arrbyteCommand, m_nDeviceIndex, m_bSocketConnectType);
                    Thread.Sleep(20);
                    break;
                case CommandType.WriteParamAddrControl:
                    arrbyteCommand = new byte[] { 0x54, 0xF2, m_cCommandInfo.byteScan_Mode, m_cCommandInfo.byteScan_Mode_Sub };
                    ElanTouchSwitch.SendDevCommand(arrbyteCommand, m_nDeviceIndex, m_bSocketConnectType);
                    Thread.Sleep(20);
                    break;
                case CommandType.WriteParameterOffsetIndex:
                    arrbyteCommand = new byte[] { 0x54, 0xF1, 0x00, m_cCommandInfo.byteAFE_Param_Index };
                    ElanTouchSwitch.SendDevCommand(arrbyteCommand, m_nDeviceIndex, m_bSocketConnectType);
                    Thread.Sleep(20);
                    break;
                case CommandType.WriteParameterValue:
                    arrbyteCommand = new byte[] { 0x54, 0xF3, m_cCommandInfo.byteWR_Value_H, m_cCommandInfo.byteWR_Value_L };
                    ElanTouchSwitch.SendDevCommand(arrbyteCommand, m_nDeviceIndex, m_bSocketConnectType);
                    Thread.Sleep(20);
                    break;
                case CommandType.ReadParamterValue:
                    arrbyteCommand = new byte[] { 0x53, 0xB4, 0x00, 0x01 };
                    ElanTouchSwitch.SendDevCommand(arrbyteCommand, m_nDeviceIndex, m_bSocketConnectType);
                    break;
                default:
                    break;
            }
        }
    }
}
*/