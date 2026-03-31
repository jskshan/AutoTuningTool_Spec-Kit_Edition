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
        private bool CheckAnalogParameter(frmMain.FlowStep cFlowStep, bool bLastRetry, FrequencyItem cFrequencyItem = null, RawADCSweepItem cRawADCSweepItem = null)
        {
            AppCoreDefine.FWParameterInfo[] cFWParameterInfo_Array = null;
            
            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_PH1 = new AppCoreDefine.FWParameterInfo("_SELF_PH1", cFrequencyItem.m_n_SELF_PH1, m_cReadParameter.m_n_SELF_PH1);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_PH2E_LAT = new AppCoreDefine.FWParameterInfo("_SELF_PH2E_LAT", cFrequencyItem.m_n_SELF_PH2E_LAT, m_cReadParameter.m_n_SELF_PH2E_LAT);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_PH2E_LMT = new AppCoreDefine.FWParameterInfo("_SELF_PH2E_LMT", cFrequencyItem.m_n_SELF_PH2E_LMT, m_cReadParameter.m_n_SELF_PH2E_LMT);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_PH2_LAT = new AppCoreDefine.FWParameterInfo("_SELF_PH2_LAT", cFrequencyItem.m_n_SELF_PH2_LAT, m_cReadParameter.m_n_SELF_PH2_LAT);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_PH2 = new AppCoreDefine.FWParameterInfo("_SELF_PH2", cFrequencyItem.m_n_SELF_PH2, m_cReadParameter.m_n_SELF_PH2);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_Self_DFT_NUM = new AppCoreDefine.FWParameterInfo("Self_DFT_NUM", m_cSelfParameter.m_nDFT_NUM, m_cReadParameter.m_nSelf_DFT_NUM);

                AppCoreDefine.FWParameterInfo cFWParameterInfo_Self_Gain = (m_cSelfParameter.m_nGain >= 0) ? new AppCoreDefine.FWParameterInfo("Self_Gain", m_cSelfParameter.m_nGain, m_cReadParameter.m_nSelf_Gain) :
                                                                                                             new AppCoreDefine.FWParameterInfo("Self_Gain", m_cOriginParameter.m_nSelf_Gain, m_cReadParameter.m_nSelf_Gain);

                AppCoreDefine.FWParameterInfo cFWParameterInfo_Self_CAG = (m_cSelfParameter.m_nCAG >= 0) ? new AppCoreDefine.FWParameterInfo("Self_CAG", m_cSelfParameter.m_nCAG, m_cReadParameter.m_nSelf_CAG) :
                                                                                                           new AppCoreDefine.FWParameterInfo("Self_CAG", m_cOriginParameter.m_nSelf_CAG, m_cReadParameter.m_nSelf_CAG);

                AppCoreDefine.FWParameterInfo cFWParameterInfo_Self_IQ_BSH = (m_cSelfParameter.m_nIQ_BSH >= 0) ? new AppCoreDefine.FWParameterInfo("Self_IQ_BSH", m_cSelfParameter.m_nIQ_BSH, m_cReadParameter.m_nSelf_IQ_BSH) :
                                                                                                                 new AppCoreDefine.FWParameterInfo("Self_IQ_BSH", m_cOriginParameter.m_nSelf_IQ_BSH, m_cReadParameter.m_nSelf_IQ_BSH);

                int n_SELF_SP_NUM = (m_cOriginParameter.m_n_SELF_SP_NUM - m_cOriginParameter.m_nSelf_DFT_NUM) + m_cSelfParameter.m_nDFT_NUM;
                int n_SELF_EFFECT_NUM = (m_cOriginParameter.m_n_SELF_EFFECT_NUM - m_cOriginParameter.m_nSelf_DFT_NUM) + m_cSelfParameter.m_nDFT_NUM;
                int n_SELF_PKT_WC_L = n_SELF_EFFECT_NUM * m_nRXTraceNumber;

                int n_SELF_BSH_ADC_TP_NUM_H = m_cSelfParameter.m_nDFT_NUM << 4;
                int n_SELF_BSH_ADC_TP_NUM_L = m_nRXTraceNumber;

                int n_SELF_EFFECT_FW_SET_COEF_NUM_H = m_cSelfParameter.m_nDFT_NUM;
                int n_SELF_EFFECT_FW_SET_COEF_NUM_L = 0;

                int n_SELF_DFT_NUM_IQ_FIR_CTL_H = m_cSelfParameter.m_nDFT_NUM;
                int n_SELF_DFT_NUM_IQ_FIR_CTL_L = 0x1000;

                int n_SELF_ANA_TP_CTL_01_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H;
                int n_SELF_ANA_TP_CTL_01_L = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L;

                int n_SELF_ANA_TP_CTL_00_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H;
                int n_SELF_ANA_TP_CTL_00_L = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L;

                int n_SELF_IQ_BSH_GP0_GP1_H = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_H;
                int n_SELF_IQ_BSH_GP0_GP1_L = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_L;

                if (m_cSelfParameter.m_nGain >= 0)
                {
                    n_SELF_ANA_TP_CTL_01_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H;
                    n_SELF_ANA_TP_CTL_01_L = (m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L & 0xFF87) | (m_cSelfParameter.m_nGain << 3);
                }

                if (m_cSelfParameter.m_nCAG >= 0)
                {
                    n_SELF_ANA_TP_CTL_00_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H;
                    n_SELF_ANA_TP_CTL_00_L = (m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L & 0xFFC7) | (m_cSelfParameter.m_nCAG << 3);
                }

                if (m_cSelfParameter.m_nIQ_BSH >= 0)
                {
                    n_SELF_IQ_BSH_GP0_GP1_H = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_H;
                    n_SELF_IQ_BSH_GP0_GP1_L = (m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_L & 0xF03F) | (m_cSelfParameter.m_nIQ_BSH << 6);
                }

                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_SP_NUM = new AppCoreDefine.FWParameterInfo("_SELF_SP_NUM", n_SELF_SP_NUM, m_cReadParameter.m_n_SELF_SP_NUM);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_EFFECT_NUM = new AppCoreDefine.FWParameterInfo("_SELF_EFFECT_NUM", n_SELF_EFFECT_NUM, m_cReadParameter.m_n_SELF_EFFECT_NUM);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_PKT_WC_L = new AppCoreDefine.FWParameterInfo("_SELF_PKT_WC_L", n_SELF_PKT_WC_L, m_cReadParameter.m_n_SELF_PKT_WC_L);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_BSH_ADC_TP_NUM_H = new AppCoreDefine.FWParameterInfo("_SELF_BSH_ADC_TP_NUM_H", n_SELF_BSH_ADC_TP_NUM_H, m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_H);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_BSH_ADC_TP_NUM_L = new AppCoreDefine.FWParameterInfo("_SELF_BSH_ADC_TP_NUM_L", n_SELF_BSH_ADC_TP_NUM_L, m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_L);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_EFFECT_FW_SET_COEF_NUM_H = new AppCoreDefine.FWParameterInfo("_SELF_EFFECT_FW_SET_COEF_NUM_H", n_SELF_EFFECT_FW_SET_COEF_NUM_H, m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_EFFECT_FW_SET_COEF_NUM_L = new AppCoreDefine.FWParameterInfo("_SELF_EFFECT_FW_SET_COEF_NUM_L", n_SELF_EFFECT_FW_SET_COEF_NUM_L, m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_DFT_NUM_IQ_FIR_CTL_H = new AppCoreDefine.FWParameterInfo("_SELF_DFT_NUM_IQ_FIR_CTL_H", n_SELF_DFT_NUM_IQ_FIR_CTL_H, m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_DFT_NUM_IQ_FIR_CTL_L = new AppCoreDefine.FWParameterInfo("_SELF_DFT_NUM_IQ_FIR_CTL_L", n_SELF_DFT_NUM_IQ_FIR_CTL_L, m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_ANA_TP_CTL_01_H = new AppCoreDefine.FWParameterInfo("_SELF_ANA_TP_CTL_01_H", n_SELF_ANA_TP_CTL_01_H, m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_H);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_ANA_TP_CTL_01_L = new AppCoreDefine.FWParameterInfo("_SELF_ANA_TP_CTL_01_L", n_SELF_ANA_TP_CTL_01_L, m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_L);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_ANA_TP_CTL_00_H = new AppCoreDefine.FWParameterInfo("_SELF_ANA_TP_CTL_00_H", n_SELF_ANA_TP_CTL_00_H, m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_H);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_ANA_TP_CTL_00_L = new AppCoreDefine.FWParameterInfo("_SELF_ANA_TP_CTL_00_L", n_SELF_ANA_TP_CTL_00_L, m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_L);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_IQ_BSH_GP0_GP1_H = new AppCoreDefine.FWParameterInfo("_SELF_IQ_BSH_GP0_GP1_H", n_SELF_IQ_BSH_GP0_GP1_H, m_cReadParameter.m_n_SELF_IQ_BSH_GP0_GP1_H);
                AppCoreDefine.FWParameterInfo cFWParameterInfo_SELF_IQ_BSH_GP0_GP1_L = new AppCoreDefine.FWParameterInfo("_SELF_IQ_BSH_GP0_GP1_L", n_SELF_IQ_BSH_GP0_GP1_L, m_cReadParameter.m_n_SELF_IQ_BSH_GP0_GP1_L);

                cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                {
                    cFWParameterInfo_SELF_PH1,
                    cFWParameterInfo_SELF_PH2E_LAT,
                    cFWParameterInfo_SELF_PH2E_LMT,
                    cFWParameterInfo_SELF_PH2_LAT,
                    cFWParameterInfo_SELF_PH2,
                    cFWParameterInfo_Self_DFT_NUM,
                    cFWParameterInfo_Self_Gain,
                    cFWParameterInfo_SELF_SP_NUM,
                    cFWParameterInfo_SELF_EFFECT_NUM,
                    cFWParameterInfo_SELF_PKT_WC_L,
                    cFWParameterInfo_SELF_BSH_ADC_TP_NUM_H,
                    cFWParameterInfo_SELF_BSH_ADC_TP_NUM_L,
                    cFWParameterInfo_SELF_EFFECT_FW_SET_COEF_NUM_H,
                    cFWParameterInfo_SELF_EFFECT_FW_SET_COEF_NUM_L,
                    cFWParameterInfo_SELF_DFT_NUM_IQ_FIR_CTL_H,
                    cFWParameterInfo_SELF_DFT_NUM_IQ_FIR_CTL_L,
                    cFWParameterInfo_SELF_ANA_TP_CTL_01_H,
                    cFWParameterInfo_SELF_ANA_TP_CTL_01_L,
                    cFWParameterInfo_SELF_ANA_TP_CTL_00_H,
                    cFWParameterInfo_SELF_ANA_TP_CTL_00_L,
                    cFWParameterInfo_SELF_IQ_BSH_GP0_GP1_H,
                    cFWParameterInfo_SELF_IQ_BSH_GP0_GP1_L
                };
            }
            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_FIRCOEF_SEL = new AppCoreDefine.FWParameterInfo("c_MS_FIRCOEF_SEL", cRawADCSweepItem.m_nFIRCOEF_SEL, m_cReadParameter.m_n_MS_FIRCOEF_SEL);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_FIR_TAP_NUM = new AppCoreDefine.FWParameterInfo("c_MS_FIR_TAP_NUM", cRawADCSweepItem.m_nFIR_TAP_NUM, m_cReadParameter.m_n_MS_FIR_TAP_NUM);
                    
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_SELC = new AppCoreDefine.FWParameterInfo("c_MS_SELC", cRawADCSweepItem.m_nSELC, m_cReadParameter.m_n_MS_SELC);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_REG_VSEL = new AppCoreDefine.FWParameterInfo("c_MS_REG_VSEL", cRawADCSweepItem.m_nVSEL, m_cReadParameter.m_n_MS_VSEL);

                    if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                    {
                        AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_LG = new AppCoreDefine.FWParameterInfo("c_MS_LG", cRawADCSweepItem.m_nLG, m_cReadParameter.m_n_MS_LG);

                        /*
                        if (ParamFingerAutoTuning.m_nRawADCSFixedSELGM >= 0)
                        {
                            AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_SELGM = new AppCoreDefine.FWParameterInfo("c_MS_SELGM", cRawADCSweepItem.m_nSELGM, m_cReadParameter.m_n_MS_SELGM);

                            cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                            {
                                cFWParameterInfo_MS_FIRCOEF_SEL,
                                cFWParameterInfo_MS_FIR_TAP_NUM,
                                cFWParameterInfo_MS_SELGM,
                                cFWParameterInfo_MS_SELC,
                                cFWParameterInfo_MS_REG_VSEL,
                                cFWParameterInfo_MS_LG
                            };
                        }
                        else
                        {
                            cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                            {
                                cFWParameterInfo_MS_FIRCOEF_SEL,
                                cFWParameterInfo_MS_FIR_TAP_NUM,
                                cFWParameterInfo_MS_SELC,
                                cFWParameterInfo_MS_REG_VSEL,
                                cFWParameterInfo_MS_LG
                            };
                        }
                        */

                        cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                        {
                            cFWParameterInfo_MS_FIRCOEF_SEL,
                            cFWParameterInfo_MS_FIR_TAP_NUM,
                            cFWParameterInfo_MS_SELC,
                            cFWParameterInfo_MS_REG_VSEL,
                            cFWParameterInfo_MS_LG
                        };
                    }
                    else
                    {
                        /*
                        if (ParamFingerAutoTuning.m_nRawADCSFixedSELGM >= 0)
                        {
                            AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_SELGM = new AppCoreDefine.FWParameterInfo("c_MS_SELGM", cRawADCSweepItem.m_nSELGM, m_cReadParameter.m_n_MS_SELGM);

                            cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                            {
                                cFWParameterInfo_MS_FIRCOEF_SEL,
                                cFWParameterInfo_MS_FIR_TAP_NUM,
                                cFWParameterInfo_MS_SELGM,
                                cFWParameterInfo_MS_SELC,
                                cFWParameterInfo_MS_REG_VSEL
                            };
                        }
                        else
                        {
                            cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                            {
                                cFWParameterInfo_MS_FIRCOEF_SEL,
                                cFWParameterInfo_MS_FIR_TAP_NUM,
                                cFWParameterInfo_MS_SELC,
                                cFWParameterInfo_MS_REG_VSEL
                            };
                        }
                        */

                        cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                        {
                            cFWParameterInfo_MS_FIRCOEF_SEL,
                            cFWParameterInfo_MS_FIR_TAP_NUM,
                            cFWParameterInfo_MS_SELC,
                            cFWParameterInfo_MS_REG_VSEL
                        };
                    }
                }
                else if (m_eICGenerationType == ICGenerationType.Gen7)
                {
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_FIRTB = new AppCoreDefine.FWParameterInfo("_MS_FIRTB", cRawADCSweepItem.m_nFIRTB, m_cReadParameter.m_n_MS_FIRTB);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_FIR_TAP_NUM = new AppCoreDefine.FWParameterInfo("_MS_FIR_TAP_NUM", cRawADCSweepItem.m_nFIR_TAP_NUM, m_cReadParameter.m_n_MS_FIR_TAP_NUM);
                    
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_SELC = new AppCoreDefine.FWParameterInfo("_SELC", cRawADCSweepItem.m_nSELC, m_cReadParameter.m_n_MS_SELC);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_VF_VSEL = new AppCoreDefine.FWParameterInfo("_VF_VSEL", cRawADCSweepItem.m_nVSEL, m_cReadParameter.m_n_MS_VSEL);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_LG = new AppCoreDefine.FWParameterInfo("_LG", cRawADCSweepItem.m_nLG, m_cReadParameter.m_n_MS_LG);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_SELGM = new AppCoreDefine.FWParameterInfo("_SELGM", cRawADCSweepItem.m_nSELGM, m_cReadParameter.m_n_MS_SELGM);

                    cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                    {
                        cFWParameterInfo_MS_FIRTB,
                        cFWParameterInfo_MS_FIR_TAP_NUM,
                        cFWParameterInfo_SELC,
                        cFWParameterInfo_VF_VSEL,
                        cFWParameterInfo_LG,
                        cFWParameterInfo_SELGM
                    };
                }
                else if (m_eICGenerationType == ICGenerationType.Gen6)
                {
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_FIRTB = new AppCoreDefine.FWParameterInfo("_MS_FIRTB", cRawADCSweepItem.m_nFIRTB, m_cReadParameter.m_n_MS_FIRTB);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_FIR_TAP_NUM = new AppCoreDefine.FWParameterInfo("_MS_FIR_TAP_NUM", cRawADCSweepItem.m_nFIR_TAP_NUM, m_cReadParameter.m_n_MS_FIR_TAP_NUM);

                    AppCoreDefine.FWParameterInfo cFWParameterInfo_SELC = new AppCoreDefine.FWParameterInfo("_SELC", cRawADCSweepItem.m_nSELC, m_cReadParameter.m_n_MS_SELC);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_VF_VSEL = new AppCoreDefine.FWParameterInfo("_VF_VSEL", cRawADCSweepItem.m_nVSEL, m_cReadParameter.m_n_MS_VSEL);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_LG = new AppCoreDefine.FWParameterInfo("_LG", cRawADCSweepItem.m_nLG, m_cReadParameter.m_n_MS_LG);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_SELGM = new AppCoreDefine.FWParameterInfo("_SELGM", cRawADCSweepItem.m_nSELGM, m_cReadParameter.m_n_MS_SELGM);

                    cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                    {
                        cFWParameterInfo_MS_FIRTB,
                        cFWParameterInfo_MS_FIR_TAP_NUM,
                        cFWParameterInfo_SELC,
                        cFWParameterInfo_VF_VSEL,
                        cFWParameterInfo_LG,
                        cFWParameterInfo_SELGM
                    };
                }
            }
            else
            {
                if (m_eICGenerationType == ICGenerationType.Gen9)
                {
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_PH1 = new AppCoreDefine.FWParameterInfo("PH1", cFrequencyItem.m_nPH1, m_cReadParameter.m_nPH1);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_PH2 = new AppCoreDefine.FWParameterInfo("PH2", cFrequencyItem.m_nPH2, m_cReadParameter.m_nPH2);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_PH3 = new AppCoreDefine.FWParameterInfo("PH3", cFrequencyItem.m_nPH2, m_cReadParameter.m_nPH3);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_DFT_NUM = new AppCoreDefine.FWParameterInfo("DFT_NUM", cFrequencyItem.m_nDFT_NUM, m_cReadParameter.m_n_MS_DFT_NUM);

                    cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                    {
                        cFWParameterInfo_PH1,
                        cFWParameterInfo_PH2,
                        cFWParameterInfo_PH3,
                        cFWParameterInfo_DFT_NUM
                    };
                }
                else
                {
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_PH1 = new AppCoreDefine.FWParameterInfo("PH1", cFrequencyItem.m_nPH1, m_cReadParameter.m_nPH1);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_PH2 = new AppCoreDefine.FWParameterInfo("PH2", cFrequencyItem.m_nPH2, m_cReadParameter.m_nPH2);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_PH3 = new AppCoreDefine.FWParameterInfo("PH3", cFrequencyItem.m_nPH2, m_cReadParameter.m_nPH3);
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_DFT_NUM = new AppCoreDefine.FWParameterInfo("DFT_NUM", cFrequencyItem.m_nDFT_NUM, m_cReadParameter.m_n_MS_DFT_NUM);

                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_SP_NUM;
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_EFFECT_NUM;
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_PKT_WC;

                    if (m_eICGenerationType == ICGenerationType.Gen8)
                    {
                        int n_MS_SP_NUM = (m_cOriginParameter.m_n_MS_SP_NUM - m_cOriginParameter.m_n_MS_DFT_NUM) + cFrequencyItem.m_nDFT_NUM;
                        int n_MS_EFFECT_NUM = (m_cOriginParameter.m_n_MS_EFFECT_NUM - m_cOriginParameter.m_n_MS_DFT_NUM) + cFrequencyItem.m_nDFT_NUM;
                        int nPKT_WC = n_MS_EFFECT_NUM * m_nRXTraceNumber;

                        cFWParameterInfo_MS_SP_NUM = new AppCoreDefine.FWParameterInfo("_MS_SP_NUM", n_MS_SP_NUM, m_cReadParameter.m_n_MS_SP_NUM);
                        cFWParameterInfo_MS_EFFECT_NUM = new AppCoreDefine.FWParameterInfo("_MS_EFFECT_NUM", n_MS_EFFECT_NUM, m_cReadParameter.m_n_MS_EFFECT_NUM);
                        cFWParameterInfo_PKT_WC = new AppCoreDefine.FWParameterInfo("PKT_WC", nPKT_WC, m_cReadParameter.m_nPKT_WC);

                        cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                        {
                            cFWParameterInfo_PH1,
                            cFWParameterInfo_PH2,
                            cFWParameterInfo_PH3,
                            cFWParameterInfo_DFT_NUM,
                            cFWParameterInfo_MS_SP_NUM ,
                            cFWParameterInfo_MS_EFFECT_NUM,
                            cFWParameterInfo_PKT_WC
                        };
                    }
                    else
                    {
                        cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                        {
                            cFWParameterInfo_PH1,
                            cFWParameterInfo_PH2,
                            cFWParameterInfo_PH3,
                            cFWParameterInfo_DFT_NUM
                        };
                    }
                }
            }

            foreach (AppCoreDefine.FWParameterInfo cFWParameterInfo in cFWParameterInfo_Array)
            {
                if (cFWParameterInfo.m_sParameterName == "DFT_NUM")
                {
                    if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                    {
                        if (cFWParameterInfo.m_nReadValue != cFWParameterInfo.m_nSetValue)
                        {
                            if (bLastRetry == true)
                            {
                                m_sErrorMessage = string.Format("Set {0} Not Match[Set:0x{1}, Read:0x{2}]",
                                                                cFWParameterInfo.m_sParameterName,
                                                                cFWParameterInfo.m_nSetValue.ToString("x2").ToUpper(),
                                                                cFWParameterInfo.m_nReadValue.ToString("x2").ToUpper());
                            }

                            return false;
                        }
                    }
                    else
                    {
                        if (cFWParameterInfo.m_nReadValue != m_cOriginParameter.m_n_MS_DFT_NUM)
                        {
                            if (bLastRetry == true)
                            {
                                m_sErrorMessage = string.Format("Read {0} Not Match[Orignal:0x{1}, Read:0x{2}]",
                                                                cFWParameterInfo.m_sParameterName,
                                                                m_cOriginParameter.m_n_MS_DFT_NUM.ToString("x2").ToUpper(),
                                                                cFWParameterInfo.m_nReadValue.ToString("x2").ToUpper());
                            }

                            return false;
                        }
                    }
                }
                else
                {
                    if (cFWParameterInfo.m_nReadValue != cFWParameterInfo.m_nSetValue)
                    {
                        if (bLastRetry == true)
                        {
                            m_sErrorMessage = string.Format("Set {0} Not Match[Set:0x{1}, Read:0x{2}]",
                                                            cFWParameterInfo.m_sParameterName,
                                                            cFWParameterInfo.m_nSetValue.ToString("x2").ToUpper(),
                                                            cFWParameterInfo.m_nReadValue.ToString("x2").ToUpper());
                        }

                        return false;
                    }
                }
            }

            return true;
        }
#else
        /// <summary>
        /// 檢查類比參數是否正確設定,建立參數資訊陣列並進行驗證
        /// </summary>
        /// <param name="cFlowStep">當前執行的流程步驟</param>
        /// <param name="bLastRetry">是否為最後一次重試旗標,用於驗證時的錯誤處理</param>
        /// <param name="cFrequencyItem">頻率項目設定資訊,預設為null</param>
        /// <param name="cRawADCSweepItem">Raw ADC掃描項目設定資訊,預設為null</param>
        /// <returns>所有參數驗證通過回傳true;驗證失敗則回傳false</returns>
        private bool CheckAnalogParameter(
            frmMain.FlowStep cFlowStep, 
            bool bLastRetry, 
            FrequencyItem cFrequencyItem = null, 
            RawADCSweepItem cRawADCSweepItem = null)
        {
            AppCoreDefine.FWParameterInfo[] cFWParameterInfo_Array = BuildParameterInfoArray(cFlowStep, cFrequencyItem, cRawADCSweepItem);

            return ValidateParameters(cFWParameterInfo_Array, bLastRetry);
        }

        #region 建立參數資訊陣列

        /// <summary>
        /// 根據流程步驟建立參數資訊陣列
        /// </summary>
        /// <param name="cFlowStep">當前流程步驟</param>
        /// <param name="cFrequencyItem">頻率項目參數（用於頻率掃描）</param>
        /// <param name="cRawADCSweepItem">Raw ADC Sweep 項目參數</param>
        /// <returns>韌體參數資訊陣列</returns>
        private AppCoreDefine.FWParameterInfo[] BuildParameterInfoArray(
            frmMain.FlowStep cFlowStep,
            FrequencyItem cFrequencyItem,
            RawADCSweepItem cRawADCSweepItem)
        {
            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || 
                cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                return BuildSelfParameterArray(cFrequencyItem);
            }
            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                return BuildRawADCSweepParameterArray(cRawADCSweepItem);
            }
            else
            {
                return BuildFrequencyParameterArray(cFrequencyItem);
            }
        }

        #endregion

        #region Self 參數陣列建立

        /// <summary>
        /// 建立 Self 模式完整參數陣列
        /// </summary>
        /// <param name="cFrequencyItem">頻率項目參數物件，包含 Self 模式的相位與延遲時間設定</param>
        /// <returns>包含 24 個 Self 參數資訊的完整陣列，用於參數驗證</returns>
        private AppCoreDefine.FWParameterInfo[] BuildSelfParameterArray(FrequencyItem cFrequencyItem)
        {
            // 基本參數
            var basicParams = new[]
            {
                new AppCoreDefine.FWParameterInfo("_SELF_PH1", cFrequencyItem.m_n_SELF_PH1, m_cReadParameter.m_n_SELF_PH1),
                new AppCoreDefine.FWParameterInfo("_SELF_PH2E_LAT", cFrequencyItem.m_n_SELF_PH2E_LAT, m_cReadParameter.m_n_SELF_PH2E_LAT),
                new AppCoreDefine.FWParameterInfo("_SELF_PH2E_LMT", cFrequencyItem.m_n_SELF_PH2E_LMT, m_cReadParameter.m_n_SELF_PH2E_LMT),
                new AppCoreDefine.FWParameterInfo("_SELF_PH2_LAT", cFrequencyItem.m_n_SELF_PH2_LAT, m_cReadParameter.m_n_SELF_PH2_LAT),
                new AppCoreDefine.FWParameterInfo("_SELF_PH2", cFrequencyItem.m_n_SELF_PH2, m_cReadParameter.m_n_SELF_PH2),
                new AppCoreDefine.FWParameterInfo("Self_DFT_NUM", m_cSelfParameter.m_nDFT_NUM, m_cReadParameter.m_nSelf_DFT_NUM)
            };

            // 條件參數 (Gain, CAG, IQ_BSH)
            var conditionalParams = BuildSelfConditionalParameters();

            // 計算參數
            var calculatedParams = BuildSelfCalculatedParameters();

            // 合併所有參數
            return CombineParameterArrays(basicParams, conditionalParams, calculatedParams);
        }

        /// <summary>
        /// 建立 Self 模式的條件參數陣列
        /// </summary>
        /// <returns>包含三個條件參數的陣列，每個參數根據條件使用新值或原始值</returns>
        private AppCoreDefine.FWParameterInfo[] BuildSelfConditionalParameters()
        {
            return new[]
            {
                // Self_Gain
                (m_cSelfParameter.m_nGain >= 0)
                    ? new AppCoreDefine.FWParameterInfo("Self_Gain", m_cSelfParameter.m_nGain, m_cReadParameter.m_nSelf_Gain)
                    : new AppCoreDefine.FWParameterInfo("Self_Gain", m_cOriginParameter.m_nSelf_Gain, m_cReadParameter.m_nSelf_Gain),

                // Self_CAG
                (m_cSelfParameter.m_nCAG >= 0)
                    ? new AppCoreDefine.FWParameterInfo("Self_CAG", m_cSelfParameter.m_nCAG, m_cReadParameter.m_nSelf_CAG)
                    : new AppCoreDefine.FWParameterInfo("Self_CAG", m_cOriginParameter.m_nSelf_CAG, m_cReadParameter.m_nSelf_CAG),

                // Self_IQ_BSH
                (m_cSelfParameter.m_nIQ_BSH >= 0)
                    ? new AppCoreDefine.FWParameterInfo("Self_IQ_BSH", m_cSelfParameter.m_nIQ_BSH, m_cReadParameter.m_nSelf_IQ_BSH)
                    : new AppCoreDefine.FWParameterInfo("Self_IQ_BSH", m_cOriginParameter.m_nSelf_IQ_BSH, m_cReadParameter.m_nSelf_IQ_BSH)
            };
        }

        /// <summary>
        /// 建立 Self 模式的計算參數陣列
        /// </summary>
        /// <returns>包含 15 個計算參數的陣列，用於驗證 Self 模式的暫存器設定</returns>
        private AppCoreDefine.FWParameterInfo[] BuildSelfCalculatedParameters()
        {
            // 計算數值
            int n_SELF_SP_NUM = (m_cOriginParameter.m_n_SELF_SP_NUM - m_cOriginParameter.m_nSelf_DFT_NUM) + m_cSelfParameter.m_nDFT_NUM;
            int n_SELF_EFFECT_NUM = (m_cOriginParameter.m_n_SELF_EFFECT_NUM - m_cOriginParameter.m_nSelf_DFT_NUM) + m_cSelfParameter.m_nDFT_NUM;
            int n_SELF_PKT_WC_L = n_SELF_EFFECT_NUM * m_nRXTraceNumber;

            int n_SELF_BSH_ADC_TP_NUM_H = m_cSelfParameter.m_nDFT_NUM << 4;
            int n_SELF_BSH_ADC_TP_NUM_L = m_nRXTraceNumber;

            int n_SELF_EFFECT_FW_SET_COEF_NUM_H = m_cSelfParameter.m_nDFT_NUM;
            int n_SELF_EFFECT_FW_SET_COEF_NUM_L = 0;

            int n_SELF_DFT_NUM_IQ_FIR_CTL_H = m_cSelfParameter.m_nDFT_NUM;
            int n_SELF_DFT_NUM_IQ_FIR_CTL_L = 0x1000;

            // ANA_TP_CTL_01
            int n_SELF_ANA_TP_CTL_01_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H;
            int n_SELF_ANA_TP_CTL_01_L = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L;
            if (m_cSelfParameter.m_nGain >= 0)
            {
                n_SELF_ANA_TP_CTL_01_L = (m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L & 0xFF87) | (m_cSelfParameter.m_nGain << 3);
            }

            // ANA_TP_CTL_00
            int n_SELF_ANA_TP_CTL_00_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H;
            int n_SELF_ANA_TP_CTL_00_L = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L;
            if (m_cSelfParameter.m_nCAG >= 0)
            {
                n_SELF_ANA_TP_CTL_00_L = (m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L & 0xFFC7) | (m_cSelfParameter.m_nCAG << 3);
            }

            // IQ_BSH_GP0_GP1
            int n_SELF_IQ_BSH_GP0_GP1_H = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_H;
            int n_SELF_IQ_BSH_GP0_GP1_L = m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_L;
            if (m_cSelfParameter.m_nIQ_BSH >= 0)
            {
                n_SELF_IQ_BSH_GP0_GP1_L = (m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_L & 0xF03F) | (m_cSelfParameter.m_nIQ_BSH << 6);
            }

            return new[]
            {
                new AppCoreDefine.FWParameterInfo("_SELF_SP_NUM", n_SELF_SP_NUM, m_cReadParameter.m_n_SELF_SP_NUM),
                new AppCoreDefine.FWParameterInfo("_SELF_EFFECT_NUM", n_SELF_EFFECT_NUM, m_cReadParameter.m_n_SELF_EFFECT_NUM),
                new AppCoreDefine.FWParameterInfo("_SELF_PKT_WC_L", n_SELF_PKT_WC_L, m_cReadParameter.m_n_SELF_PKT_WC_L),
                new AppCoreDefine.FWParameterInfo("_SELF_BSH_ADC_TP_NUM_H", n_SELF_BSH_ADC_TP_NUM_H, m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_H),
                new AppCoreDefine.FWParameterInfo("_SELF_BSH_ADC_TP_NUM_L", n_SELF_BSH_ADC_TP_NUM_L, m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_L),
                new AppCoreDefine.FWParameterInfo("_SELF_EFFECT_FW_SET_COEF_NUM_H", n_SELF_EFFECT_FW_SET_COEF_NUM_H, m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H),
                new AppCoreDefine.FWParameterInfo("_SELF_EFFECT_FW_SET_COEF_NUM_L", n_SELF_EFFECT_FW_SET_COEF_NUM_L, m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L),
                new AppCoreDefine.FWParameterInfo("_SELF_DFT_NUM_IQ_FIR_CTL_H", n_SELF_DFT_NUM_IQ_FIR_CTL_H, m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H),
                new AppCoreDefine.FWParameterInfo("_SELF_DFT_NUM_IQ_FIR_CTL_L", n_SELF_DFT_NUM_IQ_FIR_CTL_L, m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L),
                new AppCoreDefine.FWParameterInfo("_SELF_ANA_TP_CTL_01_H", n_SELF_ANA_TP_CTL_01_H, m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_H),
                new AppCoreDefine.FWParameterInfo("_SELF_ANA_TP_CTL_01_L", n_SELF_ANA_TP_CTL_01_L, m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_L),
                new AppCoreDefine.FWParameterInfo("_SELF_ANA_TP_CTL_00_H", n_SELF_ANA_TP_CTL_00_H, m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_H),
                new AppCoreDefine.FWParameterInfo("_SELF_ANA_TP_CTL_00_L", n_SELF_ANA_TP_CTL_00_L, m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_L),
                new AppCoreDefine.FWParameterInfo("_SELF_IQ_BSH_GP0_GP1_H", n_SELF_IQ_BSH_GP0_GP1_H, m_cReadParameter.m_n_SELF_IQ_BSH_GP0_GP1_H),
                new AppCoreDefine.FWParameterInfo("_SELF_IQ_BSH_GP0_GP1_L", n_SELF_IQ_BSH_GP0_GP1_L, m_cReadParameter.m_n_SELF_IQ_BSH_GP0_GP1_L)
            };
        }

        #endregion

        #region Raw ADC 參數陣列建立

        /// <summary>
        /// 建立 Raw ADC Sweep 參數陣列（根據 IC 世代路由）
        /// </summary>
        /// <param name="cRawADCSweepItem">Raw ADC Sweep 項目參數物件，包含掃描設定值</param>
        /// <returns>根據 IC 世代建立的參數陣列，不支援的世代返回空陣列</returns>
        private AppCoreDefine.FWParameterInfo[] BuildRawADCSweepParameterArray(RawADCSweepItem cRawADCSweepItem)
        {
            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                return BuildRawADCSweepParameterArray_Gen8(cRawADCSweepItem);
            }
            else if (m_eICGenerationType == ICGenerationType.Gen7)
            {
                return BuildRawADCSweepParameterArray_Gen7(cRawADCSweepItem);
            }
            else if (m_eICGenerationType == ICGenerationType.Gen6)
            {
                return BuildRawADCSweepParameterArray_Gen6(cRawADCSweepItem);
            }

            return new AppCoreDefine.FWParameterInfo[0];
        }

        /// <summary>
        /// 建立 Gen8 的 Raw ADC Sweep 參數陣列
        /// </summary>
        /// <param name="cRawADCSweepItem">Raw ADC Sweep 項目參數物件，包含各項掃描設定值</param>
        /// <returns>Gen8 參數資訊陣列（4-5 個參數，視 IC 方案而定）</returns>
        private AppCoreDefine.FWParameterInfo[] BuildRawADCSweepParameterArray_Gen8(RawADCSweepItem cRawADCSweepItem)
        {
            var baseParams = new[]
            {
                new AppCoreDefine.FWParameterInfo("c_MS_FIRCOEF_SEL", cRawADCSweepItem.m_nFIRCOEF_SEL, m_cReadParameter.m_n_MS_FIRCOEF_SEL),
                new AppCoreDefine.FWParameterInfo("c_MS_FIR_TAP_NUM", cRawADCSweepItem.m_nFIR_TAP_NUM, m_cReadParameter.m_n_MS_FIR_TAP_NUM),
                new AppCoreDefine.FWParameterInfo("c_MS_SELC", cRawADCSweepItem.m_nSELC, m_cReadParameter.m_n_MS_SELC),
                new AppCoreDefine.FWParameterInfo("c_MS_REG_VSEL", cRawADCSweepItem.m_nVSEL, m_cReadParameter.m_n_MS_VSEL)
            };

            if (m_eICSolutionType == ICSolutionType.Solution_8F18)
            {
                var lgParam = new AppCoreDefine.FWParameterInfo("c_MS_LG", cRawADCSweepItem.m_nLG, m_cReadParameter.m_n_MS_LG);

                /*
                // 保留原本註解: 如果需要固定 SELGM
                if (ParamFingerAutoTuning.m_nRawADCSFixedSELGM >= 0)
                {
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_SELGM = new AppCoreDefine.FWParameterInfo("c_MS_SELGM", cRawADCSweepItem.m_nSELGM, m_cReadParameter.m_n_MS_SELGM);

                    cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                    {
                        cFWParameterInfo_MS_FIRCOEF_SEL,
                        cFWParameterInfo_MS_FIR_TAP_NUM,
                        cFWParameterInfo_MS_SELGM,
                        cFWParameterInfo_MS_SELC,
                        cFWParameterInfo_MS_REG_VSEL,
                        cFWParameterInfo_MS_LG
                    };
                }
                else
                {
                    cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                    {
                        cFWParameterInfo_MS_FIRCOEF_SEL,
                        cFWParameterInfo_MS_FIR_TAP_NUM,
                        cFWParameterInfo_MS_SELC,
                        cFWParameterInfo_MS_REG_VSEL,
                        cFWParameterInfo_MS_LG
                    };
                }
                */

                return CombineParameterArrays(baseParams, new[] { lgParam });
            }
            else
            {
                /*
                // 保留原本註解: 如果需要固定 SELGM
                if (ParamFingerAutoTuning.m_nRawADCSFixedSELGM >= 0)
                {
                    AppCoreDefine.FWParameterInfo cFWParameterInfo_MS_SELGM = new AppCoreDefine.FWParameterInfo("c_MS_SELGM", cRawADCSweepItem.m_nSELGM, m_cReadParameter.m_n_MS_SELGM);

                    cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                    {
                        cFWParameterInfo_MS_FIRCOEF_SEL,
                        cFWParameterInfo_MS_FIR_TAP_NUM,
                        cFWParameterInfo_MS_SELGM,
                        cFWParameterInfo_MS_SELC,
                        cFWParameterInfo_MS_REG_VSEL
                    };
                }
                else
                {
                    cFWParameterInfo_Array = new AppCoreDefine.FWParameterInfo[]
                    {
                        cFWParameterInfo_MS_FIRCOEF_SEL,
                        cFWParameterInfo_MS_FIR_TAP_NUM,
                        cFWParameterInfo_MS_SELC,
                        cFWParameterInfo_MS_REG_VSEL
                    };
                }
                */

                return baseParams;
            }
        }

        /// <summary>
        /// 建立 Gen7 的 Raw ADC Sweep 參數陣列
        /// </summary>
        /// <param name="cRawADCSweepItem">Raw ADC Sweep 項目參數物件，包含 Gen7 掃描設定值</param>
        /// <returns>Gen7 參數資訊陣列（固定 6 個參數）</returns>
        private AppCoreDefine.FWParameterInfo[] BuildRawADCSweepParameterArray_Gen7(RawADCSweepItem cRawADCSweepItem)
        {
            return new[]
            {
                new AppCoreDefine.FWParameterInfo("_MS_FIRTB", cRawADCSweepItem.m_nFIRTB, m_cReadParameter.m_n_MS_FIRTB),
                new AppCoreDefine.FWParameterInfo("_MS_FIR_TAP_NUM", cRawADCSweepItem.m_nFIR_TAP_NUM, m_cReadParameter.m_n_MS_FIR_TAP_NUM),
                new AppCoreDefine.FWParameterInfo("_SELC", cRawADCSweepItem.m_nSELC, m_cReadParameter.m_n_MS_SELC),
                new AppCoreDefine.FWParameterInfo("_VF_VSEL", cRawADCSweepItem.m_nVSEL, m_cReadParameter.m_n_MS_VSEL),
                new AppCoreDefine.FWParameterInfo("_LG", cRawADCSweepItem.m_nLG, m_cReadParameter.m_n_MS_LG),
                new AppCoreDefine.FWParameterInfo("_SELGM", cRawADCSweepItem.m_nSELGM, m_cReadParameter.m_n_MS_SELGM)
            };
        }

        /// <summary>
        /// 建立 Gen6 的 Raw ADC Sweep 參數陣列
        /// </summary>
        /// <param name="cRawADCSweepItem">Raw ADC Sweep 項目參數物件，包含 Gen6 掃描設定值</param>
        /// <returns>Gen6 參數資訊陣列（固定 6 個參數）</returns>
        private AppCoreDefine.FWParameterInfo[] BuildRawADCSweepParameterArray_Gen6(RawADCSweepItem cRawADCSweepItem)
        {
            return new[]
            {
                new AppCoreDefine.FWParameterInfo("_MS_FIRTB", cRawADCSweepItem.m_nFIRTB, m_cReadParameter.m_n_MS_FIRTB),
                new AppCoreDefine.FWParameterInfo("_MS_FIR_TAP_NUM", cRawADCSweepItem.m_nFIR_TAP_NUM, m_cReadParameter.m_n_MS_FIR_TAP_NUM),
                new AppCoreDefine.FWParameterInfo("_SELC", cRawADCSweepItem.m_nSELC, m_cReadParameter.m_n_MS_SELC),
                new AppCoreDefine.FWParameterInfo("_VF_VSEL", cRawADCSweepItem.m_nVSEL, m_cReadParameter.m_n_MS_VSEL),
                new AppCoreDefine.FWParameterInfo("_LG", cRawADCSweepItem.m_nLG, m_cReadParameter.m_n_MS_LG),
                new AppCoreDefine.FWParameterInfo("_SELGM", cRawADCSweepItem.m_nSELGM, m_cReadParameter.m_n_MS_SELGM)
            };
        }

        #endregion

        #region 頻率參數陣列建立

        /// <summary>
        /// 建立頻率掃描參數陣列
        /// </summary>
        /// <param name="cFrequencyItem">頻率項目參數物件，包含相位與 DFT 設定值</param>
        /// <returns>頻率參數資訊陣列（Gen8: 7 個參數，其他世代: 4 個參數）</returns>
        private AppCoreDefine.FWParameterInfo[] BuildFrequencyParameterArray(FrequencyItem cFrequencyItem)
        {
            var baseParams = new[]
            {
                new AppCoreDefine.FWParameterInfo("PH1", cFrequencyItem.m_nPH1, m_cReadParameter.m_nPH1),
                new AppCoreDefine.FWParameterInfo("PH2", cFrequencyItem.m_nPH2, m_cReadParameter.m_nPH2),
                new AppCoreDefine.FWParameterInfo("PH3", cFrequencyItem.m_nPH2, m_cReadParameter.m_nPH3),
                new AppCoreDefine.FWParameterInfo("DFT_NUM", cFrequencyItem.m_nDFT_NUM, m_cReadParameter.m_n_MS_DFT_NUM)
            };

            if (m_eICGenerationType == ICGenerationType.Gen9)
            {
                return baseParams;
            }
            else if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                return BuildFrequencyParameterArray_Gen8(baseParams, cFrequencyItem);
            }
            else
            {
                return baseParams;
            }
        }

        /// <summary>
        /// 建立 Gen8 的額外頻率參數陣列
        /// </summary>
        /// <param name="baseParams">基本參數陣列（PH1, PH2, PH3, DFT_NUM），共 4 個參數</param>
        /// <param name="cFrequencyItem">頻率項目參數物件，包含新的 DFT_NUM 設定值</param>
        /// <returns>合併基本參數與額外參數的完整陣列（共 7 個參數）</returns>
        private AppCoreDefine.FWParameterInfo[] BuildFrequencyParameterArray_Gen8(
            AppCoreDefine.FWParameterInfo[] baseParams,
            FrequencyItem cFrequencyItem)
        {
            int n_MS_SP_NUM = (m_cOriginParameter.m_n_MS_SP_NUM - m_cOriginParameter.m_n_MS_DFT_NUM) + cFrequencyItem.m_nDFT_NUM;
            int n_MS_EFFECT_NUM = (m_cOriginParameter.m_n_MS_EFFECT_NUM - m_cOriginParameter.m_n_MS_DFT_NUM) + cFrequencyItem.m_nDFT_NUM;
            int nPKT_WC = n_MS_EFFECT_NUM * m_nRXTraceNumber;

            var additionalParams = new[]
            {
                new AppCoreDefine.FWParameterInfo("_MS_SP_NUM", n_MS_SP_NUM, m_cReadParameter.m_n_MS_SP_NUM),
                new AppCoreDefine.FWParameterInfo("_MS_EFFECT_NUM", n_MS_EFFECT_NUM, m_cReadParameter.m_n_MS_EFFECT_NUM),
                new AppCoreDefine.FWParameterInfo("PKT_WC", nPKT_WC, m_cReadParameter.m_nPKT_WC)
            };

            return CombineParameterArrays(baseParams, additionalParams);
        }

        #endregion

        #region 參數驗證

        //// <summary>
        /// 驗證參數陣列中的所有參數
        /// </summary>
        /// <param name="cFWParameterInfo_Array">韌體參數資訊陣列，每個元素包含參數名稱、設定值和讀取值</param>
        /// <param name="bLastRetry">是否為最後一次重試（true: 驗證失敗時設定 m_sErrorMessage; false: 靜默失敗不設定錯誤訊息）</param>
        /// <returns>true: 所有參數驗證通過; false: 任一參數驗證失敗</returns>
        private bool ValidateParameters(AppCoreDefine.FWParameterInfo[] cFWParameterInfo_Array, bool bLastRetry)
        {
            foreach (AppCoreDefine.FWParameterInfo cFWParameterInfo in cFWParameterInfo_Array)
            {
                if (!ValidateSingleParameter(cFWParameterInfo, bLastRetry))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 驗證單一參數
        /// </summary>
        /// <param name="cFWParameterInfo">韌體參數資訊物件，包含參數名稱、設定值和讀取值</param>
        /// <param name="bLastRetry">是否為最後一次重試（決定是否設定錯誤訊息）</param>
        /// <returns>true: 參數驗證通過; false: 參數驗證失敗</returns>
        private bool ValidateSingleParameter(AppCoreDefine.FWParameterInfo cFWParameterInfo, bool bLastRetry)
        {
            if (cFWParameterInfo.m_sParameterName == "DFT_NUM")
            {
                return ValidateDFTNUMParameter(cFWParameterInfo, bLastRetry);
            }
            else
            {
                return ValidateGeneralParameter(cFWParameterInfo, bLastRetry);
            }
        }

        /// <summary>
        /// 驗證 DFT_NUM 參數
        /// </summary>
        /// <param name="cFWParameterInfo">DFT_NUM 參數資訊物件，包含設定值和讀取值</param>
        /// <param name="bLastRetry">是否為最後一次重試（true: 設定錯誤訊息; false: 靜默失敗）</param>
        /// <returns>true: DFT_NUM 驗證通過; false: 驗證失敗</returns>
        private bool ValidateDFTNUMParameter(AppCoreDefine.FWParameterInfo cFWParameterInfo, bool bLastRetry)
        {
            if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
            {
                // 檢查設定值是否匹配
                if (cFWParameterInfo.m_nReadValue != cFWParameterInfo.m_nSetValue)
                {
                    if (bLastRetry)
                    {
                        m_sErrorMessage = string.Format("Set {0} Not Match[Set:0x{1}, Read:0x{2}]", cFWParameterInfo.m_sParameterName, cFWParameterInfo.m_nSetValue.ToString("X2"), cFWParameterInfo.m_nReadValue.ToString("X2"));
                    }

                    return false;
                }
            }
            else
            {
                // 檢查是否與原始值匹配
                if (cFWParameterInfo.m_nReadValue != m_cOriginParameter.m_n_MS_DFT_NUM)
                {
                    if (bLastRetry)
                    {
                        m_sErrorMessage = string.Format("Read {0} Not Match[Orignal:0x{1}, Read:0x{2}]", cFWParameterInfo.m_sParameterName, m_cOriginParameter.m_n_MS_DFT_NUM.ToString("X2"), cFWParameterInfo.m_nReadValue.ToString("X2"));
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 驗證一般參數
        /// </summary>
        /// <param name="cFWParameterInfo">韌體參數資訊物件，包含參數名稱、設定值和讀取值</param>
        /// <param name="bLastRetry">是否為最後一次重試（true: 設定錯誤訊息供診斷; false: 靜默失敗以便重試）</param>
        /// <returns>true: 參數驗證通過（讀取值 = 設定值）; false: 驗證失敗（讀取值 ≠ 設定值）</returns>
        private bool ValidateGeneralParameter(AppCoreDefine.FWParameterInfo cFWParameterInfo, bool bLastRetry)
        {
            if (cFWParameterInfo.m_nReadValue != cFWParameterInfo.m_nSetValue)
            {
                if (bLastRetry)
                {
                    m_sErrorMessage = string.Format("Set {0} Not Match[Set:0x{1}, Read:0x{2}]", cFWParameterInfo.m_sParameterName, cFWParameterInfo.m_nSetValue.ToString("X2"), cFWParameterInfo.m_nReadValue.ToString("X2"));
                }

                return false;
            }

            return true;
        }

        #endregion

        #region 輔助方法

        /// <summary>
        /// 合併多個參數陣列為單一陣列
        /// </summary>
        /// <param name="arrays">可變數量的參數陣列，將按順序合併</param>
        /// <returns>合併後的單一參數陣列，包含所有輸入陣列的元素</returns>
        private AppCoreDefine.FWParameterInfo[] CombineParameterArrays(params AppCoreDefine.FWParameterInfo[][] arrays)
        {
            int totalLength = 0;

            foreach (var array in arrays)
            {
                totalLength += array.Length;
            }

            var result = new AppCoreDefine.FWParameterInfo[totalLength];
            int offset = 0;

            foreach (var array in arrays)
            {
                Array.Copy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;
        }

        #endregion
#endif
    }
}
