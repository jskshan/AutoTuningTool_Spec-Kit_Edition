using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using Elan;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class ProcessFlow
    {
        private frmMain m_cfrmMain;

        public bool m_bProcessFinishFlag = true;

        public string m_sErrorMessage = "";

        private int m_nNoiseWaitTime        = 240000;   // ms
        private int m_nStartWaitTime        = 3000;     // ms
        private int m_nDrawLineWaitTime     = 600000;   // ms
        private int m_nPPRecordTime         = 10000;    // ms
        private int m_nPTRecordTime         = 10000;    // ms
        private int m_nRobotMovingTimeout   = 30000;    // ms

        //Flow State
        private const int m_nFLOWSTATE_NORMAL   = 0;
        private const int m_nFLOWSTATE_START    = 1;

        //Pressure Robot State
        private const int m_nPRESSUREROBOTSTATE_NORMAL          = 0;
        private const int m_nPRESSUREROBOTSTATE_GET100GZCOORD   = 1;
        private const int m_nPRESSUREROBOTSTATE_SET100GZCOORD   = 2;

        //Record State
        private const int m_nRECORD_INITIALSETTING          = 0;
        private const int m_nRECORD_FRONTSETTING            = 1;
        private const int m_nRECORD_SETTRACENUMBER          = 2;
        private const int m_nRECORD_PARAMSETTING            = 3;
        private const int m_nRECORD_PARAMWRITEREAD          = 4;
        private const int m_nRECORD_1STMIDDLESETTING        = 5;
        private const int m_nRECORD_SENDGETDATACMD          = 6;
        private const int m_nRECORD_2NDMIDDLESETTING        = 7;
        private const int m_nRECORD_ENABLEREPORT            = 8;
        private const int m_nRECORD_DISABLEREPORT           = 9;
        private const int m_nRECORD_DISABLEFINGERREPORT     = 10;
        private const int m_nRECORD_FINISHSETTING           = 11;
        private const int m_nRECORD_SENDGETDATACMD_GEN8     = 12;
        private const int m_nRECORD_READORIGINFWVALUE_GEN8  = 13;
        private const int m_nRECORD_SETMPPMODE_GEN8         = 14;

        //DigiGain Command State
        private const int m_nDIGIGAINCOMMAND_RESET      = 0;
        private const int m_nDIGIGAINCOMMAND_ENABLE     = 1;
        private const int m_nDIGIGAINCOMMAND_DISABLE    = 2;

        private const int m_nRECORDVALUE_STRING             = 0;
        private const int m_nRECORDVALUE_INT                = 1;
        private const int m_nRECORDVALUE_DOUBLE             = 2;
        private const int m_nRECORDVALUE_ERRORINFORMATION   = 3;

        private string m_sLogFilePath = "";
        private string m_sLogFileName = "";
        private string m_sStepDirectoryPath = "";

        private List<byte> m_byteSingleReport_List = new List<byte>();
        private List<List<byte>> m_byteReportData_List = new List<List<byte>>();
        private double m_dTimeCounter = 0.0;

        private ParameterSet m_cCurrentParameterSet = new ParameterSet();

        private RobotAPI m_cRobot = null;
        private InputDevice m_cInputDevice = null;
        private DataAnalysis m_cAnalysis = null;
        private SocketAPI m_cSocket = null;
        private LogAPI m_cDebugLog = null;
        private GoDrawAPI m_cGoDrawRobot = null;

        private ParameterizedThreadStart m_ptsRobotParameter = null;
        //private ParameterizedThreadStart m_ptsRecordFlowInfo = null;
        //private ParameterizedThreadStart m_ptsAnalysisParameter = null;
        private ThreadStart m_tsSocket = null;

        private Thread m_tRobot = null;
        //private Thread m_tRecord = null;
        private Thread m_tSocket = null;
        //private Thread m_tAnalysis = null;
        private Thread m_tStop = null;

        private class LTRobotParameter
        {
            public float m_fStartCoordinateX;
            public float m_fStartCoordinateY;
            public float m_fEndCoordinateX;
            public float m_fEndCoordinateY;
            public float m_fContactCoordinateZ;
            public float m_fHoverRaiseCoordinateZ_DT1st;
            public float m_fHoverRaiseCoordinateZ_DT2nd;
            public float m_fHoverRaiseCoordinateZ_PP;
            public float m_fHoverRaiseCoordinateZ_PT;
            public float m_fHoverRaiseCoordinateZ_PCT1st;
            public float m_fHoverRaiseCoordinateZ_PCT2nd;
            public float m_fPushDownCoordinateZ_DGT;
            public float m_fInitialCoordinateZ_PT;
            public float m_fHorShiftCoordinateY_LT;
            public float m_fVerShiftCoordinateX_LT;

            public float m_fHorStartCoordinateX_TPGT;
            public float m_fHorStartCoordinateY_TPGT;
            public float m_fHorEndCoordinateX_TPGT;
            public float m_fHorEndCoordinateY_TPGT;
            public float m_fVerStartCoordinateX_TPGT;
            public float m_fVerStartCoordinateY_TPGT;
            public float m_fVerEndCoordinateX_TPGT;
            public float m_fVerEndCoordinateY_TPGT;
            public float m_fContactCoordinateZ_TPGT;
        }

        private LTRobotParameter m_cLTRobotParameter = null;

        private class GoDrawParameter
        {
            public int m_nStartCoordinateX;
            public int m_nStartCoordinateY;
            public int m_nEndCoordinateX;
            public int m_nEndCoordinateY;
            public int m_nTopServoValueZ;
            public int m_nContactServoValueZ;
            public int m_nHoverServoValueZ_DT1st;
            public int m_nHoverServoValueZ_DT2nd;
            public int m_nHoverServoValueZ_PCT1st;
            public int m_nHoverServoValueZ_PCT2nd;
            public int m_nPushDownServoValueZ_DGT;
            public int m_nHorizontalShiftCoordinateY_LT;
            public int m_nVerticalShiftCoordinateX_LT;

            public int m_nDelayTime;

            public int m_nHorizontalStartCoordinateX_TPGT;
            public int m_nHorizontalStartCoordinateY_TPGT;
            public int m_nHorizontalEndCoordinateX_TPGT;
            public int m_nHorizontalEndCoordinateY_TPGT;
            public int m_nVerticalStartCoordinateX_TPGT;
            public int m_nVerticalStartCoordinateY_TPGT;
            public int m_nVerticalEndCoordinateX_TPGT;
            public int m_nVerticalEndCoordinateY_TPGT;
            public int m_nContactServoValueZ_TPGT;

            public bool m_bGoDrawReturnHome;
            public bool m_bGoDrawResetZAxis;
            public double m_dNormalSpeed;
        }

        private GoDrawParameter m_cGoDrawParameter = null;

        private class SpeedParameter
        {
            public float m_fSpeed_DGT;
            public float m_fSpeed_TPGT;
            public float m_fSpeed_PCT;
            public float m_fSpeed_DT;
            public float m_fSpeed_TT;
            public float m_fSpeed_TTSlant;
            public float m_fSpeed_LT;
        }

        private SpeedParameter m_cSpeedParameter = null;

        private int m_nNoiseReportNumber = 10000;
        private int m_nNoiseValidReportNumber = 9000;
        private int m_nNormalValidReportNumber = 150;
        private int m_nNormalFilterRXValidReportNumber = 150;
        private int m_nNormalFilterTXValidReportNumber = 70;
        private int m_nTRxSRXValidReportNumber = 1;
        private int m_nTRxSTXValidReportNumber = 1;
        private int m_nTTRXValidReportNumber = 150;
        private int m_nTTTXValidReportNumber = 70;
        private int m_nPPValidReportNumber = 100;
        private int m_nPTValidReportNumber = 1;
        private int m_nPTStartSkipReportNumber = 150;
        private int m_nPTEndSkipReportNumber = 150;
        private bool m_bForceStopFlag = false;
        private bool m_bFormCloseFlag = false;
        private int m_nRecordPrepareFlag = MainConstantParameter.m_nFLOWSTATE_INITIALIZE;
        private bool m_bRecordStartFlag = false;
        private int m_nRobotPrepareFlag = MainConstantParameter.m_nFLOWSTATE_INITIALIZE;
        private bool m_bRobotFinishedFlag = false;
        private bool m_bResetFinishFlag = false;
        private bool m_bClientCloseFlag = false;
        private bool m_bGoDrawConnectFlag = false;
        private bool m_bClientDisconnectFlag = false;
        private bool m_bDrawFinishFlag = false;
        private bool m_bDrawStartFlag = false;
        private int m_nRecordDataErrorFlag = -1;
        private bool m_bDisableRobotMovingFlag = false;
        private bool m_bDisableSetCommandFlag = false;
        private bool m_bDetectFGConnectFlag = false;
        private bool m_bDetectFGPowerOnFlag = false;

        private bool m_bGoDrawReturnFlag = false;

        private bool m_bGetOver100gCoordFlag = false;
        private float m_fPTOver100gZCoord = 0.0f;
        
        private Stopwatch m_swSingleStep = new Stopwatch(); //引用Stopwatch物件;
        private TimeSpan m_timespanPreviousStepDiffer;
        private bool m_bFirstStepCostTimeFlag = false;

        private bool m_bRetryStateFlag = false;
        private bool m_bSetDefaultFWParameterFlag = true;

        private int m_nRXTraceNumber, m_nTXTraceNumber;
        private int m_nRXStartTrace, m_nTXStartTrace;

        private int m_nDigiGainCommandState = m_nDIGIGAINCOMMAND_RESET;
        private bool m_bGetStepInfoFlag_DigiGainTuning = false;

        public bool GetStepInfoFlag_DigiGainTuning
        {
            set
            {
                m_bGetStepInfoFlag_DigiGainTuning = value;
            }
        }

        private string[] m_sDigiGainParameter_Array = new string[] 
        { 
            StringConvert.m_sCMDPARAM_DIGIGAIN_P0,
            StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_RX,
            StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_TX,
            StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_RX,
            StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_TX,
            StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_RX,
            StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_TX 
        };

        private List<string> m_sSetFWParameter_List = new List<string>();
        private List<string> m_sDefaultFWParameter_List = new List<string>();

        private int m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_NONE;
        private bool m_bSupport_Get_MPP_MT_Mode_Enable = false;

        private ElanCommand_Gen8.SendCommandInfo m_cSendCommandInfo = null;

        private ElanTouch.TraceInfo m_structTraceInfo = new ElanTouch.TraceInfo(ElanTouch.MAX_CHIP_NUM);
        private GetFrameData m_cGetFrameData;

        private int m_nReportCount = 0;
        private bool m_bCheckReportExistFlag = false;

        private UInt32 m_nFWVersion = 0;

        private ElanCommand_Gen8 m_cElanCommand_Gen8;

        private int m_nDataCount = 0;
        private int m_nDataIndex = 0;

        private object m_bResetLockFlag = new object();

        private object m_bLTRobotReturnToOriginLockFlag = new object();
        private object m_bClientCloseLockFlag = new object();
        private object m_bGoDrawCloseLockFlag = new object();

        private bool m_bEnterSetMPPParameterFlag = false;

        public enum FinishState
        {
            Initial,
            RunStop,
            RunResult
        }

        public class FinishFlowParameter
        {
            public bool m_bErrorFlag = false;
            public bool m_bStateMessageFlag = false;
            public bool m_bShowMessageBoxFlag = false;
            public int m_nFlowStepIndex = -1;
            public bool m_bSetLastStepFlag = true;
            public bool m_bSetTimeStopFlag = false;

            public FinishState m_eFinishState = FinishState.Initial;

            public bool m_bOutputMessageFlag = true;
            public bool m_bCloseAPFlag = false;

            public FinishFlowParameter()
            {
            }

            public void InitializeParameter()
            {
                m_bErrorFlag = false;
                m_bStateMessageFlag = false;
                m_bShowMessageBoxFlag = false;
                m_nFlowStepIndex = -1;
                m_bSetLastStepFlag = true;
                m_bSetTimeStopFlag = false;

                m_eFinishState = FinishState.Initial;

                m_bOutputMessageFlag = true;
                m_bCloseAPFlag = false;
            }
        }

        public FinishFlowParameter m_cFinishFlowParameter = new FinishFlowParameter();

        public class StopFlowInfo
        {
            public bool m_bOutputMessageFlag = true;
            public bool m_bCloseAPFlag = false;
        }

        public ProcessFlow(frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;

            m_cRobot = m_cfrmMain.m_cRobot;
            m_cGoDrawRobot = m_cfrmMain.m_cGoDrawRobot;
            m_cInputDevice = m_cfrmMain.m_cInputDevice;
            m_cAnalysis = m_cfrmMain.m_cAnalysis;
            m_cSocket = m_cfrmMain.m_cSocket;
            m_cDebugLog = m_cfrmMain.m_cDebugLog;
        }

        #region TP Device Connect Function
        /// <summary>
        /// 取得Device Index
        /// </summary>
        /// <param name="bOutputMessage">是否輸出訊息</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        public int GetDeviceIndex(bool bOutputMessageFlag = false)
        {
            //When the same PID
            List<string> sElanDevice_List = new List<string>();
            const int nSIZE_1K = 1024;
            int nSelectedDeviceIndex = -1;
            const string sDEFAULT_EMPTY_DEVPATH = "NoTouch Device";
            byte[] bytePathName_Array = new byte[nSIZE_1K];
            IntPtr ipPair = Marshal.AllocHGlobal(nSIZE_1K);

            //List All HID Device Path
            sElanDevice_List.Clear();
            int nDeviceCount = ElanTouch.GetDeviceCount();

            if (bOutputMessageFlag == true)
                OutputMessage(string.Format("-DeviceCount : {0}", nDeviceCount));

            for (int nDeviceIndex = 0; nDeviceIndex < nDeviceCount; nDeviceIndex++)
            {
                if (ElanTouch.TP_SUCCESS == ElanTouch.GetHIDDevPath(ipPair, bytePathName_Array.Length, nDeviceIndex))
                {
                    Marshal.Copy(ipPair, bytePathName_Array, 0, nSIZE_1K);
                    string sPathName = ElanConvert.ConvertByteArrayToCharString(bytePathName_Array);
                    sElanDevice_List.Add(sPathName);
                }
                else
                    sElanDevice_List.Add(sDEFAULT_EMPTY_DEVPATH);
            }

            Marshal.FreeHGlobal(ipPair);

            //Check Path List and select first correct Path
            //Fix bug that empty path at first device
            string szEmptyPath = sDEFAULT_EMPTY_DEVPATH;

            for (int nDeviceIndex = 0; nDeviceIndex < sElanDevice_List.Count; nDeviceIndex++)
            {
                //Zero: This instance has the same position in the sort order as value.
                if (szEmptyPath.CompareTo(sElanDevice_List[nDeviceIndex]) != 0)
                {
                    nSelectedDeviceIndex = nDeviceIndex;

                    if (bOutputMessageFlag == true)
                        OutputMessage(string.Format("-Get Device Index Success!![{0}]", nSelectedDeviceIndex));
                }
            }

            return nSelectedDeviceIndex;
        }
        #endregion

        #region Get HID Report Function
        /// <summary>
        /// 讀取Device Report Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HIDRawInputHandler(object sender, InputDevice.HIDDeviceEventArgs e)
        {
            if (CheckRecordState() == true || GetCheckReportExistFlag() == true)
            {
                if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                {
                    if (e.m_Buffer[0] == 0x17 && e.m_Buffer.Length == ParamAutoTuning.m_nGen8ReportDataLength + ParamAutoTuning.m_nShiftByteNumber)
                    {
                        m_cfrmMain.m_qbyteFIFO.EnqueueAll(e.m_Buffer, ParamAutoTuning.m_nGen8ReportDataLength + ParamAutoTuning.m_nShiftByteNumber, 0);
                        m_nReportCount++;
                    }
                }
                else
                {
                    if (e.m_Buffer[0] == 0x07 && e.m_Buffer.Length == ParamAutoTuning.m_nReportDataLength)
                        m_cfrmMain.m_qbyteFIFO.EnqueueAll(e.m_Buffer, ParamAutoTuning.m_nReportDataLength, 0);
                }
            }
        }
        #endregion

        #region Get or Check Flag Function
        /// <summary>
        /// 確認是否在錄製Report Data狀態
        /// </summary>
        /// <returns>true:正在錄製Report Data  false:不在錄製Report Data</returns>
        public bool CheckRecordState()
        {
            return !m_bRobotFinishedFlag && m_bRecordStartFlag;
        }

        /// <summary>
        /// 確認是否還有在報Report Data
        /// </summary>
        /// <returns>true:量測Report Data  false:不量測Report Data</returns>
        public bool GetCheckReportExistFlag()
        {
            return m_bCheckReportExistFlag;
        }

        /// <summary>
        /// 回傳錄製Report Data確認錯誤的Flag值
        /// </summary>
        /// <returns>錄製Report Data確認錯誤的Flag值</returns>
        public int GetRecordDataErrorFlag()
        {
            return m_nRecordDataErrorFlag;
        }

        /// <summary>
        /// 取得項目數量
        /// </summary>
        /// <returns>回傳項目數量</returns>
        public int GetDataCount()
        {
            return m_nDataCount;
        }

        /*
        /// <summary>
        /// 取得強制終止的Flag
        /// </summary>
        /// <returns>回傳強制終止的Flag</returns>
        public bool GetForceStopFlag()
        {
            return m_bForceStopFlag;
        }
        */
        #endregion

        #region Initial or Reset or Set Flag Function
        /// <summary>
        /// 流程相關Flag進行初始
        /// </summary>
        private void InitialFlowFlag()
        {
            m_bRecordStartFlag = false;
            m_nRecordPrepareFlag = MainConstantParameter.m_nFLOWSTATE_INITIALIZE;
            m_nRobotPrepareFlag = MainConstantParameter.m_nFLOWSTATE_INITIALIZE;
            m_bRobotFinishedFlag = false;
            m_bResetFinishFlag = false;
            m_bClientDisconnectFlag = false;
            m_bDrawFinishFlag = false;
            m_bDrawStartFlag = false;
            m_nRecordDataErrorFlag = -1;
            m_sErrorMessage = "";
            m_bEnterSetMPPParameterFlag = false;
        }

        /// <summary>
        /// 流程相關Flag進行重置
        /// </summary>
        private void ResetFlowFlag()
        {
            m_bRecordStartFlag = false;
            m_nRecordPrepareFlag = MainConstantParameter.m_nFLOWSTATE_INITIALIZE;
            m_nRobotPrepareFlag = MainConstantParameter.m_nFLOWSTATE_INITIALIZE;
            m_bRobotFinishedFlag = false;
            m_bResetFinishFlag = false;
            m_bClientDisconnectFlag = false;
            m_bDrawFinishFlag = false;
            m_bDrawStartFlag = false;
            m_nRecordDataErrorFlag = -1;
        }

        /// <summary>
        /// 設定DrawStartFlag為true
        /// </summary>
        public void SetDrawStartFlag()
        {
            m_bDrawStartFlag = true;
        }

        /// <summary>
        /// 設定DrawFinishFlag為true
        /// </summary>
        public void SetDrawFinishFlag()
        {
            m_bDrawFinishFlag = true;
        }

        /// <summary>
        /// 重置強制終止的Flag
        /// </summary>
        public void ResetForceStopFlag()
        {
            m_bForceStopFlag = false;
        }

        /// <summary>
        /// 設定FormCloseFlag為true
        /// </summary>
        public void SetFormCloseFlag()
        {
            m_bFormCloseFlag = true;
        }

        private void InitialRobotThreadParameter()
        {
            m_sErrorMessage = "";
        }
        #endregion

        #region Set LT Robot and GoDraw Robot Parameter Function
        /// <summary>
        /// 設定流程參數及線測機/GoDraw寫字機所使用到座標點資訊
        /// </summary>
        public void SetProcessAndRobotParameter(List<FlowStep> cFlowStep_List)
        {
            m_cLTRobotParameter = null;
            m_cGoDrawParameter = null;
            m_cSpeedParameter = null;

            if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_SINGLE)
            {
                if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_GODRAW)
                {
                    m_cLTRobotParameter = new LTRobotParameter();

                    m_cLTRobotParameter.m_fStartCoordinateX = (float)ParamAutoTuning.m_dStartXAxisCoordinate;
                    m_cLTRobotParameter.m_fStartCoordinateY = (float)ParamAutoTuning.m_dStartYAxisCoordinate;
                    m_cLTRobotParameter.m_fEndCoordinateX = (float)ParamAutoTuning.m_dEndXAxisCoordinate;
                    m_cLTRobotParameter.m_fEndCoordinateY = (float)ParamAutoTuning.m_dEndYAxisCoordinate;
                    m_cLTRobotParameter.m_fInitialCoordinateZ_PT = 0.0f;
                    m_cLTRobotParameter.m_fVerShiftCoordinateX_LT = (float)ParamAutoTuning.m_dLTHorShiftYAxisCoordinate;
                    m_cLTRobotParameter.m_fHorShiftCoordinateY_LT = (float)ParamAutoTuning.m_dLTVerShiftXAxisCoordinate;
                    m_cLTRobotParameter.m_fContactCoordinateZ = (float)ParamAutoTuning.m_dContactZAxisCoordinate;

                    m_cLTRobotParameter.m_fHorStartCoordinateX_TPGT = (float)ParamAutoTuning.m_dTPGTHorizontalStartXAxisCoordinate;
                    m_cLTRobotParameter.m_fHorStartCoordinateY_TPGT = (float)ParamAutoTuning.m_dTPGTHorizontalStartYAxisCoordinate;
                    m_cLTRobotParameter.m_fHorEndCoordinateX_TPGT = (float)ParamAutoTuning.m_dTPGTHorizontalEndXAxisCoordinate;
                    m_cLTRobotParameter.m_fHorEndCoordinateY_TPGT = (float)ParamAutoTuning.m_dTPGTHorizontalEndYAxisCoordinate;
                    m_cLTRobotParameter.m_fVerStartCoordinateX_TPGT = (float)ParamAutoTuning.m_dTPGTVerticalStartXAxisCoordinate;
                    m_cLTRobotParameter.m_fVerStartCoordinateY_TPGT = (float)ParamAutoTuning.m_dTPGTVerticalStartYAxisCoordinate;
                    m_cLTRobotParameter.m_fVerEndCoordinateX_TPGT = (float)ParamAutoTuning.m_dTPGTVerticalEndXAxisCoordinate;
                    m_cLTRobotParameter.m_fVerEndCoordinateY_TPGT = (float)ParamAutoTuning.m_dTPGTVerticalEndYAxisCoordinate;
                    m_cLTRobotParameter.m_fContactCoordinateZ_TPGT = (float)ParamAutoTuning.m_dTPGTContactZAxisCoordinate;

                    m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_DT1st = (float)ParamAutoTuning.m_dHoverHeight_DT1st;
                    m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_DT2nd = (float)ParamAutoTuning.m_dHoverHeight_DT2nd;
                    m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PP = (float)ParamAutoTuning.m_dHoverHeight_PP;
                    m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PT = (float)ParamAutoTuning.m_dHoverHeight_PT;
                    m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PCT1st = (float)ParamAutoTuning.m_dHoverHeight_PCT1st;
                    m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PCT2nd = (float)ParamAutoTuning.m_dHoverHeight_PCT2nd;
                    m_cLTRobotParameter.m_fPushDownCoordinateZ_DGT = (float)ParamAutoTuning.m_dPushDownZAxisCoordinate_DGT;
                }
                else
                {
                    m_cGoDrawParameter = new GoDrawParameter();

                    m_cGoDrawParameter.m_nStartCoordinateX = ParamAutoTuning.m_nGoDrawStartXAxisCoordinate;
                    m_cGoDrawParameter.m_nStartCoordinateY = ParamAutoTuning.m_nGoDrawStartYAxisCoordinate;
                    m_cGoDrawParameter.m_nEndCoordinateX = ParamAutoTuning.m_nGoDrawEndXAxisCoordinate;
                    m_cGoDrawParameter.m_nEndCoordinateY = ParamAutoTuning.m_nGoDrawEndYAxisCoordinate;
                    m_cGoDrawParameter.m_nHorizontalShiftCoordinateY_LT = ParamAutoTuning.m_nGoDrawLTHorShiftYAxisCoordinate;
                    m_cGoDrawParameter.m_nVerticalShiftCoordinateX_LT = ParamAutoTuning.m_nGoDrawLTVerShiftXAxisCoordinate;
                    m_cGoDrawParameter.m_nTopServoValueZ = ParamAutoTuning.m_nGoDrawTopZServoValue;
                    m_cGoDrawParameter.m_nContactServoValueZ = ParamAutoTuning.m_nGoDrawContactZServoValue;
                    m_cGoDrawParameter.m_nDelayTime = ParamAutoTuning.m_nGoDrawDelayTime;

                    m_cGoDrawParameter.m_nHorizontalStartCoordinateX_TPGT = ParamAutoTuning.m_nGoDrawTPGTHorizontalStartXAxisCoordinate;
                    m_cGoDrawParameter.m_nHorizontalStartCoordinateY_TPGT = ParamAutoTuning.m_nGoDrawTPGTHorizontalStartYAxisCoordinate;
                    m_cGoDrawParameter.m_nHorizontalEndCoordinateX_TPGT = ParamAutoTuning.m_nGoDrawTPGTHorizontalEndXAxisCoordinate;
                    m_cGoDrawParameter.m_nHorizontalEndCoordinateY_TPGT = ParamAutoTuning.m_nGoDrawTPGTHorizontalEndYAxisCoordinate;
                    m_cGoDrawParameter.m_nVerticalStartCoordinateX_TPGT = ParamAutoTuning.m_nGoDrawTPGTVerticalStartXAxisCoordinate;
                    m_cGoDrawParameter.m_nVerticalStartCoordinateY_TPGT = ParamAutoTuning.m_nGoDrawTPGTVerticalStartYAxisCoordinate;
                    m_cGoDrawParameter.m_nVerticalEndCoordinateX_TPGT = ParamAutoTuning.m_nGoDrawTPGTVerticalEndXAxisCoordinate;
                    m_cGoDrawParameter.m_nVerticalEndCoordinateY_TPGT = ParamAutoTuning.m_nGoDrawTPGTVerticalEndYAxisCoordinate;
                    m_cGoDrawParameter.m_nContactServoValueZ_TPGT = ParamAutoTuning.m_nGoDrawTPGTContactZServoValue;

                    m_cGoDrawParameter.m_nHoverServoValueZ_DT1st = ParamAutoTuning.m_nGoDrawHoverZServoValue_DT1st;
                    m_cGoDrawParameter.m_nHoverServoValueZ_DT2nd = ParamAutoTuning.m_nGoDrawHoverZServoValue_DT2nd;
                    m_cGoDrawParameter.m_nHoverServoValueZ_PCT1st = ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT1st;
                    m_cGoDrawParameter.m_nHoverServoValueZ_PCT2nd = ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT2nd;
                    m_cGoDrawParameter.m_nPushDownServoValueZ_DGT = ParamAutoTuning.m_nGoDrawPushDownZServoValue_DGT;

                    m_cGoDrawParameter.m_bGoDrawReturnHome = false;
                    m_cGoDrawParameter.m_bGoDrawResetZAxis = false;
                    m_cGoDrawParameter.m_dNormalSpeed = 50.0;
                }
            }

            m_cfrmMain.m_dHoverHeight_DT1st = ParamAutoTuning.m_dHoverHeight_DT1st;
            m_cfrmMain.m_dHoverHeight_DT2nd = ParamAutoTuning.m_dHoverHeight_DT2nd;
            m_cfrmMain.m_dHoverHeight_PP = ParamAutoTuning.m_dHoverHeight_PP;
            m_cfrmMain.m_dHoverHeight_PCT1st = ParamAutoTuning.m_dHoverHeight_PCT1st;
            m_cfrmMain.m_dHoverHeight_PCT2nd = ParamAutoTuning.m_dHoverHeight_PCT2nd;

            if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_SINGLE)
            {
                m_cSpeedParameter = new SpeedParameter();

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                {
                    m_cSpeedParameter.m_fSpeed_DGT = (float)CheckGoDrawSettingSpeed(ParamAutoTuning.m_dDGTDrawingSpeed);
                    m_cSpeedParameter.m_fSpeed_TPGT = (float)CheckGoDrawSettingSpeed(ParamAutoTuning.m_dTPGTDrawingSpeed);
                    m_cSpeedParameter.m_fSpeed_PCT = (float)CheckGoDrawSettingSpeed(ParamAutoTuning.m_dPCTDrawingSpeed);
                    m_cSpeedParameter.m_fSpeed_DT = (float)CheckGoDrawSettingSpeed(ParamAutoTuning.m_dDTDrawingSpeed);
                    m_cSpeedParameter.m_fSpeed_TT = (float)CheckGoDrawSettingSpeed(ParamAutoTuning.m_dTTDrawingSpeed);
                    m_cSpeedParameter.m_fSpeed_TTSlant = (float)CheckGoDrawSettingSpeed(ParamAutoTuning.m_dTTSlantDrawingSpeed);
                    m_cSpeedParameter.m_fSpeed_LT = (float)CheckGoDrawSettingSpeed(ParamAutoTuning.m_dLTDrawingSpeed);
                }
                else
                {
                    m_cSpeedParameter.m_fSpeed_DGT = (float)ParamAutoTuning.m_dDGTDrawingSpeed;
                    m_cSpeedParameter.m_fSpeed_TPGT = (float)ParamAutoTuning.m_dTPGTDrawingSpeed;
                    m_cSpeedParameter.m_fSpeed_PCT = (float)ParamAutoTuning.m_dPCTDrawingSpeed;
                    m_cSpeedParameter.m_fSpeed_DT = (float)ParamAutoTuning.m_dDTDrawingSpeed;
                    m_cSpeedParameter.m_fSpeed_TT = (float)ParamAutoTuning.m_dTTDrawingSpeed;
                    m_cSpeedParameter.m_fSpeed_TTSlant = (float)ParamAutoTuning.m_dTTSlantDrawingSpeed;
                    m_cSpeedParameter.m_fSpeed_LT = (float)ParamAutoTuning.m_dLTDrawingSpeed;
                }
            }

            m_nNoiseWaitTime = (int)(ParamAutoTuning.m_dNoiseTimeout * 1000);

            if (m_cfrmMain.m_sCPUType == MainConstantParameter.m_sCPUTYPE_AMD ||
                m_cfrmMain.m_sCPUType == MainConstantParameter.m_sCPUTYPE_NONE)
                m_nNoiseReportNumber = ParamAutoTuning.m_nNoiseReportNumber;
            else
                m_nNoiseReportNumber = (int)((double)ParamAutoTuning.m_nNoiseReportNumber * 1.1);

            m_nNoiseValidReportNumber = ParamAutoTuning.m_nNoiseValidReportNumber;
            m_nNormalValidReportNumber = ParamAutoTuning.m_nNormalValidReportNumber;
            m_nNormalFilterRXValidReportNumber = ParamAutoTuning.m_nNormalFilterRXValidReportNumber;
            m_nNormalFilterTXValidReportNumber = ParamAutoTuning.m_nNormalFilterTXValidReportNumber;
            m_nTRxSRXValidReportNumber = ParamAutoTuning.m_nTRxSRXValidReportNumber;
            m_nTRxSTXValidReportNumber = ParamAutoTuning.m_nTRxSTXValidReportNumber;
            m_nTTRXValidReportNumber = ParamAutoTuning.m_nTTRXValidReportNumber;
            m_nTTTXValidReportNumber = ParamAutoTuning.m_nTTTXValidReportNumber;
            m_nPPValidReportNumber = ParamAutoTuning.m_nPPValidReportNumber;
            m_nPTValidReportNumber = ParamAutoTuning.m_nPTValidReportNumber;
            m_nPTStartSkipReportNumber = ParamAutoTuning.m_nPTStartSkipReportNumber;
            m_nPTEndSkipReportNumber = ParamAutoTuning.m_nPTEndSkipReportNumber;
            m_nPPRecordTime = (int)(ParamAutoTuning.m_dPPRecordTime * 1000);
            m_nPTRecordTime = (int)(ParamAutoTuning.m_dPTRecordTime * 1000);

            m_nStartWaitTime = (int)(ParamAutoTuning.m_dStartDelayTime * 1000);
            m_nDrawLineWaitTime = (int)(ParamAutoTuning.m_fDrawLineTimeout * 1000);

            if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_GODRAW)
                m_nRobotMovingTimeout = (int)(ParamAutoTuning.m_dRobotMovingTimeout * 1000);

            if (m_nNoiseValidReportNumber > m_nNoiseReportNumber)
                m_nNoiseValidReportNumber = m_nNoiseReportNumber;

            m_cfrmMain.m_byteBuffer_Array = new byte[ParamAutoTuning.m_nReportDataLength];

            bool bCheckCoordinateFlag_Normal = false;
            bool bCheckCoordinateFlag_TPGT = false;

            foreach (FlowStep cFlowStep in cFlowStep_List)
            {
                if (bCheckCoordinateFlag_Normal == false)
                {
                    if (cFlowStep.m_eMainStep == MainTuningStep.DIGIGAINTUNING ||
                        cFlowStep.m_eMainStep == MainTuningStep.PEAKCHECKTUNING ||
                        cFlowStep.m_eMainStep == MainTuningStep.DIGITALTUNING ||
                        cFlowStep.m_eMainStep == MainTuningStep.TILTTUNING ||
                        cFlowStep.m_eMainStep == MainTuningStep.PRESSURETUNING ||
                        cFlowStep.m_eMainStep == MainTuningStep.LINEARITYTUNING)
                        bCheckCoordinateFlag_Normal = true;
                }

                if (bCheckCoordinateFlag_TPGT == false)
                {
                    if (cFlowStep.m_eMainStep == MainTuningStep.TPGAINTUNING)
                        bCheckCoordinateFlag_TPGT = true;
                }

                if (bCheckCoordinateFlag_Normal == true && bCheckCoordinateFlag_TPGT == true)
                    break;
            }

            OutputMessage(string.Format("-CPU Name : {0}", m_cfrmMain.m_sCPUName));
            OutputMessage(string.Format("-CPU Type : {0}", m_cfrmMain.m_sCPUType));

            if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_SINGLE)
            {
                OutputMessage("-Robot Info : ");

                if (bCheckCoordinateFlag_Normal == true)
                {
                    OutputMessage("-Normal Coordinate Info : ");

                    if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_GODRAW)
                    {
                        OutputMessage(string.Format("-Start : X={0}, Y={1}", m_cLTRobotParameter.m_fStartCoordinateX.ToString(), m_cLTRobotParameter.m_fStartCoordinateY.ToString()));
                        OutputMessage(string.Format("-End : X={0}, Y={1}", m_cLTRobotParameter.m_fEndCoordinateY.ToString(), m_cLTRobotParameter.m_fEndCoordinateX.ToString()));
                        OutputMessage(string.Format("-ContactZ={0}", m_cLTRobotParameter.m_fContactCoordinateZ.ToString()));
                        OutputMessage(string.Format("-HoverRaiseZ_DT1st={0}", m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_DT1st.ToString()));
                        OutputMessage(string.Format("-HoverRaiseZ_DT2nd={0}", m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_DT2nd.ToString()));
                        //OutputMessage(string.Format("-HoverRaiseZ_PP={0}", m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PP.ToString()));
                        OutputMessage(string.Format("-HoverRaiseZ_PT={0}", m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PT.ToString()));
                        OutputMessage(string.Format("-HoverRaiseZ_PCT1st={0}", m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PCT1st.ToString()));
                        OutputMessage(string.Format("-HoverRaiseZ_PCT2nd={0}", m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PCT2nd.ToString()));
                        OutputMessage(string.Format("-PushDownZ_DGT={0}", m_cLTRobotParameter.m_fPushDownCoordinateZ_DGT.ToString()));
                    }
                    else
                    {
                        OutputMessage(string.Format("-Start : X={0}, Y={1}", m_cGoDrawParameter.m_nStartCoordinateX.ToString(), m_cGoDrawParameter.m_nStartCoordinateY.ToString()));
                        OutputMessage(string.Format("-End : X={0}, Y={1}", m_cGoDrawParameter.m_nEndCoordinateX.ToString(), m_cGoDrawParameter.m_nEndCoordinateY.ToString()));
                        OutputMessage(string.Format("-ContactZServo={0}", m_cGoDrawParameter.m_nContactServoValueZ.ToString()));
                        OutputMessage(string.Format("-HoverZServo_DT1st={0}", m_cGoDrawParameter.m_nHoverServoValueZ_DT1st.ToString()));
                        OutputMessage(string.Format("-HoverZServo_DT2nd={0}", m_cGoDrawParameter.m_nHoverServoValueZ_DT2nd.ToString()));
                        OutputMessage(string.Format("-HoverZServo_PCT1st={0}", m_cGoDrawParameter.m_nHoverServoValueZ_PCT1st.ToString()));
                        OutputMessage(string.Format("-HoverZServo_PCT2nd={0}", m_cGoDrawParameter.m_nHoverServoValueZ_PCT2nd.ToString()));
                        OutputMessage(string.Format("-PushDownZServo_DGT={0}", m_cGoDrawParameter.m_nPushDownServoValueZ_DGT.ToString()));
                    }
                }

                if (bCheckCoordinateFlag_TPGT == true)
                {
                    OutputMessage("-TP_Gain Coordinate Info : ");

                    if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_GODRAW)
                    {
                        OutputMessage(string.Format("-Horizontal Start : X={0}, Y={1}", m_cLTRobotParameter.m_fHorStartCoordinateX_TPGT.ToString(), m_cLTRobotParameter.m_fHorStartCoordinateY_TPGT.ToString()));
                        OutputMessage(string.Format("-Horizontal End : X={0}, Y={1}", m_cLTRobotParameter.m_fHorEndCoordinateX_TPGT.ToString(), m_cLTRobotParameter.m_fHorEndCoordinateY_TPGT.ToString()));
                        OutputMessage(string.Format("-Vertical Start : X={0}, Y={1}", m_cLTRobotParameter.m_fVerStartCoordinateX_TPGT.ToString(), m_cLTRobotParameter.m_fVerStartCoordinateY_TPGT.ToString()));
                        OutputMessage(string.Format("-Vertical End : X={0}, Y={1}", m_cLTRobotParameter.m_fVerEndCoordinateX_TPGT.ToString(), m_cLTRobotParameter.m_fVerEndCoordinateY_TPGT.ToString()));
                        OutputMessage(string.Format("-ContactZ={0}", m_cLTRobotParameter.m_fContactCoordinateZ_TPGT.ToString()));
                    }
                    else
                    {
                        OutputMessage(string.Format("-Horizontal Start : X={0}, Y={1}", m_cGoDrawParameter.m_nHorizontalStartCoordinateX_TPGT.ToString(), m_cGoDrawParameter.m_nHorizontalStartCoordinateY_TPGT.ToString()));
                        OutputMessage(string.Format("-Horizontal End : X={0}, Y={1}", m_cGoDrawParameter.m_nHorizontalEndCoordinateX_TPGT.ToString(), m_cGoDrawParameter.m_nHorizontalEndCoordinateY_TPGT.ToString()));
                        OutputMessage(string.Format("-Vertical Start : X={0}, Y={1}", m_cGoDrawParameter.m_nVerticalStartCoordinateX_TPGT.ToString(), m_cGoDrawParameter.m_nVerticalStartCoordinateY_TPGT.ToString()));
                        OutputMessage(string.Format("-Vertical End : X={0}, Y={1}", m_cGoDrawParameter.m_nVerticalEndCoordinateX_TPGT.ToString(), m_cGoDrawParameter.m_nVerticalEndCoordinateY_TPGT.ToString()));
                        OutputMessage(string.Format("-ContactZServo={0}", m_cGoDrawParameter.m_nContactServoValueZ_TPGT.ToString()));
                    }
                }

                OutputMessage("-Time Info : ");
                OutputMessage(string.Format("-Start Delay Time={0} ms", m_nStartWaitTime.ToString()));
                OutputMessage(string.Format("-Draw Line Timeout={0} ms", m_nDrawLineWaitTime.ToString()));

                if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_GODRAW)
                    OutputMessage(string.Format("-Robot Moving Timeout={0} ms", m_nRobotMovingTimeout.ToString()));
            }

            m_nRecordDataErrorFlag = -1;

            m_bClientCloseFlag = false;
            m_bGoDrawConnectFlag = false;
            m_bDetectFGConnectFlag = false;
            m_bDetectFGPowerOnFlag = false;

            m_bGetOver100gCoordFlag = false;
            m_fPTOver100gZCoord = 0.0f;

            m_nRXTraceNumber = 0;
            m_nTXTraceNumber = 0;

            m_nDigiGainCommandState = m_nDIGIGAINCOMMAND_RESET;
            //m_bGetStepInfoFlag_DigiGainTuning = false;
            m_bSetDefaultFWParameterFlag = true;

            m_nFWVersion = 0;
        }

        private double CheckGoDrawSettingSpeed(double dSpeed)
        {
            if (dSpeed < ParamAutoTuning.m_dGoDrawCtrlrMinSpeed)
                return ParamAutoTuning.m_dGoDrawCtrlrMinSpeed;
            else if (dSpeed > ParamAutoTuning.m_dGoDrawCtrlrMaxSpeed)
                return ParamAutoTuning.m_dGoDrawCtrlrMaxSpeed;
            else
                return dSpeed;
        }
        #endregion

        #region Set and Get Flow Data Information Function
        private bool GetFlowStepDirectory(int nStepIndex, bool bRunResetAndReturnFlowFlag = true, int nFlowState = m_nFLOWSTATE_NORMAL, bool bSetCheckStepFlag = false)
        {
            OutputMessage("-Get Flow Step Directory");

            string sErrorMessage = "";
            int nResult = FileProcess.m_nGETDIRINFO_ERROR;
            bool bGetStepInfoFlag_DigiGainTuning = false;

            FileProcess cFileProcess = new FileProcess(m_cfrmMain);

            int nStepFileFlag = (bSetCheckStepFlag == true) ? FileProcess.m_nCheckStepFlag : 0xFFFF;
            nResult = cFileProcess.GetFlowStepDirectory(ref bGetStepInfoFlag_DigiGainTuning, ref sErrorMessage, m_cfrmMain.m_cCurrentFlowStep, nStepFileFlag);

            if (bGetStepInfoFlag_DigiGainTuning == true)
                m_bGetStepInfoFlag_DigiGainTuning = true;

            if (nResult == FileProcess.m_nGETDIRINFO_ERROR)
            {
                m_sErrorMessage = sErrorMessage;
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;

                if (nFlowState == m_nFLOWSTATE_NORMAL)
                {
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunResult;
                }
                else
                {
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                }

                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return false;
            }

            return true;
        }

        private bool CheckPreviousStepFileExist(MainTuningStep eMainStep, SubTuningStep eSubStep, int nStepIndex, bool bRunResetAndReturnFlow = true, int nFlowState = m_nFLOWSTATE_NORMAL)
        {
            OutputMessage("-Check Previous Step File Exist");

            CheckState cCheckState = new CheckState(m_cfrmMain);

            if (cCheckState.CheckIndependentStep(eMainStep, eSubStep) != CheckState.m_nSTEPSTATE_NORMAL)
                return true;

            if (eMainStep == MainTuningStep.TILTNO && (eSubStep == SubTuningStep.TILTNO_PTHF || eSubStep == SubTuningStep.TILTNO_BHF))
            {
                if (File.Exists(m_cfrmMain.m_sTotalResultWRFilePath_Noise) == false)
                {
                    m_sErrorMessage = string.Format("Noise \"{0}\" File Not Exist", SpecificText.m_sResultFileName);
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;

                    if (nFlowState == m_nFLOWSTATE_NORMAL)
                    {
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunResult;
                    }
                    else
                    {
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    }

                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
            }
            else if (eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS))
            {
                if (File.Exists(m_cfrmMain.m_sReferenceFilePath_Noise) == false)
                {
                    m_sErrorMessage = string.Format("Noise \"{0}\" File Not Exist", SpecificText.m_sReferenceFileName);
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;

                    if (nFlowState == m_nFLOWSTATE_NORMAL)
                    {
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunResult;
                    }
                    else
                    {
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                    }

                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
            }
            else if (eMainStep == MainTuningStep.LINEARITYTUNING && eSubStep == SubTuningStep.LINEARITYTABLE)
            {
                if (ParamAutoTuning.m_nLTUseTP_GainCompensate != MainConstantParameter.m_nLTCOMPENSATE_DISABLE)
                {
                    if (File.Exists(m_cfrmMain.m_sReferenceFilePath_TPGT) == false)
                    {
                        m_sErrorMessage = string.Format("TP_Gain Tuning \"{0}\" File Not Exist", SpecificText.m_sReferenceFileName);
                        m_cFinishFlowParameter.m_bErrorFlag = true;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;

                        if (nFlowState == m_nFLOWSTATE_NORMAL)
                        {
                            m_cFinishFlowParameter.m_eFinishState = FinishState.RunResult;
                        }
                        else
                        {
                            m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                            m_cFinishFlowParameter.m_bStateMessageFlag = true;
                            m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                        }

                        m_cfrmMain.m_bErrorFlag = true;
                        Thread.Sleep(10);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CheckFlowFlieFormat(FlowStep cFlowStep, int nStepIndex, int nFlowState = m_nFLOWSTATE_NORMAL, string sMainStep = "", string sPreviousSubStep = "")
        {
            OutputMessage("-Check Flow Flie Format");

            int nResultFlag = m_cfrmMain.CheckFlowFileFormat(m_cfrmMain.m_cCurrentFlowStep);

            if (nResultFlag != -1)
            {
                int nSubStepIndex = (int)cFlowStep.m_eSubStep;
                int nSubStepState = cFlowStep.m_nSubStepState;

                if (nFlowState == m_nFLOWSTATE_NORMAL)
                {
                    MainTuningStep eMainStep = m_cfrmMain.m_cCurrentFlowStep.m_eMainStep;
                    SubTuningStep eSubStep = m_cfrmMain.m_cCurrentFlowStep.m_eSubStep;

                    sMainStep = StringConvert.m_dictMainStepMappingTable[eMainStep];
                    sPreviousSubStep = StringConvert.m_dictPreviousSubStepMappingTable[eSubStep];
                }

                if ((nSubStepState & MainConstantParameter.m_nSTEPLOCATION_FIRST) != 0 && nResultFlag == 1)
                    m_sErrorMessage = string.Format("\"{0} Step\" Flow Invalid. Please Switch or Check \"{1}\" Step", sMainStep, sPreviousSubStep);
                else if (nResultFlag == 2)
                    m_sErrorMessage = "Fixed PH1 or Fixed PH2 Setting Error";
                else
                {
                    string sFileName = m_cfrmMain.m_sSubTuningStepFileName_Array[nSubStepIndex];
                    m_sErrorMessage = string.Format("Flow File Format Check Error in {0}", sFileName);
                }

                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;

                if (nFlowState == m_nFLOWSTATE_NORMAL)
                {
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunResult;
                }
                else
                {
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                }

                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return false;
            }

            return true;
        }

        private bool RunSetFlowInfo(FlowStep cFlowStep, int nStepIndex = 0, bool bDataAnalysisFlag = false, bool bSkipOverwriteDataFlag = false)
        {
            OutputMessage("-Run Set Flow Info");

            bool bResultFlag = SetFlowInfo(cFlowStep, bSkipOverwriteDataFlag);

            if (bResultFlag == false)
            {
                if (bDataAnalysisFlag == false)
                {
                    SetStepCostTime(nStepIndex);

                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunResult;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                }
                else
                {
                    m_cFinishFlowParameter.m_bErrorFlag = true;
                    m_cFinishFlowParameter.m_eFinishState = FinishState.RunResult;
                    m_cFinishFlowParameter.m_bStateMessageFlag = true;
                    m_cfrmMain.m_bErrorFlag = true;
                    Thread.Sleep(10);
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// 讀取項目資料的主流程
        /// </summary>
        /// <param name="cFlowStep">此階段相關資料</param>
        /// <param name="bSkipOverWriteDataFlag">是否忽略複寫Data</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        public bool SetFlowInfo(FlowStep cFlowStep, bool bSkipOverWriteDataFlag = false)
        {
            SubTuningStep eSubStep = cFlowStep.m_eSubStep;

            //Clear Result List File Data
            if (bSkipOverWriteDataFlag == false)
            {
                FileProcess cFileProcess = new FileProcess(m_cfrmMain);

                if (cFileProcess.ClearResultListData(eSubStep) == false)
                {
                    m_sErrorMessage = "Overwrite Result List Data Error in ResultList.txt";
                    return false;
                }
            }

            //Get Flow File Data
            if ((m_cfrmMain.m_nSkipPreviousStepFlag & MainConstantParameter.m_nSKIPFILE_FLOWTXT) == 0 ||
                (eSubStep != SubTuningStep.HOVER_1ST &&
                 eSubStep != SubTuningStep.HOVER_2ND &&
                 eSubStep != SubTuningStep.TILTNO_PTHF &&
                 eSubStep != SubTuningStep.TILTTUNING_PTHF &&
                 eSubStep != SubTuningStep.PRESSURESETTING &&
                 eSubStep != SubTuningStep.LINEARITYTABLE &&
                 eSubStep != SubTuningStep.PCHOVER_1ST &&
                 eSubStep != SubTuningStep.PCHOVER_2ND &&
                 eSubStep != SubTuningStep.DIGIGAIN &&
                 eSubStep != SubTuningStep.TP_GAIN))
            {
                if (GetFlowFileInfo(cFlowStep) == false)
                {
                    int nSubStepIndex = (int)eSubStep;
                    m_sErrorMessage = string.Format("Flow Data Format Error in {0}", m_cfrmMain.m_sSubTuningStepFileName_Array[nSubStepIndex]);
                    return false;
                }
            }

            //Get Step List Data
            if (eSubStep != SubTuningStep.NO && eSubStep != SubTuningStep.ELSE)
            {
                SubTuningStep eCurrentLoadSubStep = SubTuningStep.ELSE;
                bool bSkipPreviousStepFlag = (m_cfrmMain.m_nSkipPreviousStepFlag != 0) ? true : false;

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
                {
                    if (eSubStep != SubTuningStep.TILTTUNING_PTHF &&
                        eSubStep != SubTuningStep.TILTTUNING_BHF &&
                        eSubStep != SubTuningStep.PRESSURESETTING &&
                        eSubStep != SubTuningStep.PRESSUREPROTECT &&
                        eSubStep != SubTuningStep.PRESSURETABLE &&
                        eSubStep != SubTuningStep.LINEARITYTABLE &&
                        eSubStep != SubTuningStep.PCHOVER_1ST &&
                        eSubStep != SubTuningStep.PCHOVER_2ND &&
                        eSubStep != SubTuningStep.PCCONTACT &&
                        eSubStep != SubTuningStep.DIGIGAIN &&
                        eSubStep != SubTuningStep.TP_GAIN)
                        bSkipPreviousStepFlag = ((m_cfrmMain.m_nSkipPreviousStepFlag & MainConstantParameter.m_nSKIPFILE_FLOWTXT) != 0) ? true : false;
                }

                if (GetStepListInfo(ref eCurrentLoadSubStep, cFlowStep, bSkipPreviousStepFlag) == false)
                {
                    string sSubStep = StringConvert.m_dictSubStepCNMappingTable[eCurrentLoadSubStep];
                    m_sErrorMessage = string.Format("Step List Data Format Error in {0}_{1}.csv", SpecificText.m_sStepListText, sSubStep);
                    return false;
                }
            }

            string sErrorMessage = "";

            if (CheckHoverThresholdIsValid(ref sErrorMessage, eSubStep) == false)
            {
                m_sErrorMessage = sErrorMessage;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 讀取Flow File中的項目資訊
        /// </summary>
        /// <param name="cFlowStep">現階段相關資訊</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool GetFlowFileInfo(FlowStep cFlowStep)
        {
            RecordSetInfo.Clear();
            RecordFlowInfo.Initialize(cFlowStep.m_eSubStep);
            m_nDataCount = 0;

            bool bInstructionPassFlag = true;

            GetFlowFile cGetFlowFile = new GetFlowFile(cFlowStep, m_cfrmMain, m_nICSolutionType);
            cGetFlowFile.MainFlow();

            bInstructionPassFlag = cGetFlowFile.InstructionPassFlag;
            m_nDataCount = cGetFlowFile.DataCount;

            return bInstructionPassFlag;
        }

        /// <summary>
        /// 讀取StepList檔中相關數值流程
        /// </summary>
        /// <param name="eCurrentLoadSubStep">現在所讀取的StepList檔其所屬階段</param>
        /// <param name="cFlowStep">現階段相關訊息</param>
        /// <param name="bSkipPreviousStepFlag">是否要忽略上一階段的StepList檔資訊</param>
        /// <returns></returns>
        private bool GetStepListInfo(ref SubTuningStep eCurrentLoadSubStep, FlowStep cFlowStep, bool bSkipPreviousStepFlag)
        {
            MainTuningStep eMainStep = cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cFlowStep.m_eSubStep;

            CheckState cCheckState = new CheckState(m_cfrmMain);
            int nStepState = cCheckState.CheckIndependentStep(cFlowStep.m_eMainStep, cFlowStep.m_eSubStep);

            if (nStepState == CheckState.m_nSTEPSTATE_INDEPENDENT)
                return true;

            const int nSTATE_NA = 0x0000;
            const int nSTATE_DATAINFOLIST = 0x0001;
            const int nSTATE_NORMALLIST = 0x0002;
            const int nSTATE_TILTNOLIST = 0x0004;

            DataTable datatableDataInfo = null;
            DataTable datatableFWParameter = null;

            string sDataInfoTitle = "";
            string sParameterInfoTitle = "";

            string[] sColumnName_Array = new string[2] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName 
            };

            List<SubTuningStep> eSubStep_List = new List<SubTuningStep>();

            switch (eSubStep)
            {
                case SubTuningStep.NO:
                case SubTuningStep.TILTNO_PTHF:
                    return true;
                case SubTuningStep.TILTNO_BHF:
                    eSubStep_List.Add(SubTuningStep.TILTNO_PTHF);
                    break;
                case SubTuningStep.HOVER_1ST:
                case SubTuningStep.HOVER_2ND:
                case SubTuningStep.PCHOVER_1ST:
                case SubTuningStep.PCHOVER_2ND:
                case SubTuningStep.DIGIGAIN:
                case SubTuningStep.TP_GAIN:
                    eSubStep_List.Add(SubTuningStep.NO);
                    break;
                case SubTuningStep.PCCONTACT:
                    if (nStepState == CheckState.m_nSTEPSTATE_SUBSTEP)
                    {
                        eSubStep_List.Add(SubTuningStep.PCHOVER_1ST);
                        eSubStep_List.Add(SubTuningStep.PCHOVER_2ND);
                    }
                    else
                    {
                        eSubStep_List.Add(SubTuningStep.NO);
                        eSubStep_List.Add(SubTuningStep.PCHOVER_1ST);
                        eSubStep_List.Add(SubTuningStep.PCHOVER_2ND);
                    }

                    break;
                case SubTuningStep.CONTACT:
                    if (ParamAutoTuning.m_nContactStepFilterType == 1 || ParamAutoTuning.m_nContactStepFilterType == 3)
                    {
                        eSubStep_List.Add(SubTuningStep.NO);
                        eSubStep_List.Add(SubTuningStep.PCHOVER_1ST);
                        eSubStep_List.Add(SubTuningStep.PCHOVER_2ND);
                    }
                    else
                    {
                        eSubStep_List.Add(SubTuningStep.NO);
                        eSubStep_List.Add(SubTuningStep.HOVER_2ND);
                    }

                    break;
                case SubTuningStep.HOVERTRxS:
                    eSubStep_List.Add(SubTuningStep.CONTACT);
                    break;
                case SubTuningStep.CONTACTTRxS:
                    eSubStep_List.Add(SubTuningStep.CONTACT);
                    eSubStep_List.Add(SubTuningStep.HOVERTRxS);
                    break;
                case SubTuningStep.TILTTUNING_PTHF:
                    if ((FileProcess.m_nCheckStepFlag & FileProcess.m_nMAINSTEP_TILTNOISE) == 0)
                    {
                        eSubStep_List.Add(SubTuningStep.CONTACTTRxS);
                    }
                    else
                    {
                        eSubStep_List.Add(SubTuningStep.CONTACTTRxS);
                        eSubStep_List.Add(SubTuningStep.TILTNO_BHF);
                    }

                    break;
                case SubTuningStep.TILTTUNING_BHF:
                    if (nStepState == CheckState.m_nSTEPSTATE_SUBSTEP)
                    {
                        eSubStep_List.Add(SubTuningStep.TILTTUNING_PTHF);
                    }
                    else
                    {
                        if ((FileProcess.m_nCheckStepFlag & FileProcess.m_nMAINSTEP_TILTNOISE) == 0)
                        {
                            eSubStep_List.Add(SubTuningStep.CONTACTTRxS);
                            eSubStep_List.Add(SubTuningStep.TILTTUNING_PTHF);
                        }
                        else
                        {
                            eSubStep_List.Add(SubTuningStep.CONTACTTRxS);
                            eSubStep_List.Add(SubTuningStep.TILTNO_BHF);
                            eSubStep_List.Add(SubTuningStep.TILTTUNING_PTHF);
                        }
                    }

                    break;
                case SubTuningStep.PRESSURESETTING:
                case SubTuningStep.PRESSUREPROTECT:
                case SubTuningStep.LINEARITYTABLE:
                    //eSubStep_List.Add(SubTuningStep.NO);
                    eSubStep_List.Add(SubTuningStep.CONTACTTRxS);
                    break;
                case SubTuningStep.PRESSURETABLE:
                    if (nStepState == CheckState.m_nSTEPSTATE_PRESSURETABLE)
                    {
                        eSubStep_List.Add(SubTuningStep.PRESSURESETTING);
                    }
                    else
                    {
                        eSubStep_List.Add(SubTuningStep.CONTACTTRxS);
                        eSubStep_List.Add(SubTuningStep.PRESSURESETTING);
                    }

                    break;
                default:
                    break;
            }

            if (nStepState == CheckState.m_nSTEPSTATE_NORMAL)
            {
                if (cCheckState.CheckSetDigiGain(eMainStep, eSubStep) == CheckState.m_nSETDIGIGAIN_COMPUTEVALUE)
                {
                    if (m_bGetStepInfoFlag_DigiGainTuning == true)
                        eSubStep_List.Add(SubTuningStep.DIGIGAIN);
                }
            }

            foreach (SubTuningStep eEachSubStep in eSubStep_List)
            {
                eCurrentLoadSubStep = eEachSubStep;

                int nStateFlag = nSTATE_NA;

                if (((eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS) && eCurrentLoadSubStep == SubTuningStep.CONTACT) ||
                    ((eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSUREPROTECT || eSubStep == SubTuningStep.PRESSURETABLE) &&
                     (eCurrentLoadSubStep == SubTuningStep.CONTACTTRxS || eCurrentLoadSubStep == SubTuningStep.PRESSURESETTING)) ||
                    (eSubStep == SubTuningStep.LINEARITYTABLE && eCurrentLoadSubStep == SubTuningStep.CONTACTTRxS) ||
                    eCurrentLoadSubStep == SubTuningStep.DIGIGAIN)
                    nStateFlag |= nSTATE_NORMALLIST;
                else if ((eSubStep == SubTuningStep.TILTTUNING_PTHF || eSubStep == SubTuningStep.TILTTUNING_BHF) && (eCurrentLoadSubStep == SubTuningStep.CONTACTTRxS || eCurrentLoadSubStep == SubTuningStep.TILTNO_BHF))
                    nStateFlag |= nSTATE_TILTNOLIST;
                else
                    nStateFlag |= nSTATE_DATAINFOLIST;

                if ((eSubStep == SubTuningStep.TILTTUNING_PTHF || eSubStep == SubTuningStep.TILTTUNING_BHF) && eCurrentLoadSubStep == SubTuningStep.TILTNO_BHF)
                    nStateFlag |= nSTATE_DATAINFOLIST;

                if (bSkipPreviousStepFlag == true &&
                    ((eMainStep == MainTuningStep.DIGIGAINTUNING && eCurrentLoadSubStep == SubTuningStep.NO) ||
                     (eMainStep == MainTuningStep.TPGAINTUNING && eCurrentLoadSubStep == SubTuningStep.NO) ||
                     (eMainStep == MainTuningStep.PEAKCHECKTUNING && eCurrentLoadSubStep == SubTuningStep.NO) ||
                     (eMainStep == MainTuningStep.DIGITALTUNING && (eCurrentLoadSubStep == SubTuningStep.NO || eCurrentLoadSubStep == SubTuningStep.HOVERTRxS || eCurrentLoadSubStep == SubTuningStep.CONTACTTRxS)) ||
                     (eMainStep == MainTuningStep.TILTTUNING && (eCurrentLoadSubStep == SubTuningStep.CONTACTTRxS || eCurrentLoadSubStep == SubTuningStep.TILTNO_BHF)) ||
                     (eMainStep == MainTuningStep.PRESSURETUNING && eCurrentLoadSubStep == SubTuningStep.CONTACTTRxS) ||
                     (eMainStep == MainTuningStep.LINEARITYTUNING && (eCurrentLoadSubStep == SubTuningStep.CONTACTTRxS || eCurrentLoadSubStep == SubTuningStep.NO))))
                    continue;

                string[] sDataInfoColumnName_Array = null;
                string[] sFWParameterColumnName_Array = null;

                switch (eCurrentLoadSubStep)
                {
                    case SubTuningStep.NO:
                        sDataInfoTitle = "Ranking Information List";

                        if (eSubStep == SubTuningStep.HOVER_1ST || eSubStep == SubTuningStep.HOVER_2ND || eSubStep == SubTuningStep.CONTACT)
                        {
                            sDataInfoColumnName_Array = new string[11] 
                            { 
                                SpecificText.m_sP0_Detect_Time,
                                SpecificText.m_sRXInnerMax,
                                SpecificText.m_sTXInnerMax,
                                SpecificText.m_sRXInnerMaxPlus3InnerMaxSTD,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_1Trc,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_2Trc,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_3Trc,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_4Trc,
                                SpecificText.m_scActivePen_DigiGain_P0,
                                SpecificText.m_scActivePen_DigiGain_Beacon_Rx,
                                SpecificText.m_scActivePen_DigiGain_Beacon_Tx 
                            };
                        }
                        else if (eSubStep == SubTuningStep.PCHOVER_1ST || eSubStep == SubTuningStep.PCHOVER_2ND || eSubStep == SubTuningStep.PCCONTACT)
                        {
                            sDataInfoColumnName_Array = new string[2] 
                            { 
                                SpecificText.m_sRXInnerMax,
                                SpecificText.m_scActivePen_DigiGain_Beacon_Rx 
                            };
                        }
                        else if (eSubStep == SubTuningStep.DIGIGAIN)
                        {
                            sDataInfoColumnName_Array = new string[9] 
                            { 
                                SpecificText.m_sP0_Detect_Time,
                                SpecificText.m_sRXInnerMax,
                                SpecificText.m_sTXInnerMax,
                                SpecificText.m_sRXInnerMaxPlus3InnerMaxSTD,
                                SpecificText.m_sTXInnerMaxPlus3InnerMaxSTD,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_1Trc,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_2Trc,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_3Trc,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_4Trc 
                            };
                        }
                        else if (eSubStep == SubTuningStep.TP_GAIN)
                        {
                            sDataInfoColumnName_Array = new string[10] 
                            { 
                                SpecificText.m_sP0_Detect_Time,
                                SpecificText.m_sRXInnerMaxPlus3InnerMaxSTD,
                                SpecificText.m_sTXInnerMaxPlus3InnerMaxSTD,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_1Trc,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_2Trc,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_3Trc,
                                SpecificText.m_sTrcMaxMinusPreP0_TH_4Trc,
                                SpecificText.m_scActivePen_DigiGain_P0,
                                SpecificText.m_scActivePen_DigiGain_Beacon_Rx,
                                SpecificText.m_scActivePen_DigiGain_Beacon_Tx 
                            };
                        }
                        else if (eSubStep == SubTuningStep.LINEARITYTABLE)
                        {
                            sDataInfoColumnName_Array = new string[4] 
                            { 
                                SpecificText.m_sRXInnerMaxPlus3InnerMaxSTD,
                                SpecificText.m_sTXInnerMaxPlus3InnerMaxSTD,
                                SpecificText.m_scActivePen_DigiGain_Beacon_Rx,
                                SpecificText.m_scActivePen_DigiGain_Beacon_Tx 
                            };
                        }

                        break;
                    case SubTuningStep.TILTNO_PTHF:
                        sDataInfoTitle = "Step Data Information List";
                        sDataInfoColumnName_Array = new string[2] 
                        { 
                            SpecificText.m_sRXTotalMax, 
                            SpecificText.m_sTXTotalMax 
                        };
                        break;
                    case SubTuningStep.TILTNO_BHF:
                        sDataInfoTitle = "Step Data Information List";
                        sParameterInfoTitle = "FW Parameter Information List";

                        sDataInfoColumnName_Array = new string[4] 
                        { 
                            SpecificText.m_scActivePen_DigiGain_PTHF_Rx,
                            SpecificText.m_scActivePen_DigiGain_PTHF_Tx,
                            SpecificText.m_scActivePen_DigiGain_BHF_Rx,
                            SpecificText.m_scActivePen_DigiGain_BHF_Tx 
                        };

                        sFWParameterColumnName_Array = new string[8] 
                        { 
                            SpecificText.m_scActivePen_PTHF_Contact_TH_Rx,
                            SpecificText.m_scActivePen_PTHF_Contact_TH_Tx,
                            SpecificText.m_scActivePen_PTHF_Hover_TH_Rx,
                            SpecificText.m_scActivePen_PTHF_Hover_TH_Tx,
                            SpecificText.m_scActivePen_BHF_Contact_TH_Rx,
                            SpecificText.m_scActivePen_BHF_Contact_TH_Tx,
                            SpecificText.m_scActivePen_BHF_Hover_TH_Rx,
                            SpecificText.m_scActivePen_BHF_Hover_TH_Tx 
                        };
                        break;
                    case SubTuningStep.DIGIGAIN:
                        sDataInfoTitle = "Step Data Information List";
                        sParameterInfoTitle = "FW Parameter Information List";

                        sFWParameterColumnName_Array = new string[7] 
                        { 
                            SpecificText.m_scActivePen_DigiGain_P0,
                            SpecificText.m_scActivePen_DigiGain_Beacon_Rx,
                            SpecificText.m_scActivePen_DigiGain_Beacon_Tx,
                            SpecificText.m_scActivePen_DigiGain_PTHF_Rx,
                            SpecificText.m_scActivePen_DigiGain_PTHF_Tx,
                            SpecificText.m_scActivePen_DigiGain_BHF_Rx,
                            SpecificText.m_scActivePen_DigiGain_BHF_Tx 
                        };
                        break;
                    case SubTuningStep.PCHOVER_1ST:
                    case SubTuningStep.PCHOVER_2ND:
                        sDataInfoTitle = "Step Data Information List";

                        sDataInfoColumnName_Array = new string[8] 
                        { 
                            SpecificText.m_sPenPeak_1Traces_Th,
                            SpecificText.m_sPenPeak_2Traces_Th,
                            SpecificText.m_sPenPeakWidth_Th,
                            SpecificText.m_sPenPeak_4Traces_Th,
                            SpecificText.m_sPenPeak_5Traces_Th,
                            SpecificText.m_sPenPeak_5Traces_PeakPwr_Th,
                            SpecificText.m_sPenPeak_Th,
                            SpecificText.m_sPenPeakCheck_AreaUP_Pwr_TH 
                        };
                        break;
                    case SubTuningStep.HOVER_1ST:
                        sDataInfoTitle = "Step Data Information List";

                        if (ParamAutoTuning.m_nContactStepFilterType == 1)
                        {
                            sDataInfoColumnName_Array = new string[2] 
                            { 
                                SpecificText.m_sRXTotalMedian, 
                                SpecificText.m_sTXTotalMedian 
                            };
                        }
                        else if (ParamAutoTuning.m_nContactStepFilterType == 3)
                        {
                            sDataInfoColumnName_Array = new string[4] 
                            { 
                                SpecificText.m_sRXTotalMedian, 
                                SpecificText.m_sTXTotalMedian, 
                                SpecificText.m_sRXTotalMax, 
                                SpecificText.m_sTXTotalMax 
                            };
                        }

                        break;
                    case SubTuningStep.HOVER_2ND:
                        sDataInfoTitle = "Step Data Information List";

                        sDataInfoColumnName_Array = new string[6] 
                        { 
                            SpecificText.m_sP0_Detect_Time,
                            SpecificText.m_sRXTotalMedian,
                            SpecificText.m_sRXPreThreshold,
                            SpecificText.m_sRXPreTRxSThreshold,
                            SpecificText.m_sTXPreThreshold,
                            SpecificText.m_sTXPreTRxSThreshold 
                        };
                        break;
                    case SubTuningStep.CONTACT:
                        sDataInfoTitle = "Step Data Information List";

                        if (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)
                        {
                            sParameterInfoTitle = "FW Parameter Information List";

                            sFWParameterColumnName_Array = new string[12] 
                            { 
                                SpecificText.m_sRXTraceNumber,
                                SpecificText.m_sTXTraceNumber,
                                SpecificText.m_sNoiseP0_Detect_Time,
                                SpecificText.m_scActivePen_FM_P0_TH,
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Rx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Tx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Rx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Tx,
                                SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr,
                                SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr,
                                SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr,
                                SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr 
                            };
                        }
                        break;
                    case SubTuningStep.CONTACTTRxS:
                        sDataInfoTitle = "Step Data Information List";
                        sParameterInfoTitle = "FW Parameter Information List";

                        if (eSubStep == SubTuningStep.TILTTUNING_PTHF ||
                            eSubStep == SubTuningStep.TILTTUNING_BHF ||
                            eSubStep == SubTuningStep.PRESSURESETTING ||
                            eSubStep == SubTuningStep.PRESSUREPROTECT ||
                            eSubStep == SubTuningStep.PRESSURETABLE ||
                            eSubStep == SubTuningStep.LINEARITYTABLE)
                        {
                            sFWParameterColumnName_Array = new string[11] 
                            { 
                                SpecificText.m_sRXTraceNumber,
                                SpecificText.m_sTXTraceNumber,
                                SpecificText.m_scActivePen_FM_P0_TH,
                                SpecificText.m_scActivePen_Beacon_Hover_TH_Rx,
                                SpecificText.m_scActivePen_Beacon_Hover_TH_Tx,
                                SpecificText.m_scActivePen_Beacon_Contact_TH_Rx,
                                SpecificText.m_scActivePen_Beacon_Contact_TH_Tx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Rx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Tx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Rx,
                                SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Tx 
                            };
                        }
                        break;
                    case SubTuningStep.HOVERTRxS:
                        sDataInfoTitle = "Step Data Information List";
                        sDataInfoColumnName_Array = new string[2] 
                        { 
                            SpecificText.m_sRXThreshold, 
                            SpecificText.m_sTXThreshold 
                        };
                        break;
                    case SubTuningStep.TILTTUNING_PTHF:
                        sDataInfoTitle = "Step Data Information List";
                        sDataInfoColumnName_Array = new string[13] 
                        { 
                            SpecificText.m_sNormalizeRMSE_H,
                            SpecificText.m_sNormalizeRMSE_V,
                            SpecificText.m_sRXContactTH,
                            SpecificText.m_sRXHoverTH,
                            SpecificText.m_sRXRingMean,
                            SpecificText.m_sRXRingMeanMinus1STD,
                            SpecificText.m_sRXRingMeanMinus2STD,
                            SpecificText.m_sTXContactTH,
                            SpecificText.m_sTXHoverTH,
                            SpecificText.m_sTXRingMean,
                            SpecificText.m_sTXRingMeanMinus1STD,
                            SpecificText.m_sTXRingMeanMinus2STD,
                            SpecificText.m_sErrorMessage 
                        };
                        break;
                    case SubTuningStep.PRESSURESETTING:
                        sDataInfoTitle = "Step Data Information List";
                        sParameterInfoTitle = "FW Parameter Information List";

                        sFWParameterColumnName_Array = new string[3] 
                        { 
                            SpecificText.m_s_Pen_Ntrig_IQ_BSH_P,
                            SpecificText.m_scActivePen_FM_Pressure3BinsTH,
                            SpecificText.m_sPress_3BinsPwr 
                        };
                        break;
                    default:
                        break;
                }

                string sFilePath = StringConvert.ConvertStepToStepListFilePath(m_cfrmMain, cFlowStep, eEachSubStep);
                string sFileName = Path.GetFileName(sFilePath);

                if (File.Exists(sFilePath) == false)
                {
                    OutputMessage(string.Format("-{0} File Not Exist", sFileName));
                    return false;
                }

                try
                {
                    datatableDataInfo = ConvertCsvToDataTable(sFilePath, "Data Info", sDataInfoTitle, sColumnName_Array, ",");
                }
                catch
                {
                    OutputMessage(string.Format("-{0} File Information Data Convert Error", sFileName));
                    return false;
                }

                int nRecordSetCount = RecordSetInfo.m_cRecordSetParameter_List.Count;

                if (eMainStep == MainTuningStep.TILTNO && m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                {
                    nRecordSetCount = 0;

                    int nRXTraceCount = 0;
                    int nTXTraceCount = 0;

                    for (int nRecordSetIndex = 0; nRecordSetIndex < RecordSetInfo.m_cRecordSetParameter_List.Count; nRecordSetIndex++)
                    {
                        if (RecordSetInfo.m_cRecordSetParameter_List[nRecordSetIndex].m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                            nRXTraceCount++;
                        else if (RecordSetInfo.m_cRecordSetParameter_List[nRecordSetIndex].m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                            nTXTraceCount++;
                    }

                    nRecordSetCount = Math.Max(nRXTraceCount, nTXTraceCount);
                }
                else if ((eSubStep == SubTuningStep.DIGIGAIN && ParamAutoTuning.m_nDGTDrawType == 1) ||
                         eSubStep == SubTuningStep.TP_GAIN ||
                         eSubStep == SubTuningStep.TILTTUNING_PTHF ||
                         eSubStep == SubTuningStep.TILTTUNING_BHF)
                {
                    nRecordSetCount = 0;

                    int nHorizontalCount = 0;
                    int nVerticalCount = 0;

                    for (int nRecordSetIndex = 0; nRecordSetIndex < RecordSetInfo.m_cRecordSetParameter_List.Count; nRecordSetIndex++)
                    {
                        if (RecordSetInfo.m_cRecordSetParameter_List[nRecordSetIndex].m_eRobot == FlowRobot.TOUCHLINE_HOR)
                            nHorizontalCount++;
                        else if (RecordSetInfo.m_cRecordSetParameter_List[nRecordSetIndex].m_eRobot == FlowRobot.TOUCHLINE_VER)
                            nVerticalCount++;
                    }

                    nRecordSetCount = Math.Max(nHorizontalCount, nVerticalCount);
                }

                if (((eMainStep != MainTuningStep.PRESSURETUNING || eSubStep != SubTuningStep.PRESSURETABLE) &&
                     (eMainStep != MainTuningStep.LINEARITYTUNING || eSubStep != SubTuningStep.LINEARITYTABLE)) &&
                    datatableDataInfo.Rows.Count < nRecordSetCount)
                {
                    OutputMessage(string.Format("-{0} File Information No Enough Data", sFileName));
                    return false;
                }

                if ((nStateFlag & nSTATE_NORMALLIST) != 0 || (nStateFlag & nSTATE_TILTNOLIST) != 0)
                {
                    try
                    {
                        datatableFWParameter = ConvertCsvToDataTable(sFilePath, "Parameter Info", sParameterInfoTitle, sColumnName_Array, ",");
                    }
                    catch
                    {
                        OutputMessage(string.Format("-{0} File FW Parameter Data Convert Error", sFileName));
                        return false;
                    }

                    if (((eMainStep != MainTuningStep.PRESSURETUNING || eSubStep != SubTuningStep.PRESSURETABLE) &&
                         (eMainStep != MainTuningStep.LINEARITYTUNING || eSubStep != SubTuningStep.LINEARITYTABLE)) &&
                        datatableFWParameter.Rows.Count < nRecordSetCount)
                    {
                        OutputMessage(string.Format("-{0} File FW Parameter No Enough Data", sFileName));
                        return false;
                    }
                }

                string[] sValueColumnName_Array = new string[] 
                { 
                    SpecificText.m_sSettingPH1, 
                    SpecificText.m_sSettingPH2, 
                    SpecificText.m_sReadPH1, 
                    SpecificText.m_sReadPH2, 
                    SpecificText.m_sErrorMessage 
                };

                foreach (string sColumnName in sValueColumnName_Array)
                {
                    if (datatableDataInfo.Columns.Contains(sColumnName) == false)
                    {
                        OutputMessage(string.Format("-{0} File Data : \"Information Title Column\" Not Match", sFileName));
                        return false;
                    }
                }

                if ((nStateFlag & nSTATE_DATAINFOLIST) != 0)
                {
                    if (sDataInfoColumnName_Array == null)
                    {
                        OutputMessage(string.Format("-{0} File Data : \"Information Title Array\" Is Null", sFileName));
                        return false;
                    }

                    foreach (string sColumnName in sDataInfoColumnName_Array)
                    {
                        if (datatableDataInfo.Columns.Contains(sColumnName) == false)
                        {
                            OutputMessage(string.Format("-{0} File Data : \"Information Title Column\" Not Match", sFileName));
                            return false;
                        }
                    }
                }

                if ((nStateFlag & nSTATE_TILTNOLIST) != 0 || (nStateFlag & nSTATE_NORMALLIST) != 0)
                {
                    if (sFWParameterColumnName_Array == null)
                    {
                        OutputMessage(string.Format("-{0} File Data : \"Parameter Title Array\" Is Null", sFileName));
                        return false;
                    }

                    foreach (string sColumnName in sFWParameterColumnName_Array)
                    {
                        if (datatableFWParameter.Columns.Contains(sColumnName) == false)
                        {
                            OutputMessage(string.Format("-{0} File Data : \"Parameter Title Column\" Not Match", sFileName));
                            return false;
                        }
                    }
                }

                for (int nStageInfoIndex = 0; nStageInfoIndex < RecordSetInfo.m_cRecordSetParameter_List.Count; nStageInfoIndex++)
                {
                    int nRowIndex = -1;
                    int nSettingPH1 = -1;
                    int nSettingPH2 = -1;
                    int nReadPH1 = -1;
                    int nReadPH2 = -1;

                    for (int nDataInfoIndex = 0; nDataInfoIndex < datatableDataInfo.Rows.Count; nDataInfoIndex++)
                    {
                        if (ElanConvert.CheckIsInt(datatableDataInfo.Rows[nDataInfoIndex][SpecificText.m_sSettingPH1].ToString()) == true &&
                            ElanConvert.CheckIsInt(datatableDataInfo.Rows[nDataInfoIndex][SpecificText.m_sSettingPH2].ToString()) == true &&
                            ElanConvert.CheckIsInt(datatableDataInfo.Rows[nDataInfoIndex][SpecificText.m_sReadPH1].ToString()) == true &&
                            ElanConvert.CheckIsInt(datatableDataInfo.Rows[nDataInfoIndex][SpecificText.m_sReadPH2].ToString()) == true)
                        {
                            nSettingPH1 = Convert.ToInt32(datatableDataInfo.Rows[nDataInfoIndex][SpecificText.m_sSettingPH1].ToString());
                            nSettingPH2 = Convert.ToInt32(datatableDataInfo.Rows[nDataInfoIndex][SpecificText.m_sSettingPH2].ToString());
                            nReadPH1 = Convert.ToInt32(datatableDataInfo.Rows[nDataInfoIndex][SpecificText.m_sReadPH1].ToString());
                            nReadPH2 = Convert.ToInt32(datatableDataInfo.Rows[nDataInfoIndex][SpecificText.m_sReadPH2].ToString());

                            if (RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].m_nPH1 == nSettingPH1 &&
                                RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].m_nPH1 == nReadPH1 &&
                                RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].m_nPH2 == nSettingPH2 &&
                                RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].m_nPH2 == nReadPH2)
                            {
                                nRowIndex = nDataInfoIndex;
                                break;
                            }
                        }
                    }

                    if (nRowIndex == -1)
                    {
                        OutputMessage(string.Format("-{0} File Data : \"PH1 or PH2\" Set Not Found", sFileName));
                        return false;
                    }

                    string sErrorMessage = datatableDataInfo.Rows[nRowIndex][SpecificText.m_sErrorMessage].ToString();

                    if (sErrorMessage != "" && (eEachSubStep != SubTuningStep.TILTTUNING_PTHF && eEachSubStep != SubTuningStep.TILTNO_PTHF && eEachSubStep != SubTuningStep.TILTNO_BHF))
                    {
                        OutputMessage(string.Format("-{0} File Data : \"ErrorMessage\" String Error", sFileName));
                        return false;
                    }

                    if ((nStateFlag & nSTATE_DATAINFOLIST) != 0 && sDataInfoColumnName_Array != null)
                    {
                        foreach (string sColumnName in sDataInfoColumnName_Array)
                        {
                            if (sColumnName == SpecificText.m_sNormalizeRMSE_H ||
                                sColumnName == SpecificText.m_sNormalizeRMSE_V ||
                                sColumnName == SpecificText.m_sRXRingMean ||
                                sColumnName == SpecificText.m_sTXRingMean)
                            {
                                double dMainParamValue = 0.0;

                                try
                                {
                                    dMainParamValue = Convert.ToDouble(datatableDataInfo.Rows[nRowIndex][sColumnName].ToString());
                                }
                                catch
                                {
                                    OutputMessage(string.Format("-{0} File Data : \"{1}\" Value Error", sFileName, sColumnName));
                                    return false;
                                }

                                SetRecordFlowInfo(eSubStep, nStageInfoIndex, sColumnName, dMainParamValue, eCurrentLoadSubStep, m_nRECORDVALUE_DOUBLE);
                                continue;
                            }
                            else if (sColumnName == SpecificText.m_sErrorMessage)
                            {
                                string sMainParamValue = "";

                                try
                                {
                                    sMainParamValue = datatableDataInfo.Rows[nRowIndex][sColumnName].ToString();
                                }
                                catch
                                {
                                    OutputMessage(string.Format("-{0} File Data : \"{1}\" Value Error", sFileName, sColumnName));
                                    return false;
                                }

                                SetRecordFlowInfo(eSubStep, nStageInfoIndex, sColumnName, sMainParamValue, eCurrentLoadSubStep, m_nRECORDVALUE_STRING);
                                continue;
                            }
                            else if (sColumnName == SpecificText.m_sP0_Detect_Time)
                            {
                                string sMainParamValue = "";

                                try
                                {
                                    sMainParamValue = datatableDataInfo.Rows[nRowIndex][sColumnName].ToString();
                                }
                                catch
                                {
                                    OutputMessage(string.Format("-{0} File Data : \"{1}\" Value Error", sFileName, sColumnName));
                                    return false;
                                }

                                RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].SetParameter(eSubStep, sColumnName, sMainParamValue, eCurrentLoadSubStep, RecordSetParameter.m_nRECORDVALUE_STRING);
                                SetRecordFlowInfo(eSubStep, nStageInfoIndex, sColumnName, sMainParamValue, eCurrentLoadSubStep, m_nRECORDVALUE_STRING);
                                continue;
                            }

                            if (ElanConvert.CheckIsInt(datatableDataInfo.Rows[nRowIndex][sColumnName].ToString()) == false)
                            {
                                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA ||
                                    (ParamAutoTuning.m_nFlowMethodType == 1 && eMainStep == MainTuningStep.TILTTUNING && eSubStep == SubTuningStep.TILTTUNING_BHF && eCurrentLoadSubStep == SubTuningStep.TILTTUNING_PTHF))
                                    continue;
                                else
                                {
                                    OutputMessage(string.Format("-{0} File Data : \"{1}\" Value Error", sFileName, sColumnName));
                                    return false;
                                }
                            }

                            int nParameterValue = Convert.ToInt32(datatableDataInfo.Rows[nRowIndex][sColumnName].ToString());

                            if (eCurrentLoadSubStep == SubTuningStep.NO &&
                                (sColumnName != SpecificText.m_scActivePen_DigiGain_P0 &&
                                 sColumnName != SpecificText.m_scActivePen_DigiGain_Beacon_Rx &&
                                 sColumnName != SpecificText.m_scActivePen_DigiGain_Beacon_Tx))
                            {
                                if (nParameterValue < 0)
                                {
                                    OutputMessage(string.Format("-{0} File Data : \"{1}\" Value Error", sFileName, sColumnName));
                                    return false;
                                }
                                else
                                {
                                    RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].SetParameter(eSubStep, sColumnName, nParameterValue, eCurrentLoadSubStep, RecordSetParameter.m_nRECORDVALUE_INT);
                                    SetRecordFlowInfo(eSubStep, nStageInfoIndex, sColumnName, nParameterValue, eCurrentLoadSubStep, m_nRECORDVALUE_INT);
                                }
                            }
                            else
                            {
                                RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].SetParameter(eSubStep, sColumnName, nParameterValue, eCurrentLoadSubStep, RecordSetParameter.m_nRECORDVALUE_INT);
                                SetRecordFlowInfo(eSubStep, nStageInfoIndex, sColumnName, nParameterValue, eCurrentLoadSubStep, m_nRECORDVALUE_INT);
                            }
                        }
                    }

                    if (((nStateFlag & nSTATE_TILTNOLIST) != 0 || (nStateFlag & nSTATE_NORMALLIST) != 0) && sFWParameterColumnName_Array != null)
                    {
                        foreach (string sColumnName in sFWParameterColumnName_Array)
                        {
                            if (sColumnName == SpecificText.m_sNoiseP0_Detect_Time)
                            {
                                string sParameterValue = "";

                                try
                                {
                                    sParameterValue = datatableFWParameter.Rows[nRowIndex][sColumnName].ToString();
                                }
                                catch
                                {
                                    OutputMessage(string.Format("-{0} File Data : \"{1}\" Value Error", sFileName, sColumnName));
                                    return false;
                                }

                                if (sParameterValue != SpecificText.m_sP0_Detect_Time_400 && sParameterValue != SpecificText.m_sP0_Detect_Time_800)
                                {
                                    OutputMessage(string.Format("-{0} File Data : \"{1}\" Value Error", sFileName, sColumnName));
                                    return false;
                                }

                                SetRecordFlowInfo(eSubStep, nStageInfoIndex, sColumnName, sParameterValue, eCurrentLoadSubStep, m_nRECORDVALUE_STRING);
                                continue;
                            }

                            string sValue = datatableFWParameter.Rows[nRowIndex][sColumnName].ToString();

                            if (m_sDigiGainParameter_Array.Contains(sColumnName) == true)
                            {
                                string[] sSymbol_Array = sValue.Split('*');

                                if (sSymbol_Array == null || sSymbol_Array.Length < 2)
                                {
                                    OutputMessage(string.Format("-{0} File Data : \"{1}\" Value Error", sFileName, sColumnName));
                                    return false;
                                }

                                sValue = sSymbol_Array[0];
                            }

                            if (ElanConvert.CheckIsInt(sValue) == false)
                            {
                                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
                                    continue;
                                else
                                {
                                    OutputMessage(string.Format("-{0} File Data : \"{1}\" Value Error", sFileName, sColumnName));
                                    return false;
                                }
                            }

                            int nParameterValue = Convert.ToInt32(sValue);

                            if (nParameterValue < 0)
                            {
                                OutputMessage(string.Format("-{0} File Data : \"{1}\" Value Error", sFileName, sColumnName));
                                return false;
                            }
                            else
                            {
                                RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].SetParameter(eSubStep, sColumnName, nParameterValue, eCurrentLoadSubStep, RecordSetParameter.m_nRECORDVALUE_INT);
                                SetRecordFlowInfo(eSubStep, nStageInfoIndex, sColumnName, nParameterValue, eCurrentLoadSubStep, m_nRECORDVALUE_INT);
                            }
                        }
                    }
                }
            }

            if ((eMainStep == MainTuningStep.TILTTUNING && (eSubStep == SubTuningStep.TILTTUNING_PTHF || eSubStep == SubTuningStep.TILTTUNING_BHF)) ||
                (eMainStep == MainTuningStep.LINEARITYTUNING && eSubStep == SubTuningStep.LINEARITYTABLE))
            {
                for (int nStageInfoIndex = 0; nStageInfoIndex < RecordSetInfo.m_cRecordSetParameter_List.Count; nStageInfoIndex++)
                    RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].SetPTHFAndBHFThreshold(eMainStep);
            }

            return true;
        }

        /// <summary>
        /// 將CSV檔中的Data轉換為DataTable型式
        /// </summary>
        /// <param name="sFilePath">要讀取的csv檔路徑</param>
        /// <param name="sTableName">DataTable的名稱</param>
        /// <param name="sTitleName">標題字串</param>
        /// <param name="sColumnName_Array">欄位名稱陣列</param>
        /// <param name="sDelimiter">分隔的字元</param>
        /// <returns>回傳取得的DataTable</returns>
        public DataTable ConvertCsvToDataTable(string sFilePath, string sTableName, string sTitleName, string[] sColumnName_Array, string sDelimiter)
        {
            DataTable datatableDataTable = new DataTable();
            DataSet datasetDataSet = new DataSet();
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);
            bool bReadTitleFlag = false;
            string[] sCellData_Array = null;

            try
            {
                while (bReadTitleFlag == false)
                {
                    sCellData_Array = srFile.ReadLine().Split(sDelimiter.ToCharArray());

                    if (sCellData_Array == null)
                        continue;

                    if (sCellData_Array[0] == sTitleName)
                    {
                        sCellData_Array = srFile.ReadLine().Split(sDelimiter.ToCharArray());

                        if (sCellData_Array.Length >= sColumnName_Array.Length)
                        {
                            int nMatchFlag = 0;

                            for (int nTitleIndex = 0; nTitleIndex < sColumnName_Array.Length; nTitleIndex++)
                            {
                                if (sColumnName_Array[nTitleIndex] == sCellData_Array[nTitleIndex])
                                    nMatchFlag++;
                            }

                            if (nMatchFlag == sColumnName_Array.Length)
                                bReadTitleFlag = true;
                        }
                    }
                }

                datasetDataSet.Tables.Add(sTableName);

                foreach (string sCell in sCellData_Array)
                {
                    bool bAddFlag = false;
                    string sNextText = "";
                    int nCount = 0;

                    while (bAddFlag == false)
                    {
                        string sColumnName = sCell + sNextText;
                        sColumnName = sColumnName.Replace("#", "");
                        sColumnName = sColumnName.Replace("'", "");
                        sColumnName = sColumnName.Replace("&", "");

                        if (!datasetDataSet.Tables[sTableName].Columns.Contains(sColumnName))
                        {
                            datasetDataSet.Tables[sTableName].Columns.Add(sColumnName);
                            bAddFlag = true;
                        }
                        else
                        {
                            nCount++;
                            sNextText = "_" + nCount.ToString();
                        }
                    }
                }

                string sAllData = srFile.ReadToEnd();
                sAllData = sAllData.Replace("\r", "");
                string[] sRowData_Array = sAllData.Split("\n".ToCharArray());
                int nDataWidthCount = datasetDataSet.Tables[sTableName].Columns.Count;

                foreach (string sRowData in sRowData_Array)
                {
                    if (sRowData == "")
                        break;

                    string[] sItemData_Array = sRowData.Split(sDelimiter.ToCharArray());

                    if (sItemData_Array.Length != nDataWidthCount)
                        break;

                    datasetDataSet.Tables[sTableName].Rows.Add(sItemData_Array);
                }

                srFile.Close();

                datatableDataTable = datasetDataSet.Tables[0];

                return datatableDataTable;
            }
            finally
            {
                srFile.Close();
            }
        }

        /// <summary>
        /// 設定各階段的Record Flow Info
        /// </summary>
        /// <param name="eSubStep">現在的子階段</param>
        /// <param name="nItemIndex">項目索引值</param>
        /// <param name="sParamName">參數名稱</param>
        /// <param name="objParameterValue">參數數值</param>
        /// <param name="CurLoadStep">現在所讀取的StepList檔其所屬階段</param>
        /// <param name="nRecordValueType">參數數值種類</param>
        private void SetRecordFlowInfo(SubTuningStep eSubStep, int nItemIndex, string sParameterName, object objParameterValue, SubTuningStep eCurrentLoadSubStep, int nRecordValueType)
        {
            if (nRecordValueType == m_nRECORDVALUE_STRING)
            {
                string sParameterValue = Convert.ToString(objParameterValue);

                switch (eSubStep)
                {
                    case SubTuningStep.NO:
                        RecordFlowInfo.m_cNoiseParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTNO_PTHF:
                        RecordFlowInfo.m_cTNPTHFParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTNO_BHF:
                        RecordFlowInfo.m_cTNBHFParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.DIGIGAIN:
                        RecordFlowInfo.m_cDGTParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TP_GAIN:
                        RecordFlowInfo.m_cTPGTParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PCHOVER_1ST:
                        RecordFlowInfo.m_cPCTH1stParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PCHOVER_2ND:
                        RecordFlowInfo.m_cPCTH2ndParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PCCONTACT:
                        RecordFlowInfo.m_cPCTCParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.HOVER_1ST:
                        RecordFlowInfo.m_cDTH1stParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.HOVER_2ND:
                        RecordFlowInfo.m_cDTH2ndParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.CONTACT:
                        RecordFlowInfo.m_cDTCParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.HOVERTRxS:
                        RecordFlowInfo.m_cDTHTRxSParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.CONTACTTRxS:
                        RecordFlowInfo.m_cDTCTRxSParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTTUNING_PTHF:
                        RecordFlowInfo.m_cTTPTHFParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTTUNING_BHF:
                        RecordFlowInfo.m_cTTBHFParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PRESSUREPROTECT:
                        RecordFlowInfo.m_cPProtectParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PRESSURESETTING:
                        RecordFlowInfo.m_cPSettingParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PRESSURETABLE:
                        RecordFlowInfo.m_cPTableParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.LINEARITYTABLE:
                        RecordFlowInfo.m_cLTuningParameter_List[nItemIndex].SetStringParameter(sParameterName, sParameterValue, eCurrentLoadSubStep);
                        break;
                    default:
                        break;
                }
            }
            else if (nRecordValueType == m_nRECORDVALUE_INT)
            {
                int nParameterValue = -1;
                string sParameterValue = Convert.ToString(objParameterValue);
                Int32.TryParse(sParameterValue, out nParameterValue);

                switch (eSubStep)
                {
                    case SubTuningStep.NO:
                        RecordFlowInfo.m_cNoiseParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTNO_PTHF:
                        RecordFlowInfo.m_cTNPTHFParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTNO_BHF:
                        RecordFlowInfo.m_cTNBHFParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.DIGIGAIN:
                        RecordFlowInfo.m_cDGTParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TP_GAIN:
                        RecordFlowInfo.m_cTPGTParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PCHOVER_1ST:
                        RecordFlowInfo.m_cPCTH1stParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PCHOVER_2ND:
                        RecordFlowInfo.m_cPCTH2ndParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PCCONTACT:
                        RecordFlowInfo.m_cPCTCParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.HOVER_1ST:
                        RecordFlowInfo.m_cDTH1stParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.HOVER_2ND:
                        RecordFlowInfo.m_cDTH2ndParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.CONTACT:
                        RecordFlowInfo.m_cDTCParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.HOVERTRxS:
                        RecordFlowInfo.m_cDTHTRxSParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.CONTACTTRxS:
                        RecordFlowInfo.m_cDTCTRxSParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTTUNING_PTHF:
                        RecordFlowInfo.m_cTTPTHFParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTTUNING_BHF:
                        RecordFlowInfo.m_cTTBHFParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PRESSUREPROTECT:
                        RecordFlowInfo.m_cPProtectParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PRESSURESETTING:
                        RecordFlowInfo.m_cPSettingParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PRESSURETABLE:
                        RecordFlowInfo.m_cPTableParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.LINEARITYTABLE:
                        RecordFlowInfo.m_cLTuningParameter_List[nItemIndex].SetIntParameter(sParameterName, nParameterValue, eCurrentLoadSubStep);
                        break;
                    default:
                        break;
                }
            }
            else if (nRecordValueType == m_nRECORDVALUE_DOUBLE)
            {
                double dParameterValue = -1.0;
                string sParameterValue = Convert.ToString(objParameterValue);
                Double.TryParse(sParameterValue, out dParameterValue);

                switch (eSubStep)
                {
                    case SubTuningStep.NO:
                        RecordFlowInfo.m_cNoiseParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTNO_PTHF:
                        RecordFlowInfo.m_cTNPTHFParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTNO_BHF:
                        RecordFlowInfo.m_cTNBHFParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.DIGIGAIN:
                        RecordFlowInfo.m_cDGTParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TP_GAIN:
                        RecordFlowInfo.m_cTPGTParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PCHOVER_1ST:
                        RecordFlowInfo.m_cPCTH1stParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PCHOVER_2ND:
                        RecordFlowInfo.m_cPCTH2ndParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PCCONTACT:
                        RecordFlowInfo.m_cPCTCParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.HOVER_1ST:
                        RecordFlowInfo.m_cDTH1stParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.HOVER_2ND:
                        RecordFlowInfo.m_cDTH2ndParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.CONTACT:
                        RecordFlowInfo.m_cDTCParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.HOVERTRxS:
                        RecordFlowInfo.m_cDTHTRxSParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.CONTACTTRxS:
                        RecordFlowInfo.m_cDTCTRxSParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTTUNING_PTHF:
                        RecordFlowInfo.m_cTTPTHFParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.TILTTUNING_BHF:
                        RecordFlowInfo.m_cTTBHFParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PRESSUREPROTECT:
                        RecordFlowInfo.m_cPProtectParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PRESSURESETTING:
                        RecordFlowInfo.m_cPSettingParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.PRESSURETABLE:
                        RecordFlowInfo.m_cPTableParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    case SubTuningStep.LINEARITYTABLE:
                        RecordFlowInfo.m_cLTuningParameter_List[nItemIndex].SetDoubleParameter(sParameterName, dParameterValue, eCurrentLoadSubStep);
                        break;
                    default:
                        break;
                }
            }
            else if (nRecordValueType == m_nRECORDVALUE_ERRORINFORMATION)
            {
                string sParameterValue = Convert.ToString(objParameterValue);

                switch (eSubStep)
                {
                    case SubTuningStep.NO:
                        RecordFlowInfo.m_cNoiseParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.TILTNO_PTHF:
                        RecordFlowInfo.m_cTNPTHFParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.TILTNO_BHF:
                        RecordFlowInfo.m_cTNBHFParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.DIGIGAIN:
                        RecordFlowInfo.m_cDGTParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.TP_GAIN:
                        RecordFlowInfo.m_cTPGTParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.PCHOVER_1ST:
                        RecordFlowInfo.m_cPCTH1stParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.PCHOVER_2ND:
                        RecordFlowInfo.m_cPCTH2ndParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.PCCONTACT:
                        RecordFlowInfo.m_cPCTCParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.HOVER_1ST:
                        RecordFlowInfo.m_cDTH1stParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.HOVER_2ND:
                        RecordFlowInfo.m_cDTH2ndParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.CONTACT:
                        RecordFlowInfo.m_cDTCParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.HOVERTRxS:
                        RecordFlowInfo.m_cDTHTRxSParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.CONTACTTRxS:
                        RecordFlowInfo.m_cDTCTRxSParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.TILTTUNING_PTHF:
                        RecordFlowInfo.m_cTTPTHFParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.TILTTUNING_BHF:
                        RecordFlowInfo.m_cTTBHFParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.PRESSUREPROTECT:
                        RecordFlowInfo.m_cPProtectParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.PRESSURESETTING:
                        RecordFlowInfo.m_cPSettingParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.PRESSURETABLE:
                        RecordFlowInfo.m_cPTableParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    case SubTuningStep.LINEARITYTABLE:
                        RecordFlowInfo.m_cLTuningParameter_List[nItemIndex].SetErrorInformation(sParameterName, sParameterValue);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 確認HoverTRxS與ContactTRxS階段的參數大小是否合理
        /// </summary>
        /// <param name="sErrorMessage">錯誤訊息</param>
        /// <param name="eSubStep">現階段的子階段</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool CheckHoverThresholdIsValid(ref string sErrorMessage, SubTuningStep eSubStep)
        {
            if (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)
            {
                if (ParamAutoTuning.m_nDTSkipCompareThreshold != 1)
                {
                    for (int nStageInfoIndex = 0; nStageInfoIndex < RecordSetInfo.m_cRecordSetParameter_List.Count; nStageInfoIndex++)
                    {
                        int nTRxS_Beacon_Hover_TH_Rx = RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].m_nTRxS_Beacon_Hover_TH_Rx;
                        int nTRxS_Beacon_Contact_TH_Rx = RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].m_nTRxS_Beacon_Contact_TH_Rx;
                        int nTRxS_Beacon_Hover_TH_Tx = RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].m_nTRxS_Beacon_Hover_TH_Tx;
                        int nTRxS_Beacon_Contact_TH_Tx = RecordSetInfo.m_cRecordSetParameter_List[nStageInfoIndex].m_nTRxS_Beacon_Contact_TH_Tx;

                        if (nTRxS_Beacon_Hover_TH_Rx > nTRxS_Beacon_Contact_TH_Rx)
                        {
                            sErrorMessage = string.Format("[Hover_TH_Rx>Contact_TH_Rx] Check Error in Record Data Set {0}", nStageInfoIndex + 1);
                            return false;
                        }

                        if (nTRxS_Beacon_Hover_TH_Tx > nTRxS_Beacon_Contact_TH_Tx)
                        {
                            sErrorMessage = string.Format("[Hover_TH_Tx>Contact_TH_Tx] Check Error in Record Data Set {0}", nStageInfoIndex + 1);
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        #endregion

        #region Set Finish and Force Stop Function
        private void SetFinishFlow()
        {
            OutputMessage("-Set Finish Flow");

            SetNewStopButton(false);

            Set_Reset();

            if (m_cfrmMain.m_bDeviceConnectFlag == true)
            {
                ElanTouch.Disconnect();
                m_cfrmMain.m_bDeviceConnectFlag = false;
            }

            if (m_cFinishFlowParameter.m_bErrorFlag == true)
            {
                if (m_cFinishFlowParameter.m_eFinishState == FinishState.RunStop || m_cFinishFlowParameter.m_eFinishState == FinishState.RunResult)
                {
                    ReturnToOriginAndCloseClient();

                    /*
                    SetStepCostTime(m_cFinishFlowParameter.m_nFlowStepIndex, m_cFinishFlowParameter.m_bSetLastStepFlag, m_cFinishFlowParameter.m_bSetTimeStopFlag);
                    m_swSingleStep.Stop();
                    ResetFlowFlag();
                    */
                    SetStepCostTime(m_cFinishFlowParameter.m_nFlowStepIndex, true, true);
                }

                OutputMessage(string.Format("-{0}", m_sErrorMessage));
                OutputStateMessage(m_sErrorMessage, m_cFinishFlowParameter.m_bStateMessageFlag);

                /*
                if (m_cFinishFlowParameter.m_eFinishState == FinishState.RunResult)
                {
                    if (m_cFinishFlowParameter.m_bSetConnectButtonFlag == true)
                    {
                        SetNewConnectButton(m_cFinishFlowParameter.m_bConnectButtonEnableFlag);
                        SetNewStopButton(m_cFinishFlowParameter.m_bStopButtonEnableFlag);
                    }
                }
                else
                {
                    if (m_cFinishFlowParameter.m_bShowMessageBoxFlag == true)
                    {
                        SetTopMost(true, m_sErrorMessage);
                        SetTopMost(false, "");
                    }

                    SetNewConnectButton(m_cFinishFlowParameter.m_bConnectButtonEnableFlag);
                    SetNewStopButton(m_cFinishFlowParameter.m_bStopButtonEnableFlag);
                }
                */

                if (m_cFinishFlowParameter.m_eFinishState != FinishState.RunResult)
                {
                    if (m_cFinishFlowParameter.m_bShowMessageBoxFlag == true)
                    {
                        SetTopMost(true, m_sErrorMessage);
                        SetTopMost(false, "");
                    }
                }

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
                {
                    SetNewConnectButton(false);
                    SetNewStartButton(true);
                    SetNewStopButton(false);
                }
                else
                {
                    SetNewConnectButton(true);
                    SetNewStartButton(false);
                    SetNewStopButton(false);
                }

                SetModeStateComboBoxAndSettingToolStripMenuItem(true);
                SetNewDrawButton(MainConstantParameter.m_nDRAWSTATE_FINISH);
                SetNewPatternAndNewDrawButton(false);

                /*
                if (m_cFinishFlowParameter.m_eFinishState != FinishState.RunResult)
                {
                    if (m_cFinishFlowParameter.m_bSetStartButtonFlag == true)
                        SetNewStartButton(m_cFinishFlowParameter.m_bStartButtonEnableFlag);
                }
                */

                if (m_cFinishFlowParameter.m_eFinishState == FinishState.RunStop || m_cFinishFlowParameter.m_eFinishState == FinishState.RunResult)
                {
                    OutputCostTimeGroupBox();
                    WriteMainStepCostTimeInfo();
                }
            }
            else
            {
                ReturnToOriginAndCloseClient();

                //SetStepCostTime(m_cFinishFlowParameter.m_nFlowStepIndex, m_cFinishFlowParameter.m_bSetLastStepFlag, m_cFinishFlowParameter.m_bSetTimeStopFlag);
                //m_swSingleStep.Stop();
                //ResetFlowFlag();
                SetStepCostTime(m_cFinishFlowParameter.m_nFlowStepIndex, true, true);

                OutputStateMessage(m_sErrorMessage, true);

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
                {
                    SetNewConnectButton(false);
                    SetNewStartButton(true);
                    SetNewStopButton(false);
                }
                else
                {
                    SetNewConnectButton(true);
                    SetNewStartButton(false);
                    SetNewStopButton(false);
                }

                SetModeStateComboBoxAndSettingToolStripMenuItem(true);
                SetNewDrawButton(MainConstantParameter.m_nDRAWSTATE_FINISH);
                SetNewPatternAndNewDrawButton(false);

                OutputCostTimeGroupBox();
                WriteMainStepCostTimeInfo();
            }

            m_cfrmMain.SetResultMessagePanelFocus();

            m_bProcessFinishFlag = true;
        }

        /// <summary>
        /// 線測機回到原點並中斷Client端
        /// </summary>
        public void ReturnToOriginAndCloseClient()
        {
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT && m_bClientCloseFlag == false)
            {
                OutputStatusAndErrorMessageLabel("Return", "", Color.Blue);
                ReturnToOriginByLTRobot();
                SetClientClose();
            }
            else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW && m_bGoDrawConnectFlag == true)
            {
                OutputStatusAndErrorMessageLabel("Return", "", Color.Blue);
                SetGoDrawReturnToOriginAndClose();
            }
        }

        /// <summary>
        /// 設定此階段的花費時間
        /// </summary>
        /// <param name="nStepIndex">現階段索引值</param>
        /// <param name="bSetLastStepFlag">是否為最後一個階段</param>
        /// <param name="bSetTimeStopFlag">是否要停止計時</param>
        private void SetStepCostTime(int nStepIndex, bool bSetLastStepFlag = true, bool bSetTimeStopFlag = false)
        {
            if (nStepIndex <= -1)
                return;

            if (bSetLastStepFlag == true)
                m_cfrmMain.m_cFlowStep_List[nStepIndex].m_bLastStep = true;

            if ((m_cfrmMain.m_cFlowStep_List[nStepIndex].m_nSubStepState & MainConstantParameter.m_nSTEPLOCATION_LAST) != 0 || m_cfrmMain.m_cFlowStep_List[nStepIndex].m_bLastStep == true)
            {
                //m_swSingleStep.Stop();

                TimeSpan timespanDiffer = m_swSingleStep.Elapsed;
                TimeSpan timespanStepDiffer;

                if (m_bFirstStepCostTimeFlag == false)
                {
                    timespanStepDiffer = timespanDiffer;
                    m_bFirstStepCostTimeFlag = true;
                }
                else
                    timespanStepDiffer = timespanDiffer - m_timespanPreviousStepDiffer;

                m_timespanPreviousStepDiffer = timespanDiffer;

                int nDayToHourOffset = timespanStepDiffer.Days * 24;
                int nRealHours = timespanStepDiffer.Hours + nDayToHourOffset;

                m_cfrmMain.m_cFlowStep_List[nStepIndex].m_nHours = nRealHours;
                m_cfrmMain.m_cFlowStep_List[nStepIndex].m_nMinutes = timespanStepDiffer.Minutes;
                m_cfrmMain.m_cFlowStep_List[nStepIndex].m_nSeconds = timespanStepDiffer.Seconds;
            }

            if (bSetTimeStopFlag == true)
            {
                m_swSingleStep.Stop();
                ResetFlowFlag();
            }
        }

        /// <summary>
        /// 強制終止的流程
        /// </summary>
        /// <param name="bOutputMessageFlag">是否要輸出訊息</param>
        /// <param name="bCloseAPFlag">是否有要關閉AP</param>
        public void RunForceStopFlow(bool bOutputMessageFlag = true, bool bCloseAPFlag = false)
        {
            OutputMessage("-Run Force Stop Flow");

            if (m_cfrmMain.m_bForceStopFlowEnableFlag == true)
                return;

            if (m_cfrmMain.m_bRunStopFlowFinishFlag == true)
                return;

            StopFlowInfo cStopFlowInfo = new StopFlowInfo();
            cStopFlowInfo.m_bOutputMessageFlag = bOutputMessageFlag;
            cStopFlowInfo.m_bCloseAPFlag = bCloseAPFlag;

            ParameterizedThreadStart tStopParameter = new ParameterizedThreadStart(RunStopFlowThread);
            m_tStop = new Thread(tStopParameter);
            m_tStop.IsBackground = true;
            m_tStop.Start(cStopFlowInfo);
            m_tStop.Join();
        }

        private void RunStopFlowThread(object objStopInfo)
        {
            OutputStatusAndErrorMessageLabel("Stop", "", Color.Blue);
            SetNewStopButton(false);

            StopFlowInfo cStopFlowInfo = (StopFlowInfo)objStopInfo;

            m_cfrmMain.m_bForceStopFlowEnableFlag = true;

            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW ||
                m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
            {
                if ((m_cfrmMain.m_cCurrentFlowStep.m_eMainStep == MainTuningStep.NO ||
                     m_cfrmMain.m_cCurrentFlowStep.m_eMainStep == MainTuningStep.TILTNO))
                {
                    if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    {
                        CheckReportExist(true);
                        Set_Normal_Mode_Gen8();
                    }
                    else
                        Set_Stop_Get_NonSync_RXTX_Data();
                }

                Set_Reset();
            }

            m_cfrmMain.m_bRunStopFlowFinishFlag = true;

            //GoDraw Robot Return to Origin and Close Client
            ReturnToOriginAndCloseClient();

            //m_sErrorMessage = cStopFlowInfo.m_sMessage;

            if (m_cfrmMain.IsMdiChild == false)
            {
                if (cStopFlowInfo.m_bOutputMessageFlag == true)
                    OutputMessage(string.Format("-{0}", m_sErrorMessage));

                if (cStopFlowInfo.m_bCloseAPFlag == false)
                    OutputStatusAndErrorMessageLabel("Stop", "", Color.Blue, true);

                SetNewConnectButton(false);
                SetNewStopButton(false);
                SetNewDrawButton(MainConstantParameter.m_nDRAWSTATE_FINISH);
                SetScreenResetFlow();

                HideFullScreen();

                SetNewPatternAndNewDrawButton(false);
            }

            m_cfrmMain.m_nRetryCount = ParamAutoTuning.m_nRecordDataRetryCount;

            m_bForceStopFlag = true;
            m_nRecordDataErrorFlag = 1;
            m_nRecordPrepareFlag = MainConstantParameter.m_nFLOWSTATE_INITIALIZE;
            m_bRecordStartFlag = true;

            /*
            if (cStopFlowInfo.m_bClientDisconnectFlag == true)
                m_bClientDisconnectFlag = true;
            */

            /*
            if (m_tRecord != null && m_tRecord.IsAlive == true)
            {
                m_tRecord.Abort();
                m_tRecord.Join();
                m_tRecord = null;
            }
            */

            if (m_tRobot != null && m_tRobot.IsAlive == true)
            {
                m_tRobot.Abort();
                m_tRobot.Join();
                m_tRobot = null;
            }

            /*
            if (m_tAnalysis != null && m_tAnalysis.IsAlive == true)
            {
                m_tAnalysis.Abort();
                m_tAnalysis.Join();
                m_tAnalysis = null;
            }
            */

            m_cfrmMain.m_bProcessThreadFinishFlag = true;

            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
            {
                if (m_bClientDisconnectFlag == false)
                {
                    string sStopMessage = "client stop";
                    m_cSocket.RunClientSending(ref sStopMessage);
                }

                ReturnToOriginByLTRobot();

                /*
                string sRobotCommand = "";
                m_cRobot.socketRobot("NO", ref sRobotCommand);
                m_cSocket.RunClientSending(ref sRobotCommand);
                */
            }
            else if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SERVER && m_cSocket.CheckClientSocketConnect() == false)
            {
                m_cRobot.ReturnToOrigin();

                /*
                string sRobotCommand = "";
                m_cRobot.SocketRobot("HM", ref sRobotCommand);
                m_cSocket.RunClientSending(ref sRobotCommand);
                */
            }

            if (m_tSocket != null && m_tSocket.IsAlive == true)
                m_cSocket.RunForceStop();

            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SERVER && m_cSocket.CheckClientSocketConnect() == true)
            {
                m_cRobot.ReturnToOrigin();

                /*
                string sRobotCommand = "";
                m_cRobot.SocketRobot("HM", ref sRobotCommand);
                m_cSocket.RunClientSending(ref sRobotCommand);
                */
            }

            m_cRobot.ForceStop();
            m_cRobot.ClosePort();

            //m_cAnalysis.ForceStop(sMessage);

            //m_cfrmMain.m_bForceStopFlowEnable = true;

            m_cfrmMain.SetResultMessagePanelFocus();
        }
        #endregion

        #region Main Flow Related Function
        /// <summary>
        /// Elan Device Reconnect流程
        /// </summary>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        public bool RunElanDeviceReConnect()
        {
            bool bDeviceConnectFlag = false;
            
            OutputMessage("-Elan Device ReConnect!!");

            int nInterface = SetInterface();

            //Connect TP (Using ElanTouch API)
            ElanTouch.Disconnect();

            if (ElanTouch.Connect(
                    ParamAutoTuning.m_nUSBVID, 
                    ParamAutoTuning.m_nUSBPID, 
                    nInterface, 
                    nDVDD: ParamAutoTuning.m_nDVDD,
                    nVIO: ParamAutoTuning.m_nVIO,
                    nI2CAddr: ParamAutoTuning.m_nI2CAddress,
                    nI2CLength: ParamAutoTuning.m_nNormalLength
                ) == ElanTouch.TP_SUCCESS)
                bDeviceConnectFlag = true;

            //Check if Touch Device Exist?
            if (bDeviceConnectFlag == false)
                return false;

            //Get Device Index
            int nSelectedDeviceIndex = GetDeviceIndex();

            if (nSelectedDeviceIndex == -1)
            {
                ElanTouch.Disconnect();
                return false;
            }
            else
                m_cfrmMain.m_nDeviceIndex = nSelectedDeviceIndex;

            SetSPICommandLength();

            if (bDeviceConnectFlag == true)
            {
                // Register HID devices
                if (m_cfrmMain.m_cInputDevice.RegisterHIDDevice(ParamAutoTuning.m_nUSBVID, ParamAutoTuning.m_nUSBPID) == false)
                {
                    ElanTouch.Disconnect();
                    return false;
                }

                if (m_cfrmMain.m_cInputDevice.RegisterHIDDevice(ParamAutoTuning.m_nUSBVID, ParamAutoTuning.m_nUSBPID, "", 0xFF00) == false)
                {
                    ElanTouch.Disconnect();
                    return false;
                }
            }

            m_cInputDevice.HIDHandler -= HIDRawInputHandler;
            m_cInputDevice.HIDHandler += HIDRawInputHandler;

            Thread.Sleep(100);
            return true;
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

        private bool CheckReportExist(bool bStopFlowFlag = false)
        {
            int nRetryCount = 10;
            int nSendCount = 10;

            byte[] byteCommand_Array = ElanCommand_Gen8.ConvertGetReportTypeToByteArray(ElanCommand_Gen8.GetReportType.Stop, null, m_bEnterSetMPPParameterFlag);

            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                string sActionMessage = string.Format("{0}(RetryIndex={1})", MainConstantParameter.m_sSENDCOMMAND_StopGetGen8ReportData, nRetryIndex);

                OutputMessage(string.Format("-{0}", sActionMessage));

                for (int nCountIndex = 0; nCountIndex < nSendCount; nCountIndex++)
                    Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);

                m_bCheckReportExistFlag = true;
                m_nReportCount = 0;

                Thread.Sleep(100);

                m_bCheckReportExistFlag = false;

                if (m_nReportCount == 0)
                    break;
            }

            return true;
        }

        private bool GetRXTXTraceNumber(ref int nTraceNumber, int nTraceType)
        {
            int nTrace = 0;

            string sTraceTypeName = "RX";

            if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                sTraceTypeName = "RX";
            else if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                sTraceTypeName = "TX";

            for (int nRetryIndex = 0; nRetryIndex <= 4; nRetryIndex++)
            {
                if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                    nTrace = ElanTouch.GetRXTrace(2, 1000, m_cfrmMain.m_nDeviceIndex);
                else if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                    nTrace = ElanTouch.GetTXTrace(2, 1000, m_cfrmMain.m_nDeviceIndex);

                if (nRetryIndex == 0)
                    OutputMessage(string.Format("-Get {0}Trace={1}", sTraceTypeName, nTrace));
                else
                    OutputMessage(string.Format("-Get {0}Trace={1}(RetryCount={2})", sTraceTypeName, nTrace, nRetryIndex - 1));

                Thread.Sleep(20);

                if (nTrace > 0)
                    break;
            }

            //Check the Trace number
            if (nTrace <= 0)
            {
                m_sErrorMessage = string.Format("Get {0} Trace Error", sTraceTypeName);
                nTraceNumber = nTrace;
                return false;
            }

            nTraceNumber = nTrace;
            return true;
        }
        #endregion

        #region Send Command Function
        // Set Test Mode Function in Gen6/Gen7/Gen8
        public void Set_Test_Mode(bool bEnable, bool bOutputMessage = true)
        {
            if (bEnable == true)
            {
                if (bOutputMessage == true)
                {
                    string sActionMessage = "Set Enter Test Mode(Command : 55 55 55 55)";
                    OutputMessage(string.Format("-{0}", sActionMessage));
                }

                ElanTouch.EnableTestMode(true, 1000, m_cfrmMain.m_nDeviceIndex);
                //m_bEnterTestModeFlag = true;
            }
            else
            {
                if (bOutputMessage == true)
                {
                    string sActionMessage = "Set Exit Test Mode(Command : A5 A5 A5 A5)";
                    OutputMessage(string.Format("-{0}", sActionMessage));
                }

                ElanTouch.EnableTestMode(false, 1000, m_cfrmMain.m_nDeviceIndex);
                //m_bEnterTestModeFlag = false;
            }

            Thread.Sleep(1000);
        }

        // Set Reset Function in Gen6/Gen7/Gen8
        /// <summary>
        /// Send Reset Command
        /// </summary>
        public void Set_Reset()
        {
            lock (m_bResetLockFlag)
            {
                if (m_cfrmMain.m_bDeviceConnectFlag == false)
                    return;

                if (m_bResetFinishFlag == true)
                    return;

                /*
                if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    return;
                */

                byte[] byteCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(ElanCommand.ElanCommandType.ResetIC);

                string sActionMessage = "Set Reset";

                OutputMessage(string.Format("-{0}", sActionMessage));

                Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);

                m_bResetFinishFlag = true;

                Thread.Sleep(1000);
            }
        }

        // Set Stop Get NonSync RX/TX Data Function in Gen6/Gen7
        private void Set_Stop_Get_NonSync_RXTX_Data()
        {
            byte[] byteCommand_Array;
            ElanCommand.ElanCommandType eCommandType = ElanCommand.ElanCommandType.StopNonSyncRXTX;

            //Send Command to TP Device
            byteCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(eCommandType);

            string sActionMessage = "Set Stop Get NonSync RXTX Data";

            OutputMessage(string.Format("-{0}", sActionMessage));

            Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);
        }

        // Send FW Command Function in Gen6/Gen7/Gen8
        /// <summary>
        /// Send FW Command
        /// </summary>
        /// <param name="sCommandAction">Command動作</param>
        /// <param name="byteCommand_Array">Command串列</param>
        /// <param name="nCommandLength">Command長度</param>
        /// <param name="nTimeout">Timeout時間(ms)</param>
        private void Send_FW_Command(string sCommandAction, byte[] byteCommand_Array, int nCommandLength, int nTimeout)
        {
            ElanTouch.SendDevCommand(byteCommand_Array, nCommandLength, nTimeout, m_cfrmMain.m_nDeviceIndex);

            string sSendCommand = string.Format("-{0}(Command :", sCommandAction);

            for (int nByteIndex = 0; nByteIndex < nCommandLength; nByteIndex++)
                sSendCommand += string.Format(" {0}", byteCommand_Array[nByteIndex].ToString("X2"));

            sSendCommand += ")";

            WriteDebugLog(sSendCommand);
        }

        // Send Specific Type Function in Gen6/Gen7/Gen8
        /// <summary>
        /// 傳送特定的TP Device Command
        /// </summary>
        /// <param name="sCommandType">Command種類</param>
        private void Send_Specific_Type_Command(string sCommandType)
        {
            ElanCommand.ElanCommandType eCommandType = ElanCommand.ElanCommandType.NA;
            ElanCommand_Gen8.GetReportType eGetReportType_Gen8 = ElanCommand_Gen8.GetReportType.NA;
            bool bGen8Flag = false;

            switch (sCommandType)
            {
                case MainConstantParameter.m_sSENDCOMMAND_DISABLEREPORT:
                    eCommandType = ElanCommand.ElanCommandType.DisableICReport;
                    break;
                case MainConstantParameter.m_sSENDCOMMAND_ENABLEREPORT:
                    eCommandType = ElanCommand.ElanCommandType.EnableICReport;
                    break;
                case MainConstantParameter.m_sSENDCOMMAND_DISABLEFINGERREPORT:
                    eCommandType = ElanCommand.ElanCommandType.DisableFingerReport;
                    break;
                case MainConstantParameter.m_sSENDCOMMAND_STOPGETSYNCREPORTDATA:
                    eCommandType = ElanCommand.ElanCommandType.StopNonSyncRXTX;
                    break;
                case MainConstantParameter.m_sSENDCOMMAND_RESETTILTNOISESTATE:
                    eCommandType = ElanCommand.ElanCommandType.ResetTiltNoise;
                    break;
                case MainConstantParameter.m_sSENDCOMMAND_RESETTILTSTATE:
                    eCommandType = ElanCommand.ElanCommandType.ResetTilt;
                    break;
                case MainConstantParameter.m_sSENDCOMMAND_SetRead_Bulk_RAM_Data:
                    eCommandType = ElanCommand.ElanCommandType.SetRead_Bulk_RAM_Data;
                    break;
                case MainConstantParameter.m_sSENDCOMMAND_StopGetGen8ReportData:
                    eGetReportType_Gen8 = ElanCommand_Gen8.GetReportType.Stop;
                    bGen8Flag = true;
                    break;
                default:
                    break;
            }

            byte[] byteCommand_Array;

            if (bGen8Flag == true)
                byteCommand_Array = ElanCommand_Gen8.ConvertGetReportTypeToByteArray(eGetReportType_Gen8, null, m_bEnterSetMPPParameterFlag);
            else
                byteCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(eCommandType);

            string sActionMessage = sCommandType;

            OutputMessage(string.Format("-{0}", sActionMessage));

            Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);
            Thread.Sleep(20);
        }

        // Set Get Report Data Function in Gen6/Gen7
        /// <summary>
        /// Set Get Report Data Function in Gen6/Gen7
        /// </summary>
        /// <param name="cRecordSetParameter">此階段資訊</param>
        private void Set_Get_Report_Data_Gen6or7(RecordSetParameter cRecordSetParameter)
        {
            string sActionMessage = "";
            byte[] byteCommand_Array;
            ElanCommand.ElanCommandType eCommandType;

            GetDataType cGetDataType = new GetDataType();
            int nGetDataType = cGetDataType.SetGetDataType_Gen6or7(cRecordSetParameter);

            if ((nGetDataType & GetDataType.m_nGETDATATYPE_TILT) != 0 ||
                (nGetDataType & GetDataType.m_nGETDATATYPE_LINEARITY) != 0 ||
                (nGetDataType & GetDataType.m_nGETDATATYPE_DIGIGAIN) != 0)
            {
                eCommandType = ElanCommand.ElanCommandType.SetCoordFF;
                byteCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(eCommandType);

                sActionMessage = "Set Coordinate=0xFF(Gen6/Gen7)";

                OutputMessage(string.Format("-{0}", sActionMessage));

                Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);
                Thread.Sleep(20);
            }

            switch (nGetDataType)
            {
                case GetDataType.m_nDATATYPE_NTRX_400us:
                    eCommandType = ElanCommand.ElanCommandType.GetNoiseTRX_400us;
                    break;
                case GetDataType.m_nDATATYPE_NTRX_800us:
                    eCommandType = ElanCommand.ElanCommandType.GetNoiseTRX_800us;
                    break;
                case GetDataType.m_nDATATYPE_NRX_400us:
                    eCommandType = ElanCommand.ElanCommandType.GetNoiseRX_400us;
                    break;
                case GetDataType.m_nDATATYPE_NRX_800us:
                    eCommandType = ElanCommand.ElanCommandType.GetNoiseRX_800us;
                    break;
                case GetDataType.m_nDATATYPE_NTX_400us:
                    eCommandType = ElanCommand.ElanCommandType.GetNoiseTX_400us;
                    break;
                case GetDataType.m_nDATATYPE_NTX_800us:
                    eCommandType = ElanCommand.ElanCommandType.GetNoiseTX_800us;
                    break;
                case GetDataType.m_nDATATYPE_TRX_400us:
                    eCommandType = ElanCommand.ElanCommandType.GetNonSyncTRX_400us;
                    break;
                case GetDataType.m_nDATATYPE_TRX_800us:
                    eCommandType = ElanCommand.ElanCommandType.GetNonSyncTRX_800us;
                    break;
                case GetDataType.m_nDATATYPE_RX_400us:
                    eCommandType = ElanCommand.ElanCommandType.GetNonSyncRX_400us;
                    break;
                case GetDataType.m_nDATATYPE_RX_800us:
                    eCommandType = ElanCommand.ElanCommandType.GetNonSyncRX_800us;
                    break;
                case GetDataType.m_nDATATYPE_TX_400us:
                    eCommandType = ElanCommand.ElanCommandType.GetNonSyncTX_400us;
                    break;
                case GetDataType.m_nDATATYPE_TX_800us:
                    eCommandType = ElanCommand.ElanCommandType.GetNonSyncTX_800us;
                    break;
                case GetDataType.m_nDATATYPE_TRxS:
                    eCommandType = ElanCommand.ElanCommandType.GetSyncTRxS;
                    break;
                case GetDataType.m_nDATATYPE_DIGIGAIN:
                    eCommandType = ElanCommand.ElanCommandType.GetDigitalGain;
                    break;
                case GetDataType.m_nDATATYPE_TILT_BHF:
                    eCommandType = ElanCommand.ElanCommandType.GetTiltBHF;
                    break;
                case GetDataType.m_nDATATYPE_TILT_PTHF:
                    eCommandType = ElanCommand.ElanCommandType.GetTiltPTHF;
                    break;
                case GetDataType.m_nDATATYPE_PRESSURE:
                    eCommandType = ElanCommand.ElanCommandType.GetPressure;
                    break;
                case GetDataType.m_nDATATYPE_LINEARITY:
                    eCommandType = ElanCommand.ElanCommandType.GetLinearity;
                    break;
                case GetDataType.m_nDATATYPE_5TRAWDATA:
                    eCommandType = ElanCommand.ElanCommandType.Get5TRawData;
                    break;
                default:
                    eCommandType = ElanCommand.ElanCommandType.NA;
                    break;
            }

            //Send Command to TP Device
            byteCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(eCommandType);

            sActionMessage = "Set Get Report Data(Gen6/Gen7)";

            OutputMessage(string.Format("-{0}", sActionMessage));

            Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);
        }

        // Set Get Report Data Function in Gen8(8F18)
        /// <summary>
        /// Set Get Report Data Function in Gen8(8F18)
        /// </summary>
        /// <param name="cRecordSetParameter">此階段資訊</param>
        private void Set_Get_Report_Data_Gen8(RecordSetParameter cRecordSetParameter)
        {
            MainTuningStep eMainStep = cRecordSetParameter.m_eMainStep;
            SubTuningStep eSubStep = cRecordSetParameter.m_eSubStep;

            ElanCommand_Gen8.GetReportType eGetReportType;

            GetDataType cGetDataType = new GetDataType();
            int nGetDataType = cGetDataType.SetGetDataType_Gen8(cRecordSetParameter);

            ElanCommand_Gen8.GetReportInfo cGetReportInfo = new ElanCommand_Gen8.GetReportInfo();

            switch (nGetDataType)
            {
                case GetDataType.m_nDATATYPE_NTRX_400us:
                case GetDataType.m_nDATATYPE_NTRX_800us:
                case GetDataType.m_nDATATYPE_NRX_400us:
                case GetDataType.m_nDATATYPE_NRX_800us:
                    eGetReportType = ElanCommand_Gen8.GetReportType.debug_Rx;
                    cGetReportInfo.m_eTraceType = ElanCommand_Gen8.TraceType.RX;
                    cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                    cGetReportInfo.m_nRowNumber = ParamAutoTuning.m_nGen8BeaconRowNumber;
                    cGetReportInfo.m_nReportNumber = m_nNoiseReportNumber;
                    break;
                case GetDataType.m_nDATATYPE_NTX_400us:
                case GetDataType.m_nDATATYPE_NTX_800us:
                    eGetReportType = ElanCommand_Gen8.GetReportType.debug_Tx;
                    cGetReportInfo.m_eTraceType = ElanCommand_Gen8.TraceType.TX;
                    cGetReportInfo.m_nTraceNumber = m_nTXTraceNumber;
                    cGetReportInfo.m_nRowNumber = ParamAutoTuning.m_nGen8BeaconRowNumber;
                    cGetReportInfo.m_nReportNumber = m_nNoiseReportNumber;
                    break;
                case GetDataType.m_nDATATYPE_PTHF_NoSync_Gen8:
                    eGetReportType = ElanCommand_Gen8.GetReportType.debug_PTHF_noSync;

                    if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                    {
                        cGetReportInfo.m_eTraceType = ElanCommand_Gen8.TraceType.RX;

                        if (cRecordSetParameter.m_nSection == 0)
                        {
                            //cGetReportInfo.m_nTraceOffset = 0;
                            //cGetReportInfo.m_nTraceNumber = 32;
                            cGetReportInfo.m_nTraceOffset = 0;
                            cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                        }
                        else if (cRecordSetParameter.m_nSection == 1)
                        {
                            //cGetReportInfo.m_nTraceOffset = 17;
                            //cGetReportInfo.m_nTraceNumber = 32;
                            cGetReportInfo.m_nTraceOffset = 0;
                            cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                        }
                        else if (cRecordSetParameter.m_nSection == 2)
                        {
                            //cGetReportInfo.m_nTraceOffset = 35;
                            //cGetReportInfo.m_nTraceNumber = 32;
                            cGetReportInfo.m_nTraceOffset = 0;
                            cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                        }
                        else if (cRecordSetParameter.m_nSection == 3)
                        {
                            //cGetReportInfo.m_nTraceOffset = 53;
                            //cGetReportInfo.m_nTraceNumber = 32;
                            cGetReportInfo.m_nTraceOffset = 0;
                            cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                        }
                        else
                        {
                            cGetReportInfo.m_nTraceOffset = 0;
                            cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                        }

                        cGetReportInfo.m_nRowOffset = 0;
                        cGetReportInfo.m_nRowNumber = ParamAutoTuning.m_nGen8PTHFRowNumber;
                    }
                    else if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                    {
                        cGetReportInfo.m_eTraceType = ElanCommand_Gen8.TraceType.TX;
                        cGetReportInfo.m_nTraceNumber = m_nTXTraceNumber;
                        cGetReportInfo.m_nRowOffset = ParamAutoTuning.m_nGen8PTHFRowNumber;

                        if (m_nTXTraceNumber - ParamAutoTuning.m_nGen8PTHFRowNumber >= ParamAutoTuning.m_nGen8PTHFRowNumber)
                            cGetReportInfo.m_nRowNumber = ParamAutoTuning.m_nGen8PTHFRowNumber;
                        else
                            cGetReportInfo.m_nRowNumber = m_nTXTraceNumber - ParamAutoTuning.m_nGen8PTHFRowNumber;
                    }

                    cGetReportInfo.m_nReportNumber = m_nNoiseReportNumber;
                    break;
                case GetDataType.m_nDATATYPE_BHF_NoSync_Gen8:
                    eGetReportType = ElanCommand_Gen8.GetReportType.debug_BHF_noSync;

                    if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                    {
                        cGetReportInfo.m_eTraceType = ElanCommand_Gen8.TraceType.RX;

                        if (cRecordSetParameter.m_nSection == 0)
                        {
                            //cGetReportInfo.m_nTraceOffset = 0;
                            //cGetReportInfo.m_nTraceNumber = 32;
                            cGetReportInfo.m_nTraceOffset = 0;
                            cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                        }
                        else if (cRecordSetParameter.m_nSection == 1)
                        {
                            //cGetReportInfo.m_nTraceOffset = 17;
                            //cGetReportInfo.m_nTraceNumber = 32;
                            cGetReportInfo.m_nTraceOffset = 0;
                            cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                        }
                        else if (cRecordSetParameter.m_nSection == 2)
                        {
                            //cGetReportInfo.m_nTraceOffset = 35;
                            //cGetReportInfo.m_nTraceNumber = 32;
                            cGetReportInfo.m_nTraceOffset = 0;
                            cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                        }
                        else if (cRecordSetParameter.m_nSection == 3)
                        {
                            //cGetReportInfo.m_nTraceOffset = 53;
                            //cGetReportInfo.m_nTraceNumber = 32;
                            cGetReportInfo.m_nTraceOffset = 0;
                            cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                        }
                        else
                        {
                            cGetReportInfo.m_nTraceOffset = 0;
                            cGetReportInfo.m_nTraceNumber = m_nRXTraceNumber;
                        }

                        cGetReportInfo.m_nRowOffset = 0;
                        cGetReportInfo.m_nRowNumber = ParamAutoTuning.m_nGen8BHFRowNumber;
                    }
                    else if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                    {
                        cGetReportInfo.m_eTraceType = ElanCommand_Gen8.TraceType.TX;
                        cGetReportInfo.m_nTraceNumber = m_nTXTraceNumber;
                        cGetReportInfo.m_nRowOffset = ParamAutoTuning.m_nGen8BHFRowNumber;

                        if (m_nTXTraceNumber - ParamAutoTuning.m_nGen8BHFRowNumber >= ParamAutoTuning.m_nGen8BHFRowNumber)
                            cGetReportInfo.m_nRowNumber = ParamAutoTuning.m_nGen8BHFRowNumber;
                        else
                            cGetReportInfo.m_nRowNumber = m_nTXTraceNumber - ParamAutoTuning.m_nGen8BHFRowNumber;
                    }
                    
                    cGetReportInfo.m_nReportNumber = m_nNoiseReportNumber;
                    break;
                default:
                    eGetReportType = ElanCommand_Gen8.GetReportType.NA;
                    break;
            }

            //Send Get Report Data Command to TP Device
            byte[] byteCommand_Array = ElanCommand_Gen8.ConvertGetReportTypeToByteArray(eGetReportType, cGetReportInfo);

            string sActionMessage = "Set Get Report Data(Gen8)";

            OutputMessage(string.Format("-{0}", sActionMessage));

            Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);
            m_bEnterSetMPPParameterFlag = true;
        }

        // Set MPP Mode Type Function in Gen8(8F18)
        private void Set_MPP_Mode_Type_Gen8(RecordSetParameter cRecordSetParameter)
        {
            byte[] byteCommand_Array;
            ElanCommand_Gen8.MPPModeType eMPPModeType;
            int nSetCount = 5;

            GetDataType cGetDataType = new GetDataType();
            int nGetDataType = cGetDataType.SetGetDataType_Gen8(cRecordSetParameter);

            switch (nGetDataType)
            {
                case GetDataType.m_nDATATYPE_NTRX_400us:
                case GetDataType.m_nDATATYPE_NTRX_800us:
                case GetDataType.m_nDATATYPE_NRX_400us:
                case GetDataType.m_nDATATYPE_NRX_800us:
                    eMPPModeType = ElanCommand_Gen8.MPPModeType.RX_DFT;
                    break;
                case GetDataType.m_nDATATYPE_NTX_400us:
                case GetDataType.m_nDATATYPE_NTX_800us:
                    eMPPModeType = ElanCommand_Gen8.MPPModeType.TX_DFT;
                    break;
                case GetDataType.m_nDATATYPE_PTHF_NoSync_Gen8:
                    eMPPModeType = ElanCommand_Gen8.MPPModeType.PTHF_NoSync;
                    break;
                case GetDataType.m_nDATATYPE_BHF_NoSync_Gen8:
                    eMPPModeType = ElanCommand_Gen8.MPPModeType.BHF_NoSync;
                    break;
                default:
                    eMPPModeType = ElanCommand_Gen8.MPPModeType.Normal;
                    break;
            }

            //Send MPP Mode Type Command to TP Device
            byteCommand_Array = ElanCommand_Gen8.ConvertMPPModeTypeToByteArray(eMPPModeType);

            string sActionMessage = string.Format("Set MPP Mode Type(Gen8)(Type:{0})", eMPPModeType.ToString());

            OutputMessage(string.Format("-{0}", sActionMessage));

            for (int nSetIndex = 0; nSetIndex < nSetCount; nSetIndex++)
            {
                Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);

                if (nSetIndex == nSetCount - 1)
                    Thread.Sleep(500);
                else
                    Thread.Sleep(50 * (nSetIndex + 1));
            }
        }

        // Set MPP Mode Section Function in Gen8(8F18)
        private void Set_MPP_Mode_Section_Gen8(RecordSetParameter cRecordSetParameter)
        {
            byte byteSection = 0x00;

            if (cRecordSetParameter.m_bTRxSAllScanFlag == true)
                byteSection = 0x30;
            else if (cRecordSetParameter.m_nSection == 0)
                byteSection = 0x00;
            else if (cRecordSetParameter.m_nSection == 1)
                byteSection = 0x01;
            else if (cRecordSetParameter.m_nSection == 2)
                byteSection = 0x02;
            else if (cRecordSetParameter.m_nSection == 3)
                byteSection = 0x03;
            else
                return;

            byte[] byteCommand_Array = new byte[] 
            { 
                0x54,
                0x77, 
                0x00, 
                byteSection 
            };

            string sActionMessage = "";
            string sSection = "";

            if (cRecordSetParameter.m_bTRxSAllScanFlag == true)
                sSection = "All Scan";
            else
                sSection = string.Format("S{0}", cRecordSetParameter.m_nSection);

            sActionMessage = string.Format("Set MPP Mode Section(Gen8)(Section:{0})", sSection);

            OutputMessage(string.Format("-{0}", sActionMessage));

            //Send MPP Mode Section Command to TP Device
            Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);
        }

        // Set MPP Mode Edge Shadow Option Function in Gen8(8F18)
        private void Set_MPP_Mode_Edge_Shadow_Option_Gen8(RecordSetParameter cRecordSetParameter)
        {
            byte byteOption = 0x20;

            if (cRecordSetParameter.m_eEdgeShadowOption == EdgeShadowOption.NOT_CHANGE)
                return;
            else if (cRecordSetParameter.m_eEdgeShadowOption == EdgeShadowOption.OFF)
                byteOption = 0x10;
            else if (cRecordSetParameter.m_eEdgeShadowOption == EdgeShadowOption.ON)
                byteOption = 0x20;
            else
                return;

            byte[] byteCommand_Array = new byte[] 
            { 
                0x54,
                0x77, 
                0x00, 
                byteOption 
            };

            string sActionMessage = string.Format("Set MPP Mode Edge Shadow Option(Gen8)(EdgeShadowOption:{0})", cRecordSetParameter.m_eEdgeShadowOption.ToString());

            OutputMessage(string.Format("-{0}", sActionMessage));

            //Send MPP Mode Edge Shadow Option Command to TP Device
            Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);
        }

        // Set MPP Set Parameter Function in Gen8(8F18)
        private void Set_MPP_Set_Parameter_Gen8(bool bEnableFlag)
        {
            byte[] byteCommand_Array;
            ElanCommand_Gen8.MPPSetParameter eMPPSetParameter;
            int nSetCount = 5;

            if (bEnableFlag == true)
                eMPPSetParameter = ElanCommand_Gen8.MPPSetParameter.Enable;
            else
                eMPPSetParameter = ElanCommand_Gen8.MPPSetParameter.Disable;

            //Send MPP Set Parameter Command to TP Device
            byteCommand_Array = ElanCommand_Gen8.ConvertMPPSetParameterToByteArray(eMPPSetParameter);

            string sActionMessage = "Set MPP Set Parameter(Gen8)";

            OutputMessage(string.Format("-{0}", sActionMessage));

            for (int nSetIndex = 0; nSetIndex < nSetCount; nSetIndex++)
            {
                Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);

                if (nSetIndex == nSetCount - 1)
                    Thread.Sleep(500);
                else
                    Thread.Sleep(50 * (nSetIndex + 1));
            }
        }

        // Set Normal Mode Function in Gen8(8F18)
        private void Set_Normal_Mode_Gen8()
        {
            int nSetCount = 5;

            //Send Normal Mode Command to TP Device
            byte[] byteCommand_Array = ElanCommand_Gen8.ConvertMPPModeTypeToByteArray(ElanCommand_Gen8.MPPModeType.Normal);

            string sActionMessage = "Set Normal Mode(Gen8)";

            OutputMessage(string.Format("-{0}", sActionMessage));

            for (int nSetIndex = 0; nSetIndex < nSetCount; nSetIndex++)
            {
                Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);

                Thread.Sleep(50 * (nSetIndex + 1));
            }
        }

        // Get AutoTune Version Function in Gen8(8F18)
        private int Get_AutoTune_Version_Gen8()
        {
            int nRetryCount = 3;
            int nAutoTuneVersion = -1;
            byte[] byteData_Array = new byte[65];

            byte[] byteCommand_Array = new byte[] 
            { 
                0x6F,
                0x00, 
                0x00, 
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00
            };

            string sActionMessage = "Get AutoTune Version(Gen8)";

            OutputMessage(string.Format("-{0}", sActionMessage));

            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);
                
                int nResult = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_cfrmMain.m_nDeviceIndex);

                if (nResult != ElanTouch.TP_SUCCESS)
                {
                    if (nResult == ElanTouch.TP_ERR_IO_PENDING)
                    {
                        OutputMessage(string.Format("-Get AutoTune Version No Response(RetryCount:{0})", nRetryIndex));
                    }
                    else
                    {
                        OutputMessage(string.Format("-Get AutoTune Version Error(RetryCount:{0})", nRetryIndex));

                        if (nRetryIndex == nRetryCount - 1)
                            nAutoTuneVersion = -2;
                    }
                }
                else
                {
                    if (byteData_Array[0] == 0x6F)
                    {
                        nAutoTuneVersion = (byteData_Array[6] << 24) + (byteData_Array[7] << 16) + (byteData_Array[8] << 8) + byteData_Array[9];
                        OutputMessage(string.Format("-Get AutoTune Version : {0}(Response : {1} {2} {3} {4} {5} {6} {7} {8} {9} {10})", 
                            nAutoTuneVersion,
                            byteData_Array[0].ToString("X2"),
                            byteData_Array[1].ToString("X2"),
                            byteData_Array[2].ToString("X2"),
                            byteData_Array[3].ToString("X2"),
                            byteData_Array[4].ToString("X2"),
                            byteData_Array[5].ToString("X2"),
                            byteData_Array[6].ToString("X2"),
                            byteData_Array[7].ToString("X2"),
                            byteData_Array[8].ToString("X2"),
                            byteData_Array[9].ToString("X2")));
                        break;
                    }
                }

                Thread.Sleep(100);
            }

            return nAutoTuneVersion;
        }

        // Set Project Option(Disable Idle Mode) Function in Gen8(8F18)
        private bool Set_Project_Option_Gen8()
        {
            if (ParamAutoTuning.m_nGen8ProjectOptionDisableValue == 0x0000)
                return true;

            bool bErrorFlag = true;

            int nOriginProjectOption = ElanTouch.GetProjOption(1000, m_cfrmMain.m_nDeviceIndex);
            OutputMessage(string.Format("-Get _Project_Option(Gen8)(Command : 53 B1 00 01) Value=0x{0}", nOriginProjectOption.ToString("X4")));

            short nSetProjectOption = (short)(nOriginProjectOption & 0xFFFF);

            int nEnableValue = ~ParamAutoTuning.m_nGen8ProjectOptionDisableValue & 0xFFFF;
            nSetProjectOption = (short)(nSetProjectOption & nEnableValue);

            if (nSetProjectOption != nOriginProjectOption)
            {
                for (int nRetryIndex = 0; nRetryIndex <= 3; nRetryIndex++)
                {
                    OutputMessage(string.Format("-Set _Project_Option(Coommand : 54 B1 {0} {1}) Value=0x{2}", ((nSetProjectOption & 0xFF00) >> 8).ToString("X2"), (nSetProjectOption & 0x00FF).ToString("X2"), nSetProjectOption.ToString("X4")));
                    ElanTouch.SetProjOption(nSetProjectOption, 1000, m_cfrmMain.m_nDeviceIndex);
                    Thread.Sleep(ParamAutoTuning.m_nGen8SendCommandDelayTime);

                    int nReadProjectOption = ElanTouch.GetProjOption(1000, m_cfrmMain.m_nDeviceIndex);
                    OutputMessage(string.Format("-Read _Project_Option(Gen8)(Command : 53 B1 00 01) Value=0x{0}", nReadProjectOption.ToString("X4")));

                    if (nReadProjectOption == nSetProjectOption)
                    {
                        bErrorFlag = false;
                        break;
                    }
                }
            }
            else
                bErrorFlag = false;

            if (bErrorFlag == true)
                return false;
            else
                return true;
        }

        // Set FWIP Option Function in Gen8(8F18)
        private bool Set_FWIP_Option_Gen8()
        {
            if (ParamAutoTuning.m_nGen8FWIPOptionDisableValue == 0x0000)
                return true;

            bool bErrorFlag = true;

            int nOriginFWIPOption = 0x0000;
            ElanTouch.GetFWIPOption(ref nOriginFWIPOption, 1000, m_cfrmMain.m_nDeviceIndex);
            OutputMessage(string.Format("-Get _FWIP_Option(Gen8)(Command : 53 C1 00 01) Value=0x{0}", nOriginFWIPOption.ToString("X4")));

            short nSetFWIPOption = (short)(nOriginFWIPOption & 0xFFFF);

            int nEnableValue = ~ParamAutoTuning.m_nGen8FWIPOptionDisableValue & 0xFFFF;
            nSetFWIPOption = (short)(nSetFWIPOption & nEnableValue);

            if (nSetFWIPOption != nOriginFWIPOption)
            {
                for (int nRetryIndex = 0; nRetryIndex <= 3; nRetryIndex++)
                {
                    OutputMessage(string.Format("-Set _FWIP_Option(Gen8)(Coommand : 54 C1 {0} {1}) Value=0x{2}", ((nSetFWIPOption & 0xFF00) >> 8).ToString("X2"), (nSetFWIPOption & 0x00FF).ToString("X2"), nSetFWIPOption.ToString("X4")));
                    ElanTouch.SetFWIPOption(nSetFWIPOption, 1000, m_cfrmMain.m_nDeviceIndex);
                    Thread.Sleep(ParamAutoTuning.m_nGen8SendCommandDelayTime);

                    int nReadFWIPOption = 0x0000;
                    ElanTouch.GetFWIPOption(ref nReadFWIPOption, 1000, m_cfrmMain.m_nDeviceIndex);
                    OutputMessage(string.Format("-Read _FWIP_Option(Gen8)(Command : 53 C1 00 01) Value=0x{0}", nReadFWIPOption.ToString("X4")));

                    if (nReadFWIPOption == nSetFWIPOption)
                    {
                        bErrorFlag = false;
                        break;
                    }
                }
            }
            else
                bErrorFlag = false;

            if (bErrorFlag == true)
                return false;
            else
                return true;
        }

        // Get MPP MT Mode Enable/Disable Function in Gen8(8F18)
        private int Get_MPP_MT_Mode_Enable_Gen8()
        {
            int nRetryCount = 3;
            byte[] byteData_Array = new byte[65];

            byte[] byteCommand_Array = new byte[] 
            { 
                0x53,
                0x74, 
                0x00, 
                0x00
            };

            string sActionMessage = "Get MPP MT Mode Enable(Gen8)(Command : 53 74 00 00)";

            OutputMessage(string.Format("-{0}", sActionMessage));

            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);

                int nResult = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_cfrmMain.m_nDeviceIndex);

                if (nResult != ElanTouch.TP_SUCCESS)
                {
                    if (nResult == ElanTouch.TP_ERR_IO_PENDING)
                    {
                        OutputMessage(string.Format("-Get MPP MT Mode No Response(RetryCount:{0})", nRetryIndex));
                    }
                    else
                    {
                        OutputMessage(string.Format("-Get MPP MT Mode Error(RetryCount:{0})", nRetryIndex));

                        if (nRetryIndex == nRetryCount - 1)
                            return -1;
                    }
                }
                else
                {
                    if (byteData_Array[0] == 0x52 && byteData_Array[1] == 0x74)
                    {
                        int nStateValue = -2;
                        string sStateText = "Unknown";

                        if (byteData_Array[3] == 0x01)
                        {
                            sStateText = "Disable";
                            nStateValue = 0;
                        }
                        else if (byteData_Array[3] == 0x02)
                        {
                            sStateText = "Enable";
                            nStateValue = 1;
                        }

                        OutputMessage(string.Format("-Get MPP MT Mode {0}(Response : {1} {2} {3} {4})", sStateText, byteData_Array[0].ToString("X2"), byteData_Array[1].ToString("X2"), byteData_Array[2].ToString("X2"), byteData_Array[3].ToString("X2")));
                        return nStateValue;
                    }
                }

                Thread.Sleep(100);
            }

            return -1;
        }

        // Get MPP MT Mode Status Function in Gen8(8F18)
        private bool Get_MPP_MT_Mode_Status_Gen8(ref byte byteStatusHighByte, ref byte byteStatusLowByte)
        {
            int nRetryCount = 3;
            byte[] byteData_Array = new byte[65];

            byte[] byteCommand_Array = new byte[] 
            { 
                0x53,
                0x73, 
                0x00, 
                0x00
            };

            string sActionMessage = "Get MPP MT Mode Status(Gen8)(Command : 53 73 00 00)";

            OutputMessage(string.Format("-{0}", sActionMessage));

            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);

                int nResult = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_cfrmMain.m_nDeviceIndex);

                if (nResult != ElanTouch.TP_SUCCESS)
                {
                    if (nResult == ElanTouch.TP_ERR_IO_PENDING)
                    {
                        OutputMessage(string.Format("-Get MPP MT Mode Status No Response(RetryCount:{0})", nRetryIndex));
                    }
                    else
                    {
                        OutputMessage(string.Format("-Get MPP MT Mode Status Error(RetryCount:{0})", nRetryIndex));

                        if (nRetryIndex == nRetryCount - 1)
                            return false;
                    }
                }
                else
                {
                    if (byteData_Array[0] == 0x52 && byteData_Array[1] == 0x73)
                    {
                        byteStatusHighByte = byteData_Array[2];
                        byteStatusLowByte = byteData_Array[3];

                        OutputMessage(string.Format("-Get MPP MT Mode Status({0} {1})(Response : {2} {3} {4} {5})", byteData_Array[2].ToString("X2"), byteData_Array[3].ToString("X2"), byteData_Array[0].ToString("X2"), byteData_Array[1].ToString("X2"), byteData_Array[2].ToString("X2"), byteData_Array[3].ToString("X2")));
                        return true;
                    }
                }

                Thread.Sleep(100);
            }

            return false;
        }
        #endregion

        #region Set FW Parameter Function
        /// <summary>
        /// 設定參數讀寫並確認是否相同
        /// </summary>
        /// <param name="cRecordSetParameter">此階段資訊</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool RunSendFWParameter(RecordSetParameter cRecordSetParameter)
        {
            if (cRecordSetParameter.m_bDisableSetParameterFlag == true)
                return true;

            MainTuningStep eMainStep = cRecordSetParameter.m_eMainStep;
            SubTuningStep eSubStep = cRecordSetParameter.m_eSubStep;

            OutputMessage("-Record Data Flow : Set FW Parameter");

            m_sSetFWParameter_List.Clear();

            const int nRetryLimitCount = 3;
            List<string> sFWParameter_List = new List<string>();

            if (ParamAutoTuning.m_nFWTypeIndex == 1)
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_FWCHECKVERSION);

            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_PH1);
            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_PH2);

            if (eMainStep == MainTuningStep.TILTNO)
            {
                if (eSubStep == SubTuningStep.TILTNO_PTHF)
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_NOISE_PTHF);
                else if (eSubStep == SubTuningStep.TILTNO_BHF)
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_NOISE_BHF);
            }

            if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO)
            {
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_REPORTNUMBER);
            }

            CheckState cCheckState = new CheckState(m_cfrmMain);

            if (cCheckState.CheckSetDigiGain(eMainStep, eSubStep) != CheckState.m_nSETDIGIGAIN_DISABLE)
            {
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_GETDIGIGAINCOMMAND);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_DIGIGAIN_P0);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_RX);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_TX);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_RX);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_TX);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_RX);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_TX);
            }

            if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                eMainStep == MainTuningStep.TPGAINTUNING ||
                (eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)) ||
                eMainStep == MainTuningStep.TILTTUNING ||
                eMainStep == MainTuningStep.PRESSURETUNING ||
                eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_P0_TH);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_TRXS_HOVER_TH_RX);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_TRXS_HOVER_TH_TX);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_TRXS_CONTACT_TH_RX);
                sFWParameter_List.Add(StringConvert.m_sCMDPARAM_TRXS_CONTACT_TH_TX);

                if (ParamAutoTuning.m_nFWTypeIndex != 1)
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_PEN_HI_HF_THD);

                if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                    eMainStep == MainTuningStep.TPGAINTUNING ||
                    (eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)))
                {
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_EDGE_1TRC_SUBPWR);
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_EDGE_2TRC_SUBPWR);
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_EDGE_3TRC_SUBPWR);
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_EDGE_4TRC_SUBPWR);
                }

                if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                    eMainStep == MainTuningStep.TPGAINTUNING ||
                    eMainStep == MainTuningStep.TILTTUNING ||
                    eMainStep == MainTuningStep.PRESSURETUNING ||
                    eMainStep == MainTuningStep.LINEARITYTUNING)
                {
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_HOVER_TH_RX);
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_HOVER_TH_TX);
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_CONTACT_TH_RX);
                    sFWParameter_List.Add(StringConvert.m_sCMDPARAM_CONTACT_TH_TX);

                    if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                        eMainStep == MainTuningStep.TPGAINTUNING ||
                        eMainStep == MainTuningStep.TILTTUNING)
                    {
                        if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                            eMainStep == MainTuningStep.TPGAINTUNING ||
                            (eMainStep == MainTuningStep.TILTTUNING && eSubStep == SubTuningStep.TILTTUNING_PTHF) ||
                            (eMainStep == MainTuningStep.LINEARITYTUNING && eSubStep == SubTuningStep.LINEARITYTABLE))
                        {
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_PTHF_CONTACT_TH_RX);
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_PTHF_CONTACT_TH_TX);
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_PTHF_HOVER_TH_RX);
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_PTHF_HOVER_TH_TX);
                        }

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                            eMainStep == MainTuningStep.TPGAINTUNING ||
                            (eMainStep == MainTuningStep.TILTTUNING && eSubStep == SubTuningStep.TILTTUNING_BHF) ||
                            (eMainStep == MainTuningStep.LINEARITYTUNING && eSubStep == SubTuningStep.LINEARITYTABLE))
                        {
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_BHF_CONTACT_TH_RX);
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_BHF_CONTACT_TH_TX);
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_BHF_HOVER_TH_RX);
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_BHF_HOVER_TH_TX);
                        }
                    }

                    if (eMainStep == MainTuningStep.PRESSURETUNING)
                    {
                        if (eSubStep == SubTuningStep.PRESSURESETTING)
                        {
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_IQ_BSH_P);
                        }
                        else if (eSubStep == SubTuningStep.PRESSURETABLE)
                        {
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_IQ_BSH_P);
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_PRESSURE3BINSTH);
                            sFWParameter_List.Add(StringConvert.m_sCMDPARAM_PRESS_3BINSPWR);
                        }
                    }
                }
            }

            m_sSetFWParameter_List = sFWParameter_List;

            string sErrorMessage = "";
            bool bSetParameterSuccessFlag = false;
            int nRetryIncreaseTime = 0;

            for (int nRetryIndex = 0; nRetryIndex <= nRetryLimitCount; nRetryIndex++)
            {
                int nResultFlag = 0;
                nRetryIncreaseTime = nRetryIndex * 100;

                if (nRetryIndex > 0)
                {
                    if (RunElanDeviceReConnect() == false)
                        break;
                }

                Send_Specific_Type_Command(MainConstantParameter.m_sSENDCOMMAND_SetRead_Bulk_RAM_Data);
                Thread.Sleep(500);

                Send_Specific_Type_Command(MainConstantParameter.m_sSENDCOMMAND_DISABLEREPORT);
                Thread.Sleep(500);

                if ((eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO) && m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                {
                    if (Set_Project_Option_Gen8() == false)
                    {
                        string sWarning_Message = "Set Project Option(Gen8) Error";
                        OutputMessage(string.Format("-Warning : {0}", sWarning_Message), true);
                    }

                    if (Set_FWIP_Option_Gen8() == false)
                    {
                        string sWarning_Message = "Set FWIP Option(Gen8) Error";
                        OutputMessage(string.Format("-Warning : {0}", sWarning_Message), true);
                    }

                    Set_MPP_Mode_Type_Gen8(cRecordSetParameter);
                    Thread.Sleep(nRetryIncreaseTime);

                    Set_Test_Mode(true);
                    Thread.Sleep(nRetryIncreaseTime);

                    Set_MPP_Mode_Section_Gen8(cRecordSetParameter);
                    Thread.Sleep(100 + nRetryIncreaseTime);

                    Set_MPP_Mode_Edge_Shadow_Option_Gen8(cRecordSetParameter);
                    Thread.Sleep(100 + nRetryIncreaseTime);

                    Set_MPP_Set_Parameter_Gen8(true);
                    Thread.Sleep(500 + nRetryIncreaseTime);

                    m_cElanCommand_Gen8.SetFlowStep(eMainStep, eSubStep);

                    if (eMainStep == MainTuningStep.NO)
                    {
                        if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                            m_cElanCommand_Gen8.SetMPPModeType(ElanCommand_Gen8.MPPModeType.RX_DFT);
                        else if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                            m_cElanCommand_Gen8.SetMPPModeType(ElanCommand_Gen8.MPPModeType.TX_DFT);
                    }
                    else if (eMainStep == MainTuningStep.TILTNO)
                    {
                        if (eSubStep == SubTuningStep.TILTNO_PTHF)
                            m_cElanCommand_Gen8.SetMPPModeType(ElanCommand_Gen8.MPPModeType.PTHF_NoSync);
                        else if (eSubStep == SubTuningStep.TILTNO_BHF)
                            m_cElanCommand_Gen8.SetMPPModeType(ElanCommand_Gen8.MPPModeType.BHF_NoSync);
                    }

                    if (m_cElanCommand_Gen8.RunReadOriginFWParameterFlow() == false)
                    {
                        sErrorMessage = m_cElanCommand_Gen8.GetErrorMessage();
                        nResultFlag = 1;
                    }

                    /*
                    Thread.Sleep(1000);

                    if (m_cElanCommand_Gen8.CheckEnterTestMode() == false)
                    {
                        sErrorMessage = "Enter Test Mode Error";
                        nResultFlag = 1;
                        continue;
                    }
                    */

                    if (nResultFlag == 0)
                    {
                        if (ParamAutoTuning.m_nGen8CommandScriptType != 2)
                        {
                            string sPenScanState = PenScanState.m_sPENSCANSTATE_0_Rx;
                            int nTraceNumber = ParamAutoTuning.m_nGen8RealTraceNumber;

                            if (eMainStep == MainTuningStep.NO)
                            {
                                /*
                                if (ParamAutoTuning.m_nGen8TraceType == 0)
                                    sPenScanState = PenScanState.m_sPENSCANSTATE_0_Rx;
                                else if (ParamAutoTuning.m_nGen8TraceType == 1)
                                    sPenScanState = PenScanState.m_sPENSCANSTATE_0_Tx;
                                */

                                if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                                {
                                    sPenScanState = PenScanState.m_sPENSCANSTATE_0_Rx;
                                    nTraceNumber = m_nRXTraceNumber;
                                }
                                else if (cRecordSetParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                                {
                                    sPenScanState = PenScanState.m_sPENSCANSTATE_0_Tx;
                                    nTraceNumber = m_nTXTraceNumber;
                                }

                                if (ParamAutoTuning.m_nNoiseDataType == 1)
                                    nTraceNumber = ParamAutoTuning.m_nGen8RealTraceNumber;
                            }
                            else if (eMainStep == MainTuningStep.TILTNO)
                            {
                                nTraceNumber = m_nRXTraceNumber;

                                if (eSubStep == SubTuningStep.TILTNO_PTHF)
                                    sPenScanState = PenScanState.m_sPENSCANSTATE_0_PTHF;
                                else if (eSubStep == SubTuningStep.TILTNO_BHF)
                                    sPenScanState = PenScanState.m_sPENSCANSTATE_0_BHF;
                            }

                            m_cElanCommand_Gen8.SetTraceNumber(nTraceNumber);
                            m_cElanCommand_Gen8.RunSetSampleFrequencyFlow(sPenScanState, cRecordSetParameter.m_nPH1, cRecordSetParameter.m_nPH2, nRetryIncreaseTime);

                            m_cElanCommand_Gen8.RunSetFWOptionFlow();
                        }

                        if (ParamAutoTuning.m_nGen8CommandScriptType != 2)
                        {
                            if (ParamAutoTuning.m_nGen8GetAnalogAfterSwitchTestMode == 1)
                            {
                                Set_Test_Mode(false);
                                Set_Test_Mode(true);
                            }
                            else
                                Thread.Sleep(1000);

                            if (m_cElanCommand_Gen8.RunCheckSampleFrequencyFlow(m_cCurrentParameterSet) == false)
                            {
                                sErrorMessage = m_cElanCommand_Gen8.GetErrorMessage();
                                nResultFlag = 1;
                            }

                            /*
                            if (m_cElanCommand_Gen8.RunCheckFWOptionFlow() == false)
                            {
                                sErrorMessage = m_cElanCommand_Gen8.GetErrorMessage();
                                nResultFlag = 1;
                            }
                            */

                            m_cCurrentParameterSet.m_nReadReportNumber = m_nNoiseReportNumber;
                        }
                    }

                    if (nResultFlag == 0 && (ParamAutoTuning.m_nGen8CommandScriptType == 1 || ParamAutoTuning.m_nGen8CommandScriptType == 2))
                    {
                        if (m_cSendCommandInfo != null)
                            m_cElanCommand_Gen8.RunUserDefinedCommandScriptFlow(m_cSendCommandInfo);
                    }

                    Set_Test_Mode(false);
                    Thread.Sleep(nRetryIncreaseTime);
                }
                else
                {
                    for (int nParameterIndex = 0; nParameterIndex < sFWParameter_List.Count; nParameterIndex++)
                    {
                        if (m_nDigiGainCommandState == m_nDIGIGAINCOMMAND_DISABLE)
                        {
                            if (m_sDigiGainParameter_Array.Contains(sFWParameter_List[nParameterIndex]) == true)
                                continue;
                        }

                        nResultFlag = SetAndCheckFWParameter(ref sErrorMessage, sFWParameter_List[nParameterIndex], cRecordSetParameter);

                        if (nResultFlag != 0)
                            break;
                    }
                }

                if (nResultFlag == 1)
                    continue;
                else if (nResultFlag == 2)
                    break;

                bSetParameterSuccessFlag = true;
                break;
            }

            if (bSetParameterSuccessFlag == false)
            {
                m_sErrorMessage = sErrorMessage;
                m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
                return false;
            }
            else
            {
                if (ParamAutoTuning.m_nEnableReK == 1)
                {
                    OutputMessage("-Set ReK");

                    int nResultFlag = ElanTouch.ReK(5000, false, m_cfrmMain.m_nDeviceIndex);

                    if (nResultFlag != ElanTouch.TP_SUCCESS)
                    {
                        m_sErrorMessage = "ReK Fail";
                        m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                        m_cfrmMain.m_bInterruptFlag = true;
                        Thread.Sleep(10);
                        return false;
                    }

                    Thread.Sleep(500);
                }
            }

            return true;
        }

        private bool RunSendDefaultFWParameter(RecordSetParameter cRecordSetParameter)
        {
            if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                return true;

            if (m_bSetDefaultFWParameterFlag == false)
                return true;

            if (cRecordSetParameter.m_bDisableSetParameterFlag == true)
                return true;

            OutputMessage("-Record Data Flow : Set Default FW Parameter");

            MainTuningStep eMainStep = cRecordSetParameter.m_eMainStep;
            SubTuningStep eSubStep = cRecordSetParameter.m_eSubStep;

            CheckState cCheckState = new CheckState(m_cfrmMain);

            if (cCheckState.CheckIndependentStep(eMainStep, eSubStep) != CheckState.m_nSTEPSTATE_NORMAL)
                return true;

            const int nRetryLimitCount = 3;

            m_sDefaultFWParameter_List.Clear();

            m_sDefaultFWParameter_List = new List<string>() 
            { 
                //StringConvert.m_sCMDPARAM_AP_PPEAKTHRDSHOLD,
                StringConvert.m_sCMDPARAM_EDGE_1TRC_SUBPWR,
                StringConvert.m_sCMDPARAM_EDGE_2TRC_SUBPWR,
                StringConvert.m_sCMDPARAM_EDGE_3TRC_SUBPWR,
                StringConvert.m_sCMDPARAM_EDGE_4TRC_SUBPWR,
                StringConvert.m_sCMDPARAM_DIGIGAIN_P0,
                StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_RX,
                StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_TX,
                StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_RX,
                StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_TX,
                StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_RX,
                StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_TX,
                StringConvert.m_sCMDPARAM_P0_TH,
                StringConvert.m_sCMDPARAM_TRXS_CONTACT_TH_RX,
                StringConvert.m_sCMDPARAM_TRXS_CONTACT_TH_TX,
                StringConvert.m_sCMDPARAM_TRXS_HOVER_TH_RX,
                StringConvert.m_sCMDPARAM_CONTACT_TH_RX,
                StringConvert.m_sCMDPARAM_CONTACT_TH_TX,
                StringConvert.m_sCMDPARAM_PTHF_CONTACT_TH_RX,
                StringConvert.m_sCMDPARAM_PTHF_CONTACT_TH_TX,
                StringConvert.m_sCMDPARAM_PTHF_HOVER_TH_RX,
                StringConvert.m_sCMDPARAM_PTHF_HOVER_TH_TX,
                StringConvert.m_sCMDPARAM_BHF_CONTACT_TH_RX,
                StringConvert.m_sCMDPARAM_BHF_CONTACT_TH_TX,
                StringConvert.m_sCMDPARAM_BHF_HOVER_TH_RX,
                StringConvert.m_sCMDPARAM_BHF_HOVER_TH_TX 
            };

            if (ParamAutoTuning.m_nFWTypeIndex != 1)
                m_sDefaultFWParameter_List.Add(StringConvert.m_sCMDPARAM_PEN_HI_HF_THD);

            string sErrorMessage = "";
            bool bSetParameterSuccessFlag = false;

            for (int nRetryIndex = 0; nRetryIndex <= nRetryLimitCount; nRetryIndex++)
            {
                int nResultFlag = 0;

                if (nRetryIndex > 0)
                {
                    if (RunElanDeviceReConnect() == false)
                        break;
                }

                for (int nParameterIndex = 0; nParameterIndex < m_sDefaultFWParameter_List.Count; nParameterIndex++)
                {
                    if (m_sSetFWParameter_List.Contains(m_sDefaultFWParameter_List[nParameterIndex]) == true)
                        continue;

                    if (m_nDigiGainCommandState == m_nDIGIGAINCOMMAND_DISABLE)
                    {
                        if (m_sDigiGainParameter_Array.Contains(m_sDefaultFWParameter_List[nParameterIndex]) == true)
                            continue;
                    }

                    nResultFlag = SetAndCheckFWParameter(ref sErrorMessage, m_sDefaultFWParameter_List[nParameterIndex], cRecordSetParameter, true);

                    if (nResultFlag != 0)
                        break;
                }

                if (nResultFlag == 1)
                    continue;
                else if (nResultFlag == 2)
                    break;

                bSetParameterSuccessFlag = true;
                break;
            }

            if (bSetParameterSuccessFlag == false)
            {
                m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
                return false;
            }

            m_bSetDefaultFWParameterFlag = false;
            return true;
        }

        /// <summary>
        /// 確認讀寫的參數是否相同
        /// </summary>
        /// <param name="sErrorMessage">錯誤的描述字串</param>
        /// <param name="sFWParameter">Command參數名稱</param>
        /// <param name="cStageInfoParam">此階段資訊</param>
        /// <param name="bSetDefaultFWParameterFlag">是否為設定預設FW參數值</param>
        /// <returns>回傳比較結果的Flag值</returns>
        private int SetAndCheckFWParameter(ref string sErrorMessage, string sFWParameter, RecordSetParameter cRecordSetParameter, bool bSetDefaultFWParameterFlag = false)
        {
            int nResultFlag = 0;

            ElanCommand.ICValueTargetType eCommandType = ElanCommand.ICValueTargetType.NA;
            int nSetValue = 0;

            SetCommandTypeAndValue(ref eCommandType, ref nSetValue, sFWParameter, cRecordSetParameter, bSetDefaultFWParameterFlag);

            if (bSetDefaultFWParameterFlag == false)
            {
                if (sFWParameter == StringConvert.m_sCMDPARAM_FWCHECKVERSION)
                {
                    int nGetValue = GetAndCheckFWCheckVersion();
                    SetGetValue(sFWParameter, nGetValue);

                    if (m_cCurrentParameterSet.m_nFWCheckVersion == -1)
                    {
                        sErrorMessage = string.Format("Can't Get {0}", sFWParameter);
                        nResultFlag = 1;
                    }
                    else if (m_cCurrentParameterSet.m_nFWCheckVersion != ParamAutoTuning.m_nFWCheckVersion)
                    {
                        string sFWCheckVersion = nGetValue.ToString("x2").PadLeft(2, '0').ToUpper();
                        sErrorMessage = string.Format("No Support This FW.Please Check the FW! [{0}]", sFWCheckVersion);
                        nResultFlag = 2;
                    }
                }
                else if (cRecordSetParameter.m_eSubStep == SubTuningStep.PRESSURESETTING && m_cCurrentParameterSet.m_nROrgIQ_BSH_P < 0 && sFWParameter == StringConvert.m_sCMDPARAM_IQ_BSH_P)
                {
                    int nGetValue = GetFWValue(eCommandType);
                    SetGetValue(sFWParameter, nGetValue);

                    if (m_cCurrentParameterSet.m_nRIQ_BSH_P == -1)
                    {
                        sErrorMessage = string.Format("Can't Get {0}", sFWParameter);
                        nResultFlag = 1;
                    }
                    else if (m_cCurrentParameterSet.m_nRIQ_BSH_P < ParamAutoTuning.m_nPTIQ_BSH_P_LB || m_cCurrentParameterSet.m_nRIQ_BSH_P > ParamAutoTuning.m_nPTIQ_BSH_P_HB)
                    {
                        string sRIQ_BSH_P = Convert.ToString(m_cCurrentParameterSet.m_nRIQ_BSH_P).PadLeft(2, '0');
                        sErrorMessage = string.Format("Get {0} Value Over Range! [{1}]", sFWParameter, sRIQ_BSH_P);
                        nResultFlag = 2;
                    }

                    if (nResultFlag == 0)
                    {
                        m_cCurrentParameterSet.m_nSIQ_BSH_P = m_cCurrentParameterSet.m_nRIQ_BSH_P;
                        m_cCurrentParameterSet.m_nROrgIQ_BSH_P = m_cCurrentParameterSet.m_nRIQ_BSH_P;
                    }
                }
                else if (sFWParameter == StringConvert.m_sCMDPARAM_NOISE_PTHF || sFWParameter == StringConvert.m_sCMDPARAM_NOISE_BHF)
                {
                    if (sFWParameter == StringConvert.m_sCMDPARAM_NOISE_PTHF)
                        SetFWValueBySendCommand(ElanCommand.ElanCommandType.SetNoisePTHF, sFWParameter);
                    else if (sFWParameter == StringConvert.m_sCMDPARAM_NOISE_BHF)
                        SetFWValueBySendCommand(ElanCommand.ElanCommandType.SetNoiseBHF, sFWParameter);
                }
                else
                {
                    int nGetValue = -1;
                    bool bCompareFlag = true;
                    CheckState cCheckState = new CheckState(m_cfrmMain);
                    int nStepState = cCheckState.CheckIndependentStep(cRecordSetParameter.m_eMainStep, cRecordSetParameter.m_eSubStep);

                    if (nStepState == CheckState.m_nSTEPSTATE_PRESSURETABLE)
                    {
                        string[] sSetFWParameter_Array = new string[] 
                        { 
                            StringConvert.m_sCMDPARAM_IQ_BSH_P,
                            StringConvert.m_sCMDPARAM_PRESS_3BINSPWR,
                            StringConvert.m_sCMDPARAM_PRESSURE3BINSTH 
                        };

                        if (sSetFWParameter_Array.Contains(sFWParameter) == true)
                        {
                            nGetValue = SetAndGetICValue(eCommandType, nSetValue);
                            bCompareFlag = true;
                        }
                    }

                    if (nStepState != CheckState.m_nSTEPSTATE_NORMAL)
                    {
                        string[] sFWParameter_Array = new string[] 
                        { 
                            StringConvert.m_sCMDPARAM_PH1,
                            StringConvert.m_sCMDPARAM_PH2 
                        };

                        if (sFWParameter == StringConvert.m_sCMDPARAM_GETDIGIGAINCOMMAND)
                        {
                            if (m_nDigiGainCommandState != m_nDIGIGAINCOMMAND_RESET)
                                return nResultFlag;

                            bool bGetCorrectACKFlag = GetAndCheckFWValueBySendCommand(eCommandType, 16384, true);

                            CheckSetDigiGainCommandState(ref nResultFlag, ref sErrorMessage, bGetCorrectACKFlag, cRecordSetParameter, true);
                            return nResultFlag;
                        }
                        else if (sFWParameter == StringConvert.m_sCMDPARAM_REPORTNUMBER)
                        {
                            nGetValue = SetAndGetICValue(eCommandType, nSetValue);
                        }
                        else
                        {
                            nGetValue = GetFWValue(eCommandType);
                            bCompareFlag = sFWParameter_Array.Contains(sFWParameter);
                        }
                    }
                    else
                    {
                        if (sFWParameter == StringConvert.m_sCMDPARAM_GETDIGIGAINCOMMAND)
                        {
                            if (m_nDigiGainCommandState != m_nDIGIGAINCOMMAND_RESET)
                                return nResultFlag;

                            nSetValue = 16384;
                            bool bGetCorrectACKFlag = GetAndCheckFWValueBySendCommand(eCommandType, nSetValue);

                            CheckSetDigiGainCommandState(ref nResultFlag, ref sErrorMessage, bGetCorrectACKFlag, cRecordSetParameter);
                            return nResultFlag;
                        }
                        else
                        {
                            switch (ParamAutoTuning.m_nSendFWParamType)
                            {
                                case 1:
                                    nGetValue = GetFWValue(eCommandType);
                                    bCompareFlag = false;
                                    break;
                                case 2:
                                    if (sFWParameter == StringConvert.m_sCMDPARAM_PH1 || sFWParameter == StringConvert.m_sCMDPARAM_PH2)
                                    {
                                        nGetValue = GetFWValue(eCommandType);
                                        bCompareFlag = false;
                                    }
                                    else
                                        nGetValue = SetAndGetICValue(eCommandType, nSetValue);

                                    break;
                                case 3:
                                    if (m_sDigiGainParameter_Array.Contains(sFWParameter) == true)
                                    {
                                        nGetValue = GetFWValue(eCommandType);
                                        bCompareFlag = false;
                                    }
                                    else
                                        nGetValue = SetAndGetICValue(eCommandType, nSetValue);

                                    break;
                                default:
                                    nGetValue = SetAndGetICValue(eCommandType, nSetValue);
                                    break;
                            }
                        }
                    }

                    SetGetValue(sFWParameter, nGetValue);

                    if (bCompareFlag == true)
                        nResultFlag = CompareFWParameter(ref sErrorMessage, nGetValue, nSetValue, sFWParameter);
                }
            }
            else
            {
                int nGetValue = -1;

                nGetValue = SetAndGetICValue(eCommandType, nSetValue);
                nResultFlag = CompareFWParameter(ref sErrorMessage, nGetValue, nSetValue, sFWParameter);
            }

            return nResultFlag;
        }

        private void CheckSetDigiGainCommandState(ref int nResultFlag, ref string sErrorMessage, bool bGetCorrectACKFlag, RecordSetParameter cRecordSetParameter, bool bOnlyGetACKFlag = false)
        {
            string sState = "";

            if (bGetCorrectACKFlag == true)
            {
                sState = "Success";
                m_nDigiGainCommandState = m_nDIGIGAINCOMMAND_ENABLE;
            }
            else
            {
                sState = "Fail";
                m_nDigiGainCommandState = m_nDIGIGAINCOMMAND_DISABLE;

                if (cRecordSetParameter.m_eMainStep == MainTuningStep.DIGIGAINTUNING)
                {
                    if (bOnlyGetACKFlag == false)
                        sErrorMessage = "Set DigiGain Command Fail";
                    else
                        sErrorMessage = "Get DigiGain Command Fail";

                    nResultFlag = 2;
                }
            }

            if (bOnlyGetACKFlag == false)
                OutputMessage(string.Format("-Set DigiGain Command {0}", sState));
            else
                OutputMessage(string.Format("-Get DigiGain Command {0}", sState));
        }

        /// <summary>
        /// 比較TP參數設定值與讀取值是否相同
        /// </summary>
        /// <param name="sErrorMessage">錯誤訊息</param>
        /// <param name="nReadValue">讀取值</param>
        /// <param name="nSetValue">設定值</param>
        /// <param name="sParameterName">參數名稱</param>
        /// <returns>回傳比較的Flag</returns>
        private int CompareFWParameter(ref string sErrorMessage, int nReadValue, int nSetValue, string sParameterName)
        {
            if (nReadValue == -1 || nReadValue != nSetValue)
            {
                string sReadValue = Convert.ToString(nReadValue).PadLeft(2, '0');
                sErrorMessage = string.Format("Set {0} Value Error[{1}]", sParameterName, sReadValue);

                if (nReadValue == -1)
                    return 1;
                else
                    return 2;
            }

            return 0;
        }

        /// <summary>
        /// 設定Command種類以及設定值
        /// </summary>
        /// <param name="eCommandType">Command種類</param>
        /// <param name="nSetValue">Command設定值</param>
        /// <param name="sFWParameter">Command參數名稱</param>
        /// <param name="cRecordSetParameter">此階段資訊</param>
        /// <param name="bSetDefaultFWParameterFlag">是否為設定FW預設值</param>
        private void SetCommandTypeAndValue(ref ElanCommand.ICValueTargetType eCommandType,
                                            ref int nSetValue,
                                            string sFWParameter,
                                            RecordSetParameter cRecordSetParameter,
                                            bool bSetDefaultFWParameterFlag = false)
        {
            MainTuningStep eMainStep = cRecordSetParameter.m_eMainStep;
            SubTuningStep eSubStep = cRecordSetParameter.m_eSubStep;

            if (bSetDefaultFWParameterFlag == false)
            {
                CheckState cCheckState = new CheckState(m_cfrmMain);

                int nSetDigiGainState = cCheckState.CheckSetDigiGain(eMainStep, eSubStep);

                switch (sFWParameter)
                {
                    case StringConvert.m_sCMDPARAM_FWCHECKVERSION:
                        break;
                    case StringConvert.m_sCMDPARAM_PH1:
                        eCommandType = ElanCommand.ICValueTargetType.PH1;
                        nSetValue = cRecordSetParameter.m_nPH1;
                        break;
                    case StringConvert.m_sCMDPARAM_PH2:
                        eCommandType = ElanCommand.ICValueTargetType.PH2;
                        nSetValue = cRecordSetParameter.m_nPH2;
                        break;
                    case StringConvert.m_sCMDPARAM_NOISE_PTHF:
                    case StringConvert.m_sCMDPARAM_NOISE_BHF:
                        break;
                    case StringConvert.m_sCMDPARAM_REPORTNUMBER:
                        eCommandType = ElanCommand.ICValueTargetType.ReportNumber;
                        nSetValue = m_nNoiseReportNumber;
                        break;
                    case StringConvert.m_sCMDPARAM_P0_TH:
                        eCommandType = ElanCommand.ICValueTargetType.P0_TH;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingcActivePen_FM_P0_TH;
                        else
                        {
                            //nSetValue = m_cCurrentParameterSet.m_nSettingcActivePen_FM_P0_TH;
                            nSetValue = cRecordSetParameter.m_ncActivePen_FM_P0_TH;
                        }

                        if (ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH >= 0)
                            nSetValue = ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH;

                        break;
                    case StringConvert.m_sCMDPARAM_TRXS_HOVER_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.TRxS_Hover_TH_Rx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingTRxS_Beacon_Hover_TH_Rx;
                        else
                            nSetValue = cRecordSetParameter.m_nTRxS_Beacon_Hover_TH_Rx;

                        break;
                    case StringConvert.m_sCMDPARAM_TRXS_HOVER_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.TRxS_Hover_TH_Tx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingTRxS_Beacon_Hover_TH_Tx;
                        else
                            nSetValue = cRecordSetParameter.m_nTRxS_Beacon_Hover_TH_Tx;

                        break;
                    case StringConvert.m_sCMDPARAM_TRXS_CONTACT_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.TRxS_Contact_TH_Rx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingTRxS_Beacon_Contact_TH_Rx;
                        else
                            nSetValue = cRecordSetParameter.m_nTRxS_Beacon_Contact_TH_Rx;

                        break;
                    case StringConvert.m_sCMDPARAM_TRXS_CONTACT_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.TRxS_Contact_TH_Tx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingTRxS_Beacon_Contact_TH_Tx;
                        else
                            nSetValue = cRecordSetParameter.m_nTRxS_Beacon_Contact_TH_Tx;

                        break;
                    case StringConvert.m_sCMDPARAM_EDGE_1TRC_SUBPWR:
                        eCommandType = ElanCommand.ICValueTargetType.Edge_1Trc_SubPwr;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTrcMaxMinusPreP0_TH_1Trc;
                            nSetValue = m_cCurrentParameterSet.m_nSEdge_1Trc_SubPwr;
                        else
                            nSetValue = cRecordSetParameter.m_nEdge_1Trc_SubPwr;

                        break;
                    case StringConvert.m_sCMDPARAM_EDGE_2TRC_SUBPWR:
                        eCommandType = ElanCommand.ICValueTargetType.Edge_2Trc_SubPwr;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTrcMaxMinusPreP0_TH_2Trc;
                            nSetValue = m_cCurrentParameterSet.m_nSEdge_2Trc_SubPwr;
                        else
                            nSetValue = cRecordSetParameter.m_nEdge_2Trc_SubPwr;

                        break;
                    case StringConvert.m_sCMDPARAM_EDGE_3TRC_SUBPWR:
                        eCommandType = ElanCommand.ICValueTargetType.Edge_3Trc_SubPwr;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTrcMaxMinusPreP0_TH_3Trc;
                            nSetValue = m_cCurrentParameterSet.m_nSEdge_3Trc_SubPwr;
                        else
                            nSetValue = cRecordSetParameter.m_nEdge_3Trc_SubPwr;

                        break;
                    case StringConvert.m_sCMDPARAM_EDGE_4TRC_SUBPWR:
                        eCommandType = ElanCommand.ICValueTargetType.Edge_4Trc_SubPwr;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTrcMaxMinusPreP0_TH_4Trc;
                            nSetValue = m_cCurrentParameterSet.m_nSEdge_4Trc_SubPwr;
                        else
                            nSetValue = cRecordSetParameter.m_nEdge_4Trc_SubPwr;

                        break;
                    case StringConvert.m_sCMDPARAM_HOVER_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.Hover_TH_Rx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingBeacon_Hover_TH_Rx;
                        else
                            nSetValue = cRecordSetParameter.m_nBeacon_Hover_TH_Rx;

                        break;
                    case StringConvert.m_sCMDPARAM_HOVER_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.Hover_TH_Tx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingBeacon_Hover_TH_Tx;
                        else
                            nSetValue = cRecordSetParameter.m_nBeacon_Hover_TH_Tx;

                        break;
                    case StringConvert.m_sCMDPARAM_CONTACT_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.Contact_TH_Rx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingBeacon_Contact_TH_Rx;
                        else
                            nSetValue = cRecordSetParameter.m_nBeacon_Contact_TH_Rx;

                        break;
                    case StringConvert.m_sCMDPARAM_CONTACT_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.Contact_TH_Tx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingBeacon_Contact_TH_Tx;
                        else
                            nSetValue = cRecordSetParameter.m_nBeacon_Contact_TH_Tx;

                        break;
                    case StringConvert.m_sCMDPARAM_PTHF_CONTACT_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.PTHF_Contact_TH_Rx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingPTHF_Contact_TH_Rx;
                        else
                            nSetValue = cRecordSetParameter.m_nPTHF_Contact_TH_Rx;

                        break;
                    case StringConvert.m_sCMDPARAM_PTHF_CONTACT_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.PTHF_Contact_TH_Tx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingPTHF_Contact_TH_Tx;
                        else
                            nSetValue = cRecordSetParameter.m_nPTHF_Contact_TH_Tx;

                        break;
                    case StringConvert.m_sCMDPARAM_PTHF_HOVER_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.PTHF_Hover_TH_Rx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingPTHF_Hover_TH_Rx;
                        else
                            nSetValue = cRecordSetParameter.m_nPTHF_Hover_TH_Rx;

                        break;
                    case StringConvert.m_sCMDPARAM_PTHF_HOVER_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.PTHF_Hover_TH_Tx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingPTHF_Hover_TH_Tx;
                        else
                            nSetValue = cRecordSetParameter.m_nPTHF_Hover_TH_Tx;

                        break;
                    case StringConvert.m_sCMDPARAM_BHF_CONTACT_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.BHF_Contact_TH_Rx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingBHF_Contact_TH_Rx;
                        else
                            nSetValue = cRecordSetParameter.m_nBHF_Contact_TH_Rx;

                        break;
                    case StringConvert.m_sCMDPARAM_BHF_CONTACT_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.BHF_Contact_TH_Tx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingBHF_Contact_TH_Tx;
                        else
                            nSetValue = cRecordSetParameter.m_nBHF_Contact_TH_Tx;

                        break;
                    case StringConvert.m_sCMDPARAM_BHF_HOVER_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.BHF_Hover_TH_Rx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingBHF_Hover_TH_Rx;
                        else
                            nSetValue = cRecordSetParameter.m_nBHF_Hover_TH_Rx;

                        break;
                    case StringConvert.m_sCMDPARAM_BHF_HOVER_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.BHF_Hover_TH_Tx;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseTXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSettingBHF_Hover_TH_Tx;
                        else
                            nSetValue = cRecordSetParameter.m_nBHF_Hover_TH_Tx;

                        break;
                    case StringConvert.m_sCMDPARAM_IQ_BSH_P:
                        eCommandType = ElanCommand.ICValueTargetType.IQ_BSH_P;

                        if (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURESETTING && m_cCurrentParameterSet.m_nROrgIQ_BSH_P >= 0)
                            nSetValue = m_cCurrentParameterSet.m_nSIQ_BSH_P;
                        else
                            nSetValue = cRecordSetParameter.m_nIQ_BSH_P;

                        break;
                    case StringConvert.m_sCMDPARAM_PRESSURE3BINSTH:
                        eCommandType = ElanCommand.ICValueTargetType.Pressure3BinsTH;
                        nSetValue = cRecordSetParameter.m_nPressure3BinsTH;
                        break;
                    case StringConvert.m_sCMDPARAM_PRESS_3BINSPWR:
                        eCommandType = ElanCommand.ICValueTargetType.Press_3BinsPwr;
                        nSetValue = cRecordSetParameter.m_nPress_3BinsPwr;
                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_P0:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_P0;

                        if (nSetDigiGainState == CheckState.m_nSETDIGIGAIN_FIXEDVALUE ||
                            (nSetDigiGainState != CheckState.m_nSETDIGIGAIN_FIXEDVALUE && m_bGetStepInfoFlag_DigiGainTuning == false))
                            nSetValue = m_cCurrentParameterSet.m_nSDigiGain_P0;
                        else
                            nSetValue = ElanConvert.ConvertScaleToDigiGain(cRecordSetParameter.m_nDigiGain_P0);

                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_RX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_Beacon_Rx;

                        if (nSetDigiGainState == CheckState.m_nSETDIGIGAIN_FIXEDVALUE ||
                            (nSetDigiGainState != CheckState.m_nSETDIGIGAIN_FIXEDVALUE && m_bGetStepInfoFlag_DigiGainTuning == false))
                            nSetValue = m_cCurrentParameterSet.m_nSDigiGain_Beacon_Rx;
                        else
                            nSetValue = ElanConvert.ConvertScaleToDigiGain(cRecordSetParameter.m_nDigiGain_Beacon_Rx);

                        break;
                    case StringConvert.m_sCMDPARAM_GETDIGIGAINCOMMAND:
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_TX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_Beacon_Tx;

                        if (nSetDigiGainState == CheckState.m_nSETDIGIGAIN_FIXEDVALUE ||
                            (nSetDigiGainState != CheckState.m_nSETDIGIGAIN_FIXEDVALUE && m_bGetStepInfoFlag_DigiGainTuning == false))
                            nSetValue = m_cCurrentParameterSet.m_nSDigiGain_Beacon_Tx;
                        else
                            nSetValue = ElanConvert.ConvertScaleToDigiGain(cRecordSetParameter.m_nDigiGain_Beacon_Tx);

                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_RX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_PTHF_Rx;

                        if (nSetDigiGainState == CheckState.m_nSETDIGIGAIN_FIXEDVALUE ||
                            (nSetDigiGainState != CheckState.m_nSETDIGIGAIN_FIXEDVALUE && m_bGetStepInfoFlag_DigiGainTuning == false))
                            nSetValue = m_cCurrentParameterSet.m_nSDigiGain_PTHF_Rx;
                        else
                            nSetValue = ElanConvert.ConvertScaleToDigiGain(cRecordSetParameter.m_nDigiGain_PTHF_Rx);

                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_TX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_PTHF_Tx;

                        if (nSetDigiGainState == CheckState.m_nSETDIGIGAIN_FIXEDVALUE ||
                            (nSetDigiGainState != CheckState.m_nSETDIGIGAIN_FIXEDVALUE && m_bGetStepInfoFlag_DigiGainTuning == false))
                            nSetValue = m_cCurrentParameterSet.m_nSDigiGain_PTHF_Tx;
                        else
                            nSetValue = ElanConvert.ConvertScaleToDigiGain(cRecordSetParameter.m_nDigiGain_PTHF_Tx);

                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_RX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_BHF_Rx;

                        if (nSetDigiGainState == CheckState.m_nSETDIGIGAIN_FIXEDVALUE ||
                            (nSetDigiGainState != CheckState.m_nSETDIGIGAIN_FIXEDVALUE && m_bGetStepInfoFlag_DigiGainTuning == false))
                            nSetValue = m_cCurrentParameterSet.m_nSDigiGain_BHF_Rx;
                        else
                            nSetValue = ElanConvert.ConvertScaleToDigiGain(cRecordSetParameter.m_nDigiGain_BHF_Rx);

                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_TX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_BHF_Tx;

                        if (nSetDigiGainState == CheckState.m_nSETDIGIGAIN_FIXEDVALUE ||
                            (nSetDigiGainState != CheckState.m_nSETDIGIGAIN_FIXEDVALUE && m_bGetStepInfoFlag_DigiGainTuning == false))
                            nSetValue = m_cCurrentParameterSet.m_nSDigiGain_BHF_Tx;
                        else
                            nSetValue = ElanConvert.ConvertScaleToDigiGain(cRecordSetParameter.m_nDigiGain_BHF_Tx);

                        break;
                    case StringConvert.m_sCMDPARAM_PEN_HI_HF_THD:
                        eCommandType = ElanCommand.ICValueTargetType.Param_Pen_HI_HF_THD;

                        if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING)
                        {
                            //nSetValue = m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD;
                            nSetValue = m_cCurrentParameterSet.m_nSPen_HI_HF_THD;
                        }
                        else
                        {
                            //nSetValue = m_cCurrentParameterSet.m_nSettingcActivePen_FM_P0_TH;
                            nSetValue = cRecordSetParameter.m_ncActivePen_FM_P0_TH;
                        }

                        if (ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH >= 0)
                            nSetValue = ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH;

                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (sFWParameter)
                {
                    /*
                    case StringConvert.m_sCMDPARAM_AP_PPEAKTHRDSHOLD:
                        eCommandType = ElanCommand.ICValueTargetType.Param_AP_pPeakThrdshold;
                        nSetValue = ParamAutoTuning.m_nDefault_AP_pPeakThrdshold;
                        break;
                    */
                    case StringConvert.m_sCMDPARAM_EDGE_1TRC_SUBPWR:
                        eCommandType = ElanCommand.ICValueTargetType.Edge_1Trc_SubPwr;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_FM_Detect_Edge_1Trc_SubPwr;
                        break;
                    case StringConvert.m_sCMDPARAM_EDGE_2TRC_SUBPWR:
                        eCommandType = ElanCommand.ICValueTargetType.Edge_2Trc_SubPwr;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_FM_Detect_Edge_2Trc_SubPwr;
                        break;
                    case StringConvert.m_sCMDPARAM_EDGE_3TRC_SUBPWR:
                        eCommandType = ElanCommand.ICValueTargetType.Edge_3Trc_SubPwr;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_FM_Detect_Edge_3Trc_SubPwr;
                        break;
                    case StringConvert.m_sCMDPARAM_EDGE_4TRC_SUBPWR:
                        eCommandType = ElanCommand.ICValueTargetType.Edge_4Trc_SubPwr;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_FM_Detect_Edge_4Trc_SubPwr;
                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_P0:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_P0;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_DigiGain_P0;
                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_RX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_Beacon_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_DigiGain_Beacon_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_TX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_Beacon_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_DigiGain_Beacon_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_RX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_PTHF_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_DigiGain_PTHF_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_TX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_PTHF_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_DigiGain_PTHF_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_RX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_BHF_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_DigiGain_BHF_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_TX:
                        eCommandType = ElanCommand.ICValueTargetType.Param_DigiGain_BHF_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_DigiGain_BHF_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_P0_TH:
                        eCommandType = ElanCommand.ICValueTargetType.P0_TH;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_FM_P0_TH;
                        break;
                    case StringConvert.m_sCMDPARAM_TRXS_CONTACT_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.TRxS_Contact_TH_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_TRxS_Beacon_Contact_TH_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_TRXS_CONTACT_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.TRxS_Contact_TH_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_TRxS_Beacon_Contact_TH_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_TRXS_HOVER_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.TRxS_Hover_TH_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_TRxS_Beacon_Hover_TH_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_TRXS_HOVER_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.TRxS_Hover_TH_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_TRxS_Beacon_Hover_TH_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_CONTACT_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.Contact_TH_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_Beacon_Contact_TH_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_CONTACT_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.Contact_TH_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_Beacon_Contact_TH_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_HOVER_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.Hover_TH_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_Beacon_Hover_TH_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_HOVER_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.Hover_TH_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_Beacon_Hover_TH_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_PTHF_CONTACT_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.PTHF_Contact_TH_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_PTHF_Contact_TH_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_PTHF_CONTACT_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.PTHF_Contact_TH_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_PTHF_Contact_TH_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_PTHF_HOVER_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.PTHF_Hover_TH_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_PTHF_Hover_TH_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_PTHF_HOVER_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.PTHF_Hover_TH_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_PTHF_Hover_TH_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_BHF_CONTACT_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.BHF_Contact_TH_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_BHF_Contact_TH_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_BHF_CONTACT_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.BHF_Contact_TH_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_BHF_Contact_TH_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_BHF_HOVER_TH_RX:
                        eCommandType = ElanCommand.ICValueTargetType.BHF_Hover_TH_Rx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_BHF_Hover_TH_Rx;
                        break;
                    case StringConvert.m_sCMDPARAM_BHF_HOVER_TH_TX:
                        eCommandType = ElanCommand.ICValueTargetType.BHF_Hover_TH_Tx;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_BHF_Hover_TH_Tx;
                        break;
                    case StringConvert.m_sCMDPARAM_PEN_HI_HF_THD:
                        eCommandType = ElanCommand.ICValueTargetType.Param_Pen_HI_HF_THD;
                        nSetValue = ParamAutoTuning.m_nDefault_cActivePen_Pen_HI_HF_THD;
                        break;
                }
            }
        }

        /// <summary>
        /// 讀取及確認FW Check Version數值
        /// </summary>
        /// <returns>回傳讀取到的FW Check Version數值</returns>
        private int GetAndCheckFWCheckVersion()
        {
            int nGetValue = -1;
            int nRetryCount = 3;
            string sOutputMessage = "";

            OutputMessage("-Get FWCheckVersion Value Command");

            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                nGetValue = GetFWValueBySendCommand(ElanCommand.ElanCommandType.GetSNVersion, 1000);

                if (nGetValue == -1)
                {
                    sOutputMessage = string.Format("Can't Get FWCheckVersion[{0}]", nGetValue.ToString());
                    OutputMessage(string.Format("-{0}", sOutputMessage));
                }
                else if (nGetValue != ParamAutoTuning.m_nFWCheckVersion)
                {
                    sOutputMessage = string.Format("No Support This FW. Please Check the FW! [{0}]", nGetValue.ToString("x2").PadLeft(2, '0').ToUpper());
                    OutputMessage(string.Format("-{0}", sOutputMessage));
                    return nGetValue;
                }
                else
                {
                    sOutputMessage = string.Format("Check FWCheckVersion Complete[{0}]", nGetValue.ToString("x2").PadLeft(2, '0').ToUpper());
                    OutputMessage(string.Format("-{0}", sOutputMessage));
                    return nGetValue;
                }
            }

            return nGetValue;
        }

        /// <summary>
        /// 將讀取到的數值存入CurrentParameterSet
        /// </summary>
        /// <param name="sFWParameter">Command參數名稱</param>
        /// <param name="nGetValue">讀取到的數值</param>
        private void SetGetValue(string sFWParameter, int nGetValue)
        {
            switch (sFWParameter)
            {
                case StringConvert.m_sCMDPARAM_FWCHECKVERSION:
                    m_cCurrentParameterSet.m_nFWCheckVersion = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_PH1:
                    m_cCurrentParameterSet.m_nReadPH1 = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_PH2:
                    m_cCurrentParameterSet.m_nReadPH2 = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_NOISE_PTHF:
                case StringConvert.m_sCMDPARAM_NOISE_BHF:
                    break;
                case StringConvert.m_sCMDPARAM_REPORTNUMBER:
                    m_cCurrentParameterSet.m_nReadReportNumber = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_P0_TH:
                    m_cCurrentParameterSet.m_nReadcActivePen_FM_P0_TH = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_TRXS_HOVER_TH_RX:
                    m_cCurrentParameterSet.m_nReadTRxS_Beacon_Hover_TH_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_TRXS_HOVER_TH_TX:
                    m_cCurrentParameterSet.m_nReadTRxS_Beacon_Hover_TH_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_TRXS_CONTACT_TH_RX:
                    m_cCurrentParameterSet.m_nReadTRxS_Beacon_Contact_TH_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_TRXS_CONTACT_TH_TX:
                    m_cCurrentParameterSet.m_nReadTRxS_Beacon_Contact_TH_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_EDGE_1TRC_SUBPWR:
                    m_cCurrentParameterSet.m_nREdge_1Trc_SubPwr = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_EDGE_2TRC_SUBPWR:
                    m_cCurrentParameterSet.m_nREdge_2Trc_SubPwr = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_EDGE_3TRC_SUBPWR:
                    m_cCurrentParameterSet.m_nREdge_3Trc_SubPwr = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_EDGE_4TRC_SUBPWR:
                    m_cCurrentParameterSet.m_nREdge_4Trc_SubPwr = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_HOVER_TH_RX:
                    m_cCurrentParameterSet.m_nReadBeacon_Hover_TH_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_HOVER_TH_TX:
                    m_cCurrentParameterSet.m_nReadBeacon_Hover_TH_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_CONTACT_TH_RX:
                    m_cCurrentParameterSet.m_nReadBeacon_Contact_TH_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_CONTACT_TH_TX:
                    m_cCurrentParameterSet.m_nReadBeacon_Contact_TH_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_PTHF_CONTACT_TH_RX:
                    m_cCurrentParameterSet.m_nReadPTHF_Contact_TH_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_PTHF_CONTACT_TH_TX:
                    m_cCurrentParameterSet.m_nReadPTHF_Contact_TH_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_PTHF_HOVER_TH_RX:
                    m_cCurrentParameterSet.m_nReadPTHF_Hover_TH_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_PTHF_HOVER_TH_TX:
                    m_cCurrentParameterSet.m_nReadPTHF_Hover_TH_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_BHF_CONTACT_TH_RX:
                    m_cCurrentParameterSet.m_nReadBHF_Contact_TH_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_BHF_CONTACT_TH_TX:
                    m_cCurrentParameterSet.m_nReadBHF_Contact_TH_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_BHF_HOVER_TH_RX:
                    m_cCurrentParameterSet.m_nReadBHF_Hover_TH_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_BHF_HOVER_TH_TX:
                    m_cCurrentParameterSet.m_nReadBHF_Hover_TH_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_IQ_BSH_P:
                    m_cCurrentParameterSet.m_nRIQ_BSH_P = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_PRESSURE3BINSTH:
                    m_cCurrentParameterSet.m_nRPressure3BinsTH = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_PRESS_3BINSPWR:
                    m_cCurrentParameterSet.m_nR3BinsPwr = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_DIGIGAIN_P0:
                    m_cCurrentParameterSet.m_nRDigiGain_P0 = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_RX:
                    m_cCurrentParameterSet.m_nRDigiGain_Beacon_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_DIGIGAIN_BEACON_TX:
                    m_cCurrentParameterSet.m_nRDigiGain_Beacon_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_RX:
                    m_cCurrentParameterSet.m_nRDigiGain_PTHF_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_DIGIGAIN_PTHF_TX:
                    m_cCurrentParameterSet.m_nRDigiGain_PTHF_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_RX:
                    m_cCurrentParameterSet.m_nRDigiGain_BHF_Rx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_DIGIGAIN_BHF_TX:
                    m_cCurrentParameterSet.m_nRDigiGain_BHF_Tx = nGetValue;
                    break;
                case StringConvert.m_sCMDPARAM_PEN_HI_HF_THD:
                    m_cCurrentParameterSet.m_nRPen_HI_HF_THD = nGetValue;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 設定以及讀取TP數值之主流程
        /// </summary>
        /// <param name="eTargetType">設定的Command名稱</param>
        /// <param name="nTargetValue">設定的Command數值</param>
        /// <returns>回傳讀取到的數值</returns>
        private int SetAndGetICValue(ElanCommand.ICValueTargetType eTargetType, int nTargetValue)
        {
            int nGetValue = -1;
            int nRetryCount = 3;
            string sOutputMessage;

            ElanCommand.ElanCommandType eSetCommandType = ElanCommand.ElanCommandType.NA;

            if (ElanCommand.m_dictSetCmdMappingTable.ContainsKey(eTargetType) == true)
                eSetCommandType = ElanCommand.m_dictSetCmdMappingTable[eTargetType];
            else
            {
                sOutputMessage = string.Format("Set {0} Set Command Error", eTargetType.ToString());
                OutputMessage(string.Format("-{0}", sOutputMessage));
                return nGetValue;
            }

            ElanCommand.ElanCommandType eGetCommandType = ElanCommand.ElanCommandType.NA;

            if (ElanCommand.m_dictGetCmdMappingTable.ContainsKey(eTargetType) == true)
                eGetCommandType = ElanCommand.m_dictGetCmdMappingTable[eTargetType];
            else
            {
                sOutputMessage = string.Format("Set {0} Get Command Error", eTargetType.ToString());
                OutputMessage(string.Format("-{0}", sOutputMessage));
                return nGetValue;
            }

            byte[] byteSetCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(eSetCommandType, nTargetValue);
            //byte[] byteGetCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(eGetCommandType);

            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                //Send FW Command
                sOutputMessage = string.Format("Write {0} Value Command[{1}]", eTargetType.ToString(), Convert.ToString(nTargetValue).PadLeft(2, '0'));
                OutputMessage(string.Format("-{0}", sOutputMessage));
                Send_FW_Command(sOutputMessage, byteSetCommand_Array, byteSetCommand_Array.Length, 1000);
                Thread.Sleep(20);

                //Read FW Value
                sOutputMessage = string.Format("Read {0} Value Command", eTargetType.ToString());
                OutputMessage(string.Format("-{0}", sOutputMessage));
                nGetValue = GetFWValueBySendCommand(eGetCommandType, 1000);

                if (nGetValue == nTargetValue)
                {
                    sOutputMessage = string.Format("Set {0} Value Complete [{1}]", eTargetType.ToString(), Convert.ToString(nGetValue).PadLeft(2, '0'));
                    OutputMessage(string.Format("-{0}", sOutputMessage));
                    return nGetValue;
                }
                else
                {
                    sOutputMessage = string.Format("Set {0} Value Error [{1}]", eTargetType.ToString(), Convert.ToString(nGetValue).PadLeft(2, '0'));
                    OutputMessage(string.Format("-{0}", sOutputMessage));
                }
            }

            return nGetValue;
        }

        /// <summary>
        /// 設定FW數值
        /// </summary>
        /// <param name="eCommandType">Command種類</param>
        /// <param name="sCommandName">Command名稱</param>
        private void SetFWValueBySendCommand(ElanCommand.ElanCommandType eCommandType, string sCommandName)
        {
            string sOutputMessage;
            byte[] byteCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(eCommandType);

            //Send Command
            sOutputMessage = string.Format("Set {0} Command", sCommandName);

            OutputMessage(string.Format("-{0}", sOutputMessage));

            Send_FW_Command(sOutputMessage, byteCommand_Array, byteCommand_Array.Length, 1000);
            Thread.Sleep(20);
        }

        /// <summary>
        /// 讀取FW數值
        /// </summary>
        /// <param name="eTargetName">參數名稱</param>
        /// <returns>回傳參數數值</returns>
        private int GetFWValue(ElanCommand.ICValueTargetType eTargetName)
        {
            int nGetValue = -1;
            int nRetryCount = 3;
            string sOutputMessage = "";

            ElanCommand.ElanCommandType eGetCommandType = ElanCommand.ElanCommandType.NA;

            if (ElanCommand.m_dictGetCmdMappingTable.ContainsKey(eTargetName) == true)
                eGetCommandType = ElanCommand.m_dictGetCmdMappingTable[eTargetName];
            else
            {
                sOutputMessage = string.Format("Set {0} Get Command Error", eTargetName.ToString());
                OutputMessage(string.Format("-{0}", sOutputMessage));
                return nGetValue;
            }

            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                sOutputMessage = string.Format("Read {0} Value Command", eTargetName.ToString());
                OutputMessage(string.Format("-{0}", sOutputMessage));
                nGetValue = GetFWValueBySendCommand(eGetCommandType, 1000);

                if (nGetValue == -1)
                {
                    sOutputMessage = string.Format("Can't get {0} [{1}]", eTargetName.ToString(), nGetValue.ToString());
                    OutputMessage(string.Format("-{0}", sOutputMessage));
                }
                else
                {
                    sOutputMessage = string.Format("Get {0} Complete [{1}]", eTargetName.ToString(), nGetValue.ToString());
                    OutputMessage(string.Format("-{0}", sOutputMessage));
                    return nGetValue;
                }
            }

            return nGetValue;
        }

        private bool GetAndCheckFWValueBySendCommand(ElanCommand.ICValueTargetType eTargetType, int nTargetValue = 16384, bool bOnlyGetACKFlag = false)
        {
            int nGetValue = -1;
            int nRetryCount = 3;
            int nSubstractValue = 10;
            string sOutputMessage = "";
            int nPreviousValue = 0;
            int nLowBoundary = 163;
            int nHighBoundary = 65536;

            ElanCommand.ElanCommandType eSetCommandType = ElanCommand.ElanCommandType.NA;

            if (ElanCommand.m_dictSetCmdMappingTable.ContainsKey(eTargetType) == true)
                eSetCommandType = ElanCommand.m_dictSetCmdMappingTable[eTargetType];
            else
            {
                sOutputMessage = string.Format("Set {0} Set Command Error", eTargetType.ToString());
                OutputMessage(string.Format("-{0}", sOutputMessage));
                return false;
            }

            ElanCommand.ElanCommandType eGetCommandType = ElanCommand.ElanCommandType.NA;

            if (ElanCommand.m_dictGetCmdMappingTable.ContainsKey(eTargetType) == true)
                eGetCommandType = ElanCommand.m_dictGetCmdMappingTable[eTargetType];
            else
            {
                sOutputMessage = string.Format("Set {0} Get Command Error", eTargetType.ToString());
                OutputMessage(string.Format("-{0}", sOutputMessage));
                return false;
            }


            byte[] byteGetCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(eGetCommandType);

            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                if (bOnlyGetACKFlag == false)
                {
                    int nSetValue = nTargetValue - (nSubstractValue * nRetryIndex);
                    byte[] byteSetCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(eSetCommandType, nSetValue);

                    //Send FW Command
                    string sSetValue = Convert.ToString(nSetValue).PadLeft(2, '0');
                    sOutputMessage = string.Format("Write {0} Value Command[{1}]", eTargetType.ToString(), sSetValue);
                    OutputMessage(string.Format("-{0}", sOutputMessage));
                    Send_FW_Command(sOutputMessage, byteSetCommand_Array, byteSetCommand_Array.Length, 1000);
                    Thread.Sleep(20);

                    //Read FW Value
                    sOutputMessage = string.Format("Read {0} Value Command", eTargetType.ToString());
                    OutputMessage(string.Format("-{0}", sOutputMessage));
                    nGetValue = GetFWValueBySendCommand(eGetCommandType, 1000);

                    string sGetValue = Convert.ToString(nGetValue).PadLeft(2, '0');

                    if (nGetValue == nSetValue)
                    {
                        sOutputMessage = string.Format("Set {0} Value Complete[{1}]", eTargetType.ToString(), sGetValue);
                        OutputMessage(string.Format("-{0}", sOutputMessage));
                    }
                    else
                    {
                        sOutputMessage = string.Format("Set {0} Value Error[{1}]", eTargetType.ToString(), sGetValue);
                        OutputMessage(string.Format("-{0}", sOutputMessage));
                        return false;
                    }
                }
                else
                {
                    //Read FW Value
                    sOutputMessage = string.Format("Read {0} Value Command", eTargetType.ToString());
                    OutputMessage(string.Format("-{0}", sOutputMessage));
                    nGetValue = GetFWValueBySendCommand(eGetCommandType, 1000);

                    string sGetValue = Convert.ToString(nGetValue).PadLeft(2, '0');

                    if (nGetValue < nLowBoundary)
                    {
                        string sLowBoundary = Convert.ToString(nLowBoundary).PadLeft(2, '0');
                        sOutputMessage = string.Format("Get {0} Value Error [{1} Under LowBoundary:{2}]", eTargetType.ToString(), sGetValue, sLowBoundary);
                        OutputMessage(string.Format("-{0}", sOutputMessage));
                        return false;
                    }
                    else if (nGetValue > nHighBoundary)
                    {
                        string sHighBoundary = Convert.ToString(nHighBoundary).PadLeft(2, '0');
                        sOutputMessage = string.Format("Get {0} Value Error [{1} Over HighBoundary:{2}]", eTargetType.ToString(), sGetValue, sHighBoundary);
                        OutputMessage(string.Format("-{0}", sOutputMessage));
                        return false;
                    }

                    if (nRetryIndex > 0)
                    {
                        if (nPreviousValue != nGetValue)
                        {
                            string sPreviousValue = Convert.ToString(nPreviousValue).PadLeft(2, '0');
                            sOutputMessage = string.Format("Get {0} Value Error [{1}, PreviousValue:{2}]", eTargetType.ToString(), sGetValue, sPreviousValue);
                            OutputMessage(string.Format("-{0}", sOutputMessage));
                            return false;
                        }

                        nPreviousValue = nGetValue;
                    }
                    else
                    {
                        nPreviousValue = nGetValue;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 讀取TP數值
        /// </summary>
        /// <param name="eCommandType">Command種類</param>
        /// <param name="nTimeout">Timeout時間(ms)</param>
        /// <returns>回傳TP數值</returns>
        private int GetFWValueBySendCommand(ElanCommand.ElanCommandType eCommandType, int nTimeout)
        {
            int nGetValue = -1;
            byte[] byteCommand_Array = ElanCommand.ConvertCommandTypeToByteArray(eCommandType);

            string sActionMessage = "Get FW Value by Send Command";

            OutputMessage(string.Format("-{0}", sActionMessage));

            Send_FW_Command(sActionMessage, byteCommand_Array, byteCommand_Array.Length, 1000);
            nGetValue = GetFWDataBySendCommnad(eCommandType, nTimeout);
            Thread.Sleep(20);

            return nGetValue;
        }

        /// <summary>
        /// 讀取對應的數值
        /// </summary>
        /// <param name="eCommandType">Command種類</param>
        /// <param name="nTimeout">Timeout時間(ms)</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private int GetFWDataBySendCommnad(ElanCommand.ElanCommandType eCommandType, int nTimeout)
        {
            int nGetValue = -1;
            int nRetryCount = 50;

            int nCheckByteNumber = 2;
            byte[] byteCheckByte_Array = new byte[2];

            if (ParamAutoTuning.m_nFWTypeIndex == 1)
            {
                nCheckByteNumber = 3;
                byteCheckByte_Array = new byte[3];
            }

            Array.Clear(byteCheckByte_Array, 0, byteCheckByte_Array.Length - 1);

            byteCheckByte_Array[0] = 0x65;

            if (ParamAutoTuning.m_nFWTypeIndex == 1)
            {
                byteCheckByte_Array[0] = 0x95;
                byteCheckByte_Array[1] = 0x16;
            }

            switch (eCommandType)
            {
                case ElanCommand.ElanCommandType.GetParam_AP_pPeakThrdshold:
                    nCheckByteNumber = 2;
                    byteCheckByte_Array = new byte[2];

                    byteCheckByte_Array[0] = 0x52;
                    byteCheckByte_Array[1] = 0xBE;
                    break;
                case ElanCommand.ElanCommandType.GetPH1:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x8B;
                    else
                        byteCheckByte_Array[1] = 0x1B;

                    break;
                case ElanCommand.ElanCommandType.GetPH2:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x8C;
                    else
                        byteCheckByte_Array[1] = 0x1C;

                    break;
                case ElanCommand.ElanCommandType.GetReportNumber:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x77;
                    else
                        byteCheckByte_Array[1] = 0x07;

                    break;
                case ElanCommand.ElanCommandType.GetP0_TH:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x94;
                    else
                        byteCheckByte_Array[1] = 0x24;

                    break;
                case ElanCommand.ElanCommandType.GetTRxS_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xDF;
                    else
                        byteCheckByte_Array[1] = 0x6F;

                    break;
                case ElanCommand.ElanCommandType.GetTRxS_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xE0;
                    else
                        byteCheckByte_Array[1] = 0x70;

                    break;
                case ElanCommand.ElanCommandType.GetTRxS_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xDD;
                    else
                        byteCheckByte_Array[1] = 0x6D;

                    break;
                case ElanCommand.ElanCommandType.GetTRxS_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xDE;
                    else
                        byteCheckByte_Array[1] = 0x6E;

                    break;
                case ElanCommand.ElanCommandType.GetHover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x97;
                    else
                        byteCheckByte_Array[1] = 0x27;

                    break;
                case ElanCommand.ElanCommandType.GetHover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x98;
                    else
                        byteCheckByte_Array[1] = 0x28;

                    break;
                case ElanCommand.ElanCommandType.GetContact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x95;
                    else
                        byteCheckByte_Array[1] = 0x25;

                    break;
                case ElanCommand.ElanCommandType.GetContact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x96;
                    else
                        byteCheckByte_Array[1] = 0x26;

                    break;
                case ElanCommand.ElanCommandType.GetPTHF_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9C;
                    else
                        byteCheckByte_Array[1] = 0x2C;

                    break;
                case ElanCommand.ElanCommandType.GetPTHF_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9D;
                    else
                        byteCheckByte_Array[1] = 0x2D;

                    break;
                case ElanCommand.ElanCommandType.GetPTHF_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9A;
                    else
                        byteCheckByte_Array[1] = 0x2A;

                    break;
                case ElanCommand.ElanCommandType.GetPTHF_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9B;
                    else
                        byteCheckByte_Array[1] = 0x2B;

                    break;
                case ElanCommand.ElanCommandType.GetBHF_Hover_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xA0;
                    else
                        byteCheckByte_Array[1] = 0x30;

                    break;
                case ElanCommand.ElanCommandType.GetBHF_Hover_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xA1;
                    else
                        byteCheckByte_Array[1] = 0x31;

                    break;
                case ElanCommand.ElanCommandType.GetBHF_Contact_TH_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9E;
                    else
                        byteCheckByte_Array[1] = 0x2E;

                    break;
                case ElanCommand.ElanCommandType.GetBHF_Contact_TH_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x9F;
                    else
                        byteCheckByte_Array[1] = 0x2F;

                    break;
                case ElanCommand.ElanCommandType.GetEdge_1Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xB5;
                    else
                        byteCheckByte_Array[1] = 0x45;

                    break;
                case ElanCommand.ElanCommandType.GetEdge_2Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xB6;
                    else
                        byteCheckByte_Array[1] = 0x46;

                    break;
                case ElanCommand.ElanCommandType.GetEdge_3Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xB7;
                    else
                        byteCheckByte_Array[1] = 0x47;

                    break;
                case ElanCommand.ElanCommandType.GetEdge_4Trc_SubPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xB8;
                    else
                        byteCheckByte_Array[1] = 0x48;

                    break;
                case ElanCommand.ElanCommandType.GetIQ_BSH_P:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x78;
                    else
                        byteCheckByte_Array[1] = 0x08;

                    break;
                case ElanCommand.ElanCommandType.GetPressure3BinsTH:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x99;
                    else
                        byteCheckByte_Array[1] = 0x29;

                    break;
                case ElanCommand.ElanCommandType.Get3BinsPwr:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0xC8;
                    else
                        byteCheckByte_Array[1] = 0x58;

                    break;
                case ElanCommand.ElanCommandType.GetParam_DigiGain_P0:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x09;

                    break;
                case ElanCommand.ElanCommandType.GetParam_DigiGain_Beacon_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x0A;

                    break;
                case ElanCommand.ElanCommandType.GetParam_DigiGain_Beacon_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x85;

                    break;
                case ElanCommand.ElanCommandType.GetParam_DigiGain_PTHF_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x86;

                    break;
                case ElanCommand.ElanCommandType.GetParam_DigiGain_PTHF_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x87;

                    break;
                case ElanCommand.ElanCommandType.GetParam_DigiGain_BHF_Rx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x88;

                    break;
                case ElanCommand.ElanCommandType.GetParam_DigiGain_BHF_Tx:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x89;

                    break;
                case ElanCommand.ElanCommandType.GetParam_Pen_HI_HF_THD:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                        byteCheckByte_Array[2] = 0x00;
                    else
                        byteCheckByte_Array[1] = 0x32;

                    break;
                case ElanCommand.ElanCommandType.GetSNVersion:
                    byteCheckByte_Array[0] = 0x52;
                    byteCheckByte_Array[1] = 0xD3;
                    nCheckByteNumber = 2;
                    break;
                default:
                    if (ParamAutoTuning.m_nFWTypeIndex == 1)
                    {
                        byteCheckByte_Array[0] = 0x00;
                        byteCheckByte_Array[1] = 0x00;
                        byteCheckByte_Array[2] = 0x00;
                    }
                    else
                    {
                        byteCheckByte_Array[0] = 0x00;
                        byteCheckByte_Array[1] = 0x00;
                    }

                    break;
            }

            while (nRetryCount > 0)
            {
                byte[] byteData_Array = new byte[65];
                int nResult = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, nTimeout, m_cfrmMain.m_nDeviceIndex);

                if (nResult != ElanTouch.TP_SUCCESS)
                {
                    string sMessage = string.Format("TP ERRORCODE : 0x{0}", nResult.ToString("X4"));
                    WriteDebugLog(sMessage);
                    Thread.Sleep(nTimeout);
                    break;
                }

                string sGetACK = "-Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                WriteDebugLog(sGetACK);

                bool bCheckCorrectFlag = true;

                for (int nByteIndex = 0; nByteIndex < nCheckByteNumber; nByteIndex++)
                {
                    if (byteData_Array[nByteIndex] != byteCheckByte_Array[nByteIndex])
                    {
                        bCheckCorrectFlag = false;
                        break;
                    }
                }

                if (bCheckCorrectFlag == false)
                {
                    nRetryCount--;
                    continue;
                }
                else
                {
                    nGetValue = ElanCommand.ConvertGetBufferToGetValue(eCommandType, byteData_Array);
                    break;
                }
            }

            return nGetValue;
        }
        #endregion

        #region Show Warning Window Function
        private void ShowWarningMessageByTPGainCoordinateSetting(FlowStep cFlowStep, FlowRobot eRobotType)
        {
            if (cFlowStep.m_eMainStep == MainTuningStep.TPGAINTUNING && cFlowStep.m_eSubStep == SubTuningStep.TP_GAIN)
            {
                string sMessage = "";

                if (eRobotType == FlowRobot.TOUCHLINE_HOR)
                {
                    sMessage = string.Format("Draw Horizontal Line.Please Set the Correct Coordinate!(V Angle:{0} degree, R Angle:{1} degree)", ParamAutoTuning.m_nTPGTVAngle, ParamAutoTuning.m_nTPGTHorizontalRAngle);
                }
                else if (eRobotType == FlowRobot.TOUCHLINE_VER)
                {
                    sMessage = string.Format("Draw Vertical Line.Please Set the Correct Coordinate!(V Angle:{0} degree, R Angle:{1} degree)", ParamAutoTuning.m_nTPGTVAngle, ParamAutoTuning.m_nTPGTVerticalRAngle);
                }

                ShowfrmWarningMessage(sMessage);
            }
        }

        private void ShowfrmWarningMessage(string sMessage)
        {
            m_cfrmMain.Invoke((MethodInvoker)delegate
            {
                frmWarningMessage cfrmWarningMessage = new frmWarningMessage();

                int nLocationX = (int)((m_cfrmMain.Left + m_cfrmMain.Right) / 2) - (int)(cfrmWarningMessage.Width / 2);
                int nLocationY = (int)((m_cfrmMain.Top + m_cfrmMain.Bottom) / 2) - (int)(cfrmWarningMessage.Height / 2);

                if (m_cfrmMain.IsMdiChild == true)
                {
                    nLocationX = (int)((m_cfrmMain.MdiParent.Left + m_cfrmMain.MdiParent.Right) / 2) - (int)(cfrmWarningMessage.Width / 2);
                    nLocationY = (int)((m_cfrmMain.MdiParent.Top + m_cfrmMain.MdiParent.Bottom) / 2) - (int)(cfrmWarningMessage.Height / 2);
                }

                if (m_cfrmMain.m_bParentFormFlag == true)
                {
                    nLocationX = (int)((m_cfrmMain.ParentForm.Left + m_cfrmMain.ParentForm.Right) / 2) - (int)(cfrmWarningMessage.Width / 2);
                    nLocationY = (int)((m_cfrmMain.ParentForm.Top + m_cfrmMain.ParentForm.Bottom) / 2) - (int)(cfrmWarningMessage.Height / 2);
                }

                cfrmWarningMessage.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                cfrmWarningMessage.Location = new System.Drawing.Point(nLocationX, nLocationY);

                cfrmWarningMessage.LoadWarningMessage(sMessage);

                if (cfrmWarningMessage.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return;
            });
        }

        private bool Show_frmWarningMessage_2Selection(string sMessage)
        {
            bool bContinueFlag = false;

            m_cfrmMain.Invoke((MethodInvoker)delegate
            {
                frmWarningMessage_2Selection cfrmWarningMessage_2Selection = new frmWarningMessage_2Selection();

                int nLocationX = (int)((m_cfrmMain.Left + m_cfrmMain.Right) / 2) - (int)(cfrmWarningMessage_2Selection.Width / 2);
                int nLocationY = (int)((m_cfrmMain.Top + m_cfrmMain.Bottom) / 2) - (int)(cfrmWarningMessage_2Selection.Height / 2);

                if (m_cfrmMain.IsMdiChild == true)
                {
                    nLocationX = (int)((m_cfrmMain.MdiParent.Left + m_cfrmMain.MdiParent.Right) / 2) - (int)(cfrmWarningMessage_2Selection.Width / 2);
                    nLocationY = (int)((m_cfrmMain.MdiParent.Top + m_cfrmMain.MdiParent.Bottom) / 2) - (int)(cfrmWarningMessage_2Selection.Height / 2);
                }

                if (m_cfrmMain.m_bParentFormFlag == true)
                {
                    nLocationX = (int)((m_cfrmMain.ParentForm.Left + m_cfrmMain.ParentForm.Right) / 2) - (int)(cfrmWarningMessage_2Selection.Width / 2);
                    nLocationY = (int)((m_cfrmMain.ParentForm.Top + m_cfrmMain.ParentForm.Bottom) / 2) - (int)(cfrmWarningMessage_2Selection.Height / 2);
                }

                cfrmWarningMessage_2Selection.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                cfrmWarningMessage_2Selection.Location = new System.Drawing.Point(nLocationX, nLocationY);

                cfrmWarningMessage_2Selection.Load_Warning_Message(sMessage);

                if (cfrmWarningMessage_2Selection.ShowDialog() == System.Windows.Forms.DialogResult.No)
                    bContinueFlag = false;
                else
                    bContinueFlag = true;
            });

            return bContinueFlag;
        }
        #endregion

        #region Set Pattern Time and Report Number Function
        /// <summary>
        /// 設定在Pattern上顯示錄製時間及Report Data筆數
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        /// <param name="dRecordTime">錄製時間(ms)</param>
        /// <param name="nReportNumber">Report Data筆數</param>
        private void SetPatternTimeAndReportNumber(RobotParameter cRobotParameter, double dRecordTime, int nReportNumber)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            if (eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1)
                return;

            if (ParamAutoTuning.m_bDisplayReportNumber == true || eMainStep == MainTuningStep.PRESSURETUNING)
            {
                m_cfrmMain.Invoke((MethodInvoker)delegate
                {
                    m_cfrmMain.SetTimeAndReportNumber(dRecordTime, nReportNumber);
                });
            }
        }
        #endregion

        #region Check Report Data Function
        /// <summary>
        /// 確認Report Data是否有足夠有效筆數的主流程
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        /// <param name="nFlowIndex">錄製流程筆數索引值</param>
        /// <param name="bSingleModeFlag">是否為單機模式</param>
        private void CheckReportDataIsValid(RobotParameter cRobotParameter, int nFlowIndex, bool bSingleModeFlag)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;
            string sSubStepCodeName = StringConvert.m_dictSubStepCNMappingTable[eSubStep];

            bool bErrorFlag = false;
            string sErrorCode = "";
            string sErrorMessage = "";

            if (eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1)
            {
                if (m_nRecordDataErrorFlag == -1)
                {
                    string sSaveFolderPath = string.Format(@"{0}\RawData", m_sStepDirectoryPath);

                    if (Directory.Exists(sSaveFolderPath) == false)
                        Directory.CreateDirectory(sSaveFolderPath);

                    string sFilePath = string.Format(@"{0}\{1}_Record.CSV", sSaveFolderPath, m_sLogFileName);
                    GetFrameData.GetDataState eGetDataState = m_cGetFrameData.SaveRecordData(sFilePath);

                    if (eGetDataState == GetFrameData.GetDataState.GetDataState_SaveError)
                    {
                        bErrorFlag = true;
                        m_nRecordDataErrorFlag = 4;
                        sErrorMessage = m_cGetFrameData.GetErrorMessage();
                    }

                    sFilePath = string.Format("{0}_Frame.csv", m_sLogFilePath);
                    eGetDataState = m_cGetFrameData.SaveFrameData(sFilePath, ParamAutoTuning.m_nGen8RealTraceNumber);

                    if (eGetDataState == GetFrameData.GetDataState.GetDataState_SaveError)
                    {
                        bErrorFlag = true;
                        m_nRecordDataErrorFlag = 4;
                        sErrorMessage = m_cGetFrameData.GetErrorMessage();
                    }
                }

                return;
            }

            if (m_nRecordDataErrorFlag == -1 && (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO))
            {
                int nValidDataCount = 0;
                m_nRecordDataErrorFlag = CheckDataNumberIsValid(ref nValidDataCount, cRobotParameter, m_byteReportData_List);

                if (m_nRecordDataErrorFlag == 1)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                        sErrorCode = string.Format("{0}.003", sSubStepCodeName);

                    bErrorFlag = true;
                    sErrorMessage = string.Format("No Enough RX Report Section Data({0} < LB:{1})", nValidDataCount, m_nNoiseValidReportNumber);
                }
                else if (m_nRecordDataErrorFlag == 2)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                        sErrorCode = string.Format("{0}.004", sSubStepCodeName);

                    bErrorFlag = true;
                    sErrorMessage = string.Format("No Enough TX Report Section Data({0} < LB:{1})", nValidDataCount, m_nNoiseValidReportNumber);
                }
            }
            else if (m_nRecordDataErrorFlag == -1 && eMainStep == MainTuningStep.DIGIGAINTUNING)
            {
                int nValidDataCount = 0;
                m_nRecordDataErrorFlag = CheckDataNumberIsValid(ref nValidDataCount, cRobotParameter, m_byteReportData_List);

                switch (m_nRecordDataErrorFlag)
                {
                    case 1:
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.002", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough RX_Beacon Report Data({0} < LB:{1})", nValidDataCount, ParamAutoTuning.m_nDGTRXValidReportNumber);
                        break;
                    case 2:
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.003", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough RX_PTHF Report Data({0} < LB:{1})", nValidDataCount, ParamAutoTuning.m_nDGTRXValidReportNumber);
                        break;
                    case 3:
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.004", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough RX_BHF Report Data({0} < LB:{1})", nValidDataCount, ParamAutoTuning.m_nDGTRXValidReportNumber);
                        break;
                    case 4:
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.005", sSubStepCodeName);

                        bErrorFlag = true;
                        m_sErrorMessage = string.Format("No Enough TX_Beacon Report Data({0} < LB:{1})", nValidDataCount, ParamAutoTuning.m_nDGTTXValidReportNumber);
                        break;
                    case 5:
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.006", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough TX_PTHF Report Data({0} < LB:{1})", nValidDataCount, ParamAutoTuning.m_nDGTTXValidReportNumber);
                        break;
                    case 6:
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.007", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough TX_BHF Report Data({0} < LB:{1})", nValidDataCount, ParamAutoTuning.m_nDGTTXValidReportNumber);
                        break;
                    default:
                        break;
                }
            }
            else if (m_nRecordDataErrorFlag == -1 && eMainStep == MainTuningStep.TPGAINTUNING)
            {
                int nValidDataCount = 0;
                m_nRecordDataErrorFlag = CheckDataNumberIsValid(ref nValidDataCount, cRobotParameter, m_byteReportData_List);

                switch (m_nRecordDataErrorFlag)
                {
                    case 1:
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.002", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough RX_BHF Tip Report Data({0} < LB:{1})", nValidDataCount, ParamAutoTuning.m_nTPGTRXValidReportNumber);
                        break;
                    case 2:
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.003", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough RX_BHF Ring Report Data({0} < LB:{1})", nValidDataCount, ParamAutoTuning.m_nTPGTRXValidReportNumber);
                        break;
                    case 3:
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.004", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough TX_BHF Tip Report Data({0} < LB:{1})", nValidDataCount, ParamAutoTuning.m_nTPGTTXValidReportNumber);
                        break;
                    case 4:
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.005", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough TX_BHF Ring Report Data({0} < LB:{1})", nValidDataCount, ParamAutoTuning.m_nTPGTTXValidReportNumber);
                        break;
                    default:
                        break;
                }
            }
            else if (m_nRecordDataErrorFlag == -1 && eMainStep == MainTuningStep.PEAKCHECKTUNING)
            {
                int nValidDataCount = 0;
                m_nRecordDataErrorFlag = CheckDataNumberIsValid(ref nValidDataCount, cRobotParameter, m_byteReportData_List, bSingleModeFlag);

                if (m_nRecordDataErrorFlag == 1)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                        sErrorCode = string.Format("{0}.002", sSubStepCodeName);

                    bErrorFlag = true;
                    sErrorMessage = string.Format("No Enough Report Data({0} < LB:{1})", nValidDataCount, m_nNormalValidReportNumber);
                }
                else if (m_nRecordDataErrorFlag == 2)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                        sErrorCode = string.Format("{0}.003", sSubStepCodeName);

                    bErrorFlag = true;
                    sErrorMessage = string.Format("No Enough RX Report Data({0} < LB:{1})", nValidDataCount, m_nNormalFilterRXValidReportNumber);
                }
                else if (m_nRecordDataErrorFlag == 3)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                        sErrorCode = string.Format("{0}.004", sSubStepCodeName);

                    bErrorFlag = true;
                    sErrorMessage = string.Format("No Enough TX Report Data({0} < LB:{1})", nValidDataCount, m_nNormalFilterTXValidReportNumber);
                }
            }
            else if (((bSingleModeFlag == true && m_nRecordDataErrorFlag == -1) || bSingleModeFlag == false) && eMainStep == MainTuningStep.DIGITALTUNING)
            {
                if (eSubStep == SubTuningStep.HOVER_1ST || eSubStep == SubTuningStep.HOVER_2ND || eSubStep == SubTuningStep.CONTACT)
                {
                    int nValidDataCount = 0;
                    m_nRecordDataErrorFlag = CheckDataNumberIsValid(ref nValidDataCount, cRobotParameter, m_byteReportData_List, bSingleModeFlag);

                    if (m_nRecordDataErrorFlag == 1)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.002", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough Report Data({0} < LB:{1})", nValidDataCount, m_nNormalValidReportNumber);
                    }
                    else if (m_nRecordDataErrorFlag == 2)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.003", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough RX Report Data({0} < LB:{1})", nValidDataCount, m_nNormalFilterRXValidReportNumber);
                    }
                    else if (m_nRecordDataErrorFlag == 3)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.004", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough TX Report Data({0} < LB:{1})", nValidDataCount, m_nNormalFilterTXValidReportNumber);
                    }
                }
                else if (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)
                {
                    int nValidDataCount = 0;
                    m_nRecordDataErrorFlag = CheckDataNumberIsValid(ref nValidDataCount, cRobotParameter, m_byteReportData_List);

                    if (m_nRecordDataErrorFlag == 1)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.002", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No RX Report Data({0} < LB:{1})", nValidDataCount, m_nTRxSRXValidReportNumber);
                    }
                    else if (m_nRecordDataErrorFlag == 2)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.003", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No TX Report Data({0} < LB:{1})", nValidDataCount, m_nTRxSTXValidReportNumber);
                    }
                }
            }
            else if (m_nRecordDataErrorFlag == -1 && eMainStep == MainTuningStep.TILTTUNING)
            {
                int nValidDataCount = 0;
                m_nRecordDataErrorFlag = CheckDataNumberIsValid(ref nValidDataCount, cRobotParameter, m_byteReportData_List);

                string sState = "";

                if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_HOR)
                    sState = "X";
                else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_VER)
                    sState = "Y";
                else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE)
                    sState = "Slant";

                if (m_nRecordDataErrorFlag == 1)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                        sErrorCode = string.Format("{0}.002", sSubStepCodeName);

                    bErrorFlag = true;
                    sErrorMessage = string.Format("No {0} Tip Trace Data({1} < LB:{2})", sState, nValidDataCount, ParamAutoTuning.m_nTTValidTipTraceNumber);
                }
                else if (m_nRecordDataErrorFlag == 2)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                        sErrorCode = string.Format("{0}.003", sSubStepCodeName);

                    m_sErrorMessage = string.Format("No Enough {0} Tip Trace Data({1} < LB:{2})", sState, nValidDataCount, ParamAutoTuning.m_nTTValidTipTraceNumber);
                }
                else if (m_nRecordDataErrorFlag == 3)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                        sErrorCode = string.Format("{0}.004", sSubStepCodeName);

                    bErrorFlag = true;
                    sErrorMessage = string.Format("No Enough {0} Tip Report Data({1} < LB:{2})", sState, nValidDataCount, ParamAutoTuning.m_nTTPolyFitOrder + 1);
                }
                else if (m_nRecordDataErrorFlag == 4)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                        sErrorCode = string.Format("{0}.005", sSubStepCodeName);

                    bErrorFlag = true;
                    sErrorMessage = string.Format("No Enough RX Tip Report Data({1} < LB:{2})", sState, nValidDataCount, ParamAutoTuning.m_nTTRXValidReportNumber);
                }
                else if (m_nRecordDataErrorFlag == 5)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                        sErrorCode = string.Format("{0}.006", sSubStepCodeName);

                    bErrorFlag = true;
                    sErrorMessage = string.Format("No Enough TX Tip Report Data({1} < LB:{2})", sState, nValidDataCount, ParamAutoTuning.m_nTTTXValidReportNumber);
                }
            }
            else if (m_nRecordDataErrorFlag == -1 && eMainStep == MainTuningStep.PRESSURETUNING)
            {
                /*if (eSubStep == SubTuningStep.PRESSUREPROTECT)
                {
                    int nValidDataCount = 0;
                    m_nRecordDataErrorFlag = CheckDataNumberIsValid(ref nValidDataCount, rParam, DataArray);

                    if (m_nRecordDataErrorFlag == 1)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.002", sSubStepCodeName);
                        
                        bErrorFlag = true;
                        sErrorMessage = string.Format("No RX Report Data({0} < LB:{1})", nValidDataCount, m_nPPValidReportNumber);
                    }
                    else if (m_nRecordDataErrorFlag == 2)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.003", sSubStepCodeName);
                        bErrorFlag = true;
                        sErrorMessage = string.Format("No TX Report Data({0} < LB:{1})", nValidDataCount, m_nPPValidReportNumber);
                    }
                }*/
                if ((eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE) && m_nRecordDataErrorFlag != 0xF001)
                {
                    int nOutputValue = 0;
                    int nRealValidReportNumber = ParamAutoTuning.m_nPTStartSkipReportNumber + ParamAutoTuning.m_nPTValidReportNumber + ParamAutoTuning.m_nPTEndSkipReportNumber;

                    m_nRecordDataErrorFlag = CheckDataNumberIsValid(ref nOutputValue, cRobotParameter, m_byteReportData_List);

                    if (m_nRecordDataErrorFlag == 1)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.002", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No Enough Pressure Report Data({0} < LB:{1})", nOutputValue, nRealValidReportNumber);
                    }
                    else if (m_nRecordDataErrorFlag == 2)
                    {
                        m_cCurrentParameterSet.m_nBefPress_MaxDFTRxMean = m_cCurrentParameterSet.m_nPress_MaxDFTRxMean;
                        m_cCurrentParameterSet.m_nPress_MaxDFTRxMean = nOutputValue;

                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.003", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("Press_MaxDFTRx Mean Under LB({0} < LB:{1})", nOutputValue, ParamAutoTuning.m_nPressMaxDFTRxRefValueLB);
                    }
                    else if (m_nRecordDataErrorFlag == 3)
                    {
                        m_cCurrentParameterSet.m_nBefPress_MaxDFTRxMean = m_cCurrentParameterSet.m_nPress_MaxDFTRxMean;
                        m_cCurrentParameterSet.m_nPress_MaxDFTRxMean = nOutputValue;

                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.004", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("Press_MaxDFTRx Mean Over HB({0} > HB:{1})", nOutputValue, ParamAutoTuning.m_nPressMaxDFTRxRefValueHB);
                    }
                    else if (eSubStep == SubTuningStep.PRESSURESETTING && m_nRecordDataErrorFlag == -1)
                    {
                        m_cCurrentParameterSet.m_nBefPress_MaxDFTRxMean = m_cCurrentParameterSet.m_nPress_MaxDFTRxMean;
                        m_cCurrentParameterSet.m_nPress_MaxDFTRxMean = nOutputValue;

                        string sMessage = string.Format("-Press_MaxDFTRx Mean In Range({0} <= {1} <= {2})", ParamAutoTuning.m_nPressMaxDFTRxRefValueLB, nOutputValue, ParamAutoTuning.m_nPressMaxDFTRxRefValueHB);

                        bErrorFlag = false;
                        sErrorCode = "";
                        m_sErrorMessage = "";
                        OutputMessage(sMessage);
                    }

                    if (eSubStep == SubTuningStep.PRESSURESETTING)
                    {
                        if (m_nRecordDataErrorFlag == 2)
                        {
                            if (m_cCurrentParameterSet.m_nSIQ_BSH_P <= ParamAutoTuning.m_nPTIQ_BSH_P_LB || m_cCurrentParameterSet.m_nRIQ_BSH_P > m_cCurrentParameterSet.m_nROrgIQ_BSH_P)
                                cRobotParameter.m_bLastRetryFlag = true;
                        }
                        else if (m_nRecordDataErrorFlag == 3)
                        {
                            if (m_cCurrentParameterSet.m_nSIQ_BSH_P >= ParamAutoTuning.m_nPTIQ_BSH_P_HB || m_cCurrentParameterSet.m_nRIQ_BSH_P < m_cCurrentParameterSet.m_nROrgIQ_BSH_P)
                                cRobotParameter.m_bLastRetryFlag = true;
                        }
                    }
                }
            }
            else if (m_nRecordDataErrorFlag == -1 && cRobotParameter.m_cFlowStep.m_eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                if (cRobotParameter.m_cFlowStep.m_eSubStep == SubTuningStep.LINEARITYTABLE)
                {
                    int nValidDataCount = 0;
                    m_nRecordDataErrorFlag = CheckDataNumberIsValid(ref nValidDataCount, cRobotParameter, m_byteReportData_List);

                    string sState = "";

                    if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_HOR)
                        sState = "Horizontal";
                    else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_VER)
                        sState = "Vertical";

                    if (m_nRecordDataErrorFlag == 1)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.002", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("No {0} Report Data({1})", sState, nValidDataCount);
                    }
                    else if (m_nRecordDataErrorFlag == 2)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.003", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("{0} No Valid Trace Data", sState);
                    }
                    else if (m_nRecordDataErrorFlag == 3)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.004", sSubStepCodeName);

                        bErrorFlag = true;
                        sErrorMessage = string.Format("{0} No Enough Valid Trace Data", sState);
                    }
                    /*
                    else if (m_nRecordDataErrorFlag == 4)
                    {
                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                            sErrorCode = string.Format("{0}.005", sSubStepCodeName);
                    
                        bErrorFlag = true;
                        sErrorMessage = string.Format("{0} No Enough Valid Data in Tr:{1}", sState, nValidDataCount);
                    }
                    */
                }
            }

            if (ParamAutoTuning.m_nFlowMethodType == 1)
            {
                SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorCode, sErrorCode, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorMsg, m_sErrorMessage, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorMessage, m_sErrorMessage, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
            }

            WriteReportData(ref m_nRecordDataErrorFlag, m_byteReportData_List, cRobotParameter);

            string sResultMessage = "";

            if (m_nRecordDataErrorFlag == -1 || m_nRecordDataErrorFlag == 0 || cRobotParameter.m_bLastRetryFlag == true)
            {
                if (m_nRecordDataErrorFlag != -1)
                {
                    if (m_nRecordDataErrorFlag == 4)
                    {
                        bErrorFlag = true;
                        sErrorMessage = "Write Data File Error";

                        if (ParamAutoTuning.m_nFlowMethodType == 1)
                        {
                            sErrorCode = string.Format("{0}.005", sSubStepCodeName);
                            SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorCode, sErrorCode, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                            SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorMsg, m_sErrorMessage, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                            SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorMessage, m_sErrorMessage, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                        }
                    }

                    sResultMessage = "Error";
                }
                else
                    sResultMessage = "Finish";
            }
            else
                sResultMessage = "Error";

            if (bErrorFlag == true && m_sErrorMessage == "")
                m_sErrorMessage = sErrorMessage;

            if (m_sErrorMessage != "")
                OutputMessage(string.Format("-Error Message : {0}", m_sErrorMessage));

            OutputMessage(string.Format("-Record Data Set {0} {1}!!", nFlowIndex + 1, sResultMessage));
        }

        /// <summary>
        /// 確認Report Data有效筆數是否足夠
        /// </summary>
        /// <param name="nValidDataCount">有效筆數</param>
        /// <param name="cRobotParameter">線測機相關參數</param>
        /// <param name="byteReportData_List">Report Data</param>
        /// <param name="bSingleModeFlag">是否為單機模式</param>
        /// <returns>回傳錯誤Flag</returns>
        public int CheckDataNumberIsValid(ref int nValidDataCount, RobotParameter cRobotParameter, List<List<byte>> byteReportData_List, bool bSingleModeFlag = true)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            #region Noise, Tilt Noise
            if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO)
            {
                if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                {
                    if (eMainStep == MainTuningStep.NO)
                    {
                        List<byte> byteConvertReportData_List = new List<byte>();

                        int nDataLength = byteReportData_List.Count;
                        int nDataCount = 0;

                        for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                        {
                            byteConvertReportData_List.Clear();
                            byteConvertReportData_List = new List<byte>(byteReportData_List[nDataIndex]);

                            int nReportLength = byteConvertReportData_List.Count;

                            if (nReportLength != (ParamAutoTuning.m_nGen8ReportDataLength + ParamAutoTuning.m_nShiftByteNumber))
                                continue;

                            if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                                byteConvertReportData_List.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                            nDataCount++;
                        }

                        if (nDataCount < m_nNoiseValidReportNumber || nDataCount == 0)
                        {
                            nValidDataCount = nDataCount;
                            return 1;
                        }
                    }
                    else if (eMainStep == MainTuningStep.TILTNO)
                    {
                        List<byte> byteConvertReportData_List = new List<byte>();

                        int nDataLength = byteReportData_List.Count;
                        int nDataCount = 0;

                        for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                        {
                            byteConvertReportData_List.Clear();
                            byteConvertReportData_List = new List<byte>(byteReportData_List[nDataIndex]);

                            int nReportLength = byteConvertReportData_List.Count;

                            if (nReportLength != (ParamAutoTuning.m_nGen8ReportDataLength + ParamAutoTuning.m_nShiftByteNumber))
                                continue;

                            if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                                byteConvertReportData_List.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                            nDataCount++;
                        }

                        if (nDataCount < m_nNoiseValidReportNumber || nDataCount == 0)
                        {
                            nValidDataCount = nDataCount;
                            return 1;
                        }
                    }
                }
                else
                {
                    List<byte> byteConvertReportData_List = new List<byte>();

                    int nRXCount_Section1 = 0;
                    int nRXCount_Section2 = 0;
                    int nRXCount_Section3 = 0;
                    int nRXCount_Section4 = 0;

                    int nTXCount_Section1 = 0;
                    int nTXCount_Section2 = 0;

                    bool bGetRXTraceNumber = false;
                    bool bGetTXTraceNumber = false;
                    int nRXTraceNumber = 0;
                    int nTXTraceNumber = 0;

                    bool b800usDetectTime = true;

                    if (eMainStep == MainTuningStep.NO)
                    {
                        if (ParamAutoTuning.m_nAutoTune_P0_detect_time_Index != 1)
                            b800usDetectTime = false;
                    }

                    int nDataLength = byteReportData_List.Count;

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        byteConvertReportData_List.Clear();
                        byteConvertReportData_List = new List<byte>(byteReportData_List[nDataIndex]);

                        int nReportLength = byteConvertReportData_List.Count;
                        if (nReportLength != ParamAutoTuning.m_nReportDataLength)
                            continue;

                        if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                            byteConvertReportData_List.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                        if (byteConvertReportData_List[0] != 0x07 ||
                            byteConvertReportData_List[1] != 0x01 ||
                            byteConvertReportData_List[2] != 0xFF ||
                            byteConvertReportData_List[3] != 0xFF ||
                            byteConvertReportData_List[4] != 0xFF ||
                            byteConvertReportData_List[5] != 0xFF)
                            continue;

                        if ((b800usDetectTime == true && (byteConvertReportData_List[ParamAutoTuning.m_nAutoTuningInfoByte - 1] & 0x40) == 0) ||
                            (b800usDetectTime == false && (byteConvertReportData_List[ParamAutoTuning.m_nAutoTuningInfoByte - 1] & 0x40) != 0))
                            continue;

                        int nTraceTypeBitValue = Convert.ToInt32(Math.Pow(2, ParamAutoTuning.m_nTraceTypeBit - 1));
                        int nDataTypeBitValue = Convert.ToInt32(Math.Pow(2, ParamAutoTuning.m_nDataTypeBit - 1));
                        int nExecuteTypeBitValue = Convert.ToInt32(Math.Pow(2, ParamAutoTuning.m_nExecuteTypeBit - 1));
                        int nDataSectionValue = Convert.ToInt32(byteConvertReportData_List[ParamAutoTuning.m_nDataSectionByte - 1]);

                        if ((byteConvertReportData_List[ParamAutoTuning.m_nAutoTuningInfoByte - 1] & nTraceTypeBitValue) == 0)
                        {
                            if ((byteConvertReportData_List[ParamAutoTuning.m_nAutoTuningInfoByte - 1] & nDataTypeBitValue) != 1 &&
                                (byteConvertReportData_List[ParamAutoTuning.m_nAutoTuningInfoByte - 1] & nExecuteTypeBitValue) == 0)
                            {
                                int nTraceNumber = byteConvertReportData_List[ParamAutoTuning.m_nRTXTraceNumberByte - 1];

                                if (bGetRXTraceNumber == false)
                                {
                                    nRXTraceNumber = nTraceNumber;
                                    bGetRXTraceNumber = true;
                                }
                                else if (nRXTraceNumber != nTraceNumber)
                                    continue;

                                if (nDataSectionValue == 0)
                                    nRXCount_Section1++;
                                else if (nDataSectionValue == 1)
                                    nRXCount_Section2++;
                                else if (nDataSectionValue == 2)
                                    nRXCount_Section3++;
                                else if (nDataSectionValue == 3)
                                    nRXCount_Section4++;
                            }
                        }
                        else
                        {
                            if ((byteConvertReportData_List[ParamAutoTuning.m_nAutoTuningInfoByte - 1] & nDataTypeBitValue) != 1 &&
                                (byteConvertReportData_List[ParamAutoTuning.m_nAutoTuningInfoByte - 1] & nExecuteTypeBitValue) == 0)
                            {
                                int nTraceNumber = byteConvertReportData_List[ParamAutoTuning.m_nRTXTraceNumberByte - 1];

                                if (bGetTXTraceNumber == false)
                                {
                                    nTXTraceNumber = nTraceNumber;
                                    bGetTXTraceNumber = true;
                                }
                                else if (nTXTraceNumber != nTraceNumber)
                                    continue;

                                if (nDataSectionValue == 0)
                                    nTXCount_Section1++;
                                else if (nDataSectionValue == 1)
                                    nTXCount_Section2++;
                            }
                        }
                    }

                    int nRXSectionCount = (int)(nRXTraceNumber / 24);
                    int nRXSectionRem = nRXTraceNumber % 24;

                    if (nRXSectionRem > 0)
                        nRXSectionCount = nRXSectionCount + 1;

                    int nRXDataCount = nRXCount_Section1;

                    if (nRXSectionCount >= 2)
                        nRXDataCount = Math.Min(nRXDataCount, nRXCount_Section2);
                    if (nRXSectionCount >= 3)
                        nRXDataCount = Math.Min(nRXDataCount, nRXCount_Section3);
                    if (nRXSectionCount >= 4)
                        nRXDataCount = Math.Min(nRXDataCount, nRXCount_Section4);

                    int nTXSectionCount = (int)(nTXTraceNumber / 24);
                    int nTXSectionRem = nTXTraceNumber % 24;

                    if (nTXSectionRem > 0)
                        nTXSectionCount = nTXSectionCount + 1;

                    int nTXDataCount = nTXCount_Section1;

                    if (nTXSectionCount >= 2)
                        nTXDataCount = Math.Min(nTXDataCount, nTXCount_Section2);

                    if (nRXDataCount < m_nNoiseValidReportNumber || nRXDataCount == 0)
                    {
                        nValidDataCount = nRXDataCount;
                        return 1;
                    }

                    if (nTXDataCount < m_nNoiseValidReportNumber || nTXDataCount == 0)
                    {
                        nValidDataCount = nTXDataCount;
                        return 2;
                    }
                }
            }
            #endregion
            #region DigiGain Tuning
            else if (eMainStep == MainTuningStep.DIGIGAINTUNING)
            {
                const int nCHECKRXTYPE = 0x01;
                const int nCHECKTXTYPE = 0x02;
                int nCheckTypeFlag = 0;

                if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_HOR)
                    nCheckTypeFlag |= nCHECKRXTYPE;
                else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_VER)
                    nCheckTypeFlag |= nCHECKTXTYPE;
                else
                {
                    nCheckTypeFlag |= nCHECKRXTYPE;
                    nCheckTypeFlag |= nCHECKTXTYPE;
                }

                List<byte> byteConvertReportData_List = new List<byte>();

                //3 Type : Beacon/PTHF/BHF
                int[] nRXReportCount_Array = new int[3] 
                { 
                    0, 
                    0, 
                    0 
                };

                int[] nTXReportCount_Array = new int[3] 
                { 
                    0, 
                    0, 
                    0 
                };

                int nRXValidReportNumber = ParamAutoTuning.m_nDGTRXValidReportNumber;
                int nTXValidReportNumber = ParamAutoTuning.m_nDGTTXValidReportNumber;

                int nDataLength = byteReportData_List.Count;

                DigiGainDataFormat.DataByteLocation[] eDataByteLocation_Array = null;

                if (ParamAutoTuning.m_n5TRawDataType == 1)
                {
                    if ((nCheckTypeFlag & nCHECKRXTYPE) != 0 && (nCheckTypeFlag & nCHECKTXTYPE) != 0)
                    {
                        eDataByteLocation_Array = new DigiGainDataFormat.DataByteLocation[2] 
                        { 
                            DigiGainDataFormat.DataByteLocation.RX_TIP_NEWFORMAT,
                            DigiGainDataFormat.DataByteLocation.TX_TIP_NEWFORMAT 
                        };
                    }
                    else if ((nCheckTypeFlag & nCHECKRXTYPE) != 0)
                    {
                        eDataByteLocation_Array = new DigiGainDataFormat.DataByteLocation[1] 
                        { 
                            DigiGainDataFormat.DataByteLocation.RX_TIP_NEWFORMAT 
                        };
                    }
                    else if ((nCheckTypeFlag & nCHECKTXTYPE) != 0)
                    {
                        eDataByteLocation_Array = new DigiGainDataFormat.DataByteLocation[1] 
                        { 
                            DigiGainDataFormat.DataByteLocation.TX_TIP_NEWFORMAT 
                        };
                    }
                }
                else
                {
                    eDataByteLocation_Array = new DigiGainDataFormat.DataByteLocation[3] 
                    { 
                        DigiGainDataFormat.DataByteLocation.BEACON,
                        DigiGainDataFormat.DataByteLocation.PTHF,
                        DigiGainDataFormat.DataByteLocation.BHF 
                    };
                }

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    bool bTotalEnoughFlag = true;

                    if ((nCheckTypeFlag & nCHECKRXTYPE) != 0)
                    {
                        foreach (int nRXReportCount in nRXReportCount_Array)
                        {
                            if (nRXReportCount < nRXValidReportNumber)
                                bTotalEnoughFlag = false;
                        }
                    }

                    if ((nCheckTypeFlag & nCHECKTXTYPE) != 0)
                    {
                        foreach (int nTXReportCount in nTXReportCount_Array)
                        {
                            if (nTXReportCount < nTXValidReportNumber)
                                bTotalEnoughFlag = false;
                        }
                    }

                    if (bTotalEnoughFlag == true)
                        continue;

                    byteConvertReportData_List.Clear();
                    byteConvertReportData_List = new List<byte>(byteReportData_List[nDataIndex]);

                    int nReportLength = byteConvertReportData_List.Count;

                    if (nReportLength != ParamAutoTuning.m_nReportDataLength)
                        continue;

                    if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                        byteConvertReportData_List.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                    if (ParamAutoTuning.m_n5TRawDataType == 1)
                    {
                        if (byteConvertReportData_List[0] != 0x07)
                            continue;

                        if (byteConvertReportData_List[2] == 0x11 ||
                            byteConvertReportData_List[2] == 0x12 ||
                            byteConvertReportData_List[2] == 0x23)
                        {
                            int nDataTypeIndex = 0;

                            if (byteConvertReportData_List[2] == 0x11)
                                nDataTypeIndex = 0;
                            else if (byteConvertReportData_List[2] == 0x12)
                                nDataTypeIndex = 1;
                            else if (byteConvertReportData_List[2] == 0x23)
                                nDataTypeIndex = 2;

                            bool bEnoughFlag = true;

                            if ((nCheckTypeFlag & nCHECKRXTYPE) != 0)
                            {
                                if (nRXReportCount_Array[nDataTypeIndex] < nRXValidReportNumber)
                                    bEnoughFlag = false;
                            }

                            if ((nCheckTypeFlag & nCHECKTXTYPE) != 0)
                            {
                                if (nTXReportCount_Array[nDataTypeIndex] < nTXValidReportNumber)
                                    bEnoughFlag = false;
                            }

                            if (bEnoughFlag == true)
                                continue;

                            for (int nTraceTypeIndex = 0; nTraceTypeIndex < eDataByteLocation_Array.Length; nTraceTypeIndex++)
                            {
                                bool bAllByteZeroFlag = true;
                                int nStartByteIndex = (int)eDataByteLocation_Array[nTraceTypeIndex] - 1;

                                for (int nByteIndex = nStartByteIndex; nByteIndex < nStartByteIndex + 10; nByteIndex++)
                                {
                                    if (byteConvertReportData_List[nByteIndex] != 0x00)
                                    {
                                        bAllByteZeroFlag = false;
                                        break;
                                    }
                                }

                                if (bAllByteZeroFlag == false)
                                {
                                    if (eDataByteLocation_Array[nTraceTypeIndex] == DigiGainDataFormat.DataByteLocation.RX_TIP_NEWFORMAT)
                                        nRXReportCount_Array[nDataTypeIndex]++;
                                    else if (eDataByteLocation_Array[nTraceTypeIndex] == DigiGainDataFormat.DataByteLocation.TX_TIP_NEWFORMAT)
                                        nTXReportCount_Array[nDataTypeIndex]++;
                                }
                            }
                        }
                    }
                    else
                    {
                        int nTraceTypeByte = 13;

                        if (byteConvertReportData_List[0] != 0x07 ||
                            byteConvertReportData_List[2] != 0xFF ||
                            byteConvertReportData_List[3] != 0xFF ||
                            byteConvertReportData_List[4] != 0xFF ||
                            byteConvertReportData_List[5] != 0xFF ||
                            byteConvertReportData_List[9] != 0xDD ||
                            byteConvertReportData_List[10] != 0xEE)
                            continue;

                        int nTraceType = byteConvertReportData_List[nTraceTypeByte - 1];

                        //RX Trace Type
                        if ((nCheckTypeFlag & nCHECKRXTYPE) != 0)
                        {
                            if ((nTraceType & 0x01) == 0)
                            {
                                bool bEnoughFlag = true;

                                foreach (int nRXReportCount in nRXReportCount_Array)
                                {
                                    if (nRXReportCount < nRXValidReportNumber)
                                        bEnoughFlag = false;
                                }

                                if (bEnoughFlag == true)
                                    continue;

                                for (int nTypeIndex = 0; nTypeIndex < eDataByteLocation_Array.Length; nTypeIndex++)
                                {
                                    bool bAllByteZeroFlag = true;
                                    int nStartByteIndex = (int)eDataByteLocation_Array[nTypeIndex] - 1;

                                    for (int nByteIndex = nStartByteIndex; nByteIndex < nStartByteIndex + 10; nByteIndex++)
                                    {
                                        if (byteConvertReportData_List[nByteIndex] != 0x00)
                                        {
                                            bAllByteZeroFlag = false;
                                            break;
                                        }
                                    }

                                    if (bAllByteZeroFlag == false)
                                        nRXReportCount_Array[nTypeIndex]++;
                                }
                            }
                        }

                        //TX Trace Type
                        if ((nCheckTypeFlag & nCHECKTXTYPE) != 0)
                        {
                            bool bEnoughFlag = true;

                            foreach (int nTXReportCount in nTXReportCount_Array)
                            {
                                if (nTXReportCount < nTXValidReportNumber)
                                    bEnoughFlag = false;
                            }

                            if (bEnoughFlag == true)
                                continue;

                            for (int nTypeIndex = 0; nTypeIndex < eDataByteLocation_Array.Length; nTypeIndex++)
                            {
                                bool bAllByteZeroFlag = true;
                                int nStartByteIndex = (int)eDataByteLocation_Array[nTypeIndex] - 1;

                                for (int nByteIndex = nStartByteIndex; nByteIndex < nStartByteIndex + 10; nByteIndex++)
                                {
                                    if (byteConvertReportData_List[nByteIndex] != 0x00)
                                    {
                                        bAllByteZeroFlag = false;
                                        break;
                                    }
                                }

                                if (bAllByteZeroFlag == false)
                                    nTXReportCount_Array[nTypeIndex]++;
                            }
                        }
                    }
                }

                int nErrorReturnFlag = 0;

                if ((nCheckTypeFlag & nCHECKRXTYPE) != 0)
                {
                    for (int nTypeIndex = 0; nTypeIndex < nRXReportCount_Array.Length; nTypeIndex++)
                    {
                        nErrorReturnFlag++;

                        if (nRXReportCount_Array[nTypeIndex] < nRXValidReportNumber || nRXReportCount_Array[nTypeIndex] == 0)
                        {
                            nValidDataCount = nRXReportCount_Array[nTypeIndex];
                            return nErrorReturnFlag;
                        }
                    }
                }

                nErrorReturnFlag += nRXReportCount_Array.Length;

                if ((nCheckTypeFlag & nCHECKTXTYPE) != 0)
                {
                    for (int nTypeIndex = 0; nTypeIndex < nTXReportCount_Array.Length; nTypeIndex++)
                    {
                        nErrorReturnFlag++;

                        if (nTXReportCount_Array[nTypeIndex] < nTXValidReportNumber || nTXReportCount_Array[nTypeIndex] == 0)
                        {
                            nValidDataCount = nTXReportCount_Array[nTypeIndex];
                            return nErrorReturnFlag;
                        }
                    }
                }
            }
            #endregion
            #region TP_Gain Tuning
            else if (eMainStep == MainTuningStep.TPGAINTUNING)
            {
                const int nCHECKRXTYPE = 0x01;
                const int nCHECKTXTYPE = 0x02;
                int nCheckTypeFlag = 0;

                if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_HOR)
                    nCheckTypeFlag |= nCHECKRXTYPE;
                else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_VER)
                    nCheckTypeFlag |= nCHECKTXTYPE;
                else
                {
                    nCheckTypeFlag |= nCHECKRXTYPE;
                    nCheckTypeFlag |= nCHECKTXTYPE;
                }

                List<byte> byteConvertReportData_Array = new List<byte>();

                //2 Type : Tip/Ring
                int[] nRXReportCount_Array = new int[] 
                { 
                    0, 
                    0 
                };

                int[] nTXReportCount_Array = new int[] 
                { 
                    0, 
                    0 
                };

                int nRXValidReportNumber = ParamAutoTuning.m_nTPGTRXValidReportNumber;
                int nTXValidReportNumber = ParamAutoTuning.m_nTPGTTXValidReportNumber;

                int nDataLength = byteReportData_List.Count;

                TPGainDataFormat.DataByteLocation[] eDataByteLocation_Array = null;

                if ((nCheckTypeFlag & nCHECKRXTYPE) != 0 && (nCheckTypeFlag & nCHECKTXTYPE) != 0)
                {
                    eDataByteLocation_Array = new TPGainDataFormat.DataByteLocation[4] 
                    { 
                        TPGainDataFormat.DataByteLocation.RX_TIP,
                        TPGainDataFormat.DataByteLocation.TX_TIP,
                        TPGainDataFormat.DataByteLocation.RX_RING,
                        TPGainDataFormat.DataByteLocation.TX_RING 
                    };
                }
                else if ((nCheckTypeFlag & nCHECKRXTYPE) != 0)
                {
                    eDataByteLocation_Array = new TPGainDataFormat.DataByteLocation[2] 
                    { 
                        TPGainDataFormat.DataByteLocation.RX_TIP,
                        TPGainDataFormat.DataByteLocation.RX_RING 
                    };
                }
                else if ((nCheckTypeFlag & nCHECKTXTYPE) != 0)
                {
                    eDataByteLocation_Array = new TPGainDataFormat.DataByteLocation[2] 
                    { 
                        TPGainDataFormat.DataByteLocation.TX_TIP,
                        TPGainDataFormat.DataByteLocation.TX_RING 
                    };
                }

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    bool bTotalEnoughFlag = true;

                    if ((nCheckTypeFlag & nCHECKRXTYPE) != 0)
                    {
                        foreach (int nRXReportCount in nRXReportCount_Array)
                        {
                            if (nRXReportCount < nRXValidReportNumber)
                                bTotalEnoughFlag = false;
                        }
                    }

                    if ((nCheckTypeFlag & nCHECKTXTYPE) != 0)
                    {
                        foreach (int nTXReportCount in nTXReportCount_Array)
                        {
                            if (nTXReportCount < nTXValidReportNumber)
                                bTotalEnoughFlag = false;
                        }
                    }

                    if (bTotalEnoughFlag == true)
                        continue;

                    byteConvertReportData_Array.Clear();
                    byteConvertReportData_Array = new List<byte>(byteReportData_List[nDataIndex]);

                    int nReportLength = byteConvertReportData_Array.Count;

                    if (nReportLength != ParamAutoTuning.m_nReportDataLength)
                        continue;

                    if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                        byteConvertReportData_Array.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                    if (byteConvertReportData_Array[0] != 0x07)
                        continue;

                    if (byteConvertReportData_Array[2] == 0x23)
                    {
                        bool bEnoughFlag = true;

                        if ((nCheckTypeFlag & nCHECKRXTYPE) != 0)
                        {
                            foreach (int nRXReportCount in nRXReportCount_Array)
                            {
                                if (nRXReportCount < nRXValidReportNumber)
                                    bEnoughFlag = false;
                            }
                        }

                        if ((nCheckTypeFlag & nCHECKTXTYPE) != 0)
                        {
                            foreach (int nTXReportCount in nTXReportCount_Array)
                            {
                                if (nTXReportCount < nTXValidReportNumber)
                                    bEnoughFlag = false;
                            }
                        }

                        if (bEnoughFlag == true)
                            continue;

                        for (int nTraceTypeIndex = 0; nTraceTypeIndex < eDataByteLocation_Array.Length; nTraceTypeIndex++)
                        {
                            bool bAllByteZeroFlag = true;
                            int nStartByteIndex = (int)eDataByteLocation_Array[nTraceTypeIndex] - 1;

                            for (int nByteIndex = nStartByteIndex; nByteIndex < nStartByteIndex + 10; nByteIndex++)
                            {
                                if (byteConvertReportData_Array[nByteIndex] != 0x00)
                                {
                                    bAllByteZeroFlag = false;
                                    break;
                                }
                            }

                            if (bAllByteZeroFlag == false)
                            {
                                if (eDataByteLocation_Array[nTraceTypeIndex] == TPGainDataFormat.DataByteLocation.RX_TIP)
                                    nRXReportCount_Array[0]++;
                                else if (eDataByteLocation_Array[nTraceTypeIndex] == TPGainDataFormat.DataByteLocation.TX_TIP)
                                    nTXReportCount_Array[0]++;
                                else if (eDataByteLocation_Array[nTraceTypeIndex] == TPGainDataFormat.DataByteLocation.RX_RING)
                                    nRXReportCount_Array[1]++;
                                else if (eDataByteLocation_Array[nTraceTypeIndex] == TPGainDataFormat.DataByteLocation.TX_RING)
                                    nTXReportCount_Array[1]++;
                            }
                        }
                    }
                }

                int nErrorReturnFlag = 0;

                if ((nCheckTypeFlag & nCHECKRXTYPE) != 0)
                {
                    for (int nTypeIndex = 0; nTypeIndex < nRXReportCount_Array.Length; nTypeIndex++)
                    {
                        nErrorReturnFlag++;

                        if (nRXReportCount_Array[nTypeIndex] < nRXValidReportNumber || nRXReportCount_Array[nTypeIndex] == 0)
                        {
                            nValidDataCount = nRXReportCount_Array[nTypeIndex];
                            return nErrorReturnFlag;
                        }
                    }
                }

                nErrorReturnFlag += nRXReportCount_Array.Length;

                if ((nCheckTypeFlag & nCHECKTXTYPE) != 0)
                {
                    for (int nTypeIndex = 0; nTypeIndex < nTXReportCount_Array.Length; nTypeIndex++)
                    {
                        nErrorReturnFlag++;

                        if (nTXReportCount_Array[nTypeIndex] < nTXValidReportNumber || nTXReportCount_Array[nTypeIndex] == 0)
                        {
                            nValidDataCount = nTXReportCount_Array[nTypeIndex];
                            return nErrorReturnFlag;
                        }
                    }
                }
            }
            #endregion
            #region PeakCheck Tuning, Digital Tuning, Pressure Tuning->Pressure Protect
            else if (eMainStep == MainTuningStep.PEAKCHECKTUNING ||
                     eMainStep == MainTuningStep.DIGITALTUNING ||
                     (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSUREPROTECT))
            {
                #region PeakCheck Tuning, Digital Tuning(NoneSync)->Hover_1st/Hover_2nd/Contact
                if (eSubStep == SubTuningStep.HOVER_1ST ||
                    eSubStep == SubTuningStep.HOVER_2ND ||
                    eSubStep == SubTuningStep.CONTACT ||
                    eSubStep == SubTuningStep.PCHOVER_1ST ||
                    eSubStep == SubTuningStep.PCHOVER_2ND ||
                    eSubStep == SubTuningStep.PCCONTACT)
                {
                    List<byte> byteConvertReportData_List = new List<byte>();

                    List<List<int>> nOriginalData_Rx_List = new List<List<int>>();
                    List<List<int>> nOriginalData_Tx_List = new List<List<int>>();
                    List<int> nOriginalIndex_Rx_List = new List<int>();
                    List<int> nOriginalIndex_Tx_List = new List<int>();
                    List<int> nValidData_Rx_List = new List<int>();
                    List<int> nValidData_Tx_List = new List<int>();

                    int nValidReportDataCount = 0;

                    int nNormalReportDataLength = 14;
                    int nSectionNumber = 24;
                    int nIndexByte = 12;
                    int nTraceTypeByte = 13;
                    int nTraceNumberByte = 10;
                    //int nStraightUsefulDataNumber = ParamAutoTuning.m_nStraightUsefulDataNumber;

                    bool bGetRXTraceNumberFlag = false;
                    bool bGetTXTraceNumberFlag = false;
                    int nRXTraceNumber = 0;
                    int nTXTraceNumber = 0;
                    int nLeftBoundary = ParamAutoTuning.m_nDTValidReportEdgeNumber - 1;
                    int nRightBoundary = 0;
                    int nDataLength = byteReportData_List.Count;

                    bool b800usDetectTimeFlag = false;

                    if (eMainStep == MainTuningStep.PEAKCHECKTUNING)
                    {
                        if (ParamAutoTuning.m_nAutoTune_P0_detect_time_Index == 1)
                            b800usDetectTimeFlag = true;
                    }

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        byteConvertReportData_List.Clear();
                        byteConvertReportData_List = new List<byte>(byteReportData_List[nDataIndex]);

                        int nReportLength = byteConvertReportData_List.Count;

                        if (nReportLength != ParamAutoTuning.m_nReportDataLength)
                            continue;

                        if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                            byteConvertReportData_List.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                        if (byteConvertReportData_List[0] != 0x07 ||
                            byteConvertReportData_List[1] != 0x01 ||
                            byteConvertReportData_List[2] != 0xFF ||
                            byteConvertReportData_List[3] != 0xFF ||
                            byteConvertReportData_List[4] != 0xFF ||
                            byteConvertReportData_List[5] != 0xFF)
                            continue;

                        int nTraceTypeData = byteConvertReportData_List[nTraceTypeByte - 1];
                        int nIndexData = byteConvertReportData_List[nIndexByte - 1];

                        if ((b800usDetectTimeFlag == false && nTraceTypeData == MainConstantParameter.m_nGETDATATYPE_NONESYNCRX_400US) ||
                            (b800usDetectTimeFlag == true && nTraceTypeData == MainConstantParameter.m_nGETDATATYPE_NONESYNCRX_800US))
                        {
                            int nTraceNumber = byteConvertReportData_List[nTraceNumberByte - 1];

                            if (bGetRXTraceNumberFlag == false)
                            {
                                nRXTraceNumber = nTraceNumber;
                                nRightBoundary = nRXTraceNumber - ParamAutoTuning.m_nDTValidReportEdgeNumber - 1;
                                bGetRXTraceNumberFlag = true;
                            }
                            else
                            {
                                if (nTraceNumber != nRXTraceNumber)
                                    continue;
                            }

                            if (bSingleModeFlag == true)
                            {
                                if (nIndexData > nLeftBoundary && nIndexData < nRightBoundary)
                                    nValidReportDataCount++;

                                /*
                                if (nValidReportDataCount >= nNormalValidReportNumber)
                                    break;
                                */
                            }

                            int[] nData_Array = new int[nSectionNumber];

                            for (int nSectionIndex = 0; nSectionIndex < nSectionNumber; nSectionIndex++)
                                nData_Array[nSectionIndex] = (byteConvertReportData_List[2 * nSectionIndex + nNormalReportDataLength] * 256 + byteConvertReportData_List[2 * nSectionIndex + nNormalReportDataLength + 1]);

                            nOriginalData_Rx_List.Add(new List<int>(nData_Array));

                            if (nIndexData > (int)((nSectionNumber / 2) - 1) && nIndexData < nRXTraceNumber - (int)(nSectionNumber / 2))
                                nIndexData = (int)((nSectionNumber / 2) - 1);
                            else if (nIndexData >= nRXTraceNumber - (int)(nSectionNumber / 2))
                            {
                                int nValue = nRXTraceNumber - nIndexData;
                                nIndexData = nSectionNumber - nValue;
                            }

                            nOriginalIndex_Rx_List.Add(nIndexData);
                        }
                        else if ((b800usDetectTimeFlag == false && nTraceTypeData == MainConstantParameter.m_nGETDATATYPE_NONESYNCTX_400US) ||
                                 (b800usDetectTimeFlag == true && nTraceTypeData == MainConstantParameter.m_nGETDATATYPE_NONESYNCTX_800US))
                        {
                            int nTraceNumber = byteConvertReportData_List[nTraceNumberByte - 1];

                            if (bGetTXTraceNumberFlag == false)
                            {
                                nTXTraceNumber = nTraceNumber;
                                bGetTXTraceNumberFlag = true;
                            }
                            else
                            {
                                if (nTraceNumber != nTXTraceNumber)
                                    continue;
                            }

                            int[] nData_Array = new int[nSectionNumber];

                            for (int nSectionIndex = 0; nSectionIndex < nSectionNumber; nSectionIndex++)
                                nData_Array[nSectionIndex] = (byteConvertReportData_List[2 * nSectionIndex + nNormalReportDataLength] * 256 + byteConvertReportData_List[2 * nSectionIndex + nNormalReportDataLength + 1]);

                            nOriginalData_Tx_List.Add(new List<int>(nData_Array));

                            if (nIndexData > (int)((nSectionNumber / 2) - 1) && nIndexData < nTXTraceNumber - (int)(nSectionNumber / 2))
                                nIndexData = (int)((nSectionNumber / 2) - 1);
                            else if (nIndexData >= nTXTraceNumber - (int)(nSectionNumber / 2))
                            {
                                int nValue = nTXTraceNumber - nIndexData;
                                nIndexData = nSectionNumber - nValue;
                            }

                            nOriginalIndex_Tx_List.Add(nIndexData);
                        }
                    }

                    for (int nDataIndex = 0; nDataIndex < nOriginalData_Rx_List.Count; nDataIndex++)
                    {
                        if (nOriginalIndex_Rx_List[nDataIndex] <= nLeftBoundary || nOriginalIndex_Rx_List[nDataIndex] >= nRightBoundary)
                            continue;

                        int nMaxValue = nOriginalData_Rx_List[nDataIndex].Max();

                        /*
                        if (nOriginalData_Rx_List[nDataIndex][StraightUsefulDataNumber - 1] == nMaxValue && nOriginalData_Rx_List[nDataIndex][StraightUsefulDataNumber - 1] > m_CurrentParamSet.nRXFilterValue)
                            nValidData_Rx_List.Add(listnOriginalData_Rx[nDataIndex][StraightUsefulDataNumber - 1]);
                        */

                        if (nOriginalData_Rx_List[nDataIndex][nOriginalIndex_Rx_List[nDataIndex]] == nMaxValue &&
                            nOriginalData_Rx_List[nDataIndex][nOriginalIndex_Rx_List[nDataIndex]] > m_cCurrentParameterSet.m_nRXFilterValue)
                            nValidData_Rx_List.Add(nOriginalData_Rx_List[nDataIndex][nOriginalIndex_Rx_List[nDataIndex]]);
                    }

                    for (int nDataIndex = 0; nDataIndex < nOriginalData_Tx_List.Count; nDataIndex++)
                    {
                        if (nOriginalIndex_Tx_List[nDataIndex] <= nLeftBoundary || nOriginalIndex_Tx_List[nDataIndex] >= nRightBoundary)
                            continue;

                        int nMaxValue = nOriginalData_Tx_List[nDataIndex].Max();

                        /*
                        if (nOriginalData_Tx_List[nDataIndex][StraightUsefulDataNumber - 1] == nMaxValue && nOriginalData_Tx_List[nDataIndex][StraightUsefulDataNumber - 1] > m_CurrentParamSet.nTXFilterValue)
                            nValidData_Tx_List.Add(nOriginalData_Tx_List[nDataIndex][StraightUsefulDataNumber - 1]);
                        */

                        if (nOriginalData_Tx_List[nDataIndex][nOriginalIndex_Tx_List[nDataIndex]] == nMaxValue &&
                            nOriginalData_Tx_List[nDataIndex][nOriginalIndex_Tx_List[nDataIndex]] > m_cCurrentParameterSet.m_nTXFilterValue)
                            nValidData_Tx_List.Add(nOriginalData_Tx_List[nDataIndex][nOriginalIndex_Tx_List[nDataIndex]]);
                    }

                    if (nValidData_Rx_List.Count < m_nNormalFilterRXValidReportNumber || nValidData_Rx_List.Count == 0)
                    {
                        nValidDataCount = nValidData_Rx_List.Count;
                        return 2;
                    }

                    if (nValidData_Tx_List.Count < m_nNormalFilterTXValidReportNumber || nValidData_Tx_List.Count == 0)
                    {
                        nValidDataCount = nValidData_Tx_List.Count;
                        return 3;
                    }

                    if (bSingleModeFlag == true)
                    {
                        if (nValidReportDataCount < m_nNormalValidReportNumber || nValidReportDataCount == 0)
                        {
                            nValidDataCount = nValidReportDataCount;
                            return 1;
                        }
                    }
                }
                #endregion
                #region Digital Tuning(NoneSync)->HoverTRxS/ContactTRxS, Pressure Tuning->Pressure Protect
                if (eSubStep == SubTuningStep.HOVERTRxS ||
                    eSubStep == SubTuningStep.CONTACTTRxS ||
                    eSubStep == SubTuningStep.PRESSUREPROTECT)
                {
                    List<byte> byteConvertReportData_List = new List<byte>();

                    List<List<int>> nOriginalData_Rx_List = new List<List<int>>();
                    List<List<int>> nOriginalData_Tx_List = new List<List<int>>();
                    List<int> nOriginalIndex_Rx_List = new List<int>();
                    List<int> nOriginalIndex_Tx_List = new List<int>();
                    List<int> nValidData_Rx_List = new List<int>();
                    List<int> nValidData_Tx_List = new List<int>();

                    bool bDT7318TRxSSpecificReportTypeFlag = (ParamAutoTuning.m_nDT7318TRxSSpecificReportType == 1) ? true : false;

                    int nNormalReportDataLength = 14;
                    int nSectionNumber = 24;
                    int nTraceTypeByte = 13;
                    int nTraceIndexByte = 14;

                    if (bDT7318TRxSSpecificReportTypeFlag == true)
                    {
                        nNormalReportDataLength = 11;
                        nTraceTypeByte = 10;
                        nTraceIndexByte = 11;
                    }

                    int nRXTraceNumber = m_cCurrentParameterSet.m_nRXTraceNumber;
                    int nTXTraceNumber = m_cCurrentParameterSet.m_nTXTraceNumber;
                    int nLeftBoundary = ParamAutoTuning.m_nDTValidReportEdgeNumber - 1;
                    int nRightBoundary = nSectionNumber - ParamAutoTuning.m_nDTValidReportEdgeNumber;
                    int nRXValidReportNumber = m_nTRxSRXValidReportNumber;
                    int nTXValidReportNumber = m_nTRxSTXValidReportNumber;

                    if (eSubStep == SubTuningStep.PRESSUREPROTECT)
                    {
                        nLeftBoundary = ParamAutoTuning.m_nPTValidReportEdgeNumber - 1;
                        nRightBoundary = nSectionNumber - ParamAutoTuning.m_nPTValidReportEdgeNumber;
                        nRXValidReportNumber = m_nPPValidReportNumber;
                        nTXValidReportNumber = m_nPPValidReportNumber;
                    }

                    int nDataLength = byteReportData_List.Count;

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        byteConvertReportData_List.Clear();
                        byteConvertReportData_List = new List<byte>(byteReportData_List[nDataIndex]);

                        int nReportLength = byteConvertReportData_List.Count;

                        if (nReportLength != ParamAutoTuning.m_nReportDataLength)
                            continue;

                        if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                            byteConvertReportData_List.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                        if (byteConvertReportData_List[0] != 0x07 ||
                            byteConvertReportData_List[2] != 0xFF ||
                            byteConvertReportData_List[3] != 0xFF ||
                            byteConvertReportData_List[4] != 0xFF ||
                            byteConvertReportData_List[5] != 0xFF)
                            continue;

                        int nTraceTypeData = byteConvertReportData_List[nTraceTypeByte - 1];
                        int nIndexData = byteConvertReportData_List[nTraceIndexByte - 1];

                        if ((nTraceTypeData & 0x0F) == 0x08)
                        {
                            if (nIndexData >= nRXTraceNumber)
                                continue;

                            int[] nData_Array = new int[nSectionNumber];

                            for (int nSectionIndex = 0; nSectionIndex < nSectionNumber; nSectionIndex++)
                                nData_Array[nSectionIndex] = (byteConvertReportData_List[2 * nSectionIndex + nNormalReportDataLength] * 256 + byteConvertReportData_List[2 * nSectionIndex + nNormalReportDataLength + 1]);

                            nOriginalData_Rx_List.Add(new List<int>(nData_Array));

                            if (nIndexData > (int)((nSectionNumber / 2) - 1) && nIndexData < nRXTraceNumber - (int)(nSectionNumber / 2))
                                nIndexData = (int)((nSectionNumber / 2) - 1);
                            else if (nIndexData >= nRXTraceNumber - (int)(nSectionNumber / 2))
                            {
                                int nValue = nRXTraceNumber - nIndexData;
                                nIndexData = nSectionNumber - nValue;
                            }

                            nOriginalIndex_Rx_List.Add(nIndexData);
                        }
                        else if ((nTraceTypeData & 0x0F) == 0x00)
                        {
                            if (nIndexData >= nTXTraceNumber)
                                continue;

                            int[] nData_Array = new int[nSectionNumber];

                            for (int nSectionIndex = 0; nSectionIndex < nSectionNumber; nSectionIndex++)
                                nData_Array[nSectionIndex] = (byteConvertReportData_List[2 * nSectionIndex + nNormalReportDataLength] * 256 + byteConvertReportData_List[2 * nSectionIndex + nNormalReportDataLength + 1]);

                            nOriginalData_Tx_List.Add(new List<int>(nData_Array));

                            if (nIndexData > (int)((nSectionNumber / 2) - 1) && nIndexData < nTXTraceNumber - (int)(nSectionNumber / 2))
                                nIndexData = (int)((nSectionNumber / 2) - 1);
                            else if (nIndexData >= nTXTraceNumber - (int)(nSectionNumber / 2))
                            {
                                int nValue = nTXTraceNumber - nIndexData;
                                nIndexData = nSectionNumber - nValue;
                            }

                            nOriginalIndex_Tx_List.Add(nIndexData);
                        }
                    }

                    for (int nDataIndex = 0; nDataIndex < nOriginalData_Rx_List.Count; nDataIndex++)
                    {
                        if (nOriginalIndex_Rx_List[nDataIndex] <= nLeftBoundary || nOriginalIndex_Rx_List[nDataIndex] >= nRightBoundary)
                            continue;

                        int nMaxValue = nOriginalData_Rx_List[nDataIndex].Max();

                        if (nOriginalData_Rx_List[nDataIndex][nOriginalIndex_Rx_List[nDataIndex]] == nMaxValue)
                            nValidData_Rx_List.Add(nOriginalData_Rx_List[nDataIndex][nOriginalIndex_Rx_List[nDataIndex]]);
                    }

                    for (int nIndex = 0; nIndex < nOriginalData_Tx_List.Count; nIndex++)
                    {
                        if (nOriginalIndex_Tx_List[nIndex] <= nLeftBoundary || nOriginalIndex_Tx_List[nIndex] >= nRightBoundary)
                            continue;

                        int nMaxValue = nOriginalData_Tx_List[nIndex].Max();

                        if (nOriginalData_Tx_List[nIndex][nOriginalIndex_Tx_List[nIndex]] == nMaxValue)
                            nValidData_Tx_List.Add(nOriginalData_Tx_List[nIndex][nOriginalIndex_Tx_List[nIndex]]);
                    }

                    if (nValidData_Rx_List.Count < nRXValidReportNumber || nValidData_Rx_List.Count == 0)
                    {
                        nValidDataCount = nValidData_Rx_List.Count;
                        return 1;
                    }

                    if (nValidData_Tx_List.Count < nTXValidReportNumber || nValidData_Tx_List.Count == 0)
                    {
                        nValidDataCount = nValidData_Tx_List.Count;
                        return 2;
                    }
                }
                #endregion
            }
            #endregion
            #region Tilt Tuning
            else if (eMainStep == MainTuningStep.TILTTUNING)
            {
                if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_HOR ||
                    cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_VER)
                {
                    List<byte> byteConvertReportData_List = new List<byte>();

                    List<List<byte>> byteOriginalData_List = new List<List<byte>>();
                    List<int> nTipIndex_List = new List<int>();

                    int nDataLength = byteReportData_List.Count;

                    int nXTipIndexByte = 25;
                    int nYTipIndexByte = 47;

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        byteConvertReportData_List.Clear();
                        byteConvertReportData_List = new List<byte>(byteReportData_List[nDataIndex]);

                        int nReportLength = byteConvertReportData_List.Count;

                        if (nReportLength != ParamAutoTuning.m_nReportDataLength)
                            continue;

                        if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                            byteConvertReportData_List.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                        if (byteConvertReportData_List[0] != 0x07 ||
                            (eSubStep == SubTuningStep.TILTTUNING_PTHF && byteConvertReportData_List[13] != 0xAA) ||
                            (eSubStep == SubTuningStep.TILTTUNING_BHF && byteConvertReportData_List[13] != 0xBB))
                            continue;

                        byteOriginalData_List.Add(new List<byte>(byteConvertReportData_List));
                    }

                    if (byteOriginalData_List.Count == 0)
                    {
                        nValidDataCount = 0;
                        return 1;
                    }

                    nDataLength = byteOriginalData_List.Count;

                    int nTipIndexByte = -1;

                    if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_HOR)
                        nTipIndexByte = nXTipIndexByte;
                    else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_VER)
                        nTipIndexByte = nYTipIndexByte;

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        int nTipIndex = byteOriginalData_List[nDataIndex][nTipIndexByte - 1];

                        nTipIndex_List.Add(nTipIndex);
                    }

                    List<int> nSortTipIndex_List = GetListSortIndex(nTipIndex_List);

                    if (nSortTipIndex_List.Count < ParamAutoTuning.m_nTTValidTipTraceNumber)
                    {
                        nValidDataCount = nSortTipIndex_List.Count;
                        return 2;
                    }

                    nSortTipIndex_List.RemoveAt(nSortTipIndex_List.Count - 1);
                    nSortTipIndex_List.RemoveAt(0);

                    bool bGetUsefulTipIndexFlag = false;
                    int nMaxReportCount = 0;

                    for (int nSortIndex = 0; nSortIndex < nSortTipIndex_List.Count; nSortIndex++)
                    {
                        int nReportCount = 0;

                        for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                        {
                            if (nTipIndex_List[nDataIndex] == nSortTipIndex_List[nSortIndex])
                                nReportCount++;
                        }

                        if (nReportCount > nMaxReportCount)
                            nMaxReportCount = nReportCount;

                        if (nReportCount >= ParamAutoTuning.m_nTTPolyFitOrder + 1)
                        {
                            bGetUsefulTipIndexFlag = true;
                            break;
                        }
                    }

                    if (bGetUsefulTipIndexFlag == false)
                    {
                        nValidDataCount = nMaxReportCount;
                        return 3;
                    }
                }
                else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE)
                {
                    List<byte> byteConvertReportData_List = new List<byte>();

                    List<List<byte>> byteOriginalData_List = new List<List<byte>>();
                    List<int> nTipIndex_Rx_List = new List<int>();
                    List<int> nTipIndex_Tx_List = new List<int>();

                    int nDataLength = byteReportData_List.Count;

                    int nRXTraceNumber = m_cCurrentParameterSet.m_nRXTraceNumber;
                    int nTXTraceNumber = m_cCurrentParameterSet.m_nTXTraceNumber;
                    int nRXLeftBoundary = ParamAutoTuning.m_nTTValidReportEdgeNumber - 1;
                    int nRXRightBoundary = nTXTraceNumber - ParamAutoTuning.m_nTTValidReportEdgeNumber;
                    int nTXLeftBoundary = ParamAutoTuning.m_nTTValidReportEdgeNumber - 1;
                    int nTXRightBoundary = nRXTraceNumber - ParamAutoTuning.m_nTTValidReportEdgeNumber;

                    int nRXTipIndexByte = 25;
                    int nTXTipIndexByte = 47;

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        byteConvertReportData_List.Clear();
                        byteConvertReportData_List = new List<byte>(byteReportData_List[nDataIndex]);

                        int nReportLength = byteConvertReportData_List.Count;

                        if (nReportLength != ParamAutoTuning.m_nReportDataLength)
                            continue;

                        if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                            byteConvertReportData_List.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                        if (byteConvertReportData_List[0] != 0x07 ||
                            byteConvertReportData_List[2] != 0xFF ||
                            byteConvertReportData_List[3] != 0xFF ||
                            byteConvertReportData_List[4] != 0xFF ||
                            byteConvertReportData_List[5] != 0xFF ||
                            (eSubStep == SubTuningStep.TILTTUNING_PTHF && byteConvertReportData_List[13] != 0xAA) ||
                            (eSubStep == SubTuningStep.TILTTUNING_BHF && byteConvertReportData_List[13] != 0xBB))
                            continue;

                        byteOriginalData_List.Add(new List<byte>(byteConvertReportData_List));
                    }

                    if (byteOriginalData_List.Count == 0)
                    {
                        nValidDataCount = 0;
                        return 1;
                    }

                    nDataLength = byteOriginalData_List.Count;

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        int nRXTipIndex = byteOriginalData_List[nDataIndex][nRXTipIndexByte - 1];
                        int nTXTipIndex = byteOriginalData_List[nDataIndex][nTXTipIndexByte - 1];

                        nTipIndex_Rx_List.Add(nRXTipIndex);
                        nTipIndex_Tx_List.Add(nTXTipIndex);
                    }

                    int nRXValidReportCount = 0;

                    for (int nDataIndex = 0; nDataIndex < nTipIndex_Rx_List.Count; nDataIndex++)
                    {
                        if (nTipIndex_Rx_List[nDataIndex] <= nRXLeftBoundary || nTipIndex_Rx_List[nDataIndex] >= nRXRightBoundary)
                            continue;

                        nRXValidReportCount++;
                    }

                    int nTXValidReportCount = 0;

                    for (int nDataIndex = 0; nDataIndex < nTipIndex_Tx_List.Count; nDataIndex++)
                    {
                        if (nTipIndex_Tx_List[nDataIndex] <= nTXLeftBoundary || nTipIndex_Tx_List[nDataIndex] >= nTXRightBoundary)
                            continue;

                        nTXValidReportCount++;
                    }

                    if (nRXValidReportCount < m_nTTRXValidReportNumber || nTXValidReportCount == 0)
                    {
                        nValidDataCount = nRXValidReportCount;
                        return 4;
                    }

                    if (nTXValidReportCount < m_nTTTXValidReportNumber || nTXValidReportCount == 0)
                    {
                        nValidDataCount = nTXValidReportCount;
                        return 5;
                    }
                }
            }
            #endregion
            #region Pressure Tuning
            else if (eMainStep == MainTuningStep.PRESSURETUNING)
            {
                if (eSubStep == SubTuningStep.PRESSURESETTING ||
                    eSubStep == SubTuningStep.PRESSURETABLE)
                {
                    List<byte> byteConvertReportData_List = new List<byte>();

                    List<List<byte>> byteOriginalData_List = new List<List<byte>>();

                    int nDataLength = byteReportData_List.Count;

                    int nRealValidReportNumber = m_nPTStartSkipReportNumber + m_nPTValidReportNumber + m_nPTEndSkipReportNumber;

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        byteConvertReportData_List.Clear();
                        byteConvertReportData_List = new List<byte>(byteReportData_List[nDataIndex]);

                        int nReportLength = byteConvertReportData_List.Count;
                        if (nReportLength != ParamAutoTuning.m_nReportDataLength)
                            continue;

                        if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                            byteConvertReportData_List.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                        if (byteConvertReportData_List[0] != 0x07 ||
                            byteConvertReportData_List[2] != 0xFF ||
                            byteConvertReportData_List[3] != 0xFF ||
                            byteConvertReportData_List[4] != 0xFF ||
                            byteConvertReportData_List[5] != 0xFF ||
                            byteConvertReportData_List[9] != 0xFA ||
                            byteConvertReportData_List[10] != 0xFA)
                            continue;

                        if (eSubStep == SubTuningStep.PRESSURETABLE &&
                            //byteConvertReportData_List[1] != 0x03 ||
                            (byteConvertReportData_List[19] == 0x00 && byteConvertReportData_List[20] == 0x00))
                            continue;

                        byteOriginalData_List.Add(new List<byte>(byteConvertReportData_List));
                    }

                    if (byteOriginalData_List.Count < nRealValidReportNumber)
                    {
                        nValidDataCount = byteOriginalData_List.Count;
                        return 1;
                    }

                    if (eSubStep == SubTuningStep.PRESSURESETTING)
                    {
                        int nPressurePowerDataLocationByte = 17;

                        List<int> nPressurePowerData_List = new List<int>();

                        for (int nDataIndex = 0; nDataIndex < byteOriginalData_List.Count; nDataIndex++)
                        {
                            if (nDataIndex < m_nPTStartSkipReportNumber || nDataIndex > byteOriginalData_List.Count - m_nPTEndSkipReportNumber - 1)
                                continue;

                            int nValue = byteOriginalData_List[nDataIndex][nPressurePowerDataLocationByte] * 256 + byteOriginalData_List[nDataIndex][nPressurePowerDataLocationByte + 1];
                            nPressurePowerData_List.Add(nValue);
                        }

                        double dMeanValue = 0.0;

                        for (int nDataIndex = 0; nDataIndex < nPressurePowerData_List.Count; nDataIndex++)
                            dMeanValue += nPressurePowerData_List[nDataIndex];

                        int nMeanValue = (int)Math.Round(dMeanValue / nPressurePowerData_List.Count, 0, MidpointRounding.AwayFromZero);

                        nValidDataCount = nMeanValue;

                        if (nMeanValue < ParamAutoTuning.m_nPressMaxDFTRxRefValueLB)
                            return 2;
                        else if (nMeanValue > ParamAutoTuning.m_nPressMaxDFTRxRefValueHB)
                            return 3;
                    }
                }
            }
            #endregion
            #region Linearity Tuning
            else if (eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_HOR ||
                    cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_VER)
                {
                    List<byte> byteConvertReportData_List = new List<byte>();

                    List<List<byte>> byteOriginalData_List = new List<List<byte>>();

                    int nDataLength = byteReportData_List.Count;

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        byteConvertReportData_List.Clear();
                        byteConvertReportData_List = new List<byte>(byteReportData_List[nDataIndex]);

                        int nReportLength = byteConvertReportData_List.Count;

                        if (nReportLength != ParamAutoTuning.m_nReportDataLength)
                            continue;

                        if (ParamAutoTuning.m_nShiftStartByte > 0 && ParamAutoTuning.m_nShiftByteNumber > 0)
                            byteConvertReportData_List.RemoveRange(ParamAutoTuning.m_nShiftStartByte - 1, ParamAutoTuning.m_nShiftByteNumber);

                        if (ParamAutoTuning.m_n5TRawDataType == 1)
                        {
                            if (byteConvertReportData_List[0] != 0x07)
                                continue;

                            if (byteConvertReportData_List[2] != 0x11 &&
                                byteConvertReportData_List[2] != 0x12 &&
                                byteConvertReportData_List[2] != 0x23)
                                continue;
                        }
                        else
                        {
                            if (byteConvertReportData_List[0] != 0x07 ||
                                byteConvertReportData_List[2] != 0xFF ||
                                byteConvertReportData_List[3] != 0xFF ||
                                byteConvertReportData_List[4] != 0xFF ||
                                byteConvertReportData_List[5] != 0xFF ||
                                byteConvertReportData_List[9] != 0x17)
                                continue;
                        }

                        byteOriginalData_List.Add(new List<byte>(byteConvertReportData_List));
                    }

                    if (byteOriginalData_List.Count == 0)
                    {
                        nValidDataCount = 0;
                        return 1;
                    }

                    int nTraceNumber = m_cCurrentParameterSet.m_nRXTraceNumber;

                    if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_HOR)
                        nTraceNumber = m_cCurrentParameterSet.m_nRXTraceNumber;
                    else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_VER)
                        nTraceNumber = m_cCurrentParameterSet.m_nTXTraceNumber;

                    int nResultFlag = CheckAllLTFormat(ref nValidDataCount, byteOriginalData_List, cRobotParameter.m_eRobotType, nTraceNumber);

                    if (nResultFlag != -1)
                        return nResultFlag;
                }
            }
            #endregion

            return -1;
        }

        /// <summary>
        /// 確認所有線性Table的格式是否正確
        /// </summary>
        /// <param name="nValidDataCount">有效Report Data筆數</param>
        /// <param name="byteOriginalData_List">原本的Report Data陣列</param>
        /// <param name="eRobotType">線測機畫線種類</param>
        /// <param name="nTraceNumber">Trace數目</param>
        /// <returns>回傳錯誤Flag</returns>
        private int CheckAllLTFormat(ref int nValidDataCount, List<List<byte>> byteOriginalData_List, FlowRobot eRobotType, int nTraceNumber)
        {
            int nByteLocation_RXTraceIndex = 24;
            int nByteLocation_TXTraceIndex = 38;

            if (ParamAutoTuning.m_n5TRawDataType == 1)
            {
                nByteLocation_RXTraceIndex = 16;
                nByteLocation_TXTraceIndex = 29;
            }

            int nTraceIndexByteLocation = nByteLocation_RXTraceIndex;

            if (eRobotType == FlowRobot.TOUCHLINE_HOR)
                nTraceIndexByteLocation = nByteLocation_RXTraceIndex;
            else if (eRobotType == FlowRobot.TOUCHLINE_VER)
                nTraceIndexByteLocation = nByteLocation_TXTraceIndex;

            List<TracePartData> cTracePartData_List = new List<TracePartData>();

            int nMaxTraceIndex = -1;

            for (int nDataIndex = 0; nDataIndex < byteOriginalData_List.Count; nDataIndex++)
            {
                if (byteOriginalData_List[nDataIndex][nTraceIndexByteLocation - 1] == 0xFF)
                    continue;

                int nTraceIndex = byteOriginalData_List[nDataIndex][nTraceIndexByteLocation - 1];

                if (nTraceIndex > nMaxTraceIndex)
                    nMaxTraceIndex = nTraceIndex;
            }

            for (int nTraceIndex = 0; nTraceIndex < nMaxTraceIndex + 1; nTraceIndex++)
            {
                TracePartData cTracePartData = new TracePartData();
                cTracePartData.m_nTraceIndex = -1;
                //cTracePartData.m_nTraceRawDataCount = 0;
                cTracePartData_List.Add(cTracePartData);
            }

            for (int nDataIndex = 0; nDataIndex < byteOriginalData_List.Count; nDataIndex++)
            {
                if (byteOriginalData_List[nDataIndex][nTraceIndexByteLocation - 1] == 0xFF)
                    continue;

                int nTraceIndex = byteOriginalData_List[nDataIndex][nTraceIndexByteLocation - 1];

                if (cTracePartData_List[nTraceIndex].m_nTraceIndex < 0)
                    cTracePartData_List[nTraceIndex].m_nTraceIndex = nTraceIndex;

                //cTracePartData_List[nTraceIndex].m_nTraceRawDataCount++;
            }

            if (cTracePartData_List == null || cTracePartData_List.Count == 0)
            {
                nValidDataCount = 0;
                return 2;
            }
            else
            {
                int nFirstEdgeAreaNumber = (1 < ParamAutoTuning.m_nLTFirstEdgeAreaValidNumber) ? ParamAutoTuning.m_nLTFirstEdgeAreaValidNumber - 1 : 1 - 1;
                int nLastEdgeAreaNumber = (nTraceNumber - 1 > nTraceNumber - ParamAutoTuning.m_nLTLastEdgeAreaValidNumber) ? nTraceNumber - ParamAutoTuning.m_nLTLastEdgeAreaValidNumber : nTraceNumber - 1;
                int nCount = 0;

                for (int nDataIndex = 0; nDataIndex < cTracePartData_List.Count; nDataIndex++)
                {
                    if (cTracePartData_List[nDataIndex].m_nTraceIndex > nFirstEdgeAreaNumber && cTracePartData_List[nDataIndex].m_nTraceIndex < nLastEdgeAreaNumber)
                        nCount++;
                }

                if (nCount == 0)
                {
                    nValidDataCount = 0;
                    return 3;
                }

                /*
                for (int nDataIndex = 0; nDataIndex < cTracePartData_List.Count; nDataIndex++)
                {
                    if (cTracePartData_List[nDataIndex].m_nTraceIndex > StartEdgeNumber && cTracePartData_List[nDataIndex].m_nTraceIndex < EndEdgeNumber)
                    {
                        if (cTracePartData_List[nDataIndex].m_nTraceRawDataCount < SIZE_64PARTS)
                        {
                            nValidDataCount = nIndex;
                            return 4;
                        }
                    }
                }
                */
            }

            return -1;
        }

        /// <summary>
        /// List排序
        /// </summary>
        /// <param name="nData_List">List Data</param>
        /// <returns>回傳排序後的List Data</returns>
        private List<int> GetListSortIndex(List<int> nData_List)
        {
            List<int> nSortData_List = new List<int>(nData_List);

            for (int nDataIndex = 0; nDataIndex < nSortData_List.Count; nDataIndex++)
            {
                for (int nCompareIndex = nSortData_List.Count - 1; nCompareIndex > nDataIndex; nCompareIndex--)
                {

                    if (nSortData_List[nDataIndex] == nSortData_List[nCompareIndex])
                        nSortData_List.RemoveAt(nCompareIndex);
                }
            }

            nSortData_List.Sort();

            return nSortData_List;
        }

        /// <summary>
        /// 寫入Report Data檔案(.txt)
        /// </summary>
        /// <param name="nRecordDataErrorFlag">錄製Report Data錯誤Flag</param>
        /// <param name="byteReportData_List">Report Data</param>
        /// <param name="cRobotParameter">線測機相關資訊</param>
        private void WriteReportData(ref int nRecordDataErrorFlag, List<List<byte>> byteReportData_List, RobotParameter cRobotParameter)
        {
            bool bWriteErrorFlag = false;
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            string sFilePath = string.Format("{0}.txt", m_sLogFilePath);

            Dictionary<string, string> dictRecordInfoMappingTable = DefineRecordInfoMappingTable(cRobotParameter);

            bool bRemoveSpecificDataFlag = false;
            FileStream fsFile;

            string sDataTitle = GetDataTitle(eMainStep, eSubStep);

            if (m_bRetryStateFlag == true)
            {
                if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                    eMainStep == MainTuningStep.TPGAINTUNING ||
                    eMainStep == MainTuningStep.TILTTUNING ||
                    eMainStep == MainTuningStep.LINEARITYTUNING ||
                    (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURETABLE))
                    bRemoveSpecificDataFlag = true;
                else if ((eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO) && m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    bRemoveSpecificDataFlag = true;

                if (bRemoveSpecificDataFlag == true)
                {
                    StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);
                    string sData = srFile.ReadToEnd();
                    srFile.Close();

                    int nRemoveStringIndex = sData.IndexOf(sDataTitle);

                    if (nRemoveStringIndex >= 0)
                    {
                        int nRemoveStringCount = sData.Length - nRemoveStringIndex;
                        sData = sData.Remove(nRemoveStringIndex, nRemoveStringCount);

                        StreamWriter sw = new StreamWriter(sFilePath, false, Encoding.Default);

                        try
                        {
                            sw.Write(sData);
                        }
                        finally
                        {
                            sw.Close();
                        }
                    }

                    fsFile = new FileStream(sFilePath, FileMode.Append, FileAccess.Write);
                }
                else
                    fsFile = new FileStream(sFilePath, FileMode.Create, FileAccess.Write);
            }
            else
            {
                if ((eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO) && m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    fsFile = new FileStream(sFilePath, FileMode.Append, FileAccess.Write);
                else if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                         eMainStep == MainTuningStep.TPGAINTUNING ||
                         eMainStep == MainTuningStep.TILTTUNING ||
                         eMainStep == MainTuningStep.LINEARITYTUNING ||
                         (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURETABLE))
                    fsFile = new FileStream(sFilePath, FileMode.Append, FileAccess.Write);
                else
                    fsFile = new FileStream(sFilePath, FileMode.Create, FileAccess.Write);
            }

            StreamWriter swFile = new StreamWriter(fsFile);

            if (sDataTitle != "")
                swFile.WriteLine(sDataTitle);

            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_FWCHECKVERSION);
            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SUBSTEP);
            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SETTINGPH1);
            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SETTINGPH2);
            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_READPH1);
            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_READPH2);
            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_FREQUENCY);
            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_REPORTNUMBER, true);

            if (eMainStep == MainTuningStep.PEAKCHECKTUNING || eMainStep == MainTuningStep.DIGITALTUNING)
            {
                if (//PeakCheck Tuning
                    eSubStep == SubTuningStep.PCHOVER_1ST ||
                    eSubStep == SubTuningStep.PCHOVER_2ND ||
                    //Digital Tuning
                    eSubStep == SubTuningStep.HOVER_1ST ||
                    eSubStep == SubTuningStep.HOVER_2ND ||
                    eSubStep == SubTuningStep.HOVERTRxS)
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_HOVERRAISEHEIGHT, true);
            }

            if (eMainStep == MainTuningStep.DIGITALTUNING)
            {
                if (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_7318TRXSSPECIFICREPORTTYPE, true);
            }

            if (eMainStep == MainTuningStep.PRESSURETUNING)
            {
                if (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE)
                {
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_PRESSUREWEIGHT);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_REALITYWEIGHT);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_OFFSETWEIGHT);

                    if (eSubStep == SubTuningStep.PRESSURETABLE)
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_EXTRAINCWEIGHT);

                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_TOTALWEIGHT, true);
                }
                else if (eSubStep == SubTuningStep.PRESSUREPROTECT)
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_HOVERRAISEHEIGHT, true);
            }

            if (eMainStep != MainTuningStep.NO)
            {
                if (eSubStep != SubTuningStep.PRESSURETABLE)
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RANKINDEX, true);
                else
                {
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RANKINDEX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_PTPENVERSION, true);
                }
            }

            if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.PEAKCHECKTUNING || eMainStep == MainTuningStep.DIGITALTUNING)
            {
                if (//Noise
                    eSubStep == SubTuningStep.NO ||
                    //PeakCheck Tuning
                    eSubStep == SubTuningStep.PCHOVER_1ST ||
                    eSubStep == SubTuningStep.PCHOVER_2ND ||
                    eSubStep == SubTuningStep.PCCONTACT ||
                    //Digital Tuning
                    eSubStep == SubTuningStep.HOVER_1ST ||
                    eSubStep == SubTuningStep.HOVER_2ND ||
                    eSubStep == SubTuningStep.CONTACT)
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_P0_DETECT_TIME, true);
            }

            CheckState cCheckState = new CheckState(m_cfrmMain);

            if (cCheckState.CheckSetDigiGain(eMainStep, eSubStep) != CheckState.m_nSETDIGIGAIN_DISABLE)
            {
                if (m_nDigiGainCommandState == m_nDIGIGAINCOMMAND_ENABLE)
                {
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SDIGIGAIN_P0);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RDIGIGAIN_P0, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SDIGIGAIN_BEACON_RX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SDIGIGAIN_BEACON_TX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SDIGIGAIN_PTHF_RX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RDIGIGAIN_PTHF_RX, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SDIGIGAIN_PTHF_TX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RDIGIGAIN_PTHF_TX, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SDIGIGAIN_BHF_RX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RDIGIGAIN_BHF_RX, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SDIGIGAIN_BHF_TX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RDIGIGAIN_BHF_TX, true);
                }
            }

            if (eMainStep == MainTuningStep.PEAKCHECKTUNING || eMainStep == MainTuningStep.DIGITALTUNING)
            {
                //PeakCheck Tuning
                if (eSubStep == SubTuningStep.PCHOVER_1ST ||
                    eSubStep == SubTuningStep.PCHOVER_2ND ||
                    eSubStep == SubTuningStep.PCCONTACT)
                {
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISERXINNERMAX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISEP0_DETECT_TIME_INDEX, true);

                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_RX, true);
                }
                //Digital Tuning
                else if (eSubStep == SubTuningStep.HOVER_1ST ||
                         eSubStep == SubTuningStep.HOVER_2ND ||
                         eSubStep == SubTuningStep.CONTACT)
                {
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISERXINNERMAX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISETXINNERMAX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISERXINMAXPLUS3INMAXSTD);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISEP0_DETECT_TIME_INDEX, true);

                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_1TR);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_2TR);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_3TR);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_4TR, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISEDIGIGAIN_P0);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_RX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_TX, true);
                }
                else
                {
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISERXINNERMAX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_NOISETXINNERMAX, true);
                }
            }

            if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                eMainStep == MainTuningStep.TPGAINTUNING ||
                (eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)) ||
                eMainStep == MainTuningStep.TILTTUNING || eMainStep == MainTuningStep.PRESSURETUNING || eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SP0_TH);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RP0_TH, true);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_STRXS_HOVER_TH_RX);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RTRXS_HOVER_TH_RX, true);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_STRXS_HOVER_TH_TX);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RTRXS_HOVER_TH_TX, true);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_STRXS_CONTACT_TH_RX);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RTRXS_CONTACT_TH_RX, true);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_STRXS_CONTACT_TH_TX);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RTRXS_CONTACT_TH_TX, true);

                if (ParamAutoTuning.m_nFWTypeIndex != 1)
                {
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SPEN_HI_HF_THD);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RPEN_HI_HF_THD, true);
                }

                if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                    eMainStep == MainTuningStep.TPGAINTUNING ||
                    (eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)))
                {
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SEDGE_1TRC_SUBPWR);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_REDGE_1TRC_SUBPWR, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SEDGE_2TRC_SUBPWR);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_REDGE_2TRC_SUBPWR, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SEDGE_3TRC_SUBPWR);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_REDGE_3TRC_SUBPWR, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SEDGE_4TRC_SUBPWR);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_REDGE_4TRC_SUBPWR, true);
                }

                if (eMainStep == MainTuningStep.DIGIGAINTUNING || eMainStep == MainTuningStep.TPGAINTUNING ||
                    eMainStep == MainTuningStep.TILTTUNING ||
                    eMainStep == MainTuningStep.PRESSURETUNING || eMainStep == MainTuningStep.LINEARITYTUNING)
                {
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SHOVER_TH_RX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RHOVER_TH_RX, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SHOVER_TH_TX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RHOVER_TH_TX, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SCONTACT_TH_RX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RCONTACT_TH_RX, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SCONTACT_TH_TX);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RCONTACT_TH_TX, true);

                    if (eSubStep == SubTuningStep.DIGIGAIN ||
                        eSubStep == SubTuningStep.TP_GAIN ||
                        eSubStep == SubTuningStep.TILTTUNING_PTHF)
                    {
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SPTHF_CONTACT_TH_RX);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RPTHF_CONTACT_TH_RX, true);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SPTHF_CONTACT_TH_TX);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RPTHF_CONTACT_TH_TX, true);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SPTHF_HOVER_TH_RX);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RPTHF_HOVER_TH_RX, true);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SPTHF_HOVER_TH_TX);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RPTHF_HOVER_TH_TX, true);
                    }

                    if (eSubStep == SubTuningStep.DIGIGAIN ||
                        eSubStep == SubTuningStep.TP_GAIN ||
                        eSubStep == SubTuningStep.TILTTUNING_BHF)
                    {
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SBHF_CONTACT_TH_RX);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RBHF_CONTACT_TH_RX, true);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SBHF_CONTACT_TH_TX);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RBHF_CONTACT_TH_TX, true);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SBHF_HOVER_TH_RX);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RBHF_HOVER_TH_RX, true);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SBHF_HOVER_TH_TX);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RBHF_HOVER_TH_TX, true);
                    }

                    if (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE)
                    {
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SIQ_BSH_P);
                        WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RIQ_BSH_P, true);

                        if (eSubStep == SubTuningStep.PRESSURESETTING)
                        {
                            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_PRESS_MAXDFTRXMEAN);
                            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_BEFIQ_BSH_P);
                            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_BEFPRESS_MAXDFTRXMEAN, true);
                        }
                        else if (eSubStep == SubTuningStep.PRESSURETABLE)
                        {
                            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SPRESSURE3BINSTH);
                            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RPRESSURE3BINSTH, true);
                            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_SPRESS_3BINSPWR);
                            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RPRESS_3BINSPWR, true);
                        }
                    }
                }
            }

            if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO)
            {
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RXTRACENUMBER);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_TXTRACENUMBER, true);

                if (eMainStep == MainTuningStep.TILTNO && m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                {
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RXSTARTTRACE, true);
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_TXSTARTTRACE, true);
                }
            }

            if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                eMainStep == MainTuningStep.TPGAINTUNING ||
                eMainStep == MainTuningStep.TILTTUNING ||
                eMainStep == MainTuningStep.PRESSURETUNING ||
                eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_RXTRACENUMBER);
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_TXTRACENUMBER, true);

                if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                    eMainStep == MainTuningStep.TPGAINTUNING ||
                    eMainStep == MainTuningStep.TILTTUNING ||
                    eMainStep == MainTuningStep.LINEARITYTUNING)
                    WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_DRAWLINETYPE, true);
            }

            WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_CONTROLMODE, true);

            if ((eMainStep == MainTuningStep.DIGIGAINTUNING && eSubStep == SubTuningStep.DIGIGAIN) ||
                (eMainStep == MainTuningStep.TPGAINTUNING && eSubStep == SubTuningStep.TP_GAIN) ||
                (eMainStep == MainTuningStep.LINEARITYTUNING && eSubStep == SubTuningStep.LINEARITYTABLE))
                WriteInfo(ref bWriteErrorFlag, swFile, dictRecordInfoMappingTable, StringConvert.m_sRECORD_5TRAWDATATYPE, true);

            for (int nReportIndex = 0; nReportIndex < byteReportData_List.Count; nReportIndex++)
            {
                string sReportText = "";

                int nReportLength = byteReportData_List[nReportIndex].Count;

                for (int nByteIndex = 0; nByteIndex < nReportLength; nByteIndex++)
                    sReportText += byteReportData_List[nReportIndex][nByteIndex].ToString("X2") + " ";

                swFile.WriteLine(sReportText);
            }

            swFile.Flush();
            swFile.Close();
            swFile.Close();

            if (nRecordDataErrorFlag == -1 && bWriteErrorFlag == true)
                nRecordDataErrorFlag = 4;
        }

        private string GetDataTitle(MainTuningStep eMainStep, SubTuningStep eSubStep)
        {
            string sDataTitle = "";

            if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                eMainStep == MainTuningStep.TPGAINTUNING ||
                eMainStep == MainTuningStep.TILTTUNING ||
                eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                sDataTitle = string.Format("====={0}=====", m_cCurrentParameterSet.m_sDrawLineType);
            }
            else if (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURETABLE)
            {
                sDataTitle = string.Format("====={0}g=====", m_cCurrentParameterSet.m_nPressureWeight);
            }
            else if ((eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO) && m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
            {
                if (eMainStep == MainTuningStep.NO)
                {
                    if (m_cCurrentParameterSet.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                        sDataTitle = "=====RX=====";
                    else if (m_cCurrentParameterSet.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                        sDataTitle = "=====TX=====";
                }
                else if (eMainStep == MainTuningStep.TILTNO)
                {
                    if (m_cCurrentParameterSet.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                    {
                        if (m_cCurrentParameterSet.m_nSection >= 0 && m_cCurrentParameterSet.m_nSection < 4)
                        {
                            string sSection = string.Format("S{0}", m_cCurrentParameterSet.m_nSection);
                            sDataTitle = string.Format("=====RX {0}=====", sSection);
                        }
                        else
                            sDataTitle = "=====RX=====";
                    }
                    else if (m_cCurrentParameterSet.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                        sDataTitle = "=====TX=====";
                }
            }

            return sDataTitle;
        }

        /// <summary>
        /// 定義RecordInfo的Dictionary Table
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊</param>
        /// <returns>回傳Dictionary</returns>
        private Dictionary<string, string> DefineRecordInfoMappingTable(RobotParameter cRobotParameter)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            m_cCurrentParameterSet.m_sFWCheckVersion = m_cCurrentParameterSet.m_nFWCheckVersion.ToString("x2").PadLeft(2, '0').ToUpper();

            if (m_cCurrentParameterSet.m_nFWCheckVersion < 0)
                m_cCurrentParameterSet.m_sFWCheckVersion = m_cCurrentParameterSet.m_nFWCheckVersion.ToString();

            m_cCurrentParameterSet.m_sSubStep = eSubStep.ToString();
            m_cCurrentParameterSet.m_sFrequency = ElanConvert.ComputeFrequnecyToString(m_cCurrentParameterSet.m_nReadPH1, m_cCurrentParameterSet.m_nReadPH2);
            m_cCurrentParameterSet.m_sControlMode = StringConvert.m_dictControlModeMappingTable[m_cfrmMain.m_nModeFlag];

            if (eMainStep == MainTuningStep.PEAKCHECKTUNING || eMainStep == MainTuningStep.DIGITALTUNING)
            {
                if (eSubStep == SubTuningStep.HOVER_1ST || eSubStep == SubTuningStep.HOVER_2ND || eSubStep == SubTuningStep.HOVERTRxS)
                {
                    if (eSubStep == SubTuningStep.HOVER_2ND)
                        m_cCurrentParameterSet.m_sHoverRaiseHeight = ParamAutoTuning.m_dHoverHeight_DT2nd.ToString();
                    else
                        m_cCurrentParameterSet.m_sHoverRaiseHeight = ParamAutoTuning.m_dHoverHeight_DT1st.ToString();
                }
                else if (eSubStep == SubTuningStep.PCHOVER_1ST)
                    m_cCurrentParameterSet.m_sHoverRaiseHeight = ParamAutoTuning.m_dHoverHeight_PCT1st.ToString();
                else if (eSubStep == SubTuningStep.PCHOVER_2ND)
                    m_cCurrentParameterSet.m_sHoverRaiseHeight = ParamAutoTuning.m_dHoverHeight_PCT2nd.ToString();
            }

            if (eMainStep == MainTuningStep.PRESSURETUNING)
            {
                if (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE)
                {
                    m_cCurrentParameterSet.m_nPressureWeight = cRobotParameter.m_nPressureWeight;

                    if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                    {
                        int nWeightIndex = Array.FindIndex(ParamAutoTuning.m_nPressureWeight_Array, nIndex => nIndex == cRobotParameter.m_nPressureWeight);
                        int nMiddleValue = (int)((double)(m_cCurrentParameterSet.m_nStartWeight + m_cCurrentParameterSet.m_nEndWeight) / 2);
                        int n25gIndex = Array.FindIndex(ParamAutoTuning.m_nPressureWeight_Array, nIndex => nIndex == 25);

                        if (nWeightIndex <= n25gIndex)
                            nMiddleValue = (m_cCurrentParameterSet.m_nStartWeight < m_cCurrentParameterSet.m_nEndWeight) ? m_cCurrentParameterSet.m_nStartWeight : m_cCurrentParameterSet.m_nEndWeight;

                        int nRealValue = nMiddleValue - m_cCurrentParameterSet.m_nOffsetWeight;

                        if (ParamAutoTuning.m_nPTPenVersion == 1)
                        {
                            switch (cRobotParameter.m_nPressureWeight)
                            {
                                case 25:
                                    m_cCurrentParameterSet.m_nExtraIncWeight = ParamAutoTuning.m_nPTExtraIncWeight_25G;
                                    break;
                                case 50:
                                    m_cCurrentParameterSet.m_nExtraIncWeight = ParamAutoTuning.m_nPTExtraIncWeight_50G;
                                    break;
                                case 75:
                                    m_cCurrentParameterSet.m_nExtraIncWeight = ParamAutoTuning.m_nPTExtraIncWeight_75G;
                                    break;
                                case 100:
                                    m_cCurrentParameterSet.m_nExtraIncWeight = ParamAutoTuning.m_nPTExtraIncWeight_100G;
                                    break;
                                default:
                                    m_cCurrentParameterSet.m_nExtraIncWeight = 0;
                                    break;
                            }

                            m_cCurrentParameterSet.m_nPTPenVersion = 1;
                        }
                        else
                        {
                            m_cCurrentParameterSet.m_nExtraIncWeight = 0;
                            m_cCurrentParameterSet.m_nPTPenVersion = 0;
                        }

                        m_cCurrentParameterSet.m_nRealityWeight = nRealValue;
                        m_cCurrentParameterSet.m_nTotalWeight = nMiddleValue;
                    }
                }
                else if (eSubStep == SubTuningStep.PRESSUREPROTECT)
                    m_cCurrentParameterSet.m_sHoverRaiseHeight = ParamAutoTuning.m_dHoverHeight_PP.ToString();
            }

            if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.PEAKCHECKTUNING)
            {
                if (ParamAutoTuning.m_nAutoTune_P0_detect_time_Index == 1)
                    m_cCurrentParameterSet.m_sP0_detect_time = SpecificText.m_sP0_Detect_Time_800us;
                else
                    m_cCurrentParameterSet.m_sP0_detect_time = SpecificText.m_sP0_Detect_Time_400us;
            }
            else if (eMainStep == MainTuningStep.DIGITALTUNING)
            {
                if (eSubStep == SubTuningStep.HOVER_1ST ||
                    eSubStep == SubTuningStep.HOVER_2ND ||
                    eSubStep == SubTuningStep.CONTACT)
                    m_cCurrentParameterSet.m_sP0_detect_time = SpecificText.m_sP0_Detect_Time_400us;
            }

            if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                eMainStep == MainTuningStep.TPGAINTUNING ||
                eMainStep == MainTuningStep.TILTTUNING)
            {
                if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_HOR)
                    m_cCurrentParameterSet.m_sDrawLineType = StringConvert.m_sDRAWTYPE_HORIZONTAL;
                else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_VER)
                    m_cCurrentParameterSet.m_sDrawLineType = StringConvert.m_sDRAWTYPE_VERTICAL;
                else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE)
                    m_cCurrentParameterSet.m_sDrawLineType = StringConvert.m_sDRAWTYPE_SLANT;
                else
                    m_cCurrentParameterSet.m_sDrawLineType = StringConvert.m_sDRAWTYPE_NA;
            }
            else if (eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_HOR)
                    m_cCurrentParameterSet.m_sDrawLineType = StringConvert.m_sDRAWTYPE_HORIZONTAL;
                else if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE_VER)
                    m_cCurrentParameterSet.m_sDrawLineType = StringConvert.m_sDRAWTYPE_VERTICAL;
                else
                    m_cCurrentParameterSet.m_sDrawLineType = StringConvert.m_sDRAWTYPE_NA;
            }

            Dictionary<string, string> dictMappingTable = new Dictionary<string, string>()
            {
                { StringConvert.m_sRECORD_FWCHECKVERSION,               m_cCurrentParameterSet.m_sFWCheckVersion},
                { StringConvert.m_sRECORD_SUBSTEP,                      m_cCurrentParameterSet.m_sSubStep},
                { StringConvert.m_sRECORD_SETTINGPH1,                   m_cCurrentParameterSet.m_nSettingPH1.ToString().PadLeft(2, '0') },
                { StringConvert.m_sRECORD_SETTINGPH2,                   m_cCurrentParameterSet.m_nSettingPH2.ToString().PadLeft(2, '0') },
                { StringConvert.m_sRECORD_READPH1,                      m_cCurrentParameterSet.m_nReadPH1.ToString().PadLeft(2, '0') },
                { StringConvert.m_sRECORD_READPH2,                      m_cCurrentParameterSet.m_nReadPH2.ToString().PadLeft(2, '0') },
                { StringConvert.m_sRECORD_FREQUENCY,                    m_cCurrentParameterSet.m_sFrequency },
                { StringConvert.m_sRECORD_REPORTNUMBER,                 m_cCurrentParameterSet.m_nReadReportNumber.ToString() },
                { StringConvert.m_sRECORD_HOVERRAISEHEIGHT,             m_cCurrentParameterSet.m_sHoverRaiseHeight},
                { StringConvert.m_sRECORD_RANKINDEX,                    m_cCurrentParameterSet.m_nRankIndex.ToString() },
                { StringConvert.m_sRECORD_PTPENVERSION,                 m_cCurrentParameterSet.m_nPTPenVersion.ToString() },
                { StringConvert.m_sRECORD_P0_DETECT_TIME,               m_cCurrentParameterSet.m_sP0_detect_time },
                { StringConvert.m_sRECORD_NOISERXINNERMAX,              m_cCurrentParameterSet.m_nNoiseRXInnerMax.ToString() },
                { StringConvert.m_sRECORD_NOISETXINNERMAX,              m_cCurrentParameterSet.m_nNoiseTXInnerMax.ToString() },
                { StringConvert.m_sRECORD_NOISERXINMAXPLUS3INMAXSTD,    m_cCurrentParameterSet.m_nNoiseRXInMaxPlus3InMaxSTD.ToString() },
                { StringConvert.m_sRECORD_NOISEP0_DETECT_TIME_INDEX,    m_cCurrentParameterSet.m_nNoiseP0_Detect_Time_Idx.ToString() },
                { StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_1TR,  m_cCurrentParameterSet.m_nNoiseTrcMaxMinusPreP0_TH_1Trc.ToString() },
                { StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_2TR,  m_cCurrentParameterSet.m_nNoiseTrcMaxMinusPreP0_TH_2Trc.ToString() },
                { StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_3TR,  m_cCurrentParameterSet.m_nNoiseTrcMaxMinusPreP0_TH_3Trc.ToString() },
                { StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_4TR,  m_cCurrentParameterSet.m_nNoiseTrcMaxMinusPreP0_TH_4Trc.ToString() },
                { StringConvert.m_sRECORD_NOISEDIGIGAIN_P0,             m_cCurrentParameterSet.m_nNoiseDigiGain_P0.ToString() },
                { StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_RX,      m_cCurrentParameterSet.m_nNoiseDigiGain_Beacon_Rx.ToString() },
                { StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_TX,      m_cCurrentParameterSet.m_nNoiseDigiGain_Beacon_Tx.ToString() },
                { StringConvert.m_sRECORD_SP0_TH,                       m_cCurrentParameterSet.m_nSettingcActivePen_FM_P0_TH.ToString() },
                { StringConvert.m_sRECORD_RP0_TH,                       m_cCurrentParameterSet.m_nReadcActivePen_FM_P0_TH.ToString() },
                { StringConvert.m_sRECORD_STRXS_HOVER_TH_RX,            m_cCurrentParameterSet.m_nSettingTRxS_Beacon_Hover_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_RTRXS_HOVER_TH_RX,            m_cCurrentParameterSet.m_nReadTRxS_Beacon_Hover_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_STRXS_HOVER_TH_TX,            m_cCurrentParameterSet.m_nSettingTRxS_Beacon_Hover_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_RTRXS_HOVER_TH_TX,            m_cCurrentParameterSet.m_nReadTRxS_Beacon_Hover_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_STRXS_CONTACT_TH_RX,          m_cCurrentParameterSet.m_nSettingTRxS_Beacon_Contact_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_RTRXS_CONTACT_TH_RX,          m_cCurrentParameterSet.m_nReadTRxS_Beacon_Contact_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_STRXS_CONTACT_TH_TX,          m_cCurrentParameterSet.m_nSettingTRxS_Beacon_Contact_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_RTRXS_CONTACT_TH_TX,          m_cCurrentParameterSet.m_nReadTRxS_Beacon_Contact_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_SHOVER_TH_RX,                 m_cCurrentParameterSet.m_nSettingBeacon_Hover_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_RHOVER_TH_RX,                 m_cCurrentParameterSet.m_nReadBeacon_Hover_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_SHOVER_TH_TX,                 m_cCurrentParameterSet.m_nSettingBeacon_Hover_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_RHOVER_TH_TX,                 m_cCurrentParameterSet.m_nReadBeacon_Hover_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_SCONTACT_TH_RX,               m_cCurrentParameterSet.m_nSettingBeacon_Contact_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_RCONTACT_TH_RX,               m_cCurrentParameterSet.m_nReadBeacon_Contact_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_SCONTACT_TH_TX,               m_cCurrentParameterSet.m_nSettingBeacon_Contact_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_RCONTACT_TH_TX,               m_cCurrentParameterSet.m_nReadBeacon_Contact_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_SPTHF_CONTACT_TH_RX,          m_cCurrentParameterSet.m_nSettingPTHF_Contact_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_RPTHF_CONTACT_TH_RX,          m_cCurrentParameterSet.m_nReadPTHF_Contact_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_SPTHF_CONTACT_TH_TX,          m_cCurrentParameterSet.m_nSettingPTHF_Contact_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_RPTHF_CONTACT_TH_TX,          m_cCurrentParameterSet.m_nReadPTHF_Contact_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_SPTHF_HOVER_TH_RX,            m_cCurrentParameterSet.m_nSettingPTHF_Hover_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_RPTHF_HOVER_TH_RX,            m_cCurrentParameterSet.m_nReadPTHF_Hover_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_SPTHF_HOVER_TH_TX,            m_cCurrentParameterSet.m_nSettingPTHF_Hover_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_RPTHF_HOVER_TH_TX,            m_cCurrentParameterSet.m_nReadPTHF_Hover_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_SBHF_CONTACT_TH_RX,           m_cCurrentParameterSet.m_nSettingBHF_Contact_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_RBHF_CONTACT_TH_RX,           m_cCurrentParameterSet.m_nReadBHF_Contact_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_SBHF_CONTACT_TH_TX,           m_cCurrentParameterSet.m_nSettingBHF_Contact_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_RBHF_CONTACT_TH_TX,           m_cCurrentParameterSet.m_nReadBHF_Contact_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_SBHF_HOVER_TH_RX,             m_cCurrentParameterSet.m_nSettingBHF_Hover_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_RBHF_HOVER_TH_RX,             m_cCurrentParameterSet.m_nReadBHF_Hover_TH_Rx.ToString() },
                { StringConvert.m_sRECORD_SBHF_HOVER_TH_TX,             m_cCurrentParameterSet.m_nSettingBHF_Hover_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_RBHF_HOVER_TH_TX,             m_cCurrentParameterSet.m_nReadBHF_Hover_TH_Tx.ToString() },
                { StringConvert.m_sRECORD_SEDGE_1TRC_SUBPWR,            m_cCurrentParameterSet.m_nSEdge_1Trc_SubPwr.ToString() },
                { StringConvert.m_sRECORD_REDGE_1TRC_SUBPWR,            m_cCurrentParameterSet.m_nREdge_1Trc_SubPwr.ToString() },
                { StringConvert.m_sRECORD_SEDGE_2TRC_SUBPWR,            m_cCurrentParameterSet.m_nSEdge_2Trc_SubPwr.ToString() },
                { StringConvert.m_sRECORD_REDGE_2TRC_SUBPWR,            m_cCurrentParameterSet.m_nREdge_2Trc_SubPwr.ToString() },
                { StringConvert.m_sRECORD_SEDGE_3TRC_SUBPWR,            m_cCurrentParameterSet.m_nSEdge_3Trc_SubPwr.ToString() },
                { StringConvert.m_sRECORD_REDGE_3TRC_SUBPWR,            m_cCurrentParameterSet.m_nREdge_3Trc_SubPwr.ToString() },
                { StringConvert.m_sRECORD_SEDGE_4TRC_SUBPWR,            m_cCurrentParameterSet.m_nSEdge_4Trc_SubPwr.ToString() },
                { StringConvert.m_sRECORD_REDGE_4TRC_SUBPWR,            m_cCurrentParameterSet.m_nREdge_4Trc_SubPwr.ToString() },
                { StringConvert.m_sRECORD_DRAWLINETYPE,                 m_cCurrentParameterSet.m_sDrawLineType },
                { StringConvert.m_sRECORD_CONTROLMODE,                  m_cCurrentParameterSet.m_sControlMode },
                { StringConvert.m_sRECORD_RXTRACENUMBER,                m_cCurrentParameterSet.m_nRXTraceNumber.ToString() },
                { StringConvert.m_sRECORD_TXTRACENUMBER,                m_cCurrentParameterSet.m_nTXTraceNumber.ToString() },
                { StringConvert.m_sRECORD_PRESSUREWEIGHT,               m_cCurrentParameterSet.m_nPressureWeight.ToString() },
                { StringConvert.m_sRECORD_REALITYWEIGHT,                m_cCurrentParameterSet.m_nRealityWeight.ToString() },
                { StringConvert.m_sRECORD_OFFSETWEIGHT,                 m_cCurrentParameterSet.m_nOffsetWeight.ToString() },
                { StringConvert.m_sRECORD_EXTRAINCWEIGHT,               m_cCurrentParameterSet.m_nExtraIncWeight.ToString() },
                { StringConvert.m_sRECORD_TOTALWEIGHT,                  m_cCurrentParameterSet.m_nTotalWeight.ToString() },
                { StringConvert.m_sRECORD_SIQ_BSH_P,                    m_cCurrentParameterSet.m_nSIQ_BSH_P.ToString() },
                { StringConvert.m_sRECORD_RIQ_BSH_P,                    m_cCurrentParameterSet.m_nRIQ_BSH_P.ToString() },
                { StringConvert.m_sRECORD_SPRESSURE3BINSTH,             m_cCurrentParameterSet.m_nSPressure3BinsTH.ToString() },
                { StringConvert.m_sRECORD_RPRESSURE3BINSTH,             m_cCurrentParameterSet.m_nRPressure3BinsTH.ToString() },
                { StringConvert.m_sRECORD_SPRESS_3BINSPWR,              m_cCurrentParameterSet.m_nS3BinsPwr.ToString() },
                { StringConvert.m_sRECORD_RPRESS_3BINSPWR,              m_cCurrentParameterSet.m_nR3BinsPwr.ToString() },
                { StringConvert.m_sRECORD_PRESS_MAXDFTRXMEAN,           m_cCurrentParameterSet.m_nPress_MaxDFTRxMean.ToString() },
                { StringConvert.m_sRECORD_BEFIQ_BSH_P,                  m_cCurrentParameterSet.m_nBefIQ_BSH_P.ToString() },
                { StringConvert.m_sRECORD_BEFPRESS_MAXDFTRXMEAN,        m_cCurrentParameterSet.m_nBefPress_MaxDFTRxMean.ToString() },
                { StringConvert.m_sRECORD_SDIGIGAIN_P0,                 m_cCurrentParameterSet.m_nSDigiGain_P0.ToString() },
                { StringConvert.m_sRECORD_SDIGIGAIN_BEACON_RX,          m_cCurrentParameterSet.m_nSDigiGain_Beacon_Rx.ToString() },
                { StringConvert.m_sRECORD_SDIGIGAIN_BEACON_TX,          m_cCurrentParameterSet.m_nSDigiGain_Beacon_Tx.ToString() },
                { StringConvert.m_sRECORD_SDIGIGAIN_PTHF_RX,            m_cCurrentParameterSet.m_nSDigiGain_PTHF_Rx.ToString() },
                { StringConvert.m_sRECORD_SDIGIGAIN_PTHF_TX,            m_cCurrentParameterSet.m_nSDigiGain_PTHF_Tx.ToString() },
                { StringConvert.m_sRECORD_SDIGIGAIN_BHF_RX,             m_cCurrentParameterSet.m_nSDigiGain_BHF_Rx.ToString() },
                { StringConvert.m_sRECORD_SDIGIGAIN_BHF_TX,             m_cCurrentParameterSet.m_nSDigiGain_BHF_Tx.ToString() },
                { StringConvert.m_sRECORD_SPEN_HI_HF_THD,               m_cCurrentParameterSet.m_nSPen_HI_HF_THD.ToString() },
                { StringConvert.m_sRECORD_RDIGIGAIN_P0,                 m_cCurrentParameterSet.m_nRDigiGain_P0.ToString() },
                { StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX,          m_cCurrentParameterSet.m_nRDigiGain_Beacon_Rx.ToString() },
                { StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX,          m_cCurrentParameterSet.m_nRDigiGain_Beacon_Tx.ToString() },
                { StringConvert.m_sRECORD_RDIGIGAIN_PTHF_RX,            m_cCurrentParameterSet.m_nRDigiGain_PTHF_Rx.ToString() },
                { StringConvert.m_sRECORD_RDIGIGAIN_PTHF_TX,            m_cCurrentParameterSet.m_nRDigiGain_PTHF_Tx.ToString() },
                { StringConvert.m_sRECORD_RDIGIGAIN_BHF_RX,             m_cCurrentParameterSet.m_nRDigiGain_BHF_Rx.ToString() },
                { StringConvert.m_sRECORD_RDIGIGAIN_BHF_TX,             m_cCurrentParameterSet.m_nRDigiGain_BHF_Tx.ToString() },
                { StringConvert.m_sRECORD_RPEN_HI_HF_THD,               m_cCurrentParameterSet.m_nRPen_HI_HF_THD.ToString() },
                { StringConvert.m_sRECORD_5TRAWDATATYPE,                ParamAutoTuning.m_n5TRawDataType.ToString() },
                { StringConvert.m_sRECORD_TRACETYPE,                    m_cCurrentParameterSet.m_nTraceType.ToString() },
                { StringConvert.m_sRECORD_7318TRXSSPECIFICREPORTTYPE,   ParamAutoTuning.m_nDT7318TRxSSpecificReportType.ToString() },
                { StringConvert.m_sRECORD_RXSTARTTRACE,                 m_nRXStartTrace.ToString() },
                { StringConvert.m_sRECORD_TXSTARTTRACE,                 m_nTXStartTrace.ToString() }
            };

            return dictMappingTable;
        }

        /// <summary>
        /// 寫入參數數值
        /// </summary>
        /// <param name="bWriteErrorFlag">是否寫入錯誤</param>
        /// <param name="sw">StreamWriter</param>
        /// <param name="dictRecordInfoMappingTable">參數MappingTable</param>
        /// <param name="sInfoType">參數名稱</param>
        /// <param name="bEndOfLineFlag">是否為最後一個要換行</param>
        private void WriteInfo(ref bool bWriteErrorFlag, StreamWriter sw, Dictionary<string, string> dictRecordInfoMappingTable, string sInfoType, bool bEndOfLineFlag = false)
        {
            if (dictRecordInfoMappingTable.ContainsKey(sInfoType) == false)
            {
                bWriteErrorFlag = true;

                if (bEndOfLineFlag == true)
                    sw.WriteLine();

                return;
            }

            string sValue = dictRecordInfoMappingTable[sInfoType];

            if (bEndOfLineFlag == false)
                sw.Write("{0}={1},", sInfoType, sValue);
            else
                sw.WriteLine("{0}={1}", sInfoType, sValue);
        }
        #endregion

        #region frmMain Form Function
        private void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmMain.OutputMessage(sMessage, bWarning);
        }

        private void SetNewConnectButton(bool bEnable)
        {
            m_cfrmMain.SetNewConnectButton(bEnable);
        }

        private void SetNewStartButton(bool bEnable)
        {
            m_cfrmMain.SetNewStartButton(bEnable);
        }

        private void SetNewStopButton(bool bEnable)
        {
            m_cfrmMain.SetNewStopButton(bEnable);
        }

        private void SetNewDrawButton(int nButtonFlag)
        {
            m_cfrmMain.SetNewDrawButton(nButtonFlag);
        }

        private void OutputStateMessage(string sMessage, bool bResultOutputFlag = false, bool bResetOutputFlag = false, bool bStepOutputOnlyFlag = false, bool bTimerCountStopFlag = false)
        {
            m_cfrmMain.OutputStateMessage(sMessage, bResultOutputFlag, bResetOutputFlag, bStepOutputOnlyFlag, bTimerCountStopFlag);
        }

        private void SetTopMost(bool bTopMostFlag, string sMessage)
        {
            m_cfrmMain.SetTopMost(bTopMostFlag, sMessage);
        }

        private void SetModeStateComboBoxAndSettingToolStripMenuItem(bool bEnable)
        {
            m_cfrmMain.SetModeStateComboBoxAndSettingToolStripMenuItem(bEnable);
        }

        private void SetStepLabelBackColor(MainTuningStep eMainStep, bool bStepLabelClearFlag = false)
        {
            m_cfrmMain.SetStepLabelBackColor(eMainStep, bStepLabelClearFlag);
        }

        private void WriteDebugLog(string sMessage)
        {
            m_cfrmMain.m_cDebugLog.WriteLogToBuffer(sMessage);
        }

        private void OutputCostTimeGroupBox()
        {
            m_cfrmMain.OutputCostTimeGroupBox();
        }

        private void WriteMainStepCostTimeInfo()
        {
            m_cfrmMain.WriteMainStepCostTimeInfo();
        }

        private void OutputMainStatusStrip(string sStatus, int nCurrentCount, int nTotalCount = 0, int nStatusFlag = frmMain.m_nOtherFlag)
        {
            m_cfrmMain.OutputMainStatusStrip(sStatus, nCurrentCount, nTotalCount, nStatusFlag);
        }

        private void OutputStatusAndErrorMessageLabel(string sResultMessage, string sErrorMessage, Color colorForeColor, bool bOnlyChangelblStatus = false)
        {
            m_cfrmMain.OutputStatusAndErrorMessageLabel(sResultMessage, sErrorMessage, colorForeColor, bOnlyChangelblStatus);
        }

        private void SetScreenResetFlow()
        {
            m_cfrmMain.SetScreenResetFlow();
        }

        private void ShowFullScreen(FlowStep cFlowStep)
        {
            m_cfrmMain.ShowFullScreen(cFlowStep);
        }

        private void HideFullScreen()
        {
            m_cfrmMain.HideFullScreen();
        }

        private void DisableMonitor()
        {
            m_cfrmMain.DisableMonitor();
        }

        private void SetNewPatternAndNewDrawButton(bool bEnableFlag)
        {
            m_cfrmMain.SetNewPatternAndNewDrawButton(bEnableFlag);
        }

        private void SetFlowStepResult(bool bResultFlag, string sMessage = "")
        {
            m_cfrmMain.SetFlowStepResult(bResultFlag, sMessage);
        }

        private void ClearAndSetFlowStepResultList()
        {
            m_cfrmMain.ClearAndSetFlowStepResultList();
        }

        private void ShowMessageBox(string sMessage, string sTitle = frmMessageBox.m_sWarining, string sConfirmButton = "OK")
        {
            m_cfrmMain.ShowMessageBox(sMessage, sTitle, sConfirmButton);
        }
        #endregion
    }
}
