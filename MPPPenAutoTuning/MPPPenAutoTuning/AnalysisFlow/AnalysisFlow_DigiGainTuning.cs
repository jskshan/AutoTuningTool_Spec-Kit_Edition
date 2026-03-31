using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class AnalysisFlow_DigiGainTuning : AnalysisFlow
    {
        private string m_sDigiGainRatio = "";

        private const int m_nDIGIGAINSCALE_HIGHLIMIT = 399;
        private const int m_nDIGIGAINSCALE_LOWLIMIT = 1;

        private const int m_nDRAWTYPECOUNT = 2;

        private const int m_nNORMALREPORTDATALENGTH = 11;

        private int m_nTraceTypeByteLocation = 13;

        private int m_nDataTypeByteLocation_NewFormat = 3;

        private int m_nCompensatePower = 20480;

        private int m_nDigiGainScaleHB = 395;
        private int m_nDigiGainScaleLB = 5;

        private int m_nRXValidReportNumber = 1500;
        private int m_nTXValidReportNumber = 1500;

        private string[] m_sDrawType_Array = new string[m_nDRAWTYPECOUNT] 
        { 
            StringConvert.m_sDRAWTYPE_HORIZONTAL,
            StringConvert.m_sDRAWTYPE_VERTICAL 
        };

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

        /*
        public class DigiGainData
        {
            public Raw5TData m_cRaw5TData_Beacon = new Raw5TData();
            public Raw5TData m_cRaw5TData_PTHF = new Raw5TData();
            public Raw5TData m_cRaw5TData_BHF = new Raw5TData();
        }
        */

        public class DigiGainValue
        {
            public double m_dBeacon3TMean = 0.0;
            public double m_dPTHF3TMean = 0.0;
            public double m_dBHF3TMean = 0.0;
        }

        public class DataValue
        {
            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public int m_nDefaultDigiGain_P0 = -1;
            public int m_nDefaultDigiGain_Beacon_Rx = -1;
            public int m_nDefaultDigiGain_Beacon_Tx = -1;
            public int m_nDefaultDigiGain_PTHF_Rx = -1;
            public int m_nDefaultDigiGain_PTHF_Tx = -1;
            public int m_nDefaultDigiGain_BHF_Rx = -1;
            public int m_nDefaultDigiGain_BHF_Tx = -1;

            public int m_nDefaultScaleDigiGain_P0 = -1;
            public int m_nDefaultScaleDigiGain_Beacon_Rx = -1;
            public int m_nDefaultScaleDigiGain_Beacon_Tx = -1;
            public int m_nDefaultScaleDigiGain_PTHF_Rx = -1;
            public int m_nDefaultScaleDigiGain_PTHF_Tx = -1;
            public int m_nDefaultScaleDigiGain_BHF_Rx = -1;
            public int m_nDefaultScaleDigiGain_BHF_Tx = -1;

            public int m_nParameterScaleDigiGain_P0 = -1;
            public int m_nParameterScaleDigiGain_Beacon_Rx = -1;
            public int m_nParameterScaleDigiGain_Beacon_Tx = -1;
            public int m_nParameterScaleDigiGain_PTHF_Rx = -1;
            public int m_nParameterScaleDigiGain_PTHF_Tx = -1;
            public int m_nParameterScaleDigiGain_BHF_Rx = -1;
            public int m_nParameterScaleDigiGain_BHF_Tx = -1;

            public DigiGainValue m_cRXDigiGainValue = new DigiGainValue();
            public DigiGainValue m_cTXDigiGainValue = new DigiGainValue();

            /*
            public List<DigiGainData> m_cRXDigiGainData_List = new List<DigiGainData>();
            public List<DigiGainData> m_cTXDigiGainData_List = new List<DigiGainData>();
            */

            public List<Raw5TData> m_cRXRaw5TData_Beacon_List = new List<Raw5TData>();
            public List<Raw5TData> m_cRXRaw5TData_PTHF_List = new List<Raw5TData>();
            public List<Raw5TData> m_cRXRaw5TData_BHF_List = new List<Raw5TData>();
            public List<Raw5TData> m_cTXRaw5TData_Beacon_List = new List<Raw5TData>();
            public List<Raw5TData> m_cTXRaw5TData_PTHF_List = new List<Raw5TData>();
            public List<Raw5TData> m_cTXRaw5TData_BHF_List = new List<Raw5TData>();
        }

        private List<DataValue> m_cDataValue_List = null;

        public class DrawTypeInfo
        {
            public SubTuningStep m_eSubStep = SubTuningStep.ELSE;
            public int m_nSettingPH1 = -1;
            public int m_nSettingPH2 = -1;
            public int m_nReadPH1 = -1;
            public int m_nReadPH2 = -1;
            public double m_dFrequency = -1;

            public int m_nRankIndex = -1;

            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public int m_nLineCount = -1;
        }

        private DrawTypeInfo[] m_cDrawTypeInfo_Array = null;

        private void ClearDataArray()
        {
            m_byteReport_List.Clear();
            m_byteData_List.Clear();
        }

        public AnalysisFlow_DigiGainTuning(FlowStep cFlowStep, frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
            m_sFileDirectoryPath = m_cfrmMain.m_sFileDirectoryPath;
            m_sRecordProjectName = m_cfrmMain.m_sRecordProjectName;

            InitializeParameter(cFlowStep);

            m_cDataValue_List = new List<DataValue>();

            if (ParamAutoTuning.m_nDGTDrawType == 1)
                m_cDrawTypeInfo_Array = new DrawTypeInfo[m_nDRAWTYPECOUNT];

            SetRecordInfo();
            CreateErrorInfo();
        }

        public override void LoadAnalysisParameter()
        {
            m_nReportDataLength = ParamAutoTuning.m_nReportDataLength;
            m_nShiftStartByte = ParamAutoTuning.m_nShiftStartByte;
            m_nShiftByteNumber = ParamAutoTuning.m_nShiftByteNumber;

            m_nCompensatePower = ParamAutoTuning.m_nDGTCompensatePower;

            m_nDigiGainScaleHB = ParamAutoTuning.m_nDGTDigiGainScaleHB;
            m_nDigiGainScaleLB = ParamAutoTuning.m_nDGTDigiGainScaleLB;

            m_nRXValidReportNumber = ParamAutoTuning.m_nDGTRXValidReportNumber;
            m_nTXValidReportNumber = ParamAutoTuning.m_nDGTTXValidReportNumber;

            m_nNormalReportDataLength = m_nNORMALREPORTDATALENGTH;

            m_sDigiGainRatio = string.Format("*{0}/{1}", ParamAutoTuning.m_nDGTMultiplyValue.ToString("D"), ParamAutoTuning.m_nDGTDividValue.ToString("D"));
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

        public void GetData(List<DigiGainTuningParameter> cParameter_List)
        {
            // 取得資料夾內所有檔案
            string[] sValidDataFilePath_Array = GetValidReportDataFile(cParameter_List);

            if (sValidDataFilePath_Array == null || sValidDataFilePath_Array.Length == 0)
            {
                m_sErrorMessage = "No Valid Data!!";
                OutputMessage("No Valid Data!!");
                m_bErrorFlag = true;
                return;
            }

            int nFileCount = sValidDataFilePath_Array.Length;

            OutputMainStatusStrip("Analysing...", 0, nFileCount, frmMain.m_nInitialFlag);

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
                string sComputeFilePath = string.Format(@"{0}\Compute Data.csv", sComputeFolderPath);

                int nLineCounter = 0;
                string sLine = "";

                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                int nRankIndex = -1;
                int nDrawTypeState = 0;

                int nRxReportNumber_NewFormat = 0;
                int nTxReportNumber_NewFormat = 0;

                GetFileInfoFromReportData(cDataInfo, cDataValue, sFilePath);

                if (CheckInfoIsCorrect(ref sMessage, cDataInfo, cDataValue) == false)
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

                        DigiGainTuningParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2);

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

                            bool bFormatErrorFlag = false;

                            if (m_cDataInfo_List[nFileIndex].m_n5TRawDataType != 1)
                            {
                                if (byteData_Array[0] != 0x07 ||
                                    byteData_Array[2] != 0xFF || 
                                    byteData_Array[3] != 0xFF ||
                                    byteData_Array[4] != 0xFF || 
                                    byteData_Array[5] != 0xFF ||
                                    byteData_Array[9] != 0xDD || 
                                    byteData_Array[10] != 0xEE)
                                    bFormatErrorFlag = true;
                            }
                            else
                            {
                                if (byteData_Array[0] != 0x07)
                                    bFormatErrorFlag = true;
                            }

                            if (bFormatErrorFlag == true)
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

                        if (ParamAutoTuning.m_nDGTDrawType == 1)
                        {
                            if (cDataInfo.m_n5TRawDataType != 1)
                            {
                                byte byteTraceTypeByte = Convert.ToByte(sSplit_Array[m_nTraceTypeByteLocation - 1], 16);

                                if (nDrawTypeState == 1 && (byteTraceTypeByte & 0x01) != 0)
                                    continue;
                                else if (nDrawTypeState == 2 && (byteTraceTypeByte & 0x01) == 0)
                                    continue;
                            }
                        }

                        if (cDataInfo.m_n5TRawDataType == 1)
                        {
                            if (Convert.ToByte(sSplit_Array[m_nDataTypeByteLocation_NewFormat - 1], 16) != 0x11 &&
                                Convert.ToByte(sSplit_Array[m_nDataTypeByteLocation_NewFormat - 1], 16) != 0x12 &&
                                Convert.ToByte(sSplit_Array[m_nDataTypeByteLocation_NewFormat - 1], 16) != 0x23)
                                continue;
                        }

                        for (int nIndex = 0; nIndex < sSplit_Array.Length - 1; nIndex++)
                        {
                            byte byteRawData = Convert.ToByte(sSplit_Array[nIndex], 16);
                            m_byteReport_List.Add(byteRawData);
                        }

                        m_byteData_List.Add(new List<byte>(m_byteReport_List));

                        if (ParamAutoTuning.m_nDGTDrawType == 1)
                        {
                            if (cDataInfo.m_n5TRawDataType == 1)
                            {
                                if (nDrawTypeState == 1)
                                    nRxReportNumber_NewFormat++;
                                else if (nDrawTypeState == 2)
                                    nTxReportNumber_NewFormat++;
                            }
                        }
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
                    if (cDataInfo.m_n5TRawDataType != 1)
                    {
                        Raw5TData m_cRaw5TData_Beacon = new Raw5TData();
                        Raw5TData m_cRaw5TData_PTHF = new Raw5TData();
                        Raw5TData m_cRaw5TData_BHF = new Raw5TData();

                        int nTraceType = m_byteData_List[nDataIndex][m_nTraceTypeByteLocation - 1];
                        bool bRXTraceTypeFlag = true;
                        int nTraceNumber = 0;

                        //RX Data
                        if ((nTraceType & 0x01) == 0)
                        {
                            bRXTraceTypeFlag = true;
                            nTraceNumber = m_nRXTraceNumber;
                        }
                        else
                        {
                            bRXTraceTypeFlag = false;
                            nTraceNumber = m_nTXTraceNumber;
                        }

                        GetRaw5TData(m_cRaw5TData_Beacon, m_byteData_List[nDataIndex], DigiGainDataFormat.DataByteLocation.BEACON, nTraceNumber);
                        GetRaw5TData(m_cRaw5TData_PTHF, m_byteData_List[nDataIndex], DigiGainDataFormat.DataByteLocation.PTHF, nTraceNumber);
                        GetRaw5TData(m_cRaw5TData_BHF, m_byteData_List[nDataIndex], DigiGainDataFormat.DataByteLocation.BHF, nTraceNumber);

                        //RX Data
                        if (bRXTraceTypeFlag == true)
                        {
                            cDataValue.m_cRXRaw5TData_Beacon_List.Add(m_cRaw5TData_Beacon);
                            cDataValue.m_cRXRaw5TData_PTHF_List.Add(m_cRaw5TData_PTHF);
                            cDataValue.m_cRXRaw5TData_BHF_List.Add(m_cRaw5TData_BHF);
                        }
                        //TX Data
                        else
                        {
                            cDataValue.m_cTXRaw5TData_Beacon_List.Add(m_cRaw5TData_Beacon);
                            cDataValue.m_cTXRaw5TData_PTHF_List.Add(m_cRaw5TData_PTHF);
                            cDataValue.m_cTXRaw5TData_BHF_List.Add(m_cRaw5TData_BHF);
                        }
                    }
                    else
                    {
                        int nTraceType = MainConstantParameter.m_nTRACETYPE_RX;

                        if (nDataIndex < nRxReportNumber_NewFormat)
                            nTraceType = MainConstantParameter.m_nTRACETYPE_RX;
                        else if (nDataIndex >= nRxReportNumber_NewFormat && nDataIndex < nRxReportNumber_NewFormat + nTxReportNumber_NewFormat)
                            nTraceType = MainConstantParameter.m_nTRACETYPE_TX;
                        else
                            break;

                        Raw5TData cRaw5TData_RX = new Raw5TData();
                        Raw5TData cRaw5TData_TX = new Raw5TData();

                        int nDataType = m_byteData_List[nDataIndex][m_nDataTypeByteLocation_NewFormat - 1];

                        GetRaw5TData_NewFormat(cRaw5TData_RX, cRaw5TData_TX, m_byteData_List[nDataIndex]);

                        //Beacon
                        if (nDataType == 0x11)
                        {
                            if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                                cDataValue.m_cRXRaw5TData_Beacon_List.Add(cRaw5TData_RX);
                            else if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                                cDataValue.m_cTXRaw5TData_Beacon_List.Add(cRaw5TData_TX);
                        }
                        //PTHF
                        else if (nDataType == 0x12)
                        {
                            if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                                cDataValue.m_cRXRaw5TData_PTHF_List.Add(cRaw5TData_RX);
                            else if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                                cDataValue.m_cTXRaw5TData_PTHF_List.Add(cRaw5TData_TX);
                        }
                        //BHF
                        else if (nDataType == 0x23)
                        {
                            if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
                                cDataValue.m_cRXRaw5TData_BHF_List.Add(cRaw5TData_RX);
                            else if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
                                cDataValue.m_cTXRaw5TData_BHF_List.Add(cRaw5TData_TX);
                        }
                    }
                }

                WriteRaw5TData("RX 5T Raw Data", nFileIndex, sRXRawFilePath, true);
                WriteRaw5TData("TX 5T Raw Data", nFileIndex, sTXRawFilePath, false);

                if (CheckDataNumberIsEnough(nFileIndex, sFilePath) == false)
                    continue;

                sMessage = "";

                if (ComputeDataMean(ref sMessage, nFileIndex, MainConstantParameter.m_nTRACETYPE_RX) == false)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0400;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", sMessage, Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = sMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                if (ComputeDataMean(ref sMessage, nFileIndex, MainConstantParameter.m_nTRACETYPE_TX) == false)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0800;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", sMessage, Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = sMessage;
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteComputeDataFile(nFileIndex, sComputeFilePath);

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
            if (m_bErrorFlag == false)
            {
                if (ComputeDigiGainScale() == true)
                {
                    m_sErrorMessage = "Some Data Analysis Error!!";
                    OutputMessage("Some Data Analysis Error!!");
                    m_bErrorFlag = true;
                }
            }

            OutputResultData();
        }

        private void GetFileInfoFromReportData(DataInfo cDataInfo, DataValue cDataValue, string sFilePath)
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
                            cDrawTypeInfo.m_nLineCount = nLineCount;
                            lGetDrawTypeInfoFlag = 0;
                        }
                        else if (sLine == string.Format("====={0}=====", StringConvert.m_sDRAWTYPE_VERTICAL))
                        {
                            sDrawLineState = StringConvert.m_sDRAWTYPE_VERTICAL;
                            cDrawTypeInfo = new DrawTypeInfo();
                            cDrawTypeInfo.m_nLineCount = nLineCount;
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
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_RDIGIGAIN_P0, sLine, 0x000200, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX, sLine, 0x000400, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX, sLine, 0x000800, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_RDIGIGAIN_PTHF_RX, sLine, 0x001000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_RDIGIGAIN_PTHF_TX, sLine, 0x002000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_RDIGIGAIN_BHF_RX, sLine, 0x004000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_RDIGIGAIN_BHF_TX, sLine, 0x008000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_5TRAWDATATYPE, sLine, 0x010000, m_nINFOTYPE_INT);

                    if (ParamAutoTuning.m_nDGTDrawType != 1 && lGetInfoFlag == 0x1FFFF)
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

                                if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_P0)
                                    cDataValue.m_nDefaultDigiGain_P0 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX)
                                    cDataValue.m_nDefaultDigiGain_Beacon_Rx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX)
                                    cDataValue.m_nDefaultDigiGain_Beacon_Tx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_PTHF_RX)
                                    cDataValue.m_nDefaultDigiGain_PTHF_Rx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_PTHF_TX)
                                    cDataValue.m_nDefaultDigiGain_PTHF_Tx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BHF_RX)
                                    cDataValue.m_nDefaultDigiGain_BHF_Rx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BHF_TX)
                                    cDataValue.m_nDefaultDigiGain_BHF_Tx = nValue;
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

        private void GetDrawTypeInfo(ref long lGetInfoFlag, DrawTypeInfo cDrawTypeInfo, string sParameterName, string sLine, long lInfoFlag, int nValueType)
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

        private bool CheckInfoIsCorrect(ref string sErrorMessage, DataInfo cDataInfo, DataValue cDataValue)
        {
            CheckGeneralInfoIsCorrect(ref sErrorMessage, cDataInfo);

            if (cDataInfo.m_nRXTraceNumber <= 0)
                SetErrorMessage(ref sErrorMessage, "\"RX Trace Number\" Format Error");

            if (cDataInfo.m_nTXTraceNumber <= 0)
                SetErrorMessage(ref sErrorMessage, "\"TX Trace Number\" Format Error");

            if (cDataValue.m_nDefaultDigiGain_P0 < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RDIGIGAIN_P0));

            if (cDataValue.m_nDefaultDigiGain_Beacon_Rx < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX));

            if (cDataValue.m_nDefaultDigiGain_Beacon_Tx < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX));

            if (cDataValue.m_nDefaultDigiGain_PTHF_Rx < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RDIGIGAIN_PTHF_RX));

            if (cDataValue.m_nDefaultDigiGain_PTHF_Tx < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RDIGIGAIN_PTHF_TX));

            if (cDataValue.m_nDefaultDigiGain_BHF_Rx < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RDIGIGAIN_BHF_RX));

            if (cDataValue.m_nDefaultDigiGain_BHF_Tx < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RDIGIGAIN_BHF_TX));

            if (ParamAutoTuning.m_nDGTDrawType == 1)
            {
                if (cDataInfo.m_nRankIndex < 0)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RANKINDEX));

                int nDrawTypeIndex_Horizontal = Array.IndexOf(m_sDrawType_Array, StringConvert.m_sDRAWTYPE_HORIZONTAL);
                int nDrawTypeIndex_Vertical = Array.IndexOf(m_sDrawType_Array, StringConvert.m_sDRAWTYPE_VERTICAL);

                if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal] == null)
                {
                    SetErrorMessage(ref sErrorMessage, "Horizontal Data Format Not Match");
                    return false;
                }
                else if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal] == null)
                {
                    SetErrorMessage(ref sErrorMessage, "Vertical Data Format Not Match");
                    return false;
                }

                if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal].m_eSubStep != m_cDrawTypeInfo_Array[nDrawTypeIndex_Vertical].m_eSubStep)
                    SetErrorMessage(ref sErrorMessage, "Hor & Ver \"StepStatus\" Format Not Match");

                if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal].m_nSettingPH1 != m_cDrawTypeInfo_Array[nDrawTypeIndex_Vertical].m_nSettingPH1)
                    SetErrorMessage(ref sErrorMessage, "Hor & Ver \"SettingPH1\" Format Not Match");

                if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal].m_nReadPH1 != m_cDrawTypeInfo_Array[nDrawTypeIndex_Vertical].m_nReadPH1)
                    SetErrorMessage(ref sErrorMessage, "Hor & Ver \"ReadPH1\" Format Not Match");

                if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal].m_nSettingPH2 != m_cDrawTypeInfo_Array[nDrawTypeIndex_Vertical].m_nSettingPH2)
                    SetErrorMessage(ref sErrorMessage, "Hor & Ver \"SettingPH2\" Format Not Match");

                if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal].m_nReadPH2 != m_cDrawTypeInfo_Array[nDrawTypeIndex_Vertical].m_nReadPH2)
                    SetErrorMessage(ref sErrorMessage, "Hor & Ver \"ReadPH2\" Format Not Match");

                if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal].m_dFrequency != m_cDrawTypeInfo_Array[nDrawTypeIndex_Vertical].m_dFrequency)
                    SetErrorMessage(ref sErrorMessage, "Hor & Ver \"Frequency\" Format Not Match");

                if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal].m_nRankIndex != m_cDrawTypeInfo_Array[nDrawTypeIndex_Vertical].m_nRankIndex)
                    SetErrorMessage(ref sErrorMessage, "Hor & Ver \"RankIndex\" Format Not Match");

                if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal].m_nRXTraceNumber != m_cDrawTypeInfo_Array[nDrawTypeIndex_Vertical].m_nRXTraceNumber)
                    SetErrorMessage(ref sErrorMessage, "Hor & Ver \"RX TraceNumber\" Format Not Match");

                if (m_cDrawTypeInfo_Array[nDrawTypeIndex_Horizontal].m_nTXTraceNumber != m_cDrawTypeInfo_Array[nDrawTypeIndex_Vertical].m_nTXTraceNumber)
                    SetErrorMessage(ref sErrorMessage, "Hor & Ver \"TX TraceNumber\" Format Not Match");
            }

            if (sErrorMessage != "")
                return false;

            cDataValue.m_nDefaultScaleDigiGain_P0 = ElanConvert.ConvertDigiGainToScale(cDataValue.m_nDefaultDigiGain_P0);
            cDataValue.m_nDefaultScaleDigiGain_Beacon_Rx = ElanConvert.ConvertDigiGainToScale(cDataValue.m_nDefaultDigiGain_Beacon_Rx);
            cDataValue.m_nDefaultScaleDigiGain_Beacon_Tx = ElanConvert.ConvertDigiGainToScale(cDataValue.m_nDefaultDigiGain_Beacon_Tx);
            cDataValue.m_nDefaultScaleDigiGain_PTHF_Rx = ElanConvert.ConvertDigiGainToScale(cDataValue.m_nDefaultDigiGain_PTHF_Rx);
            cDataValue.m_nDefaultScaleDigiGain_PTHF_Tx = ElanConvert.ConvertDigiGainToScale(cDataValue.m_nDefaultDigiGain_PTHF_Tx);
            cDataValue.m_nDefaultScaleDigiGain_BHF_Rx = ElanConvert.ConvertDigiGainToScale(cDataValue.m_nDefaultDigiGain_BHF_Rx);
            cDataValue.m_nDefaultScaleDigiGain_BHF_Tx = ElanConvert.ConvertDigiGainToScale(cDataValue.m_nDefaultDigiGain_BHF_Tx);

            return true;
        }

        private void GetRaw5TData(Raw5TData cRaw5TData, List<byte> byteData_List, DigiGainDataFormat.DataByteLocation eDataByteLocation, int nTraceNumber)
        {
            int nStartIndex = 1;
            int nEndIndex = 5;
            int nShiftCount = 0;

            int nTraceIndex = byteData_List[(int)eDataByteLocation - 2];
            cRaw5TData.m_nTraceIndex = nTraceIndex;

            if (nTraceIndex < 2)
            {
                nEndIndex = 3 + nTraceIndex;
                nShiftCount = 2 - nTraceIndex;
            }
            else if (nTraceIndex >= nTraceNumber - 2)
            {
                nStartIndex = 3 - ((nTraceNumber - 1) - nTraceIndex);
                nShiftCount = (nTraceNumber - 3) - nTraceIndex;
            }
            else
            {
                nShiftCount = 0;
            }

            for (int nTraceDataIndex = nStartIndex; nTraceDataIndex <= nEndIndex; nTraceDataIndex++)
            {
                int nValue = byteData_List[((int)eDataByteLocation - 1) + 2 * (nTraceDataIndex - 1)] * 256 + byteData_List[((int)eDataByteLocation - 1) + 2 * (nTraceDataIndex - 1) + 1];
                int nRealIndex = nTraceDataIndex + nShiftCount;

                switch (nRealIndex)
                {
                    case 1:
                        cRaw5TData.m_nRawData_1T = nValue;
                        break;
                    case 2:
                        cRaw5TData.m_nRawData_2T = nValue;
                        break;
                    case 3:
                        cRaw5TData.m_nRawData_3T = nValue;
                        break;
                    case 4:
                        cRaw5TData.m_nRawData_4T = nValue;
                        break;
                    case 5:
                        cRaw5TData.m_nRawData_5T = nValue;
                        break;
                }
            }
        }

        private void GetRaw5TData_NewFormat(Raw5TData cRaw5TData_RX, Raw5TData cRaw5TData_TX, List<byte> byteData_List)
        {
            int nStartIndex = 1;
            int nEndIndex = 5;

            int nRXTraceByteLocation = 16;
            int nTXTraceByteLocation = 29;
            int nRX5TByteLocation = 4;
            int nTX5TByteLocation = 17;

            int nRXTraceIndex = byteData_List[nRXTraceByteLocation - 1];
            int nTXTraceIndex = byteData_List[nTXTraceByteLocation - 1];
            cRaw5TData_RX.m_nTraceIndex = nRXTraceIndex;
            cRaw5TData_TX.m_nTraceIndex = nTXTraceIndex;

            for (int nTraceDataIndex = nStartIndex; nTraceDataIndex <= nEndIndex; nTraceDataIndex++)
            {
                int nValue = byteData_List[(nRX5TByteLocation - 1) + 2 * (nTraceDataIndex - 1)] + byteData_List[(nRX5TByteLocation - 1) + 2 * (nTraceDataIndex - 1) + 1] * 256;

                switch (nTraceDataIndex)
                {
                    case 1:
                        cRaw5TData_RX.m_nRawData_1T = nValue;
                        break;
                    case 2:
                        cRaw5TData_RX.m_nRawData_2T = nValue;
                        break;
                    case 3:
                        cRaw5TData_RX.m_nRawData_3T = nValue;
                        break;
                    case 4:
                        cRaw5TData_RX.m_nRawData_4T = nValue;
                        break;
                    case 5:
                        cRaw5TData_RX.m_nRawData_5T = nValue;
                        break;
                }
            }

            for (int nTraceDataIndex = nStartIndex; nTraceDataIndex <= nEndIndex; nTraceDataIndex++)
            {
                int nValue = byteData_List[(nTX5TByteLocation - 1) + 2 * (nTraceDataIndex - 1)] + byteData_List[(nTX5TByteLocation - 1) + 2 * (nTraceDataIndex - 1) + 1] * 256;

                switch (nTraceDataIndex)
                {
                    case 1:
                        cRaw5TData_TX.m_nRawData_1T = nValue;
                        break;
                    case 2:
                        cRaw5TData_TX.m_nRawData_2T = nValue;
                        break;
                    case 3:
                        cRaw5TData_TX.m_nRawData_3T = nValue;
                        break;
                    case 4:
                        cRaw5TData_TX.m_nRawData_4T = nValue;
                        break;
                    case 5:
                        cRaw5TData_TX.m_nRawData_5T = nValue;
                        break;
                }
            }
        }

        private bool ComputeDataMean(ref string sMessage, int nFileIndex, int nTraceType)
        {
            DigiGainDataFormat.DataType[] eDataType_Array = null;
            List<Raw5TData> cRaw5TData_Beacon_List = new List<Raw5TData>();
            List<Raw5TData> cRaw5TData_PTHF_List = new List<Raw5TData>();
            List<Raw5TData> cRaw5TData_BHF_List = new List<Raw5TData>();
            int nValidReportNumber = 0;

            if (nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
            {
                eDataType_Array = new DigiGainDataFormat.DataType[] 
                { 
                    DigiGainDataFormat.DataType.Beacon_Rx,
                    DigiGainDataFormat.DataType.PTHF_Rx,
                    DigiGainDataFormat.DataType.BHF_Rx 
                };

                cRaw5TData_Beacon_List = m_cDataValue_List[nFileIndex].m_cRXRaw5TData_Beacon_List;
                cRaw5TData_PTHF_List = m_cDataValue_List[nFileIndex].m_cRXRaw5TData_PTHF_List;
                cRaw5TData_BHF_List = m_cDataValue_List[nFileIndex].m_cRXRaw5TData_BHF_List;
                nValidReportNumber = m_nRXValidReportNumber;
            }
            else if (nTraceType == MainConstantParameter.m_nTRACETYPE_TX)
            {
                eDataType_Array = new DigiGainDataFormat.DataType[] 
                { 
                    DigiGainDataFormat.DataType.Beacon_Tx,
                    DigiGainDataFormat.DataType.PTHF_Tx,
                    DigiGainDataFormat.DataType.BHF_Tx 
                };

                cRaw5TData_Beacon_List = m_cDataValue_List[nFileIndex].m_cTXRaw5TData_Beacon_List;
                cRaw5TData_PTHF_List = m_cDataValue_List[nFileIndex].m_cTXRaw5TData_PTHF_List;
                cRaw5TData_BHF_List = m_cDataValue_List[nFileIndex].m_cTXRaw5TData_BHF_List;
                nValidReportNumber = m_nTXValidReportNumber;
            }

            foreach (DigiGainDataFormat.DataType eDataType in eDataType_Array)
            {
                double dMean = 0.0;
                List<Raw5TData> cRaw5TData_List = new List<Raw5TData>();

                switch(eDataType)
                {
                    case DigiGainDataFormat.DataType.Beacon_Rx:
                    case DigiGainDataFormat.DataType.Beacon_Tx:
                        cRaw5TData_List = cRaw5TData_Beacon_List;
                        break;
                    case DigiGainDataFormat.DataType.PTHF_Rx:
                    case DigiGainDataFormat.DataType.PTHF_Tx:
                        cRaw5TData_List = cRaw5TData_PTHF_List;
                        break;
                    case DigiGainDataFormat.DataType.BHF_Rx:
                    case DigiGainDataFormat.DataType.BHF_Tx:
                        cRaw5TData_List = cRaw5TData_BHF_List;
                        break;
                    default:
                        break;

                }

                int nValidReportCount = Compute3TMean(ref dMean, cRaw5TData_List, nValidReportNumber);

                if (nValidReportCount > -1)
                {
                    sMessage = string.Format("Get {0} Valid Data Too Few({1}<LB:{2})", eDataType.ToString(), nValidReportCount, nValidReportNumber);
                    return false;
                }

                switch (eDataType)
                {
                    case DigiGainDataFormat.DataType.Beacon_Rx:
                        m_cDataValue_List[nFileIndex].m_cRXDigiGainValue.m_dBeacon3TMean = dMean;
                        break;
                    case DigiGainDataFormat.DataType.PTHF_Rx:
                        m_cDataValue_List[nFileIndex].m_cRXDigiGainValue.m_dPTHF3TMean = dMean;
                        break;
                    case DigiGainDataFormat.DataType.BHF_Rx:
                        m_cDataValue_List[nFileIndex].m_cRXDigiGainValue.m_dBHF3TMean = dMean;
                        break;
                    case DigiGainDataFormat.DataType.Beacon_Tx:
                        m_cDataValue_List[nFileIndex].m_cTXDigiGainValue.m_dBeacon3TMean = dMean;
                        break;
                    case DigiGainDataFormat.DataType.PTHF_Tx:
                        m_cDataValue_List[nFileIndex].m_cTXDigiGainValue.m_dPTHF3TMean = dMean;
                        break;
                    case DigiGainDataFormat.DataType.BHF_Tx:
                        m_cDataValue_List[nFileIndex].m_cTXDigiGainValue.m_dBHF3TMean = dMean;
                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        private int Compute3TMean(ref double dTotalMean, List<Raw5TData> cRaw5TData_List, int nValidReportNumber)
        {
            List<double> dDataMean_List = new List<double>();
            int nValidReportCount = 0;

            foreach (Raw5TData cRaw5TData in cRaw5TData_List)
            {
                if (cRaw5TData.m_nRawData_1T == 0 &&
                    cRaw5TData.m_nRawData_2T == 0 &&
                    cRaw5TData.m_nRawData_3T == 0 &&
                    cRaw5TData.m_nRawData_4T == 0 &&
                    cRaw5TData.m_nRawData_5T == 0)
                    continue;

                //double dMean = (cRaw5TData.m_nRawData_2T + cRaw5TData.m_nRawData_3T + cRaw5TData.m_nRawData_4T) / 3;
                double dMean = cRaw5TData.m_nRawData_3T;
                dDataMean_List.Add(dMean);
                nValidReportCount++;
            }

            if (dDataMean_List.Count < nValidReportNumber)
                return dDataMean_List.Count;

            dTotalMean = dDataMean_List.Average();

            return -1;
        }

        private bool CheckDataNumberIsEnough(int nFileIndex, string sFilePath)
        {
            List<Raw5TData>[] cRXRawDataList_Array = new List<Raw5TData>[] 
            { 
                m_cDataValue_List[nFileIndex].m_cRXRaw5TData_Beacon_List,
                m_cDataValue_List[nFileIndex].m_cRXRaw5TData_PTHF_List,
                m_cDataValue_List[nFileIndex].m_cRXRaw5TData_BHF_List 
            };

            DigiGainDataFormat.DataType[] eRXDataType_Array = new DigiGainDataFormat.DataType[] 
            { 
                DigiGainDataFormat.DataType.Beacon_Rx,
                DigiGainDataFormat.DataType.PTHF_Rx,
                DigiGainDataFormat.DataType.BHF_Rx 
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
                m_cDataValue_List[nFileIndex].m_cTXRaw5TData_Beacon_List,
                m_cDataValue_List[nFileIndex].m_cTXRaw5TData_PTHF_List,
                m_cDataValue_List[nFileIndex].m_cTXRaw5TData_BHF_List 
            };

            DigiGainDataFormat.DataType[] eTXDataType_Array = new DigiGainDataFormat.DataType[]
            { 
                DigiGainDataFormat.DataType.Beacon_Tx,
                DigiGainDataFormat.DataType.PTHF_Tx,
                DigiGainDataFormat.DataType.BHF_Tx 
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

        private bool ComputeDigiGainScale()
        {
            bool bErrorFlag = false;

            DigiGainDataFormat.DataType[] eDataType_Array = new DigiGainDataFormat.DataType[] 
            { 
                DigiGainDataFormat.DataType.Beacon_Rx,
                DigiGainDataFormat.DataType.PTHF_Rx,
                DigiGainDataFormat.DataType.BHF_Rx,
                DigiGainDataFormat.DataType.Beacon_Tx,
                DigiGainDataFormat.DataType.PTHF_Tx,
                DigiGainDataFormat.DataType.BHF_Tx 
            };

            for (int nDataIndex = 0; nDataIndex < m_cDataValue_List.Count; nDataIndex++)
            {
                foreach (DigiGainDataFormat.DataType eDataType in eDataType_Array)
                {
                    double dUnitPower = 0.0;

                    switch(eDataType)
                    {
                        case DigiGainDataFormat.DataType.Beacon_Rx:
                            dUnitPower = m_cDataValue_List[nDataIndex].m_cRXDigiGainValue.m_dBeacon3TMean / m_cDataValue_List[nDataIndex].m_nDefaultScaleDigiGain_Beacon_Rx;
                            break;
                        case DigiGainDataFormat.DataType.Beacon_Tx:
                            dUnitPower = m_cDataValue_List[nDataIndex].m_cTXDigiGainValue.m_dBeacon3TMean / m_cDataValue_List[nDataIndex].m_nDefaultScaleDigiGain_Beacon_Tx;
                            break;
                        case DigiGainDataFormat.DataType.PTHF_Rx:
                            dUnitPower = m_cDataValue_List[nDataIndex].m_cRXDigiGainValue.m_dPTHF3TMean / m_cDataValue_List[nDataIndex].m_nDefaultScaleDigiGain_PTHF_Rx;
                            break;
                        case DigiGainDataFormat.DataType.PTHF_Tx:
                            dUnitPower = m_cDataValue_List[nDataIndex].m_cTXDigiGainValue.m_dPTHF3TMean / m_cDataValue_List[nDataIndex].m_nDefaultScaleDigiGain_PTHF_Tx;
                            break;
                        case DigiGainDataFormat.DataType.BHF_Rx:
                            dUnitPower = m_cDataValue_List[nDataIndex].m_cRXDigiGainValue.m_dBHF3TMean / m_cDataValue_List[nDataIndex].m_nDefaultScaleDigiGain_BHF_Rx;
                            break;
                        case DigiGainDataFormat.DataType.BHF_Tx:
                            dUnitPower = m_cDataValue_List[nDataIndex].m_cTXDigiGainValue.m_dBHF3TMean / m_cDataValue_List[nDataIndex].m_nDefaultScaleDigiGain_BHF_Tx;
                            break;
                        default:
                            break;
                    }

                    int nCompensateScale = ComputeCompensateScale(dUnitPower);

                    switch (eDataType)
                    {
                        case DigiGainDataFormat.DataType.Beacon_Rx:
                            m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_P0 = nCompensateScale;
                            m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_Beacon_Rx = nCompensateScale;
                            break;
                        case DigiGainDataFormat.DataType.Beacon_Tx:
                            m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_Beacon_Tx = nCompensateScale;
                            break;
                        case DigiGainDataFormat.DataType.PTHF_Rx:
                            m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_PTHF_Rx = nCompensateScale;
                            break;
                        case DigiGainDataFormat.DataType.PTHF_Tx:
                            m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_PTHF_Tx = nCompensateScale;
                            break;
                        case DigiGainDataFormat.DataType.BHF_Rx:
                            m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_BHF_Rx = nCompensateScale;
                            break;
                        case DigiGainDataFormat.DataType.BHF_Tx:
                            m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_BHF_Tx = nCompensateScale;
                            break;
                        default:
                            break;
                    }

                    if (nCompensateScale > m_nDigiGainScaleHB)
                    {
                        m_cDataInfo_List[nDataIndex].m_nErrorFlag = m_nErrorFlag |= 0x2000;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} DigiGain : {1} Over High Boundary({2})", eDataType.ToString(), nCompensateScale, m_nDigiGainScaleHB);
                        m_cErrorInfo.m_sRecordErrorMessage = string.Format("DigiGain Over High Boundary({0}) Or Under Low Boundary({1})", m_nDigiGainScaleHB, m_nDigiGainScaleLB);

                        m_cDataInfo_List[nDataIndex].m_sErrorMessage = RunProcessErrorFlow(false);
                        bErrorFlag = true;
                    }
                    else if (nCompensateScale < m_nDigiGainScaleLB)
                    {
                        m_cDataInfo_List[nDataIndex].m_nErrorFlag = m_nErrorFlag |= 0x4000;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} DigiGain : {1} Under Low Boundary({2})", eDataType.ToString(), nCompensateScale, m_nDigiGainScaleLB);
                        m_cErrorInfo.m_sRecordErrorMessage = string.Format("DigiGain Over High Boundary({0}) Or Under Low Boundary({1})", m_nDigiGainScaleHB, m_nDigiGainScaleLB);

                        m_cDataInfo_List[nDataIndex].m_sErrorMessage = RunProcessErrorFlow(false);
                        bErrorFlag = true;
                    }
                }
            }

            return bErrorFlag;
        }

        private int ComputeCompensateScale(double dPeakValue)
        {
            int nCompensateScale = (int)(m_nCompensatePower / dPeakValue);

            if (nCompensateScale > m_nDIGIGAINSCALE_HIGHLIMIT)
                nCompensateScale = m_nDIGIGAINSCALE_HIGHLIMIT;
            else if (nCompensateScale < m_nDIGIGAINSCALE_LOWLIMIT)
                nCompensateScale = m_nDIGIGAINSCALE_LOWLIMIT;

            return nCompensateScale;
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

        private void WriteRaw5TData(string sDataTitle, int nFileIndex, string sFilePath, bool bRXTraceTypeFlag)
        {
            string sTraceType = (bRXTraceTypeFlag == true) ? "RX" : "TX";

            List<Raw5TData> cRaw5TData_Beacon_List = new List<Raw5TData>();
            List<Raw5TData> cRaw5TData_PTHF_List = new List<Raw5TData>();
            List<Raw5TData> cRaw5TData_BHF_List = new List<Raw5TData>();

            if (bRXTraceTypeFlag == true)
            {
                cRaw5TData_Beacon_List = m_cDataValue_List[nFileIndex].m_cRXRaw5TData_Beacon_List;
                cRaw5TData_PTHF_List = m_cDataValue_List[nFileIndex].m_cRXRaw5TData_PTHF_List;
                cRaw5TData_BHF_List = m_cDataValue_List[nFileIndex].m_cRXRaw5TData_BHF_List;
            }
            else
            {
                cRaw5TData_Beacon_List = m_cDataValue_List[nFileIndex].m_cTXRaw5TData_Beacon_List;
                cRaw5TData_PTHF_List = m_cDataValue_List[nFileIndex].m_cTXRaw5TData_PTHF_List;
                cRaw5TData_BHF_List = m_cDataValue_List[nFileIndex].m_cTXRaw5TData_BHF_List;
            }

            string[] sColumnName_Array = new string[] 
            { 
                "Beacon Index",
                string.Format("{0}1", sTraceType),
                string.Format("{0}2", sTraceType),
                string.Format("{0}3", sTraceType),
                string.Format("{0}4", sTraceType),
                string.Format("{0}5", sTraceType), 
                "",
                "PTHF Index",
                string.Format("{0}1", sTraceType),
                string.Format("{0}2", sTraceType),
                string.Format("{0}3", sTraceType),
                string.Format("{0}4", sTraceType),
                string.Format("{0}5", sTraceType), 
                "",
                "BHF Index",
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

                int nDataLength = GetMaxDataLength(cRaw5TData_Beacon_List, cRaw5TData_PTHF_List, cRaw5TData_BHF_List);

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    string sText = "";
                    string sRaw5TValue = "";

                    if (cRaw5TData_Beacon_List.Count - 1 >= nDataIndex)
                        sRaw5TValue = SetRaw5TValue(cRaw5TData_Beacon_List[nDataIndex]);
                    else
                        sRaw5TValue = ",,,,,";

                    sText += string.Format("{0},,", sRaw5TValue);

                    if (cRaw5TData_PTHF_List.Count - 1 >= nDataIndex)
                        sRaw5TValue = SetRaw5TValue(cRaw5TData_PTHF_List[nDataIndex]);
                    else
                        sRaw5TValue = ",,,,,";

                    sText += string.Format("{0},,", sRaw5TValue);

                    if (cRaw5TData_BHF_List.Count - 1 >= nDataIndex)
                        sRaw5TValue = SetRaw5TValue(cRaw5TData_BHF_List[nDataIndex]);
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

        private int GetMaxDataLength(List<Raw5TData> cRaw5TData_Beacon_List, List<Raw5TData> cRaw5TData_PTHF_List, List<Raw5TData> cRaw5TData_BHF_List)
        {
            int nDataLenth = (cRaw5TData_Beacon_List.Count > cRaw5TData_PTHF_List.Count) ? cRaw5TData_Beacon_List.Count : cRaw5TData_PTHF_List.Count;
            nDataLenth = (cRaw5TData_BHF_List.Count > nDataLenth) ? cRaw5TData_BHF_List.Count : nDataLenth;

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

        private void WriteComputeDataFile(int nFileIndex, string sFilePath)
        {
            string[] sColumnName_Array = new string[] 
            { 
                "Trace Type", 
                "Beacon Mean", 
                "PTHF Mean", 
                "BHF Mean" 
            };

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("DigiGain 3T Mean");

                for (int nColumnIndex = 0; nColumnIndex < sColumnName_Array.Length; nColumnIndex++)
                {
                    if (nColumnIndex < sColumnName_Array.Length - 1)
                        sw.Write(string.Format("{0},", sColumnName_Array[nColumnIndex]));
                    else
                        sw.WriteLine(sColumnName_Array[nColumnIndex]);
                }

                sw.WriteLine(string.Format("{0},{1},{2},{3}", "Rx", 
                                           m_cDataValue_List[nFileIndex].m_cRXDigiGainValue.m_dBeacon3TMean,
                                           m_cDataValue_List[nFileIndex].m_cRXDigiGainValue.m_dPTHF3TMean,
                                           m_cDataValue_List[nFileIndex].m_cRXDigiGainValue.m_dBHF3TMean));
                sw.WriteLine(string.Format("{0},{1},{2},{3}", "Tx",
                                           m_cDataValue_List[nFileIndex].m_cTXDigiGainValue.m_dBeacon3TMean,
                                           m_cDataValue_List[nFileIndex].m_cTXDigiGainValue.m_dPTHF3TMean,
                                           m_cDataValue_List[nFileIndex].m_cTXDigiGainValue.m_dBHF3TMean));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
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
            string[] sValueTypeName_Array = new string[15] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sFlowStep, 
                SpecificText.m_sSettingPH1, 
                SpecificText.m_sSettingPH2, 
                SpecificText.m_sReadPH1, 
                SpecificText.m_sReadPH2, 
                SpecificText.m_sFrequency,
                SpecificText.m_sRxBeaconMean, 
                SpecificText.m_sTxBeaconMean, 
                SpecificText.m_sRxPTHFMean, 
                SpecificText.m_sTxPTHFMean,
                SpecificText.m_sRxBHFMean, 
                SpecificText.m_sTxBHFMean, 
                SpecificText.m_sErrorMessage 
            };

            string[] sFWParameterTypeName_Array = new string[13] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                SpecificText.m_sFrequency,
                SpecificText.m_scActivePen_DigiGain_P0, 
                SpecificText.m_scActivePen_DigiGain_Beacon_Rx, 
                SpecificText.m_scActivePen_DigiGain_Beacon_Tx,
                SpecificText.m_scActivePen_DigiGain_PTHF_Rx, 
                SpecificText.m_scActivePen_DigiGain_PTHF_Tx,
                SpecificText.m_scActivePen_DigiGain_BHF_Rx, 
                SpecificText.m_scActivePen_DigiGain_BHF_Tx, 
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
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXDigiGainValue.m_dBeacon3TMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXDigiGainValue.m_dBeacon3TMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXDigiGainValue.m_dPTHF3TMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXDigiGainValue.m_dPTHF3TMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXDigiGainValue.m_dBHF3TMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXDigiGainValue.m_dBHF3TMean.ToString()));

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
                            sw.Write(string.Format("{0}{1},", m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_P0.ToString(), m_sDigiGainRatio));
                            sw.Write(string.Format("{0}{1},", m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_Beacon_Rx.ToString(), m_sDigiGainRatio));
                            sw.Write(string.Format("{0}{1},", m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_Beacon_Tx.ToString(), m_sDigiGainRatio));
                            sw.Write(string.Format("{0}{1},", m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_PTHF_Rx.ToString(), m_sDigiGainRatio));
                            sw.Write(string.Format("{0}{1},", m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_PTHF_Tx.ToString(), m_sDigiGainRatio));
                            sw.Write(string.Format("{0}{1},", m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_BHF_Rx.ToString(), m_sDigiGainRatio));
                            sw.Write(string.Format("{0}{1},", m_cDataValue_List[nDataIndex].m_nParameterScaleDigiGain_BHF_Tx.ToString(), m_sDigiGainRatio));

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
    }
}
