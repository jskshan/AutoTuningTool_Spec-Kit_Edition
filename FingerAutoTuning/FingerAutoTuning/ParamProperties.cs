using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    public class MainConstantParameter
    {
        public const int m_nTPCLOCK_63xx = 32000000;
        public const int m_nSIMULATEMAXDV = 0x200;
        public const int m_nIDEALSCANTIME = 120;

        public const string m_sDATATYPE_DV = "DV";
        public const string m_sDATATYPE_BASE = "BASE";
        public const string m_sDATATYPE_ADC = "ADC";
        public const string m_sDATATYPE_KPKN = "KPKN";
        public const string m_sDATATYPE_BASEMinusADC = "BASE-ADC";
        public const string m_sDATATYPE_CHART = "Chart";
        public const string m_sDATATYPE_RESULT = "Result";
        public const string m_sDATATYPE_RAW_DV = "RAW_DV";
        public const string m_sDATATYPE_RAW_BASE = "RAW_BASE";
        public const string m_sDATATYPE_RAW_ADC = "RAW_ADC";
        public const string m_sDATATYPE_RAW_BASEMinusADC = "RAW_BASE-ADC";
        public const string m_sDATATYPE_PREREPORT = "PreReport";
        public const string m_sDATATYPE_OBASE = "OBASE";
        public const string m_sDATATYPE_RAW_OBASE = "RAW_OBASE";
        public const string m_sDATATYPE_OBASEMinusADC = "OBASE-ADC";
        public const string m_sDATATYPE_RAW_OBASEMinusADC = "RAW_OBASE-ADC";
        public const string m_sDATATYPE_ANALYSIS = "Analysis";
        public const string m_sDATATYPE_REPORTMODE = "ReportMode";
        public const string m_sDATATYPE_REPORTMODE_SIGNAL = "ReportMode(S)";
        public const string m_sDATATYPE_RawData = "RawData";
        public const string m_sDATATYPE_RAW_RawData = "RAW_RawData";
        public const string m_sDATATYPE_RecentADCMEANMinusADC = "RADCMean-ADC";

        public const string m_sSOCKET_WINDOWS = "Windows";
        public const string m_sSOCKET_ANDROID_REMOTECLIENT = "Android(RemoteClient)";
        public const string m_sSOCKET_ANDROID_ARMTOOL = "Android(ARM_Tool)";
        public const string m_sSOCKET_CHROME_SSHSOCKETSERVER = "Chrome(SSHSocketServer)";
        public const string m_sSOCKET_CHROME_REMOTECLIENT = "Chrome(RemoteClient)";
        public const string m_sSOCKET_OTHER_SSHSOCKETSERVER = "OtherSSHSocketServer";

        public const string m_sINTERFACE_HIDOVERI2C = "HIDOverI2C";
        public const string m_sINTERFACE_I2C = "I2C";
        public const string m_sINTERFACE_USB = "USB";
        public const string m_sINTERFACE_SPI_MA_RISING_HALF = "SPI(MA_Rising_Half)";
        public const string m_sINTERFACE_SPI_MA_FALLING_HALF = "SPI(MA_Falling_Half)";
        public const string m_sINTERFACE_SPI_MA_RISING = "SPI(MA_Rising)";
        public const string m_sINTERFACE_SPI_MA_FALLING = "SPI(MA_Falling)";

        public const string m_sANDROIDSOCKET_SERVER = "Server";
        public const string m_sANDROIDSOCKET_CLIENT = "Client";

        public static TraceType[] m_eSelfTraceType_Array = null;

        public const string m_sOddTrace = "OddTrace";
        public const string m_sEvenTrace = "EvenTrace";
        public const string m_sForwardTrace = "ForwardTrace";
        public const string m_sBackwardTrace = "BackwardTrace";
        public const string m_sAllTrace = "AllTrace";

        public const string m_sKSequence_FixedValue = "FixedValue";
        public const string m_sKSequence_FileValue = "FileValue";
        public const string m_sKSequence_NA = "NA";

        public const int m_nGetBaseType_UseBaseAtFirst      = 0;
        public const int m_nGetBaseType_UseBaseAfterGetADC  = 1;
        public const int m_nGetBaseType_UseADCAfterGetADC   = 2;
        public const int m_nGetBaseType_UseNewMethodGetADC  = 3;
        public const int m_nGetBaseType_UseADCAtFirst       = 4;

        public MainConstantParameter()
        {
        }

        public static void SetSelfTraceType()
        {
            if (ParamFingerAutoTuning.m_nSelfTraceType == 0 || ParamFingerAutoTuning.m_nSelfTraceType > 2)
            {
                m_eSelfTraceType_Array = new TraceType[]
                {
                    TraceType.RX
                };
            }
            else if (ParamFingerAutoTuning.m_nSelfTraceType == 1)
            {
                m_eSelfTraceType_Array = new TraceType[]
                {
                    TraceType.TX
                };
            }
            else if (ParamFingerAutoTuning.m_nSelfTraceType == 2)
            {
                m_eSelfTraceType_Array = new TraceType[]
                {
                    TraceType.RX,
                    TraceType.TX
                };
            }
        }

        public static ICSolutionType SetICSolutionType_Gen8(int nFWVersionHighByte)
        {
            if (nFWVersionHighByte == 0x81)
                return ICSolutionType.Solution_8F09;
            else if (nFWVersionHighByte == 0x82)
                return ICSolutionType.Solution_8F11;
            else if (nFWVersionHighByte == 0x83)
                return ICSolutionType.Solution_8F18;
            else
                return ICSolutionType.Solution_8F09;
        }

        public static string GetICSolutionType_Gen8(ICSolutionType eICSolutionType)
        {
            if (eICSolutionType == ICSolutionType.Solution_8F09)
                return "8F09";
            else if (eICSolutionType == ICSolutionType.Solution_8F11)
                return "8F11";
            else if (eICSolutionType == ICSolutionType.Solution_8F18)
                return "8F18";
            else if (eICSolutionType == ICSolutionType.NA)
                return "NA";
            else
                return "Other";
        }

        public static ICSolutionType SetICSolutionType_Gen7(int nFWVersionHighByte)
        {
            if (nFWVersionHighByte == 0x67 || nFWVersionHighByte == 0x68)
                return ICSolutionType.Solution_7318;
            else if (nFWVersionHighByte == 0x64 || nFWVersionHighByte == 0x65)
                return ICSolutionType.Solution_7315;
            else
                return ICSolutionType.NA;
        }

        public static string GetICSolutionType_Gen7(ICSolutionType eICSolutionType)
        {
            if (eICSolutionType == ICSolutionType.Solution_7315)
                return "7315";
            else if (eICSolutionType == ICSolutionType.Solution_7318)
                return "7318";
            else if (eICSolutionType == ICSolutionType.NA)
                return "NA";
            else
                return "Other";
        }

        public static ICSolutionType SetICSolutionType_Gen6(int nFWVersionHighByte)
        {
            if (nFWVersionHighByte == 0x61 || nFWVersionHighByte == 0x62)
                return ICSolutionType.Solution_6315;
            else if (nFWVersionHighByte == 0x63)
                return ICSolutionType.Solution_6308;
            else if (nFWVersionHighByte == 0x59)
                return ICSolutionType.Solution_5015M;
            else
                return ICSolutionType.NA;
        }

        public static string GetICSolutionType_Gen6(ICSolutionType eICSolutionType)
        {
            if (eICSolutionType == ICSolutionType.Solution_6315)
                return "6315";
            else if (eICSolutionType == ICSolutionType.Solution_6308)
                return "6308";
            else if (eICSolutionType == ICSolutionType.Solution_5015M)
                return "5015M";
            else if (eICSolutionType == ICSolutionType.NA)
                return "NA";
            else
                return "Other";
        }
    }

    public class FrequencyItem
    {
        public int m_nTXTraceNumber;
        public int m_nRXTraceNumber;
        public int m_nPH1;
        public int m_nPH2;
        public double m_dFrequency;
        public int m_nDFT_NUM;
        //public int m_nSugDFT_NUM;
        public int m_nADCTestFrame;
        public int m_nDVTestFrame;
        public string m_sOBASEFilePath;
        public string m_sBASEFilePath;
        public string m_sADCFilePath;

        public bool m_bEnableTXn;

        public int m_n_SELF_PH1;
        public int m_n_SELF_PH2E_LAT;
        public int m_n_SELF_PH2E_LMT;
        public int m_n_SELF_PH2_LAT;
        public int m_n_SELF_PH2;
        public double m_dSelf_Frequency;
        public int m_nSelf_DFT_NUM;
        public int m_nSelf_Gain;
        public int m_nSelf_CAL;
        public int m_nSelf_CAG;
        public int m_nSelf_IQ_BSH;
        public double m_dSelf_SampleTime;

        public int m_nSelf_NCP;
        public int m_nSelf_NCN;
        public int m_nSetIndex;
    }

    public class RawADCSweepItem
    {
        public int m_nFIR_TAP_NUM;
        // Gen8
        public int m_nFIRCOEF_SEL;
        // Gen7
        public int m_nFIRTB;

        public string m_sADCFilePath;

        public int m_nDFT_NUM;
        public int m_nIQ_BSH;
        public int m_nSELC;
        public int m_nVSEL;
        public int m_nLG;
        public int m_nSELGM;

        public int m_nADCTestFrame;

        public int m_nSetIndex;
    }

    public enum ICGenerationType
    {
        None = -1,
        Other = 0,
        Gen8 = 1,
        Gen9 = 2,
        Gen7 = 3,
        Gen6 = 4
    }

    public enum ICSolutionType
    {
        NA = -1,
        Solution_8F09 = 0,
        Solution_8F11 = 1,
        Solution_8F18 = 2,
        Solution_9F07 = 3,
        Solution_7315 = 4,
        Solution_7318 = 5,
        Solution_6315 = 6,
        Solution_6308 = 7,
        Solution_5015M = 8
    }

    public enum TraceType
    {
        ALL,
        RX,
        TX
    }

    public class SelfKParameter
    {
        public int m_nNCPValue = 0;
        public int m_nNCNValue = 0;
    }

    public class GetDataInfo
    {
        public frmMain.FlowStep m_cFlowStep;
        public FrequencyItem m_cFrequencyItem;
        public RawADCSweepItem m_cRawADCSweepItem;
        public string m_sDataType;
        public int[,,] m_nFrameData_Array;
        public int m_nFrameNumber;
        public int m_nRXTraceNumber;
        public int m_nTXTraceNumber;
        public int m_nListIndex;
        public int m_nListCount;
        public string m_sStageMessage = "";
        public bool m_bUseNewMethod = false;
        public bool m_bGetBaseDelay = false;
        public bool m_bGetSelf = false;
        public bool m_bSetKParameter = false;
        public bool m_bRawADCSweep = false;
    }
}
