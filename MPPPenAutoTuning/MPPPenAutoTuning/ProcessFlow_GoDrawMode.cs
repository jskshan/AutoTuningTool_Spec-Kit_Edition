using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Elan;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class ProcessFlow
    {
        /// <summary>
        /// 寫字機(GoDraw)模式控制主流程
        /// </summary>
        /// <param name="objRobotInfo">寫字機(GoDraw)相關資訊之物件</param>
        private void RunGoDrawRobotThread(object objRobotInfo)
        {
            RobotParameter cRobotParameter = (RobotParameter)objRobotInfo;

            InitialRobotThreadParameter();

            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;
            FlowRobot eRobotType = cRobotParameter.m_eRobotType;
            int nFlowIndex = cRobotParameter.m_nFlowIndex;

            OutputMessage(string.Format("-GoDrawRobotThread : Robot Stage={0}", eRobotType.ToString()));

            int nXStart = 0;
            int nYStart = 0;
            int nXEnd = 0;
            int nYEnd = 0;
            double dSpeed = 0.0;
            int nZServoValue = 0;

            ComputeXYCoordByGoDraw(ref nXStart, ref nYStart, ref nXEnd, ref nYEnd, cRobotParameter, eRobotType);

            ComputeSpeedAndZServoValueByGoDraw(ref dSpeed, ref nZServoValue, cRobotParameter);

            string sMessage = string.Format("-GoDrawRobotThread : Set Speed={0}mm/s", dSpeed);
            OutputMessage(sMessage);

            if (m_cGoDrawParameter.m_bGoDrawResetZAxis == false)
            {
                RunGoDrawAction(GoDrawAPI.GoDrawCommand.ZUpDown);
                RunGoDrawAction(GoDrawAPI.GoDrawCommand.Top);
                m_cGoDrawParameter.m_bGoDrawResetZAxis = true;
            }

            //GoDraw is Moving to Start Location
            OutputMessage("-GoDrawRobotThread : GoDraw is Moving to Start Location");

            switch (eRobotType)
            {
                case FlowRobot.NO:
                    if (m_cGoDrawParameter.m_bGoDrawReturnHome == false)
                    {
                        RunGoDrawAction(GoDrawAPI.GoDrawCommand.Home, 0, 0, m_cGoDrawParameter.m_dNormalSpeed);
                        m_cGoDrawParameter.m_bGoDrawReturnHome = true;
                    }

                    break;
                case FlowRobot.HOVERLINE:
                case FlowRobot.TOUCHLINE:
                case FlowRobot.TOUCHLINE_HOR:
                case FlowRobot.TOUCHLINE_VER:
                case FlowRobot.HOVERPOINT_CEN:
                    if (m_cGoDrawParameter.m_bGoDrawReturnHome == false)
                    {
                        RunGoDrawAction(GoDrawAPI.GoDrawCommand.Home, 0, 0, m_cGoDrawParameter.m_dNormalSpeed);
                        m_cGoDrawParameter.m_bGoDrawReturnHome = true;
                    }

                    RunGoDrawAction(GoDrawAPI.GoDrawCommand.Move, nXStart, nYStart, m_cGoDrawParameter.m_dNormalSpeed);
                    break;
                case FlowRobot.TOUCHPOINT_CEN:
                    if (eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE) &&
                        m_bDisableRobotMovingFlag == true)
                        break;

                    if (m_cGoDrawParameter.m_bGoDrawReturnHome == false)
                    {
                        RunGoDrawAction(GoDrawAPI.GoDrawCommand.Home, 0, 0, m_cGoDrawParameter.m_dNormalSpeed);
                        m_cGoDrawParameter.m_bGoDrawReturnHome = true;
                    }

                    RunGoDrawAction(GoDrawAPI.GoDrawCommand.Move, nXStart, nYStart, m_cGoDrawParameter.m_dNormalSpeed);
                    break;
                default:
                    break;
            }

            if ((eMainStep == MainTuningStep.TPGAINTUNING && eSubStep == SubTuningStep.TP_GAIN) && m_bRetryStateFlag == false)
            {
                if (ParamAutoTuning.m_nTPGTDisplayMessage != 0)
                    ShowWarningMessageByTPGainCoordinateSetting(cRobotParameter.m_cFlowStep, eRobotType);
            }

            m_nRobotPrepareFlag |= MainConstantParameter.m_nFLOWSTATE_FIRSTSTEP;

            //Wait Record Prepare Flag (Record: Command to IC)
            OutputMessage("-GoDrawRobotThread : Wait Record Prepare");

            while (m_nRecordPrepareFlag < MainConstantParameter.m_nFLOWSTATE_FIRSTSTEP)
                Thread.Sleep(500);

            switch (eRobotType)
            {
                case FlowRobot.NO:
                    break;
                case FlowRobot.HOVERLINE:
                case FlowRobot.HOVERPOINT_CEN:
                    RunGoDrawAction(GoDrawAPI.GoDrawCommand.ZMove, 0, 0, 0, 0, 0, m_cGoDrawParameter.m_nContactServoValueZ);
                    Thread.Sleep(1000);
                    RunGoDrawAction(GoDrawAPI.GoDrawCommand.ZMove, 0, 0, 0, 0, 0, nZServoValue);
                    break;
                case FlowRobot.TOUCHLINE:
                case FlowRobot.TOUCHLINE_HOR:
                case FlowRobot.TOUCHLINE_VER:
                    RunGoDrawAction(GoDrawAPI.GoDrawCommand.ZMove, 0, 0, 0, 0, 0, nZServoValue);
                    break;
                case FlowRobot.TOUCHPOINT_CEN:
                    if (!(eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE) &&
                          m_bDisableRobotMovingFlag == true))
                    {
                        RunGoDrawAction(GoDrawAPI.GoDrawCommand.ZMove, 0, 0, 0, 0, 0, nZServoValue);
                    }

                    break;
                default:
                    break;
            }
            m_nRobotPrepareFlag |= MainConstantParameter.m_nFLOWSTATE_SECONDSTEP;

            while (m_nRecordPrepareFlag < MainConstantParameter.m_nFLOWSTATE_SECONDSTEP)
            {
                SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);
                Thread.Sleep(500);
            };

            while (m_bRecordStartFlag == false)
                Thread.Sleep(100);

            if (m_bForceStopFlag == true)
                m_bRobotFinishedFlag = true;
            else
            {
                OutputMessage("-GoDrawRobotThread : Record Prepare");

                //Robot Start Moving
                switch (eRobotType)
                {
                    case FlowRobot.NO:
                    case FlowRobot.HOVERPOINT_CEN:
                    case FlowRobot.TOUCHPOINT_CEN:
                        WaitGetDataComplete(cRobotParameter, true);
                        break;
                    case FlowRobot.HOVERLINE:
                    case FlowRobot.TOUCHLINE:
                    case FlowRobot.TOUCHLINE_HOR:
                    case FlowRobot.TOUCHLINE_VER:
                        RunGoDrawActionAndWait(cRobotParameter, GoDrawAPI.GoDrawCommand.Move, nXEnd, nYEnd, dSpeed);

                        if (m_cfrmMain.m_bForceStopFlowEnableFlag == true)
                            return;

                        Thread.Sleep(ParamAutoTuning.m_nGoDrawDelayTime);
                        m_cGoDrawRobot.m_bForceStop = true;
                        RunGoDrawAction(GoDrawAPI.GoDrawCommand.Stop);
                        m_cGoDrawRobot.m_bStop = false;
                        break;
                    default:
                        break;
                }

                m_bRobotFinishedFlag = true;

                OutputMessage("-GoDrawRobotThread : Record Finished. Back to Origin Location");

                if (!(eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE) &&
                      m_bDisableRobotMovingFlag == true))
                {
                    RunGoDrawAction(GoDrawAPI.GoDrawCommand.Top);
                }

                CheckReportDataIsValid(cRobotParameter, nFlowIndex, false);
            }
        }

        #region Compute Coordination and Speed Function
        /// <summary>
        /// 計算X, Y座標的起始點以及終點
        /// </summary>
        /// <param name="nXStart">X座標的起始點數值(mm)</param>
        /// <param name="nYStart">Y座標的起始點數值(mm)</param>
        /// <param name="nXEnd">X座標的終點數值(mm)</param>
        /// <param name="nYEnd">Y座標的終點數值(mm)</param>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        /// <param name="eRobotType">線測機流程種類</param>
        private void ComputeXYCoordByGoDraw(ref int nXStart, ref int nYStart, ref int nXEnd, ref int nYEnd, RobotParameter cRobotParameter, FlowRobot eRobotType)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            if (eRobotType == FlowRobot.TOUCHLINE_HOR)
            {
                if (eMainStep == MainTuningStep.LINEARITYTUNING && eSubStep == SubTuningStep.LINEARITYTABLE)
                {
                    nXStart = m_cGoDrawParameter.m_nStartCoordinateX;
                    nYStart = m_cGoDrawParameter.m_nStartCoordinateY + m_cGoDrawParameter.m_nHorizontalShiftCoordinateY_LT;
                    nXEnd = m_cGoDrawParameter.m_nEndCoordinateX;
                    nYEnd = m_cGoDrawParameter.m_nStartCoordinateY + m_cGoDrawParameter.m_nHorizontalShiftCoordinateY_LT;
                }
                else if (eMainStep == MainTuningStep.DIGIGAINTUNING)
                {
                    int nCenterYCoord = (int)Math.Round((m_cGoDrawParameter.m_nEndCoordinateY + m_cGoDrawParameter.m_nStartCoordinateY) / 2.0, 0, MidpointRounding.AwayFromZero);

                    nXStart = m_cGoDrawParameter.m_nStartCoordinateX;
                    nYStart = nCenterYCoord;
                    nXEnd = m_cGoDrawParameter.m_nEndCoordinateX;
                    nYEnd = nCenterYCoord;
                }
                else if (eMainStep == MainTuningStep.TPGAINTUNING)
                {
                    int nCenterYCoord = (int)Math.Round((m_cGoDrawParameter.m_nHorizontalEndCoordinateY_TPGT + m_cGoDrawParameter.m_nHorizontalStartCoordinateY_TPGT) / 2.0, 0, MidpointRounding.AwayFromZero);

                    nXStart = m_cGoDrawParameter.m_nHorizontalStartCoordinateX_TPGT;
                    nYStart = nCenterYCoord;
                    nXEnd = m_cGoDrawParameter.m_nHorizontalEndCoordinateX_TPGT;
                    nYEnd = nCenterYCoord;
                }
                else
                {
                    int nCenterXCoord = (int)Math.Round((m_cGoDrawParameter.m_nEndCoordinateX + m_cGoDrawParameter.m_nStartCoordinateX) / 2.0, 0, MidpointRounding.AwayFromZero);
                    int nCenterYCoord = (int)Math.Round((m_cGoDrawParameter.m_nEndCoordinateY + m_cGoDrawParameter.m_nStartCoordinateY) / 2.0, 0, MidpointRounding.AwayFromZero);

                    nXStart = nCenterXCoord - 15;
                    nYStart = nCenterYCoord;
                    nXEnd = nCenterXCoord + 15;
                    nYEnd = nCenterYCoord;
                }
            }
            else if (eRobotType == FlowRobot.TOUCHLINE_VER)
            {
                if (eMainStep == MainTuningStep.LINEARITYTUNING && eSubStep == SubTuningStep.LINEARITYTABLE)
                {
                    nXStart = m_cGoDrawParameter.m_nStartCoordinateX + m_cGoDrawParameter.m_nVerticalShiftCoordinateX_LT;
                    nYStart = m_cGoDrawParameter.m_nStartCoordinateY;
                    nXEnd = m_cGoDrawParameter.m_nStartCoordinateX + m_cGoDrawParameter.m_nVerticalShiftCoordinateX_LT;
                    nYEnd = m_cGoDrawParameter.m_nEndCoordinateY;
                }
                else if (eMainStep == MainTuningStep.DIGIGAINTUNING)
                {
                    int nCenterXCoord = (int)Math.Round((m_cGoDrawParameter.m_nEndCoordinateX + m_cGoDrawParameter.m_nStartCoordinateX) / 2.0, 0, MidpointRounding.AwayFromZero);

                    nXStart = nCenterXCoord;
                    nYStart = m_cGoDrawParameter.m_nStartCoordinateY;
                    nXEnd = nCenterXCoord;
                    nYEnd = m_cGoDrawParameter.m_nEndCoordinateY;
                }
                else if (eMainStep == MainTuningStep.TPGAINTUNING)
                {
                    int nCenterXCoord = (int)Math.Round((m_cGoDrawParameter.m_nVerticalEndCoordinateX_TPGT + m_cGoDrawParameter.m_nVerticalStartCoordinateX_TPGT) / 2.0, 0, MidpointRounding.AwayFromZero);

                    nXStart = nCenterXCoord;
                    nYStart = m_cGoDrawParameter.m_nVerticalStartCoordinateY_TPGT;
                    nXEnd = nCenterXCoord;
                    nYEnd = m_cGoDrawParameter.m_nVerticalEndCoordinateY_TPGT;
                }
                else
                {
                    int nCenterXCoord = (int)Math.Round((m_cGoDrawParameter.m_nEndCoordinateX + m_cGoDrawParameter.m_nStartCoordinateX) / 2.0, 0, MidpointRounding.AwayFromZero);
                    int nCenterYCoord = (int)Math.Round((m_cGoDrawParameter.m_nEndCoordinateY + m_cGoDrawParameter.m_nStartCoordinateY) / 2.0, 0, MidpointRounding.AwayFromZero);

                    nXStart = nCenterXCoord;
                    nYStart = nCenterYCoord - 15;
                    nXEnd = nCenterXCoord;
                    nYEnd = nCenterYCoord + 15;
                }
            }
            else if (eRobotType == FlowRobot.HOVERPOINT_CEN || eRobotType == FlowRobot.TOUCHPOINT_CEN)
            {
                nXStart = (int)Math.Round((m_cGoDrawParameter.m_nEndCoordinateX + m_cGoDrawParameter.m_nStartCoordinateX) / 2.0, 0, MidpointRounding.AwayFromZero);
                nYStart = (int)Math.Round((m_cGoDrawParameter.m_nEndCoordinateY + m_cGoDrawParameter.m_nStartCoordinateY) / 2.0, 0, MidpointRounding.AwayFromZero);
                nXEnd = nXStart;
                nYEnd = nYStart;
            }
            else
            {
                nXStart = m_cGoDrawParameter.m_nStartCoordinateX;
                nYStart = m_cGoDrawParameter.m_nStartCoordinateY;
                nXEnd = m_cGoDrawParameter.m_nEndCoordinateX;
                nYEnd = m_cGoDrawParameter.m_nEndCoordinateY;
            }
        }

        /// <summary>
        /// 計算畫線速度以及Z座標
        /// </summary>
        /// <param name="dSpeed">畫線速度(mm/s)</param>
        /// <param name="nZServoValue">Z伺服馬達數值</param>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        private void ComputeSpeedAndZServoValueByGoDraw(ref double dSpeed, ref int nZServoValue, RobotParameter cRobotParameter)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            if (eMainStep == MainTuningStep.DIGIGAINTUNING)
                dSpeed = Math.Round(m_cSpeedParameter.m_fSpeed_DGT, 1, MidpointRounding.AwayFromZero);
            else if (eMainStep == MainTuningStep.TPGAINTUNING)
                dSpeed = Math.Round(m_cSpeedParameter.m_fSpeed_TPGT, 1, MidpointRounding.AwayFromZero);
            else if (eMainStep == MainTuningStep.PEAKCHECKTUNING)
                dSpeed = Math.Round(m_cSpeedParameter.m_fSpeed_PCT, 1, MidpointRounding.AwayFromZero);
            else if (eMainStep == MainTuningStep.DIGITALTUNING)
                dSpeed = Math.Round(m_cSpeedParameter.m_fSpeed_DT, 1, MidpointRounding.AwayFromZero);
            else if (eMainStep == MainTuningStep.TILTTUNING)
            {
                if(cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE)
                    dSpeed = Math.Round(m_cSpeedParameter.m_fSpeed_TTSlant, 1, MidpointRounding.AwayFromZero);
                else
                    dSpeed = Math.Round(m_cSpeedParameter.m_fSpeed_TT, 1, MidpointRounding.AwayFromZero);
            }
            else if (eMainStep == MainTuningStep.LINEARITYTUNING)
                dSpeed = Math.Round(m_cSpeedParameter.m_fSpeed_LT, 1, MidpointRounding.AwayFromZero);
            else
                dSpeed = Math.Round(m_cSpeedParameter.m_fSpeed_DT, 1, MidpointRounding.AwayFromZero);

            switch (cRobotParameter.m_eRobotType)
            {
                case FlowRobot.NO:
                    dSpeed = 0.0;
                    nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ;
                    break;
                case FlowRobot.HOVERLINE:
                    switch (eSubStep)
                    {
                        case SubTuningStep.HOVER_2ND:
                            nZServoValue = m_cGoDrawParameter.m_nHoverServoValueZ_DT2nd;
                            break;
                        case SubTuningStep.PCHOVER_1ST:
                            nZServoValue = m_cGoDrawParameter.m_nHoverServoValueZ_PCT1st;
                            break;
                        case SubTuningStep.PCHOVER_2ND:
                            nZServoValue = m_cGoDrawParameter.m_nHoverServoValueZ_PCT2nd;
                            break;
                        default:
                            nZServoValue = m_cGoDrawParameter.m_nHoverServoValueZ_DT1st;
                            break;
                    }

                    break;
                case FlowRobot.TOUCHLINE:
                    if (eMainStep == MainTuningStep.DIGIGAINTUNING)
                    {
                        if (m_bDetectFGConnectFlag == false || m_bDetectFGPowerOnFlag == false)
                            nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ + m_cGoDrawParameter.m_nPushDownServoValueZ_DGT;
                        else
                            nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ;
                    }
                    else
                        nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ;

                    break;
                case FlowRobot.TOUCHLINE_HOR:
                case FlowRobot.TOUCHLINE_VER:
                    if (eMainStep == MainTuningStep.DIGIGAINTUNING)
                    {
                        if (m_bDetectFGConnectFlag == false || m_bDetectFGPowerOnFlag == false)
                            nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ + m_cGoDrawParameter.m_nPushDownServoValueZ_DGT;
                        else
                            nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ;
                    }
                    else if (eMainStep == MainTuningStep.TPGAINTUNING)
                        nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ_TPGT;
                    else
                        nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ;

                    break;
                case FlowRobot.HOVERPOINT_CEN:
                    dSpeed = 0.0;
                    nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ;
                    break;
                case FlowRobot.TOUCHPOINT_CEN:
                    dSpeed = 0.0;

                    if (eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE))
                        nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ;
                    else
                        nZServoValue = m_cGoDrawParameter.m_nContactServoValueZ;

                    break;
                default:
                    dSpeed = 0.0;
                    nZServoValue = m_cGoDrawParameter.m_nTopServoValueZ;
                    break;
            }
        }
        #endregion

        #region GoDraw Control Function
        private void RunGoDrawAction(GoDrawAPI.GoDrawCommand eCommandType,
                                     int nCoordinateX = 0,
                                     int nCoordinateY = 0,
                                     double dSpeed = 0.0,
                                     int nSleepTime = 0,
                                     int nMoveDistance = 0,
                                     int nZServoValue = 0)
        {
            if (m_cfrmMain.m_bForceStopFlowEnableFlag == true)
                return;

            bool bSuccessFlag = m_cGoDrawRobot.RunGoDrawAction(eCommandType, nCoordinateX, nCoordinateY, dSpeed, nSleepTime, nMoveDistance, nZServoValue);

            if (bSuccessFlag == false)
            {
                m_sErrorMessage = "Run GoDraw Robot Error";
                m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(20);
            }
        }

        private bool RunGoDrawActionAndWait(RobotParameter cRobotParameter,
                                            GoDrawAPI.GoDrawCommand eCommandType,
                                            int nCoordinateX = 0,
                                            int nCoordinateY = 0,
                                            double dSpeed = 0.0,
                                            int nSleepTime = 0,
                                            int nMoveDistance = 0,
                                            int nZServoValue = 0)
        {
            if (m_cfrmMain.m_bForceStopFlowEnableFlag == true)
                return true;

            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            bool bGoDrawStopFlag = false;
            long nStart = DateTime.Now.Ticks;
            long nCurrent = 0;
            int nCostTime = 0;
            bool bSuccessFlag = false;

            Thread tGoDrawMovingDetect = new Thread(() =>
            {
                bSuccessFlag = m_cGoDrawRobot.RunGoDrawAction(eCommandType, nCoordinateX, nCoordinateY, dSpeed, nSleepTime, nMoveDistance, nZServoValue);

                if (bSuccessFlag == false)
                {
                    m_sErrorMessage = "Run GoDraw Robot Error";
                    m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                    m_cfrmMain.m_bInterruptFlag = true;
                    Thread.Sleep(20);
                    return;
                }

                Thread.Sleep(m_cGoDrawParameter.m_nDelayTime);
                bGoDrawStopFlag = true;
            });

            tGoDrawMovingDetect.IsBackground = true;
            tGoDrawMovingDetect.Start();

            SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);

            while (bGoDrawStopFlag == false)
            {
                Thread.Sleep(500);  //Wait 500ms

                nCurrent = DateTime.Now.Ticks;
                nCostTime = (int)((nCurrent - nStart) / 10000);
                int nReportNumber = m_byteReportData_List.Count;
                m_dTimeCounter = Math.Round((double)nCostTime / 1000, 3, MidpointRounding.AwayFromZero);

                if (m_cfrmMain.m_bInterruptFlag == false)
                {
                    OutputMessage(string.Format("-Time : {0:0.000} s, Report : {1}", m_dTimeCounter, nReportNumber.ToString()));

                    SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);
                }
            }

            if (m_byteReportData_List.Count <= 0)
            {
                m_sErrorMessage = "No Report Data";

                if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                    eMainStep == MainTuningStep.TPGAINTUNING ||
                    (eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)) ||
                    eMainStep == MainTuningStep.TILTTUNING ||
                    eMainStep == MainTuningStep.PRESSURETUNING ||
                    eMainStep == MainTuningStep.LINEARITYTUNING)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                    {
                        string sErrorCode = string.Format("{0}.001", StringConvert.m_dictSubStepCNMappingTable[eSubStep]);
                        SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorCode, sErrorCode, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                        SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorMsg, m_sErrorMessage, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                        SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorMessage, m_sErrorMessage, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                    }

                    m_nRecordDataErrorFlag = 0xF000;
                }
                else
                    m_nRecordDataErrorFlag = 0;
            }

            return true;
        }

        private bool ConnectGoDrawRobot(int nStepIndex)
        {
            if (nStepIndex == 0 || m_bGoDrawConnectFlag == false)
            {
                OutputMessage("-Connect GoDraw Robot");

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                {
                    m_cGoDrawRobot = new GoDrawAPI(GoDrawAPI.ControlUIType.MSPen_AutoTuning, null, m_cfrmMain);
                    m_cGoDrawRobot.SetParameter();

                    if (m_cGoDrawRobot.RunConnectSerialPort() == false)
                    {
                        m_cGoDrawRobot.CloseUdpClient();

                        m_sErrorMessage = "Connect GoDraw Error";
                        m_cFinishFlowParameter.m_bErrorFlag = true;
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                        m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                        m_cfrmMain.m_bErrorFlag = true;
                        Thread.Sleep(10);
                        return false;
                    }
                    else
                        m_bGoDrawConnectFlag = true;
                }
            }

            return true;
        }

        /// <summary>
        /// 控制寫字機回到原點
        /// </summary>
        private void ReturnToOriginByGoDraw()
        {
            if (m_cGoDrawRobot != null)
            {
                m_cGoDrawRobot.m_bForceStop = true;
                m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Stop);
                m_cGoDrawRobot.m_bStop = false;
                m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Top);
                m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Home, 0, 0, m_cGoDrawParameter.m_dNormalSpeed);
            }
        }

        /// <summary>
        /// 控制寫字機回到原點並關閉寫字機連線
        /// </summary>
        private void SetGoDrawReturnToOriginAndClose()
        {
            lock (m_bGoDrawCloseLockFlag)
            {
                if (m_bGoDrawReturnFlag == false)
                {
                    m_bGoDrawReturnFlag = true;
                    ReturnToOriginByGoDraw();

                    if (m_cGoDrawRobot != null)
                    {
                        m_cGoDrawRobot.CloseUdpClient();
                        m_cGoDrawRobot.CloseCOMPort();
                        m_bGoDrawConnectFlag = false;
                    }

                    m_bGoDrawReturnFlag = false;
                }
            }
        }
        #endregion
    }
}
