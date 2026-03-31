using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Drawing;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class AnalysisFlow_Else : AnalysisFlow
    {
        public AnalysisFlow_Else(FlowStep cFlowStep, frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
            m_sFileDirectoryPath = m_cfrmMain.m_sFileDirectoryPath;
            m_sRecordProjectName = m_cfrmMain.m_sRecordProjectName;

            InitializeParameter(cFlowStep);
            CreateErrorInfo();
        }

        private void ClearDataArray()
        {
        }

        public override void LoadAnalysisParameter()
        {
            m_nReportDataLength = ParamAutoTuning.m_nReportDataLength;
            m_nShiftStartByte = ParamAutoTuning.m_nShiftStartByte;
            m_nShiftByteNumber = ParamAutoTuning.m_nShiftByteNumber;

            //m_nEdgeTraceNumber = ParamAutoTuning.m_nEdgeTraceNumber;
            m_nPartNumber = ParamAutoTuning.m_nPartNumber;
        }

        public void SetFileDirectory()
        {
            m_sStepFolderPath = string.Format(@"{0}\{1}", m_sFileDirectoryPath, m_sSubStepName);
            m_sResultFolderPath = string.Format(@"{0}\{1}({2})", m_sFileDirectoryPath, SpecificText.m_sResultText, m_sSubStepCodeName);
            m_sFlowBackUpFolderPath = string.Format(@"{0}\{1}", m_sFileDirectoryPath, SpecificText.m_sFlowText);
            m_sProjectName = m_sRecordProjectName;
            m_sProjectFolderPath = m_sFileDirectoryPath;
            m_sStepListPath = string.Format(@"{0}\{1}_{2}.csv", m_sProjectFolderPath, SpecificText.m_sStepListText, m_sSubStepCodeName);
        }

        public void GetData(List<RecordParameter> cParameter_List)
        {
            // 取得資料夾內所有檔案
            string[] sValidReportDataFile_Array = GetValidReportDataFile(cParameter_List);

            if (sValidReportDataFile_Array == null || sValidReportDataFile_Array.Length == 0)
            {
                m_sErrorMessage = "No Valid Data!!";
                OutputMessage("No Valid Data!!");
                m_bErrorFlag = true;
                return;
            }

            int nFileCount = sValidReportDataFile_Array.Length;

            OutputMainStatusStrip("Analysing...", 0, nFileCount, frmMain.m_nInitialFlag);

            foreach (string sFilePath in sValidReportDataFile_Array)
            {
                string sMessage = "";

                ClearDataArray();

                #region Get Report Data
                DataInfo cDataInfo = new DataInfo();
                m_cDataInfo_List.Add(cDataInfo);
                int nFileIndex = m_cDataInfo_List.Count - 1;
                cDataInfo = m_cDataInfo_List[nFileIndex];

                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);

                //Folder
                m_sIntegrationFolderPath = string.Format(@"{0}\{1}\Integration Data", m_sResultFolderPath, sFileName);
                Directory.CreateDirectory(m_sIntegrationFolderPath);

                //File
                m_sIntegrationFilePath = string.Format(@"{0}\Integration Data.csv", m_sIntegrationFolderPath);

                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                GetFileInfoFromReportData(cDataInfo, sFilePath);

                if (CheckInfoIsCorrect(ref sMessage, cDataInfo) == false)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0001;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", sMessage, Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = sMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                if ((m_cfrmMain.m_nSkipPreviousStepFlag & MainConstantParameter.m_nSKIPFILE_FLOWTXT) != 0)
                {
                    if (CheckDataIsInconsistent(nFileIndex) == false)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0002;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("Parameter Identical with Other Report Data In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = "Parameter Identical with Other Report Data";
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    }

                    if (m_bReadReportDataErrorFlag == true)
                        continue;
                }
                else
                {
                    if (CheckParameterAndFlowInfoIsMatch(cParameter_List, cDataInfo) == false)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0004;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("Parameter and Flow Info List Not Match In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = string.Format("Parameter and Flow Info List Not Match");
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    }

                    if (m_bReadReportDataErrorFlag == true)
                        continue;

                    if (CheckDataIsInconsistent(nFileIndex) == false)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0002;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("Parameter Identical with Other Report Data In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = "Parameter Identical with Other Report Data";
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    }

                    if (m_bReadReportDataErrorFlag == true)
                        continue;
                }
                #endregion

                if (cDataInfo.m_sRecordErrorCode != "")
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0008;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", cDataInfo.m_sRecordErrorMessage, Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = cDataInfo.m_sRecordErrorMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                m_nValidFileCount++;
                m_nFileCount++;

                OutputMessage(string.Format("Report Data({0}) Analysis Complete!!", Path.GetFileName(sFilePath)));
                OutputMainStatusStrip(string.Format("Data Set : {0}", m_nFileCount), m_nFileCount);
            }

            if (m_nValidFileCount < 1)
            {
                m_sErrorMessage = "No Enough Data. Data Analysis Error!!";
                OutputMessage("No Enough Data. Data Analysis Error!!");
                m_bErrorFlag = true;
                return;
            }
            else if (ParamAutoTuning.m_nFlowMethodType != 1 && m_nValidFileCount < m_nFileCount)
            {
                m_sErrorMessage = "Some Data Analysis Error!!";
                OutputMessage("Some Data Analysis Error!!");
                m_bErrorFlag = true;
                return;
            }
        }

        public void ComputeAndOutputResult()
        {
            OutputResultData();
        }

        protected void OutputResultData()
        {
            WriteStepListDataFile(m_sProjectName, m_sStepListPath);

            if (m_bErrorFlag == false)
            {
                FileProcess cFileProcess = new FileProcess(m_cfrmMain);
                cFileProcess.WriteResultListTxtFile(m_eSubStep, m_nSubStepState);
            }

            OutputMessage("Analysis Complete!!");
        }
    }
}
