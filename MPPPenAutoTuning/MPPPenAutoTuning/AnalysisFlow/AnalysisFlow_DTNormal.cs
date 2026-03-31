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
    public class AnalysisFlow_DTNormal : AnalysisFlow
    {
        //private int m_nStraightUsefulDataNumber = 12;

        private int m_nNormalFilterRXValidReportNumber = 150;
        private int m_nNormalFilterTXValidReportNumber = 70;

        private const double m_dHoverTHRatio_RX = 0.38;
        private const double m_dHoverTHRatio_TX = 0.38;
        private const double m_dContactTHRatio_RX = 0.3;
        private const double m_dContactTHRatio_TX = 0.3;

        private List<byte> m_byteReport_List = new List<byte>();
        private List<List<byte>> m_byteData_List = new List<List<byte>>();
        private List<List<int>> m_nRXOriginalData_List = new List<List<int>>();
        private List<List<int>> m_nTXOriginalData_List = new List<List<int>>();
        private List<int> m_nRXOriginalIndex_List = new List<int>();
        private List<int> m_nTXOriginalIndex_List = new List<int>();
        private List<List<int>> m_nRXData_List = new List<List<int>>();
        private List<List<int>> m_nTXData_List = new List<List<int>>();

        public class DataValue
        {
            public int m_nNoiseRXInnerMax = -1;
            public int m_nNoiseTXInnerMax = -1;

            public int m_nNoiseRXInMaxPlus3InMaxSTD = -1;

            public int m_nNoiseTRMaxMinusPreP0_TH_1Trc = -1;
            public int m_nNoiseTRMaxMinusPreP0_TH_2Trc = -1;
            public int m_nNoiseTRMaxMinusPreP0_TH_3Trc = -1;
            public int m_nNoiseTRMaxMinusPreP0_TH_4Trc = -1;

            public int m_nFilterRXValue = -1;
            public int m_nFilterTXValue = -1;

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

            public int[,] m_nRXStatisticHistogram_Array = null;
            public int[,] m_nTXStatisticHistogram_Array = null;

            public bool m_bGetNoiseP0_Detect_TimeFlag = false;
            public bool m_bGetP0_THFlag = false;
            public bool m_bGetEdge_SubPwrFlag = false;

            public int m_nNoiseP0_Detect_Time_Index = -1;
            public int m_nFNoiseP0_Detect_Time_Index = -1;

            public int m_nRXPreliminaryThreshold = -1;
            public int m_nTXPreliminaryThreshold = -1;
            public int m_nRXPreliminaryTRxSThreshold = -1;
            public int m_nTXPreliminaryTRxSThreshold = -1;

            public int m_nRXPreliminaryTH_M1 = -1;
            public int m_nRXPreliminaryTH_M2 = -1;
            public int m_nTXPreliminaryTH_M1 = -1;
            public int m_nTXPreliminaryTH_M2 = -1;

            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public int m_nNoiseDigiGain_P0 = -1;
            public int m_nNoiseDigiGain_Beacon_Rx = -1;
            public int m_nNoiseDigiGain_Beacon_Tx = -1;
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
        }

        public AnalysisFlow_DTNormal(FlowStep cFlowStep, frmMain cfrmMain)
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

            //m_nStraightUsefulDataNumber = ParamAutoTuning.m_nStraightUsefulDataNumber;

            m_nNormalFilterRXValidReportNumber = ParamAutoTuning.m_nNormalFilterRXValidReportNumber;
            m_nNormalFilterTXValidReportNumber = ParamAutoTuning.m_nNormalFilterTXValidReportNumber;
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

        public void GetData(List<DTNormalParameter> cParameter_List)
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
                string sDistributionFolderPath = string.Format(@"{0}\{1}\Distribution Data", m_sResultFolderPath, sFileName);
                string sProcessFolderPath = string.Format(@"{0}\{1}\Process Data", m_sResultFolderPath, sFileName);
                string sPictureFolderPath = string.Format(@"{0}\{1}\Picture", m_sResultFolderPath, sFileName);
                Directory.CreateDirectory(m_sIntegrationFolderPath);
                Directory.CreateDirectory(sComputeFolderPath);
                Directory.CreateDirectory(sDistributionFolderPath);
                Directory.CreateDirectory(sProcessFolderPath);
                Directory.CreateDirectory(sPictureFolderPath);
                
                //File
                string m_sIntegrationFilePath = string.Format(@"{0}\Integration Data.csv", m_sIntegrationFolderPath);
                string sRXRawDataFilePath = string.Format(@"{0}\RX Raw Data.csv", sProcessFolderPath);
                string sTXRawDataFilePath = string.Format(@"{0}\TX Raw Data.csv", sProcessFolderPath);
                string sRXComputeFilePath = string.Format(@"{0}\RX Compute Data.csv", sComputeFolderPath);
                string sTXComputeFilePath = string.Format(@"{0}\TX Compute Data.csv", sComputeFolderPath);
                string sRXStraightFilePath = string.Format(@"{0}\RX Straight Data.csv", sComputeFolderPath);
                string sTXStraightFilePath = string.Format(@"{0}\TX Straight Data.csv", sComputeFolderPath);
                string sRXFilterFilePath = string.Format(@"{0}\RX Filter Data.csv", sComputeFolderPath);
                string sTXFilterFilePath = string.Format(@"{0}\TX Filter Data.csv", sComputeFolderPath);
                string sRXSortFilePath = string.Format(@"{0}\RX Sort Data.csv", sComputeFolderPath);
                string sTXSortFilePath = string.Format(@"{0}\TX Sort Data.csv", sComputeFolderPath);
                string sHistogramFilePath = string.Format(@"{0}\Histogram Data.csv", sDistributionFolderPath);

                int nLineCounter = 0;
                string sLine = "";

                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                int nRankIndex = -1;
                int nFilterRXValue = -1;
                int nFilterTXValue = -1;

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

                bool bCheckFlag = false;

                if ((ParamAutoTuning.m_nContactStepFilterType >= 1 && ParamAutoTuning.m_nContactStepFilterType <= 3) && m_eSubStep == SubTuningStep.CONTACT)
                {
                    DTNormalParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2);

                    if (cParameter != null)
                    {
                        if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_LOADDATA && cParameter.m_nRankIndex != cDataInfo.m_nRankIndex)
                            continue;

                        if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_LOADDATA)
                            nRankIndex = cParameter.m_nRankIndex;
                        else
                            nRankIndex = cDataInfo.m_nRankIndex;

                        cDataInfo.m_sRecordErrorCode = cParameter.m_sErrorCode;
                        cDataInfo.m_sRecordErrorMessage = cParameter.m_sErrorMessage;

                        if (ParamAutoTuning.m_nContactStepFilterType == 1)
                        {
                            cDataValue.m_nFilterRXValue = nFilterRXValue = cParameter.m_nHover_1stRXTotalMedian;
                            cDataValue.m_nFilterTXValue = nFilterTXValue = cParameter.m_nHover_1stTXTotalMedian;
                        }
                        else if (ParamAutoTuning.m_nContactStepFilterType == 2)
                        {
                            cDataValue.m_nFilterRXValue = nFilterRXValue = ParamAutoTuning.m_nContactStepFilterRXValue;
                            cDataValue.m_nFilterTXValue = nFilterTXValue = ParamAutoTuning.m_nContactStepFilterTXValue;
                        }
                        else
                        {
                            cDataValue.m_nFilterRXValue = nFilterRXValue = cParameter.m_nHover_1stRXTotalMax;
                            cDataValue.m_nFilterTXValue = nFilterTXValue = cParameter.m_nHover_1stTXTotalMax;
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
                }
                else
                {
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

                        if (cDataValue.m_nNoiseP0_Detect_Time_Index == 1)
                        {
                            cDataValue.m_nFilterRXValue = nFilterRXValue = (int)(cDataValue.m_nNoiseRXInnerMax * ParamAutoTuning.m_dNormal800to400PwrRatio);
                            cDataValue.m_nFilterTXValue = nFilterTXValue = (int)(cDataValue.m_nNoiseTXInnerMax * ParamAutoTuning.m_dNormal800to400PwrRatio);
                        }
                        else
                        {
                            cDataValue.m_nFilterRXValue = nFilterRXValue = cDataValue.m_nNoiseRXInnerMax;
                            cDataValue.m_nFilterTXValue = nFilterTXValue = cDataValue.m_nNoiseTXInnerMax;
                        }
                    }
                    else
                    {
                        if (CheckAnalogParameterIsIdentical() == true)
                        {
                            DTNormalParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2);

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

                                cDataValue.m_nNoiseP0_Detect_Time_Index = cParameter.m_nNoiseP0_Detect_Time_Idx;

                                if (cDataValue.m_nNoiseP0_Detect_Time_Index >= 0)
                                    cDataValue.m_bGetNoiseP0_Detect_TimeFlag = true;

                                if (cDataValue.m_nNoiseP0_Detect_Time_Index == 1)
                                {
                                    cDataValue.m_nFilterRXValue = nFilterRXValue = (int)(cParameter.m_nNoiseRXInnerMax * ParamAutoTuning.m_dNormal800to400PwrRatio);
                                    cDataValue.m_nFilterTXValue = nFilterTXValue = (int)(cParameter.m_nNoiseTXInnerMax * ParamAutoTuning.m_dNormal800to400PwrRatio);
                                }
                                else
                                {
                                    cDataValue.m_nFilterRXValue = nFilterRXValue = cParameter.m_nNoiseRXInnerMax;
                                    cDataValue.m_nFilterTXValue = nFilterTXValue = cParameter.m_nNoiseTXInnerMax;
                                }

                                cDataValue.m_nNoiseRXInMaxPlus3InMaxSTD = cParameter.m_nNoiseRXInMaxPlus3InMaxSTD;
                                cDataValue.m_bGetP0_THFlag = true;
                                cDataValue.m_nNoiseTRMaxMinusPreP0_TH_1Trc = cParameter.m_nNoiseTrcMaxMinusPreP0_TH_1Trc;
                                cDataValue.m_nNoiseTRMaxMinusPreP0_TH_2Trc = cParameter.m_nNoiseTrcMaxMinusPreP0_TH_2Trc;
                                cDataValue.m_nNoiseTRMaxMinusPreP0_TH_3Trc = cParameter.m_nNoiseTrcMaxMinusPreP0_TH_3Trc;
                                cDataValue.m_nNoiseTRMaxMinusPreP0_TH_4Trc = cParameter.m_nNoiseTrcMaxMinusPreP0_TH_4Trc;
                                cDataValue.m_bGetEdge_SubPwrFlag = true;
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
                }

                if (m_eSubStep == SubTuningStep.CONTACT)
                {
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

                    ComputeP0_THAndEdge_SubPwr(nFileIndex);
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

                            if (byteData_Array[1] != 0x01)
                                continue;
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
                int nTraceNumberByte = 10;
                int nIndexByte = 12;

                bool bGetRXTraceNumberFlag = false;
                bool bGetTXTraceNumberFlag = false;

                int nLeftBoundary = ParamAutoTuning.m_nDTValidReportEdgeNumber - 1;
                int nRightBoundary = nSectionNumber - ParamAutoTuning.m_nDTValidReportEdgeNumber;

                int nRXTypeValue = MainConstantParameter.m_nGETDATATYPE_NONESYNCRX_400US;
                int nTXTypeValue = MainConstantParameter.m_nGETDATATYPE_NONESYNCTX_400US;

                if (cDataInfo.m_nP0_DetectTime_Index == 1 && (m_eSubStep == SubTuningStep.HOVER_1ST || m_eSubStep == SubTuningStep.HOVER_2ND))
                {
                    nRXTypeValue = MainConstantParameter.m_nGETDATATYPE_NONESYNCRX_800US;
                    nTXTypeValue = MainConstantParameter.m_nGETDATATYPE_NONESYNCTX_800US;
                }

                for (int nIndex = 0; nIndex < nDataLength; nIndex++)
                {
                    int nTraceTypeIntData = m_byteData_List[nIndex][nTraceTypeByte - 1];
                    int nIndexData = m_byteData_List[nIndex][nIndexByte - 1];

                    //Rx Data
                    if (((m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                          m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW ||
                          m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT) && 
                         nTraceTypeIntData == nRXTypeValue) ||
                        (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA &&
                         (nTraceTypeIntData == MainConstantParameter.m_nGETDATATYPE_NONESYNCRX_400US ||
                          nTraceTypeIntData == MainConstantParameter.m_nGETDATATYPE_NONESYNCRX_800US)))
                    {
                        int nTraceNumber = m_byteData_List[nIndex][nTraceNumberByte - 1];

                        if (bGetRXTraceNumberFlag == false)
                        {
                            m_nRXTraceNumber = nTraceNumber;
                            bGetRXTraceNumberFlag = true;
                        }
                        else
                        {
                            if (nTraceNumber != m_nRXTraceNumber)
                            {
                                cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0040;

                                m_cErrorInfo.m_sPrintErrorMessage = string.Format("RX Trace Number Not Match In Line {0} in {1} File!", nIndex + 1, Path.GetFileName(sFilePath));
                                m_cErrorInfo.m_sRecordErrorMessage = string.Format("RX Trace Number Not Match In Line {0}", nIndex + 1);
                                m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                                m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                                m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                                cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                                break;
                            }
                        }

                        int[] nData_Array = new int[nSectionNumber];

                        for (int nSectionIndex = 0; nSectionIndex < nSectionNumber; nSectionIndex++)
                            nData_Array[nSectionIndex] = (m_byteData_List[nIndex][2 * nSectionIndex + m_nNormalReportDataLength] * 256 + m_byteData_List[nIndex][2 * nSectionIndex + m_nNormalReportDataLength + 1]);

                        m_nRXOriginalData_List.Add(new List<int>(nData_Array));

                        if (nIndexData > (int)((nSectionNumber / 2) - 1) && nIndexData < m_nRXTraceNumber - (int)(nSectionNumber / 2))
                            nIndexData = (int)((nSectionNumber / 2) - 1);
                        else if (nIndexData >= m_nRXTraceNumber - (int)(nSectionNumber / 2))
                        {
                            int nValue = m_nRXTraceNumber - nIndexData;
                            nIndexData = nSectionNumber - nValue;
                        }

                        m_nRXOriginalIndex_List.Add(nIndexData);
                    }
                    //Tx Data
                    else if (((m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE ||
                               m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW ||
                               m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT) && 
                              nTraceTypeIntData == nTXTypeValue) ||
                             (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA &&
                              (nTraceTypeIntData == MainConstantParameter.m_nGETDATATYPE_NONESYNCTX_400US ||
                               nTraceTypeIntData == MainConstantParameter.m_nGETDATATYPE_NONESYNCTX_800US)))
                    {
                        int nTraceNumber = m_byteData_List[nIndex][nTraceNumberByte - 1];

                        if (bGetTXTraceNumberFlag == false)
                        {
                            m_nTXTraceNumber = nTraceNumber;
                            bGetTXTraceNumberFlag = true;
                        }
                        else
                        {
                            if (nTraceNumber != m_nTXTraceNumber)
                            {
                                cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0080;

                                m_cErrorInfo.m_sPrintErrorMessage = string.Format("TX Trace Number Not Match In Line {0} in {1} File!", nIndex + 1, Path.GetFileName(sFilePath));
                                m_cErrorInfo.m_sRecordErrorMessage = string.Format("TX Trace Number Not Match In Line {0}", nIndex + 1);
                                m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                                m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                                m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                                cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                                break;
                            }
                        }

                        int[] nData_Array = new int[nSectionNumber];

                        for (int nSectionIndex = 0; nSectionIndex < nSectionNumber; nSectionIndex++)
                            nData_Array[nSectionIndex] = (m_byteData_List[nIndex][2 * nSectionIndex + m_nNormalReportDataLength] * 256 + m_byteData_List[nIndex][2 * nSectionIndex + m_nNormalReportDataLength + 1]);

                        m_nTXOriginalData_List.Add(new List<int>(nData_Array));

                        if (nIndexData > (int)((nSectionNumber / 2) - 1) && nIndexData < m_nTXTraceNumber - (int)(nSectionNumber / 2))
                            nIndexData = (int)((nSectionNumber / 2) - 1);
                        else if (nIndexData >= m_nTXTraceNumber - (int)(nSectionNumber / 2))
                        {
                            int nValue = m_nTXTraceNumber - nIndexData;
                            nIndexData = nSectionNumber - nValue;
                        }

                        m_nTXOriginalIndex_List.Add(nIndexData);
                    }
                    else
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0100;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("Trace Type Value Error In Line {0} in {1} File!", nIndex + 1, Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = string.Format("Trace Type Value Error In Line {0}", nIndex + 1);
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                        break;
                    }
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteProcessData(string.Format("RX {0} Data", m_sSubStepCodeName), m_nRXOriginalData_List, sRXRawDataFilePath);
                WriteProcessData(string.Format("TX {0} Data", m_sSubStepCodeName), m_nTXOriginalData_List, sTXRawDataFilePath);

                m_nRXData_List = ConvertProcessData(m_nRXOriginalData_List);
                m_nTXData_List = ConvertProcessData(m_nTXOriginalData_List);

                WriteComputeDataFile("RX Compute Data", m_nRXData_List, sRXComputeFilePath);
                WriteComputeDataFile("TX Compute Data", m_nTXData_List, sTXComputeFilePath);

                List<int> nRXUsefulData_List = GetStraightUsefulData(m_nRXData_List, m_nRXOriginalIndex_List, nLeftBoundary, nRightBoundary);
                List<int> nTXUsefulData_List = GetStraightUsefulData(m_nTXData_List, m_nTXOriginalIndex_List, nLeftBoundary, nRightBoundary);

                int[] nRXUsefulData_Array = ConvertListDataToArray(nRXUsefulData_List);
                int[] nTXUsefulData_Array = ConvertListDataToArray(nTXUsefulData_List);

                WriteStraightDataFile("RX Straight Data", nRXUsefulData_Array, sRXStraightFilePath);
                WriteStraightDataFile("TX Straight Data", nTXUsefulData_Array, sTXStraightFilePath);

                List<int> nRXData_List = new List<int>();
                List<int> nTXData_List = new List<int>();

                ComputeFilterValueByDigiGainRatio(ref nFilterRXValue, ref nFilterTXValue, nFileIndex);

                bool bRXCheckFlag = GetFilterData(ref nRXData_List, nRXUsefulData_Array, nFilterRXValue);

                if (bRXCheckFlag == false)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0800;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Compute RX Valid Data Error In {0} File!", Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = "Compute RX Valid Data Error";
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                bool bTXCheckFlag = GetFilterData(ref nTXData_List, nTXUsefulData_Array, nFilterTXValue);

                if (bTXCheckFlag == false)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x1000;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Compute TX Valid Data Error In {0} File!", Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = "Compute TX Valid Data Error";
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                int[] nRXFilterData_Array = ConvertListDataToArray(nRXData_List);
                int[] nTXFilterData_Array = ConvertListDataToArray(nTXData_List);

                WriteStraightDataFile("RX Filter Data", nRXFilterData_Array, sRXFilterFilePath);
                WriteStraightDataFile("TX Filter Data", nTXFilterData_Array, sTXFilterFilePath);

                if (nRXData_List.Count < m_nNormalFilterRXValidReportNumber || nRXData_List.Count == 0)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x2000;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get RX Valid Data Too Few({0}<LB:{1}) In {2} File!", Convert.ToString(nRXData_List.Count), Convert.ToString(m_nNormalFilterRXValidReportNumber), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get RX Valid Data Too Few({0}<LB:{1})", Convert.ToString(nRXData_List.Count), Convert.ToString(m_nNormalFilterRXValidReportNumber));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                if (nTXData_List.Count < m_nNormalFilterTXValidReportNumber || nTXData_List.Count == 0)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x4000;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get TX Valid Data Too Few({0}<LB:{1}) In {2} File!", Convert.ToString(nTXData_List.Count), Convert.ToString(m_nNormalFilterTXValidReportNumber), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get TX Valid Data Too Few({0}<LB:{1})", Convert.ToString(nTXData_List.Count), Convert.ToString(m_nNormalFilterTXValidReportNumber));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                nRXData_List.Sort();
                nTXData_List.Sort();

                int[] nRXSortData_Array = ConvertListDataToArray(nRXData_List);
                int[] nTXSortData_Array = ConvertListDataToArray(nTXData_List);

                WriteStraightDataFile("RX Sort Data", nRXSortData_Array, sRXSortFilePath);
                WriteStraightDataFile("TX Sort Data", nTXSortData_Array, sTXSortFilePath);

                cDataValue.m_cRXReferenceValue = ComputeTotalValue(nRXSortData_Array);
                cDataValue.m_cTXReferenceValue = ComputeTotalValue(nTXSortData_Array);

                ComputeTraceNumberAndPreviousThreshold(nRXSortData_Array, nFileIndex, true);
                ComputeTraceNumberAndPreviousThreshold(nTXSortData_Array, nFileIndex, false);

                cDataValue.m_nRXStatisticHistogram_Array = ComputeStatisticHistogram(nRXFilterData_Array, cDataValue.m_cRXReferenceValue);
                cDataValue.m_nTXStatisticHistogram_Array = ComputeStatisticHistogram(nTXFilterData_Array, cDataValue.m_cTXReferenceValue);

                cDataValue.m_cRXReferenceValue.m_nMinGroupMean = GetMinimumGroupMean(nRXFilterData_Array, cDataValue.m_nRXStatisticHistogram_Array, cDataValue.m_cRXReferenceValue);
                cDataValue.m_cTXReferenceValue.m_nMinGroupMean = GetMinimumGroupMean(nTXFilterData_Array, cDataValue.m_nTXStatisticHistogram_Array, cDataValue.m_cTXReferenceValue);

                WriteHistogramDataFile(cDataValue.m_cRXReferenceValue, cDataValue.m_cTXReferenceValue, cDataValue.m_nRXStatisticHistogram_Array, cDataValue.m_nTXStatisticHistogram_Array, sHistogramFilePath);

                // Save Histogram Picture
                HistogramInfo HistoInfo = new HistogramInfo(1500, 321);
                SaveHistogramChartPicture(HistoInfo, cDataValue.m_cRXReferenceValue, cDataValue.m_cTXReferenceValue, cDataValue.m_nRXStatisticHistogram_Array, cDataValue.m_nTXStatisticHistogram_Array, sPictureFolderPath, cDataInfo.m_nHoverRaiseHeight);

                AddDataFileNameAndRankIndex(nFileIndex, nRankIndex);

                if (m_eSubStep == SubTuningStep.CONTACT)
                {
                    GetCurrentStepValue(nFileIndex);

                    if (ParamAutoTuning.m_nDTSkipCompareThreshold != 1)
                    {
                        int nValidFlag = CheckFWParameterValueValid(nFileIndex);

                        if (nValidFlag != -1)
                        {
                            sMessage = "";

                            if (nValidFlag == 1)
                            {
                                sMessage = "Hover_TH_Rx > Contact_TH_Rx Error";
                                m_nErrorFlag |= 0x8000;
                            }
                            else if (nValidFlag == 2)
                            {
                                sMessage = "Hover_TH_Tx > Contact_TH_Tx Error";
                                m_nErrorFlag |= 0x10000;
                            }
                            else if (nValidFlag == 3)
                            {
                                sMessage = "TRxS_Hover_TH_Rx > TRxS_Contact_TH_Rx Error";
                                m_nErrorFlag |= 0x20000;
                            }
                            else if (nValidFlag == 4)
                            {
                                sMessage = "TRxS_Hover_TH_Tx > TRxS_Contact_TH_Tx Error";
                                m_nErrorFlag |= 0x40000;
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
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x80000;

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
            if (ParamAutoTuning.m_nFlowMethodType != 1 && m_eSubStep == SubTuningStep.CONTACT)
            {
                int nErrorCount = 0;
                int nCheckFWParamErrorCount = 0;
                for (int nIndex = 0; nIndex < m_cDataInfo_List.Count; nIndex++)
                {
                    if (m_cDataInfo_List[nIndex].m_nErrorFlag != 0)
                        nErrorCount++;
                    if ((m_cDataInfo_List[nIndex].m_nErrorFlag & 0x8000) != 0 ||
                        (m_cDataInfo_List[nIndex].m_nErrorFlag & 0x10000) != 0 ||
                        (m_cDataInfo_List[nIndex].m_nErrorFlag & 0x20000) != 0 ||
                        (m_cDataInfo_List[nIndex].m_nErrorFlag & 0x40000) != 0)
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
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_P0_DETECT_TIME, sLine, 0x000080, m_nINFOTYPE_STRING);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISERXINNERMAX, sLine, 0x000100, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISETXINNERMAX, sLine, 0x000200, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISERXINMAXPLUS3INMAXSTD, sLine, 0x000400, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_1TR, sLine, 0x000800, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_2TR, sLine, 0x001000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_3TR, sLine, 0x002000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_4TR, sLine, 0x004000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISEP0_DETECT_TIME_INDEX, sLine, 0x008000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_P0, sLine, 0x010000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX, sLine, 0x020000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX, sLine, 0x040000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISEDIGIGAIN_P0, sLine, 0x080000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_RX, sLine, 0x100000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_TX, sLine, 0x200000, m_nINFOTYPE_INT);

                    if (m_eSubStep == SubTuningStep.HOVER_1ST || m_eSubStep == SubTuningStep.HOVER_2ND)
                        GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_HOVERRAISEHEIGHT, sLine, 0x400000, m_nINFOTYPE_INT);

                    if (m_eSubStep == SubTuningStep.HOVER_1ST || m_eSubStep == SubTuningStep.HOVER_2ND)
                    {
                        if (lGetInfoFlag == 0x07FFFF)
                            break;
                    }
                    else
                    {
                        if (lGetInfoFlag == 0x03FFFFF)
                            break;
                    }

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

                                if (sParameterName == StringConvert.m_sRECORD_NOISEP0_DETECT_TIME_INDEX)
                                    cDataValue.m_nNoiseP0_Detect_Time_Index = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISERXINNERMAX)
                                    cDataValue.m_nNoiseRXInnerMax = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISETXINNERMAX)
                                    cDataValue.m_nNoiseTXInnerMax = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISERXINMAXPLUS3INMAXSTD)
                                    cDataValue.m_nNoiseRXInMaxPlus3InMaxSTD = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_1TR)
                                    cDataValue.m_nNoiseTRMaxMinusPreP0_TH_1Trc = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_2TR)
                                    cDataValue.m_nNoiseTRMaxMinusPreP0_TH_2Trc = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_3TR)
                                    cDataValue.m_nNoiseTRMaxMinusPreP0_TH_3Trc = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_4TR)
                                    cDataValue.m_nNoiseTRMaxMinusPreP0_TH_4Trc = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISEDIGIGAIN_P0)
                                    cDataValue.m_nNoiseDigiGain_P0 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_RX)
                                    cDataValue.m_nNoiseDigiGain_Beacon_Rx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_TX)
                                    cDataValue.m_nNoiseDigiGain_Beacon_Tx = nValue;

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

            if ((m_cfrmMain.m_nSkipPreviousStepFlag & MainConstantParameter.m_nSKIPFILE_FLOWTXT) != 0)
            {
                if (cDataInfo.m_nRankIndex < 0)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RANKINDEX));

                if (cDataValue.m_nNoiseRXInnerMax < 0)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_NOISERXINNERMAX));

                if (cDataValue.m_nNoiseTXInnerMax < 0)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_NOISERXINNERMAX));
            }

            if (cDataInfo.m_nP0_DetectTime_Index < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_P0_DETECT_TIME));

            if (cDataValue.m_nNoiseP0_Detect_Time_Index >= 0)
                cDataValue.m_bGetNoiseP0_Detect_TimeFlag = true;

            if (cDataValue.m_nNoiseRXInMaxPlus3InMaxSTD > -1)
                cDataValue.m_bGetP0_THFlag = true;

            int nEdge_SubPwrFlag = 0;

            if (cDataValue.m_nNoiseTRMaxMinusPreP0_TH_1Trc > -1)
                nEdge_SubPwrFlag |= 0x0001;

            if (cDataValue.m_nNoiseTRMaxMinusPreP0_TH_2Trc > -1)
                nEdge_SubPwrFlag |= 0x0002;

            if (cDataValue.m_nNoiseTRMaxMinusPreP0_TH_3Trc > -1)
                nEdge_SubPwrFlag |= 0x0004;

            if (cDataValue.m_nNoiseTRMaxMinusPreP0_TH_4Trc > -1)
                nEdge_SubPwrFlag |= 0x0008;

            if (nEdge_SubPwrFlag == 0x000F)
                cDataValue.m_bGetEdge_SubPwrFlag = true;

            if (sErrorMessage != "")
                return false;

            return true;
        }

        private List<List<int>> ConvertProcessData(List<List<int>> nData_List)
        {
            List<List<int>> nResultData_List = new List<List<int>>();

            for (int nDataIndex = 0; nDataIndex < nData_List.Count; nDataIndex++)
                nResultData_List.Add(nData_List[nDataIndex]);

            return nResultData_List;
        }

        private ReferenceValue ComputeTotalValue(int[] nData_Array)
        {
            ReferenceValue cReferenceValue = new ReferenceValue();
            int nDataCount = 0;
            int nMaxValue = -1;
            int nMinValue = -1;
            double dMeanValue = 0;
            double dStdValue = 0;

            for (int nReportIndex = 0; nReportIndex < nData_Array.Length; nReportIndex++)
            {
                if (nReportIndex == 0)
                {
                    nMaxValue = nData_Array[nReportIndex];
                    nMinValue = nData_Array[nReportIndex];
                }
                else
                {
                    if (nData_Array[nReportIndex] > nMaxValue)
                        nMaxValue = nData_Array[nReportIndex];

                    if (nData_Array[nReportIndex] < nMinValue)
                        nMinValue = nData_Array[nReportIndex];
                }

                dMeanValue += nData_Array[nReportIndex];
                nDataCount++;
            }

            dMeanValue = Math.Round(dMeanValue / nDataCount, 2, MidpointRounding.AwayFromZero);
            dStdValue = ComputeStdValue(nData_Array);

            cReferenceValue.m_nTotalMax = nMaxValue;
            cReferenceValue.m_nTotalMin = nMinValue;
            cReferenceValue.m_dTotalMean = dMeanValue;
            cReferenceValue.m_dTotalStd = dStdValue;

            double dMeanPlus1Std = Math.Round(dMeanValue + dStdValue, 2, MidpointRounding.AwayFromZero);
            cReferenceValue.m_dTotalMeanPlus1Std = dMeanPlus1Std;

            double dMeanMinus1Std = Math.Round(dMeanValue - dStdValue, 2, MidpointRounding.AwayFromZero);
            cReferenceValue.m_dTotalMeanMinus1Std = (dMeanMinus1Std < 0) ? 0 : dMeanMinus1Std;

            int nMedianValue = 0;

            //數量為偶數
            if (nData_Array.Length % 2 == 0)    
            {
                int nIndex = nData_Array.Length / 2;
                double dLeftValue = double.Parse(nData_Array[nIndex - 1].ToString());
                double dRightValue = double.Parse(nData_Array[nIndex].ToString());
                nMedianValue = (int)((dLeftValue + dRightValue) / 2);
            }
            //數量為奇數
            else
            {
                int nIndex = (nData_Array.Length + 1) / 2;
                nMedianValue = (int)(double.Parse(nData_Array[nIndex - 1].ToString()));
            }

            cReferenceValue.m_nTotalMedian = nMedianValue;

            #region Bottom Mark_1
            int n25PCTIndex = nData_Array.Length / 4;
            double d25PCTMean = 0;

            for (int nDataIndex = 0; nDataIndex < n25PCTIndex; nDataIndex++)
                d25PCTMean += nData_Array[nDataIndex];

            d25PCTMean = d25PCTMean / n25PCTIndex;

            cReferenceValue.m_nTotalBottomMark_1 = (int)d25PCTMean;
            #endregion

            #region Bottom Mark_2
            int n12PCTValue = 0;

            if (nData_Array.Length % 8 == 0)
            {
                int n12PCTIndex = nData_Array.Length / 8;

                if (n12PCTIndex == 0)
                    n12PCTIndex = 1;

                n12PCTValue = (int)(double.Parse(nData_Array[n12PCTIndex - 1].ToString()));
            }
            else
            {
                int n12PCTIndex = nData_Array.Length / 8;

                if (n12PCTIndex == 0)
                    n12PCTIndex = 1;

                double dLeftValue = double.Parse(nData_Array[n12PCTIndex - 1].ToString());
                double dRightValue = double.Parse(nData_Array[n12PCTIndex].ToString());
                n12PCTValue = (int)((dLeftValue + dRightValue) / 2);
            }

            cReferenceValue.m_nTotalBottomMark_2 = n12PCTValue;
            #endregion

            if (cReferenceValue.m_nTotalMax < m_nPartNumber)
                m_nPartNumber = cReferenceValue.m_nTotalMax;

            int nValueOfPart = (int)(cReferenceValue.m_nTotalMax / m_nPartNumber);
            int nRealPartNumber = Convert.ToInt32(Math.Ceiling((double)cReferenceValue.m_nTotalMax / nValueOfPart));

            if (cReferenceValue.m_nTotalMax % nValueOfPart == 0)
                nRealPartNumber++;

            cReferenceValue.m_nRealPartNumber = nRealPartNumber;
            cReferenceValue.m_nValueOfPart = nValueOfPart;

            return cReferenceValue;
        }

        private void ComputeTraceNumberAndPreviousThreshold(int[] nData_Array, int nFileIndex, bool bRXTypeFlag = true)
        {
            if (bRXTypeFlag == true)
                m_cDataValue_List[nFileIndex].m_nRXTraceNumber = m_nRXTraceNumber;
            else
                m_cDataValue_List[nFileIndex].m_nTXTraceNumber = m_nTXTraceNumber;

            int nValue_M1 = ComputeFrontPCTValue(nData_Array, 10);

            if (bRXTypeFlag == true)
                m_cDataValue_List[nFileIndex].m_nRXPreliminaryTH_M1 = nValue_M1;
            else
                m_cDataValue_List[nFileIndex].m_nTXPreliminaryTH_M1 = nValue_M1;

            double dRatio = m_dHoverTHRatio_RX;

            if (m_eSubStep == SubTuningStep.CONTACT)
            {
                if (bRXTypeFlag == true)
                    dRatio = m_dContactTHRatio_RX;
                else
                    dRatio = m_dContactTHRatio_TX;
            }
            else
            {
                if (bRXTypeFlag == true)
                    dRatio = m_dHoverTHRatio_RX;
                else
                    dRatio = m_dHoverTHRatio_TX;
            }

            int nValue_M2 = 0;

            if (bRXTypeFlag == true)
            {
                nValue_M2 = (int)Math.Round(m_cDataValue_List[nFileIndex].m_cRXReferenceValue.m_nTotalMax * dRatio, 0, MidpointRounding.AwayFromZero);
                m_cDataValue_List[nFileIndex].m_nRXPreliminaryTH_M2 = nValue_M2;
                m_cDataValue_List[nFileIndex].m_nRXPreliminaryTRxSThreshold = Math.Max(nValue_M1, nValue_M2);

                if (m_cDataInfo_List[nFileIndex].m_nP0_DetectTime_Index == 1)
                    m_cDataValue_List[nFileIndex].m_nRXPreliminaryThreshold = (int)(m_cDataValue_List[nFileIndex].m_nRXPreliminaryTRxSThreshold / 2);
                else
                    m_cDataValue_List[nFileIndex].m_nRXPreliminaryThreshold = m_cDataValue_List[nFileIndex].m_nRXPreliminaryTRxSThreshold;
            }
            else
            {
                nValue_M2 = (int)Math.Round(m_cDataValue_List[nFileIndex].m_cTXReferenceValue.m_nTotalMax * dRatio, 0, MidpointRounding.AwayFromZero);
                m_cDataValue_List[nFileIndex].m_nTXPreliminaryTH_M2 = nValue_M2;
                m_cDataValue_List[nFileIndex].m_nTXPreliminaryTRxSThreshold = Math.Max(nValue_M1, nValue_M2);

                if (m_cDataInfo_List[nFileIndex].m_nP0_DetectTime_Index == 1)
                    m_cDataValue_List[nFileIndex].m_nTXPreliminaryThreshold = (int)(m_cDataValue_List[nFileIndex].m_nTXPreliminaryTRxSThreshold / 2);
                else
                    m_cDataValue_List[nFileIndex].m_nTXPreliminaryThreshold = m_cDataValue_List[nFileIndex].m_nTXPreliminaryTRxSThreshold;
            }
        }

        private int ComputeFrontPCTValue(int[] nData_Array, int nPCTValue)
        {
            int nValue = 0;
            int nConvertValue = (int)Math.Round(((double)100 / nPCTValue), 0, MidpointRounding.AwayFromZero);

            if (nData_Array.Length % nConvertValue == 0)
            {
                int nIndex = nData_Array.Length / nConvertValue;

                if (nIndex == 0)
                    nIndex = 1;

                nValue = (int)(double.Parse(nData_Array[nIndex - 1].ToString()));
            }
            else
            {
                int nIndex = nData_Array.Length / nConvertValue;

                if (nIndex == 0)
                    nIndex = 1;

                double dLeftValue = double.Parse(nData_Array[nIndex - 1].ToString());
                double dRightValue = double.Parse(nData_Array[nIndex].ToString());
                nValue = (int)((dLeftValue + dRightValue) / 2);
            }

            return nValue;
        }

        private List<int> GetStraightUsefulData(List<List<int>> nData_List, List<int> nIndex_List, int nLeftBoundary, int nRightBoundary)
        {
            List<int> nResultData_List = new List<int>();

            for (int nDataIndex = 0; nDataIndex < nData_List.Count; nDataIndex++)
            {
                if (nIndex_List[nDataIndex] <= nLeftBoundary || nIndex_List[nDataIndex] >= nRightBoundary)
                    continue;

                int nMaxValue = nData_List[nDataIndex].Max();

                /*
                if (nData_List[nDataIndex][StraightUsefulDataNumber - 1] == nMaxValue)
                    nResultData_List.Add(nData_List[nDataIndex][m_nStraightUsefulDataNumber - 1]);
                 */

                if (nData_List[nDataIndex][nIndex_List[nDataIndex]] == nMaxValue)
                    nResultData_List.Add(nData_List[nDataIndex][nIndex_List[nDataIndex]]);
            }

            return nResultData_List;
        }

        private void ComputeFilterValueByDigiGainRatio(ref int nRXFilterValue, ref int nTXFilterValue, int nFileIndex)
        {
            if (m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Rx != -1 && m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Rx != -1)
            {
                double dDigiGainRatio = (double)m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Rx / (double)m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Rx;
                dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                nRXFilterValue = (int)(nRXFilterValue * dDigiGainRatio);
                m_cDataValue_List[nFileIndex].m_nFilterRXValue = nRXFilterValue;
            }

            if (m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Tx != -1 && m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Tx != -1)
            {
                double dDigiGainRatio = (double)m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Tx / (double)m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Tx;
                dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                nTXFilterValue = (int)(nTXFilterValue * dDigiGainRatio);
                m_cDataValue_List[nFileIndex].m_nFilterTXValue = nTXFilterValue;
            }
        }

        private bool GetFilterData(ref List<int> nResultData_List, int[] nData_Array, int nNoiseInnerMax)
        {
            nResultData_List.Clear();

            if (nNoiseInnerMax < 0)
                return false;

            for (int nDataIndex = 0; nDataIndex < nData_Array.Length; nDataIndex++)
            {
                if (nData_Array[nDataIndex] > nNoiseInnerMax)
                    nResultData_List.Add(nData_Array[nDataIndex]);
            }

            return true;
        }

        private int[] ConvertListDataToArray(List<int> nData_List)
        {
            int[] nResultData_Array = new int[nData_List.Count];

            for (int nDataIndex = 0; nDataIndex < nData_List.Count; nDataIndex++)
                nResultData_Array[nDataIndex] = nData_List[nDataIndex];

            return nResultData_Array;
        }

        private void WriteHistogramDataFile(ReferenceValue cRXReferenceValue, ReferenceValue cTXReferenceValue, int[,] nRXData_Array, int[,] nTXData_Array, string sFilePath)
        {
            string[] sDataTypeName_Array = new string[2] 
            { 
                SpecificText.m_sScale,
                SpecificText.m_sAmount
            };

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("Total Histogram Data");
                WriteHistogramDataToCSVFile(sw, "RX", cRXReferenceValue);

                sw.WriteLine();

                sw.WriteLine("RX Total Histogram Data");
                WriteDataArrayToCSVFile(sw, sDataTypeName_Array, nRXData_Array);

                sw.WriteLine();
                sw.WriteLine();

                WriteHistogramDataToCSVFile(sw, "TX", cTXReferenceValue);

                sw.WriteLine();

                sw.WriteLine("TX Total Histogram Data");
                WriteDataArrayToCSVFile(sw, sDataTypeName_Array, nTXData_Array);
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private bool SaveHistogramChartPicture(HistogramInfo cHistogramInfo, ReferenceValue cRXReferenceValue, ReferenceValue cTXReferenceValue, int[,] nRXData_Array, int[,] nTXData_Array, string sFolderPath, int nHoverRaiseHeight = -1)
        {
            int nRXMaxAmount = 0;
            int nTXMaxAmount = 0;

            int nRXMaxScaleValue = GetMaximumScaleValue(nRXMaxAmount, nRXData_Array) + 1;
            int nTXMaxScaleValue = GetMaximumScaleValue(nTXMaxAmount, nTXData_Array) + 1;

            cHistogramInfo.m_nTraceType = MainConstantParameter.m_nTRACETYPE_RX;
            SaveHistogramChart(cHistogramInfo, cRXReferenceValue, nRXData_Array, nRXMaxScaleValue, sFolderPath, nHoverRaiseHeight);

            cHistogramInfo.m_nTraceType = MainConstantParameter.m_nTRACETYPE_TX;
            SaveHistogramChart(cHistogramInfo, cTXReferenceValue, nTXData_Array, nTXMaxScaleValue, sFolderPath, nHoverRaiseHeight);

            return true;
        }

        private void SaveHistogramChart(HistogramInfo cHistogramInfo, ReferenceValue cReferenceValue, int[,] nData_Array, int nMaxScaleValue, string sFolderPath, int nHoverRaiseHeight)
        {
            string sTitleName = "";
            string sFilePath = "";

            if (cHistogramInfo.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX)
            {
                sTitleName = "RX Total Histogram";
                sFilePath = string.Format(@"{0}\RX_{1}", sFolderPath, SpecificText.m_sHistogramFileName);
            }
            else
            {
                sTitleName = "TX Total Histogram";
                sFilePath = string.Format(@"{0}\TX_{1}", sFolderPath, SpecificText.m_sHistogramFileName);
            }

            if (nHoverRaiseHeight != -1)
                sTitleName = string.Format("{0}(Hover:{1}mm)", sTitleName, nHoverRaiseHeight);

            Chart cChart = new Chart();
            var vChartArea = new ChartArea();
            cChart.ChartAreas.Add(vChartArea);
            cChart.Width = cHistogramInfo.m_nWidth;
            cChart.Height = cHistogramInfo.m_nHeight;
            cChart.Legends.Add("Legend");
            cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
            cChart.Titles.Add(sTitleName);
            Series serSeries = new Series(sTitleName);
            serSeries.ChartType = SeriesChartType.Column;
            serSeries.BorderWidth = 1;
            serSeries.BorderColor = Color.Black;
            serSeries.CustomProperties = "PointWidth=1";
            cChart.ChartAreas[0].AxisY.Maximum = nMaxScaleValue;
            cChart.ChartAreas[0].AxisY.Title = "Amount";
            cChart.ChartAreas[0].AxisX.Maximum = nData_Array[0, nData_Array.GetLength(1) - 1];
            cChart.ChartAreas[0].AxisX.Minimum = 0;
            cChart.ChartAreas[0].AxisX.Interval = cReferenceValue.m_nValueOfPart;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            cChart.ChartAreas[0].AxisX.Title = "Value";
            serSeries.IsValueShownAsLabel = true;

            for (int nDataIndex = 0; nDataIndex < nData_Array.GetLength(1); nDataIndex++)
            {
                double dScale = 0;

                if (nDataIndex == 0)
                    dScale = (double)nData_Array[0, nDataIndex] / 2;
                else
                    dScale = ((double)nData_Array[0, nDataIndex - 1] + (double)nData_Array[0, nDataIndex]) / 2;

                serSeries.Points.AddXY(dScale, nData_Array[1, nDataIndex]);
            }

            cChart.Series.Add(serSeries);

            List<string> sLegend_List = new List<string>();
            List<double> dValue_List = new List<double>();
            List<Color> colorBackColor_List = new List<Color>();

            SetHistogramPictureValue(ref sLegend_List, ref dValue_List, ref colorBackColor_List, cReferenceValue);

            for (int nLegendIndex = 0; nLegendIndex < sLegend_List.Count; nLegendIndex++)
            {
                cChart.Series.Add(sLegend_List[nLegendIndex]);
                cChart.Series[sLegend_List[nLegendIndex]].Points.Add(new DataPoint(dValue_List[nLegendIndex], 0));
                cChart.Series[sLegend_List[nLegendIndex]].Points.Add(new DataPoint(dValue_List[nLegendIndex], nMaxScaleValue));
                cChart.Series[sLegend_List[nLegendIndex]].ChartType = SeriesChartType.Line;
                cChart.Series[sLegend_List[nLegendIndex]].BorderWidth = 2;
                cChart.Series[sLegend_List[nLegendIndex]].Color = colorBackColor_List[nLegendIndex];
            }

            cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
        }

        private int GetMinimumGroupMean(int[] nData_Array, int[,] nHistogram_Array, ReferenceValue cReferenceValue)
        {
            double dMinGroupMean = 0;

            for (int nPartIndex = 0; nPartIndex < cReferenceValue.m_nRealPartNumber; nPartIndex++)
            {
                if (nHistogram_Array[1, nPartIndex] > 0)
                {
                    int nCount = 0;

                    for (int nDataIndex = 0; nDataIndex < nData_Array.Length; nDataIndex++)
                    {
                        int nPartValue = (int)(nData_Array[nDataIndex] / cReferenceValue.m_nValueOfPart);

                        if (nPartValue == nPartIndex)
                        {
                            dMinGroupMean += nData_Array[nDataIndex];
                            nCount++;
                        }
                    }

                    dMinGroupMean = dMinGroupMean / nCount;
                    break;
                }
            }

            return (int)dMinGroupMean;
        }

        private int GetMaximumScaleValue(int nMaxAmount, int[,] nData_Array)
        {
            for (int nDataIndex = 0; nDataIndex < nData_Array.GetLength(1); nDataIndex++)
            {
                if (nData_Array[1, nDataIndex] > nMaxAmount)
                    nMaxAmount = nData_Array[1, nDataIndex];
            }

            int nLength = (int)Math.Log10(nMaxAmount) + 1;
            int nMaxScaleValue = ((int)(nMaxAmount / (Math.Pow(10, (nLength - 1)))) + 1) * (int)(Math.Pow(10, (nLength - 1))) - 1;

            return nMaxScaleValue;
        }

        private int[,] ComputeStatisticHistogram(int[] nData_Array, ReferenceValue cReferenceValue)
        {
            int[,] nHistogram_Array = new int[2, cReferenceValue.m_nRealPartNumber];
            Array.Clear(nHistogram_Array, 0, nHistogram_Array.Length);

            for (int nPartIndex = 0; nPartIndex < cReferenceValue.m_nRealPartNumber; nPartIndex++)
                nHistogram_Array[0, nPartIndex] = (nPartIndex + 1) * cReferenceValue.m_nValueOfPart;

            for (int nDataIndex = 0; nDataIndex < nData_Array.Length; nDataIndex++)
            {
                int nPartNumber = (int)(nData_Array[nDataIndex] / cReferenceValue.m_nValueOfPart);
                nHistogram_Array[1, nPartNumber] += 1;
            }

            return nHistogram_Array;
        }

        private void WriteDataArrayToCSVFile(StreamWriter sw, string[] sValueTypeName_Array, int[,] nData_Array)
        {
            for (int nXIndex = 0; nXIndex < nData_Array.GetLength(0); nXIndex++)
            {
                sw.Write(string.Format("{0},", sValueTypeName_Array[nXIndex]));

                for (int nYIndex = 0; nYIndex < nData_Array.GetLength(1); nYIndex++)
                {
                    sw.Write(string.Format("{0}", nData_Array[nXIndex, nYIndex]));

                    if (nYIndex < nData_Array.GetLength(1) - 1)
                        sw.Write(",");
                    else
                        sw.WriteLine();
                }
            }
        }

        private void WriteStraightDataFile(string sTitleName, int[] nProcessData_Array, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                int nDataLength = nProcessData_Array.Length;

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    string sText = "";

                    sText += string.Format("{0}", nProcessData_Array[nDataIndex]);
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
            string[] sValueTypeName_Array = new string[34] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sFlowStep, 
                SpecificText.m_sSettingPH1, 
                SpecificText.m_sSettingPH2, 
                SpecificText.m_sReadPH1, 
                SpecificText.m_sReadPH2, 
                SpecificText.m_sFrequency, 
                SpecificText.m_sP0_Detect_Time,
                SpecificText.m_sFilterRXValue, 
                SpecificText.m_sFilterTXValue, 
                SpecificText.m_sRXTotalMax, 
                SpecificText.m_sRXTotalMedian, 
                SpecificText.m_sRXTotalMin, 
                SpecificText.m_sRXTotalMean, 
                SpecificText.m_sRXTotalSTD, 
                SpecificText.m_sRXMeanPlus1STD, 
                SpecificText.m_sRXMeanMinus1STD, 
                SpecificText.m_sRXPreTH_M1, 
                SpecificText.m_sRXPreTH_M2, 
                SpecificText.m_sRXPreThreshold, 
                SpecificText.m_sRXPreTRxSThreshold,
                SpecificText.m_sTXTotalMax, 
                SpecificText.m_sTXTotalMedian, 
                SpecificText.m_sTXTotalMin, 
                SpecificText.m_sTXTotalMean, 
                SpecificText.m_sTXTotalSTD, 
                SpecificText.m_sTXMeanPlus1STD, 
                SpecificText.m_sTXMeanMinus1STD, 
                SpecificText.m_sTXPreTH_M1, 
                SpecificText.m_sTXPreTH_M2, 
                SpecificText.m_sTXPreThreshold, 
                SpecificText.m_sTXPreTRxSThreshold,
                SpecificText.m_sErrorMessage 
            };

            string[] sFWParameterTypeName_Array = null;

            if (m_eSubStep == SubTuningStep.CONTACT)
            {
                sFWParameterTypeName_Array = new string[23] 
                { 
                    SpecificText.m_sRanking, 
                    SpecificText.m_sFileName, 
                    SpecificText.m_sPH1, 
                    SpecificText.m_sPH2, 
                    SpecificText.m_sFrequency, 
                    SpecificText.m_sRXTraceNumber, 
                    SpecificText.m_sTXTraceNumber, 
                    SpecificText.m_sNoiseP0_Detect_Time, 
                    SpecificText.m_sNoiseRXInnerMax, 
                    SpecificText.m_sNoiseTXInnerMax,
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
            }

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
                            string sP0_Detect_Time = "N/A";

                            if (m_cDataInfo_List[nDataIndex].m_nP0_DetectTime_Index >= 0)
                                sP0_Detect_Time = (m_cDataInfo_List[nDataIndex].m_nP0_DetectTime_Index == 1) ? SpecificText.m_sP0_Detect_Time_800 : SpecificText.m_sP0_Detect_Time_400;

                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_eSubStep.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", sP0_Detect_Time));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nFilterRXValue.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nFilterTXValue.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXReferenceValue.m_nTotalMax.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXReferenceValue.m_nTotalMedian.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXReferenceValue.m_nTotalMin.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXReferenceValue.m_dTotalMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXReferenceValue.m_dTotalStd.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXReferenceValue.m_dTotalMeanPlus1Std.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXReferenceValue.m_dTotalMeanMinus1Std.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nRXPreliminaryTH_M1.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nRXPreliminaryTH_M2.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nRXPreliminaryThreshold.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nRXPreliminaryTRxSThreshold.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXReferenceValue.m_nTotalMax.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXReferenceValue.m_nTotalMedian.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXReferenceValue.m_nTotalMin.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXReferenceValue.m_dTotalMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXReferenceValue.m_dTotalStd.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXReferenceValue.m_dTotalMeanPlus1Std.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXReferenceValue.m_dTotalMeanMinus1Std.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTXPreliminaryTH_M1.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTXPreliminaryTH_M2.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTXPreliminaryThreshold.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTXPreliminaryTRxSThreshold.ToString()));

                            string sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sErrorMessage);

                            if (ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode != "")
                                sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sRecordErrorMessage);

                            sw.WriteLine(sErrorMessage);

                            break;
                        }
                    }
                }

                if (m_eSubStep == SubTuningStep.CONTACT)
                {
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
                                string sNoiseP0_Detect_Time = "N/A";

                                if (m_cDataValue_List[nDataIndex].m_nFNoiseP0_Detect_Time_Index >= 0)
                                    sNoiseP0_Detect_Time = (m_cDataValue_List[nDataIndex].m_nFNoiseP0_Detect_Time_Index == 1) ? SpecificText.m_sP0_Detect_Time_800 : SpecificText.m_sP0_Detect_Time_400;

                                string sP0_TH = (m_cDataValue_List[nDataIndex].m_bGetP0_THFlag == false) ? "N/A" : m_cDataValue_List[nDataIndex].m_ncActivePen_FM_P0_TH.ToString();

                                string sEdge_1Trc_SubPwr = m_cDataValue_List[nDataIndex].m_nEdge_1Trc_SubPwr.ToString();
                                string sEdge_2Trc_SubPwr = m_cDataValue_List[nDataIndex].m_nEdge_2Trc_SubPwr.ToString();
                                string sEdge_3Trc_SubPwr = m_cDataValue_List[nDataIndex].m_nEdge_3Trc_SubPwr.ToString();
                                string sEdge_4Trc_SubPwr = m_cDataValue_List[nDataIndex].m_nEdge_4Trc_SubPwr.ToString();

                                if (m_cDataValue_List[nDataIndex].m_bGetEdge_SubPwrFlag == false)
                                {
                                    if (m_cDataValue_List[nDataIndex].m_nEdge_1Trc_SubPwr < 0)
                                        sEdge_1Trc_SubPwr = "N/A";

                                    if (m_cDataValue_List[nDataIndex].m_nEdge_2Trc_SubPwr < 0)
                                        sEdge_2Trc_SubPwr = "N/A";

                                    if (m_cDataValue_List[nDataIndex].m_nEdge_3Trc_SubPwr < 0)
                                        sEdge_3Trc_SubPwr = "N/A";

                                    if (m_cDataValue_List[nDataIndex].m_nEdge_4Trc_SubPwr < 0)
                                        sEdge_4Trc_SubPwr = "N/A";
                                }

                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                                sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nRXTraceNumber.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTXTraceNumber.ToString()));
                                sw.Write(string.Format("{0},", sNoiseP0_Detect_Time));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nNoiseRXInnerMax.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nNoiseTXInnerMax.ToString()));
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
                                if (m_eSubStep == SubTuningStep.HOVER_2ND)
                                {
                                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                    sw.Write(string.Format("{0},", FlowRobot.TOUCHLINE.ToString()));
                                    sw.Write(string.Format("{0},", FlowRecord.TRX.ToString()));
                                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                    sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));

                                }
                                else if (m_eSubStep == SubTuningStep.CONTACT)
                                {
                                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                    sw.Write(string.Format("{0},", FlowRobot.HOVERLINE.ToString()));
                                    sw.Write(string.Format("{0},", FlowRecord.TRxS.ToString()));
                                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                    sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));
                                }
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
                if (m_eSubStep != SubTuningStep.HOVER_1ST)
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
                }

                FileProcess cFileProcess = new FileProcess(m_cfrmMain);
                cFileProcess.WriteResultListTxtFile(m_eSubStep, m_nSubStepState);
            }

            OutputMessage("Analysis Complete!!");
        }

        private bool GetPreviousStepValue(List<DTNormalParameter> cParameter_List, DataInfo cDataInfo, DataValue cDataValue)
        {
            bool bErrorFlag = true;

            for (int nParameterIndex = 0; nParameterIndex < cParameter_List.Count; nParameterIndex++)
            {
                if (cParameter_List[nParameterIndex].m_nPH1 == cDataInfo.m_nReadPH1 &&
                    cParameter_List[nParameterIndex].m_nPH2 == cDataInfo.m_nReadPH2)
                {
                    int nGetParameterFlag = 0;

                    if (cDataValue.m_bGetNoiseP0_Detect_TimeFlag == false)
                    {
                        if (cParameter_List[nParameterIndex].m_nNoiseP0_Detect_Time_Idx != -1)
                        {
                            cDataValue.m_nFNoiseP0_Detect_Time_Index = cParameter_List[nParameterIndex].m_nNoiseP0_Detect_Time_Idx;
                            cDataValue.m_bGetNoiseP0_Detect_TimeFlag = true;
                            nGetParameterFlag |= 0x0001;
                        }
                        else if (cParameter_List[nParameterIndex].m_nDTHoverP0_Detect_Time_Idx != -1)
                        {
                            cDataValue.m_nFNoiseP0_Detect_Time_Index = cParameter_List[nParameterIndex].m_nDTHoverP0_Detect_Time_Idx;
                            cDataValue.m_bGetNoiseP0_Detect_TimeFlag = true;
                            nGetParameterFlag |= 0x0001;
                        }
                    }
                    else if (cDataValue.m_bGetNoiseP0_Detect_TimeFlag == true)
                    {
                        cDataValue.m_nFNoiseP0_Detect_Time_Index = cDataValue.m_nNoiseP0_Detect_Time_Index;
                        nGetParameterFlag |= 0x0001;
                    }

                    if (cDataValue.m_bGetP0_THFlag == false && cParameter_List[nParameterIndex].m_nNoiseRXInMaxPlus3InMaxSTD != -1)
                    {
                        cDataValue.m_ncActivePen_FM_P0_TH = cParameter_List[nParameterIndex].m_nNoiseRXInMaxPlus3InMaxSTD;
                        cDataValue.m_bGetP0_THFlag = true;
                        nGetParameterFlag |= 0x0002;
                    }
                    else if (cDataValue.m_bGetP0_THFlag == true)
                    {
                        cDataValue.m_ncActivePen_FM_P0_TH = cDataValue.m_nNoiseRXInMaxPlus3InMaxSTD;
                        nGetParameterFlag |= 0x0002;
                    }
                    else
                    {
                        nGetParameterFlag |= 0x0002;
                    }

                    if (cParameter_List[nParameterIndex].m_nHover_2ndRXPreTh != -1)
                    {
                        cDataValue.m_nHover_TH_Rx = cParameter_List[nParameterIndex].m_nHover_2ndRXPreTh;
                        nGetParameterFlag |= 0x0004;
                    }

                    if (cParameter_List[nParameterIndex].m_nHover_2ndTXPreTh != -1)
                    {
                        cDataValue.m_nHover_TH_Tx = cParameter_List[nParameterIndex].m_nHover_2ndTXPreTh;
                        nGetParameterFlag |= 0x0008;
                    }

                    if (cParameter_List[nParameterIndex].m_nHover_2ndRXPreTRxSTh != -1)
                    {
                        cDataValue.m_nTRxS_Hover_TH_Rx = cParameter_List[nParameterIndex].m_nHover_2ndRXPreTRxSTh;
                        nGetParameterFlag |= 0x0010;
                    }

                    if (cParameter_List[nParameterIndex].m_nHover_2ndTXPreTRxSTh != -1)
                    {
                        cDataValue.m_nTRxS_Hover_TH_Tx = cParameter_List[nParameterIndex].m_nHover_2ndTXPreTRxSTh;
                        nGetParameterFlag |= 0x0020;
                    }

                    if (cDataValue.m_bGetEdge_SubPwrFlag == false)
                    {
                        int nEdge_SubPwrFlag = 0;

                        if (cParameter_List[nParameterIndex].m_nNoiseTrcMaxMinusPreP0_TH_1Trc != -1)
                        {
                            cDataValue.m_nEdge_1Trc_SubPwr = cParameter_List[nParameterIndex].m_nNoiseTrcMaxMinusPreP0_TH_1Trc;
                            nEdge_SubPwrFlag |= 0x0001;
                        }

                        if (cParameter_List[nParameterIndex].m_nNoiseTrcMaxMinusPreP0_TH_2Trc != -1)
                        {
                            cDataValue.m_nEdge_2Trc_SubPwr = cParameter_List[nParameterIndex].m_nNoiseTrcMaxMinusPreP0_TH_2Trc;
                            nEdge_SubPwrFlag |= 0x0002;
                        }

                        if (cParameter_List[nParameterIndex].m_nNoiseTrcMaxMinusPreP0_TH_3Trc != -1)
                        {
                            cDataValue.m_nEdge_3Trc_SubPwr = cParameter_List[nParameterIndex].m_nNoiseTrcMaxMinusPreP0_TH_3Trc;
                            nEdge_SubPwrFlag |= 0x0004;
                        }

                        if (cParameter_List[nParameterIndex].m_nNoiseTrcMaxMinusPreP0_TH_4Trc != -1)
                        {
                            cDataValue.m_nEdge_4Trc_SubPwr = cParameter_List[nParameterIndex].m_nNoiseTrcMaxMinusPreP0_TH_4Trc;
                            nEdge_SubPwrFlag |= 0x0008;
                        }

                        if (nEdge_SubPwrFlag == 0x000F)
                        {
                            cDataValue.m_bGetEdge_SubPwrFlag = true;
                            nGetParameterFlag |= 0x0040;
                        }
                        else
                            nGetParameterFlag |= 0x0040;
                    }
                    else
                    {
                        cDataValue.m_nEdge_1Trc_SubPwr = cDataValue.m_nNoiseTRMaxMinusPreP0_TH_1Trc;
                        cDataValue.m_nEdge_2Trc_SubPwr = cDataValue.m_nNoiseTRMaxMinusPreP0_TH_2Trc;
                        cDataValue.m_nEdge_3Trc_SubPwr = cDataValue.m_nNoiseTRMaxMinusPreP0_TH_3Trc;
                        cDataValue.m_nEdge_4Trc_SubPwr = cDataValue.m_nNoiseTRMaxMinusPreP0_TH_4Trc;
                        nGetParameterFlag |= 0x0040;
                    }

                    if (nGetParameterFlag == 0x007F)
                        bErrorFlag = false;

                    /*
                    CheckState cCheckState = new CheckState(m_cfrmMain);
                    
                    if (cCheckState.CheckIndependentStep(m_eMainStep, m_eSubStep) != StateCheck.nSTEPSTATE_INDEPENDENT)
                    {
                        if (cDataValue.m_nNoiseDigiGain_P0 == -1)
                            cDataValue.m_nNoiseDigiGain_P0 = ElanConvert.ConvertScaleToDigiGain(cParameter_List[nParameterIndex].m_nNoiseDigiGain_P0);

                        if (cDataValue.m_nNoiseDigiGain_Beacon_Rx == -1)
                            cDataValue.m_nNoiseDigiGain_Beacon_Rx = ElanConvert.ConvertScaleToDigiGain(cParameter_List[nParameterIndex].m_nNoiseDigiGain_Beacon_Rx);

                        if (cDataValue.m_nNoiseDigiGain_Beacon_Tx == -1)
                            cDataValue.m_nNoiseDigiGain_Beacon_Tx = ElanConvert.ConvertScaleToDigiGain(cParameter_List[nParameterIndex].m_nNoiseDigiGain_Beacon_Tx);
                    }
                    */

                    break;
                }
            }

            return !bErrorFlag;
        }

        private void ComputeP0_THAndEdge_SubPwr(int nFileIndex)
        {
            if (m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_P0 != -1 && m_cDataInfo_List[nFileIndex].m_nDigiGain_P0 != -1)
            {
                double dDigiGainRatio = (double)m_cDataInfo_List[nFileIndex].m_nDigiGain_P0 / (double)m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_P0;
                dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                int nCurrentP0_TH = m_cDataValue_List[nFileIndex].m_ncActivePen_FM_P0_TH;
                int nCompensateP0_TH = (int)(nCurrentP0_TH * dDigiGainRatio);
                m_cDataValue_List[nFileIndex].m_ncActivePen_FM_P0_TH = nCompensateP0_TH;
            }

            if (m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Rx != -1 && m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Rx != -1)
            {
                double dDigiGainRatio = (double)m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Rx / (double)m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Rx;
                dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                int nCompensateEdge_1Trc_SubPwr = (int)(m_cDataValue_List[nFileIndex].m_nEdge_1Trc_SubPwr * dDigiGainRatio);
                int nCompensateEdge_2Trc_SubPwr = (int)(m_cDataValue_List[nFileIndex].m_nEdge_2Trc_SubPwr * dDigiGainRatio);
                int nCompensateEdge_3Trc_SubPwr = (int)(m_cDataValue_List[nFileIndex].m_nEdge_3Trc_SubPwr * dDigiGainRatio);
                int nCompensateEdge_4Trc_SubPwr = (int)(m_cDataValue_List[nFileIndex].m_nEdge_4Trc_SubPwr * dDigiGainRatio);
                m_cDataValue_List[nFileIndex].m_nEdge_1Trc_SubPwr = nCompensateEdge_1Trc_SubPwr;
                m_cDataValue_List[nFileIndex].m_nEdge_2Trc_SubPwr = nCompensateEdge_2Trc_SubPwr;
                m_cDataValue_List[nFileIndex].m_nEdge_3Trc_SubPwr = nCompensateEdge_3Trc_SubPwr;
                m_cDataValue_List[nFileIndex].m_nEdge_4Trc_SubPwr = nCompensateEdge_4Trc_SubPwr;
            }
        }

        private void GetCurrentStepValue(int nFileIndex)
        {
            m_cDataValue_List[nFileIndex].m_nTRxS_Contact_TH_Rx = m_cDataValue_List[nFileIndex].m_nRXPreliminaryThreshold;
            m_cDataValue_List[nFileIndex].m_nTRxS_Contact_TH_Tx = m_cDataValue_List[nFileIndex].m_nTXPreliminaryThreshold;

            m_cDataValue_List[nFileIndex].m_nContact_TH_Rx = m_cDataValue_List[nFileIndex].m_nRXPreliminaryThreshold;
            m_cDataValue_List[nFileIndex].m_nContact_TH_Tx = m_cDataValue_List[nFileIndex].m_nTXPreliminaryThreshold;
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
    }
}
