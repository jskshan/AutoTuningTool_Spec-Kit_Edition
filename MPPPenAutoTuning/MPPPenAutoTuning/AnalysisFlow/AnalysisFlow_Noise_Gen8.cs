using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class AnalysisFlow_Noise_Gen8 : AnalysisFlow_Noise
    {
        private int m_nReportInfoDataLength = 5;
        private int m_nSectionIndexByteLocation = 1;

        private List<byte> m_byteRXReport_List = new List<byte>();
        private List<byte> m_byteTXReport_List = new List<byte>();

        private List<List<byte>> m_byteRXData_List = new List<List<byte>>();
        private List<List<byte>> m_byteTXData_List = new List<List<byte>>();

        protected override void ClearDataArray()
        {
            m_byteRXReport_List.Clear();
            m_byteTXReport_List.Clear();

            m_byteRXData_List.Clear();
            m_byteTXData_List.Clear();

            m_nRXOriginalData_List.Clear();
            m_nTXOriginalData_List.Clear();

            m_nRXData_Section1_List.Clear();
            m_nRXData_Section2_List.Clear();
            m_nRXData_Section3_List.Clear();
            m_nRXData_Section4_List.Clear();

            m_nTXData_Section1_List.Clear();
            m_nTXData_Section2_List.Clear();
        }

        public AnalysisFlow_Noise_Gen8(FlowStep cFlowStep, frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
            m_sFileDirectoryPath = m_cfrmMain.m_sFileDirectoryPath;
            m_sRecordProjectName = m_cfrmMain.m_sRecordProjectName;
            m_sFlowDirectoryPath = m_cfrmMain.m_sFlowDirectoryPath;

            InitializeParameter(cFlowStep);

            m_cDataValue_List = new List<DataValue>();

            m_bAllOverInnerReferenceValueHBFlag = false;
            m_bGen8ICSolutionTypeFlag = true;

            m_bGetRXReportDataFlag = false;
            m_bGetTXReportDataFlag = false;

            SetRecordInfo();
            CreateErrorInfo();
        }

        public override void LoadAnalysisParameter()
        {
            m_nReportDataLength = ParamAutoTuning.m_nGen8ReportDataLength;

            //m_nEdgeTraceNumber = ParamAutoTuning.m_nEdgeTraceNumber;
            m_nPartNumber = ParamAutoTuning.m_nPartNumber;

            m_bGetDistributionDataFlag = (ParamAutoTuning.m_nNoiseGetDistributionData == 1) ? true : false;

            m_nProcessReportCount = ParamAutoTuning.m_nNoiseProcessReportNumber;
            m_nStartSkipReportCount = ParamAutoTuning.m_nStartSkipReportCount;
            m_nLastSkipReportCount = ParamAutoTuning.m_nLastSkipReportCount;

            m_dInnerReferenceValueHB = ParamAutoTuning.m_dInnerReferenceValueHB;
            m_nEdgeSS_OFF_InnerRXWeightingPercent = (int)Math.Round(ParamAutoTuning.m_dEdgeSS_OFF_InnerRXWeightingPercent, 0, MidpointRounding.AwayFromZero);
            m_nEdgeSS_OFF_InnerTXWeightingPercent = (int)Math.Round(ParamAutoTuning.m_dEdgeSS_OFF_InnerTXWeightingPercent, 0, MidpointRounding.AwayFromZero);
            m_nEdgeSS_OFF_EdgeRXWeightingPercent = (int)Math.Round(ParamAutoTuning.m_dEdgeSS_OFF_EdgeRXWeightingPercent, 0, MidpointRounding.AwayFromZero);
            m_nEdgeSS_OFF_EdgeTXWeightingPercent = (int)Math.Round(ParamAutoTuning.m_dEdgeSS_OFF_EdgeTXWeightingPercent, 0, MidpointRounding.AwayFromZero);
            m_dMaxMinusMeanValueOverWarningStdevMagHB = ParamAutoTuning.m_dMaxMinusMeanValueOverWarningStdevMagHB;
            m_dMaxValueOverWarningAbsValueHB = ParamAutoTuning.m_dMaxValueOverWarningAbsValueHB;

            m_nTiltNoiseFrequencySetNumber = ParamAutoTuning.m_nTNFrequencyNumber;
            m_nOtherFrequencySetNumber = ParamAutoTuning.m_nOtherFrequencyNumber;

            if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_LOADDATA)
            {
                if (m_cfrmMain.m_sCPUType == MainConstantParameter.m_sCPUTYPE_AMD || m_cfrmMain.m_sCPUType == MainConstantParameter.m_sCPUTYPE_NONE)
                    m_nNoiseReportNumber = ParamAutoTuning.m_nNoiseReportNumber;
                else
                    m_nNoiseReportNumber = (int)((double)ParamAutoTuning.m_nNoiseReportNumber * 1.1);
            }
            else
                m_nNoiseReportNumber = ParamAutoTuning.m_nNoiseReportNumber;
        }

        public override void GetData(List<NoiseParameter> cParameter_List)
        {
            //取得資料夾內所有檔案
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

                ResetFlag();
                ClearDataArray();

                #region Get Report Data
                DataInfo cDataInfo = new DataInfo();
                DataValue cDataValue = new DataValue();
                m_cDataInfo_List.Add(cDataInfo);
                m_cDataValue_List.Add(cDataValue);
                int nFileIndex = m_cDataInfo_List.Count - 1;
                cDataInfo = m_cDataInfo_List[nFileIndex];
                cDataValue = m_cDataValue_List[nFileIndex];

                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);

                // Folder
                string sDistributionFolderPath = string.Format(@"{0}\{1}\Distribution Data", m_sResultFolderPath, sFileName);
                m_sIntegrationFolderPath = string.Format(@"{0}\{1}\Integration Data", m_sResultFolderPath, sFileName);
                string sProcessFolderPath = string.Format(@"{0}\{1}\Process Data", m_sResultFolderPath, sFileName);
                string sPictureFolderPath = string.Format(@"{0}\{1}\Picture", m_sResultFolderPath, sFileName);
                Directory.CreateDirectory(m_sIntegrationFolderPath);
                Directory.CreateDirectory(sProcessFolderPath);

                if (m_bGetDistributionDataFlag == true)
                {
                    Directory.CreateDirectory(sDistributionFolderPath);
                    Directory.CreateDirectory(sPictureFolderPath);
                }

                // File
                string sRXNoiseFilePath = string.Format(@"{0}\RX Noise Data.csv", sProcessFolderPath);
                string sTXNoiseFilePath = string.Format(@"{0}\TX Noise Data.csv", sProcessFolderPath);
                string sDistributionFilePath = string.Format(@"{0}\Distribution Data.csv", sDistributionFolderPath);
                string sHistogramFilePath = string.Format(@"{0}\Histogram Data.csv", sDistributionFolderPath);
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

                m_nRXTraceNumber = cDataInfo.m_nRXTraceNumber;
                m_nTXTraceNumber = cDataInfo.m_nTXTraceNumber;

                double dFrequency = ElanConvert.ComputeFrequnecyToDouble(cDataInfo.m_nSettingPH1, cDataInfo.m_nSettingPH2);
                cDataInfo.m_dFrequency = dFrequency;
                cDataValue.m_nPH1 = cDataInfo.m_nSettingPH1;
                cDataValue.m_nPH2 = cDataInfo.m_nSettingPH2;
                cDataValue.m_dFrequency = dFrequency;
                cDataValue.m_cRXReferenceData.InitializeArray(m_nRXTraceNumber);
                cDataValue.m_cTXReferenceData.InitializeArray(m_nTXTraceNumber);

                /*
                bool b800usDetectTimeFlag = true;

                if (frmMain.m_nModeFlag != MainConstantParameter.m_nMODE_LOADDATA && ParamAutoTuning.m_nAutoTune_P0_detect_time_Index != 1)
                    b800usDetectTimeFlag = false;
                */

                GetReportData(cDataInfo, cDataValue, sFilePath);
                #endregion

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                CheckReportDataIsEnough(cDataInfo, cDataValue, sFilePath, true);

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                CheckReportDataIsEnough(cDataInfo, cDataValue, sFilePath, false);

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                int nRXDataCount = ConvertReportData(true);
                int nTXDataCount = ConvertReportData(false);

                // Process Data
                WriteProcessData("RX Noise Data", m_nRXOriginalData_List, sRXNoiseFilePath);
                WriteProcessData("TX Noise Data", m_nTXOriginalData_List, sTXNoiseFilePath);

                if (m_bGetRXReportDataFlag == true && nRXDataCount == 0)
                    m_bGetRXReportDataFlag = false;

                if (m_bGetTXReportDataFlag == true && nTXDataCount == 0)
                    m_bGetTXReportDataFlag = false;

                if (m_bGetRXReportDataFlag == true)
                {
                    ComputeReferenceData(cDataValue, nRXDataCount, true);

                    ComputeReferenceValue(cDataValue, nRXDataCount, true);

                    ComputePreviousEdgeSubPwr(cDataValue);

                    ComputeResultData(cDataValue, nRXDataCount, true);
                }
                
                if (m_bGetTXReportDataFlag == true)
                {
                    ComputeReferenceData(cDataValue, nTXDataCount, false);
                
                    ComputeReferenceValue(cDataValue, nTXDataCount, false);
                
                    ComputeResultData(cDataValue, nTXDataCount, false);
                }

                WriteIntegrationDataFile(cDataValue, nRXDataCount, nTXDataCount);

                if (m_bGetDistributionDataFlag == true)
                {
                    if (m_bGetRXReportDataFlag == true)
                    {
                        cDataValue.m_cRXDistributionData = new DistributionData();
                        cDataValue.m_cRXDistributionData.InitializeArray(m_nRXTraceNumber, cDataValue.m_cRXReferenceValue.m_nRealPartNumber);

                        ComputeDistributionData(cDataValue, nRXDataCount, true);

                        ComputeHistogramData(cDataValue, nRXDataCount, true);

                        SaveHistogramChartPicture(cDataValue, sPictureFolderPath, true);
                    }

                    if (m_bGetTXReportDataFlag == true)
                    {
                        cDataValue.m_cTXDistributionData = new DistributionData();
                        cDataValue.m_cTXDistributionData.InitializeArray(m_nTXTraceNumber, cDataValue.m_cTXReferenceValue.m_nRealPartNumber);

                        ComputeDistributionData(cDataValue, nTXDataCount, false);

                        ComputeHistogramData(cDataValue, nTXDataCount, false);

                        SaveHistogramChartPicture(cDataValue, sPictureFolderPath, false);
                    }

                    WriteDistributionDataFile(cDataValue, sDistributionFilePath);

                    WriteHistogramDataFile(cDataValue, sHistogramFilePath);
                }

                m_nValidFileCount++;
                m_nFileCount++;

                OutputMessage(string.Format("Report Data({0}) Analysis Complete!!", Path.GetFileName(sFilePath)));
                OutputMainStatusStrip(string.Format("Data Set : {0}", m_nFileCount), m_nFileCount);
            }

            #region Final Check
            if (m_nValidFileCount < 1)
            {
                m_sErrorMessage = "No Enough Data. Data Analysis Error!!";
                OutputMessage("No Enough Data. Data Analysis Error!!");
                m_bErrorFlag = true;
                return;
            }
            #endregion
        }

        public override void ComputeAndOutputResult()
        {
            if (m_bErrorFlag == false)
            {
                SetSortData(CompareOperator.Frequency);

                if (m_bGetRXReportDataFlag == true)
                    WriteReferenceDataFile(true);

                if (m_bGetTXReportDataFlag == true)
                    WriteReferenceDataFile(false);

                WriteTotalReferenceDataFile();

                ComputeWeightingRank();

                SetSortData(CompareOperator.RankIndex);

                WriteResultDataFile();

                SaveTraceLineChartPictureFile();
                SaveFrequencyLineChartPictureFile();
            }

            OutputResultData();
        }

        protected override bool CheckInfoIsCorrect(ref string sErrorMessage, DataInfo cDataInfo)
        {
            CheckGeneralInfoIsCorrect(ref sErrorMessage, cDataInfo);

            if (sErrorMessage != "")
                return false;

            return true;
        }

        private void GetReportData(DataInfo cDataInfo, DataValue cDataValue, string sFilePath)
        {
            string sTraceType = StringConvert.m_sTRACETYPE_NA;
            int nLineCounter = 0;
            string sLine = "";

            // Read the file and display it line by line
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    nLineCounter++;

                    m_byteRXReport_List.Clear();
                    m_byteTXReport_List.Clear();

                    if (sLine == string.Format("====={0}=====", StringConvert.m_sTRACETYPE_RX))
                    {
                        m_bGetRXReportDataFlag = true;
                        sTraceType = StringConvert.m_sTRACETYPE_RX;
                    }
                    else if (sLine == string.Format("====={0}=====", StringConvert.m_sTRACETYPE_TX))
                    {
                        m_bGetTXReportDataFlag = true;
                        sTraceType = StringConvert.m_sTRACETYPE_TX;
                    }

                    if (StringConvert.CheckRecordInfoExist(sLine, m_sRecordInfo_List) == true)
                        continue;

                    string[] sSplit_Array = sLine.Split(m_charDELIMITER);

                    if (sSplit_Array.Length != m_nReportDataLength + 1)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0002;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("Report Data Length Error In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = string.Format("Report Data Length Error In Line {0}", nLineCounter);
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();

                        srFile.Close();
                        break;
                    }

                    if (m_nShiftStartByte > 0 && m_nShiftByteNumber > 0)
                    {
                        List<string> sSplit_List = sSplit_Array.ToList();
                        sSplit_List.RemoveRange(m_nShiftStartByte - 1, m_nShiftByteNumber);
                        sSplit_Array = sSplit_List.ToArray();
                    }

                    try
                    {
                        byte[] byteData_Array = new byte[m_nNormalReportDataLength];

                        for (int nByteIndex = 0; nByteIndex < m_nNormalReportDataLength; nByteIndex++)
                            byteData_Array[nByteIndex] = Convert.ToByte(sSplit_Array[nByteIndex], 16);

                        if (byteData_Array[0] != 0x17)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0004;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("Report Data Format Error In {0} File!", Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("Report Data Format Error In Line {0}", nLineCounter);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();

                            srFile.Close();
                            break;
                        }

                        if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_LOADDATA)
                        {
                        }
                    }
                    catch
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0004;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("Report Data Format Error In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = string.Format("Report Data Format Error In Line {0}", nLineCounter);
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();

                        srFile.Close();
                        break;
                    }

                    for (int nIndex = 0; nIndex < sSplit_Array.Length - 1; nIndex++)
                    {
                        byte byteRawData = Convert.ToByte(sSplit_Array[nIndex], 16);

                        if (sTraceType == StringConvert.m_sTRACETYPE_RX)
                            m_byteRXReport_List.Add(byteRawData);
                        else if (sTraceType == StringConvert.m_sTRACETYPE_TX)
                            m_byteTXReport_List.Add(byteRawData);
                    }

                    if (sTraceType == StringConvert.m_sTRACETYPE_RX)
                        m_byteRXData_List.Add(new List<byte>(m_byteRXReport_List));
                    else if (sTraceType == StringConvert.m_sTRACETYPE_TX)
                        m_byteTXData_List.Add(new List<byte>(m_byteTXReport_List));
                }
            }
            finally
            {
                srFile.Close();
            }
        }

        private void CheckReportDataIsEnough(DataInfo cDataInfo, DataValue cDataValue, string sFilePath, bool bRXTraceTypeFlag)
        {
            if (bRXTraceTypeFlag == true)
            {
                if (m_bGetRXReportDataFlag == true)
                {
                    int nSectionCount = m_nRXTraceNumber / MainConstantParameter.m_nReportMaxNumber_Gen8;

                    if (m_nRXTraceNumber % MainConstantParameter.m_nReportMaxNumber_Gen8 > 0)
                        nSectionCount++;

                    if (m_byteRXData_List.Count() < Math.Min(m_nNoiseReportNumber, ParamAutoTuning.m_nNoiseValidReportNumber) * nSectionCount)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0008;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Enough RX Report Data In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = "No Enough RX Report Data";
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    }
                }
                else
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0010;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("No RX Report Data In {0} File!", Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = "No RX Report Data";
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }
            }
            else
            {
                if (m_bGetTXReportDataFlag == true)
                {
                    int nSectionCount = m_nTXTraceNumber / MainConstantParameter.m_nReportMaxNumber_Gen8;

                    if (m_nTXTraceNumber % MainConstantParameter.m_nReportMaxNumber_Gen8 > 0)
                        nSectionCount++;

                    if (m_byteTXData_List.Count() < Math.Min(m_nNoiseReportNumber, ParamAutoTuning.m_nNoiseValidReportNumber) * nSectionCount)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0008;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Enough TX Report Data In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = "No Enough TX Report Data";
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    }
                }
                else
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0010;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("No TX Report Data In {0} File!", Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = "No TX Report Data";
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }
            }
        }

        private int ConvertReportData(bool bRXTraceTypeFlag)
        {
            if (bRXTraceTypeFlag == true)
            {
                int nRXDataLength = m_byteRXData_List.Count;
                int nNoiseDataStartByteLocation = m_nReportInfoDataLength + 1 - 1;
                int nRXSectionCount = m_nRXTraceNumber / MainConstantParameter.m_nReportMaxNumber_Gen8;

                if (m_nRXTraceNumber % MainConstantParameter.m_nReportMaxNumber_Gen8 > 0)
                    nRXSectionCount++;

                for (int nDataIndex = 0; nDataIndex < nRXDataLength; nDataIndex++)
                {
                    int nSectionIndex = Convert.ToInt32(m_byteRXData_List[nDataIndex][m_nSectionIndexByteLocation]);

                    int nTraceLength = MainConstantParameter.m_nReportMaxNumber_Gen8;

                    if (nSectionIndex == nRXSectionCount - 1)
                    {
                        nTraceLength = m_nRXTraceNumber % MainConstantParameter.m_nReportMaxNumber_Gen8;

                        if (nTraceLength == 0)
                            nTraceLength = MainConstantParameter.m_nReportMaxNumber_Gen8;
                    }

                    int[] nData_Array = new int[nTraceLength];

                    for (int nTraceIndex = 0; nTraceIndex < nTraceLength; nTraceIndex++)
                        nData_Array[nTraceIndex] = (m_byteRXData_List[nDataIndex][2 * nTraceIndex + m_nReportInfoDataLength] * 256 + m_byteRXData_List[nDataIndex][2 * nTraceIndex + m_nReportInfoDataLength + 1]);

                    if (nSectionIndex == 0)
                        m_nRXData_Section1_List.Add(new List<int>(nData_Array));
                    else if (nSectionIndex == 1)
                        m_nRXData_Section2_List.Add(new List<int>(nData_Array));
                    else if (nSectionIndex == 2)
                        m_nRXData_Section3_List.Add(new List<int>(nData_Array));
                    else if (nSectionIndex == 3)
                        m_nRXData_Section4_List.Add(new List<int>(nData_Array));
                }

                int nRXDataCount = m_nRXData_Section1_List.Count;

                if (nRXSectionCount >= 2)
                    nRXDataCount = Math.Min(nRXDataCount, m_nRXData_Section2_List.Count);
                if (nRXSectionCount >= 3)
                    nRXDataCount = Math.Min(nRXDataCount, m_nRXData_Section3_List.Count);
                if (nRXSectionCount >= 4)
                    nRXDataCount = Math.Min(nRXDataCount, m_nRXData_Section4_List.Count);

                for (int nDataIndex = 0; nDataIndex < nRXDataCount; nDataIndex++)
                {
                    List<int> nData_List = new List<int>();

                    if (nRXSectionCount >= 1)
                        nData_List.AddRange(m_nRXData_Section1_List[nDataIndex]);
                    if (nRXSectionCount >= 2)
                        nData_List.AddRange(m_nRXData_Section2_List[nDataIndex]);
                    if (nRXSectionCount >= 3)
                        nData_List.AddRange(m_nRXData_Section3_List[nDataIndex]);
                    if (nRXSectionCount >= 4)
                        nData_List.AddRange(m_nRXData_Section4_List[nDataIndex]);

                    m_nRXOriginalData_List.Add(nData_List);
                }

                m_nRXOriginalData_List.RemoveRange(nRXDataCount - m_nLastSkipReportCount, m_nLastSkipReportCount);
                m_nRXOriginalData_List.RemoveRange(0, m_nStartSkipReportCount);

                if (m_nProcessReportCount > 0)
                {
                    int nRemoveDataNumber = m_nRXOriginalData_List.Count - m_nProcessReportCount;

                    if (nRemoveDataNumber > 0)
                        m_nRXOriginalData_List.RemoveRange(m_nProcessReportCount, nRemoveDataNumber);
                }

                nRXDataCount = m_nRXOriginalData_List.Count;

                m_nRXData_Section1_List.Clear();
                m_nRXData_Section2_List.Clear();
                m_nRXData_Section3_List.Clear();
                m_nRXData_Section4_List.Clear();

                return nRXDataCount;
            }
            else
            {
                int nTXDataLength = m_byteTXData_List.Count;
                int nTXSectionCount = m_nTXTraceNumber / MainConstantParameter.m_nReportMaxNumber_Gen8;

                if (m_nTXTraceNumber % MainConstantParameter.m_nReportMaxNumber_Gen8 > 0)
                    nTXSectionCount++;

                for (int nDataIndex = 0; nDataIndex < nTXDataLength; nDataIndex++)
                {
                    int nSectionIndex = Convert.ToInt32(m_byteTXData_List[nDataIndex][m_nSectionIndexByteLocation]);

                    int nTraceLength = MainConstantParameter.m_nReportMaxNumber_Gen8;

                    if (nSectionIndex == nTXSectionCount - 1)
                    {
                        nTraceLength = m_nTXTraceNumber % MainConstantParameter.m_nReportMaxNumber_Gen8;

                        if (nTraceLength == 0)
                            nTraceLength = MainConstantParameter.m_nReportMaxNumber_Gen8;
                    }

                    int[] nData_Array = new int[nTraceLength];

                    for (int nTraceIndex = 0; nTraceIndex < nTraceLength; nTraceIndex++)
                        nData_Array[nTraceIndex] = (m_byteTXData_List[nDataIndex][2 * nTraceIndex + m_nReportInfoDataLength] * 256 + m_byteTXData_List[nDataIndex][2 * nTraceIndex + m_nReportInfoDataLength + 1]);

                    if (nSectionIndex == 0)
                        m_nTXData_Section1_List.Add(new List<int>(nData_Array));
                    else if (nSectionIndex == 1)
                        m_nTXData_Section2_List.Add(new List<int>(nData_Array));
                }

                int nTXDataCount = m_nTXData_Section1_List.Count;

                if (nTXSectionCount >= 2)
                    nTXDataCount = Math.Min(nTXDataCount, m_nTXData_Section2_List.Count);

                for (int nDataIndex = 0; nDataIndex < nTXDataCount; nDataIndex++)
                {
                    List<int> nData_List = new List<int>();

                    if (nTXSectionCount >= 1)
                        nData_List.AddRange(m_nTXData_Section1_List[nDataIndex]);
                    if (nTXSectionCount >= 2)
                        nData_List.AddRange(m_nTXData_Section2_List[nDataIndex]);

                    m_nTXOriginalData_List.Add(nData_List);
                }

                m_nTXOriginalData_List.RemoveRange(nTXDataCount - m_nLastSkipReportCount, m_nLastSkipReportCount);
                m_nTXOriginalData_List.RemoveRange(0, m_nStartSkipReportCount);

                if (m_nProcessReportCount > 0)
                {
                    int nRemoveDataNumber = m_nTXOriginalData_List.Count - m_nProcessReportCount;

                    if (nRemoveDataNumber > 0)
                        m_nTXOriginalData_List.RemoveRange(m_nProcessReportCount, nRemoveDataNumber);
                }

                nTXDataCount = m_nTXOriginalData_List.Count;

                m_nTXData_Section1_List.Clear();
                m_nTXData_Section2_List.Clear();

                return nTXDataCount;
            }
        }
    }
}
