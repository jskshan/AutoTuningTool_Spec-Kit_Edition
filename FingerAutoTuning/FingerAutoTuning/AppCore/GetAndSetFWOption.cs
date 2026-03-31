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
        private bool GetAndSetOptions(ref short nOriginProjectOption, ref short nOriginFWIPOption, ref short nReadFWIPOption)
        {
#if _USE_9F07_SOCKET
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return true;
#endif

            if (ParamFingerAutoTuning.m_nDisableSetFWOption == 1)
                return true;

            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                nOriginProjectOption = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                OutputMessage(string.Format("-Get _Project_Option(0x53, 0xB1, 0x00, 0x01) Value=0x{0}", nOriginProjectOption.ToString("X4")));

                int nValue = 0;

                nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                nOriginFWIPOption = (short)nValue;

                OutputMessage(string.Format("-Get _FWIP_Option(0x53, 0xC1, 0x00, 0x01) Value=0x{0}", nOriginFWIPOption.ToString("X4")));

                int nRetryLimitCount = 4;

                for (int nRetryIndex = 0; nRetryIndex <= nRetryLimitCount; nRetryIndex++)
                {
                    //Disable the self partial
                    short nSetProjectOption = (short)(nOriginProjectOption & 0xFFF7);

                    int nProjectOption_DisableTxn_Scan = (ParamFingerAutoTuning.m_nDisableTxn_Scan == 1) ? 0xFFFB : 0xFFFF;
                    nSetProjectOption = (short)(nSetProjectOption & nProjectOption_DisableTxn_Scan);

                    if (ParamFingerAutoTuning.m_nDisableIdleMode == 1)
                    {
                        int nProjectOption_DisableIdleMode = 0xFFFE;
                        nSetProjectOption = (short)(nSetProjectOption & nProjectOption_DisableIdleMode);
                    }

                    if (nSetProjectOption != nOriginProjectOption)
                    {
                        OutputMessage(string.Format("-Set _Project_Option(0x54, 0xB1, 0x{0}, 0x{1}) Value=0x{2}", ((nSetProjectOption & 0xFF00) >> 8).ToString("X2"), (nSetProjectOption & 0x00FF).ToString("X2"), nSetProjectOption.ToString("X4")));
                        ElanTouchSwitch.SetOption(ElanTouchSwitch.m_nOPTION_PROJECTOPTION, nSetProjectOption, m_nDeviceIndex, m_bSocketConnectType);
                        Thread.Sleep(m_nNormalDelayTime);
                    }

                    short nSetFWIPOption = (short)(nOriginFWIPOption & 0xFFFF);

                    int nFWIPOption_DisableOBLNoise = 0xFFBF;
                    nSetFWIPOption = (short)(nSetFWIPOption & nFWIPOption_DisableOBLNoise);

                    if (ParamFingerAutoTuning.m_nDisableUpdateBase == 1)
                    {
                        int nFWIPOption_DisableUpdateBase = 0xFFEF;
                        nSetFWIPOption = (short)(nSetFWIPOption & nFWIPOption_DisableUpdateBase);
                    }

                    if (nSetFWIPOption != nOriginFWIPOption)
                    {
                        OutputMessage(string.Format("-Set _FWIP_Option(0x54, 0xC1, 0x{0}, 0x{1}) Value=0x{2}", ((nSetFWIPOption & 0xFF00) >> 8).ToString("X2"), (nSetFWIPOption & 0x00FF).ToString("X2"), nSetFWIPOption.ToString("X4")));
                        ElanTouchSwitch.SetOption(ElanTouchSwitch.m_nOPTION_FWIPOPTION, nSetFWIPOption, m_nDeviceIndex, m_bSocketConnectType);
                        Thread.Sleep(m_nNormalDelayTime);
                    }

                    short nReadProjectOption = 0;

                    if (nSetProjectOption != nOriginProjectOption)
                    {
                        nReadProjectOption = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                        OutputMessage(string.Format("-Read _Project_Option(0x53, 0xB1, 0x00, 0x01) Value=0x{0}", nReadProjectOption.ToString("X4")));
                        m_nReadProjectOption = nReadProjectOption;
                    }
                    else
                        m_nReadProjectOption = nOriginProjectOption;

                    nValue = 0;

                    if (nSetFWIPOption != nOriginFWIPOption)
                    {
                        nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                        nReadFWIPOption = (short)nValue;

                        OutputMessage(string.Format("-Read _FWIP_Option(0x53, 0xC1, 0x00, 0x01) Value=0x{0}", nReadFWIPOption.ToString("X4")));
                        m_nReadFWIPOption = nReadFWIPOption;
                    }
                    else
                    {
                        nReadFWIPOption = nOriginFWIPOption;
                        m_nReadFWIPOption = nOriginFWIPOption;
                    }

                    if (nSetProjectOption != nOriginProjectOption)
                    {
                        if (nSetProjectOption != nReadProjectOption)
                        {
                            if (nRetryIndex == nRetryLimitCount)
                            {
                                m_sErrorMessage = "Set _Project_Option Error";
                                return false;
                            }
                            else
                                continue;
                        }
                    }

                    if (nSetFWIPOption != nOriginFWIPOption)
                    {
                        if (nSetFWIPOption != nReadFWIPOption)
                        {
                            if (nRetryIndex == nRetryLimitCount)
                            {
                                m_sErrorMessage = "Set _FWIP_Option Error";
                                return false;
                            }
                            else
                                continue;
                        }
                    }

                    break;
                }
            }
            else
            {
                if (ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_USB)
                    //nOriginProjectOption = (short)ElanTouch.GetProjOption(1000, m_nDeviceIndex);
                    nOriginProjectOption = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                else
                {
                    /*
                    byte[] byteCommand_Array = new byte[] { 0x53, 0xB1, 0x00, 0x01, 0x00, 0x00 };
                    nOriginProjectOption = (short)xSendCmdCheck(byteCommand_Array, 0xB1);
                    */
                    //nOriginProjectOption = (short)ElanTouch.GetProjOption(1000, m_nDeviceIndex);
                    nOriginProjectOption = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                }

                OutputMessage(string.Format("-Get _Project_Option(0x53, 0xB1, 0x00, 0x01) Value=0x{0}", nOriginProjectOption.ToString("X4")));

                int nValue = 0;

                if (ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_USB)
                {
                    //nOriginFWIPOption = (short)ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDevIdx);
                    //ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDeviceIndex);
                    nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                    nOriginFWIPOption = (short)nValue;
                }
                else
                {
                    /*
                    byte[] byteCommand_Array = new byte[] { 0x53, 0xC1, 0x00, 0x01, 0x00, 0x00 };
                    nOriginFWIPOption = (short)xSendCmdCheck(byteCommand_Array, 0xC1);
                    */
                    //ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDeviceIndex);
                    nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                    nOriginFWIPOption = (short)nValue;
                }

                OutputMessage(string.Format("-Get _FWIP_Option(0x53, 0xC1, 0x00, 0x01) Value=0x{0}", nOriginFWIPOption.ToString("X4")));

                int nRetryLimitCount = 4;

                for (int nRetryIndex = 0; nRetryIndex <= nRetryLimitCount; nRetryIndex++)
                {
                    //Disable the self partial
                    short nSetProjectOption = (short)(nOriginProjectOption & 0xFFBF);

                    if (ParamFingerAutoTuning.m_nDisableIdleMode == 1)
                    {
                        int nProjectOption_DisableIdleMode = 0xFF7F;
                        nSetProjectOption = (short)(nSetProjectOption & nProjectOption_DisableIdleMode);
                    }

                    if (nSetProjectOption != nOriginProjectOption)
                    {
                        OutputMessage(string.Format("-Set _Project_Option(0x54, 0xB1, 0x{0}, 0x{1}) Value=0x{2}", ((nSetProjectOption & 0xFF00) >> 8).ToString("X2"), (nSetProjectOption & 0x00FF).ToString("X2"), nSetProjectOption.ToString("X4")));
                        //ElanTouch.SetProjOption(nSetProjOption, 1000, m_nDeviceIndex);
                        ElanTouchSwitch.SetOption(ElanTouchSwitch.m_nOPTION_PROJECTOPTION, nSetProjectOption, m_nDeviceIndex, m_bSocketConnectType);
                        Thread.Sleep(m_nNormalDelayTime);
                    }

                    int nFWIPOption_DisableTxn_Scan = (ParamFingerAutoTuning.m_nDisableTxn_Scan == 1) ? 0xFEFF : 0xFFFF;
                    short nSetFWIPOption = (short)(nOriginFWIPOption & nFWIPOption_DisableTxn_Scan);

                    int nFWIPOption_DisableOBLNoise = 0xFFBF;
                    nSetFWIPOption = (short)(nSetFWIPOption & nFWIPOption_DisableOBLNoise);

                    if (ParamFingerAutoTuning.m_nDisableUpdateBase == 1)
                    {
                        int nFWIPOption_DisableUpdateBase = 0xFFEF;
                        nSetFWIPOption = (short)(nSetFWIPOption & nFWIPOption_DisableUpdateBase);
                    }

                    if (ParamFingerAutoTuning.m_nDisablePen_Scan == 1)
                    {
                        int nFWIPOption_DisablePen_Scan = 0xFDFF;
                        nSetFWIPOption = (short)(nSetFWIPOption & nFWIPOption_DisablePen_Scan);
                    }

                    if (ParamFingerAutoTuning.m_nDisableAddADCOffset == 1)
                    {
                        int nFWIPOption_DisableAddADCOffset = 0xEFFF;
                        nSetFWIPOption = (short)(nSetFWIPOption & nFWIPOption_DisableAddADCOffset);
                    }

                    if (nSetFWIPOption != nOriginFWIPOption)
                    {
                        OutputMessage(string.Format("-Set _FWIP_Option(0x54, 0xC1, 0x{0}, 0x{1}) Value=0x{2}", ((nSetFWIPOption & 0xFF00) >> 8).ToString("X2"), (nSetFWIPOption & 0x00FF).ToString("X2"), nSetFWIPOption.ToString("X4")));
                        //ElanTouch.SetFWIPOption(nSetFWIPOption, 1000, m_nDeviceIndex);
                        ElanTouchSwitch.SetOption(ElanTouchSwitch.m_nOPTION_FWIPOPTION, nSetFWIPOption, m_nDeviceIndex, m_bSocketConnectType);
                        Thread.Sleep(m_nNormalDelayTime);

                        //Disable OBL
                        //OutputMessage("-Disable OBL");
                        //ElanTouch.DisableAlgorithm(true, 1000, m_nDeviceIndex);
                    }

                    short nReadProjectOption = 0;

                    if (nSetProjectOption != nOriginProjectOption)
                    {
                        if (ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_USB)
                            //nReadProjectOption = (short)ElanTouch.GetProjOption(1000, m_nDeviceIndex);
                            nReadProjectOption = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                        else
                        {
                            /*
                            byte[] byteCommand_Array = new byte[] { 0x53, 0xB1, 0x00, 0x01, 0x00, 0x00 };
                            nReadProjectOption = (short)xSendCmdCheck(byteCommand_Array, 0xB1);
                            */
                            //nReadProjectOption = (short)ElanTouch.GetProjOption(1000, m_nDeviceIndex);
                            nReadProjectOption = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                        }

                        OutputMessage(string.Format("-Read _Project_Option(0x53, 0xB1, 0x00, 0x01) Value=0x{0}", nReadProjectOption.ToString("X4")));
                        m_nReadProjectOption = nReadProjectOption;
                    }
                    else
                        m_nReadProjectOption = nOriginProjectOption;

                    nValue = 0;

                    if (nSetFWIPOption != nOriginFWIPOption)
                    {
                        if (ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_USB)
                        {
                            //nReadFWIPOption = (short)ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDeviceIndex);
                            //ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDeviceIndex);
                            nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                            nReadFWIPOption = (short)nValue;
                        }
                        else
                        {
                            /*
                            byte[] byteCommand_Array = new byte[] { 0x53, 0xC1, 0x00, 0x01, 0x00, 0x00 };
                            nReadFWIPOption = (short)xSendCmdCheck(byteCommand_Array, 0xC1);
                            */
                            //ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDeviceIndex);
                            nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                            nReadFWIPOption = (short)nValue;
                        }

                        OutputMessage(string.Format("-Read _FWIP_Option(0x53, 0xC1, 0x00, 0x01) Value=0x{0}", nReadFWIPOption.ToString("X4")));
                        m_nReadFWIPOption = nReadFWIPOption;
                    }
                    else
                    {
                        nReadFWIPOption = nOriginFWIPOption;
                        m_nReadFWIPOption = nOriginFWIPOption;
                    }

                    if (nSetProjectOption != nOriginProjectOption)
                    {
                        if (nSetProjectOption != nReadProjectOption)
                        {
                            if (nRetryIndex == nRetryLimitCount)
                            {
                                m_sErrorMessage = "Set _Project_Option Error";
                                return false;
                            }
                            else
                                continue;
                        }
                    }

                    if (nSetFWIPOption != nOriginFWIPOption)
                    {
                        if (nSetFWIPOption != nReadFWIPOption)
                        {
                            if (nRetryIndex == nRetryLimitCount)
                            {
                                m_sErrorMessage = "Set _FWIP_Option Error";
                                return false;
                            }
                            else
                                continue;
                        }
                    }

                    break;
                }
            }

            return true;
        }

        private void SetRecoveryOptions(frmMain.FlowStep cFlowStep, short nOriginProjectOption, short nOriginFWIPOption)
        {
            if (ParamFingerAutoTuning.m_nDisableSetAnalogParameter != 1)
            {
                if (ParamFingerAutoTuning.m_nDisableSetFWOption != 1 && (m_eICGenerationType == ICGenerationType.Other || m_eICGenerationType == ICGenerationType.Gen7 || m_eICGenerationType == ICGenerationType.Gen6))
                {
                    SetTestModeEnable(false);

                    //Recovery the Original FW Option
                    OutputMessage("-Set Original FW Option");

                    //Enable OBL
                    //MessageOutput("-Enable OBL");
                    //ElanTouch.DisableAlgorithm(false, 1000, m_nDevIdx);

                    //Recovery the project option
                    OutputMessage(string.Format("-Set _Project_Option(0x54, 0xB1, 0x{0}, 0x{1}) Value=0x{2}", ((nOriginProjectOption & 0xFF00) >> 8).ToString("X2"), (nOriginProjectOption & 0x00FF).ToString("X2"), nOriginProjectOption.ToString("X4")));
                    //ElanTouch.SetProjOption(nOrgProjOption, 1000, m_nDeviceIndex);
                    ElanTouchSwitch.SetOption(ElanTouchSwitch.m_nOPTION_PROJECTOPTION, nOriginProjectOption, m_nDeviceIndex, m_bSocketConnectType);
                    Thread.Sleep(m_nNormalDelayTime);

                    OutputMessage(string.Format("-Set _FWIP_Option(0x54, 0xC1, 0x{0}, 0x{1}) Value=0x{2}", ((nOriginFWIPOption & 0xFF00) >> 8).ToString("X2"), (nOriginFWIPOption & 0x00FF).ToString("X2"), nOriginFWIPOption.ToString("X4")));
                    //ElanTouch.SetFWIPOption(nOrgFWIPOption, 1000, m_nDeviceIndex);
                    ElanTouchSwitch.SetOption(ElanTouchSwitch.m_nOPTION_FWIPOPTION, nOriginFWIPOption, m_nDeviceIndex, m_bSocketConnectType);
                    Thread.Sleep(m_nNormalDelayTime);
                }

                //Recovery the Original Analog Parameter
                OutputMessage("-Set Original Analog Parameter");

                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                        cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                    {
                        bool bSetSelfCALValue = (cFlowStep.m_eStep == MainStep.Self_FrequencySweep && (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2)) ? true : false;
                        SetFWParameter(cFlowStep, m_cOriginParameter, ParamFingerAutoTuning.m_nReKTimeout, true, true, bSetSelfCALValue: bSetSelfCALValue);
                    }
                    else
                        SetFWParameter(cFlowStep, m_cOriginParameter, ParamFingerAutoTuning.m_nReKTimeout, true);
                }
#if _USE_9F07_SOCKET
                else
                {
                    SetFWParameter_9F07(m_cOriginParameter, true);
                }
#endif
            }

            if (m_bSetPenFunctionOFF_8F18 == true)
                SetPenFunctionEnable_8F18(true, true);

            if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
            {
                SetTestModeEnable(true);
                m_bKeepNotReset = true;
            }
        }
#else
        #region 常數定義

        private const int RETRY_LIMIT = 4;
        private const short MASK_DISABLE_SELF_PARTIAL_GEN8 = unchecked((short)0xFFF7);
        private const short MASK_DISABLE_SELF_PARTIAL_OTHER = unchecked((short)0xFFBF);
        private const short MASK_DISABLE_TXN_SCAN_GEN8 = unchecked((short)0xFFFB);
        private const short MASK_DISABLE_TXN_SCAN_OTHER = unchecked((short)0xFEFF);
        private const short MASK_DISABLE_OBL_NOISE = unchecked((short)0xFFBF);
        private const short MASK_DISABLE_UPDATE_BASE = unchecked((short)0xFFEF);
        private const short MASK_DISABLE_IDLE_MODE_GEN8 = unchecked((short)0xFFFE);
        private const short MASK_DISABLE_IDLE_MODE_OTHER = unchecked((short)0xFF7F);
        private const short MASK_DISABLE_PEN_SCAN = unchecked((short)0xFDFF);
        private const short MASK_DISABLE_ADD_ADC_OFFSET = unchecked((short)0xEFFF);

        #endregion

        /// <summary>
        /// 取得並設定 Project Option 和 FWIP Option
        /// </summary>
        /// <param name="nOriginProjectOption">輸出參數，返回原始的 Project Option 值</param>
        /// <param name="nOriginFWIPOption">輸出參數，返回原始的 FWIP Option 值</param>
        /// <param name="nReadFWIPOption">輸出參數，返回設定後實際讀回的 FWIP Option 值</param>
        /// <returns>true: 選項讀取和設定成功; false: 讀取或設定失敗</returns>
        private bool GetAndSetOptions(ref short nOriginProjectOption, ref short nOriginFWIPOption, ref short nReadFWIPOption)
        {
#if _USE_9F07_SOCKET
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return true;
#endif

            if (ParamFingerAutoTuning.m_nDisableSetFWOption == 1)
                return true;

            // 讀取原始選項
            if (!ReadOriginalOptions(ref nOriginProjectOption, ref nOriginFWIPOption))
                return false;

            // 根據世代設定選項
            return m_eICGenerationType == ICGenerationType.Gen8
                ? SetOptionsForGen8(nOriginProjectOption, nOriginFWIPOption, ref nReadFWIPOption)
                : SetOptionsForOtherGen(nOriginProjectOption, nOriginFWIPOption, ref nReadFWIPOption);
        }

        /// <summary>
        /// 恢復原始選項設定
        /// </summary>
        /// <param name="cFlowStep">流程步驟物件，用於判斷需要恢復的參數類型和特殊處理</param>
        /// <param name="nOriginProjectOption">原始的 Project Option 值</param>
        /// <param name="nOriginFWIPOption">原始的 FWIP Option 值</param>
        private void SetRecoveryOptions(frmMain.FlowStep cFlowStep, short nOriginProjectOption, short nOriginFWIPOption)
        {
            if (ParamFingerAutoTuning.m_nDisableSetAnalogParameter == 1)
                return;

            RecoveryFWOptions(nOriginProjectOption, nOriginFWIPOption);
            RecoveryAnalogParameter(cFlowStep);
            HandleSpecialCases(cFlowStep);
        }

        #region 讀取選項

        /// <summary>
        /// 讀取原始的 Project Option 和 FWIP Option
        /// </summary>
        /// <param name="nOriginProjectOption">輸出參數，返回讀取到的 Project Option 值</param>
        /// <param name="nOriginFWIPOption">輸出參數，返回讀取到的 FWIP Option 值</param>
        /// <returns>true: 總是返回成功（目前無錯誤檢查機制）</returns>
        private bool ReadOriginalOptions(ref short nOriginProjectOption, ref short nOriginFWIPOption)
        {
            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                // 讀取 Project Option
                nOriginProjectOption = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                OutputMessage(string.Format("-Get _Project_Option(0x53, 0xB1, 0x00, 0x01) Value=0x{0}", nOriginProjectOption.ToString("X4")));

                // 讀取 FWIP Option
                int nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                nOriginFWIPOption = (short)nValue;
                OutputMessage(string.Format("-Get _FWIP_Option(0x53, 0xC1, 0x00, 0x01) Value=0x{0}", nOriginFWIPOption.ToString("X4")));
            }
            else
            {
                if (ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_USB)
                    //nOriginProjectOption = (short)ElanTouch.GetProjOption(1000, m_nDeviceIndex);
                    nOriginProjectOption = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                else
                {
                    /*
                    byte[] byteCommand_Array = new byte[] { 0x53, 0xB1, 0x00, 0x01, 0x00, 0x00 };
                    nOriginProjectOption = (short)xSendCmdCheck(byteCommand_Array, 0xB1);
                    */
                    //nOriginProjectOption = (short)ElanTouch.GetProjOption(1000, m_nDeviceIndex);
                    nOriginProjectOption = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                }

                OutputMessage(string.Format("-Get _Project_Option(0x53, 0xB1, 0x00, 0x01) Value=0x{0}", nOriginProjectOption.ToString("X4")));

                int nValue = 0;

                if (ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_USB)
                {
                    //nOriginFWIPOption = (short)ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDevIdx);
                    //ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDeviceIndex);
                    nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                    nOriginFWIPOption = (short)nValue;
                }
                else
                {
                    /*
                    byte[] byteCommand_Array = new byte[] { 0x53, 0xC1, 0x00, 0x01, 0x00, 0x00 };
                    nOriginFWIPOption = (short)xSendCmdCheck(byteCommand_Array, 0xC1);
                    */
                    //ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDeviceIndex);
                    nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                    nOriginFWIPOption = (short)nValue;
                }

                OutputMessage(string.Format("-Get _FWIP_Option(0x53, 0xC1, 0x00, 0x01) Value=0x{0}", nOriginFWIPOption.ToString("X4")));
            }

            return true;
        }

        #endregion

        #region Gen8 選項設定

        /// <summary>
        /// 設定 Gen8 世代的 Project Option 和 FWIP Option
        /// </summary>
        /// <param name="nOriginProjectOption">原始的 Project Option 值</param>
        /// <param name="nOriginFWIPOption">原始的 FWIP Option 值</param>
        /// <param name="nReadFWIPOption">輸出參數，返回實際讀回的 FWIP Option 值</param>
        /// <returns>true: 設定完成（可能成功或失敗，但總是返回 true）</returns>
        private bool SetOptionsForGen8(short nOriginProjectOption, short nOriginFWIPOption, ref short nReadFWIPOption)
        {
            for (int nRetryIndex = 0; nRetryIndex <= RETRY_LIMIT; nRetryIndex++)
            {
                var projectOption = CalculateGen8ProjectOption(nOriginProjectOption);
                var fwipOption = CalculateGen8FWIPOption(nOriginFWIPOption);

                SetOptionIfChanged(ElanTouchSwitch.m_nOPTION_PROJECTOPTION, projectOption, nOriginProjectOption, "Project");
                SetOptionIfChanged(ElanTouchSwitch.m_nOPTION_FWIPOPTION, fwipOption, nOriginFWIPOption, "FWIP");

                short nReadProjectOption = ReadOptionIfChanged(projectOption, nOriginProjectOption, ElanTouchSwitch.GetProjOption, "Project");

                nReadFWIPOption = ReadFWIPOptionIfChanged(fwipOption, nOriginFWIPOption, ElanTouchSwitch.GetFWIPOption, "FWIP");

                m_nReadProjectOption = nReadProjectOption;
                m_nReadFWIPOption = nReadFWIPOption;

                // 驗證設定
                if (!ValidateOption(projectOption, nOriginProjectOption, nReadProjectOption, "Project", nRetryIndex))
                    continue;

                if (!ValidateOption(fwipOption, nOriginFWIPOption, nReadFWIPOption, "FWIP", nRetryIndex))
                    continue;

                return true;
            }

            return true;
        }

        /// <summary>
        /// 計算Gen8 IC的專案選項值,依據參數設定停用Self Partial、TXN Scan及Idle Mode等功能
        /// </summary>
        /// <param name="nOriginProjectOption">原始專案選項值</param>
        /// <returns>回傳計算後的專案選項值,已套用對應的遮罩停用指定功能</returns>
        private short CalculateGen8ProjectOption(short nOriginProjectOption)
        {
            //Disable the self partial
            short option = (short)(nOriginProjectOption & MASK_DISABLE_SELF_PARTIAL_GEN8);

            if (ParamFingerAutoTuning.m_nDisableTxn_Scan == 1)
                option = (short)(option & MASK_DISABLE_TXN_SCAN_GEN8);

            if (ParamFingerAutoTuning.m_nDisableIdleMode == 1)
                option = (short)(option & MASK_DISABLE_IDLE_MODE_GEN8);

            return option;
        }

        /// <summary>
        /// 計算 Gen8 世代的 Project Option 值
        /// </summary>
        /// <param name="nOriginProjectOption">原始的 Project Option 值</param>
        /// <returns>計算後的 Project Option 值（已停用指定功能）</returns>
        private short CalculateGen8FWIPOption(short nOriginFWIPOption)
        {
            short option = (short)(nOriginFWIPOption & MASK_DISABLE_OBL_NOISE);

            if (ParamFingerAutoTuning.m_nDisableUpdateBase == 1)
                option = (short)(option & MASK_DISABLE_UPDATE_BASE);

            return option;
        }

        #endregion

        #region 其他世代選項設定

        /// <summary>
        /// 設定非Gen8 IC(其他世代)的專案選項及FWIP選項,計算並套用選項值,讀取並驗證設定是否正確,包含重試機制
        /// </summary>
        /// <param name="nOriginProjectOption">原始專案選項值</param>
        /// <param name="nOriginFWIPOption">原始FWIP選項值</param>
        /// <param name="nReadFWIPOption">讀取回的FWIP選項值,以參考方式傳遞並更新</param>
        /// <returns>選項設定及驗證成功回傳true;重試次數用盡仍回傳true(目前實作無失敗狀態)</returns>
        private bool SetOptionsForOtherGen(short nOriginProjectOption, short nOriginFWIPOption, ref short nReadFWIPOption)
        {
            for (int nRetryIndex = 0; nRetryIndex <= RETRY_LIMIT; nRetryIndex++)
            {
                var projectOption = CalculateOtherGenProjectOption(nOriginProjectOption);
                var fwipOption = CalculateOtherGenFWIPOption(nOriginFWIPOption);

                SetOptionIfChanged(ElanTouchSwitch.m_nOPTION_PROJECTOPTION, projectOption, nOriginProjectOption, "Project");
                SetOptionIfChanged(ElanTouchSwitch.m_nOPTION_FWIPOPTION, fwipOption, nOriginFWIPOption, "FWIP");

                short nReadProjectOption = ReadOptionIfChanged(projectOption, nOriginProjectOption, ElanTouchSwitch.GetProjOption, "Project");

                nReadFWIPOption = ReadFWIPOptionIfChanged(fwipOption, nOriginFWIPOption, ElanTouchSwitch.GetFWIPOption, "FWIP");

                m_nReadProjectOption = nReadProjectOption;
                m_nReadFWIPOption = nReadFWIPOption;

                // 驗證設定
                if (!ValidateOption(projectOption, nOriginProjectOption, nReadProjectOption, "Project", nRetryIndex))
                    continue;

                if (!ValidateOption(fwipOption, nOriginFWIPOption, nReadFWIPOption, "FWIP", nRetryIndex))
                    continue;

                return true;
            }

            return true;
        }

        /// <summary>
        /// 設定其他世代（Gen6/Gen7/Other）的 Project Option 和 FWIP Option
        /// </summary>
        /// <param name="nOriginProjectOption">原始的 Project Option 值</param>
        /// <param name="nOriginFWIPOption">原始的 FWIP Option 值</param>
        /// <param name="nReadFWIPOption">輸出參數，返回實際讀回的 FWIP Option 值</param>
        /// <returns>true: 設定完成（可能成功或失敗，但總是返回 true）</returns>
        private short CalculateOtherGenProjectOption(short nOriginProjectOption)
        {
            short option = (short)(nOriginProjectOption & MASK_DISABLE_SELF_PARTIAL_OTHER);

            if (ParamFingerAutoTuning.m_nDisableIdleMode == 1)
                option = (short)(option & MASK_DISABLE_IDLE_MODE_OTHER);

            return option;
        }

        /// <summary>
        /// 計算其他世代（Gen6/Gen7/Other）的 FWIP Option 值
        /// </summary>
        /// <param name="nOriginFWIPOption">原始的 FWIP Option 值</param>
        /// <returns>計算後的 FWIP Option 值（已停用指定功能）</returns>
        private short CalculateOtherGenFWIPOption(short nOriginFWIPOption)
        {
            short option = nOriginFWIPOption;

            if (ParamFingerAutoTuning.m_nDisableTxn_Scan == 1)
                option = (short)(option & MASK_DISABLE_TXN_SCAN_OTHER);

            option = (short)(option & MASK_DISABLE_OBL_NOISE);

            if (ParamFingerAutoTuning.m_nDisableUpdateBase == 1)
                option = (short)(option & MASK_DISABLE_UPDATE_BASE);

            if (ParamFingerAutoTuning.m_nDisablePen_Scan == 1)
                option = (short)(option & MASK_DISABLE_PEN_SCAN);

            if (ParamFingerAutoTuning.m_nDisableAddADCOffset == 1)
                option = (short)(option & MASK_DISABLE_ADD_ADC_OFFSET);

            return option;
        }

        #endregion

        #region 輔助方法

        /// <summary>
        /// 條件性設定選項值（僅在值變化時寫入）
        /// </summary>
        /// <param name="optionType">選項類型（m_nOPTION_PROJECTOPTION 或 m_nOPTION_FWIPOPTION）</param>
        /// <param name="newOption">要設定的新選項值</param>
        /// <param name="originOption">原始選項值（用於比較是否變化）</param>
        /// <param name="optionName">選項名稱（用於輸出訊息，如 "Project" 或 "FWIP"）</param>
        private void SetOptionIfChanged(int optionType, short newOption, short originOption, string optionName)
        {
            if (newOption == originOption)
                return;

            byte highByte = (byte)((newOption & 0xFF00) >> 8);
            byte lowByte = (byte)(newOption & 0x00FF);
            
            OutputMessage(string.Format("-Set _{0}_Option(0x54, {1}, 0x{2}, 0x{3}) Value=0x{4}", optionName, (optionType == ElanTouchSwitch.m_nOPTION_PROJECTOPTION ? "0xB1" : "0xC1"), highByte.ToString("X2"), lowByte.ToString("X2"), newOption.ToString("X4")));
            
            ElanTouchSwitch.SetOption(optionType, newOption, m_nDeviceIndex, m_bSocketConnectType);
            Thread.Sleep(m_nNormalDelayTime);
        }

        /// <summary>
        /// 條件性讀取選項值（僅在值變化時讀取）
        /// </summary>
        /// <param name="newOption">期望設定的新選項值</param>
        /// <param name="originOption">原始選項值（用於比較是否變化）</param>
        /// <param name="getOptionFunc">讀取選項的函數委派，簽名為 Func&lt;int, bool, short&gt;</param>
        /// <param name="optionName">選項名稱（用於輸出訊息和決定暫存器位址，如 "Project" 或 "FWIP"）</param>
        /// <returns>若值未變化返回原始值，若值有變化返回實際讀取的值</returns>
        private short ReadOptionIfChanged(short newOption, short originOption, Func<int, bool, short> getOptionFunc, string optionName)
        {
            if (newOption == originOption)
                return originOption;

            short readOption = getOptionFunc(m_nDeviceIndex, m_bSocketConnectType);
            OutputMessage(string.Format("-Read _{0}_Option(0x53, {1}, 0x00, 0x01) Value=0x{2}", optionName, (optionName == "Project" ? "0xB1" : "0xC1"), readOption.ToString("X4")));
            
            return readOption;
        }

        /// <summary>
        /// 條件性讀取 FWIP Option 值（FWIP Option 專用版本）
        /// </summary>
        /// <param name="newOption">期望設定的新選項值</param>
        /// <param name="originOption">原始選項值（用於比較是否變化）</param>
        /// <param name="getOptionFunc">讀取 FWIP Option 的函數委派，簽名為 Func&lt;int, bool, int&gt;</param>
        /// <param name="optionName">選項名稱（用於輸出訊息，通常為 "FWIP"）</param>
        /// <returns>若值未變化返回原始值，若值有變化返回實際讀取並轉換後的值（short）</returns>
        private short ReadFWIPOptionIfChanged(short newOption, short originOption, Func<int, bool, int> getOptionFunc, string optionName)
        {
            if (newOption == originOption)
                return originOption;

            int nValue = getOptionFunc(m_nDeviceIndex, m_bSocketConnectType);
            short readOption = (short)nValue;
            OutputMessage(string.Format("-Read _{0}_Option(0x53, 0xC1, 0x00, 0x01) Value=0x{1}", optionName, readOption.ToString("X4")));
            
            return readOption;
        }

        /// <summary>
        /// 驗證選項設定是否成功
        /// </summary>
        /// <param name="setOption">期望設定的選項值</param>
        /// <param name="originOption">原始選項值（用於判斷是否有設定變更）</param>
        /// <param name="readOption">實際讀回的選項值（用於驗證設定結果）</param>
        /// <param name="optionName">選項名稱（用於錯誤訊息，如 "Project" 或 "FWIP"）</param>
        /// <param name="retryIndex">當前重試索引（0 到 RETRY_LIMIT），用於判斷是否為最後一次重試</param>
        /// <returns>true: 驗證通過或無需驗證; false: 驗證失敗（需要重試或已達重試上限）</returns>
        private bool ValidateOption(short setOption, short originOption, short readOption, string optionName, int retryIndex)
        {
            if (setOption == originOption)
                return true;

            if (setOption != readOption)
            {
                if (retryIndex == RETRY_LIMIT)
                {
                    m_sErrorMessage = string.Format("Set _{0}_Option Error", optionName);
                    return false;
                }
                return false; // 需要重試
            }

            return true;
        }

        #endregion

        #region 恢復選項

        /// <summary>
        /// 恢復原始的韌體選項設定
        /// </summary>
        /// <param name="nOriginProjectOption">測試前讀取的原始 Project Option 值</param>
        /// <param name="nOriginFWIPOption">測試前讀取的原始 FWIP Option 值</param>
        private void RecoveryFWOptions(short nOriginProjectOption, short nOriginFWIPOption)
        {
            bool isTargetGeneration = m_eICGenerationType == ICGenerationType.Other ||
                                      m_eICGenerationType == ICGenerationType.Gen7 ||
                                      m_eICGenerationType == ICGenerationType.Gen6;

            if (ParamFingerAutoTuning.m_nDisableSetFWOption != 1 && isTargetGeneration)
            {
                SetTestModeEnable(false);

                //Recovery the Original FW Option
                OutputMessage("-Set Original FW Option");

                //Enable OBL
                //MessageOutput("-Enable OBL");
                //ElanTouch.DisableAlgorithm(false, 1000, m_nDeviceIndex);

                // 恢復 Project Option
                //Recovery the project option
                byte projHighByte = (byte)((nOriginProjectOption & 0xFF00) >> 8);
                byte projLowByte = (byte)(nOriginProjectOption & 0x00FF);
                OutputMessage(string.Format("-Set _Project_Option(0x54, 0xB1, 0x{0}, 0x{1}) Value=0x{2}", projHighByte.ToString("X2"), projLowByte.ToString("X2"), nOriginProjectOption.ToString("X4")));
                //ElanTouch.SetProjOption(nOriginProjectOption, 1000, m_nDeviceIndex);
                ElanTouchSwitch.SetOption(ElanTouchSwitch.m_nOPTION_PROJECTOPTION, nOriginProjectOption, m_nDeviceIndex, m_bSocketConnectType);
                Thread.Sleep(m_nNormalDelayTime);

                // 恢復 FWIP Option
                //Recovery the fwip option
                byte fwipHighByte = (byte)((nOriginFWIPOption & 0xFF00) >> 8);
                byte fwipLowByte = (byte)(nOriginFWIPOption & 0x00FF);
                OutputMessage(string.Format("-Set _FWIP_Option(0x54, 0xC1, 0x{0}, 0x{1}) Value=0x{2}", fwipHighByte.ToString("X2"), fwipLowByte.ToString("X2"), nOriginFWIPOption.ToString("X4")));
                //ElanTouch.SetFWIPOption(nOriginFWIPOption, 1000, m_nDeviceIndex);
                ElanTouchSwitch.SetOption(ElanTouchSwitch.m_nOPTION_FWIPOPTION, nOriginFWIPOption, m_nDeviceIndex, m_bSocketConnectType);
                Thread.Sleep(m_nNormalDelayTime);
            }
        }

        /// <summary>
        /// 恢復原始的類比參數設定
        /// </summary>
        /// <param name="cFlowStep">流程步驟物件，用於判斷需要恢復的參數類型</param>
        private void RecoveryAnalogParameter(frmMain.FlowStep cFlowStep)
        {
            //Recovery the Original Analog Parameter
            OutputMessage("-Set Original Analog Parameter");

            if (m_eICGenerationType != ICGenerationType.Gen9)
            {
                bool isSelfStep = cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                                  cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep;

                if (isSelfStep)
                {
                    bool bSetSelfCALValue = cFlowStep.m_eStep == MainStep.Self_FrequencySweep &&
                                           (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || 
                                            ParamFingerAutoTuning.m_nSelfFSRunCALData == 2);

                    SetFWParameter(cFlowStep, m_cOriginParameter, ParamFingerAutoTuning.m_nReKTimeout, bSetOrigin: true, bSetSelfParameter: true, bSetSelfCALValue: bSetSelfCALValue);
                }
                else
                {
                    SetFWParameter(cFlowStep, m_cOriginParameter, ParamFingerAutoTuning.m_nReKTimeout, bSetOrigin: true);
                }
            }
#if _USE_9F07_SOCKET
            else
            {
                SetFWParameter_9F07(m_cOriginParameter, true);
            }
#endif
        }

        /// <summary>
        /// 處理特殊情況的恢復邏輯
        /// </summary>
        /// <param name="cFlowStep">流程步驟物件，用於判斷是否需要特殊處理</param>
        private void HandleSpecialCases(frmMain.FlowStep cFlowStep)
        {
            if (m_bSetPenFunctionOFF_8F18)
                SetPenFunctionEnable_8F18(true, true);

            if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
            {
                SetTestModeEnable(true);
                m_bKeepNotReset = true;
            }
        }

        #endregion
#endif
    }
}
