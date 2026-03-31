using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace MPPPenAutoTuning
{
    public partial class frmResultChart : Form
    {
        private frmMain m_cfrmMain = null;

        private string m_sDataFolderPath;
        private bool m_bOnFormLoad = false;

        private List<string> m_sDataName_DTHover1st_List = new List<string>();
        private List<string> m_sDataName_DTHover2nd_List = new List<string>();
        private List<string> m_sDataName_DTContact_List = new List<string>();
        private List<string> m_sDataName_DTHoverTRxS_List = new List<string>();
        private List<string> m_sDataName_DTContactTRxS_List = new List<string>();

        private string[] m_sNoiseTraceLineChartItem_Array = new string[]
        {
            "Top 5",
            "All"
        };

        private string[] m_sTNTraceLineChartItem_Array = new string[]
        {
            "PTHF Top 5",
            "PTHF All",
            "BHF Top 5",
            "BHF All",
            //"Total Top 5",
            //"Total All"
        };

        // Receive Data Folder Path from frmMain
        public string DataFolderPath
        {
            get
            {
                return m_sDataFolderPath;
            }

            set
            {
                m_sDataFolderPath = value;
            }
        }

        public frmResultChart(frmMain cfrmMain)
        {
            InitializeComponent();

            m_cfrmMain = cfrmMain;

            foreach (string sItem in m_sNoiseTraceLineChartItem_Array)
                cbxNoiseTraceLineChartData.Items.Add(sItem);

            cbxNoiseTraceLineChartData.SelectedIndex = 0;

            foreach (string sItem in m_sTNTraceLineChartItem_Array)
                cbxTNTraceLineChartData.Items.Add(sItem);

            cbxTNTraceLineChartData.SelectedIndex = 0;
        }

        private void InitializeParameter()
        {
            cbxDTHover1stDataName.Items.Clear();
            cbxDTHover2ndDataName.Items.Clear();
            cbxDTContactDataName.Items.Clear();
            cbxDTHoverTRxSDataName.Items.Clear();
            cbxDTContactTRxSDataName.Items.Clear();

            tpgNoise.Parent = null;
            tpgDigitalTuning.Parent = null;
            tpgTiltNoise.Parent = null;

            tpgDTHover1stHisto.Parent = null;
            tpgDTHover2ndHisto.Parent = null;
            tpgDTContactHisto.Parent = null;
            tpgDTHoverTRxS.Parent = null;
            tpgDTContactTRxS.Parent = null;

            m_sDataName_DTHover1st_List.Clear();
            m_sDataName_DTHover2nd_List.Clear();
            m_sDataName_DTContact_List.Clear();
            m_sDataName_DTHoverTRxS_List.Clear();
            m_sDataName_DTContactTRxS_List.Clear();
        }

        private void frmChart_Load(object sender, EventArgs e)
        {
            string sErrorMessage = "";

            InitializeParameter();

            for (int nStepIndex = 0; nStepIndex < m_cfrmMain.m_cFlowStepResult_List.Count; nStepIndex++)
            {
                bool bSetChartDataItemFlag = false;

                MainTuningStep eMainStep = m_cfrmMain.m_cFlowStepResult_List[nStepIndex].m_eMainStep;
                SubTuningStep eSubStep = m_cfrmMain.m_cFlowStepResult_List[nStepIndex].m_eSubStep;

                switch (eMainStep)
                {
                    case MainTuningStep.NO:
                        if (eSubStep == SubTuningStep.NO)
                            bSetChartDataItemFlag = true;
                        break;
                    case MainTuningStep.TILTNO:
                        if (eSubStep == SubTuningStep.TILTNO_PTHF ||
                            eSubStep == SubTuningStep.TILTNO_BHF)
                            bSetChartDataItemFlag = true;
                        break;
                    case MainTuningStep.DIGITALTUNING:
                        if (eSubStep == SubTuningStep.HOVER_1ST ||
                            eSubStep == SubTuningStep.HOVER_2ND ||
                            eSubStep == SubTuningStep.CONTACT ||
                            eSubStep == SubTuningStep.HOVERTRxS ||
                            eSubStep == SubTuningStep.CONTACTTRxS)
                            bSetChartDataItemFlag = true;
                        break;
                    default:
                        break;
                }

                if (bSetChartDataItemFlag == true && SetChartDataItem(eMainStep, eSubStep, nStepIndex) == true)
                    SetPictureBoxByStep(ref sErrorMessage, eSubStep);
            }

            if (sErrorMessage != "")
            {
                ShowMessageBox(sErrorMessage, frmMessageBox.m_sError);
            }

            m_bOnFormLoad = true;
        }

        private bool SetChartDataItem(MainTuningStep eMainStep, SubTuningStep eSubStep, int nStepIndex)
        {
            bool bSetPrintFlag = false;

            bool bStepErrorFlag = m_cfrmMain.m_cFlowStepResult_List[nStepIndex].m_bStepErrorFlag;
            string sStepErrorMessage = m_cfrmMain.m_cFlowStepResult_List[nStepIndex].m_sStepErrorMessage;
            List<string> sDataFileName_List = m_cfrmMain.m_cFlowStepResult_List[nStepIndex].m_sDataFileName_List;

            switch(eSubStep)
            {
                case SubTuningStep.NO:
                    if (bStepErrorFlag == false || StringConvert.CheckExceptionMessage(eMainStep, eSubStep, sStepErrorMessage) == true)
                    {
                        tpgNoise.Parent = tcMain;
                        bSetPrintFlag = true;
                    }

                    break;
                case SubTuningStep.TILTNO_PTHF:
                case SubTuningStep.TILTNO_BHF:
                    if (bStepErrorFlag == false || StringConvert.CheckExceptionMessage(eMainStep, eSubStep, sStepErrorMessage) == true)
                    {
                        tpgTiltNoise.Parent = tcMain;
                        bSetPrintFlag = true;
                    }

                    break;
                case SubTuningStep.HOVER_1ST:
                    if (bStepErrorFlag == false || StringConvert.CheckExceptionMessage(eMainStep, eSubStep, sStepErrorMessage) == true)
                    {
                        tpgDigitalTuning.Parent = tcMain;
                        tpgDTHover1stHisto.Parent = tcDigitalTuning;

                        if (sDataFileName_List != null || sDataFileName_List.Count == 0)
                            m_sDataName_DTHover1st_List.AddRange(sDataFileName_List);

                        if (m_sDataName_DTHover1st_List != null && m_sDataName_DTHover1st_List.Count > 0)
                            bSetPrintFlag = true;
                    }

                    break;
                case SubTuningStep.HOVER_2ND:
                    if (bStepErrorFlag == false || StringConvert.CheckExceptionMessage(eMainStep, eSubStep, sStepErrorMessage) == true)
                    {
                        tpgDigitalTuning.Parent = tcMain;
                        tpgDTHover2ndHisto.Parent = tcDigitalTuning;

                        if (sDataFileName_List != null || sDataFileName_List.Count == 0)
                            m_sDataName_DTHover2nd_List.AddRange(sDataFileName_List);

                        if (m_sDataName_DTHover2nd_List != null && m_sDataName_DTHover2nd_List.Count > 0)
                            bSetPrintFlag = true;
                    }

                    break;
                case SubTuningStep.CONTACT:
                    if (bStepErrorFlag == false || StringConvert.CheckExceptionMessage(eMainStep, eSubStep, sStepErrorMessage) == true)
                    {
                        tpgDigitalTuning.Parent = tcMain;
                        tpgDTContactHisto.Parent = tcDigitalTuning;

                        if (sDataFileName_List != null || sDataFileName_List.Count == 0)
                            m_sDataName_DTContact_List.AddRange(sDataFileName_List);

                        if (m_sDataName_DTContact_List != null && m_sDataName_DTContact_List.Count > 0)
                            bSetPrintFlag = true;
                    }

                    break;
                case SubTuningStep.HOVERTRxS:
                    if (bStepErrorFlag == false || StringConvert.CheckExceptionMessage(eMainStep, eSubStep, sStepErrorMessage) == true)
                    {
                        tpgDigitalTuning.Parent = tcMain;
                        tpgDTHoverTRxS.Parent = tcDigitalTuning;

                        if (sDataFileName_List != null || sDataFileName_List.Count == 0)
                            m_sDataName_DTHoverTRxS_List.AddRange(sDataFileName_List);

                        if (m_sDataName_DTHoverTRxS_List != null && m_sDataName_DTHoverTRxS_List.Count > 0)
                            bSetPrintFlag = true;
                    }

                    break;
                case SubTuningStep.CONTACTTRxS:
                    if (bStepErrorFlag == false || StringConvert.CheckExceptionMessage(eMainStep, eSubStep, sStepErrorMessage) == true)
                    {
                        tpgDigitalTuning.Parent = tcMain;
                        tpgDTContactTRxS.Parent = tcDigitalTuning;

                        if (sDataFileName_List != null || sDataFileName_List.Count == 0)
                            m_sDataName_DTContactTRxS_List.AddRange(sDataFileName_List);

                        if (m_sDataName_DTContactTRxS_List != null && m_sDataName_DTContactTRxS_List.Count > 0)
                            bSetPrintFlag = true;
                    }

                    break;
            }

            return bSetPrintFlag;
        }

        private void SetPictureBoxByStep(ref string sErrorMessage, SubTuningStep eSubStep)
        {
            string sSubStepCodeName = StringConvert.m_dictSubStepCNMappingTable[eSubStep];
            string sStepFolderPath = string.Format(@"{0}\{1}({2})", m_sDataFolderPath, SpecificText.m_sResultText, sSubStepCodeName);

            if (Directory.Exists(sStepFolderPath) == false)
                sErrorMessage += string.Format("\"{0}({1})\" Folder Not Exist", SpecificText.m_sResultText, sSubStepCodeName);
            else
            {
                string sRXPictureFileName = "";
                string sTXPictureFileName = "";

                switch (eSubStep)
                {
                    case SubTuningStep.NO:
                        if (cbxNoiseTraceLineChartData.SelectedIndex == 0)
                        {
                            sRXPictureFileName = string.Format("RX_{0}", SpecificText.m_sChart_TopDataFileName);
                            sTXPictureFileName = string.Format("TX_{0}", SpecificText.m_sChart_TopDataFileName);
                        }
                        else if (cbxNoiseTraceLineChartData.SelectedIndex == 1)
                        {
                            sRXPictureFileName = string.Format("RX_{0}", SpecificText.m_sChart_TopDataFileName);
                            sTXPictureFileName = string.Format("TX_{0}", SpecificText.m_sChart_TopDataFileName);
                        }

                        break;
                    case SubTuningStep.TILTNO_PTHF:
                    case SubTuningStep.TILTNO_BHF:
                        switch(cbxTNTraceLineChartData.SelectedIndex)
                        {
                            case 0:
                            case 2:
                                sRXPictureFileName = string.Format("RX_{0}", SpecificText.m_sChart_TopDataFileName);
                                sTXPictureFileName = string.Format("TX_{0}", SpecificText.m_sChart_TopDataFileName);
                                break;
                            case 1:
                            case 3:
                                sRXPictureFileName = string.Format("RX_{0}", SpecificText.m_sChart_TopDataFileName);
                                sTXPictureFileName = string.Format("TX_{0}", SpecificText.m_sChart_TopDataFileName);
                                break;
                            case 4:
                                sRXPictureFileName = string.Format("TN_RX_{0}", SpecificText.m_sChart_TopDataFileName);
                                sTXPictureFileName = string.Format("TN_TX_{0}", SpecificText.m_sChart_TopDataFileName);
                                break;
                            case 5:
                                sRXPictureFileName = string.Format("TN_RX_{0}", SpecificText.m_sChart_TopDataFileName);
                                sTXPictureFileName = string.Format("TN_TX_{0}", SpecificText.m_sChart_TopDataFileName);
                                break;
                            default:
                                break;
                        }

                        break;
                    case SubTuningStep.HOVER_1ST:
                    case SubTuningStep.HOVER_2ND:
                    case SubTuningStep.CONTACT:
                        sRXPictureFileName = string.Format("RX_{0}", SpecificText.m_sHistogramFileName);
                        sTXPictureFileName = string.Format("TX_{0}", SpecificText.m_sHistogramFileName);
                        break;
                    case SubTuningStep.HOVERTRxS:
                    case SubTuningStep.CONTACTTRxS:
                        sRXPictureFileName = string.Format("RX_{0}", SpecificText.m_sChartFileName);
                        sTXPictureFileName = string.Format("TX_{0}", SpecificText.m_sChartFileName);
                        break;
                }

                switch (eSubStep)
                {
                    case SubTuningStep.NO:
                    case SubTuningStep.TILTNO_PTHF:
                    case SubTuningStep.TILTNO_BHF:
                        if (eSubStep == SubTuningStep.TILTNO_BHF &&
                            (cbxTNTraceLineChartData.SelectedIndex == 0 || cbxTNTraceLineChartData.SelectedIndex == 1))
                            SetFrequencyPictureBox(ref sErrorMessage, sStepFolderPath, eSubStep, ckbxTNFrequencyLineIncludeMaxValue.Checked);
                        else
                        {
                            string sRXPictureFilePath = string.Format(@"{0}\{1}\{2}", sStepFolderPath, SpecificText.m_sPictureText, sRXPictureFileName);
                            string sTXPictureFilePath = string.Format(@"{0}\{1}\{2}", sStepFolderPath, SpecificText.m_sPictureText, sTXPictureFileName);

                            SetTracePictureBox(ref sErrorMessage, sRXPictureFilePath, sTXPictureFilePath, eSubStep);

                            if (eSubStep == SubTuningStep.NO)
                                SetFrequencyPictureBox(ref sErrorMessage, sStepFolderPath, eSubStep, ckbxNoiseFreqChartIncludeMaxValue.Checked);
                            else if (eSubStep == SubTuningStep.TILTNO_PTHF || eSubStep == SubTuningStep.TILTNO_BHF)
                                SetFrequencyPictureBox(ref sErrorMessage, sStepFolderPath, eSubStep, ckbxTNFrequencyLineIncludeMaxValue.Checked);
                        }

                        break;
                    case SubTuningStep.HOVER_1ST:
                    case SubTuningStep.HOVER_2ND:
                    case SubTuningStep.CONTACT:
                    case SubTuningStep.HOVERTRxS:
                    case SubTuningStep.CONTACTTRxS:
                        PictureBoxInfo_DigitalTuning cPictureBoxInfo_DigitalTuning;

                        switch (eSubStep)
                        {
                            case SubTuningStep.HOVER_1ST:
                                cPictureBoxInfo_DigitalTuning = new PictureBoxInfo_DigitalTuning(m_sDataName_DTHover1st_List, 
                                                                                                 cbxDTHover1stDataName, 
                                                                                                 picbxDTHover1stRXHisto, 
                                                                                                 picbxDTHover1stTXHisto);
                                break;
                            case SubTuningStep.HOVER_2ND:
                                cPictureBoxInfo_DigitalTuning = new PictureBoxInfo_DigitalTuning(m_sDataName_DTHover2nd_List, 
                                                                                                 cbxDTHover2ndDataName,
                                                                                                 picbxDTHover2ndRXHisto, 
                                                                                                 picbxDTHover2ndTXHisto);
                                break;
                            case SubTuningStep.CONTACT:
                                cPictureBoxInfo_DigitalTuning = new PictureBoxInfo_DigitalTuning(m_sDataName_DTContact_List, 
                                                                                                 cbxDTContactDataName,
                                                                                                 picbxDTContactRXHisto, 
                                                                                                 picbxDTContactTXHisto);
                                break;
                            case SubTuningStep.HOVERTRxS:
                                cPictureBoxInfo_DigitalTuning = new PictureBoxInfo_DigitalTuning(m_sDataName_DTHoverTRxS_List, 
                                                                                                 cbxDTHoverTRxSDataName,
                                                                                                 picbxDTHoverTRxSRX, 
                                                                                                 picbxDTHoverTRxSTX);
                                break;
                            case SubTuningStep.CONTACTTRxS:
                                cPictureBoxInfo_DigitalTuning = new PictureBoxInfo_DigitalTuning(m_sDataName_DTContactTRxS_List, 
                                                                                                 cbxDTContactTRxSDataName,
                                                                                                 picbxDTContactTRxSRX, 
                                                                                                 picbxDTContactTRxSTX);
                                break;
                            default:
                                cPictureBoxInfo_DigitalTuning = null;
                                break;
                        }

                        if (cPictureBoxInfo_DigitalTuning.m_sDataName_List != null || cPictureBoxInfo_DigitalTuning.m_sDataName_List.Count == 0)
                        {
                            for (int nIndex = 0; nIndex < cPictureBoxInfo_DigitalTuning.m_sDataName_List.Count; nIndex++)
                                cPictureBoxInfo_DigitalTuning.m_cbxDataName.Items.Add(cPictureBoxInfo_DigitalTuning.m_sDataName_List[nIndex]);

                            cPictureBoxInfo_DigitalTuning.m_cbxDataName.SelectedIndex = 0;

                            string sFolderName = cPictureBoxInfo_DigitalTuning.m_cbxDataName.SelectedItem.ToString();

                            string sRXPictureFilePath = string.Format(@"{0}\{1}\{2}\{3}", sStepFolderPath, sFolderName, SpecificText.m_sPictureText, sRXPictureFileName);
                            string sTXPictureFilePath = string.Format(@"{0}\{1}\{2}\{3}", sStepFolderPath, sFolderName, SpecificText.m_sPictureText, sTXPictureFileName);

                            SetRXTXPictureBox_DigitalTuning(ref sErrorMessage, cPictureBoxInfo_DigitalTuning.m_picbxRXChart, cPictureBoxInfo_DigitalTuning.m_picbxTXChart, sRXPictureFilePath, sTXPictureFilePath);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetTracePictureBox(ref string sErrorMessage, string sRXPictureFilePath, string sTXPictureFilePath, SubTuningStep eSubStep)
        {
            PictureBox picbxRXChart = null;
            PictureBox picbxTXChart = null;

            switch (eSubStep)
            {
                case SubTuningStep.NO:
                    picbxRXChart = picbxNoiseRXTrace;
                    picbxTXChart = picbxNoiseTXTrace;
                    break;
                case SubTuningStep.TILTNO_PTHF:
                case SubTuningStep.TILTNO_BHF:
                    picbxRXChart = picbxTNRXTrace;
                    picbxTXChart = picbxTNTXTrace;
                    break;
                default:
                    break;
            }

            SetPictureBox(ref sErrorMessage, "RX_Chart", sRXPictureFilePath, picbxRXChart);
            SetPictureBox(ref sErrorMessage, "TX_Chart", sTXPictureFilePath, picbxTXChart);
        }

        private void SetFrequencyPictureBox(ref string sErrorMessage, string sStepFolderPath, SubTuningStep eSubStep, bool bIncludeMaxFlag = false)
        {
            if (sStepFolderPath != string.Empty)
            {
                PictureBox[] picbxChart_Array = null;
                string[] sFileName_Array = null;

                if (bIncludeMaxFlag == false)
                {
                    sFileName_Array = new string[5] 
                    { 
                        string.Format("RXInner_{0}", SpecificText.m_sFrqChartFileName),
                        string.Format("RXEdge_{0}", SpecificText.m_sFrqChartFileName),
                        string.Format("TXInner_{0}", SpecificText.m_sFrqChartFileName),
                        string.Format("TXEdge_{0}", SpecificText.m_sFrqChartFileName),
                        string.Format("Total_{0}", SpecificText.m_sFrqChartFileName) 
                    };
                }
                else
                {
                    sFileName_Array = new string[5] 
                    { 
                        string.Format("RXInner_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                        string.Format("RXEdge_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                        string.Format("TXInner_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                        string.Format("TXEdge_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                        string.Format("Total_{0}", SpecificText.m_sFrqChartIncludeMaxFileName) 
                    };
                }

                string[] sErrorMessage_Array = null;

                switch (eSubStep)
                {
                    case SubTuningStep.NO:
                        picbxChart_Array = new PictureBox[5] 
                        { 
                            picbxNoiseRXInnerFreq,
                            picbxNoiseRXEdgeFreq,
                            picbxNoiseTXInnerFreq,
                            picbxNoiseTXEdgeFreq,
                            picbxNoiseTotalFreq 
                        };

                        sErrorMessage_Array = new string[5] 
                        { 
                            "RXInner",
                            "RXEdge",
                            "TXInner",
                            "TXEdge",
                            "Total" 
                        };

                        break;
                    case SubTuningStep.TILTNO_PTHF:
                        picbxChart_Array = new PictureBox[5] 
                        { 
                            picbxTNPTHFRXInnerFreq,
                            picbxTNPTHFRXEdgeFreq,
                            picbxTNPTHFTXInnerFreq,
                            picbxTNPTHFTXEdgeFreq,
                            picbxTNPTHFFreq         
                        };

                        if (bIncludeMaxFlag == false)
                        {
                            sFileName_Array = new string[5] 
                            { 
                                string.Format("RXInner_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("RXEdge_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("TXInner_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("TXEdge_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("Total_{0}", SpecificText.m_sFrqChartFileName)  
                            };
                        }
                        else
                        {
                            sFileName_Array = new string[5] 
                            { 
                                string.Format("RXInner_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("RXEdge_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("TXInner_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("TXEdge_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("Total_{0}", SpecificText.m_sFrqChartIncludeMaxFileName)  
                            };
                        }

                        sErrorMessage_Array = new string[5] 
                        { 
                            "PTHF RXInner",
                            "PTHF RXEdge",
                            "PTHF TXInner",
                            "PTHF TXEdge",
                            "PTHF Total"    
                        };

                        break;
                    case SubTuningStep.TILTNO_BHF:
                        picbxChart_Array = new PictureBox[11] 
                        { 
                            picbxTNBHFRXInnerFreq,
                            picbxTNBHFRXEdgeFreq,
                            picbxTNBHFTXInnerFreq,
                            picbxTNBHFTXEdgeFreq,
                            picbxTNBHFFreq,
                            picbxTNTotalRXInnerFreq,
                            picbxTNTotalRXEdgeFreq,
                            picbxTNTotalTXInnerFreq,
                            picbxTNTotalTXEdgeFreq,
                            picbxTNTotalFreq,
                            picbxCompositeLineChart 
                        };

                        if (bIncludeMaxFlag == false)
                        {
                            sFileName_Array = new string[11] 
                            { 
                                string.Format("RXInner_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("RXEdge_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("TXInner_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("TXEdge_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("Total_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("TN_RXInner_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("TN_RXEdge_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("TN_TXInner_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("TN_TXEdge_{0}", SpecificText.m_sFrqChartFileName),
                                string.Format("TN_Total_{0}", SpecificText.m_sFrqChartFileName),
                                SpecificText.m_sFrqChartBy3SubPlotFileName 
                            };
                        }
                        else
                        {
                            sFileName_Array = new string[11] 
                            { 
                                string.Format("RXInner_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("RXEdge_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("TXInner_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("TXEdge_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("Total_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("TN_RXInner_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("TN_RXEdge_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("TN_TXInner_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("TN_TXEdge_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                string.Format("TN_Total_{0}", SpecificText.m_sFrqChartIncludeMaxFileName),
                                SpecificText.m_sFrqChartBy3SubPlotIncludeMaxFileName 
                            };
                        }

                        sErrorMessage_Array = new string[11] 
                        { 
                            "BHF RXInner",
                            "BHF RXEdge",
                            "BHF TXInner",
                            "BHF TXEdge",
                            "BHF Total",
                            "TiltNoise RXInner",
                            "TiltNoise RXEdge",
                            "TiltNoise TXInner",
                            "TiltNoise TXEdge",
                            "TiltNoise Total",
                            "Composite"         
                        };

                        break;
                    default:
                        break;
                }

                for (int nControlIndex = 0; nControlIndex < picbxChart_Array.Length; nControlIndex++)
                {
                    string sPicturePath = string.Format(@"{0}\{1}\{2}", sStepFolderPath, SpecificText.m_sPictureText, sFileName_Array[nControlIndex]);
                    SetPictureBox(ref sErrorMessage, string.Format("{0}_FrequencyChart", sErrorMessage_Array[nControlIndex]), sPicturePath, picbxChart_Array[nControlIndex]);
                }
            }
        }

        private void cbxNoiseTraceLineChartData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_bOnFormLoad == false)
                return;

            string sErrorMessage = "";
            string sRXPicFileName = "";
            string sTXPicFileName = "";
            string sStepFolderPath = string.Format(@"{0}\{1}({2})", m_sDataFolderPath, SpecificText.m_sResultText, StringConvert.m_dictSubStepCNMappingTable[SubTuningStep.NO]);
            ComboBox cbxItem = sender as ComboBox;

            switch (cbxItem.SelectedIndex)
            {
                case 0:
                    sRXPicFileName = string.Format("RX_{0}", SpecificText.m_sChart_TopDataFileName);
                    sTXPicFileName = string.Format("TX_{0}", SpecificText.m_sChart_TopDataFileName);
                    break;
                case 1:
                    sRXPicFileName = string.Format("RX_{0}", SpecificText.m_sChartFileName);
                    sTXPicFileName = string.Format("TX_{0}", SpecificText.m_sChartFileName);
                    break;
                default:
                    break;
            }

            string sRXPictureFilePath = string.Format(@"{0}\{1}\{2}", sStepFolderPath, SpecificText.m_sPictureText, sRXPicFileName);
            string sTXPictureFilePath = string.Format(@"{0}\{1}\{2}", sStepFolderPath, SpecificText.m_sPictureText, sTXPicFileName);

            SetTracePictureBox(ref sErrorMessage, sRXPictureFilePath, sTXPictureFilePath, SubTuningStep.NO);

            if (sErrorMessage != "")
            {
                ShowMessageBox(sErrorMessage, frmMessageBox.m_sError);
            }
        }

        private void cbxDTHover1stDataName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPictureBox_DigitalTuning(SubTuningStep.HOVER_1ST, sender, picbxDTHover1stRXHisto, picbxDTHover1stTXHisto);
        }
        
        private void cbxDTHover2ndDataName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPictureBox_DigitalTuning(SubTuningStep.HOVER_2ND, sender, picbxDTHover2ndRXHisto, picbxDTHover2ndTXHisto);
        }

        private void cbxDTContactDataName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPictureBox_DigitalTuning(SubTuningStep.CONTACT, sender, picbxDTContactRXHisto, picbxDTContactTXHisto);
        }

        private void cbxDTHoverTRxSDataName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPictureBox_DigitalTuning(SubTuningStep.HOVERTRxS, sender, picbxDTHoverTRxSRX, picbxDTHoverTRxSTX);
        }

        private void cbxDTContactTRxSDataName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPictureBox_DigitalTuning(SubTuningStep.CONTACTTRxS, sender, picbxDTContactTRxSRX, picbxDTContactTRxSTX);
        }

        private void SetPictureBox_DigitalTuning(SubTuningStep eSubStep, object sender, PictureBox picbxRXChart, PictureBox picbxTXChart)
        {
            if (m_bOnFormLoad == false)
                return;

            ComboBox cbxItem = sender as ComboBox;

            string sErrorMessage = "";
            string sStepFolderPath = string.Format(@"{0}\{1}({2})", m_sDataFolderPath, SpecificText.m_sResultText, StringConvert.m_dictSubStepCNMappingTable[eSubStep]);
            string sFolderName = cbxItem.SelectedItem.ToString();
            string sRXFileName = "", sTXFileName = "";

            switch (eSubStep)
            {
                case SubTuningStep.HOVER_1ST:
                case SubTuningStep.HOVER_2ND:
                case SubTuningStep.CONTACT:
                    sRXFileName = string.Format("RX_{0}", SpecificText.m_sHistogramFileName);
                    sTXFileName = string.Format("TX_{0}", SpecificText.m_sHistogramFileName);
                    break;
                case SubTuningStep.HOVERTRxS:
                case SubTuningStep.CONTACTTRxS:
                    sRXFileName = string.Format("RX_{0}", SpecificText.m_sChartFileName);
                    sTXFileName = string.Format("TX_{0}", SpecificText.m_sChartFileName);
                    break;
                default:
                    break;
            }

            string sRXPicFilePath = string.Format(@"{0}\{1}\{2}\{3}", sStepFolderPath, sFolderName, SpecificText.m_sPictureText, sRXFileName);
            string sTXPicFilePath = string.Format(@"{0}\{1}\{2}\{3}", sStepFolderPath, sFolderName, SpecificText.m_sPictureText, sTXFileName);

            SetRXTXPictureBox_DigitalTuning(ref sErrorMessage, picbxRXChart, picbxTXChart, sRXPicFilePath, sTXPicFilePath);

            if (sErrorMessage != "")
            {
                ShowMessageBox(sErrorMessage, frmMessageBox.m_sError);
            }
        }

        private void SetRXTXPictureBox_DigitalTuning(ref string sErrorMessage, PictureBox picbxRXChart, PictureBox picbxTXChart, string sRXPictureFilePath, string sTXPictureFilePath)
        {
            SetPictureBox(ref sErrorMessage, "RX", sRXPictureFilePath, picbxRXChart);
            SetPictureBox(ref sErrorMessage, "TX", sTXPictureFilePath, picbxTXChart);
        }

        private void SetPictureBox(ref string sErrorMessage, string sChartType, string sPictureFilePath, PictureBox picbxChart)
        {
            if (sPictureFilePath != string.Empty)
            {
                try
                {
                    FileStream fsPictureFile = new FileStream(sPictureFilePath, FileMode.Open, FileAccess.Read);
                    picbxChart.Image = System.Drawing.Image.FromStream(fsPictureFile);
                    fsPictureFile.Close();
                }
                catch
                {
                    picbxChart.Image = null;
                    sErrorMessage += string.Format("Load {0} Picture File Error", sChartType) + Environment.NewLine;
                }
            }
        }

        private void cbxTNTraceLineChartData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_bOnFormLoad == false)
                return;

            string sErrorMessage = "";
            string sSubStepCodeName = "";
            SubTuningStep eSubStep = SubTuningStep.TILTNO_PTHF;
            ComboBox cbxItem = sender as ComboBox;

            switch (cbxItem.SelectedIndex)
            {
                case 0:
                case 1:
                    sSubStepCodeName = StringConvert.m_dictSubStepCNMappingTable[SubTuningStep.TILTNO_PTHF];
                    eSubStep = SubTuningStep.TILTNO_PTHF;
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    sSubStepCodeName = StringConvert.m_dictSubStepCNMappingTable[SubTuningStep.TILTNO_BHF];
                    eSubStep = SubTuningStep.TILTNO_BHF;
                    break;
                default:
                    break;
            }

            string sStepFolderPath = string.Format(@"{0}\{1}({2})", m_sDataFolderPath, SpecificText.m_sResultText, sSubStepCodeName);

            string sRXPictureFileName = "";
            string sTXPictureFileName = "";

            switch (cbxItem.SelectedIndex)
            {
                case 0:
                case 2:
                    sRXPictureFileName = string.Format("RX_{0}", SpecificText.m_sChart_TopDataFileName);
                    sTXPictureFileName = string.Format("TX_{0}", SpecificText.m_sChart_TopDataFileName);
                    break;
                case 1:
                case 3:
                    sRXPictureFileName = string.Format("RX_{0}", SpecificText.m_sChartFileName);
                    sTXPictureFileName = string.Format("TX_{0}", SpecificText.m_sChartFileName);
                    break;
                case 4:
                    sRXPictureFileName = string.Format("TN_RX_{0}", SpecificText.m_sChart_TopDataFileName);
                    sTXPictureFileName = string.Format("TN_TX_{0}", SpecificText.m_sChart_TopDataFileName);
                    break;
                case 5:
                    sRXPictureFileName = string.Format("TN_RX_{0}", SpecificText.m_sChart_TopDataFileName);
                    sTXPictureFileName = string.Format("TN_TX_{0}", SpecificText.m_sChart_TopDataFileName);
                    break;
                default:
                    break;
            }

            string sRXPictureFilePath = string.Format(@"{0}\{1}\{2}", sStepFolderPath, SpecificText.m_sPictureText, sRXPictureFileName);
            string sTXPictureFilePath = string.Format(@"{0}\{1}\{2}", sStepFolderPath, SpecificText.m_sPictureText, sTXPictureFileName);

            SetTracePictureBox(ref sErrorMessage, sRXPictureFilePath, sTXPictureFilePath, eSubStep);

            if (sErrorMessage != "")
            {
                ShowMessageBox(sErrorMessage, frmMessageBox.m_sError);
            }
        }

        private void scNoiseTraceLineChart_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scNoiseRXFreqLineChart_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scNoiseTXFreqLineChart_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scTNTraceLineChart_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scTNPTHFRXFreqLineChart_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scTNPTHFTXFreqLineChart_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scTNBHFRXFreqLineChart_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scTNBHFTXFreqLineChart_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scTNTotalRXLineChart_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scTNTotalTXFreqLineChart_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scDTHover1stHisto_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scDTHover2ndHisto_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scDTContactHisto_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scDTHoverTRxS_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void scDTContactTRxS_Resize(object sender, EventArgs e)
        {
            SetSplitContainerDistance(sender);
        }

        private void SetSplitContainerDistance(object sender)
        {
            SplitContainer splitContainer = sender as SplitContainer;
            int nTotalHeight = splitContainer.Height - splitContainer.SplitterWidth;
            int nPanel1Height = (int)((double)nTotalHeight * 0.5);

            if (nPanel1Height > 0)
                splitContainer.SplitterDistance = nPanel1Height;
        }

        private frmMessageBox.ReturnStatus ShowMessageBox(string sMessage, string sTitle = frmMessageBox.m_sWarining, string sConfirmButton = "OK")
        {
            frmMessageBox cfrmMessageBox = new frmMessageBox(sTitle, sConfirmButton);
            cfrmMessageBox.ShowMessage(sMessage);

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmMessageBox.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmMessageBox.Height / 2);

            cfrmMessageBox.StartPosition = FormStartPosition.Manual;
            cfrmMessageBox.Location = new Point(nLocationX, nLocationY);

            cfrmMessageBox.ShowDialog();

            return cfrmMessageBox.m_eReturnStatus;
        }

        private void ckbxNoiseFreqChartIncludeMaxValue_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bOnFormLoad == false)
                return;

            string sErrorMessage = "";
            string sStepFolderPath = string.Format(@"{0}\{1}({2})", m_sDataFolderPath, SpecificText.m_sResultText, StringConvert.m_dictSubStepCNMappingTable[SubTuningStep.NO]);

            SetFrequencyPictureBox(ref sErrorMessage, sStepFolderPath, SubTuningStep.NO, ckbxNoiseFreqChartIncludeMaxValue.Checked);

            if (sErrorMessage != "")
            {
                ShowMessageBox(sErrorMessage, frmMessageBox.m_sError);
            }
        }

        private void ckbxTNFrequencyLineIncludeMaxValue_CheckedChanged(object sender, EventArgs e)
        {
            ckbxTNCompositeLineIncludeMaxValue.Checked = ckbxTNFrequencyLineIncludeMaxValue.Checked;

            if (m_bOnFormLoad == false)
                return;

            string sErrorMessage = "";

            SubTuningStep[] eSubStep_Array = new SubTuningStep[]
            {
                SubTuningStep.TILTNO_PTHF,
                SubTuningStep.TILTNO_BHF
            };

            foreach (SubTuningStep eSubStep in eSubStep_Array)
            {
                string sStepFolderPath = string.Format(@"{0}\{1}({2})", m_sDataFolderPath, SpecificText.m_sResultText, StringConvert.m_dictSubStepCNMappingTable[eSubStep]);

                SetFrequencyPictureBox(ref sErrorMessage, sStepFolderPath, eSubStep, ckbxTNCompositeLineIncludeMaxValue.Checked);

                if (sErrorMessage != "")
                {
                    ShowMessageBox(sErrorMessage, frmMessageBox.m_sError);
                    break;
                }
            }
        }

        private void ckbxTNCompositeLineIncludeMaxValue_CheckedChanged(object sender, EventArgs e)
        {
            ckbxTNFrequencyLineIncludeMaxValue.Checked = ckbxTNCompositeLineIncludeMaxValue.Checked;

            if (m_bOnFormLoad == false)
                return;

            string sErrorMessage = "";

            SubTuningStep[] eSubStep_Array = new SubTuningStep[]
            {
                SubTuningStep.TILTNO_PTHF,
                SubTuningStep.TILTNO_BHF
            };

            foreach (SubTuningStep eSubStep in eSubStep_Array)
            {
                string sStepFolderPath = string.Format(@"{0}\{1}({2})", m_sDataFolderPath, SpecificText.m_sResultText, StringConvert.m_dictSubStepCNMappingTable[eSubStep]);

                SetFrequencyPictureBox(ref sErrorMessage, sStepFolderPath, eSubStep, ckbxTNFrequencyLineIncludeMaxValue.Checked);

                if (sErrorMessage != "")
                {
                    ShowMessageBox(sErrorMessage, frmMessageBox.m_sError);
                    break;
                }
            }
        }
    }

    public class PictureBoxInfo_DigitalTuning
    {
        public List<string> m_sDataName_List = null;
        public ComboBox m_cbxDataName;
        public PictureBox m_picbxRXChart;
        public PictureBox m_picbxTXChart;

        public PictureBoxInfo_DigitalTuning(List<string> sDataName_List, ComboBox cbxDataName, PictureBox picbxRXChart, PictureBox picbxTXChart)
        {
            m_sDataName_List = new List<string>();
            m_sDataName_List.AddRange(sDataName_List);
            m_cbxDataName = cbxDataName;
            m_picbxRXChart = picbxRXChart;
            m_picbxTXChart = picbxTXChart;
        }
    }
}
