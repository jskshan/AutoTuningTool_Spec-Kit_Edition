using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPPPenAutoTuningParameter;

namespace Elan
{
    public class ElanCommand
    {
        public enum ElanCommandType
        {
            //Command Without Parameter
            NA,
            ResetIC,
            EnableICReport,
            DisableICReport,
            DisableFingerReport,
            GetParam_AP_pPeakThrdshold,
            GetPH1,
            GetPH2,
            GetReportNumber,
            GetP0_TH,
            GetTRxS_Hover_TH_Rx,
            GetTRxS_Hover_TH_Tx,
            GetTRxS_Contact_TH_Rx,
            GetTRxS_Contact_TH_Tx,
            GetHover_TH_Rx,
            GetHover_TH_Tx,
            GetContact_TH_Rx,
            GetContact_TH_Tx,
            GetPTHF_Hover_TH_Rx,
            GetPTHF_Hover_TH_Tx,
            GetPTHF_Contact_TH_Rx,
            GetPTHF_Contact_TH_Tx,
            GetBHF_Hover_TH_Rx,
            GetBHF_Hover_TH_Tx,
            GetBHF_Contact_TH_Rx,
            GetBHF_Contact_TH_Tx,
            GetEdge_1Trc_SubPwr,
            GetEdge_2Trc_SubPwr,
            GetEdge_3Trc_SubPwr,
            GetEdge_4Trc_SubPwr,
            GetIQ_BSH_P,
            GetPressure3BinsTH,
            Get3BinsPwr,
            GetParam_DigiGain_P0,
            GetParam_DigiGain_Beacon_Rx,
            GetParam_DigiGain_Beacon_Tx,
            GetParam_DigiGain_PTHF_Rx,
            GetParam_DigiGain_PTHF_Tx,
            GetParam_DigiGain_BHF_Rx,
            GetParam_DigiGain_BHF_Tx,
            GetParam_Pen_HI_HF_THD,
            GetSNVersion,
            SetNoisePTHF,
            SetNoiseBHF,
            GetNoiseTRX_400us,
            GetNoiseRX_400us,
            GetNoiseTX_400us,
            GetNoiseTRX_800us,
            GetNoiseRX_800us,
            GetNoiseTX_800us,
            GetSyncTRxS,
            GetNonSyncRX_400us,
            GetNonSyncTX_400us,
            GetNonSyncTRX_400us,
            GetNonSyncRX_800us,
            GetNonSyncTX_800us,
            GetNonSyncTRX_800us,
            GetDigitalGain,
            SetCoordFF,
            GetTiltPTHF,
            GetTiltBHF,
            GetPressure,
            GetLinearity,
            Get5TRawData,
            ResetTiltNoise,
            ResetTilt,
            StopNonSyncRXTX,
            SetParam_AP_pPeakThrdshold,
            // command with one parameter
            SetPH1,
            SetPH2,
            // command with two parameter (HighByte, LowByte)
            SetReportNumber,
            SetP0_TH,
            SetTRxS_Hover_TH_Rx,
            SetTRxS_Hover_TH_Tx,
            SetTRxS_Contact_TH_Rx,
            SetTRxS_Contact_TH_Tx,
            SetHover_TH_Rx,
            SetHover_TH_Tx,
            SetContact_TH_Rx,
            SetContact_TH_Tx,
            SetPTHF_Hover_TH_Rx,
            SetPTHF_Hover_TH_Tx,
            SetPTHF_Contact_TH_Rx,
            SetPTHF_Contact_TH_Tx,
            SetBHF_Hover_TH_Rx,
            SetBHF_Hover_TH_Tx,
            SetBHF_Contact_TH_Rx,
            SetBHF_Contact_TH_Tx,
            SetEdge_1Trc_SubPwr,
            SetEdge_2Trc_SubPwr,
            SetEdge_3Trc_SubPwr,
            SetEdge_4Trc_SubPwr,
            SetIQ_BSH_P,
            SetPressure3BinsTH,
            Set3BinsPwr,
            SetParam_DigiGain_P0,
            SetParam_DigiGain_Beacon_Rx,
            SetParam_DigiGain_Beacon_Tx,
            SetParam_DigiGain_PTHF_Rx,
            SetParam_DigiGain_PTHF_Tx,
            SetParam_DigiGain_BHF_Rx,
            SetParam_DigiGain_BHF_Tx,
            SetParam_Pen_HI_HF_THD,
            SetRead_Bulk_RAM_Data
        }

        public enum ICValueTargetType
        {
            NA,
            Param_AP_pPeakThrdshold,
            PH1,
            PH2,
            ReportNumber,
            P0_TH,
            TRxS_Hover_TH_Rx,
            TRxS_Hover_TH_Tx,
            TRxS_Contact_TH_Rx,
            TRxS_Contact_TH_Tx,
            Hover_TH_Rx,
            Hover_TH_Tx,
            Contact_TH_Rx,
            Contact_TH_Tx,
            PTHF_Hover_TH_Rx,
            PTHF_Hover_TH_Tx,
            PTHF_Contact_TH_Rx,
            PTHF_Contact_TH_Tx,
            BHF_Hover_TH_Rx,
            BHF_Hover_TH_Tx,
            BHF_Contact_TH_Rx,
            BHF_Contact_TH_Tx,
            Edge_1Trc_SubPwr,
            Edge_2Trc_SubPwr,
            Edge_3Trc_SubPwr,
            Edge_4Trc_SubPwr,
            IQ_BSH_P,
            Pressure3BinsTH,
            Press_3BinsPwr,
            Param_DigiGain_P0,
            Param_DigiGain_Beacon_Rx,
            Param_DigiGain_Beacon_Tx,
            Param_DigiGain_PTHF_Rx,
            Param_DigiGain_PTHF_Tx,
            Param_DigiGain_BHF_Rx,
            Param_DigiGain_BHF_Tx,
            Param_Pen_HI_HF_THD
        }

        public static Dictionary<ICValueTargetType, ElanCommandType> m_dictSetCmdMappingTable = new Dictionary<ICValueTargetType, ElanCommandType>()
        {
            { ICValueTargetType.NA,                         ElanCommandType.NA },
            { ICValueTargetType.Param_AP_pPeakThrdshold,    ElanCommandType.SetParam_AP_pPeakThrdshold },
            { ICValueTargetType.PH1,                        ElanCommandType.SetPH1 },
            { ICValueTargetType.PH2,                        ElanCommandType.SetPH2 },
            { ICValueTargetType.ReportNumber,               ElanCommandType.SetReportNumber },
            { ICValueTargetType.P0_TH,                      ElanCommandType.SetP0_TH },
            { ICValueTargetType.TRxS_Hover_TH_Rx,           ElanCommandType.SetTRxS_Hover_TH_Rx },
            { ICValueTargetType.TRxS_Hover_TH_Tx,           ElanCommandType.SetTRxS_Hover_TH_Tx },
            { ICValueTargetType.TRxS_Contact_TH_Rx,         ElanCommandType.SetTRxS_Contact_TH_Rx },
            { ICValueTargetType.TRxS_Contact_TH_Tx,         ElanCommandType.SetTRxS_Contact_TH_Tx },
            { ICValueTargetType.Hover_TH_Rx,                ElanCommandType.SetHover_TH_Rx },
            { ICValueTargetType.Hover_TH_Tx,                ElanCommandType.SetHover_TH_Tx },
            { ICValueTargetType.Contact_TH_Rx,              ElanCommandType.SetContact_TH_Rx },
            { ICValueTargetType.Contact_TH_Tx,              ElanCommandType.SetContact_TH_Tx },
            { ICValueTargetType.PTHF_Hover_TH_Rx,           ElanCommandType.SetPTHF_Hover_TH_Rx },
            { ICValueTargetType.PTHF_Hover_TH_Tx,           ElanCommandType.SetPTHF_Hover_TH_Tx },
            { ICValueTargetType.PTHF_Contact_TH_Rx,         ElanCommandType.SetPTHF_Contact_TH_Rx },
            { ICValueTargetType.PTHF_Contact_TH_Tx,         ElanCommandType.SetPTHF_Contact_TH_Tx },
            { ICValueTargetType.BHF_Hover_TH_Rx,            ElanCommandType.SetBHF_Hover_TH_Rx },
            { ICValueTargetType.BHF_Hover_TH_Tx,            ElanCommandType.SetBHF_Hover_TH_Tx },
            { ICValueTargetType.BHF_Contact_TH_Rx,          ElanCommandType.SetBHF_Contact_TH_Rx },
            { ICValueTargetType.BHF_Contact_TH_Tx,          ElanCommandType.SetBHF_Contact_TH_Tx },
            { ICValueTargetType.Edge_1Trc_SubPwr,           ElanCommandType.SetEdge_1Trc_SubPwr },
            { ICValueTargetType.Edge_2Trc_SubPwr,           ElanCommandType.SetEdge_2Trc_SubPwr },
            { ICValueTargetType.Edge_3Trc_SubPwr,           ElanCommandType.SetEdge_3Trc_SubPwr },
            { ICValueTargetType.Edge_4Trc_SubPwr,           ElanCommandType.SetEdge_4Trc_SubPwr },
            { ICValueTargetType.IQ_BSH_P,                   ElanCommandType.SetIQ_BSH_P },
            { ICValueTargetType.Pressure3BinsTH,            ElanCommandType.SetPressure3BinsTH },
            { ICValueTargetType.Press_3BinsPwr,             ElanCommandType.Set3BinsPwr },
            { ICValueTargetType.Param_DigiGain_P0,          ElanCommandType.SetParam_DigiGain_P0 },
            { ICValueTargetType.Param_DigiGain_Beacon_Rx,   ElanCommandType.SetParam_DigiGain_Beacon_Rx },
            { ICValueTargetType.Param_DigiGain_Beacon_Tx,   ElanCommandType.SetParam_DigiGain_Beacon_Tx },
            { ICValueTargetType.Param_DigiGain_PTHF_Rx,     ElanCommandType.SetParam_DigiGain_PTHF_Rx },
            { ICValueTargetType.Param_DigiGain_PTHF_Tx,     ElanCommandType.SetParam_DigiGain_PTHF_Tx },
            { ICValueTargetType.Param_DigiGain_BHF_Rx,      ElanCommandType.SetParam_DigiGain_BHF_Rx },
            { ICValueTargetType.Param_DigiGain_BHF_Tx,      ElanCommandType.SetParam_DigiGain_BHF_Tx },
            { ICValueTargetType.Param_Pen_HI_HF_THD,        ElanCommandType.SetParam_Pen_HI_HF_THD }
        };

        public static Dictionary<ICValueTargetType, ElanCommandType> m_dictGetCmdMappingTable = new Dictionary<ICValueTargetType, ElanCommandType>()
        {
            { ICValueTargetType.NA,                         ElanCommandType.NA },
            { ICValueTargetType.Param_AP_pPeakThrdshold,    ElanCommandType.GetParam_AP_pPeakThrdshold },
            { ICValueTargetType.PH1,                        ElanCommandType.GetPH1 },
            { ICValueTargetType.PH2,                        ElanCommandType.GetPH2 },
            { ICValueTargetType.ReportNumber,               ElanCommandType.GetReportNumber },
            { ICValueTargetType.P0_TH,                      ElanCommandType.GetP0_TH },
            { ICValueTargetType.TRxS_Hover_TH_Rx,           ElanCommandType.GetTRxS_Hover_TH_Rx },
            { ICValueTargetType.TRxS_Hover_TH_Tx,           ElanCommandType.GetTRxS_Hover_TH_Tx },
            { ICValueTargetType.TRxS_Contact_TH_Rx,         ElanCommandType.GetTRxS_Contact_TH_Rx },
            { ICValueTargetType.TRxS_Contact_TH_Tx,         ElanCommandType.GetTRxS_Contact_TH_Tx },
            { ICValueTargetType.Hover_TH_Rx,                ElanCommandType.GetHover_TH_Rx },
            { ICValueTargetType.Hover_TH_Tx,                ElanCommandType.GetHover_TH_Tx },
            { ICValueTargetType.Contact_TH_Rx,              ElanCommandType.GetContact_TH_Rx },
            { ICValueTargetType.Contact_TH_Tx,              ElanCommandType.GetContact_TH_Tx },
            { ICValueTargetType.PTHF_Hover_TH_Rx,           ElanCommandType.GetPTHF_Hover_TH_Rx },
            { ICValueTargetType.PTHF_Hover_TH_Tx,           ElanCommandType.GetPTHF_Hover_TH_Tx },
            { ICValueTargetType.PTHF_Contact_TH_Rx,         ElanCommandType.GetPTHF_Contact_TH_Rx },
            { ICValueTargetType.PTHF_Contact_TH_Tx,         ElanCommandType.GetPTHF_Contact_TH_Tx },
            { ICValueTargetType.BHF_Hover_TH_Rx,            ElanCommandType.GetBHF_Hover_TH_Rx },
            { ICValueTargetType.BHF_Hover_TH_Tx,            ElanCommandType.GetBHF_Hover_TH_Tx },
            { ICValueTargetType.BHF_Contact_TH_Rx,          ElanCommandType.GetBHF_Contact_TH_Rx },
            { ICValueTargetType.BHF_Contact_TH_Tx,          ElanCommandType.GetBHF_Contact_TH_Tx },
            { ICValueTargetType.Edge_1Trc_SubPwr,           ElanCommandType.GetEdge_1Trc_SubPwr },
            { ICValueTargetType.Edge_2Trc_SubPwr,           ElanCommandType.GetEdge_2Trc_SubPwr },
            { ICValueTargetType.Edge_3Trc_SubPwr,           ElanCommandType.GetEdge_3Trc_SubPwr },
            { ICValueTargetType.Edge_4Trc_SubPwr,           ElanCommandType.GetEdge_4Trc_SubPwr },
            { ICValueTargetType.IQ_BSH_P,                   ElanCommandType.GetIQ_BSH_P },
            { ICValueTargetType.Pressure3BinsTH,            ElanCommandType.GetPressure3BinsTH },
            { ICValueTargetType.Press_3BinsPwr,             ElanCommandType.Get3BinsPwr },
            { ICValueTargetType.Param_DigiGain_P0,          ElanCommandType.GetParam_DigiGain_P0 },
            { ICValueTargetType.Param_DigiGain_Beacon_Rx,   ElanCommandType.GetParam_DigiGain_Beacon_Rx },
            { ICValueTargetType.Param_DigiGain_Beacon_Tx,   ElanCommandType.GetParam_DigiGain_Beacon_Tx },
            { ICValueTargetType.Param_DigiGain_PTHF_Rx,     ElanCommandType.GetParam_DigiGain_PTHF_Rx },
            { ICValueTargetType.Param_DigiGain_PTHF_Tx,     ElanCommandType.GetParam_DigiGain_PTHF_Tx },
            { ICValueTargetType.Param_DigiGain_BHF_Rx,      ElanCommandType.GetParam_DigiGain_BHF_Rx },
            { ICValueTargetType.Param_DigiGain_BHF_Tx,      ElanCommandType.GetParam_DigiGain_BHF_Tx },
            { ICValueTargetType.Param_Pen_HI_HF_THD,        ElanCommandType.GetParam_Pen_HI_HF_THD }
        };

        public static byte[] ConvertCommandTypeToByteArray(ElanCommandType eCommandType, int nParameterValue = 0)
        {
            byte[] byteCommand_Array;
            byte byteHighByte = (byte)((nParameterValue & 0xFF00) >> 8);
            byte byteLowByte = (byte)(nParameterValue & 0xFF);

            switch (eCommandType)
            {
                case ElanCommandType.NA:
                    byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    break;
                case ElanCommandType.ResetIC:
                    byteCommand_Array = new byte[] { 0x77, 0x77, 0x77, 0x77 };
                    break;
                case ElanCommandType.EnableICReport:
                    byteCommand_Array = new byte[] { 0x54, 0xCA, 0x00, 0x00 };
                    break;
                case ElanCommandType.DisableICReport:
                    byteCommand_Array = new byte[] { 0x54, 0xCA, 0x00, 0x01 };
                    break;
                case ElanCommandType.DisableFingerReport:
                    byteCommand_Array = new byte[] { 0x54, 0xCA, 0x00, 0x02 };
                    break;
                case ElanCommandType.GetParam_AP_pPeakThrdshold:
                    byteCommand_Array = new byte[] { 0x53, 0xBE, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetPH1:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x8B, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x1B, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetPH2:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x8C, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x1C, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetReportNumber:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x77, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x07, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetP0_TH:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x94, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x24, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetTRxS_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xDF, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x6F, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetTRxS_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xE0, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x70, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetTRxS_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xDD, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x6D, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetTRxS_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xDE, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x6E, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetHover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x97, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x27, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetHover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x98, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x28, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetContact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x95, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x25, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetContact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x96, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x26, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetPTHF_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x9C, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x2C, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetPTHF_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x9D, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x2D, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetPTHF_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x9A, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x2A, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetPTHF_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x9B, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x2B, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetBHF_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xA0, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x30, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetBHF_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xA1, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x31, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetBHF_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x9E, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x2E, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetBHF_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x9F, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x2F, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetEdge_1Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xB5, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x45, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetEdge_2Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xB6, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x46, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetEdge_3Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xB7, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x47, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetEdge_4Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xB8, 0x00, 0x01, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x48, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetIQ_BSH_P:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x78, 0x00, 0x00, 0x01 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x08, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetPressure3BinsTH:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0x99, 0x00, 0x00, 0x01 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x29, 0x00, 0x01 };
                    break;
                case ElanCommandType.Get3BinsPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x96, 0x16, 0xC8, 0x00, 0x00, 0x01 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x58, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetParam_DigiGain_P0:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x09, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetParam_DigiGain_Beacon_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x0A, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetParam_DigiGain_Beacon_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x85, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetParam_DigiGain_PTHF_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x86, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetParam_DigiGain_PTHF_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x87, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetParam_DigiGain_BHF_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x88, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetParam_DigiGain_BHF_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x89, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetParam_Pen_HI_HF_THD:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x63, 0x32, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetSNVersion:
                    byteCommand_Array = new byte[] { 0x53, 0xD3, 0x00, 0x01 };
                    break;
                case ElanCommandType.SetNoisePTHF:
                    byteCommand_Array = new byte[] { 0x54, 0xF0, 0x04, 0x01 };
                    break;
                case ElanCommandType.SetNoiseBHF:
                    byteCommand_Array = new byte[] { 0x54, 0xF0, 0x08, 0x01 };
                    break;
                case ElanCommandType.GetNoiseTRX_400us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0x8A, 0x01 };
                    break;
                case ElanCommandType.GetNoiseRX_400us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0x82, 0x01 };
                    break;
                case ElanCommandType.GetNoiseTX_400us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0x83, 0x01 };
                    break;
                case ElanCommandType.GetNoiseTRX_800us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0xCA, 0x01 };
                    break;
                case ElanCommandType.GetNoiseRX_800us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0xC2, 0x01 };
                    break;
                case ElanCommandType.GetNoiseTX_800us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0xC3, 0x01 };
                    break;
                case ElanCommandType.GetSyncTRxS:
                    //byteCommand_Array = new byte[] { 0x54, 0xE0, 0x80, 0x01 };
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0xC0, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncRX_400us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0x86, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncTX_400us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0x87, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncTRX_400us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0x96, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncRX_800us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0xC6, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncTX_800us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0xC7, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncTRX_800us:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0xD6, 0x01 };
                    break;
                case ElanCommandType.GetDigitalGain:
                    byteCommand_Array = new byte[] { 0x54, 0xF0, 0x80, 0x01 };
                    break;
                case ElanCommandType.SetCoordFF:
                    //byteCommand_Array = new byte[] { 0x54, 0xE0, 0xC0, 0x01 };
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0x80, 0x01 };
                    break;
                case ElanCommandType.GetTiltPTHF:
                    byteCommand_Array = new byte[] { 0x54, 0xF0, 0x01, 0x01 };
                    break;
                case ElanCommandType.GetTiltBHF:
                    byteCommand_Array = new byte[] { 0x54, 0xF0, 0x02, 0x01 };
                    break;
                case ElanCommandType.GetPressure:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0xA0, 0x01 };
                    break;
                case ElanCommandType.GetLinearity:
                    byteCommand_Array = new byte[] { 0x54, 0xF0, 0x20, 0x01 };
                    break;
                case ElanCommandType.Get5TRawData:
                    byteCommand_Array = new byte[] { 0x54, 0xF0, 0xA0, 0x01 };
                    break;
                case ElanCommandType.ResetTiltNoise:
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0x00, 0x01 };
                    break;
                case ElanCommandType.ResetTilt:
                    byteCommand_Array = new byte[] { 0x54, 0xF0, 0x00, 0x00 };
                    break;
                case ElanCommandType.StopNonSyncRXTX:
                    //byteCommand_Array = new byte[] { 0x54, 0xE0, 0x80, 0x01 };
                    byteCommand_Array = new byte[] { 0x54, 0xE0, 0xC0, 0x01 };
                    break;
                case ElanCommandType.SetParam_AP_pPeakThrdshold:
                    byteCommand_Array = new byte[] { 0x54, 0xBE, byteHighByte, byteLowByte };
                    break;
                //Command with one parameter
                case ElanCommandType.SetPH1:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x8B, 0x00, (byte)nParameterValue, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x1B, 0x00, (byte)nParameterValue };
                    break;
                case ElanCommandType.SetPH2:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x8C, 0x00, (byte)nParameterValue, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x1C, 0x00, (byte)nParameterValue };
                    break;
                //Command with two parameter (HighByte, LowByte)
                case ElanCommandType.SetReportNumber:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x77, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x07, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetP0_TH:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x94, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x24, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetTRxS_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xDF, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x6F, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetTRxS_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xE0, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x70, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetTRxS_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xDD, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x6D, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetTRxS_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xDE, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x6E, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetHover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x97, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x27, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetHover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x98, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x28, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetContact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x95, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x25, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetContact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x96, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x26, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetPTHF_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x9C, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x2C, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetPTHF_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x9D, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x2D, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetPTHF_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x9A, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x2A, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetPTHF_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x9B, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x2B, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetBHF_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xA0, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x30, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetBHF_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xA1, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x31, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetBHF_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x9E, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x2E, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetBHF_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x9F, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x2F, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetEdge_1Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xB5, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x45, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetEdge_2Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xB6, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x46, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetEdge_3Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xB7, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x47, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetEdge_4Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xB8, byteHighByte, byteLowByte, 0xF1 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x48, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetIQ_BSH_P:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x78, byteHighByte, byteLowByte, 0x01 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x08, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetPressure3BinsTH:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0x99, byteHighByte, byteLowByte, 0x01 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x29, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.Set3BinsPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x97, 0x16, 0xC8, byteHighByte, byteLowByte, 0x01 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x58, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetParam_DigiGain_P0:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, byteHighByte, byteLowByte, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x09, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetParam_DigiGain_Beacon_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, byteHighByte, byteLowByte, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x0A, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetParam_DigiGain_Beacon_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, byteHighByte, byteLowByte, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x85, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetParam_DigiGain_PTHF_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, byteHighByte, byteLowByte, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x86, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetParam_DigiGain_PTHF_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, byteHighByte, byteLowByte, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x87, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetParam_DigiGain_BHF_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, byteHighByte, byteLowByte, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x88, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetParam_DigiGain_BHF_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, byteHighByte, byteLowByte, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x89, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetParam_Pen_HI_HF_THD:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        byteCommand_Array = new byte[] { 0x64, 0x32, byteHighByte, byteLowByte };
                    break;
                case ElanCommandType.SetRead_Bulk_RAM_Data:
                    byteCommand_Array = new byte[] { 0x54, 0xBC, 0x00, 0x01 };
                    break;
                default:
                    byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    break;
            }

            return byteCommand_Array;
        }

        public static int ConvertGetBufferToGetValue(ElanCommandType eCommandType, byte[] byteGetBuffer_Array)
        {
            int nGetValue = -1;
            int nCheckByteNumber = 2;
            int nValueByteNumber = 2;

            byte[] byteCheckByte_Array = new byte[2];

            if (ParamAutoTuning.m_nFWTypeIndex == 1)
            {
                nCheckByteNumber = 3;
                byteCheckByte_Array = new byte[3];
            }

            Array.Clear(byteCheckByte_Array, 0, byteCheckByte_Array.Length - 1);

            byteCheckByte_Array[0] = 0x65;

            if (ParamAutoTuning.m_nFWTypeIndex == 1)
            {
                byteCheckByte_Array[0] = 0x95;
                byteCheckByte_Array[1] = 0x16;
            }

            switch (eCommandType)
            {
                case ElanCommandType.GetParam_AP_pPeakThrdshold:
                    nCheckByteNumber = 2;
                    byteCheckByte_Array = new byte[2];

                    byteCheckByte_Array[0] = 0x52;
                    byteCheckByte_Array[1] = 0xBE;
                    break;
                case ElanCommandType.GetPH1:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x8B;
                    else
                        byteCheckByte_Array[1] = 0x1B;
                    break;
                case ElanCommandType.GetPH2:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x8C;
                    else
                        byteCheckByte_Array[1] = 0x1C;
                    break;
                case ElanCommandType.GetReportNumber:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x77;
                    else
                        byteCheckByte_Array[1] = 0x07;
                    break;
                case ElanCommandType.GetP0_TH:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x94;
                    else
                        byteCheckByte_Array[1] = 0x24;
                    break;
                case ElanCommandType.GetTRxS_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xDF;
                    else
                        byteCheckByte_Array[1] = 0x6F;
                    break;
                case ElanCommandType.GetTRxS_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xE0;
                    else
                        byteCheckByte_Array[1] = 0x70;
                    break;
                case ElanCommandType.GetTRxS_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xDD;
                    else
                        byteCheckByte_Array[1] = 0x6D;
                    break;
                case ElanCommandType.GetTRxS_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xDE;
                    else
                        byteCheckByte_Array[1] = 0x6E;
                    break;
                case ElanCommandType.GetHover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x97;
                    else
                        byteCheckByte_Array[1] = 0x27;
                    break;
                case ElanCommandType.GetHover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x98;
                    else
                        byteCheckByte_Array[1] = 0x28;
                    break;
                case ElanCommandType.GetContact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x95;
                    else
                        byteCheckByte_Array[1] = 0x25;
                    break;
                case ElanCommandType.GetContact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x96;
                    else
                        byteCheckByte_Array[1] = 0x26;
                    break;
                case ElanCommandType.GetPTHF_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9C;
                    else
                        byteCheckByte_Array[1] = 0x2C;
                    break;
                case ElanCommandType.GetPTHF_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9D;
                    else
                        byteCheckByte_Array[1] = 0x2D;
                    break;
                case ElanCommandType.GetPTHF_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9A;
                    else
                        byteCheckByte_Array[1] = 0x2A;
                    break;
                case ElanCommandType.GetPTHF_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9B;
                    else
                        byteCheckByte_Array[1] = 0x2B;
                    break;
                case ElanCommandType.GetBHF_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xA0;
                    else
                        byteCheckByte_Array[1] = 0x30;
                    break;
                case ElanCommandType.GetBHF_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xA1;
                    else
                        byteCheckByte_Array[1] = 0x31;
                    break;
                case ElanCommandType.GetBHF_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9E;
                    else
                        byteCheckByte_Array[1] = 0x2E;
                    break;
                case ElanCommandType.GetBHF_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9F;
                    else
                        byteCheckByte_Array[1] = 0x2F;
                    break;
                case ElanCommandType.GetEdge_1Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xB5;
                    else
                        byteCheckByte_Array[1] = 0x45;
                    break;
                case ElanCommandType.GetEdge_2Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xB6;
                    else
                        byteCheckByte_Array[1] = 0x46;
                    break;
                case ElanCommandType.GetEdge_3Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xB7;
                    else
                        byteCheckByte_Array[1] = 0x47;
                    break;
                case ElanCommandType.GetEdge_4Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xB8;
                    else
                        byteCheckByte_Array[1] = 0x48;
                    break;
                case ElanCommandType.GetIQ_BSH_P:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x78;
                    else
                        byteCheckByte_Array[1] = 0x08;
                    break;
                case ElanCommandType.GetPressure3BinsTH:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x99;
                    else
                        byteCheckByte_Array[1] = 0x29;
                    break;
                case ElanCommandType.Get3BinsPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xC8;
                    else
                        byteCheckByte_Array[1] = 0x58;
                    break;
                case ElanCommandType.GetParam_DigiGain_P0:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x09;
                    break;
                case ElanCommandType.GetParam_DigiGain_Beacon_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x0A;
                    break;
                case ElanCommandType.GetParam_DigiGain_Beacon_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x85;
                    break;
                case ElanCommandType.GetParam_DigiGain_PTHF_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x86;
                    break;
                case ElanCommandType.GetParam_DigiGain_PTHF_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x87;
                    break;
                case ElanCommandType.GetParam_DigiGain_BHF_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x88;
                    break;
                case ElanCommandType.GetParam_DigiGain_BHF_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x89;
                    break;
                case ElanCommandType.GetParam_Pen_HI_HF_THD:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x32;
                    break;
                case ElanCommandType.GetSNVersion:
                    byteCheckByte_Array[0] = 0x52;
                    byteCheckByte_Array[1] = 0xD3;
                    nCheckByteNumber = 2;
                    nValueByteNumber = 1;
                    break;
                default:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                    {
                        byteCheckByte_Array[0] = 0x00;
                        byteCheckByte_Array[1] = 0x00;
                        byteCheckByte_Array[2] = 0x00;
                    }
                    else
                    {
                        byteCheckByte_Array[0] = 0x00;
                        byteCheckByte_Array[1] = 0x00;
                    }
                    break;
            }

            for (int nByteIndex = 0; nByteIndex < nCheckByteNumber; nByteIndex++)
            {
                if (byteGetBuffer_Array[nByteIndex] != byteCheckByte_Array[nByteIndex])
                    return nGetValue;
            }

            if (nValueByteNumber == 1)
                nGetValue = (ushort)byteGetBuffer_Array[3];
            else if (nValueByteNumber == 2)
            {
                if (eCommandType == ElanCommandType.GetParam_AP_pPeakThrdshold)
                    nGetValue = (ushort)(byteGetBuffer_Array[2] << 8 | byteGetBuffer_Array[3]);
                else if (ParamAutoTuning.m_nFWTypeIndex == 1)
                    nGetValue = (ushort)(byteGetBuffer_Array[3] << 8 | byteGetBuffer_Array[4]);
                else
                    nGetValue = (ushort)(byteGetBuffer_Array[2] << 8 | byteGetBuffer_Array[3]);
            }

            return nGetValue;
        }
    }
}
