using System;
using System.Collections.Generic;
//using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
//using BlockingQueue;
using System.IO;
using System.Threading;
using System.Reflection;
//using System.Resources;
//using System.Runtime.InteropServices;
//using Microsoft.Win32;
using Elan;
using FingerAutoTuningParameter;
//using System.Net;
//using System.Net.Sockets;
//using System.Drawing.Imaging;
//using System.Runtime.Serialization.Formatters.Binary;

namespace FingerAutoTuning
{
    //Main Step
    public enum MainStep
    {
        FrequencyRank_Phase1 = 1,
        FrequencyRank_Phase2 = 2,
        AC_FrequencyRank = 3,
        Raw_ADC_Sweep = 4,
        Self_FrequencySweep = 5,
        Self_NCPNCNSweep = 6,
        Else = 7
    }

    public partial class frmMain : Form
    {
        private float m_nFormX;//當前窗體的寬度
        private float m_nFormY;//當前窗體的高度
        bool m_bIsLoaded;  // 是否已設定各控制的尺寸資料到Tag屬性

        public bool m_bParentFormFlag = false;

        private Bitmap m_btStartImage;
        private Bitmap m_btStopImage;
        private Bitmap m_btResetImage;
        private Bitmap m_btPatternImage;

        //private bool m_bSocketConnected = false;
        /// <summary>
        /// flag: The thread runing to receive Finger Report from socket.
        /// </summary>
        //private bool m_bRecvSocketFingerReport = false;

        public string m_sParentAPVersion = null;
        public string m_sFileVersion = null;
        public string m_sAPVersion = null;
        public string m_sAPTitle = null;
        public string m_sAPTotalVersion = null;

        private AppCore m_cAppCore = null;
        private InputDevice m_cInputDevice = null;
        private DebugLogAPI m_cDebugLog = null;
        private DataAnalysis m_cDataAnalysis = null;

        private frmFRPH1Chart m_cfrmFRPH1Chart = null;
        private frmFRPH2Chart m_cfrmFRPH2Chart = null;
        private frmACFRChart m_cfrmACFRChart = null;
        private frmSelfFSChart m_cfrmSelfFSChart = null;

        public List<FlowStep> m_cFlowStep_List = new List<FlowStep>();

        private ThreadStart m_tsDebugLog = null;
        private Thread m_tDebugLog = null;

        public static string m_sProgramStartTime;
        public static BlockingQueue.BlockingQueue<string> m_bqsDebugLogBuffer = new BlockingQueue.BlockingQueue<string>();

        public bool m_bExecute = false;
        //private bool m_bMultiClick = false;
        //private bool m_bTPConnected = false;
        public bool m_bReset = true;
        public bool m_bLoadData = false;
        public static bool m_bCollectFlowError = false;
        public static string m_sCollectFlowErrorMessage = "";

        public string m_sSettingFilePath = string.Format(@"{0}\{1}\ini\Setting.dat", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sDefaultFilePath = string.Format(@"{0}\{1}\ini\Default.ini", Application.StartupPath, m_sAPMainDirectoryName);
        public static string m_sGen8FWParameterAddressIniPath = string.Format(@"{0}\{1}\ini\Gen8Addr.ini", Application.StartupPath, m_sAPMainDirectoryName);

        public static byte[] m_byteEDIDData_Array = null;
        public static EDIDInfo m_cEDIDInformation;
        protected DataTable m_datatableScreenSize = new DataTable("ScreenSizeItem");

        private Dictionary<string, string> m_dictEnableCollectItem = new Dictionary<string, string>();
        public static int m_nCurrentStepIndex = 0;
        private Label[] m_lblStepItem_Array;

        public static int m_nPenLogicalXMax = 0;
        public static int m_nPenLogicalYMax = 0;

        //Declare the finger logical X/Y maximum value. For Testing only.
        public static int m_nFingerLogicalXMax = 0;
        public static int m_nFingerLogicalYMax = 0;

        public static int m_nFingerReportId = 0x01;
        public static int m_nPenReportId = 0x07;

        public static int m_nScreenSizeWidth = 0;
        public static int m_nScreenSizeHeight = 0;

        protected int m_nDeviceIndex = -1;

        public const string m_sAPMainDirectoryName = "Finger";
        public string m_sLogDirectoryPath = string.Format(@"{0}\{1}\Log", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sCmdDirectoryPath = string.Format(@"{0}\{1}\Cmd", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sCurrentLogDirectoryPath = string.Format(@"{0}\{1}\Log\Current", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sH5DataLogDirectoryPath = string.Format(@"{0}\{1}\Log\H5Data", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sResultListFilePath = string.Format(@"{0}\{1}\ResultList.txt", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sRecordLogDirectoryName;
        public string m_sRecordLogDirectoryPath = string.Format(@"{0}\{1}\Log", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sH5RecordLogDirectoryName;
        public string m_sH5RecordLogDirectoryPath = "";
        public string m_sItemListFilePath = string.Format(@"{0}\{1}\ItemList.txt", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sConnectInfoFilePath = string.Format(@"{0}\{1}\ConnectInfo.txt", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sDefaultFolderPath = "";
        public string m_sLoadDataPath = "";

        public int m_nCurrentExecuteIndex = 0;
        public AppCoreDefine.RecordState m_eRecordState = AppCoreDefine.RecordState.NORMAL;

        private frmRedmineTask m_cfrmRedmineTask = null;

        public string m_sICSolutionName = "";

        #region constant
        private const int m_nWM_INPUT = 255;
        public const int m_nWorkingFrequency = 32000000;
        #endregion

        public class FlowStep
        {
            public MainStep m_eStep = MainStep.Else;
            public string m_sStepName = "";
            public int m_nCostTime_Hour = -1;
            public int m_nCostTime_Minute = -1;
            public int m_nCostTime_Second = -1;
            public bool m_bLastStep = false;

            public FlowStep()
            {
            }

            public void SetStepName(MainStep eStep, string sStepName, bool bLastStep)
            {
                m_eStep = eStep;
                m_sStepName = sStepName;
                m_bLastStep = bLastStep;
            }

            public void SetCostTimeParameter(int nCostTime_Hour, int nCostTime_Minute, int nCostTime_Second)
            {
                m_nCostTime_Hour = nCostTime_Hour;
                m_nCostTime_Minute = nCostTime_Minute;
                m_nCostTime_Second = nCostTime_Second;
            }
        }

        private void FilterMultiClick()
        {
            Thread.Sleep(500);
            //m_bMultiClick = false;
        }

        /// <summary>
        /// 將控制項的寬，高，左邊距，頂邊距和字體大小暫存到tag屬性中
        /// </summary>
        /// <param name="cons">遞歸控制項中的控制項</param>
        private void SetTag(Control ctrlMainControl)
        {
            foreach (Control ctrlSubControl in ctrlMainControl.Controls)
            {
                ctrlSubControl.Tag = ctrlSubControl.Width + ":" + ctrlSubControl.Height + ":" + ctrlSubControl.Left + ":" + ctrlSubControl.Top + ":" + ctrlSubControl.Font.Size;

                if (ctrlSubControl.Controls.Count > 0)
                    SetTag(ctrlSubControl);
            }
        }

        private void SetControls(float fNewX, float fNewY, Control ctrlMainControl)
        {
            if (m_bIsLoaded)
            {
                //遍歷窗體中的控制項，重新設置控制項的值
                foreach (Control ctrlSubControl in ctrlMainControl.Controls)
                {
                    string[] sTag_Array = ctrlSubControl.Tag.ToString().Split(new char[] { ':' });//獲取控制項的Tag屬性值，並分割後存儲字元串數組
                    float fValue = Convert.ToSingle(sTag_Array[0]) * fNewX;//根據窗體縮放比例確定控制項的值，寬度
                    ctrlSubControl.Width = (int)fValue;//寬度
                    fValue = Convert.ToSingle(sTag_Array[1]) * fNewY;//高度
                    ctrlSubControl.Height = (int)(fValue);
                    fValue = Convert.ToSingle(sTag_Array[2]) * fNewX;//左邊距離
                    ctrlSubControl.Left = (int)(fValue);
                    fValue = Convert.ToSingle(sTag_Array[3]) * fNewY;//上邊緣距離
                    ctrlSubControl.Top = (int)(fValue);
                    Single singleCurrentSize = Convert.ToSingle(sTag_Array[4]) * fNewY;//字體大小
                    ctrlSubControl.Font = new Font(ctrlSubControl.Font.Name, singleCurrentSize, ctrlSubControl.Font.Style, ctrlSubControl.Font.Unit);

                    if (ctrlSubControl.Controls.Count > 0)
                    {
                        SetControls(fNewX, fNewY, ctrlSubControl);
                    }
                }
            }
        }

        public frmMain(string sParentAPVersion = "", string sParentAPTitle = "", bool bParentFormFlag = false)
        {
            InitializeComponent();

            m_sParentAPVersion = sParentAPVersion;

            m_bIsLoaded = false;
            m_bParentFormFlag = bParentFormFlag;

            SetButtonImageSize();

            StartDebugLog();

            WriteDebugLog(string.Format("Action - AP Start, Program Start Time = {0}", m_sProgramStartTime));

            //AP Version
            DateTime dtLastBuild = RetrieveLinkerTimestamp();

            // string sFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
            string path = Assembly.GetExecutingAssembly().Location;
            string sFileVersion = FileVersionInfo.GetVersionInfo(path).ProductVersion; // ← 讀取 InformationalVersion
            m_sFileVersion = sFileVersion;
            string sLastBuild = dtLastBuild.ToString("yyyyMMdd-HHmmss");

            m_sAPVersion = string.Format("{0} ({1})", sFileVersion, sLastBuild);
#if _USE_9F07_SOCKET
            m_sAPTitle = string.Format("Finger AutoTuning_DirectTouch V{0}", m_sAPVersion);
            this.Text = m_sAPTitle;
            m_sAPTotalVersion = string.Format("{0}_Finger AutoTuning_DirectTouch V{1}", sParentAPTitle, sFileVersion);
#else
            m_sAPTitle = string.Format("Finger AutoTuning V{0}", m_sAPVersion);
            this.Text = m_sAPTitle;
            m_sAPTotalVersion = string.Format("{0}_Finger AutoTuning V{1}", sParentAPTitle, sFileVersion);
#endif

            lblStatus.ForeColor = System.Drawing.Color.Blue;
            lblStatus.Text = "Ready";

            lblSecondaryMessage.ForeColor = System.Drawing.Color.Red;
            lblSecondaryMessage.Text = "";

            SetButton(FlowState.Initial);
            InitialstatusstripMessage(0, "Ready");

            this.menustripMain.Renderer = new MyRenderer();

            m_bExecute = false;
            //m_bMultiClick = false;
            //m_bTPConnected = false;
            m_bReset = true;
            m_bLoadData = false;
            m_bCollectFlowError = false;
            m_sCollectFlowErrorMessage = "";

            SetrjtbtnLoadData();

            m_nCurrentStepIndex = 0;
            m_nCurrentExecuteIndex = 0;

            if (File.Exists(m_sSettingFilePath) == false)
                MessageBox.Show("\"Setting.dat\" No Exist!!");
            else
                ParamFingerAutoTuning.LoadParam(m_sSettingFilePath, m_sDefaultFilePath);

            string sGetEDIDFilePath = string.Format(@"{0}\GetEDID.exe", Application.StartupPath);

            if (File.Exists(sGetEDIDFilePath) == true)
            {
                m_byteEDIDData_Array = GetEDID();

                if (m_byteEDIDData_Array == null || m_byteEDIDData_Array.Length == 0)
                {
                    m_nScreenSizeWidth = ParamFingerAutoTuning.m_nScreenSizeWidth;
                    m_nScreenSizeHeight = ParamFingerAutoTuning.m_nScreenSizeHeight;

                    string sMessage = string.Format("Get EDID Information Error. Please Select Screen Size Yourself!!(Width={0}, Height={1})", m_nScreenSizeWidth, m_nScreenSizeHeight);
                    MessageBox.Show(sMessage);
                }
                else
                {
                    m_cEDIDInformation = new EDIDInfo();

                    m_datatableScreenSize.Columns.Add("Name");
                    m_datatableScreenSize.Columns.Add("Width");
                    m_datatableScreenSize.Columns.Add("Height");

                    GetScreenSizeFromEDID();
                }
            }
            else
            {
                string sMessage = "GetEDID.exe File Not Exit!";
                MessageBox.Show(sMessage);
            }

            ParamFingerAutoTuning.SetScreenSize();

            if (Directory.Exists(m_sCurrentLogDirectoryPath) == false)
                Directory.CreateDirectory(m_sCurrentLogDirectoryPath);

            //Screen Resolution
            /*
            String sSize = SystemInformation.PrimaryMonitorSize.ToString();
            String sWidth = SystemInformation.PrimaryMonitorSize.Width.ToString();
            String sHeight = SystemInformation.PrimaryMonitorSize.Height.ToString();
            */
        }

        private void SetButtonImageSize()
        {
            m_btStartImage = btnNewStart.Image as Bitmap;
            Bitmap btStartResize = new Bitmap(m_btStartImage, new Size(40, 40));
            btnNewStart.Image = btStartResize;
            btnNewStart.ImageAlign = ContentAlignment.MiddleCenter;
            btnNewStart.TextImageRelation = TextImageRelation.ImageAboveText;
            btnNewStart.TextAlign = ContentAlignment.MiddleCenter;

            m_btStopImage = btnNewStop.Image as Bitmap;
            Bitmap btStopResize = new Bitmap(m_btStopImage, new Size(40, 40));
            btnNewStop.Image = btStopResize;
            btnNewStop.ImageAlign = ContentAlignment.MiddleCenter;
            btnNewStop.TextImageRelation = TextImageRelation.ImageAboveText;
            btnNewStop.TextAlign = ContentAlignment.MiddleCenter;

            m_btResetImage = btnNewReset.Image as Bitmap;
            Bitmap btResetResize = new Bitmap(m_btResetImage, new Size(40, 40));
            btnNewReset.Image = btResetResize;
            btnNewReset.ImageAlign = ContentAlignment.MiddleCenter;
            btnNewReset.TextImageRelation = TextImageRelation.ImageAboveText;
            btnNewReset.TextAlign = ContentAlignment.MiddleCenter;

            m_btPatternImage = btnNewPattern.Image as Bitmap;
            Bitmap btPatternResize = new Bitmap(m_btPatternImage, new Size(40, 40));
            btnNewPattern.Image = btPatternResize;
            btnNewPattern.ImageAlign = ContentAlignment.MiddleCenter;
            btnNewPattern.TextImageRelation = TextImageRelation.ImageAboveText;
            btnNewPattern.TextAlign = ContentAlignment.MiddleCenter;

            Bitmap btFRPH1ChartImage = btnFRPH1Chart.Image as Bitmap;
            Bitmap btFRPH1ChartResize = new Bitmap(btFRPH1ChartImage, new Size(18, 18));
            btnFRPH1Chart.Image = btFRPH1ChartResize;
            btnFRPH1Chart.ImageAlign = ContentAlignment.MiddleLeft;
            btnFRPH1Chart.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnFRPH1Chart.TextAlign = ContentAlignment.MiddleRight;

            Bitmap btFRPH2ChartImage = btnFRPH2Chart.Image as Bitmap;
            Bitmap btFRPH2ChartResize = new Bitmap(btFRPH2ChartImage, new Size(18, 18));
            btnFRPH2Chart.Image = btFRPH2ChartResize;
            btnFRPH2Chart.ImageAlign = ContentAlignment.MiddleLeft;
            btnFRPH2Chart.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnFRPH2Chart.TextAlign = ContentAlignment.MiddleRight;

            Bitmap btACFRChartImage = btnACFRChart.Image as Bitmap;
            Bitmap btACFRChartResize = new Bitmap(btACFRChartImage, new Size(18, 18));
            btnACFRChart.Image = btFRPH1ChartResize;
            btnACFRChart.ImageAlign = ContentAlignment.MiddleLeft;
            btnACFRChart.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnACFRChart.TextAlign = ContentAlignment.MiddleRight;

            Bitmap btSelfFSChartImage = btnSelfFSChart.Image as Bitmap;
            Bitmap btSelfFSChartResize = new Bitmap(btSelfFSChartImage, new Size(18, 18));
            btnSelfFSChart.Image = btFRPH1ChartResize;
            btnSelfFSChart.ImageAlign = ContentAlignment.MiddleLeft;
            btnSelfFSChart.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnSelfFSChart.TextAlign = ContentAlignment.MiddleRight;
        }

        private void StartDebugLog()
        {
            m_sProgramStartTime = DateTime.Now.ToString("MMdd-HHmmss");

            m_cDebugLog = new DebugLogAPI();
            //Create new Thread for DebugLogFile
            m_tsDebugLog = new ThreadStart(m_cDebugLog.WriteLogToFile);
            m_tDebugLog = new Thread(m_tsDebugLog);
            m_tDebugLog.IsBackground = true;
            m_tDebugLog.Start();
        }

        private void WriteDebugLog(string sMessage)
        {
            m_cDebugLog.WriteLogToBuffer(sMessage);
        }

        // Give the button a transparent background.
        private void MakeButtonTransparent(Button btnButton)
        {
            if (btnButton.BackgroundImage == null)
                return;

            Bitmap bm = (Bitmap)btnButton.BackgroundImage;
            bm.MakeTransparent(bm.GetPixel(0, 0));
        }

        private void MakeButtonFlatType(Button btnButton, bool bFlat = true)
        {
            if (bFlat == true)
            {
                btnButton.FlatStyle = FlatStyle.Flat;
                btnButton.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btnButton.FlatAppearance.BorderSize = 1;
                btnButton.FlatStyle = FlatStyle.Standard;
            }
        }

        private void toolstripmenuitemStep_Click(object sender, EventArgs e)
        {
            frmStepSetting cfrmStepSetting = new frmStepSetting(m_sSettingFilePath);
            //cfrmStepSetting.TopMost = true;

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmStepSetting.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmStepSetting.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmStepSetting.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmStepSetting.Height / 2);
            }

            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmStepSetting.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmStepSetting.Height / 2);
            }

            cfrmStepSetting.StartPosition = FormStartPosition.Manual;
            cfrmStepSetting.Location = new Point(nLocationX, nLocationY);

            if (cfrmStepSetting.ShowDialog() == DialogResult.Cancel)
                return;

            m_bReset = true;
            m_nCurrentStepIndex = 0;
            m_nCurrentExecuteIndex = 0;

            GenerateStepItem(ParamFingerAutoTuning.m_StepSettingParameter_Array);
            GenerateFlowStep();
            GenerateResultTabControl();
        }

        private void toolstripmenuitemTestFrequency_Click(object sender, EventArgs e)
        {
            frmTestFreqSetting cfrmTestFreqSetting = new frmTestFreqSetting(m_cFlowStep_List, this);
            //cfrmTestFreqSetting.TopMost = true;

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmTestFreqSetting.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmTestFreqSetting.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmTestFreqSetting.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmTestFreqSetting.Height / 2);
            }

            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmTestFreqSetting.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmTestFreqSetting.Height / 2);
            }

            cfrmTestFreqSetting.StartPosition = FormStartPosition.Manual;
            cfrmTestFreqSetting.Location = new Point(nLocationX, nLocationY);

            if (cfrmTestFreqSetting.ShowDialog() == DialogResult.Cancel)
            {
                if (m_bReset == true)
                {
                    m_nCurrentStepIndex = 0;
                    m_nCurrentExecuteIndex = 0;
                }

                return;
            }
        }

        private void toolstripmenuitemParameter_Click(object sender, EventArgs e)
        {
            frmParamSetting cfrmParamSetting = new frmParamSetting(this);
            //cfrmParamSetting.TopMost = true;

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmParamSetting.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmParamSetting.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmParamSetting.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmParamSetting.Height / 2);
            }

            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmParamSetting.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmParamSetting.Height / 2);
            }

            cfrmParamSetting.StartPosition = FormStartPosition.Manual;
            cfrmParamSetting.Location = new Point(nLocationX, nLocationY);

            if (cfrmParamSetting.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void toolstripmenuitemMultiAnalysis_Click(object sender, EventArgs e)
        {
            frmMultiAnalysis cfrmMultiAnalysis = new frmMultiAnalysis(this);
            //cfrmMultiAnalysis.TopMost = true;

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmMultiAnalysis.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmMultiAnalysis.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmMultiAnalysis.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmMultiAnalysis.Height / 2);
            }

            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmMultiAnalysis.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmMultiAnalysis.Height / 2);
            }

            cfrmMultiAnalysis.StartPosition = FormStartPosition.Manual;
            cfrmMultiAnalysis.Location = new Point(nLocationX, nLocationY);

            if (cfrmMultiAnalysis.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void toolstripmenuitemGetIPAddress_Click(object sender, EventArgs e)
        {
            frmGetIPAddress cfrmGetIPAddress = new frmGetIPAddress();
            //cfrmGetIPAddress.TopMost = true;

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmGetIPAddress.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmGetIPAddress.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmGetIPAddress.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmGetIPAddress.Height / 2);
            }

            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmGetIPAddress.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmGetIPAddress.Height / 2);
            }

            cfrmGetIPAddress.StartPosition = FormStartPosition.Manual;
            cfrmGetIPAddress.Location = new Point(nLocationX, nLocationY);
            cfrmGetIPAddress.GetIPAddress();

            if (cfrmGetIPAddress.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void toolstripmenuitemFeedback_Click(object sender, EventArgs e)
        {
            frmFeedback cfrmFeedback = new frmFeedback();
            //cfrmFeedback.TopMost = true;

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmFeedback.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmFeedback.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmFeedback.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmFeedback.Height / 2);
            }

            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmFeedback.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmFeedback.Height / 2);
            }

            cfrmFeedback.StartPosition = FormStartPosition.Manual;
            cfrmFeedback.Location = new Point(nLocationX, nLocationY);

            if (cfrmFeedback.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void GenerateStepItem(ParamFingerAutoTuning.StepSettingParameter[] cStepItem_Array)
        {
            if (m_lblStepItem_Array != null)
            {
                foreach (Label lblLabel in m_lblStepItem_Array)
                    this.pnlStep.Controls.Remove(lblLabel);

                m_lblStepItem_Array = null;
            }

            int nGap = 1;
            int nYStartPosition = lblStepItem.Top;
            int nIndex = 0;

            m_lblStepItem_Array = new Label[cStepItem_Array.Length];

            for (int nStepIndex = 0; nStepIndex < cStepItem_Array.Length; nStepIndex++)
            {
                if (cStepItem_Array[nStepIndex].m_bEnable == true)
                {
                    m_lblStepItem_Array[nIndex] = new Label();
                    Label lblLabel = m_lblStepItem_Array[nIndex];
                    lblLabel.Name = cStepItem_Array[nStepIndex].m_sStepName;
                    lblLabel.Text = cStepItem_Array[nStepIndex].m_sStepName;

                    lblLabel.Left = 5;
                    lblLabel.Top = nIndex * (this.lblStepItem.Height + nGap) + nYStartPosition;
                    lblLabel.BorderStyle = BorderStyle.Fixed3D;
                    lblLabel.AutoSize = false;
                    lblLabel.Width = this.lblStepItem.Width;
                    lblLabel.Height = this.lblStepItem.Height;
                    lblLabel.TextAlign = ContentAlignment.MiddleCenter;
                    lblLabel.Tag = cStepItem_Array[nStepIndex].m_Step;
                    lblLabel.BackColor = SystemColors.Control;
                    //lblLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
                    lblLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
                    lblLabel.Font = new Font("Times New Roman", 10);
                    this.pnlStep.Controls.Add(lblLabel);
                    lblLabel.Show();

                    nIndex++;
                }
            }
        }

        private void GenerateFlowStep()
        {
            m_cFlowStep_List.Clear();

            for (int nIndex = 0; nIndex < ParamFingerAutoTuning.m_StepSettingParameter_Array.Length; nIndex++)
            {
                if (ParamFingerAutoTuning.m_StepSettingParameter_Array[nIndex].m_bEnable == true)
                {
                    FlowStep cFlowStep = new FlowStep();
                    cFlowStep.SetStepName(ParamFingerAutoTuning.m_StepSettingParameter_Array[nIndex].m_Step, 
                                          ParamFingerAutoTuning.m_StepSettingParameter_Array[nIndex].m_sStepName, 
                                          false);
                    m_cFlowStep_List.Add(cFlowStep);
                }
            }

            int nFlowStepCount = m_cFlowStep_List.Count;

            if (nFlowStepCount > 0)
                m_cFlowStep_List[nFlowStepCount - 1].m_bLastStep = true;
        }

        private void GenerateResultTabControl()
        {
            TabPage[] tpgResult_Array = new TabPage[] 
            {
                tpgFRPH1,
                tpgFRPH2,
                tpgACFR,
                tpgRawADCS,
                tpgSelfFS,
                tpgSelfPNS
            };

            for (int nTabPageIndex = 0; nTabPageIndex < tpgResult_Array.Length; nTabPageIndex++)
                tpgResult_Array[nTabPageIndex].Parent = null;

            for (int nStepIndex = 0; nStepIndex < m_cFlowStep_List.Count; nStepIndex++)
            {
                for (int nSetIndex = 0; nSetIndex < ParamFingerAutoTuning.m_eStepSet_Array.Length; nSetIndex++)
                {
                    if (m_cFlowStep_List[nStepIndex].m_eStep == ParamFingerAutoTuning.m_eStepSet_Array[nSetIndex])
                    {
                        tpgResult_Array[nSetIndex].Parent = tctrlResult;
                        InitialResultTabPageContent(tpgResult_Array[nSetIndex]);
                    }
                }
            }
        }

        private void ClearResultTabPageContent()
        {
            TabPage[] tpgResult_Array = new TabPage[] 
            {
                tpgFRPH1,
                tpgFRPH2,
                tpgACFR,
                tpgRawADCS,
                tpgSelfFS,
                tpgSelfPNS
            };

            for (int nStepIndex = 0; nStepIndex < m_cFlowStep_List.Count; nStepIndex++)
            {
                for (int nSetIndex = 0; nSetIndex < ParamFingerAutoTuning.m_eStepSet_Array.Length; nSetIndex++)
                {
                    if (m_cFlowStep_List[nStepIndex].m_eStep == ParamFingerAutoTuning.m_eStepSet_Array[nSetIndex])
                        InitialResultTabPageContent(tpgResult_Array[nSetIndex]);
                }
            }
        }

        private void InitialResultTabPageContent(TabPage tpgResult)
        {
            TabPage[] tpgResult_Array = new TabPage[] 
            {
                tpgFRPH1,
                tpgFRPH2,
                tpgACFR,
                tpgRawADCS,
                tpgSelfFS,
                tpgSelfPNS
            };

            int nIndexCount = 0;

            for (int nTabPageIndex = 0; nTabPageIndex < tpgResult_Array.Length; nTabPageIndex++)
            {
                if (tpgResult == tpgResult_Array[nTabPageIndex])
                {
                    nIndexCount = nTabPageIndex;
                    break;
                }
            }

            switch (tpgResult_Array[nIndexCount].Name)
            {
#if _USE_VC2010
                case "tpgFRPH1":
                    rtbxFRPH1.Clear();
                    btnFRPH1Chart.Enabled = false;
                    break;
                case "tpgFRPH2":
                    dgvFRPH2.MakeDoubleBuffered(true);
                    InitialDataGridView(dgvFRPH2);
                    btnFRPH2Chart.Enabled = false;
                    break;
                case "tpgRawADCS":
                    dgvRawADCS.MakeDoubleBuffered(true);
                    InitialDataGridView(dgvRawADCS);
                    break;
                case "tpgACFR":
                    dgvACFR.MakeDoubleBuffered(true);
                    InitialDataGridView(dgvACFR);
                    btnACFRChart.Enabled = false;
                    break;
                case "tpgSelfFS":
                    btnSelfFSChart.Enabled = false;
                    break;
                case "tpsSelfPNS":
                    rtbxSelfPNS.Clear();
                    break;
#else
                case nameof(tpgFRPH1):
                    rtbxFRPH1.Clear();
                    btnFRPH1Chart.Enabled = false;
                    break;
                case nameof(tpgFRPH2):
                    dgvFRPH2.MakeDoubleBuffered(true);
                    InitialDataGridView(dgvFRPH2);
                    btnFRPH2Chart.Enabled = false;
                    break;
                case nameof(tpgRawADCS):
                    dgvRawADCS.MakeDoubleBuffered(true);
                    InitialDataGridView(dgvRawADCS);
                    break;
                case nameof(tpgACFR):
                    dgvACFR.MakeDoubleBuffered(true);
                    InitialDataGridView(dgvACFR);
                    btnACFRChart.Enabled = false;
                    break;
                case nameof(tpgSelfFS):
                    btnSelfFSChart.Enabled = false;
                    break;
                case nameof(tpgSelfPNS):
                    rtbxSelfPNS.Clear();
                    break;
#endif
                default:
                    break;
            }
        }

        private void InitialDataGridView(DataGridView dgvDataGridView)
        {
            while (dgvDataGridView.Rows.Count != 0)
                dgvDataGridView.Rows.RemoveAt(0);

            dgvDataGridView.Columns.Clear();
        }

        /// <summary>
        /// Get the Last Build Date
        /// </summary>
        /// <returns></returns>
        private DateTime RetrieveLinkerTimestamp()
        {
            string sFilePath = Assembly.GetCallingAssembly().Location;
            const int nPeHeaderOffset = 60;
            const int nLinkerTimestampOffset = 8;
            byte[] byteValue_Array = new byte[2048];
            Stream sStream = null;

            try
            {
                sStream = new FileStream(sFilePath, FileMode.Open, FileAccess.Read);
                sStream.Read(byteValue_Array, 0, 2048);
            }
            finally
            {
                if (sStream != null)
                {
                    sStream.Close();
                }
            }

            int nValue = System.BitConverter.ToInt32(byteValue_Array, nPeHeaderOffset);
            int nSecondsSince1970 = System.BitConverter.ToInt32(byteValue_Array, nValue + nLinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(nSecondsSince1970);
            dt = dt.AddHours(8);

            return dt;
        }

        private byte[] GetEDID()
        {
            int nErrorCode = 0;
            Process cGetEDIDProcess = new Process();
            ProcessStartInfo cGetEDIDProcessStartInfo = new ProcessStartInfo();
            cGetEDIDProcess.StartInfo.FileName = string.Format(@"{0}\GetEDID.exe", Application.StartupPath);
            cGetEDIDProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                cGetEDIDProcess.Start();
                cGetEDIDProcess.WaitForExit();
                nErrorCode = cGetEDIDProcess.ExitCode;
            }
            catch
            {
                nErrorCode = -999;
                return null;
            }

            //Open the edid data
            String sEDIDName = string.Format(@"{0}\EDID.txt", Application.StartupPath);

            if (File.Exists(sEDIDName) == false)
                return null;

            FileStream fsEDIDFile = File.Open(sEDIDName, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] byteReadData_Array = new byte[fsEDIDFile.Length];
            fsEDIDFile.Read(byteReadData_Array, 0, (int)fsEDIDFile.Length);

            return byteReadData_Array;
        }

        private void GetScreenSizeFromEDID()
        {
            /*
            m_ScreenSizeTable.ReadXml(ScreenListXMLPath);
            DataRow[] drRow_Array = m_ScreenSizeTable.Select("Name = 'AutoSet'");

            bool bAutoSet = false;
            for (int nIndex = 0; nIndex < drRow_Array.Length; nIndex++)
            {
                int RowIndex = m_ScreenSizeTable.Rows.IndexOf(drRow_Array[nIndex]);

                if (ParamPenDataCollect.m_nScreenIndex == RowIndex)
                    bAutoSet = true;
            }
            */

            string sMessage = "Get EDID :";

            for (int nIndex = 0; nIndex < m_byteEDIDData_Array.Length; nIndex++)
                sMessage += string.Format(" {0}", m_byteEDIDData_Array[nIndex].ToString("X2"));

            WriteDebugLog(sMessage);
            WriteDebugLog(string.Format("EDID Length : {0}", m_byteEDIDData_Array.Length));

            if (m_byteEDIDData_Array != null && m_byteEDIDData_Array.Length != 0)
            {
                byte byteWidth_H = (byte)((m_byteEDIDData_Array[68] & 0xF0) >> 4);
                byte byteWidth_L = m_byteEDIDData_Array[66];

                byte byteHeight_H = (byte)(m_byteEDIDData_Array[68] & 0x0F);
                byte byteHeight_L = m_byteEDIDData_Array[67];

                int nEDIDWidth = (ushort)(byteWidth_H << 8 | byteWidth_L);
                int nEDIDHeight = (ushort)(byteHeight_H << 8 | byteHeight_L);
                float Length = (float)((Math.Sqrt(nEDIDWidth * nEDIDWidth + nEDIDHeight * nEDIDHeight)) / 10 / 2.54);
                m_cEDIDInformation.m_nWidth = nEDIDWidth;
                m_cEDIDInformation.m_nHeight = nEDIDHeight;
                m_nScreenSizeWidth = nEDIDWidth;
                m_nScreenSizeHeight = nEDIDHeight;
                m_cEDIDInformation.m_dScreenSize = ElanConvert.DoubleRoundUp(Length, 1);

                int nXResolution_H = (byte)((m_byteEDIDData_Array[58] & 0xF0) >> 4);
                int nXResolution_L = m_byteEDIDData_Array[56];

                int nYResolution_H = (byte)((m_byteEDIDData_Array[61] & 0xF0) >> 4);
                int nYResolution_L = m_byteEDIDData_Array[59];

                int nEDIDXResolution = (ushort)(nXResolution_H << 8 | nXResolution_L);
                int nEDIDYResolution = (ushort)(nYResolution_H << 8 | nYResolution_L);
                m_cEDIDInformation.m_nXResolution = nEDIDXResolution;
                m_cEDIDInformation.m_nYResolution = nEDIDYResolution;

                /*
                bool bAutoSetItemMatch = false;
                for (int nIndex = 0; nIndex < drRow_Array.Length; nIndex++)
                {
                    int RowIndex = m_ScreenSizeTable.Rows.IndexOf(drRow_Array[nIndex]);

                    if (ParamPenDataCollect.m_nScreenIndex == RowIndex)
                    {
                        ParamPenDataCollect.m_fScreenWidth = EDIDInformation.EDIDInfo_Width;
                        ParamPenDataCollect.m_fScreenHeight = EDIDInformation.EDIDInfo_Height;
                        m_APsetting.m_fScreenX = EDIDInformation.EDIDInfo_Width;
                        m_APsetting.m_fScreenY = EDIDInformation.EDIDInfo_Height;
                        sScreenSize = string.Format("{0}\"", EDIDInformation.ScreenSize);
                        bAutoSetItemMatch = true;
                    }
                }

                if (bAutoSetItemMatch == false)
                    sScreenSize = Convert.ToString(m_ScreenSizeTable.Rows[ParamPenDataCollect.m_nScreenIndex]["Name"]);
                */
            }
            else
            {
                /*
                if (bAutoSet == true)
                {
                    ParamPenDataCollect.m_nScreenIndex = 0;
                    ParamPenDataCollect.m_fScreenWidth = Convert.ToDouble(m_ScreenSizeTable.Rows[0]["Width"]);
                    ParamPenDataCollect.m_fScreenHeight = Convert.ToDouble(m_ScreenSizeTable.Rows[0]["Height"]);
                    sScreenSize = Convert.ToString(m_ScreenSizeTable.Rows[0]["Name"]);
                }
                */
            }
        }

        private void MessageRtbx_TextChanged(object sender, EventArgs e)
        {
            int nMAXLINELENGTH = 10000; //20000;
            int nLineCount = rtbxMessage.Lines.Length;

            if (nLineCount > nMAXLINELENGTH)
            {
                /*
                string[] sLine_Array = rtbxMessage.Lines;
                string[] sNewLine_Array = new string[sLine_Array.Length - 1];

                Array.Copy(sLine_Array, 1, sNewLine_Array, 0, sNewLine_Array.Length);

                rtbxMessage.Lines = sNewLine_Array;
                */

                rtbxMessage.ReadOnly = false;
                rtbxMessage.Select(0, rtbxMessage.GetFirstCharIndexFromLine(1));
                rtbxMessage.SelectedText = string.Empty;
                rtbxMessage.ReadOnly = true;
            }

            rtbxMessage.SelectionStart = rtbxMessage.TextLength;

            //Scrolls the contents of the control to the current caret position.
            rtbxMessage.ScrollToCaret();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteDebugLog("Close Tool");

            if (m_cAppCore != null)
                m_cAppCore.DisconnectElanSSHClient();

            m_tDebugLog.Abort();
            m_tDebugLog.Join();
            m_cDebugLog.CloseLogFile();

#if !_USE_9F07_SOCKET
            for (int nCountIndex = 0; nCountIndex < 2; nCountIndex++)
            {
                Process[] cADBProcess_Array = Process.GetProcessesByName("adb");

                if (cADBProcess_Array.Length == 0)
                    break;

                foreach (Process cProcess in cADBProcess_Array)
                {
                    //cProcess.Dispose();
                    //cProcess.Close();
                    //cProcess.WaitForExit();

                    try
                    {
                        cProcess.CloseMainWindow();
                        cProcess.Kill();
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
#endif
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.IsMdiChild == false)
                Environment.Exit(Environment.ExitCode);
        }

        private void btnFRPH1Chart_Click(object sender, EventArgs e)
        {
            string sStepDirectoryName = StringConvert.m_dictMainStepCodeNameMappingTable[MainStep.FrequencyRank_Phase1];

            string sChartFilePath = string.Format(@"{0}\{1}\{2}\UniformityChart.jpg", m_sRecordLogDirectoryPath, sStepDirectoryName, MainConstantParameter.m_sDATATYPE_CHART);

            if (File.Exists(sChartFilePath) == false)
            {
                MessageBox.Show("Chart File Not Exist!!");
                return;
            }

            if (m_cfrmFRPH1Chart != null)
            {
                m_cfrmFRPH1Chart.Close();
                m_cfrmFRPH1Chart = null;
            }

            m_cfrmFRPH1Chart = new frmFRPH1Chart(sChartFilePath);

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(m_cfrmFRPH1Chart.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(m_cfrmFRPH1Chart.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(m_cfrmFRPH1Chart.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(m_cfrmFRPH1Chart.Height / 2);
            }

            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(m_cfrmFRPH1Chart.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(m_cfrmFRPH1Chart.Height / 2);
            }

            //獲取主屏幕的大小
            int nScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int nScreenHeight = Screen.PrimaryScreen.Bounds.Height;

            if (nScreenWidth < m_cfrmFRPH1Chart.Width)
            {
                m_cfrmFRPH1Chart.Width = nScreenWidth;
                nLocationX = 0;
            }

            if (nScreenHeight < m_cfrmFRPH1Chart.Height)
            {
                m_cfrmFRPH1Chart.Height = nScreenHeight;
                nLocationY = 0;
            }

            m_cfrmFRPH1Chart.StartPosition = FormStartPosition.Manual;
            m_cfrmFRPH1Chart.Location = new Point(nLocationX, nLocationY);

            m_cfrmFRPH1Chart.Show();
        }

        private void btnFRPH2Chart_Click(object sender, EventArgs e)
        {
            string sStepDirectoryName = StringConvert.m_dictMainStepCodeNameMappingTable[MainStep.FrequencyRank_Phase2];

            string sChartFilePath = string.Format(@"{0}\{1}\{2}\RefValueChart.jpg", m_sRecordLogDirectoryPath, sStepDirectoryName, MainConstantParameter.m_sDATATYPE_CHART);

            if (File.Exists(sChartFilePath) == false)
            {
                MessageBox.Show("Chart File Not Exist!!");
                return;
            }

            if (m_cfrmFRPH2Chart != null)
            {
                m_cfrmFRPH2Chart.Close();
                m_cfrmFRPH2Chart = null;
            }

            m_cfrmFRPH2Chart = new frmFRPH2Chart(sChartFilePath);

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(m_cfrmFRPH2Chart.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(m_cfrmFRPH2Chart.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(m_cfrmFRPH2Chart.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(m_cfrmFRPH2Chart.Height / 2);
            }

            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(m_cfrmFRPH2Chart.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(m_cfrmFRPH2Chart.Height / 2);
            }

            //獲取主屏幕的大小
            int nScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int nScreenHeight = Screen.PrimaryScreen.Bounds.Height;

            if (nScreenWidth < m_cfrmFRPH2Chart.Width)
            {
                m_cfrmFRPH2Chart.Width = nScreenWidth;
                nLocationX = 0;
            }

            if (nScreenHeight < m_cfrmFRPH2Chart.Height)
            {
                m_cfrmFRPH2Chart.Height = nScreenHeight;
                nLocationY = 0;
            }

            m_cfrmFRPH2Chart.StartPosition = FormStartPosition.Manual;
            m_cfrmFRPH2Chart.Location = new Point(nLocationX, nLocationY);

            m_cfrmFRPH2Chart.Show();
        }

        private void btnACFRChart_Click(object sender, EventArgs e)
        {
            string sStepDirectoryName = StringConvert.m_dictMainStepCodeNameMappingTable[MainStep.AC_FrequencyRank];

            string sChartFilePath = string.Format(@"{0}\{1}\{2}\RefValueChart.jpg", m_sRecordLogDirectoryPath, sStepDirectoryName, MainConstantParameter.m_sDATATYPE_CHART);

            if (File.Exists(sChartFilePath) == false)
            {
                MessageBox.Show("Chart File Not Exist!!");
                return;
            }

            if (m_cfrmACFRChart != null)
            {
                m_cfrmACFRChart.Close();
                m_cfrmACFRChart = null;
            }

            m_cfrmACFRChart = new frmACFRChart(sChartFilePath);

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(m_cfrmACFRChart.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(m_cfrmACFRChart.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(m_cfrmACFRChart.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(m_cfrmACFRChart.Height / 2);
            }

            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(m_cfrmACFRChart.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(m_cfrmACFRChart.Height / 2);
            }

            //獲取主屏幕的大小
            int nScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int nScreenHeight = Screen.PrimaryScreen.Bounds.Height;

            if (nScreenWidth < m_cfrmACFRChart.Width)
            {
                m_cfrmACFRChart.Width = nScreenWidth;
                nLocationX = 0;
            }

            if (nScreenHeight < m_cfrmACFRChart.Height)
            {
                m_cfrmACFRChart.Height = nScreenHeight;
                nLocationY = 0;
            }

            m_cfrmACFRChart.StartPosition = FormStartPosition.Manual;
            m_cfrmACFRChart.Location = new Point(nLocationX, nLocationY);

            m_cfrmACFRChart.Show();
        }

        private void btnSelfFSChart_Click(object sender, EventArgs e)
        {
            string sStepDirectoryName = StringConvert.m_dictMainStepCodeNameMappingTable[MainStep.Self_FrequencySweep];

            string sChartDirectoryPath = string.Format(@"{0}\{1}\{2}", m_sRecordLogDirectoryPath, sStepDirectoryName, MainConstantParameter.m_sDATATYPE_CHART);

            if (Directory.Exists(sChartDirectoryPath) == false)
            {
                MessageBox.Show("Chart Folder Not Exist!!");
                return;
            }

            if (m_cfrmSelfFSChart != null)
            {
                m_cfrmSelfFSChart.Close();
                m_cfrmSelfFSChart = null;
            }

            m_cfrmSelfFSChart = new frmSelfFSChart(sChartDirectoryPath);

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(m_cfrmSelfFSChart.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(m_cfrmSelfFSChart.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(m_cfrmSelfFSChart.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(m_cfrmSelfFSChart.Height / 2);
            }

            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(m_cfrmSelfFSChart.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(m_cfrmSelfFSChart.Height / 2);
            }

            //獲取主屏幕的大小
            int nScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int nScreenHeight = Screen.PrimaryScreen.Bounds.Height;

            if (nScreenWidth < m_cfrmSelfFSChart.Width)
            {
                m_cfrmSelfFSChart.Width = nScreenWidth;
                nLocationX = 0;
            }

            if (nScreenHeight < m_cfrmSelfFSChart.Height)
            {
                m_cfrmSelfFSChart.Height = nScreenHeight;
                nLocationY = 0;
            }

            m_cfrmSelfFSChart.StartPosition = FormStartPosition.Manual;
            m_cfrmSelfFSChart.Location = new Point(nLocationX, nLocationY);

            m_cfrmSelfFSChart.Show();
        }

        private void lblErrorMessage_Resize(object sender, EventArgs e)
        {
            if (this.IsMdiChild == true)
            {
                if (this.MdiParent.WindowState != FormWindowState.Minimized)
                {
                    string sText = lblSecondaryMessage.Text;
                    ResizelblErrorMessageFont(sText);
                    lblSecondaryMessage.Text = sText;
                }
            }
            else
            {
                if (this.WindowState != FormWindowState.Minimized)
                {
                    string sText = lblSecondaryMessage.Text;
                    ResizelblErrorMessageFont(sText);
                    lblSecondaryMessage.Text = sText;
                }
            }
        }

        private void dgvFRPH2_SizeChanged(object sender, EventArgs e)
        {
            /*
            int nLastColumn = dgvFRPH2.Columns.Count - 1;

            for (int nIndex = 0; nIndex < dgvFRPH2.Columns.Count; nIndex++)
            {
                if (nLastColumn > 0)
                    dgvFRPH2.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                else
                    dgvFRPH2.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            for (int nIndex = 0; nIndex < dgvFRPH2.Columns.Count; nIndex++)
            {
                int nColumnWidth = dgvFRPH2.Columns[nIndex].Width;
                dgvFRPH2.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvFRPH2.Columns[nIndex].Width = nColumnWidth;
            }

            objDGVHelper.SetDetailDGVLocation();
            */

            dgvFRPH2.AutoResizeColumns();
        }

        private void dgvFRPH2_Scroll(object sender, ScrollEventArgs e)
        {
            //objDGVHelper.SetDetailDGVLocation();
        }

        private void dgvFRPH2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (ParamFingerAutoTuning.m_nFRPH2NormalizeType == 1)
            {
                dgvFRPH2.Columns["ColorGradient"].Width = 15;
                dgvFRPH2.Columns["ColorGradient"].HeaderText = "";
            }

            DataGridViewRow dgvrDgvRow = dgvFRPH2.Rows[e.RowIndex];// get you required index
            // check the cell value under your specific column and then you can toggle your colors

            if (e.RowIndex < ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber)
                dgvrDgvRow.DefaultCellStyle.BackColor = Color.LightPink;

            if (ParamFingerAutoTuning.m_nFRPH2NormalizeType == 1)
            {
                double dPercentageValue = Convert.ToDouble(dgvrDgvRow.Cells["Reference Value"].Value);

                double dRatio = dPercentageValue / 100.0;

                if (dRatio < 0.0)
                    dRatio = 0.0;

                int nRedValue = 0;
                int nGreenValue = 0;
                int nBlueValue = 0;

                if (dRatio >= 0.5)
                {
                    nRedValue = (int)Math.Round(((-255 * (dRatio - 0.5)) / 0.5) + 255, 0, MidpointRounding.AwayFromZero);
                    nGreenValue = (int)Math.Round(((-127 * (dRatio - 0.5)) / 0.5) + 255, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    nRedValue = 255;
                    nGreenValue = (int)Math.Round((255 * dRatio) / 0.5, 0, MidpointRounding.AwayFromZero);
                }

                dgvrDgvRow.Cells["ColorGradient"].Style.BackColor = Color.FromArgb(nRedValue, nGreenValue, nBlueValue);
            }
        }

        private void dgvFRPH2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            /*
            if (e.RowIndex < 0)
                return;

            if (ParamFingerAutoTuning.m_nFRPH2NormalizeType == 1)
            {
                DataGridViewRow DgvRow = FRPH2Dgv.Rows[e.RowIndex];

                int nColumnIndex = DgvRow.Cells["Reference Value"].ColumnIndex;

                if (e.ColumnIndex == nColumnIndex && e.RowIndex != -1)
                {
                    using (Brush borderBrush = new SolidBrush(Color.Black))
                    {
                        using (Pen borderPen = new Pen(borderBrush, 2))
                        {
                            Rectangle rectDimensions = e.CellBounds;
                            rectDimensions.Width -= 2;
                            rectDimensions.Height -= 2;
                            rectDimensions.X = rectDimensions.Left + 1;
                            rectDimensions.Y = rectDimensions.Top + 1;

                            e.Graphics.DrawRectangle(borderPen, rectDimensions);

                            e.Handled = true;
                        }
                    }
                }
            }
            */
        }

        private void dgvACFR_SizeChanged(object sender, EventArgs e)
        {
            if (dgvACFR.Width > ndgvACFRTotalWidth)
            {
                for (int nColumnIndex = 0; nColumnIndex < dgvACFR.Columns.Count; nColumnIndex++)
                {
                    dgvACFR.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                dgvACFR.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                for (int nColumnIndex = 0; nColumnIndex < dgvACFR.Columns.Count; nColumnIndex++)
                {
                    dgvACFR.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    int nWidth = dgvACFR.Columns[nColumnIndex].Width;
                    dgvACFR.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvACFR.Columns[nColumnIndex].Width = nWidth;
                }
            }

            dgvACFR.AutoResizeColumns();
        }

        private void dgvACFR_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            double dMaxValue = 0.0;

            for (int nIndex = 0; nIndex < dgvACFR.Rows.Count; nIndex++)
            {
                double dValue = Convert.ToDouble(dgvACFR.Rows[nIndex].Cells["Composite SNR(dB)"].Value);

                if (nIndex == 0)
                    dMaxValue = dValue;
                else if (dValue > dMaxValue)
                    dMaxValue = dValue;

                double dACSNRValue = Convert.ToDouble(dgvACFR.Rows[nIndex].Cells["AC SNR(dB)"].Value);

                if (dACSNRValue < 0)
                    dgvACFR.Rows[nIndex].Cells["AC SNR(dB)"].Style.BackColor = Color.Red;

                double dLCMSNRValue = Convert.ToDouble(dgvACFR.Rows[nIndex].Cells["LCM SNR(dB)"].Value);

                if (dLCMSNRValue < 0)
                    dgvACFR.Rows[nIndex].Cells["LCM SNR(dB)"].Style.BackColor = Color.Red;
            }

            dgvACFR.Columns["ColorGradient"].Width = 15;
            dgvACFR.Columns["ColorGradient"].HeaderText = "";

            DataGridViewRow dgvrDgvRow = dgvACFR.Rows[e.RowIndex];// get you required index
            // check the cell value under your specific column and then you can toggle your colors

            if (e.RowIndex < ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber)
                dgvrDgvRow.DefaultCellStyle.BackColor = Color.LightPink;

            double dSNRValue = Convert.ToDouble(dgvrDgvRow.Cells["Composite SNR(dB)"].Value);

            double dRatio = dSNRValue / dMaxValue;

            int nRedValue = 0;
            int nGreenValue = 0;
            int nBlueValue = 0;

            if (dSNRValue < 0)
            {
                nRedValue = 0;
                nGreenValue = 255;
                nBlueValue = 255;
                dgvrDgvRow.Cells["Composite SNR(dB)"].Style.BackColor = Color.Red;
            }
            else if (dRatio >= 0.5)
            {
                nRedValue = (int)Math.Round(((-255 * (dRatio - 0.5)) / 0.5) + 255, 0, MidpointRounding.AwayFromZero);
                nGreenValue = (int)Math.Round(((-127 * (dRatio - 0.5)) / 0.5) + 255, 0, MidpointRounding.AwayFromZero);
            }
            else
            {
                nRedValue = 255;
                nGreenValue = (int)Math.Round((255 * dRatio) / 0.5, 0, MidpointRounding.AwayFromZero);
            }

            dgvrDgvRow.Cells["ColorGradient"].Style.BackColor = Color.FromArgb(nRedValue, nGreenValue, nBlueValue);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            /*
            //Check the folder exist or not.
            btnStartSocket.Enabled = false;

            ConnectSocket();
            */

            m_nFormX = this.Width;//獲取窗體的寬度
            m_nFormY = this.Height;//獲取窗體的高度
            m_bIsLoaded = true;// 已設定各控制項的尺寸到Tag屬性中
            SetTag(this);//調用方法

            GenerateStepItem(ParamFingerAutoTuning.m_StepSettingParameter_Array);
            GenerateFlowStep();
            GenerateResultTabControl();

            rjtbtnLoadData.Location = new Point(lblLoadData.Right, lblLoadData.Top - ((rjtbtnLoadData.Height - lblLoadData.Height) / 2));
        }

        //HID event
        protected override void WndProc(ref Message m)
        {
            if (m_cInputDevice != null)
            {
                switch (m.Msg)
                {
                    case m_nWM_INPUT:
                        m_cInputDevice.ProcessMessage(m);
                        break;
                    default:
                        break;
                }
            }

            base.WndProc(ref m);
        }

        private void rjtbtnLoadData_CheckedChanged(object sender, EventArgs e)
        {
            int nCheckValue = (rjtbtnLoadData.Checked == true) ? 1 : 0;

            IniFileFormat.WriteValue("Main Window Setting", "rjtbtnLoadData", nCheckValue.ToString(), m_sItemListFilePath, true, false);
        }

        private void SetrjtbtnLoadData()
        {
            string sLoadData = IniFileFormat.ReadValue("Main Window Setting", "rjtbtnLoadData", m_sItemListFilePath, "0");

            rjtbtnLoadData.Checked = (sLoadData == "1") ? true : false;
        }

        private void btnNewStart_Click(object sender, EventArgs e)
        {
            WriteDebugLog("Action - Start Button(Start Finger AutoTuning Flow)");

            SetButton(FlowState.Start);
            rtbxMessage.Clear();
            InitialstatusstripMessage(0, "Start");

            //if (cbxLoadData.Checked == true || m_bReset == true)
            if (rjtbtnLoadData.Checked == true || m_bReset == true)
            {
                SetlblStepBackColor(MainStep.Else, true);

                lblStatus.ForeColor = Color.Blue;
                lblStatus.Text = "Ready";

                lblSecondaryMessage.ForeColor = Color.Red;
                lblSecondaryMessage.Text = "";

                rtbxMessage.Clear();
                m_nCurrentStepIndex = 0;
                m_nCurrentExecuteIndex = 0;

                ClearResultTabPageContent();
            }

            /*
            if (m_iCurStepIdx == 0)
                m_bReset = true;
            */

            //m_bTPConnected = false;
            m_bCollectFlowError = false;
            m_sCollectFlowErrorMessage = "";

            //frmMain frmParent = (frmMain)Tag;

            SetlblStepBackColor(MainStep.Else, true);

            lblStatus.ForeColor = Color.Blue;
            lblStatus.Text = "Execute";

            lblSecondaryMessage.ForeColor = Color.Red;
            lblSecondaryMessage.Text = "";

            ResizelblCurrentStepFont("");

            //m_bLoadData = cbxLoadData.Checked;
            m_bLoadData = rjtbtnLoadData.Checked;

            m_cInputDevice = new InputDevice(Handle);
            m_cDataAnalysis = new DataAnalysis();
            m_cAppCore = new AppCore(ref m_cInputDevice, ref m_cDebugLog, ref m_cDataAnalysis, this);

            m_bExecute = true;
            //App.m_bExecute = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(m_cAppCore.ExecuteMainWorkFlow), m_cFlowStep_List);
        }

        private void btnNewStart_MouseEnter(object sender, EventArgs e)
        {
            SetButtonProperties(m_btStartImage, btnNewStart, true, Color.Navy);
        }

        private void btnNewStart_MouseLeave(object sender, EventArgs e)
        {
            SetButtonProperties(m_btStartImage, btnNewStart, false, Color.Navy);
        }

        private void btnNewStart_MouseHover(object sender, EventArgs e)
        {
            SetButtonProperties(m_btStartImage, btnNewStart, true, Color.Navy);
        }

        private void btnNewStart_MouseMove(object sender, MouseEventArgs e)
        {
            SetButtonProperties(m_btStartImage, btnNewStart, true, Color.Navy);
        }

        private void btnNewStop_Click(object sender, EventArgs e)
        {
            WriteDebugLog("Action - Stop Button(Stop Finger AutoTuning Flow)");

            //ButtonOnOffOption(btnStop, "StopBtn_Disable", false);
            btnNewStop.Enabled = false;
            m_bExecute = false;
            //m_cAppCore.m_bExecute = false;
            //m_cDataAnalysis.m_bExecute = false;
            m_cAppCore.m_sErrorMessage = "Force Stop By User";

            Process[] cProcess_Array = Process.GetProcesses();

            foreach (Process cProcess in cProcess_Array)
            {
                if (cProcess.ProcessName == "HDF5ConvertTool")
                {
                    cProcess.Dispose();
                    cProcess.Close();

                    try
                    {
                        cProcess.CloseMainWindow();
                        cProcess.Kill();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void btnNewStop_MouseEnter(object sender, EventArgs e)
        {
            SetButtonProperties(m_btStopImage, btnNewStop, true, Color.Red);
        }

        private void btnNewStop_MouseLeave(object sender, EventArgs e)
        {
            SetButtonProperties(m_btStopImage, btnNewStop, false, Color.Red);
        }

        private void btnNewStop_MouseHover(object sender, EventArgs e)
        {
            SetButtonProperties(m_btStopImage, btnNewStop, true, Color.Red);
        }

        private void btnNewStop_MouseMove(object sender, MouseEventArgs e)
        {
            SetButtonProperties(m_btStopImage, btnNewStop, true, Color.Red);
        }

        private void btnNewReset_Click(object sender, EventArgs e)
        {
            WriteDebugLog("Action - Reset Button(Reset Finger AutoTuning Flow)");

            //ButtonOnOffOption(btnReset, "ResetBtn_Disable", false);
            btnNewReset.Enabled = false;

            m_bReset = true;
            m_eRecordState = AppCoreDefine.RecordState.NORMAL;
            m_nCurrentStepIndex = 0;
            m_nCurrentExecuteIndex = 0;

            SetlblStepBackColor(MainStep.Else, true);

            lblStatus.ForeColor = Color.Blue;
            lblStatus.Text = "Ready";

            lblSecondaryMessage.ForeColor = Color.Red;
            lblSecondaryMessage.Text = "";

            ResizelblCurrentStepFont("");
            toolstripstatuslabelStatus.Text = "";

            rtbxMessage.Clear();

            ClearResultTabPageContent();

            InitialstatusstripMessage(0, "Ready");

            //ButtonOnOffOption(btnReset, "ResetBtn_Enable", true);
            btnNewReset.Enabled = true;
        }

        private void btnNewReset_MouseEnter(object sender, EventArgs e)
        {
            SetButtonProperties(m_btResetImage, btnNewReset, true, Color.Green);
        }

        private void btnNewReset_MouseLeave(object sender, EventArgs e)
        {
            SetButtonProperties(m_btResetImage, btnNewReset, false, Color.Green);
        }

        private void btnNewReset_MouseHover(object sender, EventArgs e)
        {
            SetButtonProperties(m_btResetImage, btnNewReset, true, Color.Green);
        }

        private void btnNewReset_MouseMove(object sender, MouseEventArgs e)
        {
            SetButtonProperties(m_btResetImage, btnNewReset, true, Color.Green);
        }

        private void btnNewPattern_Click(object sender, EventArgs e)
        {
            m_cAppCore.DisplayPattern(m_cAppCore.m_sStepName, m_cAppCore.m_nCurrentExecuteIndex, m_cAppCore.m_nTotalCount);
            m_cAppCore.DisableMonitor();
        }

        private void btnNewPattern_MouseEnter(object sender, EventArgs e)
        {
            SetButtonProperties(m_btPatternImage, btnNewPattern, true, Color.Purple);
        }

        private void btnNewPattern_MouseLeave(object sender, EventArgs e)
        {
            SetButtonProperties(m_btPatternImage, btnNewPattern, false, Color.Purple);
        }

        private void btnNewPattern_MouseHover(object sender, EventArgs e)
        {
            SetButtonProperties(m_btPatternImage, btnNewPattern, true, Color.Purple);
        }

        private void btnNewPattern_MouseMove(object sender, MouseEventArgs e)
        {
            SetButtonProperties(m_btPatternImage, btnNewPattern, true, Color.Purple);
        }

        private void SetButtonProperties(Bitmap btImage, Button btnControl, bool bEnter, Color colorBorderColor)
        {
            Bitmap btImageResize;

            if (bEnter == true)
                btImageResize = new Bitmap(btImage, new Size(45, 45));
            else
                btImageResize = new Bitmap(btImage, new Size(40, 40));

            btnControl.Image = btImageResize;

            if (bEnter == true)
                btnControl.FlatAppearance.BorderSize = 1;
            else
                btnControl.FlatAppearance.BorderSize = 0;

            btnControl.FlatAppearance.BorderColor = colorBorderColor;

            if (bEnter == true)
                btnControl.BackColor = Color.LightYellow;
            else
                btnControl.BackColor = SystemColors.Control;
        }

        private void dgvRawADCS_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow dgvrDgvRow = dgvRawADCS.Rows[e.RowIndex];// get you required index
            // check the cell value under your specific column and then you can toggle your colors
        }

        private void dgvRawADCS_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void dgvRawADCS_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void dgvRawADCS_SizeChanged(object sender, EventArgs e)
        {
            /*
            int nLastColumn = dgvRawADCS.Columns.Count - 1;

            for (int nIndex = 0; nIndex < dgvRawADCS_S1.Columns.Count; nIndex++)
            {
                if (nLastColumn > 0)
                    dgvRawADCS.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                else
                    dgvRawADCS.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            for (int nIndex = 0; nIndex < dgvRawADCS.Columns.Count; nIndex++)
            {
                int nColumnWidth = dgvRawADCS.Columns[nIndex].Width;
                dgvRawADCS.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvRawADCS.Columns[nIndex].Width = nColumnWidth;
            }

            objDGVHelper.SetDetailDGVLocation();
            */

            //dgvRawADCS.AutoResizeColumns();

            int nColumnCount = dgvRawADCS.Columns.Count;
            int nColumnWidth = (int)Math.Ceiling((double)dgvRawADCS.Width / nColumnCount);

            for (int nIndex = 0; nIndex < dgvRawADCS.Columns.Count; nIndex++)
            {
                dgvRawADCS.Columns[nIndex].Width = nColumnWidth;
            }
        }

        private void dgvRawADCS_Sorted(object sender, EventArgs e)
        {
            int nRankIndex = 0;
            int nSELCIndex = 1;
            int nVSELIndex = 2;
            int nLGIndex = 3;
            int nSELGMIndex = 4;
            int nSuggestIQ_BSHIndex = 5;

            for (int nColumnIndex = 0; nColumnIndex < dgvRawADCS.Columns.Count; nColumnIndex++)
            {
                if (dgvRawADCS.Columns[nColumnIndex].HeaderText == "Rank")
                    nRankIndex = nColumnIndex;
                else if (dgvRawADCS.Columns[nColumnIndex].HeaderText == "SELC")
                    nSELCIndex = nColumnIndex;
                else if (dgvRawADCS.Columns[nColumnIndex].HeaderText.Contains("VSEL") == true)
                    nVSELIndex = nColumnIndex;
                else if (dgvRawADCS.Columns[nColumnIndex].HeaderText == "LG")
                    nLGIndex = nColumnIndex;
                else if (dgvRawADCS.Columns[nColumnIndex].HeaderText == "SELGM")
                    nSELGMIndex = nColumnIndex;
                else if (dgvRawADCS.Columns[nColumnIndex].HeaderText.Contains("Suggest IQ_BSH") == true)
                    nSuggestIQ_BSHIndex = nColumnIndex;
            }

            for (int nColumnIndex = 0; nColumnIndex < dgvRawADCS.Columns.Count; nColumnIndex++)
            {
                /*
                dgvRawADCS.EnableHeadersVisualStyles = false;

                if (nColumnIndex == nRankIndex)
                {
                    dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.BackColor = Color.LightCyan;
                    dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.ForeColor = Color.Black;
                }
                else if (nColumnIndex == nSELCIndex || nColumnIndex == nVSELIndex || nColumnIndex == nLGIndex || nColumnIndex == nSELGMIndex)
                {
                    dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.BackColor = Color.LightGreen;
                    dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.ForeColor = Color.Black;
                }
                else if (nColumnIndex == nSuggestIQ_BSHIndex)
                {
                    dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.BackColor = Color.LightYellow;
                    dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.ForeColor = Color.Black;
                }
                else
                {
                    dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.BackColor = Color.LightGray;
                    dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.ForeColor = Color.Black;
                }
                */

                for (int nRowIndex = 0; nRowIndex < dgvRawADCS.Rows.Count; nRowIndex++)
                {
                    dgvRawADCS.Rows[nRowIndex].Cells[nRankIndex].Style.BackColor = Color.LightCyan;
                    dgvRawADCS.Rows[nRowIndex].Cells[nSELCIndex].Style.BackColor = Color.LightGreen;
                    dgvRawADCS.Rows[nRowIndex].Cells[nVSELIndex].Style.BackColor = Color.LightGreen;
                    dgvRawADCS.Rows[nRowIndex].Cells[nLGIndex].Style.BackColor = Color.LightGreen;
                    dgvRawADCS.Rows[nRowIndex].Cells[nSELGMIndex].Style.BackColor = Color.LightGreen;
                    dgvRawADCS.Rows[nRowIndex].Cells[nSuggestIQ_BSHIndex].Style.BackColor = Color.LightYellow;
                }
            }
        }

        private void toolstripmenuitemRedmineTask_Click(object sender, EventArgs e)
        {
            if (m_cfrmRedmineTask == null)
            {
                m_cfrmRedmineTask = new frmRedmineTask(this);
                //m_cfrmRedmineTask.TopMost = true;

                int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(m_cfrmRedmineTask.Width / 2);
                int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(m_cfrmRedmineTask.Height / 2);

                if (this.IsMdiChild == true)
                {
                    nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(m_cfrmRedmineTask.Width / 2);
                    nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(m_cfrmRedmineTask.Height / 2);
                }

                if (m_bParentFormFlag == true)
                {
                    nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(m_cfrmRedmineTask.Width / 2);
                    nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(m_cfrmRedmineTask.Height / 2);
                }

                m_cfrmRedmineTask.StartPosition = FormStartPosition.Manual;
                m_cfrmRedmineTask.Location = new Point(nLocationX, nLocationY);
                m_cfrmRedmineTask.WindowState = FormWindowState.Maximized;

                /*
                if (m_cfrmRedmineTask.ShowDialog() == DialogResult.Cancel)
                    return;
                */
                m_cfrmRedmineTask.Show();
            }
            else
            {
                m_cfrmRedmineTask.Show();
            }
        }
    }

    public static class ControlExtentions
    {
        public static void MakeDoubleBuffered(this Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }
    }

    public class MyRenderer : ToolStripSystemRenderer
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (!e.AffectedBounds.Equals(e.ToolStrip.Bounds))
                base.OnRenderToolStripBorder(e);
            else
                ControlPaint.DrawBorder3D(e.Graphics, e.AffectedBounds.Left, e.AffectedBounds.Bottom - 3, e.AffectedBounds.Right, 3, Border3DStyle.Etched, Border3DSide.Bottom | Border3DSide.Top);
        }
    }

    public class EDIDInfo
    {
        public int m_nWidth = 0;
        public int m_nHeight = 0;
        public double m_dScreenSize = 0;
        public int m_nXResolution = 0;
        public int m_nYResolution = 0;
    }
}
