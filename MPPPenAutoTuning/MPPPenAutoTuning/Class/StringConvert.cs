using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
	class StringConvert
	{
        public static Dictionary<string, int> m_dictOperatorLevel;

        public static Dictionary<MainTuningStep, string> m_dictMainStepMappingTable = new Dictionary<MainTuningStep, string>()
        {
            { MainTuningStep.NO,                 "Noise" },
            { MainTuningStep.TILTNO,             "Tilt Noise" },
            { MainTuningStep.DIGIGAINTUNING,     "DigiGain Tuning" },
            { MainTuningStep.TPGAINTUNING,       "TP_Gain Tuning" },
            { MainTuningStep.PEAKCHECKTUNING,    "PeakCheck Tuning" },
            { MainTuningStep.DIGITALTUNING,      "Digital Tuning" },
            { MainTuningStep.TILTTUNING,         "Tilt Tuning" },
            { MainTuningStep.PRESSURETUNING,     "Pressure Tuning" },
            { MainTuningStep.LINEARITYTUNING,    "Linearity Tuning" },
            { MainTuningStep.SERVERCONTRL,       "Else" }
        };

        public static Dictionary<SubTuningStep, string> m_dictSubStepMappingTable = new Dictionary<SubTuningStep, string>()
        {
            { SubTuningStep.NO,                  "Noise" },
            { SubTuningStep.TILTNO_PTHF,         "TiltNoise_PTHF" },
            { SubTuningStep.TILTNO_BHF,          "TiltNoise_BHF" },
            { SubTuningStep.HOVER_1ST,           "Hover_1st" },
            { SubTuningStep.HOVER_2ND,           "Hover_2nd" },
            { SubTuningStep.HOVERTRxS,           "HoverTRxS" },
            { SubTuningStep.CONTACT,             "Contact" },
            { SubTuningStep.CONTACTTRxS,         "ContactTRxS" },
            { SubTuningStep.TILTTUNING_PTHF,     "TiltTuning_PTHF" },
            { SubTuningStep.TILTTUNING_BHF,      "TiltTuning_BHF" },
            { SubTuningStep.PRESSURESETTING,     "PressureSetting" },
            { SubTuningStep.PRESSUREPROTECT,     "PressureProtect" },
            { SubTuningStep.PRESSURETABLE,       "PressureTable" },
            { SubTuningStep.LINEARITYTABLE,      "LinearityTable" },
            { SubTuningStep.PCHOVER_1ST,         "PeakCheckHover_1st" },
            { SubTuningStep.PCHOVER_2ND,         "PeakCheckHover_2nd" },
            { SubTuningStep.PCCONTACT,           "PeakCheckContact" },
            { SubTuningStep.DIGIGAIN,            "DigiGain" },
            { SubTuningStep.TP_GAIN,             "TP_Gain" },
            { SubTuningStep.ELSE,                "Else" }
        };


        public static Dictionary<MainTuningStep, string> m_dictMainStepCommandScriptMappingTable = new Dictionary<MainTuningStep, string>()
        {
            { MainTuningStep.NO,                 "Noise" },
            { MainTuningStep.TILTNO,             "Tilt Noise" },
            { MainTuningStep.DIGIGAINTUNING,     "DigiGain Tuning" },
            { MainTuningStep.TPGAINTUNING,       "TPGain Tuning" },
            { MainTuningStep.PEAKCHECKTUNING,    "PeakCheck Tuning" },
            { MainTuningStep.DIGITALTUNING,      "Digital Tuning" },
            { MainTuningStep.TILTTUNING,         "Tilt Tuning" },
            { MainTuningStep.PRESSURETUNING,     "Pressure Tuning" },
            { MainTuningStep.LINEARITYTUNING,    "Linearity Tuning" },
            { MainTuningStep.SERVERCONTRL,       "Else" }
        };

        public static Dictionary<SubTuningStep, string> m_dictSubStepCommandScriptMappingTable = new Dictionary<SubTuningStep, string>()
        {
            { SubTuningStep.NO,                  "Noise" },
            { SubTuningStep.TILTNO_PTHF,         "TiltNoisePTHF" },
            { SubTuningStep.TILTNO_BHF,          "TiltNoiseBHF" },
            { SubTuningStep.HOVER_1ST,           "Hover1st" },
            { SubTuningStep.HOVER_2ND,           "Hover2nd" },
            { SubTuningStep.HOVERTRxS,           "HoverTRxS" },
            { SubTuningStep.CONTACT,             "Contact" },
            { SubTuningStep.CONTACTTRxS,         "ContactTRxS" },
            { SubTuningStep.TILTTUNING_PTHF,     "TiltTuningPTHF" },
            { SubTuningStep.TILTTUNING_BHF,      "TiltTuningBHF" },
            { SubTuningStep.PRESSURESETTING,     "PressureSetting" },
            { SubTuningStep.PRESSUREPROTECT,     "PressureProtect" },
            { SubTuningStep.PRESSURETABLE,       "PressureTable" },
            { SubTuningStep.LINEARITYTABLE,      "LinearityTable" },
            { SubTuningStep.PCHOVER_1ST,         "PeakCheckHover1st" },
            { SubTuningStep.PCHOVER_2ND,         "PeakCheckHover2nd" },
            { SubTuningStep.PCCONTACT,           "PeakCheckContact" },
            { SubTuningStep.DIGIGAIN,            "DigiGain" },
            { SubTuningStep.TP_GAIN,             "TPGain" },
            { SubTuningStep.ELSE,                "Else" }
        };

        public static Dictionary<SubTuningStep, string> m_dictSubStepCNMappingTable = new Dictionary<SubTuningStep, string>()
        {
            { SubTuningStep.NO,                  "N" },
            { SubTuningStep.TILTNO_PTHF,         "TN_PTHF" },
            { SubTuningStep.TILTNO_BHF,          "TN_BHF" },
            { SubTuningStep.HOVER_1ST,           "H_1st" },
            { SubTuningStep.HOVER_2ND,           "H_2nd" },
            { SubTuningStep.HOVERTRxS,           "HTRxS" },
            { SubTuningStep.CONTACT,             "C" },
            { SubTuningStep.CONTACTTRxS,         "CTRxS" },
            { SubTuningStep.TILTTUNING_PTHF,     "TT_PTHF" },
            { SubTuningStep.TILTTUNING_BHF,      "TT_BHF" },
            { SubTuningStep.PRESSURESETTING,     "PS" },
            { SubTuningStep.PRESSUREPROTECT,     "PP" },
            { SubTuningStep.PRESSURETABLE,       "PT" },
            { SubTuningStep.LINEARITYTABLE,      "LT" },
            { SubTuningStep.PCHOVER_1ST,         "PCTH_1st" },
            { SubTuningStep.PCHOVER_2ND,         "PCTH_2nd" },
            { SubTuningStep.PCCONTACT,           "PCTC" },
            { SubTuningStep.DIGIGAIN,            "DigiGain" },
            { SubTuningStep.TP_GAIN,             "TP_Gain" },
            { SubTuningStep.ELSE,                "Else" }
        };

        public static Dictionary<int, string> m_dictPatternTypeMappingTable = new Dictionary<int, string>()
        {
            { MainConstantParameter.m_nDRAWTYPE_NONE,                      "" },
            { MainConstantParameter.m_nDRAWTYPE_SLANTLINE,                 "(Slant Line)" },
            { MainConstantParameter.m_nDRAWTYPE_HORIZONTALLINE,            "(Horizontal Line)" },
            { MainConstantParameter.m_nDRAWTYPE_VERTICALLINE,              "(Vertical Line)" },
            { MainConstantParameter.m_nDRAWTYPE_CENTERPOINT,               "(Center Point)" },
            { MainConstantParameter.m_nDRAWTYPE_HORIZONTALLONGLINE,        "(Horizontal Line)" },
            { MainConstantParameter.m_nDRAWTYPE_VERTICALLONGLINE,          "(Vertical Line)" },
            { MainConstantParameter.m_nDRAWTYPE_HORIZONTALMIDDLELONGLINE,  "(Horizontal Line)" },
            { MainConstantParameter.m_nDRAWTYPE_VERTICALMIDDLELONGLINE,    "(Vertical Line)" }
        };

        public static Dictionary<int, int> m_dictTPGTRAngleMappingTable = new Dictionary<int, int>()
        {
            { MainConstantParameter.m_nDRAWTYPE_HORIZONTALMIDDLELONGLINE,  ParamAutoTuning.m_nTPGTHorizontalRAngle },
            { MainConstantParameter.m_nDRAWTYPE_VERTICALMIDDLELONGLINE,    ParamAutoTuning.m_nTPGTVerticalRAngle }
        };

        public static Dictionary<int, string> m_dictControlModeMappingTable = new Dictionary<int, string>()
        {
            { MainConstantParameter.m_nMODE_DEBUG,           "Debug Mode" },
            { MainConstantParameter.m_nMODE_SERVER,          "Server Mode" },
            { MainConstantParameter.m_nMODE_CLIENT,          "Client Mode" },
            { MainConstantParameter.m_nMODE_GODRAW,          "GoDraw Mode" },
            { MainConstantParameter.m_nMODE_SINGLE,          "Single Mode" },
            { MainConstantParameter.m_nMODE_LOADDATA,        "LoadData Mode" }
        };

        public static Dictionary<SubTuningStep, SubTuningStep[]> m_dictNextSubStepMappingTable = new Dictionary<SubTuningStep, SubTuningStep[]>()
        {
            { SubTuningStep.NO,                  new SubTuningStep[] { SubTuningStep.HOVER_1ST, SubTuningStep.TILTNO_PTHF, SubTuningStep.PCHOVER_1ST, SubTuningStep.DIGIGAIN, SubTuningStep.TP_GAIN } },
            { SubTuningStep.HOVER_1ST,           new SubTuningStep[] { SubTuningStep.HOVER_2ND } },
            { SubTuningStep.HOVER_2ND,           new SubTuningStep[] { SubTuningStep.CONTACT } },
            { SubTuningStep.CONTACT,             new SubTuningStep[] { SubTuningStep.HOVERTRxS } },
            { SubTuningStep.HOVERTRxS,           new SubTuningStep[] { SubTuningStep.CONTACTTRxS } },
            { SubTuningStep.CONTACTTRxS,         new SubTuningStep[] { SubTuningStep.TILTTUNING_PTHF, SubTuningStep.PRESSURESETTING, SubTuningStep.LINEARITYTABLE } },
            { SubTuningStep.TILTNO_PTHF,         new SubTuningStep[] { SubTuningStep.TILTNO_BHF } },
            { SubTuningStep.TILTNO_BHF,          new SubTuningStep[] { SubTuningStep.ELSE } },
            { SubTuningStep.TILTTUNING_PTHF,     new SubTuningStep[] { SubTuningStep.TILTTUNING_BHF } },
            { SubTuningStep.TILTTUNING_BHF,      new SubTuningStep[] { SubTuningStep.ELSE } },
            { SubTuningStep.PRESSURESETTING,     new SubTuningStep[] { SubTuningStep.PRESSURETABLE } },
            { SubTuningStep.PRESSUREPROTECT,     new SubTuningStep[] { SubTuningStep.PRESSURETABLE } },
            { SubTuningStep.PRESSURETABLE,       new SubTuningStep[] { SubTuningStep.ELSE } },
            { SubTuningStep.LINEARITYTABLE,      new SubTuningStep[] { SubTuningStep.ELSE } },
            { SubTuningStep.PCHOVER_1ST,         new SubTuningStep[] { SubTuningStep.PCHOVER_2ND } },
            { SubTuningStep.PCHOVER_2ND,         new SubTuningStep[] { SubTuningStep.PCCONTACT } },
            { SubTuningStep.PCCONTACT,           new SubTuningStep[] { SubTuningStep.ELSE } },
            { SubTuningStep.DIGIGAIN,            new SubTuningStep[] { SubTuningStep.ELSE } },
            { SubTuningStep.TP_GAIN,             new SubTuningStep[] { SubTuningStep.ELSE } },
            { SubTuningStep.ELSE,                new SubTuningStep[] { SubTuningStep.ELSE } }
        };

        public static Dictionary<SubTuningStep, string> m_dictPreviousSubStepMappingTable = new Dictionary<SubTuningStep, string>()
        {
            { SubTuningStep.HOVER_1ST,           "Noise" },
            { SubTuningStep.TILTNO_PTHF,         "Noise" },
            { SubTuningStep.TILTNO_BHF,          "Noise" },
            { SubTuningStep.PCHOVER_1ST,         "Noise" },
            { SubTuningStep.DIGIGAIN,            "Noise" },
            { SubTuningStep.TP_GAIN,             "Noise" },
            { SubTuningStep.TILTTUNING_PTHF,     "Digital Tuning" },
            { SubTuningStep.PRESSURESETTING,     "Digital Tuning" },
            { SubTuningStep.LINEARITYTABLE,      "Digital Tuning" }
        };

        #region Declare FW Command Parameter Name
        public const string m_sCMDPARAM_FWCHECKVERSION          = "FWCheckVersion";
        //public const string m_sCMDPARAM_AP_PPEAKTHRDSHOLD       = "_AP_pPeakThrdshold";
        public const string m_sCMDPARAM_PH1                     = SpecificText.m_sPH1;
        public const string m_sCMDPARAM_PH2                     = SpecificText.m_sPH2;
        public const string m_sCMDPARAM_NOISE_PTHF              = "PTHF Noise Scan";
        public const string m_sCMDPARAM_NOISE_BHF               = "BHF Noise Scan";
        public const string m_sCMDPARAM_REPORTNUMBER            = "ReportNumber";
        public const string m_sCMDPARAM_P0_TH                   = SpecificText.m_scActivePen_FM_P0_TH;
        public const string m_sCMDPARAM_TRXS_HOVER_TH_RX        = SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Rx;
        public const string m_sCMDPARAM_TRXS_HOVER_TH_TX        = SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Tx;
        public const string m_sCMDPARAM_TRXS_CONTACT_TH_RX      = SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Rx;
        public const string m_sCMDPARAM_TRXS_CONTACT_TH_TX      = SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Tx;
        public const string m_sCMDPARAM_EDGE_1TRC_SUBPWR        = SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr;
        public const string m_sCMDPARAM_EDGE_2TRC_SUBPWR        = SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr;
        public const string m_sCMDPARAM_EDGE_3TRC_SUBPWR        = SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr;
        public const string m_sCMDPARAM_EDGE_4TRC_SUBPWR        = SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr;
        public const string m_sCMDPARAM_HOVER_TH_RX             = SpecificText.m_scActivePen_Beacon_Hover_TH_Rx;
        public const string m_sCMDPARAM_HOVER_TH_TX             = SpecificText.m_scActivePen_Beacon_Hover_TH_Tx;
        public const string m_sCMDPARAM_CONTACT_TH_RX           = SpecificText.m_scActivePen_Beacon_Contact_TH_Rx;
        public const string m_sCMDPARAM_CONTACT_TH_TX           = SpecificText.m_scActivePen_Beacon_Contact_TH_Tx;
        public const string m_sCMDPARAM_PTHF_CONTACT_TH_RX      = SpecificText.m_scActivePen_PTHF_Contact_TH_Rx;
        public const string m_sCMDPARAM_PTHF_CONTACT_TH_TX      = SpecificText.m_scActivePen_PTHF_Contact_TH_Tx;
        public const string m_sCMDPARAM_PTHF_HOVER_TH_RX        = SpecificText.m_scActivePen_PTHF_Hover_TH_Rx;
        public const string m_sCMDPARAM_PTHF_HOVER_TH_TX        = SpecificText.m_scActivePen_PTHF_Hover_TH_Tx;
        public const string m_sCMDPARAM_BHF_CONTACT_TH_RX       = SpecificText.m_scActivePen_BHF_Contact_TH_Rx;
        public const string m_sCMDPARAM_BHF_CONTACT_TH_TX       = SpecificText.m_scActivePen_BHF_Contact_TH_Tx;
        public const string m_sCMDPARAM_BHF_HOVER_TH_RX         = SpecificText.m_scActivePen_BHF_Hover_TH_Rx;
        public const string m_sCMDPARAM_BHF_HOVER_TH_TX         = SpecificText.m_scActivePen_BHF_Hover_TH_Tx;
        public const string m_sCMDPARAM_IQ_BSH_P                = SpecificText.m_s_Pen_Ntrig_IQ_BSH_P;
        public const string m_sCMDPARAM_PRESSURE3BINSTH         = SpecificText.m_scActivePen_FM_Pressure3BinsTH;
        public const string m_sCMDPARAM_PRESS_3BINSPWR          = SpecificText.m_sPress_3BinsPwr;
        public const string m_sCMDPARAM_DIGIGAIN_P0             = SpecificText.m_scActivePen_DigiGain_P0;
        public const string m_sCMDPARAM_DIGIGAIN_BEACON_RX      = SpecificText.m_scActivePen_DigiGain_Beacon_Rx;
        public const string m_sCMDPARAM_DIGIGAIN_BEACON_TX      = SpecificText.m_scActivePen_DigiGain_Beacon_Tx;
        public const string m_sCMDPARAM_DIGIGAIN_PTHF_RX        = SpecificText.m_scActivePen_DigiGain_PTHF_Rx;
        public const string m_sCMDPARAM_DIGIGAIN_PTHF_TX        = SpecificText.m_scActivePen_DigiGain_PTHF_Tx;
        public const string m_sCMDPARAM_DIGIGAIN_BHF_RX         = SpecificText.m_scActivePen_DigiGain_BHF_Rx;
        public const string m_sCMDPARAM_DIGIGAIN_BHF_TX         = SpecificText.m_scActivePen_DigiGain_BHF_Tx;
        public const string m_sCMDPARAM_PEN_HI_HF_THD           = "cActivePen_Pen_HI_HF_THD";
        public const string m_sCMDPARAM_GETDIGIGAINCOMMAND      = "GetDigiGainCommand";
        #endregion

        #region Declare Record Info & Parameter
        public const string m_sRECORD_FWCHECKVERSION                = "FWCheckVersion";
        public const string m_sRECORD_SUBSTEP                       = "StepStatus";
        public const string m_sRECORD_SETTINGPH1                    = SpecificText.m_sSettingPH1;
        public const string m_sRECORD_SETTINGPH2                    = SpecificText.m_sSettingPH2;
        public const string m_sRECORD_READPH1                       = SpecificText.m_sReadPH1;
        public const string m_sRECORD_READPH2                       = SpecificText.m_sReadPH2;
        public const string m_sRECORD_FREQUENCY                     = SpecificText.m_sFrequency;
        public const string m_sRECORD_REPORTNUMBER                  = "ReportNumber";
        public const string m_sRECORD_HOVERRAISEHEIGHT              = "HoverRaiseHeight";
        public const string m_sRECORD_RANKINDEX                     = "RankIndex";
        public const string m_sRECORD_P0_DETECT_TIME                = "AutoTune_P0_detect_time";
        public const string m_sRECORD_NOISERXINNERMAX               = SpecificText.m_sNoiseRXInnerMax;
        public const string m_sRECORD_NOISETXINNERMAX               = SpecificText.m_sNoiseTXInnerMax;
        public const string m_sRECORD_NOISERXINMAXPLUS3INMAXSTD     = "NoiseRXInnerMax+3*InnerMaxSTD";
        public const string m_sRECORD_NOISEP0_DETECT_TIME_INDEX     = "NoiseP0_Detect_Time_Index";
        public const string m_sRECORD_NOISETRMAXMINUSPREP0_TH_1TR   = "NoiseTrcMaxMinusPreP0_TH_1Trc";
        public const string m_sRECORD_NOISETRMAXMINUSPREP0_TH_2TR   = "NoiseTrcMaxMinusPreP0_TH_2Trc";
        public const string m_sRECORD_NOISETRMAXMINUSPREP0_TH_3TR   = "NoiseTrcMaxMinusPreP0_TH_3Trc";
        public const string m_sRECORD_NOISETRMAXMINUSPREP0_TH_4TR   = "NoiseTrcMaxMinusPreP0_TH_4Trc";
        public const string m_sRECORD_NOISEDIGIGAIN_P0              = "NoiseDigiGain_P0";
        public const string m_sRECORD_NOISEDIGIGAIN_BEACON_RX       = "NoiseDigiGain_Beacon_Rx";
        public const string m_sRECORD_NOISEDIGIGAIN_BEACON_TX       = "NoiseDigiGain_Beacon_Tx";
        public const string m_sRECORD_PTHFNOISERXTOTALMAX           = "PTHFNoiseRXTotalMax";
        public const string m_sRECORD_PTHFNOISETXTOTALMAX           = "PTHFNoiseTXTotalMax";
        public const string m_sRECORD_SP0_TH                        = "SettingcActivePen_FM_P0_TH";
        public const string m_sRECORD_RP0_TH                        = "ReadcActivePen_FM_P0_TH";
        public const string m_sRECORD_STRXS_HOVER_TH_RX             = "SettingTRxS_Beacon_Hover_TH_Rx";
        public const string m_sRECORD_RTRXS_HOVER_TH_RX             = "ReadTRxS_Beacon_Hover_TH_Rx";
        public const string m_sRECORD_STRXS_HOVER_TH_TX             = "SettingTRxS_Beacon_Hover_TH_Tx";
        public const string m_sRECORD_RTRXS_HOVER_TH_TX             = "ReadTRxS_Beacon_Hover_TH_Tx";
        public const string m_sRECORD_STRXS_CONTACT_TH_RX           = "SettingTRxS_Beacon_Contact_TH_Rx";
        public const string m_sRECORD_RTRXS_CONTACT_TH_RX           = "ReadTRxS_Beacon_Contact_TH_Rx";
        public const string m_sRECORD_STRXS_CONTACT_TH_TX           = "SettingTRxS_Beacon_Contact_TH_Tx";
        public const string m_sRECORD_RTRXS_CONTACT_TH_TX           = "ReadTRxS_Beacon_Contact_TH_Tx";
        public const string m_sRECORD_SHOVER_TH_RX                  = "SettingBeacon_Hover_TH_Rx";
        public const string m_sRECORD_RHOVER_TH_RX                  = "ReadBeacon_Hover_TH_Rx";
        public const string m_sRECORD_SHOVER_TH_TX                  = "SettingBeacon_Hover_TH_Tx";
        public const string m_sRECORD_RHOVER_TH_TX                  = "ReadBeacon_Hover_TH_Tx";
        public const string m_sRECORD_SCONTACT_TH_RX                = "SettingBeacon_Contact_TH_Rx";
        public const string m_sRECORD_RCONTACT_TH_RX                = "ReadBeacon_Contact_TH_Rx";
        public const string m_sRECORD_SCONTACT_TH_TX                = "SettingBeacon_Contact_TH_Tx";
        public const string m_sRECORD_RCONTACT_TH_TX                = "ReadBeacon_Contact_TH_Tx";
        public const string m_sRECORD_SPTHF_CONTACT_TH_RX           = "SettingPTHF_Contact_TH_Rx";
        public const string m_sRECORD_RPTHF_CONTACT_TH_RX           = "ReadPTHF_Contact_TH_Rx";
        public const string m_sRECORD_SPTHF_CONTACT_TH_TX           = "SettingPTHF_Contact_TH_Tx";
        public const string m_sRECORD_RPTHF_CONTACT_TH_TX           = "ReadPTHF_Contact_TH_Tx";
        public const string m_sRECORD_SPTHF_HOVER_TH_RX             = "SettingPTHF_Hover_TH_Rx";
        public const string m_sRECORD_RPTHF_HOVER_TH_RX             = "ReadPTHF_Hover_TH_Rx";
        public const string m_sRECORD_SPTHF_HOVER_TH_TX             = "SettingPTHF_Hover_TH_Tx";
        public const string m_sRECORD_RPTHF_HOVER_TH_TX             = "ReadPTHF_Hover_TH_Tx";
        public const string m_sRECORD_SBHF_CONTACT_TH_RX            = "SettingBHF_Contact_TH_Rx";
        public const string m_sRECORD_RBHF_CONTACT_TH_RX            = "ReadBHF_Contact_TH_Rx";
        public const string m_sRECORD_SBHF_CONTACT_TH_TX            = "SettingBHF_Contact_TH_Tx";
        public const string m_sRECORD_RBHF_CONTACT_TH_TX            = "ReadBHF_Contact_TH_Tx";
        public const string m_sRECORD_SBHF_HOVER_TH_RX              = "SettingBHF_Hover_TH_Rx";
        public const string m_sRECORD_RBHF_HOVER_TH_RX              = "ReadBHF_Hover_TH_Rx";
        public const string m_sRECORD_SBHF_HOVER_TH_TX              = "SettingBHF_Hover_TH_Tx";
        public const string m_sRECORD_RBHF_HOVER_TH_TX              = "ReadBHF_Hover_TH_Tx";
        public const string m_sRECORD_SEDGE_1TRC_SUBPWR             = "SettingEdge_1Trc_SubPwr";
        public const string m_sRECORD_REDGE_1TRC_SUBPWR             = "ReadEdge_1Trc_SubPwr";
        public const string m_sRECORD_SEDGE_2TRC_SUBPWR             = "SettingEdge_2Trc_SubPwr";
        public const string m_sRECORD_REDGE_2TRC_SUBPWR             = "ReadEdge_2Trc_SubPwr";
        public const string m_sRECORD_SEDGE_3TRC_SUBPWR             = "SettingEdge_3Trc_SubPwr";
        public const string m_sRECORD_REDGE_3TRC_SUBPWR             = "ReadEdge_3Trc_SubPwr";
        public const string m_sRECORD_SEDGE_4TRC_SUBPWR             = "SettingEdge_4Trc_SubPwr";
        public const string m_sRECORD_REDGE_4TRC_SUBPWR             = "ReadEdge_4Trc_SubPwr";
        public const string m_sRECORD_RXTRACENUMBER                 = SpecificText.m_sRXTraceNumber;
        public const string m_sRECORD_TXTRACENUMBER                 = SpecificText.m_sTXTraceNumber;
        public const string m_sRECORD_DRAWLINETYPE                  = "DrawLineType";
        public const string m_sRECORD_CONTROLMODE                   = "ControlMode";
        public const string m_sRECORD_DRAWLINESTATUS                = "DrawLineStatus";
        public const string m_sRECORD_WEIGHTSTATUS                  = "WeightStatus";
        public const string m_sRECORD_PRESSUREWEIGHT                = "PressureWeight";
        public const string m_sRECORD_REALITYWEIGHT                 = "RealityWeight";
        public const string m_sRECORD_OFFSETWEIGHT                  = "OffsetWeight";
        public const string m_sRECORD_EXTRAINCWEIGHT                = "ExtraIncWeight";
        public const string m_sRECORD_PTPENVERSION                  = "PTPenVersion";
        public const string m_sRECORD_TOTALWEIGHT                   = "TotalWeight";
        public const string m_sRECORD_SIQ_BSH_P                     = "SettingIQ_BSH_P";
        public const string m_sRECORD_RIQ_BSH_P                     = "ReadIQ_BSH_P";
        public const string m_sRECORD_SPRESSURE3BINSTH              = "SettingPressure3BinsTH";
        public const string m_sRECORD_RPRESSURE3BINSTH              = "ReadPressure3BinsTH";
        public const string m_sRECORD_SPRESS_3BINSPWR               = "SettingPress_3BinsPwr";
        public const string m_sRECORD_RPRESS_3BINSPWR               = "ReadPress_3BinsPwr";
        public const string m_sRECORD_PRESS_MAXDFTRXMEAN            = "PressureMaxDFTRxMean";
        public const string m_sRECORD_BEFIQ_BSH_P                   = "BefIQ_BSH_P";
        public const string m_sRECORD_BEFPRESS_MAXDFTRXMEAN         = "BefPressureMaxDFTRxMean";
        public const string m_sRECORD_SDIGIGAIN_P0                  = "SettingDigiGain_P0";
        public const string m_sRECORD_RDIGIGAIN_P0                  = "ReadDigiGain_P0";
        public const string m_sRECORD_SDIGIGAIN_BEACON_RX           = "SettingDigiGain_Beacon_Rx";
        public const string m_sRECORD_RDIGIGAIN_BEACON_RX           = "ReadDigiGain_Beacon_Rx";
        public const string m_sRECORD_SDIGIGAIN_BEACON_TX           = "SettingDigiGain_Beacon_Tx";
        public const string m_sRECORD_RDIGIGAIN_BEACON_TX           = "ReadDigiGain_Beacon_Tx";
        public const string m_sRECORD_SDIGIGAIN_PTHF_RX             = "SettingDigiGain_PTHF_Rx";
        public const string m_sRECORD_RDIGIGAIN_PTHF_RX             = "ReadDigiGain_PTHF_Rx";
        public const string m_sRECORD_SDIGIGAIN_PTHF_TX             = "SettingDigiGain_PTHF_Tx";
        public const string m_sRECORD_RDIGIGAIN_PTHF_TX             = "ReadDigiGain_PTHF_Tx";
        public const string m_sRECORD_SDIGIGAIN_BHF_RX              = "SettingDigiGain_BHF_Rx";
        public const string m_sRECORD_RDIGIGAIN_BHF_RX              = "ReadDigiGain_BHF_Rx";
        public const string m_sRECORD_SDIGIGAIN_BHF_TX              = "SettingDigiGain_BHF_Tx";
        public const string m_sRECORD_RDIGIGAIN_BHF_TX              = "ReadDigiGain_BHF_Tx";
        public const string m_sRECORD_SPEN_HI_HF_THD                = "SettingPen_HI_HF_THD";
        public const string m_sRECORD_RPEN_HI_HF_THD                = "ReadPen_HI_HF_THD";
        public const string m_sRECORD_DIGIGAINCOMMANDENABLE         = "DigiGainCommandEnable";
        public const string m_sRECORD_5TRAWDATATYPE                 = "5TRawDataType";
        public const string m_sRECORD_TRACETYPE                     = "TraceType";
        public const string m_sRECORD_7318TRXSSPECIFICREPORTTYPE    = "7318TRxSSpecificReportType";
        public const string m_sRECORD_RXSTARTTRACE                  = "RXStartTrace";
        public const string m_sRECORD_TXSTARTTRACE                  = "TXStartTrace";
        #endregion

        #region Declare DrawLine Type
        public const string m_sDRAWTYPE_NA          = "NA";
        public const string m_sDRAWTYPE_HORIZONTAL  = "HorizontalLine";
        public const string m_sDRAWTYPE_VERTICAL    = "VerticalLine";
        public const string m_sDRAWTYPE_SLANT       = "SlantLine";
        public const string m_sDRAWTYPE_CENTERPOINT = "CenterPoint";
        #endregion

        #region Declare Trace Type
        public const string m_sTRACETYPE_NA = "NA";
        public const string m_sTRACETYPE_RX = "RX";
        public const string m_sTRACETYPE_TX = "TX";
        #endregion

        public static string ConvertStepToHoverHeight(frmMain cfrmMain)
        {
            SubTuningStep eSubStep = cfrmMain.m_cCurrentFlowStep.m_eSubStep;
            string sDataState = "";

            if (eSubStep != SubTuningStep.HOVER_1ST && eSubStep != SubTuningStep.HOVER_2ND &&
                eSubStep != SubTuningStep.HOVERTRxS && eSubStep != SubTuningStep.PRESSUREPROTECT &&
                eSubStep != SubTuningStep.PCHOVER_1ST && eSubStep != SubTuningStep.PCHOVER_2ND)
                return sDataState;
            
            double dHoverHeight = 0.0;

            switch (eSubStep)
            {
                case SubTuningStep.HOVER_1ST:
                case SubTuningStep.HOVERTRxS:
                    dHoverHeight = Math.Round(cfrmMain.m_dHoverHeight_DT1st, 2, MidpointRounding.AwayFromZero);
                    break;
                case SubTuningStep.HOVER_2ND:
                    dHoverHeight = Math.Round(cfrmMain.m_dHoverHeight_DT2nd, 2, MidpointRounding.AwayFromZero);
                    break;
                case SubTuningStep.PRESSUREPROTECT:
                    dHoverHeight = Math.Round(cfrmMain.m_dHoverHeight_PP, 2, MidpointRounding.AwayFromZero);
                    break;
                case SubTuningStep.PCHOVER_1ST:
                    dHoverHeight = Math.Round(cfrmMain.m_dHoverHeight_PCT1st, 2, MidpointRounding.AwayFromZero);
                    break;
                case SubTuningStep.PCHOVER_2ND:
                    dHoverHeight = Math.Round(cfrmMain.m_dHoverHeight_PCT2nd, 2, MidpointRounding.AwayFromZero);
                    break;
                default:
                    break;
            }

            sDataState = string.Format("({0}mm)", Convert.ToString(dHoverHeight));

            return sDataState;
        }

        public static string ConvertStepToStepListFilePath(frmMain cfrmMain, FlowStep cFlowStep, SubTuningStep eSubStep)
        {
            string sFileName = string.Format("{0}_{1}.csv", SpecificText.m_sStepListText, m_dictSubStepCNMappingTable[eSubStep]);

            string sDirectoryPath = "";

            switch (eSubStep)
            {
                case SubTuningStep.NO:
                    return string.Format(@"{0}\{1}", cfrmMain.m_sFileDirectoryPath_Noise, sFileName);
                case SubTuningStep.TILTNO_PTHF:
                case SubTuningStep.TILTNO_BHF:
                    sDirectoryPath = cfrmMain.m_sFileDirectoryPath;

                    if (cFlowStep.m_eMainStep != MainTuningStep.TILTNO)
                        sDirectoryPath = cfrmMain.m_sFileDirectoryPath_TN;

                    return string.Format(@"{0}\\{1}", sDirectoryPath, sFileName);
                case SubTuningStep.HOVER_1ST:
                case SubTuningStep.HOVER_2ND:
                case SubTuningStep.CONTACT:
                case SubTuningStep.HOVERTRxS:
                case SubTuningStep.CONTACTTRxS:
                    sDirectoryPath = cfrmMain.m_sFileDirectoryPath;

                    if (cFlowStep.m_eMainStep != MainTuningStep.DIGITALTUNING)
                        sDirectoryPath = cfrmMain.m_sFileDirectoryPath_DT;

                    return string.Format(@"{0}\\{1}", sDirectoryPath, sFileName);
                case SubTuningStep.DIGIGAIN:
                    sDirectoryPath = cfrmMain.m_sFileDirectoryPath;

                    if (cFlowStep.m_eMainStep != MainTuningStep.DIGIGAINTUNING)
                        sDirectoryPath = cfrmMain.m_sFileDirectoryPath_DGT;

                    return string.Format(@"{0}\\{1}", sDirectoryPath, sFileName);
                case SubTuningStep.TP_GAIN:
                    sDirectoryPath = cfrmMain.m_sFileDirectoryPath;

                    if (cFlowStep.m_eMainStep != MainTuningStep.TPGAINTUNING)
                        sDirectoryPath = cfrmMain.m_sFileDirectoryPath_TPGT;

                    return string.Format(@"{0}\\{1}", sDirectoryPath, sFileName);
                case SubTuningStep.TILTTUNING_PTHF:
                case SubTuningStep.TILTTUNING_BHF:
                case SubTuningStep.PRESSURESETTING:
                case SubTuningStep.PRESSUREPROTECT:
                case SubTuningStep.PRESSURETABLE:
                case SubTuningStep.LINEARITYTABLE:
                case SubTuningStep.PCHOVER_1ST:
                case SubTuningStep.PCHOVER_2ND:
                case SubTuningStep.PCCONTACT:
                case SubTuningStep.ELSE:
                    return string.Format(@"{0}\\{1}", cfrmMain.m_sFileDirectoryPath, sFileName);
                default:
                    return string.Format(@"{0}\\{1}", cfrmMain.m_sFileDirectoryPath, sFileName);
            }
        }

        public static bool CheckExceptionMessage(MainTuningStep eMainStep, SubTuningStep eSubStep, string sErrorMessage)
        {
            string[] sExceptionErrorMessage_Array = null;

            if (eMainStep == MainTuningStep.NO ||
                eMainStep == MainTuningStep.TILTNO)
            {
                sExceptionErrorMessage_Array = new string[1] 
                { 
                    "Inner Reference Value Over High Boundary" 
                };
            }
            else if (eMainStep == MainTuningStep.DIGITALTUNING)
            {
                sExceptionErrorMessage_Array = new string[1] 
                { 
                    "FW Parameter Setting Check Error"
                };
            }

            if (sExceptionErrorMessage_Array != null)
            {
                for (int nMessageIndex = 0; nMessageIndex < sExceptionErrorMessage_Array.Length; nMessageIndex++)
                {
                    if (sErrorMessage.Contains(sExceptionErrorMessage_Array[nMessageIndex]) == true)
                        return true;
                }
            }

            return false;
        }

        /*
        public static bool CheckExceptionMessage_DigitalTuning(string sErrorMessage)
        {
            string[] sExceptionErrorMessage_Array = new string[1] 
            { 
                "FW Parameter Setting Check Error" 
            };

            for (int nMessageIndex = 0; nMessageIndex < sExceptionErrorMessage_Array.Length; nMessageIndex++)
            {
                if (sErrorMessage.Contains(sExceptionErrorMessage_Array[nMessageIndex]) == true)
                    return true;
            }

            return false;
        }
        */

        public static bool CheckRecordInfoExist(string sLine, List<string> sRecordInfo_List)
        {
            bool bCheckFlag = false;

            for (int nInfoIndex = 0; nInfoIndex < sRecordInfo_List.Count; nInfoIndex++)
            {
                if (sRecordInfo_List[nInfoIndex] == StringConvert.m_sRECORD_DRAWLINESTATUS)
                {
                    string[] sDrawLineType_Array = new string[] 
                    { 
                        m_sDRAWTYPE_NA,
                        m_sDRAWTYPE_HORIZONTAL,
                        m_sDRAWTYPE_VERTICAL,
                        m_sDRAWTYPE_SLANT 
                    };

                    for (int nTypeIndex = 0; nTypeIndex < sDrawLineType_Array.Length; nTypeIndex++)
                    {
                        string sDrawLineType = string.Format("====={0}=====", sDrawLineType_Array[nTypeIndex]);

                        if (sLine == sDrawLineType)
                        {
                            bCheckFlag = true;
                            break;
                        }
                    }

                    if (bCheckFlag == true)
                        break;
                }
                else if (sRecordInfo_List[nInfoIndex] == StringConvert.m_sRECORD_WEIGHTSTATUS)
                {
                    for (int nWeightIndex = 0; nWeightIndex < ParamAutoTuning.m_nPressureWeight_Array.Length; nWeightIndex++)
                    {
                        string sPressureWeight = string.Format("====={0}g=====", ParamAutoTuning.m_nPressureWeight_Array[nWeightIndex]);

                        if (sLine == sPressureWeight)
                        {
                            bCheckFlag = true;
                            break;
                        }
                    }

                    if (bCheckFlag == true)
                        break;
                }
                else if (sRecordInfo_List[nInfoIndex] == StringConvert.m_sRECORD_TRACETYPE)
                {
                    string[] sDrawLineType_Array = new string[] 
                    { 
                        m_sTRACETYPE_NA,
                        m_sTRACETYPE_RX,
                        m_sTRACETYPE_TX  
                    };

                    for (int nTypeIndex = 0; nTypeIndex < sDrawLineType_Array.Length; nTypeIndex++)
                    {
                        string sTraceType = string.Format("====={0}=====", sDrawLineType_Array[nTypeIndex]);

                        if (sLine == sTraceType)
                        {
                            bCheckFlag = true;
                            break;
                        }
                    }

                    if (bCheckFlag == true)
                        break;
                }
                else
                {
                    string sParameterName = string.Format("{0}=", sRecordInfo_List[nInfoIndex]);

                    if (sLine.Contains(sParameterName) == true)
                    {
                        bCheckFlag = true;
                        break;
                    }
                }
            }

            return bCheckFlag;
        }

        private static double ComputeValueByOperator(double dLeftValue, double dRightValue, char charOperator)
        {
            switch (charOperator)
            {
                case '+':
                    return dLeftValue + dRightValue;
                case '-':
                    return dLeftValue - dRightValue;
                case '*':
                    return dLeftValue * dRightValue;
                case '/':
                    return dLeftValue / dRightValue;
            }

            return 0;
        }

        public static int GetCalculateResult(string sSource)
        {
            Stack<string> sData_Stack = new Stack<string>();
            var vList = sSource.Split(' ');

            for (int nStringIndex = 0; nStringIndex < vList.Length; nStringIndex++)
            {
                string sValue = vList[nStringIndex];

                if (ElanConvert.CheckIsInt(sValue) == true)
                {
                    sData_Stack.Push(sValue);
                }
                else if (OperatorLevelDictionary.ContainsKey(sValue))
                {
                    double dRight = double.Parse(sData_Stack.Pop());
                    double dLeft = double.Parse(sData_Stack.Pop());
                    sData_Stack.Push(ComputeValueByOperator(dLeft, dRight, sValue[0]).ToString());
                }
            }

            int nValue = (int)(double.Parse(sData_Stack.Pop()));

            return nValue;
        }

        public static UInt16 GetCalculateResult_Uint16(string sSource)
        {
            Stack<string> sData_Stack = new Stack<string>();
            var vList = sSource.Split(' ');

            for (int nStringIndex = 0; nStringIndex < vList.Length; nStringIndex++)
            {
                string sValue = vList[nStringIndex];

                if (ElanConvert.CheckIsInt(sValue) == true)
                {
                    sData_Stack.Push(sValue);
                }
                else if (OperatorLevelDictionary.ContainsKey(sValue))
                {
                    double dRight = double.Parse(sData_Stack.Pop());
                    double dLeft = double.Parse(sData_Stack.Pop());
                    sData_Stack.Push(ComputeValueByOperator(dLeft, dRight, sValue[0]).ToString());
                }
            }

            UInt16 nValue = (UInt16)(double.Parse(sData_Stack.Pop()));

            return nValue;
        }

        public static string ConvertStringToRPN(ref bool bErrorFlag, ref string sErrorMessage, string sSource)
        {
            bErrorFlag = false;
            sErrorMessage = "";
            StringBuilder sbResult = new StringBuilder();
            Stack<string> sData_Stack = new Stack<string>();
            string[] sList_Array = sSource.Split(' ');

            for (int nStringIndex = 0; nStringIndex < sList_Array.Length; nStringIndex++)
            {
                string sValue = sList_Array[nStringIndex];

                bool bHexFlag_1 = sValue.Contains("0x");
                bool bHexFlag_2 = sValue.Contains("0X");

                if (bHexFlag_1 == true || bHexFlag_2 == true)
                {
                    sValue = sValue.Replace("0x", "");
                    sValue = sValue.Replace("0X", "");

                    try
                    {
                        int nValue = Int32.Parse(sValue, System.Globalization.NumberStyles.HexNumber);
                        sbResult.Append(nValue.ToString() + " ");
                    }
                    catch(Exception ex)
                    {
                        bErrorFlag = true;
                        sErrorMessage = ex.Message.ToString();
                        return sbResult.ToString();
                    }
                }
                else if (ElanConvert.CheckIsInt(sValue) == true)
                {
                    sbResult.Append(sValue + " ");
                }
                else if (OperatorLevelDictionary.ContainsKey(sValue))
                {
                    if (sData_Stack.Count > 0)
                    {
                        var vPrevious = sData_Stack.Peek();

                        if (vPrevious == "(")
                        {
                            sData_Stack.Push(sValue);
                            continue;
                        }
                        if (sValue == "(")
                        {
                            sData_Stack.Push(sValue);
                            continue;
                        }
                        if (sValue == ")")
                        {
                            while (sData_Stack.Count > 0 && sData_Stack.Peek() != "(")
                            {
                                sbResult.Append(sData_Stack.Pop() + " ");
                            }

                            //Pop the "("
                            sData_Stack.Pop();
                            continue;
                        }
                        if (OperatorLevelDictionary[sValue] < OperatorLevelDictionary[vPrevious])
                        {
                            while (sData_Stack.Count > 0)
                            {
                                var vTop = sData_Stack.Pop();

                                if (vTop != "(" && vTop != ")")
                                {
                                    sbResult.Append(vTop + " ");
                                }
                                else
                                {
                                    break;
                                }
                            }

                            sData_Stack.Push(sValue);
                        }
                        else
                        {
                            sData_Stack.Push(sValue);
                        }
                    }
                    else
                    {
                        sData_Stack.Push(sValue);
                    }
                }
                else
                {
                    bErrorFlag = true;
                    sErrorMessage = "Error String Format";
                    return sbResult.ToString();
                }
            }

            if (sData_Stack.Count > 0)
            {
                while (sData_Stack.Count > 0)
                {
                    var vTop = sData_Stack.Pop();

                    if (vTop != "(" && vTop != ")")
                    {
                        sbResult.Append(vTop + " ");
                    }
                }
            }

            return sbResult.ToString();
        }

        public static string InsertBlank(string sSource)
        {
            StringBuilder sbResult = new StringBuilder();
            var vList = sSource.ToCharArray();

            foreach (var vValue in vList)
            {
                if (OperatorLevelDictionary.ContainsKey(vValue.ToString()))
                {
                    sbResult.Append(" ");
                    sbResult.Append(vValue.ToString());
                    sbResult.Append(" ");
                }
                else
                {
                    sbResult.Append(vValue);
                }
            }

            return sbResult.ToString();
        }

        //運算符字典 方便查詢運算符優先級
        private static Dictionary<string, int> OperatorLevelDictionary
        {
            get
            {
                if (m_dictOperatorLevel == null)
                {
                    m_dictOperatorLevel = new Dictionary<string, int>();
                    m_dictOperatorLevel.Add("+", 0);
                    m_dictOperatorLevel.Add("-", 0);
                    m_dictOperatorLevel.Add("(", 1);
                    m_dictOperatorLevel.Add("*", 1);
                    m_dictOperatorLevel.Add("/", 1);
                    m_dictOperatorLevel.Add(")", 0);
                }
                return m_dictOperatorLevel;
            }
        }
    }
}
