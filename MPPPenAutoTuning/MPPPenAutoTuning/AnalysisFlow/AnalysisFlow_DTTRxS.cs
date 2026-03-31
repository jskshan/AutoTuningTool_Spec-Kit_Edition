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
    public class AnalysisFlow_DTTRxS : AnalysisFlow
    {
        private bool m_bDisplayChartDetailValueFlag = false;

        private int m_nTRxSRXValidReportNumber = 150;
        private int m_nTRxSTXValidReportNumber = 70;

        private double m_dNHoverTHRatio_RX = 0.45;
        private double m_dNHoverTHRatio_TX = 0.45;
        private double m_dNContactTHRatio_RX = 0.45;
        private double m_dNContactTHRatio_TX = 0.45;
        private double m_dP0THCompRatio_800us = 0.8;

        private bool m_bDT7318TRxSSpecificReportTypeFlag = false;

        private List<byte> m_byteReport_List = new List<byte>();
        private List<List<byte>> m_byteData_List = new List<List<byte>>();
        private List<List<int>> m_nRXOriginalData_List = new List<List<int>>();
        private List<List<int>> m_nTXOriginalData_List = new List<List<int>>();
        private List<int> m_nRXOriginalIndex_List = new List<int>();
        private List<int> m_nTXOriginalIndex_List = new List<int>();
        private List<List<int>> m_nRXData_List = new List<List<int>>();
        private List<List<int>> m_nTXData_List = new List<List<int>>();

        private List<int> m_nRXTraceIndex_List = new List<int>();
        private List<int> m_nTXTraceIndex_List = new List<int>();
        private List<int> m_nRXFirstMax_List = new List<int>();
        private List<int> m_nRXSecondMax_List = new List<int>();
        private List<int> m_nRXThirdMax_List = new List<int>();
        private List<int> m_nTXFirstMax_List = new List<int>();
        private List<int> m_nTXSecondMax_List = new List<int>();
        private List<int> m_nTXThirdMax_List = new List<int>();

        private List<int> m_nRXOriginalTraceIndex_List = new List<int>();
        private List<int> m_nTXOriginalTraceIndex_List = new List<int>();
        private List<int> m_nRXOriginalFirstMax_List = new List<int>();
        private List<int> m_nRXOriginalSecondMax_List = new List<int>();
        private List<int> m_nRXOriginalThirdMax_List = new List<int>();
        private List<int> m_nTXOriginalFirstMax_List = new List<int>();
        private List<int> m_nTXOriginalSecondMax_List = new List<int>();
        private List<int> m_nTXOriginalThirdMax_List = new List<int>();

        public class DataValue
        {
            public int m_ncActivePen_FM_P0_TH = -1;
            public int m_nHover_TH_Rx = -1;
            public int m_nHover_TH_Tx = -1;
            public int m_nContact_TH_Rx = -1;
            public int m_nContact_TH_Tx = -1;
            public int m_nTRxS_Hover_TH_Rx = -1;
            public int m_nTRxS_Hover_TH_Tx = -1;
            public int m_nTRxS_Contact_TH_Rx = -1;
            public int m_nTRxS_Contact_TH_Tx = -1;

            public int m_nEdge_1Trc_SubPwr = -1;
            public int m_nEdge_2Trc_SubPwr = -1;
            public int m_nEdge_3Trc_SubPwr = -1;
            public int m_nEdge_4Trc_SubPwr = -1;

            public ReferenceValue m_cRXReferenceValue = new ReferenceValue();
            public ReferenceValue m_cTXReferenceValue = new ReferenceValue();

            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public int m_nRXThreshold = -1;
            public int m_nTXThreshold = -1;

            public int m_nNoiseP0_Detect_Time_Index = -1;

            public double m_dRXSecondMaxMean = 0.0;
            public double m_dTXSecondMaxMean = 0.0;
            public double m_dRXThirdMaxMean = 0.0;
            public double m_dTXThirdMaxMean = 0.0;
            public int m_nRXSecondAndThirdMean = 0;
            public int m_nTXSecondAndThirdMean = 0;

            public int m_nNoiseDigiGain_Beacon_Rx = -1;
            public int m_nNoiseDigiGain_Beacon_Tx = -1;

            public int[] m_nNoiseRXMaxData_Array = null;
            public bool m_bGetNoiseRXMaxFlag = false;
            public int[] m_nNoiseTXMaxData_Array = null;
            public bool m_bGetNoiseTXMaxFlag = false;
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

            m_nRXTraceIndex_List.Clear();
            m_nTXTraceIndex_List.Clear();
            m_nRXFirstMax_List.Clear();
            m_nRXSecondMax_List.Clear();
            m_nRXThirdMax_List.Clear();
            m_nTXFirstMax_List.Clear();
            m_nTXSecondMax_List.Clear();
            m_nTXThirdMax_List.Clear();

            m_nRXOriginalTraceIndex_List.Clear();
            m_nTXOriginalTraceIndex_List.Clear();
            m_nRXOriginalFirstMax_List.Clear();
            m_nRXOriginalSecondMax_List.Clear();
            m_nRXOriginalThirdMax_List.Clear();
            m_nTXOriginalFirstMax_List.Clear();
            m_nTXOriginalSecondMax_List.Clear();
            m_nTXOriginalThirdMax_List.Clear();
        }

        public AnalysisFlow_DTTRxS(FlowStep cFlowStep, frmMain cfrmMain)
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

            m_nTRxSRXValidReportNumber = ParamAutoTuning.m_nTRxSRXValidReportNumber;
            m_nTRxSTXValidReportNumber = ParamAutoTuning.m_nTRxSTXValidReportNumber;

            m_dNHoverTHRatio_RX = ParamAutoTuning.m_dDTNormalHoverTHRatio_RX;
            m_dNHoverTHRatio_TX = ParamAutoTuning.m_dDTNormalHoverTHRatio_TX;
            m_dNContactTHRatio_RX = ParamAutoTuning.m_dDTNormalContactTHRatio_RX;
            m_dNContactTHRatio_TX = ParamAutoTuning.m_dDTNormalContactTHRatio_TX;
            m_dP0THCompRatio_800us = ParamAutoTuning.m_dDTP0THCompRatio_800us;

            m_bDisplayChartDetailValueFlag = (ParamAutoTuning.m_nDTDisplayChartDetailValue == 1) ? true : false;

            m_bDT7318TRxSSpecificReportTypeFlag = (ParamAutoTuning.m_nDT7318TRxSSpecificReportType == 1) ? true : false;
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

        public void GetData(List<DTTRxSParameter> cParameter_List)   
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
                string sPictureFolderPath = string.Format(@"{0}\{1}\Picture", m_sResultFolderPath, sFileName);
                Directory.CreateDirectory(m_sIntegrationFolderPath);
                Directory.CreateDirectory(sComputeFolderPath);
                Directory.CreateDirectory(sProcessFolderPath);
                Directory.CreateDirectory(sPictureFolderPath);

                //File
                m_sIntegrationFilePath = string.Format(@"{0}\Integration Data.csv", m_sIntegrationFolderPath);
                string sRXRawDataFilePath = string.Format(@"{0}\RX Raw Data.csv", sProcessFolderPath);
                string sTXRawDataFilePath = string.Format(@"{0}\TX Raw Data.csv", sProcessFolderPath);
                string sRXComputeFilePath = string.Format(@"{0}\RX Compute Data.csv", sComputeFolderPath);
                string sTXComputeFilePath = string.Format(@"{0}\TX Compute Data.csv", sComputeFolderPath);
                string sRXStraightFilePath = string.Format(@"{0}\RX Straight Data.csv", sComputeFolderPath);
                string sTXStraightFilePath = string.Format(@"{0}\TX Straight Data.csv", sComputeFolderPath);

                int nLineCounter = 0;
                string sLine = "";

                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                int nRankIndex = -1;

                GetFileInfoFromReportData(cDataInfo, sFilePath);

                if (m_bDT7318TRxSSpecificReportTypeFlag == true)
                    m_nNormalReportDataLength = 11;
                else
                    m_nNormalReportDataLength = ParamAutoTuning.m_nNormalReportDataLength;

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

                        DTTRxSParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2);

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

                            cDataValue.m_nRXTraceNumber = cParameter.m_nRXTraceNumber;
                            cDataValue.m_nTXTraceNumber = cParameter.m_nTXTraceNumber;
                            cDataValue.m_nNoiseP0_Detect_Time_Index = cParameter.m_nNoiseP0_Detect_Time_Idx;

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

                if (GetPreviousStepValue(cParameter_List, cDataInfo, cDataValue) == false)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0008;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get Previous Step Value Error In {0} File!", Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = "Get Previous Step Value Error";
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

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

                if (m_bDT7318TRxSSpecificReportTypeFlag == true)
                {
                    nTraceTypeByte = 10;
                    nTraceIndexByte = 11;
                }

                int nLeftBoundary = ParamAutoTuning.m_nDTValidReportEdgeNumber - 1;
                int nRightBoundary = nSectionNumber - ParamAutoTuning.m_nDTValidReportEdgeNumber;

                m_nRXTraceNumber = m_cDataValue_List[nFileIndex].m_nRXTraceNumber;
                m_nTXTraceNumber = m_cDataValue_List[nFileIndex].m_nTXTraceNumber;

                for (int nIndex = 0; nIndex < nDataLength; nIndex++)
                {
                    int nTraceTypeIntData = m_byteData_List[nIndex][nTraceTypeByte - 1];
                    int nIndexData = m_byteData_List[nIndex][nTraceIndexByte - 1];

                    //Rx Data
                    if ((nTraceTypeIntData & 0x0F) == 0x08)
                    {
                        if (nIndexData >= m_nRXTraceNumber)
                            continue;

                        /*
                        if (nIndexData >= m_nRXTraceNumber)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0040;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("RX Trace Index Error In Line {0} in {1} File!", nIndex + 1, Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("RX Trace Index Error In Line {0}", nIndex + 1);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                            break;
                        }
                        */

                        int[] nData_Array = new int[nSectionNumber];

                        for (int nSectionIndex = 0; nSectionIndex < nSectionNumber; nSectionIndex++)
                            nData_Array[nSectionIndex] = (m_byteData_List[nIndex][2 * nSectionIndex + m_nNormalReportDataLength] * 256 + m_byteData_List[nIndex][2 * nSectionIndex + m_nNormalReportDataLength + 1]);

                        m_nRXOriginalData_List.Add(new List<int>(nData_Array));
                        m_nRXOriginalIndex_List.Add(nIndexData);
                    }
                    //Tx Data
                    else if ((nTraceTypeIntData & 0x0F) == 0x00)
                    {
                        if (nIndexData >= m_nTXTraceNumber)
                            continue;

                        /*
                        if (nIndexData >= m_nTXTraceNumber)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0080;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("TX Trace Index Error In Line {0} in {1} File!", nIndex + 1, Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("TX Trace Index Error In Line {0}", nIndex + 1);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                            break;
                        }
                        */

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

                GetStraightUsefulData(m_nRXData_List, m_nRXOriginalIndex_List, nLeftBoundary, nRightBoundary, true);
                GetStraightUsefulData(m_nTXData_List, m_nTXOriginalIndex_List, nLeftBoundary, nRightBoundary, false);

                if (m_nRXFirstMax_List.Count < m_nTRxSRXValidReportNumber || m_nRXFirstMax_List.Count == 0)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0100;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get RX Valid Data Too Few({0}<LB:{1}) In {2} File!", Convert.ToString(m_nRXFirstMax_List.Count), Convert.ToString(m_nTRxSRXValidReportNumber), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get RX Valid Data Too Few({0}<LB:{1})", Convert.ToString(m_nRXFirstMax_List.Count), Convert.ToString(m_nTRxSRXValidReportNumber));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                if (m_nTXFirstMax_List.Count < m_nTRxSTXValidReportNumber || m_nTXFirstMax_List.Count == 0)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0200;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get TX Valid Data Too Few({0}<LB:{1}) In {2} File!", Convert.ToString(m_nTXFirstMax_List.Count), Convert.ToString(m_nTRxSTXValidReportNumber), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get TX Valid Data Too Few({0}<LB:{1})", Convert.ToString(m_nTXFirstMax_List.Count));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                int[] nRXTraceIndexData_Array = ConvertListDataToArray(m_nRXTraceIndex_List);
                int[] nTXTraceIndexData_Array = ConvertListDataToArray(m_nTXTraceIndex_List);
                int[] nRXFirstMaxData_Array = ConvertListDataToArray(m_nRXFirstMax_List);
                int[] nTXFirstMaxData_Array = ConvertListDataToArray(m_nTXFirstMax_List);
                int[] nRXSecondMaxData_Array = ConvertListDataToArray(m_nRXSecondMax_List);
                int[] nTXSecondMaxData_Array = ConvertListDataToArray(m_nTXSecondMax_List);
                int[] nRXThirdMaxData_Array = ConvertListDataToArray(m_nRXThirdMax_List);
                int[] nTXThirdMaxData_Array = ConvertListDataToArray(m_nTXThirdMax_List);

                WriteStraightDataFile("RX Straight Data", nRXTraceIndexData_Array, nRXFirstMaxData_Array, nRXSecondMaxData_Array, nRXThirdMaxData_Array, sRXStraightFilePath);
                WriteStraightDataFile("TX Straight Data", nTXTraceIndexData_Array, nTXFirstMaxData_Array, nTXSecondMaxData_Array, nTXThirdMaxData_Array, sTXStraightFilePath);

                cDataValue.m_cRXReferenceValue = ComputeMaxValue(nRXFirstMaxData_Array);
                cDataValue.m_cTXReferenceValue = ComputeMaxValue(nTXFirstMaxData_Array);

                ComputeThreshold(m_nRXSecondMax_List, m_nRXThirdMax_List, nFileIndex, true);
                ComputeThreshold(m_nTXSecondMax_List, m_nTXThirdMax_List, nFileIndex, false);

                if (File.Exists(m_sReferenceFilePath) == true)
                {
                    if (GetNoiseMaxData(nFileIndex) == false)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x4000;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get DigiGain Data Error In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = "Get DigiGain Data Error";
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    }

                    if (m_bReadReportDataErrorFlag == true)
                        continue;

                    ComputeCompensateNoiseMax(nFileIndex);
                }

                SaveLineChartFile(nFileIndex, sPictureFolderPath, true);
                SaveLineChartFile(nFileIndex, sPictureFolderPath, false);

                AddDataFileNameAndRankIndex(nFileIndex, nRankIndex);

                if (m_eSubStep == SubTuningStep.CONTACTTRxS)
                {
                    GetCurrentStepValue(nFileIndex);

                    int nValidFlag = CheckFWParameterValueValid(nFileIndex);

                    if (ParamAutoTuning.m_nDTSkipCompareThreshold != 1)
                    {
                        if (nValidFlag != -1)
                        {
                            sMessage = "";

                            if (nValidFlag == 1)
                            {
                                sMessage = "Hover_TH_Rx > Contact_TH_Rx Error";
                                m_nErrorFlag |= 0x0400;
                            }
                            else if (nValidFlag == 2)
                            {
                                sMessage = "Hover_TH_Tx > Contact_TH_Tx Error";
                                m_nErrorFlag |= 0x0800;
                            }
                            else if (nValidFlag == 3)
                            {
                                sMessage = "TRxS_Hover_TH_Rx > TRxS_Contact_TH_Rx Error";
                                m_nErrorFlag |= 0x1000;
                            }
                            else if (nValidFlag == 4)
                            {
                                sMessage = "TRxS_Hover_TH_Tx > TRxS_Contact_TH_Tx Error";
                                m_nErrorFlag |= 0x2000;
                            }

                            cDataInfo.m_nErrorFlag = m_nErrorFlag;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", sMessage, Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = sMessage;
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();

                            if (m_bReadReportDataErrorFlag == true)
                                continue;
                        }
                    }
                }

                if (cDataInfo.m_sRecordErrorCode != "")
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x4000;

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

            #region Final Check
            if (ParamAutoTuning.m_nFlowMethodType != 1 && m_eSubStep == SubTuningStep.CONTACTTRxS)
            {
                int nErrorCount = 0;
                int nCheckFWParamErrorCount = 0;

                for (int nIndex = 0; nIndex < m_cDataInfo_List.Count; nIndex++)
                {
                    if (m_cDataInfo_List[nIndex].m_nErrorFlag != 0)
                        nErrorCount++;
                    if ((m_cDataInfo_List[nIndex].m_nErrorFlag & 0x0400) != 0 ||
                        (m_cDataInfo_List[nIndex].m_nErrorFlag & 0x0800) != 0 ||
                        (m_cDataInfo_List[nIndex].m_nErrorFlag & 0x1000) != 0 ||
                        (m_cDataInfo_List[nIndex].m_nErrorFlag & 0x2000) != 0)
                        nCheckFWParamErrorCount++;
                }

                if (nCheckFWParamErrorCount > 0 && nCheckFWParamErrorCount == nErrorCount)
                {
                    m_sErrorMessage = "FW Parameter Setting Check Error";
                    OutputMessage("FW Parameter Setting Check Error!!");
                    m_bErrorFlag = true;
                    return;
                }
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
            #endregion
        }

        private bool GetNoiseMaxData(int nFileIndex)
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

            DataTable datatableRXData = GetCSVFileData(m_eMainStep, m_eSubStep, "RX Noise Max Data", m_sReferenceFilePath, "RXNoiseMaxData", ",", true);
            DataTable datatableTXData = GetCSVFileData(m_eMainStep, m_eSubStep, "TX Noise Max Data", m_sReferenceFilePath, "RXNoiseMaxData", ",", false);

            if (datatableRXData == null || datatableTXData == null)
                return false;

            for (int nRowIndex = 0; nRowIndex < datatableRXData.Rows.Count; nRowIndex++)
            {
                int nPH1Value = Convert.ToInt32(datatableRXData.Rows[nRowIndex][SpecificText.m_sPH1]);
                int nPH2Value = Convert.ToInt32(datatableRXData.Rows[nRowIndex][SpecificText.m_sPH2]);

                if (m_cDataInfo_List[nFileIndex].m_nReadPH1 == nPH1Value && m_cDataInfo_List[nFileIndex].m_nReadPH2 == nPH2Value)
                {
                    m_cDataValue_List[nFileIndex].m_nNoiseRXMaxData_Array = new int[nRXTraceNumber];

                    for (int nTraceIndex = 1; nTraceIndex <= nRXTraceNumber; nTraceIndex++)
                    {
                        string sTraceIndex = Convert.ToString(nTraceIndex);
                        m_cDataValue_List[nFileIndex].m_nNoiseRXMaxData_Array[nTraceIndex - 1] = Convert.ToInt32(datatableRXData.Rows[nRowIndex][sTraceIndex]);
                    }

                    m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Rx = Convert.ToInt32(datatableRXData.Rows[nRowIndex][SpecificText.m_scActivePen_DigiGain_Beacon_Rx]);
                    m_cDataValue_List[nFileIndex].m_bGetNoiseRXMaxFlag = true;
                }
            }

            for (int nRowIndex = 0; nRowIndex < datatableTXData.Rows.Count; nRowIndex++)
            {
                int nPH1Value = Convert.ToInt32(datatableTXData.Rows[nRowIndex][SpecificText.m_sPH1]);
                int nPH2Value = Convert.ToInt32(datatableTXData.Rows[nRowIndex][SpecificText.m_sPH2]);

                if (m_cDataInfo_List[nFileIndex].m_nReadPH1 == nPH1Value && m_cDataInfo_List[nFileIndex].m_nReadPH2 == nPH2Value)
                {
                    m_cDataValue_List[nFileIndex].m_nNoiseTXMaxData_Array = new int[nTXTraceNumber];

                    for (int nTraceIndex = 1; nTraceIndex <= nTXTraceNumber; nTraceIndex++)
                    {
                        string sTraceIndex = Convert.ToString(nTraceIndex);
                        m_cDataValue_List[nFileIndex].m_nNoiseTXMaxData_Array[nTraceIndex - 1] = Convert.ToInt32(datatableTXData.Rows[nRowIndex][sTraceIndex]);
                    }

                    m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Tx = Convert.ToInt32(datatableTXData.Rows[nRowIndex][SpecificText.m_scActivePen_DigiGain_Beacon_Tx]);
                    m_cDataValue_List[nFileIndex].m_bGetNoiseTXMaxFlag = true;
                }
            }

            return true;
        }

        private void ComputeCompensateNoiseMax(int nFileIndex)
        {
            if (m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Rx != -1 && m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Rx != -1 &&
                m_cDataValue_List[nFileIndex].m_bGetNoiseRXMaxFlag == true)
            {
                double dDigiGainRatio = (double)m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Rx / (double)m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Rx;
                dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;

                for (int nTraceIndex = 0; nTraceIndex < m_cDataValue_List[nFileIndex].m_nNoiseRXMaxData_Array.Length; nTraceIndex++)
                {
                    int nCompensateValue = (int)Math.Ceiling(m_cDataValue_List[nFileIndex].m_nNoiseRXMaxData_Array[nTraceIndex] * dDigiGainRatio);
                    m_cDataValue_List[nFileIndex].m_nNoiseRXMaxData_Array[nTraceIndex] = nCompensateValue;
                }
            }

            if (m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Tx != -1 && m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Tx != -1 &&
                m_cDataValue_List[nFileIndex].m_bGetNoiseTXMaxFlag == true)
            {
                double dDigiGainRatio = (double)m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Tx / (double)m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Tx;
                dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;

                for (int nTraceIndex = 0; nTraceIndex < m_cDataValue_List[nFileIndex].m_nNoiseTXMaxData_Array.Length; nTraceIndex++)
                {
                    int nCompensateValue = (int)Math.Ceiling(m_cDataValue_List[nFileIndex].m_nNoiseTXMaxData_Array[nTraceIndex] * dDigiGainRatio);
                    m_cDataValue_List[nFileIndex].m_nNoiseTXMaxData_Array[nTraceIndex] = nCompensateValue;
                }
            }
        }

        private void ComputeThreshold(List<int> nSecondMax_List, List<int> nThirdMax_List, int nFileIndex, bool bRXTraceTypeFlag = true)
        {
            double dSecondMaxMean = 0.0;
            double dThirdMaxMean = 0.0;
            int nSecondAndThirdMean = 0;

            for (int nIndex = 0; nIndex < nSecondMax_List.Count; nIndex++)
                dSecondMaxMean += nSecondMax_List[nIndex];

            dSecondMaxMean = Math.Round(dSecondMaxMean / nSecondMax_List.Count, 2, MidpointRounding.AwayFromZero);

            for (int nIndex = 0; nIndex < nThirdMax_List.Count; nIndex++)
                dThirdMaxMean += nThirdMax_List[nIndex];

            dThirdMaxMean = Math.Round(dThirdMaxMean / nThirdMax_List.Count, 2, MidpointRounding.AwayFromZero);

            nSecondAndThirdMean = (int)((dSecondMaxMean + dThirdMaxMean) / 2);

            if (bRXTraceTypeFlag == true)
            {
                int nThreshold = nSecondAndThirdMean;

                if (ParamAutoTuning.m_dDTThresholdRatio_RX > 0.0)
                    nThreshold = (int)(nSecondAndThirdMean * ParamAutoTuning.m_dDTThresholdRatio_RX);

                m_cDataValue_List[nFileIndex].m_dRXSecondMaxMean = dSecondMaxMean;
                m_cDataValue_List[nFileIndex].m_dRXThirdMaxMean = dThirdMaxMean;
                m_cDataValue_List[nFileIndex].m_nRXSecondAndThirdMean = nSecondAndThirdMean;
                m_cDataValue_List[nFileIndex].m_nRXThreshold = nThreshold;
            }
            else
            {
                int nThreshold = nSecondAndThirdMean;

                if (ParamAutoTuning.m_dDTThresholdRatio_TX > 0.0)
                    nThreshold = (int)(nSecondAndThirdMean * ParamAutoTuning.m_dDTThresholdRatio_TX);

                m_cDataValue_List[nFileIndex].m_dTXSecondMaxMean = dSecondMaxMean;
                m_cDataValue_List[nFileIndex].m_dTXThirdMaxMean = dThirdMaxMean;
                m_cDataValue_List[nFileIndex].m_nTXSecondAndThirdMean = nSecondAndThirdMean;
                m_cDataValue_List[nFileIndex].m_nTXThreshold = nThreshold;
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
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX, sLine, 0x000080, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX, sLine, 0x000100, m_nINFOTYPE_INT);

                    if (m_eSubStep == SubTuningStep.HOVERTRxS)
                        GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_HOVERRAISEHEIGHT, sLine, 0x000200, m_nINFOTYPE_INT);

                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_7318TRXSSPECIFICREPORTTYPE, sLine, 0x000400, m_nINFOTYPE_INT);

                    if (m_eSubStep == SubTuningStep.HOVERTRxS)
                    {
                        if (lGetInfoFlag == 0x0007FF)
                            break;
                    }
                    else
                    {
                        if (lGetInfoFlag == 0x0005FF)
                            break;
                    }
                }
            }
            finally
            {
                srFile.Close();
            }

            if (cDataInfo.m_n7318TRxSSpecificReportType == 1)
                m_bDT7318TRxSSpecificReportTypeFlag = true;
        }

        protected override bool CheckInfoIsCorrect(ref string sErrorMessage, DataInfo cDataInfo)
        {
            CheckGeneralInfoIsCorrect(ref sErrorMessage, cDataInfo);

            if (cDataInfo.m_nRankIndex < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RANKINDEX));

            if (sErrorMessage != "")
                return false;

            return true;
        }

        private List<List<int>> ConvertProcessData(List<List<int>> nInputData_List, List<int> nIndex_List, int nTraceNumber)
        {
            int[] nData_Array = new int[nTraceNumber]; 
            List<List<int>> nResultData_List = new List<List<int>>();

            for (int nDataIndex = 0; nDataIndex < nInputData_List.Count; nDataIndex++)
            {
                Array.Clear(nData_Array, 0, nData_Array.Length);

                int nStartIndex = 0;
                int nEndIndex = 23;

                if (nTraceNumber <= 24)
                {
                    nStartIndex = 0;
                    nEndIndex = nTraceNumber - 1;
                }
                else if (nIndex_List[nDataIndex] <= 11)
                {
                    nStartIndex = 0;
                    nEndIndex = 23;
                }
                else if (nIndex_List[nDataIndex] > 11 && nIndex_List[nDataIndex] < nTraceNumber - 12)
                {
                    nStartIndex = nIndex_List[nDataIndex] - 11;
                    nEndIndex = nIndex_List[nDataIndex] + 12;
                }
                else if (nIndex_List[nDataIndex] >= nTraceNumber - 12)
                {
                    nStartIndex = nTraceNumber - 24;
                    nEndIndex = nTraceNumber - 1;
                }

                int nCount = 0;

                for (int nValueIndex = nStartIndex; nValueIndex <= nEndIndex; nValueIndex++)
                {
                    nData_Array[nValueIndex] = nInputData_List[nDataIndex][nCount];
                    nCount++;
                }

                List<int> nData_List = new List<int>(nData_Array);
                nResultData_List.Add(nData_List);
            }

            return nResultData_List;
        }

        private ReferenceValue ComputeMaxValue(int[] nData_Array)
        {
            ReferenceValue cReferenceValue = new ReferenceValue();
            int nMaxValue = -1;

            for (int nDataIndex = 0; nDataIndex < nData_Array.Length; nDataIndex++)
            {
                if (nDataIndex == 0)
                    nMaxValue = nData_Array[nDataIndex];
                else
                {
                    if (nData_Array[nDataIndex] > nMaxValue)
                        nMaxValue = nData_Array[nDataIndex];
                }
            }

            cReferenceValue.m_nTotalMax = nMaxValue;

            return cReferenceValue;
        }

        private void GetStraightUsefulData(List<List<int>> nData_List, List<int> nIndex_List, int nLeftBoundary, int nRightBoundary, bool bRXTraceTypeFlag = true)
        {
            int nSectionNumber = 24;

            for (int nDataIndex = 0; nDataIndex < nData_List.Count; nDataIndex++)
            {
                int nIndexData = nIndex_List[nDataIndex];

                if (bRXTraceTypeFlag == true)
                {
                    if (nIndex_List[nDataIndex] > (int)((nSectionNumber / 2) - 1) && nIndex_List[nDataIndex] < m_nRXTraceNumber - (int)(nSectionNumber / 2))
                        nIndexData = (int)((nSectionNumber / 2) - 1);
                    else if (nIndex_List[nDataIndex] >= m_nRXTraceNumber - (int)(nSectionNumber / 2))
                    {
                        int nValue = m_nRXTraceNumber - nIndexData;
                        nIndexData = nSectionNumber - nValue;
                    }
                }
                else
                {
                    if (nIndex_List[nDataIndex] > (int)((nSectionNumber / 2) - 1) && nIndex_List[nDataIndex] < m_nTXTraceNumber - (int)(nSectionNumber / 2))
                        nIndexData = (int)((nSectionNumber / 2) - 1);
                    else if (nIndex_List[nDataIndex] >= m_nTXTraceNumber - (int)(nSectionNumber / 2))
                    {
                        int nValue = m_nTXTraceNumber - nIndexData;
                        nIndexData = nSectionNumber - nValue;
                    }
                }

                int nFirstMaxValue = FindMaxIndexValue(nData_List[nDataIndex], 1);
                int nSecondMaxValue = FindMaxIndexValue(nData_List[nDataIndex], 2);
                int nThirdMaxValue = FindMaxIndexValue(nData_List[nDataIndex], 3);

                if (bRXTraceTypeFlag == true)
                {
                    m_nRXOriginalTraceIndex_List.Add(nIndex_List[nDataIndex] + 1);
                    m_nRXOriginalFirstMax_List.Add(nFirstMaxValue);
                    m_nRXOriginalSecondMax_List.Add(nSecondMaxValue);
                    m_nRXOriginalThirdMax_List.Add(nThirdMaxValue);
                }
                else
                {
                    m_nTXOriginalTraceIndex_List.Add(nIndex_List[nDataIndex] + 1);
                    m_nTXOriginalFirstMax_List.Add(nFirstMaxValue);
                    m_nTXOriginalSecondMax_List.Add(nSecondMaxValue);
                    m_nTXOriginalThirdMax_List.Add(nThirdMaxValue);
                }

                if (nIndexData <= nLeftBoundary || nIndexData >= nRightBoundary)
                    continue;

                if (bRXTraceTypeFlag == true)
                {
                    m_nRXTraceIndex_List.Add(nIndex_List[nDataIndex] + 1);
                    m_nRXFirstMax_List.Add(nFirstMaxValue);
                    m_nRXSecondMax_List.Add(nSecondMaxValue);
                    m_nRXThirdMax_List.Add(nThirdMaxValue);
                }
                else
                {
                    m_nTXTraceIndex_List.Add(nIndex_List[nDataIndex] + 1);
                    m_nTXFirstMax_List.Add(nFirstMaxValue);
                    m_nTXSecondMax_List.Add(nSecondMaxValue);
                    m_nTXThirdMax_List.Add(nThirdMaxValue);
                }
            }
        }

        private int[] ConvertListDataToArray(List<int> nData_List)
        {
            int[] nResultData_Array = new int[nData_List.Count];

            for (int nDataIndex = 0; nDataIndex < nData_List.Count; nDataIndex++)
                nResultData_Array[nDataIndex] = nData_List[nDataIndex];

            return nResultData_Array;
        }

        private void WriteStraightDataFile(string sTitleName, int[] nTraceIndex_Array, int[] nFirstMax_Array, int[] nSecondMax_Array, int[] nThirdMax_Array, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                sw.WriteLine("TraceIndex,FirstMax,SecondMax,ThirdMax");

                int nDataLength = nFirstMax_Array.Length;

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    string sText = "";

                    sText += string.Format("{0},", nTraceIndex_Array[nDataIndex]);
                    sText += string.Format("{0},", nFirstMax_Array[nDataIndex]);
                    sText += string.Format("{0},", nSecondMax_Array[nDataIndex]);
                    sText += string.Format("{0}", nThirdMax_Array[nDataIndex]);
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
            string[] sValueTypeName_Array = new string[21] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sFlowStep, 
                SpecificText.m_sSettingPH1, 
                SpecificText.m_sSettingPH2, 
                SpecificText.m_sReadPH1, 
                SpecificText.m_sReadPH2, 
                SpecificText.m_sFrequency, 
                SpecificText.m_sRXTotalMax, 
                SpecificText.m_sTXTotalMax, 
                SpecificText.m_sRXSecondMaxMean, 
                SpecificText.m_sRXThirdMaxMean, 
                SpecificText.m_sRXTotalMaxMean, 
                SpecificText.m_sRXThresholdRatio, 
                SpecificText.m_sRXThreshold,
                SpecificText.m_sTXSecondMaxMean, 
                SpecificText.m_sTXThirdMaxMean, 
                SpecificText.m_sTXTotalMaxMean, 
                SpecificText.m_sTXThresholdRatio, 
                SpecificText.m_sTXThreshold, 
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
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXReferenceValue.m_nTotalMax.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXReferenceValue.m_nTotalMax.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_dRXSecondMaxMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_dRXThirdMaxMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nRXSecondAndThirdMean.ToString()));
                            sw.Write(string.Format("{0},", ParamAutoTuning.m_dDTThresholdRatio_RX.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nRXThreshold.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_dTXSecondMaxMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_dTXThirdMaxMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTXSecondAndThirdMean.ToString()));
                            sw.Write(string.Format("{0},", ParamAutoTuning.m_dDTThresholdRatio_TX.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTXThreshold.ToString()));

                            string sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sErrorMessage);

                            if (ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode != "")
                                sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sRecordErrorMessage);

                            sw.WriteLine(sErrorMessage);

                            break;
                        }
                    }
                }

                if (m_eSubStep == SubTuningStep.CONTACTTRxS)
                {
                    string[] sFWParameterTypeName_Array = new string[20] 
                    { 
                        SpecificText.m_sRanking, 
                        SpecificText.m_sFileName, 
                        SpecificText.m_sPH1, 
                        SpecificText.m_sPH2, 
                        SpecificText.m_sFrequency, 
                        SpecificText.m_sRXTraceNumber, 
                        SpecificText.m_sTXTraceNumber, 
                        SpecificText.m_scActivePen_FM_P0_TH,
                        SpecificText.m_scActivePen_Beacon_Contact_TH_Rx, 
                        SpecificText.m_scActivePen_Beacon_Contact_TH_Tx,
                        SpecificText.m_scActivePen_Beacon_Hover_TH_Rx, 
                        SpecificText.m_scActivePen_Beacon_Hover_TH_Tx,
                        SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Rx, 
                        SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Tx,
                        SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Rx, 
                        SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Tx,
                        SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr, 
                        SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr,
                        SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr, 
                        SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr 
                    };

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
                                string sP0_TH = (m_cDataValue_List[nDataIndex].m_ncActivePen_FM_P0_TH < 0) ? "N/A" : m_cDataValue_List[nDataIndex].m_ncActivePen_FM_P0_TH.ToString();

                                string sEdge_1Trc_SubPwr = m_cDataValue_List[nDataIndex].m_nEdge_1Trc_SubPwr.ToString();
                                string sEdge_2Trc_SubPwr = m_cDataValue_List[nDataIndex].m_nEdge_2Trc_SubPwr.ToString();
                                string sEdge_3Trc_SubPwr = m_cDataValue_List[nDataIndex].m_nEdge_3Trc_SubPwr.ToString();
                                string sEdge_4Trc_SubPwr = m_cDataValue_List[nDataIndex].m_nEdge_4Trc_SubPwr.ToString();

                                if (m_cDataValue_List[nDataIndex].m_nEdge_1Trc_SubPwr < 0)
                                    sEdge_1Trc_SubPwr = "N/A";

                                if (m_cDataValue_List[nDataIndex].m_nEdge_2Trc_SubPwr < 0)
                                    sEdge_2Trc_SubPwr = "N/A";

                                if (m_cDataValue_List[nDataIndex].m_nEdge_3Trc_SubPwr < 0)
                                    sEdge_3Trc_SubPwr = "N/A";

                                if (m_cDataValue_List[nDataIndex].m_nEdge_4Trc_SubPwr < 0)
                                    sEdge_4Trc_SubPwr = "N/A";

                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                                sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nRXTraceNumber.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTXTraceNumber.ToString()));
                                sw.Write(string.Format("{0},", sP0_TH));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nContact_TH_Rx.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nContact_TH_Tx.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nHover_TH_Rx.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nHover_TH_Tx.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTRxS_Contact_TH_Rx.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTRxS_Contact_TH_Tx.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTRxS_Hover_TH_Rx.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTRxS_Hover_TH_Tx.ToString()));
                                sw.Write(string.Format("{0},", sEdge_1Trc_SubPwr));
                                sw.Write(string.Format("{0},", sEdge_2Trc_SubPwr));
                                sw.Write(string.Format("{0},", sEdge_3Trc_SubPwr));
                                sw.WriteLine(string.Format("{0}", sEdge_4Trc_SubPwr));

                                break;
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
                                if (m_eSubStep == SubTuningStep.HOVERTRxS)
                                {
                                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                    sw.Write(string.Format("{0},", FlowRobot.TOUCHLINE.ToString()));
                                    sw.Write(string.Format("{0},", FlowRecord.TRxS.ToString()));
                                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                    sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));

                                }
                                else if (m_eSubStep == SubTuningStep.CONTACTTRxS)
                                {
                                    if (eSubStep == SubTuningStep.TILTTUNING_PTHF)
                                    {
                                        if (m_cDataInfo_List[nDataIndex].m_dFrequency > ParamAutoTuning.m_nFrequencyLB_MPP180)
                                        {
                                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                            sw.Write(string.Format("{0},", FlowRobot.TOUCHLINE_HOR.ToString()));
                                            sw.Write(string.Format("{0},", FlowRecord.TILT.ToString()));
                                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                            sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));

                                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                            sw.Write(string.Format("{0},", FlowRobot.TOUCHLINE_VER.ToString()));
                                            sw.Write(string.Format("{0},", FlowRecord.TILT.ToString()));
                                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                            sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));

                                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                            sw.Write(string.Format("{0},", FlowRobot.TOUCHLINE.ToString()));
                                            sw.Write(string.Format("{0},", FlowRecord.TILT.ToString()));
                                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                            sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));
                                        }
                                    }
                                    else if (eSubStep == SubTuningStep.PRESSURESETTING)
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
                                    else if (eSubStep == SubTuningStep.LINEARITYTABLE)
                                    {
                                        if (bSetPTFlowFlag == true)
                                            break;

                                        sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                        sw.Write(string.Format("{0},", FlowRobot.TOUCHLINE_HOR.ToString()));
                                        sw.Write(string.Format("{0},", FlowRecord.LINEARITY.ToString()));
                                        sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                        sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));

                                        sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                        sw.Write(string.Format("{0},", FlowRobot.TOUCHLINE_VER.ToString()));
                                        sw.Write(string.Format("{0},", FlowRecord.LINEARITY.ToString()));
                                        sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                        sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));

                                        bSetPTFlowFlag = true;
                                    }
                                    /*
                                    else if (eSubStep == SubTuningStep.PRESSUREPROTECT)
                                    {
                                        if (bSetPTFlowFlag == true)
                                            break;

                                        sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                        sw.Write(string.Format("{0},", FlowRobot.HOVERPOINT_CEN.ToString()));
                                        sw.Write(string.Format("{0},", FlowRecord.TRxS.ToString()));
                                        sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                        sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));

                                        bSetPTFlowFlag = true;
                                    }
                                    */
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

        private bool GetPreviousStepValue(List<DTTRxSParameter> cParameter_List, DataInfo cDataInfo, DataValue cDataValue)
        {
            bool bErrorFlag = true;

            if (m_eSubStep == SubTuningStep.CONTACTTRxS)
            {
                for (int nParameterIndex = 0; nParameterIndex < cParameter_List.Count; nParameterIndex++)
                {
                    if (cParameter_List[nParameterIndex].m_nPH1 == cDataInfo.m_nReadPH1 &&
                        cParameter_List[nParameterIndex].m_nPH2 == cDataInfo.m_nReadPH2)
                    {
                        int nGetParameterFlag = 0;

                        cDataValue.m_ncActivePen_FM_P0_TH = cParameter_List[nParameterIndex].m_nPreP0_TH;
                        nGetParameterFlag |= 0x0001;

                        if (cParameter_List[nParameterIndex].m_nPreTRxS_Hover_TH_Rx != -1)
                        {
                            cDataValue.m_nTRxS_Hover_TH_Rx = cParameter_List[nParameterIndex].m_nPreTRxS_Hover_TH_Rx;
                            nGetParameterFlag |= 0x0002;
                        }

                        if (cParameter_List[nParameterIndex].m_nPreTRxS_Hover_TH_Tx != -1)
                        {
                            cDataValue.m_nTRxS_Hover_TH_Tx = cParameter_List[nParameterIndex].m_nPreTRxS_Hover_TH_Tx;
                            nGetParameterFlag |= 0x0004;
                        }

                        cDataValue.m_nEdge_1Trc_SubPwr = cParameter_List[nParameterIndex].m_nPreEdge_1Trc_SubPwr;
                        nGetParameterFlag |= 0x0008;

                        cDataValue.m_nEdge_2Trc_SubPwr = cParameter_List[nParameterIndex].m_nPreEdge_2Trc_SubPwr;
                        nGetParameterFlag |= 0x0010;

                        cDataValue.m_nEdge_3Trc_SubPwr = cParameter_List[nParameterIndex].m_nPreEdge_3Trc_SubPwr;
                        nGetParameterFlag |= 0x0020;

                        cDataValue.m_nEdge_4Trc_SubPwr = cParameter_List[nParameterIndex].m_nPreEdge_4Trc_SubPwr;
                        nGetParameterFlag |= 0x0040;

                        if (cParameter_List[nParameterIndex].m_nNoiseP0_Detect_Time_Idx != -1)
                        {
                            cDataValue.m_nNoiseP0_Detect_Time_Index = cParameter_List[nParameterIndex].m_nNoiseP0_Detect_Time_Idx;
                            nGetParameterFlag |= 0x0080;
                        }

                        if (nGetParameterFlag == 0x00FF)
                            bErrorFlag = false;

                        break;
                    }
                }
            }
            else
                bErrorFlag = false;

            return !bErrorFlag;
        }

        private void GetCurrentStepValue(int nFileIndex)
        {
            m_cDataValue_List[nFileIndex].m_nTRxS_Contact_TH_Rx = m_cDataValue_List[nFileIndex].m_nRXThreshold;
            m_cDataValue_List[nFileIndex].m_nTRxS_Contact_TH_Tx = m_cDataValue_List[nFileIndex].m_nTXThreshold;

            m_cDataValue_List[nFileIndex].m_nHover_TH_Rx = (int)(m_cDataValue_List[nFileIndex].m_nTRxS_Hover_TH_Rx * m_dNHoverTHRatio_RX);
            m_cDataValue_List[nFileIndex].m_nHover_TH_Tx = (int)(m_cDataValue_List[nFileIndex].m_nTRxS_Hover_TH_Tx * m_dNHoverTHRatio_TX);
            m_cDataValue_List[nFileIndex].m_nContact_TH_Rx = (int)(m_cDataValue_List[nFileIndex].m_nTRxS_Contact_TH_Rx * m_dNContactTHRatio_RX);
            m_cDataValue_List[nFileIndex].m_nContact_TH_Tx = (int)(m_cDataValue_List[nFileIndex].m_nTRxS_Contact_TH_Tx * m_dNContactTHRatio_TX);

            if (m_cDataValue_List[nFileIndex].m_nNoiseP0_Detect_Time_Index == 1)
            {
                int nCompValue = (int)(m_cDataValue_List[nFileIndex].m_nTRxS_Hover_TH_Rx * m_dP0THCompRatio_800us);

                if (m_cDataValue_List[nFileIndex].m_ncActivePen_FM_P0_TH > nCompValue)
                    m_cDataValue_List[nFileIndex].m_ncActivePen_FM_P0_TH = nCompValue;
            }
            else
            {
                if (m_cDataValue_List[nFileIndex].m_ncActivePen_FM_P0_TH > m_cDataValue_List[nFileIndex].m_nHover_TH_Rx)
                    m_cDataValue_List[nFileIndex].m_ncActivePen_FM_P0_TH = m_cDataValue_List[nFileIndex].m_nHover_TH_Rx;
            }
        }

        private int CheckFWParameterValueValid(int nFileIndex)
        {
            if (m_cDataValue_List[nFileIndex].m_nHover_TH_Rx > m_cDataValue_List[nFileIndex].m_nContact_TH_Rx)
                return 1;
            else if (m_cDataValue_List[nFileIndex].m_nHover_TH_Tx > m_cDataValue_List[nFileIndex].m_nContact_TH_Tx)
                return 2;
            else if (m_cDataValue_List[nFileIndex].m_nTRxS_Hover_TH_Rx > m_cDataValue_List[nFileIndex].m_nTRxS_Contact_TH_Rx)
                return 3;
            else if (m_cDataValue_List[nFileIndex].m_nTRxS_Hover_TH_Tx > m_cDataValue_List[nFileIndex].m_nTRxS_Contact_TH_Tx)
                return 4;

            return -1;
        }

        private int FindMaxIndexValue(List<int> nInputData_List, int nMaxIndex = 1)
        {
            List<int> nData_List = new List<int>();

            for (int nIndex = 0; nIndex < nInputData_List.Count; ++nIndex)
            {
                int nCurrentValue = nInputData_List[nIndex];
                int nInsertValue = 0;

                for (int nLength = Math.Min(nData_List.Count, nMaxIndex); nInsertValue < nLength; ++nInsertValue)
                {
                    if (nCurrentValue > nData_List[nInsertValue])
                        break;
                }

                if (nInsertValue < nMaxIndex)
                    nData_List.Insert(nInsertValue, nCurrentValue);
            }

            return nData_List[nMaxIndex - 1];
        }

       private void SaveLineChartFile(int nFileIndex, string sDirectoryPath, bool bRXTraceTypeFlag = true)
        {
            string sTitleName = "";
            string sFilePath = "";

            List<int> nTraceIndexData_List = null;
            List<int> nFirstMaxData_List = null;
            List<int> nSecondMaxData_List = null;
            List<int> nThirdMaxData_List = null;

            double dSecondMaxMean = 0.0;
            double dThirdMaxMean = 0.0;
            int nTotalMean = 0;
            int nInterval = 0;

            if (bRXTraceTypeFlag == true)
            {
                sTitleName = "RX";
                sFilePath = string.Format(@"{0}\RX_{1}", sDirectoryPath, SpecificText.m_sChartFileName);

                nTraceIndexData_List = new List<int>(m_nRXOriginalTraceIndex_List);
                nFirstMaxData_List = new List<int>(m_nRXOriginalFirstMax_List);
                nSecondMaxData_List = new List<int>(m_nRXOriginalSecondMax_List);
                nThirdMaxData_List = new List<int>(m_nRXOriginalThirdMax_List);

                dSecondMaxMean = m_cDataValue_List[nFileIndex].m_dRXSecondMaxMean;
                dThirdMaxMean = m_cDataValue_List[nFileIndex].m_dRXThirdMaxMean;

                if (m_bDisplayChartDetailValueFlag == false)
                    nTotalMean = m_cDataValue_List[nFileIndex].m_nRXThreshold;
                else
                    nTotalMean = m_cDataValue_List[nFileIndex].m_nRXSecondAndThirdMean;

                nInterval = (int)(nFirstMaxData_List.Count / m_cDataValue_List[nFileIndex].m_nRXTraceNumber);
            }
            else
            {
                sTitleName = "TX";
                sFilePath = string.Format(@"{0}\TX_{1}", sDirectoryPath, SpecificText.m_sChartFileName);

                nTraceIndexData_List = new List<int>(m_nTXOriginalTraceIndex_List);
                nFirstMaxData_List = new List<int>(m_nTXOriginalFirstMax_List);
                nSecondMaxData_List = new List<int>(m_nTXOriginalSecondMax_List);
                nThirdMaxData_List = new List<int>(m_nTXOriginalThirdMax_List);

                dSecondMaxMean = m_cDataValue_List[nFileIndex].m_dTXSecondMaxMean;
                dThirdMaxMean = m_cDataValue_List[nFileIndex].m_dTXThirdMaxMean;

                if (m_bDisplayChartDetailValueFlag == false)
                    nTotalMean = m_cDataValue_List[nFileIndex].m_nTXThreshold;
                else
                    nTotalMean = m_cDataValue_List[nFileIndex].m_nTXSecondAndThirdMean;

                nInterval = (int)(nFirstMaxData_List.Count / m_cDataValue_List[nFileIndex].m_nTXTraceNumber);
            }

            if (m_cDataInfo_List[nFileIndex].m_nHoverRaiseHeight != -1)
                sTitleName = string.Format("{0}(Hover:{1}mm)", sTitleName, m_cDataInfo_List[nFileIndex].m_nHoverRaiseHeight);

            Series serFirstMaxValue = new Series("First Max Value");
            Series serSecondMaxValue = new Series("Second Max Value");
            Series serThirdMaxValue = new Series("Third Max Value");
            Series serSecondMaxMean = new Series(string.Format("Second Max Mean({0})", dSecondMaxMean.ToString()));
            Series serThirdMaxMean = new Series(string.Format("Third Max Mean({0})", dThirdMaxMean.ToString()));
            Series serTotalMean = new Series(string.Format("Total Mean({0})", nTotalMean.ToString()));

            if (m_bDisplayChartDetailValueFlag == false)
            {
                string sThresholdState = "";

                if (m_eSubStep == SubTuningStep.HOVERTRxS)
                    sThresholdState = "Hover";
                else if (m_eSubStep == SubTuningStep.CONTACTTRxS)
                    sThresholdState = "Contact";

                string sTraceType = (bRXTraceTypeFlag == true) ? "Rx" : "Tx";

                serTotalMean = new Series(string.Format("{0}_TH_{1}({2})", sThresholdState, sTraceType, nTotalMean.ToString()));
            }

            string[] sDataName_Array = new string[] 
            { 
                SpecificText.m_sFirstMaxValue, 
                SpecificText.m_sSecondMaxValue, 
                SpecificText.m_sThirdMaxValue, 
                SpecificText.m_sSecondMaxMean,
                SpecificText.m_sThirdMaxMean, 
                SpecificText.m_sTotalMean 
            };

            Series[] serData_Array = new Series[] 
            { 
                serFirstMaxValue, 
                serSecondMaxValue, 
                serThirdMaxValue, 
                serSecondMaxMean, 
                serThirdMaxMean, 
                serTotalMean 
            };

            Color[] colorBackColor_Array = new Color[] 
            { 
                Color.Blue, 
                Color.Brown, 
                Color.LimeGreen, 
                Color.Green,
                Color.Yellow, 
                Color.Red 
            };

            if (m_bDisplayChartDetailValueFlag == false)
            {
                sDataName_Array = new string[] 
                { 
                    SpecificText.m_sFirstMaxValue, 
                    SpecificText.m_sSecondMaxValue, 
                    SpecificText.m_sThirdMaxValue,
                    SpecificText.m_sTotalMean 
                };

                serData_Array = new Series[] 
                { 
                    serFirstMaxValue, 
                    serSecondMaxValue, 
                    serThirdMaxValue, 
                    serTotalMean 
                };

                colorBackColor_Array = new Color[] 
                { 
                    Color.Blue, 
                    Color.Brown, 
                    Color.LimeGreen, 
                    Color.Red 
                };
            }

            //Show Line Chart
            Chart cChart = new Chart();
            var varChartArea = new ChartArea();
            cChart.ChartAreas.Add(varChartArea);
            cChart.Width = 2000;
            cChart.Height = 428;
            cChart.Legends.Add("Legend");
            cChart.Legends["Legend"].Font = new Font("Times New Roman", 10);
            cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
            cChart.Titles.Add(sTitleName);
            cChart.Titles[0].Font = new Font("Times New Roman", 18);
            cChart.ChartAreas[0].AxisY.Title = "Value";
            cChart.ChartAreas[0].AxisY.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.ChartAreas[0].AxisX.Title = "Trace Index";
            cChart.ChartAreas[0].AxisX.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Times New Roman", 10);
            cChart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            cChart.ChartAreas[0].AxisX.IsLabelAutoFit = false;
            cChart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;
            cChart.ChartAreas[0].AxisX.Interval = nInterval;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            if ((bRXTraceTypeFlag == true && m_cDataValue_List[nFileIndex].m_bGetNoiseRXMaxFlag == true) ||
                (bRXTraceTypeFlag == false && m_cDataValue_List[nFileIndex].m_bGetNoiseTXMaxFlag == true))
            {
                int[] nNoiseMaxData_Array;

                if (bRXTraceTypeFlag == true)
                    nNoiseMaxData_Array = m_cDataValue_List[nFileIndex].m_nNoiseRXMaxData_Array;
                else
                    nNoiseMaxData_Array = m_cDataValue_List[nFileIndex].m_nNoiseTXMaxData_Array;

                Series serNoiseMaxValue = new Series("Noise Max");

                serNoiseMaxValue.ChartType = SeriesChartType.Line;
                serNoiseMaxValue.IsValueShownAsLabel = false;
                serNoiseMaxValue.Color = Color.Purple;
                serNoiseMaxValue.BorderWidth = 2;

                for (int nDataIndex = 0; nDataIndex < nFirstMaxData_List.Count; nDataIndex++)
                {
                    int nTraceIndex = nTraceIndexData_List[nDataIndex];

                    if (nTraceIndex <= nNoiseMaxData_Array.Length)
                        serNoiseMaxValue.Points.AddXY(nTraceIndexData_List[nDataIndex].ToString(), nNoiseMaxData_Array[nTraceIndex - 1]);
                }

                cChart.Series.Add(serNoiseMaxValue);
            }

            for (int nSeriesIndex = 0; nSeriesIndex < serData_Array.Length; nSeriesIndex++)
            {
                serData_Array[nSeriesIndex].ChartType = SeriesChartType.Line;
                serData_Array[nSeriesIndex].IsValueShownAsLabel = false;
                serData_Array[nSeriesIndex].Color = colorBackColor_Array[nSeriesIndex];

                if (nSeriesIndex >= 3)
                    serData_Array[nSeriesIndex].BorderWidth = 2;

                for (int nDataIndex = 0; nDataIndex < nFirstMaxData_List.Count; nDataIndex++)
                {
                    switch (sDataName_Array[nSeriesIndex])
                    {
                        case SpecificText.m_sFirstMaxValue:
                            serData_Array[nSeriesIndex].Points.AddXY(nTraceIndexData_List[nDataIndex].ToString(), nFirstMaxData_List[nDataIndex]);
                            break;
                        case SpecificText.m_sSecondMaxValue:
                            serData_Array[nSeriesIndex].Points.AddXY(nTraceIndexData_List[nDataIndex].ToString(), nSecondMaxData_List[nDataIndex]);
                            break;
                        case SpecificText.m_sThirdMaxValue:
                            serData_Array[nSeriesIndex].Points.AddXY(nTraceIndexData_List[nDataIndex].ToString(), nThirdMaxData_List[nDataIndex]);
                            break;
                        case SpecificText.m_sSecondMaxMean:
                            serData_Array[nSeriesIndex].Points.AddXY(nTraceIndexData_List[nDataIndex].ToString(), dSecondMaxMean);
                            break;
                        case SpecificText.m_sThirdMaxMean:
                            serData_Array[nSeriesIndex].Points.AddXY(nTraceIndexData_List[nDataIndex].ToString(), dThirdMaxMean);
                            break;
                        case SpecificText.m_sTotalMean:
                            serData_Array[nSeriesIndex].Points.AddXY(nTraceIndexData_List[nDataIndex].ToString(), nTotalMean);
                            break;
                        default:
                            break;
                    }
                }

                cChart.Series.Add(serData_Array[nSeriesIndex]);
            }

            cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
        }
    }
}
