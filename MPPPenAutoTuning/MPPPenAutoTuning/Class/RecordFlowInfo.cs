using System.Collections.Generic;

namespace MPPPenAutoTuning
{
    public class RecordFlowInfo
    {
        public static List<RecordParameter> m_cElseParameter_List = new List<RecordParameter>();

        public static List<NoiseParameter> m_cNoiseParameter_List = new List<NoiseParameter>();

        public static List<TiltNoiseParameter> m_cTNPTHFParameter_List = new List<TiltNoiseParameter>();
        public static List<TiltNoiseParameter> m_cTNBHFParameter_List = new List<TiltNoiseParameter>();

        public static List<DigiGainTuningParameter> m_cDGTParameter_List = new List<DigiGainTuningParameter>();

        public static List<TPGainTuningParameter> m_cTPGTParameter_List = new List<TPGainTuningParameter>();

        public static List<PeakCheckTuningParameter> m_cPCTH1stParameter_List = new List<PeakCheckTuningParameter>();
        public static List<PeakCheckTuningParameter> m_cPCTH2ndParameter_List = new List<PeakCheckTuningParameter>();
        public static List<PeakCheckTuningParameter> m_cPCTCParameter_List = new List<PeakCheckTuningParameter>();

        public static List<DTNormalParameter> m_cDTH1stParameter_List = new List<DTNormalParameter>();
        public static List<DTNormalParameter> m_cDTH2ndParameter_List = new List<DTNormalParameter>();
        public static List<DTNormalParameter> m_cDTCParameter_List = new List<DTNormalParameter>();

        public static List<DTTRxSParameter> m_cDTHTRxSParameter_List = new List<DTTRxSParameter>();
        public static List<DTTRxSParameter> m_cDTCTRxSParameter_List = new List<DTTRxSParameter>();

        public static List<TiltTuningParameter> m_cTTPTHFParameter_List = new List<TiltTuningParameter>();
        public static List<TiltTuningParameter> m_cTTBHFParameter_List = new List<TiltTuningParameter>();

        public static List<PressureTuningParameter> m_cPProtectParameter_List = new List<PressureTuningParameter>();
        public static List<PressureTuningParameter> m_cPSettingParameter_List = new List<PressureTuningParameter>();
        public static List<PressureTuningParameter> m_cPTableParameter_List = new List<PressureTuningParameter>();

        public static List<LinearityTuningParameter> m_cLTuningParameter_List = new List<LinearityTuningParameter>();

        public static void Initialize(SubTuningStep eSubStep)
        {
            switch (eSubStep)
            {
                case SubTuningStep.NO:
                    m_cNoiseParameter_List = new List<NoiseParameter>();
                    break;
                case SubTuningStep.TILTNO_PTHF:
                    m_cTNPTHFParameter_List = new List<TiltNoiseParameter>();
                    break;
                case SubTuningStep.TILTNO_BHF:
                    m_cTNBHFParameter_List = new List<TiltNoiseParameter>();
                    break;
                case SubTuningStep.DIGIGAIN:
                    m_cDGTParameter_List = new List<DigiGainTuningParameter>();
                    break;
                case SubTuningStep.TP_GAIN:
                    m_cTPGTParameter_List = new List<TPGainTuningParameter>();
                    break;
                case SubTuningStep.PCHOVER_1ST:
                    m_cPCTH1stParameter_List = new List<PeakCheckTuningParameter>();
                    break;
                case SubTuningStep.PCHOVER_2ND:
                    m_cPCTH2ndParameter_List = new List<PeakCheckTuningParameter>();
                    break;
                case SubTuningStep.PCCONTACT:
                    m_cPCTCParameter_List = new List<PeakCheckTuningParameter>();
                    break;
                case SubTuningStep.HOVER_1ST:
                    m_cDTH1stParameter_List = new List<DTNormalParameter>();
                    break;
                case SubTuningStep.HOVER_2ND:
                    m_cDTH2ndParameter_List = new List<DTNormalParameter>();
                    break;
                case SubTuningStep.CONTACT:
                    m_cDTCParameter_List = new List<DTNormalParameter>();
                    break;
                case SubTuningStep.HOVERTRxS:
                    m_cDTHTRxSParameter_List = new List<DTTRxSParameter>();
                    break;
                case SubTuningStep.CONTACTTRxS:
                    m_cDTCTRxSParameter_List = new List<DTTRxSParameter>();
                    break;
                case SubTuningStep.TILTTUNING_PTHF:
                    m_cTTPTHFParameter_List = new List<TiltTuningParameter>();
                    break;
                case SubTuningStep.TILTTUNING_BHF:
                    m_cTTBHFParameter_List = new List<TiltTuningParameter>();
                    break;
                case SubTuningStep.PRESSUREPROTECT:
                    m_cPProtectParameter_List = new List<PressureTuningParameter>();
                    break;
                case SubTuningStep.PRESSURESETTING:
                    m_cPSettingParameter_List = new List<PressureTuningParameter>();
                    break;
                case SubTuningStep.PRESSURETABLE:
                    m_cPTableParameter_List = new List<PressureTuningParameter>();
                    break;
                case SubTuningStep.LINEARITYTABLE:
                    m_cLTuningParameter_List = new List<LinearityTuningParameter>();
                    break;
            }
        }
    }

    public class RecordParameter
    {
        public int m_nRankIndex = 0;
        public int m_nPH1 = 8;
        public int m_nPH2 = 20;
        public string m_sErrorCode = "";
        public string m_sErrorMessage = "";

        public void SetBasicInfo(int nRankIndex, int nPH1, int nPH2)
        {
            m_nRankIndex = nRankIndex;
            m_nPH1 = nPH1;
            m_nPH2 = nPH2;
        }

        public virtual void SetIntParameter(string sParameterName, int nParameterValue, SubTuningStep eSubStep)
        {
        }

        public virtual void SetStringParameter(string sParameterName, string sParameterValue, SubTuningStep eSubStep)
        {
        }

        public virtual void SetDoubleParameter(string sParameterName, double dParameterValue, SubTuningStep eSubStep)
        {
        }

        public void SetErrorInformation(string sParameterName, string sParameterValue)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sErrorCode:
                    m_sErrorCode = sParameterValue;
                    break;
                case SpecificText.m_sErrorMsg:
                case SpecificText.m_sErrorMessage:
                    m_sErrorMessage = sParameterValue;
                    break;
                default:
                    break;
            }
        }
    }

    public class NoiseParameter : RecordParameter
    {
    }

    public class TiltNoiseParameter : RecordParameter
    {
        public int m_nTN_PTHFRXTotalMax = -1;
        public int m_nTN_PTHFTXTotalMax = -1;

        public override void SetIntParameter(string sParameterName, int nParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sRXTotalMax:
                    m_nTN_PTHFRXTotalMax = nParameterValue;
                    break;
                case SpecificText.m_sTXTotalMax:
                    m_nTN_PTHFTXTotalMax = nParameterValue;
                    break;
                default:
                    break;
            }
        }
    }

    public class DigiGainTuningParameter : RecordParameter
    {
        public int m_nNoiseP0_Detect_Time_Idx = -1;
        public int m_nDTHoverP0_Detect_Time_Idx = -1;

        public int m_nNoiseRXInnerMax = -1;
        public int m_nNoiseTXInnerMax = -1;

        public int m_nNoiseRXInMaxPlus3InMaxSTD = -1;
        public int m_nNoiseTXInMaxPlus3InMaxSTD = -1;

        public int m_nNoiseTrcMaxMinusPreP0_TH_1Trc = -1;
        public int m_nNoiseTrcMaxMinusPreP0_TH_2Trc = -1;
        public int m_nNoiseTrcMaxMinusPreP0_TH_3Trc = -1;
        public int m_nNoiseTrcMaxMinusPreP0_TH_4Trc = -1;

        public override void SetIntParameter(string sParameterName, int nParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sRXInnerMax:
                    m_nNoiseRXInnerMax = nParameterValue;
                    break;
                case SpecificText.m_sTXInnerMax:
                    m_nNoiseTXInnerMax = nParameterValue;
                    break;
                case SpecificText.m_sRXInnerMaxPlus3InnerMaxSTD:
                    m_nNoiseRXInMaxPlus3InMaxSTD = nParameterValue;
                    break;
                case SpecificText.m_sTXInnerMaxPlus3InnerMaxSTD:
                    m_nNoiseTXInMaxPlus3InMaxSTD = nParameterValue;
                    break;
                case SpecificText.m_sTrcMaxMinusPreP0_TH_1Trc:
                    m_nNoiseTrcMaxMinusPreP0_TH_1Trc = nParameterValue;
                    break;
                case SpecificText.m_sTrcMaxMinusPreP0_TH_2Trc:
                    m_nNoiseTrcMaxMinusPreP0_TH_2Trc = nParameterValue;
                    break;
                case SpecificText.m_sTrcMaxMinusPreP0_TH_3Trc:
                    m_nNoiseTrcMaxMinusPreP0_TH_3Trc = nParameterValue;
                    break;
                case SpecificText.m_sTrcMaxMinusPreP0_TH_4Trc:
                    m_nNoiseTrcMaxMinusPreP0_TH_4Trc = nParameterValue;
                    break;
                default:
                    break;
            }
        }

        public override void SetStringParameter(string sParameterName, string sParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sP0_Detect_Time:
                    if (sParameterValue == SpecificText.m_sP0_Detect_Time_400)
                    {
                        if (eSubStep == SubTuningStep.NO)
                            m_nNoiseP0_Detect_Time_Idx = 0;
                        else
                            m_nDTHoverP0_Detect_Time_Idx = 0;
                    }
                    else if (sParameterValue == SpecificText.m_sP0_Detect_Time_800)
                    {
                        if (eSubStep == SubTuningStep.NO)
                            m_nNoiseP0_Detect_Time_Idx = 1;
                        else
                            m_nDTHoverP0_Detect_Time_Idx = 1;
                    }
                    else
                    {
                        if (eSubStep == SubTuningStep.NO)
                            m_nNoiseP0_Detect_Time_Idx = -1;
                        else
                            m_nDTHoverP0_Detect_Time_Idx = -1;
                    }

                    break;
                default:
                    break;
            }
        }
    }

    public class TPGainTuningParameter : RecordParameter
    {
        public override void SetIntParameter(string sParameterName, int nParameterValue, SubTuningStep eSubStep)
        {
        }
    }

    public class PeakCheckTuningParameter : RecordParameter
    {
        public int m_nNoiseRXInnerMax = -1;

        public int m_nNoiseDigiGain_Beacon_Rx = -1;

        public int m_nPenPeak_1Traces_Th_PCH1st = -1;
        public int m_nPenPeak_2Traces_Th_PCH1st = -1;
        public int m_nPenPeakWidth_Th_PCH1st = -1;
        public int m_nPenPeak_4Traces_Th_PCH1st = -1;
        public int m_nPenPeak_5Traces_Th_PCH1st = -1;
        public int m_nPenPeak_5Traces_PeakPwr_Th_PCH1st = -1;
        public int m_nPenPeak_Th_PCH1st = -1;
        public int m_nPenPeakCheck_AreaUP_Pwr_TH_PCH1st = -1;

        public int m_nPenPeak_1Traces_Th_PCH2nd = -1;
        public int m_nPenPeak_2Traces_Th_PCH2nd = -1;
        public int m_nPenPeakWidth_Th_PCH2nd = -1;
        public int m_nPenPeak_4Traces_Th_PCH2nd = -1;
        public int m_nPenPeak_5Traces_Th_PCH2nd = -1;
        public int m_nPenPeak_5Traces_PeakPwr_Th_PCH2nd = -1;
        public int m_nPenPeak_Th_PCH2nd = -1;
        public int m_nPenPeakCheck_AreaUP_Pwr_TH_PCH2nd = -1;

        public override void SetIntParameter(string sParameterName, int nParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sRXInnerMax:
                    m_nNoiseRXInnerMax = nParameterValue;
                    break;
                case SpecificText.m_sPenPeak_1Traces_Th:
                    if (eSubStep == SubTuningStep.PCHOVER_1ST)
                        m_nPenPeak_1Traces_Th_PCH1st = nParameterValue;
                    else if (eSubStep == SubTuningStep.PCHOVER_2ND)
                        m_nPenPeak_1Traces_Th_PCH2nd = nParameterValue;

                    break;
                case SpecificText.m_sPenPeak_2Traces_Th:
                    if (eSubStep == SubTuningStep.PCHOVER_1ST)
                        m_nPenPeak_2Traces_Th_PCH1st = nParameterValue;
                    else if (eSubStep == SubTuningStep.PCHOVER_2ND)
                        m_nPenPeak_2Traces_Th_PCH2nd = nParameterValue;

                    break;
                case SpecificText.m_sPenPeakWidth_Th:
                    if (eSubStep == SubTuningStep.PCHOVER_1ST)
                        m_nPenPeakWidth_Th_PCH1st = nParameterValue;
                    else if (eSubStep == SubTuningStep.PCHOVER_2ND)
                        m_nPenPeakWidth_Th_PCH2nd = nParameterValue;

                    break;
                case SpecificText.m_sPenPeak_4Traces_Th:
                    if (eSubStep == SubTuningStep.PCHOVER_1ST)
                        m_nPenPeak_4Traces_Th_PCH1st = nParameterValue;
                    else if (eSubStep == SubTuningStep.PCHOVER_2ND)
                        m_nPenPeak_4Traces_Th_PCH2nd = nParameterValue;

                    break;
                case SpecificText.m_sPenPeak_5Traces_Th:
                    if (eSubStep == SubTuningStep.PCHOVER_1ST)
                        m_nPenPeak_5Traces_Th_PCH1st = nParameterValue;
                    else if (eSubStep == SubTuningStep.PCHOVER_2ND)
                        m_nPenPeak_5Traces_Th_PCH2nd = nParameterValue;

                    break;
                case SpecificText.m_sPenPeak_5Traces_PeakPwr_Th:
                    if (eSubStep == SubTuningStep.PCHOVER_1ST)
                        m_nPenPeak_5Traces_PeakPwr_Th_PCH1st = nParameterValue;
                    else if (eSubStep == SubTuningStep.PCHOVER_2ND)
                        m_nPenPeak_5Traces_PeakPwr_Th_PCH2nd = nParameterValue;

                    break;
                case SpecificText.m_sPenPeak_Th:
                    if (eSubStep == SubTuningStep.PCHOVER_1ST)
                        m_nPenPeak_Th_PCH1st = nParameterValue;
                    else if (eSubStep == SubTuningStep.PCHOVER_2ND)
                        m_nPenPeak_Th_PCH2nd = nParameterValue;

                    break;
                case SpecificText.m_sPenPeakCheck_AreaUP_Pwr_TH:
                    if (eSubStep == SubTuningStep.PCHOVER_1ST)
                        m_nPenPeakCheck_AreaUP_Pwr_TH_PCH1st = nParameterValue;
                    else if (eSubStep == SubTuningStep.PCHOVER_2ND)
                        m_nPenPeakCheck_AreaUP_Pwr_TH_PCH2nd = nParameterValue;

                    break;
                case SpecificText.m_scActivePen_DigiGain_Beacon_Rx:
                    m_nNoiseDigiGain_Beacon_Rx = nParameterValue;
                    break;
                default:
                    break;
            }
        }
    }

    public class DTNormalParameter : RecordParameter
    {
        public int m_nNoiseP0_Detect_Time_Idx = -1;
        public int m_nDTHoverP0_Detect_Time_Idx = -1;

        public int m_nNoiseRXInnerMax = -1;
        public int m_nNoiseTXInnerMax = -1;

        public int m_nNoiseRXInMaxPlus3InMaxSTD = -1;

        public int m_nNoiseDigiGain_P0 = -1;
        public int m_nNoiseDigiGain_Beacon_Rx = -1;
        public int m_nNoiseDigiGain_Beacon_Tx = -1;

        public int m_nNoiseTrcMaxMinusPreP0_TH_1Trc = -1;
        public int m_nNoiseTrcMaxMinusPreP0_TH_2Trc = -1;
        public int m_nNoiseTrcMaxMinusPreP0_TH_3Trc = -1;
        public int m_nNoiseTrcMaxMinusPreP0_TH_4Trc = -1;

        public int m_nHover_1stRXTotalMedian = -1;
        public int m_nHover_1stTXTotalMedian = -1;
        public int m_nHover_1stRXTotalMax = -1;
        public int m_nHover_1stTXTotalMax = -1;

        public int m_nHover_1stRXPreTh = -1;
        public int m_nHover_1stTXPreTh = -1;
        public int m_nHover_1stRXPreTRxSTh = -1;
        public int m_nHover_1stTXPreTRxSTh = -1;
        public int m_nHover_2ndRXPreTh = -1;
        public int m_nHover_2ndTXPreTh = -1;
        public int m_nHover_2ndRXPreTRxSTh = -1;
        public int m_nHover_2ndTXPreTRxSTh = -1;

        public override void SetIntParameter(string sParameterName, int nParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sRXInnerMax:
                    m_nNoiseRXInnerMax = nParameterValue;
                    break;
                case SpecificText.m_sTXInnerMax:
                    m_nNoiseTXInnerMax = nParameterValue;
                    break;
                case SpecificText.m_sRXTotalMedian:
                    if (eSubStep == SubTuningStep.HOVER_1ST)
                        m_nHover_1stRXTotalMedian = nParameterValue;

                    break;
                case SpecificText.m_sTXTotalMedian:
                    if (eSubStep == SubTuningStep.HOVER_1ST)
                        m_nHover_1stTXTotalMedian = nParameterValue;

                    break;
                case SpecificText.m_sRXTotalMax:
                    if (eSubStep == SubTuningStep.HOVER_1ST)
                        m_nHover_1stRXTotalMax = nParameterValue;

                    break;
                case SpecificText.m_sTXTotalMax:
                    if (eSubStep == SubTuningStep.HOVER_1ST)
                        m_nHover_1stTXTotalMax = nParameterValue;

                    break;
                case SpecificText.m_sRXInnerMaxPlus3InnerMaxSTD:
                    m_nNoiseRXInMaxPlus3InMaxSTD = nParameterValue;
                    break;
                case SpecificText.m_sTrcMaxMinusPreP0_TH_1Trc:
                    m_nNoiseTrcMaxMinusPreP0_TH_1Trc = nParameterValue;
                    break;
                case SpecificText.m_sTrcMaxMinusPreP0_TH_2Trc:
                    m_nNoiseTrcMaxMinusPreP0_TH_2Trc = nParameterValue;
                    break;
                case SpecificText.m_sTrcMaxMinusPreP0_TH_3Trc:
                    m_nNoiseTrcMaxMinusPreP0_TH_3Trc = nParameterValue;
                    break;
                case SpecificText.m_sTrcMaxMinusPreP0_TH_4Trc:
                    m_nNoiseTrcMaxMinusPreP0_TH_4Trc = nParameterValue;
                    break;
                case SpecificText.m_sRXPreThreshold:
                    if (eSubStep == SubTuningStep.HOVER_1ST)
                        m_nHover_1stRXPreTh = nParameterValue;
                    else if (eSubStep == SubTuningStep.HOVER_2ND)
                        m_nHover_2ndRXPreTh = nParameterValue;

                    break;
                case SpecificText.m_sTXPreThreshold:
                    if (eSubStep == SubTuningStep.HOVER_1ST)
                        m_nHover_1stTXPreTh = nParameterValue;
                    else if (eSubStep == SubTuningStep.HOVER_2ND)
                        m_nHover_2ndTXPreTh = nParameterValue;

                    break;
                case SpecificText.m_sRXPreTRxSThreshold:
                    if (eSubStep == SubTuningStep.HOVER_1ST)
                        m_nHover_1stRXPreTRxSTh = nParameterValue;
                    else if (eSubStep == SubTuningStep.HOVER_2ND)
                        m_nHover_2ndRXPreTRxSTh = nParameterValue;

                    break;
                case SpecificText.m_sTXPreTRxSThreshold:
                    if (eSubStep == SubTuningStep.HOVER_1ST)
                        m_nHover_1stTXPreTRxSTh = nParameterValue;
                    else if (eSubStep == SubTuningStep.HOVER_2ND)
                        m_nHover_2ndTXPreTRxSTh = nParameterValue;

                    break;
                case SpecificText.m_scActivePen_DigiGain_P0:
                    m_nNoiseDigiGain_P0 = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_DigiGain_Beacon_Rx:
                    m_nNoiseDigiGain_Beacon_Rx = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_DigiGain_Beacon_Tx:
                    m_nNoiseDigiGain_Beacon_Tx = nParameterValue;
                    break;
                default:
                    break;
            }
        }

        public override void SetStringParameter(string sParameterName, string sParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sP0_Detect_Time:
                    if (sParameterValue == SpecificText.m_sP0_Detect_Time_400)
                    {
                        if (eSubStep == SubTuningStep.NO)
                            m_nNoiseP0_Detect_Time_Idx  = 0;
                        else
                            m_nDTHoverP0_Detect_Time_Idx = 0;
                    }
                    else if (sParameterValue == SpecificText.m_sP0_Detect_Time_800)
                    {
                        if (eSubStep == SubTuningStep.NO)
                            m_nNoiseP0_Detect_Time_Idx = 1;
                        else
                            m_nDTHoverP0_Detect_Time_Idx = 1;
                    }
                    else
                    {
                        if (eSubStep == SubTuningStep.NO)
                            m_nNoiseP0_Detect_Time_Idx = -1;
                        else
                            m_nDTHoverP0_Detect_Time_Idx = -1;
                    }

                    break;
                default:
                    break;
            }
        }
    }

    public class DTTRxSParameter : RecordParameter
    {
        public int m_nNoiseP0_Detect_Time_Idx = -1;

        public int m_nRXTraceNumber = -1;
        public int m_nTXTraceNumber = -1;

        public int m_nPreP0_TH = -1;
        public int m_nPreHover_TH_Rx = -1;
        public int m_nPreHover_TH_Tx = -1;
        public int m_nPreContact_TH_Rx = -1;
        public int m_nPreContact_TH_Tx = -1;
        public int m_nPreTRxS_Hover_TH_Rx = -1;
        public int m_nPreTRxS_Hover_TH_Tx = -1;
        public int m_nPreEdge_1Trc_SubPwr = -1;
        public int m_nPreEdge_2Trc_SubPwr = -1;
        public int m_nPreEdge_3Trc_SubPwr = -1;
        public int m_nPreEdge_4Trc_SubPwr = -1;

        public override void SetIntParameter(string sParameterName, int nParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sRXTraceNumber:
                    m_nRXTraceNumber = nParameterValue;
                    break;
                case SpecificText.m_sTXTraceNumber:
                    m_nTXTraceNumber = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_FM_P0_TH:
                    m_nPreP0_TH = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_Beacon_Hover_TH_Rx:
                    m_nPreHover_TH_Rx = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_Beacon_Hover_TH_Tx:
                    m_nPreHover_TH_Tx = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_Beacon_Contact_TH_Rx:
                    m_nPreContact_TH_Rx = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_Beacon_Contact_TH_Tx:
                    m_nPreContact_TH_Tx = nParameterValue;
                    break;
                case SpecificText.m_sRXThreshold:
                    m_nPreTRxS_Hover_TH_Rx = nParameterValue;
                    break;
                case SpecificText.m_sTXThreshold:
                    m_nPreTRxS_Hover_TH_Tx = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr:
                    m_nPreEdge_1Trc_SubPwr = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr:
                    m_nPreEdge_2Trc_SubPwr = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr:
                    m_nPreEdge_3Trc_SubPwr = nParameterValue;
                    break;
                case SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr:
                    m_nPreEdge_4Trc_SubPwr = nParameterValue;
                    break;
                default:
                    break;
            }
        }

        public override void SetStringParameter(string sParameterName, string sParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sNoiseP0_Detect_Time:
                    if (sParameterValue == SpecificText.m_sP0_Detect_Time_400)
                        m_nNoiseP0_Detect_Time_Idx = 0;
                    else if (sParameterValue == SpecificText.m_sP0_Detect_Time_800)
                        m_nNoiseP0_Detect_Time_Idx = 1;

                    break;
                default:
                    break;
            }
        }
    }

    public class TiltTuningParameter : RecordParameter
    {
        public string m_sPreviousErrorMessage = "N/A";

        public double m_dPTHFNormRMSE_H = -1.0;
        public double m_dPTHFNormRMSE_V = -1.0;

        public int m_nPTHFContact_TH_Rx = -1;
        public int m_nPTHFContact_TH_Tx = -1;
        public int m_nPTHFHover_TH_Rx = -1;
        public int m_nPTHFHover_TH_Tx = -1;

        public double m_dPTHFRXRingMean = -1.0;
        public double m_dPTHFTXRingMean = -1.0;

        public int m_nPTHFRXRMeanMinus1STD = -1;
        public int m_nPTHFRXRMeanMinus2STD = -1;
        public int m_nPTHFTXRMeanMinus1STD = -1;
        public int m_nPTHFTXRMeanMinus2STD = -1;

        public override void SetIntParameter(string sParameterName, int nParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sRXContactTH:
                    m_nPTHFContact_TH_Rx = nParameterValue;
                    break;
                case SpecificText.m_sTXContactTH:
                    m_nPTHFContact_TH_Tx = nParameterValue;
                    break;
                case SpecificText.m_sRXHoverTH:
                    m_nPTHFHover_TH_Rx = nParameterValue;
                    break;
                case SpecificText.m_sTXHoverTH:
                    m_nPTHFHover_TH_Tx = nParameterValue;
                    break;
                case SpecificText.m_sRXRingMeanMinus1STD:
                    m_nPTHFRXRMeanMinus1STD = nParameterValue;
                    break;
                case SpecificText.m_sRXRingMeanMinus2STD:
                    m_nPTHFRXRMeanMinus2STD = nParameterValue;
                    break;
                case SpecificText.m_sTXRingMeanMinus1STD:
                    m_nPTHFTXRMeanMinus1STD = nParameterValue;
                    break;
                case SpecificText.m_sTXRingMeanMinus2STD:
                    m_nPTHFTXRMeanMinus2STD = nParameterValue;
                    break;
                default:
                    break;
            }
        }

        public override void SetDoubleParameter(string sParameterName, double dParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sNormalizeRMSE_H:
                    m_dPTHFNormRMSE_H = dParameterValue;
                    break;
                case SpecificText.m_sNormalizeRMSE_V:
                    m_dPTHFNormRMSE_V = dParameterValue;
                    break;
                case SpecificText.m_sRXRingMean:
                    m_dPTHFRXRingMean = dParameterValue;
                    break;
                case SpecificText.m_sTXRingMean:
                    m_dPTHFTXRingMean = dParameterValue;
                    break;
                default:
                    break;
            }
        }

        public override void SetStringParameter(string sParameterName, string sParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sErrorMessage:
                    m_sPreviousErrorMessage = sParameterValue;
                    break;
                default:
                    break;
            }
        }
    }

    public class PressureTuningParameter : RecordParameter
    {
        public int m_nRXTraceNumber = -1;
        public int m_nTXTraceNumber = -1;

        public override void SetIntParameter(string sParameterName, int nParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sRXTraceNumber:
                    m_nRXTraceNumber = nParameterValue;
                    break;
                case SpecificText.m_sTXTraceNumber:
                    m_nTXTraceNumber = nParameterValue;
                    break;
                default:
                    break;
            }
        }
    }

    public class LinearityTuningParameter : RecordParameter
    {
        public int m_nNoiseRXInMaxPlus3InMaxSTD = -1;
        public int m_nNoiseTXInMaxPlus3InMaxSTD = -1;

        public override void SetIntParameter(string sParameterName, int nParameterValue, SubTuningStep eSubStep)
        {
            switch (sParameterName)
            {
                case SpecificText.m_sRXInnerMaxPlus3InnerMaxSTD:
                    m_nNoiseRXInMaxPlus3InMaxSTD = nParameterValue;
                    break;
                case SpecificText.m_sTXInnerMaxPlus3InnerMaxSTD:
                    m_nNoiseTXInMaxPlus3InMaxSTD = nParameterValue;
                    break;
                default:
                    break;
            }
        }
    }
}
