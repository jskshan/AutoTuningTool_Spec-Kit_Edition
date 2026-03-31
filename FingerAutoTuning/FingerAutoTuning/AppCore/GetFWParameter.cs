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
        private bool GetFWParameter(frmMain.FlowStep cFlowStep, bool bGetOrigin = false, bool bNotDisableScanMode_9F07 = false)
        {
            if (ParamFingerAutoTuning.m_nDisableSetAnalogParameter == 1)
                return true;

            if (bGetOrigin == true)
            {
                //Get Origin Analog Parameter
                OutputMessage("-Get Original Analog Parameter");
            }

            int nSendCommandDelayTime_Gen8 = ParamFingerAutoTuning.m_nGen8SendCommandDelayTime;
            int nRetryCount = 3;

            string sCommonMessage = "Get";
            
            if (bGetOrigin == true)
                sCommonMessage = "Get Origin";

            if (m_eICGenerationType != ICGenerationType.Gen9)
            {
                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_WINDOWS && ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_USB)
                {
                    if (m_eICGenerationType == ICGenerationType.Gen8)
                    {
                        for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
                        {
                            bool bReadDataError = false;

                            if (nRetryIndex > 0)
                            {
                                SetTestModeEnable(false);
                                SetTestModeEnable(true);
                            }

                            if (bGetOrigin == true)
                                SetTestModeEnable(true);

                            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                                cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                            {
                                ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eSelfTraceType);

                                Thread.Sleep(nSendCommandDelayTime_Gen8);

                                ReadDataInfo_Gen8 cReadDataInfo_SP_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_SP_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_EFFECT_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_EFFECT_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_DFT_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_DFT_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH1 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH1, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH2E_LAT = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH2E_LAT, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH2_LAT = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH2_LAT, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH2 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH2, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_PKT_WC_L = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PKT_WC_L, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_BSH_ADC_TP_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_BSH_ADC_TP_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_EFFECT_FW_SET_COEF_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_EFFECT_FW_SET_COEF_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_DFT_NUM_IQ_FIR_CTL = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_DFT_NUM_IQ_FIR_CTL, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_ANA_TP_CTL_01 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_01, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_ANA_TP_CTL_00 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_00, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_IQ_BSH_GP0_GP1 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_IQ_BSH_GP0_GP1, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);

                                List<ReadDataInfo_Gen8> cReadDataInfo_List = new List<ReadDataInfo_Gen8>()
                                {
                                    cReadDataInfo_SP_NUM,
                                    cReadDataInfo_EFFECT_NUM,
                                    cReadDataInfo_DFT_NUM,
                                    cReadDataInfo_PH1,
                                    cReadDataInfo_PH2E_LAT,
                                    cReadDataInfo_PH2_LAT,
                                    cReadDataInfo_PH2,
                                    cReadDataInfo_PKT_WC_L,
                                    cReadDataInfo_BSH_ADC_TP_NUM,
                                    cReadDataInfo_EFFECT_FW_SET_COEF_NUM,
                                    cReadDataInfo_DFT_NUM_IQ_FIR_CTL,
                                    cReadDataInfo_ANA_TP_CTL_01,
                                    cReadDataInfo_ANA_TP_CTL_00,
                                    cReadDataInfo_IQ_BSH_GP0_GP1
                                };

                                /*
                                if (cFlowStep.m_eStep == MainStep.SELF_FREQUENCYSWEEP && 
                                    (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2)
                                    cReadDataInfo_List.Add(cReadDataInfo_ANA_TP_CTL_00);
                                */

                                foreach (ReadDataInfo_Gen8 cReadDataInfo in cReadDataInfo_List)
                                {
                                    if (GetReadData_Gen8(cReadDataInfo, cElanCommand_Gen8, sCommonMessage, nSendCommandDelayTime_Gen8) == false)
                                    {
                                        bReadDataError = true;

                                        if (nRetryIndex == nRetryCount)
                                        {
                                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                            return false;
                                        }
                                        else
                                            break;
                                    }
                                }

                                if (bReadDataError == true)
                                    continue;

                                if (bGetOrigin == true)
                                {
                                    m_cOriginParameter.m_n_SELF_SP_NUM = cReadDataInfo_SP_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_EFFECT_NUM = cReadDataInfo_EFFECT_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_nSelf_DFT_NUM = cReadDataInfo_DFT_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_PH1 = cReadDataInfo_PH1.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_PH2E_LMT = cReadDataInfo_PH2E_LAT.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_PH2E_LAT = cReadDataInfo_PH2E_LAT.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_PH2_LAT = cReadDataInfo_PH2_LAT.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_PH2_MUX_LAT = cReadDataInfo_PH2_LAT.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_PH2 = cReadDataInfo_PH2.m_nOutputValue2;
                                    m_cOriginParameter.m_nSelf_Gain = (cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue2 & 0x0078) >> 3;
                                    m_cOriginParameter.m_nSelf_CAG = (cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2 & 0x0038) >> 3;
                                    m_cOriginParameter.m_nSelf_IQ_BSH = (cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue2 & 0x0FC0) >> 6;
                                    m_cOriginParameter.m_n_SELF_PKT_WC_L = cReadDataInfo_PKT_WC_L.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_H = cReadDataInfo_BSH_ADC_TP_NUM.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_L = cReadDataInfo_BSH_ADC_TP_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H = cReadDataInfo_EFFECT_FW_SET_COEF_NUM.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L = cReadDataInfo_EFFECT_FW_SET_COEF_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H = cReadDataInfo_DFT_NUM_IQ_FIR_CTL.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L = cReadDataInfo_DFT_NUM_IQ_FIR_CTL.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H = cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L = cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_H = cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_L = cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue2;

                                    /*
                                    if (cFlowStep.m_eStep == MainStep.SELF_FREQUENCYSWEEP &&
                                        (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
                                    {
                                        m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue1;
                                        m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2;
                                    }
                                    */
                                }
                                else
                                {
                                    m_cReadParameter.m_n_SELF_SP_NUM = cReadDataInfo_SP_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_EFFECT_NUM = cReadDataInfo_EFFECT_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_nSelf_DFT_NUM = cReadDataInfo_DFT_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_PH1 = cReadDataInfo_PH1.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_PH2E_LMT = cReadDataInfo_PH2E_LAT.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_PH2E_LAT = cReadDataInfo_PH2E_LAT.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_PH2_LAT = cReadDataInfo_PH2_LAT.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_PH2_MUX_LAT = cReadDataInfo_PH2_LAT.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_PH2 = cReadDataInfo_PH2.m_nOutputValue2;
                                    m_cReadParameter.m_nSelf_Gain = (cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue2 & 0x0078) >> 3;
                                    m_cReadParameter.m_nSelf_CAG = (cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2 & 0x0038) >> 3;
                                    m_cReadParameter.m_nSelf_IQ_BSH = (cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue2 & 0x0FC0) >> 6;
                                    m_cReadParameter.m_n_SELF_PKT_WC_L = cReadDataInfo_PKT_WC_L.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_H = cReadDataInfo_BSH_ADC_TP_NUM.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_L = cReadDataInfo_BSH_ADC_TP_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H = cReadDataInfo_EFFECT_FW_SET_COEF_NUM.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L = cReadDataInfo_EFFECT_FW_SET_COEF_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H = cReadDataInfo_DFT_NUM_IQ_FIR_CTL.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L = cReadDataInfo_DFT_NUM_IQ_FIR_CTL.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_H = cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_L = cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_H = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_L = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_IQ_BSH_GP0_GP1_H = cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_IQ_BSH_GP0_GP1_L = cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue2;

                                    /*
                                    if (cFlowStep.m_eStep == MainStep.SELF_FREQUENCYSWEEP && 
                                        (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
                                    {
                                        m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_H = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue1;
                                        m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_L = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2;
                                    }
                                    */
                                }
                            }
                            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                            {
                                ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);

                                Thread.Sleep(nSendCommandDelayTime_Gen8);

                                ReadDataInfo_Gen8 cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_BIN_FIRCOEF_SEL_TAP_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_MS_IQ_BSH_GP0_GP1 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_IQ_BSH_GP0_GP1, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_MS_AFE_DFT_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_DFT_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_TP_CTL_01 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_CTL_04 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_TP_CTL_01_2 = null;
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_CTL_04_2 = null;
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_TP_CTL_06 = null;
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_TP_CTL_06_2 = null;
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_TP_CTL_07 = null;

                                List<ReadDataInfo_Gen8> cReadDataInfo_List = new List<ReadDataInfo_Gen8>()
                                {
                                    cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM,
                                    cReadDataInfo_MS_IQ_BSH_GP0_GP1,
                                    cReadDataInfo_MS_AFE_DFT_NUM,
                                    cReadDataInfo_MS_ANA_TP_CTL_01,
                                    cReadDataInfo_MS_ANA_CTL_04,
                                };

                                if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                                {
                                    cReadDataInfo_MS_ANA_TP_CTL_01_2 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01_2, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                    cReadDataInfo_MS_ANA_CTL_04_2 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04_2, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                    cReadDataInfo_MS_ANA_TP_CTL_06 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                    cReadDataInfo_MS_ANA_TP_CTL_06_2 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06_2, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                    cReadDataInfo_MS_ANA_TP_CTL_07 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_07, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);

                                    cReadDataInfo_List.Add(cReadDataInfo_MS_ANA_TP_CTL_01_2);
                                    cReadDataInfo_List.Add(cReadDataInfo_MS_ANA_CTL_04_2);
                                    cReadDataInfo_List.Add(cReadDataInfo_MS_ANA_TP_CTL_06);
                                    cReadDataInfo_List.Add(cReadDataInfo_MS_ANA_TP_CTL_06_2);
                                    cReadDataInfo_List.Add(cReadDataInfo_MS_ANA_TP_CTL_07);
                                }

                                foreach (ReadDataInfo_Gen8 cReadDataInfo in cReadDataInfo_List)
                                {
                                    if (GetReadData_Gen8(cReadDataInfo, cElanCommand_Gen8, sCommonMessage, nSendCommandDelayTime_Gen8) == false)
                                    {
                                        bReadDataError = true;

                                        if (nRetryIndex == nRetryCount)
                                        {
                                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                            return false;
                                        }
                                        else
                                            break;
                                    }
                                }

                                if (bReadDataError == true)
                                    continue;

                                if (bGetOrigin == true)
                                {
                                    m_cOriginParameter.m_n_MS_FIRCOEF_SEL = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue2 & 0x000F;
                                    m_cOriginParameter.m_n_MS_FIR_TAP_NUM = (cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue1 & 0x07F0) >> 4;
                                    m_cOriginParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2 & 0x0078) >> 3;
                                    m_cOriginParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0_GP1.m_nOutputValue2 & 0x003F;
                                    m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_AFE_DFT_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2 & 0x0180) >> 7;
                                    m_cOriginParameter.m_n_MS_VSEL = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue2 & 0x0003;

                                    m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue1;
                                    m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_H = cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue1;
                                    m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_L = cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_04_H = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue1;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_04_L = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue2;

                                    if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                                    {
                                        m_cOriginParameter.m_n_MS_LG = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue2 & 0x0003;

                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_H = cReadDataInfo_MS_ANA_TP_CTL_01_2.m_nOutputValue1;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_L = cReadDataInfo_MS_ANA_TP_CTL_01_2.m_nOutputValue2;
                                        m_cOriginParameter.m_n_MS_ANA_CTL_04_2_H = cReadDataInfo_MS_ANA_CTL_04_2.m_nOutputValue1;
                                        m_cOriginParameter.m_n_MS_ANA_CTL_04_2_L = cReadDataInfo_MS_ANA_CTL_04_2.m_nOutputValue2;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_H = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue1;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_L = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue2;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_H = cReadDataInfo_MS_ANA_TP_CTL_06_2.m_nOutputValue1;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_L = cReadDataInfo_MS_ANA_TP_CTL_06_2.m_nOutputValue2;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_H = cReadDataInfo_MS_ANA_TP_CTL_07.m_nOutputValue1;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_L = cReadDataInfo_MS_ANA_TP_CTL_07.m_nOutputValue2;
                                    }
                                }
                                else
                                {
                                    m_cReadParameter.m_n_MS_FIRCOEF_SEL = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue2 & 0x000F;
                                    m_cReadParameter.m_n_MS_FIR_TAP_NUM = (cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue1 & 0x07F0) >> 4;
                                    m_cReadParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2 & 0x0078) >> 3;
                                    m_cReadParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0_GP1.m_nOutputValue2 & 0x003F;
                                    m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_AFE_DFT_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2 & 0x0180) >> 7;
                                    m_cReadParameter.m_n_MS_VSEL = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue2 & 0x0003;

                                    m_cReadParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue1;
                                    m_cReadParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_MS_ANA_TP_CTL_01_H = cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue1;
                                    m_cReadParameter.m_n_MS_ANA_TP_CTL_01_L = cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2;
                                    m_cReadParameter.m_n_MS_ANA_CTL_04_H = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue1;
                                    m_cReadParameter.m_n_MS_ANA_CTL_04_L = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue2;

                                    if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                                    {
                                        m_cReadParameter.m_n_MS_LG = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue2 & 0x0003;

                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_01_2_H = cReadDataInfo_MS_ANA_TP_CTL_01_2.m_nOutputValue1;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_01_2_L = cReadDataInfo_MS_ANA_TP_CTL_01_2.m_nOutputValue2;
                                        m_cReadParameter.m_n_MS_ANA_CTL_04_2_H = cReadDataInfo_MS_ANA_CTL_04_2.m_nOutputValue1;
                                        m_cReadParameter.m_n_MS_ANA_CTL_04_2_L = cReadDataInfo_MS_ANA_CTL_04_2.m_nOutputValue2;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_06_H = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue1;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_06_L = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue2;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_06_2_H = cReadDataInfo_MS_ANA_TP_CTL_06_2.m_nOutputValue1;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_06_2_L = cReadDataInfo_MS_ANA_TP_CTL_06_2.m_nOutputValue2;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_07_H = cReadDataInfo_MS_ANA_TP_CTL_07.m_nOutputValue1;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_07_L = cReadDataInfo_MS_ANA_TP_CTL_07.m_nOutputValue2;
                                    }
                                }
                            }
                            else
                            {
                                ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);

                                Thread.Sleep(nSendCommandDelayTime_Gen8);

                                ReadDataInfo_Gen8 cReadDataInfo_PH1 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_PH1, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH2 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_PH2, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH3 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_PH3, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_Sum = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_DFT_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_SP_NUM = null;
                                ReadDataInfo_Gen8 cReadDataInfo_EFFECT_NUM = null;
                                ReadDataInfo_Gen8 cReadDataInfo_PKT_WC = null;

                                List<ReadDataInfo_Gen8> cReadDataInfo_List = new List<ReadDataInfo_Gen8>()
                                {
                                    cReadDataInfo_PH1,
                                    cReadDataInfo_PH2,
                                    cReadDataInfo_PH3,
                                    cReadDataInfo_Sum
                                };

                                if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                                {
                                    cReadDataInfo_SP_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_SP_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                    cReadDataInfo_EFFECT_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_EFFECT_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                    cReadDataInfo_PKT_WC = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType.PKT_WC, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 4);

                                    cReadDataInfo_List.Add(cReadDataInfo_SP_NUM);
                                    cReadDataInfo_List.Add(cReadDataInfo_EFFECT_NUM);
                                    cReadDataInfo_List.Add(cReadDataInfo_PKT_WC);
                                }

                                foreach (ReadDataInfo_Gen8 cReadDataInfo in cReadDataInfo_List)
                                {
                                    if (GetReadData_Gen8(cReadDataInfo, cElanCommand_Gen8, sCommonMessage, nSendCommandDelayTime_Gen8) == false)
                                    {
                                        bReadDataError = true;

                                        if (nRetryIndex == nRetryCount)
                                        {
                                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                            return false;
                                        }
                                        else
                                            break;
                                    }
                                }

                                if (bReadDataError == true)
                                    continue;

                                if (bGetOrigin == true)
                                {
                                    m_cOriginParameter.m_nPH1 = cReadDataInfo_PH1.m_nOutputValue2;
                                    m_cOriginParameter.m_nPH2 = cReadDataInfo_PH2.m_nOutputValue2;
                                    m_cOriginParameter.m_nPH3 = cReadDataInfo_PH3.m_nOutputValue2;
                                    m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_Sum.m_nOutputValue2;

                                    if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                                    {
                                        m_cOriginParameter.m_n_MS_SP_NUM = cReadDataInfo_SP_NUM.m_nOutputValue2;
                                        m_cOriginParameter.m_n_MS_EFFECT_NUM = cReadDataInfo_EFFECT_NUM.m_nOutputValue2;
                                        m_cOriginParameter.m_nPKT_WC = cReadDataInfo_PKT_WC.m_nOutputValue2;
                                    }
                                }
                                else
                                {
                                    m_cReadParameter.m_nPH1 = cReadDataInfo_PH1.m_nOutputValue2;
                                    m_cReadParameter.m_nPH2 = cReadDataInfo_PH2.m_nOutputValue2;
                                    m_cReadParameter.m_nPH3 = cReadDataInfo_PH3.m_nOutputValue2;
                                    m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_Sum.m_nOutputValue2;

                                    if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                                    {
                                        m_cReadParameter.m_n_MS_SP_NUM = cReadDataInfo_SP_NUM.m_nOutputValue2;
                                        m_cReadParameter.m_n_MS_EFFECT_NUM = cReadDataInfo_EFFECT_NUM.m_nOutputValue2;
                                        m_cReadParameter.m_nPKT_WC = cReadDataInfo_PKT_WC.m_nOutputValue2;
                                    }
                                }
                            }

                            if (bGetOrigin == true)
                                SetTestModeEnable(false);

                            if (bReadDataError == false)
                                break;
                        }
                    }
                    else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                    {
                        if (bGetOrigin == true)
                            SetTestModeEnable(true);

                        if (m_eICGenerationType == ICGenerationType.Gen7)
                        {
                            for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
                            {
                                bool bReadDataError = false;
                                ElanCommand_Gen6or7 cElanCommand = new ElanCommand_Gen6or7(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eICGenerationType, m_eICSolutionType);

                                Thread.Sleep(m_nNormalDelayTime);

                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_FIRTB = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIRTB, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_FIR_TAP_NUM = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_DFT_NUM = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_DFT_NUM, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_IQ_BSH_GP0 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._IQ_BSH, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_5 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_2 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_2, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_3 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_3, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_6 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_6, cElanCommand.m_cReadCommandInfo.sParameterName, 4);

                                List<ReadDataInfo_Gen6or7> cReadDataInfo_List = new List<ReadDataInfo_Gen6or7>()
                                {
                                    cReadDataInfo_MS_FIRTB,
                                    cReadDataInfo_MS_FIR_TAP_NUM,
                                    cReadDataInfo_MS_DFT_NUM,
                                    cReadDataInfo_MS_IQ_BSH_GP0,
                                    cReadDataInfo_MS_ANA_CTL_5,
                                    cReadDataInfo_MS_ANA_CTL_2,
                                    cReadDataInfo_MS_ANA_CTL_3,
                                    cReadDataInfo_MS_ANA_CTL_6
                                };

                                foreach (ReadDataInfo_Gen6or7 cReadDataInfo in cReadDataInfo_List)
                                {
                                    if (GetReadData_Gen6or7(cReadDataInfo, cElanCommand, sCommonMessage, 100) == false)
                                    {
                                        bReadDataError = true;

                                        if (nRetryIndex == nRetryCount)
                                        {
                                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                            return false;
                                        }
                                        else
                                            break;
                                    }
                                }

                                if (bReadDataError == true)
                                    continue;

                                if (bGetOrigin == true)
                                {
                                    m_cOriginParameter.m_n_MS_FIRTB = cReadDataInfo_MS_FIRTB.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_FIR_TAP_NUM = cReadDataInfo_MS_FIR_TAP_NUM.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_DFT_NUM.m_nOutputValue;

                                    if (m_eICSolutionType == ICSolutionType.Solution_7318)
                                        m_cOriginParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0.m_nOutputValue & 0x003F;
                                    else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                                        m_cOriginParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0.m_nOutputValue & 0x003F;

                                    m_cOriginParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue & 0x000C) >> 2;
                                    m_cOriginParameter.m_n_MS_VSEL = (cReadDataInfo_MS_ANA_CTL_2.m_nOutputValue & 0x0006) >> 1;
                                    m_cOriginParameter.m_n_MS_LG = (cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue & 0x0018) >> 3;
                                    m_cOriginParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue & 0x0F00) >> 8;

                                    m_cOriginParameter.m_n_MS_ANA_CTL_05 = cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_02 = cReadDataInfo_MS_ANA_CTL_2.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_03 = cReadDataInfo_MS_ANA_CTL_3.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_06 = cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue;
                                }
                                else
                                {
                                    m_cReadParameter.m_n_MS_FIRTB = cReadDataInfo_MS_FIRTB.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_FIR_TAP_NUM = cReadDataInfo_MS_FIR_TAP_NUM.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_DFT_NUM.m_nOutputValue;

                                    if (m_eICSolutionType == ICSolutionType.Solution_7318)
                                        m_cReadParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0.m_nOutputValue & 0x003F;
                                    else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                                        m_cReadParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0.m_nOutputValue & 0x003F;

                                    m_cReadParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue & 0x000C) >> 2;
                                    m_cReadParameter.m_n_MS_VSEL = (cReadDataInfo_MS_ANA_CTL_2.m_nOutputValue & 0x0006) >> 1;
                                    m_cReadParameter.m_n_MS_LG = (cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue & 0x0018) >> 3;
                                    m_cReadParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue & 0x0F00) >> 8;

                                    m_cReadParameter.m_n_MS_ANA_CTL_05 = cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_ANA_CTL_02 = cReadDataInfo_MS_ANA_CTL_2.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_ANA_CTL_03 = cReadDataInfo_MS_ANA_CTL_3.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_ANA_CTL_06 = cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue;
                                }

                                if (bReadDataError == false)
                                    break;
                            }
                        }
                        else if (m_eICGenerationType == ICGenerationType.Gen6)
                        {
                            for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
                            {
                                bool bReadDataError = false;
                                ElanCommand_Gen6or7 cElanCommand = new ElanCommand_Gen6or7(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eICGenerationType, m_eICSolutionType);

                                Thread.Sleep(m_nNormalDelayTime);

                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_FIRTB = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIRTB, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_FIR_TAP_NUM = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_DFT_NUM = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_DFT_NUM, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_IQ_BSH = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._IQ_BSH, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_8 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_8, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_5 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_4 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_4, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                
                                List<ReadDataInfo_Gen6or7> cReadDataInfo_List = new List<ReadDataInfo_Gen6or7>()
                                {
                                    cReadDataInfo_MS_FIRTB,
                                    cReadDataInfo_MS_FIR_TAP_NUM,
                                    cReadDataInfo_MS_DFT_NUM,
                                    cReadDataInfo_IQ_BSH,
                                    cReadDataInfo_MS_ANA_CTL_8,
                                    cReadDataInfo_MS_ANA_CTL_5,
                                    cReadDataInfo_MS_ANA_CTL_4
                                };

                                foreach (ReadDataInfo_Gen6or7 cReadDataInfo in cReadDataInfo_List)
                                {
                                    if (GetReadData_Gen6or7(cReadDataInfo, cElanCommand, sCommonMessage, 100) == false)
                                    {
                                        bReadDataError = true;

                                        if (nRetryIndex == nRetryCount)
                                        {
                                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                            return false;
                                        }
                                        else
                                            break;
                                    }
                                }

                                if (bReadDataError == true)
                                    continue;

                                if (bGetOrigin == true)
                                {
                                    m_cOriginParameter.m_n_MS_FIRTB = cReadDataInfo_MS_FIRTB.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_FIR_TAP_NUM = cReadDataInfo_MS_FIR_TAP_NUM.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_DFT_NUM.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_IQ_BSH = cReadDataInfo_IQ_BSH.m_nOutputValue;

                                    m_cOriginParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue & 0x00C0) >> 6;
                                    m_cOriginParameter.m_n_MS_VSEL = (cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue & 0x00C0) >> 6;
                                    m_cOriginParameter.m_n_MS_LG = (cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue & 0x000C) >> 2;
                                    m_cOriginParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_CTL_4.m_nOutputValue & 0x7000) >> 12;

                                    m_cOriginParameter.m_n_MS_ANA_CTL_08 = cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_05 = cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_04 = cReadDataInfo_MS_ANA_CTL_4.m_nOutputValue;
                                }
                                else
                                {
                                    m_cReadParameter.m_n_MS_FIRTB = cReadDataInfo_MS_FIRTB.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_FIR_TAP_NUM = cReadDataInfo_MS_FIR_TAP_NUM.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_DFT_NUM.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_IQ_BSH = cReadDataInfo_IQ_BSH.m_nOutputValue;

                                    m_cReadParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue & 0x00C0) >> 6;
                                    m_cReadParameter.m_n_MS_VSEL = (cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue & 0x00C0) >> 6;
                                    m_cReadParameter.m_n_MS_LG = (cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue & 0x000C) >> 2;
                                    m_cReadParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_CTL_4.m_nOutputValue & 0x7000) >> 12;

                                    m_cReadParameter.m_n_MS_ANA_CTL_08 = cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_ANA_CTL_05 = cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_ANA_CTL_04 = cReadDataInfo_MS_ANA_CTL_4.m_nOutputValue;
                                }

                                if (bReadDataError == false)
                                    break;
                            }
                        }

                        if (bGetOrigin == true)
                            SetTestModeEnable(false);
                    }
                    else
                    {
                        ReadDataInfo cReadDataInfo_PH1 = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH1, "PH1");
                        ReadDataInfo cReadDataInfo_PH2 = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH2, "PH2");
                        ReadDataInfo cReadDataInfo_PH3 = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH3, "PH3");
                        ReadDataInfo cReadDataInfo_DFT_NUM = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_SUM, "DFT_NUM");

                        List<ReadDataInfo> cReadDataInfo_List = new List<ReadDataInfo>()
                        {
                            cReadDataInfo_PH1,
                            cReadDataInfo_PH2,
                            cReadDataInfo_PH3,
                            cReadDataInfo_DFT_NUM
                        };

                        Thread.Sleep(m_nNormalDelayTime);

                        for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
                        {
                            bool bReadDataError = false;

                            foreach (ReadDataInfo cReadDataInfo in cReadDataInfo_List)
                            {
                                if (cReadDataInfo.m_bGetParameter == true)
                                    continue;

                                if (GetReadData(cReadDataInfo, sCommonMessage) == false)
                                {
                                    bReadDataError = true;

                                    if (nRetryIndex == nRetryCount)
                                    {
                                        m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                        return false;
                                    }
                                    else
                                        break;
                                }
                            }

                            if (bReadDataError == true)
                                continue;

                            if (bGetOrigin == true)
                            {
                                m_cOriginParameter.m_nPH1 = cReadDataInfo_PH1.m_nOutputValue;
                                m_cOriginParameter.m_nPH2 = cReadDataInfo_PH2.m_nOutputValue;
                                m_cOriginParameter.m_nPH3 = cReadDataInfo_PH3.m_nOutputValue;
                                m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_DFT_NUM.m_nOutputValue;
                            }
                            else
                            {
                                m_cReadParameter.m_nPH1 = cReadDataInfo_PH1.m_nOutputValue;
                                m_cReadParameter.m_nPH2 = cReadDataInfo_PH2.m_nOutputValue;
                                m_cReadParameter.m_nPH3 = cReadDataInfo_PH3.m_nOutputValue;
                                m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_DFT_NUM.m_nOutputValue;
                            }

                            if (bReadDataError == false)
                                break;
                        }

                        if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                        {
                            /*
                            byte[] byteCommand_Array = new byte[] { 0x53, 0xB1, 0x00, 0x01, 0x00, 0x00 };
                            nReadProjectOption = (short)xSendCmdCheck(byteCommand_Array, 0xB1);
                            */
                            //ElanTouch.GetProjectOption(ref nValue, 1000, m_nDeviceIndex);
                            int nValue = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                            short nReadProjectOption = (short)nValue;
                            OutputMessage(string.Format("-Read _Project_Option(0x53, 0xB1, 0x00, 0x01) Value=0x{0}", nReadProjectOption.ToString("X4")));
                            Thread.Sleep(m_nNormalDelayTime);

                            /*
                            byte[] byteCommand_Array = new byte[] { 0x53, 0xC1, 0x00, 0x01, 0x00, 0x00 };
                            nReadFWIPOption = (short)xSendCmdCheck(byteCommand_Array, 0xC1);
                            */
                            //ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDeviceIndex);
                            nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                            short nReadFWIPOption = (short)nValue;
                            OutputMessage(string.Format("-Read _FWIP_Option(0x53, 0xC1, 0x00, 0x01) Value=0x{0}", nReadFWIPOption.ToString("X4")));
                            Thread.Sleep(m_nNormalDelayTime);
                        }
                    }
                }
                else
                {
                    if (m_eICGenerationType == ICGenerationType.Gen8)
                    {
                        for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
                        {
                            bool bReadDataError = false;

                            if (nRetryIndex > 0)
                            {
                                SetTestModeEnable(false);
                                SetTestModeEnable(true);
                            }

                            if (bGetOrigin == true)
                                SetTestModeEnable(true);

                            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                                cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                            {
                                ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eSelfTraceType);

                                Thread.Sleep(nSendCommandDelayTime_Gen8);

                                ReadDataInfo_Gen8 cReadDataInfo_SP_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_SP_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_EFFECT_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_EFFECT_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_DFT_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_DFT_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH1 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH1, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH2E_LAT = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH2E_LAT, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH2_LAT = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH2_LAT, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH2 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH2, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_PKT_WC_L = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PKT_WC_L, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_BSH_ADC_TP_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_BSH_ADC_TP_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_EFFECT_FW_SET_COEF_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_EFFECT_FW_SET_COEF_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_DFT_NUM_IQ_FIR_CTL = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_DFT_NUM_IQ_FIR_CTL, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_ANA_TP_CTL_01 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_01, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_ANA_TP_CTL_00 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_00, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_IQ_BSH_GP0_GP1 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_IQ_BSH_GP0_GP1, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);

                                List<ReadDataInfo_Gen8> cReadDataInfo_List = new List<ReadDataInfo_Gen8>()
                                {
                                    cReadDataInfo_SP_NUM,
                                    cReadDataInfo_EFFECT_NUM,
                                    cReadDataInfo_DFT_NUM,
                                    cReadDataInfo_PH1,
                                    cReadDataInfo_PH2E_LAT,
                                    cReadDataInfo_PH2_LAT,
                                    cReadDataInfo_PH2,
                                    cReadDataInfo_PKT_WC_L,
                                    cReadDataInfo_BSH_ADC_TP_NUM,
                                    cReadDataInfo_EFFECT_FW_SET_COEF_NUM,
                                    cReadDataInfo_DFT_NUM_IQ_FIR_CTL,
                                    cReadDataInfo_ANA_TP_CTL_01,
                                    cReadDataInfo_ANA_TP_CTL_00,
                                    cReadDataInfo_IQ_BSH_GP0_GP1
                                };

                                /*
                                if (cFlowStep.m_eStep == MainStep.SELF_FREQUENCYSWEEP && 
                                    (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
                                    cReadDataInfo_List.Add(cReadDataInfo_ANA_TP_CTL_00);
                                */

                                foreach (ReadDataInfo_Gen8 cReadDataInfo in cReadDataInfo_List)
                                {
                                    if (GetReadData_Gen8(cReadDataInfo, cElanCommand_Gen8, sCommonMessage, nSendCommandDelayTime_Gen8) == false)
                                    {
                                        bReadDataError = true;

                                        if (nRetryIndex == nRetryCount)
                                        {
                                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                            return false;
                                        }
                                        else
                                            break;
                                    }
                                }

                                if (bReadDataError == true)
                                    continue;

                                if (bGetOrigin == true)
                                {
                                    m_cOriginParameter.m_n_SELF_SP_NUM = cReadDataInfo_SP_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_EFFECT_NUM = cReadDataInfo_EFFECT_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_nSelf_DFT_NUM = cReadDataInfo_DFT_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_PH1 = cReadDataInfo_PH1.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_PH2E_LMT = cReadDataInfo_PH2E_LAT.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_PH2E_LAT = cReadDataInfo_PH2E_LAT.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_PH2_LAT = cReadDataInfo_PH2_LAT.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_PH2_MUX_LAT = cReadDataInfo_PH2_LAT.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_PH2 = cReadDataInfo_PH2.m_nOutputValue2;
                                    m_cOriginParameter.m_nSelf_Gain = (cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue2 & 0x0078) >> 3;
                                    m_cOriginParameter.m_nSelf_CAG = (cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2 & 0x0038) >> 3;
                                    m_cOriginParameter.m_nSelf_IQ_BSH = (cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue2 & 0x0FC0) >> 6;
                                    m_cOriginParameter.m_n_SELF_PKT_WC_L = cReadDataInfo_PKT_WC_L.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_H = cReadDataInfo_BSH_ADC_TP_NUM.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_L = cReadDataInfo_BSH_ADC_TP_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H = cReadDataInfo_EFFECT_FW_SET_COEF_NUM.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L = cReadDataInfo_EFFECT_FW_SET_COEF_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H = cReadDataInfo_DFT_NUM_IQ_FIR_CTL.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L = cReadDataInfo_DFT_NUM_IQ_FIR_CTL.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H = cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L = cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2;
                                    m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_H = cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue1;
                                    m_cOriginParameter.m_n_SELF_IQ_BSH_GP0_GP1_L = cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue2;

                                    /*
                                    if (cFlowStep.m_eStep == MainStep.SELF_FREQUENCYSWEEP && 
                                        (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
                                    {
                                        m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue1;
                                        m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2;
                                    }
                                    */
                                }
                                else
                                {
                                    m_cReadParameter.m_n_SELF_SP_NUM = cReadDataInfo_SP_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_EFFECT_NUM = cReadDataInfo_EFFECT_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_nSelf_DFT_NUM = cReadDataInfo_DFT_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_PH1 = cReadDataInfo_PH1.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_PH2E_LMT = cReadDataInfo_PH2E_LAT.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_PH2E_LAT = cReadDataInfo_PH2E_LAT.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_PH2_LAT = cReadDataInfo_PH2_LAT.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_PH2_MUX_LAT = cReadDataInfo_PH2_LAT.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_PH2 = cReadDataInfo_PH2.m_nOutputValue2;
                                    m_cReadParameter.m_nSelf_Gain = (cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue2 & 0x0078) >> 3;
                                    m_cReadParameter.m_nSelf_CAG = (cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2 & 0x0038) >> 3;
                                    m_cReadParameter.m_nSelf_IQ_BSH = (cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue2 & 0x0FC0) >> 6;
                                    m_cReadParameter.m_n_SELF_PKT_WC_L = cReadDataInfo_PKT_WC_L.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_H = cReadDataInfo_BSH_ADC_TP_NUM.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_L = cReadDataInfo_BSH_ADC_TP_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H = cReadDataInfo_EFFECT_FW_SET_COEF_NUM.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L = cReadDataInfo_EFFECT_FW_SET_COEF_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H = cReadDataInfo_DFT_NUM_IQ_FIR_CTL.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L = cReadDataInfo_DFT_NUM_IQ_FIR_CTL.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_H = cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_L = cReadDataInfo_ANA_TP_CTL_01.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_H = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_L = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2;
                                    m_cReadParameter.m_n_SELF_IQ_BSH_GP0_GP1_H = cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue1;
                                    m_cReadParameter.m_n_SELF_IQ_BSH_GP0_GP1_L = cReadDataInfo_IQ_BSH_GP0_GP1.m_nOutputValue2;

                                    /*
                                    if (cFlowStep.m_eStep == MainStep.SELF_FREQUENCYSWEEP && 
                                        (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
                                    {
                                        m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_H = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue1;
                                        m_cReadParameter.m_n_SELF_ANA_TP_CTL_00_L = cReadDataInfo_ANA_TP_CTL_00.m_nOutputValue2;
                                    }
                                    */
                                }
                            }
                            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                            {
                                ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);

                                Thread.Sleep(nSendCommandDelayTime_Gen8);

                                ReadDataInfo_Gen8 cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_BIN_FIRCOEF_SEL_TAP_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_MS_IQ_BSH_GP0_GP1 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_IQ_BSH_GP0_GP1, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_MS_AFE_DFT_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_DFT_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_TP_CTL_01 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_CTL_04 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_TP_CTL_01_2 = null;
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_CTL_04_2 = null;
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_TP_CTL_06 = null;
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_TP_CTL_06_2 = null;
                                ReadDataInfo_Gen8 cReadDataInfo_MS_ANA_TP_CTL_07 = null;

                                List<ReadDataInfo_Gen8> cReadDataInfo_List = new List<ReadDataInfo_Gen8>()
                                {
                                    cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM,
                                    cReadDataInfo_MS_IQ_BSH_GP0_GP1,
                                    cReadDataInfo_MS_AFE_DFT_NUM,
                                    cReadDataInfo_MS_ANA_TP_CTL_01,
                                    cReadDataInfo_MS_ANA_CTL_04,
                                };

                                if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                                {
                                    cReadDataInfo_MS_ANA_TP_CTL_01_2 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01_2, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                    cReadDataInfo_MS_ANA_CTL_04_2 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04_2, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                    cReadDataInfo_MS_ANA_TP_CTL_06 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                    cReadDataInfo_MS_ANA_TP_CTL_06_2 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06_2, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);
                                    cReadDataInfo_MS_ANA_TP_CTL_07 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_07, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 2, 4);

                                    cReadDataInfo_List.Add(cReadDataInfo_MS_ANA_TP_CTL_01_2);
                                    cReadDataInfo_List.Add(cReadDataInfo_MS_ANA_CTL_04_2);
                                    cReadDataInfo_List.Add(cReadDataInfo_MS_ANA_TP_CTL_06);
                                    cReadDataInfo_List.Add(cReadDataInfo_MS_ANA_TP_CTL_06_2);
                                    cReadDataInfo_List.Add(cReadDataInfo_MS_ANA_TP_CTL_07);
                                }

                                foreach (ReadDataInfo_Gen8 cReadDataInfo in cReadDataInfo_List)
                                {
                                    if (GetReadData_Gen8(cReadDataInfo, cElanCommand_Gen8, sCommonMessage, nSendCommandDelayTime_Gen8) == false)
                                    {
                                        bReadDataError = true;

                                        if (nRetryIndex == nRetryCount)
                                        {
                                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                            return false;
                                        }
                                        else
                                            break;
                                    }
                                }

                                if (bReadDataError == true)
                                    continue;

                                if (bGetOrigin == true)
                                {
                                    m_cOriginParameter.m_n_MS_FIRCOEF_SEL = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue2 & 0x000F;
                                    m_cOriginParameter.m_n_MS_FIR_TAP_NUM = (cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue1 & 0x07F0) >> 4;
                                    m_cOriginParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2 & 0x0078) >> 3;
                                    m_cOriginParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0_GP1.m_nOutputValue2 & 0x003F;
                                    m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_AFE_DFT_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2 & 0x0180) >> 7;
                                    m_cOriginParameter.m_n_MS_VSEL = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue2 & 0x0003;

                                    m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue1;
                                    m_cOriginParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue2;
                                    m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_H = cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue1;
                                    m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_L = cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_04_H = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue1;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_04_L = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue2;

                                    if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                                    {
                                        m_cOriginParameter.m_n_MS_LG = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue2 & 0x0003;

                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_H = cReadDataInfo_MS_ANA_TP_CTL_01_2.m_nOutputValue1;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_01_2_L = cReadDataInfo_MS_ANA_TP_CTL_01_2.m_nOutputValue2;
                                        m_cOriginParameter.m_n_MS_ANA_CTL_04_2_H = cReadDataInfo_MS_ANA_CTL_04_2.m_nOutputValue1;
                                        m_cOriginParameter.m_n_MS_ANA_CTL_04_2_L = cReadDataInfo_MS_ANA_CTL_04_2.m_nOutputValue2;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_H = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue1;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_L = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue2;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_H = cReadDataInfo_MS_ANA_TP_CTL_06_2.m_nOutputValue1;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_06_2_L = cReadDataInfo_MS_ANA_TP_CTL_06_2.m_nOutputValue2;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_H = cReadDataInfo_MS_ANA_TP_CTL_07.m_nOutputValue1;
                                        m_cOriginParameter.m_n_MS_ANA_TP_CTL_07_L = cReadDataInfo_MS_ANA_TP_CTL_07.m_nOutputValue2;
                                    }
                                }
                                else
                                {
                                    m_cReadParameter.m_n_MS_FIRCOEF_SEL = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue2 & 0x000F;
                                    m_cReadParameter.m_n_MS_FIR_TAP_NUM = (cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue1 & 0x07F0) >> 4;
                                    m_cReadParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2 & 0x0078) >> 3;
                                    m_cReadParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0_GP1.m_nOutputValue2 & 0x003F;
                                    m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_AFE_DFT_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2 & 0x0180) >> 7;
                                    m_cReadParameter.m_n_MS_VSEL = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue2 & 0x0003;

                                    m_cReadParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue1;
                                    m_cReadParameter.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L = cReadDataInfo_MS_BIN_FIRCOEF_SEL_TAP_NUM.m_nOutputValue2;
                                    m_cReadParameter.m_n_MS_ANA_TP_CTL_01_H = cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue1;
                                    m_cReadParameter.m_n_MS_ANA_TP_CTL_01_L = cReadDataInfo_MS_ANA_TP_CTL_01.m_nOutputValue2;
                                    m_cReadParameter.m_n_MS_ANA_CTL_04_H = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue1;
                                    m_cReadParameter.m_n_MS_ANA_CTL_04_L = cReadDataInfo_MS_ANA_CTL_04.m_nOutputValue2;

                                    if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                                    {
                                        m_cReadParameter.m_n_MS_LG = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue2 & 0x0003;

                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_01_2_H = cReadDataInfo_MS_ANA_TP_CTL_01_2.m_nOutputValue1;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_01_2_L = cReadDataInfo_MS_ANA_TP_CTL_01_2.m_nOutputValue2;
                                        m_cReadParameter.m_n_MS_ANA_CTL_04_2_H = cReadDataInfo_MS_ANA_CTL_04_2.m_nOutputValue1;
                                        m_cReadParameter.m_n_MS_ANA_CTL_04_2_L = cReadDataInfo_MS_ANA_CTL_04_2.m_nOutputValue2;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_06_H = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue1;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_06_L = cReadDataInfo_MS_ANA_TP_CTL_06.m_nOutputValue2;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_06_2_H = cReadDataInfo_MS_ANA_TP_CTL_06_2.m_nOutputValue1;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_06_2_L = cReadDataInfo_MS_ANA_TP_CTL_06_2.m_nOutputValue2;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_07_H = cReadDataInfo_MS_ANA_TP_CTL_07.m_nOutputValue1;
                                        m_cReadParameter.m_n_MS_ANA_TP_CTL_07_L = cReadDataInfo_MS_ANA_TP_CTL_07.m_nOutputValue2;
                                    }
                                }
                            }
                            else
                            {
                                ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);

                                Thread.Sleep(nSendCommandDelayTime_Gen8);

                                ReadDataInfo_Gen8 cReadDataInfo_PH1 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_PH1, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH2 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_PH2, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_PH3 = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_PH3, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_DFT_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_DFT_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                ReadDataInfo_Gen8 cReadDataInfo_SP_NUM = null;
                                ReadDataInfo_Gen8 cReadDataInfo_EFFECT_NUM = null;
                                ReadDataInfo_Gen8 cReadDataInfo_PKT_WC = null;

                                List<ReadDataInfo_Gen8> cReadDataInfo_List = new List<ReadDataInfo_Gen8>()
                                {
                                    cReadDataInfo_PH1,
                                    cReadDataInfo_PH2,
                                    cReadDataInfo_PH3,
                                    cReadDataInfo_DFT_NUM
                                };

                                if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                                {
                                    cReadDataInfo_SP_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_SP_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                    cReadDataInfo_EFFECT_NUM = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_EFFECT_NUM, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 2);
                                    cReadDataInfo_PKT_WC = new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType.PKT_WC, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, 1, 4);

                                    cReadDataInfo_List.Add(cReadDataInfo_SP_NUM);
                                    cReadDataInfo_List.Add(cReadDataInfo_EFFECT_NUM);
                                    cReadDataInfo_List.Add(cReadDataInfo_PKT_WC);
                                }

                                foreach (ReadDataInfo_Gen8 cReadDataInfo in cReadDataInfo_List)
                                {
                                    if (GetReadData_Gen8(cReadDataInfo, cElanCommand_Gen8, sCommonMessage, nSendCommandDelayTime_Gen8) == false)
                                    {
                                        bReadDataError = true;

                                        if (nRetryIndex == nRetryCount)
                                        {
                                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                            return false;
                                        }
                                        else
                                            break;
                                    }
                                }

                                if (bReadDataError == true)
                                    continue;

                                if (bGetOrigin == true)
                                {
                                    m_cOriginParameter.m_nPH1 = cReadDataInfo_PH1.m_nOutputValue2;
                                    m_cOriginParameter.m_nPH2 = cReadDataInfo_PH2.m_nOutputValue2;
                                    m_cOriginParameter.m_nPH3 = cReadDataInfo_PH3.m_nOutputValue2;
                                    m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_DFT_NUM.m_nOutputValue2;

                                    if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                                    {
                                        m_cOriginParameter.m_n_MS_SP_NUM = cReadDataInfo_SP_NUM.m_nOutputValue2;
                                        m_cOriginParameter.m_n_MS_EFFECT_NUM = cReadDataInfo_EFFECT_NUM.m_nOutputValue2;
                                        m_cOriginParameter.m_nPKT_WC = cReadDataInfo_PKT_WC.m_nOutputValue2;
                                    }
                                }
                                else
                                {
                                    m_cReadParameter.m_nPH1 = cReadDataInfo_PH1.m_nOutputValue2;
                                    m_cReadParameter.m_nPH2 = cReadDataInfo_PH2.m_nOutputValue2;
                                    m_cReadParameter.m_nPH3 = cReadDataInfo_PH3.m_nOutputValue2;
                                    m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_DFT_NUM.m_nOutputValue2;

                                    if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                                    {
                                        m_cReadParameter.m_n_MS_SP_NUM = cReadDataInfo_SP_NUM.m_nOutputValue2;
                                        m_cReadParameter.m_n_MS_EFFECT_NUM = cReadDataInfo_EFFECT_NUM.m_nOutputValue2;
                                        m_cReadParameter.m_nPKT_WC = cReadDataInfo_PKT_WC.m_nOutputValue2;
                                    }
                                }
                            }

                            if (bGetOrigin == true)
                                SetTestModeEnable(false);

                            if (bReadDataError == false)
                                break;
                        }
                    }
                    else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                    {
                        if (m_eICGenerationType == ICGenerationType.Gen7)
                        {
                            for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
                            {
                                bool bReadDataError = false;
                                ElanCommand_Gen6or7 cElanCommand = new ElanCommand_Gen6or7(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eICGenerationType, m_eICSolutionType);

                                Thread.Sleep(m_nNormalDelayTime);

                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_FIRTB = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIRTB, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_FIR_TAP_NUM = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_DFT_NUM = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_DFT_NUM, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_IQ_BSH_GP0 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._IQ_BSH, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_5 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_2 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_2, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_3 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_3, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_6 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_6, cElanCommand.m_cReadCommandInfo.sParameterName, 4);

                                List<ReadDataInfo_Gen6or7> cReadDataInfo_List = new List<ReadDataInfo_Gen6or7>()
                                {
                                    cReadDataInfo_MS_FIRTB,
                                    cReadDataInfo_MS_FIR_TAP_NUM,
                                    cReadDataInfo_MS_DFT_NUM,
                                    cReadDataInfo_MS_IQ_BSH_GP0,
                                    cReadDataInfo_MS_ANA_CTL_5,
                                    cReadDataInfo_MS_ANA_CTL_2,
                                    cReadDataInfo_MS_ANA_CTL_3,
                                    cReadDataInfo_MS_ANA_CTL_6
                                };

                                foreach (ReadDataInfo_Gen6or7 cReadDataInfo in cReadDataInfo_List)
                                {
                                    if (GetReadData_Gen6or7(cReadDataInfo, cElanCommand, sCommonMessage, m_nNormalDelayTime) == false)
                                    {
                                        bReadDataError = true;

                                        if (nRetryIndex == nRetryCount)
                                        {
                                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                            return false;
                                        }
                                        else
                                            break;
                                    }
                                }

                                if (bReadDataError == true)
                                    continue;

                                if (bGetOrigin == true)
                                {
                                    m_cOriginParameter.m_n_MS_FIRTB = cReadDataInfo_MS_FIRTB.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_FIR_TAP_NUM = cReadDataInfo_MS_FIR_TAP_NUM.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_DFT_NUM.m_nOutputValue;

                                    if (m_eICSolutionType == ICSolutionType.Solution_7318)
                                        m_cOriginParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0.m_nOutputValue & 0x003F;
                                    else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                                        m_cOriginParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0.m_nOutputValue & 0x003F;

                                    m_cOriginParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue & 0x000C) >> 2;
                                    m_cOriginParameter.m_n_MS_VSEL = (cReadDataInfo_MS_ANA_CTL_2.m_nOutputValue & 0x0006) >> 1;
                                    m_cOriginParameter.m_n_MS_LG = (cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue & 0x0018) >> 3;
                                    m_cOriginParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue & 0x0F00) >> 8;

                                    m_cOriginParameter.m_n_MS_ANA_CTL_05 = cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_02 = cReadDataInfo_MS_ANA_CTL_2.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_03 = cReadDataInfo_MS_ANA_CTL_3.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_06 = cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue;
                                }
                                else
                                {
                                    m_cReadParameter.m_n_MS_FIRTB = cReadDataInfo_MS_FIRTB.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_FIR_TAP_NUM = cReadDataInfo_MS_FIR_TAP_NUM.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_DFT_NUM.m_nOutputValue;

                                    if (m_eICSolutionType == ICSolutionType.Solution_7318)
                                        m_cReadParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0.m_nOutputValue & 0x003F;
                                    else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                                        m_cReadParameter.m_n_MS_IQ_BSH = cReadDataInfo_MS_IQ_BSH_GP0.m_nOutputValue & 0x003F;

                                    m_cReadParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue & 0x000C) >> 2;
                                    m_cReadParameter.m_n_MS_VSEL = (cReadDataInfo_MS_ANA_CTL_2.m_nOutputValue & 0x0006) >> 1;
                                    m_cReadParameter.m_n_MS_LG = (cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue & 0x0018) >> 3;
                                    m_cReadParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue & 0x0F00) >> 8;

                                    m_cReadParameter.m_n_MS_ANA_CTL_05 = cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_ANA_CTL_02 = cReadDataInfo_MS_ANA_CTL_2.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_ANA_CTL_03 = cReadDataInfo_MS_ANA_CTL_3.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_ANA_CTL_06 = cReadDataInfo_MS_ANA_CTL_6.m_nOutputValue;
                                }

                                if (bReadDataError == false)
                                    break;
                            }
                        }
                        else if (m_eICGenerationType == ICGenerationType.Gen6)
                        {
                            for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
                            {
                                bool bReadDataError = false;
                                ElanCommand_Gen6or7 cElanCommand = new ElanCommand_Gen6or7(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eICGenerationType, m_eICSolutionType);

                                Thread.Sleep(m_nNormalDelayTime);

                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_FIRTB = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIRTB, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_FIR_TAP_NUM = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_DFT_NUM = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_DFT_NUM, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_IQ_BSH = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._IQ_BSH, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_8 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_8, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_5 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                ReadDataInfo_Gen6or7 cReadDataInfo_MS_ANA_CTL_4 = new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_4, cElanCommand.m_cReadCommandInfo.sParameterName, 4);
                                
                                List<ReadDataInfo_Gen6or7> cReadDataInfo_List = new List<ReadDataInfo_Gen6or7>()
                                {
                                    cReadDataInfo_MS_FIRTB,
                                    cReadDataInfo_MS_FIR_TAP_NUM,
                                    cReadDataInfo_MS_DFT_NUM,
                                    cReadDataInfo_IQ_BSH,
                                    cReadDataInfo_MS_ANA_CTL_8,
                                    cReadDataInfo_MS_ANA_CTL_5,
                                    cReadDataInfo_MS_ANA_CTL_4
                                };

                                foreach (ReadDataInfo_Gen6or7 cReadDataInfo in cReadDataInfo_List)
                                {
                                    if (GetReadData_Gen6or7(cReadDataInfo, cElanCommand, sCommonMessage, m_nNormalDelayTime) == false)
                                    {
                                        bReadDataError = true;

                                        if (nRetryIndex == nRetryCount)
                                        {
                                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                            return false;
                                        }
                                        else
                                            break;
                                    }
                                }

                                if (bReadDataError == true)
                                    continue;

                                if (bGetOrigin == true)
                                {
                                    m_cOriginParameter.m_n_MS_FIRTB = cReadDataInfo_MS_FIRTB.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_FIR_TAP_NUM = cReadDataInfo_MS_FIR_TAP_NUM.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_DFT_NUM.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_IQ_BSH = cReadDataInfo_IQ_BSH.m_nOutputValue; 

                                    m_cOriginParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue & 0x00C0) >> 6;
                                    m_cOriginParameter.m_n_MS_VSEL = (cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue & 0x00C0) >> 6;
                                    m_cOriginParameter.m_n_MS_LG = (cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue & 0x000C) >> 2;
                                    m_cOriginParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_CTL_4.m_nOutputValue & 0x7000) >> 12;

                                    m_cOriginParameter.m_n_MS_ANA_CTL_08 = cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_05 = cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue;
                                    m_cOriginParameter.m_n_MS_ANA_CTL_04 = cReadDataInfo_MS_ANA_CTL_4.m_nOutputValue;
                                }
                                else
                                {
                                    m_cReadParameter.m_n_MS_FIRTB = cReadDataInfo_MS_FIRTB.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_FIR_TAP_NUM = cReadDataInfo_MS_FIR_TAP_NUM.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_MS_DFT_NUM.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_IQ_BSH = cReadDataInfo_IQ_BSH.m_nOutputValue;

                                    m_cReadParameter.m_n_MS_SELC = (cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue & 0x00C0) >> 6;
                                    m_cReadParameter.m_n_MS_VSEL = (cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue & 0x00C0) >> 6;
                                    m_cReadParameter.m_n_MS_LG = (cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue & 0x000C) >> 2;
                                    m_cReadParameter.m_n_MS_SELGM = (cReadDataInfo_MS_ANA_CTL_4.m_nOutputValue & 0x7000) >> 12;

                                    m_cReadParameter.m_n_MS_ANA_CTL_08 = cReadDataInfo_MS_ANA_CTL_8.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_ANA_CTL_05 = cReadDataInfo_MS_ANA_CTL_5.m_nOutputValue;
                                    m_cReadParameter.m_n_MS_ANA_CTL_04 = cReadDataInfo_MS_ANA_CTL_4.m_nOutputValue;
                                }

                                if (bReadDataError == false)
                                    break;
                            }
                        }
                    }
                    else
                    {
                        /*
                        Thread.Sleep(m_nNormalDelayTime);
                        byte[] byteCommand_Array = new byte[] { 0x53, 0xC5, 0x00, 0x01, 0x00, 0x00 };
                        nPH1 = xSendCmdCheck(byteCommand_Array, 0xC5);
                        OutputMessage(string.Format("-{0} PH1=0x{1}", sCommonMessage, nPH1.ToString("x2").ToUpper()));
                        Thread.Sleep(m_nNormalDelayTime);
                        byteCommand_Array = new byte[] { 0x53, 0xC6, 0x00, 0x01, 0x00, 0x00 };
                        nPH2 = xSendCmdCheck(cmd, 0xC6);
                        OutputMessage(string.Format("-{0} PH2=0x{1}", sCommonMessage, nPH2.ToString("x2").ToUpper()));
                        Thread.Sleep(m_nNormalDelayTime);
                        byteCommand_Array = new byte[] { 0x53, 0xC7, 0x00, 0x01, 0x00, 0x00 };
                        nPH3 = xSendCmdCheck(cmd, 0xC7);
                        OutputMessage(string.Format("-{0} PH3=0x{1}", sCommonMessage, nPH3.ToString("x2").ToUpper()));
                        Thread.Sleep(m_nNormalDelayTime);
                        byteCommand_Array = new byte[] { 0x53, 0xDA, 0x00, 0x01, 0x00, 0x00 };
                        nSum = xSendCmdCheck(byteCommand_Array, 0xDA);
                        OutputMessage(string.Format("-{0} Sum=0x{1}", sCommonMessage, nSum.ToString("x2").ToUpper()));
                        */

                        Thread.Sleep(m_nNormalDelayTime);

                        for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
                        {
                            bool bReadDataError = false;

                            ReadDataInfo cReadDataInfo_PH1 = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH1, "PH1");
                            ReadDataInfo cReadDataInfo_PH2 = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH2, "PH2");
                            ReadDataInfo cReadDataInfo_PH3 = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH3, "PH3");
                            ReadDataInfo cReadDataInfo_DFT_NUM = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_SUM, "DFT_NUM");

                            List<ReadDataInfo> cReadDataInfo_List = new List<ReadDataInfo>()
                            {
                                cReadDataInfo_PH1,
                                cReadDataInfo_PH2,
                                cReadDataInfo_PH3,
                                cReadDataInfo_DFT_NUM
                            };

                            foreach (ReadDataInfo cReadDataInfo in cReadDataInfo_List)
                            {
                                if (GetReadData(cReadDataInfo, sCommonMessage) == false)
                                {
                                    bReadDataError = true;

                                    if (nRetryIndex == nRetryCount)
                                    {
                                        m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                        return false;
                                    }
                                    else
                                        break;
                                }
                            }

                            if (bReadDataError == true)
                                continue;

                            if (bGetOrigin == true)
                            {
                                m_cOriginParameter.m_nPH1 = cReadDataInfo_PH1.m_nOutputValue;
                                m_cOriginParameter.m_nPH2 = cReadDataInfo_PH2.m_nOutputValue;
                                m_cOriginParameter.m_nPH3 = cReadDataInfo_PH3.m_nOutputValue;
                                m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_DFT_NUM.m_nOutputValue;
                            }
                            else
                            {
                                m_cReadParameter.m_nPH1 = cReadDataInfo_PH1.m_nOutputValue;
                                m_cReadParameter.m_nPH2 = cReadDataInfo_PH2.m_nOutputValue;
                                m_cReadParameter.m_nPH3 = cReadDataInfo_PH3.m_nOutputValue;
                                m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_DFT_NUM.m_nOutputValue;
                            }

                            if (bReadDataError == false)
                                break;
                        }

                        if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                        {
                            /*
                            byte[] byteCommand_Array = new byte[] { 0x53, 0xB1, 0x00, 0x01, 0x00, 0x00 };
                            nReadProjectOption = (short)xSendCmdCheck(byteCommand_Array, 0xB1);
                            */
                            //ElanTouch.GetProjectOption(ref nValue, 1000, m_nDeviceIndex);
                            int nValue = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                            short nReadProjectOption = (short)nValue;
                            OutputMessage(string.Format("-Read _Project_Option(0x53, 0xB1, 0x00, 0x01) Value=0x{0}", nReadProjectOption.ToString("X4")));
                            Thread.Sleep(m_nNormalDelayTime);

                            /*
                            byte[] byteCommand_Array = new byte[] { 0x53, 0xC1, 0x00, 0x01, 0x00, 0x00 };
                            nReadFWIPOption = (short)xSendCmdCheck(byteCommand_Array, 0xC1);
                            */
                            //ElanTouch.GetFWIPOption(ref nValue, 1000, m_nDeviceIndex);
                            nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                            short nReadFWIPOption = (short)nValue;

                            OutputMessage(string.Format("-Read _FWIP_Option(0x53, 0xC1, 0x00, 0x01) Value=0x{0}", nReadFWIPOption.ToString("X4")));
                            Thread.Sleep(m_nNormalDelayTime);
                        }
                    }
                }
            }
#if _USE_9F07_SOCKET
            else
            {
                Thread.Sleep(m_nNormalDelayTime);

                for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
                {
                    bool bReadDataError = false;

                    ReadDataInfo cReadDataInfo_PH1 = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH1, "PH1");
                    ReadDataInfo cReadDataInfo_PH2 = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH2, "PH2");
                    ReadDataInfo cReadDataInfo_PH3 = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH3, "PH3");
                    ReadDataInfo cReadDataInfo_Sum = new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_SUM, "Sum");

                    List<ReadDataInfo> cReadDataInfo_List = new List<ReadDataInfo>()
                    {
                        cReadDataInfo_PH1,
                        cReadDataInfo_PH2,
                        cReadDataInfo_PH3,
                        cReadDataInfo_Sum
                    };

                    if (bNotDisableScanMode_9F07 == false)
                    {
                        SetScanModeDisable(true);

                        SkipNoUsedResponseData();
                    }

                    foreach (ReadDataInfo cReadDataInfo in cReadDataInfo_List)
                    {
                        if (GetReadData_9F07(cReadDataInfo, sCommonMessage) == false)
                        {
                            if (m_cfrmParent.m_bExecute == false)
                                return false;

                            bReadDataError = true;

                            /*
                            if (nRetryIndex == nRetryCount)
                            {
                                SetScanModeDisable(false);
                                m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                                return false;
                            }
                            else
                                break;
                            */
                            SetScanModeDisable(false);
                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                            return false;
                        }
                    }

                    SetScanModeDisable(false);

                    if (bReadDataError == true)
                        continue;

                    if (bGetOrigin == true)
                    {
                        m_cOriginParameter.m_nPH1 = cReadDataInfo_PH1.m_nOutputValue;
                        m_cOriginParameter.m_nPH2 = cReadDataInfo_PH2.m_nOutputValue;
                        m_cOriginParameter.m_nPH3 = cReadDataInfo_PH3.m_nOutputValue;
                        m_cOriginParameter.m_n_MS_DFT_NUM = cReadDataInfo_Sum.m_nOutputValue;
                    }
                    else
                    {
                        m_cReadParameter.m_nPH1 = cReadDataInfo_PH1.m_nOutputValue;
                        m_cReadParameter.m_nPH2 = cReadDataInfo_PH2.m_nOutputValue;
                        m_cReadParameter.m_nPH3 = cReadDataInfo_PH3.m_nOutputValue;
                        m_cReadParameter.m_n_MS_DFT_NUM = cReadDataInfo_Sum.m_nOutputValue;
                    }

                    if (bReadDataError == false)
                        break;
                }
            }
#endif

            Thread.Sleep(m_nNormalDelayTime);
            
            return true;
        }

        #region Mark It.
        /*
        private bool GetFWParameter(frmMain.FlowStep cFlowStep, bool bGetOrigin = false)
        {
            int nSendCommandDelayTime_Gen8 = ParamFingerAutoTuning.m_nGen8SendCommandDelayTime;

            string sCommonMessage = "Get";
            
            if (bGetOrigin == true)
                sCommonMessage = "Get Origin";

            if (ParamFingerAutoTuning.m_nWindowsSPIInterfaceType == (int)UserInterfaceDef.InterfaceType.IF_USB)
            {
                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    int nValue = 0;
                    bool bResult = false;

                    ElanTouchSwitch.EnableTestMode(true, m_nDeviceIndex, m_bSocketConnectType);
                    m_bEnterTestMode = true;
                    Thread.Sleep(m_nNormalDelayTime);

                    if (cFlowStep.m_eStep == MainStep.SELF_FREQUENCYSWEEP)
                    {
                        ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eSelfTraceType);

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_SP_NUM = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_SP_NUM, ElanCommand_Gen8.ParameterType._SELF_SP_NUM);
                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_SP_NUM.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_EFFECT_NUM = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_EFFECT_NUM, ElanCommand_Gen8.ParameterType._SELF_EFFECT_NUM);
                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_EFFECT_NUM.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_DFT_NUM = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_DFT_NUM, ElanCommand_Gen8.ParameterType._SELF_DFT_NUM);
                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_DFT_NUM.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_PH1 = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_PH1, ElanCommand_Gen8.ParameterType._SELF_PH1);
                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_PH1.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_PH2E_LMT = 0;
                        int n_SELF_PH2E_LAT = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_PH2E_LAT, ElanCommand_Gen8.ParameterType._SELF_PH2E_LAT);

                        n_SELF_PH2E_LMT = n_SELF_PH2E_LAT >> 8;
                        n_SELF_PH2E_LAT = (n_SELF_PH2E_LAT & 0x00FF);

                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_PH2_LAT = 0;
                        int n_SELF_PH2_MUX_LAT = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_PH2_LAT, ElanCommand_Gen8.ParameterType._SELF_PH2_LAT);

                        n_SELF_PH2_MUX_LAT = (n_SELF_PH2_LAT & 0x00FF);
                        n_SELF_PH2_LAT = n_SELF_PH2_LAT >> 8;

                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_PH2 = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_PH2, ElanCommand_Gen8.ParameterType._SELF_PH2);

                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_PH2.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_PKT_WC_L = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_PKT_WC_L, ElanCommand_Gen8.ParameterType._SELF_PKT_WC_L);

                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_PKT_WC_L.ToString("x4").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_BSH_ADC_TP_NUM_H = 0;
                        int n_SELF_BSH_ADC_TP_NUM_L = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref n_SELF_BSH_ADC_TP_NUM_H, ref n_SELF_BSH_ADC_TP_NUM_L, 
                                                             ElanCommand_Gen8.ParameterType._SELF_BSH_ADC_TP_NUM);

                        OutputMessage(string.Format("-{0} {1}=0x{2}{3}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                        n_SELF_BSH_ADC_TP_NUM_H.ToString("x4").ToUpper(),
                                                        n_SELF_BSH_ADC_TP_NUM_L.ToString("x4").ToUpper()));

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_EFFECT_FW_SET_COEF_NUM_H = 0;
                        int n_SELF_EFFECT_FW_SET_COEF_NUM_L = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref n_SELF_EFFECT_FW_SET_COEF_NUM_H, ref n_SELF_EFFECT_FW_SET_COEF_NUM_L,
                                                             ElanCommand_Gen8.ParameterType._SELF_EFFECT_FW_SET_COEF_NUM);

                        OutputMessage(string.Format("-{0} {1}=0x{2}{3}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                        n_SELF_EFFECT_FW_SET_COEF_NUM_H.ToString("x4").ToUpper(),
                                                        n_SELF_EFFECT_FW_SET_COEF_NUM_L.ToString("x4").ToUpper()));

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_DFT_NUM_IQ_FIR_CTL_H = 0;
                        int n_SELF_DFT_NUM_IQ_FIR_CTL_L = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref n_SELF_DFT_NUM_IQ_FIR_CTL_H, ref n_SELF_DFT_NUM_IQ_FIR_CTL_L,
                                                             ElanCommand_Gen8.ParameterType._SELF_DFT_NUM_IQ_FIR_CTL);

                        OutputMessage(string.Format("-{0} {1}=0x{2}{3}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                        n_SELF_DFT_NUM_IQ_FIR_CTL_H.ToString("x4").ToUpper(),
                                                        n_SELF_DFT_NUM_IQ_FIR_CTL_L.ToString("x4").ToUpper()));

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_ANA_TP_CTL_01_H = 0;
                        int n_SELF_ANA_TP_CTL_01_L = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref n_SELF_ANA_TP_CTL_01_H, ref n_SELF_ANA_TP_CTL_01_L,
                                                             ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_01);

                        OutputMessage(string.Format("-{0} {1}=0x{2}{3}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                        n_SELF_ANA_TP_CTL_01_H.ToString("x4").ToUpper(),
                                                        n_SELF_ANA_TP_CTL_01_L.ToString("x4").ToUpper()));

                        int nSELF_SELGM = (n_SELF_ANA_TP_CTL_01_L & 0x0078) >> 3;

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        if (bGetOrigin == true)
                        {
                            m_cOriginParameter.m_n_SELF_SP_NUM = n_SELF_SP_NUM;
                            m_cOriginParameter.m_n_SELF_EFFECT_NUM = n_SELF_EFFECT_NUM;
                            m_cOriginParameter.m_nSelf_Sum = n_SELF_DFT_NUM;
                            m_cOriginParameter.m_n_SELF_PH1 = n_SELF_PH1;
                            m_cOriginParameter.m_n_SELF_PH2E_LMT = n_SELF_PH2E_LMT;
                            m_cOriginParameter.m_n_SELF_PH2E_LAT = n_SELF_PH2E_LAT;
                            m_cOriginParameter.m_n_SELF_PH2_LAT = n_SELF_PH2_LAT;
                            m_cOriginParameter.m_n_SELF_PH2_MUX_LAT = n_SELF_PH2_MUX_LAT;
                            m_cOriginParameter.m_n_SELF_PH2 = n_SELF_PH2;
                            m_cOriginParameter.m_nSelf_Gain = nSELF_SELGM;
                            m_cOriginParameter.m_n_SELF_PKT_WC_L = n_SELF_PKT_WC_L;
                            m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_H = n_SELF_BSH_ADC_TP_NUM_H;
                            m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_L = n_SELF_BSH_ADC_TP_NUM_L;
                            m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H = n_SELF_EFFECT_FW_SET_COEF_NUM_H;
                            m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L = n_SELF_EFFECT_FW_SET_COEF_NUM_L;
                            m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H = n_SELF_DFT_NUM_IQ_FIR_CTL_H;
                            m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L = n_SELF_DFT_NUM_IQ_FIR_CTL_L;
                            m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H = n_SELF_ANA_TP_CTL_01_H;
                            m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L = n_SELF_ANA_TP_CTL_01_L;
                        }
                        else
                        {
                            m_cReadParameter.m_n_SELF_SP_NUM = n_SELF_SP_NUM;
                            m_cReadParameter.m_n_SELF_EFFECT_NUM = n_SELF_EFFECT_NUM;
                            m_cReadParameter.m_nSelf_Sum = n_SELF_DFT_NUM;
                            m_cReadParameter.m_n_SELF_PH1 = n_SELF_PH1;
                            m_cReadParameter.m_n_SELF_PH2E_LMT = n_SELF_PH2E_LMT;
                            m_cReadParameter.m_n_SELF_PH2E_LAT = n_SELF_PH2E_LAT;
                            m_cReadParameter.m_n_SELF_PH2_LAT = n_SELF_PH2_LAT;
                            m_cReadParameter.m_n_SELF_PH2_MUX_LAT = n_SELF_PH2_MUX_LAT;
                            m_cReadParameter.m_n_SELF_PH2 = n_SELF_PH2;
                            m_cReadParameter.m_nSelf_Gain = nSELF_SELGM;
                            m_cReadParameter.m_n_SELF_PKT_WC_L = n_SELF_PKT_WC_L;
                            m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_H = n_SELF_BSH_ADC_TP_NUM_H;
                            m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_L = n_SELF_BSH_ADC_TP_NUM_L;
                            m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H = n_SELF_EFFECT_FW_SET_COEF_NUM_H;
                            m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L = n_SELF_EFFECT_FW_SET_COEF_NUM_L;
                            m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H = n_SELF_DFT_NUM_IQ_FIR_CTL_H;
                            m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L = n_SELF_DFT_NUM_IQ_FIR_CTL_L;
                            m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_H = n_SELF_ANA_TP_CTL_01_H;
                            m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_L = n_SELF_ANA_TP_CTL_01_L;
                        }
                    }
                    else
                    {
                        ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int nPH1 = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref nPH1, ElanCommand_Gen8.ParameterType._MS_PH1);
                        OutputMessage(string.Format("-{0} PH1=0x{1}", sCommonMessage, nPH1.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int nPH2 = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref nPH2, ElanCommand_Gen8.ParameterType._MS_PH2);
                        OutputMessage(string.Format("-{0} PH2=0x{1}", sCommonMessage, nPH2.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int nPH3 = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref nPH3, ElanCommand_Gen8.ParameterType._MS_PH3);
                        OutputMessage(string.Format("-{0} PH3=0x{1}", sCommonMessage, nPH3.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int nSum = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref nSum, ElanCommand_Gen8.ParameterType._MS_DFT_NUM);
                        OutputMessage(string.Format("-{0} Sum=0x{1}", sCommonMessage, nSum.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        if (bGetOrigin == true)
                        {
                            m_cOriginParameter.m_nPH1 = nPH1;
                            m_cOriginParameter.m_nPH2 = nPH2;
                            m_cOriginParameter.m_nPH3 = nPH3;
                            m_cOriginParameter.m_nSum = nSum;
                        }
                        else
                        {
                            m_cReadParameter.m_nPH1 = nPH1;
                            m_cReadParameter.m_nPH2 = nPH2;
                            m_cReadParameter.m_nPH3 = nPH3;
                            m_cReadParameter.m_nSum = nSum;
                        }

                        if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                        {
                            int n_MS_SP_NUM = 0;
                            bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_MS_SP_NUM, ElanCommand_Gen8.ParameterType._MS_SP_NUM);
                            OutputMessage(string.Format("-{0} _MS_SP_NUM=0x{1}", sCommonMessage, n_MS_SP_NUM.ToString("x2").ToUpper()));
                            Thread.Sleep(nSendCommandDelayTime_Gen8);

                            int n_MS_EFFECT_NUM = 0;
                            bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_MS_EFFECT_NUM, ElanCommand_Gen8.ParameterType._MS_EFFECT_NUM);
                            OutputMessage(string.Format("-{0} _MS_EFFECT_NUM=0x{1}", sCommonMessage, n_MS_EFFECT_NUM.ToString("x2").ToUpper()));
                            Thread.Sleep(nSendCommandDelayTime_Gen8);

                            int nPKT_WC = 0;
                            bResult = cElanCommand_Gen8.ReadData(ref nValue, ref nPKT_WC, ElanCommand_Gen8.ParameterType.PKT_WC);
                            OutputMessage(string.Format("-{0} PKT_WC=0x{1}", sCommonMessage, nPKT_WC.ToString("x4").ToUpper()));
                            Thread.Sleep(nSendCommandDelayTime_Gen8);

                            if (bGetOrigin == true)
                            {
                                m_cOriginParameter.m_n_MS_SP_NUM = n_MS_SP_NUM;
                                m_cOriginParameter.m_n_MS_EFFECT_NUM = n_MS_EFFECT_NUM;
                                m_cOriginParameter.m_nPKT_WC = nPKT_WC;
                            }
                            else
                            {
                                m_cReadParameter.m_n_MS_SP_NUM = n_MS_SP_NUM;
                                m_cReadParameter.m_n_MS_EFFECT_NUM = n_MS_EFFECT_NUM;
                                m_cReadParameter.m_nPKT_WC = nPKT_WC;
                            }
                        }
                    }

                    ElanTouchSwitch.EnableTestMode(false, m_nDeviceIndex, m_bSocketConnectType);
                    m_bEnterTestMode = false;
                    Thread.Sleep(m_nNormalDelayTime);
                }
                else
                {
                    Thread.Sleep(m_nNormalDelayTime);

                    //nPH1 = ElanTouch.GetPH1(1000, m_nDeviceIndex);
                    int nPH1 = ElanTouchSwitch.GetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_PH1, m_nDeviceIndex, m_bSocketConnectType);
                    OutputMessage(string.Format("-{0} PH1=0x{1}", sCommonMessage, nPH1.ToString("x2").ToUpper()));
                    Thread.Sleep(m_nNormalDelayTime);

                    //nPH2 = ElanTouch.GetPH2(1000, m_nDeviceIndex);
                    int nPH2 = ElanTouchSwitch.GetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_PH2, m_nDeviceIndex, m_bSocketConnectType);
                    OutputMessage(string.Format("-{0} PH2=0x{1}", sCommonMessage, nPH2.ToString("x2").ToUpper()));
                    Thread.Sleep(m_nNormalDelayTime);

                    //nPH3 = ElanTouch.GetPH3(1000, m_nDeviceIndex);
                    int nPH3 = ElanTouchSwitch.GetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_PH3, m_nDeviceIndex, m_bSocketConnectType);
                    OutputMessage(string.Format("-{0} PH3=0x{1}", sCommonMessage, nPH3.ToString("x2").ToUpper()));
                    Thread.Sleep(m_nNormalDelayTime);

                    //nSum = ElanTouch.GetSUM(1000, m_nDeviceIndex);
                    int nSum = ElanTouchSwitch.GetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_SUM, m_nDeviceIndex, m_bSocketConnectType);
                    OutputMessage(string.Format("-{0} Sum=0x{1}", sCommonMessage, nSum.ToString("x2").ToUpper()));
                    Thread.Sleep(m_nNormalDelayTime);

                    if (bGetOrigin == true)
                    {
                        m_cOriginParameter.m_nPH1 = nPH1;
                        m_cOriginParameter.m_nPH2 = nPH2;
                        m_cOriginParameter.m_nPH3 = nPH3;
                        m_cOriginParameter.m_nSum = nSum;
                    }
                    else
                    {
                        m_cReadParameter.m_nPH1 = nPH1;
                        m_cReadParameter.m_nPH2 = nPH2;
                        m_cReadParameter.m_nPH3 = nPH3;
                        m_cReadParameter.m_nSum = nSum;
                    }
                }
            }
            else
            {
                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    int nValue = 0;
                    bool bResult = false;

                    ElanTouchSwitch.EnableTestMode(true, m_nDeviceIndex, m_bSocketConnectType);
                    m_bEnterTestMode = true;
                    Thread.Sleep(m_nNormalDelayTime);

                    if (cFlowStep.eStep == MainStep.SELF_FREQUENCYSWEEP)
                    {
                        ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eSelfTraceType);

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_SP_NUM = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_SP_NUM, ElanCommand_Gen8.ParameterType._SELF_SP_NUM);
                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_SP_NUM.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_EFFECT_NUM = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_EFFECT_NUM, ElanCommand_Gen8.ParameterType._SELF_EFFECT_NUM);
                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_EFFECT_NUM.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_DFT_NUM = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_DFT_NUM, ElanCommand_Gen8.ParameterType._SELF_DFT_NUM);
                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_DFT_NUM.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_PH1 = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_PH1, ElanCommand_Gen8.ParameterType._SELF_PH1);
                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_PH1.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_PH2E_LMT = 0;
                        int n_SELF_PH2E_LAT = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_PH2E_LAT, ElanCommand_Gen8.ParameterType._SELF_PH2E_LAT);

                        n_SELF_PH2E_LMT = n_SELF_PH2E_LAT >> 8;
                        n_SELF_PH2E_LAT = (n_SELF_PH2E_LAT & 0x00FF);

                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_PH2_LAT = 0;
                        int n_SELF_PH2_MUX_LAT = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_PH2_LAT, ElanCommand_Gen8.ParameterType._SELF_PH2_LAT);

                        n_SELF_PH2_MUX_LAT = (n_SELF_PH2_LAT & 0x00FF);
                        n_SELF_PH2_LAT = n_SELF_PH2_LAT >> 8;

                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_PH2 = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_PH2, ElanCommand_Gen8.ParameterType._SELF_PH2);

                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_PH2.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_PKT_WC_L = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_SELF_PKT_WC_L, ElanCommand_Gen8.ParameterType._SELF_PKT_WC_L);

                        OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName, n_SELF_PKT_WC_L.ToString("x4").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_BSH_ADC_TP_NUM_H = 0;
                        int n_SELF_BSH_ADC_TP_NUM_L = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref n_SELF_BSH_ADC_TP_NUM_H, ref n_SELF_BSH_ADC_TP_NUM_L,
                                                             ElanCommand_Gen8.ParameterType._SELF_BSH_ADC_TP_NUM);

                        OutputMessage(string.Format("-{0} {1}=0x{2}{3}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                        n_SELF_BSH_ADC_TP_NUM_H.ToString("x4").ToUpper(),
                                                        n_SELF_BSH_ADC_TP_NUM_L.ToString("x4").ToUpper()));

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_EFFECT_FW_SET_COEF_NUM_H = 0;
                        int n_SELF_EFFECT_FW_SET_COEF_NUM_L = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref n_SELF_EFFECT_FW_SET_COEF_NUM_H, ref n_SELF_EFFECT_FW_SET_COEF_NUM_L,
                                                             ElanCommand_Gen8.ParameterType._SELF_EFFECT_FW_SET_COEF_NUM);

                        OutputMessage(string.Format("-{0} {1}=0x{2}{3}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                        n_SELF_EFFECT_FW_SET_COEF_NUM_H.ToString("x4").ToUpper(),
                                                        n_SELF_EFFECT_FW_SET_COEF_NUM_L.ToString("x4").ToUpper()));

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_DFT_NUM_IQ_FIR_CTL_H = 0;
                        int n_SELF_DFT_NUM_IQ_FIR_CTL_L = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref n_SELF_DFT_NUM_IQ_FIR_CTL_H, ref n_SELF_DFT_NUM_IQ_FIR_CTL_L,
                                                             ElanCommand_Gen8.ParameterType._SELF_DFT_NUM_IQ_FIR_CTL);

                        OutputMessage(string.Format("-{0} {1}=0x{2}{3}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                        n_SELF_DFT_NUM_IQ_FIR_CTL_H.ToString("x4").ToUpper(),
                                                        n_SELF_DFT_NUM_IQ_FIR_CTL_L.ToString("x4").ToUpper()));

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int n_SELF_ANA_TP_CTL_01_H = 0;
                        int n_SELF_ANA_TP_CTL_01_L = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref n_SELF_ANA_TP_CTL_01_H, ref n_SELF_ANA_TP_CTL_01_L,
                                                             ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_01);

                        OutputMessage(string.Format("-{0} {1}=0x{2}{3}", sCommonMessage, cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                        n_SELF_ANA_TP_CTL_01_H.ToString("x4").ToUpper(),
                                                        n_SELF_ANA_TP_CTL_01_L.ToString("x4").ToUpper()));

                        int nSELF_SELGM = (n_SELF_ANA_TP_CTL_01_L & 0x0078) >> 3;

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        if (bGetOrigin == true)
                        {
                            m_cOriginParameter.m_n_SELF_SP_NUM = n_SELF_SP_NUM;
                            m_cOriginParameter.m_n_SELF_EFFECT_NUM = n_SELF_EFFECT_NUM;
                            m_cOriginParameter.m_nSelf_Sum = n_SELF_DFT_NUM;
                            m_cOriginParameter.m_n_SELF_PH1 = n_SELF_PH1;
                            m_cOriginParameter.m_n_SELF_PH2E_LMT = n_SELF_PH2E_LMT;
                            m_cOriginParameter.m_n_SELF_PH2E_LAT = n_SELF_PH2E_LAT;
                            m_cOriginParameter.m_n_SELF_PH2_LAT = n_SELF_PH2_LAT;
                            m_cOriginParameter.m_n_SELF_PH2_MUX_LAT = n_SELF_PH2_MUX_LAT;
                            m_cOriginParameter.m_n_SELF_PH2 = n_SELF_PH2;
                            m_cOriginParameter.m_nSelf_Gain = nSELF_SELGM;
                            m_cOriginParameter.m_n_SELF_PKT_WC_L = n_SELF_PKT_WC_L;
                            m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_H = n_SELF_BSH_ADC_TP_NUM_H;
                            m_cOriginParameter.m_n_SELF_BSH_ADC_TP_NUM_L = n_SELF_BSH_ADC_TP_NUM_L;
                            m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H = n_SELF_EFFECT_FW_SET_COEF_NUM_H;
                            m_cOriginParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L = n_SELF_EFFECT_FW_SET_COEF_NUM_L;
                            m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H = n_SELF_DFT_NUM_IQ_FIR_CTL_H;
                            m_cOriginParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L = n_SELF_DFT_NUM_IQ_FIR_CTL_L;
                            m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_H = n_SELF_ANA_TP_CTL_01_H;
                            m_cOriginParameter.m_n_SELF_ANA_TP_CTL_01_L = n_SELF_ANA_TP_CTL_01_L;
                        }
                        else
                        {
                            m_cReadParameter.m_n_SELF_SP_NUM = n_SELF_SP_NUM;
                            m_cReadParameter.m_n_SELF_EFFECT_NUM = n_SELF_EFFECT_NUM;
                            m_cReadParameter.m_nSelf_Sum = n_SELF_DFT_NUM;
                            m_cReadParameter.m_n_SELF_PH1 = n_SELF_PH1;
                            m_cReadParameter.m_n_SELF_PH2E_LMT = n_SELF_PH2E_LMT;
                            m_cReadParameter.m_n_SELF_PH2E_LAT = n_SELF_PH2E_LAT;
                            m_cReadParameter.m_n_SELF_PH2_LAT = n_SELF_PH2_LAT;
                            m_cReadParameter.m_n_SELF_PH2_MUX_LAT = n_SELF_PH2_MUX_LAT;
                            m_cReadParameter.m_n_SELF_PH2 = n_SELF_PH2;
                            m_cReadParameter.m_nSelf_Gain = nSELF_SELGM;
                            m_cReadParameter.m_n_SELF_PKT_WC_L = n_SELF_PKT_WC_L;
                            m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_H = n_SELF_BSH_ADC_TP_NUM_H;
                            m_cReadParameter.m_n_SELF_BSH_ADC_TP_NUM_L = n_SELF_BSH_ADC_TP_NUM_L;
                            m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H = n_SELF_EFFECT_FW_SET_COEF_NUM_H;
                            m_cReadParameter.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L = n_SELF_EFFECT_FW_SET_COEF_NUM_L;
                            m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H = n_SELF_DFT_NUM_IQ_FIR_CTL_H;
                            m_cReadParameter.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L = n_SELF_DFT_NUM_IQ_FIR_CTL_L;
                            m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_H = n_SELF_ANA_TP_CTL_01_H;
                            m_cReadParameter.m_n_SELF_ANA_TP_CTL_01_L = n_SELF_ANA_TP_CTL_01_L;
                        }
                    }
                    else
                    {
                        ElanCommand_Gen8 cElanCommand_Gen8 = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);

                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int nPH1 = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref nPH1, ElanCommand_Gen8.ParameterType._MS_PH1);
                        OutputMessage(string.Format("-{0} PH1=0x{1}", sCommonMessage, nPH1.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int nPH2 = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref nPH2, ElanCommand_Gen8.ParameterType._MS_PH2);
                        OutputMessage(string.Format("-{0} PH2=0x{1}", sCommonMessage, nPH2.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int nPH3 = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref nPH3, ElanCommand_Gen8.ParameterType._MS_PH3);
                        OutputMessage(string.Format("-{0} PH3=0x{1}", sCommonMessage, nPH3.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        int nSum = 0;
                        bResult = cElanCommand_Gen8.ReadData(ref nValue, ref nSum, ElanCommand_Gen8.ParameterType._MS_DFT_NUM);
                        OutputMessage(string.Format("-{0} Sum=0x{1}", sCommonMessage, nSum.ToString("x2").ToUpper()));
                        Thread.Sleep(nSendCommandDelayTime_Gen8);

                        if (bGetOrigin == true)
                        {
                            m_cOriginParameter.m_nPH1 = nPH1;
                            m_cOriginParameter.m_nPH2 = nPH2;
                            m_cOriginParameter.m_nPH3 = nPH3;
                            m_cOriginParameter.m_nSum = nSum;
                        }
                        else
                        {
                            m_cReadParameter.m_nPH1 = nPH1;
                            m_cReadParameter.m_nPH2 = nPH2;
                            m_cReadParameter.m_nPH3 = nPH3;
                            m_cReadParameter.m_nSum = nSum;
                        }

                        if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
                        {
                            int n_MS_SP_NUM = 0;
                            bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_MS_SP_NUM, ElanCommand_Gen8.ParameterType._MS_SP_NUM);
                            OutputMessage(string.Format("-{0} _MS_SP_NUM=0x{1}", sCommonMessage, n_MS_SP_NUM.ToString("x2").ToUpper()));
                            Thread.Sleep(nSendCommandDelayTime_Gen8);

                            int n_MS_EFFECT_NUM = 0;
                            bResult = cElanCommand_Gen8.ReadData(ref nValue, ref n_MS_EFFECT_NUM, ElanCommand_Gen8.ParameterType._MS_EFFECT_NUM);
                            OutputMessage(string.Format("-{0} _MS_EFFECT_NUM=0x{1}", sCommonMessage, n_MS_EFFECT_NUM.ToString("x2").ToUpper()));
                            Thread.Sleep(nSendCommandDelayTime_Gen8);

                            int nPKT_WC = 0;
                            bResult = cElanCommand_Gen8.ReadData(ref nValue, ref nPKT_WC, ElanCommand_Gen8.ParameterType.PKT_WC);
                            OutputMessage(string.Format("-{0} PKT_WC=0x{1}", sCommonMessage, nPKT_WC.ToString("x4").ToUpper()));
                            Thread.Sleep(nSendCommandDelayTime_Gen8);

                            if (bGetOrigin == true)
                            {
                                m_cOriginParameter.m_n_MS_SP_NUM = n_MS_SP_NUM;
                                m_cOriginParameter.m_n_MS_EFFECT_NUM = n_MS_EFFECT_NUM;
                                m_cOriginParameter.m_nPKT_WC = nPKT_WC;
                            }
                            else
                            {
                                m_cReadParameter.m_n_MS_SP_NUM = n_MS_SP_NUM;
                                m_cReadParameter.m_n_MS_EFFECT_NUM = n_MS_EFFECT_NUM;
                                m_cReadParameter.m_nPKT_WC = nPKT_WC;
                            }
                        }
                    }

                    ElanTouchSwitch.EnableTestMode(false, m_nDeviceIndex, m_bSocketConnectType);
                    m_bEnterTestMode = false;
                    Thread.Sleep(m_nNormalDelayTime);
                }
                else
                {
                    Thread.Sleep(m_nNormalDelayTime);

                    //nPH1 = ElanTouch.GetPH1(1000, m_nDeviceIndex);
                    int nPH1 = ElanTouchSwitch.GetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_PH1, m_nDeviceIndex, m_bSocketConnectType);
                    OutputMessage(string.Format("-{0} PH1=0x{1}", sCommonMessage, nPH1.ToString("x2").ToUpper()));
                    Thread.Sleep(m_nNormalDelayTime);

                    //nPH2 = ElanTouch.GetPH2(1000, m_nDeviceIndex);
                    int nPH2 = ElanTouchSwitch.GetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_PH2, m_nDeviceIndex, m_bSocketConnectType);
                    OutputMessage(string.Format("-{0} PH2=0x{1}", sCommonMessage, nPH2.ToString("x2").ToUpper()));
                    Thread.Sleep(m_nNormalDelayTime);

                    //nPH3 = ElanTouch.GetPH3(1000, m_nDeviceIndex);
                    int nPH3 = ElanTouchSwitch.GetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_PH3, m_nDeviceIndex, m_bSocketConnectType);
                    OutputMessage(string.Format("-{0} PH3=0x{1}", sCommonMessage, nPH3.ToString("x2").ToUpper()));
                    Thread.Sleep(m_nNormalDelayTime);

                    //nSum = ElanTouch.GetSUM(1000, m_nDeviceIndex);
                    int nSum = ElanTouchSwitch.GetAnalogParameter(ElanTouchSwitch.m_nPARAMETER_SUM, m_nDeviceIndex, m_bSocketConnectType);
                    OutputMessage(string.Format("-{0} Sum=0x{1}", sCommonMessage, nSum.ToString("x2").ToUpper()));
                    Thread.Sleep(m_nNormalDelayTime);

                    if (bGetOrigin == true)
                    {
                        m_cOriginParameter.m_nPH1 = nPH1;
                        m_cOriginParameter.m_nPH2 = nPH2;
                        m_cOriginParameter.m_nPH3 = nPH3;
                        m_cOriginParameter.m_nSum = nSum;
                    }
                    else
                    {
                        m_cReadParameter.m_nPH1 = nPH1;
                        m_cReadParameter.m_nPH2 = nPH2;
                        m_cReadParameter.m_nPH3 = nPH3;
                        m_cReadParameter.m_nSum = nSum;
                    }
                }
            }

            return true;
        }
        */
        #endregion

        public class ReadDataInfo_Gen8
        {
            public int m_nValue1 = 0;
            public int m_nValue2 = 0;
            public ElanCommand_Gen8.ParameterType m_eParameterType;
            public string m_sParameterName;
            public int m_nWordNumber;
            public int m_nHexNumber;

            public int m_nOutputValue1 = 0;
            public int m_nOutputValue2 = 0;

            public ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType eParameterType, string sParameterName, int nWordNumber, int nHexNumber)
            {
                m_eParameterType = eParameterType;
                m_sParameterName = sParameterName;
                m_nWordNumber = nWordNumber;
                m_nHexNumber = nHexNumber;
            }
        }

        private bool GetReadData_Gen8(ReadDataInfo_Gen8 cReadDataInfo_Gen8, ElanCommand_Gen8 cElanCommand_Gen8, string sCommonMessage, int nDelayTime)
        {
            bool bResult = cElanCommand_Gen8.ReadData(ref cReadDataInfo_Gen8.m_nValue1, ref cReadDataInfo_Gen8.m_nValue2, cReadDataInfo_Gen8.m_eParameterType);

            switch (cReadDataInfo_Gen8.m_eParameterType)
            {
                case ElanCommand_Gen8.ParameterType._SELF_SP_NUM:
                case ElanCommand_Gen8.ParameterType._SELF_EFFECT_NUM:
                case ElanCommand_Gen8.ParameterType._SELF_DFT_NUM:
                case ElanCommand_Gen8.ParameterType._SELF_PH1:
                case ElanCommand_Gen8.ParameterType._SELF_PH2:
                case ElanCommand_Gen8.ParameterType._SELF_PKT_WC_L:
                case ElanCommand_Gen8.ParameterType._MS_PH1:
                case ElanCommand_Gen8.ParameterType._MS_PH2:
                case ElanCommand_Gen8.ParameterType._MS_PH3:
                case ElanCommand_Gen8.ParameterType._MS_AFE_DFT_NUM:
                case ElanCommand_Gen8.ParameterType._MS_AFE_EFFECT_NUM:
                case ElanCommand_Gen8.ParameterType.PKT_WC:
                    cReadDataInfo_Gen8.m_nOutputValue2 = cReadDataInfo_Gen8.m_nValue2;
                    break;
                case ElanCommand_Gen8.ParameterType._MS_BIN_FIRCOEF_SEL_TAP_NUM:
                case ElanCommand_Gen8.ParameterType._MS_IQ_BSH_GP0_GP1:
                case ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01:
                case ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01_2:
                case ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04:
                case ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04_2:
                case ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06:
                case ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06_2:
                case ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_07:
                    cReadDataInfo_Gen8.m_nOutputValue1 = cReadDataInfo_Gen8.m_nValue1;
                    cReadDataInfo_Gen8.m_nOutputValue2 = cReadDataInfo_Gen8.m_nValue2;
                    break;
                case ElanCommand_Gen8.ParameterType._SELF_PH2E_LAT:
                    int n_SELF_PH2E_LMT = cReadDataInfo_Gen8.m_nValue2 >> 8;
                    int n_SELF_PH2E_LAT = (cReadDataInfo_Gen8.m_nValue2 & 0x00FF);
                    cReadDataInfo_Gen8.m_nOutputValue1 = n_SELF_PH2E_LAT;
                    cReadDataInfo_Gen8.m_nOutputValue2 = n_SELF_PH2E_LMT;
                    break;
                case ElanCommand_Gen8.ParameterType._SELF_PH2_LAT:
                    int n_SELF_PH2_MUX_LAT = (cReadDataInfo_Gen8.m_nValue2 & 0x00FF);
                    int n_SELF_PH2_LAT = cReadDataInfo_Gen8.m_nValue2 >> 8;
                    cReadDataInfo_Gen8.m_nOutputValue1 = n_SELF_PH2_MUX_LAT;
                    cReadDataInfo_Gen8.m_nOutputValue2 = n_SELF_PH2_LAT;
                    break;
                default:
                    cReadDataInfo_Gen8.m_nOutputValue1 = cReadDataInfo_Gen8.m_nValue1;
                    cReadDataInfo_Gen8.m_nOutputValue2 = cReadDataInfo_Gen8.m_nValue2;
                    break;
            }

            if (cReadDataInfo_Gen8.m_nWordNumber == 2)
            {
                if (cReadDataInfo_Gen8.m_nHexNumber == 4)
                {
                    OutputMessage(string.Format("-{0} {1}=0x{2}{3}", sCommonMessage,
                                                cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                cReadDataInfo_Gen8.m_nOutputValue1.ToString("x4").ToUpper(),
                                                cReadDataInfo_Gen8.m_nOutputValue2.ToString("x4").ToUpper()));
                }
                else if (cReadDataInfo_Gen8.m_nHexNumber == 2)
                {
                    OutputMessage(string.Format("-{0} {1}=0x{2}{3}", sCommonMessage,
                                                cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                cReadDataInfo_Gen8.m_nOutputValue1.ToString("x2").ToUpper(),
                                                cReadDataInfo_Gen8.m_nOutputValue2.ToString("x2").ToUpper()));
                }
            }
            else if (cReadDataInfo_Gen8.m_nWordNumber == 1)
            {
                if (cReadDataInfo_Gen8.m_nHexNumber == 4)
                {
                    OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage,
                                                cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                cReadDataInfo_Gen8.m_nOutputValue2.ToString("x4").ToUpper()));
                }
                else if (cReadDataInfo_Gen8.m_nHexNumber == 2)
                {
                    OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage,
                                                cElanCommand_Gen8.m_cReadCommandInfo.sParameterName,
                                                cReadDataInfo_Gen8.m_nOutputValue2.ToString("x2").ToUpper()));
                }
            }

            Thread.Sleep(nDelayTime);

            return bResult;
        }

        public class ReadDataInfo_Gen6or7
        {
            public int m_nValue = 0;
            public ElanCommand_Gen6or7.ParameterType m_eParameterType;
            //public string m_sParameterName;
            public int m_nHexNumber;

            public int m_nOutputValue = 0;

            public ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType eParameterType, string sParameterName, int nHexNumber)
            {
                m_eParameterType = eParameterType;
                m_nHexNumber = nHexNumber;
            }
        }

        private bool GetReadData_Gen6or7(ReadDataInfo_Gen6or7 cReadDataInfo, ElanCommand_Gen6or7 cElanCommand, string sCommonMessage, int nDelayTime)
        {
            bool bResult = cElanCommand.ReadData(ref cReadDataInfo.m_nValue, cReadDataInfo.m_eParameterType);

            switch (cReadDataInfo.m_eParameterType)
            {
                case ElanCommand_Gen6or7.ParameterType._MS_FIRTB:
                case ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM:
                case ElanCommand_Gen6or7.ParameterType._MS_DFT_NUM:
                case ElanCommand_Gen6or7.ParameterType._IQ_BSH:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_2:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_3:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_4:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_6:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_8:
                    cReadDataInfo.m_nOutputValue = cReadDataInfo.m_nValue;
                    break;
                default:
                    cReadDataInfo.m_nOutputValue = cReadDataInfo.m_nValue;
                    break;
            }

            if (cReadDataInfo.m_nHexNumber == 4)
            {
                OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage,
                                            cElanCommand.m_cReadCommandInfo.sParameterName,
                                            cReadDataInfo.m_nOutputValue.ToString("x4").ToUpper()));
            }
            else if (cReadDataInfo.m_nHexNumber == 2)
            {
                OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage,
                                            cElanCommand.m_cReadCommandInfo.sParameterName,
                                            cReadDataInfo.m_nOutputValue.ToString("x2").ToUpper()));
            }

            Thread.Sleep(nDelayTime);

            return bResult;
        }

        public class ReadDataInfo
        {
            public int m_nParameterType;
            public string m_sParameterName;

            public int m_nOutputValue = -1;
            public bool m_bGetParameter = false;

            public ReadDataInfo(int nParameterType, string sParameterName)
            {
                m_nParameterType = nParameterType;
                m_sParameterName = sParameterName;
            }
        }

        private bool GetReadData(ReadDataInfo cReadDataInfo, string sCommonMessage)
        {
            //nValue = ElanTouch.GetPH1(1000, m_nDeviceIndex);
            int nValue = ElanTouchSwitch.GetAnalogParameter(cReadDataInfo.m_nParameterType, m_nDeviceIndex, m_bSocketConnectType);
            cReadDataInfo.m_nOutputValue = nValue;
            OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cReadDataInfo.m_sParameterName, cReadDataInfo.m_nOutputValue.ToString("x2").ToUpper()));

            Thread.Sleep(m_nNormalDelayTime);

            if (nValue < 0)
                return false;
            else
            {
                cReadDataInfo.m_bGetParameter = true;
                return true;
            }
        }

#if _USE_9F07_SOCKET
        private bool GetReadData_9F07(ReadDataInfo cReadDataInfo, string sCommonMessage)
        {
            int nRetryCount = 5;
            byte[] byteBuffer_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];

            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                if (m_cfrmParent.m_bExecute == false)
                    return false;

                if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH1)
                {
                    ElanTouchSwitch.SendDevCommand(new byte[] { 0x53, 0xC5, 0x00, 0x00 }, 0, m_bSocketConnectType);
                }
                else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH2)
                {
                    ElanTouchSwitch.SendDevCommand(new byte[] { 0x53, 0xC6, 0x00, 0x00 }, 0, m_bSocketConnectType);
                }
                else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH3)
                {
                    ElanTouchSwitch.SendDevCommand(new byte[] { 0x53, 0xC7, 0x00, 0x00 }, 0, m_bSocketConnectType);
                }
                else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_SUM)
                {
                    ElanTouchSwitch.SendDevCommand(new byte[] { 0x53, 0xCB, 0x00, 0x00 }, 0, m_bSocketConnectType);
                }

                Thread.Sleep(30);

                int nResultFlag = ElanTouchSwitch.ReadDevData(byteBuffer_Array, 0, m_bSocketConnectType, 1000);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == true)
                {
                    bool bErrorFlag = true;

                    if (byteBuffer_Array[2] == 0x00 && byteBuffer_Array[3] == 0x00)
                    {
                        OutputMessage(string.Format("-{0} {1} Value Error(Response[0]=0x{2}, Response[1]=0x{3})(RetryCount={4})", sCommonMessage, cReadDataInfo.m_sParameterName, byteBuffer_Array[0].ToString("x2").ToUpper(), byteBuffer_Array[1].ToString("x2").ToUpper(), nRetryIndex));

                        if (nRetryIndex == nRetryCount - 1)
                            return false;

                        Thread.Sleep(m_nNormalDelayTime);
                        continue;
                    }
                    else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH1 && byteBuffer_Array[0] == 0x52 && byteBuffer_Array[1] == 0xC5)
                    {
                        bErrorFlag = false;
                    }
                    else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH2 && byteBuffer_Array[0] == 0x52 && byteBuffer_Array[1] == 0xC6)
                    {
                        bErrorFlag = false;
                    }
                    else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH3 && byteBuffer_Array[0] == 0x52 && byteBuffer_Array[1] == 0xC7)
                    {
                        bErrorFlag = false;
                    }
                    else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_SUM && byteBuffer_Array[0] == 0x52 && byteBuffer_Array[1] == 0xCB)
                    {
                        bErrorFlag = false;
                    }

                    if (bErrorFlag == true)
                    {
                        OutputMessage(string.Format("-{0} {1} Value Error(Response[0]=0x{2}, Response[1]=0x{3})(RetryCount={4})", sCommonMessage, cReadDataInfo.m_sParameterName, byteBuffer_Array[0].ToString("x2").ToUpper(), byteBuffer_Array[1].ToString("x2").ToUpper(), nRetryIndex));

                        if (nRetryIndex == nRetryCount - 1)
                            return false;

                        Thread.Sleep(m_nNormalDelayTime);
                        continue;
                    }

                    int nValue = (int)(byteBuffer_Array[2] << 8) + (int)byteBuffer_Array[3];
                    cReadDataInfo.m_nOutputValue = nValue;
                    OutputMessage(string.Format("-{0} {1}=0x{2}", sCommonMessage, cReadDataInfo.m_sParameterName, cReadDataInfo.m_nOutputValue.ToString("x2").ToUpper()));

                    Thread.Sleep(m_nNormalDelayTime);
                    break;
                }
                else
                {
                    OutputMessage(string.Format("-{0} {1} No Response Data(RetryCount={2})", sCommonMessage, cReadDataInfo.m_sParameterName, nRetryIndex));

                    if (nRetryIndex == nRetryCount - 1)
                        return false;
                }

                Thread.Sleep(m_nNormalDelayTime);
            }

            return true;
        }
#endif
#else
        /// <summary>
        /// 簡化版本的韌體參數讀取函式
        /// 支援多種IC世代(Gen6/Gen7/Gen8/Gen9)和不同的流程步驟
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="bGetOrigin"></param>
        /// <param name="bNotDisableScanMode_9F07"></param>
        /// <returns></returns>
        private bool GetFWParameter(frmMain.FlowStep cFlowStep, bool bGetOrigin = false, bool bNotDisableScanMode_9F07 = false)
        {
            // 早期返回檢查
            if (ParamFingerAutoTuning.m_nDisableSetAnalogParameter == 1)
                return true;

            // 初始化共用變數
            int nRetryCount = 3;
            int nSendCommandDelayTime_Gen8 = ParamFingerAutoTuning.m_nGen8SendCommandDelayTime;
            string sCommonMessage = bGetOrigin ? "Get Origin" : "Get";

            if (bGetOrigin)
                OutputMessage("-Get Original Analog Parameter");

            // Gen9 處理 (使用9F07協定)
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return HandleGen9Reading(cFlowStep, bGetOrigin, bNotDisableScanMode_9F07, sCommonMessage, nRetryCount);

            // Windows Socket + USB Interface 處理
            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_WINDOWS &&
                ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_USB)
            {
                return HandleWindowsUSB(cFlowStep, bGetOrigin, sCommonMessage, nRetryCount, nSendCommandDelayTime_Gen8);
            }

            // 其他Socket類型處理
            return HandleOtherSocket(cFlowStep, bGetOrigin, sCommonMessage, nRetryCount, nSendCommandDelayTime_Gen8);
        }

        /// <summary>
        /// 處理Gen9 (9F07協定)的參數讀取
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="bGetOrigin"></param>
        /// <param name="bNotDisableScanMode_9F07"></param>
        /// <param name="sCommonMessage"></param>
        /// <param name="nRetryCount"></param>
        /// <returns></returns>
        private bool HandleGen9Reading(frmMain.FlowStep cFlowStep, bool bGetOrigin, bool bNotDisableScanMode_9F07, string sCommonMessage, int nRetryCount)
        {
#if _USE_9F07_SOCKET
            Thread.Sleep(m_nNormalDelayTime);

            for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
            {
                var readDataList = new List<ReadDataInfo>
                {
                    new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH1, "PH1"),
                    new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH2, "PH2"),
                    new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH3, "PH3"),
                    new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_SUM, "Sum")
                };

                if (!bNotDisableScanMode_9F07)
                {
                    SetScanModeDisable(true);
                    SkipNoUsedResponseData();
                }

                bool bReadDataError = false;
                foreach (var dataInfo in readDataList)
                {
                    if (!GetReadData_9F07(dataInfo, sCommonMessage))
                    {
                        if (!m_cfrmParent.m_bExecute)
                            return false;

                        SetScanModeDisable(false);
                        m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                        return false;
                    }
                }

                SetScanModeDisable(false);

                if (!bReadDataError)
                {
                    StoreBasicParameters(readDataList, bGetOrigin);
                    break;
                }
            }

            Thread.Sleep(m_nNormalDelayTime);
            return true;
#else
            return true;
#endif
        }

        /// <summary>
        /// 處理Windows USB介面的參數讀取
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="bGetOrigin"></param>
        /// <param name="sCommonMessage"></param>
        /// <param name="nRetryCount"></param>
        /// <param name="nSendCommandDelayTime"></param>
        /// <returns></returns>
        private bool HandleWindowsUSB(frmMain.FlowStep cFlowStep, bool bGetOrigin, string sCommonMessage, int nRetryCount, int nSendCommandDelayTime)
        {
            if (m_eICGenerationType == ICGenerationType.Gen8)
                return HandleGen8Reading(cFlowStep, bGetOrigin, sCommonMessage, nRetryCount, nSendCommandDelayTime, true);

            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                if (bGetOrigin)
                    SetTestModeEnable(true);

                bool result = m_eICGenerationType == ICGenerationType.Gen7
                    ? HandleGen7RawADCSweep(bGetOrigin, sCommonMessage, nRetryCount)
                    : HandleGen6RawADCSweep(bGetOrigin, sCommonMessage, nRetryCount);

                if (bGetOrigin)
                    SetTestModeEnable(false);

                return result;
            }

            // 處理基本參數讀取(PH1/PH2/PH3/DFT_NUM)
            return HandleBasicParameterReading(bGetOrigin, sCommonMessage, nRetryCount);
        }

        /// <summary>
        /// 處理其他Socket類型的參數讀取
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="bGetOrigin"></param>
        /// <param name="sCommonMessage"></param>
        /// <param name="nRetryCount"></param>
        /// <param name="nSendCommandDelayTime"></param>
        /// <returns></returns>
        private bool HandleOtherSocket(frmMain.FlowStep cFlowStep, bool bGetOrigin, string sCommonMessage, int nRetryCount, int nSendCommandDelayTime)
        {
            if (m_eICGenerationType == ICGenerationType.Gen8)
                return HandleGen8Reading(cFlowStep, bGetOrigin, sCommonMessage, nRetryCount, nSendCommandDelayTime, false);

            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                bool result = m_eICGenerationType == ICGenerationType.Gen7
                    ? HandleGen7RawADCSweep(bGetOrigin, sCommonMessage, nRetryCount)
                    : HandleGen6RawADCSweep(bGetOrigin, sCommonMessage, nRetryCount);

                return result;
            }

            return HandleBasicParameterReading(bGetOrigin, sCommonMessage, nRetryCount);
        }

        /// <summary>
        /// 處理Gen8的參數讀取(包含Self和MS模式)
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="bGetOrigin"></param>
        /// <param name="sCommonMessage"></param>
        /// <param name="nRetryCount"></param>
        /// <param name="nDelay"></param>
        /// <param name="isWindowsUSB"></param>
        /// <returns></returns>
        private bool HandleGen8Reading(frmMain.FlowStep cFlowStep, bool bGetOrigin, string sCommonMessage, int nRetryCount, int nDelay, bool isWindowsUSB)
        {
            for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
            {
                bool bReadDataError = false;

                // 重試時重新啟用測試模式
                if (nRetryIndex > 0 && isWindowsUSB)
                {
                    SetTestModeEnable(false);
                    SetTestModeEnable(true);
                }

                if (bGetOrigin)
                    SetTestModeEnable(true);

                // Self模式參數讀取
                if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                {
                    if (!ReadGen8SelfParameters(bGetOrigin, sCommonMessage, nDelay, ref bReadDataError))
                    {
                        if (nRetryIndex == nRetryCount)
                        {
                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                            return false;
                        }
                        continue;
                    }
                }
                // Raw ADC Sweep模式參數讀取
                else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                {
                    if (!ReadGen8RawADCSweepParameters(bGetOrigin, sCommonMessage, nDelay, ref bReadDataError))
                    {
                        if (nRetryIndex == nRetryCount)
                        {
                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                            return false;
                        }
                        continue;
                    }
                }
                // MS模式基本參數讀取
                else
                {
                    if (!ReadGen8MSBasicParameters(bGetOrigin, sCommonMessage, nDelay, ref bReadDataError))
                    {
                        if (nRetryIndex == nRetryCount)
                        {
                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                            return false;
                        }
                        continue;
                    }
                }

                if (bGetOrigin && isWindowsUSB)
                    SetTestModeEnable(false);

                if (!bReadDataError)
                    break;
            }

            return true;
        }

        /// <summary>
        /// 讀取Gen8 Self模式參數
        /// </summary>
        /// <param name="bGetOrigin"></param>
        /// <param name="sCommonMessage"></param>
        /// <param name="nDelay"></param>
        /// <param name="bReadDataError"></param>
        /// <returns></returns>
        private bool ReadGen8SelfParameters(bool bGetOrigin, string sCommonMessage, int nDelay, ref bool bReadDataError)
        {
            var command = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eSelfTraceType);
            Thread.Sleep(nDelay);

            // 建立參數讀取列表
            var readDataList = new List<ReadDataInfo_Gen8>
            {
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_SP_NUM, command.m_cReadCommandInfo.sParameterName, 1, 2),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_EFFECT_NUM, command.m_cReadCommandInfo.sParameterName, 1, 2),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_DFT_NUM, command.m_cReadCommandInfo.sParameterName, 1, 2),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH1, command.m_cReadCommandInfo.sParameterName, 1, 2),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH2E_LAT, command.m_cReadCommandInfo.sParameterName, 1, 2),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH2_LAT, command.m_cReadCommandInfo.sParameterName, 1, 2),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PH2, command.m_cReadCommandInfo.sParameterName, 1, 4),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_PKT_WC_L, command.m_cReadCommandInfo.sParameterName, 1, 2),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_BSH_ADC_TP_NUM, command.m_cReadCommandInfo.sParameterName, 2, 4),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_EFFECT_FW_SET_COEF_NUM, command.m_cReadCommandInfo.sParameterName, 2, 4),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_DFT_NUM_IQ_FIR_CTL, command.m_cReadCommandInfo.sParameterName, 2, 4),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_01, command.m_cReadCommandInfo.sParameterName, 2, 4),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_00, command.m_cReadCommandInfo.sParameterName, 2, 4),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_IQ_BSH_GP0_GP1, command.m_cReadCommandInfo.sParameterName, 2, 4)
            };

            /*
            if (cFlowStep.m_eStep == MainStep.SELF_FREQUENCYSWEEP && 
                (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
                readDataList.Add(new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._SELF_ANA_TP_CTL_00, command.m_cReadCommandInfo.sParameterName, 2, 4));
            */

            // 讀取所有參數
            foreach (var dataInfo in readDataList)
            {
                if (!GetReadData_Gen8(dataInfo, command, sCommonMessage, nDelay))
                {
                    bReadDataError = true;
                    return false;
                }
            }

            // 儲存參數到對應的結構
            StoreSelfParameters(readDataList, bGetOrigin);
            return true;
        }

        /// <summary>
        /// 讀取Gen8 Raw ADC Sweep模式參數
        /// </summary>
        /// <param name="bGetOrigin"></param>
        /// <param name="sCommonMessage"></param>
        /// <param name="nDelay"></param>
        /// <param name="bReadDataError"></param>
        /// <returns></returns>
        private bool ReadGen8RawADCSweepParameters(bool bGetOrigin, string sCommonMessage, int nDelay, ref bool bReadDataError)
        {
            var command = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);
            Thread.Sleep(nDelay);

            var readDataList = new List<ReadDataInfo_Gen8>
            {
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_BIN_FIRCOEF_SEL_TAP_NUM, command.m_cReadCommandInfo.sParameterName, 2, 4),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_IQ_BSH_GP0_GP1, command.m_cReadCommandInfo.sParameterName, 2, 4),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_DFT_NUM, command.m_cReadCommandInfo.sParameterName, 1, 4),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01, command.m_cReadCommandInfo.sParameterName, 2, 4),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04, command.m_cReadCommandInfo.sParameterName, 2, 4)
            };

            // Solution 8F18 額外參數
            if (m_eICSolutionType == ICSolutionType.Solution_8F18)
            {
                readDataList.AddRange(new[]
                {
                    new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01_2, command.m_cReadCommandInfo.sParameterName, 2, 4),
                    new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04_2, command.m_cReadCommandInfo.sParameterName, 2, 4),
                    new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06, command.m_cReadCommandInfo.sParameterName, 2, 4),
                    new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06_2, command.m_cReadCommandInfo.sParameterName, 2, 4),
                    new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_07, command.m_cReadCommandInfo.sParameterName, 2, 4)
                });
            }

            foreach (var dataInfo in readDataList)
            {
                if (!GetReadData_Gen8(dataInfo, command, sCommonMessage, nDelay))
                {
                    bReadDataError = true;
                    return false;
                }
            }

            StoreRawADCSweepParameters(readDataList, bGetOrigin);
            return true;
        }

        /// <summary>
        /// 讀取Gen8 MS模式基本參數
        /// </summary>
        /// <param name="bGetOrigin"></param>
        /// <param name="sCommonMessage"></param>
        /// <param name="nDelay"></param>
        /// <param name="bReadDataError"></param>
        /// <returns></returns>
        private bool ReadGen8MSBasicParameters(bool bGetOrigin, string sCommonMessage, int nDelay, ref bool bReadDataError)
        {
            var command = new ElanCommand_Gen8(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType);
            Thread.Sleep(nDelay);

            var readDataList = new List<ReadDataInfo_Gen8>
            {
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_PH1, command.m_cReadCommandInfo.sParameterName, 1, 2),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_PH2, command.m_cReadCommandInfo.sParameterName, 1, 2),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_PH3, command.m_cReadCommandInfo.sParameterName, 1, 2),
                new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_DFT_NUM, command.m_cReadCommandInfo.sParameterName, 1, 2)
            };

            // 啟用Sum設定時的額外參數
            if (ParamFingerAutoTuning.m_nEnableSetSum == 1)
            {
                readDataList.AddRange(new[]
                {
                    new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_SP_NUM, command.m_cReadCommandInfo.sParameterName, 1, 2),
                    new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType._MS_AFE_EFFECT_NUM, command.m_cReadCommandInfo.sParameterName, 1, 2),
                    new ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType.PKT_WC, command.m_cReadCommandInfo.sParameterName, 1, 4)
                });
            }

            foreach (var dataInfo in readDataList)
            {
                if (!GetReadData_Gen8(dataInfo, command, sCommonMessage, nDelay))
                {
                    bReadDataError = true;
                    return false;
                }
            }

            StoreMSBasicParameters(readDataList, bGetOrigin);
            return true;
        }

        /// <summary>
        /// 處理Gen7 Raw ADC Sweep模式讀取
        /// </summary>
        /// <param name="bGetOrigin"></param>
        /// <param name="sCommonMessage"></param>
        /// <param name="nRetryCount"></param>
        /// <returns></returns>
        private bool HandleGen7RawADCSweep(bool bGetOrigin, string sCommonMessage, int nRetryCount)
        {
            for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
            {
                var command = new ElanCommand_Gen6or7(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eICGenerationType, m_eICSolutionType);
                Thread.Sleep(m_nNormalDelayTime);

                var readDataList = new List<ReadDataInfo_Gen6or7>
                {
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIRTB, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_DFT_NUM, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._IQ_BSH, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_2, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_3, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_6, command.m_cReadCommandInfo.sParameterName, 4)
                };

                bool bReadDataError = false;

                foreach (var dataInfo in readDataList)
                {
                    if (!GetReadData_Gen6or7(dataInfo, command, sCommonMessage, m_nNormalDelayTime))
                    {
                        bReadDataError = true;

                        if (nRetryIndex == nRetryCount)
                        {
                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                            return false;
                        }

                        break;
                    }
                }

                if (bReadDataError)
                    continue;

                StoreGen7Parameters(readDataList, bGetOrigin);
                break;
            }

            return true;
        }

        /// <summary>
        /// 處理Gen6 Raw ADC Sweep模式讀取
        /// </summary>
        /// <param name="bGetOrigin"></param>
        /// <param name="sCommonMessage"></param>
        /// <param name="nRetryCount"></param>
        /// <returns></returns>
        private bool HandleGen6RawADCSweep(bool bGetOrigin, string sCommonMessage, int nRetryCount)
        {
            for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
            {
                var command = new ElanCommand_Gen6or7(m_cfrmParent, m_nDeviceIndex, m_bSocketConnectType, m_eICGenerationType, m_eICSolutionType);
                Thread.Sleep(m_nNormalDelayTime);

                var readDataList = new List<ReadDataInfo_Gen6or7>
                {
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIRTB, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_DFT_NUM, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._IQ_BSH, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_8, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5, command.m_cReadCommandInfo.sParameterName, 4),
                    new ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_4, command.m_cReadCommandInfo.sParameterName, 4)
                };

                bool bReadDataError = false;

                foreach (var dataInfo in readDataList)
                {
                    if (!GetReadData_Gen6or7(dataInfo, command, sCommonMessage, m_nNormalDelayTime))
                    {
                        bReadDataError = true;

                        if (nRetryIndex == nRetryCount)
                        {
                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                            return false;
                        }

                        break;
                    }
                }

                if (bReadDataError)
                    continue;

                StoreGen6Parameters(readDataList, bGetOrigin);
                break;
            }

            return true;
        }

        /// <summary>
        /// 處理基本參數讀取(PH1/PH2/PH3/DFT_NUM)
        /// </summary>
        /// <param name="bGetOrigin"></param>
        /// <param name="sCommonMessage"></param>
        /// <param name="nRetryCount"></param>
        /// <returns></returns>
        private bool HandleBasicParameterReading(bool bGetOrigin, string sCommonMessage, int nRetryCount)
        {
            Thread.Sleep(m_nNormalDelayTime);

            for (int nRetryIndex = 0; nRetryIndex <= nRetryCount; nRetryIndex++)
            {
                var readDataList = new List<ReadDataInfo>
                {
                    new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH1, "PH1"),
                    new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH2, "PH2"),
                    new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_PH3, "PH3"),
                    new ReadDataInfo(ElanTouchSwitch.m_nPARAMETER_SUM, "DFT_NUM")
                };

                bool bReadDataError = false;

                foreach (var dataInfo in readDataList)
                {
                    if (!GetReadData(dataInfo, sCommonMessage))
                    {
                        bReadDataError = true;

                        if (nRetryIndex == nRetryCount)
                        {
                            m_sErrorMessage = string.Format("{0} FW Parameter Error", sCommonMessage);
                            return false;
                        }

                        break;
                    }
                }

                if (bReadDataError)
                    continue;

                StoreBasicParameters(readDataList, bGetOrigin);
                break;
            }

            // SSH Socket Server特殊處理
            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
            {
                /*
                byte[] byteCommand_Array = new byte[] { 0x53, 0xB1, 0x00, 0x01, 0x00, 0x00 };
                nReadProjectOption = (short)xSendCmdCheck(byteCommand_Array, 0xB1);
                */
                int nValue = ElanTouchSwitch.GetProjOption(m_nDeviceIndex, m_bSocketConnectType);
                short nReadProjectOption = (short)nValue;
                OutputMessage(string.Format("-Read _Project_Option(0x53, 0xB1, 0x00, 0x01) Value=0x{0}", nReadProjectOption.ToString("X4")));
                Thread.Sleep(m_nNormalDelayTime);

                /*
                byte[] byteCommand_Array = new byte[] { 0x53, 0xC1, 0x00, 0x01, 0x00, 0x00 };
                nReadFWIPOption = (short)xSendCmdCheck(byteCommand_Array, 0xC1);
                */
                nValue = ElanTouchSwitch.GetFWIPOption(m_nDeviceIndex, m_bSocketConnectType);
                short nReadFWIPOption = (short)nValue;
                OutputMessage(string.Format("-Read _FWIP_Option(0x53, 0xC1, 0x00, 0x01) Value=0x{0}", nReadFWIPOption.ToString("X4")));
                Thread.Sleep(m_nNormalDelayTime);
            }

            return true;
        }

        // ========== 參數儲存輔助函式 ==========

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="bGetOrigin"></param>
        private void StoreSelfParameters(List<ReadDataInfo_Gen8> dataList, bool bGetOrigin)
        {
            var target = bGetOrigin ? m_cOriginParameter : m_cReadParameter;

            target.m_n_SELF_SP_NUM = dataList[0].m_nOutputValue2;
            target.m_n_SELF_EFFECT_NUM = dataList[1].m_nOutputValue2;
            target.m_nSelf_DFT_NUM = dataList[2].m_nOutputValue2;
            target.m_n_SELF_PH1 = dataList[3].m_nOutputValue2;
            target.m_n_SELF_PH2E_LMT = dataList[4].m_nOutputValue2;
            target.m_n_SELF_PH2E_LAT = dataList[4].m_nOutputValue1;
            target.m_n_SELF_PH2_LAT = dataList[5].m_nOutputValue2;
            target.m_n_SELF_PH2_MUX_LAT = dataList[5].m_nOutputValue1;
            target.m_n_SELF_PH2 = dataList[6].m_nOutputValue2;
            target.m_nSelf_Gain = (dataList[11].m_nOutputValue2 & 0x0078) >> 3;
            target.m_nSelf_CAG = (dataList[12].m_nOutputValue2 & 0x0038) >> 3;
            target.m_nSelf_IQ_BSH = (dataList[13].m_nOutputValue2 & 0x0FC0) >> 6;
            target.m_n_SELF_PKT_WC_L = dataList[7].m_nOutputValue2;
            target.m_n_SELF_BSH_ADC_TP_NUM_H = dataList[8].m_nOutputValue1;
            target.m_n_SELF_BSH_ADC_TP_NUM_L = dataList[8].m_nOutputValue2;
            target.m_n_SELF_EFFECT_FW_SET_COEF_NUM_H = dataList[9].m_nOutputValue1;
            target.m_n_SELF_EFFECT_FW_SET_COEF_NUM_L = dataList[9].m_nOutputValue2;
            target.m_n_SELF_DFT_NUM_IQ_FIR_CTL_H = dataList[10].m_nOutputValue1;
            target.m_n_SELF_DFT_NUM_IQ_FIR_CTL_L = dataList[10].m_nOutputValue2;
            target.m_n_SELF_ANA_TP_CTL_01_H = dataList[11].m_nOutputValue1;
            target.m_n_SELF_ANA_TP_CTL_01_L = dataList[11].m_nOutputValue2;
            target.m_n_SELF_ANA_TP_CTL_00_H = dataList[12].m_nOutputValue1;
            target.m_n_SELF_ANA_TP_CTL_00_L = dataList[12].m_nOutputValue2;
            target.m_n_SELF_IQ_BSH_GP0_GP1_H = dataList[13].m_nOutputValue1;
            target.m_n_SELF_IQ_BSH_GP0_GP1_L = dataList[13].m_nOutputValue2;

            /*
            if (cFlowStep.m_eStep == MainStep.SELF_FREQUENCYSWEEP && 
                (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
            {
                target.m_n_SELF_ANA_TP_CTL_00_H = dataList[12].m_nOutputValue1;
                target.m_n_SELF_ANA_TP_CTL_00_L = dataList[12].m_nOutputValue2;
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="bGetOrigin"></param>
        private void StoreRawADCSweepParameters(List<ReadDataInfo_Gen8> dataList, bool bGetOrigin)
        {
            var target = bGetOrigin ? m_cOriginParameter : m_cReadParameter;

            target.m_n_MS_FIRCOEF_SEL = dataList[0].m_nOutputValue2 & 0x000F;
            target.m_n_MS_FIR_TAP_NUM = (dataList[0].m_nOutputValue1 & 0x07F0) >> 4;
            target.m_n_MS_SELGM = (dataList[3].m_nOutputValue2 & 0x0078) >> 3;
            target.m_n_MS_IQ_BSH = dataList[1].m_nOutputValue2 & 0x003F;
            target.m_n_MS_DFT_NUM = dataList[2].m_nOutputValue2;
            target.m_n_MS_SELC = (dataList[3].m_nOutputValue2 & 0x0180) >> 7;
            target.m_n_MS_VSEL = dataList[4].m_nOutputValue2 & 0x0003;

            target.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H = dataList[0].m_nOutputValue1;
            target.m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L = dataList[0].m_nOutputValue2;
            target.m_n_MS_ANA_TP_CTL_01_H = dataList[3].m_nOutputValue1;
            target.m_n_MS_ANA_TP_CTL_01_L = dataList[3].m_nOutputValue2;
            target.m_n_MS_ANA_CTL_04_H = dataList[4].m_nOutputValue1;
            target.m_n_MS_ANA_CTL_04_L = dataList[4].m_nOutputValue2;

            if (m_eICSolutionType == ICSolutionType.Solution_8F18)
            {
                target.m_n_MS_LG = dataList[7].m_nOutputValue2 & 0x0003;

                target.m_n_MS_ANA_TP_CTL_01_2_H = dataList[5].m_nOutputValue1;
                target.m_n_MS_ANA_TP_CTL_01_2_L = dataList[5].m_nOutputValue2;
                target.m_n_MS_ANA_CTL_04_2_H = dataList[6].m_nOutputValue1;
                target.m_n_MS_ANA_CTL_04_2_L = dataList[6].m_nOutputValue2;
                target.m_n_MS_ANA_TP_CTL_06_H = dataList[7].m_nOutputValue1;
                target.m_n_MS_ANA_TP_CTL_06_L = dataList[7].m_nOutputValue2;
                target.m_n_MS_ANA_TP_CTL_06_2_H = dataList[8].m_nOutputValue1;
                target.m_n_MS_ANA_TP_CTL_06_2_L = dataList[8].m_nOutputValue2;
                target.m_n_MS_ANA_TP_CTL_07_H = dataList[9].m_nOutputValue1;
                target.m_n_MS_ANA_TP_CTL_07_L = dataList[9].m_nOutputValue2;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="bGetOrigin"></param>
        private void StoreMSBasicParameters(List<ReadDataInfo_Gen8> dataList, bool bGetOrigin)
        {
            var target = bGetOrigin ? m_cOriginParameter : m_cReadParameter;

            target.m_nPH1 = dataList[0].m_nOutputValue2;
            target.m_nPH2 = dataList[1].m_nOutputValue2;
            target.m_nPH3 = dataList[2].m_nOutputValue2;
            target.m_n_MS_DFT_NUM = dataList[3].m_nOutputValue2;

            if (ParamFingerAutoTuning.m_nEnableSetSum == 1 && dataList.Count > 4)
            {
                target.m_n_MS_SP_NUM = dataList[4].m_nOutputValue2;
                target.m_n_MS_EFFECT_NUM = dataList[5].m_nOutputValue2;
                target.m_nPKT_WC = dataList[6].m_nOutputValue2;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="bGetOrigin"></param>
        private void StoreGen7Parameters(List<ReadDataInfo_Gen6or7> dataList, bool bGetOrigin)
        {
            var target = bGetOrigin ? m_cOriginParameter : m_cReadParameter;

            target.m_n_MS_FIRTB = dataList[0].m_nOutputValue;
            target.m_n_MS_FIR_TAP_NUM = dataList[1].m_nOutputValue;
            target.m_n_MS_DFT_NUM = dataList[2].m_nOutputValue;

            if (m_eICSolutionType == ICSolutionType.Solution_7318 || m_eICSolutionType == ICSolutionType.Solution_7315)
                target.m_n_MS_IQ_BSH = dataList[3].m_nOutputValue & 0x003F;

            target.m_n_MS_SELC = (dataList[4].m_nOutputValue & 0x000C) >> 2;
            target.m_n_MS_VSEL = (dataList[5].m_nOutputValue & 0x0006) >> 1;
            target.m_n_MS_LG = (dataList[7].m_nOutputValue & 0x0018) >> 3;
            target.m_n_MS_SELGM = (dataList[7].m_nOutputValue & 0x0F00) >> 8;

            target.m_n_MS_ANA_CTL_05 = dataList[4].m_nOutputValue;
            target.m_n_MS_ANA_CTL_02 = dataList[5].m_nOutputValue;
            target.m_n_MS_ANA_CTL_03 = dataList[6].m_nOutputValue;
            target.m_n_MS_ANA_CTL_06 = dataList[7].m_nOutputValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="bGetOrigin"></param>
        private void StoreGen6Parameters(List<ReadDataInfo_Gen6or7> dataList, bool bGetOrigin)
        {
            var target = bGetOrigin ? m_cOriginParameter : m_cReadParameter;

            target.m_n_MS_FIRTB = dataList[0].m_nOutputValue;
            target.m_n_MS_FIR_TAP_NUM = dataList[1].m_nOutputValue;
            target.m_n_MS_DFT_NUM = dataList[2].m_nOutputValue;
            target.m_n_MS_IQ_BSH = dataList[3].m_nOutputValue;

            target.m_n_MS_SELC = (dataList[4].m_nOutputValue & 0x00C0) >> 6;
            target.m_n_MS_VSEL = (dataList[5].m_nOutputValue & 0x00C0) >> 6;
            target.m_n_MS_LG = (dataList[4].m_nOutputValue & 0x000C) >> 2;
            target.m_n_MS_SELGM = (dataList[6].m_nOutputValue & 0x7000) >> 12;

            target.m_n_MS_ANA_CTL_08 = dataList[4].m_nOutputValue;
            target.m_n_MS_ANA_CTL_05 = dataList[5].m_nOutputValue;
            target.m_n_MS_ANA_CTL_04 = dataList[6].m_nOutputValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="bGetOrigin"></param>
        private void StoreBasicParameters(List<ReadDataInfo> dataList, bool bGetOrigin)
        {
            var target = bGetOrigin ? m_cOriginParameter : m_cReadParameter;

            target.m_nPH1 = dataList[0].m_nOutputValue;
            target.m_nPH2 = dataList[1].m_nOutputValue;
            target.m_nPH3 = dataList[2].m_nOutputValue;
            target.m_n_MS_DFT_NUM = dataList[3].m_nOutputValue;
        }

        // ========== 資料類別定義 ==========

        /// <summary>
        /// Gen8 參數讀取資料類別
        /// 用於Gen8世代IC的參數讀取和資料儲存
        /// </summary>
        public class ReadDataInfo_Gen8
        {
            public int m_nValue1 = 0;
            public int m_nValue2 = 0;
            public ElanCommand_Gen8.ParameterType m_eParameterType;
            public string m_sParameterName;
            public int m_nWordNumber;  // 讀取的Word數量 (1 or 2)
            public int m_nHexNumber;   // 顯示的十六進位位數 (2 or 4)

            public int m_nOutputValue1 = 0;
            public int m_nOutputValue2 = 0;

            public ReadDataInfo_Gen8(ElanCommand_Gen8.ParameterType eParameterType, string sParameterName, int nWordNumber, int nHexNumber)
            {
                m_eParameterType = eParameterType;
                m_sParameterName = sParameterName;
                m_nWordNumber = nWordNumber;
                m_nHexNumber = nHexNumber;
            }
        }

        /// <summary>
        /// Gen6/Gen7 參數讀取資料類別
        /// 用於Gen6和Gen7世代IC的參數讀取和資料儲存
        /// </summary>
        public class ReadDataInfo_Gen6or7
        {
            public int m_nValue = 0;
            public ElanCommand_Gen6or7.ParameterType m_eParameterType;
            //public string m_sParameterName;  // 暫時不使用
            public int m_nHexNumber;  // 顯示的十六進位位數 (2 or 4)

            public int m_nOutputValue = 0;

            public ReadDataInfo_Gen6or7(ElanCommand_Gen6or7.ParameterType eParameterType, string sParameterName, int nHexNumber)
            {
                m_eParameterType = eParameterType;
                m_nHexNumber = nHexNumber;
            }
        }

        /// <summary>
        /// 基本參數讀取資料類別
        /// 用於基本的參數讀取(PH1/PH2/PH3/DFT_NUM等)
        /// </summary>
        public class ReadDataInfo
        {
            public int m_nParameterType;
            public string m_sParameterName;

            public int m_nOutputValue = -1;
            public bool m_bGetParameter = false;  // 標記是否已成功讀取

            public ReadDataInfo(int nParameterType, string sParameterName)
            {
                m_nParameterType = nParameterType;
                m_sParameterName = sParameterName;
            }
        }

        // ========== Gen8 讀取函式 ==========

        /// <summary>
        /// Gen8 參數讀取底層函式
        /// 處理Gen8世代IC的參數讀取，包含特殊參數的位元處理和輸出格式化
        /// </summary>
        /// <param name="cReadDataInfo_Gen8">讀取資料資訊物件</param>
        /// <param name="cElanCommand_Gen8">Gen8命令物件</param>
        /// <param name="sCommonMessage">訊息前綴</param>
        /// <param name="nDelayTime">延遲時間(ms)</param>
        /// <returns>讀取是否成功</returns>
        private bool GetReadData_Gen8(ReadDataInfo_Gen8 cReadDataInfo_Gen8, ElanCommand_Gen8 cElanCommand_Gen8, string sCommonMessage, int nDelayTime)
        {
            // 執行實際的硬體讀取操作
            bool bResult = cElanCommand_Gen8.ReadData(ref cReadDataInfo_Gen8.m_nValue1, ref cReadDataInfo_Gen8.m_nValue2, cReadDataInfo_Gen8.m_eParameterType);

            // 根據參數類型進行特殊處理
            ProcessGen8OutputValues(cReadDataInfo_Gen8);

            // 格式化並輸出讀取結果
            OutputGen8Message(cReadDataInfo_Gen8, cElanCommand_Gen8, sCommonMessage);

            Thread.Sleep(nDelayTime);

            return bResult;
        }

        /// <summary>
        /// 處理Gen8參數的輸出值
        /// 根據不同的參數類型進行位元操作和數值轉換
        /// </summary>
        private void ProcessGen8OutputValues(ReadDataInfo_Gen8 cReadDataInfo_Gen8)
        {
            switch (cReadDataInfo_Gen8.m_eParameterType)
            {
                // 只使用Value2的參數類型
                case ElanCommand_Gen8.ParameterType._SELF_SP_NUM:
                case ElanCommand_Gen8.ParameterType._SELF_EFFECT_NUM:
                case ElanCommand_Gen8.ParameterType._SELF_DFT_NUM:
                case ElanCommand_Gen8.ParameterType._SELF_PH1:
                case ElanCommand_Gen8.ParameterType._SELF_PH2:
                case ElanCommand_Gen8.ParameterType._SELF_PKT_WC_L:
                case ElanCommand_Gen8.ParameterType._MS_PH1:
                case ElanCommand_Gen8.ParameterType._MS_PH2:
                case ElanCommand_Gen8.ParameterType._MS_PH3:
                case ElanCommand_Gen8.ParameterType._MS_AFE_DFT_NUM:
                case ElanCommand_Gen8.ParameterType._MS_AFE_EFFECT_NUM:
                case ElanCommand_Gen8.ParameterType.PKT_WC:
                    cReadDataInfo_Gen8.m_nOutputValue2 = cReadDataInfo_Gen8.m_nValue2;
                    break;

                // 同時使用Value1和Value2的參數類型
                case ElanCommand_Gen8.ParameterType._MS_BIN_FIRCOEF_SEL_TAP_NUM:
                case ElanCommand_Gen8.ParameterType._MS_IQ_BSH_GP0_GP1:
                case ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01:
                case ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_01_2:
                case ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04:
                case ElanCommand_Gen8.ParameterType._MS_ANA_CTL_04_2:
                case ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06:
                case ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_06_2:
                case ElanCommand_Gen8.ParameterType._MS_ANA_TP_CTL_07:
                    cReadDataInfo_Gen8.m_nOutputValue1 = cReadDataInfo_Gen8.m_nValue1;
                    cReadDataInfo_Gen8.m_nOutputValue2 = cReadDataInfo_Gen8.m_nValue2;
                    break;

                // SELF_PH2E_LAT: 需要分離高低位元組
                case ElanCommand_Gen8.ParameterType._SELF_PH2E_LAT:
                    int n_SELF_PH2E_LMT = cReadDataInfo_Gen8.m_nValue2 >> 8;        // 取高位元組
                    int n_SELF_PH2E_LAT = (cReadDataInfo_Gen8.m_nValue2 & 0x00FF);  // 取低位元組
                    cReadDataInfo_Gen8.m_nOutputValue1 = n_SELF_PH2E_LAT;
                    cReadDataInfo_Gen8.m_nOutputValue2 = n_SELF_PH2E_LMT;
                    break;

                // SELF_PH2_LAT: 需要分離高低位元組
                case ElanCommand_Gen8.ParameterType._SELF_PH2_LAT:
                    int n_SELF_PH2_MUX_LAT = (cReadDataInfo_Gen8.m_nValue2 & 0x00FF);  // 取低位元組
                    int n_SELF_PH2_LAT = cReadDataInfo_Gen8.m_nValue2 >> 8;            // 取高位元組
                    cReadDataInfo_Gen8.m_nOutputValue1 = n_SELF_PH2_MUX_LAT;
                    cReadDataInfo_Gen8.m_nOutputValue2 = n_SELF_PH2_LAT;
                    break;

                // 預設處理: 直接複製
                default:
                    cReadDataInfo_Gen8.m_nOutputValue1 = cReadDataInfo_Gen8.m_nValue1;
                    cReadDataInfo_Gen8.m_nOutputValue2 = cReadDataInfo_Gen8.m_nValue2;
                    break;
            }
        }

        /// <summary>
        /// 輸出Gen8參數讀取訊息
        /// 根據Word數量和Hex位數格式化輸出
        /// </summary>
        private void OutputGen8Message(ReadDataInfo_Gen8 cReadDataInfo_Gen8, ElanCommand_Gen8 cElanCommand_Gen8, string sCommonMessage)
        {
            string parameterName = cElanCommand_Gen8.m_cReadCommandInfo.sParameterName;

            if (cReadDataInfo_Gen8.m_nWordNumber == 2)
            {
                // 2個Word: 顯示Value1和Value2
                if (cReadDataInfo_Gen8.m_nHexNumber == 4)
                {
                    // 4位十六進位 (例: 0x12345678)
                    OutputMessage(string.Format("-{0} {1}=0x{2}{3}",
                        sCommonMessage,
                        parameterName,
                        cReadDataInfo_Gen8.m_nOutputValue1.ToString("x4").ToUpper(),
                        cReadDataInfo_Gen8.m_nOutputValue2.ToString("x4").ToUpper())
                    );
                }
                else if (cReadDataInfo_Gen8.m_nHexNumber == 2)
                {
                    // 2位十六進位 (例: 0x1234)
                    OutputMessage(string.Format("-{0} {1}=0x{2}{3}",
                        sCommonMessage,
                        parameterName,
                        cReadDataInfo_Gen8.m_nOutputValue1.ToString("x2").ToUpper(),
                        cReadDataInfo_Gen8.m_nOutputValue2.ToString("x2").ToUpper())
                    );
                }
            }
            else if (cReadDataInfo_Gen8.m_nWordNumber == 1)
            {
                // 1個Word: 只顯示Value2
                if (cReadDataInfo_Gen8.m_nHexNumber == 4)
                {
                    // 4位十六進位
                    OutputMessage(string.Format("-{0} {1}=0x{2}",
                        sCommonMessage,
                        parameterName,
                        cReadDataInfo_Gen8.m_nOutputValue2.ToString("x4").ToUpper())
                    );
                }
                else if (cReadDataInfo_Gen8.m_nHexNumber == 2)
                {
                    // 2位十六進位
                    OutputMessage(string.Format("-{0} {1}=0x{2}",
                        sCommonMessage,
                        parameterName,
                        cReadDataInfo_Gen8.m_nOutputValue2.ToString("x2").ToUpper())
                    );
                }
            }
        }

        // ========== Gen6/Gen7 讀取函式 ==========

        /// <summary>
        /// Gen6/Gen7 參數讀取底層函式
        /// 處理Gen6和Gen7世代IC的參數讀取，相較Gen8結構較簡單
        /// </summary>
        /// <param name="cReadDataInfo">讀取資料資訊物件</param>
        /// <param name="cElanCommand">Gen6或Gen7命令物件</param>
        /// <param name="sCommonMessage">訊息前綴</param>
        /// <param name="nDelayTime">延遲時間(ms)</param>
        /// <returns>讀取是否成功</returns>
        private bool GetReadData_Gen6or7(ReadDataInfo_Gen6or7 cReadDataInfo, ElanCommand_Gen6or7 cElanCommand, string sCommonMessage, int nDelayTime)
        {
            // 執行實際的硬體讀取操作
            bool bResult = cElanCommand.ReadData(ref cReadDataInfo.m_nValue, cReadDataInfo.m_eParameterType);

            // 處理輸出值 (Gen6/Gen7大多數情況下直接使用讀取值)
            ProcessGen6or7OutputValue(cReadDataInfo);

            // 格式化並輸出讀取結果
            OutputGen6or7Message(cReadDataInfo, cElanCommand, sCommonMessage);

            Thread.Sleep(nDelayTime);

            return bResult;
        }

        /// <summary>
        /// 處理Gen6/Gen7參數的輸出值
        /// Gen6/Gen7大多數情況下不需要特殊處理，直接複製即可
        /// </summary>
        private void ProcessGen6or7OutputValue(ReadDataInfo_Gen6or7 cReadDataInfo)
        {
            switch (cReadDataInfo.m_eParameterType)
            {
                case ElanCommand_Gen6or7.ParameterType._MS_FIRTB:
                case ElanCommand_Gen6or7.ParameterType._MS_FIR_TAP_NUM:
                case ElanCommand_Gen6or7.ParameterType._MS_DFT_NUM:
                case ElanCommand_Gen6or7.ParameterType._IQ_BSH:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_2:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_3:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_4:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_5:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_6:
                case ElanCommand_Gen6or7.ParameterType._MS_ANA_CTL_8:
                    cReadDataInfo.m_nOutputValue = cReadDataInfo.m_nValue;
                    break;
                default:
                    cReadDataInfo.m_nOutputValue = cReadDataInfo.m_nValue;
                    break;
            }
        }

        /// <summary>
        /// 輸出Gen6/Gen7參數讀取訊息
        /// 根據Hex位數格式化輸出
        /// </summary>
        private void OutputGen6or7Message(ReadDataInfo_Gen6or7 cReadDataInfo, ElanCommand_Gen6or7 cElanCommand, string sCommonMessage)
        {
            string parameterName = cElanCommand.m_cReadCommandInfo.sParameterName;

            if (cReadDataInfo.m_nHexNumber == 4)
            {
                // 4位十六進位
                OutputMessage(string.Format("-{0} {1}=0x{2}",
                    sCommonMessage,
                    parameterName,
                    cReadDataInfo.m_nOutputValue.ToString("x4").ToUpper())
                );
            }
            else if (cReadDataInfo.m_nHexNumber == 2)
            {
                // 2位十六進位
                OutputMessage(string.Format("-{0} {1}=0x{2}",
                    sCommonMessage,
                    parameterName,
                    cReadDataInfo.m_nOutputValue.ToString("x2").ToUpper())
                );
            }
        }

        // ========== 基本參數讀取函式 ==========

        /// <summary>
        /// 基本參數讀取底層函式
        /// 用於讀取基本的類比參數(PH1/PH2/PH3/DFT_NUM)
        /// </summary>
        /// <param name="cReadDataInfo">讀取資料資訊物件</param>
        /// <param name="sCommonMessage">訊息前綴</param>
        /// <returns>讀取是否成功</returns>
        private bool GetReadData(ReadDataInfo cReadDataInfo, string sCommonMessage)
        {
            //nValue = ElanTouch.GetPH1(1000, m_nDeviceIndex);  // 舊版API

            // 使用Switch版本的API讀取類比參數
            int nValue = ElanTouchSwitch.GetAnalogParameter(cReadDataInfo.m_nParameterType, m_nDeviceIndex, m_bSocketConnectType);
            cReadDataInfo.m_nOutputValue = nValue;

            // 輸出讀取結果 (固定使用2位十六進位)
            OutputMessage(string.Format("-{0} {1}=0x{2}",
                sCommonMessage,
                cReadDataInfo.m_sParameterName,
                cReadDataInfo.m_nOutputValue.ToString("x2").ToUpper())
            );

            Thread.Sleep(m_nNormalDelayTime);

            // 檢查讀取是否成功 (負值表示失敗)
            if (nValue < 0)
                return false;
            else
            {
                cReadDataInfo.m_bGetParameter = true;
                return true;
            }
        }

        // ========== Gen9 (9F07協定) 讀取函式 ==========

#if _USE_9F07_SOCKET
        /// <summary>
        /// Gen9 (9F07協定) 參數讀取底層函式
        /// 使用特殊的9F07通訊協定讀取參數，包含完整的錯誤檢查和重試機制
        /// </summary>
        /// <param name="cReadDataInfo">讀取資料資訊物件</param>
        /// <param name="sCommonMessage">訊息前綴</param>
        /// <returns>讀取是否成功</returns>
        private bool GetReadData_9F07(ReadDataInfo cReadDataInfo, string sCommonMessage)
        {
            int nRetryCount = 5;  // 重試次數
            byte[] byteBuffer_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];

            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                // 檢查是否需要中止執行
                if (m_cfrmParent.m_bExecute == false)
                    return false;

                // 根據參數類型發送對應的讀取命令
                SendParameterCommand_9F07(cReadDataInfo);

                Thread.Sleep(30);

                // 讀取裝置回應
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteBuffer_Array, 0, m_bSocketConnectType, 1000);

                // 檢查觸控面板狀態
                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == true)
                {
                    // 驗證回應資料
                    var validationResult = ValidateResponseData_9F07(byteBuffer_Array, cReadDataInfo, sCommonMessage, nRetryIndex);
            
                    if (validationResult == ValidationResult.Success)
                    {
                        // 解析並儲存讀取值
                        int nValue = (int)(byteBuffer_Array[2] << 8) + (int)byteBuffer_Array[3];
                        cReadDataInfo.m_nOutputValue = nValue;
                        OutputMessage(string.Format("-{0} {1}=0x{2}", 
                            sCommonMessage, 
                            cReadDataInfo.m_sParameterName, 
                            cReadDataInfo.m_nOutputValue.ToString("x2").ToUpper()));

                        Thread.Sleep(m_nNormalDelayTime);
                        break;
                    }
                    else if (validationResult == ValidationResult.FinalRetryFailed)
                    {
                        return false;
                    }
                    // ValidationResult.Retry: 繼續重試
                }
                else
                {
                    // 沒有回應資料
                    OutputMessage(string.Format("-{0} {1} No Response Data(RetryCount={2})", 
                        sCommonMessage, 
                        cReadDataInfo.m_sParameterName, 
                        nRetryIndex));

                    if (nRetryIndex == nRetryCount - 1)
                        return false;
                }

                Thread.Sleep(m_nNormalDelayTime);
            }

            return true;
        }

        /// <summary>
        /// 驗證結果列舉
        /// </summary>
        private enum ValidationResult
        {
            Success,           // 驗證成功
            Retry,            // 需要重試
            FinalRetryFailed  // 最後一次重試失敗
        }

        /// <summary>
        /// 發送9F07協定的參數讀取命令
        /// </summary>
        private void SendParameterCommand_9F07(ReadDataInfo cReadDataInfo)
        {
            if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH1)
            {
                ElanTouchSwitch.SendDevCommand(new byte[] { 0x53, 0xC5, 0x00, 0x00 }, 0, m_bSocketConnectType);
            }
            else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH2)
            {
                ElanTouchSwitch.SendDevCommand(new byte[] { 0x53, 0xC6, 0x00, 0x00 }, 0, m_bSocketConnectType);
            }
            else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH3)
            {
                ElanTouchSwitch.SendDevCommand(new byte[] { 0x53, 0xC7, 0x00, 0x00 }, 0, m_bSocketConnectType);
            }
            else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_SUM)
            {
                ElanTouchSwitch.SendDevCommand(new byte[] { 0x53, 0xCB, 0x00, 0x00 }, 0, m_bSocketConnectType);
            }
        }

        /// <summary>
        /// 驗證9F07協定的回應資料
        /// 檢查回應標頭是否正確，資料是否有效
        /// </summary>
        private ValidationResult ValidateResponseData_9F07(byte[] byteBuffer_Array, ReadDataInfo cReadDataInfo, string sCommonMessage, int nRetryIndex)
        {
            bool bErrorFlag = true;

            // 檢查資料是否為空值
            if (byteBuffer_Array[2] == 0x00 && byteBuffer_Array[3] == 0x00)
            {
                OutputMessage(string.Format("-{0} {1} Value Error(Response[0]=0x{2}, Response[1]=0x{3})(RetryCount={4})", 
                    sCommonMessage, 
                    cReadDataInfo.m_sParameterName, 
                    byteBuffer_Array[0].ToString("x2").ToUpper(), 
                    byteBuffer_Array[1].ToString("x2").ToUpper(), 
                    nRetryIndex));

                return HandleRetryLogic(nRetryIndex);
            }
    
            // 驗證回應標頭是否與請求匹配
            if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH1 && 
                byteBuffer_Array[0] == 0x52 && byteBuffer_Array[1] == 0xC5)
            {
                bErrorFlag = false;
            }
            else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH2 && 
                     byteBuffer_Array[0] == 0x52 && byteBuffer_Array[1] == 0xC6)
            {
                bErrorFlag = false;
            }
            else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_PH3 && 
                     byteBuffer_Array[0] == 0x52 && byteBuffer_Array[1] == 0xC7)
            {
                bErrorFlag = false;
            }
            else if (cReadDataInfo.m_nParameterType == ElanTouchSwitch.m_nPARAMETER_SUM && 
                     byteBuffer_Array[0] == 0x52 && byteBuffer_Array[1] == 0xCB)
            {
                bErrorFlag = false;
            }

            // 如果標頭錯誤
            if (bErrorFlag == true)
            {
                OutputMessage(string.Format("-{0} {1} Value Error(Response[0]=0x{2}, Response[1]=0x{3})(RetryCount={4})", 
                    sCommonMessage, 
                    cReadDataInfo.m_sParameterName, 
                    byteBuffer_Array[0].ToString("x2").ToUpper(), 
                    byteBuffer_Array[1].ToString("x2").ToUpper(), 
                    nRetryIndex));

                return HandleRetryLogic(nRetryIndex);
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// 處理重試邏輯
        /// </summary>
        private ValidationResult HandleRetryLogic(int nRetryIndex)
        {
            const int nRetryCount = 5;
    
            if (nRetryIndex == nRetryCount - 1)
                return ValidationResult.FinalRetryFailed;

            Thread.Sleep(m_nNormalDelayTime);
            return ValidationResult.Retry;
        }
#endif
#endif
    }
}
