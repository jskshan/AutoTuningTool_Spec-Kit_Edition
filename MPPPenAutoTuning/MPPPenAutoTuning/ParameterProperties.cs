using System.Collections.Generic;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    /// <summary>
    /// Main Tuning Step Set
    /// </summary>
    public enum MainTuningStep
    {
        NO = 1,
        TILTNO = 2,
        DIGIGAINTUNING = 3,
        TPGAINTUNING = 4,
        PEAKCHECKTUNING = 5,
        DIGITALTUNING = 6,
        TILTTUNING = 7,
        PRESSURETUNING = 8,
        LINEARITYTUNING = 9,
        SERVERCONTRL = 10,
        ELSE = 11
    }

    /// <summary>
    /// Sub Tuning Step Set
    /// </summary>
    public enum SubTuningStep
    {
        NO = 0,
        HOVER_1ST = 1,
        HOVER_2ND = 2,
        CONTACT = 3,
        HOVERTRxS = 4,
        CONTACTTRxS = 5,
        TILTNO_PTHF = 6,
        TILTNO_BHF = 7,
        TILTTUNING_PTHF = 8,
        TILTTUNING_BHF = 9,
        PRESSURESETTING = 10,
        PRESSUREPROTECT = 11,
        PRESSURETABLE = 12,
        LINEARITYTABLE = 13,
        PCHOVER_1ST = 14,
        PCHOVER_2ND = 15,
        PCCONTACT = 16,
        DIGIGAIN = 17,
        TP_GAIN = 18,
        ELSE = 19
    }

    /// <summary>
    /// EDID Info Class
    /// </summary>
    public class EDIDInfo
    {
        public int nEDIDInfo_Width = 0;
        public int nEDIDInfo_Height = 0;
        public double dScreenSize = 0;
    }

    /// <summary>
    /// Monitor State Set
    /// </summary>
    public enum MonitorState : int
    {
        MONITOR_ON = -1,
        MONITOR_OFF = 2,
        MONITOR_STANBY = 1
    };

    /// <summary>
    /// Flow Robot Set
    /// </summary>
    public enum FlowRobot
    {
        NO,
        HOVERLINE,
        HOVERPOINT_CEN,
        TOUCHLINE,
        TOUCHLINE_HOR,
        TOUCHLINE_VER,
        TOUCHPOINT_CEN
    }

    /// <summary>
    /// Flow Record Set
    /// </summary>
    public enum FlowRecord
    {
        NTRX,
        NRX,
        NTX,
        RX,
        TX,
        TRX,
        TRxS,
        TILT,
        PRESSURE,
        LINEARITY,
        DIGIGAIN,
        TP_GAIN,
        PTHF_NoSync_Gen8,
        BHF_NoSync_Gen8
    }

    /// <summary>
    /// Parameter Set Class
    /// </summary>
    public class ParameterSet
    {
        private const int m_nSETDIGIGAINTYPE_NONE           = 0;
        private const int m_nSETDIGIGAINTYPE_DEFAULT        = 1;
        private const int m_nSETDIGIGAINTYPE_COMPUTEVALUE   = 2;

        public int m_nSettingPH1 = -1, m_nSettingPH2 = -1;
        public int m_nReadPH1 = -1, m_nReadPH2 = -1;

        public int m_nTraceType = MainConstantParameter.m_nTRACETYPE_NONE;
        public int m_nSection = -1;

        public int m_nSettingcActivePen_FM_P0_TH = -1;
        public int m_nSettingTRxS_Beacon_Hover_TH_Rx = -1;
        public int m_nSettingTRxS_Beacon_Hover_TH_Tx = -1;
        public int m_nSettingTRxS_Beacon_Contact_TH_Rx = -1;
        public int m_nSettingTRxS_Beacon_Contact_TH_Tx = -1;
        public int m_nSettingBeacon_Hover_TH_Rx = -1;
        public int m_nSettingBeacon_Hover_TH_Tx = -1;
        public int m_nSettingBeacon_Contact_TH_Rx = -1;
        public int m_nSettingBeacon_Contact_TH_Tx = -1;

        public int m_nSPen_HI_HF_THD = -1;

        public int m_nSEdge_1Trc_SubPwr = -1;
        public int m_nSEdge_2Trc_SubPwr = -1;
        public int m_nSEdge_3Trc_SubPwr = -1;
        public int m_nSEdge_4Trc_SubPwr = -1;

        public int m_nSetDigiGainType = m_nSETDIGIGAINTYPE_NONE;

        public int m_nSDigiGain_P0 = -1;
        public int m_nSDigiGain_Beacon_Rx = -1;
        public int m_nSDigiGain_Beacon_Tx = -1;
        public int m_nSDigiGain_PTHF_Rx = -1;
        public int m_nSDigiGain_PTHF_Tx = -1;
        public int m_nSDigiGain_BHF_Rx = -1;
        public int m_nSDigiGain_BHF_Tx = -1;

        public int m_nSettingPTHF_Contact_TH_Rx = -1;
        public int m_nSettingPTHF_Contact_TH_Tx = -1;
        public int m_nSettingPTHF_Hover_TH_Rx = -1;
        public int m_nSettingPTHF_Hover_TH_Tx = -1;
        public int m_nSettingBHF_Contact_TH_Rx = -1;
        public int m_nSettingBHF_Contact_TH_Tx = -1;
        public int m_nSettingBHF_Hover_TH_Rx = -1;
        public int m_nSettingBHF_Hover_TH_Tx = -1;

        public int m_nReadcActivePen_FM_P0_TH = -1;
        public int m_nReadTRxS_Beacon_Hover_TH_Rx = -1;
        public int m_nReadTRxS_Beacon_Hover_TH_Tx = -1;
        public int m_nReadTRxS_Beacon_Contact_TH_Rx = -1;
        public int m_nReadTRxS_Beacon_Contact_TH_Tx = -1;
        public int m_nReadBeacon_Hover_TH_Rx = -1;
        public int m_nReadBeacon_Hover_TH_Tx = -1;
        public int m_nReadBeacon_Contact_TH_Rx = -1;
        public int m_nReadBeacon_Contact_TH_Tx = -1;

        public int m_nRPen_HI_HF_THD = -1;

        public int m_nRXTraceNumber = -1;
        public int m_nTXTraceNumber = -1;

        public int m_nREdge_1Trc_SubPwr = -1;
        public int m_nREdge_2Trc_SubPwr = -1;
        public int m_nREdge_3Trc_SubPwr = -1;
        public int m_nREdge_4Trc_SubPwr = -1;

        public int m_nRDigiGain_P0 = -1;
        public int m_nRDigiGain_Beacon_Rx = -1;
        public int m_nRDigiGain_Beacon_Tx = -1;
        public int m_nRDigiGain_PTHF_Rx = -1;
        public int m_nRDigiGain_PTHF_Tx = -1;
        public int m_nRDigiGain_BHF_Rx = -1;
        public int m_nRDigiGain_BHF_Tx = -1;

        public int m_nReadPTHF_Contact_TH_Rx = -1;
        public int m_nReadPTHF_Contact_TH_Tx = -1;
        public int m_nReadPTHF_Hover_TH_Rx = -1;
        public int m_nReadPTHF_Hover_TH_Tx = -1;
        public int m_nReadBHF_Contact_TH_Rx = -1;
        public int m_nReadBHF_Contact_TH_Tx = -1;
        public int m_nReadBHF_Hover_TH_Rx = -1;
        public int m_nReadBHF_Hover_TH_Tx = -1;

        public int m_nReadReportNumber = -1;
        public int m_nFWCheckVersion = -1;
        public int m_nRankIndex = 0;
        public int m_nNoiseP0_Detect_Time_Idx = -1;
        public int m_nNoiseRXInnerMax = -1, m_nNoiseTXInnerMax = -1;

        public int m_nNoiseRXInMaxPlus3InMaxSTD = -1;
        public int m_nNoiseTXInMaxPlus3InMaxSTD = -1;

        public int m_nNoiseTrcMaxMinusPreP0_TH_1Trc = -1;
        public int m_nNoiseTrcMaxMinusPreP0_TH_2Trc = -1;
        public int m_nNoiseTrcMaxMinusPreP0_TH_3Trc = -1;
        public int m_nNoiseTrcMaxMinusPreP0_TH_4Trc = -1;

        public int m_nNoiseDigiGain_P0 = -1;
        public int m_nNoiseDigiGain_Beacon_Rx = -1;
        public int m_nNoiseDigiGain_Beacon_Tx = -1;

        public int m_nTNDigiGain_PTHF_Rx = -1;
        public int m_nTNDigiGain_PTHF_Tx = -1;
        public int m_nTNDigiGain_BHF_Rx = -1;
        public int m_nTNDigiGain_BHF_Tx = -1;

        public int m_nRXFilterValue = -1, m_nTXFilterValue = -1;

        public int m_nPressureWeight = -1;
        public int m_nStartWeight = -1;
        public int m_nEndWeight = -1;
        public int m_nRealityWeight = -1;
        public int m_nOffsetWeight = -1;
        public int m_nExtraIncWeight = -1;
        public int m_nPTPenVersion = -1;
        public int m_nTotalWeight = -1;

        public int m_nROrgIQ_BSH_P = -1;
        public int m_nSIQ_BSH_P = -1;
        public int m_nSPressure3BinsTH = -1;
        public int m_nS3BinsPwr = -1;
        public int m_nRIQ_BSH_P = -1;
        public int m_nRPressure3BinsTH = -1;
        public int m_nR3BinsPwr = -1;
        public int m_nPress_MaxDFTRxMean = -1;
        public int m_nBefIQ_BSH_P = -1;
        public int m_nBefPress_MaxDFTRxMean = -1;

        public string m_sSubStep = "";
        public string m_sFWCheckVersion = "";
        public string m_sFrequency = "";
        public string m_sHoverRaiseHeight = "";
        public string m_sP0_detect_time = "";
        public string m_sDrawLineType = "";
        public string m_sControlMode = "";

        /// <summary>
        /// 設定參數初始化
        /// </summary>
        public void InitializeParameter()
        {
            m_nSettingPH1 = -1;
            m_nSettingPH2 = -1;
            m_nReadPH1 = -1;
            m_nReadPH2 = -1;

            m_nReadReportNumber = -1;
            m_nFWCheckVersion = -1;
            m_nRankIndex = 0;

            m_nNoiseP0_Detect_Time_Idx = -1;
            m_nNoiseRXInnerMax = -1;
            m_nNoiseTXInnerMax = -1;

            m_nNoiseRXInMaxPlus3InMaxSTD = -1;
            m_nNoiseTXInMaxPlus3InMaxSTD = -1;

            m_nNoiseTrcMaxMinusPreP0_TH_1Trc = -1;
            m_nNoiseTrcMaxMinusPreP0_TH_2Trc = -1;
            m_nNoiseTrcMaxMinusPreP0_TH_3Trc = -1;
            m_nNoiseTrcMaxMinusPreP0_TH_4Trc = -1;

            m_nNoiseDigiGain_P0 = -1;
            m_nNoiseDigiGain_Beacon_Rx = -1;
            m_nNoiseDigiGain_Beacon_Tx = -1;

            m_nTNDigiGain_PTHF_Rx = -1;
            m_nTNDigiGain_PTHF_Tx = -1;
            m_nTNDigiGain_BHF_Rx = -1;
            m_nTNDigiGain_BHF_Tx = -1;

            m_nRXFilterValue = -1;
            m_nTXFilterValue = -1;

            m_nSettingcActivePen_FM_P0_TH = -1;
            m_nSettingTRxS_Beacon_Hover_TH_Rx = -1;
            m_nSettingTRxS_Beacon_Hover_TH_Tx = -1;
            m_nSettingTRxS_Beacon_Contact_TH_Rx = -1;
            m_nSettingTRxS_Beacon_Contact_TH_Tx = -1;
            m_nSettingBeacon_Hover_TH_Rx = -1;
            m_nSettingBeacon_Hover_TH_Tx = -1;
            m_nSettingBeacon_Contact_TH_Rx = -1;
            m_nSettingBeacon_Contact_TH_Tx = -1;

            m_nRXTraceNumber = -1;
            m_nTXTraceNumber = -1;

            m_nSEdge_1Trc_SubPwr = -1;
            m_nSEdge_2Trc_SubPwr = -1;
            m_nSEdge_3Trc_SubPwr = -1;
            m_nSEdge_4Trc_SubPwr = -1;

            m_nSetDigiGainType = m_nSETDIGIGAINTYPE_NONE;

            m_nSDigiGain_P0 = -1;
            m_nSDigiGain_Beacon_Rx = -1;
            m_nSDigiGain_Beacon_Tx = -1;
            m_nSDigiGain_PTHF_Rx = -1;
            m_nSDigiGain_PTHF_Tx = -1;
            m_nSDigiGain_BHF_Rx = -1;
            m_nSDigiGain_BHF_Tx = -1;

            m_nSettingPTHF_Contact_TH_Rx = -1;
            m_nSettingPTHF_Contact_TH_Tx = -1;
            m_nSettingPTHF_Hover_TH_Rx = -1;
            m_nSettingPTHF_Hover_TH_Tx = -1;
            m_nSettingBHF_Contact_TH_Rx = -1;
            m_nSettingBHF_Contact_TH_Tx = -1;
            m_nSettingBHF_Hover_TH_Rx = -1;
            m_nSettingBHF_Hover_TH_Tx = -1;

            m_nReadcActivePen_FM_P0_TH = -1;
            m_nReadTRxS_Beacon_Hover_TH_Rx = -1;
            m_nReadTRxS_Beacon_Hover_TH_Tx = -1;
            m_nReadTRxS_Beacon_Contact_TH_Rx = -1;
            m_nReadTRxS_Beacon_Contact_TH_Tx = -1;
            m_nReadBeacon_Hover_TH_Rx = -1;
            m_nReadBeacon_Hover_TH_Tx = -1;
            m_nReadBeacon_Contact_TH_Rx = -1;
            m_nReadBeacon_Contact_TH_Tx = -1;

            m_nREdge_1Trc_SubPwr = -1;
            m_nREdge_2Trc_SubPwr = -1;
            m_nREdge_3Trc_SubPwr = -1;
            m_nREdge_4Trc_SubPwr = -1;

            m_nRDigiGain_P0 = -1;
            m_nRDigiGain_Beacon_Rx = -1;
            m_nRDigiGain_Beacon_Tx = -1;
            m_nRDigiGain_PTHF_Rx = -1;
            m_nRDigiGain_PTHF_Tx = -1;
            m_nRDigiGain_BHF_Rx = -1;
            m_nRDigiGain_BHF_Tx = -1;

            m_nReadPTHF_Contact_TH_Rx = -1;
            m_nReadPTHF_Contact_TH_Tx = -1;
            m_nReadPTHF_Hover_TH_Rx = -1;
            m_nReadPTHF_Hover_TH_Tx = -1;
            m_nReadBHF_Contact_TH_Rx = -1;
            m_nReadBHF_Contact_TH_Tx = -1;
            m_nReadBHF_Hover_TH_Rx = -1;
            m_nReadBHF_Hover_TH_Tx = -1;

            m_nPressureWeight = -1;
            m_nStartWeight = -1;
            m_nEndWeight = -1;
            m_nRealityWeight = -1;
            m_nOffsetWeight = -1;
            m_nExtraIncWeight = -1;
            m_nPTPenVersion = -1;
            m_nTotalWeight = -1;

            m_nROrgIQ_BSH_P = -1;
            m_nSIQ_BSH_P = -1;
            m_nSPressure3BinsTH = -1;
            m_nS3BinsPwr = -1;
            m_nRIQ_BSH_P = -1;
            m_nRPressure3BinsTH = -1;
            m_nR3BinsPwr = -1;
            m_nPress_MaxDFTRxMean = -1;
            m_nBefIQ_BSH_P = -1;
            m_nBefPress_MaxDFTRxMean = -1;

            m_sSubStep = "";
            m_sFWCheckVersion = "";
            m_sFrequency = "";
            m_sHoverRaiseHeight = "";
            m_sP0_detect_time = "";
            m_sDrawLineType = "";
            m_sControlMode = "";
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="cfrmMain">frmMain表單控制項</param>
        /// <param name="cRecordSetParameter">階段資訊參數</param>
        /// <param name="bGetStepInfoFlag_DigiGainTuning">是否有取得DigiGain Tuning的階段資訊</param>
        /// <param name="nICSolutionType">IC Solution種類</param>
        public void SetParameterValue(frmMain cfrmMain, RecordSetParameter cRecordSetParameter, bool bGetStepInfoFlag_DigiGainTuning, int nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_NONE)
        {
            MainTuningStep eMainStep = cRecordSetParameter.m_eMainStep;
            SubTuningStep eSubStep = cRecordSetParameter.m_eSubStep;

            m_nSettingPH1 = cRecordSetParameter.m_nPH1;
            m_nSettingPH2 = cRecordSetParameter.m_nPH2;

            CheckState cCheckState = new CheckState(cfrmMain);

            if (cCheckState.CheckIndependentStep(eMainStep, eSubStep) != CheckState.m_nSTEPSTATE_NORMAL)
                m_nRankIndex = 1;
            else
                m_nRankIndex = cRecordSetParameter.m_nRankIndex;

            if ((eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO) && nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
            {
                m_nTraceType = cRecordSetParameter.m_nTraceType;

                if (eMainStep == MainTuningStep.TILTNO)
                    m_nSection = cRecordSetParameter.m_nSection;
            }

            m_nNoiseP0_Detect_Time_Idx = cRecordSetParameter.m_nNoiseP0_DetectTime_Idx;
            m_nNoiseRXInnerMax = cRecordSetParameter.m_nNoiseRXInnerMax;
            m_nNoiseTXInnerMax = cRecordSetParameter.m_nNoiseTXInnerMax;

            m_nNoiseRXInMaxPlus3InMaxSTD = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
            m_nNoiseTXInMaxPlus3InMaxSTD = cRecordSetParameter.m_nNoiseTXInMaxPlus3InMaxSTD;

            m_nNoiseTrcMaxMinusPreP0_TH_1Trc = cRecordSetParameter.m_nNoiseTrcMaxMinusPreP0_TH_1Trc;
            m_nNoiseTrcMaxMinusPreP0_TH_2Trc = cRecordSetParameter.m_nNoiseTrcMaxMinusPreP0_TH_2Trc;
            m_nNoiseTrcMaxMinusPreP0_TH_3Trc = cRecordSetParameter.m_nNoiseTrcMaxMinusPreP0_TH_3Trc;
            m_nNoiseTrcMaxMinusPreP0_TH_4Trc = cRecordSetParameter.m_nNoiseTrcMaxMinusPreP0_TH_4Trc;

            if (cCheckState.CheckIndependentStep(eMainStep, eSubStep) != CheckState.m_nSTEPSTATE_NORMAL)
                m_nSetDigiGainType = m_nSETDIGIGAINTYPE_NONE;
            else
            {
                if (cCheckState.CheckSetDigiGain(eMainStep, eSubStep) == CheckState.m_nSETDIGIGAIN_FIXEDVALUE)
                    m_nSetDigiGainType = m_nSETDIGIGAINTYPE_DEFAULT;
                else if (cCheckState.CheckSetDigiGain(eMainStep, eSubStep) == CheckState.m_nSETDIGIGAIN_COMPUTEVALUE)
                    m_nSetDigiGainType = (bGetStepInfoFlag_DigiGainTuning == true) ? m_nSETDIGIGAINTYPE_COMPUTEVALUE : m_nSETDIGIGAINTYPE_DEFAULT;
            }

            if (m_nSetDigiGainType == m_nSETDIGIGAINTYPE_NONE)
            {
                m_nSDigiGain_P0 = -1;
                m_nSDigiGain_Beacon_Rx = -1;
                m_nSDigiGain_Beacon_Tx = -1;
                m_nSDigiGain_PTHF_Rx = -1;
                m_nSDigiGain_PTHF_Tx = -1;
                m_nSDigiGain_BHF_Rx = -1;
                m_nSDigiGain_BHF_Tx = -1;
            }
            else if (m_nSetDigiGainType == m_nSETDIGIGAINTYPE_DEFAULT)
            {
                m_nSDigiGain_P0 = SetDigiGain(ParamAutoTuning.m_nDefault_cActivePen_DigiGain_P0);
                m_nSDigiGain_Beacon_Rx = SetDigiGain(ParamAutoTuning.m_nDefault_cActivePen_DigiGain_Beacon_Rx);
                m_nSDigiGain_Beacon_Tx = SetDigiGain(ParamAutoTuning.m_nDefault_cActivePen_DigiGain_Beacon_Tx);
                m_nSDigiGain_PTHF_Rx = SetDigiGain(ParamAutoTuning.m_nDefault_cActivePen_DigiGain_PTHF_Rx);
                m_nSDigiGain_PTHF_Tx = SetDigiGain(ParamAutoTuning.m_nDefault_cActivePen_DigiGain_PTHF_Tx);
                m_nSDigiGain_BHF_Rx = SetDigiGain(ParamAutoTuning.m_nDefault_cActivePen_DigiGain_BHF_Rx);
                m_nSDigiGain_BHF_Tx = SetDigiGain(ParamAutoTuning.m_nDefault_cActivePen_DigiGain_BHF_Tx);
            }
            else if (m_nSetDigiGainType == m_nSETDIGIGAINTYPE_COMPUTEVALUE)
            {
                m_nSDigiGain_P0 = ElanConvert.ConvertScaleToDigiGain(SetDigiGainScale(cRecordSetParameter.m_nDigiGain_P0));
                m_nSDigiGain_Beacon_Rx = ElanConvert.ConvertScaleToDigiGain(SetDigiGainScale(cRecordSetParameter.m_nDigiGain_Beacon_Rx));
                m_nSDigiGain_Beacon_Tx = ElanConvert.ConvertScaleToDigiGain(SetDigiGainScale(cRecordSetParameter.m_nDigiGain_Beacon_Tx));
                m_nSDigiGain_PTHF_Rx = ElanConvert.ConvertScaleToDigiGain(SetDigiGainScale(cRecordSetParameter.m_nDigiGain_PTHF_Rx));
                m_nSDigiGain_PTHF_Tx = ElanConvert.ConvertScaleToDigiGain(SetDigiGainScale(cRecordSetParameter.m_nDigiGain_PTHF_Tx));
                m_nSDigiGain_BHF_Rx = ElanConvert.ConvertScaleToDigiGain(SetDigiGainScale(cRecordSetParameter.m_nDigiGain_BHF_Rx));
                m_nSDigiGain_BHF_Tx = ElanConvert.ConvertScaleToDigiGain(SetDigiGainScale(cRecordSetParameter.m_nDigiGain_BHF_Tx));
            }

            if (eMainStep == MainTuningStep.DIGITALTUNING && eSubStep == SubTuningStep.CONTACT)
            {
                if (ParamAutoTuning.m_nContactStepFilterType == 1)
                {
                    m_nRXFilterValue = cRecordSetParameter.m_nHover1stRXMedian;
                    m_nTXFilterValue = cRecordSetParameter.m_nHover1stTXMedian;
                }
                else if (ParamAutoTuning.m_nContactStepFilterType == 2)
                {
                    m_nRXFilterValue = ParamAutoTuning.m_nContactStepFilterRXValue;
                    m_nTXFilterValue = ParamAutoTuning.m_nContactStepFilterTXValue;
                }
                else if (ParamAutoTuning.m_nContactStepFilterType == 3)
                {
                    m_nRXFilterValue = cRecordSetParameter.m_nHover1stRXMax;
                    m_nTXFilterValue = cRecordSetParameter.m_nHover1stTXMax;
                }
                else
                {
                    if (cRecordSetParameter.m_nNoiseP0_DetectTime_Idx == 1)
                    {
                        m_nRXFilterValue = (int)(cRecordSetParameter.m_nNoiseRXInnerMax * ParamAutoTuning.m_dNormal800to400PwrRatio);
                        m_nTXFilterValue = (int)(cRecordSetParameter.m_nNoiseTXInnerMax * ParamAutoTuning.m_dNormal800to400PwrRatio);
                    }
                    else
                    {
                        m_nRXFilterValue = cRecordSetParameter.m_nNoiseRXInnerMax;
                        m_nTXFilterValue = cRecordSetParameter.m_nNoiseTXInnerMax;
                    }
                }

                m_nNoiseDigiGain_P0 = cRecordSetParameter.m_nNoiseDigiGain_P0;
                m_nNoiseDigiGain_Beacon_Rx = cRecordSetParameter.m_nNoiseDigiGain_Beacon_Rx;
                m_nNoiseDigiGain_Beacon_Tx = cRecordSetParameter.m_nNoiseDigiGain_Beacon_Tx;
            }
            else if (eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVER_1ST || eSubStep == SubTuningStep.HOVER_2ND))
            {
                if (cRecordSetParameter.m_nNoiseP0_DetectTime_Idx == 1)
                {
                    m_nRXFilterValue = (int)(cRecordSetParameter.m_nNoiseRXInnerMax * ParamAutoTuning.m_dNormal800to400PwrRatio);
                    m_nTXFilterValue = (int)(cRecordSetParameter.m_nNoiseTXInnerMax * ParamAutoTuning.m_dNormal800to400PwrRatio);
                }
                else
                {
                    m_nRXFilterValue = cRecordSetParameter.m_nNoiseRXInnerMax;
                    m_nTXFilterValue = cRecordSetParameter.m_nNoiseTXInnerMax;
                }

                m_nNoiseDigiGain_P0 = cRecordSetParameter.m_nNoiseDigiGain_P0;
                m_nNoiseDigiGain_Beacon_Rx = cRecordSetParameter.m_nNoiseDigiGain_Beacon_Rx;
                m_nNoiseDigiGain_Beacon_Tx = cRecordSetParameter.m_nNoiseDigiGain_Beacon_Tx;
            }
            else if (eMainStep == MainTuningStep.TPGAINTUNING)
            {
                m_nNoiseDigiGain_P0 = cRecordSetParameter.m_nNoiseDigiGain_P0;
                m_nNoiseDigiGain_Beacon_Rx = cRecordSetParameter.m_nNoiseDigiGain_Beacon_Rx;
                m_nNoiseDigiGain_Beacon_Tx = cRecordSetParameter.m_nNoiseDigiGain_Beacon_Tx;
            }
            else
            {
                m_nRXFilterValue = cRecordSetParameter.m_nNoiseRXInnerMax;
                m_nTXFilterValue = cRecordSetParameter.m_nNoiseTXInnerMax;

                m_nNoiseDigiGain_Beacon_Rx = cRecordSetParameter.m_nNoiseDigiGain_Beacon_Rx;
            }

            ComputeFilterValueByDigiGainRatio();

            if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
            {
                m_nSettingcActivePen_FM_P0_TH = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
                m_nSettingTRxS_Beacon_Hover_TH_Rx = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
                m_nSettingTRxS_Beacon_Hover_TH_Tx = cRecordSetParameter.m_nNoiseTXInMaxPlus3InMaxSTD;
                m_nSettingTRxS_Beacon_Contact_TH_Rx = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
                m_nSettingTRxS_Beacon_Contact_TH_Tx = cRecordSetParameter.m_nNoiseTXInMaxPlus3InMaxSTD;

                m_nSEdge_1Trc_SubPwr = cRecordSetParameter.m_nNoiseTrcMaxMinusPreP0_TH_1Trc;
                m_nSEdge_2Trc_SubPwr = cRecordSetParameter.m_nNoiseTrcMaxMinusPreP0_TH_2Trc;
                m_nSEdge_3Trc_SubPwr = cRecordSetParameter.m_nNoiseTrcMaxMinusPreP0_TH_3Trc;
                m_nSEdge_4Trc_SubPwr = cRecordSetParameter.m_nNoiseTrcMaxMinusPreP0_TH_4Trc;

                m_nSettingBeacon_Hover_TH_Rx = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
                m_nSettingBeacon_Hover_TH_Tx = cRecordSetParameter.m_nNoiseTXInMaxPlus3InMaxSTD;
                m_nSettingBeacon_Contact_TH_Rx = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
                m_nSettingBeacon_Contact_TH_Tx = cRecordSetParameter.m_nNoiseTXInMaxPlus3InMaxSTD;

                m_nSettingPTHF_Contact_TH_Rx = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
                m_nSettingPTHF_Contact_TH_Tx = cRecordSetParameter.m_nNoiseTXInMaxPlus3InMaxSTD;
                m_nSettingPTHF_Hover_TH_Rx = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
                m_nSettingPTHF_Hover_TH_Tx = cRecordSetParameter.m_nNoiseTXInMaxPlus3InMaxSTD;

                m_nSettingBHF_Contact_TH_Rx = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
                m_nSettingBHF_Contact_TH_Tx = cRecordSetParameter.m_nNoiseTXInMaxPlus3InMaxSTD;
                m_nSettingBHF_Hover_TH_Rx = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
                m_nSettingBHF_Hover_TH_Tx = cRecordSetParameter.m_nNoiseTXInMaxPlus3InMaxSTD;

                if (ParamAutoTuning.m_nFWTypeIndex != 1)
                    m_nSPen_HI_HF_THD = cRecordSetParameter.m_nNoiseRXInMaxPlus3InMaxSTD;

                if (cRecordSetParameter.m_eMainStep == MainTuningStep.TPGAINTUNING)
                    ComputeThresholdParameterByDigiGainRatio();
            }
            else if ((eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)) ||
                     eMainStep == MainTuningStep.TILTTUNING ||
                     eMainStep == MainTuningStep.PRESSURETUNING ||
                     eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                m_nSettingcActivePen_FM_P0_TH = cRecordSetParameter.m_ncActivePen_FM_P0_TH;
                m_nSettingTRxS_Beacon_Hover_TH_Rx = cRecordSetParameter.m_nTRxS_Beacon_Hover_TH_Rx;
                m_nSettingTRxS_Beacon_Hover_TH_Tx = cRecordSetParameter.m_nTRxS_Beacon_Hover_TH_Tx;
                m_nSettingTRxS_Beacon_Contact_TH_Rx = cRecordSetParameter.m_nTRxS_Beacon_Contact_TH_Rx;
                m_nSettingTRxS_Beacon_Contact_TH_Tx = cRecordSetParameter.m_nTRxS_Beacon_Contact_TH_Tx;

                /*
                if (m_nSettingcActivePen_FM_P0_TH < m_nSettingTRxS_Beacon_Hover_TH_Rx)
                    m_nSettingcActivePen_FM_P0_TH = m_nSettingTRxS_Beacon_Hover_TH_Rx;
                */

                if (ParamAutoTuning.m_nFWTypeIndex != 1)
                    m_nSPen_HI_HF_THD = cRecordSetParameter.m_ncActivePen_FM_P0_TH;

                if (eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS))
                {
                    m_nRXTraceNumber = cRecordSetParameter.m_nRX_TraceNumber;
                    m_nTXTraceNumber = cRecordSetParameter.m_nTX_TraceNumber;
                    m_nSEdge_1Trc_SubPwr = cRecordSetParameter.m_nEdge_1Trc_SubPwr;
                    m_nSEdge_2Trc_SubPwr = cRecordSetParameter.m_nEdge_2Trc_SubPwr;
                    m_nSEdge_3Trc_SubPwr = cRecordSetParameter.m_nEdge_3Trc_SubPwr;
                    m_nSEdge_4Trc_SubPwr = cRecordSetParameter.m_nEdge_4Trc_SubPwr;
                }

                if (eMainStep == MainTuningStep.TILTTUNING || eMainStep == MainTuningStep.PRESSURETUNING || eMainStep == MainTuningStep.LINEARITYTUNING)
                {
                    m_nRXTraceNumber = cRecordSetParameter.m_nRX_TraceNumber;
                    m_nTXTraceNumber = cRecordSetParameter.m_nTX_TraceNumber;
                    m_nSettingBeacon_Hover_TH_Rx = cRecordSetParameter.m_nBeacon_Hover_TH_Rx;
                    m_nSettingBeacon_Hover_TH_Tx = cRecordSetParameter.m_nBeacon_Hover_TH_Tx;
                    m_nSettingBeacon_Contact_TH_Rx = cRecordSetParameter.m_nBeacon_Contact_TH_Rx;
                    m_nSettingBeacon_Contact_TH_Tx = cRecordSetParameter.m_nBeacon_Contact_TH_Tx;

                    if (eSubStep == SubTuningStep.TILTTUNING_PTHF)
                    {
                        m_nSettingPTHF_Contact_TH_Rx = cRecordSetParameter.m_nPTHF_Contact_TH_Rx;
                        m_nSettingPTHF_Contact_TH_Tx = cRecordSetParameter.m_nPTHF_Contact_TH_Tx;
                        m_nSettingPTHF_Hover_TH_Rx = cRecordSetParameter.m_nPTHF_Hover_TH_Rx;
                        m_nSettingPTHF_Hover_TH_Tx = cRecordSetParameter.m_nPTHF_Hover_TH_Tx;
                    }
                    else if (eSubStep == SubTuningStep.TILTTUNING_BHF)
                    {
                        m_nSettingBHF_Contact_TH_Rx = cRecordSetParameter.m_nBHF_Contact_TH_Rx;
                        m_nSettingBHF_Contact_TH_Tx = cRecordSetParameter.m_nBHF_Contact_TH_Tx;
                        m_nSettingBHF_Hover_TH_Rx = cRecordSetParameter.m_nBHF_Hover_TH_Rx;
                        m_nSettingBHF_Hover_TH_Tx = cRecordSetParameter.m_nBHF_Hover_TH_Tx;
                    }
                    else if (eSubStep == SubTuningStep.PRESSURETABLE)
                    {
                        m_nSIQ_BSH_P = cRecordSetParameter.m_nIQ_BSH_P;
                        m_nSPressure3BinsTH = cRecordSetParameter.m_nPressure3BinsTH;
                        m_nS3BinsPwr = cRecordSetParameter.m_nPress_3BinsPwr;
                    }
                    else if (eSubStep == SubTuningStep.LINEARITYTABLE)
                    {
                        m_nSettingPTHF_Contact_TH_Rx = cRecordSetParameter.m_nPTHF_Contact_TH_Rx;
                        m_nSettingPTHF_Contact_TH_Tx = cRecordSetParameter.m_nPTHF_Contact_TH_Tx;
                        m_nSettingPTHF_Hover_TH_Rx = cRecordSetParameter.m_nPTHF_Hover_TH_Rx;
                        m_nSettingPTHF_Hover_TH_Tx = cRecordSetParameter.m_nPTHF_Hover_TH_Tx;

                        m_nSettingBHF_Contact_TH_Rx = cRecordSetParameter.m_nBHF_Contact_TH_Rx;
                        m_nSettingBHF_Contact_TH_Tx = cRecordSetParameter.m_nBHF_Contact_TH_Tx;
                        m_nSettingBHF_Hover_TH_Rx = cRecordSetParameter.m_nBHF_Hover_TH_Rx;
                        m_nSettingBHF_Hover_TH_Tx = cRecordSetParameter.m_nBHF_Hover_TH_Tx;
                    }
                }
            }

            if (ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH >= 0)
            {
                m_nSettingcActivePen_FM_P0_TH = ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH;
                m_nSPen_HI_HF_THD = ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH;
            }
        }

        /// <summary>
        /// 設定RX/TX Trace數
        /// </summary>
        /// <param name="nRXTraceNumber">RX Trace數</param>
        /// <param name="nTXTraceNumber">TX Trace數</param>
        public void SetRXTXTraceNumber(int nRXTraceNumber, int nTXTraceNumber)
        {
            m_nRXTraceNumber = nRXTraceNumber;
            m_nTXTraceNumber = nTXTraceNumber;
        }

        /// <summary>
        /// 設定DigiGainScale
        /// </summary>
        /// <param name="nScaleValue">Scale數值</param>
        /// <returns>回傳DigiGainScale</returns>
        private int SetDigiGainScale(int nScaleValue)
        {
            if (nScaleValue > 399)
                return 399;
            else if (nScaleValue < 1)
                return 1;

            return nScaleValue;
        }

        /// <summary>
        /// 設定DigiGain
        /// </summary>
        /// <param name="nDigiGainValue">DigiGain數值</param>
        /// <returns>回傳DigiGain</returns>
        private int SetDigiGain(int nDigiGainValue)
        {
            int nHB = (int)((double)399 * ParamAutoTuning.m_nDGTMultiplyValue / ParamAutoTuning.m_nDGTDividValue);
            int nLB = (int)((double)1 * ParamAutoTuning.m_nDGTMultiplyValue / ParamAutoTuning.m_nDGTDividValue);

            if (nDigiGainValue > nHB)
                return nHB;
            else if (nDigiGainValue < nLB)
                return nLB;

            return nDigiGainValue;
        }

        /// <summary>
        /// 應用DigiGain Ration計算過濾值
        /// </summary>
        private void ComputeFilterValueByDigiGainRatio()
        {
            if (m_nNoiseDigiGain_Beacon_Rx != -1 && m_nSDigiGain_Beacon_Rx != -1)
            {
                double dDigiGainRatio = (double)m_nSDigiGain_Beacon_Rx / (double)m_nNoiseDigiGain_Beacon_Rx;
                dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                m_nRXFilterValue = (int)(m_nRXFilterValue * dDigiGainRatio);
            }

            if (m_nNoiseDigiGain_Beacon_Tx != -1 && m_nSDigiGain_Beacon_Tx != -1)
            {
                double dDigiGainRatio = (double)m_nSDigiGain_Beacon_Tx / (double)m_nNoiseDigiGain_Beacon_Tx;
                dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                m_nTXFilterValue = (int)(m_nTXFilterValue * dDigiGainRatio);
            }
        }

        /// <summary>
        /// 應用DigiGain Ration計算門檻值
        /// </summary>
        private void ComputeThresholdParameterByDigiGainRatio()
        {
            if (m_nNoiseDigiGain_P0 != -1)
            {
                if (m_nSDigiGain_P0 != -1)
                {
                    double dDigiGainRatio = (double)m_nSDigiGain_P0 / (double)m_nNoiseDigiGain_Beacon_Rx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nSettingcActivePen_FM_P0_TH = (int)(m_nSettingcActivePen_FM_P0_TH * dDigiGainRatio);
                    m_nSPen_HI_HF_THD = (int)(m_nSPen_HI_HF_THD * dDigiGainRatio);
                }
            }

            if (m_nNoiseDigiGain_Beacon_Rx != -1)
            {
                if (m_nSDigiGain_Beacon_Rx != -1)
                {
                    double dDigiGainRatio = (double)m_nSDigiGain_Beacon_Rx / (double)m_nNoiseDigiGain_Beacon_Rx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nSettingTRxS_Beacon_Hover_TH_Rx = (int)(m_nSettingTRxS_Beacon_Hover_TH_Rx * dDigiGainRatio);
                    m_nSettingTRxS_Beacon_Contact_TH_Rx = (int)(m_nSettingTRxS_Beacon_Contact_TH_Rx * dDigiGainRatio);
                    m_nSEdge_1Trc_SubPwr = (int)(m_nSEdge_1Trc_SubPwr * dDigiGainRatio);
                    m_nSEdge_2Trc_SubPwr = (int)(m_nSEdge_2Trc_SubPwr * dDigiGainRatio);
                    m_nSEdge_3Trc_SubPwr = (int)(m_nSEdge_3Trc_SubPwr * dDigiGainRatio);
                    m_nSEdge_4Trc_SubPwr = (int)(m_nSEdge_4Trc_SubPwr * dDigiGainRatio);
                    m_nSettingBeacon_Hover_TH_Rx = (int)(m_nSettingBeacon_Hover_TH_Rx * dDigiGainRatio);
                    m_nSettingBeacon_Contact_TH_Rx = (int)(m_nSettingBeacon_Contact_TH_Rx * dDigiGainRatio);
                }

                if (m_nSDigiGain_PTHF_Rx != -1)
                {
                    double dDigiGainRatio = (double)m_nSDigiGain_PTHF_Rx / (double)m_nNoiseDigiGain_Beacon_Rx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nSettingPTHF_Contact_TH_Rx = (int)(m_nSettingPTHF_Contact_TH_Rx * dDigiGainRatio);
                    m_nSettingPTHF_Hover_TH_Rx = (int)(m_nSettingPTHF_Hover_TH_Rx * dDigiGainRatio);
                }

                if (m_nSDigiGain_BHF_Rx != -1)
                {
                    double dDigiGainRatio = (double)m_nSDigiGain_BHF_Rx / (double)m_nNoiseDigiGain_Beacon_Rx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nSettingBHF_Contact_TH_Rx = (int)(m_nSettingBHF_Contact_TH_Rx * dDigiGainRatio);
                    m_nSettingBHF_Hover_TH_Rx = (int)(m_nSettingBHF_Hover_TH_Rx * dDigiGainRatio);
                }
            }

            if (m_nNoiseDigiGain_Beacon_Tx != -1)
            {
                if (m_nSDigiGain_Beacon_Tx != -1)
                {
                    double dDigiGainRatio = (double)m_nSDigiGain_Beacon_Tx / (double)m_nNoiseDigiGain_Beacon_Tx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nSettingTRxS_Beacon_Hover_TH_Tx = (int)(m_nSettingTRxS_Beacon_Hover_TH_Tx * dDigiGainRatio);
                    m_nSettingTRxS_Beacon_Contact_TH_Tx = (int)(m_nSettingTRxS_Beacon_Contact_TH_Tx * dDigiGainRatio);
                    m_nSettingBeacon_Hover_TH_Tx = (int)(m_nSettingBeacon_Hover_TH_Tx * dDigiGainRatio);
                    m_nSettingBeacon_Contact_TH_Tx = (int)(m_nSettingBeacon_Contact_TH_Tx * dDigiGainRatio);
                }

                if (m_nSDigiGain_PTHF_Tx != -1)
                {
                    double dDigiGainRatio = (double)m_nSDigiGain_PTHF_Tx / (double)m_nNoiseDigiGain_Beacon_Tx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nSettingPTHF_Contact_TH_Tx = (int)(m_nSettingPTHF_Contact_TH_Tx * dDigiGainRatio);
                    m_nSettingPTHF_Hover_TH_Tx = (int)(m_nSettingPTHF_Hover_TH_Tx * dDigiGainRatio);
                }

                if (m_nSDigiGain_BHF_Tx != -1)
                {
                    double dDigiGainRatio = (double)m_nSDigiGain_BHF_Tx / (double)m_nNoiseDigiGain_Beacon_Tx;
                    dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                    m_nSettingBHF_Contact_TH_Tx = (int)(m_nSettingBHF_Contact_TH_Tx * dDigiGainRatio);
                    m_nSettingBHF_Hover_TH_Tx = (int)(m_nSettingBHF_Hover_TH_Tx * dDigiGainRatio);
                }
            }
        }
    }

    /// <summary>
    /// Robot Parameter Class
    /// </summary>
    public class RobotParameter
    {
        public FlowRobot m_eRobotType = FlowRobot.NO;
        public FlowRecord m_eRecordType = FlowRecord.NTRX;
        public int m_nPressureWeight = 0;
        public int m_nFlowIndex = 0;
        public FlowStep m_cFlowStep = null;
        public bool m_bLastRetryFlag = false;
        public int m_nTraceType = MainConstantParameter.m_nTRACETYPE_NONE;
        public int m_nSection = -1;

        public RobotParameter(FlowStep cFlowStep, RecordSetParameter cRecordSetParameter, int nFlowIndex, bool bLastRetryFlag)
        {
            m_eRobotType = cRecordSetParameter.m_eRobot;
            m_eRecordType = cRecordSetParameter.m_eRecord;
            m_nPressureWeight = cRecordSetParameter.m_nWeight;
            m_nTraceType = cRecordSetParameter.m_nTraceType;
            m_nSection = cRecordSetParameter.m_nSection;
            m_nFlowIndex = nFlowIndex;
            m_cFlowStep = cFlowStep;
            m_bLastRetryFlag = bLastRetryFlag;
        }
    }

    /// <summary>
    /// MainConstantParameter Class
    /// </summary>
    public class MainConstantParameter
    {
        //Sent to the window that is getting raw input.
        //A window receives this message through its WindowProc function.
        public const int m_nWM_INPUT = 255;
        //IC Clock Frequency
        public const int m_nICCLOCKFREQUENCY = 32000000;
        //Flow File List Item Length
        public const int m_nFLOWLISTITEM_LENGTH = 4;

        public const int m_nNLAPPLUS2 = 2;

        //Mode Constant Parameter
        public const int m_nMODE_DEBUG      = 0;
        public const int m_nMODE_SERVER     = 1;
        public const int m_nMODE_CLIENT     = 2;
        public const int m_nMODE_GODRAW     = 3;
        public const int m_nMODE_SINGLE     = 4;
        public const int m_nMODE_LOADDATA   = 5;

        //Mode Constant Parameter
        public const string m_sMODE_DEBUG       = "Debug";
        public const string m_sMODE_SERVER      = "Server";
        public const string m_sMODE_CLIENT      = "Client";
        public const string m_sMODE_GODRAW      = "GoDraw";
        public const string m_sMODE_SINGLE      = "Single";
        public const string m_sMODE_LOADDATA    = "LoadData";

        //Draw Line Type Constant Parameter
        public const int m_nDRAWTYPE_NONE                       = 0;
        public const int m_nDRAWTYPE_SLANTLINE                  = 1;
        public const int m_nDRAWTYPE_HORIZONTALLINE             = 2;
        public const int m_nDRAWTYPE_VERTICALLINE               = 3;
        public const int m_nDRAWTYPE_CENTERPOINT                = 4;
        public const int m_nDRAWTYPE_HORIZONTALLONGLINE         = 5;
        public const int m_nDRAWTYPE_VERTICALLONGLINE           = 6;
        public const int m_nDRAWTYPE_HORIZONTALMIDDLELONGLINE   = 7;
        public const int m_nDRAWTYPE_VERTICALMIDDLELONGLINE     = 8;

        //SubStep Location Constant Parameter
        public const int m_nSTEPLOCATION_FIRST  = 1;
        public const int m_nSTEPLOCATION_LAST   = 2;

        //Skip Load File Process Constant Parameter
        public const int m_nSKIPFILE_FLOWTXT        = 0x0001;
        public const int m_nSKIPFILE_STEPLISTCSV    = 0x0002;

        //Draw State Constant Parameter
        public const int m_nDRAWSTATE_READYTODRAW   = 0;
        public const int m_nDRAWSTATE_STARTTODRAW   = 1;
        public const int m_nDRAWSTATE_FINISH        = 2;

        //Send Command Constant Parameter
        public const string m_sSENDCOMMAND_DISABLEREPORT            = "Disable Touch Report";
        public const string m_sSENDCOMMAND_ENABLEREPORT             = "Enable Touch Report";
        public const string m_sSENDCOMMAND_DISABLEFINGERREPORT      = "Disable Finger Touch Report";
        public const string m_sSENDCOMMAND_STOPGETSYNCREPORTDATA    = "Stop Get NoSync Report Data";
        public const string m_sSENDCOMMAND_RESETTILTNOISESTATE      = "Reset Tilt Noise State";
        public const string m_sSENDCOMMAND_RESETTILTSTATE           = "Reset Tilt State";
        public const string m_sSENDCOMMAND_SetRead_Bulk_RAM_Data    = "Set Read_Bulk_RAM_Data";
        public const string m_sSENDCOMMAND_StopGetGen8ReportData    = "Stop Get Gen8 Report Data";

        //Flow State Constant Parameter
        public const int m_nFLOWSTATE_INITIALIZE    = 0x00;
        public const int m_nFLOWSTATE_FIRSTSTEP     = 0x01;
        public const int m_nFLOWSTATE_SECONDSTEP    = 0x03;

        //Linearity Tuning Data Type Constant Parameter
        public const int m_nLTDATATYPE_RX5T     = 0;
        public const int m_nLTDATATYPE_TX5T     = 1;
        public const int m_nLTDATATYPE_RX3T     = 2;
        public const int m_nLTDATATYPE_TX3T     = 3;
        public const int m_nLTDATATYPE_RX2TLE   = 4;
        public const int m_nLTDATATYPE_TX2TLE   = 5;
        public const int m_nLTDATATYPE_RX2THE   = 6;
        public const int m_nLTDATATYPE_TX2THE   = 7;

        //Get Data Type Constant Parameter
        public const int m_nGETDATATYPE_NONESYNCRX_400US = 0x96;
        public const int m_nGETDATATYPE_NONESYNCTX_400US = 0x97;
        public const int m_nGETDATATYPE_NONESYNCRX_800US = 0xD6;
        public const int m_nGETDATATYPE_NONESYNCTX_800US = 0xD7;

        //Trace Type Constant Parameter
        public const int m_nTRACETYPE_NONE  = -1;
        public const int m_nTRACETYPE_RX    = 0;
        public const int m_nTRACETYPE_TX    = 1;

        //CPU Type
        public const string m_sCPUTYPE_INTEL    = "Intel";
        public const string m_sCPUTYPE_AMD      = "AMD";
        public const string m_sCPUTYPE_ELSE     = "Else";
        public const string m_sCPUTYPE_NONE     = "None";

        //GoDraw Controller Type
        public const string m_sGODRAWCTRLTYPE_GENERAL       = "General";
        public const string m_sGODRAWCTRLTYPE_NORMALXYAXIS  = "Normal X,Y Axis";
        public const string m_sGODRAWCTRLTYPE_NORMALZAXIS   = "Normal Z Axis";
        public const string m_sGODRAWCTRLTYPE_TPGTXYAXIS    = "TP_GainTuning X,Y Axis";
        public const string m_sGODRAWCTRLTYPE_TPGTZAXIS     = "TP_GainTuning Z Axis";

        //GoDraw Controller Parameter(General)
        public const string m_sGODRAWCTRLPARAMETER_GENERAL = "General Parameter";

        //GoDraw Controller Parameter(X, Y Axis)
        public const string m_sGODRAWCTRLPARAMETER_XYAXIS_START             = "Start(Top-Left)";
        public const string m_sGODRAWCTRLPARAMETER_XYAXIS_END               = "End(Bottom-Right)";
        public const string m_sGODRAWCTRLPARAMETER_XYAXIS_HORIZONTALSTART   = "HorizontalLine Start(Top-Left)";
        public const string m_sGODRAWCTRLPARAMETER_XYAXIS_HORIZONTALEND     = "HorizontalLine End(Bottom-Right)";
        public const string m_sGODRAWCTRLPARAMETER_XYAXIS_VERTICALSTART     = "VerticalLine Start(Top-Left)";
        public const string m_sGODRAWCTRLPARAMETER_XYAXIS_VERTICALEND       = "VerticalLine End(Bottom-Right)";

        //GoDraw Controller Parameter(Z Axis)
        public const string m_sGODRAWCTRLPARAMETER_ZAXIS_TOP            = "Top";
        public const string m_sGODRAWCTRLPARAMETER_ZAXIS_CONTACT        = "Contact";
        public const string m_sGODRAWCTRLPARAMETER_ZAXIS_DT_HOVER1ST    = "DigitalTuning Hover1st";
        public const string m_sGODRAWCTRLPARAMETER_ZAXIS_DT_HOVER2ND    = "DigitalTuning Hover2nd";
        public const string m_sGODRAWCTRLPARAMETER_ZAXIS_PCT_HOVER1ST   = "PeakCheckTuning Hover1st";
        public const string m_sGODRAWCTRLPARAMETER_ZAXIS_PCT_HOVER2ND   = "PeakCheckTuning Hover2nd";

        public static string[] sGoDrawControlType_Array = new string[]
        {
            m_sGODRAWCTRLTYPE_GENERAL,
            m_sGODRAWCTRLTYPE_NORMALXYAXIS,
            m_sGODRAWCTRLTYPE_NORMALZAXIS,
            m_sGODRAWCTRLTYPE_TPGTXYAXIS,
            m_sGODRAWCTRLTYPE_TPGTZAXIS
        };

        public static string[] sGoDrawControlParameter_General_Array = new string[]
        {
            m_sGODRAWCTRLPARAMETER_GENERAL
        };

        public static string[] sGoDrawControlParameter_NormalXYAxis_Array = new string[]
        {
            m_sGODRAWCTRLPARAMETER_XYAXIS_START,
            m_sGODRAWCTRLPARAMETER_XYAXIS_END
        };

        public static string[] sGoDrawControlParameter_TPGTXYAxis_Array = new string[]
        {
            m_sGODRAWCTRLPARAMETER_XYAXIS_HORIZONTALSTART,
            m_sGODRAWCTRLPARAMETER_XYAXIS_HORIZONTALEND,
            m_sGODRAWCTRLPARAMETER_XYAXIS_VERTICALSTART,
            m_sGODRAWCTRLPARAMETER_XYAXIS_VERTICALEND
        };

        public static string[] sGoDrawControlParameter_NormalZAxis_Array = new string[]
        {
            m_sGODRAWCTRLPARAMETER_ZAXIS_TOP,
            m_sGODRAWCTRLPARAMETER_ZAXIS_CONTACT,
            m_sGODRAWCTRLPARAMETER_ZAXIS_DT_HOVER1ST,
            m_sGODRAWCTRLPARAMETER_ZAXIS_DT_HOVER2ND,
            m_sGODRAWCTRLPARAMETER_ZAXIS_PCT_HOVER1ST,
            m_sGODRAWCTRLPARAMETER_ZAXIS_PCT_HOVER2ND
        };

        public static string[] sGoDrawControlParameter_TPGTZAxis_Array = new string[]
        {
            m_sGODRAWCTRLPARAMETER_ZAXIS_CONTACT
        };

        public const int m_nLTCOMPENSATE_DISABLE    = 0;
        public const int m_nLTCOMPENSATE_TIPGAIN    = 1;
        public const int m_nLTCOMPENSATE_RINGGAIN   = 2;

        public const int m_nICSOLUTIONTYPE_NONE     = -1;
        public const int m_nICSOLUTIONTYPE_OTHER    = 0;
        public const int m_nICSOLUTIONTYPE_GEN8     = 1;

        public static int[] m_nTraceType_Array = null;

        public static void SetGen8TraceType()
        {
            if (ParamAutoTuning.m_nGen8TraceType == 0)
            {
                m_nTraceType_Array = new int[]
                {
                    m_nTRACETYPE_RX
                };
            }
            else if (ParamAutoTuning.m_nGen8TraceType == 1)
            {
                m_nTraceType_Array = new int[]
                {
                    m_nTRACETYPE_TX
                };
            }
            else
            {
                m_nTraceType_Array = new int[]
                {
                    m_nTRACETYPE_RX,
                    m_nTRACETYPE_TX
                };
            }
        }

        public static Gen8Solution m_eGen8ICSolution = Gen8Solution.Solution_8F09;

        public static void SetGen8ICSolution(int nFWVersionHighByte)
        {
            if (nFWVersionHighByte == 0x81)
                m_eGen8ICSolution = Gen8Solution.Solution_8F09;
            else if (nFWVersionHighByte == 0x82)
                m_eGen8ICSolution = Gen8Solution.Solution_8F11;
            else if (nFWVersionHighByte == 0x83)
                m_eGen8ICSolution = Gen8Solution.Solution_8F18;
            else
                m_eGen8ICSolution = Gen8Solution.Solution_8F09;
        }

        public static string GetGen8ICSolution()
        {
            if (m_eGen8ICSolution == Gen8Solution.Solution_8F09)
                return "8F09";
            else if (m_eGen8ICSolution == Gen8Solution.Solution_8F11)
                return "8F11";
            else if (m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                return "8F18";
            else
                return "Other";
        }

        public const int m_nReportMaxNumber_Gen8 = 30;

        /*
        public const string m_sINTERFACE_I2C = "I2C";
        public const string m_sINTERFACE_HIDOVERI2C = "HIDOverI2C";
        public const string m_sINTERFACE_HIDOVERI2C_USB = "HIDOverI2CorUSB";
        public const string m_sINTERFACE_USB = "USB";
        public const string m_sINTERFACE_SPI = "SPI";
        */

        public const string m_sSPIINTERFACE_NA = "NA";
        public const string m_sSPIINTERFACE_MA_RISING_HALF = "SPI(MA_Rising_Half)";
        public const string m_sSPIINTERFACE_MA_FALLING_HALF = "SPI(MA_Falling_Half)";
        public const string m_sSPIINTERFACE_MA_RISING = "SPI(MA_Rising)";
        public const string m_sSPIINTERFACE_MA_FALLING = "SPI(MA_Falling)";

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

    /// <summary>
    /// Gen8 Solution Set
    /// </summary>
    public enum Gen8Solution
    {
        Solution_8F09,
        Solution_8F11,
        Solution_8F18
    }

    /// <summary>
    /// Pattern Setting Class
    /// </summary>
    public class PatternSetting
    {
        public int m_nPatternSettingIndex = 0;
        public int m_nColorIndex = -1;
        public string m_sPatternPath = "";
    }

    /// <summary>
    /// Flow Step Class
    /// </summary>
    public class FlowStep
    {
        public MainTuningStep m_eMainStep = MainTuningStep.ELSE;
        public SubTuningStep m_eSubStep = SubTuningStep.ELSE;
        public bool m_bLastStep = false;
        public string m_sFlowFileName = "";
        public bool m_bStepErrorFlag = true;
        public string m_sStepErrorMessage = "";
        public List<string> m_sDataFileName_List = new List<string>();
        //public List<int> m_nDataFileRank_List = new List<int>();
        public int m_nPatternType = MainConstantParameter.m_nDRAWTYPE_NONE;
        public bool m_bDrawLineByUser = false;
        public int m_nSubStepState = 0;
        public int m_nPressureWieght = 0;
        public int m_nParamIQ_BSH_P = -1;
        public string m_sDescription = "";

        public int m_nHours = -1;
        public int m_nMinutes = -1;
        public int m_nSeconds = -1;
    }

    /// <summary>
    /// Linearity Tuning的Trace項目 Class
    /// </summary>
    class TracePartData
    {
        public int m_nTraceIndex = -1;
        //public int m_nTraceRawDataCount = 0;
    }

    /// <summary>
    /// 數據分析所使用到的基本資訊 Class
    /// </summary>
    class DataAnalysisInfo
    {
        public string m_sFilePath = "";
        public FlowStep m_cFlowStep = null;
        public bool m_bLoadDataMode = false;
        public string m_sSelectFolderPath = "";
    }

    /// <summary>
    /// 設定讀取Report Data Command種類 Class
    /// </summary>
    public class GetDataType
    {
        public const int m_nDATATYPE_NA         = 0x0000;
        public const int m_nDATATYPE_NTRX_400us = 0x0001;
        public const int m_nDATATYPE_NTRX_800us = 0x1001;
        public const int m_nDATATYPE_NRX_400us  = 0x0002;
        public const int m_nDATATYPE_NRX_800us  = 0x1002;
        public const int m_nDATATYPE_NTX_400us  = 0x0003;
        public const int m_nDATATYPE_NTX_800us  = 0x1003;
        public const int m_nDATATYPE_RX_400us   = 0x0004;
        public const int m_nDATATYPE_RX_800us   = 0x1004;
        public const int m_nDATATYPE_TX_400us   = 0x0005;
        public const int m_nDATATYPE_TX_800us   = 0x1005;
        public const int m_nDATATYPE_TRX_400us  = 0x0006;
        public const int m_nDATATYPE_TRX_800us  = 0x1006;
        public const int m_nDATATYPE_TRxS       = 0x1007;
        public const int m_nDATATYPE_TILT_PTHF  = 0x2008;
        public const int m_nDATATYPE_TILT_BHF   = 0x2009;
        public const int m_nDATATYPE_PRESSURE   = 0x400A;
        public const int m_nDATATYPE_LINEARITY  = 0x800B;
        public const int m_nDATATYPE_DIGIGAIN   = 0x1000C;
        public const int m_nDATATYPE_5TRAWDATA  = 0x0000D;

        public const int m_nDATATYPE_PTHF_NoSync_Gen8   = 0x0000E;
        public const int m_nDATATYPE_BHF_NoSync_Gen8    = 0x0000F;

        public const int m_nDETECTTIME_800us        = 0x1000;
        public const int m_nGETDATATYPE_TILT        = 0x2000;
        public const int m_nGETDATATYPE_PRESSURE    = 0x4000;
        public const int m_nGETDATATYPE_LINEARITY   = 0x8000;
        public const int m_nGETDATATYPE_DIGIGAIN    = 0x10000;

        /// <summary>
        /// 設定讀取Report Data Command種類
        /// </summary>
        /// <param name="cRecordSetParameter">階段資訊參數</param>
        /// <returns>回傳Data Type</returns>
        public int SetGetDataType_Gen6or7(RecordSetParameter cRecordSetParameter)
        {
            MainTuningStep eMainStep = cRecordSetParameter.m_eMainStep;
            SubTuningStep eSubStep = cRecordSetParameter.m_eSubStep;

            int nGetDataType = m_nDATATYPE_NA;

            FlowRecord eRecordAction = cRecordSetParameter.m_eRecord;

            if (eMainStep != MainTuningStep.TILTTUNING)
            {
                switch (eRecordAction)
                {
                    case FlowRecord.NTRX:
                        nGetDataType = m_nDATATYPE_NTRX_400us;
                        break;
                    case FlowRecord.NRX:
                        nGetDataType = m_nDATATYPE_NRX_400us;
                        break;
                    case FlowRecord.NTX:
                        nGetDataType = m_nDATATYPE_NTX_400us;
                        break;
                    case FlowRecord.RX:
                        nGetDataType = m_nDATATYPE_RX_400us;
                        break;
                    case FlowRecord.TX:
                        nGetDataType = m_nDATATYPE_TX_400us;
                        break;
                    case FlowRecord.TRX:
                        nGetDataType = m_nDATATYPE_TRX_400us;
                        break;
                    case FlowRecord.TRxS:
                        nGetDataType = m_nDATATYPE_TRxS;
                        break;
                    case FlowRecord.PRESSURE:
                        nGetDataType = m_nDATATYPE_PRESSURE;
                        break;
                    case FlowRecord.LINEARITY:
                        if (ParamAutoTuning.m_n5TRawDataType == 1)
                            nGetDataType = m_nDATATYPE_5TRAWDATA;
                        else
                            nGetDataType = m_nDATATYPE_LINEARITY;

                        break;
                    case FlowRecord.DIGIGAIN:
                        if (ParamAutoTuning.m_n5TRawDataType == 1)
                            nGetDataType = m_nDATATYPE_5TRAWDATA;
                        else
                            nGetDataType = m_nDATATYPE_DIGIGAIN;

                        break;
                    case FlowRecord.TP_GAIN:
                        nGetDataType = m_nDATATYPE_5TRAWDATA;
                        break;
                    default:
                        break;
                }

                switch (eMainStep)
                {
                    case MainTuningStep.NO:
                    case MainTuningStep.PEAKCHECKTUNING:
                        if (ParamAutoTuning.m_nAutoTune_P0_detect_time_Index == 1)
                            nGetDataType |= m_nDETECTTIME_800us;

                        break;
                    case MainTuningStep.TILTNO:
                        nGetDataType |= m_nDETECTTIME_800us;
                        break;
                    case MainTuningStep.DIGITALTUNING:
                        if (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)
                            nGetDataType |= m_nDETECTTIME_800us;

                        break;
                    case MainTuningStep.PRESSURETUNING:
                        if (eSubStep == SubTuningStep.PRESSUREPROTECT)
                            nGetDataType |= m_nDETECTTIME_800us;

                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (eRecordAction == FlowRecord.TILT)
                {
                    if (eSubStep == SubTuningStep.TILTTUNING_PTHF)
                        nGetDataType = m_nDATATYPE_TILT_PTHF;
                    else
                        nGetDataType = m_nDATATYPE_TILT_BHF;
                }
            }

            return nGetDataType;
        }

        /// <summary>
        /// 設定讀取Report Data Command種類(Gen8)
        /// </summary>
        /// <param name="cRecordSetParameter">階段資訊參數</param>
        /// <returns>回傳Data Type</returns>
        public int SetGetDataType_Gen8(RecordSetParameter cRecordSetParameter)
        {
            int nGetDataType = m_nDATATYPE_NA;

            FlowRecord eRecordAction = cRecordSetParameter.m_eRecord;

            switch (eRecordAction)
            {
                case FlowRecord.NTRX:
                    nGetDataType = m_nDATATYPE_NRX_400us;
                    break;
                case FlowRecord.NRX:
                    nGetDataType = m_nDATATYPE_NRX_400us;
                    break;
                case FlowRecord.NTX:
                    nGetDataType = m_nDATATYPE_NTX_400us;
                    break;
                case FlowRecord.PTHF_NoSync_Gen8:
                    nGetDataType = m_nDATATYPE_PTHF_NoSync_Gen8;
                    break;
                case FlowRecord.BHF_NoSync_Gen8:
                    nGetDataType = m_nDATATYPE_BHF_NoSync_Gen8;
                    break;
                default:
                    break;
            }

            return nGetDataType;
        }
    }

    /// <summary>
    /// Check State Class
    /// </summary>
    public class CheckState
    {
        public const int m_nSETDIGIGAIN_DISABLE         = 0;
        public const int m_nSETDIGIGAIN_FIXEDVALUE      = 1;
        public const int m_nSETDIGIGAIN_COMPUTEVALUE    = 2;

        public const int m_nSTEPSTATE_NORMAL        = 0;
        public const int m_nSTEPSTATE_INDEPENDENT   = 1;
        public const int m_nSTEPSTATE_SUBSTEP       = 2;
        public const int m_nSTEPSTATE_PRESSURETABLE = 3;

        private frmMain m_cfrmMain;

        public CheckState(frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
        }

        /// <summary>
        /// 確認設定DigiGain的方式
        /// </summary>
        /// <param name="eMainStep">Main Step</param>
        /// <param name="eSupStep">Sun Step</param>
        /// <returns>回傳設定DigiGain方式旗標</returns>
        public int CheckSetDigiGain(MainTuningStep eMainStep, SubTuningStep eSupStep)
        {
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_DEBUG ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SERVER ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
                return m_nSETDIGIGAIN_DISABLE;

            if (eMainStep == MainTuningStep.DIGIGAINTUNING &&
                eSupStep == SubTuningStep.DIGIGAIN)
                return m_nSETDIGIGAIN_FIXEDVALUE;

            if (ParamAutoTuning.m_nSetDigiGain == 0)
                return m_nSETDIGIGAIN_DISABLE;
            else if (ParamAutoTuning.m_nSetDigiGain == 1)
            {
                if (eMainStep == MainTuningStep.NO ||
                    eMainStep == MainTuningStep.TILTNO)
                    return m_nSETDIGIGAIN_FIXEDVALUE;
                else
                    return m_nSETDIGIGAIN_DISABLE;
            }
            else
            {
                if (eMainStep == MainTuningStep.NO ||
                    eMainStep == MainTuningStep.TILTNO)
                    return m_nSETDIGIGAIN_FIXEDVALUE;
                else
                    return m_nSETDIGIGAIN_COMPUTEVALUE;
            }
        }

        /// <summary>
        /// 確認是否使用獨立階段模式
        /// </summary>
        /// <param name="eMainStep">Main Step</param>
        /// <param name="eSubStep">Sub Step</param>
        /// <returns>回傳是否使用獨立階段模式的旗標</returns>
        public int CheckIndependentStep(MainTuningStep eMainStep, SubTuningStep eSubStep)
        {
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
                return m_nSTEPSTATE_NORMAL;
            else if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.DIGITALTUNING || eMainStep == MainTuningStep.PEAKCHECKTUNING)
                return m_nSTEPSTATE_NORMAL;
            else if (ParamAutoTuning.m_nProcessType == 1)
            {
                if ((eMainStep == MainTuningStep.TILTNO && eSubStep == SubTuningStep.TILTNO_BHF) ||
                     (eMainStep == MainTuningStep.PEAKCHECKTUNING && eSubStep == SubTuningStep.PCCONTACT) ||
                     (eMainStep == MainTuningStep.TILTTUNING && eSubStep == SubTuningStep.TILTTUNING_BHF))
                    return m_nSTEPSTATE_SUBSTEP;
                else if (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURETABLE)
                    return m_nSTEPSTATE_PRESSURETABLE;
                else
                    return m_nSTEPSTATE_INDEPENDENT;
            }
            else
                return m_nSTEPSTATE_NORMAL;
        }
    }

    /// <summary>
    /// DigiGainDataFormat Class
    /// </summary>
    public class DigiGainDataFormat
    {
        /// <summary>
        /// DataType Set
        /// </summary>
        public enum DataType
        {
            Beacon_Rx,
            PTHF_Rx,
            BHF_Rx,
            Beacon_Tx,
            PTHF_Tx,
            BHF_Tx
        }

        /// <summary>
        /// DataByteLocation Set
        /// </summary>
        public enum DataByteLocation : int
        {
            BEACON = 16,
            PTHF = 28,
            BHF = 40,
            RX_TIP_NEWFORMAT = 4,
            TX_TIP_NEWFORMAT = 17,
        }
    }

    /// <summary>
    /// TPGainDataFormat Class
    /// </summary>
    public class TPGainDataFormat
    {
        /// <summary>
        /// DataType Set
        /// </summary>
        public enum DataType
        {
            Rx_Tip,
            Rx_Ring,
            Tx_Tip,
            Tx_Ring
        }

        /// <summary>
        /// DataByteLocation Set
        /// </summary>
        public enum DataByteLocation : int
        {
            RX_TIP = 4,
            TX_TIP = 17,
            RX_RING = 36,
            TX_RING = 49
        }
    }

    /// <summary>
    /// GoDrawControllerParameter Class
    /// </summary>
    public class GoDrawControllerParameter
    {
        public string m_sCOMPPort = "";
        public double m_dSpeed = 0.0;
        public int m_nMaxCoordinateX = 0;
        public int m_nMaxCoordinateY = 0;

        public int m_nCoordinateX = 0;
        public int m_nCoordinateY = 0;

        public int m_nServoValueZ = 0;

        public void SetCoordinate(int nCoordinateX, int nCoordinateY)
        {
            m_nCoordinateX = nCoordinateX;
            m_nCoordinateY = nCoordinateY;
        }

        public void SetServoValue(int nServoValueZ)
        {
            m_nServoValueZ = nServoValueZ;
        }
    }

    /// <summary>
    /// Specific Text Class
    /// </summary>
    public class SpecificText
    {
        public const string m_sErrorCode = "ErrorCode";
        public const string m_sErrorMsg = "ErrorMsg";
        public const string m_sErrorMessage = "ErrorMessage";

        public const string m_sRXTotalMax = "RXTotalMax";
        public const string m_sTXTotalMax = "TXTotalMax";

        public const string m_sRXInnerMax = "RXInnerMax";
        public const string m_sTXInnerMax = "TXInnerMax";

        public const string m_sRXInnerMaxPlus3InnerMaxSTD = "RXInnerMax+3*InnerMaxSTD";
        public const string m_sTXInnerMaxPlus3InnerMaxSTD = "TXInnerMax+3*InnerMaxSTD";

        public const string m_sTrcMaxMinusPreP0_TH_1Trc = "TrcMaxMinusPreP0_TH_1Trc";
        public const string m_sTrcMaxMinusPreP0_TH_2Trc = "TrcMaxMinusPreP0_TH_2Trc";
        public const string m_sTrcMaxMinusPreP0_TH_3Trc = "TrcMaxMinusPreP0_TH_3Trc";
        public const string m_sTrcMaxMinusPreP0_TH_4Trc = "TrcMaxMinusPreP0_TH_4Trc";

        public const string m_sP0_Detect_Time = "P0_Detect_Time";
        public const string m_sP0_Detect_Time_400 = "400";
        public const string m_sP0_Detect_Time_800 = "800";
        public const string m_sP0_Detect_Time_400us = "400us";
        public const string m_sP0_Detect_Time_800us = "800us";

        public const string m_sPenPeak_1Traces_Th = "PenPeak_1Traces_Th";
        public const string m_sPenPeak_2Traces_Th = "PenPeak_2Traces_Th";
        public const string m_sPenPeakWidth_Th = "PenPeakWidth_Th";
        public const string m_sPenPeak_4Traces_Th = "PenPeak_4Traces_Th";
        public const string m_sPenPeak_5Traces_Th = "PenPeak_5Traces_Th";
        public const string m_sPenPeak_5Traces_PeakPwr_Th = "PenPeak_5Traces_PeakPwr_Th";
        public const string m_sPenPeak_Th = "PenPeak_Th";
        public const string m_sPenPeakCheck_AreaUP_Pwr_TH = "PenPeakCheck_AreaUP_Pwr_TH";

        public const string m_scActivePen_DigiGain_P0 = "cActivePen_DigiGain_P0";
        public const string m_scActivePen_DigiGain_Beacon_Rx = "cActivePen_DigiGain_Beacon_Rx";
        public const string m_scActivePen_DigiGain_Beacon_Tx = "cActivePen_DigiGain_Beacon_Tx";
        public const string m_scActivePen_DigiGain_PTHF_Rx = "cActivePen_DigiGain_PTHF_Rx";
        public const string m_scActivePen_DigiGain_PTHF_Tx = "cActivePen_DigiGain_PTHF_Tx";
        public const string m_scActivePen_DigiGain_BHF_Rx = "cActivePen_DigiGain_BHF_Rx";
        public const string m_scActivePen_DigiGain_BHF_Tx = "cActivePen_DigiGain_BHF_Tx";

        public const string m_sRXTotalMedian = "RXTotalMedian";
        public const string m_sTXTotalMedian = "TXTotalMedian";
        public const string m_sRXTotalMean = "RXTotalMean";
        public const string m_sRXTotalMin = "RXTotalMin";
        public const string m_sRXEdgeMax = "RXEdgeMax";
        public const string m_sTXTotalMean = "TXTotalMean";
        public const string m_sTXTotalMin = "TXTotalMin";
        public const string m_sTXEdgeMax = "TXEdgeMax";

        public const string m_sRXPreThreshold = "RXPreThreshold";
        public const string m_sTXPreThreshold = "TXPreThreshold";
        public const string m_sRXPreTRxSThreshold = "RXPreTRxSThreshold";
        public const string m_sTXPreTRxSThreshold = "TXPreTRxSThreshold";

        public const string m_sRXTraceNumber = "RXTraceNumber";
        public const string m_sTXTraceNumber = "TXTraceNumber";

        public const string m_scActivePen_FM_P0_TH = "cActivePen_FM_P0_TH";
        public const string m_scActivePen_Beacon_Hover_TH_Rx = "cActivePen_Beacon_Hover_TH_Rx";
        public const string m_scActivePen_Beacon_Hover_TH_Tx = "cActivePen_Beacon_Hover_TH_Tx";
        public const string m_scActivePen_Beacon_Contact_TH_Rx = "cActivePen_Beacon_Contact_TH_Rx";
        public const string m_scActivePen_Beacon_Contact_TH_Tx = "cActivePen_Beacon_Contact_TH_Tx";
        public const string m_scActivePen_TRxS_Beacon_Hover_TH_Rx = "cActivePen_TRxS_Beacon_Hover_TH_Rx";
        public const string m_scActivePen_TRxS_Beacon_Hover_TH_Tx = "cActivePen_TRxS_Beacon_Hover_TH_Tx";
        public const string m_scActivePen_TRxS_Beacon_Contact_TH_Rx = "cActivePen_TRxS_Beacon_Contact_TH_Rx";
        public const string m_scActivePen_TRxS_Beacon_Contact_TH_Tx = "cActivePen_TRxS_Beacon_Contact_TH_Tx";

        public const string m_scActivePen_PTHF_Contact_TH_Rx = "cActivePen_PTHF_Contact_TH_Rx";
        public const string m_scActivePen_PTHF_Contact_TH_Tx = "cActivePen_PTHF_Contact_TH_Tx";
        public const string m_scActivePen_PTHF_Hover_TH_Rx = "cActivePen_PTHF_Hover_TH_Rx";
        public const string m_scActivePen_PTHF_Hover_TH_Tx = "cActivePen_PTHF_Hover_TH_Tx";

        public const string m_scActivePen_BHF_Contact_TH_Rx = "cActivePen_BHF_Contact_TH_Rx";
        public const string m_scActivePen_BHF_Contact_TH_Tx = "cActivePen_BHF_Contact_TH_Tx";
        public const string m_scActivePen_BHF_Hover_TH_Rx = "cActivePen_BHF_Hover_TH_Rx";
        public const string m_scActivePen_BHF_Hover_TH_Tx = "cActivePen_BHF_Hover_TH_Tx";

        public const string m_sRXThreshold = "RXThreshold";
        public const string m_sTXThreshold = "TXThreshold";

        public const string m_scActivePen_FM_Detect_Edge_1Trc_SubPwr = "cActivePen_FM_Detect_Edge_1Trc_SubPwr";
        public const string m_scActivePen_FM_Detect_Edge_2Trc_SubPwr = "cActivePen_FM_Detect_Edge_2Trc_SubPwr";
        public const string m_scActivePen_FM_Detect_Edge_3Trc_SubPwr = "cActivePen_FM_Detect_Edge_3Trc_SubPwr";
        public const string m_scActivePen_FM_Detect_Edge_4Trc_SubPwr = "cActivePen_FM_Detect_Edge_4Trc_SubPwr";

        public const string m_sNoiseP0_Detect_Time = "NoiseP0_Detect_Time";

        public const string m_sRXContactTH = "RXContactTH";
        public const string m_sTXContactTH = "TXContactTH";
        public const string m_sRXHoverTH = "RXHoverTH";
        public const string m_sTXHoverTH = "TXHoverTH";

        public const string m_sRXRingMeanMinus1STD = "RXRingMean-1*STD";
        public const string m_sRXRingMeanMinus2STD = "RXRingMean-2*STD";
        public const string m_sTXRingMeanMinus1STD = "TXRingMean-1*STD";
        public const string m_sTXRingMeanMinus2STD = "TXRingMean-2*STD";

        public const string m_sNormalizeRMSE_H = "NormalizeRMSE_H";
        public const string m_sNormalizeRMSE_V = "NormalizeRMSE_V";

        public const string m_sRXRingMean = "RXRingMean";
        public const string m_sTXRingMean = "TXRingMean";

        public const string m_sNoiseRXInnerMax = "NoiseRXInnerMax";
        public const string m_sNoiseTXInnerMax = "NoiseTXInnerMax";

        public const string m_s_Pen_Ntrig_IQ_BSH_P = "_Pen_Ntrig_IQ_BSH_P";
        public const string m_scActivePen_FM_Pressure3BinsTH = "cActivePen_FM_Pressure3BinsTH";
        public const string m_sPress_3BinsPwr = "Press_3BinsPwr";

        public const string m_sSettingPH1 = "SettingPH1";
        public const string m_sSettingPH2 = "SettingPH2";
        public const string m_sReadPH1 = "ReadPH1";
        public const string m_sReadPH2 = "ReadPH2";

        public const string m_sIndex = "Index";
        public const string m_sPH1 = "PH1";
        public const string m_sPH2 = "PH2";
        public const string m_sSKIP_NUM = "SKIP_NUM";
        public const string m_sFrequency_KHz = "Frequency(KHz)";
        public const string m_sFrequency = "Frequency";
        public const string m_sSettingErrorMessage = "Setting ErrorMessage";
        public const string m_sTraceNumber = "TraceNumber";

        public const string m_sInnerTXMultipleWeighting = "InnerTX*Weighting";
        public const string m_sInnerRXMultipleWeighting = "InnerRX*Weighting";
        public const string m_sEdgeTXMultipleWeighting = "EdgeTX*Weighting";
        public const string m_sEdgeRXMultipleWeighting = "EdgeRX*Weighting";

        public const string m_sColumn1 = "Column1";

        public const string m_sRX_Inner = "RX Inner";
        public const string m_sTX_Inner = "TX Inner";
        public const string m_sRX_Edge = "RX Edge";
        public const string m_sTX_Edge = "TX Edge";

        public const string m_sFileName = "FileName";
        public const string m_sRanking = "Ranking";

        public const string m_sThreshold1 = "Threshold1";
        public const string m_sProtectErrorMessage = "Protect ErrorMessage";
        public const string m_sTableErrorMessage = "Table ErrorMessage";

        public const string m_scPenTiltPwrTHTXHB = "cPenTiltPwrTHTXHB";
        public const string m_scPenTiltPwrTHTXLB = "cPenTiltPwrTHTXLB";

        public const string m_sRX5TLinearityTableTxtFile = "RX 5T LinearityTable.txt";
        public const string m_sTX5TLinearityTableTxtFile = "TX 5T LinearityTable.txt";
        public const string m_sRX3TLinearityTableTxtFile = "RX 3T LinearityTable.txt";
        public const string m_sTX3TLinearityTableTxtFile = "TX 3T LinearityTable.txt";
        public const string m_sRX2TLELinearityTableTxtFile = "RX 2TLE LinearityTable.txt";
        public const string m_sTX2TLELinearityTableTxtFile = "TX 2TLE LinearityTable.txt";
        public const string m_sRX2THELinearityTableTxtFile = "RX 2THE LinearityTable.txt";
        public const string m_sTX2THELinearityTableTxtFile = "TX 2THE LinearityTable.txt";

        public const string m_sDataInfo = "DataInfo";

        public const string m_sPTHF_H = "PTHF_H";
        public const string m_sPTHF_V = "PTHF_V";
        public const string m_sBHF_H = "BHF_H";
        public const string m_sBHF_V = "BHF_V";
        public const string m_sTotalScore = "TotalScore";
        public const string m_sTotal_Score = "Total Score";

        public const string m_sResult = "Result";
        public const string m_sPTHFExceptionMessage = "PTHFExceptionMessage";
        public const string m_sBHFExceptionMessage = "BHFExceptionMessage";

        public const string m_sException_Message = "Exception Message";

        public const string m_sInnerTX = "InnerTX";
        public const string m_sInnerRX = "InnerRX";
        public const string m_sEdgeTX = "EdgeTX";
        public const string m_sEdgeRX = "EdgeRX";

        public const string m_sFlowStep = "FlowStep";

        public const string m_sRxBeaconMean = "Rx Beacon Mean";
        public const string m_sTxBeaconMean = "Tx Beacon Mean";
        public const string m_sRxPTHFMean = "Rx PTHF Mean";
        public const string m_sTxPTHFMean = "Tx PTHF Mean";
        public const string m_sRxBHFMean = "Rx BHF Mean";
        public const string m_sTxBHFMean = "Tx BHF Mean";

        public const string m_sFilterRXValue = "FilterRXValue";
        public const string m_sFilterTXValue = "FilterTXValue";

        public const string m_sPenPeak1Tr = "PenPeak1Tr";
        public const string m_sPenPeak2Tr = "PenPeak2Tr";
        public const string m_sPenPeak3Tr = "PenPeak3Tr";
        public const string m_sPenPeak4Tr = "PenPeak4Tr";
        public const string m_sPenPeak5Tr = "PenPeak5Tr";

        public const string m_sRXTotalSTD = "RXTotalSTD";
        public const string m_sRXMeanPlus1STD = "RXMean+1STD";
        public const string m_sRXMeanMinus1STD = "RXMean-1STD";
        public const string m_sRXPreTH_M1 = "RXPreTH_M1";
        public const string m_sRXPreTH_M2 = "RXPreTH_M2";
        public const string m_sTXTotalSTD = "RXTotalSTD";
        public const string m_sTXMeanPlus1STD = "TXMean+1STD";
        public const string m_sTXMeanMinus1STD = "TXMean-1STD";
        public const string m_sTXPreTH_M1 = "TXPreTH_M1";
        public const string m_sTXPreTH_M2 = "TXPreTH_M2";

        public const string m_sFirstMaxValue = "FirstMaxValue";
        public const string m_sSecondMaxValue = "SecondMaxValue";
        public const string m_sThirdMaxValue = "ThirdMaxValue";
        public const string m_sSecondMaxMean = "SecondMaxMean";
        public const string m_sThirdMaxMean = "ThirdMaxMean";
        public const string m_sTotalMax = "TotalMax";
        public const string m_sTotalMin = "TotalMin";
        public const string m_sTotalMean = "TotalMean";

        public const string m_sRXSecondMaxMean = "RXSecondMaxMean";
        public const string m_sRXThirdMaxMean = "RXThirdMaxMean";
        public const string m_sRXTotalMaxMean = "RXTotalMaxMean";
        public const string m_sRXThresholdRatio = "RXThresholdRatio";
        public const string m_sTXSecondMaxMean = "TXSecondMaxMean";
        public const string m_sTXThirdMaxMean = "TXThirdMaxMean";
        public const string m_sTXTotalMaxMean = "TXTotalMaxMean";
        public const string m_sTXThresholdRatio = "TXThresholdRatio";

        public const string m_sRMSE_H = "RMSE_H";
        public const string m_sRMSE_V = "RMSE_V";

        public const string m_sRXRingSTD = "RXRingSTD";
        public const string m_sTXRingSTD = "TXRingSTD";

        public const string m_sPressMaxDFTRxMax = "PressMaxDFTRxMax";
        public const string m_sPressMaxDFTRxMean = "PressMaxDFTRxMean";

        public const string m_sMax_MeanPlus3Std = "Max Mean+3*Std";
        public const string m_sMax_MeanPlus3Std_Trace = "Max Mean+3*Std Trace";
        public const string m_sMax_MeanPlus3Std_Location = "Max Mean+3*Std Location";
        public const string m_sEdge_Max_MeanPlus3Std = "Edge Max Mean+3*Std";
        public const string m_sEdge_Max_MeanPlus3Std_Trace = "Edge Max Mean+3*Std Trace";
        public const string m_sInner_Max_MeanPlus3Std = "Inner Max Mean+3*Std";
        public const string m_sInner_Max_MeanPlus3Std_Trace = "Inner Max Mean+3*Std Trace";

        public const string m_sMeanPlus3Std = "Mean+3*Std";
        public const string m_sOver_3Std_HLBPercent = "Over 3Std HLBPercent";
        public const string m_sInRange_3Std_HLBPercent = "InRange 3Std HLBPercent";
        public const string m_sInRange_2Std_HLBPercent = "InRange 2Std HLBPercent";
        public const string m_sInRange_1Std_HLBPercent = "InRange 1Std HLBPercent";

        public const string m_sScale = "Scale";
        public const string m_sAmount = "Amount";

        public const string m_sRank = "Rank";
        public const string m_sBeacon_Rank = "Beacon Rank";
        public const string m_sBeacon_RX_Inner = "Beacon RX Inner";
        public const string m_sBeacon_TX_Inner = "Beacon TX Inner";
        public const string m_sBeacon_RX_Edge = "Beacon RX Edge";
        public const string m_sBeacon_TX_Edge = "Beacon TX Edge";
        public const string m_sPTHF_Rank = "PTHF Rank";
        public const string m_sPTHF_RX_Inner = "PTHF RX Inner";
        public const string m_sPTHF_TX_Inner = "PTHF TX Inner";
        public const string m_sPTHF_RX_Edge = "PTHF RX Edge";
        public const string m_sPTHF_TX_Edge = "PTHF TX Edge";
        public const string m_sBHF_Rank = "BHF Rank";
        public const string m_sBHF_RX_Inner = "BHF RX Inner";
        public const string m_sBHF_TX_Inner = "BHF TX Inner";
        public const string m_sBHF_RX_Edge = "BHF RX Edge";
        public const string m_sBHF_TX_Edge = "BHF TX Edge";

        public const string m_sResultFileName = "Total_Result_WR.csv";
        public const string m_sTNResultFileName = "TN_Total_Result_WR.csv";
        public const string m_sNRankFileName = "N_Total_Rank_WR.csv";

        public const string m_sResultText = "Result";
        public const string m_sStepListText = "StepList";
        public const string m_sFlowText = "Flow";
        public const string m_sPictureText = "Picture";
        public const string m_sReferenceText = "Reference";

        public const string m_sReferenceFileName = "Reference.csv";
        public const string m_sRXReferenceFileName = "Reference_RX.csv";
        public const string m_sTXReferenceFileName = "Reference_TX.csv";
        public const string m_sTNReferenceFileName = "Reference_TN.csv";
        public const string m_sTotalReferenceFileName = "Reference_Total.csv";

        public const string m_sPressureTableFileName = "PressureTable.txt";

        public const string m_sChartFileName = "Chart.jpg";
        public const string m_sChart_TopDataFileName = "Chart_TopData.jpg";
        public const string m_sFreChartFileName = "FreChart.jpg";
        public const string m_sFrqChartFileName = "FrqChart.jpg";
        public const string m_sFrqChartIncludeMaxFileName = "FrqChart_Max.jpg";
        public const string m_sHistogramFileName = "Histogram.jpg";
        public const string m_sFrqChartBy3SubPlotFileName = "FrqChartBy3SubPlot.jpg";
        public const string m_sFrqChartBy3SubPlotIncludeMaxFileName = "FrqChartBy3SubPlot_Max.jpg";

        public const string m_sBackUpText = "BackUp";
        public const string m_sErrorBackUpText = "ErrorBackUp";
    }

    public enum EdgeShadowOption
    {
        ON,
        OFF,
        NOT_CHANGE
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
}
