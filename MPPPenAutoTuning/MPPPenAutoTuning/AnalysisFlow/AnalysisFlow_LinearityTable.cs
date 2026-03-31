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
    public class AnalysisFlow_LinearityTable : AnalysisFlow
    {
        //64 Parts Constant Parameter
        private const int m_n64PARTS_SIZE = 64;
        private const int m_n64PARTS_BASEVALUE = 1024;
        private const int m_n64PARTS_NEGATIVEDATA = 0x8000;

        private const int m_nDRAWTYPECOUNT = 2;

        private const int m_nNORMALREPORTDATALENGTH = 11;

        //Linearity Tuning Data Type Constant Parameter
        private const string m_sLTDATATYPE_5T = "5T";
        private const string m_sLTDATATYPE_3T = "3T";
        private const string m_sLTDATATYPE_2TLE = "2TLE";
        private const string m_sLTDATATYPE_2THE = "2THE";

        //Trace Type Constant Parameter
        private const string m_sTRACETYPE_RX = "RX";
        private const string m_sTRACETYPE_TX = "TX";

        //Byte Location Constant Parameter
        private const int m_nBYTELOCATION_RX5TRACES = 12;
        private const int m_nBYTELOCATION_RXDEV = 22;
        private const int m_nBYTELOCATION_RXTRACEINDEX = 24;
        private const int m_nBYTELOCATION_TX5TRACES = 26;
        private const int m_nBYTELOCATION_TXDEV = 36;
        private const int m_nBYTELOCATION_TXTRACEINDEX = 38;

        private const int m_nBYTELOCATION_RX5TRACES_NEWFORMAT = 4;
        private const int m_nBYTELOCATION_RXDEV_NEWFORMAT = 14;
        private const int m_nBYTELOCATION_RXTRACEINDEX_NEWFORMAT = 16;
        private const int m_nBYTELOCATION_TX5TRACES_NEWFORMAT = 17;
        private const int m_nBYTELOCATION_TXDEV_NEWFORMAT = 27;
        private const int m_nBYTELOCATION_TXTRACEINDEX_NEWFORMAT = 29;

        private int m_nFirstEdgeAreaValidNumber = 3;
        private int m_nLastEdgeAreaValidNumber = 3;

        private string[] m_sDrawType_Array = new string[m_nDRAWTYPECOUNT] 
        { 
            StringConvert.m_sDRAWTYPE_HORIZONTAL,
            StringConvert.m_sDRAWTYPE_VERTICAL 
        };

        private string[] m_sLTDataType_Array = new string[] 
        { 
            m_sLTDATATYPE_5T,
            m_sLTDATATYPE_3T,
            m_sLTDATATYPE_2TLE,
            m_sLTDATATYPE_2THE 
        };

        private string[] m_sTraceType_Array = new string[] 
        { 
            m_sTRACETYPE_RX,                                           
            m_sTRACETYPE_TX 
        };

        private List<byte> m_byteReport_List = new List<byte>();
        private List<List<byte>> m_byteHorizontalOriginalData_List = new List<List<byte>>();
        private List<List<byte>> m_byteVerticalOriginalData_List = new List<List<byte>>();

        public class TracePartData
        {
            public int m_nTraceIndex = -1;
            public long[] m_lTracePartData_Array = new long[m_n64PARTS_SIZE];
            //public int m_nValidDataCount = 0;
            public List<int> m_nTraceRawData_List = new List<int>();
        }

        public class LTData
        {
            public long[] m_lRXLTData_Array = new long[m_n64PARTS_SIZE];
            public long[] m_lTXLTData_Array = new long[m_n64PARTS_SIZE];
            public List<TracePartData> m_cRXTracePartData_List = new List<TracePartData>();
            public List<TracePartData> m_cTXTracePartData_List = new List<TracePartData>();
        }

        public class DataValue
        {
            public int m_nInvalidDataCount_Horizontal = 0;
            public int m_nInvalidDataCount_Vertical = 0;

            public LTData m_cLTData_5T = new LTData();
            public LTData m_cLTData_3T = new LTData();
            public LTData m_cLTData_2TLE = new LTData();
            public LTData m_cLTData_2THE = new LTData();

            public double[] m_dRXGain_Array = null;
            public double[] m_dTXGain_Array = null;
        }

        private List<DataValue> m_cDataValue_List = null;

        public class DrawTypeInfo
        {
            public SubTuningStep m_eSubStep = SubTuningStep.ELSE;
            public int m_nSettingPH1 = -1;
            public int m_nSettingPH2 = -1;
            public int m_nReadPH1 = -1;
            public int m_nReadPH2 = -1;
            public double m_dFrequency = -1.0;

            public int m_nRankIndex = -1;

            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public int m_nLineCount = -1;

            public int m_n5TRawDataType = 0;
        }

        private DrawTypeInfo[] m_cDrawTypeInfo_Array = null;

        private void ClearDataArray()
        {
            m_byteReport_List.Clear();
            m_byteHorizontalOriginalData_List.Clear();
            m_byteVerticalOriginalData_List.Clear();
        }

        public AnalysisFlow_LinearityTable(FlowStep cFlowStep, frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
            m_sFileDirectoryPath = m_cfrmMain.m_sFileDirectoryPath;
            m_sRecordProjectName = m_cfrmMain.m_sRecordProjectName;

            InitializeParameter(cFlowStep);

            m_cDataValue_List = new List<DataValue>();
            m_cDrawTypeInfo_Array = new DrawTypeInfo[m_nDRAWTYPECOUNT];

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
            m_nFirstEdgeAreaValidNumber = ParamAutoTuning.m_nLTFirstEdgeAreaValidNumber;
            m_nLastEdgeAreaValidNumber = ParamAutoTuning.m_nLTLastEdgeAreaValidNumber;

            m_nNormalReportDataLength = m_nNORMALREPORTDATALENGTH;
        }

        public void SetFileDirectory()
        {
            m_sStepFolderPath = string.Format(@"{0}\{1}", m_sFileDirectoryPath, m_sSubStepName);
            m_sResultFolderPath = string.Format(@"{0}\{1}({2})", m_sFileDirectoryPath, SpecificText.m_sResultText, m_sSubStepCodeName);
            m_sFlowBackUpFolderPath = string.Format(@"{0}\{1}", m_sFileDirectoryPath, SpecificText.m_sFlowText);
            m_sProjectName = m_sRecordProjectName;
            m_sProjectFolderPath = m_sFileDirectoryPath;
            m_sStepListPath = string.Format(@"{0}\{1}_{2}.csv", m_sProjectFolderPath, SpecificText.m_sStepListText, m_sSubStepCodeName);

            m_sReferenceFilePath = string.Format(@"{0}\{1}", m_sStepFolderPath, SpecificText.m_sReferenceFileName);
        }

        public void GetData(List<LinearityTuningParameter> cParameter_List)
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
                Directory.CreateDirectory(m_sIntegrationFolderPath);
                Directory.CreateDirectory(sComputeFolderPath);

                //File
                m_sIntegrationFilePath = string.Format(@"{0}\Integration Data.csv", m_sIntegrationFolderPath);
                string sRX5TLTFilePath = string.Format(@"{0}\RX 5T LinearityTable.txt", m_sResultFolderPath);
                string sRX3TLTFilePath = string.Format(@"{0}\RX 3T LinearityTable.txt", m_sResultFolderPath);
                string sRX2TLELTFilePath = string.Format(@"{0}\RX 2TLE LinearityTable.txt", m_sResultFolderPath);
                string sRX2THELTFilePath = string.Format(@"{0}\RX 2THE LinearityTable.txt", m_sResultFolderPath);
                string sTX5TLTFilePath = string.Format(@"{0}\TX 5T LinearityTable.txt", m_sResultFolderPath);
                string sTX3TLTFilePath = string.Format(@"{0}\TX 3T LinearityTable.txt", m_sResultFolderPath);
                string sTX2TLELTFilePath = string.Format(@"{0}\TX 2TLE LinearityTable.txt", m_sResultFolderPath);
                string sTX2THELTFilePath = string.Format(@"{0}\TX 2THE LinearityTable.txt", m_sResultFolderPath);
                string sTotalLTFilePath = string.Format(@"{0}\Total LinearityTable.txt", m_sResultFolderPath);

                int nLineCounter = 0;
                string sLine = "";

                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                int nRankIndex = -1;
                int nDrawTypeState = 0;

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

                    nRankIndex = m_cDataInfo_List[nFileIndex].m_nRankIndex;
                }
                else
                {
                    if (CheckAnalogParameterIsIdentical() == true)
                    {
                        bool bCheckFlag = false;

                        LinearityTuningParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2 && x.m_nRankIndex == cDataInfo.m_nRankIndex);

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

                        int nLineCount_Horizontal = m_cDrawTypeInfo_Array[Array.IndexOf(m_sDrawType_Array, StringConvert.m_sDRAWTYPE_HORIZONTAL)].m_nLineCount;
                        int nLineCount_Vertical = m_cDrawTypeInfo_Array[Array.IndexOf(m_sDrawType_Array, StringConvert.m_sDRAWTYPE_VERTICAL)].m_nLineCount;
                        int nLineIndex = nLineCounter - 1;

                        if (nLineCount_Horizontal < nLineCount_Vertical)
                        {
                            if (nLineIndex > nLineCount_Horizontal && nLineIndex < nLineCount_Vertical)
                                nDrawTypeState = 1;
                            else if (nLineIndex > nLineCount_Vertical)
                                nDrawTypeState = 2;
                        }
                        else
                        {
                            if (nLineIndex > nLineCount_Vertical && nLineIndex < nLineCount_Horizontal)
                                nDrawTypeState = 2;
                            else if (nLineIndex > nLineCount_Horizontal)
                                nDrawTypeState = 1;
                        }

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
                            byte[] byteData_Array = new byte[m_nNormalReportDataLength];

                            for (int nIndex = 0; nIndex < m_nNormalReportDataLength; nIndex++)
                                byteData_Array[nIndex] = Convert.ToByte(sSplit_Array[nIndex], 16);

                            bool bErrorFormatFlag = false;

                            if (m_cDataInfo_List[nFileIndex].m_n5TRawDataType != 1)
                            {
                                if (byteData_Array[0] != 0x07 ||
                                    byteData_Array[2] != 0xFF || 
                                    byteData_Array[3] != 0xFF ||
                                    byteData_Array[4] != 0xFF || 
                                    byteData_Array[5] != 0xFF ||
                                    byteData_Array[9] != 0x17)
                                    bErrorFormatFlag = true;
                            }
                            else
                            {
                                if (byteData_Array[0] != 0x07)
                                    bErrorFormatFlag = true;
                            }

                            if (bErrorFormatFlag == true)
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

                            if (m_cDataInfo_List[nFileIndex].m_n5TRawDataType == 1)
                            {
                                if (byteData_Array[2] != 0x11)
                                    continue;
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

                        if (nDrawTypeState == 1)
                            m_byteHorizontalOriginalData_List.Add(new List<byte>(m_byteReport_List));
                        else if (nDrawTypeState == 2)
                            m_byteVerticalOriginalData_List.Add(new List<byte>(m_byteReport_List));
                    }
                }
                finally
                {
                    srFile.Close();
                }
                #endregion

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                m_nRXTraceNumber = m_cDataInfo_List[nFileIndex].m_nRXTraceNumber;
                m_nTXTraceNumber = m_cDataInfo_List[nFileIndex].m_nTXTraceNumber;

                if (m_byteHorizontalOriginalData_List.Count == 0)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0020;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Horizontal Report Data({0}) In {1} File!",  Convert.ToString(m_byteHorizontalOriginalData_List.Count), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("No Horizontal Report Data({0})", Convert.ToString(m_byteHorizontalOriginalData_List.Count));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();

                    break;
                }
                else if (m_byteVerticalOriginalData_List.Count == 0)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0040;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Vertical Report Data({0}) In {1} File!", Convert.ToString(m_byteVerticalOriginalData_List.Count), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("No Vertical Report Data({0})", Convert.ToString(m_byteVerticalOriginalData_List.Count));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();

                    break;
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                CheckState cCheckState = new CheckState(m_cfrmMain);

                if (cCheckState.CheckIndependentStep(m_eMainStep, m_eSubStep) != CheckState.m_nSTEPSTATE_INDEPENDENT)
                {
                    if (ParamAutoTuning.m_nLTUseTP_GainCompensate != MainConstantParameter.m_nLTCOMPENSATE_DISABLE)
                    {
                        if (File.Exists(m_sReferenceFilePath) == true)
                        {
                            if (GetTPGTCompensateData(nFileIndex) == false)
                            {
                                cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0400;

                                m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get TP_Gain Compensate Data Error In {0} File!", Path.GetFileName(sFilePath));
                                m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get TP_Gain Compensate Data Error In {0} File", Path.GetFileName(sFilePath));
                                m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                                m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                                m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                                cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                                break;
                            }
                        }
                        else
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0200;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("\"{0}\" File Not Exist!", SpecificText.m_sReferenceFileName);
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("\"{0}\" File Not Exist", SpecificText.m_sReferenceFileName);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                            break;
                        }
                    }
                }

                sMessage = "";

                for (int nTraceTypeIndex = 0; nTraceTypeIndex < m_sTraceType_Array.Length; nTraceTypeIndex++)
                {
                    sMessage = "";
                    string sTraceType = m_sTraceType_Array[nTraceTypeIndex];
                    List<List<byte>> byteOriginalData_List = new List<List<byte>>();

                    if (sTraceType == m_sTRACETYPE_RX)
                        byteOriginalData_List = m_byteHorizontalOriginalData_List;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        byteOriginalData_List = m_byteVerticalOriginalData_List;

                    for (int nLTDataTypeIndex = 0; nLTDataTypeIndex < m_sLTDataType_Array.Length; nLTDataTypeIndex++)
                    {
                        string sLTDataType = m_sLTDataType_Array[nLTDataTypeIndex];
                        int nInvalidIndex = 0;

                        ComputeDEVAndWriteDEVDataFile(byteOriginalData_List, sLTDataType, sTraceType, sComputeFolderPath, nFileIndex);

                        int nErrorFlag = ComputeLTData(ref nInvalidIndex, byteOriginalData_List, nFileIndex, sLTDataType, sTraceType);

                        if (nErrorFlag != -1 && sMessage == "")
                        {
                            if (nErrorFlag == 1)
                                sMessage = string.Format("{0} {1} Type No Valid Trace Data", sLTDataType, sTraceType);
                            else if (nErrorFlag == 2)
                                sMessage = string.Format("{0} {1} Type No Enough Valid Trace Data", sLTDataType, sTraceType);
                            /*
                            else if (nErrorFlag == 3)
                                sMessage = string.Format("{0} {1} Type No Enough Valid Data in Tr:{2}", sLTDataType, sTraceType, nInvalidIndex);
                            */

                            continue;
                        }

                        ComputeAverageLTData(nFileIndex, sLTDataType, sTraceType);

                        long[] lLTData_Array = cDataValue.m_cLTData_5T.m_lRXLTData_Array;
                        string sLTFilePath = sRX5TLTFilePath;

                        switch (sLTDataType)
                        {
                            case m_sLTDATATYPE_5T:
                                if (sTraceType == m_sTRACETYPE_RX)
                                {
                                    lLTData_Array = cDataValue.m_cLTData_5T.m_lRXLTData_Array;
                                    sLTFilePath = sRX5TLTFilePath;
                                }
                                else if (sTraceType == m_sTRACETYPE_TX)
                                {
                                    lLTData_Array = cDataValue.m_cLTData_5T.m_lTXLTData_Array;
                                    sLTFilePath = sTX5TLTFilePath;
                                }

                                break;
                            case m_sLTDATATYPE_3T:
                                if (sTraceType == m_sTRACETYPE_RX)
                                {
                                    lLTData_Array = cDataValue.m_cLTData_3T.m_lRXLTData_Array;
                                    sLTFilePath = sRX3TLTFilePath;
                                }
                                else if (sTraceType == m_sTRACETYPE_TX)
                                {
                                    lLTData_Array = cDataValue.m_cLTData_3T.m_lTXLTData_Array;
                                    sLTFilePath = sTX3TLTFilePath;
                                }

                                break;
                            case m_sLTDATATYPE_2TLE:
                                if (sTraceType == m_sTRACETYPE_RX)
                                {
                                    lLTData_Array = cDataValue.m_cLTData_2TLE.m_lRXLTData_Array;
                                    sLTFilePath = sRX2TLELTFilePath;
                                }
                                else if (sTraceType == m_sTRACETYPE_TX)
                                {
                                    lLTData_Array = cDataValue.m_cLTData_2TLE.m_lTXLTData_Array;
                                    sLTFilePath = sTX2TLELTFilePath;
                                }

                                break;
                            case m_sLTDATATYPE_2THE:
                                if (sTraceType == m_sTRACETYPE_RX)
                                {
                                    lLTData_Array = cDataValue.m_cLTData_2THE.m_lRXLTData_Array;
                                    sLTFilePath = sRX2THELTFilePath;
                                }
                                else if (sTraceType == m_sTRACETYPE_TX)
                                {
                                    lLTData_Array = cDataValue.m_cLTData_2THE.m_lTXLTData_Array;
                                    sLTFilePath = sTX2THELTFilePath;
                                }

                                break;
                            default:
                                break;
                        }

                        WriteLTDataFile(lLTData_Array, sLTFilePath);
                    }
                }

                WriteTotalLTDataFile(nFileIndex, sTotalLTFilePath);

                if (sMessage != "")
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0080;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", cDataInfo.m_sRecordErrorMessage, Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = cDataInfo.m_sRecordErrorMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    break;
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                if (cDataInfo.m_sRecordErrorCode != "")
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0100;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", cDataInfo.m_sRecordErrorMessage, Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = cDataInfo.m_sRecordErrorMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteIntegrationDataFile(m_sIntegrationFilePath, m_nRXTraceNumber, m_nTXTraceNumber, cDataValue.m_nInvalidDataCount_Horizontal, cDataValue.m_nInvalidDataCount_Vertical, m_nErrorFlag);

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

        private bool GetTPGTCompensateData(int nFileIndex)
        {
            string sLine = "";
            int nRXTraceNumber = 0;
            int nTXTraceNumber = 0;

            StreamReader srFile = new StreamReader(m_sReferenceFilePath, Encoding.Default);

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSplit_Array = sLine.Split(new char[1] { ',' });

                    if (sSplit_Array.Length == 2)
                    {
                        if (sSplit_Array[0] == SpecificText.m_sRXTraceNumber)
                            nRXTraceNumber = Convert.ToInt32(sSplit_Array[1]);
                        else if (sSplit_Array[0] == SpecificText.m_sTXTraceNumber)
                            nTXTraceNumber = Convert.ToInt32(sSplit_Array[1]);
                    }
                }
            }
            finally
            {
                srFile.Close();
            }

            DataTable datatableRXData = null;
            DataTable datatableTXData = null;

            if (ParamAutoTuning.m_nLTUseTP_GainCompensate == MainConstantParameter.m_nLTCOMPENSATE_TIPGAIN)
            {
                datatableRXData = GetCSVFileData(m_eMainStep, m_eSubStep, "RX Tip Gain Data", m_sReferenceFilePath, "RXTipGainData", ",", true);
                datatableTXData = GetCSVFileData(m_eMainStep, m_eSubStep, "TX Tip Gain Data", m_sReferenceFilePath, "TXTipGainData", ",", false);
            }
            else if (ParamAutoTuning.m_nLTUseTP_GainCompensate == MainConstantParameter.m_nLTCOMPENSATE_RINGGAIN)
            {
                datatableRXData = GetCSVFileData(m_eMainStep, m_eSubStep, "RX Tip FWGain After TPGain Data", m_sReferenceFilePath, "RXTipGainData", ",", true);
                datatableTXData = GetCSVFileData(m_eMainStep, m_eSubStep, "TX Tip FWGain After TPGain Data", m_sReferenceFilePath, "TXTipGainData", ",", false);
            }

            if (datatableRXData == null || datatableTXData == null)
                return false;

            for (int nRowIndex = 0; nRowIndex < datatableRXData.Rows.Count; nRowIndex++)
            {
                int nPH1Value = Convert.ToInt32(datatableRXData.Rows[nRowIndex][SpecificText.m_sPH1]);
                int nPH2Value = Convert.ToInt32(datatableRXData.Rows[nRowIndex][SpecificText.m_sPH2]);

                if (m_cDataInfo_List[nFileIndex].m_nReadPH1 == nPH1Value && m_cDataInfo_List[nFileIndex].m_nReadPH2 == nPH2Value)
                {
                    m_cDataValue_List[nFileIndex].m_dRXGain_Array = new double[nRXTraceNumber];

                    for (int nTraceIndex = 1; nTraceIndex <= nRXTraceNumber; nTraceIndex++)
                    {
                        string sTraceIndex = Convert.ToString(nTraceIndex);
                        double dValue = Convert.ToDouble(datatableRXData.Rows[nRowIndex][sTraceIndex]);
                        int nValue = (int)Math.Round(dValue, 0, MidpointRounding.AwayFromZero);
                        nValue = (nValue < 1) ? 1 : nValue;
                        m_cDataValue_List[nFileIndex].m_dRXGain_Array[nTraceIndex - 1] = nValue;
                    }
                }
            }

            for (int nRowIndex = 0; nRowIndex < datatableTXData.Rows.Count; nRowIndex++)
            {
                int nPH1Value = Convert.ToInt32(datatableTXData.Rows[nRowIndex][SpecificText.m_sPH1]);
                int nPH2Value = Convert.ToInt32(datatableTXData.Rows[nRowIndex][SpecificText.m_sPH2]);

                if (m_cDataInfo_List[nFileIndex].m_nReadPH1 == nPH1Value && m_cDataInfo_List[nFileIndex].m_nReadPH2 == nPH2Value)
                {
                    m_cDataValue_List[nFileIndex].m_dTXGain_Array = new double[nTXTraceNumber];

                    for (int nTraceIndex = 1; nTraceIndex <= nTXTraceNumber; nTraceIndex++)
                    {
                        string sTraceIndex = Convert.ToString(nTraceIndex);
                        double dValue = Convert.ToDouble(datatableTXData.Rows[nRowIndex][sTraceIndex]);
                        int nValue = (int)Math.Round(dValue, 0, MidpointRounding.AwayFromZero);
                        nValue = (nValue < 1) ? 1 : nValue;
                        m_cDataValue_List[nFileIndex].m_dTXGain_Array[nTraceIndex - 1] = nValue;
                    }
                }
            }

            return true;
        }

        protected override void GetFileInfoFromReportData(DataInfo cDataInfo, string sFilePath)
        {
            long lGetInfoFlag = 0;
            string sLine = "";

            DrawTypeInfo cDrawTypeInfo = null;
            int nTotalInfoFlag = 0;

            cDataInfo.m_sFileName = Path.GetFileName(sFilePath);

            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            string sDrawLineState = "";
            int nLineCount = 0;

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    if (sLine == string.Format("====={0}=====", StringConvert.m_sDRAWTYPE_HORIZONTAL))
                    {
                        sDrawLineState = StringConvert.m_sDRAWTYPE_HORIZONTAL;
                        cDrawTypeInfo = new DrawTypeInfo();
                        cDrawTypeInfo.m_nLineCount = nLineCount;
                        lGetInfoFlag = 0;
                    }
                    else if (sLine == string.Format("====={0}=====", StringConvert.m_sDRAWTYPE_VERTICAL))
                    {
                        sDrawLineState = StringConvert.m_sDRAWTYPE_VERTICAL;
                        cDrawTypeInfo = new DrawTypeInfo();
                        cDrawTypeInfo.m_nLineCount = nLineCount;
                        lGetInfoFlag = 0;
                    }

                    GetDrawTypeInfo(ref lGetInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_SUBSTEP, sLine, 0x000001, m_nINFOTYPE_TUNINGSTEP);
                    GetDrawTypeInfo(ref lGetInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_SETTINGPH1, sLine, 0x000002, m_nINFOTYPE_INT);
                    GetDrawTypeInfo(ref lGetInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_SETTINGPH2, sLine, 0x000004, m_nINFOTYPE_INT);
                    GetDrawTypeInfo(ref lGetInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_READPH1, sLine, 0x000008, m_nINFOTYPE_INT);
                    GetDrawTypeInfo(ref lGetInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_READPH2, sLine, 0x000010, m_nINFOTYPE_INT);
                    GetDrawTypeInfo(ref lGetInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_FREQUENCY, sLine, 0x000020, m_nINFOTYPE_DOUBLE);
                    GetDrawTypeInfo(ref lGetInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_RANKINDEX, sLine, 0x000040, m_nINFOTYPE_INT);
                    GetDrawTypeInfo(ref lGetInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_RXTRACENUMBER, sLine, 0x000080, m_nINFOTYPE_INT);
                    GetDrawTypeInfo(ref lGetInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_TXTRACENUMBER, sLine, 0x000100, m_nINFOTYPE_INT);
                    GetDrawTypeInfo(ref lGetInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_5TRAWDATATYPE, sLine, 0x000200, m_nINFOTYPE_INT);

                    if (lGetInfoFlag == 0x0003FF)
                    {
                        if (sDrawLineState == StringConvert.m_sDRAWTYPE_HORIZONTAL)
                        {
                            int nDrawTypeIndex = Array.IndexOf(m_sDrawType_Array, sDrawLineState);
                            m_cDrawTypeInfo_Array[nDrawTypeIndex] = cDrawTypeInfo;
                            nTotalInfoFlag |= 0x01;
                        }
                        else if (sDrawLineState == StringConvert.m_sDRAWTYPE_VERTICAL)
                        {
                            int nDrawTypeIndex = Array.IndexOf(m_sDrawType_Array, sDrawLineState);
                            m_cDrawTypeInfo_Array[nDrawTypeIndex] = cDrawTypeInfo;
                            nTotalInfoFlag |= 0x02;
                        }
                    }

                    if (nTotalInfoFlag == 0x03)
                        break;

                    nLineCount++;
                }
            }
            finally
            {
                srFile.Close();
            }
        }

        protected void GetDrawTypeInfo(ref long lGetInfoFlag, DrawTypeInfo cDrawTypeInfo, string sParameterName, string sLine, long lInfoFlag, int nValueType)
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
                                SubTuningStep eSubStep;
                                try
                                {
                                    eSubStep = (SubTuningStep)Enum.Parse(typeof(SubTuningStep), sValue);
                                }
                                catch
                                {
                                    eSubStep = SubTuningStep.ELSE;
                                }

                                if (sParameterName == StringConvert.m_sRECORD_SUBSTEP)
                                    cDrawTypeInfo.m_eSubStep = eSubStep;

                                break;
                            case m_nINFOTYPE_INT:
                                int nValue = 0;

                                if (sParameterName == StringConvert.m_sRECORD_RANKINDEX ||
                                    sParameterName == StringConvert.m_sRECORD_5TRAWDATATYPE)
                                {
                                    if (ElanConvert.CheckIsInt(sValue) == true)
                                        nValue = Convert.ToInt32(sValue);
                                }
                                else
                                {
                                    if (sValue.Length == 2 && ElanConvert.CheckIsInt(sValue) == true)
                                        nValue = Convert.ToInt32(sValue);
                                }

                                if (sParameterName == StringConvert.m_sRECORD_SETTINGPH1)
                                    cDrawTypeInfo.m_nSettingPH1 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_SETTINGPH2)
                                    cDrawTypeInfo.m_nSettingPH2 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_READPH1)
                                    cDrawTypeInfo.m_nReadPH1 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_READPH2)
                                    cDrawTypeInfo.m_nReadPH2 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RANKINDEX)
                                    cDrawTypeInfo.m_nRankIndex = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RXTRACENUMBER)
                                    cDrawTypeInfo.m_nRXTraceNumber = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_TXTRACENUMBER)
                                    cDrawTypeInfo.m_nTXTraceNumber = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_5TRAWDATATYPE)
                                    cDrawTypeInfo.m_n5TRawDataType = nValue;

                                break;
                            case m_nINFOTYPE_DOUBLE:
                                double dValue = 0;

                                if (Double.TryParse(sValue, out dValue) == true)
                                {
                                    if (sParameterName == StringConvert.m_sRECORD_FREQUENCY)
                                        cDrawTypeInfo.m_dFrequency = dValue;
                                }

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

        protected override bool CheckInfoIsCorrect(ref string sErrorMessage, DataInfo cDataInfo)
        {
            int nHorizontalDrawTypeIndex = Array.IndexOf(m_sDrawType_Array, StringConvert.m_sDRAWTYPE_HORIZONTAL);
            int nVerticalDrawTypeIndex = Array.IndexOf(m_sDrawType_Array, StringConvert.m_sDRAWTYPE_VERTICAL);

            if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_eSubStep != m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_eSubStep)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor & Ver \"{0}\" Format Not Match", StringConvert.m_sRECORD_SUBSTEP));
            else
                cDataInfo.m_eSubStep = m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_eSubStep;

            if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nSettingPH1 != m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_nSettingPH1)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor & Ver \"{0}\" Format Not Match", StringConvert.m_sRECORD_SETTINGPH1));
            else
                cDataInfo.m_nSettingPH1 = m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nSettingPH1;

            if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nReadPH1 != m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_nReadPH1)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor & Ver \"{0}\" Format Not Match", StringConvert.m_sRECORD_READPH1));
            else
                cDataInfo.m_nReadPH1 = m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nReadPH1;

            if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nSettingPH2 != m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_nSettingPH2)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor & Ver \"{0}\" Format Not Match", StringConvert.m_sRECORD_SETTINGPH2));
            else
                cDataInfo.m_nSettingPH2 = m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nSettingPH2;

            if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nReadPH2 != m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_nReadPH2)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor & Ver \"{0}\" Format Not Match", StringConvert.m_sRECORD_READPH2));
            else
                cDataInfo.m_nReadPH2 = m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nReadPH2;

            if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_dFrequency != m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_dFrequency)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor & Ver \"{0}\" Format Not Match", StringConvert.m_sRECORD_FREQUENCY));
            else
                cDataInfo.m_dFrequency = m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_dFrequency;

            CheckGeneralInfoIsCorrect(ref sErrorMessage, cDataInfo);

            if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nRankIndex != m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_nRankIndex)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor & Ver \"{0}\" Format Not Match", StringConvert.m_sRECORD_RANKINDEX));
            else
            {
                cDataInfo.m_nRankIndex = m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nRankIndex;

                if (cDataInfo.m_nRankIndex < 0)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RANKINDEX));
            }

            if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nRXTraceNumber <= 0)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor \"{0}\" Value Error", StringConvert.m_sRECORD_RXTRACENUMBER));
            else if (m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_nRXTraceNumber <= 0)
                SetErrorMessage(ref sErrorMessage, string.Format("Ver \"{0}\" Value Error", StringConvert.m_sRECORD_RXTRACENUMBER));
            else if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nRXTraceNumber != m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_nRXTraceNumber)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor & Ver \"{0}\" Format Not Match", StringConvert.m_sRECORD_RXTRACENUMBER));
            else
                cDataInfo.m_nRXTraceNumber = m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nRXTraceNumber;

            if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nTXTraceNumber <= 0)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor \"{0}\" Value Error", StringConvert.m_sRECORD_TXTRACENUMBER));
            else if (m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_nTXTraceNumber <= 0)
                SetErrorMessage(ref sErrorMessage, string.Format("Ver \"{0}\" Value Error", StringConvert.m_sRECORD_TXTRACENUMBER));
            else if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nTXTraceNumber != m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_nTXTraceNumber)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor & Ver \"{0}\" Format Not Match", StringConvert.m_sRECORD_TXTRACENUMBER));
            else
                cDataInfo.m_nTXTraceNumber = m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_nTXTraceNumber;

            if (m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_n5TRawDataType != m_cDrawTypeInfo_Array[nVerticalDrawTypeIndex].m_n5TRawDataType)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor & Ver \"{0}\" Format Not Match", StringConvert.m_sRECORD_5TRAWDATATYPE));
            else
                cDataInfo.m_n5TRawDataType = m_cDrawTypeInfo_Array[nHorizontalDrawTypeIndex].m_n5TRawDataType;

            if (sErrorMessage != "")
                return false;

            return true;
        }

        private void WriteLTDataFile(long[] lLTData_Array, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nValueIndex = 0; nValueIndex < lLTData_Array.Length; nValueIndex++)
                {
                    if (nValueIndex % 8 == 0)
                        sw.Write(".DW		".ToString());

                    sw.Write(lLTData_Array[nValueIndex].ToString("0000"));

                    if (nValueIndex % 8 == 7)
                        sw.WriteLine();
                    else
                        sw.Write(", ");
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WriteTotalLTDataFile(int nFileIndex, string sFilePath)
        {
            string sNoteText = "";
            string sLTTitleName = "";
            long[] lLTData_Array = null;

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nLTDataTypeIndex = 0; nLTDataTypeIndex < m_sLTDataType_Array.Length; nLTDataTypeIndex++)
                {
                    string sLTDataType = m_sLTDataType_Array[nLTDataTypeIndex];

                    for (int nTraceTypeIndex = 0; nTraceTypeIndex < m_sTraceType_Array.Length; nTraceTypeIndex++)
                    {
                        string sTraceType = m_sTraceType_Array[nTraceTypeIndex];

                        switch (sLTDataType)
                        {
                            case m_sLTDATATYPE_5T:
                                if (sTraceType == m_sTRACETYPE_RX)
                                {
                                    lLTData_Array = m_cDataValue_List[nFileIndex].m_cLTData_5T.m_lRXLTData_Array;
                                    sLTTitleName = "ActivePen_Position_XMAP";
                                    sNoteText = "//5T XMAP";
                                }
                                else if (sTraceType == m_sTRACETYPE_TX)
                                {
                                    lLTData_Array = m_cDataValue_List[nFileIndex].m_cLTData_5T.m_lTXLTData_Array;
                                    sLTTitleName = "ActivePen_Position_YMAP";
                                    sNoteText = "//5T YMAP";
                                }

                                break;
                            case m_sLTDATATYPE_3T:
                                if (sTraceType == m_sTRACETYPE_RX)
                                {
                                    lLTData_Array = m_cDataValue_List[nFileIndex].m_cLTData_3T.m_lRXLTData_Array;
                                    sLTTitleName = "ActivePen_Position_XMAP";
                                    sNoteText = "//3T XMAP";
                                }
                                else if (sTraceType == m_sTRACETYPE_TX)
                                {
                                    lLTData_Array = m_cDataValue_List[nFileIndex].m_cLTData_3T.m_lTXLTData_Array;
                                    sLTTitleName = "ActivePen_Position_YMAP";
                                    sNoteText = "//3T YMAP";
                                }

                                break;
                            case m_sLTDATATYPE_2TLE:
                                if (sTraceType == m_sTRACETYPE_RX)
                                {
                                    lLTData_Array = m_cDataValue_List[nFileIndex].m_cLTData_2TLE.m_lRXLTData_Array;
                                    sLTTitleName = "ActivePen_Position_Edge2T_XMAP_LE";
                                    sNoteText = "//2T XMAP";
                                }
                                else if (sTraceType == m_sTRACETYPE_TX)
                                {
                                    lLTData_Array = m_cDataValue_List[nFileIndex].m_cLTData_2TLE.m_lTXLTData_Array;
                                    sLTTitleName = "ActivePen_Position_Edge2T_YMAP_LE";
                                    sNoteText = "//2T YMAP";
                                }

                                break;
                            case m_sLTDATATYPE_2THE:
                                if (sTraceType == m_sTRACETYPE_RX)
                                {
                                    lLTData_Array = m_cDataValue_List[nFileIndex].m_cLTData_2THE.m_lRXLTData_Array;
                                    sLTTitleName = "ActivePen_Position_Edge2T_YMAP_HE";
                                    sNoteText = "//2T XMAP";
                                }
                                else if (sTraceType == m_sTRACETYPE_TX)
                                {
                                    lLTData_Array = m_cDataValue_List[nFileIndex].m_cLTData_2THE.m_lTXLTData_Array;
                                    sLTTitleName = "ActivePen_Position_Edge2T_YMAP_HE";
                                    sNoteText = "//2T YMAP";
                                }

                                break;
                            default:
                                break;
                        }

                        sw.WriteLine(sNoteText);
                        sw.WriteLine(string.Format("{0}:", sLTTitleName));

                        if (lLTData_Array != null)
                        {
                            for (int nValueIndex = 0; nValueIndex < lLTData_Array.Length; nValueIndex++)
                            {
                                if (nValueIndex % 8 == 0)
                                    sw.Write(".DW		".ToString());

                                sw.Write(lLTData_Array[nValueIndex].ToString("0000"));

                                if (nValueIndex % 8 == 7)
                                    sw.WriteLine();
                                else
                                    sw.Write(", ");
                            }
                        }

                        if (!(nLTDataTypeIndex == m_sLTDataType_Array.Length - 1 && nTraceTypeIndex == m_sTraceType_Array.Length - 1))
                            sw.WriteLine();
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

        private void WriteIntegrationDataFile(string sFilePath, int nRXTraceNumber, int nTXTraceNumber, int nHorizontalInvalidReportCount, int nVerticalInvalidReportCount, int nErrorFlag)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_INTEGRATION);

                sw.WriteLine();

                sw.WriteLine("Information");
                sw.WriteLine(string.Format("RX Trace Number,{0}", nRXTraceNumber));
                sw.WriteLine(string.Format("TX Trace Number,{0}", nRXTraceNumber));
                sw.WriteLine(string.Format("Horizontal Invalid Report Count,{0}", nHorizontalInvalidReportCount));
                sw.WriteLine(string.Format("Vertical Invalid Report Count,{0}", nVerticalInvalidReportCount));
                sw.WriteLine(string.Format("ErrorFlag,{0}", nErrorFlag));
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
            string[] sValueTypeName_Array = new string[9] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sFlowStep, 
                SpecificText.m_sSettingPH1, 
                SpecificText.m_sSettingPH2, 
                SpecificText.m_sReadPH1, 
                SpecificText.m_sReadPH2, 
                SpecificText.m_sFrequency, 
                SpecificText.m_sErrorMessage 
            };

            string[] sFWParameterTypeName_Array = new string[6] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                SpecificText.m_sFrequency, 
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
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));

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

        public void ComputeAndOutputResult()
        {
            OutputResultData();
        }

        private void OutputResultData()
        {
            WriteStepListDataFile(m_sProjectName, m_sStepListPath);

            if (m_bErrorFlag == false)
            {
                FileProcess cFileProcess = new FileProcess(m_cfrmMain);
                cFileProcess.WriteResultListTxtFile(m_eSubStep, m_nSubStepState);
            }

            OutputMessage("Analysis Complete!!");
        }

        private void ComputeDEVAndWriteDEVDataFile(List<List<byte>> byteOriginalData_List, string sLTDataType, string sTraceType, string sFolderPath, int nFileIndex)
        {
            int n5TracesByteLocation = m_nBYTELOCATION_RX5TRACES;
            //int nDEVByteLocation = m_nBYTELOCATION_RXDEV;
            int nTraceIndexByteLocation = m_nBYTELOCATION_RXTRACEINDEX;
            int nTraceNumber = m_nRXTraceNumber;
            double[] dGain_Array = m_cDataValue_List[nFileIndex].m_dRXGain_Array;

            if (m_cDataInfo_List[nFileIndex].m_n5TRawDataType != 1)
            {
                if (sTraceType == m_sTRACETYPE_RX)
                {
                    n5TracesByteLocation = m_nBYTELOCATION_RX5TRACES;
                    //nDEVByteLocation = m_nBYTELOCATION_RXDEV;
                    nTraceIndexByteLocation = m_nBYTELOCATION_RXTRACEINDEX;
                    nTraceNumber = m_nRXTraceNumber;
                    dGain_Array = m_cDataValue_List[nFileIndex].m_dRXGain_Array;
                }
                else if (sTraceType == m_sTRACETYPE_TX)
                {
                    n5TracesByteLocation = m_nBYTELOCATION_TX5TRACES;
                    //nDEVByteLocation = m_nBYTELOCATION_TXDEV;
                    nTraceIndexByteLocation = m_nBYTELOCATION_TXTRACEINDEX;
                    nTraceNumber = m_nTXTraceNumber;
                    dGain_Array = m_cDataValue_List[nFileIndex].m_dTXGain_Array;
                }
            }
            else
            {
                if (sTraceType == m_sTRACETYPE_RX)
                {
                    n5TracesByteLocation = m_nBYTELOCATION_RX5TRACES_NEWFORMAT;
                    //nDEVByteLocation = m_nBYTELOCATION_RXDEV_NEWFORMAT;
                    nTraceIndexByteLocation = m_nBYTELOCATION_RXTRACEINDEX_NEWFORMAT;
                    nTraceNumber = m_nRXTraceNumber;
                    dGain_Array = m_cDataValue_List[nFileIndex].m_dRXGain_Array;
                }
                else if (sTraceType == m_sTRACETYPE_TX)
                {
                    n5TracesByteLocation = m_nBYTELOCATION_TX5TRACES_NEWFORMAT;
                    //nDEVByteLocation = m_nBYTELOCATION_TXDEV_NEWFORMAT;
                    nTraceIndexByteLocation = m_nBYTELOCATION_TXTRACEINDEX_NEWFORMAT;
                    nTraceNumber = m_nTXTraceNumber;
                    dGain_Array = m_cDataValue_List[nFileIndex].m_dTXGain_Array;
                }
            }

            List<List<int>> nRawData_List = new List<List<int>>();
            List<int> nDevData_List = new List<int>();
            List<int> nTraceIndex_List = new List<int>();
            List<byte> byteHighByte_List = new List<byte>();
            List<byte> byteLowByte_List = new List<byte>();

            for (int nDataIndex = 0; nDataIndex < byteOriginalData_List.Count; nDataIndex++)
            {
                int nTraceIndex = byteOriginalData_List[nDataIndex][nTraceIndexByteLocation - 1];
                nTraceIndex_List.Add(nTraceIndex);

                int nValue_1T = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation] << 8) +
                                (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation - 1]);
                int nValue_2T = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 2] << 8) +
                                (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 2 - 1]);
                int nValue_3T = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 4] << 8) +
                                (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 4 - 1]);
                int nValue_4T = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 6] << 8) +
                                (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 6 - 1]);
                int nValue_5T = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 8] << 8) +
                                (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 8 - 1]);

                int[] n5TData_Array = new int[5] 
                { 
                    0, 
                    0, 
                    0, 
                    0, 
                    0 
                };

                CheckState cCheckState = new CheckState(m_cfrmMain);

                if (cCheckState.CheckIndependentStep(m_eMainStep, m_eSubStep) != CheckState.m_nSTEPSTATE_INDEPENDENT)
                {
                    if (ParamAutoTuning.m_nLTUseTP_GainCompensate != MainConstantParameter.m_nLTCOMPENSATE_DISABLE)
                    {
                        if (nTraceIndex == 0)
                        {
                            nValue_3T = (int)Math.Round(nValue_3T * dGain_Array[nTraceIndex], 0, MidpointRounding.AwayFromZero);
                            nValue_4T = (int)Math.Round(nValue_4T * dGain_Array[nTraceIndex + 1], 0, MidpointRounding.AwayFromZero);
                            nValue_5T = (int)Math.Round(nValue_5T * dGain_Array[nTraceIndex + 2], 0, MidpointRounding.AwayFromZero);
                        }
                        else if (nTraceIndex == 1)
                        {
                            nValue_2T = (int)Math.Round(nValue_2T * dGain_Array[nTraceIndex - 1], 0, MidpointRounding.AwayFromZero);
                            nValue_3T = (int)Math.Round(nValue_3T * dGain_Array[nTraceIndex], 0, MidpointRounding.AwayFromZero);
                            nValue_4T = (int)Math.Round(nValue_4T * dGain_Array[nTraceIndex + 1], 0, MidpointRounding.AwayFromZero);
                            nValue_5T = (int)Math.Round(nValue_5T * dGain_Array[nTraceIndex + 2], 0, MidpointRounding.AwayFromZero);
                        }
                        else if (nTraceIndex == nTraceNumber - 1)
                        {
                            nValue_1T = (int)Math.Round(nValue_1T * dGain_Array[nTraceIndex - 2], 0, MidpointRounding.AwayFromZero);
                            nValue_2T = (int)Math.Round(nValue_2T * dGain_Array[nTraceIndex - 1], 0, MidpointRounding.AwayFromZero);
                            nValue_3T = (int)Math.Round(nValue_3T * dGain_Array[nTraceIndex], 0, MidpointRounding.AwayFromZero);
                        }
                        else if (nTraceIndex == nTraceNumber - 2)
                        {
                            nValue_1T = (int)Math.Round(nValue_1T * dGain_Array[nTraceIndex - 2], 0, MidpointRounding.AwayFromZero);
                            nValue_2T = (int)Math.Round(nValue_2T * dGain_Array[nTraceIndex - 1], 0, MidpointRounding.AwayFromZero);
                            nValue_3T = (int)Math.Round(nValue_3T * dGain_Array[nTraceIndex], 0, MidpointRounding.AwayFromZero);
                            nValue_4T = (int)Math.Round(nValue_4T * dGain_Array[nTraceIndex + 1], 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            nValue_1T = (int)Math.Round(nValue_1T * dGain_Array[nTraceIndex - 2], 0, MidpointRounding.AwayFromZero);
                            nValue_2T = (int)Math.Round(nValue_2T * dGain_Array[nTraceIndex - 1], 0, MidpointRounding.AwayFromZero);
                            nValue_3T = (int)Math.Round(nValue_3T * dGain_Array[nTraceIndex], 0, MidpointRounding.AwayFromZero);
                            nValue_4T = (int)Math.Round(nValue_4T * dGain_Array[nTraceIndex + 1], 0, MidpointRounding.AwayFromZero);
                            nValue_5T = (int)Math.Round(nValue_5T * dGain_Array[nTraceIndex + 2], 0, MidpointRounding.AwayFromZero);
                        }

                        n5TData_Array[0] = nValue_1T;
                        n5TData_Array[1] = nValue_2T;
                        n5TData_Array[2] = nValue_3T;
                        n5TData_Array[3] = nValue_4T;
                        n5TData_Array[4] = nValue_5T;
                    }
                }

                if (sLTDataType == m_sLTDATATYPE_5T)
                {
                    if (cCheckState.CheckIndependentStep(m_eMainStep, m_eSubStep) != CheckState.m_nSTEPSTATE_INDEPENDENT &&
                        ParamAutoTuning.m_nLTUseTP_GainCompensate != MainConstantParameter.m_nLTCOMPENSATE_DISABLE)
                    {
                        int nDEVValue = Compute5TDeviation(n5TData_Array, nTraceIndex, nTraceNumber);
                        nDevData_List.Add(nDEVValue);

                        byte byteLB = (byte)(nDEVValue & 0x00FF);
                        byteLowByte_List.Add(byteLB);
                        byte byteHB = (byte)((nDEVValue & 0xFF00) >> 8);
                        byteHighByte_List.Add(byteHB);
                    }
                    else
                    {
                        //Get Deviation By Report Data
                        /*
                        int nDEVRawValue = (int)(byteOriginalData_List[nDataIndex][nDEVByteLocation] << 8) + (int)(byteOriginalData_List[nDataIndex][nDEVByteLocation - 1]);
                        int nDEVValue = (nDEVRawValue < m_n64PARTS_NEGATIVEDATA) ? nDEVRawValue : (-(0x10000 - nDEVRawValue));
                        nDevData_List.Add(nDEVValue);
                        */

                        int nDEVValue = Compute5TDeviation(n5TData_Array, nTraceIndex, nTraceNumber);
                        nDevData_List.Add(nDEVValue);

                        byte byteLB = (byte)(nDEVValue & 0x00FF);
                        byteLowByte_List.Add(byteLB);
                        byte byteHB = (byte)((nDEVValue & 0xFF00) >> 8);
                        byteHighByte_List.Add(byteHB);
                    }
                }
                else
                {
                    /*
                    int nCValue = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 2] << 8) +
                                  (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 2 - 1]);
                    //nCValue = (nCValue < NEGATIVEDATA_PART) ? nCValue : (-(0x10000 - nCValue));
                    int nRValue = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 4] << 8) +
                                  (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 4 - 1]);
                    //nRValue = (nRValue < NEGATIVEDATA_PART) ? nRValue : (-(0x10000 - nRValue));
                    int nMValue = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 6] << 8) +
                                  (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 6 - 1]);
                    //nMValue = (nMValue < NEGATIVEDATA_PART) ? nMValue : (-(0x10000 - nMValue));
                    */

                    int nCValue = nValue_2T;
                    int nRValue = nValue_3T;
                    int nMValue = nValue_4T;

                    int nDEVValue = 0;

                    switch (sLTDataType)
                    {
                        case m_sLTDATATYPE_3T:
                            if (nCValue + nRValue + nMValue == 0)
                                nDEVValue = 0;
                            else
                                nDEVValue = (int)((-2048 * nCValue + 2048 * nMValue) / (nCValue + nRValue + nMValue));
                            break;
                        case m_sLTDATATYPE_2TLE:
                            if (nRValue + nMValue == 0)
                                nDEVValue = 1024;
                            else
                                nDEVValue = (int)((-2048 * nRValue + 2048 * nMValue) / (nRValue + nMValue)) + 1024;
                            break;
                        case m_sLTDATATYPE_2THE:
                            if (nCValue + nRValue == 0)
                                nDEVValue = -1024;
                            else
                                nDEVValue = (int)((-2048 * nCValue + 2048 * nRValue) / (nCValue + nRValue)) - 1024;
                            break;
                        default:
                            break;
                    }

                    nDEVValue = (nDEVValue < m_n64PARTS_NEGATIVEDATA) ? nDEVValue : (-(0x10000 - nDEVValue));
                    nDevData_List.Add(nDEVValue);

                    byte byteLB = (byte)(nDEVValue & 0x00FF);
                    byteLowByte_List.Add(byteLB);
                    byte byteHB = (byte)((nDEVValue & 0xFF00) >> 8);
                    byteHighByte_List.Add(byteHB);
                }

                List<int> nSingleRawData_List = new List<int>();
                nSingleRawData_List.Add(nValue_1T);
                nSingleRawData_List.Add(nValue_2T);
                nSingleRawData_List.Add(nValue_3T);
                nSingleRawData_List.Add(nValue_4T);
                nSingleRawData_List.Add(nValue_5T);
                nRawData_List.Add(nSingleRawData_List);
            }

            string[] sColumnName_Array = new string[] 
            { 
                "Trace Index",
                string.Format("{0}1", sTraceType),
                string.Format("{0}2", sTraceType),
                string.Format("{0}3", sTraceType),
                string.Format("{0}4", sTraceType),
                string.Format("{0}5", sTraceType),
                "Deviation", 
                "LowByte", 
                "HighByte" 
            };

            string sFilePath = string.Format(@"{0}\{1}_{2}_Data.csv", sFolderPath, sTraceType, sLTDataType);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nColumnIndex = 0; nColumnIndex < sColumnName_Array.Length; nColumnIndex++)
                {
                    if (nColumnIndex < sColumnName_Array.Length - 1)
                        sw.Write(string.Format("{0},", sColumnName_Array[nColumnIndex]));
                    else
                        sw.WriteLine(sColumnName_Array[nColumnIndex]);
                }

                int nDataLength = nRawData_List.Count;

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    string sText = "";

                    sText += string.Format("{0},", nTraceIndex_List[nDataIndex]);

                    for (int nTraceIndex = 0; nTraceIndex < nRawData_List[nDataIndex].Count; nTraceIndex++)
                        sText += string.Format("{0},", nRawData_List[nDataIndex][nTraceIndex]);

                    sText += string.Format("{0},", nDevData_List[nDataIndex]);
                    sText += string.Format("{0},", ConvertByteToHexString(byteLowByte_List[nDataIndex]));
                    sText += string.Format("{0},", ConvertByteToHexString(byteHighByte_List[nDataIndex]));

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

        private int Compute5TDeviation(int[] nRaw5TData_Array, int nTraceIndex, int nTraceNumber)
        {
            #region NoTable Deviation
            /*
            if (nTraceIndex == 0)
            {
                int nValue3 = (int)(nRaw5TData_Array[2] / 4);
                int nValue4 = (int)(nRaw5TData_Array[3] / 4);

                int nDeviation = (int)((-2048 * nValue3 + 2048 * nValue4) / (nValue3 + nValue4)) + 1024;

                return nDeviation;
            }
            else if (nTraceIndex == nTraceNumber - 1)
            {
                int nValue2 = (int)(nRaw5TData_Array[1] / 4);
                int nValue3 = (int)(nRaw5TData_Array[2] / 4);

                int nDeviation = (int)((-2048 * nValue2 + 2048 * nValue3) / (nValue2 + nValue3)) - 1024;

                return nDeviation;
            }
            else
            {
                int nValue1 = (int)(nRaw5TData_Array[0] / 4);
                int nValue2 = (int)(nRaw5TData_Array[1] / 4);
                int nValue3 = (int)(nRaw5TData_Array[2] / 4);
                int nValue4 = (int)(nRaw5TData_Array[3] / 4);
                int nValue5 = (int)(nRaw5TData_Array[4] / 4);

                int nDiffer = nValue4 - nValue2;
                int nSum = nValue2 + nValue3 + nValue4;
                double dRatio = (double)nDiffer / nSum;
                int nDeviation = (int)(dRatio * 2048);

                return nDeviation;
            }
            */
            #endregion

            int nValue1 = nRaw5TData_Array[0];
            int nValue2 = nRaw5TData_Array[1];
            int nValue3 = nRaw5TData_Array[2];
            int nValue4 = nRaw5TData_Array[3];
            int nValue5 = nRaw5TData_Array[4];

            int nDeviation = (-2048 * nValue1 + -1024 * nValue2 + 1024 * nValue4 + 2048 * nValue5) / (nValue1 + nValue2 + nValue3 + nValue4 + nValue5);

            return nDeviation;
        }

        private string ConvertByteToHexString(byte byteValue)
        {
            StringBuilder sb = new StringBuilder(10);
            sb.AppendFormat("{0:X2} ", byteValue);

            return sb.ToString();
        }

        private int ComputeLTData(ref int nInvalidIndex, List<List<byte>> byteOriginalData_List, int nFileIndex, string sLTDataType, string sTraceType)
        {
            int n5TracesByteLocation = m_nBYTELOCATION_RX5TRACES;
            int nDEVByteLocation = m_nBYTELOCATION_RXDEV;
            int nTraceIndexByteLocation = m_nBYTELOCATION_RXTRACEINDEX;
            int nTraceNumber = m_nRXTraceNumber;

            if (m_cDataInfo_List[nFileIndex].m_n5TRawDataType != 1)
            {
                if (sTraceType == m_sTRACETYPE_RX)
                {
                    n5TracesByteLocation = m_nBYTELOCATION_RX5TRACES;
                    nDEVByteLocation = m_nBYTELOCATION_RXDEV;
                    nTraceIndexByteLocation = m_nBYTELOCATION_RXTRACEINDEX;
                    nTraceNumber = m_nRXTraceNumber;
                }
                else if (sTraceType == m_sTRACETYPE_TX)
                {
                    n5TracesByteLocation = m_nBYTELOCATION_TX5TRACES;
                    nDEVByteLocation = m_nBYTELOCATION_TXDEV;
                    nTraceIndexByteLocation = m_nBYTELOCATION_TXTRACEINDEX;
                    nTraceNumber = m_nTXTraceNumber;
                }
            }
            else
            {
                if (sTraceType == m_sTRACETYPE_RX)
                {
                    n5TracesByteLocation = m_nBYTELOCATION_RX5TRACES_NEWFORMAT;
                    nDEVByteLocation = m_nBYTELOCATION_RXDEV_NEWFORMAT;
                    nTraceIndexByteLocation = m_nBYTELOCATION_RXTRACEINDEX_NEWFORMAT;
                    nTraceNumber = m_nRXTraceNumber;
                }
                else if (sTraceType == m_sTRACETYPE_TX)
                {
                    n5TracesByteLocation = m_nBYTELOCATION_TX5TRACES_NEWFORMAT;
                    nDEVByteLocation = m_nBYTELOCATION_TXDEV_NEWFORMAT;
                    nTraceIndexByteLocation = m_nBYTELOCATION_TXTRACEINDEX_NEWFORMAT;
                    nTraceNumber = m_nTXTraceNumber;
                }
            }

            List<TracePartData> cTracePartData_List = new List<TracePartData>();

            int nMaxTraceIndex = -1;
            int nInvalidDataCount = 0;

            for (int nDataIndex = 0; nDataIndex < byteOriginalData_List.Count; nDataIndex++)
            {
                if (byteOriginalData_List[nDataIndex][nTraceIndexByteLocation - 1] == 0xFF)
                {
                    nInvalidDataCount++;
                    continue;
                }

                int nTraceIndex = byteOriginalData_List[nDataIndex][nTraceIndexByteLocation - 1];

                if (nTraceIndex > nMaxTraceIndex)
                    nMaxTraceIndex = nTraceIndex;
            }

            if (sLTDataType == m_sLTDATATYPE_5T)
            {
                if (sTraceType == m_sTRACETYPE_RX)
                    m_cDataValue_List[nFileIndex].m_nInvalidDataCount_Horizontal = nInvalidDataCount;
                else if (sTraceType == m_sTRACETYPE_TX)
                    m_cDataValue_List[nFileIndex].m_nInvalidDataCount_Vertical = nInvalidDataCount;
            }
                    
            for (int nTraceIndex = 0; nTraceIndex < nMaxTraceIndex + 1; nTraceIndex++)
            {
                TracePartData cTracePartData = new TracePartData();
                cTracePartData.m_nTraceIndex = -1;
                Array.Clear(cTracePartData.m_lTracePartData_Array, 0, cTracePartData.m_lTracePartData_Array.Length);

                cTracePartData_List.Add(cTracePartData);
            }

            if (sLTDataType == m_sLTDATATYPE_5T)
            {
                for (int nDataIndex = 0; nDataIndex < byteOriginalData_List.Count; nDataIndex++)
                {
                    if (byteOriginalData_List[nDataIndex][nTraceIndexByteLocation - 1] == 0xFF)
                        continue;

                    int nTraceIndex = byteOriginalData_List[nDataIndex][nTraceIndexByteLocation - 1];

                    if (cTracePartData_List[nTraceIndex].m_nTraceIndex < 0)
                        cTracePartData_List[nTraceIndex].m_nTraceIndex = nTraceIndex;

                    int nDEVRawValue = (int)(byteOriginalData_List[nDataIndex][nDEVByteLocation] << 8) + (int)(byteOriginalData_List[nDataIndex][nDEVByteLocation - 1]);
                    int nDEVValue = (nDEVRawValue < m_n64PARTS_NEGATIVEDATA) ? nDEVRawValue : (-(0x10000 - nDEVRawValue));
                    cTracePartData_List[nTraceIndex].m_nTraceRawData_List.Add(nDEVValue);
                }
            }
            else
            {
                for (int nDataIndex = 0; nDataIndex < byteOriginalData_List.Count; nDataIndex++)
                {
                    int nCValue = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 2] << 8) +
                                  (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 2 - 1]);
                    //nCValue = (nCValue < NEGATIVEDATA_PART) ? nCValue : (-(0x10000 - nCValue));
                    int nRValue = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 4] << 8) +
                                  (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 4 - 1]);
                    //nRValue = (nRValue < NEGATIVEDATA_PART) ? nRValue : (-(0x10000 - nRValue));
                    int nMValue = (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 6] << 8) +
                                  (int)(byteOriginalData_List[nDataIndex][n5TracesByteLocation + 6 - 1]);
                    //nMValue = (nMValue < NEGATIVEDATA_PART) ? nMValue : (-(0x10000 - nMValue));

                    int nTraceIndex = byteOriginalData_List[nDataIndex][nTraceIndexByteLocation - 1];

                    if (cTracePartData_List[nTraceIndex].m_nTraceIndex < 0)
                        cTracePartData_List[nTraceIndex].m_nTraceIndex = nTraceIndex;

                    int nDEVValue = 0;

                    switch(sLTDataType)
                    {
                        case m_sLTDATATYPE_3T:
                            if (nCValue + nRValue + nMValue == 0)
                                continue;

                            nDEVValue = (int)((-2048 * nCValue + 2048 * nMValue) / (nCValue + nRValue + nMValue));
                            //cTracePartData_List[nTraceIndex].m_nTraceRawData_List.Add(nDEVValue);
                            break;
                        case m_sLTDATATYPE_2TLE:
                            if (nRValue + nMValue == 0)
                                continue;

                            nDEVValue = (int)((-2048 * nRValue + 2048 * nMValue) / (nRValue + nMValue)) + 1024;
                            //cTracePartData_List[nTraceIndex].m_nTraceRawData_List.Add(nDEVValue);
                            break;
                        case m_sLTDATATYPE_2THE:
                            if (nCValue + nRValue == 0)
                                continue;

                            nDEVValue = (int)((-2048 * nCValue + 2048 * nRValue) / (nCValue + nRValue)) - 1024;
                            //cTracePartData_List[nTraceIndex].m_nTraceRawData_List.Add(nDEVValue);
                            break;
                        default:
                            break;
                    }

                    nDEVValue = (nDEVValue < m_n64PARTS_NEGATIVEDATA) ? nDEVValue : (-(0x10000 - nDEVValue));
                    cTracePartData_List[nTraceIndex].m_nTraceRawData_List.Add(nDEVValue);
                }
            }

            for (int nTraceIndex = nMaxTraceIndex; nTraceIndex >= 0; nTraceIndex--)
            {
                if (cTracePartData_List[nTraceIndex].m_nTraceIndex < 0)
                {
                    cTracePartData_List.RemoveAt(nTraceIndex);
                    continue;
                }

                cTracePartData_List[nTraceIndex].m_nTraceRawData_List.Sort();

                long[] l64PartDataMean_Array = new long[m_n64PARTS_SIZE];
                int[] n64PartDataCount_Array = new int[m_n64PARTS_SIZE];
                Array.Clear(l64PartDataMean_Array, 0, l64PartDataMean_Array.Length);
                Array.Clear(n64PartDataCount_Array, 0, n64PartDataCount_Array.Length);

                int nModValue = (int)(cTracePartData_List[nTraceIndex].m_nTraceRawData_List.Count / m_n64PARTS_SIZE);
                int nRemValue = (int)(cTracePartData_List[nTraceIndex].m_nTraceRawData_List.Count % m_n64PARTS_SIZE);

                for (int nSizeIndex = 0; nSizeIndex < m_n64PARTS_SIZE; nSizeIndex++)
                    n64PartDataCount_Array[nSizeIndex] = nModValue;

                if (nRemValue > 0)
                {
                    for (int nRemValueIndex = 0; nRemValueIndex < nRemValue; nRemValueIndex++)
                        n64PartDataCount_Array[nRemValueIndex] += 1;
                }

                int nStartIndex = 0;

                for (int nSizeIndex = 0; nSizeIndex < m_n64PARTS_SIZE; nSizeIndex++)
                {
                    for (int nArrayIndex = nStartIndex; nArrayIndex < nStartIndex + n64PartDataCount_Array[nSizeIndex]; nArrayIndex++)
                        l64PartDataMean_Array[nSizeIndex] += cTracePartData_List[nTraceIndex].m_nTraceRawData_List[nArrayIndex];

                    nStartIndex += n64PartDataCount_Array[nSizeIndex];
                }

                for (int nSizeIndex = 0; nSizeIndex < m_n64PARTS_SIZE; nSizeIndex++)
                {
                    if (n64PartDataCount_Array[nSizeIndex] == 0)
                        cTracePartData_List[nTraceIndex].m_lTracePartData_Array[nSizeIndex] = 0;
                    else
                        cTracePartData_List[nTraceIndex].m_lTracePartData_Array[nSizeIndex] = (long)Math.Round((float)l64PartDataMean_Array[nSizeIndex] / (float)n64PartDataCount_Array[nSizeIndex]) + m_n64PARTS_BASEVALUE;
                }
            }

            if (cTracePartData_List == null || cTracePartData_List.Count == 0)
                return 1;
            else
            {
                int nFirstEdgeAreaNumber = ComputeEdgeAreaNumber(nTraceNumber, true);
                int nLastEdgeAreaNumber = ComputeEdgeAreaNumber(nTraceNumber, false);
                int nCount = 0;

                for (int nDataIndex = 0; nDataIndex < cTracePartData_List.Count; nDataIndex++)
                {
                    if (cTracePartData_List[nDataIndex].m_nTraceIndex > nFirstEdgeAreaNumber && cTracePartData_List[nDataIndex].m_nTraceIndex < nLastEdgeAreaNumber)
                        nCount++;
                }

                if (nCount == 0)
                    return 2;

                /*
                for (int nDataIndex = 0; nDataIndex < cTracePartData_List.Count; nDataIndex++)
                {
                    if (cTracePartData_List[nDataIndex].m_nTraceIndex > nFirstEdgeAreaNumber && cTracePartData_List[nDataIndex].m_nTraceIndex < nLastEdgeAreaNumber)
                    {
                        if (cTracePartData_List[nDataIndex].m_nValidDataCount < m_n64PARTS_SIZE)
                        {
                            nInvalidIndex = nDataIndex;
                            return 3;
                        }
                    }
                }
                */
            }

            switch (sLTDataType)
            {
                case m_sLTDATATYPE_5T:
                    if (sTraceType == m_sTRACETYPE_RX)
                        m_cDataValue_List[nFileIndex].m_cLTData_5T.m_cRXTracePartData_List = cTracePartData_List;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        m_cDataValue_List[nFileIndex].m_cLTData_5T.m_cTXTracePartData_List = cTracePartData_List;
                    break;
                case m_sLTDATATYPE_3T:
                    if (sTraceType == m_sTRACETYPE_RX)
                        m_cDataValue_List[nFileIndex].m_cLTData_3T.m_cRXTracePartData_List = cTracePartData_List;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        m_cDataValue_List[nFileIndex].m_cLTData_3T.m_cTXTracePartData_List = cTracePartData_List;
                    break;
                case m_sLTDATATYPE_2TLE:
                    if (sTraceType == m_sTRACETYPE_RX)
                        m_cDataValue_List[nFileIndex].m_cLTData_2TLE.m_cRXTracePartData_List = cTracePartData_List;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        m_cDataValue_List[nFileIndex].m_cLTData_2TLE.m_cTXTracePartData_List = cTracePartData_List;
                    break;
                case m_sLTDATATYPE_2THE:
                    if (sTraceType == m_sTRACETYPE_RX)
                        m_cDataValue_List[nFileIndex].m_cLTData_2THE.m_cRXTracePartData_List = cTracePartData_List;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        m_cDataValue_List[nFileIndex].m_cLTData_2THE.m_cTXTracePartData_List = cTracePartData_List;
                    break;
                default:
                    break;
            }

            return -1;
        }

        private void ComputeAverageLTData(int nFileIndex, string sLTDataType, string sTraceType)
        {
            List<TracePartData> cTracePartData_List = new List<TracePartData>();
            long[] lAverageLTData_Array = new long[m_n64PARTS_SIZE];
            Array.Clear(lAverageLTData_Array, 0, lAverageLTData_Array.Length);

            int nTraceNumber = m_nRXTraceNumber;

            if (sTraceType == m_sTRACETYPE_RX)
                nTraceNumber = m_nRXTraceNumber;
            else if (sTraceType == m_sTRACETYPE_TX)
                nTraceNumber = m_nTXTraceNumber;

            switch(sLTDataType)
            {
                case m_sLTDATATYPE_5T:
                    if (sTraceType == m_sTRACETYPE_RX)
                        cTracePartData_List = m_cDataValue_List[nFileIndex].m_cLTData_5T.m_cRXTracePartData_List;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        cTracePartData_List = m_cDataValue_List[nFileIndex].m_cLTData_5T.m_cTXTracePartData_List;

                    break;
                case m_sLTDATATYPE_3T:
                    if (sTraceType == m_sTRACETYPE_RX)
                        cTracePartData_List = m_cDataValue_List[nFileIndex].m_cLTData_3T.m_cRXTracePartData_List;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        cTracePartData_List = m_cDataValue_List[nFileIndex].m_cLTData_3T.m_cTXTracePartData_List;

                    break;
                case m_sLTDATATYPE_2TLE:
                    if (sTraceType == m_sTRACETYPE_RX)
                        cTracePartData_List = m_cDataValue_List[nFileIndex].m_cLTData_2TLE.m_cRXTracePartData_List;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        cTracePartData_List = m_cDataValue_List[nFileIndex].m_cLTData_2TLE.m_cTXTracePartData_List;

                    break;
                case m_sLTDATATYPE_2THE:
                    if (sTraceType == m_sTRACETYPE_RX)
                        cTracePartData_List = m_cDataValue_List[nFileIndex].m_cLTData_2THE.m_cRXTracePartData_List;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        cTracePartData_List = m_cDataValue_List[nFileIndex].m_cLTData_2THE.m_cTXTracePartData_List;

                    break;
                default:
                    break;
            }

            int nFirstEdgeAreaNumber = ComputeEdgeAreaNumber(nTraceNumber, true);
            int nLastEdgeAreaNumber = ComputeEdgeAreaNumber(nTraceNumber, false);

            for (int nSizeIndex = 0; nSizeIndex < m_n64PARTS_SIZE; nSizeIndex++)
            {
                int nCount = 0;
                long lSumValue = 0;

                for (int nDataIndex = 0; nDataIndex < cTracePartData_List.Count; nDataIndex++)
                {
                    if (cTracePartData_List[nDataIndex].m_nTraceIndex > nFirstEdgeAreaNumber && cTracePartData_List[nDataIndex].m_nTraceIndex < nLastEdgeAreaNumber)
                    {
                        lSumValue += cTracePartData_List[nDataIndex].m_lTracePartData_Array[nSizeIndex];
                        nCount++;
                    }
                }

                lAverageLTData_Array[nSizeIndex] = lSumValue / nCount;
            }

            switch(sLTDataType)
            {
                case m_sLTDATATYPE_5T:
                    if (sTraceType == m_sTRACETYPE_RX)
                        m_cDataValue_List[nFileIndex].m_cLTData_5T.m_lRXLTData_Array = lAverageLTData_Array;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        m_cDataValue_List[nFileIndex].m_cLTData_5T.m_lTXLTData_Array = lAverageLTData_Array;

                    break;
                case m_sLTDATATYPE_3T:
                    if (sTraceType == m_sTRACETYPE_RX)
                        m_cDataValue_List[nFileIndex].m_cLTData_3T.m_lRXLTData_Array = lAverageLTData_Array;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        m_cDataValue_List[nFileIndex].m_cLTData_3T.m_lTXLTData_Array = lAverageLTData_Array;

                    break;
                case m_sLTDATATYPE_2TLE:
                    if (sTraceType == m_sTRACETYPE_RX)
                        m_cDataValue_List[nFileIndex].m_cLTData_2TLE.m_lRXLTData_Array = lAverageLTData_Array;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        m_cDataValue_List[nFileIndex].m_cLTData_2TLE.m_lTXLTData_Array = lAverageLTData_Array;

                    break;
                case m_sLTDATATYPE_2THE:
                    if (sTraceType == m_sTRACETYPE_RX)
                        m_cDataValue_List[nFileIndex].m_cLTData_2THE.m_lRXLTData_Array = lAverageLTData_Array;
                    else if (sTraceType == m_sTRACETYPE_TX)
                        m_cDataValue_List[nFileIndex].m_cLTData_2THE.m_lTXLTData_Array = lAverageLTData_Array;

                    break;
                default:
                    break;
            }
        }

        private int ComputeEdgeAreaNumber(int nTraceNumber, bool bFirstEdgeAreaFlag)
        {
            int nEdgeAreaNumber = 0;

            if (bFirstEdgeAreaFlag == true)
                nEdgeAreaNumber = (1 < m_nFirstEdgeAreaValidNumber) ? m_nFirstEdgeAreaValidNumber - 1 : 1 - 1;
            else
                nEdgeAreaNumber = (nTraceNumber - 1 > nTraceNumber - m_nLastEdgeAreaValidNumber) ? nTraceNumber - m_nLastEdgeAreaValidNumber : nTraceNumber - 1;

            return nEdgeAreaNumber;
        }
    }
}
