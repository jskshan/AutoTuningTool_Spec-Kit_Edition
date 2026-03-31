using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using Elan;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class ProcessFlow
    {
        /// <summary>
        /// 單機模式控制主流程
        /// </summary>
        /// <param name="objRobotInfo">線測機相關資訊之物件</param>
        private void RunFakeRobotThread(object objRobotInfo)
        {
            RobotParameter cRobotParameter = (RobotParameter)objRobotInfo;

            InitialRobotThreadParameter();

            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;
            FlowRobot eRobotType = cRobotParameter.m_eRobotType;
            int nFlowIndex = cRobotParameter.m_nFlowIndex;

            string sMessage = string.Format("-FakeRobotThread : Robot Stage={0}", eRobotType.ToString());
            OutputMessage(sMessage);

            //Robot is Moving to Start Location
            OutputMessage("-FakeRobotThread : Fake Robot is Moving to Start Location");

            if (eMainStep == MainTuningStep.TPGAINTUNING && eSubStep == SubTuningStep.TP_GAIN)
            {
                if (ParamAutoTuning.m_nTPGTDisplayMessage != 0)
                    ShowWarningMessageByTPGainCoordinateSetting(cRobotParameter.m_cFlowStep, eRobotType);
            }

            m_nRobotPrepareFlag |= MainConstantParameter.m_nFLOWSTATE_FIRSTSTEP;

            if (eSubStep == SubTuningStep.PRESSURETABLE)
            {
                m_cCurrentParameterSet.m_nRealityWeight = cRobotParameter.m_nPressureWeight;
                m_cCurrentParameterSet.m_nOffsetWeight = 0;
                m_cCurrentParameterSet.m_nExtraIncWeight = 0;
                m_cCurrentParameterSet.m_nPTPenVersion = 0;
                m_cCurrentParameterSet.m_nTotalWeight = cRobotParameter.m_nPressureWeight;
            }

            m_nRobotPrepareFlag |= MainConstantParameter.m_nFLOWSTATE_SECONDSTEP;

            //Wait Record Prepare Flag (Record: Command to IC)
            OutputMessage("-FakeRobotThread : Wait Record Prepare");

            while (m_nRecordPrepareFlag < MainConstantParameter.m_nFLOWSTATE_SECONDSTEP)
            {
                SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);
                Thread.Sleep(500);
            };

            if (m_bForceStopFlag == true)
                m_bRobotFinishedFlag = true;
            else
            {
                OutputMessage("-FakeRobotThread : Record Prepare");

                //Robot Start Moving
                switch (eRobotType)
                {
                    case FlowRobot.NO:
                        WaitGetDataComplete(cRobotParameter, false);
                        break;
                    case FlowRobot.HOVERLINE:
                    case FlowRobot.TOUCHLINE:
                    case FlowRobot.TOUCHLINE_HOR:
                    case FlowRobot.TOUCHLINE_VER:
                    case FlowRobot.HOVERPOINT_CEN:
                    case FlowRobot.TOUCHPOINT_CEN:
                        WaitDrawFinish(cRobotParameter);
                        break;
                    default: break;
                }

                m_bRobotFinishedFlag = true;

                OutputMessage("-FakeRobotThread : Record Finished. Back to Origin Location");

                if (eRobotType != FlowRobot.NO)
                {
                    SetNewDrawButton(MainConstantParameter.m_nDRAWSTATE_FINISH);
                    OutputStatusAndErrorMessageLabel("Execute", "", Color.Blue, true);
                }

                CheckReportDataIsValid(cRobotParameter, nFlowIndex, true);
            }
        }

        #region Wait Get Data Function
        /// <summary>
        /// 確認讀取Report Data完成
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        /// <param name="bClientOrGoDrawModeFlag">是否為Client/GoDraw Mode</param>
        private void WaitGetDataComplete(RobotParameter cRobotParameter, bool bClientOrGoDrawModeFlag)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            string sReadCommand = "";
            string sEchoCommand = "";
            long lStartTime = DateTime.Now.Ticks;
            long lCurrentTime = 0;
            int nCostTime = 0;
            bool bRobotEndStopFlag = true;
            bool bGetDataFinishFlag = false;

            if (eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1)
            {
                if (bClientOrGoDrawModeFlag == true)
                {
                    Thread tRobotMovingDetect = new Thread(() =>
                    {
                        bRobotEndStopFlag = false;

                        while (bGetDataFinishFlag == false)
                        {
                            Thread.Sleep(500);
                            m_cRobot.SetSocketRobot(RobotAPI.RobotCode.IsMoving, ref sReadCommand);
                            sEchoCommand = sReadCommand;
                            m_cSocket.RunClientSending(ref sEchoCommand);
                        }

                        bRobotEndStopFlag = true;
                    });

                    tRobotMovingDetect.IsBackground = true;
                    tRobotMovingDetect.Start();
                }

                GetFrameData.GetDataState eGetDataState = GetFrameData.GetDataState.GetDataState_NA;

                while (true)
                {
                    if (m_cGetFrameData != null)
                    {
                        eGetDataState = m_cGetFrameData.GetGetDataState();

                        if (eGetDataState != GetFrameData.GetDataState.GetDataState_NA)
                            break;
                    }

                    Thread.Sleep(500);
                }

                bGetDataFinishFlag = true;

                if (eGetDataState != GetFrameData.GetDataState.GetDataState_Success)
                {
                    m_nRecordDataErrorFlag = 0;

                    if (eGetDataState == GetFrameData.GetDataState.GetDataState_GetError)
                        m_sErrorMessage = m_cGetFrameData.GetErrorMessage();
                    else if (eGetDataState == GetFrameData.GetDataState.GetDataState_ReconnectError)
                        m_sErrorMessage = m_cGetFrameData.GetErrorMessage();
                }
            }
            else
            {
                int nDataNumberBoundary = m_nNoiseReportNumber;
                int nDataValidNumberBoundary = m_nNoiseValidReportNumber;
                int nCostTimeOut = m_nNoiseWaitTime;

                bool bNoReportFlag = false;
                long lNoReportStartTime = 0;
                int nPreviousDataNumber = 0;
                int nNoReportCheckStartTime = 5000;

                long lPreviousTime = 0;
                double dReportRate = 0.0;
                List<double> dReportRate_List = new List<double>();
                double dAverageReportRate = 0.0;

                if (eMainStep == MainTuningStep.PRESSURETUNING)
                {
                    switch (eSubStep)
                    {
                        case SubTuningStep.PRESSURESETTING:
                        case SubTuningStep.PRESSURETABLE:
                            nCostTimeOut = m_nPTRecordTime;
                            nDataNumberBoundary = m_nPTValidReportNumber;
                            nDataValidNumberBoundary = m_nPTValidReportNumber;
                            break;
                        case SubTuningStep.PRESSUREPROTECT:
                            nCostTimeOut = m_nPPRecordTime;
                            nDataNumberBoundary = m_nPPValidReportNumber;
                            nDataValidNumberBoundary = m_nPPValidReportNumber;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    {
                        if (cRobotParameter.m_eRecordType == FlowRecord.NTRX ||
                            cRobotParameter.m_eRecordType == FlowRecord.NRX ||
                            cRobotParameter.m_eRecordType == FlowRecord.NTX)
                        {
                            int nSectorNumber = 0;

                            if (cRobotParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                            {
                                nSectorNumber = m_nRXTraceNumber / MainConstantParameter.m_nReportMaxNumber_Gen8;

                                if ((m_nRXTraceNumber % MainConstantParameter.m_nReportMaxNumber_Gen8) > 0)
                                    nSectorNumber++;
                            }
                            else if (cRobotParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                            {
                                nSectorNumber = m_nTXTraceNumber / MainConstantParameter.m_nReportMaxNumber_Gen8;

                                if ((m_nRXTraceNumber % MainConstantParameter.m_nReportMaxNumber_Gen8) > 0)
                                    nSectorNumber++;
                            }

                            nDataNumberBoundary = m_nNoiseReportNumber * nSectorNumber;

                            if (m_nNoiseReportNumber < m_nNoiseValidReportNumber)
                                nDataValidNumberBoundary = m_nNoiseReportNumber * nSectorNumber;
                            else
                                nDataValidNumberBoundary = m_nNoiseValidReportNumber * nSectorNumber;
                        }
                        else if (cRobotParameter.m_eRecordType == FlowRecord.PTHF_NoSync_Gen8 ||
                                 cRobotParameter.m_eRecordType == FlowRecord.BHF_NoSync_Gen8)
                        {
                            int nSectorNumber = 0;

                            if (cRobotParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                            {
                                nSectorNumber = m_nRXTraceNumber / MainConstantParameter.m_nReportMaxNumber_Gen8;

                                if ((m_nRXTraceNumber % MainConstantParameter.m_nReportMaxNumber_Gen8) > 0)
                                    nSectorNumber++;
                            }
                            else if (cRobotParameter.m_nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                            {
                                nSectorNumber = m_nTXTraceNumber / MainConstantParameter.m_nReportMaxNumber_Gen8;

                                if ((m_nRXTraceNumber % MainConstantParameter.m_nReportMaxNumber_Gen8) > 0)
                                    nSectorNumber++;
                            }

                            nDataNumberBoundary = m_nNoiseReportNumber * nSectorNumber;

                            if (m_nNoiseReportNumber < m_nNoiseValidReportNumber)
                                nDataValidNumberBoundary = m_nNoiseReportNumber * nSectorNumber;
                            else
                                nDataValidNumberBoundary = m_nNoiseValidReportNumber * nSectorNumber;
                        }
                    }
                    else
                    {
                        switch (cRobotParameter.m_eRecordType)
                        {
                            case FlowRecord.NTRX:
                                nDataNumberBoundary = m_nNoiseReportNumber * 6;
                                nDataValidNumberBoundary = m_nNoiseValidReportNumber * 6;
                                break;
                            case FlowRecord.NRX:
                                nDataNumberBoundary = m_nNoiseReportNumber * 4;
                                nDataValidNumberBoundary = m_nNoiseValidReportNumber * 4;
                                break;
                            case FlowRecord.NTX:
                                nDataNumberBoundary = m_nNoiseReportNumber * 2;
                                nDataValidNumberBoundary = m_nNoiseValidReportNumber * 2;
                                break;
                            default:
                                break;
                        }
                    }

                    nCostTimeOut = m_nNoiseWaitTime;
                }

                if (bClientOrGoDrawModeFlag == true)
                {
                    Thread tRobotMovingDetect = new Thread(() =>
                    {
                        bRobotEndStopFlag = false;

                        while (!bGetDataFinishFlag)
                        {
                            Thread.Sleep(500);
                            m_cRobot.SetSocketRobot(RobotAPI.RobotCode.IsMoving, ref sReadCommand);
                            sEchoCommand = sReadCommand;
                            m_cSocket.RunClientSending(ref sEchoCommand);
                        }

                        bRobotEndStopFlag = true;
                    });

                    tRobotMovingDetect.IsBackground = true;
                    tRobotMovingDetect.Start();
                }

                SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);

                if (eMainStep == MainTuningStep.PRESSURETUNING)
                {
                    while (nCostTime < nCostTimeOut)
                    {
                        lCurrentTime = DateTime.Now.Ticks;
                        nCostTime = (int)((lCurrentTime - lStartTime) / 10000);
                        Thread.Sleep(500);

                        if (m_cfrmMain.m_bInterruptFlag == false)
                        {
                            int nDataNumber = m_byteReportData_List.Count;
                            m_dTimeCounter = Math.Round((double)nCostTime / 1000, 3, MidpointRounding.AwayFromZero);

                            OutputMessage(string.Format("-Time : {0:0.000} s, Report : {1}", m_dTimeCounter, nDataNumber.ToString()));

                            SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);
                        }
                    }
                }
                else
                {
                    while ((nCostTime < nCostTimeOut) && m_byteReportData_List.Count < nDataNumberBoundary)
                    {
                        lCurrentTime = DateTime.Now.Ticks;
                        nCostTime = (int)((lCurrentTime - lStartTime) / 10000);
                        Thread.Sleep(500);

                        int nDataNumber = m_byteReportData_List.Count;
                        m_dTimeCounter = Math.Round((double)nCostTime / 1000, 3, MidpointRounding.AwayFromZero);

                        if (nCostTime > nNoReportCheckStartTime)
                        {
                            if (nDataNumber == nPreviousDataNumber)
                            {
                                if (nDataNumber == 0)
                                {
                                    if (bNoReportFlag == false)
                                        lNoReportStartTime = lCurrentTime;
                                }

                                if (bNoReportFlag == false)
                                    bNoReportFlag = true;
                            }
                            else
                            {
                                bNoReportFlag = false;
                                lNoReportStartTime = lCurrentTime;
                            }

                            if (m_cfrmMain.m_bInterruptFlag == false)
                            {
                                //Compute Report Rate
                                if (ParamAutoTuning.m_nNoiseDisplayReportRate == 1)
                                {
                                    int nIntervalCostTime = (int)((lCurrentTime - lPreviousTime) / 10000);
                                    double dIntervalTimeCounter = Math.Round((double)nIntervalCostTime / 1000, 3, MidpointRounding.AwayFromZero);
                                    int nDifferDataNumber = nDataNumber - nPreviousDataNumber;

                                    if (nDifferDataNumber > 0)
                                    {
                                        dReportRate = Math.Round(nDifferDataNumber / dIntervalTimeCounter, 2, MidpointRounding.AwayFromZero);
                                        dReportRate_List.Add(dReportRate);
                                    }

                                    OutputMessage(string.Format("-Time : {0:0.000} s, Report : {1}, ReportRate : {2}", m_dTimeCounter, nDataNumber.ToString(), dReportRate.ToString()));

                                    lPreviousTime = lCurrentTime;
                                }
                                else
                                    OutputMessage(string.Format("-Time : {0:0.000} s, Report : {1}", m_dTimeCounter, nDataNumber.ToString()));

                                SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);
                            }

                            nPreviousDataNumber = nDataNumber;

                            if (bNoReportFlag == true)
                            {
                                int nNoReportCostTime = (int)((lCurrentTime - lNoReportStartTime) / 10000);
                                double dNoReportTimeCounter = Math.Round((double)nNoReportCostTime / 1000, 3, MidpointRounding.AwayFromZero);

                                if (dNoReportTimeCounter >= ParamAutoTuning.m_dNoiseNoReportInterruptTime)
                                    break;
                            }
                        }
                        else
                        {
                            if (m_cfrmMain.m_bInterruptFlag == false)
                            {
                                //Compute Report Rate
                                if (ParamAutoTuning.m_nNoiseDisplayReportRate == 1)
                                    OutputMessage(string.Format("-Time : {0:0.000} s, Report : {1}, ReportRate : {2}", m_dTimeCounter, nDataNumber.ToString(), dReportRate.ToString()));
                                else
                                    OutputMessage(string.Format("-Time : {0:0.000} s, Report : {1}", m_dTimeCounter, nDataNumber.ToString()));


                                SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);
                            }
                        }
                    }

                    //Compute Report Rate
                    if (ParamAutoTuning.m_nNoiseDisplayReportRate == 1)
                    {
                        dReportRate_List.RemoveAt(0);
                        dReportRate_List.RemoveAt(dReportRate_List.Count - 1);
                        dAverageReportRate = Math.Round(dReportRate_List.Average(), 2, MidpointRounding.AwayFromZero);
                        OutputMessage(string.Format("-Average Report Rate : {0:0.00}", dAverageReportRate));
                    }
                }

                bGetDataFinishFlag = true;

                if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT && m_cSocket.m_bServerDisconnectFlag == true)
                    return;

                if (m_byteReportData_List.Count <= 0)
                {
                    m_sErrorMessage = "No Report Data";

                    if (eMainStep == MainTuningStep.PRESSURETUNING)
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
                else if (m_byteReportData_List.Count < nDataValidNumberBoundary)
                {
                    m_nRecordDataErrorFlag = 1;
                    m_sErrorMessage = "No Enough Report Data";

                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                    {
                        string sErrorCode = string.Format("{0}.002", StringConvert.m_dictSubStepCNMappingTable[eSubStep]);
                        SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorCode, sErrorCode, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                        SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorMsg, m_sErrorMessage, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                        SetRecordFlowInfo(eSubStep, cRobotParameter.m_nFlowIndex, SpecificText.m_sErrorMessage, m_sErrorMessage, SubTuningStep.ELSE, m_nRECORDVALUE_ERRORINFORMATION);
                    }
                }
            }

            while (!bRobotEndStopFlag)
                Thread.Sleep(100);
        }
        #endregion

        #region Wait Draw Function
        /// <summary>
        /// 確認是否畫線完成
        /// </summary>
        /// <param name="cRobotParameter">線測機相關資訊Class</param>
        private void WaitDrawFinish(RobotParameter cRobotParameter)
        {
            MainTuningStep eMainStep = cRobotParameter.m_cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cRobotParameter.m_cFlowStep.m_eSubStep;

            long lStartTime = DateTime.Now.Ticks;
            long lCurrentTime = 0;
            int nCostTime = 0;

            SetPatternTimeAndReportNumber(cRobotParameter, m_dTimeCounter, m_byteReportData_List.Count);

            while (!m_bDrawFinishFlag && nCostTime < m_nDrawLineWaitTime)
            {
                lCurrentTime = DateTime.Now.Ticks;
                nCostTime = (int)((lCurrentTime - lStartTime) / 10000);
                Thread.Sleep(500);

                if (m_cfrmMain.m_bInterruptFlag == false)
                {
                    int nDataNumber = m_byteReportData_List.Count;
                    m_dTimeCounter = Math.Round((double)nCostTime / 1000, 3, MidpointRounding.AwayFromZero);

                    OutputMessage(string.Format("-Time : {0:0.000} s, Report : {1}", m_dTimeCounter, nDataNumber.ToString()));

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
        }
        #endregion
    }
}
