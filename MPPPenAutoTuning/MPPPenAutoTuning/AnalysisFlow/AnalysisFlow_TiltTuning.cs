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
using MathNet.Numerics;

namespace MPPPenAutoTuning
{
    public class AnalysisFlow_TiltTuning : AnalysisFlow
    {
        private const int m_nXTipIndexByte = 25;
        private const int m_nXTipPwrStartByte = 15;
        private const int m_nXRingIndexByte = 36;
        private const int m_nXRingPwrStartByte = 26;
        private const int m_nYTipIndexByte = 47;
        private const int m_nYTipPwrStartByte = 37;
        private const int m_nYRingIndexByte = 58;
        private const int m_nYRingPwrStartByte = 48;

        private const int m_nPowerCount = 5;
        private const int m_nOSRValue = 256;

        private int m_nPolyFitOrder = 4;

        private int m_nValidTipTraceNumber = 5;
        private int m_nRXValidReportNumber = 150;
        private int m_nTXValidReportNumber = 70;

        private double m_dHoverRatio_RX = 0.6;
        private double m_dHoverRatio_TX = 0.6;

        private List<byte> m_byteReport_List = new List<byte>();
        private List<List<byte>> m_byteData_Horizontal_List = new List<List<byte>>();
        private List<List<byte>> m_byteData_Vertical_List = new List<List<byte>>();
        private List<List<byte>> m_byteData_Slant_List = new List<List<byte>>();

        private List<int> m_nTraceIndex_RX_List = new List<int>();
        private List<int> m_nTraceIndex_TX_List = new List<int>();
        private List<int> m_nFirstMax_RX_List = new List<int>();
        private List<int> m_nSecondMax_RX_List = new List<int>();
        private List<int> m_nThirdMax_RX_List = new List<int>();
        private List<int> m_nFirstMax_TX_List = new List<int>();
        private List<int> m_nSecondMax_TX_List = new List<int>();
        private List<int> m_nThirdMax_TX_List = new List<int>();

        private List<int> m_nOriginalTraceIndex_RX_List = new List<int>();
        private List<int> m_nOriginalTraceIndex_TX_List = new List<int>();
        private List<int> m_nOriginalFirstMax_RX_List = new List<int>();
        private List<int> m_nOriginalSecondMax_RX_List = new List<int>();
        private List<int> m_nOriginalThirdMax_RX_List = new List<int>();
        private List<int> m_nOriginalFirstMax_TX_List = new List<int>();
        private List<int> m_nOriginalSecondMax_TX_List = new List<int>();
        private List<int> m_nOriginalThirdMax_TX_List = new List<int>();

        private List<int> m_nRingTraceIndex_RX_List = new List<int>();
        private List<int> m_nRingMax_RX_List = new List<int>();
        private List<int> m_nRingTraceIndex_TX_List = new List<int>();
        private List<int> m_nRingMax_TX_List = new List<int>();

        private List<int> m_nOriginalRingTraceIndex_RX_List = new List<int>();
        private List<int> m_nOriginalRingMax_RX_List = new List<int>();
        private List<int> m_nOriginalRingTraceIndex_TX_List = new List<int>();
        private List<int> m_nOriginalRingMax_TX_List = new List<int>();

        public class DataValue
        {
            public List<int> m_nXTipIndex_List = new List<int>();
            public List<List<int>> m_nXTipPwr_List = new List<List<int>>();
            public List<int> m_nXRingIndex_List = new List<int>();
            public List<List<int>> m_nXRingPwr_List = new List<List<int>>();
            public List<int> m_nYTipIndex_List = new List<int>();
            public List<List<int>> m_nYTipPwr_List = new List<List<int>>();
            public List<int> m_nYRingIndex_List = new List<int>();
            public List<List<int>> m_nYRingPwr_List = new List<List<int>>();

            public List<int> m_nSortTipIndex_List = new List<int>();
            public List<int> m_nValidTipIndex_List = new List<int>();
            public List<int> m_nMajorTipIndex_List = new List<int>();
            public List<List<int>> m_nMajorTipPwr_List = new List<List<int>>();
            public List<int> m_nMajorRingIndex_List = new List<int>();
            public List<List<int>> m_nMajorRingPwr_List = new List<List<int>>();

            public List<double> m_dMajorTipDev_List = new List<double>();
            public List<double> m_dMajorRingDev_List = new List<double>();
            public List<double> m_dMajorCoordDiff_List = new List<double>();

            public List<List<double>> m_dSingleCoordDiff_List = new List<List<double>>();
            public List<List<double>> m_dSingleCurve_List = new List<List<double>>();

            public List<double> m_dIndexRMSE_List = new List<double>();
            public double m_dRMSE = -1.0;
            public double m_dNormalizeRMSE = -1.0;

            public double m_dPTHFNormalizeRMSE = -1.0;
        }

        private List<DataValue> m_cDataValue_Horizontal_List = null;
        private List<DataValue> m_cDataValue_Vertical_List = null;

        public class SlantDataValue
        {
            public List<int> m_nRXTipIndex_List = new List<int>();
            public List<List<int>> m_nRXTipPwr_List = new List<List<int>>();
            public List<int> m_nRXRingIndex_List = new List<int>();
            public List<List<int>> m_nRXRingPwr_List = new List<List<int>>();
            public List<int> m_nTXTipIndex_List = new List<int>();
            public List<List<int>> m_nTXTipPwr_List = new List<List<int>>();
            public List<int> m_nTXRingIndex_List = new List<int>();
            public List<List<int>> m_nTXRingPwr_List = new List<List<int>>();

            public int m_nRXContactTH = -1;
            public int m_nTXContactTH = -1;
            public int m_nRXHoverTH = -1;
            public int m_nTXHoverTH = -1;

            public int m_nRXRingMeanMinus1STD = -1;
            public int m_nRXRingMeanMinus2STD = -1;
            public int m_nTXRingMeanMinus1STD = -1;
            public int m_nTXRingMeanMinus2STD = -1;

            public double m_dRXSecondMaxMean = 0.0;
            public double m_dTXSecondMaxMean = 0.0;
            public double m_dRXThirdMaxMean = 0.0;
            public double m_dTXThirdMaxMean = 0.0;

            public double m_dRXRingMean = 0.0;
            public double m_dRXRingSTD = 0.0;
            public double m_dTXRingMean = 0.0;
            public double m_dTXRingSTD = 0.0;

            public int m_nPTHF_Contact_TH_Rx = -1;
            public int m_nPTHF_Contact_TH_Tx = -1;
            public int m_nPTHF_Hover_TH_Rx = -1;
            public int m_nPTHF_Hover_TH_Tx = -1;

            public double m_dPTHF_RXRingMean = -1.0;
            public double m_dPTHF_TXRingMean = -1.0;
            public int m_nPTHF_RXRingMeanMinus1STD = -1;
            public int m_nPTHF_RXRingMeanMinus2STD = -1;
            public int m_nPTHF_TXRingMeanMinus1STD = -1;
            public int m_nPTHF_TXRingMeanMinus2STD = -1;

            public int m_nPwrTHTXHB = -1;
            public int m_nPwrTHTXLB = -1;
        }

        private List<SlantDataValue> m_cDataValue_Slant_List = null;

        public class TTDataInfo
        {
            public SubTuningStep m_eSubStep = SubTuningStep.ELSE;
            public int m_nSettingPH1 = -1;
            public int m_nSettingPH2 = -1;
            public int m_nReadPH1 = -1;
            public int m_nReadPH2 = -1;
            public double m_dFrequency = -1;

            public int m_nRankIndex = -1;

            public int m_nLineCount = -1;

            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public string m_sDrawLineType = StringConvert.m_sDRAWTYPE_NA;
        }

        private List<TTDataInfo> m_cTTDataInfo_Horizontal_List = null;
        private List<TTDataInfo> m_cTTDataInfo_Vertical_List = null;
        private List<TTDataInfo> m_cTTDataInfo_Slant_List = null;

        public class BHFDataResult
        {
            public double m_dTotalScore = -1.0;

            public int m_nNewRankIndex = -1;

            public string m_sResult = "";
            public string m_sPTHFErrorMessage = "";
        }

        private List<BHFDataResult> m_cBHFDataResult_List = null;

        public class RingValue
        {
            public double m_dMean = -1.0;
            public int m_nMeanMinus1STD = -1;
            public int m_nMeanMinus2STD = -1;

            public RingValue(double dMean, int dMeanMinus1STD, int dMeanMinus2STD)
            {
                m_dMean = dMean;
                m_nMeanMinus1STD = dMeanMinus1STD;
                m_nMeanMinus2STD = dMeanMinus2STD;
            }
        }

        private void ClearDataArray()
        {
            m_byteReport_List.Clear();
            m_byteData_Horizontal_List.Clear();
            m_byteData_Vertical_List.Clear();
            m_byteData_Slant_List.Clear();

            m_nTraceIndex_RX_List.Clear();
            m_nTraceIndex_TX_List.Clear();
            m_nFirstMax_RX_List.Clear();
            m_nSecondMax_RX_List.Clear();
            m_nThirdMax_RX_List.Clear();
            m_nFirstMax_TX_List.Clear();
            m_nSecondMax_TX_List.Clear();
            m_nThirdMax_TX_List.Clear();

            m_nOriginalTraceIndex_RX_List.Clear();
            m_nOriginalTraceIndex_TX_List.Clear();
            m_nOriginalFirstMax_RX_List.Clear();
            m_nOriginalSecondMax_RX_List.Clear();
            m_nOriginalThirdMax_RX_List.Clear();
            m_nOriginalFirstMax_TX_List.Clear();
            m_nOriginalSecondMax_TX_List.Clear();
            m_nOriginalThirdMax_TX_List.Clear();

            m_nRingTraceIndex_RX_List.Clear();
            m_nRingMax_RX_List.Clear();
            m_nRingTraceIndex_TX_List.Clear();
            m_nRingMax_TX_List.Clear();

            m_nOriginalRingTraceIndex_RX_List.Clear();
            m_nOriginalRingMax_RX_List.Clear();
            m_nOriginalRingTraceIndex_TX_List.Clear();
            m_nOriginalRingMax_TX_List.Clear();
        }

        public AnalysisFlow_TiltTuning(FlowStep cFlowStep, frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
            m_sFileDirectoryPath = m_cfrmMain.m_sFileDirectoryPath;
            m_sRecordProjectName = m_cfrmMain.m_sRecordProjectName;
            m_sFlowDirectoryPath = m_cfrmMain.m_sFlowDirectoryPath;

            InitializeParameter(cFlowStep);

            m_cDataValue_Horizontal_List = new List<DataValue>();
            m_cDataValue_Vertical_List = new List<DataValue>();
            m_cDataValue_Slant_List = new List<SlantDataValue>();

            m_cTTDataInfo_Horizontal_List = new List<TTDataInfo>();
            m_cTTDataInfo_Vertical_List = new List<TTDataInfo>();
            m_cTTDataInfo_Slant_List = new List<TTDataInfo>();

            SetRecordInfo();
            CreateErrorInfo();

            if (m_eSubStep == SubTuningStep.TILTTUNING_BHF)
                m_cBHFDataResult_List = new List<BHFDataResult>();
        }

        public override void LoadAnalysisParameter()
        {
            m_nReportDataLength = ParamAutoTuning.m_nReportDataLength;
            m_nShiftStartByte = ParamAutoTuning.m_nShiftStartByte;
            m_nShiftByteNumber = ParamAutoTuning.m_nShiftByteNumber;

            //m_nEdgeTraceNumber = ParamAutoTuning.m_nEdgeTraceNumber;
            m_nPartNumber = ParamAutoTuning.m_nPartNumber;

            m_nPolyFitOrder = ParamAutoTuning.m_nTTPolyFitOrder;

            m_nValidTipTraceNumber = ParamAutoTuning.m_nTTValidTipTraceNumber;
            m_nRXValidReportNumber = ParamAutoTuning.m_nTTRXValidReportNumber;
            m_nTXValidReportNumber = ParamAutoTuning.m_nTTTXValidReportNumber;

            if (m_eSubStep == SubTuningStep.TILTTUNING_PTHF)
            {
                m_dHoverRatio_RX = ParamAutoTuning.m_dTTPTHFHoverRatio_RX;
                m_dHoverRatio_TX = ParamAutoTuning.m_dTTPTHFHoverRatio_TX;
            }
            else if (m_eSubStep == SubTuningStep.TILTTUNING_BHF)
            {
                m_dHoverRatio_RX = ParamAutoTuning.m_dTTBHFHoverRatio_RX;
                m_dHoverRatio_TX = ParamAutoTuning.m_dTTBHFHoverRatio_TX;
            }
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

        public void GetData(List<TiltTuningParameter> cParameter_List)
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

            m_cfrmMain.OutputMainStatusStrip("Analysing...", 0, nFileCount, frmMain.m_nInitialFlag);

            foreach (string sFilePath in sValidReportDataFile_Array)
            {
                string sMessage = "";

                ClearDataArray();

                #region Get Report Data
                DataInfo cDataInfo = new DataInfo();
                DataValue cDataValue_Horizontal = new DataValue();
                DataValue cDataValue_Vertical = new DataValue();
                SlantDataValue cDataValue_Slant = new SlantDataValue();
                TTDataInfo cTTDataInfo_Horizontal = new TTDataInfo();
                TTDataInfo cTTDataInfo_Vertical = new TTDataInfo();
                TTDataInfo cTTDataInfo_Slant = new TTDataInfo();
                BHFDataResult cBHFDataResult = new BHFDataResult();

                m_cDataInfo_List.Add(cDataInfo);
                m_cDataValue_Horizontal_List.Add(cDataValue_Horizontal);
                m_cDataValue_Vertical_List.Add(cDataValue_Vertical);
                m_cDataValue_Slant_List.Add(cDataValue_Slant);
                m_cTTDataInfo_Horizontal_List.Add(cTTDataInfo_Horizontal);
                m_cTTDataInfo_Vertical_List.Add(cTTDataInfo_Vertical);
                m_cTTDataInfo_Slant_List.Add(cTTDataInfo_Slant);

                if (m_eSubStep == SubTuningStep.TILTTUNING_BHF)
                    m_cBHFDataResult_List.Add(cBHFDataResult);

                int nFileIndex = m_cDataInfo_List.Count - 1;
                cDataInfo = m_cDataInfo_List[nFileIndex];
                cDataValue_Horizontal = m_cDataValue_Horizontal_List[nFileIndex];
                cDataValue_Vertical = m_cDataValue_Vertical_List[nFileIndex];
                cDataValue_Slant = m_cDataValue_Slant_List[nFileIndex];
                cTTDataInfo_Horizontal = m_cTTDataInfo_Horizontal_List[nFileIndex];
                cTTDataInfo_Vertical = m_cTTDataInfo_Vertical_List[nFileIndex];
                cTTDataInfo_Slant = m_cTTDataInfo_Slant_List[nFileIndex];

                if (m_eSubStep == SubTuningStep.TILTTUNING_BHF)
                    cBHFDataResult = m_cBHFDataResult_List[nFileIndex];

                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);

                //Folder
                m_sIntegrationFolderPath = string.Format(@"{0}\{1}\Integration Data", m_sResultFolderPath, sFileName);
                string sComputeFolderPath = string.Format(@"{0}\{1}\Compute Data", m_sResultFolderPath, sFileName);
                string sPictureFolderPath = string.Format(@"{0}\{1}\Picture", m_sResultFolderPath, sFileName);
                Directory.CreateDirectory(m_sIntegrationFolderPath);
                Directory.CreateDirectory(sComputeFolderPath);
                Directory.CreateDirectory(sPictureFolderPath);

                //File
                m_sIntegrationFilePath = string.Format(@"{0}\Integration Data.csv", m_sIntegrationFolderPath);
                string sHorizontalComputeFilePath = string.Format(@"{0}\Hor Compute Data.csv", sComputeFolderPath);
                string sVerticalComputeFilePath = string.Format(@"{0}\Ver Compute Data.csv", sComputeFolderPath);
                string sSlantComputeFilePath = string.Format(@"{0}\Slant Compute Data.csv", sComputeFolderPath);
                string sHorizontalUsefulFilePath = string.Format(@"{0}\Hor Useful Data.csv", sComputeFolderPath);
                string sVerticalUsefulFilePath = string.Format(@"{0}\Ver Useful Data.csv", sComputeFolderPath);
                string sHorizontalPolyFitFilePath = string.Format(@"{0}\Hor PolyFit Data.csv", sComputeFolderPath);
                string sVerticalPolyFitFilePath = string.Format(@"{0}\Ver PolyFit Data.csv", sComputeFolderPath);
                string sSlantRXDataFilePath = string.Format(@"{0}\Slant RX Data.csv", sComputeFolderPath);
                string sSlantTXDataFilePath = string.Format(@"{0}\Slant TX Data.csv", sComputeFolderPath);

                int nLineCounter = 0;
                string sLine = "";

                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                int nRankIndex = -1;

                GetFileInfoFromReportData(sFilePath, cDataInfo, cTTDataInfo_Horizontal, cTTDataInfo_Vertical, cTTDataInfo_Slant);

                if (CheckInfoIsCorrect(ref sMessage, cDataInfo, cTTDataInfo_Horizontal, cTTDataInfo_Vertical, cTTDataInfo_Slant) == false)
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

                        TiltTuningParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2);

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

                if (m_eSubStep == SubTuningStep.TILTTUNING_BHF)
                {
                    if (GetPreviousStepValue(cParameter_List, cDataInfo, cDataValue_Horizontal, cDataValue_Vertical, cDataValue_Slant, cBHFDataResult) == false)
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
                }

                int[] nDataLocation_Horizontal_Array = new int[2] 
                {
                    -1, 
                    -1
                };

                int[] nDataLocation_Vertical_Array = new int[2] 
                {
                    -1, 
                    -1
                };

                int[] nDataLocation_Slant_Array = new int[2] 
                {
                    -1, 
                    -1
                };

                if (cTTDataInfo_Horizontal.m_nLineCount > cTTDataInfo_Vertical.m_nLineCount)
                {
                    if (cTTDataInfo_Slant.m_nLineCount > cTTDataInfo_Vertical.m_nLineCount)
                    {
                        if (cTTDataInfo_Slant.m_nLineCount > cTTDataInfo_Horizontal.m_nLineCount)
                        {
                            nDataLocation_Horizontal_Array = new int[2] 
                            { 
                                cTTDataInfo_Horizontal.m_nLineCount, 
                                cTTDataInfo_Slant.m_nLineCount 
                            };

                            nDataLocation_Vertical_Array = new int[2] 
                            { 
                                cTTDataInfo_Vertical.m_nLineCount, 
                                cTTDataInfo_Horizontal.m_nLineCount 
                            };

                            nDataLocation_Slant_Array = new int[2] 
                            { 
                                cTTDataInfo_Slant.m_nLineCount, 
                                -1 
                            };
                        }
                        else
                        {
                            nDataLocation_Horizontal_Array = new int[2] 
                            { 
                                cTTDataInfo_Horizontal.m_nLineCount, 
                                -1 
                            };

                            nDataLocation_Vertical_Array = new int[2] 
                            { 
                                cTTDataInfo_Vertical.m_nLineCount, 
                                cTTDataInfo_Slant.m_nLineCount 
                            };

                            nDataLocation_Slant_Array = new int[2] 
                            { 
                                cTTDataInfo_Slant.m_nLineCount, 
                                cTTDataInfo_Horizontal.m_nLineCount 
                            };
                        }
                    }
                    else
                    {
                        nDataLocation_Horizontal_Array = new int[2] 
                        { 
                            cTTDataInfo_Horizontal.m_nLineCount, 
                            -1 
                        };

                        nDataLocation_Vertical_Array = new int[2] 
                        { 
                            cTTDataInfo_Vertical.m_nLineCount, 
                            cTTDataInfo_Horizontal.m_nLineCount 
                        };

                        nDataLocation_Slant_Array = new int[2] 
                        { 
                            cTTDataInfo_Slant.m_nLineCount, 
                            cTTDataInfo_Vertical.m_nLineCount 
                        };
                    }
                }
                else
                {
                    if (cTTDataInfo_Slant.m_nLineCount > cTTDataInfo_Horizontal.m_nLineCount)
                    {
                        if (cTTDataInfo_Slant.m_nLineCount > cTTDataInfo_Vertical.m_nLineCount)
                        {
                            nDataLocation_Horizontal_Array = new int[2] 
                            { 
                                cTTDataInfo_Horizontal.m_nLineCount, 
                                cTTDataInfo_Vertical.m_nLineCount 
                            };

                            nDataLocation_Vertical_Array = new int[2] 
                            { 
                                cTTDataInfo_Vertical.m_nLineCount, 
                                cTTDataInfo_Slant.m_nLineCount 
                            };

                            nDataLocation_Slant_Array = new int[2] 
                            { 
                                cTTDataInfo_Slant.m_nLineCount, 
                                -1 
                            };
                        }
                        else
                        {
                            nDataLocation_Horizontal_Array = new int[2] 
                            { 
                                cTTDataInfo_Horizontal.m_nLineCount, 
                                cTTDataInfo_Slant.m_nLineCount 
                            };

                            nDataLocation_Vertical_Array = new int[2] 
                            { 
                                cTTDataInfo_Vertical.m_nLineCount, 
                                -1 
                            };

                            nDataLocation_Slant_Array = new int[2] 
                            { 
                                cTTDataInfo_Slant.m_nLineCount, 
                                cTTDataInfo_Vertical.m_nLineCount 
                            };
                        }
                    }
                    else
                    {
                        nDataLocation_Horizontal_Array = new int[2] 
                        { 
                            cTTDataInfo_Horizontal.m_nLineCount, 
                            cTTDataInfo_Vertical.m_nLineCount 
                        };

                        nDataLocation_Vertical_Array = new int[2] 
                        { 
                            cTTDataInfo_Vertical.m_nLineCount, 
                            -1 
                        };

                        nDataLocation_Slant_Array = new int[2] 
                        { 
                            cTTDataInfo_Slant.m_nLineCount, 
                            cTTDataInfo_Horizontal.m_nLineCount 
                        };
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
                            byte[] byteData_Array = new byte[14];

                            for (int nIndex = 0; nIndex <= 13; nIndex++)
                                byteData_Array[nIndex] = Convert.ToByte(sSplit_Array[nIndex], 16);

                            if (byteData_Array[0] != 0x07 ||
                                byteData_Array[2] != 0xFF || 
                                byteData_Array[3] != 0xFF ||
                                byteData_Array[4] != 0xFF || 
                                byteData_Array[5] != 0xFF ||
                                (m_eSubStep == SubTuningStep.TILTTUNING_PTHF && byteData_Array[13] != 0xAA) ||
                                (m_eSubStep == SubTuningStep.TILTTUNING_BHF && byteData_Array[13] != 0xBB))
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

                        if (nDataLocation_Horizontal_Array[1] < 0)
                        {
                            if (nLineCounter > nDataLocation_Horizontal_Array[0])
                                m_byteData_Horizontal_List.Add(new List<byte>(m_byteReport_List));
                        }
                        else
                        {
                            if (nLineCounter > nDataLocation_Horizontal_Array[0] && nLineCounter < nDataLocation_Horizontal_Array[1])
                                m_byteData_Horizontal_List.Add(new List<byte>(m_byteReport_List));
                        }

                        if (nDataLocation_Vertical_Array[1] < 0)
                        {
                            if (nLineCounter > nDataLocation_Vertical_Array[0])
                                m_byteData_Vertical_List.Add(new List<byte>(m_byteReport_List));
                        }
                        else
                        {
                            if (nLineCounter > nDataLocation_Vertical_Array[0] && nLineCounter < nDataLocation_Vertical_Array[1])
                                m_byteData_Vertical_List.Add(new List<byte>(m_byteReport_List));
                        }

                        if (nDataLocation_Slant_Array[1] < 0)
                        {
                            if (nLineCounter > nDataLocation_Slant_Array[0])
                                m_byteData_Slant_List.Add(new List<byte>(m_byteReport_List));
                        }
                        else
                        {
                            if (nLineCounter > nDataLocation_Slant_Array[0] && nLineCounter < nDataLocation_Slant_Array[1])
                                m_byteData_Slant_List.Add(new List<byte>(m_byteReport_List));
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

                m_nRXTraceNumber = cDataInfo.m_nRXTraceNumber;
                m_nTXTraceNumber = cDataInfo.m_nTXTraceNumber;

                int nRXLeftBoundary = ParamAutoTuning.m_nDTValidReportEdgeNumber - 1;
                int nRXRightBoundary = m_nRXTraceNumber - ParamAutoTuning.m_nDTValidReportEdgeNumber;
                int nTXLeftBoundary = ParamAutoTuning.m_nDTValidReportEdgeNumber - 1;
                int nTXRightBoundary = m_nTXTraceNumber - ParamAutoTuning.m_nDTValidReportEdgeNumber;

                int nDataLength_Horizontal = m_byteData_Horizontal_List.Count;

                if (nDataLength_Horizontal == 0)
                {
                    if (m_nErrorFlag == 0)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0040;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Horizontal Report Data In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = "No Horizontal Report Data";
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    }
                }
                else
                {
                    GetRegionData(cDataValue_Horizontal, m_byteData_Horizontal_List, nFileIndex, true);

                    cDataValue_Horizontal.m_nSortTipIndex_List = GetSortIndex(cDataValue_Horizontal.m_nXTipIndex_List);

                    if (cDataValue_Horizontal.m_nSortTipIndex_List.Count < m_nValidTipTraceNumber)
                    {
                        if (m_nErrorFlag == 0)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0080;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Enough Horizontal Tip Trace({0}<LB({1})) In {2} File!", cDataValue_Horizontal.m_nSortTipIndex_List.Count, m_nValidTipTraceNumber, Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("No Enough Horizontal Tip Trace({0}<LB({1}))", cDataValue_Horizontal.m_nSortTipIndex_List.Count, m_nValidTipTraceNumber);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                        }
                    }
                    else
                    {
                        cDataValue_Horizontal.m_nValidTipIndex_List = GetUsefulIndex(cDataValue_Horizontal.m_nSortTipIndex_List);

                        GetMajorData(cDataValue_Horizontal, nFileIndex, true);

                        WriteComputeDataFile("Horizontal Compute Data", cDataValue_Horizontal, sHorizontalComputeFilePath);

                        GetSingleCoordDiff(cDataValue_Horizontal, nFileIndex, true);

                        WriteSingleCoordDiffDataFile("Horizontal CoordDiff Data", cDataValue_Horizontal, sHorizontalUsefulFilePath);

                        GetFinalCoordDiff(cDataValue_Horizontal, nFileIndex, true);

                        if (cDataValue_Horizontal.m_nValidTipIndex_List.Count == 0)
                        {
                            if (m_nErrorFlag == 0)
                            {
                                cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0100;

                                m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Enough Horizontal Useful Tip Trace({0})) In {1} File!", cDataValue_Horizontal.m_nValidTipIndex_List.Count, Path.GetFileName(sFilePath));
                                m_cErrorInfo.m_sRecordErrorMessage = string.Format("No Enough Horizontal Useful Tip Trace({0}))", cDataValue_Horizontal.m_nValidTipIndex_List.Count);
                                m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                                m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                                m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                                cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                            }
                        }
                        else
                        {
                            GetSingleCurve(cDataValue_Horizontal, nFileIndex, true);

                            ComputeIndexRMSE(cDataValue_Horizontal, nFileIndex, true);

                            WritePolyFitDataFile("Horizontal PolyFit Data", cDataValue_Horizontal, sHorizontalPolyFitFilePath);

                            cDataValue_Horizontal.m_dRMSE = ComputeMeanRMSE(cDataValue_Horizontal.m_dIndexRMSE_List);
                        }
                    }
                }

                int nDataLength_Vertical = m_byteData_Vertical_List.Count;

                if (nDataLength_Vertical == 0)
                {
                    if (m_nErrorFlag == 0)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0200;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Vertical Report Data In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = "No Vertical Report Data";
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    }
                }
                else
                {
                    GetRegionData(cDataValue_Vertical, m_byteData_Vertical_List, nFileIndex, false);

                    cDataValue_Vertical.m_nSortTipIndex_List = GetSortIndex(cDataValue_Vertical.m_nYTipIndex_List);

                    if (cDataValue_Vertical.m_nSortTipIndex_List.Count < m_nValidTipTraceNumber)
                    {
                        if (m_nErrorFlag == 0)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0400;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Enough Vertical Tip Trace({0}<LB({1})) In {2} File!", cDataValue_Horizontal.m_nSortTipIndex_List.Count, m_nValidTipTraceNumber, Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("No Enough Vertical Tip Trace({0}<LB({1}))", cDataValue_Horizontal.m_nSortTipIndex_List.Count, m_nValidTipTraceNumber);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                        }
                    }
                    else
                    {
                        cDataValue_Vertical.m_nValidTipIndex_List = GetUsefulIndex(cDataValue_Vertical.m_nSortTipIndex_List);

                        GetMajorData(cDataValue_Vertical, nFileIndex, false);

                        WriteComputeDataFile("Vertical Compute Data", cDataValue_Vertical, sVerticalComputeFilePath);

                        GetSingleCoordDiff(cDataValue_Vertical, nFileIndex, false);

                        WriteSingleCoordDiffDataFile("Vertical CoordDiff Data", cDataValue_Vertical, sVerticalUsefulFilePath);

                        GetFinalCoordDiff(cDataValue_Vertical, nFileIndex, false);

                        if (cDataValue_Vertical.m_nValidTipIndex_List.Count == 0)
                        {
                            if (m_nErrorFlag == 0)
                            {
                                cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0800;

                                m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Enough Vertical Useful Tip Trace({0})) In {1} File!", cDataValue_Vertical.m_nValidTipIndex_List.Count, Path.GetFileName(sFilePath));
                                m_cErrorInfo.m_sRecordErrorMessage = string.Format("No Enough Vertical Useful Tip Trace({0}))", cDataValue_Vertical.m_nValidTipIndex_List.Count);
                                m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                                m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                                m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                                cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                            }
                        }
                        else
                        {
                            GetSingleCurve(cDataValue_Vertical, nFileIndex, false);

                            ComputeIndexRMSE(cDataValue_Vertical, nFileIndex, false);

                            WritePolyFitDataFile("Vertical PolyFit Data", cDataValue_Vertical, sVerticalPolyFitFilePath);

                            cDataValue_Vertical.m_dRMSE = ComputeMeanRMSE(cDataValue_Vertical.m_dIndexRMSE_List);
                        }
                    }
                }

                int nDataLength_Slant = m_byteData_Slant_List.Count;

                if (nDataLength_Slant == 0)
                {
                    if (m_nErrorFlag == 0)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x1000;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Slant Report Data In {0} File!", Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = "No Slant Report Data";
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    }
                }
                else
                {
                    GetRegionData(cDataValue_Slant, m_byteData_Slant_List, nFileIndex);

                    WriteSlantComputeDataFile("Slant Compute Data", cDataValue_Slant, sSlantComputeFilePath);

                    GetStraightUsefulData(cDataValue_Slant, nRXLeftBoundary, nRXRightBoundary, true);
                    GetStraightUsefulData(cDataValue_Slant, nTXLeftBoundary, nTXRightBoundary, false);

                    if (m_nFirstMax_RX_List.Count < m_nRXValidReportNumber || m_nFirstMax_RX_List.Count == 0)
                    {
                        if (m_nErrorFlag == 0)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x2000;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Enough Slant RX Data({0}<LB({1})) In {2} File!", m_nFirstMax_RX_List.Count, m_nRXValidReportNumber, Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("No Enough Slant RX Data({0}<LB({1}))", m_nFirstMax_RX_List.Count, m_nRXValidReportNumber);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                        }
                    }
                    else
                    {
                        int[] nTipTraceIndexData_RX_Array = ConvertListDataToArray(m_nTraceIndex_RX_List);
                        int[] nFirstMaxData_RX_Array = ConvertListDataToArray(m_nFirstMax_RX_List);
                        int[] nSecondMaxData_RX_Array = ConvertListDataToArray(m_nSecondMax_RX_List);
                        int[] nThirdMaxData_RX_Array = ConvertListDataToArray(m_nThirdMax_RX_List);
                        int[] nRingTraceIndexData_RX_Array = ConvertListDataToArray(m_nRingTraceIndex_RX_List);
                        int[] nRingMaxData_RX_Array = ConvertListDataToArray(m_nRingMax_RX_List);

                        WriteStraightDataFile("RX Straight Data", nTipTraceIndexData_RX_Array, nFirstMaxData_RX_Array, nSecondMaxData_RX_Array, nThirdMaxData_RX_Array, nRingTraceIndexData_RX_Array, nRingMaxData_RX_Array, sSlantRXDataFilePath);

                        ComputeTipThreshold(m_nSecondMax_RX_List, m_nThirdMax_RX_List, nFileIndex, true);

                        ComputeRingThreshold(nRingMaxData_RX_Array, nFileIndex, true);

                        SaveLineChartFile(nFileIndex, sPictureFolderPath, true);
                    }

                    if (m_nFirstMax_TX_List.Count < m_nTXValidReportNumber || m_nFirstMax_TX_List.Count == 0)
                    {
                        if (m_nErrorFlag == 0)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x4000;

                            m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Enough Slant TX Data({0}<LB({1})) In {2} File!", m_nFirstMax_TX_List.Count, m_nTXValidReportNumber, Path.GetFileName(sFilePath));
                            m_cErrorInfo.m_sRecordErrorMessage = string.Format("No Enough Slant TX Data({0}<LB({1}))", m_nFirstMax_TX_List.Count, m_nTXValidReportNumber);
                            m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                            m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                            m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                            cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                        }
                    }
                    else
                    {
                        int[] nTipTraceIndexData_TX_List = ConvertListDataToArray(m_nTraceIndex_TX_List);
                        int[] nFirstMaxData_TX_List = ConvertListDataToArray(m_nFirstMax_TX_List);
                        int[] nSecondMaxData_TX_List = ConvertListDataToArray(m_nSecondMax_TX_List);
                        int[] nThirdMaxData_TX_List = ConvertListDataToArray(m_nThirdMax_TX_List);
                        int[] nRingTraceIndexData_TX_List = ConvertListDataToArray(m_nRingTraceIndex_TX_List);
                        int[] nRingMaxData_TX_List = ConvertListDataToArray(m_nRingMax_TX_List);

                        WriteStraightDataFile("TX Straight Data", nTipTraceIndexData_TX_List, nFirstMaxData_TX_List, nSecondMaxData_TX_List, nThirdMaxData_TX_List, nRingTraceIndexData_TX_List, nRingMaxData_TX_List, sSlantTXDataFilePath);

                        ComputeTipThreshold(m_nSecondMax_TX_List, m_nThirdMax_TX_List, nFileIndex, false);

                        ComputeRingThreshold(nRingMaxData_TX_List, nFileIndex, false);

                        SaveLineChartFile(nFileIndex, sPictureFolderPath, false);
                    }
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                if (m_eSubStep == SubTuningStep.TILTTUNING_BHF)
                    ComputeFinalRingThreshold(nFileIndex);

                if (cDataInfo.m_sRecordErrorCode != "")
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x8000;

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

        private void ComputeTipThreshold(List<int> nSecondMax_List, List<int> nThirdMax_List, int nFileIndex, bool bRXTraceTypeFlag = true)
        {
            double dSecondMaxMean = 0.0;
            double dThirdMaxMean = 0.0;
            int nThreshold = 0;

            for (int nIndex = 0; nIndex < nSecondMax_List.Count; nIndex++)
                dSecondMaxMean += nSecondMax_List[nIndex];

            dSecondMaxMean = Math.Round(dSecondMaxMean / nSecondMax_List.Count, 2, MidpointRounding.AwayFromZero);

            for (int nIndex = 0; nIndex < nThirdMax_List.Count; nIndex++)
                dThirdMaxMean += nThirdMax_List[nIndex];

            dThirdMaxMean = Math.Round(dThirdMaxMean / nThirdMax_List.Count, 2, MidpointRounding.AwayFromZero);

            nThreshold = (int)((dSecondMaxMean + dThirdMaxMean) / 2);

            if (bRXTraceTypeFlag == true)
            {
                m_cDataValue_Slant_List[nFileIndex].m_dRXSecondMaxMean = dSecondMaxMean;
                m_cDataValue_Slant_List[nFileIndex].m_dRXThirdMaxMean = dThirdMaxMean;
                m_cDataValue_Slant_List[nFileIndex].m_nRXContactTH = nThreshold;
                m_cDataValue_Slant_List[nFileIndex].m_nRXHoverTH = (int)(nThreshold * m_dHoverRatio_RX);
            }
            else
            {
                m_cDataValue_Slant_List[nFileIndex].m_dTXSecondMaxMean = dSecondMaxMean;
                m_cDataValue_Slant_List[nFileIndex].m_dTXThirdMaxMean = dThirdMaxMean;
                m_cDataValue_Slant_List[nFileIndex].m_nTXContactTH = nThreshold;
                m_cDataValue_Slant_List[nFileIndex].m_nTXHoverTH = (int)(nThreshold * m_dHoverRatio_TX);
            }
        }

        private void ComputeRingThreshold(int[] nMax_Array, int nFileIndex, bool bRXTraceTypeFlag = true)
        {
            double dMean = 0.0;
            double dStd = 0.0;
            int nMeanMinus1Std = 0;
            int nMeanMinus2Std = 0;

            for (int nIndex = 0; nIndex < nMax_Array.Length; nIndex++)
                dMean += nMax_Array[nIndex];

            dMean = Math.Round(dMean / nMax_Array.Length, 2, MidpointRounding.AwayFromZero);

            dStd = ComputeStdValue(nMax_Array);

            nMeanMinus1Std = (int)(dMean - 1 * dStd);
            
            if (nMeanMinus1Std < 0)
                nMeanMinus1Std = 0;

            nMeanMinus2Std = (int)(dMean - 2 * dStd);
            
            if (nMeanMinus2Std < 0)
                nMeanMinus2Std = 0;

            if (bRXTraceTypeFlag == true)
            {
                m_cDataValue_Slant_List[nFileIndex].m_dRXRingMean = dMean;
                m_cDataValue_Slant_List[nFileIndex].m_dRXRingSTD = dStd;
                m_cDataValue_Slant_List[nFileIndex].m_nRXRingMeanMinus1STD = nMeanMinus1Std;
                m_cDataValue_Slant_List[nFileIndex].m_nRXRingMeanMinus2STD = nMeanMinus2Std;
            }
            else
            {
                m_cDataValue_Slant_List[nFileIndex].m_dTXRingMean = dMean;
                m_cDataValue_Slant_List[nFileIndex].m_dTXRingSTD = dStd;
                m_cDataValue_Slant_List[nFileIndex].m_nTXRingMeanMinus1STD = nMeanMinus1Std;
                m_cDataValue_Slant_List[nFileIndex].m_nTXRingMeanMinus2STD = nMeanMinus2Std;
            }
        }

        public void ComputeAndOutputResult()
        {
            if (m_bErrorFlag == false)
            {
                ComputeNormalizeRMSE();

                if (m_eSubStep == SubTuningStep.TILTTUNING_BHF)
                    ComputeTotalScore();
            }

            OutputResultData();
        }

        private void GetFileInfoFromReportData(string sFilePath, DataInfo cDataInfo, TTDataInfo cTTDataInfo_Horizontal, TTDataInfo cTTDataInfo_Vertical, TTDataInfo cTTDataInfo_Slant)
        {
            long lGetInfoFlag_Horizontal = 0;
            long lGetInfoFlag_Vertical = 0;
            long lGetInfoFlag_Slant = 0;
            string sLine = "";

            cDataInfo.m_sFileName = Path.GetFileName(sFilePath);

            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            int nDrawLineState = 0;
            int nLineCount = 0;

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    if (sLine == string.Format("====={0}=====", StringConvert.m_sDRAWTYPE_HORIZONTAL))
                    {
                        nDrawLineState = 1;
                        cTTDataInfo_Horizontal.m_nLineCount = nLineCount;
                    }
                    else if (sLine == string.Format("====={0}=====", StringConvert.m_sDRAWTYPE_VERTICAL))
                    {
                        nDrawLineState = 2;
                        cTTDataInfo_Vertical.m_nLineCount = nLineCount;
                    }
                    else if (sLine == string.Format("====={0}=====", StringConvert.m_sDRAWTYPE_SLANT))
                    {
                        nDrawLineState = 3;
                        cTTDataInfo_Slant.m_nLineCount = nLineCount;
                    }

                    if (nDrawLineState == 1)
                    {
                        GetFileInfo(ref lGetInfoFlag_Horizontal, cTTDataInfo_Horizontal, StringConvert.m_sRECORD_SUBSTEP, sLine, 0x000001, m_nINFOTYPE_TUNINGSTEP);
                        GetFileInfo(ref lGetInfoFlag_Horizontal, cTTDataInfo_Horizontal, StringConvert.m_sRECORD_SETTINGPH1, sLine, 0x000002, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Horizontal, cTTDataInfo_Horizontal, StringConvert.m_sRECORD_SETTINGPH2, sLine, 0x000004, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Horizontal, cTTDataInfo_Horizontal, StringConvert.m_sRECORD_READPH1, sLine, 0x000008, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Horizontal, cTTDataInfo_Horizontal, StringConvert.m_sRECORD_READPH2, sLine, 0x000010, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Horizontal, cTTDataInfo_Horizontal, StringConvert.m_sRECORD_FREQUENCY, sLine, 0x000020, m_nINFOTYPE_DOUBLE);
                        GetFileInfo(ref lGetInfoFlag_Horizontal, cTTDataInfo_Horizontal, StringConvert.m_sRECORD_RANKINDEX, sLine, 0x000040, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Horizontal, cTTDataInfo_Horizontal, StringConvert.m_sRECORD_RXTRACENUMBER, sLine, 0x000080, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Horizontal, cTTDataInfo_Horizontal, StringConvert.m_sRECORD_TXTRACENUMBER, sLine, 0x000100, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Horizontal, cTTDataInfo_Horizontal, StringConvert.m_sRECORD_DRAWLINETYPE, sLine, 0x000200, m_nINFOTYPE_STRING);
                    }
                    else if (nDrawLineState == 2)
                    {
                        GetFileInfo(ref lGetInfoFlag_Vertical, cTTDataInfo_Vertical, StringConvert.m_sRECORD_SUBSTEP, sLine, 0x000001, m_nINFOTYPE_TUNINGSTEP);
                        GetFileInfo(ref lGetInfoFlag_Vertical, cTTDataInfo_Vertical, StringConvert.m_sRECORD_SETTINGPH1, sLine, 0x000002, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Vertical, cTTDataInfo_Vertical, StringConvert.m_sRECORD_SETTINGPH2, sLine, 0x000004, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Vertical, cTTDataInfo_Vertical, StringConvert.m_sRECORD_READPH1, sLine, 0x000008, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Vertical, cTTDataInfo_Vertical, StringConvert.m_sRECORD_READPH2, sLine, 0x000010, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Vertical, cTTDataInfo_Vertical, StringConvert.m_sRECORD_FREQUENCY, sLine, 0x000020, m_nINFOTYPE_DOUBLE);
                        GetFileInfo(ref lGetInfoFlag_Vertical, cTTDataInfo_Vertical, StringConvert.m_sRECORD_RANKINDEX, sLine, 0x000040, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Vertical, cTTDataInfo_Vertical, StringConvert.m_sRECORD_RXTRACENUMBER, sLine, 0x000080, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Vertical, cTTDataInfo_Vertical, StringConvert.m_sRECORD_TXTRACENUMBER, sLine, 0x000100, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Vertical, cTTDataInfo_Vertical, StringConvert.m_sRECORD_DRAWLINETYPE, sLine, 0x000200, m_nINFOTYPE_STRING);
                    }
                    else if (nDrawLineState == 3)
                    {
                        GetFileInfo(ref lGetInfoFlag_Slant, cTTDataInfo_Slant, StringConvert.m_sRECORD_SUBSTEP, sLine, 0x000001, m_nINFOTYPE_TUNINGSTEP);
                        GetFileInfo(ref lGetInfoFlag_Slant, cTTDataInfo_Slant, StringConvert.m_sRECORD_SETTINGPH1, sLine, 0x000002, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Slant, cTTDataInfo_Slant, StringConvert.m_sRECORD_SETTINGPH2, sLine, 0x000004, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Slant, cTTDataInfo_Slant, StringConvert.m_sRECORD_READPH1, sLine, 0x000008, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Slant, cTTDataInfo_Slant, StringConvert.m_sRECORD_READPH2, sLine, 0x000010, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Slant, cTTDataInfo_Slant, StringConvert.m_sRECORD_FREQUENCY, sLine, 0x000020, m_nINFOTYPE_DOUBLE);
                        GetFileInfo(ref lGetInfoFlag_Slant, cTTDataInfo_Slant, StringConvert.m_sRECORD_RANKINDEX, sLine, 0x000040, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Slant, cTTDataInfo_Slant, StringConvert.m_sRECORD_RXTRACENUMBER, sLine, 0x000080, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Slant, cTTDataInfo_Slant, StringConvert.m_sRECORD_TXTRACENUMBER, sLine, 0x000100, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag_Slant, cTTDataInfo_Slant, StringConvert.m_sRECORD_DRAWLINETYPE, sLine, 0x000200, m_nINFOTYPE_STRING);
                        GetFileInfo(ref lGetInfoFlag_Slant, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_PTHF_RX, sLine, 0x000400, m_nINFOTYPE_STRING);
                        GetFileInfo(ref lGetInfoFlag_Slant, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_PTHF_TX, sLine, 0x000800, m_nINFOTYPE_STRING);
                        GetFileInfo(ref lGetInfoFlag_Slant, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BHF_RX, sLine, 0x001000, m_nINFOTYPE_STRING);
                        GetFileInfo(ref lGetInfoFlag_Slant, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BHF_TX, sLine, 0x002000, m_nINFOTYPE_STRING);
                    }

                    if (lGetInfoFlag_Horizontal == 0x0003FF && lGetInfoFlag_Vertical == 0x0003FF && lGetInfoFlag_Slant == 0x003FFF)
                        break;

                    nLineCount++;
                }
            }
            finally
            {
                srFile.Close();
            }
        }

        private void GetFileInfo(ref long lGetInfoFlag, TTDataInfo cTTDataInfo, string sParameterName, string sLine, long lInfoFlag, int nValueType)
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
                                    cTTDataInfo.m_eSubStep = eSubStep;

                                break;
                            case m_nINFOTYPE_INT:
                                int nValue = 0;

                                if (sParameterName == StringConvert.m_sRECORD_RANKINDEX ||
                                    sParameterName == StringConvert.m_sRECORD_RXTRACENUMBER ||
                                    sParameterName == StringConvert.m_sRECORD_TXTRACENUMBER)
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
                                    cTTDataInfo.m_nSettingPH1 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_SETTINGPH2)
                                    cTTDataInfo.m_nSettingPH2 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_READPH1)
                                    cTTDataInfo.m_nReadPH1 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_READPH2)
                                    cTTDataInfo.m_nReadPH2 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RANKINDEX)
                                    cTTDataInfo.m_nRankIndex = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RXTRACENUMBER)
                                    cTTDataInfo.m_nRXTraceNumber = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_TXTRACENUMBER)
                                    cTTDataInfo.m_nTXTraceNumber = nValue;

                                break;
                            case m_nINFOTYPE_DOUBLE:
                                double dValue = 0;

                                if (Double.TryParse(sValue, out dValue) == true)
                                {
                                    if (sParameterName == StringConvert.m_sRECORD_FREQUENCY)
                                        cTTDataInfo.m_dFrequency = dValue;
                                }

                                break;
                            case m_nINFOTYPE_STRING:
                                if (sParameterName == StringConvert.m_sRECORD_DRAWLINETYPE)
                                {
                                    if (sValue == StringConvert.m_sDRAWTYPE_HORIZONTAL)
                                        cTTDataInfo.m_sDrawLineType = StringConvert.m_sDRAWTYPE_HORIZONTAL;
                                    else if (sValue == StringConvert.m_sDRAWTYPE_VERTICAL)
                                        cTTDataInfo.m_sDrawLineType = StringConvert.m_sDRAWTYPE_VERTICAL;
                                    else if (sValue == StringConvert.m_sDRAWTYPE_SLANT)
                                        cTTDataInfo.m_sDrawLineType = StringConvert.m_sDRAWTYPE_SLANT;
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

        private bool CheckInfoIsCorrect(ref string sErrorMessage, DataInfo cDataInfo, TTDataInfo cTTDataInfo_Horizontal, TTDataInfo cTTDataInfo_Vertical, TTDataInfo cTTDataInfo_Slant)
        {
            if (cTTDataInfo_Horizontal.m_eSubStep != cTTDataInfo_Vertical.m_eSubStep ||
                cTTDataInfo_Horizontal.m_eSubStep != cTTDataInfo_Slant.m_eSubStep ||
                cTTDataInfo_Vertical.m_eSubStep != cTTDataInfo_Slant.m_eSubStep)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor, Ver & Slant \"{0}\" Format Not Match", StringConvert.m_sRECORD_SUBSTEP));
            else
                cDataInfo.m_eSubStep = cTTDataInfo_Horizontal.m_eSubStep;

            if (cTTDataInfo_Horizontal.m_nSettingPH1 != cTTDataInfo_Vertical.m_nSettingPH1 ||
                cTTDataInfo_Horizontal.m_nSettingPH1 != cTTDataInfo_Slant.m_nSettingPH1 ||
                cTTDataInfo_Vertical.m_nSettingPH1 != cTTDataInfo_Slant.m_nSettingPH1)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor, Ver & Slant \"{0}\" Format Not Match", StringConvert.m_sRECORD_SETTINGPH1));
            else
                cDataInfo.m_nSettingPH1 = cTTDataInfo_Horizontal.m_nSettingPH1;

            if (cTTDataInfo_Horizontal.m_nReadPH1 != cTTDataInfo_Vertical.m_nReadPH1 ||
                cTTDataInfo_Horizontal.m_nReadPH1 != cTTDataInfo_Slant.m_nReadPH1 ||
                cTTDataInfo_Vertical.m_nReadPH1 != cTTDataInfo_Slant.m_nReadPH1)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor, Ver & Slant \"{0}\" Format Not Match", StringConvert.m_sRECORD_READPH1));
            else
                cDataInfo.m_nReadPH1 = cTTDataInfo_Horizontal.m_nReadPH1;

            if (cTTDataInfo_Horizontal.m_nSettingPH2 != cTTDataInfo_Vertical.m_nSettingPH2 ||
                cTTDataInfo_Horizontal.m_nSettingPH2 != cTTDataInfo_Slant.m_nSettingPH2 ||
                cTTDataInfo_Vertical.m_nSettingPH2 != cTTDataInfo_Slant.m_nSettingPH2)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor, Ver & Slant \"{0}\" Format Not Match", StringConvert.m_sRECORD_SETTINGPH2));
            else
                cDataInfo.m_nSettingPH2 = cTTDataInfo_Horizontal.m_nSettingPH2;

            if (cTTDataInfo_Horizontal.m_nReadPH2 != cTTDataInfo_Vertical.m_nReadPH2 ||
                cTTDataInfo_Horizontal.m_nReadPH2 != cTTDataInfo_Slant.m_nReadPH2 ||
                cTTDataInfo_Vertical.m_nReadPH2 != cTTDataInfo_Slant.m_nReadPH2)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor, Ver & Slant \"{0}\" Format Not Match", StringConvert.m_sRECORD_READPH2));
            else
                cDataInfo.m_nReadPH2 = cTTDataInfo_Horizontal.m_nReadPH2;

            if (cTTDataInfo_Horizontal.m_dFrequency != cTTDataInfo_Vertical.m_dFrequency ||
                cTTDataInfo_Horizontal.m_dFrequency != cTTDataInfo_Slant.m_dFrequency ||
                cTTDataInfo_Vertical.m_dFrequency != cTTDataInfo_Slant.m_dFrequency)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor, Ver & Slant \"{0}\" Format Not Match", StringConvert.m_sRECORD_FREQUENCY));
            else
                cDataInfo.m_dFrequency = cTTDataInfo_Horizontal.m_dFrequency;

            CheckGeneralInfoIsCorrect(ref sErrorMessage, cDataInfo);

            if (cTTDataInfo_Horizontal.m_nRankIndex != cTTDataInfo_Vertical.m_nRankIndex ||
                cTTDataInfo_Horizontal.m_nRankIndex != cTTDataInfo_Slant.m_nRankIndex ||
                cTTDataInfo_Vertical.m_nRankIndex != cTTDataInfo_Slant.m_nRankIndex)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor, Ver & Slant \"{0}\" Format Not Match", StringConvert.m_sRECORD_RANKINDEX));
            else
            {
                cDataInfo.m_nRankIndex = cTTDataInfo_Horizontal.m_nRankIndex;

                if (cDataInfo.m_nRankIndex < 0)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RANKINDEX));
            }

            if (cTTDataInfo_Horizontal.m_nRXTraceNumber != cTTDataInfo_Vertical.m_nRXTraceNumber ||
                cTTDataInfo_Horizontal.m_nRXTraceNumber != cTTDataInfo_Slant.m_nRXTraceNumber ||
                cTTDataInfo_Vertical.m_nRXTraceNumber != cTTDataInfo_Slant.m_nRXTraceNumber)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor, Ver & Slant \"{0}\" Format Not Match", StringConvert.m_sRECORD_RXTRACENUMBER));
            else
            {
                cDataInfo.m_nRXTraceNumber = cTTDataInfo_Horizontal.m_nRXTraceNumber;

                if (cDataInfo.m_nRXTraceNumber <= 0)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_RXTRACENUMBER));
            }

            if (cTTDataInfo_Horizontal.m_nTXTraceNumber != cTTDataInfo_Vertical.m_nTXTraceNumber ||
                cTTDataInfo_Horizontal.m_nTXTraceNumber != cTTDataInfo_Slant.m_nTXTraceNumber ||
                cTTDataInfo_Vertical.m_nTXTraceNumber != cTTDataInfo_Slant.m_nTXTraceNumber)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor, Ver & Slant \"{0}\" Format Not Match", StringConvert.m_sRECORD_TXTRACENUMBER));
            else
            {
                cDataInfo.m_nTXTraceNumber = cTTDataInfo_Horizontal.m_nTXTraceNumber;

                if (cDataInfo.m_nTXTraceNumber <= 0)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_TXTRACENUMBER));
            }

            if (cTTDataInfo_Horizontal.m_sDrawLineType != StringConvert.m_sDRAWTYPE_HORIZONTAL)
                SetErrorMessage(ref sErrorMessage, string.Format("Hor \"{0}\" Format Error", StringConvert.m_sRECORD_DRAWLINETYPE));
            else if (cTTDataInfo_Vertical.m_sDrawLineType != StringConvert.m_sDRAWTYPE_VERTICAL)
                SetErrorMessage(ref sErrorMessage, string.Format("Ver \"{0}\" Format Error", StringConvert.m_sRECORD_DRAWLINETYPE));
            else if (cTTDataInfo_Slant.m_sDrawLineType != StringConvert.m_sDRAWTYPE_SLANT)
                SetErrorMessage(ref sErrorMessage, string.Format("Slant \"{0}\" Format Error", StringConvert.m_sRECORD_DRAWLINETYPE));

            if (cTTDataInfo_Horizontal.m_nLineCount < 0)
                SetErrorMessage(ref sErrorMessage, "Get Horizontal Data Location Error");
            else if (cTTDataInfo_Vertical.m_nLineCount < 0)
                SetErrorMessage(ref sErrorMessage, "Get Vertical Data Location Error");
            else if (cTTDataInfo_Slant.m_nLineCount < 0)
                SetErrorMessage(ref sErrorMessage, "Get Slant Data Location Error");

            if (sErrorMessage != "")
                return false;

            return true;
        }

        private void GetRegionData(DataValue cOriginalDataValue, List<List<byte>> byteData_List, int nFileIndex, bool bHorizontalStatusFlag = true)
        {
            int nDataLength = byteData_List.Count;
            DataValue cDataValue = new DataValue();
            cDataValue = cOriginalDataValue;

            for (int nIndex = 0; nIndex < nDataLength; nIndex++)
            {
                int[] nXTipPwr = new int[m_nPowerCount];
                int[] nXRingPwr = new int[m_nPowerCount];
                int[] nYTipPwr = new int[m_nPowerCount];
                int[] nYRingPwr = new int[m_nPowerCount];

                for (int mIndex = 0; mIndex < 5; mIndex++)
                {
                    nXTipPwr[mIndex] = (byteData_List[nIndex][m_nXTipPwrStartByte + 2 * mIndex - 1] * 256 + byteData_List[nIndex][m_nXTipPwrStartByte + 2 * mIndex]);
                    nXRingPwr[mIndex] = (byteData_List[nIndex][m_nXRingPwrStartByte + 2 * mIndex - 1] * 256 + byteData_List[nIndex][m_nXRingPwrStartByte + 2 * mIndex]);
                    nYTipPwr[mIndex] = (byteData_List[nIndex][m_nYTipPwrStartByte + 2 * mIndex - 1] * 256 + byteData_List[nIndex][m_nYTipPwrStartByte + 2 * mIndex]);
                    nYRingPwr[mIndex] = (byteData_List[nIndex][m_nYRingPwrStartByte + 2 * mIndex - 1] * 256 + byteData_List[nIndex][m_nYRingPwrStartByte + 2 * mIndex]);
                }

                int nXTipIndex = byteData_List[nIndex][m_nXTipIndexByte - 1];
                int nXRingIndex = byteData_List[nIndex][m_nXRingIndexByte - 1];
                int nYTipIndex = byteData_List[nIndex][m_nYTipIndexByte - 1];
                int nYRingIndex = byteData_List[nIndex][m_nYRingIndexByte - 1];

                cDataValue.m_nXTipPwr_List.Add(new List<int>(nXTipPwr));
                cDataValue.m_nXTipIndex_List.Add(nXTipIndex);

                cDataValue.m_nXRingPwr_List.Add(new List<int>(nXRingPwr));
                cDataValue.m_nXRingIndex_List.Add(nXRingIndex);

                cDataValue.m_nYTipPwr_List.Add(new List<int>(nYTipPwr));
                cDataValue.m_nYTipIndex_List.Add(nYTipIndex);

                cDataValue.m_nYRingPwr_List.Add(new List<int>(nYRingPwr));
                cDataValue.m_nYRingIndex_List.Add(nYRingIndex);

                if (bHorizontalStatusFlag == true)
                {
                    m_cDataValue_Horizontal_List[nFileIndex].m_nXTipPwr_List = cDataValue.m_nXTipPwr_List;
                    m_cDataValue_Horizontal_List[nFileIndex].m_nXTipIndex_List = cDataValue.m_nXTipIndex_List;

                    m_cDataValue_Horizontal_List[nFileIndex].m_nXRingPwr_List = cDataValue.m_nXRingPwr_List;
                    m_cDataValue_Horizontal_List[nFileIndex].m_nXRingIndex_List = cDataValue.m_nXRingIndex_List;

                    m_cDataValue_Horizontal_List[nFileIndex].m_nYTipPwr_List = cDataValue.m_nYTipPwr_List;
                    m_cDataValue_Horizontal_List[nFileIndex].m_nYTipIndex_List = cDataValue.m_nYTipIndex_List;

                    m_cDataValue_Horizontal_List[nFileIndex].m_nYRingPwr_List = cDataValue.m_nYRingPwr_List;
                    m_cDataValue_Horizontal_List[nFileIndex].m_nYRingIndex_List = cDataValue.m_nYRingIndex_List;
                }
                else
                {
                    m_cDataValue_Vertical_List[nFileIndex].m_nXTipPwr_List = cDataValue.m_nXTipPwr_List;
                    m_cDataValue_Vertical_List[nFileIndex].m_nXTipIndex_List = cDataValue.m_nXTipIndex_List;

                    m_cDataValue_Vertical_List[nFileIndex].m_nXRingPwr_List = cDataValue.m_nXRingPwr_List;
                    m_cDataValue_Vertical_List[nFileIndex].m_nXRingIndex_List = cDataValue.m_nXRingIndex_List;

                    m_cDataValue_Vertical_List[nFileIndex].m_nYTipPwr_List = cDataValue.m_nYTipPwr_List;
                    m_cDataValue_Vertical_List[nFileIndex].m_nYTipIndex_List = cDataValue.m_nYTipIndex_List;

                    m_cDataValue_Vertical_List[nFileIndex].m_nYRingPwr_List = cDataValue.m_nYRingPwr_List;
                    m_cDataValue_Vertical_List[nFileIndex].m_nYRingIndex_List = cDataValue.m_nYRingIndex_List;
                }
            }
        }

        private void GetRegionData(SlantDataValue cOriginalDataValue, List<List<byte>> byteData_List, int nFileIndex)
        {
            int nDataLength = byteData_List.Count;
            SlantDataValue cDataValue = new SlantDataValue();
            cDataValue = cOriginalDataValue;

            for (int nIndex = 0; nIndex < nDataLength; nIndex++)
            {
                int[] nRXTipPwr = new int[m_nPowerCount];
                int[] nRXRingPwr = new int[m_nPowerCount];
                int[] nTXTipPwr = new int[m_nPowerCount];
                int[] nTXRingPwr = new int[m_nPowerCount];

                for (int mIndex = 0; mIndex < 5; mIndex++)
                {
                    nRXTipPwr[mIndex] = (byteData_List[nIndex][m_nXTipPwrStartByte + 2 * mIndex - 1] * 256 + byteData_List[nIndex][m_nXTipPwrStartByte + 2 * mIndex]);
                    nRXRingPwr[mIndex] = (byteData_List[nIndex][m_nXRingPwrStartByte + 2 * mIndex - 1] * 256 + byteData_List[nIndex][m_nXRingPwrStartByte + 2 * mIndex]);
                    nTXTipPwr[mIndex] = (byteData_List[nIndex][m_nYTipPwrStartByte + 2 * mIndex - 1] * 256 + byteData_List[nIndex][m_nYTipPwrStartByte + 2 * mIndex]);
                    nTXRingPwr[mIndex] = (byteData_List[nIndex][m_nYRingPwrStartByte + 2 * mIndex - 1] * 256 + byteData_List[nIndex][m_nYRingPwrStartByte + 2 * mIndex]);
                }

                int nRXTipIndex = byteData_List[nIndex][m_nXTipIndexByte - 1];
                int nRXRingIndex = byteData_List[nIndex][m_nXRingIndexByte - 1];
                int nTXTipIndex = byteData_List[nIndex][m_nYTipIndexByte - 1];
                int nTXRingIndex = byteData_List[nIndex][m_nYRingIndexByte - 1];

                cDataValue.m_nRXTipPwr_List.Add(new List<int>(nRXTipPwr));
                cDataValue.m_nRXTipIndex_List.Add(nRXTipIndex);
                cDataValue.m_nRXRingPwr_List.Add(new List<int>(nRXRingPwr));
                cDataValue.m_nRXRingIndex_List.Add(nRXRingIndex);

                cDataValue.m_nTXTipPwr_List.Add(new List<int>(nTXTipPwr));
                cDataValue.m_nTXTipIndex_List.Add(nTXTipIndex);
                cDataValue.m_nTXRingPwr_List.Add(new List<int>(nTXRingPwr));
                cDataValue.m_nTXRingIndex_List.Add(nTXRingIndex);

                m_cDataValue_Slant_List[nFileIndex].m_nRXTipPwr_List = cDataValue.m_nRXTipPwr_List;
                m_cDataValue_Slant_List[nFileIndex].m_nRXTipIndex_List = cDataValue.m_nRXTipIndex_List;
                m_cDataValue_Slant_List[nFileIndex].m_nRXRingPwr_List = cDataValue.m_nRXRingPwr_List;
                m_cDataValue_Slant_List[nFileIndex].m_nRXRingIndex_List = cDataValue.m_nRXRingIndex_List;

                m_cDataValue_Slant_List[nFileIndex].m_nTXTipPwr_List = cDataValue.m_nTXTipPwr_List;
                m_cDataValue_Slant_List[nFileIndex].m_nTXTipIndex_List = cDataValue.m_nTXTipIndex_List;
                m_cDataValue_Slant_List[nFileIndex].m_nTXRingPwr_List = cDataValue.m_nTXRingPwr_List;
                m_cDataValue_Slant_List[nFileIndex].m_nTXRingIndex_List = cDataValue.m_nTXRingIndex_List;
            }
        }

        private void GetMajorData(DataValue cOriginalDataValue, int nFileIndex, bool bHorizontalStatusFlag = true)
        {
            DataValue cDataValue = new DataValue();
            cDataValue = cOriginalDataValue;

            if (bHorizontalStatusFlag == true)
            {
                int nDataLength = cDataValue.m_nXTipIndex_List.Count;

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    bool bUsefulIndexFlag = false;

                    for (int nValidIndex = 0; nValidIndex < cDataValue.m_nValidTipIndex_List.Count; nValidIndex++)
                    {
                        if (cDataValue.m_nXTipIndex_List[nDataIndex] == cDataValue.m_nValidTipIndex_List[nValidIndex])
                        {
                            bUsefulIndexFlag = true;
                            break;
                        }
                    }

                    if (bUsefulIndexFlag == true)
                    {
                        cDataValue.m_nMajorTipIndex_List.Add(cDataValue.m_nXTipIndex_List[nDataIndex]);
                        cDataValue.m_nMajorTipPwr_List.Add(cDataValue.m_nXTipPwr_List[nDataIndex]);

                        double dTmp_TipDev = ComputeDeviationValue(cDataValue.m_nXTipPwr_List[nDataIndex]);
                        cDataValue.m_dMajorTipDev_List.Add(dTmp_TipDev);

                        cDataValue.m_nMajorRingIndex_List.Add(cDataValue.m_nXRingIndex_List[nDataIndex]);
                        cDataValue.m_nMajorRingPwr_List.Add(cDataValue.m_nXRingPwr_List[nDataIndex]);

                        double dTmp_RingDev = ComputeDeviationValue(cDataValue.m_nXRingPwr_List[nDataIndex]);
                        cDataValue.m_dMajorRingDev_List.Add(dTmp_RingDev);

                        double dTmp_CoordDiff = ComputeCoordDiffValue(cDataValue.m_nXTipIndex_List[nDataIndex], cDataValue.m_nXRingIndex_List[nDataIndex], dTmp_TipDev, dTmp_RingDev);
                        cDataValue.m_dMajorCoordDiff_List.Add(dTmp_CoordDiff);
                    }
                }

                m_cDataValue_Horizontal_List[nFileIndex] = cDataValue;
            }
            else
            {
                int nDataLength = cDataValue.m_nYTipIndex_List.Count;

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    bool bUsefulIndexFlag = false;

                    for (int nValidIndex = 0; nValidIndex < cDataValue.m_nValidTipIndex_List.Count; nValidIndex++)
                    {
                        if (cDataValue.m_nYTipIndex_List[nDataIndex] == cDataValue.m_nValidTipIndex_List[nValidIndex])
                        {
                            bUsefulIndexFlag = true;
                            break;
                        }
                    }

                    if (bUsefulIndexFlag == true)
                    {
                        cDataValue.m_nMajorTipIndex_List.Add(cDataValue.m_nYTipIndex_List[nDataIndex]);
                        cDataValue.m_nMajorTipPwr_List.Add(cDataValue.m_nYTipPwr_List[nDataIndex]);

                        double dTipDev = ComputeDeviationValue(cDataValue.m_nYTipPwr_List[nDataIndex]);
                        cDataValue.m_dMajorTipDev_List.Add(dTipDev);

                        cDataValue.m_nMajorRingIndex_List.Add(cDataValue.m_nYRingIndex_List[nDataIndex]);
                        cDataValue.m_nMajorRingPwr_List.Add(cDataValue.m_nYRingPwr_List[nDataIndex]);

                        double dRingDev = ComputeDeviationValue(cDataValue.m_nYRingPwr_List[nDataIndex]);
                        cDataValue.m_dMajorRingDev_List.Add(dRingDev);

                        double dCoordDiff = ComputeCoordDiffValue(cDataValue.m_nYTipIndex_List[nDataIndex], cDataValue.m_nYRingIndex_List[nDataIndex], dTipDev, dRingDev);
                        cDataValue.m_dMajorCoordDiff_List.Add(dCoordDiff);
                    }
                }

                m_cDataValue_Vertical_List[nFileIndex] = cDataValue;
            }
        }

        private void GetSingleCoordDiff(DataValue cOriginalDataValue, int nFileIndex, bool bHorizontalStatusFlag = true)
        {
            DataValue cDataValue = new DataValue();
            cDataValue = cOriginalDataValue;

            for (int nIndex = 0; nIndex < cDataValue.m_nValidTipIndex_List.Count; nIndex++)
                cDataValue.m_dSingleCoordDiff_List.Add(new List<double>());

            for (int nMajorIndex = 0; nMajorIndex < cDataValue.m_nMajorTipIndex_List.Count; nMajorIndex++)
            {
                for (int nValidIndex = 0; nValidIndex < cDataValue.m_nValidTipIndex_List.Count; nValidIndex++)
                {
                    if (cDataValue.m_nMajorTipIndex_List[nMajorIndex] == cDataValue.m_nValidTipIndex_List[nValidIndex])
                    {
                        cDataValue.m_dSingleCoordDiff_List[nValidIndex].Add(cDataValue.m_dMajorCoordDiff_List[nMajorIndex]);
                        break;
                    }
                }
            }

            if (bHorizontalStatusFlag == true)
                m_cDataValue_Horizontal_List[nFileIndex] = cDataValue;
            else
                m_cDataValue_Vertical_List[nFileIndex] = cDataValue;
        }

        private void GetFinalCoordDiff(DataValue cOriginalDataValue, int nFileIndex, bool bHorizontalStatusFlag = true)
        {
            DataValue cDataValue = new DataValue();
            cDataValue = cOriginalDataValue;

            for (int nIndex = cDataValue.m_nValidTipIndex_List.Count - 1; nIndex >= 0; nIndex--)
            {
                if (cDataValue.m_dSingleCoordDiff_List[nIndex].Count < m_nPolyFitOrder + 1)
                {
                    cDataValue.m_nValidTipIndex_List.RemoveAt(nIndex);
                    cDataValue.m_dSingleCoordDiff_List.RemoveAt(nIndex);
                }
            }

            if (bHorizontalStatusFlag == true)
                m_cDataValue_Horizontal_List[nFileIndex] = cDataValue;
            else
                m_cDataValue_Vertical_List[nFileIndex] = cDataValue;
        }

        private void GetSingleCurve(DataValue cOriginalDataValue, int nFileIndex, bool bHorizontalStatusFlag = true)
        {
            DataValue cDataValue = new DataValue();
            cDataValue = cOriginalDataValue;

            for (int nIndex = 0; nIndex < cDataValue.m_nValidTipIndex_List.Count; nIndex++)
                cDataValue.m_dSingleCurve_List.Add(new List<double>());

            for (int nIndex = 0; nIndex < cDataValue.m_nValidTipIndex_List.Count; nIndex++)
            {
                double[] dCoordDiff_Array = cDataValue.m_dSingleCoordDiff_List[nIndex].ToArray();
                double[] dIncreaseOrder_Array = ComputeIncreaseOrderArray(dCoordDiff_Array);

                double[] dCoeff_Array = ComputePolyFit(dIncreaseOrder_Array, dCoordDiff_Array, m_nPolyFitOrder);

                double[] dCurve_Array = ComputeCurveArray(dIncreaseOrder_Array, dCoeff_Array);

                cDataValue.m_dSingleCurve_List[nIndex] = new List<double>(dCurve_Array);
            }

            if (bHorizontalStatusFlag == true)
                m_cDataValue_Horizontal_List[nFileIndex] = cDataValue;
            else
                m_cDataValue_Vertical_List[nFileIndex] = cDataValue;
        }

        private void GetStraightUsefulData(SlantDataValue cDataValue, int nLeftBoundary, int nRightBoundary, bool bRXTraceTypeFlag = true)
        {
            List<int> nTipIndex_List = null;
            List<List<int>> nTipPwr_List = null;
            List<int> nRingIndex_List = null;
            List<List<int>> nRingPwr_List = null;

            if (bRXTraceTypeFlag == true)
            {
                nTipIndex_List = new List<int>(cDataValue.m_nRXTipIndex_List);
                nTipPwr_List = new List<List<int>>(cDataValue.m_nRXTipPwr_List);
                nRingIndex_List = new List<int>(cDataValue.m_nRXRingIndex_List);
                nRingPwr_List = new List<List<int>>(cDataValue.m_nRXRingPwr_List);
            }
            else
            {
                nTipIndex_List = new List<int>(cDataValue.m_nTXTipIndex_List);
                nTipPwr_List = new List<List<int>>(cDataValue.m_nTXTipPwr_List);
                nRingIndex_List = new List<int>(cDataValue.m_nTXRingIndex_List);
                nRingPwr_List = new List<List<int>>(cDataValue.m_nTXRingPwr_List);
            }

            for (int nIndex = 0; nIndex < nTipIndex_List.Count; nIndex++)
            {
                int nTipIndexData = nTipIndex_List[nIndex];
                int nRingIndexData = nRingIndex_List[nIndex];

                int nFirstMaxValue = FindMaxIndexValue(nTipPwr_List[nIndex], 1);
                int nSecondMaxValue = FindMaxIndexValue(nTipPwr_List[nIndex], 2);
                int nThirdMaxValue = FindMaxIndexValue(nTipPwr_List[nIndex], 3);

                int nRingMaxValue = FindMaxIndexValue(nRingPwr_List[nIndex], 1);

                if (bRXTraceTypeFlag == true)
                {
                    m_nOriginalTraceIndex_RX_List.Add(nTipIndexData + 1);
                    m_nOriginalFirstMax_RX_List.Add(nFirstMaxValue);
                    m_nOriginalSecondMax_RX_List.Add(nSecondMaxValue);
                    m_nOriginalThirdMax_RX_List.Add(nThirdMaxValue);

                    m_nOriginalRingTraceIndex_RX_List.Add(nRingIndexData + 1);
                    m_nOriginalRingMax_RX_List.Add(nRingMaxValue);
                }
                else
                {
                    m_nOriginalTraceIndex_TX_List.Add(nTipIndexData + 1);
                    m_nOriginalFirstMax_TX_List.Add(nFirstMaxValue);
                    m_nOriginalSecondMax_TX_List.Add(nSecondMaxValue);
                    m_nOriginalThirdMax_TX_List.Add(nThirdMaxValue);

                    m_nOriginalRingTraceIndex_TX_List.Add(nRingIndexData + 1);
                    m_nOriginalRingMax_TX_List.Add(nRingMaxValue);
                }

                if (nTipIndexData <= nLeftBoundary || nTipIndexData >= nRightBoundary)
                    continue;

                if (bRXTraceTypeFlag == true)
                {
                    m_nTraceIndex_RX_List.Add(nTipIndexData + 1);
                    m_nFirstMax_RX_List.Add(nFirstMaxValue);
                    m_nSecondMax_RX_List.Add(nSecondMaxValue);
                    m_nThirdMax_RX_List.Add(nThirdMaxValue);

                    m_nRingTraceIndex_RX_List.Add(nRingIndexData + 1);
                    m_nRingMax_RX_List.Add(nRingMaxValue);
                }
                else
                {
                    m_nTraceIndex_TX_List.Add(nTipIndexData + 1);
                    m_nFirstMax_TX_List.Add(nFirstMaxValue);
                    m_nSecondMax_TX_List.Add(nSecondMaxValue);
                    m_nThirdMax_TX_List.Add(nThirdMaxValue);

                    m_nRingTraceIndex_TX_List.Add(nRingIndexData + 1);
                    m_nRingMax_TX_List.Add(nRingMaxValue);
                }
            }
        }

        private int[] ConvertListDataToArray(List<int> nData_List)
        {
            int[] nData_Array = new int[nData_List.Count];

            for (int nIndex = 0; nIndex < nData_List.Count; nIndex++)
                nData_Array[nIndex] = nData_List[nIndex];

            return nData_Array;
        }

        private void ComputeIndexRMSE(DataValue cOriginalDataValue, int nFileIndex, bool bHorizontalStatusFlag = true)
        {
            DataValue cDataValue = new DataValue();
            cDataValue = cOriginalDataValue;

            for (int nIndex = 0; nIndex < cDataValue.m_nValidTipIndex_List.Count; nIndex++)
                cDataValue.m_dIndexRMSE_List.Add(new double());

            for (int nIndex = 0; nIndex < cDataValue.m_nValidTipIndex_List.Count; nIndex++)
                cDataValue.m_dIndexRMSE_List[nIndex] = ComputeRMSE(cDataValue.m_dSingleCoordDiff_List[nIndex], cDataValue.m_dSingleCurve_List[nIndex]);

            if (bHorizontalStatusFlag == true)
                m_cDataValue_Horizontal_List[nFileIndex] = cDataValue;
            else
                m_cDataValue_Vertical_List[nFileIndex] = cDataValue;
        }

        private double ComputeMeanRMSE(List<double> dIndexRMSE)
        { 
            double dValue = 0.0;

            for (int nIndex = 0; nIndex < dIndexRMSE.Count; nIndex++)
                dValue += dIndexRMSE[nIndex];

            dValue = Math.Round(dValue / dIndexRMSE.Count, 3, MidpointRounding.AwayFromZero);

            return dValue;
        }

        private void WriteStraightDataFile(string sTitleName, int[] nTipIndexData_Array, int[] nFirstMaxData_Array, int[] nSecondMaxData_Array, int[] nThirdMaxData_Array, int[] nRingIndexData_Array, int[] nRingMaxData_Array, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                sw.WriteLine("Tip TraceIndex,FirstMax,SecondMax,ThirdMax,,,Ring TraceIndex,Max");

                int nDataLength = nFirstMaxData_Array.Length;

                for (int nIndex = 0; nIndex < nDataLength; nIndex++)
                {
                    string sText = "";

                    sText += string.Format("{0},", nTipIndexData_Array[nIndex]);
                    sText += string.Format("{0},", nFirstMaxData_Array[nIndex]);
                    sText += string.Format("{0},", nSecondMaxData_Array[nIndex]);
                    sText += string.Format("{0},", nThirdMaxData_Array[nIndex]);
                    sText += ",,";
                    sText += string.Format("{0},", nRingIndexData_Array[nIndex]);
                    sText += string.Format("{0}", nRingMaxData_Array[nIndex]);
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

        private void WriteComputeDataFile(string sTitleName, DataValue cDataValue, string Write_Into_Name)
        {
            FileStream fs = new FileStream(Write_Into_Name, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                sw.Write("TipIndex,,");

                for (int nIndex = 0; nIndex < m_nPowerCount; nIndex++)
                {
                    sw.Write(string.Format("{0},", nIndex + 1));

                    if (nIndex == m_nPowerCount - 1)
                        sw.Write(",");
                }

                sw.Write("TipDev,,");
                sw.Write("RingIndex,,");

                for (int nIndex = 0; nIndex < m_nPowerCount; nIndex++)
                {
                    sw.Write(string.Format("{0},", nIndex + 1));

                    if (nIndex == m_nPowerCount - 1)
                        sw.Write(",");
                }

                sw.Write("RingDev,,");
                sw.WriteLine("CoordDiff");

                int nDataLength = cDataValue.m_nMajorTipIndex_List.Count;

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    string sText = "";

                    sText += string.Format("{0},,", cDataValue.m_nMajorTipIndex_List[nDataIndex]);

                    for (int nPowerIndex = 0; nPowerIndex < m_nPowerCount; nPowerIndex++)
                    {
                        sText += string.Format("{0},", cDataValue.m_nMajorTipPwr_List[nDataIndex][nPowerIndex]);

                        if (nPowerIndex == m_nPowerCount - 1)
                            sText += ",";
                    }

                    sText += string.Format("{0},,", Convert.ToDouble(cDataValue.m_dMajorTipDev_List[nDataIndex]));
                    sText += string.Format("{0},,", cDataValue.m_nMajorRingIndex_List[nDataIndex]);

                    for (int nPowerIndex = 0; nPowerIndex < m_nPowerCount; nPowerIndex++)
                    {
                        sText += string.Format("{0},", cDataValue.m_nMajorRingPwr_List[nDataIndex][nPowerIndex]);

                        if (nPowerIndex == m_nPowerCount - 1)
                            sText += ",";
                    }

                    sText += string.Format("{0},,", Convert.ToDouble(cDataValue.m_dMajorRingDev_List[nDataIndex]));
                    sText += string.Format("{0}", Convert.ToDouble(cDataValue.m_dMajorCoordDiff_List[nDataIndex]));

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

        private void WriteSlantComputeDataFile(string sTitleName, SlantDataValue cDataValue, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                sw.Write("RX TipIndex,,");

                for (int nIndex = 0; nIndex < m_nPowerCount; nIndex++)
                    sw.Write(string.Format("{0},", nIndex + 1));

                sw.Write(",,");
                sw.Write("RX RingIndex,,");

                for (int nIndex = 0; nIndex < m_nPowerCount; nIndex++)
                    sw.Write(string.Format("{0},", nIndex + 1));

                sw.Write(",,");
                sw.Write("TX TipIndex,,");

                for (int nIndex = 0; nIndex < m_nPowerCount; nIndex++)
                    sw.Write(string.Format("{0},", nIndex + 1));

                sw.Write(",,");
                sw.Write("TX RingIndex,,");

                for (int nIndex = 0; nIndex < m_nPowerCount; nIndex++)
                {
                    if (nIndex == m_nPowerCount - 1)
                        sw.Write(string.Format("{0}", nIndex + 1));
                    else
                        sw.Write(string.Format("{0},", nIndex + 1));
                }

                sw.WriteLine();

                int nDataLength = cDataValue.m_nRXTipIndex_List.Count;

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    string sText = "";

                    sText += string.Format("{0},,", cDataValue.m_nRXTipIndex_List[nDataIndex] + 1);

                    for (int nPowerIndex = 0; nPowerIndex < m_nPowerCount; nPowerIndex++)
                        sText += string.Format("{0},", cDataValue.m_nRXTipPwr_List[nDataIndex][nPowerIndex]);

                    sText += ",,";
                    sText += string.Format("{0},,", cDataValue.m_nRXRingIndex_List[nDataIndex] + 1);

                    for (int nPowerIndex = 0; nPowerIndex < m_nPowerCount; nPowerIndex++)
                        sText += string.Format("{0},", cDataValue.m_nRXRingPwr_List[nDataIndex][nPowerIndex]);

                    sText += ",,";
                    sText += string.Format("{0},,", cDataValue.m_nTXTipIndex_List[nDataIndex] + 1);

                    for (int nPowerIndex = 0; nPowerIndex < m_nPowerCount; nPowerIndex++)
                        sText += string.Format("{0},", cDataValue.m_nTXTipPwr_List[nDataIndex][nPowerIndex]);

                    sText += ",,";
                    sText += string.Format("{0},,", cDataValue.m_nTXRingIndex_List[nDataIndex] + 1);

                    for (int nPowerIndex = 0; nPowerIndex < m_nPowerCount; nPowerIndex++)
                    {
                        if (nPowerIndex == m_nPowerCount - 1)
                            sText += string.Format("{0}", cDataValue.m_nTXRingPwr_List[nDataIndex][nPowerIndex]);
                        else
                            sText += string.Format("{0},", cDataValue.m_nTXRingPwr_List[nDataIndex][nPowerIndex]);
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

        private void WriteSingleCoordDiffDataFile(string sTitleName, DataValue cDataValue, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                int nMaxLength = 0;

                for (int nIndex = 0; nIndex < cDataValue.m_nValidTipIndex_List.Count; nIndex++)
                {
                    if (nIndex != cDataValue.m_nValidTipIndex_List.Count - 1)
                        sw.Write(string.Format("{0},", cDataValue.m_nValidTipIndex_List[nIndex]));
                    else
                        sw.WriteLine(string.Format("{0}", cDataValue.m_nValidTipIndex_List[nIndex]));

                    if (cDataValue.m_dSingleCoordDiff_List[nIndex].Count > nMaxLength)
                        nMaxLength = cDataValue.m_dSingleCoordDiff_List[nIndex].Count;
                }

                for (int nDataIndex = 0; nDataIndex < nMaxLength; nDataIndex++)
                {
                    string sText = "";

                    for (int nPowerIndex = 0; nPowerIndex < cDataValue.m_nValidTipIndex_List.Count; nPowerIndex++)
                    {
                        if (nDataIndex + 1 <= cDataValue.m_dSingleCoordDiff_List[nPowerIndex].Count)
                        {
                            if (nPowerIndex != cDataValue.m_nValidTipIndex_List.Count - 1)
                                sText += string.Format("{0},", Convert.ToDouble(cDataValue.m_dSingleCoordDiff_List[nPowerIndex][nDataIndex]));
                            else
                                sText += string.Format("{0}", Convert.ToDouble(cDataValue.m_dSingleCoordDiff_List[nPowerIndex][nDataIndex]));
                        }
                        else
                        {
                            if (nPowerIndex != cDataValue.m_nValidTipIndex_List.Count - 1)
                                sText += ",";
                        }
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

        private void WritePolyFitDataFile(string sTitleName, DataValue cDataValue, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                int nMaxLength = 0;

                string sLine_1 = "";
                string sLine_2 = "";
                string sLine_3 = "";

                for (int nIndex = 0; nIndex < cDataValue.m_nValidTipIndex_List.Count; nIndex++)
                {
                    if (nIndex != cDataValue.m_nValidTipIndex_List.Count - 1)
                    {
                        sLine_1 += string.Format("{0},,,", cDataValue.m_nValidTipIndex_List[nIndex]);
                        sLine_2 += "RMSE,,,";
                        sLine_3 += string.Format("{0},,,", Convert.ToDouble(cDataValue.m_dIndexRMSE_List[nIndex]));
                    }
                    else
                    {
                        sLine_1 += string.Format("{0}", cDataValue.m_nValidTipIndex_List[nIndex]);
                        sLine_2 += "RMSE";
                        sLine_3 += string.Format("{0}", Convert.ToDouble(cDataValue.m_dIndexRMSE_List[nIndex]));
                    }
                }

                sw.WriteLine(sLine_1);
                sw.WriteLine(sLine_2);
                sw.WriteLine(sLine_3);

                for (int nIndex = 0; nIndex < cDataValue.m_nValidTipIndex_List.Count; nIndex++)
                {
                    if (nIndex != cDataValue.m_nValidTipIndex_List.Count - 1)
                        sw.Write("CoordDiff,Curve,,");
                    else
                        sw.WriteLine("CoordDiff,Curve");

                    if (cDataValue.m_dSingleCoordDiff_List[nIndex].Count > nMaxLength)
                        nMaxLength = cDataValue.m_dSingleCoordDiff_List[nIndex].Count;
                }

                for (int nDataIndex = 0; nDataIndex < nMaxLength; nDataIndex++)
                {
                    string sText = "";

                    for (int nValidIndex = 0; nValidIndex < cDataValue.m_nValidTipIndex_List.Count; nValidIndex++)
                    {
                        if (nDataIndex + 1 <= cDataValue.m_dSingleCoordDiff_List[nValidIndex].Count)
                            sText += string.Format("{0},{1},,", Convert.ToDouble(cDataValue.m_dSingleCoordDiff_List[nValidIndex][nDataIndex]), Convert.ToDouble(cDataValue.m_dSingleCurve_List[nValidIndex][nDataIndex]));
                        else
                        {
                            if (nValidIndex != cDataValue.m_nValidTipIndex_List.Count - 1)
                                sText += ",,,";
                        }
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
            string[] sValueTypeName_Array = new string[29] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sFlowStep, 
                SpecificText.m_sSettingPH1, 
                SpecificText.m_sSettingPH2, 
                SpecificText.m_sReadPH1, 
                SpecificText.m_sReadPH2, 
                SpecificText.m_sFrequency, 
                SpecificText.m_sRMSE_H, 
                SpecificText.m_sRMSE_V, 
                SpecificText.m_sNormalizeRMSE_H, 
                SpecificText.m_sNormalizeRMSE_V, 
                SpecificText.m_sRXSecondMaxMean, 
                SpecificText.m_sRXThirdMaxMean, 
                SpecificText.m_sRXContactTH, 
                SpecificText.m_sRXHoverTH, 
                SpecificText.m_sRXRingMean, 
                SpecificText.m_sRXRingSTD, 
                SpecificText.m_sRXRingMeanMinus1STD, 
                SpecificText.m_sRXRingMeanMinus2STD,
                SpecificText.m_sTXSecondMaxMean, 
                SpecificText.m_sTXThirdMaxMean, 
                SpecificText.m_sTXContactTH, 
                SpecificText.m_sTXHoverTH, 
                SpecificText.m_sTXRingMean, 
                SpecificText.m_sTXRingSTD, 
                SpecificText.m_sTXRingMeanMinus1STD, 
                SpecificText.m_sTXRingMeanMinus2STD,
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
                            sw.Write(string.Format("{0},", m_cDataValue_Horizontal_List[nDataIndex].m_dRMSE.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Vertical_List[nDataIndex].m_dRMSE.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Horizontal_List[nDataIndex].m_dNormalizeRMSE.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Vertical_List[nDataIndex].m_dNormalizeRMSE.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_dRXSecondMaxMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_dRXThirdMaxMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_nRXContactTH.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_nRXHoverTH.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_dRXRingMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_dRXRingSTD.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_nRXRingMeanMinus1STD.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_nRXRingMeanMinus2STD.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_dTXSecondMaxMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_dTXThirdMaxMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_nTXContactTH.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_nTXHoverTH.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_dTXRingMean.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_dTXRingSTD.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_nTXRingMeanMinus1STD.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_Slant_List[nDataIndex].m_nTXRingMeanMinus2STD.ToString()));

                            string sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sErrorMessage);

                            if (ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode != "")
                                sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sRecordErrorMessage);

                            sw.WriteLine(sErrorMessage);

                            break;
                        }
                    }
                }

                if (m_eSubStep == SubTuningStep.TILTTUNING_BHF)
                {

                    string[] sRankTypeName_Array = new string[13] 
                    { 
                        SpecificText.m_sRanking, 
                        SpecificText.m_sFileName, 
                        SpecificText.m_sFrequency, 
                        SpecificText.m_sPH1, 
                        SpecificText.m_sPH2, 
                        SpecificText.m_sPTHF_H, 
                        SpecificText.m_sPTHF_V, 
                        SpecificText.m_sBHF_H, 
                        SpecificText.m_sBHF_V, 
                        SpecificText.m_sTotalScore, 
                        SpecificText.m_sResult, 
                        SpecificText.m_sPTHFExceptionMessage, 
                        SpecificText.m_sBHFExceptionMessage
                    };

                    sw.WriteLine();
                    sw.WriteLine("Ranking Data Information List");

                    for (int nColumnIndex = 0; nColumnIndex < sRankTypeName_Array.Length; nColumnIndex++)
                    {
                        sw.Write(sRankTypeName_Array[nColumnIndex]);

                        if (nColumnIndex == sRankTypeName_Array.Length - 1)
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
                                string sPTHFNormalizeRMSE_Horizontal = "N/A";
                                string sPTHFNormalizeRMSE_Vertical = "N/A";
                                string sBHFNormalizeRMSE_Horizontal = "N/A";
                                string sBHFNormalizeRMSE_Vertical = "N/A";
                                string sTotalScore = "N/A";

                                if (m_cDataValue_Horizontal_List[nDataIndex].m_dPTHFNormalizeRMSE >= 0)
                                    sPTHFNormalizeRMSE_Horizontal = m_cDataValue_Horizontal_List[nDataIndex].m_dPTHFNormalizeRMSE.ToString();

                                if (m_cDataValue_Vertical_List[nDataIndex].m_dPTHFNormalizeRMSE >= 0)
                                    sPTHFNormalizeRMSE_Vertical = m_cDataValue_Vertical_List[nDataIndex].m_dPTHFNormalizeRMSE.ToString();

                                if (m_cDataValue_Horizontal_List[nDataIndex].m_dNormalizeRMSE >= 0)
                                    sBHFNormalizeRMSE_Horizontal = m_cDataValue_Horizontal_List[nDataIndex].m_dNormalizeRMSE.ToString();

                                if (m_cDataValue_Vertical_List[nDataIndex].m_dNormalizeRMSE >= 0)
                                    sBHFNormalizeRMSE_Vertical = m_cDataValue_Vertical_List[nDataIndex].m_dNormalizeRMSE.ToString();

                                if (m_cBHFDataResult_List[nDataIndex].m_dTotalScore >= 0)
                                    sTotalScore = m_cBHFDataResult_List[nDataIndex].m_dTotalScore.ToString();

                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                                sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                                sw.Write(string.Format("{0},", sPTHFNormalizeRMSE_Horizontal));
                                sw.Write(string.Format("{0},", sPTHFNormalizeRMSE_Vertical));
                                sw.Write(string.Format("{0},", sBHFNormalizeRMSE_Horizontal));
                                sw.Write(string.Format("{0},", sBHFNormalizeRMSE_Vertical));
                                sw.Write(string.Format("{0},", sTotalScore));
                                sw.Write(string.Format("{0},", m_cBHFDataResult_List[nDataIndex].m_sResult));
                                sw.Write(string.Format("{0},", m_cBHFDataResult_List[nDataIndex].m_sPTHFErrorMessage));

                                string sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sErrorMessage);

                                if (ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode != "")
                                    sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sRecordErrorMessage);

                                sw.WriteLine(sErrorMessage);

                                break;
                            }
                        }
                    }

                    string[] sFWParameterTypeName_Array = new string[15] 
                    { 
                        SpecificText.m_sRanking, 
                        SpecificText.m_sFileName, 
                        SpecificText.m_sFrequency, 
                        SpecificText.m_sPH1,
                        SpecificText.m_sPH2, 
                        SpecificText.m_scActivePen_PTHF_Contact_TH_Rx, 
                        SpecificText.m_scActivePen_PTHF_Contact_TH_Tx, 
                        SpecificText.m_scActivePen_PTHF_Hover_TH_Rx, 
                        SpecificText.m_scActivePen_PTHF_Hover_TH_Tx,
                        SpecificText.m_scActivePen_BHF_Contact_TH_Rx, 
                        SpecificText.m_scActivePen_BHF_Contact_TH_Tx, 
                        SpecificText.m_scActivePen_BHF_Hover_TH_Rx, 
                        SpecificText.m_scActivePen_BHF_Hover_TH_Tx,
                        SpecificText.m_scPenTiltPwrTHTXHB, 
                        SpecificText.m_scPenTiltPwrTHTXLB 
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

                    foreach (int nRankValue in nRankSort_List)
                    {
                        for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                        {
                            if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                            {
                                string sPTHFCTH_Rx = m_cDataValue_Slant_List[nDataIndex].m_nPTHF_Contact_TH_Rx.ToString();
                                string sPTHFCTH_Tx = m_cDataValue_Slant_List[nDataIndex].m_nPTHF_Contact_TH_Tx.ToString();
                                string sPTHFHTH_Rx = m_cDataValue_Slant_List[nDataIndex].m_nPTHF_Hover_TH_Rx.ToString();
                                string sPTHFHTH_Tx = m_cDataValue_Slant_List[nDataIndex].m_nPTHF_Hover_TH_Tx.ToString();
                                string sBHFCTH_Rx = m_cDataValue_Slant_List[nDataIndex].m_nRXContactTH.ToString();
                                string sBHFCTH_Tx = m_cDataValue_Slant_List[nDataIndex].m_nTXContactTH.ToString();
                                string sBHFHTH_Rx = m_cDataValue_Slant_List[nDataIndex].m_nRXHoverTH.ToString();
                                string sBHFHTH_Tx = m_cDataValue_Slant_List[nDataIndex].m_nTXHoverTH.ToString();
                                string sPwrTHTXHB = m_cDataValue_Slant_List[nDataIndex].m_nPwrTHTXHB.ToString();
                                string sPwrTHTXLB = m_cDataValue_Slant_List[nDataIndex].m_nPwrTHTXLB.ToString();

                                if (m_cDataValue_Slant_List[nDataIndex].m_nPTHF_Contact_TH_Rx < 0)
                                    sPTHFCTH_Rx = "N/A";

                                if (m_cDataValue_Slant_List[nDataIndex].m_nPTHF_Contact_TH_Tx < 0)
                                    sPTHFCTH_Tx = "N/A";

                                if (m_cDataValue_Slant_List[nDataIndex].m_nPTHF_Hover_TH_Rx < 0)
                                    sPTHFHTH_Rx = "N/A";

                                if (m_cDataValue_Slant_List[nDataIndex].m_nPTHF_Hover_TH_Tx < 0)
                                    sPTHFHTH_Tx = "N/A";

                                if (m_cDataValue_Slant_List[nDataIndex].m_nRXContactTH < 0)
                                    sBHFCTH_Rx = "N/A";

                                if (m_cDataValue_Slant_List[nDataIndex].m_nTXContactTH < 0)
                                    sBHFCTH_Tx = "N/A";

                                if (m_cDataValue_Slant_List[nDataIndex].m_nRXHoverTH < 0)
                                    sBHFHTH_Rx = "N/A";

                                if (m_cDataValue_Slant_List[nDataIndex].m_nTXHoverTH < 0)
                                    sBHFCTH_Tx = "N/A";

                                if (m_cDataValue_Slant_List[nDataIndex].m_nPwrTHTXHB < 0)
                                    sPwrTHTXHB = "N/A";

                                if (m_cDataValue_Slant_List[nDataIndex].m_nPwrTHTXLB < 0)
                                    sPwrTHTXLB = "N/A";

                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                                sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                                sw.Write(string.Format("{0},", sPTHFCTH_Rx));
                                sw.Write(string.Format("{0},", sPTHFCTH_Tx));
                                sw.Write(string.Format("{0},", sPTHFHTH_Rx));
                                sw.Write(string.Format("{0},", sPTHFHTH_Tx));
                                sw.Write(string.Format("{0},", sBHFCTH_Rx));
                                sw.Write(string.Format("{0},", sBHFCTH_Tx));
                                sw.Write(string.Format("{0},", sBHFHTH_Rx));
                                sw.Write(string.Format("{0},", sBHFHTH_Tx));
                                sw.Write(string.Format("{0},", sPwrTHTXHB));
                                sw.WriteLine(string.Format("{0}", sPwrTHTXLB));
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
                if (m_eSubStep != SubTuningStep.TILTTUNING_BHF)
                {
                    if (Directory.Exists(m_cfrmMain.m_sFlowDirectoryPath) == false)
                        Directory.CreateDirectory(m_cfrmMain.m_sFlowDirectoryPath);

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

        private bool GetPreviousStepValue(List<TiltTuningParameter> cParameter_List, DataInfo cDataInfo, DataValue cDataValue_Horizontal, DataValue cDataValue_Vertical, SlantDataValue cDataValue_Slant, BHFDataResult cBHFDataResult)
        {
            bool bErrorFlag = true;

            for (int nParameterIndex = 0; nParameterIndex < cParameter_List.Count; nParameterIndex++)
            {
                if (cParameter_List[nParameterIndex].m_nPH1 == cDataInfo.m_nReadPH1 &&
                    cParameter_List[nParameterIndex].m_nPH2 == cDataInfo.m_nReadPH2)
                {
                    int nGetParameterFlag = 0;

                    cDataValue_Horizontal.m_dPTHFNormalizeRMSE = cParameter_List[nParameterIndex].m_dPTHFNormRMSE_H;

                    cDataValue_Vertical.m_dPTHFNormalizeRMSE = cParameter_List[nParameterIndex].m_dPTHFNormRMSE_V;

                    if (cParameter_List[nParameterIndex].m_sPreviousErrorMessage != "N/A")
                    {
                        cBHFDataResult.m_sPTHFErrorMessage = cParameter_List[nParameterIndex].m_sPreviousErrorMessage;
                        nGetParameterFlag |= 0x0001;
                    }

                    cDataValue_Slant.m_nPTHF_Contact_TH_Rx = cParameter_List[nParameterIndex].m_nPTHFContact_TH_Rx;
                    cDataValue_Slant.m_nPTHF_Contact_TH_Tx = cParameter_List[nParameterIndex].m_nPTHFContact_TH_Tx;

                    cDataValue_Slant.m_nPTHF_Hover_TH_Rx = cParameter_List[nParameterIndex].m_nPTHFHover_TH_Rx;
                    cDataValue_Slant.m_nPTHF_Hover_TH_Tx = cParameter_List[nParameterIndex].m_nPTHFHover_TH_Tx;

                    cDataValue_Slant.m_dPTHF_RXRingMean = cParameter_List[nParameterIndex].m_dPTHFRXRingMean;
                    cDataValue_Slant.m_nPTHF_RXRingMeanMinus1STD = cParameter_List[nParameterIndex].m_nPTHFRXRMeanMinus1STD;
                    cDataValue_Slant.m_nPTHF_RXRingMeanMinus2STD = cParameter_List[nParameterIndex].m_nPTHFRXRMeanMinus2STD;

                    cDataValue_Slant.m_dPTHF_TXRingMean = cParameter_List[nParameterIndex].m_dPTHFTXRingMean;
                    cDataValue_Slant.m_nPTHF_TXRingMeanMinus1STD = cParameter_List[nParameterIndex].m_nPTHFTXRMeanMinus1STD;
                    cDataValue_Slant.m_nPTHF_TXRingMeanMinus2STD = cParameter_List[nParameterIndex].m_nPTHFTXRMeanMinus2STD;

                    if (nGetParameterFlag == 0x0001)
                        bErrorFlag = false;

                    break;
                }
            }

            return !bErrorFlag;
        }

        private double ComputeDeviationValue(List<int> nData_List)
        {
            double dValue = 0.0;

            for (int nIndex = 0; nIndex < m_nPowerCount; nIndex++)
                dValue += nData_List[nIndex];

            double dDevValue = Math.Round(m_nOSRValue * ((-2 * nData_List[0] - nData_List[1] + nData_List[3] + 2 * nData_List[4]) / dValue), 2, MidpointRounding.AwayFromZero);

            return dDevValue;
        }

        private double ComputeCoordDiffValue(int nTipIndex, int nRingIndex, double dTipDev, double dRingDev)
        {
            double dCoordDiff = Math.Round(m_nOSRValue * (nTipIndex - nRingIndex) + (dTipDev - dRingDev), 2, MidpointRounding.AwayFromZero);

            return dCoordDiff;
        }

        private double[] ComputeIncreaseOrderArray(double[] nData_Array)
        {
            int nLength = nData_Array.Length;
            double[] dIncreaseOrder_Array = new double[nLength];

            for (int nIndex = 0; nIndex < nLength; nIndex++)
                dIncreaseOrder_Array[nIndex] = nIndex + 1;

            return dIncreaseOrder_Array;
        }

        private double[] ComputeCurveArray(double[] dIncreaseOrder_Array, double[] dCoeff_Array)
        {
            double[] dCurve_Array = new double[dIncreaseOrder_Array.Length];

            for (int nIndex = 0; nIndex < dIncreaseOrder_Array.Length; nIndex++)
            {
                double dValue = ComputePolyFitValue(dIncreaseOrder_Array[nIndex], dCoeff_Array);
                dCurve_Array[nIndex] = dValue;
            }

            return dCurve_Array;
        }

        private double ComputePolyFitValue(double dIncreaseOrderValue, double[] dCoeff_Array)
        {
            double dPolyFitValue = 0.0;

            for (int nIndex = 0; nIndex < dCoeff_Array.Length; nIndex++)
                dPolyFitValue += dCoeff_Array[nIndex] * Math.Pow(dIncreaseOrderValue, nIndex);

            return Math.Round(dPolyFitValue, 2, MidpointRounding.AwayFromZero);
        }

        private double[] ComputePolyFit(double[] dXValue, double[] dYValue, int nOrder)
        {
            double[] dCoeff_Array = Fit.Polynomial(dXValue, dYValue, nOrder);

            return dCoeff_Array;
        }

        private double ComputeRMSE(List<double> dCoordDiff_List, List<double> dCurve_List)
        {
            double dSumValue = 0.0;
            double dRMSEValue = 0.0;

            for (int nIndex = 0; nIndex < dCoordDiff_List.Count; nIndex++)
            {
                double dDifferValue = dCoordDiff_List[nIndex] - dCurve_List[nIndex];
                dSumValue += Math.Pow(dDifferValue, 2);
            }

            double dRootValue = Math.Sqrt(dSumValue);

            dRMSEValue = Math.Round(dRootValue / dCoordDiff_List.Count, 3, MidpointRounding.AwayFromZero);

            return dRMSEValue;
        }

        private void ComputeNormalizeRMSE()
        {
            double dRMSESumValue_Horizontal = 0.0;
            double dRMSESumValue_Vertical = 0.0;

            for (int nIndex = 0; nIndex < m_cDataInfo_List.Count; nIndex++)
            {
                if (m_cDataValue_Horizontal_List[nIndex].m_dRMSE >= 0)
                    dRMSESumValue_Horizontal += m_cDataValue_Horizontal_List[nIndex].m_dRMSE;

                if (m_cDataValue_Vertical_List[nIndex].m_dRMSE >= 0)
                    dRMSESumValue_Vertical += m_cDataValue_Vertical_List[nIndex].m_dRMSE;
            }

            for (int nIndex = 0; nIndex < m_cDataInfo_List.Count; nIndex++)
            {
                if (m_cDataValue_Horizontal_List[nIndex].m_dRMSE >= 0)
                    m_cDataValue_Horizontal_List[nIndex].m_dNormalizeRMSE = Math.Round(m_cDataValue_Horizontal_List[nIndex].m_dRMSE / dRMSESumValue_Horizontal, 3, MidpointRounding.AwayFromZero);

                if (m_cDataValue_Vertical_List[nIndex].m_dRMSE >= 0)
                    m_cDataValue_Vertical_List[nIndex].m_dNormalizeRMSE = Math.Round(m_cDataValue_Vertical_List[nIndex].m_dRMSE / dRMSESumValue_Vertical, 3, MidpointRounding.AwayFromZero);
            }
        }

        private void ComputeTotalScore()
        {
            List<int> nDataFlag_List = new List<int>();

            for (int nIndex = 0; nIndex < m_cDataInfo_List.Count; nIndex++)
            {
                double dTotalScore = 0.0;
                int nFlag = 0;

                if (m_cDataValue_Horizontal_List[nIndex].m_dPTHFNormalizeRMSE >= 0)
                {
                    dTotalScore += m_cDataValue_Horizontal_List[nIndex].m_dPTHFNormalizeRMSE;
                    nFlag++;
                }

                if (m_cDataValue_Vertical_List[nIndex].m_dPTHFNormalizeRMSE >= 0)
                {
                    dTotalScore += m_cDataValue_Vertical_List[nIndex].m_dPTHFNormalizeRMSE;
                    nFlag++;
                }

                if (m_cDataValue_Horizontal_List[nIndex].m_dNormalizeRMSE >= 0)
                {
                    dTotalScore += m_cDataValue_Horizontal_List[nIndex].m_dNormalizeRMSE;
                    nFlag++;
                }

                if (m_cDataValue_Vertical_List[nIndex].m_dNormalizeRMSE >= 0)
                {
                    dTotalScore += m_cDataValue_Vertical_List[nIndex].m_dNormalizeRMSE;
                    nFlag++;
                }

                if (nFlag > 0)
                    m_cBHFDataResult_List[nIndex].m_dTotalScore = Math.Round(dTotalScore, 3, MidpointRounding.AwayFromZero);

                if (m_cBHFDataResult_List[nIndex].m_sPTHFErrorMessage == "")
                    nFlag++;

                if (ParamAutoTuning.m_nFlowMethodType == 1)
                {
                    if (m_cDataInfo_List[nIndex].m_sRecordErrorCode == "" && m_cDataInfo_List[nIndex].m_sErrorMessage == "")
                        nFlag++;
                }
                else if (m_cDataInfo_List[nIndex].m_sErrorMessage == "")
                    nFlag++;

                nDataFlag_List.Add(nFlag);
            }

            bool bGetRankFlag = false;
            int nNewRankValue = 0;

            while (bGetRankFlag == true || nNewRankValue == 0)
            {
                double dMinTotalScore = 0.0;

                nNewRankValue++;
                bGetRankFlag = false;

                for (int nIndex = 0; nIndex < m_cDataInfo_List.Count; nIndex++)
                {
                    if (nDataFlag_List[nIndex] == 6 && m_cBHFDataResult_List[nIndex].m_nNewRankIndex == -1)
                    {
                        if (bGetRankFlag == false)
                            dMinTotalScore = m_cBHFDataResult_List[nIndex].m_dTotalScore;
                        else if (m_cBHFDataResult_List[nIndex].m_dTotalScore < dMinTotalScore)
                            dMinTotalScore = m_cBHFDataResult_List[nIndex].m_dTotalScore;

                        bGetRankFlag = true;
                    }
                }

                if (bGetRankFlag == true)
                {
                    for (int nIndex = 0; nIndex < m_cDataInfo_List.Count; nIndex++)
                    {
                        if (nDataFlag_List[nIndex] == 6 && m_cBHFDataResult_List[nIndex].m_nNewRankIndex == -1)
                        {
                            if (m_cBHFDataResult_List[nIndex].m_dTotalScore == dMinTotalScore)
                                m_cBHFDataResult_List[nIndex].m_nNewRankIndex = nNewRankValue;
                        }
                    }
                }
            }

            for (int nIndex = 0; nIndex < m_cDataInfo_List.Count; nIndex++)
            {
                if (m_cBHFDataResult_List[nIndex].m_nNewRankIndex == -1)
                    m_cBHFDataResult_List[nIndex].m_sResult = "Error";
            }
        }

        private void ComputeFinalRingThreshold(int nFileIndex)
        {
            RingValue cPTHF_RX = new RingValue(m_cDataValue_Slant_List[nFileIndex].m_dPTHF_RXRingMean, m_cDataValue_Slant_List[nFileIndex].m_nPTHF_RXRingMeanMinus1STD, m_cDataValue_Slant_List[nFileIndex].m_nPTHF_RXRingMeanMinus2STD);
            RingValue cPTHF_TX = new RingValue(m_cDataValue_Slant_List[nFileIndex].m_dPTHF_TXRingMean, m_cDataValue_Slant_List[nFileIndex].m_nPTHF_TXRingMeanMinus1STD, m_cDataValue_Slant_List[nFileIndex].m_nPTHF_TXRingMeanMinus2STD);
            RingValue cBHF_RX = new RingValue(m_cDataValue_Slant_List[nFileIndex].m_dRXRingMean, m_cDataValue_Slant_List[nFileIndex].m_nRXRingMeanMinus1STD, m_cDataValue_Slant_List[nFileIndex].m_nRXRingMeanMinus2STD);
            RingValue cBHF_TX = new RingValue(m_cDataValue_Slant_List[nFileIndex].m_dTXRingMean, m_cDataValue_Slant_List[nFileIndex].m_nTXRingMeanMinus1STD, m_cDataValue_Slant_List[nFileIndex].m_nTXRingMeanMinus2STD);
            
            RingValue[] cRingValue_Array = new RingValue[] 
            { 
                cPTHF_RX, 
                cPTHF_TX, 
                cBHF_RX, 
                cBHF_TX 
            };

            double dMinMean = 0.0;
            bool bGetValueFlag = false;

            for (int nIndex = 0; nIndex < cRingValue_Array.Length; nIndex++)
            {
                if (cRingValue_Array[nIndex].m_dMean > 0)
                {
                    if (bGetValueFlag == false)
                    {
                        dMinMean = cRingValue_Array[nIndex].m_dMean;
                        bGetValueFlag = true;
                    }
                    else
                    {
                        if (cRingValue_Array[nIndex].m_dMean < dMinMean)
                            dMinMean = cRingValue_Array[nIndex].m_dMean;
                    }
                }
            }

            if (bGetValueFlag == false)
                return;

            int nMinMeanMinus1STD = 0;
            int nMinMeanMinus2STD = 0;
            bool bGetMeanMinus1STDFlag = false;
            bool bGetMeanMinus2STDFlag = false;

            for (int nIndex = 0; nIndex < cRingValue_Array.Length; nIndex++)
            {
                if (cRingValue_Array[nIndex].m_dMean == dMinMean)
                {
                    if (bGetMeanMinus1STDFlag == false)
                    {
                        nMinMeanMinus1STD = cRingValue_Array[nIndex].m_nMeanMinus1STD;
                        bGetMeanMinus1STDFlag = true;
                    }
                    else
                    {
                        if (cRingValue_Array[nIndex].m_nMeanMinus1STD < nMinMeanMinus1STD)
                            nMinMeanMinus1STD = cRingValue_Array[nIndex].m_nMeanMinus1STD;
                    }

                    if (bGetMeanMinus2STDFlag == false)
                    {
                        nMinMeanMinus2STD = cRingValue_Array[nIndex].m_nMeanMinus2STD;
                        bGetMeanMinus2STDFlag = true;
                    }
                    else
                    {
                        if (cRingValue_Array[nIndex].m_nMeanMinus2STD < nMinMeanMinus2STD)
                            nMinMeanMinus2STD = cRingValue_Array[nIndex].m_nMeanMinus2STD;
                    }
                }
            }

            m_cDataValue_Slant_List[nFileIndex].m_nPwrTHTXHB = nMinMeanMinus1STD;
            m_cDataValue_Slant_List[nFileIndex].m_nPwrTHTXLB = nMinMeanMinus2STD;
        }

        /*
        private List<int> GetRankSortList(List<BHFDataResult> cDataResult_List)
        {
            List<int> nRank_List = new List<int>();

            for (int nIndex = 0; nIndex < cDataResult_List.Count; nIndex++)
            {
                if (cDataResult_List[nIndex].m_nNewRankIndex > 0)
                    nRank_List.Add(cDataResult_List[nIndex].m_nNewRankIndex);
            }

            for (int nRankIndex = 0; nRankIndex < nRank_List.Count; nRankIndex++)
            {
                for (int nCompareIndex = nRank_List.Count - 1; nCompareIndex > nRankIndex; nCompareIndex--)
                {
                    if (nRank_List[nRankIndex] == nRank_List[nCompareIndex])
                        nRank_List.RemoveAt(nCompareIndex);
                }
            }

            nRank_List.Sort();

            return nRank_List;
        }
        */

        private List<int> GetSortIndex(List<int> nInputData_List)
        {
            List<int> nData_List = new List<int>(nInputData_List);

            for (int nDataIndex = 0; nDataIndex < nData_List.Count; nDataIndex++)
            {
                for (int nCompareIndex = nData_List.Count - 1; nCompareIndex > nDataIndex; nCompareIndex--)
                {

                    if (nData_List[nDataIndex] == nData_List[nCompareIndex])
                        nData_List.RemoveAt(nCompareIndex);
                }
            }

            nData_List.Sort();

            return nData_List;
        }

        private List<int> GetUsefulIndex(List<int> nInputData_List)
        {
            List<int> nData_List = new List<int>(nInputData_List);

            nData_List.RemoveAt(nData_List.Count - 1);
            nData_List.RemoveAt(0);

            return nData_List;
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

                nTraceIndexData_List = new List<int>(m_nOriginalTraceIndex_RX_List);
                nFirstMaxData_List = new List<int>(m_nOriginalFirstMax_RX_List);
                nSecondMaxData_List = new List<int>(m_nOriginalSecondMax_RX_List);
                nThirdMaxData_List = new List<int>(m_nOriginalThirdMax_RX_List);

                dSecondMaxMean = m_cDataValue_Slant_List[nFileIndex].m_dRXSecondMaxMean;
                dThirdMaxMean = m_cDataValue_Slant_List[nFileIndex].m_dRXThirdMaxMean;
                nTotalMean = m_cDataValue_Slant_List[nFileIndex].m_nRXContactTH;
                nInterval = (int)(nFirstMaxData_List.Count / m_cDataInfo_List[nFileIndex].m_nRXTraceNumber);
            }
            else
            {
                sTitleName = "TX";
                sFilePath = string.Format(@"{0}\TX_{1}", sDirectoryPath, SpecificText.m_sChartFileName);

                nTraceIndexData_List = new List<int>(m_nOriginalTraceIndex_TX_List);
                nFirstMaxData_List = new List<int>(m_nOriginalFirstMax_TX_List);
                nSecondMaxData_List = new List<int>(m_nOriginalSecondMax_TX_List);
                nThirdMaxData_List = new List<int>(m_nOriginalThirdMax_TX_List);

                dSecondMaxMean = m_cDataValue_Slant_List[nFileIndex].m_dTXSecondMaxMean;
                dThirdMaxMean = m_cDataValue_Slant_List[nFileIndex].m_dTXThirdMaxMean;
                nTotalMean = m_cDataValue_Slant_List[nFileIndex].m_nTXContactTH;
                nInterval = (int)(nFirstMaxData_List.Count / m_cDataInfo_List[nFileIndex].m_nTXTraceNumber);
            }

            Series serSeries_1 = new Series("First Max Value");
            Series serSeries_2 = new Series("Second Max Value");
            Series serSeries_3 = new Series("Third Max Value");
            Series serSeries_4 = new Series(string.Format("Second Max Mean({0})", dSecondMaxMean.ToString()));
            Series serSeries_5 = new Series(string.Format("Third Max Mean({0})", dThirdMaxMean.ToString()));
            Series serSeries_6 = new Series(string.Format("Total Mean({0})", nTotalMean.ToString()));

            Series[] serSeries_Array = new Series[] 
            { 
                serSeries_1, 
                serSeries_2, 
                serSeries_3, 
                serSeries_4, 
                serSeries_5, 
                serSeries_6 
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

            //Show Line Chart
            Chart cChart = new Chart();
            var vChartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            cChart.ChartAreas.Add(vChartArea);
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

            for (int nIndex = 0; nIndex < serSeries_Array.Length; nIndex++)
            {
                serSeries_Array[nIndex].ChartType = SeriesChartType.Line;
                serSeries_Array[nIndex].IsValueShownAsLabel = false;
                serSeries_Array[nIndex].Color = colorBackColor_Array[nIndex];

                if (nIndex >= 3)
                    serSeries_Array[nIndex].BorderWidth = 2;

                for (int mIndex = 0; mIndex < nFirstMaxData_List.Count; mIndex++)
                {
                    switch (nIndex)
                    {
                        case 0:
                            serSeries_Array[nIndex].Points.AddXY(nTraceIndexData_List[mIndex].ToString(), nFirstMaxData_List[mIndex]);
                            break;
                        case 1:
                            serSeries_Array[nIndex].Points.AddXY(nTraceIndexData_List[mIndex].ToString(), nSecondMaxData_List[mIndex]);
                            break;
                        case 2:
                            serSeries_Array[nIndex].Points.AddXY(nTraceIndexData_List[mIndex].ToString(), nThirdMaxData_List[mIndex]);
                            break;
                        case 3:
                            serSeries_Array[nIndex].Points.AddXY(nTraceIndexData_List[mIndex].ToString(), dSecondMaxMean);
                            break;
                        case 4:
                            serSeries_Array[nIndex].Points.AddXY(nTraceIndexData_List[mIndex].ToString(), dThirdMaxMean);
                            break;
                        case 5:
                            serSeries_Array[nIndex].Points.AddXY(nTraceIndexData_List[mIndex].ToString(), nTotalMean);
                            break;
                        default:
                            break;
                    }
                }

                cChart.Series.Add(serSeries_Array[nIndex]);
            }

            cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
        }
    }
}
