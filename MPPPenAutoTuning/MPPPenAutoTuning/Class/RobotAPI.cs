using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using BlockingQueue;
using System.Threading;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class RobotAPI
    {
        private frmMain m_cfrmMain;

        private Thread m_tRobotConnect = null;

        private const int m_nWAITSECONDTIMEOUT = 10;    //Original : 60

        private HW_ForceGauge_SHIMPO_FGP05 m_cFG;
        private HW_LT_DT_500F m_cLT;

        #region RobotCode, RobotCommend and MachineCode
        public enum RobotCode
        {
            Stop,
            ReturnOriginal,
            SetAbsoluteMove,
            GetPosition,
            SetLineAbsoluteMove,
            SetSpeed,
            IsMoving,
            SetWeight,
            GetWeight,
            SetWeightCalibration,
            SetAbsoluteZ
        }

        enum RobotCommand
        {
            STOP,
            ReturnOriginal,
            MoveAbsoluteXYZ,
            MoveAbsoluteX,
            MoveAbsoluteY,
            MoveAbsoluteZ,
            MoveRelativeXYZ,
            MoveRelativeX,
            MoveRelativeY,
            MoveRelativeZ,
            LineAbsoluteXYZ,
            LineRelativeXYZ,
            GetPositionXYZ,
            GetPositionX,
            GetPositionY,
            GetPositionZ,
            SetSpeed,
            IsMoving,
            SetWeight,
            GetWeight,
            SetWeightCalibration
        }

        private static string[] m_sMachineCode_Array = 
        { 
            "STOP",
            "HM",
            "MA",
            "MX",
            "MY",
            "MZ",
            "MAR",
            "MXR",
            "MYR",
            "MZR",
            "LA",
            "LAR",
            "PA",
            "PX",
            "PY",
            "PZ",
            "SP",
            "IV",
            "FORCE",
            "FGGET",
            "FGTARE" 
        };
        #endregion

        public RobotAPI(frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;

            m_cLT = new HW_LT_DT_500F();
            m_cFG = new HW_ForceGauge_SHIMPO_FGP05();
        }

        public void ClosePort()
        {
            try
            {
                m_cLT.Disconnect();
            }
            catch
            {
            }

            try
            {
                m_cFG.Disconnect();
            }
            catch
            {
            }
        }

        private void ConvertRobotCommandToString(RobotCommand eRobotCommand, 
                                                 ref string sRobotCommand, 
                                                 float fXCoord = 0.0f, 
                                                 float fYCoord = 0.0f, 
                                                 float fZCoord = 0.0f,
                                                 float fSpeed = 0.0f, 
                                                 float fWeight = 0.0f)
        {
            sRobotCommand = "";

            switch (eRobotCommand)
            {
                //No Parameter Command [Command]
                case RobotCommand.STOP:
                case RobotCommand.ReturnOriginal:
                case RobotCommand.GetPositionXYZ:
                case RobotCommand.GetPositionX:
                case RobotCommand.GetPositionY:
                case RobotCommand.GetPositionZ:
                case RobotCommand.IsMoving:
                case RobotCommand.GetWeight:
                case RobotCommand.SetWeightCalibration:
                    sRobotCommand = m_sMachineCode_Array[(int)eRobotCommand];
                    break;
                //Three Parameter Command [Command p1,p2,p3]
                case RobotCommand.MoveAbsoluteXYZ:
                case RobotCommand.MoveRelativeXYZ:
                case RobotCommand.LineAbsoluteXYZ:
                case RobotCommand.LineRelativeXYZ:
                    sRobotCommand = m_sMachineCode_Array[(int)eRobotCommand];
                    sRobotCommand += string.Format(" {0}", fXCoord.ToString());
                    sRobotCommand += string.Format(",{0}", fYCoord.ToString());
                    sRobotCommand += string.Format(",{0}", fZCoord.ToString());
                    break;
                //One Parameter Command [Command p1]
                case RobotCommand.MoveAbsoluteX:
                case RobotCommand.MoveRelativeX:
                    sRobotCommand = m_sMachineCode_Array[(int)eRobotCommand];
                    sRobotCommand += string.Format(" {0}", fXCoord.ToString());
                    break;
                case RobotCommand.MoveAbsoluteY:
                case RobotCommand.MoveRelativeY:
                    sRobotCommand = m_sMachineCode_Array[(int)eRobotCommand];
                    sRobotCommand += string.Format(" {0}", fYCoord.ToString());
                    break;
                case RobotCommand.MoveAbsoluteZ:
                case RobotCommand.MoveRelativeZ:
                    sRobotCommand = m_sMachineCode_Array[(int)eRobotCommand];
                    sRobotCommand += string.Format(" {0}", fZCoord.ToString());
                    break;
                case RobotCommand.SetSpeed:
                    sRobotCommand = m_sMachineCode_Array[(int)eRobotCommand];
                    sRobotCommand += string.Format(" {0}", fSpeed.ToString());
                    break;
                case RobotCommand.SetWeight:
                    sRobotCommand = m_sMachineCode_Array[(int)eRobotCommand];
                    sRobotCommand += fWeight.ToString();
                    break;
                default:
                    break;
            }
        }

        public void RunRobotCommand(string sInput, ref string sOutput, int nTimeout = m_nWAITSECONDTIMEOUT, bool bOutputMessage = true)
        {
            if (m_cLT.IsOpen == false) 
                return;

            if (bOutputMessage == true)
                OutputMessage(string.Format("-Robot Get Command: {0}", sInput));

            int nSetTimeout = nTimeout * 1000;
            int nFGTarget = -1;
            int nFGResult = -1;

            nFGTarget = GetForceTarget(sInput);

            if (nFGTarget >= 0)
            {
                nFGResult = GetFGTargetValue(nFGTarget);
                sOutput = nFGResult.ToString();
            }
            else
            {
                if (sInput == "FGGET")
                {
                    if (!m_cFG.IsOpen)
                    {
                        sOutput = "-9999";
                    }
                    else
                    {
                        sOutput = m_cFG.GetValue().ToString();
                    }
                }
                else if (sInput == "FGTARE")
                {
                    if (m_cFG.IsOpen)
                    {
                        m_cFG.Tare();
                        sOutput = "OK";
                    }
                }
                else
                {
                    if (sInput != null && sInput != "")
                    {
                        string sRealCommand = sInput.Substring(0, 2);

                        if (sRealCommand == "IV")
                        {
                            // when LT return Value
                            sOutput = m_cLT.Communication_EchoGetTwice(sInput, nSetTimeout);
                        }
                        else
                        {
                            sOutput = m_cLT.Communication_EchoGetOnce(sInput, nSetTimeout);
                        }
                    }
                }
            }

            if (bOutputMessage == true)
                OutputMessage(string.Format("-Robot Get Response: {0}", sOutput));
        }

        private int GetForceTarget(string sInput)
        {
            /*
            string sForceString = "FORCE";
            int nInputLength = (sInput.Length < sForceString.Length) ? sInput.Length : sForceString.Length;

            if (sInput.Substring(0, nInputLength) == sForceString)
            {
                int nIntStrLength = sInput.Length - sForceString.Length;
                return Int32.Parse(sInput.Substring(sForceString.Length, nIntStrLength));
            }
            */

            if (sInput == null || sInput == "")
                return -1;

            if (sInput.Substring(0, 2) == "FO")
                return Int32.Parse(sInput.Remove(0, 5));
            else
                return -1;
        }

        private int GetFGTargetValue(int nTargetValue)  //Return Real FG Value
        {
            int nFGValue = 0;
            int nPreviousFGValue = 0;

            nFGValue = m_cFG.GetValue();

            if (nFGValue < 0)
                return nFGValue;

            while (nFGValue < nTargetValue)
            {
                nPreviousFGValue = nFGValue;

                if (m_cFG.GetValue() == 0)
                    m_cLT.Move(0, 0, 0.05);
                else
                    m_cLT.Move(0, 0, 0.01);

                Thread.Sleep(100);
                nFGValue = m_cFG.GetValue();
            }

            //After While Loop, nFGValue > nTargetValue > nPreviousFGValue
            if ((nFGValue - nTargetValue) > (nTargetValue - nFGValue))
            {
                m_cLT.Move(0, 0, -0.01);
                nFGValue = m_cFG.GetValue();
            }

            return nFGValue;
        }

        public bool RunRobotTest()
        {
            bool bRobotConnect = false;

            m_tRobotConnect = new Thread(() =>
            {
                bRobotConnect = RunRS232DeviceConnectTest();
            });

            m_tRobotConnect.IsBackground = true;
            m_tRobotConnect.Start();
            m_tRobotConnect.Join();

            return bRobotConnect;
        }

        public bool RunRS232DeviceConnectTest()
        {
            if (m_cLT.IsOpen) 
            { 
                m_cLT.Disconnect(); 
            }

            if (m_cFG.IsOpen) 
            { 
                m_cFG.Disconnect(); 
            }

            string[] sSerialPortName_Array = SerialPort.GetPortNames();
            List<string> sSerialPortName_List = new List<string>(sSerialPortName_Array);

            if (sSerialPortName_Array.Length < 1)
            {
                OutputMessage("-Can not Find Robot");
                return false;
            }

            //Connect FG
            bool bFGResultFlag = false;
            string sFGUsingName = null;

            foreach (string sName in sSerialPortName_List)
            {
                sFGUsingName = sName;
                m_cFG.PortName = sFGUsingName;

                if (m_cfrmMain.m_sSelectFGCOMPort != "N/A" && m_cfrmMain.m_sSelectFGCOMPort != sFGUsingName.ToString())
                    continue;

                OutputMessage(string.Format("-Find COM Port: {0}", sFGUsingName.ToString()));

                if (!m_cFG.Connect())
                {
                    OutputMessage("-COM Port No Response");
                    continue;
                }

                if (m_cFG.TestDevice())
                {
                    m_cFG.Tare();
                    m_cFG.SwitchToKg();
                    bFGResultFlag = true;
                    OutputMessage(string.Format("-ForceGauge(Pressure Robot) at {0}", sFGUsingName.ToString()));
                    break;
                }
                else
                    OutputMessage("-COM Port No Response");

                m_cFG.Disconnect();
            }

            if (bFGResultFlag == true)
            {
                m_cfrmMain.m_sSelectFGCOMPort = sFGUsingName.ToString();
                sSerialPortName_List.Remove(sFGUsingName);
            }
            else
            {
                OutputMessage("-ForceGauge(Pressure Robot) Can Not Connect");

                //Can not find ForceGauge
                //return false;
            }

            //Connect LT
            bool bLTResultFlag = false;
            string sLTUsingName = null;

            foreach (string sName in sSerialPortName_List)
            {
                sLTUsingName = sName;
                m_cLT.PortName = sLTUsingName;

                if (m_cfrmMain.m_sSelectLTCOMPort != "N/A" && m_cfrmMain.m_sSelectLTCOMPort != sLTUsingName.ToString())
                    continue;

                OutputMessage(string.Format("-Find COM Port: {0}", sLTUsingName.ToString()));

                if (m_cLT.Connect() == false)
                {
                    OutputMessage("-COM Port No Response");
                    continue;
                }

                if (m_cLT.TestDevice() == true)
                {
                    bLTResultFlag = true;
                    OutputMessage(string.Format("-LT Robot at {0}", sLTUsingName.ToString()));

                    string sRealCommand = "";
                    string sEchoMessage = "";

                    ConvertRobotCommandToString(RobotCommand.SetSpeed, ref sRealCommand, 0.0f, 0.0f, 0.0f, 150.0f);
                    RunRobotCommand(sRealCommand, ref sEchoMessage, m_nWAITSECONDTIMEOUT, false);

                    ConvertRobotCommandToString(RobotCommand.MoveAbsoluteXYZ, ref sRealCommand, 0.0f, 0.0f, 0.0f);
                    RunRobotCommand(sRealCommand, ref sEchoMessage, m_nWAITSECONDTIMEOUT, false);

                    bool bRobotStopFlag = false;

                    while (!bRobotStopFlag)
                    {
                        Thread.Sleep(500);  //Wait 500s
                        ConvertRobotCommandToString(RobotCommand.IsMoving, ref sRealCommand);
                        RunRobotCommand(sRealCommand, ref sEchoMessage, 1, false);

                        char charResult = sEchoMessage[0];

                        if (charResult == '1')
                            bRobotStopFlag = true;
                    }

                    OutputMessage("-Robot Start: Back to the Origin Location");
                    break;
                }
                else
                    OutputMessage("-COM Port No Response");

                m_cLT.Disconnect();
            }

            if (bLTResultFlag == true)
            {
                m_cfrmMain.m_sSelectLTCOMPort = sLTUsingName.ToString();
                sSerialPortName_List.Remove(sLTUsingName);
            }
            else
            {
                //Can not Find LT
                return false;
            }

            return true;
        }

        public bool RunRobotKeepConnectTest()
        {
            bool bLTPassFlag = false;
            bool bFGPassFlag = false;

            bLTPassFlag = m_cLT.TestDevice();
            bFGPassFlag = m_cFG.TestDevice();

            if (!bLTPassFlag)
            {
                ClosePort();
                return false;
            }

            if (bFGPassFlag)
                m_cFG.Tare();

            OutputMessage("-Robot Start: Robot Keep Connect");
            return true;
        }

        //RS232 Device Reset
        public void ReturnToOrigin()  
        {
            #region Set LT Return To Origin
            string sRealCommand = "";
            string sEchoMessage = "";

            ConvertRobotCommandToString(RobotCommand.STOP, ref sRealCommand);
            RunRobotCommand(sRealCommand, ref sEchoMessage, m_nWAITSECONDTIMEOUT, false);

            m_cLT.ReturnOriginal();
            #endregion

            //FG
            if (m_cFG.IsOpen)
                m_cFG.Tare();
        }

        public bool SetSocketRobot(RobotCode eRobotCode, ref string sRealCommand, float fXCoord = 0.0f, float fYCoord = 0.0f, float fZCoord = 0.0f, float fSpeed = 0.0f, float fWeight = 0.0f)
        {
            sRealCommand = "";

            switch (eRobotCode)
            {
                case RobotCode.ReturnOriginal:
                    ConvertRobotCommandToString(RobotCommand.ReturnOriginal, ref sRealCommand, fXCoord, fYCoord, fZCoord);
                    break;
                case RobotCode.SetAbsoluteMove:
                    ConvertRobotCommandToString(RobotCommand.MoveAbsoluteXYZ, ref sRealCommand, fXCoord, fYCoord, fZCoord);
                    break;
                case RobotCode.SetSpeed:
                    ConvertRobotCommandToString(RobotCommand.SetSpeed, ref sRealCommand, 0, 0, 0, fSpeed);
                    break;
                /*
                case RobotCode.GetPosition:
                   ConvertRobotCommandToString(RobotCommand.GetPositionXYZ, ref sRealCommand);
                   break;
                */
                case RobotCode.IsMoving:
                    ConvertRobotCommandToString(RobotCommand.IsMoving, ref sRealCommand);
                    break;
                case RobotCode.SetLineAbsoluteMove:
                    ConvertRobotCommandToString(RobotCommand.LineAbsoluteXYZ, ref sRealCommand, fXCoord, fYCoord, fZCoord);
                    break;
                case RobotCode.Stop:
                    ConvertRobotCommandToString(RobotCommand.STOP, ref sRealCommand);
                    break;
                case RobotCode.SetWeight:
                    ConvertRobotCommandToString(RobotCommand.SetWeight, ref sRealCommand, 0, 0, 0, 0, fWeight);
                    break;
                case RobotCode.GetWeight:
                    ConvertRobotCommandToString(RobotCommand.GetWeight, ref sRealCommand);
                    break;
                case RobotCode.SetWeightCalibration:
                    ConvertRobotCommandToString(RobotCommand.SetWeightCalibration, ref sRealCommand);
                    break;
                case RobotCode.SetAbsoluteZ:
                    ConvertRobotCommandToString(RobotCommand.MoveAbsoluteZ, ref sRealCommand, fXCoord, fYCoord, fZCoord);
                    break;
                default:
                    break;
            }
            return true;
        }

        public void ForceStop()
        {
            if (m_tRobotConnect != null && m_tRobotConnect.IsAlive == true)
            {
                m_tRobotConnect.Abort();
                m_tRobotConnect.Join();
            }
        }

        private void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmMain.OutputMessage(sMessage, bWarning);
        }
    }
}