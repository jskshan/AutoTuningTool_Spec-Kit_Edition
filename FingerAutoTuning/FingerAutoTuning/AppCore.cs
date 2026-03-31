using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Data;
using System.Net;
using System.Net.Sockets;
using BlockingQueue;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        #region 開關顯示器
        public int WM_SYSCOMMAND = 0x0112;
        public int SC_MONITORPOWER = 0xF170;

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);
        #endregion

        #region 設定滑鼠位置
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);
        #endregion

        #region 模擬滑鼠事件
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        #endregion

        //Main Object
        private InputDevice m_cInputDevice = null;
        private DebugLogAPI m_cDebugLog = null;
        private DataAnalysis m_cDataAnalysis = null;
        private frmMain m_cfrmParent = null;

        private ElanSSHClient m_cElanSSHClient = null;

        private bool m_bSocketConnectType = false;

        //Flow Step List
        private List<frmMain.FlowStep> m_cFlowStep_List = null;

        //private bool m_bFingerReportTest = false;
        private FrameMgr m_cFrameMgr = new FrameMgr();
        private BlockingQueue.BlockingQueue<Frame> m_bqcFrameQueue = new BlockingQueue.BlockingQueue<Frame>();
        private BlockingQueue.BlockingQueue<byte> m_bqbyteFIFO = new BlockingQueue.BlockingQueue<byte>();

        //private bool m_bEnablePollingDummyCommand = false;

        //Option Boolean
        private bool m_bStartRecord = false;
        private bool m_bEnterTestMode = false;
        private bool m_bDisableReport = false;
        private bool m_bMoveDataError = false;
        private bool m_bCopyDataError = false;
        private bool m_bTPConnected = false;
        private bool m_bCreateNewFolder = false;
        private bool m_bKeepNotReset = false;
#if _USE_9F07_SOCKET
        private bool m_bDisableScanMode_9F07 = false;
#endif

        /// <summary>
        /// Store the finger or pen report into log.
        /// </summary>
        private StreamWriter m_swReportLog = null;
        private object m_objReportLocker = new object();

        /*
        private StreamWriter m_swRecordLog = null;
        private object m_objRecordLocker = new object();
        private int m_nRecordCount = 0;
        private FileStream m_fs = null;
        */
        private SaveData m_cSaveData = null;

        #region Remote Server Code
        private bool m_bSocketConnected = false;
        public bool SocketConnected
        {
            get { return m_bSocketConnected; }
        }
        //J2++
        //A object to run the android remote server.
        private BaseExecServer m_cRunServerMgr = null;
        //J2--
        //private bool m_bSocketConnected = false;
        /// <summary>
        /// flag: The thread runing to receive Finger Report from socket.
        /// </summary>
        private bool m_bRecvSocketFingerReport = false;

        private bool m_bSocketWaitCallback = false;
        public bool SocketWaitCallback
        {
            get { return m_bSocketWaitCallback; }
        }
        #endregion

        private int m_nDeviceIndex = -1;

        private string m_sLogDirectoryPath = "";
        private string m_sH5LogDirectoryPath = "";
        private int m_nStartIndex = 0;

        private ElanTouch.TraceInfo m_structTraceInfo = new ElanTouch.TraceInfo(ElanTouch.MAX_CHIP_NUM);
        private ElanTouch_Socket.TraceInfo m_structTraceInfo_Socket = new ElanTouch_Socket.TraceInfo(ElanTouch_Socket.MAX_CHIP_NUM);
        private int m_nICType = 0;

        private AppCoreDefine.FWParameter m_cOriginParameter = new AppCoreDefine.FWParameter();
        private AppCoreDefine.FWParameter m_cReadParameter = new AppCoreDefine.FWParameter();

        private int m_nReadProjectOption = 0;
        private int m_nReadFWIPOption = 0;

        private int m_nRXTraceNumber = 0;
        private int m_nTXTraceNumber = 0;

        //Monitor On/Off Option
        //private int m_nDisableMonitor = 0;

        //Error Message String
        public bool m_bFlowComplete = true;

        //Error Message String
        public string m_sErrorMessage = "";

        //Pattern Object
        private frmPHCKPattern m_cfrmPHCKPattern = null;
        private frmFullScreen m_cfrmFullScreen = null;

        //Show Pattern Status
        public bool m_bStartShowFullScreen = false;

        public string m_sStepName = "";
        public int m_nCurrentExecuteIndex = 0;
        public int m_nTotalCount = 0;

        private string m_sStartTime;

        private string m_sMutualFrequencyPath = string.Format(@"{0}\{1}\FreqSet\FreqSet.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
        private string m_sACFrequencyPath = string.Format(@"{0}\{1}\FreqSet\ACFreqSet.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
        private string m_sRawADCSweepPath = string.Format(@"{0}\{1}\FreqSet\RawADCSweepSet.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
        private string m_sSelfFrequencyPath = string.Format(@"{0}\{1}\FreqSet\SelfFreqSet.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
        private string m_sSkipFrequencyPath = string.Format(@"{0}\{1}\FreqSet\SkipFreqSet.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName);

        private Stopwatch m_swSingleStep = new Stopwatch();//引用stopwatch物件;
        private TimeSpan m_tsPreviousStepTimeSpan;

        private AppCoreDefine.TimerCount m_cTotalTimerCount = new AppCoreDefine.TimerCount();

        private List<string> m_sGetData_List = null;

        public List<FrequencyItem> m_cFreqencyItem_List = null;         //new List<FrequencyItem>();
        public List<RawADCSweepItem> m_cRawADCSweepItem_List = null;    //new List<RawADCSweepItem>();

        private bool m_bGenerateH5Data = false;

        //After Send Command Delay Time(ms)
        private int m_nNormalDelayTime = 10;

        private ICGenerationType m_eICGenerationType = ICGenerationType.None;
        private ICSolutionType m_eICSolutionType = ICSolutionType.NA;

        private UInt32 m_nFWVersion = 0x0000;

        private ElanCommand_Gen8.SendCommandInfo m_cSendCommandInfo = null;

        private TraceType m_eSelfTraceType = TraceType.RX;
        private AppCoreDefine.SelfParameter m_cSelfParameter = new AppCoreDefine.SelfParameter();

        //private List<byte[]> m_byteReport_List = null;

        private bool m_bGetNameByFrequencyList = false;
        private string m_sProjectName = "";

        private bool m_bRegistHIDDevice = false;

        private bool m_bLastRun = true;

        private string m_sCommandScriptFilePath = "";
        private bool m_bGetCommandScriptFile = false;
        
        private int m_nSelfGetReportSequence = -1;
        private int m_nSelfNCPValue = -1;
        private int m_nSelfNCNValue = -1;

        private bool m_bSetPenFunctionOFF_8F18 = false;

        private ThreadStart m_tsKeepWakeUp = null;
        private Thread m_tKeepWakeUp = null;

        private bool m_bGetLocalIPAddressFlag = false;
        private string m_sLocalIPAddress = "";

        private bool m_bGetFirstDataFlag = true;

        private int m_nNoResetBaseFunctionFlag_Gen8 = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cInputDevice"></param>
        /// <param name="cDebugLog"></param>
        /// <param name="cDataAnalysis"></param>
        /// <param name="cfrmParent"></param>
        public AppCore(ref InputDevice cInputDevice, ref DebugLogAPI cDebugLog, ref DataAnalysis cDataAnalysis, frmMain cfrmParent)
        {
            m_cInputDevice = cInputDevice;
            m_cDebugLog = cDebugLog;
            m_cDataAnalysis = cDataAnalysis;
            m_cfrmParent = cfrmParent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objParameter"></param>
        public void ExecuteMainWorkFlow(object objParameter)
        {
            List<string> sMultiTestFilePath_List = new List<string>();
            m_bLastRun = true;
            m_bFlowComplete = true;
            m_sErrorMessage = "";

            m_bGetLocalIPAddressFlag = false;
            m_sLocalIPAddress = "";

            if (SetMultiTestFileList(ref sMultiTestFilePath_List) == false)
                return;

            for (int nFileIndex = 0; nFileIndex < sMultiTestFilePath_List.Count; nFileIndex++)
            {
                m_swSingleStep.Reset();//碼表歸零
                m_swSingleStep.Start();//碼表開始計時

                OutputlblStatus("Execute", "", Color.Blue, true);

                string sFilePath = sMultiTestFilePath_List[nFileIndex];
                bool bCleanFlowStepList = (nFileIndex == 0) ? true : false;
                m_bLastRun = (nFileIndex < sMultiTestFilePath_List.Count - 1) ? false : true;

                InitializeSetting(objParameter, bCleanFlowStepList);

                KeepAndroidWakeUp();

                if (RunFrontWork(sFilePath) == false)
                    return;

                if (ConnectDevice(frmMain.m_nCurrentStepIndex) == false)
                    return;

                for (int nStepIndex = m_nStartIndex; nStepIndex < m_cFlowStep_List.Count; nStepIndex++)
                {
                    frmMain.FlowStep cFlowStep = m_cFlowStep_List[nStepIndex];

                    SetStepState(cFlowStep, nStepIndex, sFilePath);

                    if (DeleteStepFile(cFlowStep.m_eStep) == false)
                        return;

                    if (LoadLogData(cFlowStep, nStepIndex) == false)
                        return;

                    if (GetFreqencyListInfo(cFlowStep, nStepIndex) == false)
                        return;

                    if (SetRecordDirectory(cFlowStep, nStepIndex, m_nStartIndex) == false)
                        return;

                    if (SetRecordData(cFlowStep, nStepIndex) == false)
                        return;

                    if (MoveLogDataAndWriteResultList(cFlowStep, nStepIndex) == false)
                        return;

                    if (LoadFreqencyList(cFlowStep, nStepIndex) == false)
                        return;

                    if (CheckStepSupported(cFlowStep, nStepIndex) == false)
                        return;

                    if (CheckUserDefinedFile(m_cFlowStep_List, nStepIndex) == false)
                        return;

                    if (ExecuteRecordData(cFlowStep) == false)
                        m_bFlowComplete = false;

                    if (m_bFlowComplete == true)
                    {
                        if (m_cDataAnalysis.ExecuteMainWorkFlow(
                            ref m_sErrorMessage, m_cFlowStep_List[nStepIndex], m_sLogDirectoryPath, m_sH5LogDirectoryPath, m_bGenerateH5Data, 
                            m_cfrmParent, m_sProjectName, m_sSkipFrequencyPath) == false)
                            m_bFlowComplete = false;
                    }

                    if (m_bFlowComplete == true)
                    {
                        if (OutputResultData(cFlowStep) == false)
                            m_bFlowComplete = false;

                        if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                        {
                            m_cfrmParent.m_bReset = true;
                            m_cfrmParent.m_eRecordState = AppCoreDefine.RecordState.NORMAL;
                            frmMain.m_nCurrentStepIndex = 0;
                            m_cfrmParent.m_nCurrentExecuteIndex = 0;
                        }
                    }

                    if (m_bFlowComplete == false || m_sErrorMessage != "")
                    {
                        SetStepCostTime(nStepIndex, true);
                        break;
                    }
                    else
                        SetStepCostTime(nStepIndex);

                    m_cfrmParent.m_nCurrentExecuteIndex = 0;

                    if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank && nStepIndex < m_cFlowStep_List.Count - 1)
                    {
                        ShowWarningMessage("Please Remove the Fixture from the Screen.");
                    }
                }

                RunOutputResult(m_bFlowComplete);

                if (m_bFlowComplete == false || m_sErrorMessage != "")
                    return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMutltTestFilePath_List"></param>
        /// <returns></returns>
        private bool SetMultiTestFileList(ref List<string> sMutltTestFilePath_List)
        {
            if (m_cfrmParent.m_bLoadData == true)
            {
                sMutltTestFilePath_List.Add("NA");
            }
            else if (ParamFingerAutoTuning.m_nRunMultiTest == 1)
            {
                OutputMessage("[State]Get MultiTest File Path");

                string sMultiTestDirecotryPath = string.Format(@"{0}\{1}\FreqSet\MultiTest", Application.StartupPath, frmMain.m_sAPMainDirectoryName);

                if (Directory.Exists(sMultiTestDirecotryPath) == false)
                {
                    m_sErrorMessage = "MultiTest Folder Not Exist";
                    SetOutputResult(m_nStartIndex, false);
                    return false;
                }

                foreach (string sFilePath in Directory.GetFiles(sMultiTestDirecotryPath, "*.txt"))
                    sMutltTestFilePath_List.Add(sFilePath);

                if (sMutltTestFilePath_List.Count == 0)
                {
                    m_sErrorMessage = "No Any MultiTest File";
                    SetOutputResult(m_nStartIndex, false);
                    return false;
                }
            }
            else
                sMutltTestFilePath_List.Add("NA");

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objParameter"></param>
        /// <param name="bCleanFlowStepList"></param>
        private void InitializeSetting(object objParameter, bool bCleanFlowStepList)
        {
            m_eICGenerationType = ICGenerationType.None;
            m_eICSolutionType = ICSolutionType.NA;

            m_bKeepNotReset = false;

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                m_bSocketConnectType = true;
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT ||
                     ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL ||
                     ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT)
                m_bSocketConnectType = true;
            else
                m_bSocketConnectType = false;

            //Setting After Send Command Delay Time by using Android System and SPI Interface
            if ((ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT ||
                 ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL) &&
                (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_RISING_HALF ||
                 ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_RISING ||
                 ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING_HALF ||
                 ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING))
                m_nNormalDelayTime = 100;

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
                m_nNormalDelayTime = 100;

            if (ParamFingerAutoTuning.m_nNormalDelayTime > 0)
                m_nNormalDelayTime = ParamFingerAutoTuning.m_nNormalDelayTime;

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                m_nNormalDelayTime = ParamFingerAutoTuning.m_nSSHSocketServerNormalDelayTime;

#if _USE_9F07_SOCKET
            if (ParamFingerAutoTuning.m_nUseICSolutionType_9F07 == 1)
            {
                m_eICGenerationType = ICGenerationType.Gen9;
                m_eICSolutionType = ICSolutionType.Solution_9F07;
            }

            if (m_eICGenerationType == ICGenerationType.Gen9)
                m_nNormalDelayTime = ParamFingerAutoTuning.m_nNormalDelayTime_9F07;
#endif

            if (bCleanFlowStepList == true)
            {
                if (m_cFlowStep_List != null)
                    m_cFlowStep_List.Clear();

                m_cFlowStep_List = (List<frmMain.FlowStep>)objParameter;
                m_sLogDirectoryPath = "";
                m_nStartIndex = frmMain.m_nCurrentStepIndex;
            }

            m_bGetNameByFrequencyList = false;
            m_sProjectName = "";

            m_bRegistHIDDevice = false;

            m_sCommandScriptFilePath = "";
            m_bGetCommandScriptFile = false;
            
            m_nSelfGetReportSequence = -1;
            m_nSelfNCPValue = -1;
            m_nSelfNCNValue = -1;
            
            m_nFWVersion = 0x0000;

            m_nNoResetBaseFunctionFlag_Gen8 = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFilePath"></param>
        /// <returns></returns>
        private bool RunFrontWork(string sFilePath)
        {
            InitialParameter();

            if (m_cfrmParent.m_bLoadData == false)
            {
                if (CheckFrontWork(m_cFlowStep_List, sFilePath) == false)
                {
                    SetOutputResult(m_nStartIndex, false);
                    return false;
                }
            }

            if ((m_cfrmParent.m_bLoadData == false && (ParamFingerAutoTuning.m_nGenerateH5FileData == 1 || ParamFingerAutoTuning.m_nGenerateH5FileData == 2)) ||
                (m_cfrmParent.m_bLoadData == true && ParamFingerAutoTuning.m_nGenerateH5FileData == 2))
                m_bGenerateH5Data = true;
            else
                m_bGenerateH5Data = false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitialParameter()
        {
            m_cOriginParameter = new AppCoreDefine.FWParameter();
            m_cReadParameter = new AppCoreDefine.FWParameter();

            m_nReadProjectOption = 0;
            m_nReadFWIPOption = 0;

            m_cSelfParameter.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep_List"></param>
        /// <param name="sFilePath"></param>
        /// <returns></returns>
        private bool CheckFrontWork(List<frmMain.FlowStep> cFlowStep_List, string sFilePath)
        {
            OutputMessage("[State]Front Work Check");

            for (int nStepIndex = frmMain.m_nCurrentStepIndex; nStepIndex < cFlowStep_List.Count; nStepIndex++)
            {
                MainStep eStep = cFlowStep_List[nStepIndex].m_eStep;

                switch (eStep)
                {
                    case MainStep.FrequencyRank_Phase1:
                        break;
                    case MainStep.FrequencyRank_Phase2:
                    case MainStep.AC_FrequencyRank:
                        bool bValidDataFlag = false;
                        string sStepFilePath = m_sMutualFrequencyPath;

                        if (ParamFingerAutoTuning.m_nRunMultiTest == 1)
                            sStepFilePath = sFilePath;

                        if (File.Exists(sStepFilePath) == true)
                        {
                            string sLine = "";

                            StreamReader srFile = new StreamReader(sStepFilePath, Encoding.Default);

                            try
                            {
                                while ((sLine = srFile.ReadLine()) != null)
                                {
                                    string[] sSplit_Array = sLine.Split(',');

                                    if (sSplit_Array.Length >= 2)
                                    {
                                        if (ElanConvert.IsInt(sSplit_Array[1]) == true)
                                        {
                                            bValidDataFlag = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                srFile.Close();
                            }
                        }

                        if (bValidDataFlag == false)
                        {
                            m_sErrorMessage = string.Format("Frequency Set Error[Step:{0}]", cFlowStep_List[nStepIndex].m_sStepName);
                            return false;
                        }

                        break;
                    case MainStep.Raw_ADC_Sweep:
                        break;
                    case MainStep.Self_FrequencySweep:
                    case MainStep.Self_NCPNCNSweep:
                        bValidDataFlag = false;
                        sStepFilePath = m_sSelfFrequencyPath;

                        if (ParamFingerAutoTuning.m_nRunMultiTest == 1)
                            sStepFilePath = sFilePath;

                        if (File.Exists(sStepFilePath) == true)
                        {
                            string sLine = "";

                            StreamReader srFile = new StreamReader(sStepFilePath, Encoding.Default);

                            try
                            {
                                while ((sLine = srFile.ReadLine()) != null)
                                {
                                    string[] sSplit_Array = sLine.Split(',');

                                    if (sSplit_Array.Length >= 8)
                                    {
                                        for (int nSplitIndex = 1; nSplitIndex <= 5; nSplitIndex++)
                                        {
                                            if (ElanConvert.IsInt(sSplit_Array[nSplitIndex]) == true)
                                            {
                                                bValidDataFlag = true;
                                                break;
                                            }
                                        }

                                        if (bValidDataFlag == true)
                                            break;
                                    }
                                }
                            }
                            finally
                            {
                                srFile.Close();
                            }
                        }

                        if (bValidDataFlag == false)
                        {
                            m_sErrorMessage = string.Format("Frequency Set Error[Step:{0}]", cFlowStep_List[nStepIndex].m_sStepName);
                            return false;
                        }

                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nStepIndex"></param>
        /// <returns></returns>
        private bool ConnectDevice(int nStepIndex)
        {
            if (m_cfrmParent.m_bLoadData == false)
            {
                if (ConnectToTP() == false)
                {
                    SetOutputResult(nStepIndex, false);
                    return false;
                }

                //Check Gen8/Gen7 Solution Type
                if (GetFW_Version() == false)
                {
                    SetOutputResult(nStepIndex, false);
                    return false;
                }

                /*
                if (m_eICGenerationType == ICGenerationType.Gen8)
                    SetSPICommandLength();
                */

                if (WriteConnectInfoFile() == false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nStepIndex"></param>
        /// <param name="sFilePath"></param>
        private void SetStepState(frmMain.FlowStep cFlowStep, int nStepIndex, string sFilePath)
        {
            frmMain.m_nCurrentStepIndex = nStepIndex;
            m_sStepName = cFlowStep.m_sStepName;

            if (ParamFingerAutoTuning.m_nRunMultiTest == 1)
            {
                if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase1 ||
                    cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2 ||
                    cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                    m_sMutualFrequencyPath = sFilePath;

                if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                    m_sACFrequencyPath = sFilePath;

                if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                    m_sRawADCSweepPath = sFilePath;

                if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                    cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                    m_sSelfFrequencyPath = sFilePath;
            }

            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep ||
                cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                m_bGenerateH5Data = false;
                MainConstantParameter.SetSelfTraceType();
            }

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.SetlblStepBackColor(cFlowStep.m_eStep);
                OutputMessage(string.Format("[Execute]Step : {0}", m_cFlowStep_List[nStepIndex].m_sStepName));
                m_cfrmParent.ResizelblCurrentStepFont(cFlowStep.m_sStepName);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eStep"></param>
        /// <returns></returns>
        private bool DeleteStepFile(MainStep eStep)
        {
            OutputMessage("[State]Delete Step File");

            if (eStep == MainStep.FrequencyRank_Phase1)
            {
                while (File.Exists(m_sSkipFrequencyPath) == true)
                {
                    File.Delete(m_sSkipFrequencyPath);
                    Thread.Sleep(10);
                }
            }
            else if (eStep == MainStep.FrequencyRank_Phase2)
            {
                while (File.Exists(m_sACFrequencyPath) == true)
                {
                    File.Delete(m_sACFrequencyPath);
                    Thread.Sleep(10);
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nStepIndex"></param>
        /// <returns></returns>
        private bool LoadLogData(frmMain.FlowStep cFlowStep, int nStepIndex)
        {
            if (m_cfrmParent.m_bLoadData == true)
            {
                bool bCheckStepFolder = false;

                if (nStepIndex > m_nStartIndex)
                    bCheckStepFolder = CheckStepFolderExist(cFlowStep);

                if (bCheckStepFolder == false)
                    ShowfrmFolderSelect(cFlowStep.m_eStep);

                if (CheckDefaultFolder(cFlowStep) == false)
                {
                    SetOutputResult(nStepIndex, false);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nStepIndex"></param>
        /// <returns></returns>
        private bool GetFreqencyListInfo(frmMain.FlowStep cFlowStep, int nStepIndex)
        {
            m_sProjectName = "";
            m_bGetNameByFrequencyList = false;

            if (m_cfrmParent.m_bLoadData == false)
            {
                if (GetFreqeuncyItemListInfo(cFlowStep) == false)
                {
                    SetOutputResult(nStepIndex, false);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nStepIndex"></param>
        /// <param name="nStartStepIndex"></param>
        /// <returns></returns>
        private bool SetRecordDirectory(frmMain.FlowStep cFlowStep, int nStepIndex, int nStartStepIndex)
        {
            bool bSuccessFlag = true;

            if (nStepIndex == nStartStepIndex)
            {
                if (m_cfrmParent.m_bLoadData == true)
                {
                    m_sStartTime = DateTime.Now.ToString("MMdd-HHmmss");
                    CreateRecordDirectory(m_sStartTime, m_bGenerateH5Data);
                    m_cfrmParent.m_bReset = false;
                    m_bCreateNewFolder = true;
                }
                else if (ParamFingerAutoTuning.m_nEnableContinueFlow == 1)
                {
                    bSuccessFlag = SetRecordDirectory(cFlowStep, m_bGenerateH5Data);
                    m_bCreateNewFolder = false;
                }
                else if (m_cfrmParent.m_bReset == true)
                {
                    m_sStartTime = DateTime.Now.ToString("MMdd-HHmmss");
                    CreateRecordDirectory(m_sStartTime, m_bGenerateH5Data);
                    m_cfrmParent.m_bReset = false;
                    m_bCreateNewFolder = true;
                }
                else
                    m_bCreateNewFolder = false;
            }

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.OutputtoolstripstatuslabelStatus(string.Format("Directory : {0}", m_cfrmParent.m_sRecordLogDirectoryName));
            });

            return bSuccessFlag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nStepIndex"></param>
        /// <returns></returns>
        private bool SetRecordData(frmMain.FlowStep cFlowStep, int nStepIndex)
        {
            if (m_cfrmParent.m_bLoadData == true)
            {
                if (CheckDefaultFolder(cFlowStep) == false)
                {
                    SetOutputResult(nStepIndex, false);
                    return false;
                }

                SetLogDirectoryPath(cFlowStep);

                if (CopyLogData(cFlowStep) == false)
                {
                    m_bCopyDataError = true;
                    SetOutputResult(nStepIndex, false);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nStepIndex"></param>
        /// <returns></returns>
        private bool MoveLogDataAndWriteResultList(frmMain.FlowStep cFlowStep, int nStepIndex)
        {
            m_bMoveDataError = false;

            if (m_bCreateNewFolder == true)
            {
                if (MovePreviousData(cFlowStep) == false)
                {
                    m_bMoveDataError = true;
                    m_cfrmParent.m_bReset = true;
                    SetOutputResult(nStepIndex, false);
                    return false;
                }

                MoveUnnecessaryDirectory();
            }

            WriteResultListTxt(cFlowStep.m_eStep);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nStepIndex"></param>
        /// <returns></returns>
        private bool LoadFreqencyList(frmMain.FlowStep cFlowStep, int nStepIndex)
        {
            if (m_cfrmParent.m_bLoadData == false)
            {
                if (LoadFreqeuncyItemList(cFlowStep) == false)
                {
                    SetOutputResult(nStepIndex, false);
                    return false;
                }

                if (GetAndSavePreviousData(cFlowStep) == false)
                {
                    SetOutputResult(nStepIndex, false);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <returns></returns>
        private bool ExecuteRecordData(frmMain.FlowStep cFlowStep)
        {
            bool bFlowComplete = true;

            if (m_cfrmParent.m_bLoadData == false)
                bFlowComplete = RecordData(cFlowStep);

            return bFlowComplete;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nStepIndex"></param>
        /// <returns></returns>
        private bool CheckStepSupported(frmMain.FlowStep cFlowStep, int nStepIndex)
        {
            if (m_cfrmParent.m_bLoadData == false)
            {
                if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                {

                    if (!(m_eICGenerationType == ICGenerationType.Gen8 ||
                          (m_eICGenerationType == ICGenerationType.Gen7 && (m_eICSolutionType == ICSolutionType.Solution_7318 || m_eICSolutionType == ICSolutionType.Solution_7315) ||
                          (m_eICGenerationType == ICGenerationType.Gen6 && (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)))))
                    {
                        string sStep = StringConvert.m_dictMainStepMappingTable[cFlowStep.m_eStep];
                        m_sErrorMessage = string.Format("No Support this IC Solution[Step:{0}]", sStep);
                        SetOutputResult(nStepIndex, false);
                        return false;
                    }
                }
                else if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                {
                    if (m_eICGenerationType != ICGenerationType.Gen8)
                    {
                        string sStep = StringConvert.m_dictMainStepMappingTable[cFlowStep.m_eStep];
                        m_sErrorMessage = string.Format("No Support this IC Solution[Step:{0}]", sStep);
                        SetOutputResult(nStepIndex, false);
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listcFlowStep"></param>
        /// <param name="nStepIndex"></param>
        /// <returns></returns>
        private bool CheckUserDefinedFile(List<frmMain.FlowStep> listcFlowStep, int nStepIndex)
        {
            if (m_cfrmParent.m_bLoadData == true)
                return true;

            if (nStepIndex == frmMain.m_nCurrentStepIndex)
            {
                bool bErrorFlag = false;

                if (m_eICGenerationType == ICGenerationType.Gen8 && (ParamFingerAutoTuning.m_nCommandScriptType == 1 || ParamFingerAutoTuning.m_nCommandScriptType == 2))
                {
                    if (m_bGetCommandScriptFile == false)
                    {
                        string sFilePath = string.Format(@"{0}\{1}", m_cfrmParent.m_sCmdDirectoryPath, ParamFingerAutoTuning.m_sUserDefinedFilePath);

                        if (File.Exists(sFilePath) == true)
                            m_sCommandScriptFilePath = sFilePath;
                        else
                            m_sCommandScriptFilePath = ParamFingerAutoTuning.m_sUserDefinedFilePath;
                    }

                    if (File.Exists(m_sCommandScriptFilePath) == false)
                    {
                        m_sErrorMessage = "User Defined Command Script File Not Exist";
                        bErrorFlag = true;
                    }
                    else
                    {
                        ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);
                        bool bResultFlag = cElanCommand_Gen8.CheckUserDefinedFile(listcFlowStep, m_sCommandScriptFilePath);

                        if (bResultFlag == false)
                        {
                            m_sErrorMessage = cElanCommand_Gen8.GetErrorMessage();
                            bErrorFlag = true;
                        }
                    }
                }

                if (bErrorFlag == true)
                {
                    SetOutputResult(nStepIndex, false);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RunFinishAndRecovery()
        {
            OutputMessage("[State]Finish and Recovery");

            if (m_bTPConnected == true)
            {
                if (ParamFingerAutoTuning.m_nKeepDoNotReset == 1 && m_bKeepNotReset == true)
                    MessageBox.Show("Finish & Keep Don Not Reset State!");

                if (m_bSetPenFunctionOFF_8F18 == true)
                    SetPenFunctionEnable_8F18(true, true);

                if (m_bEnterTestMode == true)
                    SetTestModeEnable(false);

#if _USE_9F07_SOCKET
                if (m_bDisableScanMode_9F07 == true)
                    SetScanModeDisable(false);
#endif

                if (m_bDisableReport == true)
                    SetReportEnable(true);

                if (ParamFingerAutoTuning.m_nDisableReset != 1)
                    SetReset(false);

                //ElanTouch.Disconnect();
                ElanTouchSwitch.Disconnect(m_bSocketConnectType);
                m_bTPConnected = false;
                m_bEnterTestMode = false;
            }

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL)
            {
                if (m_bSocketConnected == true)
                    Thread.Sleep(100);

                CloseAndroidRemoteServer();
            }

            if (m_cfrmParent.m_bLoadData == false &&
                (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL) &&
                ParamFingerAutoTuning.m_nKeepWakeUpIntervalTime > 0)
            {
                if (m_tKeepWakeUp != null && m_tKeepWakeUp.IsAlive == true)
                {
                    m_tKeepWakeUp.Abort();
                    m_tKeepWakeUp = null;
                    m_tsKeepWakeUp = null;
                }
            }

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                DisconnectElanSSHClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMessage"></param>
        private void ShowWarningMessage(string sMessage)
        {
            SetReportEnable(false, false);

            SetTestModeEnable(true);

            ShowfrmWarningMessage(sMessage);

            SetTestModeEnable(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMessage"></param>
        /// <param name="bWarning"></param>
        private void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.OutputMessage(sMessage, bWarning);
            });

            OutputDebugLog(sMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sResultMessage"></param>
        /// <param name="sErrorMessage"></param>
        /// <param name="colorForeColor"></param>
        /// <param name="bOnlyChangelblStatus"></param>
        /// <param name="bResetFlag"></param>
        private void OutputlblStatus(string sResultMessage, string sErrorMessage, Color colorForeColor, bool bOnlyChangelblStatus = false, bool bResetFlag = false)
        {
            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.OutputlblStatus(sResultMessage, sErrorMessage, colorForeColor, bOnlyChangelblStatus, bResetFlag);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMessage"></param>
        public void OutputDebugLog(string sMessage)
        {
            m_cfrmParent.OutputDebugLog(sMessage);
        }
    }
}
