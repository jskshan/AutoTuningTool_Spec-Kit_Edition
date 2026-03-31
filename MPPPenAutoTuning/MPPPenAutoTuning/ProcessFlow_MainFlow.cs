using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using Elan;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class ProcessFlow
    {
        private ThreadStart m_tsMainThread = null;
        private Thread m_tMainThread = null;

        private ThreadStart m_tsListenThread = null;
        private Thread m_tListenThread = null;

        private bool m_bLastRetryFlag = false;

        /// <summary>
        /// Runs the main process thread
        /// </summary>
        /// <param name="objParameter">The parameter object</param>
        public void RunMainProcessThread(object objParameter)
        {
            // Create a new ThreadStart delegate for the RunListenThread method
            m_tsListenThread = new ThreadStart(RunListenThread);
            // Create a new Thread using the ThreadStart delegate
            m_tListenThread = new Thread(m_tsListenThread);
            // Start the listen thread
            m_tListenThread.Start();

            // Create a new ThreadStart delegate for the RunMainThread method
            m_tsMainThread = new ThreadStart(RunMainThread);
            // Create a new Thread using the ThreadStart delegate
            m_tMainThread = new Thread(m_tsMainThread);
            // Start the main thread
            m_tMainThread.Start();
        }

        /// <summary>
        /// Runs the listen thread.
        /// </summary>
        private void RunListenThread()
        {
            while (true)
            {
                // Check if the interrupt flag is true (interrupted by user)
                if (m_cfrmMain.m_bInterruptFlag == true)
                {
                    // Abort and join the robot thread if it is running
                    if (m_tRobot != null && m_tRobot.IsAlive == true)
                    {
                        m_tRobot.Abort();
                        m_tRobot.Join();
                        m_tRobot = null;
                    }

                    // Abort and join the main thread if it is running
                    if (m_tMainThread != null && m_tMainThread.IsAlive == true)
                    {
                        m_tMainThread.Abort();
                        m_tMainThread.Join();
                        m_tMainThread = null;
                    }

                    // Set the necessary flags and parameters for finishing the flow
                    //m_sErrorMessage = "Interrupt by User";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cfrmMain.m_bErrorFlag = true;

                    // Run the force stop flow
                    RunForceStopFlow(m_cFinishFlowParameter.m_bOutputMessageFlag, m_cFinishFlowParameter.m_bCloseAPFlag);

                    // Set the finish flow
                    SetFinishFlow();
                    break;
                }
                // Check if there is an error flag
                else if (m_cfrmMain.m_bErrorFlag == true)
                {
                    // Abort and join the robot thread if it is running
                    if (m_tRobot != null && m_tRobot.IsAlive == true)
                    {
                        m_tRobot.Abort();
                        m_tRobot.Join();
                        m_tRobot = null;
                    }

                    // Abort and join the main thread if it is running
                    if (m_tMainThread != null && m_tMainThread.IsAlive == true)
                    {
                        m_tMainThread.Abort();
                        m_tMainThread.Join();
                        m_tMainThread = null;
                    }

                    // Set the finish flow
                    SetFinishFlow();
                    break;
                }

                // Sleep for a short interval
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Runs the main thread
        /// </summary>
        private void RunMainThread()
        {
            m_bProcessFinishFlag = false;

            OutputMessage("-Start Main Process Flow");

            // Initialize finish flow parameters and flow flags
            m_cFinishFlowParameter.InitializeParameter();
            InitialFlowFlag();

            // Get the flow step list
            List<FlowStep> cFlowStep_List = m_cfrmMain.m_cFlowStep_List;

            if (cFlowStep_List != null && cFlowStep_List.Count > 0)
            {
                FlowStep cFirstFlowStep = cFlowStep_List[0];
                m_cfrmMain.m_cCurrentFlowStep = cFirstFlowStep;

                // Delete flow file and clear result list data for specific mode flags
                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                    m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                    m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                {
                    m_cfrmMain.DeleteFlowFile(cFirstFlowStep);

                    FileProcess cFileProcess = new FileProcess(m_cfrmMain);
                    cFileProcess.ClearResultListData(cFirstFlowStep.m_eSubStep, true);
                }
            }
            else
            {
                m_sErrorMessage = "Flow Step List Check Error";
                m_cfrmMain.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = false;
                m_cFinishFlowParameter.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }

            // Set file directory for specific mode flags
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
            {
                bool bLoadDataModeFlag = (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA) ? true : false;
                m_cfrmMain.SetFileDirectory(bLoadDataModeFlag);
            }

            #region Server Mode Flow
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SERVER)
            {
                OutputMessage("-Start Server Mode");

                if (cFlowStep_List != null && cFlowStep_List[0].m_eMainStep == MainTuningStep.SERVERCONTRL)
                {
                    SetStepLabelBackColor(cFlowStep_List[0].m_eMainStep);

                    // Check if the robot is disconnected
                    if (m_cRobot.RunRobotKeepConnectTest() == false)
                    {
                        m_sErrorMessage = "Robot Disconnect";
                        m_cfrmMain.m_bErrorFlag = true;
                        m_cFinishFlowParameter.m_bStateMessageFlag = false;
                        m_cFinishFlowParameter.m_bErrorFlag = true;
                        Thread.Sleep(10);
                        return;
                    }

                    // Run the server listen flow
                    RunServerListenFlow();
                    return;
                }
                else
                {
                    m_sErrorMessage = "Server Mode Control Setting Error";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = false;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return;
                }
            }
            #endregion
            #region Client Mode & Single Mode & GoDraw Mode Flow
            else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                     m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                     m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
            {
                OutputMessage(string.Format("-Start {0} Mode", m_cfrmMain.GetModeName()));

                // Run the main process flow
                RunMainProcessFlow();
            }
            #endregion
            #region LoadData Mode Flow
            else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
            {
                OutputMessage("-Start LoadData Mode");

                MainTuningStep eMainStep = m_cfrmMain.m_cCurrentFlowStep.m_eMainStep;
                SetStepLabelBackColor(eMainStep);

                // Run the load data flow
                RunLoadDataFlow();
            }
            #endregion

            // Set the finish flow
            SetFinishFlow();

            if (m_tListenThread.IsAlive == true)
            {
                m_tListenThread.Abort();
                m_tListenThread.Join();
                m_tListenThread = null;
            }
        }

        /// <summary>
        /// Runs the main process flow
        /// </summary>
        private void RunMainProcessFlow()
        {
            List<FlowStep> cFlowStep_List = m_cfrmMain.m_cFlowStep_List;
            m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_NONE;
            m_cfrmMain.m_bRunStopFlowFinishFlag = false;

            for (int nStepIndex = 0; nStepIndex < cFlowStep_List.Count; nStepIndex++)
            {
                FlowStep cFlowStep = cFlowStep_List[nStepIndex];

                if (nStepIndex == 0)
                {
                    // Reset the stopwatch
                    m_swSingleStep.Reset();
                    // Start the stopwatch
                    m_swSingleStep.Start();
                    m_bFirstStepCostTimeFlag = false;

                    // Get FW Version & IC Solution Type
                    if (GetFWVersion(nStepIndex) == false)
                        return;

                    if (WriteConnectInfoFile(nStepIndex) == false)
                        return;

                    // Check Gen8 MPP MT Mode Enable
                    if (Check_MPP_MT_Mode_Enable_Gen8(nStepIndex) == false)
                        return;

                    // Check Gen8 AutoTune Version
                    if (Check_AutoTune_Version_Gen8(nStepIndex) == false)
                        return;

                    // Check User Defined Command Script File
                    if (CheckUserDefinedFile(cFlowStep_List, nStepIndex) == false)
                        return;
                }

                // Get RX/TX Trace Number
                if (GetTraceNumber(cFlowStep, nStepIndex) == false)
                    return;

                //Get RX/TX Start Trace
                if (GetStartTrace(nStepIndex) == false)
                    return;

                // Get TP Information
                if (GetTPInformation(cFlowStep, nStepIndex) == false)
                    return;

                // Set TP Information
                //SetTPInformation(cFlowStep, nStepIndex);

                // Check Robot Parameter
                if (CheckRobotParameter(nStepIndex) == false)
                    return;

                // Connect GoDraw Robot
                if (ConnectGoDrawRobot(nStepIndex) == false)
                    return;

                if (cFlowStep.m_eMainStep == MainTuningStep.ELSE)
                    continue;

                m_cfrmMain.m_cCurrentFlowStep = cFlowStep_List[nStepIndex];
                m_cfrmMain.m_sCurrentMainStep = StringConvert.m_dictMainStepMappingTable[m_cfrmMain.m_cCurrentFlowStep.m_eMainStep];
                m_cfrmMain.m_sCurrentSubStep = StringConvert.m_dictSubStepMappingTable[m_cfrmMain.m_cCurrentFlowStep.m_eSubStep];
                SetStepLabelBackColor(m_cfrmMain.m_cCurrentFlowStep.m_eMainStep);

                string sHoverHeight = StringConvert.ConvertStepToHoverHeight(m_cfrmMain);
                string sMessage = string.Format("Main={0}, Sub={1}{2}", m_cfrmMain.m_sCurrentMainStep, m_cfrmMain.m_sCurrentSubStep, sHoverHeight);
                OutputStateMessage(sMessage, false, false, true);
                OutputMessage(string.Format("-Flow Step : {0}", sMessage));

                MainTuningStep eMainStep = m_cfrmMain.m_cCurrentFlowStep.m_eMainStep;
                SubTuningStep eSubStep = m_cfrmMain.m_cCurrentFlowStep.m_eSubStep;

                if ((eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO) && m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    MainConstantParameter.SetGen8TraceType();

                //Check Pressure Robot Connect
                if (CheckPressureRobotConnect(nStepIndex) == false)
                    return;

                //Check Pressure Robot PowerOn
                if (CheckPressureRobotPowerOn(nStepIndex, cFlowStep_List[nStepIndex]) == false)
                    return;

                if (nStepIndex > 0)
                {
                    //Get Flow Step Directory
                    if (GetFlowStepDirectory(nStepIndex) == false)
                        return;

                    //Check Reference.csv/Total_Result_WR.csv File Exist
                    if (CheckPreviousStepFileExist(eMainStep, eSubStep, nStepIndex) == false)
                        return;

                    //Check Flow File Format
                    if (CheckFlowFlieFormat(m_cfrmMain.m_cCurrentFlowStep, nStepIndex) == false)
                        return;

                    //Set Flow Info
                    if (RunSetFlowInfo(m_cfrmMain.m_cCurrentFlowStep, nStepIndex) == false)
                        return;

                    OutputMessage("-Check Data Information OK");
                    OutputMainStatusStrip("Record Ready", 0, m_nDataCount, frmMain.m_nInitialFlag);
                }
                else
                {
                    if ((eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO) && m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    {
                        //Set Flow Info
                        if (RunSetFlowInfo(m_cfrmMain.m_cCurrentFlowStep, nStepIndex, false, true) == false)
                            return;

                        OutputMessage("-Check Data Information OK");
                        OutputMainStatusStrip("Record Ready", 0, m_nDataCount, frmMain.m_nInitialFlag);
                    }
                }

                RunSingleStepFlow(m_cfrmMain.m_cCurrentFlowStep, nStepIndex);

                SetStepCostTime(nStepIndex, false);
            }

            m_cfrmMain.m_bRunStopFlowFinishFlag = true;
        }

        /// <summary>
        /// Performs the single step flow within a stage
        /// </summary>
        /// <param name="cFlowStep">Information related to the current stage</param>
        /// <param name="nStepIndex">Current Step index</param>
        public void RunSingleStepFlow(FlowStep cFlowStep, int nStepIndex)
        {
            OutputMessage("-Run Single Step Flow");

            MainTuningStep eMainStep = cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cFlowStep.m_eSubStep;

            m_bLastRetryFlag = false;
            int nRetryCount = 0;
            //string sResultFilePath = "";
            //string sErrorMessage = "";
            m_cCurrentParameterSet.InitializeParameter();

            if (m_cLTRobotParameter != null)
                m_cLTRobotParameter.m_fInitialCoordinateZ_PT = 0.0f;

            m_bDisableRobotMovingFlag = false;
            m_bDisableSetCommandFlag = false;
            m_bRetryStateFlag = false;
            m_bSetDefaultFWParameterFlag = true;

            string sControlMode = "";

            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                sControlMode = "Client Mode";
            else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
                sControlMode = "Single Mode";
            else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
            {
                sControlMode = "GoDraw Mode";
                m_cGoDrawParameter.m_bGoDrawReturnHome = false;
            }

            if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                RunSingleStepMeasureFlow_Gen8(ref nRetryCount, sControlMode, cFlowStep, nStepIndex);
            else
                RunSingleStepMeasureFlow_Gen6or7(ref nRetryCount, sControlMode, cFlowStep, nStepIndex);

            OutputMainStatusStrip(string.Format("Record Data Set : {0}", m_nDataCount), m_nDataCount);
            OutputMessage("-Finish Record Data Flow");

            RunDataAnalysis(cFlowStep, nStepIndex);

            OutputAndDisplayResultData(cFlowStep, nStepIndex);

            SetFlowStepResult(!m_cfrmMain.m_bErrorFlag, m_sErrorMessage);

            ClearAndSetFlowStepResultList();
        }

        /// <summary>
        /// Executes the measure flow for a single step in Gen6 or Gen7
        /// </summary>
        /// <param name="nRetryCount">The retry count</param>
        /// <param name="sControlMode">The control mode</param>
        /// <param name="cFlowStep">The current flow step</param>
        /// <param name="nStepIndex">The current step index</param>
        private void RunSingleStepMeasureFlow_Gen6or7(ref int nRetryCount, string sControlMode, FlowStep cFlowStep, int nStepIndex)
        {
            for (int nDataIndex = 0; nDataIndex < m_nDataCount; nDataIndex++)
            {
                OutputMessage(string.Format("-{0} Start", sControlMode));

                m_nDataIndex = nDataIndex;
                int nDataNumber = nDataIndex + 1;
                m_byteReportData_List.Clear();
                m_dTimeCounter = 0.0;
                m_bRetryStateFlag = false;
                m_cGetFrameData = null;
                m_cSendCommandInfo = null;

                if (nRetryCount > 0)
                    m_bRetryStateFlag = true;

                if (SetDataFilePath(nStepIndex, nDataIndex) == false)
                    return;

                OutputMessage(string.Format("-Record Data Set {0} Start", nDataNumber));
                OutputMainStatusStrip(string.Format("Record Data Set : {0}", nDataNumber), nDataNumber);

                WriteInfoFile(cFlowStep);

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                    m_ptsRobotParameter = new ParameterizedThreadStart(RunSocketRobotThread);
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
                    m_ptsRobotParameter = new ParameterizedThreadStart(RunFakeRobotThread);
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                    m_ptsRobotParameter = new ParameterizedThreadStart(RunGoDrawRobotThread);

                m_tRobot = new Thread(m_ptsRobotParameter);

                string sRobot = RecordSetInfo.m_cRecordSetParameter_List[nDataIndex].m_eRobot.ToString();
                string sRecord = RecordSetInfo.m_cRecordSetParameter_List[nDataIndex].m_eRecord.ToString();
                OutputMessage(string.Format("-Robot Type={0}, Record Type={1}", sRobot, sRecord));

                ResetFlowFlag();

                string sControlRobot = "";

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                    sControlRobot = "Socket Robot";
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
                    sControlRobot = "Fake Robot";
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                    sControlRobot = "GoDraw Robot";

                if (ParamAutoTuning.m_nRecordDataRetryCount <= 0)
                    m_bLastRetryFlag = true;

                RecordSetParameter cRecordSetParameter = RecordSetInfo.m_cRecordSetParameter_List[nDataIndex];

                m_tRobot.IsBackground = true;
                RobotParameter cRobotParameter = new RobotParameter(cFlowStep, cRecordSetParameter, nDataIndex, m_bLastRetryFlag);

                OutputMessage(string.Format("-{0} Thread Start : {1}", sControlRobot, nDataNumber));
                m_tRobot.Start(cRobotParameter);

                OutputMessage("-Start Record Data Flow");
                RunRecordDataFlow_Gen6or7(cRecordSetParameter);

                //Run Robot Thread
                m_tRobot.Join();

                CheckStepDataIsValid(ref nDataIndex, ref nRetryCount, cFlowStep, nStepIndex, nDataNumber);
            }
        }

        /// <summary>
        /// Executes the measure flow for a single step in Gen8
        /// </summary>
        /// <param name="nRetryCount">The retry count</param>
        /// <param name="sControlMode">The control mode</param>
        /// <param name="cFlowStep">The current flow step</param>
        /// <param name="nStepIndex">The current step index</param>
        private void RunSingleStepMeasureFlow_Gen8(ref int nRetryCount, string sControlMode, FlowStep cFlowStep, int nStepIndex)
        {
            m_cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmMain);

            for (int nDataIndex = 0; nDataIndex < m_nDataCount; nDataIndex++)
            {
                OutputMessage(string.Format("-{0} Start", sControlMode));

                m_nDataIndex = nDataIndex;
                int nDataNumber = nDataIndex + 1;
                m_byteReportData_List.Clear();
                m_dTimeCounter = 0.0;
                m_bRetryStateFlag = false;
                m_cGetFrameData = null;
                m_cSendCommandInfo = null;

                if (nRetryCount > 0)
                    m_bRetryStateFlag = true;

                if (LoadUserDefinedCommandScript(cFlowStep, nStepIndex, m_nDataIndex) == false)
                    return;

                if (SetDataFilePath(nStepIndex, nDataIndex) == false)
                    return;

                OutputMessage(string.Format("-Record Data Set {0} Start", nDataNumber));
                OutputMainStatusStrip(string.Format("Record Data Set : {0}", nDataNumber), nDataNumber);

                WriteInfoFile(cFlowStep);

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                    m_ptsRobotParameter = new ParameterizedThreadStart(RunSocketRobotThread);
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
                    m_ptsRobotParameter = new ParameterizedThreadStart(RunFakeRobotThread);
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                    m_ptsRobotParameter = new ParameterizedThreadStart(RunGoDrawRobotThread);

                m_tRobot = new Thread(m_ptsRobotParameter);

                string sRobot = RecordSetInfo.m_cRecordSetParameter_List[nDataIndex].m_eRobot.ToString();
                string sRecord = RecordSetInfo.m_cRecordSetParameter_List[nDataIndex].m_eRecord.ToString();
                OutputMessage(string.Format("-Robot Type={0}, Record Type={1}", sRobot, sRecord));

                ResetFlowFlag();

                string sControlRobot = "";

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                    sControlRobot = "Socket Robot";
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
                    sControlRobot = "Fake Robot";
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                    sControlRobot = "GoDraw Robot";

                if (ParamAutoTuning.m_nRecordDataRetryCount <= 0)
                    m_bLastRetryFlag = true;

                RecordSetParameter cRecordSetParameter = RecordSetInfo.m_cRecordSetParameter_List[nDataIndex];

                m_tRobot.IsBackground = true;
                RobotParameter cRobotParameter = new RobotParameter(cFlowStep, cRecordSetParameter, nDataIndex, m_bLastRetryFlag);

                OutputMessage(string.Format("-{0} Thread Start : {1}", sControlRobot, nDataNumber));
                m_tRobot.Start(cRobotParameter);

                OutputMessage("-Start Record Data Flow");
                RunRecordDataFlow_Gen8(cRecordSetParameter);

                //Run Robot Thread
                m_tRobot.Join();

                CheckStepDataIsValid(ref nDataIndex, ref nRetryCount, cFlowStep, nStepIndex, nDataIndex);
            }
        }

        /// <summary>
        /// Runs the record data flow for Gen6 or Gen7
        /// </summary>
        /// <param name="cRecordSetParameter">The stage information parameter</param>
        private void RunRecordDataFlow_Gen6or7(RecordSetParameter cRecordSetParameter)
        {
            MainTuningStep eMainStep = cRecordSetParameter.m_eMainStep;

            // Run partial steps for record data flow
            RunRecordPartialStep(m_nRECORD_INITIALSETTING, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_FRONTSETTING, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_PARAMSETTING, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_SETTRACENUMBER, cRecordSetParameter);

            OutputMessage(string.Format("-Record Data Flow : Record Stage={0}", cRecordSetParameter.m_eRecord.ToString()));

            // Wait for the robot to move to the start position
            OutputMessage("-Record Data Flow : Wait Robot Move to Start Position");

            while (m_nRobotPrepareFlag < MainConstantParameter.m_nFLOWSTATE_FIRSTSTEP)
                Thread.Sleep(500);

            OutputMessage("-Record Data Flow : Robot OK. Start to Send Command");

            RunRecordPartialStep(m_nRECORD_PARAMWRITEREAD, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_1STMIDDLESETTING, cRecordSetParameter);

            if (m_cfrmMain.m_cCurrentFlowStep.m_bDrawLineByUser == true)
            {
                SetNewDrawButton(MainConstantParameter.m_nDRAWSTATE_READYTODRAW);
                OutputStatusAndErrorMessageLabel("Ready to DrawLine", "", Color.Blue, true);

                while (m_bDrawStartFlag == false)
                    Thread.Sleep(20);
            }

            RunRecordPartialStep(m_nRECORD_SENDGETDATACMD, cRecordSetParameter);
            m_nRecordPrepareFlag |= MainConstantParameter.m_nFLOWSTATE_FIRSTSTEP;

            while (m_nRobotPrepareFlag < MainConstantParameter.m_nFLOWSTATE_SECONDSTEP)
                Thread.Sleep(500);

            ShowFullScreen(m_cfrmMain.m_cCurrentFlowStep);

            DisableMonitor();

            RunRecordPartialStep(m_nRECORD_2NDMIDDLESETTING, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_ENABLEREPORT, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_DISABLEFINGERREPORT, cRecordSetParameter);

            OutputMessage("-Record Data Flow : Start Record");
            m_nRecordPrepareFlag |= MainConstantParameter.m_nFLOWSTATE_SECONDSTEP;

            if (m_cfrmMain.m_cCurrentFlowStep.m_bDrawLineByUser == true)
            {
                SetNewDrawButton(MainConstantParameter.m_nDRAWSTATE_STARTTODRAW);
                OutputStatusAndErrorMessageLabel("Start to DrawLine", "", Color.Blue, true);
            }

            m_cfrmMain.m_qbyteFIFO.Clear();
            m_bRecordStartFlag = true;
            m_nReportCount = 0;
            m_bCheckReportExistFlag = false;

            // Process record data until the recording is complete
            while (CheckRecordState() == true)
            {
                // Dequeue report data
                if (m_cfrmMain.m_qbyteFIFO.DequeueAll(5000, m_cfrmMain.m_byteBuffer_Array, ParamAutoTuning.m_nReportDataLength) == false)
                    continue;

                SetReportDataList(m_cfrmMain.m_byteBuffer_Array);
            }

            // Wait for the robot to finish
            while (m_bRobotFinishedFlag == false)
                Thread.Sleep(500);

            if (eMainStep != MainTuningStep.NO && eMainStep != MainTuningStep.TILTNO)
                Thread.Sleep(200);

            SetScreenResetFlow();
            HideFullScreen();

            RunRecordPartialStep(m_nRECORD_DISABLEREPORT, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_FINISHSETTING, cRecordSetParameter);
        }

        /// <summary>
        /// Runs the record data flow for Gen8
        /// </summary>
        /// <param name="cRecordSetParameter">The stage information parameter</param>
        private void RunRecordDataFlow_Gen8(RecordSetParameter cRecordSetParameter)
        {
            MainTuningStep eMainStep = cRecordSetParameter.m_eMainStep;

            // Run partial steps for record data flow
            RunRecordPartialStep(m_nRECORD_DISABLEREPORT, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_INITIALSETTING, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_FRONTSETTING, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_READORIGINFWVALUE_GEN8, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_PARAMSETTING, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_SETTRACENUMBER, cRecordSetParameter);

            OutputMessage(string.Format("-Record Data Flow : Record Stage={0}", cRecordSetParameter.m_eRecord.ToString()));

            // Wait for the robot to move to the start position
            OutputMessage("-Record Data Flow : Wait Robot Move to Start Position");

            while (m_nRobotPrepareFlag < MainConstantParameter.m_nFLOWSTATE_FIRSTSTEP)
            {
                if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO)
                    Thread.Sleep(10);
                else
                    Thread.Sleep(500);
            }

            OutputMessage("-Record Data Flow : Robot OK. Start to Send Command");

            RunRecordPartialStep(m_nRECORD_PARAMWRITEREAD, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_SETMPPMODE_GEN8, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_1STMIDDLESETTING, cRecordSetParameter);

            if (m_cfrmMain.m_cCurrentFlowStep.m_bDrawLineByUser == true)
            {
                SetNewDrawButton(MainConstantParameter.m_nDRAWSTATE_READYTODRAW);
                OutputStatusAndErrorMessageLabel("Ready to DrawLine", "", Color.Blue, true);

                while (m_bDrawStartFlag == false)
                    Thread.Sleep(20);
            }

            RunRecordPartialStep(m_nRECORD_SENDGETDATACMD, cRecordSetParameter);
            m_nRecordPrepareFlag |= MainConstantParameter.m_nFLOWSTATE_FIRSTSTEP;

            while (m_nRobotPrepareFlag < MainConstantParameter.m_nFLOWSTATE_SECONDSTEP)
            {
                if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO)
                    Thread.Sleep(10);
                else
                    Thread.Sleep(500);
            }

            ShowFullScreen(m_cfrmMain.m_cCurrentFlowStep);

            DisableMonitor();

            RunRecordPartialStep(m_nRECORD_2NDMIDDLESETTING, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_DISABLEREPORT, cRecordSetParameter);

            OutputMessage("-Record Data Flow : Start Record");
            m_nRecordPrepareFlag |= MainConstantParameter.m_nFLOWSTATE_SECONDSTEP;

            if (m_cfrmMain.m_cCurrentFlowStep.m_bDrawLineByUser == true)
            {
                SetNewDrawButton(MainConstantParameter.m_nDRAWSTATE_STARTTODRAW);
                OutputStatusAndErrorMessageLabel("Start to DrawLine", "", Color.Blue, true);
            }

            m_cfrmMain.m_qbyteFIFO.Clear();
            m_bRecordStartFlag = true;
            m_nReportCount = 0;
            m_bCheckReportExistFlag = false;

            RunRecordPartialStep(m_nRECORD_SENDGETDATACMD_GEN8, cRecordSetParameter);

            if (eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1)
            {
                // Get frame data for noise tuning
                SetTraceInformation(m_nRXTraceNumber, m_nTXTraceNumber);

                m_cGetFrameData = new GetFrameData(m_cfrmMain,
                                                   this,
                                                   m_nTXTraceNumber,
                                                   m_nRXTraceNumber,
                                                   m_nICSolutionType,
                                                   m_cfrmMain.m_nDeviceIndex,
                                                   m_structTraceInfo,
                                                   ParamAutoTuning.m_nNoiseFrameNumber,
                                                   m_nFWVersion);

                m_cGetFrameData.GetdVFrameData();
            }
            else
            {
                // Process report data
                while (CheckRecordState() == true)
                {
                    if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO)
                    {
                        m_cfrmMain.m_byteBuffer_Array = new byte[ParamAutoTuning.m_nGen8ReportDataLength];

                        if (m_cfrmMain.m_qbyteFIFO.DequeueAll(100, m_cfrmMain.m_byteBuffer_Array, ParamAutoTuning.m_nGen8ReportDataLength + ParamAutoTuning.m_nShiftByteNumber) == false)
                            continue;
                    }

                    SetReportDataList(m_cfrmMain.m_byteBuffer_Array);
                }
            }

            // Wait for the robot to finish
            while (m_bRobotFinishedFlag == false)
            {
                if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO)
                    Thread.Sleep(10);
                else
                    Thread.Sleep(500);
            }

            if (!(eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO))
                Thread.Sleep(200);

            SetScreenResetFlow();
            HideFullScreen();

            RunRecordPartialStep(m_nRECORD_DISABLEREPORT, cRecordSetParameter);
            RunRecordPartialStep(m_nRECORD_FINISHSETTING, cRecordSetParameter);
        }

        #region Record Data Related Function
        /// <summary>
        /// Sets the configuration for recording Report Data in different stages
        /// </summary>
        /// <param name="nRecordStep">The recording step for Report Data</param>
        /// <param name="cRecordSetParameter">Information about the current stage</param>
        private void RunRecordPartialStep(int nRecordStep, RecordSetParameter cRecordSetParameter)
        {
            MainTuningStep eMainStep = cRecordSetParameter.m_eMainStep;
            SubTuningStep eSubStep = cRecordSetParameter.m_eSubStep;

            switch (nRecordStep)
            {
                case m_nRECORD_INITIALSETTING:
                    if (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURESETTING && m_cCurrentParameterSet.m_nROrgIQ_BSH_P < 0)
                        m_cCurrentParameterSet.InitializeParameter();

                    break;
                case m_nRECORD_FRONTSETTING:
                    if ((eMainStep == MainTuningStep.DIGIGAINTUNING ||
                         eMainStep == MainTuningStep.TPGAINTUNING ||
                         eMainStep == MainTuningStep.PEAKCHECKTUNING ||
                         eMainStep == MainTuningStep.DIGITALTUNING ||
                         eMainStep == MainTuningStep.TILTTUNING ||
                         eMainStep == MainTuningStep.PRESSURETUNING ||
                         eMainStep == MainTuningStep.LINEARITYTUNING) &&
                        m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
                        m_cfrmMain.m_cCurrentFlowStep.m_bDrawLineByUser = true;

                    m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_NONE;

                    switch (eMainStep)
                    {
                        case MainTuningStep.DIGIGAINTUNING:
                        case MainTuningStep.TPGAINTUNING:
                            if (cRecordSetParameter.m_eRobot == FlowRobot.TOUCHLINE_HOR)
                                m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_HORIZONTALMIDDLELONGLINE;
                            else if (cRecordSetParameter.m_eRobot == FlowRobot.TOUCHLINE_VER)
                                m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_VERTICALMIDDLELONGLINE;
                            else if (cRecordSetParameter.m_eRobot == FlowRobot.TOUCHLINE)
                                m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_SLANTLINE;

                            break;
                        case MainTuningStep.PEAKCHECKTUNING:
                        case MainTuningStep.DIGITALTUNING:
                            m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_SLANTLINE;
                            break;
                        case MainTuningStep.TILTTUNING:
                            if (cRecordSetParameter.m_eRobot == FlowRobot.TOUCHLINE_HOR)
                                m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_HORIZONTALLINE;
                            else if (cRecordSetParameter.m_eRobot == FlowRobot.TOUCHLINE_VER)
                                m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_VERTICALLINE;
                            else if (cRecordSetParameter.m_eRobot == FlowRobot.TOUCHLINE)
                                m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_SLANTLINE;

                            break;
                        case MainTuningStep.PRESSURETUNING:
                            m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_CENTERPOINT;
                            break;
                        case MainTuningStep.LINEARITYTUNING:
                            if (cRecordSetParameter.m_eRobot == FlowRobot.TOUCHLINE_HOR)
                                m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_HORIZONTALLONGLINE;
                            else if (cRecordSetParameter.m_eRobot == FlowRobot.TOUCHLINE_VER)
                                m_cfrmMain.m_cCurrentFlowStep.m_nPatternType = MainConstantParameter.m_nDRAWTYPE_VERTICALLONGLINE;

                            break;
                        default:
                            break;
                    }

                    if ((eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO))
                    {
                        string sDescription = "";

                        if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                        {
                            if (eMainStep == MainTuningStep.NO)
                            {
                                if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                                    sDescription = string.Format("({0:0.000}KHz {1})", cRecordSetParameter.m_dFrequency, "RX");
                                else if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                                    sDescription = string.Format("({0:0.000}KHz {1})", cRecordSetParameter.m_dFrequency, "TX");
                            }
                            else if (eMainStep == MainTuningStep.TILTNO)
                            {
                                if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                                {
                                    if (cRecordSetParameter.m_nSection >= 0 && cRecordSetParameter.m_nSection < 4)
                                    {
                                        string sSection = string.Format("S{0}", cRecordSetParameter.m_nSection);
                                        sDescription = string.Format("({0:0.000}KHz {1} {2})", cRecordSetParameter.m_dFrequency, "RX", sSection);
                                    }
                                    else
                                        sDescription = string.Format("({0:0.000}KHz {1})", cRecordSetParameter.m_dFrequency, "RX");
                                }
                                else if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                                    sDescription = string.Format("({0:0.000}KHz {1})", cRecordSetParameter.m_dFrequency, "TX");
                            }
                        }
                        else 
                        {
                            sDescription = string.Format("({0:0.000}KHz)", cRecordSetParameter.m_dFrequency);
                        }

                        m_cfrmMain.m_cCurrentFlowStep.m_sDescription = sDescription;
                    }

                    SetStateMessage(nRecordStep, cRecordSetParameter);
                    break;
                case m_nRECORD_SETTRACENUMBER:
                    m_cCurrentParameterSet.SetRXTXTraceNumber(m_nRXTraceNumber, m_nTXTraceNumber);
                    break;
                case m_nRECORD_READORIGINFWVALUE_GEN8:
                    /*
                    if (m_nICSolutionType == MainConstantParameter.nICSOLUTIONTYPE_GEN8 && m_cElanCommand_Gen8.ReadGetOriginFWValueFlag() == false)
                    {
                        if (RunGetOriginFWValue(cRecordSetParameter) == false)
                            return;
                    }
                    */

                    break;
                case m_nRECORD_PARAMSETTING:
                    if (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURETABLE)
                    {
                        if (m_bDisableSetCommandFlag == false)
                            m_cCurrentParameterSet.SetParameterValue(m_cfrmMain, cRecordSetParameter, m_bGetStepInfoFlag_DigiGainTuning);
                    }
                    else
                        m_cCurrentParameterSet.SetParameterValue(m_cfrmMain, cRecordSetParameter, m_bGetStepInfoFlag_DigiGainTuning, m_nICSolutionType);

                    break;
                case m_nRECORD_PARAMWRITEREAD:
                    if (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURETABLE)
                    {
                        if (m_bDisableSetCommandFlag == false)
                        {
                            if (RunSendFWParameter(cRecordSetParameter) == false)
                                return;

                            if (RunSendDefaultFWParameter(cRecordSetParameter) == false)
                                return;
                        }
                    }
                    else
                    {
                        if (RunSendFWParameter(cRecordSetParameter) == false)
                            return;

                        if (RunSendDefaultFWParameter(cRecordSetParameter) == false)
                            return;
                    }

                    break;
                case m_nRECORD_SETMPPMODE_GEN8:
                    if ((m_cfrmMain.m_cCurrentFlowStep.m_eMainStep == MainTuningStep.NO || m_cfrmMain.m_cCurrentFlowStep.m_eMainStep == MainTuningStep.TILTNO) &&
                        m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    {
                        if (!(eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1))
                        {
                            if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                            {
                                Set_MPP_Mode_Type_Gen8(cRecordSetParameter);

                                if (Check_MPP_MT_Mode_Status_Gen8(cRecordSetParameter) == false)
                                    return;
                            }
                        }
                    }

                    break;
                case m_nRECORD_1STMIDDLESETTING:
                    SetStateMessage(nRecordStep, cRecordSetParameter);
                    break;
                case m_nRECORD_SENDGETDATACMD:
                    if (!((m_cfrmMain.m_cCurrentFlowStep.m_eMainStep == MainTuningStep.NO || m_cfrmMain.m_cCurrentFlowStep.m_eMainStep == MainTuningStep.TILTNO) &&
                          m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8))
                    {
                        if (!(eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1))
                        {
                            if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                                Set_Get_Report_Data_Gen8(cRecordSetParameter);
                            else if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_OTHER)
                                Set_Get_Report_Data_Gen6or7(cRecordSetParameter);

                            OutputMessage("-Record Data Flow : Set Get Report Data Complete");
                        }
                    }

                    break;
                case m_nRECORD_SENDGETDATACMD_GEN8:
                    if ((m_cfrmMain.m_cCurrentFlowStep.m_eMainStep == MainTuningStep.NO || m_cfrmMain.m_cCurrentFlowStep.m_eMainStep == MainTuningStep.TILTNO))
                    {
                        if (!(eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1))
                        {
                            Set_Get_Report_Data_Gen8(cRecordSetParameter);
                            OutputMessage("-Record Data Flow : Set Get Report Data Complete");
                        }
                    }

                    break;
                case m_nRECORD_2NDMIDDLESETTING:
                    if ((eMainStep != MainTuningStep.NO && eMainStep != MainTuningStep.TILTNO) && m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT &&
                        m_nICSolutionType != MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    {
                        OutputMessage(string.Format("-Record Data Flow : Record Prepare, Please Wait {0} ms.", m_nStartWaitTime.ToString()));
                        Thread.Sleep(m_nStartWaitTime);
                    }

                    break;
                case m_nRECORD_ENABLEREPORT:
                    Send_Specific_Type_Command(MainConstantParameter.m_sSENDCOMMAND_ENABLEREPORT);
                    break;
                case m_nRECORD_DISABLEREPORT:
                    Send_Specific_Type_Command(MainConstantParameter.m_sSENDCOMMAND_DISABLEREPORT);
                    break;
                case m_nRECORD_DISABLEFINGERREPORT:
                    Send_Specific_Type_Command(MainConstantParameter.m_sSENDCOMMAND_DISABLEFINGERREPORT);
                    break;
                case m_nRECORD_FINISHSETTING:
                    if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    {
                        if ((eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType != 1) || eMainStep == MainTuningStep.TILTNO)
                        {
                            CheckReportExist();

                            if (m_nDataIndex == m_nDataCount - 1)
                                Set_Normal_Mode_Gen8();
                        }
                    }
                    else if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_OTHER)
                    {
                        if (eMainStep == MainTuningStep.TILTNO || eMainStep == MainTuningStep.TILTTUNING)
                        {
                            // Reset Tilt Noise Status
                            if (eMainStep == MainTuningStep.TILTNO)
                                Send_Specific_Type_Command(MainConstantParameter.m_sSENDCOMMAND_RESETTILTNOISESTATE);

                            // Reset Tilt Status
                            Send_Specific_Type_Command(MainConstantParameter.m_sSENDCOMMAND_RESETTILTSTATE);
                        }

                        else if (eMainStep == MainTuningStep.PEAKCHECKTUNING || eMainStep == MainTuningStep.DIGITALTUNING)
                        {
                            // Stop Get Sync Report Data
                            if (eSubStep == SubTuningStep.PCHOVER_1ST ||
                                eSubStep == SubTuningStep.PCHOVER_2ND ||
                                eSubStep == SubTuningStep.PCCONTACT ||
                                eSubStep == SubTuningStep.HOVER_1ST ||
                                eSubStep == SubTuningStep.HOVER_2ND ||
                                eSubStep == SubTuningStep.CONTACT)
                                Send_Specific_Type_Command(MainConstantParameter.m_sSENDCOMMAND_STOPGETSYNCREPORTDATA);
                        }
                        else if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                                 eMainStep == MainTuningStep.TPGAINTUNING ||
                                 eMainStep == MainTuningStep.LINEARITYTUNING)
                        {
                            //Reset
                            Set_Reset();
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Get the firmware version from the device
        /// </summary>
        /// <param name="nStepIndex">The index of the step</param>
        /// <returns>True if the firmware version is successfully retrieved, false otherwise</returns>
        private bool GetFWVersion(int nStepIndex)
        {
            OutputMessage("-Get FW Version");

            UInt32 nFWVersion = 0;
            bool bSuccessFlag = false;

            for (int nRetryIndex = 0; nRetryIndex <= 4; nRetryIndex++)
            {
                int nResultFlag = ElanTouch.GetFWVer(ref nFWVersion, 1000, m_cfrmMain.m_nDeviceIndex);

                if (nRetryIndex == 0)
                    OutputMessage(string.Format("-Get FW Version(Command : 53 00 00 01) Value=0x{0}", nFWVersion.ToString("X4").ToUpper()));
                else
                    OutputMessage(string.Format("-Get FW Version(Command : 53 00 00 01) Value=0x{0}(RetryCount={1})", nFWVersion.ToString("X4").ToUpper(), nRetryIndex - 1));

                Thread.Sleep(1000);

                if (nResultFlag == ElanTouch.TP_SUCCESS)
                {
                    bSuccessFlag = true;
                    break;
                }
            }

            if (bSuccessFlag == true)
            {
                int nFWVersion_HighByte = ((int)nFWVersion & 0xFF00) >> 8;
                int nGen8Type = (nFWVersion_HighByte & 0xF0) >> 4;

                //Check the FW Version
                if (nFWVersion == 0x0000)
                {
                    m_sErrorMessage = string.Format("Get FW_Version Value Error(FW_Version=0x{0})", nFWVersion.ToString("X4"));
                    m_cfrmMain.m_eICGenerationType = ICGenerationType.None;
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
                else if (nGen8Type == 0x8)
                {
                    m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_GEN8;
                    MainConstantParameter.SetGen8ICSolution(nFWVersion_HighByte);
                    string sSolution = MainConstantParameter.GetGen8ICSolution();
                    m_cfrmMain.m_eICGenerationType = ICGenerationType.Gen8;
                    m_cfrmMain.m_eICSolutionType = MainConstantParameter.SetICSolutionType_Gen8(nFWVersion_HighByte);
                    string sSolutionName = MainConstantParameter.GetICSolutionType_Gen8(m_cfrmMain.m_eICSolutionType);
                    m_cfrmMain.m_sICSolutionName = string.Format("Generation 8({0})", sSolutionName);
                    OutputMessage(string.Format("-Get IC Type : Gen 8({0})", sSolution));

                    //ParamAutoTuning.LoadGen8FWParameterAddress();
                    //OutputMessage("-Load Gen 8 FW Parameter Address Finish");
                }
                else if (nFWVersion_HighByte == 0x64 || nFWVersion_HighByte == 0x65 ||
                     nFWVersion_HighByte == 0x67 || nFWVersion_HighByte == 0x68)
                {
                    m_cfrmMain.m_eICGenerationType = ICGenerationType.Gen7;
                    m_cfrmMain.m_eICSolutionType = MainConstantParameter.SetICSolutionType_Gen7(nFWVersion_HighByte);
                    string sSolutionName = MainConstantParameter.GetICSolutionType_Gen7(m_cfrmMain.m_eICSolutionType);
                    m_cfrmMain.m_sICSolutionName = string.Format("Generation 7({0})", sSolutionName);
                    OutputMessage(string.Format("-Get IC Type : Generation 7({0})", sSolutionName));
                }
                else if (nFWVersion_HighByte == 0x59 || nFWVersion_HighByte == 0x61 ||
                         nFWVersion_HighByte == 0x62 || nFWVersion_HighByte == 0x63)
                {
                    m_cfrmMain.m_eICGenerationType = ICGenerationType.Gen6;
                    m_cfrmMain.m_eICSolutionType = MainConstantParameter.SetICSolutionType_Gen6(nFWVersion_HighByte);
                    string sSolutionName = MainConstantParameter.GetICSolutionType_Gen6(m_cfrmMain.m_eICSolutionType);
                    m_cfrmMain.m_sICSolutionName = string.Format("Generation 6({0})", sSolutionName);
                    OutputMessage(string.Format("-Get IC Type : Generation 6({0})", sSolutionName));
                }
                else
                {
                    m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_OTHER;
                    m_cfrmMain.m_eICGenerationType = ICGenerationType.Other;
                    m_cfrmMain.m_sICSolutionName = "Other";
                    OutputMessage("-Get IC Type : Other");
                }

                m_nFWVersion = nFWVersion;
                return true;
            }
            else
            {
                m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_NONE;
                m_sErrorMessage = "Get FW Version Error";
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return false;
            }
        }

        private bool WriteConnectInfoFile(int nStepIndex)
        {
            WriteValue("Connect Information", "ProjectName", ParamAutoTuning.m_sProjectName, m_cfrmMain.m_sConnectInfoFilePath, true, false);
            WriteValue("Connect Information", "ICSolution", m_cfrmMain.m_sICSolutionName, m_cfrmMain.m_sConnectInfoFilePath, true, false);

            return true;
        }

        public static void WriteValue(string Section, string Key, string Value, string m_sPath, bool bAlwaysWrite = true, bool bSpace = true)
        {
            IniWriteValue(Section, Key, Value, m_sPath, bAlwaysWrite, bSpace);
        }

        private static void IniWriteValue(string Section, string Key, string Value, string m_sPath, bool bAlwaysWrite = true, bool bSpace = true)
        {
            if (bAlwaysWrite == false)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = Win32.GetPrivateProfileString(Section, Key, "DataNotExist!\\[N/A]", temp, 255, m_sPath);

                if (temp != null)
                {
                    if (temp.ToString() == "DataNotExist!\\[N/A]")
                        return;
                }
                else
                    return;
            }

            if (bSpace == true)
                Value = string.Format(" {0}", Value);

            Win32.WritePrivateProfileString(Section, Key, Value, m_sPath);
        }

        private bool Check_MPP_MT_Mode_Enable_Gen8(int nStepIndex)
        {
            m_bSupport_Get_MPP_MT_Mode_Enable = false;

            if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
            {
                int nStateValue = Get_MPP_MT_Mode_Enable_Gen8();

                if (nStateValue == 1)
                {
                    OutputMessage("-Check MPP MT Mode is Enable");
                    m_bSupport_Get_MPP_MT_Mode_Enable = true;
                }
                else if (nStateValue == 0)
                {
                    m_sErrorMessage = "Check MPP MT Mode is Disable. Please Set \"MPP_MT_Enable\"=\"1\" in FW";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
                else if (nStateValue == -2)
                {
                    string sWarningMessage = "未支援讀取\"MPP MT Mode\"狀態，請自行確認FW中是否已設定\"MPP_MT_Enable\"=\"1\"。若已確認為是則請點擊\"Continue\"按鈕；若未確認則請點擊\"Stop\"按鈕停止流程進行確認";
                    bool bContinueFlag = Show_frmWarningMessage_2Selection(sWarningMessage);

                    if (bContinueFlag == true)
                    {
                        OutputMessage("-Select Continue when Check MPP MT Mode is Disable or Unknown");
                        return true;
                    }
                    else
                    {
                        m_sErrorMessage = "Select Stop to Check \"MPP_MT_Enable\"=\"1\" in FW";
                        // m_sErrorMessage = "Check MPP MT Mode is Disable. Please Set \"MPP_MT_Enable\" = 1 in FW";
                        m_cFinishFlowParameter.m_bErrorFlag = true;
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                        m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                        m_cfrmMain.m_bErrorFlag = true;
                        Thread.Sleep(10);
                        return false;
                    }
                }
                else
                {
                    m_sErrorMessage = "Check MPP MT Mode Error";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
            }

            return true;
        }

        private bool Check_MPP_MT_Mode_Status_Gen8(RecordSetParameter cRecordSetParameter)
        {
            if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8 && m_bSupport_Get_MPP_MT_Mode_Enable == true)
            {
                // ElanCommand_Gen8.MPPModeType eMPPModeType;
                byte byteCheckHighByte = 0x00;
                byte byteCheckLowByte = 0x00;

                GetDataType cGetDataType = new GetDataType();
                int nGetDataType = cGetDataType.SetGetDataType_Gen8(cRecordSetParameter);

                switch (nGetDataType)
                {
                    case GetDataType.m_nDATATYPE_NTRX_400us:
                    case GetDataType.m_nDATATYPE_NTRX_800us:
                    case GetDataType.m_nDATATYPE_NRX_400us:
                    case GetDataType.m_nDATATYPE_NRX_800us:
                        // eMPPModeType = ElanCommand_Gen8.MPPModeType.RX_DFT;
                        byteCheckHighByte = 0x01;
                        byteCheckLowByte = 0x08;
                        break;
                    case GetDataType.m_nDATATYPE_NTX_400us:
                    case GetDataType.m_nDATATYPE_NTX_800us:
                        // eMPPModeType = ElanCommand_Gen8.MPPModeType.TX_DFT;
                        byteCheckHighByte = 0x01;
                        byteCheckLowByte = 0x10;
                        break;
                    case GetDataType.m_nDATATYPE_PTHF_NoSync_Gen8:
                        // eMPPModeType = ElanCommand_Gen8.MPPModeType.PTHF_NoSync;
                        byteCheckHighByte = 0x01;
                        byteCheckLowByte = 0x86;
                        break;
                    case GetDataType.m_nDATATYPE_BHF_NoSync_Gen8:
                        // eMPPModeType = ElanCommand_Gen8.MPPModeType.BHF_NoSync;
                        byteCheckHighByte = 0x01;
                        byteCheckLowByte = 0x84;
                        break;
                    default:
                        // eMPPModeType = ElanCommand_Gen8.MPPModeType.Normal;
                        byteCheckHighByte = 0x00;
                        byteCheckLowByte = 0x00;
                        break;
                }

                byte byteStatusHighByte = 0x00;
                byte byteStatusLowByte = 0x00;

                bool bResult = Get_MPP_MT_Mode_Status_Gen8(ref byteStatusHighByte, ref byteStatusLowByte);

                if (bResult == true)
                {
                    if (byteStatusHighByte == byteCheckHighByte && byteStatusLowByte == byteCheckLowByte)
                    {
                        OutputMessage("-Check MPP MT Mode Status is Matched");
                    }
                    else
                    {
                        m_sErrorMessage = "Check MPP MT Mode Status is Not Matched";
                        m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                        m_cfrmMain.m_bInterruptFlag = true;
                        Thread.Sleep(10);
                        return false;
                    }
                }
                else
                {
                    m_sErrorMessage = "Get MPP MT Mode Status Error";
                    m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                    m_cfrmMain.m_bInterruptFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
            }

            return true;
        }

        private bool Check_AutoTune_Version_Gen8(int nStepIndex)
        {
            if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
            {
                int nAutoTuneVersion = Get_AutoTune_Version_Gen8();

                if (nAutoTuneVersion == -1)
                {
                    ParamAutoTuning.LoadGen8FWParameterAddress(nAutoTuneVersion);
                    OutputMessage("-Load Gen 8 FW Parameter Address Finish");
                }
                else if (nAutoTuneVersion == 1)
                {
                    ParamAutoTuning.LoadGen8FWParameterAddress(nAutoTuneVersion);
                    OutputMessage("-Load Gen 8 FW Parameter Address Finish");
                }
                else
                {
                    m_sErrorMessage = "Check AutoTune Version Error";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the user-defined file exists and perform necessary checks
        /// </summary>
        /// <param name="cFlowStep_List">The list of flow steps</param>
        /// <param name="nStepIndex">The index of the step</param>
        /// <returns>True if the user-defined file check is successful, false otherwise</returns>
        private bool CheckUserDefinedFile(List<FlowStep> cFlowStep_List, int nStepIndex)
        {
            if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8 && (ParamAutoTuning.m_nGen8CommandScriptType == 1 || ParamAutoTuning.m_nGen8CommandScriptType == 2))
            {
                OutputMessage("-Check User Defined File");

                string sFilePath = string.Format(@"{0}\{1}", m_cfrmMain.m_sCmdDirectoryPath, ParamAutoTuning.m_sGen8UserDefinedPath);

                if (File.Exists(sFilePath) == false)
                    sFilePath = ParamAutoTuning.m_sGen8UserDefinedPath;

                if (File.Exists(sFilePath) == false)
                {
                    m_sErrorMessage = "User Defined Command Script File Not Exist";
                    OutputMessage(string.Format("-{0}", m_sErrorMessage));
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }

                bool bResultFlag = m_cElanCommand_Gen8.CheckUserDefinedFile(cFlowStep_List, sFilePath);

                if (bResultFlag == false)
                {
                    m_sErrorMessage = m_cElanCommand_Gen8.GetErrorMessage();
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get touch panel (TP) information for the specified main tuning step
        /// </summary>
        /// <param name="eMainStep">The flow step</param>
        /// <param name="nStepIndex">The index of the step</param>
        /// <returns>True if the TP information retrieval is successful, false otherwise</returns>
        private bool GetTPInformation(FlowStep cFlowStep, int nStepIndex)
        {
            if (m_nICSolutionType != MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                return true;

            MainTuningStep eMainStep = cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cFlowStep.m_eSubStep;

            int nRowNumber = GetRowNumber_Gen8(eMainStep, eSubStep);

            if ((eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1) || nRowNumber > m_nTXTraceNumber)
            {
                OutputMessage("-Get TP Information");

                IntPtr pTraceInfo = Marshal.AllocHGlobal(Marshal.SizeOf(m_structTraceInfo));

                if (ElanTouch.GetTraceInfo(pTraceInfo, ElanDefine.DEFAULT_PARTIAL_NUM, ElanDefine.TIME_1SEC, m_cfrmMain.m_nDeviceIndex) != ElanTouch.TP_SUCCESS)
                {
                    m_sErrorMessage = "Get TP Information Error";
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }

                m_structTraceInfo = (ElanTouch.TraceInfo)Marshal.PtrToStructure(pTraceInfo, typeof(ElanTouch.TraceInfo));
                Marshal.FreeHGlobal(pTraceInfo);
            }

            return true;
        }

        private void SetTPInformation(FlowStep cFlowStep, int nStepIndex)
        {
            if (m_nICSolutionType != MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                return;

            MainTuningStep eMainStep = cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cFlowStep.m_eSubStep;

            if (eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1)
                return;

            int nRowNumber = GetRowNumber_Gen8(eMainStep, eSubStep);

            if (nRowNumber > m_nTXTraceNumber)
                SetTraceInformation(m_nRXTraceNumber, nRowNumber);
        }

        private int GetRowNumber_Gen8(MainTuningStep eMainStep, SubTuningStep eSubStep)
        {
            int nRowNumber = m_nTXTraceNumber;

            if (eMainStep == MainTuningStep.NO)
                nRowNumber = ParamAutoTuning.m_nGen8BeaconRowNumber;
            else if (eMainStep == MainTuningStep.TILTNO)
            {
                if (eSubStep == SubTuningStep.TILTNO_PTHF)
                    nRowNumber = ParamAutoTuning.m_nGen8PTHFRowNumber * 2;
                else if (eSubStep == SubTuningStep.TILTNO_BHF)
                    nRowNumber = ParamAutoTuning.m_nGen8BHFRowNumber * 2;
            }

            return nRowNumber;
        }

        private bool GetTraceNumber(FlowStep cFlowStep, int nStepIndex)
        {
            MainTuningStep eMainStep = cFlowStep.m_eMainStep;

            if (m_nRXTraceNumber == 0)
            {
                // Check the RX Trace number
                int nRXTraceNumber = 0;

                if (GetRXTXTraceNumber(ref nRXTraceNumber, MainConstantParameter.m_nTRACETYPE_RX) == false)
                {
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }

                m_nRXTraceNumber = nRXTraceNumber;

                if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8 && eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1)
                    //m_nRXTraceNumber = ParamAutoTuning.m_nGen8RealTraceNumber;
                    m_nRXTraceNumber = nRXTraceNumber;
            }

            if (m_nTXTraceNumber == 0)
            {
                // Check the TX Trace number
                int nTXTraceNumber = 0;

                if (GetRXTXTraceNumber(ref nTXTraceNumber, MainConstantParameter.m_nTRACETYPE_TX) == false)
                {
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }

                m_nTXTraceNumber = nTXTraceNumber;

                if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8 && eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1)
                    m_nTXTraceNumber = ParamAutoTuning.m_nGen8BeaconRowNumber;
            }

            return true;
        }

        private bool GetStartTrace(int nStepIndex)
        {
            if (m_nICSolutionType != MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                return true;

            int nRXStartTrace = 0, nTXStartTrace = 0;

            m_cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmMain);

            if (m_cElanCommand_Gen8.GetTXRXStartTrace_Gen8(ref nRXStartTrace, ref nTXStartTrace) == false)
            {
                m_sErrorMessage = "Get Start Trace Error";
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return false;
            }

            m_nRXStartTrace = nRXStartTrace;
            m_nTXStartTrace = nTXStartTrace;

            return true;
        }

        /// <summary>
        /// Check the robot parameter for the specified step index
        /// </summary>
        /// <param name="nStepIndex">The index of the step</param>
        /// <returns>True if the robot parameter is valid, false otherwise</returns>
        private bool CheckRobotParameter(int nStepIndex)
        {
            if (nStepIndex == 0)
            {
                OutputMessage("-Check Robot Parameter");

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                {
                    if (m_cLTRobotParameter == null)
                    {
                        m_sErrorMessage = "LT Robot Parameter Setting Error";
                        m_cFinishFlowParameter.m_bErrorFlag = true;
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                        m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                        m_cfrmMain.m_bErrorFlag = true;
                        Thread.Sleep(10);
                        return false;
                    }
                }
                else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                {
                    if (m_cGoDrawParameter == null)
                    {
                        m_sErrorMessage = "GoDraw Parameter Setting Error";
                        m_cFinishFlowParameter.m_bErrorFlag = true;
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                        m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                        m_cfrmMain.m_bErrorFlag = true;
                        Thread.Sleep(10);
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Load the user-defined command script for the specified flow step and data index
        /// </summary>
        /// <param name="cFlowStep">The flow step</param>
        /// <param name="nStepIndex">The index of the step</param>
        /// <param name="nDataIndex">The index of the data</param>
        /// <returns>True if the user-defined command script is loaded successfully, false otherwise</returns>
        private bool LoadUserDefinedCommandScript(FlowStep cFlowStep, int nStepIndex, int nDataIndex)
        {
            if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8 && (ParamAutoTuning.m_nGen8CommandScriptType == 1 || ParamAutoTuning.m_nGen8CommandScriptType == 2))
            {
                string sFilePath = string.Format(@"{0}\{1}", m_cfrmMain.m_sCmdDirectoryPath, ParamAutoTuning.m_sGen8UserDefinedPath);

                if (File.Exists(sFilePath) == false)
                    sFilePath = ParamAutoTuning.m_sGen8UserDefinedPath;

                if (File.Exists(sFilePath) == false)
                {
                    m_sErrorMessage = "User Defined Command Script File Not Exist";
                    OutputMessage(string.Format("-{0}", m_sErrorMessage));
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }

                bool bResultFlag = m_cElanCommand_Gen8.LoadUserDefinedCommandScript(ref m_cSendCommandInfo, cFlowStep, nDataIndex, sFilePath);

                if (bResultFlag == false)
                {
                    m_sErrorMessage = m_cElanCommand_Gen8.GetErrorMessage();
                    OutputMessage(string.Format("-{0}", m_sErrorMessage));
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Generate the log file path for the specified step and data index
        /// </summary>
        /// <param name="nStepIndex">The index of the step</param>
        /// <param name="nDataIndex">The index of the data</param>
        /// <returns>True if the log file path is set successfully, false otherwise</returns>
        private bool SetDataFilePath(int nStepIndex, int nDataIndex)
        {
            if (Directory.Exists(m_cfrmMain.m_sFileDirectoryPath) == false)
                Directory.CreateDirectory(m_cfrmMain.m_sFileDirectoryPath);

            SubTuningStep eSubStep = RecordSetInfo.m_cRecordSetParameter_List[nDataIndex].m_eSubStep;
            string sSubStepName = StringConvert.m_dictSubStepMappingTable[eSubStep];
            string sSubStepCodeName = StringConvert.m_dictSubStepCNMappingTable[eSubStep];

            int nDataNumber = nDataIndex + 1;

            if (sSubStepName == "Else")
            {
                m_sErrorMessage = string.Format("Record Data Set {0} Setting Error", nDataNumber);
                OutputMessage(string.Format("-{0}", m_sErrorMessage));
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return false;
            }

            m_sStepDirectoryPath = string.Format(@"{0}\\{1}", m_cfrmMain.m_sFileDirectoryPath, sSubStepName);

            if (Directory.Exists(m_sStepDirectoryPath) == false)
                Directory.CreateDirectory(m_sStepDirectoryPath);

            if (sSubStepCodeName != "Else")
                sSubStepCodeName += string.Format("_", sSubStepCodeName);

            string sText = "_";

            if (eSubStep == SubTuningStep.NO)
                sText = "KHz_";

            int nPH1 = RecordSetInfo.m_cRecordSetParameter_List[nDataIndex].m_nPH1;
            int nPH2 = RecordSetInfo.m_cRecordSetParameter_List[nDataIndex].m_nPH2;
            string sFrequency = ElanConvert.ComputeFrequnecyToString(nPH1, nPH2);
            sText = string.Format("{0}{1}", sFrequency, sText);

            string sPH1 = Convert.ToString(nPH1);
            sPH1 = sPH1.ToUpper();
            sPH1 = string.Format("{0}_", sPH1.PadLeft(2, '0'));

            string sPH2 = Convert.ToString(nPH2);
            sPH2 = sPH2.ToUpper();
            sPH2 = sPH2.PadLeft(2, '0');

            m_sLogFileName = string.Format("{0}{1}{2}{3}", sSubStepCodeName, sText, sPH1, sPH2);
            m_sLogFilePath = string.Format(@"{0}\{1}", m_sStepDirectoryPath, m_sLogFileName);

            string sNote = RecordSetInfo.m_cRecordSetParameter_List[nDataIndex].m_sNote;

            if (sNote != "")
            {
                m_sLogFileName = string.Format("{0}_{1}", m_sLogFileName, sNote);
                m_sLogFilePath = string.Format(@"{0}_{1}", m_sLogFilePath, sNote);
            }

            return true;
        }

        /// <summary>
        /// Create an "Info.txt" file in the Log folder with the measurement information for the current stage (ICSolution)
        /// </summary>
        /// <param name="cFlowStep">The current flow step</param>
        private void WriteInfoFile(FlowStep cFlowStep)
        {
            string sFilePath = string.Format(@"{0}\{1}", m_cfrmMain.m_sFileDirectoryPath, frmMain.m_sInfoTxtFile);

            string sICSolution = "None";

            // Determine the ICSolution based on the value of m_nICSolutionType
            switch (m_nICSolutionType)
            {
                case MainConstantParameter.m_nICSOLUTIONTYPE_GEN8:
                    sICSolution = "Gen8";
                    break;
                case MainConstantParameter.m_nICSOLUTIONTYPE_OTHER:
                    sICSolution = "Other";
                    break;
                case MainConstantParameter.m_nICSOLUTIONTYPE_NONE:
                    sICSolution = "None";
                    break;
            }

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                // Write the ICSolution information to the file
                sw.WriteLine(string.Format("ICSolution={0}", sICSolution));
            }
            finally
            {
                // Flush and close the StreamWriter and FileStream
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// Set the trace information for the given RX and TX trace numbers
        /// </summary>
        /// <param name="nRXTraceNumber">Number of RX traces</param>
        /// <param name="nTXTraceNumber">Number of TX traces</param>
        private void SetTraceInformation(int nRXTraceNumber, int nTXTraceNumber)
        {
            int nChipNumber = 1;

            if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                nChipNumber = 1;
            else if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_OTHER)
                nChipNumber = 1;

            // Set the total number of X and Y traces
            m_structTraceInfo.nYTotal = nTXTraceNumber;
            m_structTraceInfo.nXTotal = nRXTraceNumber;

            if (nChipNumber > 0)
            {
                // Set the X-axis total and the number of RX traces for chip 1
                //m_structTraceInfo.nXTotal += nRXTraceNumber;
                m_structTraceInfo.XAxis[0] = nRXTraceNumber;
            }

            if (nChipNumber > 1)
            {
                // Set the X-axis total and the number of RX traces for chip 2
                //m_structTraceInfo.nXTotal += nRXTraceNumber;
                m_structTraceInfo.XAxis[1] = nRXTraceNumber;
            }

            if (nChipNumber > 2)
            {
                // Set the X-axis total and the number of RX traces for chip 3
                //m_structTraceInfo.nXTotal += nRXTraceNumber;
                m_structTraceInfo.XAxis[2] = nRXTraceNumber;
            }

            m_structTraceInfo.nPartialNum = 0;
            m_structTraceInfo.nChipNum = nChipNumber;

            // Allocate memory for the trace information structure and set its values
            //Set the trace information to library
            IntPtr npTraceInfo = Marshal.AllocHGlobal(Marshal.SizeOf(m_structTraceInfo));
            Marshal.StructureToPtr(m_structTraceInfo, npTraceInfo, true);
            // Set the trace information in the library
            ElanTouch.SetTraceInfo(npTraceInfo);
            // Free the allocated memory
            Marshal.FreeHGlobal(npTraceInfo);
        }

        /// <summary>
        /// Set the output information for the recording of the Report Data stage
        /// </summary>
        /// <param name="nRecordStep">Recording step for the Report Data</param>
        /// <param name="cRecordSetParameter">Information for this stage</param>
        private void SetStateMessage(int nRecordStep, RecordSetParameter cRecordSetParameter)
        {
            MainTuningStep eMainStep = cRecordSetParameter.m_eMainStep;
            SubTuningStep eSubStep = cRecordSetParameter.m_eSubStep;
            double dFrequency = cRecordSetParameter.m_dFrequency;

            if (nRecordStep == m_nRECORD_FRONTSETTING)
            {
                if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO)
                {
                    if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    {
                        string sSubStep = StringConvert.m_dictSubStepMappingTable[m_cfrmMain.m_cCurrentFlowStep.m_eSubStep];

                        string sDescription = "";

                        if (eMainStep == MainTuningStep.NO)
                        {
                            if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                                sDescription = string.Format("({0:0.000}KHz {1})", dFrequency, "RX");
                            else if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                                sDescription = string.Format("({0:0.000}KHz {1})", dFrequency, "TX");
                        }
                        else if (eMainStep == MainTuningStep.TILTNO)
                        {
                            if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                            {
                                if (cRecordSetParameter.m_nSection >= 0 && cRecordSetParameter.m_nSection < 4)
                                {
                                    string sSection = string.Format("S{0}", cRecordSetParameter.m_nSection);
                                    sDescription = string.Format("({0:0.000}KHz {1} {2})", dFrequency, "RX", sSection);
                                }
                                else
                                    sDescription = string.Format("({0:0.000}KHz {1})", dFrequency, "RX");
                            }
                            else if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                                sDescription = string.Format("({0:0.000}KHz {1})", dFrequency, "TX");
                        }

                        string sMessage = string.Format("{0}{1}", sSubStep, sDescription);

                        OutputStateMessage(sMessage, false, false, true);
                    }
                    else
                    {
                        string sSubStep = StringConvert.m_dictSubStepMappingTable[m_cfrmMain.m_cCurrentFlowStep.m_eSubStep];
                        string sDescription = string.Format("({0:0.000}KHz)", dFrequency);
                        string sMessage = string.Format("{0}{1}", sSubStep, sDescription);

                        OutputStateMessage(sMessage, false, false, true);
                    }
                }
                else if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                         eMainStep == MainTuningStep.TPGAINTUNING ||
                         eMainStep == MainTuningStep.TILTTUNING ||
                         eMainStep == MainTuningStep.LINEARITYTUNING)
                {
                    string sSubStep = StringConvert.m_dictSubStepMappingTable[m_cfrmMain.m_cCurrentFlowStep.m_eSubStep];
                    string sPatternType = StringConvert.m_dictPatternTypeMappingTable[m_cfrmMain.m_cCurrentFlowStep.m_nPatternType];

                    string sMessage = string.Format("{0}{1}", sSubStep, sPatternType);

                    if (cRecordSetParameter.m_eMainStep == MainTuningStep.TPGAINTUNING)
                    {
                        string sAngle = StringConvert.m_dictTPGTRAngleMappingTable[m_cfrmMain.m_cCurrentFlowStep.m_nPatternType].ToString();
                        sMessage = string.Format("{0}(V:{1}, R:{2})", sMessage, ParamAutoTuning.m_nTPGTVAngle, sAngle);
                    }

                    OutputStateMessage(sMessage, false, false, true);
                }
            }
            else if (eMainStep == MainTuningStep.PRESSURETUNING)
            {
                //if (eSubStep == SubTuningStep.PRESSURESETTING || (nRecordStep == m_nRECORD_FRONTSETTING && eSubStep == SubTuningStep.PRESSURETABLE))
                if (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE)
                {
                    m_cfrmMain.m_cCurrentFlowStep.m_nPressureWieght = cRecordSetParameter.m_nWeight;

                    string sSubStep = StringConvert.m_dictSubStepMappingTable[m_cfrmMain.m_cCurrentFlowStep.m_eSubStep];
                    string sWeight = Convert.ToString(cRecordSetParameter.m_nWeight);
                    string sIQ_BSH_P = "";

                    if (eSubStep == SubTuningStep.PRESSURESETTING && m_cCurrentParameterSet.m_nSIQ_BSH_P >= 0)
                    {
                        m_cfrmMain.m_cCurrentFlowStep.m_nParamIQ_BSH_P = m_cCurrentParameterSet.m_nSIQ_BSH_P;
                        sIQ_BSH_P = string.Format("[IQ_BSH_P={0}]", m_cCurrentParameterSet.m_nSIQ_BSH_P);
                    }

                    string sMessage = string.Format("{0}{1}({2}g)", sSubStep, sIQ_BSH_P, sWeight);
                    OutputStateMessage(sMessage, false, false, true);
                }
            }
        }

        /// <summary>
        /// Store a single Report Data array into the Report Data List
        /// </summary>
        /// <param name="byteReport_Array">Report Data array</param>
        private void SetReportDataList(byte[] byteReport_Array)
        {
            m_byteSingleReport_List.Clear();

            foreach (byte byteReport in byteReport_Array)
                m_byteSingleReport_List.Add(byteReport);

            m_byteReportData_List.Add(new List<byte>(m_byteSingleReport_List));
        }

        /// <summary>
        /// Checks if the step data is valid
        /// </summary>
        /// <param name="nDataIndex">The index of the data</param>
        /// <param name="nRetryCount">The retry count</param>
        /// <param name="cFlowStep">The flow step</param>
        /// <param name="nStepIndex">The step index</param>
        /// <param name="nDataNumber">The data number</param>
        private void CheckStepDataIsValid(ref int nDataIndex, ref int nRetryCount, FlowStep cFlowStep, int nStepIndex, int nDataNumber)
        {
            m_bLastRetryFlag = false;

            // Get the error flag for the record data
            int nRecordDataErrorFlag = GetRecordDataErrorFlag();

            // If the error flag is either 0 or -1, reset the retry count and return
            if (!(nRecordDataErrorFlag == 0 || nRecordDataErrorFlag != -1))
            {
                nRetryCount = 0;
                return;
            }

            // Get the main and sub tuning steps
            MainTuningStep eMainStep = cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cFlowStep.m_eSubStep;

            // Handle the case where the record data error flag is 0
            if (nRecordDataErrorFlag == 0)
            {
                nRetryCount = 0;

                // Update error message and output error message
                m_sErrorMessage = string.Format("{0} in Record Data Set : {1}, Please Restart Device or Check FW", m_sErrorMessage, nDataNumber);
                OutputMessage(string.Format("-Record Error in Record Data Set : {0}.", nDataNumber));

                nDataIndex--;

                // Set the finish flow parameters and error flags
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }

            // Handle the case where the record data error flag is not -1
            if (nRecordDataErrorFlag != -1)
            {
                // Check if the retry count is within the limit
                if (nRetryCount < ParamAutoTuning.m_nRecordDataRetryCount)
                {
                    // Check specific conditions for pressure tuning and pressure setting steps
                    if (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURESETTING && (nRecordDataErrorFlag == 2 || nRecordDataErrorFlag == 3))
                    {
                        bool bForceStopErrorFlag = false;

                        // Handle error flag 2
                        if (nRecordDataErrorFlag == 2)
                        {
                            if ((m_cCurrentParameterSet.m_nSIQ_BSH_P <= ParamAutoTuning.m_nPTIQ_BSH_P_LB ||
                                 m_cCurrentParameterSet.m_nRIQ_BSH_P > m_cCurrentParameterSet.m_nROrgIQ_BSH_P))
                                bForceStopErrorFlag = true;
                            else
                            {
                                m_cCurrentParameterSet.m_nBefIQ_BSH_P = m_cCurrentParameterSet.m_nRIQ_BSH_P;
                                m_cCurrentParameterSet.m_nSIQ_BSH_P--;
                            }
                        }
                        // Handle error flag 3
                        else if (nRecordDataErrorFlag == 3)
                        {
                            if ((m_cCurrentParameterSet.m_nSIQ_BSH_P >= ParamAutoTuning.m_nPTIQ_BSH_P_HB ||
                                 m_cCurrentParameterSet.m_nRIQ_BSH_P < m_cCurrentParameterSet.m_nROrgIQ_BSH_P))
                                bForceStopErrorFlag = true;
                            else
                            {
                                m_cCurrentParameterSet.m_nBefIQ_BSH_P = m_cCurrentParameterSet.m_nRIQ_BSH_P;
                                m_cCurrentParameterSet.m_nSIQ_BSH_P++;
                            }
                        }

                        // Handle force stop error flag
                        if (bForceStopErrorFlag == true)
                        {
                            nRetryCount = 0;
                            string sHintMessage = string.Format("Can't Find Suitable _Pen_Ntrig_IQ_BSH_P[{0}]", m_cCurrentParameterSet.m_nRIQ_BSH_P);
                            OutputMessage(string.Format("-{0}", sHintMessage));

                            if (ParamAutoTuning.m_nFlowMethodType != 1)
                            {
                                m_cfrmMain.m_sErrorMessage = string.Format("{0}.{1}", m_sErrorMessage, sHintMessage);

                                nDataIndex--;

                                // Set the finish flow parameters and error flags
                                m_cFinishFlowParameter.m_bErrorFlag = true;
                                m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                                m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                                m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                                m_cfrmMain.m_bErrorFlag = true;
                                Thread.Sleep(10);
                            }
                            else
                            {
                                nRetryCount = 0;

                                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
                                {
                                    string sWarningMessage = string.Format("{0}. Please Retry Again!!", m_sErrorMessage);
                                    ShowMessageBox(sWarningMessage);
                                }
                                else
                                    OutputMessage(string.Format("-{0}", m_sErrorMessage));
                            }

                            m_sErrorMessage = string.Format("{0}. {1}", m_sErrorMessage, sHintMessage);

                            nDataIndex--;

                            // Set the finish flow parameters and error flags
                            m_cFinishFlowParameter.m_bErrorFlag = true;
                            m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                            m_cFinishFlowParameter.m_bStateMessageFlag = true;
                            m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                            m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                            m_cfrmMain.m_bErrorFlag = true;
                            Thread.Sleep(10);
                        }
                    }
                    else
                    {
                        // Handle other steps
                        if (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURETABLE)
                        {
                            if (nDataIndex == 0)
                            {
                                m_bDisableRobotMovingFlag = false;
                                m_bDisableSetCommandFlag = false;
                            }
                        }
                        else
                            m_bDisableRobotMovingFlag = false;

                        if (nRetryCount == ParamAutoTuning.m_nRecordDataRetryCount - 1)
                            m_bLastRetryFlag = true;

                        nRetryCount++;

                        if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE && (eMainStep != MainTuningStep.NO && eMainStep != MainTuningStep.TILTNO))
                        {
                            string sWarningMessage = string.Format("{0}. Please Retry Again!!", m_sErrorMessage) + Environment.NewLine + string.Format("(Retry Count = {0})", nRetryCount);
                            ShowMessageBox(sWarningMessage);
                        }
                        else
                        {
                            OutputMessage(string.Format("-{0} in Record Data Set : {1}", m_sErrorMessage, nDataIndex + 1));
                        }
                    }

                    nDataIndex--;
                }
                else
                {
                    // Handle case when retry count exceeds the limit
                    nRetryCount = 0;

                    OutputMessage(string.Format("-Record Error in Record Data Set : {0}", nDataIndex + 1));

                    if (ParamAutoTuning.m_nFlowMethodType != 1)
                    {
                        m_sErrorMessage = string.Format("{0} in Record Data Set : {1}", m_sErrorMessage, nDataIndex + 1);

                        nDataIndex--;

                        // Set the finish flow parameters and error flags
                        m_cFinishFlowParameter.m_bErrorFlag = true;
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                        m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                        m_cfrmMain.m_bErrorFlag = true;
                        Thread.Sleep(10);
                    }
                }
            }
        }
        #endregion
    }
}
