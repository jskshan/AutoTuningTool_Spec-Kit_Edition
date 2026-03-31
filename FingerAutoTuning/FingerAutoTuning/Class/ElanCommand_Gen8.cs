using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using FingerAutoTuning;
using FingerAutoTuningParameter;

namespace Elan
{
    class ElanCommand_Gen8
    {
        public class SendCommandInfo
        {
            public List<CommandInfo> cCommandInfo_List = new List<CommandInfo>();
        }

        public class CommandInfo
        {
            public byte[] byteCommand_Array;
            public int nDelayTime = ParamFingerAutoTuning.m_nGen8SendCommandDelayTime;
        }

        public enum ParameterType
        {
            //Mutual
            _MS_PH1,
            _MS_PH2,
            _MS_PH3,
            _MS_AFE_DFT_NUM,
            _MS_AFE_SP_NUM,
            _MS_AFE_EFFECT_NUM,
            PKT_WC,
            //Raw ADC Sweep
            _MS_BIN_FIRCOEF_SEL_TAP_NUM,
            _MS_IQ_BSH_GP0_GP1,
            _MS_ANA_TP_CTL_01,
            _MS_ANA_TP_CTL_01_2,
            _MS_ANA_CTL_04,
            _MS_ANA_CTL_04_2,
            _MS_ANA_TP_CTL_06,
            _MS_ANA_TP_CTL_06_2,
            _MS_ANA_TP_CTL_07,
            //Self
            _SELF_PH1,
            _SELF_PH2E_LAT,
            _SELF_PH2_LAT,
            _SELF_PH2,
            _SELF_DFT_NUM,
            _SELF_SP_NUM,
            _SELF_EFFECT_NUM,
            _SELF_PKT_WC_L,
            _SELF_BSH_ADC_TP_NUM,
            _SELF_EFFECT_FW_SET_COEF_NUM,
            _SELF_DFT_NUM_IQ_FIR_CTL,
            _SELF_ANA_TP_CTL_01,
            _SELF_ANA_TP_CTL_00,
            _SELF_IQ_BSH_GP0_GP1
        }

        public class DataType
        {
            public const byte byteRead_8009_AFE = 0x67;
            public const byte byteWrite_8009_AFE = 0x68;
            public const byte byteRead_902_DataPath = 0x69;
            public const byte byteWrite_902_DataPath = 0x6A;
        }

        public class ParameterClass
        {
            public const byte byteMutual_AFE_Para_Addr = 0x11;
            public const byte byteNoTX_AFE_Para_Addr = 0x12;
            public const byte byteOBL_AFE_Para_Addr = 0x13;
            public const byte byteScript_Mutual_Scan_Addr = 0x14;
            public const byte byteScript_NoTX_Scan_Addr = 0x15;
            public const byte byteScript_OBL_Scan_Addr = 0x16;
            public const byte byteSelf_RX_AFE_Para_Addr = 0x21;
            public const byte byteSelf_TX_AFE_Para_Addr = 0x22;
            public const byte byteScript_Self_RX_Scan_Addr = 0x23;
            public const byte byteScript_Self_TX_Scan_Addr = 0x24;
            public const byte byteMutual_DataPath_Para = 0x11;
            public const byte byteNoTX_DataPath_Para = 0x12;
            public const byte byteOBL_DataPath_Para = 0x13;
            public const byte byteSelf_RX_DataPath_Para = 0x21;
            public const byte byteSelf_TX_DataPath_Para = 0x22;
        }

        public class WriteCommandInfo
        {
            public string sParameterName = "";
            public byte byteType = 0x00;
            public byte byteClass = 0x00;
            public byte byteAddress1_H = 0x00;
            public byte byteAddress1_L = 0x00;
            public byte byteAddress2_H = 0x00;
            public byte byteAddress2_L = 0x00;
            public byte byteValue1_H = 0x00;
            public byte byteValue1_L = 0x00;
            public byte byteValue2_H = 0x00;
            public byte byteValue2_L = 0x00;
        }

        public class ReadCommandInfo
        {
            public string sParameterName = "";
            public byte byteType = 0x00;
            public byte byteClass = 0x00;
            public byte byteAddress1_H = 0x00;
            public byte byteAddress1_L = 0x00;
            public byte byteAddress2_H = 0x00;
            public byte byteAddress2_L = 0x00;
        }

        private frmMain m_cfrmParent = null;
        public static int m_nIN_DATA_LENGTH = 65;

        private int m_nDeviceIndex = 0;
        private bool m_bSocketConnectType = false;
        private TraceType m_eTraceType = TraceType.ALL;

        public WriteCommandInfo m_cWriteCommandInfo = new WriteCommandInfo();
        public ReadCommandInfo m_cReadCommandInfo = new ReadCommandInfo();

        private string m_sErrorMessage = "";

        public ElanCommand_Gen8(frmMain cfrmParent, int nDeviceIndex, bool bSocket, TraceType eTraceType = TraceType.ALL)
        {
            m_cfrmParent = cfrmParent;
            m_nDeviceIndex = nDeviceIndex;
            m_bSocketConnectType = bSocket;
            m_eTraceType = eTraceType;
        }

        private void SetWriteCommandInfo(ParameterType eParameterType, int nValue1 = 0, int nValue2 = 0)
        {
            m_cWriteCommandInfo = new WriteCommandInfo();

            byte byteHighByte = 0x00;
            byte byteLowByte = 0x00;

            switch (eParameterType)
            {
                case ParameterType._MS_PH1:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_PH1 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_PH1 & 0x00FF);
                    break;
                case ParameterType._MS_PH2:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_PH2 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_PH2 & 0x00FF);
                    break;
                case ParameterType._MS_PH3:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_PH3 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_PH3 & 0x00FF);
                    break;
                case ParameterType._MS_AFE_DFT_NUM:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_DFT_NUM & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_DFT_NUM & 0x00FF);
                    break;
                case ParameterType._MS_AFE_SP_NUM:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_SP_NUM & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_SP_NUM & 0x00FF);
                    break;
                case ParameterType._MS_AFE_EFFECT_NUM:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_EFFECT_NUM & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_EFFECT_NUM & 0x00FF);
                    break;
                case ParameterType.PKT_WC:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteScript_Mutual_Scan_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_PKT_WC & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_PKT_WC & 0x00FF);
                    break;
                case ParameterType._MS_BIN_FIRCOEF_SEL_TAP_NUM:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_902_DataPath;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_DataPath_Para;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_BIN_FIRCOEF_SEL_TAP_NUM & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_BIN_FIRCOEF_SEL_TAP_NUM & 0x00FF);
                    break;
                case ParameterType._MS_IQ_BSH_GP0_GP1:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_902_DataPath;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_DataPath_Para;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_IQ_BSH_GP0_GP1 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_IQ_BSH_GP0_GP1 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_TP_CTL_01:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_01 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_01 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_TP_CTL_01_2:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_01_2 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_01_2 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_04:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_CTL_04 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_CTL_04 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_04_2:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_CTL_04_2 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_CTL_04_2 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_TP_CTL_06:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_06 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_06 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_TP_CTL_06_2:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_06_2 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_06_2 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_TP_CTL_07:
                    m_cWriteCommandInfo.sParameterName = eParameterType.ToString();
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;
                    m_cWriteCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cWriteCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_07 & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_07 & 0x00FF);
                    break;
                case ParameterType._SELF_PH1:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_PH1", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x9E;
                    break;
                case ParameterType._SELF_PH2E_LAT:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_PH2E_LAT", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0xA7;
                    break;
                case ParameterType._SELF_PH2_LAT:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_PH2_LAT", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0xA8;
                    break;
                case ParameterType._SELF_PH2:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_PH2", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x9F;
                    break;
                case ParameterType._SELF_DFT_NUM:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_DFT_NUM", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x9C;
                    break;
                case ParameterType._SELF_SP_NUM:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_SP_NUM", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x9A;
                    break;
                case ParameterType._SELF_EFFECT_NUM:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_EFFECT_NUM", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x9B;
                    break;
                case ParameterType._SELF_PKT_WC_L:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_PKT_WC_L", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteScript_Self_RX_Scan_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteScript_Self_TX_Scan_Addr;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x04;
                    break;
                case ParameterType._SELF_BSH_ADC_TP_NUM:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_BSH_ADC_TP_NUM", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_902_DataPath;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x0E;
                    break;
                case ParameterType._SELF_EFFECT_FW_SET_COEF_NUM:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_EFFECT_FW_SET_COEF_NUM", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_902_DataPath;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x0F;
                    break;
                case ParameterType._SELF_DFT_NUM_IQ_FIR_CTL:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_DFT_NUM_IQ_FIR_CTL", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_902_DataPath;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x12;
                    break;
                case ParameterType._SELF_ANA_TP_CTL_01:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_ANA_TP_CTL_01", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x71;
                    break;
                case ParameterType._SELF_ANA_TP_CTL_00:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_ANA_TP_CTL_00", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x70;
                    break;
                case ParameterType._SELF_IQ_BSH_GP0_GP1:
                    m_cWriteCommandInfo.sParameterName = string.Format("_SELF_{0}_IQ_BSH_GP0_GP1", m_eTraceType.ToString());
                    m_cWriteCommandInfo.byteType = DataType.byteWrite_902_DataPath;

                    if (m_eTraceType == TraceType.RX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cWriteCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cWriteCommandInfo.byteAddress2_H = 0x00;
                    m_cWriteCommandInfo.byteAddress2_L = 0x10;
                    break;
                default:
                    break;
            }

            byteHighByte = (byte)((nValue1 & 0xFF00) >> 8);
            byteLowByte = (byte)(nValue1 & 0xFF);

            m_cWriteCommandInfo.byteValue1_H = byteHighByte;
            m_cWriteCommandInfo.byteValue1_L = byteLowByte;

            byteHighByte = (byte)((nValue2 & 0xFF00) >> 8);
            byteLowByte = (byte)(nValue2 & 0xFF);

            m_cWriteCommandInfo.byteValue2_H = byteHighByte;
            m_cWriteCommandInfo.byteValue2_L = byteLowByte;
        }

        private void SetReadCommandInfo(ParameterType eParameterType)
        {
            m_cReadCommandInfo = new ReadCommandInfo();

            switch (eParameterType)
            {
                case ParameterType._MS_PH1:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_PH1 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_PH1 & 0x00FF);
                    break;
                case ParameterType._MS_PH2:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_PH2 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_PH2 & 0x00FF);
                    break;
                case ParameterType._MS_PH3:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_PH3 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_PH3 & 0x00FF);
                    break;
                case ParameterType._MS_AFE_DFT_NUM:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_DFT_NUM & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_DFT_NUM & 0x00FF);
                    break;
                case ParameterType._MS_AFE_SP_NUM:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_SP_NUM & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_SP_NUM & 0x00FF);
                    break;
                case ParameterType._MS_AFE_EFFECT_NUM:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_EFFECT_NUM & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_AFE_EFFECT_NUM & 0x00FF);
                    break;
                case ParameterType.PKT_WC:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteScript_Mutual_Scan_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_PKT_WC & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_PKT_WC & 0x00FF);
                    break;
                case ParameterType._MS_BIN_FIRCOEF_SEL_TAP_NUM:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_902_DataPath;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_DataPath_Para;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_BIN_FIRCOEF_SEL_TAP_NUM & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_BIN_FIRCOEF_SEL_TAP_NUM & 0x00FF);
                    break;
                case ParameterType._MS_IQ_BSH_GP0_GP1:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_902_DataPath;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_DataPath_Para;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_IQ_BSH_GP0_GP1 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_IQ_BSH_GP0_GP1 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_TP_CTL_01:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_01 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_01 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_TP_CTL_01_2:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_01_2 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_01_2 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_04:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_CTL_04 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_CTL_04 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_04_2:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_CTL_04_2 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_CTL_04_2 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_TP_CTL_06:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_06 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_06 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_TP_CTL_06_2:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_06_2 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_06_2 & 0x00FF);
                    break;
                case ParameterType._MS_ANA_TP_CTL_07:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;
                    m_cReadCommandInfo.byteClass = ParameterClass.byteMutual_AFE_Para_Addr;
                    m_cReadCommandInfo.byteAddress2_H = (byte)((ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_07 & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress2_L = (byte)(ParamFingerAutoTuning.m_nRelativeAddress_MS_ANA_TP_CTL_07 & 0x00FF);
                    break;
                case ParameterType._SELF_PH1:
                    m_cReadCommandInfo.sParameterName = string.Format("_SELF_{0}_PH1", m_eTraceType.ToString());
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x9E;
                    break;
                case ParameterType._SELF_PH2E_LAT:
                    m_cReadCommandInfo.sParameterName = string.Format("_SELF_{0}_PH2E_LAT", m_eTraceType.ToString());
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0xA7;
                    break;
                case ParameterType._SELF_PH2_LAT:
                    m_cReadCommandInfo.sParameterName = string.Format("_SELF_{0}_PH2_LAT", m_eTraceType.ToString());
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0xA8;
                    break;
                case ParameterType._SELF_PH2:
                    m_cReadCommandInfo.sParameterName = string.Format("_SELF_{0}_PH2", m_eTraceType.ToString());
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x9F;
                    break;
                case ParameterType._SELF_DFT_NUM:
                    m_cReadCommandInfo.sParameterName = string.Format("_SELF_{0}_DFT_NUM", m_eTraceType.ToString());
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x9C;
                    break;
                case ParameterType._SELF_SP_NUM:
                    m_cReadCommandInfo.sParameterName = string.Format("_SELF_{0}_SP_NUM", m_eTraceType.ToString());
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x9A;
                    break;
                case ParameterType._SELF_EFFECT_NUM:
                    m_cReadCommandInfo.sParameterName = string.Format("_SELF_{0}_EFFECT_NUM", m_eTraceType.ToString());
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x9B;
                    break;
                case ParameterType._SELF_PKT_WC_L:
                    m_cReadCommandInfo.sParameterName = string.Format("_SELF_{0}_PKT_WC_L", m_eTraceType.ToString());
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteScript_Self_RX_Scan_Addr;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteScript_Self_TX_Scan_Addr;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x04;
                    break;
                case ParameterType._SELF_BSH_ADC_TP_NUM:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_902_DataPath;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x0E;
                    break;
                case ParameterType._SELF_EFFECT_FW_SET_COEF_NUM:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_902_DataPath;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x0F;
                    break;
                case ParameterType._SELF_DFT_NUM_IQ_FIR_CTL:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_902_DataPath;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x12;
                    break;
                case ParameterType._SELF_ANA_TP_CTL_01:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x71;
                    break;
                case ParameterType._SELF_ANA_TP_CTL_00:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_8009_AFE;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x70;
                    break;
                case ParameterType._SELF_IQ_BSH_GP0_GP1:
                    m_cReadCommandInfo.sParameterName = eParameterType.ToString();
                    m_cReadCommandInfo.byteType = DataType.byteRead_902_DataPath;

                    if (m_eTraceType == TraceType.RX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_RX_DataPath_Para;
                    else if (m_eTraceType == TraceType.TX)
                        m_cReadCommandInfo.byteClass = ParameterClass.byteSelf_TX_DataPath_Para;

                    m_cReadCommandInfo.byteAddress2_H = 0x00;
                    m_cReadCommandInfo.byteAddress2_L = 0x10;
                    break;
                default:
                    break;
            }
        }

        public void RunSendSelfCalibrationParameter(int nNCPValue, int nNCNValue, bool bSetCALValue, int nCALValue, int nStartTrace = -1, int nStartTraceNumber = 0, int nEndTraceNumber = 0)
        {
            int nStartAddressByte = 0x07;
            int nEndAddressByte = 0x30;

            int nCALEnableStartAddress = nStartTrace + (nStartTraceNumber - 1) + 0x07;
            int nCALEnableEndAddress = nStartTrace + (nEndTraceNumber - 1) + 0x07;

            for (int nAddressIndex = nStartAddressByte; nAddressIndex <= nEndAddressByte; nAddressIndex++)
            {
                int nSetNCPValue = nNCPValue << 9;
                int nSetNCNValue = nNCNValue << 4;

                int nSetCALValue = 0;

                if (bSetCALValue == true)
                {
                    if (nStartTrace >= 0)
                    {
                        if (nAddressIndex >= nCALEnableStartAddress && nAddressIndex <= nCALEnableEndAddress)
                            nSetCALValue = nCALValue;
                        else
                            nSetCALValue = 0;
                    }
                    else
                        nSetCALValue = nCALValue;
                }

                int nMixValue = nSetNCPValue + nSetNCNValue + nSetCALValue;

                int nHighByte = (nMixValue & 0xFF00) >> 8;
                int nLowByte = nMixValue & 0x00FF;

                if (m_cfrmParent.m_bExecute == false)
                    return;

                byte byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;

                if (m_eTraceType == TraceType.RX)
                    byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                else if (m_eTraceType == TraceType.TX)
                    byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                byte[] byteCommand_Array = new byte[] { 0x68, byteClass, 0x00, 0x00, 0x00, (byte)nAddressIndex, 0x00, 0x00, (byte)nHighByte, (byte)nLowByte };
                SendSelfCalibrationParameterCommand(byteCommand_Array);
            }
        }

        public void SendSelfCalibrationParameterCommand(byte[] byteCommand_Array)
        {
            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);
            Thread.Sleep(ParamFingerAutoTuning.m_nGen8SendCommandDelayTime);

            string sSendCommand = "";

            for (int nByteIndex = 0; nByteIndex < byteCommand_Array.Length; nByteIndex++)
                sSendCommand += string.Format(" {0}", byteCommand_Array[nByteIndex].ToString("X2"));

            m_cfrmParent.OutputDebugLog(string.Format("Send Self Calibration Parameter Command : {0}", sSendCommand));

            string sMessage = string.Format("-Send Self Calibration Parameter Command : {0}", sSendCommand);
            m_cfrmParent.OutputMessage(sMessage);
        }

        public void RunSendSelfCalibrationParameterByFile(string sFilePath, bool bSetCALValue, int nCALValue, int nStartTrace = -1, int nStartTraceNumber = 0, int nEndTraceNumber = 0)
        {
            if (File.Exists(sFilePath) == false)
                return;

            List<int> n_MS_MM_RX0_List = new List<int>();

            // Read the file and display it line by line.
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);
            string sLine = "";

            while ((sLine = srFile.ReadLine()) != null)
            {
                string[] sData_Array = sLine.Split(',');

                if (sData_Array[0].IndexOf("0x") == 0)
                {
                    string sValue = sData_Array[0].Replace("0x", "");
                    sValue = sValue.Trim();

                    int nValue = Convert.ToInt32(sValue, 16);

                    n_MS_MM_RX0_List.Add(nValue);
                }
            }

            int nStartAddressByte = 0x07;
            int nEndAddressByte = 0x30;

            int nCALEnableStartAddress = nStartTrace + (nStartTraceNumber - 1) + 0x07;
            int nCALEnableEndAddress = nStartTrace + (nEndTraceNumber - 1) + 0x07;

            for (int nValueIndex = 0; nValueIndex < n_MS_MM_RX0_List.Count; nValueIndex++)
            {
                int nAddressIndex = nStartAddressByte + nValueIndex;

                if (nAddressIndex > nEndAddressByte)
                    break;

                int nSetCALValue = 0;

                if (bSetCALValue == true)
                {
                    if (nStartTrace >= 0)
                    {
                        if (nAddressIndex >= nCALEnableStartAddress && nAddressIndex <= nCALEnableEndAddress)
                            nSetCALValue = nCALValue;
                        else
                            nSetCALValue = 0;
                    }
                    else
                        nSetCALValue = nCALValue;
                }

                int nMixValue = n_MS_MM_RX0_List[nValueIndex] + nSetCALValue;

                int nHighByte = (nMixValue & 0xFF00) >> 8;
                int nLowByte = nMixValue & 0x00FF;

                if (m_cfrmParent.m_bExecute == false)
                    return;

                byte byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;

                if (m_eTraceType == TraceType.RX)
                    byteClass = ParameterClass.byteSelf_RX_AFE_Para_Addr;
                else if (m_eTraceType == TraceType.TX)
                    byteClass = ParameterClass.byteSelf_TX_AFE_Para_Addr;

                byte[] byteCommand_Array = new byte[] { 0x68, byteClass, 0x00, 0x00, 0x00, (byte)nAddressIndex, 0x00, 0x00, (byte)nHighByte, (byte)nLowByte };
                SendSelfCalibrationParameterCommand(byteCommand_Array);
            }
        }

        public void SendWriteCommand(ParameterType eParameterType, int nValue1 = 0, int nValue2 = 0)
        {
            SetWriteCommandInfo(eParameterType, nValue1, nValue2);
            SendWriteCommand();
        }

        public void SendRawDataCountCommand()
        {
            //byte[] byteCommand_Array = new byte[] { 0x54, 0xBC, 0x00, 0x02 };
            byte[] byteCommand_Array = new byte[] 
            { 
                0x54, 
                0xBC, 
                0x00, 
                (byte)ParamFingerAutoTuning.m_nGen8ReadBulkRAMDataValue 
            };
            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);
            Thread.Sleep(10);
        }

        public bool CheckEnterTestMode(int nTimeout = 1000)
        {
            int nRetryCount = 50;
            bool bEnterTestMode = false;

            byte[] byteCommand_Array = new byte[] 
            { 
                0x53, 
                0xB5, 
                0x00, 
                0x01 
            };

            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

            while (nRetryCount > 0)
            {
                byte[] byteData_Array = new byte[m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                {
                    m_cfrmParent.OutputDebugLog(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(nTimeout);
                    break;
                }

                string sGetACK = "Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                m_cfrmParent.OutputDebugLog(sGetACK);

                if (byteData_Array[0] == 0x52 && byteData_Array[1] == 0xB5)
                {
                    if (byteData_Array[3] == 0x01)
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

        public bool ReadData(ref int nValue_1, ref int nValue_2, ParameterType eParameterType, int nTimeout = 1000)
        {
            nValue_1 = -1;
            nValue_2 = -1;

            bool bResultFlag = false;
            int nRetryCount = 50;

            SetReadCommandInfo(eParameterType);
            SendReadCommand();

            while (nRetryCount > 0)
            {
                byte[] byteData_Array = new byte[m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType, ParamFingerAutoTuning.m_nGen8GetACKTimeout);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                {
                    m_cfrmParent.OutputDebugLog(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(nTimeout);
                    bResultFlag = false;
                    break;
                }

                string sGetACK = "Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                m_cfrmParent.OutputDebugLog(sGetACK);

                if (byteData_Array[0] == m_cReadCommandInfo.byteType &&
                    byteData_Array[1] == m_cReadCommandInfo.byteClass &&
                    byteData_Array[2] == m_cReadCommandInfo.byteAddress1_H && byteData_Array[3] == m_cReadCommandInfo.byteAddress1_L &&
                    byteData_Array[4] == m_cReadCommandInfo.byteAddress2_H && byteData_Array[5] == m_cReadCommandInfo.byteAddress2_L)
                {
                    nValue_1 = (ushort)(byteData_Array[6] << 8 | byteData_Array[7]);
                    nValue_2 = (ushort)(byteData_Array[8] << 8 | byteData_Array[9]);
                    bResultFlag = true;
                    break;
                }

                nRetryCount--;
            }

            Thread.Sleep(20);
            return bResultFlag;
        }

        private void SendWriteCommand()
        {
            string sParameterName = m_cWriteCommandInfo.sParameterName;
            byte byteType = m_cWriteCommandInfo.byteType;
            byte byteClass = m_cWriteCommandInfo.byteClass;
            byte byteAddress1_H = m_cWriteCommandInfo.byteAddress1_H;
            byte byteAddress1_L = m_cWriteCommandInfo.byteAddress1_L;
            byte byteAddress2_H = m_cWriteCommandInfo.byteAddress2_H;
            byte byteAddress2_L = m_cWriteCommandInfo.byteAddress2_L;
            byte byteValue1_H = m_cWriteCommandInfo.byteValue1_H;
            byte byteValue1_L = m_cWriteCommandInfo.byteValue1_L;
            byte byteValue2_H = m_cWriteCommandInfo.byteValue2_H;
            byte byteValue2_L = m_cWriteCommandInfo.byteValue2_L;

            byte[] byteCommand_Array = new byte[] 
            { 
                byteType, 
                byteClass, 
                byteAddress1_H, 
                byteAddress1_L, 
                byteAddress2_H, 
                byteAddress2_L,
                byteValue1_H, 
                byteValue1_L, 
                byteValue2_H, 
                byteValue2_L 
            };
            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

            /*
            string sMessage = string.Format("-Set {0}(Word1=0x{1}{2}, Word2=0x{3}{4})", sParameterName, byteAddress1_H.ToString("X2"), byteAddress1_L.ToString("X2"),
                                            byteAddress2_H.ToString("X2"), byteAddress2_L.ToString("X2"));
            m_cfrmParent.MessageOutput(sMessage);
            */

            string sSendCommand = "";

            for (int nByteIndex = 0; nByteIndex < byteCommand_Array.Length; nByteIndex++)
                sSendCommand += string.Format(" {0}", byteCommand_Array[nByteIndex].ToString("X2"));

            m_cfrmParent.OutputDebugLog(string.Format("Send Write Command :{0}", sSendCommand));

            Thread.Sleep(20);
        }

        private void SendReadCommand()
        {
            string sParameterName = m_cReadCommandInfo.sParameterName;
            byte byteType = m_cReadCommandInfo.byteType;
            byte byteClass = m_cReadCommandInfo.byteClass;
            byte byteAddress1_H = m_cReadCommandInfo.byteAddress1_H;
            byte byteAddress1_L = m_cReadCommandInfo.byteAddress1_L;
            byte byteAddress2_H = m_cReadCommandInfo.byteAddress2_H;
            byte byteAddress2_L = m_cReadCommandInfo.byteAddress2_L;

            byte[] byteCommand_Array = new byte[] 
            { 
                byteType, 
                byteClass, 
                byteAddress1_H, 
                byteAddress1_L, 
                byteAddress2_H, 
                byteAddress2_L,
                0x00, 
                0x00, 
                0x00, 
                0x00 
            };

            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);
            //Thread.Sleep(20);

            string sSendCommand = "";

            for (int nByteIndex = 0; nByteIndex < byteCommand_Array.Length; nByteIndex++)
                sSendCommand += string.Format(" {0}", byteCommand_Array[nByteIndex].ToString("X2"));

            m_cfrmParent.OutputDebugLog(string.Format("Send Read Command :{0}", sSendCommand));
        }

        public string GetErrorMessage()
        {
            return m_sErrorMessage;
        }

        public void RunUserDefinedCommandScriptFlow(SendCommandInfo cSendCommandInfo)
        {
            foreach (CommandInfo cCommandInfo in cSendCommandInfo.cCommandInfo_List)
            {
                if (m_cfrmParent.m_bExecute == false)
                    return;

                SendUserDefinedCommand(cCommandInfo);
            }
        }

        private void SendUserDefinedCommand(CommandInfo cCommandInfo)
        {
            ElanTouchSwitch.SendDevCommand(cCommandInfo.byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);
            Thread.Sleep(cCommandInfo.nDelayTime);

            string sSendCommand = "";

            for (int nByteIndex = 0; nByteIndex < cCommandInfo.byteCommand_Array.Length; nByteIndex++)
                sSendCommand += string.Format(" {0}", cCommandInfo.byteCommand_Array[nByteIndex].ToString("X2"));

            m_cfrmParent.OutputDebugLog(string.Format("Send User Defined Command : {0}", sSendCommand));

            string sMessage = string.Format("-Send Command : {0}", sSendCommand);
            m_cfrmParent.OutputMessage(sMessage);
        }

        public bool CheckUserDefinedFile(List<frmMain.FlowStep> cFlowStep_List, string sFilePath)
        {
            List<string> sScriptData_List = new List<string>();

            // Read the file and display it line by line.
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            string sLine = "";

            while ((sLine = srFile.ReadLine()) != null)
                sScriptData_List.Add(sLine);

            srFile.Close();

            foreach (frmMain.FlowStep cFlowStep in cFlowStep_List)
            {
                SendCommandInfo cSendCommandInfo = null;

                bool bCheckResultFlag = CheckFormat(ref cSendCommandInfo, cFlowStep, 0, sScriptData_List);

                if (bCheckResultFlag == false)
                    return false;
            }

            return true;
        }

        public bool LoadUserDefinedCommandScript(ref SendCommandInfo cSendCommandInfo, frmMain.FlowStep cFlowStep, int nRecordDataIndex, string sFilePath)
        {
            List<string> sScriptData_List = new List<string>();

            // Read the file and display it line by line.
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            string sLine = "";

            while ((sLine = srFile.ReadLine()) != null)
                sScriptData_List.Add(sLine);

            srFile.Close();

            bool bCheckResult = CheckFormat(ref cSendCommandInfo, cFlowStep, nRecordDataIndex, sScriptData_List, true);

            return bCheckResult;
        }

        private bool CheckFormat(ref SendCommandInfo cSendCommandInfo, frmMain.FlowStep cFlowStep, int nRecordDataIndex, List<string> sScriptData_List,
                                 bool bGetSendCommandInfo = false)
        {
            bool bGetSection = false;
            bool bCreateCommandInfo = false;
            string sMainStep = StringConvert.m_dictMainStepMappingTable[cFlowStep.m_eStep];
            CommandInfo cCommandInfo = null;

            for (int nLineIndex = 0; nLineIndex < sScriptData_List.Count; nLineIndex++)
            {
                string sLineData = sScriptData_List[nLineIndex];

                if (sLineData == "")
                    continue;

                if (sLineData.Substring(0, 1) == "[" && sLineData.Substring(sLineData.Length - 1, 1) == "]")
                {
                    if (bGetSection == true)
                        break;

                    sLineData = sLineData.Replace('[', ' ');
                    sLineData = sLineData.Replace(']', ' ');
                    sLineData = sLineData.Trim();

                    string[] sSplitData_Array = sLineData.Split('_');

                    if (sSplitData_Array.Length == 2)
                    {
                        if (sSplitData_Array[0].Trim() == sMainStep)
                        {
                            if (bGetSendCommandInfo == true)
                            {
                                if (sSplitData_Array[1].Trim().ToUpper() == "ALL")
                                {
                                    bGetSection = true;
                                    bCreateCommandInfo = true;
                                }
                                else if (ElanConvert.IsInt(sSplitData_Array[1].Trim()) == false)
                                {
                                    m_sErrorMessage = string.Format("Record Index Error in Line {0} in User Defined File", nLineIndex + 1);
                                    return false;
                                }
                                else if (Convert.ToInt32(sSplitData_Array[1].Trim()) == nRecordDataIndex + 1)
                                {
                                    bGetSection = true;
                                    bCreateCommandInfo = true;
                                }
                                else
                                    bGetSection = false;
                            }
                            else
                            {
                                if (sSplitData_Array[1].Trim().ToUpper() == "ALL")
                                    bGetSection = true;
                                else if (ElanConvert.IsInt(sSplitData_Array[1].Trim()) == false)
                                {
                                    m_sErrorMessage = string.Format("Record Index Error in Line {0} in User Defined File", nLineIndex + 1);
                                    return false;
                                }
                                else
                                    bGetSection = true;
                            }
                        }
                        else
                            bGetSection = false;
                    }
                    else
                        bGetSection = false;

                    continue;
                }

                if (bGetSection == true)
                {
                    if (bGetSendCommandInfo == true && bCreateCommandInfo == true)
                    {
                        cSendCommandInfo = new SendCommandInfo();
                        bCreateCommandInfo = false;
                    }

                    string[] sSplitData_Array = sLineData.Split('=');

                    if (sSplitData_Array[0].Trim() == "Command")
                    {
                        if (bGetSendCommandInfo == true)
                        {
                            if (cCommandInfo != null)
                            {
                                cSendCommandInfo.cCommandInfo_List.Add(cCommandInfo);
                                cCommandInfo = null;
                            }
                        }

                        if (sSplitData_Array.Length < 2)
                        {
                            m_sErrorMessage = string.Format("Command No Value in Line {0} in User Defined File", nLineIndex + 1);
                            return false;
                        }
                        else
                        {
                            int nByteNumber = 0;
                            string sCommand = sSplitData_Array[1].Trim();
                            string[] sCommandByte_Array = sCommand.Split(' ');

                            for (int nByteIndex = 0; nByteIndex < sCommandByte_Array.Length; nByteIndex++)
                            {
                                string sCommandByte = sCommandByte_Array[nByteIndex];

                                if (sCommandByte.Length != 2)
                                {
                                    m_sErrorMessage = string.Format("Byte Not 2 Charactors in Line {0} in User Defined File", nLineIndex + 1);
                                    return false;
                                }
                                else if (nByteIndex == 0 && sCommandByte != "00" && ParamFingerAutoTuning.m_nEnableCheckUserDefinedFormat == 1)
                                {
                                    m_sErrorMessage = string.Format("Byte[{0}] Not \"00\" in Line {1} in User Defined File", nByteIndex, nLineIndex + 1);
                                    return false;
                                }
                                else if (ElanConvert.CheckIsHexaDecimal(sCommandByte) == false)
                                {
                                    m_sErrorMessage = string.Format("Byte[{0}] Not Hex in Line {1} in User Defined File", nByteIndex, nLineIndex + 1);
                                    return false;
                                }

                                if (nByteIndex == 1)
                                    nByteNumber = Convert.ToInt32(sCommandByte, 16);
                            }

                            if (sCommandByte_Array.Length != nByteNumber + 2 && ParamFingerAutoTuning.m_nEnableCheckUserDefinedFormat == 1)
                            {
                                m_sErrorMessage = string.Format("Command Length Error in Line {0} in User Defined File", nLineIndex + 1);
                                return false;
                            }
                            else
                            {
                                if (bGetSendCommandInfo == true)
                                {
                                    if (ParamFingerAutoTuning.m_nEnableCheckUserDefinedFormat == 1)
                                    {
                                        byte[] byteCommand_Array = new byte[nByteNumber];
                                        Array.Clear(byteCommand_Array, 0, byteCommand_Array.Length);

                                        for (int nByteIndex = 0; nByteIndex < nByteNumber; nByteIndex++)
                                        {
                                            short nValue = Int16.Parse(sCommandByte_Array[nByteIndex + 2], System.Globalization.NumberStyles.HexNumber);
                                            byteCommand_Array[nByteIndex] = (byte)nValue;
                                        }

                                        cCommandInfo = new CommandInfo();
                                        cCommandInfo.byteCommand_Array = byteCommand_Array;
                                    }
                                    else
                                    {
                                        byte[] byteCommand_Array = new byte[sCommandByte_Array.Length - 2];
                                        Array.Clear(byteCommand_Array, 0, byteCommand_Array.Length);

                                        for (int nByteIndex = 2; nByteIndex < sCommandByte_Array.Length; nByteIndex++)
                                        {
                                            short nValue = Int16.Parse(sCommandByte_Array[nByteIndex], System.Globalization.NumberStyles.HexNumber);
                                            byteCommand_Array[nByteIndex - 2] = (byte)nValue;
                                        }

                                        cCommandInfo = new CommandInfo();
                                        cCommandInfo.byteCommand_Array = byteCommand_Array;
                                    }
                                }
                            }
                        }
                    }
                    else if (sSplitData_Array[0].Trim() == "DelayTime")
                    {
                        if (sSplitData_Array.Length < 2)
                        {
                            m_sErrorMessage = string.Format("DelayTime No Value in Line {0} in User Defined File", nLineIndex + 1);
                            return false;
                        }
                        else
                        {
                            string sDelayTime = sSplitData_Array[1].Trim();

                            if (ElanConvert.IsInt(sDelayTime) == false)
                            {
                                m_sErrorMessage = string.Format("Delay Time Not Integer in Line {0} in User Defined File", nLineIndex + 1);
                                return false;
                            }
                            else
                            {
                                if (bGetSendCommandInfo == true)
                                {
                                    if (cCommandInfo != null)
                                    {
                                        cCommandInfo.nDelayTime = Convert.ToInt32(sDelayTime);
                                        cSendCommandInfo.cCommandInfo_List.Add(cCommandInfo);
                                        cCommandInfo = null;
                                    }
                                }
                            }
                        }
                    }
                }

                if (nLineIndex == sScriptData_List.Count - 1)
                {
                    if (cCommandInfo != null)
                    {
                        cSendCommandInfo.cCommandInfo_List.Add(cCommandInfo);
                        cCommandInfo = null;
                    }
                }
            }

            return true;
        }
    }
}