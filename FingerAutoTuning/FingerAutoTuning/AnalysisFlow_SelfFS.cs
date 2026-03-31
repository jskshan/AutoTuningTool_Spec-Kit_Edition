using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FingerAutoTuningParameter;
//using Elan;
using System.Drawing;

namespace FingerAutoTuning
{
    class AnalysisFlow_SelfFS : AnalysisFlow
    {
        private const int m_nPNModePeriodNumber = 8;

        private const string m_sDATATYPE_PNMODERAWMEAN = "PNModeRawMean";
        private const string m_sDATATYPE_PNMODERAWSTD = "PNModeRawStd";
        private const string m_sDATATYPE_ADCMEAN = "ADCMean";
        private const string m_sDATATYPE_ADCSTD = "ADCStd";
        private const string m_sDATATYPE_ADCMEDIAN = "ADCMedian";
        private const string m_sDATATYPE_ADCMAX = "ADCMax";
        private const string m_sDATATYPE_ADCMIN = "ADCMin";

        private const string m_sDATATYPE_PMODERAWMEAN = "PModeRawMean";
        private const string m_sDATATYPE_PMODERAWSTD = "PModeRawStd";
        private const string m_sDATATYPE_PMODERAWMAX = "PModeRawMax";
        private const string m_sDATATYPE_PMODERAWMIN = "PModeRawMin";
        private const string m_sDATATYPE_RAWPPERIODSTD = "RawPPeriodStd";
        private const string m_sDATATYPE_NMODERAWMEAN = "NModeRawMean";
        private const string m_sDATATYPE_NMODERAWSTD = "NModeRawStd";
        private const string m_sDATATYPE_NMODERAWMAX = "NModeRawMax";
        private const string m_sDATATYPE_NMODERAWMIN = "NModeRawMin";
        private const string m_sDATATYPE_RAWNPERIODSTD = "RawNPeriodStd";
        private const string m_sDATATYPE_RAWNPNMinusMean = "RawP-NMean";
        private const string m_sDATATYPE_ConvertADCMean = "ConvertADCMean";

        private const string m_sKVALUETYPE_KP = "KP Value";
        private const string m_sKVALUETYPE_KN = "KN Value";

        private const string m_sDATATYPE_NOISEREFERENCE = "NoiseReference";

        private string[] m_sDataType_Array = new string[]
        {
            m_sDATATYPE_PNMODERAWMEAN,
            m_sDATATYPE_PNMODERAWSTD,
            m_sDATATYPE_ADCMEAN,
            m_sDATATYPE_ADCSTD,
            m_sDATATYPE_PMODERAWMEAN,
            m_sDATATYPE_PMODERAWSTD,
            m_sDATATYPE_NMODERAWMEAN,
            m_sDATATYPE_NMODERAWSTD,
            m_sDATATYPE_PMODERAWMAX,
            m_sDATATYPE_PMODERAWMIN,
            m_sDATATYPE_NMODERAWMAX,
            m_sDATATYPE_NMODERAWMIN,
            m_sDATATYPE_RAWPPERIODSTD,
            m_sDATATYPE_RAWNPERIODSTD,
            m_sDATATYPE_RAWNPNMinusMean,
            m_sDATATYPE_ConvertADCMean
        };

        private string[] m_sKValueType_Array = new string[]
        {
            m_sKVALUETYPE_KP,
            m_sKVALUETYPE_KN
        };

        private int m_nTraceNumber_RX = 0;
        private int m_nTraceNumber_TX = 0;

        private enum KValueState
        {
            NA,
            GetKValue,
            NoKValue
        }

        private enum RawData
        {
            P,
            N,
            PNMinus,
            ConvertADC
        }

        private KValueState m_eGetKValue = KValueState.NA;
        private bool m_bRunCalibrationSequence = false;
        private bool m_bSetCAL = false;

        private bool m_bOutputChart = false;

        //0:Unsigned Value,  1:Signed Value,  2:Unsigned Value + Signed Value
        private int m_nComputeValueType = 0;

        private int m_nReportPartTraceLength = 20;

        public class DataInfo
        {
            public string m_sFileName = "";
            public int m_n_SELF_PH1 = 0;
            public int m_n_SELF_PH2E_LAT = 0;
            public int m_n_SELF_PH2E_LMT = 0;
            public int m_n_SELF_PH2_LAT = 0;
            public int m_n_SELF_PH2 = 0;
            public int m_nSelf_DFT_NUM = 0;
            public int m_nSelf_Gain = -1;
            public int m_nSelf_CAG = -1;
            public int m_nSelf_IQ_BSH = -1;
            public int m_nRXTraceNumber = 0;
            public int m_nTXTraceNumber = 0;
            public double m_dFrequency = 0.0;
            public bool m_bGetKValue = false;
            public int m_nRepeatIndex = 0;
            public int m_nNCPValue = -1;
            public int m_nNCNValue = -1;
            public int m_nCALValue = -1;

            public List<int[,]> m_nPNModeRawData_List = new List<int[,]>();
            public List<int[]> m_nADCData_List = new List<int[]>();
            public int[] m_nKPValue_Array = null;
            public int[] m_nKNValue_Array = null;

            public int[,] m_nRawPPeriodData_Array;
            public int[,] m_nRawNPeriodData_Array;

            public long[,] m_lRawPNMinusData_Array;
            public int[,] m_nConvertADCData_Array;

            public StatisticData m_cStatisticData = new StatisticData();

            public List<PNModePeriodInfo> m_cPNModePeriodInfo_List = new List<PNModePeriodInfo>();

            public List<int[]> m_nSignedReportData_List = new List<int[]>();
            public List<int[]> m_nUnsignedReportData_List = new List<int[]>();
            public ReportStatisticData m_cReportStatisticData = new ReportStatisticData();
        }

        public class InfoDataComparer : IComparer<DataInfo>
        {
            public int Compare(DataInfo cDataInfo1, DataInfo cDataInfo2)
            {
                if (m_nCompareOperator == m_nCOMPARE_Frequency)
                {
                    if (cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency) != 0)
                        return cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency);
                    else if (cDataInfo1.m_n_SELF_PH2.CompareTo(cDataInfo2.m_n_SELF_PH2) != 0)
                        return cDataInfo1.m_n_SELF_PH2.CompareTo(cDataInfo2.m_n_SELF_PH2);
                    else if (cDataInfo1.m_n_SELF_PH2E_LMT.CompareTo(cDataInfo2.m_n_SELF_PH2E_LMT) != 0)
                        return cDataInfo1.m_n_SELF_PH2E_LMT.CompareTo(cDataInfo2.m_n_SELF_PH2E_LMT);
                    else if (cDataInfo1.m_nSelf_DFT_NUM.CompareTo(cDataInfo2.m_nSelf_DFT_NUM) != 0)
                        return cDataInfo1.m_nSelf_DFT_NUM.CompareTo(cDataInfo2.m_nSelf_DFT_NUM);
                    else if (cDataInfo1.m_nRepeatIndex.CompareTo(cDataInfo2.m_nRepeatIndex) != 0)
                        return cDataInfo1.m_nRepeatIndex.CompareTo(cDataInfo2.m_nRepeatIndex);
                    else if (cDataInfo1.m_nNCPValue.CompareTo(cDataInfo2.m_nNCPValue) != 0)
                        return cDataInfo1.m_nNCPValue.CompareTo(cDataInfo2.m_nNCPValue);
                    else if (cDataInfo1.m_nNCNValue.CompareTo(cDataInfo2.m_nNCNValue) != 0)
                        return cDataInfo1.m_nNCNValue.CompareTo(cDataInfo2.m_nNCNValue);
                    else
                        return cDataInfo1.m_n_SELF_PH1.CompareTo(cDataInfo2.m_n_SELF_PH1);
                }
                else
                    return cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency);
            }
        }

        private List<DataInfo> m_cDataInfo_RX_List = null;
        private List<DataInfo> m_cDataInfo_TX_List = null;

        private List<DataInfo> m_cDataInfo_Signal_RX_List = null;
        private List<DataInfo> m_cDataInfo_Signal_TX_List = null;

        public class StatisticData
        {
            public List<double> m_dPNModeRawStd_List = new List<double>();
            public List<double> m_dPNModeRawMean_List = new List<double>();

            public List<double> m_dADCStd_List = new List<double>();
            public List<double> m_dADCMean_List = new List<double>();

            public List<double> m_dPModeRawStd_List = new List<double>();
            public List<double> m_dPModeRawMean_List = new List<double>();
            public List<int> m_nPModeRawMax_List = new List<int>();
            public List<int> m_nPModeRawMin_List = new List<int>();

            public List<double> m_dNModeRawStd_List = new List<double>();
            public List<double> m_dNModeRawMean_List = new List<double>();
            public List<int> m_nNModeRawMax_List = new List<int>();
            public List<int> m_nNModeRawMin_List = new List<int>();

            public List<double> m_dRawPPeriodStd_List = new List<double>();
            public List<double> m_dRawNPeriodStd_List = new List<double>();

            public List<double> m_dRawPNMinusMean_List = new List<double>();
            public List<double> m_dConvertADCMean_List = new List<double>();
        }

        public class ReportStatisticData
        {
            public List<double> m_dStd_Signed_List = new List<double>();
            public List<double> m_dMean_Signed_List = new List<double>();
            public List<double> m_dMedian_Signed_List = new List<double>();
            public List<int> m_nMax_Signed_List = new List<int>();
            public List<int> m_nMin_Signed_List = new List<int>();
            public double m_dMaxNoise_Signed = 0.0;
            public double m_dMeanNoise_Signed = 0.0;

            public List<double> m_dStd_Unsigned_List = new List<double>();
            public List<double> m_dMean_Unsigned_List = new List<double>();
            public List<double> m_dMedian_Unsigned_List = new List<double>();
            public List<int> m_nMax_Unsigned_List = new List<int>();
            public List<int> m_nMin_Unsigned_List = new List<int>();
            public double m_dMaxNoise_Unsigned = 0.0;
            public double m_dMeanNoise_Unsigned = 0.0;
        }

        public class PNModePeriodInfo
        {
            public double[] m_dMean_Array = new double[m_nPNModePeriodNumber];
            public double[] m_dStd_Array = new double[m_nPNModePeriodNumber];
        }

        private class ChartColor
        {
            public List<int> m_nColorR_List = new List<int>();
            public List<int> m_nColorG_List = new List<int>();
            public List<int> m_nColorB_List = new List<int>();
        }

        public AnalysisFlow_SelfFS(frmMain.FlowStep cFlowStep, string sLogDirectoryPath, string sH5LogDirectoryPath, bool bGenerateH5Data, frmMain cfrmParent, string sProjectName)
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
            if (ParamFingerAutoTuning.m_nSelfFSReportSignedValue == 2)
                m_nComputeValueType = 2;
            else if (ParamFingerAutoTuning.m_nSelfFSReportSignedValue == 1)
                m_nComputeValueType = 1;
            else
                m_nComputeValueType = 0;
        }

        public override void InitializeSourceDataList()
        {
            if (ParamFingerAutoTuning.m_nSelfFSGetDataType != 1)
                m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_DV);
            else
            {
                m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_REPORTMODE);

                if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1)
                    m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL);
            }
        }

        public override bool MainFlow(ref string sErrorMessage)
        {
            if (GetDataCount() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (ParamFingerAutoTuning.m_nSelfFSGetDataType != 1)
            {
                if (ReaddVData() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }

                if (SavePNModeRawData() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }

                if (AnalysisData() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }

                if (SaveRawPNPeriodData() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }

                if (SaveAnalysis() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }

                if (SavePNModePeriodAnalysis() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }

                if (m_bOutputChart == true)
                {
                    if (SaveLineChart() == false)
                    {
                        SetErrorMessage(ref sErrorMessage);
                        return false;
                    }
                }
            }
            else
            {
                if (ReadReportData(MainConstantParameter.m_sDATATYPE_REPORTMODE) == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }

                if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1)
                {
                    if (ReadReportData(MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL) == false)
                    {
                        SetErrorMessage(ref sErrorMessage);
                        return false;
                    }
                }

                if (SaveReportData() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }

                if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1)
                {
                    if (SaveReportData(true) == false)
                    {
                        SetErrorMessage(ref sErrorMessage);
                        return false;
                    }
                }

                if (AnalysisReportData() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }

                if (SaveReportStatisticFile() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }

                if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1)
                {
                    if (SaveReportStatisticFile(true) == false)
                    {
                        SetErrorMessage(ref sErrorMessage);
                        return false;
                    }
                }

                if (m_bOutputChart == true)
                {
                    if (SaveReportLineChart() == false)
                    {
                        SetErrorMessage(ref sErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ReaddVData()
        {
            string sDataPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_DV);

            m_nAnalysisCount = m_nTotalFileCount + 1;

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.InitialstatusstripMessage(m_nAnalysisCount, "Data Analysis...");
            });

            foreach (string sFilePath in Directory.EnumerateFiles(sDataPath, "*.csv", SearchOption.TopDirectoryOnly))
            {
                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);
                Self_FileCheckInfo cSelf_FileCheckInfo = new Self_FileCheckInfo();
                List<int[,]> nPNModeRawData_List = new List<int[,]>();
                List<int[]> nADCData_List = new List<int[]>();

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                if (CheckFileInfo(ref cSelf_FileCheckInfo, srFile, sFileName) == false)
                    return false;

                if (CheckFileInfoIdentical(cSelf_FileCheckInfo, sFileName, MainConstantParameter.m_sDATATYPE_DV) == false)
                    return false;

                int nSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LAT, cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT,
                                                                    cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT, cSelf_FileCheckInfo.m_nRead_SELF_PH2);
                double dFrequncy = ElanConvert.Convert2Frequency(cSelf_FileCheckInfo.m_nRead_SELF_PH1, nSelfPH2Sum);

                DataInfo cDataInfo = new DataInfo();
                cDataInfo.m_sFileName = sFileName;
                cDataInfo.m_dFrequency = dFrequncy;
                cDataInfo.m_n_SELF_PH1 = cSelf_FileCheckInfo.m_nRead_SELF_PH1;
                cDataInfo.m_n_SELF_PH2E_LMT = cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT;
                cDataInfo.m_n_SELF_PH2_LAT = cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT;
                cDataInfo.m_n_SELF_PH2 = cSelf_FileCheckInfo.m_nRead_SELF_PH2;
                cDataInfo.m_nSelf_DFT_NUM = cSelf_FileCheckInfo.m_nReadSelf_DFT_NUM;
                cDataInfo.m_nSelf_Gain = cSelf_FileCheckInfo.m_nReadSelf_Gain;
                cDataInfo.m_nSelf_CAG = cSelf_FileCheckInfo.m_nReadSelf_CAG;
                cDataInfo.m_nSelf_IQ_BSH = cSelf_FileCheckInfo.m_nReadSelf_IQ_BSH;
                cDataInfo.m_nRXTraceNumber = cSelf_FileCheckInfo.m_nRXTraceNumber;
                cDataInfo.m_nTXTraceNumber = cSelf_FileCheckInfo.m_nTXTraceNumber;
                cDataInfo.m_bGetKValue = cSelf_FileCheckInfo.m_bGetKValue;
                cDataInfo.m_nNCPValue = cSelf_FileCheckInfo.m_nNCPValue;
                cDataInfo.m_nNCNValue = cSelf_FileCheckInfo.m_nNCNValue;
                cDataInfo.m_nCALValue = cSelf_FileCheckInfo.m_nCALValue;

                int nListIndex = 0;

                if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                {
                    if (m_cDataInfo_RX_List == null)
                        m_cDataInfo_RX_List = new List<DataInfo>();

                    m_cDataInfo_RX_List.Add(cDataInfo);
                    nListIndex = m_cDataInfo_RX_List.Count - 1;
                }
                else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                {
                    if (m_cDataInfo_TX_List == null)
                        m_cDataInfo_TX_List = new List<DataInfo>();

                    m_cDataInfo_TX_List.Add(cDataInfo);
                    nListIndex = m_cDataInfo_TX_List.Count - 1;
                }

                int[] nKPValue_Array = new int[cSelf_FileCheckInfo.m_nRXTraceNumber];
                int[] nKNValue_Array = new int[cSelf_FileCheckInfo.m_nRXTraceNumber];

                srFile = new StreamReader(sFilePath, Encoding.Default);

                if (GetFrameData(ref nPNModeRawData_List, ref nADCData_List, ref nKPValue_Array, ref nKNValue_Array, cSelf_FileCheckInfo, srFile, sFileName) == false)
                    return false;

                if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                {
                    m_cDataInfo_RX_List[nListIndex].m_nPNModeRawData_List = nPNModeRawData_List;
                    m_cDataInfo_RX_List[nListIndex].m_nADCData_List = nADCData_List;
                    m_cDataInfo_RX_List[nListIndex].m_nKPValue_Array = nKPValue_Array;
                    m_cDataInfo_RX_List[nListIndex].m_nKNValue_Array = nKNValue_Array;
                }
                else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                {
                    m_cDataInfo_TX_List[nListIndex].m_nPNModeRawData_List = nPNModeRawData_List;
                    m_cDataInfo_TX_List[nListIndex].m_nADCData_List = nADCData_List;
                    m_cDataInfo_TX_List[nListIndex].m_nKPValue_Array = nKPValue_Array;
                    m_cDataInfo_TX_List[nListIndex].m_nKNValue_Array = nKNValue_Array;
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

        private bool ReadReportData(string sDataType)
        {
            string sDataPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDataType);

            m_nAnalysisCount = m_nTotalFileCount + 1;

            if (sDataType != MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL)
            {
                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.InitialstatusstripMessage(m_nAnalysisCount, "Data Analysis...");
                });
            }

            foreach (string sFilePath in Directory.EnumerateFiles(sDataPath, "*.txt", SearchOption.TopDirectoryOnly))
            {
                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);
                Self_FileCheckInfo cSelf_FileCheckInfo = new Self_FileCheckInfo();
                List<int[]> nSignedReportData_List = new List<int[]>();
                List<int[]> nUnsignedReportData_List = new List<int[]>();

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                if (CheckFileInfo(ref cSelf_FileCheckInfo, srFile, sFileName, false) == false)
                    return false;

                if (CheckFileInfoIdentical(cSelf_FileCheckInfo, sFileName, sDataType) == false)
                    return false;

                int nSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LAT, cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT,
                                                                    cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT, cSelf_FileCheckInfo.m_nRead_SELF_PH2);
                double dFrequncy = ElanConvert.Convert2Frequency(cSelf_FileCheckInfo.m_nRead_SELF_PH1, nSelfPH2Sum);

                DataInfo cDataInfo = new DataInfo();
                cDataInfo.m_sFileName = sFileName;
                cDataInfo.m_dFrequency = dFrequncy;
                cDataInfo.m_n_SELF_PH1 = cSelf_FileCheckInfo.m_nRead_SELF_PH1;
                cDataInfo.m_n_SELF_PH2E_LMT = cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT;
                cDataInfo.m_n_SELF_PH2_LAT = cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT;
                cDataInfo.m_n_SELF_PH2 = cSelf_FileCheckInfo.m_nRead_SELF_PH2;
                cDataInfo.m_nSelf_DFT_NUM = cSelf_FileCheckInfo.m_nReadSelf_DFT_NUM;
                cDataInfo.m_nSelf_Gain = cSelf_FileCheckInfo.m_nReadSelf_Gain;
                cDataInfo.m_nSelf_CAG = cSelf_FileCheckInfo.m_nReadSelf_CAG;
                cDataInfo.m_nSelf_IQ_BSH = cSelf_FileCheckInfo.m_nReadSelf_IQ_BSH;
                cDataInfo.m_nRXTraceNumber = cSelf_FileCheckInfo.m_nRXTraceNumber;
                cDataInfo.m_nTXTraceNumber = cSelf_FileCheckInfo.m_nTXTraceNumber;
                cDataInfo.m_bGetKValue = cSelf_FileCheckInfo.m_bGetKValue;
                cDataInfo.m_nRepeatIndex = cSelf_FileCheckInfo.m_nRepeatIndex;
                cDataInfo.m_nNCPValue = cSelf_FileCheckInfo.m_nNCPValue;
                cDataInfo.m_nNCNValue = cSelf_FileCheckInfo.m_nNCNValue;
                cDataInfo.m_nCALValue = cSelf_FileCheckInfo.m_nCALValue;

                int nListIndex = 0;

                if (sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL)
                {
                    if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                    {
                        if (m_cDataInfo_Signal_RX_List == null)
                            m_cDataInfo_Signal_RX_List = new List<DataInfo>();

                        m_cDataInfo_Signal_RX_List.Add(cDataInfo);
                        nListIndex = m_cDataInfo_Signal_RX_List.Count - 1;
                    }
                    else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                    {
                        if (m_cDataInfo_Signal_TX_List == null)
                            m_cDataInfo_Signal_TX_List = new List<DataInfo>();

                        m_cDataInfo_Signal_TX_List.Add(cDataInfo);
                        nListIndex = m_cDataInfo_Signal_TX_List.Count - 1;
                    }
                }
                else
                {
                    if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                    {
                        if (m_cDataInfo_RX_List == null)
                            m_cDataInfo_RX_List = new List<DataInfo>();

                        m_cDataInfo_RX_List.Add(cDataInfo);
                        nListIndex = m_cDataInfo_RX_List.Count - 1;
                    }
                    else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                    {
                        if (m_cDataInfo_TX_List == null)
                            m_cDataInfo_TX_List = new List<DataInfo>();

                        m_cDataInfo_TX_List.Add(cDataInfo);
                        nListIndex = m_cDataInfo_TX_List.Count - 1;
                    }
                }

                srFile = new StreamReader(sFilePath, Encoding.Default);

                if (GetReportData(ref nSignedReportData_List, ref nUnsignedReportData_List, cSelf_FileCheckInfo, srFile, sFileName) == false)
                    return false;

                if (sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL)
                {
                    if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                    {
                        m_cDataInfo_Signal_RX_List[nListIndex].m_nSignedReportData_List = nSignedReportData_List;
                        m_cDataInfo_Signal_RX_List[nListIndex].m_nUnsignedReportData_List = nUnsignedReportData_List;
                    }
                    else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                    {
                        m_cDataInfo_Signal_TX_List[nListIndex].m_nSignedReportData_List = nSignedReportData_List;
                        m_cDataInfo_Signal_TX_List[nListIndex].m_nUnsignedReportData_List = nUnsignedReportData_List;
                    }
                }
                else
                {
                    if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                    {
                        m_cDataInfo_RX_List[nListIndex].m_nSignedReportData_List = nSignedReportData_List;
                        m_cDataInfo_RX_List[nListIndex].m_nUnsignedReportData_List = nUnsignedReportData_List;
                    }
                    else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                    {
                        m_cDataInfo_TX_List[nListIndex].m_nSignedReportData_List = nSignedReportData_List;
                        m_cDataInfo_TX_List[nListIndex].m_nUnsignedReportData_List = nUnsignedReportData_List;
                    }
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

        private bool CheckFileInfo(ref Self_FileCheckInfo cSelf_FileCheckInfo, StreamReader srFile, string sFileName, bool bCsvFile = true)
        {
            string sLine = "";
            bool bEndInfo = false;

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSubString_Array = null;

                    if (bCsvFile == true)
                        sSubString_Array = sLine.Split(',');
                    else
                    {
                        sSubString_Array = sLine.Split('=');

                        for (int nSplitIndex = 0; nSplitIndex < sSubString_Array.Length; nSplitIndex++)
                            sSubString_Array[nSplitIndex] = sSubString_Array[nSplitIndex].Trim();
                    }

                    if (sSubString_Array.Length >= 2)
                    {
                        if (sSubString_Array[0] == "TXTraceNumber")
                            Int32.TryParse(sSubString_Array[1], out cSelf_FileCheckInfo.m_nTXTraceNumber);
                        else if (sSubString_Array[0] == "RXTraceNumber")
                            Int32.TryParse(sSubString_Array[1], out cSelf_FileCheckInfo.m_nRXTraceNumber);
                        else if (sSubString_Array[0] == "Set_SELF_PH1(Hex)")
                            cSelf_FileCheckInfo.m_nSet_SELF_PH1 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Set_SELF_PH2E_LAT(Hex)")
                            cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LAT = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Set_SELF_PH2E_LMT(Hex)")
                            cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LMT = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Set_SELF_PH2_LAT(Hex)")
                            cSelf_FileCheckInfo.m_nSet_SELF_PH2_LAT = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Set_SELF_PH2(Hex)")
                            cSelf_FileCheckInfo.m_nSet_SELF_PH2 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "SetSelf_Sum(Hex)" || sSubString_Array[0] == "SetSelf_DFT_NUM(Hex)")
                            cSelf_FileCheckInfo.m_nSetSelf_DFT_NUM = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "SetSelf_Gain(Hex)")
                            cSelf_FileCheckInfo.m_nSetSelf_Gain = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_SELF_PH1(Hex)")
                            cSelf_FileCheckInfo.m_nRead_SELF_PH1 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_SELF_PH2E_LAT(Hex)")
                            cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LAT = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_SELF_PH2E_LMT(Hex)")
                            cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_SELF_PH2_LAT(Hex)")
                            cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_SELF_PH2(Hex)")
                            cSelf_FileCheckInfo.m_nRead_SELF_PH2 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "ReadSelf_Sum(Hex)" || sSubString_Array[0] == "ReadSelf_DFT_NUM(Hex)")
                            cSelf_FileCheckInfo.m_nReadSelf_DFT_NUM = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "ReadSelf_Gain(Hex)")
                            cSelf_FileCheckInfo.m_nReadSelf_Gain = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "ReadSelf_CAG(Hex)")
                            cSelf_FileCheckInfo.m_nReadSelf_CAG = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "ReadSelf_IQ_BSH(Hex)")
                            cSelf_FileCheckInfo.m_nReadSelf_IQ_BSH = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "TraceType")
                        {
                            if (sSubString_Array[1] == "RX")
                                cSelf_FileCheckInfo.m_eTraceType = TraceType.RX;
                            else if (sSubString_Array[1] == "TX")
                                cSelf_FileCheckInfo.m_eTraceType = TraceType.TX;
                        }
                        else if (sSubString_Array[0] == "GetSelfKValue")
                        {
                            if (sSubString_Array[1] == "1")
                                cSelf_FileCheckInfo.m_bGetKValue = true;
                            else
                                cSelf_FileCheckInfo.m_bGetKValue = false;
                        }
                        else if (sSubString_Array[0] == "RepeatIndex")
                            cSelf_FileCheckInfo.m_nRepeatIndex = Convert.ToInt32(sSubString_Array[1]);
                        else if (sSubString_Array[0] == "SelfNCPValue")
                            cSelf_FileCheckInfo.m_nNCPValue = Convert.ToInt32(sSubString_Array[1]);
                        else if (sSubString_Array[0] == "SelfNCNValue")
                            cSelf_FileCheckInfo.m_nNCNValue = Convert.ToInt32(sSubString_Array[1]);
                        else if (sSubString_Array[0] == "SelfCAL")
                        {
                            cSelf_FileCheckInfo.m_nCALValue = Convert.ToInt32(sSubString_Array[1]);
                            bEndInfo = true;
                        }
                    }

                    /*
                    if (cSelf_FileCheckInfo.m_nSet_SELF_PH1 > -1 && cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LAT > -1 && cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LMT > -1 && 
                        cSelf_FileCheckInfo.m_nSet_SELF_PH2_LAT > -1 && cSelf_FileCheckInfo.m_nRead_SELF_PH2 > -1 &&
                        cSelf_FileCheckInfo.m_nRead_SELF_PH1 > -1 && cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LAT > -1 && cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT > -1 && 
                        cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT > -1 && cSelf_FileCheckInfo.m_nRead_SELF_PH2 > -1 &&
                        cSelf_FileCheckInfo.m_nSetSelf_DFT_NUM > -1 && cSelf_FileCheckInfo.m_nReadSelf_DFT_NUM > -1 &&
                        cSelf_FileCheckInfo.m_nTXTraceNumber > -1 && cSelf_FileCheckInfo.m_nRXTraceNumber > -1 &&
                        cSelf_FileCheckInfo.m_eTraceType != TraceType.ALL)
                        break;
                    */

                    if (bEndInfo == true)
                        break;
                }
            }
            finally
            {
                srFile.Close();
            }

            if (cSelf_FileCheckInfo.m_nSet_SELF_PH1 == -1 || cSelf_FileCheckInfo.m_nRead_SELF_PH1 == -1 ||
                cSelf_FileCheckInfo.m_nSet_SELF_PH1 != cSelf_FileCheckInfo.m_nRead_SELF_PH1)
            {
                m_sErrorMessage = string.Format("Read _SELF_PH1 Error in {0}[Set:0x{1} Read:0x{2}]", sFileName,
                                                cSelf_FileCheckInfo.m_nSet_SELF_PH1.ToString("x2").ToUpper(),
                                                cSelf_FileCheckInfo.m_nRead_SELF_PH1.ToString("x2").ToUpper());
                return false;
            }

            if (cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LAT == -1 || cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LAT == -1 ||
                cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LAT != cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LAT)
            {
                m_sErrorMessage = string.Format("Read _SELF_PH2E_LAT Error in {0}[Set:0x{1} Read:0x{2}]", sFileName,
                                                cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LAT.ToString("x2").ToUpper(),
                                                cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LAT.ToString("x2").ToUpper());
                return false;
            }

            if (cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LMT == -1 || cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT == -1 ||
                cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LMT != cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT)
            {
                m_sErrorMessage = string.Format("Read _SELF_PH2E_LMT Error in {0}[Set:0x{1} Read:0x{2}]", sFileName,
                                                cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LMT.ToString("x2").ToUpper(),
                                                cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT.ToString("x2").ToUpper());
                return false;
            }

            if (cSelf_FileCheckInfo.m_nSet_SELF_PH2_LAT == -1 || cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT == -1 ||
                cSelf_FileCheckInfo.m_nSet_SELF_PH2_LAT != cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT)
            {
                m_sErrorMessage = string.Format("Read _SELF_PH2_LAT Error in {0}[Set:0x{1} Read:0x{2}]", sFileName,
                                                cSelf_FileCheckInfo.m_nSet_SELF_PH2_LAT.ToString("x2").ToUpper(),
                                                cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT.ToString("x2").ToUpper());
                return false;
            }

            if (cSelf_FileCheckInfo.m_nSet_SELF_PH2 == -1 || cSelf_FileCheckInfo.m_nRead_SELF_PH2 == -1 ||
               cSelf_FileCheckInfo.m_nSet_SELF_PH2 != cSelf_FileCheckInfo.m_nRead_SELF_PH2)
            {
                m_sErrorMessage = string.Format("Read _SELF_PH2 Error in {0}[Set:0x{1} Read:0x{2}]", sFileName,
                                                cSelf_FileCheckInfo.m_nSet_SELF_PH2.ToString("x2").ToUpper(),
                                                cSelf_FileCheckInfo.m_nRead_SELF_PH2.ToString("x2").ToUpper());
                return false;
            }

            if (cSelf_FileCheckInfo.m_nTXTraceNumber == -1)
            {
                m_sErrorMessage = string.Format("Read TXTraceNumber Error in {0}[Number={1}]", sFileName, cSelf_FileCheckInfo.m_nTXTraceNumber);
                return false;
            }

            if (cSelf_FileCheckInfo.m_nRXTraceNumber == -1)
            {
                m_sErrorMessage = string.Format("Read RXTraceNumber Error in {0}[Number={1}]", sFileName, cSelf_FileCheckInfo.m_nRXTraceNumber);
                return false;
            }

            if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX && m_nTraceNumber_RX == 0)
                m_nTraceNumber_RX = cSelf_FileCheckInfo.m_nRXTraceNumber;
            else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX && m_nTraceNumber_TX == 0)
                m_nTraceNumber_TX = cSelf_FileCheckInfo.m_nRXTraceNumber;

            if (m_eGetKValue == KValueState.NA)
            {
                if (cSelf_FileCheckInfo.m_bGetKValue == true)
                    m_eGetKValue = KValueState.GetKValue;
                else
                    m_eGetKValue = KValueState.NoKValue;
            }

            if (cSelf_FileCheckInfo.m_nNCPValue >= 0)
                m_bRunCalibrationSequence = true;

            if (cSelf_FileCheckInfo.m_nNCNValue >= 0)
                m_bRunCalibrationSequence = true;

            if (cSelf_FileCheckInfo.m_nCALValue >= 0)
                m_bSetCAL = true;

            return true;
        }

        private bool CheckFileInfoIdentical(Self_FileCheckInfo cSelf_FileCheckInfo, string sFileName, string sDataType)
        {
            List<DataInfo> cDataInfo_List = null;

            if (sDataType == MainConstantParameter.m_sDATATYPE_DV ||
                sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE)
            {
                if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                    cDataInfo_List = m_cDataInfo_RX_List;
                else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                    cDataInfo_List = m_cDataInfo_TX_List;
            }
            else if (sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL)
            {
                if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                    cDataInfo_List = m_cDataInfo_Signal_RX_List;
                else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                    cDataInfo_List = m_cDataInfo_Signal_TX_List;
            }

            if (cDataInfo_List == null)
                return true;

            for (int nDataIndex = 0; nDataIndex < cDataInfo_List.Count; nDataIndex++)
            {
                string sMatchFileName = "";

                switch (sDataType)
                {
                    case MainConstantParameter.m_sDATATYPE_DV:
                    case MainConstantParameter.m_sDATATYPE_REPORTMODE:
                    case MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL:
                        sMatchFileName = cDataInfo_List[nDataIndex].m_sFileName;
                        break;
                    default:
                        break;
                }

                int nSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LAT, cSelf_FileCheckInfo.m_nSet_SELF_PH2E_LMT,
                                                                    cSelf_FileCheckInfo.m_nSet_SELF_PH2_LAT, cSelf_FileCheckInfo.m_nSet_SELF_PH2);
                int nSelfPH1PH2Sum = cSelf_FileCheckInfo.m_nSet_SELF_PH1 + nSelfPH2Sum;

                int nCompareSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cDataInfo_List[nDataIndex].m_n_SELF_PH2E_LAT, cDataInfo_List[nDataIndex].m_n_SELF_PH2E_LMT,
                                                                           cDataInfo_List[nDataIndex].m_n_SELF_PH2E_LAT, cDataInfo_List[nDataIndex].m_n_SELF_PH2);
                int nCompareSelfPH1PH2Sum = cDataInfo_List[nDataIndex].m_n_SELF_PH1 + nCompareSelfPH2Sum;

                int nSelfDFT_NUM = cSelf_FileCheckInfo.m_nSetSelf_DFT_NUM;
                int nCompareSelfDFT_NUM = cDataInfo_List[nDataIndex].m_nSelf_DFT_NUM;

                if (cSelf_FileCheckInfo.m_nSet_SELF_PH1 == cDataInfo_List[nDataIndex].m_n_SELF_PH1 &&
                    cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT == cDataInfo_List[nDataIndex].m_n_SELF_PH2E_LMT &&
                    nSelfPH2Sum == nCompareSelfPH2Sum &&
                    nSelfDFT_NUM == nCompareSelfDFT_NUM &&
                    cSelf_FileCheckInfo.m_nNCPValue == cDataInfo_List[nDataIndex].m_nNCPValue &&
                    cSelf_FileCheckInfo.m_nNCNValue == cDataInfo_List[nDataIndex].m_nNCNValue)
                {
                    m_sErrorMessage = string.Format("Self FW Parameter Unique Check Error in {0} and {1}", sFileName, sMatchFileName);
                    return false;
                }

                /*
                if (cDataInfo_List[nDataIndex].m_nTXTraceNumber != cSelf_FileCheckInfo.m_nTXTraceNumber)
                {
                    m_sErrorMessage = string.Format("TXTraceNumber Identical Check Error in {0} and {1}[Number1={2} Number2={3}]", sFileName,
                                                    sMatchFileName,
                                                    cSelf_FileCheckInfo.m_nTXTraceNumber,
                                                    cDataInfo_List[nDataIndex].m_nTXTraceNumber);
                    return false;
                }
                */

                if (cDataInfo_List[nDataIndex].m_nRXTraceNumber != cSelf_FileCheckInfo.m_nRXTraceNumber)
                {
                    m_sErrorMessage = string.Format("RXTraceNumber Identical Check Error in {0} and {1}[Number1={2} Number2={3}]", sFileName,
                                                    sMatchFileName,
                                                    cSelf_FileCheckInfo.m_nRXTraceNumber,
                                                    cDataInfo_List[nDataIndex].m_nRXTraceNumber);
                    return false;
                }

                if (cDataInfo_List[nDataIndex].m_bGetKValue != cSelf_FileCheckInfo.m_bGetKValue)
                {
                    m_sErrorMessage = string.Format("Get KValue Identical Check Error in {0} and {1}[Number1={2} Number2={3}]", sFileName,
                                                    sMatchFileName,
                                                    cSelf_FileCheckInfo.m_bGetKValue.ToString(),
                                                    cDataInfo_List[nDataIndex].m_bGetKValue.ToString());
                    return false;
                }
            }

            return true;
        }

        private bool GetFrameData(ref List<int[,]> nPNModeRawData_List, ref List<int[]> nADCData_List, ref int[] nKPValue_Array, ref int[] nKNValue_Array,
                                  Self_FileCheckInfo cSelf_FileCheckInfo, StreamReader srFile, string sFileName)
        {
            bool bGetFrameData = false;
            bool bGetKValue = false;
            int[,] nSinglePNModeRawData_Array = null;
            int[] nSingleADCData_Array = null;
            int nTXCount = 0;
            string sLine = "";

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSplit_Array = sLine.Split(',');

                    if (sSplit_Array.Length >= 2)
                    {
                        if (sSplit_Array[0] == "Frame")
                        {
                            bGetFrameData = true;
                            nTXCount = 0;
                            nSinglePNModeRawData_Array = new int[cSelf_FileCheckInfo.m_nSetSelf_DFT_NUM, cSelf_FileCheckInfo.m_nRXTraceNumber];
                            nSingleADCData_Array = new int[cSelf_FileCheckInfo.m_nRXTraceNumber];
                            continue;
                        }
                    }

                    if (bGetFrameData == true)
                    {
                        if (nTXCount == cSelf_FileCheckInfo.m_nTXTraceNumber)
                        {
                            if (sSplit_Array.Length >= cSelf_FileCheckInfo.m_nRXTraceNumber)
                            {
                                for (int nRXIndex = 0; nRXIndex < cSelf_FileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                                    nSingleADCData_Array[nRXIndex] = Convert.ToInt32(sSplit_Array[nRXIndex]);
                            }

                            nADCData_List.Add(nSingleADCData_Array);
                            bGetFrameData = false;
                        }
                        else
                        {
                            if (cSelf_FileCheckInfo.m_bGetKValue == true && nTXCount <= 1)
                            {
                                if (bGetKValue == false)
                                {
                                    if (nTXCount == 0)
                                    {
                                        if (sSplit_Array.Length >= cSelf_FileCheckInfo.m_nRXTraceNumber)
                                        {
                                            for (int nRXIndex = 0; nRXIndex < cSelf_FileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                                                nKNValue_Array[nRXIndex] = Convert.ToInt32(sSplit_Array[nRXIndex]);

                                            nTXCount++;
                                        }
                                    }
                                    else if (nTXCount == 1)
                                    {
                                        if (sSplit_Array.Length >= cSelf_FileCheckInfo.m_nRXTraceNumber)
                                        {
                                            for (int nRXIndex = 0; nRXIndex < cSelf_FileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                                                nKPValue_Array[nRXIndex] = Convert.ToInt32(sSplit_Array[nRXIndex]);

                                            nTXCount++;
                                        }

                                        bGetKValue = true;
                                    }
                                }
                                else
                                {
                                    nTXCount++;
                                    continue;
                                }
                            }
                            else
                            {
                                int nTXIndex = nTXCount;

                                if (cSelf_FileCheckInfo.m_bGetKValue == true)
                                    nTXIndex -= 2;

                                if (sSplit_Array.Length >= cSelf_FileCheckInfo.m_nRXTraceNumber)
                                {
                                    for (int nRXIndex = 0; nRXIndex < cSelf_FileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                                        nSinglePNModeRawData_Array[nTXIndex, nRXIndex] = Convert.ToInt32(sSplit_Array[nRXIndex]);

                                    nTXCount++;
                                }

                                if (nTXCount == cSelf_FileCheckInfo.m_nTXTraceNumber)
                                {
                                    nPNModeRawData_List.Add(nSinglePNModeRawData_Array);
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

            if (nPNModeRawData_List == null || nPNModeRawData_List.Count == 0)
            {
                m_sErrorMessage = string.Format("Read Self PN Mode Raw Data Error in {0}[Count:{1}]", sFileName, nPNModeRawData_List.Count);
                return false;
            }

            if (nADCData_List == null || nADCData_List.Count == 0)
            {
                m_sErrorMessage = string.Format("Read Self ADC Data Error in {0}[Count:{1}]", sFileName, nADCData_List.Count);
                return false;
            }

            return true;
        }

        private bool GetReportData(ref List<int[]> nSignedReportData_List, ref List<int[]> nUnsignedReportData_List, Self_FileCheckInfo cSelf_FileCheckInfo,
                                   StreamReader srFile, string sFileName)
        {
            int nTraceNumber = 0;

            if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                nTraceNumber = cSelf_FileCheckInfo.m_nRXTraceNumber;
            else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                nTraceNumber = cSelf_FileCheckInfo.m_nTXTraceNumber;

            int nTraceDataCount = nTraceNumber;

            if (ParamFingerAutoTuning.m_nSelfFSGetReportType == 2)
            {
                if (nTraceNumber >= 20)
                    nTraceDataCount = 20;
            }

            if (ParamFingerAutoTuning.m_nSelfFSGetReportType != 2)
            {
                nTraceDataCount = nTraceNumber / 2;

                if (nTraceNumber % 2 != 0)
                    nTraceDataCount++;
            }

            if (ParamFingerAutoTuning.m_nSelfFSGetReportType != 1)
            {
                string sLine = "";

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplit_Array = sLine.Split(' ');

                        if (sSplit_Array.Length >= nTraceDataCount * 2 + 2)
                        {
                            byte[] byteData_Array = new byte[nTraceDataCount * 2 + 2];

                            for (int nValueIndex = 0; nValueIndex < byteData_Array.Length; nValueIndex++)
                                byteData_Array[nValueIndex] = Convert.ToByte(sSplit_Array[nValueIndex], 16);

                            if (byteData_Array[0] != 0x01)
                                continue;

                            int[] nSignedReportData_Array = new int[nTraceDataCount];
                            int[] nUnsignedReportData_Array = new int[nTraceDataCount];

                            for (int nDataIndex = 0; nDataIndex < nTraceDataCount; nDataIndex++)
                            {
                                int nValue = byteData_Array[nDataIndex * 2 + 2] + byteData_Array[nDataIndex * 2 + 2 + 1] * 256;
                                nUnsignedReportData_Array[nDataIndex] = nValue;

                                if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                                {
                                    if (nValue >= 32767)
                                        nValue = nValue - 65536;

                                    nSignedReportData_Array[nDataIndex] = nValue;
                                }
                            }

                            if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                                nSignedReportData_List.Add(nSignedReportData_Array);

                            nUnsignedReportData_List.Add(nUnsignedReportData_Array);
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }
            }
            else
            {
                string sLine = "";
                string sTracePart = "All";
                bool bOddEvenType = true;
                List<int[]> nSignedReportData_First_List = new List<int[]>();
                List<int[]> nSignedReportData_Second_List = new List<int[]>();
                List<int[]> nUnsignedReportData_First_List = new List<int[]>();
                List<int[]> nUnsignedReportData_Second_List = new List<int[]>();

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplit_Array = sLine.Split(' ');

                        if (sLine == "====================Odd Trace========================")
                        {
                            sTracePart = MainConstantParameter.m_sOddTrace;
                            bOddEvenType = true;
                            continue;
                        }
                        else if (sLine == "====================Even Trace=======================")
                        {
                            sTracePart = MainConstantParameter.m_sEvenTrace;
                            bOddEvenType = true;
                            continue;
                        }
                        else if (sLine == "====================Forward Trace=======================")
                        {
                            sTracePart = MainConstantParameter.m_sForwardTrace;
                            bOddEvenType = false;
                            continue;
                        }
                        else if (sLine == "====================Backward Trace======================")
                        {
                            sTracePart = MainConstantParameter.m_sBackwardTrace;
                            bOddEvenType = false;
                            continue;
                        }

                        if (sSplit_Array.Length >= nTraceDataCount * 2 + 2)
                        {
                            byte[] byteData_Array = new byte[nTraceDataCount * 2 + 2];

                            for (int nValueIndex = 0; nValueIndex < byteData_Array.Length; nValueIndex++)
                                byteData_Array[nValueIndex] = Convert.ToByte(sSplit_Array[nValueIndex], 16);

                            if (byteData_Array[0] != 0x01)
                                continue;

                            int[] nSignedReportData_Array = new int[nTraceDataCount];
                            int[] nUnsignedReportData_Array = new int[nTraceDataCount];

                            for (int nDataIndex = 0; nDataIndex < nTraceDataCount; nDataIndex++)
                            {
                                int nValue = byteData_Array[nDataIndex * 2 + 2] + byteData_Array[nDataIndex * 2 + 2 + 1] * 256;
                                nUnsignedReportData_Array[nDataIndex] = nValue;

                                if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                                {
                                    if (nValue >= 32767)
                                        nValue = nValue - 65536;

                                    nSignedReportData_Array[nDataIndex] = nValue;
                                }
                            }

                            if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                            {
                                if (sTracePart == MainConstantParameter.m_sOddTrace)
                                    nSignedReportData_First_List.Add(nSignedReportData_Array);
                                else if (sTracePart == MainConstantParameter.m_sEvenTrace)
                                    nSignedReportData_Second_List.Add(nSignedReportData_Array);
                                else if (sTracePart == MainConstantParameter.m_sForwardTrace)
                                    nSignedReportData_First_List.Add(nSignedReportData_Array);
                                else if (sTracePart == MainConstantParameter.m_sBackwardTrace)
                                    nSignedReportData_Second_List.Add(nSignedReportData_Array);
                            }

                            if (sTracePart == MainConstantParameter.m_sOddTrace)
                                nUnsignedReportData_First_List.Add(nUnsignedReportData_Array);
                            else if (sTracePart == MainConstantParameter.m_sEvenTrace)
                                nUnsignedReportData_Second_List.Add(nUnsignedReportData_Array);
                            else if (sTracePart == MainConstantParameter.m_sForwardTrace)
                                nUnsignedReportData_First_List.Add(nUnsignedReportData_Array);
                            else if (sTracePart == MainConstantParameter.m_sBackwardTrace)
                                nUnsignedReportData_Second_List.Add(nUnsignedReportData_Array);
                        }
                    }

                    int nMinLength = (nUnsignedReportData_First_List.Count < nUnsignedReportData_Second_List.Count) ? nUnsignedReportData_First_List.Count : nUnsignedReportData_Second_List.Count;

                    for (int nReportIndex = 0; nReportIndex < nMinLength; nReportIndex++)
                    {
                        int[] nUnsignedReportData_Array = new int[nTraceNumber];

                        if (bOddEvenType == true)
                        {
                            for (int nValueIndex = 0; nValueIndex < nTraceDataCount; nValueIndex++)
                            {
                                nUnsignedReportData_Array[nValueIndex * 2] = nUnsignedReportData_First_List[nReportIndex][nValueIndex];
                                nUnsignedReportData_Array[nValueIndex * 2 + 1] = nUnsignedReportData_Second_List[nReportIndex][nValueIndex];
                            }
                        }
                        else
                        {
                            for (int nValueIndex = 0; nValueIndex < nTraceNumber; nValueIndex++)
                            {
                                if (nValueIndex < m_nReportPartTraceLength)
                                    nUnsignedReportData_Array[nValueIndex] = nUnsignedReportData_First_List[nReportIndex][nValueIndex];
                                else
                                    nUnsignedReportData_Array[nValueIndex] = nUnsignedReportData_Second_List[nReportIndex][nValueIndex - m_nReportPartTraceLength];
                            }
                        }

                        nUnsignedReportData_List.Add(nUnsignedReportData_Array);

                        if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                        {
                            int[] nSignedReportData_Array = new int[nTraceNumber];

                            if (bOddEvenType == true)
                            {
                                for (int nValueIndex = 0; nValueIndex < nTraceDataCount; nValueIndex++)
                                {
                                    nSignedReportData_Array[nValueIndex * 2] = nSignedReportData_First_List[nReportIndex][nValueIndex];
                                    nSignedReportData_Array[nValueIndex * 2 + 1] = nSignedReportData_Second_List[nReportIndex][nValueIndex];
                                }
                            }
                            else
                            {
                                for (int nValueIndex = 0; nValueIndex < nTraceNumber; nValueIndex++)
                                {
                                    if (nValueIndex < m_nReportPartTraceLength)
                                        nSignedReportData_Array[nValueIndex] = nSignedReportData_First_List[nReportIndex][nValueIndex];
                                    else
                                        nSignedReportData_Array[nValueIndex] = nSignedReportData_Second_List[nReportIndex][nValueIndex - m_nReportPartTraceLength];
                                }
                            }

                            nSignedReportData_List.Add(nSignedReportData_Array);
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }
            }

            if (nUnsignedReportData_List == null || nUnsignedReportData_List.Count == 0)
            {
                m_sErrorMessage = string.Format("Read Self Report Mode Data Error in {0}[Count:{1}]", sFileName, nUnsignedReportData_List.Count);
                return false;
            }

            return true;
        }

        private bool AnalysisData()
        {
            ComputeStatisticData();

            ComputePNModePeriodStatisticData();

            return true;
        }

        private void ComputeStatisticData()
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();

                if (eTraceType == TraceType.RX)
                    cDataInfo_List = m_cDataInfo_RX_List;
                else if (eTraceType == TraceType.TX)
                    cDataInfo_List = m_cDataInfo_TX_List;

                foreach (DataInfo cDataInfo in cDataInfo_List)
                {
                    int nBSH_Magnification = ConvertBSHToMagnification(cDataInfo.m_nSelf_IQ_BSH);
                    int nPeriodCount = cDataInfo.m_nSelf_DFT_NUM / 8;
                    int nRemainder = cDataInfo.m_nSelf_DFT_NUM % 8;

                    int nRawPPeriodCount = nPeriodCount;
                    int nRawNPeriodCount = nPeriodCount;

                    if (nRemainder > 0)
                    {
                        if (nRemainder > 4)
                            nRawNPeriodCount++;
                        else
                            nRawPPeriodCount++;
                    }

                    int[,] nRawPPerdiodData_Array = new int[cDataInfo.m_nRXTraceNumber, nRawPPeriodCount * cDataInfo.m_nPNModeRawData_List.Count];
                    int[,] nRawNPerdiodData_Array = new int[cDataInfo.m_nRXTraceNumber, nRawNPeriodCount * cDataInfo.m_nPNModeRawData_List.Count];

                    long[,] lRawPNMinusData_Array = new long[cDataInfo.m_nRXTraceNumber, cDataInfo.m_nPNModeRawData_List.Count];
                    int[,] nConvertADCData_Array = new int[cDataInfo.m_nRXTraceNumber, cDataInfo.m_nPNModeRawData_List.Count];

                    for (int nTraceIndex = 0; nTraceIndex < cDataInfo.m_nRXTraceNumber; nTraceIndex++)
                    {
                        List<int> nPNModeRawData_List = new List<int>();
                        List<int> nADCData_List = new List<int>();
                        List<int> nPModeRawData_List = new List<int>();
                        List<int> nNModeRawData_List = new List<int>();

                        int nRawPCount = 0, nRawNCount = 0, nRawPPeriod = 0, nRawNPeriod = 0;
                        int nRawPPeriodIndex = 0, nRawNPeriodIndex = 0;

                        List<int> nRawPPeriod1DData_List = new List<int>();
                        List<int> nRawNPeriod1DData_List = new List<int>();
                        List<long> lRawPNMinus1DData_List = new List<long>();
                        List<int> nConvertADC1DData_List = new List<int>();

                        for (int nFrameIndex = 0; nFrameIndex < cDataInfo.m_nPNModeRawData_List.Count; nFrameIndex++)
                        {
                            long lRawPSum = 0, lRawNSum = 0;

                            for (int nTXIndex = 0; nTXIndex < cDataInfo.m_nSelf_DFT_NUM; nTXIndex++)
                            {
                                nPNModeRawData_List.Add(cDataInfo.m_nPNModeRawData_List[nFrameIndex][nTXIndex, nTraceIndex]);

                                int nModeValue = nTXIndex % 8;

                                if (nModeValue < 4)
                                {
                                    nPModeRawData_List.Add(cDataInfo.m_nPNModeRawData_List[nFrameIndex][nTXIndex, nTraceIndex]);
                                    nRawPPeriod += cDataInfo.m_nPNModeRawData_List[nFrameIndex][nTXIndex, nTraceIndex];
                                    nRawPCount++;

                                    if (nRawPCount == 4 || nTXIndex == cDataInfo.m_nSelf_DFT_NUM - 1)
                                    {
                                        nRawPPerdiodData_Array[nTraceIndex, nRawPPeriodIndex] = nRawPPeriod;
                                        nRawPPeriod = 0;
                                        nRawPCount = 0;
                                        nRawPPeriodIndex++;
                                    }

                                    lRawPSum += cDataInfo.m_nPNModeRawData_List[nFrameIndex][nTXIndex, nTraceIndex];
                                }
                                else
                                {
                                    nNModeRawData_List.Add(cDataInfo.m_nPNModeRawData_List[nFrameIndex][nTXIndex, nTraceIndex]);
                                    nRawNPeriod += cDataInfo.m_nPNModeRawData_List[nFrameIndex][nTXIndex, nTraceIndex];
                                    nRawNCount++;

                                    if (nRawNCount == 4 || nTXIndex == cDataInfo.m_nSelf_DFT_NUM - 1)
                                    {
                                        nRawNPerdiodData_Array[nTraceIndex, nRawNPeriodIndex] = nRawNPeriod;
                                        nRawNPeriod = 0;
                                        nRawNCount = 0;
                                        nRawNPeriodIndex++;
                                        
                                    }

                                    lRawNSum += cDataInfo.m_nPNModeRawData_List[nFrameIndex][nTXIndex, nTraceIndex];
                                }
                            }

                            nADCData_List.Add(cDataInfo.m_nADCData_List[nFrameIndex][nTraceIndex]);

                            long lRawPNMinusValue = lRawPSum - lRawNSum;
                            lRawPNMinusData_Array[nTraceIndex, nFrameIndex] = lRawPNMinusValue;

                            if (cDataInfo.m_nSelf_IQ_BSH >= 0)
                            {
                                int nConvertADC = (int)Math.Round((double)lRawPNMinusValue / nBSH_Magnification, 0, MidpointRounding.AwayFromZero);
                                nConvertADCData_Array[nTraceIndex, nFrameIndex] = nConvertADC;
                            }
                        }

                        for (int nValueIndex = 0; nValueIndex < nRawPPerdiodData_Array.GetLength(1); nValueIndex++)
                            nRawPPeriod1DData_List.Add(nRawPPerdiodData_Array[nTraceIndex, nValueIndex]);

                        for (int nValueIndex = 0; nValueIndex < nRawNPerdiodData_Array.GetLength(1); nValueIndex++)
                            nRawNPeriod1DData_List.Add(nRawNPerdiodData_Array[nTraceIndex, nValueIndex]);

                        for (int nValueIndex = 0; nValueIndex < lRawPNMinusData_Array.GetLength(1); nValueIndex++)
                            lRawPNMinus1DData_List.Add(lRawPNMinusData_Array[nTraceIndex, nValueIndex]);

                        for (int nValueIndex = 0; nValueIndex < nConvertADCData_Array.GetLength(1); nValueIndex++)
                            nConvertADC1DData_List.Add(nConvertADCData_Array[nTraceIndex, nValueIndex]);

                        double dPNModeRawMean = Math.Round(nPNModeRawData_List.Average(), 3, MidpointRounding.AwayFromZero);
                        double dPNModeRawStd = Math.Round(MathMethod.ComputeStd(nPNModeRawData_List), 3, MidpointRounding.AwayFromZero);

                        double dADCMean = Math.Round(nADCData_List.Average(), 3, MidpointRounding.AwayFromZero);
                        double dADCStd = Math.Round(MathMethod.ComputeStd(nADCData_List), 3, MidpointRounding.AwayFromZero);

                        cDataInfo.m_cStatisticData.m_dPNModeRawMean_List.Add(dPNModeRawMean);
                        cDataInfo.m_cStatisticData.m_dPNModeRawStd_List.Add(dPNModeRawStd);

                        cDataInfo.m_cStatisticData.m_dADCMean_List.Add(dADCMean);
                        cDataInfo.m_cStatisticData.m_dADCStd_List.Add(dADCStd);

                        double dPModeRawMean = Math.Round(nPModeRawData_List.Average(), 3, MidpointRounding.AwayFromZero);
                        double dPModeRawStd = Math.Round(MathMethod.ComputeStd(nPModeRawData_List), 3, MidpointRounding.AwayFromZero);
                        int nPModeRawMax = nPModeRawData_List.Max();
                        int nPModeRawMin = nPModeRawData_List.Min();
                        double dRawPPeriodStd = Math.Round(MathMethod.ComputeStd(nRawPPeriod1DData_List), 3, MidpointRounding.AwayFromZero);

                        double dNModeRawMean = Math.Round(nNModeRawData_List.Average(), 3, MidpointRounding.AwayFromZero);
                        double dNModeRawStd = Math.Round(MathMethod.ComputeStd(nNModeRawData_List), 3, MidpointRounding.AwayFromZero);
                        int nNModeRawMax = nNModeRawData_List.Max();
                        int nNModeRawMin = nNModeRawData_List.Min();
                        double dRawNPeriodStd = Math.Round(MathMethod.ComputeStd(nRawNPeriod1DData_List), 3, MidpointRounding.AwayFromZero);

                        double dPawPNMinusMean = Math.Round(lRawPNMinus1DData_List.Average(), 3, MidpointRounding.AwayFromZero);
                        double dConvertADCMean = Math.Round(nConvertADC1DData_List.Average(), 3, MidpointRounding.AwayFromZero);

                        cDataInfo.m_cStatisticData.m_dPModeRawMean_List.Add(dPModeRawMean);
                        cDataInfo.m_cStatisticData.m_dPModeRawStd_List.Add(dPModeRawStd);
                        cDataInfo.m_cStatisticData.m_nPModeRawMax_List.Add(nPModeRawMax);
                        cDataInfo.m_cStatisticData.m_nPModeRawMin_List.Add(nPModeRawMin);
                        cDataInfo.m_cStatisticData.m_dRawPPeriodStd_List.Add(dRawPPeriodStd);
                        cDataInfo.m_nRawPPeriodData_Array = nRawPPerdiodData_Array;

                        cDataInfo.m_cStatisticData.m_dNModeRawMean_List.Add(dNModeRawMean);
                        cDataInfo.m_cStatisticData.m_dNModeRawStd_List.Add(dNModeRawStd);
                        cDataInfo.m_cStatisticData.m_nNModeRawMax_List.Add(nNModeRawMax);
                        cDataInfo.m_cStatisticData.m_nNModeRawMin_List.Add(nNModeRawMin);
                        cDataInfo.m_cStatisticData.m_dRawNPeriodStd_List.Add(dRawNPeriodStd);
                        cDataInfo.m_nRawNPeriodData_Array = nRawNPerdiodData_Array;

                        cDataInfo.m_cStatisticData.m_dRawPNMinusMean_List.Add(dPawPNMinusMean);
                        cDataInfo.m_cStatisticData.m_dConvertADCMean_List.Add(dConvertADCMean);
                        cDataInfo.m_lRawPNMinusData_Array = lRawPNMinusData_Array;
                        cDataInfo.m_nConvertADCData_Array = nConvertADCData_Array;
                    }
                }
            }
        }

        private void ComputePNModePeriodStatisticData()
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();

                if (eTraceType == TraceType.RX)
                    cDataInfo_List = m_cDataInfo_RX_List;
                else if (eTraceType == TraceType.TX)
                    cDataInfo_List = m_cDataInfo_TX_List;

                foreach (DataInfo cDataInfo in cDataInfo_List)
                {
                    for (int nTraceIndex = 0; nTraceIndex < cDataInfo.m_nRXTraceNumber; nTraceIndex++)
                    {
                        List<List<int>> nPNModePeriodData_List = new List<List<int>>();

                        for (int nPeriodIndex = 0; nPeriodIndex < m_nPNModePeriodNumber; nPeriodIndex++)
                        {
                            List<int> nValue_List = new List<int>();
                            nPNModePeriodData_List.Add(nValue_List);
                        }

                        for (int nFrameIndex = 0; nFrameIndex < cDataInfo.m_nPNModeRawData_List.Count; nFrameIndex++)
                        {
                            for (int nTXIndex = 0; nTXIndex < cDataInfo.m_nSelf_DFT_NUM; nTXIndex++)
                            {
                                int nModeValue = nTXIndex % m_nPNModePeriodNumber;
                                nPNModePeriodData_List[nModeValue].Add(cDataInfo.m_nPNModeRawData_List[nFrameIndex][nTXIndex, nTraceIndex]);
                            }
                        }

                        PNModePeriodInfo cPNModePeriodInfo = new PNModePeriodInfo();

                        for (int nPeriodIndex = 0; nPeriodIndex < m_nPNModePeriodNumber; nPeriodIndex++)
                        {
                            double dMean = Math.Round(nPNModePeriodData_List[nPeriodIndex].Average(), 3, MidpointRounding.AwayFromZero);
                            double dStd = Math.Round(MathMethod.ComputeStd(nPNModePeriodData_List[nPeriodIndex]), 3, MidpointRounding.AwayFromZero);

                            cPNModePeriodInfo.m_dMean_Array[nPeriodIndex] = dMean;
                            cPNModePeriodInfo.m_dStd_Array[nPeriodIndex] = dStd;
                        }

                        cDataInfo.m_cPNModePeriodInfo_List.Add(cPNModePeriodInfo);
                    }
                }
            }
        }

        private bool SaveAnalysis()
        {
            bool bError = false;

            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();
                int nTraceNumber = 0;

                if (eTraceType == TraceType.RX)
                {
                    cDataInfo_List = m_cDataInfo_RX_List;
                    nTraceNumber = m_nTraceNumber_RX;
                }
                else if (eTraceType == TraceType.TX)
                {
                    cDataInfo_List = m_cDataInfo_TX_List;
                    nTraceNumber = m_nTraceNumber_TX;
                }

                m_nCompareOperator = m_nCOMPARE_Frequency;
                cDataInfo_List.Sort(new InfoDataComparer());

                string sReportFilePath = string.Format(@"{0}\Analysis_{1}.csv", m_sLogDirectoryPath, eTraceType.ToString());

                FileStream fs = new FileStream(sReportFilePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                List<string> sColumnHeader_List = new List<string>() { 
                    "Index", 
                    "PH1", 
                    "PH2E_LMT", 
                    "PH2_LAT", 
                    "PH2", 
                    "Frequency(KHz)", 
                    "DFT_NUM", 
                    "Gain", 
                    "CAG", 
                    "IQ_BSH",
                    "NCP", 
                    "NCN", 
                    "CAL", 
                    "" 
                };

                for (int nTraceIndex = 1; nTraceIndex <= nTraceNumber; nTraceIndex++)
                    sColumnHeader_List.Add(nTraceIndex.ToString());

                try
                {
                    Write_Tool_Information(sw, m_sFILETYPE_ANALYSIS);

                    for (int nDataIndex = 0; nDataIndex < m_sDataType_Array.Length; nDataIndex++)
                    {
                        bool bLineSpace = true;

                        if (nDataIndex == m_sDataType_Array.Length - 1)
                            bLineSpace = false;

                        WriteData(sw, sColumnHeader_List, cDataInfo_List, m_sDataType_Array[nDataIndex], bLineSpace);
                    }

                    if (m_eGetKValue == KValueState.GetKValue)
                    {
                        sw.WriteLine();

                        for (int nDataIndex = 0; nDataIndex < m_sKValueType_Array.Length; nDataIndex++)
                        {
                            bool bLineSpace = true;

                            if (nDataIndex == m_sDataType_Array.Length - 1)
                                bLineSpace = false;

                            WriteKValueData(sw, sColumnHeader_List, cDataInfo_List, m_sKValueType_Array[nDataIndex], bLineSpace);
                        }
                    }
                }
                catch (IOException ex)
                {
                    string sMessage = ex.Message;

                    m_sErrorMessage = string.Format("Save {0} Analysis Data Error", eTraceType.ToString());
                    bError = true;
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }

                if (bError == true)
                    break;
            }

            if (bError == true)
                return false;
            else
                return true;
        }

        private bool SavePNModePeriodAnalysis()
        {
            bool bError = false;

            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();
                int nTraceNumber = 0;

                if (eTraceType == TraceType.RX)
                {
                    cDataInfo_List = m_cDataInfo_RX_List;
                    nTraceNumber = m_nTraceNumber_RX;
                }
                else if (eTraceType == TraceType.TX)
                {
                    cDataInfo_List = m_cDataInfo_TX_List;
                    nTraceNumber = m_nTraceNumber_TX;
                }

                m_nCompareOperator = m_nCOMPARE_Frequency;
                cDataInfo_List.Sort(new InfoDataComparer());

                string sReportFilePath = string.Format(@"{0}\PNModePeriodAnalysis_{1}.csv", m_sLogDirectoryPath, eTraceType.ToString());

                FileStream fs = new FileStream(sReportFilePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                List<string> sColumnHeader_List = new List<string>() { 
                    "Index", 
                    "PH1", 
                    "PH2E_LMT", 
                    "PH2_LAT", 
                    "PH2", 
                    "Frequency(KHz)", 
                    "DFT_NUM", 
                    "Gain", 
                    "CAG", 
                    "IQ_BSH",
                    "NCP", 
                    "NCN", 
                    "CAL", 
                    "" 
                };

                for (int nTraceIndex = 1; nTraceIndex <= nTraceNumber; nTraceIndex++)
                    sColumnHeader_List.Add(nTraceIndex.ToString());

                try
                {
                    Write_Tool_Information(sw, m_sFILETYPE_ANALYSIS);

                    sw.WriteLine();

                    sw.WriteLine("PN Mode Period Mean");

                    for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_List.Count; nColumnIndex++)
                    {
                        if (nColumnIndex != sColumnHeader_List.Count - 1)
                            sw.Write(string.Format("{0},", sColumnHeader_List[nColumnIndex]));
                        else
                            sw.WriteLine(sColumnHeader_List[nColumnIndex]);
                    }

                    for (int nDataIndex = 0; nDataIndex < cDataInfo_List.Count; nDataIndex++)
                    {
                        DataInfo cDataInfo = cDataInfo_List[nDataIndex];

                        for (int nPeriodIndex = 0; nPeriodIndex < m_nPNModePeriodNumber; nPeriodIndex++)
                        {
                            if (nPeriodIndex == 0)
                            {
                                sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper()));
                                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2.ToString("x2").ToUpper()));
                                sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_DFT_NUM.ToString()));
                                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_Gain.ToString()));
                                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_CAG.ToString()));
                                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_IQ_BSH.ToString()));

                                if (m_bRunCalibrationSequence == true)
                                {
                                    sw.Write(string.Format("{0},", cDataInfo.m_nNCPValue.ToString()));
                                    sw.Write(string.Format("{0},", cDataInfo.m_nNCNValue.ToString()));
                                }
                                else
                                {
                                    sw.Write("NA,");
                                    sw.Write("NA,");
                                }

                                if (m_bSetCAL == true)
                                    sw.Write(string.Format("{0},", cDataInfo.m_nCALValue.ToString()));
                                else
                                    sw.Write("NA,");

                                sw.Write(",");
                            }
                            else
                            {
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                            }

                            for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                            {
                                if (nTraceIndex == nTraceNumber - 1)
                                    sw.WriteLine(cDataInfo.m_cPNModePeriodInfo_List[nTraceIndex].m_dMean_Array[nPeriodIndex]);
                                else
                                    sw.Write(string.Format("{0},", cDataInfo.m_cPNModePeriodInfo_List[nTraceIndex].m_dMean_Array[nPeriodIndex]));
                            }
                        }
                    }

                    sw.WriteLine();

                    sw.WriteLine("PN Mode Period Std");

                    for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_List.Count; nColumnIndex++)
                    {
                        if (nColumnIndex != sColumnHeader_List.Count - 1)
                            sw.Write(string.Format("{0},", sColumnHeader_List[nColumnIndex]));
                        else
                            sw.WriteLine(sColumnHeader_List[nColumnIndex]);
                    }

                    for (int nDataIndex = 0; nDataIndex < cDataInfo_List.Count; nDataIndex++)
                    {
                        DataInfo cDataInfo = cDataInfo_List[nDataIndex];

                        for (int nPeriodIndex = 0; nPeriodIndex < m_nPNModePeriodNumber; nPeriodIndex++)
                        {
                            if (nPeriodIndex == 0)
                            {
                                sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper()));
                                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2.ToString("x2").ToUpper()));
                                sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_DFT_NUM.ToString()));
                                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_Gain.ToString()));
                                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_CAG.ToString()));
                                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_IQ_BSH.ToString()));

                                if (m_bRunCalibrationSequence == true)
                                {
                                    sw.Write(string.Format("{0},", cDataInfo.m_nNCPValue.ToString()));
                                    sw.Write(string.Format("{0},", cDataInfo.m_nNCNValue.ToString()));
                                }
                                else
                                {
                                    sw.Write("NA,");
                                    sw.Write("NA,");
                                }

                                if (m_bSetCAL == true)
                                    sw.Write(string.Format("{0},", cDataInfo.m_nCALValue.ToString()));
                                else
                                    sw.Write("NA,");

                                sw.Write(",");
                            }
                            else
                            {
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                                sw.Write(",");
                            }

                            for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                            {
                                if (nTraceIndex == nTraceNumber - 1)
                                    sw.WriteLine(cDataInfo.m_cPNModePeriodInfo_List[nTraceIndex].m_dStd_Array[nPeriodIndex]);
                                else
                                    sw.Write(string.Format("{0},", cDataInfo.m_cPNModePeriodInfo_List[nTraceIndex].m_dStd_Array[nPeriodIndex]));
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    string sMessage = ex.Message;

                    m_sErrorMessage = string.Format("Save {0} PN Mode Period Analysis Data Error", eTraceType.ToString());
                    bError = true;
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }

                if (bError == true)
                    break;
            }

            if (bError == true)
                return false;
            else
                return true;
        }

        private void WriteData(StreamWriter sw, List<string> sColumnHeader_List, List<DataInfo> cDataInfo_List, string sDataType, bool bLineSpace)
        {
            string sTitleName = string.Format("{0} Information", sDataType);

            sw.WriteLine(sTitleName);

            for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_List.Count; nColumnIndex++)
            {
                if (nColumnIndex != sColumnHeader_List.Count - 1)
                    sw.Write(string.Format("{0},", sColumnHeader_List[nColumnIndex]));
                else
                    sw.WriteLine(sColumnHeader_List[nColumnIndex]);
            }

            for (int nDataIndex = 0; nDataIndex < cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = cDataInfo_List[nDataIndex];

                List<double> dValue_List = new List<double>();
                List<int> nValue_List = new List<int>();

                if (sDataType == m_sDATATYPE_PNMODERAWMEAN)
                    dValue_List = cDataInfo.m_cStatisticData.m_dPNModeRawMean_List;
                else if (sDataType == m_sDATATYPE_PNMODERAWSTD)
                    dValue_List = cDataInfo.m_cStatisticData.m_dPNModeRawStd_List;
                else if (sDataType == m_sDATATYPE_ADCMEAN)
                    dValue_List = cDataInfo.m_cStatisticData.m_dADCMean_List;
                else if (sDataType == m_sDATATYPE_ADCSTD)
                    dValue_List = cDataInfo.m_cStatisticData.m_dADCStd_List;
                else if (sDataType == m_sDATATYPE_PMODERAWMEAN)
                    dValue_List = cDataInfo.m_cStatisticData.m_dPModeRawMean_List;
                else if (sDataType == m_sDATATYPE_PMODERAWSTD)
                    dValue_List = cDataInfo.m_cStatisticData.m_dPModeRawStd_List;
                else if (sDataType == m_sDATATYPE_NMODERAWMEAN)
                    dValue_List = cDataInfo.m_cStatisticData.m_dNModeRawMean_List;
                else if (sDataType == m_sDATATYPE_NMODERAWSTD)
                    dValue_List = cDataInfo.m_cStatisticData.m_dNModeRawStd_List;
                else if (sDataType == m_sDATATYPE_RAWPPERIODSTD)
                    dValue_List = cDataInfo.m_cStatisticData.m_dRawPPeriodStd_List;
                else if (sDataType == m_sDATATYPE_PMODERAWMAX)
                    nValue_List = cDataInfo.m_cStatisticData.m_nPModeRawMax_List;
                else if (sDataType == m_sDATATYPE_PMODERAWMIN)
                    nValue_List = cDataInfo.m_cStatisticData.m_nPModeRawMin_List;
                else if (sDataType == m_sDATATYPE_NMODERAWMAX)
                    nValue_List = cDataInfo.m_cStatisticData.m_nNModeRawMax_List;
                else if (sDataType == m_sDATATYPE_NMODERAWMIN)
                    nValue_List = cDataInfo.m_cStatisticData.m_nNModeRawMin_List;
                else if (sDataType == m_sDATATYPE_RAWNPERIODSTD)
                    dValue_List = cDataInfo.m_cStatisticData.m_dRawNPeriodStd_List;
                else if (sDataType == m_sDATATYPE_RAWNPNMinusMean)
                    dValue_List = cDataInfo.m_cStatisticData.m_dRawPNMinusMean_List;
                else if (sDataType == m_sDATATYPE_ConvertADCMean)
                    dValue_List = cDataInfo.m_cStatisticData.m_dConvertADCMean_List;

                sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2.ToString("x2").ToUpper()));
                sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_DFT_NUM.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_Gain.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_CAG.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_IQ_BSH.ToString()));

                if (m_bRunCalibrationSequence == true)
                {
                    sw.Write(string.Format("{0},", cDataInfo.m_nNCPValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nNCNValue.ToString()));
                }
                else
                {
                    sw.Write("NA,");
                    sw.Write("NA,");
                }

                if (m_bSetCAL == true)
                    sw.Write(string.Format("{0},", cDataInfo.m_nCALValue.ToString()));
                else
                    sw.Write("NA,");

                sw.Write(",");

                for (int nTraceIndex = 0; nTraceIndex < cDataInfo.m_nRXTraceNumber; nTraceIndex++)
                {
                    if (sDataType == m_sDATATYPE_PMODERAWMAX ||
                        sDataType == m_sDATATYPE_PMODERAWMIN ||
                        sDataType == m_sDATATYPE_NMODERAWMAX ||
                        sDataType == m_sDATATYPE_NMODERAWMIN)
                    {
                        if (nTraceIndex == cDataInfo.m_nRXTraceNumber - 1)
                            sw.WriteLine(string.Format("{0}", nValue_List[nTraceIndex]));
                        else
                            sw.Write(string.Format("{0},", nValue_List[nTraceIndex]));
                    }
                    else
                    {
                        if (nTraceIndex == cDataInfo.m_nRXTraceNumber - 1)
                            sw.WriteLine(string.Format("{0}", dValue_List[nTraceIndex]));
                        else
                            sw.Write(string.Format("{0},", dValue_List[nTraceIndex]));
                    }
                }
            }

            if (bLineSpace == true)
                sw.WriteLine();
        }

        private void WriteKValueData(StreamWriter sw, List<string> sColumnHeader_List, List<DataInfo> cDataInfo_List, string sDataType, bool bLineSpace)
        {
            string sTitleName = string.Format("{0} Information", sDataType);

            sw.WriteLine(sTitleName);

            for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_List.Count; nColumnIndex++)
            {
                if (nColumnIndex != sColumnHeader_List.Count - 1)
                    sw.Write(string.Format("{0},", sColumnHeader_List[nColumnIndex]));
                else
                    sw.WriteLine(sColumnHeader_List[nColumnIndex]);
            }

            for (int nDataIndex = 0; nDataIndex < cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = cDataInfo_List[nDataIndex];

                int[] nValue_Array = null;

                if (sDataType == m_sKVALUETYPE_KP)
                    nValue_Array = cDataInfo.m_nKPValue_Array;
                else if (sDataType == m_sKVALUETYPE_KN)
                    nValue_Array = cDataInfo.m_nKNValue_Array;

                sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2.ToString("x2").ToUpper()));
                sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_DFT_NUM.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_Gain.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_CAG.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_IQ_BSH.ToString()));

                if (m_bRunCalibrationSequence == true)
                {
                    sw.Write(string.Format("{0},", cDataInfo.m_nNCPValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nNCNValue.ToString()));
                }
                else
                {
                    sw.Write("NA,");
                    sw.Write("NA,");
                }

                if (m_bSetCAL == true)
                    sw.Write(string.Format("{0},", cDataInfo.m_nCALValue.ToString()));
                else
                    sw.Write("NA,");

                sw.Write(",");

                for (int nTraceIndex = 0; nTraceIndex < cDataInfo.m_nRXTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex == cDataInfo.m_nRXTraceNumber - 1)
                        sw.WriteLine(string.Format("{0}", nValue_Array[nTraceIndex]));
                    else
                        sw.Write(string.Format("{0},", nValue_Array[nTraceIndex]));
                }
            }

            if (bLineSpace == true)
                sw.WriteLine();
        }

        private bool SaveLineChart()
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();
                int nTraceNumber = 0;

                if (eTraceType == TraceType.RX)
                {
                    cDataInfo_List = m_cDataInfo_RX_List;
                    nTraceNumber = m_nTraceNumber_RX;
                }
                else if (eTraceType == TraceType.TX)
                {
                    cDataInfo_List = m_cDataInfo_TX_List;
                    nTraceNumber = m_nTraceNumber_TX;
                }

                m_nCompareOperator = m_nCOMPARE_Frequency;
                cDataInfo_List.Sort(new InfoDataComparer());

                string sDirectoryPath = string.Format(@"{0}\{1}\{2}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART, eTraceType.ToString());

                if (Directory.Exists(sDirectoryPath) == false)
                    Directory.CreateDirectory(sDirectoryPath);

                int nDataCount = cDataInfo_List.Count;
                ChartColor cChartColor = SetColorList(nDataCount, false);

                for (int nDataIndex = 0; nDataIndex < m_sDataType_Array.Length; nDataIndex++)
                {
                    if (CreateLineChartByTrace(m_sDataType_Array[nDataIndex], eTraceType, sDirectoryPath, nTraceNumber, cDataInfo_List, cChartColor) == false)
                        return false;
                }

                double dFrequency_LB = 0.0;
                double dFrequency_HB = 0.0;

                GetFrequencyHLBounary(ref dFrequency_HB, ref dFrequency_LB, cDataInfo_List);

                cChartColor = SetColorList(nTraceNumber, false);

                for (int nDataIndex = 0; nDataIndex < m_sDataType_Array.Length; nDataIndex++)
                {
                    if (CreateLineChartByFrequency(m_sDataType_Array[nDataIndex], eTraceType, sDirectoryPath, cDataInfo_List, cChartColor, dFrequency_HB, dFrequency_LB,
                                                   nTraceNumber) == false)
                        return false;
                }
            }

            return true;
        }

        private bool CreateLineChartByTrace(string sChartType, TraceType eTraceType, string sDirectoryPath, int nTraceNumber, List<DataInfo> cDataInfo_List,
                                            ChartColor cChartColor)
        {
            string sChartTitle = "";
            string sYValueTitle = "";
            string sFileName = string.Format("{0}TraceChart_{1}", sChartType, eTraceType.ToString());

            switch (sChartType)
            {
                case m_sDATATYPE_PNMODERAWMEAN:
                    sChartTitle = "PNModeRawData Mean";
                    sYValueTitle = "Mean";
                    break;
                case m_sDATATYPE_PNMODERAWSTD:
                    sChartTitle = "PNModeRawData Std";
                    sYValueTitle = "Std";
                    break;
                case m_sDATATYPE_ADCMEAN:
                    sChartTitle = "ADCData Mean";
                    sYValueTitle = "Mean";
                    break;
                case m_sDATATYPE_ADCSTD:
                    sChartTitle = "ADCData Std";
                    sYValueTitle = "Std";
                    break;
                case m_sDATATYPE_PMODERAWMEAN:
                    sChartTitle = "PModeRawData Mean";
                    sYValueTitle = "Mean";
                    break;
                case m_sDATATYPE_PMODERAWSTD:
                    sChartTitle = "PModeRawData Std";
                    sYValueTitle = "Std";
                    break;
                case m_sDATATYPE_NMODERAWMEAN:
                    sChartTitle = "NModeRawData Mean";
                    sYValueTitle = "Mean";
                    break;
                case m_sDATATYPE_NMODERAWSTD:
                    sChartTitle = "NModeRawData Std";
                    sYValueTitle = "Std";
                    break;
                default:
                    break;
            }

            double dMax = 0.0;
            double dMin = 0.0;
            GetMaxMinValue(ref dMax, ref dMin, sChartType, cDataInfo_List);

            int nValueLength = ((int)dMin).ToString("D").Length;

            if (((int)dMin).ToString("D").IndexOf('-') == 0)
                nValueLength = nValueLength - 1;

            int nYMinimumValue = 0;

            if (nValueLength > 1)
            {
                int nModeValue = (int)Math.Pow((double)10, (double)(nValueLength - 1));

                if (dMin < 0)
                    nYMinimumValue = (((int)dMin / nModeValue) - 1) * nModeValue;
                else
                    nYMinimumValue = ((int)dMin / nModeValue) * nModeValue;
            }

            string sTitleName = string.Format("{0} Distribution By Trace({1})", sChartTitle, eTraceType.ToString());
            string sFilePath = string.Format(@"{0}\{1}.jpg", sDirectoryPath, sFileName);

            //Show Line Chart
            Chart cChart = new Chart();
            var cChartArea = new ChartArea();
            cChart.ChartAreas.Add(cChartArea);
            cChart.Width = 1500;
            cChart.Height = 500;
            cChart.Legends.Add("Legend");
            cChart.Legends["Legend"].Font = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
            cChart.Titles.Add(sTitleName);
            cChart.Titles[0].Font = new Font("Times New Roman", 18);
            cChart.ChartAreas[0].AxisY.Title = string.Format("{0} Value", sYValueTitle);
            cChart.ChartAreas[0].AxisY.TitleFont = new Font("Times New Roman", 10);
            cChart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisX.Title = "Trace Number";
            cChart.ChartAreas[0].AxisX.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisX.Interval = 2;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;
            cChart.Titles[0].Position.X = 50;
            cChart.Titles[0].Position.Height = 10;
            cChart.ChartAreas[0].Position.X = 0;
            cChart.ChartAreas[0].Position.Y = 50;
            cChart.ChartAreas[0].Position.Width = 77;
            cChart.ChartAreas[0].Position.Height = 90;
            cChart.ChartAreas[0].AxisX.Minimum = 1;
            cChart.ChartAreas[0].AxisX.Maximum = nTraceNumber;
            cChart.ChartAreas[0].AxisX.Interval = 1;

            if (nValueLength > 1)
                cChart.ChartAreas[0].AxisY.Minimum = nYMinimumValue;

            cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.Legends["Legend"].Position.X = 78;
            cChart.Legends["Legend"].Position.Y = 0;
            cChart.Legends["Legend"].Position.Width = 22;

            for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
            {
                DataInfo cDataInfo = cDataInfo_List[nSetIndex];

                string sFrequency = string.Format("{0}KHz", cDataInfo.m_dFrequency.ToString("F3"));
                Series cSeries = new Series(sFrequency);
                cSeries.ChartType = SeriesChartType.Line;
                cSeries.IsValueShownAsLabel = false;
                cSeries.MarkerStyle = MarkerStyle.Circle;

                cSeries.Color = Color.FromArgb(cChartColor.m_nColorR_List[nSetIndex], cChartColor.m_nColorG_List[nSetIndex], cChartColor.m_nColorB_List[nSetIndex]);

                for (int nTraceIndex = 1; nTraceIndex <= nTraceNumber; nTraceIndex++)
                {
                    if (sChartType == m_sDATATYPE_PNMODERAWMEAN)
                        cSeries.Points.AddXY(nTraceIndex, cDataInfo.m_cStatisticData.m_dPNModeRawMean_List[nTraceIndex - 1]);
                    else if (sChartType == m_sDATATYPE_PNMODERAWSTD)
                        cSeries.Points.AddXY(nTraceIndex, cDataInfo.m_cStatisticData.m_dPNModeRawStd_List[nTraceIndex - 1]);
                    else if (sChartType == m_sDATATYPE_ADCMEAN)
                        cSeries.Points.AddXY(nTraceIndex, cDataInfo.m_cStatisticData.m_dADCMean_List[nTraceIndex - 1]);
                    else if (sChartType == m_sDATATYPE_ADCSTD)
                        cSeries.Points.AddXY(nTraceIndex, cDataInfo.m_cStatisticData.m_dADCStd_List[nTraceIndex - 1]);
                    else if (sChartType == m_sDATATYPE_PMODERAWMEAN)
                        cSeries.Points.AddXY(nTraceIndex, cDataInfo.m_cStatisticData.m_dPModeRawMean_List[nTraceIndex - 1]);
                    else if (sChartType == m_sDATATYPE_PMODERAWSTD)
                        cSeries.Points.AddXY(nTraceIndex, cDataInfo.m_cStatisticData.m_dPModeRawStd_List[nTraceIndex - 1]);
                    else if (sChartType == m_sDATATYPE_NMODERAWMEAN)
                        cSeries.Points.AddXY(nTraceIndex, cDataInfo.m_cStatisticData.m_dNModeRawMean_List[nTraceIndex - 1]);
                    else if (sChartType == m_sDATATYPE_NMODERAWSTD)
                        cSeries.Points.AddXY(nTraceIndex, cDataInfo.m_cStatisticData.m_dNModeRawStd_List[nTraceIndex - 1]);
                }

                cChart.Series.Add(cSeries);
            }

            if (cDataInfo_List.Count / 22 >= 2 && cDataInfo_List.Count % 22 > 0)
            {
                cChart.ChartAreas[0].Position.Width = 70;
                cChart.Legends["Legend"].Position.X = 71;
                cChart.Legends["Legend"].Position.Width = 29;
                cChart.Legends["Legend"].Position.Height = 100;
                cChart.Legends["Legend"].Font = new Font("Times New Roman", 8);
            }
            else if (cDataInfo_List.Count / 14 >= 1 && cDataInfo_List.Count % 14 > 0)
            {
                cChart.Legends["Legend"].Position.Height = 100;
                cChart.Legends["Legend"].Font = new Font("Times New Roman", 14);
            }
            else
            {
                int nHeight = cDataInfo_List.Count * 8;

                if (cDataInfo_List.Count >= 13)
                    nHeight = cDataInfo_List.Count * 7;

                cChart.Legends["Legend"].Position.Height = nHeight;
                cChart.Legends["Legend"].Font = new Font("Times New Roman", 16);
            }

            try
            {
                cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
            }
            catch
            {
                m_sErrorMessage = string.Format("Save Chart File Error[{0}]", sFileName);
                return false;
            }

            return true;
        }

        private bool CreateLineChartByFrequency(string sChartType, TraceType eTraceType, string sDirectoryPath, List<DataInfo> cDataInfo_List, ChartColor cChartColor,
                                                double dFrequency_HB, double dFrequency_LB, int nTraceNumber, bool bSignedData = true)
        {
            string sChartTitle = "";
            string sYValueTitle = "";
            string sFileName = string.Format("{0}FrequencyChart_{1}", sChartType, eTraceType.ToString());

            switch (sChartType)
            {
                case m_sDATATYPE_PNMODERAWMEAN:
                    sChartTitle = "PNModeRawData Mean";
                    sYValueTitle = "Mean";
                    break;
                case m_sDATATYPE_PNMODERAWSTD:
                    sChartTitle = "PNModeRawData Std";
                    sYValueTitle = "Std";
                    break;
                case m_sDATATYPE_ADCMEAN:
                    sChartTitle = "ADCData Mean";
                    sYValueTitle = "Mean";
                    break;
                case m_sDATATYPE_ADCSTD:
                    sChartTitle = "ADCData Std";
                    sYValueTitle = "Std";
                    break;
                case m_sDATATYPE_PMODERAWMEAN:
                    sChartTitle = "PModeRawData Mean";
                    sYValueTitle = "Mean";
                    break;
                case m_sDATATYPE_PMODERAWSTD:
                    sChartTitle = "PModeRawData Std";
                    sYValueTitle = "Std";
                    break;
                case m_sDATATYPE_NMODERAWMEAN:
                    sChartTitle = "NModeRawData Mean";
                    sYValueTitle = "Mean";
                    break;
                case m_sDATATYPE_NMODERAWSTD:
                    sChartTitle = "NModeRawData Std";
                    sYValueTitle = "Std";
                    break;
                case m_sDATATYPE_NOISEREFERENCE:
                    if (bSignedData == true)
                    {
                        sChartTitle = "NoiseReference_Signed(ADC Std Max & Mean)";
                        sFileName = string.Format("{0}FrequencyChart_Signed_{1}", sChartType, eTraceType.ToString());
                    }
                    else
                    {
                        sChartTitle = "NoiseReference_Unsigned(ADC Std Max & Mean)";
                        sFileName = string.Format("{0}FrequencyChart_Unsigned_{1}", sChartType, eTraceType.ToString());
                    }
                    sYValueTitle = "Noise";
                    break;
                default:
                    break;
            }

            double dMax = 0.0;
            double dMin = 0.0;

            GetMaxMinValue(ref dMax, ref dMin, sChartType, cDataInfo_List, bSignedData);

            int nValueLength = ((int)dMin).ToString("D").Length;

            if (((int)dMin).ToString("D").IndexOf('-') == 0)
                nValueLength = nValueLength - 1;

            int nYMinimumValue = 0;

            if (nValueLength > 1)
            {
                int nModeValue = (int)Math.Pow((double)10, (double)(nValueLength - 1));

                if (dMin < 0)
                    nYMinimumValue = (((int)dMin / nModeValue) - 1) * nModeValue;
                else
                    nYMinimumValue = ((int)dMin / nModeValue) * nModeValue;
            }

            string sTitleName = string.Format("{0} Distribution By Freqeuncy({1})", sChartTitle, eTraceType.ToString());
            string sFilePath = string.Format(@"{0}\{1}.jpg", sDirectoryPath, sFileName);

            //Show Line Chart
            Chart cChart = new Chart();
            var cChartArea = new ChartArea();
            cChart.ChartAreas.Add(cChartArea);
            cChart.Width = 1500;
            cChart.Height = 500;
            cChart.Legends.Add("Legend");
            cChart.Legends["Legend"].Font = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
            cChart.Titles.Add(sTitleName);
            cChart.Titles[0].Font = new Font("Times New Roman", 18);
            cChart.ChartAreas[0].AxisY.Title = string.Format("{0} Value", sYValueTitle);
            cChart.ChartAreas[0].AxisY.TitleFont = new Font("Times New Roman", 10);
            cChart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisX.Title = "Frequency(KHz)";
            cChart.ChartAreas[0].AxisX.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisX.Interval = 2;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;
            cChart.Titles[0].Position.X = 50;
            cChart.Titles[0].Position.Height = 10;

            if (sChartType == m_sDATATYPE_NOISEREFERENCE)
            {
                cChart.ChartAreas[0].Position.X = 0;
                cChart.ChartAreas[0].Position.Y = 50;
                cChart.ChartAreas[0].Position.Width = 90;
                cChart.ChartAreas[0].Position.Height = 90;
            }
            else
            {
                cChart.ChartAreas[0].Position.X = 0;
                cChart.ChartAreas[0].Position.Y = 50;
                cChart.ChartAreas[0].Position.Width = 77;
                cChart.ChartAreas[0].Position.Height = 90;
            }

            cChart.ChartAreas[0].AxisX.Minimum = dFrequency_LB;
            cChart.ChartAreas[0].AxisX.Maximum = dFrequency_HB;
            cChart.ChartAreas[0].AxisX.Interval = 5;

            if (nValueLength > 1)
                cChart.ChartAreas[0].AxisY.Minimum = nYMinimumValue;

            cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            if (sChartType == m_sDATATYPE_NOISEREFERENCE)
            {
                Series cSeries_Max = new Series("Max");
                cSeries_Max.ChartType = SeriesChartType.Line;
                cSeries_Max.MarkerStyle = MarkerStyle.Circle;
                cSeries_Max.IsValueShownAsLabel = false;
                cSeries_Max.Color = Color.Red;

                Series cSeries_Mean = new Series("Mean");
                cSeries_Mean.ChartType = SeriesChartType.Line;
                cSeries_Mean.MarkerStyle = MarkerStyle.Circle;
                cSeries_Mean.IsValueShownAsLabel = false;
                cSeries_Mean.Color = Color.Blue;

                for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
                {
                    DataInfo cDataInfo = cDataInfo_List[nSetIndex];

                    if (bSignedData == true)
                    {
                        cSeries_Max.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cReportStatisticData.m_dMaxNoise_Signed);
                        cSeries_Mean.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cReportStatisticData.m_dMeanNoise_Signed);
                    }
                    else
                    {
                        cSeries_Max.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cReportStatisticData.m_dMaxNoise_Unsigned);
                        cSeries_Mean.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cReportStatisticData.m_dMeanNoise_Unsigned);
                    }
                }

                cChart.Series.Add(cSeries_Max);
                cChart.Series.Add(cSeries_Mean);
            }
            else
            {
                cChart.Legends["Legend"].Position.X = 78;
                cChart.Legends["Legend"].Position.Y = 0;
                cChart.Legends["Legend"].Position.Width = 22;

                for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                {
                    string sTraceIndex = string.Format("Trace={0}", nTraceIndex + 1);
                    Series cSeries = new Series(sTraceIndex);
                    cSeries.ChartType = SeriesChartType.Line;
                    cSeries.MarkerStyle = MarkerStyle.Circle;
                    cSeries.IsValueShownAsLabel = false;
                    cSeries.Color = Color.FromArgb(cChartColor.m_nColorR_List[nTraceIndex], cChartColor.m_nColorG_List[nTraceIndex], cChartColor.m_nColorB_List[nTraceIndex]);

                    for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
                    {
                        DataInfo cDataInfo = cDataInfo_List[nSetIndex];

                        if (sChartType == m_sDATATYPE_PNMODERAWMEAN)
                            cSeries.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cStatisticData.m_dPNModeRawMean_List[nTraceIndex]);
                        else if (sChartType == m_sDATATYPE_PNMODERAWSTD)
                            cSeries.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cStatisticData.m_dPNModeRawStd_List[nTraceIndex]);
                        else if (sChartType == m_sDATATYPE_ADCMEAN)
                            cSeries.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cStatisticData.m_dADCMean_List[nTraceIndex]);
                        else if (sChartType == m_sDATATYPE_ADCSTD)
                            cSeries.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cStatisticData.m_dADCStd_List[nTraceIndex]);
                        else if (sChartType == m_sDATATYPE_PMODERAWMEAN)
                            cSeries.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cStatisticData.m_dPModeRawMean_List[nTraceIndex]);
                        else if (sChartType == m_sDATATYPE_PMODERAWSTD)
                            cSeries.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cStatisticData.m_dPModeRawStd_List[nTraceIndex]);
                        else if (sChartType == m_sDATATYPE_NMODERAWMEAN)
                            cSeries.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cStatisticData.m_dNModeRawMean_List[nTraceIndex]);
                        else if (sChartType == m_sDATATYPE_NMODERAWSTD)
                            cSeries.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_cStatisticData.m_dNModeRawStd_List[nTraceIndex]);
                    }

                    cChart.Series.Add(cSeries);
                }

                if (cDataInfo_List.Count / 22 >= 2 && cDataInfo_List.Count % 22 > 0)
                {
                    cChart.ChartAreas[0].Position.Width = 70;
                    cChart.Legends["Legend"].Position.X = 71;
                    cChart.Legends["Legend"].Position.Width = 29;
                    cChart.Legends["Legend"].Position.Height = 100;
                    cChart.Legends["Legend"].Font = new Font("Times New Roman", 8);
                }
                else if (cDataInfo_List.Count / 14 >= 1 && cDataInfo_List.Count % 14 > 0)
                {
                    cChart.Legends["Legend"].Position.Height = 100;
                    cChart.Legends["Legend"].Font = new Font("Times New Roman", 14);
                }
                else
                {
                    int nHeight = cDataInfo_List.Count * 8;

                    if (cDataInfo_List.Count >= 13)
                        nHeight = cDataInfo_List.Count * 7;

                    cChart.Legends["Legend"].Position.Height = nHeight;
                    cChart.Legends["Legend"].Font = new Font("Times New Roman", 16);
                }
            }

            try
            {
                cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
            }
            catch
            {
                m_sErrorMessage = string.Format("Save Chart File Error[{0}]", sFileName);
                return false;
            }

            return true;
        }

        private ChartColor SetColorList(int nDataCount, bool bRandom = true)
        {
            List<int> nColorR_List = new List<int>();
            List<int> nColorG_List = new List<int>();
            List<int> nColorB_List = new List<int>();


            if (bRandom == true)
            {
                for (int nSetIndex = 0; nSetIndex < nDataCount; nSetIndex++)
                {
                    if (nSetIndex == 0)
                    {
                        nColorR_List.Add(255);
                        nColorG_List.Add(0);
                        nColorB_List.Add(0);
                    }
                    else if (nSetIndex == 1)
                    {
                        nColorR_List.Add(0);
                        nColorG_List.Add(255);
                        nColorB_List.Add(0);
                    }
                    else if (nSetIndex == 2)
                    {
                        nColorR_List.Add(0);
                        nColorG_List.Add(0);
                        nColorB_List.Add(255);
                    }
                    else if (nSetIndex == 3)
                    {
                        nColorR_List.Add(255);
                        nColorG_List.Add(255);
                        nColorB_List.Add(0);
                    }
                    else if (nSetIndex == 4)
                    {
                        nColorR_List.Add(255);
                        nColorG_List.Add(0);
                        nColorB_List.Add(255);
                    }
                    else if (nSetIndex == 5)
                    {
                        nColorR_List.Add(0);
                        nColorG_List.Add(255);
                        nColorB_List.Add(255);
                    }
                    else
                    {
                        Color colorSettingColor = GetRandomColor();
                        nColorR_List.Add(colorSettingColor.R);
                        nColorG_List.Add(colorSettingColor.G);
                        nColorB_List.Add(colorSettingColor.B);
                    }
                }
            }
            else
            {
                int nPartValue = (nDataCount / 3);

                if (nDataCount % 3 != 0)
                    nPartValue++;

                for (int nSetIndex = 0; nSetIndex < nDataCount; nSetIndex++)
                {
                    int nInterval = 255 / nPartValue;

                    if (nSetIndex < nPartValue)
                    {
                        int nR = 255 - (nSetIndex * nInterval);

                        int nG = 255 / 2;

                        if (nSetIndex % 2 == 0)
                            nG = nG + (((nSetIndex / 2) + 1) * nInterval);
                        else
                            nG = nG - (((nSetIndex / 2) + 1) * nInterval);

                        int nB = 0 + (nSetIndex * nInterval);

                        nR = (nR > 255) ? 255 : nR;
                        nG = (nG > 255) ? 255 : nG;
                        nB = (nB > 255) ? 255 : nB;

                        nColorR_List.Add(nR);
                        nColorG_List.Add(nG);
                        nColorB_List.Add(nB);
                    }
                    else if (nSetIndex >= nPartValue && nSetIndex < nPartValue * 2)
                    {
                        int nOffsetIndex = nSetIndex - nPartValue;

                        int nG = 255 - (nOffsetIndex * nInterval);

                        int nB = 255 / 2;

                        if (nOffsetIndex % 2 == 0)
                            nB = nB + (((nOffsetIndex / 2) + 1) * nInterval);
                        else
                            nB = nB - (((nOffsetIndex / 2) + 1) * nInterval);

                        int nR = 0 + (nOffsetIndex * nInterval);

                        nR = (nR > 255) ? 255 : nR;
                        nG = (nG > 255) ? 255 : nG;
                        nB = (nB > 255) ? 255 : nB;

                        nColorR_List.Add(nR);
                        nColorG_List.Add(nG);
                        nColorB_List.Add(nB);
                    }
                    else
                    {
                        int nOffsetIndex = nSetIndex - (nPartValue * 2);

                        int nB = 255 - (nOffsetIndex * nInterval);

                        int nR = 255 / 2;

                        if (nOffsetIndex % 2 == 0)
                            nR = nR + (((nOffsetIndex / 2) + 1) * nInterval);
                        else
                            nR = nR - (((nOffsetIndex / 2) + 1) * nInterval);

                        int nG = 0 + (nOffsetIndex * nInterval);

                        nR = (nR > 255) ? 255 : nR;
                        nG = (nG > 255) ? 255 : nG;
                        nB = (nB > 255) ? 255 : nB;

                        nColorR_List.Add(nR);
                        nColorG_List.Add(nG);
                        nColorB_List.Add(nB);
                    }
                }
            }

            ChartColor cChartColor = new ChartColor();
            cChartColor.m_nColorR_List = nColorR_List;
            cChartColor.m_nColorG_List = nColorG_List;
            cChartColor.m_nColorB_List = nColorB_List;

            return cChartColor;
        }

        private Color GetRandomColor()
        {
            Random cRandomNum_First = new Random(Guid.NewGuid().GetHashCode());
            Random cRandomNum_Second = new Random(Guid.NewGuid().GetHashCode());

            int nColorR = cRandomNum_First.Next(255);
            int nColorG = cRandomNum_Second.Next(255);
            int nColorB = (nColorR + nColorG > 400) ? 0 : 400 - nColorR - nColorG;
            nColorB = (nColorB > 254) ? 254 : nColorB;
            Color colorSettingColor = Color.FromArgb(nColorR, nColorG, nColorB);

            return colorSettingColor;
        }

        private void GetMaxMinValue(ref double dMax, ref double dMin, string sChartType, List<DataInfo> cDataInfo_List, bool bSignedData = true)
        {
            for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
            {
                if (sChartType == m_sDATATYPE_NOISEREFERENCE)
                {
                    if (nSetIndex == 0)
                    {
                        if (bSignedData == true)
                        {
                            double dMaxValue = (cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed > cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Signed) ? cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed : cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Signed;
                            double dMinValue = (cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed < cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Signed) ? cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed : cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Signed;

                            dMax = dMaxValue;
                            dMin = dMinValue;
                        }
                        else
                        {
                            double dMaxValue = (cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Unsigned > cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Unsigned) ? cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed : cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Unsigned;
                            double dMinValue = (cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Unsigned < cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Unsigned) ? cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed : cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Unsigned;

                            dMax = dMaxValue;
                            dMin = dMinValue;
                        }
                    }
                    else
                    {
                        double dMaxValue = 0.0;
                        double dMinValue = 0.0;

                        if (bSignedData == true)
                        {
                            dMaxValue = (cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed > cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Signed) ? cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed : cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Signed;
                            dMinValue = (cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed < cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Signed) ? cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed : cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Signed;
                        }
                        else
                        {
                            dMaxValue = (cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Unsigned > cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Unsigned) ? cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed : cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Unsigned;
                            dMinValue = (cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Unsigned < cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Unsigned) ? cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMaxNoise_Signed : cDataInfo_List[nSetIndex].m_cReportStatisticData.m_dMeanNoise_Unsigned;
                        }

                        if (dMaxValue > dMax)
                            dMax = dMaxValue;

                        if (dMinValue < dMin)
                            dMin = dMinValue;
                    }
                }
                else
                {
                    List<double> dValue_List = new List<double>();

                    if (sChartType == m_sDATATYPE_PNMODERAWMEAN)
                        dValue_List = cDataInfo_List[nSetIndex].m_cStatisticData.m_dPNModeRawMean_List;
                    else if (sChartType == m_sDATATYPE_PNMODERAWSTD)
                        dValue_List = cDataInfo_List[nSetIndex].m_cStatisticData.m_dPNModeRawStd_List;
                    else if (sChartType == m_sDATATYPE_ADCMEAN)
                        dValue_List = cDataInfo_List[nSetIndex].m_cStatisticData.m_dADCMean_List;
                    else if (sChartType == m_sDATATYPE_ADCSTD)
                        dValue_List = cDataInfo_List[nSetIndex].m_cStatisticData.m_dADCStd_List;
                    else if (sChartType == m_sDATATYPE_PMODERAWMEAN)
                        dValue_List = cDataInfo_List[nSetIndex].m_cStatisticData.m_dPModeRawMean_List;
                    else if (sChartType == m_sDATATYPE_PMODERAWSTD)
                        dValue_List = cDataInfo_List[nSetIndex].m_cStatisticData.m_dPModeRawStd_List;
                    else if (sChartType == m_sDATATYPE_NMODERAWMEAN)
                        dValue_List = cDataInfo_List[nSetIndex].m_cStatisticData.m_dNModeRawMean_List;
                    else if (sChartType == m_sDATATYPE_NMODERAWSTD)
                        dValue_List = cDataInfo_List[nSetIndex].m_cStatisticData.m_dNModeRawStd_List;

                    double dMaxValue = dValue_List.Max();
                    double dMinValue = dValue_List.Min();

                    if (nSetIndex == 0)
                    {
                        dMax = dMaxValue;
                        dMin = dMinValue;
                    }
                    else
                    {
                        if (dMaxValue > dMax)
                            dMax = dMaxValue;

                        if (dMinValue < dMin)
                            dMin = dMinValue;
                    }
                }
            }
        }

        private void GetFrequencyHLBounary(ref double dFrequency_HB, ref double dFrequency_LB, List<DataInfo> cDataInfo_List)
        {
            for (int nSetIndex = 0; nSetIndex < cDataInfo_List.Count; nSetIndex++)
            {
                if (nSetIndex == 0)
                {
                    dFrequency_HB = cDataInfo_List[nSetIndex].m_dFrequency;
                    dFrequency_LB = cDataInfo_List[nSetIndex].m_dFrequency;
                }
                else
                {
                    if (cDataInfo_List[nSetIndex].m_dFrequency > dFrequency_HB)
                        dFrequency_HB = cDataInfo_List[nSetIndex].m_dFrequency;

                    if (cDataInfo_List[nSetIndex].m_dFrequency < dFrequency_LB)
                        dFrequency_LB = cDataInfo_List[nSetIndex].m_dFrequency;
                }
            }

            dFrequency_LB = (int)(dFrequency_LB / 5) * 5;
            dFrequency_HB = ((int)(dFrequency_HB / 5) + 1) * 5;
        }

        private bool SavePNModeRawData()
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();

                if (eTraceType == TraceType.RX)
                    cDataInfo_List = m_cDataInfo_RX_List;
                else if (eTraceType == TraceType.TX)
                    cDataInfo_List = m_cDataInfo_TX_List;

                foreach (DataInfo cDataInfo in cDataInfo_List)
                {
                    if (WritePNModeRawData(cDataInfo, eTraceType) == false)
                        return false;
                }
            }

            return true;
        }

        private bool WritePNModeRawData(DataInfo cDataInfo, TraceType eTraceType)
        {
            bool bError = false;
            int nFrameNumber = cDataInfo.m_nPNModeRawData_List.Count;

            string sDirectoryName = MainConstantParameter.m_sDATATYPE_ANALYSIS;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = cDataInfo.m_n_SELF_PH1;
            int nPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cDataInfo.m_n_SELF_PH2E_LAT, cDataInfo.m_n_SELF_PH2E_LMT, cDataInfo.m_n_SELF_PH2_LAT, cDataInfo.m_n_SELF_PH2);
            double dFrequency = cDataInfo.m_dFrequency;

            string sFileName = string.Format("PNModeRaw_{0}_{1}_{2}_{3}", dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2Sum.ToString("x2").ToUpper(),
                                             eTraceType.ToString());

            if (m_bRunCalibrationSequence == true)
                sFileName = string.Format("{0}_P{1:00}N{2:00}", sFileName, cDataInfo.m_nNCPValue, cDataInfo.m_nNCNValue);

            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("PNModeRawData");

                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < cDataInfo.m_nSelf_DFT_NUM; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < cDataInfo.m_nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex == cDataInfo.m_nRXTraceNumber - 1)
                                sw.WriteLine(cDataInfo.m_nPNModeRawData_List[nFrameIndex][nTXIndex, nRXIndex]);
                            else
                                sw.Write(string.Format("{0},", cDataInfo.m_nPNModeRawData_List[nFrameIndex][nTXIndex, nRXIndex]));
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save PNModeRaw Data Error in {0} Frequency={1}KHz(FileName:{2})", eTraceType.ToString(), dFrequency.ToString(), sFileName);
                bError = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bError == true)
                return false;
            else
                return true;
        }

        private bool SaveRawPNPeriodData()
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();

                if (eTraceType == TraceType.RX)
                    cDataInfo_List = m_cDataInfo_RX_List;
                else if (eTraceType == TraceType.TX)
                    cDataInfo_List = m_cDataInfo_TX_List;

                foreach (DataInfo cDataInfo in cDataInfo_List)
                {
                    if (WriteRawPNPeriodData(cDataInfo, eTraceType, RawData.P) == false)
                        return false;

                    if (WriteRawPNPeriodData(cDataInfo, eTraceType, RawData.N) == false)
                        return false;

                    /*
                    if (WriteRawPNPeriodData(cDataInfo, eTraceType, RawData.PNMinus) == false)
                        return false;
                    */

                    if (WriteRawPNPeriodData(cDataInfo, eTraceType, RawData.ConvertADC) == false)
                        return false;
                }
            }

            return true;
        }

        private bool WriteRawPNPeriodData(DataInfo cDataInfo, TraceType eTraceType, RawData eRawDataType)
        {
            bool bError = false;
            int nFrameNumber = cDataInfo.m_nPNModeRawData_List.Count;

            string sDirectoryName = MainConstantParameter.m_sDATATYPE_ANALYSIS;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int[,] nRawPeriodData_Array = null;
            long[,] lRawPNMinusData_Array = null;
            string sDataType = "";

            if (eRawDataType == RawData.P)
            {
                nRawPeriodData_Array = cDataInfo.m_nRawPPeriodData_Array;
                sDataType = "RawPPeriod";
            }
            else if (eRawDataType == RawData.N)
            {
                nRawPeriodData_Array = cDataInfo.m_nRawNPeriodData_Array;
                sDataType = "RawNPeriod";
            }
            else if (eRawDataType == RawData.PNMinus)
            {
                lRawPNMinusData_Array = cDataInfo.m_lRawPNMinusData_Array;
                sDataType = "RawP-N";
            }
            else if (eRawDataType == RawData.ConvertADC)
            {
                nRawPeriodData_Array = cDataInfo.m_nConvertADCData_Array;
                sDataType = "ConvertADC";
            }

            int nPH1 = cDataInfo.m_n_SELF_PH1;
            int nPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cDataInfo.m_n_SELF_PH2E_LAT, cDataInfo.m_n_SELF_PH2E_LMT, cDataInfo.m_n_SELF_PH2_LAT, cDataInfo.m_n_SELF_PH2);
            double dFrequency = cDataInfo.m_dFrequency;

            string sFileName = string.Format("{0}_{1}_{2}_{3}_{4}", sDataType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), 
                                             nPH2Sum.ToString("x2").ToUpper(), eTraceType.ToString());

            if (m_bRunCalibrationSequence == true)
                sFileName = string.Format("{0}_P{1:00}N{2:00}", sFileName, cDataInfo.m_nNCPValue, cDataInfo.m_nNCNValue);

            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(sDataType);

                for (int nTraceIndex = 0; nTraceIndex < cDataInfo.m_nRXTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < cDataInfo.m_nRXTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex));
                    else
                        sw.WriteLine(nTraceIndex);
                }

                if (eRawDataType == RawData.PNMinus)
                {
                    if (lRawPNMinusData_Array != null)
                    {
                        for (int nTXIndex = 0; nTXIndex < lRawPNMinusData_Array.GetLength(1); nTXIndex++)
                        {
                            for (int nRXIndex = 0; nRXIndex < cDataInfo.m_nRXTraceNumber; nRXIndex++)
                            {
                                if (nRXIndex == cDataInfo.m_nRXTraceNumber - 1)
                                    sw.WriteLine(lRawPNMinusData_Array[nRXIndex, nTXIndex]);
                                else
                                    sw.Write(string.Format("{0},", lRawPNMinusData_Array[nRXIndex, nTXIndex]));
                            }
                        }
                    }
                }
                else
                {
                    if (nRawPeriodData_Array != null)
                    {
                        for (int nTXIndex = 0; nTXIndex < nRawPeriodData_Array.GetLength(1); nTXIndex++)
                        {
                            for (int nRXIndex = 0; nRXIndex < cDataInfo.m_nRXTraceNumber; nRXIndex++)
                            {
                                if (nRXIndex == cDataInfo.m_nRXTraceNumber - 1)
                                    sw.WriteLine(nRawPeriodData_Array[nRXIndex, nTXIndex]);
                                else
                                    sw.Write(string.Format("{0},", nRawPeriodData_Array[nRXIndex, nTXIndex]));
                            }
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in {1} Frequency={2}KHz(FileName:{3})", sDataType, eTraceType.ToString(), dFrequency.ToString(), sFileName);
                bError = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bError == true)
                return false;
            else
                return true;
        }

        private bool SaveReportData(bool bSignalData = false)
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();

                if (bSignalData == true)
                {
                    if (eTraceType == TraceType.RX)
                        cDataInfo_List = m_cDataInfo_Signal_RX_List;
                    else if (eTraceType == TraceType.TX)
                        cDataInfo_List = m_cDataInfo_Signal_TX_List;
                }
                else
                {
                    if (eTraceType == TraceType.RX)
                        cDataInfo_List = m_cDataInfo_RX_List;
                    else if (eTraceType == TraceType.TX)
                        cDataInfo_List = m_cDataInfo_TX_List;
                }

                bool bRepeat = false;

                foreach (DataInfo cDataInfo in cDataInfo_List)
                {
                    if (cDataInfo.m_nRepeatIndex > 0)
                    {
                        bRepeat = true;
                        break;
                    }
                }

                foreach (DataInfo cDataInfo in cDataInfo_List)
                {
                    if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                    {
                        if (WriteReportData(cDataInfo, eTraceType, true, bSignalData, bRepeat) == false)
                            return false;
                    }

                    if (m_nComputeValueType == 0 || m_nComputeValueType == 2)
                    {
                        if (WriteReportData(cDataInfo, eTraceType, false, bSignalData, bRepeat) == false)
                            return false;
                    }
                }
            }

            return true;
        }

        private bool WriteReportData(DataInfo cDataInfo, TraceType eTraceType, bool bSigned, bool bSignalData = false, bool bRepeat = false)
        {
            bool bError = false;

            string sDirectoryName = MainConstantParameter.m_sDATATYPE_ANALYSIS;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = cDataInfo.m_n_SELF_PH1;
            int nPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cDataInfo.m_n_SELF_PH2E_LAT, cDataInfo.m_n_SELF_PH2E_LMT, cDataInfo.m_n_SELF_PH2_LAT, cDataInfo.m_n_SELF_PH2);
            double dFrequency = cDataInfo.m_dFrequency;

            int nTraceNumber = cDataInfo.m_nRXTraceNumber;
            int nTraceDataCount = nTraceNumber;

            if (ParamFingerAutoTuning.m_nSelfFSGetReportType == 2)
            {
                if (nTraceNumber >= 20)
                    nTraceDataCount = 20;
            }

            if (ParamFingerAutoTuning.m_nSelfFSGetReportType != 1 && ParamFingerAutoTuning.m_nSelfFSGetReportType != 2)
            {
                nTraceDataCount = nTraceNumber / 2;

                if (nTraceNumber % 2 != 0)
                    nTraceDataCount++;
            }

            string sSignedType = "";
            List<int[]> nReportData_List;

            if (bSigned == true)
            {
                sSignedType = "Signed";
                nReportData_List = cDataInfo.m_nSignedReportData_List;
            }
            else
            {
                sSignedType = "Unsigned";
                nReportData_List = cDataInfo.m_nUnsignedReportData_List;
            }

            string sState = "";

            if (bSignalData == true)
                sState = "(S)";

            string sFileName = "";

            if (bRepeat == true)
            {
                sFileName = string.Format("Report{0}_{1}_{2}_{3}_{4}_{5}_{6}", sState, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2Sum.ToString("x2").ToUpper(),
                                         eTraceType.ToString(), cDataInfo.m_nRepeatIndex, sSignedType);
            }
            else
            {
                sFileName = string.Format("Report{0}_{1}_{2}_{3}_{4}_{5}", sState, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2Sum.ToString("x2").ToUpper(),
                                          eTraceType.ToString(), sSignedType);
            }

            if (m_bRunCalibrationSequence == true)
                sFileName = string.Format("{0}_P{1:00}N{2:00}", sFileName, cDataInfo.m_nNCPValue, cDataInfo.m_nNCNValue);

            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("Report Data");

                for (int nTraceIndex = 0; nTraceIndex < nTraceDataCount; nTraceIndex++)
                {
                    if (ParamFingerAutoTuning.m_nSelfFSGetReportType != 1 && ParamFingerAutoTuning.m_nSelfFSGetReportType != 2)
                    {
                        if (nTraceIndex == 0)
                            sw.Write(string.Format(",{0},", nTraceIndex * 2));
                        else if (nTraceIndex == nTraceDataCount - 1)
                            sw.WriteLine(nTraceIndex * 2);
                        else
                            sw.Write(string.Format("{0},", nTraceIndex * 2));
                    }
                    else
                    {
                        if (nTraceIndex == 0)
                            sw.Write(string.Format(",{0},", nTraceIndex));
                        else if (nTraceIndex == nTraceDataCount - 1)
                            sw.WriteLine(nTraceIndex);
                        else
                            sw.Write(string.Format("{0},", nTraceIndex));
                    }
                }

                for (int nReportIndex = 0; nReportIndex < nReportData_List.Count; nReportIndex++)
                {
                    sw.Write(string.Format("{0},", nReportIndex));

                    for (int nTraceIndex = 0; nTraceIndex < nTraceDataCount; nTraceIndex++)
                    {
                        if (nTraceIndex == nTraceDataCount - 1)
                            sw.WriteLine(nReportData_List[nReportIndex][nTraceIndex]);
                        else
                            sw.Write(string.Format("{0},", nReportData_List[nReportIndex][nTraceIndex]));
                    }
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Report{1} Data Error in {2} Frequency={3}KHz(FileName:{4})", sSignedType, sState, eTraceType.ToString(),
                                                dFrequency.ToString(), sFileName);
                bError = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bError == true)
                return false;
            else
                return true;
        }

        private bool AnalysisReportData()
        {
            ComputeReportStatisticData();

            if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1)
                ComputeReportStatisticData(true);

            return true;
        }

        private void ComputeReportStatisticData(bool bSignalData = false)
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();

                if (bSignalData == true)
                {
                    if (eTraceType == TraceType.RX)
                        cDataInfo_List = m_cDataInfo_Signal_RX_List;
                    else if (eTraceType == TraceType.TX)
                        cDataInfo_List = m_cDataInfo_Signal_TX_List;
                }
                else
                {
                    if (eTraceType == TraceType.RX)
                        cDataInfo_List = m_cDataInfo_RX_List;
                    else if (eTraceType == TraceType.TX)
                        cDataInfo_List = m_cDataInfo_TX_List;
                }

                foreach (DataInfo cDataInfo in cDataInfo_List)
                {
                    int nTraceNumber = cDataInfo.m_nRXTraceNumber;
                    int nTraceDataCount = nTraceNumber;

                    if (ParamFingerAutoTuning.m_nSelfFSGetReportType == 2)
                    {
                        if (nTraceNumber >= 20)
                            nTraceDataCount = 20;
                    }

                    if (ParamFingerAutoTuning.m_nSelfFSGetReportType != 1 && ParamFingerAutoTuning.m_nSelfFSGetReportType != 2)
                    {
                        nTraceDataCount = nTraceNumber / 2;

                        if (nTraceNumber % 2 != 0)
                            nTraceDataCount++;
                    }

                    for (int nTraceIndex = 0; nTraceIndex < nTraceDataCount; nTraceIndex++)
                    {
                        List<int> nReportData_List = new List<int>();

                        if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                        {
                            nReportData_List.Clear();

                            for (int nReportIndex = 0; nReportIndex < cDataInfo.m_nSignedReportData_List.Count; nReportIndex++)
                                nReportData_List.Add(cDataInfo.m_nSignedReportData_List[nReportIndex][nTraceIndex]);

                            double dMean = Math.Round(nReportData_List.Average(), 3, MidpointRounding.AwayFromZero);
                            double dStd = Math.Round(MathMethod.ComputeStd(nReportData_List), 3, MidpointRounding.AwayFromZero);
                            double dMedian = Math.Round(GetMedian(nReportData_List), 1, MidpointRounding.AwayFromZero);
                            int nMax = nReportData_List.Max();
                            int nMin = nReportData_List.Min();

                            cDataInfo.m_cReportStatisticData.m_dMean_Signed_List.Add(dMean);
                            cDataInfo.m_cReportStatisticData.m_dStd_Signed_List.Add(dStd);
                            cDataInfo.m_cReportStatisticData.m_dMedian_Signed_List.Add(dMedian);
                            cDataInfo.m_cReportStatisticData.m_nMax_Signed_List.Add(nMax);
                            cDataInfo.m_cReportStatisticData.m_nMin_Signed_List.Add(nMin);
                        }

                        if (m_nComputeValueType == 0 || m_nComputeValueType == 2)
                        {
                            nReportData_List.Clear();

                            for (int nReportIndex = 0; nReportIndex < cDataInfo.m_nUnsignedReportData_List.Count; nReportIndex++)
                                nReportData_List.Add(cDataInfo.m_nUnsignedReportData_List[nReportIndex][nTraceIndex]);

                            double dMean = Math.Round(nReportData_List.Average(), 3, MidpointRounding.AwayFromZero);
                            double dStd = Math.Round(MathMethod.ComputeStd(nReportData_List), 3, MidpointRounding.AwayFromZero);
                            double dMedian = Math.Round(GetMedian(nReportData_List), 1, MidpointRounding.AwayFromZero);
                            int nMax = nReportData_List.Max();
                            int nMin = nReportData_List.Min();

                            cDataInfo.m_cReportStatisticData.m_dMean_Unsigned_List.Add(dMean);
                            cDataInfo.m_cReportStatisticData.m_dStd_Unsigned_List.Add(dStd);
                            cDataInfo.m_cReportStatisticData.m_dMedian_Unsigned_List.Add(dMedian);
                            cDataInfo.m_cReportStatisticData.m_nMax_Unsigned_List.Add(nMax);
                            cDataInfo.m_cReportStatisticData.m_nMin_Unsigned_List.Add(nMin);
                        }
                    }

                    if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                    {
                        double dMeanStd = Math.Round(cDataInfo.m_cReportStatisticData.m_dStd_Signed_List.Average(), 3, MidpointRounding.AwayFromZero);
                        double dMeanNoise = dMeanStd;
                        cDataInfo.m_cReportStatisticData.m_dMeanNoise_Signed = dMeanNoise;
                    }

                    if (m_nComputeValueType == 0 || m_nComputeValueType == 2)
                    {
                        double dMeanStd = Math.Round(cDataInfo.m_cReportStatisticData.m_dStd_Unsigned_List.Average(), 3, MidpointRounding.AwayFromZero);
                        double dMeanNoise = dMeanStd;
                        cDataInfo.m_cReportStatisticData.m_dMeanNoise_Unsigned = dMeanNoise;
                    }
                }

                int nMaxNoiseTraceIndex_Signed = 0;
                double dMaxNoise_Signed = 0.0;
                int nMaxNoiseTraceIndex_Unsigned = 0;
                double dMaxNoise_Unsigned = 0.0;
                bool bGetData_Signed = false;
                bool bGetData_Unsigned = false;

                foreach (DataInfo cDataInfo in cDataInfo_List)
                {
                    int nTraceNumber = cDataInfo.m_nRXTraceNumber;
                    int nTraceDataCount = nTraceNumber;

                    if (ParamFingerAutoTuning.m_nSelfFSGetReportType != 1)
                    {
                        nTraceDataCount = nTraceNumber / 2;

                        if (nTraceNumber % 2 != 0)
                            nTraceDataCount++;
                    }

                    for (int nTraceIndex = 0; nTraceIndex < nTraceDataCount; nTraceIndex++)
                    {
                        if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                        {
                            if (bGetData_Signed == false && nTraceIndex == 0)
                            {
                                nMaxNoiseTraceIndex_Signed = nTraceIndex;
                                dMaxNoise_Signed = cDataInfo.m_cReportStatisticData.m_dStd_Signed_List[nTraceIndex];
                                bGetData_Signed = true;
                            }
                            else if (cDataInfo.m_cReportStatisticData.m_dStd_Signed_List[nTraceIndex] > dMaxNoise_Signed)
                            {
                                nMaxNoiseTraceIndex_Signed = nTraceIndex;
                                dMaxNoise_Signed = cDataInfo.m_cReportStatisticData.m_dStd_Signed_List[nTraceIndex];
                            }
                        }

                        if (m_nComputeValueType == 0 || m_nComputeValueType == 2)
                        {
                            if (bGetData_Unsigned == false && nTraceIndex == 0)
                            {
                                nMaxNoiseTraceIndex_Unsigned = nTraceIndex;
                                dMaxNoise_Unsigned = cDataInfo.m_cReportStatisticData.m_dStd_Unsigned_List[nTraceIndex];
                                bGetData_Unsigned = true;
                            }
                            else if (cDataInfo.m_cReportStatisticData.m_dStd_Unsigned_List[nTraceIndex] > dMaxNoise_Unsigned)
                            {
                                nMaxNoiseTraceIndex_Unsigned = nTraceIndex;
                                dMaxNoise_Unsigned = cDataInfo.m_cReportStatisticData.m_dStd_Unsigned_List[nTraceIndex];
                            }
                        }
                    }
                }

                if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                {
                    foreach (DataInfo cDataInfo in cDataInfo_List)
                    {
                        cDataInfo.m_cReportStatisticData.m_dMaxNoise_Signed = Math.Round(cDataInfo.m_cReportStatisticData.m_dStd_Signed_List[nMaxNoiseTraceIndex_Signed],
                                                                                         3, MidpointRounding.AwayFromZero);
                    }
                }

                if (m_nComputeValueType == 0 || m_nComputeValueType == 2)
                {
                    foreach (DataInfo cDataInfo in cDataInfo_List)
                    {
                        cDataInfo.m_cReportStatisticData.m_dMaxNoise_Unsigned = Math.Round(cDataInfo.m_cReportStatisticData.m_dStd_Signed_List[nMaxNoiseTraceIndex_Unsigned],
                                                                                           3, MidpointRounding.AwayFromZero);
                    }
                }
            }
        }

        private double GetMedian(List<int> nSource_List)
        {
            // Create a copy of the input, and sort the copy
            int[] nData_Array = nSource_List.ToArray();
            Array.Sort(nData_Array);
            int nCount = nData_Array.Length;

            if (nCount == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (nCount % 2 == 0)
            {
                // count is even, average two middle elements
                int nValue1 = nData_Array[nCount / 2 - 1];
                int nValue2 = nData_Array[nCount / 2];
                return (double)(nValue1 + nValue2) / 2;
            }
            else
            {
                // count is odd, return the middle element
                return nData_Array[nCount / 2];
            }
        }

        private bool SaveReportStatisticFile(bool bSignalData = false)
        {
            bool bError = false;

            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();
                int nTraceNumber = 0;

                if (bSignalData == true)
                {
                    if (eTraceType == TraceType.RX)
                    {
                        cDataInfo_List = m_cDataInfo_Signal_RX_List;
                        nTraceNumber = m_nTraceNumber_RX;
                    }
                    else if (eTraceType == TraceType.TX)
                    {
                        cDataInfo_List = m_cDataInfo_Signal_TX_List;
                        nTraceNumber = m_nTraceNumber_TX;
                    }
                }
                else
                {
                    if (eTraceType == TraceType.RX)
                    {
                        cDataInfo_List = m_cDataInfo_RX_List;
                        nTraceNumber = m_nTraceNumber_RX;
                    }
                    else if (eTraceType == TraceType.TX)
                    {
                        cDataInfo_List = m_cDataInfo_TX_List;
                        nTraceNumber = m_nTraceNumber_TX;
                    }
                }

                int nTraceDataCount = nTraceNumber;

                if (ParamFingerAutoTuning.m_nSelfFSGetReportType == 2)
                {
                    if (nTraceNumber >= 20)
                        nTraceDataCount = 20;
                }

                if (ParamFingerAutoTuning.m_nSelfFSGetReportType != 1 && ParamFingerAutoTuning.m_nSelfFSGetReportType != 2)
                {
                    nTraceDataCount = nTraceNumber / 2;

                    if (nTraceNumber % 2 != 0)
                        nTraceDataCount++;
                }

                m_nCompareOperator = m_nCOMPARE_Frequency;
                cDataInfo_List.Sort(new InfoDataComparer());

                string sState = "";

                if (bSignalData == true)
                    sState = "(S)";

                string sReportFilePath = string.Format(@"{0}\ReportStatistic{1}_{2}.csv", m_sLogDirectoryPath, sState, eTraceType.ToString());

                FileStream fs = new FileStream(sReportFilePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                List<string> sColumnHeader_List = new List<string>() { 
                    "Index", 
                    "PH1", 
                    "PH2E_LMT", 
                    "PH2_LAT", 
                    "PH2", 
                    "Frequency(KHz)", 
                    "Sum", 
                    "Gain", 
                    "CAG", 
                    "IQ_BSH",
                    "NCP", 
                    "NCN", 
                    "CAL", 
                    "RepeatIndex" 
                };

                for (int nTraceIndex = 0; nTraceIndex < nTraceDataCount; nTraceIndex++)
                {
                    if (ParamFingerAutoTuning.m_nSelfFSGetReportType != 1 && ParamFingerAutoTuning.m_nSelfFSGetReportType != 2)
                    {
                        sColumnHeader_List.Add((nTraceIndex * 2).ToString());
                    }
                    else
                        sColumnHeader_List.Add(nTraceIndex.ToString());
                }

                List<string> sColumnHeader_Std_List = new List<string>(sColumnHeader_List);
                sColumnHeader_Std_List.Add("Max");
                sColumnHeader_Std_List.Add("Mean");

                try
                {
                    Write_Tool_Information(sw, m_sFILETYPE_REPORT_STATISTIC);

                    if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                    {
                        WriteReportStatisticData(sw, sColumnHeader_List, cDataInfo_List, nTraceDataCount, m_sDATATYPE_ADCMEAN, true);

                        sw.WriteLine();

                        WriteReportStatisticData(sw, sColumnHeader_Std_List, cDataInfo_List, nTraceDataCount, m_sDATATYPE_ADCSTD, true);

                        sw.WriteLine();

                        WriteReportStatisticData(sw, sColumnHeader_List, cDataInfo_List, nTraceDataCount, m_sDATATYPE_ADCMEDIAN, true);
                    }

                    if (m_nComputeValueType == 0 || m_nComputeValueType == 2)
                    {
                        if (m_nComputeValueType == 2)
                            sw.WriteLine();

                        WriteReportStatisticData(sw, sColumnHeader_List, cDataInfo_List, nTraceDataCount, m_sDATATYPE_ADCMEAN, false);

                        sw.WriteLine();

                        WriteReportStatisticData(sw, sColumnHeader_Std_List, cDataInfo_List, nTraceDataCount, m_sDATATYPE_ADCSTD, false);

                        sw.WriteLine();

                        WriteReportStatisticData(sw, sColumnHeader_List, cDataInfo_List, nTraceDataCount, m_sDATATYPE_ADCMEDIAN, false);
                    }

                    if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                    {
                        sw.WriteLine();

                        WriteReportStatisticData(sw, sColumnHeader_List, cDataInfo_List, nTraceDataCount, m_sDATATYPE_ADCMAX, true);

                        sw.WriteLine();

                        WriteReportStatisticData(sw, sColumnHeader_List, cDataInfo_List, nTraceDataCount, m_sDATATYPE_ADCMIN, true);
                    }

                    if (m_nComputeValueType == 0 || m_nComputeValueType == 2)
                    {
                        sw.WriteLine();

                        WriteReportStatisticData(sw, sColumnHeader_List, cDataInfo_List, nTraceDataCount, m_sDATATYPE_ADCMAX, false);

                        sw.WriteLine();

                        WriteReportStatisticData(sw, sColumnHeader_List, cDataInfo_List, nTraceDataCount, m_sDATATYPE_ADCMIN, false);
                    }
                }
                catch (IOException ex)
                {
                    string sMessage = ex.Message;

                    m_sErrorMessage = string.Format("Save {0} Report{1} Statistic Data Error", eTraceType.ToString(), sState);
                    bError = true;
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }

                if (bError == true)
                    break;
            }

            if (bError == true)
                return false;
            else
                return true;
        }

        private void WriteReportStatisticData(StreamWriter sw, List<string> sColumnHeader_List, List<DataInfo> cDataInfo_List, int nTraceDataCount, string sDataType,
                                              bool bSigned)
        {
            string sTitleName = "";
            string sValueType = "";

            if (sDataType == m_sDATATYPE_ADCMEAN)
                sValueType = "Mean";
            else if (sDataType == m_sDATATYPE_ADCSTD)
                sValueType = "Std";
            else if (sDataType == m_sDATATYPE_ADCMEDIAN)
                sValueType = "Median";
            else if (sDataType == m_sDATATYPE_ADCMAX)
                sValueType = "Max";
            else if (sDataType == m_sDATATYPE_ADCMIN)
                sValueType = "Min";

            if (bSigned == true)
                sTitleName = string.Format("Report {0} Data(Signed)", sValueType);
            else
                sTitleName = string.Format("Report {0} Data(Unsigned)", sValueType);

            sw.WriteLine(sTitleName);

            for (int nColumnIndex = 0; nColumnIndex < sColumnHeader_List.Count; nColumnIndex++)
            {
                if (nColumnIndex != sColumnHeader_List.Count - 1)
                    sw.Write(string.Format("{0},", sColumnHeader_List[nColumnIndex]));
                else
                    sw.WriteLine(sColumnHeader_List[nColumnIndex]);
            }

            for (int nDataIndex = 0; nDataIndex < cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = cDataInfo_List[nDataIndex];

                List<double> dValue_List = new List<double>();
                List<int> nValue_List = new List<int>();

                if (bSigned == true)
                {
                    if (sDataType == m_sDATATYPE_ADCMEAN)
                        dValue_List = cDataInfo.m_cReportStatisticData.m_dMean_Signed_List;
                    else if (sDataType == m_sDATATYPE_ADCSTD)
                        dValue_List = cDataInfo.m_cReportStatisticData.m_dStd_Signed_List;
                    else if (sDataType == m_sDATATYPE_ADCMEDIAN)
                        dValue_List = cDataInfo.m_cReportStatisticData.m_dMedian_Signed_List;
                    else if (sDataType == m_sDATATYPE_ADCMAX)
                        nValue_List = cDataInfo.m_cReportStatisticData.m_nMax_Signed_List;
                    else if (sDataType == m_sDATATYPE_ADCMIN)
                        nValue_List = cDataInfo.m_cReportStatisticData.m_nMin_Signed_List;
                }
                else
                {
                    if (sDataType == m_sDATATYPE_ADCMEAN)
                        dValue_List = cDataInfo.m_cReportStatisticData.m_dMean_Unsigned_List;
                    else if (sDataType == m_sDATATYPE_ADCSTD)
                        dValue_List = cDataInfo.m_cReportStatisticData.m_dStd_Unsigned_List;
                    else if (sDataType == m_sDATATYPE_ADCMEDIAN)
                        dValue_List = cDataInfo.m_cReportStatisticData.m_dMedian_Unsigned_List;
                    else if (sDataType == m_sDATATYPE_ADCMAX)
                        nValue_List = cDataInfo.m_cReportStatisticData.m_nMax_Unsigned_List;
                    else if (sDataType == m_sDATATYPE_ADCMIN)
                        nValue_List = cDataInfo.m_cReportStatisticData.m_nMin_Unsigned_List;
                }

                sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2.ToString("x2").ToUpper()));
                sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_DFT_NUM.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_Gain.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_CAG.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_IQ_BSH.ToString()));

                if (m_bRunCalibrationSequence == true)
                {
                    sw.Write(string.Format("{0},", cDataInfo.m_nNCPValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nNCNValue.ToString()));
                }
                else
                {
                    sw.Write("NA,");
                    sw.Write("NA,");
                }

                if (m_bSetCAL == true)
                    sw.Write(string.Format("{0},", cDataInfo.m_nCALValue.ToString()));
                else
                    sw.Write("NA,");

                sw.Write(string.Format("{0},", cDataInfo.m_nRepeatIndex.ToString()));

                if (sDataType == m_sDATATYPE_ADCMAX || sDataType == m_sDATATYPE_ADCMIN)
                {
                    for (int nTraceIndex = 0; nTraceIndex < nTraceDataCount; nTraceIndex++)
                    {
                        if (nTraceIndex == nTraceDataCount - 1)
                            sw.WriteLine(string.Format("{0}", nValue_List[nTraceIndex]));
                        else
                            sw.Write(string.Format("{0},", nValue_List[nTraceIndex]));
                    }
                }
                else if (sDataType == m_sDATATYPE_ADCSTD)
                {
                    for (int nTraceIndex = 0; nTraceIndex < nTraceDataCount; nTraceIndex++)
                        sw.Write(string.Format("{0},", dValue_List[nTraceIndex]));

                    if (bSigned == true)
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_cReportStatisticData.m_dMaxNoise_Signed));
                        sw.WriteLine(string.Format("{0}", cDataInfo.m_cReportStatisticData.m_dMeanNoise_Signed));
                    }
                    else
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_cReportStatisticData.m_dMaxNoise_Unsigned));
                        sw.WriteLine(string.Format("{0}", cDataInfo.m_cReportStatisticData.m_dMeanNoise_Unsigned));
                    }
                }
                else
                {
                    for (int nTraceIndex = 0; nTraceIndex < nTraceDataCount; nTraceIndex++)
                    {
                        if (nTraceIndex == nTraceDataCount - 1)
                            sw.WriteLine(string.Format("{0}", dValue_List[nTraceIndex]));
                        else
                            sw.Write(string.Format("{0},", dValue_List[nTraceIndex]));
                    }
                }
            }
        }

        private bool SaveReportLineChart()
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();
                int nTraceNumber = 0;

                if (eTraceType == TraceType.RX)
                {
                    cDataInfo_List = m_cDataInfo_RX_List;
                    nTraceNumber = m_nTraceNumber_RX;
                }
                else if (eTraceType == TraceType.TX)
                {
                    cDataInfo_List = m_cDataInfo_TX_List;
                    nTraceNumber = m_nTraceNumber_TX;
                }

                m_nCompareOperator = m_nCOMPARE_Frequency;
                cDataInfo_List.Sort(new InfoDataComparer());

                string sDirectoryPath = string.Format(@"{0}\{1}\{2}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART, eTraceType.ToString());

                if (Directory.Exists(sDirectoryPath) == false)
                    Directory.CreateDirectory(sDirectoryPath);

                double dFrequency_LB = 0.0;
                double dFrequency_HB = 0.0;

                GetFrequencyHLBounary(ref dFrequency_HB, ref dFrequency_LB, cDataInfo_List);

                ChartColor cChartColor = new ChartColor();

                if (m_nComputeValueType == 0 || m_nComputeValueType == 2)
                {
                    if (CreateLineChartByFrequency(m_sDATATYPE_NOISEREFERENCE, eTraceType, sDirectoryPath, cDataInfo_List, cChartColor, dFrequency_HB, dFrequency_LB,
                                                   nTraceNumber, false) == false)
                        return false;
                }

                if (m_nComputeValueType == 1 || m_nComputeValueType == 2)
                {
                    if (CreateLineChartByFrequency(m_sDATATYPE_NOISEREFERENCE, eTraceType, sDirectoryPath, cDataInfo_List, cChartColor, dFrequency_HB, dFrequency_LB,
                                                   nTraceNumber, true) == false)
                        return false;
                }
            }

            return true;
        }

        private int ConvertBSHToMagnification(int nIQ_BSH)
        {
            int nMagnification = 32;
            int nBSHDiffer = nIQ_BSH - 19;

            if (nBSHDiffer < 0)
            {
                for (int nIndex = nBSHDiffer; nIndex < 0; nIndex++)
                    nMagnification = nMagnification / 2;
            }
            else if (nBSHDiffer > 0)
            {
                for (int nIndex = 0; nIndex < nBSHDiffer; nIndex++)
                    nMagnification = nMagnification * 2;
            }

            return nMagnification;
        }
    }
}
