using System;
using System.Collections.Generic;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class RecordSetInfo
    {
        public static List<RecordSetParameter> m_cRecordSetParameter_List = new List<RecordSetParameter>();

        public static void Clear()
        {
            m_cRecordSetParameter_List.Clear();
        }
    }

    // Record Set Parameter
    public class RecordSetParameter
    {
        public const int m_nRECORDVALUE_STRING = 0;
        public const int m_nRECORDVALUE_INT = 1;

        public int m_nRankIndex;
        public FlowRobot m_eRobot;
        public FlowRecord m_eRecord;
        public MainTuningStep m_eMainStep;
        public SubTuningStep m_eSubStep;
        public int m_nPH1;
        public int m_nPH2;
        public string m_sNote;
        public int m_nWeight = -1;
        public double m_dFrequency;
        public int m_nSection = -1;
        public bool m_bDisableSetParameterFlag = false;
        public bool m_bTRxSAllScanFlag = false;
        public EdgeShadowOption m_eEdgeShadowOption = EdgeShadowOption.NOT_CHANGE;

        public int m_nTraceType = MainConstantParameter.m_nTRACETYPE_NONE;

        //Tilt Noise Step
        public int m_nTNPTHFRXTotalMax;
        public int m_nTNPTHFTXTotalMax;

        //Digital Tuning Normal Step
        public int m_nNoiseP0_DetectTime_Idx;
        public int m_nNoiseRXInnerMax;
        public int m_nNoiseTXInnerMax;

        public int m_nNoiseRXInMaxPlus3InMaxSTD;
        public int m_nNoiseTXInMaxPlus3InMaxSTD;

        public int m_nNoiseTrcMaxMinusPreP0_TH_1Trc;
        public int m_nNoiseTrcMaxMinusPreP0_TH_2Trc;
        public int m_nNoiseTrcMaxMinusPreP0_TH_3Trc;
        public int m_nNoiseTrcMaxMinusPreP0_TH_4Trc;

        public int m_nHover1stRXMedian;
        public int m_nHover1stTXMedian;
        public int m_nHover1stRXMax;
        public int m_nHover1stTXMax;

        //Digital Tuning TRxS Step
        public int m_nRX_TraceNumber;
        public int m_nTX_TraceNumber;
        public int m_nEdge_1Trc_SubPwr;
        public int m_nEdge_2Trc_SubPwr;
        public int m_nEdge_3Trc_SubPwr;
        public int m_nEdge_4Trc_SubPwr;

        //Digital Tuning TRxS & Tilt Tuning Step
        public int m_ncActivePen_FM_P0_TH;
        public int m_nTRxS_Beacon_Hover_TH_Rx;
        public int m_nTRxS_Beacon_Hover_TH_Tx;
        public int m_nTRxS_Beacon_Contact_TH_Rx;
        public int m_nTRxS_Beacon_Contact_TH_Tx;

        //Tilt Tuning Step
        public int m_nBeacon_Hover_TH_Rx;
        public int m_nBeacon_Hover_TH_Tx;
        public int m_nBeacon_Contact_TH_Rx;
        public int m_nBeacon_Contact_TH_Tx;
        public int m_nPTHF_Contact_TH_Rx;
        public int m_nPTHF_Contact_TH_Tx;
        public int m_nPTHF_Hover_TH_Rx;
        public int m_nPTHF_Hover_TH_Tx;
        public int m_nBHF_Contact_TH_Rx;
        public int m_nBHF_Contact_TH_Tx;
        public int m_nBHF_Hover_TH_Rx;
        public int m_nBHF_Hover_TH_Tx;

        //Pressure Tuning Step
        public int m_nIQ_BSH_P;
        public int m_nPressure3BinsTH;
        public int m_nPress_3BinsPwr;

        //All Step
        public int m_nDigiGain_P0;
        public int m_nDigiGain_Beacon_Rx;
        public int m_nDigiGain_Beacon_Tx;
        public int m_nDigiGain_PTHF_Rx;
        public int m_nDigiGain_PTHF_Tx;
        public int m_nDigiGain_BHF_Rx;
        public int m_nDigiGain_BHF_Tx;

        //Digital Tuning - Contact
        public int m_nNoiseDigiGain_P0;
        public int m_nNoiseDigiGain_Beacon_Rx;
        public int m_nNoiseDigiGain_Beacon_Tx;

        //Tilt Tuning
        public int m_nTNDigiGain_PTHF_Rx;
        public int m_nTNDigiGain_PTHF_Tx;
        public int m_nTNDigiGain_BHF_Rx;
        public int m_nTNDigiGain_BHF_Tx;

        public RecordSetParameter()
        {
            m_nRankIndex = 0;
            m_eRobot = FlowRobot.NO;
            m_eRecord = FlowRecord.NTRX;
            m_eMainStep = MainTuningStep.ELSE;
            m_eSubStep = SubTuningStep.ELSE;
            m_nPH1 = 8;
            m_nPH2 = 20;
            m_sNote = "";
            m_nWeight = -1;

            m_nTraceType = MainConstantParameter.m_nTRACETYPE_NONE;

            m_nTNPTHFRXTotalMax = -1;
            m_nTNPTHFTXTotalMax = -1;

            m_nNoiseP0_DetectTime_Idx = -1;
            m_nNoiseRXInnerMax = -1;
            m_nNoiseTXInnerMax = -1;

            m_nNoiseRXInMaxPlus3InMaxSTD = -1;
            m_nNoiseTXInMaxPlus3InMaxSTD = -1;

            m_nNoiseTrcMaxMinusPreP0_TH_1Trc = -1;
            m_nNoiseTrcMaxMinusPreP0_TH_2Trc = -1;
            m_nNoiseTrcMaxMinusPreP0_TH_3Trc = -1;
            m_nNoiseTrcMaxMinusPreP0_TH_4Trc = -1;

            m_nHover1stRXMedian = -1;
            m_nHover1stTXMedian = -1;
            m_nHover1stRXMax = -1;
            m_nHover1stTXMax = -1;

            m_nRX_TraceNumber = -1;
            m_nTX_TraceNumber = -1;
            m_nEdge_1Trc_SubPwr = -1;
            m_nEdge_2Trc_SubPwr = -1;
            m_nEdge_3Trc_SubPwr = -1;
            m_nEdge_4Trc_SubPwr = -1;

            m_ncActivePen_FM_P0_TH = -1;
            m_nTRxS_Beacon_Hover_TH_Rx = -1;
            m_nTRxS_Beacon_Hover_TH_Tx = -1;
            m_nTRxS_Beacon_Contact_TH_Rx = -1;
            m_nTRxS_Beacon_Contact_TH_Tx = -1;

            m_nBeacon_Hover_TH_Rx = -1;
            m_nBeacon_Hover_TH_Tx = -1;
            m_nBeacon_Contact_TH_Rx = -1;
            m_nBeacon_Contact_TH_Tx = -1;
            m_nPTHF_Contact_TH_Rx = -1;
            m_nPTHF_Contact_TH_Tx = -1;
            m_nPTHF_Hover_TH_Rx = -1;
            m_nPTHF_Hover_TH_Tx = -1;
            m_nBHF_Contact_TH_Rx = -1;
            m_nBHF_Contact_TH_Tx = -1;
            m_nBHF_Hover_TH_Rx = -1;
            m_nBHF_Hover_TH_Tx = -1;

            m_nIQ_BSH_P = -1;
            m_nPressure3BinsTH = -1;
            m_nPress_3BinsPwr = -1;

            m_nDigiGain_P0 = -1;
            m_nDigiGain_Beacon_Rx = -1;
            m_nDigiGain_Beacon_Tx = -1;
            m_nDigiGain_PTHF_Rx = -1;
            m_nDigiGain_PTHF_Tx = -1;
            m_nDigiGain_BHF_Rx = -1;
            m_nDigiGain_BHF_Tx = -1;

            m_nNoiseDigiGain_P0 = -1;
            m_nNoiseDigiGain_Beacon_Rx = -1;

            m_nTNDigiGain_PTHF_Rx = -1;
            m_nTNDigiGain_PTHF_Tx = -1;
            m_nTNDigiGain_BHF_Rx = -1;
            m_nTNDigiGain_BHF_Tx = -1;
        }

        public void SetParameter(SubTuningStep eCompareSubStep, string sParameterName, object objParameterValue, SubTuningStep eCurrentSubStep, int nRecordValueType)
        {
            if (nRecordValueType == m_nRECORDVALUE_STRING)
            {
                string sParameterValue = Convert.ToString(objParameterValue);

                switch (sParameterName)
                {
                    case SpecificText.m_sP0_Detect_Time:
                        if (sParameterValue == SpecificText.m_sP0_Detect_Time_400)
                        {
                            if (eCurrentSubStep == SubTuningStep.NO)
                                m_nNoiseP0_DetectTime_Idx = 0;
                        }
                        else if (sParameterValue == SpecificText.m_sP0_Detect_Time_800)
                        {
                            if (eCurrentSubStep == SubTuningStep.NO)
                                m_nNoiseP0_DetectTime_Idx = 1;
                        }
                        else
                        {
                            if (eCurrentSubStep == SubTuningStep.NO)
                                m_nNoiseP0_DetectTime_Idx = -1;
                        }

                        break;
                    default:
                        break;
                }
            }
            else if (nRecordValueType == m_nRECORDVALUE_INT)
            {
                int nParameterValue = -1;
                string sParameterValue = Convert.ToString(objParameterValue);
                Int32.TryParse(sParameterValue, out nParameterValue);

                switch (sParameterName)
                {
                    case SpecificText.m_sRXInnerMax:
                        m_nNoiseRXInnerMax = nParameterValue;
                        break;
                    case SpecificText.m_sTXInnerMax:
                        m_nNoiseTXInnerMax = nParameterValue;
                        break;
                    case SpecificText.m_sRXTotalMedian:
                        if (eCurrentSubStep == SubTuningStep.HOVER_1ST)
                        {
                            if (ParamAutoTuning.m_nContactStepFilterType == 1)
                                m_nHover1stRXMedian = nParameterValue;
                        }

                        break;
                    case SpecificText.m_sTXTotalMedian:
                        if (eCurrentSubStep == SubTuningStep.HOVER_1ST)
                        {
                            if (ParamAutoTuning.m_nContactStepFilterType == 1)
                                m_nHover1stTXMedian = nParameterValue;
                        }

                        break;
                    case SpecificText.m_sRXTotalMax:
                        if (eCompareSubStep == SubTuningStep.TILTNO_BHF)
                            m_nHover1stRXMax = nParameterValue;
                        else if (eCompareSubStep == SubTuningStep.HOVER_1ST)
                            m_nHover1stTXMax = nParameterValue;

                        break;
                    case SpecificText.m_sTXTotalMax:
                        if (eCompareSubStep == SubTuningStep.TILTNO_BHF)
                            m_nTNPTHFRXTotalMax = nParameterValue;
                        else if (eCompareSubStep == SubTuningStep.HOVER_1ST)
                            m_nTNPTHFTXTotalMax = nParameterValue;

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
                    case SpecificText.m_sRXTraceNumber:
                        m_nRX_TraceNumber = nParameterValue;
                        break;
                    case SpecificText.m_sTXTraceNumber:
                        m_nTX_TraceNumber = nParameterValue;
                        break;
                    case SpecificText.m_sNoiseRXInnerMax:
                        m_nNoiseRXInnerMax = nParameterValue;
                        break;
                    case SpecificText.m_sNoiseTXInnerMax:
                        m_nNoiseTXInnerMax = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_FM_P0_TH:
                        m_ncActivePen_FM_P0_TH = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Rx:
                        m_nTRxS_Beacon_Hover_TH_Rx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Tx:
                        m_nTRxS_Beacon_Hover_TH_Tx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Rx:
                        m_nTRxS_Beacon_Contact_TH_Rx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Tx:
                        m_nTRxS_Beacon_Contact_TH_Tx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_Beacon_Hover_TH_Rx:
                        m_nBeacon_Hover_TH_Rx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_Beacon_Hover_TH_Tx:
                        m_nBeacon_Hover_TH_Tx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_Beacon_Contact_TH_Rx:
                        m_nBeacon_Contact_TH_Rx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_Beacon_Contact_TH_Tx:
                        m_nBeacon_Contact_TH_Tx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_PTHF_Contact_TH_Rx:
                        m_nPTHF_Contact_TH_Rx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_PTHF_Contact_TH_Tx:
                        m_nPTHF_Contact_TH_Tx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_PTHF_Hover_TH_Rx:
                        m_nPTHF_Hover_TH_Rx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_PTHF_Hover_TH_Tx:
                        m_nPTHF_Hover_TH_Tx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_BHF_Contact_TH_Rx:
                        m_nBHF_Contact_TH_Rx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_BHF_Contact_TH_Tx:
                        m_nBHF_Contact_TH_Tx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_BHF_Hover_TH_Rx:
                        m_nBHF_Hover_TH_Rx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_BHF_Hover_TH_Tx:
                        m_nBHF_Hover_TH_Tx = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr:
                        m_nEdge_1Trc_SubPwr = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr:
                        m_nEdge_2Trc_SubPwr = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr:
                        m_nEdge_3Trc_SubPwr = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr:
                        m_nEdge_4Trc_SubPwr = nParameterValue;
                        break;
                    case SpecificText.m_s_Pen_Ntrig_IQ_BSH_P:
                        m_nIQ_BSH_P = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_FM_Pressure3BinsTH:
                        m_nPressure3BinsTH = nParameterValue;
                        break;
                    case SpecificText.m_sPress_3BinsPwr:
                        m_nPress_3BinsPwr = nParameterValue;
                        break;
                    case SpecificText.m_scActivePen_DigiGain_P0:
                        if (eCurrentSubStep == SubTuningStep.NO)
                            m_nNoiseDigiGain_P0 = nParameterValue;
                        else
                            m_nDigiGain_P0 = nParameterValue;

                        break;
                    case SpecificText.m_scActivePen_DigiGain_Beacon_Rx:
                        if (eCurrentSubStep == SubTuningStep.NO)
                            m_nNoiseDigiGain_Beacon_Rx = nParameterValue;
                        else
                            m_nDigiGain_Beacon_Rx = nParameterValue;

                        break;
                    case SpecificText.m_scActivePen_DigiGain_Beacon_Tx:
                        if (eCurrentSubStep == SubTuningStep.NO)
                            m_nNoiseDigiGain_Beacon_Tx = nParameterValue;
                        else
                            m_nDigiGain_Beacon_Tx = nParameterValue;

                        break;
                    case SpecificText.m_scActivePen_DigiGain_PTHF_Rx:
                        if (eCurrentSubStep == SubTuningStep.TILTNO_BHF)
                            m_nTNDigiGain_PTHF_Rx = nParameterValue;
                        else
                            m_nDigiGain_PTHF_Rx = nParameterValue;

                        break;
                    case SpecificText.m_scActivePen_DigiGain_PTHF_Tx:
                        if (eCurrentSubStep == SubTuningStep.TILTNO_BHF)
                            m_nTNDigiGain_PTHF_Tx = nParameterValue;
                        else
                            m_nDigiGain_PTHF_Tx = nParameterValue;

                        break;
                    case SpecificText.m_scActivePen_DigiGain_BHF_Rx:
                        if (eCurrentSubStep == SubTuningStep.TILTNO_BHF)
                            m_nTNDigiGain_BHF_Rx = nParameterValue;
                        else
                            m_nDigiGain_BHF_Rx = nParameterValue;

                        break;
                    case SpecificText.m_scActivePen_DigiGain_BHF_Tx:
                        if (eCurrentSubStep == SubTuningStep.TILTNO_BHF)
                            m_nTNDigiGain_BHF_Tx = nParameterValue;
                        else
                            m_nDigiGain_BHF_Tx = nParameterValue;

                        break;
                    default:
                        break;
                }
            }
        }

        public void SetPTHFAndBHFThreshold(MainTuningStep eMainStep)
        {
            if (eMainStep == MainTuningStep.TILTTUNING)
            {
                if (m_nTNDigiGain_PTHF_Rx > -1 && m_nDigiGain_PTHF_Rx > -1)
                {
                    double dDigiGainRatio = (double)m_nDigiGain_PTHF_Rx / (double)m_nTNDigiGain_PTHF_Rx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nPTHF_Contact_TH_Rx = (int)(m_nPTHF_Contact_TH_Rx * dDigiGainRatio);
                    m_nPTHF_Hover_TH_Rx = (int)(m_nPTHF_Hover_TH_Rx * dDigiGainRatio);
                }

                if (m_nTNDigiGain_PTHF_Tx > -1 && m_nDigiGain_PTHF_Tx > -1)
                {
                    double dDigiGainRatio = (double)m_nDigiGain_PTHF_Tx / (double)m_nTNDigiGain_PTHF_Tx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nPTHF_Contact_TH_Rx = (int)(m_nPTHF_Contact_TH_Tx * dDigiGainRatio);
                    m_nPTHF_Hover_TH_Rx = (int)(m_nPTHF_Hover_TH_Tx * dDigiGainRatio);
                }

                if (m_nTNDigiGain_BHF_Rx > -1 && m_nDigiGain_BHF_Rx > -1)
                {
                    double dDigiGainRatio = (double)m_nDigiGain_BHF_Rx / (double)m_nTNDigiGain_BHF_Rx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nBHF_Contact_TH_Rx = (int)(m_nBHF_Contact_TH_Rx * dDigiGainRatio);
                    m_nBHF_Hover_TH_Rx = (int)(m_nBHF_Hover_TH_Rx * dDigiGainRatio);
                }

                if (m_nTNDigiGain_BHF_Tx > -1 && m_nDigiGain_BHF_Tx > -1)
                {
                    double dDigiGainRatio = (double)m_nDigiGain_BHF_Tx / (double)m_nTNDigiGain_BHF_Tx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nBHF_Contact_TH_Rx = (int)(m_nBHF_Contact_TH_Tx * dDigiGainRatio);
                    m_nBHF_Hover_TH_Rx = (int)(m_nBHF_Hover_TH_Tx * dDigiGainRatio);
                }
            }
            else if (eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                if (m_nNoiseDigiGain_Beacon_Rx > -1)
                {
                    if (m_nDigiGain_PTHF_Rx > -1)
                    {
                        double dDigiGainRatio = (double)m_nDigiGain_PTHF_Rx / (double)m_nNoiseDigiGain_Beacon_Rx;
                        dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                        m_nPTHF_Contact_TH_Rx = (int)(m_nPTHF_Contact_TH_Rx * dDigiGainRatio);
                        m_nPTHF_Hover_TH_Rx = (int)(m_nPTHF_Hover_TH_Rx * dDigiGainRatio);
                    }

                    if (m_nDigiGain_BHF_Rx > -1)
                    {
                        double dDigiGainRatio = (double)m_nDigiGain_BHF_Rx / (double)m_nTNDigiGain_BHF_Rx;
                        dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                        m_nBHF_Contact_TH_Rx = (int)(m_nBHF_Contact_TH_Rx * dDigiGainRatio);
                        m_nBHF_Hover_TH_Rx = (int)(m_nBHF_Hover_TH_Rx * dDigiGainRatio);
                    }
                }

                if (m_nNoiseDigiGain_Beacon_Tx > -1)
                {
                    if (m_nDigiGain_PTHF_Tx > -1)
                    {
                        double dDigiGainRatio = (double)m_nDigiGain_PTHF_Tx / (double)m_nNoiseDigiGain_Beacon_Tx;
                        dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                        m_nPTHF_Contact_TH_Rx = (int)(m_nPTHF_Contact_TH_Tx * dDigiGainRatio);
                        m_nPTHF_Hover_TH_Rx = (int)(m_nPTHF_Hover_TH_Tx * dDigiGainRatio);
                    }

                    if (m_nDigiGain_BHF_Tx > -1)
                    {
                        double dDigiGainRatio = (double)m_nDigiGain_BHF_Tx / (double)m_nNoiseDigiGain_Beacon_Tx;
                        dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                        m_nBHF_Contact_TH_Rx = (int)(m_nBHF_Contact_TH_Tx * dDigiGainRatio);
                        m_nBHF_Hover_TH_Rx = (int)(m_nBHF_Hover_TH_Tx * dDigiGainRatio);
                    }
                }
            }
        }
    }
}
