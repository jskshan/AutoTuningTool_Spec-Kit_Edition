using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
#if DISABLE_V1_0_2_6
        /// <summary>
        /// Set the specific analog parameter
        /// 1.Exit test mode
        /// 2.Set PH1/PH2/Sum
        /// 3.ReK
        /// </summary>
        /// <param name="cFWParameter"></param>
        /// <param name="nReKTimeout"></param>
        /// <returns></returns>
        private AppCoreDefine.SetState SetFWParameter(frmMain.FlowStep cFlowStep, AppCoreDefine.FWParameter cFWParameter, int nReKTimeout, 
                                                      bool bSetOrigin = false, bool bSetSelfParameter = false, 
                                                      string sSetSelfKSequence = MainConstantParameter.m_sKSequence_NA, bool bSetSelfCALValue = false, 
                                                      int nSelfCALStartTrace = -1, int nSelfCALStartTraceNumber = 0, int nSelfCALEndTraceNumber = 0,
                                                      int nRetryIncreaseTime = 0)
        {
            int nSendCommandDelayTime_Gen8 = ParamFingerAutoTuning.m_nGen8SendCommandDelayTime;

            //若在Test Mode下,則離開Test Mode
            Thread.Sleep(m_nNormalDelayTime);
            //ElanTouch.EnableTestMode(false, 1000, m_nDevIdx);

            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                int nSendCommandDelayTime = nSendCommandDelayTime_Gen8 + nRetryIncreaseTime;

                if (bSetSelfParameter == true)
                {
                    ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eSelfTraceType);

                    if (bSetOrigin == true)
                        SetTestModeEnable(true);

                    Thread.Sleep(1000);

                    if (ParamFingerAutoTuning.m_nCommandScriptType != 2)
                    {
                        if (ParamFingerAutoTuning.m_nGen8JustSetSelfNCPNCN != 1)
                        {
                            int n_SELF_SP_NUM = m_cOriginParameter.m_n_SELF_SP_NUM;
                            int n_SELF_EFFECT_NUM = m_cOriginParameter.m_n_SELF_EFFECT_NUM;

                            int n_SELF_PH2E_LAT = (cFWParameter.m_n_SELF_PH2E_LMT << 8) | cFWParameter.m_n_SELF_PH2E_LAT;
                            int n_SELF_PH2_LAT = (cFWParameter.m_n_SELF_PH2_LAT << 8) | cFWParameter.m_n_SELF_PH2_MUX_LAT;

                            int n_SELF_PKT_WC_L = m_cOriginParameter.m_n_SELF_PKT_WC_L;

                            int n_SELF_BSH_ADC_TP_NUM_H = m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_H;
                            int n_SELF_BSH_ADC_TP_NUM_L = m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_L;

                            int n_SELF_EFFECT_FW_SET_COEF_NUM_H = m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H;
                            int n_SELF_EFFECT_FW_SET_COEF_NUM_L = m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L;

                            int n_SELF_DFT_NUM_IQ_FIR_CTL_H = m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H;
                            int n_SELF_DFT_NUM_IQ_FIR_CTL_L = m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L;

                            int n_SELF_ANA_TP_CTL_01_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H;
                            int n_SELF_ANA_TP_CTL_01_L = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L;

                            int n_SELF_ANA_TP_CTL_00_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H;
                            int n_SELF_ANA_TP_CTL_00_L = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L;

                            int n_SELF_IQ_BSH_GP0_GP1_H = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_H;
                            int n_SELF_IQ_BSH_GP0_GP1_L = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_L;

                            if (bSetOrigin == false)
                            {
                                n_SELF_SP_NUM = (m_cOriginParameter.m_n_SELF_SP_NUM - m_cOriginParameter.m_nSelf_DFT_NUM) + cFWParameter.m_nSelf_DFT_NUM;
                                n_SELF_EFFECT_NUM = (m_cOriginParameter.m_n_SELF_EFFECT_NUM - m_cOriginParameter.m_nSelf_DFT_NUM) + cFWParameter.m_nSelf_DFT_NUM;

                                n_SELF_PKT_WC_L = n_SELF_EFFECT_NUM * m_nRXTraceNumber;

                                n_SELF_BSH_ADC_TP_NUM_H = cFWParameter.m_nSelf_DFT_NUM << 4;
                                n_SELF_BSH_ADC_TP_NUM_L = m_nRXTraceNumber;

                                n_SELF_EFFECT_FW_SET_COEF_NUM_H = cFWParameter.m_nSelf_DFT_NUM;
                                n_SELF_EFFECT_FW_SET_COEF_NUM_L = 0;

                                n_SELF_DFT_NUM_IQ_FIR_CTL_H = cFWParameter.m_nSelf_DFT_NUM;
                                n_SELF_DFT_NUM_IQ_FIR_CTL_L = 0x1000;

                                if (cFWParameter.m_nSelf_Gain >= 0)
                                {
                                    n_SELF_ANA_TP_CTL_01_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H;
                                    n_SELF_ANA_TP_CTL_01_L = (m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L & 0xFF87) | (cFWParameter.m_nSelf_Gain << 3);
                                }

                                if (cFWParameter.m_nSelf_CAG >= 0)
                                {
                                    n_SELF_ANA_TP_CTL_00_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H;
                                    n_SELF_ANA_TP_CTL_00_L = (m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L & 0xFFC7) | (cFWParameter.m_nSelf_CAG << 3);
                                }

                                if (cFWParameter.m_nSelf_IQ_BSH >= 0)
                                {
                                    n_SELF_IQ_BSH_GP0_GP1_H = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_H;
                                    n_SELF_IQ_BSH_GP0_GP1_L = (m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_L & 0xF03F) | (cFWParameter.m_nSelf_IQ_BSH << 6);
                                }
                            }

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_SP_NUM, nValue2: n_SELF_SP_NUM);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, n_SELF_SP_NUM.ToString("x2").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_EFFECT_NUM, nValue2: n_SELF_EFFECT_NUM);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, n_SELF_EFFECT_NUM.ToString("x2").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_DFT_NUM, nValue2: cFWParameter.m_nSelf_DFT_NUM);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, cFWParameter.m_nSelf_DFT_NUM.ToString("x2").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_PH1, nValue2: cFWParameter.m_n_SELF_PH1);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, cFWParameter.m_n_SELF_PH1.ToString("x2").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_PH2E_LAT, nValue2: n_SELF_PH2E_LAT);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, n_SELF_PH2E_LAT.ToString("x4").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_PH2_LAT, nValue2: n_SELF_PH2_LAT);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, n_SELF_PH2_LAT.ToString("x4").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_PH2, nValue2: cFWParameter.m_n_SELF_PH2);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, cFWParameter.m_n_SELF_PH2.ToString("x4").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_PKT_WC_L, nValue2: n_SELF_PKT_WC_L);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, n_SELF_PKT_WC_L.ToString("x4").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_BSH_ADC_TP_NUM, n_SELF_BSH_ADC_TP_NUM_H, n_SELF_BSH_ADC_TP_NUM_L);
                            OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                        n_SELF_BSH_ADC_TP_NUM_H.ToString("x4").ToUpper(),
                                                        n_SELF_BSH_ADC_TP_NUM_L.ToString("x4").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_EFFECT_FW_SET_COEF_NUM, n_SELF_EFFECT_FW_SET_COEF_NUM_H, n_SELF_EFFECT_FW_SET_COEF_NUM_L);
                            OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                        n_SELF_EFFECT_FW_SET_COEF_NUM_H.ToString("x4").ToUpper(),
                                                        n_SELF_EFFECT_FW_SET_COEF_NUM_L.ToString("x4").ToUpper()));


                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_DFT_NUM_IQ_FIR_CTL, n_SELF_DFT_NUM_IQ_FIR_CTL_H, n_SELF_DFT_NUM_IQ_FIR_CTL_L);
                            OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                        n_SELF_DFT_NUM_IQ_FIR_CTL_H.ToString("x4").ToUpper(),
                                                        n_SELF_DFT_NUM_IQ_FIR_CTL_L.ToString("x4").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            if (bSetOrigin == true || (bSetOrigin == false && cFWParameter.m_nSelf_Gain >= 0))
                            {
                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_01, n_SELF_ANA_TP_CTL_01_H, n_SELF_ANA_TP_CTL_01_L);
                                OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                            n_SELF_ANA_TP_CTL_01_H.ToString("x4").ToUpper(),
                                                            n_SELF_ANA_TP_CTL_01_L.ToString("x4").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);
                            }

                            if (bSetOrigin == true && bSetSelfCALValue == true)
                            {
                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_00, n_SELF_ANA_TP_CTL_00_H, n_SELF_ANA_TP_CTL_00_L);
                                OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                            n_SELF_ANA_TP_CTL_00_H.ToString("x4").ToUpper(),
                                                            n_SELF_ANA_TP_CTL_00_L.ToString("x4").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);
                            }
                            else if (bSetOrigin == true || (bSetOrigin == false && cFWParameter.m_nSelf_CAG >= 0))
                            {
                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_00, n_SELF_ANA_TP_CTL_00_H, n_SELF_ANA_TP_CTL_00_L);
                                OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                            n_SELF_ANA_TP_CTL_00_H.ToString("x4").ToUpper(),
                                                            n_SELF_ANA_TP_CTL_00_L.ToString("x4").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);
                            }

                            if (bSetOrigin == true || (bSetOrigin == false && cFWParameter.m_nSelf_IQ_BSH >= 0))
                            {
                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._SELF_IQ_BSH_GP0_GP1, n_SELF_IQ_BSH_GP0_GP1_H, n_SELF_IQ_BSH_GP0_GP1_L);
                                OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                            n_SELF_IQ_BSH_GP0_GP1_H.ToString("x4").ToUpper(),
                                                            n_SELF_IQ_BSH_GP0_GP1_L.ToString("x4").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);
                            }
                        }

                        if (bSetOrigin == false && m_cfrmParent.m_bExecute == true)
                        {
                            if (sSetSelfKSequence == MainConstantParameter.m_sKSequence_FixedValue)
                            {
                                cElanCommand_Gen8.RunSendSelfCalibrationParameter(m_nSelfNCPValue, m_nSelfNCNValue, bSetSelfCALValue, m_cSelfParameter.m_nCAL, 
                                                                                  nSelfCALStartTrace, nSelfCALStartTraceNumber, nSelfCALEndTraceNumber);
                            }
                            else if (sSetSelfKSequence == MainConstantParameter.m_sKSequence_FileValue)
                            {
                                string s_MS_MM_RX0FilePath = string.Format(@"{0}\{1}\ini\{2}", Application.StartupPath, frmMain.m_sAPMainDirectoryName, 
                                                                           ParamFingerAutoTuning.m_sSelfFS_MS_MM_RX0FileName);

                                cElanCommand_Gen8.RunSendSelfCalibrationParameterByFile(s_MS_MM_RX0FilePath, bSetSelfCALValue, m_cSelfParameter.m_nCAL, 
                                                                                        nSelfCALStartTrace, nSelfCALStartTraceNumber, nSelfCALEndTraceNumber);
                            }
                        }
                    }

                    if (bSetOrigin == true)
                        SetTestModeEnable(false);
                }
                else
                {
                    ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);

                    /*
                    ElanTouchSwitch.EnableTestMode(false, m_nDevIdx, m_bSocketConnectType);
                    m_bEnterTestMode = false;
                    Thread.Sleep(m_nNormalDelayTime);

                    Thread.Sleep(1000);

                    cElanCommand_Gen8.SendRawDataCountCommand();

                    Thread.Sleep(1000);
                    */

                    if (bSetOrigin == true)
                        SetTestModeEnable(true);

                    Thread.Sleep(1000 + nRetryIncreaseTime);

                    /*
                    if (cElanCommand_Gen8.CheckEnterTestMode() == false)
                        return SETSTATE_ENTERTESTMODEERROR;
                    */

                    if (ParamFingerAutoTuning.m_nCommandScriptType != 2)
                    {
                        if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                        {
                            int n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H = m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H;
                            int n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L = m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L;

                            int n_MS_ANA_TP_CTL_01_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_H;
                            int n_MS_ANA_TP_CTL_01_L = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_L;
                            int n_MS_ANA_TP_CTL_01_2_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_H;
                            int n_MS_ANA_TP_CTL_01_2_L = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_L;

                            int n_MS_ANA_CTL_04_H = m_cOriginParameter.m_n_MS_ANA_CTL_04_H;
                            int n_MS_ANA_CTL_04_L = m_cOriginParameter.m_n_MS_ANA_CTL_04_L;
                            int n_MS_ANA_CTL_04_2_H = m_cOriginParameter.m_n_MS_ANA_CTL_04_2_H;
                            int n_MS_ANA_CTL_04_2_L = m_cOriginParameter.m_n_MS_ANA_CTL_04_2_L;

                            int n_MS_ANA_TP_CTL_06_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_H;
                            int n_MS_ANA_TP_CTL_06_L = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_L;
                            int n_MS_ANA_TP_CTL_06_2_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_H;
                            int n_MS_ANA_TP_CTL_06_2_L = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_L;
                            int n_MS_ANA_TP_CTL_07_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_H;
                            int n_MS_ANA_TP_CTL_07_L = m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_L;

                            if (bSetOrigin == false)
                            {
                                n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H = (m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H & 0xF80F) | (cFWParameter.m_n_MS_FIR_TAP_NUM << 4);
                                n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L = (m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L & 0xFFF0) | (cFWParameter.m_n_MS_FIRCOEF_SEL);

                                n_MS_ANA_TP_CTL_01_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_H;
                                n_MS_ANA_TP_CTL_01_L = (m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_L & 0x7E07) | (cFWParameter.m_n_MS_SELC << 7) | (cFWParameter.m_n_MS_SELGM << 3);

                                n_MS_ANA_CTL_04_H = m_cOriginParameter.m_n_MS_ANA_CTL_04_H;
                                n_MS_ANA_CTL_04_L = (m_cOriginParameter.m_n_MS_ANA_CTL_04_L & 0xFFFC) | (cFWParameter.m_n_MS_VSEL);

                                if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                                {
                                    n_MS_ANA_TP_CTL_01_2_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_H;
                                    n_MS_ANA_TP_CTL_01_2_L = (m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_L & 0x7E07) | (cFWParameter.m_n_MS_SELC << 7) | (cFWParameter.m_n_MS_SELGM << 3);

                                    n_MS_ANA_CTL_04_2_H = m_cOriginParameter.m_n_MS_ANA_CTL_04_2_H;
                                    n_MS_ANA_CTL_04_2_L = (m_cOriginParameter.m_n_MS_ANA_CTL_04_2_L & 0xFFFC) | (cFWParameter.m_n_MS_VSEL);

                                    n_MS_ANA_TP_CTL_06_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_H;
                                    n_MS_ANA_TP_CTL_06_L = (m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_L & 0xFFFC) | (cFWParameter.m_n_MS_LG);
                                    n_MS_ANA_TP_CTL_06_2_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_H;
                                    n_MS_ANA_TP_CTL_06_2_L = (m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_L & 0xFFFC) | (cFWParameter.m_n_MS_LG);
                                    n_MS_ANA_TP_CTL_07_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_H;
                                    n_MS_ANA_TP_CTL_07_L = (m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_L & 0xFC3C) | (cFWParameter.m_n_MS_SELGM << 6) | (cFWParameter.m_n_MS_LG);
                                }
                            }

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_BIN_FIRCOEF_SEL_TAP_NUM, n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H, n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L);
                            OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                        n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H.ToString("x4").ToUpper(),
                                                        n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L.ToString("x4").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01, n_MS_ANA_TP_CTL_01_H, n_MS_ANA_TP_CTL_01_L);
                            OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                        n_MS_ANA_TP_CTL_01_H.ToString("x4").ToUpper(),
                                                        n_MS_ANA_TP_CTL_01_L.ToString("x4").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                            {
                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01_2, n_MS_ANA_TP_CTL_01_2_H, n_MS_ANA_TP_CTL_01_2_L);
                                OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                            n_MS_ANA_TP_CTL_01_2_H.ToString("x4").ToUpper(),
                                                            n_MS_ANA_TP_CTL_01_2_L.ToString("x4").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);
                            }

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04, n_MS_ANA_CTL_04_H, n_MS_ANA_CTL_04_L);
                            OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                        n_MS_ANA_CTL_04_H.ToString("x4").ToUpper(),
                                                        n_MS_ANA_CTL_04_L.ToString("x4").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                            {
                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04_2, n_MS_ANA_CTL_04_2_H, n_MS_ANA_CTL_04_2_L);
                                OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                            n_MS_ANA_CTL_04_2_H.ToString("x4").ToUpper(),
                                                            n_MS_ANA_CTL_04_2_L.ToString("x4").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);

                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06, n_MS_ANA_TP_CTL_06_H, n_MS_ANA_TP_CTL_06_L);
                                OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                            n_MS_ANA_TP_CTL_06_H.ToString("x4").ToUpper(),
                                                            n_MS_ANA_TP_CTL_06_L.ToString("x4").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);

                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06_2, n_MS_ANA_TP_CTL_06_2_H, n_MS_ANA_TP_CTL_06_2_L);
                                OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                            n_MS_ANA_TP_CTL_06_2_H.ToString("x4").ToUpper(),
                                                            n_MS_ANA_TP_CTL_06_2_L.ToString("x4").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);

                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_07, n_MS_ANA_TP_CTL_07_H, n_MS_ANA_TP_CTL_07_L);
                                OutputMessage(string.Format("-Set {0}=0x{1}{2}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName,
                                                            n_MS_ANA_TP_CTL_07_H.ToString("x4").ToUpper(),
                                                            n_MS_ANA_TP_CTL_07_L.ToString("x4").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);
                            }
                        }
                        else
                        {
                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_PH1, nValue2:cFWParameter.m_nPH1);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, cFWParameter.m_nPH1.ToString("x2").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_PH2, nValue2:cFWParameter.m_nPH2);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, cFWParameter.m_nPH2.ToString("x2").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_PH3, nValue2:cFWParameter.m_nPH3);
                            OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, cFWParameter.m_nPH3.ToString("x2").ToUpper()));

                            Thread.Sleep(nSendCommandDelayTime);

                            if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                            {
                                int n_MS_SP_NUM = m_cOriginParameter.m_n_MS_SP_NUM;
                                int n_MS_EFFECT_NUM = m_cOriginParameter.m_n_MS_EFFECT_NUM;
                                int nPKT_WC = m_cOriginParameter.m_nPKT_WC;

                                if (bSetOrigin == false)
                                {
                                    n_MS_SP_NUM = (m_cOriginParameter.m_n_MS_DFT_NUM - m_cOriginParameter.m_n_MS_SP_NUM) + cFWParameter.m_n_MS_DFT_NUM;
                                    n_MS_EFFECT_NUM = (m_cOriginParameter.m_n_MS_DFT_NUM - m_cOriginParameter.m_n_MS_EFFECT_NUM) + cFWParameter.m_n_MS_DFT_NUM;
                                    nPKT_WC = n_MS_EFFECT_NUM * m_nRXTraceNumber;
                                }

                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_AFE_SP_NUM, nValue2:n_MS_SP_NUM);
                                OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, n_MS_SP_NUM.ToString("x2").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);

                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_AFE_EFFECT_NUM, nValue2:n_MS_EFFECT_NUM);
                                OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, n_MS_EFFECT_NUM.ToString("x2").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);

                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType._MS_AFE_DFT_NUM, nValue2:cFWParameter.m_n_MS_DFT_NUM);
                                OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, cFWParameter.m_n_MS_DFT_NUM.ToString("x2").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);

                                cElanCommand_Gen8.SendWriteCommand(ElanCommand_Gen8.ParameterType.PKT_WC, nValue2:nPKT_WC);
                                OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand_Gen8.m_cWriteCommandInfo.sParameterName, nPKT_WC.ToString("x4").ToUpper()));

                                Thread.Sleep(nSendCommandDelayTime);
                            }
                        }
                    }

                    if (bSetOrigin == true)
                        SetTestModeEnable(false);
                }
            }
            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data);

                SetTestModeEnable(true);

                if (m_eICGenerationType == ICGenerationType.Gen7)
                {
                    ElanCommand_Gen6or7 cElanCommand = new ElanCommand_Gen6or7(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eICGenerationType, m_eICSolutionType);

                    Thread.Sleep(m_nNormalDelayTime);

                    int n_MS_FIRTB = m_cOriginParameter.m_n_MS_FIRTB;

                    int n_MS_FIR_TAP_NUM = m_cOriginParameter.m_n_MS_FIR_TAP_NUM;

                    int n_MS_ANA_CTL_05 = m_cOriginParameter.m_n_MS_ANA_CTL_05;
                    int n_MS_ANA_CTL_02 = m_cOriginParameter.m_n_MS_ANA_CTL_02;
                    int n_MS_ANA_CTL_03 = m_cOriginParameter.m_n_MS_ANA_CTL_03;
                    int n_MS_ANA_CTL_06 = m_cOriginParameter.m_n_MS_ANA_CTL_06;

                    if (bSetOrigin == false)
                    {
                        n_MS_FIRTB = cFWParameter.m_n_MS_FIRTB;

                        n_MS_FIR_TAP_NUM = cFWParameter.m_n_MS_FIR_TAP_NUM;

                        n_MS_ANA_CTL_05 = (m_cOriginParameter.m_n_MS_ANA_CTL_05 & 0xFFF3) | (cFWParameter.m_n_MS_SELC << 2);
                        n_MS_ANA_CTL_02 = (m_cOriginParameter.m_n_MS_ANA_CTL_02 & 0xFFF9) | (cFWParameter.m_n_MS_VSEL << 1);
                        n_MS_ANA_CTL_03 = (m_cOriginParameter.m_n_MS_ANA_CTL_03 & 0xFFF9) | (cFWParameter.m_n_MS_VSEL << 1);
                        n_MS_ANA_CTL_06 = (m_cOriginParameter.m_n_MS_ANA_CTL_06 & 0xF0E7) | (cFWParameter.m_n_MS_SELGM << 8) | (cFWParameter.m_n_MS_LG << 3);
                    }

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_FIRTB, n_MS_FIRTB);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_FIRTB.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM, n_MS_FIR_TAP_NUM);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_FIR_TAP_NUM.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5, n_MS_ANA_CTL_05);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_ANA_CTL_05.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_2, n_MS_ANA_CTL_02);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_ANA_CTL_02.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_3, n_MS_ANA_CTL_03);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_ANA_CTL_03.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_6, n_MS_ANA_CTL_06);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_ANA_CTL_06.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);
                }
                else if (m_eICGenerationType == ICGenerationType.Gen6)
                {
                    ElanCommand_Gen6or7 cElanCommand = new ElanCommand_Gen6or7(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eICGenerationType, m_eICSolutionType);

                    Thread.Sleep(m_nNormalDelayTime);

                    int n_MS_FIRTB = m_cOriginParameter.m_n_MS_FIRTB;

                    int n_MS_FIR_TAP_NUM = m_cOriginParameter.m_n_MS_FIR_TAP_NUM;

                    int n_MS_ANA_CTL_08 = m_cOriginParameter.m_n_MS_ANA_CTL_08;
                    int n_MS_ANA_CTL_05 = m_cOriginParameter.m_n_MS_ANA_CTL_05;
                    int n_MS_ANA_CTL_04 = m_cOriginParameter.m_n_MS_ANA_CTL_04;

                    if (bSetOrigin == false)
                    {
                        n_MS_FIRTB = cFWParameter.m_n_MS_FIRTB;

                        n_MS_FIR_TAP_NUM = cFWParameter.m_n_MS_FIR_TAP_NUM;

                        n_MS_ANA_CTL_08 = (m_cOriginParameter.m_n_MS_ANA_CTL_08 & 0xFF33) | (cFWParameter.m_n_MS_SELC << 6) | (cFWParameter.m_n_MS_LG << 2);
                        n_MS_ANA_CTL_05 = (m_cOriginParameter.m_n_MS_ANA_CTL_05 & 0xFF3F) | (cFWParameter.m_n_MS_VSEL << 6);
                        n_MS_ANA_CTL_04 = (m_cOriginParameter.m_n_MS_ANA_CTL_04 & 0x8FFF) | (cFWParameter.m_n_MS_SELGM << 12);
                    }

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_FIRTB, n_MS_FIRTB);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_FIRTB.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM, n_MS_FIR_TAP_NUM);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_FIR_TAP_NUM.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_8, n_MS_ANA_CTL_08);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_ANA_CTL_08.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5, n_MS_ANA_CTL_05);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_ANA_CTL_05.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);

                    cElanCommand.SendWriteCommand(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_4, n_MS_ANA_CTL_04);
                    OutputMessage(string.Format("-Set {0}=0x{1}", cElanCommand.m_cWriteCommandInfo.sParameterName, n_MS_ANA_CTL_04.ToString("x4").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);
                }

                if (bSetOrigin == true)
                    SetTestModeEnable(false);
            }
            else
            {
                SetTestModeEnable(false);

                //ElanTouch.SetSUM(nSum, 1000, m_nDeviceIndex);
                //Thread.Sleep(m_nNormalDelayTime);

                //ElanTouch.ClearFR(1000, m_nDeviceIndex);
                //Thread.Sleep(m_nNormalDelayTime);

                OutputMessage(string.Format("-Set PH1=0x{0}", cFWParameter.m_nPH1.ToString("x2").ToUpper()));
                //ElanTouch.SetPH1(nPH1, 1000, m_nDeviceIndex);
                ElanTouchSwitch.SetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_PH1, cFWParameter.m_nPH1, m_nDeviceIndex, m_bSocketConnectType);
                /*
                byte[] byteCommand_Array = new byte[] { 0x54, 0xC5, (byte)((cFWParameter.m_nPH1 & 0xFF00) >> 8), (byte)(cFWParameter.m_nPH1 & 0x00FF) };
                SendDevCommand(byteCommand_Array);
                */
                Thread.Sleep(m_nNormalDelayTime);

                OutputMessage(string.Format("-Set PH2=0x{0}", cFWParameter.m_nPH2.ToString("x2").ToUpper()));
                //ElanTouch.SetPH2(nPH2, 1000, m_nDeviceIndex);
                ElanTouchSwitch.SetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_PH2, cFWParameter.m_nPH2, m_nDeviceIndex, m_bSocketConnectType);
                /*
                byteCommand_Array = new byte[] { 0x54, 0xC6, (byte)((cFWParameter.m_nPH2 & 0xFF00) >> 8), (byte)(cFWParameter.m_nPH2 & 0x00FF) };
                SendDevCommand(byteCommand_Array);
                */
                Thread.Sleep(m_nNormalDelayTime);

                OutputMessage(string.Format("-Set PH3=0x{0}", cFWParameter.m_nPH3.ToString("x2").ToUpper()));
                //ElanTouch.SetPH3(nPH3, 1000, m_nDevIdx);
                ElanTouchSwitch.SetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_PH3, cFWParameter.m_nPH3, m_nDeviceIndex, m_bSocketConnectType);
                /*
                byteCommand_Array = new byte[] { 0x54, 0xC7, (byte)((cFWParameter.m_nPH3 & 0xFF00) >> 8), (byte)(cFWParameter.m_nPH3 & 0x00FF) };
                SendDevCommand(byteCommand_Array);
                */
                Thread.Sleep(m_nNormalDelayTime);

                if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                {
                    OutputMessage(string.Format("-Set DFT_NUM=0x{0}", cFWParameter.m_n_MS_DFT_NUM.ToString("x2").ToUpper()));
                    //ElanTouch.SetSUM(nSum, 1000, m_nDeviceIndex);
                    ElanTouchSwitch.SetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_SUM, cFWParameter.m_n_MS_DFT_NUM, m_nDeviceIndex, m_bSocketConnectType);
                    Thread.Sleep(m_nNormalDelayTime);
                }

                OutputMessage("-Set TP Parameter(0x54, 0x2D, 0x00, 0x01)");
                //ElanTouch.SetTPParameter(1000, m_nDeviceIndex);
                // 00 04 54 2D 00 01
                ElanTouchSwitch.SetFWParameter(ElanTouchSwitch.m_nPARAMETER_TPPARAMETER, m_nDeviceIndex, m_bSocketConnectType);
                /*
                byteCommand_Array = new byte[] { 0x54, 0x2D, 0x00, 0x01 };
                SendDevCommand(byteCommand_Array);
                */
                Thread.Sleep(m_nNormalDelayTime);

                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    Thread.Sleep(2000);
            }

            /*
            if (bSetOrigin == true)
                SetReK(nReKTimeout, bSetOrigin);
            else if ((m_eICGenerationType == ICGenerationType.Other || m_eICGenerationType == ICGenerationType.Gen7 || m_eICGenerationType == ICGenerationType.Gen6) && cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
            }
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
            {
                for (int nRetryIndex = 0; nRetryIndex <= 5; nRetryIndex++)
                {
                    AppCoreDefine.SetState eSetState = SetReK(nReKTimeout);

                    if (eSetState == AppCoreDefine.SetState.Success)
                        break;
                    else if (nRetryIndex == 5)
                        return eSetState;
                }

                if (bSetOrigin == false && ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    Thread.Sleep(2000);
            }
            else if ((m_eICGenerationType == ICGenerationType.Other || m_eICGenerationType == ICGenerationType.Gen7 || m_eICGenerationType == ICGenerationType.Gen6) && cFlowStep.m_eStep != MainStep.Raw_ADC_Sweep)
            {
                //SetReK(nReKTimeout);

                for (int nRetryIndex = 0; nRetryIndex <= 5; nRetryIndex++)
                {
                    AppCoreDefine.SetState eSetState = SetReK(nReKTimeout);

                    if (eSetState == AppCoreDefine.SetState.Success)
                        break;
                    else if (nRetryIndex == 5)
                        return eSetState;
                }
            }
            */

            // 提取共用的判斷條件
            bool isTargetGeneration = m_eICGenerationType == ICGenerationType.Other ||
                                      m_eICGenerationType == ICGenerationType.Gen7 ||
                                      m_eICGenerationType == ICGenerationType.Gen6;

            bool isSSHSocket = ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER ||
                               ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER;

            // 簡化後的判斷式
            if (bSetOrigin)
            {
                SetReK(nReKTimeout, bSetOrigin);
            }
            else if (isTargetGeneration && cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                // 空處理
            }
            else if (isSSHSocket)
            {
                AppCoreDefine.SetState eSetState = RetrySetReK(nReKTimeout);

                if (eSetState != AppCoreDefine.SetState.Success)
                    return eSetState;

                if (!bSetOrigin && ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    Thread.Sleep(2000);
            }
            else if (isTargetGeneration && cFlowStep.m_eStep != MainStep.Raw_ADC_Sweep)
            {
                //SetReK(nReKTimeout);

                AppCoreDefine.SetState eSetState = RetrySetReK(nReKTimeout);

                if (eSetState != AppCoreDefine.SetState.Success)
                    return eSetState;
            }

            return AppCoreDefine.SetState.Success;
        }

        private AppCoreDefine.SetState RetrySetReK(int nReKTimeout)
        {
            for (int nRetryIndex = 0; nRetryIndex <= 5; nRetryIndex++)
            {
                AppCoreDefine.SetState eSetState = SetReK(nReKTimeout);

                if (eSetState == AppCoreDefine.SetState.Success)
                    return eSetState;

                if (nRetryIndex == 5)
                    return eSetState;
            }

            return AppCoreDefine.SetState.Success; // 理論上不會執行到
        }

#if _USE_9F07_SOCKET
        private AppCoreDefine.SetState SetFWParameter_9F07(AppCoreDefine.FWParameter cFWParameter, bool bSetOrigin = false, int nRetryIncreaseTime = 0, bool bNotEnableScanMode_9F07 = false)
        {
            SetScanModeDisable(true);

            SkipNoUsedResponseData();

            OutputMessage(string.Format("-Set PH1=0x{0}", cFWParameter.m_nPH1.ToString("x2").ToUpper()));
            int nPH1_H = (cFWParameter.m_nPH1 & 0xFF00) >> 8;
            int nPH1_L = cFWParameter.m_nPH1 & 0x00FF;
            ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xC5, (byte)nPH1_H, (byte)nPH1_L }, 0, m_bSocketConnectType);
            Thread.Sleep(m_nNormalDelayTime);

            OutputMessage(string.Format("-Set PH2=0x{0}", cFWParameter.m_nPH2.ToString("x2").ToUpper()));
            int nPH2_H = (cFWParameter.m_nPH2 & 0xFF00) >> 8;
            int nPH2_L = cFWParameter.m_nPH2 & 0x00FF;
            ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xC6, (byte)nPH2_H, (byte)nPH2_L }, 0, m_bSocketConnectType);
            Thread.Sleep(m_nNormalDelayTime);

            OutputMessage(string.Format("-Set PH3=0x{0}", cFWParameter.m_nPH3.ToString("x2").ToUpper()));
            int nPH3_H = (cFWParameter.m_nPH3 & 0xFF00) >> 8;
            int nPH3_L = cFWParameter.m_nPH3 & 0x00FF;
            ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xC7, (byte)nPH3_H, (byte)nPH3_L }, 0, m_bSocketConnectType);
            Thread.Sleep(m_nNormalDelayTime);

            if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
            {
                OutputMessage(string.Format("-Set Sum=0x{0}", cFWParameter.m_n_MS_DFT_NUM.ToString("x2").ToUpper()));
                int nSum_H = (cFWParameter.m_n_MS_DFT_NUM & 0xFF00) >> 8;
                int nSum_L = cFWParameter.m_n_MS_DFT_NUM & 0x00FF;
                ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xCB, (byte)nSum_H, (byte)nSum_L }, 0, m_bSocketConnectType);
                Thread.Sleep(m_nNormalDelayTime);
            }

            if (bNotEnableScanMode_9F07 == false)
                SetScanModeDisable(false);

            return AppCoreDefine.SetState.Success;
        }
#endif
#else
        /// <summary>
        /// 設定特定的類比參數
        /// 1. 退出測試模式
        /// 2. 設定 PH1/PH2/Sum
        /// 3. ReK
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFWParameter"></param>
        /// <param name="nReKTimeout"></param>
        /// <param name="bSetOrigin"></param>
        /// <param name="bSetSelfParameter"></param>
        /// <param name="sSetSelfKSequence"></param>
        /// <param name="bSetSelfCALValue"></param>
        /// <param name="nSelfCALStartTrace"></param>
        /// <param name="nSelfCALStartTraceNumber"></param>
        /// <param name="nSelfCALEndTraceNumber"></param>
        /// <param name="nRetryIncreaseTime"></param>
        /// <returns></returns>
        private AppCoreDefine.SetState SetFWParameter(
            frmMain.FlowStep cFlowStep, 
            AppCoreDefine.FWParameter cFWParameter, 
            int nReKTimeout,
            bool bSetOrigin = false, 
            bool bSetSelfParameter = false,
            string sSetSelfKSequence = MainConstantParameter.m_sKSequence_NA, 
            bool bSetSelfCALValue = false,
            int nSelfCALStartTrace = -1, 
            int nSelfCALStartTraceNumber = 0, 
            int nSelfCALEndTraceNumber = 0,
            int nRetryIncreaseTime = 0)
        {
            //若在Test Mode下,則離開Test Mode
            Thread.Sleep(m_nNormalDelayTime);
            //ElanTouch.EnableTestMode(false, 1000, m_nDeviceIndex);

            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                return HandleGen8Setting(
                    cFlowStep, cFWParameter, nReKTimeout, bSetOrigin, bSetSelfParameter, 
                    sSetSelfKSequence, bSetSelfCALValue, nSelfCALStartTrace, nSelfCALStartTraceNumber, 
                    nSelfCALEndTraceNumber, nRetryIncreaseTime
                );
            }
            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                return HandleRawADCSweep(cFWParameter, bSetOrigin);
            }
            else
            {
                return HandleOtherGeneration(cFlowStep, cFWParameter, nReKTimeout, bSetOrigin);
            }
        }

#if _USE_9F07_SOCKET
        /// <summary>
        /// 設定特定的類比參數
        /// </summary>
        /// <param name="cFWParameter"></param>
        /// <param name="bSetOrigin"></param>
        /// <param name="nRetryIncreaseTime"></param>
        /// <param name="bNotEnableScanMode_9F07"></param>
        /// <returns></returns>
        private AppCoreDefine.SetState SetFWParameter_9F07(AppCoreDefine.FWParameter cFWParameter, bool bSetOrigin = false, int nRetryIncreaseTime = 0, bool bNotEnableScanMode_9F07 = false)
        {
            SetScanModeDisable(true);

            SkipNoUsedResponseData();

            OutputMessage(string.Format("-Set PH1=0x{0}", cFWParameter.m_nPH1.ToString("x2").ToUpper()));
            int nPH1_H = (cFWParameter.m_nPH1 & 0xFF00) >> 8;
            int nPH1_L = cFWParameter.m_nPH1 & 0x00FF;
            ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xC5, (byte)nPH1_H, (byte)nPH1_L }, 0, m_bSocketConnectType);
            Thread.Sleep(m_nNormalDelayTime);

            OutputMessage(string.Format("-Set PH2=0x{0}", cFWParameter.m_nPH2.ToString("x2").ToUpper()));
            int nPH2_H = (cFWParameter.m_nPH2 & 0xFF00) >> 8;
            int nPH2_L = cFWParameter.m_nPH2 & 0x00FF;
            ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xC6, (byte)nPH2_H, (byte)nPH2_L }, 0, m_bSocketConnectType);
            Thread.Sleep(m_nNormalDelayTime);

            OutputMessage(string.Format("-Set PH3=0x{0}", cFWParameter.m_nPH3.ToString("x2").ToUpper()));
            int nPH3_H = (cFWParameter.m_nPH3 & 0xFF00) >> 8;
            int nPH3_L = cFWParameter.m_nPH3 & 0x00FF;
            ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xC7, (byte)nPH3_H, (byte)nPH3_L }, 0, m_bSocketConnectType);
            Thread.Sleep(m_nNormalDelayTime);

            if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
            {
                OutputMessage(string.Format("-Set Sum=0x{0}", cFWParameter.m_n_MS_DFT_NUM.ToString("x2").ToUpper()));
                int nSum_H = (cFWParameter.m_n_MS_DFT_NUM & 0xFF00) >> 8;
                int nSum_L = cFWParameter.m_n_MS_DFT_NUM & 0x00FF;
                ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xCB, (byte)nSum_H, (byte)nSum_L }, 0, m_bSocketConnectType);
                Thread.Sleep(m_nNormalDelayTime);
            }

            if (bNotEnableScanMode_9F07 == false)
                SetScanModeDisable(false);

            return AppCoreDefine.SetState.Success;
        }
#endif

        #region Gen8 處理

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="cFWParameter"></param>
        /// <param name="nReKTimeout"></param>
        /// <param name="bSetOrigin"></param>
        /// <param name="bSetSelfParameter"></param>
        /// <param name="sSetSelfKSequence"></param>
        /// <param name="bSetSelfCALValue"></param>
        /// <param name="nSelfCALStartTrace"></param>
        /// <param name="nSelfCALStartTraceNumber"></param>
        /// <param name="nSelfCALEndTraceNumber"></param>
        /// <param name="nRetryIncreaseTime"></param>
        /// <returns></returns>
        private AppCoreDefine.SetState HandleGen8Setting(
            frmMain.FlowStep cFlowStep,
            AppCoreDefine.FWParameter cFWParameter,
            int nReKTimeout,
            bool bSetOrigin,
            bool bSetSelfParameter,
            string sSetSelfKSequence,
            bool bSetSelfCALValue,
            int nSelfCALStartTrace,
            int nSelfCALStartTraceNumber,
            int nSelfCALEndTraceNumber,
            int nRetryIncreaseTime)
        {
            int nSendCommandDelayTime = ParamFingerAutoTuning.m_nGen8SendCommandDelayTime + nRetryIncreaseTime;
            ElanCommand_Gen8 cElanCommand = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eSelfTraceType);


            if (bSetSelfParameter)
            {
                if (bSetOrigin)
                    SetTestModeEnable(true);

                Thread.Sleep(1000);
            }
            else
            {
                /*
                ElanTouchSwitch.EnableTestMode(false, m_nDeviceIndex, m_bSocketConnectType);
                m_bEnterTestMode = false;
                Thread.Sleep(m_nNormalDelayTime);

                Thread.Sleep(1000);

                cElanCommand_Gen8.SendRawDataCountCommand();

                Thread.Sleep(1000);
                */

                if (bSetOrigin)
                    SetTestModeEnable(true);

                Thread.Sleep(1000 + nRetryIncreaseTime);

                /*
                if (cElanCommand_Gen8.CheckEnterTestMode() == false)
                    return SETSTATE_ENTERTESTMODEERROR;
                */
            }

            if (ParamFingerAutoTuning.m_nCommandScriptType != 2)
            {
                if (bSetSelfParameter)
                {
                    SetGen8SelfParameters(
                        cElanCommand, cFWParameter, nSendCommandDelayTime, bSetOrigin, 
                        bSetSelfCALValue, sSetSelfKSequence, nSelfCALStartTrace, nSelfCALStartTraceNumber, 
                        nSelfCALEndTraceNumber
                    );
                }
                else
                {
                    SetGen8MSParameters(cElanCommand, cFlowStep, cFWParameter, nSendCommandDelayTime, bSetOrigin);
                }
            }

            if (bSetOrigin)
                SetTestModeEnable(false);

            return HandleReKProcess(cFlowStep, nReKTimeout, bSetOrigin);
        }

        /// <summary>
        /// 處理 Gen8 IC 的參數設定與 ReK 流程
        /// <para>根據 bSetSelfParameter 決定設定 Self 或 MS 參數</para>
        /// <para>包含進出測試模式、發送命令、執行 ReK 等完整流程</para>
        /// </summary>
        /// <param name="cFlowStep">當前流程步驟</param>
        /// <param name="cFWParameter">韌體參數設定</param>
        /// <param name="nReKTimeout">ReK 流程逾時時間（毫秒）</param>
        /// <param name="bSetOrigin">是否執行完整流程（包含測試模式切換）</param>
        /// <param name="bSetSelfParameter">true: 設定 Self 參數; false: 設定 MS 參數</param>
        /// <param name="sSetSelfKSequence">Self K 序列設定字串</param>
        /// <param name="bSetSelfCALValue">是否設定 Self 校正值</param>
        /// <param name="nSelfCALStartTrace">Self 校正起始 Trace 位置</param>
        /// <param name="nSelfCALStartTraceNumber">Self 校正起始 Trace 數量</param>
        /// <param name="nSelfCALEndTraceNumber">Self 校正結束 Trace 數量</param>
        /// <param name="nRetryIncreaseTime">重試時額外增加的延遲時間（毫秒）</param>
        /// <returns>設定狀態：成功或錯誤類型</returns>
        private void SetGen8SelfParameters(
            ElanCommand_Gen8 cElanCommand,
            AppCoreDefine.FWParameter cFWParameter,
            int nSendCommandDelayTime,
            bool bSetOrigin,
            bool bSetSelfCALValue,
            string sSetSelfKSequence,
            int nSelfCALStartTrace,
            int nSelfCALStartTraceNumber,
            int nSelfCALEndTraceNumber)
        {
            if (ParamFingerAutoTuning.m_nGen8JustSetSelfNCPNCN == 1)
                return;

            var selfParams = CalculateSelfParameters(cFWParameter, bSetOrigin);

            // 依序設定所有參數
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_SP_NUM, selfParams.SP_NUM, nSendCommandDelayTime);
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_EFFECT_NUM, selfParams.EFFECT_NUM, nSendCommandDelayTime);
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_DFT_NUM, cFWParameter.m_nSelf_DFT_NUM, nSendCommandDelayTime);
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_PH1, cFWParameter.m_n_SELF_PH1, nSendCommandDelayTime);
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_PH2E_LAT, selfParams.PH2E_LAT, nSendCommandDelayTime);
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_PH2_LAT, selfParams.PH2_LAT, nSendCommandDelayTime);
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_PH2, cFWParameter.m_n_SELF_PH2, nSendCommandDelayTime);
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_PKT_WC_L, selfParams.PKT_WC_L, nSendCommandDelayTime);

            SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_BSH_ADC_TP_NUM, selfParams.BSH_ADC_TP_NUM_H, selfParams.BSH_ADC_TP_NUM_L, nSendCommandDelayTime);
            SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_EFFECT_FW_SET_COEF_NUM, selfParams.EFFECT_FW_SET_COEF_NUM_H, selfParams.EFFECT_FW_SET_COEF_NUM_L, nSendCommandDelayTime);
            SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_DFT_NUM_IQ_FIR_CTL, selfParams.DFT_NUM_IQ_FIR_CTL_H, selfParams.DFT_NUM_IQ_FIR_CTL_L, nSendCommandDelayTime);

            // 條件性設定參數
            if (bSetOrigin || (!bSetOrigin && cFWParameter.m_nSelf_Gain >= 0))
            {
                SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_01, selfParams.ANA_TP_CTL_01_H, selfParams.ANA_TP_CTL_01_L, nSendCommandDelayTime);
            }

            if ((bSetOrigin && bSetSelfCALValue) || (!bSetOrigin && cFWParameter.m_nSelf_CAG >= 0))
            {
                SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_00, selfParams.ANA_TP_CTL_00_H, selfParams.ANA_TP_CTL_00_L, nSendCommandDelayTime);
            }

            if (bSetOrigin || (!bSetOrigin && cFWParameter.m_nSelf_IQ_BSH >= 0))
            {
                SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._SELF_IQ_BSH_GP0_GP1, selfParams.IQ_BSH_GP0_GP1_H, selfParams.IQ_BSH_GP0_GP1_L, nSendCommandDelayTime);
            }

            // K Sequence 處理
            if (!bSetOrigin && m_cfrmParent.m_bExecute)
            {
                HandleSelfKSequence(cElanCommand, sSetSelfKSequence, bSetSelfCALValue, nSelfCALStartTrace, nSelfCALStartTraceNumber, nSelfCALEndTraceNumber);
            }
        }

        /// <summary>
        /// 設定 Gen8 IC 的 MS (Mutual Scan) 相關參數
        /// </summary>
        /// <remarks>
        /// 設定內容根據流程步驟而定：
        /// <list type="bullet">
        /// <item>Raw_ADC_Sweep 步驟：設定 Raw ADC Sweep 專用參數</item>
        /// <item>其他步驟：設定 MS_PH1, MS_PH2, MS_PH3 參數，若啟用則額外設定 Sum 參數</item>
        /// </list>
        /// </remarks>
        /// <param name="cElanCommand">Gen8 命令執行物件</param>
        /// <param name="cFlowStep">當前執行的流程步驟，用於判斷參數設定類型</param>
        /// <param name="cFWParameter">包含所有韌體參數值的物件</param>
        /// <param name="nSendCommandDelayTime">每次發送命令後的等待時間（毫秒）</param>
        /// <param name="bSetOrigin">是否設定為原始模式（影響子函式行為）</param>
        private void SetGen8MSParameters(
            ElanCommand_Gen8 cElanCommand,
            frmMain.FlowStep cFlowStep,
            AppCoreDefine.FWParameter cFWParameter,
            int nSendCommandDelayTime,
            bool bSetOrigin)
        {
            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                SetGen8RawADCSweepParameters(cElanCommand, cFWParameter, nSendCommandDelayTime, bSetOrigin);
            }
            else
            {
                // 一般參數設定
                SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._MS_PH1, cFWParameter.m_nPH1, nSendCommandDelayTime);
                SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._MS_PH2, cFWParameter.m_nPH2, nSendCommandDelayTime);
                SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._MS_PH3, cFWParameter.m_nPH3, nSendCommandDelayTime);

                if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                {
                    SetGen8SumParameters(cElanCommand, cFWParameter, nSendCommandDelayTime, bSetOrigin);
                }
            }
        }

        /// <summary>
        /// 設定 Gen8 IC 的 Raw ADC Sweep 相關暫存器參數
        /// </summary>
        /// <remarks>
        /// 設定的暫存器包括：
        /// <list type="bullet">
        /// <item>BIN_FIRCOEF_SEL_TAP_NUM - FIR 係數選擇和 TAP 數量</item>
        /// <item>ANA_TP_CTL_01 - 類比觸控控制暫存器 01</item>
        /// <item>ANA_CTL_04 - 類比控制暫存器 04</item>
        /// <item>8F18 方案額外設定：ANA_TP_CTL_01_2, ANA_CTL_04_2, ANA_TP_CTL_06, ANA_TP_CTL_06_2, ANA_TP_CTL_07</item>
        /// </list>
        /// </remarks>
        /// <param name="cElanCommand">Gen8 命令執行物件</param>
        /// <param name="cFWParameter">包含所有韌體參數值的物件</param>
        /// <param name="nSendCommandDelayTime">每次發送命令後的等待時間（毫秒）</param>
        /// <param name="bSetOrigin">是否設定為原始模式，影響參數計算邏輯</param>
        private void SetGen8RawADCSweepParameters(
            ElanCommand_Gen8 cElanCommand,
            AppCoreDefine.FWParameter cFWParameter,
            int nSendCommandDelayTime,
            bool bSetOrigin)
        {
            var rawParams = CalculateRawADCSweepParameters(cFWParameter, bSetOrigin);

            SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._MS_BIN_FIRCOEF_SEL_TAP_NUM, rawParams.BIN_FIRCOEF_SEL_TAP_NUM_H, rawParams.BIN_FIRCOEF_SEL_TAP_NUM_L, nSendCommandDelayTime);
            SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01, rawParams.ANA_TP_CTL_01_H, rawParams.ANA_TP_CTL_01_L, nSendCommandDelayTime);

            if (m_eICSolutionType == ICSolutionType.Solution_8F18)
            {
                SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01_2, rawParams.ANA_TP_CTL_01_2_H, rawParams.ANA_TP_CTL_01_2_L, nSendCommandDelayTime);
            }

            SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04, rawParams.ANA_CTL_04_H, rawParams.ANA_CTL_04_L, nSendCommandDelayTime);

            if (m_eICSolutionType == ICSolutionType.Solution_8F18)
            {
                SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04_2, rawParams.ANA_CTL_04_2_H, rawParams.ANA_CTL_04_2_L, nSendCommandDelayTime);
                SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06, rawParams.ANA_TP_CTL_06_H, rawParams.ANA_TP_CTL_06_L, nSendCommandDelayTime);
                SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06_2, rawParams.ANA_TP_CTL_06_2_H, rawParams.ANA_TP_CTL_06_2_L, nSendCommandDelayTime);
                SendGen8CommandPair(cElanCommand, ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_07, rawParams.ANA_TP_CTL_07_H, rawParams.ANA_TP_CTL_07_L, nSendCommandDelayTime);
            }
        }

        /// <summary>
        /// 設定 Gen8 IC 的 MS Sum (Mutual Scan 累加) 參數
        /// </summary>
        /// <remarks>
        /// 設定的參數包括：
        /// <list type="bullet">
        /// <item>MS_AFE_SP_NUM - MS AFE 起始點數量</item>
        /// <item>MS_AFE_EFFECT_NUM - MS AFE 有效數量</item>
        /// <item>MS_AFE_DFT_NUM - MS AFE DFT 數量</item>
        /// <item>PKT_WC - 封包字數 (Word Count)</item>
        /// </list>
        /// <para>當 bSetOrigin 為 false 時，會根據原始 DFT_NUM 與新 DFT_NUM 的差異重新計算 SP_NUM 和 EFFECT_NUM</para>
        /// </remarks>
        /// <param name="cElanCommand">Gen8 命令執行物件</param>
        /// <param name="cFWParameter">包含新的韌體參數值的物件</param>
        /// <param name="nSendCommandDelayTime">每次發送命令後的等待時間（毫秒）</param>
        /// <param name="bSetOrigin">是否使用原始參數值（true: 直接使用原始值; false: 根據 DFT_NUM 變化計算新值）</param>
        private void SetGen8SumParameters(
            ElanCommand_Gen8 cElanCommand,
            AppCoreDefine.FWParameter cFWParameter,
            int nSendCommandDelayTime,
            bool bSetOrigin)
        {
            int n_MS_SP_NUM = m_cOriginParameter.m_n_MS_SP_NUM;
            int n_MS_EFFECT_NUM = m_cOriginParameter.m_n_MS_EFFECT_NUM;
            int nPKT_WC = m_cOriginParameter.m_nPKT_WC;

            if (!bSetOrigin)
            {
                n_MS_SP_NUM = (m_cOriginParameter.m_n_MS_DFT_NUM - m_cOriginParameter.m_n_MS_SP_NUM) + cFWParameter.m_n_MS_DFT_NUM;
                n_MS_EFFECT_NUM = (m_cOriginParameter.m_n_MS_DFT_NUM - m_cOriginParameter.m_n_MS_EFFECT_NUM) + cFWParameter.m_n_MS_DFT_NUM;
                nPKT_WC = n_MS_EFFECT_NUM * m_nRXTraceNumber;
            }

            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._MS_AFE_SP_NUM, n_MS_SP_NUM, nSendCommandDelayTime);
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._MS_AFE_EFFECT_NUM, n_MS_EFFECT_NUM, nSendCommandDelayTime);
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType._MS_AFE_DFT_NUM, cFWParameter.m_n_MS_DFT_NUM, nSendCommandDelayTime);
            SendGen8Command(cElanCommand, ElanCommand_Gen8.ParameterType.PKT_WC, nPKT_WC, nSendCommandDelayTime);
        }

        #endregion

        #region Gen6/Gen7 Raw ADC 處理

        /// <summary>
        /// 處理 Raw ADC Sweep 的完整設定流程
        /// </summary>
        /// <remarks>
        /// 執行流程：
        /// <list type="number">
        /// <item>退出測試模式</item>
        /// <item>設定讀取 Bulk RAM 資料</item>
        /// <item>進入測試模式</item>
        /// <item>根據 IC 世代設定對應參數 (Gen6/Gen7)</item>
        /// <item>若 bSetOrigin 為 true，退出測試模式</item>
        /// </list>
        /// </remarks>
        /// <param name="cFWParameter">包含 Raw ADC Sweep 所需的韌體參數</param>
        /// <param name="bSetOrigin">是否設定為原始模式（true: 完整流程含退出測試模式; false: 保持測試模式）</param>
        /// <returns>設定狀態：目前固定返回 Success</returns>
        private AppCoreDefine.SetState HandleRawADCSweep(
            AppCoreDefine.FWParameter cFWParameter,
            bool bSetOrigin)
        {
            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data);
            SetTestModeEnable(true);

            if (m_eICGenerationType == ICGenerationType.Gen7)
            {
                SetGen7RawADCSweepParameters(cFWParameter, bSetOrigin);
            }
            else if (m_eICGenerationType == ICGenerationType.Gen6)
            {
                SetGen6RawADCSweepParameters(cFWParameter, bSetOrigin);
            }

            if (bSetOrigin)
                SetTestModeEnable(false);

            return AppCoreDefine.SetState.Success;
        }

        /// <summary>
        /// 設定 Gen7 IC 的 Raw ADC Sweep 暫存器參數
        /// </summary>
        /// <remarks>
        /// 設定的暫存器包括：
        /// <list type="bullet">
        /// <item>MS_FIRTB - FIR 濾波器表格</item>
        /// <item>MS_FIR_TAP_NUM - FIR TAP 數量</item>
        /// <item>MS_ANA_CTL_5 - 類比控制暫存器 5</item>
        /// <item>MS_ANA_CTL_2 - 類比控制暫存器 2</item>
        /// <item>MS_ANA_CTL_3 - 類比控制暫存器 3</item>
        /// <item>MS_ANA_CTL_6 - 類比控制暫存器 6</item>
        /// </list>
        /// <para>參數值透過 CalculateGen7Parameters 方法計算取得</para>
        /// </remarks>
        /// <param name="cFWParameter">包含韌體參數值的物件</param>
        /// <param name="bSetOrigin">是否設定為原始模式，影響參數計算邏輯</param>
        private void SetGen7RawADCSweepParameters(AppCoreDefine.FWParameter cFWParameter, bool bSetOrigin)
        {
            ElanCommand_Gen6or7 cElanCommand = new ElanCommand_Gen6or7(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eICGenerationType, m_eICSolutionType);

            Thread.Sleep(m_nNormalDelayTime);

            var gen7Params = CalculateGen7Parameters(cFWParameter, bSetOrigin);

            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_FIRTB, gen7Params.FIRTB);
            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM, gen7Params.FIR_TAP_NUM);
            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5, gen7Params.ANA_CTL_05);
            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_2, gen7Params.ANA_CTL_02);
            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_3, gen7Params.ANA_CTL_03);
            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_6, gen7Params.ANA_CTL_06);
        }

        /// <summary>
        /// 設定 Gen6 IC 的 Raw ADC Sweep 暫存器參數
        /// </summary>
        /// <remarks>
        /// 設定的暫存器包括：
        /// <list type="bullet">
        /// <item>MS_FIRTB - FIR 濾波器表格</item>
        /// <item>MS_FIR_TAP_NUM - FIR TAP 數量</item>
        /// <item>MS_ANA_CTL_8 - 類比控制暫存器 8</item>
        /// <item>MS_ANA_CTL_5 - 類比控制暫存器 5</item>
        /// <item>MS_ANA_CTL_4 - 類比控制暫存器 4</item>
        /// </list>
        /// <para>參數值透過 CalculateGen6Parameters 方法計算取得</para>
        /// </remarks>
        /// <param name="cFWParameter">包含韌體參數值的物件</param>
        /// <param name="bSetOrigin">是否設定為原始模式，影響參數計算邏輯</param>
        private void SetGen6RawADCSweepParameters(AppCoreDefine.FWParameter cFWParameter, bool bSetOrigin)
        {
            ElanCommand_Gen6or7 cElanCommand = new ElanCommand_Gen6or7(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eICGenerationType, m_eICSolutionType);

            Thread.Sleep(m_nNormalDelayTime);

            var gen6Params = CalculateGen6Parameters(cFWParameter, bSetOrigin);

            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_FIRTB, gen6Params.FIRTB);
            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM, gen6Params.FIR_TAP_NUM);
            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_8, gen6Params.ANA_CTL_08);
            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5, gen6Params.ANA_CTL_05);
            SendGen6or7Command(cElanCommand, ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_4, gen6Params.ANA_CTL_04);
        }

        #endregion

        #region 其他世代處理

        /// <summary>
        /// 處理其他世代 IC 的參數設定與 ReK 流程
        /// </summary>
        /// <remarks>
        /// 執行流程：
        /// <list type="number">
        /// <item>退出測試模式</item>
        /// <item>設定類比參數 PH1 (0x54, 0xC5)</item>
        /// <item>設定類比參數 PH2 (0x54, 0xC6)</item>
        /// <item>設定類比參數 PH3 (0x54, 0xC7)</item>
        /// <item>若啟用 Sum，設定 DFT_NUM 參數</item>
        /// <item>設定 TP 參數 (0x54, 0x2D, 0x00, 0x01)</item>
        /// <item>執行 ReK 流程</item>
        /// </list>
        /// <para>適用於不屬於 Gen6/Gen7/Gen8 的舊世代或特殊世代 IC</para>
        /// </remarks>
        /// <param name="cFlowStep">當前流程步驟</param>
        /// <param name="cFWParameter">包含類比參數值的韌體參數物件</param>
        /// <param name="nReKTimeout">ReK 流程逾時時間（毫秒）</param>
        /// <param name="bSetOrigin">是否設定為原始模式，影響 ReK 流程行為</param>
        /// <returns>設定狀態：成功或錯誤類型</returns>
        private AppCoreDefine.SetState HandleOtherGeneration(
            frmMain.FlowStep cFlowStep,
            AppCoreDefine.FWParameter cFWParameter,
            int nReKTimeout,
            bool bSetOrigin)
        {
            SetTestModeEnable(false);

            //ElanTouch.SetSUM(nSum, 1000, m_nDeviceIndex);
            //Thread.Sleep(m_nNormalDelayTime);

            //ElanTouch.ClearFR(1000, m_nDeviceIndex);
            //Thread.Sleep(m_nNormalDelayTime);

            //ElanTouch.SetPH1(nPH1, 1000, m_nDeviceIndex);
            SetAnalogParameter("PH1", ElanTouchSwitch.m_nPARAMETER_PH1, cFWParameter.m_nPH1);
            /*
            byte[] byteCommand_Array = new byte[] { 0x54, 0xC5, (byte)((cFWParameter.m_nPH1 & 0xFF00) >> 8), (byte)(cFWParameter.m_nPH1 & 0x00FF) };
            SendDevCommand(byteCommand_Array);
            */

            //ElanTouch.SetPH2(nPH2, 1000, m_nDeviceIndex);
            SetAnalogParameter("PH2", ElanTouchSwitch.m_nPARAMETER_PH2, cFWParameter.m_nPH2);
            /*
            byteCommand_Array = new byte[] { 0x54, 0xC6, (byte)((cFWParameter.m_nPH2 & 0xFF00) >> 8), (byte)(cFWParameter.m_nPH2 & 0x00FF) };
            SendDevCommand(byteCommand_Array);
            */

            //ElanTouch.SetPH3(nPH3, 1000, m_nDevIdx);
            SetAnalogParameter("PH3", ElanTouchSwitch.m_nPARAMETER_PH3, cFWParameter.m_nPH3);
            /*
            byteCommand_Array = new byte[] { 0x54, 0xC7, (byte)((cFWParameter.m_nPH3 & 0xFF00) >> 8), (byte)(cFWParameter.m_nPH3 & 0x00FF) };
            SendDevCommand(byteCommand_Array);
            */

            if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
            {
                //ElanTouch.SetSUM(nSum, 1000, m_nDeviceIndex);
                SetAnalogParameter("DFT_NUM", ElanTouchSwitch.m_nPARAMETER_SUM, cFWParameter.m_n_MS_DFT_NUM);
            }

            OutputMessage("-Set TP Parameter(0x54, 0x2D, 0x00, 0x01)");
            //ElanTouch.SetTPParameter(1000, m_nDeviceIndex);
            // 00 04 54 2D 00 01
            ElanTouchSwitch.SetFWParameter(ElanTouchSwitch.m_nPARAMETER_TPPARAMETER, m_nDeviceIndex, m_bSocketConnectType);
            /*
            byteCommand_Array = new byte[] { 0x54, 0x2D, 0x00, 0x01 };
            SendDevCommand(byteCommand_Array);
            */
            Thread.Sleep(m_nNormalDelayTime);

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                Thread.Sleep(2000);

            return HandleReKProcess(cFlowStep, nReKTimeout, bSetOrigin);
        }

        #endregion

        #region 輔助方法

        /// <summary>
        /// 處理 ReK (Re-Calibration) 重新校正流程的條件執行邏輯
        /// </summary>
        /// <remarks>
        /// 執行邏輯：
        /// <list type="bullet">
        /// <item><strong>原始模式 (bSetOrigin = true)</strong>: 直接執行 ReK</item>
        /// <item><strong>目標世代 (Gen6/Gen7/Other) + Raw ADC Sweep 步驟</strong>: 跳過 ReK</item>
        /// <item><strong>SSH Socket 類型</strong>: 執行帶重試的 ReK，OTHER_SSHSOCKETSERVER 額外延遲 2 秒</item>
        /// <item><strong>目標世代 + 非 Raw ADC Sweep 步驟</strong>: 執行帶重試的 ReK</item>
        /// <item><strong>其他情況</strong>: 跳過 ReK</item>
        /// </list>
        /// <para>目標世代定義：Gen6, Gen7, Other（非 Gen8）</para>
        /// </remarks>
        /// <param name="cFlowStep">當前流程步驟，用於判斷是否為 Raw_ADC_Sweep</param>
        /// <param name="nReKTimeout">ReK 流程逾時時間（毫秒）</param>
        /// <param name="bSetOrigin">是否為原始模式設定（true: 無條件執行 ReK; false: 依條件判斷）</param>
        /// <returns>設定狀態：Success 或重試 ReK 的錯誤狀態</returns>
        private AppCoreDefine.SetState HandleReKProcess(
            frmMain.FlowStep cFlowStep,
            int nReKTimeout,
            bool bSetOrigin)
        {
            bool isTargetGeneration = m_eICGenerationType == ICGenerationType.Other ||
                                      m_eICGenerationType == ICGenerationType.Gen7 ||
                                      m_eICGenerationType == ICGenerationType.Gen6;

            bool isSSHSocket = ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER ||
                               ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER;

            if (bSetOrigin)
            {
                SetReK(nReKTimeout, bSetOrigin);
            }
            else if (isTargetGeneration && cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                // 無需處理
            }
            else if (isSSHSocket)
            {
                AppCoreDefine.SetState eSetState = RetrySetReK(nReKTimeout);

                if (eSetState != AppCoreDefine.SetState.Success)
                    return eSetState;

                if (!bSetOrigin && ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    Thread.Sleep(2000);
            }
            else if (isTargetGeneration && cFlowStep.m_eStep != MainStep.Raw_ADC_Sweep)
            {
                AppCoreDefine.SetState eSetState = RetrySetReK(nReKTimeout);

                if (eSetState != AppCoreDefine.SetState.Success)
                    return eSetState;
            }

            return AppCoreDefine.SetState.Success;
        }

        /// <summary>
        /// 執行帶重試機制的 ReK (Re-Calibration) 重新校正流程
        /// </summary>
        /// <remarks>
        /// 重試邏輯：
        /// <list type="bullet">
        /// <item>最多嘗試 6 次（nRetryIndex: 0~5）</item>
        /// <item>任一次成功即返回 Success</item>
        /// <item>全部失敗則返回最後一次的錯誤狀態</item>
        /// </list>
        /// <para>主要用於 SSH Socket 連線或不穩定環境，提高 ReK 成功率</para>
        /// </remarks>
        /// <param name="nReKTimeout">單次 ReK 流程的逾時時間（毫秒）</param>
        /// <returns>設定狀態：成功返回 Success，失敗返回最後一次的錯誤狀態</returns>
        private AppCoreDefine.SetState RetrySetReK(int nReKTimeout)
        {
            for (int nRetryIndex = 0; nRetryIndex <= 5; nRetryIndex++)
            {
                AppCoreDefine.SetState eSetState = SetReK(nReKTimeout);

                if (eSetState == AppCoreDefine.SetState.Success)
                    return AppCoreDefine.SetState.Success;

                if (nRetryIndex == 5)
                    return eSetState;
            }

            return AppCoreDefine.SetState.Success;
        }

        /// <summary>
        /// 發送 Gen8 IC 的單一參數設定命令
        /// </summary>
        /// <remarks>
        /// 執行流程：
        /// <list type="number">
        /// <item>透過 ElanCommand_Gen8 物件發送寫入命令</item>
        /// <item>輸出設定訊息到日誌（參數名稱與十六進位值）</item>
        /// <item>延遲指定時間等待命令完成</item>
        /// </list>
        /// <para>此方法用於設定單一數值參數，如 PH1, PH2, PH3 等</para>
        /// </remarks>
        /// <param name="cmd">Gen8 命令執行物件</param>
        /// <param name="type">參數類型（如 _MS_PH1, _MS_PH2 等）</param>
        /// <param name="value">參數值（整數）</param>
        /// <param name="delay">命令發送後的延遲時間（毫秒）</param>
        private void SendGen8Command(ElanCommand_Gen8 cmd, ElanCommand_Gen8.ParameterType type, int value, int delay)
        {
            cmd.SendWriteCommand(type, nValue2: value);
            OutputMessage(string.Format("-Set {0}=0x{1}", cmd.m_cWriteCommandInfo.sParameterName, value.ToString("X")));
            Thread.Sleep(delay);
        }

        /// <summary>
        /// 發送 Gen8 IC 的雙位元組參數設定命令
        /// </summary>
        /// <remarks>
        /// 執行流程：
        /// <list type="number">
        /// <item>透過 ElanCommand_Gen8 物件發送寫入命令（包含高低位元組）</item>
        /// <item>輸出設定訊息到日誌（參數名稱與完整十六進位值）</item>
        /// <item>延遲指定時間等待命令完成</item>
        /// </list>
        /// <para>此方法用於設定需要高低位元組分開傳送的參數，如 Raw ADC Sweep 的暫存器參數</para>
        /// <para>輸出格式：參數名稱=0xHHHHLLLL（高位元組4位+低位元組4位）</para>
        /// </remarks>
        /// <param name="cmd">Gen8 命令執行物件</param>
        /// <param name="type">參數類型（如 _MS_BIN_FIRCOEF_SEL_TAP_NUM 等）</param>
        /// <param name="high">參數的高位元組值</param>
        /// <param name="low">參數的低位元組值</param>
        /// <param name="delay">命令發送後的延遲時間（毫秒）</param>
        private void SendGen8CommandPair(ElanCommand_Gen8 cmd, ElanCommand_Gen8.ParameterType type, int high, int low, int delay)
        {
            cmd.SendWriteCommand(type, high, low);
            OutputMessage(string.Format("-Set {0}=0x{1}{2}", cmd.m_cWriteCommandInfo.sParameterName, high.ToString("X4"), low.ToString("X4")));
            Thread.Sleep(delay);
        }

        /// <summary>
        /// 發送 Gen6 或 Gen7 IC 的參數設定命令
        /// </summary>
        /// <remarks>
        /// 執行流程：
        /// <list type="number">
        /// <item>透過 ElanCommand_Gen6or7 物件發送寫入命令</item>
        /// <item>輸出設定訊息到日誌（參數名稱與十六進位值，固定顯示 4 位）</item>
        /// <item>延遲正常延遲時間 (m_nNormalDelayTime) 等待命令完成</item>
        /// </list>
        /// <para>此方法用於 Gen6 和 Gen7 世代的參數設定，與 Gen8 的主要差異在於使用固定的正常延遲時間</para>
        /// <para>輸出格式：參數名稱=0xXXXX（固定4位十六進位）</para>
        /// </remarks>
        /// <param name="cmd">Gen6 或 Gen7 命令執行物件</param>
        /// <param name="type">參數類型（如 _MS_FIRTB, _MS_FIR_TAP_NUM, _MS_ANA_CTL_X 等）</param>
        /// <param name="value">參數值（整數）</param>
        private void SendGen6or7Command(ElanCommand_Gen6or7 cmd, ElanCommand_Gen6or7.ParameterType type, int value)
        {
            cmd.SendWriteCommand(type, value);
            OutputMessage(string.Format("-Set {0}=0x{1}", cmd.m_cWriteCommandInfo.sParameterName, value.ToString("X4")));
            Thread.Sleep(m_nNormalDelayTime);
        }

        /// <summary>
        /// 設定類比參數（適用於舊世代 IC）
        /// </summary>
        /// <remarks>
        /// 執行流程：
        /// <list type="number">
        /// <item>輸出設定訊息到日誌（參數名稱與十六進位值，固定顯示 2 位）</item>
        /// <item>透過 ElanTouchSwitch 設定類比參數到指定裝置</item>
        /// <item>延遲正常延遲時間 (m_nNormalDelayTime) 等待命令完成</item>
        /// </list>
        /// <para>此方法主要用於非 Gen6/Gen7/Gen8 世代的 IC，設定基本類比參數如 PH1, PH2, PH3, DFT_NUM 等</para>
        /// <para>輸出格式：參數名稱=0xXX（固定2位十六進位）</para>
        /// </remarks>
        /// <param name="name">參數名稱（用於日誌顯示，如 "PH1", "PH2", "DFT_NUM"）</param>
        /// <param name="paramType">參數類型代碼（ElanTouchSwitch 定義的參數常數，如 m_nPARAMETER_PH1）</param>
        /// <param name="value">參數值（整數）</param>
        private void SetAnalogParameter(string name, int paramType, int value)
        {
            OutputMessage(string.Format("-Set {0}=0x{1}", name, value.ToString("X2")));
            ElanTouchSwitch.SetAnalogParameter(paramType, value, m_nDeviceIndex, m_bSocketConnectType);
            Thread.Sleep(m_nNormalDelayTime);
        }

        /// <summary>
        /// 處理 Self K Sequence 校正參數設定
        /// </summary>
        /// <remarks>
        /// 根據 K Sequence 類型執行不同的校正參數設定方式：
        /// <list type="bullet">
        /// <item><strong>FixedValue (固定值模式)</strong>: 使用固定的 NCP 和 NCN 值進行校正參數設定</item>
        /// <item><strong>FileValue (檔案值模式)</strong>: 從指定的 ini 檔案讀取校正參數進行設定</item>
        /// </list>
        /// <para>兩種模式都支援可選的 CAL 值設定，以及指定 Trace 範圍的校正</para>
        /// <para>檔案路徑格式：{應用程式目錄}\{主目錄名稱}\ini\{Self FS_MS_MM_RX0 檔案名稱}</para>
        /// </remarks>
        /// <param name="cElanCommand">Gen8 命令執行物件</param>
        /// <param name="sSetSelfKSequence">K Sequence 類型（"FixedValue" 或 "FileValue"）</param>
        /// <param name="bSetSelfCALValue">是否設定 Self CAL 校正值</param>
        /// <param name="nSelfCALStartTrace">Self CAL 起始 Trace 索引位置</param>
        /// <param name="nSelfCALStartTraceNumber">Self CAL 起始 Trace 數量</param>
        /// <param name="nSelfCALEndTraceNumber">Self CAL 結束 Trace 數量</param>
        private void HandleSelfKSequence(
            ElanCommand_Gen8 cElanCommand,
            string sSetSelfKSequence,
            bool bSetSelfCALValue,
            int nSelfCALStartTrace,
            int nSelfCALStartTraceNumber,
            int nSelfCALEndTraceNumber)
        {
            if (sSetSelfKSequence == MainConstantParameter.m_sKSequence_FixedValue)
            {
                cElanCommand.RunSendSelfCalibrationParameter(
                    m_nSelfNCPValue, m_nSelfNCNValue, 
                    bSetSelfCALValue, m_cSelfParameter.m_nCAL, nSelfCALStartTrace, 
                    nSelfCALStartTraceNumber, nSelfCALEndTraceNumber
                );
            }
            else if (sSetSelfKSequence == MainConstantParameter.m_sKSequence_FileValue)
            {
                string sFilePath = string.Format(@"{0}\{1}\ini\{2}", Application.StartupPath, frmMain.m_sAPMainDirectoryName, ParamFingerAutoTuning.m_sSelfFS_MS_MM_RX0FileName);
                cElanCommand.RunSendSelfCalibrationParameterByFile(
                    sFilePath, bSetSelfCALValue, 
                    m_cSelfParameter.m_nCAL, nSelfCALStartTrace, nSelfCALStartTraceNumber, nSelfCALEndTraceNumber
                );
            }
        }

        #endregion

        #region 參數計算結構

        /// <summary>
        /// Gen8 Self 模式參數集合類別
        /// </summary>
        /// <remarks>
        /// 封裝 Gen8 IC Self 模式所需的所有暫存器參數，包括：
        /// <list type="bullet">
        /// <item><strong>基本參數</strong>: SP_NUM (起始點數量), EFFECT_NUM (有效數量), PKT_WC_L (封包字數低位元組)</item>
        /// <item><strong>延遲參數</strong>: PH2E_LAT (相位2結束延遲), PH2_LAT (相位2延遲)</item>
        /// <item><strong>ADC 參數</strong>: BSH_ADC_TP_NUM_H/L (基線 ADC 觸控點數量高低位元組)</item>
        /// <item><strong>係數參數</strong>: EFFECT_FW_SET_COEF_NUM_H/L (有效韌體設定係數數量高低位元組)</item>
        /// <item><strong>DFT/FIR 參數</strong>: DFT_NUM_IQ_FIR_CTL_H/L (DFT 數量與 IQ FIR 控制高低位元組)</item>
        /// <item><strong>類比控制參數</strong>: ANA_TP_CTL_01_H/L, ANA_TP_CTL_00_H/L (類比觸控控制暫存器高低位元組)</item>
        /// <item><strong>IQ 參數</strong>: IQ_BSH_GP0_GP1_H/L (IQ 基線 GP0/GP1 高低位元組)</item>
        /// </list>
        /// <para>此類別作為資料傳輸物件 (DTO)，用於在 Self 參數計算與發送之間傳遞完整的參數集</para>
        /// <para>大部分參數採用高低位元組分離的格式 (H/L)，以符合 Gen8 硬體通訊協定</para>
        /// </remarks>
        private class SelfParameters
        {
            public int SP_NUM, EFFECT_NUM, PKT_WC_L;
            public int PH2E_LAT, PH2_LAT;
            public int BSH_ADC_TP_NUM_H, BSH_ADC_TP_NUM_L;
            public int EFFECT_FW_SET_COEF_NUM_H, EFFECT_FW_SET_COEF_NUM_L;
            public int DFT_NUM_IQ_FIR_CTL_H, DFT_NUM_IQ_FIR_CTL_L;
            public int ANA_TP_CTL_01_H, ANA_TP_CTL_01_L;
            public int ANA_TP_CTL_00_H, ANA_TP_CTL_00_L;
            public int IQ_BSH_GP0_GP1_H, IQ_BSH_GP0_GP1_L;
        }

        /// <summary>
        /// Gen8 Raw ADC Sweep 參數集合類別
        /// </summary>
        /// <remarks>
        /// 封裝 Gen8 IC Raw ADC Sweep 模式所需的所有暫存器參數，包括：
        /// <list type="bullet">
        /// <item><strong>FIR 濾波器參數</strong>: BIN_FIRCOEF_SEL_TAP_NUM_H/L (二進位 FIR 係數選擇與 TAP 數量高低位元組)</item>
        /// <item><strong>類比觸控控制參數組 1</strong>: ANA_TP_CTL_01_H/L (類比觸控控制暫存器 01 高低位元組)</item>
        /// <item><strong>類比觸控控制參數組 1-2</strong>: ANA_TP_CTL_01_2_H/L (類比觸控控制暫存器 01-2 高低位元組，8F18 方案專用)</item>
        /// <item><strong>類比控制參數組 4</strong>: ANA_CTL_04_H/L (類比控制暫存器 04 高低位元組)</item>
        /// <item><strong>類比控制參數組 4-2</strong>: ANA_CTL_04_2_H/L (類比控制暫存器 04-2 高低位元組，8F18 方案專用)</item>
        /// <item><strong>類比觸控控制參數組 6</strong>: ANA_TP_CTL_06_H/L (類比觸控控制暫存器 06 高低位元組，8F18 方案專用)</item>
        /// <item><strong>類比觸控控制參數組 6-2</strong>: ANA_TP_CTL_06_2_H/L (類比觸控控制暫存器 06-2 高低位元組，8F18 方案專用)</item>
        /// <item><strong>類比觸控控制參數組 7</strong>: ANA_TP_CTL_07_H/L (類比觸控控制暫存器 07 高低位元組，8F18 方案專用)</item>
        /// </list>
        /// <para>此類別作為資料傳輸物件 (DTO)，用於在 Raw ADC Sweep 參數計算與發送之間傳遞完整的參數集</para>
        /// <para>所有參數採用高低位元組分離格式 (H/L)，以符合 Gen8 硬體通訊協定</para>
        /// <para>標註 "8F18 方案專用" 的參數僅在 Solution_8F18 方案中使用，其他方案會跳過這些參數</para>
        /// </remarks>
        private class RawADCSweepParameters
        {
            public int BIN_FIRCOEF_SEL_TAP_NUM_H, BIN_FIRCOEF_SEL_TAP_NUM_L;
            public int ANA_TP_CTL_01_H, ANA_TP_CTL_01_L;
            public int ANA_TP_CTL_01_2_H, ANA_TP_CTL_01_2_L;
            public int ANA_CTL_04_H, ANA_CTL_04_L;
            public int ANA_CTL_04_2_H, ANA_CTL_04_2_L;
            public int ANA_TP_CTL_06_H, ANA_TP_CTL_06_L;
            public int ANA_TP_CTL_06_2_H, ANA_TP_CTL_06_2_L;
            public int ANA_TP_CTL_07_H, ANA_TP_CTL_07_L;
        }

        /// <summary>
        /// Gen7 Raw ADC Sweep 參數集合類別
        /// </summary>
        /// <remarks>
        /// 封裝 Gen7 IC Raw ADC Sweep 模式所需的所有暫存器參數，包括：
        /// <list type="bullet">
        /// <item><strong>FIR 濾波器參數</strong>: FIRTB (FIR 濾波器表格), FIR_TAP_NUM (FIR TAP 數量)</item>
        /// <item><strong>類比控制參數</strong>: ANA_CTL_05 (類比控制暫存器 5), ANA_CTL_02 (類比控制暫存器 2), ANA_CTL_03 (類比控制暫存器 3), ANA_CTL_06 (類比控制暫存器 6)</item>
        /// </list>
        /// <para>此類別作為資料傳輸物件 (DTO)，用於在 Gen7 Raw ADC Sweep 參數計算與發送之間傳遞完整的參數集</para>
        /// <para>與 Gen8 不同，Gen7 參數使用單一整數值而非高低位元組分離格式，結構較為簡潔</para>
        /// <para>參數設定順序固定為：FIRTB → FIR_TAP_NUM → ANA_CTL_05 → ANA_CTL_02 → ANA_CTL_03 → ANA_CTL_06</para>
        /// </remarks>
        private class Gen7Parameters
        {
            public int FIRTB, FIR_TAP_NUM;
            public int ANA_CTL_05, ANA_CTL_02, ANA_CTL_03, ANA_CTL_06;
        }

        /// <summary>
        /// Gen6 Raw ADC Sweep 參數集合類別
        /// </summary>
        /// <remarks>
        /// 封裝 Gen6 IC Raw ADC Sweep 模式所需的所有暫存器參數，包括：
        /// <list type="bullet">
        /// <item><strong>FIR 濾波器參數</strong>: FIRTB (FIR 濾波器表格), FIR_TAP_NUM (FIR TAP 數量)</item>
        /// <item><strong>類比控制參數</strong>: ANA_CTL_08 (類比控制暫存器 8), ANA_CTL_05 (類比控制暫存器 5), ANA_CTL_04 (類比控制暫存器 4)</item>
        /// </list>
        /// <para>此類別作為資料傳輸物件 (DTO)，用於在 Gen6 Raw ADC Sweep 參數計算與發送之間傳遞完整的參數集</para>
        /// <para>與 Gen8 不同，Gen6 參數使用單一整數值而非高低位元組分離格式，結構較為簡潔</para>
        /// <para>參數設定順序固定為：FIRTB → FIR_TAP_NUM → ANA_CTL_08 → ANA_CTL_05 → ANA_CTL_04</para>
        /// </remarks>
        private class Gen6Parameters
        {
            public int FIRTB, FIR_TAP_NUM;
            public int ANA_CTL_08, ANA_CTL_05, ANA_CTL_04;
        }

        /// <summary>
        /// 計算 Gen8 Self 模式的參數值
        /// </summary>
        /// <remarks>
        /// 根據 bSetOrigin 參數決定計算方式：
        /// <list type="bullet">
        /// <item><strong>原始模式 (bSetOrigin = true)</strong>: 直接使用原始參數值，不進行任何計算</item>
        /// <item><strong>調整模式 (bSetOrigin = false)</strong>: 根據新的 DFT_NUM 重新計算相關參數
        ///   <list type="number">
        ///   <item>根據 DFT_NUM 差異調整 SP_NUM, EFFECT_NUM, PKT_WC_L</item>
        ///   <item>重新計算 BSH_ADC_TP_NUM (基線 ADC 觸控點數量)</item>
        ///   <item>重新計算 EFFECT_FW_SET_COEF_NUM (有效韌體設定係數數量)</item>
        ///   <item>重新計算 DFT_NUM_IQ_FIR_CTL (DFT 數量與 IQ FIR 控制)</item>
        ///   <item>若 Gain >= 0，調整 ANA_TP_CTL_01_L 的 Gain 位元欄位 (bit 3-6)</item>
        ///   <item>若 CAG >= 0，調整 ANA_TP_CTL_00_L 的 CAG 位元欄位 (bit 3-5)</item>
        ///   <item>若 IQ_BSH >= 0，調整 IQ_BSH_GP0_GP1_L 的 IQ_BSH 位元欄位 (bit 6-11)</item>
        ///   </list>
        /// </item>
        /// </list>
        /// <para>延遲參數 PH2E_LAT 和 PH2_LAT 會組合多個韌體參數值</para>
        /// </remarks>
        /// <param name="cFWParameter">包含新的韌體參數值的物件</param>
        /// <param name="bSetOrigin">是否使用原始參數值 (true: 原始值; false: 計算調整值)</param>
        /// <returns>計算完成的 Self 參數集合</returns>
        private SelfParameters CalculateSelfParameters(AppCoreDefine.FWParameter cFWParameter, bool bSetOrigin)
        {
            var result = new SelfParameters
            {
                SP_NUM = m_cOriginParameter.m_n_SELF_SP_NUM,
                EFFECT_NUM = m_cOriginParameter.m_n_SELF_EFFECT_NUM,
                PKT_WC_L = m_cOriginParameter.m_n_SELF_PKT_WC_L,
                PH2E_LAT = (cFWParameter.m_n_SELF_PH2E_LMT << 8) | cFWParameter.m_n_SELF_PH2E_LAT,
                PH2_LAT = (cFWParameter.m_n_SELF_PH2_LAT << 8) | cFWParameter.m_n_SELF_PH2_MUX_LAT,
                BSH_ADC_TP_NUM_H = m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_H,
                BSH_ADC_TP_NUM_L = m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_L,
                EFFECT_FW_SET_COEF_NUM_H = m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H,
                EFFECT_FW_SET_COEF_NUM_L = m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L,
                DFT_NUM_IQ_FIR_CTL_H = m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H,
                DFT_NUM_IQ_FIR_CTL_L = m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L,
                ANA_TP_CTL_01_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H,
                ANA_TP_CTL_01_L = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L,
                ANA_TP_CTL_00_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H,
                ANA_TP_CTL_00_L = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L,
                IQ_BSH_GP0_GP1_H = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_H,
                IQ_BSH_GP0_GP1_L = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_L
            };

            if (!bSetOrigin)
            {
                result.SP_NUM = (m_cOriginParameter.m_n_SELF_SP_NUM - m_cOriginParameter.m_nSelf_DFT_NUM) + cFWParameter.m_nSelf_DFT_NUM;
                result.EFFECT_NUM = (m_cOriginParameter.m_n_SELF_EFFECT_NUM - m_cOriginParameter.m_nSelf_DFT_NUM) + cFWParameter.m_nSelf_DFT_NUM;
                result.PKT_WC_L = result.EFFECT_NUM * m_nRXTraceNumber;

                result.BSH_ADC_TP_NUM_H = cFWParameter.m_nSelf_DFT_NUM << 4;
                result.BSH_ADC_TP_NUM_L = m_nRXTraceNumber;

                result.EFFECT_FW_SET_COEF_NUM_H = cFWParameter.m_nSelf_DFT_NUM;
                result.EFFECT_FW_SET_COEF_NUM_L = 0;

                result.DFT_NUM_IQ_FIR_CTL_H = cFWParameter.m_nSelf_DFT_NUM;
                result.DFT_NUM_IQ_FIR_CTL_L = 0x1000;

                if (cFWParameter.m_nSelf_Gain >= 0)
                {
                    result.ANA_TP_CTL_01_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H;
                    result.ANA_TP_CTL_01_L = (m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L & 0xFF87) | (cFWParameter.m_nSelf_Gain << 3);
                }

                if (cFWParameter.m_nSelf_CAG >= 0)
                {
                    result.ANA_TP_CTL_00_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H;
                    result.ANA_TP_CTL_00_L = (m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L & 0xFFC7) | (cFWParameter.m_nSelf_CAG << 3);
                }

                if (cFWParameter.m_nSelf_IQ_BSH >= 0)
                {
                    result.IQ_BSH_GP0_GP1_H = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_H;
                    result.IQ_BSH_GP0_GP1_L = (m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_L & 0xF03F) | (cFWParameter.m_nSelf_IQ_BSH << 6);
                }
            }

            return result;
        }

        /// <summary>
        /// 計算 Gen8 Raw ADC Sweep 模式的參數值
        /// </summary>
        /// <remarks>
        /// 根據 bSetOrigin 參數決定計算方式：
        /// <list type="bullet">
        /// <item><strong>原始模式 (bSetOrigin = true)</strong>: 直接使用原始參數值，不進行任何計算</item>
        /// <item><strong>調整模式 (bSetOrigin = false)</strong>: 根據新的韌體參數重新計算暫存器值
        ///   <list type="number">
        ///   <item>計算 BIN_FIRCOEF_SEL_TAP_NUM: 組合 FIR_TAP_NUM (bit 4-10) 和 FIRCOEF_SEL (bit 0-3)</item>
        ///   <item>計算 ANA_TP_CTL_01: 組合 SELC (bit 7-14) 和 SELGM (bit 3-6)</item>
        ///   <item>計算 ANA_CTL_04: 設定 VSEL (bit 0-1)</item>
        ///   <item><strong>8F18 方案專用</strong>: 額外計算 ANA_TP_CTL_01_2, ANA_CTL_04_2, ANA_TP_CTL_06 系列和 ANA_TP_CTL_07</item>
        ///   </list>
        /// </item>
        /// </list>
        /// <para>使用位元遮罩操作保留原始暫存器中不需修改的位元</para>
        /// <para>8F18 方案需要設定額外的暫存器參數以支援進階功能</para>
        /// </remarks>
        /// <param name="cFWParameter">包含新的韌體參數值的物件</param>
        /// <param name="bSetOrigin">是否使用原始參數值 (true: 原始值; false: 計算調整值)</param>
        /// <returns>計算完成的 Raw ADC Sweep 參數集合</returns>
        private RawADCSweepParameters CalculateRawADCSweepParameters(AppCoreDefine.FWParameter cFWParameter, bool bSetOrigin)
        {
            var result = new RawADCSweepParameters
            {
                BIN_FIRCOEF_SEL_TAP_NUM_H = m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H,
                BIN_FIRCOEF_SEL_TAP_NUM_L = m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L,
                ANA_TP_CTL_01_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_H,
                ANA_TP_CTL_01_L = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_L,
                ANA_TP_CTL_01_2_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_H,
                ANA_TP_CTL_01_2_L = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_L,
                ANA_CTL_04_H = m_cOriginParameter.m_n_MS_ANA_CTL_04_H,
                ANA_CTL_04_L = m_cOriginParameter.m_n_MS_ANA_CTL_04_L,
                ANA_CTL_04_2_H = m_cOriginParameter.m_n_MS_ANA_CTL_04_2_H,
                ANA_CTL_04_2_L = m_cOriginParameter.m_n_MS_ANA_CTL_04_2_L,
                ANA_TP_CTL_06_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_H,
                ANA_TP_CTL_06_L = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_L,
                ANA_TP_CTL_06_2_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_H,
                ANA_TP_CTL_06_2_L = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_L,
                ANA_TP_CTL_07_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_H,
                ANA_TP_CTL_07_L = m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_L
            };

            if (!bSetOrigin)
            {
                result.BIN_FIRCOEF_SEL_TAP_NUM_H = (m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H & 0xF80F) | (cFWParameter.m_n_MS_FIR_TAP_NUM << 4);
                result.BIN_FIRCOEF_SEL_TAP_NUM_L = (m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L & 0xFFF0) | cFWParameter.m_n_MS_FIRCOEF_SEL;

                result.ANA_TP_CTL_01_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_H;
                result.ANA_TP_CTL_01_L = (m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_L & 0x7E07) | (cFWParameter.m_n_MS_SELC << 7) | (cFWParameter.m_n_MS_SELGM << 3);

                result.ANA_CTL_04_H = m_cOriginParameter.m_n_MS_ANA_CTL_04_H;
                result.ANA_CTL_04_L = (m_cOriginParameter.m_n_MS_ANA_CTL_04_L & 0xFFFC) | cFWParameter.m_n_MS_VSEL;

                if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                {
                    result.ANA_TP_CTL_01_2_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_H;
                    result.ANA_TP_CTL_01_2_L = (m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_L & 0x7E07) | (cFWParameter.m_n_MS_SELC << 7) | (cFWParameter.m_n_MS_SELGM << 3);

                    result.ANA_CTL_04_2_H = m_cOriginParameter.m_n_MS_ANA_CTL_04_2_H;
                    result.ANA_CTL_04_2_L = (m_cOriginParameter.m_n_MS_ANA_CTL_04_2_L & 0xFFFC) | cFWParameter.m_n_MS_VSEL;

                    result.ANA_TP_CTL_06_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_H;
                    result.ANA_TP_CTL_06_L = (m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_L & 0xFFFC) | cFWParameter.m_n_MS_LG;
                    result.ANA_TP_CTL_06_2_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_H;
                    result.ANA_TP_CTL_06_2_L = (m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_L & 0xFFFC) | cFWParameter.m_n_MS_LG;
                    result.ANA_TP_CTL_07_H = m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_H;
                    result.ANA_TP_CTL_07_L = (m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_L & 0xFC3C) | (cFWParameter.m_n_MS_SELGM << 6) | cFWParameter.m_n_MS_LG;
                }
            }

            return result;
        }

        /// <summary>
        /// 計算 Gen7 Raw ADC Sweep 模式的參數值
        /// </summary>
        /// <remarks>
        /// 根據 bSetOrigin 參數決定計算方式：
        /// <list type="bullet">
        /// <item><strong>原始模式 (bSetOrigin = true)</strong>: 直接使用原始參數值，不進行任何計算</item>
        /// <item><strong>調整模式 (bSetOrigin = false)</strong>: 根據新的韌體參數重新計算暫存器值
        ///   <list type="number">
        ///   <item>直接使用新的 FIRTB 和 FIR_TAP_NUM 值</item>
        ///   <item>計算 ANA_CTL_05: 設定 SELC (bit 2-3)</item>
        ///   <item>計算 ANA_CTL_02: 設定 VSEL (bit 1-2)</item>
        ///   <item>計算 ANA_CTL_03: 設定 VSEL (bit 1-2)</item>
        ///   <item>計算 ANA_CTL_06: 組合 SELGM (bit 8-11) 和 LG (bit 3-7)</item>
        ///   </list>
        /// </item>
        /// </list>
        /// <para>使用位元遮罩操作保留原始暫存器中不需修改的位元</para>
        /// <para>與 Gen8 相比，Gen7 的參數結構較簡單，不需要高低位元組分離</para>
        /// </remarks>
        /// <param name="cFWParameter">包含新的韌體參數值的物件</param>
        /// <param name="bSetOrigin">是否使用原始參數值 (true: 原始值; false: 計算調整值)</param>
        /// <returns>計算完成的 Gen7 參數集合</returns>
        private Gen7Parameters CalculateGen7Parameters(AppCoreDefine.FWParameter cFWParameter, bool bSetOrigin)
        {
            var result = new Gen7Parameters
            {
                FIRTB = m_cOriginParameter.m_n_MS_FIRTB,
                FIR_TAP_NUM = m_cOriginParameter.m_n_MS_FIR_TAP_NUM,
                ANA_CTL_05 = m_cOriginParameter.m_n_MS_ANA_CTL_05,
                ANA_CTL_02 = m_cOriginParameter.m_n_MS_ANA_CTL_02,
                ANA_CTL_03 = m_cOriginParameter.m_n_MS_ANA_CTL_03,
                ANA_CTL_06 = m_cOriginParameter.m_n_MS_ANA_CTL_06
            };

            if (!bSetOrigin)
            {
                result.FIRTB = cFWParameter.m_n_MS_FIRTB;
                result.FIR_TAP_NUM = cFWParameter.m_n_MS_FIR_TAP_NUM;
                result.ANA_CTL_05 = (m_cOriginParameter.m_n_MS_ANA_CTL_05 & 0xFFF3) | (cFWParameter.m_n_MS_SELC << 2);
                result.ANA_CTL_02 = (m_cOriginParameter.m_n_MS_ANA_CTL_02 & 0xFFF9) | (cFWParameter.m_n_MS_VSEL << 1);
                result.ANA_CTL_03 = (m_cOriginParameter.m_n_MS_ANA_CTL_03 & 0xFFF9) | (cFWParameter.m_n_MS_VSEL << 1);
                result.ANA_CTL_06 = (m_cOriginParameter.m_n_MS_ANA_CTL_06 & 0xF0E7) | (cFWParameter.m_n_MS_SELGM << 8) | (cFWParameter.m_n_MS_LG << 3);
            }

            return result;
        }

        /// <summary>
        /// 計算 Gen6 Raw ADC Sweep 模式的參數值
        /// </summary>
        /// <remarks>
        /// 根據 bSetOrigin 參數決定計算方式：
        /// <list type="bullet">
        /// <item><strong>原始模式 (bSetOrigin = true)</strong>: 直接使用原始參數值，不進行任何計算</item>
        /// <item><strong>調整模式 (bSetOrigin = false)</strong>: 根據新的韌體參數重新計算暫存器值
        ///   <list type="number">
        ///   <item>直接使用新的 FIRTB 和 FIR_TAP_NUM 值</item>
        ///   <item>計算 ANA_CTL_08: 組合 SELC (bit 6-7) 和 LG (bit 2-3)</item>
        ///   <item>計算 ANA_CTL_05: 設定 VSEL (bit 6-7)</item>
        ///   <item>計算 ANA_CTL_04: 設定 SELGM (bit 12-14)</item>
        ///   </list>
        /// </item>
        /// </list>
        /// <para>使用位元遮罩操作保留原始暫存器中不需修改的位元</para>
        /// <para>Gen6 的參數配置與 Gen7 類似但位元欄位位置不同，需注意各參數的位元遮罩</para>
        /// </remarks>
        /// <param name="cFWParameter">包含新的韌體參數值的物件</param>
        /// <param name="bSetOrigin">是否使用原始參數值 (true: 原始值; false: 計算調整值)</param>
        /// <returns>計算完成的 Gen6 參數集合</returns>
        private Gen6Parameters CalculateGen6Parameters(AppCoreDefine.FWParameter cFWParameter, bool bSetOrigin)
        {
            var result = new Gen6Parameters
            {
                FIRTB = m_cOriginParameter.m_n_MS_FIRTB,
                FIR_TAP_NUM = m_cOriginParameter.m_n_MS_FIR_TAP_NUM,
                ANA_CTL_08 = m_cOriginParameter.m_n_MS_ANA_CTL_08,
                ANA_CTL_05 = m_cOriginParameter.m_n_MS_ANA_CTL_05,
                ANA_CTL_04 = m_cOriginParameter.m_n_MS_ANA_CTL_04
            };

            if (!bSetOrigin)
            {
                result.FIRTB = cFWParameter.m_n_MS_FIRTB;
                result.FIR_TAP_NUM = cFWParameter.m_n_MS_FIR_TAP_NUM;
                result.ANA_CTL_08 = (m_cOriginParameter.m_n_MS_ANA_CTL_08 & 0xFF33) | (cFWParameter.m_n_MS_SELC << 6) | (cFWParameter.m_n_MS_LG << 2);
                result.ANA_CTL_05 = (m_cOriginParameter.m_n_MS_ANA_CTL_05 & 0xFF3F) | (cFWParameter.m_n_MS_VSEL << 6);
                result.ANA_CTL_04 = (m_cOriginParameter.m_n_MS_ANA_CTL_04 & 0x8FFF) | (cFWParameter.m_n_MS_SELGM << 12);
            }

            return result;
        }

        #endregion
#endif
    }
}
