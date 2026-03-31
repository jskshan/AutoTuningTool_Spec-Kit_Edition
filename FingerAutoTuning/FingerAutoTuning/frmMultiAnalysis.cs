using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    public partial class frmMultiAnalysis : Form
    {
        private frmMain m_cfrmMain = null;

        public const int m_nCOMPARE_FREQUENCY = 0;
        public const int m_nCOMPARE_SCORE = 1;
        public static int m_nCompareOperator = m_nCOMPARE_SCORE;

        private const int m_nSpcMainDistance = 300;
        private const int m_nSpcMinor1Distance = 90;

        private int m_nCurrentStepSelectIndex = -1;
        private string m_sAnalysisErrorMessage = "";
        private string m_sReportFormat = "";

        private string m_sStartTime = "";
        public static string m_sMultiAnalysisDirectory = string.Format(@"{0}\{1}\MultiAnalysis", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
        //private string m_sReportFile = "";
        private string m_sReportFilePath = "";
        private string m_sChartFile = "";

        private int m_nToolTipIndex;
        private int m_nToolIndex = -1;

        private ctrlMADataGridView m_ctrlMADgvDisplay;
        private ctrlMAChart m_ctrlMAChartDisplay;

        public enum StepEnum : int
        { 
            FrequencyRank_Phase2 = 0,
            AC_FrequencyRank = 1
        }

        public enum PanelEnum : int
        {
            DgvRank = 0,
            Chart = 1
        }

        private StepEnum m_eCurrentStep;

        public class FileDataInfo
        {
            public int m_nFileNo = 0;
            public List<FrequencyValue> m_cFrequencyValue_List = new List<FrequencyValue>();
        }

        public class FrequencyValue
        {
            public double m_dFrequency = 0.0;
            public int m_nPH1PH2Sum = 0;
            public int m_nMinPH1 = 0;
            public int m_nMinPH2 = 0;
            public int m_nSuggestSUM = 0;
            public double m_dScore = 0.0;

            public double m_dLCMScore = 0.0;
            public double m_dACScore = 0.0;
        }

        public class AnalysisInfo
        {
            public List<FrequencyAnalysisValue> m_cFrequencyAnalysisValue_List = new List<FrequencyAnalysisValue>();
        }

        public AnalysisInfo cAnalysisInfo = null;

        public class FrequencyAnalysisValue
        {
            public double m_dFrequency = 0.0;
            public int m_nPH1PH2Sum = 0;
            public int m_nMinPH1 = 0;
            public int m_nMinPH2 = 0;
            public int m_nSuggestSUM = 0;
            public double m_dScore = 0.0;
            public int m_nDataCount = 0;

            public double m_dLCMScore = 0.0;
            public double m_dACScore = 0.0;
        }

        public class SortDataComparer : IComparer<FrequencyAnalysisValue>
        {
            public int Compare(FrequencyAnalysisValue cFrequencyAnalysisValue_1, FrequencyAnalysisValue cFrequencyAnalysisValue_2)
            {
                if (m_nCompareOperator == m_nCOMPARE_FREQUENCY)
                    return cFrequencyAnalysisValue_1.m_dFrequency.CompareTo(cFrequencyAnalysisValue_2.m_dFrequency);
                else if (m_nCompareOperator == m_nCOMPARE_SCORE)
                {
                    if (-cFrequencyAnalysisValue_1.m_dScore.CompareTo(cFrequencyAnalysisValue_2.m_dScore) != 0)
                        return -cFrequencyAnalysisValue_1.m_dScore.CompareTo(cFrequencyAnalysisValue_2.m_dScore);
                    else if (-cFrequencyAnalysisValue_1.m_dFrequency.CompareTo(cFrequencyAnalysisValue_2.m_dFrequency) != 0)
                        return -cFrequencyAnalysisValue_1.m_dFrequency.CompareTo(cFrequencyAnalysisValue_2.m_dFrequency);
                    else
                        return 1;
                }
                else
                    return cFrequencyAnalysisValue_1.m_dFrequency.CompareTo(cFrequencyAnalysisValue_2.m_dFrequency);
            }
        }

        public class DataColumnIndex
        {
            public bool m_bGetIndex = false;
            public int m_nFrequencyIndex = -1;
            public int m_nPH1PH2SumIndex = -1;
            public int m_nMinPH1Index = -1;
            public int m_nMinPH2Index = -1;
            public int m_nSuggestSUMIndex = -1;
            public int m_nScoreIndex = -1;

            public int m_nLCMScoreIndex = -1;
            public int m_nACScoreIndex = -1;
        }

        public DataColumnIndex m_cDataColumnIndex = new DataColumnIndex();

        public frmMultiAnalysis(frmMain cfrmMain)
        {
            InitializeComponent();

            m_cfrmMain = cfrmMain;
        }

        private void frmMultiAnalysis_Load(object sender, EventArgs e)
        {
            string[] sStepName_Array = new string[] 
            { 
                "FrequencyRank Phase2", 
                "AC FrequencyRank" 
            };
            SetComboBoxItem(sStepName_Array, cbxStep);
            m_nCurrentStepSelectIndex = 0;
            SetReportFormat();

            string[] sDataName_Array = new string[] 
            { 
                "Rank Table", 
                "Chart" 
            };
            SetComboBoxItem(sDataName_Array, cbxData);

            clbxFile.HorizontalScrollbar = true;
            btnStart.Enabled = false;

            clbxFile_Resize(sender, e);
            //clbxFile.MouseMove += new System.Windows.Forms.MouseEventHandler(ShowCheckBoxToolTip);
            clbxFile.MouseHover += new EventHandler(clbxFile_MouseHover);
            clbxFile.MouseMove += new MouseEventHandler(clbxFile_MouseMove);

            ResizeSplitContainer(splitContainerMain);
            ResizeSplitContainer(splitContainerMinor1);
        }

        private void SetComboBoxItem(string[] sItemName_Array, ComboBox ctrlComboBox)
        {
            foreach (string sItemName in sItemName_Array)
                ctrlComboBox.Items.Add(sItemName);

            ctrlComboBox.SelectedIndex = 0;
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Application.StartupPath;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Title = "Please Select Report.csv Files";
            openFileDialog.DefaultExt = "csv";
            openFileDialog.Filter = "csv files (*.csv)|*.csv";
            openFileDialog.FilterIndex = 1;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = true;
            openFileDialog.ReadOnlyChecked = true;
            openFileDialog.ShowReadOnly = true;

            DialogResult dr = openFileDialog.ShowDialog();

            if (dr != System.Windows.Forms.DialogResult.OK)
                return;

            List<string> sFormatErrorFile_List = new List<string>();

            foreach (String sFilePath in openFileDialog.FileNames)
            {
                bool bMatchFlag = false;
                string sLine = "";

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        if (sLine == m_sReportFormat)
                        {
                            bMatchFlag = true;
                            break;
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }

                if (bMatchFlag == true)
                {
                    bool bExistFlag = false;

                    for (int nIndex = 0; nIndex < clbxFile.Items.Count; nIndex++)
                    {
                        string sExistFilePath = clbxFile.Items[nIndex].ToString();

                        if (sFilePath.Equals(sExistFilePath))
                        {
                            bExistFlag = true;
                            break;
                        }
                    }

                    if (bExistFlag == false)
                    {
                        clbxFile.Items.Add(sFilePath);
                        //RedsizeFileCheckedListBoxItemText();
                        btnStart.Enabled = true;
                    }
                }
                else
                    sFormatErrorFile_List.Add(sFilePath);
            }

            if (sFormatErrorFile_List.Count > 0)
            {
                string sFormatErrorMessage = "Format Error : ";

                foreach (string sFormatErrorFile in sFormatErrorFile_List)
                    sFormatErrorMessage += Environment.NewLine + string.Format("File : {0}", sFormatErrorFile);

                MessageBox.Show(sFormatErrorMessage);
                return;
            }
        }

        private void cbxStep_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxStep.SelectedIndex != m_nCurrentStepSelectIndex)
            {
                if (clbxFile.Items.Count > 0)
                {
                    clbxFile.Items.Clear();
                    btnStart.Enabled = true;
                }

                m_nCurrentStepSelectIndex = cbxStep.SelectedIndex;
                SetReportFormat();
            }
        }

        private void SetReportFormat()
        {
            string[] sColumnHeader_Array = null;
            m_sReportFormat = "";

            if (cbxStep.SelectedIndex == 0)
            {
                sColumnHeader_Array = new string[] 
                { 
                    "Rank", 
                    "Frequency(KHz)", 
                    "PH1+PH2", 
                    "Minimum PH1", 
                    "Minimum PH2", 
                    "DFT_NUM", 
                    "Suggest DFT_NUM",
                    "Signal RefValue", 
                    "Noise Mean", 
                    "Noise Std", 
                    "Noise PosRef", 
                    "Noise NegRef",
                    "Noise RefValue", 
                    "RawSNR RefValue", 
                    "Reference Value" 
                };
            }
            else if (cbxStep.SelectedIndex == 1)
            {
                if (ParamFingerAutoTuning.m_nACFRModeType == 1)
                {
                    sColumnHeader_Array = new string[] 
                    { 
                        "Rank", 
                        "Frequency(KHz)", 
                        "PH1+PH2", 
                        "Minimum PH1", 
                        "Minimum PH2", 
                        "DFT_NUM", 
                        "SuggestDFT_NUM",
                        "Signal RefValue", 
                        "AC Noise Mean", 
                        "AC Noise Std", 
                        "AC Noise PosRef",
                        "AC Noise NegRef", 
                        "AC RawSNR", 
                        "AC SNR(dB)",
                        "LCM Noise Mean", 
                        "LCM Noise Std", 
                        "LCM Noise PosRef", 
                        "LCM Noise NegRef", 
                        "LCM RawSNR", 
                        "LCM SNR(dB)", 
                        "Composite RawSNR", 
                        "Composite SNR(dB)" 
                    };
                }
                else
                {
                    sColumnHeader_Array = new string[] 
                    { 
                        "Rank", 
                        "Frequency(KHz)", 
                        "PH1+PH2", 
                        "Minimum PH1", 
                        "Minimum PH2", 
                        "Signal RefValue", 
                        "AC Noise Mean", 
                        "AC Noise Std", 
                        "AC Noise PosRef", 
                        "AC Noise NegRef", 
                        "AC RawSNR", 
                        "AC SNR(dB)",
                        "LCM Noise Mean", 
                        "LCM Noise Std", 
                        "LCM Noise PosRef", 
                        "LCM Noise NegRef", 
                        "LCM RawSNR", 
                        "LCM SNR(dB)", 
                        "Composite RawSNR", 
                        "Composite SNR(dB)" 
                    };
                }
            }

            for (int nIndex = 0; nIndex < sColumnHeader_Array.Length; nIndex++)
            {
                if (nIndex < sColumnHeader_Array.Length - 1)
                    m_sReportFormat += string.Format("{0},", sColumnHeader_Array[nIndex]);
                else
                    m_sReportFormat += sColumnHeader_Array[nIndex];
            }
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            for (int nIndex = clbxFile.Items.Count - 1; nIndex >= 0; nIndex--)
            {
                if (clbxFile.GetItemChecked(nIndex) == true)
                    clbxFile.Items.RemoveAt(nIndex);
            }

            if (clbxFile.Items.Count == 0)
                btnStart.Enabled = false;
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int nIndex = clbxFile.Items.Count - 1; nIndex >= 0; nIndex--)
                clbxFile.SetItemChecked(nIndex, true);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ControllerOnOffOption(false);

            if (cbxStep.SelectedIndex == 0)
                m_eCurrentStep = StepEnum.FrequencyRank_Phase2;
            else if (cbxStep.SelectedIndex == 1)
                m_eCurrentStep = StepEnum.AC_FrequencyRank;

            m_sStartTime = System.DateTime.Now.ToString("MMdd-HHmmss");
            int nFileNumber = 0;
            List<FileDataInfo> cFileDataInfo_List = new List<FileDataInfo>();

            for (int nIndex = 0; nIndex < clbxFile.Items.Count; nIndex++)
            {
                string sFilePath = clbxFile.Items[nIndex].ToString();

                if (File.Exists(sFilePath) == false)
                    continue;

                FileDataInfo cFileDataInfo = new FileDataInfo();

                string sLine;
                bool bGetColumnTitle = false;

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        if (sLine == m_sReportFormat)
                        {
                            GetDataColumnIndex(sLine);
                            bGetColumnTitle = true;
                            continue;
                        }

                        if (bGetColumnTitle == true)
                        {
                            if (sLine == "")
                                break;

                            FrequencyValue cFrequencyValue = new FrequencyValue();
                            cFrequencyValue = GetFileDataInfo(sLine);
                            cFileDataInfo.m_cFrequencyValue_List.Add(cFrequencyValue);
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }

                nFileNumber++;
                cFileDataInfo.m_nFileNo = nFileNumber;
                cFileDataInfo_List.Add(cFileDataInfo);
            }

            cAnalysisInfo = new AnalysisInfo();

            ComputeAnalysisData(cFileDataInfo_List);

            if (SelectFilePath() == false)
            {
                MessageBox.Show(m_sAnalysisErrorMessage);
                ControllerOnOffOption(true);
                return;
            }

            /*
            if (SaveChartFile() == false)
            {
                MessageBox.Show(m_sAnalysisErrorMessage);
                ControllerOnOffOption(true);
                return;
            }
            */

            PutUserControllerInPanel();

            ControllerOnOffOption(true);
        }

        private void GetDataColumnIndex(string sLine)
        {
            if (m_cDataColumnIndex.m_bGetIndex == false)
            {
                string[] sSplit_Array = sLine.Split(',');

                for (int nIndex = 0; nIndex < sSplit_Array.Length; nIndex++)
                {
                    switch (sSplit_Array[nIndex])
                    {
                        case "Frequency(KHz)":
                            m_cDataColumnIndex.m_nFrequencyIndex = nIndex;
                            break;
                        case "PH1+PH2":
                            m_cDataColumnIndex.m_nPH1PH2SumIndex = nIndex;
                            break;
                        case "Minimum PH1":
                            m_cDataColumnIndex.m_nMinPH1Index = nIndex;
                            break;
                        case "Minimum PH2":
                            m_cDataColumnIndex.m_nMinPH2Index = nIndex;
                            break;
                        default:
                            break;
                    }

                    if (m_eCurrentStep == StepEnum.FrequencyRank_Phase2)
                    {
                        switch (sSplit_Array[nIndex])
                        {
                            case "Suggest SUM":
                                m_cDataColumnIndex.m_nSuggestSUMIndex = nIndex;
                                break;
                            case "Reference Value":
                                m_cDataColumnIndex.m_nScoreIndex = nIndex;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (m_eCurrentStep == StepEnum.AC_FrequencyRank)
                    {
                        switch (sSplit_Array[nIndex])
                        {
                            case "SuggestSum":
                                m_cDataColumnIndex.m_nSuggestSUMIndex = nIndex;
                                break;
                            case "Composite SNR(dB)":
                                m_cDataColumnIndex.m_nScoreIndex = nIndex;
                                break;
                            case "AC SNR(dB)":
                                m_cDataColumnIndex.m_nACScoreIndex = nIndex;
                                break;
                            case "LCM SNR(dB)":
                                m_cDataColumnIndex.m_nLCMScoreIndex = nIndex;
                                break;
                        }
                        
                    }
                }
            }
        }

        private FrequencyValue GetFileDataInfo(string sLine)
        {
            List<int> nColumnIndex_List = new List<int>{ m_cDataColumnIndex.m_nFrequencyIndex, 
                                                         m_cDataColumnIndex.m_nPH1PH2SumIndex, 
                                                         m_cDataColumnIndex.m_nMinPH1Index, 
                                                         m_cDataColumnIndex.m_nMinPH2Index, 
                                                         m_cDataColumnIndex.m_nSuggestSUMIndex, 
                                                         m_cDataColumnIndex.m_nScoreIndex };

            if (m_eCurrentStep == StepEnum.AC_FrequencyRank)
            {
                nColumnIndex_List.Add(m_cDataColumnIndex.m_nACScoreIndex);
                nColumnIndex_List.Add(m_cDataColumnIndex.m_nLCMScoreIndex);
            }

            FrequencyValue cFrequencyValue = new FrequencyValue();

            string[] sSplit_Array = sLine.Split(',');

            for (int nSplitIndex = 0; nSplitIndex < sSplit_Array.Length; nSplitIndex++)
            {
                for (int nColumnIndex = 0; nColumnIndex < nColumnIndex_List.Count; nColumnIndex++)
                {
                    if (nSplitIndex == nColumnIndex_List[nColumnIndex])
                    {
                        switch (nColumnIndex)
                        {
                            case 0:
                                double dFrequency = 0.0;
                                Double.TryParse(sSplit_Array[nSplitIndex], out dFrequency);
                                cFrequencyValue.m_dFrequency = dFrequency;
                                break;
                            case 1:
                                cFrequencyValue.m_nPH1PH2Sum = Convert.ToInt32(sSplit_Array[nSplitIndex], 16);
                                break;
                            case 2:
                                cFrequencyValue.m_nMinPH1 = Convert.ToInt32(sSplit_Array[nSplitIndex], 16);
                                break;
                            case 3:
                                cFrequencyValue.m_nMinPH2 = Convert.ToInt32(sSplit_Array[nSplitIndex], 16);
                                break;
                            case 4:
                                int nSuggestSUM = 0;
                                Int32.TryParse(sSplit_Array[nSplitIndex], out nSuggestSUM);
                                cFrequencyValue.m_nSuggestSUM = nSuggestSUM;
                                break;
                            case 5:
                                double dScore = 0.0;
                                Double.TryParse(sSplit_Array[nSplitIndex], out dScore);
                                cFrequencyValue.m_dScore = dScore;
                                break;
                            case 6:
                                Double.TryParse(sSplit_Array[nSplitIndex], out dScore);
                                cFrequencyValue.m_dACScore = dScore;
                                break;
                            case 7:
                                Double.TryParse(sSplit_Array[nSplitIndex], out dScore);
                                cFrequencyValue.m_dLCMScore = dScore;
                                break;
                        }

                        break;
                    }
                }
            }

            return cFrequencyValue;
        }

        private void ComputeAnalysisData(List<FileDataInfo> cFileDataInfo_List)
        {
            foreach (FileDataInfo cFileDataInfo in cFileDataInfo_List)
            {
                List<FrequencyValue> cFrequencyValue_List = cFileDataInfo.m_cFrequencyValue_List;

                for (int nIndex = 0; nIndex < cFrequencyValue_List.Count; nIndex++)
                {
                    int nPH1PH2Sum = cFrequencyValue_List[nIndex].m_nPH1PH2Sum;

                    if (cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count > 0)
                    {
                        int nListIndex = cAnalysisInfo.m_cFrequencyAnalysisValue_List.FindIndex(x => x.m_nPH1PH2Sum == nPH1PH2Sum);

                        if (nListIndex >= 0)
                        {
                            FrequencyAnalysisValue cFrequencyAnalysisValue = cAnalysisInfo.m_cFrequencyAnalysisValue_List[nListIndex];

                            if (cFrequencyValue_List[nIndex].m_nMinPH1 > cFrequencyAnalysisValue.m_nMinPH1)
                                cFrequencyAnalysisValue.m_nMinPH1 = cFrequencyValue_List[nIndex].m_nMinPH1;

                            if (cFrequencyValue_List[nIndex].m_nMinPH2 > cFrequencyAnalysisValue.m_nMinPH2)
                                cFrequencyAnalysisValue.m_nMinPH2 = cFrequencyValue_List[nIndex].m_nMinPH2;

                            if (cFrequencyValue_List[nIndex].m_nSuggestSUM < cFrequencyAnalysisValue.m_nSuggestSUM)
                                cFrequencyAnalysisValue.m_nSuggestSUM = cFrequencyValue_List[nIndex].m_nSuggestSUM;

                            cFrequencyAnalysisValue.m_dScore += cFrequencyValue_List[nIndex].m_dScore;

                            if (m_eCurrentStep == StepEnum.AC_FrequencyRank)
                            {
                                cFrequencyAnalysisValue.m_dACScore += cFrequencyValue_List[nIndex].m_dACScore;
                                cFrequencyAnalysisValue.m_dLCMScore += cFrequencyValue_List[nIndex].m_dLCMScore;
                            }

                            cFrequencyAnalysisValue.m_nDataCount++;
                        }
                        else
                        {
                            FrequencyAnalysisValue cFrequencyAnalysisValue = new FrequencyAnalysisValue();

                            cFrequencyAnalysisValue.m_dFrequency = cFrequencyValue_List[nIndex].m_dFrequency;
                            cFrequencyAnalysisValue.m_nPH1PH2Sum = cFrequencyValue_List[nIndex].m_nPH1PH2Sum;
                            cFrequencyAnalysisValue.m_nMinPH1 = cFrequencyValue_List[nIndex].m_nMinPH1;
                            cFrequencyAnalysisValue.m_nMinPH2 = cFrequencyValue_List[nIndex].m_nMinPH2;
                            cFrequencyAnalysisValue.m_nSuggestSUM = cFrequencyValue_List[nIndex].m_nSuggestSUM;
                            cFrequencyAnalysisValue.m_dScore += cFrequencyValue_List[nIndex].m_dScore;

                            if (m_eCurrentStep == StepEnum.AC_FrequencyRank)
                            {
                                cFrequencyAnalysisValue.m_dACScore += cFrequencyValue_List[nIndex].m_dACScore;
                                cFrequencyAnalysisValue.m_dLCMScore += cFrequencyValue_List[nIndex].m_dLCMScore;
                            }

                            cFrequencyAnalysisValue.m_nDataCount++;

                            cAnalysisInfo.m_cFrequencyAnalysisValue_List.Add(cFrequencyAnalysisValue);
                        }
                    }
                    else
                    {
                        FrequencyAnalysisValue cFrequencyAnalysisValue = new FrequencyAnalysisValue();

                        cFrequencyAnalysisValue.m_dFrequency = cFrequencyValue_List[nIndex].m_dFrequency;
                        cFrequencyAnalysisValue.m_nPH1PH2Sum = cFrequencyValue_List[nIndex].m_nPH1PH2Sum;
                        cFrequencyAnalysisValue.m_nMinPH1 = cFrequencyValue_List[nIndex].m_nMinPH1;
                        cFrequencyAnalysisValue.m_nMinPH2 = cFrequencyValue_List[nIndex].m_nMinPH2;
                        cFrequencyAnalysisValue.m_nSuggestSUM = cFrequencyValue_List[nIndex].m_nSuggestSUM;
                        cFrequencyAnalysisValue.m_dScore += cFrequencyValue_List[nIndex].m_dScore;

                        if (m_eCurrentStep == StepEnum.AC_FrequencyRank)
                        {
                            cFrequencyAnalysisValue.m_dACScore += cFrequencyValue_List[nIndex].m_dACScore;
                            cFrequencyAnalysisValue.m_dLCMScore += cFrequencyValue_List[nIndex].m_dLCMScore;
                        }

                        cFrequencyAnalysisValue.m_nDataCount++;

                        cAnalysisInfo.m_cFrequencyAnalysisValue_List.Add(cFrequencyAnalysisValue);
                    }
                }
            }

            List<int> nRemoveIndex_List = new List<int>();

            for (int nIndex = 0; nIndex < cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count; nIndex++)
            {
                FrequencyAnalysisValue cFrequencyAnalysisValue = cAnalysisInfo.m_cFrequencyAnalysisValue_List[nIndex];

                if (cFrequencyAnalysisValue.m_nDataCount < cFileDataInfo_List.Count)
                {
                    nRemoveIndex_List.Add(nIndex);
                    continue;
                }

                cFrequencyAnalysisValue.m_dScore = Math.Round(cFrequencyAnalysisValue.m_dScore / cFrequencyAnalysisValue.m_nDataCount, 3, MidpointRounding.AwayFromZero);

                if (m_eCurrentStep == StepEnum.AC_FrequencyRank)
                {
                    cFrequencyAnalysisValue.m_dACScore = Math.Round(cFrequencyAnalysisValue.m_dACScore / cFrequencyAnalysisValue.m_nDataCount, 3, MidpointRounding.AwayFromZero);
                    cFrequencyAnalysisValue.m_dLCMScore = Math.Round(cFrequencyAnalysisValue.m_dLCMScore / cFrequencyAnalysisValue.m_nDataCount, 3, MidpointRounding.AwayFromZero);
                }
            }

            for (int nIndex = cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count - 1; nIndex >= 0; nIndex--)
            {
                for (int nRemoveIndex = 0; nRemoveIndex < nRemoveIndex_List.Count; nRemoveIndex++)
                {
                    if (nIndex == nRemoveIndex_List[nRemoveIndex])
                    {
                        cAnalysisInfo.m_cFrequencyAnalysisValue_List.RemoveAt(nIndex);
                        break;
                    }
                }
            }

            if (m_eCurrentStep == StepEnum.FrequencyRank_Phase2)
            {
                double dMaxValue = 0.0;

                foreach (FrequencyAnalysisValue cFrequencyAnalysisValue in cAnalysisInfo.m_cFrequencyAnalysisValue_List)
                {
                    if (cFrequencyAnalysisValue.m_dScore > dMaxValue)
                        dMaxValue = cFrequencyAnalysisValue.m_dScore;
                }

                foreach (FrequencyAnalysisValue cFrequencyAnalysisValue in cAnalysisInfo.m_cFrequencyAnalysisValue_List)
                    cFrequencyAnalysisValue.m_dScore = Math.Round((cFrequencyAnalysisValue.m_dScore / dMaxValue) * 100, 3, MidpointRounding.AwayFromZero);
            }
        }

        private bool SelectFilePath()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "csv File|*.csv";
            saveFileDialog.Title = "Save a CSV File";
            saveFileDialog.InitialDirectory = Application.StartupPath;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FilterIndex = 1;
            //saveFileDialog.CheckFileExists = true;
            saveFileDialog.CheckPathExists = true;

            DialogResult dr = saveFileDialog.ShowDialog();

            if (dr != System.Windows.Forms.DialogResult.OK)
            {
                m_sAnalysisErrorMessage = "Select File Path Error";
                return false;
            }

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog.FileName != "")
            {
                m_sReportFilePath = saveFileDialog.FileName;

                if (SaveReportFile(saveFileDialog.FileName) == false)
                    return false;
            }
            else
            {
                m_sAnalysisErrorMessage = "Select File Path Error";
                return false;
            }

            return true;
        }

        private bool SaveReportFile(string sFilePath)
        {
            bool bErrorFlag = false;
            int nRankIndex = 1;

            string[] sColumnHeader_Array = new string[] 
            { 
                "Rank", 
                "Frequency(KHz)", 
                "PH1+PH2", 
                "Minimum PH1", 
                "Minimum PH2", 
                "Suggest SUM", 
                "Reference Value" 
            };

            if (m_eCurrentStep == StepEnum.AC_FrequencyRank)
            {
                sColumnHeader_Array = new string[] 
                { 
                    "Rank", 
                    "Frequency(KHz)", 
                    "PH1+PH2", 
                    "Minimum PH1",
                    "Minimum PH2", 
                    "Suggest SUM", 
                    "AC SNR(dB)", 
                    "LCM SNR(dB)", 
                    "Composite SNR(dB)" 
                };
            }

            /*
            if (Directory.Exists(m_sMultiAnalysisDirectory) == false)
                Directory.CreateDirectory(m_sMultiAnalysisDirectory);

            m_sReportFile = string.Format("MAReport_{0}.csv", m_sStartTime);
            string sFilePath = string.Format(@"{0}\{1}", m_sMultiAnalysisDirectory, m_sReportFile);
            */

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            
            try
            {
                Write_Tool_Information(sw);

                sw.WriteLine();

                m_nCompareOperator = m_nCOMPARE_SCORE;
                cAnalysisInfo.m_cFrequencyAnalysisValue_List.Sort(new SortDataComparer());

                sw.WriteLine("Frequency Rank:");

                for (int nIndex = 0; nIndex < sColumnHeader_Array.Length; nIndex++)
                {
                    if (nIndex != sColumnHeader_Array.Length - 1)
                        sw.Write(string.Format("{0},", sColumnHeader_Array[nIndex]));
                    else
                        sw.WriteLine(sColumnHeader_Array[nIndex]);
                }

                foreach (FrequencyAnalysisValue cFrequencyAnalysisValue in cAnalysisInfo.m_cFrequencyAnalysisValue_List)
                {
                    sw.Write(string.Format("{0},", nRankIndex.ToString()));
                    sw.Write(string.Format("{0},", cFrequencyAnalysisValue.m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("0x{0},", cFrequencyAnalysisValue.m_nPH1PH2Sum.ToString("x2").ToUpper()));
                    sw.Write(string.Format("0x{0},", cFrequencyAnalysisValue.m_nMinPH1.ToString("x2").ToUpper()));
                    sw.Write(string.Format("0x{0},", cFrequencyAnalysisValue.m_nMinPH2.ToString("x2").ToUpper()));
                    sw.Write(string.Format("{0},", cFrequencyAnalysisValue.m_nSuggestSUM.ToString()));

                    if (m_eCurrentStep == StepEnum.AC_FrequencyRank)
                    {
                        sw.Write(string.Format("{0},", cFrequencyAnalysisValue.m_dACScore.ToString("0.000")));
                        sw.Write(string.Format("{0},", cFrequencyAnalysisValue.m_dLCMScore.ToString("0.000")));
                    }

                    sw.WriteLine(string.Format("{0}", cFrequencyAnalysisValue.m_dScore.ToString("0.000")));

                    nRankIndex++;
                }

                sw.WriteLine("Frequency Rank:");
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sAnalysisErrorMessage = "Save Report Data Error";
                bErrorFlag = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bErrorFlag == true)
                return false;
            else
                return true;
        }

        private bool SaveChartFile()
        {
            bool bErrorFlag = false;
            m_sChartFile = string.Format("Chart_{0}.jpg", m_sStartTime);
            string sFilePath = string.Format(@"{0}\{1}", m_sMultiAnalysisDirectory, m_sChartFile);

            string sValueName = "";

            if (m_eCurrentStep == StepEnum.FrequencyRank_Phase2)
                sValueName = "ReferenceValue";
            else if (m_eCurrentStep == StepEnum.AC_FrequencyRank)
                sValueName = "SNR(dB)";

            string sTitleName = string.Format("{0} Distribution By Frequency", sValueName);

            double dBestRankLB = 0.0;

            m_nCompareOperator = m_nCOMPARE_SCORE;
            cAnalysisInfo.m_cFrequencyAnalysisValue_List.Sort(new SortDataComparer());

            if (cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count < ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber)
                dBestRankLB = cAnalysisInfo.m_cFrequencyAnalysisValue_List[cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count - 1].m_dScore;
            else
                dBestRankLB = cAnalysisInfo.m_cFrequencyAnalysisValue_List[9].m_dScore;

            m_nCompareOperator = m_nCOMPARE_FREQUENCY;
            cAnalysisInfo.m_cFrequencyAnalysisValue_List.Sort(new SortDataComparer());

            int nInterval = 5;
            double dMaxFrequency = 0.0;
            double dMinFrequency = 0.0;

            for (int nIndex = 0; nIndex < cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count; nIndex++)
            {
                FrequencyAnalysisValue cFrequencyAnalysisValue = cAnalysisInfo.m_cFrequencyAnalysisValue_List[nIndex];

                if (nIndex == 0)
                {
                    dMaxFrequency = cFrequencyAnalysisValue.m_dFrequency;
                    dMinFrequency = cFrequencyAnalysisValue.m_dFrequency;
                }
                else
                {
                    if (cFrequencyAnalysisValue.m_dFrequency > dMaxFrequency)
                        dMaxFrequency = cFrequencyAnalysisValue.m_dFrequency;

                    if (cFrequencyAnalysisValue.m_dFrequency < dMinFrequency)
                        dMinFrequency = cFrequencyAnalysisValue.m_dFrequency;
                }
            }

            int nMinFrequency = (int)(dMinFrequency / nInterval) * nInterval;

            double dDiffer = dMaxFrequency - (double)nMinFrequency;
            int nPart = (int)(dDiffer / nInterval);

            if (dDiffer % nInterval != 0)
                nPart++;

            int nMaxFrequency = nMinFrequency + nPart * nInterval;

            Series cSeries = new Series(string.Format("{0} vs. Frequency", sValueName));

            //Show Line Chart
            Chart cChart = new Chart();
            var varChartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            cChart.ChartAreas.Add(varChartArea);
            cChart.Width = 1500;
            cChart.Height = 500;
            cChart.Legends.Add("Legend");
            cChart.Legends["Legend"].Font = new Font("Times New Roman", 10);
            cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
            cChart.Titles.Add(sTitleName);
            cChart.Titles[0].Font = new Font("Times New Roman", 18);
            cChart.ChartAreas[0].AxisY.Title = sValueName;
            cChart.ChartAreas[0].AxisY.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.ChartAreas[0].AxisX.Title = "Frequency(KHz)";
            cChart.ChartAreas[0].AxisX.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Times New Roman", 10);
            //cChart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //cChart.ChartAreas[0].AxisX.IsLabelAutoFit = false;
            //cChart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;
            cChart.ChartAreas[0].AxisX.Minimum = nMinFrequency;
            cChart.ChartAreas[0].AxisX.Maximum = nMaxFrequency;
            cChart.ChartAreas[0].AxisX.Interval = nInterval;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            cSeries.ChartType = SeriesChartType.Line;
            cSeries.MarkerStyle = MarkerStyle.Circle;
            cSeries.MarkerSize = 5;
            cSeries.IsValueShownAsLabel = false;
            cSeries.Color = Color.Blue;

            for (int nIndex = 0; nIndex < cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count; nIndex++)
            {
                FrequencyAnalysisValue cFrequencyAnalysisValue = cAnalysisInfo.m_cFrequencyAnalysisValue_List[nIndex];
                var vLine = cSeries.Points[cSeries.Points.AddXY(cFrequencyAnalysisValue.m_dFrequency, cFrequencyAnalysisValue.m_dScore)];

                int nFindIndex = cAnalysisInfo.m_cFrequencyAnalysisValue_List.FindIndex(x => x.m_dFrequency == cFrequencyAnalysisValue.m_dFrequency);

                if (cFrequencyAnalysisValue.m_dScore >= dBestRankLB)
                {
                    vLine.MarkerColor = Color.Red;
                    vLine.Label = string.Format("{0}KHz", cFrequencyAnalysisValue.m_dFrequency);
                    vLine.IsValueShownAsLabel = true;
                }
                else
                    vLine.MarkerColor = Color.Blue;

                vLine.Color = Color.Blue;
            }

            cChart.Series.Add(cSeries);

            try
            {
                cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
            }
            catch
            {
                m_sAnalysisErrorMessage = "Save Chart File Error";
                bErrorFlag = true;
            }

            if (bErrorFlag == true)
                return false;
            else
                return true;
        }

        private void ControllerOnOffOption(bool bEnable)
        {
            ComboBox[] cbxExceptComboBox_Array = new ComboBox[] 
            { 
                cbxData 
            };

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is ComboBox)
                {
                    if (cbxExceptComboBox_Array.Contains(ctrl) == false)
                        ctrl.Enabled = bEnable;
                }
                else if (ctrl is Button)
                    ctrl.Enabled = bEnable;
                else if (ctrl is CheckedListBox)
                    ctrl.Enabled = bEnable;
            }
        }

        private void splitContainerMain_Resize(object sender, EventArgs e)
        {
            SplitContainer splitContainer = sender as SplitContainer;
            ResizeSplitContainer(splitContainer);
        }

        private void ResizeSplitContainer(SplitContainer splitContainer)
        {
            if (splitContainer == splitContainerMain)
            {
                splitContainer.SplitterDistance = m_nSpcMainDistance;

                /*
                SplitContainer splitContainer = sender as SplitContainer;
                int nSTotalWidth = splitContainer.Width - splitContainer.SplitterWidth;
                int nPanel1Width = (int)((double)nSTotalWidth * 0.25);
                
                if (nPanel1Width > 0)
                    splitContainer.SplitterDistance = nPanel1Width;
                */
            }
            else if (splitContainer == splitContainerMinor1)
            {
                int nSTotalHeight = splitContainer.Height - m_nSpcMinor1Distance;
                splitContainer.SplitterDistance = nSTotalHeight;

                Button[] btnButton_Array = new Button[] 
                { 
                    btnAddFile, 
                    btnSelectAll, 
                    btnRemoveFile 
                };

                int nButtonTotalWidth = 0;

                for (int nIndex = 0; nIndex < btnButton_Array.Length; nIndex++)
                    nButtonTotalWidth += btnButton_Array[nIndex].Width;

                int nSpaceDistance = (int)((splitContainer.Width - nButtonTotalWidth) / (btnButton_Array.Length + 1));

                for (int nIndex = 0; nIndex < btnButton_Array.Length; nIndex++)
                {
                    int nButtonLeft = nSpaceDistance * (nIndex + 1);

                    for (int mIndex = 0; mIndex < nIndex; mIndex++)
                        nButtonLeft += btnButton_Array[mIndex].Width;

                    btnButton_Array[nIndex].Left = nButtonLeft;
                }

                int nSplitContainerWidthCenter = (int)(splitContainer.Width / 2);
                int nStartButtonWidthCenter = (int)(btnStart.Width / 2);
                btnStart.Left = nSplitContainerWidthCenter - nStartButtonWidthCenter;

                /*
                SplitContainer splitContainer = sender as SplitContainer;
                int nSTotalHeight = splitContainer.Height - splitContainer.SplitterWidth;
                int nPanel1Height = (int)((double)nSTotalHeight * 0.85);
                
                if (nPanel1Height > 0)
                    splitContainer.SplitterDistance = nPanel1Height;
                */
            }
        }

        private void splitContainerMinor1_Resize(object sender, EventArgs e)
        {
            SplitContainer splitContainer = sender as SplitContainer;
            ResizeSplitContainer(splitContainer);
        }

        private void cbxData_SelectedIndexChanged(object sender, EventArgs e)
        {
            PutUserControllerInPanel();
        }

        private void PutUserControllerInPanel()
        {
            if (cAnalysisInfo == null)
                return;

            if (cbxData.SelectedIndex == 0)
            {
                //pnlData.Controls.Clear();
                SetControllerBringToFront(PanelEnum.DgvRank);

                //string sFilePath = string.Format(@"{0}\{1}", m_sMultiAnalysisDirectory, m_sReportFile);

                m_ctrlMADgvDisplay = new ctrlMADataGridView(m_eCurrentStep, m_sReportFilePath);

                m_ctrlMADgvDisplay.Left = 0;
                m_ctrlMADgvDisplay.Top = 0;
                m_ctrlMADgvDisplay.Width = pnlData.Width;
                m_ctrlMADgvDisplay.Height = pnlData.Height;
                m_ctrlMADgvDisplay.Visible = true;

                m_ctrlMADgvDisplay.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

                pnlData.Controls.Add(m_ctrlMADgvDisplay);

                SetControllerBringToFront(PanelEnum.DgvRank);
            }
            else if (cbxData.SelectedIndex == 1)
            {
                //pnlData.Controls.Clear();
                SetControllerBringToFront(PanelEnum.Chart);

                m_ctrlMAChartDisplay = new ctrlMAChart(m_eCurrentStep, cAnalysisInfo);

                m_ctrlMAChartDisplay.Left = 0;
                m_ctrlMAChartDisplay.Top = 0;
                m_ctrlMAChartDisplay.Width = pnlData.Width;
                m_ctrlMAChartDisplay.Height = pnlData.Height;
                m_ctrlMAChartDisplay.Visible = true;

                m_ctrlMAChartDisplay.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

                pnlData.Controls.Add(m_ctrlMAChartDisplay);

                SetControllerBringToFront(PanelEnum.Chart);
            }
        }

        private void SetControllerBringToFront(PanelEnum ePanelType)
        {
            foreach (Control ctrl in pnlData.Controls)
            {
                switch (ePanelType)
                {
                    case PanelEnum.DgvRank:
                        if (ctrl is ctrlMADataGridView)
                        {
                            ctrl.BringToFront();
                            return;
                        }

                        break;
                    case PanelEnum.Chart:
                        if (ctrl is ctrlMAChart)
                        {
                            ctrl.BringToFront();
                            return;
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        private void clbxFile_Resize(object sender, EventArgs e)
        {
            //ResizeFileCheckedListBoxItemText();
        }

        private void ShowCheckBoxToolTip(object sender, MouseEventArgs e)
        {
            if (m_nToolTipIndex != clbxFile.IndexFromPoint(e.Location))
            {
                m_nToolTipIndex = clbxFile.IndexFromPoint(clbxFile.PointToClient(MousePosition));

                if (m_nToolTipIndex > -1)
                {
                    toolTip1.SetToolTip(clbxFile, clbxFile.Items[m_nToolTipIndex].ToString());
                }
            }
        }

        private void clbxFile_MouseMove(object sender, MouseEventArgs e)
        {
            int nIndex = clbxFile.IndexFromPoint(e.Location);

            if (m_nToolIndex != nIndex)
                GetToolTip();
        }

        private void clbxFile_MouseHover(object sender, EventArgs e)
        {
            GetToolTip();

            /*
            Point pos = clbxFile.PointToClient(MousePosition);
            int nIndex = clbxFile.IndexFromPoint(pos);

            if (nIndex > -1)
            {
                pos = this.PointToClient(MousePosition);
                toolTip1.Show(clbxFile.Items[nIndex].ToString(), this, pos.X, pos.Y, 3000);
            }
            */
        }

        private void GetToolTip()
        {
            Point pos = clbxFile.PointToClient(MousePosition);
            m_nToolIndex = clbxFile.IndexFromPoint(pos);

            if (m_nToolIndex > -1)
            {
                pos = this.PointToClient(MousePosition);
                //toolTip1.ToolTipTitle = "ToolTip for CheckedListBox";
                toolTip1.SetToolTip(clbxFile, clbxFile.Items[m_nToolIndex].ToString());
            }
        }

        //private void ResizeFileCheckedListBoxItemText()
        //{
        //    int nWidth = clbxFile.Width;
        //
        //    for (int nIndex = 0; nIndex < clbxFile.Items.Count; nIndex++)
        //    {
        //        string sCompressionName = "";
        //        string sFilePath = clbxFile.Items[nIndex].ToString();
        //        Graphics g = this.CreateGraphics();
        //        Font font = new Font("Times New Roman", 12.0f);
        //        SizeF sf = g.MeasureString(sFilePath, font);

        //        if (sf.Width > nWidth - 120)
        //        {
        //            string sUpDirectoryName = Path.GetFileName(Path.GetDirectoryName(sFilePath));
        //            sCompressionName = string.Format(@"...\{0}\{1}", sUpDirectoryName, Path.GetFileName(sFilePath));
        //            clbxFile.Items[nIndex] = sCompressionName;
        //        }
        //    }
        //}

        private void Write_Tool_Information(StreamWriter sw)
        {
            string sTool_Name = "FingerAutoTuningTool";

            sw.WriteLine(string.Format("ToolName,{0}", sTool_Name));
            sw.WriteLine(string.Format("AutoTuningToolVersion,{0}", m_cfrmMain.m_sParentAPVersion));
            sw.WriteLine(string.Format("FingerAutoTuningVersion,{0}", m_cfrmMain.m_sAPVersion));
            sw.WriteLine("=====================================================");
        }
    }
}
