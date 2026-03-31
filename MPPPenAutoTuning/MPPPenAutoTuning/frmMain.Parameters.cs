using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MPPPenAutoTuning
{
    partial class frmMain
    {
        private SubTuningStep[] m_eNoiseSubStep_Array = new SubTuningStep[1] 
        { 
            SubTuningStep.NO 
        };

        private SubTuningStep[] m_eTiltNoiseSubStep_Array = new SubTuningStep[2] 
        { 
            SubTuningStep.TILTNO_PTHF,
            SubTuningStep.TILTNO_BHF 
        };

        private SubTuningStep[] m_eDigiGainTuningSubStep_Array = new SubTuningStep[1] 
        { 
            SubTuningStep.DIGIGAIN 
        };

        private SubTuningStep[] m_eTPGainTuningSubStep_Array = new SubTuningStep[1] 
        { 
            SubTuningStep.TP_GAIN 
        };

        private SubTuningStep[] m_ePeakCheckTuningSubStep_Array = new SubTuningStep[3] 
        { 
            SubTuningStep.PCHOVER_1ST,
            SubTuningStep.PCHOVER_2ND,
            SubTuningStep.PCCONTACT 
        };

        private SubTuningStep[] m_eDigitialTuningSubStep_Array = new SubTuningStep[5] 
        { 
            SubTuningStep.HOVER_1ST,
            SubTuningStep.HOVER_2ND,
            SubTuningStep.CONTACT,
            SubTuningStep.HOVERTRxS,
            SubTuningStep.CONTACTTRxS 
        };

        private SubTuningStep[] m_eTiltTuningSubStep_Array = new SubTuningStep[2] 
        { 
            SubTuningStep.TILTTUNING_PTHF,
            SubTuningStep.TILTTUNING_BHF 
        };

        private SubTuningStep[] m_ePressureTuningSubStep_Array = new SubTuningStep[2] 
        { 
            SubTuningStep.PRESSURESETTING, 
            SubTuningStep.PRESSURETABLE 
        };

        private SubTuningStep[] m_eLinearityTuningSubStep_Array = new SubTuningStep[1] 
        { 
            SubTuningStep.LINEARITYTABLE 
        };

        private SubTuningStep[] m_eServerControlSubStep_Array = new SubTuningStep[1] 
        {
            SubTuningStep.ELSE 
        };

        private SubTuningStep[] m_eElseSubStep_Array = new SubTuningStep[1] 
        { 
            SubTuningStep.ELSE 
        };

        public string[] m_sSubTuningStepFileName_Array = new string[] 
        { 
            "flow.txt",
            "flow_Hover.txt",
            "flow_Hover.txt",
            "flow_Contact.txt",
            "flow_HoverTRxS.txt",
            "flow_ContactTRxS.txt",
            "flow_TiltNoise_PTHF.txt",
            "flow_TiltNoise_BHF.txt",
            "flow_TiltTuning_PTHF.txt",
            "flow_TiltTuning_BHF.txt",
            "flow_PressureSetting.txt",
            "flow_PressureProtect.txt",
            "flow_PressureTable.txt",
            "flow_LinearityTable.txt",
            "flow_PCTuning_Hover.txt",
            "flow_PCTuning_Hover.txt",
            "flow_PCTuning_Contact.txt",
            "flow_DigiGain_Tuning.txt",
            "flow_TPGain_Tuning.txt",
            "flow_Else.txt"
        };

        public Color[] m_colorScreenColor_Array = new Color[]
        { 
            Color.AliceBlue, //No use
            Color.AliceBlue, //No use
            Color.Black,
            Color.White,
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.FromArgb(255, 0, 255, 0),
            Color.Yellow,
            Color.Cyan,
            Color.Magenta 
        };

        private List<string> m_sEqualSymbol_List = new List<string>() 
        { 
            SpecificText.m_sFileName,
            SpecificText.m_sPH1,
            SpecificText.m_sPH2,
            SpecificText.m_sFrequency_KHz,
            SpecificText.m_sErrorMessage,
            SpecificText.m_sProtectErrorMessage,
            SpecificText.m_sSettingErrorMessage,
            SpecificText.m_sTableErrorMessage 
        };

        public const int m_nOtherFlag = 0;
        public const int m_nInitialFlag = 1;
        public const int m_nMaximumFlag = 2;
        public const int m_nStepOutputFlag = 3;

        private const int m_nCommonList = 0;
        private const int m_nTotalList = 1;

        public class MainStepCostTimeInfo
        {
            public MainTuningStep m_eMainStep = MainTuningStep.ELSE;
            public string m_sMainStepName = "";
            public string m_sMainStepCostTime = "";
        }
    }
}
