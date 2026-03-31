using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MPPPenAutoTuning;
using MPPPenAutoTuningParameter;

namespace Elan
{
    class ElanCommand_Gen8
    {
        private MainTuningStep m_eMainStep = MainTuningStep.NO;
        private SubTuningStep m_eSubStep = SubTuningStep.NO;
        private int m_nTraceNumber = 0;

        public class SendCommandInfo
        {
            public List<CommandInfo> m_cCommandInfo_List = new List<CommandInfo>();
        }

        public class CommandInfo
        {
            public byte[] m_byteCommand_Array;
            public int m_nDelayTime = 100;
        }

        public enum TraceType
        {
            RX,
            TX
        }

        private enum ClassType
        {
            AFE,
            DataPath,
            Script
        }

        public enum ParameterType
        {
            //FW Analog Parameter
            MPP_SP_NUM,
            MPP_EFFECT_NUM,
            MPP_DFT_NUM,
            PH1_Beacon,
            PH2_Beacon,
            PH3_Beacon,
            TX_PPI_H,
            TX_PPI_L,
            TX_PPI_H_S0,
            TX_PPI_L_S0,
            TX_PPI_H_S1,
            TX_PPI_L_S1,
            TX_PPI_H_S2,
            TX_PPI_L_S2,
            TX_PPI_H_S3,
            TX_PPI_L_S3,
            _PEN_MS_BSH_ADC_TP_NUM,
            _PEN_MS_EFFECT_FW_SET_COEF_NUM,
            _PEN_MS_DFT_NUM_IQ_FIR_CTL,
            _PEN_MS_COS_INC_0,
            _PEN_MS_SIN_INC_0,

            TP_NUM,
            TRxS0_ADC_TP_NUM_SUM,
            TRxS1_ADC_TP_NUM_SUM,
            TRxS2_ADC_TP_NUM_SUM,
            TRxS3_ADC_TP_NUM_SUM,

            //FW Option Parameter
            _Pen_MS_ANA_TP_CTL_00,
            _Pen_MS_ANA_TP_CTL_02,
            _Pen_MS_ANA_TP_CTL_03,
            _Pen_MS_ANA_TP_CTL_04,
            _Pen_MS_ANA_TP_CTL_00_2,
            _Pen_MS_ANA_TP_CTL_02_2,
            _Pen_MS_ANA_TP_CTL_03_2,
            _Pen_MS_ANA_TP_CTL_04_2,
            _Pen_MS_ANA_PH_CTL_00,
            _Pen_MS_M_S_CTL,
            _Pen_MS_CKS_CTL,
            _Pen_MS_CT_ADC_SH_LMT,
        }

        public enum GetReportType
        {
            debug_Rx,
            debug_Tx,
            debug_BHF_noSync,
            debug_PTHF_noSync,
            Stop,
            NA
        }

        public enum MPPModeType
        {
            RX_DFT,
            TX_DFT,
            PTHF_NoSync,
            BHF_NoSync,
            Normal
        }

        public enum MPPSetParameter
        {
            Enable,
            Disable
        }

        public enum AFEType
        {
            NA,
            DTMode,
            CTMode
        }

        public enum FilterType
        {
            NA,
            DisableFilter,
            LPF_0to300KHz,
            LPF_0to75KHz
        }

        public class GetReportInfo
        {
            public TraceType m_eTraceType = TraceType.RX;
            public int m_nTraceNumber = 0;
            public int m_nRowNumber = 0;
            public int m_nTraceOffset = 0;
            public int m_nRowOffset = 0;
            public int m_nReportNumber = 0;
        }

        public class DataType
        {
            public const byte m_byteRead_8009_AFE = 0x67;
            public const byte m_byteWrite_8009_AFE = 0x68;
            public const byte m_byteRead_902_DataPath = 0x69;
            public const byte m_byteWrite_902_DataPath = 0x6A;
        }

        public class ParameterClass
        {
            public const byte m_byteMPP_Init_AFE_Para_Addr = 0x31;
            public const byte m_byteMPP_PD_Freq_Addr = 0x32;
            public const byte m_byteMPP_RX_Freq_Addr = 0x33;
            public const byte m_byteMPP_RXRaw_Freq_Addr = 0x34;
            public const byte m_byteMPP_TX_Freq_Addr = 0x35;
            public const byte m_byteMPP_TXRaw_Freq_Addr = 0x36;
            public const byte m_byteMPP_TRxS_Freq_Addr = 0x37;
            public const byte m_byteMPP_TRxS_Rpt_Freq_Addr = 0x38;
            public const byte m_byteMPP_BHF_Freq_Addr = 0x39;
            public const byte m_byteMPP_PTHF_Freq_Addr = 0x3A;
            public const byte m_byteScript_MPP_PD_Addr = 0x41;
            public const byte m_byteScript_MPP_RX_Addr = 0x42;
            public const byte m_byteScript_MPP_RXRaw_Addr = 0x43;
            public const byte m_byteScript_MPP_TX_Addr = 0x44;
            public const byte m_byteScript_MPP_TXRaw_Addr = 0x45;
            public const byte m_byteScript_MPP_TRxS_Addr = 0x46;
            public const byte m_byteScript_MPP_TRxS_Rpt_Addr = 0x47;
            public const byte m_byteScript_MPP_TRxS_1stRpt_Addr = 0x48;
            public const byte m_byteScript_MPP_AnalogPressure_Addr = 0x49;
            public const byte m_byteScript_MPP_PTHF_Addr = 0x4A;
            public const byte m_byteScript_MPP_PT_HI_HF_Addr = 0x4B;
            public const byte m_byteScript_MPP_BHF_Addr = 0x4C;
            public const byte m_byteScript_MPP_BHF_Rpt_Addr = 0x4D;

            // 8F18 TRxS
            public const byte m_byteScript_MPP_Beacon_TRxS_S0_Set_Combine_Addr = 0x4E;
            public const byte m_byteScript_MPP_Beacon_TRxS_S1_Set_Combine_Addr = 0x4F;
            public const byte m_byteScript_MPP_Beacon_TRxS_S2_Set_Combine_Addr = 0x50;
            public const byte m_byteScript_MPP_Beacon_TRxS_S3_Set_Combine_Addr = 0x51;
            public const byte m_byteScript_MPP_BHF_TRxS_S0_Set_Combine_Addr = 0x52;
            public const byte m_byteScript_MPP_BHF_TRxS_S1_Set_Combine_Addr = 0x53;
            public const byte m_byteScript_MPP_BHF_TRxS_S2_Set_Combine_Addr = 0x54;
            public const byte m_byteScript_MPP_BHF_TRxS_S3_Set_Combine_Addr = 0x55;
            public const byte m_byteScript_MPP_PTHF_TRxS_S0_Set_Combine_Addr = 0x56;
            public const byte m_byteScript_MPP_PTHF_TRxS_S1_Set_Combine_Addr = 0x57;
            public const byte m_byteScript_MPP_PTHF_TRxS_S2_Set_Combine_Addr = 0x58;
            public const byte m_byteScript_MPP_PTHF_TRxS_S3_Set_Combine_Addr = 0x59;

            public const byte m_byteMPP_DataPath_Para = 0x31;
            public const byte m_byteMPP_BD_DataPath_Para = 0x32;
            public const byte m_byteMPP_BRx_DataPath_Para = 0x33;
            public const byte m_byteMPP_BTx_DataPath_Para = 0x34;
            public const byte m_byteMPP_TRxS_DataPath_Para = 0x35;
            public const byte m_byteMPP_PTHF_DataPath_Para = 0x36;
            public const byte m_byteMPP_PT_HI_HF_DataPath_Para = 0x37;
            public const byte m_byteMPP_BHF_DataPath_Para = 0x38;
            public const byte m_byteMPP_Pressure_DataPath_Para = 0x39;  
        }

        private frmMain m_cfrmMain = null;
        private const int m_nIN_DATA_LENGTH = 65;

        private int m_nDeviceIndex = 0;

        private AFEType m_eAFEType = AFEType.NA;
        private FilterType m_eFilterType = FilterType.NA;

        private string m_sErrorMessage = "";

        private MPPModeType m_eMPPModeType = MPPModeType.RX_DFT;

        private class WriteCommandInfo
        {
            public byte m_byteType = 0x00;
            public byte m_byteClass = 0x00;
            public byte m_byteAddress1_H = 0x00;
            public byte m_byteAddress1_L = 0x00;
            public byte m_byteAddress2_H = 0x00;
            public byte m_byteAddress2_L = 0x00;
            public byte m_byteValue1_H = 0x00;
            public byte m_byteValue1_L = 0x00;
            public byte m_byteValue2_H = 0x00;
            public byte m_byteValue2_L = 0x00;
        }

        private class ReadCommandInfo
        {
            public byte m_byteType = 0x00;
            public byte m_byteClass = 0x00;
            public byte m_byteAddress1_H = 0x00;
            public byte m_byteAddress1_L = 0x00;
            public byte m_byteAddress2_H = 0x00;
            public byte m_byteAddress2_L = 0x00;
        }

        private class SampleFrequencyInfo
        {
            public WriteParameter m_cMPP_SP_NUM = new WriteParameter(ParameterType.MPP_SP_NUM);
            public WriteParameter m_cMPP_EFFECT_NUM = new WriteParameter(ParameterType.MPP_EFFECT_NUM);
            public WriteParameter m_cMPP_DFT_NUM = new WriteParameter(ParameterType.MPP_DFT_NUM);
            public WriteParameter m_cPH1_Beacon = new WriteParameter(ParameterType.PH1_Beacon);
            public WriteParameter m_cPH2_Beacon = new WriteParameter(ParameterType.PH2_Beacon);
            public WriteParameter m_cPH3_Beacon = new WriteParameter(ParameterType.PH3_Beacon);
            public WriteParameter m_c_PEN_MS_BSH_ADC_TP_NUM = new WriteParameter(ParameterType._PEN_MS_BSH_ADC_TP_NUM);
            public WriteParameter m_c_PEN_MS_EFFECT_FW_SET_COEF_NUM = new WriteParameter(ParameterType._PEN_MS_EFFECT_FW_SET_COEF_NUM);
            public WriteParameter m_c_PEN_MS_DFT_NUM_IQ_FIR_CTL = new WriteParameter(ParameterType._PEN_MS_DFT_NUM_IQ_FIR_CTL);
            public WriteParameter m_c_PEN_MS_COS_INC_0 = new WriteParameter(ParameterType._PEN_MS_COS_INC_0);
            public WriteParameter m_c_PEN_MS_SIN_INC_0 = new WriteParameter(ParameterType._PEN_MS_SIN_INC_0);

            public WriteParameter m_cTX_PPI_H = null;
            public WriteParameter m_cTX_PPI_L = null;

            public WriteParameter m_cTX_PPI_H_S0 = null;
            public WriteParameter m_cTX_PPI_L_S0 = null;
            public WriteParameter m_cTX_PPI_H_S1 = null;
            public WriteParameter m_cTX_PPI_L_S1 = null;
            public WriteParameter m_cTX_PPI_H_S2 = null;
            public WriteParameter m_cTX_PPI_L_S2 = null;
            public WriteParameter m_cTX_PPI_H_S3 = null;
            public WriteParameter m_cTX_PPI_L_S3 = null;

            public void DeclareTX_PPI(MainTuningStep eMainStep)
            {
                if (eMainStep == MainTuningStep.NO)
                {
                    m_cTX_PPI_H = new WriteParameter(ParameterType.TX_PPI_H);
                    m_cTX_PPI_L = new WriteParameter(ParameterType.TX_PPI_L);
                }
                else if (eMainStep == MainTuningStep.TILTNO)
                {
                    m_cTX_PPI_H_S0 = new WriteParameter(ParameterType.TX_PPI_H_S0, 0);
                    m_cTX_PPI_L_S0 = new WriteParameter(ParameterType.TX_PPI_L_S0, 0);
                    m_cTX_PPI_H_S1 = new WriteParameter(ParameterType.TX_PPI_H_S1, 1);
                    m_cTX_PPI_L_S1 = new WriteParameter(ParameterType.TX_PPI_L_S1, 1);
                    m_cTX_PPI_H_S2 = new WriteParameter(ParameterType.TX_PPI_H_S2, 2);
                    m_cTX_PPI_L_S2 = new WriteParameter(ParameterType.TX_PPI_L_S2, 2);
                    m_cTX_PPI_H_S3 = new WriteParameter(ParameterType.TX_PPI_H_S3, 3);
                    m_cTX_PPI_L_S3 = new WriteParameter(ParameterType.TX_PPI_L_S3, 3);
                }
            }
        }

        private SampleFrequencyInfo m_cSampleFrequencyInfo = null;

        private class AFETypeInfo
        {
            public WriteParameter m_cANA_TP_CTL_00 = new WriteParameter(ParameterType._Pen_MS_ANA_TP_CTL_00);
            public WriteParameter m_cANA_TP_CTL_04 = new WriteParameter(ParameterType._Pen_MS_ANA_TP_CTL_04);
            public WriteParameter m_cANA_PH_CTL_00 = new WriteParameter(ParameterType._Pen_MS_ANA_PH_CTL_00);
            public WriteParameter m_cMS_M_S_CTL = new WriteParameter(ParameterType._Pen_MS_M_S_CTL);
            public WriteParameter m_cMS_CKS_CTL = new WriteParameter(ParameterType._Pen_MS_CKS_CTL);
            public WriteParameter m_cMS_CT_ADC_SH_LMT = new WriteParameter(ParameterType._Pen_MS_CT_ADC_SH_LMT);

            public WriteParameter m_cANA_TP_CTL_00_2 = new WriteParameter(ParameterType._Pen_MS_ANA_TP_CTL_00_2);
            public WriteParameter m_cANA_TP_CTL_04_2 = new WriteParameter(ParameterType._Pen_MS_ANA_TP_CTL_04_2);
        }

        private AFETypeInfo m_cAFETypeInfo = new AFETypeInfo();

        private class FilterTypeInfo
        {
            public WriteParameter m_cANA_TP_CTL_02 = new WriteParameter(ParameterType._Pen_MS_ANA_TP_CTL_02);
            public WriteParameter m_cANA_TP_CTL_03 = new WriteParameter(ParameterType._Pen_MS_ANA_TP_CTL_03);
            public WriteParameter m_cANA_TP_CTL_04 = new WriteParameter(ParameterType._Pen_MS_ANA_TP_CTL_04);

            public WriteParameter m_cANA_TP_CTL_02_2 = new WriteParameter(ParameterType._Pen_MS_ANA_TP_CTL_02_2);
            public WriteParameter m_cANA_TP_CTL_03_2 = new WriteParameter(ParameterType._Pen_MS_ANA_TP_CTL_03_2);
            public WriteParameter m_cANA_TP_CTL_04_2 = new WriteParameter(ParameterType._Pen_MS_ANA_TP_CTL_04_2);
        }

        private FilterTypeInfo m_cFilterTypeInfo = new FilterTypeInfo();

        private class WriteParameter
        {
            public ParameterType m_eParameterType;
            public int m_nValue_1 = 0;
            public int m_nValue_2 = 0;
            public int m_nSection = -1;

            public WriteParameter(ParameterType eParameterType)
            {
                m_eParameterType = eParameterType;
            }

            public WriteParameter(ParameterType eParameterType, int nSection)
            {
                m_eParameterType = eParameterType;
                m_nSection = nSection;
            }
        }

        private class FWValue
        {
            public bool m_bGetValueFlag = false;
            public int m_nSP_NUM = 0;
            public int m_nEFFECT_NUM = 0;
            public int m_nDFT_NUM = 0;
            public int m_nBSH_ADC_TP_NUM_Other_L = 0;
            public int m_nBSH_ADC_TP_NUM_Other_H = 0;
            public int m_nEFFECT_FW_SET_COEF_NUM_Other = 0;
            public int m_nDFT_NUM_IQ_FIR_CTL_Other = 0;

            public int m_nRX_OR_TX_NUM = 0;

            public int m_nTRxS0_ADC_TP_NUM_SUM = 0;
            public int m_nTRxS1_ADC_TP_NUM_SUM = 0;
            public int m_nTRxS2_ADC_TP_NUM_SUM = 0;
            public int m_nTRxS3_ADC_TP_NUM_SUM = 0;
        }

        private FWValue m_cOriginFWValue = new FWValue();

        public bool ReadGetOriginFWValueFlag()
        {
            return m_cOriginFWValue.m_bGetValueFlag;
        }

        public ElanCommand_Gen8(frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
            m_nDeviceIndex = m_cfrmMain.m_nDeviceIndex;
        }

        public void SetMPPModeType(MPPModeType eMPPModeType)
        {
            m_eMPPModeType = eMPPModeType;
        }

        private WriteCommandInfo SetWriteCommandInfo(ParameterType eParameterType, int nValue1 = 0, int nValue2 = 0, int nSection = -1)
        {
            WriteCommandInfo cWriteCommandInfo = new WriteCommandInfo();

            byte byteHighByte = 0x00;
            byte byteLowByte = 0x00;

            switch (eParameterType)
            {
                case ParameterType.MPP_SP_NUM:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.AFE);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_MPP_SP_NUM & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_MPP_SP_NUM & 0x00FF);
                    break;
                case ParameterType.MPP_EFFECT_NUM:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.AFE);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_MPP_EFFECT_NUM & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_MPP_EFFECT_NUM & 0x00FF);
                    break;
                case ParameterType.MPP_DFT_NUM:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.AFE);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_MPP_DFT_NUM & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_MPP_DFT_NUM & 0x00FF);
                    break;
                case ParameterType.PH1_Beacon:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.AFE);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PH1_Beacon & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PH1_Beacon & 0x00FF);
                    break;
                case ParameterType.PH2_Beacon:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.AFE);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PH2_Beacon & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PH2_Beacon & 0x00FF);
                    break;
                case ParameterType.PH3_Beacon:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.AFE);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PH3_Beacon & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PH3_Beacon & 0x00FF);
                    break;
                case ParameterType.TX_PPI_H:
                case ParameterType.TX_PPI_H_S0:
                case ParameterType.TX_PPI_H_S1:
                case ParameterType.TX_PPI_H_S2:
                case ParameterType.TX_PPI_H_S3:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.Script, nSection);

                    SetTX_PPI_HAddress(cWriteCommandInfo, nSection);
                    break;
                case ParameterType.TX_PPI_L:
                case ParameterType.TX_PPI_L_S0:
                case ParameterType.TX_PPI_L_S1:
                case ParameterType.TX_PPI_L_S2:
                case ParameterType.TX_PPI_L_S3:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.Script, nSection);

                    SetTX_PPI_LAddress(cWriteCommandInfo, nSection);
                    break;
                case ParameterType._PEN_MS_BSH_ADC_TP_NUM:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_902_DataPath;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.DataPath);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PEN_MS_BSH_ADC_TP_NUM & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PEN_MS_BSH_ADC_TP_NUM & 0x00FF);
                    break;
                case ParameterType._PEN_MS_EFFECT_FW_SET_COEF_NUM:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_902_DataPath;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.DataPath);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PEN_MS_EFFECT_FW_SET_COEF_NUM & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PEN_MS_EFFECT_FW_SET_COEF_NUM & 0x00FF);
                    break;
                case ParameterType._PEN_MS_DFT_NUM_IQ_FIR_CTL:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_902_DataPath;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.DataPath);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PEN_MS_DFT_NUM_IQ_FIR_CTL & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PEN_MS_DFT_NUM_IQ_FIR_CTL & 0x00FF);
                    break;
                case ParameterType._PEN_MS_COS_INC_0:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_902_DataPath;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.DataPath);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PEN_MS_COS_INC_0 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PEN_MS_COS_INC_0 & 0x00FF);
                    break;
                case ParameterType._PEN_MS_SIN_INC_0:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_902_DataPath;

                    SetAFEAndDataPathClass(cWriteCommandInfo, ClassType.DataPath);

                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PEN_MS_SIN_INC_0 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PEN_MS_SIN_INC_0 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_00:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_02:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_03:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_04:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_00_2:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00_2 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00_2 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_02_2:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02_2 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02_2 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_03_2:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03_2 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03_2 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_04_2:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04_2 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04_2 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_PH_CTL_00:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_PH_CTL_00 & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_PH_CTL_00 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_M_S_CTL:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_M_S_CTL & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_M_S_CTL & 0x00FF);
                    break;
                case ParameterType._Pen_MS_CKS_CTL:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_CKS_CTL & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_CKS_CTL & 0x00FF);
                    break;
                case ParameterType._Pen_MS_CT_ADC_SH_LMT:
                    cWriteCommandInfo.m_byteType = DataType.m_byteWrite_8009_AFE;
                    cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_CT_ADC_SH_LMT & 0xFF00) >> 8);
                    cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_CT_ADC_SH_LMT & 0x00FF);
                    break;
                default:
                    break;
            }

            byteHighByte = (byte)((nValue1 & 0xFF00) >> 8);
            byteLowByte = (byte)(nValue1 & 0xFF);

            cWriteCommandInfo.m_byteValue1_H = byteHighByte;
            cWriteCommandInfo.m_byteValue1_L = byteLowByte;

            byteHighByte = (byte)((nValue2 & 0xFF00) >> 8);
            byteLowByte = (byte)(nValue2 & 0xFF);

            cWriteCommandInfo.m_byteValue2_H = byteHighByte;
            cWriteCommandInfo.m_byteValue2_L = byteLowByte;

            return cWriteCommandInfo;
        }

        private void SetAFEAndDataPathClass(WriteCommandInfo cWriteCommandInfo, ClassType eClassType, int nSection = -1)
        {
            if (eClassType == ClassType.AFE)
            {
                switch (m_eMPPModeType)
                {
                    case MPPModeType.RX_DFT:
                        cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_RX_Freq_Addr;
                        break;
                    case MPPModeType.TX_DFT:
                        cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_TX_Freq_Addr;
                        break;
                    case MPPModeType.PTHF_NoSync:
                        cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_PTHF_Freq_Addr;
                        break;
                    case MPPModeType.BHF_NoSync:
                        cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_BHF_Freq_Addr;
                        break;
                    default:
                        break;
                }
            }
            else if (eClassType == ClassType.DataPath)
            {
                switch (m_eMPPModeType)
                {
                    case MPPModeType.RX_DFT:
                        cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_BRx_DataPath_Para;
                        break;
                    case MPPModeType.TX_DFT:
                        cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_BTx_DataPath_Para;
                        break;
                    case MPPModeType.PTHF_NoSync:
                        cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_PTHF_DataPath_Para;
                        break;
                    case MPPModeType.BHF_NoSync:
                        cWriteCommandInfo.m_byteClass = ParameterClass.m_byteMPP_BHF_DataPath_Para;
                        break;
                    default:
                        break;
                }
            }
            else if (eClassType == ClassType.Script)
            {
                switch (m_eMPPModeType)
                {
                    case MPPModeType.RX_DFT:
                        cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_RX_Addr;
                        break;
                    case MPPModeType.TX_DFT:
                        cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_TX_Addr;
                        break;
                    case MPPModeType.PTHF_NoSync:
                        if (nSection == 0)
                            cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_PTHF_TRxS_S0_Set_Combine_Addr;
                        else if (nSection == 1)
                            cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_PTHF_TRxS_S1_Set_Combine_Addr;
                        else if (nSection == 2)
                            cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_PTHF_TRxS_S2_Set_Combine_Addr;
                        else if (nSection == 3)
                            cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_PTHF_TRxS_S3_Set_Combine_Addr;
                        else
                            cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_PTHF_Addr;

                        break;
                    case MPPModeType.BHF_NoSync:
                        if (nSection == 0)
                            cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_BHF_TRxS_S0_Set_Combine_Addr;
                        else if (nSection == 1)
                            cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_BHF_TRxS_S1_Set_Combine_Addr;
                        else if (nSection == 2)
                            cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_BHF_TRxS_S2_Set_Combine_Addr;
                        else if (nSection == 3)
                            cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_BHF_TRxS_S3_Set_Combine_Addr;
                        else
                            cWriteCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_BHF_Addr;

                        break;
                    default:
                        break;
                }
            }
        }

        private void SetTX_PPI_HAddress(WriteCommandInfo cWriteCommandInfo, int nSection)
        {
            if (nSection == 0)
            {
                cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S0 & 0xFF00) >> 8);
                cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S0 & 0x00FF);
            }
            else if (nSection == 1)
            {
                cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S1 & 0xFF00) >> 8);
                cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S1 & 0x00FF);
            }
            else if (nSection == 2)
            {
                cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S2 & 0xFF00) >> 8);
                cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S2 & 0x00FF);
            }
            else if (nSection == 3)
            {
                cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S3 & 0xFF00) >> 8);
                cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S3 & 0x00FF);
            }
            else
            {
                cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_H & 0xFF00) >> 8);
                cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_H & 0x00FF);
            }
        }

        private void SetTX_PPI_LAddress(WriteCommandInfo cWriteCommandInfo, int nSection)
        {
            if (nSection == 0)
            {
                cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S0 & 0xFF00) >> 8);
                cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S0 & 0x00FF);
            }
            else if (nSection == 1)
            {
                cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S1 & 0xFF00) >> 8);
                cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S1 & 0x00FF);
            }
            else if (nSection == 2)
            {
                cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S2 & 0xFF00) >> 8);
                cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S2 & 0x00FF);
            }
            else if (nSection == 3)
            {
                cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S3 & 0xFF00) >> 8);
                cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S3 & 0x00FF);
            }
            else
            {
                cWriteCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_L & 0xFF00) >> 8);
                cWriteCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_L & 0x00FF);
            }
        }

        private ReadCommandInfo SetReadCommandInfo(ParameterType eParameterType, int nSection = -1)
        {
            ReadCommandInfo cReadCommandInfo = new ReadCommandInfo();

            switch (eParameterType)
            {
                case ParameterType.MPP_SP_NUM:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.AFE);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_MPP_SP_NUM & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_MPP_SP_NUM & 0x00FF);
                    break;
                case ParameterType.MPP_EFFECT_NUM:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.AFE);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_MPP_EFFECT_NUM & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_MPP_EFFECT_NUM & 0x00FF);
                    break;
                case ParameterType.MPP_DFT_NUM:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.AFE);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_MPP_DFT_NUM & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_MPP_DFT_NUM & 0x00FF);
                    break;
                case ParameterType.PH1_Beacon:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.AFE);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PH1_Beacon & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PH1_Beacon & 0x00FF);
                    break;
                case ParameterType.PH2_Beacon:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.AFE);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PH2_Beacon & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PH2_Beacon & 0x00FF);
                    break;
                case ParameterType.PH3_Beacon:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.AFE);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PH3_Beacon & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PH3_Beacon & 0x00FF);
                    break;
                case ParameterType.TX_PPI_H:
                case ParameterType.TX_PPI_H_S0:
                case ParameterType.TX_PPI_H_S1:
                case ParameterType.TX_PPI_H_S2:
                case ParameterType.TX_PPI_H_S3:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.Script, nSection);

                    SetTX_PPI_HAddress(cReadCommandInfo, nSection);
                    break;
                case ParameterType.TX_PPI_L:
                case ParameterType.TX_PPI_L_S0:
                case ParameterType.TX_PPI_L_S1:
                case ParameterType.TX_PPI_L_S2:
                case ParameterType.TX_PPI_L_S3:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.Script, nSection);

                    SetTX_PPI_LAddress(cReadCommandInfo, nSection);
                    break;
                case ParameterType._PEN_MS_BSH_ADC_TP_NUM:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_902_DataPath;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.DataPath);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PEN_MS_BSH_ADC_TP_NUM & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PEN_MS_BSH_ADC_TP_NUM & 0x00FF);
                    break;
                case ParameterType._PEN_MS_EFFECT_FW_SET_COEF_NUM:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_902_DataPath;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.DataPath);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PEN_MS_EFFECT_FW_SET_COEF_NUM & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PEN_MS_EFFECT_FW_SET_COEF_NUM & 0x00FF);
                    break;
                case ParameterType._PEN_MS_DFT_NUM_IQ_FIR_CTL:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_902_DataPath;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.DataPath);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PEN_MS_DFT_NUM_IQ_FIR_CTL & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PEN_MS_DFT_NUM_IQ_FIR_CTL & 0x00FF);
                    break;
                case ParameterType._PEN_MS_COS_INC_0:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_902_DataPath;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.DataPath);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PEN_MS_COS_INC_0 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PEN_MS_COS_INC_0 & 0x00FF);
                    break;
                case ParameterType._PEN_MS_SIN_INC_0:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_902_DataPath;

                    SetAFEAndDataPathClass(cReadCommandInfo, ClassType.DataPath);

                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_PEN_MS_SIN_INC_0 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_PEN_MS_SIN_INC_0 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_00:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_02:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_03:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_04:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_00_2:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00_2 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00_2 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_02_2:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02_2 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02_2 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_03_2:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03_2 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03_2 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_TP_CTL_04_2:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04_2 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04_2 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_ANA_PH_CTL_00:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_PH_CTL_00 & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_ANA_PH_CTL_00 & 0x00FF);
                    break;
                case ParameterType._Pen_MS_M_S_CTL:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_M_S_CTL & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_M_S_CTL & 0x00FF);
                    break;
                case ParameterType._Pen_MS_CKS_CTL:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_CKS_CTL & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_CKS_CTL & 0x00FF);
                    break;
                case ParameterType._Pen_MS_CT_ADC_SH_LMT:
                    cReadCommandInfo.m_byteType = DataType.m_byteRead_8009_AFE;
                    cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_Init_AFE_Para_Addr;
                    cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_Pen_MS_CT_ADC_SH_LMT & 0xFF00) >> 8);
                    cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_Pen_MS_CT_ADC_SH_LMT & 0x00FF);
                    break;
                default:
                    break;
            }

            return cReadCommandInfo;
        }

        private void SetAFEAndDataPathClass(ReadCommandInfo cReadCommandInfo, ClassType eClassType, int nSection = -1)
        {
            if (eClassType == ClassType.AFE)
            {
                switch (m_eMPPModeType)
                {
                    case MPPModeType.RX_DFT:
                        cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_RX_Freq_Addr;
                        break;
                    case MPPModeType.TX_DFT:
                        cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_TX_Freq_Addr;
                        break;
                    case MPPModeType.PTHF_NoSync:
                        cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_PTHF_Freq_Addr;
                        break;
                    case MPPModeType.BHF_NoSync:
                        cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_BHF_Freq_Addr;
                        break;
                    default:
                        break;
                }
            }
            else if (eClassType == ClassType.DataPath)
            {
                switch (m_eMPPModeType)
                {
                    case MPPModeType.RX_DFT:
                        cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_BRx_DataPath_Para;
                        break;
                    case MPPModeType.TX_DFT:
                        cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_BTx_DataPath_Para;
                        break;
                    case MPPModeType.PTHF_NoSync:
                        cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_PTHF_DataPath_Para;
                        break;
                    case MPPModeType.BHF_NoSync:
                        cReadCommandInfo.m_byteClass = ParameterClass.m_byteMPP_BHF_DataPath_Para;
                        break;
                    default:
                        break;
                }
            }
            else if (eClassType == ClassType.Script)
            {
                switch (m_eMPPModeType)
                {
                    case MPPModeType.RX_DFT:
                        cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_RX_Addr;
                        break;
                    case MPPModeType.TX_DFT:
                        cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_TX_Addr;
                        break;
                    case MPPModeType.PTHF_NoSync:
                        if (nSection == 0)
                            cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_PTHF_TRxS_S0_Set_Combine_Addr;
                        else if (nSection == 1)
                            cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_PTHF_TRxS_S1_Set_Combine_Addr;
                        else if (nSection == 2)
                            cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_PTHF_TRxS_S2_Set_Combine_Addr;
                        else if (nSection == 3)
                            cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_PTHF_TRxS_S3_Set_Combine_Addr;
                        else
                            cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_PTHF_Addr;

                        break;
                    case MPPModeType.BHF_NoSync:
                        if (nSection == 0)
                            cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_BHF_TRxS_S0_Set_Combine_Addr;
                        else if (nSection == 1)
                            cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_BHF_TRxS_S1_Set_Combine_Addr;
                        else if (nSection == 2)
                            cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_BHF_TRxS_S2_Set_Combine_Addr;
                        else if (nSection == 3)
                            cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_BHF_TRxS_S3_Set_Combine_Addr;
                        else
                            cReadCommandInfo.m_byteClass = ParameterClass.m_byteScript_MPP_BHF_Addr;

                        break;
                    default:
                        break;
                }
            }
        }

        private void SetTX_PPI_HAddress(ReadCommandInfo cReadCommandInfo, int nSection)
        {
            if (nSection == 0)
            {
                cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S0 & 0xFF00) >> 8);
                cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S0 & 0x00FF);
            }
            else if (nSection == 1)
            {
                cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S1 & 0xFF00) >> 8);
                cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S1 & 0x00FF);
            }
            else if (nSection == 2)
            {
                cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S2 & 0xFF00) >> 8);
                cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S2 & 0x00FF);
            }
            else if (nSection == 3)
            {
                cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S3 & 0xFF00) >> 8);
                cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_H_S3 & 0x00FF);
            }
            else
            {
                cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_H & 0xFF00) >> 8);
                cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_H & 0x00FF);
            }
        }

        private void SetTX_PPI_LAddress(ReadCommandInfo cReadCommandInfo, int nSection)
        {
            if (nSection == 0)
            {
                cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S0 & 0xFF00) >> 8);
                cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S0 & 0x00FF);
            }
            else if (nSection == 1)
            {
                cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S1 & 0xFF00) >> 8);
                cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S1 & 0x00FF);
            }
            else if (nSection == 2)
            {
                cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S2 & 0xFF00) >> 8);
                cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S2 & 0x00FF);
            }
            else if (nSection == 3)
            {
                cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S3 & 0xFF00) >> 8);
                cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_L_S3 & 0x00FF);
            }
            else
            {
                cReadCommandInfo.m_byteAddress2_H = (byte)((ParamAutoTuning.m_nRelativeAddress_TX_PPI_L & 0xFF00) >> 8);
                cReadCommandInfo.m_byteAddress2_L = (byte)(ParamAutoTuning.m_nRelativeAddress_TX_PPI_L & 0x00FF);
            }
        }

        public void SetFlowStep(MainTuningStep eMainStep, SubTuningStep eSubStep)
        {
            m_eMainStep = eMainStep;
            m_eSubStep = eSubStep;

            m_cSampleFrequencyInfo = new SampleFrequencyInfo();
            m_cSampleFrequencyInfo.DeclareTX_PPI(m_eMainStep);
        }

        public void SetTraceNumber(int nTraceNumber)
        {
            m_nTraceNumber = nTraceNumber;
        }

        public void RunSetSampleFrequencyFlow(string sPenScanState, int nPH1, int nPH2, int nRetryIncreaseTime)
        {
            ComputeDFT_NUMAndCoefficient cComputeDFT_NUMAndCoeff = new ComputeDFT_NUMAndCoefficient();
            cComputeDFT_NUMAndCoeff.LoadParameter(sPenScanState, nPH1, nPH2, ParamAutoTuning.m_nGen8SKIP_NUM);
            cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            Int16 nDFT_NUM = cComputeDFT_NUMAndCoeff.GetDFT_NUM();
            Int16 nCOS_INC_0_H = cComputeDFT_NUMAndCoeff.GetCoefficient(ComputeDFT_NUMAndCoefficient.CoefficientType.COS_INC_0_H);
            Int16 nCOS_INC_0_L = cComputeDFT_NUMAndCoeff.GetCoefficient(ComputeDFT_NUMAndCoefficient.CoefficientType.COS_INC_0_L);
            Int16 nSIN_INC_0_H = cComputeDFT_NUMAndCoeff.GetCoefficient(ComputeDFT_NUMAndCoefficient.CoefficientType.SIN_INC_0_H);
            Int16 nSIN_INC_0_L = cComputeDFT_NUMAndCoeff.GetCoefficient(ComputeDFT_NUMAndCoefficient.CoefficientType.SIN_INC_0_L);

            int nSP_NUM = nDFT_NUM + (m_cOriginFWValue.m_nSP_NUM - m_cOriginFWValue.m_nDFT_NUM);
            int nEFFECT_NUM = nDFT_NUM + (m_cOriginFWValue.m_nEFFECT_NUM - m_cOriginFWValue.m_nDFT_NUM);

            if (m_eMainStep == MainTuningStep.NO)
            {
                int nRX_OR_TX_NUM = m_nTraceNumber;
                //int nRX_OR_TX_NUM = m_cOriginFWValue.m_nRX_OR_TX_NUM;

                int nTX_PPI_H = (int)(((nRX_OR_TX_NUM * nEFFECT_NUM) & 0xFFFF0000) >> 16);
                int nTX_PPI_L = (nRX_OR_TX_NUM * nEFFECT_NUM) & 0x0000FFFF;

                m_cSampleFrequencyInfo.m_cTX_PPI_H.m_nValue_2 = nTX_PPI_H;
                m_cSampleFrequencyInfo.m_cTX_PPI_L.m_nValue_2 = nTX_PPI_L;
            }
            else if (m_eMainStep == MainTuningStep.TILTNO)
            {
                int nTX_PPI_H_S0 = (int)(((m_cOriginFWValue.m_nTRxS0_ADC_TP_NUM_SUM * nEFFECT_NUM) & 0xFFFF0000) >> 16);
                int nTX_PPI_L_S0 = (m_cOriginFWValue.m_nTRxS0_ADC_TP_NUM_SUM * nEFFECT_NUM) & 0x0000FFFF;
                int nTX_PPI_H_S1 = (int)(((m_cOriginFWValue.m_nTRxS1_ADC_TP_NUM_SUM * nEFFECT_NUM) & 0xFFFF0000) >> 16);
                int nTX_PPI_L_S1 = (m_cOriginFWValue.m_nTRxS1_ADC_TP_NUM_SUM * nEFFECT_NUM) & 0x0000FFFF;
                int nTX_PPI_H_S2 = (int)(((m_cOriginFWValue.m_nTRxS2_ADC_TP_NUM_SUM * nEFFECT_NUM) & 0xFFFF0000) >> 16);
                int nTX_PPI_L_S2 = (m_cOriginFWValue.m_nTRxS2_ADC_TP_NUM_SUM * nEFFECT_NUM) & 0x0000FFFF;
                int nTX_PPI_H_S3 = (int)(((m_cOriginFWValue.m_nTRxS3_ADC_TP_NUM_SUM * nEFFECT_NUM) & 0xFFFF0000) >> 16);
                int nTX_PPI_L_S3 = (m_cOriginFWValue.m_nTRxS3_ADC_TP_NUM_SUM * nEFFECT_NUM) & 0x0000FFFF;

                m_cSampleFrequencyInfo.m_cTX_PPI_H_S0.m_nValue_2 = nTX_PPI_H_S0;
                m_cSampleFrequencyInfo.m_cTX_PPI_L_S0.m_nValue_2 = nTX_PPI_L_S0;
                m_cSampleFrequencyInfo.m_cTX_PPI_H_S1.m_nValue_2 = nTX_PPI_H_S1;
                m_cSampleFrequencyInfo.m_cTX_PPI_L_S1.m_nValue_2 = nTX_PPI_L_S1;
                m_cSampleFrequencyInfo.m_cTX_PPI_H_S2.m_nValue_2 = nTX_PPI_H_S2;
                m_cSampleFrequencyInfo.m_cTX_PPI_L_S2.m_nValue_2 = nTX_PPI_L_S2;
                m_cSampleFrequencyInfo.m_cTX_PPI_H_S3.m_nValue_2 = nTX_PPI_H_S3;
                m_cSampleFrequencyInfo.m_cTX_PPI_L_S3.m_nValue_2 = nTX_PPI_L_S3;
            }

            int nBSH_ADC_TP_NUM_Value_1 = (nDFT_NUM << 4) | m_cOriginFWValue.m_nBSH_ADC_TP_NUM_Other_H;

            m_cSampleFrequencyInfo.m_cMPP_SP_NUM.m_nValue_2 = nSP_NUM;
            m_cSampleFrequencyInfo.m_cMPP_EFFECT_NUM.m_nValue_2 = nEFFECT_NUM;
            m_cSampleFrequencyInfo.m_cMPP_DFT_NUM.m_nValue_2 = nDFT_NUM;
            m_cSampleFrequencyInfo.m_cPH1_Beacon.m_nValue_2 = nPH1;
            m_cSampleFrequencyInfo.m_cPH2_Beacon.m_nValue_2 = nPH2;
            m_cSampleFrequencyInfo.m_cPH3_Beacon.m_nValue_2 = nPH2;
            
            m_cSampleFrequencyInfo.m_c_PEN_MS_BSH_ADC_TP_NUM.m_nValue_1 = nBSH_ADC_TP_NUM_Value_1;
            m_cSampleFrequencyInfo.m_c_PEN_MS_BSH_ADC_TP_NUM.m_nValue_2 = m_cOriginFWValue.m_nBSH_ADC_TP_NUM_Other_L;
            m_cSampleFrequencyInfo.m_c_PEN_MS_EFFECT_FW_SET_COEF_NUM.m_nValue_1 = nDFT_NUM;
            m_cSampleFrequencyInfo.m_c_PEN_MS_EFFECT_FW_SET_COEF_NUM.m_nValue_2 = m_cOriginFWValue.m_nEFFECT_FW_SET_COEF_NUM_Other;
            m_cSampleFrequencyInfo.m_c_PEN_MS_DFT_NUM_IQ_FIR_CTL.m_nValue_1 = nDFT_NUM;
            m_cSampleFrequencyInfo.m_c_PEN_MS_DFT_NUM_IQ_FIR_CTL.m_nValue_2 = m_cOriginFWValue.m_nDFT_NUM_IQ_FIR_CTL_Other;
            m_cSampleFrequencyInfo.m_c_PEN_MS_COS_INC_0.m_nValue_1 = nCOS_INC_0_H;
            m_cSampleFrequencyInfo.m_c_PEN_MS_COS_INC_0.m_nValue_2 = nCOS_INC_0_L;
            m_cSampleFrequencyInfo.m_c_PEN_MS_SIN_INC_0.m_nValue_1 = nSIN_INC_0_H;
            m_cSampleFrequencyInfo.m_c_PEN_MS_SIN_INC_0.m_nValue_2 = nSIN_INC_0_L;

            WriteParameter[] cWriteParameter_Array = null;

            if (m_eMainStep == MainTuningStep.NO)
            {
                cWriteParameter_Array = new WriteParameter[]
                {
                    m_cSampleFrequencyInfo.m_cMPP_SP_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_EFFECT_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_DFT_NUM,
                    m_cSampleFrequencyInfo.m_cPH1_Beacon,
                    m_cSampleFrequencyInfo.m_cPH2_Beacon,
                    m_cSampleFrequencyInfo.m_cPH3_Beacon,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_BSH_ADC_TP_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_EFFECT_FW_SET_COEF_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_DFT_NUM_IQ_FIR_CTL,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_COS_INC_0,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_SIN_INC_0
                };
            }
            else if (m_eMainStep == MainTuningStep.TILTNO)
            {
                cWriteParameter_Array = new WriteParameter[]
                {
                    m_cSampleFrequencyInfo.m_cMPP_SP_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_EFFECT_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_DFT_NUM,
                    m_cSampleFrequencyInfo.m_cPH1_Beacon,
                    m_cSampleFrequencyInfo.m_cPH2_Beacon,
                    m_cSampleFrequencyInfo.m_cPH3_Beacon,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S0,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S0,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S1,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S1,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S2,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S2,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S3,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S3,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_BSH_ADC_TP_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_EFFECT_FW_SET_COEF_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_DFT_NUM_IQ_FIR_CTL,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_COS_INC_0,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_SIN_INC_0
                };
            }

            foreach(WriteParameter cWriteParameter in cWriteParameter_Array)
            {
                WriteCommandInfo cWriteCommandInfo = SetWriteCommandInfo(cWriteParameter.m_eParameterType, cWriteParameter.m_nValue_1, cWriteParameter.m_nValue_2, cWriteParameter.m_nSection);
                SendWriteCommand(cWriteCommandInfo, cWriteParameter);

                Thread.Sleep(nRetryIncreaseTime);
            }
        }

        public void RunSetFWOptionFlow()
        {
            switch (ParamAutoTuning.m_nGen8AFEType)
            {
                case 0:
                    m_eAFEType = AFEType.NA;
                    break;
                case 1:
                    m_eAFEType = AFEType.DTMode;
                    break;
                case 2:
                    m_eAFEType = AFEType.CTMode;
                    break;
                default:
                    break;
            }

            if (m_eAFEType != AFEType.NA)
            {
                switch(m_eAFEType)
                {
                    case AFEType.DTMode:
                        m_cAFETypeInfo.m_cANA_TP_CTL_00.m_nValue_2 = 0x4000;
                        m_cAFETypeInfo.m_cANA_TP_CTL_04.m_nValue_2 = 0x0000;

                        if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                            m_cAFETypeInfo.m_cANA_PH_CTL_00.m_nValue_2 = 0x0196;
                        else
                            m_cAFETypeInfo.m_cANA_PH_CTL_00.m_nValue_2 = 0x1192;

                        m_cAFETypeInfo.m_cMS_M_S_CTL.m_nValue_2 = 0x0000;
                        m_cAFETypeInfo.m_cMS_CKS_CTL.m_nValue_2 = 0x0180;
                        m_cAFETypeInfo.m_cMS_CT_ADC_SH_LMT.m_nValue_2 = 0x0000;

                        if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                        {
                            m_cAFETypeInfo.m_cANA_TP_CTL_00_2.m_nValue_2 = m_cAFETypeInfo.m_cANA_TP_CTL_00.m_nValue_2;
                            m_cAFETypeInfo.m_cANA_TP_CTL_04_2.m_nValue_2 = m_cAFETypeInfo.m_cANA_TP_CTL_04.m_nValue_2;
                        }

                        break;
                    case AFEType.CTMode:
                        if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                            m_cAFETypeInfo.m_cANA_TP_CTL_00.m_nValue_2 = 0x40C0;
                        else
                            m_cAFETypeInfo.m_cANA_TP_CTL_00.m_nValue_2 = 0x4080;

                        m_cAFETypeInfo.m_cANA_TP_CTL_04.m_nValue_2 = 0x0200;
                        m_cAFETypeInfo.m_cANA_PH_CTL_00.m_nValue_2 = 0x0000;
                        m_cAFETypeInfo.m_cMS_M_S_CTL.m_nValue_2 = 0x0004;
                        m_cAFETypeInfo.m_cMS_CKS_CTL.m_nValue_2 = 0x0270;

                        if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                            m_cAFETypeInfo.m_cMS_CT_ADC_SH_LMT.m_nValue_2 = 0x002A;
                        else
                            m_cAFETypeInfo.m_cMS_CT_ADC_SH_LMT.m_nValue_2 = 0x002F;

                        if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                        {
                            m_cAFETypeInfo.m_cANA_TP_CTL_00_2.m_nValue_2 = m_cAFETypeInfo.m_cANA_TP_CTL_00.m_nValue_2;
                            m_cAFETypeInfo.m_cANA_TP_CTL_04_2.m_nValue_2 = m_cAFETypeInfo.m_cANA_TP_CTL_04.m_nValue_2;
                        }

                        break;
                    default:
                        break;
                }

                WriteParameter[] cWriteParameter_Array = null;

                if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                {
                    cWriteParameter_Array = new WriteParameter[]
                    {
                        m_cAFETypeInfo.m_cANA_TP_CTL_00,
                        m_cAFETypeInfo.m_cANA_TP_CTL_04,
                        m_cAFETypeInfo.m_cANA_PH_CTL_00,
                        m_cAFETypeInfo.m_cMS_M_S_CTL,
                        m_cAFETypeInfo.m_cMS_CKS_CTL,
                        m_cAFETypeInfo.m_cMS_CT_ADC_SH_LMT,
                        m_cAFETypeInfo.m_cANA_TP_CTL_00_2,
                        m_cAFETypeInfo.m_cANA_TP_CTL_04_2
                    };
                }
                else
                {
                    cWriteParameter_Array = new WriteParameter[]
                    {
                        m_cAFETypeInfo.m_cANA_TP_CTL_00,
                        m_cAFETypeInfo.m_cANA_TP_CTL_04,
                        m_cAFETypeInfo.m_cANA_PH_CTL_00,
                        m_cAFETypeInfo.m_cMS_M_S_CTL,
                        m_cAFETypeInfo.m_cMS_CKS_CTL,
                        m_cAFETypeInfo.m_cMS_CT_ADC_SH_LMT
                    };
                }

                foreach (WriteParameter cWriteParameter in cWriteParameter_Array)
                {
                    WriteCommandInfo cWriteCommandInfo = SetWriteCommandInfo(cWriteParameter.m_eParameterType, cWriteParameter.m_nValue_1, cWriteParameter.m_nValue_2, cWriteParameter.m_nSection);
                    SendWriteCommand(cWriteCommandInfo, cWriteParameter);
                }
            }

            switch (ParamAutoTuning.m_nGen8FilterType)
            {
                case 0:
                    m_eFilterType = FilterType.NA;
                    break;
                case 1:
                    m_eFilterType = FilterType.DisableFilter;
                    break;
                case 2:
                    m_eFilterType = FilterType.LPF_0to300KHz;
                    break;
                case 3:
                    m_eFilterType = FilterType.LPF_0to75KHz;
                    break;
                default:
                    break;
            }

            if (m_eFilterType != FilterType.NA)
            {
                switch (m_eFilterType)
                {
                    case FilterType.DisableFilter:
                        m_cFilterTypeInfo.m_cANA_TP_CTL_02.m_nValue_2 = 0x0000;
                        m_cFilterTypeInfo.m_cANA_TP_CTL_03.m_nValue_2 = 0x0000;
                        m_cFilterTypeInfo.m_cANA_TP_CTL_04.m_nValue_2 = 0x0000;

                        if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                        {
                            m_cFilterTypeInfo.m_cANA_TP_CTL_02_2.m_nValue_2 = m_cFilterTypeInfo.m_cANA_TP_CTL_02.m_nValue_2;
                            m_cFilterTypeInfo.m_cANA_TP_CTL_03_2.m_nValue_2 = m_cFilterTypeInfo.m_cANA_TP_CTL_03.m_nValue_2;
                            m_cFilterTypeInfo.m_cANA_TP_CTL_04_2.m_nValue_2 = m_cFilterTypeInfo.m_cANA_TP_CTL_04.m_nValue_2;
                        }

                        break;
                    case FilterType.LPF_0to300KHz:
                        m_cFilterTypeInfo.m_cANA_TP_CTL_02.m_nValue_2 = 0x1064;
                        m_cFilterTypeInfo.m_cANA_TP_CTL_03.m_nValue_2 = 0x04A4;
                        m_cFilterTypeInfo.m_cANA_TP_CTL_04.m_nValue_2 = 0x6000;

                        if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                        {
                            m_cFilterTypeInfo.m_cANA_TP_CTL_02_2.m_nValue_2 = m_cFilterTypeInfo.m_cANA_TP_CTL_02.m_nValue_2;
                            m_cFilterTypeInfo.m_cANA_TP_CTL_03_2.m_nValue_2 = m_cFilterTypeInfo.m_cANA_TP_CTL_03.m_nValue_2;
                            m_cFilterTypeInfo.m_cANA_TP_CTL_04_2.m_nValue_2 = m_cFilterTypeInfo.m_cANA_TP_CTL_04.m_nValue_2;
                        }

                        break;
                    case FilterType.LPF_0to75KHz:
                        m_cFilterTypeInfo.m_cANA_TP_CTL_02.m_nValue_2 = 0x1257;
                        m_cFilterTypeInfo.m_cANA_TP_CTL_03.m_nValue_2 = 0x1780;
                        m_cFilterTypeInfo.m_cANA_TP_CTL_04.m_nValue_2 = 0x6000;

                        if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                        {
                            m_cFilterTypeInfo.m_cANA_TP_CTL_02_2.m_nValue_2 = m_cFilterTypeInfo.m_cANA_TP_CTL_02.m_nValue_2;
                            m_cFilterTypeInfo.m_cANA_TP_CTL_03_2.m_nValue_2 = m_cFilterTypeInfo.m_cANA_TP_CTL_03.m_nValue_2;
                            m_cFilterTypeInfo.m_cANA_TP_CTL_04_2.m_nValue_2 = m_cFilterTypeInfo.m_cANA_TP_CTL_04.m_nValue_2;
                        }

                        break;
                    default:
                        break;
                }

                WriteParameter[] cWriteParameter_Array = null;

                if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                {
                    cWriteParameter_Array = new WriteParameter[]
                    {
                        m_cFilterTypeInfo.m_cANA_TP_CTL_02,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_03,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_04,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_02_2,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_03_2,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_04_2
                    };
                }
                else
                {
                    cWriteParameter_Array = new WriteParameter[]
                    {
                        m_cFilterTypeInfo.m_cANA_TP_CTL_02,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_03,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_04
                    };
                }

                foreach (WriteParameter cWriteParameter in cWriteParameter_Array)
                {
                    WriteCommandInfo cWriteCommandInfo = SetWriteCommandInfo(cWriteParameter.m_eParameterType, cWriteParameter.m_nValue_1, cWriteParameter.m_nValue_2, cWriteParameter.m_nSection);
                    SendWriteCommand(cWriteCommandInfo, cWriteParameter);
                }
            }
        }

        public void RunUserDefinedCommandScriptFlow(SendCommandInfo cSendCommandInfo)
        {
            foreach (CommandInfo cCommandInfo in cSendCommandInfo.m_cCommandInfo_List)
                SendUserDefinedCommand(cCommandInfo);
        }

        public bool RunCheckSampleFrequencyFlow(ParameterSet cParameterSet)
        {
            int nReadValue_1 = -1, nReadValue_2 = -1;
            bool bResultFlag;

            WriteParameter[] cWriteParameter_Array = null;

            if (m_eMainStep == MainTuningStep.NO)
            {
                cWriteParameter_Array = new WriteParameter[]
                {
                    m_cSampleFrequencyInfo.m_cMPP_SP_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_EFFECT_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_DFT_NUM,
                    m_cSampleFrequencyInfo.m_cPH1_Beacon,
                    m_cSampleFrequencyInfo.m_cPH2_Beacon,
                    m_cSampleFrequencyInfo.m_cPH3_Beacon,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_BSH_ADC_TP_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_EFFECT_FW_SET_COEF_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_DFT_NUM_IQ_FIR_CTL,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_COS_INC_0,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_SIN_INC_0
                };
            }
            else if (m_eMainStep == MainTuningStep.TILTNO)
            {
                cWriteParameter_Array = new WriteParameter[]
                {
                    m_cSampleFrequencyInfo.m_cMPP_SP_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_EFFECT_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_DFT_NUM,
                    m_cSampleFrequencyInfo.m_cPH1_Beacon,
                    m_cSampleFrequencyInfo.m_cPH2_Beacon,
                    m_cSampleFrequencyInfo.m_cPH3_Beacon,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S0,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S0,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S1,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S1,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S2,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S2,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S3,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S3,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_BSH_ADC_TP_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_EFFECT_FW_SET_COEF_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_DFT_NUM_IQ_FIR_CTL,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_COS_INC_0,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_SIN_INC_0
                };
            }

            foreach(WriteParameter cWriteParameter in cWriteParameter_Array)
            {
                bResultFlag = ReadData(ref nReadValue_1, ref nReadValue_2, cWriteParameter.m_eParameterType, cWriteParameter.m_nSection);

                int nSetValue_1 = (cWriteParameter.m_nValue_1 & 0xFFFF);
                int nSetValue_2 = (cWriteParameter.m_nValue_2 & 0xFFFF);

                if (CheckValueIdentical(bResultFlag, 
                                        cWriteParameter.m_eParameterType.ToString(),
                                        nReadValue_1,
                                        nSetValue_1,
                                        nReadValue_2,
                                        nSetValue_2) == false)
                    return false;

                switch (cWriteParameter.m_eParameterType)
                {
                    case ParameterType.PH1_Beacon:
                        cParameterSet.m_nReadPH1 = nReadValue_2;
                        break;
                    case ParameterType.PH2_Beacon:
                        cParameterSet.m_nReadPH2 = nReadValue_2;
                        break;
                }
            }

            return true;
        }

        public bool RunCheckFWOptionFlow()
        {
            int nValue1 = -1, nValue2 = -1;
            bool bResultFlag;

            if (m_eAFEType != AFEType.NA)
            {
                WriteParameter[] cWriteParameter_Array = null;

                if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                {
                    cWriteParameter_Array = new WriteParameter[]
                    {
                        m_cAFETypeInfo.m_cANA_TP_CTL_00,
                        m_cAFETypeInfo.m_cANA_TP_CTL_04,
                        m_cAFETypeInfo.m_cANA_PH_CTL_00,
                        m_cAFETypeInfo.m_cMS_M_S_CTL,
                        m_cAFETypeInfo.m_cMS_CKS_CTL,
                        m_cAFETypeInfo.m_cMS_CT_ADC_SH_LMT,
                        m_cAFETypeInfo.m_cANA_TP_CTL_00_2,
                        m_cAFETypeInfo.m_cANA_TP_CTL_04_2
                    };
                }
                else
                {
                    cWriteParameter_Array = new WriteParameter[]
                    {
                        m_cAFETypeInfo.m_cANA_TP_CTL_00,
                        m_cAFETypeInfo.m_cANA_TP_CTL_04,
                        m_cAFETypeInfo.m_cANA_PH_CTL_00,
                        m_cAFETypeInfo.m_cMS_M_S_CTL,
                        m_cAFETypeInfo.m_cMS_CKS_CTL,
                        m_cAFETypeInfo.m_cMS_CT_ADC_SH_LMT
                    };
                }

                foreach (WriteParameter cWriteParameter in cWriteParameter_Array)
                {
                    bResultFlag = ReadData(ref nValue1, ref nValue2, cWriteParameter.m_eParameterType, cWriteParameter.m_nSection);

                    if (CheckValueIdentical(bResultFlag, 
                                            cWriteParameter.m_eParameterType.ToString(),
                                            nValue1, 
                                            cWriteParameter.m_nValue_1,
                                            nValue2, 
                                            cWriteParameter.m_nValue_2) == false)
                        return false;
                }
            }

            if (m_eFilterType != FilterType.NA)
            {
                WriteParameter[] cWriteParameter_Array = null;

                if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                {
                    cWriteParameter_Array = new WriteParameter[]
                    {
                        m_cFilterTypeInfo.m_cANA_TP_CTL_02,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_03,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_04,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_02_2,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_03_2,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_04_2
                    };
                }
                else
                {
                    cWriteParameter_Array = new WriteParameter[]
                    {
                        m_cFilterTypeInfo.m_cANA_TP_CTL_02,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_03,
                        m_cFilterTypeInfo.m_cANA_TP_CTL_04
                    };
                }

                foreach (WriteParameter cWriteParameter in cWriteParameter_Array)
                {
                    bResultFlag = ReadData(ref nValue1, ref nValue2, cWriteParameter.m_eParameterType, cWriteParameter.m_nSection);

                    if (CheckValueIdentical(bResultFlag, 
                                            cWriteParameter.m_eParameterType.ToString(),
                                            nValue1, 
                                            cWriteParameter.m_nValue_1,
                                            nValue2, 
                                            cWriteParameter.m_nValue_2) == false)
                        return false;
                }
            }

            return true;
        }

        private bool CheckValueIdentical(bool bResultFlag, string sParameterName, int nReadValue1 = 0, int nWriteValue1 = 0, int nReadValue2 = 0, int nWriteValue2 = 0)
        {
            if (bResultFlag == false)
            {
                string sMessage = string.Format("Get {0} Value Error", sParameterName);
                m_sErrorMessage = sMessage;
                OutputMessage(string.Format("-{0}", sMessage));
                return false;
            }
            else
            {
                if (nReadValue1 != nWriteValue1)
                {
                    string sMessage = string.Format("{0} Value Not Match(ReadValue1=0x{1}, WriteValue1=0x{2})", 
                                                    sParameterName, 
                                                    nReadValue1.ToString("X4"),
                                                    nWriteValue1.ToString("X4"));
                    m_sErrorMessage = sMessage;
                    OutputMessage(string.Format("-{0}", sMessage));
                    return false;
                }
                else if (nReadValue2 != nWriteValue2)
                {
                    string sMessage = string.Format("{0} Value Not Match(ReadValue2=0x{1}, WriteValue2=0x{2})", 
                                                    sParameterName, 
                                                    nReadValue2.ToString("X4"),
                                                    nWriteValue2.ToString("X4"));
                    m_sErrorMessage = sMessage;
                    OutputMessage(string.Format("-{0}", sMessage));
                    return false;
                }
                else
                {
                    string sMessage = string.Format("{0} Value Match(ReadValue1=0x{1}, WriteValue1=0x{2}, ReadValue2=0x{3}, WriteValue2=0x{4})", 
                                                    sParameterName,
                                                    nReadValue1.ToString("X4"), 
                                                    nWriteValue1.ToString("X4"),
                                                    nReadValue2.ToString("X4"), 
                                                    nWriteValue2.ToString("X4"));
                    OutputMessage(string.Format("-{0}", sMessage));
                    return true;
                }
            }
        }

        private bool CheckValueValid(bool bResultFlag, string sParameterName, int nReadValue1 = 0, int nReadValue2 = 0)
        {
            if (bResultFlag == false)
            {
                string sMessage = string.Format("Get {0} Value Error", sParameterName);
                m_sErrorMessage = sMessage;
                OutputMessage(string.Format("-{0}", sMessage));
                return false;
            }
            else
            {
                string sMessage = string.Format("Get {0} Value Success(ReadValue1=0x{1}, ReadValue2=0x{2})", 
                                                sParameterName,
                                                nReadValue1.ToString("X4"), 
                                                nReadValue2.ToString("X4"));
                OutputMessage(string.Format("-{0}", sMessage));
                return true;
            }
        }

        public string GetErrorMessage()
        {
            return m_sErrorMessage;
        }

        public void SendRawDataCountCommand()
        {
            byte[] byteCommand_Array = new byte[] { 0x54, 0xBC, 0x00, 0x02 };
            ElanTouch.SendDevCommand(byteCommand_Array, byteCommand_Array.Length, 1000, m_nDeviceIndex);
            Thread.Sleep(10);
        }

        public bool CheckEnterTestMode(int nTimeout = 1000)
        {
            int nRetryCount = 50;
            bool bEnterTestModeFlag = false;

            byte[] byteCommand_Array = new byte[] { 0x53, 0xB5, 0x00, 0x01 };

            ElanTouch.SendDevCommand(byteCommand_Array, byteCommand_Array.Length, 1000, m_nDeviceIndex);

            while (nRetryCount > 0)
            {
                byte[] byteData_Array = new byte[m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_nDeviceIndex);

                if (nResultFlag != ElanTouch.TP_SUCCESS)
                {
                    WriteDebugLog(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(nTimeout);
                    break;
                }

                string sGetACK = "-Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                WriteDebugLog(sGetACK);

                if (byteData_Array[0] == 0x52 && byteData_Array[1] == 0xB5)
                {
                    if (byteData_Array[3] == 0x01)
                    {
                        bEnterTestModeFlag = true;
                        break;
                    }
                }

                nRetryCount--;
            }

            if (bEnterTestModeFlag == true)
                return true;
            else
                return false;
        }

        public bool ReadData(ref int nValue_1, ref int nValue_2, ParameterType eParameterType, int nSection = -1, int nTimeout = 1000)
        {
            nValue_1 = -1;
            nValue_2 = -1;

            bool bResultFlag = false;
            int nRetryCount = 50;

            ReadCommandInfo cReadCommandInfo = SetReadCommandInfo(eParameterType, nSection);

            SendReadCommand(cReadCommandInfo);

            while (nRetryCount > 0)
            {
                byte[] byteData_Array = new byte[m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_nDeviceIndex);

                if (nResultFlag != ElanTouch.TP_SUCCESS)
                {
                    WriteDebugLog(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(nTimeout);
                    bResultFlag = false;
                    break;
                }

                string sGetACK = "-Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                WriteDebugLog(sGetACK);

                if (byteData_Array[0] == cReadCommandInfo.m_byteType &&
                    byteData_Array[1] == cReadCommandInfo.m_byteClass &&
                    byteData_Array[2] == cReadCommandInfo.m_byteAddress1_H && 
                    byteData_Array[3] == cReadCommandInfo.m_byteAddress1_L &&
                    byteData_Array[4] == cReadCommandInfo.m_byteAddress2_H && 
                    byteData_Array[5] == cReadCommandInfo.m_byteAddress2_L)
                {
                    nValue_1 = (ushort)(byteData_Array[6] << 8 | byteData_Array[7]);
                    nValue_2 = (ushort)(byteData_Array[8] << 8 | byteData_Array[9]);
                    bResultFlag = true;
                    break;
                }

                nRetryCount--;
            }

            Thread.Sleep(ParamAutoTuning.m_nGen8SendCommandDelayTime);
            return bResultFlag;
        }

        private void SendWriteCommand(WriteCommandInfo cWriteCommandInfo, WriteParameter cWriteParameter)
        {
            string sParameterName = cWriteParameter.m_eParameterType.ToString();

            if (cWriteParameter.m_nSection < 0)
                sParameterName = cWriteParameter.m_eParameterType.ToString();
            else
                sParameterName = string.Format("{0}_S{1}", sParameterName, cWriteParameter.m_nSection);

            byte byteType = cWriteCommandInfo.m_byteType;
            byte byteClass = cWriteCommandInfo.m_byteClass;
            byte byteAddress1_H = cWriteCommandInfo.m_byteAddress1_H;
            byte byteAddress1_L = cWriteCommandInfo.m_byteAddress1_L;
            byte byteAddress2_H = cWriteCommandInfo.m_byteAddress2_H;
            byte byteAddress2_L = cWriteCommandInfo.m_byteAddress2_L;
            byte byteValue1_H = cWriteCommandInfo.m_byteValue1_H;
            byte byteValue1_L = cWriteCommandInfo.m_byteValue1_L;
            byte byteValue2_H = cWriteCommandInfo.m_byteValue2_H;
            byte byteValue2_L = cWriteCommandInfo.m_byteValue2_L;

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

            string sMessage = string.Format("-Set {0}(Addr1=0x{1}{2}, Addr2=0x{3}{4}, Value1=0x{5}{6}, Value2=0x{7}{8})",
                                            sParameterName,
                                            byteAddress1_H.ToString("X2"),
                                            byteAddress1_L.ToString("X2"),
                                            byteAddress2_H.ToString("X2"),
                                            byteAddress2_L.ToString("X2"),
                                            byteValue1_H.ToString("X2"),
                                            byteValue1_L.ToString("X2"),
                                            byteValue2_H.ToString("X2"),
                                            byteValue2_L.ToString("X2"));
            OutputMessage(sMessage);

            string sSendCommand = "-Send Command :";

            for (int nByteIndex = 0; nByteIndex < byteCommand_Array.Length; nByteIndex++)
                sSendCommand += string.Format(" {0}", byteCommand_Array[nByteIndex].ToString("X2"));

            WriteDebugLog(sSendCommand);

            ElanTouch.SendDevCommand(byteCommand_Array, byteCommand_Array.Length, 1000, m_nDeviceIndex);
            Thread.Sleep(ParamAutoTuning.m_nGen8SendCommandDelayTime);
        }

        private void SendUserDefinedCommand(CommandInfo cCommandInfo)
        {
            ElanTouch.SendDevCommand(cCommandInfo.m_byteCommand_Array, cCommandInfo.m_byteCommand_Array.Length, 1000, m_nDeviceIndex);
            Thread.Sleep(cCommandInfo.m_nDelayTime);

            string sSendCommand = "";

            for (int nByteIndex = 0; nByteIndex < cCommandInfo.m_byteCommand_Array.Length; nByteIndex++)
                sSendCommand += string.Format(" {0}", cCommandInfo.m_byteCommand_Array[nByteIndex].ToString("X2"));

            string sMessage = string.Format("-Send Command :{0}", sSendCommand);
            OutputMessage(sMessage);

            WriteDebugLog(string.Format("-Send User Defined Command :{0}", sSendCommand));
        }

        private void SendReadCommand(ReadCommandInfo cReadCommandInfo)
        {
            byte byteType = cReadCommandInfo.m_byteType;
            byte byteClass = cReadCommandInfo.m_byteClass;
            byte byteAddress1_H = cReadCommandInfo.m_byteAddress1_H;
            byte byteAddress1_L = cReadCommandInfo.m_byteAddress1_L;
            byte byteAddress2_H = cReadCommandInfo.m_byteAddress2_H;
            byte byteAddress2_L = cReadCommandInfo.m_byteAddress2_L;

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

            string sSendCommand = "";

            for (int nByteIndex = 0; nByteIndex < byteCommand_Array.Length; nByteIndex++)
                sSendCommand += string.Format(" {0}", byteCommand_Array[nByteIndex].ToString("X2"));

            WriteDebugLog(string.Format("-Send Command :{0}", sSendCommand));

            ElanTouch.SendDevCommand(byteCommand_Array, byteCommand_Array.Length, 1000, m_nDeviceIndex);
            //Thread.Sleep(20);
        }

        public bool GetTXRXStartTrace_Gen8(ref int nRXStartTrace, ref int nTXStartTrace, int nTimeout = 1000)
        {
            int nRetryCount = 4;
            int nRetryIndex = 0;
            bool bResultFlag = false;

            byte[] byteCommand_Array = new byte[] 
            {
                0x53,
                0x7F,
                0x00,
                0x00
            };

            ElanTouch.SendDevCommand(byteCommand_Array, byteCommand_Array.Length, 1000, m_nDeviceIndex);

            while (nRetryIndex < nRetryCount)
            {
                byte[] byteData_Array = new byte[m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_nDeviceIndex);

                if (nResultFlag != ElanTouch.TP_SUCCESS)
                {
                    WriteDebugLog(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(nTimeout);
                    nRetryIndex++;
                    continue;
                }

                string sGetACK = "-Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                nTXStartTrace = byteData_Array[2];
                nRXStartTrace = byteData_Array[3];

                if (nRetryCount == 4)
                    OutputMessage(string.Format("-Get TX Start Trace={0}, RX Start Trace={1}", nTXStartTrace, nRXStartTrace));
                else
                    OutputMessage(string.Format("-Get TX Start Trace={0}, RX Start Trace={1}(RetryCount={2})", nTXStartTrace, nRXStartTrace, nRetryIndex));

                WriteDebugLog(sGetACK);

                bResultFlag = true;
                Thread.Sleep(20);

                if (bResultFlag == true)
                    break;
            }

            if (bResultFlag == false)
                return false;

            return true;
        }

        public bool CheckUserDefinedFile(List<FlowStep> cFlowStep_List, string sFilePath)
        {
            List<string> sScriptData_List = new List<string>();

            //Read the file and display it line by line.
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            string sLine = "";

            while ((sLine = srFile.ReadLine()) != null)
                sScriptData_List.Add(sLine);

            srFile.Close();

            foreach (FlowStep cFlowStep in cFlowStep_List)
            {
                SendCommandInfo cSendCommandInfo = null;

                bool bCheckResultFlag = CheckFormat(ref cSendCommandInfo, cFlowStep, 0, sScriptData_List);

                if (bCheckResultFlag == false)
                    return false;
            }

            return true;
        }

        public bool LoadUserDefinedCommandScript(ref SendCommandInfo cSendCommandInfo, FlowStep cFlowStep, int nRecordDataIndex, string sFilePath)
        {
            List<string> sScriptData_List = new List<string>();

            //Read the file and display it line by line.
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            string sLine = "";

            while ((sLine = srFile.ReadLine()) != null)
                sScriptData_List.Add(sLine);

            srFile.Close();

            bool bCheckResultFlag = CheckFormat(ref cSendCommandInfo, cFlowStep, nRecordDataIndex, sScriptData_List, true);

            return bCheckResultFlag;
        }

        private bool CheckFormat(ref SendCommandInfo cSendCommandInfo, FlowStep cFlowStep, int nRecordDataIndex, List<string> sScriptData_List, bool bGetSendCommandInfo = false)
        {
            bool bGetSection = false;
            bool bCreateCommandInfo = false;
            string sMainStep = StringConvert.m_dictMainStepCommandScriptMappingTable[cFlowStep.m_eMainStep];
            string sSubStep = StringConvert.m_dictSubStepCommandScriptMappingTable[cFlowStep.m_eSubStep];
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

                    if (sSplitData_Array.Length == 3)
                    {
                        if (sSplitData_Array[0].Trim() == sMainStep && sSplitData_Array[1].Trim() == sSubStep)
                        {
                            if (bGetSendCommandInfo == true)
                            {
                                if (sSplitData_Array[2].Trim().ToUpper() == "ALL")
                                {
                                    bGetSection = true;
                                    bCreateCommandInfo = true;
                                }
                                else if (ElanConvert.CheckIsInt(sSplitData_Array[2].Trim()) == false)
                                {
                                    m_sErrorMessage = string.Format("Record Index Error in Line {0} in User Defined File", nLineIndex + 1);
                                    return false;
                                }
                                else if (Convert.ToInt32(sSplitData_Array[2].Trim()) == nRecordDataIndex + 1)
                                {
                                    bGetSection = true;
                                    bCreateCommandInfo = true;
                                }
                                else
                                    bGetSection = false;
                            }
                            else
                            {
                                if (sSplitData_Array[2].Trim().ToUpper() == "ALL")
                                    bGetSection = true;
                                else if (ElanConvert.CheckIsInt(sSplitData_Array[2].Trim()) == false)
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
                                else if (nByteIndex == 0 && sCommandByte != "00" && ParamAutoTuning.m_nGen8EnableCheckUserDefinedFormat == 1)
                                {
                                    m_sErrorMessage = string.Format("Byte[{0}] Not \"00\" in Line {1} in User Defined File", nByteIndex, nLineIndex + 1);
                                    return false;
                                }
                                else if (ElanConvert.CheckIsHexDecimal(sCommandByte) == false)
                                {
                                    m_sErrorMessage = string.Format("Byte[{0}] Not Hex in Line {1} in User Defined File", nByteIndex, nLineIndex + 1);
                                    return false;
                                }

                                if (nByteIndex == 1)
                                    nByteNumber = Convert.ToInt32(sCommandByte, 16);
                            }

                            if (sCommandByte_Array.Length != nByteNumber + 2 && ParamAutoTuning.m_nGen8EnableCheckUserDefinedFormat == 1)
                            {
                                m_sErrorMessage = string.Format("Command Length Error in Line {0} in User Defined File", nLineIndex + 1);
                                return false;
                            }
                            else
                            {
                                if (bGetSendCommandInfo == true)
                                {
                                    if (ParamAutoTuning.m_nGen8EnableCheckUserDefinedFormat == 1)
                                    {
                                        byte[] byteCommand_Array = new byte[nByteNumber];
                                        Array.Clear(byteCommand_Array, 0, byteCommand_Array.Length);

                                        for (int nByteIndex = 0; nByteIndex < nByteNumber; nByteIndex++)
                                        {
                                            short nValue = Int16.Parse(sCommandByte_Array[nByteIndex + 2], System.Globalization.NumberStyles.HexNumber);
                                            byteCommand_Array[nByteIndex] = (byte)nValue;
                                        }

                                        cCommandInfo = new CommandInfo();
                                        cCommandInfo.m_byteCommand_Array = byteCommand_Array;
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
                                        cCommandInfo.m_byteCommand_Array = byteCommand_Array;
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

                            if (ElanConvert.CheckIsInt(sDelayTime) == false)
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
                                        cCommandInfo.m_nDelayTime = Convert.ToInt32(sDelayTime);
                                        cSendCommandInfo.m_cCommandInfo_List.Add(cCommandInfo);
                                        cCommandInfo = null;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public static byte[] ConvertGetReportTypeToByteArray(GetReportType eGetReportType, GetReportInfo cGetReportInfo = null, bool bEnterSetMPPParameterFlag = false)
        {
            byte[] byteCommand_Array;

            switch (eGetReportType)
            {
                case GetReportType.debug_Rx:
                    byte byteReportNumberHighByte = (byte)((cGetReportInfo.m_nReportNumber & 0xFF00) >> 8);
                    byte byteReportNumberLowByte = (byte)(cGetReportInfo.m_nReportNumber & 0x00FF);

                    byteCommand_Array = new byte[] 
                    { 
                        0x6C, 
                        0x00, 
                        0x03, 
                        (byte)cGetReportInfo.m_nTraceOffset,
                        (byte)cGetReportInfo.m_nRowOffset, 
                        (byte)cGetReportInfo.m_nTraceNumber,
                        (byte)cGetReportInfo.m_nRowNumber, 
                        byteReportNumberHighByte, 
                        byteReportNumberLowByte, 
                        0x00 
                    };
                    break;
                case GetReportType.debug_Tx:
                    byteReportNumberHighByte = (byte)((cGetReportInfo.m_nReportNumber & 0xFF00) >> 8);
                    byteReportNumberLowByte = (byte)(cGetReportInfo.m_nReportNumber & 0x00FF);

                    byteCommand_Array = new byte[] 
                    { 
                        0x6C, 
                        0x00, 
                        0x03, 
                        (byte)cGetReportInfo.m_nTraceOffset,
                        (byte)cGetReportInfo.m_nRowOffset, 
                        (byte)cGetReportInfo.m_nTraceNumber,
                        (byte)cGetReportInfo.m_nRowNumber, 
                        byteReportNumberHighByte, 
                        byteReportNumberLowByte, 
                        0x00 
                    };
                    break;
                case GetReportType.debug_PTHF_noSync:
                case GetReportType.debug_BHF_noSync:
                    byteReportNumberHighByte = (byte)((cGetReportInfo.m_nReportNumber & 0xFF00) >> 8);
                    byteReportNumberLowByte = (byte)(cGetReportInfo.m_nReportNumber & 0x00FF);

                    byteCommand_Array = new byte[] 
                    { 
                        0x6C, 
                        0x00, 
                        0x03, 
                        (byte)cGetReportInfo.m_nTraceOffset,
                        (byte)cGetReportInfo.m_nRowOffset, 
                        (byte)cGetReportInfo.m_nTraceNumber,
                        (byte)cGetReportInfo.m_nRowNumber, 
                        byteReportNumberHighByte, 
                        byteReportNumberLowByte, 
                        0x00 
                    };
                    break;
                case GetReportType.Stop:
                default:
                    byte byteHighByte = 0x00, byteLowByte = 0x00;

                    if (bEnterSetMPPParameterFlag == true)
                        byteLowByte = 0x02;

                    byteCommand_Array = new byte[] 
                    { 
                        0x6C, 
                        byteHighByte, 
                        byteLowByte, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00 
                    };
                    break;
            }
            return byteCommand_Array;
        }

        public static byte[] ConvertMPPModeTypeToByteArray(MPPModeType eMPPModeType)
        {
            byte[] byteCommand_Array;

            switch (eMPPModeType)
            {
                case MPPModeType.RX_DFT:
                    byteCommand_Array = new byte[] 
                    { 
                        0x54, 
                        0xB2, 
                        0x01, 
                        0x08 
                    };
                    break;
                case MPPModeType.TX_DFT:
                    byteCommand_Array = new byte[] 
                    { 
                        0x54, 
                        0xB2,
                        0x01, 
                        0x10 
                    };
                    break;
                case MPPModeType.PTHF_NoSync:
                    byteCommand_Array = new byte[] 
                    { 
                        0x54, 
                        0xB2,
                        0x01, 
                        0x86
                    };
                    break;
                case MPPModeType.BHF_NoSync:
                    byteCommand_Array = new byte[] 
                    { 
                        0x54, 
                        0xB2,
                        0x01, 
                        0x84
                    };
                    break;
                case MPPModeType.Normal:
                default:
                    byteCommand_Array = new byte[] 
                    { 
                        0x54, 
                        0xB2, 
                        0x00, 
                        0x00 
                    };
                    break;
            }
            return byteCommand_Array;
        }

        public static byte[] GetMPPModeStatus(MPPModeType eMPPModeType)
        {
            byte[] byteData_Array;

            switch (eMPPModeType)
            {
                case MPPModeType.RX_DFT:
                    byteData_Array = new byte[] 
                    { 
                        0x01, 
                        0x08 
                    };
                    break;
                case MPPModeType.TX_DFT:
                    byteData_Array = new byte[] 
                    { 
                        0x01, 
                        0x10 
                    };
                    break;
                case MPPModeType.PTHF_NoSync:
                    byteData_Array = new byte[] 
                    { 
                        0x01, 
                        0x86
                    };
                    break;
                case MPPModeType.BHF_NoSync:
                    byteData_Array = new byte[] 
                    { 
                        0x01, 
                        0x84
                    };
                    break;
                case MPPModeType.Normal:
                default:
                    byteData_Array = new byte[] 
                    {  
                        0x00, 
                        0x00 
                    };
                    break;
            }
            return byteData_Array;
        }

        public static byte[] ConvertMPPSetParameterToByteArray(MPPSetParameter eMPPSetParameter)
        {
            byte[] byteCommand_Array;

            switch (eMPPSetParameter)
            {
                case MPPSetParameter.Enable:
                    byteCommand_Array = new byte[] 
                    { 
                        0x6C, 
                        0x00, 
                        0x02, 
                        0x00,
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00 
                    };
                    break;
                case MPPSetParameter.Disable:
                default:
                    byteCommand_Array = new byte[] 
                    { 
                        0x6C, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00, 
                        0x00 
                    };
                    break;
            }
            return byteCommand_Array;
        }

        public bool RunReadOriginFWParameterFlow()
        {
            if (m_cOriginFWValue.m_bGetValueFlag == true)
                return true;

            int nValue1 = -1, nValue2 = -1;
            bool bResultFlag;
            int nRetryCount = 3;

            int nTX_PPI_H = 0;
            int nTX_PPI_H_S0 = 0;
            int nTX_PPI_H_S1 = 0;
            int nTX_PPI_H_S2 = 0;
            int nTX_PPI_H_S3 = 0;

            WriteParameter[] cReadParameter_Array = null;

            if (m_eMainStep == MainTuningStep.NO)
            {
                cReadParameter_Array = new WriteParameter[]
                {
                    m_cSampleFrequencyInfo.m_cMPP_SP_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_EFFECT_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_DFT_NUM,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_BSH_ADC_TP_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_EFFECT_FW_SET_COEF_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_DFT_NUM_IQ_FIR_CTL
                };
            }
            else if (m_eMainStep == MainTuningStep.TILTNO)
            {
                cReadParameter_Array = new WriteParameter[]
                {
                    m_cSampleFrequencyInfo.m_cMPP_SP_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_EFFECT_NUM,
                    m_cSampleFrequencyInfo.m_cMPP_DFT_NUM,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S0,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S0,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S1,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S1,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S2,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S2,
                    m_cSampleFrequencyInfo.m_cTX_PPI_H_S3,
                    m_cSampleFrequencyInfo.m_cTX_PPI_L_S3,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_BSH_ADC_TP_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_EFFECT_FW_SET_COEF_NUM,
                    m_cSampleFrequencyInfo.m_c_PEN_MS_DFT_NUM_IQ_FIR_CTL
                };
            }

            foreach (WriteParameter cReadParameter in cReadParameter_Array)
            {
                for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
                {
                    bResultFlag = ReadData(ref nValue1, ref nValue2, cReadParameter.m_eParameterType, cReadParameter.m_nSection);

                    if (CheckValueValid(bResultFlag, cReadParameter.m_eParameterType.ToString(), nValue1, nValue2) == false)
                    {
                        if (nRetryIndex == nRetryCount)
                            return false;
                        else
                            continue;
                    }

                    switch (cReadParameter.m_eParameterType)
                    {
                        case ParameterType.MPP_SP_NUM:
                            m_cOriginFWValue.m_nSP_NUM = nValue2;
                            break;
                        case ParameterType.MPP_EFFECT_NUM:
                            m_cOriginFWValue.m_nEFFECT_NUM = nValue2;
                            break;
                        case ParameterType.MPP_DFT_NUM:
                            m_cOriginFWValue.m_nDFT_NUM = nValue2;
                            break;
                        case ParameterType._PEN_MS_BSH_ADC_TP_NUM:
                            m_cOriginFWValue.m_nBSH_ADC_TP_NUM_Other_H = (nValue1 & 0x000F);
                            m_cOriginFWValue.m_nBSH_ADC_TP_NUM_Other_L = nValue2;
                            break;
                        case ParameterType._PEN_MS_EFFECT_FW_SET_COEF_NUM:
                            m_cOriginFWValue.m_nEFFECT_FW_SET_COEF_NUM_Other = nValue2;
                            break;
                        case ParameterType._PEN_MS_DFT_NUM_IQ_FIR_CTL:
                            m_cOriginFWValue.m_nDFT_NUM_IQ_FIR_CTL_Other = nValue2;
                            break;
                        /*
                        case ParameterType.TX_PPI_H:
                            if (cReadParameter.m_nSection < 0)
                                nTX_PPI_H = nValue2;
                            else if (cReadParameter.m_nSection == 0)
                                nTX_PPI_H_S0 = nValue2;
                            else if (cReadParameter.m_nSection == 1)
                                nTX_PPI_H_S1 = nValue2;
                            else if (cReadParameter.m_nSection == 2)
                                nTX_PPI_H_S2 = nValue2;
                            else if (cReadParameter.m_nSection == 3)
                                nTX_PPI_H_S3 = nValue2;

                            break;
                        case ParameterType.TX_PPI_L:
                            if (cReadParameter.m_nSection < 0)
                                m_cOriginFWValue.m_nRX_OR_TX_NUM = (int)(((nTX_PPI_H << 16) + nValue2) / m_cOriginFWValue.m_nEFFECT_NUM);
                            else if (cReadParameter.m_nSection == 0)
                                m_cOriginFWValue.m_nTRxS0_ADC_TP_NUM_SUM = (int)(((nTX_PPI_H_S0 << 16) + nValue2) / m_cOriginFWValue.m_nEFFECT_NUM);
                            else if (cReadParameter.m_nSection == 1)
                                m_cOriginFWValue.m_nTRxS1_ADC_TP_NUM_SUM = (int)(((nTX_PPI_H_S1 << 16) + nValue2) / m_cOriginFWValue.m_nEFFECT_NUM);
                            else if (cReadParameter.m_nSection == 2)
                                m_cOriginFWValue.m_nTRxS2_ADC_TP_NUM_SUM = (int)(((nTX_PPI_H_S2 << 16) + nValue2) / m_cOriginFWValue.m_nEFFECT_NUM);
                            else if (cReadParameter.m_nSection == 3)
                                m_cOriginFWValue.m_nTRxS3_ADC_TP_NUM_SUM = (int)(((nTX_PPI_H_S3 << 16) + nValue2) / m_cOriginFWValue.m_nEFFECT_NUM);

                            break;
                         */
                        case ParameterType.TX_PPI_H:
                            nTX_PPI_H = nValue2;
                            break;
                        case ParameterType.TX_PPI_H_S0:
                            nTX_PPI_H_S0 = nValue2;
                            break;
                        case ParameterType.TX_PPI_H_S1:
                            nTX_PPI_H_S1 = nValue2;
                            break;
                        case ParameterType.TX_PPI_H_S2:
                            nTX_PPI_H_S2 = nValue2;
                            break;
                        case ParameterType.TX_PPI_H_S3:
                            nTX_PPI_H_S3 = nValue2;
                            break;
                        case ParameterType.TX_PPI_L:
                            m_cOriginFWValue.m_nRX_OR_TX_NUM = (int)(((nTX_PPI_H << 16) + nValue2) / m_cOriginFWValue.m_nEFFECT_NUM);
                            break;
                        case ParameterType.TX_PPI_L_S0:
                            m_cOriginFWValue.m_nTRxS0_ADC_TP_NUM_SUM = (int)(((nTX_PPI_H_S0 << 16) + nValue2) / m_cOriginFWValue.m_nEFFECT_NUM);
                            break;
                        case ParameterType.TX_PPI_L_S1:
                            m_cOriginFWValue.m_nTRxS1_ADC_TP_NUM_SUM = (int)(((nTX_PPI_H_S1 << 16) + nValue2) / m_cOriginFWValue.m_nEFFECT_NUM);
                            break;
                        case ParameterType.TX_PPI_L_S2:
                            m_cOriginFWValue.m_nTRxS2_ADC_TP_NUM_SUM = (int)(((nTX_PPI_H_S2 << 16) + nValue2) / m_cOriginFWValue.m_nEFFECT_NUM);
                            break;
                        case ParameterType.TX_PPI_L_S3:
                            m_cOriginFWValue.m_nTRxS3_ADC_TP_NUM_SUM = (int)(((nTX_PPI_H_S3 << 16) + nValue2) / m_cOriginFWValue.m_nEFFECT_NUM);
                            break;
                        default:
                            break;
                    }

                    if (bResultFlag == true)
                        break;
                }
            }

            m_cOriginFWValue.m_bGetValueFlag = true;
            return true;
        }

        private void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmMain.OutputMessage(sMessage, bWarning);
        }

        private void WriteDebugLog(string sMessage)
        {
            m_cfrmMain.WriteDebugLog(sMessage);
        }
    }
}
