using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Elan;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <returns></returns>
        private bool MovePreviousData(frmMain.FlowStep cFlowStep)
        {
            OutputMessage("[State]Move Previous Data");

            if (File.Exists(m_cfrmParent.m_sResultListFilePath) == true)
            {
                string sStepDirectoryPath = "";
                string sStepDirectoryName = "";

                List<string> sDataType_List = new List<string>();

                switch (cFlowStep.m_eStep)
                {
                    case MainStep.FrequencyRank_Phase1:
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_ADC);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_CHART);
                        break;
                    case MainStep.FrequencyRank_Phase2:
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_ADC);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_CHART);

                        string[] sDataType_Array = new string[]
                        {
                            MainConstantParameter.m_sDATATYPE_BASE,
                            MainConstantParameter.m_sDATATYPE_RAW_BASE,
                            MainConstantParameter.m_sDATATYPE_DV,
                            MainConstantParameter.m_sDATATYPE_RAW_DV,
                            MainConstantParameter.m_sDATATYPE_BASEMinusADC,
                            MainConstantParameter.m_sDATATYPE_RAW_BASEMinusADC,
                            MainConstantParameter.m_sDATATYPE_RecentADCMEANMinusADC,
                            MainConstantParameter.m_sDATATYPE_ANALYSIS
                        };

                        foreach (string sDataType in sDataType_Array)
                            sDataType_List.Add(sDataType);
                        break;
                    case MainStep.AC_FrequencyRank:
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_ADC);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_CHART);

                        sDataType_Array = new string[]
                        {
                            MainConstantParameter.m_sDATATYPE_BASE,
                            MainConstantParameter.m_sDATATYPE_RAW_BASE,
                            MainConstantParameter.m_sDATATYPE_BASEMinusADC,
                            MainConstantParameter.m_sDATATYPE_RAW_BASEMinusADC,
                            MainConstantParameter.m_sDATATYPE_OBASE,
                            MainConstantParameter.m_sDATATYPE_RAW_OBASE,
                            MainConstantParameter.m_sDATATYPE_OBASEMinusADC,
                            MainConstantParameter.m_sDATATYPE_RAW_OBASEMinusADC,
                            MainConstantParameter.m_sDATATYPE_PREREPORT,
                            MainConstantParameter.m_sDATATYPE_ANALYSIS
                        };

                        foreach (string sDataType in sDataType_Array)
                            sDataType_List.Add(sDataType);
                        break;
                    case MainStep.Raw_ADC_Sweep:
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_ADC);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_CHART);
                        break;
                    case MainStep.Self_FrequencySweep:
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_DV);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_DV);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_CHART);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_ANALYSIS);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_REPORTMODE);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL);
                        break;
                    case MainStep.Self_NCPNCNSweep:
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RawData);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_RAW_RawData);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_CHART);
                        sDataType_List.Add(MainConstantParameter.m_sDATATYPE_ANALYSIS);
                        break;
                    default:
                        break;
                }

                IniFileFormat.GetStepDirectoryInfo(ref sStepDirectoryPath, ref sStepDirectoryName, m_cfrmParent, cFlowStep.m_eStep);

                if (Directory.Exists(sStepDirectoryPath) == false)
                    return true;

                string sBackUpDirectoryPath = string.Format(@"{0}\BackUp\{1}", m_cfrmParent.m_sLogDirectoryPath, sStepDirectoryName);

                if (Directory.Exists(sBackUpDirectoryPath) == false)
                    Directory.CreateDirectory(sBackUpDirectoryPath);

                while (Directory.Exists(sBackUpDirectoryPath) == false)
                    Thread.Sleep(10);

                if (MoveOrCopyFile(sStepDirectoryPath, sBackUpDirectoryPath, "*.txt", "Result", false, false) == false)
                    return false;

                string sStepName = StringConvert.m_dictMainStepMappingTable[cFlowStep.m_eStep];
                string sStepCodeName = StringConvert.m_dictMainStepCodeNameMappingTable[cFlowStep.m_eStep];
                string sMoveDirectoryPath = string.Format(@"{0}\{1}", sStepDirectoryPath, sStepCodeName);

                if (Directory.Exists(sMoveDirectoryPath) == true)
                {
                    string sDestinationDirectoryPath = string.Format(@"{0}\{1}", sBackUpDirectoryPath, sStepCodeName);

                    if (Directory.Exists(sDestinationDirectoryPath) == false)
                        Directory.CreateDirectory(sDestinationDirectoryPath);

                    while (Directory.Exists(sDestinationDirectoryPath) == false)
                        Thread.Sleep(10);

                    #region Mark It.
                    /*
                    if (Directory.Exists(sDestDirPath) == true)
                    {
                        sProgressStr = "Delete BackUp Unnecessary Directory...";

                        m_frmParent.BeginInvoke((MethodInvoker)delegate
                        {
                            m_frmParent.MsgMenuStripInit(0, sProgressStr);
                        });

                        Directory.Delete(sDestDirPath, true);
                        while (Directory.Exists(sDestDirPath))
                        {
                            if (m_frmParent.m_bExecute == false)
                                return false;

                            Thread.Sleep(10);
                        }
                    }
                    */
                    #endregion

                    if (MoveOrCopyFile(sMoveDirectoryPath, sDestinationDirectoryPath, "*.csv", "Base", false) == false)
                        return false;

                    foreach (string sDataType in sDataType_List)
                    {
                        string sFileType = "*.csv";

                        switch (sDataType)
                        {
                            case MainConstantParameter.m_sDATATYPE_CHART:
                                sFileType = "*.jpg";
                                break;
                            case MainConstantParameter.m_sDATATYPE_RAW_ADC:
                            case MainConstantParameter.m_sDATATYPE_RAW_BASE:
                            case MainConstantParameter.m_sDATATYPE_RAW_BASEMinusADC:
                            case MainConstantParameter.m_sDATATYPE_RAW_DV:
                            case MainConstantParameter.m_sDATATYPE_RAW_OBASE:
                            case MainConstantParameter.m_sDATATYPE_RAW_OBASEMinusADC:
                                sFileType = "*.CSV";
                                break;
                            case MainConstantParameter.m_sDATATYPE_REPORTMODE:
                            case MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL:
                                sFileType = "*.txt";
                                break;
                            default:
                                sFileType = "*.csv";
                                break;
                        }

                        string sMoveSubDirectoryPath = string.Format(@"{0}\{1}", sMoveDirectoryPath, sDataType);

                        if (Directory.Exists(sMoveSubDirectoryPath) == false)
                            continue;

                        string sDestinationSubDirectoryPath = string.Format(@"{0}\{1}", sDestinationDirectoryPath, sDataType);

                        if (Directory.Exists(sDestinationSubDirectoryPath) == false)
                            Directory.CreateDirectory(sDestinationSubDirectoryPath);

                        while (Directory.Exists(sDestinationSubDirectoryPath) == false)
                            Thread.Sleep(10);

                        if ((cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep) &&
                            sDataType == MainConstantParameter.m_sDATATYPE_CHART)
                        {
                            string[] sDirectoryPath_Array = Directory.GetDirectories(sMoveSubDirectoryPath, "*", SearchOption.TopDirectoryOnly);

                            if (MoveFolder(sDirectoryPath_Array, sDestinationSubDirectoryPath, sDataType) == false)
                                return false;
                        }
                        else
                        {
                            if (MoveOrCopyFile(sMoveSubDirectoryPath, sDestinationSubDirectoryPath, sFileType, sDataType, true) == false)
                                return false;

                            Directory.Delete(sMoveSubDirectoryPath, true);

                            while (Directory.Exists(sMoveSubDirectoryPath))
                                Thread.Sleep(10);
                        }
                    }

                    Directory.Delete(sMoveDirectoryPath, true);

                    while (Directory.Exists(sMoveDirectoryPath))
                        Thread.Sleep(10);
                }

                if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                {
                    sMoveDirectoryPath = string.Format(@"{0}\{1}", sStepDirectoryPath, sStepCodeName);

                    if (Directory.Exists(sMoveDirectoryPath) == true && Directory.GetDirectories(sMoveDirectoryPath).Length == 0)
                    {
                        Directory.Delete(sMoveDirectoryPath, true);

                        while (Directory.Exists(sMoveDirectoryPath))
                            Thread.Sleep(10);
                    }
                }

                if (Directory.Exists(sStepDirectoryPath) == true && Directory.GetDirectories(sStepDirectoryPath).Length == 0)
                {
                    Directory.Delete(sStepDirectoryPath, true);

                    while (Directory.Exists(sStepDirectoryPath))
                        Thread.Sleep(10);
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sSourceDirectoryPath"></param>
        /// <param name="sDestinationDirectoryPath"></param>
        /// <param name="sFileType"></param>
        /// <param name="sDirectoryType"></param>
        /// <param name="bStepDirectory"></param>
        /// <param name="bMoveMotion"></param>
        /// <returns></returns>
        private bool MoveOrCopyFile(
            string sSourceDirectoryPath, string sDestinationDirectoryPath, string sFileType, string sDirectoryType, bool bStepDirectory,
            bool bMoveMotion = true)
        {
            int nFileCount = Directory.GetFiles(sSourceDirectoryPath, sFileType).Count();
            string sProgressMessage = string.Format("{0} File Count : {1}", sDirectoryType, nFileCount);

            if (nFileCount == 0)
                return true;

            int nFileIndex = 0;

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.InitialstatusstripMessage(nFileCount, sProgressMessage);
            });

            foreach (string sFilePath in Directory.GetFiles(sSourceDirectoryPath, sFileType))
            {
                string sSourceFilePath = sFilePath;
                string sTargetFilePath = string.Format(@"{0}\{1}", sDestinationDirectoryPath, Path.GetFileName(sFilePath));

                if (File.Exists(sTargetFilePath) == true)
                    AppCoreDefine.FileProcessFlow(AppCoreDefine.FileProcess.Delete, "", sTargetFilePath);

                bool bCompleteFlag = false;

                if (bMoveMotion == true)
                    bCompleteFlag = AppCoreDefine.FileProcessFlow(AppCoreDefine.FileProcess.Move, sSourceFilePath, sTargetFilePath);
                else
                    bCompleteFlag = AppCoreDefine.FileProcessFlow(AppCoreDefine.FileProcess.Copy, sSourceFilePath, sTargetFilePath);

                if (bCompleteFlag == false)
                {
                    if (bStepDirectory == true)
                        m_sErrorMessage = string.Format("Move Previous Data Error[Step:{0}]", sDirectoryType);
                    else
                        m_sErrorMessage = string.Format("Move Previous Data Error[{0} File]", sDirectoryType);

                    return false;
                }

                sProgressMessage = string.Format("Move {0} File : {1}/{2}", sDirectoryType, nFileIndex, nFileCount);

                nFileIndex++;

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, nFileIndex, sProgressMessage);
                });

                if (m_cfrmParent.m_bExecute == false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sDirectoryPath_Array"></param>
        /// <param name="sDestinationDirectoryPath"></param>
        /// <param name="sDirectoryType"></param>
        /// <returns></returns>
        private bool MoveFolder(string[] sDirectoryPath_Array, string sDestinationDirectoryPath, string sDirectoryType)
        {
            int nFolderCount = sDirectoryPath_Array.Length;

            string sProgressMessage = string.Format("{0} Folder Count : {1}", sDirectoryType, nFolderCount);

            if (nFolderCount == 0)
                return true;

            int nFolderIndex = 0;

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.InitialstatusstripMessage(nFolderCount, sProgressMessage);
            });

            foreach (string sDirectoryPath in sDirectoryPath_Array)
            {
                string sFolderName = Path.GetFileName(sDirectoryPath);
                string sDestinationPath = string.Format(@"{0}\{1}", sDestinationDirectoryPath, sFolderName);

                bool bComplete = false;

                try
                {
                    Directory.Move(sDirectoryPath, sDestinationPath);
                    bComplete = true;
                }
                catch
                {
                    MessageBox.Show(string.Format("Move Folder Error({0})", sDirectoryPath));
                }

                if (bComplete == false)
                {
                    try
                    {
                        Directory.Move(sDirectoryPath, sDestinationPath);
                    }
                    catch
                    {
                        m_sErrorMessage = "Move Previous Data Error[Chart Directory]";
                        return false;
                    }
                }

                nFolderCount++;

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, nFolderIndex, sProgressMessage);
                });

                if (m_cfrmParent.m_bExecute == false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void MoveUnnecessaryDirectory()
        {
            string[] sCurrentDirectoryPath_Array = Directory.GetDirectories(m_cfrmParent.m_sCurrentLogDirectoryPath);

            foreach (string sDirectoryPath in sCurrentDirectoryPath_Array)
            {
                string sDirectoryName = Path.GetFileName(sDirectoryPath);

                if (sDirectoryName == m_cfrmParent.m_sRecordLogDirectoryName)
                    continue;

                if (Directory.GetDirectories(sDirectoryPath).Length == 0)
                {
                    string sBackUpDirectoryPath = string.Format(@"{0}\BackUp\{1}", m_cfrmParent.m_sLogDirectoryPath, sDirectoryName);

                    if (Directory.Exists(sBackUpDirectoryPath) == true)
                    {
                        Directory.Delete(sBackUpDirectoryPath, true);

                        while (Directory.Exists(sBackUpDirectoryPath))
                            Thread.Sleep(10);
                    }

                    try
                    {
                        Directory.Move(sDirectoryPath, sBackUpDirectoryPath);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eCurrentStep"></param>
        private void WriteResultListTxt(MainStep eCurrentStep)
        {
            OutputMessage("[State]Write ResultList Txt File");

            foreach (MainStep eStep in IniFileFormat.eStep_Array)
            {
                string sStepName = StringConvert.m_dictMainStepMappingTable[eStep];

                if (eCurrentStep == eStep)
                    IniFileFormat.WriteValue("Step Directory", sStepName, m_cfrmParent.m_sRecordLogDirectoryName, m_cfrmParent.m_sResultListFilePath, true, false);
                else
                {
                    if (IniFileFormat.CheckParameterExist("Step Directory", sStepName, m_cfrmParent.m_sResultListFilePath) == false)
                        IniFileFormat.WriteValue("Step Directory", sStepName, "", m_cfrmParent.m_sResultListFilePath, true, false);
                }
            }
        }
    }
}
