using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using MathNet.Numerics;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using FingerAutoTuningParameter;
using Elan;

namespace FingerAutoTuning
{
    public class AnalysisFlow_FRPH2 : AnalysisFlow
    {
        private int m_nTXTraceNumber = 0;
        private int m_nRXTraceNumber = 0;
        private int m_nMinPH1 = 0x10;
        private int m_nPH2LB = 0x00;
        private int m_nBestRankNumber = 10;
        //private static double m_dBestPercentage = 90.0;
        private bool m_bEnableTXn = false;

        private int m_nComputeRXTraceReference = 0;
        private int m_nPenEffectRXTrace = 0;

        private int m_nADCTestFrame = 300;
        private int m_nDVTestFrame = 300;

        private int m_nChartReferenceValueHB = 0;
        private int m_nChartReferenceValueLB = 0;
        private int m_nChartReferenceValueInterval = 0;

        private bool m_bRecordRecentADCMeanForBase = false;
        private int m_nRecentADCMeanForBaseFrameNumber = 10;

        public class DataInfo
        {
            public string m_sBASEFileName = "";
            public string m_sADCFileName = "";
            public string m_sDVFileName = "";
            public bool m_bMatch = false;
            public int m_nSetIndex = -1;
            public int m_nPH1Value = 0;
            public int m_nPH2Value = 0;
            public int m_nPH3Value = 0;
            public int m_nDFT_NUMValue = 0;
            public int m_nRXTraceNumber = 0;
            public int m_nTXTraceNumber = 0;
            public int m_nFWIP_Option = 0;
            public double m_dFrequency = 0.0;
            public int m_nSuggestDFT_NUM = 0;

            public int[,] m_nBASEData_Array = null;
            //public List<int[,]> m_nBASEMinusADCData_List = null;

            public double m_dSingleMinUniformity = 0.0;
            public double m_dUniformityMean = 0.0;
            public double m_dNormalizeUniformity = 0.0;

            public BASEMinusADCOffsetData m_cBASEMinusADCOffsetData = new BASEMinusADCOffsetData();

            public RecentADCMeanForBaseData m_cRecentADCMeanForBaseData = new RecentADCMeanForBaseData();

            public StatisticData m_cStatisticData = new StatisticData();

            public RXTraceReference m_cRXTraceReference = new RXTraceReference();
        }

        public class DataInfoComparer : IComparer<DataInfo>
        {
            public int Compare(DataInfo cDataInfo1, DataInfo cDataInfo2)
            {
                if (m_nCompareOperator == m_nCOMPARE_Frequency)
                    return cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency);
                else if (m_nCompareOperator == m_nCOMPARE_Normalize)
                {
                    if (-cDataInfo1.m_cStatisticData.m_dNormalizeSNRReference.CompareTo(cDataInfo2.m_cStatisticData.m_dNormalizeSNRReference) != 0)
                        return -cDataInfo1.m_cStatisticData.m_dNormalizeSNRReference.CompareTo(cDataInfo2.m_cStatisticData.m_dNormalizeSNRReference);
                    else if (-cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency) != 0)
                        return -cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency);
                    else
                        return 1;
                }
                else
                    return cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency);
            }
        }

        private List<DataInfo> m_cDataInfo_List = new List<DataInfo>();

        public class StatisticData
        {
            public double m_dNoiseMean = 0.0;
            public double m_dNoiseStd = 0.0;

            public double m_dNoisePositiveReference = 0.0;
            public double m_dNoiseNegativeReference = 0.0;

            public double m_dNoiseSqrtDistanceReference = 0.0;

            public double m_dNormalizeUniformityReference = 0.0;
            public double m_dNormalizeNoiseSqrtDistanceReference = 0.0;

            public double m_dRawSNR = 0.0;
            public double m_dNormalizeSNRReference = 0.0;

            //public double m_dRangeMeanNoiseSNR = 0.0;
        }

        public class BestRankSortData
        {
            public string m_sBASEFileName = "";
            public string m_sADCFileName = "";
            public int m_nPH1Value = 0;
            public int m_nPH2Value = 0;
            public int m_nSuggestDFT_NUM = 0;
            public double m_dFrequency = 0.0;

            public double m_dNormalizeSNRReference = 0.0;

            public List<PH1PH2Pair> m_cPH1PH2Pair_List = new List<PH1PH2Pair>();
        }

        public class BestSortDataComparer : IComparer<BestRankSortData>
        {
            public int Compare(BestRankSortData cBestRankSortData1, BestRankSortData cBestRankSortData2)
            {
                if (m_nCompareOperator == m_nCOMPARE_Frequency)
                    return -cBestRankSortData1.m_dFrequency.CompareTo(cBestRankSortData2.m_dFrequency);
                else if (m_nCompareOperator == m_nCOMPARE_Normalize)
                    return -cBestRankSortData1.m_dNormalizeSNRReference.CompareTo(cBestRankSortData2.m_dNormalizeSNRReference);
                else
                    return -cBestRankSortData1.m_dFrequency.CompareTo(cBestRankSortData2.m_dFrequency);
            }
        }

        private List<BestRankSortData> m_cBestRankSortData_List = new List<BestRankSortData>();

        public class PH1PH2Pair
        {
            public int m_nPH1Value = 0;
            public int m_nPH2Value = 0;
        }

        public class RXTraceReference
        {
            public List<double> m_listdMean = new List<double>();
            public List<double> m_listdStd = new List<double>();
            public List<double> m_listdReference = new List<double>();

            public double m_dNoEffectMean = 0.0;
            public double m_dNoEffectStd = 0.0;
            public double m_dNoEffectReference = 0.0;
        }

        public class BASEMinusADCOffsetData
        {
            public double m_dADCMaxMinDiffer = 0.0;
            public double m_dADCMean = 0.0;
            public double m_dBASEMinusADCOffsetMean = 0.0;
            public double m_dBASEMinusADCOffsetStd = 0.0;
        }

        public class RecentADCMeanForBaseData
        {
            public double m_dMean = 0.0;
            public double m_dStd = 0.0;
            public double m_dNoiseSqrtDistanceReference = 0.0;
            public double m_dNormalizeNoiseSqrtDistanceReference = 0.0;

            public double m_dRawSNR = 0.0;

            public double m_dNormalizeSNRReference = 0.0;
        }

        public class UniformityValueErrorData
        {
            public double m_dFrequency = 0.0;
            public int m_nPH1PH2Sum = 0;
            public string m_sMessage = "";
        }

        private List<UniformityValueErrorData> m_cUniformityValueErrorData_List = new List<UniformityValueErrorData>();

        private bool m_bWarningOccurred = false;

        public AnalysisFlow_FRPH2(frmMain.FlowStep cFlowStep, string sLogDirectoryPath, string sH5LogDirectoryPath, bool bGenerateH5Data, frmMain cfrmParent, string sProjectName)
        {
            m_cFlowStep = cFlowStep;
            m_cfrmParent = cfrmParent;
            m_sLogDirectoryPath = sLogDirectoryPath;
            m_sH5LogDirectoryPath = sH5LogDirectoryPath;
            m_bGenerateH5Data = bGenerateH5Data;
            m_sProjectName = sProjectName;

            InitializeParameter();
            InitializeSourceDataList();
        }

        public override void InitializeParameter()
        {
            m_nMinPH1 = ParamFingerAutoTuning.m_nFRPH2MinPH1;
            m_nBestRankNumber = ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber;
            m_nComputeRXTraceReference = ParamFingerAutoTuning.m_nFRPH2ComputeRXTraceReference;
            m_nPenEffectRXTrace = ParamFingerAutoTuning.m_nFRPH2PenEffectRXTrace;

            m_nADCTestFrame = ParamFingerAutoTuning.m_nFRPH2ADCTestFrame;
            m_nDVTestFrame = ParamFingerAutoTuning.m_nFRPH2DVTestFrame;

            m_nChartReferenceValueHB = ParamFingerAutoTuning.m_nFRPH2ChartReferenceValueHB;
            m_nChartReferenceValueLB = ParamFingerAutoTuning.m_nFRPH2ChartReferenceValueLB;
            m_nChartReferenceValueInterval = ParamFingerAutoTuning.m_nFRPH2ChartReferenceValueInterval;

            if (ParamFingerAutoTuning.m_nFRPH2DataType != 2 && ParamFingerAutoTuning.m_nFRPH2RecordRecentADCMeanForBase == 1)
                m_bRecordRecentADCMeanForBase = true;
            else
                m_bRecordRecentADCMeanForBase = false;

            m_nRecentADCMeanForBaseFrameNumber = ParamFingerAutoTuning.m_nFRPH2RecentADCMeanForBaseFrameNumber;
        }

        public override void InitializeSourceDataList()
        {
            m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_BASE);
            m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_ADC);

            if (ParamFingerAutoTuning.m_nFRPH2DataType == 1)
                m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_DV);		
        }

        public override bool MainFlow(ref string sErrorMessage)
        {
            if (GetDataCount() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (ReadBASEData() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (ReadADCData() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (ParamFingerAutoTuning.m_nFRPH2DataType == 1)
            {
                if (ReadDVData() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }
            }

            if (AnalysisData() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (SaveAnalysisFile() == false)
            {
                CopyDataToH5Directory();
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (SaveLineChartFile() == false)
            {
                CopyDataToH5Directory();
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (SaveReportFile() == false)
            {
                CopyDataToH5Directory();
                SetErrorMessage(ref sErrorMessage);
                return false;
            }
            if (m_nComputeRXTraceReference == 1)
            {
                if (ProcessRXTraceData() == false)
                {
                    CopyDataToH5Directory();
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }
            }

            CopyDataToH5Directory();

            UpdateProgressBar();

            m_cDataInfo_List.Clear();
            m_cDataInfo_List = null;
            GC.Collect();

            return true;
        }

        private bool ReadBASEData()
        {
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_BASE);

            m_nAnalysisCount = m_nTotalFileCount + 1;

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.InitialstatusstripMessage(m_nAnalysisCount, "Data Analysis...");
            });

            foreach (string sFilePath in Directory.EnumerateFiles(sDirectoryPath, "*.csv", SearchOption.TopDirectoryOnly))
            {
                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);
                FileCheckInfo cFileCheckInfo = new FileCheckInfo();
                List<int[,]> nFrameData_List = new List<int[,]>();

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                if (CheckFileInfo(ref cFileCheckInfo, srFile, sFileName) == false)
                    return false;

                if (CheckFileInfoIdentical(cFileCheckInfo, sFileName, MainConstantParameter.m_sDATATYPE_ADC) == false)
                    return false;

                double dFrequncy = ElanConvert.Convert2Frequency(cFileCheckInfo.m_nReadPH1, cFileCheckInfo.m_nReadPH2);

                DataInfo cDataInfo = new DataInfo();
                cDataInfo.m_sBASEFileName = sFileName;
                cDataInfo.m_nSetIndex = cFileCheckInfo.m_nSetIndex;
                cDataInfo.m_dFrequency = dFrequncy;
                cDataInfo.m_nPH1Value = cFileCheckInfo.m_nReadPH1;
                cDataInfo.m_nPH2Value = cFileCheckInfo.m_nReadPH2;
                cDataInfo.m_nPH3Value = cFileCheckInfo.m_nReadPH3;
                cDataInfo.m_nRXTraceNumber = cFileCheckInfo.m_nRXTraceNumber;
                cDataInfo.m_nTXTraceNumber = cFileCheckInfo.m_nTXTraceNumber;
                cDataInfo.m_nFWIP_Option = cFileCheckInfo.m_nFWIP_Option;
                m_cDataInfo_List.Add(cDataInfo);
                int nDataIndex = m_cDataInfo_List.Count - 1;

                srFile = new StreamReader(sFilePath, Encoding.Default);

                if (GetFrameData(ref nFrameData_List, cFileCheckInfo, srFile, sFileName, ReadDataType.ADC) == false)
                    return false;

                if (nFrameData_List.Count > 0)
                    m_cDataInfo_List[nDataIndex].m_nBASEData_Array = nFrameData_List[0];

                string sMessage = string.Format("Analysis : {0}/{1}", m_nProgressIndex + 1, m_nAnalysisCount);

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, m_nProgressIndex + 1, sMessage);
                });

                if (m_cfrmParent.m_bExecute == false)
                    return false;

                m_nProgressIndex++;
            }

            return true;
        }

        private bool CheckFileInfo(ref FileCheckInfo cFileCheckInfo, StreamReader srFile, string sFileName)
        {
            string sLine = "";

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSubString_Array = sLine.Split(',');

                    if (sSubString_Array.Length >= 2)
                    {
                        if (sSubString_Array[0] == "TXTraceNumber")
                            Int32.TryParse(sSubString_Array[1], out cFileCheckInfo.m_nTXTraceNumber);
                        else if (sSubString_Array[0] == "RXTraceNumber")
                            Int32.TryParse(sSubString_Array[1], out cFileCheckInfo.m_nRXTraceNumber);
                        else if (sSubString_Array[0] == "SetIndex")
                            cFileCheckInfo.m_nSetIndex = Convert.ToInt32(sSubString_Array[1]);
                        else if (sSubString_Array[0] == "SetPH1(Hex)")
                            cFileCheckInfo.m_nSetPH1 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "SetPH2(Hex)")
                            cFileCheckInfo.m_nSetPH2 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "SetPH3(Hex)")
                            cFileCheckInfo.m_nSetPH3 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "SetSum(Hex)" || sSubString_Array[0] == "SetDFT_NUM(Hex)")
                            cFileCheckInfo.m_nSetDFT_NUM = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "ReadPH1(Hex)")
                            cFileCheckInfo.m_nReadPH1 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "ReadPH2(Hex)")
                            cFileCheckInfo.m_nReadPH2 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "ReadPH3(Hex)")
                            cFileCheckInfo.m_nReadPH3 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "ReadSum(Hex)" || sSubString_Array[0] == "ReadDFT_NUM(Hex)")
                            cFileCheckInfo.m_nReadDFT_NUM = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "ReadFWIP_Option(Hex)")
                            cFileCheckInfo.m_nFWIP_Option = Convert.ToInt32(sSubString_Array[1], 16);
                    }

                    if (cFileCheckInfo.m_nSetPH1 > -1 && cFileCheckInfo.m_nSetPH2 > -1 &&
                        cFileCheckInfo.m_nReadPH1 > -1 && cFileCheckInfo.m_nReadPH2 > -1 &&
                        cFileCheckInfo.m_nSetDFT_NUM > -1 && cFileCheckInfo.m_nReadDFT_NUM > -1 &&
                        cFileCheckInfo.m_nTXTraceNumber > -1 && cFileCheckInfo.m_nRXTraceNumber > -1 &&
                        cFileCheckInfo.m_nFWIP_Option > -1)
                        break;
                }
            }
            finally
            {
                srFile.Close();
            }

            if (cFileCheckInfo.m_nSetPH1 == -1 || cFileCheckInfo.m_nReadPH1 == -1 ||
                cFileCheckInfo.m_nSetPH1 != cFileCheckInfo.m_nReadPH1)
            {
                m_sErrorMessage = string.Format("Read PH1 Error in {0}[Set:0x{1} Read:0x{2}]", sFileName,
                                                cFileCheckInfo.m_nSetPH1.ToString("x2").ToUpper(),
                                                cFileCheckInfo.m_nReadPH1.ToString("x2").ToUpper());
                return false;
            }

            if (cFileCheckInfo.m_nSetPH2 == -1 || cFileCheckInfo.m_nReadPH2 == -1 ||
                cFileCheckInfo.m_nSetPH2 != cFileCheckInfo.m_nReadPH2)
            {
                m_sErrorMessage = string.Format("Read PH2 Error in {0}[Set:0x{1} Read:0x{2}]", sFileName,
                                                cFileCheckInfo.m_nSetPH2.ToString("x2").ToUpper(),
                                                cFileCheckInfo.m_nReadPH2.ToString("x2").ToUpper());
                return false;
            }

            if (cFileCheckInfo.m_nTXTraceNumber == -1)
            {
                m_sErrorMessage = string.Format("Read TXTraceNumber Error in {0}[Number={1}]", sFileName, cFileCheckInfo.m_nTXTraceNumber);
                return false;
            }

            if (cFileCheckInfo.m_nRXTraceNumber == -1)
            {
                m_sErrorMessage = string.Format("Read RXTraceNumber Error in {0}[Number={1}]", sFileName, cFileCheckInfo.m_nRXTraceNumber);
                return false;
            }

            if (cFileCheckInfo.m_nFWIP_Option == -1)
            {
                m_sErrorMessage = string.Format("Read FWIP_Option Error in {0}[Value=0x{1}]", sFileName, cFileCheckInfo.m_nFWIP_Option.ToString("x4").ToUpper());
                return false;
            }

            if (m_nTXTraceNumber <= 0)
                m_nTXTraceNumber = cFileCheckInfo.m_nTXTraceNumber;

            if (m_nRXTraceNumber <= 0)
                m_nRXTraceNumber = cFileCheckInfo.m_nRXTraceNumber;

            return true;
        }

        private bool CheckFileInfoIdentical(FileCheckInfo cFileCheckInfo, string sFileName, string sDataType)
        {
            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                string sMatchFileName = "";

                switch (sDataType)
                {
                    case MainConstantParameter.m_sDATATYPE_BASE:
                        sMatchFileName = m_cDataInfo_List[nDataIndex].m_sBASEFileName;
                        break;
                    case MainConstantParameter.m_sDATATYPE_ADC:
                        sMatchFileName = m_cDataInfo_List[nDataIndex].m_sADCFileName;
                        break;
                    case MainConstantParameter.m_sDATATYPE_DV:
                        sMatchFileName = m_cDataInfo_List[nDataIndex].m_sDVFileName;
                        break;
                    default:
                        break;
                }

                if (m_cDataInfo_List[nDataIndex].m_nPH1Value == cFileCheckInfo.m_nReadPH1)
                {
                    m_sErrorMessage = string.Format("PH1 Unique Check Error in {0} and {1}[Value1=0x{2}]", sFileName, 
                                                    sMatchFileName,
                                                    cFileCheckInfo.m_nReadPH1.ToString("x2").ToUpper());
                    return false;
                }

                if (m_cDataInfo_List[nDataIndex].m_nPH2Value != cFileCheckInfo.m_nReadPH2)
                {
                    m_sErrorMessage = string.Format("PH2 Unique Check Error in {0} and {1}[Value=0x{2} Value2=0x{3}]", sFileName, 
                                                    sMatchFileName,
                                                    cFileCheckInfo.m_nReadPH2.ToString("x2").ToUpper(),
                                                    m_cDataInfo_List[nDataIndex].m_nPH2Value.ToString("x2").ToUpper());
                    return false;
                }

                if (m_cDataInfo_List[nDataIndex].m_nTXTraceNumber != cFileCheckInfo.m_nTXTraceNumber)
                {
                    m_sErrorMessage = string.Format("TXTraceNumber Identical Check Error in {0} and {1}[Number1={2} Number2={3}]", sFileName, 
                                                    sMatchFileName,
                                                    cFileCheckInfo.m_nTXTraceNumber,
                                                    m_cDataInfo_List[nDataIndex].m_nTXTraceNumber);
                    return false;
                }

                if (m_cDataInfo_List[nDataIndex].m_nRXTraceNumber != cFileCheckInfo.m_nRXTraceNumber)
                {
                    m_sErrorMessage = string.Format("RXTraceNumber Identical Check Error in {0} and {1}[Number1={2} Number2={3}]", sFileName, 
                                                    sMatchFileName,
                                                    cFileCheckInfo.m_nRXTraceNumber,
                                                    m_cDataInfo_List[nDataIndex].m_nRXTraceNumber);
                    return false;
                }

                if (m_cDataInfo_List[nDataIndex].m_nFWIP_Option != cFileCheckInfo.m_nFWIP_Option)
                {
                    m_sErrorMessage = string.Format("FWIP_Option Identical Check Error in {0} and {1}[Value1=0x{2} Value2=0x{3}]", sFileName, 
                                                    sMatchFileName,
                                                    cFileCheckInfo.m_nFWIP_Option.ToString("x4").ToUpper(),
                                                    m_cDataInfo_List[nDataIndex].m_nFWIP_Option.ToString("x4").ToUpper());
                    return false;
                }
            }

            return true;
        }

        private bool CheckFileInfoMatch(FileCheckInfo cFileCheckInfo, string sFileName, string sDataType)
        {
            bool bMatch = false;

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                if (m_cDataInfo_List[nDataIndex].m_nPH1Value == cFileCheckInfo.m_nReadPH1 &&
                    m_cDataInfo_List[nDataIndex].m_nPH2Value == cFileCheckInfo.m_nReadPH2)
                {
                    string sMatchFileName = "";

                    switch (sDataType)
                    {
                        case MainConstantParameter.m_sDATATYPE_BASE:
                            sMatchFileName = m_cDataInfo_List[nDataIndex].m_sBASEFileName;
                            break;
                        case MainConstantParameter.m_sDATATYPE_ADC:
                            sMatchFileName = m_cDataInfo_List[nDataIndex].m_sADCFileName;
                            break;
                        case MainConstantParameter.m_sDATATYPE_DV:
                            sMatchFileName = m_cDataInfo_List[nDataIndex].m_sDVFileName;
                            break;
                        default:
                            break;
                    }

                    if (m_cDataInfo_List[nDataIndex].m_bMatch == true || sMatchFileName != sFileName)
                    {
                        m_sErrorMessage = string.Format("Files have been Overlapped in {0} and {1}", sFileName, sMatchFileName);
                        return false;
                    }
                    else
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nFWIP_Option != cFileCheckInfo.m_nFWIP_Option)
                        {
                            m_sErrorMessage = string.Format("FWIP_Option Identical Check Error in {0} and {1}[Value1=0x{2} Value2=0x{3}]", sFileName, 
                                                            sMatchFileName,
                                                            cFileCheckInfo.m_nFWIP_Option.ToString("x4").ToUpper(),
                                                            m_cDataInfo_List[nDataIndex].m_nFWIP_Option.ToString("x4").ToUpper());
                            return false;
                        }

                        bMatch = true;
                        break;
                    }
                }
            }

            if (bMatch == false)
            {
                m_sErrorMessage = string.Format("No Match File with {0}", sFileName);
                return false;
            }

            return true;
        }

        private bool GetFrameData(ref List<int[,]> nFrameData_List, FileCheckInfo cFileCheckInfo, StreamReader srFile, string sFileName, ReadDataType eReadDataType)
        {
            bool bGetFrameDataFlag = false;
            int[,] nSingleFrame_Array = null;
            int nTXCount = 0;
            string sLine = "";
            int nFrameCount = 0;

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSplit_Array = sLine.Split(',');

                    if (sSplit_Array.Length >= 2)
                    {
                        if (sSplit_Array[0] == "Frame")
                        {
                            bGetFrameDataFlag = true;
                            nTXCount = 0;
                            nSingleFrame_Array = new int[cFileCheckInfo.m_nTXTraceNumber, cFileCheckInfo.m_nRXTraceNumber];
                            continue;
                        }
                    }

                    if (bGetFrameDataFlag == true)
                    {
                        if (sSplit_Array.Length >= cFileCheckInfo.m_nRXTraceNumber)
                        {
                            for (int nRXIndex = 0; nRXIndex < cFileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                                nSingleFrame_Array[nTXCount, nRXIndex] = Convert.ToInt32(sSplit_Array[nRXIndex]);

                            nTXCount++;
                        }

                        if (nTXCount == cFileCheckInfo.m_nTXTraceNumber)
                        {
                            nFrameData_List.Add(nSingleFrame_Array);
                            bGetFrameDataFlag = false;
                            nFrameCount++;
                        }
                    }

                    if (eReadDataType == ReadDataType.ADC)
                    {
                        if (nFrameCount >= m_nADCTestFrame)
                            break;
                    }
                    else if (eReadDataType == ReadDataType.dV)
                    {
                        if (nFrameCount >= m_nDVTestFrame)
                            break;
                    }
                    
                }
            }
            finally
            {
                srFile.Close();
            }

            if (nFrameData_List == null || nFrameData_List.Count == 0)
            {
                m_sErrorMessage = string.Format("Read Data Frame Error in {0}[Count:{1}]", sFileName, nFrameData_List.Count);
                return false;
            }

            return true;
        }

        private bool ReadADCData()
        {
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_ADC);

            foreach (string sFilePath in Directory.EnumerateFiles(sDirectoryPath, "*.csv", SearchOption.TopDirectoryOnly))
            {
                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);
                FileCheckInfo cFileCheckInfo = new FileCheckInfo();
                List<int[,]> nFrameData_List = new List<int[,]>();

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                if (CheckFileInfo(ref cFileCheckInfo, srFile, sFileName) == false)
                    return false;

                int nDataIndex = m_cDataInfo_List.FindIndex(x => x.m_nPH1Value == cFileCheckInfo.m_nReadPH1 && x.m_nPH2Value == cFileCheckInfo.m_nReadPH2);

                m_cDataInfo_List[nDataIndex].m_sADCFileName = sFileName;

                if (CheckFileInfoMatch(cFileCheckInfo, sFileName, MainConstantParameter.m_sDATATYPE_ADC) == false)
                    return false;

                srFile = new StreamReader(sFilePath, Encoding.Default);

                if (GetFrameData(ref nFrameData_List, cFileCheckInfo, srFile, sFileName, ReadDataType.ADC) == false)
                    return false;

                ComputeDFT_NUMAndSuggestDFT_NUM(cFileCheckInfo, nDataIndex, cFileCheckInfo.m_nTXTraceNumber);

                if (ComputeUniformityData(nDataIndex, nFrameData_List, cFileCheckInfo.m_nRXTraceNumber, cFileCheckInfo.m_nTXTraceNumber) == false)
                    return false;

                //ComputeNormalizeUniformity();

                if (ParamFingerAutoTuning.m_nFRPH2DataType != 1)
                {
                    if (ComputeNoiseReferenceValue(nDataIndex, nFrameData_List, cFileCheckInfo.m_nRXTraceNumber, cFileCheckInfo.m_nTXTraceNumber) == false)
                        return false;
                }

                string sMessage = string.Format("Analysis : {0}/{1}", m_nProgressIndex + 1, m_nAnalysisCount);

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, m_nProgressIndex + 1, sMessage);
                });

                if (m_cfrmParent.m_bExecute == false)
                    return false;

                m_nProgressIndex++;
            }

            return true;
        }

        private bool ReadDVData()
        {
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_DV);

            foreach (string sFilePath in Directory.EnumerateFiles(sDirectoryPath, "*.csv", SearchOption.TopDirectoryOnly))
            {
                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);
                FileCheckInfo cFileCheckInfo = new FileCheckInfo();
                List<int[,]> nFrameData_Array = new List<int[,]>();

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                if (CheckFileInfo(ref cFileCheckInfo, srFile, sFileName) == false)
                    return false;

                int nDataIndex = m_cDataInfo_List.FindIndex(x => x.m_nPH1Value == cFileCheckInfo.m_nReadPH1 && x.m_nPH2Value == cFileCheckInfo.m_nReadPH2);

                m_cDataInfo_List[nDataIndex].m_sDVFileName = sFileName;

                if (CheckFileInfoMatch(cFileCheckInfo, sFileName, MainConstantParameter.m_sDATATYPE_DV) == false)
                    return false;

                srFile = new StreamReader(sFilePath, Encoding.Default);

                if (GetFrameData(ref nFrameData_Array, cFileCheckInfo, srFile, sFileName, ReadDataType.dV) == false)
                    return false;

                ComputeNoiseReferenceValue(nDataIndex, nFrameData_Array, cFileCheckInfo.m_nRXTraceNumber, cFileCheckInfo.m_nTXTraceNumber, false);

                string sMessage = string.Format("Analysis : {0}/{1}", m_nProgressIndex + 1, m_nAnalysisCount);

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, m_nProgressIndex + 1, sMessage);
                });

                if (m_cfrmParent.m_bExecute == false)
                    return false;

                m_nProgressIndex++;
            }

            return true;
        }

        private void ComputeDFT_NUMAndSuggestDFT_NUM(FileCheckInfo cFileCheckInfo, int nDataIndex, int nTXTraceNumber)
        {
            m_cDataInfo_List[nDataIndex].m_nDFT_NUMValue = cFileCheckInfo.m_nReadDFT_NUM;

            DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];
            m_cDataInfo_List[nDataIndex].m_nSuggestDFT_NUM = ElanConvert.Convert2SuggestDFT_NUM(cDataInfo.m_nPH1Value, cDataInfo.m_nPH2Value, nTXTraceNumber, ParamFingerAutoTuning.m_nIdealScanTime);
        }

        private bool ComputeUniformityData(int nDataIndex, List<int[,]> nFrameData_List, int nRXTraceNumber, int nTXTraceNumber)
        {
            double dMinUniformityValue = 0.0;
            double dRealityMinUniformityValue = 0.0;

            int nMaxMinDiffer = 0;
            double dMeanSum = 0.0;
            
            int nFrameMinValue = 0;
            int nFrameMaxValue = 0;

            for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
            {
                List<int> nData_List = new List<int>();
                double dRealityUniformityValue = 0.0;
                int nMaxValue = 0;
                int nMinValue = 0;

                for (int nTXIndex = 1; nTXIndex < nTXTraceNumber - 1; nTXIndex++)
                {
                    for (int nRXIndex = 1; nRXIndex < nRXTraceNumber - 1; nRXIndex++)
                    {
                        if (nTXIndex == 1 && nRXIndex == 1)
                        {
                            nMaxValue = nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                            nMinValue = nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                        }
                        else
                        {
                            if (nFrameData_List[nFrameIndex][nTXIndex, nRXIndex] > nMaxValue)
                                nMaxValue = nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];

                            if (nFrameData_List[nFrameIndex][nTXIndex, nRXIndex] < nMinValue)
                                nMinValue = nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                        }

                        nData_List.Add(nFrameData_List[nFrameIndex][nTXIndex, nRXIndex]);
                    }
                }

                double dMeam = Math.Round(nData_List.Average(), 3);
                dMeanSum += dMeam;

                nMaxMinDiffer += nMaxValue - nMinValue;

                double dUniformityValue = Math.Round(((double)nMinValue / nMaxValue) * 100, 3, MidpointRounding.AwayFromZero);

                if (Double.IsNaN(dUniformityValue) == true)
                    continue;

                dRealityUniformityValue = dUniformityValue;

                if (dUniformityValue < 0.0)
                    dUniformityValue = 0.0;
                else if (nMinValue <= 0 && nMaxValue <= 0)
                    dUniformityValue = 0.0;

                if (nFrameIndex == 0)
                {
                    dRealityMinUniformityValue = dRealityUniformityValue;
                    dMinUniformityValue = dUniformityValue;
                    nFrameMinValue = nMinValue;
                    nFrameMaxValue = nMaxValue;
                }
                else
                {
                    if (dRealityUniformityValue < dRealityMinUniformityValue)
                    {
                        dRealityMinUniformityValue = dRealityUniformityValue;
                        dMinUniformityValue = dUniformityValue;
                        nFrameMinValue = nMinValue;
                        nFrameMaxValue = nMaxValue;
                    }
                }
            }

            double dTotalMaxMinDiffer = Math.Round((double)nMaxMinDiffer / nFrameData_List.Count, 3);
            double dTotalMean = Math.Round(dMeanSum / nFrameData_List.Count, 3);

            m_cDataInfo_List[nDataIndex].m_cBASEMinusADCOffsetData.m_dADCMaxMinDiffer = dTotalMaxMinDiffer;
            m_cDataInfo_List[nDataIndex].m_cBASEMinusADCOffsetData.m_dADCMean = dTotalMean;

            m_cDataInfo_List[nDataIndex].m_dSingleMinUniformity = dMinUniformityValue;

            string sWarningMessage = "";
            string sMessage = "";

            if (Double.IsNaN(dRealityMinUniformityValue) == true)
            {
                sWarningMessage = string.Format("Get Uniformity Value({0}) Error in Frequency={1:0.000}KHz", dRealityMinUniformityValue, m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000"));
                OutputMessage(string.Format("Warning : {0}", sWarningMessage), true);

                sMessage = string.Format("Get Uniformity Value({0}) Error", dRealityMinUniformityValue);

                UniformityValueErrorData cUniformityValueErrorData = new UniformityValueErrorData();
                cUniformityValueErrorData.m_dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;
                cUniformityValueErrorData.m_nPH1PH2Sum = m_cDataInfo_List[nDataIndex].m_nPH1Value + m_cDataInfo_List[nDataIndex].m_nPH2Value;
                cUniformityValueErrorData.m_sMessage = sMessage;
                m_cUniformityValueErrorData_List.Add(cUniformityValueErrorData);
                m_bWarningOccurred = true;
            }
            else
            {
                if (dRealityMinUniformityValue <= 0.0)
                {
                    sWarningMessage = string.Format("Get Negative Uniformity Value={0:0.00}, ADC Min Value={1}, ADC Max Value={2} Error in Frequency={3:0.000}KHz)", dRealityMinUniformityValue.ToString("0.00"), nFrameMinValue, nFrameMaxValue, m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000"));
                    sMessage = string.Format("Get Negative Uniformity Value={0:0.00}, ADC Min Value={1}, ADC Max Value={2} Error", dRealityMinUniformityValue.ToString("0.00"), nFrameMinValue, nFrameMaxValue);
                }
                else if (nFrameMinValue < 0 && nFrameMaxValue < 0)
                {
                    sWarningMessage = string.Format("Get Negative ADC Min Value({0}) and ADC Max Value({1}) Error in Frequency={2:0.000}KHz", nFrameMinValue, nFrameMaxValue, m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000"));
                    sMessage = string.Format("Get Negative ADC Min Value({0}) and ADC Max Value({1}) Error", nFrameMinValue, nFrameMaxValue);
                }

                if (dRealityMinUniformityValue <= 0.0 || (nFrameMinValue < 0 && nFrameMaxValue < 0))
                {
                    OutputMessage(string.Format("Warning : {0}", sWarningMessage), true);

                    UniformityValueErrorData cUniformityValueErrorData = new UniformityValueErrorData();
                    cUniformityValueErrorData.m_dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;
                    cUniformityValueErrorData.m_nPH1PH2Sum = m_cDataInfo_List[nDataIndex].m_nPH1Value + m_cDataInfo_List[nDataIndex].m_nPH2Value;
                    cUniformityValueErrorData.m_sMessage = sMessage;
                    m_cUniformityValueErrorData_List.Add(cUniformityValueErrorData);
                    m_bWarningOccurred = true;
                }
                else
                {
                    int nADCHighBoundary = (int)Math.Round(32767 * 0.95, 0, MidpointRounding.AwayFromZero);

                    if (nFrameMaxValue >= nADCHighBoundary)
                    {
                        sWarningMessage = string.Format("Get ADC Value({0}) Over High Boundary({1}) Error in Frequency={2:0.000}KHz", nFrameMaxValue, nADCHighBoundary, m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000"));
                        OutputMessage(string.Format("Warning : {0}", sWarningMessage), true);

                        sMessage = string.Format("Get ADC Value({0}) Over High Boundary({1}) Error", nFrameMaxValue, nADCHighBoundary);

                        UniformityValueErrorData cUniformityValueErrorData = new UniformityValueErrorData();
                        cUniformityValueErrorData.m_dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;
                        cUniformityValueErrorData.m_nPH1PH2Sum = m_cDataInfo_List[nDataIndex].m_nPH1Value + m_cDataInfo_List[nDataIndex].m_nPH2Value;
                        cUniformityValueErrorData.m_sMessage = sMessage;
                        m_cUniformityValueErrorData_List.Add(cUniformityValueErrorData);
                        m_bWarningOccurred = true;
                    }

                    int[,] nFrameMean_Array = new int[nTXTraceNumber, nRXTraceNumber];

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            int nSumValue = 0;

                            for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
                                nSumValue += nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];

                            int nMeanValue = (int)Math.Round((double)nSumValue / nFrameData_List.Count, 0, MidpointRounding.AwayFromZero);
                            nFrameMean_Array[nTXIndex, nRXIndex] = nMeanValue;
                        }
                    }

                    /*
                    int nMaxOfMeanValue = 0;
                    int nMinOfMeanValue = 0;
            
                    for (int nTXIndex = 1; nTXIndex < nTXTraceNumber - 1; nTXIndex++)
                    {
                        for (int nRXIndex = 1; nRXIndex < nRXTraceNumber - 1; nRXIndex++)
                        {
                            if (nTXIndex == 1 && nRXIndex == 1)
                            {
                                nMaxOfMeanValue = nFrameMean_Array[nTXIndex, nRXIndex];
                                nMinOfMeanValue = nFrameMean_Array[nTXIndex, nRXIndex];
                            }
                            else
                            {
                                if (nFrameMean_Array[nTXIndex, nRXIndex] > nMaxOfMeanValue)
                                    nMaxOfMeanValue = nFrameMean_Array[nTXIndex, nRXIndex];

                                if (nFrameMean_Array[nTXIndex, nRXIndex] < nMinOfMeanValue)
                                    nMinOfMeanValue = nFrameMean_Array[nTXIndex, nRXIndex];
                            }
                        }
                    }

                    double dUniformityMeanValue = Math.Round(((double)nMinOfMeanValue / nMaxOfMeanValue) * 100, 3, MidpointRounding.AwayFromZero);

                    m_cDataInfo_List[nDataIndex].m_dUniformityMean = dUniformityMeanValue;
                    */
                }
            }

            return true;
        }

        private bool ComputeNoiseReferenceValue(int nDataIndex, List<int[,]> nFrameData_List, int nRXTraceNumber, int nTXTraceNumber, bool bComputeDVFlag = true)
        {
            List<int[,]> nDVFrameData_List = null;

            if (bComputeDVFlag == true)
            {
                if (ParamFingerAutoTuning.m_nFRPH2DataType == 2)
                {
                    nDVFrameData_List = new List<int[,]>();
                    int[,] nSingledVFrameData_Array = null;

                    for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
                    {
                        int[,] nBASEData_Array = new int[nTXTraceNumber, nRXTraceNumber];

                        if (nFrameIndex < m_nRecentADCMeanForBaseFrameNumber)
                        {
                            int nFrameHighBoundary = Math.Min(m_nRecentADCMeanForBaseFrameNumber, nFrameData_List.Count);

                            for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                            {
                                for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                                {
                                    double dSumValue = 0.0;

                                    for (int nRecentADCFrameIndex = 0; nRecentADCFrameIndex < nFrameHighBoundary; nRecentADCFrameIndex++)
                                        dSumValue += nFrameData_List[nRecentADCFrameIndex][nTXIndex, nRXIndex];

                                    int nMeanValue = (int)Math.Round(dSumValue / nFrameHighBoundary, 0, MidpointRounding.AwayFromZero);
                                    nBASEData_Array[nTXIndex, nRXIndex] = nMeanValue;
                                }
                            }
                        }
                        else
                        {
                            for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                            {
                                for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                                {
                                    double dSumValue = 0.0;

                                    for (int nRecentADCFrameIndex = nFrameIndex - m_nRecentADCMeanForBaseFrameNumber; nRecentADCFrameIndex < nFrameIndex; nRecentADCFrameIndex++)
                                        dSumValue += nFrameData_List[nRecentADCFrameIndex][nTXIndex, nRXIndex];

                                    int nMeanValue = (int)Math.Round(dSumValue / m_nRecentADCMeanForBaseFrameNumber, 0, MidpointRounding.AwayFromZero);
                                    nBASEData_Array[nTXIndex, nRXIndex] = nMeanValue;
                                }
                            }
                        }

                        nSingledVFrameData_Array = new int[nTXTraceNumber, nRXTraceNumber];

                        for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                        {
                            for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                            {
                                int nDVValue = nBASEData_Array[nTXIndex, nRXIndex] - nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                                nSingledVFrameData_Array[nTXIndex, nRXIndex] = nDVValue;
                            }
                        }

                        nDVFrameData_List.Add(nSingledVFrameData_Array);
                    }

                    WriteRecentADCMeanDVFrameData(m_cDataInfo_List[nDataIndex], nDVFrameData_List);
                }
                else
                {

                    nDVFrameData_List = new List<int[,]>();
                    int[,] nSingledVFrameData_Array = null;
                    int[,] nBASEData_Array = m_cDataInfo_List[nDataIndex].m_nBASEData_Array;

                    for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
                    {
                        nSingledVFrameData_Array = new int[nTXTraceNumber, nRXTraceNumber];

                        for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                        {
                            for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                            {
                                int nDVValue = nBASEData_Array[nTXIndex, nRXIndex] - nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                                nSingledVFrameData_Array[nTXIndex, nRXIndex] = nDVValue;
                            }
                        }

                        nDVFrameData_List.Add(nSingledVFrameData_Array);
                    }
                }

                //m_cDataInfo_List[nDataIndex].m_nBASEMinusADCData_List = new List<int[,]>();
                //m_cDataInfo_List[nDataIndex].m_nBASEMinusADCData_List = nDVFrameData_List;
            }
            else
            {
                nDVFrameData_List = nFrameData_List;

                //m_cDataInfo_List[nDataIndex].m_nBASEMinusADCData_List = new List<int[,]>();
                //m_cDataInfo_List[nDataIndex].m_nBASEMinusADCData_List = nDVFrameData_List;
            }

            List<long> lDVValue_List = new List<long>();
            bool bGetValueFlag = false;
            int nTotalMinValue = 0;

            for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
            {
                int nMinValue = 0;

                for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                {
                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        lDVValue_List.Add(nDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex]);
                        bGetValueFlag = true;

                        if (nRXIndex == 0 && nTXIndex == 0)
                            nMinValue = nDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                        else
                        {
                            if (nDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex] < nMinValue)
                                nMinValue = nDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                        }
                    }
                }

                if (nFrameIndex == 0)
                    nTotalMinValue = nMinValue;
                else
                {
                    if (nMinValue < nTotalMinValue)
                        nTotalMinValue = nMinValue;
                }
            }

            if (bGetValueFlag == true)
            {
                double dMean = Math.Round(lDVValue_List.Average(), 2, MidpointRounding.AwayFromZero);
                double dStd = Math.Round(MathMethod.ComputeStd(lDVValue_List), 2, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseMean = dMean;
                m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseStd = dStd;
            }

            double dPositiveReference = m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseMean + 3 * m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseStd;

            if (dPositiveReference < 0.0)
                dPositiveReference = 0.0;

            double dNegativeReference = m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseMean - 3 * m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseStd;

            if (dNegativeReference > 0.0)
                dNegativeReference = 0.0;

            m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoisePositiveReference = Math.Round(dPositiveReference, 2, MidpointRounding.AwayFromZero);
            m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseNegativeReference = Math.Round(dNegativeReference, 2, MidpointRounding.AwayFromZero);

            #region Normal Distribution
            /*
            int nSliceNumber = 3;

            int nMaxValue = lDVValue_List.Max();
            int nMinValue = lDVValue_List.Min();

            int nPartNumber = (int)((nMaxValue - nMinValue) / nSliceNumber) + 1;

            int[] nNumberData_Array = new int[nPartNumber];
            Array.Clear(nNumberData_Array, 0, nNumberData_Array.Length);

            for (int nValueIndex = 0; nValueIndex < lDVValue_List.Count; nValueIndex++)
            {
                int nNumber = (int)((lDVValue_List[nValueIndex] - nMinValue) / nSliceNumber);
                nNumberData_Array[nNumber]++;
            }

            double dMeanP1Std = m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNMean + 1 * m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNStd;
            double dMeanM1Std = m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNMean - 1 * m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNStd;
            double dMeanP2Std = m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNMean + 2 * m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNStd;
            double dMeanM2Std = m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNMean - 2 * m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNStd;
            double dMeanP3Std = m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNMean + 3 * m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNStd;
            double dMeanM3Std = m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNMean - 3 * m_cDataInfo_List[nListIndex].m_NR_SP1PHData.m_fNStd;

            int n1StdCount = 0;
            int n2StdCount = 0;
            int n3StdCount = 0;

            for (int nIndex = 0; nIndex < lDVValue_List.Count; nIndex++)
            {
                if (lDVValue_List[nIndex] >= dMeanM1Std && lDVValue_List[nIndex] <= dMeanP1Std)
                    n1StdCount++;

                if (lDVValue_List[nIndex] >= dMeanM2Std && lDVValue_List[nIndex] <= dMeanP2Std)
                    n2StdCount++;

                if (lDVValue_List[nIndex] >= dMeanM3Std && lDVValue_List[nIndex] <= dMeanP3Std)
                    n3StdCount++;
            }

            double d1StdPtg = Math.Round(((double)n1StdCount / lDVValue_List.Count) * 100, 2, MidpointRounding.AwayFromZero);
            double d2StdPtg = Math.Round(((double)n2StdCount / lDVValue_List.Count) * 100, 2, MidpointRounding.AwayFromZero);
            double d3StdPtg = Math.Round(((double)n3StdCount / lDVValue_List.Count) * 100, 2, MidpointRounding.AwayFromZero);

            double[] dDistributePtg_Array = new double[3] { d1StdPtg, d2StdPtg, d3StdPtg };

            WriteDistributionData(m_cDataInfo_List[nDataIndex].m_fFrequency, m_cDataInfo_List[nDataIndex].m_nPH1Value, m_cDataInfo_List[nDataIndex].m_nPH2Value,
                                  nSliceNumber, nMinValue, dDistributePtg_Array, nNumberData_Array);
            */
            #endregion

            lDVValue_List.Clear();
            lDVValue_List = null;

            if (m_nComputeRXTraceReference == 1)
            {
                /*
                if (WriteRXTraceData(m_cDataInfo_List[nDataIndex], nDVFrameData_List) == false)
                    return false;
                */

                ComputeRXTraceReference(m_cDataInfo_List[nDataIndex], nDVFrameData_List);
            }

            if (bGetValueFlag == true)
            {
                List<long> lOffsetDVValue_List = new List<long>();

                for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            int nOffsetValue = nDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex] - nTotalMinValue;
                            lOffsetDVValue_List.Add(nOffsetValue);
                        }
                    }
                }

                double dMean = Math.Round(lOffsetDVValue_List.Average(), 2, MidpointRounding.AwayFromZero);
                double dStd = Math.Round(MathMethod.ComputeStd(lOffsetDVValue_List), 2, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_cBASEMinusADCOffsetData.m_dBASEMinusADCOffsetMean = dMean;
                m_cDataInfo_List[nDataIndex].m_cBASEMinusADCOffsetData.m_dBASEMinusADCOffsetStd = dStd;
            }

            if (m_bRecordRecentADCMeanForBase == true)
            {
                List<int[,]> nRecentADCMeanDVFrameData_List = new List<int[,]>();
                int[,] nSingledVFrameData_Array = null;

                for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
                {
                    int[,] nBASEData_Array = new int[nTXTraceNumber, nRXTraceNumber];

                    if (nFrameIndex < m_nRecentADCMeanForBaseFrameNumber)
                    {
                        int nFrameHighBoundary = Math.Min(m_nRecentADCMeanForBaseFrameNumber, nFrameData_List.Count);
                        
                        for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                        {
                            for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                            {
                                double dSumValue = 0.0;

                                for (int nRecentADCFrameIndex = 0; nRecentADCFrameIndex < nFrameHighBoundary; nRecentADCFrameIndex++)
                                    dSumValue += nFrameData_List[nRecentADCFrameIndex][nTXIndex, nRXIndex];

                                int nMeanValue = (int)Math.Round(dSumValue / nFrameHighBoundary, 0, MidpointRounding.AwayFromZero);
                                nBASEData_Array[nTXIndex, nRXIndex] = nMeanValue;
                            }
                        }
                    }
                    else
                    {
                        for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                        {
                            for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                            {
                                double dSumValue = 0.0;

                                for (int nRecentADCFrameIndex = nFrameIndex - m_nRecentADCMeanForBaseFrameNumber; nRecentADCFrameIndex < nFrameIndex; nRecentADCFrameIndex++)
                                    dSumValue += nFrameData_List[nRecentADCFrameIndex][nTXIndex, nRXIndex];

                                int nMeanValue = (int)Math.Round(dSumValue / m_nRecentADCMeanForBaseFrameNumber, 0, MidpointRounding.AwayFromZero);
                                nBASEData_Array[nTXIndex, nRXIndex] = nMeanValue;
                            }
                        }
                    }

                    nSingledVFrameData_Array = new int[nTXTraceNumber, nRXTraceNumber];

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            int nDVValue = nBASEData_Array[nTXIndex, nRXIndex] - nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                            nSingledVFrameData_Array[nTXIndex, nRXIndex] = nDVValue;
                        }
                    }

                    nRecentADCMeanDVFrameData_List.Add(nSingledVFrameData_Array);
                }

                WriteRecentADCMeanDVFrameData(m_cDataInfo_List[nDataIndex], nRecentADCMeanDVFrameData_List);

                List<long> lRecentADCMeanDVValue_List = new List<long>();

                for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            int nValue = nRecentADCMeanDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                            lRecentADCMeanDVValue_List.Add(nValue);
                        }
                    }
                }

                double dMean = Math.Round(lRecentADCMeanDVValue_List.Average(), 2, MidpointRounding.AwayFromZero);
                double dStd = Math.Round(MathMethod.ComputeStd(lRecentADCMeanDVValue_List), 2, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_cRecentADCMeanForBaseData.m_dMean = dMean;
                m_cDataInfo_List[nDataIndex].m_cRecentADCMeanForBaseData.m_dStd = dStd;

                double dRecentADCMeanForBasePositiveReference = dMean + 3 * dStd;

                if (dRecentADCMeanForBasePositiveReference < 0.0)
                    dRecentADCMeanForBasePositiveReference = 0.0;

                double dRecentADCMeanForBaseNegativeReference = dMean - 3 * dStd;

                if (dRecentADCMeanForBaseNegativeReference > 0.0)
                    dRecentADCMeanForBaseNegativeReference = 0.0;

                dRecentADCMeanForBasePositiveReference = Math.Round(dRecentADCMeanForBasePositiveReference, 2, MidpointRounding.AwayFromZero);
                dRecentADCMeanForBaseNegativeReference = Math.Round(dRecentADCMeanForBaseNegativeReference, 2, MidpointRounding.AwayFromZero);

                double dRecentADCMeanForBaseSqrtDistanceReference = Math.Sqrt((dRecentADCMeanForBasePositiveReference * dRecentADCMeanForBasePositiveReference) +
                                                                              (dRecentADCMeanForBaseNegativeReference * dRecentADCMeanForBaseNegativeReference));

                if (dRecentADCMeanForBaseSqrtDistanceReference == 0.0)
                    dRecentADCMeanForBaseSqrtDistanceReference = 0.001;

                m_cDataInfo_List[nDataIndex].m_cRecentADCMeanForBaseData.m_dNoiseSqrtDistanceReference = Math.Round(dRecentADCMeanForBaseSqrtDistanceReference, 3, MidpointRounding.AwayFromZero);
            }

            return true;
        }

        private void WriteDistributionData(double dFrequency, int nPH1Value, int nPH2Value, int nSliceNumber, int nMinValue, double[] dDistributePercentage_Array, int[] nNumberData_Array)
        {
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_RESULT);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            string sReportFilePath = string.Format(@"{0}\{1:0.000}_0x{2}_0x{3}.csv", sDirectoryPath, dFrequency.ToString("0.000"), nPH1Value.ToString("X4"), nPH2Value.ToString("X4"));

            FileStream fs = new FileStream(sReportFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("Range,Number");

                for (int nNumberIndex = 0; nNumberIndex < nNumberData_Array.Length; nNumberIndex++)
                    sw.WriteLine(string.Format("{0}~{1},{2}", nMinValue + nNumberIndex * nSliceNumber, nMinValue + (nNumberIndex + 1) * nSliceNumber - 1, nNumberData_Array[nNumberIndex]));

                sw.WriteLine();
                sw.WriteLine("NDRange,NDPercentage");
                sw.WriteLine(string.Format("Mean+-1Std,{0}", dDistributePercentage_Array[0]));
                sw.WriteLine(string.Format("Mean+-2Std,{0}", dDistributePercentage_Array[1]));
                sw.WriteLine(string.Format("Mean+-3Std,{0}", dDistributePercentage_Array[2]));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WriteRecentADCMeanDVFrameData(DataInfo cDataInfo, List<int[,]> nRecentADCMeanDVFrameData_List)
        {
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_RecentADCMEANMinusADC);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            string sDataFilePath = string.Format(@"{0}\RADCMean-ADC_{1:0.000}_{2}_{3}.csv", sDirectoryPath, cDataInfo.m_dFrequency.ToString("0.000"), cDataInfo.m_nPH1Value.ToString("X2"), cDataInfo.m_nPH2Value.ToString("X2"));

            FileStream fs = new FileStream(sDataFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETYPE_PROCESS);
                sw.WriteLine(string.Format("DataType,{0}", MainConstantParameter.m_sDATATYPE_RecentADCMEANMinusADC));
                sw.WriteLine(string.Format("TXTraceNumber,{0}", m_nTXTraceNumber));
                sw.WriteLine(string.Format("RXTraceNumber,{0}", m_nRXTraceNumber));
                sw.WriteLine(string.Format("SetIndex,{0}", cDataInfo.m_nSetIndex));
                WriteParameter(sw, "SetPH1(Hex)", cDataInfo.m_nPH1Value, false, -1, true);
                WriteParameter(sw, "SetPH2(Hex)", cDataInfo.m_nPH2Value, false, -1, true);
                WriteParameter(sw, "SetPH3(Hex)", cDataInfo.m_nPH3Value, false, -1, true);
                WriteParameter(sw, "SetDFT_NUM(Hex)", cDataInfo.m_nDFT_NUMValue, false, -1, true);
                sw.WriteLine(string.Format("Frequency(KHz),{0:0.000}", cDataInfo.m_dFrequency.ToString("0.000")));
                sw.WriteLine("=====================================================");
                sw.WriteLine();

                for (int nFrameIndex = 0; nFrameIndex < nRecentADCMeanDVFrameData_List.Count; nFrameIndex++)
                {
                    if (nFrameIndex > 0)
                        sw.WriteLine();

                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < m_nRXTraceNumber - 1)
                                sw.Write(string.Format("{0},", nRecentADCMeanDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex]));
                            else
                                sw.WriteLine(nRecentADCMeanDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex]);
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

        private bool AnalysisData()
        {
            m_nPH2LB = m_cDataInfo_List[0].m_nPH2Value;

            ComputeRawSNR();

            ComputeNormalizeSNR();

            ComputeBestSort();

            ComputePH1PH2Pair();

            return true;
        }

        private void ComputeRawSNR()
        {
            double dMaxNoiseSqrtDistanceReference = 0.0;

            double dMaxNoiseSqrtDistanceReference_RecentADCForBase = 0.0;

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                double dUniformityReference = cDataInfo.m_dSingleMinUniformity;

                double dSqrtDistanceReference = Math.Sqrt((cDataInfo.m_cStatisticData.m_dNoisePositiveReference * cDataInfo.m_cStatisticData.m_dNoisePositiveReference) +
                                                          (cDataInfo.m_cStatisticData.m_dNoiseNegativeReference * cDataInfo.m_cStatisticData.m_dNoiseNegativeReference));

                if (dSqrtDistanceReference == 0.0)
                    dSqrtDistanceReference = 0.001;

                m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseSqrtDistanceReference = Math.Round(dSqrtDistanceReference, 3, MidpointRounding.AwayFromZero);

                if (m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseSqrtDistanceReference > dMaxNoiseSqrtDistanceReference)
                    dMaxNoiseSqrtDistanceReference = m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseSqrtDistanceReference;

                if (m_bRecordRecentADCMeanForBase == true)
                {
                    if (m_cDataInfo_List[nDataIndex].m_cRecentADCMeanForBaseData.m_dNoiseSqrtDistanceReference > dMaxNoiseSqrtDistanceReference_RecentADCForBase)
                        dMaxNoiseSqrtDistanceReference_RecentADCForBase = m_cDataInfo_List[nDataIndex].m_cRecentADCMeanForBaseData.m_dNoiseSqrtDistanceReference;
                }
            }

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                double dNoiseSqrtDistanceReference = m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseSqrtDistanceReference;

                double dNormalizeValue = (dNoiseSqrtDistanceReference - 0.0) / (dMaxNoiseSqrtDistanceReference - 0.0);

                if (dNormalizeValue < 0.001)
                    dNormalizeValue = 0.001;

                m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNormalizeNoiseSqrtDistanceReference = Math.Round(dNormalizeValue, 3, MidpointRounding.AwayFromZero);

                if (m_bRecordRecentADCMeanForBase == true)
                {
                    double dNoiseSqrtDistanceReference_RecentADCMeanForBase = m_cDataInfo_List[nDataIndex].m_cRecentADCMeanForBaseData.m_dNoiseSqrtDistanceReference;

                    double dNormalizeValue_RecentADCMeanForBase = (dNoiseSqrtDistanceReference_RecentADCMeanForBase - 0.0) / (dMaxNoiseSqrtDistanceReference_RecentADCForBase - 0.0);

                    if (dNormalizeValue_RecentADCMeanForBase < 0.001)
                        dNormalizeValue_RecentADCMeanForBase = 0.001;

                    m_cDataInfo_List[nDataIndex].m_cRecentADCMeanForBaseData.m_dNormalizeNoiseSqrtDistanceReference = Math.Round(dNormalizeValue_RecentADCMeanForBase, 3, MidpointRounding.AwayFromZero);
                }

                double dUniformityReference = Math.Round(cDataInfo.m_dSingleMinUniformity / 100, 3, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNormalizeUniformityReference = dUniformityReference;
            }

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                double dUniformityReference = cDataInfo.m_cStatisticData.m_dNormalizeUniformityReference;
                double dNormalizeSqrtDistanceReference = cDataInfo.m_cStatisticData.m_dNormalizeNoiseSqrtDistanceReference;

                //double dRawSNRValue = Math.Round(m_cDataInfo_List[nDataIndex].m_fNormalizeUniformity / m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNoiseSqrtDistanceReference, 3, MidpointRounding.AwayFromZero);
                double dRawSNRValue = Math.Round(dUniformityReference / dNormalizeSqrtDistanceReference, 3, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dRawSNR = dRawSNRValue;
            }

            if (m_bRecordRecentADCMeanForBase == true)
            {
                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                    double dUniformityReference = cDataInfo.m_cStatisticData.m_dNormalizeUniformityReference;
                    double dNormalizeSqrtDistanceReference = cDataInfo.m_cRecentADCMeanForBaseData.m_dNormalizeNoiseSqrtDistanceReference;
                    double dRawSNRValue = Math.Round(dUniformityReference / dNormalizeSqrtDistanceReference, 3, MidpointRounding.AwayFromZero);

                    m_cDataInfo_List[nDataIndex].m_cRecentADCMeanForBaseData.m_dRawSNR = dRawSNRValue;
                }
            }
        }

        private void ComputeNormalizeSNR()
        {
            double dMaxRawSNR = 0.0;
            double dMinRawSNR = 0.0;

            double dMaxRawSNR_RecentADCForBase = 0.0;

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                if (nDataIndex == 0)
                {
                    dMaxRawSNR = cDataInfo.m_cStatisticData.m_dRawSNR;
                    dMinRawSNR = cDataInfo.m_cStatisticData.m_dRawSNR;
                }
                else
                {
                    if (cDataInfo.m_cStatisticData.m_dRawSNR > dMaxRawSNR)
                        dMaxRawSNR = cDataInfo.m_cStatisticData.m_dRawSNR;

                    if (cDataInfo.m_cStatisticData.m_dRawSNR < dMinRawSNR)
                        dMinRawSNR = cDataInfo.m_cStatisticData.m_dRawSNR;
                }

                if (m_bRecordRecentADCMeanForBase == true)
                {
                    if (nDataIndex == 0)
                    {
                        dMaxRawSNR_RecentADCForBase = cDataInfo.m_cRecentADCMeanForBaseData.m_dRawSNR;
                    }
                    else
                    {
                        if (cDataInfo.m_cRecentADCMeanForBaseData.m_dRawSNR > dMaxRawSNR_RecentADCForBase)
                            dMaxRawSNR_RecentADCForBase = cDataInfo.m_cRecentADCMeanForBaseData.m_dRawSNR;
                    }
                }
            }

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                double dNormalizeSNR = 0.0;

                if (ParamFingerAutoTuning.m_nFRPH2NormalizeType == 1)
                    dNormalizeSNR = Math.Round((cDataInfo.m_cStatisticData.m_dRawSNR / dMaxRawSNR) * 100, 2, MidpointRounding.AwayFromZero);
                else
                    dNormalizeSNR = Math.Round((cDataInfo.m_cStatisticData.m_dRawSNR - dMinRawSNR) / (dMaxRawSNR - dMinRawSNR) * 100, 2, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dNormalizeSNRReference = dNormalizeSNR;

                if (m_bRecordRecentADCMeanForBase == true)
                {
                    double dNormalizeSNR_RecentADCForBase = Math.Round((cDataInfo.m_cRecentADCMeanForBaseData.m_dRawSNR / dMaxRawSNR_RecentADCForBase) * 100, 2, MidpointRounding.AwayFromZero);
                    m_cDataInfo_List[nDataIndex].m_cRecentADCMeanForBaseData.m_dNormalizeSNRReference = dNormalizeSNR_RecentADCForBase;
                }
            }

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                double dMeanValue = 0.0;
                int nCount = 0;

                for (int nCompareIndex = 0; nCompareIndex < m_cDataInfo_List.Count; nCompareIndex++)
                {
                    DataInfo cCompareDataInfo = m_cDataInfo_List[nCompareIndex];

                    double dFrequencyDiffer = Math.Abs(cDataInfo.m_dFrequency - cCompareDataInfo.m_dFrequency);

                    if (dFrequencyDiffer <= 5.0)
                    {
                        dMeanValue += cCompareDataInfo.m_cStatisticData.m_dNormalizeSNRReference;
                        nCount++;
                    }
                }

                /*
                dMeanValue = Math.Round(dMeanValue / nCount, 3, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_cStatisticData.m_dRangeMeanNoiseSNR = dMeanValue;
                */
            }
        }

        private void ComputeBestSort()
        {
            #region Mark It
            /*
            #region Top10 Mean
            m_CompareOperator = COM_NORMALIZESNR;
            m_cDataInfo_List.Sort(new DataInfoComparer());
            
            double dMeanValue = 0.0;

            for (int nDataIndex = 0; nDataIndex < m_nBestRankNumber; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];
                dMeanValue += cDataInfo.m_NR_SP1PHData.m_fNormalizeSNR;
            }

            dMeanValue = dMeanValue / m_nBestRankNumber;

            if (dMeanValue > m_dBestPercentage)
                m_dBestPercentage = dMeanValue;
            #endregion

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                bool bAddFlag = false;
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                if (cDataInfo.m_NR_SP1PHData.m_fNormalizeSNR >= m_dBestPercentage)
                    bAddFlag = true;

                if (bAddFlag == true)
                {
                    BestRankSortData cBestRankSortData = new BestRankSortData();
                    cBestRankSortData.m_sFileName = cDataInfo.m_sFileName;
                    cBestRankSortData.m_nPH1Value = cDataInfo.m_nPH1Value;
                    cBestRankSortData.m_nPH2Value = cDataInfo.m_nPH2Value;
                    cBestRankSortData.m_fFrequency = cDataInfo.m_fFrequency;
                    cBestRankSortData.m_nSuggestSumValue = cDataInfo.m_nSuggestSumValue;
                    cBestRankSortData.m_fNormalizeSNR = cDataInfo.m_NR_SP1PHData.m_fNormalizeSNR;

                    m_cBestRankSortData_List.Add(cBestRankSortData);
                }
            }

            if (m_cBestRankSortData_List.Count < m_nBestRankNumber)
            {
                int nDiffer = m_nBestRankNumber - m_cBestRankSortData_List.Count;

                double dMaxDiffer = 0.0;
                int nMaxDifferIndex = 0;

                for (int nDataIndex = m_cBestRankSortData_List.Count - 1; nDataIndex < m_cBestRankSortData_List.Count + nDiffer - 1; nDataIndex++)
                {
                    if (nDataIndex == m_cBestRankSortData_List.Count - 1)
                    {
                        double dTMean = m_fBestPercentage;

                        double dBMean = 0.0;
                        int nBCount = 0;
             
                        for (int nCompareIndex = nDataIndex + 1; nCompareIndex < m_cBestRankSortData_List.Count + nDiffer; nCompareIndex++)
                        {
                            DataInfo cDataInfo = m_cDataInfo_List[nCompareIndex];

                            double dValue = cDataInfo.m_NR_SP1PHData.m_fNormalizeSNR;

                            dBMean += dValue;
                            nBCount++;
                        }

                        dBMean = dBMean / nBCount;
                        dMaxDiffer = dTMean - dBMean;
                        nMaxDifferIndex = nDataIndex;
                    }
                    else
                    {
                        double dTMean = m_dBestPercentage;
                        int nTCount = 1;
             
                        for (int nCompareIndex = m_cDataInfo_List.Count; nCompareIndex <= nDataIndex; nCompareIndex++)
                        {
                            DataInfo cDataInfo = m_cDataInfo_List[nCompareIndex];

                            double dValue = cDataInfo.m_NR_SP1PHData.m_fNormalizeSNR;

                            dTMean += dValue;
                            nTCount++;
                        }

                        dTMean = dTMean / nTCount;

                        double dBMean = 0.0;
                        int nBCount = 0;
             
                        for (int nCompareIndex = nDataIndex + 1; nCompareIndex < m_cDataInfo_List.Count + nDiffer; nCompareIndex++)
                        {
                            DataInfo cDataInfo =  m_cDataInfo_List[nCompareIndex];

                            double dValue = cDataInfo.m_NR_SP1PHData.m_fNormalizeSNR;

                            dBMean += dValue;
                            nBCount++;
                        }

                        dBMean = dBMean / nBCount;

                        double dDiffer = dTMean - dBMean;

                        if (dDiffer > dMaxDiffer)
                        {
                            dMaxDiffer = dDiffer;
                            nMaxDifferIndex = nDataIndex;
                        }
                    }
                }

                if (nMaxDifferIndex > m_cBestRankSortData_List.Count - 1)
                {
                    for (int nDataIndex = m_cBestRankSortData_List.Count; nDataIndex <= nMaxDifferIdx; nDataIndex++)
                    {
                        DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                        BestRankSortData cBestRankSortData = new BestRankSortData();
                        cBestRankSortData.m_sFileName = cDataInfo.m_sFileName;
                        cBestRankSortData.m_nPH1Value = cDataInfo.m_nPH1Value;
                        cBestRankSortData.m_nPH2Value = cDataInfo.m_nPH2Value;
                        cBestRankSortData.m_fFrequency = cDataInfo.m_fFrequency;
                        cBestRankSortData.m_nSuggestSumValue = cDataInfo.m_nSuggestSumValue;
                        cBestRankSortData.m_fNormalizeSNR = cDataInfo.m_NR_SP1PHData.m_fNormalizeSNR;

                        m_cBestRankSortData_List.Add(cBestRankSortData);
                    }
                }
            }

            m_CompareOperator = COM_FREQUENCY;
            m_cBestRankSortData_List.Sort(new BestRankSortDataComparer());

            if (m_cBestRankSortData_List.Count > m_nBestRankNumber)
            {
                for (int nDataIndex = m_BestSortDataList.Count - 1; nDataIndex >= m_nBestRankNumber; nDataIndex--)
                    m_cBestRankSortData_List.RemoveAt(nDataIndex);
            }
            else if (m_BestSortDataList.Count < m_nBestRankNumber)
            {
                int nDiffer = m_nBestRankNumber - m_cBestRankSortData_List.Count;
                
                for (int nDataIndex = m_cBestRankSortData_List.Count; nDataIndex < m_cBestRankSortData_List.Count + nDiffer; nDataIndex++)
                {
                    DataInfo cDataInfo = m_cDataInfo_List[nIndex];

                    BestRankSortData cBestRankSortData = new BestRankSortData();
                    cBestRankSortData.m_sFileName = cDataInfo.m_sFileName;
                    cBestRankSortData.m_nPH1Value = cDataInfo.m_nPH1Value;
                    cBestRankSortData.m_nPH2Value = cDataInfo.m_nPH2Value;
                    cBestRankSortData.m_fFrequency = cDataInfo.m_fFrequency;
                    cBestRankSortData.m_nSuggestSumValue = cDataInfo.m_nSuggestSumValue;
                    cBestRankSortData.m_fNormalizeSNR = cDataInfo.m_NR_SP1PHData.m_fNormalizeSNR;

                    m_cBestRankSortData_List.Add(cBestRankSortData);

                    if (m_cBestRankSortData_List.Count == m_nBestRankNumber)
                        break;
                }
            }
            */
            #endregion

            m_nCompareOperator = m_nCOMPARE_Normalize;
            m_cDataInfo_List.Sort(new DataInfoComparer());

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                if (nDataIndex < m_nBestRankNumber)
                {
                    DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                    BestRankSortData cBestRankSortData = new BestRankSortData();
                    cBestRankSortData.m_sBASEFileName = cDataInfo.m_sBASEFileName;
                    cBestRankSortData.m_sADCFileName = cDataInfo.m_sADCFileName;
                    cBestRankSortData.m_nPH1Value = cDataInfo.m_nPH1Value;
                    cBestRankSortData.m_nPH2Value = cDataInfo.m_nPH2Value;
                    cBestRankSortData.m_dFrequency = cDataInfo.m_dFrequency;
                    cBestRankSortData.m_nSuggestDFT_NUM = cDataInfo.m_nSuggestDFT_NUM;
                    cBestRankSortData.m_dNormalizeSNRReference = cDataInfo.m_cStatisticData.m_dNormalizeSNRReference;

                    m_cBestRankSortData_List.Add(cBestRankSortData);
                }
            }
        }

        private void ComputePH1PH2Pair()
        {
            for (int nDataIndex = 0; nDataIndex < m_cBestRankSortData_List.Count; nDataIndex++)
            {
                BestRankSortData cBestRankSortData = m_cBestRankSortData_List[nDataIndex];

                int nPH2LB = m_nPH2LB;
                int nPH1PH2Sum = cBestRankSortData.m_nPH1Value + nPH2LB;

                int nPH2Value = nPH2LB;
                int nPH1Value = nPH1PH2Sum - nPH2Value;

                while (nPH1Value >= m_nMinPH1)
                {
                    if (nPH1Value <= nPH2Value)
                    {
                        PH1PH2Pair cPH1PH2Pair = new PH1PH2Pair();
                        cPH1PH2Pair.m_nPH1Value = nPH1Value;
                        cPH1PH2Pair.m_nPH2Value = nPH2Value;
                        m_cBestRankSortData_List[nDataIndex].m_cPH1PH2Pair_List.Add(cPH1PH2Pair);
                    }

                    nPH2Value++;
                    nPH1Value--;
                }
            }
        }

        private bool SaveLineChartFile()
        {
            bool bErrorFlag = false;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            if (m_bGenerateH5Data == true)
            {
                string sH5DirectoryPath = string.Format(@"{0}\{1}", m_sH5LogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART);

                if (Directory.Exists(sH5DirectoryPath) == false)
                    Directory.CreateDirectory(sH5DirectoryPath);
            }

            string sTitleName = "ReferenceValue Distribution By Frequency";
            string sFilePath = string.Format(@"{0}\RefValueChart.jpg", sDirectoryPath);

            m_nCompareOperator = m_nCOMPARE_Frequency;
            m_cDataInfo_List.Sort(new DataInfoComparer());

            int nInterval = 5;
            double dMaxFrequency = 0.0;
            double dMinFrequency = 0.0;

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                if (nDataIndex == 0)
                {
                    dMaxFrequency = cDataInfo.m_dFrequency;
                    dMinFrequency = cDataInfo.m_dFrequency;
                }
                else
                {
                    if (cDataInfo.m_dFrequency > dMaxFrequency)
                        dMaxFrequency = cDataInfo.m_dFrequency;

                    if (cDataInfo.m_dFrequency < dMinFrequency)
                        dMinFrequency = cDataInfo.m_dFrequency;
                }
            }

            int nMinFrequency = (int)dMinFrequency;

            double dDiffer = dMaxFrequency - (double)nMinFrequency;
            int nPart = (int)(dDiffer / nInterval);

            if (dDiffer % nInterval != 0)
                nPart++;

            int nMaxFrequency = nMinFrequency + nPart * nInterval;

            Series cSeries = new Series("ReferenceValue vs. Frequency");

            //Show Line Chart
            Chart cChart = new Chart();
            var vChartArea = new ChartArea();
            cChart.ChartAreas.Add(vChartArea);
            cChart.Width = 1500;
            cChart.Height = 500;
            cChart.Legends.Add("Legend");
            cChart.Legends["Legend"].Font = new Font("Times New Roman", 10);
            cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
            cChart.Titles.Add(sTitleName);
            cChart.Titles[0].Font = new Font("Times New Roman", 18);
            cChart.ChartAreas[0].AxisY.Title = "Reference Value";
            cChart.ChartAreas[0].AxisY.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.ChartAreas[0].AxisX.Title = "Frequency(KHz)";
            cChart.ChartAreas[0].AxisX.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Times New Roman", 10);
            //cChart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //cChart.ChartAreas[0].AxisX.IsLabelAutoFit = false;
            //cChart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;
            cChart.ChartAreas[0].AxisX.Minimum = nMinFrequency;
            cChart.ChartAreas[0].AxisX.Maximum = nMaxFrequency;
            cChart.ChartAreas[0].AxisX.Interval = nInterval;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            if (m_nChartReferenceValueHB != m_nChartReferenceValueLB)
            {
                if (m_nChartReferenceValueHB < m_nChartReferenceValueLB)
                {
                    int nValue = m_nChartReferenceValueHB;
                    m_nChartReferenceValueHB = m_nChartReferenceValueLB;
                    m_nChartReferenceValueLB = nValue;
                }

                cChart.ChartAreas[0].AxisY.Maximum = m_nChartReferenceValueHB;
                cChart.ChartAreas[0].AxisY.Minimum = m_nChartReferenceValueLB;
            }

            if (m_nChartReferenceValueInterval > 0)
                cChart.ChartAreas[0].AxisY.Interval = m_nChartReferenceValueInterval;

            cSeries.ChartType = SeriesChartType.Line;
            cSeries.MarkerStyle = MarkerStyle.Circle;
            cSeries.MarkerSize = 5;
            cSeries.IsValueShownAsLabel = false;
            cSeries.Color = Color.Blue;

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];
                //cSeries.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cStatisticData.m_dNormalizeSNRReference);
                var vLine = cSeries.Points[cSeries.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cStatisticData.m_dNormalizeSNRReference)];

                int nFindIndex = m_cBestRankSortData_List.FindIndex(x => x.m_dFrequency == cDataInfo.m_dFrequency);

                if (nFindIndex >= 0)
                {
                    vLine.MarkerColor = Color.Red;
                    vLine.Label = string.Format("{0}KHz", cDataInfo.m_dFrequency);
                    vLine.IsValueShownAsLabel = true;
                }
                else
                    vLine.MarkerColor = Color.Blue;

                vLine.Color = Color.Blue;
            }

            cChart.Series.Add(cSeries);

            try
            {
                cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
            }
            catch
            {
                m_sErrorMessage = "Save Chart File Error";
                bErrorFlag = true;
            }

            if (bErrorFlag == true)
                return false;
            else
                return true;
        }

        private bool SaveAnalysisFile()
        {
            bool bErrorFlag = false;
            string sReportFilePath = string.Format(@"{0}\Analysis.csv", m_sLogDirectoryPath);

            FileStream fs = new FileStream(sReportFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string[] sColumnHeader_Rank_Array = new string[] { 
                "Rank", 
                "PH1", 
                "PH2", 
                "Frequency(KHz)", 
                "DFT_NUM", 
                "SuggestDFT_NUM", 
                "Signal RefValue", 
                "Normalize Signal RefValue", 
                "Noise Mean", 
                "Noise Std", 
                "Noise PosRef", 
                "Noise NegRef", 
                "Noise RefValue", 
                "Normalize Noise RefValue", 
                "RawSNR RefValue", 
                "Reference Value" 
            };
            string[] sColumnHeader_Frequency_Array = new string[] { 
                "PH1", 
                "PH2", 
                "Frequency(KHz)", 
                "DFT_NUM", 
                "SuggestDFT_NUM", 
                "Signal RefValue", 
                "Normalize Signal RefValue", 
                "Noise Mean", 
                "Noise Std", 
                "Noise PosRef", 
                "Noise NegRef", 
                "Noise RefValue", 
                "Normalize Noise RefValue", 
                "RawSNR RefValue", 
                "Reference Value" 
            };
            string[] sColumnHeader_OtherReference_Array = new string[] { 
                "PH1", 
                "PH2", 
                "Frequency(KHz)", 
                "DFT_NUM", 
                "ADCMaxMinDiffer", 
                "ADCMean", 
                "BASE-ADCOffsetMean", 
                "BASE-ADCOffsetStd", 
                "", 
                "RecentADCForBASE-ADC Mean", 
                "RecentADCForBASE-ADC Std", 
                "RecentADCForBASE-ADC Noise RefValue", 
                "RecentADCForBASE-ADC Normalize Noise RefValue", 
                "RecentADCForBASE-ADC RawSNR RefValue", 
                "RecentADCForBASE-ADC Reference Value"
            };
            string[] sColumnHeader_BestSort_Array = new string[] { 
                "Rank", 
                "PH1", 
                "PH2", 
                "Frequency(KHz)", 
                "SuggestDFT_NUM", 
                "Reference Value" 
            };
            string[] sColumnHeader_BestSortPH1PH2Pair_Array = new string[] { 
                "Rank", 
                "Frequency(KHz)", 
                "SuggestDFT_NUM", 
                "Reference Value", 
                "", 
                "PH1", 
                "PH2" 
            };

            try
            {
                Write_Tool_Information(sw, m_sFILETYPE_ANALYSIS);

                m_nCompareOperator = m_nCOMPARE_Normalize;
                m_cDataInfo_List.Sort(new DataInfoComparer());

                sw.WriteLine("Rank Information");

                for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_Rank_Array.Length; nColumnIndex++)
                {
                    if (nColumnIndex != sColumnHeader_Rank_Array.Length - 1)
                        sw.Write(string.Format("{0},", sColumnHeader_Rank_Array[nColumnIndex]));
                    else
                        sw.WriteLine(sColumnHeader_Rank_Array[nColumnIndex]);
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                    sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                    sw.Write(string.Format("0x{0},", cDataInfo.m_nPH1Value.ToString("x2").ToUpper()));
                    sw.Write(string.Format("0x{0},", cDataInfo.m_nPH2Value.ToString("x2").ToUpper()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSuggestDFT_NUM.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dSingleMinUniformity.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNormalizeUniformityReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseStd.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoisePositiveReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseNegativeReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseSqrtDistanceReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNormalizeNoiseSqrtDistanceReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dRawSNR.ToString("0.000")));
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_cStatisticData.m_dNormalizeSNRReference.ToString("0.00")));
                }

                sw.WriteLine();

                m_nCompareOperator = m_nCOMPARE_Frequency;
                m_cDataInfo_List.Sort(new DataInfoComparer());

                sw.WriteLine("Frequency Information");

                for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_Frequency_Array.Length; nColumnIndex++)
                {
                    if (nColumnIndex != sColumnHeader_Frequency_Array.Length - 1)
                        sw.Write(string.Format("{0},", sColumnHeader_Frequency_Array[nColumnIndex]));
                    else
                        sw.WriteLine(sColumnHeader_Frequency_Array[nColumnIndex]);
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                    sw.Write(string.Format("0x{0},", cDataInfo.m_nPH1Value.ToString("x2").ToUpper()));
                    sw.Write(string.Format("0x{0},", cDataInfo.m_nPH2Value.ToString("x2").ToUpper()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSuggestDFT_NUM.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dSingleMinUniformity.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNormalizeUniformityReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseStd.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoisePositiveReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseNegativeReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseSqrtDistanceReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNormalizeNoiseSqrtDistanceReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dRawSNR.ToString("0.000")));
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_cStatisticData.m_dNormalizeSNRReference.ToString("0.00")));
                }

                sw.WriteLine();

                sw.WriteLine("Other Reference by Frequency");

                for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_OtherReference_Array.Length; nColumnIndex++)
                {
                    if (nColumnIndex != sColumnHeader_OtherReference_Array.Length - 1)
                        sw.Write(string.Format("{0},", sColumnHeader_OtherReference_Array[nColumnIndex]));
                    else
                        sw.WriteLine(sColumnHeader_OtherReference_Array[nColumnIndex]);
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                    sw.Write(string.Format("0x{0},", cDataInfo.m_nPH1Value.ToString("x2").ToUpper()));
                    sw.Write(string.Format("0x{0},", cDataInfo.m_nPH2Value.ToString("x2").ToUpper()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_cBASEMinusADCOffsetData.m_dADCMaxMinDiffer.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cBASEMinusADCOffsetData.m_dADCMean.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cBASEMinusADCOffsetData.m_dBASEMinusADCOffsetMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cBASEMinusADCOffsetData.m_dBASEMinusADCOffsetStd.ToString("0.00")));
                    sw.Write(",");

                    if (m_bRecordRecentADCMeanForBase == true)
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_cRecentADCMeanForBaseData.m_dMean.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cRecentADCMeanForBaseData.m_dStd.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cRecentADCMeanForBaseData.m_dNoiseSqrtDistanceReference.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cRecentADCMeanForBaseData.m_dNormalizeNoiseSqrtDistanceReference.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cRecentADCMeanForBaseData.m_dRawSNR.ToString("0.000")));
                        sw.WriteLine(cDataInfo.m_cRecentADCMeanForBaseData.m_dNormalizeSNRReference.ToString("0.00"));
                    }
                    else
                    {
                        sw.Write(",");
                        sw.Write(",");
                        sw.Write(",");
                        sw.Write(",");
                        sw.Write(",");
                        sw.WriteLine();
                    }
                }

                sw.WriteLine();

                sw.WriteLine("BestSort Data");

                for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_BestSort_Array.Length; nColumnIndex++)
                {
                    if (nColumnIndex != sColumnHeader_BestSort_Array.Length - 1)
                        sw.Write(string.Format("{0},", sColumnHeader_BestSort_Array[nColumnIndex]));
                    else
                        sw.WriteLine(sColumnHeader_BestSort_Array[nColumnIndex]);
                }

                for (int nDataIndex = 0; nDataIndex < m_cBestRankSortData_List.Count; nDataIndex++)
                {
                    BestRankSortData cBestRankSortData = m_cBestRankSortData_List[nDataIndex];

                    sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                    sw.Write(string.Format("0x{0},", cBestRankSortData.m_nPH1Value.ToString("x2").ToUpper()));
                    sw.Write(string.Format("0x{0},", cBestRankSortData.m_nPH2Value.ToString("x2").ToUpper()));
                    sw.Write(string.Format("{0},", cBestRankSortData.m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", cBestRankSortData.m_nSuggestDFT_NUM.ToString()));
                    sw.WriteLine(string.Format("{0}", cBestRankSortData.m_dNormalizeSNRReference.ToString("0.00")));
                }

                sw.WriteLine();

                sw.WriteLine("BestSort PH1PH2Pair");

                for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_BestSortPH1PH2Pair_Array.Length; nColumnIndex++)
                {
                    if (nColumnIndex != sColumnHeader_BestSortPH1PH2Pair_Array.Length - 1)
                        sw.Write(string.Format("{0},", sColumnHeader_BestSortPH1PH2Pair_Array[nColumnIndex]));
                    else
                        sw.WriteLine(sColumnHeader_BestSortPH1PH2Pair_Array[nColumnIndex]);
                }

                for (int nDataIndex = 0; nDataIndex < m_cBestRankSortData_List.Count; nDataIndex++)
                {
                    BestRankSortData cBestRankSortData = m_cBestRankSortData_List[nDataIndex];

                    for (int nPairIndex = 0; nPairIndex < cBestRankSortData.m_cPH1PH2Pair_List.Count; nPairIndex++)
                    {
                        if (nPairIndex == 0)
                        {
                            sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                            sw.Write(string.Format("{0},", cBestRankSortData.m_dFrequency.ToString("0.000")));
                            sw.Write(string.Format("{0},", cBestRankSortData.m_nSuggestDFT_NUM.ToString()));
                            sw.Write(string.Format("{0},", cBestRankSortData.m_dNormalizeSNRReference.ToString("0.00")));
                            sw.Write(",");
                            sw.Write(string.Format("0x{0},", cBestRankSortData.m_cPH1PH2Pair_List[nPairIndex].m_nPH1Value.ToString("x2").ToUpper()));
                            sw.WriteLine(string.Format("0x{0}", cBestRankSortData.m_cPH1PH2Pair_List[nPairIndex].m_nPH2Value.ToString("x2").ToUpper()));
                        }
                        else
                        {
                            sw.Write(",,,,,");
                            sw.Write(string.Format("0x{0},", cBestRankSortData.m_cPH1PH2Pair_List[nPairIndex].m_nPH1Value.ToString("x2").ToUpper()));
                            sw.WriteLine(string.Format("0x{0}", cBestRankSortData.m_cPH1PH2Pair_List[nPairIndex].m_nPH2Value.ToString("x2").ToUpper()));
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = "Save Analysis Data Error";
                bErrorFlag = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bErrorFlag == true)
                return false;
            else
                return true;
        }

        private bool SaveReportFile()
        {
            int nTXnBitValue = m_cDataInfo_List[0].m_nFWIP_Option & 0x0100;

            m_bEnableTXn = (nTXnBitValue > 0) ? true : false;

            bool bErrorFlag = false;
            string sReportFilePath = string.Format(@"{0}\Report.csv", m_sLogDirectoryPath);

            FileStream fs = new FileStream(sReportFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string[] sColumnHeader_FrequencyRank_Array = new string[] { 
                "Rank", 
                "Frequency(KHz)", 
                "PH1+PH2", 
                "Minimum PH1", 
                "Minimum PH2", 
                "DFT_NUM", 
                "Suggest DFT_NUM", 
                "Signal RefValue", 
                "Noise Mean", 
                "Noise Std", 
                "Noise PosRef", 
                "Noise NegRef",
                "Noise RefValue", 
                "RawSNR RefValue", 
                "Reference Value" 
            };

            try
            {
                Write_Tool_Information(sw, m_sFILETYPE_REPORT);

                sw.WriteLine("Option Record:");

                if (m_bEnableTXn == true)
                    sw.WriteLine("EnableTXn,1");
                else
                    sw.WriteLine("EnableTXn,0");

                sw.WriteLine("Analog Parameter Info:");
                sw.WriteLine(string.Format("TXTraceNumber,{0}", m_nTXTraceNumber.ToString("D")));
                sw.WriteLine(string.Format("RXTraceNumber,{0}", m_nRXTraceNumber.ToString("D")));
                sw.WriteLine(string.Format("MinPH1(Hex),{0}", m_nMinPH1.ToString("x2").ToUpper()));
                sw.WriteLine(string.Format("PH2LB(Hex),{0}", m_nPH2LB.ToString("x2").ToUpper()));

                sw.WriteLine();

                m_nCompareOperator = m_nCOMPARE_Normalize;
                m_cDataInfo_List.Sort(new DataInfoComparer());

                sw.WriteLine("Frequency Rank:");

                for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_FrequencyRank_Array.Length; nColumnIndex++)
                {
                    if (nColumnIndex != sColumnHeader_FrequencyRank_Array.Length - 1)
                        sw.Write(string.Format("{0},", sColumnHeader_FrequencyRank_Array[nColumnIndex]));
                    else
                        sw.WriteLine(sColumnHeader_FrequencyRank_Array[nColumnIndex]);
                }

                for (int nListIndex = 0; nListIndex < m_cDataInfo_List.Count; nListIndex++)
                {
                    DataInfo cDataInfo = m_cDataInfo_List[nListIndex];

                    int nPH1PH2Sum = cDataInfo.m_nPH1Value + cDataInfo.m_nPH2Value;

                    sw.Write(string.Format("{0},", (nListIndex + 1).ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("0x{0},", nPH1PH2Sum.ToString("x2").ToUpper()));
                    sw.Write(string.Format("0x{0},", m_nMinPH1.ToString("x2").ToUpper()));
                    sw.Write(string.Format("0x{0},", m_nPH2LB.ToString("x2").ToUpper()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSuggestDFT_NUM.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dSingleMinUniformity.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseStd.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoisePositiveReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseNegativeReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dNoiseSqrtDistanceReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cStatisticData.m_dRawSNR.ToString("0.000")));
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_cStatisticData.m_dNormalizeSNRReference.ToString("0.00")));
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = "Save Report Data Error";
                bErrorFlag = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bErrorFlag == true)
                return false;
            else
                return true;
        }

        private bool ProcessRXTraceData()
        {
            //foreach (DataInfo cDataInfo in m_cDataInfo_List)
            //{
            //    /*
            //    if (WriteRXTraceData(cDataInfo) == false)
            //        return false;
            //    */
            //
            //    ComputeRXTraceReference(cDataInfo);
            //}
        
            if (WriteRXTraceReference() == false)
                return false;
        
            return true;
        }

        /*
        private bool WriteRXTraceData(DataInfo cDataInfo)
        {
            bool bErrorFlag = false;
            int nFrameNumber = cDataInfo.m_nBASEMinusADCData_List.Count;

            string sDirectoryName = MainConstantParameter.m_sDATATYPE_ANALYSIS;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            double dFrequency = cDataInfo.m_dFrequency;

            string sFileName = string.Format("RXTraceData_{0:0.000}_{1}_{2}", dFrequency.ToString("0.000"), cDataInfo.m_nPH1Value.ToString("x2").ToUpper(), cDataInfo.m_nPH2Value.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("RXTraceData");

                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < cDataInfo.m_nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < cDataInfo.m_nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex == cDataInfo.m_nRXTraceNumber - 1)
                                sw.WriteLine(cDataInfo.m_nBASEMinusADCData_List[nFrameIndex][nTXIndex, nRXIndex]);
                            else
                                sw.Write(string.Format("{0},", cDataInfo.m_nBASEMinusADCData_List[nFrameIndex][nTXIndex, nRXIndex]));
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save RXTrace Data Error in Frequency={0:0.000}KHz", dFrequency.ToString("0.000"));
                bErrorFlag = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bErrorFlag == true)
                return false;
            else
                return true;
        }
        */

        private bool WriteRXTraceData(DataInfo cDataInfo, List<int[,]> nBASEMinusADCData_List)
        {
            bool bErrorFlag = false;
            int nFrameNumber = nBASEMinusADCData_List.Count;

            string sDirectoryName = MainConstantParameter.m_sDATATYPE_ANALYSIS;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            double dFrequency = cDataInfo.m_dFrequency;

            string sFileName = string.Format("RXTraceData_{0:0.000}_{1}_{2}", dFrequency.ToString("0.000"), cDataInfo.m_nPH1Value.ToString("x2").ToUpper(), cDataInfo.m_nPH2Value.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("RXTraceData");

                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < cDataInfo.m_nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < cDataInfo.m_nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex == cDataInfo.m_nRXTraceNumber - 1)
                                sw.WriteLine(nBASEMinusADCData_List[nFrameIndex][nTXIndex, nRXIndex]);
                            else
                                sw.Write(string.Format("{0},", nBASEMinusADCData_List[nFrameIndex][nTXIndex, nRXIndex]));
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save RXTrace Data Error in Frequency={0:0.000}KHz", dFrequency.ToString("0.000"));
                bErrorFlag = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bErrorFlag == true)
                return false;
            else
                return true;
        }

        /*
        private void ComputeRXTraceReference(DataInfo cDataInfo)
        {
            List<int> nNoPenEffectValue_List = new List<int>();

            for (int nRXTraceIndex = 0; nRXTraceIndex < cDataInfo.m_nRXTraceNumber; nRXTraceIndex++)
            {
                List<int> nValue_List = new List<int>();

                foreach (int[,] nFrameData_Array in cDataInfo.m_nBASEMinusADCData_List)
                {
                    for (int nTXTraceIndex = 0; nTXTraceIndex < cDataInfo.m_nTXTraceNumber; nTXTraceIndex++)
                    {
                        nValue_List.Add(nFrameData_Array[nTXTraceIndex, nRXTraceIndex]);

                        if (m_nPenEffectRXTrace > 0)
                        {
                            if (nRXTraceIndex + 1 < m_nPenEffectRXTrace - 1 || nRXTraceIndex + 1 > m_nPenEffectRXTrace + 1)
                                nNoPenEffectValue_List.Add(nFrameData_Array[nTXTraceIndex, nRXTraceIndex]);
                        }
                    }
                }

                double dMean = Math.Round(nValue_List.Average(), 3, MidpointRounding.AwayFromZero);
                double dStd = Math.Round(MathMethod.ComputeStd(nValue_List), 3, MidpointRounding.AwayFromZero);

                cDataInfo.m_cRXTraceReference.m_listdMean.Add(dMean);
                cDataInfo.m_cRXTraceReference.m_listdStd.Add(dStd);

                double dMeanPlus3Std = dMean + 3 * dStd;
                double dMeanMinus3Std = dMean - 3 * dStd;

                double dReference = Math.Round(Math.Sqrt(dMeanPlus3Std * dMeanPlus3Std + dMeanMinus3Std * dMeanMinus3Std), 3, MidpointRounding.AwayFromZero);

                cDataInfo.m_cRXTraceReference.m_listdReference.Add(dReference);
            }

            if (m_nPenEffectRXTrace > 0)
            {
                double dMean = Math.Round(nNoPenEffectValue_List.Average(), 3, MidpointRounding.AwayFromZero);
                double dStd = Math.Round(MathMethod.ComputeStd(nNoPenEffectValue_List), 3, MidpointRounding.AwayFromZero);

                cDataInfo.m_cRXTraceReference.m_dNoEffectMean = dMean;
                cDataInfo.m_cRXTraceReference.m_dNoEffectStd = dStd;

                double dMeanPlus3Std = dMean + 3 * dStd;
                double dMeanMinus3Std = dMean - 3 * dStd;

                double dReference = Math.Round(Math.Sqrt(dMeanPlus3Std * dMeanPlus3Std + dMeanMinus3Std * dMeanMinus3Std), 3, MidpointRounding.AwayFromZero);

                cDataInfo.m_cRXTraceReference.m_dNoEffectReference = dReference;
            }
        }
        */

        private void ComputeRXTraceReference(DataInfo cDataInfo, List<int[,]> nBASEMinusADCData_List)
        {
            List<int> nNoPenEffectValue_List = new List<int>();

            for (int nRXTraceIndex = 0; nRXTraceIndex < cDataInfo.m_nRXTraceNumber; nRXTraceIndex++)
            {
                List<int> nValue_List = new List<int>();

                foreach (int[,] nFrameData_Array in nBASEMinusADCData_List)
                {
                    for (int nTXTraceIndex = 0; nTXTraceIndex < cDataInfo.m_nTXTraceNumber; nTXTraceIndex++)
                    {
                        nValue_List.Add(nFrameData_Array[nTXTraceIndex, nRXTraceIndex]);

                        if (m_nPenEffectRXTrace > 0)
                        {
                            if (nRXTraceIndex + 1 < m_nPenEffectRXTrace - 1 || nRXTraceIndex + 1 > m_nPenEffectRXTrace + 1)
                                nNoPenEffectValue_List.Add(nFrameData_Array[nTXTraceIndex, nRXTraceIndex]);
                        }
                    }
                }

                double dMean = Math.Round(nValue_List.Average(), 3, MidpointRounding.AwayFromZero);
                double dStd = Math.Round(MathMethod.ComputeStd(nValue_List), 3, MidpointRounding.AwayFromZero);

                cDataInfo.m_cRXTraceReference.m_listdMean.Add(dMean);
                cDataInfo.m_cRXTraceReference.m_listdStd.Add(dStd);

                double dMeanPlus3Std = dMean + 3 * dStd;
                double dMeanMinus3Std = dMean - 3 * dStd;

                double dReference = Math.Round(Math.Sqrt(dMeanPlus3Std * dMeanPlus3Std + dMeanMinus3Std * dMeanMinus3Std), 3, MidpointRounding.AwayFromZero);

                cDataInfo.m_cRXTraceReference.m_listdReference.Add(dReference);
            }

            if (m_nPenEffectRXTrace > 0)
            {
                double dMean = Math.Round(nNoPenEffectValue_List.Average(), 3, MidpointRounding.AwayFromZero);
                double dStd = Math.Round(MathMethod.ComputeStd(nNoPenEffectValue_List), 3, MidpointRounding.AwayFromZero);

                cDataInfo.m_cRXTraceReference.m_dNoEffectMean = dMean;
                cDataInfo.m_cRXTraceReference.m_dNoEffectStd = dStd;

                double dMeanPlus3Std = dMean + 3 * dStd;
                double dMeanMinus3Std = dMean - 3 * dStd;

                double dReference = Math.Round(Math.Sqrt(dMeanPlus3Std * dMeanPlus3Std + dMeanMinus3Std * dMeanMinus3Std), 3, MidpointRounding.AwayFromZero);

                cDataInfo.m_cRXTraceReference.m_dNoEffectReference = dReference;
            }
        }

        private bool WriteRXTraceReference()
        {
            bool bErrorFlag = false;

            m_nCompareOperator = m_nCOMPARE_Frequency;
            m_cDataInfo_List.Sort(new DataInfoComparer());

            string sDirectoryName = MainConstantParameter.m_sDATATYPE_ANALYSIS;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            string sFilePath = string.Format(@"{0}\RXTraceReference.csv", sDirectoryPath);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("RXTraceReference");

                sw.Write("Frequency,,");

                for (int nRXTraceIndex = 0; nRXTraceIndex < m_nRXTraceNumber; nRXTraceIndex++)
                {
                    if (m_nPenEffectRXTrace > 0)
                        sw.Write(string.Format("{0},", (nRXTraceIndex + 1).ToString()));
                    else
                    {
                        if (nRXTraceIndex == m_nRXTraceNumber - 1)
                            sw.Write((nRXTraceIndex + 1).ToString());
                        else
                            sw.Write(string.Format("{0},", (nRXTraceIndex + 1).ToString()));
                    }
                }

                if (m_nPenEffectRXTrace > 0)
                    sw.WriteLine("NoEffect");

                foreach (DataInfo cDataInfo in m_cDataInfo_List)
                {
                    sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                    sw.Write(",");

                    for (int nRXTraceIndex = 0; nRXTraceIndex < m_nRXTraceNumber; nRXTraceIndex++)
                    {
                        if (m_nPenEffectRXTrace > 0)
                            sw.Write(string.Format("{0},", cDataInfo.m_cRXTraceReference.m_listdReference[nRXTraceIndex]));
                        else
                        {
                            if (nRXTraceIndex == m_nRXTraceNumber - 1)
                                sw.WriteLine(cDataInfo.m_cRXTraceReference.m_listdReference[nRXTraceIndex]);
                            else
                                sw.Write(string.Format("{0},", cDataInfo.m_cRXTraceReference.m_listdReference[nRXTraceIndex]));
                        }
                    }

                    if (m_nPenEffectRXTrace > 0)
                        sw.WriteLine(cDataInfo.m_cRXTraceReference.m_dNoEffectReference);
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = "Save RXTrace Reference Error";
                bErrorFlag = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bErrorFlag == true)
                return false;
            else
                return true;
        }

        private void WriteParameter(StreamWriter sw, string sParameterName, int nParameterValue, bool bInteger = false, int nHexNumber = -1, bool bNoneNegative = false)
        {
            if (bInteger == true)
                sw.WriteLine(string.Format("{0},{1}", sParameterName, nParameterValue.ToString("D")));
            else
            {
                if (bNoneNegative == true)
                {
                    if (nHexNumber == -1)
                    {
                        string sMessage = nParameterValue.ToString("x").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                    }
                    else if (nHexNumber == 2)
                    {
                        string sMessage = nParameterValue.ToString("x2").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 2)));
                    }
                    else if (nHexNumber == 4)
                    {
                        string sMessage = nParameterValue.ToString("x4").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 4)));
                    }
                    else
                    {
                        string sMessage = nParameterValue.ToString("x").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                    }
                }
                else
                {
                    if (nParameterValue >= 0)
                    {
                        if (nHexNumber == -1)
                        {
                            string sMessage = nParameterValue.ToString("x").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                        }
                        else if (nHexNumber == 2)
                        {
                            string sMessage = nParameterValue.ToString("x2").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 2)));
                        }
                        else if (nHexNumber == 4)
                        {
                            string sMessage = nParameterValue.ToString("x4").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 4)));
                        }
                        else
                        {
                            string sMessage = nParameterValue.ToString("x").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                        }
                    }
                    else
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, nParameterValue.ToString("D")));
                }
            }
        }

        private bool CheckUniformityValueCalculationError()
        {
            if (ParamFingerAutoTuning.m_nIgnoreAnomalyUniformity == 1)
                return true;

            if (m_bWarningOccurred)
            {
                m_sErrorMessage = "Uniformity Value Calculation Error";
                return false;
            }
            else
                return true;
        }
    }
}
