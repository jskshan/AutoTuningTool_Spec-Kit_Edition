using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Elan;
using System.Diagnostics;
using BlockingQueue;
using System.Reflection;
using System.Threading;
using System.IO;
using MPPPenAutoTuningParameter;
using System.Runtime.InteropServices;

namespace MPPPenAutoTuning
{
    public partial class frmMain : Form
    {
        #region Toggle Display
        public int WM_SYSCOMMAND = 0x0112;
        public int SC_MONITORPOWER = 0xF170;

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);
        #endregion

        // The current width of the form
        private float m_fWindowWidth;
        // The current height of the form
        private float m_fWindowHeight;
        // Indicates whether the size data for each control has been set in the Tag property
        private bool m_bIsLoadFlag;

        public bool m_bParentFormFlag = false;

        private Bitmap m_btConnectImage;
        private Bitmap m_btStartImage;
        private Bitmap m_btStopImage;

        public bool m_bRobotConnectFlag = false;
        public bool m_bDeviceConnectFlag = false;
        private bool m_bMultiClick = false;

        //public bool m_bParameterSettingFlag = false;

        public RobotAPI m_cRobot = null;
        public InputDevice m_cInputDevice = null;
        public DataAnalysis m_cAnalysis = null;
        public ProcessFlow m_cProcessFlow = null;
        public SocketAPI m_cSocket = null;
        public LogAPI m_cDebugLog = null;
        public GoDrawAPI m_cGoDrawRobot = null;

        private ThreadStart m_tsDebugLog = null;
        private Thread m_tDebugLog = null;

        // Reference to Stopwatch object
        private Stopwatch m_swCostTime = new Stopwatch();

        // MPP Pen directory name
        public const string m_sAPMainDirectoryName = "MPP Pen";

        // CPU Name
        public string m_sCPUName = "";
        // CPU Type
        public string m_sCPUType;

        // Start execution time (MMdd-HHmmss)
        private string m_sStartFlowTime;
        // Program load time (MMdd-HHmmss)
        public string m_sProgramEnterTime;

        // Directory path for storing data
        public string m_sFileDirectoryPath;
        // Directory path for storing Noise Step data
        public string m_sFileDirectoryPath_Noise;
        // Directory path for storing TiltNoise Step data
        public string m_sFileDirectoryPath_TN;
        // Directory path for storing DigiGain Tuning Step data
        public string m_sFileDirectoryPath_DGT;
        // Directory path for storing TP_Gain Tuning Step data
        public string m_sFileDirectoryPath_TPGT;
        // Directory path for storing Digital Tuning Step data
        public string m_sFileDirectoryPath_DT;

        public string m_sTotalResultWRFilePath_Noise;
        public string m_sReferenceFilePath_Noise;
        public string m_sReferenceFilePath_TPGT;

        // Saved project name
        public string m_sRecordProjectName = "";
        // Saved directory name
        public string m_sRecordDirectoryName = "";
        // Saved Noise Step directory name
        public string m_sRecordDirectoryName_Noise = "";
        // Saved TiltNoise Step directory name
        public string m_sRecordDirectoryName_TN = "";
        // Saved DigiGain Tuning Step directory name
        public string m_sRecordDirectoryName_DGT = "";
        // Saved TP_Gain Tuning Step directory name
        public string m_sRecordDirectoryName_TPGT = "";

        // Saved Digital Tuning Step directory name
        public string m_sRecordDirectoryName_DT = "";

        // Default folder path for Load Data Mode
        public string m_sDefaultFolderPath = "";

        // Recorded directory path
        private string m_sRecordDirectoryPath = "";

        // Directory path for saving data logs
        public string m_sLogDirecotryPath = string.Format(@"{0}\{1}\Log", Application.StartupPath, m_sAPMainDirectoryName);
        // Directory path for saving flow information
        public string m_sFlowDirectoryPath = string.Format(@"{0}\{1}\Flow", Application.StartupPath, m_sAPMainDirectoryName);
        // Directory path for saving command information
        public string m_sCmdDirectoryPath = string.Format(@"{0}\{1}\Cmd", Application.StartupPath, m_sAPMainDirectoryName);
        // Directory path for saving result lists
        public string m_sResultListDirectoryPath = string.Format(@"{0}\{1}\ResultList", Application.StartupPath, m_sAPMainDirectoryName);
        // File path for saving the ResultList.txt file
        public string m_sResultListFilePath = "";
        public string m_sConnectInfoFilePath = string.Format(@"{0}\{1}\ConnectInfo.txt", Application.StartupPath, m_sAPMainDirectoryName);

        public const string m_sInfoTxtFile = "Info.txt";

        // Device Index
        public int m_nDeviceIndex = 0;

        private frmFullScreen m_cfrmFullScreen = null;

        public ParamEnvironment m_cAPsetting;

        private frmPHCKPattern m_cfrmPHCKPattern = null;
        public frmParameterSetting m_cfrmParameterSetting = null;

        public List<FlowStep> m_cFlowStep_List = new List<FlowStep>();
        public List<FlowStep> m_cFlowStepResult_List = new List<FlowStep>();
        public List<MainStepCostTimeInfo> m_cMainStepCostTimeInfo_List = new List<MainStepCostTimeInfo>();

        public FlowStep m_cCurrentFlowStep = null;
        public string m_sCurrentMainStep = "";
        public string m_sCurrentSubStep = "";

        public BlockingQueue<byte> m_qbyteFIFO = new BlockingQueue<byte>();
        public byte[] m_byteBuffer_Array = null;

        public BlockingQueue<string> m_qsLogBuffer = new BlockingQueue<string>();

        public string m_sParentAPVersion = null;
        public string m_sFileVersion = null;
        public string m_sAPVersion = null;
        public string m_sAPTitle = null;
        public string m_sAPTotalVersion = null;

        public int m_nRetryCount = 0;
        public string m_sErrorMessage = "";

        public string m_sIniPath = string.Format(@"{0}\{1}\ini\Setting.ini", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sScreenListXMLPath = string.Format(@"{0}\{1}\ini\ScreenList.xml", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sDefaultFWParameterIniPath = string.Format(@"{0}\{1}\ini\FWParameter.ini", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sDefaultIniPath = string.Format(@"{0}\{1}\ini\Default.ini", Application.StartupPath, m_sAPMainDirectoryName);
        public string m_sGen8FWParameterAddressIniPath = string.Format(@"{0}\{1}\ini\Gen8Addr.ini", Application.StartupPath, m_sAPMainDirectoryName);

        public bool m_bObjectEnableFlag = true;

        public byte[] m_byteEDIDData_Array = null;
        public EDIDInfo m_cEDIDInformation;
        private DataTable m_datatableScreenSize = new DataTable("ScreenSizeItem");

        private Label[] m_lblTestItem_Array;
        
        public int m_nModeFlag = -1;

        public bool m_bResultErrorFlag = false;
        private bool m_bStartExecuteFlag = false;
        public bool m_bProcessThreadFinishFlag = false;
        public bool m_bForceStopFlowEnableFlag = false;
        public bool m_bRunStopFlowFinishFlag = true;

        #region Declare SkipPreviousStepFlag
        public int m_nSkipPreviousStepFlag = 0;
        public string m_sSpecificFlowFile = "";
        #endregion

        public double m_dHoverHeight_DT1st = 0;
        public double m_dHoverHeight_DT2nd = 0;
        public double m_dHoverHeight_PP = 0;
        public double m_dHoverHeight_PCT1st = 0;
        public double m_dHoverHeight_PCT2nd = 0;

        private int m_nDrawButtonStateFlag = MainConstantParameter.m_nDRAWSTATE_FINISH;

        public string m_sScreenSize = "";
        public string m_sSelectLTCOMPort = "N/A";
        public string m_sSelectFGCOMPort = "N/A";

        private int m_nTotalHours = 0;
        private int m_nTotalMinutes = 0;
        private int m_nTotalSeconds = 0;

        public string m_sTotalRankFileName = "TotalRank.txt";
        public string m_sTotalRankFilePath = "";

        public bool m_bStartShowFullScreenFlag = false;

        public bool m_bErrorFlag = false;
        public bool m_bInterruptFlag = false;

        private DataGridView[] m_dgvDataGridViewControl_Array;
        //private ListView[] m_dgvListViewControl_Array;
        private TabPage[] m_tpgTabPageControl_Array;
        private RichTextBox[] m_rtbxRichTextBoxControl_Array;

        private frmRedmineTask m_cfrmRedmineTask = null;

        public string m_sICSolutionName = "";

        public ICGenerationType m_eICGenerationType = ICGenerationType.None;
        public ICSolutionType m_eICSolutionType = ICSolutionType.NA;

        /// <summary>
        /// Set form initialization settings
        /// </summary>
        private void InitializeFormSetting()
        {
            // Set AP feature version
            if (ParamAutoTuning.m_nVersionType == 1 || ParamAutoTuning.m_nVersionType == 5)
            {
                cbxModeState.SelectedItem = MainConstantParameter.m_sMODE_SINGLE;
                m_nModeFlag = MainConstantParameter.m_nMODE_SINGLE;
            }
            else if (ParamAutoTuning.m_nVersionType == 2)
            {
                cbxModeState.SelectedItem = MainConstantParameter.m_sMODE_SERVER;
                m_nModeFlag = MainConstantParameter.m_nMODE_SERVER;
            }
            else if (ParamAutoTuning.m_nVersionType == 3)
            {
                cbxModeState.SelectedItem = MainConstantParameter.m_sMODE_CLIENT;
                m_nModeFlag = MainConstantParameter.m_nMODE_CLIENT;
            }
            else if (ParamAutoTuning.m_nVersionType == 4)
            {
                cbxModeState.SelectedItem = MainConstantParameter.m_sMODE_GODRAW;
                m_nModeFlag = MainConstantParameter.m_nMODE_GODRAW;
            }
            else if (ParamAutoTuning.m_nVersionType == 6)
            {
                cbxModeState.SelectedItem = MainConstantParameter.m_sMODE_LOADDATA;
                m_nModeFlag = MainConstantParameter.m_nMODE_LOADDATA;
            }
            else
            {
                cbxModeState.SelectedItem = MainConstantParameter.m_sMODE_SINGLE;
                m_nModeFlag = MainConstantParameter.m_nMODE_SINGLE;
            }

            // Display parameter settings
            m_cfrmParameterSetting.DispalyParameterSetting(m_bObjectEnableFlag);

            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            //this.MaximizeBox = false;
            // Set custom renderer
            menustripTop.Renderer = new MyRenderer();
        }

        /// <summary>
        /// Sets the initialization of the form when it is loaded
        /// </summary>
        private void InitializeFormLoad()
        {
            // Initialize DataGridView control array
            m_dgvDataGridViewControl_Array = new DataGridView[] 
            {
                dgvNoiseRank,
                dgvTTRank,
                dgvTNPTHFRank,
                dgvTNBHFRank,
                dgvTNTotalRank
            };

            // Initialize TabPage control array
            m_tpgTabPageControl_Array = new TabPage[]
            {
                tpgTNPTHF, 
                tpgTNPTHFRank, 
                tpgTNBHF, 
                tpgTNBHFRank, 
                tpgTNPreliminaryList, 
                tpgTNTotalRank,
                tpgDGTTotalList, 
                tpgTPGTTotalList,
                tpgPCHover_1st, 
                tpgPCHover_2nd, 
                tpgPCContact, 
                tpgPCTotalList,
                tpgDTHover_1st, 
                tpgDTHover_2nd, 
                tpgDTContact, 
                tpgDTPreliminaryList, 
                tpgDTHoverTRxS,
                tpgDTContactTRxS, 
                tpgDTTotalList, 
                tpgTTPTHF, 
                tpgTTBHF, 
                tpgTTRank,
                tpgTTTotalList, 
                tpgPTTotalList, 
                tpgPTPressureTable, 
                tpgLTTotalList,
                tpgLTRX5TTable, 
                tpgLTTX5TTable, 
                tpgLTRX3TTable, 
                tpgLTTX3TTable, 
                tpgLTRX2TLETable, 
                tpgLTTX2TLETable, 
                tpgLTRX2THETable,
                tpgTX2THETable
            };

            // Initialize RichTextBox control array
            m_rtbxRichTextBoxControl_Array = new RichTextBox[]
            {
                rtbxMessage,
                rtbxTNPTHFResultList, 
                rtbxTNBHFResultList, 
                rtbxTNPreliminaryList,
                rtbxDGTTotalList,
                rtbxTPGTTotalList,
                rtbxPCTHover_1stResultList, 
                rtbxPCTHover_2ndResultList, 
                rtbxPCTContactResultList, 
                rtbxPCTTotalList,
                rtbxDTHover_1stResultList, 
                rtbxDTHover_2ndResultList, 
                rtbxDTContactResultList, 
                rtbxDTPreliminaryList,
                rtbxDTHoverTRxSResultList, 
                rtbxDTContactTRxSResultList, 
                rtbxDTTotalList,
                rtbxTTPTHFResultList, 
                rtbxTTBHFResultList, 
                rtbxTTTotalList,
                rtbxPCTTotalList, 
                rtbxPTPressureTable,
                rtbxLTTotalList, 
                rtbxRX5TTable, 
                rtbxTX5TTable, 
                rtbxRX3TTable, 
                rtbxTX3TTable, 
                rtbxRX2TLETable, 
                rtbxTX2TLETable, 
                rtbxRX2THETable, 
                rtbxTX2THETable
            };

            // Initialize flag variables
            m_bRobotConnectFlag = false;
            m_bDeviceConnectFlag = false;
            m_bMultiClick = false;

            //m_bParameterSettingFlag = false;

            m_sErrorMessage = "";

            m_nDeviceIndex = 0;

            m_sSelectLTCOMPort = "N/A";
            m_sSelectFGCOMPort = "N/A";

            m_sFileDirectoryPath = "";
            m_sFileDirectoryPath_Noise = "";
            m_sFileDirectoryPath_TN = "";
            m_sFileDirectoryPath_DT = "";
            m_sFileDirectoryPath_DGT = "";
            m_sFileDirectoryPath_TPGT = "";

            m_sRecordProjectName = "";
            m_sRecordDirectoryName = "";
            m_sRecordDirectoryName_Noise = "";
            m_sRecordDirectoryName_TN = "";
            m_sRecordDirectoryName_DT = "";
            m_sRecordDirectoryName_DGT = "";
            m_sRecordDirectoryName_TPGT = "";
            m_sRecordDirectoryPath = "";

            m_nModeFlag = -1;

            m_dHoverHeight_DT1st = 0.0;
            m_dHoverHeight_DT2nd = 0.0;
            m_dHoverHeight_PCT1st = 0.0;
            m_dHoverHeight_PCT2nd = 0.0;

            m_bResultErrorFlag = false;
            m_bStartExecuteFlag = false;
            m_bProcessThreadFinishFlag = false;
            m_bForceStopFlowEnableFlag = false;
            m_bRunStopFlowFinishFlag = true;

            m_nDrawButtonStateFlag = MainConstantParameter.m_nDRAWSTATE_FINISH;

            // Output status and error message to the label
            OutputStatusAndErrorMessageLabel("Ready", "", Color.Blue);

            // Set DataGridView controls to use double buffering
            SetDataGridViewMakeDoubleBuffered();

            // Set TabPage controls to null
            SetTabPageNull();

            // Clear the list of main step cost time information
            m_cMainStepCostTimeInfo_List.Clear();

            // Set the enable state of "NewPattern" button, "Chart" button, and "NewDraw" button
            SetNewPatternButton(false);
            SetChartButton(false);
            SetNewDrawButton(false);

            // Set the position of cbxModeState dropdown box
            cbxModeState.Location = new Point(lblModeState.Right, lblModeState.Top - ((cbxModeState.Height - lblModeState.Height) / 2));
        }

        /// <summary>
        /// Sets the DataGridView controls to use the MakeDoubleBuffered method
        /// </summary>
        private void SetDataGridViewMakeDoubleBuffered()
        {
            // Enable double buffering for each DataGridView control in m_dgvDataGridViewControl_Array to improve display performance
            foreach (DataGridView dgvDataGridViewControl in m_dgvDataGridViewControl_Array)
                dgvDataGridViewControl.MakeDoubleBuffered(true);
        }

        /// <summary>
        /// Sets the TabPage controls to null
        /// </summary>
        private void SetTabPageNull()
        {
            // Set the Parent property of each TabPage control in m_tpgTabPageControl_Array to null
            foreach (TabPage tpgTabPageControl in m_tpgTabPageControl_Array)
                tpgTabPageControl.Parent = null;
        }

        /// <summary>
        /// Initializes the settings for the Connect flow execution
        /// </summary>
        private void InitializeConnectFlow()
        {
            // Set button states
            SetNewConnectButton(false);
            SetNewStartButton(false);
            SetNewStopButton(false);
            SetChartButton(false);
            SetNewDrawButton(false);

            // Initialize CPU-related information
            m_sCPUName = "";
            m_sCPUType = MainConstantParameter.m_sCPUTYPE_NONE;

            // Clear status bar
            toolstripstatuslblProgress.Text = "";
            toolstripstatuslblStep.Text = "";

            // Clear file directory information
            m_sFileDirectoryPath = "";
            m_sFileDirectoryPath_Noise = "";
            m_sFileDirectoryPath_TN = "";
            m_sFileDirectoryPath_DT = "";
            m_sFileDirectoryPath_DGT = "";
            m_sFileDirectoryPath_TPGT = "";

            // Clear record directory information
            m_sRecordProjectName = "";
            m_sRecordDirectoryName = "";
            m_sRecordDirectoryName_Noise = "";
            m_sRecordDirectoryName_TN = "";
            m_sRecordDirectoryName_DT = "";
            m_sRecordDirectoryName_DGT = "";
            m_sRecordDirectoryName_TPGT = "";

            m_sRecordDirectoryPath = "";

            // Clear Hover Height related information
            m_dHoverHeight_DT1st = 0.0;
            m_dHoverHeight_DT2nd = 0.0;
            m_dHoverHeight_PCT1st = 0.0;
            m_dHoverHeight_PCT2nd = 0.0;

            // Clear result-related information
            m_bResultErrorFlag = false;
            m_nSkipPreviousStepFlag = 0;
            m_sSpecificFlowFile = "";
            m_bProcessThreadFinishFlag = false;
            m_bForceStopFlowEnableFlag = false;
            m_bRunStopFlowFinishFlag = true;

            // Clear current flow step information
            m_cCurrentFlowStep = null;
            m_sCurrentMainStep = "";
            m_sCurrentSubStep = "";
        }

        /// <summary>
        /// Initializes the settings for the MainProcess flow execution
        /// </summary>
        private void InitializeMainProcessFlow()
        {
            //Disable Connect/Start/Stop/Chart Button
            SetNewConnectButton(false);
            SetNewStartButton(false);
            SetChartButton(false);

            // Clear the execution time records of the main flow steps
            m_cMainStepCostTimeInfo_List.Clear();

            // Set the status bar to "Start"
            toolstripstatuslblProgress.Text = "Start";
            toolstripstatuslblStep.Text = "";
            toolstripprogressbarMain.Value = 0;

            // Set the start execution flag and set the process thread finish flag to false
            m_bStartExecuteFlag = true;
            m_bProcessThreadFinishFlag = false;

            // Disable the ModeState combo box and Setting menu item
            SetModeStateComboBoxAndSettingToolStripMenuItem(false);

            // Enable the Stop button
            SetNewStopButton(true);
        }

        /// <summary>
        /// Initializes the control components
        /// </summary>
        private void InitializeControl()
        {
            // Clears the RichTextBox for displaying messages
            rtbxMessage.Clear();

            // Sets the Parent of TabPage controls to null.
            SetTabPageNull();

            // Performs initialization for all DataGridView controls
            foreach (DataGridView dgvDataGridViewControl in m_dgvDataGridViewControl_Array)
                InitializeDataGridView(dgvDataGridViewControl);

            // Clears all RichTextBox controls
            foreach (RichTextBox rtbxRichTextBoxControl in m_rtbxRichTextBoxControl_Array)
                rtbxRichTextBoxControl.Clear();

            // Clears the time cost-related control
            gbxCostTime.Controls.Clear();
        }

        /// <summary>
        /// Initializes the DataGridView control component
        /// </summary>
        /// <param name="dgvControl">The DataGridView control component</param>
        private void InitializeDataGridView(DataGridView dgvControl)
        {
            // Deletes all rows from the DataGridView
            while (dgvControl.Rows.Count != 0)
                dgvControl.Rows.RemoveAt(0);

            // Clears all columns from the DataGridView
            dgvControl.Columns.Clear();
        }

        /// <summary>
        /// Initializes the ListView control component
        /// </summary>
        /// <param name="lvControl">The ListView control component</param>
        private void InitializeListView(ListView lvControl)
        {
            // Clears the items and groups from the ListView
            lvControl.Clear();
            lvControl.Items.Clear();
            lvControl.Groups.Clear();
        }

        /// <summary>
        /// Stores the width, height, left margin, top margin, and font size of the control component in the Tag property
        /// </summary>
        /// <param name="ctrlMainControl">The control component within which the recursive control components are traversed</param>
        private void SetTag(Control ctrlMainControl)
        {
            // Iterate through all child controls
            foreach (Control ctrlCurControl in ctrlMainControl.Controls)
            {
                // Set the tag as a string representation of the control's width, height, left corner coordinates, and font size
                ctrlCurControl.Tag = string.Format("{0}:{1}:{2}:{3}:{4}", 
                                                   ctrlCurControl.Width, 
                                                   ctrlCurControl.Height, 
                                                   ctrlCurControl.Left, 
                                                   ctrlCurControl.Top, 
                                                   ctrlCurControl.Font.Size);

                // If the control has child controls, recursively set the tag
                if (ctrlCurControl.Controls.Count > 0)
                    SetTag(ctrlCurControl);
            }
        }

        /// <summary>
        /// Sets the width, height, left margin, top margin, and font size of each control component
        /// </summary>
        /// <param name="fNewWidth">The new width value</param>
        /// <param name="fNewHeight">The new height value</param>
        /// <param name="ctrlMainControl">The control component within which the recursive control components are traversed</param>
        private void SetControls(float fNewWidth, float fNewHeight, Control ctrlMainControl)
        {
            if (m_bIsLoadFlag == true)
            {
                // Iterate through the control components in the window and reset their property values
                foreach (Control ctrlControl in ctrlMainControl.Controls)
                {
                    // Get the Tag property value of the control component and split it into a string array
                    string[] sInfo_Array = ctrlControl.Tag.ToString().Split(new char[] { ':' });
                    // Determine the control component's value based on the window scaling ratio (width)
                    float fValue = Convert.ToSingle(sInfo_Array[0]) * fNewWidth;
                    // Set the width
                    ctrlControl.Width = (int)fValue;
                    // Set the height
                    fValue = Convert.ToSingle(sInfo_Array[1]) * fNewHeight;
                    ctrlControl.Height = (int)fValue;
                    // Set the left margin
                    fValue = Convert.ToSingle(sInfo_Array[2]) * fNewWidth;
                    ctrlControl.Left = (int)fValue;
                    // Set the top margin
                    fValue = Convert.ToSingle(sInfo_Array[3]) * fNewHeight;
                    ctrlControl.Top = (int)fValue;
                    // Set the font size
                    Single singleCurSize = Convert.ToSingle(sInfo_Array[4]) * fNewHeight;
                    ctrlControl.Font = new Font(ctrlControl.Font.Name, singleCurSize, ctrlControl.Font.Style, ctrlControl.Font.Unit);

                    if (ctrlControl.Controls.Count > 0)
                        SetControls(fNewWidth, fNewHeight, ctrlControl);
                }
            }
        }

        /// <summary>
        /// Constructor for frmMain, executed when the program starts
        /// </summary>
        /// <param name="bParentFormFlag">Flag indicating if it is a child form</param>
        public frmMain(string sParentAPVersion = "", string sParentAPTitle = "", bool bParentFormFlag = false)
        {
            InitializeComponent();

            m_sParentAPVersion = sParentAPVersion;

            // Initialize parameter environment
            m_cAPsetting = new ParamEnvironment(this);

            // Set parent form flag
            m_bParentFormFlag = bParentFormFlag;

            // Set button image size
            SetButtonImageSize();

            // Compute and check DFT_NUM and coefficients
            /*
            ComputeDFT_NUMAndCoefficient cComputeDFT_NUMAndCoeff = new ComputeDFT_NUMAndCoefficient();
            cComputeDFT_NUMAndCoeff.WriteDFT_NUMAndCoefficientFile();
            */

            // Form not loaded
            m_bIsLoadFlag = false;

            // Set program enter time
            m_sProgramEnterTime = System.DateTime.Now.ToString("MMdd-HHmmss");

            // Initialize Log API
            m_cDebugLog = new LogAPI(this);

            // Create new thread for DebugLog
            m_tsDebugLog = new ThreadStart(m_cDebugLog.WriteLogToFileThread);
            m_tDebugLog = new Thread(m_tsDebugLog);
            m_tDebugLog.IsBackground = true;
            m_tDebugLog.Start();

            // Record DebugLog
            WriteDebugLog("-Open Tool, Program Enter Time = " + m_sProgramEnterTime);

            // Set object enable flag
            m_bObjectEnableFlag = true;

            // Set total rank file path
            m_sTotalRankFilePath = string.Format(@"{0}\{1}", m_sFlowDirectoryPath, m_sTotalRankFileName);

            // Set result list file path
            m_sResultListFilePath = string.Format(@"{0}\ResultList.txt", m_sResultListDirectoryPath);

            ParamAutoTuning.GetMainForm(this);

            // Check if the settings file exists
            if (File.Exists(m_sIniPath) == false)
            {
                // Display warning message
                ShowMessageBox("\"Setting.ini\" No Exist!!");
            }
            else
            {
                // Load parameters
                ParamAutoTuning.LoadParameter();
                // Load PHCK mode settings
                LoadPHCKPatternSetting();
            }

            // Check if the default FW parameter file exists
            if (File.Exists(m_sDefaultFWParameterIniPath) == false)
            {
                // Display warning message
                ShowMessageBox("\"FWParameter.ini\" No Exist!!");
            }
            else
            {
                // Load default FW parameters
                ParamAutoTuning.LoadDefaultFWParameter();
            }

            // Check if the flow directory exists
            if (Directory.Exists(m_sFlowDirectoryPath) == false)
            {
                // Create flow directory
                Directory.CreateDirectory(m_sFlowDirectoryPath);
            }

            string sGetEDIDFilePath = string.Format(@"{0}\GetEDID.exe", Application.StartupPath);

            if (File.Exists(sGetEDIDFilePath) == true)
            {
                // Get EDID information
                m_byteEDIDData_Array = GetEDID();

                // Check if the screen list XML file exists
                if (File.Exists(m_sScreenListXMLPath) == false)
                {
                    // Display warning message
                    ShowMessageBox("\"ScreenList.xml\" No Exist!!");
                }
                else
                {
                    // If the retrieved EDID data is empty, display an error message
                    if (m_byteEDIDData_Array == null || m_byteEDIDData_Array.Length == 0)
                    {
                        ShowMessageBox("Get EDID Information Error. Please Select Screen Size Yourself!!");
                    }

                    // Create EDID information object
                    m_cEDIDInformation = new EDIDInfo();

                    // Set screen size information table columns
                    m_datatableScreenSize.Columns.Add("Name");
                    m_datatableScreenSize.Columns.Add("Width");
                    m_datatableScreenSize.Columns.Add("Height");

                    // Read screen size from EDID data
                    GetScreenSizeFromEDID();
                }
            }
            else
            {
                ShowMessageBox("GetEDID.exe File Not Exist!");
            }

            // Retrieve program compile date, time, and version number
            DateTime dtLastBuild = RetrieveLinkerTimestamp();

            // string sFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
            string path = Assembly.GetExecutingAssembly().Location;
            string sFileVersion = FileVersionInfo.GetVersionInfo(path).ProductVersion; // ← 讀取 InformationalVersion
            m_sFileVersion = sFileVersion;
            string sLastBuild = dtLastBuild.ToString("yyyyMMdd-HHmmss");

            // Set window title and version information
            m_sAPVersion = string.Format("{0} ({1})", sFileVersion, sLastBuild);
            m_sAPTitle = string.Format("MPP Pen AutoTuning V{0}", m_sAPVersion);
            this.Text = m_sAPTitle;
            m_sAPTotalVersion = string.Format("{0}_MPP Pen AutoTuning V{1}", sParentAPTitle, sFileVersion);

            // Hide Help menu item
            toolstripmenuitemHelp.Visible = false;

            // Initialize buffer array
            m_byteBuffer_Array = new byte[ParamAutoTuning.m_nReportDataLength];

            // Create RobotAPI, InputDevice, SocketAPI, and DataAnalysis objects
            m_cRobot = new RobotAPI(this);
            m_cInputDevice = new InputDevice(Handle, this);
            m_cSocket = new SocketAPI(ref m_cRobot, this);
            m_cAnalysis = new DataAnalysis(this);

            // Create ProcessFlow object, set network communication flow controller,
            // create ParameterSetting form object, and hide it
            m_cProcessFlow = new ProcessFlow(this);
            m_cSocket.SetProcessFlow();

            m_cfrmParameterSetting = new frmParameterSetting(m_bObjectEnableFlag, this);
            //m_cfrmParameterSetting.Hide();

            // Set initial form state
            InitializeFormSetting();
            InitializeFormLoad();
        }

        /// <summary>
        /// Retrieves the linker timestamp of the current assembly
        /// </summary>
        /// <returns>The timestamp as DateTime</returns>
        private DateTime RetrieveLinkerTimestamp()
        {
            string sFilePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int nPeHeaderOffset = 60;
            const int nLinkerTimestampOffset = 8;
            byte[] byteData_Array = new byte[2048];
            System.IO.Stream stream = null;

            try
            {
                // Open the file and read the first 2048 bytes
                stream = new System.IO.FileStream(sFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                stream.Read(byteData_Array, 0, 2048);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            // Get the timestamp
            int nData = System.BitConverter.ToInt32(byteData_Array, nPeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(byteData_Array, nData + nLinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(8);
            return dt;
        }

        /// <summary>
        /// Sets the size of the Button control's image
        /// </summary>
        private void SetButtonImageSize()
        {
            // Get the Connect button icon and resize it
            m_btConnectImage = btnNewConnect.Image as Bitmap;
            Bitmap btConnectResize = new Bitmap(m_btConnectImage, new Size(45, 45));
            btnNewConnect.Image = btConnectResize;
            btnNewConnect.ImageAlign = ContentAlignment.MiddleCenter;
            btnNewConnect.TextImageRelation = TextImageRelation.ImageAboveText;
            btnNewConnect.TextAlign = ContentAlignment.MiddleCenter;

            // Get the Start button icon and resize it
            m_btStartImage = btnNewStart.Image as Bitmap;
            Bitmap btStartResize = new Bitmap(m_btStartImage, new Size(45, 45));
            btnNewStart.Image = btStartResize;
            btnNewStart.ImageAlign = ContentAlignment.MiddleCenter;
            btnNewStart.TextImageRelation = TextImageRelation.ImageAboveText;
            btnNewStart.TextAlign = ContentAlignment.MiddleCenter;

            // Get the Stop button icon and resize it
            m_btStopImage = btnNewStop.Image as Bitmap;
            Bitmap btStopResize = new Bitmap(m_btStopImage, new Size(45, 45));
            btnNewStop.Image = btStopResize;
            btnNewStop.ImageAlign = ContentAlignment.MiddleCenter;
            btnNewStop.TextImageRelation = TextImageRelation.ImageAboveText;
            btnNewStop.TextAlign = ContentAlignment.MiddleCenter;

            // Get the Pattern button icon and resize it
            Bitmap btPatternImage = btnNewPattern.Image as Bitmap;
            Bitmap btPatternResize = new Bitmap(btPatternImage, new Size(18, 18));
            btnNewPattern.Image = btPatternResize;
            btnNewPattern.ImageAlign = ContentAlignment.MiddleLeft;
            btnNewPattern.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnNewPattern.TextAlign = ContentAlignment.MiddleRight;

            // Get the Draw button icon and resize it
            Bitmap btDrawImage = btnNewDraw.Image as Bitmap;
            Bitmap btDrawResize = new Bitmap(btDrawImage, new Size(18, 18));
            btnNewDraw.Image = btDrawResize;
            btnNewDraw.ImageAlign = ContentAlignment.MiddleLeft;
            btnNewDraw.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnNewDraw.TextAlign = ContentAlignment.MiddleRight;

            // Get the Chart button icon and resize it
            Bitmap btChartImage = btnChart.Image as Bitmap;
            Bitmap btChartResize = new Bitmap(btChartImage, new Size(18, 18));
            btnChart.Image = btChartResize;
            btnChart.ImageAlign = ContentAlignment.MiddleLeft;
            btnChart.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnChart.TextAlign = ContentAlignment.MiddleRight;
        }

        /// <summary>
        /// Overrides the Windows message processing procedure
        /// </summary>
        /// <param name="mMessage">Windows message</param>
        protected override void WndProc(ref Message mMessage)
        {
            if (m_cInputDevice != null)
            {
                switch (mMessage.Msg)
                {
                    // Check if it is an Input Device Windows message
                    case MainConstantParameter.m_nWM_INPUT:
                        // If currently in recording state or checking if report exists
                        if (m_cProcessFlow.CheckRecordState() == true || m_cProcessFlow.GetCheckReportExistFlag() == true)
                        {
                            // Call InputDevice's ProcessMessage to handle the Windows message
                            m_cInputDevice.ProcessMessage(mMessage);
                        }

                        break;
                    default:
                        break;
                }
            }

            base.WndProc(ref mMessage);
        }


        /// <summary>
        /// Event handler for the "cbxModeState" ComboBox control
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event arguments</param>
        private void cbxModeState_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set the mode flag
            m_nModeFlag = SetModeFlag();

            // Set object state based on the mode
            if (m_nModeFlag == MainConstantParameter.m_nMODE_SERVER ||
                m_nModeFlag == MainConstantParameter.m_nMODE_DEBUG ||
                m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
            {
                SetObjectState(cbxModeState.SelectedItem.ToString());
            }

            // Create step items
            CreateStepItem(ParamAutoTuning.m_cStepSettingParameter_Array);
            // Create mode labels
            CreateModeLabel();
            // Create flow steps
            CreateFlowStep();
            // Create result TabControl
            CreateResultTabControl(m_bStartExecuteFlag);

            // Set the connection and start button state based on the mode
            if (m_nModeFlag == MainConstantParameter.m_nMODE_SERVER ||
                m_nModeFlag == MainConstantParameter.m_nMODE_DEBUG ||
                m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
            {
                SetNewConnectButton(true);
                SetNewStartButton(false);
            }
            else if (m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
            {
                SetNewConnectButton(false);
                SetNewStartButton(true);
            }
        }

        /// <summary>
        /// Sets the flag to enable or disable objects based on the currently selected mode state
        /// </summary>
        /// <param name="sModeState">The selected mode state as a string</param>
        private void SetObjectState(string sModeState)
        {
            if (sModeState != MainConstantParameter.m_sMODE_SINGLE)
                m_bObjectEnableFlag = !CheckModeState(sModeState);
        }

        /// <summary>
        /// Displays a full-screen window icon
        /// </summary>
        /// <param name="cFlowStep">The flow step</param>
        public void ShowFullScreen(FlowStep cFlowStep)
        {
            // If the display type is 3, return
            if (ParamAutoTuning.m_nDisplayType == 3)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                // Create a pattern setting object
                PatternSetting cPatternSetting = new PatternSetting();

                // Set the pattern setting index based on the display type
                if (ParamAutoTuning.m_nDisplayType == 0)
                    cPatternSetting.m_nPatternSettingIndex = 1;
                else if (ParamAutoTuning.m_nDisplayType == 1)
                    cPatternSetting.m_nPatternSettingIndex = 2;

                // Get the progress status information
                int nCurrentNumber = toolstripprogressbarMain.Value;
                int nTotalNumber = toolstripprogressbarMain.Maximum;
                string sProgressStatus = string.Format("{0} / {1}", nCurrentNumber, nTotalNumber);

                string sMessage = "";
                string sSubStep = StringConvert.m_dictSubStepMappingTable[cFlowStep.m_eSubStep];

                // Set the appropriate information based on the main step
                switch (cFlowStep.m_eMainStep)
                {
                    case MainTuningStep.NO:
                    case MainTuningStep.TILTNO:
                    case MainTuningStep.PEAKCHECKTUNING:
                    case MainTuningStep.DIGITALTUNING:
                        string sHoverHeight = StringConvert.ConvertStepToHoverHeight(this);
                        sMessage = string.Format("{0}{1}", sSubStep, sHoverHeight);
                        break;
                    case MainTuningStep.DIGIGAINTUNING:
                    case MainTuningStep.TILTTUNING:
                    case MainTuningStep.LINEARITYTUNING:
                        string sPatternType = StringConvert.m_dictPatternTypeMappingTable[cFlowStep.m_nPatternType];
                        sMessage = string.Format("{0}{1}", sSubStep, sPatternType);
                        break;
                    case MainTuningStep.TPGAINTUNING:
                        sPatternType = StringConvert.m_dictPatternTypeMappingTable[cFlowStep.m_nPatternType];
                        int nRAngle = StringConvert.m_dictTPGTRAngleMappingTable[cFlowStep.m_nPatternType];
                        sMessage = string.Format("{0}{1}(V:{2}, R:{3})", sSubStep, sPatternType, ParamAutoTuning.m_nTPGTVAngle, nRAngle);
                        break;
                    case MainTuningStep.PRESSURETUNING:
                        switch (cFlowStep.m_eSubStep)
                        {
                            case SubTuningStep.PRESSURESETTING:
                                string sIQ_BSH_P = string.Format("[IQ_BSH_P={0}]", cFlowStep.m_nParamIQ_BSH_P);
                                string sWeight = Convert.ToString(cFlowStep.m_nPressureWieght);
                                sMessage = string.Format("{0}{1}({2}g)", sSubStep, sIQ_BSH_P, sWeight);
                                break;
                            case SubTuningStep.PRESSURETABLE:
                                sWeight = Convert.ToString(cFlowStep.m_nPressureWieght);
                                sMessage = string.Format("{0}({1}g)", sSubStep, sWeight);
                                break;
                            case SubTuningStep.PRESSUREPROTECT:
                                sHoverHeight = StringConvert.ConvertStepToHoverHeight(this);
                                sMessage = string.Format("{0}{1}", sSubStep, sHoverHeight);
                                break;
                            default:
                                break;
                        }

                        break;
                    default:
                        break;
                }


                // Check if the main step is "Noise" or "TiltNoise"
                if (cFlowStep.m_eMainStep == MainTuningStep.NO || cFlowStep.m_eMainStep == MainTuningStep.TILTNO)
                {
                    // append description to the message
                    sMessage = string.Format("{0}{1}", sMessage, cFlowStep.m_sDescription);
                }

                // If it is the second pattern setting and the pattern type is 0 in auto-tuning mode
                if (cPatternSetting.m_nPatternSettingIndex == 2 && ParamAutoTuning.m_nPatternType == 0)
                {
                    // Create a PHCKPattern window and show it in fullscreen
                    m_cfrmPHCKPattern = new frmPHCKPattern(true, m_byteEDIDData_Array, m_cEDIDInformation, this, sProgressStatus, sMessage, cFlowStep.m_nPatternType, cFlowStep.m_bDrawLineByUser);
                    m_cfrmPHCKPattern.Show();
                    // m_cfrmPHCKPattern.TopMost = true;
                    m_bStartShowFullScreenFlag = true;
                }
                else
                {
                    // Get information about all screens
                    Screen[] cAllScreen_Array = Screen.AllScreens;

                    // First pattern setting
                    if (cPatternSetting.m_nPatternSettingIndex == 1)
                    {
                        // Get the selected color
                        cPatternSetting.m_nColorIndex = m_cfrmParameterSetting.m_nColorSelectIndex;

                        // Create a FullScreen window and show it in fullscreen
                        m_cfrmFullScreen = new frmFullScreen(true, m_cfrmParameterSetting.m_colorDisplayBackColor, this, sProgressStatus, sMessage, cFlowStep.m_nPatternType, cFlowStep.m_bDrawLineByUser);

                        if (cPatternSetting.m_nColorIndex > 0 && cPatternSetting.m_nColorIndex < 11)
                        {
                            // Set the window background color
                            m_cfrmFullScreen.BackColor = m_cfrmParameterSetting.m_colorDisplayBackColor;

                            if (cAllScreen_Array.Length > 1)
                            {
                                // If there are multiple screens, display the window on the second screen
                                m_cfrmFullScreen.Location = cAllScreen_Array[1].Bounds.Location;
                                m_cfrmFullScreen.StartPosition = FormStartPosition.Manual;
                                m_cfrmFullScreen.Location = new Point(cAllScreen_Array[1].Bounds.Location.X, cAllScreen_Array[1].Bounds.Location.Y);
                            }

                            m_cfrmFullScreen.Show();
                            m_cfrmFullScreen.TopMost = true;
                            m_bStartShowFullScreenFlag = true;

                            SetNewPatternButton(false);
                        }
                        else
                        {
                            m_cfrmFullScreen = null;
                        }
                    }
                    // Second pattern setting
                    else if (cPatternSetting.m_nPatternSettingIndex == 2)
                    {
                        // Set the pattern path to the manually selected path for tuning
                        cPatternSetting.m_sPatternPath = ParamAutoTuning.m_sManualPatternPath;

                        // Declare and load the pattern image
                        Bitmap bmPatternPicture;
                        bmPatternPicture = new Bitmap(cPatternSetting.m_sPatternPath);

                        // Show the FullScreen window and display the pattern on it
                        m_cfrmFullScreen = new frmFullScreen(true, Color.White, this, sProgressStatus, sMessage, cFlowStep.m_nPatternType, cFlowStep.m_bDrawLineByUser, bmPatternPicture);

                        // If there are multiple screens, display the window on the second screen
                        if (cAllScreen_Array.Length > 1)
                        {
                            m_cfrmFullScreen.Location = cAllScreen_Array[1].Bounds.Location;
                            m_cfrmFullScreen.StartPosition = FormStartPosition.Manual;
                            m_cfrmFullScreen.Location = new Point(cAllScreen_Array[1].Bounds.Location.X, cAllScreen_Array[1].Bounds.Location.Y);
                        }

                        // Show the FullScreen window and set it as the topmost window
                        m_cfrmFullScreen.Show();
                        m_cfrmFullScreen.TopMost = true;

                        // Set the flag to start showing the FullScreen window to true
                        m_bStartShowFullScreenFlag = true;

                        SetNewPatternButton(false);
                    }
                }
            });
        }

        /// <summary>
        /// Set display time and report number
        /// </summary>
        /// <param name="dTime">Time</param>
        /// <param name="nReportNumber">Report number</param>
        public void SetTimeAndReportNumber(double dTime, int nReportNumber)
        {
            // If the interrupt flag is true, return
            if (m_bInterruptFlag == true)
                return;

            // If the display type is 0 (FullScreen display)
            if (ParamAutoTuning.m_nDisplayType == 0)
            {
                // If the FullScreen window is not null, set the text for display time and report number
                if (m_cfrmFullScreen != null)
                    m_cfrmFullScreen.SetTimeAndReportNumberText(dTime, nReportNumber);
            }
            // If the display type is 1 (PHCH pattern display)
            else if (ParamAutoTuning.m_nDisplayType == 1)
            {
                // If the PHCK pattern window is not null, set the text for display time and report number
                if (m_cfrmPHCKPattern != null)
                    m_cfrmPHCKPattern.SetTimeAndReportNumberText(dTime, nReportNumber);
            }
        }

        /// <summary>
        /// Set the number of displayed frames
        /// </summary>
        /// <param name="sFrameMessage">Frame message</param>
        public void SetFrameNumber(string sFrameMessage)
        {
            // If the display type is 0 (FullScreen display)
            if (ParamAutoTuning.m_nDisplayType == 0)
            {
                // If the FullScreen window is not null, set the text for the number of displayed frames
                if (m_cfrmFullScreen != null)
                    m_cfrmFullScreen.SetFrameNumberText(sFrameMessage);
            }
            // If the display type is 1 (PHCH pattern display)
            else if (ParamAutoTuning.m_nDisplayType == 1)
            {
                // If the PHCK pattern window is not null, set the text for the number of displayed frames
                if (m_cfrmPHCKPattern != null)
                    m_cfrmPHCKPattern.SetFrameNumberText(sFrameMessage);
            }
        }

        /// <summary>
        /// Hide the FullScreen window
        /// </summary>
        public void HideFullScreen()
        {
            // If the display mode is 3, return directly
            if (ParamAutoTuning.m_nDisplayType == 3)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                PatternSetting cPatternSetting = new PatternSetting();

                // Set the pattern setting index based on the display mode
                if (ParamAutoTuning.m_nDisplayType == 0)
                    cPatternSetting.m_nPatternSettingIndex = 1;
                else if (ParamAutoTuning.m_nDisplayType == 1)
                    cPatternSetting.m_nPatternSettingIndex = 2;

                // Execute the following code while the start show full screen flag is true
                while (m_bStartShowFullScreenFlag == true)
                {
                    // Set the NewPattern button to disabled state
                    SetNewPatternButton(false);

                    // If the display mode is 1 and the pattern type is 0, execute the following code
                    if (cPatternSetting.m_nPatternSettingIndex == 2 && ParamAutoTuning.m_nPatternType == 0)
                    {
                        if (m_cfrmPHCKPattern != null)
                        {
                            // Close the PHCK pattern FullScreen window
                            m_cfrmPHCKPattern.Close();
                            m_cfrmPHCKPattern.Dispose();
                            m_cfrmPHCKPattern = null;
                            m_bStartShowFullScreenFlag = false;
                        }
                    }
                    else
                    {
                        if (m_cfrmFullScreen != null)
                        {
                            // Close the FullScreen window
                            m_cfrmFullScreen.Close();
                            m_cfrmFullScreen.Dispose();
                            m_cfrmFullScreen = null;
                            m_bStartShowFullScreenFlag = false;
                        }
                    }

                    // Pause the thread for 100 milliseconds
                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// Disable the monitor
        /// </summary>
        public void DisableMonitor()
        {
            // If the display mode is 2 (only disable monitor), perform the following operations
            if (ParamAutoTuning.m_nDisplayType == 2)
            {
                bool bCheckFlag = false;

                // Enter an infinite loop until the monitor is disabled
                while (bCheckFlag == false)
                {
                    bool bSendMessageCompleteFlag = false;

                    /*
                    Thread tSendMessage = new Thread(() =>
                    {
                        SendMessage(Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, (int)MonitorState.MONITOR_OFF);
                        bSendMsgComplete = true;
                    });
            
                    tSendMessage.IsBackground = true;
                    tSendMessage.Start();
                    */

                    // Execute the background thread for sending the monitor off message
                    this.Invoke((MethodInvoker)delegate
                    {
                        SendMessage(Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, (int)MonitorState.MONITOR_OFF);
                        bSendMessageCompleteFlag = true;
                    });

                    // Wait for the completion of the monitor off message and check for timeouts
                    long nStart = DateTime.Now.Ticks;
                    long nCurrent = 0;
                    int nCostTime = 0;
                    bool bStopFlag = false;

                    while (bSendMessageCompleteFlag == false)
                    {
                        nCurrent = DateTime.Now.Ticks;
                        nCostTime = (int)((nCurrent - nStart) / 10000);

                        if (nCostTime > 4000)
                        {
                            bStopFlag = true;
                            break;
                        }

                        Thread.Sleep(100);
                    }

                    /*
                    if (bStopFlag == true)
                    {
                        if (tSendMessage != null || tSendMessage.IsAlive == true)
                        {
                            tSendMessage.Abort();
                            tSendMessage.Join();
                        }
                    }
                    else
                        bCheckFlag = true;
                    */

                    // If the monitor off message is not timed out, mark it as completed
                    if (bStopFlag == false)
                        bCheckFlag = true;

                    // Enable the NewPattern button
                    if (bSendMessageCompleteFlag == true)
                        SetNewPatternButton(true);
                }

                // Wait for a while to avoid immediately turning on the monitor
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Set the screen reset flow
        /// </summary>
        public void SetScreenResetFlow()
        {
            // Determine the display type
            if (ParamAutoTuning.m_nDisplayType == 2)
            {
                // Set the check flag to false
                bool bCheckFlag = false;

                while (bCheckFlag == false)
                {
                    // Set the send message complete flag to false
                    bool bSendMessageCompleteFlag = false;

                    /*
                    Thread tSendMessage = new Thread(() =>
                    {
                        SendMessage(Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, (int)MonitorState.MONITOR_ON);
                        bSendMsgComplete = true;
                    });
            
                    tSendMessage.IsBackground = true;
                    tSendMessage.Start();
                    */

                    // Invoke the SendMessage method on the UI thread to send the monitor on message
                    this.Invoke((MethodInvoker)delegate
                    {
                        SendMessage(Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, (int)MonitorState.MONITOR_ON);
                        bSendMessageCompleteFlag = true;
                    });

                    // Record the start time
                    long nStart = DateTime.Now.Ticks;
                    long nCurrent = 0;
                    int nCostTime = 0;
                    // Set the stop flag to false
                    bool bStopFlag = false;

                    while (bSendMessageCompleteFlag == false)
                    {
                        // Get the current time
                        nCurrent = DateTime.Now.Ticks;
                        // Calculate the elapsed time in milliseconds
                        nCostTime = (int)((nCurrent - nStart) / 10000);

                        // If more than 4 seconds have passed, set the stop flag to true
                        if (nCostTime > 4000)
                        {
                            bStopFlag = true;
                            break;
                        }

                        Thread.Sleep(100);
                    }

                    /*
                    if (bStopFlag == true)
                    {
                        if (tSendMessage != null || tSendMessage.IsAlive == true)
                        {
                            tSendMessage.Abort();
                            tSendMessage.Join();
                        }
                    }
                    else
                        bCheckFlag = true;
                    */
                    if (bStopFlag == false)
                        bCheckFlag = true;

                    // Set the NewPattern button to disabled
                    if (bSendMessageCompleteFlag == true)
                        SetNewPatternButton(false);
                }

                //MouseMoveEvent();
                /*
                if (bErrorFlag == true)
                    MouseMoveEvent();
                */
            }

            #region Modify Back Light
            /*
            if (ParamFingerAutoTuning.m_nBackLightValue > -1)
            {
                int nSetCount = 0;
                bool bCheckFlag = false;

                while (bCheckFlag == false)
                {
                    if (nCurBrightness > -1)
                    {
                        SetBrightness((byte)nCurBrightness);

                        int nGetCount = 0;

                        while (true)
                        {
                            try
                            {
                                int nCheckBrightness = GetBrightness();

                                if (nCurBrightness == nCheckBrightness)
                                    bCheckFlag = true;
                            }
                            catch
                            {
                            }

                            if (bCheckFlag == true)
                                break;
                            else if (nGetCount >= 3)
                                break;

                            Thread.Sleep(10);
                            nGetCount++;
                        }

                        if (bCheckFlag == false)
                            nSetCount++;
                        else if (nGetCount >= 3)
                            break;
                    }
                    else
                        break;
                }
            }
            */
            #endregion
        }

        /// <summary>
        /// Load environment settings from the INI file
        /// </summary>
        public void LoadPHCKPatternSetting()
        {
            //string sIniPath = string.Format(@"{0}\ini\Setting.ini", Application.StartupPath);
            //m_cAPsetting.LoadParameter(sIniPath);

            // Call the GetParameter method to load the settings
            m_cAPsetting.GetParameter();
        }

        

        /// <summary>
        /// Create mode label
        /// </summary>
        private void CreateModeLabel()
        {
            // Number of modes
            const int nModeNumber = 5;

            // Foreground color array
            Color[] colorForeColor_Array = new Color[nModeNumber] 
            { 
                Color.Green,
                Color.Red,
                Color.Yellow,
                Color.Blue,
                Color.Purple
            };

            for (int nModeIndex = 0; nModeIndex < nModeNumber; nModeIndex++)
            {
                // If the selected item index matches the mode index
                if (cbxModeState.SelectedIndex == nModeIndex)
                {
                    // Set mode label text
                    lblMode.Text = string.Format("{0} Mode", cbxModeState.SelectedItem.ToString());
                    // Set mode label foreground color
                    lblMode.ForeColor = colorForeColor_Array[nModeIndex];

                    // Calculate background color by subtracting RGB values from white
                    Color colorBackColor = Color.FromArgb(colorForeColor_Array[nModeIndex].A,
                                                          255 - colorForeColor_Array[nModeIndex].R,
                                                          255 - colorForeColor_Array[nModeIndex].G,
                                                          255 - colorForeColor_Array[nModeIndex].B);
                    // Set mode label background color
                    pnlMode.BackColor = colorBackColor;
                    break;
                }
            }
        }

        /// <summary>
        /// Create FlowStep items
        /// </summary>
        /// <param name="bCheckByRadioButton">Flag to indicate whether RadioButton is used for checking</param>
        private void CreateFlowStep(bool bCheckByRadioButtonFlag = true)
        {
            m_cFlowStep_List.Clear();

            if (bCheckByRadioButtonFlag == false && ParamAutoTuning.m_nVersionType == 1)
            {
                // Create a FlowStep for NO main step and NO sub step
                FlowStep cFlowStep = new FlowStep();
                cFlowStep.m_eMainStep = MainTuningStep.NO;
                cFlowStep.m_eSubStep = SubTuningStep.NO;
                cFlowStep.m_bLastStep = true;
                cFlowStep.m_nSubStepState |= (MainConstantParameter.m_nSTEPLOCATION_FIRST | MainConstantParameter.m_nSTEPLOCATION_LAST);
                m_cFlowStep_List.Add(cFlowStep);
            }
            else if ((bCheckByRadioButtonFlag == true && (m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                                                          m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                                                          m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW ||
                                                          m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)) ||
                     (bCheckByRadioButtonFlag == false && (ParamAutoTuning.m_nVersionType == 3 || 
                                                           ParamAutoTuning.m_nVersionType == 4 || 
                                                           ParamAutoTuning.m_nVersionType == 5 || 
                                                           ParamAutoTuning.m_nVersionType == 6)))
            {
                // SubTuningStep arrays for different main tuning steps
                SubTuningStep[][] eSubTuningStep_Array = new SubTuningStep[][] 
                { 
                    m_eNoiseSubStep_Array,
                    m_eTiltNoiseSubStep_Array,
                    m_eDigiGainTuningSubStep_Array,
                    m_eTPGainTuningSubStep_Array,
                    m_ePeakCheckTuningSubStep_Array,
                    m_eDigitialTuningSubStep_Array,
                    m_eTiltTuningSubStep_Array,
                    m_ePressureTuningSubStep_Array,
                    m_eLinearityTuningSubStep_Array 
                };

                for (int nParameterIndex = 0; nParameterIndex < ParamAutoTuning.m_cStepSettingParameter_Array.Length; nParameterIndex++)
                {
                    if (ParamAutoTuning.m_cStepSettingParameter_Array[nParameterIndex].m_bEnable == true)
                    {
                        SubTuningStep[] eStep_Array = null;
                        bool bSetSubTuningStep = false;

                        for (int nSetIndex = 0; nSetIndex < ParamAutoTuning.m_eMainTuningStepSet_Array.Length; nSetIndex++)
                        {
                            if (ParamAutoTuning.m_eMainTuningStepSet_Array[nSetIndex] != MainTuningStep.SERVERCONTRL &&
                                ParamAutoTuning.m_cStepSettingParameter_Array[nParameterIndex].m_eTuningStep == ParamAutoTuning.m_eMainTuningStepSet_Array[nSetIndex])
                            {
                                eStep_Array = eSubTuningStep_Array[nSetIndex];
                                bSetSubTuningStep = true;
                            }
                        }

                        if (bSetSubTuningStep == false)
                            eStep_Array = new SubTuningStep[1] { SubTuningStep.ELSE };
                        
                        for (int nStepIndex = 0; nStepIndex < eStep_Array.Length; nStepIndex++)
                        {
                            // Create a FlowStep for each main and sub step
                            FlowStep cFlowStep = new FlowStep();
                            cFlowStep.m_eMainStep = ParamAutoTuning.m_cStepSettingParameter_Array[nParameterIndex].m_eTuningStep;
                            cFlowStep.m_eSubStep = eStep_Array[nStepIndex];
                            cFlowStep.m_bLastStep = false;

                            if (nStepIndex == 0)
                                cFlowStep.m_nSubStepState |= MainConstantParameter.m_nSTEPLOCATION_FIRST;

                            if (nStepIndex == eStep_Array.Length - 1)
                                cFlowStep.m_nSubStepState |= MainConstantParameter.m_nSTEPLOCATION_LAST;

                            m_cFlowStep_List.Add(cFlowStep);
                        }
                    }
                }

                int nFlowStepListCount = m_cFlowStep_List.Count;
                
                if (nFlowStepListCount > 0)
                    m_cFlowStep_List[nFlowStepListCount - 1].m_bLastStep = true;
            }
            else if ((bCheckByRadioButtonFlag == true && m_nModeFlag == MainConstantParameter.m_nMODE_SERVER) ||
                     (bCheckByRadioButtonFlag == false && ParamAutoTuning.m_nVersionType == 2))
            {
                // Create a FlowStep for SERVERCONTRL main step and ELSE sub step
                FlowStep cFlowStep = new FlowStep();
                cFlowStep.m_eMainStep = MainTuningStep.SERVERCONTRL;
                cFlowStep.m_eSubStep = SubTuningStep.ELSE;
                cFlowStep.m_bLastStep = true;
                cFlowStep.m_nSubStepState |= (MainConstantParameter.m_nSTEPLOCATION_FIRST | MainConstantParameter.m_nSTEPLOCATION_LAST);
                m_cFlowStep_List.Add(cFlowStep);
            }
            else
            {
                // Create a FlowStep for ELSE main step and ELSE sub step
                FlowStep cFlowStep = new FlowStep();
                cFlowStep.m_eMainStep = MainTuningStep.ELSE;
                cFlowStep.m_eSubStep = SubTuningStep.ELSE;
                cFlowStep.m_bLastStep = true;
                cFlowStep.m_nSubStepState |= (MainConstantParameter.m_nSTEPLOCATION_FIRST | MainConstantParameter.m_nSTEPLOCATION_LAST);
                m_cFlowStep_List.Add(cFlowStep);
            }
        }

        /// <summary>
        /// Create the result TabControl with the option to enable confirmation
        /// </summary>
        /// <param name="bCheckFlag">Flag to enable confirmation</param>
        private void CreateResultTabControl(bool bCheckFlag = false)
        {
            // Declare an array of TabPage
            TabPage[] tpgResult_Array = new TabPage[] 
            { 
                tpgNoiseResult,
                tpgTNResult,
                tpgDGTResult,
                tpgTPGTResult,
                tpgPCTResult,
                tpgDTResult,
                tpgTTResult,
                tpgPTResult,
                tpgLTResult 
            };

            // No confirmation required
            if (bCheckFlag == false)
            {
                // Set the Parent property of all TabPages to null
                for (int nArrayIndex = 0; nArrayIndex < tpgResult_Array.Length; nArrayIndex++)
                    tpgResult_Array[nArrayIndex].Parent = null;

                // Set the Parent property of tpgCostTime to null
                tpgCostTime.Parent = null;

                // Traverse through all FlowSteps in the m_cFlowStep_List and set the corresponding TabPage's Parent property based on the FlowStep
                for (int nListIndex = 0; nListIndex < m_cFlowStep_List.Count; nListIndex++)
                {
                    for (int nStepIndex = 0; nStepIndex < ParamAutoTuning.m_eMainTuningStepSet_Array.Length; nStepIndex++)
                    {
                        if (ParamAutoTuning.m_eMainTuningStepSet_Array[nStepIndex] != MainTuningStep.SERVERCONTRL && m_cFlowStep_List[nListIndex].m_eMainStep == ParamAutoTuning.m_eMainTuningStepSet_Array[nStepIndex])
                        {
                            // Set the Parent property of the corresponding TabPage to tcResult
                            tpgResult_Array[nStepIndex].Parent = tcResult;
                            // Initialize the content of the TabPage
                            InitializeResultTabPageContent(tpgResult_Array[nStepIndex]);
                        }
                    }
                }

                // If the mode flag is not MODE_SERVER, set the Parent property of tpgCostTime to tcResult and clear the controls in gbxCostTime
                if (m_nModeFlag != MainConstantParameter.m_nMODE_SERVER)
                {
                    tpgCostTime.Parent = tcResult;
                    gbxCostTime.Controls.Clear();
                }
            }
        }

        /// <summary>
        /// Initialize the content of the ResultTabPage
        /// </summary>
        /// <param name="tpgResult">Result TabPage control component</param>
        private void InitializeResultTabPageContent(TabPage tpgResult)
        {
#if _USE_VC2010
            if (tpgResult.Name == tpgNoiseResult.Name)
            {
                InitializeDataGridView(dgvNoiseRank);
            } 
            else if (tpgResult.Name == tpgTNResult.Name)
            {
                tpgTNPTHF.Parent = null;
                tpgTNPTHFRank.Parent = null;
                tpgTNBHF.Parent = null;
                tpgTNBHFRank.Parent = null;
                tpgTNPreliminaryList.Parent = null;
                tpgTNTotalRank.Parent = null;

                //InitializeListView(PTHFResultlistView);
                InitializeDataGridView(dgvTNPTHFRank);
                //InitializeListView(BHFResultlistView);
                InitializeDataGridView(dgvTNBHFRank);
                //InitializeListView(TNPreLististView);
                InitializeDataGridView(dgvTNTotalRank);

                rtbxTNPTHFResultList.Clear();
                rtbxTNBHFResultList.Clear();
                rtbxTNPreliminaryList.Clear();
            }
            else if (tpgResult.Name == tpgDGTResult.Name)
            {
                tpgDGTTotalList.Parent = null;

                rtbxDGTTotalList.Clear();
            }
            else if (tpgResult.Name == tpgTPGTResult.Name)
            {
                tpgTPGTTotalList.Parent = null;

                rtbxTPGTTotalList.Clear();
                tcTPGTResult.TabPages.Clear();
            }
            else if (tpgResult.Name == tpgPCTResult.Name)
            {
                tpgPCHover_1st.Parent = null;
                tpgPCHover_2nd.Parent = null;
                tpgPCContact.Parent = null;
                tpgPCTotalList.Parent = null;

                //InitializeListView(PCHover_1stResultlistView);
                //InitializeListView(PCHover_2ndResultlistView);
                //InitializeListView(PCContactResultlistView);
                //InitializeListView(PCTotalListlistView);
                rtbxPCTHover_1stResultList.Clear();
                rtbxPCTHover_2ndResultList.Clear();
                rtbxPCTContactResultList.Clear();
                rtbxPCTTotalList.Clear();
            }
            else if (tpgResult.Name == tpgDTResult.Name)
            {
                tpgDTHover_1st.Parent = null;
                tpgDTHover_2nd.Parent = null;
                tpgDTContact.Parent = null;
                tpgDTPreliminaryList.Parent = null;
                tpgDTHoverTRxS.Parent = null;
                tpgDTContactTRxS.Parent = null;
                tpgDTTotalList.Parent = null;

                //InitializeListView(Hover_1stResultlistView);
                //InitializeListView(Hover_2ndResultlistView);
                //InitializeListView(ContactResultlistView);
                //InitializeListView(DTPreListlistView);
                //InitializeListView(HoverTRxSListView);
                //InitializeListView(ContactTRxSListView);
                //InitializeListView(DTTotalListListView);

                rtbxDTHover_1stResultList.Clear();
                rtbxDTHover_2ndResultList.Clear();
                rtbxDTContactResultList.Clear();
                rtbxDTPreliminaryList.Clear();
                rtbxDTHoverTRxSResultList.Clear();
                rtbxDTContactTRxSResultList.Clear();
                rtbxDTTotalList.Clear();
            }
            else if (tpgResult.Name == tpgTTResult.Name)
            {
                tpgTTPTHF.Parent = null;
                tpgTTBHF.Parent = null;
                tpgTTRank.Parent = null;
                tpgTTTotalList.Parent = null;

                //InitializeListView(TTPTHFResultlistView);
                //InitializeListView(TTBHFResultlistView);

                InitializeDataGridView(dgvTTRank);

                //InitializeListView(TTTotalListlistView);

                rtbxTTPTHFResultList.Clear();
                rtbxTTBHFResultList.Clear();
                rtbxTTTotalList.Clear();
            }
            else if (tpgResult.Name == tpgPTResult.Name)
            {
                tpgPTTotalList.Parent = null;
                tpgPTPressureTable.Parent = null;

                //InitializeListView(PTTotalListlistView);

                rtbxPTTotalList.Clear();
                rtbxPTPressureTable.Clear();
            }
            else if (tpgResult.Name == tpgLTResult.Name)
            {
                tpgLTTotalList.Parent = null;
                tpgLTRX5TTable.Parent = null;
                tpgLTTX5TTable.Parent = null;
                tpgLTRX3TTable.Parent = null;
                tpgLTTX3TTable.Parent = null;
                tpgLTRX2TLETable.Parent = null;
                tpgLTTX2TLETable.Parent = null;
                tpgLTRX2THETable.Parent = null;
                tpgTX2THETable.Parent = null;

                //InitializeListView(LTTotalListlistView);

                rtbxLTTotalList.Clear();
                rtbxRX5TTable.Clear();
                rtbxTX5TTable.Clear();
                rtbxRX3TTable.Clear();
                rtbxTX3TTable.Clear();
                rtbxRX2TLETable.Clear();
                rtbxTX2TLETable.Clear();
                rtbxRX2THETable.Clear();
                rtbxTX2THETable.Clear();
            }
#else
            switch (tpgResult.Name)
            {
                /*
                case "tpgNoiseResult":
                    InitializeDataGridView(dgvNoiseRank);
                    break;
                case "tpgTNResult":
                    tpgTNPTHF.Parent = null;
                    tpgTNPTHFRank.Parent = null;
                    tpgTNBHF.Parent = null;
                    tpgTNBHFRank.Parent = null;
                    tpgTNPreList.Parent = null;
                    tpgTNTotalRank.Parent = null;
                    
                    //InitializeListView(PTHFResultlistView);
                    InitializeDataGridView(dgvTNPTHFRank);
                    //InitializeListView(BHFResultlistView);
                    InitializeDataGridView(dgvTNBHFRank);
                    //InitializeListView(TNPreLististView);
                    InitializeDataGridView(dgvTNTotalRank);

                    rtbxTNPTHFResultList.Clear();
                    rtbxTNBHFResultList.Clear();
                    rtbxTNPreList.Clear();
                    break;
                case "tpgDGTResult":
                    tpgDGTTotalList.Parent = null;

                    rtbxDGTTotalList.Clear();
                    break;
                case "tpgTPGTResult":
                    tpgTPGTTotalList.Parent = null;

                    rtbxTPGTTotalList.Clear();
                    tcTPGTResult.TabPages.Clear();
                    break;
                case "tpgPCTResult":
                    tpgPCHover_1st.Parent = null;
                    tpgPCHover_2nd.Parent = null;
                    tpgPCContact.Parent = null;
                    tpgPCTotalList.Parent = null;

                    //InitializeListView(PCHover_1stResultlistView);
                    //InitializeListView(PCHover_2ndResultlistView);
                    //InitializeListView(PCContactResultlistView);
                    //InitializeListView(PCTotalListlistView);
                    rtbxPCTHover_1stResultList.Clear();
                    rtbxPCTHover_2ndResultList.Clear();
                    rtbxPCTContactResultList.Clear();
                    rtbxPCTTotalList.Clear();
                    break;
                case "tpgDTResult":
                    tpgDTHover_1st.Parent = null;
                    tpgDTHover_2nd.Parent = null;
                    tpgDTContact.Parent = null;
                    tpgDTPreList.Parent = null;
                    tpgDTHoverTRxS.Parent = null;
                    tpgDTContactTRxS.Parent = null;
                    tpgDTTotalList.Parent = null;

                    //InitializeListView(Hover_1stResultlistView);
                    //InitializeListView(Hover_2ndResultlistView);
                    //InitializeListView(ContactResultlistView);
                    //InitializeListView(DTPreListlistView);
                    //InitializeListView(HoverTRxSListView);
                    //InitializeListView(ContactTRxSListView);
                    //InitializeListView(DTTotalListListView);

                    rtbxDTHover_1stResultList.Clear();
                    rtbxDTHover_2ndResultList.Clear();
                    rtbxDTContactResultList.Clear();
                    rtbxDTPreList.Clear();
                    rtbxDTHoverTRxSResultList.Clear();
                    rtbxDTContactTRxSResultList.Clear();
                    rtbxDTTotalList.Clear();
                    break;
                case "tpgTTResult":
                    tpgTTPTHF.Parent = null;
                    tpgTTBHF.Parent = null;
                    tpgTTRankResult.Parent = null;
                    tpgTTTotalList.Parent = null;

                    //InitializeListView(TTPTHFResultlistView);
                    //InitializeListView(TTBHFResultlistView);

                    InitializeDataGridView(dgvTTRank);

                    //InitializeListView(TTTotalListlistView);

                    rtbxTTPTHFResultList.Clear();
                    rtbxTTBHFResultList.Clear();
                    rtbxTTTotalList.Clear();
                    break;
                case "tpgPTResult":
                    tpgPTTotalList.Parent = null;
                    tpgPTPressureTable.Parent = null;

                    //InitializeListView(PTTotalListlistView);

                    rtbxPTTotalList.Clear();
                    rtbxPTPressureTable.Clear();
                    break;
                case "tpgLTResult":
                    tpgLTTotalList.Parent = null;
                    tpgLTRX5TTable.Parent = null;
                    tpgLTTX5TTable.Parent = null;
                    tpgLTRX3TTable.Parent = null;
                    tpgLTTX3TTable.Parent = null;
                    tpgLTRX2TLETable.Parent = null;
                    tpgLTTX2TLETable.Parent = null;
                    tpgLTRX2THETable.Parent = null;
                    tpgTX2THETable.Parent = null;

                    //InitializeListView(LTTotalListlistView);

                    rtbxLTTotalList.Clear();
                    rtbxRX5TTable.Clear();
                    rtbxTX5TTable.Clear();
                    rtbxRX3TTable.Clear();
                    rtbxTX3TTable.Clear();
                    rtbxRX2TLETable.Clear();
                    rtbxTX2TLETable.Clear();
                    rtbxRX2THETable.Clear();
                    rtbxTX2THETable.Clear();
                    break;
                */
                case nameof(tpgNoiseResult):
                    InitializeDataGridView(dgvNoiseRank);
                    break;
                case nameof(tpgTNResult):
                    tpgTNPTHF.Parent = null;
                    tpgTNPTHFRank.Parent = null;
                    tpgTNBHF.Parent = null;
                    tpgTNBHFRank.Parent = null;
                    tpgTNPreList.Parent = null;
                    tpgTNTotalRank.Parent = null;

                    //InitializeListView(PTHFResultlistView);
                    InitializeDataGridView(dgvTNPTHFRank);
                    //InitializeListView(BHFResultlistView);
                    InitializeDataGridView(dgvTNBHFRank);
                    //InitializeListView(TNPreLististView);
                    InitializeDataGridView(dgvTNTotalRank);

                    rtbxTNPTHFResultList.Clear();
                    rtbxTNBHFResultList.Clear();
                    rtbxTNPreList.Clear();
                    break;
                case nameof(tpgDGTResult):
                    tpgDGTTotalList.Parent = null;

                    rtbxDGTTotalList.Clear();
                    break;
                case nameof(tpgTPGTResult):
                    tpgTPGTTotalList.Parent = null;

                    rtbxTPGTTotalList.Clear();
                    tcTPGTResult.TabPages.Clear();
                    break;
                case nameof(tpgPCTResult):
                    tpgPCHover_1st.Parent = null;
                    tpgPCHover_2nd.Parent = null;
                    tpgPCContact.Parent = null;
                    tpgPCTotalList.Parent = null;

                    //InitializeListView(PCHover_1stResultlistView);
                    //InitializeListView(PCHover_2ndResultlistView);
                    //InitializeListView(PCContactResultlistView);
                    //InitializeListView(PCTotalListlistView);
                    rtbxPCTHover_1stResultList.Clear();
                    rtbxPCTHover_2ndResultList.Clear();
                    rtbxPCTContactResultList.Clear();
                    rtbxPCTTotalList.Clear();
                    break;
                case nameof(tpgDTResult):
                    tpgDTHover_1st.Parent = null;
                    tpgDTHover_2nd.Parent = null;
                    tpgDTContact.Parent = null;
                    tpgDTPreList.Parent = null;
                    tpgDTHoverTRxS.Parent = null;
                    tpgDTContactTRxS.Parent = null;
                    tpgDTTotalList.Parent = null;

                    //InitializeListView(Hover_1stResultlistView);
                    //InitializeListView(Hover_2ndResultlistView);
                    //InitializeListView(ContactResultlistView);
                    //InitializeListView(DTPreListlistView);
                    //InitializeListView(HoverTRxSListView);
                    //InitializeListView(ContactTRxSListView);
                    //InitializeListView(DTTotalListListView);

                    rtbxDTHover_1stResultList.Clear();
                    rtbxDTHover_2ndResultList.Clear();
                    rtbxDTContactResultList.Clear();
                    rtbxDTPreList.Clear();
                    rtbxDTHoverTRxSResultList.Clear();
                    rtbxDTContactTRxSResultList.Clear();
                    rtbxDTTotalList.Clear();
                    break;
                case nameof(tpgTTResult):
                    tpgTTPTHF.Parent = null;
                    tpgTTBHF.Parent = null;
                    tpgTTRankResult.Parent = null;
                    tpgTTTotalList.Parent = null;

                    //InitializeListView(TTPTHFResultlistView);
                    //InitializeListView(TTBHFResultlistView);

                    InitializeDataGridView(dgvTTRank);

                    //InitializeListView(TTTotalListlistView);

                    rtbxTTPTHFResultList.Clear();
                    rtbxTTBHFResultList.Clear();
                    rtbxTTTotalList.Clear();
                    break;
                case nameof(tpgPTResult):
                    tpgPTTotalList.Parent = null;
                    tpgPTPressureTable.Parent = null;

                    //InitializeListView(PTTotalListlistView);

                    rtbxPTTotalList.Clear();
                    rtbxPTPressureTable.Clear();
                    break;
                case nameof(tpgLTResult):
                    tpgLTTotalList.Parent = null;
                    tpgLTRX5TTable.Parent = null;
                    tpgLTTX5TTable.Parent = null;
                    tpgLTRX3TTable.Parent = null;
                    tpgLTTX3TTable.Parent = null;
                    tpgLTRX2TLETable.Parent = null;
                    tpgLTTX2TLETable.Parent = null;
                    tpgLTRX2THETable.Parent = null;
                    tpgTX2THETable.Parent = null;

                    //InitializeListView(LTTotalListlistView);

                    rtbxLTTotalList.Clear();
                    rtbxRX5TTable.Clear();
                    rtbxTX5TTable.Clear();
                    rtbxRX3TTable.Clear();
                    rtbxTX3TTable.Clear();
                    rtbxRX2TLETable.Clear();
                    rtbxTX2TLETable.Clear();
                    rtbxRX2THETable.Clear();
                    rtbxTX2THETable.Clear();
                    break;
                default:
                    break;
            }
#endif
        }

        /// <summary>
        /// Load the main setting parameters
        /// </summary>
        public void LoadMainSettingParameter()
        {
            if (m_nModeFlag == MainConstantParameter.m_nMODE_DEBUG)
                ParamAutoTuning.m_nVersionType = 1;
            else if (m_nModeFlag == MainConstantParameter.m_nMODE_SERVER)
                ParamAutoTuning.m_nVersionType = 2;
            else if (m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                ParamAutoTuning.m_nVersionType = 3;
            else if (m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                ParamAutoTuning.m_nVersionType = 4;
            else if (m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
                ParamAutoTuning.m_nVersionType = 5;
            else if (m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
                ParamAutoTuning.m_nVersionType = 6;

            m_cfrmParameterSetting.SetParameter();
        }

        /// <summary>
        /// Event triggered when the content of rtbxBoxMessage changes
        /// </summary>
        /// <param name="sender">The control that raised the event</param>
        /// <param name="e">The event arguments</param>
        private void rtbxBoxMessage_TextChanged(object sender, EventArgs e)
        {
            // Maximum line length limit
            const int nMaxLineLength = 10000;   //20000;
            // Current line count
            int nLineCount = rtbxMessage.Lines.Length;

            // If the current line count exceeds the limit
            if (nLineCount > nMaxLineLength)
            {
                /*
                // Get all lines
                string[] sLine_Array = rtbxMessage.Lines;
                // Reduce one line
                string[] sNewLine_Array = new string[sLine_Array.Length - 1];

                // Copy the new line array
                Array.Copy(sLine_Array, 1, sNewLine_Array, 0, sNewLine_Array.Length);

                // Update the line count of the text box
                rtbxMessage.Lines = sNewLine_Array;
                */

                rtbxMessage.ReadOnly = false;
                rtbxMessage.Select(0, rtbxMessage.GetFirstCharIndexFromLine(1));
                rtbxMessage.SelectedText = string.Empty;
                rtbxMessage.ReadOnly = true;
            }

            // Set the insertion point position to the end
            rtbxMessage.SelectionStart = rtbxMessage.TextLength;

            // Scrolls the contents of the control to the current caret position
            rtbxMessage.ScrollToCaret();
        }

        /// <summary>
        /// Event triggered when frmMain form is closing
        /// </summary>
        /// <param name="sender">The control that raised the event</param>
        /// <param name="e">The event arguments</param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Set the form close flag
            m_cProcessFlow.SetFormCloseFlag();

            // Set the process thread finish flag to false
            m_bProcessThreadFinishFlag = false;

            // If the process is not finished yet
            if (m_cProcessFlow.m_bProcessFinishFlag == false)
            {
                // Set the error message for force closing the main flow
                m_cProcessFlow.m_sErrorMessage = "Main Flow Force Close by Tool Close";

                // Create a thread to forcefully stop the flow
                Thread tForceStop = new Thread(() =>
                {
                    m_cProcessFlow.RunForceStopFlow(true, true);
                });

                // Set the thread as a background thread
                tForceStop.IsBackground = true;
                // Start the thread
                tForceStop.Start();

                // Record the current time
                long nStart = DateTime.Now.Ticks;
                long nCurrent = 0;
                int nCostTime = 0;
                // Set the flag to track the start time of the process thread finish
                bool bSetProcessThreadFinishStartTime = false;

                // While the thread for force stopping the flow is still running
                while (tForceStop.IsAlive == true)
                {
                    // Get the current time
                    nCurrent = DateTime.Now.Ticks;
                    // Calculate the elapsed time in milliseconds
                    nCostTime = (int)((nCurrent - nStart) / 10000);

                    // If it has been more than 15 milliseconds or more than 5 milliseconds and the process thread is not finished
                    if (nCostTime > 15 || (nCostTime > 5 && m_bProcessThreadFinishFlag == false))
                    {
                        // Record debug message: Close Tool
                        WriteDebugLog("-Close Tool");

                        // Terminate the debug log thread
                        m_tDebugLog.Abort();
                        // Wait for the debug log thread to finish
                        m_tDebugLog.Join();
                        // Close the debug log file
                        m_cDebugLog.CloseLogFile();

                        // If the form is not an MDI child form
                        if (this.IsMdiChild == false)
                        {
                            // Terminate the current process
                            Process.GetCurrentProcess().Kill();
                        }

                        // If there is no parent form
                        if (m_bParentFormFlag == false)
                        {
                            // Terminate the current process
                            Process.GetCurrentProcess().Kill();
                        }

                        return;
                    }
                    else if (m_bProcessThreadFinishFlag == true && bSetProcessThreadFinishStartTime == false)
                    {
                        nStart = DateTime.Now.Ticks;
                        bSetProcessThreadFinishStartTime = true;
                        //Application.DoEvents();
                        Thread.Sleep(10);
                    }
                    else
                    {
                        //Application.DoEvents();
                        Thread.Sleep(10);
                    }
                }
            }

            m_cInputDevice = null;

            // Close Socket
            if (m_cSocket.GetClientConnect() == true)
            {
                string sCloseMessage = "close client";
                // Send a close message to the client side
                m_cSocket.RunClientSending(ref sCloseMessage);
            }

            // Forcefully stop the Socket thread
            m_cSocket.RunForceStop();

            // Close Robot Port
            m_cRobot.ClosePort();

            // Disconnect Elan Touch
            ElanTouch.Disconnect();

            // Record debug message: Close Tool
            WriteDebugLog("-Close Tool");

            // Terminate the debug log thread
            m_tDebugLog.Abort();
            // Wait for the debug log thread to finish
            m_tDebugLog.Join();
            // Close the debug log file
            m_cDebugLog.CloseLogFile();
        }

        /// <summary>
        /// Event triggered after frmMain form is closed
        /// </summary>
        /// <param name="sender">The control that raised the event</param>
        /// <param name="e">The event arguments</param>
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Close();

            // If the form is not an MDI child form
            if (this.IsMdiChild == false)
            {
                // Terminate the application
                Environment.Exit(Environment.ExitCode);
            }

            if (m_bParentFormFlag == false)
            {
                // Terminate the application
                Environment.Exit(Environment.ExitCode);
            }
        }

        /// <summary>
        /// Event triggered when frmMain form is loaded
        /// </summary>
        /// <param name="sender">The control that raised the event</param>
        /// <param name="e">The event arguments</param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Get the width of the form
            m_fWindowWidth = this.Width;
            // Get the height of the form
            m_fWindowHeight = this.Height;
            // The control sizes have been set to Tag property
            m_bIsLoadFlag = true;
            // Call the method
            SetTag(this);

            // Set the program running mode
            m_nModeFlag = SetModeFlag();

            // Create step options
            CreateStepItem(ParamAutoTuning.m_cStepSettingParameter_Array);
            // Create labels for display modes
            CreateModeLabel();
            // Create Flow Step
            CreateFlowStep();
            // Create TabControl for displaying results
            CreateResultTabControl();
        }

        /// <summary>
        /// Event triggered after frmMain form is shown.
        /// </summary>
        /// <param name="sender">The control that raised the event</param>
        /// <param name="e">The event arguments</param>
        private void frmMain_Shown(object sender, EventArgs e)
        {
            // If it is in Single mode
            if (m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
            {
                // If there are Flow Steps and the step count is greater than 0
                if (m_cFlowStep_List != null && m_cFlowStep_List.Count > 0)
                {
                    // Get the first Flow Step
                    FlowStep cFirstFlowStep = m_cFlowStep_List[0];

                    // Check the file format
                    int nCheckFlowFileFormatFlag = CheckFlowFileFormat(cFirstFlowStep);

                    // If it is the first Flow Step and the file format is not -1 or 2
                    if (cFirstFlowStep.m_eSubStep == SubTuningStep.NO && (nCheckFlowFileFormatFlag != -1 && nCheckFlowFileFormatFlag != 2))
                        ShowFlowSettingWindow();
                }
            }
        }

        /// <summary>
        /// Event triggered when the frmMain form size changes
        /// </summary>
        /// <param name="sender">The control that raised the event</param>
        /// <param name="e">The event arguments</param>
        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            /*
            if (Hover_1stResultlistView.Parent != null)
                SetWidths(Hover_1stResultlistView);

            if (Hover_2ndResultlistView.Parent != null)
                SetWidths(Hover_2ndResultlistView);

            if (ContactResultlistView.Parent != null)
                SetWidths(ContactResultlistView);

            if (DTPreListlistView.Parent != null)
                SetWidths(DTPreListlistView);
            */
        }

        /// <summary>
        /// Displays the Flow Setting window
        /// </summary>
        public void ShowFlowSettingWindow()
        {
            // Create a new instance of the Flow Setting form
            frmFlowSetting cfrmFlowSetting = new frmFlowSetting(this);

            // Set the window position to the center of the main form
            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmFlowSetting.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmFlowSetting.Height / 2);

            // If the main form is an MDI child form, set the position to the center of the MDI parent form
            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmFlowSetting.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmFlowSetting.Height / 2);
            }

            // If the main form is a child control, set the position to the center of its parent form
            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmFlowSetting.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmFlowSetting.Height / 2);
            }

            // Set the form's start position to manual and set its location
            cfrmFlowSetting.StartPosition = FormStartPosition.Manual;
            cfrmFlowSetting.Location = new Point(nLocationX, nLocationY);

            // Show the Flow Setting window and set it as the topmost window
            cfrmFlowSetting.ShowDialog();
            cfrmFlowSetting.TopMost = true;

            // If the user clicked the "Save" button, show a format error message
            if (cfrmFlowSetting.m_bSaveFlag == true)
                cfrmFlowSetting.ShowFormatErrorMessage();
        }

        /// <summary>
        /// Loads Rank Data into a DataGridView for display
        /// </summary>
        /// <param name="sErrorMessage">Error message when loading fails</param>
        /// <param name="sFilePath">Rank Data file path</param>
        /// <param name="cFlowStep">Current FlowStep</param>
        /// <param name="bTotalDataFlag">Whether to display Total Data</param>
        /// <param name="bGen8ICSolutionTypeFlag">Whether it is a Gen8 IC Solution</param>
        /// <returns>Returns whether the display is successful</returns>
        public bool LoadRankDataToDataGridView(ref string sErrorMessage, string sFilePath, FlowStep cFlowStep, bool bTotalDataFlag = false, bool bGen8ICSolutionTypeFlag = false)
        {
            string sText = "Rank Data";

            // Check if Total Data should be displayed
            if (bTotalDataFlag == true)
                sText = "Total Rank Data";

            DataTable datatableRankData = null;

            // Check if it is a test step that doesn't require loading Rank Data
            if (cFlowStep.m_eSubStep == SubTuningStep.NO ||
                cFlowStep.m_eSubStep == SubTuningStep.TILTNO_PTHF ||
                cFlowStep.m_eSubStep == SubTuningStep.TILTNO_BHF ||
                cFlowStep.m_eSubStep == SubTuningStep.TILTTUNING_BHF)
            {
                // Check if the Rank Data file exists
                if (File.Exists(sFilePath) == false)
                {
                    sErrorMessage = string.Format("{0} File Not Exist!", sText);
                    return false;
                }

                try
                {
                    // Convert Rank Data file to DataTable format
                    datatableRankData = ConvertCsvToDataTable(cFlowStep.m_eSubStep, sFilePath, "data", ",", false, false, true);

                    // Check if certain columns need to be removed
                    if (cFlowStep.m_eSubStep != SubTuningStep.TILTTUNING_BHF)
                    {
                        datatableRankData.Columns.Remove(SpecificText.m_sColumn1);

                        string[] sRemoveColumn_Array = new string[]
                        {
                            //SpecificText.m_sTotalMax,
                            SpecificText.m_sInnerTXMultipleWeighting,
                            SpecificText.m_sInnerRXMultipleWeighting,
                            SpecificText.m_sEdgeTXMultipleWeighting,
                            SpecificText.m_sEdgeRXMultipleWeighting
                        };

                        foreach (string sRemoveColumn in sRemoveColumn_Array)
                            datatableRankData.Columns.Remove(sRemoveColumn);

                        datatableRankData.Columns.Remove(SpecificText.m_sFileName);
                    }
                    else
                    {
                        datatableRankData.Columns.Remove(SpecificText.m_sFileName);
                    }
                }
                catch
                {
                    sErrorMessage = string.Format("Read {0} File Error!", sText);
                    return false;
                }

                // Check if the DataTable is empty
                if (datatableRankData == null)
                {
                    sErrorMessage = string.Format("Display {0} Error!", sText);
                    return false;
                }

                // Display different information based on different FlowSteps
                switch (cFlowStep.m_eSubStep)
                {
                    case SubTuningStep.NO:
                        OutputResultDataGridView(datatableRankData);
                        break;
                    case SubTuningStep.TILTNO_PTHF:
                        OutputRankDataGridView_TiltNoise(tcTNResult, tpgTNPTHFRank, dgvTNPTHFRank, datatableRankData);
                        break;
                    case SubTuningStep.TILTNO_BHF:
                        if (bTotalDataFlag == true)
                            OutputRankDataGridView_TiltNoise(tcTNResult, tpgTNTotalRank, dgvTNTotalRank, datatableRankData);
                        else
                            OutputRankDataGridView_TiltNoise(tcTNResult, tpgTNBHFRank, dgvTNBHFRank, datatableRankData);
                        
                        this.Invoke((MethodInvoker)delegate
                        {
                            tcTNResult.SelectedTab = tpgTNTotalRank;
                        });
                        break;
                    case SubTuningStep.TILTTUNING_BHF:
                        OutputResultDataGridView_TiltTuning(tcTTResult, tpgTTRank, datatableRankData);
                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Load StepList file data into RichTextBox
        /// </summary>
        /// <param name="sErrorMessage">Error message when loading fails</param>
        /// <param name="sFilePath">Path of the StepList data file</param>
        /// <param name="cFlowStep">Current FlowStep</param>
        /// <returns>Returns whether the display is correct</returns>
        public bool LoadStepListDataToRichTextBox(ref string sErrorMessage, string sFilePath, FlowStep cFlowStep)
        {
            string[] sFileInfoName_Array = new string[5] 
            { 
                SpecificText.m_sFileName, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                SpecificText.m_sFrequency_KHz, 
                SpecificText.m_sErrorMessage 
            };

            string[] sFileInfoValue_Array = new string[5] 
            { 
                SpecificText.m_sFileName, 
                SpecificText.m_sReadPH1, 
                SpecificText.m_sReadPH2, 
                SpecificText.m_sFrequency, 
                SpecificText.m_sErrorMessage 
            };

            string[] sFWParameterName_Array = null;
            string[] sFWParameterValue_Array = null;
            string[] sCanbeNA_Array = null;
            bool bLoadFWParameterFlag = false;

            switch (cFlowStep.m_eMainStep)
            {
                case MainTuningStep.TILTNO:
                    if (cFlowStep.m_eSubStep == SubTuningStep.TILTNO_BHF)
                    {
                        sFWParameterName_Array = new string[12] 
                        { 
                            SpecificText.m_sFileName, 
                            SpecificText.m_sPH1, 
                            SpecificText.m_sPH2, 
                            SpecificText.m_sFrequency_KHz,
                            SpecificText.m_scActivePen_PTHF_Contact_TH_Rx, 
                            SpecificText.m_scActivePen_PTHF_Contact_TH_Tx,
                            SpecificText.m_scActivePen_PTHF_Hover_TH_Rx, 
                            SpecificText.m_scActivePen_PTHF_Hover_TH_Tx,
                            SpecificText.m_scActivePen_BHF_Contact_TH_Rx, 
                            SpecificText.m_scActivePen_BHF_Contact_TH_Tx,
                            SpecificText.m_scActivePen_BHF_Hover_TH_Rx, 
                            SpecificText.m_scActivePen_BHF_Hover_TH_Tx
                        };

                        sFWParameterValue_Array = new string[12] 
                        { 
                            SpecificText.m_sFileName, 
                            SpecificText.m_sPH1, 
                            SpecificText.m_sPH2, 
                            SpecificText.m_sFrequency,
                            SpecificText.m_scActivePen_PTHF_Contact_TH_Rx, 
                            SpecificText.m_scActivePen_PTHF_Contact_TH_Tx,
                            SpecificText.m_scActivePen_PTHF_Hover_TH_Rx, 
                            SpecificText.m_scActivePen_PTHF_Hover_TH_Tx,
                            SpecificText.m_scActivePen_BHF_Contact_TH_Rx, 
                            SpecificText.m_scActivePen_BHF_Contact_TH_Tx,
                            SpecificText.m_scActivePen_BHF_Hover_TH_Rx, 
                            SpecificText.m_scActivePen_BHF_Hover_TH_Tx 
                        };

                        bLoadFWParameterFlag = true;
                    }

                    break;
                case MainTuningStep.DIGIGAINTUNING:
                    sFWParameterName_Array = new string[12] 
                    { 
                        SpecificText.m_sFileName, 
                        SpecificText.m_sPH1, 
                        SpecificText.m_sPH2, 
                        SpecificText.m_sFrequency_KHz,
                        SpecificText.m_scActivePen_DigiGain_P0, 
                        SpecificText.m_scActivePen_DigiGain_Beacon_Rx, 
                        SpecificText.m_scActivePen_DigiGain_Beacon_Tx,
                        SpecificText.m_scActivePen_DigiGain_PTHF_Rx, 
                        SpecificText.m_scActivePen_DigiGain_PTHF_Tx,
                        SpecificText.m_scActivePen_DigiGain_BHF_Rx, 
                        SpecificText.m_scActivePen_DigiGain_BHF_Tx, 
                        SpecificText.m_sErrorMessage 
                    };

                    sFWParameterValue_Array = new string[12] 
                    { 
                        SpecificText.m_sFileName, 
                        SpecificText.m_sPH1, 
                        SpecificText.m_sPH2, 
                        SpecificText.m_sFrequency,
                        SpecificText.m_scActivePen_DigiGain_P0, 
                        SpecificText.m_scActivePen_DigiGain_Beacon_Rx, 
                        SpecificText.m_scActivePen_DigiGain_Beacon_Tx,
                        SpecificText.m_scActivePen_DigiGain_PTHF_Rx, 
                        SpecificText.m_scActivePen_DigiGain_PTHF_Tx,
                        SpecificText.m_scActivePen_DigiGain_BHF_Rx, 
                        SpecificText.m_scActivePen_DigiGain_BHF_Tx, 
                        SpecificText.m_sErrorMessage  
                    };

                    bLoadFWParameterFlag = true;
                    break;
                case MainTuningStep.TPGAINTUNING:
                    sFWParameterName_Array = new string[5] 
                    { 
                        SpecificText.m_sFileName, 
                        SpecificText.m_sPH1, 
                        SpecificText.m_sPH2, 
                        SpecificText.m_sFrequency_KHz, 
                        SpecificText.m_sErrorMessage 
                    };
                    break;
                case MainTuningStep.PEAKCHECKTUNING:
                    if (cFlowStep.m_eSubStep == SubTuningStep.PCCONTACT)
                    {
                        sFWParameterName_Array = new string[12] 
                        { 
                            SpecificText.m_sFileName, 
                            SpecificText.m_sPH1, 
                            SpecificText.m_sPH2, 
                            SpecificText.m_sFrequency_KHz,
                            SpecificText.m_sPenPeak_Th, 
                            SpecificText.m_sPenPeakWidth_Th,
                            SpecificText.m_sPenPeak_2Traces_Th, 
                            SpecificText.m_sPenPeak_1Traces_Th,
                            SpecificText.m_sPenPeak_4Traces_Th, 
                            SpecificText.m_sPenPeak_5Traces_Th,
                            SpecificText.m_sPenPeak_5Traces_PeakPwr_Th, 
                            SpecificText.m_sPenPeakCheck_AreaUP_Pwr_TH 
                        };

                        sFWParameterValue_Array = new string[12] 
                        { 
                            SpecificText.m_sFileName, 
                            SpecificText.m_sPH1, 
                            SpecificText.m_sPH2, 
                            SpecificText.m_sFrequency,
                            SpecificText.m_sPenPeak_Th, 
                            SpecificText.m_sPenPeakWidth_Th,
                            SpecificText.m_sPenPeak_2Traces_Th, 
                            SpecificText.m_sPenPeak_1Traces_Th,
                            SpecificText.m_sPenPeak_4Traces_Th, 
                            SpecificText.m_sPenPeak_5Traces_Th,
                            SpecificText.m_sPenPeak_5Traces_PeakPwr_Th, 
                            SpecificText.m_sPenPeakCheck_AreaUP_Pwr_TH 
                        };

                        bLoadFWParameterFlag = true;
                    }

                    break;
                case MainTuningStep.DIGITALTUNING:
                    if (cFlowStep.m_eSubStep == SubTuningStep.CONTACT || cFlowStep.m_eSubStep == SubTuningStep.CONTACTTRxS)
                    {
                        if (cFlowStep.m_eSubStep == SubTuningStep.CONTACT)
                        {
                            sFWParameterName_Array = new string[12] 
                            { 
                                SpecificText.m_sPH1, 
                                SpecificText.m_sPH2, 
                                SpecificText.m_sFrequency_KHz, 
                                SpecificText.m_scActivePen_FM_P0_TH,
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Rx, 
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Tx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Rx, 
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Tx,
                                SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr, 
                                SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr,
                                SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr, 
                                SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr 
                            };

                            sFWParameterValue_Array = new string[12] 
                            { 
                                SpecificText.m_sPH1, 
                                SpecificText.m_sPH2, 
                                SpecificText.m_sFrequency, 
                                SpecificText.m_scActivePen_FM_P0_TH,
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Rx, 
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Tx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Rx, 
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Tx,
                                SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr, 
                                SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr,
                                SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr, 
                                SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr 
                            };
                        }
                        else if (cFlowStep.m_eSubStep == SubTuningStep.CONTACTTRxS)
                        {
                            sFWParameterName_Array = new string[16] 
                            { 
                                SpecificText.m_sPH1, 
                                SpecificText.m_sPH2, 
                                SpecificText.m_sFrequency_KHz, 
                                SpecificText.m_scActivePen_FM_P0_TH,
                                SpecificText.m_scActivePen_Beacon_Contact_TH_Rx, 
                                SpecificText.m_scActivePen_Beacon_Contact_TH_Tx,
                                SpecificText.m_scActivePen_Beacon_Hover_TH_Rx, 
                                SpecificText.m_scActivePen_Beacon_Hover_TH_Tx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Rx, 
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Tx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Rx, 
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Tx,
                                SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr, 
                                SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr,
                                SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr, 
                                SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr 
                            };

                            sFWParameterValue_Array = new string[16] 
                            { 
                                SpecificText.m_sPH1, 
                                SpecificText.m_sPH2, 
                                SpecificText.m_sFrequency, 
                                SpecificText.m_scActivePen_FM_P0_TH,
                                SpecificText.m_scActivePen_Beacon_Contact_TH_Rx, 
                                SpecificText.m_scActivePen_Beacon_Contact_TH_Tx,
                                SpecificText.m_scActivePen_Beacon_Hover_TH_Rx, 
                                SpecificText.m_scActivePen_Beacon_Hover_TH_Tx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Rx, 
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Tx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Rx, 
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Tx,
                                SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr, 
                                SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr,
                                SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr, 
                                SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr 
                            };
                        }

                        sCanbeNA_Array = new string[5] 
                        { 
                            SpecificText.m_scActivePen_FM_P0_TH,
                            SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr,
                            SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr,
                            SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr,
                            SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr 
                        };

                        bLoadFWParameterFlag = true;
                    }

                    break;
                case MainTuningStep.PRESSURETUNING:
                    if (cFlowStep.m_eSubStep == SubTuningStep.PRESSURESETTING)
                    {
                        sFWParameterName_Array = new string[7] 
                        { 
                            SpecificText.m_sPH1, 
                            SpecificText.m_sPH2, 
                            SpecificText.m_sFrequency_KHz,
                            SpecificText.m_s_Pen_Ntrig_IQ_BSH_P, 
                            SpecificText.m_scActivePen_FM_Pressure3BinsTH,
                            SpecificText.m_sPress_3BinsPwr, 
                            SpecificText.m_sSettingErrorMessage 
                        };

                        sFWParameterValue_Array = new string[7] 
                        { 
                            SpecificText.m_sPH1, 
                            SpecificText.m_sPH2, 
                            SpecificText.m_sFrequency, 
                            SpecificText.m_s_Pen_Ntrig_IQ_BSH_P, 
                            SpecificText.m_scActivePen_FM_Pressure3BinsTH,
                            SpecificText.m_sPress_3BinsPwr, 
                            SpecificText.m_sErrorMessage 
                        };

                        sCanbeNA_Array = new string[3] 
                        { 
                            SpecificText.m_s_Pen_Ntrig_IQ_BSH_P, 
                            SpecificText.m_scActivePen_FM_Pressure3BinsTH, 
                            SpecificText.m_sPress_3BinsPwr 
                        };

                        bLoadFWParameterFlag = true;
                    }
                    else if (cFlowStep.m_eSubStep == SubTuningStep.PRESSUREPROTECT)
                    {
                        sFWParameterName_Array = new string[5] 
                        { 
                            SpecificText.m_sPH1, 
                            SpecificText.m_sPH2, 
                            SpecificText.m_sFrequency_KHz, 
                            SpecificText.m_sThreshold1, 
                            SpecificText.m_sProtectErrorMessage 
                        };

                        sFWParameterValue_Array = new string[5] 
                        { 
                            SpecificText.m_sPH1, 
                            SpecificText.m_sPH2, 
                            SpecificText.m_sFrequency, 
                            SpecificText.m_sThreshold1, 
                            SpecificText.m_sErrorMessage 
                        };

                        sCanbeNA_Array = new string[1] 
                        { 
                            SpecificText.m_sThreshold1 
                        };

                        bLoadFWParameterFlag = true;
                    }

                    break;
                case MainTuningStep.LINEARITYTUNING:
                    if (cFlowStep.m_eSubStep == SubTuningStep.LINEARITYTABLE)
                    {
                        sFWParameterName_Array = new string[4] 
                        { 
                            SpecificText.m_sPH1, 
                            SpecificText.m_sPH2, 
                            SpecificText.m_sFrequency_KHz, 
                            SpecificText.m_sErrorMessage 
                        };

                        sFWParameterValue_Array = new string[4] 
                        { 
                            SpecificText.m_sPH1, 
                            SpecificText.m_sPH2, 
                            SpecificText.m_sFrequency, 
                            SpecificText.m_sErrorMessage 
                        };

                        bLoadFWParameterFlag = true;
                    }

                    break;
                default:
                    break;
            }

            DataTable datatableFileInfo = null;
            DataTable datatableFWParameter = null;

            if (File.Exists(sFilePath) == false)
            {
                sErrorMessage = "StepList Data File Not Exist!";
                return false;
            }

            try
            {
                datatableFileInfo = ConvertCsvToDataTable(cFlowStep.m_eSubStep, sFilePath, "data", ",");
            }
            catch
            {
                sErrorMessage = "Read StepList Data File Error!";
                return false;
            }

            if (datatableFileInfo == null)
            {
                sErrorMessage = "Display StepList Data Error!";
                return false;
            }

            if (datatableFileInfo.Rows.Count == 0)
            {
                sErrorMessage = "No Any StepList Data!";
                return false;
            }

            if (bLoadFWParameterFlag == true)
            {
                try
                {
                    datatableFWParameter = ConvertCsvToDataTable(cFlowStep.m_eSubStep, sFilePath, "data", ",", true);
                }
                catch
                {
                    sErrorMessage = "Read TotalList Data File Error!";
                    return false;
                }

                if (datatableFWParameter == null)
                {
                    sErrorMessage = "Display TotalList Data Error!";
                    return false;
                }

                if (datatableFWParameter.Rows.Count == 0)
                {
                    sErrorMessage = "No Any TotalList Data!";
                    return false;
                }
            }

            switch (cFlowStep.m_eSubStep)
            {
                case SubTuningStep.TILTNO_PTHF:
                    OutputResultRichTextBox(tcTNResult, tpgTNPTHF, rtbxTNPTHFResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    break;
                case SubTuningStep.TILTNO_BHF:
                    OutputResultRichTextBox(tcTNResult, tpgTNBHF, rtbxTNBHFResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    OutputResultRichTextBox(tcTNResult, tpgTNPreliminaryList, rtbxTNPreliminaryList, datatableFWParameter, sFWParameterName_Array, sFWParameterValue_Array, sCanbeNA_Array);
                    break;
                case SubTuningStep.DIGIGAIN:
                    OutputResultRichTextBox(tcDGTResult, tpgDGTTotalList, rtbxDGTTotalList, datatableFWParameter, sFWParameterName_Array, sFWParameterValue_Array, sCanbeNA_Array, 0, cFlowStep.m_eSubStep);
                    break;
                case SubTuningStep.TP_GAIN:
                    OutputResultRichTextBox(tcTPGTResult, tpgTPGTTotalList, rtbxTPGTTotalList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    break;
                case SubTuningStep.PCHOVER_1ST:
                    OutputResultRichTextBox(tcPCTResult, tpgPCHover_1st, rtbxPCTHover_1stResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    break;
                case SubTuningStep.PCHOVER_2ND:
                    OutputResultRichTextBox(tcPCTResult, tpgPCHover_2nd, rtbxPCTHover_2ndResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    break;
                case SubTuningStep.PCCONTACT:
                    OutputResultRichTextBox(tcPCTResult, tpgPCContact, rtbxPCTContactResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    OutputResultRichTextBox(tcPCTResult, tpgPCTotalList, rtbxPCTTotalList, datatableFWParameter, sFWParameterName_Array, sFWParameterValue_Array, sCanbeNA_Array);
                    break;
                case SubTuningStep.HOVER_1ST:
                    OutputResultRichTextBox(tcDTResult, tpgDTHover_1st, rtbxDTHover_1stResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    break;
                case SubTuningStep.HOVER_2ND:
                    OutputResultRichTextBox(tcDTResult, tpgDTHover_2nd, rtbxDTHover_2ndResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    break;
                case SubTuningStep.CONTACT:
                    OutputResultRichTextBox(tcDTResult, tpgDTContact, rtbxDTContactResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    OutputResultRichTextBox(tcDTResult, tpgDTPreliminaryList, rtbxDTPreliminaryList, datatableFWParameter, sFWParameterName_Array, sFWParameterValue_Array, sCanbeNA_Array);
                    break;
                case SubTuningStep.HOVERTRxS:
                    OutputResultRichTextBox(tcDTResult, tpgDTHoverTRxS, rtbxDTHoverTRxSResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    break;
                case SubTuningStep.CONTACTTRxS:
                    OutputResultRichTextBox(tcDTResult, tpgDTContactTRxS, rtbxDTContactTRxSResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    OutputResultRichTextBox(tcDTResult, tpgDTTotalList, rtbxDTTotalList, datatableFWParameter, sFWParameterName_Array, sFWParameterValue_Array, sCanbeNA_Array);
                    break;
                case SubTuningStep.TILTTUNING_PTHF:
                    OutputResultRichTextBox(tcTTResult, tpgTTPTHF, rtbxTTPTHFResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    break;
                case SubTuningStep.TILTTUNING_BHF:
                    OutputResultRichTextBox(tcTTResult, tpgTTBHF, rtbxTTBHFResultList, datatableFileInfo, sFileInfoName_Array, sFileInfoValue_Array);
                    break;
                case SubTuningStep.PRESSURESETTING:
                    OutputResultRichTextBox(tcPTResult, tpgPTTotalList, rtbxPTTotalList, datatableFWParameter, sFWParameterName_Array, sFWParameterValue_Array, sCanbeNA_Array);
                    break;
                case SubTuningStep.PRESSUREPROTECT:
                    OutputResultRichTextBox(tcPTResult, tpgPTTotalList, rtbxPTTotalList, datatableFWParameter, sFWParameterName_Array, sFWParameterValue_Array, sCanbeNA_Array);
                    break;
                case SubTuningStep.LINEARITYTABLE:
                    OutputResultRichTextBox(tcLTResult, tpgLTTotalList, rtbxLTTotalList, datatableFWParameter, sFWParameterName_Array, sFWParameterValue_Array, sCanbeNA_Array);
                    break;
                default:
                    break;
            }

            return true;
        }

        /// <summary>
        /// Add StepList file data to RichTextBox
        /// </summary>
        /// <param name="sErrorMessage">Error message when loading fails</param>
        /// <param name="sFilePath">Path of the StepList data file</param>
        /// <param name="cFlowStep">Current FlowStep</param>
        /// <returns>Returns whether the display is correct</returns>
        public bool AddStepListDataToRichTextBox(ref string sErrorMessage, string sFilePath, FlowStep cFlowStep)
        {
            if (cFlowStep.m_eSubStep != SubTuningStep.PRESSURETABLE)
                return true;

            string[] sFWParameterName_Array = new string[1] 
            { 
                SpecificText.m_sTableErrorMessage 
            };

            string[] sFWParameterValue_Array = new string[1] 
            { 
                SpecificText.m_sErrorMessage 
            };

            string[] sFWParameterMove_Array = new string[1] 
            { 
                SpecificText.m_sSettingErrorMessage 
            };

            DataTable datatableFWParameter = null;

            if (File.Exists(sFilePath) == false)
            {
                sErrorMessage = "StepList Data File Not Exist!";
                return false;
            }

            try
            {
                datatableFWParameter = ConvertCsvToDataTable(cFlowStep.m_eSubStep, sFilePath, "data", ",", true);
            }
            catch
            {
                sErrorMessage = "Read TotalList Data File Error!";
                return false;
            }

            if (datatableFWParameter == null)
            {
                sErrorMessage = "Display TotalList Data Error!";
                return false;
            }
            else if (datatableFWParameter.Rows.Count == 0)
            {
                sErrorMessage = "No Any TotalList Data!";
                return false;
            }

            ModifyResultRichTextBox(tcPTResult, tpgPTTotalList, rtbxPTTotalList, datatableFWParameter, sFWParameterName_Array, sFWParameterValue_Array, sFWParameterMove_Array);
            return true;
        }

        /// <summary>
        /// Load only TotalList file data to RichTextBox
        /// </summary>
        /// <param name="sErrorMessage">Error message when loading fails</param>
        /// <param name="sFilePath">Path of the StepList data file</param>
        /// <param name="cFlowStep">Current FlowStep</param>
        /// <returns>Returns whether the display is correct</returns>
        public bool LoadJustTotalListDataToRichTextBox(ref string sErrorMessage, string sFilePath, FlowStep cFlowStep)
        {
            string[] sFWParameterName_Array = new string[13] 
            { 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                SpecificText.m_sFrequency_KHz,
                SpecificText.m_scActivePen_PTHF_Contact_TH_Rx, 
                SpecificText.m_scActivePen_PTHF_Contact_TH_Tx,
                SpecificText.m_scActivePen_PTHF_Hover_TH_Rx, 
                SpecificText.m_scActivePen_PTHF_Hover_TH_Tx,
                SpecificText.m_scActivePen_BHF_Contact_TH_Rx, 
                SpecificText.m_scActivePen_BHF_Contact_TH_Tx,
                SpecificText.m_scActivePen_BHF_Hover_TH_Rx, 
                SpecificText.m_scActivePen_BHF_Hover_TH_Tx,
                SpecificText.m_scPenTiltPwrTHTXHB, 
                SpecificText.m_scPenTiltPwrTHTXLB 
            };

            string[] sFWParameterValue_Array = new string[13] 
            { 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                SpecificText.m_sFrequency,
                SpecificText.m_scActivePen_PTHF_Contact_TH_Rx, 
                SpecificText.m_scActivePen_PTHF_Contact_TH_Tx,
                SpecificText.m_scActivePen_PTHF_Hover_TH_Rx, 
                SpecificText.m_scActivePen_PTHF_Hover_TH_Tx,
                SpecificText.m_scActivePen_BHF_Contact_TH_Rx, 
                SpecificText.m_scActivePen_BHF_Contact_TH_Tx,
                SpecificText.m_scActivePen_BHF_Hover_TH_Rx, 
                SpecificText.m_scActivePen_BHF_Hover_TH_Tx,
                SpecificText.m_scPenTiltPwrTHTXHB, 
                SpecificText.m_scPenTiltPwrTHTXLB 
            };

            string[] sCanbeNA_Array = new string[10] 
            { 
                SpecificText.m_scActivePen_PTHF_Contact_TH_Rx, 
                SpecificText.m_scActivePen_PTHF_Contact_TH_Tx,
                SpecificText.m_scActivePen_PTHF_Hover_TH_Rx, 
                SpecificText.m_scActivePen_PTHF_Hover_TH_Tx,
                SpecificText.m_scActivePen_BHF_Contact_TH_Rx, 
                SpecificText.m_scActivePen_BHF_Contact_TH_Tx,
                SpecificText.m_scActivePen_BHF_Hover_TH_Rx, 
                SpecificText.m_scActivePen_BHF_Hover_TH_Tx,
                SpecificText.m_scPenTiltPwrTHTXHB, 
                SpecificText.m_scPenTiltPwrTHTXLB 
            };

            DataTable datatableFWParameter = null;

            if (File.Exists(sFilePath) == false)
            {
                sErrorMessage = "StepList Data File Not Exist!";
                return false;
            }

            try
            {
                datatableFWParameter = ConvertCsvToDataTable(cFlowStep.m_eSubStep, sFilePath, "data", ",", true, true);
            }
            catch
            {
                sErrorMessage = "Read TotalList Data File Error!";
                return false;
            }

            if (datatableFWParameter == null)
            {
                sErrorMessage = "Display TotalList Data Error!";
                return false;
            }

            if (datatableFWParameter.Rows.Count == 0)
            {
                sErrorMessage = "No Any TotalList Data!";
                return false;
            }

            OutputResultRichTextBox(tcTTResult, tpgTTTotalList, rtbxTTTotalList, datatableFWParameter, sFWParameterName_Array, sFWParameterValue_Array, sCanbeNA_Array);
            return true;
        }

        /// <summary>
        /// Load Pressure Table data to RichTextBox
        /// </summary>
        /// <param name="sErrorMessage">Error message when loading fails</param>
        /// <param name="sFilePath">Path of the StepList data file</param>
        /// <param name="cFlowStep">Current FlowStep</param>
        /// <returns>Returns whether the display is correct</returns>
        public bool LoadPTTableToRichTextBox(ref string sErrorMessage, string sFilePath, FlowStep cFlowStep)
        {
            if (File.Exists(sFilePath) == false)
            {
                sErrorMessage = "Table Data File Not Exist!";
                return false;
            }

            StreamReader srPTTable = new StreamReader(sFilePath);
            string sPTTableData = srPTTable.ReadToEnd();
            srPTTable.Close();

            OutputRichTextBox(tcPTResult, tpgPTPressureTable, rtbxPTPressureTable, sPTTableData);

            DisplayTableData(tcPTResult, tpgPTPressureTable, rtbxPTPressureTable, sFilePath);
            return true;
        }

        /// <summary>
        /// Load TP_Gain Table data to RichTextBox
        /// </summary>
        /// <param name="sErrorMessage">Error message when loading fails</param>
        /// <param name="sResultDirectoryPath">Result directory path</param>
        /// <returns>Returns whether the display is correct</returns>
        public bool LoadTPGainTableToRichTextBox(ref string sErrorMessage, string sResultDirectoryPath)
        {
            string sTPGainTableDirectoryPath = string.Format(@"{0}\TPGainTable", sResultDirectoryPath);

            if (Directory.Exists(sTPGainTableDirectoryPath) == false)
            {
                sErrorMessage = string.Format("TP Gain Table Folder Not Exist!");
                return false;
            }
            else
            {
                string[] sTPGainTableFilePath_Array = Directory.GetFiles(sTPGainTableDirectoryPath, "*.txt");

                foreach (string sFilePath in sTPGainTableFilePath_Array)
                {
                    StreamReader srTPGainTable = new StreamReader(sFilePath);
                    string sTPGainTableData = srTPGainTable.ReadToEnd();
                    srTPGainTable.Close();

                    string sTitleName = Path.GetFileNameWithoutExtension(sFilePath);

                    TabPage tpgTPGainTable = new TabPage(sTitleName);
                    RichTextBox rtbxTPGainTable = new RichTextBox();
                    rtbxTPGainTable.ReadOnly = true;
                    rtbxTPGainTable.Dock = DockStyle.Fill;
                    rtbxTPGainTable.WordWrap = false;
                    rtbxTPGainTable.Text = sTPGainTableData;
                    tpgTPGainTable.Controls.Add(rtbxTPGainTable);

                    this.Invoke((MethodInvoker)delegate
                    {
                        tcTPGTResult.TabPages.Add(tpgTPGainTable);
                    });
                }
            }

            return true;
        }

        /// <summary>
        /// Load Linearity Table data to RichTextBox
        /// </summary>
        /// <param name="sErrorMessage">Error message when loading fails</param>
        /// <param name="nLTTypeIndex">Index of the Linearity Table type</param>
        /// <param name="sResultDirectoryPath">Result directory path</param>
        /// <returns>Returns whether the display is correct</returns>
        public bool LoadLTTableToRichTextBox(ref string sErrorMessage, int nLTTypeIndex, string sResultDirectoryPath)
        {
            // Define the array of Linearity Table file names
            string[] sFileName_Array = new string[] 
            { 
                SpecificText.m_sRX5TLinearityTableTxtFile, 
                SpecificText.m_sTX5TLinearityTableTxtFile,
                SpecificText.m_sRX3TLinearityTableTxtFile, 
                SpecificText.m_sTX3TLinearityTableTxtFile,
                SpecificText.m_sRX2TLELinearityTableTxtFile, 
                SpecificText.m_sTX2TLELinearityTableTxtFile,
                SpecificText.m_sRX2THELinearityTableTxtFile, 
                SpecificText.m_sTX2THELinearityTableTxtFile 
            };

            // Define the array of corresponding TabPages
            TabPage[] tpgTable_Array = new TabPage[] 
            { 
                tpgLTRX5TTable, 
                tpgLTTX5TTable,
                tpgLTRX3TTable, 
                tpgLTTX3TTable,
                tpgLTRX2TLETable, 
                tpgLTTX2TLETable,
                tpgLTRX2THETable, 
                tpgTX2THETable 
            };

            // Define the array of corresponding RichTextBoxes
            RichTextBox[] rtbxTable_Array = new RichTextBox[] 
            { 
                rtbxRX5TTable, 
                rtbxTX5TTable,
                rtbxRX3TTable, 
                rtbxTX3TTable,
                rtbxRX2TLETable, 
                rtbxTX2TLETable,
                rtbxRX2THETable, 
                rtbxTX2THETable 
            };

            // Construct the file path of the Linearity Table
            string sLTTableFilePath = string.Format(@"{0}\{1}", sResultDirectoryPath, sFileName_Array[nLTTypeIndex]);

            if (File.Exists(sLTTableFilePath) == false)
            {
                sErrorMessage = string.Format("Table Data File({0}) Not Exist!", sFileName_Array[nLTTypeIndex]);
                return false;
            }

            // Display the Linearity Table data in the corresponding RichTextBox
            DisplayTableData(tcLTResult, tpgTable_Array[nLTTypeIndex], rtbxTable_Array[nLTTypeIndex], sLTTableFilePath);
            return true;
        }

        /// <summary>
        /// Display table data
        /// </summary>
        /// <param name="tcControl">TabControl control</param>
        /// <param name="tpgControl">TabPage control</param>
        /// <param name="rtbxControl">RichTextBox control</param>
        /// <param name="sTableFilePath">Table file path</param>
        private void DisplayTableData(TabControl tcControl, TabPage tpgControl, RichTextBox rtbxControl, string sTableFilePath)
        {
            // Read the content of the file
            StreamReader srLTTable = new StreamReader(sTableFilePath);
            string sLTTableData = srLTTable.ReadToEnd();
            srLTTable.Close();

            // Output the content to the RichTextBox control
            OutputRichTextBox(tcControl, tpgControl, rtbxControl, sLTTableData);
        }

        /// <summary>
        /// Converts a CSV file to DataTable format
        /// </summary>
        /// <param name="eSubStep">SubStep</param>
        /// <param name="sFilePath">CSV file path</param>
        /// <param name="sTableName">Name of the DataTable</param>
        /// <param name="sDelimiter">Delimiter used in the CSV file</param>
        /// <param name="bReadFWParameterFlag">Flag indicating whether to read FW parameters</param>
        /// <param name="bJustFWParameterFlag">Flag indicating whether only FW parameters are present</param>
        /// <param name="bReadRankDataFlag">Flag indicating whether to read rank data</param>
        /// <returns>The DataTable obtained</returns>
        public DataTable ConvertCsvToDataTable(SubTuningStep eSubStep, 
                                               string sFilePath, 
                                               string sTableName, 
                                               string sDelimiter, 
                                               bool bReadFWParameterFlag = false,
                                               bool bJustFWParameterFlag = false, 
                                               bool bReadRankDataFlag = false)
        {
            string sDataInfoMainTitle = "";
            string sFWParameterMainTitle = "FW Parameter Information List";

            string[] sTitleName_Array = new string[2] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName 
            };

            if (eSubStep == SubTuningStep.NO || ((eSubStep == SubTuningStep.TILTNO_PTHF || eSubStep == SubTuningStep.TILTNO_BHF) && bReadRankDataFlag == true))
                sDataInfoMainTitle = "Ranking Data(Weighting Ranking)";
            else if (eSubStep == SubTuningStep.TILTTUNING_BHF)
            {
                sDataInfoMainTitle = "Step Data Information List";
                sFWParameterMainTitle = "Ranking Data Information List";
            }
            else
                sDataInfoMainTitle = "Step Data Information List";

            if (bJustFWParameterFlag == true)
            {
                if (eSubStep == SubTuningStep.TILTTUNING_BHF)
                    sFWParameterMainTitle = "FW Parameter Information List";
            }

            DataTable datatableData = new DataTable();
            DataSet dsDataSet = new DataSet();
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            bool bReadRankingTitleFlag = false;
            string[] sColumnData_Array = null;

            try
            {
                while (bReadRankingTitleFlag == false)
                {
                    sColumnData_Array = srFile.ReadLine().Split(sDelimiter.ToCharArray());

                    if (sColumnData_Array == null)
                        continue;

                    if (bReadFWParameterFlag == false)
                    {
                        if (sColumnData_Array[0] == sDataInfoMainTitle)
                        {
                            sColumnData_Array = srFile.ReadLine().Split(sDelimiter.ToCharArray());
                            CheckMatchTitle(ref bReadRankingTitleFlag, sColumnData_Array, sTitleName_Array);
                        }
                    }
                    else
                    {
                        if (sColumnData_Array[0] == sFWParameterMainTitle)
                        {
                            sColumnData_Array = srFile.ReadLine().Split(sDelimiter.ToCharArray());
                            CheckMatchTitle(ref bReadRankingTitleFlag, sColumnData_Array, sTitleName_Array);
                        }
                    }
                }

                dsDataSet.Tables.Add(sTableName);

                foreach (string sColumnData in sColumnData_Array)
                {
                    bool bAddFlag = false;
                    string sNextText = "";
                    int nNextIndex = 0;

                    while (bAddFlag == false)
                    {
                        string sColumnName = sColumnData + sNextText;
                        sColumnName = sColumnName.Replace("#", "");
                        sColumnName = sColumnName.Replace("'", "");
                        sColumnName = sColumnName.Replace("&", "");

                        if (dsDataSet.Tables[sTableName].Columns.Contains(sColumnName) == false)
                        {
                            dsDataSet.Tables[sTableName].Columns.Add(sColumnName);
                            bAddFlag = true;
                        }
                        else
                        {
                            nNextIndex++;
                            sNextText = string.Format("_{0}", nNextIndex);
                        }
                    }
                }

                string sAllData = srFile.ReadToEnd();
                sAllData = sAllData.Replace("\r", "");
                string[] sRowData_Array = sAllData.Split("\n".ToCharArray());
                int nDataWidthCount = dsDataSet.Tables[sTableName].Columns.Count;

                foreach (string sRowData in sRowData_Array)
                {
                    if (sRowData == "")
                        break;

                    string[] sItem_Array = sRowData.Split(sDelimiter.ToCharArray());

                    if (sItem_Array.Length != nDataWidthCount)
                        break;

                    dsDataSet.Tables[sTableName].Rows.Add(sItem_Array);
                }

                srFile.Close();

                datatableData = dsDataSet.Tables[0];

                return datatableData;
            }
            finally
            {
                srFile.Close();
            }
        }

        /// <summary>
        /// Checks if the column titles match the expected titles
        /// </summary>
        /// <param name="bReadRankTitleFlag">Flag indicating whether the rank title has been read</param>
        /// <param name="sColumnData_Array">Array of column data</param>
        /// <param name="sTitleName_Array">Array of expected title names</param>
        private void CheckMatchTitle(ref bool bReadRankTitleFlag, string[] sColumnData_Array, string[] sTitleName_Array)
        {
            // If the number of column data is greater than or equal to the number of expected title names
            if (sColumnData_Array.Length >= sTitleName_Array.Length)
            {
                int nMatchFlag = 0;

                // Compare each title name with the corresponding column data name
                for (int nTitleIndex = 0; nTitleIndex < sTitleName_Array.Length; nTitleIndex++)
                {
                    if (sTitleName_Array[nTitleIndex] == sColumnData_Array[nTitleIndex])
                        nMatchFlag++;
                }

                // If all titles match, set the flag to indicate that the rank title has been read
                if (nMatchFlag == sTitleName_Array.Length)
                    bReadRankTitleFlag = true;
            }
        }

        /// <summary>
        /// Writes the cost time information of the main steps (MainStep) to a file
        /// </summary>
        public void WriteMainStepCostTimeInfo()
        {
            // Get the file path for the cost time file
            string sCostTimeFilePath = string.Format(@"{0}\CostTime.txt", m_sRecordDirectoryPath);

            // Create a file stream object
            FileStream fs = new FileStream(sCostTimeFilePath, FileMode.Create);
            // Create a stream writer object
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                // Pad the hours, minutes, and seconds with leading zeros
                string sTotalHour = m_nTotalHours.ToString().PadLeft(2, '0');
                string sTotalMinute = m_nTotalMinutes.ToString().PadLeft(2, '0');
                string sTotalSecond = m_nTotalSeconds.ToString().PadLeft(2, '0');

                // Write the total cost time
                sw.WriteLine(string.Format("Total Cost Time = {0}hr:{1}m:{2}s", sTotalHour, sTotalMinute, sTotalSecond));

                sw.WriteLine();

                // Write the cost time for each individual main step
                sw.WriteLine("Single Main Step Cost Time");

                for (int nIndex = 0; nIndex < m_cMainStepCostTimeInfo_List.Count; nIndex++)
                    sw.WriteLine(string.Format("{0} = {1}", m_cMainStepCostTimeInfo_List[nIndex].m_sMainStepName, m_cMainStepCostTimeInfo_List[nIndex].m_sMainStepCostTime));
            }
            finally
            {
                // Flush the buffer and close the stream writer and file stream
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// Event triggered when the "Step Setting" item in the toolbar is clicked
        /// </summary>
        /// <param name="sender">The control that triggered the event</param>
        /// <param name="e">The event arguments</param>
        private void toolstripmenuitemStepSetting_Click(object sender, EventArgs e)
        {
            // Create a Step Setting window object
            frmStepSetting cfrmStepSetting = new frmStepSetting();
            // Set the Step Setting window to be topmost
            cfrmStepSetting.TopMost = true;

            // Calculate the position for the Step Setting window to be displayed
            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmStepSetting.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmStepSetting.Height / 2);

            // If the current window is a child window, display it in the center of the parent window
            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmStepSetting.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmStepSetting.Height / 2);
            }

            // If the parent object of the Step Setting window is the main form, display it in the center of the main form
            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmStepSetting.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmStepSetting.Height / 2);
            }

            // Set the start position and location of the Step Setting window
            cfrmStepSetting.StartPosition = FormStartPosition.Manual;
            cfrmStepSetting.Location = new Point(nLocationX, nLocationY);

            // Display the Step Setting window, if the user clicks the "Cancel" button, exit the method
            if (cfrmStepSetting.ShowDialog() == DialogResult.Cancel)
                return;

            // Create step items based on the Step Setting parameters
            CreateStepItem(ParamAutoTuning.m_cStepSettingParameter_Array);
            // Create flow steps
            CreateFlowStep();
            // Create result tab controls
            CreateResultTabControl(m_bStartExecuteFlag);

            // Enable the "Start" button if the execution mode is Load Data mode
            bool bLoadDataModeFlag = (m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA) ? true : false;
            btnNewStart.Enabled = bLoadDataModeFlag;
        }

        // <summary>
        /// Event triggered when the "Flow Setting" item in the toolbar is clicked
        /// </summary>
        /// <param name="sender">The control that triggered the event</param>
        /// <param name="e">The event arguments</param>
        private void toolstripmenuitemFlowSetting_Click(object sender, EventArgs e)
        {
            // Display the Flow Setting window
            ShowFlowSettingWindow();
        }

        /// <summary>
        /// Event triggered when the "Parameter Setting" item in the toolbar is clicked
        /// </summary>
        /// <param name="sender">The control that triggered the event</param>
        /// <param name="e">The event arguments</param>
        private void toolstripmenuitemParameterSetting_Click(object sender, EventArgs e)
        {
            // Set the mode flag
            int nModeFlag = SetModeFlag();

            /// Set the mode state of the Parameter Setting form and display the parameter setting
            m_cfrmParameterSetting.SetModeState(nModeFlag);
            m_cfrmParameterSetting.DispalyParameterSetting(m_bObjectEnableFlag);

            // Calculate the display position of the Parameter Setting form
            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(m_cfrmParameterSetting.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(m_cfrmParameterSetting.Height / 2);

            // If the current form is an MDI child form, calculate the display position using the MDI parent form's position
            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(m_cfrmParameterSetting.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(m_cfrmParameterSetting.Height / 2);
            }

            // If the current form has a parent form, calculate the display position using the parent form's position
            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(m_cfrmParameterSetting.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(m_cfrmParameterSetting.Height / 2);
            }

            // Set the start position of the Parameter Setting form
            m_cfrmParameterSetting.StartPosition = FormStartPosition.Manual;
            m_cfrmParameterSetting.Location = new Point(nLocationX, nLocationY);

            // Show the Parameter Setting form
            m_cfrmParameterSetting.ShowDialog();
            //if (m_cfrmParameterSetting.ShowDialog() == DialogResult.Cancel)
            //    m_cfrmParameterSetting.SetParameter();

            //m_cfrmParameterSetting.Show();
        }

        /// <summary>
        /// Event triggered when the "Frequency Setting" item in the toolbar is clicked
        /// </summary>
        /// <param name="sender">The control that triggered the event</param>
        /// <param name="e">The event arguments</param>
        private void toolstripmenuitemFrequencySetting_Click(object sender, EventArgs e)
        {
            // Create a Frequency Setting form object
            frmFrequencySetting cfrmFrequencySetting = new frmFrequencySetting(this);

            // Calculate the display position of the Frequency Setting form
            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmFrequencySetting.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmFrequencySetting.Height / 2);

            // If the main form is an MDI child form, calculate the display position using the center of the MDI window
            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmFrequencySetting.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmFrequencySetting.Height / 2);
            }

            // If the main form is a child form of another form, calculate the display position using the center of the parent form
            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmFrequencySetting.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmFrequencySetting.Height / 2);
            }

            // Set the start position of the Frequency Setting form
            cfrmFrequencySetting.StartPosition = FormStartPosition.Manual;
            cfrmFrequencySetting.Location = new Point(nLocationX, nLocationY);

            // Show the Frequency Setting form
            cfrmFrequencySetting.Show();
        }

        /// <summary>
        /// Checks if the flow file format is correct and returns the result
        /// </summary>
        /// <param name="cFlowStep">The current FlowStep</param>
        /// <returns>The confirmation result flag</returns>
        public int CheckFlowFileFormat(FlowStep cFlowStep)
        {
            // Create a CheckFlowFileInfo object for the check
            CheckFlowFileInfo cCheckFlowFileInfo = new CheckFlowFileInfo(cFlowStep, this);
            // Perform the check and return the result
            return cCheckFlowFileInfo.MainFlow();
        }

        /// <summary>
        /// Event triggered when the Chart button is clicked
        /// </summary>
        /// <param name="sender">The control that triggered the event</param>
        /// <param name="e">The event arguments</param>
        private void btnChart_Click(object sender, EventArgs e)
        {
            // Write debug log: Clicked Chart button
            WriteDebugLog("-Click Chart Button");
            // Get the data folder path
            string sDataFolderPath = btnChart.Tag.ToString();

            // If there is an error result, read the backup (ErrorBackUp) file path
            if (m_bResultErrorFlag == true)
                sDataFolderPath = string.Format(@"{0}\{1}\{2}", m_sLogDirecotryPath, SpecificText.m_sErrorBackUpText, m_sRecordDirectoryName);

            // If the data folder does not exist, show a message and exit the method
            if (Directory.Exists(sDataFolderPath) == false)
            {
                ShowMessageBox("File Directory Not Exist!!");
                return;
            }

            // Create the Result Chart window
            frmResultChart cfrmResultChart = new frmResultChart(this);

            // Set the window's horizontal position
            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmResultChart.Width / 2);
            // Set the window's vertical position
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmResultChart.Height / 2);

            // If this window is a child window, adjust the horizontal and vertical positions
            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmResultChart.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmResultChart.Height / 2);
            }

            // If there is a parent form, adjust the horizontal and vertical positions
            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmResultChart.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmResultChart.Height / 2);
            }

            // Set the window's start position
            cfrmResultChart.StartPosition = FormStartPosition.Manual;
            // Set the window's location
            cfrmResultChart.Location = new Point(nLocationX, nLocationY);

            // Set the data folder path for the window
            cfrmResultChart.DataFolderPath = sDataFolderPath;
            // Show the window
            cfrmResultChart.Visible = true;
        }

        /// <summary>
        /// Event triggered when the mouse enters the Chart button
        /// </summary>
        /// <param name="sender">The control that triggered the event</param>
        /// <param name="e">The event arguments</param>
        private void btnChart_MouseEnter(object sender, EventArgs e)
        {
            // Set the button's border size to 1
            btnChart.FlatAppearance.BorderSize = 1;
            // Set the button's border color to blue
            btnChart.FlatAppearance.BorderColor = Color.Blue;
            // Set the button's background color to light yellow
            btnChart.BackColor = Color.LightYellow;
        }

        /// <summary>
        /// Event triggered when the mouse leaves the Chart button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnChart_MouseLeave(object sender, EventArgs e)
        {
            // Set the button's border size to 0 to remove the border
            btnChart.FlatAppearance.BorderSize = 0;
            // Set the button's border color to blue
            btnChart.FlatAppearance.BorderColor = Color.Blue;
            // Set the button's background color to the default control color
            btnChart.BackColor = SystemColors.Control;
        }

        /// <summary>
        /// Retrieves the EDID (Extended Display Identification Data)
        /// </summary>
        /// <returns>The EDID data as byte array</returns>
        private byte[] GetEDID()
        {
            int nErrorCode = 0;
            Process pGetEDIDProcess = new Process();
            ProcessStartInfo psiGetEDIDProcStartInfo = new ProcessStartInfo();

            // Set the path to the GetEDID.exe program and the window display style
            pGetEDIDProcess.StartInfo.FileName = string.Format(@"{0}\GetEDID.exe", Application.StartupPath);
            pGetEDIDProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                // Execute the GetEDID.exe program
                pGetEDIDProcess.Start();
                pGetEDIDProcess.WaitForExit();
                nErrorCode = pGetEDIDProcess.ExitCode;
            }
            catch
            {
                // If an exception occurs during execution, set the error code to -999 and return null
                nErrorCode = -999;
                return null;
            }

            // Open the EDID Data File
            string sEDIDName = string.Format(@"{0}\EDID.txt", Application.StartupPath);

            if (File.Exists(sEDIDName) == false)
                return null;

            FileStream fsEDIDFile = File.Open(sEDIDName, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] byteReadData_Array = new byte[fsEDIDFile.Length];
            fsEDIDFile.Read(byteReadData_Array, 0, (int)fsEDIDFile.Length);

            return byteReadData_Array;
        }

        /// <summary>
        /// Retrieves screen size information from the EDID file
        /// </summary>
        private void GetScreenSizeFromEDID()
        {
            // Read the XML file that stores screen size information
            m_datatableScreenSize.ReadXml(m_sScreenListXMLPath);
            // Get the data row with the name "AutoSet"
            DataRow[] datarowScreenSize_Array = m_datatableScreenSize.Select("Name = 'AutoSet'");

            bool bAutoSetFlag = false;

            // Check if it is in auto-set mode
            for (int nDataIndex = 0; nDataIndex < datarowScreenSize_Array.Length; nDataIndex++)
            {
                int nRowIndex = m_datatableScreenSize.Rows.IndexOf(datarowScreenSize_Array[nDataIndex]);

                if (ParamAutoTuning.m_nScreenIndex == nRowIndex)
                    bAutoSetFlag = true;
            }

            // Get the content of the EDID file
            string sGetEDID = "-Get EDID :";

            for (int nByteIndex = 0; nByteIndex < m_byteEDIDData_Array.Length; nByteIndex++)
                sGetEDID += string.Format(" {0}", m_byteEDIDData_Array[nByteIndex].ToString("X2"));

            WriteDebugLog(sGetEDID);
            WriteDebugLog(string.Format("-EDID Length : {0}", m_byteEDIDData_Array.Length));

            // Parse the content of the EDID file and retrieve the screen size
            if (m_byteEDIDData_Array != null && m_byteEDIDData_Array.Length != 0)
            {
                byte byteWidthHighByte = (byte)((m_byteEDIDData_Array[68] & 0xF0) >> 4);
                byte byteWidthLowByte = m_byteEDIDData_Array[66];

                byte byteHeightHighByte = (byte)(m_byteEDIDData_Array[68] & 0x0F);
                byte byteHeightLowByte = m_byteEDIDData_Array[67];

                int nEDID_Width = (ushort)(byteWidthHighByte << 8 | byteWidthLowByte);
                int nEDID_Height = (ushort)(byteHeightHighByte << 8 | byteHeightLowByte);
                float fLength = (float)((Math.Sqrt(nEDID_Width * nEDID_Width + nEDID_Height * nEDID_Height)) / 10 / 2.54);

                // Store the parsed screen size information in the m_cEDIDInformation structure
                m_cEDIDInformation.nEDIDInfo_Width = nEDID_Width;
                m_cEDIDInformation.nEDIDInfo_Height = nEDID_Height;
                m_cEDIDInformation.dScreenSize = ElanConvert.SetDoubleValueRoundUp(fLength, 1);

                bool bAutoSetItemMatchFlag = false;

                // Check if there is a matching auto-set screen size data row
                for (int nDataIndex = 0; nDataIndex < datarowScreenSize_Array.Length; nDataIndex++)
                {
                    int nRowIndex = m_datatableScreenSize.Rows.IndexOf(datarowScreenSize_Array[nDataIndex]);

                    if (ParamAutoTuning.m_nScreenIndex == nRowIndex)
                    {
                        ParamAutoTuning.m_dScreenWidth = m_cEDIDInformation.nEDIDInfo_Width;
                        ParamAutoTuning.m_dScreenHeight = m_cEDIDInformation.nEDIDInfo_Height;
                        m_cAPsetting.m_fScreenX = m_cEDIDInformation.nEDIDInfo_Width;
                        m_cAPsetting.m_fScreenY = m_cEDIDInformation.nEDIDInfo_Height;
                        m_sScreenSize = string.Format("{0}\"", m_cEDIDInformation.dScreenSize);
                        bAutoSetItemMatchFlag = true;
                    }
                }

                if (bAutoSetItemMatchFlag == false)
                    m_sScreenSize = Convert.ToString(m_datatableScreenSize.Rows[ParamAutoTuning.m_nScreenIndex]["Name"]);
            }
            else
            {
                if (bAutoSetFlag == true)
                {
                    ParamAutoTuning.m_nScreenIndex = 0;
                    ParamAutoTuning.m_dScreenWidth = Convert.ToDouble(m_datatableScreenSize.Rows[0]["Width"]);
                    ParamAutoTuning.m_dScreenHeight = Convert.ToDouble(m_datatableScreenSize.Rows[0]["Height"]);
                    m_sScreenSize = Convert.ToString(m_datatableScreenSize.Rows[0]["Name"]);
                }
            }
        }

        /// <summary>
        /// Creates step items
        /// </summary>
        /// <param name="cStepItem_Array">Array of step items</param>
        private void CreateStepItem(ParamAutoTuning.StepSettingParameter[] cStepItem_Array)
        {
            // If step items already exist, remove the original ones
            if (m_lblTestItem_Array != null)
            {
                foreach (Label lblControl in m_lblTestItem_Array)
                    pnlStep.Controls.Remove(lblControl);

                m_lblTestItem_Array = null;
            }

            // Gap between step items
            int nGap = 1;
            // Y-coordinate of the first step item
            int nYStartPosition = lblStepItem.Top;
            // Index of the step item
            int nIndex = 0;

            // Create an array of Label for step items
            m_lblTestItem_Array = new Label[cStepItem_Array.Length];

            // Create step items
            for (int nItemIndex = 0; nItemIndex < cStepItem_Array.Length; nItemIndex++)
            {
                bool bSetLabelFlag = false;

                // If in Server mode, only create step items controlled by the Server
                if (m_nModeFlag == MainConstantParameter.m_nMODE_SERVER)
                {
                    if (cStepItem_Array[nItemIndex].m_eTuningStep == MainTuningStep.SERVERCONTRL)
                        bSetLabelFlag = true;
                }
                // If in Client, Single, GoDraw, or LoadData mode, and the step item is enabled, create the step item
                else if (bSetLabelFlag == true ||
                         ((m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                           m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                           m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW ||
                           m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA) && cStepItem_Array[nItemIndex].m_bEnable == true))
                {
                    m_lblTestItem_Array[nIndex] = new Label();
                    Label lblControl = m_lblTestItem_Array[nIndex];
                    lblControl.Name = cStepItem_Array[nItemIndex].m_sStepName;
                    lblControl.Text = cStepItem_Array[nItemIndex].m_sStepName;

                    lblControl.Left = 5;
                    lblControl.Top = nIndex * (lblStepItem.Height + nGap) + nYStartPosition;
                    lblControl.BorderStyle = BorderStyle.Fixed3D;
                    lblControl.AutoSize = false;
                    lblControl.Width = lblStepItem.Width;
                    lblControl.Height = lblStepItem.Height;
                    lblControl.TextAlign = ContentAlignment.MiddleCenter;
                    lblControl.Tag = cStepItem_Array[nItemIndex].m_eTuningStep;
                    lblControl.BackColor = SystemColors.Control;
                    lblControl.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
                    lblControl.Font = new Font("Times New Roman", 12);
                    pnlStep.Controls.Add(lblControl);
                    lblControl.Show();

                    nIndex++;
                }
            }
        }

        /// <summary>
        /// Sets the file directory path and creates the corresponding folder
        /// </summary>
        /// <param name="bLoadDataModeFlag">Flag indicating whether it is in LoadData mode</param>
        public void SetFileDirectory(bool bLoadDataModeFlag)
        {
            if (Directory.Exists(m_sLogDirecotryPath) == false)
                Directory.CreateDirectory(m_sLogDirecotryPath);

            // Set the folder name as the project name and the start flow time
            string sDirectoryName = string.Format("{0}_{1}", ParamAutoTuning.m_sProjectName, m_sStartFlowTime);

            // If the project name is an empty string, use only the start flow time as the folder name
            if (ParamAutoTuning.m_sProjectName == "")
                sDirectoryName = m_sStartFlowTime;

            // If in LoadData mode, append "_LoadData" to the folder name
            if (bLoadDataModeFlag == true)
                sDirectoryName = string.Format("{0}_LoadData", sDirectoryName);

            // Set the file directory path and record the folder name
            m_sFileDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirecotryPath, sDirectoryName);
            m_sRecordDirectoryName = sDirectoryName;
            WriteDebugLog("-Record Folder Name = " + sDirectoryName);

            // Set the recorded project name
            m_sRecordProjectName = ParamAutoTuning.m_sProjectName;

            // Create the directory if it doesn't exist
            if (Directory.Exists(m_sFileDirectoryPath) == false)
                Directory.CreateDirectory(m_sFileDirectoryPath);
        }

        /// <summary>
        /// Retrieves the original directory information
        /// </summary>
        /// <param name="nDataNumber">The number of data files</param>
        /// <param name="sProjectName">The project name</param>
        /// <returns>True if the directory information was successfully obtained; otherwise, false</returns>
        private bool GetOriginalDirectoryInfo(ref int nDataNumber, ref string sProjectName)
        {
            bool bProjectNameFileFlag = false;

            // Search for all .txt files in the default folder
            foreach (string sFilePath in Directory.GetFiles(m_sDefaultFolderPath, "*.txt"))
            {
                string sLine = "";
                string sFileNameWithExtension = Path.GetFileName(sFilePath);

                // If it is the ProjectName.txt file
                if (sFileNameWithExtension == "ProjectName.txt")
                {
                    StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                    // Read each line in the file
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        // If the line contains the "ProjectName=" string
                        if (sLine.Contains("ProjectName=") == true)
                        {
                            // Split the line into a string array using commas and equal signs as separators
                            string[] sSplit_Array = sLine.Split(new char[2] { '=', ',' });

                            // Find the "ProjectName" string in the array and retrieve its value
                            for (int nSplitIndex = 0; nSplitIndex < sSplit_Array.Length; nSplitIndex++)
                            {
                                if (sSplit_Array[nSplitIndex].Replace(" ", "") == "ProjectName")
                                    sProjectName = sSplit_Array[nSplitIndex + 1];
                            }

                            // Mark that ProjectName was found
                            bProjectNameFileFlag = true;
                            break;
                        }
                    }

                    srFile.Close();

                    // If ProjectName was found, continue to the next file
                    if (bProjectNameFileFlag == true)
                        continue;
                }

                // If it is not ProjectName.txt, increment the data file count
                nDataNumber++;
            }

            return bProjectNameFileFlag;
        }

        /// <summary>
        /// Removes all flow files associated with the given flow step
        /// </summary>
        /// <param name="cFlowStep">The current flow step</param>
        public void DeleteFlowFile(FlowStep cFlowStep)
        {
            SubTuningStep[] eSubStep_Array = null;

            if (cFlowStep.m_eMainStep == MainTuningStep.NO)
            {
                eSubStep_Array = new SubTuningStep[] 
                { 
                    SubTuningStep.TILTNO_PTHF, 
                    SubTuningStep.TILTNO_BHF,
                    SubTuningStep.DIGIGAIN,
                    SubTuningStep.TP_GAIN,
                    SubTuningStep.PCHOVER_1ST, 
                    SubTuningStep.PCHOVER_2ND, 
                    SubTuningStep.PCCONTACT,
                    SubTuningStep.HOVER_1ST, 
                    SubTuningStep.HOVER_2ND, 
                    SubTuningStep.CONTACT,
                    SubTuningStep.HOVERTRxS, 
                    SubTuningStep.CONTACTTRxS,
                    SubTuningStep.TILTTUNING_PTHF, 
                    SubTuningStep.TILTTUNING_BHF,
                    SubTuningStep.PRESSURESETTING, 
                    SubTuningStep.PRESSURETABLE,
                    SubTuningStep.LINEARITYTABLE,
                    SubTuningStep.ELSE 
                };
            }
            else if (cFlowStep.m_eMainStep == MainTuningStep.TILTNO)
            {
                /*
                eSubStep_Array = new SubTuningStep[] 
                { 
                    SubTuningStep.TILTTUNING_PTHF, 
                    SubTuningStep.TILTTUNING_BHF,
                    SubTuningStep.ELSE 
                };
                */

                eSubStep_Array = new SubTuningStep[] 
                { 
                    SubTuningStep.ELSE 
                };
            }
            else if (cFlowStep.m_eMainStep == MainTuningStep.DIGIGAINTUNING)
            {
                eSubStep_Array = new SubTuningStep[] 
                { 
                    SubTuningStep.ELSE 
                };
            }
            else if (cFlowStep.m_eMainStep == MainTuningStep.TPGAINTUNING)
            {
                eSubStep_Array = new SubTuningStep[] 
                { 
                    SubTuningStep.ELSE 
                };
            }
            else if (cFlowStep.m_eMainStep == MainTuningStep.DIGITALTUNING)
            {
                eSubStep_Array = new SubTuningStep[] 
                { 
                    SubTuningStep.CONTACT, 
                    SubTuningStep.HOVERTRxS,
                    SubTuningStep.CONTACTTRxS,
                    SubTuningStep.TILTTUNING_PTHF, 
                    SubTuningStep.TILTTUNING_BHF,
                    SubTuningStep.PRESSURESETTING, 
                    SubTuningStep.PRESSURETABLE,
                    SubTuningStep.LINEARITYTABLE,
                    SubTuningStep.ELSE 
                };
            }
            else
                return;

            if (Directory.Exists(m_sFlowDirectoryPath) == false)
                return;

            string sFlowBackUpDirectoryPath = string.Format(@"{0}\{1}", m_sFlowDirectoryPath, SpecificText.m_sBackUpText);

            if (Directory.Exists(sFlowBackUpDirectoryPath) == true)
            {
                Directory.Delete(sFlowBackUpDirectoryPath, true);

                while (Directory.Exists(sFlowBackUpDirectoryPath))
                    Thread.Sleep(10);
            }

            Directory.CreateDirectory(sFlowBackUpDirectoryPath);

            foreach (string sFilePath in Directory.GetFiles(m_sFlowDirectoryPath, "*.txt"))
            {
                string sFileNameWithExtension = Path.GetFileName(sFilePath);

                for (int nStepIndex = 0; nStepIndex < eSubStep_Array.Length; nStepIndex++)
                {
                    int nSubStepIndex = Convert.ToInt32(eSubStep_Array[nStepIndex]);

                    if (sFileNameWithExtension == m_sSubTuningStepFileName_Array[nSubStepIndex])
                    {
                        string sDestinationFilePath = string.Format(@"{0}\{1}", sFlowBackUpDirectoryPath, sFileNameWithExtension);
                        File.Move(sFilePath, sDestinationFilePath);
                        break;
                    }
                }

                if (cFlowStep.m_eMainStep == MainTuningStep.NO)
                {
                    if (sFileNameWithExtension == m_sTotalRankFileName)
                    {
                        string sDestinationFilePath = string.Format(@"{0}\{1}", sFlowBackUpDirectoryPath, sFileNameWithExtension);
                        File.Move(sFilePath, sDestinationFilePath);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the Result directory
        /// </summary>
        private void RemoveResultDirectory()
        {
            // If the result directory exists, delete the directory and move it to the error backup directory
            if (m_sFileDirectoryPath != "" && Directory.Exists(m_sFileDirectoryPath) == true)
            {
                try
                {
                    string sErrorBackUpDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirecotryPath, SpecificText.m_sErrorBackUpText);

                    // If the error backup directory does not exist, create the directory
                    if (Directory.Exists(sErrorBackUpDirectoryPath) == false)
                        Directory.CreateDirectory(sErrorBackUpDirectoryPath);

                    // Wait for 10 milliseconds until the error backup directory is successfully created
                    while (Directory.Exists(sErrorBackUpDirectoryPath) == false)
                        Thread.Sleep(10);

                    // Move the directory to the error backup directory
                    string sDestinationDirectoryPath = string.Format(@"{0}\{1}", sErrorBackUpDirectoryPath, m_sRecordDirectoryName);
                    Directory.Move(m_sFileDirectoryPath, sDestinationDirectoryPath);
                    m_sRecordDirectoryPath = sDestinationDirectoryPath;
                }
                catch
                {
                    // Handle any exceptions that occur during the process
                }
            }
        }

        /// <summary>
        /// Sets the usage mode
        /// </summary>
        /// <returns>The usage mode flag</returns>
        private int SetModeFlag()
        {
            // Initialize the mode flag as -1
            int nModeFlag = -1;

            // Check if it is in Server mode
            if (CheckModeState(MainConstantParameter.m_sMODE_SERVER) == true)
                nModeFlag = MainConstantParameter.m_nMODE_SERVER;
            // Check if it is in Client mode
            else if (CheckModeState(MainConstantParameter.m_sMODE_CLIENT) == true)
                nModeFlag = MainConstantParameter.m_nMODE_CLIENT;
            // Check if it is in GoDraw mode
            else if (CheckModeState(MainConstantParameter.m_sMODE_GODRAW) == true)
                nModeFlag = MainConstantParameter.m_nMODE_GODRAW;
            // Check if it is in Single mode
            else if (CheckModeState(MainConstantParameter.m_sMODE_SINGLE) == true)
                nModeFlag = MainConstantParameter.m_nMODE_SINGLE;
            // Check if it is in LoadData mode
            else if (CheckModeState(MainConstantParameter.m_sMODE_LOADDATA) == true)
                nModeFlag = MainConstantParameter.m_nMODE_LOADDATA;

            return nModeFlag;
        }

        /// <summary>
        /// Filters multiple clicks
        /// </summary>
        private void FilterMultiClick()
        {
            // Wait for 500 milliseconds
            Thread.Sleep(500);
            // Set the multi-click flag to false
            m_bMultiClick = false;
        }

        /// <summary>
        /// Event handler triggered when the selected index of the tcTTResult control changes
        /// </summary>
        /// <param name="sender">The control that raised the event</param>
        /// <param name="e">The event arguments</param>
        private void tcTTResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When the selected tab is tpgTTRankResult
            if (tcTTResult.SelectedTab == tpgTTRank)
            {
                // Array to store value parameter names and error parameter names
                string[] sValueParameterName_Array = new string[] 
                { 
                    SpecificText.m_sRanking, 
                    SpecificText.m_sPTHF_H, 
                    SpecificText.m_sPTHF_V, 
                    SpecificText.m_sBHF_H, 
                    SpecificText.m_sBHF_V, 
                    SpecificText.m_sTotalScore 
                };

                string[] sErrorParameterName_Array = new string[] 
                { 
                    SpecificText.m_sResult, 
                    SpecificText.m_sPTHFExceptionMessage, 
                    SpecificText.m_sBHFExceptionMessage 
                };

                // For each value parameter name (sValueParameterName)
                foreach (string sValueParameterName in sValueParameterName_Array)
                {
                    // If the dgvTTRank table contains the column
                    if (dgvTTRank.Columns.Contains(sValueParameterName))
                    {
                        // For each cell in dgvTTRank
                        for (int nRowIndex = 0; nRowIndex < dgvTTRank.Rows.Count; nRowIndex++)
                        {
                            // If the cell value is "N/A", set the text color to red
                            if (dgvTTRank[sValueParameterName, nRowIndex].Value.ToString() == "N/A")
                                dgvTTRank[sValueParameterName, nRowIndex].Style.ForeColor = Color.Red;
                        }
                    }
                }

                // For each error parameter name (sErrorParameterName)
                foreach (string sErrorParameterName in sErrorParameterName_Array)
                {
                    // If the dgvTTRank table contains the column
                    if (dgvTTRank.Columns.Contains(sErrorParameterName))
                    {
                        // For each cell in dgvTTRank
                        for (int nRowIndex = 0; nRowIndex < dgvTTRank.Rows.Count; nRowIndex++)
                        {
                            // If the cell value is not an empty string, set the text color to red
                            if (dgvTTRank[sErrorParameterName, nRowIndex].Value.ToString() != "")
                                dgvTTRank[sErrorParameterName, nRowIndex].Style.ForeColor = Color.Red;
                        }
                    }
                }

                dgvTTRank.AutoResizeColumns();
            }
        }

        /// <summary>
        /// Event handler triggered when the selected index of the tcResult control changes
        /// </summary>
        /// <param name="sender">The control that raised the event</param>
        /// <param name="e">The event arguments</param>
        private void tcResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Convert the event sender to a TabControl object
            TabControl tcControl = (TabControl)sender;

            // If no TabPage is selected, return directly
            if (tcControl.SelectedTab == null)
                return;

            // Set the DataGridView format and foreground color based on the selected TabPage
            SetDgvFormatAndForeColor(tcControl.SelectedTab);
        }

        /// <summary>
        /// Event handler triggered when the selected index of the tcTNResult control changes
        /// </summary>
        /// <param name="sender">The control that raised the event</param>
        /// <param name="e">The event arguments</param>
        private void tcTNResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Convert the event sender to a TabControl object
            TabControl tcControl = (TabControl)sender;

            // If no TabPage is selected, return directly
            if (tcControl.SelectedTab == null)
                return;

            // Set the DataGridView format and foreground color based on the tpgTNResult TabPage
            SetDgvFormatAndForeColor(tpgTNResult);
        }

        /// <summary>
        /// Set the format and font color of the DataGridView based on the given TabPage parameter
        /// </summary>
        /// <param name="tpgControl">TabPage</param>
        private void SetDgvFormatAndForeColor(TabPage tpgControl)
        {
            DataGridView[] dgvControl_Array = null;

            // If the given TabPage is the Noise result page
            if (tpgControl.Name == tpgNoiseResult.Name)
            {
                // Set the format and font color of the Noise result DataGridView
                SetDgvFormatAndForeColor(dgvNoiseRank);
            }
            // If the given TabPage is the TiltNoise result page
            else if (tpgControl.Name == tpgTNResult.Name)
            {
                if (tcTNResult.SelectedTab == null)
                    return;

                // Depending on the selected TabPage, set the corresponding DataGridView format and font color
                dgvControl_Array = new DataGridView[] 
                { 
                    dgvTNBHFRank, 
                    dgvTNPTHFRank, 
                    dgvTNTotalRank 
                };

                foreach (DataGridView dgvControl in dgvControl_Array)
                {
                    if (tcTNResult.SelectedTab.Contains(dgvControl) == true)
                        SetDgvFormatAndForeColor(dgvControl);
                }
            }
        }

        /// <summary>
        /// Set the format and font color of the DataGridView based on the given DataGridView parameter
        /// </summary>
        /// <param name="dgvControl">DataGridView control</param>
        private void SetDgvFormatAndForeColor(DataGridView dgvControl)
        {
            // Set the column sorting mode to not sortable and align the text of the last column to the left
            for (int nColumnIndex = 0; nColumnIndex < dgvControl.Columns.Count; nColumnIndex++)
            {
                dgvControl.Columns[nColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;

                if (nColumnIndex == dgvControl.Columns.Count - 1)
                    dgvControl.Columns[nColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            // If the table contains the SpecificText.m_sException_Message column, set its text color to red
            if (dgvControl.Columns.Contains(SpecificText.m_sException_Message) == true)
            {
                for (int nRowIndex = 0; nRowIndex < dgvControl.Rows.Count; nRowIndex++)
                {
                    if (dgvControl[SpecificText.m_sException_Message, nRowIndex].Value.ToString() != "")
                    {
                        if (dgvControl[SpecificText.m_sException_Message, nRowIndex].Value.ToString().Contains("Max Value") == true && dgvControl[SpecificText.m_sException_Message, nRowIndex].Value.ToString().Contains("Over Warning") == true)
                            dgvControl[SpecificText.m_sException_Message, nRowIndex].Style.ForeColor = Color.DarkOrange;
                        else
                            dgvControl[SpecificText.m_sException_Message, nRowIndex].Style.ForeColor = Color.Red;
                    }
                }
            }

            // Resize the column widths to fit the content
            dgvControl.AutoResizeColumns();
        }

        /// <summary>
        /// Event triggered when the "GoDraw Controller" item in the toolbar is clicked.
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event arguments</param>
        private void toolstripmenuitemGoDrawController_Click(object sender, EventArgs e)
        {
            // Create an instance of the GoDraw Controller form
            frmGoDrawController cfrmGoDrawController = new frmGoDrawController(this);
            // cfrmGoDrawController.TopMost = true;

            // Set the position of the form
            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmGoDrawController.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmGoDrawController.Height / 2);

            // If the current form is a child form, adjust the position relative to the main form
            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmGoDrawController.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmGoDrawController.Height / 2);
            }

            // If the parent form reference is needed, adjust the position relative to the parent form
            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmGoDrawController.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmGoDrawController.Height / 2);
            }

            // Set the form's starting position and display the form
            cfrmGoDrawController.StartPosition = FormStartPosition.Manual;
            cfrmGoDrawController.Location = new Point(nLocationX, nLocationY);
            // cfrmGoDrawController.Show();

            // Wait for the user to close the form, do nothing if the user clicks the "Cancel" button
            if (cfrmGoDrawController.ShowDialog() == DialogResult.Cancel)
                return;
        }

        /// <summary>
        /// Event triggered when the "DFTNUMAndCoeffConverter" item in the toolbar is clicked
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event arguments</param>
        private void toolstripmenuitemDFTNUMAndCoeffConverter_Click(object sender, EventArgs e)
        {
            // Create an instance of the DFTNUMAndCoeffConverter form
            frmDFT_NUMAndCoeffConverter cfrmDFT_NUMAndCoeffConverter = new frmDFT_NUMAndCoeffConverter(this);
            //cfrmDFT_NUMAndCoeffConverter.TopMost = true;

            // Set the position of the form to be centered
            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmDFT_NUMAndCoeffConverter.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmDFT_NUMAndCoeffConverter.Height / 2);

            // If the form is an MdiChild, adjust the position to be centered relative to the MdiParent
            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmDFT_NUMAndCoeffConverter.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmDFT_NUMAndCoeffConverter.Height / 2);
            }

            // If the form is a control of the parent form, adjust the position to be centered relative to the parent form
            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmDFT_NUMAndCoeffConverter.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmDFT_NUMAndCoeffConverter.Height / 2);
            }

            // Set the form's starting position
            cfrmDFT_NUMAndCoeffConverter.StartPosition = FormStartPosition.Manual;
            cfrmDFT_NUMAndCoeffConverter.Location = new Point(nLocationX, nLocationY);
            //cfrmGoDrawController.Show();

            // Display the form
            if (cfrmDFT_NUMAndCoeffConverter.ShowDialog() == DialogResult.Cancel)
                return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolstripmenuitemSummarizeLogData_Click(object sender, EventArgs e)
        {
            // Create an instance of the Summarize Log Data form
            frmSummarizeLogData cfrmSummarizeLogData = new frmSummarizeLogData();
            // cfrmSummarizeLogData.TopMost = true;

            // Set the position of the form
            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmSummarizeLogData.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmSummarizeLogData.Height / 2);

            // If the current form is a child form, adjust the position relative to the main form
            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmSummarizeLogData.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmSummarizeLogData.Height / 2);
            }

            // If the parent form reference is needed, adjust the position relative to the parent form
            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmSummarizeLogData.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmSummarizeLogData.Height / 2);
            }

            // Set the form's starting position and display the form
            cfrmSummarizeLogData.StartPosition = FormStartPosition.Manual;
            cfrmSummarizeLogData.Location = new Point(nLocationX, nLocationY);
            // cfrmSummarizeLogData.Show();

            // Wait for the user to close the form, do nothing if the user clicks the "Cancel" button
            if (cfrmSummarizeLogData.ShowDialog() == DialogResult.Cancel)
                return;
        }

        /// <summary>
        /// Triggered when the "Connect" button is clicked
        /// </summary>
        /// <param name="sender">The control that triggered the event</param>
        /// <param name="e">The event</param>
        private void btnNewConnect_Click(object sender, EventArgs e)
        {
            // Executes the Connect flow
            RunNewConnectFlow();
        }

        // <summary>
        /// Executes the Connect flow
        /// </summary>
        private void RunNewConnectFlow()
        {
            // Records a debug message indicating that the Connect button was clicked
            WriteDebugLog("-Click Connect Button");

            // Sets the mode flag
            m_nModeFlag = SetModeFlag();

            // Resets the error flag to false
            m_bErrorFlag = false;
            // Resets the interrupt flag to false
            m_bInterruptFlag = false;

            // Creates an InputDevice object
            m_cInputDevice = new InputDevice(Handle, this);

            // Resets the force stop flag to false
            m_cProcessFlow.ResetForceStopFlag();
            // Sets the initial state of the Connect flow
            InitializeConnectFlow();

            // Displays the message "Connect" in the status bar and error message label in blue font
            OutputStatusAndErrorMessageLabel("Connect", "", Color.Blue);

            // Creates flow steps
            CreateFlowStep();
            // Creates result tab control pages
            CreateResultTabControl();
            // Sets the initial state of the control
            InitializeControl();

            // Sets the step label background color
            SetStepLabelBackColor(MainTuningStep.ELSE, true);

            // Creates a ConnectFlow object
            ConnectFlow cConnectFlow = new ConnectFlow(this);
            // Creates a thread and starts the ConnectFlow
            ThreadPool.QueueUserWorkItem(new WaitCallback(cConnectFlow.RunConnectThread));
        }

        /// <summary>
        /// Triggered when the mouse enters the Connect button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewConnect_MouseEnter(object sender, EventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btConnectImage, btnNewConnect, true, Color.Green);
        }

        /// <summary>
        /// Triggered when the mouse leaves the Connect button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewConnect_MouseLeave(object sender, EventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btConnectImage, btnNewConnect, false, Color.Green);
        }

        /// <summary>
        /// Triggered when the mouse hovers over the Connect button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewConnect_MouseHover(object sender, EventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btConnectImage, btnNewConnect, true, Color.Green);
        }

        /// <summary>
        /// Triggered when the mouse moves over the Connect button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewConnect_MouseMove(object sender, MouseEventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btConnectImage, btnNewConnect, true, Color.Green);
        }

        /// <summary>
        /// Triggered when the "Start" button is clicked
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewStart_Click(object sender, EventArgs e)
        {
            // Executes the Main Process flow
            RunNewMainProcessFlow();
        }

        // <summary>
        /// Executes the Main Process flow
        /// </summary>
        private void RunNewMainProcessFlow()
        {
            // Resets the stopwatch
            m_swCostTime.Reset();
            // Starts the stopwatch
            m_swCostTime.Start();

            // Records the start execution time
            m_sStartFlowTime = System.DateTime.Now.ToString("MMdd-HHmmss");
            WriteDebugLog(string.Format("-Click Start Button, Start Flow Time = {0}", m_sStartFlowTime));

            // Sets the mode flag
            m_nModeFlag = SetModeFlag();

            m_bErrorFlag = false;
            m_bInterruptFlag = false;

            // Initializes the main process flow
            InitializeMainProcessFlow();

            // If in Load Data mode, loads the configuration file and related settings
            if (m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
            {
                m_bResultErrorFlag = false;
                LoadMainSettingParameter();
                ParamAutoTuning.SaveParameter();
                ParamAutoTuning.LoadParameter();
                LoadPHCKPatternSetting();
                CreateFlowStep(false);
                CreateResultTabControl();
                InitializeControl();
            }

            // Sets the background color of the step labels
            SetStepLabelBackColor(MainTuningStep.ELSE, true);

            // Outputs the status and error message to the label
            OutputStatusAndErrorMessageLabel("Execute", "", Color.Blue);

            // Sets the log directory path
            m_sLogDirecotryPath = string.Format(@"{0}\{1}\Log", Application.StartupPath, m_sAPMainDirectoryName);

            // Creates a new thread to execute the Main Process
            ThreadPool.QueueUserWorkItem(new WaitCallback(m_cProcessFlow.RunMainProcessThread));
        }

        /// <summary>
        /// Triggered when the mouse enters the Start button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewStart_MouseEnter(object sender, EventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btStartImage, btnNewStart, true, Color.Navy);
        }

        /// <summary>
        /// Triggered when the mouse leaves the Start button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewStart_MouseLeave(object sender, EventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btStartImage, btnNewStart, false, Color.Navy);
        }

        /// <summary>
        /// Triggered when the mouse hovers over the Start button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewStart_MouseHover(object sender, EventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btStartImage, btnNewStart, true, Color.Navy);
        }

        /// <summary>
        /// Triggered when the mouse moves over the Start button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewStart_MouseMove(object sender, MouseEventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btStartImage, btnNewStart, true, Color.Navy);
        }

        /// <summary>
        /// Triggered when the Stop button is clicked
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewStop_Click(object sender, EventArgs e)
        {
            // Executes the Stop flow
            RunStopFlow();
        }

        /// <summary>
        /// Executes the Stop flow
        /// </summary>
        private void RunStopFlow()
        {
            // Records the Debug message: Click Stop Button
            WriteDebugLog("-Click Stop Button");

            // Disables the Stop button
            btnNewStop.Enabled = false;
            // Sets the focus on the status label
            lblStatus.Focus();
            // Outputs the message: Click Stop Button
            OutputMessage("-Click Stop Button");

            if (m_nModeFlag == MainConstantParameter.m_nMODE_SERVER)
            {
                // If it is in Server mode, executes a forced stop of the Socket connection
                m_cSocket.RunForceStop();
            }
            else
            {
                /*
                Thread tForceStop = new Thread(() =>
                {
                    m_cProcessFlow.RunForceStopFlow();
                });

                tForceStop.IsBackground = true;
                tForceStop.Start();
                */
            }

            m_cProcessFlow.m_sErrorMessage = "Interrupt by User";
            m_cProcessFlow.m_cFinishFlowParameter.m_bOutputMessageFlag = false;
            m_bInterruptFlag = true;

            SetResultMessagePanelFocus();
        }

        /// <summary>
        /// Triggered when the mouse enters the Stop button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewStop_MouseEnter(object sender, EventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btStopImage, btnNewStop, true, Color.Red);
        }

        /// <summary>
        /// Triggered when the mouse leaves the Stop button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewStop_MouseLeave(object sender, EventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btStopImage, btnNewStop, false, Color.Red);
        }

        /// <summary>
        /// Triggered when the mouse hovers over the Stop button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewStop_MouseHover(object sender, EventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btStopImage, btnNewStop, true, Color.Red);
        }

        /// <summary>
        /// Triggered when the mouse moves over the Stop button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewStop_MouseMove(object sender, MouseEventArgs e)
        {
            // Sets the button properties
            SetButtonProperty(m_btStopImage, btnNewStop, true, Color.Red);
        }

        /// <summary>
        /// Triggered when the Pattern button is clicked
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewPattern_Click(object sender, EventArgs e)
        {
            // Records debug message
            WriteDebugLog("-Click Pattern Button");

            // Displays the fullscreen screen and executes monitor disable
            ShowFullScreen(m_cCurrentFlowStep);
            DisableMonitor();
            btnNewPattern.Focus();
        }

        /// <summary>
        /// Triggered when the mouse enters the Pattern button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewPattern_MouseEnter(object sender, EventArgs e)
        {
            btnNewPattern.FlatAppearance.BorderSize = 1;
            btnNewPattern.FlatAppearance.BorderColor = Color.Blue;
            btnNewPattern.BackColor = Color.LightYellow;
        }

        /// <summary>
        /// Triggered when the mouse leaves the Pattern button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewPattern_MouseLeave(object sender, EventArgs e)
        {
            btnNewPattern.FlatAppearance.BorderSize = 0;
            btnNewPattern.FlatAppearance.BorderColor = Color.Blue;
            btnNewPattern.BackColor = SystemColors.Control;
        }

        /// <summary>
        /// Triggered when the Draw button is clicked
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewDraw_Click(object sender, EventArgs e)
        {
            // If clicked multiple times, return directly
            if (m_bMultiClick == true)
                return;

            // Start a new thread to execute the multi-click filter
            new Thread(FilterMultiClick).Start();
            m_bMultiClick = true;

            // If the state is ready to start drawing
            if (m_nDrawButtonStateFlag == MainConstantParameter.m_nDRAWSTATE_READYTODRAW)
            {
                // Set the state to start drawing
                m_nDrawButtonStateFlag = MainConstantParameter.m_nDRAWSTATE_STARTTODRAW;
                WriteDebugLog("-Click Start Draw Button");
                // Set the draw start flag
                m_cProcessFlow.SetDrawStartFlag();
                btnNewDraw.Focus();
            }
            // If the state is currently drawing
            else if (m_nDrawButtonStateFlag == MainConstantParameter.m_nDRAWSTATE_STARTTODRAW)
            {
                // Set the state to draw finished
                m_nDrawButtonStateFlag = MainConstantParameter.m_nDRAWSTATE_FINISH;
                WriteDebugLog("-Click Draw Button");
                // Disable the Draw and Pattern buttons
                btnNewDraw.Enabled = false;
                btnNewPattern.Enabled = false;
                // Set the draw finish flag
                m_cProcessFlow.SetDrawFinishFlag();
                pnlResultMessage.Focus();
            }
        }

        /// <summary>
        /// Triggered when the mouse enters the Draw button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewDraw_MouseEnter(object sender, EventArgs e)
        {
            btnNewDraw.FlatAppearance.BorderSize = 1;
            btnNewDraw.FlatAppearance.BorderColor = Color.Black;
            btnNewDraw.BackColor = Color.LightYellow;
        }

        /// <summary>
        /// Triggered when the mouse leaves the Draw button
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void btnNewDraw_MouseLeave(object sender, EventArgs e)
        {
            btnNewDraw.FlatAppearance.BorderSize = 0;
            btnNewDraw.FlatAppearance.BorderColor = Color.Black;
            btnNewDraw.BackColor = SystemColors.Control;
        }

        /// <summary>
        /// Set the properties of the button
        /// </summary>
        /// <param name="btImage">The image to use</param>
        /// <param name="btnControl">The button control</param>
        /// <param name="bEnter">Whether it is a mouse enter event</param>
        /// <param name="colorBorderColor">Border color</param>
        private void SetButtonProperty(Bitmap btImage, Button btnControl, bool bEnterFlag, Color colorBorderColor)
        {
            Bitmap btImageResize;

            // When the mouse enters the button area
            if (bEnterFlag == true)
                btImageResize = new Bitmap(btImage, new Size(50, 50));
            else
                btImageResize = new Bitmap(btImage, new Size(45, 45));

            // Set the button image
            btnControl.Image = btImageResize;

            // When the mouse enters the button area
            if (bEnterFlag == true)
                btnControl.FlatAppearance.BorderSize = 1;
            else
                btnControl.FlatAppearance.BorderSize = 0;

            // Set the button border color
            btnControl.FlatAppearance.BorderColor = colorBorderColor;

            // When the mouse enters the button area
            if (bEnterFlag == true)
                btnControl.BackColor = Color.LightYellow;
            else
                btnControl.BackColor = SystemColors.Control;
        }

        /// <summary>
        /// Show a message box
        /// </summary>
        /// <param name="sMessage">The message to display</param>
        /// <param name="sTitle">The title of the window</param>
        /// <param name="sConfirmButton">The text of the confirm button</param>
        /// <returns>The status of the message box</returns>
        public frmMessageBox.ReturnStatus ShowMessageBox(string sMessage, string sTitle = frmMessageBox.m_sWarining, string sConfirmButton = "OK")
        {
            // Create a message box and display the message
            frmMessageBox cfrmMessageBox = new frmMessageBox(sTitle, sConfirmButton);
            cfrmMessageBox.ShowMessage(sMessage);

            // Calculate the position where the message box should be displayed
            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmMessageBox.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmMessageBox.Height / 2);

            // If the window is an MDI child window, adjust the position calculation
            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmMessageBox.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmMessageBox.Height / 2);
            }

            // If the window is a parent form of a control, adjust the position calculation
            if (m_bParentFormFlag == true)
            {
                nLocationX = (int)((this.ParentForm.Left + this.ParentForm.Right) / 2) - (int)(cfrmMessageBox.Width / 2);
                nLocationY = (int)((this.ParentForm.Top + this.ParentForm.Bottom) / 2) - (int)(cfrmMessageBox.Height / 2);
            }

            // Set the display position of the message box
            cfrmMessageBox.StartPosition = FormStartPosition.Manual;
            cfrmMessageBox.Location = new Point(nLocationX, nLocationY);

            // Display the message box and wait for the user to press a button
            cfrmMessageBox.ShowDialog();

            return cfrmMessageBox.m_eReturnStatus;
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
        /// <summary>
        /// Sets the control to use double buffering
        /// </summary>
        /// <param name="cControl">The control component</param>
        /// <param name="bSettingFlag">Flag indicating whether to enable or disable double buffering</param>
        public static void MakeDoubleBuffered(this Control cControl, bool bSettingFlag)
        {
            Type typeControl = cControl.GetType();
            PropertyInfo pi = typeControl.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(cControl, bSettingFlag, null);
        }
    }

    public class MyRenderer : ToolStripSystemRenderer
    {
        /// <summary>
        /// Sets the border of the ToolStrip
        /// </summary>
        /// <param name="e">The event</param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // If the affected area is not equal to the bounds of the ToolStrip, call the base class's OnRenderToolStripBorder method
            if (!e.AffectedBounds.Equals(e.ToolStrip.Bounds))
                base.OnRenderToolStripBorder(e);
            else
            {
                // Draws a 3D etched border at the top and bottom of the ToolStrip
                ControlPaint.DrawBorder3D(e.Graphics, e.AffectedBounds.Left, e.AffectedBounds.Bottom - 3, e.AffectedBounds.Right, 3, Border3DStyle.Etched, Border3DSide.Bottom | Border3DSide.Top);
            }
        }
    }
}
