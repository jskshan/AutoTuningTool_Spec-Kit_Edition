using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using FingerAutoTuning;
using FingerAutoTuningParameter;

namespace Elan
{
    public class AppCoreDefine
    {
        public enum MonitorState : int
        {
            MONITOR_ON = -1,
            MONITOR_OFF = 2,
            MONITOR_STANBY = 1
        };

        #region Record State
        public enum RecordState : int
        {
            NORMAL = 0,
            ACFRFIRSTSTAGE = 1,
            ACFRSECONDSTAGE = 2
        };

        public static Dictionary<RecordState, string> dictRecordStateCodeNameMappingTable = new Dictionary<RecordState, string>()
        {
            {RecordState.NORMAL,            "Normal"},
            {RecordState.ACFRFIRSTSTAGE,    "Stage 1"},
            {RecordState.ACFRSECONDSTAGE,   "Stage 2"}
        };
        #endregion

        public enum GetDataRelatedStep
        {
            Step_EnterKPKNCommand,
            Step_SetRead_Bulk_RAM_Data,
            Step_EnterTestMode,
            Step_ReConnect,
            Step_ExitKPKNCommand,
            Step_ExitTestMode,
            Step_TransferTestModeViaHID
        };

        public enum FileProcess : int
        {
            Move = 0,
            Copy = 1,
            Delete = 2
        };

        public static bool FileProcessFlow(FileProcess FileProcType, string sSourceFilePath, string sTargetFilePath, bool bShowMessage = true)
        {
            bool bProcessComplete = false;

            try
            {
                switch (FileProcType)
                {
                    case FileProcess.Move:
                        File.Move(sSourceFilePath, sTargetFilePath);
                        break;
                    case FileProcess.Copy:
                        File.Copy(sSourceFilePath, sTargetFilePath, true);
                        while (File.Exists(sTargetFilePath) == false)
                            Thread.Sleep(10);
                        break;
                    case FileProcess.Delete:
                        File.Delete(sTargetFilePath);
                        while (File.Exists(sTargetFilePath))
                            Thread.Sleep(10);
                        break;
                    default:
                        break;
                }

                bProcessComplete = true;
            }
            catch
            {
                switch (FileProcType)
                {
                    case FileProcess.Move:
                        if (bShowMessage == true)
                            MessageBox.Show(string.Format("Move File Error({0})", sSourceFilePath));
                        break;
                    case FileProcess.Copy:
                        if (bShowMessage == true)
                            MessageBox.Show(string.Format("Copy File Error({0})", sSourceFilePath));
                        break;
                    case FileProcess.Delete:
                        if (bShowMessage == true)
                            MessageBox.Show(string.Format("Delete File Error({0})", sTargetFilePath));
                        break;
                }
            }

            if (bProcessComplete == false)
            {
                try
                {
                    switch (FileProcType)
                    {
                        case FileProcess.Move:
                            File.Move(sSourceFilePath, sTargetFilePath);
                            break;
                        case FileProcess.Copy:
                            File.Copy(sSourceFilePath, sTargetFilePath, true);
                            while (File.Exists(sTargetFilePath) == false)
                                Thread.Sleep(10);
                            break;
                        case FileProcess.Delete:
                            File.Delete(sTargetFilePath);
                            while (File.Exists(sTargetFilePath))
                                Thread.Sleep(10);
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public class FWParameter
        {
            // Mutual
            public int m_nPH1 = 0;
            public int m_nPH2 = 0;
            public int m_nPH3 = 0;
            public int m_n_MS_DFT_NUM = 0;
            public int m_n_MS_SP_NUM = 0;
            public int m_n_MS_EFFECT_NUM = 0;
            public int m_nPKT_WC = 0;

            // Raw ADC Sweep
            public int m_n_MS_FIR_TAP_NUM = 0;
            public int m_n_MS_IQ_BSH = 0;
            public int m_n_MS_SELC = 0;
            public int m_n_MS_VSEL = 0;
            public int m_n_MS_LG = 0;
            public int m_n_MS_SELGM = 0;
            #region Gen8
            public int m_n_MS_FIRCOEF_SEL = 0;

            public int m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_H = 0;
            public int m_n_MS_BIN_FIRCOEF_SEL_TAP_NUM_L = 0;
            public int m_n_MS_ANA_TP_CTL_01_H = 0;
            public int m_n_MS_ANA_TP_CTL_01_L = 0;
            public int m_n_MS_ANA_TP_CTL_01_2_H = 0;
            public int m_n_MS_ANA_TP_CTL_01_2_L = 0;
            public int m_n_MS_ANA_CTL_04_H = 0;
            public int m_n_MS_ANA_CTL_04_L = 0;
            public int m_n_MS_ANA_CTL_04_2_H = 0;
            public int m_n_MS_ANA_CTL_04_2_L = 0;
            public int m_n_MS_ANA_TP_CTL_06_H = 0;
            public int m_n_MS_ANA_TP_CTL_06_L = 0;
            public int m_n_MS_ANA_TP_CTL_06_2_H = 0;
            public int m_n_MS_ANA_TP_CTL_06_2_L = 0;
            public int m_n_MS_ANA_TP_CTL_07_H = 0;
            public int m_n_MS_ANA_TP_CTL_07_L = 0;
            #endregion
            #region Gen7
            public int m_n_MS_FIRTB = 0;
            
            public int m_n_MS_ANA_CTL_02 = 0;
            public int m_n_MS_ANA_CTL_03 = 0;
            public int m_n_MS_ANA_CTL_04 = 0;
            public int m_n_MS_ANA_CTL_05 = 0;
            public int m_n_MS_ANA_CTL_06 = 0;
            public int m_n_MS_ANA_CTL_08 = 0;
            #endregion

            // Self
            public int m_n_SELF_PH1 = 0;
            public int m_n_SELF_PH2E_LMT = 0;
            public int m_n_SELF_PH2E_LAT = 0;
            public int m_n_SELF_PH2_LAT = 0;
            public int m_n_SELF_PH2_MUX_LAT = 0;
            public int m_n_SELF_PH2 = 0;
            public int m_nSelf_DFT_NUM = 0;
            public int m_nSelf_Gain = -1;
            public int m_nSelf_CAG = -1;
            public int m_nSelf_IQ_BSH = -1;
            public int m_n_SELF_SP_NUM = 0;
            public int m_n_SELF_EFFECT_NUM = 0;
            public int m_n_SELF_PKT_WC_L = 0;
            public int m_n_SELF_BSH_ADC_TP_NUM_H = 0;
            public int m_n_SELF_BSH_ADC_TP_NUM_L = 0;
            public int m_n_SELF_EFFECT_FW_SET_COEF_NUM_H = 0;
            public int m_n_SELF_EFFECT_FW_SET_COEF_NUM_L = 0;
            public int m_n_SELF_DFT_NUM_IQ_FIR_CTL_H = 0;
            public int m_n_SELF_DFT_NUM_IQ_FIR_CTL_L = 0;

            public int m_n_SELF_ANA_TP_CTL_01_H = 0;
            public int m_n_SELF_ANA_TP_CTL_01_L = 0;

            public int m_n_SELF_ANA_TP_CTL_00_H = 0;
            public int m_n_SELF_ANA_TP_CTL_00_L = 0;

            public int m_n_SELF_IQ_BSH_GP0_GP1_H = 0;
            public int m_n_SELF_IQ_BSH_GP0_GP1_L = 0;

            public void Initialize()
            {
                m_nPH1 = 0;
                m_nPH2 = 0;
                m_nPH3 = 0;
                m_n_MS_DFT_NUM = 0;

                m_n_MS_FIRCOEF_SEL = 0;
                m_n_MS_FIR_TAP_NUM = 1;
                m_n_MS_IQ_BSH = 18;
                m_n_MS_SELC = 2;
                m_n_MS_VSEL = 0;
                m_n_MS_LG = 1;

                m_n_MS_FIRTB = 0x4000;

                m_n_SELF_PH1 = 0;
                m_n_SELF_PH2E_LMT = 0;
                m_n_SELF_PH2E_LAT = 0;
                m_n_SELF_PH2_LAT = 0;
                m_n_SELF_PH2_MUX_LAT = 0;
                m_n_SELF_PH2 = 0;
                m_nSelf_DFT_NUM = 0;
                m_nSelf_Gain = -1;
                m_nSelf_CAG = -1;
                m_nSelf_IQ_BSH = -1;
            }
        }

        public const byte m_byteMODE_MUTUAL = 0x01;
        public const byte m_byteMODE_MUTUALSELF = 0x08;

        public class TimerCount
        {
            public int m_nHours = -1;
            public int m_nMinutes = -1;
            public int m_nSeconds = -1;

            public TimerCount()
            {
            }

            public void SetTimerParameter(int nHours, int nMinutes, int nSeconds)
            {
                m_nHours = nHours;
                m_nMinutes = nMinutes;
                m_nSeconds = nSeconds;
            }
        }

        public class FWParameterInfo
        {
            public string m_sParameterName = "";
            public int m_nSetValue = -1;
            public int m_nReadValue = -1;

            public FWParameterInfo(string sParameterName, int nSetValue, int nReadValue)
            {
                m_sParameterName = sParameterName;
                m_nSetValue = nSetValue;
                m_nReadValue = nReadValue;
            }
        }

        public class FileInfoData
        {
            public int m_nRX_TraceNumber = -1;
            public int m_nTX_TraceNumber = -1;

            public int m_nSetPH1 = -1;
            public int m_nSetPH2 = -1;
            public int m_nReadPH1 = -1;
            public int m_nReadPH2 = -1;
            //public int m_nFWIP_Option = -1;

            public int m_nSetSELC = -1;
            public int m_nSetVSEL = -1;
            public int m_nSetLG = -1;
            public int m_nSetSELGM = -1;
            public int m_nReadSELC = -1;
            public int m_nReadVSEL = -1;
            public int m_nReadLG = -1;
            public int m_nReadSELGM = -1;

            public string m_sSelfTraceType = "";
            public int m_nSet_SELF_PH1 = -1;
            public int m_nSet_SELF_PH2E_LAT = -1;
            public int m_nSet_SELF_PH2E_LMT = -1;
            public int m_nSet_SELF_PH2_LAT = -1;
            public int m_nSet_SELF_PH2 = -1;
            public int m_nSet_SELF_DFT_NUM = -1;
            public int m_nSet_SELF_SELGM = -1;
            public int m_nRead_SELF_PH1 = -1;
            public int m_nRead_SELF_PH2E_LAT = -1;
            public int m_nRead_SELF_PH2E_LMT = -1;
            public int m_nRead_SELF_PH2_LAT = -1;
            public int m_nRead_SELF_PH2 = -1;
            public int m_nRead_SELF_DFT_NUM = -1;
            public int m_nRead_SELF_SELGM = -1;

            public int m_nSelf_NCP = -1;
            public int m_nSelf_NCN = -1;
        }

        public class SelfParameter
        {
            public int m_nDFT_NUM = 0;
            public int m_nGain = -1;
            public int m_nCAL = 0;
            public int m_nCAG = -1;
            public int m_nIQ_BSH = -1;

            private MainStep m_eStep;
            private FrequencyItem m_cFrequencyItem;

            public SelfParameter()
            {
            }

            public void Initialize()
            {
                m_nDFT_NUM = 0;
                m_nGain = -1;
                m_nCAL = 0;
                m_nCAG = -1;
                m_nIQ_BSH = -1;
            }

            public void SetRelatedData(MainStep eStep, FrequencyItem cFrequencyItem)
            {
                m_eStep = eStep;
                m_cFrequencyItem = cFrequencyItem;
            }

            public void SetDFT_NUM()
            {
                if (m_eStep == MainStep.Self_FrequencySweep)
                {
                    if (ParamFingerAutoTuning.m_nSelfFSSum <= 0)
                        m_nDFT_NUM = m_cFrequencyItem.m_nSelf_DFT_NUM;
                    else
                        m_nDFT_NUM = ParamFingerAutoTuning.m_nSelfFSSum;
                }
                else if (m_eStep == MainStep.Self_NCPNCNSweep)
                {
                    m_nDFT_NUM = m_cFrequencyItem.m_nSelf_DFT_NUM;
                }
            }

            public void SetGain()
            {
                if (m_eStep == MainStep.Self_FrequencySweep)
                {
                    if (ParamFingerAutoTuning.m_nSelfFSGain < 0)
                    {
                        if (m_cFrequencyItem.m_nSelf_Gain >= 0)
                            m_nGain = m_cFrequencyItem.m_nSelf_Gain;
                    }
                    else
                        m_nGain = ParamFingerAutoTuning.m_nSelfFSGain;
                }
                else if (m_eStep == MainStep.Self_NCPNCNSweep)
                {
                    m_nGain = m_cFrequencyItem.m_nSelf_Gain;
                }
            }

            public void SetCAL()
            {
                if (m_eStep == MainStep.Self_FrequencySweep)
                {
                    if (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2)
                    {
                        if (ParamFingerAutoTuning.m_nSelfFSCALValue < 0)
                            m_nCAL = m_cFrequencyItem.m_nSelf_CAL;
                        else
                            m_nCAL = ParamFingerAutoTuning.m_nSelfFSCALValue;
                    }
                }
            }

            public void SetCAG()
            {
                if (m_eStep == MainStep.Self_FrequencySweep)
                {
                    if (ParamFingerAutoTuning.m_nSelfFSCAG < 0)
                    {
                        if (m_cFrequencyItem.m_nSelf_CAG >= 0)
                            m_nCAG = m_cFrequencyItem.m_nSelf_CAG;
                    }
                    else
                        m_nCAG = ParamFingerAutoTuning.m_nSelfFSCAG;
                }
            }

            public void SetIQ_BSH()
            {
                if (m_eStep == MainStep.Self_FrequencySweep)
                {
                    if (ParamFingerAutoTuning.m_nSelfFSIQ_BSH < 0)
                    {
                        if (m_cFrequencyItem.m_nSelf_IQ_BSH >= 0)
                            m_nIQ_BSH = m_cFrequencyItem.m_nSelf_IQ_BSH;
                    }
                    else
                        m_nIQ_BSH = ParamFingerAutoTuning.m_nSelfFSIQ_BSH;
                }
            }
        }

        //Set State
        public enum SetState
        {
            Initial,
            Success,
            Error,
            Reset,
            EnterTestModeError,
            Running
        }
    }
}
