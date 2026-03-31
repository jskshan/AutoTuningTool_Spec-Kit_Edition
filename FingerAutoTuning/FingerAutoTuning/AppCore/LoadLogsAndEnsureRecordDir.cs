using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
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
        private bool CheckStepFolderExist(frmMain.FlowStep cFlowStep)
        {
            OutputMessage("[State]Check Step Folder Exist");

            string sStepDirectoryName = StringConvert.m_dictMainStepCodeNameMappingTable[cFlowStep.m_eStep];
            string sStepFolderPath = string.Format(@"{0}\{1}", m_cfrmParent.m_sDefaultFolderPath, sStepDirectoryName);

            if (Directory.Exists(sStepFolderPath) == true)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eStep"></param>
        private void ShowfrmFolderSelect(MainStep eStep)
        {
            OutputMessage("[State]Show Folder Select");

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                frmFolderSelect m_cfrmFolderSelect = new frmFolderSelect(m_cfrmParent);

                int nLocationX = (int)((m_cfrmParent.Left + m_cfrmParent.Right) / 2) - (int)(m_cfrmFolderSelect.Width / 2);
                int nLocationY = (int)((m_cfrmParent.Top + m_cfrmParent.Bottom) / 2) - (int)(m_cfrmFolderSelect.Height / 2);

                if (m_cfrmParent.IsMdiChild == true)
                {
                    nLocationX = (int)((m_cfrmParent.MdiParent.Left + m_cfrmParent.MdiParent.Right) / 2) - (int)(m_cfrmFolderSelect.Width / 2);
                    nLocationY = (int)((m_cfrmParent.MdiParent.Top + m_cfrmParent.MdiParent.Bottom) / 2) - (int)(m_cfrmFolderSelect.Height / 2);
                }

                if (m_cfrmParent.m_bParentFormFlag == true)
                {
                    nLocationX = (int)((m_cfrmParent.ParentForm.Left + m_cfrmParent.ParentForm.Right) / 2) - (int)(m_cfrmFolderSelect.Width / 2);
                    nLocationY = (int)((m_cfrmParent.ParentForm.Top + m_cfrmParent.ParentForm.Bottom) / 2) - (int)(m_cfrmFolderSelect.Height / 2);
                }

                m_cfrmFolderSelect.StartPosition = FormStartPosition.Manual;
                m_cfrmFolderSelect.Location = new Point(nLocationX, nLocationY);

                m_cfrmFolderSelect.FolderSelectLoad(eStep);

                if (m_cfrmFolderSelect.ShowDialog() == DialogResult.Cancel)
                    return;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <returns></returns>
        private bool CheckDefaultFolder(frmMain.FlowStep cFlowStep)
        {
            OutputMessage("[State]Check Default Folder");

            if (m_cfrmParent.m_sDefaultFolderPath == null)
            {
                m_sErrorMessage = "Load Folder Browser Error";
                return false;
            }
            else if (Directory.Exists(m_cfrmParent.m_sDefaultFolderPath) == false)
            {
                m_sErrorMessage = "Select Folder Path Not Exist";
                return false;
            }
            else
            {
                if (CheckStepFolderExist(cFlowStep) == false)
                {
                    m_sErrorMessage = string.Format("Folder Path Select Error[Step:{0}]", cFlowStep.m_sStepName);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sRecordTime"></param>
        /// <param name="bGenerateH5Data"></param>
        private void CreateRecordDirectory(string sRecordTime, bool bGenerateH5Data = false)
        {
            OutputMessage("[State]Create Record Directory");

            if (Directory.Exists(m_cfrmParent.m_sCurrentLogDirectoryPath) == false)
                Directory.CreateDirectory(m_cfrmParent.m_sCurrentLogDirectoryPath);

            if (m_bGetNameByFrequencyList == true)
            {
                if (m_sProjectName == "")
                    m_cfrmParent.m_sRecordLogDirectoryName = sRecordTime;
                else
                    m_cfrmParent.m_sRecordLogDirectoryName = string.Format("{0}_{1}", m_sProjectName, sRecordTime);
            }
            else
            {
                m_sProjectName = ParamFingerAutoTuning.m_sProjectName;

                if (ParamFingerAutoTuning.m_sProjectName == "")
                    m_cfrmParent.m_sRecordLogDirectoryName = sRecordTime;
                else
                    m_cfrmParent.m_sRecordLogDirectoryName = string.Format("{0}_{1}", ParamFingerAutoTuning.m_sProjectName, sRecordTime);
            }

            m_cfrmParent.m_sRecordLogDirectoryPath = string.Format(@"{0}\{1}", m_cfrmParent.m_sCurrentLogDirectoryPath, m_cfrmParent.m_sRecordLogDirectoryName);
            OutputMessage(string.Format("-Record Directory Name : {0}", m_cfrmParent.m_sRecordLogDirectoryName));

            if (Directory.Exists(m_cfrmParent.m_sRecordLogDirectoryPath) == false)
                Directory.CreateDirectory(m_cfrmParent.m_sRecordLogDirectoryPath);

            if (bGenerateH5Data == true)
            {
                if (Directory.Exists(m_cfrmParent.m_sH5DataLogDirectoryPath) == false)
                    Directory.CreateDirectory(m_cfrmParent.m_sH5DataLogDirectoryPath);

                m_cfrmParent.m_sH5RecordLogDirectoryName = string.Format("{0}_H5", m_cfrmParent.m_sRecordLogDirectoryName);
                m_cfrmParent.m_sH5RecordLogDirectoryPath = string.Format(@"{0}\{1}", m_cfrmParent.m_sH5DataLogDirectoryPath, m_cfrmParent.m_sH5RecordLogDirectoryName);

                OutputMessage(string.Format("-H5Data Record Directory Name : {0}", m_cfrmParent.m_sH5RecordLogDirectoryName));

                if (Directory.Exists(m_cfrmParent.m_sH5RecordLogDirectoryPath) == false)
                    Directory.CreateDirectory(m_cfrmParent.m_sH5RecordLogDirectoryPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <param name="bGenerateH5Data"></param>
        /// <returns></returns>
        private bool SetRecordDirectory(frmMain.FlowStep cFlowStep, bool bGenerateH5Data = false)
        {
            OutputMessage("[State]Set Record Directory");

            if (File.Exists(m_cfrmParent.m_sResultListFilePath) == false)
            {
                m_sErrorMessage = "ResultList.txt Not Exist";
                return false;
            }
            else
            {
                string sStepDirectoryPath = "";
                string sStepDirectoryName = "";

                IniFileFormat.GetStepDirectoryInfo(ref sStepDirectoryPath, ref sStepDirectoryName, m_cfrmParent, cFlowStep.m_eStep);

                if (Directory.Exists(sStepDirectoryPath) == false)
                {
                    m_sErrorMessage = string.Format("Step Directory : {0} Not Exist", sStepDirectoryName);
                    return false;
                }

                m_cfrmParent.m_sRecordLogDirectoryName = sStepDirectoryName;
                m_cfrmParent.m_sRecordLogDirectoryPath = string.Format(@"{0}\{1}", m_cfrmParent.m_sCurrentLogDirectoryPath, m_cfrmParent.m_sRecordLogDirectoryName);
                OutputMessage(string.Format("-Record Directory Name : {0}", m_cfrmParent.m_sRecordLogDirectoryName));

                if (bGenerateH5Data == true)
                {
                    if (Directory.Exists(m_cfrmParent.m_sH5DataLogDirectoryPath) == false)
                        Directory.CreateDirectory(m_cfrmParent.m_sH5DataLogDirectoryPath);

                    m_cfrmParent.m_sH5RecordLogDirectoryName = string.Format("{0}_H5", m_cfrmParent.m_sRecordLogDirectoryName);
                    m_cfrmParent.m_sH5RecordLogDirectoryPath = string.Format(@"{0}\{1}", m_cfrmParent.m_sH5DataLogDirectoryPath, m_cfrmParent.m_sH5RecordLogDirectoryName);

                    OutputMessage(string.Format("-H5Data Record Directory Name : {0}", m_cfrmParent.m_sH5RecordLogDirectoryName));

                    if (Directory.Exists(m_cfrmParent.m_sH5RecordLogDirectoryPath) == false)
                    {
                        m_sErrorMessage = string.Format("Step HDF5 Directory : {0} Not Exist", sStepDirectoryName);
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        private void SetLogDirectoryPath(frmMain.FlowStep cFlowStep)
        {
            OutputMessage("[State]Log Directory Path Set");

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

            m_cfrmParent.m_sLoadDataPath = string.Format(@"{0}\{1}", m_cfrmParent.m_sDefaultFolderPath, sStepCodeName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <returns></returns>
        private bool CopyLogData(frmMain.FlowStep cFlowStep)
        {
            OutputMessage("[State]Copy Log Data");

            m_bCopyDataError = false;

            List<string> sDataType_List = new List<string>();

            switch (cFlowStep.m_eStep)
            {
                case MainStep.FrequencyRank_Phase1:
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_ADC);
                    break;
                case MainStep.FrequencyRank_Phase2:
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_ADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_BASE);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_BASE);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_BASEMinusADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_BASEMinusADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RecentADCMEANMinusADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_DV);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_DV);
                    break;
                case MainStep.AC_FrequencyRank:
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_ADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_BASE);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_BASE);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_BASEMinusADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_BASEMinusADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_OBASE);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_OBASE);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_OBASEMinusADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_OBASEMinusADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_PREREPORT);
                    break;
                case MainStep.Raw_ADC_Sweep:
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_ADC);
                    break;
                case MainStep.Self_FrequencySweep:
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_DV);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_DV);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_REPORTMODE);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL);
                    break;
                case MainStep.Self_NCPNCNSweep:
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RawData);
                    sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_RawData);
                    break;
                default:
                    break;
            }

            foreach (string sDataType in sDataType_List)
            {
                string sSourceDirectoryPath = string.Format(@"{0}\{1}", m_cfrmParent.m_sLoadDataPath, sDataType);

                if (Directory.Exists(sSourceDirectoryPath) == false)
                    continue;

                string sTargetDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDataType);

                if (Directory.Exists(sTargetDirectoryPath) == false)
                    Directory.CreateDirectory(sTargetDirectoryPath);

                if (m_bGenerateH5Data == true)
                {
                    string sTargetH5DirPath = string.Format(@"{0}\{1}", m_sH5LogDirectoryPath, sDataType);

                    if (Directory.Exists(sTargetH5DirPath) == false)
                        Directory.CreateDirectory(sTargetH5DirPath);
                }

                string sFileType = "*.csv";

                if (sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE ||
                    sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL)
                    sFileType = "*.txt";

                int nFileCount = Directory.GetFiles(sSourceDirectoryPath, sFileType).Count();
                string sProgressMessage = string.Format("{0} File Count : {1}", sDataType, nFileCount);
                int nFileIndex = 0;

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.InitialstatusstripMessage(nFileCount, sProgressMessage);
                });

                foreach (string sFilePath in Directory.GetFiles(sSourceDirectoryPath, sFileType))
                {
                    string sSourceFilePath = sFilePath;
                    string sTargetFilePath = string.Format(@"{0}\{1}\{2}", m_sLogDirectoryPath, sDataType, Path.GetFileName(sFilePath));

                    try
                    {
                        File.Copy(sSourceFilePath, sTargetFilePath, true);
                    }
                    catch
                    {
                        m_sErrorMessage = "Copy Log File Error";
                        return false;
                    }

                    while (File.Exists(sTargetFilePath) == false)
                        Thread.Sleep(10);

                    //執行Python所寫"HDF5ConvertTool"將CSV Raw Data檔轉換為H5檔
                    if (m_bGenerateH5Data == true)
                    {
                        string sCsvFilePath = sTargetFilePath;
                        string sH5DirectoryPath = string.Format(@"{0}\{1}", m_sH5LogDirectoryPath, sDataType);

                        if (ConvertHDF5Data(sCsvFilePath, sH5DirectoryPath) == false)
                            return false;
                    }

                    m_cDebugLog.WriteLogToBuffer("Copy File : " + Path.GetFileName(sSourceFilePath));

                    sProgressMessage = string.Format("Copy {0} File : {1}/{2}", sDataType, nFileIndex, nFileCount);

                    nFileIndex++;

                    m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                    {
                        m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, nFileIndex, sProgressMessage);
                    });

                    if (m_cfrmParent.m_bExecute == false)
                        return false;
                }
            }

            return true;
        }
    }
}
