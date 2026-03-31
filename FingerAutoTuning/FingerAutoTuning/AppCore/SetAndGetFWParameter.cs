using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
#if DISABLE_V1_0_2_6
        private bool SetAndGetFWParameter(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem = null, RawADCSweepItem cRawADCSweepItem = null)
        {
            if (ParamFingerAutoTuning.m_nDisableSetAnalogParameter == 1)
            {
                if (m_eICGenerationType == ICGenerationType.Gen9)
                {
                    m_cReadParameter.m_nPH1 = cFrequencyItem.m_nPH1;
                    m_cReadParameter.m_nPH2 = cFrequencyItem.m_nPH2;
                    m_cReadParameter.m_nPH3 = cFrequencyItem.m_nPH2;
                    m_cReadParameter.m_n_MS_DFT_NUM = cFrequencyItem.m_nDFT_NUM;
                }
                else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                {
                    m_cReadParameter.m_n_MS_SELC = cRawADCSweepItem.m_nSELC;
                    m_cReadParameter.m_n_MS_VSEL = cRawADCSweepItem.m_nVSEL;
                    m_cReadParameter.m_n_MS_LG = cRawADCSweepItem.m_nLG;
                    m_cReadParameter.m_n_MS_SELGM = cRawADCSweepItem.m_nSELGM;
                }
                else if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                {
                    m_cReadParameter.m_n_SELF_PH1 = cFrequencyItem.m_n_SELF_PH1;
                    m_cReadParameter.m_n_SELF_PH2E_LAT = cFrequencyItem.m_n_SELF_PH2E_LAT;
                    m_cReadParameter.m_n_SELF_PH2E_LMT = cFrequencyItem.m_n_SELF_PH2E_LMT;
                    m_cReadParameter.m_n_SELF_PH2_LAT = cFrequencyItem.m_n_SELF_PH2_LAT;
                    m_cReadParameter.m_n_SELF_PH2 = cFrequencyItem.m_n_SELF_PH2;
                    m_cReadParameter.m_nSelf_DFT_NUM = m_cSelfParameter.m_nDFT_NUM;
                    m_cReadParameter.m_nSelf_Gain = m_cSelfParameter.m_nGain;
                    m_cReadParameter.m_nSelf_CAG = m_cSelfParameter.m_nCAG;
                    m_cReadParameter.m_nSelf_IQ_BSH = m_cSelfParameter.m_nIQ_BSH;
                }
                else
                {
                    m_cReadParameter.m_nPH1 = cFrequencyItem.m_nPH1;
                    m_cReadParameter.m_nPH2 = cFrequencyItem.m_nPH2;
                    m_cReadParameter.m_nPH3 = cFrequencyItem.m_nPH2;
                    m_cReadParameter.m_n_MS_DFT_NUM = cFrequencyItem.m_nDFT_NUM;
                }

                return true;
            }

            const int nRETRYLIMITTIMES = 3;
            bool bSetParameterSuccessFlag = false;
            AppCoreDefine.SetState eSetState = AppCoreDefine.SetState.Initial;
            int nRetryIncreaseTime = 0;
            int nReKSuccessCount = 0;

            for (int nRetryIndex = 0; nRetryIndex <= nRETRYLIMITTIMES; nRetryIndex++)
            {
                if (m_cfrmParent.m_bExecute == false)
                    break;

                m_cReadParameter.Initialize();

                bool bErrorFlag = false;
                bool bLastRetryFlag = false;
                nRetryIncreaseTime = nRetryIndex * 100;

                if (nRetryIndex == nRETRYLIMITTIMES)
                    bLastRetryFlag = true;

                if (nRetryIndex > 0 && m_eICGenerationType != ICGenerationType.Gen9 && (!(ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER && nReKSuccessCount > 0 && m_bGetFirstDataFlag == true)))
                {
                    if (ConnectToTP() == false)
                        break;

                    SetReportEnable(false);

                    //Get And Set Project_Option, FWIP_Option
                    short nOriginProjectOption = 0;
                    short nOriginFWIPOption = 0;
                    short nReadFWIPOption = 0;

                    if (GetAndSetOptions(ref nOriginProjectOption, ref nOriginFWIPOption, ref nReadFWIPOption) == false)
                        break;
                }

                for (int nSetIndex = 0; nSetIndex < nRETRYLIMITTIMES; nSetIndex++)
                {
                    if (eSetState == AppCoreDefine.SetState.Reset)
                    {
                        if (SetFrontState() == false)
                        {
                            bErrorFlag = true;
                            break;
                        }
                    }

                    if (m_eICGenerationType == ICGenerationType.Gen8)
                    {
                        SetTestModeEnable(true);
                        Thread.Sleep(nRetryIncreaseTime);

                        if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                            cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data, null, true);
                        else
                            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data);

                        Thread.Sleep(nRetryIncreaseTime);
                    }

                    if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                        cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                    {
                        AppCoreDefine.FWParameter cFWParameter = new AppCoreDefine.FWParameter();
                        cFWParameter.m_n_SELF_PH1 = cFrequencyItem.m_n_SELF_PH1;
                        cFWParameter.m_n_SELF_PH2E_LAT = cFrequencyItem.m_n_SELF_PH2E_LAT;
                        cFWParameter.m_n_SELF_PH2E_LMT = cFrequencyItem.m_n_SELF_PH2E_LMT;
                        cFWParameter.m_n_SELF_PH2_LAT = cFrequencyItem.m_n_SELF_PH2E_LMT;
                        cFWParameter.m_n_SELF_PH2_MUX_LAT = m_cOriginParameter.m_n_SELF_PH2_MUX_LAT;
                        cFWParameter.m_n_SELF_PH2 = cFrequencyItem.m_n_SELF_PH2;
                        cFWParameter.m_nSelf_DFT_NUM = m_cSelfParameter.m_nDFT_NUM;
                        cFWParameter.m_nSelf_Gain = m_cSelfParameter.m_nGain;
                        cFWParameter.m_nSelf_CAG = m_cSelfParameter.m_nCAG;
                        cFWParameter.m_nSelf_IQ_BSH = m_cSelfParameter.m_nIQ_BSH;

                        string sSetKSequence = MainConstantParameter.m_sKSequence_NA;
                        bool bSetCALValue = false;
                        int nCALStartTrace = -1;
                        int nCALStartTraceNumber = 0;
                        int nCALEndTraceNumber = 0;

                        if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep)
                        {
                            if (ParamFingerAutoTuning.m_nSelfFSRunKSequence == 1 || ParamFingerAutoTuning.m_nSelfFSRunKSequence == 2)
                                sSetKSequence = MainConstantParameter.m_sKSequence_FixedValue;
                            else if (ParamFingerAutoTuning.m_nSelfFSRunKSequence == 3)
                                sSetKSequence = MainConstantParameter.m_sKSequence_FileValue;

                            if (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2)
                            {
                                bSetCALValue = true;

                                if (ParamFingerAutoTuning.m_nSelfFSRunCALData == 2)
                                {
                                    nCALStartTrace = ParamFingerAutoTuning.m_nSelfFSCALStartTrace;
                                    nCALStartTraceNumber = ParamFingerAutoTuning.m_nSelfFSCALStartTraceNumber;
                                    nCALEndTraceNumber = ParamFingerAutoTuning.m_nSelfFSCALEndTraceNumber;
                                }
                            }
                        }
                        else if (cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                            sSetKSequence = MainConstantParameter.m_sKSequence_FixedValue;

                        eSetState = SetFWParameter(cFlowStep, cFWParameter, ParamFingerAutoTuning.m_nReKTimeout, bSetSelfParameter: true, sSetSelfKSequence: sSetKSequence, bSetSelfCALValue: bSetCALValue,
                                                   nSelfCALStartTrace: nCALStartTrace, nSelfCALStartTraceNumber: nCALStartTraceNumber,
                                                   nSelfCALEndTraceNumber: nCALEndTraceNumber, nRetryIncreaseTime: nRetryIncreaseTime);
                    }
                    else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                    {
                        AppCoreDefine.FWParameter cFWParameter = new AppCoreDefine.FWParameter();

                        if (m_eICGenerationType == ICGenerationType.Gen8)
                        {
                            cFWParameter.m_n_MS_SELC = cRawADCSweepItem.m_nSELC;
                            cFWParameter.m_n_MS_VSEL = cRawADCSweepItem.m_nVSEL;
                            cFWParameter.m_n_MS_LG = cRawADCSweepItem.m_nLG;
                            cFWParameter.m_n_MS_SELGM = cRawADCSweepItem.m_nSELGM;
                            cFWParameter.m_n_MS_FIRCOEF_SEL = ParamFingerAutoTuning.m_nRawADCSGen8FixedFIRCOEF_SEL;
                            cFWParameter.m_n_MS_FIR_TAP_NUM = ParamFingerAutoTuning.m_nRawADCSFixedFIR_TAP_NUM;
                            cFWParameter.m_n_MS_DFT_NUM = m_cOriginParameter.m_n_MS_DFT_NUM;
                            cFWParameter.m_n_MS_IQ_BSH = m_cOriginParameter.m_n_MS_IQ_BSH;
                        }
                        else if (m_eICGenerationType == ICGenerationType.Gen7)
                        {
                            cFWParameter.m_n_MS_SELC = cRawADCSweepItem.m_nSELC;
                            cFWParameter.m_n_MS_VSEL = cRawADCSweepItem.m_nVSEL;
                            cFWParameter.m_n_MS_LG = cRawADCSweepItem.m_nLG;
                            cFWParameter.m_n_MS_SELGM = cRawADCSweepItem.m_nSELGM;
                            cFWParameter.m_n_MS_FIRTB = ParamFingerAutoTuning.m_nRawADCSGen6or7FixedFIRTB;
                            cFWParameter.m_n_MS_FIR_TAP_NUM = ParamFingerAutoTuning.m_nRawADCSFixedFIR_TAP_NUM;
                            cFWParameter.m_n_MS_DFT_NUM = m_cOriginParameter.m_n_MS_DFT_NUM;
                            cFWParameter.m_n_MS_IQ_BSH = m_cOriginParameter.m_n_MS_IQ_BSH;
                        }
                        else if (m_eICGenerationType == ICGenerationType.Gen6)
                        {
                            cFWParameter.m_n_MS_SELC = cRawADCSweepItem.m_nSELC;
                            cFWParameter.m_n_MS_VSEL = cRawADCSweepItem.m_nVSEL;
                            cFWParameter.m_n_MS_LG = cRawADCSweepItem.m_nLG;
                            cFWParameter.m_n_MS_SELGM = cRawADCSweepItem.m_nSELGM;
                            cFWParameter.m_n_MS_FIRTB = ParamFingerAutoTuning.m_nRawADCSGen6or7FixedFIRTB;
                            cFWParameter.m_n_MS_FIR_TAP_NUM = ParamFingerAutoTuning.m_nRawADCSFixedFIR_TAP_NUM;
                            cFWParameter.m_n_MS_DFT_NUM = m_cOriginParameter.m_n_MS_DFT_NUM;
                            cFWParameter.m_n_MS_IQ_BSH = m_cOriginParameter.m_n_MS_IQ_BSH;
                        }

                        eSetState = SetFWParameter(cFlowStep, cFWParameter, ParamFingerAutoTuning.m_nReKTimeout, nRetryIncreaseTime: nRetryIncreaseTime);
                    }
                    else
                    {
                        if (m_eICGenerationType != ICGenerationType.Gen9)
                        {
                            AppCoreDefine.FWParameter cFWParameter = new AppCoreDefine.FWParameter();
                            cFWParameter.m_nPH1 = cFrequencyItem.m_nPH1;
                            cFWParameter.m_nPH2 = cFrequencyItem.m_nPH2;
                            cFWParameter.m_nPH3 = cFrequencyItem.m_nPH2;
                            cFWParameter.m_n_MS_DFT_NUM = cFrequencyItem.m_nDFT_NUM;

                            eSetState = SetFWParameter(cFlowStep, cFWParameter, ParamFingerAutoTuning.m_nReKTimeout, nRetryIncreaseTime: nRetryIncreaseTime);
                        }
#if _USE_9F07_SOCKET
                        else
                        {

                            AppCoreDefine.FWParameter cFWParameter = new AppCoreDefine.FWParameter();
                            cFWParameter.m_nPH1 = cFrequencyItem.m_nPH1;
                            cFWParameter.m_nPH2 = cFrequencyItem.m_nPH2;
                            cFWParameter.m_nPH3 = cFrequencyItem.m_nPH2;
                            cFWParameter.m_n_MS_DFT_NUM = cFrequencyItem.m_nDFT_NUM;

                            eSetState = SetFWParameter_9F07(cFWParameter, nRetryIncreaseTime:nRetryIncreaseTime);
                        }
#endif
                    }

                    if (m_cfrmParent.m_bExecute == false)
                    {
                        bErrorFlag = true;
                        break;
                    }

                    if (m_eICGenerationType != ICGenerationType.Gen9)
                    {
                        if ((m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nCommandScriptType != 2) ||
                            m_eICGenerationType == ICGenerationType.Gen7 ||
                            m_eICGenerationType == ICGenerationType.Gen6 ||
                            m_eICGenerationType == ICGenerationType.Other)
                        {
                            if (m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nGen8GetAnalogAfterSwitchTestMode == 1)
                            {
                                SetTestModeEnable(false);
                                Thread.Sleep(100);
                                SetTestModeEnable(true);
                            }

                            if (GetFWParameter(cFlowStep) == false)
                                eSetState = AppCoreDefine.SetState.Error;

                            if (eSetState == AppCoreDefine.SetState.Success)
                            {
                                if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                                {
                                    if (CheckAnalogParameter(cFlowStep, bLastRetryFlag, null, cRawADCSweepItem) == false)
                                        eSetState = AppCoreDefine.SetState.Error;
                                }
                                else
                                {
                                    if (CheckAnalogParameter(cFlowStep, bLastRetryFlag, cFrequencyItem) == false)
                                        eSetState = AppCoreDefine.SetState.Error;
                                }
                            }
                        }
                    }
#if _USE_9F07_SOCKET
                    else
                    {
                        bool bResultFlag = GetFWParameter(cFlowStep);

                        if (bResultFlag == false)
                        {
                            eSetState = AppCoreDefine.SetState.Error;
                            //return false;
                        }
                        else
                        {
                            if (CheckAnalogParameter(cFlowStep, bLastRetryFlag, cFrequencyItem) == false)
                                eSetState = AppCoreDefine.SetState.Error;
                        }
                    }
#endif

                    if (eSetState != AppCoreDefine.SetState.Success)
                    {
                        OutputMessage(string.Format("-SetState:{0}, RetryIndex:{1}, SetIndex:{2}, Error:{3}, SetParameterSuccess:{4}", eSetState.ToString(), nRetryIndex, nSetIndex, bErrorFlag, bSetParameterSuccessFlag));

                        if (nSetIndex == nRETRYLIMITTIMES - 1)
                        {
                            m_sErrorMessage = "Set Analog Parameter Fail";
                            bErrorFlag = true;
                            break;
                        }
                        else
                        {
                            Thread.Sleep(500);
                            continue;
                        }
                    }
                    else
                        OutputDebugLog(string.Format("-SetState:{0}, RetryIndex:{1}, SetIndex:{2}, Error:{3}, SetParameterSuccess:{4}", eSetState.ToString(), nRetryIndex, nSetIndex, bErrorFlag, bSetParameterSuccessFlag));

                    break;
                }

                if (bErrorFlag == true)
                {
                    if (nRetryIndex == nRETRYLIMITTIMES)
                        break;
                    else
                        continue;
                }

                /*
                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    MessageOutput("-Disable Report");
                    //ElanTouch.DisableTPReport(true, 1000, m_nDeviceIndex);
                    ElanTouchSwitch.DisableTPReport(true, m_nDeviceIndex, m_bSocketConnectType);
                    m_bDisableReport = true;
                    Thread.Sleep(m_nNormalDelayTime);
                }
                */

                /*
                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    if ((m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nCommandScriptType != 2) ||
                        m_eICGenerationType == ICGenerationType.Gen7 ||
                        m_eICGenerationType == ICGenerationType.Gen6 ||
                        m_eICGenerationType == ICGenerationType.Other)
                    {
                        if (m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nGen8GetAnalogAfterSwitchTestMode == 1)
                        {
                            SetTestModeEnable(false);
                            Thread.Sleep(100);
                            SetTestModeEnable(true);
                        }

                        if (GetFWParameter(cFlowStep) == false)
                            return false;

                        if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                        {
                            if (CheckAnalogParameter(cFlowStep, bLastRetryFlag, null, cRawADCSweepItem) == false)
                                bErrorFlag = true;
                        }
                        else
                        {
                            if (CheckAnalogParameter(cFlowStep, bLastRetryFlag, cFrequencyItem) == false)
                                bErrorFlag = true;
                        }
                    }
                }
#if _USE_9F07_SOCKET
                else
                {
                    bool bResultFlag = GetFWParameter(cFlowStep);

                    if (bResultFlag == false)
                    {
                        bErrorFlag = true;
                        //return false;
                    }
                    else
                    {
                        if (CheckAnalogParameter(cFlowStep, bLastRetryFlag, cFrequencyItem) == false)
                            bErrorFlag = true;
                    }
                }
#endif
                */

                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    if (eSetState == AppCoreDefine.SetState.Success)
                        SetUserDefinedCommand();

                    if (eSetState == AppCoreDefine.SetState.Success)
                        eSetState = SetReK(ParamFingerAutoTuning.m_nReKTimeout);

                    if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2 || cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                    {
                        if (ParamFingerAutoTuning.m_nGen8GetBaseType == 2 || ParamFingerAutoTuning.m_nGen8GetBaseType == 3)
                            SetTestModeEnable(false);
                        else if (ParamFingerAutoTuning.m_nGen8SetUpdateBaseType == 1)
                        {
                            SetFast_UpdateBase_Gen8();
                            SetTestModeEnable(false);
                        }
                        else if (ParamFingerAutoTuning.m_nGen8SetUpdateBaseType == 2)
                        {
                            SetTestModeEnable(false);
                            SetReportEnable(false);
                            SetFast_UpdateBase_Gen8();
                        }
                        else if (ParamFingerAutoTuning.m_nGen8SetUpdateBaseType == 3)
                        {
                            SetFast_UpdateBase_Gen8(bSetUpdateBaseDelayTime: false);
                            SetTestModeEnable(false);
                            Thread.Sleep(ParamFingerAutoTuning.m_nGen8UpdateBaseDelayTime);
                        }
                        else if (ParamFingerAutoTuning.m_nGen8SetUpdateBaseType == 4)
                        {
                            SetFast_UpdateBase_Gen8();
                            SetTestModeEnable(false);
                            SetReportEnable(false);
                            Thread.Sleep(100);
                            SetFast_UpdateBase_Gen8();
                        }
                        else if (ParamFingerAutoTuning.m_nGen8SetUpdateBaseType == 5)
                        {
                            if (m_nNoResetBaseFunctionFlag_Gen8 == 1)
                                SetTestModeEnable(false);
                            else
                            {
                                int nResetBaseFlag = SetReset_Base_Gen8(bSetUpdateBaseDelayTime: false);

                                if (nResetBaseFlag == 0)
                                    bErrorFlag = true;
                                else if (nResetBaseFlag == -1)
                                {
                                    SetTestModeEnable(false);
                                    m_nNoResetBaseFunctionFlag_Gen8 = 1;
                                }
                                else
                                    m_nNoResetBaseFunctionFlag_Gen8 = 0;
                            }
                        }
                        else
                            SetTestModeEnable(false);
                    }
                    else
                    {
                        SetTestModeEnable(false);
                    }
                }

                if (bErrorFlag == true)
                {
                    OutputMessage(string.Format("-SetState:{0}, RetryIndex:{1}, Error:{2}, bSetParameterSuccess:{3}", eSetState.ToString(), nRetryIndex, bErrorFlag, bSetParameterSuccessFlag));

                    if (nRetryIndex == nRETRYLIMITTIMES)
                        break;
                    else
                        continue;
                }

                if (m_cfrmParent.m_bExecute == true)
                {
                    m_sErrorMessage = "";
                    bErrorFlag = false;
                }

                bSetParameterSuccessFlag = true;

                OutputDebugLog(string.Format("-SetState:{0}, RetryIndex:{1}, Error:{2}, bSetParameterSuccess:{3}", eSetState.ToString(), nRetryIndex, bErrorFlag, bSetParameterSuccessFlag));

                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER && m_bGetFirstDataFlag == true && nReKSuccessCount == 0)
                {
                    nReKSuccessCount++;
                    continue;
                }

                break;
            }

            return bSetParameterSuccessFlag;
        }

        private void SetUserDefinedCommand()
        {
            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eSelfTraceType);

                Thread.Sleep(1000);

                if ((ParamFingerAutoTuning.m_nCommandScriptType == 1 || ParamFingerAutoTuning.m_nCommandScriptType == 2) && m_cfrmParent.m_bExecute == true)
                {
                    if (m_cSendCommandInfo != null)
                        cElanCommand_Gen8.RunUserDefinedCommandScriptFlow(m_cSendCommandInfo);
                }
            }
        }
#else
        /// <summary>
        /// 設定並讀取韌體參數的主函式
        /// 支援多種流程步驟和IC世代，包含完整的重試機制和錯誤處理
        /// </summary>
        /// <param name="cFlowStep">流程步驟</param>
        /// <param name="cFrequencyItem">頻率項目參數</param>
        /// <param name="cRawADCSweepItem">Raw ADC掃描項目參數</param>
        /// <returns>設定參數是否成功</returns>
        private bool SetAndGetFWParameter(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem = null, RawADCSweepItem cRawADCSweepItem = null)
        {
            // 如果停用類比參數設定，直接複製參數值並返回
            if (ParamFingerAutoTuning.m_nDisableSetAnalogParameter == 1)
            {
                CopyParametersDirectly(cFlowStep, cFrequencyItem, cRawADCSweepItem);
                return true;
            }

            // 初始化重試相關變數
            const int nRETRYLIMITTIMES = 3;
            bool bSetParameterSuccessFlag = false;
            int nReKSuccessCount = 0;

            // 主要重試迴圈
            for (int nRetryIndex = 0; nRetryIndex <= nRETRYLIMITTIMES; nRetryIndex++)
            {
                if (!m_cfrmParent.m_bExecute)
                    break;

                // 執行單次設定嘗試
                var result = ExecuteSingleSetAttempt(cFlowStep, cFrequencyItem, cRawADCSweepItem, nRetryIndex, nRETRYLIMITTIMES, ref nReKSuccessCount);

                if (result.Success)
                {
                    bSetParameterSuccessFlag = true;
                    OutputDebugLog(string.Format("-SetState:{0}, RetryIndex:{1}, Error:{2}, bSetParameterSuccess:{3}",
                        result.SetState.ToString(), nRetryIndex, false, bSetParameterSuccessFlag));

                    // SSH Socket Server 特殊處理
                    if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER &&
                        m_bGetFirstDataFlag == true && nReKSuccessCount == 0)
                    {
                        nReKSuccessCount++;
                        continue;
                    }

                    break;
                }

                OutputMessage(string.Format("-SetState:{0}, RetryIndex:{1}, Error:{2}, bSetParameterSuccess:{3}",
                    result.SetState.ToString(), nRetryIndex, result.HasError, bSetParameterSuccessFlag));

                if (nRetryIndex == nRETRYLIMITTIMES)
                    break;
            }

            return bSetParameterSuccessFlag;
        }

        /// <summary>
        /// 單次設定嘗試的結果
        /// </summary>
        private class SetAttemptResult
        {
            public bool Success { get; set; }
            public bool HasError { get; set; }
            public AppCoreDefine.SetState SetState { get; set; }
        }

        /// <summary>
        /// 執行單次設定嘗試
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="cRawADCSweepItem"></param>
        /// <param name="nRetryIndex"></param>
        /// <param name="nRETRYLIMITTIMES"></param>
        /// <param name="nReKSuccessCount"></param>
        /// <returns></returns>
        private SetAttemptResult ExecuteSingleSetAttempt(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem,
            RawADCSweepItem cRawADCSweepItem, int nRetryIndex, int nRETRYLIMITTIMES, ref int nReKSuccessCount)
        {
            m_cReadParameter.Initialize();

            bool bErrorFlag = false;
            bool bLastRetryFlag = (nRetryIndex == nRETRYLIMITTIMES);
            int nRetryIncreaseTime = nRetryIndex * 100;
            AppCoreDefine.SetState eSetState = AppCoreDefine.SetState.Initial;

            // 重試時重新連接（除了特殊情況）
            if (nRetryIndex > 0 && m_eICGenerationType != ICGenerationType.Gen9 &&
                !(ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER &&
                  nReKSuccessCount > 0 && m_bGetFirstDataFlag == true))
            {
                if (!ReconnectAndInitialize())
                    return new SetAttemptResult { Success = false, HasError = true, SetState = eSetState };
            }

            // 內部設定重試迴圈
            for (int nSetIndex = 0; nSetIndex < nRETRYLIMITTIMES; nSetIndex++)
            {
                // Reset狀態處理
                if (eSetState == AppCoreDefine.SetState.Reset)
                {
                    if (!SetFrontState())
                    {
                        bErrorFlag = true;
                        break;
                    }
                }

                // Gen8 前置處理
                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    PrepareGen8Setting(cFlowStep, nRetryIncreaseTime);
                }

                // 根據流程步驟設定參數
                eSetState = SetParametersByFlowStep(cFlowStep, cFrequencyItem, cRawADCSweepItem, nRetryIncreaseTime);

                if (!m_cfrmParent.m_bExecute)
                {
                    bErrorFlag = true;
                    break;
                }

                // 讀取並驗證參數
                if (!ValidateParametersSetting(cFlowStep, cFrequencyItem, cRawADCSweepItem, bLastRetryFlag, ref eSetState))
                {
                    OutputMessage(string.Format("-SetState:{0}, RetryIndex:{1}, SetIndex:{2}, Error:{3}",
                        eSetState.ToString(), nRetryIndex, nSetIndex, bErrorFlag));

                    if (nSetIndex == nRETRYLIMITTIMES - 1)
                    {
                        m_sErrorMessage = "Set Analog Parameter Fail";
                        bErrorFlag = true;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                else
                {
                    OutputDebugLog(string.Format("-SetState:{0}, RetryIndex:{1}, SetIndex:{2}, Error:{3}",
                        eSetState.ToString(), nRetryIndex, nSetIndex, bErrorFlag));
                }

                break;
            }

            if (bErrorFlag)
            {
                return new SetAttemptResult { Success = false, HasError = true, SetState = eSetState };
            }

            // Gen8 後續處理
            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                if (!ProcessGen8PostSetting(cFlowStep, ref eSetState, ref bErrorFlag))
                {
                    return new SetAttemptResult { Success = false, HasError = true, SetState = eSetState };
                }
            }

            if (bErrorFlag)
            {
                return new SetAttemptResult { Success = false, HasError = true, SetState = eSetState };
            }

            if (m_cfrmParent.m_bExecute)
            {
                m_sErrorMessage = "";
                bErrorFlag = false;
            }

            return new SetAttemptResult { Success = true, HasError = false, SetState = eSetState };
        }

        /// <summary>
        /// 停用參數設定時直接複製參數值
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="cRawADCSweepItem"></param>
        private void CopyParametersDirectly(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem, RawADCSweepItem cRawADCSweepItem)
        {
            if (m_eICGenerationType == ICGenerationType.Gen9)
            {
                m_cReadParameter.m_nPH1 = cFrequencyItem.m_nPH1;
                m_cReadParameter.m_nPH2 = cFrequencyItem.m_nPH2;
                m_cReadParameter.m_nPH3 = cFrequencyItem.m_nPH2;
                m_cReadParameter.m_n_MS_DFT_NUM = cFrequencyItem.m_nDFT_NUM;
            }
            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                m_cReadParameter.m_n_MS_SELC = cRawADCSweepItem.m_nSELC;
                m_cReadParameter.m_n_MS_VSEL = cRawADCSweepItem.m_nVSEL;
                m_cReadParameter.m_n_MS_LG = cRawADCSweepItem.m_nLG;
                m_cReadParameter.m_n_MS_SELGM = cRawADCSweepItem.m_nSELGM;
            }
            else if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                m_cReadParameter.m_n_SELF_PH1 = cFrequencyItem.m_n_SELF_PH1;
                m_cReadParameter.m_n_SELF_PH2E_LAT = cFrequencyItem.m_n_SELF_PH2E_LAT;
                m_cReadParameter.m_n_SELF_PH2E_LMT = cFrequencyItem.m_n_SELF_PH2E_LMT;
                m_cReadParameter.m_n_SELF_PH2_LAT = cFrequencyItem.m_n_SELF_PH2_LAT;
                m_cReadParameter.m_n_SELF_PH2 = cFrequencyItem.m_n_SELF_PH2;
                m_cReadParameter.m_nSelf_DFT_NUM = m_cSelfParameter.m_nDFT_NUM;
                m_cReadParameter.m_nSelf_Gain = m_cSelfParameter.m_nGain;
                m_cReadParameter.m_nSelf_CAG = m_cSelfParameter.m_nCAG;
                m_cReadParameter.m_nSelf_IQ_BSH = m_cSelfParameter.m_nIQ_BSH;
            }
            else
            {
                m_cReadParameter.m_nPH1 = cFrequencyItem.m_nPH1;
                m_cReadParameter.m_nPH2 = cFrequencyItem.m_nPH2;
                m_cReadParameter.m_nPH3 = cFrequencyItem.m_nPH2;
                m_cReadParameter.m_n_MS_DFT_NUM = cFrequencyItem.m_nDFT_NUM;
            }
        }

        /// <summary>
        /// 重新連接並初始化
        /// </summary>
        private bool ReconnectAndInitialize()
        {
            if (!ConnectToTP())
                return false;

            SetReportEnable(false);

            // Get And Set Project_Option, FWIP_Option
            short nOriginProjectOption = 0;
            short nOriginFWIPOption = 0;
            short nReadFWIPOption = 0;

            if (!GetAndSetOptions(ref nOriginProjectOption, ref nOriginFWIPOption, ref nReadFWIPOption))
                return false;

            return true;
        }

        /// <summary>
        /// 準備Gen8設定
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="nRetryIncreaseTime"></param>
        private void PrepareGen8Setting(frmMain.FlowStep cFlowStep, int nRetryIncreaseTime)
        {
            SetTestModeEnable(true);
            Thread.Sleep(nRetryIncreaseTime);

            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data, null, true);
            else
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data);

            Thread.Sleep(nRetryIncreaseTime);
        }

        /// <summary>
        /// 根據流程步驟設定參數
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="cRawADCSweepItem"></param>
        /// <param name="nRetryIncreaseTime"></param>
        /// <returns></returns>
        private AppCoreDefine.SetState SetParametersByFlowStep(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem,
            RawADCSweepItem cRawADCSweepItem, int nRetryIncreaseTime)
        {
            // Self 模式參數設定
            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                return SetSelfParameters(cFlowStep, cFrequencyItem, nRetryIncreaseTime);
            }
            // Raw ADC Sweep 參數設定
            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                return SetRawADCSweepParameters(cFlowStep, cRawADCSweepItem, nRetryIncreaseTime);
            }
            // 一般參數設定
            else
            {
                return SetNormalParameters(cFlowStep, cFrequencyItem, nRetryIncreaseTime);
            }
        }

        /// <summary>
        /// 設定Self模式參數
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="nRetryIncreaseTime"></param>
        /// <returns></returns>
        private AppCoreDefine.SetState SetSelfParameters(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem, int nRetryIncreaseTime)
        {
            AppCoreDefine.FWParameter cFWParameter = new AppCoreDefine.FWParameter();
            cFWParameter.m_n_SELF_PH1 = cFrequencyItem.m_n_SELF_PH1;
            cFWParameter.m_n_SELF_PH2E_LAT = cFrequencyItem.m_n_SELF_PH2E_LAT;
            cFWParameter.m_n_SELF_PH2E_LMT = cFrequencyItem.m_n_SELF_PH2E_LMT;
            cFWParameter.m_n_SELF_PH2_LAT = cFrequencyItem.m_n_SELF_PH2E_LMT;
            cFWParameter.m_n_SELF_PH2_MUX_LAT = m_cOriginParameter.m_n_SELF_PH2_MUX_LAT;
            cFWParameter.m_n_SELF_PH2 = cFrequencyItem.m_n_SELF_PH2;
            cFWParameter.m_nSelf_DFT_NUM = m_cSelfParameter.m_nDFT_NUM;
            cFWParameter.m_nSelf_Gain = m_cSelfParameter.m_nGain;
            cFWParameter.m_nSelf_CAG = m_cSelfParameter.m_nCAG;
            cFWParameter.m_nSelf_IQ_BSH = m_cSelfParameter.m_nIQ_BSH;

            string sSetKSequence = MainConstantParameter.m_sKSequence_NA;
            bool bSetCALValue = false;
            int nCALStartTrace = -1;
            int nCALStartTraceNumber = 0;
            int nCALEndTraceNumber = 0;

            // Self_FrequencySweep 特殊處理
            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep)
            {
                if (ParamFingerAutoTuning.m_nSelfFSRunKSequence == 1 || ParamFingerAutoTuning.m_nSelfFSRunKSequence == 2)
                    sSetKSequence = MainConstantParameter.m_sKSequence_FixedValue;
                else if (ParamFingerAutoTuning.m_nSelfFSRunKSequence == 3)
                    sSetKSequence = MainConstantParameter.m_sKSequence_FileValue;

                if (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2)
                {
                    bSetCALValue = true;

                    if (ParamFingerAutoTuning.m_nSelfFSRunCALData == 2)
                    {
                        nCALStartTrace = ParamFingerAutoTuning.m_nSelfFSCALStartTrace;
                        nCALStartTraceNumber = ParamFingerAutoTuning.m_nSelfFSCALStartTraceNumber;
                        nCALEndTraceNumber = ParamFingerAutoTuning.m_nSelfFSCALEndTraceNumber;
                    }
                }
            }
            // Self_NCPNCNSweep 特殊處理
            else if (cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                sSetKSequence = MainConstantParameter.m_sKSequence_FixedValue;
            }

            return SetFWParameter(cFlowStep, cFWParameter, ParamFingerAutoTuning.m_nReKTimeout,
                bSetSelfParameter: true, sSetSelfKSequence: sSetKSequence, bSetSelfCALValue: bSetCALValue,
                nSelfCALStartTrace: nCALStartTrace, nSelfCALStartTraceNumber: nCALStartTraceNumber,
                nSelfCALEndTraceNumber: nCALEndTraceNumber, nRetryIncreaseTime: nRetryIncreaseTime);
        }

        /// <summary>
        /// 設定Raw ADC Sweep參數
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cRawADCSweepItem"></param>
        /// <param name="nRetryIncreaseTime"></param>
        /// <returns></returns>
        private AppCoreDefine.SetState SetRawADCSweepParameters(frmMain.FlowStep cFlowStep, RawADCSweepItem cRawADCSweepItem, int nRetryIncreaseTime)
        {
            AppCoreDefine.FWParameter cFWParameter = new AppCoreDefine.FWParameter();

            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                cFWParameter.m_n_MS_SELC = cRawADCSweepItem.m_nSELC;
                cFWParameter.m_n_MS_VSEL = cRawADCSweepItem.m_nVSEL;
                cFWParameter.m_n_MS_LG = cRawADCSweepItem.m_nLG;
                cFWParameter.m_n_MS_SELGM = cRawADCSweepItem.m_nSELGM;
                cFWParameter.m_n_MS_FIRCOEF_SEL = ParamFingerAutoTuning.m_nRawADCSGen8FixedFIRCOEF_SEL;
                cFWParameter.m_n_MS_FIR_TAP_NUM = ParamFingerAutoTuning.m_nRawADCSFixedFIR_TAP_NUM;
                cFWParameter.m_n_MS_DFT_NUM = m_cOriginParameter.m_n_MS_DFT_NUM;
                cFWParameter.m_n_MS_IQ_BSH = m_cOriginParameter.m_n_MS_IQ_BSH;
            }
            else if (m_eICGenerationType == ICGenerationType.Gen7)
            {
                cFWParameter.m_n_MS_SELC = cRawADCSweepItem.m_nSELC;
                cFWParameter.m_n_MS_VSEL = cRawADCSweepItem.m_nVSEL;
                cFWParameter.m_n_MS_LG = cRawADCSweepItem.m_nLG;
                cFWParameter.m_n_MS_SELGM = cRawADCSweepItem.m_nSELGM;
                cFWParameter.m_n_MS_FIRTB = ParamFingerAutoTuning.m_nRawADCSGen6or7FixedFIRTB;
                cFWParameter.m_n_MS_FIR_TAP_NUM = ParamFingerAutoTuning.m_nRawADCSFixedFIR_TAP_NUM;
                cFWParameter.m_n_MS_DFT_NUM = m_cOriginParameter.m_n_MS_DFT_NUM;
                cFWParameter.m_n_MS_IQ_BSH = m_cOriginParameter.m_n_MS_IQ_BSH;
            }
            else if (m_eICGenerationType == ICGenerationType.Gen6)
            {
                cFWParameter.m_n_MS_SELC = cRawADCSweepItem.m_nSELC;
                cFWParameter.m_n_MS_VSEL = cRawADCSweepItem.m_nVSEL;
                cFWParameter.m_n_MS_LG = cRawADCSweepItem.m_nLG;
                cFWParameter.m_n_MS_SELGM = cRawADCSweepItem.m_nSELGM;
                cFWParameter.m_n_MS_FIRTB = ParamFingerAutoTuning.m_nRawADCSGen6or7FixedFIRTB;
                cFWParameter.m_n_MS_FIR_TAP_NUM = ParamFingerAutoTuning.m_nRawADCSFixedFIR_TAP_NUM;
                cFWParameter.m_n_MS_DFT_NUM = m_cOriginParameter.m_n_MS_DFT_NUM;
                cFWParameter.m_n_MS_IQ_BSH = m_cOriginParameter.m_n_MS_IQ_BSH;
            }

            return SetFWParameter(cFlowStep, cFWParameter, ParamFingerAutoTuning.m_nReKTimeout, nRetryIncreaseTime: nRetryIncreaseTime);
        }

        /// <summary>
        /// 設定一般參數
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="nRetryIncreaseTime"></param>
        /// <returns></returns>
        private AppCoreDefine.SetState SetNormalParameters(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem, int nRetryIncreaseTime)
        {
            if (m_eICGenerationType != ICGenerationType.Gen9)
            {
                AppCoreDefine.FWParameter cFWParameter = new AppCoreDefine.FWParameter();
                cFWParameter.m_nPH1 = cFrequencyItem.m_nPH1;
                cFWParameter.m_nPH2 = cFrequencyItem.m_nPH2;
                cFWParameter.m_nPH3 = cFrequencyItem.m_nPH2;
                cFWParameter.m_n_MS_DFT_NUM = cFrequencyItem.m_nDFT_NUM;

                return SetFWParameter(cFlowStep, cFWParameter, ParamFingerAutoTuning.m_nReKTimeout, nRetryIncreaseTime: nRetryIncreaseTime);
            }
#if _USE_9F07_SOCKET
            else
            {
                AppCoreDefine.FWParameter cFWParameter = new AppCoreDefine.FWParameter();
                cFWParameter.m_nPH1 = cFrequencyItem.m_nPH1;
                cFWParameter.m_nPH2 = cFrequencyItem.m_nPH2;
                cFWParameter.m_nPH3 = cFrequencyItem.m_nPH2;
                cFWParameter.m_n_MS_DFT_NUM = cFrequencyItem.m_nDFT_NUM;

                return SetFWParameter_9F07(cFWParameter, nRetryIncreaseTime: nRetryIncreaseTime);
            }
#else
            else
            {
                return AppCoreDefine.SetState.Error;
            }
#endif
        }

        /// <summary>
        /// 驗證參數設定
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFrequencyItem"></param>
        /// <param name="cRawADCSweepItem"></param>
        /// <param name="bLastRetryFlag"></param>
        /// <param name="eSetState"></param>
        /// <returns></returns>
        private bool ValidateParametersSetting(frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem,
            RawADCSweepItem cRawADCSweepItem, bool bLastRetryFlag, ref AppCoreDefine.SetState eSetState)
        {
            if (m_eICGenerationType != ICGenerationType.Gen9)
            {
                if ((m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nCommandScriptType != 2) ||
                    m_eICGenerationType == ICGenerationType.Gen7 ||
                    m_eICGenerationType == ICGenerationType.Gen6 ||
                    m_eICGenerationType == ICGenerationType.Other)
                {
                    // Gen8 特殊處理: 切換測試模式後重新讀取
                    if (m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nGen8GetAnalogAfterSwitchTestMode == 1)
                    {
                        SetTestModeEnable(false);
                        Thread.Sleep(100);
                        SetTestModeEnable(true);
                    }

                    // 讀取參數
                    if (!GetFWParameter(cFlowStep))
                        eSetState = AppCoreDefine.SetState.Error;

                    // 驗證參數
                    if (eSetState == AppCoreDefine.SetState.Success)
                    {
                        if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                        {
                            if (!CheckAnalogParameter(cFlowStep, bLastRetryFlag, null, cRawADCSweepItem))
                                eSetState = AppCoreDefine.SetState.Error;
                        }
                        else
                        {
                            if (!CheckAnalogParameter(cFlowStep, bLastRetryFlag, cFrequencyItem))
                                eSetState = AppCoreDefine.SetState.Error;
                        }
                    }
                }
            }
#if _USE_9F07_SOCKET
            else
            {
                bool bResultFlag = GetFWParameter(cFlowStep);

                if (!bResultFlag)
                {
                    eSetState = AppCoreDefine.SetState.Error;
                    //return false;
                }
                else
                {
                    if (!CheckAnalogParameter(cFlowStep, bLastRetryFlag, cFrequencyItem))
                        eSetState = AppCoreDefine.SetState.Error;
                }
            }
#endif

            return eSetState == AppCoreDefine.SetState.Success;
        }

        /// <summary>
        /// 處理Gen8後續設定
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="eSetState"></param>
        /// <param name="bErrorFlag"></param>
        /// <returns></returns>
        private bool ProcessGen8PostSetting(frmMain.FlowStep cFlowStep, ref AppCoreDefine.SetState eSetState, ref bool bErrorFlag)
        {
            /*
            OutputMessage("-Disable Report");
            //ElanTouch.DisableTPReport(true, 1000, m_nDeviceIndex);
            ElanTouchSwitch.DisableTPReport(true, m_nDeviceIndex, m_bSocketConnectType);
            m_bDisableReport = true;
            Thread.Sleep(m_nNormalDelayTime);
            */

            /*
            if (m_eICGenerationType != ICGenerationType.Gen9)
            {
                if ((m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nCommandScriptType != 2) ||
                    m_eICGenerationType == ICGenerationType.Gen7 ||
                    m_eICGenerationType == ICGenerationType.Gen6 ||
                    m_eICGenerationType == ICGenerationType.Other)
                {
                    if (m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nGen8GetAnalogAfterSwitchTestMode == 1)
                    {
                        SetTestModeEnable(false);
                        Thread.Sleep(100);
                        SetTestModeEnable(true);
                    }

                    if (GetFWParameter(cFlowStep) == false)
                        return false;

                    if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                    {
                        if (CheckAnalogParameter(cFlowStep, bLastRetryFlag, null, cRawADCSweepItem) == false)
                            bErrorFlag = true;
                    }
                    else
                    {
                        if (CheckAnalogParameter(cFlowStep, bLastRetryFlag, cFrequencyItem) == false)
                            bErrorFlag = true;
                    }
                }
            }
#if _USE_9F07_SOCKET
            else
            {
                bool bResultFlag = GetFWParameter(cFlowStep);

                if (bResultFlag == false)
                {
                    bErrorFlag = true;
                    //return false;
                }
                else
                {
                    if (CheckAnalogParameter(cFlowStep, bLastRetryFlag, cFrequencyItem) == false)
                        bErrorFlag = true;
                }
            }
#endif
            */

            // 設定使用者自訂命令
            if (eSetState == AppCoreDefine.SetState.Success)
                SetUserDefinedCommand();

            // 設定ReK
            if (eSetState == AppCoreDefine.SetState.Success)
                eSetState = SetReK(ParamFingerAutoTuning.m_nReKTimeout);

            // 根據流程步驟處理UpdateBase
            if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2 || cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
            {
                return ProcessUpdateBase(ref bErrorFlag);
            }
            else
            {
                SetTestModeEnable(false);
            }

            return true;
        }

        /// <summary>
        /// 處理UpdateBase相關設定
        /// </summary>
        /// <param name="bErrorFlag"></param>
        /// <returns></returns>
        private bool ProcessUpdateBase(ref bool bErrorFlag)
        {
            if (ParamFingerAutoTuning.m_nGen8GetBaseType == 2 || ParamFingerAutoTuning.m_nGen8GetBaseType == 3)
            {
                // Type 2或3: 只關閉測試模式
                SetTestModeEnable(false);
            }
            else if (ParamFingerAutoTuning.m_nGen8SetUpdateBaseType == 1)
            {
                // Type 1: 快速更新Base後關閉測試模式
                SetFast_UpdateBase_Gen8();
                SetTestModeEnable(false);
            }
            else if (ParamFingerAutoTuning.m_nGen8SetUpdateBaseType == 2)
            {
                // Type 2: 關閉測試模式、關閉報告、快速更新Base
                SetTestModeEnable(false);
                SetReportEnable(false);
                SetFast_UpdateBase_Gen8();
            }
            else if (ParamFingerAutoTuning.m_nGen8SetUpdateBaseType == 3)
            {
                // Type 3: 快速更新Base(不延遲)、關閉測試模式、等待
                SetFast_UpdateBase_Gen8(bSetUpdateBaseDelayTime: false);
                SetTestModeEnable(false);
                Thread.Sleep(ParamFingerAutoTuning.m_nGen8UpdateBaseDelayTime);
            }
            else if (ParamFingerAutoTuning.m_nGen8SetUpdateBaseType == 4)
            {
                // Type 4: 快速更新Base、關閉測試模式、關閉報告、再次更新Base
                SetFast_UpdateBase_Gen8();
                SetTestModeEnable(false);
                SetReportEnable(false);
                Thread.Sleep(100);
                SetFast_UpdateBase_Gen8();
            }
            else if (ParamFingerAutoTuning.m_nGen8SetUpdateBaseType == 5)
            {
                // Type 5: Reset Base或關閉測試模式
                if (m_nNoResetBaseFunctionFlag_Gen8 == 1)
                {
                    SetTestModeEnable(false);
                }
                else
                {
                    int nResetBaseFlag = SetReset_Base_Gen8(bSetUpdateBaseDelayTime: false);

                    if (nResetBaseFlag == 0)
                    {
                        bErrorFlag = true;
                    }
                    else if (nResetBaseFlag == -1)
                    {
                        SetTestModeEnable(false);
                        m_nNoResetBaseFunctionFlag_Gen8 = 1;
                    }
                    else
                    {
                        m_nNoResetBaseFunctionFlag_Gen8 = 0;
                    }
                }
            }
            else
            {
                // 預設: 只關閉測試模式
                SetTestModeEnable(false);
            }

            return true;
        }

        /// <summary>
        /// 執行使用者自訂命令腳本
        /// </summary>
        /// <remarks>
        /// 執行條件與流程：
        /// <list type="number">
        /// <item>僅支援 Gen8 IC 世代</item>
        /// <item>延遲 1 秒後開始執行</item>
        /// <item>檢查命令腳本類型是否為 1 或 2（使用者自訂腳本模式）</item>
        /// <item>確認執行狀態 (m_bExecute) 為 true</item>
        /// <item>若命令資訊物件 (m_cSendCommandInfo) 存在，執行使用者自訂命令腳本流程</item>
        /// </list>
        /// <para>此方法允許使用者透過腳本檔案自訂 Gen8 IC 的命令序列，提供彈性的參數設定方式</para>
        /// <para>命令腳本類型說明：0=標準模式, 1=使用者自訂腳本, 2=進階自訂腳本</para>
        /// <para>非 Gen8 世代會跳過此方法，不執行任何操作</para>
        /// </remarks>
        private void SetUserDefinedCommand()
        {
            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eSelfTraceType);

                Thread.Sleep(1000);

                if ((ParamFingerAutoTuning.m_nCommandScriptType == 1 || ParamFingerAutoTuning.m_nCommandScriptType == 2) && m_cfrmParent.m_bExecute == true)
                {
                    if (m_cSendCommandInfo != null)
                        cElanCommand_Gen8.RunUserDefinedCommandScriptFlow(m_cSendCommandInfo);
                }
            }
        }

        /// <summary>
        /// 設定前置狀態（準備階段的初始化設定）
        /// </summary>
        /// <remarks>
        /// 執行流程：
        /// <list type="number">
        /// <item>停用 Report 功能 (SetReportEnable(false))</item>
        /// <item>取得並設定專案選項 (Project_Option) 和韌體 IP 選項 (FWIP_Option)
        ///   <list type="bullet">
        ///   <item>讀取原始的 Project Option 值</item>
        ///   <item>讀取原始的 FWIP Option 值</item>
        ///   <item>讀取並驗證 FWIP Option</item>
        ///   </list>
        /// </item>
        /// </list>
        /// <para>此方法用於在開始測試或校正流程前，確保裝置處於正確的初始狀態</para>
        /// <para>若取得或設定選項失敗，會返回 false 中止後續流程</para>
        /// </remarks>
        /// <returns>true: 前置狀態設定成功; false: 設定失敗</returns>
        private bool SetFrontState()
        {
            SetReportEnable(false);

            //Get And Set Project_Option, FWIP_Option
            short nOriginProjectOption = 0;
            short nOriginFWIPOption = 0;
            short nReadFWIPOption = 0;

            if (GetAndSetOptions(ref nOriginProjectOption, ref nOriginFWIPOption, ref nReadFWIPOption) == false)
                return false;

            return true;
        }
#endif
    }
}
