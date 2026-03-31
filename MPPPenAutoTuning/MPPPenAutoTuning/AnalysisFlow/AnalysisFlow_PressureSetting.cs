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
    public class AnalysisFlow_PressureSetting : AnalysisFlow
    {
        private int m_nPTValidReportNumber = 1;
        private int m_nPTStartSkipReportNumber = 150;
        private int m_nPTEndSkipReportNumber = 150;

        private bool m_bOverRangeFlag = false;

        private List<byte> m_byteReport_List = new List<byte>();

        public class DataValue
        {
            public SubTuningStep m_eSubStep = SubTuningStep.ELSE;
            public int m_nSettingPH1 = -1;
            public int m_nSettingPH2 = -1;
            public int m_nReadPH1 = -1;
            public int m_nReadPH2 = -1;
            public double m_dFrequency = -1;

            public int m_nRankIndex = -1;

            public int m_nPressureMaxDFTRXMeanValue = -1;
            public int m_nPressMaxDFTRxMaxValue = -1;

            public int m_nIQ_BSH_P = -1;
            public int m_nPressure3BinsTH = -1;
            public int m_nPress_3BinsPwr = -1;

            public List<List<byte>> m_byteOriginalData_List = new List<List<byte>>();
            public List<int> m_nOriginalData_List = new List<int>();
            public List<int> m_nPressureMaxDFTRX_List = new List<int>();
        }

        public List<DataValue> m_cDataValue_List = null;

        private void ClearDataArray()
        {
            m_byteReport_List.Clear();
        }

        public AnalysisFlow_PressureSetting(FlowStep cFlowStep, frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
            m_sFileDirectoryPath = m_cfrmMain.m_sFileDirectoryPath;
            m_sRecordProjectName = m_cfrmMain.m_sRecordProjectName;
            m_sFlowDirectoryPath = m_cfrmMain.m_sFlowDirectoryPath;

            InitializeParameter(cFlowStep);

            m_cDataValue_List = new List<DataValue>();

            SetRecordInfo();
            CreateErrorInfo();
        }

        public override void LoadAnalysisParameter()
        {
            m_nReportDataLength = ParamAutoTuning.m_nReportDataLength;
            m_nShiftStartByte = ParamAutoTuning.m_nShiftStartByte;
            m_nShiftByteNumber = ParamAutoTuning.m_nShiftByteNumber;

            //m_nEdgeTraceNumber = ParamAutoTuning.m_nEdgeTraceNumber;
            m_nPartNumber = ParamAutoTuning.m_nPartNumber;

            m_nPTValidReportNumber = ParamAutoTuning.m_nPTValidReportNumber;
            m_nPTStartSkipReportNumber = ParamAutoTuning.m_nPTStartSkipReportNumber;
            m_nPTEndSkipReportNumber = ParamAutoTuning.m_nPTEndSkipReportNumber;

            m_nNormalReportDataLength = 11;
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

        public void GetData(List<PressureTuningParameter> cParameter_List)
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

            foreach (string FilePath in sValidReportDataFile_Array)
            {
                string sMessage = "";

                ClearDataArray();

                #region Get Report Data
                DataInfo cDataInfo = new DataInfo();
                DataValue cDataValue = new DataValue();
                m_cDataInfo_List.Add(cDataInfo);
                m_cDataValue_List.Add(cDataValue);
                int nFileIndex = m_cDataInfo_List.Count - 1;
                cDataInfo = m_cDataInfo_List[nFileIndex];
                cDataValue = m_cDataValue_List[nFileIndex];

                string sFileName = Path.GetFileNameWithoutExtension(FilePath);

                //Folder
                m_sIntegrationFolderPath = string.Format(@"{0}\{1}\Integration Data", m_sResultFolderPath, sFileName);
                string sComputeFolderPath = string.Format(@"{0}\{1}\Compute Data", m_sResultFolderPath, sFileName);
                string sProcessFolderPath = string.Format(@"{0}\{1}\Process Data", m_sResultFolderPath, sFileName);
                Directory.CreateDirectory(m_sIntegrationFolderPath);
                Directory.CreateDirectory(sComputeFolderPath);
                Directory.CreateDirectory(sProcessFolderPath);

                //File
                m_sIntegrationFilePath = string.Format(@"{0}\Integration Data.csv", m_sIntegrationFolderPath);
                string sRawDataFilePath = string.Format(@"{0}\Raw Data.csv", sProcessFolderPath);
                string sComputeFilePath = string.Format(@"{0}\Compute Data.csv", sComputeFolderPath);
                string sValidDataFilePath = string.Format(@"{0}\Valid Data.csv", sComputeFolderPath);

                int nLineCounter = 0;
                string sLine = "";

                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                int nRankIndex = -1;

                GetFileInfoFromReportData(cDataInfo, FilePath);

                if (CheckInfoIsCorrect(ref sMessage, cDataInfo, cDataValue) == false)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0001;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", sMessage, Path.GetFileName(FilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = sMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = FilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                if ((m_cfrmMain.m_nSkipPreviousStepFlag & MainConstantParameter.m_nSKIPFILE_FLOWTXT) != 0)
                {
                    if (CheckAnalogParameterIsIdentical() == true)
                    {
                        if (CheckDataIsInconsistent(nFileIndex) == false)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0002;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("Parameter Identical with Other Report Data In {0} File!", Path.GetFileName(FilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = "Parameter Identical with Other Report Data";
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = FilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                        }

                        if (m_bReadReportDataErrorFlag == true)
                            continue;
                    }

                    nRankIndex = m_cDataInfo_List[nFileIndex].m_nRankIndex;
                }
                else
                {
                    if (CheckAnalogParameterIsIdentical() == true)
                    {
                        bool bCheckFlag = false;

                        PressureTuningParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2 && x.m_nRankIndex == cDataInfo.m_nRankIndex);

                        if (cParameter != null)
                        {
                            nRankIndex = cParameter.m_nRankIndex;

                            if (ParamAutoTuning.m_nFlowMethodType == 1)
                            {
                                cDataInfo.m_sRecordErrorCode = cParameter.m_sErrorCode;
                                cDataInfo.m_sRecordErrorMessage = cParameter.m_sErrorMessage;
                            }

                            bCheckFlag = true;
                        }

                        if (bCheckFlag == false)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0004;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("Parameter and Flow Info List Not Match In {0} File!", Path.GetFileName(FilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = "Parameter and Flow Info List Not Match";
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = FilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                        }

                        if (m_bReadReportDataErrorFlag == true)
                            continue;

                        if (CheckDataIsInconsistent(nFileIndex) == false)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0002;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("Parameter Identical with Other Report Data In {0} File!", Path.GetFileName(FilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = "Parameter Identical with Other Report Data";
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = FilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                        }

                        if (m_bReadReportDataErrorFlag == true)
                            continue;
                    }
                }

                // Read the file and display it line by line
                StreamReader srFile = new StreamReader(FilePath, Encoding.Default);

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        nLineCounter++;

                        if (StringConvert.CheckRecordInfoExist(sLine, m_sRecordInfo_List) == true)
                            continue;

                        m_byteReport_List.Clear();

                        string[] sSplit_Array = sLine.Split(m_charDELIMITER);

                        if (sSplit_Array.Length != m_nReportDataLength + 1)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0008;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("Report Data Length Error In {0} File!", Path.GetFileName(FilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("Report Data Length Error In Line {0}", nLineCounter);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = FilePath;

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
                            byte[] byteData_Array = new byte[14];

                            for (int nIndex = 0; nIndex <= 10; nIndex++)
                                byteData_Array[nIndex] = Convert.ToByte(sSplit_Array[nIndex], 16);

                            if (byteData_Array[0] != 0x07 ||
                                byteData_Array[2] != 0xFF ||
                                byteData_Array[3] != 0xFF ||
                                byteData_Array[4] != 0xFF ||
                                byteData_Array[5] != 0xFF ||
                                byteData_Array[9] != 0xFA ||
                                byteData_Array[10] != 0xFA)
                            {
                                cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0010;

                                m_cErrorInfo.m_sPrintErrorMessage = string.Format("Report Data Format Error In {0} File!", Path.GetFileName(FilePath));
                                m_cErrorInfo.m_sRecordErrorMessage = string.Format("Report Data Format Error In Line {0}", nLineCounter);
                                m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                                m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                                m_cErrorInfo.m_sCurrnetFilePath = FilePath;

                                cDataInfo.m_sErrorMessage = RunProcessErrorFlow();

                                srFile.Close();
                                break;
                            }
                        }
                        catch
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0010;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("Report Data Format Error In {0} File!", Path.GetFileName(FilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("Report Data Format Error In Line {0}", nLineCounter);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = FilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();

                            srFile.Close();
                            break;
                        }

                        for (int nIndex = 0; nIndex < sSplit_Array.Length - 1; nIndex++)
                        {
                            byte byteRawData = Convert.ToByte(sSplit_Array[nIndex], 16);
                            m_byteReport_List.Add(byteRawData);
                        }

                        cDataValue.m_byteOriginalData_List.Add(new List<byte>(m_byteReport_List));
                    }
                }
                finally
                {
                    srFile.Close();
                }
                #endregion

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                ConvertPressurePowerData(nFileIndex);

                if (cDataValue.m_nPressureMaxDFTRX_List.Count == 0 || cDataValue.m_nPressureMaxDFTRX_List.Count < m_nPTValidReportNumber)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0020;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get Valid Data Too Few({0}<LB:{1}) In {2} File!", Convert.ToString(cDataValue.m_nPressureMaxDFTRX_List.Count), Convert.ToString(m_nPTValidReportNumber), Path.GetFileName(FilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get Valid Data Too Few({0}<LB:{1})", Convert.ToString(cDataValue.m_nPressureMaxDFTRX_List.Count), Convert.ToString(m_nPTValidReportNumber));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = FilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    break;
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteRawDataFile("Pressure Raw Data", cDataValue.m_nOriginalData_List, sRawDataFilePath);
                WriteValidDataFile("Pressure Valid Data", cDataValue.m_nPressureMaxDFTRX_List, sValidDataFilePath);

                int nComputeThresholdFlag = ComputeThreshold(nFileIndex);

                if (nComputeThresholdFlag != 0)
                {
                    sMessage = "";

                    if (nComputeThresholdFlag == 1)
                    {
                        sMessage = string.Format("Pressure Power Mean Under LB({0}<LB:{1})", cDataValue.m_nPressureMaxDFTRXMeanValue, ParamAutoTuning.m_nPressMaxDFTRxRefValueLB);
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0040;
                        m_bOverRangeFlag = true;
                    }
                    else if (nComputeThresholdFlag == 2)
                    {
                        sMessage = string.Format("Pressure Power Mean Over HB({0}>HB:{1})", cDataValue.m_nPressureMaxDFTRXMeanValue, ParamAutoTuning.m_nPressMaxDFTRxRefValueHB);
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0080;
                        m_bOverRangeFlag = true;
                    }

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", sMessage, Path.GetFileName(FilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = sMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = FilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (cDataInfo.m_sRecordErrorCode != "")
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0040;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", cDataInfo.m_sRecordErrorMessage, Path.GetFileName(FilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = cDataInfo.m_sRecordErrorMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = FilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteIntegrationDataFile(m_sIntegrationFilePath, m_nRXTraceNumber, m_nTXTraceNumber, m_nErrorFlag);

                m_nValidFileCount++;
                m_nFileCount++;

                OutputMessage(string.Format("Report Data({0}) Analysis Complete!!", Path.GetFileName(FilePath)));
                OutputMainStatusStrip(string.Format("Data Set : {0}", m_nFileCount), m_nFileCount);
            }

            if (ParamAutoTuning.m_nFlowMethodType != 1 && m_nValidFileCount < 1)
            {
                m_sErrorMessage = "No Enough Data. Data Analysis Error!!";
                OutputMessage("No Enough Data. Data Analysis Error!!");
                m_bErrorFlag = true;
                return;
            }
        }

        public void ComputeAndOutputResult()
        {
            OutputResultData();
        }

        private void GetFileInfoFromReportData(DataInfo cDataInfo, DataValue cDataValue, string sFilePath)
        {
            long lGetInfoFlag = 0;
            string sLine = "";

            cDataInfo.m_sFileName = Path.GetFileName(sFilePath);

            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_SUBSTEP, sLine, 0x000001, m_nINFOTYPE_TUNINGSTEP);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_SETTINGPH1, sLine, 0x000002, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_SETTINGPH2, sLine, 0x000004, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_READPH1, sLine, 0x000008, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_READPH2, sLine, 0x000010, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_FREQUENCY, sLine, 0x000020, m_nINFOTYPE_DOUBLE);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RANKINDEX, sLine, 0x000040, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_RIQ_BSH_P, sLine, 0x000080, m_nINFOTYPE_INT);

                    if (lGetInfoFlag == 0x0000FF)
                        break;
                }
            }
            finally
            {
                srFile.Close();
            }
        }

        private void GetFileInfo(ref long lGetInfoFlag, DataValue cDataValue, string sParameterName, string sLine, long lInfoFlag, int nValueType)
        {
            if (sLine.Contains(sParameterName) == true)
            {
                string[] sSplit_Array = sLine.Split(new char[2] { '=', ',' });

                for (int nSplitIndex = 0; nSplitIndex < sSplit_Array.Length; nSplitIndex++)
                {
                    if (sSplit_Array[nSplitIndex].Replace(" ", "") == sParameterName)
                    {
                        string sValue = sSplit_Array[nSplitIndex + 1].Trim();

                        switch (nValueType)
                        {
                            case m_nINFOTYPE_TUNINGSTEP:
                                break;
                            case m_nINFOTYPE_INT:
                                int nValue = 0;

                                if (ElanConvert.CheckIsInt(sValue) == true)
                                    nValue = Convert.ToInt32(sValue);

                                if (sParameterName == StringConvert.m_sRECORD_RIQ_BSH_P)
                                    cDataValue.m_nIQ_BSH_P = nValue;

                                break;
                            case m_nINFOTYPE_DOUBLE:
                                break;
                            default:
                                break;
                        }

                        lGetInfoFlag |= lInfoFlag;
                        break;
                    }
                }
            }
        }

        private bool CheckInfoIsCorrect(ref string sErrorMessage, DataInfo cDataInfo, DataValue cDataValue)
        {
            CheckGeneralInfoIsCorrect(ref sErrorMessage, cDataInfo);

            if (cDataInfo.m_nRankIndex < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RANKINDEX));

            if (cDataValue.m_nIQ_BSH_P < 0 ||
                cDataValue.m_nIQ_BSH_P < ParamAutoTuning.m_nPTIQ_BSH_P_LB ||
                cDataValue.m_nIQ_BSH_P > ParamAutoTuning.m_nPTIQ_BSH_P_HB)
                SetErrorMessage(ref sErrorMessage, "\"_Pen_Ntrig_IQ_BSH_P\" Format Error Or Over Range");

            if (sErrorMessage != "")
                return false;

            return true;
        }

        private void ConvertPressurePowerData(int nFileIndex)
        {
            int nPressureMaxDFTRXDataByteLocation = 17;
            int nPressurePowerDataByteLocation = 19;

            int nDataCount = m_cDataValue_List[nFileIndex].m_byteOriginalData_List.Count;

            for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
            {
                if (//m_cDataValue_List[nFileIndex].m_byteOriginalData_List[nDataIndex][1] != 0x03 ||
                    (m_cDataValue_List[nFileIndex].m_byteOriginalData_List[nDataIndex][nPressurePowerDataByteLocation] == 0x00 &&
                     m_cDataValue_List[nFileIndex].m_byteOriginalData_List[nDataIndex][nPressurePowerDataByteLocation + 1] == 0x00))
                    continue;

                int nValue = m_cDataValue_List[nFileIndex].m_byteOriginalData_List[nDataIndex][nPressureMaxDFTRXDataByteLocation] * 256 + m_cDataValue_List[nFileIndex].m_byteOriginalData_List[nDataIndex][nPressureMaxDFTRXDataByteLocation + 1];

                m_cDataValue_List[nFileIndex].m_nOriginalData_List.Add(nValue);
            }

            for (int nDataIndex = 0; nDataIndex < m_cDataValue_List[nFileIndex].m_nOriginalData_List.Count; nDataIndex++)
            {
                if (nDataIndex < m_nPTStartSkipReportNumber || nDataIndex > m_cDataValue_List[nFileIndex].m_nOriginalData_List.Count - m_nPTEndSkipReportNumber - 1)
                    continue;

                m_cDataValue_List[nFileIndex].m_nPressureMaxDFTRX_List.Add(m_cDataValue_List[nFileIndex].m_nOriginalData_List[nDataIndex]);
            }
        }

        private void WriteIntegrationDataFile(string sFilePath, int nRXDataNumber, int nTXDataNumber, int nErrorFlag)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            
            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_INTEGRATION);

                sw.WriteLine();

                sw.WriteLine("Information");
                sw.WriteLine(string.Format("RX Data Number,{0}", nRXDataNumber));
                sw.WriteLine(string.Format("TX Data Number,{0}", nTXDataNumber));
                sw.WriteLine(string.Format("ErrorFlag,{0}", nErrorFlag));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WriteRawDataFile(string sTitleName, List<int> nProcessData_List, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                for (int nDataIndex = 0; nDataIndex < nProcessData_List.Count; nDataIndex++)
                    sw.WriteLine(string.Format("{0}", nProcessData_List[nDataIndex]));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WriteValidDataFile(string sTitleName, List<int> nProcessData_List, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                sw.WriteLine();
                sw.WriteLine("PressureMaxDFTRx");

                for (int nIndex = 0; nIndex < nProcessData_List.Count; nIndex++)
                    sw.WriteLine(string.Format("{0}", nProcessData_List[nIndex]));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        protected override void WriteStepListDataFile(string sProjectName, string sFilePath)
        {
            string[] sValueTypeName_Array = new string[11] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sFlowStep, 
                SpecificText.m_sSettingPH1, 
                SpecificText.m_sSettingPH2, 
                SpecificText.m_sReadPH1, 
                SpecificText.m_sReadPH2, 
                SpecificText.m_sFrequency, 
                SpecificText.m_sPressMaxDFTRxMax, 
                SpecificText.m_sPressMaxDFTRxMean, 
                SpecificText.m_sErrorMessage 
            };

            string[] sFWParameterTypeName_Array = new string[9] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                SpecificText.m_sFrequency, 
                SpecificText.m_s_Pen_Ntrig_IQ_BSH_P, 
                SpecificText.m_scActivePen_FM_Pressure3BinsTH, 
                SpecificText.m_sPress_3BinsPwr,
                SpecificText.m_sErrorMessage 
            };

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_RESULT);

                sw.WriteLine();

                sw.WriteLine("Project Information");
                sw.WriteLine(string.Format("Flow Step,{0}", m_eSubStep.ToString()));
                sw.WriteLine(string.Format("Control Mode,{0}", StringConvert.m_dictControlModeMappingTable[m_cfrmMain.m_nModeFlag]));

                sw.WriteLine();

                sw.WriteLine("Step Data Information List");

                for (int nColumnIndex = 0; nColumnIndex < sValueTypeName_Array.Length; nColumnIndex++)
                {
                    sw.Write(sValueTypeName_Array[nColumnIndex]);
                    if (nColumnIndex == sValueTypeName_Array.Length - 1)
                        sw.WriteLine();
                    else
                        sw.Write(",");
                }

                /*
                int nMinRank = GetMinRankIndex(m_cDataInfo_List);
                int nMaxRank = GetMaxRankIndex(m_cDataInfo_List);
                int nRankIndex = 0;
                */

                List<int> nRankSort_List = GetRankSortList(m_cDataInfo_List);

                foreach (int nRankValue in nRankSort_List)
                {
                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                        {
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_eSubStep.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPressMaxDFTRxMaxValue.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPressureMaxDFTRXMeanValue.ToString()));

                            string sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sErrorMessage);

                            if (ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode != "")
                                sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sRecordErrorMessage);

                            sw.WriteLine(sErrorMessage);

                            break;
                        }
                    }
                }

                sw.WriteLine();
                sw.WriteLine("FW Parameter Information List");

                for (int nColumnIndex = 0; nColumnIndex < sFWParameterTypeName_Array.Length; nColumnIndex++)
                {
                    sw.Write(sFWParameterTypeName_Array[nColumnIndex]);

                    if (nColumnIndex == sFWParameterTypeName_Array.Length - 1)
                        sw.WriteLine();
                    else
                        sw.Write(",");
                }

                //nRankIndex = 0;

                foreach (int nRankValue in nRankSort_List)
                {
                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                        {
                            string sParamIQ_BSH_P = m_cDataValue_List[nDataIndex].m_nIQ_BSH_P.ToString();
                            string sParamPressure3BinsTH = m_cDataValue_List[nDataIndex].m_nPressure3BinsTH.ToString();
                            string sParamPress_3BinsPwr = m_cDataValue_List[nDataIndex].m_nPress_3BinsPwr.ToString();

                            if (m_cDataValue_List[nDataIndex].m_nIQ_BSH_P < 0 || m_bOverRangeFlag == true)
                                sParamIQ_BSH_P = "N/A";

                            if (m_cDataValue_List[nDataIndex].m_nPressure3BinsTH < 0)
                                sParamPressure3BinsTH = "N/A";

                            if (m_cDataValue_List[nDataIndex].m_nPress_3BinsPwr < 0)
                                sParamPress_3BinsPwr = "N/A";
                            else
                                sParamPress_3BinsPwr = "80";

                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", sParamIQ_BSH_P));
                            sw.Write(string.Format("{0},", sParamPressure3BinsTH));
                            sw.Write(string.Format("{0},", sParamPress_3BinsPwr));

                            string sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sErrorMessage);

                            if (ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode != "")
                                sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sRecordErrorMessage);

                            sw.WriteLine(sErrorMessage);

                            break;
                        }
                    }
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WriteFlowDataFile(SubTuningStep eSubStep, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                bool bSetPTFlowFlag = false;

                /*
                int nMinRank = GetMinRankIndex(m_cDataInfo_List);
                int nMaxRank = GetMaxRankIndex(m_cDataInfo_List);
                */

                List<int> nRankSort_List = GetRankSortList(m_cDataInfo_List);

                foreach (int nRankValue in nRankSort_List)
                {
                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                        {
                            if ((ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode == "" && m_cDataInfo_List[nDataIndex].m_sErrorMessage == "") ||
                                ParamAutoTuning.m_nFlowMethodType != 1)
                            {
                                if (eSubStep == SubTuningStep.PRESSURETABLE)
                                {
                                    if (bSetPTFlowFlag == true)
                                        break;

                                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                    sw.Write(string.Format("{0},", FlowRobot.TOUCHPOINT_CEN.ToString()));
                                    sw.Write(string.Format("{0},", FlowRecord.PRESSURE.ToString()));
                                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                    sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));

                                    bSetPTFlowFlag = true;
                                }
                            }

                            break;
                        }

                        if (bSetPTFlowFlag == true)
                            break;
                    }
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void OutputResultData()
        {
            WriteStepListDataFile(m_sProjectName, m_sStepListPath);

            if (m_bErrorFlag == false)
            {
                if (Directory.Exists(m_sFlowDirectoryPath) == false)
                    Directory.CreateDirectory(m_sFlowDirectoryPath);

                SubTuningStep[] sNextSubStep_Array = StringConvert.m_dictNextSubStepMappingTable[m_eSubStep];

                for (int nIndex = 0; nIndex < sNextSubStep_Array.Length; nIndex++)
                {
                    string sFlowFileName = m_cfrmMain.m_sSubTuningStepFileName_Array[(int)sNextSubStep_Array[nIndex]];
                    string sFlowFilePath = string.Format(@"{0}\{1}", m_sFlowDirectoryPath, sFlowFileName);
                    string sFlowFileBackUpPath = string.Format(@"{0}\{1}", m_sFlowBackUpFolderPath, sFlowFileName);

                    WriteFlowDataFile(sNextSubStep_Array[nIndex], sFlowFilePath);
                    WriteFlowDataFile(sNextSubStep_Array[nIndex], sFlowFileBackUpPath);
                }

                FileProcess cFileProcess = new FileProcess(m_cfrmMain);
                cFileProcess.WriteResultListTxtFile(m_eSubStep, m_nSubStepState);
            }

            OutputMessage("Analysis Complete!!");
        }

        private int ComputeThreshold(int nFileIndex)
        {
            int nErrorFlag = 0;
            double dMeanValue = 0.0;
            int nDataCount = m_cDataValue_List[nFileIndex].m_nPressureMaxDFTRX_List.Count;

            for (int nIndex = 0; nIndex < nDataCount; nIndex++)
                dMeanValue += m_cDataValue_List[nFileIndex].m_nPressureMaxDFTRX_List[nIndex];

            m_cDataValue_List[nFileIndex].m_nPressureMaxDFTRXMeanValue = (int)Math.Round(dMeanValue / nDataCount, 0, MidpointRounding.AwayFromZero);

            if (m_cDataValue_List[nFileIndex].m_nPressureMaxDFTRXMeanValue < ParamAutoTuning.m_nPressMaxDFTRxRefValueLB)
            {
                nErrorFlag = 1;
                return nErrorFlag;
            }
            else if (m_cDataValue_List[nFileIndex].m_nPressureMaxDFTRXMeanValue > ParamAutoTuning.m_nPressMaxDFTRxRefValueHB)
            {
                nErrorFlag = 2;
                return nErrorFlag;
            }

            m_cDataValue_List[nFileIndex].m_nPressMaxDFTRxMaxValue = m_cDataValue_List[nFileIndex].m_nPressureMaxDFTRX_List.Max();

            m_cDataValue_List[nFileIndex].m_nPressure3BinsTH = (int)Math.Round((double)m_cDataValue_List[nFileIndex].m_nPressMaxDFTRxMaxValue / 4, 0, MidpointRounding.AwayFromZero);
            m_cDataValue_List[nFileIndex].m_nPress_3BinsPwr = (int)Math.Round(((double)m_cDataValue_List[nFileIndex].m_nPressureMaxDFTRXMeanValue / ParamAutoTuning.m_nPressMaxDFTRxRefValueLB) * 0x50, 0, MidpointRounding.AwayFromZero);

            return nErrorFlag;
        }
    }
}