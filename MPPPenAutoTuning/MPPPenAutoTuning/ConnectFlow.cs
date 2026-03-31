using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Management;
using Elan;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class ConnectFlow
    {
        private frmMain m_cfrmMain;

        private string m_sIniPath;
        private string m_sDefaultIniPath;

        private ThreadStart m_tsMainThread = null;
        private Thread m_tMainThread = null;

        private ThreadStart m_tsListenThread = null;
        private Thread m_tListenThread = null;

        private string m_sErrorMessage = "";

        private const int m_nSTEP_Normal            = 0x0001;
        private const int m_nSTEP_DigiGainTuning    = 0x0002;
        private const int m_nSTEP_TPGainTuning      = 0x0004;

        /// <summary>
        /// Parameters related to finishing the flow
        /// </summary>
        public class FinishFlowParameter
        {
            public bool m_bErrorFlag = false;                   // Flag indicating if there is an error
            public bool m_bStateMessageFlag = false;            // Flag indicating if a state message should be displayed
            public bool m_bShowMessageBoxFlag = false;          // Flag indicating if a message box should be shown
            public bool m_bConnectButtonEnableFlag = false;     // Flag indicating if the connect button should be enabled
            public SubTuningStep m_eSubStep = SubTuningStep.NO; // Sub step of the tuning process
            public bool m_bNoiseStepFlowCheckFlag = false;      // Flag indicating if the noise step flow should be checked

            public FinishFlowParameter()
            {
            }

            public void InitializeParameter()
            {
                m_bErrorFlag = false;
                m_bStateMessageFlag = false; 
                m_bShowMessageBoxFlag = false; 
                m_bConnectButtonEnableFlag = false;
                m_eSubStep = SubTuningStep.NO;
                m_bNoiseStepFlowCheckFlag = false;
            }
        }

        public FinishFlowParameter m_cFinishFlowParameter = new FinishFlowParameter();

        /// <summary>
        /// Constructor of the ConnectFlow class, takes an instance of frmMain class object as a parameter
        /// </summary>
        /// <param name="cfrmMain">Instance of frmMain form object</param>
        public ConnectFlow(frmMain cfrmMain)
        {
            // Set the m_cfrmMain object within the ConnectFlow class to the passed cfrmMain object
            m_cfrmMain = cfrmMain;

            // Assign the value of m_sIniPath in the ConnectFlow class to the m_sIniPath property of the frmMain form object
            m_sIniPath = m_cfrmMain.m_sIniPath;
            // Assign the value of m_sDefaultIniPath in the ConnectFlow class to the m_sDefaultIniPath property of the frmMain form object
            m_sDefaultIniPath = m_cfrmMain.m_sDefaultIniPath;

            // Initialize the m_cFinishFlowParameter object with default values
            m_cFinishFlowParameter.InitializeParameter();
        }

        /// <summary>
        /// Execute the Connect thread
        /// </summary>
        /// <param name="objParameter">Related parameter object</param>
        public void RunConnectThread(object objParameter)
        {
            // Create and start the m_tListenThread thread for running the RunListenThread method
            m_tsListenThread = new ThreadStart(RunListenThread);
            m_tListenThread = new Thread(m_tsListenThread);
            m_tListenThread.Start();

            // Create and start the m_tMainThread thread for running the RunMainThread method
            m_tsMainThread = new ThreadStart(RunMainThread);
            m_tMainThread = new Thread(m_tsMainThread);
            m_tMainThread.Start();
        }

        /// <summary>
        /// Execute the Listen thread
        /// </summary>
        private void RunListenThread()
        {
            // Infinite loop for continuous listening
            while (true)
            {
                // If interrupted by the user, end the thread, display an error message, and restore button state
                if (m_cfrmMain.m_bInterruptFlag == true)
                {
                    if (m_tMainThread.IsAlive == true)
                    {
                        m_tMainThread.Abort();
                        m_tMainThread.Join();
                        m_tMainThread = null;
                    }

                    //m_sErrorMessage = "Interrupt by User";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                    SetFinishFlow();
                    m_cfrmMain.m_bErrorFlag = true;
                    break;
                }
                // If an error has occurred, end the thread and restore button state
                else if (m_cfrmMain.m_bErrorFlag == true)
                {
                    if (m_tMainThread.IsAlive == true)
                    {
                        m_tMainThread.Abort();
                        m_tMainThread.Join();
                        m_tMainThread = null;
                    }

                    SetFinishFlow();
                    break;
                }

                // Pause the thread for 5 milliseconds
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Execute the main flow thread
        /// </summary>
        private void RunMainThread()
        {
            OutputMessage("-Start Connect Flow");

            SetModeStateComboBoxAndSettingToolStripMenuItem(false);
            
            #region Check Parameter Setting Window Close(Mark It)
            //CheckParameterSettingWindowIsClosed();
            #endregion

            #region Get & Set Screen Refresh Rate(Mark It)
            //GetAndSetScreenRefreshRate();
            #endregion

            #region Load Parameter Setting
            LoadParameterSetting();
            #endregion

            #region Check Flow Step Is Not Null
            CheckFlowStepIsNotNull();
            #endregion

            FlowStep cFirstFlowStep = m_cfrmMain.m_cFlowStep_List[0];

            #region Client Mode & Single Mode & GoDraw Mode
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
            {
                #region Check Pressure Tuning Step Is Exist By GoDraw Mode
                CheckPressureTuningStepIsExistByGoDrawMode();
                #endregion

                #region Get CPU Type
                GetCPUType();
                #endregion

                #region Check Display Pattern Is Normal
                CheckDisplayPatternIsNormal();
                #endregion

                #region Get Flow Step Directory
                GetFlowStepDirectory(cFirstFlowStep);
                #endregion

                #region Check Process & LT/GoDraw Robot Setting Is Normal
                CheckProcessAndRobotSettingIsNormal();
                #endregion

                #region Check & Connect ELAN Device
                CheckAndConnectElanDevice();
                #endregion

                #region Set Flow Step Info
                SetFlowStepInfo(cFirstFlowStep);
                #endregion

                #region Check Flow Step File Format Is Correct
                CheckFlowStepFileFormatIsCorrect(cFirstFlowStep);
                #endregion

                #region Run Client Connect
                RunClientConnect();
                #endregion

                #region Check & Connect GoDraw Robot
                CheckAndConnectGoDrawRobot();
                #endregion
            }
            #endregion
            #region Server Mode
            else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SERVER)
            {
                #region Check & Connect LT Robot
                CheckAndConnectLTRobot();
                #endregion

                #region Run Network Pin Connect Test(Detect Client's IP)
                RunNetworkPinConnectTest();
                #endregion
            }
            #endregion
            #region Debug Mode(Mark It)
            /*
            else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_DEBUG)
            {
                #region Set Flow Info
                SetFlowStepInfo(cFirstFlowStep, false);
                #endregion
            }
            */
            #endregion

            SetFinishFlow();

            if (m_tListenThread.IsAlive == true)
            {
                m_tListenThread.Abort();
                m_tListenThread.Join();
                m_tListenThread = null;
            }
        }

        /// <summary>
        /// Check if the Parameter Setting window is closed
        /// </summary>
        private void CheckParameterSettingWindowIsClosed()
        {
            /*
            OutputMessage("-Check Parameter Setting Window is Closed"); 
            
            if (m_cfrmMain.m_bParameterSettingFlag == true)
            {
                m_sErrorMessage = "Parameter Setting Window is Open. Please Close It";
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }
            */
        }

        /// <summary>
        /// Get and set the screen refresh rate
        /// </summary>
        private void GetAndSetScreenRefreshRate()
        {
            /*
            OutputMessage("-Get and Set Screen Refresh Rate");
            
            int nScreenRefreshRate = 48;
            Win32.DEVMODE vDevMode = new Win32.DEVMODE();
            Win32.EnumDisplaySettings(null, Win32.ENUM_CURRENT_SETTINGS, ref vDevMode);

            string sMessage = string.Format("Width:{0} Height:{1} Color:{2} Frequency:{3}", vDevMode.dmPelsWidth, vDevMode.dmPelsHeight, 1 << vDevMode.dmBitsPerPel, vDevMode.dmDisplayFrequency)
            MessageBox.Show(sMessage);

            Win32.DEVMODE newMode = vDevMode;
            newMode.dmDisplayFrequency = nScreenRefreshRate;
            int nResult = Win32.ChangeDisplaySettings(ref newMode, 0);

            if (nResult == Win32.DISP_CHANGE_SUCCESSFUL)
                MessageBox.Show("Success.");
            else if (nResult == Win32.DISP_CHANGE_BADMODE)
                MessageBox.Show("Mode not supported.");
            else if (nResult == Win32.DISP_CHANGE_RESTART)
                MessageBox.Show("Restart required.");
            else
                MessageBox.Show(string.Format("Failed. Error code = {0}", nResult));

            vDevMode = new Win32.DEVMODE();
            Win32.EnumDisplaySettings(null, Win32.ENUM_CURRENT_SETTINGS, ref vDevMode);

            sMessage = string.Format("Width:{0} Height:{1} Color:{2} Frequency:{3}", vDevMode.dmPelsWidth, vDevMode.dmPelsHeight, 1 << vDevMode.dmBitsPerPel, vDevMode.dmDisplayFrequency);
            ShowMessageBox(sMessage);
            */
        }

        /// <summary>
        /// Load parameter settings
        /// </summary>
        private void LoadParameterSetting()
        {
            OutputMessage("-Load Parameter Setting");

            // Load main setting parameters
            m_cfrmMain.LoadMainSettingParameter();
            // Save adjustment parameters
            ParamAutoTuning.SaveParameter();
            // Load adjustment parameters
            ParamAutoTuning.LoadParameter();

            // If it is in Client, Single, or GoDraw mode, load the default FW parameters
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                ParamAutoTuning.LoadDefaultFWParameter();

            // Load PHCK Pattern settings
            m_cfrmMain.LoadPHCKPatternSetting();
        }

        /// <summary>
        /// Check if the FlowStep is not null
        /// </summary>
        private void CheckFlowStepIsNotNull()
        {
            OutputMessage("-Check Flow Step");

            if (m_cfrmMain.m_cFlowStep_List == null || m_cfrmMain.m_cFlowStep_List.Count <= 0)
            {
                m_sErrorMessage = "Step Setting Error";
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }
        }

        /// <summary>
        /// Check if the Pressure Tuning step exists when using GoDraw mode.
        /// </summary>
        private void CheckPressureTuningStepIsExistByGoDrawMode()
        {
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
            {
                OutputMessage("-Check Pressure Tuning Step is Exist By Using GoDraw Mode");

                if (m_cfrmMain.m_cFlowStep_List.Exists(x => x.m_eMainStep == MainTuningStep.PRESSURETUNING) == true)
                {
                    string sMainStep = StringConvert.m_dictMainStepMappingTable[MainTuningStep.PRESSURETUNING];
                    m_sErrorMessage = string.Format("GoDraw Mode can't do \"{0}\" Step. Please Remove It", sMainStep);
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return;
                }
            }
        }

        /// <summary>
        /// Get the CPU type
        /// </summary>
        private void GetCPUType()
        {
            OutputMessage("-Get CPU Type");

            if (ParamAutoTuning.m_nGetCPUType != 1)
            {
                m_cfrmMain.m_sCPUName = "None";
                m_cfrmMain.m_sCPUType = MainConstantParameter.m_sCPUTYPE_NONE;
                return;
            }

            m_cfrmMain.m_sCPUName = "";

            // Search for Win32_Processor CPU processor
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_Processor");

            // Get the CPU name
            foreach (ManagementObject mo in mos.Get())
                m_cfrmMain.m_sCPUName = mo["Name"].ToString();

            mos.Dispose();

            // Determine the CPU brand type
            if (m_cfrmMain.m_sCPUName.Contains("Intel") == true || m_cfrmMain.m_sCPUName.Contains("INTEL") == true)
                m_cfrmMain.m_sCPUType = MainConstantParameter.m_sCPUTYPE_INTEL;
            else if (m_cfrmMain.m_sCPUName.Contains("AMD") == true || m_cfrmMain.m_sCPUName.Contains("amd") == true)
                m_cfrmMain.m_sCPUType = MainConstantParameter.m_sCPUTYPE_AMD;
            else
                m_cfrmMain.m_sCPUType = MainConstantParameter.m_sCPUTYPE_ELSE;
        }

        /// <summary>
        /// Check if the display pattern is normal
        /// </summary>
        private void CheckDisplayPatternIsNormal()
        {
            OutputMessage("-Check Display Pattern");

            if (ParamAutoTuning.m_nDisplayType == 1 && ParamAutoTuning.m_nPatternType == 1)
            {
                // Check if the pattern file exists in manual mode
                if (File.Exists(ParamAutoTuning.m_sManualPatternPath) == false)
                {
                    // If the file doesn't exist, set error message and related parameters, then return
                    m_sErrorMessage = "Pattern File Not Exist";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return;
                }
                else
                {
                    // If the file exists, try to load the pattern file
                    try
                    {
                        Bitmap bmPatternPicture = new Bitmap(ParamAutoTuning.m_sManualPatternPath);
                    }
                    catch
                    {
                        // If loading fails, set error message and related parameters, then return
                        m_sErrorMessage = "Pattern File Load Error";
                        m_cFinishFlowParameter.m_bErrorFlag = true;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                        m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                        m_cfrmMain.m_bErrorFlag = true;
                        Thread.Sleep(10);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Get the directory path for the Flow Step
        /// </summary>
        /// <param name="cFlowStep">The current Flow Step</param>
        private void GetFlowStepDirectory(FlowStep cFlowStep)
        {
            OutputMessage("-Get Flow Step Directory");

            FileProcess.m_nCheckStepFlag = 0xFFFF;

            CheckState cCheckState = new CheckState(m_cfrmMain);

            // Check if it is not an independent execution stage step
            if (cCheckState.CheckIndependentStep(cFlowStep.m_eMainStep, cFlowStep.m_eSubStep) == CheckState.m_nSTEPSTATE_NORMAL)
            {
                string sErrorMessage = "";
                bool bGetStepInfo_DigiGainTuning = false;

                FileProcess cFileProcess = new FileProcess(m_cfrmMain);

                // Get the directory path for the Flow Step
                if (cFileProcess.GetFlowStepDirectory(ref bGetStepInfo_DigiGainTuning, ref sErrorMessage, cFlowStep) == FileProcess.m_nGETDIRINFO_ERROR)
                {
                    m_sErrorMessage = sErrorMessage;
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                    m_cFinishFlowParameter.m_eSubStep = cFlowStep.m_eSubStep;
                    m_cFinishFlowParameter.m_bNoiseStepFlowCheckFlag = true;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return;
                }

                m_cfrmMain.m_cProcessFlow.GetStepInfoFlag_DigiGainTuning = bGetStepInfo_DigiGainTuning;
            }
        }

        /// <summary>
        /// Check if the process and robot-related parameters are set correctly
        /// </summary>
        private void CheckProcessAndRobotSettingIsNormal()
        {
            OutputMessage("-Check Process And Robot Setting");

            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT || m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
            {
                //TODO: Modify Set Robot/GoDraw Coordinate, Separate Robot/GoDraw and Other Parameter (Wait Time..etc)
                string sDrawDevice = (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW) ? "GoDraw" : "Robot";

                OutputMessage(string.Format("-Check {0} Coordinate and Parameter Setting", sDrawDevice));

                int nCheckStepFlag = 0;

                if ((nCheckStepFlag & m_nSTEP_Normal) == 0)
                {
                    if (m_cfrmMain.m_cFlowStep_List.Exists(x => x.m_eMainStep == MainTuningStep.DIGIGAINTUNING) == true ||
                        m_cfrmMain.m_cFlowStep_List.Exists(x => x.m_eMainStep == MainTuningStep.PEAKCHECKTUNING) == true ||
                        m_cfrmMain.m_cFlowStep_List.Exists(x => x.m_eMainStep == MainTuningStep.DIGITALTUNING) == true ||
                        m_cfrmMain.m_cFlowStep_List.Exists(x => x.m_eMainStep == MainTuningStep.TILTTUNING) == true ||
                        m_cfrmMain.m_cFlowStep_List.Exists(x => x.m_eMainStep == MainTuningStep.PRESSURETUNING) == true ||
                        m_cfrmMain.m_cFlowStep_List.Exists(x => x.m_eMainStep == MainTuningStep.LINEARITYTUNING) == true)
                        nCheckStepFlag |= m_nSTEP_Normal;
                }

                if ((nCheckStepFlag & m_nSTEP_DigiGainTuning) == 0)
                {
                    if (m_cfrmMain.m_cFlowStep_List.Exists(x => x.m_eMainStep == MainTuningStep.DIGIGAINTUNING) == true)
                        nCheckStepFlag |= m_nSTEP_DigiGainTuning;
                }

                if ((nCheckStepFlag & m_nSTEP_TPGainTuning) == 0)
                {
                    if (m_cfrmMain.m_cFlowStep_List.Exists(x => x.m_eMainStep == MainTuningStep.TPGAINTUNING) == true)
                        nCheckStepFlag |= m_nSTEP_TPGainTuning;
                }

                if ((nCheckStepFlag & m_nSTEP_Normal) != 0)
                {
                    if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                    {
                        double[,] dCoordinateSet_Array = new double[,] 
                        { 
                            { ParamAutoTuning.m_dStartXAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dStartYAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dEndXAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dEndYAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dContactZAxisCoordinate, 100.0 } 
                        };

                        double[] dCompareCoordinate_Array = new double[] 
                        { 
                            ParamAutoTuning.m_dHoverHeight_DT1st,
                            ParamAutoTuning.m_dHoverHeight_DT2nd,
                            ParamAutoTuning.m_dHoverHeight_PP,
                            ParamAutoTuning.m_dHoverHeight_PCT1st,
                            ParamAutoTuning.m_dHoverHeight_PCT2nd 
                        };

                        bool bCheckCoordinateErrorFlag = false;

                        for (int nCoordinateIndex = 0; nCoordinateIndex < dCoordinateSet_Array.GetLength(0); nCoordinateIndex++)
                        {
                            if (CheckCoordinateValid(dCoordinateSet_Array[nCoordinateIndex, 0], dCoordinateSet_Array[nCoordinateIndex, 1]) == false)
                                bCheckCoordinateErrorFlag = true;
                        }

                        for (int nCoordinateIndex = 0; nCoordinateIndex < dCompareCoordinate_Array.Length; nCoordinateIndex++)
                        {
                            if (CheckCompareCoordinateValid(ParamAutoTuning.m_dContactZAxisCoordinate, dCompareCoordinate_Array[nCoordinateIndex]) == false)
                                bCheckCoordinateErrorFlag = true;
                        }

                        if (bCheckCoordinateErrorFlag == true)
                        {
                            m_sErrorMessage = "Robot Coordinate Error. Please Set Robot Coordinate";
                            m_cFinishFlowParameter.m_bErrorFlag = true;
                            m_cFinishFlowParameter.m_bStateMessageFlag = true;
                            m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                            m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                            m_cfrmMain.m_bErrorFlag = true;
                            Thread.Sleep(10);
                            return;
                        }
                    }
                    else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                    {
                        int[,] nCoordinateSet_Array = new int[,] 
                        { 
                            { ParamAutoTuning.m_nGoDrawStartXAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX },
                            { ParamAutoTuning.m_nGoDrawStartYAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY },
                            { ParamAutoTuning.m_nGoDrawEndXAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX },
                            { ParamAutoTuning.m_nGoDrawEndYAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY } 
                        };

                        int[] nServoValueSet_Array = new int[] 
                        { 
                            ParamAutoTuning.m_nGoDrawContactZServoValue,
                            ParamAutoTuning.m_nGoDrawHoverZServoValue_DT1st,
                            ParamAutoTuning.m_nGoDrawHoverZServoValue_DT2nd,
                            ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT1st,
                            ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT2nd 
                        };

                        bool bCheckCoordinateErrorFlag = false;

                        for (int nCoordinateIndex = 0; nCoordinateIndex < nCoordinateSet_Array.GetLength(0); nCoordinateIndex++)
                        {
                            if (CheckCoordinateValid(nCoordinateSet_Array[nCoordinateIndex, 0], nCoordinateSet_Array[nCoordinateIndex, 1]) == false)
                                bCheckCoordinateErrorFlag = true;
                        }

                        for (int nServoValueIndex = 0; nServoValueIndex < nServoValueSet_Array.GetLength(0); nServoValueIndex++)
                        {
                            if (CheckGoDrawServoValueValid(nServoValueSet_Array[nServoValueIndex]) == false)
                                bCheckCoordinateErrorFlag = true;
                        }

                        if ((nCheckStepFlag & m_nSTEP_DigiGainTuning) != 0)
                        {
                            if (CheckGoDrawServoValueValid(ParamAutoTuning.m_nGoDrawContactZServoValue + ParamAutoTuning.m_nGoDrawPushDownZServoValue_DGT) == false)
                                bCheckCoordinateErrorFlag = true;
                        }

                        if (bCheckCoordinateErrorFlag == true)
                        {
                            m_sErrorMessage = "GoDraw X,Y Coordinate or Z ServoValue Error. Please Set GoDraw Coordinate or Z ServoValue";
                            m_cFinishFlowParameter.m_bErrorFlag = true;
                            m_cFinishFlowParameter.m_bStateMessageFlag = true;
                            m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                            m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                            m_cfrmMain.m_bErrorFlag = true;
                            Thread.Sleep(10);
                            return;
                        }
                    }
                }

                if ((nCheckStepFlag & m_nSTEP_TPGainTuning) != 0)
                {
                    if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                    {
                        double[,] dCoordinateSet_Array = new double[,] 
                        { 
                            { ParamAutoTuning.m_dTPGTHorizontalStartXAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dTPGTHorizontalStartYAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dTPGTHorizontalEndXAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dTPGTHorizontalEndYAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dTPGTVerticalStartXAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dTPGTVerticalStartYAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dTPGTVerticalEndXAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dTPGTVerticalEndYAxisCoordinate, 500.0 },
                            { ParamAutoTuning.m_dTPGTContactZAxisCoordinate, 100.0 }
                        };

                        bool bCheckCoordinateErrorFlag = false;

                        for (int nCoordinateIndex = 0; nCoordinateIndex < dCoordinateSet_Array.GetLength(0); nCoordinateIndex++)
                        {
                            if (CheckCoordinateValid(dCoordinateSet_Array[nCoordinateIndex, 0], dCoordinateSet_Array[nCoordinateIndex, 1]) == false)
                                bCheckCoordinateErrorFlag = true;
                        }

                        if (bCheckCoordinateErrorFlag == true)
                        {
                            m_sErrorMessage = "Robot Coordinate Error. Please Set Robot Coordinate";
                            m_cFinishFlowParameter.m_bErrorFlag = true;
                            m_cFinishFlowParameter.m_bStateMessageFlag = true;
                            m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                            m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                            m_cfrmMain.m_bErrorFlag = true;
                            Thread.Sleep(10);
                            return;
                        }
                    }
                    else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                    {
                        int[,] nCoordinateSet_Array = new int[,] 
                        { 
                            { ParamAutoTuning.m_nGoDrawTPGTHorizontalStartXAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX },
                            { ParamAutoTuning.m_nGoDrawTPGTHorizontalStartYAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY },
                            { ParamAutoTuning.m_nGoDrawTPGTHorizontalEndXAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX },
                            { ParamAutoTuning.m_nGoDrawTPGTHorizontalEndYAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY },
                            { ParamAutoTuning.m_nGoDrawTPGTVerticalStartXAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX },
                            { ParamAutoTuning.m_nGoDrawTPGTVerticalStartYAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY },
                            { ParamAutoTuning.m_nGoDrawTPGTVerticalEndXAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX },
                            { ParamAutoTuning.m_nGoDrawTPGTVerticalEndYAxisCoordinate, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY }
                        };

                        int[] nServoValueSet_Array = new int[] 
                        { 
                            ParamAutoTuning.m_nGoDrawTPGTContactZServoValue 
                        };

                        bool bCheckCoordinateErrorFlag = false;

                        for (int nCoordinateIndex = 0; nCoordinateIndex < nCoordinateSet_Array.GetLength(0); nCoordinateIndex++)
                        {
                            if (CheckCoordinateValid(nCoordinateSet_Array[nCoordinateIndex, 0], nCoordinateSet_Array[nCoordinateIndex, 1]) == false)
                                bCheckCoordinateErrorFlag = true;
                        }

                        for (int nServoValueIndex = 0; nServoValueIndex < nServoValueSet_Array.GetLength(0); nServoValueIndex++)
                        {
                            if (CheckGoDrawServoValueValid(nServoValueSet_Array[nServoValueIndex]) == false)
                                bCheckCoordinateErrorFlag = true;
                        }

                        if (bCheckCoordinateErrorFlag == true)
                        {
                            m_sErrorMessage = "GoDraw X,Y Coordinate or Z ServoValue Error. Please Set GoDraw Coordinate or Z ServoValue";
                            m_cFinishFlowParameter.m_bErrorFlag = true;
                            m_cFinishFlowParameter.m_bStateMessageFlag = true;
                            m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                            m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                            m_cfrmMain.m_bErrorFlag = true;
                            Thread.Sleep(10);
                            return;
                        }
                    }
                }

                bool bCheckSpeedErrorFlag = false;
                string sMainStep = "";

                foreach (FlowStep cFlowStep in m_cfrmMain.m_cFlowStep_List)
                {
                    double dDrawSpeed = 0.0;
                    double dDrawSpeed_Slant = 0.0;

                    switch (cFlowStep.m_eMainStep)
                    {
                        case MainTuningStep.DIGIGAINTUNING:
                            dDrawSpeed = ParamAutoTuning.m_dDGTDrawingSpeed;
                            break;
                        case MainTuningStep.TPGAINTUNING:
                            dDrawSpeed = ParamAutoTuning.m_dTPGTDrawingSpeed;
                            break;
                        case MainTuningStep.PEAKCHECKTUNING:
                            dDrawSpeed = ParamAutoTuning.m_dPCTDrawingSpeed;
                            break;
                        case MainTuningStep.DIGITALTUNING:
                            dDrawSpeed = ParamAutoTuning.m_dDTDrawingSpeed;
                            break;
                        case MainTuningStep.TILTTUNING:
                            dDrawSpeed = ParamAutoTuning.m_dTTDrawingSpeed;
                            dDrawSpeed_Slant = ParamAutoTuning.m_dTTSlantDrawingSpeed;
                            break;
                        case MainTuningStep.LINEARITYTUNING:
                            dDrawSpeed = ParamAutoTuning.m_dLTDrawingSpeed;
                            break;
                        default:
                            break;
                    }

                    if (cFlowStep.m_eMainStep == MainTuningStep.TILTTUNING)
                    {
                        if (dDrawSpeed < 0.0 || dDrawSpeed_Slant < 0.0)
                        {
                            sMainStep = StringConvert.m_dictMainStepMappingTable[cFlowStep.m_eMainStep];
                            bCheckSpeedErrorFlag = true;
                            break;
                        }
                    }
                    else
                    {
                        if (dDrawSpeed < 0.0)
                        {
                            sMainStep = StringConvert.m_dictMainStepMappingTable[cFlowStep.m_eMainStep];
                            bCheckSpeedErrorFlag = true;
                            break;
                        }
                    }
                }

                if (bCheckSpeedErrorFlag == true)
                {
                    m_sErrorMessage = string.Format("{0} Speed Error. Please Set {1} Speed", sDrawDevice, sMainStep);
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return;
                }
            }

            m_cfrmMain.m_cProcessFlow.SetProcessAndRobotParameter(m_cfrmMain.m_cFlowStep_List);
        }

        /// <summary>
        /// Check and connect the Elan device
        /// </summary>
        private void CheckAndConnectElanDevice()
        {
            OutputMessage("-Check and Connect Elan Device");

            int nInterface = SetInterface();

            // Connect TP (Using ElanTouch API)
            ElanTouch.Disconnect();

            // Connect to the touch device using the ElanTouch API
            if (ElanTouch.Connect(ParamAutoTuning.m_nUSBVID, ParamAutoTuning.m_nUSBPID, nInterface, 
                                  nDVDD: ParamAutoTuning.m_nDVDD,
                                  nVIO: ParamAutoTuning.m_nVIO,
                                  nI2CAddr: ParamAutoTuning.m_nI2CAddress,
                                  nI2CLength: ParamAutoTuning.m_nNormalLength) == ElanTouch.TP_SUCCESS)
                m_cfrmMain.m_bDeviceConnectFlag = true;

            // Check if the touch device is connected
            if (m_cfrmMain.m_bDeviceConnectFlag == false)
            {
                // Set error flags and display error message if the touch device is not connected
                m_sErrorMessage = "No Touch Device";
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }

            // Get the device index
            int nSelectedDeviceIndex = -1;
            nSelectedDeviceIndex = m_cfrmMain.m_cProcessFlow.GetDeviceIndex(true);

            if (nSelectedDeviceIndex == -1)
            {
                // Set error flags and display error message if the device index cannot be obtained
                m_sErrorMessage = "Get Device Index Error";
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }
            else
                m_cfrmMain.m_nDeviceIndex = nSelectedDeviceIndex;

            if (m_cfrmMain.m_bDeviceConnectFlag == true)
            {
                // Register HID devices
                if (m_cfrmMain.m_cInputDevice.RegisterHIDDevice(ParamAutoTuning.m_nUSBVID, ParamAutoTuning.m_nUSBPID) == false)
                {
                    // Set error flags and display error message if device registration fails
                    m_sErrorMessage = "Device Regist Fail(0x0D)";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return;
                }

                if (m_cfrmMain.m_cInputDevice.RegisterHIDDevice(ParamAutoTuning.m_nUSBVID, ParamAutoTuning.m_nUSBPID, "", 0xFF00) == false)
                {
                    // Set error flags and display error message if device registration fails
                    m_sErrorMessage = "Device Regist Fail(0xFF00)";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return;
                }

                // Set HID handler for raw input
                m_cfrmMain.m_cInputDevice.HIDHandler -= m_cfrmMain.m_cProcessFlow.HIDRawInputHandler;
                m_cfrmMain.m_cInputDevice.HIDHandler += m_cfrmMain.m_cProcessFlow.HIDRawInputHandler;
            }

            SetSPICommandLength();
        }

        private int SetInterface()
        {
            int nInterface = ElanTouch.INTERFACE_WIN_HID;

            /*
            if (((int)UserInterfaceDefine.InterfaceType.IF_I2C == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_I2C_TDDI == ParamAutoTuning.m_nWindowsSPIInterfaceType))
            {
                nInterface = ElanTouch.INTERFACE_WIN_BRIDGE_I2C;
            }
            */

            if (((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                     ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING_HALF_CYCLE == ParamAutoTuning.m_nWindowsSPIInterfaceType))
            {
                nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_RISING_HALF << 4);
            }
            else if (((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                     ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING_HALF_CYCLE == ParamAutoTuning.m_nWindowsSPIInterfaceType))
            {
                nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_FALLING_HALF << 4);
            }
            else if (((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                     ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING == ParamAutoTuning.m_nWindowsSPIInterfaceType))
            {
                nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_RISING << 4);
            }
            else if (((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                     ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING == ParamAutoTuning.m_nWindowsSPIInterfaceType))
            {
                nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_FALLING << 4);
            }
            else
            {
                nInterface = ElanTouch.INTERFACE_WIN_HID;
            }

            return nInterface;
        }

        private void SetSPICommandLength()
        {
            if (((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING_HALF_CYCLE == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING_HALF_CYCLE == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING == ParamAutoTuning.m_nWindowsSPIInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING == ParamAutoTuning.m_nWindowsSPIInterfaceType))
                ElanTouch.SetSPICmdLength(ParamAutoTuning.m_nWindowsSPICommandLength);
        }

        /// <summary>
        /// Set Flow Step information
        /// </summary>
        /// <param name="cFlowStep">The current Flow Step</param>
        /// <param name="bShowMessageBoxFlag">Whether to show a message box</param>
        private void SetFlowStepInfo(FlowStep cFlowStep, bool bShowMessageBoxFlag = true)
        {
            OutputMessage("-Set Flow Step Info");

            // Set Flow Step information and check if it is set successfully
            if (m_cfrmMain.m_cProcessFlow.SetFlowInfo(cFlowStep, true) == false)
            {
                m_sErrorMessage = m_cfrmMain.m_cProcessFlow.m_sErrorMessage;
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = bShowMessageBoxFlag;
                m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }
        }

        /// <summary>
        /// Check if the Flow Step file format is correct
        /// </summary>
        /// <param name="cFlowStep">The Flow Step to check</param>
        private void CheckFlowStepFileFormatIsCorrect(FlowStep cFlowStep)
        {
            OutputMessage("-Check Flow Step File Format");

            // Check if the Flow Step file format is correct
            int nCheckFlowFileFormatFlag = m_cfrmMain.CheckFlowFileFormat(cFlowStep);

            if (nCheckFlowFileFormatFlag != -1)
            {
                int nSubStepIndex = (int)cFlowStep.m_eSubStep;
                int nSubStepState = cFlowStep.m_nSubStepState;
                MainTuningStep eMainStep = cFlowStep.m_eMainStep;
                SubTuningStep eSubStep = cFlowStep.m_eSubStep;

                // If SubStep is the first step and the file format is incorrect, display an error message
                if ((nSubStepState & MainConstantParameter.m_nSTEPLOCATION_FIRST) != 0 && nCheckFlowFileFormatFlag == 1)
                {
                    if (eSubStep == SubTuningStep.NO)
                        m_sErrorMessage = "Flow Invalid. Please Switch and Check Flow Setting";
                    else
                    {
                        string sMainStep = StringConvert.m_dictMainStepMappingTable[eMainStep];
                        string sPreviousSubStep = StringConvert.m_dictPreviousSubStepMappingTable[eSubStep];
                        m_sErrorMessage = string.Format("\"{0} Step\" Flow Invalid. Please Switch or Check \"{1}\" Step", sMainStep, sPreviousSubStep);
                    }
                }
                // If the file format is incorrect, display an error message
                else if (nCheckFlowFileFormatFlag == 2)
                    m_sErrorMessage = "Fixed PH1 or Fixed PH2 Setting Error";
                else
                {
                    string sFileName = m_cfrmMain.m_sSubTuningStepFileName_Array[nSubStepIndex];
                    m_sErrorMessage = string.Format("Flow Data Format Check Error in {0}", sFileName);
                }
                
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                m_cFinishFlowParameter.m_eSubStep = cFlowStep.m_eSubStep;
                m_cFinishFlowParameter.m_bNoiseStepFlowCheckFlag = true;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }
        }

        /// <summary>
        /// Run client connection
        /// </summary>
        private void RunClientConnect()
        {
            // If the mode is Client mode
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
            {
                OutputMessage("-Run Client Connect");

                // Attempt to establish a client connection, display an error message if failed
                if (m_cfrmMain.m_cSocket.RunClientConnect() == false)
                {
                    m_sErrorMessage = "Can not Connect to Server. Please Check the Network";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return;
                }
            }
        }

        /// <summary>
        /// Check and connect to the GoDraw robot
        /// </summary>
        private void CheckAndConnectGoDrawRobot()
        {
            // If it is GoDraw mode
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
            {
                // Output message
                OutputMessage("-Check And Connect GoDraw Robot");

                // Initialize GoDrawAPI
                m_cfrmMain.m_cGoDrawRobot = new GoDrawAPI(GoDrawAPI.ControlUIType.MSPen_AutoTuning, null, m_cfrmMain);

                // Set parameters
                m_cfrmMain.m_cGoDrawRobot.SetParameter();

                // If connecting to GoDraw is successful
                if (m_cfrmMain.m_cGoDrawRobot.RunConnectSerialPort() == true)
                {
                    // Close UdpClient
                    m_cfrmMain.m_cGoDrawRobot.CloseUdpClient();
                    // Close COMPort
                    m_cfrmMain.m_cGoDrawRobot.CloseCOMPort();
                    // Output success message
                    OutputMessage("-Connect GoDraw Robot Success");
                }
                // If connection fails
                else
                {
                    // Close UdpClient
                    m_cfrmMain.m_cGoDrawRobot.CloseUdpClient();

                    m_sErrorMessage = "Can not Connect to GoDraw. Please Check the GoDraw Connect";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return;
                }
            }
        }

        /// <summary>
        /// Check and connect to the LT Robot
        /// </summary>
        private void CheckAndConnectLTRobot()
        {
            OutputMessage("-Check And Connect LT Robot");

            // Test LT Robot connection
            Thread tRobotTest = new Thread(() =>
            {
                m_cfrmMain.m_bRobotConnectFlag = m_cfrmMain.m_cRobot.RunRobotTest();
            });

            tRobotTest.IsBackground = true;
            tRobotTest.Start();

            while (tRobotTest.IsAlive == true)
                Thread.Sleep(10);

            // Check if LT Robot exists
            if (m_cfrmMain.m_bRobotConnectFlag == false)
            {
                m_cfrmMain.m_sSelectLTCOMPort = "N/A";
                m_cfrmMain.m_sSelectFGCOMPort = "N/A";

                m_sErrorMessage = "No LT Robot";
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }
        }

        /// <summary>
        /// Run network pin connect test to confirm network connection
        /// </summary>
        private void RunNetworkPinConnectTest()
        {
            OutputMessage("-Run Network Pin Connect Test");

            if (m_cfrmMain.m_cSocket.RunNetworkPinConnectTest(1) == true)
            {
                OutputMessage("-Detect Client's IP Success");
            }
            else
            {
                m_sErrorMessage = "Can not Connect to Client. Please Check the Network and Close Client's Firewall";
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                m_cFinishFlowParameter.m_bConnectButtonEnableFlag = true;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }
        }

        /// <summary>
        /// Set the finish flow
        /// </summary>
        private void SetFinishFlow()
        {
            OutputMessage("-Set Finish Flow");

            // If there is an error
            if (m_cFinishFlowParameter.m_bErrorFlag == true)
            {
                // Set device disconnection
                SetDeviceDisconnect();

                // Output error message
                OutputMessage(string.Format("-{0}", m_sErrorMessage));
                // Output state message
                OutputStateMessage(m_sErrorMessage, m_cFinishFlowParameter.m_bStateMessageFlag);

                if (m_cFinishFlowParameter.m_bNoiseStepFlowCheckFlag == false)
                {
                    if (m_cFinishFlowParameter.m_bShowMessageBoxFlag == true)
                        ShowMessageBox(string.Format("{0}!", m_sErrorMessage));
                }
                else
                {
                    if (m_cFinishFlowParameter.m_eSubStep == SubTuningStep.NO)
                        m_cfrmMain.ShowFlowSettingWindow();
                    else
                    {
                        if (m_cFinishFlowParameter.m_bShowMessageBoxFlag == true)
                            ShowMessageBox(string.Format("{0}!", m_sErrorMessage));
                    }
                }

                SetNewConnectButton(m_cFinishFlowParameter.m_bConnectButtonEnableFlag);
                SetModeStateComboBoxAndSettingToolStripMenuItem(true);
            }
            // If there is no error
            else
            {
                SetNewConnectButton(true);

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SERVER)
                    SetNewStartButton(m_cfrmMain.m_bRobotConnectFlag);
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                         m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                         m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                    SetNewStartButton(m_cfrmMain.m_bDeviceConnectFlag);
                /*
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_DEBUG)
                    SetNewStartButton(true);
                */
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
                    SetNewStartButton(true);

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                    m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                    m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                {
                    OutputMainStatusStrip("Record Ready", 0, m_cfrmMain.m_cProcessFlow.GetDataCount(), frmMain.m_nInitialFlag);
                }

                OutputStatusAndErrorMessageLabel("Ready", "", Color.Blue);

                SetModeStateComboBoxAndSettingToolStripMenuItem(true);
            }

            m_cfrmMain.SetResultMessagePanelFocus();
        }

        /// <summary>
        /// Set device disconnection
        /// </summary>
        private void SetDeviceDisconnect()
        {
            // If the current device is connected, disconnect it
            if (m_cfrmMain.m_bDeviceConnectFlag == true)
            {
                // Disconnect Elan Touch
                ElanTouch.Disconnect();
                // Set the device connection flag to false
                m_cfrmMain.m_bDeviceConnectFlag = false;
            }
        }

        /// <summary>
        /// Check if the coordinate value is valid
        /// </summary>
        /// <param name="dCoordinateValue">The coordinate value</param>
        /// <param name="dLimitHighBoundary">The upper limit value</param>
        /// <returns>Whether it is a valid value</returns>
        private bool CheckCoordinateValid(double dCoordinateValue, double dLimitHighBoundary)
        {
            if (dCoordinateValue < 0.0 || dCoordinateValue > dLimitHighBoundary)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Check if the coordinate value is valid
        /// </summary>
        /// <param name="nCoordinateValue">The coordinate value</param>
        /// <param name="nLimitHighBoundary">The upper limit value</param>
        /// <returns>Whether it is a valid value</returns>
        private bool CheckCoordinateValid(int nCoordinateValue, int nLimitHighBoundary)
        {
            if (nCoordinateValue < 0 || nCoordinateValue > nLimitHighBoundary)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Check if the compare coordinate value is valid
        /// </summary>
        /// <param name="dCoordinateValue">The original coordinate value</param>
        /// <param name="dCompareCoordinateValue">The compare coordinate value</param>
        /// <returns>Whether it is a valid value</returns>
        private bool CheckCompareCoordinateValid(double dCoordinateValue, double dCompareCoordinateValue)
        {
            if (dCompareCoordinateValue < 0 || dCoordinateValue < dCompareCoordinateValue)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Check if the GoDraw servo value is valid
        /// </summary>
        /// <param name="nServoValue">The servo value</param>
        /// <returns>Whether it is a valid value</returns>
        private bool CheckGoDrawServoValueValid(int nServoValue)
        {
            if (nServoValue < ParamAutoTuning.m_nGoDrawCtrlrMinServoValue || nServoValue > ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Output a message
        /// </summary>
        /// <param name="sMessage">The message</param>
        private void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmMain.OutputMessage(sMessage, bWarning);
        }

        /// <summary>
        /// Output a state message
        /// </summary>
        /// <param name="sMessage">The message</param>
        /// <param name="bResultOutputFlag">Whether to output the result</param>
        /// <param name="bResetOutputFlag">Whether to reset the result</param>
        /// <param name="bStepOutputOnlyFlag">Whether to only output the step</param>
        /// <param name="bTimerCountStopFlag">Whether to stop the timer count</param>
        private void OutputStateMessage(string sMessage, bool bResultOutputFlag = false, bool bResetOutputFlag = false, bool bStepOutputOnlyFlag = false, bool bTimerCountStopFlag = false)
        {
            m_cfrmMain.OutputStateMessage(sMessage, bResultOutputFlag, bResetOutputFlag, bStepOutputOnlyFlag, bTimerCountStopFlag);
        }

        /// <summary>
        /// Set the Mode State ComboBox and Setting ToolStripMenuItem controls to be enabled or disabled
        /// </summary>
        /// <param name="bEnableFlag">Whether to enable</param>
        private void SetModeStateComboBoxAndSettingToolStripMenuItem(bool bEnableFlag)
        {
            m_cfrmMain.SetModeStateComboBoxAndSettingToolStripMenuItem(bEnableFlag);
        }

        /// <summary>
        /// Set the Connect Button control to be enabled or disabled
        /// </summary>
        /// <param name="bEnableFlag">Whether to enable</param>
        private void SetNewConnectButton(bool bEnableFlag)
        {
            m_cfrmMain.SetNewConnectButton(bEnableFlag);
        }

        /// <summary>
        /// Set the Start Button control to be enabled or disabled
        /// </summary>
        /// <param name="bEnableFlag">Whether to enable</param>
        private void SetNewStartButton(bool bEnableFlag)
        {
            m_cfrmMain.SetNewStartButton(bEnableFlag);
        }

        /// <summary>
        /// Output the Main StatusStrip control
        /// </summary>
        /// <param name="sStatus">The status</param>
        /// <param name="nCurrentCount">The current count</param>
        /// <param name="nTotalCount">The total count</param>
        /// <param name="nStatusFlag">The status flag</param>
        private void OutputMainStatusStrip(string sStatus, int nCurrentCount, int nTotalCount = 0, int nStatusFlag = frmMain.m_nOtherFlag)
        {
            m_cfrmMain.OutputMainStatusStrip(sStatus, nCurrentCount, nTotalCount, nStatusFlag);
        }

        /// <summary>
        /// Output the Status and ErrorMessage Label controls
        /// </summary>
        /// <param name="sResultMessage">The result message</param>
        /// <param name="sErrorMessage">The error message</param>
        /// <param name="colorForeColor">The font color</param>
        /// <param name="bOnlyChangelblStatusFlag">Whether to only change the Status Label</param>
        private void OutputStatusAndErrorMessageLabel(string sResultMessage, string sErrorMessage, Color colorForeColor, bool bOnlyChangelblStatusFlag = false)
        {
            m_cfrmMain.OutputStatusAndErrorMessageLabel(sResultMessage, sErrorMessage, colorForeColor, bOnlyChangelblStatusFlag);
        }

        /// <summary>
        /// Show a message box
        /// </summary>
        /// <param name="sMessage">The message</param>
        /// <param name="sTitle">The title</param>
        /// <param name="sConfirmButton">The text for the confirmation button</param>
        private void ShowMessageBox(string sMessage, string sTitle = frmMessageBox.m_sMessage, string sConfirmButton = "OK")
        {
            m_cfrmMain.ShowMessageBox(sMessage, sTitle, sConfirmButton);
        }
    }
}
