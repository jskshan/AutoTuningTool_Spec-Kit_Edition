using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
//using Microsoft.Data.Analysis;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
//using System.Runtime.InteropServices;
//using System.Diagnostics;
using OfficeOpenXml;

namespace MPPPenAutoTuning
{
    public partial class frmSummarizeLogData : Form
    {
        private enum DataType
        {
            AllData,
            AllModifyData
        }

        private enum PhaseType
        {
            Beacon,
            BHF,
            PTHF
        }

        PhaseType[] m_ePhaseType_Array = new PhaseType[]
        {
            PhaseType.Beacon,
            PhaseType.BHF,
            PhaseType.PTHF
        };

        Dictionary<PhaseType, string> dictNewStepNameMapping = new Dictionary<PhaseType, string>()
        {
            { PhaseType.Beacon, "N" },
            { PhaseType.BHF,    "TN_BHF" },
            { PhaseType.PTHF,   "TN_PTHF" }
        };

        Dictionary<PhaseType, string> dictOldStepNameMapping = new Dictionary<PhaseType, string>()
        {
            { PhaseType.Beacon, "Noise" },
            { PhaseType.BHF,    "TiltNoise_BHF" },
            { PhaseType.PTHF,   "TiltNoise_PTHF" }
        };

        Dictionary<string, Type> dictColumnTypeMapping = new Dictionary<string, Type>()
        {
            { SpecificText.m_sRanking,      typeof(int) },
            { SpecificText.m_sFileName,     typeof(string) },
            { SpecificText.m_sFlowStep,     typeof(string) },
            { SpecificText.m_sReadPH1,      typeof(int) },
            { SpecificText.m_sReadPH2,      typeof(int) },
            { SpecificText.m_sFrequency,    typeof(double) },
            { SpecificText.m_sRXTotalMean,  typeof(double) },
            { SpecificText.m_sRXTotalMin,   typeof(int) },
            { SpecificText.m_sRXTotalMax,   typeof(int) },
            { SpecificText.m_sTXTotalMean,  typeof(double) },
            { SpecificText.m_sTXTotalMin,   typeof(int) },
            { SpecificText.m_sTXTotalMax,   typeof(int) },
            { SpecificText.m_sInnerTX,      typeof(double) },
            { SpecificText.m_sInnerRX,      typeof(double) },
            { SpecificText.m_sEdgeTX,       typeof(double) }, 
            { SpecificText.m_sEdgeRX,       typeof(double) },
            { m_sSKU_Name,                  typeof(string) },
            { m_sPhase,                     typeof(string) },
            { m_sTrace,                     typeof(string) },
            { m_sValue,                     typeof(double) },
            { m_ssubRank,                   typeof(int) },
            { SpecificText.m_sIndex,        typeof(int) }
        };

        private const string m_sSKU_Name = "SKU_Name";
        private const string m_sPhase = "Phase";
        private const string m_sTrace = "Trace";
        private const string m_sValue = "Value";
        private const string m_ssubRank = "subRank";

        private const string m_sRxBeaconMax = "RxBeaconMax";
        private const string m_sTxBeaconMax = "TxBeaconMax";
        private const string m_sRxBHFMax = "RxBHFMax";
        private const string m_sTxBHFMax = "TxBHFMax";
        private const string m_sRxPTHFMax = "RxPTHFMax";
        private const string m_sTxPTHFMax = "TxPTHFMax";

        private const string m_sMax_RX = "Max_RX";
        private const string m_sMax_TX = "Max_TX";

        private string m_sDefaultSourceFolderPath = string.Format(@"{0}", Application.StartupPath);
        private string m_sDefaultOutputFolderPath = string.Format(@"{0}", Application.StartupPath);

        private string m_sDataFolderPath = "";
        private string m_sOutputFolderPath = "";

        private string m_sDirectoryName = "";
        private List<string> m_sNotFoundFileDirectoryPath_List = new List<string>();

        private DataTable m_dtAll = null;

        private string m_sStepListFileName_Noise = "";
        private string m_sStepListFileName_TN_BHF = "";
        private string m_sStepListFileName_TN_PTHF = "";

        private int m_nCurrentValue = 0;
        private int m_nMaxValue = 0;

        private List<string> m_sSKUName_List = new List<string>();

        private bool m_bFileCreateFlag_LogSummary = false;

        public frmSummarizeLogData()
        {
            InitializeComponent();

            btnStart.Enabled = false;

            lblStatus.Text = "Ready";

            rtbxMessage.Clear();

            ckbxOutputFolderName.Checked = false;

            if (ckbxOutputFolderName.Checked == true)
                tbxOutputFolderName.Enabled = true;
            else
                tbxOutputFolderName.Enabled = false;
        }

        private void frmSummarizeLogData_Load(object sender, EventArgs e)
        {
            CheckStartButtonEnable();
        }

        private void btnSelectDataPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdDataFolder = new FolderBrowserDialog();
            fbdDataFolder.Description = "Please Select Data Folder Path";
            //fbdDataFolder.RootFolder = Environment.SpecialFolder.DesktopDirectory;
            fbdDataFolder.SelectedPath = m_sDefaultSourceFolderPath;

            if (fbdDataFolder.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(fbdDataFolder.SelectedPath) == true)
                {
                    m_sDefaultSourceFolderPath = fbdDataFolder.SelectedPath;
                    tbxDataPath.Text = m_sDefaultSourceFolderPath;
                    tbxDataPath.SelectionStart = tbxDataPath.Text.Length;
                }
            }

            CheckStartButtonEnable();
        }

        private void btnSelectOutputPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdOutputFolder = new FolderBrowserDialog();
            fbdOutputFolder.Description = "Please Select Output Folder Path";
            //fbdOutputFolder.RootFolder = Environment.SpecialFolder.DesktopDirectory;
            fbdOutputFolder.SelectedPath = m_sDefaultOutputFolderPath;

            if (fbdOutputFolder.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(fbdOutputFolder.SelectedPath) == true)
                {
                    m_sDefaultOutputFolderPath = fbdOutputFolder.SelectedPath;
                    tbxOutputPath.Text = m_sDefaultOutputFolderPath;
                    tbxOutputPath.SelectionStart = tbxOutputPath.Text.Length;
                }
            }

            CheckStartButtonEnable();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunStartProcess), null);
        }

        private void RunStartProcess(object objParameter)
        {
            OutputStatus("Process", Color.Black);

            SetInitialFlow();

            OutputMessage("The program has been started, please do not close the program");

            InitializeParameter();

            if (CheckOputFolderNameIsValid() == false)
                return;

            if (ReadDataFolderPath() == false)
                return;

            if (ReadOutputFolderPath() == false)
                return;

            string[] sFileName = CheckFileName();

            OutputMessage("..................................");

            CopyDataToExcel(sFileName);

            OutputMessage("..................................");

            ChangeAllDataFormat();

            OutputMessage("The program has ended. Thank you for using it");

            SetFinishFlow();

            OutputStatus("Complete", Color.Green);
        }

        private void OutputStatus(string sMessage, Color colorForeColor)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lblStatus.Text = sMessage;
                lblStatus.ForeColor = colorForeColor;
            });
        }

        private void OutputMessage(string sMessage)
        {
            this.Invoke((MethodInvoker)delegate
            {
                rtbxMessage.Text += sMessage + Environment.NewLine;
            });
        }

        private void SetProgressBar(int nMin, int nMax, int nStep)
        {
            this.Invoke((MethodInvoker)delegate
            {
                pbProgress.Minimum = nMin;
                pbProgress.Maximum = nMax;
                pbProgress.Step = nStep;
            });
        }

        private void AddProgressBar()
        {
            this.Invoke((MethodInvoker)delegate
            {
                pbProgress.PerformStep();
            });
        }

        private void tbxDataPath_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(tbxDataPath.Text) == true)
                m_sDefaultSourceFolderPath = tbxDataPath.Text;

            CheckStartButtonEnable();
        }

        private void tbxOutputPath_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(tbxOutputPath.Text) == true)
                m_sDefaultOutputFolderPath = tbxOutputPath.Text;

            CheckStartButtonEnable();
        }

        private void rtbxMessage_TextChanged(object sender, EventArgs e)
        {
            rtbxMessage.SelectionStart = rtbxMessage.TextLength;
            rtbxMessage.ScrollToCaret();
        }

        private void ckbxOutputFolderName_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbxOutputFolderName.Checked == true)
                tbxOutputFolderName.Enabled = true;
            else
                tbxOutputFolderName.Enabled = false;
        }

        private void CheckStartButtonEnable(bool bOutputHintFlag = true)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (Directory.Exists(tbxDataPath.Text) == true && Directory.Exists(tbxOutputPath.Text) == true)
                {
                    btnStart.Focus();
                    btnStart.ForeColor = Color.Blue;
                    btnStart.Enabled = true;

                    if (bOutputHintFlag == true)
                        OutputStatus("Please Click Start Button", Color.Blue);
                }
                else if (Directory.Exists(tbxDataPath.Text) == false)
                {
                    btnSelectDataPath.Focus();
                    btnSelectDataPath.ForeColor = Color.Blue;
                    lblDataPath.ForeColor = Color.Blue;
                    btnStart.Enabled = false;

                    if (bOutputHintFlag == true)
                        OutputStatus("Please Select Data Path", Color.Blue);
                }
                else if (Directory.Exists(tbxOutputPath.Text) == false)
                {
                    btnSelectOutputPath.Focus();
                    btnSelectOutputPath.ForeColor = Color.Blue;
                    lblOutputPath.ForeColor = Color.Blue;
                    btnStart.Enabled = false;

                    if (bOutputHintFlag == true)
                        OutputStatus("Please Select Output Path", Color.Blue);
                }

                if (Directory.Exists(tbxDataPath.Text) == true)
                {
                    btnSelectDataPath.ForeColor = Color.Black;
                    lblDataPath.ForeColor = Color.Black;
                }

                if (Directory.Exists(tbxOutputPath.Text) == true)
                {
                    btnSelectOutputPath.ForeColor = Color.Black;
                    lblOutputPath.ForeColor = Color.Black;
                }
            });
        }

        private void SetInitialFlow()
        {
            this.Invoke((MethodInvoker)delegate
            {
                rtbxMessage.Clear();

                tbxDataPath.Enabled = false;
                btnSelectDataPath.Enabled = false;
                ckbxOutputFolderName.Enabled = false;
                tbxOutputFolderName.Enabled = false;
                btnStart.ForeColor = Color.Black;
                btnStart.Enabled = false;
                ckbxIncludeMaxValueInChart.Enabled = false;

                pbProgress.Minimum = 0;
                pbProgress.Maximum = 100;
                pbProgress.Step = 1;
                pbProgress.Value = 0;
            });
        }

        private void InitializeParameter()
        {
            m_sDataFolderPath = "";
            m_sOutputFolderPath = "";

            m_sDirectoryName = "";

            m_sNotFoundFileDirectoryPath_List.Clear();

            m_dtAll = null;

            m_sSKUName_List.Clear();

            m_sStepListFileName_Noise = "";
            m_sStepListFileName_TN_BHF = "";
            m_sStepListFileName_TN_PTHF = "";

            m_bFileCreateFlag_LogSummary = false;
        }

        private void DisplayProgress(bool bInitializeFlag = false, int nDataCount = 0)
        {
            if (bInitializeFlag == true)
            {
                m_nCurrentValue = 0;
                m_nMaxValue = nDataCount * 2 + 2;
                SetProgressBar(0, m_nMaxValue, 1);
                string sPercentage = Convert.ToString((int)(((double)m_nCurrentValue / (double)m_nMaxValue) * 100.0));
                OutputStatus(string.Format("Process({0}%)", sPercentage), Color.Black);
            }
            else
            {
                AddProgressBar();
                m_nCurrentValue++;
                string sPercentage = Convert.ToString((int)(((double)m_nCurrentValue / (double)m_nMaxValue) * 100.0));
                OutputStatus(string.Format("Process({0}%)", sPercentage), Color.Black);
            }
        }

        private void SetFinishFlow(bool bErrorFlag = false)
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnSelectDataPath.Enabled = true;
                btnSelectOutputPath.Enabled = true;
                tbxDataPath.Enabled = true;
                tbxOutputPath.Enabled = true;
                ckbxIncludeMaxValueInChart.Enabled = true;

                ckbxOutputFolderName.Enabled = true;

                if (ckbxOutputFolderName.Checked == true)
                    tbxOutputFolderName.Enabled = true;
                else
                    tbxOutputFolderName.Enabled = false;
            });

            CheckStartButtonEnable(false);

            if (bErrorFlag == true)
                OutputStatus("Error", Color.Red);
            else
                OutputStatus("Complete", Color.Green);
        }

        private bool CheckOputFolderNameIsValid()
        {
            if (ckbxOutputFolderName.Checked == false)
                return true;

            if (ElanConvert.CheckIsValidFileName(tbxOutputFolderName.Text) == true)
            {
                OutputMessage(string.Format("Folder Name Format : {0}", tbxOutputFolderName.Text));
                return true;
            }
            else
            {
                OutputMessage("Output Folder Name Format Error");
                SetFinishFlow(true);
                return false;
            }
        }

        private bool ReadDataFolderPath()
        {
            if (Directory.Exists(tbxDataPath.Text) == true)
            {
                m_sDataFolderPath = tbxDataPath.Text;
                m_sDefaultSourceFolderPath = m_sDataFolderPath;

                OutputMessage(string.Format("Data Folder Path : {0}", m_sDataFolderPath));
                return true;
            }
            else
            {
                OutputMessage(string.Format("Data Folder Path Not Exist({0})", tbxDataPath.Text));
                SetFinishFlow(true);
                return false;
            }
        }

        private bool ReadOutputFolderPath()
        {
            if (Directory.Exists(tbxOutputPath.Text) == true)
            {
                m_sOutputFolderPath = tbxOutputPath.Text;
                m_sDefaultOutputFolderPath = m_sOutputFolderPath;

                OutputMessage(string.Format("Output Folder Path : {0}", m_sOutputFolderPath));
                return true;
            }
            else
            {
                OutputMessage(string.Format("Output Folder Path Not Exist({0})", tbxOutputPath.Text));
                SetFinishFlow(true);
                return false;
            }
        }

        private string[] CheckFileName()
        {
            string[] sOutputFileName_Array = null;

            string sFileName_1 = string.Format("StepList_{0}.csv", dictNewStepNameMapping[PhaseType.Beacon]);
            string sFileName_2 = string.Format("StepList_{0}.csv", dictOldStepNameMapping[PhaseType.Beacon]);

            string[] sFilePath_1_Array = Directory.GetFiles(m_sDataFolderPath, sFileName_1, SearchOption.AllDirectories);
            string[] sFilePath_2_Array = Directory.GetFiles(m_sDataFolderPath, sFileName_2, SearchOption.AllDirectories);

            if (sFilePath_1_Array.Length > 0 && sFilePath_2_Array.Length == 0)
            {
                sOutputFileName_Array = new string[]
                {
                    string.Format("StepList_{0}.csv", dictNewStepNameMapping[PhaseType.Beacon]),
                    string.Format("StepList_{0}.csv", dictNewStepNameMapping[PhaseType.BHF]),
                    string.Format("StepList_{0}.csv", dictNewStepNameMapping[PhaseType.PTHF])
                };

                m_sStepListFileName_Noise =  string.Format("StepList_{0}.csv", dictNewStepNameMapping[PhaseType.Beacon]);
                m_sStepListFileName_TN_BHF = string.Format("StepList_{0}.csv", dictNewStepNameMapping[PhaseType.BHF]);
                m_sStepListFileName_TN_PTHF = string.Format("StepList_{0}.csv", dictNewStepNameMapping[PhaseType.PTHF]);

                m_sDirectoryName = new DirectoryInfo(Path.GetDirectoryName(sFilePath_1_Array[0])).Name;

                return sOutputFileName_Array;
            }
            if (sFilePath_1_Array.Length == 0 && sFilePath_2_Array.Length > 0)
            {
                sOutputFileName_Array = new string[]
                {
                    string.Format("StepList_{0}.csv", dictOldStepNameMapping[PhaseType.Beacon]),
                    string.Format("StepList_{0}.csv", dictOldStepNameMapping[PhaseType.BHF]),
                    string.Format("StepList_{0}.csv", dictOldStepNameMapping[PhaseType.PTHF])
                };

                m_sStepListFileName_Noise = string.Format("StepList_{0}.csv", dictOldStepNameMapping[PhaseType.Beacon]);
                m_sStepListFileName_TN_BHF = string.Format("StepList_{0}.csv", dictOldStepNameMapping[PhaseType.BHF]);
                m_sStepListFileName_TN_PTHF = string.Format("StepList_{0}.csv", dictOldStepNameMapping[PhaseType.PTHF]);

                m_sDirectoryName = new DirectoryInfo(Path.GetDirectoryName(sFilePath_2_Array[0])).Name;

                return sOutputFileName_Array;
            }
            else
                OutputMessage("Target file name does not comply with the default format");

            return sOutputFileName_Array;
        }

        private void SearchFile(ref List<string> sFilePath_List, ref List<string> sDirectoryPath_List, string[] sFileName_Array, string sFolderPath, string[] sSpareFileName_Array = null)
        {
            List<string> sRawDirectoryPath_List = new List<string>();

            foreach (string sFileName in sFileName_Array)
            {
                string[] sFilePath_Array = Directory.GetFiles(sFolderPath, sFileName, SearchOption.AllDirectories);

                if (sFilePath_Array == null || sFilePath_Array.Length == 0)
                {
                    if (sSpareFileName_Array != null)
                    {
                        foreach (string sSpareFileName in sSpareFileName_Array)
                        {
                            string[] sSpareFilePath_Array = Directory.GetFiles(sFolderPath, sSpareFileName, SearchOption.AllDirectories);

                            foreach (string sFilePath in sSpareFilePath_Array)
                            {
                                sFilePath_List.Add(sFilePath);

                                string sDirectoryPath = Path.GetDirectoryName(sFilePath);
                                sRawDirectoryPath_List.Add(sDirectoryPath);
                            }
                        }
                    }
                }
                else
                {
                    foreach (string sFilePath in sFilePath_Array)
                    {
                        sFilePath_List.Add(sFilePath);

                        string sDirectoryPath = Path.GetDirectoryName(sFilePath);
                        sRawDirectoryPath_List.Add(sDirectoryPath);
                    }
                }
            }

            sFilePath_List.Sort();
            sDirectoryPath_List = sRawDirectoryPath_List.Distinct().ToList();
            sDirectoryPath_List.Sort();
        }

        // 集中 output 資料夾命名規則，讓 UI 與驗證腳本共用相同邏輯。
        private string BuildSummaryOutputFolderName()
        {
            if (ckbxOutputFolderName.Checked == true)
                return tbxOutputFolderName.Text;

            string[] sSplitText_Array = m_sDirectoryName.Split('_');
            string sOutputFolderName = "";
            int nMaxCount = (sSplitText_Array.Length < 3) ? sSplitText_Array.Length : 3;

            for (int nTextIndex = 0; nTextIndex < nMaxCount; nTextIndex++)
            {
                if (nTextIndex == nMaxCount - 1)
                    sOutputFolderName = string.Format("{0}{1}", sOutputFolderName, sSplitText_Array[nTextIndex]);
                else
                    sOutputFolderName = string.Format("{0}{1}_", sOutputFolderName, sSplitText_Array[nTextIndex]);
            }

            return string.Format("output_{0}", sOutputFolderName);
        }

        // 集中 summary 輸出資料夾建立，避免各輸出方法自行重複處理目錄。
        private string EnsureSummaryOutputFolderPath()
        {
            string sOutputFolderName = BuildSummaryOutputFolderName();

            m_sOutputFolderPath = string.Format(@"{0}\{1}", m_sDefaultOutputFolderPath, sOutputFolderName);

            if (Directory.Exists(m_sOutputFolderPath) == false)
                Directory.CreateDirectory(m_sOutputFolderPath);

            return m_sOutputFolderPath;
        }

        // 集中 summary 輸出檔名與完整路徑規則，避免固定檔名散落在流程內。
        private string GetSummaryOutputFilePath(string sFileName)
        {
            string sOutputFolderPath = EnsureSummaryOutputFolderPath();
            return string.Format(@"{0}\{1}", sOutputFolderPath, sFileName);
        }

        private void CopyDataToExcel(string[] sFileName_Array)
        {
            OutputMessage("In progress...");

            string sDate = DateTime.Now.ToString("yyyy-MM-dd");
            string sTime = DateTime.Now.ToString("HH:mm");

            EnsureSummaryOutputFolderPath();

            string sFileNotFoundPath = GetSummaryOutputFilePath("file_not_found.txt");
            string sLogSummaryPath = GetSummaryOutputFilePath("Log Summary.xlsx");
            string sAllDataPath = GetSummaryOutputFilePath("df_all.xlsx");

            List<string> sFilePath_List = new List<string>();
            List<string> sDirectoryPath_List = new List<string>();

            SearchFile(ref sFilePath_List, ref sDirectoryPath_List, sFileName_Array, m_sDataFolderPath);

            WriteDateAndTimeToFileNotFoundFile(sFileNotFoundPath, sDate, sTime);

            int nDataIndex = 0;

            DisplayProgress(true, sDirectoryPath_List.Count);

            foreach (string sDirectoryPath in sDirectoryPath_List)
            {
                string sSKUName = new DirectoryInfo(sDirectoryPath).Name;
                m_sSKUName_List.Add(sSKUName);

                List<string> sExistFilePath_1_List = new List<string>();
                List<string> sNotExistFileName_List = new List<string>();

                foreach (string sFileName in sFileName_Array)
                {
                    string sFilePath = string.Format(@"{0}\{1}", sDirectoryPath, sFileName);

                    if (File.Exists(sFilePath) == true)
                        sExistFilePath_1_List.Add(sFilePath);
                    else
                    {
                        m_sNotFoundFileDirectoryPath_List.Add(sDirectoryPath);
                        sNotExistFileName_List.Add(sFileName);

                        WriteDirectoryPathAndFileNameToFileNotFoundFile(sFileNotFoundPath, sDirectoryPath, sFileName);
                    }
                }

                DataTable dtStepList = MergeDataToDataTable_StepList(sExistFilePath_1_List, sSKUName);

                int nWRExistFlag = 0;
                List<string> sExistFilePath_2_List = new List<string>();

                foreach (PhaseType ePhaseType in m_ePhaseType_Array)
                    CheckTotalRankDataIsExist(ref nWRExistFlag, ref sExistFilePath_2_List, ref sNotExistFileName_List, ePhaseType, sDirectoryPath, sFileNotFoundPath);

                DisplayProgress();

                DataTable dtMerge = null;

                if (nWRExistFlag != 3)
                {
                    OutputMessage(string.Format("{0} does not exist!", SpecificText.m_sResultFileName));

                    dtMerge = dtStepList.Copy();
                }
                else
                {
                    DataTable dtTotalRank = MergeDataToDataTable_TotalRank(sExistFilePath_2_List);

                    ConcatTotalRankDataTable(ref dtStepList, dtTotalRank);

                    dtMerge = dtStepList.Copy();
                }

                if (m_dtAll == null)
                {
                    m_dtAll = new DataTable("All Table");
                    m_dtAll = dtMerge.Copy();
                }
                else
                    m_dtAll.Merge(dtMerge);

                DataTable dtBeacon = null;

                foreach (PhaseType ePhaseType in m_ePhaseType_Array)
                {
                    if (ePhaseType == PhaseType.Beacon)
                        dtBeacon = ExportBeaconTotalRankDataToExcel(sNotExistFileName_List, dtStepList, sLogSummaryPath, nDataIndex);
                    else
                        ExportBHFAndPTHFTotalRankDataToExcel(sNotExistFileName_List, ePhaseType, dtStepList, dtBeacon, sLogSummaryPath, nDataIndex);
                }

                SetTitleAndColumnFormatToExcel(sLogSummaryPath, sSKUName, nDataIndex * 23, 8);

                string[] sChartFileName_Array = null;
                string[] sSpareChartFileName_Array = null;

                if (ckbxIncludeMaxValueInChart.Checked == true)
                {
                    sChartFileName_Array = new string[]
                    {
                        string.Format("Total_{0}", SpecificText.m_sFrqChartIncludeMaxFileName)
                    };

                    sSpareChartFileName_Array = new string[]
                    {
                        string.Format("Total_{0}", SpecificText.m_sFreChartFileName),
                        string.Format("Total_{0}", SpecificText.m_sFrqChartFileName)
                    };
                }
                else
                {
                    sChartFileName_Array = new string[]
                    {
                        string.Format("Total_{0}", SpecificText.m_sFreChartFileName),
                        string.Format("Total_{0}", SpecificText.m_sFrqChartFileName)
                    };
                }

                List<string> sChartFilePath_List = new List<string>();
                List<string> sChartDirectoryPath_List = new List<string>();

                SearchFile(ref sChartFilePath_List, ref sChartDirectoryPath_List, sChartFileName_Array, sDirectoryPath, sSpareChartFileName_Array);

                AddImageToExcel(sLogSummaryPath, sChartFilePath_List, nDataIndex);

                nDataIndex++;

                DisplayProgress();
            }
            
            ExportAllDataToExcel(m_dtAll, sAllDataPath, DataType.AllData);

            DisplayProgress();
        }

        private DataTable MergeDataToDataTable_StepList(List<string> sFilePath_List, string sSKUName)
        {
            int nSkipRowNumber = 10;

            string[] nGetColumnName_Array = new string[]
            {
                SpecificText.m_sRanking,
                SpecificText.m_sFileName,
                SpecificText.m_sFlowStep,
                SpecificText.m_sReadPH1,
                SpecificText.m_sReadPH2,
                SpecificText.m_sFrequency,
                SpecificText.m_sRXTotalMean,
                SpecificText.m_sRXTotalMin,
                SpecificText.m_sRXTotalMax,
                SpecificText.m_sTXTotalMean,
                SpecificText.m_sTXTotalMin,
                SpecificText.m_sTXTotalMax
            };

            string[] sRemoveColumnName_Array = new string[]
            {
                SpecificText.m_sFileName,
                SpecificText.m_sFlowStep,
                SpecificText.m_sReadPH1,
                SpecificText.m_sReadPH2,
                SpecificText.m_sRXTotalMean,
                SpecificText.m_sRXTotalMin,
                SpecificText.m_sTXTotalMean,
                SpecificText.m_sTXTotalMin
            };

            DataTable dtOutput = null;

            foreach (string sFilePath in sFilePath_List)
            {
                List<string> sRemoverColumnName_List = new List<string>();

                DataTable dtOriginal = ConvertCSVFileToDataTable(sFilePath, "StepList Table", ",", nSkipRowNumber);

                DataView view = new DataView(dtOriginal);
                DataTable dtProcess = view.ToTable("StepList Table", false, nGetColumnName_Array);

                dtProcess.Columns.Add(m_sSKU_Name, dictColumnTypeMapping[m_sSKU_Name]);
                dtProcess.Columns.Add(m_sPhase, dictColumnTypeMapping[m_sPhase]);

                foreach (DataRow dr in dtProcess.Rows)
                {
                    dr[m_sSKU_Name] = sSKUName;

                    if (dr[SpecificText.m_sFlowStep].ToString() == "NO")
                        dr[m_sPhase] = PhaseType.Beacon.ToString();
                    else if (dr[SpecificText.m_sFlowStep].ToString() == "TILTNO_BHF")
                        dr[m_sPhase] = PhaseType.BHF.ToString();
                    else if (dr[SpecificText.m_sFlowStep].ToString() == "TILTNO_PTHF")
                        dr[m_sPhase] = PhaseType.PTHF.ToString();
                }

                foreach (string sColumnName in sRemoveColumnName_Array)
                    dtProcess.Columns.Remove(sColumnName);

                if (dtOutput == null)
                {
                    dtOutput = new DataTable("StepList Table");
                    dtOutput = dtProcess.Copy();
                }
                else
                    dtOutput.Merge(dtProcess);
            }

            return dtOutput;
        }

        private void CheckTotalRankDataIsExist(ref int nWRExistFlag, ref List<string> sExistFilePath_2_List, ref List<string> sNotExistFileName_List, PhaseType ePhaseType, string sDirectoryPath, string sFileNotFoundPath)
        {
            string sResultFileName = string.Format(@"Result({0})\{1}", dictNewStepNameMapping[ePhaseType], SpecificText.m_sResultFileName);
            string sResultFilePath = string.Format(@"{0}\{1}", sDirectoryPath, sResultFileName);

            if (File.Exists(sResultFilePath) == true)
            {
                nWRExistFlag++;
                sExistFilePath_2_List.Add(sResultFilePath);
            }
            else
            {
                m_sNotFoundFileDirectoryPath_List.Add(sDirectoryPath);
                sNotExistFileName_List.Add(SpecificText.m_sResultFileName);

                WriteDirectoryPathAndFileNameToFileNotFoundFile(sFileNotFoundPath, sDirectoryPath, sResultFileName);
            }
        }

        private DataTable MergeDataToDataTable_TotalRank(List<string> sFilePath_List)
        {
            int nSkipRowNumber = 8;

            string[] nGetColumnName_Array = new string[]
            {
                SpecificText.m_sRanking,
                SpecificText.m_sFileName,
                SpecificText.m_sInnerTX,
                SpecificText.m_sInnerRX,
                SpecificText.m_sEdgeTX,
                SpecificText.m_sEdgeRX
            };

            string[] sRemoveColumnName_Array = new string[]
            {
                SpecificText.m_sRanking,
                SpecificText.m_sFileName
            };

            DataTable dtOutput = null;

            foreach (string sFilePath in sFilePath_List)
            {
                List<string> sRemoverColumnName_List = new List<string>();

                DataTable dtOriginal = ConvertCSVFileToDataTable(sFilePath, "TotalRank Table", ",", nSkipRowNumber);

                DataView view = new DataView(dtOriginal);
                DataTable dtProcess = view.ToTable("TotalRank Table", false, nGetColumnName_Array);

                dtProcess.Columns.Add(SpecificText.m_sFrequency, dictColumnTypeMapping[SpecificText.m_sFrequency]);
                dtProcess.Columns.Add(m_sPhase, dictColumnTypeMapping[m_sPhase]);

                foreach (DataRow dr in dtProcess.Rows)
                {
                    string sFileName = dr[SpecificText.m_sFileName].ToString();
                    string[] sSplit_Array = sFileName.Split('_');

                    if (sFileName.IndexOf(dictNewStepNameMapping[PhaseType.Beacon]) == 0)
                    {
                        dr[SpecificText.m_sFrequency] = Convert.ToDouble(sSplit_Array[1].Replace("KHz", ""));
                        dr[m_sPhase] = PhaseType.Beacon.ToString();
                    }
                    else if (sFileName.IndexOf(dictNewStepNameMapping[PhaseType.BHF]) == 0)
                    {
                        dr[SpecificText.m_sFrequency] = Convert.ToDouble(sSplit_Array[2].Replace("KHz", ""));
                        dr[m_sPhase] = PhaseType.BHF.ToString();
                    }
                    else if (sFileName.IndexOf(dictNewStepNameMapping[PhaseType.PTHF]) == 0)
                    {
                        dr[SpecificText.m_sFrequency] = Convert.ToDouble(sSplit_Array[2].Replace("KHz", ""));
                        dr[m_sPhase] = PhaseType.PTHF.ToString();
                    }
                }

                foreach (string sColumnName in sRemoveColumnName_Array)
                    dtProcess.Columns.Remove(sColumnName);

                if (dtOutput == null)
                {
                    dtOutput = new DataTable("TotalRank Table");
                    dtOutput = dtProcess.Copy();
                }
                else
                    dtOutput.Merge(dtProcess);
            }

            return dtOutput;
        }

        public DataTable ConvertCSVFileToDataTable(string sFilePath, string sTableName, string sDelimiter, int nSkipRowNumber = 0)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            StreamReader sr = new StreamReader(sFilePath, System.Text.Encoding.Default);

            for (int nSkipIndex = 0; nSkipIndex < nSkipRowNumber; nSkipIndex++)
                sr.ReadLine();

            string[] sColumn_Array = sr.ReadLine().Split(sDelimiter.ToCharArray());
            ds.Tables.Add(sTableName);

            foreach (string sColumn in sColumn_Array)
            {
                bool bAddedFlag = false;
                string sNextText = "";
                int nIndex = 0;

                while (bAddedFlag == false)
                {
                    string sColumnName = sColumn + sNextText;
                    sColumnName = sColumnName.Replace("#", "");
                    sColumnName = sColumnName.Replace("'", "");
                    sColumnName = sColumnName.Replace("&", "");

                    if (ds.Tables[sTableName].Columns.Contains(sColumnName) == false)
                    {
                        if (dictColumnTypeMapping.ContainsKey(sColumnName) == true)
                            ds.Tables[sTableName].Columns.Add(sColumnName, dictColumnTypeMapping[sColumnName]);
                        else
                            ds.Tables[sTableName].Columns.Add(sColumnName, typeof(string));

                        bAddedFlag = true;
                    }
                    else
                    {
                        nIndex++;
                        sNextText = "_" + nIndex.ToString();
                    }
                }
            }

            string sAllData = sr.ReadToEnd();
            string[] sRow_Array = sAllData.Split("\n".ToCharArray());

            foreach (string sRow in sRow_Array)
            {
                if (sRow == "" || sRow == "\r")
                    break;

                string[] sItem_Array = sRow.Split(sDelimiter.ToCharArray());
                ds.Tables[sTableName].Rows.Add(sItem_Array);
            }

            sr.Close();

            dt = ds.Tables[0];

            return dt;
        }

        private void ConcatTotalRankDataTable(ref DataTable dtStepList, DataTable dtTotalRank)
        {
            string[] sMergeColumn_Array = new string[]
            {
                SpecificText.m_sInnerTX,
                SpecificText.m_sInnerRX,
                SpecificText.m_sEdgeTX,
                SpecificText.m_sEdgeRX
            };

            foreach (string sMergeColumn in sMergeColumn_Array)
                dtStepList.Columns.Add(sMergeColumn, dictColumnTypeMapping[sMergeColumn]);

            foreach (DataRow drStepList in dtStepList.Rows)
            {
                double dFrequency_StepList = Convert.ToDouble(drStepList[SpecificText.m_sFrequency].ToString());
                string sPhase_StepList = drStepList[m_sPhase].ToString();

                foreach (DataRow drTotalRank in dtTotalRank.Rows)
                {
                    double dFrequency_TotalRank = Convert.ToDouble(drTotalRank[SpecificText.m_sFrequency].ToString());
                    string sPhase_TotalRank = drTotalRank[m_sPhase].ToString();

                    if (dFrequency_StepList == dFrequency_TotalRank && sPhase_StepList == sPhase_TotalRank)
                    {
                        foreach (string sMergeColumn in sMergeColumn_Array)
                            drStepList[sMergeColumn] = drTotalRank[sMergeColumn];

                        break;
                    }
                }
            }
        }

        private DataTable GetDataTableByPhase(DataTable dt, PhaseType ePhase)
        {
            string sPhase = ePhase.ToString();

            DataTable dtOutput = new DataTable(string.Format("{0}Title Table", sPhase));

            dtOutput.Columns.Add(SpecificText.m_sRanking);
            dtOutput.Columns.Add(SpecificText.m_sFrequency);  

            foreach (DataRow dr in dt.Rows)
            {
                if (dr[m_sPhase].ToString() == sPhase)
                    dtOutput.Rows.Add(dr[SpecificText.m_sRanking].ToString(), dr[SpecificText.m_sFrequency].ToString());
            }

            return dtOutput;
        }

        private DataTable GetAllDataTableByPhase(DataTable dt, PhaseType ePhase)
        {
            string sPhase = ePhase.ToString();

            DataTable dtOutput = new DataTable(string.Format("{0}Title Table", sPhase));
            dtOutput = dt.Copy();

            for (int nRowIndex = dtOutput.Rows.Count - 1; nRowIndex >= 0; nRowIndex--)
            {
                if (dtOutput.Rows[nRowIndex][m_sPhase].ToString() != sPhase)
                    dtOutput.Rows.RemoveAt(nRowIndex);
            }

            return dtOutput;
        }

        private void RemoveDataColumn(ref DataTable dt, bool bIncludeRanking)
        {
            string[] sColumnName_Array = null;

            if (bIncludeRanking == false)
            {
                sColumnName_Array = new string[]
                {
                    SpecificText.m_sRanking,
                    SpecificText.m_sFrequency,
                    SpecificText.m_sRXTotalMax,
                    SpecificText.m_sTXTotalMax
                };
            }
            else
            {
                sColumnName_Array = new string[]
                {
                    SpecificText.m_sFrequency,
                    SpecificText.m_sRXTotalMax,
                    SpecificText.m_sTXTotalMax
                };
            }

            for (int nColumnIndex = dt.Columns.Count - 1; nColumnIndex >= 0; nColumnIndex--)
            {
                string sColumnName = dt.Columns[nColumnIndex].ToString();

                if (sColumnName_Array.Contains(sColumnName) == false)
                    dt.Columns.RemoveAt(nColumnIndex);
            }
        }

        private void SortDataTableByRanking(ref DataTable dtSource, DataTable dtRanking)
        {
            dtSource.Columns.Add(SpecificText.m_sIndex, dictColumnTypeMapping[SpecificText.m_sIndex]);

            foreach (DataRow drRanking in dtRanking.Rows)
            {
                int nRankNumber_Ranking = Convert.ToInt32(drRanking[SpecificText.m_sRanking].ToString());
                double dFrequency_Ranking = Convert.ToDouble(drRanking[SpecificText.m_sFrequency].ToString());

                foreach (DataRow drSource in dtSource.Rows)
                {
                    double dFrequency_Source = Convert.ToDouble(drSource[SpecificText.m_sFrequency].ToString());

                    if (dFrequency_Ranking == dFrequency_Source)
                    {
                        drSource[SpecificText.m_sIndex] = nRankNumber_Ranking;
                        break;
                    }
                }
            }
           
            dtSource.DefaultView.Sort = string.Format("{0} ASC", SpecificText.m_sIndex);
            dtSource = dtSource.DefaultView.ToTable();
            dtSource.Columns.Remove(SpecificText.m_sIndex);
        }

        private void WriteDateAndTimeToFileNotFoundFile(string sFilePath, string sDate, string sTime)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.Write(string.Format("{0} ", sDate));
                sw.WriteLine(string.Format("{0} : ", sTime));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WriteDirectoryPathAndFileNameToFileNotFoundFile(string sFilePath, string sDirectoryPath, string sFileName)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.Write(string.Format("{0} :", sDirectoryPath));
                sw.WriteLine(string.Format("{0}", sFileName));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private DataTable ExportBeaconTotalRankDataToExcel(List<string> sNotExistFileName_List, DataTable dtStepList, string sLogSummaryPath, int nDataIndex)
        {
            DataTable dtBeacon = null;

            if (sNotExistFileName_List.Contains(m_sStepListFileName_Noise) == true)
            {
                if (sNotExistFileName_List.Contains(m_sStepListFileName_TN_BHF) == true)
                {
                    if (sNotExistFileName_List.Contains(m_sStepListFileName_TN_PTHF) == true)
                        OutputMessage("Three CSV files do not exist");
                    else
                    {
                        DataTable dtPTHF_Title = GetDataTableByPhase(dtStepList, PhaseType.PTHF);
                        ExportToExcel(dtPTHF_Title, sLogSummaryPath, 1 + nDataIndex * 23, 8);
                    }
                }
                else
                {
                    DataTable dtBHF_Title = GetDataTableByPhase(dtStepList, PhaseType.BHF);
                    ExportToExcel(dtBHF_Title, sLogSummaryPath, 1 + nDataIndex * 23, 8); ;
                }

                OutputMessage(string.Format("StepList_{0}.csv does not exist", dictNewStepNameMapping[PhaseType.Beacon]));
            }
            else
            {
                dtBeacon = GetAllDataTableByPhase(dtStepList, PhaseType.Beacon);
                RemoveDataColumn(ref dtBeacon, false);
                ExportToExcel(dtBeacon, sLogSummaryPath, 1 + nDataIndex * 23, 8, true, new string[] { SpecificText.m_sRXTotalMax, SpecificText.m_sTXTotalMax });
            }

            return dtBeacon;
        }

        private void ExportBHFAndPTHFTotalRankDataToExcel(List<string> sNotExistFileName_List, PhaseType ePhaseType, DataTable dtStepList, DataTable dtBeacon, string sLogSummaryPath, int nDataIndex)
        {
            string sStepListFileName = "";
            int nStartColumnIndex = 0;

            if (ePhaseType == PhaseType.BHF)
            {
                sStepListFileName = m_sStepListFileName_TN_BHF;
                nStartColumnIndex = 12;
            }
            else if (ePhaseType == PhaseType.PTHF)
            {
                sStepListFileName = m_sStepListFileName_TN_PTHF;
                nStartColumnIndex = 15;
            }

            if (sNotExistFileName_List.Contains(sStepListFileName) == true)
                OutputMessage(string.Format("StepList_{0}.csv does not exist", dictNewStepNameMapping[ePhaseType]));
            else
            {
                DataTable dt = GetAllDataTableByPhase(dtStepList, ePhaseType);
                RemoveDataColumn(ref dt, true);

                if (sNotExistFileName_List.Contains(m_sStepListFileName_Noise) == false)
                    SortDataTableByRanking(ref dt, dtBeacon);

                ExportToExcel(dt, sLogSummaryPath, 1 + nDataIndex * 23, nStartColumnIndex, true, new string[] { SpecificText.m_sRXTotalMax, SpecificText.m_sTXTotalMax });
            }
        }

        private void ExportToExcel(DataTable dt, string sFilePath, int nStartRowIndex, int nStartColumnIndex, bool bHighlightFlag = false, string[] sHighlightColumn_Array = null)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            ExcelPackage cExcelPackage;
            ExcelWorksheet cSheet;

            if (m_bFileCreateFlag_LogSummary == true)
            {
                var vFileInfo = new FileInfo(sFilePath);
                cExcelPackage = new ExcelPackage(vFileInfo);
                cSheet = cExcelPackage.Workbook.Worksheets["summary"];
            }
            else
            {
                cExcelPackage = new ExcelPackage();
                cSheet = cExcelPackage.Workbook.Worksheets.Add("summary");
                cSheet.Name = "summary";

                m_bFileCreateFlag_LogSummary = true;
            }

            cSheet.Cells.Style.Font.Name = "新細明體";
            cSheet.Cells.Style.Font.Size = 11;
            //cSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            cSheet.Cells.Style.Numberformat.Format = "0";

            int nColumnIndex = 0;

            for (nColumnIndex = 1; nColumnIndex <= 18; nColumnIndex++)
                cSheet.Columns[nColumnIndex].Width = 13.7;

            // Write the data. Remember that Excel is 1-indexed
            nColumnIndex = nStartColumnIndex + 1;
            int nRowIndex = nStartRowIndex + 1;

            foreach (DataColumn dc in dt.Columns)
            {
                cSheet.Cells[nRowIndex, nColumnIndex].Value = dc.ToString();
                cSheet.Cells[nRowIndex, nColumnIndex].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                nColumnIndex++;
            }

            nRowIndex++;

            int nValueStartRowIndex = nRowIndex;

            nColumnIndex = nStartColumnIndex + 1;

            foreach (DataColumn dc in dt.Columns)
            {
                int nMaxIndex = 0;
                int nMinIndex = 0;
                bool bSetHighlightFlag = false;

                int nValueIndex = 0;

                if (bHighlightFlag == true)
                {
                    if (sHighlightColumn_Array.Contains(dc.ToString()) == true)
                    {
                        bSetHighlightFlag = true;

                        double dMaxValue = 0.0;
                        double dMinValue = 0.0;

                        nValueIndex = 0;

                        foreach (DataRow dr in dt.Rows)
                        {
                            double dValue = Convert.ToDouble(dr[dc].ToString());

                            if (nValueIndex == 0)
                                dMaxValue = dMinValue = dValue;
                            else
                            {
                                if (dValue > dMaxValue)
                                {
                                    dMaxValue = dValue;
                                    nMaxIndex = nValueIndex;
                                }

                                if (dValue < dMinValue)
                                {
                                    dMinValue = dValue;
                                    nMinIndex = nValueIndex;
                                }
                            }

                            nValueIndex++;
                        }
                    }
                }

                nRowIndex = nValueStartRowIndex;

                nValueIndex = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    if (dc.DataType == typeof(int))
                    {
                        cSheet.Cells[nRowIndex, nColumnIndex].Value = Convert.ToInt32(dr[dc].ToString());
                        cSheet.Cells[nRowIndex, nColumnIndex].Style.Numberformat.Format = "General";
                    }
                    else if (dc.DataType == typeof(double))
                    {
                        cSheet.Cells[nRowIndex, nColumnIndex].Value = Convert.ToDouble(dr[dc].ToString());
                        cSheet.Cells[nRowIndex, nColumnIndex].Style.Numberformat.Format = "General";
                    }
                    else if (dc.DataType == typeof(string))
                        cSheet.Cells[nRowIndex, nColumnIndex].Value = dr[dc].ToString();

                    cSheet.Cells[nRowIndex, nColumnIndex].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    if (bSetHighlightFlag == true)
                    {
                        if (nMinIndex == nValueIndex)
                        {
                            cSheet.Cells[nRowIndex, nColumnIndex].Style.Font.Color.SetColor(Color.Blue);
                            cSheet.Cells[nRowIndex, nColumnIndex].Style.Font.Bold = true;
                        }

                        if (nMaxIndex == nValueIndex)
                        {
                            cSheet.Cells[nRowIndex, nColumnIndex].Style.Font.Color.SetColor(Color.Red);
                            cSheet.Cells[nRowIndex, nColumnIndex].Style.Font.Bold = true;
                        }
                    }

                    nRowIndex++;
                    nValueIndex++;
                }

                nColumnIndex++;
            }

            cSheet.Protection.IsProtected = false;
            cSheet.Protection.AllowSelectLockedCells = false;

            //cExcelPackage.SaveAs(sFilePath);
            //cExcelPackage.Dispose();

            // 建立檔案串流
            FileStream fs = new FileStream(sFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            // 把剛剛的Excel物件真實存進檔案裡
            cExcelPackage.SaveAs(fs);
            // 關閉串流
            fs.Close();
            cExcelPackage.Dispose();
        }

        private void SetTitleAndColumnFormatToExcel(string sFilePath, string sSKUName, int nStartRowIndex, int nStartColumnIndex)
        {
            Dictionary<int, string> dictColumnMapping = new Dictionary<int, string>()
            {
                { 10, m_sRxBeaconMax },
                { 11, m_sTxBeaconMax },
                { 13, m_sRxBHFMax },
                { 14, m_sTxBHFMax },
                { 16, m_sRxPTHFMax },
                { 17, m_sTxPTHFMax }
            };

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            ExcelPackage cExcelPackage;
            ExcelWorksheet cSheet;

            if (m_bFileCreateFlag_LogSummary == true)
            {
                var vFileInfo = new FileInfo(sFilePath);
                cExcelPackage = new ExcelPackage(vFileInfo);
                cSheet = cExcelPackage.Workbook.Worksheets["summary"];
            }
            else
            {
                cExcelPackage = new ExcelPackage();
                cSheet = cExcelPackage.Workbook.Worksheets.Add("summary");
                cSheet.Name = "summary";

                m_bFileCreateFlag_LogSummary = true;
            }

            cSheet.Cells.Style.Font.Name = "新細明體";
            cSheet.Cells.Style.Font.Size = 11;
            //cSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            cSheet.Cells.Style.Numberformat.Format = "0";

            // Write the data. Remember that Excel is 1-indexed
            int nTitleIndex = nStartColumnIndex + 1;
            int nRowIndex = nStartRowIndex + 1;

            cSheet.Cells[nRowIndex, nTitleIndex].Value = sSKUName;
            cSheet.Cells[nRowIndex, nTitleIndex].Style.Font.Bold = true;
            cSheet.Cells[nRowIndex, nTitleIndex].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            nRowIndex++;

            int nColumnIndex = 0;

            foreach (KeyValuePair<int, string> structItem in dictColumnMapping)
            {
                nColumnIndex = structItem.Key + 1;
                string sColumnName = structItem.Value;

                cSheet.Cells[nRowIndex, nColumnIndex].Value = sColumnName;
            }

            using (var vRange = cSheet.Cells[nRowIndex, nStartColumnIndex + 1, nRowIndex, nStartColumnIndex + 1 + 9])
            {
                vRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                vRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                vRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                vRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            for (int nDataIndex = 0; nDataIndex < 10; nDataIndex++)
            {
                nColumnIndex = nDataIndex + nStartColumnIndex + 1;
                cSheet.Cells[nRowIndex, nColumnIndex].Style.Font.Bold = true;
            }

            cSheet.Protection.IsProtected = false;
            cSheet.Protection.AllowSelectLockedCells = false;

            //cExcelPackage.SaveAs(sFilePath);
            //cExcelPackage.Dispose();

            // 建立檔案串流
            FileStream fs = new FileStream(sFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            // 把剛剛的Excel物件真實存進檔案裡
            cExcelPackage.SaveAs(fs);
            // 關閉串流
            fs.Close();
            cExcelPackage.Dispose();
        }

        private void AddImageToExcel(string sFilePath, List<string> sChartFilePath_List, int nDataIndex)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            ExcelPackage cExcelPackage;
            ExcelWorksheet cSheet;

            if (m_bFileCreateFlag_LogSummary == true)
            {
                var vFileInfo = new FileInfo(sFilePath);
                cExcelPackage = new ExcelPackage(vFileInfo);
                cSheet = cExcelPackage.Workbook.Worksheets["summary"];
            }
            else
            {
                cExcelPackage = new ExcelPackage();
                cSheet = cExcelPackage.Workbook.Worksheets.Add("summary");
                cSheet.Name = "Sheet1";

                m_bFileCreateFlag_LogSummary = true;
            }

            for (int nChartIndex = 0; nChartIndex < sChartFilePath_List.Count; nChartIndex++)
            {
                string sFileName = string.Format("Chart_{0}_{1}", nDataIndex + 1, nChartIndex + 1);
                
                var vImageFile = new FileInfo(sChartFilePath_List[nChartIndex]);
                OfficeOpenXml.Drawing.ExcelPicture pic = cSheet.Drawings.AddPicture(sFileName, vImageFile);
                int nTopRow = nDataIndex * 23 + nChartIndex * 7;
                pic.SetPosition(nTopRow, 0, 0, 0);
                pic.SetSize(750, 145);
            }

            cSheet.Protection.IsProtected = false;
            cSheet.Protection.AllowSelectLockedCells = false;

            //cExcelPackage.SaveAs(sFilePath);
            //cExcelPackage.Dispose();

            // 建立檔案串流
            FileStream fs = new FileStream(sFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            // 把剛剛的Excel物件真實存進檔案裡
            cExcelPackage.SaveAs(fs);
            // 關閉串流
            fs.Close();
            cExcelPackage.Dispose();
        }

        private void ExportAllDataToExcel(DataTable dt, string sFilePath, DataType eDataType)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            ExcelPackage cExcelPackage = new ExcelPackage();
            ExcelWorksheet cSheet = cExcelPackage.Workbook.Worksheets.Add("Sheet1");
            cSheet.Name = "Sheet1";
            cSheet.Cells.Style.Font.Name = "新細明體";
            cSheet.Cells.Style.Font.Size = 11;
            cSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            cSheet.Cells.Style.Numberformat.Format = "0";

            // Write the data. Remember that Excel is 1-indexed
            int nColumnIndex = 1;
            int nRowIndex = 1;

            foreach (DataColumn dc in dt.Columns)
            {
                cSheet.Cells[nRowIndex, nColumnIndex].Value = dc.ToString();
                cSheet.Cells[nRowIndex, nColumnIndex].Style.Font.Bold = true;

                nColumnIndex++;
            }

            using (var vRange = cSheet.Cells[1, 1, 1, dt.Columns.Count])
            {
                vRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                vRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                vRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                vRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            nRowIndex++;

            int nValueStartRowIndex = nRowIndex;
            nColumnIndex = 1;

            foreach (DataColumn dc in dt.Columns)
            {
                nRowIndex = nValueStartRowIndex;

                foreach (DataRow dr in dt.Rows)
                {
                    if (dc.DataType == typeof(int))
                    {
                        cSheet.Cells[nRowIndex, nColumnIndex].Value = Convert.ToInt32(dr[dc].ToString());
                        cSheet.Cells[nRowIndex, nColumnIndex].Style.Numberformat.Format = "General";
                    }
                    else if (dc.DataType == typeof(double))
                    {
                        cSheet.Cells[nRowIndex, nColumnIndex].Value = Convert.ToDouble(dr[dc].ToString());
                        cSheet.Cells[nRowIndex, nColumnIndex].Style.Numberformat.Format = "General";
                    }
                    else if (dc.DataType == typeof(string))
                        cSheet.Cells[nRowIndex, nColumnIndex].Value = dr[dc].ToString();

                    nRowIndex++;
                }

                nColumnIndex++;
            }

            cSheet.Cells.AutoFitColumns();

            cSheet.Protection.IsProtected = false;
            cSheet.Protection.AllowSelectLockedCells = false;

            //cExcelPackage.SaveAs(sFilePath);
            //cExcelPackage.Dispose();

            // 建立檔案串流
            FileStream fs = new FileStream(sFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            // 把剛剛的Excel物件真實存進檔案裡
            cExcelPackage.SaveAs(fs);
            // 關閉串流
            fs.Close();
            cExcelPackage.Dispose();
        }

        private void ChangeAllDataFormat()
        {
            string sAllModifyDataPath = GetSummaryOutputFilePath("df_all_M.xlsx");

            DataTable dtAllModify = GetAllModifyDataTable();

            ExportAllDataToExcel(dtAllModify, sAllModifyDataPath, DataType.AllModifyData);

            DisplayProgress();
        }

        private DataTable GetAllModifyDataTable()
        {
            DataTable dtOutput = null;

            PhaseType[] ePhase_Array = new PhaseType[]
            {
                PhaseType.Beacon,
                PhaseType.BHF,
                PhaseType.PTHF
            };

            Dictionary<string, string> dictTraceTypeMapping = new Dictionary<string, string>()
            {
                { m_sMax_RX, SpecificText.m_sRXTotalMax },
                { m_sMax_TX, SpecificText.m_sTXTotalMax },
                { SpecificText.m_sInnerTX, SpecificText.m_sInnerRX },
                { SpecificText.m_sInnerRX, SpecificText.m_sInnerRX },
                { SpecificText.m_sEdgeTX, SpecificText.m_sEdgeTX },
                { SpecificText.m_sEdgeRX, SpecificText.m_sEdgeTX }
            };

            foreach (string sSKUName in m_sSKUName_List)
            {
                foreach (PhaseType ePhase in ePhase_Array)
                {
                    string sExpression = string.Format("{0} = '{1}' and {2} = '{3}'", m_sSKU_Name, sSKUName, m_sPhase, ePhase.ToString());
                    string sSort = string.Format("{0} ASC", SpecificText.m_sRanking);
                    DataRow[] drSort_1_Array;
                    drSort_1_Array = m_dtAll.Select(sExpression, sSort);

                    if (drSort_1_Array == null || drSort_1_Array.Length == 0)
                        continue;

                    DataTable dt_1 = drSort_1_Array.CopyToDataTable();

                    DataTable dt_2 = new DataTable();
                    dt_2.Columns.Add(SpecificText.m_sRanking, dictColumnTypeMapping[SpecificText.m_sRanking]);
                    dt_2.Columns.Add(SpecificText.m_sFrequency, dictColumnTypeMapping[SpecificText.m_sFrequency]);
                    dt_2.Columns.Add(m_sSKU_Name, dictColumnTypeMapping[m_sSKU_Name]);
                    dt_2.Columns.Add(m_sPhase, dictColumnTypeMapping[m_sPhase]);
                    dt_2.Columns.Add(m_sTrace, dictColumnTypeMapping[m_sTrace]);
                    dt_2.Columns.Add(m_sValue, dictColumnTypeMapping[m_sValue]);

                    foreach (DataRow dr in dt_1.Rows)
                    {
                        string sRanking = dr[SpecificText.m_sRanking].ToString();
                        string sFrequency = dr[SpecificText.m_sFrequency].ToString();
                        string sSKU_Name = dr[m_sSKU_Name].ToString();
                        string sPhase = dr[m_sPhase].ToString();

                        foreach (KeyValuePair<string, string> structItem in dictTraceTypeMapping)
                        {
                            double dValue = Convert.ToDouble(dr[structItem.Value].ToString());
                            dt_2.Rows.Add(sRanking, sFrequency, sSKU_Name, sPhase, structItem.Key, dValue);
                        }
                    }

                    foreach (KeyValuePair<string, string> structItem in dictTraceTypeMapping)
                    {
                        sExpression = string.Format("{0} = '{1}'", m_sTrace, structItem.Key);
                        sSort = string.Format("{0} ASC", m_sValue);

                        DataRow[] drSort_2_Array;
                        drSort_2_Array = dt_2.Select(sExpression, sSort);
                        DataTable dt_3 = drSort_2_Array.CopyToDataTable();

                        dt_3.Columns.Add(m_ssubRank, dictColumnTypeMapping[m_ssubRank]);

                        int nSubRank = 1;

                        foreach (DataRow dr in dt_3.Rows)
                        {
                            dr[m_ssubRank] = nSubRank.ToString();
                            nSubRank++;
                        }

                        if (dtOutput == null)
                        {
                            dtOutput = new DataTable();
                            dtOutput = dt_3.Copy();
                        }
                        else
                        {
                            dtOutput.Merge(dt_3);
                        }
                    }
                }
            }

            return dtOutput;
        }

        private void WriteDataTableToCSVFile(DataTable dt, string sFilePath)
        {
            StreamWriter sw = new StreamWriter(sFilePath, false);

            // Header   
            for (int nColumnIndex = 0; nColumnIndex < dt.Columns.Count; nColumnIndex++)
            {
                sw.Write(dt.Columns[nColumnIndex]);

                if (nColumnIndex < dt.Columns.Count - 1)
                    sw.Write(",");
            }

            sw.Write(sw.NewLine);

            foreach (DataRow dr in dt.Rows)
            {
                for (int nColumnIndex = 0; nColumnIndex < dt.Columns.Count; nColumnIndex++)
                {
                    if (Convert.IsDBNull(dr[nColumnIndex]) == false)
                    {
                        string sValue = dr[nColumnIndex].ToString();

                        if (sValue.Contains(',') == true)
                        {
                            sValue = String.Format("\"{0}\"", sValue);
                            sw.Write(sValue);
                        }
                        else
                            sw.Write(dr[nColumnIndex].ToString());
                    }

                    if (nColumnIndex < dt.Columns.Count - 1)
                        sw.Write(",");
                }

                sw.Write(sw.NewLine);
            }

            sw.Close();
        }

        #region Use Microsoft.Office.Interop.Excel
        /*
        private void ExportToExcel(DataTable dt, string sFilePath, int nStartRowIndex, int nStartColumnIndex, bool bHighlightFlag = false, string[] sHighlightColumn_Array = null)
        {
            Excel.Application iApp;
            Excel._Workbook iWorkbook;
            Excel._Worksheet iWorksheet;

            // Start Excel and get Application object
            iApp = new Microsoft.Office.Interop.Excel.Application();
            iApp.Visible = false;
            iApp.StandardFont = "新細明體";
            iApp.StandardFontSize = 11;

            if (File.Exists(sFilePath) == true)
            {
                iWorkbook = iApp.Workbooks.Open(sFilePath);
                iWorksheet = iWorkbook.Worksheets["summary"];
            }
            else
            {
                // Get a new workbook.
                iWorkbook = (Microsoft.Office.Interop.Excel._Workbook)(iApp.Workbooks.Add(""));
                iWorksheet = (Microsoft.Office.Interop.Excel._Worksheet)iWorkbook.ActiveSheet;
                iWorksheet.Name = "summary";
            }

            int nColumnIndex = 0;

            for (nColumnIndex = 1; nColumnIndex <= 18; nColumnIndex++)
                iWorksheet.Columns[nColumnIndex].ColumnWidth = 13;

            // Write the data. Remember that Excel is 1-indexed
            nColumnIndex = nStartColumnIndex + 1;
            int nRowIndex = nStartRowIndex + 1;

            foreach (DataColumn dc in dt.Columns)
            {
                iWorksheet.Cells[nRowIndex, nColumnIndex] = dc.ToString();
                iWorksheet.Cells[nRowIndex, nColumnIndex].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                nColumnIndex++;
            }

            nRowIndex++;

            int nValueStartRowIndex = nRowIndex;

            nColumnIndex = nStartColumnIndex + 1;

            foreach (DataColumn dc in dt.Columns)
            {
                int nMaxIndex = 0;
                int nMinIndex = 0;
                bool bSetHighlightFlag = false;

                int nValueIndex = 0;

                if (bHighlightFlag == true)
                {
                    if (sHighlightColumn_Array.Contains(dc.ToString()) == true)
                    {
                        bSetHighlightFlag = true;

                        double dMaxValue = 0.0;
                        double dMinValue = 0.0;

                        nValueIndex = 0;

                        foreach (DataRow dr in dt.Rows)
                        {
                            double dValue = Convert.ToDouble(dr[dc].ToString());

                            if (nValueIndex == 0)
                                dMaxValue = dMinValue = dValue;
                            else
                            {
                                if (dValue > dMaxValue)
                                {
                                    dMaxValue = dValue;
                                    nMaxIndex = nValueIndex;
                                }

                                if (dValue < dMinValue)
                                {
                                    dMinValue = dValue;
                                    nMinIndex = nValueIndex;
                                }
                            }

                            nValueIndex++;
                        }
                    }
                }

                nRowIndex = nValueStartRowIndex;

                nValueIndex = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    iWorksheet.Cells[nRowIndex, nColumnIndex] = dr[dc];
                    iWorksheet.Cells[nRowIndex, nColumnIndex].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    if (bSetHighlightFlag == true)
                    {
                        if (nMinIndex == nValueIndex)
                        {
                            iWorksheet.Cells[nRowIndex, nColumnIndex].Font.Color = Color.Blue;
                            iWorksheet.Cells[nRowIndex, nColumnIndex].Font.Bold = true;
                        }

                        if (nMaxIndex == nValueIndex)
                        {
                            iWorksheet.Cells[nRowIndex, nColumnIndex].Font.Color = Color.Red;
                            iWorksheet.Cells[nRowIndex, nColumnIndex].Font.Bold = true;
                        }
                    }

                    nRowIndex++;
                    nValueIndex++;
                }

                nColumnIndex++;
            }

            // Save the Excel file
            iApp.Visible = false;
            iApp.UserControl = false;
            iApp.DisplayAlerts = false;
            
            //iWorkbook.SaveAs(sFilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, 
            //                 Type.Missing, Type.Missing, Type.Missing);

            // Exit Excel
            //iWorkbook.Close();
            //iApp.Quit();

            iWorkbook.SaveAs(sFilePath);
            iWorkbook.Close(true);
            iApp.Quit();
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(iApp);
            Marshal.ReleaseComObject(iApp);
        }
        
        private void SetTitleAndColumnFormatToExcel(string sFilePath, string sSKUName, int nStartRowIndex, int nStartColumnIndex)
        {
            Dictionary<int, string> dictColumnMapping = new Dictionary<int, string>()
            {
                { 10, "RxBeaconMax" },
                { 11, "TxBeaconMax" },
                { 13, "RxBHFMax" },
                { 14, "TxBHFMax" },
                { 16, "RxPTHFMax" },
                { 17, "TxPTHFMax" }
            };

            Excel.Application iApp;
            Excel._Workbook iWorkbook;
            Excel._Worksheet iWorksheet;

            // Start Excel and get Application object
            iApp = new Microsoft.Office.Interop.Excel.Application();
            iApp.Visible = false;
            iApp.StandardFont = "新細明體";
            iApp.StandardFontSize = 11;

            if (File.Exists(sFilePath) == true)
            {
                iWorkbook = iApp.Workbooks.Open(sFilePath);
                iWorksheet = iWorkbook.Worksheets["summary"];
            }
            else
            {
                // Get a new workbook.
                iWorkbook = (Microsoft.Office.Interop.Excel._Workbook)(iApp.Workbooks.Add(""));
                iWorksheet = (Microsoft.Office.Interop.Excel._Worksheet)iWorkbook.ActiveSheet;
                iWorksheet.Name = "summary";
            }

            // Write the data. Remember that Excel is 1-indexed
            int nTitleIndex = nStartColumnIndex + 1;
            int nRowIndex = nStartRowIndex + 1;

            iWorksheet.Cells[nRowIndex, nTitleIndex] = sSKUName;
            iWorksheet.Cells[nRowIndex, nTitleIndex].Font.Bold = true;

            nRowIndex++;

            int nColumnIndex = 0;

            foreach (KeyValuePair<int, string> structItem in dictColumnMapping)
            {
                nColumnIndex = structItem.Key + 1;
                string sColumnName = structItem.Value;

                iWorksheet.Cells[nRowIndex, nColumnIndex] = sColumnName;
            }

            string sStartColumn = GetLetterByNumber(nStartColumnIndex + 1);
            string sEndColumn = GetLetterByNumber(nStartColumnIndex + 1 + 9);

            Excel.Range iRange = iWorksheet.get_Range(string.Format("{0}{1}", sStartColumn, nRowIndex), string.Format("{0}{1}", sEndColumn, nRowIndex));
            iRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            for (int nDataIndex = 0; nDataIndex < 10; nDataIndex++)
            {
                nColumnIndex = nDataIndex + nStartColumnIndex + 1;
                iWorksheet.Cells[nRowIndex, nColumnIndex].Font.Bold = true;
            }

            // Save the Excel file
            iApp.Visible = false;
            iApp.UserControl = false;
            iApp.DisplayAlerts = false;
            
            //iWorkbook.SaveAs(sFilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing,
            //                 Type.Missing, Type.Missing, Type.Missing);

            // Exit Excel
            //iWorkbook.Close();
            //iApp.Quit();

            iWorkbook.SaveAs(sFilePath);
            iWorkbook.Close(true);
            iApp.Quit();
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(iApp);
            Marshal.ReleaseComObject(iApp);
        }
        
        private void AddImageToExcel(string sFilePath, List<string> sChartFilePath_List, int nDataIndex)
        {
            Excel.Application iApp;
            Excel._Workbook iWorkbook;
            Excel._Worksheet iWorksheet;

            // Start Excel and get Application object
            iApp = new Microsoft.Office.Interop.Excel.Application();
            iApp.Visible = false;
            iApp.StandardFont = "新細明體";
            iApp.StandardFontSize = 11;

            if (File.Exists(sFilePath) == true)
            {
                iWorkbook = iApp.Workbooks.Open(sFilePath);
                iWorksheet = iWorkbook.Worksheets["summary"];
            }
            else
            {
                // Get a new workbook
                iWorkbook = (Microsoft.Office.Interop.Excel._Workbook)(iApp.Workbooks.Add(""));
                iWorksheet = (Microsoft.Office.Interop.Excel._Worksheet)iWorkbook.ActiveSheet;
                iWorksheet.Name = "summary";
            }
            
            double dHeight = iWorksheet.StandardHeight;
            double dStartTop = nDataIndex * (23 * dHeight);

            for (int nChartIndex = 0; nChartIndex < sChartFilePath_List.Count; nChartIndex++)
            {
                float fTop = (float)(dStartTop + nChartIndex * 120);
                iWorksheet.Shapes.AddPicture(sChartFilePath_List[nChartIndex], Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, fTop, 560, 120);
            }

            // Save the Excel file
            iApp.Visible = false;
            iApp.UserControl = false;
            iApp.DisplayAlerts = false;

            //iWorkbook.SaveAs(sFilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing,
            //                 Type.Missing, Type.Missing, Type.Missing);

            // Exit Excel
            //iWorkbook.Close();
            //iApp.Quit();

            iWorkbook.SaveAs(sFilePath);
            iWorkbook.Close(true);
            iApp.Quit();
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(iApp);
            Marshal.ReleaseComObject(iApp);
        }
        
        private void ExportAllDataToExcel(DataTable dt, string sFilePath)
        {
            Excel.Application iApp;
            Excel._Workbook iWorkbook;
            Excel._Worksheet iWorksheet;

            // Start Excel and get Application object
            iApp = new Microsoft.Office.Interop.Excel.Application();
            iApp.Visible = false;
            iApp.StandardFont = "新細明體";
            iApp.StandardFontSize = 11;

            // Get a new workbook
            iWorkbook = (Microsoft.Office.Interop.Excel._Workbook)(iApp.Workbooks.Add(""));
            iWorksheet = (Microsoft.Office.Interop.Excel._Worksheet)iWorkbook.ActiveSheet;
            iWorksheet.Name = "Sheet1";

            // Write the data. Remember that Excel is 1-indexed
            int nColumnIndex = 1;
            int nRowIndex = 1;

            foreach (DataColumn dc in dt.Columns)
            {
                iWorksheet.Cells[nRowIndex, nColumnIndex] = dc.ToString();
                iWorksheet.Cells[nRowIndex, nColumnIndex].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                iWorksheet.Cells[nRowIndex, nColumnIndex].Font.Bold = true;
                nColumnIndex++;
            }

            string sStartColumn = GetLetterByNumber(1);
            string sEndColumn = GetLetterByNumber(dt.Columns.Count);

            Excel.Range iRange = iWorksheet.get_Range(string.Format("{0}{1}", sStartColumn, nRowIndex), string.Format("{0}{1}", sEndColumn, nRowIndex));
            iRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            nRowIndex++;

            int nValueStartRowIndex = nRowIndex;
            nColumnIndex = 1;

            foreach (DataColumn dc in dt.Columns)
            {
                nRowIndex = nValueStartRowIndex;

                foreach (DataRow dr in dt.Rows)
                {
                    iWorksheet.Cells[nRowIndex, nColumnIndex] = dr[dc];
                    iWorksheet.Cells[nRowIndex, nColumnIndex].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    nRowIndex++;
                }

                nColumnIndex++;
            }

            iWorksheet.Columns.AutoFit();

            // Save the Excel file
            iApp.Visible = false;
            iApp.UserControl = false;
            iApp.DisplayAlerts = false;

            //iWorkbook.SaveAs(sFilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, 
            //                 Type.Missing, Type.Missing, Type.Missing);

            // Exit Excel
            //iWorkbook.Close();
            //iApp.Quit();

            iWorkbook.SaveAs(sFilePath);
            iWorkbook.Close(true);
            iApp.Quit();
        }
        
        private string GetLetterByNumber(int nNumber)
        {
            string[] sLetter_Array = new string[]
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
            };

            if (nNumber > sLetter_Array.Length)
                return "";

            return sLetter_Array[nNumber - 1];
        }
        */
        #endregion
    }
}
