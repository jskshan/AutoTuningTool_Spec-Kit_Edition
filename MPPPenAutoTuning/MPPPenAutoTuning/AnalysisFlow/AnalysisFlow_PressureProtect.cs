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
    class AnalysisFlow_PressureProtect : AnalysisFlow
    {
        private int m_nPPValidReportNumber = 100;

        private List<byte> m_byteReport_List = new List<byte>();
        private List<List<byte>> m_byteData_List = new List<List<byte>>();
        private List<List<int>> m_nRXOriginalData_List = new List<List<int>>();
        private List<List<int>> m_nTXOriginalData_List = new List<List<int>>();
        private List<int> m_nRXOriginalIndex_List = new List<int>();
        private List<int> m_nTXOriginalIndex_List = new List<int>();
        private List<List<int>> m_nRXData_List = new List<List<int>>();
        private List<List<int>> m_nTXData_List = new List<List<int>>();

        private List<int> m_nRXValidTraceIndex_List = new List<int>();
        private List<int> m_nTXValidTraceIndex_List = new List<int>();
        private List<List<int>> m_nRXValidData_List = new List<List<int>>();
        private List<List<int>> m_nTXValidData_List = new List<List<int>>();

        public class DataValue
        {
            public int m_nThreshold = -1;
        }

        private List<DataValue> m_cDataValue_List = null;

        private void ClearDataArray()
        {
            m_byteReport_List.Clear();
            m_byteData_List.Clear();
            m_nRXOriginalData_List.Clear();
            m_nTXOriginalData_List.Clear();
            m_nRXOriginalIndex_List.Clear();
            m_nTXOriginalIndex_List.Clear();
            m_nRXData_List.Clear();
            m_nTXData_List.Clear();

            m_nRXValidTraceIndex_List.Clear();
            m_nTXValidTraceIndex_List.Clear();
            m_nRXValidData_List.Clear();
            m_nTXValidData_List.Clear();
        }

        public AnalysisFlow_PressureProtect(FlowStep cFlowStep, frmMain cfrmMain)
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

            m_nNormalReportDataLength = ParamAutoTuning.m_nNormalReportDataLength;

            m_nPPValidReportNumber = ParamAutoTuning.m_nPPValidReportNumber;
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

            foreach (string sFilePath in sValidReportDataFile_Array)
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

                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);

                //Folder
                m_sIntegrationFolderPath = string.Format(@"{0}\{1}\Integration Data", m_sResultFolderPath, sFileName);
                string sComputeFolderPath = string.Format(@"{0}\{1}\Compute Data", m_sResultFolderPath, sFileName);
                string sProcessFolderPath = string.Format(@"{0}\{1}\Process Data", m_sResultFolderPath, sFileName);
                Directory.CreateDirectory(m_sIntegrationFolderPath);
                Directory.CreateDirectory(sComputeFolderPath);
                Directory.CreateDirectory(sProcessFolderPath);

                //File
                m_sIntegrationFilePath = string.Format(@"{0}\Integration Data.csv", m_sIntegrationFolderPath);
                string sRXRawDataFilePath = string.Format(@"{0}\RX Raw Data.csv", sProcessFolderPath);
                string sTXRawDataFilePath = string.Format(@"{0}\TX Raw Data.csv", sProcessFolderPath);
                string sRXComputeFilePath = string.Format(@"{0}\RX Compute Data.csv", sComputeFolderPath);
                string sTXComputeFilePath = string.Format(@"{0}\TX Compute Data.csv", sComputeFolderPath);
                string sRXValidDataFilePath = string.Format(@"{0}\RX Valid Data.csv", sComputeFolderPath);
                string sTXValidDataFilePath = string.Format(@"{0}\TX Valid Data.csv", sComputeFolderPath);

                int nLineCounter = 0;
                string sLine = "";

                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                int nRankIndex = -1;

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
                    if (CheckAnalogParameterIsIdentical() == true)
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

                    nRankIndex = cDataInfo.m_nRankIndex;
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

                            cDataInfo.m_nRXTraceNumber = cParameter.m_nRXTraceNumber;
                            cDataInfo.m_nTXTraceNumber = cParameter.m_nTXTraceNumber;

                            bCheckFlag = true;
                        }

                        if (bCheckFlag == false)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0004;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("Parameter and Flow Info List Not Match In {0} File!", Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = "Parameter and Flow Info List Not Match";
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
                }

                // Read the file and display it line by line
                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

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
                            byte[] byteData_Array = new byte[6];

                            for (int nIndex = 0; nIndex <= 5; nIndex++)
                                byteData_Array[nIndex] = Convert.ToByte(sSplit_Array[nIndex], 16);

                            if (byteData_Array[0] != 0x07 ||
                                byteData_Array[2] != 0xFF || 
                                byteData_Array[3] != 0xFF ||
                                byteData_Array[4] != 0xFF || 
                                byteData_Array[5] != 0xFF)
                            {
                                cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0010;

                                m_cErrorInfo.m_sPrintErrorMessage = string.Format("Report Data Format Error In {0} File!", Path.GetFileName(sFilePath));
                                m_cErrorInfo.m_sRecordErrorMessage = string.Format("Report Data Format Error In Line {0}", nLineCounter);
                                m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                                m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                                m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                                cDataInfo.m_sErrorMessage = RunProcessErrorFlow();

                                srFile.Close();
                                break;
                            }
                        }
                        catch
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0010;

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
                            m_byteReport_List.Add(byteRawData);
                        }

                        m_byteData_List.Add(new List<byte>(m_byteReport_List));
                    }
                }
                finally
                {
                    srFile.Close();
                }
                #endregion

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                int nDataLength = m_byteData_List.Count;

                int nSectionNumber = 24;
                int nTraceTypeByte = 13;
                int nTraceIndexByte = 14;

                int nLeftBoundary = ParamAutoTuning.m_nPTValidReportEdgeNumber - 1;
                int nRightBoundary = nSectionNumber - ParamAutoTuning.m_nPTValidReportEdgeNumber;

                m_nRXTraceNumber = cDataInfo.m_nRXTraceNumber;
                m_nTXTraceNumber = cDataInfo.m_nTXTraceNumber;

                for (int nIndex = 0; nIndex < nDataLength; nIndex++)
                {
                    int nTraceTypeIntData = m_byteData_List[nIndex][nTraceTypeByte - 1];
                    int nIndexData = m_byteData_List[nIndex][nTraceIndexByte - 1];

                    //Rx Data
                    if ((nTraceTypeIntData & 0x0F) == 0x08)
                    {
                        if (nIndexData >= m_nRXTraceNumber)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0020;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("RX Trace Index Error In Line {0} in {1} File!", nIndex + 1, Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("RX Trace Index Error In Line {0}", nIndex + 1);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                            break;
                        }

                        int[] nData_Array = new int[nSectionNumber];

                        for (int nSectionIndex = 0; nSectionIndex < nSectionNumber; nSectionIndex++)
                            nData_Array[nSectionIndex] = (m_byteData_List[nIndex][2 * nSectionIndex + m_nNormalReportDataLength] * 256 + m_byteData_List[nIndex][2 * nSectionIndex + m_nNormalReportDataLength + 1]);

                        m_nRXOriginalData_List.Add(new List<int>(nData_Array));
                        m_nRXOriginalIndex_List.Add(nIndexData);
                    }
                    //Tx Data
                    else if ((nTraceTypeIntData & 0x0F) == 0x00)
                    {
                        if (nIndexData >= m_nRXTraceNumber)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0040;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("TX Trace Index Error In Line {0} in {1} File!", nIndex + 1, Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("TX Trace Index Error In Line {0}", nIndex + 1);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                            break;
                        }

                        int[] nData_Array = new int[nSectionNumber];

                        for (int nSectionIndex = 0; nSectionIndex < nSectionNumber; nSectionIndex++)
                            nData_Array[nSectionIndex] = (m_byteData_List[nIndex][2 * nSectionIndex + m_nNormalReportDataLength] * 256 + m_byteData_List[nIndex][2 * nSectionIndex + m_nNormalReportDataLength + 1]);

                        m_nTXOriginalData_List.Add(new List<int>(nData_Array));
                        m_nTXOriginalIndex_List.Add(nIndexData);
                    }
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteProcessData(string.Format("RX {0} Data", m_sSubStepCodeName), m_nRXOriginalData_List, sRXRawDataFilePath);
                WriteProcessData(string.Format("TX {0} Data", m_sSubStepCodeName), m_nTXOriginalData_List, sTXRawDataFilePath);

                m_nRXData_List = ConvertProcessData(m_nRXOriginalData_List, m_nRXOriginalIndex_List, m_nRXTraceNumber);
                m_nTXData_List = ConvertProcessData(m_nTXOriginalData_List, m_nTXOriginalIndex_List, m_nTXTraceNumber);

                WriteComputeDataFile("RX Compute Data", m_nRXData_List, sRXComputeFilePath);
                WriteComputeDataFile("TX Compute Data", m_nTXData_List, sTXComputeFilePath);

                GetValidData(m_nRXData_List, m_nRXOriginalIndex_List, nLeftBoundary, nRightBoundary, true);
                GetValidData(m_nTXData_List, m_nTXOriginalIndex_List, nLeftBoundary, nRightBoundary, false);

                if (m_nRXValidData_List.Count < m_nPPValidReportNumber || m_nRXValidData_List.Count == 0)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0080;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get RX Valid Data Too Few({0}<LB:{1}) In {2} File!", Convert.ToString(m_nRXValidData_List.Count), Convert.ToString(m_nPPValidReportNumber), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get RX Valid Data Too Few({0}<LB:{1})", Convert.ToString(m_nRXValidData_List.Count), Convert.ToString(m_nPPValidReportNumber));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                if (m_nTXValidData_List.Count < m_nPPValidReportNumber || m_nTXValidData_List.Count == 0)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0100;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get TX Valid Data Too Few({0}<LB:{1}) In {2} File!", Convert.ToString(m_nTXValidData_List.Count), Convert.ToString(m_nPPValidReportNumber), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get TX Valid Data Too Few({0}<LB:{1})", Convert.ToString(m_nTXValidData_List.Count), Convert.ToString(m_nPPValidReportNumber));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteComputeDataFile("RX Valid Data", m_nRXValidData_List, sRXValidDataFilePath);
                WriteComputeDataFile("TX Valid Data", m_nTXValidData_List, sTXValidDataFilePath);

                if (cDataInfo.m_sRecordErrorCode != "")
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0200;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", cDataInfo.m_sRecordErrorMessage, Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = cDataInfo.m_sRecordErrorMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteIntegrationDataFile(m_sIntegrationFilePath, m_nRXTraceNumber, m_nTXTraceNumber, m_nErrorFlag);

                m_nValidFileCount++;
                m_nFileCount++;

                OutputMessage(string.Format("Report Data({0}) Analysis Complete!!", Path.GetFileName(sFilePath)));
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

        protected override void GetFileInfoFromReportData(DataInfo cDataInfo, string sFilePath)
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
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RXTRACENUMBER, sLine, 0x000080, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_TXTRACENUMBER, sLine, 0x000100, m_nINFOTYPE_INT);

                    if (lGetInfoFlag == 0x0001FF)
                        break;

                }
            }
            finally
            {
                srFile.Close();
            }
        }

        protected override bool CheckInfoIsCorrect(ref string sErrorMessage, DataInfo cDataInfo)
        {
            CheckGeneralInfoIsCorrect(ref sErrorMessage, cDataInfo);

            if (cDataInfo.m_nRankIndex < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RANKINDEX));

            if (cDataInfo.m_nRXTraceNumber <= 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RXTRACENUMBER));

            if (cDataInfo.m_nTXTraceNumber <= 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_TXTRACENUMBER));

            if (sErrorMessage != "")
                return false;

            return true;
        }

        private List<List<int>> ConvertProcessData(List<List<int>> nInputData_List, List<int> nIndex_List, int nTraceNumber)
        {
            int[] nData_Array = new int[nTraceNumber];
            List<List<int>> nProcessData_List = new List<List<int>>();

            for (int nIndex = 0; nIndex < nInputData_List.Count; nIndex++)
            {
                Array.Clear(nData_Array, 0, nData_Array.Length);

                int nStartIndex = 0;
                int nEndIndex = 23;

                if (nIndex_List[nIndex] <= 11)
                {
                    nStartIndex = 0;
                    nEndIndex = 23;
                }
                else if (nIndex_List[nIndex] > 11 && nIndex_List[nIndex] < nTraceNumber - 12)
                {
                    nStartIndex = nIndex_List[nIndex] - 11;
                    nEndIndex = nIndex_List[nIndex] + 12;
                }
                else if (nIndex_List[nIndex] >= nTraceNumber - 12)
                {
                    nStartIndex = nTraceNumber - 24;
                    nEndIndex = nTraceNumber - 1;
                }

                int nCount = 0;

                for (int nDataIndex = nStartIndex; nDataIndex <= nEndIndex; nDataIndex++)
                {
                    nData_Array[nDataIndex] = nInputData_List[nIndex][nCount];
                    nCount++;
                }

                List<int> nData_List = new List<int>(nData_Array);
                nProcessData_List.Add(nData_List);
            }

            return nProcessData_List;
        }

        private void GetValidData(List<List<int>> nInputData_List, List<int> nIndex_List, int nLeftBoundary, int nRightBoundary, bool bRXTraceTypeFlag = true)
        {
            int nSectionNumber = 24;

            for (int nIndex = 0; nIndex < nInputData_List.Count; nIndex++)
            {
                int nIndexData = nIndex_List[nIndex];

                if (bRXTraceTypeFlag == true)
                {
                    if (nIndex_List[nIndex] > (int)((nSectionNumber / 2) - 1) && nIndex_List[nIndex] < m_nRXTraceNumber - (int)(nSectionNumber / 2))
                        nIndexData = (int)((nSectionNumber / 2) - 1);
                    else if (nIndex_List[nIndex] >= m_nRXTraceNumber - (int)(nSectionNumber / 2))
                    {
                        int nValue = m_nRXTraceNumber - nIndexData;
                        nIndexData = nSectionNumber - nValue;
                    }
                }
                else
                {
                    if (nIndex_List[nIndex] > (int)((nSectionNumber / 2) - 1) && nIndex_List[nIndex] < m_nTXTraceNumber - (int)(nSectionNumber / 2))
                        nIndexData = (int)((nSectionNumber / 2) - 1);
                    else if (nIndex_List[nIndex] >= m_nTXTraceNumber - (int)(nSectionNumber / 2))
                    {
                        int nValue = m_nTXTraceNumber - nIndexData;
                        nIndexData = nSectionNumber - nValue;
                    }
                }

                if (nIndexData <= nLeftBoundary || nIndexData >= nRightBoundary)
                    continue;

                if (bRXTraceTypeFlag == true)
                {
                    m_nRXValidTraceIndex_List.Add(nIndex_List[nIndex] + 1);
                    m_nRXValidData_List.Add(nInputData_List[nIndex]);
                }
                else
                {
                    m_nTXValidTraceIndex_List.Add(nIndex_List[nIndex] + 1);
                    m_nTXValidData_List.Add(nInputData_List[nIndex]);
                }
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

        private void WriteComputeDataFile(string sTitleName, List<List<int>> nProcessData_List, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                int nDataLength = nProcessData_List.Count;

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    string sText = "";

                    int nTraceNumber = nProcessData_List[nDataIndex].Count;

                    for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                    {
                        sText += string.Format("{0}", nProcessData_List[nDataIndex][nTraceIndex]);

                        if (nTraceIndex < nTraceNumber - 1)
                            sText += ",";
                    }

                    sw.WriteLine(sText);
                }
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
                SpecificText.m_sRXTraceNumber, 
                SpecificText.m_sTXTraceNumber, 
                SpecificText.m_sErrorMessage 
            };

            string[] sFWParameterTypeName_Array = new string[9] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                SpecificText.m_sFrequency, 
                SpecificText.m_sRXTraceNumber, 
                SpecificText.m_sTXTraceNumber, 
                SpecificText.m_sThreshold1, 
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
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankSort_List[nRankValue])
                        {
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_eSubStep.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRXTraceNumber.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nTXTraceNumber.ToString()));

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
                            m_cDataValue_List[nDataIndex].m_nThreshold = 100;
                            string sTheshold1 = m_cDataValue_List[nDataIndex].m_nThreshold.ToString();

                            if (m_cDataValue_List[nDataIndex].m_nThreshold < 0)
                                sTheshold1 = "N/A";

                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRXTraceNumber.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nTXTraceNumber.ToString()));
                            sw.Write(string.Format("{0},", sTheshold1));

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

        private void WriteFlowDataFile(string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
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
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                sw.Write(string.Format("{0},", FlowRobot.TOUCHPOINT_CEN.ToString()));
                                sw.Write(string.Format("{0},", FlowRecord.PRESSURE.ToString()));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));
                            }

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

                    WriteFlowDataFile(sFlowFilePath);
                    WriteFlowDataFile(sFlowFileBackUpPath);
                }

                FileProcess cFileProcess = new FileProcess(m_cfrmMain);
                cFileProcess.WriteResultListTxtFile(m_eSubStep, m_nSubStepState);
            }

            OutputMessage("Analysis Complete!!");
        }
    }
}
