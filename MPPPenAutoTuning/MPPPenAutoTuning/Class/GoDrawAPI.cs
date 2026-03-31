using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using MathNet.Numerics;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class GoDrawAPI
    {
        private frmGoDrawController m_cfrmGoDrawController;
        private frmMain m_cfrmMain;

        private string m_sCOMPPort = "NA";
        private double m_dSpeed = 0.5;
        private int m_nMaxCoordinateX = 300;
        private int m_nMaxCoordinateY = 210;
        private int m_nMaxServoValue = 30000;
        private int m_nMinServoValue = 3000;
        private int m_nTopServoValue = 26000;
        private int m_nHoverServoValue = 10000;
        private int m_nContactServoValue = 3000;

        private double m_nTimeCoefficent = 1000;
        private int m_nDistanceCoefficent = 80;
        private int m_nRevceiveDataBuffer = 1024;
        private int m_nBaudRate = 115200;
        private double m_dCauseDelayTimeMaxSpeed = ParamAutoTuning.m_dGoDrawCtrlrCauseDelayMaxSpeed;

        private IPEndPoint m_ipedIPEndPoint;    // = new IPEndPoint(IPAddress.Any, 5555);
        private UdpClient m_ucUdpClient = null;        // = new UdpClient(ipep.Port);
        private DateTime dtStartTime;

        #region GoDraw Commend Type
        public enum GoDrawCommand
        {
            Sleep,
            Home,
            Hover,
            Contact,
            Top,
            Move,
            VMove,
            Right,
            Left,
            Up,
            Down,
            ZUpDown,
            Stop,
            ZMove
        }
        #endregion

        public enum ControlUIType
        {
            GoDrawController,
            MSPen_AutoTuning
        }

        public enum OutputMessageType
        {
            LabelMessage,
            RichTextBoxMessage
        }

        private enum RunCommandState
        {
            Success,
            Error_Ack,
            Error_Other
        }

        private ControlUIType m_eControlUIType;

        //private string m_sScript = "";
        private SerialPort m_spSerialPort;
        private Thread m_tDoReceive;
        private string m_sCOMPortBuffer = "";
        //private string m_sSetRegion = "";
        private bool m_bBreakFlag = false;
        private bool m_bSerialPortReceived = false;
        private bool m_bCOMPortConnected = false;
        private bool m_bDoReceiveData = false;
        private bool m_bReceiveData = false;
        private RunCommandState m_eRunCommandState = RunCommandState.Success;
        private bool m_bResetFlag = true;
        //private int m_nRepeatTime = 0;
        //private bool m_bWaitScript = false;

        private double m_dPreviousCoordinateX = 0.0;
        private double m_dPreviousCoordinateY = 0.0;
        private double m_dCurrentCoordinateX = 0.0;
        private double m_dCurrentCoordinateY = 0.0;
        private double m_dDesitinationCoordinateX = 0.0;
        private double m_dDesitinationCoordinateY = 0.0;
        private int m_nCurrentServoValue = 0;

        private int m_nMoveType = 0;
        private int m_nEstimateType = 0;
        private int m_nSplitTime = 500;

        private int m_nCurrentThreadID = 0;

        //public delegate void Display(string sBuffer);
        public bool m_bActionFlag = false;

        public bool m_bStop = false;
        //private bool m_bSend = false;
        public bool m_bForceStop = false;
        private bool m_bCheckReveiveAck = false;

        private class GoDrawCommandInfo
        {
            public int nCostTime = 0;
            public string[] arrsCommand;

            public double dCoordinateX;
            public double dCoordinateY;
        }

        private double[] m_dEstimateDelayTimeCoefficient_Array;
        private double[] m_dDelayTimeSpeedRatioCoefficient_Array;

        private double[] ComputeEstimateDelayTimeFit()
        {
            double[] dDistance_Array = new double[] 
            { 
                ParamAutoTuning.m_nGoDrawCtrlrEstimateDelay_Distance1,
                ParamAutoTuning.m_nGoDrawCtrlrEstimateDelay_Distance2 
            };

            double[] dTime_Array = new double[] 
            { 
                ParamAutoTuning.m_nGoDrawCtrlrEstimateDelay_Time1,
                ParamAutoTuning.m_nGoDrawCtrlrEstimateDelay_Time2_Speed1 
            };

            double[] dCoefficient_Array = Fit.Polynomial(dDistance_Array, dTime_Array, 1);

            return dCoefficient_Array;
        }

        private double[] ComputeDelayTimeSpeedRatioFit()
        {
            double[] dSpeed_Array = new double[] 
            { 
                ParamAutoTuning.m_dGoDrawCtrlrEstimateDelay_Speed1,
                ParamAutoTuning.m_dGoDrawCtrlrEstimateDelay_Speed2 
            };

            double[] dTimeRatio_Array = new double[] 
            { 
                (double)ParamAutoTuning.m_nGoDrawCtrlrEstimateDelay_Time2_Speed1 / (double)ParamAutoTuning.m_nGoDrawCtrlrEstimateDelay_Time2_Speed1,
                (double)ParamAutoTuning.m_nGoDrawCtrlrEstimateDelay_Time2_Speed2 / (double)ParamAutoTuning.m_nGoDrawCtrlrEstimateDelay_Time2_Speed1 
            };

            double[] dCoefficient_Array = Fit.Polynomial(dSpeed_Array, dTimeRatio_Array, 1);

            return dCoefficient_Array;
        }

        private int ComputeEstimateDelayTime(double dDistance, double dSpeed)
        {
            int nFitDelayTime = 0;

            if (dDistance < ParamAutoTuning.m_nGoDrawCtrlrEstimateDelay_Distance1 || dSpeed == 0.0 || dSpeed >= m_dCauseDelayTimeMaxSpeed)
                return nFitDelayTime;

            double dFitDelayTime = m_dEstimateDelayTimeCoefficient_Array[0] + m_dEstimateDelayTimeCoefficient_Array[1] * dDistance;
            double dSpeedRatio = m_dDelayTimeSpeedRatioCoefficient_Array[0] + m_dDelayTimeSpeedRatioCoefficient_Array[1] * dSpeed;
            nFitDelayTime = (int)(dFitDelayTime * dSpeedRatio) + 1;

            return nFitDelayTime;
        }

        /*
        private double[] m_dSetSpeed_Array = new double[] { 0.5, 1.0, 2.0, 3.0, 4.0 };
        private double[] m_d300mmRealSpeed_Array = new double[] { 300 / 607.0, 300 / 303.5, 300 / 151.0, 300 / 100.0, 300 / 75.0 };

        private double[] m_dCoefficient_Array;

        private double[] ComputePolyFit(double[] dX_Array, double[] dY_Array, int nOrder)
        {
            double[] dCoefficient_Array = Fit.Polynomial(dX_Array, dY_Array, nOrder);
            return dCoefficient_Array;
        }
        
        private double ComputeCurveFitValue(double dXValue)
        {
            double dCurveFitValue = 0.0;

            dCurveFitValue = m_dCoefficient_Array[0] + m_dCoefficient_Array[1] * dXValue;

            return dCurveFitValue;
        }
        */

        public GoDrawAPI(ControlUIType eControlUIType, frmGoDrawController cfrmGoDrawController = null, frmMain cfrmMain = null)
        {
            m_eControlUIType = eControlUIType;

            if (eControlUIType == ControlUIType.GoDrawController)
                m_cfrmGoDrawController = cfrmGoDrawController;
            else if (eControlUIType == ControlUIType.MSPen_AutoTuning)
                m_cfrmMain = cfrmMain;

            //m_dCoefficient_Array = ComputePolyFit(m_d300mmRealSpeed_Array, m_dSetSpeed_Array, 2);
            m_dEstimateDelayTimeCoefficient_Array = ComputeEstimateDelayTimeFit();
            m_dDelayTimeSpeedRatioCoefficient_Array = ComputeDelayTimeSpeedRatioFit();

            m_ipedIPEndPoint = new IPEndPoint(IPAddress.Any, 5555);
            m_ucUdpClient = new UdpClient(m_ipedIPEndPoint.Port);

            m_bActionFlag = false;
            m_dCurrentCoordinateX = 0.0;
            m_dCurrentCoordinateY = 0.0;
        }

        public void SetParameter()
        {
            if (m_eControlUIType == ControlUIType.GoDrawController)
            {
                m_sCOMPPort = m_cfrmGoDrawController.m_sCOMPPort;
                m_dSpeed = m_cfrmGoDrawController.m_dSpeed;
                m_nMaxCoordinateX = m_cfrmGoDrawController.m_nMaxCoordinateX;
                m_nMaxCoordinateY = m_cfrmGoDrawController.m_nMaxCoordinateY;
                m_nMaxServoValue = ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue;
                m_nMinServoValue = ParamAutoTuning.m_nGoDrawCtrlrMinServoValue;
                m_nTopServoValue = m_cfrmGoDrawController.m_nTopServoValue;
                m_nHoverServoValue = m_cfrmGoDrawController.m_nHoverServoValue;
                m_nContactServoValue = m_cfrmGoDrawController.m_nContactServoValue;
                m_nMoveType = ParamAutoTuning.m_nGoDrawCtrlrMoveType;
                m_nEstimateType = ParamAutoTuning.m_nGoDrawCtrlrEstimateType;
            }
            else if (m_eControlUIType == ControlUIType.MSPen_AutoTuning)
            {
                m_sCOMPPort = ParamAutoTuning.m_sGoDrawCtrlrCOMPort;
                m_nMaxCoordinateX = ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX;
                m_nMaxCoordinateY = ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY;
                m_nMaxServoValue = ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue;
                m_nMinServoValue = ParamAutoTuning.m_nGoDrawCtrlrMinServoValue;
                m_nMoveType = ParamAutoTuning.m_nGoDrawCtrlrMoveType;
                m_nEstimateType = ParamAutoTuning.m_nGoDrawCtrlrEstimateType;
            }
        }

        private bool ConnectSerialPort(string sCOMPort, Int32 nBaudRate)
        {
            bool bConnectFlag = false;
            m_bActionFlag = true;
            SetUIButtonState(!m_bActionFlag);

            try
            {
                CloseUdpClient();

                if (m_spSerialPort != null)
                    CloseCOMPort();

                m_spSerialPort = new SerialPort();

                if (m_spSerialPort.IsOpen)
                {
                    m_spSerialPort.Close();
                }

                //設定 Serial Port 參數
                m_spSerialPort.PortName = sCOMPort;
                m_spSerialPort.BaudRate = nBaudRate;
                m_spSerialPort.DataBits = 8;
                m_spSerialPort.StopBits = StopBits.One;

                if (!m_spSerialPort.IsOpen)
                {
                    //開啟 Serial Port
                    m_spSerialPort.Open();
                    m_spSerialPort.DiscardInBuffer();
                    m_bSerialPortReceived = true;
                    m_bCOMPortConnected = true;
                    bConnectFlag = true;

                    //開啟執行續做接收動作
                    m_tDoReceive = new Thread(DoReceive);
                    m_tDoReceive.IsBackground = true;
                    m_tDoReceive.Start();
                    m_bDoReceiveData = true;
                    m_bReceiveData = false;
                }
            }
            catch (Exception ex)
            {
                string sMessage = string.Format("-Connect Serial Port Error({0})", ex.Message);
                OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                sMessage = string.Format("Connect Serial Port Error");
                OutputMessage(OutputMessageType.LabelMessage, sMessage);
                m_bCOMPortConnected = false;
            }

            m_bActionFlag = false;
            SetUIButtonState(!m_bActionFlag);

            return bConnectFlag;
        }

        public void CloseUdpClient()
        {
            if (m_ucUdpClient != null)
            {
                m_ucUdpClient.Close();
                m_ucUdpClient = null;
            }
        }

        public void CloseCOMPort()
        {
            try
            {
                m_bDoReceiveData = false;

                if (m_tDoReceive != null)
                    m_tDoReceive.Abort();

                if (m_spSerialPort.IsOpen)
                    m_spSerialPort.Close();

                m_bCOMPortConnected = false;
                m_bActionFlag = false;
                SetUIButtonState(!m_bActionFlag);
            }
            catch (Exception ex)
            {
                string sMessage = string.Format("-Close COM Port Error({0})", ex.Message);
                OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                sMessage = "Close COM Port Error";
                OutputMessage(OutputMessageType.LabelMessage, sMessage);
            }
        }

        private void DoReceive()
        {
            Byte[] byteBuffer_Array = new Byte[m_nRevceiveDataBuffer];

            try
            {
                while (m_bSerialPortReceived)
                {
                    if (m_spSerialPort.BytesToRead > 0)
                    {
                        Array.Clear(byteBuffer_Array, 0, m_nRevceiveDataBuffer);
                        Int32 nLength = m_spSerialPort.Read(byteBuffer_Array, 0, byteBuffer_Array.Length);

                        //string sBuffer = Encoding.ASCII.GetString(byteBuffer_Array);
                        m_sCOMPortBuffer = Encoding.ASCII.GetString(byteBuffer_Array);
                        Array.Resize(ref byteBuffer_Array, nLength);
                        //Display displayDisplayMessage = new Display(ShowMessage);
                        //this.Invoke(displayDisplayMessage, new Object[] { byteBuffer_Array });
                        //this.Invoke(displayDisplayMessage, new Object[] { m_sCOMPortBuffer });
                        Array.Resize(ref byteBuffer_Array, m_nRevceiveDataBuffer);
                        m_bReceiveData = true;

                        if (m_bCheckReveiveAck == true)
                        {
                            string[] sSeparator_Array = new string[] { "\n\r", "\r\n" };
                            string[] sSplitData_Array = m_sCOMPortBuffer.Split(sSeparator_Array, StringSplitOptions.None);

                            bool bOKFlag = false;

                            foreach (string sSplitData in sSplitData_Array)
                            {
                                if (sSplitData == "OK")
                                {
                                    bOKFlag = true;
                                    break;
                                }
                            }

                            if (bOKFlag == true)
                                m_eRunCommandState = RunCommandState.Success;
                            else
                            {
                                string sMessage = "-Receive Data Error(Not Include OK)";
                                OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                                sMessage = "Receive Data Error";
                                OutputMessage(OutputMessageType.LabelMessage, sMessage);
                                m_eRunCommandState = RunCommandState.Error_Ack;
                            }
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                if (m_bDoReceiveData == true)
                {
                    m_bStop = true;
                    string sMessage = string.Format("-Receive Data Error({0})", ex.Message);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                    sMessage = "Receive Data Error";
                    OutputMessage(OutputMessageType.LabelMessage, sMessage);
                    m_eRunCommandState = RunCommandState.Error_Other;
                }
            }
        }
        /*
        public void ShowMessage(string buffer)
        {
            lblCOMPortReceive.Text += buffer;
        }
        */

        private void SendData(Object objSendBuffer)
        {
            if (objSendBuffer != null)
            {
                Byte[] byteBuffer_Array = objSendBuffer as Byte[];

                try
                {
                    m_spSerialPort.Write(byteBuffer_Array, 0, byteBuffer_Array.Length);
                }
                catch (Exception ex)
                {
                    m_bStop = true;
                    CloseCOMPort();

                    string sMessage = string.Format("-Send Data Error({0})", ex.Message);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                    sMessage = "Send Data Error";
                    OutputMessage(OutputMessageType.LabelMessage, sMessage);
                }
            }
        }

        /*
        private RunCommandState ReceiveData()
        {
            try
            {
                Byte[] byteBuffer_Array = new Byte[1024];

                if (m_spSerialPort.BytesToRead > 0)
                {
                    Array.Clear(byteBuffer_Array, 0, 1024);
                    Int32 nLength = m_spSerialPort.Read(byteBuffer_Array, 0, byteBuffer_Array.Length);

                    //string sBuffer = Encoding.ASCII.GetString(byteBuffer_Array);
                    m_sCOMPortBuffer = Encoding.ASCII.GetString(byteBuffer_Array);
                    Array.Resize(ref byteBuffer_Array, nLength);
                    //Array.Resize(ref byteBuffer_Array, nLength);
                    //Display displayDisplayMessage = new Display(ShowMessage);
                    //this.Invoke(displayDisplayMessage, new Object[] { byteBuffer_Array });
                    //this.Invoke(displayDisplayMessage, new Object[] { m_sCOMPortBuffer });
                    Array.Resize(ref arrbyteBuffer, 1024);
                }

                string[] sSeparator_Array = new string[] { "\n\r" };
                string[] sSplitData_Array = m_sCOMPortBuffer.Split(sSeparator_Array, StringSplitOptions.None);

                bool bOKFlag = false;
                foreach (string sSplitData in sSplitData_Array)
                {
                    if (sSplitData == "OK")
                    {
                        bOKFlag = true;
                        break;
                    }
                }

               if (bOKFlag == true)
                    return RunCommandState.Success;
                else
                {
                    string sMessage = "-Receive Data Error(Not Include OK)");
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                    sMessage = "Receive Data Error";
                    OutputMessage(OutputMessageType.LabelMessage, sMessage);
                    return RunCommandState.Error_Ack;
                }
            }
            catch(Exception ex)
            {
                string sMessage = string.Format("-Receive Data Error({0})", ex.Message);
                OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                sMessage = "Receive Data Error";
                OutputMessage(OutputMessageType.LabelMessage, sMessage);
                return RunCommandState.Error_Other;
            }
        }
        */

        public bool RunConnectSerialPort()
        {
            m_bStop = false;

            if (m_sCOMPPort == "NA")
            {
                string[] sSerialPortName_Array = SerialPort.GetPortNames();
                List<string> sSerialPortName_List = new List<string>(sSerialPortName_Array);

                if (sSerialPortName_Array.Length < 1)
                {
                    string sMessage = "-Can not Find Any COM Port";
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                    sMessage = "Can not Find Any COM Port";
                    OutputMessage(OutputMessageType.LabelMessage, sMessage);
                    return false;
                }

                foreach (string sPortName in sSerialPortName_List)
                {
                    string sMessage = string.Format("-Find COM Port: {0}", sPortName);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);

                    if (ConnectSerialPort(sPortName, m_nBaudRate) == true)
                    {
                        sMessage = string.Format("-Find GoDraw at COM Port: {0}", sPortName);
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                        break;
                    }
                    else
                    {
                        sMessage = "-COM Port No Response";
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                    }
                }
            }
            else
            {
                ConnectSerialPort(m_sCOMPPort, m_nBaudRate);
            }

            return m_bCOMPortConnected;
        }

        public bool RunGoDrawAction(GoDrawCommand eCommandType, 
                                    int nCoordinateX = 0, 
                                    int nCoordinateY = 0, 
                                    double dSpeed = 0.0, 
                                    int nSleepTime = 0,
                                    int nMoveDistance = 0, 
                                    int nZServoValue = 0)
        {
            string sCompleteMessage = "";
            //m_bWaitScript = true;
            m_bActionFlag = true;
            SetUIButtonState(!m_bActionFlag);

            string sTypeMessage = string.Format("-Send Type: {0}", eCommandType.ToString());
            OutputMessage(OutputMessageType.RichTextBoxMessage, sTypeMessage);

            double dTotalDistance = 0.0;
            int nTotalCostTime = 0;
            bool bErrorFlag = false;
            int nDelayTime = 0;

            if (eCommandType != GoDrawCommand.Stop)
            {
                bErrorFlag = OutputGoDrawActionInfo(ref dTotalDistance, ref nTotalCostTime, eCommandType, nCoordinateX, nCoordinateY, dSpeed, nSleepTime, nMoveDistance, nZServoValue);

                if (m_nEstimateType != 0)
                    nDelayTime = ComputeEstimateDelayTime(dTotalDistance, dSpeed);

                if (eCommandType == GoDrawCommand.Home)
                {
                    if (m_dCurrentCoordinateX == 0.0 && m_dCurrentCoordinateY == 0.0)
                    {
                        //m_bWaitScript = false;
                        m_bActionFlag = false;
                        SetUIButtonState(!m_bActionFlag);

                        string sMessage = "-Just at Home";
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                        sCompleteMessage = "Action Complete";
                        OutputMessage(OutputMessageType.LabelMessage, sCompleteMessage);

                        return true;
                    }
                }
            }

            List<GoDrawCommandInfo> cGoDrawCommand_List = new List<GoDrawCommandInfo>();
            cGoDrawCommand_List = GetGoDrawCommandInfo(ref bErrorFlag, eCommandType, nTotalCostTime, nDelayTime, nCoordinateX, nCoordinateY, dSpeed, nSleepTime, nMoveDistance, nZServoValue);

            if (bErrorFlag == true)
            {
                m_bActionFlag = false;
                SetUIButtonState(!m_bActionFlag);
                return false;
            }

            if (eCommandType != GoDrawCommand.Stop)
            {
                m_bForceStop = false;
                dtStartTime = DateTime.Now;
            }

            bErrorFlag = false;
            m_nCurrentThreadID = 0;
            m_bCheckReveiveAck = true;

            if ((eCommandType == GoDrawCommand.Move ||
                 eCommandType == GoDrawCommand.Right ||
                 eCommandType == GoDrawCommand.Left ||
                 eCommandType == GoDrawCommand.Down ||
                 eCommandType == GoDrawCommand.Up) &&
                m_nMoveType == 1 && dSpeed < m_dCauseDelayTimeMaxSpeed)
            {
                #region Method 1 : Multi-Thread
                /*
                foreach (GoDrawCommandInfo cGoDrawCommand in cGoDrawCommand_List)
                {
                    DateTime dtThreadStartTime = DateTime.Now;

                    m_dPreviousCoordinateX = m_dDesitinationCoordinateX;
                    m_dPreviousCoordinateY = m_dDesitinationCoordinateY;

                    m_dDesitinationCoordinateX = cGoDrawCommand.dCoordinateX;
                    m_dDesitinationCoordinateY = cGoDrawCommand.dCoordinateY;

                    string[] sCommand_Array = cGoDrawCommand.arrsCommand;
                    int nCostTime = cGoDrawCommand.nCostTime;
                    
                    m_bSend = false;

                    if (!m_bCOMPortConnected)
                    {
                        string sMessage = "-Device Connect Fail";
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                        sMessage = "Device Connect Fail!";
                        OutputMessage(OutputMessageType.LabelMessage, sMessage);
                        m_bActionFlag = false;
                        SetUIButtonState(!m_bActionFlag);
                        bErrorFlag = true;
                        break;
                    }

                    if (m_bBreakFlag == true)
                    {
                        string sMessage = "-User Interrupt";
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                        sMessage = "User Interrupt!";
                        OutputMessage(OutputMessageType.LabelMessage, sMessage);
                        //m_bWaitScript = false;
                        m_bActionFlag = false;
                        SetUIButtonState(!m_bActionFlag);
                        bErrorFlag = true;
                        break;
                    }

                    Thread tAction = new Thread(() =>
                    {
                        for (int nCommandIndex = 0; nCommandIndex < sCommand_Array.Length; nCommandIndex++)
                        {
                            //string sCommandMessage = string.Format("-Send Command: {0}", sCommand_Array[nCommandIndex]);
                            //OutputMessage(OutputMessageType.RichTextBoxMessage, sCommandMessage);

                            string[] sSplit_Array = arrsCommand[nCommandIndex].Split(',');

                            RunCommandState eRunCommandState = RunGoDrawCommand(eCommandType, sCommand_Array[nCommandIndex], nCostTime, Thread.CurrentThread.ManagedThreadId);

                            if (eRunCommandState != RunCommandState.Success)
                            {
                                if (eRunCommandState == RunCommandState.Error_Ack)
                                {
                                    bool bStopErrorFlag = false;
                                    int nStopCostTime = 0;
                                    string[] sStopCommand_Array = SetGoDrawCommandScript(ref bStopErrorFlag, ref nStopCostTime, GoDrawCommand.Stop);

                                    for (int nStopCommandIndex = 0; nStopCommandIndex < sStopCommand_Array.Length; nStopCommandIndex++)
                                        RunGoDrawCommand(GoDrawCommand.Stop, sStopCommand_Array[nStopCommandIndex], nCostTime);

                                    m_bStop = false;
                                }

                                m_bActionFlag = false;
                                SetUIButtonState(!m_bActionFlag);
                                bErrorFlag = true;
                                break;
                            }

                            OutputCoordinate(m_dCurrentCoordinateX, m_dCurrentCoordinateY, m_nCurrentServoValue);
                        }
                    });
                    tAction.IsBackground = true;
                    tAction.Start();

                    if (bErrorFlag == true || m_bForceStop == true)
                    {
                        tAction.Abort();
                        m_bForceStop = false;
                        break;
                    }

                    while (m_bSend == false)
                        Thread.Sleep(1);

                    while (true)
                    {
                        double dTimeInterval_ms = (DateTime.Now - dtThreadStartTime).TotalMilliseconds;

                        if (dTimeInterval_ms >= nCostTime)
                            break;

                        Thread.Sleep(1);
                    }
                }
                */
                #endregion

                #region Method 2 : Send All Command On Time
                m_bCheckReveiveAck = false;

                double dDestinationX = cGoDrawCommand_List[cGoDrawCommand_List.Count - 1].dCoordinateX;
                double dDestinationY = cGoDrawCommand_List[cGoDrawCommand_List.Count - 1].dCoordinateY;

                bool bSetTimer = false;
                DateTime dtActionStartTime = DateTime.Now;

                m_dPreviousCoordinateX = m_dCurrentCoordinateX;
                m_dPreviousCoordinateY = m_dCurrentCoordinateY;

                m_dDesitinationCoordinateX = dDestinationX;
                m_dDesitinationCoordinateY = dDestinationY;

                Thread tOutputCoordinate = new Thread(() =>
                {
                    while (bSetTimer == false)
                        Thread.Sleep(1);

                    while (true)
                    {
                        if (m_bForceStop == true)
                        {
                            m_dDesitinationCoordinateX = m_dCurrentCoordinateX;
                            m_dDesitinationCoordinateY = m_dCurrentCoordinateY;
                            break;
                        }

                        double dTimeInterval_ms = (DateTime.Now - dtActionStartTime).TotalMilliseconds;

                        double dOutputCoordinateX = 0.0;
                        double dOutputCoordinateY = 0.0;

                        if (dTimeInterval_ms >= nTotalCostTime)
                        {
                            m_dCurrentCoordinateX = m_dDesitinationCoordinateX;
                            m_dCurrentCoordinateY = m_dDesitinationCoordinateY;

                            dOutputCoordinateX = m_dCurrentCoordinateX;
                            dOutputCoordinateY = m_dCurrentCoordinateY;

                            OutputCoordinate(dOutputCoordinateX, dOutputCoordinateY, m_nCurrentServoValue);
                            break;
                        }
                        else
                        {
                            double dIntervalX = (m_dDesitinationCoordinateX - m_dPreviousCoordinateX) * (dTimeInterval_ms / nTotalCostTime);
                            double dIntervalY = (m_dDesitinationCoordinateY - m_dPreviousCoordinateY) * (dTimeInterval_ms / nTotalCostTime);

                            m_dCurrentCoordinateX = dIntervalX + m_dPreviousCoordinateX;
                            m_dCurrentCoordinateY = dIntervalY + m_dPreviousCoordinateY;

                            dOutputCoordinateX = Math.Round(m_dCurrentCoordinateX, 1, MidpointRounding.AwayFromZero);
                            dOutputCoordinateY = Math.Round(m_dCurrentCoordinateY, 1, MidpointRounding.AwayFromZero);

                            OutputCoordinate(dOutputCoordinateX, dOutputCoordinateY, m_nCurrentServoValue);
                        }

                        Thread.Sleep(1);
                    }
                });
                tOutputCoordinate.IsBackground = true;
                tOutputCoordinate.Start();

                foreach (GoDrawCommandInfo cGoDrawCommand in cGoDrawCommand_List)
                {
                    string[] sCommand_Array = cGoDrawCommand.arrsCommand;
                    int nCostTime = cGoDrawCommand.nCostTime;

                    //m_bSend = false;

                    if (!m_bCOMPortConnected)
                    {
                        string sMessage = "-Device Connect Fail";
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                        sMessage = "Device Connect Fail!";
                        OutputMessage(OutputMessageType.LabelMessage, sMessage);
                        m_bActionFlag = false;
                        SetUIButtonState(!m_bActionFlag);
                        bErrorFlag = true;
                        break;
                    }

                    if (m_bBreakFlag == true)
                    {
                        string sMessage = "-User Interrupt";
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                        sMessage = "User Interrupt!";
                        OutputMessage(OutputMessageType.LabelMessage, sMessage);
                        //m_bWaitScript = false;
                        m_bActionFlag = false;
                        SetUIButtonState(!m_bActionFlag);
                        bErrorFlag = true;
                        break;
                    }

                    for (int nCommandIndex = 0; nCommandIndex < sCommand_Array.Length; nCommandIndex++)
                    {
                        if (m_bForceStop == true)
                            break;

                        /*
                        string sCommandMessage = string.Format("-Send Command: {0}", sCommand_Array[nCommandIndex]);
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sCommandMessage);
                        */

                        string[] sSplit_Array = sCommand_Array[nCommandIndex].Split(',');

                        RunCommandState eRunCommandState = RunGoDrawCommand(eCommandType, sCommand_Array[nCommandIndex], nCostTime, 0, false);

                        if (bSetTimer == false)
                        {
                            dtActionStartTime = DateTime.Now;
                            bSetTimer = true;
                        }

                        if (eRunCommandState != RunCommandState.Success)
                        {
                            if (eRunCommandState == RunCommandState.Error_Ack)
                            {
                                bool bStopErrorFlag = false;
                                int nStopCostTime = 0;
                                string[] sStopCommand_Array = SetGoDrawCommandScript(ref bStopErrorFlag, ref nStopCostTime, GoDrawCommand.Stop);

                                for (int nStopCommandIndex = 0; nStopCommandIndex < sStopCommand_Array.Length; nStopCommandIndex++)
                                    RunGoDrawCommand(GoDrawCommand.Stop, sStopCommand_Array[nStopCommandIndex], nCostTime);

                                m_bStop = false;
                            }

                            m_bActionFlag = false;
                            SetUIButtonState(!m_bActionFlag);
                            bErrorFlag = true;
                            break;
                        }
                    }

                    if (m_bForceStop == true)
                        break;
                }

                while (true)
                {
                    if (m_bForceStop == true)
                        break;

                    double dTimeInterval_ms = (DateTime.Now - dtActionStartTime).TotalMilliseconds;

                    if (dTimeInterval_ms >= nTotalCostTime)
                        break;

                    Thread.Sleep(1);
                }
                #endregion
            }
            else
            {
                foreach (GoDrawCommandInfo cGoDrawCommand in cGoDrawCommand_List)
                {
                    m_dPreviousCoordinateX = m_dCurrentCoordinateX;
                    m_dPreviousCoordinateY = m_dCurrentCoordinateY;

                    m_dDesitinationCoordinateX = cGoDrawCommand.dCoordinateX;
                    m_dDesitinationCoordinateY = cGoDrawCommand.dCoordinateY;

                    string[] sCommand_Array = cGoDrawCommand.arrsCommand;
                    int nCostTime = nTotalCostTime;

                    for (int nCommandIndex = 0; nCommandIndex < sCommand_Array.Length; nCommandIndex++)
                    {
                        if (!m_bCOMPortConnected)
                        {
                            string sMessage = "-Device Connect Fail";
                            OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                            sMessage = "Device Connect Fail!";
                            OutputMessage(OutputMessageType.LabelMessage, sMessage);
                            m_bActionFlag = false;
                            SetUIButtonState(!m_bActionFlag);
                            bErrorFlag = true;
                            break;
                        }

                        if (m_bBreakFlag == true)
                        {
                            string sMessage = "-User Interrupt";
                            OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                            sMessage = "User Interrupt!";
                            OutputMessage(OutputMessageType.LabelMessage, sMessage);
                            //m_bWaitScript = false;
                            m_bActionFlag = false;
                            SetUIButtonState(!m_bActionFlag);
                            bErrorFlag = true;
                            break;
                        }

                        /*
                        string sCommandMessage = string.Format("-Send Command: {0}", arrsCommand[nCommandIndex]);
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sCommandMessage);
                        */

                        if (sCommand_Array[nCommandIndex] == "" || sCommand_Array[nCommandIndex] == "VerSStart")
                            continue;

                        string[] sSplit_Array = sCommand_Array[nCommandIndex].Split(',');

                        if (sSplit_Array[0] == "Sleep")
                        {
                            if (sSplit_Array.Length >= 2)
                            {
                                int nSleepTimeValue = Convert.ToInt32(sSplit_Array[1]);
                                Thread.Sleep(nSleepTimeValue * 1000);
                            }
                            else
                            {
                                string sMessage = "-Sleep Command Format Error";
                                OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                                sMessage = "Sleep Command Format Error";
                                OutputMessage(OutputMessageType.LabelMessage, sMessage);
                                m_bActionFlag = false;
                                SetUIButtonState(!m_bActionFlag);
                                bErrorFlag = true;
                                break;
                            }
                        }
                        else
                        {
                            RunCommandState eRunCommandState = RunGoDrawCommand(eCommandType, sCommand_Array[nCommandIndex], nCostTime);

                            if (eRunCommandState != RunCommandState.Success)
                            {
                                if (eRunCommandState == RunCommandState.Error_Ack)
                                {
                                    bool bStopErrorFlag = false;
                                    int nStopCostTime = 0;
                                    string[] sStopCommand_Array = SetGoDrawCommandScript(ref bStopErrorFlag, ref nStopCostTime, GoDrawCommand.Stop);

                                    for (int nStopCommandIndex = 0; nStopCommandIndex < sStopCommand_Array.Length; nStopCommandIndex++)
                                        RunGoDrawCommand(GoDrawCommand.Stop, sStopCommand_Array[nStopCommandIndex], nCostTime);

                                    m_bStop = false;
                                }

                                m_bActionFlag = false;
                                SetUIButtonState(!m_bActionFlag);
                                bErrorFlag = true;
                                break;
                            }
                        }

                        OutputCoordinate(m_dCurrentCoordinateX, m_dCurrentCoordinateY, m_nCurrentServoValue);
                    }

                    if (m_bStop == true)
                        break;
                }
            }

            if (eCommandType != GoDrawCommand.Stop)
            {
                double dTimeInterval = (DateTime.Now - dtStartTime).TotalSeconds;
                string sDurationMessage = string.Format("-Time Duration: {0} s", dTimeInterval.ToString());
                OutputMessage(OutputMessageType.RichTextBoxMessage, sDurationMessage);
            }

            //m_bWaitScript = false;
            m_bActionFlag = false;
            SetUIButtonState(!m_bActionFlag);

            sCompleteMessage = "Action Complete";
            OutputMessage(OutputMessageType.LabelMessage, sCompleteMessage);

            return true;
        }

        private bool OutputGoDrawActionInfo(ref double dTotalDistance, 
                                            ref int nTotalCostTime, 
                                            GoDrawCommand eCommandType, 
                                            int nCoordinateX = 0, 
                                            int nCoordinateY = 0, 
                                            double dSpeed = 0.0, 
                                            int nSleepTime = 0, 
                                            int nMoveDistance = 0, 
                                            int nZServoValue = 0)
        {
            switch(eCommandType)
            {
                case GoDrawCommand.Home:
                    string sMessage = "-Send Info: Back to Origin(X=0mm, Y=0mm)";
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                    break;
                case GoDrawCommand.Hover:
                    sMessage = string.Format("-Send Info: Hover Z={0}Servo Value", m_nHoverServoValue);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);

                    if (CheckValidRange(eCommandType) == false)
                        return true;

                    m_nCurrentServoValue = m_nHoverServoValue;
                    break;
                case GoDrawCommand.Contact:
                    sMessage = string.Format("-Send Info: Contact Z={0}Servo Value", m_nContactServoValue);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);

                    if (CheckValidRange(eCommandType) == false)
                        return true;

                    m_nCurrentServoValue = m_nContactServoValue;
                    break;
                case GoDrawCommand.Top:
                    sMessage = string.Format("-Send Info: Top Z={0}Servo Value", m_nTopServoValue);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);

                    if (CheckValidRange(eCommandType) == false)
                        return true;

                    m_nCurrentServoValue = m_nTopServoValue;
                    break;
                case GoDrawCommand.ZMove:
                    sMessage = string.Format("-Send Info: Z={0}Servo Value", nZServoValue);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);

                    if (CheckValidRange(eCommandType, nZServoValue) == false)
                        return true;

                    m_nCurrentServoValue = nZServoValue;
                    break;
                case GoDrawCommand.Move:
                    sMessage = string.Format("-Send Info: Move to X={0}mm, Y={1}mm, Speed={2}mm/s", nCoordinateX, nCoordinateY, dSpeed);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);

                    if (CheckValidRange(eCommandType, nCoordinateX, nCoordinateY) == false)
                        return true;

                    break;
                case GoDrawCommand.Left:
                    sMessage = string.Format("-Send Info: Move Left, Distance={0}mm, Speed={1}mm/s", nMoveDistance, dSpeed);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);

                    if (CheckValidRange(eCommandType, nMoveDistance) == false)
                        return true;

                    break;
                case GoDrawCommand.Right:
                    sMessage = string.Format("-Send Info: Move Right, Distance={0}mm, Speed={1}mm/s", nMoveDistance, dSpeed);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);

                    if (CheckValidRange(eCommandType, nMoveDistance) == false)
                        return true;

                    break;
                case GoDrawCommand.Down:
                    sMessage = string.Format("-Send Info: Move Down, Distance={0}mm, Speed={1}mm/s", nMoveDistance, dSpeed);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);

                    if (CheckValidRange(eCommandType, nMoveDistance) == false)
                        return true;

                    break;
                case GoDrawCommand.Up:
                    sMessage = string.Format("-Send Info: Move Up, Distance={0}mm, Speed={1}mm/s", nMoveDistance, dSpeed);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);

                    if (CheckValidRange(eCommandType, nMoveDistance) == false)
                        return true;

                    break;
                default:
                    break;
            }

            switch (eCommandType)
            {
                case GoDrawCommand.Home:
                    dTotalDistance = ComputeDistance(0, 0);
                    nTotalCostTime = (int)(dTotalDistance * m_nTimeCoefficent / dSpeed);
                    break;
                case GoDrawCommand.Move:
                    dTotalDistance = ComputeDistance(nCoordinateX, nCoordinateY);
                    nTotalCostTime = (int)(dTotalDistance * m_nTimeCoefficent / dSpeed);
                    break;
                case GoDrawCommand.Left:
                case GoDrawCommand.Right:
                case GoDrawCommand.Down:
                case GoDrawCommand.Up:
                    dTotalDistance = nMoveDistance;
                    nTotalCostTime = (int)(dTotalDistance * m_nTimeCoefficent / dSpeed);
                    break;
                default:
                    break;
            }

            return false;
        }

        private bool CheckValidRange(GoDrawCommand eCommandType, int nValue1 = 0, int nValue2 = 0)
        {
            switch (eCommandType)
            {
                case GoDrawCommand.Hover:
                case GoDrawCommand.Contact:
                case GoDrawCommand.Top:
                case GoDrawCommand.ZMove:
                    int nServoValue = 0;

                    if (eCommandType == GoDrawCommand.Hover)
                        nServoValue = m_nHoverServoValue;
                    else if (eCommandType == GoDrawCommand.Contact)
                        nServoValue = m_nContactServoValue;
                    else if (eCommandType == GoDrawCommand.Top)
                        nServoValue = m_nTopServoValue;
                    else if (eCommandType == GoDrawCommand.ZMove)
                        nServoValue = nValue1;

                    if (nServoValue < m_nMinServoValue || nServoValue > m_nMaxServoValue)
                    {
                        string sMessage = string.Format("-ServoValue({0}) Range Error(Range:{1}~{2}). Please Check it", nServoValue, m_nMinServoValue, m_nMaxServoValue);
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                        sMessage = string.Format("ServoValue({0}) Range Error(Range:{1}~{2}). Please Check it", nServoValue, m_nMinServoValue, m_nMaxServoValue);
                        OutputMessage(OutputMessageType.LabelMessage, sMessage);
                        return false;
                    }

                    break;
                case GoDrawCommand.Move:
                    //Move by X,Y
                    if (nValue1 > m_nMaxCoordinateX || nValue2 > m_nMaxCoordinateY)
                    {
                        string sMessage = string.Format("-X, Y Coordinate Error(X:0~{0}, Y:0~{1}). Please Check it", m_nMaxCoordinateX, m_nMaxCoordinateY);
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                        sMessage = string.Format("X, Y Coordinate Error(X:0~{0}, Y:0~{1}). Please Check it", m_nMaxCoordinateX, m_nMaxCoordinateY);
                        OutputMessage(OutputMessageType.LabelMessage, sMessage);
                        return false;
                    }

                    break;
                case GoDrawCommand.Left:
                case GoDrawCommand.Right:
                case GoDrawCommand.Down:
                case GoDrawCommand.Up:
                    double dDestinationCoordinate = 0.0;
                    int nMaxCoordinate = 0;
                    string sAxis = "";
                    bool bErrorFlag = false;

                    if (eCommandType == GoDrawCommand.Left)
                    {
                        dDestinationCoordinate = m_dCurrentCoordinateX - nValue1;
                        nMaxCoordinate = m_nMaxCoordinateX;
                        sAxis = "X";

                        if (dDestinationCoordinate < 0.0)
                            bErrorFlag = true;
                    }
                    else if (eCommandType == GoDrawCommand.Right)
                    {
                        dDestinationCoordinate = m_dCurrentCoordinateX + nValue1;
                        nMaxCoordinate = m_nMaxCoordinateX;
                        sAxis = "X";

                        if (dDestinationCoordinate > nMaxCoordinate)
                            bErrorFlag = true;
                    }
                    else if (eCommandType == GoDrawCommand.Down)
                    {
                        dDestinationCoordinate = m_dCurrentCoordinateY + nValue1;
                        nMaxCoordinate = m_nMaxCoordinateY;
                        sAxis = "Y";

                        if (dDestinationCoordinate > nMaxCoordinate)
                            bErrorFlag = true;
                    }
                    else if (eCommandType == GoDrawCommand.Up)
                    {
                        dDestinationCoordinate = m_dCurrentCoordinateY - nValue1;
                        nMaxCoordinate = m_nMaxCoordinateY;
                        sAxis = "Y";

                        if (dDestinationCoordinate < 0.0)
                            bErrorFlag = true;
                    }

                    if (bErrorFlag == true)
                    {
                        string sMessage = string.Format("-{0}({1}) Boundary Error({0}:0~{2}). Please Check it", sAxis, dDestinationCoordinate, nMaxCoordinate);
                        OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                        sMessage = string.Format("{0}({1}) Boundary Error({0}:0~{2}). Please Check it", sAxis, dDestinationCoordinate, nMaxCoordinate);
                        OutputMessage(OutputMessageType.LabelMessage, sMessage);
                        return false;
                    }

                    break;
                default:
                    break;
            }

            return true;
        }

        private double ComputeDistance(int nCoordinateX, int nCoordinateY)
        {
            double dDistanceX = Math.Round(nCoordinateX - m_dCurrentCoordinateX, 1, MidpointRounding.AwayFromZero);
            double dDistanceY = Math.Round(nCoordinateY - m_dCurrentCoordinateY, 1, MidpointRounding.AwayFromZero);

            return Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY);
        }

        private List<GoDrawCommandInfo> GetGoDrawCommandInfo(ref bool bErrorFlag, 
                                                             GoDrawCommand eCommandType, 
                                                             int nTotalCostTime, 
                                                             int nDelayTime, 
                                                             int nCoordinateX = 0, 
                                                             int nCoordinateY = 0, 
                                                             double dSpeed = 0.0, 
                                                             int nSleepTime = 0, 
                                                             int nMoveDistance = 0, 
                                                             int nZServoValue = 0)
        {
            if (bErrorFlag == true)
                return null;

            double dRealSpeed = dSpeed;

            if (nDelayTime > 0 && m_nEstimateType != 0)
            {
                if (dSpeed < m_dCauseDelayTimeMaxSpeed)
                {
                    dRealSpeed = dSpeed * ((double)(nDelayTime + nTotalCostTime) / nTotalCostTime);
                    dRealSpeed = (Math.Ceiling(dRealSpeed * 100000) / 100000);

                    string sMessage = string.Format("-Set Real Speed={0}mm/s(Set Speed={1}mm/s < {2}mm/s)", dRealSpeed, dSpeed, m_dCauseDelayTimeMaxSpeed);
                    OutputMessage(OutputMessageType.RichTextBoxMessage, sMessage);
                }
            }

            List<GoDrawCommandInfo> cGoDrawCommand_List = new List<GoDrawCommandInfo>();
            cGoDrawCommand_List.Clear();

            int nCostTime = 0;
            string[] sCommand_Array;
            GoDrawCommandInfo cGoDrawCommand;

            double dCoordinateX = 0.0;
            double dCoordinateY = 0.0;

            if (m_nMoveType == 1)
            {
                switch(eCommandType)
                {
                    case GoDrawCommand.Move:
                        if (dSpeed < m_dCauseDelayTimeMaxSpeed)
                        {
                            double dDistanceX = Math.Round(nCoordinateX - m_dDesitinationCoordinateX, 1, MidpointRounding.AwayFromZero);
                            double dDistanceY = Math.Round(nCoordinateY - m_dDesitinationCoordinateY, 1, MidpointRounding.AwayFromZero);

                            double dTotalDistance = Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY);
                            int nSetCostTime = (int)(dTotalDistance * m_nTimeCoefficent / dRealSpeed);

                            int nSplitCount = nSetCostTime / m_nSplitTime;

                            if (nSetCostTime % m_nSplitTime != 0)
                                nSplitCount++;

                            double dSplitDistance = ((double)m_nSplitTime / 1000) * dRealSpeed;
                            double dSplitDistanceX = dDistanceX * (dSplitDistance / dTotalDistance);
                            double dSplitDistanceY = dDistanceY * (dSplitDistance / dTotalDistance);

                            double[,] dSplitCoordinate_Array = new double[nSplitCount, 2];

                            for (int nCountIndex = 0; nCountIndex < nSplitCount; nCountIndex++)
                            {
                                if (nCountIndex == nSplitCount - 1)
                                {
                                    if (nSetCostTime % m_nSplitTime != 0)
                                    {
                                        double dLastSplitRatio = (double)(nSetCostTime % m_nSplitTime) / m_nSplitTime;
                                        double dLastSplitDistanceX = Math.Round(dSplitDistanceX * dLastSplitRatio, 1, MidpointRounding.AwayFromZero);
                                        double dLastSplitDistanceY = Math.Round(dSplitDistanceY * dLastSplitRatio, 1, MidpointRounding.AwayFromZero);

                                        if (dLastSplitDistanceX < 0.0125)
                                            dLastSplitDistanceX = 0.0125;

                                        if (dLastSplitDistanceY < 0.0125)
                                            dLastSplitDistanceY = 0.0125;

                                        double dLastCoordinateX = m_dDesitinationCoordinateX + dSplitDistanceX * nCountIndex + dLastSplitDistanceX;
                                        double dLastCoordinateY = m_dDesitinationCoordinateY + dSplitDistanceY * nCountIndex + dLastSplitDistanceY;

                                        dSplitCoordinate_Array[nCountIndex, 0] = dLastCoordinateX;
                                        dSplitCoordinate_Array[nCountIndex, 1] = dLastCoordinateY;
                                    }
                                    else
                                    {
                                        dSplitCoordinate_Array[nCountIndex, 0] = m_dDesitinationCoordinateX + dSplitDistanceX * (nCountIndex + 1);
                                        dSplitCoordinate_Array[nCountIndex, 1] = m_dDesitinationCoordinateY + dSplitDistanceY * (nCountIndex + 1);
                                    }
                                }
                                else
                                {
                                    dSplitCoordinate_Array[nCountIndex, 0] = m_dDesitinationCoordinateX + dSplitDistanceX * (nCountIndex + 1);
                                    dSplitCoordinate_Array[nCountIndex, 1] = m_dDesitinationCoordinateY + dSplitDistanceY * (nCountIndex + 1);
                                }
                            }

                            for (int nCountIndex = 0; nCountIndex < nSplitCount; nCountIndex++)
                            {
                                nCostTime = 0;
                                sCommand_Array = SetGoDrawCommandScript(ref bErrorFlag, ref nCostTime, eCommandType, dSplitCoordinate_Array[nCountIndex, 0], dSplitCoordinate_Array[nCountIndex, 1], dRealSpeed);

                                cGoDrawCommand = new GoDrawCommandInfo();
                                cGoDrawCommand.nCostTime = nCostTime;
                                cGoDrawCommand.arrsCommand = sCommand_Array;
                                cGoDrawCommand.dCoordinateX = dSplitCoordinate_Array[nCountIndex, 0];
                                cGoDrawCommand.dCoordinateY = dSplitCoordinate_Array[nCountIndex, 1];
                                cGoDrawCommand_List.Add(cGoDrawCommand);
                            }
                        }
                        else
                        {
                            nCostTime = 0;
                            sCommand_Array = SetGoDrawCommandScript(ref bErrorFlag, ref nCostTime, eCommandType, nCoordinateX, nCoordinateY, dRealSpeed, nSleepTime, nMoveDistance, nZServoValue);

                            cGoDrawCommand = new GoDrawCommandInfo();
                            cGoDrawCommand.nCostTime = nCostTime;
                            cGoDrawCommand.arrsCommand = sCommand_Array;
                            cGoDrawCommand.dCoordinateX = nCoordinateX;
                            cGoDrawCommand.dCoordinateY = nCoordinateY;
                            cGoDrawCommand_List.Add(cGoDrawCommand);
                        }
                        break;
                    case GoDrawCommand.Left:
                    case GoDrawCommand.Right:
                    case GoDrawCommand.Down:
                    case GoDrawCommand.Up:
                        if (dSpeed < m_dCauseDelayTimeMaxSpeed)
                        {
                            int nSetCostTime = (int)(nMoveDistance * m_nTimeCoefficent / dRealSpeed);

                            int nSplitCount = nSetCostTime / m_nSplitTime;

                            if (nSetCostTime % m_nSplitTime != 0)
                                nSplitCount++;

                            double dSplitDistance = ((double)m_nSplitTime / 1000) * dRealSpeed;

                            double[] dSplitDistance_Array = new double[nSplitCount];

                            for (int nCountIndex = 0; nCountIndex < nSplitCount; nCountIndex++)
                            {
                                if (nCountIndex == nSplitCount - 1)
                                {
                                    if (nSetCostTime % m_nSplitTime != 0)
                                    {
                                        double dLastSplitRatio = (double)(nSetCostTime % m_nSplitTime) / m_nSplitTime;

                                        double dLastDistance = Math.Round(dSplitDistance * dLastSplitRatio, 2, MidpointRounding.AwayFromZero);

                                        if (dLastDistance < 0.0125)
                                            dLastDistance = 0.0125;

                                        dSplitDistance_Array[nCountIndex] = dLastDistance;
                                    }
                                    else
                                        dSplitDistance_Array[nCountIndex] = dSplitDistance;
                                }
                                else
                                    dSplitDistance_Array[nCountIndex] = dSplitDistance;
                            }

                            double dDistance = 0;

                            for (int nCountIndex = 0; nCountIndex < nSplitCount; nCountIndex++)
                            {
                                nCostTime = 0;
                                sCommand_Array = SetGoDrawCommandScript(ref bErrorFlag, ref nCostTime, eCommandType, 0.0, 0.0, dRealSpeed, 0, dSplitDistance_Array[nCountIndex]);

                                cGoDrawCommand = new GoDrawCommandInfo();
                                cGoDrawCommand.nCostTime = nCostTime;
                                cGoDrawCommand.arrsCommand = sCommand_Array;

                                dDistance += dSplitDistance_Array[nCountIndex];
                                SetCoordinate(eCommandType, ref dCoordinateX, ref dCoordinateY, dDistance, nCoordinateX, nCoordinateY);
                                cGoDrawCommand.dCoordinateX = dCoordinateX;
                                cGoDrawCommand.dCoordinateY = dCoordinateY;
                                cGoDrawCommand_List.Add(cGoDrawCommand);
                            }
                        }
                        else
                        {
                            nCostTime = 0;
                            sCommand_Array = SetGoDrawCommandScript(ref bErrorFlag, ref nCostTime, eCommandType, nCoordinateX, nCoordinateY, dRealSpeed, nSleepTime, nMoveDistance, nZServoValue);

                            cGoDrawCommand = new GoDrawCommandInfo();
                            cGoDrawCommand.nCostTime = nCostTime;
                            cGoDrawCommand.arrsCommand = sCommand_Array;
                            SetCoordinate(eCommandType, ref dCoordinateX, ref dCoordinateY, nMoveDistance, nCoordinateX, nCoordinateY);
                            cGoDrawCommand.dCoordinateX = dCoordinateX;
                            cGoDrawCommand.dCoordinateY = dCoordinateY;
                            cGoDrawCommand_List.Add(cGoDrawCommand);
                        }
                        break;
                    default:
                        nCostTime = 0;
                        sCommand_Array = SetGoDrawCommandScript(ref bErrorFlag, ref nCostTime, eCommandType, nCoordinateX, nCoordinateY, dRealSpeed, nSleepTime, nMoveDistance, nZServoValue);

                        cGoDrawCommand = new GoDrawCommandInfo();
                        cGoDrawCommand.nCostTime = nCostTime;
                        cGoDrawCommand.arrsCommand = sCommand_Array;
                        SetCoordinate(eCommandType, ref dCoordinateX, ref dCoordinateY, nMoveDistance, nCoordinateX, nCoordinateY);
                        cGoDrawCommand.dCoordinateX = dCoordinateX;
                        cGoDrawCommand.dCoordinateY = dCoordinateY;
                        cGoDrawCommand_List.Add(cGoDrawCommand);
                        break;
                }
            }
            else
            {
                nCostTime = 0;
                sCommand_Array = SetGoDrawCommandScript(ref bErrorFlag, ref nCostTime, eCommandType, nCoordinateX, nCoordinateY, dRealSpeed, nSleepTime, nMoveDistance, nZServoValue);

                cGoDrawCommand = new GoDrawCommandInfo();
                cGoDrawCommand.nCostTime = nCostTime;
                cGoDrawCommand.arrsCommand = sCommand_Array;
                SetCoordinate(eCommandType, ref dCoordinateX, ref dCoordinateY, nMoveDistance, nCoordinateX, nCoordinateY);
                cGoDrawCommand.dCoordinateX = dCoordinateX;
                cGoDrawCommand.dCoordinateY = dCoordinateY;
                cGoDrawCommand_List.Add(cGoDrawCommand);
            }

            return cGoDrawCommand_List;
        }

        private void SetCoordinate(GoDrawCommand eCommandType, ref double dCoordinateX, ref double dCoordinateY, double dMoveDistance, int nDestinationCoordinateX, int nDestinationCoordinateY)
        {
            switch (eCommandType)
            {
                case GoDrawCommand.Home:
                    dCoordinateX = 0.0;
                    dCoordinateY = 0.0;
                    break;
                case GoDrawCommand.Right:
                    dCoordinateX = m_dCurrentCoordinateX + dMoveDistance;
                    dCoordinateY = m_dCurrentCoordinateY;
                    break;
                case GoDrawCommand.Left:
                    dCoordinateX = m_dCurrentCoordinateX - dMoveDistance;
                    dCoordinateY = m_dCurrentCoordinateY;
                    break;
                case GoDrawCommand.Down:
                    dCoordinateX = m_dCurrentCoordinateX;
                    dCoordinateY = m_dCurrentCoordinateY + dMoveDistance;
                    break;
                case GoDrawCommand.Up:
                    dCoordinateX = m_dCurrentCoordinateX;
                    dCoordinateY = m_dCurrentCoordinateY - dMoveDistance;
                    break;
                case GoDrawCommand.Move:
                    dCoordinateX = nDestinationCoordinateX;
                    dCoordinateY = nDestinationCoordinateY;
                    break;
                default:
                    dCoordinateX = m_dCurrentCoordinateX;
                    dCoordinateY = m_dCurrentCoordinateY;
                    break;
            }
        }

        private RunCommandState RunGoDrawCommand(GoDrawCommand eGoDrawCommand, string sCommand, int nCostTime, int nThreadID = 0, bool bWaitTimeout = true)
        {
            string[] sSplit_Array = sCommand.Split(',');

            /*
            if (eGoDrawCommand == GoDrawCommand.Stop)
                m_bStop = true;
            */

            while (true)
            {
                //if (lb_ComReceive.Text.Contains("OK"))
                //if (m_bResetFlag == true || m_bStop == true)
                if (m_bResetFlag == true || eGoDrawCommand == GoDrawCommand.Stop)
                {
                    /*
                    string[] sSplit_Array = tb_ComInput.Text.Split(',');
                    
                    if (sSplit_Array[0].Contains("VerSEnd"))
                    {
                        label_X.Text = arrsSplit[1];
                        label_Y.Text = arrsSplit[2];
                        return;
                    }
                    */

                    if (sSplit_Array[0].Contains("VerSEnd"))
                    {
                        if (sSplit_Array.Length >= 3)
                        {
                            m_dDesitinationCoordinateX = Convert.ToDouble(sSplit_Array[1]);
                            m_dDesitinationCoordinateY = Convert.ToDouble(sSplit_Array[2]);
                            return RunCommandState.Success;
                        }
                        else
                            return RunCommandState.Error_Other;
                    }

                    if (!m_bCOMPortConnected)
                        RunConnectSerialPort();

                    if (!m_bCOMPortConnected)
                        return RunCommandState.Error_Other;

                    //lb_ComReceive.Text = "";
                    m_bResetFlag = false;
                    Byte[] byteData_Array = new byte[1];
                    byteData_Array[0] = 0x0d;
                    ASCIIEncoding asciieEncode = new ASCIIEncoding();
                    String sDecoded = asciieEncode.GetString(byteData_Array);
                    SendData(Encoding.ASCII.GetBytes(sCommand + sDecoded));

                    DateTime dtMoveStartTime = DateTime.Now;

                    if (eGoDrawCommand == GoDrawCommand.Stop)
                        m_bStop = true;
                    //else
                        //m_bSend = true;

                    /*
                    RunCommandState eRunCommandState = CheckRecieveData();

                    if (eRunCommandState != RunCommandState.Success)
                        return eRunCommandState;
                    */

                    if (bWaitTimeout == false)
                        break;

                    if (sSplit_Array[0].ToUpper() == "SM")
                    {
                        m_nCurrentThreadID = nThreadID;
                        /*
                        double dCoefficient1 = Convert.ToDouble(arrsSplit[2]) / 80;
                        double dCoefficient2 = Convert.ToDouble(arrsSplit[3]) / 80;

                        double dCoordinateX = (dCoefficient1 + dCoefficient2) / 2;
                        double dCoordinateY = dCoefficient1 - dCoordinateX;
                        m_dCurrentCoordinateX = Math.Round(m_dCurrentCoordinateX + dCoordinateX, 1, MidpointRounding.AwayFromZero);
                        m_dCurrentCoordinateY = Math.Round(m_dCurrentCoordinateY + dCoordinateY, 1, MidpointRounding.AwayFromZero);
                        */

                        /*
                        if (Convert.ToInt32(sSplit_Array[1]) <= 100)
                            Thread.Sleep(Convert.ToInt32(sSplit_Array[1]));
                        else
                            Thread.Sleep(Convert.ToInt32(sSplit_Array[1]));    // - 100);
                        

                        m_dCurrentCoordinateX = m_dPreviousCoordinateX;
                        m_dCurrentCoordinateY = m_dPreviousCoordinateY;
                        */

                        //int nCostTime = Convert.ToInt32(sSplit_Array[1]);
                        int nTotalCostTime = nCostTime;

                        while (true)
                        {
                            Thread.Sleep(1);

                            double dTimeInterval_ms = (DateTime.Now - dtMoveStartTime).TotalMilliseconds;

                            double dOutputCoordinateX = 0.0;
                            double dOutputCoordinateY = 0.0;

                            if (dTimeInterval_ms >= nTotalCostTime)
                            {
                                if (m_nCurrentThreadID == nThreadID)
                                {
                                    m_dCurrentCoordinateX = m_dDesitinationCoordinateX;
                                    m_dCurrentCoordinateY = m_dDesitinationCoordinateY;

                                    dOutputCoordinateX = m_dCurrentCoordinateX;
                                    dOutputCoordinateY = m_dCurrentCoordinateY;

                                    OutputCoordinate(dOutputCoordinateX, dOutputCoordinateY, m_nCurrentServoValue);
                                }
                                break;
                            }
                            else
                            {
                                if (m_nCurrentThreadID == nThreadID)
                                {
                                    double dIntervalX = (m_dDesitinationCoordinateX - m_dPreviousCoordinateX) * (dTimeInterval_ms / nTotalCostTime);
                                    double dIntervalY = (m_dDesitinationCoordinateY - m_dPreviousCoordinateY) * (dTimeInterval_ms / nTotalCostTime);

                                    m_dCurrentCoordinateX = dIntervalX + m_dPreviousCoordinateX;
                                    m_dCurrentCoordinateY = dIntervalY + m_dPreviousCoordinateY;

                                    dOutputCoordinateX = Math.Round(m_dCurrentCoordinateX, 1, MidpointRounding.AwayFromZero);
                                    dOutputCoordinateY = Math.Round(m_dCurrentCoordinateY, 1, MidpointRounding.AwayFromZero);

                                    OutputCoordinate(dOutputCoordinateX, dOutputCoordinateY, m_nCurrentServoValue);
                                }
                                else
                                    break;
                            }

                            if (m_bStop == true)
                            {
                                if (m_nCurrentThreadID == nThreadID)
                                {
                                    m_dDesitinationCoordinateX = m_dCurrentCoordinateX;
                                    m_dDesitinationCoordinateY = m_dCurrentCoordinateY;
                                }
                                break;
                            }
                        }

                        /*
                        int nIntervalTime = 100;

                        int nDividCount = nDelayTime / nIntervalTime;

                        if (nDelayTime % nIntervalTime != 0)
                            nDividCount++;

                        double dIntervalCoorinateX = (m_dDesitinationCoordinateX - m_dPreviousCoordinateX) / nDividCount;
                        double dIntervalCoorinateY = (m_dDesitinationCoordinateY - m_dPreviousCoordinateY) / nDividCount;

                        for (int nIntervalIndex = 0; nIntervalIndex < nDividCount; nIntervalIndex++)
                        {
                            if (m_bStop == true)
                                break;

                            Thread.Sleep(nIntervalTime);

                            double dOutputCoordinateX = 0.0;
                            double dOutputCoordinateY = 0.0;

                            if (nIntervalIndex == nDividCount - 1)
                            {
                                m_dCurrentCoordinateX = m_dDesitinationCoordinateX;
                                m_dCurrentCoordinateY = m_dDesitinationCoordinateY;

                                dOutputCoordinateX = m_dCurrentCoordinateX;
                                dOutputCoordinateY = m_dCurrentCoordinateY;
                            }
                            else
                            {
                                m_dCurrentCoordinateX += dIntervalCoorinateX;
                                m_dCurrentCoordinateY += dIntervalCoorinateY;

                                if (nIntervalIndex % 1 == 0)
                                {
                                    dOutputCoordinateX = Math.Round(m_dCurrentCoordinateX, 1, MidpointRounding.AwayFromZero);
                                    dOutputCoordinateY = Math.Round(m_dCurrentCoordinateY, 1, MidpointRounding.AwayFromZero);
                                }
                            }

                            if (nIntervalIndex == nDividCount - 1 || nIntervalIndex % 1 == 0)
                                OutputCoordinate(dOutputCoordinateX, dOutputCoordinateY, m_nCurrentServoValue);
                        }
                        */
                    }
                    else if (sSplit_Array[0].ToUpper() == "TP")
                        Thread.Sleep(1000);
                    else if (sSplit_Array[0].ToUpper() == "SP")
                        Thread.Sleep(Convert.ToInt32(sSplit_Array[2]));
                    else
                        Thread.Sleep(100);

                    break;
                }

                Thread.Sleep(10);
            }

            m_bResetFlag = true;

            return RunCommandState.Success;
        }

        private RunCommandState CheckRecieveData()
        {
            bool bGetRecieveData = false;

            for (int nSleepIndex = 0; nSleepIndex < 100; nSleepIndex++)
            {
                if (m_bReceiveData == true)
                {
                    if (m_eRunCommandState != RunCommandState.Success)
                    {
                        bGetRecieveData = true;
                        m_bResetFlag = true;
                        m_bReceiveData = false;
                        return m_eRunCommandState;
                    }
                    else
                    {
                        bGetRecieveData = true;
                        m_bReceiveData = false;
                        break;
                    }
                }

                Thread.Sleep(10);
            }

            if (bGetRecieveData == false)
            {
                m_bResetFlag = true;
                return RunCommandState.Error_Other;
            }

            return m_eRunCommandState;
        }

        private string[] SetGoDrawCommandScript(ref bool bErrorFlag, 
                                                ref int nCostTime, 
                                                GoDrawCommand eCommandType, 
                                                double dCoordinateX = 0.0, 
                                                double dCoordinateY = 0.0, 
                                                double dSpeed = 0.0, 
                                                int nSleepTime = 0, 
                                                double dMoveDistance = 0, 
                                                int nZServoValue = 0)
        {
            string[] sCommandScript_Array;

            switch (eCommandType)
            {
                case GoDrawCommand.Sleep:
                    sCommandScript_Array = new string[1];
                    sCommandScript_Array[0] = string.Format("Sleep,{0}", nSleepTime);
                    break;
                case GoDrawCommand.Home:
                    sCommandScript_Array = new string[1];
                    //double nDistance = Math.Sqrt(m_dDesitinationCoordinateX * m_dDesitinationCoordinateX + m_dDesitinationCoordinateY * m_dDesitinationCoordinateY);
                    double dDistanceX = Math.Round(0.0 - m_dDesitinationCoordinateX, 1, MidpointRounding.AwayFromZero);
                    double dDistanceY = Math.Round(0.0 - m_dDesitinationCoordinateY, 1, MidpointRounding.AwayFromZero);

                    //int nConvertSpeed = (int)(nDistance * 1000 / dSpeed);
                    //nConvertSpeed = (nConvertSpeed < 10) ? 10 : nConvertSpeed;
                    //int nConvertSpeed = (int)(Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY) * m_nTimeCoefficent / dSpeed);
                    nCostTime = (int)(Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY) * m_nTimeCoefficent / dSpeed);

                    double dFitSpeed = dSpeed;

                    /*
                    if (dSpeed < m_dCauseDelayTimeMaxSpeed)
                        dFitSpeed = ComputeCurveFitValue(dSpeed);
                    */

                    int nConvertSpeed = (int)(Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY) * m_nTimeCoefficent / dFitSpeed);
                    /*
                    sCommandScript_Array[0] = string.Format("SM,{0},{1},{2}", 
                     *                                      nConvertSpeed.ToString(),
                                                            (-1 * (m_dDesitinationCoordinateX + m_dDesitinationCoordinateY) * 80).ToString(),
                                                            (-1 * (m_dDesitinationCoordinateX - m_dDesitinationCoordinateY) * 80).ToString());
                    */
                    sCommandScript_Array[0] = string.Format("SM,{0},{1},{2}", 
                                                            nConvertSpeed.ToString(),
                                                            ((dDistanceX + dDistanceY) * m_nDistanceCoefficent).ToString(), 
                                                            ((dDistanceX - dDistanceY) * m_nDistanceCoefficent).ToString());
                    break;
                case GoDrawCommand.Hover:
                    sCommandScript_Array = new string[2];
                    sCommandScript_Array[0] = string.Format("SC,5,{0}", m_nHoverServoValue.ToString());
                    sCommandScript_Array[1] = "SP,0,400";
                    break;
                case GoDrawCommand.Contact:
                    sCommandScript_Array = new string[2];
                    sCommandScript_Array[0] = string.Format("SC,5,{0}", m_nContactServoValue.ToString());
                    sCommandScript_Array[1] = "SP,0,400";
                    break;
                case GoDrawCommand.Top:
                    sCommandScript_Array = new string[2];
                    sCommandScript_Array[0] = string.Format("SC,5,{0}", m_nTopServoValue.ToString());
                    sCommandScript_Array[1] = "SP,0,400";
                    /*
                    sCommandScript_Array = new string[1];
                    sCommandScript_Array[0] = "SP,1,200";
                    */
                    break;
                case GoDrawCommand.ZMove:
                    sCommandScript_Array = new string[2];
                    sCommandScript_Array[0] = string.Format("SC,5,{0}", nZServoValue.ToString());
                    sCommandScript_Array[1] = "SP,0,400";
                    break;
                case GoDrawCommand.Move:
                    dDistanceX = Math.Round(dCoordinateX - m_dDesitinationCoordinateX, 1, MidpointRounding.AwayFromZero);
                    dDistanceY = Math.Round(dCoordinateY - m_dDesitinationCoordinateY, 1, MidpointRounding.AwayFromZero);

                    //nConvertSpeed = (int)(Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY) * m_nTimeCoefficent / dSpeed);
                    nCostTime = (int)(Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY) * m_nTimeCoefficent / dSpeed);

                    dFitSpeed = dSpeed;

                    /*
                    if (dSpeed < m_dCauseDelayTimeMaxSpeed)
                        dFitSpeed = ComputeCurveFitValue(dSpeed);
                    */

                    nConvertSpeed = (int)(Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY) * m_nTimeCoefficent / dFitSpeed);

                    sCommandScript_Array = new string[1];
                    sCommandScript_Array[0] = string.Format("SM,{0},{1},{2}", 
                                                            nConvertSpeed.ToString(), 
                                                            ((dDistanceX + dDistanceY) * m_nDistanceCoefficent).ToString(), 
                                                            ((dDistanceX - dDistanceY) * m_nDistanceCoefficent).ToString());

                    /*
                    m_dPreviousCoordinateX = m_dDesitinationCoordinateX;
                    m_dPreviousCoordinateY = m_dDesitinationCoordinateY;

                    m_dDesitinationCoordinateX = dCoordinateX;
                    m_dDesitinationCoordinateY = dCoordinateY;
                    */
                    break;
                case GoDrawCommand.VMove:
                    dDistanceX = (dCoordinateX - m_dDesitinationCoordinateX) * 10;
                    dDistanceY = (dCoordinateY - m_dDesitinationCoordinateY) * 10;
                    double dParameter1 = dDistanceX + dDistanceY;
                    double dParameter2 = dDistanceX - dDistanceY;
                    //if (dParameter1 > 1000)
                    //nConvertSpeed = (int)(Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY) * m_nTimeCoefficent / dSpeed) / 10;
                    //int nConvertSpeed = (int)(Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY) * 1000 / dSpeed);

                    dFitSpeed = dSpeed;

                    /*
                    if (dSpeed < m_dCauseDelayTimeMaxSpeed)
                        dFitSpeed = ComputeCurveFitValue(dSpeed);
                    */

                    nConvertSpeed = (int)(Math.Sqrt(dDistanceX * dDistanceX + dDistanceY * dDistanceY) * m_nTimeCoefficent / dFitSpeed) / 10;

                    sCommandScript_Array = new string[6];
                    sCommandScript_Array[0] = "VerSStart";
                    sCommandScript_Array[1] = string.Format("SM,{0},{1},{2}", ((int)(1.0 * nConvertSpeed / 8)).ToString(), (1 * dParameter1).ToString(), (1 * dParameter2).ToString());
                    sCommandScript_Array[2] = string.Format("SM,{0},{1},{2}", ((int)(1.8 * nConvertSpeed / 8)).ToString(), (2 * dParameter1).ToString(), (2 * dParameter2).ToString());
                    sCommandScript_Array[3] = string.Format("SM,{0},{1},{2}", ((int)(2.0 * nConvertSpeed / 8)).ToString(), (3 * dParameter1).ToString(), (3 * dParameter2).ToString());
                    sCommandScript_Array[4] = string.Format("SM,{0},{1},{2}", ((int)(1.8 * nConvertSpeed / 8)).ToString(), (2 * dParameter1).ToString(), (2 * dParameter2).ToString());

                    m_dPreviousCoordinateX = m_dDesitinationCoordinateX;
                    m_dPreviousCoordinateY = m_dDesitinationCoordinateY;

                    m_dDesitinationCoordinateX = dCoordinateX;
                    m_dDesitinationCoordinateY = dCoordinateY;

                    sCommandScript_Array[5] = string.Format("VerSEnd,{0},{1}", m_dDesitinationCoordinateX.ToString(), m_dDesitinationCoordinateY.ToString());
                    break;
                case GoDrawCommand.Right:
                    nCostTime = (int)(dMoveDistance * m_nTimeCoefficent / dSpeed);

                    dFitSpeed = dSpeed;

                    /*
                    if (dSpeed < m_dCauseDelayTimeMaxSpeed)
                        dFitSpeed = ComputeCurveFitValue(dSpeed);
                    */

                    //nConvertSpeed = (int)(dMoveDistance * m_nTimeCoefficent / dSpeed);
                    nConvertSpeed = (int)(dMoveDistance * m_nTimeCoefficent / dFitSpeed);
                    int nMove = (int)(dMoveDistance * m_nDistanceCoefficent);

                    /*
                    m_dPreviousCoordinateX = m_dDesitinationCoordinateX;
                    m_dPreviousCoordinateY = m_dDesitinationCoordinateY;

                    m_dDesitinationCoordinateX += nMoveDistance;
                    */

                    sCommandScript_Array = new string[1];
                    sCommandScript_Array[0] = string.Format("SM,{0},{1},{2}", nConvertSpeed.ToString(), nMove.ToString(), nMove.ToString());
                    break;
                case GoDrawCommand.Left:
                    nCostTime = (int)(dMoveDistance * m_nTimeCoefficent / dSpeed);

                    dFitSpeed = dSpeed;

                    /*
                    if (dSpeed < m_dCauseDelayTimeMaxSpeed)
                        dFitSpeed = ComputeCurveFitValue(dSpeed);
                    */

                    //nConvertSpeed = (int)(dMoveDistance * m_nTimeCoefficent / dSpeed);
                    nConvertSpeed = (int)(dMoveDistance * m_nTimeCoefficent / dFitSpeed);
                    nMove = (int)(dMoveDistance * m_nDistanceCoefficent);

                    /*
                    m_dPreviousCoordinateX = m_dDesitinationCoordinateX;
                    m_dPreviousCoordinateY = m_dDesitinationCoordinateY;

                    m_dDesitinationCoordinateX -= nMoveDistance;
                    */

                    sCommandScript_Array = new string[1];
                    sCommandScript_Array[0] = string.Format("SM,{0},-{1},-{2}", nConvertSpeed.ToString(), nMove.ToString(), nMove.ToString());
                    break;
                case GoDrawCommand.Down:
                    nCostTime = (int)(dMoveDistance * m_nTimeCoefficent / dSpeed);

                    dFitSpeed = dSpeed;

                    /*
                    if (dSpeed < m_dCauseDelayTimeMaxSpeed)
                        dFitSpeed = ComputeCurveFitValue(dSpeed);
                    */

                    //nConvertSpeed = (int)(dMoveDistance * m_nTimeCoefficent / dSpeed);
                    nConvertSpeed = (int)(dMoveDistance * m_nTimeCoefficent / dFitSpeed);
                    nMove = (int)(dMoveDistance * m_nDistanceCoefficent);

                    /*
                    m_dPreviousCoordinateX = m_dDesitinationCoordinateX;
                    m_dPreviousCoordinateY = m_dDesitinationCoordinateY;

                    m_dDesitinationCoordinateY += nMoveDistance;
                    */

                    sCommandScript_Array = new string[1];
                    sCommandScript_Array[0] = string.Format("SM,{0},{1},-{2}", nConvertSpeed.ToString(), nMove.ToString(), nMove.ToString());
                    break;
                case GoDrawCommand.Up:
                    nCostTime = (int)(dMoveDistance * m_nTimeCoefficent / dSpeed);

                    dFitSpeed = dSpeed;

                    /*
                    if (dSpeed < m_dCauseDelayTimeMaxSpeed)
                        dFitSpeed = ComputeCurveFitValue(dSpeed);
                    */

                    //nConvertSpeed = (int)(dMoveDistance * m_nTimeCoefficent / dSpeed);
                    nConvertSpeed = (int)(dMoveDistance * m_nTimeCoefficent / dFitSpeed);
                    nMove = (int)(dMoveDistance * m_nDistanceCoefficent);

                    /*
                    m_dPreviousCoordinateX = m_dDesitinationCoordinateX;
                    m_dPreviousCoordinateY = m_dDesitinationCoordinateY;

                    m_dDesitinationCoordinateY -= nMoveDistance;
                    */

                    sCommandScript_Array = new string[1];
                    sCommandScript_Array[0] = string.Format("SM,{0},-{1},{2}", nConvertSpeed.ToString(), nMove.ToString(), nMove.ToString());
                    break;
                case GoDrawCommand.ZUpDown:
                    sCommandScript_Array = new string[7];
                    sCommandScript_Array[0] = string.Format("SC,4,{0}", m_nTopServoValue.ToString());
                    sCommandScript_Array[1] = string.Format("SC,5,{0}", m_nContactServoValue.ToString());
                    sCommandScript_Array[2] = "SC,11,1800";
                    sCommandScript_Array[3] = "SC,12,900";
                    sCommandScript_Array[4] = "Ql,Sl,0";
                    sCommandScript_Array[5] = "TP";
                    sCommandScript_Array[6] = "SM,10,0,0";
                    break;
                case GoDrawCommand.Stop:
                    sCommandScript_Array = new string[1];
                    sCommandScript_Array[0] = "ES";
                    break;
                default:
                    sCommandScript_Array = null;
                    break;
            }

            return sCommandScript_Array;
        }

        private void OutputMessage(OutputMessageType eOutputMessageType, string sMessage)
        {
            if (m_eControlUIType == ControlUIType.GoDrawController)
            {
                if (eOutputMessageType == OutputMessageType.LabelMessage)
                    m_cfrmGoDrawController.DisplayMessageLabel(sMessage);
                else if (eOutputMessageType == OutputMessageType.RichTextBoxMessage)
                    m_cfrmGoDrawController.DisplayProcessMessageRichTextBox(sMessage);
            }
            else if (m_eControlUIType == ControlUIType.MSPen_AutoTuning)
            {
                if (eOutputMessageType == OutputMessageType.LabelMessage)
                { }
                else if (eOutputMessageType == OutputMessageType.RichTextBoxMessage)
                    OutputMessage(sMessage);
            }
        }

        private void OutputCoordinate(double dCoordinateX, double dCoordinateY, int nZServoValue)
        {
            if (m_eControlUIType == ControlUIType.GoDrawController)
            {
                m_cfrmGoDrawController.DisplaylblCoordinate(dCoordinateX, dCoordinateY, nZServoValue);
            }
            else if (m_eControlUIType == ControlUIType.MSPen_AutoTuning)
            {
                /*
                string sMessage = string.Format("-X = {0}, Y = {1}", dCoordinateX.ToString("F1"), dCoordinateY.ToString("F1"));
                MessageOutput(sMessage);
                */
            }
        }

        private void SetUIButtonState(bool bEnable)
        {
            if (m_eControlUIType == ControlUIType.GoDrawController)
            {
                m_cfrmGoDrawController.SetButtonState(bEnable);
            }
            else if (m_eControlUIType == ControlUIType.MSPen_AutoTuning)
            {
            }
        }

        private void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmMain.OutputMessage(sMessage, bWarning);
        }

        private class ComboboxItem
        {
            public ComboboxItem(string sValue, string sText)
            {
                Value = sValue;
                Text = sText;
            }

            public string Value
            {
                get;
                set;
            }

            public string Text
            {
                get;
                set;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
