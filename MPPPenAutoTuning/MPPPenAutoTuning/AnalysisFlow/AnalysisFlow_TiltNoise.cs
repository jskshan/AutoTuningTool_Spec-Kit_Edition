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
    public class AnalysisFlow_TiltNoise : AnalysisFlow
    {
        protected enum ReferenceDataType
        {
            MeanPlus3Std,
            Mean,
            Std,
            Max,
            Min
        }

        protected enum DataType
        {
            Beacon,
            PTHF,
            BHF
        }

        protected static CompareOperator m_eCompareOperator = CompareOperator.Frequency;

        protected bool m_bGetBeaconDataFlag = false;

        protected string m_sTNResultFilePath = "";

        protected string m_sPTHFResultFilePath = "";
        protected string m_sBHFResultFilePath = "";

        protected string m_sTNReferenceFilePath = "";
        protected string m_sRXReferenceFilePath = "";
        protected string m_sTXReferenceFilePath = "";

        protected string m_sPictureFolderPath = "";

        protected int m_nProcessReportCount = -1;
        protected int m_nStartSkipReportCount = 20;
        protected int m_nLastSkipReportCount = 20;

        private int m_nTraceNumberByteLocation = 10;
        private int m_nAutoTuningInfoByteLocation = 13;
        private int m_nTraceTypeBitLocation = 1;
        private int m_nDataTypeBitLocation = 2;
        private int m_nExecuteTypeBitLocation = 3;
        private int m_nDataSectionByteLocation = 14;

        protected int m_nNoiseReportNumber = 0;

        protected bool m_bGetDistributionDataFlag = false;

        protected bool m_bGetRXReportDataFlag = false;
        protected bool m_bGetTXReportDataFlag = false;

        private List<List<byte>> m_byteData_List = new List<List<byte>>();

        protected List<List<int>> m_nRXOriginalData_List = new List<List<int>>();
        protected List<List<int>> m_nTXOriginalData_List = new List<List<int>>();

        protected List<List<int>> m_nRXData_Section1_List = new List<List<int>>();
        protected List<List<int>> m_nRXData_Section2_List = new List<List<int>>();
        protected List<List<int>> m_nRXData_Section3_List = new List<List<int>>();
        protected List<List<int>> m_nRXData_Section4_List = new List<List<int>>();

        protected List<List<int>> m_nTXData_Section1_List = new List<List<int>>();
        protected List<List<int>> m_nTXData_Section2_List = new List<List<int>>();

        protected List<DataInfo> m_cTotalDataInfo_List = null;

        protected List<DataValue> m_cDataValue_List = null;

        protected List<DataValue> m_cTotalDataValue_List = null;

        public class DataValue
        {
            public int m_nPH1 = -1;
            public int m_nPH2 = -1;
            public double m_dFrequency = 0.0;

            public int m_nRankIndex = 0;

            public int m_nPTHF_Contact_TH_Rx = -1;
            public int m_nPTHF_Contact_TH_Tx = -1;
            public int m_nPTHF_Hover_TH_Rx = -1;
            public int m_nPTHF_Hover_TH_Tx = -1;

            public bool m_bGetPTHFRXReferenceDataFlag = false;
            public bool m_bGetPTHFTXReferenceDataFlag = false;

            public ReferenceValue m_cRXReferenceValue = new ReferenceValue();
            public ReferenceValue m_cTXReferenceValue = new ReferenceValue();

            public ReferenceData m_cRXReferenceData = new ReferenceData();
            public ReferenceData m_cTXReferenceData = new ReferenceData();

            public ResultData m_cRXResultData = new ResultData();
            public ResultData m_cTXResultData = new ResultData();

            public bool m_bInnerReferenceValueOverHBFlag = false;

            public double m_dWeightingInnerRXValue = 0.0;
            public double m_dWeightingInnerTXValue = 0.0;
            public double m_dWeightingEdgeRXValue = 0.0;
            public double m_dWeightingEdgeTXValue = 0.0;
            public double m_dWeightingScore = 0.0;

            public DistributionData m_cRXDistributionData = null;
            public DistributionData m_cTXDistributionData = null;

            public PreviousStepData m_cRXPreviousStepData = null;
            public PreviousStepData m_cTXPreviousStepData = null;

            public RankData m_cRankData_Beacon = null;
            public RankData m_cRankData_PTHF = null;
            public RankData m_cRankData_BHF = null;

            public DataValue SetDeepCopy()
            {
                DataValue cDeepCopy = new DataValue();
                cDeepCopy.m_nPH1 = this.m_nPH1;
                cDeepCopy.m_nPH2 = this.m_nPH2;
                cDeepCopy.m_dFrequency = this.m_dFrequency;
                cDeepCopy.m_nRankIndex = this.m_nRankIndex;

                cDeepCopy.m_nPTHF_Contact_TH_Rx = this.m_nPTHF_Contact_TH_Rx;
                cDeepCopy.m_nPTHF_Contact_TH_Tx = this.m_nPTHF_Contact_TH_Tx;
                cDeepCopy.m_nPTHF_Hover_TH_Rx = this.m_nPTHF_Hover_TH_Rx;
                cDeepCopy.m_nPTHF_Hover_TH_Tx = this.m_nPTHF_Hover_TH_Tx;

                cDeepCopy.m_cRXReferenceValue = this.m_cRXReferenceValue;
                cDeepCopy.m_cTXReferenceValue = this.m_cTXReferenceValue;

                cDeepCopy.m_cRXReferenceData = this.m_cRXReferenceData;
                cDeepCopy.m_cTXReferenceData = this.m_cTXReferenceData;

                cDeepCopy.m_cRXResultData = this.m_cRXResultData;
                cDeepCopy.m_cTXResultData = this.m_cTXResultData;

                cDeepCopy.m_dWeightingInnerRXValue = this.m_dWeightingInnerRXValue;
                cDeepCopy.m_dWeightingInnerTXValue = this.m_dWeightingInnerTXValue;
                cDeepCopy.m_dWeightingEdgeRXValue = this.m_dWeightingEdgeRXValue;
                cDeepCopy.m_dWeightingEdgeTXValue = this.m_dWeightingEdgeTXValue;
                cDeepCopy.m_dWeightingScore = this.m_dWeightingScore;

                return cDeepCopy;
            }
        }

        public class DataValueComparer : IComparer<DataValue>
        {
            public int Compare(DataValue cDataValue_1, DataValue cDataValue_2)
            {
                if (m_eCompareOperator == CompareOperator.Frequency)
                    return cDataValue_1.m_dFrequency.CompareTo(cDataValue_2.m_dFrequency);
                else if (m_eCompareOperator == CompareOperator.RankIndex)
                    return cDataValue_1.m_nRankIndex.CompareTo(cDataValue_2.m_nRankIndex);
                else if (m_eCompareOperator == CompareOperator.Beacon)
                    return cDataValue_1.m_cRankData_Beacon.m_nRankIndex.CompareTo(cDataValue_2.m_cRankData_Beacon.m_nRankIndex);
                else if (m_eCompareOperator == CompareOperator.PTHF)
                    return cDataValue_1.m_cRankData_PTHF.m_nRankIndex.CompareTo(cDataValue_2.m_cRankData_PTHF.m_nRankIndex);
                else if (m_eCompareOperator == CompareOperator.BHF)
                    return cDataValue_1.m_cRankData_BHF.m_nRankIndex.CompareTo(cDataValue_2.m_cRankData_BHF.m_nRankIndex);
                else
                    return cDataValue_1.m_dFrequency.CompareTo(cDataValue_2.m_dFrequency);
            }
        }

        public class RankData
        {
            public int m_nRankIndex = 0;

            public double m_dTXInnerValue = 0.0;
            public double m_dRXInnerValue = 0.0;
            public double m_dTXEdgeValue = 0.0;
            public double m_dRXEdgeValue = 0.0;
            public int m_nTotalMaxValue = 0;
        }

        public class ReferenceData
        {
            public double[] m_dMean_Array = null;
            public double[] m_dStd_Array = null;
            public int[] m_nMax_Array = null;
            public int[] m_nMin_Array = null;
            public double[] m_dMeanPlus3Std_Array = null;

            public ReferenceData()
            {
            }

            public void InitializeArray(int nTraceNumber)
            {
                m_dMean_Array = new double[nTraceNumber];
                m_dStd_Array = new double[nTraceNumber];
                m_nMax_Array = new int[nTraceNumber];
                m_nMin_Array = new int[nTraceNumber];
                m_dMeanPlus3Std_Array = new double[nTraceNumber];

                Array.Clear(m_dMean_Array, 0, m_dMean_Array.Length);
                Array.Clear(m_dStd_Array, 0, m_dStd_Array.Length);
                Array.Clear(m_nMax_Array, 0, m_nMax_Array.Length);
                Array.Clear(m_nMin_Array, 0, m_nMin_Array.Length);
                Array.Clear(m_dMeanPlus3Std_Array, 0, m_dMeanPlus3Std_Array.Length);
            }
        }

        public class ResultData
        {
            public double m_dEdgeMaxReferenceData = 0.0;
            public int m_nEdgeMaxIndex = 0;
            public double m_dInnerMaxReferenceData = 0.0;
            public int m_nInnerMaxIndex = 0;
            public double m_dTotalMaxReferenceData = 0.0;
            public int m_nTotalMaxIndex = 0;
        }

        public class DistributionData
        {
            public double[] m_dInRange1Std_Array = null;
            public double[] m_dInRange2Std_Array = null;
            public double[] m_dInRange3Std_Array = null;
            public double[] m_dOver3Std_Array = null;

            public int[] m_nHistogramScale_Array = null;
            public int[] m_nHistogramAmount_Array = null;

            public DistributionData()
            {
            }

            public void InitializeArray(int nTraceNumber, int nPartNumber)
            {
                m_dInRange1Std_Array = new double[nTraceNumber];
                m_dInRange2Std_Array = new double[nTraceNumber];
                m_dInRange3Std_Array = new double[nTraceNumber];
                m_dOver3Std_Array = new double[nTraceNumber];

                m_nHistogramScale_Array = new int[nPartNumber];
                m_nHistogramAmount_Array = new int[nPartNumber];

                Array.Clear(m_dInRange1Std_Array, 0, m_dInRange1Std_Array.Length);
                Array.Clear(m_dInRange2Std_Array, 0, m_dInRange2Std_Array.Length);
                Array.Clear(m_dInRange3Std_Array, 0, m_dInRange3Std_Array.Length);
                Array.Clear(m_dOver3Std_Array, 0, m_dOver3Std_Array.Length);

                Array.Clear(m_nHistogramScale_Array, 0, m_nHistogramScale_Array.Length);
                Array.Clear(m_nHistogramAmount_Array, 0, m_nHistogramAmount_Array.Length);
            }
        }

        public class PreviousStepData
        {
            public double m_dEdgeMaxReferenceData = 0.0;
            public int m_nEdgeMaxIndex = 0;
            public double m_dInnerMaxReferenceData = 0.0;
            public int m_nInnerMaxIndex = 0;
            public double m_dTotalMaxReferenceData = 0.0;
            public int m_nTotalMaxIndex = 0;

            public double[] m_dReferenceValue_Array = null;
            public double[] m_dMeanValue_Array = null;
            public double[] m_dStdevValue_Array = null;
            public int[] m_nMaxValue_Array = null;

            public void InitializeArray(int nTraceNumber)
            {
                m_dReferenceValue_Array = new double[nTraceNumber];
                Array.Clear(m_dReferenceValue_Array, 0, m_dReferenceValue_Array.Length);
                m_dMeanValue_Array = new double[nTraceNumber];
                Array.Clear(m_dMeanValue_Array, 0, m_dMeanValue_Array.Length);
                m_dStdevValue_Array = new double[nTraceNumber];
                Array.Clear(m_dStdevValue_Array, 0, m_dStdevValue_Array.Length);
                m_nMaxValue_Array = new int[nTraceNumber];
                Array.Clear(m_nMaxValue_Array, 0, m_nMaxValue_Array.Length);
            }

            public int m_nTotalMaxValue = 0;
        }

        protected void SetSortData(CompareOperator eCompareOperator, bool bTotalDataFlag = false)
        {
            List<DataInfo> cDataInfo_List;
            List<DataValue> cDataValue_List;

            if (bTotalDataFlag == false)
            {
                cDataInfo_List = m_cDataInfo_List;
                cDataValue_List = m_cDataValue_List;
            }
            else
            {
                cDataInfo_List = m_cTotalDataInfo_List;
                cDataValue_List = m_cTotalDataValue_List;
            }

            if (eCompareOperator == CompareOperator.Frequency)
            {
                cDataInfo_List.Sort((x, y) => x.m_dFrequency.CompareTo(y.m_dFrequency));
                cDataValue_List.Sort((x, y) => x.m_dFrequency.CompareTo(y.m_dFrequency));
            }
            else if (eCompareOperator == CompareOperator.RankIndex)
            {
                cDataInfo_List.Sort((x, y) => x.m_nRankIndex.CompareTo(y.m_nRankIndex));
                cDataValue_List.Sort((x, y) => x.m_nRankIndex.CompareTo(y.m_nRankIndex));
            }
        }

        protected void ResetFlag()
        {
            m_bGetRXReportDataFlag = false;
            m_bGetTXReportDataFlag = false;
        }

        protected virtual void ClearDataArray()
        {
            m_byteData_List.Clear();

            m_nRXOriginalData_List.Clear();
            m_nTXOriginalData_List.Clear();

            m_nRXData_Section1_List.Clear();
            m_nRXData_Section2_List.Clear();
            m_nRXData_Section3_List.Clear();
            m_nRXData_Section4_List.Clear();

            m_nTXData_Section1_List.Clear();
            m_nTXData_Section2_List.Clear();
        }

        public AnalysisFlow_TiltNoise()
        {
        }

        public AnalysisFlow_TiltNoise(FlowStep cFlowStep, frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
            m_sFileDirectoryPath = m_cfrmMain.m_sFileDirectoryPath;
            m_sRecordProjectName = m_cfrmMain.m_sRecordProjectName;
            m_sFlowDirectoryPath = m_cfrmMain.m_sFlowDirectoryPath;

            InitializeParameter(cFlowStep);

            m_cDataValue_List = new List<DataValue>();

            m_bAllOverInnerReferenceValueHBFlag = false;

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

            m_bGetDistributionDataFlag = (ParamAutoTuning.m_nNoiseGetDistributionData == 1) ? true : false;

            m_nProcessReportCount = ParamAutoTuning.m_nNoiseProcessReportNumber;
            m_nStartSkipReportCount = ParamAutoTuning.m_nStartSkipReportCount;
            m_nLastSkipReportCount = ParamAutoTuning.m_nLastSkipReportCount;

            m_nNormalReportDataLength = ParamAutoTuning.m_nNormalReportDataLength;
            m_nTraceNumberByteLocation = ParamAutoTuning.m_nRTXTraceNumberByte;
            m_nAutoTuningInfoByteLocation = ParamAutoTuning.m_nAutoTuningInfoByte;
            m_nTraceTypeBitLocation = ParamAutoTuning.m_nTraceTypeBit;
            m_nDataTypeBitLocation = ParamAutoTuning.m_nDataTypeBit;
            m_nExecuteTypeBitLocation = ParamAutoTuning.m_nExecuteTypeBit;
            m_nDataSectionByteLocation = ParamAutoTuning.m_nDataSectionByte;

            m_dInnerReferenceValueHB = ParamAutoTuning.m_dInnerReferenceValueHB;
            m_nEdgeSS_OFF_InnerRXWeightingPercent = (int)Math.Round(ParamAutoTuning.m_dEdgeSS_OFF_InnerRXWeightingPercent, 0, MidpointRounding.AwayFromZero);
            m_nEdgeSS_OFF_InnerTXWeightingPercent = (int)Math.Round(ParamAutoTuning.m_dEdgeSS_OFF_InnerTXWeightingPercent, 0, MidpointRounding.AwayFromZero);
            m_nEdgeSS_OFF_EdgeRXWeightingPercent = (int)Math.Round(ParamAutoTuning.m_dEdgeSS_OFF_EdgeRXWeightingPercent, 0, MidpointRounding.AwayFromZero);
            m_nEdgeSS_OFF_EdgeTXWeightingPercent = (int)Math.Round(ParamAutoTuning.m_dEdgeSS_OFF_EdgeTXWeightingPercent, 0, MidpointRounding.AwayFromZero);
            m_dMaxMinusMeanValueOverWarningStdevMagHB = ParamAutoTuning.m_dMaxMinusMeanValueOverWarningStdevMagHB;
            m_dMaxValueOverWarningAbsValueHB = ParamAutoTuning.m_dMaxValueOverWarningAbsValueHB;

            if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_LOADDATA)
            {
                if (m_cfrmMain.m_sCPUType == MainConstantParameter.m_sCPUTYPE_AMD ||
                    m_cfrmMain.m_sCPUType == MainConstantParameter.m_sCPUTYPE_NONE)
                    m_nNoiseReportNumber = ParamAutoTuning.m_nNoiseReportNumber;
                else
                    m_nNoiseReportNumber = (int)((double)ParamAutoTuning.m_nNoiseReportNumber * 1.1);
            }
            else
                m_nNoiseReportNumber = ParamAutoTuning.m_nNoiseReportNumber;
        }

        public void SetFileDirectory()
        {
            m_sStepFolderPath = string.Format(@"{0}\{1}", m_sFileDirectoryPath, m_sSubStepName);
            m_sResultFolderPath = string.Format(@"{0}\{1}({2})", m_sFileDirectoryPath, SpecificText.m_sResultText, m_sSubStepCodeName);
            m_sFlowBackUpFolderPath = string.Format(@"{0}\{1}", m_sFileDirectoryPath, SpecificText.m_sFlowText);
            m_sProjectName = m_sRecordProjectName;
            m_sProjectFolderPath = m_sFileDirectoryPath;
            m_sStepListPath = string.Format(@"{0}\{1}_{2}.csv", m_sProjectFolderPath, SpecificText.m_sStepListText, m_sSubStepCodeName);

            m_sTNResultFilePath = string.Format(@"{0}\{1}", m_sResultFolderPath, SpecificText.m_sTNResultFileName);
            m_sResultFilePath = string.Format(@"{0}\{1}", m_sResultFolderPath, SpecificText.m_sResultFileName);

            m_sNoiseResultFilePath = string.Format(@"{0}\{1}", m_sStepFolderPath, SpecificText.m_sNRankFileName);

            m_sPictureFolderPath = string.Format(@"{0}\{1}", m_sResultFolderPath, SpecificText.m_sPictureText);

            m_sReferenceFolderPath = string.Format(@"{0}\{1}", m_sResultFolderPath, SpecificText.m_sReferenceText);
            m_sReferenceFilePath = string.Format(@"{0}\{1}", m_sReferenceFolderPath, SpecificText.m_sReferenceFileName);
            m_sRXReferenceFilePath = string.Format(@"{0}\{1}", m_sReferenceFolderPath, SpecificText.m_sRXReferenceFileName);
            m_sTXReferenceFilePath = string.Format(@"{0}\{1}", m_sReferenceFolderPath, SpecificText.m_sTXReferenceFileName);

            if (m_eSubStep != SubTuningStep.TILTNO_BHF)
                return;

            string sPTHFCodeName = StringConvert.m_dictSubStepCNMappingTable[SubTuningStep.TILTNO_PTHF];
            string sPTHFResultFolderPath = string.Format(@"{0}\{1}({2})", m_sFileDirectoryPath, SpecificText.m_sResultText, sPTHFCodeName);
            m_sPTHFResultFilePath = string.Format(@"{0}\{1}", sPTHFResultFolderPath, SpecificText.m_sResultFileName);

            string sBHFCodeName = StringConvert.m_dictSubStepCNMappingTable[SubTuningStep.TILTNO_BHF];
            string sBHFResultFolderPath = string.Format(@"{0}\{1}({2})", m_sFileDirectoryPath, SpecificText.m_sResultText, sBHFCodeName);
            m_sBHFResultFilePath = string.Format(@"{0}\{1}", sBHFResultFolderPath, SpecificText.m_sResultFileName);

            m_sTNReferenceFilePath = string.Format(@"{0}\{1}", m_sReferenceFolderPath, SpecificText.m_sTNReferenceFileName);
        }

        public virtual void GetData(List<TiltNoiseParameter> cParameter_List)
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
                }
                else
                {
                    bool bCheckFlag = false;

                    if (CheckAnalogParameterIsIdentical() == true)
                    {
                        TiltNoiseParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2);

                        if (cParameter != null)
                        {
                            if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_LOADDATA && cParameter.m_nRankIndex != cDataInfo.m_nRankIndex)
                                continue;

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

                if (m_eSubStep == SubTuningStep.TILTNO_BHF)
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
                }

                GetReportData(cDataInfo, cDataValue, sFilePath);
                #endregion

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                CheckTotalReportDataIsEnough(cDataInfo, cDataValue, sFilePath);

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                SeperateRXTXReportData(cDataInfo, cDataValue, sFilePath);

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                cDataValue.m_cRXReferenceData.InitializeArray(m_nRXTraceNumber);
                cDataValue.m_cTXReferenceData.InitializeArray(m_nTXTraceNumber);

                int nRXDataCount = ConvertReportData(cDataInfo, cDataValue, sFilePath, true);

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                int nTXDataCount = ConvertReportData(cDataInfo, cDataValue, sFilePath, false);

                if (m_bReadReportDataErrorFlag == true)
                    continue;

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

                CheckRecordErrorCodeHasValue(cDataInfo, sFilePath);

                if (m_bReadReportDataErrorFlag == true)
                    continue;

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

        public virtual void ComputeAndOutputResult()
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

                if (m_eSubStep == SubTuningStep.TILTNO_BHF)
                {
                    SetDeepCopyForTotalDataList();

                    GetPreviousStepData(true);
                    GetPreviousStepData(false);

                    if (ComputeTotalReferenceValue() == false)
                        return;

                    ComputeTotalResultData();

                    ComputeWeightingRank(true);

                    SetSortData(CompareOperator.RankIndex, true);

                    WriteResultDataFile(true);

                    //SaveTraceLineChartPictureFile(true);
                    SaveFrequencyLineChartPictureFile(true);

                    if (File.Exists(m_sNoiseResultFilePath) == true)
                    {
                        GetPreviousStepListData(DataType.Beacon);
                        m_bGetBeaconDataFlag = true;
                    }

                    GetPreviousStepListData(DataType.PTHF);

                    GetPreviousStepListData(DataType.BHF);

                    SaveFrequencyLineChartPictureFileBy3SubPlot();

                    WriteCompositeReferenceDataFile();
                }
            }

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
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_TRACETYPE, sLine, 0x000080, m_nINFOTYPE_STRING);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RXTRACENUMBER, sLine, 0x000100, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_TXTRACENUMBER, sLine, 0x000200, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_PTHF_RX, sLine, 0x000400, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_PTHF_TX, sLine, 0x000800, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BHF_RX, sLine, 0x001000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BHF_TX, sLine, 0x002000, m_nINFOTYPE_INT);

                    if (lGetInfoFlag == 0x003FFF)
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

            if (sErrorMessage != "")
                return false;

            return true;
        }

        protected bool GetPreviousStepValue(List<TiltNoiseParameter> cParameter_List, DataInfo cDataInfo, DataValue cDataValue)
        {
            bool bErrorFlag = true;

            for (int nParameterIndex = 0; nParameterIndex < cParameter_List.Count; nParameterIndex++)
            {
                if (cParameter_List[nParameterIndex].m_nPH1 == cDataInfo.m_nReadPH1 &&
                    cParameter_List[nParameterIndex].m_nPH2 == cDataInfo.m_nReadPH2)
                {
                    int nGetParameterFlag = 0;

                    if (cParameter_List[nParameterIndex].m_nTN_PTHFRXTotalMax != -1)
                    {
                        cDataValue.m_nPTHF_Contact_TH_Rx = cParameter_List[nParameterIndex].m_nTN_PTHFRXTotalMax;
                        cDataValue.m_nPTHF_Hover_TH_Rx = cParameter_List[nParameterIndex].m_nTN_PTHFRXTotalMax;
                        nGetParameterFlag |= 0x0001;
                    }

                    if (cParameter_List[nParameterIndex].m_nTN_PTHFTXTotalMax != -1)
                    {
                        cDataValue.m_nPTHF_Contact_TH_Tx = cParameter_List[nParameterIndex].m_nTN_PTHFTXTotalMax;
                        cDataValue.m_nPTHF_Hover_TH_Tx = cParameter_List[nParameterIndex].m_nTN_PTHFTXTotalMax;
                        nGetParameterFlag |= 0x0002;
                    }

                    if (nGetParameterFlag == 0x0003)
                        bErrorFlag = false;

                    break;
                }
            }

            return !bErrorFlag;
        }

        private void GetReportData(DataInfo cDataInfo, DataValue cDataValue, string sFilePath)
        {
            int nLineCounter = 0;
            string sLine = "";

            // Read the file and display it line by line
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    nLineCounter++;

                    if (StringConvert.CheckRecordInfoExist(sLine, m_sRecordInfo_List) == true)
                        continue;

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

                        if (byteData_Array[0] != 0x07 ||
                            byteData_Array[1] != 0x01 ||
                            byteData_Array[2] != 0xFF ||
                            byteData_Array[3] != 0xFF ||
                            byteData_Array[4] != 0xFF ||
                            byteData_Array[5] != 0xFF ||
                            (byteData_Array[m_nAutoTuningInfoByteLocation - 1] & 0x40) == 0)
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

                    List<byte> byteReport_List = new List<byte>();

                    for (int nIndex = 0; nIndex < sSplit_Array.Length - 1; nIndex++)
                    {
                        byte byteRawData = Convert.ToByte(sSplit_Array[nIndex], 16);
                        byteReport_List.Add(byteRawData);
                    }

                    m_byteData_List.Add(new List<byte>(byteReport_List));
                }
            }
            finally
            {
                srFile.Close();
            }
        }

        private void CheckTotalReportDataIsEnough(DataInfo cDataInfo, DataValue cDataValue, string sFilePath)
        {
            if (m_byteData_List.Count < Math.Min(m_nNoiseReportNumber, ParamAutoTuning.m_nNoiseValidReportNumber) * 6)
            {
                cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0008;

                m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Enough Report Data In {0} File!", Path.GetFileName(sFilePath));
                m_cErrorInfo.m_sRecordErrorMessage = "No Enough Report Data";
                m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
            }
        }

        private void SeperateRXTXReportData(DataInfo cDataInfo, DataValue cDataValue, string sFilePath)
        {
            for (int nDataIndex = 0; nDataIndex < m_byteData_List.Count; nDataIndex++)
            {
                List<byte> byteData_List = m_byteData_List[nDataIndex];

                int nTraceTypeBitValue = Convert.ToInt32(Math.Pow(2, m_nTraceTypeBitLocation - 1));
                int nDataTypeBitValue = Convert.ToInt32(Math.Pow(2, m_nDataTypeBitLocation - 1));
                int nExecuteTypeBitValue = Convert.ToInt32(Math.Pow(2, m_nExecuteTypeBitLocation - 1));

                if ((byteData_List[m_nAutoTuningInfoByteLocation - 1] & nTraceTypeBitValue) == 0)
                {
                    if ((byteData_List[m_nAutoTuningInfoByteLocation - 1] & nDataTypeBitValue) != 1 &&
                        (byteData_List[m_nAutoTuningInfoByteLocation - 1] & nExecuteTypeBitValue) == 0)
                    {
                        if (GetRXTXReportData(cDataInfo, cDataValue, byteData_List, nDataIndex, sFilePath, true) == false)
                            break;
                    }
                }
                else
                {
                    if ((byteData_List[m_nAutoTuningInfoByteLocation - 1] & nDataTypeBitValue) != 1 &&
                        (byteData_List[m_nAutoTuningInfoByteLocation - 1] & nExecuteTypeBitValue) == 0)
                    {
                        if (GetRXTXReportData(cDataInfo, cDataValue, byteData_List, nDataIndex, sFilePath, false) == false)
                            break;
                    }
                }
            }
        }

        private bool GetRXTXReportData(DataInfo cDataInfo, DataValue cDataValue, List<byte> byteData_List, int nDataIndex, string sFilePath, bool bRXTraceTypeFlag)
        {
            int nSectionValue = Convert.ToInt32(byteData_List[m_nDataSectionByteLocation - 1]);
            int nTraceNumber = Convert.ToInt32(byteData_List[m_nTraceNumberByteLocation - 1]);

            if (bRXTraceTypeFlag == true)
            {
                if (m_bGetRXReportDataFlag == false)
                {
                    m_nRXTraceNumber = nTraceNumber;
                    m_bGetRXReportDataFlag = true;
                }
                else if (m_nRXTraceNumber != nTraceNumber)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0010;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("RX Trace Number Not Match In Line {0} in {1} File!", nDataIndex + 1, Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("RX Trace Number Not Match In Line {0}", nDataIndex + 1);
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    return false;
                }
            }
            else
            {
                if (m_bGetTXReportDataFlag == false)
                {
                    m_nTXTraceNumber = nTraceNumber;
                    m_bGetTXReportDataFlag = true;
                }
                else if (m_nTXTraceNumber != nTraceNumber)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0020;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("TX Trace Number Not Match In Line {0} in {1} File!", nDataIndex + 1, Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("TX Trace Number Not Match In Line {0}", nDataIndex + 1);
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                    return false;
                }
            }

            int nSectionCount = (int)(nTraceNumber / 24);
            int nSectionRemainder = nTraceNumber % 24;

            if (nSectionRemainder > 0)
                nSectionCount = nSectionCount + 1;

            int nSectionTraceNumber = 0;

            if (nSectionValue + 1 < nSectionCount)
                nSectionTraceNumber = 24;
            else
            {
                if (nSectionRemainder > 0)
                    nSectionTraceNumber = nSectionRemainder;
                else
                    nSectionTraceNumber = 24;
            }

            int[] nData_Array = new int[nSectionTraceNumber];

            for (int nIndex = 0; nIndex < nSectionTraceNumber; nIndex++)
                nData_Array[nIndex] = (byteData_List[2 * nIndex + m_nNormalReportDataLength] * 256 + byteData_List[2 * nIndex + m_nNormalReportDataLength + 1]);

            if (nSectionValue + 1 <= nSectionCount)
            {
                if (bRXTraceTypeFlag == true)
                {
                    if (nSectionValue == 0)
                        m_nRXData_Section1_List.Add(new List<int>(nData_Array));
                    else if (nSectionValue == 1)
                        m_nRXData_Section2_List.Add(new List<int>(nData_Array));
                    else if (nSectionValue == 2)
                        m_nRXData_Section3_List.Add(new List<int>(nData_Array));
                    else if (nSectionValue == 3)
                        m_nRXData_Section4_List.Add(new List<int>(nData_Array));
                }
                else
                {
                    if (nSectionValue == 0)
                        m_nTXData_Section1_List.Add(new List<int>(nData_Array));
                    else if (nSectionValue == 1)
                        m_nTXData_Section2_List.Add(new List<int>(nData_Array));
                }
            }

            return true;
        }

        private int ConvertReportData(DataInfo cDataInfo, DataValue cDataValue, string sFilePath, bool bRXTraceTypeFlag)
        {
            int nTraceNumber = 0;

            if (bRXTraceTypeFlag == true)
                nTraceNumber = m_nRXTraceNumber;
            else
                nTraceNumber = m_nTXTraceNumber;

            int nSectionCount = (int)(nTraceNumber / 24);

            int nSectionRemainder = nTraceNumber % 24;

            if (nSectionRemainder > 0)
                nSectionCount = nSectionCount + 1;

            string sTraceType = "";
            int nDataCount = 0;

            if (bRXTraceTypeFlag == true)
            {
                nDataCount = m_nRXData_Section1_List.Count;

                if (nSectionCount >= 2)
                    nDataCount = Math.Min(nDataCount, m_nRXData_Section2_List.Count);
                if (nSectionCount >= 3)
                    nDataCount = Math.Min(nDataCount, m_nRXData_Section3_List.Count);
                if (nSectionCount >= 4)
                    nDataCount = Math.Min(nDataCount, m_nRXData_Section4_List.Count);

                sTraceType = StringConvert.m_sTRACETYPE_RX;
            }
            else
            {
                nDataCount = m_nTXData_Section1_List.Count;

                if (nSectionCount >= 2)
                    nDataCount = Math.Min(nDataCount, m_nTXData_Section2_List.Count);

                sTraceType = StringConvert.m_sTRACETYPE_TX;
            }

            if (nDataCount < Math.Min(m_nNoiseReportNumber, ParamAutoTuning.m_nNoiseValidReportNumber) || nDataCount == 0)
            {
                if (bRXTraceTypeFlag == true)
                    m_nErrorFlag |= 0x0040;
                else
                    m_nErrorFlag |= 0x0080;

                cDataInfo.m_nErrorFlag = m_nErrorFlag;

                m_cErrorInfo.m_sPrintErrorMessage = string.Format("No Enough {0} Report Data In {1} File!", sTraceType, Path.GetFileName(sFilePath));
                m_cErrorInfo.m_sRecordErrorMessage = string.Format("No Enough {0} Report Data", sTraceType);
                m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                if (bRXTraceTypeFlag == true)
                    m_cErrorInfo.m_nRXDataNumber = nDataCount;
                else
                    m_cErrorInfo.m_nTXDataNumber = nDataCount;

                cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                return nDataCount;
            }

            if (bRXTraceTypeFlag == true)
            {
                for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
                {
                    List<int> nData_List = new List<int>();

                    if (nSectionCount >= 1)
                        nData_List.AddRange(m_nRXData_Section1_List[nDataIndex]);
                    if (nSectionCount >= 2)
                        nData_List.AddRange(m_nRXData_Section2_List[nDataIndex]);
                    if (nSectionCount >= 3)
                        nData_List.AddRange(m_nRXData_Section3_List[nDataIndex]);
                    if (nSectionCount >= 4)
                        nData_List.AddRange(m_nRXData_Section4_List[nDataIndex]);

                    m_nRXOriginalData_List.Add(nData_List);
                }

                m_nRXOriginalData_List.RemoveRange(nDataCount - m_nLastSkipReportCount, m_nLastSkipReportCount);
                m_nRXOriginalData_List.RemoveRange(0, m_nStartSkipReportCount);

                if (m_nProcessReportCount > 0)
                {
                    int nRemoveDataNumber = m_nRXOriginalData_List.Count - m_nProcessReportCount;

                    if (nRemoveDataNumber > 0)
                        m_nRXOriginalData_List.RemoveRange(m_nProcessReportCount, nRemoveDataNumber);
                }

                nDataCount = m_nRXOriginalData_List.Count;

                m_nRXData_Section1_List.Clear();
                m_nRXData_Section2_List.Clear();
                m_nRXData_Section3_List.Clear();
                m_nRXData_Section4_List.Clear();

                m_bGetRXReportDataFlag = true;
            }
            else
            {
                for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
                {
                    List<int> nData_List = new List<int>();

                    if (nSectionCount >= 1)
                        nData_List.AddRange(m_nTXData_Section1_List[nDataIndex]);
                    if (nSectionCount >= 2)
                        nData_List.AddRange(m_nTXData_Section2_List[nDataIndex]);

                    m_nTXOriginalData_List.Add(nData_List);
                }

                m_nTXOriginalData_List.RemoveRange(nDataCount - m_nLastSkipReportCount, m_nLastSkipReportCount);
                m_nTXOriginalData_List.RemoveRange(0, m_nStartSkipReportCount);

                if (m_nProcessReportCount > 0)
                {
                    int nRemoveDataNumber = m_nTXOriginalData_List.Count - m_nProcessReportCount;

                    if (nRemoveDataNumber > 0)
                        m_nTXOriginalData_List.RemoveRange(m_nProcessReportCount, nRemoveDataNumber);
                }

                nDataCount = m_nTXOriginalData_List.Count;

                m_nTXData_Section1_List.Clear();
                m_nTXData_Section2_List.Clear();

                m_bGetTXReportDataFlag = true;
            }

            return nDataCount;
        }

        protected void ComputeReferenceData(DataValue cDataValue, int nDataCount, bool bRXTraceTypeFlag)
        {
            if (bRXTraceTypeFlag == true)
            {
                if (m_bGetRXReportDataFlag == true && nDataCount > 0)
                {
                    for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                    {
                        int[] nValue_Array = new int[nDataCount];

                        for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
                            nValue_Array[nDataIndex] = m_nRXOriginalData_List[nDataIndex][nTraceIndex];

                        List<int> nValue_List = nValue_Array.ToList();

                        double dMean = Math.Round(nValue_List.Average(), 2, MidpointRounding.AwayFromZero);
                        double dStd = ComputeStdValue(nValue_Array);
                        int nMax = nValue_List.Max();
                        int nMin = nValue_List.Min();
                        double dMeanPlus3Std = Math.Round(dMean + 3 * dStd, 2, MidpointRounding.AwayFromZero);

                        cDataValue.m_cRXReferenceData.m_dMean_Array[nTraceIndex] = dMean;
                        cDataValue.m_cRXReferenceData.m_dStd_Array[nTraceIndex] = dStd;
                        cDataValue.m_cRXReferenceData.m_nMax_Array[nTraceIndex] = nMax;
                        cDataValue.m_cRXReferenceData.m_nMin_Array[nTraceIndex] = nMin;
                        cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex] = dMeanPlus3Std;
                    }
                }
            }
            else
            {
                if (m_bGetTXReportDataFlag == true && nDataCount > 0)
                {
                    for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                    {
                        int[] nValue_Array = new int[nDataCount];

                        for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
                            nValue_Array[nDataIndex] = m_nTXOriginalData_List[nDataIndex][nTraceIndex];

                        List<int> nValue_List = nValue_Array.ToList();

                        double dMean = Math.Round(nValue_List.Average(), 2, MidpointRounding.AwayFromZero);
                        double dStd = ComputeStdValue(nValue_Array);
                        int nMax = nValue_List.Max();
                        int nMin = nValue_List.Min();
                        double dMeanPlus3Std = dMean + 3 * dStd;

                        cDataValue.m_cTXReferenceData.m_dMean_Array[nTraceIndex] = dMean;
                        cDataValue.m_cTXReferenceData.m_dStd_Array[nTraceIndex] = dStd;
                        cDataValue.m_cTXReferenceData.m_nMax_Array[nTraceIndex] = nMax;
                        cDataValue.m_cTXReferenceData.m_nMin_Array[nTraceIndex] = nMin;
                        cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex] = dMeanPlus3Std;
                    }
                }
            }
        }

        protected void ComputeReferenceValue(DataValue cDataValue, int nDataCount, bool bRXTraceTypeFlag)
        {
            ReferenceValue cReferenceValue = new ReferenceValue();
            int nMaxValue = -1;
            int nMinValue = -1;
            int nInnerMaxValue = -1;
            int nEdgeMaxValue = -1;
            double dMeanValue = 0;

            int nTraceNumber = m_nRXTraceNumber;
            ReferenceData cReferenceData;

            if (bRXTraceTypeFlag == true)
            {
                nTraceNumber = m_nRXTraceNumber;
                cReferenceData = cDataValue.m_cRXReferenceData;
            }
            else
            {
                nTraceNumber = m_nTXTraceNumber;
                cReferenceData = cDataValue.m_cTXReferenceData;
            }

            for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
            {
                // Edge
                if (nTraceIndex < m_nEdgeTraceNumber || nTraceIndex >= nTraceNumber - m_nEdgeTraceNumber)
                {
                    if (nTraceIndex == 0)
                        nEdgeMaxValue = cReferenceData.m_nMax_Array[nTraceIndex];
                    else if (cReferenceData.m_nMax_Array[nTraceIndex] > nEdgeMaxValue)
                        nEdgeMaxValue = cReferenceData.m_nMax_Array[nTraceIndex];
                }
                // Inner
                else
                {
                    if (nTraceIndex == m_nEdgeTraceNumber)
                        nInnerMaxValue = cReferenceData.m_nMax_Array[nTraceIndex];
                    else if (cReferenceData.m_nMax_Array[nTraceIndex] > nInnerMaxValue)
                        nInnerMaxValue = cReferenceData.m_nMax_Array[nTraceIndex];
                }

                if (nTraceIndex == 0)
                {
                    nMaxValue = cReferenceData.m_nMax_Array[nTraceIndex];
                    nMinValue = cReferenceData.m_nMin_Array[nTraceIndex];
                }
                else
                {
                    if (cReferenceData.m_nMax_Array[nTraceIndex] > nMaxValue)
                        nMaxValue = cReferenceData.m_nMax_Array[nTraceIndex];

                    if (cReferenceData.m_nMin_Array[nTraceIndex] < nMinValue)
                        nMinValue = cReferenceData.m_nMin_Array[nTraceIndex];
                }

                dMeanValue += cReferenceData.m_dMean_Array[nTraceIndex];
            }

            dMeanValue = Math.Round(dMeanValue / cReferenceData.m_dMean_Array.Length, 2, MidpointRounding.AwayFromZero);

            cReferenceValue.m_nTotalMax = nMaxValue;
            cReferenceValue.m_nInnerMax = nInnerMaxValue;
            cReferenceValue.m_nEdgeMax = nEdgeMaxValue;
            cReferenceValue.m_nTotalMin = nMinValue;
            cReferenceValue.m_dTotalMean = dMeanValue;

            if (cReferenceValue.m_nTotalMax < m_nPartNumber)
                m_nPartNumber = cReferenceValue.m_nTotalMax;

            int nValueOfPart = (int)(cReferenceValue.m_nTotalMax / m_nPartNumber);
            int nRealPartNumber = Convert.ToInt32(Math.Ceiling((double)cReferenceValue.m_nTotalMax / nValueOfPart));

            if (cReferenceValue.m_nTotalMax % nValueOfPart == 0)
                nRealPartNumber++;

            List<double> dValue_List = new List<double>();

            if (dValue_List.Count > 0)
            {
                double dMinValue = dValue_List.Min();

                if (dMinValue < 0)
                {
                    double dDifferValue = Math.Abs(Math.Round(0 - dMinValue, 2, MidpointRounding.AwayFromZero));
                    int nNegativePartNumber = Convert.ToInt32(Math.Ceiling(dDifferValue / nValueOfPart));

                    if (dDifferValue % nValueOfPart != 0)
                        nNegativePartNumber++;

                    nRealPartNumber += nNegativePartNumber;
                }
            }

            cReferenceValue.m_nRealPartNumber = nRealPartNumber;
            cReferenceValue.m_nValueOfPart = nValueOfPart;

            if (m_bGetDistributionDataFlag == true)
            {
                if (bRXTraceTypeFlag == true)
                {
                    List<int> nValue_List = new List<int>();

                    for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                    {
                        for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
                            nValue_List.Add(m_nRXOriginalData_List[nDataIndex][nTraceIndex]);
                    }

                    double dStdevValue = Math.Round(MathMethod.ComputeStd(nValue_List), 2, MidpointRounding.AwayFromZero);
                    cReferenceValue.m_dTotalStd = dStdevValue;
                }
                else
                {
                    List<int> nValue_List = new List<int>();

                    for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                    {
                        for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
                            nValue_List.Add(m_nTXOriginalData_List[nDataIndex][nTraceIndex]);
                    }

                    double dStdevValue = Math.Round(MathMethod.ComputeStd(nValue_List), 2, MidpointRounding.AwayFromZero);
                    cReferenceValue.m_dTotalStd = dStdevValue;
                }
            }

            if (bRXTraceTypeFlag == true)
                cDataValue.m_cRXReferenceValue = cReferenceValue;
            else
                cDataValue.m_cTXReferenceValue = cReferenceValue;
        }

        protected void ComputeResultData(DataValue cDataValue, int nDataCount, bool bRXTraceTypeFlag)
        {
            if (bRXTraceTypeFlag == true)
            {
                double dEdgeMaxReferenceData = 0.0;
                int nEdgeMaxIndex = 0;
                double dInnerMaxReferenceData = 0.0;
                int nInnerMaxIndex = 0;
                double dTotalMaxReferenceData = 0.0;
                int nTotalMaxIndex = 0;

                if (m_bGetRXReportDataFlag == true && nDataCount > 0)
                {
                    for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                    {
                        if (nTraceIndex < m_nEdgeTraceNumber || nTraceIndex >= m_nRXTraceNumber - m_nEdgeTraceNumber)
                        {
                            if (nTraceIndex == 0)
                            {
                                dEdgeMaxReferenceData = cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex];
                                nEdgeMaxIndex = nTraceIndex;
                            }
                            else
                            {
                                if (cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex] > dEdgeMaxReferenceData)
                                {
                                    dEdgeMaxReferenceData = cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex];
                                    nEdgeMaxIndex = nTraceIndex;
                                }
                            }
                        }
                        else
                        {
                            if (nTraceIndex == m_nEdgeTraceNumber)
                            {
                                dInnerMaxReferenceData = cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex];
                                nInnerMaxIndex = nTraceIndex;
                            }
                            else
                            {
                                if (cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex] > dInnerMaxReferenceData)
                                {
                                    dInnerMaxReferenceData = cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex];
                                    nInnerMaxIndex = nTraceIndex;
                                }
                            }
                        }
                    }

                    if (dEdgeMaxReferenceData > dInnerMaxReferenceData)
                    {
                        dTotalMaxReferenceData = dEdgeMaxReferenceData;
                        nTotalMaxIndex = nEdgeMaxIndex;
                    }
                    else
                    {
                        dTotalMaxReferenceData = dInnerMaxReferenceData;
                        nTotalMaxIndex = nInnerMaxIndex;
                    }

                    cDataValue.m_cRXResultData.m_dEdgeMaxReferenceData = dEdgeMaxReferenceData;
                    cDataValue.m_cRXResultData.m_nEdgeMaxIndex = nEdgeMaxIndex;
                    cDataValue.m_cRXResultData.m_dInnerMaxReferenceData = dInnerMaxReferenceData;
                    cDataValue.m_cRXResultData.m_nInnerMaxIndex = nInnerMaxIndex;
                    cDataValue.m_cRXResultData.m_dTotalMaxReferenceData = dTotalMaxReferenceData;
                    cDataValue.m_cRXResultData.m_nTotalMaxIndex = nTotalMaxIndex;
                }
            }
            else
            {
                double dEdgeMaxReferenceData = 0.0;
                int nEdgeMaxIndex = 0;
                double dInnerMaxReferenceData = 0.0;
                int nInnerMaxIndex = 0;
                double dTotalMaxReferenceData = 0.0;
                int nTotalMaxIndex = 0;

                if (m_bGetTXReportDataFlag == true && nDataCount > 0)
                {
                    for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                    {
                        if (nTraceIndex < m_nEdgeTraceNumber || nTraceIndex >= m_nTXTraceNumber - m_nEdgeTraceNumber)
                        {
                            if (nTraceIndex == 0)
                            {
                                dEdgeMaxReferenceData = cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex];
                                nEdgeMaxIndex = nTraceIndex;
                            }
                            else
                            {
                                if (cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex] > dEdgeMaxReferenceData)
                                {
                                    dEdgeMaxReferenceData = cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex];
                                    nEdgeMaxIndex = nTraceIndex;
                                }
                            }
                        }
                        else
                        {
                            if (nTraceIndex == m_nEdgeTraceNumber)
                            {
                                dInnerMaxReferenceData = cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex];
                                nInnerMaxIndex = nTraceIndex;
                            }
                            else
                            {
                                if (cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex] > dInnerMaxReferenceData)
                                {
                                    dInnerMaxReferenceData = cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex];
                                    nInnerMaxIndex = nTraceIndex;
                                }
                            }
                        }
                    }

                    if (dEdgeMaxReferenceData > dInnerMaxReferenceData)
                    {
                        dTotalMaxReferenceData = dEdgeMaxReferenceData;
                        nTotalMaxIndex = nEdgeMaxIndex;
                    }
                    else
                    {
                        dTotalMaxReferenceData = dInnerMaxReferenceData;
                        nTotalMaxIndex = nInnerMaxIndex;
                    }

                    cDataValue.m_cTXResultData.m_dEdgeMaxReferenceData = dEdgeMaxReferenceData;
                    cDataValue.m_cTXResultData.m_nEdgeMaxIndex = nEdgeMaxIndex;
                    cDataValue.m_cTXResultData.m_dInnerMaxReferenceData = dInnerMaxReferenceData;
                    cDataValue.m_cTXResultData.m_nInnerMaxIndex = nInnerMaxIndex;
                    cDataValue.m_cTXResultData.m_dTotalMaxReferenceData = dTotalMaxReferenceData;
                    cDataValue.m_cTXResultData.m_nTotalMaxIndex = nTotalMaxIndex;
                }
            }
        }

        protected void WriteIntegrationDataFile(DataValue cDataValue, int nRXDataCount, int nTXDataCount)
        {
            ReferenceValue m_cRXReferenceValue = cDataValue.m_cRXReferenceValue;
            ReferenceValue m_cTXReferenceValue = cDataValue.m_cTXReferenceValue;

            string[] sValueTypeName_Array = new string[3] 
            { 
                SpecificText.m_sTotalMax, 
                SpecificText.m_sTotalMin, 
                SpecificText.m_sTotalMean 
            };

            double[] dRXValueTypeValue_Array = new double[3] 
            { 
                m_cRXReferenceValue.m_nTotalMax, 
                m_cRXReferenceValue.m_nTotalMin, 
                m_cRXReferenceValue.m_dTotalMean 
            };

            double[] dTXValueTypeValue_Array = new double[3] 
            { 
                m_cTXReferenceValue.m_nTotalMax, 
                m_cTXReferenceValue.m_nTotalMin, 
                m_cTXReferenceValue.m_dTotalMean 
            };

            string[] sNoiseValueTypeName_Array = new string[7] 
            { 
                SpecificText.m_sMax_MeanPlus3Std,
                SpecificText.m_sMax_MeanPlus3Std_Trace,
                SpecificText.m_sMax_MeanPlus3Std_Location,
                SpecificText.m_sEdge_Max_MeanPlus3Std,
                SpecificText.m_sEdge_Max_MeanPlus3Std_Trace,
                SpecificText.m_sInner_Max_MeanPlus3Std,
                SpecificText.m_sInner_Max_MeanPlus3Std_Trace 
            };

            FileStream fs = new FileStream(m_sIntegrationFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_INTEGRATION);

                if (m_bGetRXReportDataFlag == true)
                {
                    sw.WriteLine();

                    sw.WriteLine("RX Analysis Data");

                    for (int nIndex = 0; nIndex < sValueTypeName_Array.Length; nIndex++)
                    {
                        sw.Write(string.Format("{0},", sValueTypeName_Array[nIndex]));
                        sw.WriteLine(string.Format("{0}", dRXValueTypeValue_Array[nIndex].ToString()));
                    }
                }

                if (m_bGetTXReportDataFlag == true)
                {
                    sw.WriteLine();

                    sw.WriteLine("TX Analysis Data");

                    for (int nIndex = 0; nIndex < sValueTypeName_Array.Length; nIndex++)
                    {
                        sw.Write(string.Format("{0},", sValueTypeName_Array[nIndex]));
                        sw.WriteLine(string.Format("{0}", dTXValueTypeValue_Array[nIndex].ToString()));
                    }
                }

                if (m_bGetRXReportDataFlag == true)
                {
                    sw.WriteLine();

                    sw.WriteLine("RX EdgeInner Analysis Data");

                    for (int nValueIndex = 0; nValueIndex < sNoiseValueTypeName_Array.Length; nValueIndex++)
                    {
                        sw.Write(string.Format("{0},", sNoiseValueTypeName_Array[nValueIndex]));

                        switch (sNoiseValueTypeName_Array[nValueIndex])
                        {
                            case SpecificText.m_sMax_MeanPlus3Std:
                                sw.WriteLine(cDataValue.m_cRXResultData.m_dTotalMaxReferenceData);
                                break;
                            case SpecificText.m_sMax_MeanPlus3Std_Trace:
                                sw.WriteLine(cDataValue.m_cRXResultData.m_nTotalMaxIndex + 1);
                                break;
                            case SpecificText.m_sMax_MeanPlus3Std_Location:
                                if (cDataValue.m_cRXResultData.m_nTotalMaxIndex < m_nEdgeTraceNumber || cDataValue.m_cRXResultData.m_nTotalMaxIndex >= m_nRXTraceNumber - m_nEdgeTraceNumber)
                                    sw.WriteLine("Edge");
                                else
                                    sw.WriteLine("Inner");

                                break;
                            case SpecificText.m_sEdge_Max_MeanPlus3Std:
                                sw.WriteLine(cDataValue.m_cRXResultData.m_dEdgeMaxReferenceData);
                                break;
                            case SpecificText.m_sEdge_Max_MeanPlus3Std_Trace:
                                sw.WriteLine(cDataValue.m_cRXResultData.m_nEdgeMaxIndex + 1);
                                break;
                            case SpecificText.m_sInner_Max_MeanPlus3Std:
                                sw.WriteLine(cDataValue.m_cRXResultData.m_dInnerMaxReferenceData);
                                break;
                            case SpecificText.m_sInner_Max_MeanPlus3Std_Trace:
                                sw.WriteLine(cDataValue.m_cRXResultData.m_nInnerMaxIndex + 1);
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (m_bGetTXReportDataFlag == true)
                {
                    sw.WriteLine();

                    sw.WriteLine("TX EdgeInner Analysis Data");

                    for (int nValueIndex = 0; nValueIndex < sNoiseValueTypeName_Array.Length; nValueIndex++)
                    {
                        sw.Write(string.Format("{0},", sNoiseValueTypeName_Array[nValueIndex]));

                        switch (sNoiseValueTypeName_Array[nValueIndex])
                        {
                            case SpecificText.m_sMax_MeanPlus3Std:
                                sw.WriteLine(cDataValue.m_cTXResultData.m_dTotalMaxReferenceData);
                                break;
                            case SpecificText.m_sMax_MeanPlus3Std_Trace:
                                sw.WriteLine(cDataValue.m_cTXResultData.m_nTotalMaxIndex + 1);
                                break;
                            case SpecificText.m_sMax_MeanPlus3Std_Location:
                                if (cDataValue.m_cTXResultData.m_nTotalMaxIndex < m_nEdgeTraceNumber || cDataValue.m_cTXResultData.m_nTotalMaxIndex >= m_nTXTraceNumber - m_nEdgeTraceNumber)
                                    sw.WriteLine("Edge");
                                else
                                    sw.WriteLine("Inner");

                                break;
                            case SpecificText.m_sEdge_Max_MeanPlus3Std:
                                sw.WriteLine(cDataValue.m_cTXResultData.m_dEdgeMaxReferenceData);
                                break;
                            case SpecificText.m_sEdge_Max_MeanPlus3Std_Trace:
                                sw.WriteLine(cDataValue.m_cTXResultData.m_nEdgeMaxIndex + 1);
                                break;
                            case SpecificText.m_sInner_Max_MeanPlus3Std:
                                sw.WriteLine(cDataValue.m_cTXResultData.m_dInnerMaxReferenceData);
                                break;
                            case SpecificText.m_sInner_Max_MeanPlus3Std_Trace:
                                sw.WriteLine(cDataValue.m_cTXResultData.m_nInnerMaxIndex + 1);
                                break;
                            default:
                                break;
                        }
                    }
                }

                sw.WriteLine();

                sw.WriteLine("Information");
                sw.WriteLine(string.Format("RX Data Number,{0}", nRXDataCount));
                sw.WriteLine(string.Format("TX Data Number,{0}", nTXDataCount));
                sw.WriteLine(string.Format("ErrorFlag,{0}", m_nErrorFlag));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        protected void ComputeDistributionData(DataValue cDataValue, int nDataCount, bool bRXTraceTypeFlag)
        {
            int nTraceNumber = (bRXTraceTypeFlag == true) ? m_nRXTraceNumber : m_nTXTraceNumber;
            ReferenceData cReferenceData = (bRXTraceTypeFlag == true) ? cDataValue.m_cRXReferenceData : cDataValue.m_cTXReferenceData;
            List<List<int>> nOriginalData_List = (bRXTraceTypeFlag == true) ? m_nRXOriginalData_List : m_nTXOriginalData_List;
            DistributionData cDistributionData = (bRXTraceTypeFlag == true) ? cDataValue.m_cRXDistributionData : cDataValue.m_cTXDistributionData;

            for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
            {
                double dMean = cReferenceData.m_dMean_Array[nTraceIndex];
                double dStd = cReferenceData.m_dStd_Array[nTraceIndex];

                double dMeanPlus1Std = dMean + dStd;
                double dMeanPlus2Std = dMean + 2 * dStd;
                double dMeanPlus3Std = dMean + 3 * dStd;
                double dMeanMinus1Std = dMean - dStd;
                double dMeanMinus2Std = dMean - 2 * dStd;
                double dMeanMinus3Std = dMean - 3 * dStd;

                int nOver3StdCount = 0;
                int nInRange3StdCount = 0;
                int nInRange2StdCount = 0;
                int nInRange1StdCount = 0;

                for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
                {
                    double dValue = Convert.ToDouble(nOriginalData_List[nDataIndex][nTraceIndex]);

                    if (dValue > dMeanPlus3Std || dValue < dMeanMinus3Std)
                        nOver3StdCount++;

                    if (dValue < dMeanPlus3Std && dValue > dMeanMinus3Std)
                        nInRange3StdCount++;

                    if (dValue < dMeanPlus2Std && dValue > dMeanMinus2Std)
                        nInRange2StdCount++;

                    if (dValue < dMeanPlus1Std && dValue > dMeanMinus1Std)
                        nInRange1StdCount++;
                }

                cDistributionData.m_dOver3Std_Array[nTraceIndex] = Math.Round(((double)nOver3StdCount / nDataCount) * 100, 2, MidpointRounding.AwayFromZero);
                cDistributionData.m_dInRange3Std_Array[nTraceIndex] = Math.Round(((double)nInRange3StdCount / nDataCount) * 100, 2, MidpointRounding.AwayFromZero);
                cDistributionData.m_dInRange2Std_Array[nTraceIndex] = Math.Round(((double)nInRange2StdCount / nDataCount) * 100, 2, MidpointRounding.AwayFromZero);
                cDistributionData.m_dInRange1Std_Array[nTraceIndex] = Math.Round(((double)nInRange1StdCount / nDataCount) * 100, 2, MidpointRounding.AwayFromZero);
            }
        }

        protected void WriteDistributionDataFile(DataValue cDataValue, string sFilePath)
        {
            string[] sValueTypeName_Array = new string[5] 
            { 
                SpecificText.m_sMeanPlus3Std,
                SpecificText.m_sOver_3Std_HLBPercent,
                SpecificText.m_sInRange_3Std_HLBPercent,
                SpecificText.m_sInRange_2Std_HLBPercent,
                SpecificText.m_sInRange_1Std_HLBPercent
            };

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_PROCESS);

                if (m_bGetRXReportDataFlag == true)
                {
                    double[,] dRXDistributionData_Array = new double[5, m_nRXTraceNumber];

                    for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                    {
                        dRXDistributionData_Array[0, nTraceIndex] = cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex];
                        dRXDistributionData_Array[1, nTraceIndex] = cDataValue.m_cRXDistributionData.m_dOver3Std_Array[nTraceIndex];
                        dRXDistributionData_Array[2, nTraceIndex] = cDataValue.m_cRXDistributionData.m_dInRange3Std_Array[nTraceIndex];
                        dRXDistributionData_Array[3, nTraceIndex] = cDataValue.m_cRXDistributionData.m_dInRange2Std_Array[nTraceIndex];
                        dRXDistributionData_Array[4, nTraceIndex] = cDataValue.m_cRXDistributionData.m_dInRange1Std_Array[nTraceIndex];
                    }

                    sw.WriteLine("RX Normal Distribution Data");
                    WriteTraceNumberToCSVFile(sw, m_nRXTraceNumber);
                    WriteDataArrayToCSVFile(sw, sValueTypeName_Array, dRXDistributionData_Array);

                    sw.WriteLine();
                }

                if (m_bGetTXReportDataFlag == true)
                {
                    double[,] dTXDistributionData_Array = new double[5, m_nTXTraceNumber];

                    for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                    {
                        dTXDistributionData_Array[0, nTraceIndex] = cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex];
                        dTXDistributionData_Array[1, nTraceIndex] = cDataValue.m_cTXDistributionData.m_dOver3Std_Array[nTraceIndex];
                        dTXDistributionData_Array[2, nTraceIndex] = cDataValue.m_cTXDistributionData.m_dInRange3Std_Array[nTraceIndex];
                        dTXDistributionData_Array[3, nTraceIndex] = cDataValue.m_cTXDistributionData.m_dInRange2Std_Array[nTraceIndex];
                        dTXDistributionData_Array[4, nTraceIndex] = cDataValue.m_cTXDistributionData.m_dInRange1Std_Array[nTraceIndex];
                    }

                    sw.WriteLine("TX Normal Distribution Data");
                    WriteTraceNumberToCSVFile(sw, m_nTXTraceNumber);
                    WriteDataArrayToCSVFile(sw, sValueTypeName_Array, dTXDistributionData_Array);

                    sw.WriteLine();
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        protected void ComputeHistogramData(DataValue cDataValue, int nDataCount, bool bRXTraceTypeFlag)
        {
            int nTraceNumber = (bRXTraceTypeFlag == true) ? m_nRXTraceNumber : m_nTXTraceNumber;
            int nRealPartNumber = (bRXTraceTypeFlag == true) ? cDataValue.m_cRXReferenceValue.m_nRealPartNumber : cDataValue.m_cTXReferenceValue.m_nRealPartNumber;
            int nValueOfPart = (bRXTraceTypeFlag == true) ? cDataValue.m_cRXReferenceValue.m_nValueOfPart : cDataValue.m_cTXReferenceValue.m_nValueOfPart;

            DistributionData cDistributionData = (bRXTraceTypeFlag == true) ? cDataValue.m_cRXDistributionData : cDataValue.m_cTXDistributionData;
            List<List<int>> nOriginalData_List = (bRXTraceTypeFlag == true) ? m_nRXOriginalData_List : m_nTXOriginalData_List;

            for (int nPartIndex = 0; nPartIndex < nRealPartNumber; nPartIndex++)
                cDistributionData.m_nHistogramScale_Array[nPartIndex] = (nPartIndex + 1) * nValueOfPart;

            for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
            {
                for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                {
                    int nPartNumber = (int)(nOriginalData_List[nDataIndex][nTraceIndex] / nValueOfPart);
                    cDistributionData.m_nHistogramAmount_Array[nPartNumber]++;
                }
            }
        }

        protected void WriteHistogramDataFile(DataValue cDataValue, string sFilePath)
        {
            string[] sValueTypeName_Array = new string[2] 
            { 
                SpecificText.m_sScale,
                SpecificText.m_sAmount
            };

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_PROCESS);

                sw.WriteLine("Total Histogram Data");

                if (m_bGetRXReportDataFlag == true)
                {
                    WriteHistogramDataToCSVFile(sw, "RX", cDataValue.m_cRXReferenceValue);

                    sw.WriteLine();

                    sw.WriteLine("RX Total Histogram Data");
                    WriteHistogramDataToCSVFile(sw, sValueTypeName_Array, cDataValue.m_cRXDistributionData.m_nHistogramScale_Array, cDataValue.m_cRXDistributionData.m_nHistogramAmount_Array);

                    sw.WriteLine();
                }

                if (m_bGetTXReportDataFlag == true)
                {
                    WriteHistogramDataToCSVFile(sw, "TX", cDataValue.m_cTXReferenceValue);

                    sw.WriteLine();

                    sw.WriteLine("TX Total Histogram Data");
                    WriteHistogramDataToCSVFile(sw, sValueTypeName_Array, cDataValue.m_cTXDistributionData.m_nHistogramScale_Array, cDataValue.m_cTXDistributionData.m_nHistogramAmount_Array);

                    sw.WriteLine();
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WriteHistogramDataToCSVFile(StreamWriter sw, string[] sValueTypeName_Array, int[] nScaleData_Array, int[] nAmountData_Array)
        {
            for (int nTypeIndex = 0; nTypeIndex < sValueTypeName_Array.Length; nTypeIndex++)
            {
                sw.Write(string.Format("{0},", sValueTypeName_Array[nTypeIndex]));

                if (sValueTypeName_Array[nTypeIndex] == SpecificText.m_sScale)
                {
                    for (int nDataIndex = 0; nDataIndex < nScaleData_Array.Length; nDataIndex++)
                    {
                        if (nDataIndex == 0)
                            sw.Write(string.Format("{0}~{1}", 0, nScaleData_Array[nDataIndex]));
                        else
                            sw.Write(string.Format("{0}~{1}", nScaleData_Array[nDataIndex - 1] + 1, nScaleData_Array[nDataIndex]));

                        if (nDataIndex < nScaleData_Array.Length - 1)
                            sw.Write(",");
                        else
                            sw.WriteLine();
                    }
                }
                else if (sValueTypeName_Array[nTypeIndex] == SpecificText.m_sAmount)
                {
                    for (int nDataIndex = 0; nDataIndex < nAmountData_Array.Length; nDataIndex++)
                    {
                        sw.Write(string.Format("{0}", nAmountData_Array[nDataIndex]));

                        if (nDataIndex < nAmountData_Array.Length - 1)
                            sw.Write(",");
                        else
                            sw.WriteLine();
                    }
                }
            }
        }

        protected void SaveHistogramChartPicture(DataValue cDataValue, string sFolderPath, bool bRXTraceTypeFlag)
        {
            HistogramInfo cHistogramInfo = new HistogramInfo(1200, 320);

            int nMaxAmount = 0;
            int nMaxScaleValue = 0;
            int[] nScale_Array, nAmount_Array;
            ReferenceValue cReferenceValue;

            if (bRXTraceTypeFlag == true)
            {
                nScale_Array = cDataValue.m_cRXDistributionData.m_nHistogramScale_Array;
                nAmount_Array = cDataValue.m_cRXDistributionData.m_nHistogramAmount_Array;
                cReferenceValue = cDataValue.m_cRXReferenceValue;
                cHistogramInfo.m_nTraceType = MainConstantParameter.m_nTRACETYPE_RX;
            }
            else
            {
                nScale_Array = cDataValue.m_cTXDistributionData.m_nHistogramScale_Array;
                nAmount_Array = cDataValue.m_cTXDistributionData.m_nHistogramAmount_Array;
                cReferenceValue = cDataValue.m_cTXReferenceValue;
                cHistogramInfo.m_nTraceType = MainConstantParameter.m_nTRACETYPE_TX;
            }

            nMaxScaleValue = GetMaximumScaleValue(nMaxAmount, nAmount_Array) + 1;

            SaveHistogramChart(cHistogramInfo, cReferenceValue, nScale_Array, nAmount_Array, nMaxScaleValue, sFolderPath);
        }

        private int GetMaximumScaleValue(int nMaxAmount, int[] nAmount_Array)
        {
            for (int nDataIndex = 0; nDataIndex < nAmount_Array.Length; nDataIndex++)
            {
                if (nAmount_Array[nDataIndex] > nMaxAmount)
                    nMaxAmount = nAmount_Array[nDataIndex];
            }

            int nLength = (int)Math.Log10(nMaxAmount) + 1;
            int nMaxScaleValue = ((int)(nMaxAmount / (Math.Pow(10, (nLength - 1)))) + 1) * (int)(Math.Pow(10, (nLength - 1))) - 1;

            return nMaxScaleValue;
        }

        private void SaveHistogramChart(HistogramInfo cHistogramInfo, ReferenceValue cReferenceValue, int[] nScale_Array, int[] nAmount_Array, int nMaxScaleValue, string sFolderPath)
        {
            string sTitleName = "";
            string sFilePath = "";
            string sTraceType = (cHistogramInfo.m_nTraceType == MainConstantParameter.m_nTRACETYPE_RX) ? StringConvert.m_sTRACETYPE_RX : StringConvert.m_sTRACETYPE_TX;

            sTitleName = string.Format("{0} Total Histogram", sTraceType);
            sFilePath = string.Format(@"{0}\{1}_{2}", sFolderPath, sTraceType, SpecificText.m_sHistogramFileName);

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
            cChart.ChartAreas[0].AxisX.Maximum = nScale_Array[nScale_Array.Length - 1];
            cChart.ChartAreas[0].AxisX.Minimum = 0;
            cChart.ChartAreas[0].AxisX.Interval = cReferenceValue.m_nValueOfPart;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            cChart.ChartAreas[0].AxisX.Title = "Value";
            serSeries.IsValueShownAsLabel = true;

            for (int nDataIndex = 0; nDataIndex < nScale_Array.Length; nDataIndex++)
            {
                double dScale = 0;

                if (nDataIndex == 0)
                    dScale = (double)nScale_Array[nDataIndex] / 2;
                else
                    dScale = ((double)nScale_Array[nDataIndex - 1] + (double)nScale_Array[nDataIndex]) / 2;

                serSeries.Points.AddXY(dScale, nAmount_Array[nDataIndex]);
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

        protected void CheckRecordErrorCodeHasValue(DataInfo cDataInfo, string sFilePath)
        {
            if (cDataInfo.m_sRecordErrorCode != "")
            {
                cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0800;

                m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", cDataInfo.m_sRecordErrorMessage, Path.GetFileName(sFilePath));
                m_cErrorInfo.m_sRecordErrorMessage = cDataInfo.m_sRecordErrorMessage;
                m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
            }
        }

        protected void WriteReferenceDataFile(bool bRXTraceTypeFlag)
        {
            string[] sTitleName_Array = new string[6] 
            { 
                SpecificText.m_sIndex,
                SpecificText.m_sFileName,
                SpecificText.m_sFrequency, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                "",
            };

            string sFilePath = m_sRXReferenceFilePath;
            int nTraceNumber = m_nRXTraceNumber;

            if (bRXTraceTypeFlag == true)
            {
                sFilePath = m_sRXReferenceFilePath;
                nTraceNumber = m_nRXTraceNumber;
            }
            else
            {
                sFilePath = m_sTXReferenceFilePath;
                nTraceNumber = m_nTXTraceNumber;
            }

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_REFERENCE);

                sw.WriteLine("Mean+3*Std Data");

                for (int nTitleIndex = 0; nTitleIndex < sTitleName_Array.Length; nTitleIndex++)
                    sw.Write(string.Format("{0},", sTitleName_Array[nTitleIndex]));

                for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < nTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex + 1));
                    else
                        sw.WriteLine(string.Format("{0}", nTraceIndex + 1));
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                    {
                        string sValue = GetReferenceData(m_cDataValue_List[nDataIndex], nTraceIndex, bRXTraceTypeFlag, ReferenceDataType.MeanPlus3Std);

                        if (nTraceIndex < nTraceNumber - 1)
                            sw.Write(string.Format("{0},", sValue));
                        else
                            sw.WriteLine(sValue);
                    }
                }

                sw.WriteLine();

                sw.WriteLine("Mean Data");

                for (int nTitleIndex = 0; nTitleIndex < sTitleName_Array.Length; nTitleIndex++)
                    sw.Write(string.Format("{0},", sTitleName_Array[nTitleIndex]));

                for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < nTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex + 1));
                    else
                        sw.WriteLine(string.Format("{0}", nTraceIndex + 1));
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                    {
                        string sValue = GetReferenceData(m_cDataValue_List[nDataIndex], nTraceIndex, bRXTraceTypeFlag, ReferenceDataType.Mean);

                        if (nTraceIndex < nTraceNumber - 1)
                            sw.Write(string.Format("{0},", sValue));
                        else
                            sw.WriteLine(sValue);
                    }
                }

                sw.WriteLine();

                sw.WriteLine("Std Data");

                for (int nTitleIndex = 0; nTitleIndex < sTitleName_Array.Length; nTitleIndex++)
                    sw.Write(string.Format("{0},", sTitleName_Array[nTitleIndex]));

                for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < nTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex + 1));
                    else
                        sw.WriteLine(string.Format("{0}", nTraceIndex + 1));
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                    {
                        string sValue = GetReferenceData(m_cDataValue_List[nDataIndex], nTraceIndex, bRXTraceTypeFlag, ReferenceDataType.Std);

                        if (nTraceIndex < nTraceNumber - 1)
                            sw.Write(string.Format("{0},", sValue));
                        else
                            sw.WriteLine(sValue);
                    }
                }

                sw.WriteLine();

                sw.WriteLine("Max Data");

                for (int nTitleIndex = 0; nTitleIndex < sTitleName_Array.Length; nTitleIndex++)
                    sw.Write(string.Format("{0},", sTitleName_Array[nTitleIndex]));

                for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < nTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex + 1));
                    else
                        sw.WriteLine(string.Format("{0}", nTraceIndex + 1));
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                    {
                        string sValue = GetReferenceData(m_cDataValue_List[nDataIndex], nTraceIndex, bRXTraceTypeFlag, ReferenceDataType.Max);

                        if (nTraceIndex < nTraceNumber - 1)
                            sw.Write(string.Format("{0},", sValue));
                        else
                            sw.WriteLine(sValue);
                    }
                }

                sw.WriteLine();

                sw.WriteLine("Min Data");

                for (int nTitleIndex = 0; nTitleIndex < sTitleName_Array.Length; nTitleIndex++)
                    sw.Write(string.Format("{0},", sTitleName_Array[nTitleIndex]));

                for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < nTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex + 1));
                    else
                        sw.WriteLine(string.Format("{0}", nTraceIndex + 1));
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                    {
                        string sValue = GetReferenceData(m_cDataValue_List[nDataIndex], nTraceIndex, bRXTraceTypeFlag, ReferenceDataType.Min);

                        if (nTraceIndex < nTraceNumber - 1)
                            sw.Write(string.Format("{0},", sValue));
                        else
                            sw.WriteLine(sValue);
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

        private string GetReferenceData(DataValue cDataValue, int nTraceIndex, bool bRXTraceTypeFlag, ReferenceDataType eReferenceDataType)
        {
            string sValue = "";

            if (bRXTraceTypeFlag == true)
            {
                switch (eReferenceDataType)
                {
                    case ReferenceDataType.MeanPlus3Std:
                        sValue = Convert.ToString(cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex]);
                        break;
                    case ReferenceDataType.Mean:
                        sValue = Convert.ToString(cDataValue.m_cRXReferenceData.m_dMean_Array[nTraceIndex]);
                        break;
                    case ReferenceDataType.Std:
                        sValue = Convert.ToString(cDataValue.m_cRXReferenceData.m_dStd_Array[nTraceIndex]);
                        break;
                    case ReferenceDataType.Max:
                        sValue = Convert.ToString(cDataValue.m_cRXReferenceData.m_nMax_Array[nTraceIndex]);
                        break;
                    case ReferenceDataType.Min:
                        sValue = Convert.ToString(cDataValue.m_cRXReferenceData.m_nMin_Array[nTraceIndex]);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (eReferenceDataType)
                {
                    case ReferenceDataType.MeanPlus3Std:
                        sValue = Convert.ToString(cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex]);
                        break;
                    case ReferenceDataType.Mean:
                        sValue = Convert.ToString(cDataValue.m_cTXReferenceData.m_dMean_Array[nTraceIndex]);
                        break;
                    case ReferenceDataType.Std:
                        sValue = Convert.ToString(cDataValue.m_cTXReferenceData.m_dStd_Array[nTraceIndex]);
                        break;
                    case ReferenceDataType.Max:
                        sValue = Convert.ToString(cDataValue.m_cTXReferenceData.m_nMax_Array[nTraceIndex]);
                        break;
                    case ReferenceDataType.Min:
                        sValue = Convert.ToString(cDataValue.m_cTXReferenceData.m_nMin_Array[nTraceIndex]);
                        break;
                    default:
                        break;
                }
            }

            return sValue;
        }

        protected void WriteTotalReferenceDataFile()
        {
            string[] sTitleName_Array = new string[10] 
            { 
                SpecificText.m_sIndex,
                SpecificText.m_sFileName,
                SpecificText.m_sFrequency, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                "",
                SpecificText.m_sRX_Inner, 
                SpecificText.m_sTX_Inner, 
                SpecificText.m_sRX_Edge, 
                SpecificText.m_sTX_Edge,
            };

            FileStream fs = new FileStream(m_sReferenceFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_REFERENCE);

                for (int nTitleIndex = 0; nTitleIndex < sTitleName_Array.Length; nTitleIndex++)
                {
                    sw.Write(string.Format("{0}", sTitleName_Array[nTitleIndex]));

                    if (nTitleIndex < sTitleName_Array.Length - 1)
                        sw.Write(",");
                    else
                        sw.WriteLine();
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXResultData.m_dInnerMaxReferenceData));
                    sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXResultData.m_dInnerMaxReferenceData));
                    sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cRXResultData.m_dEdgeMaxReferenceData));
                    sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_cTXResultData.m_dEdgeMaxReferenceData));
                    sw.WriteLine();
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        protected void ComputeWeightingRank(bool bTotalDataFlag = false)
        {
            m_bAllOverInnerReferenceValueHBFlag = false;

            double dEdgeSS_OFF_InnerRXWeightRatio = Convert.ToDouble(m_nEdgeSS_OFF_InnerRXWeightingPercent) / 100;
            double dEdgeSS_OFF_InnerTXWeightRatio = Convert.ToDouble(m_nEdgeSS_OFF_InnerTXWeightingPercent) / 100;
            double dEdgeSS_OFF_EdgeRXWeightRatio = Convert.ToDouble(m_nEdgeSS_OFF_EdgeRXWeightingPercent) / 100;
            double dEdgeSS_OFF_EdgeTXWeightRatio = Convert.ToDouble(m_nEdgeSS_OFF_EdgeTXWeightingPercent) / 100;

            List<DataInfo> cDataInfo_List;
            List<DataValue> cDataValue_List;

            if (bTotalDataFlag == false)
            {
                cDataInfo_List = m_cDataInfo_List;
                cDataValue_List = m_cDataValue_List;
            }
            else
            {
                cDataInfo_List = m_cTotalDataInfo_List;
                cDataValue_List = m_cTotalDataValue_List;
            }

            foreach (DataValue cDataValue in cDataValue_List)
            {
                double dWeightingScore = Math.Round(cDataValue.m_cRXResultData.m_dInnerMaxReferenceData * dEdgeSS_OFF_InnerRXWeightRatio +
                                                    cDataValue.m_cTXResultData.m_dInnerMaxReferenceData * dEdgeSS_OFF_InnerTXWeightRatio +
                                                    cDataValue.m_cRXResultData.m_dEdgeMaxReferenceData * dEdgeSS_OFF_EdgeRXWeightRatio +
                                                    cDataValue.m_cTXResultData.m_dEdgeMaxReferenceData * dEdgeSS_OFF_EdgeTXWeightRatio,
                                                    2, MidpointRounding.AwayFromZero);

                cDataValue.m_dWeightingInnerTXValue = Math.Round(cDataValue.m_cTXResultData.m_dInnerMaxReferenceData * dEdgeSS_OFF_InnerTXWeightRatio, 2, MidpointRounding.AwayFromZero);
                cDataValue.m_dWeightingInnerRXValue = Math.Round(cDataValue.m_cRXResultData.m_dInnerMaxReferenceData * dEdgeSS_OFF_InnerRXWeightRatio, 2, MidpointRounding.AwayFromZero);
                cDataValue.m_dWeightingEdgeTXValue = Math.Round(cDataValue.m_cTXResultData.m_dEdgeMaxReferenceData * dEdgeSS_OFF_EdgeTXWeightRatio, 2, MidpointRounding.AwayFromZero);
                cDataValue.m_dWeightingEdgeRXValue = Math.Round(cDataValue.m_cRXResultData.m_dEdgeMaxReferenceData * dEdgeSS_OFF_EdgeRXWeightRatio, 2, MidpointRounding.AwayFromZero);
                cDataValue.m_dWeightingScore = dWeightingScore;

                if (cDataValue.m_cRXResultData.m_dInnerMaxReferenceData >= m_dInnerReferenceValueHB || cDataValue.m_cTXResultData.m_dInnerMaxReferenceData >= m_dInnerReferenceValueHB)
                    cDataValue.m_bInnerReferenceValueOverHBFlag = true;
            }

            cDataValue_List.Sort((x, y) => x.m_dWeightingScore.CompareTo(y.m_dWeightingScore));

            int nOverInnerReferenceValueHBCount = 0;

            for (int nDataIndex = 0; nDataIndex < cDataValue_List.Count; nDataIndex++)
            {
                DataValue cDataValue = cDataValue_List[nDataIndex];
                cDataValue.m_nRankIndex = nDataIndex + 1;

                DataInfo cDataInfo = cDataInfo_List.Find(x => (x.m_nReadPH1 == cDataValue.m_nPH1 && x.m_nReadPH2 == cDataValue.m_nPH2));

                cDataInfo.m_nRankIndex = nDataIndex + 1;

                if (cDataValue.m_bInnerReferenceValueOverHBFlag == true)
                {
                    string sErrorMessage = string.Format("Inner Reference Value Over High Boundary({0})", Convert.ToString(m_dInnerReferenceValueHB));
                    cDataInfo.m_sErrorMessage = sErrorMessage;
                    cDataInfo.m_bInnerReferenceValueOverHBFlag = true;
                    nOverInnerReferenceValueHBCount++;
                }
                else if (bTotalDataFlag == false)
                {
                    if (m_dMaxMinusMeanValueOverWarningStdevMagHB >= 0.0)
                    {
                        double dRXStdevMagnification = 0.0;
                        double dRXStdev = 0.0;

                        for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                        {
                            if (cDataValue.m_cRXReferenceData.m_nMax_Array[nTraceIndex] == cDataValue.m_cRXReferenceValue.m_nTotalMax)
                            {
                                double dMagnificationValue = Math.Round((cDataValue.m_cRXReferenceValue.m_nTotalMax - cDataValue.m_cRXReferenceData.m_dMean_Array[nTraceIndex]) / cDataValue.m_cRXReferenceData.m_dStd_Array[nTraceIndex], 2, MidpointRounding.AwayFromZero);

                                if (dMagnificationValue > dRXStdevMagnification)
                                {
                                    dRXStdevMagnification = dMagnificationValue;
                                    dRXStdev = cDataValue.m_cRXReferenceData.m_dStd_Array[nTraceIndex];
                                }
                            }
                        }

                        double dTXStdevMagnification = 0.0;
                        double dTXStdev = 0.0;

                        for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                        {
                            if (cDataValue.m_cTXReferenceData.m_nMax_Array[nTraceIndex] == cDataValue.m_cTXReferenceValue.m_nTotalMax)
                            {
                                double dMagnificationValue = Math.Round((cDataValue.m_cTXReferenceValue.m_nTotalMax - cDataValue.m_cTXReferenceData.m_dMean_Array[nTraceIndex]) / cDataValue.m_cTXReferenceData.m_dStd_Array[nTraceIndex], 2, MidpointRounding.AwayFromZero);

                                if (dMagnificationValue > dTXStdevMagnification)
                                {
                                    dTXStdevMagnification = dMagnificationValue;
                                    dTXStdev = cDataValue.m_cTXReferenceData.m_dStd_Array[nTraceIndex];
                                }
                            }
                        }

                        if (dRXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB || dTXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB)
                        {
                            string sRXTXMaxValue = "";

                            if (dRXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB && dTXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB)
                                sRXTXMaxValue = string.Format("RX:{0}({1}*Stdev+Mean) / TX:{2}({3}*Stdev+Mean)", cDataValue.m_cRXReferenceValue.m_nTotalMax, dRXStdevMagnification, cDataValue.m_cTXReferenceValue.m_nTotalMax, dTXStdevMagnification);
                            else if (dRXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB)
                                sRXTXMaxValue = string.Format("RX:{0}(={1}*Stdev+Mean)", cDataValue.m_cRXReferenceValue.m_nTotalMax, dRXStdevMagnification);
                            else if (dTXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB)
                                sRXTXMaxValue = string.Format("TX:{0}(={1}*Stdev+Mean)", cDataValue.m_cTXReferenceValue.m_nTotalMax, dTXStdevMagnification);

                            string sWarningMessage = string.Format("Max Value({0}) Over Warning Stdev Mag. HB({1}*Stdev)", sRXTXMaxValue, Convert.ToDouble(m_dMaxMinusMeanValueOverWarningStdevMagHB));
                            cDataInfo.m_sWarningMessage = sWarningMessage;
                        }
                    }
                    else if (m_dMaxValueOverWarningAbsValueHB >= 0.0)
                    {
                        if (cDataValue.m_cRXReferenceValue.m_nTotalMax >= m_dMaxValueOverWarningAbsValueHB || cDataValue.m_cTXReferenceValue.m_nTotalMax >= m_dMaxValueOverWarningAbsValueHB)
                        {
                            string sRXTXMaxValue = "";

                            if (cDataValue.m_cRXReferenceValue.m_nTotalMax >= m_dMaxValueOverWarningAbsValueHB && cDataValue.m_cTXReferenceValue.m_nTotalMax >= m_dMaxValueOverWarningAbsValueHB)
                                sRXTXMaxValue = string.Format("RX:{0} / TX:{1}", cDataValue.m_cRXReferenceValue.m_nTotalMax, cDataValue.m_cTXReferenceValue.m_nTotalMax);
                            else if (cDataValue.m_cRXReferenceValue.m_nTotalMax >= m_dMaxValueOverWarningAbsValueHB)
                                sRXTXMaxValue = string.Format("RX:{0}", cDataValue.m_cRXReferenceValue.m_nTotalMax);
                            else if (cDataValue.m_cTXReferenceValue.m_nTotalMax >= m_dMaxValueOverWarningAbsValueHB)
                                sRXTXMaxValue = string.Format("TX:{0}", cDataValue.m_cTXReferenceValue.m_nTotalMax);

                            cDataInfo.m_sWarningMessage = string.Format("Max Value({0}) Over Warning Abs. Value HB({1})", sRXTXMaxValue, Convert.ToString(m_dMaxValueOverWarningAbsValueHB));
                        }
                    }
                }

                if (bTotalDataFlag == true)
                {
                    if (m_dMaxMinusMeanValueOverWarningStdevMagHB >= 0.0)
                    {
                        double dPTHFRXStdevMagnification = 0.0;
                        double dPTHFRXStdev = 0.0;

                        for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                        {
                            if (cDataValue.m_cRXPreviousStepData.m_nMaxValue_Array[nTraceIndex] == cDataValue.m_cRXPreviousStepData.m_nTotalMaxValue)
                            {
                                double dMagnificationValue = Math.Round((cDataValue.m_cRXPreviousStepData.m_nTotalMaxValue - cDataValue.m_cRXPreviousStepData.m_dMeanValue_Array[nTraceIndex]) / cDataValue.m_cRXPreviousStepData.m_dStdevValue_Array[nTraceIndex], 2, MidpointRounding.AwayFromZero);

                                if (dMagnificationValue > dPTHFRXStdevMagnification)
                                {
                                    dPTHFRXStdevMagnification = dMagnificationValue;
                                    dPTHFRXStdev = cDataValue.m_cRXPreviousStepData.m_dStdevValue_Array[nTraceIndex];
                                }
                            }
                        }

                        double dPTHFTXStdevMagnification = 0.0;
                        double dPTHFTXStdev = 0.0;

                        for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                        {
                            if (cDataValue.m_cTXPreviousStepData.m_nMaxValue_Array[nTraceIndex] == cDataValue.m_cTXPreviousStepData.m_nTotalMaxValue)
                            {
                                double dMagnificationValue = Math.Round((cDataValue.m_cTXPreviousStepData.m_nTotalMaxValue - cDataValue.m_cTXPreviousStepData.m_dMeanValue_Array[nTraceIndex]) / cDataValue.m_cTXPreviousStepData.m_dStdevValue_Array[nTraceIndex], 2, MidpointRounding.AwayFromZero);

                                if (dMagnificationValue > dPTHFTXStdevMagnification)
                                {
                                    dPTHFTXStdevMagnification = dMagnificationValue;
                                    dPTHFTXStdev = cDataValue.m_cTXPreviousStepData.m_dStdevValue_Array[nTraceIndex];
                                }
                            }
                        }

                        double dBHFRXStdevMagnification = 0.0;
                        double dBHFRXStdev = 0.0;

                        for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                        {
                            if (cDataValue.m_cRXReferenceData.m_nMax_Array[nTraceIndex] == cDataValue.m_cRXReferenceValue.m_nTotalMax)
                            {
                                double dMagnificationValue = Math.Round((cDataValue.m_cRXReferenceValue.m_nTotalMax - cDataValue.m_cRXReferenceData.m_dMean_Array[nTraceIndex]) / cDataValue.m_cRXReferenceData.m_dStd_Array[nTraceIndex], 2, MidpointRounding.AwayFromZero);

                                if (dMagnificationValue > dPTHFRXStdevMagnification)
                                {
                                    dBHFRXStdevMagnification = dMagnificationValue;
                                    dBHFRXStdev = cDataValue.m_cRXReferenceData.m_dStd_Array[nTraceIndex];
                                }
                            }
                        }

                        double dBHFTXStdevMagnification = 0.0;
                        double dBHFTXStdev = 0.0;

                        for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                        {
                            if (cDataValue.m_cTXReferenceData.m_nMax_Array[nTraceIndex] == cDataValue.m_cTXReferenceValue.m_nTotalMax)
                            {
                                double dMagnificationValue = Math.Round((cDataValue.m_cTXReferenceValue.m_nTotalMax - cDataValue.m_cTXReferenceData.m_dMean_Array[nTraceIndex]) / cDataValue.m_cTXReferenceData.m_dStd_Array[nTraceIndex], 2, MidpointRounding.AwayFromZero);

                                if (dMagnificationValue > dBHFTXStdevMagnification)
                                {
                                    dBHFTXStdevMagnification = dMagnificationValue;
                                    dBHFTXStdev = cDataValue.m_cTXReferenceData.m_dStd_Array[nTraceIndex];
                                }
                            }
                        }

                        string sWarningMessage = "";

                        string sPTHFRXTXMaxValue = "";

                        if (dPTHFRXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB && dPTHFTXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB)
                            sPTHFRXTXMaxValue = string.Format("PTHF RX:{0}({1}*Stdev+Mean) / TX:{2}({3}*Stdev+Mean)", cDataValue.m_cRXPreviousStepData.m_nTotalMaxValue, dPTHFRXStdevMagnification, cDataValue.m_cTXPreviousStepData.m_nTotalMaxValue, dPTHFTXStdevMagnification);
                        else if (dPTHFRXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB)
                            sPTHFRXTXMaxValue = string.Format("PTHF RX:{0}({1}*Stdev+Mean)", cDataValue.m_cRXPreviousStepData.m_nTotalMaxValue, dPTHFRXStdevMagnification);
                        else if (dPTHFTXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB)
                            sPTHFRXTXMaxValue = string.Format("PTHF TX:{0}({1}*Stdev+Mean)", cDataValue.m_cTXPreviousStepData.m_nTotalMaxValue, dPTHFTXStdevMagnification);

                        string sBHFRXTXMaxValue = "";

                        if (dBHFRXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB && dBHFTXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB)
                            sBHFRXTXMaxValue = string.Format("BHF RX:{0}({1}*Stdev+Mean) / TX:{2}({3}*Stdev-Mean", cDataValue.m_cRXReferenceValue.m_nTotalMax, dBHFRXStdevMagnification, cDataValue.m_cTXReferenceValue.m_nTotalMax, dBHFTXStdevMagnification);
                        else if (dBHFRXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB)
                            sBHFRXTXMaxValue = string.Format("BHF RX:{0}({1}*Stdev+Mean)", cDataValue.m_cRXReferenceValue.m_nTotalMax, dBHFRXStdevMagnification);
                        else if (dBHFTXStdevMagnification >= m_dMaxMinusMeanValueOverWarningStdevMagHB)
                            sBHFRXTXMaxValue = string.Format("BHF TX:{0}({1}*Stdev+Mean)", cDataValue.m_cTXReferenceValue.m_nTotalMax, dBHFTXStdevMagnification);

                        if (sPTHFRXTXMaxValue != "" && sBHFRXTXMaxValue != "")
                            sWarningMessage = string.Format("Max Value({0} & {1}) Over Warning Stdev Mag. HB({2}*Stdev)", sPTHFRXTXMaxValue, sBHFRXTXMaxValue, Convert.ToString(m_dMaxMinusMeanValueOverWarningStdevMagHB));
                        else if (sPTHFRXTXMaxValue != "")
                            sWarningMessage = string.Format("Max Value({0}) Over Warning Stdev Mag. HB({1}*Stdev)", sPTHFRXTXMaxValue, Convert.ToString(m_dMaxMinusMeanValueOverWarningStdevMagHB));
                        else if (sBHFRXTXMaxValue != "")
                            sWarningMessage = string.Format("Max Value({0}) Over Warning Stdev Mag. HB({1}*Stdev)", sBHFRXTXMaxValue, Convert.ToString(m_dMaxMinusMeanValueOverWarningStdevMagHB));

                        cDataInfo.m_sWarningMessage = sWarningMessage;
                    }
                    else if (m_dMaxValueOverWarningAbsValueHB >= 0.0)
                    {
                        string sWarningMessage = "";

                        string sPTHFRXTXMaxValue = "";

                        if (cDataValue.m_cRXPreviousStepData.m_nTotalMaxValue >= m_dMaxValueOverWarningAbsValueHB && cDataValue.m_cTXPreviousStepData.m_nTotalMaxValue >= m_dMaxValueOverWarningAbsValueHB)
                            sPTHFRXTXMaxValue = string.Format("PTHF RX:{0} / TX:{1}", cDataValue.m_cRXPreviousStepData.m_nTotalMaxValue, cDataValue.m_cTXPreviousStepData.m_nTotalMaxValue);
                        else if (cDataValue.m_cRXPreviousStepData.m_nTotalMaxValue >= m_dMaxValueOverWarningAbsValueHB)
                            sPTHFRXTXMaxValue = string.Format("PTHF RX:{0}", cDataValue.m_cRXPreviousStepData.m_nTotalMaxValue);
                        else if (cDataValue.m_cTXPreviousStepData.m_nTotalMaxValue >= m_dMaxValueOverWarningAbsValueHB)
                            sPTHFRXTXMaxValue = string.Format("PTHF TX:{0}", cDataValue.m_cTXPreviousStepData.m_nTotalMaxValue);

                        string sBHFRXTXMaxValue = "";

                        if (cDataValue.m_cRXReferenceValue.m_nTotalMax >= m_dMaxValueOverWarningAbsValueHB && cDataValue.m_cTXReferenceValue.m_nTotalMax >= m_dMaxValueOverWarningAbsValueHB)
                            sBHFRXTXMaxValue = string.Format("BHF RX:{0} / TX:{1}", cDataValue.m_cRXReferenceValue.m_nTotalMax, cDataValue.m_cTXReferenceValue.m_nTotalMax);
                        else if (cDataValue.m_cRXReferenceValue.m_nTotalMax >= m_dMaxValueOverWarningAbsValueHB)
                            sBHFRXTXMaxValue = string.Format("BHF RX:{0}", cDataValue.m_cRXReferenceValue.m_nTotalMax);
                        else if (cDataValue.m_cTXReferenceValue.m_nTotalMax >= m_dMaxValueOverWarningAbsValueHB)
                            sBHFRXTXMaxValue = string.Format("BHF TX:{0}", cDataValue.m_cTXReferenceValue.m_nTotalMax);

                        if (sPTHFRXTXMaxValue != "" && sBHFRXTXMaxValue != "")
                            sWarningMessage = string.Format("Max Value({0} & {1}) Over Warning Abs. Value HB({2})", sPTHFRXTXMaxValue, sBHFRXTXMaxValue, Convert.ToString(m_dMaxValueOverWarningAbsValueHB));
                        else if (sPTHFRXTXMaxValue != "")
                            sWarningMessage = string.Format("Max Value({0}) Over Warning Abs. Value HB({1})", sPTHFRXTXMaxValue, Convert.ToString(m_dMaxValueOverWarningAbsValueHB));
                        else if (sBHFRXTXMaxValue != "")
                            sWarningMessage = string.Format("Max Value({0}) Over Warning Abs. Value HB({1})", sBHFRXTXMaxValue, Convert.ToString(m_dMaxValueOverWarningAbsValueHB));

                        cDataInfo.m_sWarningMessage = sWarningMessage;
                    }
                }
            }

            /*
            int nDataCount = cDataInfo_List.Count;

            if (nOverInnerReferenceValueHBCount == nDataCount)
            {
                m_sErrorMessage = string.Format("Inner Reference Value Over High Boundary({0})", Convert.ToString(m_dInnerReferenceValueHB));
                m_bAllOverInnerReferenceValueHBFlag = true;
                m_bErrorFlag = true;
            }
            */
        }

        protected void WriteResultDataFile(bool bTotalDataFlag = false)
        {
            string sFilePath;
            List<DataInfo> cDataInfo_List;
            List<DataValue> cDataValue_List;

            if (bTotalDataFlag == false)
            {
                sFilePath = m_sResultFilePath;
                cDataInfo_List = m_cDataInfo_List;
                cDataValue_List = m_cDataValue_List;
            }
            else
            {
                sFilePath = m_sTNResultFilePath;
                cDataInfo_List = m_cTotalDataInfo_List;
                cDataValue_List = m_cTotalDataValue_List;
            }

            string[] sTitleName_Array = new string[17] 
            { 
                SpecificText.m_sRanking,
                SpecificText.m_sFileName,
                SpecificText.m_sFrequency, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2,
                SpecificText.m_sTotalMax,
                SpecificText.m_sInnerTX, 
                SpecificText.m_sInnerRX, 
                SpecificText.m_sEdgeTX, 
                SpecificText.m_sEdgeRX, 
                "",
                SpecificText.m_sInnerTXMultipleWeighting, 
                SpecificText.m_sInnerRXMultipleWeighting, 
                SpecificText.m_sEdgeTXMultipleWeighting, 
                SpecificText.m_sEdgeRXMultipleWeighting,
                SpecificText.m_sTotal_Score, 
                SpecificText.m_sException_Message 
            };

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_REPORT);

                sw.WriteLine();

                sw.WriteLine("Project Information");

                sw.WriteLine();

                sw.WriteLine("Ranking Data(Weighting Ranking)");
                WriteWeightingRankDataToCSVFile(cDataInfo_List, cDataValue_List, sw, sTitleName_Array);
                sw.WriteLine();

                sw.WriteLine("Information,");
                sw.WriteLine("WeightingRankingType,1");
                sw.WriteLine(string.Format("ErrorMessage,{0}", m_sErrorMessage));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WriteWeightingRankDataToCSVFile(List<DataInfo> cDataInfo_List, List<DataValue> cDataValue_List, StreamWriter sw, string[] sTitleName_Array)
        {
            for (int nTitleIndex = 0; nTitleIndex < sTitleName_Array.Length; nTitleIndex++)
            {
                sw.Write(string.Format("{0}", sTitleName_Array[nTitleIndex]));

                if (nTitleIndex < sTitleName_Array.Length - 1)
                    sw.Write(",");
                else
                    sw.WriteLine();
            }

            foreach (DataInfo cDataInfo in cDataInfo_List)
            {
                DataValue cDataValue = cDataValue_List.Find(x => (x.m_nPH1 == cDataInfo.m_nSettingPH1 && x.m_nPH2 == cDataInfo.m_nSettingPH2));

                sw.Write(string.Format("{0},", cDataInfo.m_nRankIndex));
                sw.Write(string.Format("{0},", cDataInfo.m_sFileName));
                sw.Write(string.Format("{0:0.000},", cDataInfo.m_dFrequency.ToString("0.000")));
                sw.Write(string.Format("{0},", cDataInfo.m_nReadPH1));
                sw.Write(string.Format("{0},", cDataInfo.m_nReadPH2));
                sw.Write(string.Format("{0},", Math.Max(cDataValue.m_cRXReferenceValue.m_nTotalMax, cDataValue.m_cTXReferenceValue.m_nTotalMax)));
                sw.Write(string.Format("{0},", cDataValue.m_cTXResultData.m_dInnerMaxReferenceData));
                sw.Write(string.Format("{0},", cDataValue.m_cRXResultData.m_dInnerMaxReferenceData));
                sw.Write(string.Format("{0},", cDataValue.m_cTXResultData.m_dEdgeMaxReferenceData));
                sw.Write(string.Format("{0},", cDataValue.m_cRXResultData.m_dEdgeMaxReferenceData));
                sw.Write(",");
                sw.Write(string.Format("{0},", cDataValue.m_dWeightingInnerTXValue));
                sw.Write(string.Format("{0},", cDataValue.m_dWeightingInnerRXValue));
                sw.Write(string.Format("{0},", cDataValue.m_dWeightingEdgeTXValue));
                sw.Write(string.Format("{0},", cDataValue.m_dWeightingEdgeRXValue));
                sw.Write(string.Format("{0},", cDataValue.m_dWeightingScore));

                if (cDataInfo.m_sWarningMessage != "")
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_sWarningMessage));
                else
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_sErrorMessage));
            }
        }

        protected bool SaveTraceLineChartPictureFile(bool bTotalDataFlag = false)
        {
            string sResultFilePath = m_sResultFilePath;

            if (bTotalDataFlag == false)
                sResultFilePath = m_sResultFilePath;
            else
                sResultFilePath = m_sTNResultFilePath;

            if (File.Exists(sResultFilePath) == false)
                return false;

            if (File.Exists(m_sPictureFolderPath) == false)
                Directory.CreateDirectory(m_sPictureFolderPath);

            int nDataCount = GetDataCount(bTotalDataFlag);

            int nTopDataNumber = (nDataCount <= 5) ? nDataCount : 5;

            List<int[]> nTopColor_List = new List<int[]>();
            List<int[]> nAllColor_List = new List<int[]>();

            SetColorList(ref nTopColor_List, nTopDataNumber);
            SetColorList(ref nAllColor_List, nDataCount);

            //Show RX Picture
            SaveLineChart(nTopDataNumber, nTopColor_List, nAllColor_List, bTotalDataFlag, true);

            //Show TX Picture
            SaveLineChart(nTopDataNumber, nTopColor_List, nAllColor_List, bTotalDataFlag, false);

            return true;
        }

        private int GetDataCount(bool bTotalDataFlag)
        {
            List<DataInfo> cDataInfo_List;
            List<DataValue> cDataValue_List;

            if (bTotalDataFlag == false)
            {
                cDataInfo_List = m_cDataInfo_List;
                cDataValue_List = m_cDataValue_List;
            }
            else
            {
                cDataInfo_List = m_cTotalDataInfo_List;
                cDataValue_List = m_cTotalDataValue_List;
            }

            int nDataCount = 0;

            for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
            {
                bool bErrorFlag = true;

                for (int nCompareIndex = 0; nCompareIndex < cDataInfo_List.Count; nCompareIndex++)
                {
                    if (cDataValue_List[nCompareIndex].m_nRankIndex == nSetIndex + 1 && (cDataInfo_List[nCompareIndex].m_sErrorMessage == "" || cDataValue_List[nCompareIndex].m_bInnerReferenceValueOverHBFlag == true))
                    {
                        bErrorFlag = false;
                        break;
                    }
                }

                if (bErrorFlag == true)
                    continue;

                nDataCount++;
            }

            return nDataCount;
        }

        private void SaveLineChart(int nTopDataNumber, List<int[]> nTopColor_List, List<int[]> nAllColor_List, bool bTotalDataFlag, bool bRXTraceTypeFlag)
        {
            string sTitleName = "";
            string sTop5FilePath = "";
            string sFilePath = "";
            string sTop5FileName = "";
            string sFileName = "";
            string sTraceType = (bRXTraceTypeFlag == true) ? StringConvert.m_sTRACETYPE_RX : StringConvert.m_sTRACETYPE_TX;
            int nTraceNumber = (bRXTraceTypeFlag == true) ? m_nRXTraceNumber : m_nTXTraceNumber;

            sTitleName = sTraceType;
            sTop5FileName = string.Format("{0}_{1}", sTraceType, SpecificText.m_sChart_TopDataFileName);
            sFileName = string.Format("{0}_{1}", sTraceType, SpecificText.m_sChartFileName);

            if (bTotalDataFlag == true)
            {
                sTop5FileName = string.Format("TN_{0}", sTop5FileName);
                sFileName = string.Format("TN_{0}", sFileName);
            }

            sTop5FilePath = string.Format(@"{0}\{1}", m_sPictureFolderPath, sTop5FileName);
            sFilePath = string.Format(@"{0}\{1}", m_sPictureFolderPath, sFileName);

            List<Series> serTop5Series_List = new List<Series>();
            List<Series> serAllSeries_List = new List<Series>();

            List<DataInfo> cDataInfo_List;
            List<DataValue> cDataValue_List;

            if (bTotalDataFlag == false)
            {
                cDataInfo_List = m_cDataInfo_List;
                cDataValue_List = m_cDataValue_List;
            }
            else
            {
                cDataInfo_List = m_cTotalDataInfo_List;
                cDataValue_List = m_cTotalDataValue_List;
            }

            for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
            {
                bool bErrorFlag = true;
                DataInfo cDataInfo = new DataInfo();
                DataValue cDataValue = new DataValue();

                for (int nCompareIndex = 0; nCompareIndex < cDataInfo_List.Count; nCompareIndex++)
                {
                    if (cDataValue_List[nCompareIndex].m_nRankIndex == nSetIndex + 1)
                    {
                        if (cDataInfo_List[nCompareIndex].m_sErrorMessage == "" || (cDataInfo_List[nCompareIndex].m_sErrorMessage != "" && cDataValue_List[nCompareIndex].m_bInnerReferenceValueOverHBFlag == true))
                        {
                            cDataInfo = cDataInfo_List[nCompareIndex];
                            cDataValue = cDataValue_List[nCompareIndex];
                            bErrorFlag = false;
                        }
                        break;
                    }
                }

                if (bErrorFlag == true)
                    continue;

                //string sFileNameWithoutExtension = cDataInfo.m_sFileName.Replace(".txt", "");
                string sDataName = string.Format("{0:0.000}KHz_{1:00}_{2:00}", cDataInfo.m_dFrequency, cDataInfo.m_nSettingPH1, cDataInfo.m_nSettingPH2);
                Series serSeries_Top = new Series(sDataName);
                Series serSeries_All = new Series(sDataName);
                serSeries_Top.ChartType = SeriesChartType.Line;
                serSeries_Top.IsValueShownAsLabel = false;
                serSeries_All.ChartType = SeriesChartType.Line;
                serSeries_All.IsValueShownAsLabel = false;

                for (int nTraceIndex = 1; nTraceIndex <= nTraceNumber; nTraceIndex++)
                {
                    if (bRXTraceTypeFlag == true)
                    {
                        serSeries_Top.Points.AddXY(nTraceIndex, cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex - 1]);
                        serSeries_All.Points.AddXY(nTraceIndex, cDataValue.m_cRXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex - 1]);
                    }
                    else
                    {
                        serSeries_Top.Points.AddXY(nTraceIndex, cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex - 1]);
                        serSeries_All.Points.AddXY(nTraceIndex, cDataValue.m_cTXReferenceData.m_dMeanPlus3Std_Array[nTraceIndex - 1]);
                    }
                }

                if (nSetIndex <= nTopDataNumber - 1)
                {
                    serSeries_Top.Color = Color.FromArgb(nTopColor_List[nSetIndex][0], nTopColor_List[nSetIndex][1], nTopColor_List[nSetIndex][2]);
                    serTop5Series_List.Add(serSeries_Top);
                }

                serSeries_All.Color = Color.FromArgb(nAllColor_List[nSetIndex][0], nAllColor_List[nSetIndex][1], nAllColor_List[nSetIndex][2]);
                serAllSeries_List.Add(serSeries_All);
            }

            SaveChart(sTitleName, serTop5Series_List, serAllSeries_List, nTopDataNumber, sTop5FilePath, sFilePath, bRXTraceTypeFlag);
        }

        private void SaveChart(string sTitleName, List<Series> serTop5Series_List, List<Series> serAllSeries_List, int nTopDataNumber, string sTop5FilePath, string sFilePath, bool bRXTraceType)
        {
            //Show Line Chart
            Chart cChart = new Chart();
            var vChartArea = new ChartArea();
            cChart.ChartAreas.Add(vChartArea);
            cChart.Width = 1500;
            cChart.Height = 321;
            cChart.Legends.Add("Legend");
            cChart.Legends["Legend"].Font = new Font("Times New Roman", 9);
            cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
            cChart.Titles.Add(sTitleName);
            cChart.Titles[0].Font = new Font("Times New Roman", 18);
            cChart.ChartAreas[0].AxisY.Title = "Reference Value";
            cChart.ChartAreas[0].AxisY.TitleFont = new Font("Times New Roman", 10);
            cChart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisX.Title = "Trace Number";
            cChart.ChartAreas[0].AxisX.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Times New Roman", 11);

            if (bRXTraceType == true)
            {
                if (m_nRXTraceNumber > 40)
                    cChart.ChartAreas[0].AxisX.Interval = 2;
                else
                    cChart.ChartAreas[0].AxisX.Interval = 1;
            }
            else
            {
                if (m_nTXTraceNumber > 40)
                    cChart.ChartAreas[0].AxisX.Interval = 2;
                else
                    cChart.ChartAreas[0].AxisX.Interval = 1;
            }

            cChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.Titles[0].Position.X = 50;
            cChart.Titles[0].Position.Height = 10;
            cChart.ChartAreas[0].Position.X = 0;
            cChart.ChartAreas[0].Position.Y = 50;
            cChart.ChartAreas[0].Position.Width = 90;
            cChart.ChartAreas[0].Position.Height = 90;
            cChart.Legends["Legend"].Position.X = 91;
            cChart.Legends["Legend"].Position.Y = 0;
            cChart.Legends["Legend"].Position.Width = 10;

            if (bRXTraceType == true)
            {
                if (m_nRXTraceNumber > 40)
                {
                    if (m_nRXTraceNumber % 2 == 1)
                        cChart.ChartAreas[0].AxisX.Maximum = m_nRXTraceNumber + 1;
                    else
                        cChart.ChartAreas[0].AxisX.Maximum = m_nRXTraceNumber;
                }
                else
                    cChart.ChartAreas[0].AxisX.Maximum = m_nRXTraceNumber;
            }
            else
            {
                if (m_nTXTraceNumber > 40)
                {
                    if (m_nTXTraceNumber % 2 == 1)
                        cChart.ChartAreas[0].AxisX.Maximum = m_nTXTraceNumber + 1;
                    else
                        cChart.ChartAreas[0].AxisX.Maximum = m_nTXTraceNumber;
                }
                else
                    cChart.ChartAreas[0].AxisX.Maximum = m_nTXTraceNumber;
            }

            foreach (Series serSeries in serTop5Series_List)
                cChart.Series.Add(serSeries);

            int nHeight = nTopDataNumber * 8;
            cChart.Legends["Legend"].Position.Height = nHeight;
            cChart.SaveImage(sTop5FilePath, ChartImageFormat.Jpeg);

            cChart.Series.Clear();

            //cChart.ChartAreas[0].Position.Width = 90;

            foreach (Series serSeries in serAllSeries_List)
                cChart.Series.Add(serSeries);

            //cChart.Legends["Legend"].Position.Width = 22;

            if (m_cDataInfo_List.Count / 22 >= 2 && m_cDataInfo_List.Count % 22 > 0)
            {
                cChart.ChartAreas[0].Position.Width = 70;
                cChart.Legends["Legend"].Position.X = 71;
                cChart.Legends["Legend"].Position.Width = 29;
                cChart.Legends["Legend"].Position.Height = 100;
                cChart.Legends["Legend"].Font = new Font("Times New Roman", 7);
            }
            else if (m_cDataInfo_List.Count / 14 >= 1 || (m_cDataInfo_List.Count / 14 == 1 && m_cDataInfo_List.Count % 14 > 0))
            {
                cChart.ChartAreas[0].Position.Width = 77;
                cChart.Legends["Legend"].Position.X = 78;
                cChart.Legends["Legend"].Position.Width = 22;
                cChart.Legends["Legend"].Position.Height = 100;
                cChart.Legends["Legend"].Font = new Font("Times New Roman", 8);
            }
            else
            {
                nHeight = m_cDataInfo_List.Count * 8;

                if (m_cDataInfo_List.Count >= 13)
                    nHeight = m_cDataInfo_List.Count * 7;

                if (nHeight > 100)
                    nHeight = 100;

                cChart.Legends["Legend"].Position.Height = nHeight;
                cChart.Legends["Legend"].Font = new Font("Times New Roman", 9);
            }

            cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
        }

        protected bool SaveFrequencyLineChartPictureFile(bool bTotalDataFlag = false)
        {
            List<DataInfo> cDataInfo_List;
            List<DataValue> cDataValue_List;

            if (bTotalDataFlag == false)
            {
                cDataInfo_List = m_cDataInfo_List;
                cDataValue_List = m_cDataValue_List;
            }
            else
            {
                cDataInfo_List = m_cTotalDataInfo_List;
                cDataValue_List = m_cTotalDataValue_List;
            }

            List<double> dAllFrequency_List = new List<double>();

            double dFrequencyHB = ParamAutoTuning.m_nFrequencyHB;
            double dFrequencyLB = ParamAutoTuning.m_nFrequencyLB_MPP180;
            int nAnalogValueHB = (int)(MainConstantParameter.m_nICCLOCKFREQUENCY / (ParamAutoTuning.m_nFrequencyLB_MPP180 * 1000));
            int nAnalogValueLB = (int)(MainConstantParameter.m_nICCLOCKFREQUENCY / (ParamAutoTuning.m_nFrequencyHB * 1000));

            for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
            {
                if (nSetIndex == 0)
                {
                    dFrequencyLB = cDataInfo_List[nSetIndex].m_dFrequency;
                    dFrequencyHB = cDataInfo_List[nSetIndex].m_dFrequency;
                }
                else
                {
                    if (cDataInfo_List[nSetIndex].m_dFrequency < dFrequencyLB)
                        dFrequencyLB = cDataInfo_List[nSetIndex].m_dFrequency;

                    if (m_cDataInfo_List[nSetIndex].m_dFrequency > dFrequencyHB)
                        dFrequencyHB = cDataInfo_List[nSetIndex].m_dFrequency;
                }
            }

            nAnalogValueHB = (int)Math.Round(MainConstantParameter.m_nICCLOCKFREQUENCY / (dFrequencyLB * 1000), 0, MidpointRounding.AwayFromZero);
            nAnalogValueLB = (int)Math.Round(MainConstantParameter.m_nICCLOCKFREQUENCY / (dFrequencyHB * 1000), 0, MidpointRounding.AwayFromZero);

            for (int nAnalogValue = nAnalogValueHB; nAnalogValue >= nAnalogValueLB; nAnalogValue--)
            {
                double dFrequency = Math.Round((double)MainConstantParameter.m_nICCLOCKFREQUENCY / (nAnalogValue * 1000), 3, MidpointRounding.AwayFromZero);
                dAllFrequency_List.Add(dFrequency);
            }

            List<int> nAllFrequencyAmount_List = new List<int>();

            for (int nFrequencyIndex = 0; nFrequencyIndex < dAllFrequency_List.Count; nFrequencyIndex++)
            {
                nAllFrequencyAmount_List.Add(0);

                for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
                {
                    if (cDataInfo_List[nSetIndex].m_dFrequency == dAllFrequency_List[nFrequencyIndex] && (cDataInfo_List[nSetIndex].m_sErrorMessage == "" || cDataValue_List[nSetIndex].m_bInnerReferenceValueOverHBFlag == true))
                        nAllFrequencyAmount_List[nFrequencyIndex]++;
                }
            }

            for (int nChartIndex = 0; nChartIndex < m_nChartType_Array.Length; nChartIndex++)
                SaveFrequencyLineChart(dAllFrequency_List, nAllFrequencyAmount_List, m_nChartType_Array[nChartIndex], bTotalDataFlag);

            return true;
        }

        private void SaveFrequencyLineChart(List<double> dAllFrequency_List, List<int> nAllFrequencyAmount_List, int nDataType, bool bTotalDataFlag)
        {
            List<DataInfo> cDataInfo_List;
            List<DataValue> cDataValue_List;

            if (bTotalDataFlag == false)
            {
                cDataInfo_List = m_cDataInfo_List;
                cDataValue_List = m_cDataValue_List;
            }
            else
            {
                cDataInfo_List = m_cTotalDataInfo_List;
                cDataValue_List = m_cTotalDataValue_List;
            }

            string sTitleName = SetTitleName(nDataType);
            string sTotalDataText = (bTotalDataFlag == true) ? "TN_" : "";
            string sFilePath = string.Format(@"{0}\\{1}{2}_{3}", m_sPictureFolderPath, sTotalDataText, sTitleName.Replace(" ", ""), SpecificText.m_sFrqChartFileName);
            string sIncludeMaxFilePath = string.Format(@"{0}\\{1}{2}_{3}", m_sPictureFolderPath, sTotalDataText, sTitleName.Replace(" ", ""), SpecificText.m_sFrqChartIncludeMaxFileName);

            List<Series> serSeries_List = new List<Series>();
            List<Series> serSeries_IncludeMax_List = new List<Series>();

            if (nDataType != m_nTRACEAREATYPE_TOTAL)
            {
                int nMaxValueDataType = nDataType;

                if (nDataType == m_nTRACEAREATYPE_RXINNER)
                    nMaxValueDataType = m_nTRACEAREATYPE_RXINNER_MAX;
                else if (nDataType == m_nTRACEAREATYPE_RXEDGE)
                    nMaxValueDataType = m_nTRACEAREATYPE_RXEDGE_MAX;
                else if (nDataType == m_nTRACEAREATYPE_TXINNER)
                    nMaxValueDataType = m_nTRACEAREATYPE_TXINNER_MAX;
                else if (nDataType == m_nTRACEAREATYPE_TXEDGE)
                    nMaxValueDataType = m_nTRACEAREATYPE_TXEDGE_MAX;

                Series serSeries = new Series("Reference Value in Frequency");
                Series serSeries_MaxValue = new Series("Max Value in Frequency");
                serSeries.ChartType = SeriesChartType.Line;
                serSeries.BorderWidth = 2;
                serSeries_MaxValue.ChartType = SeriesChartType.Line;
                serSeries_MaxValue.BorderDashStyle = ChartDashStyle.DashDot;
                serSeries_MaxValue.BorderWidth = 1;
                serSeries.MarkerStyle = MarkerStyle.Circle;
                serSeries_MaxValue.MarkerStyle = MarkerStyle.Circle;
                serSeries.Color = Color.Blue;
                serSeries_MaxValue.Color = Color.Red;

                int nCount = 0;
                bool bHasDataFlag = false;
                double dLastValue = -1.0;
                double dLastMaxValue = -1.0;

                for (int nFrequencyIndex = 0; nFrequencyIndex < dAllFrequency_List.Count; nFrequencyIndex++)
                {
                    if (nAllFrequencyAmount_List[nFrequencyIndex] > 0)
                    {
                        for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
                        {
                            if (cDataInfo_List[nSetIndex].m_dFrequency == dAllFrequency_List[nFrequencyIndex] && (cDataInfo_List[nSetIndex].m_sErrorMessage == "" || cDataValue_List[nSetIndex].m_bInnerReferenceValueOverHBFlag == true))
                            {
                                dLastValue = GetFrequencyReferenceValue(cDataValue_List[nSetIndex], nDataType);
                                serSeries.Points.AddXY(nFrequencyIndex + 1, dLastValue);
                                dLastMaxValue = GetFrequencyReferenceValue(cDataValue_List[nSetIndex], nMaxValueDataType);
                                serSeries_MaxValue.Points.AddXY(nFrequencyIndex + 1, dLastMaxValue);
                                serSeries.Points[nCount].AxisLabel = dAllFrequency_List[nFrequencyIndex].ToString("F3");
                                serSeries_MaxValue.Points[nCount].AxisLabel = dAllFrequency_List[nFrequencyIndex].ToString("F3");
                                bHasDataFlag = true;
                                nCount++;
                            }
                        }
                    }
                    else
                    {
                        if (bHasDataFlag == true)
                        {
                            serSeries.Points.AddXY(nFrequencyIndex + 1, dLastValue * -1);
                            serSeries_MaxValue.Points.AddXY(nFrequencyIndex + 1, dLastMaxValue * -1);
                            bHasDataFlag = false;
                        }
                        else
                        {
                            if (nFrequencyIndex < dAllFrequency_List.Count - 1 && nAllFrequencyAmount_List[nFrequencyIndex + 1] > 0)
                            {
                                for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
                                {
                                    if (cDataInfo_List[nSetIndex].m_dFrequency == dAllFrequency_List[nFrequencyIndex + 1] && (cDataInfo_List[nSetIndex].m_sErrorMessage == "" || cDataValue_List[nSetIndex].m_bInnerReferenceValueOverHBFlag == true))
                                    {
                                        double dPreviousValue = GetFrequencyReferenceValue(cDataValue_List[nSetIndex], nDataType);
                                        serSeries.Points.AddXY(nFrequencyIndex + 1, dPreviousValue * -1);
                                        double dPreviousMaxValue = GetFrequencyReferenceValue(cDataValue_List[nSetIndex], nMaxValueDataType);
                                        serSeries_MaxValue.Points.AddXY(nFrequencyIndex + 1, dPreviousValue * -1);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                serSeries.Points.AddXY(nFrequencyIndex + 1, -1);
                                serSeries_MaxValue.Points.AddXY(nFrequencyIndex + 1, -1);
                            }
                        }

                        serSeries.Points[nCount].AxisLabel = dAllFrequency_List[nFrequencyIndex].ToString("F3");
                        serSeries_MaxValue.Points[nCount].AxisLabel = dAllFrequency_List[nFrequencyIndex].ToString("F3");
                        nCount++;
                    }
                }

                serSeries_List.Add(serSeries);
                serSeries_IncludeMax_List.Add(serSeries);
                serSeries_IncludeMax_List.Add(serSeries_MaxValue);
            }
            else
            {
                Series serRXInner = new Series("RX Inner");
                Series serRXEdge = new Series("RX Edge");
                Series serTXInner = new Series("TX Inner");
                Series serTXEdge = new Series("TX Edge");

                Series serRXInner_IncludeMax = new Series("RX Inner");
                Series serRXEdge_IncludeMax = new Series("RX Edge");
                Series serTXInner_IncludeMax = new Series("TX Inner");
                Series serTXEdge_IncludeMax = new Series("TX Edge");
                Series serTotalMax_IncludeMax = new Series("Total Max");

                Series[] serDataType_Array = new Series[4] 
                { 
                    serRXInner, 
                    serRXEdge, 
                    serTXInner, 
                    serTXEdge 
                };

                Color[] colorBackColor_Array = new Color[4] 
                { 
                    Color.Blue, 
                    Color.Orange, 
                    Color.Green, 
                    Color.Purple 
                };

                Series[] serDataType_IncludeMax_Array = new Series[5] 
                { 
                    serRXInner_IncludeMax, 
                    serRXEdge_IncludeMax, 
                    serTXInner_IncludeMax, 
                    serTXEdge_IncludeMax,
                    serTotalMax_IncludeMax
                };

                Color[] colorBackColor_IncludeMax_Array = new Color[5] 
                { 
                    Color.Blue, 
                    Color.Orange, 
                    Color.Green, 
                    Color.Purple,
                    Color.Red
                };

                for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                {
                    serDataType_Array[nTypeIndex].ChartType = SeriesChartType.Line;
                    serDataType_Array[nTypeIndex].MarkerStyle = MarkerStyle.Circle;
                    serDataType_Array[nTypeIndex].Color = colorBackColor_Array[nTypeIndex];
                }

                for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                {
                    if (nTypeIndex < serDataType_IncludeMax_Array.Length - 1)
                    {
                        serDataType_IncludeMax_Array[nTypeIndex].ChartType = SeriesChartType.Line;
                        serDataType_IncludeMax_Array[nTypeIndex].BorderWidth = 2;
                        serDataType_IncludeMax_Array[nTypeIndex].MarkerStyle = MarkerStyle.Circle;
                    }
                    else
                    {
                        serDataType_IncludeMax_Array[nTypeIndex].ChartType = SeriesChartType.Line;
                        serDataType_IncludeMax_Array[nTypeIndex].BorderWidth = 1;
                        serDataType_IncludeMax_Array[nTypeIndex].BorderDashStyle = ChartDashStyle.DashDot;
                        serDataType_IncludeMax_Array[nTypeIndex].MarkerStyle = MarkerStyle.Circle;
                    }

                    serDataType_IncludeMax_Array[nTypeIndex].Color = colorBackColor_IncludeMax_Array[nTypeIndex];
                }

                int nCount = 0;
                bool bHasDataFlag = false;

                double[] dLastValue_Array = new double[4] 
                { 
                    -1.0, 
                    -1.0, 
                    -1.0, 
                    -1.0 
                };

                double[] dLastValue_IncludeMax_Array = new double[5] 
                { 
                    -1.0, 
                    -1.0, 
                    -1.0, 
                    -1.0,
                    -1.0
                };

                for (int nFrequencyIndex = 0; nFrequencyIndex < dAllFrequency_List.Count; nFrequencyIndex++)
                {
                    if (nAllFrequencyAmount_List[nFrequencyIndex] > 0)
                    {
                        for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
                        {
                            if (cDataInfo_List[nSetIndex].m_dFrequency == dAllFrequency_List[nFrequencyIndex] && (cDataInfo_List[nSetIndex].m_sErrorMessage == "" || cDataValue_List[nSetIndex].m_bInnerReferenceValueOverHBFlag == true))
                            {
                                dLastValue_Array = GetFrequencyReferenceValue(cDataValue_List[nSetIndex]);

                                for (int sIndex = 0; sIndex < dLastValue_Array.Length; sIndex++)
                                {
                                    serDataType_Array[sIndex].Points.AddXY(nFrequencyIndex + 1, dLastValue_Array[sIndex]);

                                    if (sIndex == 0)
                                        serDataType_Array[sIndex].Points[nCount].AxisLabel = dAllFrequency_List[nFrequencyIndex].ToString("F3");
                                }

                                dLastValue_IncludeMax_Array = GetFrequencyReferenceValueAndMaxValue(cDataValue_List[nSetIndex]);

                                for (int sIndex = 0; sIndex < dLastValue_IncludeMax_Array.Length; sIndex++)
                                {
                                    serDataType_IncludeMax_Array[sIndex].Points.AddXY(nFrequencyIndex + 1, dLastValue_IncludeMax_Array[sIndex]);

                                    if (sIndex == 0)
                                        serDataType_IncludeMax_Array[sIndex].Points[nCount].AxisLabel = dAllFrequency_List[nFrequencyIndex].ToString("F3");
                                }

                                bHasDataFlag = true;
                                nCount++;
                            }
                        }
                    }
                    else
                    {
                        if (bHasDataFlag == true)
                        {
                            for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                                serDataType_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, dLastValue_Array[nTypeIndex] * -1);

                            for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                                serDataType_IncludeMax_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, dLastValue_IncludeMax_Array[nTypeIndex] * -1);

                            bHasDataFlag = false;
                        }
                        else
                        {
                            if (nFrequencyIndex < dAllFrequency_List.Count - 1 && nAllFrequencyAmount_List[nFrequencyIndex + 1] > 0)
                            {
                                for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
                                {
                                    if (cDataInfo_List[nSetIndex].m_dFrequency == dAllFrequency_List[nFrequencyIndex + 1] && (cDataInfo_List[nSetIndex].m_sErrorMessage == "" || cDataValue_List[nSetIndex].m_bInnerReferenceValueOverHBFlag == true))
                                    {
                                        double[] dPreviousValue_Array = GetFrequencyReferenceValue(cDataValue_List[nSetIndex]);

                                        for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                                            serDataType_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, dPreviousValue_Array[nTypeIndex] * -1);

                                        double[] dPreviousMaxValue_Array = GetFrequencyReferenceValueAndMaxValue(cDataValue_List[nSetIndex]);

                                        for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                                            serDataType_IncludeMax_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, dPreviousMaxValue_Array[nTypeIndex] * -1);

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                                    serDataType_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, -1);

                                for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                                    serDataType_IncludeMax_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, -1);
                            }
                        }

                        serDataType_Array[0].Points[nCount].AxisLabel = dAllFrequency_List[nFrequencyIndex].ToString("F3");
                        serDataType_IncludeMax_Array[0].Points[nCount].AxisLabel = dAllFrequency_List[nFrequencyIndex].ToString("F3");
                        nCount++;
                    }
                }

                for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                    serSeries_List.Add(serDataType_Array[nTypeIndex]);

                for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                    serSeries_IncludeMax_List.Add(serDataType_IncludeMax_Array[nTypeIndex]);
            }

            SaveFrequencyChart(nDataType, sTitleName, serSeries_List, sFilePath);
            SaveFrequencyChart(nDataType, sTitleName, serSeries_IncludeMax_List, sIncludeMaxFilePath);
        }

        private double GetFrequencyReferenceValue(DataValue cDataValue, int nDataType)
        {
            double dValue = -1.0;

            if (nDataType == m_nTRACEAREATYPE_RXINNER)
                dValue = cDataValue.m_cRXResultData.m_dInnerMaxReferenceData;
            else if (nDataType == m_nTRACEAREATYPE_RXEDGE)
                dValue = cDataValue.m_cRXResultData.m_dEdgeMaxReferenceData;
            else if (nDataType == m_nTRACEAREATYPE_TXINNER)
                dValue = cDataValue.m_cTXResultData.m_dInnerMaxReferenceData;
            else if (nDataType == m_nTRACEAREATYPE_TXEDGE)
                dValue = cDataValue.m_cTXResultData.m_dEdgeMaxReferenceData;
            else if (nDataType == m_nTRACEAREATYPE_RXINNER_MAX)
                dValue = cDataValue.m_cRXReferenceValue.m_nInnerMax;
            else if (nDataType == m_nTRACEAREATYPE_RXEDGE_MAX)
                dValue = cDataValue.m_cRXReferenceValue.m_nEdgeMax;
            else if (nDataType == m_nTRACEAREATYPE_TXINNER_MAX)
                dValue = cDataValue.m_cTXReferenceValue.m_nInnerMax;
            else if (nDataType == m_nTRACEAREATYPE_TXEDGE_MAX)
                dValue = cDataValue.m_cTXReferenceValue.m_nEdgeMax;
            else if (nDataType == m_nTRACEAREATYPE_TOTAL_MAX)
                dValue = Math.Max(cDataValue.m_cRXReferenceValue.m_nTotalMax, cDataValue.m_cTXReferenceValue.m_nTotalMax);

            return dValue;
        }

        private double[] GetFrequencyReferenceValue(DataValue cDataValue)
        {
            double[] dValue_Array = new double[4] 
            { 
                -1.0, 
                -1.0, 
                -1.0, 
                -1.0 
            };

            dValue_Array[0] = cDataValue.m_cRXResultData.m_dInnerMaxReferenceData;
            dValue_Array[1] = cDataValue.m_cRXResultData.m_dEdgeMaxReferenceData;
            dValue_Array[2] = cDataValue.m_cTXResultData.m_dInnerMaxReferenceData;
            dValue_Array[3] = cDataValue.m_cTXResultData.m_dEdgeMaxReferenceData;

            return dValue_Array;
        }

        private double[] GetFrequencyReferenceValueAndMaxValue(DataValue cDataValue)
        {
            double[] dValue_Array = new double[5] 
            { 
                -1.0, 
                -1.0, 
                -1.0, 
                -1.0,
                -1.0
            };

            dValue_Array[0] = cDataValue.m_cRXResultData.m_dInnerMaxReferenceData;
            dValue_Array[1] = cDataValue.m_cRXResultData.m_dEdgeMaxReferenceData;
            dValue_Array[2] = cDataValue.m_cTXResultData.m_dInnerMaxReferenceData;
            dValue_Array[3] = cDataValue.m_cTXResultData.m_dEdgeMaxReferenceData;
            dValue_Array[4] = Math.Max(cDataValue.m_cRXReferenceValue.m_nTotalMax, cDataValue.m_cTXReferenceValue.m_nTotalMax);

            return dValue_Array;
        }

        private double[] GetFrequencyReferenceValue(DataValue cDataValue, DataType eDataType)
        {
            double[] dValue_Array = new double[4] 
            { 
                -1.0, 
                -1.0, 
                -1.0, 
                -1.0 
            };

            if (eDataType == DataType.Beacon)
            {
                dValue_Array[0] = cDataValue.m_cRankData_Beacon.m_dRXInnerValue;
                dValue_Array[1] = cDataValue.m_cRankData_Beacon.m_dRXEdgeValue;
                dValue_Array[2] = cDataValue.m_cRankData_Beacon.m_dTXInnerValue;
                dValue_Array[3] = cDataValue.m_cRankData_Beacon.m_dTXEdgeValue;
            }
            else if (eDataType == DataType.PTHF)
            {
                dValue_Array[0] = cDataValue.m_cRankData_PTHF.m_dRXInnerValue;
                dValue_Array[1] = cDataValue.m_cRankData_PTHF.m_dRXEdgeValue;
                dValue_Array[2] = cDataValue.m_cRankData_PTHF.m_dTXInnerValue;
                dValue_Array[3] = cDataValue.m_cRankData_PTHF.m_dTXEdgeValue;
            }
            else if (eDataType == DataType.BHF)
            {
                dValue_Array[0] = cDataValue.m_cRankData_BHF.m_dRXInnerValue;
                dValue_Array[1] = cDataValue.m_cRankData_BHF.m_dRXEdgeValue;
                dValue_Array[2] = cDataValue.m_cRankData_BHF.m_dTXInnerValue;
                dValue_Array[3] = cDataValue.m_cRankData_BHF.m_dTXEdgeValue;
            }

            return dValue_Array;
        }

        private double[] GetFrequencyReferenceValueAndMaxValue(DataValue cDataValue, DataType eDataType)
        {
            double[] dValue_Array = new double[5] 
            { 
                -1.0, 
                -1.0, 
                -1.0, 
                -1.0,
                -1.0
            };

            if (eDataType == DataType.Beacon)
            {
                dValue_Array[0] = cDataValue.m_cRankData_Beacon.m_dRXInnerValue;
                dValue_Array[1] = cDataValue.m_cRankData_Beacon.m_dRXEdgeValue;
                dValue_Array[2] = cDataValue.m_cRankData_Beacon.m_dTXInnerValue;
                dValue_Array[3] = cDataValue.m_cRankData_Beacon.m_dTXEdgeValue;
                dValue_Array[4] = cDataValue.m_cRankData_Beacon.m_nTotalMaxValue;
            }
            else if (eDataType == DataType.PTHF)
            {
                dValue_Array[0] = cDataValue.m_cRankData_PTHF.m_dRXInnerValue;
                dValue_Array[1] = cDataValue.m_cRankData_PTHF.m_dRXEdgeValue;
                dValue_Array[2] = cDataValue.m_cRankData_PTHF.m_dTXInnerValue;
                dValue_Array[3] = cDataValue.m_cRankData_PTHF.m_dTXEdgeValue;
                dValue_Array[4] = cDataValue.m_cRankData_PTHF.m_nTotalMaxValue;
            }
            else if (eDataType == DataType.BHF)
            {
                dValue_Array[0] = cDataValue.m_cRankData_BHF.m_dRXInnerValue;
                dValue_Array[1] = cDataValue.m_cRankData_BHF.m_dRXEdgeValue;
                dValue_Array[2] = cDataValue.m_cRankData_BHF.m_dTXInnerValue;
                dValue_Array[3] = cDataValue.m_cRankData_BHF.m_dTXEdgeValue;
                dValue_Array[4] = cDataValue.m_cRankData_BHF.m_nTotalMaxValue;
            }

            return dValue_Array;
        }

        private void SaveFrequencyChart(int nDataType, string sTitleName, List<Series> serSeries_List, string sFilePath)
        {
            Chart cChart = new Chart();
            var vChartArea = new ChartArea();
            cChart.ChartAreas.Add(vChartArea);
            cChart.Width = 1500;
            cChart.Height = (nDataType != m_nTRACEAREATYPE_TOTAL) ? 303 : 324;
            cChart.Legends.Add("Legend");
            cChart.Legends["Legend"].Alignment = StringAlignment.Far;
            cChart.Legends["Legend"].Docking = Docking.Top;
            cChart.Legends["Legend"].Font = new Font("Times New Roman", 10);
            cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
            cChart.Titles.Add(sTitleName);
            cChart.Titles[0].Font = new Font("Times New Roman", 18);
            cChart.ChartAreas[0].AxisY.Title = "Reference Value";
            cChart.ChartAreas[0].AxisY.TitleFont = new Font("Times New Roman", 10);
            cChart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisX.Title = SpecificText.m_sFrequency_KHz;
            cChart.ChartAreas[0].AxisX.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisX.Interval = 1;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            cChart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            cChart.ChartAreas[0].AxisY.Minimum = 0;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            foreach (Series serSeries in serSeries_List)
                cChart.Series.Add(serSeries);

            cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
        }

        protected void SetDeepCopyForTotalDataList()
        {
            m_cTotalDataValue_List = new List<DataValue>();

            foreach (DataValue cDataValue in m_cDataValue_List)
                m_cTotalDataValue_List.Add(cDataValue.SetDeepCopy());

            m_cTotalDataInfo_List = new List<DataInfo>();

            foreach (DataInfo cDataInfo in m_cDataInfo_List)
                m_cTotalDataInfo_List.Add(cDataInfo.SetDeepCopy());

            foreach (DataInfo cDataInfo in m_cTotalDataInfo_List)
            {
                double dFrequency = cDataInfo.m_dFrequency;
                int nPH1 = cDataInfo.m_nReadPH1;
                int nPH2 = cDataInfo.m_nReadPH2;

                cDataInfo.m_sFileName = string.Format("TN_{0:0.000}_{1}_{2}", dFrequency, nPH1.ToString().PadLeft(2, '0'), nPH2.ToString().PadLeft(2, '0'));
            }
        }

        protected void GetPreviousStepData(bool bRXTraceTypeFlag)
        {
            string sCodeName = StringConvert.m_dictSubStepCNMappingTable[SubTuningStep.TILTNO_PTHF];
            string sResultFilePath = string.Format(@"{0}\{1}({2})", m_sFileDirectoryPath, SpecificText.m_sResultText, sCodeName);
            string sReferenceFilePath = string.Format(@"{0}\{1}", sResultFilePath, SpecificText.m_sReferenceText);

            if (bRXTraceTypeFlag == true)
                sReferenceFilePath = string.Format(@"{0}\{1}", sReferenceFilePath, SpecificText.m_sRXReferenceFileName);
            else
                sReferenceFilePath = string.Format(@"{0}\{1}", sReferenceFilePath, SpecificText.m_sTXReferenceFileName);

            string sLine;
            bool bReadReferenceValueTitleFlag = false, bReadMeanValueTitleFlag = false, bReadStdValueTitleFlag = false, bReadMaxValueTitleFlag = false, bReadColumnNameFlag = false;

            int nFileNameColumnIndex = 1;
            int nPH1ColumnIndex = 3;
            int nPH2ColumnIndex = 4;

            int nDataOffsetIndex = 6;

            StreamReader srFile = new StreamReader(sReferenceFilePath, Encoding.Default);

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    if (sLine == "Mean+3*Std Data")
                    {
                        bReadReferenceValueTitleFlag = true;
                        continue;
                    }
                    else if (sLine == "Mean Data")
                    {
                        bReadMeanValueTitleFlag = true;
                        continue;
                    }
                    else if (sLine == "Std Data")
                    {
                        bReadStdValueTitleFlag = true;
                        continue;
                    }
                    else if (sLine == "Max Data")
                    {
                        bReadMaxValueTitleFlag = true;
                        continue;
                    }
                    else if (sLine == "")
                    {
                        bReadReferenceValueTitleFlag = false;
                        bReadMeanValueTitleFlag = false;
                        bReadStdValueTitleFlag = false;
                        bReadMaxValueTitleFlag = false;
                        bReadColumnNameFlag = false;
                        continue;
                    }

                    if (bReadReferenceValueTitleFlag == true)
                    {
                        string[] sData_Array = sLine.Split(',');

                        if (bReadColumnNameFlag == false)
                        {
                            GetColumnNameIndex(ref nFileNameColumnIndex, sData_Array, SpecificText.m_sFileName);
                            GetColumnNameIndex(ref nPH1ColumnIndex, sData_Array, SpecificText.m_sPH1);
                            GetColumnNameIndex(ref nPH2ColumnIndex, sData_Array, SpecificText.m_sPH2);
                            bReadColumnNameFlag = true;
                            continue;
                        }
                        else
                        {
                            if (sData_Array.Length < nDataOffsetIndex)
                                continue;

                            string sFileName = sData_Array[nFileNameColumnIndex];
                            int nPH1Value = Convert.ToInt32(sData_Array[nPH1ColumnIndex]);
                            int nPH2Value = Convert.ToInt32(sData_Array[nPH2ColumnIndex]);

                            DataValue cDataValue = m_cTotalDataValue_List.Find(x => x.m_nPH1 == nPH1Value && x.m_nPH2 == nPH2Value);

                            if (cDataValue != null)
                            {
                                if (bRXTraceTypeFlag == true)
                                {
                                    if (cDataValue.m_cRXPreviousStepData == null)
                                    {
                                        cDataValue.m_cRXPreviousStepData = new PreviousStepData();
                                        cDataValue.m_cRXPreviousStepData.InitializeArray(m_nRXTraceNumber);
                                    }

                                    for (int nIndex = nDataOffsetIndex; nIndex < sData_Array.Length; nIndex++)
                                        cDataValue.m_cRXPreviousStepData.m_dReferenceValue_Array[nIndex - nDataOffsetIndex] = Convert.ToDouble(sData_Array[nIndex]);
                                }
                                else
                                {
                                    if (cDataValue.m_cTXPreviousStepData == null)
                                    {
                                        cDataValue.m_cTXPreviousStepData = new PreviousStepData();
                                        cDataValue.m_cTXPreviousStepData.InitializeArray(m_nTXTraceNumber);
                                    }

                                    for (int nIndex = nDataOffsetIndex; nIndex < sData_Array.Length; nIndex++)
                                        cDataValue.m_cTXPreviousStepData.m_dReferenceValue_Array[nIndex - nDataOffsetIndex] = Convert.ToDouble(sData_Array[nIndex]);
                                }
                            }
                        }
                    }
                    else if (bReadMeanValueTitleFlag == true)
                    {
                        string[] sData_Array = sLine.Split(',');

                        if (bReadColumnNameFlag == false)
                        {
                            GetColumnNameIndex(ref nFileNameColumnIndex, sData_Array, SpecificText.m_sFileName);
                            GetColumnNameIndex(ref nPH1ColumnIndex, sData_Array, SpecificText.m_sPH1);
                            GetColumnNameIndex(ref nPH2ColumnIndex, sData_Array, SpecificText.m_sPH2);
                            bReadColumnNameFlag = true;
                            continue;
                        }
                        else
                        {
                            if (sData_Array.Length < nDataOffsetIndex)
                                continue;

                            string sFileName = sData_Array[nFileNameColumnIndex];
                            int nPH1Value = Convert.ToInt32(sData_Array[nPH1ColumnIndex]);
                            int nPH2Value = Convert.ToInt32(sData_Array[nPH2ColumnIndex]);

                            DataValue cDataValue = m_cTotalDataValue_List.Find(x => x.m_nPH1 == nPH1Value && x.m_nPH2 == nPH2Value);

                            if (cDataValue != null)
                            {
                                if (bRXTraceTypeFlag == true)
                                {
                                    if (cDataValue.m_cRXPreviousStepData == null)
                                    {
                                        cDataValue.m_cRXPreviousStepData = new PreviousStepData();
                                        cDataValue.m_cRXPreviousStepData.InitializeArray(m_nRXTraceNumber);
                                    }

                                    for (int nIndex = nDataOffsetIndex; nIndex < sData_Array.Length; nIndex++)
                                        cDataValue.m_cRXPreviousStepData.m_dMeanValue_Array[nIndex - nDataOffsetIndex] = Convert.ToDouble(sData_Array[nIndex]);
                                }
                                else
                                {
                                    if (cDataValue.m_cTXPreviousStepData == null)
                                    {
                                        cDataValue.m_cTXPreviousStepData = new PreviousStepData();
                                        cDataValue.m_cTXPreviousStepData.InitializeArray(m_nTXTraceNumber);
                                    }

                                    for (int nIndex = nDataOffsetIndex; nIndex < sData_Array.Length; nIndex++)
                                        cDataValue.m_cTXPreviousStepData.m_dMeanValue_Array[nIndex - nDataOffsetIndex] = Convert.ToDouble(sData_Array[nIndex]);
                                }
                            }
                        }
                    }
                    else if (bReadStdValueTitleFlag == true)
                    {
                        string[] sData_Array = sLine.Split(',');

                        if (bReadColumnNameFlag == false)
                        {
                            GetColumnNameIndex(ref nFileNameColumnIndex, sData_Array, SpecificText.m_sFileName);
                            GetColumnNameIndex(ref nPH1ColumnIndex, sData_Array, SpecificText.m_sPH1);
                            GetColumnNameIndex(ref nPH2ColumnIndex, sData_Array, SpecificText.m_sPH2);
                            bReadColumnNameFlag = true;
                            continue;
                        }
                        else
                        {
                            if (sData_Array.Length < nDataOffsetIndex)
                                continue;

                            string sFileName = sData_Array[nFileNameColumnIndex];
                            int nPH1Value = Convert.ToInt32(sData_Array[nPH1ColumnIndex]);
                            int nPH2Value = Convert.ToInt32(sData_Array[nPH2ColumnIndex]);

                            DataValue cDataValue = m_cTotalDataValue_List.Find(x => x.m_nPH1 == nPH1Value && x.m_nPH2 == nPH2Value);

                            if (cDataValue != null)
                            {
                                if (bRXTraceTypeFlag == true)
                                {
                                    if (cDataValue.m_cRXPreviousStepData == null)
                                    {
                                        cDataValue.m_cRXPreviousStepData = new PreviousStepData();
                                        cDataValue.m_cRXPreviousStepData.InitializeArray(m_nRXTraceNumber);
                                    }

                                    for (int nIndex = nDataOffsetIndex; nIndex < sData_Array.Length; nIndex++)
                                        cDataValue.m_cRXPreviousStepData.m_dStdevValue_Array[nIndex - nDataOffsetIndex] = Convert.ToDouble(sData_Array[nIndex]);
                                }
                                else
                                {
                                    if (cDataValue.m_cTXPreviousStepData == null)
                                    {
                                        cDataValue.m_cTXPreviousStepData = new PreviousStepData();
                                        cDataValue.m_cTXPreviousStepData.InitializeArray(m_nTXTraceNumber);
                                    }

                                    for (int nIndex = nDataOffsetIndex; nIndex < sData_Array.Length; nIndex++)
                                        cDataValue.m_cTXPreviousStepData.m_dStdevValue_Array[nIndex - nDataOffsetIndex] = Convert.ToDouble(sData_Array[nIndex]);
                                }
                            }
                        }
                    }
                    else if (bReadMaxValueTitleFlag == true)
                    {
                        string[] sData_Array = sLine.Split(',');

                        if (bReadColumnNameFlag == false)
                        {
                            GetColumnNameIndex(ref nFileNameColumnIndex, sData_Array, SpecificText.m_sFileName);
                            GetColumnNameIndex(ref nPH1ColumnIndex, sData_Array, SpecificText.m_sPH1);
                            GetColumnNameIndex(ref nPH2ColumnIndex, sData_Array, SpecificText.m_sPH2);
                            bReadColumnNameFlag = true;
                            continue;
                        }
                        else
                        {
                            if (sData_Array.Length < nDataOffsetIndex)
                                continue;

                            string sFileName = sData_Array[nFileNameColumnIndex];
                            int nPH1Value = Convert.ToInt32(sData_Array[nPH1ColumnIndex]);
                            int nPH2Value = Convert.ToInt32(sData_Array[nPH2ColumnIndex]);

                            DataValue cDataValue = m_cTotalDataValue_List.Find(x => x.m_nPH1 == nPH1Value && x.m_nPH2 == nPH2Value);

                            if (cDataValue != null)
                            {
                                if (bRXTraceTypeFlag == true)
                                {
                                    if (cDataValue.m_cRXPreviousStepData == null)
                                    {
                                        cDataValue.m_cRXPreviousStepData = new PreviousStepData();
                                        cDataValue.m_cRXPreviousStepData.InitializeArray(m_nRXTraceNumber);
                                    }

                                    for (int nIndex = nDataOffsetIndex; nIndex < sData_Array.Length; nIndex++)
                                        cDataValue.m_cRXPreviousStepData.m_nMaxValue_Array[nIndex - nDataOffsetIndex] = Convert.ToInt32(sData_Array[nIndex]);

                                    int nMaxValue = 0;

                                    for (int nIndex = nDataOffsetIndex; nIndex < sData_Array.Length; nIndex++)
                                    {
                                        if (Convert.ToInt32(sData_Array[nIndex]) > nMaxValue)
                                            nMaxValue = Convert.ToInt32(sData_Array[nIndex]);
                                    }

                                    cDataValue.m_cRXPreviousStepData.m_nTotalMaxValue = nMaxValue;
                                }
                                else
                                {
                                    if (cDataValue.m_cTXPreviousStepData == null)
                                    {
                                        cDataValue.m_cTXPreviousStepData = new PreviousStepData();
                                        cDataValue.m_cTXPreviousStepData.InitializeArray(m_nTXTraceNumber);
                                    }

                                    for (int nIndex = nDataOffsetIndex; nIndex < sData_Array.Length; nIndex++)
                                        cDataValue.m_cTXPreviousStepData.m_nMaxValue_Array[nIndex - nDataOffsetIndex] = Convert.ToInt32(sData_Array[nIndex]);

                                    int nMaxValue = 0;

                                    for (int nIndex = nDataOffsetIndex; nIndex < sData_Array.Length; nIndex++)
                                    {
                                        if (Convert.ToInt32(sData_Array[nIndex]) > nMaxValue)
                                            nMaxValue = Convert.ToInt32(sData_Array[nIndex]);
                                    }

                                    cDataValue.m_cTXPreviousStepData.m_nTotalMaxValue = nMaxValue;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                srFile.Close();
            }
        }

        private void GetColumnNameIndex(ref int nColumnNameIndex, string[] sData_Array, string sColumnName)
        {
            int nMatchColumnIndex = -1;

            for (int nColumnIndex = 0; nColumnIndex < sData_Array.Length; nColumnIndex++)
            {
                if (sData_Array[nColumnIndex] == sColumnName)
                {
                    nMatchColumnIndex = nColumnIndex;
                    break;
                }
            }

            if (nMatchColumnIndex > -1)
                nColumnNameIndex = nMatchColumnIndex;
        }

        protected bool ComputeTotalReferenceValue()
        {
            string[] sTraceType_Array = new string[]
            {
                StringConvert.m_sTRACETYPE_RX,
                StringConvert.m_sTRACETYPE_TX
            };

            for (int nDataIndex = 0; nDataIndex < m_cTotalDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];
                DataValue cDataValue = m_cDataValue_List[nDataIndex];
                DataInfo cTotalDataInfo = m_cTotalDataInfo_List[nDataIndex];
                DataValue cTotalDataValue = m_cTotalDataValue_List[nDataIndex];

                int nErrorFlag = 0;

                foreach (string sTraceType in sTraceType_Array)
                {
                    ReferenceData cReferenceData = cDataValue.m_cRXReferenceData;
                    PreviousStepData cPreviousStepData = cTotalDataValue.m_cRXPreviousStepData;
                    int nTraceNumber = cDataInfo.m_nRXTraceNumber;

                    if (sTraceType == StringConvert.m_sTRACETYPE_RX)
                    {
                        cReferenceData = cDataValue.m_cRXReferenceData;
                        cPreviousStepData = cTotalDataValue.m_cRXPreviousStepData;
                        nTraceNumber = cDataInfo.m_nRXTraceNumber;
                    }
                    else if (sTraceType == StringConvert.m_sTRACETYPE_TX)
                    {
                        cReferenceData = cDataValue.m_cTXReferenceData;
                        cPreviousStepData = cTotalDataValue.m_cTXPreviousStepData;
                        nTraceNumber = cDataInfo.m_nTXTraceNumber;
                    }

                    if (cPreviousStepData != null)
                    {
                        double dEdgeMaxValue = 0.0;
                        int nEdgeMaxIndex = 0;
                        double dInnerMaxValue = 0.0;
                        int nInnerMaxIndex = 0;
                        double dTotalMaxValue = 0.0;
                        int nTotalMaxIndex = 0;

                        for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                        {
                            double dValue = Math.Round((cReferenceData.m_dMeanPlus3Std_Array[nTraceIndex] + cPreviousStepData.m_dReferenceValue_Array[nTraceIndex]) / 2.0, 2, MidpointRounding.AwayFromZero);
                            cReferenceData.m_dMeanPlus3Std_Array[nTraceIndex] = dValue;

                            if (nTraceIndex < m_nEdgeTraceNumber || nTraceIndex >= nTraceNumber - m_nEdgeTraceNumber)
                            {
                                if (nTraceIndex == 0)
                                {
                                    dEdgeMaxValue = cPreviousStepData.m_dReferenceValue_Array[nTraceIndex];
                                    nEdgeMaxIndex = nTraceIndex;
                                }
                                else
                                {
                                    if (cPreviousStepData.m_dReferenceValue_Array[nTraceIndex] > dEdgeMaxValue)
                                    {
                                        dEdgeMaxValue = cPreviousStepData.m_dReferenceValue_Array[nTraceIndex];
                                        nEdgeMaxIndex = nTraceIndex;
                                    }
                                }
                            }
                            else
                            {
                                if (nTraceIndex == m_nEdgeTraceNumber)
                                {
                                    dInnerMaxValue = cPreviousStepData.m_dReferenceValue_Array[nTraceIndex];
                                    nInnerMaxIndex = nTraceIndex;
                                }
                                else
                                {
                                    if (cPreviousStepData.m_dReferenceValue_Array[nTraceIndex] > dInnerMaxValue)
                                    {
                                        dInnerMaxValue = cPreviousStepData.m_dReferenceValue_Array[nTraceIndex];
                                        nInnerMaxIndex = nTraceIndex;
                                    }
                                }
                            }

                            if (nTraceIndex == 0)
                            {
                                dTotalMaxValue = cPreviousStepData.m_dReferenceValue_Array[nTraceIndex];
                                nTotalMaxIndex = nTraceIndex;
                            }
                            else
                            {
                                if (cPreviousStepData.m_dReferenceValue_Array[nTraceIndex] > dTotalMaxValue)
                                {
                                    dTotalMaxValue = cTotalDataValue.m_cRXPreviousStepData.m_dReferenceValue_Array[nTraceIndex];
                                    nTotalMaxIndex = nTraceIndex;
                                }
                            }
                        }

                        if (sTraceType == StringConvert.m_sTRACETYPE_RX)
                        {
                            cTotalDataValue.m_cRXPreviousStepData.m_dEdgeMaxReferenceData = dEdgeMaxValue;
                            cTotalDataValue.m_cRXPreviousStepData.m_nEdgeMaxIndex = nEdgeMaxIndex;
                            cTotalDataValue.m_cRXPreviousStepData.m_dInnerMaxReferenceData = dInnerMaxValue;
                            cTotalDataValue.m_cRXPreviousStepData.m_nInnerMaxIndex = nInnerMaxIndex;
                            cTotalDataValue.m_cRXPreviousStepData.m_dTotalMaxReferenceData = dTotalMaxValue;
                            cTotalDataValue.m_cRXPreviousStepData.m_nTotalMaxIndex = nTotalMaxIndex;
                            cTotalDataValue.m_bGetPTHFRXReferenceDataFlag = true;
                        }
                        else if (sTraceType == StringConvert.m_sTRACETYPE_TX)
                        {
                            cTotalDataValue.m_cTXPreviousStepData.m_dEdgeMaxReferenceData = dEdgeMaxValue;
                            cTotalDataValue.m_cTXPreviousStepData.m_nEdgeMaxIndex = nEdgeMaxIndex;
                            cTotalDataValue.m_cTXPreviousStepData.m_dInnerMaxReferenceData = dInnerMaxValue;
                            cTotalDataValue.m_cTXPreviousStepData.m_nInnerMaxIndex = nInnerMaxIndex;
                            cTotalDataValue.m_cTXPreviousStepData.m_dTotalMaxReferenceData = dTotalMaxValue;
                            cTotalDataValue.m_cTXPreviousStepData.m_nTotalMaxIndex = nTotalMaxIndex;
                            cTotalDataValue.m_bGetPTHFTXReferenceDataFlag = true;
                        }
                    }
                }

                if (cTotalDataValue.m_bGetPTHFRXReferenceDataFlag == false)
                {
                    cTotalDataInfo.m_nErrorFlag = nErrorFlag |= 0x1000;
                    cTotalDataInfo.m_sErrorMessage = "Get PTHF RX Data Error";
                    m_sErrorMessage = "Get PTHF RX Data Error";
                    m_bErrorFlag = true;
                    return false;
                }

                if (cTotalDataValue.m_bGetPTHFTXReferenceDataFlag == false)
                {
                    cTotalDataInfo.m_nErrorFlag = nErrorFlag |= 0x2000;
                    cTotalDataInfo.m_sErrorMessage = "Get PTHF TX Data Error";
                    m_sErrorMessage = "Get PTHF TX Data Error";
                    m_bErrorFlag = true;
                    return false;
                }
            }

            return true;
        }

        protected void ComputeTotalResultData()
        {
            string[] sTraceType_Array = new string[]
            {
                StringConvert.m_sTRACETYPE_RX,
                StringConvert.m_sTRACETYPE_TX
            };

            for (int nDataIndex = 0; nDataIndex < m_cTotalDataValue_List.Count; nDataIndex++)
            {
                DataValue cDataValue = m_cDataValue_List[nDataIndex];
                DataValue cTotalDataValue = m_cTotalDataValue_List[nDataIndex];

                foreach (string sTraceType in sTraceType_Array)
                {
                    ResultData cResultData = cDataValue.m_cRXResultData;
                    PreviousStepData cPreviousStepData = cTotalDataValue.m_cRXPreviousStepData;

                    if (sTraceType == StringConvert.m_sTRACETYPE_RX)
                    {
                        cResultData = cDataValue.m_cRXResultData;
                        cPreviousStepData = cTotalDataValue.m_cRXPreviousStepData;
                    }
                    else if (sTraceType == StringConvert.m_sTRACETYPE_TX)
                    {
                        cResultData = cDataValue.m_cTXResultData;
                        cPreviousStepData = cTotalDataValue.m_cTXPreviousStepData;
                    }

                    double dEdgeMaxReferenceData = Math.Max(cResultData.m_dEdgeMaxReferenceData, cPreviousStepData.m_dEdgeMaxReferenceData);
                    double dInnerMaxReferenceData = Math.Max(cResultData.m_dInnerMaxReferenceData, cPreviousStepData.m_dInnerMaxReferenceData);
                    double dTotalMaxReferenceData = Math.Max(cResultData.m_dTotalMaxReferenceData, cPreviousStepData.m_dTotalMaxReferenceData);
                    
                    /*
                    double dEdgeMaxReferenceData = Math.Round((cResultData.m_dEdgeMaxReferenceData + cPreviousStepData.m_dEdgeMaxReferenceData) / 2.0, 2, MidpointRounding.AwayFromZero);
                    double dInnerMaxReferenceData = Math.Round((cResultData.m_dInnerMaxReferenceData + cPreviousStepData.m_dInnerMaxReferenceData) / 2.0, 2, MidpointRounding.AwayFromZero);
                    double dTotalMaxReferenceData = Math.Round((cResultData.m_dTotalMaxReferenceData + cPreviousStepData.m_dTotalMaxReferenceData) / 2.0, 2, MidpointRounding.AwayFromZero);
                    */

                    if (sTraceType == StringConvert.m_sTRACETYPE_RX)
                    {
                        cTotalDataValue.m_cRXResultData.m_dEdgeMaxReferenceData = dEdgeMaxReferenceData;
                        cTotalDataValue.m_cRXResultData.m_dInnerMaxReferenceData = dInnerMaxReferenceData;
                        cTotalDataValue.m_cRXResultData.m_dTotalMaxReferenceData = dTotalMaxReferenceData;

                        cTotalDataValue.m_cRXReferenceValue.m_nTotalMax = Math.Max(cTotalDataValue.m_cRXReferenceValue.m_nTotalMax, cTotalDataValue.m_cRXPreviousStepData.m_nTotalMaxValue);
                    }
                    else if (sTraceType == StringConvert.m_sTRACETYPE_TX)
                    {
                        cTotalDataValue.m_cTXResultData.m_dEdgeMaxReferenceData = dEdgeMaxReferenceData;
                        cTotalDataValue.m_cTXResultData.m_dInnerMaxReferenceData = dInnerMaxReferenceData;
                        cTotalDataValue.m_cTXResultData.m_dTotalMaxReferenceData = dTotalMaxReferenceData;

                        cTotalDataValue.m_cTXReferenceValue.m_nTotalMax = Math.Max(cTotalDataValue.m_cTXReferenceValue.m_nTotalMax, cTotalDataValue.m_cTXPreviousStepData.m_nTotalMaxValue);
                    }
                }
            }
        }

        protected void GetPreviousStepListData(DataType eDataType)
        {
            string sFilePath = "";

            if (eDataType == DataType.Beacon)
                sFilePath = m_sNoiseResultFilePath;
            else if (eDataType == DataType.PTHF)
                sFilePath = m_sPTHFResultFilePath;
            else if (eDataType == DataType.BHF)
                sFilePath = m_sBHFResultFilePath;

            string sLine;
            bool bReadTitleFlag = false, bReadColumnNameFlag = false;

            int nRankColumnIndex = 0;
            int nFrequencyColumnIndex = 2;
            int nPH1ColumnIndex = 3;
            int nPH2ColumnIndex = 4;
            int nTotalMaxColumnIndex = 5;
            int nTXInnerValueColumnIndex = 6;
            int nRXInnerValueColumnIndex = 7;
            int nTXEdgeValueColumnIndex = 8;
            int nRXEdgeValueColumnIndex = 9;

            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    if (sLine == "Ranking Data(Weighting Ranking)")
                    {
                        bReadTitleFlag = true;
                        continue;
                    }
                    else if (sLine == "")
                    {
                        bReadTitleFlag = false;
                        continue;
                    }

                    if (bReadTitleFlag == true)
                    {
                        string[] sData_Array = sLine.Split(',');

                        if (bReadColumnNameFlag == false)
                        {
                            GetColumnNameIndex(ref nRankColumnIndex, sData_Array, SpecificText.m_sRanking);
                            GetColumnNameIndex(ref nFrequencyColumnIndex, sData_Array, SpecificText.m_sFrequency);
                            GetColumnNameIndex(ref nPH1ColumnIndex, sData_Array, SpecificText.m_sPH1);
                            GetColumnNameIndex(ref nPH2ColumnIndex, sData_Array, SpecificText.m_sPH2);
                            GetColumnNameIndex(ref nTotalMaxColumnIndex, sData_Array, SpecificText.m_sTotalMax);
                            GetColumnNameIndex(ref nTXInnerValueColumnIndex, sData_Array, SpecificText.m_sInnerTX);
                            GetColumnNameIndex(ref nRXInnerValueColumnIndex, sData_Array, SpecificText.m_sInnerRX);
                            GetColumnNameIndex(ref nTXEdgeValueColumnIndex, sData_Array, SpecificText.m_sEdgeTX);
                            GetColumnNameIndex(ref nRXEdgeValueColumnIndex, sData_Array, SpecificText.m_sEdgeRX);
                            bReadColumnNameFlag = true;
                            continue;
                        }
                        else
                        {
                            int nRankIndex = Convert.ToInt32(sData_Array[nRankColumnIndex]);
                            double dFrequency = Convert.ToDouble(sData_Array[nFrequencyColumnIndex]);
                            int nPH1 = Convert.ToInt32(sData_Array[nPH1ColumnIndex]);
                            int nPH2 = Convert.ToInt32(sData_Array[nPH2ColumnIndex]);

                            DataValue cDataValue = m_cTotalDataValue_List.Find(x => x.m_nPH1 == nPH1 && x.m_nPH2 == nPH2);

                            if (cDataValue == null)
                                continue;

                            int nTotalMaxValue = Convert.ToInt32(sData_Array[nTotalMaxColumnIndex]);
                            double dTXInnerValue = Convert.ToDouble(sData_Array[nTXInnerValueColumnIndex]);
                            double dRXInnerValue = Convert.ToDouble(sData_Array[nRXInnerValueColumnIndex]);
                            double dTXEdgeValue = Convert.ToDouble(sData_Array[nTXEdgeValueColumnIndex]);
                            double dRXEdgeValue = Convert.ToDouble(sData_Array[nRXEdgeValueColumnIndex]);

                            RankData cRankData = new RankData();
                            cRankData.m_nRankIndex = nRankIndex;
                            cRankData.m_nTotalMaxValue = nTotalMaxValue;
                            cRankData.m_dTXInnerValue = dTXInnerValue;
                            cRankData.m_dRXInnerValue = dRXInnerValue;
                            cRankData.m_dTXEdgeValue = dTXEdgeValue;
                            cRankData.m_dRXEdgeValue = dRXEdgeValue;

                            if (eDataType == DataType.Beacon)
                            {
                                cDataValue.m_cRankData_Beacon = new RankData();
                                cDataValue.m_cRankData_Beacon = cRankData;
                            }
                            else if (eDataType == DataType.PTHF)
                            {
                                cDataValue.m_cRankData_PTHF = new RankData();
                                cDataValue.m_cRankData_PTHF = cRankData;
                            }
                            else if (eDataType == DataType.BHF)
                            {
                                cDataValue.m_cRankData_BHF = new RankData();
                                cDataValue.m_cRankData_BHF = cRankData;
                            }
                        }
                    }
                }
            }
            finally
            {
                srFile.Close();
            }
        }

        protected bool SaveFrequencyLineChartPictureFileBy3SubPlot()
        {
            List<double> dAllFrequency_List = new List<double>();

            double dFrequencyHB = ParamAutoTuning.m_nFrequencyHB;
            double dFrequencyLB = ParamAutoTuning.m_nFrequencyLB_MPP180;
            int nAnalogValueHB = (int)(MainConstantParameter.m_nICCLOCKFREQUENCY / (ParamAutoTuning.m_nFrequencyLB_MPP180 * 1000));
            int nAnalogValueLB = (int)(MainConstantParameter.m_nICCLOCKFREQUENCY / (ParamAutoTuning.m_nFrequencyHB * 1000));

            for (int nSetIndex = 0; nSetIndex < m_cDataInfo_List.Count; nSetIndex++)
            {
                if (nSetIndex == 0)
                {
                    dFrequencyLB = m_cDataInfo_List[nSetIndex].m_dFrequency;
                    dFrequencyHB = m_cDataInfo_List[nSetIndex].m_dFrequency;
                }
                else
                {
                    if (m_cDataInfo_List[nSetIndex].m_dFrequency < dFrequencyLB)
                        dFrequencyLB = m_cDataInfo_List[nSetIndex].m_dFrequency;

                    if (m_cDataInfo_List[nSetIndex].m_dFrequency > dFrequencyHB)
                        dFrequencyHB = m_cDataInfo_List[nSetIndex].m_dFrequency;
                }
            }

            nAnalogValueHB = (int)Math.Round(MainConstantParameter.m_nICCLOCKFREQUENCY / (dFrequencyLB * 1000), 0, MidpointRounding.AwayFromZero);
            nAnalogValueLB = (int)Math.Round(MainConstantParameter.m_nICCLOCKFREQUENCY / (dFrequencyHB * 1000), 0, MidpointRounding.AwayFromZero);

            for (int nAnalogValue = nAnalogValueHB; nAnalogValue >= nAnalogValueLB; nAnalogValue--)
            {
                double dFrequency = Math.Round((double)MainConstantParameter.m_nICCLOCKFREQUENCY / (nAnalogValue * 1000), 3, MidpointRounding.AwayFromZero);
                dAllFrequency_List.Add(dFrequency);
            }

            List<int> nAllFrequencyAmount_List = new List<int>();

            for (int nFrequencyIndex = 0; nFrequencyIndex < dAllFrequency_List.Count; nFrequencyIndex++)
            {
                nAllFrequencyAmount_List.Add(0);

                for (int nSetIndex = 0; nSetIndex < m_cTotalDataInfo_List.Count; nSetIndex++)
                {
                    if (m_cTotalDataInfo_List[nSetIndex].m_dFrequency == dAllFrequency_List[nFrequencyIndex] && (m_cTotalDataInfo_List[nSetIndex].m_sErrorMessage == "" || m_cTotalDataInfo_List[nSetIndex].m_bInnerReferenceValueOverHBFlag == true))
                        nAllFrequencyAmount_List[nFrequencyIndex]++;
                }
            }

            SaveFrequencyLineChartBy3SubPlot(dAllFrequency_List, nAllFrequencyAmount_List);

            return true;
        }

        private void SaveFrequencyLineChartBy3SubPlot(List<double> dAllFrequencyRange_List, List<int> nAllFrequencyAmount_List)
        {
            string sFilePath = string.Format(@"{0}\\{1}", m_sPictureFolderPath, SpecificText.m_sFrqChartBy3SubPlotFileName);
            string sIncludeMaxFilePath = string.Format(@"{0}\\{1}", m_sPictureFolderPath, SpecificText.m_sFrqChartBy3SubPlotIncludeMaxFileName);

            DataType[] eDataType_Array = new DataType[]
            {
                DataType.Beacon,
                DataType.PTHF,
                DataType.BHF
            };

            List<Series>[] serDataChart_Array = new List<Series>[3];
            List<Series>[] serDataChart_IncludeMax_Array = new List<Series>[3];

            for (int nDataIndex = 0; nDataIndex < eDataType_Array.Length; nDataIndex++)
            {
                DataType eDataType = eDataType_Array[nDataIndex];

                if (eDataType == DataType.Beacon)
                {
                    if (m_bGetBeaconDataFlag == false)
                        continue;
                }


                string sDataType = eDataType.ToString();
                string sTitleName = sDataType;

                List<Series> serChartSeries_List = new List<Series>();
                List<Series> serChartSeries_IncludeMax_List = new List<Series>();

                Series serRXInner = new Series(string.Format("{0} RX Inner", sDataType));
                Series serRXEdge = new Series(string.Format("{0} RX Edge", sDataType));
                Series serTXInner = new Series(string.Format("{0} TX Inner", sDataType));
                Series serTXEdge = new Series(string.Format("{0} TX Edge", sDataType));

                Series serRXInner_IncludeMax = new Series(string.Format("{0} RX Inner", sDataType));
                Series serRXEdge_IncludeMax = new Series(string.Format("{0} RX Edge", sDataType));
                Series serTXInner_IncludeMax = new Series(string.Format("{0} TX Inner", sDataType));
                Series serTXEdge_IncludeMax = new Series(string.Format("{0} TX Edge", sDataType));
                Series serTotalMax_IncludeMax = new Series(string.Format("{0} Total Max", sDataType));

                Series[] serDataType_Array = new Series[4] 
                { 
                    serRXInner, 
                    serRXEdge, 
                    serTXInner, 
                    serTXEdge 
                };

                Color[] colorBackColor_Array = new Color[4] 
                { 
                    Color.Blue, 
                    Color.Orange, 
                    Color.Green, 
                    Color.Purple 
                };

                Series[] serDataType_IncludeMax_Array = new Series[5] 
                { 
                    serRXInner_IncludeMax, 
                    serRXEdge_IncludeMax, 
                    serTXInner_IncludeMax, 
                    serTXEdge_IncludeMax,
                    serTotalMax_IncludeMax
                };

                Color[] colorBackColor_IncludeMax_Array = new Color[5] 
                { 
                    Color.Blue, 
                    Color.Orange, 
                    Color.Green, 
                    Color.Purple,
                    Color.Red
                };

                for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                {
                    serDataType_Array[nTypeIndex].ChartType = SeriesChartType.Line;
                    serDataType_Array[nTypeIndex].MarkerStyle = MarkerStyle.Circle;
                    serDataType_Array[nTypeIndex].Color = colorBackColor_Array[nTypeIndex];
                }

                for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                {
                    if (nTypeIndex < serDataType_IncludeMax_Array.Length - 1)
                    {
                        serDataType_IncludeMax_Array[nTypeIndex].ChartType = SeriesChartType.Line;
                        serDataType_IncludeMax_Array[nTypeIndex].BorderWidth = 2;
                        serDataType_IncludeMax_Array[nTypeIndex].MarkerStyle = MarkerStyle.Circle;
                    }
                    else
                    {
                        serDataType_IncludeMax_Array[nTypeIndex].ChartType = SeriesChartType.Line;
                        serDataType_IncludeMax_Array[nTypeIndex].BorderDashStyle = ChartDashStyle.DashDot;
                        serDataType_IncludeMax_Array[nTypeIndex].BorderWidth = 1;
                        serDataType_IncludeMax_Array[nTypeIndex].MarkerStyle = MarkerStyle.Circle;
                    }

                    serDataType_IncludeMax_Array[nTypeIndex].Color = colorBackColor_IncludeMax_Array[nTypeIndex];
                }

                int nCount = 0;
                bool bHasDataFlag = false;

                double[] dLastValue_Array = new double[4] 
                { 
                    -1.0, 
                    -1.0, 
                    -1.0, 
                    -1.0 
                };

                double[] dLastValue_IncludeMax_Array = new double[5] 
                { 
                    -1.0, 
                    -1.0, 
                    -1.0, 
                    -1.0,
                    -1.0
                };

                for (int nFrequencyIndex = 0; nFrequencyIndex < dAllFrequencyRange_List.Count; nFrequencyIndex++)
                {
                    if (nAllFrequencyAmount_List[nFrequencyIndex] > 0)
                    {
                        for (int nSetIndex = 0; nSetIndex < m_cTotalDataInfo_List.Count; nSetIndex++)
                        {
                            DataInfo cTotalDataInfo = m_cTotalDataInfo_List[nSetIndex];
                            DataValue cTotalDataValue = m_cTotalDataValue_List[nSetIndex];

                            if (cTotalDataInfo.m_dFrequency == dAllFrequencyRange_List[nFrequencyIndex] && (cTotalDataInfo.m_sErrorMessage == "" || cTotalDataInfo.m_bInnerReferenceValueOverHBFlag == true))
                            {
                                dLastValue_Array = GetFrequencyReferenceValue(cTotalDataValue, eDataType);

                                for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                                {
                                    serDataType_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, dLastValue_Array[nTypeIndex]);
                                    serDataType_Array[nTypeIndex].Points[nCount].AxisLabel = dAllFrequencyRange_List[nFrequencyIndex].ToString("F3");
                                }

                                dLastValue_IncludeMax_Array = GetFrequencyReferenceValueAndMaxValue(cTotalDataValue, eDataType);

                                for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                                {
                                    serDataType_IncludeMax_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, dLastValue_IncludeMax_Array[nTypeIndex]);
                                    serDataType_IncludeMax_Array[nTypeIndex].Points[nCount].AxisLabel = dAllFrequencyRange_List[nFrequencyIndex].ToString("F3");
                                }

                                bHasDataFlag = true;
                                nCount++;
                            }
                        }
                    }
                    else
                    {
                        if (bHasDataFlag == true)
                        {
                            for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                                serDataType_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, dLastValue_Array[nTypeIndex] * -1);

                            for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                                serDataType_IncludeMax_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, dLastValue_IncludeMax_Array[nTypeIndex] * -1);

                            bHasDataFlag = false;
                        }
                        else
                        {
                            if (nFrequencyIndex < dAllFrequencyRange_List.Count - 1 && nAllFrequencyAmount_List[nFrequencyIndex + 1] > 0)
                            {
                                for (int nSetIndex = 0; nSetIndex < m_cTotalDataInfo_List.Count; nSetIndex++)
                                {
                                    DataInfo cTotalDataInfo = m_cTotalDataInfo_List[nSetIndex];
                                    DataValue cTotalDataValue = m_cTotalDataValue_List[nSetIndex];

                                    if (cTotalDataInfo.m_dFrequency == dAllFrequencyRange_List[nFrequencyIndex + 1] && (cTotalDataInfo.m_sErrorMessage == "" || cTotalDataInfo.m_bInnerReferenceValueOverHBFlag == true))
                                    {
                                        double[] dPreviousValue_Array = GetFrequencyReferenceValue(cTotalDataValue, eDataType);

                                        for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                                            serDataType_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, dPreviousValue_Array[nTypeIndex] * -1);

                                        double[] dPreviousMaxValue_Array = GetFrequencyReferenceValueAndMaxValue(cTotalDataValue, eDataType);

                                        for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                                            serDataType_IncludeMax_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, dPreviousMaxValue_Array[nTypeIndex] * -1);

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                                    serDataType_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, -1);

                                for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                                    serDataType_IncludeMax_Array[nTypeIndex].Points.AddXY(nFrequencyIndex + 1, -1);
                            }
                        }

                        for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                            serDataType_Array[nTypeIndex].Points[nCount].AxisLabel = dAllFrequencyRange_List[nFrequencyIndex].ToString("F3");

                        for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                            serDataType_IncludeMax_Array[nTypeIndex].Points[nCount].AxisLabel = dAllFrequencyRange_List[nFrequencyIndex].ToString("F3");

                        nCount++;
                    }
                }

                for (int nTypeIndex = 0; nTypeIndex < serDataType_Array.Length; nTypeIndex++)
                    serChartSeries_List.Add(serDataType_Array[nTypeIndex]);

                for (int nTypeIndex = 0; nTypeIndex < serDataType_IncludeMax_Array.Length; nTypeIndex++)
                    serChartSeries_IncludeMax_List.Add(serDataType_IncludeMax_Array[nTypeIndex]);

                serDataChart_Array[nDataIndex] = serChartSeries_List;
                serDataChart_IncludeMax_Array[nDataIndex] = serChartSeries_IncludeMax_List;
            }

            SaveFrequencyChartBy3SubPlot(eDataType_Array, serDataChart_Array, sFilePath);
            SaveFrequencyChartBy3SubPlot(eDataType_Array, serDataChart_IncludeMax_Array, sIncludeMaxFilePath);
        }

        private void SaveFrequencyChartBy3SubPlot(DataType[] eDataType_Array, List<Series>[] serDataChartList_Array, string sFilePath)
        {
            int nDataNumber = 0;

            foreach (List<Series> serDataChart_List in serDataChartList_Array)
            {
                if (serDataChart_List == null)
                    continue;

                foreach (Series serDataChart in serDataChart_List)
                {
                    if (serDataChart.Points.Count > nDataNumber)
                        nDataNumber = serDataChart.Points.Count;
                }
            }

            Chart cChart = new Chart();
            cChart.Width = 2400;
            cChart.Height = 1200;

            float fPreviousChartBottom = 0;
            float fPreviousChartHeight = 31;
            float fPreviousChartWidth = 90;

            for (int nDataIndex = 0; nDataIndex < eDataType_Array.Length; nDataIndex++)
            {
                string sDataType = eDataType_Array[nDataIndex].ToString();

                var vChartArea = new ChartArea(sDataType);
                cChart.ChartAreas.Add(vChartArea);
                cChart.Titles.Add(sDataType);
                cChart.Titles[nDataIndex].Font = new Font("Times New Roman", 20);
                cChart.Titles[nDataIndex].Position.Y = fPreviousChartBottom + 1;
                cChart.Titles[nDataIndex].Position.X = 50;  //42;
                vChartArea.Position.Y = cChart.Titles[nDataIndex].Position.Bottom + 1;
                vChartArea.Position.Height = fPreviousChartHeight;
                vChartArea.Position.Width = fPreviousChartWidth;
                vChartArea.Position.X = 0;
                cChart.ChartAreas[nDataIndex].AxisY.StripLines.Add(new StripLine());
                cChart.ChartAreas[nDataIndex].AxisY.Title = "Reference Value";
                cChart.ChartAreas[nDataIndex].AxisY.TitleFont = new Font("Times New Roman", 14);

                if (nDataIndex == eDataType_Array.Length - 1)
                {
                    cChart.ChartAreas[nDataIndex].AxisX.Title = "Frequency(KHz)";
                    cChart.ChartAreas[nDataIndex].AxisX.TitleFont = new Font("Times New Roman", 14);
                }

                cChart.ChartAreas[nDataIndex].AxisY.LabelStyle.Font = new Font("Times New Roman", 11);

                if (nDataNumber > 21)
                {
                    cChart.ChartAreas[nDataIndex].AxisX.LabelStyle.Font = new Font("Times New Roman", 11);
                    cChart.ChartAreas[nDataIndex].AxisX.LabelStyle.Angle = -90;
                }
                else
                {
                    cChart.ChartAreas[nDataIndex].AxisX.LabelStyle.Font = new Font("Times New Roman", 12);
                    cChart.ChartAreas[nDataIndex].AxisX.LabelStyle.Angle = 0;
                }

                cChart.ChartAreas[nDataIndex].AxisX.Interval = 1;
                cChart.ChartAreas[nDataIndex].AxisX.MajorGrid.LineWidth = 0;

                cChart.ChartAreas[nDataIndex].InnerPlotPosition = new ElementPosition(7, 3, 93, 80);
                cChart.ChartAreas[nDataIndex].AxisY.Minimum = 0;
                //cChart.ChartAreas[nDataIndex].AxisX.MajorGrid.Enabled = false;
                //cChart.ChartAreas[nDataIndex].AxisX.MajorTickMark.Enabled = false;
                //cChart.ChartAreas[nDataIndex].AxisX.Minimum = 0;

                cChart.ChartAreas[nDataIndex].AxisX.MajorGrid.LineWidth = 1;
                cChart.ChartAreas[nDataIndex].AxisX.MajorGrid.LineColor = Color.LightGray;
                cChart.ChartAreas[nDataIndex].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                cChart.ChartAreas[nDataIndex].AxisY.MajorGrid.LineWidth = 1;
                cChart.ChartAreas[nDataIndex].AxisY.MajorGrid.LineColor = Color.LightGray;
                cChart.ChartAreas[nDataIndex].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

                cChart.Legends.Add(new Legend(sDataType));
                cChart.Legends[sDataType].Font = new Font("Times New Roman", 12);
                cChart.Legends[sDataType].InsideChartArea = sDataType;
                cChart.Legends[sDataType].IsDockedInsideChartArea = false;
                cChart.Legends[sDataType].Docking = Docking.Right;

                fPreviousChartBottom = vChartArea.Position.Bottom;
                fPreviousChartHeight = vChartArea.Position.Height;
                fPreviousChartWidth = vChartArea.Position.Width;

                if (serDataChartList_Array[nDataIndex] == null)
                    continue;

                for (int nTypeIndex = 0; nTypeIndex < serDataChartList_Array[nDataIndex].Count; nTypeIndex++)
                {
                    Series serSeries = serDataChartList_Array[nDataIndex][nTypeIndex];
                    serSeries.ChartArea = vChartArea.Name;
                    serSeries.Legend = sDataType;
                    serSeries.IsVisibleInLegend = true;

                    cChart.Series.Add(serSeries);
                }
            }

            cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
        }

        protected void WriteCompositeReferenceDataFile()
        {
            string[] sValueTypeName_Array = new string[] 
            { 
                SpecificText.m_sIndex,
                SpecificText.m_sFrequency, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                "",
                SpecificText.m_sBeacon_Rank,
                SpecificText.m_sBeacon_RX_Inner, 
                SpecificText.m_sBeacon_TX_Inner,
                SpecificText.m_sBeacon_RX_Edge, 
                SpecificText.m_sBeacon_TX_Edge,
                "",
                SpecificText.m_sPTHF_Rank,
                SpecificText.m_sPTHF_RX_Inner,
                SpecificText.m_sPTHF_TX_Inner,
                SpecificText.m_sPTHF_RX_Edge, 
                SpecificText.m_sPTHF_TX_Edge,
                "",
                SpecificText.m_sBHF_Rank,
                SpecificText.m_sBHF_RX_Inner,
                SpecificText.m_sBHF_TX_Inner,
                SpecificText.m_sBHF_RX_Edge, 
                SpecificText.m_sBHF_TX_Edge
            };

            FileStream fs = new FileStream(m_sTNReferenceFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_REFERENCE);

                sw.WriteLine();

                sw.WriteLine("Project Information");
                sw.WriteLine();

                for (int nValueIndex = 0; nValueIndex < sValueTypeName_Array.Length; nValueIndex++)
                {
                    if (nValueIndex < sValueTypeName_Array.Length - 1)
                        sw.Write(string.Format("{0},", sValueTypeName_Array[nValueIndex]));
                    else
                        sw.WriteLine(string.Format("{0},", sValueTypeName_Array[nValueIndex]));
                }

                if (m_bGetBeaconDataFlag == true)
                    m_eCompareOperator = CompareOperator.Beacon;
                else
                    m_eCompareOperator = CompareOperator.BHF;

                m_cTotalDataValue_List.Sort(new DataValueComparer());

                int nDataIndex = 0;

                foreach (DataValue cDataValue in m_cTotalDataValue_List)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0:0.000},", cDataValue.m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataValue.m_nPH1));
                    sw.Write(string.Format("{0},", cDataValue.m_nPH2));
                    sw.Write(",");

                    if (m_bGetBeaconDataFlag == true)
                    {
                        sw.Write(string.Format("{0},", cDataValue.m_cRankData_Beacon.m_nRankIndex));
                        sw.Write(string.Format("{0},", cDataValue.m_cRankData_Beacon.m_dRXInnerValue));
                        sw.Write(string.Format("{0},", cDataValue.m_cRankData_Beacon.m_dTXInnerValue));
                        sw.Write(string.Format("{0},", cDataValue.m_cRankData_Beacon.m_dRXEdgeValue));
                        sw.Write(string.Format("{0},", cDataValue.m_cRankData_Beacon.m_dTXEdgeValue));
                    }
                    else
                    {
                        sw.Write(",");
                        sw.Write(",");
                        sw.Write(",");
                    }

                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataValue.m_cRankData_PTHF.m_nRankIndex));
                    sw.Write(string.Format("{0},", cDataValue.m_cRankData_PTHF.m_dRXInnerValue));
                    sw.Write(string.Format("{0},", cDataValue.m_cRankData_PTHF.m_dTXInnerValue));
                    sw.Write(string.Format("{0},", cDataValue.m_cRankData_PTHF.m_dRXEdgeValue));
                    sw.Write(string.Format("{0},", cDataValue.m_cRankData_PTHF.m_dTXEdgeValue));

                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataValue.m_cRankData_BHF.m_nRankIndex));
                    sw.Write(string.Format("{0},", cDataValue.m_cRankData_BHF.m_dRXInnerValue));
                    sw.Write(string.Format("{0},", cDataValue.m_cRankData_BHF.m_dTXInnerValue));
                    sw.Write(string.Format("{0},", cDataValue.m_cRankData_BHF.m_dRXEdgeValue));
                    sw.WriteLine(string.Format("{0}", cDataValue.m_cRankData_BHF.m_dTXEdgeValue));

                    nDataIndex++;
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        protected void OutputResultData()
        {
            WriteStepListDataFile(m_sProjectName, m_sStepListPath);

            if (m_bErrorFlag == false)
            {
                if (m_eSubStep != SubTuningStep.TILTNO_BHF)
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

        protected override void WriteStepListDataFile(string sProjectName, string sFilePath)
        {
            string[] sValueTypeName_Array = new string[19] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sFlowStep, 
                SpecificText.m_sSettingPH1, 
                SpecificText.m_sSettingPH2, 
                SpecificText.m_sReadPH1, 
                SpecificText.m_sReadPH2, 
                SpecificText.m_sFrequency,
                SpecificText.m_scActivePen_DigiGain_PTHF_Rx, 
                SpecificText.m_scActivePen_DigiGain_PTHF_Tx,
                SpecificText.m_scActivePen_DigiGain_BHF_Rx, 
                SpecificText.m_scActivePen_DigiGain_BHF_Tx,
                SpecificText.m_sRXTotalMax, 
                SpecificText.m_sRXTotalMin, 
                SpecificText.m_sRXTotalMean,
                SpecificText.m_sTXTotalMax, 
                SpecificText.m_sTXTotalMin, 
                SpecificText.m_sTXTotalMean,
                SpecificText.m_sErrorMessage 
            };

            string[] sFWParameterTypeName_Array = null;

            if (m_eSubStep == SubTuningStep.TILTNO_BHF)
            {
                sFWParameterTypeName_Array = new string[13] 
                { 
                    SpecificText.m_sRanking, 
                    SpecificText.m_sFileName, 
                    SpecificText.m_sPH1, 
                    SpecificText.m_sPH2, 
                    SpecificText.m_sFrequency, 
                    SpecificText.m_scActivePen_PTHF_Contact_TH_Rx, 
                    SpecificText.m_scActivePen_PTHF_Contact_TH_Tx,
                    SpecificText.m_scActivePen_PTHF_Hover_TH_Rx, 
                    SpecificText.m_scActivePen_PTHF_Hover_TH_Tx,
                    SpecificText.m_scActivePen_BHF_Contact_TH_Rx, 
                    SpecificText.m_scActivePen_BHF_Contact_TH_Tx,
                    SpecificText.m_scActivePen_BHF_Hover_TH_Rx, 
                    SpecificText.m_scActivePen_BHF_Hover_TH_Tx
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

                foreach (DataInfo cDataInfo in m_cDataInfo_List)
                {
                    DataValue cDataValue = m_cDataValue_List.Find(x => (x.m_nPH1 == cDataInfo.m_nSettingPH1 && x.m_nPH2 == cDataInfo.m_nSettingPH2));

                    sw.Write(string.Format("{0},", cDataValue.m_nRankIndex));
                    sw.Write(string.Format("{0},", cDataInfo.m_sFileName));
                    sw.Write(string.Format("{0},", cDataInfo.m_eSubStep.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSettingPH1.ToString().PadLeft(2, '0')));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSettingPH2.ToString().PadLeft(2, '0')));
                    sw.Write(string.Format("{0},", cDataInfo.m_nReadPH1.ToString().PadLeft(2, '0')));
                    sw.Write(string.Format("{0},", cDataInfo.m_nReadPH2.ToString().PadLeft(2, '0')));
                    sw.Write(string.Format("{0:0.000},", cDataInfo.m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDigiGain_PTHF_Rx));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDigiGain_PTHF_Tx));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDigiGain_BHF_Rx));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDigiGain_BHF_Tx));
                    sw.Write(string.Format("{0},", cDataValue.m_cRXReferenceValue.m_nTotalMax));
                    sw.Write(string.Format("{0},", cDataValue.m_cRXReferenceValue.m_nTotalMin));
                    sw.Write(string.Format("{0},", cDataValue.m_cRXReferenceValue.m_dTotalMean.ToString()));
                    sw.Write(string.Format("{0},", cDataValue.m_cTXReferenceValue.m_nTotalMax));
                    sw.Write(string.Format("{0},", cDataValue.m_cTXReferenceValue.m_nTotalMin));
                    sw.Write(string.Format("{0},", cDataValue.m_cTXReferenceValue.m_dTotalMean.ToString()));

                    if (cDataInfo.m_sWarningMessage != "")
                        sw.WriteLine(string.Format("{0}", cDataInfo.m_sWarningMessage));
                    else
                    {
                        string sErrorMessage = string.Format("{0}", cDataInfo.m_sErrorMessage);

                        if (ParamAutoTuning.m_nFlowMethodType == 1 && cDataInfo.m_sRecordErrorCode != "")
                            sErrorMessage = string.Format("{0}", cDataInfo.m_sRecordErrorMessage);

                        sw.WriteLine(sErrorMessage);
                    }
                }

                if (m_eSubStep == SubTuningStep.TILTNO_BHF)
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

                    foreach (DataInfo cDataInfo in m_cDataInfo_List)
                    {
                        DataValue cDataValue = m_cDataValue_List.Find(x => (x.m_nPH1 == cDataInfo.m_nSettingPH1 && x.m_nPH2 == cDataInfo.m_nSettingPH2));

                        sw.Write(string.Format("{0},", cDataValue.m_nRankIndex));
                        sw.Write(string.Format("{0},", cDataInfo.m_sFileName));
                        sw.Write(string.Format("{0},", cDataInfo.m_nReadPH1.ToString().PadLeft(2, '0')));
                        sw.Write(string.Format("{0},", cDataInfo.m_nReadPH2.ToString().PadLeft(2, '0')));
                        sw.Write(string.Format("{0:0.000},", cDataInfo.m_dFrequency.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataValue.m_nPTHF_Contact_TH_Rx));
                        sw.Write(string.Format("{0},", cDataValue.m_nPTHF_Contact_TH_Tx));
                        sw.Write(string.Format("{0},", cDataValue.m_nPTHF_Hover_TH_Rx));
                        sw.Write(string.Format("{0},", cDataValue.m_nPTHF_Hover_TH_Tx));
                        sw.Write(string.Format("{0},", cDataValue.m_cRXReferenceValue.m_nTotalMax));
                        sw.Write(string.Format("{0},", cDataValue.m_cTXReferenceValue.m_nTotalMax));
                        sw.Write(string.Format("{0},", cDataValue.m_cRXReferenceValue.m_nTotalMax));
                        sw.WriteLine(string.Format("{0}", cDataValue.m_cTXReferenceValue.m_nTotalMax));
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
            int nDataCount = m_cDataInfo_List.Count;

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nRankIndex = 0; nRankIndex < nDataCount; nRankIndex++)
                {
                    if (((ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nRankIndex].m_sRecordErrorCode == "" && m_cDataInfo_List[nRankIndex].m_sErrorMessage == "") || ParamAutoTuning.m_nFlowMethodType != 1) &&
                        m_cDataInfo_List[nRankIndex].m_dFrequency > ParamAutoTuning.m_nFrequencyLB_MPP180)
                    {
                        sw.Write(string.Format("{0},", m_cDataInfo_List[nRankIndex].m_nRankIndex));
                        sw.Write(string.Format("{0},", FlowRobot.NO.ToString()));
                        sw.Write(string.Format("{0},", FlowRecord.NTRX.ToString()));
                        sw.Write(string.Format("{0},", m_cDataInfo_List[nRankIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                        sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nRankIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));
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
