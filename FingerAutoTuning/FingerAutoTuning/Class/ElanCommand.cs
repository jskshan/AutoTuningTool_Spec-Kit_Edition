using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FingerAutoTuningParameter;

namespace Elan
{
    public class ElanCommand
    {
        /// <summary>
        /// The data length that read from TP
        /// </summary>
        public static int m_nIN_DATA_LENGTH = 65;

        public enum ElanCommandType
        {
            // command without parameter
            NA,
            ResetIC,
            EnableICReport,
            DisableICReport,
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
            SetOverTHType,
            SetCoordFF,
            GetTiltPTHF,
            GetTiltBHF,
            GetPressure,
            GetLinearity,
            ResetTiltNoise,
            ResetTilt,
            StopNonSyncRXTX,
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
            Set3BinsPwr
        }

        public enum ICValueTargetType
        {
            NA,
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
            SNVersion
        }

        public static Dictionary<ICValueTargetType, ElanCommandType> dictSetCommandMappingTable = new Dictionary<ICValueTargetType, ElanCommandType>()
        {
            {ICValueTargetType.NA,                  ElanCommandType.NA},
            {ICValueTargetType.PH1,                 ElanCommandType.SetPH1},
            {ICValueTargetType.PH2,                 ElanCommandType.SetPH2},
            {ICValueTargetType.ReportNumber,        ElanCommandType.SetReportNumber},
            {ICValueTargetType.P0_TH,               ElanCommandType.SetP0_TH},
            {ICValueTargetType.TRxS_Hover_TH_Rx,    ElanCommandType.SetTRxS_Hover_TH_Rx},
            {ICValueTargetType.TRxS_Hover_TH_Tx,    ElanCommandType.SetTRxS_Hover_TH_Tx},
            {ICValueTargetType.TRxS_Contact_TH_Rx,  ElanCommandType.SetTRxS_Contact_TH_Rx},
            {ICValueTargetType.TRxS_Contact_TH_Tx,  ElanCommandType.SetTRxS_Contact_TH_Tx},
            {ICValueTargetType.Hover_TH_Rx,         ElanCommandType.SetHover_TH_Rx},
            {ICValueTargetType.Hover_TH_Tx,         ElanCommandType.SetHover_TH_Tx},
            {ICValueTargetType.Contact_TH_Rx,       ElanCommandType.SetContact_TH_Rx},
            {ICValueTargetType.Contact_TH_Tx,       ElanCommandType.SetContact_TH_Tx},
            {ICValueTargetType.PTHF_Hover_TH_Rx,    ElanCommandType.SetPTHF_Hover_TH_Rx},
            {ICValueTargetType.PTHF_Hover_TH_Tx,    ElanCommandType.SetPTHF_Hover_TH_Tx},
            {ICValueTargetType.PTHF_Contact_TH_Rx,  ElanCommandType.SetPTHF_Contact_TH_Rx},
            {ICValueTargetType.PTHF_Contact_TH_Tx,  ElanCommandType.SetPTHF_Contact_TH_Tx},
            {ICValueTargetType.BHF_Hover_TH_Rx,     ElanCommandType.SetBHF_Hover_TH_Rx},
            {ICValueTargetType.BHF_Hover_TH_Tx,     ElanCommandType.SetBHF_Hover_TH_Tx},
            {ICValueTargetType.BHF_Contact_TH_Rx,   ElanCommandType.SetBHF_Contact_TH_Rx},
            {ICValueTargetType.BHF_Contact_TH_Tx,   ElanCommandType.SetBHF_Contact_TH_Tx},
            {ICValueTargetType.Edge_1Trc_SubPwr,    ElanCommandType.SetEdge_1Trc_SubPwr},
            {ICValueTargetType.Edge_2Trc_SubPwr,    ElanCommandType.SetEdge_2Trc_SubPwr},
            {ICValueTargetType.Edge_3Trc_SubPwr,    ElanCommandType.SetEdge_3Trc_SubPwr},
            {ICValueTargetType.Edge_4Trc_SubPwr,    ElanCommandType.SetEdge_4Trc_SubPwr},
            {ICValueTargetType.IQ_BSH_P,            ElanCommandType.SetIQ_BSH_P},
            {ICValueTargetType.Pressure3BinsTH,     ElanCommandType.SetPressure3BinsTH},
            {ICValueTargetType.Press_3BinsPwr,      ElanCommandType.Set3BinsPwr}
        };

        public static Dictionary<ICValueTargetType, ElanCommandType> dictGetCommandMappingTable = new Dictionary<ICValueTargetType, ElanCommandType>()
        {
            {ICValueTargetType.NA,                  ElanCommandType.NA},
            {ICValueTargetType.PH1,                 ElanCommandType.GetPH1},
            {ICValueTargetType.PH2,                 ElanCommandType.GetPH2},
            {ICValueTargetType.ReportNumber,        ElanCommandType.GetReportNumber},
            {ICValueTargetType.P0_TH,               ElanCommandType.GetP0_TH},
            {ICValueTargetType.TRxS_Hover_TH_Rx,    ElanCommandType.GetTRxS_Hover_TH_Rx},
            {ICValueTargetType.TRxS_Hover_TH_Tx,    ElanCommandType.GetTRxS_Hover_TH_Tx},
            {ICValueTargetType.TRxS_Contact_TH_Rx,  ElanCommandType.GetTRxS_Contact_TH_Rx},
            {ICValueTargetType.TRxS_Contact_TH_Tx,  ElanCommandType.GetTRxS_Contact_TH_Tx},
            {ICValueTargetType.Hover_TH_Rx,         ElanCommandType.GetHover_TH_Rx},
            {ICValueTargetType.Hover_TH_Tx,         ElanCommandType.GetHover_TH_Tx},
            {ICValueTargetType.Contact_TH_Rx,       ElanCommandType.GetContact_TH_Rx},
            {ICValueTargetType.Contact_TH_Tx,       ElanCommandType.GetContact_TH_Tx},
            {ICValueTargetType.PTHF_Hover_TH_Rx,    ElanCommandType.GetPTHF_Hover_TH_Rx},
            {ICValueTargetType.PTHF_Hover_TH_Tx,    ElanCommandType.GetPTHF_Hover_TH_Tx},
            {ICValueTargetType.PTHF_Contact_TH_Rx,  ElanCommandType.GetPTHF_Contact_TH_Rx},
            {ICValueTargetType.PTHF_Contact_TH_Tx,  ElanCommandType.GetPTHF_Contact_TH_Tx},
            {ICValueTargetType.BHF_Hover_TH_Rx,     ElanCommandType.GetBHF_Hover_TH_Rx},
            {ICValueTargetType.BHF_Hover_TH_Tx,     ElanCommandType.GetBHF_Hover_TH_Tx},
            {ICValueTargetType.BHF_Contact_TH_Rx,   ElanCommandType.GetBHF_Contact_TH_Rx},
            {ICValueTargetType.BHF_Contact_TH_Tx,   ElanCommandType.GetBHF_Contact_TH_Tx},
            {ICValueTargetType.Edge_1Trc_SubPwr,    ElanCommandType.GetEdge_1Trc_SubPwr},
            {ICValueTargetType.Edge_2Trc_SubPwr,    ElanCommandType.GetEdge_2Trc_SubPwr},
            {ICValueTargetType.Edge_3Trc_SubPwr,    ElanCommandType.GetEdge_3Trc_SubPwr},
            {ICValueTargetType.Edge_4Trc_SubPwr,    ElanCommandType.GetEdge_4Trc_SubPwr},
            {ICValueTargetType.IQ_BSH_P,            ElanCommandType.GetIQ_BSH_P},
            {ICValueTargetType.Pressure3BinsTH,     ElanCommandType.GetPressure3BinsTH},
            {ICValueTargetType.Press_3BinsPwr,      ElanCommandType.Get3BinsPwr},
            {ICValueTargetType.SNVersion,           ElanCommandType.GetSNVersion}
        };

        public static byte[] ConvertCommandToByte(ElanCommandType eCommandType, int nParameter = 0)
        {
            byte[] byteCommand_Array;
            byte byteHighByte = (byte)((nParameter & 0xFF00) >> 8);
            byte byteLowByte = (byte)(nParameter & 0xFF);

            switch (eCommandType)
            {
                case ElanCommandType.NA:
                    byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    break;
                case ElanCommandType.ResetIC:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x77, 0x77, 0x77, 0x77 };
                    break;
                case ElanCommandType.EnableICReport:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xCA, 0x00, 0x00 };
                    break;
                case ElanCommandType.DisableICReport:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xCA, 0x00, 0x01 };
                    break;
                case ElanCommandType.GetSNVersion:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x53, 0xD3, 0x00, 0x01 };
                    break;
                case ElanCommandType.SetNoisePTHF:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xF0, 0x04, 0x01 };
                    break;
                case ElanCommandType.SetNoiseBHF:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xF0, 0x08, 0x01 };
                    break;
                case ElanCommandType.GetNoiseTRX_400us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0x8A, 0x01 };
                    break;
                case ElanCommandType.GetNoiseRX_400us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0x82, 0x01 };
                    break;
                case ElanCommandType.GetNoiseTX_400us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0x83, 0x01 };
                    break;
                case ElanCommandType.GetNoiseTRX_800us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0xCA, 0x01 };
                    break;
                case ElanCommandType.GetNoiseRX_800us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0xC2, 0x01 };
                    break;
                case ElanCommandType.GetNoiseTX_800us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0xC3, 0x01 };
                    break;
                case ElanCommandType.GetSyncTRxS:
                    //byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0x80, 0x01 };
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0xC0, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncRX_400us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0x86, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncTX_400us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0x87, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncTRX_400us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0x96, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncRX_800us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0xC6, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncTX_800us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0xC7, 0x01 };
                    break;
                case ElanCommandType.GetNonSyncTRX_800us:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0xD6, 0x01 };
                    break;
                case ElanCommandType.SetOverTHType:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xF0, 0x40, 0x01 };
                    break;
                case ElanCommandType.SetCoordFF:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0xC0, 0x01 };
                    break;
                case ElanCommandType.GetTiltPTHF:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xF0, 0x01, 0x01 };
                    break;
                case ElanCommandType.GetTiltBHF:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xF0, 0x02, 0x01 };
                    break;
                case ElanCommandType.GetPressure:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0xA0, 0x01 };
                    break;
                case ElanCommandType.GetLinearity:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xF0, 0x20, 0x01 };
                    break;
                case ElanCommandType.ResetTiltNoise:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0x00, 0x01 };
                    break;
                case ElanCommandType.ResetTilt:
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xF0, 0x00, 0x00 };
                    break;
                case ElanCommandType.StopNonSyncRXTX:
                    //byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0x80, 0x01 };
                    byteCommand_Array = new byte[] { 0x00, 0x04, 0x54, 0xE0, 0xC0, 0x01 };
                    break;
                default:
                    byteCommand_Array = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    break;
            }

            return byteCommand_Array;
        }

        public static int ConvertToICGetValue(ElanCommandType eCommandType, byte[] byteGetBuffer_Array)
        {
            int nGetValue = -1;
            int nCheckByteNumber = 2;
            int nValueByteNumber = 2;

            byte[] byteCheckByte_Array = new byte[2];

            for (int nIndex = 0; nIndex < nCheckByteNumber; nIndex++)
            {
                if (byteGetBuffer_Array[nIndex] != byteCheckByte_Array[nIndex])
                    return nGetValue;
            }

            if (nValueByteNumber == 1)
                nGetValue = (ushort)byteGetBuffer_Array[3];
            else if (nValueByteNumber == 2)
            {
                /*
                if (ParamFingerAutoTuning.m_nFWTypeIndex == 1)
                    nGetValue = (ushort)(byteGetBuffer_Array[3] << 8 | byteGetBuffer_Array[4]);
                else
                    nGetValue = (ushort)(byteGetBuffer_Array[2] << 8 | byteGetBuffer_Array[3]);
                */
            }

            return nGetValue;
        }
    }
}
