using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FingerAutoTuning
{
    class DataAnalysis
    {
        private AnalysisFlow m_cAnalysisFlowProcess = null;
        private frmMain m_cfrmParent = null;
        private MainStep m_eStep;

        /// <summary>
        /// 依目前 MainStep 建立對應的 AnalysisFlow，將流程映射維持在 DataAnalysis 集中管理
        /// </summary>
        private AnalysisFlow CreateAnalysisFlow(frmMain.FlowStep cFlowStep, string sLogDirectoryPath, string sH5LogDirectoryPath, bool bGenerateH5Data, frmMain cfrmParent, string sProjectName)
        {
            switch (m_eStep)
            {
                case MainStep.FrequencyRank_Phase1:
                    return new AnalysisFlow_FRPH1(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);

                case MainStep.FrequencyRank_Phase2:
                    return new AnalysisFlow_FRPH2(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);

                case MainStep.AC_FrequencyRank:
                    return new AnalysisFlow_ACFR(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);

                case MainStep.Raw_ADC_Sweep:
                    return new AnalysisFlow_RawADCS(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);

                case MainStep.Self_FrequencySweep:
                    return new AnalysisFlow_SelfFS(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);

                case MainStep.Self_NCPNCNSweep:
                    return new AnalysisFlow_SelfPNS(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);

                default:
                    return null;
            }
        }

        /// <summary>
        /// 統一執行 AnalysisFlow 的 parameter load、特例前置注入與 MainFlow 呼叫，避免重複的型別轉型樣板碼
        /// </summary>
        private bool ExecuteAnalysisFlow(ref string sErrorMessage, string sSkipFreqSetFilePath)
        {
            if (m_cAnalysisFlowProcess == null)
            {
                sErrorMessage = string.Format("Unsupported Analysis Flow: {0}", m_eStep);
                return false;
            }

            m_cAnalysisFlowProcess.LoadAnalysisParameter();

            if (m_eStep == MainStep.FrequencyRank_Phase1)
                ((AnalysisFlow_FRPH1)m_cAnalysisFlowProcess).GetSkipFreqSetFilePath(sSkipFreqSetFilePath);

            return m_cAnalysisFlowProcess.MainFlow(ref sErrorMessage);
        }

        public bool ExecuteMainWorkFlow(
            ref string m_sErrorMessage, frmMain.FlowStep cFlowStep, string sLogDirectoryPath, string sH5LogDirectoryPath, bool bGenerateH5Data,
            frmMain cfrmParent, string sProjectName, string sSkipFreqSetFilePath)
        {
            if (m_sErrorMessage != "")
                return false;

            m_cfrmParent = cfrmParent;
            m_eStep = cFlowStep.m_eStep;

            OutputMessage("[State]Analysis Flow");

            m_cAnalysisFlowProcess = CreateAnalysisFlow(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);

            return ExecuteAnalysisFlow(ref m_sErrorMessage, sSkipFreqSetFilePath);
        }

        private void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.OutputMessage(sMessage, bWarning);
            });
               
            m_cfrmParent.OutputDebugLog(sMessage);
        }
    }
}
