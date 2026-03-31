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
        /// 線測機模式控制主流程
        /// </summary>
        /// <param name="objRobotInfo">線測機相關資訊之物件</param>
        private void RunSocketRobotThread(object objRobotInfo)
        {
            RobotParameter cRobotParameter = (RobotParameter)objRobotInfo;

            InitialRobotThreadParameter();

            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;
            FlowRobot eRobotType = cRobotParameter.m_eRobotType;
            int nFlowIndex = cRobotParameter.m_nFlowIndex;

            OutputMessage(string.Format("-ClientRobotThread : Robot Stage={0}", eRobotType.ToString()));

            string sRobotCommand = "";
            float fXStart = 0.0f;
            float fYStart = 0.0f;
            float fXEnd = 0.0f;
            float fYEnd = 0.0f;
            float fSpeed = 0.0f;
            float fZCoord = 0.0f;

            ComputeXYCoordByLTRobot(ref fXStart, ref fYStart, ref fXEnd, ref fYEnd, cRobotParameter, eRobotType);

            ComputeSpeedAndZCoordByLTRobot(ref fSpeed, ref fZCoord, cRobotParameter);

            ComputeOffsetWeight(cRobotParameter);

            string sMessage = string.Format("-ClientRobotThread : Set Speed={0}mm/s", fSpeed);
            OutputMessage(sMessage);

            //Robot is Moving to Start Location
            OutputMessage("-ClientRobotThread : Robot is Moving to Start Location");

            switch (eRobotType)
            {
                case FlowRobot.NO:
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 150);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, 0, 0, 0);
                    m_cSocket.RunClientSending(ref sRobotCommand);

                    if (WaitRobotStop() == false)
                        return;

                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.ReturnOriginal, ref sRobotCommand);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    break;
                case FlowRobot.HOVERLINE:
                case FlowRobot.TOUCHLINE:
                case FlowRobot.TOUCHLINE_HOR:
                case FlowRobot.TOUCHLINE_VER:
                case FlowRobot.HOVERPOINT_CEN:
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 150);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, fXStart, fYStart);
                    m_cSocket.RunClientSending(ref sRobotCommand);

                    if (WaitRobotStop(cRobotParameter) == false)
                        return;

                    break;
                case FlowRobot.TOUCHPOINT_CEN:
                    if (eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE) &&
                        m_bDisableRobotMovingFlag == true)
                        break;

                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 150);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, fXStart, fYStart);
                    m_cSocket.RunClientSending(ref sRobotCommand);

                    if (WaitRobotStop(cRobotParameter) == false)
                        return;

                    SetFGCalibration(cRobotParameter);
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
            OutputMessage("-ClientRobotThread : Wait Record Prepare");

            while (m_nRecordPrepareFlag < MainConstantParameter.m_nFLOWSTATE_FIRSTSTEP)
                Thread.Sleep(500);

            switch (eRobotType)
            {
                case FlowRobot.NO:
                    break;
                case FlowRobot.HOVERLINE:
                case FlowRobot.HOVERPOINT_CEN:
                    float fDropSpeed = 100;

                    if (m_bDetectFGConnectFlag == true && m_bDetectFGPowerOnFlag == true)
                        fDropSpeed = 10;

                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, fDropSpeed);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, fXStart, fYStart, m_cLTRobotParameter.m_fContactCoordinateZ);
                    m_cSocket.RunClientSending(ref sRobotCommand);

                    if (WaitRobotStop(cRobotParameter, false, true, true) == false)
                        return;

                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 100);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, fXStart, fYStart, fZCoord);
                    m_cSocket.RunClientSending(ref sRobotCommand);

                    if (WaitRobotStop(cRobotParameter, false, true, true) == false)
                        return;

                    break;
                case FlowRobot.TOUCHLINE:
                case FlowRobot.TOUCHLINE_HOR:
                case FlowRobot.TOUCHLINE_VER:
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 100);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, fXStart, fYStart, fZCoord);
                    m_cSocket.RunClientSending(ref sRobotCommand);

                    if (WaitRobotStop(cRobotParameter, false, true, true) == false)
                        return;

                    break;
                case FlowRobot.TOUCHPOINT_CEN:
                    if (!(eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE) &&
                          m_bDisableRobotMovingFlag == true))
                    {
                        m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 100);
                        m_cSocket.RunClientSending(ref sRobotCommand);
                        m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, fXStart, fYStart, fZCoord);
                        m_cSocket.RunClientSending(ref sRobotCommand);

                        if (WaitRobotStop(cRobotParameter, false, true, true) == false)
                            return;
                    }

                    if (SetFGWeightValue(cRobotParameter, fXStart, fYStart, fZCoord) == false)
                        return;

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
                OutputMessage("-ClientRobotThread : Record Prepare");

                //Robot Start Moving
                switch (eRobotType)
                {
                    case FlowRobot.NO:
                    case FlowRobot.HOVERPOINT_CEN:
                    case FlowRobot.TOUCHPOINT_CEN:
                        WaitGetDataComplete(cRobotParameter, true);

                        if (GetFGWeightValue(cRobotParameter) == false)
                            return;
                        break;
                    case FlowRobot.HOVERLINE:
                    case FlowRobot.TOUCHLINE:
                    case FlowRobot.TOUCHLINE_HOR:
                    case FlowRobot.TOUCHLINE_VER:
                        m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, fSpeed);
                        m_cSocket.RunClientSending(ref sRobotCommand);
                        m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetLineAbsoluteMove, ref sRobotCommand, fXEnd, fYEnd, fZCoord);
                        m_cSocket.RunClientSending(ref sRobotCommand);
                        WaitRobotStop(cRobotParameter, true);
                        break;
                    default:
                        break;
                }

                m_bRobotFinishedFlag = true;

                OutputMessage("-ClientRobotThread : Record Finished. Back to Origin Location");

                if (!(eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE) &&
                      m_bDisableRobotMovingFlag == true))
                {
                    //m_cRobot.SetSocketRobot(RobotAPI.RobotCode.ReturnOriginal, ref sRobotCommand);
                    //m_cSocket.RunClientSending(ref sRobotCommand);

                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 150);
                    m_cSocket.RunClientSending(ref sRobotCommand);

                    if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO)
                        m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, 0, 0);
                    else
                        m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, fXEnd, fYEnd);

                    m_cSocket.RunClientSending(ref sRobotCommand);

                    if (WaitRobotStop(cRobotParameter) == false)
                        return;
                }

                CheckReportDataIsValid(cRobotParameter, nFlowIndex, false);

                DetectClientSocket();
            }
        }

        #region Compute Coordination, Speed and Weight Function
        /// <summary>
        /// 計算X, Y座標的起始點以及終點
        /// </summary>
        /// <param name="fXStart">X座標的起始點數值(mm)</param>
        /// <param name="fYStart">Y座標的起始點數值(mm)</param>
        /// <param name="fXEnd">X座標的終點數值(mm)</param>
        /// <param name="fYEnd">Y座標的終點數值(mm)</param>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        /// <param name="eRobotType">線測機流程種類</param>
        private void ComputeXYCoordByLTRobot(ref float fXStart, ref float fYStart, ref float fXEnd, ref float fYEnd, RobotParameter cRobotParameter, FlowRobot eRobotType)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            if (eRobotType == FlowRobot.TOUCHLINE_HOR)
            {
                if (eMainStep == MainTuningStep.LINEARITYTUNING && eSubStep == SubTuningStep.LINEARITYTABLE)
                {
                    fXStart = m_cLTRobotParameter.m_fStartCoordinateX;
                    fYStart = m_cLTRobotParameter.m_fStartCoordinateY + m_cLTRobotParameter.m_fHorShiftCoordinateY_LT;
                    fXEnd = m_cLTRobotParameter.m_fEndCoordinateX;
                    fYEnd = m_cLTRobotParameter.m_fStartCoordinateY + m_cLTRobotParameter.m_fHorShiftCoordinateY_LT;
                }
                else if (eMainStep == MainTuningStep.DIGIGAINTUNING)
                {
                    float fCenterYCoord = (float)Math.Round((m_cLTRobotParameter.m_fEndCoordinateY + m_cLTRobotParameter.m_fStartCoordinateY) / 2, 2, MidpointRounding.AwayFromZero);

                    fXStart = m_cLTRobotParameter.m_fStartCoordinateX;
                    fYStart = fCenterYCoord;
                    fXEnd = m_cLTRobotParameter.m_fEndCoordinateX;
                    fYEnd = fCenterYCoord;
                }
                else if (eMainStep == MainTuningStep.TPGAINTUNING)
                {
                    float fCenterYCoord = (float)Math.Round((m_cLTRobotParameter.m_fHorEndCoordinateY_TPGT + m_cLTRobotParameter.m_fHorStartCoordinateY_TPGT) / 2, 2, MidpointRounding.AwayFromZero);

                    fXStart = m_cLTRobotParameter.m_fHorStartCoordinateX_TPGT;
                    fYStart = fCenterYCoord;
                    fXEnd = m_cLTRobotParameter.m_fHorEndCoordinateX_TPGT;
                    fYEnd = fCenterYCoord;
                }
                else
                {
                    float fCenterXCoord = (float)Math.Round((m_cLTRobotParameter.m_fEndCoordinateX + m_cLTRobotParameter.m_fStartCoordinateX) / 2, 2, MidpointRounding.AwayFromZero);
                    float fCenterYCoord = (float)Math.Round((m_cLTRobotParameter.m_fEndCoordinateY + m_cLTRobotParameter.m_fStartCoordinateY) / 2, 2, MidpointRounding.AwayFromZero);

                    fXStart = (float)Math.Round(fCenterXCoord - 15, 2, MidpointRounding.AwayFromZero);
                    fYStart = fCenterYCoord;
                    fXEnd = (float)Math.Round(fCenterXCoord + 15, 2, MidpointRounding.AwayFromZero);
                    fYEnd = fCenterYCoord;
                }
            }
            else if (eRobotType == FlowRobot.TOUCHLINE_VER)
            {
                if (eMainStep == MainTuningStep.LINEARITYTUNING && eSubStep == SubTuningStep.LINEARITYTABLE)
                {
                    fXStart = m_cLTRobotParameter.m_fStartCoordinateX + m_cLTRobotParameter.m_fVerShiftCoordinateX_LT;
                    fYStart = m_cLTRobotParameter.m_fStartCoordinateY;
                    fXEnd = m_cLTRobotParameter.m_fStartCoordinateX + m_cLTRobotParameter.m_fVerShiftCoordinateX_LT;
                    fYEnd = m_cLTRobotParameter.m_fEndCoordinateY;
                }
                else if (eMainStep == MainTuningStep.DIGIGAINTUNING)
                {
                    float fCenterXCoord = (float)Math.Round((m_cLTRobotParameter.m_fEndCoordinateX + m_cLTRobotParameter.m_fStartCoordinateX) / 2, 2, MidpointRounding.AwayFromZero);

                    fXStart = fCenterXCoord;
                    fYStart = m_cLTRobotParameter.m_fStartCoordinateY;
                    fXEnd = fCenterXCoord;
                    fYEnd = m_cLTRobotParameter.m_fEndCoordinateY;
                }
                else if (eMainStep == MainTuningStep.TPGAINTUNING)
                {
                    float fCenterXCoord = (float)Math.Round((m_cLTRobotParameter.m_fVerEndCoordinateX_TPGT + m_cLTRobotParameter.m_fVerStartCoordinateX_TPGT) / 2, 2, MidpointRounding.AwayFromZero);

                    fXStart = fCenterXCoord;
                    fYStart = m_cLTRobotParameter.m_fVerStartCoordinateY_TPGT;
                    fXEnd = fCenterXCoord;
                    fYEnd = m_cLTRobotParameter.m_fVerEndCoordinateY_TPGT;
                }
                else
                {
                    float fCenterXCoord = (float)Math.Round((m_cLTRobotParameter.m_fEndCoordinateX + m_cLTRobotParameter.m_fStartCoordinateX) / 2, 2, MidpointRounding.AwayFromZero);
                    float fCenterYCoord = (float)Math.Round((m_cLTRobotParameter.m_fEndCoordinateY + m_cLTRobotParameter.m_fStartCoordinateY) / 2, 2, MidpointRounding.AwayFromZero);

                    fXStart = fCenterXCoord;
                    fYStart = (float)Math.Round(fCenterYCoord - 15, 2, MidpointRounding.AwayFromZero);
                    fXEnd = fCenterXCoord;
                    fYEnd = (float)Math.Round(fCenterYCoord + 15, 2, MidpointRounding.AwayFromZero);
                }
            }
            else if (eRobotType == FlowRobot.HOVERPOINT_CEN || eRobotType == FlowRobot.TOUCHPOINT_CEN)
            {
                fXStart = (float)Math.Round((m_cLTRobotParameter.m_fEndCoordinateX + m_cLTRobotParameter.m_fStartCoordinateX) / 2, 2, MidpointRounding.AwayFromZero);
                fYStart = (float)Math.Round((m_cLTRobotParameter.m_fEndCoordinateY + m_cLTRobotParameter.m_fStartCoordinateY) / 2, 2, MidpointRounding.AwayFromZero);
                fXEnd = fXStart;
                fYEnd = fYStart;
            }
            else
            {
                fXStart = m_cLTRobotParameter.m_fStartCoordinateX;
                fYStart = m_cLTRobotParameter.m_fStartCoordinateY;
                fXEnd = m_cLTRobotParameter.m_fEndCoordinateX;
                fYEnd = m_cLTRobotParameter.m_fEndCoordinateY;
            }
        }

        /// <summary>
        /// 計算畫線速度以及Z座標
        /// </summary>
        /// <param name="fSpeed">畫線速度(mm/s)</param>
        /// <param name="fZCoord">Z座標點數值(mm)</param>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        private void ComputeSpeedAndZCoordByLTRobot(ref float fSpeed, ref float fZCoord, RobotParameter cRobotParameter)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            if (eMainStep == MainTuningStep.DIGIGAINTUNING)
                fSpeed = m_cSpeedParameter.m_fSpeed_DGT;
            else if (eMainStep == MainTuningStep.TPGAINTUNING)
                fSpeed = m_cSpeedParameter.m_fSpeed_TPGT;
            else if (eMainStep == MainTuningStep.PEAKCHECKTUNING)
                fSpeed = m_cSpeedParameter.m_fSpeed_PCT;
            else if (eMainStep == MainTuningStep.DIGITALTUNING)
                fSpeed = m_cSpeedParameter.m_fSpeed_DT;
            else if (eMainStep == MainTuningStep.TILTTUNING)
            {
                if (cRobotParameter.m_eRobotType == FlowRobot.TOUCHLINE)
                    fSpeed = m_cSpeedParameter.m_fSpeed_TTSlant;
                else
                    fSpeed = m_cSpeedParameter.m_fSpeed_TT;
            }
            else if (eMainStep == MainTuningStep.LINEARITYTUNING)
                fSpeed = m_cSpeedParameter.m_fSpeed_LT;
            else
                fSpeed = m_cSpeedParameter.m_fSpeed_DT;

            switch (cRobotParameter.m_eRobotType)
            {
                case FlowRobot.NO:
                    fSpeed = 0.0f;
                    fZCoord = 0.0f;
                    break;
                case FlowRobot.HOVERLINE:
                    switch (eSubStep)
                    {
                        case SubTuningStep.HOVER_2ND:
                            fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ - m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_DT2nd;
                            break;
                        case SubTuningStep.PCHOVER_1ST:
                            fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ - m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PCT1st;
                            break;
                        case SubTuningStep.PCHOVER_2ND:
                            fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ - m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PCT2nd;
                            break;
                        default:
                            fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ - m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_DT1st;
                            break;
                    }

                    break;
                case FlowRobot.TOUCHLINE:
                    if (eMainStep == MainTuningStep.DIGIGAINTUNING)
                    {
                        if (m_bDetectFGConnectFlag == false || m_bDetectFGPowerOnFlag == false)
                            fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ + m_cLTRobotParameter.m_fPushDownCoordinateZ_DGT;
                        else
                            fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ;
                    }
                    else
                        fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ;

                    break;
                case FlowRobot.TOUCHLINE_HOR:
                case FlowRobot.TOUCHLINE_VER:
                    if (eMainStep == MainTuningStep.DIGIGAINTUNING)
                    {
                        if (m_bDetectFGConnectFlag == false || m_bDetectFGPowerOnFlag == false)
                            fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ + m_cLTRobotParameter.m_fPushDownCoordinateZ_DGT;
                        else
                            fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ;
                    }
                    else if (eMainStep == MainTuningStep.TPGAINTUNING)
                        fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ_TPGT;
                    else
                        fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ;

                    break;
                case FlowRobot.HOVERPOINT_CEN:
                    fSpeed = 0.0f;
                    fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ - m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PP;
                    break;
                case FlowRobot.TOUCHPOINT_CEN:
                    fSpeed = 0.0f;

                    if (eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE))
                    {
                        //fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ + (float)ParamAutoTuning.m_fContactPressZAxisCoord;

                        if (m_cLTRobotParameter.m_fInitialCoordinateZ_PT == 0)
                            fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ - m_cLTRobotParameter.m_fHoverRaiseCoordinateZ_PT;
                        else
                        {
                            if (m_bDisableRobotMovingFlag == false)
                                fZCoord = (float)(Math.Round(m_cLTRobotParameter.m_fInitialCoordinateZ_PT, 2, MidpointRounding.AwayFromZero) - 0.2);
                            else
                                fZCoord = m_cLTRobotParameter.m_fInitialCoordinateZ_PT;
                        }
                    }
                    else
                        fZCoord = m_cLTRobotParameter.m_fContactCoordinateZ;

                    break;
                default:
                    fSpeed = 0.0f;
                    fZCoord = 0.0f;
                    break;
            }
        }

        /// <summary>
        /// 計算偏差的公克數
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        private void ComputeOffsetWeight(RobotParameter cRobotParameter)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            if (eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE))
            {
                int nWeightIndex = Array.FindIndex(ParamAutoTuning.m_nPressureWeight_Array, n => n == cRobotParameter.m_nPressureWeight);

                int nFirstValue = ParamAutoTuning.m_nPTFirstOffsetWeight;
                int nLastValue = ParamAutoTuning.m_nPTLastOffsetWeight;

                int nOffsetWeight = (int)(nFirstValue - ((double)(nFirstValue - nLastValue) / (ParamAutoTuning.m_nPRESSURE_DATA_NUMBER - 2) * nWeightIndex));

                m_cCurrentParameterSet.m_nOffsetWeight = nOffsetWeight;
            }
        }
        #endregion

        #region LT Robot Control Function
        /// <summary>
        /// 控制線測機回到原點
        /// </summary>
        private void ReturnToOriginByLTRobot()
        {
            lock (m_bLTRobotReturnToOriginLockFlag)
            {
                if (m_cSocket.m_bServerDisconnectFlag == true)
                    return;

                if (m_bClientCloseFlag == true)
                    return;

                string sRobotCommand = "";

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.Stop, ref sRobotCommand);
                m_cSocket.RunClientSending(ref sRobotCommand);

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 150);
                m_cSocket.RunClientSending(ref sRobotCommand);

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteZ, ref sRobotCommand, 0, 0, 0);
                m_cSocket.RunClientSending(ref sRobotCommand);

                WaitRobotStop(false);

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 150);
                m_cSocket.RunClientSending(ref sRobotCommand);

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, 0, 0, 0);
                m_cSocket.RunClientSending(ref sRobotCommand);

                WaitRobotStop(false);

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.ReturnOriginal, ref sRobotCommand);
                m_cSocket.RunClientSending(ref sRobotCommand);

                if (m_bDetectFGConnectFlag == true)
                {
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetWeightCalibration, ref sRobotCommand);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                }
            }
        }

        /// <summary>
        /// 中斷並關閉Client端連線
        /// </summary>
        private void SetClientClose()
        {
            lock (m_bClientCloseLockFlag)
            {
                if (m_cSocket.m_bServerDisconnectFlag == true)
                    return;

                //Robot Back to Original Point
                if (m_bClientCloseFlag == false)
                {
                    /*
                    string sRobotCommand = "";
                    m_cRobot.SocketRobot("NO", ref sRobotCommand);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    */

                    //Close Socket
                    string sMessage = "close client";

                    m_cSocket.RunClientSending(ref sMessage);
                    m_cSocket.RunSocketClose();
                    m_bClientCloseFlag = true;
                }
            }
        }

        /// <summary>
        /// 偵測Client端連線
        /// </summary>
        private void DetectClientSocket()
        {
            string sRealCommand = "";
            string sEchoCommand = "";

            m_cRobot.SetSocketRobot(RobotAPI.RobotCode.IsMoving, ref sRealCommand);
            sEchoCommand = sRealCommand;
            m_cSocket.RunClientSending(ref sEchoCommand);
        }
        #endregion

        #region Wait LT Robot Function
        /// <summary>
        /// 確認線測試是否完成/終止
        /// </summary>
        /// <param name="bForceStopFlag">是否要強制終止的Flag</param>
        /// <param name="bOutputMessageFlag">是否要輸出訊息</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool WaitRobotStop(bool bForceStopFlag = true, bool bOutputMessageFlag = true)
        {
            bool bRobotStopFlag = false;
            string sReadCommand = "";
            string sEchoCommand = "";
            long lStartTime = DateTime.Now.Ticks;
            long lCurrentTime = 0;
            int nCostTime = 0;
            bool bOverMovingTimeoutFlag = false;
            bool bRobotEndStopFlag = true;

            Thread tRobotMovingDetect = new Thread(() =>
            {
                bRobotEndStopFlag = false;

                while (!bRobotStopFlag)
                {
                    Thread.Sleep(500);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.IsMoving, ref sReadCommand);
                    sEchoCommand = sReadCommand;
                    m_cSocket.RunClientSending(ref sEchoCommand);
                    char charResult = sEchoCommand[0];

                    if (charResult == '1')
                    {
                        bRobotStopFlag = true;

                        if (bOutputMessageFlag == true)
                            OutputMessage("-Robot End Moving");
                    }
                    else
                    {
                        lCurrentTime = DateTime.Now.Ticks;
                        nCostTime = (int)((lCurrentTime - lStartTime) / 10000);

                        if (nCostTime > m_nRobotMovingTimeout)
                        {
                            bOverMovingTimeoutFlag = true;
                            break;
                        }
                    }
                }

                bRobotEndStopFlag = true;
            });

            tRobotMovingDetect.IsBackground = true;
            tRobotMovingDetect.Start();

            while (!bRobotStopFlag)
            {
                Thread.Sleep(500);  //Wait 500ms

                if (bOverMovingTimeoutFlag == true)
                    break;
            }

            while (bRobotEndStopFlag == false)
                Thread.Sleep(100);

            if (bOverMovingTimeoutFlag == true)
            {
                if (bForceStopFlag == true)
                {
                    m_sErrorMessage = "Robot Moving Error";
                    m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                    m_cfrmMain.m_bInterruptFlag = true;
                    Thread.Sleep(10);
                    return false;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// 確認線測試是否完成/終止
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        /// <param name="bRecordFlag">是否還在錄製Data的Flag</param>
        /// <param name="bForceStopFlag">是否要強制終止的Flag</param>
        /// <param name="bDetectOverLoadingWeightFlag">是否要偵測超出極限重量</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool WaitRobotStop(RobotParameter cRobotParameter, bool bRecordFlag = false, bool bForceStopFlag = true, bool bDetectOverLoadingWeightFlag = false)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            bool bRobotStop = false;
            string sRealCommand = "";
            string sEchoCommand = "";
            long nStart = DateTime.Now.Ticks;
            long nCurrent = 0;
            int nCostTime = 0;
            bool bOverMovingTimeout = false;
            bool bOverLoadingWeight = false;
            int nFGValue = 0;

            Thread tRobotMovingDetect = new Thread(() =>
            {
                while (!bRobotStop)
                {
                    Thread.Sleep(500);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.IsMoving, ref sRealCommand);
                    sEchoCommand = sRealCommand;
                    m_cSocket.RunClientSending(ref sEchoCommand);
                    char charResult = sEchoCommand[0];

                    if (charResult == '1')
                    {
                        bRobotStop = true;
                        OutputMessage("-Robot End Moving");
                    }
                    else
                    {
                        if (bRecordFlag == false)
                        {
                            nCurrent = DateTime.Now.Ticks;
                            nCostTime = (int)((nCurrent - nStart) / 10000);

                            if (nCostTime > m_nRobotMovingTimeout)
                            {
                                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.Stop, ref sRealCommand);
                                sEchoCommand = sRealCommand;
                                m_cSocket.RunClientSending(ref sEchoCommand);
                                bOverMovingTimeout = true;
                                break;
                            }
                        }
                    }

                    if (m_bDetectFGConnectFlag == true && m_bDetectFGPowerOnFlag == true && bDetectOverLoadingWeightFlag == true)
                    {
                        Thread.Sleep(100);
                        nFGValue = GetFGValue(false);

                        if (nFGValue > ParamAutoTuning.m_nMaxLoadingWeight)
                        {
                            m_cRobot.SetSocketRobot(RobotAPI.RobotCode.Stop, ref sRealCommand);
                            sEchoCommand = sRealCommand;
                            m_cSocket.RunClientSending(ref sEchoCommand);
                            bOverLoadingWeight = true;
                            break;
                        }
                    }
                }
            });

            tRobotMovingDetect.IsBackground = true;
            tRobotMovingDetect.Start();

            if (bRecordFlag == true)
                SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);

            while (bRobotStop == false)
            {
                Thread.Sleep(500);  //Wait 500ms

                if (bRecordFlag == true)
                {
                    nCurrent = DateTime.Now.Ticks;
                    nCostTime = (int)((nCurrent - nStart) / 10000);
                    int nReportNumber = m_byteReportData_List.Count;
                    m_dTimeCounter = Math.Round((double)nCostTime / 1000, 3, MidpointRounding.AwayFromZero);

                    if (m_cfrmMain.m_bInterruptFlag == false)
                    {
                        string sMessage = string.Format("-Time : {0:0.000} s, Report : {1}", m_dTimeCounter, nReportNumber.ToString());
                        OutputMessage(sMessage);

                        SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);
                    }
                }

                if (bOverMovingTimeout == true || bOverLoadingWeight == true)
                    break;
            }

            if (bRecordFlag == true)
            {
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
                            string sSubStepCodeName = StringConvert.m_dictSubStepCNMappingTable[eSubStep];
                            string sErrorCode = string.Format("{0}.001", sSubStepCodeName);
                            SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorCode, sErrorCode, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                            SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorMsg, m_sErrorMessage, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                            SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorMessage, m_sErrorMessage, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                        }

                        m_nRecordDataErrorFlag = 0xF000;
                    }
                    else
                        m_nRecordDataErrorFlag = 0;
                }
            }
            else
            {
                if (bOverMovingTimeout == true)
                {
                    if (bForceStopFlag == true)
                    {
                        m_sErrorMessage = "Robot Moving Timeout";
                        m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                        m_cfrmMain.m_bInterruptFlag = true;
                        Thread.Sleep(10);
                    }

                    return false;
                }

                if (bOverLoadingWeight == true)
                {
                    if (bForceStopFlag == true)
                    {
                        string sFGValue = Convert.ToString(nFGValue);
                        string sMaxLoadingWeight = Convert.ToString(ParamAutoTuning.m_nMaxLoadingWeight);
                        m_sErrorMessage = string.Format("Detect Over Loading Weight({0}g > MaxLoad({1}))", sFGValue, sMaxLoadingWeight);
                        m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                        m_cfrmMain.m_bInterruptFlag = true;
                        Thread.Sleep(10);
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 確認線測試是否完成/終止
        /// </summary>
        /// <param name="sMessage">錯誤訊息</param>
        /// <param name="bOverWriteMessageFlag">是否要複寫訊息</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool WaitRobotStop(ref string sMessage, bool bOverWriteMessageFlag = true)
        {
            bool bRobotStopFlag = false;
            string sReadCommand = "";
            string sEchoCommand = "";
            long lStartTime = DateTime.Now.Ticks;
            long lCurrentTime = 0;
            int nCostTime = 0;
            bool bOverMovingTimeoutFlag = false;
            bool bRobotEndStopFlag = true;

            Thread tRobotMovingDetect = new Thread(() =>
            {
                bRobotEndStopFlag = false;

                while (!bRobotStopFlag)
                {
                    Thread.Sleep(500);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.IsMoving, ref sReadCommand);
                    sEchoCommand = sReadCommand;
                    m_cSocket.RunClientSending(ref sEchoCommand);
                    char charResult = sEchoCommand[0];

                    if (charResult == '1')
                    {
                        bRobotStopFlag = true;
                        OutputMessage("-Robot End Moving");
                    }
                    else
                    {
                        lCurrentTime = DateTime.Now.Ticks;
                        nCostTime = (int)((lCurrentTime - lStartTime) / 10000);

                        if (nCostTime > m_nRobotMovingTimeout)
                        {
                            bOverMovingTimeoutFlag = true;
                            break;
                        }
                    }
                }

                bRobotEndStopFlag = true;
            });

            tRobotMovingDetect.IsBackground = true;
            tRobotMovingDetect.Start();

            while (!bRobotStopFlag)
            {
                Thread.Sleep(500);  //Wait 500ms

                if (bOverMovingTimeoutFlag == true)
                    break;
            }

            while (!bRobotEndStopFlag)
                Thread.Sleep(100);

            if (bOverMovingTimeoutFlag == true)
            {
                if (bOverWriteMessageFlag == true)
                    sMessage = "Robot Moving Error";

                return false;
            }

            return true;
        }

        /// <summary>
        /// 確認線測試是否完成/終止以及偵測壓力計
        /// </summary>
        /// <param name="sMessage">錯誤訊息</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool WaitRobotStopWithFGDetect(ref string sMessage)
        {
            bool bRobotStopFlag = false;
            string sReadCommand = "";
            string sEchoCommand = "";
            long lStartTime = DateTime.Now.Ticks;
            long lCurrentTime = 0;
            int nCostTime = 0;
            bool bOverMovingTimeoutFlag = false;
            bool bOverLoadingWeightFlag = false;
            int nFGValue = 0;

            Thread tRobotMovingDetect = new Thread(() =>
            {
                while (!bRobotStopFlag)
                {
                    Thread.Sleep(500);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.IsMoving, ref sReadCommand);
                    sEchoCommand = sReadCommand;
                    m_cSocket.RunClientSending(ref sEchoCommand);
                    char charResult = sEchoCommand[0];

                    if (charResult == '1')
                    {
                        bRobotStopFlag = true;
                        OutputMessage("-Robot End Moving");
                    }
                    else
                    {
                        lCurrentTime = DateTime.Now.Ticks;
                        nCostTime = (int)((lCurrentTime - lStartTime) / 10000);

                        if (nCostTime > m_nRobotMovingTimeout)
                        {
                            m_cRobot.SetSocketRobot(RobotAPI.RobotCode.Stop, ref sReadCommand);
                            sEchoCommand = sReadCommand;
                            m_cSocket.RunClientSending(ref sEchoCommand);
                            bOverMovingTimeoutFlag = true;
                            break;
                        }
                    }

                    if (m_bDetectFGConnectFlag == true && m_bDetectFGPowerOnFlag == true)
                    {
                        Thread.Sleep(100);
                        nFGValue = GetFGValue(false);

                        if (nFGValue > ParamAutoTuning.m_nMaxLoadingWeight)
                        {
                            m_cRobot.SetSocketRobot(RobotAPI.RobotCode.Stop, ref sReadCommand);
                            sEchoCommand = sReadCommand;
                            m_cSocket.RunClientSending(ref sEchoCommand);
                            bOverLoadingWeightFlag = true;
                            break;
                        }
                    }
                }
            });

            tRobotMovingDetect.IsBackground = true;
            tRobotMovingDetect.Start();

            while (!bRobotStopFlag)
            {
                Thread.Sleep(500);  //Wait 500ms

                if (bOverMovingTimeoutFlag == true || bOverLoadingWeightFlag == true)
                    break;
            }

            if (bOverMovingTimeoutFlag == true)
            {
                sMessage = "Robot Moving Timeout";
                return false;
            }

            if (bOverLoadingWeightFlag == true)
            {
                string sFGValue = Convert.ToString(nFGValue);
                string sMaxLoadingWeight = Convert.ToString(ParamAutoTuning.m_nMaxLoadingWeight);
                sMessage = string.Format("Detect Over Loading Weight({0}g > MaxLoad({1}))", sFGValue, sMaxLoadingWeight);
                return false;
            }

            return true;
        }
        #endregion

        /// <summary>
        /// 讀取壓力計公克數值
        /// </summary>
        /// <param name="bDetectErrorFlag">是否要偵測壓力計錯誤</param>
        /// <returns>回傳壓力計公克數值(g)</returns>
        private int GetFGValue(bool bDetectErrorFlag = true)
        {
            if (m_bDetectFGConnectFlag == false || m_bDetectFGPowerOnFlag == false)
            {
                m_sErrorMessage = "Pressure Robot Detect Error";
                m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
                return -1;
            }

            int nFGValue = 0;
            string sRobotCommand = "";

            m_cRobot.SetSocketRobot(RobotAPI.RobotCode.GetWeight, ref sRobotCommand);
            m_cSocket.RunClientSending(ref sRobotCommand);

            char[] charDelimiter_Array = 
            { 
                '\0' 
            };

            string[] sSplitString_Array = sRobotCommand.Split(charDelimiter_Array);

            if (ElanConvert.CheckIsInt(sSplitString_Array[0]) == false)
            {
                if (bDetectErrorFlag == true)
                {
                    m_sErrorMessage = "Pressure Getting Value Format Error";
                    m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                    m_cfrmMain.m_bInterruptFlag = true;
                    Thread.Sleep(10);
                }

                return -1;
            }
            else
            {
                nFGValue = Convert.ToInt32(sSplitString_Array[0]);

                if (nFGValue <= -999)
                {
                    if (bDetectErrorFlag == true)
                    {
                        string sFGValue = Convert.ToString(nFGValue);
                        m_sErrorMessage = string.Format("Pressure Setting Value Error[{0}]", sFGValue);
                        m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                        m_cfrmMain.m_bInterruptFlag = true;
                        Thread.Sleep(10);
                    }

                    return -1;
                }
            }

            return nFGValue;
        }

        /// <summary>
        /// 設定壓力計校正
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        private void SetFGCalibration(RobotParameter cRobotParameter)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            if (eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE))
            {
                string sRobotCommand = "";

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetWeightCalibration, ref sRobotCommand);
                m_cSocket.RunClientSending(ref sRobotCommand);
            }
        }

        /// <summary>
        /// 設定壓力計公克數值之主流程
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        /// <param name="fxStart">X座標點數值(mm)</param>
        /// <param name="fyStart">Y座標點數值(mm)</param>
        /// <param name="fzCoord">Z座標點數值(mm)</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool SetFGWeightValue(RobotParameter cRobotParameter, float fXStart, float fYStart, float fZCoord)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            if (eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE))
            {
                if (eMainStep == MainTuningStep.PRESSURETUNING && eSubStep == SubTuningStep.PRESSURESETTING)
                {
                    if (SetAndCheckFGValue(cRobotParameter, fXStart, fYStart, fZCoord, ParamAutoTuning.m_nPTInitialWeight, m_nPRESSUREROBOTSTATE_GET100GZCOORD) == false)
                        return false;
                    if (SetAndCheckFGValue(cRobotParameter, fXStart, fYStart, m_cLTRobotParameter.m_fInitialCoordinateZ_PT) == false)
                        return false;
                }
                else
                {
                    if (cRobotParameter.m_nPressureWeight <= 25)
                    {
                        if (SetAndCheckFGValue(cRobotParameter, fXStart, fYStart, fZCoord, ParamAutoTuning.m_nPTInitialWeight, m_nPRESSUREROBOTSTATE_SET100GZCOORD) == false)
                            return false;
                        if (SetAndCheckFGValue(cRobotParameter, fXStart, fYStart, m_cLTRobotParameter.m_fInitialCoordinateZ_PT) == false)
                            return false;
                    }
                    else
                    {
                        if (SetAndCheckFGValue(cRobotParameter, fXStart, fYStart, fZCoord) == false)
                            return false;
                    }
                }

                if (eSubStep == SubTuningStep.PRESSURETABLE)
                {
                    if (m_bDisableRobotMovingFlag == false)
                        m_bDisableRobotMovingFlag = true;

                    if (m_bDisableSetCommandFlag == false)
                        m_bDisableSetCommandFlag = true;
                }
            }

            return true;
        }

        /// <summary>
        /// 設定及確認壓力計的公克數值
        /// </summary>
        /// <param name="cRobotParameter">線測機相關參數Class</param>
        /// <param name="fXStart">X座標點數值(mm)</param>
        /// <param name="fYStart">Y座標點數值(mm)</param>
        /// <param name="fZCoord">Z初始座標點數值(mm)</param>
        /// <param name="nWeightValue">設定的公克數值(g)</param>
        /// <param name="nState">設定公克數值的狀態</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool SetAndCheckFGValue(RobotParameter cRobotParameter, float fXStart, float fYStart, float fZCoord, int nWeightValue = -1000, int nState = m_nPRESSUREROBOTSTATE_NORMAL)
        {
            int nDropBaseWeight_LB = 100;
            int nDropBaseWeight_HB = 300;

            if (m_bDetectFGConnectFlag == false || m_bDetectFGPowerOnFlag == false)
            {
                m_sErrorMessage = "Pressure Robot Detect Error";
                m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
                return false;
            }

            bool bPressureWeight = false;

            if (nWeightValue == -1000)
            {
                nWeightValue = cRobotParameter.m_nPressureWeight + m_cCurrentParameterSet.m_nOffsetWeight;
                m_cCurrentParameterSet.m_nTotalWeight = nWeightValue;
                bPressureWeight = true;
            }

            string sWeightValue = Convert.ToString(nWeightValue);
            OutputMessage(string.Format("-Set Weight = {0}g", sWeightValue));

            int nFGValue = 0;
            string sRobotCommand = "";

            float f1stValue = (float)ParamAutoTuning.m_dPT1stZDropScale;
            float f2ndValue = (float)ParamAutoTuning.m_dPT2ndZDropScale;
            string s1stValue = Convert.ToString(ParamAutoTuning.m_dPT1stZDropScale);
            string s2ndValue = Convert.ToString(ParamAutoTuning.m_dPT2ndZDropScale);
            int n1stDoubleDigits = 2;
            int n2ndDoubleDigits = 2;

            if (bPressureWeight == false)
            {
                f1stValue = (float)ParamAutoTuning.m_dPTIni1stZDropScale;
                f2ndValue = (float)ParamAutoTuning.m_dPTIni2ndZDropScale;
                s1stValue = Convert.ToString(ParamAutoTuning.m_dPTIni1stZDropScale);
                s2ndValue = Convert.ToString(ParamAutoTuning.m_dPTIni2ndZDropScale);
            }

            string[] sSplit1stValue_Array = s1stValue.Split('.');
            string[] sSplit2ndValue_Array = s2ndValue.Split('.');

            if (sSplit1stValue_Array.Length <= 1)
            {
                m_sErrorMessage = "Pressure 1st Drop Scale Setting Error";
                m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
                return false;
            }
            else
                n1stDoubleDigits = sSplit1stValue_Array[sSplit1stValue_Array.Length - 1].Length;

            if (sSplit2ndValue_Array.Length <= 1)
            {
                m_sErrorMessage = "Pressure 2nd Drop Scale Setting Error";
                m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
                return false;
            }
            else
                n2ndDoubleDigits = sSplit2ndValue_Array[sSplit2ndValue_Array.Length - 1].Length;

            if (nWeightValue <= 50 && bPressureWeight == true)
            {
                f1stValue = (float)ParamAutoTuning.m_dPT1stZDropScale;
                f2ndValue = (float)ParamAutoTuning.m_dPT2ndZDropScale;
                s1stValue = Convert.ToString(ParamAutoTuning.m_dPT1stZDropScale);
                s2ndValue = Convert.ToString(ParamAutoTuning.m_dPT2ndZDropScale);

                if (sSplit1stValue_Array.Length <= 1)
                {
                    m_sErrorMessage = "Pressure 1st Drop Scale Setting Error";
                    m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                    m_cfrmMain.m_bInterruptFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
                else
                    n1stDoubleDigits = sSplit1stValue_Array[sSplit1stValue_Array.Length - 1].Length;

                if (sSplit2ndValue_Array.Length <= 1)
                {
                    m_sErrorMessage = "Pressure 2nd Drop Scale Setting Error";
                    m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                    m_cfrmMain.m_bInterruptFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
                else
                    n2ndDoubleDigits = sSplit2ndValue_Array[sSplit2ndValue_Array.Length - 1].Length;
            }

            m_cRobot.SetSocketRobot(RobotAPI.RobotCode.GetWeight, ref sRobotCommand);
            m_cSocket.RunClientSending(ref sRobotCommand);

            if (sRobotCommand == null)
            {
                m_sErrorMessage = "Pressure Setting Value Null Error";
                m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
                return false;
            }
            else
            {
                char[] charDelimiter_Array = 
                { 
                    '\0'
                };

                string[] sSplitString_Array = sRobotCommand.Split(charDelimiter_Array);

                if (ElanConvert.CheckIsInt(sSplitString_Array[0]) == false)
                {
                    m_sErrorMessage = "Pressure Setting Value Format Error";
                    m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                    m_cfrmMain.m_bInterruptFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
                else
                {
                    nFGValue = Convert.ToInt32(sSplitString_Array[0]);

                    if (nFGValue <= -999)
                    {
                        m_sErrorMessage = string.Format("Pressure Setting Value Error", nFGValue);
                        m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                        m_cfrmMain.m_bInterruptFlag = true;
                        Thread.Sleep(10);
                        return false;
                    }
                }
            }

            int nPreviousFGValue = 0;
            bool bFGStopFlag = false;
            bool bFirstDropFlag = true;

            while (!bFGStopFlag)
            {
                nPreviousFGValue = nFGValue;

                int nBaseValue = 0;

                if (bPressureWeight == false)
                    nBaseValue = 20;

                if (nState == m_nPRESSUREROBOTSTATE_GET100GZCOORD && m_bGetOver100gCoordFlag == false && (nPreviousFGValue > nDropBaseWeight_LB && nPreviousFGValue < nDropBaseWeight_HB))
                {
                    m_fPTOver100gZCoord = fZCoord;
                    m_bGetOver100gCoordFlag = true;
                }

                if (m_bGetOver100gCoordFlag == true && bFirstDropFlag == true && nState == m_nPRESSUREROBOTSTATE_SET100GZCOORD)
                {
                    fZCoord = m_fPTOver100gZCoord;
                    bFirstDropFlag = false;
                }
                else if (nPreviousFGValue <= nBaseValue)
                    fZCoord = (float)Math.Round(fZCoord + f1stValue, n1stDoubleDigits, MidpointRounding.AwayFromZero);
                else if (nPreviousFGValue < nWeightValue)
                    fZCoord = (float)Math.Round(fZCoord + f2ndValue, n2ndDoubleDigits, MidpointRounding.AwayFromZero);
                else if (nPreviousFGValue > nWeightValue || nPreviousFGValue > 400)
                    fZCoord = (float)Math.Round(fZCoord - f2ndValue, n2ndDoubleDigits, MidpointRounding.AwayFromZero);

                if (nPreviousFGValue <= nBaseValue || nPreviousFGValue < nWeightValue || nPreviousFGValue > nWeightValue || nPreviousFGValue > 400)
                {
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, fXStart, fYStart, fZCoord);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    WaitRobotStop(false, false);
                    nFGValue = GetFGValue();
                }

                if (nPreviousFGValue > nBaseValue)
                {
                    if ((nPreviousFGValue < nWeightValue && nFGValue > nWeightValue) ||
                        (nPreviousFGValue > nWeightValue && nFGValue < nWeightValue) ||
                        nFGValue == nWeightValue)
                    {
                        bFGStopFlag = true;

                        if (nState == m_nPRESSUREROBOTSTATE_GET100GZCOORD && m_bGetOver100gCoordFlag == false)
                        {
                            m_fPTOver100gZCoord = fZCoord;
                            m_bGetOver100gCoordFlag = true;
                        }
                    }
                }
            }

            //After While Loop, FGValue > Target > nPreviousFGValue
            if (nPreviousFGValue < nWeightValue && nFGValue > nWeightValue)
            {
                if ((nFGValue - nWeightValue) > (nWeightValue - nPreviousFGValue))
                {
                    float fValue = (float)Math.Round(f2ndValue / 2, n2ndDoubleDigits + 1, MidpointRounding.AwayFromZero);
                    fZCoord = (float)Math.Round(fZCoord - fValue, n2ndDoubleDigits + 1, MidpointRounding.AwayFromZero);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, fXStart, fYStart, fZCoord);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    WaitRobotStop(false, false);
                    nFGValue = GetFGValue();
                }
            }
            else if (nPreviousFGValue > nWeightValue && nFGValue < nWeightValue)
            {
                if ((nPreviousFGValue - nWeightValue) < (nWeightValue - nFGValue))
                {
                    float fValue = (float)Math.Round(f2ndValue / 2, n2ndDoubleDigits + 1, MidpointRounding.AwayFromZero);
                    fZCoord = (float)Math.Round(fZCoord + fValue, n2ndDoubleDigits + 1, MidpointRounding.AwayFromZero);
                    m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, fXStart, fYStart, fZCoord);
                    m_cSocket.RunClientSending(ref sRobotCommand);
                    WaitRobotStop(false, false);
                    nFGValue = GetFGValue();
                }
            }

            m_cCurrentParameterSet.m_nStartWeight = nFGValue;

            /*
            if (bPressureWeight == true)
                m_cLTRobotParameter.m_fInitialCoordinateZ_PT = (float)(Math.Round(fzCoord, 2, MidpointRounding.AwayFromZero) - 0.2);
            else
                m_cLTRobotParameter.m_fInitialCoordinateZ_PT = fZCoord;
            */

            m_cLTRobotParameter.m_fInitialCoordinateZ_PT = fZCoord;

            string sFGValue = Convert.ToString(nFGValue);
            OutputMessage(string.Format("-Current Weight = {0}g", sFGValue));

            return true;
        }

        /// <summary>
        /// 讀取壓力計公克數值之主流程
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool GetFGWeightValue(RobotParameter cRobotParameter)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            if (eMainStep == MainTuningStep.PRESSURETUNING && (eSubStep == SubTuningStep.PRESSURESETTING || eSubStep == SubTuningStep.PRESSURETABLE))
            {
                if (GetAndCheckFGValue(cRobotParameter) == false)
                    return false;

                int nStartOriginWeight = m_cCurrentParameterSet.m_nStartWeight - m_cCurrentParameterSet.m_nOffsetWeight;
                int nStartDifferWeight = Math.Abs(cRobotParameter.m_nPressureWeight - nStartOriginWeight);

                if (nStartDifferWeight > ParamAutoTuning.m_nPTMaxWeightDiffer)
                {
                    m_sErrorMessage = string.Format("Start Weight Value Differ Too Large[{0}>HB:{1}]", nStartDifferWeight, ParamAutoTuning.m_nPTMaxWeightDiffer);
                    m_nRecordDataErrorFlag = 0xF002;
                }

                int nEndOriginWeight = m_cCurrentParameterSet.m_nEndWeight - m_cCurrentParameterSet.m_nOffsetWeight;
                int nEndDifferWeight = Math.Abs(cRobotParameter.m_nPressureWeight - nEndOriginWeight);

                if (nEndDifferWeight > ParamAutoTuning.m_nPTMaxWeightDiffer)
                {
                    m_sErrorMessage = string.Format("End Weight Value Differ Too Large[{0}>HB:{1}]", nEndDifferWeight, ParamAutoTuning.m_nPTMaxWeightDiffer);
                    m_nRecordDataErrorFlag = 0xF004;
                }

                int nOffsetValue = Math.Abs(m_cCurrentParameterSet.m_nStartWeight - m_cCurrentParameterSet.m_nEndWeight);

                if (nOffsetValue > ParamAutoTuning.m_nPTMaxOffsetDiffer)
                {
                    m_sErrorMessage = string.Format("Pressure Weight Value Offset Too Large[{0}>HB:{1}]", nOffsetValue, ParamAutoTuning.m_nPTMaxOffsetDiffer);
                    m_nRecordDataErrorFlag = 0xF001;
                }
            }

            return true;
        }

        /// <summary>
        /// 讀取及確認壓力計的公克數值
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool GetAndCheckFGValue(RobotParameter cRobotParameter)
        {
            if (m_bDetectFGConnectFlag == false || m_bDetectFGPowerOnFlag == false)
            {
                m_sErrorMessage = "Pressure Robot Detect Error";
                m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
                return false;
            }

            int nFGValue = 0;
            string sRobotCommand = "";

            m_cRobot.SetSocketRobot(RobotAPI.RobotCode.GetWeight, ref sRobotCommand);
            m_cSocket.RunClientSending(ref sRobotCommand);

            if (sRobotCommand == null)
            {
                m_sErrorMessage = "Pressure Getting Value Null Error";
                m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
                return false;
            }
            else
            {
                char[] charDelimiter_Array = 
                { 
                    '\0' 
                };

                string[] sSplitString_Array = sRobotCommand.Split(charDelimiter_Array);

                if (ElanConvert.CheckIsInt(sSplitString_Array[0]) == false)
                {
                    m_sErrorMessage = "Pressure Getting Value Format Error";
                    m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                    m_cfrmMain.m_bInterruptFlag = true;
                    Thread.Sleep(10);
                    return false;
                }
                else
                {
                    nFGValue = Convert.ToInt32(sSplitString_Array[0]);

                    if (nFGValue >= 0)
                        m_cCurrentParameterSet.m_nEndWeight = nFGValue;
                    else
                    {
                        string sFGValue = Convert.ToString(nFGValue);
                        m_sErrorMessage = string.Format("Pressure Getting Value Error[{0}]", sFGValue);
                        m_cFinishFlowParameter.m_bOutputMessageFlag = false;
                        m_cfrmMain.m_bInterruptFlag = true;
                        Thread.Sleep(10);
                        return false;
                    }
                }
            }

            OutputMessage(string.Format("-Get Weight = {0}g", Convert.ToString(nFGValue)));

            return true;
        }

        #region Pressure Robot Control Function
        /// <summary>
        /// 確認壓力計是否有連線
        /// </summary>
        /// <param name="nStepIndex">現階段索引值</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool CheckPressureRobotConnect(int nStepIndex)
        {
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT && nStepIndex == 0)
            {
                OutputMessage("-Check Pressure Robot Connect");

                SubTuningStep[] eSubStep_Array = new SubTuningStep[] 
                { 
                    SubTuningStep.PRESSURESETTING, 
                    SubTuningStep.PRESSURETABLE 
                };

                bool bSetStepFlag_PressureTuning = false;

                for (int nSubStepIndex = 0; nSubStepIndex < eSubStep_Array.Length; nSubStepIndex++)
                {
                    FlowStep cFlowStep = m_cfrmMain.m_cFlowStep_List.Find(nSelectStepIndex => (nSelectStepIndex.m_eSubStep == eSubStep_Array[nSubStepIndex]));

                    if (cFlowStep != null)
                    {
                        bSetStepFlag_PressureTuning = true;
                        break;
                    }
                }

                string sRobotCommand = "";

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.GetWeight, ref sRobotCommand);
                m_cSocket.RunClientSending(ref sRobotCommand);

                if (sRobotCommand == null)
                {
                    if (bSetStepFlag_PressureTuning == true)
                    {
                        m_sErrorMessage = "Pressure Robot Connect Error";
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
                    char[] charDelimiter_Array = 
                    { 
                        '\0' 
                    };

                    string[] sSplitString_Array = sRobotCommand.Split(charDelimiter_Array);

                    if (ElanConvert.CheckIsInt(sSplitString_Array[0]) == false)
                    {
                        if (bSetStepFlag_PressureTuning == true)
                        {
                            m_sErrorMessage = "Pressure Robot Connect Value Format Error";
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
                        int nWeightValue = Convert.ToInt32(sSplitString_Array[0]);

                        if (nWeightValue <= -999)
                        {
                            if (bSetStepFlag_PressureTuning == true)
                            {
                                m_sErrorMessage = "Pressure Robot Connect Error. Please Connect and Power On the Pressure Robot";
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
                            m_bDetectFGConnectFlag = true;
                    }
                }
            }

            OutputMessage(string.Format("m_bDetectFGConnectFlag = {0}", m_bDetectFGConnectFlag.ToString()));

            return true;
        }

        /// <summary>
        /// 確認壓力計是否有啟動
        /// </summary>
        /// <param name="nStepIndex">現階段索引值</param>
        /// <returns>true:無發生錯誤  false:有發生錯誤</returns>
        private bool CheckPressureRobotPowerOn(int nStepIndex, FlowStep cFlowStep)
        {
            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT && (nStepIndex == 0 || (cFlowStep.m_eMainStep == MainTuningStep.PRESSURETUNING && cFlowStep.m_eSubStep == SubTuningStep.PRESSURESETTING)))
            {
                OutputMessage("-Check Pressure Robot Power On");

                if (m_bDetectFGConnectFlag == false)
                    return true;

                m_bDetectFGPowerOnFlag = false;

                string sRobotCommand = "";
                string sMessage = "";
                int nFirstWeight = 0;

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 150);
                m_cSocket.RunClientSending(ref sRobotCommand);
                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, m_cLTRobotParameter.m_fStartCoordinateX, m_cLTRobotParameter.m_fStartCoordinateY);
                m_cSocket.RunClientSending(ref sRobotCommand);

                if (WaitRobotStop(ref sMessage) == false)
                {
                    RunPressureRobotPowerOnErrorFlow(false, nStepIndex, sMessage);
                    return false;
                }

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 50);
                m_cSocket.RunClientSending(ref sRobotCommand);
                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, m_cLTRobotParameter.m_fStartCoordinateX, m_cLTRobotParameter.m_fStartCoordinateY, m_cLTRobotParameter.m_fContactCoordinateZ);
                m_cSocket.RunClientSending(ref sRobotCommand);

                if (WaitRobotStopWithFGDetect(ref sMessage) == false)
                {
                    RunPressureRobotPowerOnErrorFlow(false, nStepIndex, sMessage);
                    return false;
                }

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.GetWeight, ref sRobotCommand);
                m_cSocket.RunClientSending(ref sRobotCommand);

                if (sRobotCommand == null)
                {
                    RunPressureRobotPowerOnErrorFlow(true, nStepIndex, "Pressure Robot PowerOn Error");
                    return false;
                }
                else
                {
                    char[] charDelimiter_Array = 
                    { 
                        '\0' 
                    };

                    string[] sSplitString_Array = sRobotCommand.Split(charDelimiter_Array);

                    if (ElanConvert.CheckIsInt(sSplitString_Array[0]) == false)
                    {
                        RunPressureRobotPowerOnErrorFlow(true, nStepIndex, "Pressure Robot PowerOn Value Format Error");
                        return false;
                    }
                    else
                    {
                        nFirstWeight = Convert.ToInt32(sSplitString_Array[0]);

                        if (nFirstWeight > ParamAutoTuning.m_dPTFGDetectPowerOnMaxWeight)
                        {
                            RunPressureRobotPowerOnErrorFlow(true, nStepIndex, string.Format("Pressure Robot PowerOn Value Error[{0}]. Please Reconnect and Power On the Pressure Robot", nFirstWeight));
                            return false;
                        }
                    }
                }

                float fZStartDropDown = (float)Math.Round(m_cLTRobotParameter.m_fContactCoordinateZ + ParamAutoTuning.m_dPTFGDetectPowerOnZDropScale, 2, MidpointRounding.AwayFromZero);

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, m_cLTRobotParameter.m_fStartCoordinateX, m_cLTRobotParameter.m_fStartCoordinateY, fZStartDropDown);
                m_cSocket.RunClientSending(ref sRobotCommand);

                if (WaitRobotStop(ref sMessage) == false)
                {
                    RunPressureRobotPowerOnErrorFlow(false, nStepIndex, sMessage);
                    return false;
                }

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.GetWeight, ref sRobotCommand);
                m_cSocket.RunClientSending(ref sRobotCommand);

                if (sRobotCommand == null)
                {
                    RunPressureRobotPowerOnErrorFlow(true, nStepIndex, "Pressure Robot PowerOn Error");
                    return false;
                }
                else
                {
                    char[] charDelimiter_Array = 
                    { 
                        '\0' 
                    };

                    string[] sSplitString_Array = sRobotCommand.Split(charDelimiter_Array);

                    if (ElanConvert.CheckIsInt(sSplitString_Array[0]) == false)
                    {
                        RunPressureRobotPowerOnErrorFlow(true, nStepIndex, "Pressure Robot PowerOn Value Format Error");
                        return false;
                    }
                    else
                    {
                        int nWeightValue = Convert.ToInt32(sSplitString_Array[0]);

                        if (nWeightValue <= 0 || nWeightValue <= nFirstWeight)
                        {
                            RunPressureRobotPowerOnErrorFlow(true, nStepIndex, string.Format("Pressure Robot PowerOn Value Error[{0}, {1}]. Please Power On the Pressure Robot", nFirstWeight, nWeightValue));
                            return false;
                        }
                        else
                            m_bDetectFGPowerOnFlag = true;
                    }
                }

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 150);
                m_cSocket.RunClientSending(ref sRobotCommand);
                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, m_cLTRobotParameter.m_fStartCoordinateX, m_cLTRobotParameter.m_fStartCoordinateY);
                m_cSocket.RunClientSending(ref sRobotCommand);

                if (WaitRobotStop(ref sMessage) == false)
                {
                    RunPressureRobotPowerOnErrorFlow(false, nStepIndex, sMessage);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 壓力計啟動錯誤後的流程
        /// </summary>
        /// <param name="bPressureRobotErrorFlag">壓力計錯誤的Flag</param>
        /// <param name="nStepIndex">現階段索引值</param>
        /// <param name="sMessage">錯誤訊息</param>
        private void RunPressureRobotPowerOnErrorFlow(bool bPressureRobotErrorFlag, int nStepIndex, string sMessage)
        {
            if (bPressureRobotErrorFlag == true)
            {
                string sRobotCommand = "";

                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetSpeed, ref sRobotCommand, 0, 0, 0, 150);
                m_cSocket.RunClientSending(ref sRobotCommand);
                m_cRobot.SetSocketRobot(RobotAPI.RobotCode.SetAbsoluteMove, ref sRobotCommand, m_cLTRobotParameter.m_fStartCoordinateX, m_cLTRobotParameter.m_fStartCoordinateY);
                m_cSocket.RunClientSending(ref sRobotCommand);
                WaitRobotStop(ref sMessage, false);
            }

            m_sErrorMessage = sMessage;
            m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
            m_cFinishFlowParameter.m_bOutputMessageFlag = false;
            m_cfrmMain.m_bInterruptFlag = true;
            Thread.Sleep(10);
        }
        #endregion
    }
}
