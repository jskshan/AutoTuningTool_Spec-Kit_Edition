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

        public bool ExecuteMainWorkFlow(
            ref string m_sErrorMessage, frmMain.FlowStep cFlowStep, string sLogDirectoryPath, string sH5LogDirectoryPath, bool bGenerateH5Data,
            frmMain cfrmParent, string sProjectName, string sSkipFreqSetFilePath)
        {
            if (m_sErrorMessage != "")
                return false;

            m_cfrmParent = cfrmParent;
            m_eStep = cFlowStep.m_eStep;

            OutputMessage("[State]Analysis Flow");

            bool bFlowComplete = true;

            if (m_eStep == MainStep.FrequencyRank_Phase1)
                m_cAnalysisFlowProcess = new AnalysisFlow_FRPH1(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);
            else if (m_eStep == MainStep.FrequencyRank_Phase2)
                m_cAnalysisFlowProcess = new AnalysisFlow_FRPH2(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);
            else if (m_eStep == MainStep.AC_FrequencyRank)
                m_cAnalysisFlowProcess = new AnalysisFlow_ACFR(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);
            else if (m_eStep == MainStep.Raw_ADC_Sweep)
                m_cAnalysisFlowProcess = new AnalysisFlow_RawADCS(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);
            else if (m_eStep == MainStep.Self_FrequencySweep)
                m_cAnalysisFlowProcess = new AnalysisFlow_SelfFS(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);
            else if (m_eStep == MainStep.Self_NCPNCNSweep)
                m_cAnalysisFlowProcess = new AnalysisFlow_SelfPNS(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath, bGenerateH5Data, cfrmParent, sProjectName);

            m_cAnalysisFlowProcess.LoadAnalysisParameter();

            if (m_eStep == MainStep.FrequencyRank_Phase1)
            {
                AnalysisFlow_FRPH1 cFlowItem = (AnalysisFlow_FRPH1)m_cAnalysisFlowProcess;

                cFlowItem.LoadAnalysisParameter();

                cFlowItem.GetSkipFreqSetFilePath(sSkipFreqSetFilePath);

                bFlowComplete = cFlowItem.MainFlow(ref m_sErrorMessage);
            }
            else if (m_eStep == MainStep.FrequencyRank_Phase2)
            {
                AnalysisFlow_FRPH2 cFlowItem = (AnalysisFlow_FRPH2)m_cAnalysisFlowProcess;

                cFlowItem.LoadAnalysisParameter();

                bFlowComplete = cFlowItem.MainFlow(ref m_sErrorMessage);
            }
            else if (m_eStep == MainStep.AC_FrequencyRank)
            {
                AnalysisFlow_ACFR cFlowItem = (AnalysisFlow_ACFR)m_cAnalysisFlowProcess;

                cFlowItem.LoadAnalysisParameter();

                bFlowComplete = cFlowItem.MainFlow(ref m_sErrorMessage);
            }
            else if (m_eStep == MainStep.Raw_ADC_Sweep)
            {
                AnalysisFlow_RawADCS cFlowItem = (AnalysisFlow_RawADCS)m_cAnalysisFlowProcess;

                cFlowItem.LoadAnalysisParameter();

                bFlowComplete = cFlowItem.MainFlow(ref m_sErrorMessage);
            }
            else if (m_eStep == MainStep.Self_FrequencySweep)
            {
                AnalysisFlow_SelfFS cFlowItem = (AnalysisFlow_SelfFS)m_cAnalysisFlowProcess;

                cFlowItem.LoadAnalysisParameter();

                bFlowComplete = cFlowItem.MainFlow(ref m_sErrorMessage);
            }
            else if (m_eStep == MainStep.Self_NCPNCNSweep)
            {
                AnalysisFlow_SelfPNS cFlowItem = (AnalysisFlow_SelfPNS)m_cAnalysisFlowProcess;

                cFlowItem.LoadAnalysisParameter();

                bFlowComplete = cFlowItem.MainFlow(ref m_sErrorMessage);
            }

            return bFlowComplete;
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
