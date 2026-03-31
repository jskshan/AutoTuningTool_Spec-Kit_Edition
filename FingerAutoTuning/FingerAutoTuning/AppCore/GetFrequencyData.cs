using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        /// <summary>
        /// 從頻率列表檔案中讀取專案名稱(Name)、命令腳本檔案(CommandFile)及Self Frequency Sweep的報告取得序列(GetReportSequence)等資訊
        /// </summary>
        /// <param name="cFlowStep">當前執行的流程步驟,用於判斷要讀取的檔案路徑(Mutual/Self頻率或Raw ADC Sweep)</param>
        /// <returns>固定回傳true(即使檔案不存在或發生錯誤)</returns>
        private bool GetFreqeuncyItemListInfo(frmMain.FlowStep cFlowStep)
        {
            OutputMessage("[State]Get Frequency List Info");

            string sFilePath = "";

            if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase1 ||
                cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2 ||
                cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                sFilePath = m_sMutualFrequencyPath;
            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                sFilePath = m_sRawADCSweepPath;
            else if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                     cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                sFilePath = m_sSelfFrequencyPath;

            if (File.Exists(sFilePath) == true)
            {
                string sLine = "";

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplit_Array = sLine.Split('=');

                        if (sSplit_Array.Length >= 2)
                        {
                            if (sSplit_Array[0].Trim() == "Name")
                            {
                                for (int nSplitIndex = 1; nSplitIndex < sSplit_Array.Length; nSplitIndex++)
                                {
                                    if (nSplitIndex == 1)
                                        m_sProjectName = sSplit_Array[nSplitIndex].TrimStart();
                                    else
                                        m_sProjectName = string.Format("{0}={1}", m_sProjectName, sSplit_Array[nSplitIndex]);
                                }

                                m_bGetNameByFrequencyList = true;
                            }
                            else if (sSplit_Array[0].Trim() == "CommandFile")
                            {
                                string sFileName = "";

                                for (int nSplitIndex = 1; nSplitIndex < sSplit_Array.Length; nSplitIndex++)
                                {
                                    if (nSplitIndex == 1)
                                        sFileName = sSplit_Array[nSplitIndex].TrimStart();
                                    else
                                        sFileName = string.Format("{0}={1}", sFileName, sSplit_Array[nSplitIndex]);
                                }

                                m_sCommandScriptFilePath = string.Format("{0}\\{1}", m_cfrmParent.m_sCmdDirectoryPath, sFileName);
                                m_bGetCommandScriptFile = true;
                            }

                            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep && ParamFingerAutoTuning.m_nSelfFSGetDataType == 1 &&
                                ParamFingerAutoTuning.m_nSelfFSGetReportType == 1)
                            {
                                if (sSplit_Array[0].Trim() == "GetReportSequence")
                                {
                                    for (int nSplitIndex = 1; nSplitIndex < sSplit_Array.Length; nSplitIndex++)
                                    {
                                        if (sSplit_Array.Length > 2)
                                            m_sErrorMessage = string.Format("Get ReportSequence Length Error");
                                        else
                                        {
                                            if (ElanConvert.IsInt(sSplit_Array[1]) == false)
                                                m_sErrorMessage = string.Format("Get ReportSequence Format Error");
                                            else
                                                m_nSelfGetReportSequence = Convert.ToInt32(sSplit_Array[1]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                            break;
                    }
                }
                finally
                {
                    srFile.Close();
                }
            }

            return true;
        }

        /// <summary>
        /// 在AC_FrequencyRank模式下取得並儲存前一步驟(FrequencyRank_Phase2)的資料,驗證前置步驟記錄檔是否存在並複製報告檔案至當前目錄
        /// </summary>
        /// <param name="cFlowStep">當前執行的流程步驟,用於判斷是否需要執行此流程</param>
        /// <returns>非AC_FrequencyRank模式或所有驗證及複製成功回傳true;ResultList檔案不存在、前置步驟記錄檔不存在或報告檔案不存在則回傳false並設定錯誤訊息</returns>
        private bool GetAndSavePreviousData(frmMain.FlowStep cFlowStep)
        {
            if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank && ParamFingerAutoTuning.m_nACFRModeType != 1)
            {
                OutputMessage("[State]Get and Save Previous Data Flow");

                string sStepDirectoryPath = "";
                string sStepDirectoryName = "";
                string sPreviousStepName = StringConvert.m_dictMainStepMappingTable[MainStep.FrequencyRank_Phase2];
                string sPreviousStepCodeName = StringConvert.m_dictMainStepCodeNameMappingTable[MainStep.FrequencyRank_Phase2];

                if (File.Exists(m_cfrmParent.m_sResultListFilePath) == true)
                {
                    IniFileFormat.GetStepDirectoryInfo(ref sStepDirectoryPath, ref sStepDirectoryName, m_cfrmParent, MainStep.FrequencyRank_Phase2);

                    string sPreviousStepPath = string.Format(@"{0}\{1}", sStepDirectoryPath, sPreviousStepCodeName);

                    if (Directory.Exists(sPreviousStepPath) == false)
                    {
                        m_sErrorMessage = string.Format("\"{0}\" Log Not Exist. Please Run \"{1}\" Step First", sPreviousStepCodeName, sPreviousStepName);
                        return false;
                    }
                }
                else
                {
                    m_sErrorMessage = string.Format("ResultList File Not Exist. Please Run \"{0}\" Step First", sPreviousStepName);
                    return false;
                }

                #region Mark It.
                /*
                string sPreviousStepBASEPath = string.Format(@"{0}\{1}\{2}", sStepFileDirPath, sPreviousStepCodeName, MainConstantParameter.DATATYPE_BASE);

                if (Directory.Exists(sPreviousStepBASEPath) == false)
                {
                    m_sErrorMessage = string.Format("\"{0}\" Directory Not Exist. Please Run \"{1}\" Step First", MainConstantParameter.DATATYPE_BASE, sPreviousStepName);
                    return false;
                }

                string sPreviousStepBASEMinusADCPath = string.Format(@"{0}\{1}\{2}", sStepFileDirPath, sPreviousStepCodeName, MainConstantParameter.DATATYPE_BASEMinusADC);

                if (Directory.Exists(sPreStepBASEMinusADCPath) == false)
                {
                    m_sErrorMessage = string.Format("\"{0}\" Directory Not Exist. Please Run \"{1}\" Step First", MainConstantParameter.DATATYPE_BASEMinusADC, sPreviousStepName);
                    return false;
                }
                */
                #endregion

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

                string sPreviousStepReportPath = string.Format(@"{0}\{1}\Report.csv", sStepDirectoryPath, sPreviousStepCodeName);

                if (File.Exists(sPreviousStepReportPath) == true)
                {
                    CopyPreviousReportFile(sPreviousStepReportPath, m_sLogDirectoryPath);

                    if (m_bGenerateH5Data == true)
                        CopyPreviousReportFile(sPreviousStepReportPath, m_sH5LogDirectoryPath);
                }
                else
                {
                    m_sErrorMessage = string.Format("\"{0}\" Report File Not Exist. Please Run \"{1}\" Step First", sPreviousStepName, sPreviousStepName);
                    return false;
                }

                #region Mark It.
                /*
                bool[] bGetData_Array = new bool[LFreqItemParam.Count];
                Array.Clear(bGetData_Array, 0, bGetData_Array.Length);

                foreach (string sFilePath in System.IO.Directory.GetFiles(sPreviousStepBASEPath, "*.csv"))
                {
                    int nTXTraceNumber = -1;
                    int nRXTraceNumber = -1;
                    int nReadPH1 = -1;
                    int nReadPH2 = -1;
                    string sLine = "";
                    StreamReader srReadFile = new StreamReader(sFilePath, Encoding.Default);

                    try
                    {
                        while ((sLine = srReadFile.ReadLine()) != null)
                        {
                            string[] sSubString_Array = sLine.Split(',');

                            if (sSubString_Array.Length >= 2)
                            {
                                if (sSubString_Array[0] == "TXTraceNumber")
                                    Int32.TryParse(sSubString_Array[1], out nTXTraceNumber);
                                else if (SubStringArray[0] == "RXTraceNumber")
                                    Int32.TryParse(sSubString_Array[1], out nRXTraceNumber);
                                else if (sSubString_Array[0] == "ReadPH1(Hex)")
                                    nReadPH1 = Convert.ToInt32(sSubString_Array[1], 16);
                                else if (sSubString_Array[0] == "ReadPH2(Hex)")
                                    nReadPH2 = Convert.ToInt32(sSubString_Array[1], 16);
                            }

                            if (nTXTraceNumber > -1 && nRXTraceNumber > -1 && 
                                nReadPH1 > -1 && nReadPH2 > -1)
                                break;
                        }
                    }
                    finally
                    {
                        srReadFile.Close();
                    }

                    for (int nIndex = 0; nIndex < LFreqItemParam.Count; nIndex++)
                    {
                        if (bGetData_Array[nIndex] == false)
                        {
                            FrequencyItemParam cCurItem = LFreqItemParam[nIndex];

                            if (nReadPH1 == cCurItem.m_nPH1 && nReadPH2 == cCurItem.m_nPH2 &&
                                nTXTraceNumber == cCurItem.m_nTXTraceNumber && nRXTraceNumber == cCurItem.m_nRXTraceNumber)
                            {
                                string sPBASEDirPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.DATATYPE_PBASE);

                                if (Directory.Exists(sPBASEDirPath) == false)
                                    Directory.CreateDirectory(sPBASEDirPath);

                                string sTargetFilePath = string.Format(@"{0}\{1}", sPBASEDirPath, Path.GetFileName(sFilePath));

                                File.Copy(sFilePath, sTargetFilePath);

                                LFreqItemParam[nIndex].m_sPBASEFilePath = sTargetFilePath;

                                bGetData_Array[nIndex] = true;
                                break;
                            }
                        }
                    }
                }

                for (int nIndex = 0; nIndex < bGetData_Array.Length; nIndex++)
                {
                    if (bGetData_Array[nIndex] == false)
                    {
                        m_sErrorMessage = string.Format("Get PreStep {0} File Error(RankIndex={1})", MainConstantParameter.DATATYPE_BASE, nIndex + 1);
                        return false;
                    }
                }

                bGetData_Array = new bool[LFreqItemParam.Count];
                Array.Clear(bGetData_Array, 0, bGetData_Array.Length);

                foreach (string sFilePath in System.IO.Directory.GetFiles(sPreviousStepBASEMinusADCPath, "*.csv"))
                {
                    int nTXTraceNumber = -1;
                    int nRXTraceNumber = -1;
                    int nReadPH1 = -1;
                    int nReadPH2 = -1;
                    string sLine = "";
                    string sCopyLine = "";
                    bool bCopyFlag = false;
                    StreamReader srReadFile = new StreamReader(sFilePath, Encoding.Default);

                    try
                    {
                        while ((sLine = srReadFile.ReadLine()) != null)
                        {
                            string[] sSubString_Array = sLine.Split(',');

                            if (sSubString_Array.Length >= 2)
                            {
                                if (sSubString_Array[0] == "TXTraceNumber")
                                    Int32.TryParse(sSubString_Array[1], out nTXTraceNumber);
                                else if (sSubString_Array[0] == "RXTraceNumber")
                                    Int32.TryParse(sSubString_Array[1], out nRXTraceNumber);
                                else if (sSubString_Array[0] == "ReadPH1(Hex)")
                                    nReadPH1 = Convert.ToInt32(sSubString_Array[1], 16);
                                else if (sSubString_Array[0] == "ReadPH2(Hex)")
                                    nReadPH2 = Convert.ToInt32(sSubString_Array[1], 16);

                                if (sSubString_Array[0] == "TXTraceNumber")
                                {
                                    sCopyLine += sLine + Environment.NewLine;
                                    bCopyFlag = true;
                                }
                                else if (sSubString_Array[0] == "ReadFWIP_Option(Hex)")
                                {
                                    sCopyLine += sLine;
                                    bCopyFlag = false;
                                }
                                else if (sSubString_Array[0] == "Frame")
                                {
                                    bCopyFlag = false;
                                }
                                else if (bCopyFlag == true)
                                    sCopyLine += sLine + Environment.NewLine;
                                
                            }

                            if (nTXTraceNumber > -1 && nRXTraceNumber > -1 && 
                                nReadPH1 > -1 && nReadPH2 > -1 && bCopyFlag == false)
                                break;
                        }
                    }
                    finally
                    {
                        srReadFile.Close();
                    }

                    for (int nIndex = 0; nIndex < LFreqItemParam.Count; nIndex++)
                    {
                        if (bGetData_Array[nIndex] == false)
                        {
                            FrequencyItemParam cCurItem = LFreqItemParam[nIndex];

                            if (nReadPH1 == cCurItem.m_nPH1 && nReadPH2 == cCurItem.m_nPH2 &&
                                nTXTraceNumber == cCurItem.m_nTXTraceNumber && nRXTraceNumber == cCurItem.m_nRXTraceNumber)
                            {
                                List<int[,]> nFrameData_List = new List<int[,]>();
                                int[,] MaxFrameData_Array = new int[nTXTraceNumber, nRXTraceNumber];
                                srReadFile = new StreamReader(sFilePath, Encoding.Default);

                                if (xGetFrameData(ref nFrameData_List, srReadFile, nTXTraceNumber, nRXTraceNumber, Path.GetFileName(sFilePath)) == false)
                                    return false;

                                for (int nY = 0; nY < nTXTraceNumber; nY++)
                                {
                                    for (int nX = 0; nX < nRXTraceNumber; nX++)
                                    {
                                        List<int> nValue_List = new List<int>();

                                        for (int i = 0; i < nFrameData_List.Count; i++)
                                            nValue_List.Add(nFrameData_List[i][nY, nX]);

                                        int nMaxValue = nValue_List.Max();
                                        MaxFrameData_Array[nY, nX] = nMaxValue;
                                    }
                                }

                                if (xSaveFrameDataByFile(cCurItem, MainConstantParameter.DATATYPE_MAX_PBASEMinusPADC, m_sLogDirectoryPath, 
                                                         MaxFrameData_Array, nTXTraceNumber, nRXTraceNumber, sCopyLine) == false)
                                    return false;

                                bGetData_Array[nIndex] = true;
                                break;
                            }
                        }
                    }
                }

                for (int nIndex = 0; nIndex < bGetData_Array.Length; nIndex++)
                {
                    if (bGetData_Array[nIndex] == false)
                    {
                        m_sErrorMessage = string.Format("Get PreviousStep {0} File Error(RankIndex={1})", MainConstantParameter.DATATYPE_BASEMinusADC, nIndex + 1);
                        return false;
                    }
                }
                */
                #endregion
            }

            return true;
        }

        /// <summary>
        /// 根據流程步驟載入對應的頻率項目或Raw ADC Sweep項目列表,並驗證列表是否有效
        /// </summary>
        /// <param name="cFlowStep">當前執行的流程步驟,用於判斷要載入的列表類型及參數</param>
        /// <returns>成功載入且列表有效(包含至少一個項目)回傳true;列表為空、載入失敗或驗證失敗則回傳false並設定錯誤訊息</returns>
        private bool LoadFreqeuncyItemList(frmMain.FlowStep cFlowStep)
        {
            OutputMessage("[State]Load Frequency Set");

            if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase1)
            {
                SetFrequencyItemList cSetFrequencyItemList = new SetFrequencyItemList(m_cfrmParent);
                m_cFreqencyItem_List = cSetFrequencyItemList.SetFrequencyItem_FRPH1();
            }
            else if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2 ||
                     (cFlowStep.m_eStep == MainStep.AC_FrequencyRank && ParamFingerAutoTuning.m_nACFRModeType == 1))
            {
                SetFrequencyItemList cSetFrequencyItemList = new SetFrequencyItemList(m_cfrmParent);
                m_cFreqencyItem_List = cSetFrequencyItemList.SetFrequencyItem_FRPH2_ACFR(cFlowStep, m_sMutualFrequencyPath, m_sSkipFrequencyPath);
                m_sErrorMessage = cSetFrequencyItemList.ErrorMessage;

                if (cSetFrequencyItemList.PassFlag == false)
                    return false;
            }
            else if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank && ParamFingerAutoTuning.m_nACFRModeType != 1)
            {
                SetFrequencyItemList cSetFrequencyItemList = new SetFrequencyItemList(m_cfrmParent);
                m_cFreqencyItem_List = cSetFrequencyItemList.SetFrequencyItem_ACFR(m_sACFrequencyPath, m_sSkipFrequencyPath);
                m_sErrorMessage = cSetFrequencyItemList.ErrorMessage;

                if (cSetFrequencyItemList.PassFlag == false)
                    return false;
            }
            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                SetRawADCSweepItemList cSetRawADCSweepItemList = new SetRawADCSweepItemList();
                m_cRawADCSweepItem_List = cSetRawADCSweepItemList.SetFrequencyItem_RawADCS(m_eICGenerationType, m_eICSolutionType);
            }
            else if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                     cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                SetFrequencyItemList cSetFrequencyItemList = new SetFrequencyItemList(m_cfrmParent);

                if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep)
                    m_cFreqencyItem_List = cSetFrequencyItemList.SetFrequencyItem_SelfFS(m_sSelfFrequencyPath);
                else if (cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                    m_cFreqencyItem_List = cSetFrequencyItemList.SetFrequencyItem_SelfPNS(m_sSelfFrequencyPath);

                m_sErrorMessage = cSetFrequencyItemList.ErrorMessage;

                if (cSetFrequencyItemList.PassFlag == false)
                    return false;
            }

            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                if (m_cRawADCSweepItem_List == null || m_cRawADCSweepItem_List.Count == 0)
                {
                    m_sErrorMessage = string.Format("No Any Valiable Raw ADC Sweep Set[Step:{0}]", cFlowStep.m_sStepName);
                    return false;
                }
                else
                    return true;
            }
            else
            {
                if (m_cFreqencyItem_List == null || m_cFreqencyItem_List.Count == 0)
                {
                    m_sErrorMessage = string.Format("No Any Valiable Frequency Set[Step:{0}]", cFlowStep.m_sStepName);
                    return false;
                }
                else
                    return true;
            }
        }

        /// <summary>
        /// 複製前一步驟的報告檔案至當前記錄目錄的PreReport子目錄中,並重新命名為PreReport.csv
        /// </summary>
        /// <param name="sPreviousStepReportPath">前一步驟報告檔案的完整路徑</param>
        /// <param name="sLogDirectoryPath">當前記錄檔目錄路徑</param>
        private void CopyPreviousReportFile(string sPreviousStepReportPath, string sLogDirectoryPath)
        {
            string sPreviousReportPath = string.Format(@"{0}\{1}", sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_PREREPORT);

            if (Directory.Exists(sPreviousReportPath) == false)
                Directory.CreateDirectory(sPreviousReportPath);

            string sTargetFilePath = string.Format(@"{0}\PreReport.csv", sPreviousReportPath);

            File.Copy(sPreviousStepReportPath, sTargetFilePath);
        }
    }
}
