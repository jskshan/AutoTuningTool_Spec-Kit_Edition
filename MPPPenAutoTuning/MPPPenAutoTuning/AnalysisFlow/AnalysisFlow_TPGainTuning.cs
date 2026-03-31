using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    class AnalysisFlow_TPGainTuning : AnalysisFlow
    {
        private const int m_nDRAWTYPECOUNT = 2;

        private int m_nDataTypeByteLocation = 3;

        private int m_nRXValidReportNumber = 1500;
        private int m_nTXValidReportNumber = 1500;

        private int m_nTXStartPin = 0;
        private int m_nTXEndPin = 47;
        private int m_nRXStartPin = 0;
        private int m_nRXEndPin = 83;
        private int m_nGainRatio = 8192;
        private string m_sICGenerationType = "Gen6"; //"Gen6", "Gen7", "Gen6308"

        private string[] m_sDrawType_Array = new string[m_nDRAWTYPECOUNT] 
        { 
            StringConvert.m_sDRAWTYPE_HORIZONTAL,
            StringConvert.m_sDRAWTYPE_VERTICAL 
        };

        private bool m_bWriteTxS3Flag = false;

        private int m_nTXOffset = 0;
        private int m_nRXOffset = 0;
        private bool[] m_bTXTracePinFlag_Array = new bool[48];
        private bool[] m_bRXTracePinFlag_Array = new bool[84];

        private string m_sTPGainTableFolderPath = "";

        private List<byte> m_byteReport_List = new List<byte>();
        private List<List<byte>> m_byteData_List = new List<List<byte>>();

        public class Raw5TData
        {
            public int m_nTraceIndex = -1;

            public int m_nRawData_1T = 0;
            public int m_nRawData_2T = 0;
            public int m_nRawData_3T = 0;
            public int m_nRawData_4T = 0;
            public int m_nRawData_5T = 0;
        }

        public class TPGainValue
        {
            public long[,] m_lMaxPowerTip_Array = new long[101, 11];
            public long[,] m_lMaxPowerRing_Array = new long[101, 11];

            public int m_nTraceCntTip = 0;
            public int m_nTraceCntRing = 0;

            public double[] m_dAvgPowerTip_Array = new double[101];
            public double[] m_dAvgPowerRing_Array = new double[101];
            public double[] m_dAvgPowerTip_S3_Array = new double[101];
            public double[] m_dAvgPowerRing_S3_Array = new double[101];
            public double[] m_dAvgGainTip_Array = new double[101];
            public double[] m_dAvgGainRing_Array = new double[101];
            public double[] m_dAvgGainTip_S3_Array = new double[101];
            public double[] m_dAvgGainRing_S3_Array = new double[101];

            public int[] m_nTipPowerAfterTPGain_Array = new int[101];
            public double[] m_dTipFWGainAfterTPGain_Array = new double[101];
            public long[] m_lTipFWGain_Array = new long[101];
        }

        public class DataValue
        {
            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public TPGainValue m_cRXTPGainValue = new TPGainValue();
            public TPGainValue m_cTXTPGainValue = new TPGainValue();

            public List<Raw5TData> m_cRXRaw5TData_BHFTip_List = new List<Raw5TData>();
            public List<Raw5TData> m_cRXRaw5TData_BHFRing_List = new List<Raw5TData>();
            public List<Raw5TData> m_cTXRaw5TData_BHFTip_List = new List<Raw5TData>();
            public List<Raw5TData> m_cTXRaw5TData_BHFRing_List = new List<Raw5TData>();
        }

        private List<DataValue> m_cDataValue_List = null;

        public class DrawTypeInfo
        {
            public SubTuningStep eSubStep = SubTuningStep.ELSE;
            public int nSettingPH1 = -1;
            public int nSettingPH2 = -1;
            public int nReadPH1 = -1;
            public int nReadPH2 = -1;
            public double dFrequency = -1;

            public int nRankIndex = -1;

            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public int nLineCount = -1;
        }

        private DrawTypeInfo[] m_cDrawTypeInfo_Array = null;

        private void ClearDataArray()
        {
            m_byteReport_List.Clear();
            m_byteData_List.Clear();
        }

        public AnalysisFlow_TPGainTuning(FlowStep cFlowStep, frmMain cfrmMain)
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
            m_nRXValidReportNumber = ParamAutoTuning.m_nTPGTRXValidReportNumber;
            m_nTXValidReportNumber = ParamAutoTuning.m_nTPGTTXValidReportNumber;

            m_nTXStartPin = ParamAutoTuning.m_nTPGTTXStartPin;
            m_nTXEndPin = ParamAutoTuning.m_nTPGTTXEndPin;
            m_nRXStartPin = ParamAutoTuning.m_nTPGTRXStartPin;
            m_nRXEndPin = ParamAutoTuning.m_nTPGTRXEndPin;
            m_nGainRatio = ParamAutoTuning.m_nTPGTGainRatio;
        }

        public void SetFileDirectory()
        {
            m_sStepFolderPath = string.Format(@"{0}\{1}", m_sFileDirectoryPath, m_sSubStepName);
            m_sResultFolderPath = string.Format(@"{0}\{1}({2})", m_sFileDirectoryPath, SpecificText.m_sResultText, m_sSubStepCodeName);
            m_sFlowBackUpFolderPath = string.Format(@"{0}\{1}", m_sFileDirectoryPath, SpecificText.m_sFlowText);
            m_sProjectName = m_sRecordProjectName;
            m_sProjectFolderPath = m_sFileDirectoryPath;
            m_sStepListPath = string.Format(@"{0}\{1}_{2}.csv", m_sProjectFolderPath, SpecificText.m_sStepListText, m_sSubStepCodeName);
            m_sTPGainTableFolderPath = string.Format(@"{0}\TPGainTable", m_sResultFolderPath);

            m_sReferenceFolderPath = string.Format(@"{0}\{1}", m_sResultFolderPath, SpecificText.m_sReferenceText);
            m_sReferenceFilePath = string.Format(@"{0}\{1}", m_sReferenceFolderPath, SpecificText.m_sReferenceFileName);
        }

        public void CreateTPGainTableFolder()
        {
            if (Directory.Exists(m_sTPGainTableFolderPath) == false)
                Directory.CreateDirectory(m_sTPGainTableFolderPath);
        }

        public void SetRxTxValidPinAndOffset()
        {
            Array.Clear(m_bTXTracePinFlag_Array, 0, m_bTXTracePinFlag_Array.Length);

            if (m_nTXStartPin > m_nTXEndPin)
            {
                int nPin = m_nTXEndPin;
                m_nTXEndPin = m_nTXStartPin;
                m_nTXStartPin = nPin;
            }

            for (int nPinIndex = 0; nPinIndex < m_bTXTracePinFlag_Array.Length; nPinIndex++)
            {
                if (nPinIndex >= m_nTXStartPin && nPinIndex <= m_nTXEndPin)
                    m_bTXTracePinFlag_Array[nPinIndex] = true;
                else
                    m_bTXTracePinFlag_Array[nPinIndex] = false;
            }

            Array.Clear(m_bRXTracePinFlag_Array, 0, m_bRXTracePinFlag_Array.Length);

            if (m_nRXStartPin > m_nRXEndPin)
            {
                int nPin = m_nRXEndPin;
                m_nRXEndPin = m_nRXStartPin;
                m_nRXStartPin = nPin;
            }

            for (int nPinIndex = 0; nPinIndex < m_bRXTracePinFlag_Array.Length; nPinIndex++)
            {
                if (nPinIndex >= m_nRXStartPin && nPinIndex <= m_nRXEndPin)
                    m_bRXTracePinFlag_Array[nPinIndex] = true;
                else
                    m_bRXTracePinFlag_Array[nPinIndex] = false;
            }

            for (int nArrayIndex = 0; nArrayIndex < m_bTXTracePinFlag_Array.Length; nArrayIndex++)
            {
                if (m_bTXTracePinFlag_Array[nArrayIndex] == true)
                {
                    m_nTXOffset = nArrayIndex;
                    break;
                }
            }

            for (int nArrayIndex = 0; nArrayIndex < m_bRXTracePinFlag_Array.Length; nArrayIndex++)
            {
                if (m_bRXTracePinFlag_Array[nArrayIndex] == true)
                {
                    m_nRXOffset = nArrayIndex;
                    break;
                }
            }
        }

        public void GetData(List<TPGainTuningParameter> cParameter_List)
        {
            //取得資料夾內所有檔案
            string[] sValidDataFilePath_Array = GetValidReportDataFile(cParameter_List);

            if (sValidDataFilePath_Array == null || sValidDataFilePath_Array.Length == 0)
            {
                m_sErrorMessage = "No Valid Data!!";
                OutputMessage("No Valid Data!!");
                m_bErrorFlag = true;
                return;
            }

            int nFileCount = sValidDataFilePath_Array.Length;

            m_cfrmMain.OutputMainStatusStrip("Analysing...", 0, nFileCount, frmMain.m_nInitialFlag);

            foreach (string sFilePath in sValidDataFilePath_Array)
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
                string sRXRawFilePath = string.Format(@"{0}\RX Raw Data.csv", sProcessFolderPath);
                string sTXRawFilePath = string.Format(@"{0}\TX Raw Data.csv", sProcessFolderPath);
                string sProcessFilePath = string.Format(@"{0}\Process Data.csv", sProcessFolderPath);
                string sComputeFilePath = string.Format(@"{0}\Compute Data.csv", sComputeFolderPath);

                int nLineCounter = 0;
                string sLine = "";

                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                int nRankIndex = -1;
                int nDrawTypeState = 0;

                int nRXReportNumber = 0;
                int nTXReportNumber = 0;

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

                        TPGainTuningParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2);

                        if (cParameter != null)
                        {
                            if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_LOADDATA && cParameter.m_nRankIndex != cDataInfo.m_nRankIndex)
                                continue;

                            if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_LOADDATA)
                                nRankIndex = cParameter.m_nRankIndex;
                            else
                                nRankIndex = cDataInfo.m_nRankIndex;

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

                        if (ParamAutoTuning.m_nDGTDrawType == 1)
                        {
                            int nLineCount_Horizontal = m_cDrawTypeInfo_Array[Array.IndexOf(m_sDrawType_Array, StringConvert.m_sDRAWTYPE_HORIZONTAL)].nLineCount;
                            int nLineCount_Vertical = m_cDrawTypeInfo_Array[Array.IndexOf(m_sDrawType_Array, StringConvert.m_sDRAWTYPE_VERTICAL)].nLineCount;
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
                        }

                        string[] sSplit_Array = sLine.Split(m_charDELIMITER);

                        if (sSplit_Array.Length != m_nReportDataLength + 1)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0010;

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

                            if (byteData_Array[0] != 0x07)
                            {
                                cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0020;

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
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0020;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("Report Data Format Error In {0} File!", Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("Report Data Format Error In Line {0}", nLineCounter);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();

                            srFile.Close();
                            break;
                        }

                        if (Convert.ToByte(sSplit_Array[m_nDataTypeByteLocation - 1], 16) != 0x23)
                            continue;

                        for (int nIndex = 0; nIndex < sSplit_Array.Length - 1; nIndex++)
                        {
                            byte byteRawData = Convert.ToByte(sSplit_Array[nIndex], 16);
                            m_byteReport_List.Add(byteRawData);
                        }

                        m_byteData_List.Add(new List<byte>(m_byteReport_List));

                        if (nDrawTypeState == 1)
                            nRXReportNumber++;
                        else if (nDrawTypeState == 2)
                            nTXReportNumber++;
                    }
                }
                finally
                {
                    srFile.Close();
                }
                #endregion

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                int nDataCount = m_byteData_List.Count;

                for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
                {
                    int nTraceType = MainConstantParameter.m_nTRACETYPE_RX;

                    if (nDataIndex < nRXReportNumber)
                        nTraceType = MainConstantParameter.m_nTRACETYPE_RX;
                    else if (nDataIndex >= nRXReportNumber && nDataIndex < nRXReportNumber + nTXReportNumber)
                        nTraceType = MainConstantParameter.m_nTRACETYPE_TX;
                    else
                        break;

                    Raw5TData cRaw5TData_Tip = new Raw5TData();
                    Raw5TData cRaw5TData_Ring = new Raw5TData();

                    int nDataType = m_byteData_List[nDataIndex][m_nDataTypeByteLocation - 1];

                    GetRaw5TData(cRaw5TData_Tip, cRaw5TData_Ring, m_byteData_List[nDataIndex], nTraceType);

                    if (nDataType == 0x23)
                    {
                        if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                        {
                            cDataValue.m_cRXRaw5TData_BHFTip_List.Add(cRaw5TData_Tip);
                            cDataValue.m_cRXRaw5TData_BHFRing_List.Add(cRaw5TData_Ring);
                        }
                        else if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                        {
                            cDataValue.m_cTXRaw5TData_BHFTip_List.Add(cRaw5TData_Tip);
                            cDataValue.m_cTXRaw5TData_BHFRing_List.Add(cRaw5TData_Ring);
                        }
                    }
                }

                WriteRaw5TDataFile("RX BHF 5T Raw Data", nFileIndex, sRXRawFilePath, true);
                WriteRaw5TDataFile("TX BHF 5T Raw Data", nFileIndex, sTXRawFilePath, false);

                if (CheckDataNumberIsEnough(nFileIndex, sFilePath) == false)
                    continue;

                ComputeAverageGainAndPower(nFileIndex);

                WriteProcessDataFile(nFileIndex, sProcessFilePath);

                ComputeTipFWGain(nFileIndex);

                WriteComputeDataFile(nFileIndex, sComputeFilePath);

                CreateAndSaveTPGainTableFile(nFileIndex);

                if (cDataInfo.m_sRecordErrorCode != "")
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x1000;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", cDataInfo.m_sRecordErrorMessage, Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = cDataInfo.m_sRecordErrorMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteIntegrationDataFile(m_sIntegrationFilePath, m_nErrorFlag);

                m_nValidFileCount++;
                m_nFileCount++;

                OutputMessage(string.Format("Report Data({0}) Analysis Complete!!", Path.GetFileName(sFilePath)));
                m_cfrmMain.OutputMainStatusStrip(string.Format("Data Set : {0}", m_nFileCount), m_nFileCount);
            }

            #region Final Check
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
            #endregion
        }

        public void ComputeAndOutputResult()
        {
            OutputResultData();
        }

        protected override void GetFileInfoFromReportData(DataInfo cDataInfo, string sFilePath)
        {
            long lGetInfoFlag = 0;
            string sLine = "";

            long lGetDrawTypeInfoFlag = 0;
            int nDrawTypeInfoFlag = 0;
            DrawTypeInfo cDrawTypeInfo = null;
            string sDrawLineState = "";
            int nLineCount = 0;

            cDataInfo.m_sFileName = Path.GetFileName(sFilePath);

            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    if (ParamAutoTuning.m_nDGTDrawType == 1)
                    {
                        if (sLine == string.Format("====={0}=====", StringConvert.m_sDRAWTYPE_HORIZONTAL))
                        {
                            sDrawLineState = StringConvert.m_sDRAWTYPE_HORIZONTAL;
                            cDrawTypeInfo = new DrawTypeInfo();
                            cDrawTypeInfo.nLineCount = nLineCount;
                            lGetDrawTypeInfoFlag = 0;
                        }
                        else if (sLine == string.Format("====={0}=====", StringConvert.m_sDRAWTYPE_VERTICAL))
                        {
                            sDrawLineState = StringConvert.m_sDRAWTYPE_VERTICAL;
                            cDrawTypeInfo = new DrawTypeInfo();
                            cDrawTypeInfo.nLineCount = nLineCount;
                            lGetDrawTypeInfoFlag = 0;
                        }

                        if (cDrawTypeInfo != null)
                        {
                            GetDrawTypeInfo(ref lGetDrawTypeInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_SUBSTEP, sLine, 0x000001, m_nINFOTYPE_TUNINGSTEP);
                            GetDrawTypeInfo(ref lGetDrawTypeInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_SETTINGPH1, sLine, 0x000002, m_nINFOTYPE_INT);
                            GetDrawTypeInfo(ref lGetDrawTypeInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_SETTINGPH2, sLine, 0x000004, m_nINFOTYPE_INT);
                            GetDrawTypeInfo(ref lGetDrawTypeInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_READPH1, sLine, 0x000008, m_nINFOTYPE_INT);
                            GetDrawTypeInfo(ref lGetDrawTypeInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_READPH2, sLine, 0x000010, m_nINFOTYPE_INT);
                            GetDrawTypeInfo(ref lGetDrawTypeInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_FREQUENCY, sLine, 0x000020, m_nINFOTYPE_DOUBLE);
                            GetDrawTypeInfo(ref lGetDrawTypeInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_RANKINDEX, sLine, 0x000040, m_nINFOTYPE_INT);
                            GetDrawTypeInfo(ref lGetDrawTypeInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_RXTRACENUMBER, sLine, 0x000080, m_nINFOTYPE_INT);
                            GetDrawTypeInfo(ref lGetDrawTypeInfoFlag, cDrawTypeInfo, StringConvert.m_sRECORD_TXTRACENUMBER, sLine, 0x000100, m_nINFOTYPE_INT);
                        }

                        if (lGetDrawTypeInfoFlag == 0x0001FF)
                        {
                            if (sDrawLineState == StringConvert.m_sDRAWTYPE_HORIZONTAL)
                            {
                                int nDrawTypeIndex = Array.IndexOf(m_sDrawType_Array, sDrawLineState);
                                m_cDrawTypeInfo_Array[nDrawTypeIndex] = cDrawTypeInfo;
                                nDrawTypeInfoFlag |= 0x01;
                            }
                            else if (sDrawLineState == StringConvert.m_sDRAWTYPE_VERTICAL)
                            {
                                int nDrawTypeIndex = Array.IndexOf(m_sDrawType_Array, sDrawLineState);
                                m_cDrawTypeInfo_Array[nDrawTypeIndex] = cDrawTypeInfo;
                                nDrawTypeInfoFlag |= 0x02;
                            }
                        }

                        nLineCount++;

                        if (lGetInfoFlag == 0x1FFFF)
                            continue;
                    }

                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_SUBSTEP, sLine, 0x000001, m_nINFOTYPE_TUNINGSTEP);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_SETTINGPH1, sLine, 0x000002, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_SETTINGPH2, sLine, 0x000004, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_READPH1, sLine, 0x000008, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_READPH2, sLine, 0x000010, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_FREQUENCY, sLine, 0x000020, m_nINFOTYPE_DOUBLE);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RANKINDEX, sLine, 0x000040, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RXTRACENUMBER, sLine, 0x000080, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_TXTRACENUMBER, sLine, 0x000100, m_nINFOTYPE_INT);

                    if (ParamAutoTuning.m_nDGTDrawType != 1 && lGetInfoFlag == 0x1FFFF)
                        break;
                }
            }
            finally
            {
                srFile.Close();
            }
        }

        protected void GetFileInfo(ref long lGetInfoFlag, DataValue cDataValue, string sParameterName, string sLine, long lInfoFlag, int nValueType)
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
                                    cDrawTypeInfo.eSubStep = eSubStep;

                                break;
                            case m_nINFOTYPE_INT:
                                int nValue = 0;

                                if (sParameterName == StringConvert.m_sRECORD_RANKINDEX)
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
                                    cDrawTypeInfo.nSettingPH1 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_SETTINGPH2)
                                    cDrawTypeInfo.nSettingPH2 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_READPH1)
                                    cDrawTypeInfo.nReadPH1 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_READPH2)
                                    cDrawTypeInfo.nReadPH2 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RANKINDEX)
                                    cDrawTypeInfo.nRankIndex = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RXTRACENUMBER)
                                    cDrawTypeInfo.m_nRXTraceNumber = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_TXTRACENUMBER)
                                    cDrawTypeInfo.m_nTXTraceNumber = nValue;

                                break;
                            case m_nINFOTYPE_DOUBLE:
                                double dValue = 0;

                                if (Double.TryParse(sValue, out dValue) == true)
                                {
                                    if (sParameterName == StringConvert.m_sRECORD_FREQUENCY)
                                        cDrawTypeInfo.dFrequency = dValue;
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

        private void GetRaw5TData(Raw5TData cRaw5TData_Tip, Raw5TData cRaw5TData_Ring, List<byte> byteData_List, int nTraceType)
        {
            int nStartIndex = 1;
            int nEndIndex = 5;

            int nTip5TByteLocation = 4;
            int nTipTraceByteLocation = 16;
            int nRing5TByteLocation = 36;
            int nRingTraceByteLocation = 48;

            if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
            {
                nTip5TByteLocation = 4;
                nTipTraceByteLocation = 16;
                nRing5TByteLocation = 36;
                nRingTraceByteLocation = 48;
            }
            else if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
            {
                nTip5TByteLocation = 17;
                nTipTraceByteLocation = 29;
                nRing5TByteLocation = 49;
                nRingTraceByteLocation = 61;
            }

            int nTipTraceIndex = byteData_List[nTipTraceByteLocation - 1];
            int nRingTraceIndex = byteData_List[nRingTraceByteLocation - 1];
            cRaw5TData_Tip.m_nTraceIndex = nTipTraceIndex;
            cRaw5TData_Ring.m_nTraceIndex = nRingTraceIndex;

            //Tip
            for (int nTraceDataIndex = nStartIndex; nTraceDataIndex <= nEndIndex; nTraceDataIndex++)
            {
                int nValue = byteData_List[(nTip5TByteLocation - 1) + 2 * (nTraceDataIndex - 1)] + byteData_List[(nTip5TByteLocation - 1) + 2 * (nTraceDataIndex - 1) + 1] * 256;

                switch (nTraceDataIndex)
                {
                    case 1:
                        cRaw5TData_Tip.m_nRawData_1T = nValue;
                        break;
                    case 2:
                        cRaw5TData_Tip.m_nRawData_2T = nValue;
                        break;
                    case 3:
                        cRaw5TData_Tip.m_nRawData_3T = nValue;
                        break;
                    case 4:
                        cRaw5TData_Tip.m_nRawData_4T = nValue;
                        break;
                    case 5:
                        cRaw5TData_Tip.m_nRawData_5T = nValue;
                        break;
                }
            }

            //Ring
            for (int nTraceDataIndex = nStartIndex; nTraceDataIndex <= nEndIndex; nTraceDataIndex++)
            {
                int nValue = byteData_List[(nRing5TByteLocation - 1) + 2 * (nTraceDataIndex - 1)] + byteData_List[(nRing5TByteLocation - 1) + 2 * (nTraceDataIndex - 1) + 1] * 256;

                switch (nTraceDataIndex)
                {
                    case 1:
                        cRaw5TData_Ring.m_nRawData_1T = nValue;
                        break;
                    case 2:
                        cRaw5TData_Ring.m_nRawData_2T = nValue;
                        break;
                    case 3:
                        cRaw5TData_Ring.m_nRawData_3T = nValue;
                        break;
                    case 4:
                        cRaw5TData_Ring.m_nRawData_4T = nValue;
                        break;
                    case 5:
                        cRaw5TData_Ring.m_nRawData_5T = nValue;
                        break;
                }
            }
        }

        private bool CheckDataNumberIsEnough(int nFileIndex, string sFilePath)
        {
            List<Raw5TData>[] cRXRawDataList_Array = new List<Raw5TData>[] 
            { 
                m_cDataValue_List[nFileIndex].m_cRXRaw5TData_BHFTip_List,
                m_cDataValue_List[nFileIndex].m_cRXRaw5TData_BHFRing_List 
            };

            TPGainDataFormat.DataType[] eRXDataType_Array = new TPGainDataFormat.DataType[] 
            { 
                TPGainDataFormat.DataType.Rx_Tip,
                TPGainDataFormat.DataType.Rx_Ring 
            };

            for (int nDataIndex = 0; nDataIndex < cRXRawDataList_Array.Length; nDataIndex++)
            {
                if (cRXRawDataList_Array[nDataIndex].Count < m_nRXValidReportNumber || cRXRawDataList_Array[nDataIndex].Count == 0)
                {
                    m_cDataInfo_List[nFileIndex].m_nErrorFlag = m_nErrorFlag |= 0x0100;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get {0} Valid Data Too Few({1}<LB:{2}) In {3} File!", eRXDataType_Array[nDataIndex].ToString(), Convert.ToString(cRXRawDataList_Array[nDataIndex].Count), Convert.ToString(m_nRXValidReportNumber), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get {0} Valid Data Too Few({1}<LB:{2})", eRXDataType_Array[nDataIndex].ToString(), Convert.ToString(cRXRawDataList_Array[nDataIndex].Count), Convert.ToString(m_nRXValidReportNumber));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    m_cDataInfo_List[nFileIndex].m_sErrorMessage = RunProcessErrorFlow();
                    return false;
                }
            }

            List<Raw5TData>[] cTXRawDataList_Array = new List<Raw5TData>[] 
            { 
                m_cDataValue_List[nFileIndex].m_cTXRaw5TData_BHFTip_List,
                m_cDataValue_List[nFileIndex].m_cTXRaw5TData_BHFRing_List 
            };

            TPGainDataFormat.DataType[] eTXDataType_Array = new TPGainDataFormat.DataType[] 
            { 
                TPGainDataFormat.DataType.Tx_Tip,
                TPGainDataFormat.DataType.Tx_Ring 
            };

            for (int nDataIndex = 0; nDataIndex < cTXRawDataList_Array.Length; nDataIndex++)
            {
                if (cTXRawDataList_Array[nDataIndex].Count < m_nTXValidReportNumber || cTXRawDataList_Array[nDataIndex].Count == 0)
                {
                    m_cDataInfo_List[nFileIndex].m_nErrorFlag = m_nErrorFlag |= 0x0200;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get {0} Valid Data Too Few({1}<LB:{2}) In {3} File!", eTXDataType_Array[nDataIndex].ToString(), Convert.ToString(cTXRawDataList_Array[nDataIndex].Count), Convert.ToString(m_nTXValidReportNumber), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get {0} Valid Data Too Few({1}<LB:{2})", eTXDataType_Array[nDataIndex].ToString(), Convert.ToString(cTXRawDataList_Array[nDataIndex].Count), Convert.ToString(m_nTXValidReportNumber));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    m_cDataInfo_List[nFileIndex].m_sErrorMessage = RunProcessErrorFlow();
                    return false;
                }
            }

            return true;
        }

        private void ComputeAverageGainAndPower(int nFileIndex)
        {
            int[] nTraceType_Array = new int[] 
            { 
                MainConstantParameter.m_nTRACETYPE_RX, 
                MainConstantParameter.m_nTRACETYPE_TX 
            };

            foreach (int nTraceType in nTraceType_Array)
            {
                long[,] lMaxPowerTip_Array = new long[101, 11];
                long[,] lMaxPowerRing_Array = new long[101, 11];
                double[] dAvgPowerTip_Array = new double[101];
                double[] dAvgPowerRing_Array = new double[101];
                double[] dAvgGainTip_Array = new double[101];
                double[] dAvgGainRing_Array = new double[101];
                double[] dAvgPowerTip_S3_Array = new double[101];
                double[] dAvgPowerRing_S3_Array = new double[101];
                double[] dAvgGainTip_S3_Array = new double[101];
                double[] dAvgGainRing_S3_Array = new double[101];

                int nTraceCntTip = 0;
                int nTraceCntRing = 0;
                long lTraceSumTip = 0;
                long lTraceSumRing = 0;

                int nGroupDivideValue = 0;
                //int nGroupRx = 0;
                //int nGroupTx = 0;

                List<Raw5TData> cRaw5TData_Tip_List = new List<Raw5TData>();
                List<Raw5TData> cRaw5TData_Ring_List = new List<Raw5TData>();
                int nOffset = 0;

                if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                {
                    cRaw5TData_Tip_List = m_cDataValue_List[nFileIndex].m_cRXRaw5TData_BHFTip_List;
                    cRaw5TData_Ring_List = m_cDataValue_List[nFileIndex].m_cRXRaw5TData_BHFRing_List;
                    nOffset = m_nRXOffset;
                }
                else if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                {
                    cRaw5TData_Tip_List = m_cDataValue_List[nFileIndex].m_cTXRaw5TData_BHFTip_List;
                    cRaw5TData_Ring_List = m_cDataValue_List[nFileIndex].m_cTXRaw5TData_BHFRing_List;
                    nOffset = m_nTXOffset;
                }

                #region Get Top 10 Max Value By Trace
                //找每條Trace的最大十筆值
                foreach (Raw5TData cRaw5TData_Tip in cRaw5TData_Tip_List)
                {
                    if (nGroupDivideValue == 0)
                    {
                        //nGroupRx = 0;
                        //nGroupTx = 0;
                    }

                    int nTipPower = cRaw5TData_Tip.m_nRawData_3T;
                    int nTipIndex = cRaw5TData_Tip.m_nTraceIndex + nOffset;

                    for (int nValueIndex = 1; nValueIndex <= 10; nValueIndex++)
                    {
                        if (nTipPower > lMaxPowerTip_Array[nTipIndex, nValueIndex])
                        {
                            for (int nCompareIndex = 10; nCompareIndex >= nValueIndex + 1; nCompareIndex--)
                            {
                                lMaxPowerTip_Array[nTipIndex, nCompareIndex] = lMaxPowerTip_Array[nTipIndex, nCompareIndex - 1];
                            }

                            lMaxPowerTip_Array[nTipIndex, nValueIndex] = nTipPower;
                            break;
                        }
                    }
                }

                foreach (Raw5TData cRaw5TData_Ring in cRaw5TData_Ring_List)
                {
                    if (nGroupDivideValue == 0)
                    {
                        //nGroupRx = 0;
                        //nGroupTx = 0;
                    }

                    int nRingPower = cRaw5TData_Ring.m_nRawData_3T;
                    int nRingIndex = cRaw5TData_Ring.m_nTraceIndex + nOffset;

                    for (int nValueIndex = 1; nValueIndex <= 10; nValueIndex++)
                    {
                        if (nRingPower > lMaxPowerRing_Array[nRingIndex, nValueIndex])
                        {
                            for (int nCompareIndex = 10; nCompareIndex >= nValueIndex + 1; nCompareIndex--)
                            {
                                lMaxPowerRing_Array[nRingIndex, nCompareIndex] = lMaxPowerRing_Array[nRingIndex, nCompareIndex - 1];
                            }

                            lMaxPowerRing_Array[nRingIndex, nValueIndex] = nRingPower;
                            break;
                        }
                    }
                }
                #endregion

                #region Compute Average Value
                int nTipStart = -1;
                int nTipEnd = -1;
                int nRingStart = -1;
                int nRingEnd = -1;
                double dAvgIndex = 0.0;

                //計算平均值
                for (int nTraceIndex = 0; nTraceIndex <= 100; nTraceIndex++)
                {
                    if (lMaxPowerTip_Array[nTraceIndex, 1] >= 1)
                    {
                        if (nTipStart == -1)
                            nTipStart = nTraceIndex;  //Start Trace Number
                        else
                            nTipEnd = nTraceIndex;    //End Trace Number

                        long lSum = 0;

                        for (int nValueIndex = 1; nValueIndex <= 10; nValueIndex++)
                            lSum += lMaxPowerTip_Array[nTraceIndex, nValueIndex];

                        double dAvg = (double)lSum / 10;
                        dAvgPowerTip_Array[nTraceIndex] = dAvg;
                        nTraceCntTip++;
                        lTraceSumTip = (long)Math.Round((double)lTraceSumTip + dAvg, 0);
                        dAvgIndex += nTraceIndex;
                    }

                    if (lMaxPowerRing_Array[nTraceIndex, 1] >= 1)
                    {
                        if (nRingStart == -1)
                            nRingStart = nTraceIndex;
                        else
                            nRingEnd = nTraceIndex;

                        long lSum = 0;

                        for (int nValueIndex = 1; nValueIndex <= 10; nValueIndex++)
                            lSum += lMaxPowerRing_Array[nTraceIndex, nValueIndex];

                        double dAvg = (double)lSum / 10;
                        dAvgPowerRing_Array[nTraceIndex] = dAvg;
                        nTraceCntRing++;
                        lTraceSumRing = (long)Math.Round((double)lTraceSumRing + dAvg, 0); ;
                    }
                }

                double dAllTraceAvgTip = 0.0;
                double dAllTraceAvgRing = 0.0;

                if (nTraceCntTip > 2)
                    dAllTraceAvgTip = (lTraceSumTip - dAvgPowerTip_Array[nTipStart] - dAvgPowerTip_Array[nTipEnd]) / (nTraceCntTip - 2);

                if (nTraceCntRing > 2)
                    dAllTraceAvgRing = (lTraceSumRing - dAvgPowerRing_Array[nRingStart] - dAvgPowerRing_Array[nRingEnd]) / (nTraceCntRing - 2);
                #endregion

                #region Compute Average Gain & Power
                //Group judgement
                if (m_sICGenerationType == "Gen6")  //6315
                {
                    string sGroup = "";

                    if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                    {
                        //Vertical Line
                        dAvgIndex = dAvgIndex / nTraceCntTip; //Get Center Rx Index

                        if (dAvgIndex <= 41)
                            sGroup = "S1S2";
                        else
                            sGroup = "S3S4";

                        //保留舊的分區方法，但先做不分區
                        sGroup = "Vertical";
                    }
                    else if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                    {
                        //Horizontal Line
                        dAvgIndex = dAvgIndex / nTraceCntTip; //Get Center Tx Index

                        if (dAvgIndex <= 23)
                            sGroup = "S2S4";
                        else
                            sGroup = "S1S3";

                        //保留舊的分區方法，但先做不分區
                        sGroup = "Horizontal";
                    }

                    for (int nValueIndex = 0; nValueIndex <= 100; nValueIndex++)
                    {
                        switch (sGroup)
                        {
                            case "S1S2":    //Tx
                            case "S2S4":    //Rx
                            case "Vertical":    //Tx
                            case "Horizontal":      //Rx
                                if (lMaxPowerTip_Array[nValueIndex, 1] >= 1 && nTraceCntTip >= 3)
                                    dAvgGainTip_Array[nValueIndex] = dAllTraceAvgTip / dAvgPowerTip_Array[nValueIndex];

                                if (lMaxPowerRing_Array[nValueIndex, 1] >= 1 && nTraceCntRing >= 3)
                                    dAvgGainRing_Array[nValueIndex] = dAllTraceAvgRing / dAvgPowerRing_Array[nValueIndex];

                                break;
                            case "S3S4":    //Tx
                            case "S1S3":    //Rx
                                if (lMaxPowerTip_Array[nValueIndex, 1] >= 1 && nTraceCntTip >= 3)
                                {
                                    dAvgGainTip_S3_Array[nValueIndex] = dAllTraceAvgTip / dAvgPowerTip_Array[nValueIndex];
                                    dAvgPowerTip_S3_Array[nValueIndex] = dAvgPowerTip_Array[nValueIndex];
                                }

                                if (lMaxPowerRing_Array[nValueIndex, 1] >= 1 && nTraceCntRing >= 3)
                                {
                                    dAvgGainRing_S3_Array[nValueIndex] = dAllTraceAvgRing / dAvgPowerRing_Array[nValueIndex];
                                    dAvgPowerRing_S3_Array[nValueIndex] = dAvgPowerRing_Array[nValueIndex];
                                }

                                break;
                            default:
                                break;
                        }
                    }

                    if (ParamAutoTuning.m_nTPGTEdgeProcess == 1)
                    {
                        int nTracePin_Start = 0;
                        int nTracePin_End = 0;

                        if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                        {
                            nTracePin_Start = m_nTXStartPin;
                            nTracePin_End = m_nTXEndPin;
                        }
                        else if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                        {
                            nTracePin_Start = m_nRXStartPin;
                            nTracePin_End = m_nRXEndPin;
                        }

                        dAvgGainTip_Array[nTracePin_Start] = (dAvgGainTip_Array[nTracePin_Start + 1] + dAvgGainTip_Array[nTracePin_Start + 2]) / 2.0;
                        dAvgGainRing_Array[nTracePin_Start] = (dAvgGainRing_Array[nTracePin_Start + 1] + dAvgGainRing_Array[nTracePin_Start + 2]) / 2.0;
                        dAvgGainTip_Array[nTracePin_End] = (dAvgGainTip_Array[nTracePin_End - 2] + dAvgGainTip_Array[nTracePin_End - 1]) / 2.0;
                        dAvgGainRing_Array[nTracePin_End] = (dAvgGainRing_Array[nTracePin_End - 2] + dAvgGainRing_Array[nTracePin_End - 1]) / 2.0;

                        dAvgGainTip_S3_Array[nTracePin_Start] = (dAvgGainTip_S3_Array[nTracePin_Start + 1] + dAvgGainTip_S3_Array[nTracePin_Start + 2]) / 2.0;
                        dAvgGainRing_S3_Array[nTracePin_Start] = (dAvgGainRing_S3_Array[nTracePin_Start + 1] + dAvgGainRing_S3_Array[nTracePin_Start + 2]) / 2.0;
                        dAvgGainTip_S3_Array[nTracePin_End] = (dAvgGainTip_S3_Array[nTracePin_End - 2] + dAvgGainTip_S3_Array[nTracePin_End - 1]) / 2.0;
                        dAvgGainRing_S3_Array[nTracePin_End] = (dAvgGainRing_S3_Array[nTracePin_End - 2] + dAvgGainRing_S3_Array[nTracePin_End - 1]) / 2.0;
                    }
                }
                else if (m_sICGenerationType == "Gen7")     //7315
                {
                    string sGroup = "";

                    if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                    {
                        //Vertical Line
                        dAvgIndex = dAvgIndex / nTraceCntTip;     //Get Center Rx Index

                        if (dAvgIndex <= 46)
                            sGroup = "S1S2";
                        else
                            sGroup = "S1S2";    //sGroup = "S3";  //先保留S3的功能
                    }
                    else if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                    {
                        //Horizontal Line
                        sGroup = "All";
                    }

                    for (int nValueIndex = 0; nValueIndex <= 100; nValueIndex++)
                    {
                        switch (sGroup)
                        {
                            case "All":     //Rx
                            case "S1S2":    //Tx
                                if (lMaxPowerTip_Array[nValueIndex, 1] >= 1 && nTraceCntTip >= 3)
                                    dAvgGainTip_Array[nValueIndex] = dAllTraceAvgTip / dAvgPowerTip_Array[nValueIndex];

                                if (lMaxPowerRing_Array[nValueIndex, 1] >= 1 && nTraceCntRing >= 3)
                                    dAvgGainRing_Array[nValueIndex] = dAllTraceAvgRing / dAvgPowerRing_Array[nValueIndex];

                                break;
                            case "S3":  //Tx Partial
                                if (lMaxPowerTip_Array[nValueIndex, 1] >= 1 && nTraceCntTip >= 3)
                                {
                                    dAvgGainTip_S3_Array[nValueIndex] = dAllTraceAvgTip / dAvgPowerTip_Array[nValueIndex];
                                    dAvgPowerTip_S3_Array[nValueIndex] = dAvgPowerTip_Array[nValueIndex];
                                }

                                if (lMaxPowerRing_Array[nValueIndex, 1] >= 1 && nTraceCntRing >= 3)
                                {
                                    dAvgGainRing_S3_Array[nValueIndex] = dAllTraceAvgRing / dAvgPowerRing_Array[nValueIndex];
                                    dAvgPowerRing_S3_Array[nValueIndex] = dAvgPowerRing_Array[nValueIndex];
                                }

                                m_bWriteTxS3Flag = true;
                                break;
                            default:
                                break;
                        }
                    }

                    if (ParamAutoTuning.m_nTPGTEdgeProcess == 1)
                    {
                        int nTracePin_Start = 0;
                        int nTracePin_End = 0;

                        if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                        {
                            nTracePin_Start = m_nTXStartPin;
                            nTracePin_End = m_nTXEndPin;
                        }
                        else if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                        {
                            nTracePin_Start = m_nRXStartPin;
                            nTracePin_End = m_nRXEndPin;
                        }

                        dAvgGainTip_Array[nTracePin_Start] = (dAvgGainTip_Array[nTracePin_Start + 1] + dAvgGainTip_Array[nTracePin_Start + 2]) / 2.0;
                        dAvgGainRing_Array[nTracePin_Start] = (dAvgGainRing_Array[nTracePin_Start + 1] + dAvgGainRing_Array[nTracePin_Start + 2]) / 2.0;
                        dAvgGainTip_Array[nTracePin_End] = (dAvgGainTip_Array[nTracePin_End - 2] + dAvgGainTip_Array[nTracePin_End - 1]) / 2.0;
                        dAvgGainRing_Array[nTracePin_End] = (dAvgGainRing_Array[nTracePin_End - 2] + dAvgGainRing_Array[nTracePin_End - 1]) / 2.0;

                        if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                        {
                            dAvgGainTip_S3_Array[nTracePin_Start] = (dAvgGainTip_S3_Array[nTracePin_Start + 1] + dAvgGainTip_S3_Array[nTracePin_Start + 2]) / 2.0;
                            dAvgGainRing_S3_Array[nTracePin_Start] = (dAvgGainRing_S3_Array[nTracePin_Start + 1] + dAvgGainRing_S3_Array[nTracePin_Start + 2]) / 2.0;
                            dAvgGainTip_S3_Array[nTracePin_End] = (dAvgGainTip_S3_Array[nTracePin_End - 2] + dAvgGainTip_S3_Array[nTracePin_End - 1]) / 2.0;
                            dAvgGainRing_S3_Array[nTracePin_End] = (dAvgGainRing_S3_Array[nTracePin_End - 2] + dAvgGainRing_S3_Array[nTracePin_End - 1]) / 2.0;
                        }
                    }
                }
                else if (m_sICGenerationType == "Gen6308")
                {
                    string sGroup = "";

                    if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                        //Verical Line
                        sGroup = "AllTx";
                    else if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                        //Horizontal Line
                        sGroup = "AllRx";

                    for (int nValueIndex = 0; nValueIndex <= 100; nValueIndex++)
                    {
                        switch (sGroup)
                        {
                            case "AllRx":   //Rx
                            case "AllTx":   //Tx
                                if (lMaxPowerTip_Array[nValueIndex, 1] >= 1 && nTraceCntTip >= 3)
                                    dAvgGainTip_Array[nValueIndex] = dAllTraceAvgTip / dAvgPowerTip_Array[nValueIndex];

                                if (lMaxPowerRing_Array[nValueIndex, 1] >= 1 && nTraceCntRing >= 3)
                                    dAvgGainRing_Array[nValueIndex] = dAllTraceAvgRing / dAvgPowerRing_Array[nValueIndex];

                                break;
                            default:
                                break;
                        }
                    }

                    if (ParamAutoTuning.m_nTPGTEdgeProcess == 1)
                    {
                        int nTracePin_Start = 0;
                        int nTracePin_End = 0;

                        if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                        {
                            nTracePin_Start = m_nTXStartPin;
                            nTracePin_End = m_nTXEndPin;
                        }
                        else if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                        {
                            nTracePin_Start = m_nRXStartPin;
                            nTracePin_End = m_nRXEndPin;
                        }

                        dAvgGainTip_Array[nTracePin_Start] = (dAvgGainTip_Array[nTracePin_Start + 1] + dAvgGainTip_Array[nTracePin_Start + 2]) / 2.0;
                        dAvgGainRing_Array[nTracePin_Start] = (dAvgGainRing_Array[nTracePin_Start + 1] + dAvgGainRing_Array[nTracePin_Start + 2]) / 2.0;
                        dAvgGainTip_Array[nTracePin_End] = (dAvgGainTip_Array[nTracePin_End - 2] + dAvgGainTip_Array[nTracePin_End - 1]) / 2.0;
                        dAvgGainRing_Array[nTracePin_End] = (dAvgGainRing_Array[nTracePin_End - 2] + dAvgGainRing_Array[nTracePin_End - 1]) / 2.0;
                    }
                }
                #endregion

                #region Copy Interal Array & Value To Global Array & Value
                if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                {
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lMaxPowerTip_Array = lMaxPowerTip_Array;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lMaxPowerRing_Array = lMaxPowerRing_Array;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_Array = dAvgPowerTip_Array;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_Array = dAvgPowerRing_Array;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_S3_Array = dAvgPowerTip_S3_Array;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_S3_Array = dAvgPowerRing_S3_Array;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_Array = dAvgGainTip_Array;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array = dAvgGainRing_Array;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_S3_Array = dAvgGainTip_S3_Array;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_S3_Array = dAvgGainRing_S3_Array;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTraceCntTip = nTraceCntTip;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTraceCntRing = nTraceCntRing;
                }
                else if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                {
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerTip_Array = lMaxPowerTip_Array;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerRing_Array = lMaxPowerRing_Array;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_Array = dAvgPowerTip_Array;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_Array = dAvgPowerRing_Array;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_S3_Array = dAvgPowerTip_S3_Array;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_S3_Array = dAvgPowerRing_S3_Array;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_Array = dAvgGainTip_Array;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array = dAvgGainRing_Array;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_S3_Array = dAvgGainTip_S3_Array;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_S3_Array = dAvgGainRing_S3_Array;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntTip = nTraceCntTip;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntRing = nTraceCntRing;
                }
                #endregion
            }
        }

        private void WriteProcessDataFile(int nFileIndex, string sFilePath)
        {
            FileStream fsFile = new FileStream(sFilePath, FileMode.Create);
            StreamWriter swFile = new StreamWriter(fsFile);

            try
            {
                #region  Trace Number
                swFile.Write("Trace,");

                for (int nTraceIndex = 0; nTraceIndex <= 83; nTraceIndex++)
                {
                    if (nTraceIndex == 83)
                        swFile.WriteLine(string.Format("{0}", nTraceIndex));
                    else
                        swFile.Write(string.Format("{0},", nTraceIndex));
                }
                #endregion
                
                #region Tx Gain & Power Data
                //TipTxGain
                swFile.Write("TipTxGain,");
                int nTipTxEndIndex = 0;

                for (int nTraceIndex = 0; nTraceIndex < m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerTip_Array.GetLength(0); nTraceIndex++)
                {
                    if ((m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerTip_Array[nTraceIndex, 1] >= 1 && 
                         m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntTip >= 3) ||
                        m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_Array[nTraceIndex] > 0.0)
                        nTipTxEndIndex = nTraceIndex;
                }

                for (int nTraceIndex = 0; nTraceIndex <= nTipTxEndIndex; nTraceIndex++)
                {
                    if ((m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerTip_Array[nTraceIndex, 1] >= 1 && 
                         m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntTip >= 3) ||
                        m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_Array[nTraceIndex] > 0.0)
                        swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_Array[nTraceIndex]));

                    if (nTraceIndex == nTipTxEndIndex)
                        swFile.WriteLine();
                    else
                        swFile.Write(",");
                }

                //RingTxGain
                swFile.Write("RingTxGain,");
                int nRingTxEndIndex = 0;

                for (int nTraceIndex = 0; nTraceIndex < m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerRing_Array.GetLength(0); nTraceIndex++)
                {
                    if ((m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerRing_Array[nTraceIndex, 1] >= 1 && 
                         m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntRing >= 3) ||
                        m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] > 0.0)
                        nRingTxEndIndex = nTraceIndex;
                }

                for (int nTraceIndex = 0; nTraceIndex <= nRingTxEndIndex; nTraceIndex++)
                {
                    if ((m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerRing_Array[nTraceIndex, 1] >= 1 && 
                         m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntRing >= 3) ||
                        m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] > 0.0)
                        swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex]));

                    if (nTraceIndex == nRingTxEndIndex)
                        swFile.WriteLine();
                    else
                        swFile.Write(",");
                }

                //TipTxPwr
                swFile.Write("TipTxPwr,");

                for (int nTraceIndex = 0; nTraceIndex <= nTipTxEndIndex; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerTip_Array[nTraceIndex, 1] >= 1 && 
                        m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntTip >= 3)
                        swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex]));

                    if (nTraceIndex == nTipTxEndIndex)
                        swFile.WriteLine();
                    else
                        swFile.Write(",");
                }

                //RingTxPwr
                swFile.Write("RingTxPwr,");

                for (int nTraceIndex = 0; nTraceIndex <= nRingTxEndIndex; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerRing_Array[nTraceIndex, 1] >= 1 && 
                        m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntRing >= 3)
                        swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_Array[nTraceIndex]));

                    if (nTraceIndex == nTipTxEndIndex)
                        swFile.WriteLine();
                    else
                        swFile.Write(",");
                }
                #endregion

                #region Tx S3 Gain & Power Data
                if (m_sICGenerationType == "Gen7")
                {
                    if (m_bWriteTxS3Flag == false)
                    {
                        swFile.WriteLine("TipTxGain S3");
                        swFile.WriteLine("RingTxGain S3");
                        swFile.WriteLine("TipTxPwr S3");
                        swFile.WriteLine("RingTxPwr S3");
                    }
                    else
                    {
                        //TipTxGain S3
                        swFile.Write("TipTxGain S3,");

                        for (int nTraceIndex = 0; nTraceIndex <= nTipTxEndIndex; nTraceIndex++)
                        {
                            if ((m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerTip_Array[nTraceIndex, 1] >= 1 && 
                                 m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntTip >= 3) ||
                                m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_S3_Array[nTraceIndex] > 0.0)
                                swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_S3_Array[nTraceIndex]));

                            if (nTraceIndex == nTipTxEndIndex)
                                swFile.WriteLine();
                            else
                                swFile.Write(",");
                        }

                        //RingTxGain S3
                        swFile.Write("RingTxGain S3,");

                        for (int nTraceIndex = 0; nTraceIndex <= nRingTxEndIndex; nTraceIndex++)
                        {
                            if ((m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerRing_Array[nTraceIndex, 1] >= 1 && 
                                 m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntRing >= 3) ||
                                m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_S3_Array[nTraceIndex] > 0.0)
                                swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_S3_Array[nTraceIndex]));

                            if (nTraceIndex == nRingTxEndIndex)
                                swFile.WriteLine();
                            else
                                swFile.Write(",");
                        }

                        //TipTxPwr S3
                        swFile.Write("TipTxPwr S3,");

                        for (int nTraceIndex = 0; nTraceIndex <= nTipTxEndIndex; nTraceIndex++)
                        {
                            if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerTip_Array[nTraceIndex, 1] >= 1 && 
                                m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntTip >= 3)
                                swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_S3_Array[nTraceIndex]));

                            if (nTraceIndex == nTipTxEndIndex)
                                swFile.WriteLine();
                            else
                                swFile.Write(",");
                        }

                        //RingTxPwr S3
                        swFile.Write("RingTxPwr S3,");

                        for (int nTraceIndex = 0; nTraceIndex <= nRingTxEndIndex; nTraceIndex++)
                        {
                            if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lMaxPowerRing_Array[nTraceIndex, 1] >= 1 && 
                                m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTraceCntRing >= 3)
                                swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_S3_Array[nTraceIndex]));

                            if (nTraceIndex == nTipTxEndIndex)
                                swFile.WriteLine();
                            else
                                swFile.Write(",");
                        }
                    }
                }
                else
                {
                    swFile.WriteLine();
                    swFile.WriteLine();
                    swFile.WriteLine();
                    swFile.WriteLine();
                }
                #endregion

                #region Rx Gain & Power Data
                //TipRxGain
                swFile.Write("TipRxGain,");
                int nTipRxEndIndex = 0;

                for (int nTraceIndex = 0; nTraceIndex < m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lMaxPowerTip_Array.GetLength(0); nTraceIndex++)
                {
                    if ((m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lMaxPowerTip_Array[nTraceIndex, 1] >= 1 && 
                         m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTraceCntTip >= 3) ||
                        m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_Array[nTraceIndex] > 0.0)
                        nTipRxEndIndex = nTraceIndex;
                }

                for (int nTraceIndex = 0; nTraceIndex <= nTipRxEndIndex; nTraceIndex++)
                {
                    if ((m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lMaxPowerTip_Array[nTraceIndex, 1] >= 1 && 
                         m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTraceCntTip >= 3) ||
                        m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_Array[nTraceIndex] > 0.0)
                        swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_Array[nTraceIndex]));

                    if (nTraceIndex == nTipRxEndIndex)
                        swFile.WriteLine();
                    else
                        swFile.Write(",");
                }

                //RingRxGain
                swFile.Write("RingRxGain,");
                int nRingRxEndIndex = 0;

                for (int nTraceIndex = 0; nTraceIndex < m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lMaxPowerRing_Array.GetLength(0); nTraceIndex++)
                {
                    if ((m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lMaxPowerRing_Array[nTraceIndex, 1] >= 1 && 
                         m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTraceCntRing >= 3) ||
                        m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] > 0.0)
                        nRingRxEndIndex = nTraceIndex;
                }

                for (int nTraceIndex = 0; nTraceIndex <= nRingRxEndIndex; nTraceIndex++)
                {
                    if ((m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lMaxPowerRing_Array[nTraceIndex, 1] >= 1 && 
                         m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTraceCntRing >= 3) ||
                        m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] > 0.0)
                        swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex]));

                    if (nTraceIndex == nRingRxEndIndex)
                        swFile.WriteLine();
                    else
                        swFile.Write(",");
                }

                //TipRxPwr
                swFile.Write("TipRxPwr,");

                for (int nTraceIndex = 0; nTraceIndex <= nTipRxEndIndex; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lMaxPowerTip_Array[nTraceIndex, 1] >= 1 && 
                        m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTraceCntTip >= 3)
                        swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex]));

                    if (nTraceIndex == nTipRxEndIndex)
                        swFile.WriteLine();
                    else
                        swFile.Write(",");
                }

                //RingRxPwr
                swFile.Write("RingRxPwr,");

                for (int nTraceIndex = 0; nTraceIndex <= nRingRxEndIndex; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lMaxPowerRing_Array[nTraceIndex, 1] >= 1 && 
                        m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTraceCntRing >= 3)
                        swFile.Write(string.Format("{0}", m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_Array[nTraceIndex]));

                    if (nTraceIndex == nTipRxEndIndex)
                        swFile.WriteLine();
                    else
                        swFile.Write(",");
                }
                #endregion
            }
            finally
            {
                swFile.Flush();
                swFile.Close();
                fsFile.Close();
            }
        }

        private void ComputeTipFWGain(int nFileIndex)
        {
            double dTxSum = 0.0;
            long lRxSum = 0;
            int nTxCnt = 0, nRxCnt = 0;

            #region Compute Tip TP Gain By Ring TP Gain Again
            //用Ring的TP Gain重新計算Tip的TP Gain
            for (int nTraceIndex = 0; nTraceIndex <= 100; nTraceIndex++)
            {
                //Tx First
                if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] != 0.0 && 
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex] != 0.0)
                {
                    int nValue = (int)(m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] * 
                                       m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex] + 0.5);
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex] = nValue;
                    nTxCnt++;
                    dTxSum += Convert.ToInt64(m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex]);
                }

                if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] != 0.0 && 
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex] != 0.0)
                {
                    int nValue = (int)(m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] * 
                                       m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex] + 0.5);
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex] = nValue;
                    nRxCnt++;
                    lRxSum += Convert.ToInt64(m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex]);
                }
            }
            #endregion

            #region Compute Tx/Rx Average
            //計算平均
            if (nTxCnt > 0)
                dTxSum = dTxSum / nTxCnt;

            if (nRxCnt > 0)
                lRxSum = (long)Math.Round((double)lRxSum / nRxCnt, 0);
            #endregion

            #region Compute Tx/Rx Tip FW Gain
            for (int nTraceIndex = 0; nTraceIndex <= 100; nTraceIndex++)
            {
                //Tx First
                if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] != 0.0 && 
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex] != 0.0)
                {
                    double dValue = dTxSum / m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex];
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dTipFWGainAfterTPGain_Array[nTraceIndex] = dValue;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lTipFWGain_Array[nTraceIndex] = (int)(dValue * m_nGainRatio + 0.5);
                }
                else
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lTipFWGain_Array[nTraceIndex] = m_nGainRatio;

                if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] != 0.0 && 
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex] != 0.0)
                {
                    double dValue = (double)lRxSum / m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex];
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dTipFWGainAfterTPGain_Array[nTraceIndex] = dValue;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lTipFWGain_Array[nTraceIndex] = (int)(dValue * m_nGainRatio + 0.5);
                }
                else
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lTipFWGain_Array[nTraceIndex] = m_nGainRatio;
            }
            #endregion
        }

        private void WriteComputeDataFile(int nFileIndex, string sFilePath)
        {
            FileStream fsFile = new FileStream(sFilePath, FileMode.Create);
            StreamWriter swFile = new StreamWriter(fsFile);

            try
            {
                #region  Trace Number
                swFile.Write("Trace,");

                for (int nTraceIndex = 1; nTraceIndex <= 84; nTraceIndex++)
                {
                    if (nTraceIndex == 84)
                        swFile.WriteLine(string.Format("{0}", nTraceIndex));
                    else
                        swFile.Write(string.Format("{0},", nTraceIndex));
                }
                #endregion

                #region TxTipPwrAfterTPGain
                swFile.Write("TxTipPwrAfterTPGain,");
                int nTipTxEndIndex = 0;

                for (int nTraceIndex = 0; nTraceIndex < m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTipPowerAfterTPGain_Array.Length; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex] > 0)
                        nTipTxEndIndex = nTraceIndex;
                }

                for (int nTraceIndex = 0; nTraceIndex <= nTipTxEndIndex; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex] > 0)
                        swFile.Write(m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex]);

                    if (nTraceIndex < nTipTxEndIndex)
                        swFile.Write(",");
                    else
                        swFile.WriteLine();
                }
                #endregion

                #region RxTipPwrAfterTPGain
                swFile.Write("RxTipPwrAfterTPGain,");
                int nTipRxEndIndex = 0;

                for (int nTraceIndex = 0; nTraceIndex < m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTipPowerAfterTPGain_Array.Length; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex] > 0)
                        nTipRxEndIndex = nTraceIndex;
                }

                for (int nTraceIndex = 0; nTraceIndex <= nTipRxEndIndex; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex] > 0)
                        swFile.Write(m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_nTipPowerAfterTPGain_Array[nTraceIndex]);

                    if (nTraceIndex < nTipRxEndIndex)
                        swFile.Write(",");
                    else
                        swFile.WriteLine();
                }
                #endregion

                #region TxTipFWGainAfterTPGain
                swFile.Write("TxTipFWGainAfterTPGain,");
                nTipTxEndIndex = 0;

                for (int nTraceIndex = 0; nTraceIndex < m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dTipFWGainAfterTPGain_Array.Length; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dTipFWGainAfterTPGain_Array[nTraceIndex] > 0)
                        nTipTxEndIndex = nTraceIndex;
                }

                for (int nTraceIndex = 0; nTraceIndex <= nTipTxEndIndex; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dTipFWGainAfterTPGain_Array[nTraceIndex] > 0)
                        swFile.Write(m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dTipFWGainAfterTPGain_Array[nTraceIndex]);

                    if (nTraceIndex < nTipTxEndIndex)
                        swFile.Write(",");
                    else
                        swFile.WriteLine();
                }
                #endregion

                #region RxTipFWGainAfterTPGain
                swFile.Write("RxTipFWGainAfterTPGain,");
                nTipRxEndIndex = 0;

                for (int nTraceIndex = 0; nTraceIndex < m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dTipFWGainAfterTPGain_Array.Length; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dTipFWGainAfterTPGain_Array[nTraceIndex] > 0)
                        nTipRxEndIndex = nTraceIndex;
                }

                for (int nTraceIndex = 0; nTraceIndex <= nTipRxEndIndex; nTraceIndex++)
                {
                    if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dTipFWGainAfterTPGain_Array[nTraceIndex] > 0)
                        swFile.Write(m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dTipFWGainAfterTPGain_Array[nTraceIndex]);

                    if (nTraceIndex < nTipRxEndIndex)
                        swFile.Write(",");
                    else
                        swFile.WriteLine();
                }
                #endregion
            }
            finally
            {
                swFile.Flush();
                swFile.Close();
                fsFile.Close();
            }
        }

        private void CreateAndSaveTPGainTableFile(int nFileIndex)
        {
            string sTPGainTableFilePath = string.Format(@"{0}\R{1}_{2}_{3}_{4}.txt", 
                                                        m_sTPGainTableFolderPath,
                                                        m_cDataInfo_List[nFileIndex].m_nRankIndex.ToString(),
                                                        m_cDataInfo_List[nFileIndex].m_dFrequency.ToString(),
                                                        m_cDataInfo_List[nFileIndex].m_nSettingPH1.ToString().PadLeft(2, '0'),
                                                        m_cDataInfo_List[nFileIndex].m_nSettingPH2.ToString().PadLeft(2, '0'));

            double dInputGain = 0.0;
            double dGainStep = 0.0;
            long lOutputValue = 0;

            long[] lRxTipGain_Array = new long[101];
            long[] lRxRingGain_Array = new long[101];
            long[] lTxTipGain_Array = new long[101];
            long[] lTxRingGain_Array = new long[101];
            long[] lTxTipFWGain_Array = new long[101];
            long[] lRxTipFWGain_Array = new long[101];

            long[] lRxTipGainS1S3_Array = new long[101];
            long[] lRxRingGainS1S3_Array = new long[101];
            long[] lRxTipGainS2S4_Array = new long[101];
            long[] lRxRingGainS2S4_Array = new long[101];
            long[] lTxTipGainS1S2_Array = new long[101];
            long[] lTxRingGainS1S2_Array = new long[101];
            long[] lTxTipGainS3S4_Array = new long[101];
            long[] lTxRingGainS3S4_Array = new long[101];

            long[] lTxRingGainS2_Array = new long[5];

            #region Set Output TP Gain Data
            #region ============================= Part of Gen7315 =============================
            #region Compute Tx/Rx Tip/Ring Gain For Gen7
            //Gen7 First
            int nInitial100 = 128;      //Gen7 is 128, Gen6 is 32
            dGainStep = 1 / (double)nInitial100;     //1+7
            int nMaxDiv = 256 - 1;
            int nFinalValue = 0;

            //Change Data
            for (int nTraceIndex = 0; nTraceIndex <= 100; nTraceIndex++)
            {
                double dValue = 0.0;

                if (nTraceIndex <= 3)   //For Gen7 S2
                {
                    dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_S3_Array[nTraceIndex];    //TipS2

                    if (dValue == 0.0)
                        nFinalValue = nInitial100;
                    else
                    {
                        dInputGain = dValue / dGainStep;
                        //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                        nFinalValue = (int)(dInputGain + 0.5);

                        if (nFinalValue > nMaxDiv)
                            nFinalValue = nMaxDiv;
                    }

                    lTxRingGainS2_Array[nTraceIndex] = nFinalValue;
                }

                dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_Array[nTraceIndex];   //Tip Rx

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lRxTipGain_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex];   //Ring Rx

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lRxRingGain_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_Array[nTraceIndex];   //Tip Tx

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lTxTipGain_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex];   //Ring Tx

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lTxRingGain_Array[nTraceIndex] = nFinalValue;
            }
            #endregion

            #region Part of Gen7
            //OutS2_Tx
            string sOutS2_Tx = ".DW    ";
            lOutputValue = lTxRingGainS2_Array[(1 - 1) * 2] + lTxRingGainS2_Array[(1 - 1) * 2 + 1] * 256;
            sOutS2_Tx += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
            lOutputValue = lTxRingGainS2_Array[(2 - 1) * 2] + lTxRingGain_Array[(2 - 1) * 2 + 1] * 256;
            sOutS2_Tx += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());

            //Out_TxTip
            string sOut_TxTip = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 21; nTraceIndex++)
            {
                lOutputValue = lTxTipGain_Array[(nTraceIndex - 1) * 2] + lTxTipGain_Array[(nTraceIndex - 1) * 2 + 1] * 256;

                if (nTraceIndex == 1)
                    sOut_TxTip += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_TxTip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_TxRing
            string sOut_TxRing = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 21; nTraceIndex++)
            {
                lOutputValue = lTxRingGain_Array[(nTraceIndex - 1) * 2] + lTxRingGain_Array[(nTraceIndex - 1) * 2 + 1] * 256;

                if (nTraceIndex == 1)
                    sOut_TxRing += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_TxRing += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //TRxS Group Offset For Rx On 7315
            int nTRxS_Group_0_Rx_Offset = 0;
            int nTRxS_Group_1_Rx_Offset = 21;
            int nTRxS_Group_2_Rx_Offset = 42;

            //Out_RxS0Tip
            string sOut_RxS0Tip = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 15; nTraceIndex++)
            {
                lOutputValue = lRxTipGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_0_Rx_Offset] +
                                   lRxTipGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_0_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS0Tip += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS0Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_RxS1Tip
            string sOut_RxS1Tip = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 15; nTraceIndex++)
            {
                lOutputValue = lRxTipGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_1_Rx_Offset] +
                                   lRxTipGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_1_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS1Tip += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS1Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_RxS2Tip
            string sOut_RxS2Tip = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 15; nTraceIndex++)
            {
                lOutputValue = lRxTipGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_2_Rx_Offset] +
                                   lRxTipGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_2_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS2Tip += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS2Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_RxS0Ring
            string sOut_RxS0Ring = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 15; nTraceIndex++)
            {
                lOutputValue = lRxRingGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_0_Rx_Offset] +
                                   lRxRingGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_0_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS0Ring += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS0Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_RxS1Tip
            string sOut_RxS1Ring = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 15; nTraceIndex++)
            {
                lOutputValue = lRxRingGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_1_Rx_Offset] +
                                   lRxRingGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_1_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS1Ring += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS1Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_RxS2Tip
            string sOut_RxS2Ring = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 15; nTraceIndex++)
            {
                lOutputValue = lRxRingGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_2_Rx_Offset] +
                                   lRxRingGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_2_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS2Ring += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS2Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }
            #endregion
            #endregion

            #region ============================= Part of Gen7318 =============================
            //Out_TxTip7318
            string sOut_TxTip7318 = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 24; nTraceIndex++)
            {
                lOutputValue = lTxTipGain_Array[(nTraceIndex - 1) * 2] + lTxTipGain_Array[(nTraceIndex - 1) * 2 + 1] * 256;

                if (nTraceIndex == 1)
                    sOut_TxTip7318 += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_TxTip7318 += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_TxRing7318
            string sOut_TxRing7318 = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 24; nTraceIndex++)
            {
                lOutputValue = lTxRingGain_Array[(nTraceIndex - 1) * 2] + lTxRingGain_Array[(nTraceIndex - 1) * 2 + 1] * 256;

                if (nTraceIndex == 1)
                    sOut_TxRing7318 += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_TxRing7318 += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //TRxS Group Offset For Rx On 7318
            nTRxS_Group_0_Rx_Offset = 0;
            nTRxS_Group_1_Rx_Offset = 24;
            nTRxS_Group_2_Rx_Offset = 48;

            //Out_RxS0Tip7318
            string sOut_RxS0Tip7318 = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 18; nTraceIndex++)
            {
                lOutputValue = lRxTipGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_0_Rx_Offset] +
                                   lRxTipGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_0_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS0Tip7318 += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS0Tip7318 += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_RxS1Tip7318
            string sOut_RxS1Tip7318 = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 18; nTraceIndex++)
            {
                lOutputValue = lRxTipGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_1_Rx_Offset] +
                                   lRxTipGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_1_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS1Tip7318 += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS1Tip7318 += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_RxS2Tip7318
            string sOut_RxS2Tip7318 = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 18; nTraceIndex++)
            {
                lOutputValue = lRxTipGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_2_Rx_Offset] +
                                   lRxTipGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_2_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS2Tip7318 += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS2Tip7318 += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_RxS0Ring7318
            string sOut_RxS0Ring7318 = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 18; nTraceIndex++)
            {
                lOutputValue = lRxRingGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_0_Rx_Offset] +
                                   lRxRingGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_0_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS0Ring7318 += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS0Ring7318 += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_RxS1Ring7318
            string sOut_RxS1Ring7318 = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 18; nTraceIndex++)
            {
                lOutputValue = lRxRingGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_1_Rx_Offset] +
                                   lRxRingGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_1_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS1Ring7318 += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS1Ring7318 += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //Out_RxS2Ring7318
            string sOut_RxS2Ring7318 = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 18; nTraceIndex++)
            {
                lOutputValue = lRxRingGain_Array[(nTraceIndex - 1) * 2 + nTRxS_Group_2_Rx_Offset] +
                                   lRxRingGain_Array[(nTraceIndex - 1) * 2 + 1 + nTRxS_Group_2_Rx_Offset] * 256;

                if (nTraceIndex == 1)
                    sOut_RxS2Ring7318 += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOut_RxS2Ring7318 += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }
            #endregion

            #region ============================= Part of Gen6308 =============================
            #region Compute Tx/Rx Tip/Ring Gain For Gen6308
            nInitial100 = 128;      //Gen7 is 128, Gen6315/6308 is 32
            dGainStep = 1 / (double)nInitial100;
            nMaxDiv = 256 - 1;

            //Change Data
            for (int nTraceIndex = 0; nTraceIndex <= 100; nTraceIndex++)
            {
                double dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_Array[nTraceIndex];   //Tip Tx

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lTxTipGain_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex];   //Ring Tx

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lTxRingGain_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_Array[nTraceIndex];   //Tip Rx

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lRxTipGain_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex];   //Ring Rx

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lRxRingGain_Array[nTraceIndex] = nFinalValue;
            }
            #endregion

            #region Part of Gen6308
            //OutAllTx_Tip
            string sOutAllTx_Tip = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 12; nTraceIndex++)
            {
                lOutputValue = lTxTipGain_Array[(nTraceIndex - 1) * 2] + lTxTipGain_Array[(nTraceIndex - 1) * 2 + 1] * 256;

                if (nTraceIndex == 1)
                    sOutAllTx_Tip += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOutAllTx_Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //OutAllTx_Ring
            string sOutAllTx_Ring = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 12; nTraceIndex++)
            {
                lOutputValue = lTxRingGain_Array[(nTraceIndex - 1) * 2] + lTxRingGain_Array[(nTraceIndex - 1) * 2 + 1] * 256;

                if (nTraceIndex == 1)
                    sOutAllTx_Ring += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOutAllTx_Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //OutAllRx_Tip
            string sOutAllRx_Tip = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 20; nTraceIndex++)
            {
                lOutputValue = lRxTipGain_Array[(nTraceIndex - 1) * 2] + lRxTipGain_Array[(nTraceIndex - 1) * 2 + 1] * 256;

                if (nTraceIndex == 1)
                    sOutAllRx_Tip += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOutAllRx_Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }

            //OutAllRx_Ring
            string sOutAllRx_Ring = ".DW    ";

            for (int nTraceIndex = 1; nTraceIndex <= 20; nTraceIndex++)
            {
                lOutputValue = lRxRingGain_Array[(nTraceIndex - 1) * 2] + lRxRingGain_Array[(nTraceIndex - 1) * 2 + 1] * 256;

                if (nTraceIndex == 1)
                    sOutAllRx_Ring += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                else
                    sOutAllRx_Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
            }
            #endregion
            #endregion

            #region ============================= Part of Gen6 =============================
            #region Compute Tx/Rx Tip/Ring Gain For Gen6
            //是否重算能量的平均值
            /*
            //ReCalculate Average
            long lSumPwrRxTip = 0, lSumPwrTxTip = 0;
            int nSumCntRxTip = 0, nSumCntTxTip = 0;
            long lSumPwrRxRing = 0, lSumPwrTxRing = 0;
            int nSumCntRxRing = 0, nSumCntTxRing = 0;

            //Tip
            for (int nTraceIndex = 0; nTraceIndex <= 100; nTraceIndex++)
            {
                if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex] != 0.0)      //TipTx
                {
                    lSumPwrTxTip += Convert.ToInt64(m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex]);
                    nSumCntTxTip++;
                }

                if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_S3_Array[nTraceIndex] != 0.0)      //TipTx_S3
                {
                    lSumPwrTxTip += Convert.ToInt64(m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_S3_Array[nTraceIndex]);
                    nSumCntTxTip++;
                }

                if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex] != 0.0)      //TipRx
                {
                    lSumPwrRxTip += Convert.ToInt64(m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex]);
                    nSumCntRxTip++;
                }

                if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_S3_Array[nTraceIndex] != 0.0)      //TipRx_S3
                {
                    lSumPwrRxTip += Convert.ToInt64(m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_S3_Array[nTraceIndex]);
                    nSumCntRxTip++;
                }

                if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_Array[nTraceIndex] != 0.0)      //RingTx
                {
                    lSumPwrTxRing += Convert.ToInt64(m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_Array[nTraceIndex]);
                    nSumCntTxRing++;
                }

                if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_S3_Array[nTraceIndex] != 0.0)      //RingTx_S3
                {
                    lSumPwrTxRing += Convert.ToInt64(m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_S3_Array[nTraceIndex]);
                    nSumCntTxRing++;
                }

                if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_Array[nTraceIndex] != 0.0)      //RingRx
                {
                    lSumPwrRxRing += Convert.ToInt64(m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_Array[nTraceIndex]);
                    nSumCntRxRing++;
                }

                if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_S3_Array[nTraceIndex] != 0.0)      //RingRx_S3
                {
                    lSumPwrRxRing += Convert.ToInt64(m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_S3_Array[nTraceIndex]);
                    nSumCntRxRing++;
                }
            }

            if (nSumCntRxTip >= 1)
                lSumPwrRxTip = lSumPwrRxTip / nSumCntRxTip;

            if (nSumCntTxTip >= 1)
                lSumPwrTxTip = lSumPwrTxTip / nSumCntTxTip;

            if (nSumCntRxRing >= 1)
                lSumPwrRxRing = lSumPwrRxRing / nSumCntRxRing;

            if (nSumCntTxRing >= 1)
                lSumPwrTxRing = lSumPwrTxRing / nSumCntTxRing;

            for (int nTraceIndex = 0; nTraceIndex <= 100; nTraceIndex++)
            {
                if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex] != 0.0)      //TipTx
                {
                    double dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex];
                    dValue = lSumPwrTxTip / dValue;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_Array[nTraceIndex] = dValue;
                }

                if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_S3_Array[nTraceIndex] != 0.0)      //TipTx_S3
                {
                    double dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerTip_S3_Array[nTraceIndex];
                    dValue = lSumPwrTxTip / dValue;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_S3_Array[nTraceIndex] = dValue;
                }

                if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex] != 0.0)      //TipRx
                {
                    double dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_Array[nTraceIndex];
                    dValue = lSumPwrRxTip / dValue;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_Array[nTraceIndex] = dValue;
                }

                if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_S3_Array[nTraceIndex] != 0.0)      //TipRx_S3
                {
                    double dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerTip_S3_Array[nTraceIndex];
                    dValue = lSumPwrRxTip / dValue;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_S3_Array[nTraceIndex] = dValue;
                }

                if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_Array[nTraceIndex] != 0.0)      //RingTx
                {
                    double dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_Array[nTraceIndex];
                    dValue = lSumPwrTxRing / dValue;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] = dValue;
                }

                if (m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_S3_Array[nTraceIndex] != 0.0)      //RingTx_S3
                {
                    double dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgPowerRing_S3_Array[nTraceIndex];
                    dValue = lSumPwrTxRing / dValue;
                    m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_S3_Array[nTraceIndex] = dValue;
                }

                if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_Array[nTraceIndex] != 0.0)      //RingRx
                {
                    double dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_Array[nTraceIndex];
                    dValue = lSumPwrRxRing / dValue;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex] = dValue;
                }

                if (m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_S3_Array[nTraceIndex] != 0.0)      //RingRx_S3
                {
                    double dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgPowerRing_S3_Array[nTraceIndex];
                    dValue = lSumPwrRxRing / dValue;
                    m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_S3_Array[nTraceIndex] = dValue;
                }
            }
            */

            nInitial100 = 32;       //Gen7 is 128, Gen6 is 32
            dGainStep = 0.03125;    //3 + 5
            nMaxDiv = 256 - 1;

            //Change Data
            for (int nTraceIndex = 0; nTraceIndex <= 100; nTraceIndex++)
            {
                double dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_Array[nTraceIndex];   //Tx Tip S1S2

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lTxTipGainS1S2_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex];   //Tx Ring S1S2

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lTxRingGainS1S2_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainTip_Array[nTraceIndex];   //Tx Tip S3S4

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lTxTipGainS3S4_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex];   //Tx Ring S3S4

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lTxRingGainS3S4_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_Array[nTraceIndex];   //Rx Tip S2S4

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lRxTipGainS2S4_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex];   //Rx Ring S2S4

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lRxRingGainS2S4_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainTip_Array[nTraceIndex];   //Rx Tip S1S3

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lRxTipGainS1S3_Array[nTraceIndex] = nFinalValue;

                dValue = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex];   //Rx Ring S1S3

                if (dValue == 0.0)
                    nFinalValue = nInitial100;
                else
                {
                    dInputGain = dValue / dGainStep;
                    //nFinalValue = (int)Math.Round(dInputGain, 0, MidpointRounding.AwayFromZero);
                    nFinalValue = (int)(dInputGain + 0.5);

                    if (nFinalValue > nMaxDiv)
                        nFinalValue = nMaxDiv;
                }

                lRxRingGainS1S3_Array[nTraceIndex] = nFinalValue;
            }
            #endregion

            #region S0 & S1 的 ADC0 的 TP Gain 產生
            int nCount = 1;
            int nCheckLoop = 0;
            int nGain = 1;
            lOutputValue = 0;
            string sOutS0_1ADC0Tip = ".DW    ";

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    //實際有資料範圍
                    if (m_bRXTracePinFlag_Array[nCheckLoop] == true)       //判斷此Trace是否有使用
                    {
                        //有資料
                        lOutputValue += lRxTipGainS2S4_Array[nCheckLoop] * nGain;
                        nGain += 255;
                        nCount++;
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;

                    if (nCount <= 3)
                        sOutS0_1ADC0Tip += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                    else
                        sOutS0_1ADC0Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());

                    lOutputValue = 0;
                }

                nCheckLoop++;
            }

            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS0_1ADC0Ring = ".DW    ";

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    //實際有資料範圍
                    if (m_bRXTracePinFlag_Array[nCheckLoop] == true)       //判斷此Trace是否有使用
                    {
                        //有資料
                        lOutputValue += lRxRingGainS2S4_Array[nCheckLoop] * nGain;
                        nGain += 255;
                        nCount++;
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;

                    if (nCount <= 3)
                        sOutS0_1ADC0Ring += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                    else
                        sOutS0_1ADC0Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());

                    lOutputValue = 0;
                }

                nCheckLoop++;
            }
            #endregion

            #region S0 的 ADC1 的 TP Gain 產生
            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS0ADC1Tip = "";      //".DW    ",  ADC1並在ADC0後，不需要.DW

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    if (nCheckLoop <= 5)
                    {
                        //Check Rx
                        //CheckLoop 0~5 is Rx 42~47
                        //實際有資料範圍
                        if (m_bRXTracePinFlag_Array[nCheckLoop + 42] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lRxTipGainS2S4_Array[nCheckLoop + 42] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                    else
                    {
                        //Check Tx
                        //CheckLoop 6~41 is Tx 12~47
                        //實際有資料範圍
                        if (m_bTXTracePinFlag_Array[nCheckLoop - 6 + 12] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lTxTipGainS1S2_Array[nCheckLoop - 6 + 12] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;
                    sOutS0ADC1Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
                    lOutputValue = 0;
                }

                nCheckLoop++;
            }

            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS0ADC1Ring = "";      //".DW    ",  ADC1並在ADC0後，不需要.DW

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    if (nCheckLoop <= 5)
                    {
                        //Check Rx
                        //CheckLoop 0~5 is Rx 42~47
                        //實際有資料範圍
                        if (m_bRXTracePinFlag_Array[nCheckLoop + 42] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lRxRingGainS2S4_Array[nCheckLoop + 42] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                    else
                    {
                        //Check Tx
                        //CheckLoop 6~41 is Tx 12~47
                        //實際有資料範圍
                        if (m_bTXTracePinFlag_Array[nCheckLoop - 6 + 12] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lTxRingGainS1S2_Array[nCheckLoop - 6 + 12] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;
                    sOutS0ADC1Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
                    lOutputValue = 0;
                }

                nCheckLoop++;
            }
            #endregion

            #region S1 的 ADC1 的 TP Gain 產生
            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS1ADC1Tip = "";      //".DW    ",  ADC1並在ADC0後，不需要.DW

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    if (nCheckLoop <= 5)
                    {
                        //Check Rx
                        //CheckLoop 0~5 is Rx 42~47
                        //實際有資料範圍
                        if (m_bRXTracePinFlag_Array[nCheckLoop + 42] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lRxTipGainS2S4_Array[nCheckLoop + 42] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                    else
                    {
                        //Check Tx
                        //CheckLoop 6~41 is Tx 12~47
                        //實際有資料範圍
                        if (m_bTXTracePinFlag_Array[nCheckLoop - 6] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lTxTipGainS1S2_Array[nCheckLoop - 6] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;
                    sOutS1ADC1Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
                    lOutputValue = 0;
                }

                nCheckLoop++;
            }

            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS1ADC1Ring = "";      //".DW    ",  ADC1並在ADC0後，不需要.DW

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    if (nCheckLoop <= 5)
                    {
                        //Check Rx
                        //CheckLoop 0~5 is Rx 42~47
                        //實際有資料範圍
                        if (m_bRXTracePinFlag_Array[nCheckLoop + 42] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lRxRingGainS2S4_Array[nCheckLoop + 42] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                    else
                    {
                        //Check Tx
                        //CheckLoop 6~41 is Tx 12~47
                        //實際有資料範圍
                        if (m_bTXTracePinFlag_Array[nCheckLoop - 6] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lTxRingGainS1S2_Array[nCheckLoop - 6] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;
                    sOutS1ADC1Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
                    lOutputValue = 0;
                }

                nCheckLoop++;
            }
            #endregion

            #region S2 的 ADC0 的 TP Gain 產生
            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS2ADC0Tip = ".DW    ";

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    if (nCheckLoop <= 35)
                    {
                        //Check Tx
                        //CheckLoop 0~35 is Tx 12~47
                        //實際有資料範圍
                        if (m_bTXTracePinFlag_Array[nCheckLoop + 12] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lTxTipGainS1S2_Array[nCheckLoop + 12] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                    else
                    {
                        //Check Rx
                        //CheckLoop 36~41 is Rx 36~41
                        //實際有資料範圍
                        if (m_bRXTracePinFlag_Array[nCheckLoop - 36 + 36] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lRxTipGainS2S4_Array[nCheckLoop - 36 + 36] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;

                    if (nCount <= 3)
                        sOutS2ADC0Tip += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                    else
                        sOutS2ADC0Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
                    lOutputValue = 0;
                }

                nCheckLoop++;
            }

            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS2ADC0Ring = ".DW    ";

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    if (nCheckLoop <= 35)
                    {
                        //Check Tx
                        //CheckLoop 0~35 is Tx 12~47
                        //實際有資料範圍
                        if (m_bTXTracePinFlag_Array[nCheckLoop + 12] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lTxRingGainS1S2_Array[nCheckLoop + 12] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                    else
                    {
                        //Check Rx
                        //CheckLoop 36~41 is Rx 36~41
                        //實際有資料範圍
                        if (m_bRXTracePinFlag_Array[nCheckLoop - 36 + 36] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lRxRingGainS2S4_Array[nCheckLoop - 36 + 36] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;

                    if (nCount <= 3)
                        sOutS2ADC0Ring += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                    else
                        sOutS2ADC0Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
                    lOutputValue = 0;
                }

                nCheckLoop++;
            }
            #endregion

            #region S3 的 ADC0 的 TP Gain 產生
            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS3ADC0Tip = ".DW    ";

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    if (nCheckLoop <= 35)
                    {
                        //Check Tx
                        //CheckLoop 0~35 is Tx 0~35
                        //實際有資料範圍
                        if (m_bTXTracePinFlag_Array[nCheckLoop] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lTxTipGainS1S2_Array[nCheckLoop] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                    else
                    {
                        //Check Rx
                        //CheckLoop 36~41 is Rx 36~41
                        //實際有資料範圍
                        if (m_bRXTracePinFlag_Array[nCheckLoop - 36 + 36] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lRxTipGainS2S4_Array[nCheckLoop - 36 + 36] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;

                    if (nCount <= 3)
                        sOutS3ADC0Tip += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                    else
                        sOutS3ADC0Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
                    lOutputValue = 0;
                }

                nCheckLoop++;
            }

            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS3ADC0Ring = ".DW    ";

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    if (nCheckLoop <= 35)
                    {
                        //Check Tx
                        //CheckLoop 0~35 is Tx 0~35
                        //實際有資料範圍
                        if (m_bTXTracePinFlag_Array[nCheckLoop] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lTxRingGainS1S2_Array[nCheckLoop] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                    else
                    {
                        //Check Rx
                        //CheckLoop 36~41 is Rx 36~41
                        //實際有資料範圍
                        if (m_bRXTracePinFlag_Array[nCheckLoop - 36 + 36] == true)       //判斷此Trace是否有使用
                        {
                            //有資料
                            lOutputValue += lRxRingGainS2S4_Array[nCheckLoop - 36 + 36] * nGain;
                            nGain += 255;
                            nCount++;
                        }
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;

                    if (nCount <= 3)
                        sOutS3ADC0Ring += string.Format("0x{0}", lOutputValue.ToString("x4").ToUpper());
                    else
                        sOutS3ADC0Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
                    lOutputValue = 0;
                }

                nCheckLoop++;
            }
            #endregion

            #region S2 & S3 的 ADC1 的 TP Gain 產生
            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS2_3ADC1Tip = "";      //".DW    ",  ADC1並在ADC0後，不需要.DW

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    //實際有資料範圍
                    if (m_bRXTracePinFlag_Array[nCheckLoop + 42] == true)       //判斷此Trace是否有使用
                    {
                        //有資料
                        lOutputValue += lRxTipGainS2S4_Array[nCheckLoop + 42] * nGain;
                        nGain += 255;
                        nCount++;
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;
                    sOutS2_3ADC1Tip += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
                    lOutputValue = 0;
                }

                nCheckLoop++;
            }

            nCount = 1;
            nCheckLoop = 0;
            nGain = 1;
            lOutputValue = 0;
            string sOutS2_3ADC1Ring = "";      //".DW    ",  ADC1並在ADC0後，不需要.DW

            while (nCount <= 42)
            {
                if (nCheckLoop <= 41)
                {
                    //實際有資料範圍
                    if (m_bRXTracePinFlag_Array[nCheckLoop + 42] == true)       //判斷此Trace是否有使用
                    {
                        //有資料
                        lOutputValue += lRxRingGainS2S4_Array[nCheckLoop + 42] * nGain;
                        nGain += 255;
                        nCount++;
                    }
                }
                else
                {
                    //將剩下的資料填滿0x2020
                    lOutputValue += nInitial100 * nGain;
                    nGain += 255;
                    nCount++;
                }

                if (nGain >= 511)   //Done a Word '1+255+255 = 511
                {
                    nGain = 1;
                    sOutS2_3ADC1Ring += string.Format(",    0x{0}", lOutputValue.ToString("x4").ToUpper());
                    lOutputValue = 0;
                }

                nCheckLoop++;
            }
            #endregion

            #region Merge S0/S1/S2/S3 Tip/Ring Data
            string sOutTRxS_Tip_S0 = sOutS0_1ADC0Tip + sOutS0ADC1Tip;       //S0 Tip
            string sOutTRxS_Ring_S0 = sOutS0_1ADC0Ring + sOutS0ADC1Ring;    //S0 Ring
            string sOutTRxS_Tip_S1 = sOutS0_1ADC0Tip + sOutS1ADC1Tip;       //S1 Tip
            string sOutTRxS_Ring_S1 = sOutS0_1ADC0Ring + sOutS1ADC1Ring;    //S1 Ring
            string sOutTRxS_Tip_S2 = sOutS2ADC0Tip + sOutS2_3ADC1Tip;       //S2 Tip
            string sOutTRxS_Ring_S2 = sOutS2ADC0Ring + sOutS2_3ADC1Ring;    //S2 Ring
            string sOutTRxS_Tip_S3 = sOutS3ADC0Tip + sOutS2_3ADC1Tip;       //S3 Tip
            string sOutTRxS_Ring_S3 = sOutS3ADC0Ring + sOutS2_3ADC1Ring;    //S3 Ring
            #endregion
            #endregion

            #region FW TP Gain
            lRxTipFWGain_Array = m_cDataValue_List[nFileIndex].m_cRXTPGainValue.m_lTipFWGain_Array;
            lTxTipFWGain_Array = m_cDataValue_List[nFileIndex].m_cTXTPGainValue.m_lTipFWGain_Array;

            string sOutRxFWGain = ".DW    ";
            nCount = 0;

            for (int nTraceIndex = 0; nTraceIndex <= 100; nTraceIndex++)
            {
                if (nTraceIndex >= m_bRXTracePinFlag_Array.Length)
                    break;

                if (m_bRXTracePinFlag_Array[nTraceIndex] == true)      //判斷此Trace是否有使用
                {
                    if (nCount <= 0)
                        sOutRxFWGain += string.Format("{0}", lRxTipFWGain_Array[nTraceIndex].ToString());
                    else
                        sOutRxFWGain += string.Format(",    {0}", lRxTipFWGain_Array[nTraceIndex].ToString());

                    nCount++;
                }
            }

            string sOutTxFWGain = ".DW    ";
            nCount = 0;

            for (int nTraceIndex = 0; nTraceIndex <= 100; nTraceIndex++)
            {
                if (nTraceIndex >= m_bTXTracePinFlag_Array.Length)
                    break;

                if (m_bTXTracePinFlag_Array[nTraceIndex] == true)      //判斷此Trace是否有使用
                {
                    if (nCount <= 0)
                        sOutTxFWGain += string.Format("{0}", lTxTipFWGain_Array[nTraceIndex].ToString());
                    else
                        sOutTxFWGain += string.Format(",    {0}", lTxTipFWGain_Array[nTraceIndex].ToString());

                    nCount++;
                }
            }
            #endregion

            #region Output TP Gain Table
            string sTPGainTable = ".if (Pen_TP_Gain_Option != Pen_TP_Gain_Off)" + Environment.NewLine;
            sTPGainTable += ".if (cSolution_ID == cEKTH7315x1)" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += ".IF (_Pen_Tx0Tx1_Special_Gain == Pen_Tx0Tx1_Special_Gain_ON)" + Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_S2_Tx:" + Environment.NewLine;
            sTPGainTable += sOutS2_Tx + Environment.NewLine;
            sTPGainTable += ".ENDIF" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_Tx:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOut_TxTip + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOut_TxRing + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_Rx_S0:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOut_RxS0Tip + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOut_RxS0Ring + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_Rx_S1:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOut_RxS1Tip + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOut_RxS1Ring + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_Rx_S2:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOut_RxS2Tip + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOut_RxS2Ring + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += ".elseif (cSolution_ID == cEKTH6315x1)   ;===========================================" + Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_S0:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOutTRxS_Tip_S0 + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOutTRxS_Ring_S0 + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_S1:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOutTRxS_Tip_S1 + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOutTRxS_Ring_S1 + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_S2:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOutTRxS_Tip_S2 + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOutTRxS_Ring_S2 + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_S3:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOutTRxS_Tip_S3 + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOutTRxS_Ring_S3 + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += ".elseif (cSolution_ID == cEKTH6308x1)   ;===========================================" + Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_Rx_HF:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOutAllRx_Tip + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOutAllRx_Ring + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_Tx_HF:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOutAllTx_Tip + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOutAllTx_Ring + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += ".elseif (cSolution_ID == cEKTH7318x1)   ;===========================================" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_Tx:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOut_TxTip7318 + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOut_TxRing7318 + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_Rx_S0:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOut_RxS0Tip7318 + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOut_RxS0Ring7318 + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_Rx_S1:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOut_RxS1Tip7318 + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOut_RxS1Ring7318 + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_HF_Rx_S2:" + Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Tip)" + Environment.NewLine;
            sTPGainTable += sOut_RxS2Tip7318 + Environment.NewLine;
            sTPGainTable += ".elseif (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += sOut_RxS2Ring7318 + Environment.NewLine;
            sTPGainTable += ".endif" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += ".endif //(cSolution_ID == cEKTH7315x1)" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += ".if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_All_Rx:" + Environment.NewLine;
            sTPGainTable += sOutRxFWGain + Environment.NewLine;
            sTPGainTable += "ActivePen_TP_Gain_TRxS_All_Tx:" + Environment.NewLine;
            sTPGainTable += sOutTxFWGain + Environment.NewLine;
            sTPGainTable += ".endif //.if (_Pen_TP_Gain_TRxS_HF == Pen_TP_Gain_TRxS_HF_Ring)" + Environment.NewLine;
            sTPGainTable += Environment.NewLine;
            sTPGainTable += ".endif //(Pen_TP_Gain_Option != Pen_TP_Gain_Off)";
            #endregion

            #region Save TP Gain Table
            FileStream fsFile = new FileStream(sTPGainTableFilePath, FileMode.Create);
            StreamWriter swFile = new StreamWriter(fsFile);

            try
            {
                swFile.WriteLine(sTPGainTable);
            }
            finally
            {
                swFile.Flush();
                swFile.Close();
                fsFile.Close();
            }
            #endregion
            #endregion
        }

        protected void OutputResultData()
        {
            WriteReferenceDataFile(m_sProjectName, m_sReferenceFilePath);
            WriteStepListDataFile(m_sProjectName, m_sStepListPath);

            if (m_bErrorFlag == false)
            {
                FileProcess cFileProcess = new FileProcess(m_cfrmMain);
                cFileProcess.WriteResultListTxtFile(m_eSubStep, m_nSubStepState);
            }

            OutputMessage("Analysis Complete!!");
        }

        private void WriteRaw5TDataFile(string sDataTitle, int nFileIndex, string sFilePath, bool bRXTraceTypeFlag)
        {
            string sTraceType = (bRXTraceTypeFlag == true) ? "RX" : "TX";

            List<Raw5TData> cRaw5TData_Tip_List = new List<Raw5TData>();
            List<Raw5TData> cRaw5TData_Ring_List = new List<Raw5TData>();

            if (bRXTraceTypeFlag == true)
            {
                cRaw5TData_Tip_List = m_cDataValue_List[nFileIndex].m_cRXRaw5TData_BHFTip_List;
                cRaw5TData_Ring_List = m_cDataValue_List[nFileIndex].m_cRXRaw5TData_BHFRing_List;
            }
            else
            {
                cRaw5TData_Tip_List = m_cDataValue_List[nFileIndex].m_cTXRaw5TData_BHFTip_List;
                cRaw5TData_Ring_List = m_cDataValue_List[nFileIndex].m_cTXRaw5TData_BHFRing_List;
            }

            string[] sColumnName_Array = new string[] 
            { 
                "Tip Index",
                string.Format("{0}1", sTraceType),
                string.Format("{0}2", sTraceType),
                string.Format("{0}3", sTraceType),
                string.Format("{0}4", sTraceType),
                string.Format("{0}5", sTraceType), 
                "",
                "Ring Index",
                string.Format("{0}1", sTraceType),
                string.Format("{0}2", sTraceType),
                string.Format("{0}3", sTraceType),
                string.Format("{0}4", sTraceType),
                string.Format("{0}5", sTraceType) 
            };

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sDataTitle));

                for (int nColumnIndex = 0; nColumnIndex < sColumnName_Array.Length; nColumnIndex++)
                {
                    if (nColumnIndex < sColumnName_Array.Length - 1)
                        sw.Write(string.Format("{0},", sColumnName_Array[nColumnIndex]));
                    else
                        sw.WriteLine(sColumnName_Array[nColumnIndex]);
                }

                int nDataLength = GetMaxDataLength(cRaw5TData_Tip_List, cRaw5TData_Ring_List);

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    string sText = "";
                    string sRaw5TValue = "";

                    if (cRaw5TData_Tip_List.Count - 1 >= nDataIndex)
                        sRaw5TValue = SetRaw5TValue(cRaw5TData_Tip_List[nDataIndex]);
                    else
                        sRaw5TValue = ",,,,,";

                    sText += string.Format("{0},,", sRaw5TValue);

                    if (cRaw5TData_Ring_List.Count - 1 >= nDataIndex)
                        sRaw5TValue = SetRaw5TValue(cRaw5TData_Ring_List[nDataIndex]);
                    else
                        sRaw5TValue = "";

                    sText += string.Format("{0}", sRaw5TValue);

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

        private int GetMaxDataLength(List<Raw5TData> cRaw5TData_Tip_List, List<Raw5TData> cRaw5TData_Ring_List)
        {
            int nDataLenth = (cRaw5TData_Tip_List.Count > cRaw5TData_Ring_List.Count) ? cRaw5TData_Tip_List.Count : cRaw5TData_Ring_List.Count;

            return nDataLenth;
        }

        private string SetRaw5TValue(Raw5TData cRaw5TData)
        {
            string sRaw5TValue = string.Format("{0},", cRaw5TData.m_nTraceIndex.ToString());

            for (int nTraceDataIndex = 1; nTraceDataIndex <= 5; nTraceDataIndex++)
            {
                switch (nTraceDataIndex)
                {
                    case 1:
                        sRaw5TValue += string.Format("{0},", cRaw5TData.m_nRawData_1T.ToString());
                        break;
                    case 2:
                        sRaw5TValue += string.Format("{0},", cRaw5TData.m_nRawData_2T.ToString());
                        break;
                    case 3:
                        sRaw5TValue += string.Format("{0},", cRaw5TData.m_nRawData_3T.ToString());
                        break;
                    case 4:
                        sRaw5TValue += string.Format("{0},", cRaw5TData.m_nRawData_4T.ToString());
                        break;
                    case 5:
                        sRaw5TValue += cRaw5TData.m_nRawData_5T.ToString();
                        break;
                }
            }

            return sRaw5TValue;
        }

        private void WriteIntegrationDataFile(string sFilePath, int nErrorFlag)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_INTEGRATION);

                sw.WriteLine();

                sw.WriteLine("Information");
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
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WriteReferenceDataFile(string sProjectName, string sFilePath)
        {
            string[] sColumnName_Array = new string[]
            {
                SpecificText.m_sRank,
                SpecificText.m_sFrequency,
                SpecificText.m_sPH1,
                SpecificText.m_sPH2
            };

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_REFERENCE);

                sw.WriteLine();

                sw.WriteLine("Project Information");
                sw.WriteLine(string.Format("Flow Step,{0}", m_eSubStep.ToString()));
                sw.WriteLine(string.Format("Control Mode,{0}", StringConvert.m_dictControlModeMappingTable[m_cfrmMain.m_nModeFlag]));
                sw.WriteLine(string.Format("RXTraceNumber,{0}", m_nRXTraceNumber));
                sw.WriteLine(string.Format("TXTraceNumber,{0}", m_nTXTraceNumber));

                List<int> nRankSort_List = GetRankSortList(m_cDataInfo_List);

                sw.WriteLine();

                int nStartTrace = m_nRXStartPin;
                int nEndTrace = m_nRXTraceNumber + m_nRXStartPin;

                sw.WriteLine("RX Tip Gain Data");

                foreach (string sColumnName in sColumnName_Array)
                    sw.Write(string.Format("{0},", sColumnName));

                for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                {
                    sw.Write(string.Format("{0}", Convert.ToString(nTraceIndex + 1)));

                    if (nTraceIndex < m_nRXTraceNumber - 1)
                        sw.Write(",");
                    else
                        sw.WriteLine();
                }

                foreach (int nRankValue in nRankSort_List)
                {
                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                        {
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));

                            for (int nTraceIndex = nStartTrace; nTraceIndex < nEndTrace; nTraceIndex++)
                            {
                                sw.Write(string.Format("{0}", Convert.ToString(m_cDataValue_List[nDataIndex].m_cRXTPGainValue.m_dAvgGainTip_Array[nTraceIndex])));

                                if (nTraceIndex < nEndTrace - 1)
                                    sw.Write(",");
                                else
                                    sw.WriteLine();
                            }
                        }
                    }
                }

                sw.WriteLine();

                sw.WriteLine("RX Ring Gain Data");

                foreach (string sColumnName in sColumnName_Array)
                    sw.Write(string.Format("{0},", sColumnName));

                for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                {
                    sw.Write(string.Format("{0}", Convert.ToString(nTraceIndex + 1)));

                    if (nTraceIndex < m_nRXTraceNumber - 1)
                        sw.Write(",");
                    else
                        sw.WriteLine();
                }

                foreach (int nRankValue in nRankSort_List)
                {
                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                        {
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));

                            for (int nTraceIndex = nStartTrace; nTraceIndex < nEndTrace; nTraceIndex++)
                            {
                                sw.Write(string.Format("{0}", Convert.ToString(m_cDataValue_List[nDataIndex].m_cRXTPGainValue.m_dAvgGainRing_Array[nTraceIndex])));

                                if (nTraceIndex < nEndTrace - 1)
                                    sw.Write(",");
                                else
                                    sw.WriteLine();
                            }
                        }
                    }
                }

                sw.WriteLine();

                sw.WriteLine("RX Tip FWGain After TPGain Data");

                foreach (string sColumnName in sColumnName_Array)
                    sw.Write(string.Format("{0},", sColumnName));

                for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                {
                    sw.Write(string.Format("{0}", Convert.ToString(nTraceIndex + 1)));

                    if (nTraceIndex < m_nRXTraceNumber - 1)
                        sw.Write(",");
                    else
                        sw.WriteLine();
                }

                foreach (int nRankValue in nRankSort_List)
                {
                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                        {
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));

                            for (int nTraceIndex = nStartTrace; nTraceIndex < nEndTrace; nTraceIndex++)
                            {
                                sw.Write(string.Format("{0}", Convert.ToString(m_cDataValue_List[nDataIndex].m_cRXTPGainValue.m_dTipFWGainAfterTPGain_Array[nTraceIndex])));

                                if (nTraceIndex < nEndTrace - 1)
                                    sw.Write(",");
                                else
                                    sw.WriteLine();
                            }
                        }
                    }
                }

                sw.WriteLine();

                nStartTrace = m_nTXStartPin;
                nEndTrace = m_nTXTraceNumber + m_nTXStartPin;

                sw.WriteLine("TX Tip Gain Data");

                foreach (string sColumnName in sColumnName_Array)
                    sw.Write(string.Format("{0},", sColumnName));

                for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                {
                    sw.Write(string.Format("{0}", Convert.ToString(nTraceIndex + 1)));

                    if (nTraceIndex < m_nTXTraceNumber - 1)
                        sw.Write(",");
                    else
                        sw.WriteLine();
                }

                foreach (int nRankValue in nRankSort_List)
                {
                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                        {
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));

                            for (int nTraceIndex = nStartTrace; nTraceIndex < nEndTrace; nTraceIndex++)
                            {
                                sw.Write(string.Format("{0}", Convert.ToString(m_cDataValue_List[nDataIndex].m_cTXTPGainValue.m_dAvgGainTip_Array[nTraceIndex])));

                                if (nTraceIndex < nEndTrace - 1)
                                    sw.Write(",");
                                else
                                    sw.WriteLine();
                            }
                        }
                    }
                }

                sw.WriteLine();

                sw.WriteLine("TX Ring Gain Data");

                foreach (string sColumnName in sColumnName_Array)
                    sw.Write(string.Format("{0},", sColumnName));

                for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                {
                    sw.Write(string.Format("{0}", Convert.ToString(nTraceIndex + 1)));

                    if (nTraceIndex < m_nTXTraceNumber - 1)
                        sw.Write(",");
                    else
                        sw.WriteLine();
                }

                foreach (int nRankValue in nRankSort_List)
                {
                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                        {
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));

                            for (int nTraceIndex = nStartTrace; nTraceIndex < nEndTrace; nTraceIndex++)
                            {
                                sw.Write(string.Format("{0}", Convert.ToString(m_cDataValue_List[nDataIndex].m_cTXTPGainValue.m_dAvgGainRing_Array[nTraceIndex])));

                                if (nTraceIndex < nEndTrace - 1)
                                    sw.Write(",");
                                else
                                    sw.WriteLine();
                            }
                        }
                    }
                }

                sw.WriteLine();

                sw.WriteLine("TX Tip FWGain After TPGain Data");

                foreach (string sColumnName in sColumnName_Array)
                    sw.Write(string.Format("{0},", sColumnName));

                for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                {
                    sw.Write(string.Format("{0}", Convert.ToString(nTraceIndex + 1)));

                    if (nTraceIndex < m_nTXTraceNumber - 1)
                        sw.Write(",");
                    else
                        sw.WriteLine();
                }

                foreach (int nRankValue in nRankSort_List)
                {
                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                        {
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));

                            for (int nTraceIndex = nStartTrace; nTraceIndex < nEndTrace; nTraceIndex++)
                            {
                                sw.Write(string.Format("{0}", Convert.ToString(m_cDataValue_List[nDataIndex].m_cTXTPGainValue.m_dTipFWGainAfterTPGain_Array[nTraceIndex])));

                                if (nTraceIndex < nEndTrace - 1)
                                    sw.Write(",");
                                else
                                    sw.WriteLine();
                            }
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
    }
}
