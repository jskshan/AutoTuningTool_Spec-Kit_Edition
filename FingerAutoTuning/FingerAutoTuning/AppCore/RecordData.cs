using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <returns></returns>
        private bool RecordData(frmMain.FlowStep cFlowStep)
        {
            OutputMessage("[State]Record Main Flow");

            bool bFlowComplete = true;

            if (RegistHIDDevice(cFlowStep) == false)
            {
                bFlowComplete = false;
                return bFlowComplete;
            }

            CreateLogDirectory(cFlowStep);

            int nTotalCount = 0;

            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                nTotalCount = m_cRawADCSweepItem_List.Count;
            else
                nTotalCount = m_cFreqencyItem_List.Count;

            m_nTotalCount = nTotalCount;
            int nExecuteIndex = m_cfrmParent.m_nCurrentExecuteIndex;
            string sMessage = "";

            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                sMessage = string.Format("Parameter Set Count : {0}", nTotalCount);
            else
                sMessage = string.Format("Frequency Set Count : {0}", nTotalCount);

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                OutputMessage(string.Format("-{0}", sMessage));
                m_cfrmParent.InitialstatusstripMessage(nTotalCount, sMessage);
            });

            SetPenFunctionEnable_8F18(false);

            SetReportEnable(false);

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data);

            if (SetDCDCEnable_8F18() == false)
            {
                bFlowComplete = false;
                return bFlowComplete;
            }

            if (GetTraceNumber() == false)
            {
                bFlowComplete = false;
                return bFlowComplete;
            }

            if (ExecuteGetTPInformation(cFlowStep) == false)
            {
                bFlowComplete = false;
                return bFlowComplete;
            }

            SetTraceInformation(cFlowStep, nExecuteIndex);

            //SetPenFunctionEnable_8F18(false);

            //Get And Set Project_Option, FWIP_Option
            short nOriginProjectOption = 0;
            short nOriginFWIPOption = 0;
            short nReadFWIPOption = 0;

            if (GetAndSetOptions(ref nOriginProjectOption, ref nOriginFWIPOption, ref nReadFWIPOption) == false)
            {
                bFlowComplete = false;
                return bFlowComplete;
            }

            if (cFlowStep.m_eStep != MainStep.Self_FrequencySweep && cFlowStep.m_eStep != MainStep.Self_NCPNCNSweep)
            {
                if (GetFWParameter(cFlowStep, true) == false)
                {
                    bFlowComplete = false;
                    return bFlowComplete;
                }
            }

            AppCoreDefine.RecordState eRecordState = m_cfrmParent.m_eRecordState;
            bool bGetFirstDataFlag = true;
            m_bGetFirstDataFlag = true;

            while (m_cfrmParent.m_bExecute)
            {
                string sFlowState = "";
                m_cSendCommandInfo = null;

                // Load User Defined Command Script
                if (LoadUserDefinedCommandScript(cFlowStep, nExecuteIndex) == false)
                {
                    bFlowComplete = false;
                    return bFlowComplete;
                }

                if (OutputStepState(ref sFlowState, ref nExecuteIndex, ref eRecordState, cFlowStep, nTotalCount) == false)
                {
                    if (nExecuteIndex == nTotalCount)
                        break;
                }

                GetDataList(cFlowStep, eRecordState);

                DeleteUnuseDataFile(cFlowStep, nExecuteIndex, false);

                SetStepName(cFlowStep, sFlowState, nExecuteIndex, false);

                m_nCurrentExecuteIndex = nExecuteIndex;

                /*
                if (ParamFingerAutoTuning.m_nUseSSHSocketServer == 1 && bGetFirstDataFlag == false)
                {
                    SetReset(false);
                    Thread.Sleep(3000);

                    SetReportEnable(false);

                    if (GetAndSetOptions(ref nOriginProjectOption, ref nOriginFWIPOption, ref nReadFWIPOption) == false)
                        break;
                }
                */

                if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                {
                    RawADCSweepItem cRawADCSweepItem = m_cRawADCSweepItem_List[nExecuteIndex];

                    SetStepMessage(cFlowStep, cRawADCSweepItem, sFlowState, nExecuteIndex, nTotalCount);

                    /*
                    if (CheckDataFile(cFlowStep, null, nExecuteIndex, m_nTXTraceNumber, m_nRXTraceNumber, cRawADCSweepItem) == false)
                    {
                        bFlowComplete = false;
                        return bFlowComplete;
                    }

                    if (m_sGetData_List.Count == 0)
                    {
                        nExecuteIndex++;
                        continue;
                    }
                    */

                    if (CheckTraceNumberMatch(cFlowStep, null, nReadFWIPOption, nExecuteIndex, cRawADCSweepItem) == false)
                    {
                        bFlowComplete = false;
                        return bFlowComplete;
                    }

                    if (SetAndGetFWParameter(cFlowStep, null, cRawADCSweepItem) == false)
                    {
                        bFlowComplete = false;
                        break;
                    }

                    int nBrightness = SetScreenState(nExecuteIndex, nTotalCount);

                    if (GetDataMainFlow(cFlowStep, nBrightness, null, m_nRXTraceNumber, m_nTXTraceNumber, nExecuteIndex, nTotalCount, eRecordState, sFlowState, bGetFirstDataFlag: bGetFirstDataFlag, cRawADCSweepItem: cRawADCSweepItem) == false)
                    {
                        bFlowComplete = false;
                        break;
                    }

                    RunScreenResetFlow(nBrightness, false);
                }
                else
                {
                    FrequencyItem cFrequencyItem = m_cFreqencyItem_List[nExecuteIndex];

                    SetStepMessage(cFlowStep, cFrequencyItem, sFlowState, nExecuteIndex, nTotalCount);

                    if (cFlowStep.m_eStep != MainStep.Self_FrequencySweep && cFlowStep.m_eStep != MainStep.Self_NCPNCNSweep)
                    {
                        if (GetAndCheckOriginalBASEPath(cFlowStep, cFrequencyItem, nExecuteIndex) == false)
                        {
                            bFlowComplete = false;
                            return bFlowComplete;
                        }

                        /*
                        if (CheckDataFile(cFlowStep, cFrequencyItem, nExecuteIndex, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                        {
                            bFlowComplete = false;
                            return bFlowComplete;
                        }
                        
                        if (m_sGetData_List.Count == 0)
                        {
                            nExecuteIndex++;
                            continue;
                        }
                        */

                        if (CheckTraceNumberMatch(cFlowStep, cFrequencyItem, nReadFWIPOption, nExecuteIndex) == false)
                        {
                            bFlowComplete = false;
                            return bFlowComplete;
                        }
                    }

                    if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep)
                    {
                        RunSelfFrequencySweepFlow(ref bFlowComplete, cFlowStep, cFrequencyItem, sFlowState, nExecuteIndex, nTotalCount, eRecordState, bGetFirstDataFlag);

                        if (bFlowComplete == false)
                            break;
                    }
                    else if (cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                    {
                        RunSelfNCPNCNSweepFlow(ref bFlowComplete, cFlowStep, cFrequencyItem, sFlowState, nExecuteIndex, nTotalCount, eRecordState, bGetFirstDataFlag);

                        if (bFlowComplete == false)
                            break;
                    }
                    else
                    {
                        //1.Set, Read & Check PH1/PH2/Sum
                        if (SetAndGetFWParameter(cFlowStep, cFrequencyItem) == false)
                        {
                            bFlowComplete = false;
                            break;
                        }

                        int nBrightness = SetScreenState(nExecuteIndex, nTotalCount);

                        if (GetDataMainFlow(cFlowStep, nBrightness, cFrequencyItem, m_nRXTraceNumber, m_nTXTraceNumber, nExecuteIndex, nTotalCount, eRecordState, sFlowState, bGetFirstDataFlag: bGetFirstDataFlag) == false)
                        {
                            bFlowComplete = false;
                            break;
                        }

                        RunScreenResetFlow(nBrightness, false);
                    }
                }

                bGetFirstDataFlag = false;
                m_bGetFirstDataFlag = false;
                nExecuteIndex++;
                m_cfrmParent.m_nCurrentExecuteIndex = nExecuteIndex;
            }

            SetRecoveryOptions(cFlowStep, nOriginProjectOption, nOriginFWIPOption);

            return bFlowComplete;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bFlowComplete"></param>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="sFlowState"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="nTotalCount"></param>
        /// <param name="eRecordState"></param>
        /// <param name="bGetFirstDataFlag"></param>
        private void RunSelfFrequencySweepFlow(
            ref bool bFlowComplete, frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem, string sFlowState, int nExecuteIndex,
            int nTotalCount, AppCoreDefine.RecordState eRecordState, bool bGetFirstDataFlag)
        {
            bool bGetFWParameter = false;

            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                m_eSelfTraceType = eTraceType;

                SetTraceInformation(cFlowStep, nExecuteIndex);

                if (ParamFingerAutoTuning.m_nSelfFSRunKSequence == 1 ||
                    ParamFingerAutoTuning.m_nSelfFSRunKSequence == 2)
                {
                    List<SelfKParameter> cSelfKParameter_List = new List<SelfKParameter>();
                    int nNCPLB = ParamFingerAutoTuning.m_nSelfFSNCPValueLB;
                    int nNCPHB = ParamFingerAutoTuning.m_nSelfFSNCPValueHB;
                    int nNCNLB = ParamFingerAutoTuning.m_nSelfFSNCNValueLB;
                    int nNCNHB = ParamFingerAutoTuning.m_nSelfFSNCNValueHB;

                    //Set NCP/NCN and NCP=NCN
                    if (ParamFingerAutoTuning.m_nSelfFSRunKSequence == 1)
                    {
                        for (int nNCPIndex = nNCPLB; nNCPIndex <= nNCPHB; nNCPIndex++)
                        {
                            SelfKParameter cSelfKParameter = new SelfKParameter();
                            cSelfKParameter.m_nNCPValue = nNCPIndex;
                            cSelfKParameter.m_nNCNValue = nNCPIndex;

                            cSelfKParameter_List.Add(cSelfKParameter);
                        }
                    }
                    //Set NCP/NCN and NCP!=NCN
                    else if (ParamFingerAutoTuning.m_nSelfFSRunKSequence == 2)
                    {
                        for (int nNCPIndex = nNCPLB; nNCPIndex <= nNCPHB; nNCPIndex++)
                        {
                            for (int nNCNIndex = nNCNLB; nNCNIndex <= nNCNHB; nNCNIndex++)
                            {
                                SelfKParameter cSelfKParameter = new SelfKParameter();
                                cSelfKParameter.m_nNCPValue = nNCPIndex;
                                cSelfKParameter.m_nNCNValue = nNCNIndex;

                                cSelfKParameter_List.Add(cSelfKParameter);
                            }
                        }
                    }

                    foreach (SelfKParameter cSelfKParameter in cSelfKParameter_List)
                    {
                        m_nSelfNCPValue = cSelfKParameter.m_nNCPValue;
                        m_nSelfNCNValue = cSelfKParameter.m_nNCNValue;

                        DeleteUnuseDataFile(cFlowStep, nExecuteIndex, true);

                        /*
                        if (CheckDataFile(cFlowStep, cFrequencyItem, nExecuteIndex, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                        {
                            bFlowComplete = false;
                            return;
                        }

                        if (m_sGetData_List.Count == 0)
                            continue;
                        */

                        if (bGetFWParameter == false)
                        {
                            if (GetFWParameter(cFlowStep, true) == false)
                            {
                                bFlowComplete = false;
                                return;
                            }

                            bGetFWParameter = true;
                        }

                        //1.Set, Read & Check PH1/PH2/Sum
                        if (SetAndGetFWParameter(cFlowStep, cFrequencyItem) == false)
                        {
                            bFlowComplete = false;
                            break;
                        }

                        SetStepName(cFlowStep, sFlowState, nExecuteIndex, true, true);

                        int nBrightness = SetScreenState(nExecuteIndex, nTotalCount);

                        if (GetDataMainFlow(cFlowStep, nBrightness, cFrequencyItem, m_nRXTraceNumber, m_nTXTraceNumber, nExecuteIndex, nTotalCount, eRecordState, sFlowState,
                                            bGetFirstDataFlag: bGetFirstDataFlag) == false)
                        {
                            bFlowComplete = false;
                            break;
                        }

                        RunScreenResetFlow(nBrightness, false);
                    }
                }
                else
                {
                    DeleteUnuseDataFile(cFlowStep, nExecuteIndex, true);

                    /*
                    if (CheckDataFile(cFlowStep, cFrequencyItem, nExecuteIndex, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                    {
                        bFlowComplete = false;
                        return;
                    }

                    if (m_sGetData_List.Count == 0)
                        continue;
                    */

                    if (bGetFWParameter == false)
                    {
                        if (GetFWParameter(cFlowStep, true) == false)
                        {
                            bFlowComplete = false;
                            return;
                        }

                        bGetFWParameter = true;
                    }

                    //1.Set, Read & Check PH1/PH2/Sum
                    if (SetAndGetFWParameter(cFlowStep, cFrequencyItem) == false)
                    {
                        bFlowComplete = false;
                        break;
                    }

                    SetStepName(cFlowStep, sFlowState, nExecuteIndex, true);

                    int nBrightness = SetScreenState(nExecuteIndex, nTotalCount);

                    if (GetDataMainFlow(cFlowStep, nBrightness, cFrequencyItem, m_nRXTraceNumber, m_nTXTraceNumber, nExecuteIndex, nTotalCount, eRecordState, sFlowState,
                                        bGetFirstDataFlag: bGetFirstDataFlag) == false)
                    {
                        bFlowComplete = false;
                        break;
                    }

                    RunScreenResetFlow(nBrightness, false);
                }
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bFlowComplete"></param>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="sFlowState"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="nTotalCount"></param>
        /// <param name="eRecordState"></param>
        /// <param name="bGetFirstDataFlag"></param>
        private void RunSelfNCPNCNSweepFlow(
            ref bool bFlowComplete, frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem, string sFlowState, int nExecuteIndex,
            int nTotalCount, AppCoreDefine.RecordState eRecordState, bool bGetFirstDataFlag)
        {
            bool bGetFWParameter = false;

            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                m_eSelfTraceType = eTraceType;

                SetTraceInformation(cFlowStep, nExecuteIndex);

                m_nSelfNCPValue = cFrequencyItem.m_nSelf_NCP;
                m_nSelfNCNValue = cFrequencyItem.m_nSelf_NCN;

                DeleteUnuseDataFile(cFlowStep, nExecuteIndex, true);

                /*
                if (CheckDataFile(cFlowStep, cFrequencyItem, nExecuteIndex, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                {
                    bFlowComplete = false;
                    return;
                }

                if (m_sGetData_List.Count == 0)
                    continue;
                */

                if (bGetFWParameter == false)
                {
                    if (GetFWParameter(cFlowStep, true) == false)
                    {
                        bFlowComplete = false;
                        return;
                    }

                    bGetFWParameter = true;
                }

                //1.Set, Read & Check PH1/PH2/Sum
                if (SetAndGetFWParameter(cFlowStep, cFrequencyItem) == false)
                {
                    bFlowComplete = false;
                    break;
                }

                SetStepName(cFlowStep, sFlowState, nExecuteIndex, true, true);

                int nBrightness = SetScreenState(nExecuteIndex, nTotalCount);

                if (GetDataMainFlow(cFlowStep, nBrightness, cFrequencyItem, m_nRXTraceNumber, m_nTXTraceNumber, nExecuteIndex, nTotalCount, eRecordState, sFlowState,
                                    bGetFirstDataFlag: bGetFirstDataFlag) == false)
                {
                    bFlowComplete = false;
                    break;
                }

                RunScreenResetFlow(nBrightness, false);
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        private void CreateLogDirectory(frmMain.FlowStep cFlowStep)
        {
            string sStepCodeName = StringConvert.m_dictMainStepCodeNameMappingTable[cFlowStep.m_eStep];
            m_sLogDirectoryPath = string.Format(@"{0}\{1}", m_cfrmParent.m_sRecordLogDirectoryPath, sStepCodeName);
            m_sH5LogDirectoryPath = string.Format(@"{0}\{1}", m_cfrmParent.m_sH5RecordLogDirectoryPath, sStepCodeName);

            if (Directory.Exists(m_sLogDirectoryPath) == false)
                Directory.CreateDirectory(m_sLogDirectoryPath);

            if (m_bGenerateH5Data == true)
            {
                if (Directory.Exists(m_sH5LogDirectoryPath) == false)
                    Directory.CreateDirectory(m_sH5LogDirectoryPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool GetTraceNumber()
        {
            if (m_eICGenerationType != ICGenerationType.Gen9)
            {
                //Check the RX Trace number
                int nRXTraceNumber = 0;

                if (GetRXTXTraceNumber(ref nRXTraceNumber, TraceType.RX) == false)
                    return false;

                m_nRXTraceNumber = nRXTraceNumber;

                //Check the TX Trace number
                int nTXTraceNumber = 0;

                if (GetRXTXTraceNumber(ref nTXTraceNumber, TraceType.TX) == false)
                    return false;

                m_nTXTraceNumber = nTXTraceNumber;
            }
#if _USE_9F07_SOCKET
            else
            {
                #region Get the trace information
                //目前暫時取得Trace Information的作法,以後可能需要跟main flow溝通
                ElanDirectTochMainFlow.Initialize();
                bool bResultFlag = GetTraceInfo();
                ElanDirectTochMainFlow.Uninitialize();
                #endregion

                if (bResultFlag == false)
                {
                    m_sErrorMessage = "Get RX/TX Trace Number Error";
                    return false;
                }
            }
#endif

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <returns></returns>
        private bool ExecuteGetTPInformation(frmMain.FlowStep cFlowStep)
        {
#if _USE_9F07_SOCKET
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return true;
#endif

            if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
            {
                OutputMessage("-Get TP Information");

                bool bGetTPInfoFlag = GetTPInformation(cFlowStep);

                if (bGetTPInfoFlag == false)
                {
                    m_sErrorMessage = "Get TP Information Error";
                    return false;
                }
            }
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
            {
                OutputMessage("-Get TP Information");

                bool bGetTPInfoFlag = GetTPInformation(cFlowStep);

                if (bGetTPInfoFlag == false)
                {
                    m_sErrorMessage = "Get TP Information Error";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFlowState"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="eRecordState"></param>
        /// <param name="cFlowStep"></param>
        /// <param name="nTotalCount"></param>
        /// <returns></returns>
        private bool OutputStepState(ref string sFlowState, ref int nExecuteIndex, ref AppCoreDefine.RecordState eRecordState, frmMain.FlowStep cFlowStep, int nTotalCount)
        {
            if (nExecuteIndex == 0 && cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
            {
                if (eRecordState == AppCoreDefine.RecordState.NORMAL)
                {
                    eRecordState = AppCoreDefine.RecordState.ACFRFIRSTSTAGE;
                    m_cfrmParent.m_eRecordState = eRecordState;
                    sFlowState = AppCoreDefine.dictRecordStateCodeNameMappingTable[eRecordState];

                    OutputlblStatus(string.Format("Execute({0})", sFlowState), "", Color.Blue, true);
                }
            }

            if (nExecuteIndex == nTotalCount)
            {
                if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                {
                    if (eRecordState == AppCoreDefine.RecordState.ACFRFIRSTSTAGE)
                    {
                        eRecordState = AppCoreDefine.RecordState.ACFRSECONDSTAGE;
                        m_cfrmParent.m_eRecordState = eRecordState;
                        sFlowState = AppCoreDefine.dictRecordStateCodeNameMappingTable[eRecordState];
                    }
                    else
                        return false;

                    OutputlblStatus(string.Format("Execute({0})", sFlowState), "", Color.Blue, true);

                    if (eRecordState == AppCoreDefine.RecordState.ACFRSECONDSTAGE)
                    {
                        nExecuteIndex = 0;
                        m_cfrmParent.m_nCurrentExecuteIndex = 0;
                        ShowWarningMessage("Please Put the Fixture on the Screen. And Then Don't Move!");
                    }
                }
                else
                    return false;
            }

            if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
            {
                sFlowState = AppCoreDefine.dictRecordStateCodeNameMappingTable[eRecordState];
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="bSelfFlow"></param>
        private void DeleteUnuseDataFile(frmMain.FlowStep cFlowStep, int nExecuteIndex, bool bSelfFlow)
        {
            if (m_bCreateNewFolder == false)
                return;

            if (bSelfFlow == false)
            {
                if (nExecuteIndex == 0 &&
                    (cFlowStep.m_eStep != MainStep.Self_FrequencySweep && cFlowStep.m_eStep != MainStep.Self_NCPNCNSweep))
                    DeleteUnuseDataFile(cFlowStep, m_nTXTraceNumber, m_nRXTraceNumber);
            }
            else
            {
                if (nExecuteIndex == 0 &&
                    (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep))
                    DeleteUnuseDataFile(cFlowStep, m_nTXTraceNumber, m_nRXTraceNumber);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nTXTraceNumber"></param>
        /// <param name="nRXTraceNumber"></param>
        /// <returns></returns>
        private bool DeleteUnuseDataFile(frmMain.FlowStep cFlowStep, int nTXTraceNumber, int nRXTraceNumber)
        {
            string sStepCodeName = StringConvert.m_dictMainStepCodeNameMappingTable[cFlowStep.m_eStep];

            foreach (string sDataType in m_sGetData_List)
            {
                string sDataDirectoryPath = string.Format(@"{0}\{1}\{2}", m_cfrmParent.m_sRecordLogDirectoryPath, sStepCodeName, sDataType);

                if (Directory.Exists(sDataDirectoryPath) == false)
                    continue;

                bool bGetMatchFile = false;
                //string[] sExtension_Array = new string[] { ".csv", ".CSV" };

                string sFileType = "*.csv";
                bool bCsvFile = true;

                if (sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE ||
                    sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL)
                {
                    sFileType = "*.txt";
                    bCsvFile = false;
                }

                if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                {
                    foreach (string sFilePath in Directory.EnumerateFiles(sDataDirectoryPath, sFileType, SearchOption.TopDirectoryOnly))
                    {
                        foreach (RawADCSweepItem cRawADCSweepItem in m_cRawADCSweepItem_List)
                        {
                            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                            bGetMatchFile = CheckFileInfo(cRawADCSweepItem, cFlowStep, sDataType, srFile, nTXTraceNumber, nRXTraceNumber, bCsvFile: bCsvFile);

                            if (bGetMatchFile == true)
                                break;
                        }

                        if (bGetMatchFile == true)
                            AppCoreDefine.FileProcessFlow(AppCoreDefine.FileProcess.Delete, "", sFilePath);
                    }
                }
                else
                {
                    //foreach (string sFilePath in Directory.EnumerateFiles(sDataDirectoryPath, "*.", SearchOption.TopDirectoryOnly).Where(s => sExtension_Array.Any(ext => ext == Path.GetExtension(s))))
                    foreach (string sFilePath in Directory.EnumerateFiles(sDataDirectoryPath, sFileType, SearchOption.TopDirectoryOnly))
                    {
                        foreach (FrequencyItem cFreqencyItem in m_cFreqencyItem_List)
                        {
                            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                            bGetMatchFile = CheckFileInfo(cFreqencyItem, cFlowStep, sDataType, srFile, nTXTraceNumber, nRXTraceNumber, bCsvFile: bCsvFile);

                            if (bGetMatchFile == true)
                                break;
                        }

                        if (bGetMatchFile == true)
                            AppCoreDefine.FileProcessFlow(AppCoreDefine.FileProcess.Delete, "", sFilePath);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="sFlowState"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="bSelfFlow"></param>
        /// <param name="bRunKSequence"></param>
        private void SetStepName(frmMain.FlowStep cFlowStep, string sFlowState, int nExecuteIndex, bool bSelfFlow, bool bRunKSequence = false)
        {
            if (bSelfFlow == false)
            {
                if (nExecuteIndex == 0)
                {
                    if (sFlowState != "")
                        m_sStepName = string.Format("{0}({1})", cFlowStep.m_sStepName, sFlowState);
                    else
                        m_sStepName = cFlowStep.m_sStepName;
                }
            }
            else
            {
                if (bRunKSequence == true)
                {
                    if (sFlowState != "")
                        m_sStepName = string.Format("{0}({1})", cFlowStep.m_sStepName, sFlowState);
                    else
                        m_sStepName = cFlowStep.m_sStepName;

                    m_sStepName = string.Format("{0}({1})(NCP={2} NCN={3})", m_sStepName, m_eSelfTraceType.ToString(), m_nSelfNCPValue, m_nSelfNCNValue);
                }
                else
                {
                    if (sFlowState != "")
                        m_sStepName = string.Format("{0}({1})", cFlowStep.m_sStepName, sFlowState);
                    else
                        m_sStepName = cFlowStep.m_sStepName;

                    m_sStepName = string.Format("{0}({1})", m_sStepName, m_eSelfTraceType.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="sFlowState"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="nTotalCount"></param>
        private void SetStepMessage(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem, string sFlowState, int nExecuteIndex, int nTotalCount)
        {
            string sMessage = "";

            string sFrequencySet = "Frequency Set";

            if (sFlowState != "")
                sFrequencySet = string.Format("{0} Frequency Set", sFlowState);

            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                m_cSelfParameter.SetRelatedData(cFlowStep.m_eStep, cFrequencyItem);
                m_cSelfParameter.SetDFT_NUM();
                m_cSelfParameter.SetGain();
                m_cSelfParameter.SetCAL();
                m_cSelfParameter.SetCAG();
                m_cSelfParameter.SetIQ_BSH();

                sMessage = string.Format("-{0} : {1} Start Record[_SELF_PH1=0x{2}, _SELF_PH2E_LMT=0x{3}, _SELF_PH2_LAT=0x{4}, _SELF_PH2=0x{5}, " +
                                         "_SELF_DFT_NUM={6}, _SELF_SELGM={7}, _SELF_CAG={8}, _SELF_IQ_BSH={9}, Frequency={10}KHz]",
                                         sFrequencySet, nExecuteIndex + 1,
                                         cFrequencyItem.m_n_SELF_PH1.ToString("x2").ToUpper(), cFrequencyItem.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper(),
                                         cFrequencyItem.m_n_SELF_PH2_LAT.ToString("x2").ToUpper(), cFrequencyItem.m_n_SELF_PH2.ToString("x2").ToUpper(),
                                         m_cSelfParameter.m_nDFT_NUM.ToString("D"), m_cSelfParameter.m_nGain.ToString("D"),
                                         m_cSelfParameter.m_nCAG.ToString("D"), m_cSelfParameter.m_nIQ_BSH.ToString("D"),
                                         cFrequencyItem.m_dSelf_Frequency.ToString("0.000"));
            }
            else
            {
                m_cFreqencyItem_List[nExecuteIndex].m_nDFT_NUM = ComputeSuggestDFT_NUM(m_cFreqencyItem_List[nExecuteIndex], m_nTXTraceNumber);

                if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                {
                    sMessage = string.Format("-{0} : {1} Start Record[PH1=0x{2}, PH2=0x{3}, DFT_NUM=0x{4}, Frequency={5}KHz]",
                                             sFrequencySet, nExecuteIndex + 1,
                                             cFrequencyItem.m_nPH1.ToString("x2").ToUpper(), cFrequencyItem.m_nPH2.ToString("x2").ToUpper(),
                                             cFrequencyItem.m_nDFT_NUM.ToString("x2").ToUpper(), cFrequencyItem.m_dFrequency.ToString("0.000"));
                }
                else
                {
                    sMessage = string.Format("-{0} : {1} Start Record[PH1=0x{2}, PH2=0x{3}, Frequency={4}KHz]",
                                             sFrequencySet, nExecuteIndex + 1,
                                             cFrequencyItem.m_nPH1.ToString("x2").ToUpper(), cFrequencyItem.m_nPH2.ToString("x2").ToUpper(),
                                             cFrequencyItem.m_dFrequency.ToString("0.000"));
                }
            }

            string sProgressMessage = string.Format("{0} : {1}/{2}", sFrequencySet, nExecuteIndex + 1, nTotalCount);

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                OutputMessage(sMessage);
                m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, nExecuteIndex + 1, sProgressMessage);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cRawADCSweepItem"></param>
        /// <param name="sFlowState"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="nTotalCount"></param>
        private void SetStepMessage(frmMain.FlowStep cFlowStep, RawADCSweepItem cRawADCSweepItem, string sFlowState, int nExecuteIndex, int nTotalCount)
        {
            string sMessage = "";
            string sProgressMessage = "";

            string sParameterSet = "Parameter Set";

            if (sFlowState != "")
                sParameterSet = string.Format("{0} Parameter Set");

            sMessage = string.Format("-{0} : {1} Start Record[SELC={2}, VSEL={3}, LG={4}, SELGM={5}]",
                                     sParameterSet, nExecuteIndex + 1, cRawADCSweepItem.m_nSELC, cRawADCSweepItem.m_nVSEL, cRawADCSweepItem.m_nLG, cRawADCSweepItem.m_nSELGM);

            sProgressMessage = string.Format("{0} : {1}/{2}", sParameterSet, nExecuteIndex + 1, nTotalCount);

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                OutputMessage(sMessage);
                m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, nExecuteIndex + 1, sProgressMessage);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="nReadFWIPOption"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="cRawADCSweepItem"></param>
        /// <returns></returns>
        private bool CheckTraceNumberMatch(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem, int nReadFWIPOption, int nExecuteIndex, RawADCSweepItem cRawADCSweepItem = null)
        {
            if (nExecuteIndex == 0)
            {
                if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                {
                }
                else
                {
                    if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank && ParamFingerAutoTuning.m_nACFRModeType != 1)
                    {
                        if (m_nRXTraceNumber != cFrequencyItem.m_nRXTraceNumber)
                        {
                            m_sErrorMessage = "RX Trace Not Match With PreStep";
                            return false;
                        }

                        if (m_nTXTraceNumber != cFrequencyItem.m_nTXTraceNumber)
                        {
                            m_sErrorMessage = "TX Trace Not Match With PreStep";
                            return false;
                        }
                    }

                    if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2 ||
                        cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                    {
                        int nTXnValue = nReadFWIPOption & 0x0100;

                        if (nTXnValue > 0 && cFrequencyItem.m_bEnableTXn == false)
                        {
                            m_sErrorMessage = "Txn_Scan Setting Not Match(Current:Txn_Scan Enable)";
                            return false;
                        }
                        else if (nTXnValue == 0 && cFrequencyItem.m_bEnableTXn == true)
                        {
                            m_sErrorMessage = "Txn_Scan Setting Not Match(Current:Txn_Scan Disable)";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nExecuteIndex"></param>
        /// <param name="nTotalCount"></param>
        /// <returns></returns>
        private int SetScreenState(int nExecuteIndex, int nTotalCount)
        {
            int nBrightness = SetBrightness();

            DisplayPattern(m_sStepName, nExecuteIndex, nTotalCount);

            DisableMonitor();

            return nBrightness;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="eRecordState"></param>
        private void GetDataList(frmMain.FlowStep cFlowStep, AppCoreDefine.RecordState eRecordState)
        {
            m_sGetData_List = new List<string>();

            if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2 || cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
            {
                if (eRecordState == AppCoreDefine.RecordState.ACFRFIRSTSTAGE)
                    m_sGetData_List.Add(MainConstantParameter.m_sDATATYPE_OBASE);
                else
                    m_sGetData_List.Add(MainConstantParameter.m_sDATATYPE_BASE);
            }

            if (eRecordState == AppCoreDefine.RecordState.ACFRFIRSTSTAGE)
                return;

            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep)
            {
                if (ParamFingerAutoTuning.m_nSelfFSGetDataType == 1)
                {
                    m_sGetData_List.Add(MainConstantParameter.m_sDATATYPE_REPORTMODE);

                    if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1)
                        m_sGetData_List.Add(MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL);
                }
                else
                    m_sGetData_List.Add(MainConstantParameter.m_sDATATYPE_DV);
            }
            else if (cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                m_sGetData_List.Add(MainConstantParameter.m_sDATATYPE_RawData);
            else
            {
                m_sGetData_List.Add(MainConstantParameter.m_sDATATYPE_ADC);

                if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2 || cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                    m_sGetData_List.Add(MainConstantParameter.m_sDATATYPE_BASEMinusADC);

                if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank && eRecordState == AppCoreDefine.RecordState.ACFRSECONDSTAGE)
                    m_sGetData_List.Add(MainConstantParameter.m_sDATATYPE_OBASEMinusADC);

                if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2 && ParamFingerAutoTuning.m_nFRPH2DataType == 1)
                    m_sGetData_List.Add(MainConstantParameter.m_sDATATYPE_DV);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="nExecuteIndex"></param>
        /// <returns></returns>
        private bool GetAndCheckOriginalBASEPath(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem, int nExecuteIndex)
        {
            if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank && m_cfrmParent.m_eRecordState == AppCoreDefine.RecordState.ACFRSECONDSTAGE)
            {
                if (m_cFreqencyItem_List[nExecuteIndex].m_sOBASEFilePath != null)
                    return true;

                string sStepCodeName = StringConvert.m_dictMainStepCodeNameMappingTable[cFlowStep.m_eStep];
                string sDataDirectoryPath = string.Format(@"{0}\{1}\{2}", m_cfrmParent.m_sRecordLogDirectoryPath, sStepCodeName, MainConstantParameter.m_sDATATYPE_OBASE);

                if (Directory.Exists(sDataDirectoryPath) == false)
                {
                    m_sErrorMessage = string.Format("OBASE Data Folder Not Exist");
                    return false;
                }

                double dFrequency = cFrequencyItem.m_dFrequency;
                string sFileName = string.Format("{0}_{1}_{2}_{3}.csv", MainConstantParameter.m_sDATATYPE_OBASE, dFrequency.ToString("0.000"), cFrequencyItem.m_nPH1.ToString("X2"), cFrequencyItem.m_nPH2.ToString("X2"));
                string sFilePath = string.Format(@"{0}\{1}", sDataDirectoryPath, sFileName);

                if (File.Exists(sFilePath) == false)
                {
                    m_sErrorMessage = string.Format("OBASE Data File Not Exist in Frequency Set : {0}(FileName:{1})", nExecuteIndex + 1, sFileName);
                    return false;
                }

                m_cFreqencyItem_List[nExecuteIndex].m_sOBASEFilePath = sFilePath;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="nTXTraceNumber"></param>
        /// <param name="nRXTraceNumber"></param>
        /// <param name="cRawADCSweepItem"></param>
        /// <returns></returns>
        /*
        private bool CheckDataFile(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem, int nExecuteIndex, int nTXTraceNumber, int nRXTraceNumber, RawADCSweepItem cRawADCSweepItem = null)
        {
            List<string> sRemoveData_List = new List<string>();

            string sStepCodeName = StringConvert.m_dictMainStepCodeNameMappingTable[cFlowStep.m_eStep];

            foreach (string sDataType in m_sGetData_List)
            {
                bool bAllFileExist = false;

                string sDataDirectoryPath = string.Format(@"{0}\{1}\{2}", m_cfrmParent.m_sRecordLogDirectoryPath, sStepCodeName, sDataType);

                if (Directory.Exists(sDataDirectoryPath) == false)
                    continue;

                bool bGetMatchFile = false;

                string sCsvMatchFileName = "";
                //string[] sExtension_List = new string[] { ".csv", ".CSV" };

                string sFileType = "*.csv";
                bool bCsvFile = true;

                if (sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE ||
                    sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL)
                {
                    sFileType = "*.txt";
                    bCsvFile = false;
                }

                if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                {
                    foreach (string sFilePath in Directory.EnumerateFiles(sDataDirectoryPath, sFileType, SearchOption.TopDirectoryOnly))
                    {
                        StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                        if (CheckFileInfo(ref bGetMatchFile, cRawADCSweepItem, cFlowStep, sDataType, srFile, sFilePath, nExecuteIndex, nTXTraceNumber, nRXTraceNumber, bCsvFile: bCsvFile) == false)
                            return false;

                        if (bGetMatchFile == true)
                        {
                            switch (sDataType)
                            {
                                case MainConstantParameter.m_sDATATYPE_ADC:
                                    m_cRawADCSweepItem_List[nExecuteIndex].m_sADCFilePath = sFilePath;
                                    break;
                                default:
                                    break;
                            }

                            bAllFileExist = true;
                            sCsvMatchFileName = Path.GetFileNameWithoutExtension(sFilePath);
                            break;
                        }
                    }
                }
                else
                {
                    //foreach (string sFilePath in Directory.EnumerateFiles(sDataDirectoryPath, "*.", SearchOption.TopDirectoryOnly).Where(s => sExtension_List.Any(ext => ext == Path.GetExtension(s))))
                    foreach (string sFilePath in Directory.EnumerateFiles(sDataDirectoryPath, sFileType, SearchOption.TopDirectoryOnly))
                    {
                        StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                        if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                            cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                        {
                            int nSelfGetMatchFlag = 0;

                            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
                            {
                                bGetMatchFile = false;

                                if (CheckFileInfo(ref bGetMatchFile, cFrequencyItem, cFlowStep, sDataType, srFile, sFilePath, nExecuteIndex, nTXTraceNumber, nRXTraceNumber, bCsvFile: bCsvFile) == false)
                                    return false;

                                if (bGetMatchFile == true)
                                    nSelfGetMatchFlag++;
                            }

                            if (nSelfGetMatchFlag == MainConstantParameter.m_eSelfTraceType_Array.Length)
                                bAllFileExist = true;
                        }
                        else
                        {
                            if (CheckFileInfo(ref bGetMatchFile, cFrequencyItem, cFlowStep, sDataType, srFile, sFilePath, nExecuteIndex, nTXTraceNumber, nRXTraceNumber, bCsvFile: bCsvFile) == false)
                                return false;

                            if (bGetMatchFile == true)
                            {
                                if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2 || cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                                {
                                    switch (sDataType)
                                    {
                                        case MainConstantParameter.m_sDATATYPE_OBASE:
                                            m_cFreqencyItem_List[nExecuteIndex].m_sOBASEFilePath = sFilePath;
                                            break;
                                        case MainConstantParameter.m_sDATATYPE_BASE:
                                            m_cFreqencyItem_List[nExecuteIndex].m_sBASEFilePath = sFilePath;
                                            break;
                                        case MainConstantParameter.m_sDATATYPE_ADC:
                                            m_cFreqencyItem_List[nExecuteIndex].m_sADCFilePath = sFilePath;
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                bAllFileExist = true;
                                sCsvMatchFileName = Path.GetFileNameWithoutExtension(sFilePath);
                                break;
                            }
                        }
                    }
                }

                if (m_bGenerateH5Data == true)
                {
                    if (bAllFileExist == true)
                    {
                        bAllFileExist = false;
                        sDataDirectoryPath = string.Format(@"{0}\{1}\{2}", m_cfrmParent.m_sH5RecordLogDirectoryPath, sStepCodeName, sDataType);

                        foreach (string FilePath in Directory.EnumerateFiles(sDataDirectoryPath, "*.h5", SearchOption.TopDirectoryOnly))
                        {
                            string sH5FileName = Path.GetFileNameWithoutExtension(FilePath);

                            if (sH5FileName == sCsvMatchFileName)
                            {
                                bAllFileExist = true;
                                break;
                            }
                        }
                    }
                }

                if (bAllFileExist == true)
                    sRemoveData_List.Add(sDataType);
            }

            foreach (string sDataType in sRemoveData_List)
                m_sGetData_List.Remove(sDataType);

            return true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bGetMatchFile"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="cFlowStep"></param>
        /// <param name="sDataType"></param>
        /// <param name="srFile"></param>
        /// <param name="sFilePath"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="nTXTraceNumber"></param>
        /// <param name="nRXTraceNumber"></param>
        /// <param name="bCsvFile"></param>
        /// <returns></returns>
        /*
        private bool CheckFileInfo(
            ref bool bGetMatchFile, FrequencyItem cFrequencyItem, frmMain.FlowStep cFlowStep, string sDataType, StreamReader srFile, 
            string sFilePath, int nExecuteIndex, int nTXTraceNumber, int nRXTraceNumber, bool bCsvFile = true)
        {
            AppCoreDefine.FileInfoData cFileInfoData = new AppCoreDefine.FileInfoData();

            cFileInfoData = CheckInfoMatch(ref bGetMatchFile, cFrequencyItem, cFlowStep, srFile, nTXTraceNumber, nRXTraceNumber, m_eSelfTraceType, bCsvFile);

            if (bGetMatchFile == true)
                return true;

            string sFileName = Path.GetFileNameWithoutExtension(sFilePath);

            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                int nSetSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cFileInfoData.m_nSet_SELF_PH2E_LAT, cFileInfoData.m_nSet_SELF_PH2E_LMT, 
                                                                       cFileInfoData.m_nSet_SELF_PH2_LAT, cFileInfoData.m_nSet_SELF_PH2);
                int nSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cFrequencyItem.m_n_SELF_PH2E_LAT, cFrequencyItem.m_n_SELF_PH2E_LMT, 
                                                                    cFrequencyItem.m_n_SELF_PH2_LAT, cFrequencyItem.m_n_SELF_PH2);
                int nReadSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cFileInfoData.m_nRead_SELF_PH2E_LAT, cFileInfoData.m_nRead_SELF_PH2E_LMT, cFileInfoData.m_nRead_SELF_PH2_LAT, cFileInfoData.m_nRead_SELF_PH2);

                //if (cFileInfoData.m_nSet_SELF_PH1 + nSetSelfPH2Sum != cFrequencyItem.m_n_SELF_PH1 + nSelfPH2Sum ||
                //    cFileInfoData.m_nRead_SELF_PH1 + nReadSelfPH2Sum != cFrequencyItem.m_n_SELF_PH1 + nSelfPH2Sum)
                //    return true;

                //if (cFileInfoData.m_nSet_SELF_PH1 + nSetSelfPH2Sum == cFrequencyItem.m_n_SELF_PH1 + nSelfPH2Sum ||
                //     cFileInfoData.m_nRead_SELF_PH1 + nReadSelfPH2Sum == cFrequencyItem.m_n_SELF_PH1 + nSelfPH2Sum)
                //{
                //    m_sErrorMessage = string.Format("Self_PH1 and Self_PH2 Not Match in Frequency Set : {0} and File : {1}", nExecuteIndex + 1, sFileName);
                //    return false;
                //}
            }
            else
            {
                if (cFileInfoData.m_nSetPH1 + cFileInfoData.m_nSetPH2 != cFrequencyItem.m_nPH1 + cFrequencyItem.m_nPH2 ||
                    cFileInfoData.m_nReadPH1 + cFileInfoData.m_nReadPH2 != cFrequencyItem.m_nPH1 + cFrequencyItem.m_nPH2)
                    return true;

                if (cFileInfoData.m_nSetPH1 + cFileInfoData.m_nSetPH2 == cFrequencyItem.m_nPH1 + cFrequencyItem.m_nPH2 ||
                    cFileInfoData.m_nReadPH1 + cFileInfoData.m_nReadPH2 == cFrequencyItem.m_nPH1 + cFrequencyItem.m_nPH2)
                {
                    m_sErrorMessage = string.Format("PH1 and PH2 Not Match in Frequency Set : {0} and File : {1}", nExecuteIndex + 1, sFileName);
                    return false;
                }
            }

            if (cFileInfoData.m_nTX_TraceNumber != nTXTraceNumber)
            {
                m_sErrorMessage = string.Format("TXTraceNumber Not Match in Frequency Set : {0} and File : {1}", nExecuteIndex + 1, sFileName);
                return false;
            }

            if (cFileInfoData.m_nRX_TraceNumber != nRXTraceNumber)
            {
                m_sErrorMessage = string.Format("RXTraceNumber Not Match in Frequency Set : {0} and File : {1}", nExecuteIndex + 1, sFileName);
                return false;
            }

            return true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bGetMatchFile"></param>
        /// <param name="cRawADCSweepItem"></param>
        /// <param name="cFlowStep"></param>
        /// <param name="sDataType"></param>
        /// <param name="srFile"></param>
        /// <param name="sFilePath"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="nTXTraceNumber"></param>
        /// <param name="nRXTraceNumber"></param>
        /// <param name="bCsvFile"></param>
        /// <returns></returns>
        /*
        private bool CheckFileInfo(
            ref bool bGetMatchFile, RawADCSweepItem cRawADCSweepItem, frmMain.FlowStep cFlowStep, string sDataType, StreamReader srFile,
            string sFilePath, int nExecuteIndex, int nTXTraceNumber, int nRXTraceNumber, bool bCsvFile = true)
        {
            AppCoreDefine.FileInfoData cFileInfoData = new AppCoreDefine.FileInfoData();

            cFileInfoData = CheckInfoMatch(ref bGetMatchFile, cRawADCSweepItem, cFlowStep, srFile, nTXTraceNumber, nRXTraceNumber, bCsvFile);

            if (bGetMatchFile == true)
                return true;

            string sFileName = Path.GetFileNameWithoutExtension(sFilePath);

            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                if ((cFileInfoData.m_nSetSELC != cRawADCSweepItem.m_nSELC && cFileInfoData.m_nReadSELC != cRawADCSweepItem.m_nSELC) ||
                    (cFileInfoData.m_nSetVSEL != cRawADCSweepItem.m_nVSEL && cFileInfoData.m_nReadVSEL != cRawADCSweepItem.m_nVSEL) ||
                    (cFileInfoData.m_nSetLG != cRawADCSweepItem.m_nLG && cFileInfoData.m_nReadLG != cRawADCSweepItem.m_nLG) ||
                    (cFileInfoData.m_nSetSELGM != cRawADCSweepItem.m_nSELGM && cFileInfoData.m_nReadSELGM != cRawADCSweepItem.m_nSELGM))
                    return true;

                if ((cFileInfoData.m_nSetSELC == cRawADCSweepItem.m_nSELC && cFileInfoData.m_nReadSELC == cRawADCSweepItem.m_nSELC) &&
                    (cFileInfoData.m_nSetVSEL == cRawADCSweepItem.m_nVSEL && cFileInfoData.m_nReadVSEL == cRawADCSweepItem.m_nVSEL) &&
                    (cFileInfoData.m_nSetLG == cRawADCSweepItem.m_nLG && cFileInfoData.m_nReadLG == cRawADCSweepItem.m_nLG) &&
                    (cFileInfoData.m_nSetSELGM == cRawADCSweepItem.m_nSELGM && cFileInfoData.m_nReadLG == cRawADCSweepItem.m_nSELGM))
                {
                    m_sErrorMessage = string.Format("SELC / VSEL / LG / SELGM Repeated in Parameter Set : {0} and File : {1}", nExecuteIndex + 1, sFileName);
                    return false;
                }
            }

            if (cFileInfoData.m_nTX_TraceNumber != nTXTraceNumber)
            {
                m_sErrorMessage = string.Format("TXTraceNumber Not Match in Parameter Set : {0} and File : {1}", nExecuteIndex + 1, sFileName);
                return false;
            }

            if (cFileInfoData.m_nRX_TraceNumber != nRXTraceNumber)
            {
                m_sErrorMessage = string.Format("RXTraceNumber Not Match in Parameter Set : {0} and File : {1}", nExecuteIndex + 1, sFileName);
                return false;
            }

            return true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFrequencyItem"></param>
        /// <param name="cFlowStep"></param>
        /// <param name="sDataType"></param>
        /// <param name="srFile"></param>
        /// <param name="nTXTraceNumber"></param>
        /// <param name="nRXTraceNumber"></param>
        /// <param name="bCsvFile"></param>
        /// <returns></returns>
        private bool CheckFileInfo(
            FrequencyItem cFrequencyItem, frmMain.FlowStep cFlowStep, string sDataType, StreamReader srFile, int nTXTraceNumber,
            int nRXTraceNumber, bool bCsvFile = true)
        {
            bool bMatchFlag = false;

            CheckInfoMatch(ref bMatchFlag, cFrequencyItem, cFlowStep, srFile, nTXTraceNumber, nRXTraceNumber, eSelfTraceType: m_eSelfTraceType, bCsvFile: bCsvFile);

            return bMatchFlag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cRawADCSweepItem"></param>
        /// <param name="cFlowStep"></param>
        /// <param name="sDataType"></param>
        /// <param name="srFile"></param>
        /// <param name="nTXTraceNumber"></param>
        /// <param name="nRXTraceNumber"></param>
        /// <param name="bCsvFile"></param>
        /// <returns></returns>
        private bool CheckFileInfo(
            RawADCSweepItem cRawADCSweepItem, frmMain.FlowStep cFlowStep, string sDataType, StreamReader srFile, int nTXTraceNumber,
            int nRXTraceNumber, bool bCsvFile = true)
        {
            bool bMatchFlag = false;

            CheckInfoMatch(ref bMatchFlag, cRawADCSweepItem, cFlowStep, srFile, nTXTraceNumber, nRXTraceNumber, bCsvFile: bCsvFile);

            return bMatchFlag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bMatchFlag"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="cFlowStep"></param>
        /// <param name="srFile"></param>
        /// <param name="nTXTraceNumber"></param>
        /// <param name="nRXTraceNumber"></param>
        /// <param name="eSelfTraceType"></param>
        /// <param name="bCsvFile"></param>
        /// <returns></returns>
        private AppCoreDefine.FileInfoData CheckInfoMatch(
            ref bool bMatchFlag, FrequencyItem cFrequencyItem, frmMain.FlowStep cFlowStep, StreamReader srFile,
            int nTXTraceNumber, int nRXTraceNumber, TraceType eSelfTraceType = TraceType.ALL, bool bCsvFile = true)
        {
            string sLine = "";
            AppCoreDefine.FileInfoData cFileInfoData = new AppCoreDefine.FileInfoData();

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSplit_Array = null;

                    if (bCsvFile == false)
                    {
                        sSplit_Array = sLine.Split('=');

                        for (int nSplitIndex = 0; nSplitIndex < sSplit_Array.Length; nSplitIndex++)
                            sSplit_Array[nSplitIndex] = sSplit_Array[nSplitIndex].Trim();
                    }
                    else
                        sSplit_Array = sLine.Split(',');

                    if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                        cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                    {
                        if (sSplit_Array.Length >= 2)
                        {
                            if (sSplit_Array[0] == "TXTraceNumber")
                                Int32.TryParse(sSplit_Array[1], out cFileInfoData.m_nTX_TraceNumber);
                            else if (sSplit_Array[0] == "RXTraceNumber")
                                Int32.TryParse(sSplit_Array[1], out cFileInfoData.m_nRX_TraceNumber);
                            else if (sSplit_Array[0] == "TraceType")
                                cFileInfoData.m_sSelfTraceType = sSplit_Array[1];
                            else if (sSplit_Array[0] == "Set_SELF_PH1(Hex)")
                                cFileInfoData.m_nSet_SELF_PH1 = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "Set_SELF_PH2E_LAT(Hex)")
                                cFileInfoData.m_nSet_SELF_PH2E_LAT = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "Set_SELF_PH2E_LMT(Hex)")
                                cFileInfoData.m_nSet_SELF_PH2E_LMT = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "Set_SELF_PH2_LAT(Hex)")
                                cFileInfoData.m_nSet_SELF_PH2_LAT = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "Set_SELF_PH2(Hex)")
                                cFileInfoData.m_nSet_SELF_PH2 = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "SetSelf_Sum(Hex)")
                                cFileInfoData.m_nSet_SELF_DFT_NUM = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "SetSelf_Gain(Hex)")
                                cFileInfoData.m_nSet_SELF_SELGM = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "Read_SELF_PH1(Hex)")
                                cFileInfoData.m_nRead_SELF_PH1 = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "Read_SELF_PH2E_LAT(Hex)")
                                cFileInfoData.m_nRead_SELF_PH2E_LAT = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "Read_SELF_PH2E_LMT(Hex)")
                                cFileInfoData.m_nRead_SELF_PH2E_LMT = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "Read_SELF_PH2_LAT(Hex)")
                                cFileInfoData.m_nRead_SELF_PH2_LAT = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "Read_SELF_PH2(Hex)")
                                cFileInfoData.m_nRead_SELF_PH2 = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "ReadSelf_Sum(Hex)")
                                cFileInfoData.m_nRead_SELF_DFT_NUM = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "ReadSelf_Gain(Hex)")
                                cFileInfoData.m_nRead_SELF_SELGM = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "SelfNCPValue")
                                cFileInfoData.m_nSelf_NCP = Convert.ToInt32(sSplit_Array[1]);
                            else if (sSplit_Array[0] == "SelfNCNValue")
                                cFileInfoData.m_nSelf_NCN = Convert.ToInt32(sSplit_Array[1]);
                        }

                        if (cFileInfoData.m_sSelfTraceType != "" &&
                            cFileInfoData.m_nSet_SELF_PH1 > -1 && cFileInfoData.m_nSet_SELF_PH2E_LAT > -1 && cFileInfoData.m_nSet_SELF_PH2E_LMT > -1 &&
                            cFileInfoData.m_nSet_SELF_PH2_LAT > -1 && cFileInfoData.m_nSet_SELF_PH2 > -1 && cFileInfoData.m_nSet_SELF_DFT_NUM > -1 &&
                            cFileInfoData.m_nRead_SELF_PH1 > -1 && cFileInfoData.m_nRead_SELF_PH2E_LAT > -1 && cFileInfoData.m_nRead_SELF_PH2E_LMT > -1 &&
                            cFileInfoData.m_nRead_SELF_PH2_LAT > -1 && cFileInfoData.m_nRead_SELF_PH2 > -1 &&
                            cFileInfoData.m_nRead_SELF_DFT_NUM > -1 && cFileInfoData.m_nRead_SELF_SELGM > -1 &&
                            cFileInfoData.m_nTX_TraceNumber > -1 && cFileInfoData.m_nRX_TraceNumber > -1 &&
                            cFileInfoData.m_nSelf_NCP > -1 && cFileInfoData.m_nSelf_NCN > -1)
                            break;
                    }
                    else
                    {
                        if (sSplit_Array.Length >= 2)
                        {
                            if (sSplit_Array[0] == "TXTraceNumber")
                                Int32.TryParse(sSplit_Array[1], out cFileInfoData.m_nTX_TraceNumber);
                            else if (sSplit_Array[0] == "RXTraceNumber")
                                Int32.TryParse(sSplit_Array[1], out cFileInfoData.m_nRX_TraceNumber);
                            else if (sSplit_Array[0] == "SetPH1(Hex)")
                                cFileInfoData.m_nSetPH1 = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "SetPH2(Hex)")
                                cFileInfoData.m_nSetPH2 = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "ReadPH1(Hex)")
                                cFileInfoData.m_nReadPH1 = Convert.ToInt32(sSplit_Array[1], 16);
                            else if (sSplit_Array[0] == "ReadPH2(Hex)")
                                cFileInfoData.m_nReadPH2 = Convert.ToInt32(sSplit_Array[1], 16);
                            /*
                            else if (sSplit_Array[0] == "ReadFWIP_Option(Hex)")
                                cFileInfoData.m_nFWIP_Option = Convert.ToInt32(sSplit_Array[1], 16);
                            */
                        }

                        if (cFileInfoData.m_nSetPH1 > -1 && cFileInfoData.m_nSetPH2 > -1 &&
                            cFileInfoData.m_nReadPH1 > -1 && cFileInfoData.m_nReadPH2 > -1 &&
                            cFileInfoData.m_nTX_TraceNumber > -1 && cFileInfoData.m_nRX_TraceNumber > -1)
                            //cFileInfoData.m_nFWIP_Option > -1)
                            break;
                    }
                }
            }
            finally
            {
                srFile.Close();
            }

            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                if (eSelfTraceType == TraceType.ALL)
                {
                    if (cFileInfoData.m_nSet_SELF_PH1 == cFrequencyItem.m_n_SELF_PH1 && cFileInfoData.m_nRead_SELF_PH1 == cFrequencyItem.m_n_SELF_PH1 &&
                        cFileInfoData.m_nSet_SELF_PH2E_LAT == cFrequencyItem.m_n_SELF_PH2E_LAT && cFileInfoData.m_nRead_SELF_PH2E_LAT == cFrequencyItem.m_n_SELF_PH2E_LAT &&
                        cFileInfoData.m_nSet_SELF_PH2E_LMT == cFrequencyItem.m_n_SELF_PH2E_LMT && cFileInfoData.m_nRead_SELF_PH2E_LMT == cFrequencyItem.m_n_SELF_PH2E_LMT &&
                        cFileInfoData.m_nSet_SELF_PH2_LAT == cFrequencyItem.m_n_SELF_PH2_LAT && cFileInfoData.m_nRead_SELF_PH2_LAT == cFrequencyItem.m_n_SELF_PH2_LAT &&
                        cFileInfoData.m_nSet_SELF_PH2 == cFrequencyItem.m_n_SELF_PH2 && cFileInfoData.m_nRead_SELF_PH2 == cFrequencyItem.m_n_SELF_PH2 &&
                        cFileInfoData.m_nSet_SELF_DFT_NUM == cFrequencyItem.m_nSelf_DFT_NUM && cFileInfoData.m_nRead_SELF_DFT_NUM == cFrequencyItem.m_nSelf_DFT_NUM &&
                        cFileInfoData.m_nTX_TraceNumber == nTXTraceNumber && cFileInfoData.m_nRX_TraceNumber == nRXTraceNumber &&
                        cFileInfoData.m_nSelf_NCP == m_nSelfNCPValue && cFileInfoData.m_nSelf_NCN == m_nSelfNCNValue)
                        bMatchFlag = true;
                }
                else
                {
                    if (cFileInfoData.m_nSet_SELF_PH1 == cFrequencyItem.m_n_SELF_PH1 && cFileInfoData.m_nRead_SELF_PH1 == cFrequencyItem.m_n_SELF_PH1 &&
                        cFileInfoData.m_nSet_SELF_PH2E_LAT == cFrequencyItem.m_n_SELF_PH2E_LAT && cFileInfoData.m_nRead_SELF_PH2E_LAT == cFrequencyItem.m_n_SELF_PH2E_LAT &&
                        cFileInfoData.m_nSet_SELF_PH2E_LMT == cFrequencyItem.m_n_SELF_PH2E_LMT && cFileInfoData.m_nRead_SELF_PH2E_LMT == cFrequencyItem.m_n_SELF_PH2E_LMT &&
                        cFileInfoData.m_nSet_SELF_PH2_LAT == cFrequencyItem.m_n_SELF_PH2_LAT && cFileInfoData.m_nRead_SELF_PH2_LAT == cFrequencyItem.m_n_SELF_PH2_LAT &&
                        cFileInfoData.m_nSet_SELF_PH2 == cFrequencyItem.m_n_SELF_PH2 && cFileInfoData.m_nRead_SELF_PH2 == cFrequencyItem.m_n_SELF_PH2 &&
                        cFileInfoData.m_nTX_TraceNumber == nTXTraceNumber && cFileInfoData.m_nRX_TraceNumber == nRXTraceNumber &&
                        cFileInfoData.m_sSelfTraceType == eSelfTraceType.ToString() &&
                        cFileInfoData.m_nSelf_NCP == m_nSelfNCPValue && cFileInfoData.m_nSelf_NCN == m_nSelfNCNValue)
                        bMatchFlag = true;
                }
            }
            else
            {
                if (cFileInfoData.m_nSetPH1 == cFrequencyItem.m_nPH1 && cFileInfoData.m_nReadPH1 == cFrequencyItem.m_nPH1 &&
                    cFileInfoData.m_nSetPH2 == cFrequencyItem.m_nPH2 && cFileInfoData.m_nReadPH2 == cFrequencyItem.m_nPH2 &&
                    cFileInfoData.m_nTX_TraceNumber == nTXTraceNumber && cFileInfoData.m_nRX_TraceNumber == nRXTraceNumber)
                    bMatchFlag = true;
            }

            return cFileInfoData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bMatchFlag"></param>
        /// <param name="cRawADCSweepItem"></param>
        /// <param name="cFlowStep"></param>
        /// <param name="srFile"></param>
        /// <param name="nTXTraceNumber"></param>
        /// <param name="nRXTraceNumber"></param>
        /// <param name="bCsvFile"></param>
        /// <returns></returns>
        private AppCoreDefine.FileInfoData CheckInfoMatch(
            ref bool bMatchFlag, RawADCSweepItem cRawADCSweepItem, frmMain.FlowStep cFlowStep, StreamReader srFile,
            int nTXTraceNumber, int nRXTraceNumber, bool bCsvFile = true)
        {
            string sLine = "";
            AppCoreDefine.FileInfoData cFileInfoData = new AppCoreDefine.FileInfoData();

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSplit_Array = null;

                    if (bCsvFile == false)
                    {
                        sSplit_Array = sLine.Split('=');

                        for (int nSplitIndex = 0; nSplitIndex < sSplit_Array.Length; nSplitIndex++)
                            sSplit_Array[nSplitIndex] = sSplit_Array[nSplitIndex].Trim();
                    }
                    else
                        sSplit_Array = sLine.Split(',');

                    if (sSplit_Array.Length >= 2)
                    {
                        if (sSplit_Array[0] == "TXTraceNumber")
                            Int32.TryParse(sSplit_Array[1], out cFileInfoData.m_nTX_TraceNumber);
                        else if (sSplit_Array[0] == "RXTraceNumber")
                            Int32.TryParse(sSplit_Array[1], out cFileInfoData.m_nRX_TraceNumber);
                        else if (sSplit_Array[0] == "Set_SELC(Hex)")
                            cFileInfoData.m_nSetSELC = Convert.ToInt32(sSplit_Array[1], 16);
                        else if (sSplit_Array[0] == "Set_VSEL(Hex)")
                            cFileInfoData.m_nSetVSEL = Convert.ToInt32(sSplit_Array[1], 16);
                        else if (sSplit_Array[0] == "Set_LG(Hex)")
                            cFileInfoData.m_nSetLG = Convert.ToInt32(sSplit_Array[1], 16);
                        else if (sSplit_Array[0] == "Set_SELGM(Hex)")
                            cFileInfoData.m_nSetSELGM = Convert.ToInt32(sSplit_Array[1], 16);
                        else if (sSplit_Array[0] == "Read_SELC(Hex)")
                            cFileInfoData.m_nReadSELC = Convert.ToInt32(sSplit_Array[1], 16);
                        else if (sSplit_Array[0] == "Read_VSEL(Hex)")
                            cFileInfoData.m_nReadVSEL = Convert.ToInt32(sSplit_Array[1], 16);
                        else if (sSplit_Array[0] == "Read_LG(Hex)")
                            cFileInfoData.m_nReadLG = Convert.ToInt32(sSplit_Array[1], 16);
                        else if (sSplit_Array[0] == "Read_SELGM(Hex)")
                            cFileInfoData.m_nReadSELGM = Convert.ToInt32(sSplit_Array[1], 16);
                    }

                    if (cFileInfoData.m_nSetSELC > -1 && cFileInfoData.m_nSetVSEL > -1 && cFileInfoData.m_nSetLG > -1 && cFileInfoData.m_nSetSELGM > -1 &&
                        cFileInfoData.m_nReadSELC > -1 && cFileInfoData.m_nReadVSEL > -1 && cFileInfoData.m_nReadLG > -1 && cFileInfoData.m_nReadSELGM > -1 &&
                        cFileInfoData.m_nTX_TraceNumber > -1 && cFileInfoData.m_nRX_TraceNumber > -1)
                        break;
                }
            }
            finally
            {
                srFile.Close();
            }

            if (cFileInfoData.m_nSetSELC == cRawADCSweepItem.m_nSELC && cFileInfoData.m_nReadSELC == cRawADCSweepItem.m_nSELC &&
                cFileInfoData.m_nSetVSEL == cRawADCSweepItem.m_nVSEL && cFileInfoData.m_nReadVSEL == cRawADCSweepItem.m_nVSEL &&
                cFileInfoData.m_nSetLG == cRawADCSweepItem.m_nLG && cFileInfoData.m_nReadLG == cRawADCSweepItem.m_nLG &&
                cFileInfoData.m_nSetSELGM == cRawADCSweepItem.m_nSELGM && cFileInfoData.m_nReadSELGM == cRawADCSweepItem.m_nSELGM &&
                cFileInfoData.m_nTX_TraceNumber == nTXTraceNumber && cFileInfoData.m_nRX_TraceNumber == nRXTraceNumber)
                bMatchFlag = true;

            return cFileInfoData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nRecordDataIndex"></param>
        /// <returns></returns>
        private bool LoadUserDefinedCommandScript(frmMain.FlowStep cFlowStep, int nRecordDataIndex)
        {
            if (m_eICGenerationType == ICGenerationType.Gen8 && (ParamFingerAutoTuning.m_nCommandScriptType == 1 || ParamFingerAutoTuning.m_nCommandScriptType == 2))
            {
                if (File.Exists(m_sCommandScriptFilePath) == false)
                {
                    m_sErrorMessage = "User Defined Command Script File Not Exist";
                    return false;
                }

                ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);
                bool bResultFlag = cElanCommand_Gen8.LoadUserDefinedCommandScript(ref m_cSendCommandInfo, cFlowStep, nRecordDataIndex, m_sCommandScriptFilePath);

                if (bResultFlag == false)
                {
                    m_sErrorMessage = cElanCommand_Gen8.GetErrorMessage();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFreqencyItem"></param>
        /// <param name="nTXTraceNumber"></param>
        /// <returns></returns>
        private int ComputeSuggestDFT_NUM(FrequencyItem cFreqencyItem, int nTXTraceNumber)
        {
            if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
            {
                int nPH1Value = cFreqencyItem.m_nPH1;
                int nPH2Value = cFreqencyItem.m_nPH2;
                int nSumValue = ElanConvert.Convert2SuggestDFT_NUM(nPH1Value, nPH2Value, nTXTraceNumber, ParamFingerAutoTuning.m_nIdealScanTime);
                return nSumValue;
            }
            else
            {
                int nSumValue = m_cOriginParameter.m_n_MS_DFT_NUM;
                return nSumValue;
            }
        }
    }
}
