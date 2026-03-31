using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using MathNet.Numerics;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Data;
using System.Text.RegularExpressions;
using FingerAutoTuningParameter;
using Elan;

namespace FingerAutoTuning
{
    public class AnalysisFlow_ACFR : AnalysisFlow
    {
        private const int m_nVALUETYPE_Hex = 0;
        private const int m_nVALUETYPE_Int = 1;

        private const int m_nDIFFERTYPE_RX          = 0;
        private const int m_nDIFFERTYPE_TX          = 1;
        private const int m_nDIFFERTYPE_TX_RXDiffer = 2;

        private int m_nTXTraceNumber = 0;
        private int m_nRXTraceNumber = 0;
        private int m_nMinPH1 = 0x10;
        private int m_nPH2LB = 0x00;
        private bool m_bEnableTXn = false;

        private const int m_nSNRMethod_Origin   = 0;
        private const int m_nSNRMethod_Sqrt     = 1;
        private int m_nSNRMethodType = m_nSNRMethod_Sqrt;

        private int m_nMultiFrameCount_Horizontal = 10;
        //private double m_dSTDTHRatio_Horizontal  = 0.7;
        //private double m_dSTDTHRatio_Vertical = 1.0;
        private double m_dSignalAreaPercentage_Horizontal = 70.0;
        private double m_dSignalAreaPercentage_Vertical = 70.0;
        private double m_dNoneSignalAreaPercentage = 30.0;

        private int m_nADCTestFrame = 300;

        private int m_nChartSNRValueHB = 0;
        private int m_nChartSNRValueLB = 0;
        private int m_nChartSNRValueInterval = 0;

        private enum FilterMaskType
        {
            Type01_Sobel_3x3,
            Type02_Sobel_3x3_Strong,
            Type03_Another_Sharp,
            Type04_Another_Smooth,
            Type05_Identical,
            Type06_Roberts,
            Type07_Sobel_5x5_Smooth,
            Type08_Sobel_5x5_Sharp,
            Type09_Another_Strong_Sharp
        }

        private enum GetSignalAreaMethod
        {
            UseBaseDifferEdgeDetect,  //New Method
            UseDVEdgeDetect,          //Old Method
            ByUserDefine
        }

        private enum UseBaseDifferEdgeDetectMethod
        {
            UseConvolutionG,
            UseConvolutionGxGy
        }

        private FilterMaskType m_eFilterMaskType = FilterMaskType.Type08_Sobel_5x5_Sharp;

        private GetSignalAreaMethod m_eGetSignalAreaMethod = GetSignalAreaMethod.UseBaseDifferEdgeDetect;
        private UseBaseDifferEdgeDetectMethod m_eUseBaseDifferEdgeDetectMethod = UseBaseDifferEdgeDetectMethod.UseConvolutionG;
        private bool m_bUseTotalDataGetSignalTrace = false;
        private int m_nSignalRXStartTrace = 0;
        private int m_nSignalRXEndTrace = 0;
        private int m_nSignalTXStartTrace = 0;
        private int m_nSignalTXEndTrace = 0;

        private double m_dThresholdStdRatio = 1.0;

        private bool[] m_bTotalTraceVerticalFlag_Array = null;
        private bool[] m_bTotalTraceHorizontalFlag_Array = null;

        public class DataInfo
        {
            public string m_sBASEFileName = "";
            public string m_sOBASEFileName = "";
            public string m_sADCFileName = "";
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
            public int[,] m_nOBASEData_Array = null;

            //public int[,,] m_nBASEMinusADC_Array = null;
            //public int[,,] m_nOBASEMinusADC_Array = null;

            //public double[] m_dFrameStd_Array = null;
            //public double[] m_dFrameStdHorizontalTheshold_Array = null;
            //public double[] m_dFrameStdVerticalThreshold_Array = null;
            //public double[,] m_dVerticalStd_Array = null;
            //public double[,] m_dHorizontalStd_Array = null;
            public bool[,] m_bFrameVerticalFlag_Array = null;
            public bool[,] m_bFrameHorizontalFlag_Array = null;

            public bool[] m_bTraceVerticalFlag_Array = null;
            public bool[] m_bTraceHorizontalFlag_Array = null;

            public PreReportInfo m_cPreReportInfo = new PreReportInfo();

            public NoiseReferenceData m_cACNoiseReferenceData = new NoiseReferenceData();
            public NoiseReferenceData m_cLCMNoiseReferenceData = new NoiseReferenceData();

            public double m_dSignalReferenceValue = 0.0;

            public double m_dRawSignalValue_SignalToACNoise = 0.0;
            public double m_dRawNoiseValue_SignalToACNoise = 0.0;
            public double m_dRawSNRValue_SignalToACNoise = 0.0;
            public double m_dSNRValue_SignalToACNoise = 0.0;
            public double m_dRawSignalValue_SignalToLCMNoise = 0.0;
            public double m_dRawNoiseValue_SignalToLCMNoise = 0.0;
            public double m_dRawSNRValue_SignalToLCMNoise = 0.0;
            public double m_dSNRValue_SignalToLCMNoise = 0.0;
            public double m_dRawSNRValue_Compostite = 0.0;
            public double m_dSNRValue_Compostite = 0.0;
        }

        public class DataInfoComparer : IComparer<DataInfo>
        {
            public int Compare(DataInfo cDataInfo1, DataInfo cDataInfo2)
            {
                if (m_nCompareOperator == m_nCOMPARE_Frequency)
                    return cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency);
                else if (m_nCompareOperator == m_nCOMPARE_Normalize)
                {
                    if (-cDataInfo1.m_dSNRValue_Compostite.CompareTo(cDataInfo2.m_dSNRValue_Compostite) != 0)
                        return -cDataInfo1.m_dSNRValue_Compostite.CompareTo(cDataInfo2.m_dSNRValue_Compostite);
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

        public class PreReportInfo
        {
            public bool m_bGetInfo = false;

            public double m_dSignalReference = 0.0;

            public double m_dNoiseMean = 0.0;
            public double m_dNoiseStd = 0.0;

            public double m_dNoisePositiveReference = 0.0;
            public double m_dNoiseNegativeReference = 0.0;

            public double m_dNoiseReference = 0.0;
        }

        public class NoiseReferenceData
        {
            public double m_dNoiseMean = 0.0;
            public double m_dNoiseStd = 0.0;

            public double m_dNoisePositiveReference = 0.0;
            public double m_dNoiseNegativeReference = 0.0;
        }

        public class VerticalMeanDifferInfo
        {
            public int m_nTrace = 0;
            public double m_dPreviousMeanValue = 0.0;
            public double m_dCurrentMeanValue = 0.0;
            public double m_dNextMeanValue = 0.0;
        }

        public class SignalAreaInfo
        {
            public SignalTraceInfo m_cTXSignalTraceInfo = new SignalTraceInfo();
            public SignalTraceInfo m_cRXSignalTraceInfo = new SignalTraceInfo();

            public int m_nNumber = 0;
            public double m_dReferenceValue = 0.0;
        }

        public class SignalTraceInfo
        {
            public int m_nStart = 0;
            public int m_nEnd = 0;
        }

        public class ConvGStdReference
        {
            public double[] m_dStd_Original_Array = null;
            public double[] m_dStd_NearMean_Array = null;

            public double m_dThreshold = 0.0;
            public double m_dStd = 0.0;
            public double m_dMedian = 0.0;
            public double m_dMean = 0.0;
            public double m_dMin = 0.0;
            public double m_dMeanUnderMedian = 0.0;
        }

        public AnalysisFlow_ACFR(frmMain.FlowStep cFlowStep, string sLogDirectoryPath, string sH5LogDirectoryPath, bool bGenerateH5Data, frmMain cfrmParent, string sProjectName)
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
            m_nMultiFrameCount_Horizontal = ParamFingerAutoTuning.m_nACFRMultiFrameCount_Hor;
            //m_dSTDTHRatio_Horizontal = ParamFingerAutoTuning.m_dACFRSTDTHRatio_Hor;
            //m_dSTDTHRatio_Vertical = ParamFingerAutoTuning.m_dACFRSTDTHRatio_Ver;
            m_dSignalAreaPercentage_Horizontal = ParamFingerAutoTuning.m_dACFRSignalAreaPtg_Hor;
            m_dSignalAreaPercentage_Vertical = ParamFingerAutoTuning.m_dACFRSignalAreaPtg_Ver;
            m_dNoneSignalAreaPercentage = ParamFingerAutoTuning.m_dACFRNonSignalAreaPtg;

            if (ParamFingerAutoTuning.m_nACFRSNRMethodType == 1)
                m_nSNRMethodType = m_nSNRMethod_Sqrt;
            else
                m_nSNRMethodType = m_nSNRMethod_Origin;

            if (ParamFingerAutoTuning.m_nACFRGetSignalAreaMethod == 1)
                m_eGetSignalAreaMethod = GetSignalAreaMethod.UseBaseDifferEdgeDetect;
            else if (ParamFingerAutoTuning.m_nACFRGetSignalAreaMethod == 2)
                m_eGetSignalAreaMethod = GetSignalAreaMethod.UseDVEdgeDetect;
            else if (ParamFingerAutoTuning.m_nACFRGetSignalAreaMethod == 3)
                m_eGetSignalAreaMethod = GetSignalAreaMethod.ByUserDefine;
            else
                m_eGetSignalAreaMethod = GetSignalAreaMethod.UseBaseDifferEdgeDetect;

            if (ParamFingerAutoTuning.m_nACFRUseBaseDifferEdgeDetectMethod == 1)
                m_eUseBaseDifferEdgeDetectMethod = UseBaseDifferEdgeDetectMethod.UseConvolutionG;
            else if (ParamFingerAutoTuning.m_nACFRUseBaseDifferEdgeDetectMethod == 2)
                m_eUseBaseDifferEdgeDetectMethod = UseBaseDifferEdgeDetectMethod.UseConvolutionGxGy;

            if (ParamFingerAutoTuning.m_nACFRUseTotalDataGetSignalTrace == 0)
                m_bUseTotalDataGetSignalTrace = false;
            else if (ParamFingerAutoTuning.m_nACFRUseTotalDataGetSignalTrace == 1)
                m_bUseTotalDataGetSignalTrace = true;

            if (ParamFingerAutoTuning.m_nACFRFilterMaskType == 1)
                m_eFilterMaskType = FilterMaskType.Type01_Sobel_3x3;
            else if (ParamFingerAutoTuning.m_nACFRFilterMaskType == 2)
                m_eFilterMaskType = FilterMaskType.Type02_Sobel_3x3_Strong;
            else if (ParamFingerAutoTuning.m_nACFRFilterMaskType == 3)
                m_eFilterMaskType = FilterMaskType.Type03_Another_Sharp;
            else if (ParamFingerAutoTuning.m_nACFRFilterMaskType == 4)
                m_eFilterMaskType = FilterMaskType.Type04_Another_Smooth;
            else if (ParamFingerAutoTuning.m_nACFRFilterMaskType == 5)
                m_eFilterMaskType = FilterMaskType.Type05_Identical;
            else if (ParamFingerAutoTuning.m_nACFRFilterMaskType == 6)
                m_eFilterMaskType = FilterMaskType.Type06_Roberts;
            else if (ParamFingerAutoTuning.m_nACFRFilterMaskType == 7)
                m_eFilterMaskType = FilterMaskType.Type07_Sobel_5x5_Smooth;
            else if (ParamFingerAutoTuning.m_nACFRFilterMaskType == 8)
                m_eFilterMaskType = FilterMaskType.Type08_Sobel_5x5_Sharp;
            else if (ParamFingerAutoTuning.m_nACFRFilterMaskType == 9)
                m_eFilterMaskType = FilterMaskType.Type09_Another_Strong_Sharp;

            m_dThresholdStdRatio = ParamFingerAutoTuning.m_dACFRThresholdStdRatio;

            m_nSignalRXStartTrace = ParamFingerAutoTuning.m_nACFRSignalRXStartTrace;
            m_nSignalRXEndTrace = ParamFingerAutoTuning.m_nACFRSignalRXEndTrace;
            m_nSignalTXStartTrace = ParamFingerAutoTuning.m_nACFRSignalTXStartTrace;
            m_nSignalTXEndTrace = ParamFingerAutoTuning.m_nACFRSignalTXEndTrace;

            if (m_nSignalRXEndTrace < m_nSignalRXStartTrace)
            {
                int nSwitchValue = m_nSignalRXStartTrace;
                m_nSignalRXStartTrace = m_nSignalRXEndTrace;
                m_nSignalRXEndTrace = nSwitchValue;
            }

            if (m_nSignalTXEndTrace < m_nSignalTXStartTrace)
            {
                int nSwitchValue = m_nSignalTXStartTrace;
                m_nSignalTXStartTrace = m_nSignalTXEndTrace;
                m_nSignalTXEndTrace = nSwitchValue;
            }

            m_nADCTestFrame = ParamFingerAutoTuning.m_nACFRADCTestFrame;

            m_nChartSNRValueHB = ParamFingerAutoTuning.m_nACFRChartSNRValueHB;
            m_nChartSNRValueLB = ParamFingerAutoTuning.m_nACFRChartSNRValueLB;
            m_nChartSNRValueInterval = ParamFingerAutoTuning.m_nACFRChartSNRValueInterval;
        }

        public override void InitializeSourceDataList()
        {
            m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_BASE);
            m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
            m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_OBASE);

            if (ParamFingerAutoTuning.m_nACFRModeType != 1)
                m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_PREREPORT);
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

            if (ReadOBASEData() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (ParamFingerAutoTuning.m_nACFRModeType != 1)
            {
                if (ReadPreReport() == false)
                {
                    SetErrorMessage(ref sErrorMessage);
                    return false;
                }
            }

            if (ReadADCData() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
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

                if (CheckFileInfoIdentical(cFileCheckInfo, sFileName, MainConstantParameter.m_sDATATYPE_BASE) == false)
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

                if (GetFrameData(ref nFrameData_List, cFileCheckInfo, srFile, sFileName, ReadDataType.Base) == false)
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
                    case MainConstantParameter.m_sDATATYPE_OBASE:
                        sMatchFileName = m_cDataInfo_List[nDataIndex].m_sADCFileName;
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
                        case MainConstantParameter.m_sDATATYPE_OBASE:
                            sMatchFileName = m_cDataInfo_List[nDataIndex].m_sOBASEFileName;
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
            int[,] nSingleFrameData_Array = null;
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
                            nSingleFrameData_Array = new int[cFileCheckInfo.m_nTXTraceNumber, cFileCheckInfo.m_nRXTraceNumber];
                            continue;
                        }
                    }

                    if (bGetFrameDataFlag == true)
                    {
                        if (sSplit_Array.Length >= cFileCheckInfo.m_nRXTraceNumber)
                        {
                            for (int nRXIndex = 0; nRXIndex < cFileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                                nSingleFrameData_Array[nTXCount, nRXIndex] = Convert.ToInt32(sSplit_Array[nRXIndex]);

                            nTXCount++;
                        }

                        if (nTXCount == cFileCheckInfo.m_nTXTraceNumber)
                        {
                            nFrameData_List.Add(nSingleFrameData_Array);
                            bGetFrameDataFlag = false;
                            nFrameCount++;
                        }
                    }

                    if (eReadDataType == ReadDataType.ADC)
                    {
                        if (nFrameCount >= m_nADCTestFrame)
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

        private bool GetFrameData(ref List<int[,]> nFrameData_List, StreamReader srFile, string sFileName, ReadDataType eReadDataType)
        {
            bool bGetFrameDataFlag = false;
            int[,] nSingleFrameData_Array = null;
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
                            nSingleFrameData_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];
                            continue;
                        }
                    }

                    if (bGetFrameDataFlag == true)
                    {
                        if (sSplit_Array.Length >= m_nRXTraceNumber)
                        {
                            for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                                nSingleFrameData_Array[nTXCount, nRXIndex] = Convert.ToInt32(sSplit_Array[nRXIndex]);

                            nTXCount++;
                        }

                        if (nTXCount == m_nTXTraceNumber)
                        {
                            nFrameData_List.Add(nSingleFrameData_Array);
                            bGetFrameDataFlag = false;
                            nFrameCount++;
                        }
                    }

                    if (eReadDataType == ReadDataType.ADC)
                    {
                        if (nFrameCount >= m_nADCTestFrame)
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

        private bool ReadOBASEData()
        {
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_OBASE);

            foreach (string sFilePath in Directory.EnumerateFiles(sDirectoryPath, "*.csv", SearchOption.TopDirectoryOnly))
            {
                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);
                FileCheckInfo cFileCheckInfo = new FileCheckInfo();
                List<int[,]> nFrameData_List = new List<int[,]>();

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                if (CheckFileInfo(ref cFileCheckInfo, srFile, sFileName) == false)
                    return false;

                int nDataIndex = m_cDataInfo_List.FindIndex(x => x.m_nPH1Value == cFileCheckInfo.m_nReadPH1 && x.m_nPH2Value == cFileCheckInfo.m_nReadPH2);

                if (nDataIndex >= 0)
                {
                    m_cDataInfo_List[nDataIndex].m_sOBASEFileName = sFileName;

                    if (CheckFileInfoMatch(cFileCheckInfo, sFileName, MainConstantParameter.m_sDATATYPE_OBASE) == false)
                        return false;

                    srFile = new StreamReader(sFilePath, Encoding.Default);

                    if (GetFrameData(ref nFrameData_List, cFileCheckInfo, srFile, sFileName, ReadDataType.Base) == false)
                        return false;

                    m_cDataInfo_List[nDataIndex].m_sOBASEFileName = sFileName;

                    if (nFrameData_List.Count > 0)
                        m_cDataInfo_List[nDataIndex].m_nOBASEData_Array = nFrameData_List[0];

                    string sMessage = string.Format("Analysis : {0}/{1}", m_nProgressIndex + 1, m_nAnalysisCount);

                    m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                    {
                        m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, m_nProgressIndex + 1, sMessage);
                    });

                    m_nProgressIndex++;
                }

                if (m_cfrmParent.m_bExecute == false)
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

                int[,,] nBASEMinusADC_Array = new int[nFrameData_List.Count, m_nTXTraceNumber, m_nRXTraceNumber];
                int[,,] nOBASEMinusADC_Array = new int[nFrameData_List.Count, m_nTXTraceNumber, m_nRXTraceNumber];

                if (ComputeDVData(ref nBASEMinusADC_Array, nDataIndex, nFrameData_List, m_nRXTraceNumber, m_nTXTraceNumber, MainConstantParameter.m_sDATATYPE_BASEMinusADC) == false)
                    return false;

                if (ComputeDVData(ref nOBASEMinusADC_Array, nDataIndex, nFrameData_List, m_nRXTraceNumber, m_nTXTraceNumber, MainConstantParameter.m_sDATATYPE_OBASEMinusADC) == false)
                    return false;

                ComputeDFT_NUMAndSuggestDFT_NUM(cFileCheckInfo, nDataIndex, cFileCheckInfo.m_nTXTraceNumber);

                if (ComputeReferenceData_SingleSet(nDataIndex, nBASEMinusADC_Array, nOBASEMinusADC_Array) == false)
                    return false;

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

        private bool ComputeDVData(ref int[,,] nDVFrameData_Array, int nDataIndex, List<int[,]> nFrameData_List, int nRXTraceNumber, int nTXTraceNumber, string sDataType)
        {
            int[,] nBASEData_Array = null;

            if (sDataType == MainConstantParameter.m_sDATATYPE_BASEMinusADC)
                nBASEData_Array = m_cDataInfo_List[nDataIndex].m_nBASEData_Array;
            else if (sDataType == MainConstantParameter.m_sDATATYPE_OBASEMinusADC)
                nBASEData_Array = m_cDataInfo_List[nDataIndex].m_nOBASEData_Array;

            for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
            {
                for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                {
                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        int nDVValue = nBASEData_Array[nTXIndex, nRXIndex] - nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                        nDVFrameData_Array[nFrameIndex, nTXIndex, nRXIndex] = nDVValue;
                    }
                }
            }

            /*
            if (SaveFrameData(nDataIndex, sDataType, nDVFrameData_Array, nTXTraceNumber, nRXTraceNumber) == false)
                return false;
            */

            return true;
        }

        private bool SaveFrameData(int nDataIndex, string sDataType, int[,,] nFrameData_Array, int nTXTraceNumber, int nRXTraceNumber)
        {
            bool bError = false;
            int nFrameNumber = nFrameData_Array.GetLength(0);
            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sDataType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", m_sLogDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                                sw.Write(string.Format("{0},", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]));
                            else
                                sw.WriteLine(nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]);
                        }
                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0}-{1} Data Error in Frequency={2:0.000}KHz", sDataType, MainConstantParameter.m_sDATATYPE_ADC, dFrequency.ToString("0.000"));
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

        private bool ReadPreReport()
        {
            string sPreReportPath = string.Format(@"{0}\{1}\PreReport.csv", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_PREREPORT);

            int nFRPH2MinPH2 = -1;
            int nFRPH2FixedPH1 = -1;

            string sLine = "";

            StreamReader srFile = new StreamReader(sPreReportPath, Encoding.Default);

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSplit_Array = sLine.Split(',');

                    if (sSplit_Array.Length >= 2)
                    {
                        if (GetParameterValue(ref nFRPH2FixedPH1, sSplit_Array, "MinPH1(Hex)", m_nVALUETYPE_Hex) == false)
                            return false;
                        else if (GetParameterValue(ref nFRPH2MinPH2, sSplit_Array, "PH2LB(Hex)", m_nVALUETYPE_Hex) == false)
                            return false;
                    }

                    if (nFRPH2FixedPH1 > -1 && nFRPH2MinPH2 > -1)
                        break;
                }
            }
            finally
            {
                srFile.Close();
            }

            DataTable datatableData = null;

            try
            {
                datatableData = ConvertCsvToDataTable(sPreReportPath, "Frequency Rank:");
            }
            catch
            {
                m_sErrorMessage = string.Format("Get Data Table Error in {0}\"PreReport.csv File", MainConstantParameter.m_sDATATYPE_PREREPORT);
                return false;
            }

            string[] nTitleColumn_Array = new string[] 
            { 
                "Frequency(KHz)", 
                "PH1+PH2", 
                "Signal RefValue", 
                "Noise Mean", 
                "Noise Std", 
                "Noise PosRef", 
                "Noise NegRef", 
                "Noise RefValue" 
            };

            for (int nColumnIndex = 0; nColumnIndex < nTitleColumn_Array.Length; nColumnIndex++)
            {
                if (datatableData.Columns.Contains(nTitleColumn_Array[nColumnIndex]) == false)
                {
                    m_sErrorMessage = string.Format("Get Column Title:{0} Error in {1}\"PreReport.csv File", nTitleColumn_Array[nColumnIndex], MainConstantParameter.m_sDATATYPE_PREREPORT);
                    return false;
                }
            }

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                for (int nRowIndex = 0; nRowIndex < datatableData.Rows.Count; nRowIndex++)
                {
                    double dFrequency = Convert.ToDouble(datatableData.Rows[nRowIndex]["Frequency(KHz)"]);
                    string sPH1PH2Sum = datatableData.Rows[nRowIndex]["PH1+PH2"].ToString();

                    string sToken = "0x";
                    int nIndex = sPH1PH2Sum.IndexOf(sToken);
                    int nPH1PH2Sum = 0;

                    if (nIndex >= 0)
                    {
                        string sPH1PH2SumMessage = sPH1PH2Sum.Substring(nIndex + sToken.Length);

                        if (ElanConvert.IsHexadecimal(sPH1PH2SumMessage) == true)
                            nPH1PH2Sum = Convert.ToInt32(sPH1PH2SumMessage, 16);
                    }

                    int nPH1Value = nPH1PH2Sum - nFRPH2MinPH2;

                    if (m_cDataInfo_List[nDataIndex].m_dFrequency == dFrequency &&
                        m_cDataInfo_List[nDataIndex].m_nPH1Value == nPH1Value &&
                        m_cDataInfo_List[nDataIndex].m_nPH2Value == nFRPH2MinPH2)
                    {
                        double dSignalReference = Convert.ToDouble(datatableData.Rows[nRowIndex]["Signal RefValue"]);
                        double dNoiseMean = Convert.ToDouble(datatableData.Rows[nRowIndex]["Noise Mean"]);
                        double dNoiseStd = Convert.ToDouble(datatableData.Rows[nRowIndex]["Noise Std"]);
                        double dNoisePositiveReference = Convert.ToDouble(datatableData.Rows[nRowIndex]["Noise PosRef"]);
                        double dNoiseNegativeReference = Convert.ToDouble(datatableData.Rows[nRowIndex]["Noise NegRef"]);
                        double dNoiseReference = Convert.ToDouble(datatableData.Rows[nRowIndex]["Noise RefValue"]);

                        m_cDataInfo_List[nDataIndex].m_cPreReportInfo.m_dSignalReference = dSignalReference;
                        m_cDataInfo_List[nDataIndex].m_cPreReportInfo.m_dNoiseMean = dNoiseMean;
                        m_cDataInfo_List[nDataIndex].m_cPreReportInfo.m_dNoiseStd = dNoiseStd;
                        m_cDataInfo_List[nDataIndex].m_cPreReportInfo.m_dNoisePositiveReference = dNoisePositiveReference;
                        m_cDataInfo_List[nDataIndex].m_cPreReportInfo.m_dNoiseNegativeReference = dNoiseNegativeReference;
                        m_cDataInfo_List[nDataIndex].m_cPreReportInfo.m_dNoiseReference = dNoiseReference;
                        m_cDataInfo_List[nDataIndex].m_cPreReportInfo.m_bGetInfo = true;
                        break;
                    }
                }
            }

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                if (m_cDataInfo_List[nDataIndex].m_cPreReportInfo.m_bGetInfo == false)
                {
                    m_sErrorMessage = string.Format("Get PreReport Info Error in Frequency={0}KHz", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString());
                    return false;
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

            return true;
        }
        
        private bool GetParameterValue(ref int nValue, string[] sSplit_Array, string sParameterName, int nValueType)
        {
            if (sSplit_Array[0] == sParameterName)
            {
                string sValue = sSplit_Array[1];

                if (nValueType == m_nVALUETYPE_Hex)
                {
                    if (ElanConvert.IsHexadecimal(sValue) == false)
                    {
                        m_sErrorMessage = string.Format("Get {0} Value Format Error in PreReport.csv", sParameterName);
                        return false;
                    }
                    else
                        nValue = Convert.ToInt32(sValue, 16);
                }
                else if (nValueType == m_nVALUETYPE_Int)
                {
                    if (ElanConvert.IsInt(sValue) == false)
                    {
                        m_sErrorMessage = string.Format("Get {0} Value Format Error in PreReport.csv", sParameterName);
                        return false;
                    }
                    else
                        nValue = Convert.ToInt32(sValue);
                }
            }

            return true;
        }

        /// <summary>
        /// 將Csv讀入DataTable
        /// </summary>
        /// <param name="sFilePath">csv檔案路徑</param>
        /// <param name="sTitleName">表示第n行是欄位title,第n+1行是記錄開始</param>
        private DataTable ConvertCsvToDataTable(string sFilePath, string sTitleName)
        {
            DataTable datatableData = new DataTable();
            String sSplitToken = "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)";
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default, false);
            int i = 0, m = 0;
            bool bGetTitle = false;
            bool bGetColumnHeader = false;
            srFile.Peek();

            while (srFile.Peek() > 0)
            {
                m = m + 1;
                string str = srFile.ReadLine();

                if (str == sTitleName)
                {
                    bGetTitle = true;
                    continue;
                }

                if (bGetTitle == true)
                {
                    if (bGetColumnHeader == false) //如果是欄位行，則自動加入欄位。
                    {
                        MatchCollection mcs = Regex.Matches(str, sSplitToken);
                        foreach (Match mc in mcs)
                        {
                            datatableData.Columns.Add(mc.Value); //增加列標題
                        }
                        bGetColumnHeader = true;
                    }
                    else
                    {
                        MatchCollection mcs = Regex.Matches(str, "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
                        i = 0;
                        DataRow datarowData = datatableData.NewRow();

                        foreach (Match mc in mcs)
                        {
                            datarowData[i] = mc.Value;
                            i++;
                        }

                        datatableData.Rows.Add(datarowData);  //DataTable 增加一行     
                    }
                }
            }

            return datatableData;
        }

        private bool AnalysisData()
        {
            m_nPH2LB = m_cDataInfo_List[0].m_nPH2Value;

            /*
            if (ComputeSignalArea() == false)
                return false;

            if (ParamFingerAutoTuning.m_nACFRModeType != 1)
            {
                if (ComputeACNoiseAreaAndReferenceValue() == false)
                    return false;
            }
            else
            {
                if (ComputeNoiseAreaAndReferenceValue() == false)
                    return false;
            }

            if (ComputeSignalReferenceValue() == false)
                return false;
            */

            if (ReGetSignalAreaByTotalData() == false)
                return false;

            if (ComputeSNRValue() == false)
                return false;

            if (ComputeCompositeSNRValue() == false)
                return false;

            return true;
        }

        private bool ComputeReferenceData_SingleSet(int nDataIndex, int[,,] nBASEMinusADC_Array, int[,,] nOBASEMinusADC_Array)
        {
            if (ComputeSignalArea(nDataIndex, nOBASEMinusADC_Array) == false)
                return false;

            if (m_eGetSignalAreaMethod == GetSignalAreaMethod.UseBaseDifferEdgeDetect && (m_eUseBaseDifferEdgeDetectMethod == UseBaseDifferEdgeDetectMethod.UseConvolutionG || m_eUseBaseDifferEdgeDetectMethod == UseBaseDifferEdgeDetectMethod.UseConvolutionGxGy) &&
                m_bUseTotalDataGetSignalTrace == true)
                return true;

            if (ParamFingerAutoTuning.m_nACFRModeType != 1)
            {
                if (ComputeACNoiseAreaAndReferenceValue(nDataIndex, nBASEMinusADC_Array) == false)
                    return false;
            }
            else
            {
                if (ComputeNoiseAreaAndReferenceValue(nDataIndex, nBASEMinusADC_Array) == false)
                    return false;
            }

            if (ComputeSignalReferenceValue(nDataIndex, nOBASEMinusADC_Array) == false)
                return false;

            return true;
        }

        private bool ComputeReferenceData_TotalData(int nDataIndex, int[,,] nBASEMinusADC_Array, int[,,] nOBASEMinusADC_Array)
        {
            if (ParamFingerAutoTuning.m_nACFRModeType != 1)
            {
                if (ComputeACNoiseAreaAndReferenceValue(nDataIndex, nBASEMinusADC_Array) == false)
                    return false;
            }
            else
            {
                if (ComputeNoiseAreaAndReferenceValue(nDataIndex, nBASEMinusADC_Array) == false)
                    return false;
            }

            if (ComputeSignalReferenceValue(nDataIndex, nOBASEMinusADC_Array) == false)
                return false;

            return true;
        }

        private void ComputeDFT_NUMAndSuggestDFT_NUM(FileCheckInfo cFileCheckInfo, int nDataIndex, int nTXTraceNumber)
        {
            m_cDataInfo_List[nDataIndex].m_nDFT_NUMValue = cFileCheckInfo.m_nReadDFT_NUM;

            DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];
            m_cDataInfo_List[nDataIndex].m_nSuggestDFT_NUM = ElanConvert.Convert2SuggestDFT_NUM(cDataInfo.m_nPH1Value, cDataInfo.m_nPH2Value, nTXTraceNumber, ParamFingerAutoTuning.m_nIdealScanTime);
        }

        private bool ComputeSignalArea(int nDataIndex, int[,,] nOBASEMinusADC_Array)
        {
            #region Method 3:STD Threshold Compare
            /*
            for (int nIndex = 0; nIndex < m_InfoDataList.Count; nIndex++)
            {
                List<int[,]> SignalFrameData = new List<int[,]>();
                List<int[,]> DVFrameData = m_InfoDataList[nIndex].m_OBASEMinusADCList;

                double[] FrameStdArray = new double[DVFrameData.Count];
                double[,] VerStdArray = new double[DVFrameData.Count, m_nRXTraceNumber];
                double[,] HorStdArray = new double[DVFrameData.Count, m_nTXTraceNumber];
                bool[,] VerFlagArray = new bool[DVFrameData.Count, m_nRXTraceNumber];
                bool[,] HorFlagArray = new bool[DVFrameData.Count, m_nTXTraceNumber];
                Array.Clear(VerFlagArray, 0, VerFlagArray.Length);
                Array.Clear(HorFlagArray, 0, HorFlagArray.Length);

                for (int nFrameIdx = 0; nFrameIdx < DVFrameData.Count; nFrameIdx++)
                {
                    List<int> FrameList = new List<int>();
                    for (int nY = 0; nY < m_nTXTraceNumber; nY++)
                    {
                        for (int nX = 0; nX < m_nRXTraceNumber; nX++)
                            FrameList.Add(DVFrameData[nFrameIdx][nY, nX]);
                    }

                    FrameStdArray[nFrameIdx] = Math.Round(MathMethod.GetSD(FrameList), 3, MidpointRounding.AwayFromZero);

                    for (int nY = 0; nY < m_nTXTraceNumber; nY++)
                    {
                        List<int> HorList = new List<int>();
                        for (int nX = 0; nX < m_nRXTraceNumber; nX++)
                            HorList.Add(DVFrameData[nFrameIdx][nY, nX]);

                        HorStdArray[nFrameIdx, nY] = Math.Round(MathMethod.GetSD(HorList), 3, MidpointRounding.AwayFromZero);

                        if (HorStdArray[nFrameIdx, nY] > FrameStdArray[nFrameIdx])
                        {
                            if (nY == 0)
                            {
                                HorFlagArray[nFrameIdx, nY] = true;
                                HorFlagArray[nFrameIdx, nY + 1] = true;
                            }
                            else if (nY == m_nTXTraceNumber - 1)
                            {
                                HorFlagArray[nFrameIdx, nY - 1] = true;
                                HorFlagArray[nFrameIdx, nY] = true;
                            }
                            else
                            {
                                HorFlagArray[nFrameIdx, nY - 1] = true;
                                HorFlagArray[nFrameIdx, nY] = true;
                                HorFlagArray[nFrameIdx, nY + 1] = true;
                            }
                        }
                    }

                    for (int nX = 0; nX < m_nRXTraceNumber; nX++)
                    {
                        List<int> VerList = new List<int>();
                        for (int nY = 0; nY < m_nTXTraceNumber; nY++)
                            VerList.Add(DVFrameData[nFrameIdx][nY, nX]);

                        VerStdArray[nFrameIdx, nX] = Math.Round(MathMethod.GetSD(VerList), 3, MidpointRounding.AwayFromZero);

                        if (VerStdArray[nFrameIdx, nX] > FrameStdArray[nFrameIdx])
                        {
                            if (nX == 0)
                            {
                                VerFlagArray[nFrameIdx, nX] = true;
                                VerFlagArray[nFrameIdx, nX + 1] = true;
                            }
                            else if (nX == m_nTXTraceNumber - 1)
                            {
                                VerFlagArray[nFrameIdx, nX - 1] = true;
                                VerFlagArray[nFrameIdx, nX] = true;
                            }
                            else
                            {
                                VerFlagArray[nFrameIdx, nX - 1] = true;
                                VerFlagArray[nFrameIdx, nX] = true;
                                VerFlagArray[nFrameIdx, nX + 1] = true;
                            }
                        }
                    }
                }

                for (int nFrameIdx = 0; nFrameIdx < DVFrameData.Count; nFrameIdx++)
                {
                    int[,] SFrameData = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                    for (int nY = 0; nY < m_nTXTraceNumber; nY++)
                    {
                        for (int nX = 0; nX < m_nRXTraceNumber; nX++)
                        {
                            if (HorFlagArray[nFrameIdx, nY] == true &&
                                VerFlagArray[nFrameIdx, nX] == true)
                            {
                                if (nFrameIdx == 0)
                                {
                                    if (HorFlagArray[nFrameIdx, nY + 1] == true &&
                                        VerFlagArray[nFrameIdx, nX + 1] == true)
                                        SFrameData[nY, nX] = DVFrameData[nFrameIdx][nY, nX];
                                    else
                                        SFrameData[nY, nX] = 0;
                                }
                                else if (nFrameIdx == DVFrameData.Count - 1)
                                {
                                    if (HorFlagArray[nFrameIdx, nY - 1] == true &&
                                        VerFlagArray[nFrameIdx, nX - 1] == true)
                                        SFrameData[nY, nX] = DVFrameData[nFrameIdx][nY, nX];
                                    else
                                        SFrameData[nY, nX] = 0;
                                }
                                else
                                {
                                    if (HorFlagArray[nFrameIdx, nY + 1] == true &&
                                        VerFlagArray[nFrameIdx, nX + 1] == true)
                                        SFrameData[nY, nX] = DVFrameData[nFrameIdx][nY, nX];
                                    else if (VerFlagArray[nFrameIdx, nY - 1] == true &&
                                             HorFlagArray[nFrameIdx, nX - 1] == true)
                                        SFrameData[nY, nX] = DVFrameData[nFrameIdx][nY, nX];
                                    else
                                        SFrameData[nY, nX] = 0;
                                }
                                

                                SFrameData[nY, nX] = DVFrameData[nFrameIdx][nY, nX];
                            }
                            else
                                SFrameData[nY, nX] = 0;
                        }
                    }

                    SignalFrameData.Add(SFrameData);
                }

                if (xSaveParticularData(nIndex, "Pure OBASE-ADC", DVFrameData, HorStdArray, VerStdArray, FrameStdArray, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                    return false;

                if (xSaveParticularData(nIndex, "Pure Signal", SignalFrameData, HorStdArray, VerStdArray, FrameStdArray, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                    return false;
            }
            */
            #endregion

            #region Method 2:Sobel Edge Detection Convolution + STD Theshold Compare(利用5x5 Sobel邊緣偵測方法以及計算各條Trace之標準差過濾出Signal區域的Trace)
            double dSignalAreaRatio_Horizontal = Math.Round(m_dSignalAreaPercentage_Horizontal / 100.0, 4, MidpointRounding.AwayFromZero);
            double dSignalAreaRatio_Vertical = Math.Round(m_dSignalAreaPercentage_Vertical / 100.0, 4, MidpointRounding.AwayFromZero);
            double dNoneSignalAreaRatio = Math.Round(m_dNoneSignalAreaPercentage / 100.0, 4, MidpointRounding.AwayFromZero);

            int[,] nHorizontalFilter_Array = null, nVerticalFilter_Array = null;

            if (m_eGetSignalAreaMethod == GetSignalAreaMethod.UseDVEdgeDetect || m_eGetSignalAreaMethod == GetSignalAreaMethod.UseBaseDifferEdgeDetect)
            {
                //int[,] nFilter_Array = new int[,] { { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, -24, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 } };
                //int[,] nFilter_Array = new int[,] { { 0, 1, 0 }, { 1, -4, 1 },{ 0, 1, 0 } };  //Edge Detection Array
                //int[,] nFilter_Array = new int[,] { { 1, 0, 1 }, { 0, 4, 0 }, { 1, 0, 1 } };  //Sharp Cross Array
                //int[,] nFilter_Array = new int[,] { { 1, 1, 1 }, { 1, -8, 1 }, { 1, 1, 1 } };  //Neighbor Edge Array
                //int[,] nFilter_Array = new int[,] { { 1, 1, 1 }, { 1, 2, 1 }, { 1, 1, 1 } };  //Sharp Array
                //int[,] nFilter_Array = new int[,] { { 1, 1, 1 }, { 1, 0, 1 }, { 1, 1, 1 } };  //Edge Smooth Array

                //Type01 Sobel 3x3 Mask
                if (m_eFilterMaskType == FilterMaskType.Type01_Sobel_3x3)
                {
                    nHorizontalFilter_Array = new int[,] 
                    { 
                        { -1, 0, 1 }, 
                        { -2, 0, 2 }, 
                        { -1, 0, 1 } 
                    };  //Sobel Vertical Edge 3x3 Array

                    nVerticalFilter_Array = new int[,] 
                    { 
                        { -1, -2, -1 }, 
                        {  0,  0,  0 }, 
                        {  1,  2,  1 } 
                    };  //Sobel Horizontal Edge 3x3 Array
                }
                //Type02 Sobel 3x3 Strong Mask
                else if (m_eFilterMaskType == FilterMaskType.Type02_Sobel_3x3_Strong)
                {
                    nHorizontalFilter_Array = new int[,] 
                    { 
                        {  -3, 0,  3 }, 
                        { -10, 0, 10 }, 
                        {  -3,  0, 3 } 
                    };  //Sobel Vertical Edge 3x3 Array

                    nVerticalFilter_Array = new int[,] 
                    { 
                        { -3, -10, -3 }, 
                        {  0,   0,  0 }, 
                        {  3,  10,  3 } 
                    };  //Sobel Horizontal Edge 3x3 Array
                }
                //Type03 Another Sharp Mask
                else if (m_eFilterMaskType == FilterMaskType.Type03_Another_Sharp)
                {
                    nHorizontalFilter_Array = new int[,] 
                    { 
                        {  -5,  -4,   0,   4,   5 }, 
                        {  -8, -10,   0,  10,   8 }, 
                        { -10, -20,   0,  20,  10 }, 
                        {  -8, -10,   0,  10,   8 }, 
                        {  -5,  -4,   0,   4,   5 } 
                    };  //Sobel Vertical Edge 5x5 Array

                    nVerticalFilter_Array = new int[,] 
                    { 
                        {  -5,  -8, -10,  -8,  -5 }, 
                        {  -4, -10, -20, -10,  -4 }, 
                        {   0,   0,   0,   0,   0 }, 
                        {   4,  10,  20,  10,   4 }, 
                        {   5,   8,  10,   8,   5 } 
                    };  //Sobel Horizontal Edge 5x5 Array
                }
                //Type04 Another Smooth Mask
                else if (m_eFilterMaskType == FilterMaskType.Type04_Another_Smooth)
                {
                    nHorizontalFilter_Array = new int[,] 
                    { 
                        {  -5,  -4,   0,   4,   5 }, 
                        { -10,  -8,   0,   8,  10 }, 
                        { -20, -10,   0,  10,  20 }, 
                        { -10,  -8,   0,   8,  10 }, 
                        {  -5,  -4,   0,   4,   5 } 
                    };  //Vertical Edge 5x5 Array

                    nVerticalFilter_Array = new int[,] 
                    { 
                        {  -5, -10, -20, -10,  -5 }, 
                        {  -4,  -8, -10,  -8,  -4 }, 
                        {   0,   0,   0,   0,   0 }, 
                        {   4,   8,  10,   8,   4 }, 
                        {   5,  10,  20,  10,   5 } 
                    };  //Horizontal Edge 5x5 Array
                }
                //Type05 Identical Mask
                else if (m_eFilterMaskType == FilterMaskType.Type05_Identical)
                {
                    nHorizontalFilter_Array = new int[,] 
                    { 
                        {  -1,  -1,   0,   1,   1 }, 
                        {  -1,  -1,   0,   1,   1 }, 
                        {  -1,  -1,   0,   1,   1 }, 
                        {  -1,  -1,   0,   1,   1 }, 
                        {  -1,  -1,   0,   1,   1 } 
                    };  //Vertical Edge 5x5 Array

                    nVerticalFilter_Array = new int[,] 
                    { 
                        {  -1,  -1,  -1,  -1,  -1 }, 
                        {  -1,  -1,  -1,  -1,  -1 }, 
                        {   0,   0,   0,   0,   0 }, 
                        {   1,   1,   1,   1,   1 }, 
                        {   1,   1,   1,   1,   1 } 
                    };  //Horizontal Edge 5x5 Array
                }
                //Type06 Roberts Mask
                else if (m_eFilterMaskType == FilterMaskType.Type06_Roberts)
                {
                    nHorizontalFilter_Array = new int[,] 
                    { 
                        {  1,  0,  0,  0,  0 }, 
                        {  0,  1,  0,  0,  0 }, 
                        {  0,  0,  0,  0,  0 }, 
                        {  0,  0,  0, -1,  0 }, 
                        {  0,  0,  0,  0, -1 } 
                    };  //Roberts Vertical Edge 5x5 Array

                    nVerticalFilter_Array = new int[,] 
                    { 
                        {  0,  0,  0,  0,  1 }, 
                        {  0,  0,  0,  1,  0 }, 
                        {  0,  0,  0,  0,  0 }, 
                        {  0, -1,  0,  0,  0 }, 
                        { -1,  0,  0,  0,  0 } 
                    };  //Roberts Horizontal Edge 5x5 Array
                }
                //Type07 Sobel 5x5 Smooth Mask
                else if (m_eFilterMaskType == FilterMaskType.Type07_Sobel_5x5_Smooth)
                {
                    nHorizontalFilter_Array = new int[,] 
                    { 
                        {  2,  1,  0,  -1,  -2 }, 
                        {  2,  1,  0,  -1,  -2 }, 
                        {  4,  2,  0,  -2,  -4 }, 
                        {  2,  1,  0,  -1,  -2 }, 
                        {  2,  1,  0,  -1,  -2 } 
                    };  //Sobel Vertical Edge 5x5 Array

                    nVerticalFilter_Array = new int[,] 
                    { 
                        {  2,  2,  4,  2,  2 }, 
                        {  1,  1,  2,  1,  1 }, 
                        {  0,  0,  0,  0,  0 }, 
                        { -1, -1, -2, -1, -1 }, 
                        { -2, -2, -4, -2, -2 } 
                    };  //Sobel Horizontal Edge 5x5 Array
                }
                //Type08 Sobel 5x5 Sharp Mask
                else if (m_eFilterMaskType == FilterMaskType.Type08_Sobel_5x5_Sharp)
                {
                    nHorizontalFilter_Array = new int[,] 
                    { 
                        {  1,  2,  0,  -2,  -1 }, 
                        {  1,  2,  0,  -2,  -1 }, 
                        {  2,  4,  0,  -4,  -2 }, 
                        {  1,  2,  0,  -2,  -1 }, 
                        {  1,  2,  0,  -2,  -1 } 
                    };  //Sobel Vertical Edge 5x5 Array

                    nVerticalFilter_Array = new int[,] 
                    { 
                        {  1,  1,  2,  1,  1 }, 
                        {  2,  2,  4,  2,  2 }, 
                        {  0,  0,  0,  0,  0 }, 
                        { -2, -2, -4, -2, -2 }, 
                        { -1, -1, -2, -1, -1 } 
                    };  //Sobel Horizontal Edge 5x5 Array
                }
                //Type09 Another Strong Sharp Mask
                else if (m_eFilterMaskType == FilterMaskType.Type09_Another_Strong_Sharp)
                {
                    nHorizontalFilter_Array = new int[,] 
                    { 
                        {  -1,  -1,   0,   1,  1 }, 
                        {  -1,  -1,   0,   1,  1 }, 
                        {  -1, -10,   0,  10,  1 }, 
                        {  -1,  -1,   0,   1,  1 }, 
                        {  -1,  -1,   0,   1,  1 } 
                    };  //Sobel Vertical Edge 5x5 Array

                    nVerticalFilter_Array = new int[,] 
                    { 
                        {  -1, -1,  -1, -1, -1 }, 
                        {  -1, -1, -10, -1, -1 }, 
                        {   0,  0,   0,  0,  0 }, 
                        {   1,  1,  10,  1,  1 }, 
                        {   1,  1,   1,  1,  1 } 
                    };  //Sobel Horizontal Edge 5x5 Array
                }
            }

            /*
            if (m_eGetSignalAreaMethod == GetSignalAreaMethod.UseDVEdgeDetect)
            {
                //Type01 Sobel 3x3 Mask
                //nHorizontalFilter_Array = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };  //Sobel Vertical Edge 3x3 Array
                //nVerticalFilter_Array = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };  //Sobel Horizontal Edge 3x3 Array

                //Type02 Sobel 3x3 Strong Mask
                //nHorizontalFilter_Array = new int[,] { { -3, 0, 3 }, { -10, 0, 10 }, { -3, 0, 3 } };  //Sobel Vertical Edge 3x3 Array
                //nVerticalFilter_Array = new int[,] { { -3, -10, -3 }, { 0, 0, 0 }, { 3, 10, 3 } };  //Sobel Horizontal Edge 3x3 Array

                //Type03 Another Sharp Mask
                //nHorizontalFilter_Array = new int[,] { {  -5,  -4,   0,   4,   5 }, 
                //                                       {  -8, -10,   0,  10,   8 }, 
                //                                       { -10, -20,   0,  20,  10 }, 
                //                                       {  -8, -10,   0,  10,   8 }, 
                //                                       {  -5,  -4,   0,   4,   5 } };  //Sobel Vertical Edge 5x5 Array
                //nVerticalFilter_Array = new int[,] { {  -5,  -8, -10,  -8,  -5 }, 
                //                                     {  -4, -10, -20, -10,  -4 }, 
                //                                     {   0,   0,   0,   0,   0 }, 
                //                                     {   4,  10,  20,  10,   4 }, 
                //                                     {   5,   8,  10,   8,   5 } };  //Sobel Horizontal Edge 5x5 Array

                //Type04 Another Smooth Mask
                //nHorizontalFilter_Array = new int[,] { {  -5,  -4,   0,   4,   5 }, 
                //                                       { -10,  -8,   0,   8,  10 }, 
                //                                       { -20, -10,   0,  10,  20 }, 
                //                                       { -10,  -8,   0,   8,  10 }, 
                //                                       {  -5,  -4,   0,   4,   5 } };  //Vertical Edge 5x5 Array
                //nVerticalFilter_Array = new int[,] { {  -5, -10, -20, -10,  -5 }, 
                //                                     {  -4,  -8, -10,  -8,  -4 }, 
                //                                     {   0,   0,   0,   0,   0 }, 
                //                                     {   4,   8,  10,   8,   4 }, 
                //                                     {   5,  10,  20,  10,   5 } };  //Horizontal Edge 5x5 Array

                //Type05 Identical Mask
                //nHorizontalFilter_Array = new int[,] { {  -1,  -1,   0,   1,   1 }, 
                //                                       {  -1,  -1,   0,   1,   1 }, 
                //                                       {  -1,  -1,   0,   1,   1 }, 
                //                                       {  -1,  -1,   0,   1,   1 }, 
                //                                       {  -1,  -1,   0,   1,   1 } };  //Vertical Edge 5x5 Array
                //nVerticalFilter_Array = new int[,] { {  -1,  -1,  -1,  -1,  -1 }, 
                //                                     {  -1,  -1,  -1,  -1,  -1 }, 
                //                                     {   0,   0,   0,   0,   0 }, 
                //                                     {   1,   1,   1,   1,   1 }, 
                //                                     {   1,   1,   1,   1,   1 } };  //Horizontal Edge 5x5 Array

                //Type06 Roberts Mask
                //nHorizontalFilter_Array = new int[,] { { 1, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, {0, 0, 0, -1, 0 }, { 0, 0, 0, 0, -1 } };  //Roberts Vertical Edge 5x5 Array
                //nVerticalFilter_Array = new int[,] { { 0, 0, 0, 0, 1 }, { 0, 0, 0, 1, 0 }, { 0, 0, 0, 0, 0 }, { 0, -1, 0, 0, 0 }, { -1, 0, 0, 0, 0 } };  //Roberts Horizontal Edge 5x5 Array

                //Type07 Sobel 5x5 Smooth Mask
                //nHorizontalFilter_Array = new int[,] { {  2,  1,  0,  -1,  -2 }, 
                //                                       {  2,  1,  0,  -1,  -2 }, 
                //                                       {  4,  2,  0,  -2,  -4 }, 
                //                                       {  2,  1,  0,  -1,  -2 }, 
                //                                       {  2,  1,  0,  -1,  -2 } };  //Sobel Vertical Edge 5x5 Array
                //nVerticalFilter_Array = new int[,] { {  2,  2,  4,  2,  2 }, 
                //                                     {  1,  1,  2,  1,  1 }, 
                //                                     {  0,  0,  0,  0,  0 }, 
                //                                     { -1, -1, -2, -1, -1 }, 
                //                                     { -2, -2, -4, -2, -2 } };  //Sobel Horizontal Edge 5x5 Array
            }
            else if (m_eGetSignalAreaMethod == GetSignalAreaMethod.UseBaseDifferEdgeDetect)
            {
                //Type01 Sobel 3x3 Mask
                //nHorizontalFilter_Array = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };  //Sobel Vertical Edge 3x3 Array
                //nVerticalFilter_Array = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };  //Sobel Horizontal Edge 3x3 Array

                //Type02 Sobel 3x3 Strong Mask
                //nHorizontalFilter_Array = new int[,] { { -3, 0, 3 }, { -10, 0, 10 }, { -3, 0, 3 } };  //Sobel Vertical Edge 3x3 Array
                //nVerticalFilter_Array = new int[,] { { -3, -10, -3 }, { 0, 0, 0 }, { 3, 10, 3 } };  //Sobel Horizontal Edge 3x3 Array

                //Type09 Another Strong Sharp Mask
                //nHorizontalFilter_Array = new int[,] { {  -1,  -1,   0,   1,  1 }, 
                //                                       {  -1,  -1,   0,   1,  1 }, 
                //                                       {  -1, -10,   0,  10,  1 }, 
                //                                       {  -1,  -1,   0,   1,  1 }, 
                //                                       {  -1,  -1,   0,   1,  1 } };  //Sobel Vertical Edge 5x5 Array
                //nVerticalFilter_Array = new int[,] { {  -1, -1,  -1, -1, -1 }, 
                //                                     {  -1, -1, -10, -1, -1 }, 
                //                                     {   0,  0,   0,  0,  0 }, 
                //                                     {   1,  1,  10,  1,  1 }, 
                //                                     {   1,  1,   1,  1,  1 } };  //Sobel Horizontal Edge 5x5 Array

                //Type03 Another Sharp Mask
                //nHorizontalFilter_Array = new int[,] { {  -5,  -4,   0,   4,   5 }, 
                //                                       {  -8, -10,   0,  10,   8 }, 
                //                                       { -10, -20,   0,  20,  10 }, 
                //                                       {  -8, -10,   0,  10,   8 }, 
                //                                       {  -5,  -4,   0,   4,   5 } };  //Sobel Vertical Edge 5x5 Array
                //nVerticalFilter_Array = new int[,] { {  -5,  -8, -10,  -8,  -5 }, 
                //                                     {  -4, -10, -20, -10,  -4 }, 
                //                                     {   0,   0,   0,   0,   0 }, 
                //                                     {   4,  10,  20,  10,   4 }, 
                //                                     {   5,   8,  10,   8,   5 } };  //Sobel Horizontal Edge 5x5 Array

                //Type07 Sobel 5x5 Smooth Mask
                //nHorizontalFilter_Array = new int[,] { {  2,  1,  0,  -1,  -2 }, 
                //                                       {  2,  1,  0,  -1,  -2 }, 
                //                                       {  4,  2,  0,  -2,  -4 }, 
                //                                       {  2,  1,  0,  -1,  -2 }, 
                //                                       {  2,  1,  0,  -1,  -2 } };  //Sobel Vertical Edge 5x5 Array
                //nVerticalFilter_Array = new int[,] { {  2,  2,  4,  2,  2 }, 
                //                                     {  1,  1,  2,  1,  1 }, 
                //                                     {  0,  0,  0,  0,  0 }, 
                //                                     { -1, -1, -2, -1, -1 }, 
                //                                     { -2, -2, -4, -2, -2 } };  //Sobel Horizontal Edge 5x5 Array

                //Type08 Sobel 5x5 Sharp Mask
                //nHorizontalFilter_Array = new int[,] { {  1,  2,  0,  -2,  -1 }, 
                //                                       {  1,  2,  0,  -2,  -1 }, 
                //                                       {  2,  4,  0,  -4,  -2 }, 
                //                                       {  1,  2,  0,  -2,  -1 }, 
                //                                       {  1,  2,  0,  -2,  -1 } };  //Sobel Vertical Edge 5x5 Array
                //nVerticalFilter_Array = new int[,] { {  1,  1,  2,  1,  1 }, 
                //                                     {  2,  2,  4,  2,  2 }, 
                //                                     {  0,  0,  0,  0,  0 }, 
                //                                     { -2, -2, -4, -2, -2 }, 
                //                                     { -1, -1, -2, -1, -1 } };  //Sobel Horizontal Edge 5x5 Array
            }
            */

            List<int[,]> nFrameRXDifferData_List = new List<int[,]>();
            List<int[,]> nSignalFrameData_List = new List<int[,]>();
            int[,,] nDVFrameData_Array = nOBASEMinusADC_Array;

            //計算有TX方向Signal區域與有RX方向Signal區域之各自的Frame數
            int nTXSignalAreaFrameCount = 0;
            int nRXSignalAreaFrameCount = 0;

            #region Use DV Edge Detect
            if (m_eGetSignalAreaMethod == GetSignalAreaMethod.UseDVEdgeDetect)
            {
                List<uint[,]> nConvolutionFrameData_List = new List<uint[,]>();
                List<int[,]> nConvolutionGxFrameData_List = new List<int[,]>();
                List<int[,]> nConvolutionGyFrameData_List = new List<int[,]>();
                List<long[,]> lConvolutionFrameRXDifferData_List = new List<long[,]>();
                //List<long[,]> lConvolutionFrameTXDifferData_List = new List<long[,]>();

                #region Compute Frame Mean and Use Sobel Edge Detect
                /*
                    List<int[,]> listnFrameMean_List = new List<int[,]>();
                    int[,] nFrameMean_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                    for (int nY = 0; nY < m_nTXTraceNumber; nY++)
                    {
                        for (int nX = 0; nX < m_nRXTraceNumber; nX++)
                        {
                            List<int> nValueData_List = new List<int>();

                            for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_List.Count; nFrameIndex++)
                                nValueData_List.Add(nDVFrameData_List[nFrameIndex][nY, nX]);

                            int nMean = (int)Math.Round(nValueData_List.Average(), 0, MidpointRounding.AwayFromZero);
                            nFrameMean_Array[nY, nX] = nMean;
                        }
                    }

                    listnFrameMean_List.Add(nFrameMean_Array);

                    List<int[,]> nHorizontalConvolution_List = new List<int[,]>();
                    List<int[,]> nVerticalConvolution_List = new List<int[,]>();
                    List<int[,]> nTotalConvolution_List = new List<int[,]>();
                    int[,] nHorizontalConvolution_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];
                    int[,] nVerticalConvolution_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                    nHorizontalConvolution_Array = ComputeConvolution(nFrameMean_Array, m_nTXTraceNumber, m_nRXTraceNumber, nHorizontalFilter_Array, nHorizontalFilter_Array.GetLength(0),
                                                                   nHorizontalFilter_Array.GetLength(1));
                    nHorizontalConvolution_List.Add(nHorizontalConvolution_Array);

                    nVerticalConvolution_Array = ComputeConvolution(nFrameMean_Array, m_nTXTraceNumber, m_nRXTraceNumber, nVerticalFilter_Array, nVerticalFilter_Array.GetLength(0),
                                                                 nVerticalFilter_Array.GetLength(1));
                    nVerticalConvolution_List.Add(nVerticalConvolution_Array);

                    int[,] nTotalConvolution_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                    //計算Sobel G Array(G = sqrt(Gx*Gx + Gy*Gy))
                    for (int nY = 0; nY < m_nTXTraceNumber; nY++)
                    {
                        for (int nX = 0; nX < m_nRXTraceNumber; nX++)
                        {
                            int nSqrtValue = (int)Math.Round(Math.Sqrt(nHorizontalConvolution_Array[nY, nX] * nHorizontalConvolution_Array[nY, nX] +
                                                                       nVerticalConvolution_Array[nY, nX] * nVerticalConvolution_Array[nY, nX]), 0, MidpointRounding.AwayFromZero);
                            nTotalConvolution_Array[nY, nX] = nSqrtValue;
                        }
                    }

                    nTotalConvolution_List.Add(nTotalConvolution_Array);

                    if (ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 2)
                    {
                        if (SaveConvolutionData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "FrameMean", listnFrameMean_List, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                            return false;

                        if (SaveConvolutionData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "FrameMeanConv Gx", nHorizontalConvolution_List, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                            return false;

                        if (SaveConvolutionData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "FrameMeanConv Gy", nVerticalConvolution_List, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                            return false;

                        if (SaveConvolutionData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "FrameMeanConv G", nTotalConvolution_List, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                            return false;
                    }
                    */
                #endregion

                for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                {
                    int[,] nHorizontalSingleFrameData_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];
                    int[,] nVerticalSingleFrameData_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];
                    uint[,] nSingleFrameData_Array = new uint[m_nTXTraceNumber, m_nRXTraceNumber];
                    long[,] lSingleFrameRXDifferData_Array = new long[m_nTXTraceNumber, m_nRXTraceNumber - 1];
                    //long[,] lSignleFrameTXDifferData_Array = new long[m_nTXTraceNumber - 1, m_nRXTraceNumber];

                    int[,] n2DDVFrameData_Array = Get2DArrayFrom3DArray(nDVFrameData_Array, nFrameIndex);

                    //計算Sobel Gx Array
                    nHorizontalSingleFrameData_Array = ComputeConvolution(n2DDVFrameData_Array, m_nTXTraceNumber, m_nRXTraceNumber, nHorizontalFilter_Array, nHorizontalFilter_Array.GetLength(0),
                                                                          nHorizontalFilter_Array.GetLength(1));
                    nConvolutionGxFrameData_List.Add(nHorizontalSingleFrameData_Array);

                    //計算Sobel Gy Array
                    nVerticalSingleFrameData_Array = ComputeConvolution(n2DDVFrameData_Array, m_nTXTraceNumber, m_nRXTraceNumber, nVerticalFilter_Array, nVerticalFilter_Array.GetLength(0),
                                                                        nVerticalFilter_Array.GetLength(1));
                    nConvolutionGyFrameData_List.Add(nVerticalSingleFrameData_Array);

                    //計算Sobel G Array(G = sqrt(Gx*Gx + Gy*Gy))
                    for (int nY = 0; nY < m_nTXTraceNumber; nY++)
                    {
                        for (int nX = 0; nX < m_nRXTraceNumber; nX++)
                        {
                            uint nSqrtValue = (uint)Math.Round(Math.Sqrt(nHorizontalSingleFrameData_Array[nY, nX] * nHorizontalSingleFrameData_Array[nY, nX] +
                                                                         nVerticalSingleFrameData_Array[nY, nX] * nVerticalSingleFrameData_Array[nY, nX]), 0,
                                                                         MidpointRounding.AwayFromZero);
                            nSingleFrameData_Array[nY, nX] = nSqrtValue;
                        }
                    }

                    nConvolutionFrameData_List.Add(nSingleFrameData_Array);

                    //計算G Array之RX Differ Array
                    for (int nY = 0; nY < m_nTXTraceNumber; nY++)
                    {
                        for (int nX = 0; nX < m_nRXTraceNumber - 1; nX++)
                        {
                            long nRXDiffer = 0;

                            if (nSingleFrameData_Array[nY, nX + 1] > nSingleFrameData_Array[nY, nX])
                                nRXDiffer = (int)(nSingleFrameData_Array[nY, nX] - nSingleFrameData_Array[nY, nX + 1]);
                            else
                                nRXDiffer = nSingleFrameData_Array[nY, nX] - nSingleFrameData_Array[nY, nX + 1];

                            lSingleFrameRXDifferData_Array[nY, nX] = nRXDiffer;
                        }
                    }

                    lConvolutionFrameRXDifferData_List.Add(lSingleFrameRXDifferData_Array);

                    //計算G Array之TX Differ Array
                    /*
                    for (int nX = 0; nX < m_nRXTraceNumber; nX++)
                    {
                        for (int nY = 0; nY < m_nTXTraceNumber - 1; nY++)
                        {
                            long nTXDiffer = 0;

                            if (nSingleFrameData_Array[nY + 1, nX] > nSingleFrameData_Array[nY, nX])
                                nTXDiffer = (int)(nSingleFrameData_Array[nY, nX] - nSingleFrameData_Array[nY + 1, nX]);
                            else
                                nTXDiffer = nSingleFrameData_Array[nY, nX] - nSingleFrameData_Array[nY + 1, nX];

                            lSignleFrameTXDifferData_Array[nY, nX] = nTXDiffer;
                        }
                    }

                    lConvolutionFrameTXDifferData_List.Add(lSignleFrameTXDifferData_Array);
                    */
                }

                //double[] dFrameStd_Array = new double[nConvolutionFrameData_List.Count];
                //double[] dFrameStdHorizontalThreshold_Array = new double[nConvolutionFrameData_List.Count];
                //double[] dFrameStdVerticalThreshold_Array = new double[nConvolutionFrameData_List.Count];
                //double[,] dVerticalStd_Array = new double[nConvolutionFrameData_List.Count, m_nRXTraceNumber];
                double[,] dVerticalMean_Array = new double[nConvolutionFrameData_List.Count, m_nRXTraceNumber];
                List<VerticalMeanDifferInfo> cVerticalMeanDifferInfo_List = new List<VerticalMeanDifferInfo>();
                //double[,] dHorizontalStd_Array = new double[nConvolutionFrameData_List.Count, m_nTXTraceNumber];
                bool[,] bVerticalFlag_Array = new bool[nConvolutionFrameData_List.Count, m_nRXTraceNumber];
                bool[,] bHorizontalFlag_Array = new bool[nConvolutionFrameData_List.Count, m_nTXTraceNumber];
                Array.Clear(bVerticalFlag_Array, 0, bVerticalFlag_Array.Length);
                Array.Clear(bHorizontalFlag_Array, 0, bHorizontalFlag_Array.Length);

                double[] dFrameRXDifferMFStd_Array = new double[nConvolutionFrameData_List.Count];
                double[,] dHorizotnalRXDifferMFStd_Array = new double[nConvolutionFrameData_List.Count, m_nTXTraceNumber];
                //double[] dFrameTXDifferStd_Array = new double[nConvolutionFrameData_List.Count];
                //double[,] dVerticalTXDifferStd_Array = new double[nConvolutionFrameData_List.Count, m_nRXTraceNumber];
                double[] dFramePRXDifferStd_Array = new double[nConvolutionFrameData_List.Count];
                double[,] dVerticalPRXDifferStd_Array = new double[nConvolutionFrameData_List.Count, m_nRXTraceNumber];

                int nHalfFrameCount_Horizontal = m_nMultiFrameCount_Horizontal / 2;

                for (int nFrameIndex = 0; nFrameIndex < nConvolutionFrameData_List.Count; nFrameIndex++)
                {
                    int nStartFrame_Horizontal = 0;
                    int nEndFrame_Horizontal = 0;

                    if (nFrameIndex < nHalfFrameCount_Horizontal)
                    {
                        nStartFrame_Horizontal = nFrameIndex;

                        if (nFrameIndex + m_nMultiFrameCount_Horizontal > nConvolutionFrameData_List.Count - 1)
                            nEndFrame_Horizontal = nConvolutionFrameData_List.Count - 1;
                        else
                            nEndFrame_Horizontal = nFrameIndex + m_nMultiFrameCount_Horizontal;
                    }
                    else if (nConvolutionFrameData_List.Count - nFrameIndex - 1 < nHalfFrameCount_Horizontal)
                    {
                        nEndFrame_Horizontal = nConvolutionFrameData_List.Count - 1;

                        if (nConvolutionFrameData_List.Count - 1 - m_nMultiFrameCount_Horizontal < 0)
                            nStartFrame_Horizontal = 0;
                        else
                            nStartFrame_Horizontal = nConvolutionFrameData_List.Count - 1 - m_nMultiFrameCount_Horizontal;
                    }
                    else
                    {
                        nStartFrame_Horizontal = nFrameIndex - nHalfFrameCount_Horizontal;
                        nEndFrame_Horizontal = nFrameIndex + nHalfFrameCount_Horizontal;
                    }


                    //計算G Array整張Frame之STD、Frame STD Horizontal Threshold、Frame STD Vertical Threshold
                    /*
                    List<double> dFrame_List = new List<double>();

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                            dFrame_List.Add(nConvolutionFrameData_Array[nFrameIndex][nTXIndex, nRXIndex]);
                    }

                    dFrameStd_Array[nFrameIndex] = Math.Round(MathMethod.GetSD(dFrame_List), 3, MidpointRounding.AwayFromZero);
                    dFrameStdHorizontalThreshold_Array[nFrameIndex] = Math.Round(dFrameStd_Array[nFrameIndex] * m_dSTDTHRatio_Horizontal, 3, MidpointRounding.AwayFromZero);
                    dFrameStdVerticalThreshold_Array[nFrameIndex] = Math.Round(dFrameStd_Array[nFrameIndex] * m_dSTDTHRatio_Vertical, 3, MidpointRounding.AwayFromZero);
                    */

                    //計算G Array各條TX Trace之STD
                    /*
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        List<double> dHorizontalData_List = new List<double>();

                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                            dHorizontalData_List.Add(nConvolutionFrameData_Array[nFrameIndex][nTXIndex, nRXIndex]);

                        dHorizontalStd_Array[nFrameIndex, nTXIndex] = Math.Round(MathMethod.GetSD(dHorizontalData_List), 3, MidpointRounding.AwayFromZero);
                    }
                    */

                    //計算G Array各條TX Trace之STD
                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        List<double> dVerticalData_List = new List<double>();

                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                            dVerticalData_List.Add(nConvolutionFrameData_List[nFrameIndex][nTXIndex, nRXIndex]);

                        //dVerticalStd_Array[nFrameIndex, nRXIndex] = Math.Round(MathMethod.GetSD(dVerticalData_List), 3, MidpointRounding.AwayFromZero);
                        dVerticalMean_Array[nFrameIndex, nRXIndex] = Math.Round(dVerticalData_List.Average(), 3, MidpointRounding.AwayFromZero);
                    }

                    //計算RX Differ Array多張Frame之STD
                    List<double> dFrameRXDiffer_List = new List<double>();

                    for (int nMFIndex = nStartFrame_Horizontal; nMFIndex <= nEndFrame_Horizontal; nMFIndex++)
                    {
                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                        {
                            for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber - 1; nRXIndex++)
                                dFrameRXDiffer_List.Add(lConvolutionFrameRXDifferData_List[nMFIndex][nTXIndex, nRXIndex]);
                        }
                    }

                    dFrameRXDifferMFStd_Array[nFrameIndex] = Math.Round(MathMethod.ComputeStd(dFrameRXDiffer_List), 3, MidpointRounding.AwayFromZero);

                    //計算多張Frame RX Differ Array各條TX Trace之STD
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        List<double> dHorizontalRXDiffer_List = new List<double>();

                        for (int nMFIndex = nStartFrame_Horizontal; nMFIndex <= nEndFrame_Horizontal; nMFIndex++)
                        {
                            for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber - 1; nRXIndex++)
                                dHorizontalRXDiffer_List.Add(lConvolutionFrameRXDifferData_List[nMFIndex][nTXIndex, nRXIndex]);
                        }

                        dHorizotnalRXDifferMFStd_Array[nFrameIndex, nTXIndex] = Math.Round(MathMethod.ComputeStd(dHorizontalRXDiffer_List), 3, MidpointRounding.AwayFromZero);
                    }

                    //計算TX Differ Array整張Frame之STD
                    /*
                    List<double> dFrameTXDiffer_List = new List<double>();

                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber - 1; nTXIndex++)
                            dFrameTXDiffer_List.Add(lConvolutionFrameTXDifferData_Array[nFrameIndex][nY, nRXIndex]);
                    }

                    dFrameTXDifferStd_Array[nFrameIndex] = Math.Round(MathMethod.GetSD(dFrameTXDiffer_List), 3, MidpointRounding.AwayFromZero);
                    */

                    //計算TX Differ Array各條RX Trace之STD
                    /*
                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        List<double> dVerticalTXDiffer_List = new List<double>();

                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber - 1; nTXIndex++)
                            dVerticalTXDiffer_List.Add(lConvolutionFrameTXDifferData_Array[nFrameIndex][nTXIndex, nRXIndex]);

                        dVerticalTXDifferStd_Array[nFrameIndex, nRXIndex] = Math.Round(MathMethod.GetSD(dVerticalTXDiffer_List), 3, MidpointRounding.AwayFromZero);
                    }
                    */
                }

                for (int nFrameIndex = 0; nFrameIndex < nConvolutionFrameData_List.Count; nFrameIndex++)
                {
                    VerticalMeanDifferInfo cVerticalMeanDifferInfo = new VerticalMeanDifferInfo();
                    int[,] nSingleRXDifferData_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                    int[,] n2DDVFrameData_Array = Get2DArrayFrom3DArray(nDVFrameData_Array, nFrameIndex);

                    //使用G Array TX方向之STD初步判定是否為Signal區域
                    /*
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                        bHorizontalFlag_Array[nFrameIndex, nTXIndex] = xInitDetermineSignalAreaFlag(dHorizontalStd_Array, dFrameStdHorizontalThreshold_Array, bHorizontalFlag_Array[nFrameIndex, nTXIndex], nFrameIndex, nTXIndex, m_nTXTraceNumber, nConvolutionFrameData_Array.Count);
                    */

                    //使用G Array RX方向之STD初步判定是否為Signal區域
                    /*
                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                        bVerticalFlag_Array[nFrameIndex, nRXIndex] = xInitDetermineSignalAreaFlag_NoDebounce(dVerticalStd_Array, dFrameStdVerticalThreshold_Array, bVerticalFlag_Array[nFrameIndex, nRXIndex], nFrameIndex, nRXIndex, m_nRXTraceNumber, nConvolutionFrameData_Array.Count);
                    */
                    double dMinMeanValue = 0.0;
                    int nMinTraceIndex = 0;

                    for (int nRXIndex = 1; nRXIndex < m_nRXTraceNumber - 1; nRXIndex++)
                    {
                        double dCurrentMeanValue = dVerticalMean_Array[nFrameIndex, nRXIndex];

                        if (nRXIndex == 1)
                        {
                            dMinMeanValue = dCurrentMeanValue;
                            nMinTraceIndex = nRXIndex;
                        }
                        else if (dCurrentMeanValue < dMinMeanValue)
                        {
                            dMinMeanValue = dCurrentMeanValue;
                            nMinTraceIndex = nRXIndex;
                        }
                    }

                    int nMinDVMeanTraceIndex = 0;

                    if (dMinMeanValue == 0)
                    {
                        List<int> nCurrent_List = new List<int>();
                        List<int> nNext_List = new List<int>();

                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                        {
                            nCurrent_List.Add(n2DDVFrameData_Array[nTXIndex, nMinTraceIndex]);
                            nNext_List.Add(n2DDVFrameData_Array[nTXIndex, nMinTraceIndex + 1]);
                        }

                        double dCurrentMeanValue = nCurrent_List.Average();
                        double dNextMeanValue = nNext_List.Average();
                        /*
                        int nCurrentMedianValue = nCurrent_List.GetMedian();
                        int nNextMedianValue = nNext_List.GetMedian();
                        double dCurrentStdValue = MathMethod.GetSD(nCurrent_List);
                        double dNextStdValue = MathMethod.GetSD(nNext_List);

                        double dCurrentSKValue = 3 * (dCurrentMeanValue - nCurrentMedianValue) / dCurrentStdValue;
                        double dNextSKValue = 3 * (dNextMeanValue - nNextMedianValue) / dNextStdValue;
                        */

                        if (dCurrentMeanValue < dNextMeanValue)
                            nMinDVMeanTraceIndex = nMinTraceIndex;
                        else
                            nMinDVMeanTraceIndex = nMinTraceIndex + 1;

                        cVerticalMeanDifferInfo.m_nTrace = nMinTraceIndex;
                        cVerticalMeanDifferInfo.m_dCurrentMeanValue = dCurrentMeanValue;
                        cVerticalMeanDifferInfo.m_dNextMeanValue = dNextMeanValue;
                    }
                    else if (nMinTraceIndex == m_nRXTraceNumber - 1)
                    {
                        List<int> nCurrent_List = new List<int>();
                        List<int> nPrevious_List = new List<int>();

                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                        {
                            nCurrent_List.Add(n2DDVFrameData_Array[nTXIndex, nMinTraceIndex]);
                            nPrevious_List.Add(n2DDVFrameData_Array[nTXIndex, nMinTraceIndex - 1]);
                        }

                        double dCurrentMeanValue = nCurrent_List.Average();
                        double dPreviousMeanValue = nPrevious_List.Average();
                        /*
                        int nCurrentMedianValue = nCurrent_List.GetMedian();
                        int nPreviousMedianValue = nPrevious_List.GetMedian();
                        double dCurrentStdValue = MathMethod.GetSD(nCurrent_List);
                        double dPreviousStdValue = MathMethod.GetSD(nPrevious_List);

                        double dCurrentSKValue = 3 * (dCurrentMeanValue - nCurrentMedianValue) / dCurrentStdValue;
                        double dPreviousSKValue = 3 * (dPreviousMeanValue - nPreviousMedianValue) / dPreviousStdValue;
                        */

                        if (dCurrentMeanValue < dPreviousMeanValue)
                            nMinDVMeanTraceIndex = nMinTraceIndex;
                        else
                            nMinDVMeanTraceIndex = nMinTraceIndex - 1;

                        cVerticalMeanDifferInfo.m_nTrace = nMinTraceIndex;
                        cVerticalMeanDifferInfo.m_dPreviousMeanValue = dPreviousMeanValue;
                        cVerticalMeanDifferInfo.m_dCurrentMeanValue = dCurrentMeanValue;
                    }
                    else
                    {
                        List<int> nPrevious_List = new List<int>();
                        List<int> nCurrent_List = new List<int>();
                        List<int> nNext_List = new List<int>();

                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                        {
                            nPrevious_List.Add(n2DDVFrameData_Array[nTXIndex, nMinTraceIndex - 1]);
                            nCurrent_List.Add(n2DDVFrameData_Array[nTXIndex, nMinTraceIndex]);
                            nNext_List.Add(n2DDVFrameData_Array[nTXIndex, nMinTraceIndex + 1]);
                        }

                        double dPreviousMeanValue = nPrevious_List.Average();
                        double dCurrentMeanValue = nCurrent_List.Average();
                        double dNextMeanValue = nNext_List.Average();
                        /*
                        int nPreviousMedianValue = nPrevious_List.GetMedian();
                        int nCurrentMedianValue = nCurrent_List.GetMedian();
                        int nNextMedianValue = nNext_List.GetMedian();
                        double dPreviousStdValue = MathMethod.GetSD(nPrevious_List);
                        double dCurrentStdValue = MathMethod.GetSD(nCurrent_List);
                        double dNextStdValue = MathMethod.GetSD(nNext_List);

                        double dPreviousSKValue = 3 * (dPreviousMeanValue - nPreviousMedianValue) / dPreviousStdValue;
                        double dCurrentSKValue = 3 * (dCurrentMeanValue - nCurrentMedianValue) / dCurrentStdValue;
                        double dNextSKValue = 3 * (dNextMeanValue - nNextMedianValue) / dNextStdValue;
                        */

                        if (dCurrentMeanValue < dNextMeanValue)
                        {
                            if (dPreviousMeanValue < dCurrentMeanValue)
                                nMinDVMeanTraceIndex = nMinTraceIndex - 1;
                            else
                                nMinDVMeanTraceIndex = nMinTraceIndex;
                        }
                        else
                        {
                            if (dPreviousMeanValue < dNextMeanValue)
                                nMinDVMeanTraceIndex = nMinTraceIndex - 1;
                            else
                                nMinDVMeanTraceIndex = nMinTraceIndex + 1;
                        }

                        cVerticalMeanDifferInfo.m_nTrace = nMinTraceIndex;
                        cVerticalMeanDifferInfo.m_dPreviousMeanValue = dPreviousMeanValue;
                        cVerticalMeanDifferInfo.m_dCurrentMeanValue = dCurrentMeanValue;
                        cVerticalMeanDifferInfo.m_dNextMeanValue = dNextMeanValue;
                    }

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                            nSingleRXDifferData_Array[nTXIndex, nRXIndex] = n2DDVFrameData_Array[nTXIndex, nRXIndex] - n2DDVFrameData_Array[nTXIndex, nMinDVMeanTraceIndex];
                    }

                    nFrameRXDifferData_List.Add(nSingleRXDifferData_Array);
                    cVerticalMeanDifferInfo_List.Add(cVerticalMeanDifferInfo);
                }

                for (int nFrameIndex = 0; nFrameIndex < nConvolutionFrameData_List.Count; nFrameIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (dHorizotnalRXDifferMFStd_Array[nFrameIndex, nTXIndex] >= dFrameRXDifferMFStd_Array[nFrameIndex])
                            bHorizontalFlag_Array[nFrameIndex, nTXIndex] = true;
                    }

                    //使用RX Differ Array 複判TX方向其STD是否低於整面STD，若為是則改判為非Signal區域
                    /*
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (bHorizontalFlag_Array[nFrameIndex, nTXIndex] == true)
                        {
                            if (dHorizontalRXDifferStd_Array[nFrameIndex, nTXIndex] < dFrameRXDifferStd_Array[nFrameIndex])
                                bHorizontalFlag_Array[nFrameIndex, nTXIndex] = false;
                        }
                    }
                    */

                    //使用TX Differ Array 複判RX方向其STD是否低於整面STD，若為是則改判為非Signal區域
                    /*
                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        if (bVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                        {
                            if (dVerticalTXDifferStd_Array[nFrameIndex, nRXIndex] < dFrameTXDifferStd_Array[nFrameIndex])
                                bVerticalFlag_Array[nFrameIndex, nRXIndex] = false;
                        }
                    }
                    */

                    //計算RX Differ Array整張Frame之STD
                    List<double> dFramePRXDiffer_List = new List<double>();

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                            dFramePRXDiffer_List.Add(nFrameRXDifferData_List[nFrameIndex][nTXIndex, nRXIndex]);
                    }

                    dFramePRXDifferStd_Array[nFrameIndex] = Math.Round(MathMethod.ComputeStd(dFramePRXDiffer_List), 3, MidpointRounding.AwayFromZero);

                    //計算RX Differ Array各條RX Trace之STD
                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        List<double> dPRXDiffer_List = new List<double>();

                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                            dPRXDiffer_List.Add(nFrameRXDifferData_List[nFrameIndex][nTXIndex, nRXIndex]);

                        dVerticalPRXDifferStd_Array[nFrameIndex, nRXIndex] = Math.Round(MathMethod.ComputeStd(dPRXDiffer_List), 3, MidpointRounding.AwayFromZero);
                    }
                }

                for (int nFrameIndex = 0; nFrameIndex < nConvolutionFrameData_List.Count; nFrameIndex++)
                {
                    //使用RX Differ Array RX方向之STD初步判定是否為Signal區域
                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        bVerticalFlag_Array[nFrameIndex, nRXIndex] = InitializeDetermineSignalAreaFlag(dVerticalPRXDifferStd_Array,
                                                                                                       dFramePRXDifferStd_Array,
                                                                                                       bVerticalFlag_Array[nFrameIndex, nRXIndex],
                                                                                                       nFrameIndex,
                                                                                                       nRXIndex,
                                                                                                       m_nRXTraceNumber,
                                                                                                       nConvolutionFrameData_List.Count);
                    }

                    //判斷若是TX方向上其RX方向判定為Signal區域之DV值皆為負值則將此條TX方向改判定為非Signal區域
                    /*
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                        ExcludeAllNegValueTrace(ref bHorizontalFlag_Array, bVerticalFlag_Array, nDVFrameData_List, nFrameIndex, nTXIndex);
                    */
                }

                for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                {
                    #region Mark It.
                    /*
                        double dEdgeThreshold_Horizontal = Math.Round(dFrameStd_Array[nFrameIndex] * m_dEdgeSTDTHRatio_Horizontal, 3, MidpointRounding.AwayFromZero);
                        double dEdgeThreshold_Vertical = Math.Round(dFrameStd_Array[nFrameIndex] * m_dEdgeSTDTHRatio_Vertical, 3, MidpointRounding.AwayFromZero);

                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber - 1; nTXIndex++)
                        {
                            if (bHorizontalFlag_Array[nFrameIndex, nTXIndex] == true && bHorizontalFlag_Array[nFrameIndex, nTXIndex + 1] == false)
                            {
                                if (dHorizontalStd_Array[nFrameIndex, nTXIndex + 1] > dEdgeThreshold_Horizontal)
                                {
                                    bHorizontalFlag_Array[nFrameIndex, nTXIndex + 1] = true;
                                    nY++;
                                }
                            }
                            else if (bHorizontalFlag_Array[nFrameIndex, nTXIndex] == false && bHorizontalFlag_Array[nFrameIndex, nTXIndex + 1] == true)
                            {
                                if (dHorizontalStd_Array[nFrameIndex, nTXIndex] > dEdgeThreshold_Vertical)
                                    bHorizontalFlag_Array[nFrameIndex, nTXIndex] = true;
                            }
                        }
                        */

                    /*
                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber - 1; nRXIndex++)
                    {
                        if (bVerticalFlag_Array[nFrameIndex, nRXIndex] == true && bVerticalFlag_Array[nFrameIndex, nRXIndex + 1] == false)
                        {
                            if (dVerticalStd_Array[nFrameIndex, nRXIndex + 1] > dEdgeThreshold_Vertical)
                            {
                                bVerticalFlag_Array[nFrameIndex, nRXIndex + 1] = true;
                                nX++;
                            }
                        }
                        else if (bVerticalFlag_Array[nFrameIndex, nRXIndex] == false && bVerticalFlag_Array[nFrameIndex, nRXIndex + 1] == true)
                        {
                            if (dVerticalStd_Array[nFrameIndex, nRXIndex] > dEdgeThreshold_Vertical)
                                bVerticalFlag_Array[nFrameIndex, nRXIndex] = true;
                        }
                    }
                    */

                    /*
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (bHorizontalFlag_Array[nFrameIndex, nTXIndex] == true)
                        {
                            bool bAllNegativeFlag = true;

                            for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                            {
                                if (bVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                                {
                                    if (nDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex] > 0)
                                    {
                                        bAllNegativeFlag = false;
                                        break;
                                    }
                                }
                            }

                            if (bAllNegativeFlag == true)
                                bHorizontalFlag_Array[nFrameIndex, nTXIndex] = false;
                        }
                    }
                    */
                    #endregion

                    //判斷TX方向之Signal區域其相鄰Trace需大於或等於2條以上之Trace數，若否則改判此條TX方向Trace為非Signal區域
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (bHorizontalFlag_Array[nFrameIndex, nTXIndex] == true)
                        {
                            if (nTXIndex == 0)
                            {
                                if (bHorizontalFlag_Array[nFrameIndex, nTXIndex + 1] == false)
                                    bHorizontalFlag_Array[nFrameIndex, nTXIndex] = false;
                            }
                            else if (nTXIndex == m_nTXTraceNumber - 1)
                            {
                                if (bHorizontalFlag_Array[nFrameIndex, nTXIndex - 1] == false)
                                    bHorizontalFlag_Array[nFrameIndex, nTXIndex] = false;
                            }
                            else if (bHorizontalFlag_Array[nFrameIndex, nTXIndex + 1] == false &&
                                     bHorizontalFlag_Array[nFrameIndex, nTXIndex - 1] == false)
                                bHorizontalFlag_Array[nFrameIndex, nTXIndex] = false;
                        }
                    }
                }

                for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                {
                    int nCount = 0;

                    //計算TX方向之此條Trace判定為Signal區域之Frame數
                    for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                    {
                        if (bHorizontalFlag_Array[nFrameIndex, nTXIndex] == true)
                            nCount++;
                    }

                    //計算此條Trace判定為Signal區域之Frame數/總Frame數之比例值
                    double dRatioValue = (double)nCount / nDVFrameData_Array.GetLength(0);

                    //若此條Trace之比例值高於等於fSignalAreaRatio，則將此條TX方向之Trace皆改判定為Signal區域
                    if (dRatioValue >= dSignalAreaRatio_Horizontal)
                    {
                        for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                            bHorizontalFlag_Array[nFrameIndex, nTXIndex] = true;

                        //判斷若是TX方向上其RX方向判定為Signal區域之DV值有正值則將此條TX方向改判定為Signal區域
                        /*
                        for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_List.Count; nFrameIndex++)
                            xExcludeAllNegValueTrace(ref bHorizontalFlag_Array, bVerticalFlag_Array, nDVFrameData_List, nFrameIndex, nTXIndex, false);
                        */
                    }
                    //若此條Trace之比例值低於fNonSignalAreaRatio，則將此條TX方向之Trace皆改判定為非Signal區域
                    else if (dRatioValue < dNoneSignalAreaRatio)
                    {
                        for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                            bHorizontalFlag_Array[nFrameIndex, nTXIndex] = false;
                    }
                }

                /*
                for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                {
                    int nCount = 0;

                    //計算TX方向之此條Trace判定為Signal區域之Frame數
                    for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_List.Count; nFrameIndex++)
                    {
                        if (bVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                            nCount++;
                    }

                    //計算此條Trace判定為Signal區域之Frame數/總Frame數之比例值
                    double dRatioValue = (double)nCount / nDVFrameData_List.Count;

                    //若此條Trace之比例值高於等於fSignalAreaRatio，則將此條TX方向之Trace皆改判定為Signal區域
                    if (dRatioValue >= dSignalAreaRatio)
                    {
                        for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_List.Count; nFrameIndex++)
                            bVerticalFlag_Array[nFrameIndex, nRXIndex] = true;
                    }
                    //若此條Trace之比例值低於fNoneSignalAreaRatio，則將此條TX方向之Trace皆改判定為非Signal區域
                    else if (dRatioValue < dNoneSignalAreaRatio)
                    {
                        for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_List.Count; nFrameIndex++)
                            bVerticalFlag_Array[nFrameIndex, nRXIndex] = false;
                    }
                }
                */

                /*
                bool bNoHorizontalSignalArea = true;

                for (int nFrameIndex = 0; nFrameIndex < nConvolutionFrameData_List.Count; nFrameIndex++)
                {
                    bNoHorizontalSignalArea = true;

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (bHorizontalFlag_Array[nFrameIndex, nTXIndex] == true)
                        {
                            bNoHorizontalSignalArea = false;
                            break;
                        }

                    }

                    if (bNoHorizontalSignalArea == true)
                    {
                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                            bHorizontalFlag_Array[nFrameIndex, nTXIndex] = xInitDetermineSignalAreaFlag(dHorizontalRXDifferStd_Array, dFrameRXDifferStd_Array, bHorizontalFlag_Array[nFrameIndex, nTXIndex], nFrameIndex, nTXIndex, m_nTXTraceNumber, nConvolutionFrameData_List.Count);

                        //判斷若是TX方向上其RX方向判定為Signal區域之DV值皆為負值則將此條TX方向改判定為非Signal區域
                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                            xExcludeAllNegValueTrace(ref bHorizontalFlag_Array, bVerticalFlag_Array, nDVFrameData_List, nFrameIndex, nTXIndex);
                    }
                }
                */

                //輸出Signal區域之Frame Data，Signal區域填入其DV值，非Signal區域填入0
                for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                {
                    int[,] nSingleFrameData_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                        {
                            if (bHorizontalFlag_Array[nFrameIndex, nTXIndex] == true &&
                                bVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                            {
                                #region Mark It.
                                /*
                                    if (nFrameIndex == 0)
                                    {
                                        if (bHorizontalFlag_Array[nFrameIndex, nTXIndex + 1] == true &&
                                            bVerticalFlag_Array[nFrameIndex, nRXIndex + 1] == true)
                                            nSingleFrameData_Array[nTXIndex, nRXIndex] = nDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                                        else
                                            nSingleFrameData_Array[nTXIndex, nRXIndex] = 0;
                                    }
                                    else if (nFrameIndex == nDVFrameData_List.Count - 1)
                                    {
                                        if (bHorizontalFlag_Array[nFrameIndex, nTXIndex - 1] == true &&
                                            bVerticalFlag_Array[nFrameIndex, nRXIndex - 1] == true)
                                            nSingleFrameData_Array[nTXIndex, nRXIndex] = nDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                                        else
                                            nSingleFrameData_Array[nTXIndex, nRXIndex] = 0;
                                    }
                                    else
                                    {
                                        if (bHorizontalFlag_Array[nFrameIndex, nTXIndex + 1] == true &&
                                            bVerticalFlag_Array[nFrameIndex, nRXIndex + 1] == true)
                                            nSingleFrameData_Array[nTXIndex, nRXIndex] = nDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                                        else if (bHorizontalFlag_Array[nFrameIndex, nRXIndex - 1] == true && 
                                                 bVerticalFlag_Array[nFrameIndex, nTXIndex - 1] == true)
                                            nSingleFrameData_Array[nTXIndex, nRXIndex] = nDVFrameData_List[nFrameIndex][nTXIndex, nRXIndex];
                                        else
                                            nSingleFrameData_Array[nTXIndex, nRXIndex] = 0;
                                    }
                                    */
                                #endregion

                                nSingleFrameData_Array[nTXIndex, nRXIndex] = nDVFrameData_Array[nFrameIndex, nTXIndex, nRXIndex];
                            }
                            else
                                nSingleFrameData_Array[nTXIndex, nRXIndex] = 0;
                        }
                    }

                    nSignalFrameData_List.Add(nSingleFrameData_Array);
                }

                //m_cDataInfo_List[nDataIndex].m_FrameStdArray = dFrameStd_Array;
                //m_cDataInfo_List[nDataIndex].m_FrameStdHorTHArray = dFrameStdHorizontalThreshold_Array;
                //m_cDataInfo_List[nDataIndex].m_FrameStdVerTHArray = dFrameStdVerticalThreshold_Array;
                //m_cDataInfo_List[nDataIndex].m_HorStdArray = dHorizontalStd_Array;
                //m_cDataInfo_List[nDataIndex].m_VerStdArray = dVerticalStd_Array;
                m_cDataInfo_List[nDataIndex].m_bFrameHorizontalFlag_Array = bHorizontalFlag_Array;
                m_cDataInfo_List[nDataIndex].m_bFrameVerticalFlag_Array = bVerticalFlag_Array;

                for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                {
                    for (int nFlagIndex = 0; nFlagIndex < bHorizontalFlag_Array.GetLength(1); nFlagIndex++)
                    {
                        if (bHorizontalFlag_Array[nFrameIndex, nFlagIndex] == true)
                        {
                            nTXSignalAreaFrameCount++;
                            break;
                        }
                    }

                    for (int nFlagIndex = 0; nFlagIndex < bVerticalFlag_Array.GetLength(1); nFlagIndex++)
                    {
                        if (bVerticalFlag_Array[nFrameIndex, nFlagIndex] == true)
                        {
                            nRXSignalAreaFrameCount++;
                            break;
                        }
                    }
                }

                if (ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 2)
                {
                    if (SaveConvolutionData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvGx", nConvolutionGxFrameData_List, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                        return false;

                    if (SaveConvolutionData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvGy", nConvolutionGyFrameData_List, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                        return false;

                    /*
                    if (SaveParticularData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvG", nConvolutionFrameData_List, dHorizontalStd_Array, dVerticalStd_Array, dVerticalMean_Array, dVerticalMeanDiffer_Array, cVerticalMeanDifferInfo_List,
                                           dFrameStd_Array, dFrameStdHorizontalThreshold_Array, dFrameStdVerticalThreshold_Array, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                    */
                    if (SaveParticularData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvG", nConvolutionFrameData_List, dVerticalMean_Array, cVerticalMeanDifferInfo_List, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                        return false;

                    if (SaveParticularDifferData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvGRXDiffer", lConvolutionFrameRXDifferData_List, dHorizotnalRXDifferMFStd_Array, dFrameRXDifferMFStd_Array,
                                                 m_nTXTraceNumber, m_nRXTraceNumber - 1, m_nDIFFERTYPE_RX) == false)
                        return false;

                    /*
                    if (SaveParticularDifferData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvGTXDiffer", lConvolutionFrameTXDifferData_List, dVerticalTXDifferStd_Array, dFrameTXDifferStd_Array,
                                                 m_nTXTraceNumber - 1, m_nRXTraceNumber, m_nDIFFERTYPE_TX) == false)
                        return false;
                    */

                    if (SaveParticularDifferData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "RXDiffer", nFrameRXDifferData_List, dVerticalPRXDifferStd_Array, dFramePRXDifferStd_Array,
                                                 m_nTXTraceNumber, m_nRXTraceNumber, m_nDIFFERTYPE_TX_RXDiffer) == false)
                        return false;
                }
            }
            #endregion
            #region Use Base Differ Edge Detect
            else if (m_eGetSignalAreaMethod == GetSignalAreaMethod.UseBaseDifferEdgeDetect)
            {
                int[,] nOBASEData_Array = m_cDataInfo_List[nDataIndex].m_nOBASEData_Array;
                int[,] nBASEData_Array = m_cDataInfo_List[nDataIndex].m_nBASEData_Array;
                int[,] nBASEDiffer_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];
                int[,] nBASEDifferOffset_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                for (int nRxIndex = 0; nRxIndex < m_nRXTraceNumber; nRxIndex++)
                {
                    //int nRXSumValue = 0;
                    int[] nRXBASEDiffer_Array = new int[m_nTXTraceNumber];

                    for (int nTxIndex = 0; nTxIndex < m_nTXTraceNumber; nTxIndex++)
                    {
                        nBASEDiffer_Array[nTxIndex, nRxIndex] = nOBASEData_Array[nTxIndex, nRxIndex] - nBASEData_Array[nTxIndex, nRxIndex];
                        //nRXSumValue += nBASEDiffer_Array[nTxIndex, nRxIndex];
                        nRXBASEDiffer_Array[nTxIndex] = nBASEDiffer_Array[nTxIndex, nRxIndex];
                    }

                    //int nRXMeanValue = (int)Math.Round((double)nRXSumValue / m_nTXTraceNumber, 0, MidpointRounding.AwayFromZero);
                    int nRXMedianValue = MathMethod.ComputeMedian(nRXBASEDiffer_Array);

                    for (int nTxIndex = 0; nTxIndex < m_nTXTraceNumber; nTxIndex++)
                    {
                        //nBASEDifferOffset_Array[nTxIndex, nRxIndex] = nBASEDiffer_Array[nTxIndex, nRxIndex] - nRXMeanValue;
                        nBASEDifferOffset_Array[nTxIndex, nRxIndex] = nBASEDiffer_Array[nTxIndex, nRxIndex] - nRXMedianValue;
                    }
                }

                for (int nTxIndex = 0; nTxIndex < m_nTXTraceNumber; nTxIndex++)
                {
                    //int nTXSumValue = 0;
                    int[] nTXBASEDiffer_Array = new int[m_nRXTraceNumber];

                    for (int nRxIndex = 0; nRxIndex < m_nRXTraceNumber; nRxIndex++)
                    {
                        //nTXSumValue += nBASEDifferOffset_Array[nTxIndex, nRxIndex];
                        nTXBASEDiffer_Array[nRxIndex] = nBASEDifferOffset_Array[nTxIndex, nRxIndex];
                    }

                    //int nTXMeanValue = (int)Math.Round((double)nTXSumValue / m_nRXTraceNumber, 0, MidpointRounding.AwayFromZero);
                    int nTXMedianValue = MathMethod.ComputeMedian(nTXBASEDiffer_Array);

                    for (int nRxIndex = 0; nRxIndex < m_nRXTraceNumber; nRxIndex++)
                    {
                        //nBASEDifferOffset_Array[nTxIndex, nRxIndex] = nBASEDifferOffset_Array[nTxIndex, nRxIndex] - nTXMeanValue;
                        nBASEDifferOffset_Array[nTxIndex, nRxIndex] = nBASEDifferOffset_Array[nTxIndex, nRxIndex] - nTXMedianValue;
                    }
                }

                int[,] nHorizontalGxFrameData_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];
                int[,] nVerticalGyFrameData_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];
                uint[,] nGFrameData_Array = new uint[m_nTXTraceNumber, m_nRXTraceNumber];

                //計算Sobel Gx Array
                nHorizontalGxFrameData_Array = ComputeConvolution(nBASEDifferOffset_Array, m_nTXTraceNumber, m_nRXTraceNumber, nHorizontalFilter_Array, nHorizontalFilter_Array.GetLength(0), nHorizontalFilter_Array.GetLength(1));

                //計算Sobel Gy Array
                nVerticalGyFrameData_Array = ComputeConvolution(nBASEDifferOffset_Array, m_nTXTraceNumber, m_nRXTraceNumber, nVerticalFilter_Array, nVerticalFilter_Array.GetLength(0), nVerticalFilter_Array.GetLength(1));

                bool[] bHorizontalFlag_Array = new bool[m_nTXTraceNumber];
                bool[] bVerticalFlag_Array = new bool[m_nRXTraceNumber];

                ConvGStdReference cConvGStdReference_Horizontal = new ConvGStdReference();
                ConvGStdReference cConvGStdReference_Vertical = new ConvGStdReference();

                if (m_eUseBaseDifferEdgeDetectMethod == UseBaseDifferEdgeDetectMethod.UseConvolutionG)
                {
                    //計算Sobel G Array(G = sqrt(Gx*Gx + Gy*Gy))
                    for (int nY = 0; nY < m_nTXTraceNumber; nY++)
                    {
                        for (int nX = 0; nX < m_nRXTraceNumber; nX++)
                        {
                            uint nSqrtValue = (uint)Math.Round(Math.Sqrt(nHorizontalGxFrameData_Array[nY, nX] * nHorizontalGxFrameData_Array[nY, nX] + nVerticalGyFrameData_Array[nY, nX] * nVerticalGyFrameData_Array[nY, nX]), 0, MidpointRounding.AwayFromZero);
                            nGFrameData_Array[nY, nX] = nSqrtValue;
                        }
                    }

                    double[] dHorizotnalStd_Original_Array = new double[m_nTXTraceNumber];
                    double[] dVerticalStd_Original_Array = new double[m_nRXTraceNumber];

                    //計算BASE Differ Conv G各條RX Trace之STD
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        List<double> dHorizontal_List = new List<double>();

                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                            dHorizontal_List.Add(nGFrameData_Array[nTXIndex, nRXIndex]);

                        dHorizotnalStd_Original_Array[nTXIndex] = Math.Round(MathMethod.ComputeStd(dHorizontal_List), 3, MidpointRounding.AwayFromZero);
                    }

                    double[] dHorizontalStd_NearMean_Array = new double[dHorizotnalStd_Original_Array.Length];
                    Array.Copy(dHorizotnalStd_Original_Array, dHorizontalStd_NearMean_Array, dHorizotnalStd_Original_Array.Length);

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (nTXIndex == 0)
                        {
                            dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex] + dHorizotnalStd_Original_Array[nTXIndex + 1]) / 2.0;
                        }
                        else if (nTXIndex == m_nTXTraceNumber - 1)
                        {
                            dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex - 1] + dHorizotnalStd_Original_Array[nTXIndex]) / 2.0;
                        }
                        else
                        {
                            //if (dHorizotnalStd_Original_Array[nTXIndex - 1] > dHorizotnalStd_Original_Array[nTXIndex + 1])
                            //{
                            //    dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex - 1] + dHorizotnalStd_Original_Array[nTXIndex]) / 2.0;
                            //}
                            //else
                            //{
                            //    dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex] + dHorizotnalStd_Original_Array[nTXIndex + 1]) / 2.0;
                            //}

                            //dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex - 1] + dHorizotnalStd_Original_Array[nTXIndex] + dHorizotnalStd_Original_Array[nTXIndex + 1]) / 3.0;

                            if (dHorizotnalStd_Original_Array[nTXIndex - 1] > dHorizotnalStd_Original_Array[nTXIndex + 1])
                            {
                                dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex] + dHorizotnalStd_Original_Array[nTXIndex + 1]) / 2.0;
                            }
                            else
                            {
                                dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex - 1] + dHorizotnalStd_Original_Array[nTXIndex]) / 2.0;
                            }

                            //dHorizontalStd_NearMean_Array[nTXIndex] = dHorizotnalStd_Original_Array[nTXIndex];
                        }
                    }

                    //計算BASE Differ Conv G各條TX Trace之STD
                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        List<double> dVertical_List = new List<double>();

                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                            dVertical_List.Add(nGFrameData_Array[nTXIndex, nRXIndex]);

                        dVerticalStd_Original_Array[nRXIndex] = Math.Round(MathMethod.ComputeStd(dVertical_List), 3, MidpointRounding.AwayFromZero);
                    }

                    double[] dVerticalStd_NearMean_Array = new double[dVerticalStd_Original_Array.Length];
                    Array.Copy(dVerticalStd_Original_Array, dVerticalStd_NearMean_Array, dVerticalStd_Original_Array.Length);

                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex] + dVerticalStd_Original_Array[nRXIndex + 1]) / 2.0;
                        }
                        else if (nRXIndex == m_nRXTraceNumber - 1)
                        {
                            dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex - 1] + dVerticalStd_Original_Array[nRXIndex]) / 2.0;
                        }
                        else
                        {
                            //if (dVerticalStd_Original_Array[nRXIndex - 1] > dVerticalStd_Original_Array[nRXIndex + 1])
                            //{
                            //    dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex - 1] + dVerticalStd_Original_Array[nRXIndex]) / 2.0;
                            //}
                            //else
                            //{
                            //    dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex] + dVerticalStd_Original_Array[nRXIndex + 1]) / 2.0;
                            //}

                            //dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex - 1] + dVerticalStd_Original_Array[nRXIndex] + dVerticalStd_Original_Array[nRXIndex + 1]) / 3.0;

                            if (dVerticalStd_Original_Array[nRXIndex - 1] > dVerticalStd_Original_Array[nRXIndex + 1])
                            {
                                dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex] + dVerticalStd_Original_Array[nRXIndex + 1]) / 2.0;
                            }
                            else
                            {
                                dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex - 1] + dVerticalStd_Original_Array[nRXIndex]) / 2.0;
                            }

                            //dVerticalStd_NearMean_Array[nRXIndex] = dVerticalStd_Original_Array[nRXIndex];
                        }
                    }

                    cConvGStdReference_Horizontal.m_dStd_Original_Array = dHorizotnalStd_Original_Array;
                    cConvGStdReference_Horizontal.m_dStd_NearMean_Array = dHorizontalStd_NearMean_Array;
                    cConvGStdReference_Vertical.m_dStd_Original_Array = dVerticalStd_Original_Array;
                    cConvGStdReference_Vertical.m_dStd_NearMean_Array = dVerticalStd_NearMean_Array;

                    double dMinByHorizotnalStd = dHorizontalStd_NearMean_Array.ToList().Min();
                    cConvGStdReference_Horizontal.m_dMin = dMinByHorizotnalStd;
                    double dMinByVerticalStd = dVerticalStd_NearMean_Array.ToList().Min();
                    cConvGStdReference_Vertical.m_dMin = dMinByVerticalStd;

                    double dMedianByHorizotnalStd = MathMethod.ComputeMedian(dHorizontalStd_NearMean_Array);
                    cConvGStdReference_Horizontal.m_dMedian = dMedianByHorizotnalStd;
                    double dMedianByVerticalStd = MathMethod.ComputeMedian(dVerticalStd_NearMean_Array);
                    cConvGStdReference_Vertical.m_dMedian = dMedianByVerticalStd;

                    double dSumValue = 0.0;
                    int nCountValue = 0;

                    for (int nValueIndex = 0; nValueIndex < dHorizontalStd_NearMean_Array.Length; nValueIndex++)
                    {
                        if (dHorizontalStd_NearMean_Array[nValueIndex] <= dMedianByHorizotnalStd)
                        {
                            dSumValue += dHorizontalStd_NearMean_Array[nValueIndex];
                            nCountValue++;
                        }
                    }

                    double dMeanUnderMedianByHorizotnalStd = dSumValue / (double)nCountValue;
                    cConvGStdReference_Horizontal.m_dMeanUnderMedian = dMeanUnderMedianByHorizotnalStd;

                    dSumValue = 0.0;
                    nCountValue = 0;

                    for (int nValueIndex = 0; nValueIndex < dVerticalStd_NearMean_Array.Length; nValueIndex++)
                    {
                        if (dVerticalStd_NearMean_Array[nValueIndex] <= dMedianByVerticalStd)
                        {
                            dSumValue += dVerticalStd_NearMean_Array[nValueIndex];
                            nCountValue++;
                        }
                    }

                    double dMeanUnderMedianByVerticalStd = dSumValue / (double)nCountValue;
                    cConvGStdReference_Vertical.m_dMeanUnderMedian = dMeanUnderMedianByVerticalStd;

                    //計算BASE Differ Conv G Horizontal Std之Mean
                    double dMeanByHorizotnalStd = Math.Round(dHorizontalStd_NearMean_Array.ToList().Average(), 3, MidpointRounding.AwayFromZero);
                    cConvGStdReference_Horizontal.m_dMean = dMeanByHorizotnalStd;

                    /*
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        dHorizontalStd_NearMean_Array[nTXIndex] = dHorizontalStd_NearMean_Array[nTXIndex] - dMeanByHorizotnalStd;
                    }
                    */

                    //計算BASE Differ Conv G Vertical Std之Mean
                    double dMeanByVerticalStd = Math.Round(dVerticalStd_NearMean_Array.ToList().Average(), 3, MidpointRounding.AwayFromZero);
                    cConvGStdReference_Vertical.m_dMean = dMeanByVerticalStd;

                    /*
                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        dVerticalStd_NearMean_Array[nRXIndex] = dVerticalStd_NearMean_Array[nRXIndex] - dMeanByVerticalStd;
                    }
                    */

                    //計算BASE Differ Conv G Horizontal Std之STD
                    double dStdByHorizotnalStd = Math.Round(MathMethod.ComputeStd(dHorizontalStd_NearMean_Array.ToList()), 3, MidpointRounding.AwayFromZero);
                    cConvGStdReference_Horizontal.m_dStd = dStdByHorizotnalStd;

                    //計算BASE Differ Conv G Vertical Std之STD
                    double dStdByVerticalStd = Math.Round(MathMethod.ComputeStd(dVerticalStd_NearMean_Array.ToList()), 3, MidpointRounding.AwayFromZero);
                    cConvGStdReference_Vertical.m_dStd = dStdByVerticalStd;

                    double dHorizontalThreshold = dMeanUnderMedianByHorizotnalStd + (dStdByHorizotnalStd * m_dThresholdStdRatio);
                    cConvGStdReference_Horizontal.m_dThreshold = dHorizontalThreshold;
                    double dVerticalThreshold = dMeanUnderMedianByVerticalStd + (dStdByVerticalStd * m_dThresholdStdRatio);
                    cConvGStdReference_Vertical.m_dThreshold = dVerticalThreshold;

                    bool[] bPreviousHorizontalFlag_Array = new bool[m_nTXTraceNumber];
                    bool[] bPreviousVerticalFlag_Array = new bool[m_nRXTraceNumber];

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (dHorizontalStd_NearMean_Array[nTXIndex] > dHorizontalThreshold)
                            bPreviousHorizontalFlag_Array[nTXIndex] = true;
                        else
                            bPreviousHorizontalFlag_Array[nTXIndex] = false;
                    }

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (nTXIndex == 0)
                        {
                            if (bPreviousHorizontalFlag_Array[nTXIndex] == true)
                            {
                                if (bPreviousHorizontalFlag_Array[nTXIndex + 1] == true)
                                    bHorizontalFlag_Array[nTXIndex] = true;
                                else
                                    bHorizontalFlag_Array[nTXIndex] = false;
                            }
                            else
                            {
                                bHorizontalFlag_Array[nTXIndex] = false;
                            }
                        }
                        else if (nTXIndex == m_nTXTraceNumber - 1)
                        {
                            if (bPreviousHorizontalFlag_Array[nTXIndex] == true)
                            {
                                if (bPreviousHorizontalFlag_Array[nTXIndex - 1] == true)
                                    bHorizontalFlag_Array[nTXIndex] = true;
                                else
                                    bHorizontalFlag_Array[nTXIndex] = false;
                            }
                            else
                            {
                                bHorizontalFlag_Array[nTXIndex] = false;
                            }
                        }
                        else
                        {
                            if (bPreviousHorizontalFlag_Array[nTXIndex] == true)
                            {
                                if (bPreviousHorizontalFlag_Array[nTXIndex - 1] == true || bPreviousHorizontalFlag_Array[nTXIndex + 1] == true)
                                    bHorizontalFlag_Array[nTXIndex] = true;
                                else
                                    bHorizontalFlag_Array[nTXIndex] = false;
                            }
                            else
                            {
                                bHorizontalFlag_Array[nTXIndex] = false;
                            }
                        }
                    }

                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        if (dVerticalStd_NearMean_Array[nRXIndex] > dVerticalThreshold)
                            bPreviousVerticalFlag_Array[nRXIndex] = true;
                        else
                            bPreviousVerticalFlag_Array[nRXIndex] = false;
                    }

                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            if (bPreviousVerticalFlag_Array[nRXIndex] == true)
                            {
                                if (bPreviousVerticalFlag_Array[nRXIndex + 1] == true)
                                    bVerticalFlag_Array[nRXIndex] = true;
                                else
                                    bVerticalFlag_Array[nRXIndex] = false;
                            }
                            else
                            {
                                bVerticalFlag_Array[nRXIndex] = false;
                            }
                        }
                        else if (nRXIndex == m_nRXTraceNumber - 1)
                        {
                            if (bPreviousVerticalFlag_Array[nRXIndex] == true)
                            {
                                if (bPreviousVerticalFlag_Array[nRXIndex - 1] == true)
                                    bVerticalFlag_Array[nRXIndex] = true;
                                else
                                    bVerticalFlag_Array[nRXIndex] = false;
                            }
                            else
                            {
                                bVerticalFlag_Array[nRXIndex] = false;
                            }
                        }
                        else
                        {
                            if (bPreviousVerticalFlag_Array[nRXIndex] == true)
                            {
                                if (bPreviousVerticalFlag_Array[nRXIndex - 1] == true || bPreviousVerticalFlag_Array[nRXIndex + 1] == true)
                                    bVerticalFlag_Array[nRXIndex] = true;
                                else
                                    bVerticalFlag_Array[nRXIndex] = false;
                            }
                            else
                            {
                                bVerticalFlag_Array[nRXIndex] = false;
                            }
                        }
                    }

                    /*
                    // 指定要分成的群集
                    int k = 2;

                    // 執行K-Means分群
                    List<KMeansMethod.Cluster> cHorizontalCluster_List = KMeans(dHorizontalStd_NearMean_Array, k);
                    List<KMeansMethod.Cluster> cVerticalCluster_List = KMeans(dVerticalStd_NearMean_Array, k);
                    */

                    m_cDataInfo_List[nDataIndex].m_bTraceHorizontalFlag_Array = bHorizontalFlag_Array;
                    m_cDataInfo_List[nDataIndex].m_bTraceVerticalFlag_Array = bVerticalFlag_Array;
                }
                else if (m_eUseBaseDifferEdgeDetectMethod == UseBaseDifferEdgeDetectMethod.UseConvolutionGxGy)
                {
                    double[] dHorizotnalStd_Original_Array = new double[m_nTXTraceNumber];
                    double[] dVerticalStd_Original_Array = new double[m_nRXTraceNumber];

                    //計算BASE Differ Conv Gx各條RX Trace之STD
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        List<double> dHorizontal_List = new List<double>();

                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                            dHorizontal_List.Add(nHorizontalGxFrameData_Array[nTXIndex, nRXIndex]);

                        dHorizotnalStd_Original_Array[nTXIndex] = Math.Round(MathMethod.ComputeStd(dHorizontal_List), 3, MidpointRounding.AwayFromZero);
                    }

                    //計算BASE Differ Conv Gy各條RX Trace之STD
                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        List<double> dVertical_List = new List<double>();

                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                            dVertical_List.Add(nVerticalGyFrameData_Array[nTXIndex, nRXIndex]);

                        dVerticalStd_Original_Array[nRXIndex] = Math.Round(MathMethod.ComputeStd(dVertical_List), 3, MidpointRounding.AwayFromZero);
                    }

                    double[] dHorizontalStd_NearMean_Array = new double[dHorizotnalStd_Original_Array.Length];
                    Array.Copy(dHorizotnalStd_Original_Array, dHorizontalStd_NearMean_Array, dHorizotnalStd_Original_Array.Length);

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (nTXIndex == 0)
                        {
                            dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex] + dHorizotnalStd_Original_Array[nTXIndex + 1]) / 2.0;
                        }
                        else if (nTXIndex == m_nTXTraceNumber - 1)
                        {
                            dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex - 1] + dHorizotnalStd_Original_Array[nTXIndex]) / 2.0;
                        }
                        else
                        {
                            if (dHorizotnalStd_Original_Array[nTXIndex - 1] > dHorizotnalStd_Original_Array[nTXIndex + 1])
                            {
                                dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex - 1] + dHorizotnalStd_Original_Array[nTXIndex]) / 2.0;
                            }
                            else
                            {
                                dHorizontalStd_NearMean_Array[nTXIndex] = (dHorizotnalStd_Original_Array[nTXIndex] + dHorizotnalStd_Original_Array[nTXIndex + 1]) / 2.0;
                            }
                        }
                    }

                    double[] dVerticalStd_NearMean_Array = new double[dVerticalStd_Original_Array.Length];
                    Array.Copy(dVerticalStd_Original_Array, dVerticalStd_NearMean_Array, dVerticalStd_Original_Array.Length);

                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex] + dVerticalStd_Original_Array[nRXIndex + 1]) / 2.0;
                        }
                        else if (nRXIndex == m_nRXTraceNumber - 1)
                        {
                            dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex - 1] + dVerticalStd_Original_Array[nRXIndex]) / 2.0;
                        }
                        else
                        {
                            if (dVerticalStd_Original_Array[nRXIndex - 1] > dVerticalStd_Original_Array[nRXIndex + 1])
                            {
                                dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex - 1] + dVerticalStd_Original_Array[nRXIndex]) / 2.0;
                            }
                            else
                            {
                                dVerticalStd_NearMean_Array[nRXIndex] = (dVerticalStd_Original_Array[nRXIndex] + dVerticalStd_Original_Array[nRXIndex + 1]) / 2.0;
                            }
                        }
                    }

                    cConvGStdReference_Horizontal.m_dStd_Original_Array = dHorizotnalStd_Original_Array;
                    cConvGStdReference_Horizontal.m_dStd_NearMean_Array = dHorizontalStd_NearMean_Array;
                    cConvGStdReference_Vertical.m_dStd_Original_Array = dVerticalStd_Original_Array;
                    cConvGStdReference_Vertical.m_dStd_NearMean_Array = dVerticalStd_NearMean_Array;

                    double dMinByHorizotnalStd = dHorizontalStd_NearMean_Array.ToList().Min();
                    cConvGStdReference_Horizontal.m_dMin = dMinByHorizotnalStd;
                    double dMinByVerticalStd = dVerticalStd_NearMean_Array.ToList().Min();
                    cConvGStdReference_Vertical.m_dMin = dMinByVerticalStd;

                    //計算BASE Differ Conv Gx Horizontal Std之STD
                    double dStdByHorizotnalStd = Math.Round(MathMethod.ComputeStd(dHorizontalStd_NearMean_Array.ToList()), 3, MidpointRounding.AwayFromZero);
                    cConvGStdReference_Horizontal.m_dStd = dStdByHorizotnalStd;

                    //計算BASE Differ Conv Gy Vertical Std之STD
                    double dStdByVerticalStd = Math.Round(MathMethod.ComputeStd(dVerticalStd_NearMean_Array.ToList()), 3, MidpointRounding.AwayFromZero);
                    cConvGStdReference_Vertical.m_dStd = dStdByVerticalStd;

                    double dHorizontalThreshold = dMinByHorizotnalStd + (dStdByHorizotnalStd * m_dThresholdStdRatio);
                    cConvGStdReference_Horizontal.m_dThreshold = dHorizontalThreshold;
                    double dVerticalThreshold = dMinByVerticalStd + (dStdByVerticalStd * m_dThresholdStdRatio);
                    cConvGStdReference_Vertical.m_dThreshold = dVerticalThreshold;

                    bool[] bPreviousHorizontalFlag_Array = new bool[m_nTXTraceNumber];
                    bool[] bPreviousVerticalFlag_Array = new bool[m_nRXTraceNumber];

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (dHorizontalStd_NearMean_Array[nTXIndex] > dHorizontalThreshold)
                            bPreviousHorizontalFlag_Array[nTXIndex] = true;
                        else
                            bPreviousHorizontalFlag_Array[nTXIndex] = false;
                    }

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (nTXIndex == 0)
                        {
                            if (bPreviousHorizontalFlag_Array[nTXIndex] == true)
                            {
                                if (bPreviousHorizontalFlag_Array[nTXIndex + 1] == true)
                                    bHorizontalFlag_Array[nTXIndex] = true;
                                else
                                    bHorizontalFlag_Array[nTXIndex] = false;
                            }
                            else
                            {
                                bHorizontalFlag_Array[nTXIndex] = false;
                            }
                        }
                        else if (nTXIndex == m_nTXTraceNumber - 1)
                        {
                            if (bPreviousHorizontalFlag_Array[nTXIndex] == true)
                            {
                                if (bPreviousHorizontalFlag_Array[nTXIndex - 1] == true)
                                    bHorizontalFlag_Array[nTXIndex] = true;
                                else
                                    bHorizontalFlag_Array[nTXIndex] = false;
                            }
                            else
                            {
                                bHorizontalFlag_Array[nTXIndex] = false;
                            }
                        }
                        else
                        {
                            if (bPreviousHorizontalFlag_Array[nTXIndex] == true)
                            {
                                if (bPreviousHorizontalFlag_Array[nTXIndex - 1] == true || bPreviousHorizontalFlag_Array[nTXIndex + 1] == true)
                                    bHorizontalFlag_Array[nTXIndex] = true;
                                else
                                    bHorizontalFlag_Array[nTXIndex] = false;
                            }
                            else
                            {
                                bHorizontalFlag_Array[nTXIndex] = false;
                            }
                        }
                    }

                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        if (dVerticalStd_NearMean_Array[nRXIndex] > dVerticalThreshold)
                            bPreviousVerticalFlag_Array[nRXIndex] = true;
                        else
                            bPreviousVerticalFlag_Array[nRXIndex] = false;
                    }

                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            if (bPreviousVerticalFlag_Array[nRXIndex] == true)
                            {
                                if (bPreviousVerticalFlag_Array[nRXIndex + 1] == true)
                                    bVerticalFlag_Array[nRXIndex] = true;
                                else
                                    bVerticalFlag_Array[nRXIndex] = false;
                            }
                            else
                            {
                                bVerticalFlag_Array[nRXIndex] = false;
                            }
                        }
                        else if (nRXIndex == m_nRXTraceNumber - 1)
                        {
                            if (bPreviousVerticalFlag_Array[nRXIndex] == true)
                            {
                                if (bPreviousVerticalFlag_Array[nRXIndex - 1] == true)
                                    bVerticalFlag_Array[nRXIndex] = true;
                                else
                                    bVerticalFlag_Array[nRXIndex] = false;
                            }
                            else
                            {
                                bVerticalFlag_Array[nRXIndex] = false;
                            }
                        }
                        else
                        {
                            if (bPreviousVerticalFlag_Array[nRXIndex] == true)
                            {
                                if (bPreviousVerticalFlag_Array[nRXIndex - 1] == true || bPreviousVerticalFlag_Array[nRXIndex + 1] == true)
                                    bVerticalFlag_Array[nRXIndex] = true;
                                else
                                    bVerticalFlag_Array[nRXIndex] = false;
                            }
                            else
                            {
                                bVerticalFlag_Array[nRXIndex] = false;
                            }
                        }
                    }

                    m_cDataInfo_List[nDataIndex].m_bTraceHorizontalFlag_Array = bHorizontalFlag_Array;
                    m_cDataInfo_List[nDataIndex].m_bTraceVerticalFlag_Array = bVerticalFlag_Array;
                }

                bool bGetSignalTraceFlag = false;
                bool bGetLCMNoiseTraceFlag = false;

                for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                {
                    if (bVerticalFlag_Array[nTraceIndex] == true)
                        bGetSignalTraceFlag = true;
                    else
                        bGetLCMNoiseTraceFlag = true;
                }

                if (bGetSignalTraceFlag == false)
                {
                    m_sErrorMessage = string.Format("Signal Area Not Found in Frequency={0:0.000}KHz", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000"));
                    return false;
                }
                else if (bGetLCMNoiseTraceFlag == false)
                {
                    m_sErrorMessage = string.Format("LCM Noise Area Not Found in Frequency={0:0.000}KHz", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000"));
                    return false;
                }

                if (m_bUseTotalDataGetSignalTrace == false)
                {
                    bool[,] bFrameVerticalFlag_Array = new bool[nDVFrameData_Array.GetLength(0), m_nRXTraceNumber];
                    bool[,] bFrameHorizontalFlag_Array = new bool[nDVFrameData_Array.GetLength(0), m_nTXTraceNumber];
                    Array.Clear(bFrameVerticalFlag_Array, 0, bFrameVerticalFlag_Array.Length);
                    Array.Clear(bFrameHorizontalFlag_Array, 0, bFrameHorizontalFlag_Array.Length);

                    for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                    {
                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                        {
                            if (bHorizontalFlag_Array[nTXIndex] == true)
                                bFrameHorizontalFlag_Array[nFrameIndex, nTXIndex] = true;
                        }

                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                        {
                            if (bVerticalFlag_Array[nRXIndex] == true)
                                bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] = true;
                        }
                    }

                    //輸出Signal區域之Frame Data，Signal區域填入其DV值，非Signal區域填入0
                    for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                    {
                        int[,] nFrameData_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                        for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                        {
                            for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                            {
                                if (bFrameHorizontalFlag_Array[nFrameIndex, nTXIndex] == true &&
                                    bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                                {
                                    nFrameData_Array[nTXIndex, nRXIndex] = nDVFrameData_Array[nFrameIndex, nTXIndex, nRXIndex];
                                }
                                else
                                    nFrameData_Array[nTXIndex, nRXIndex] = 0;
                            }
                        }

                        nSignalFrameData_List.Add(nFrameData_Array);
                    }

                    m_cDataInfo_List[nDataIndex].m_bFrameHorizontalFlag_Array = bFrameHorizontalFlag_Array;
                    m_cDataInfo_List[nDataIndex].m_bFrameVerticalFlag_Array = bFrameVerticalFlag_Array;

                    for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                    {
                        for (int nFlagIndex = 0; nFlagIndex < bFrameHorizontalFlag_Array.GetLength(1); nFlagIndex++)
                        {
                            if (bFrameHorizontalFlag_Array[nFrameIndex, nFlagIndex] == true)
                            {
                                nTXSignalAreaFrameCount++;
                                break;
                            }
                        }

                        for (int nFlagIndex = 0; nFlagIndex < bFrameVerticalFlag_Array.GetLength(1); nFlagIndex++)
                        {
                            if (bFrameVerticalFlag_Array[nFrameIndex, nFlagIndex] == true)
                            {
                                nRXSignalAreaFrameCount++;
                                break;
                            }
                        }
                    }
                }

                if (ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 2)
                {
                    if (m_eUseBaseDifferEdgeDetectMethod == UseBaseDifferEdgeDetectMethod.UseConvolutionG)
                    {
                        SaveFrameData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "BASEDiffer", nBASEDiffer_Array, m_nTXTraceNumber, m_nRXTraceNumber);

                        SaveFrameData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "DifferOffset", nBASEDifferOffset_Array, m_nTXTraceNumber, m_nRXTraceNumber);

                        SaveFrameData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvGx", nHorizontalGxFrameData_Array, m_nTXTraceNumber, m_nRXTraceNumber);

                        SaveFrameData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvGy", nVerticalGyFrameData_Array, m_nTXTraceNumber, m_nRXTraceNumber);

                        SaveFrameAndHorVerData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvG", nGFrameData_Array, m_nTXTraceNumber, m_nRXTraceNumber, cConvGStdReference_Horizontal, cConvGStdReference_Vertical);
                    }
                    else if (m_eUseBaseDifferEdgeDetectMethod == UseBaseDifferEdgeDetectMethod.UseConvolutionGxGy)
                    {
                        SaveFrameData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "BASEDiffer", nBASEDiffer_Array, m_nTXTraceNumber, m_nRXTraceNumber);

                        SaveFrameData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "DifferOffset", nBASEDifferOffset_Array, m_nTXTraceNumber, m_nRXTraceNumber);

                        SaveFrameAndHorizontalData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvGx", nHorizontalGxFrameData_Array, m_nTXTraceNumber, m_nRXTraceNumber, cConvGStdReference_Horizontal);

                        SaveFrameAndVerticalData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "ConvGy", nVerticalGyFrameData_Array, m_nTXTraceNumber, m_nRXTraceNumber, cConvGStdReference_Vertical);
                    }
                }
            }
            #endregion
            #region By User Define
            else if (m_eGetSignalAreaMethod == GetSignalAreaMethod.ByUserDefine)
            {
                bool[,] bFrameVerticalFlag_Array = new bool[nDVFrameData_Array.GetLength(0), m_nRXTraceNumber];
                bool[,] bFrameHorizontalFlag_Array = new bool[nDVFrameData_Array.GetLength(0), m_nTXTraceNumber];
                Array.Clear(bFrameVerticalFlag_Array, 0, bFrameVerticalFlag_Array.Length);
                Array.Clear(bFrameHorizontalFlag_Array, 0, bFrameHorizontalFlag_Array.Length);

                for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (nTXIndex + 1 >= m_nSignalTXStartTrace && nTXIndex + 1 <= m_nSignalTXEndTrace)
                            bFrameHorizontalFlag_Array[nFrameIndex, nTXIndex] = true;
                    }

                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex + 1 >= m_nSignalRXStartTrace && nRXIndex + 1 <= m_nSignalRXEndTrace)
                            bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] = true;
                    }
                }

                //輸出Signal區域之Frame Data，Signal區域填入其DV值，非Signal區域填入0
                for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                {
                    int[,] nSingleFrameData_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                        {
                            if (bFrameHorizontalFlag_Array[nFrameIndex, nTXIndex] == true &&
                                bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                            {
                                nSingleFrameData_Array[nTXIndex, nRXIndex] = nDVFrameData_Array[nFrameIndex, nTXIndex, nRXIndex];
                            }
                            else
                                nSingleFrameData_Array[nTXIndex, nRXIndex] = 0;
                        }
                    }

                    nSignalFrameData_List.Add(nSingleFrameData_Array);
                }

                m_cDataInfo_List[nDataIndex].m_bFrameHorizontalFlag_Array = bFrameHorizontalFlag_Array;
                m_cDataInfo_List[nDataIndex].m_bFrameVerticalFlag_Array = bFrameVerticalFlag_Array;

                for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                {
                    for (int nFlagIndex = 0; nFlagIndex < bFrameHorizontalFlag_Array.GetLength(1); nFlagIndex++)
                    {
                        if (bFrameHorizontalFlag_Array[nFrameIndex, nFlagIndex] == true)
                        {
                            nTXSignalAreaFrameCount++;
                            break;
                        }
                    }

                    for (int nFlagIndex = 0; nFlagIndex < bFrameVerticalFlag_Array.GetLength(1); nFlagIndex++)
                    {
                        if (bFrameVerticalFlag_Array[nFrameIndex, nFlagIndex] == true)
                        {
                            nRXSignalAreaFrameCount++;
                            break;
                        }
                    }
                }
            }
            #endregion

            if (!(m_eGetSignalAreaMethod == GetSignalAreaMethod.UseBaseDifferEdgeDetect && m_bUseTotalDataGetSignalTrace == true))
            {
                if (ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 1 || ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 2)
                {
                    if (SaveParticularData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "SignalArea", nSignalFrameData_List, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                        return false;
                }

                //計算TX方向與RX方向有Signal區域之Frame數比例值，若比例值低於fSignalAreaRatio則輸出Error
                double dTXFrameRatio = Math.Round((double)nTXSignalAreaFrameCount / nDVFrameData_Array.GetLength(0), 2, MidpointRounding.AwayFromZero);
                double dRXFrameRatio = Math.Round((double)nRXSignalAreaFrameCount / nDVFrameData_Array.GetLength(0), 2, MidpointRounding.AwayFromZero);

                if (dTXFrameRatio < dSignalAreaRatio_Vertical || dRXFrameRatio < dSignalAreaRatio_Horizontal)
                {
                    m_sErrorMessage = string.Format("Signal Area Frame Not Enough in Frequency={0:0.000}KHz(TX(Ver):{1} RX(Hor):{2})",
                                                    m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000"),
                                                    nTXSignalAreaFrameCount,
                                                    nRXSignalAreaFrameCount);
                    return false;
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
            #endregion

            #region Method 1:Noise Positive Reference Value Threshold
            /*
            int nPosRefFactor = 1;

            for (int nIndex = 0; nIndex < m_InfoDataList.Count; nIndex++)
            {
                List<int[,]> SignalFrameData = new List<int[,]>();
                double fHBThreshold = m_InfoDataList[nIndex].m_PreReportInfo.m_fNPosRef * nPosRefFactor;
                List<int[,]> DVFrameData = m_InfoDataList[nIndex].m_OBASEMinusADCList;

                for (int nFrameIdx = 0; nFrameIdx < DVFrameData.Count; nFrameIdx++)
                {
                    int[,] SFrameData = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                    for (int nY = 0; nY < m_nTXTraceNumber; nY++)
                    {
                        for (int nX = 0; nX < m_nRXTraceNumber; nX++)
                        {
                            if (DVFrameData[nFrameIdx][nY, nX] > fHBThreshold)
                            {
                                int nPreY = (nY - 1 < 0) ? 0 : nY - 1;
                                int nPreX = (nX - 1 < 0) ? 0 : nX - 1;
                                int nNextY = (nY + 1 > m_nTXTraceNumber - 1) ? m_nTXTraceNumber - 1 : nY + 1;
                                int nNextX = (nX + 1 > m_nRXTraceNumber - 1) ? m_nRXTraceNumber - 1 : nX + 1;

                                int nNBOverHBCount = 0;

                                for (int nNBYIdx = nPreY; nNBYIdx <= nNextY; nNBYIdx++)
                                {
                                    for (int nNBXIdx = nPreX; nNBXIdx <= nNextX; nNBXIdx++)
                                    {
                                        if (nNBXIdx == nY || nNBXIdx == nX)
                                            continue;

                                        if (DVFrameData[nFrameIdx][nNBYIdx, nNBXIdx] > fHBThreshold)
                                            nNBOverHBCount++;
                                    }
                                }

                                if (nNBOverHBCount > 0)
                                {
                                    if (nFrameIdx == 0)
                                    {
                                        if (DVFrameData[nFrameIdx + 1][nY, nX] > fHBThreshold)
                                            SFrameData[nY, nX] = DVFrameData[nFrameIdx][nY, nX];
                                        else
                                            SFrameData[nY, nX] = 0;
                                    }
                                    else if (nFrameIdx == DVFrameData.Count - 1)
                                    {
                                        if (DVFrameData[nFrameIdx - 1][nY, nX] > fHBThreshold)
                                            SFrameData[nY, nX] = DVFrameData[nFrameIdx][nY, nX];
                                        else
                                            SFrameData[nY, nX] = 0;
                                    }
                                    else
                                    {
                                        if (DVFrameData[nFrameIdx - 1][nY, nX] > fHBThreshold)
                                            SFrameData[nY, nX] = DVFrameData[nFrameIdx][nY, nX];
                                        else if (DVFrameData[nFrameIdx + 1][nY, nX] > fHBThreshold)
                                            SFrameData[nY, nX] = DVFrameData[nFrameIdx][nY, nX];
                                        else
                                            SFrameData[nY, nX] = 0;
                                    }
                                }
                                else
                                    SFrameData[nY, nX] = 0;
                            }
                        }
                    }

                    SignalFrameData.Add(SFrameData);
                }

                if (xSaveParticularData(nIndex, "Signal", SignalFrameData, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                    return false;
            }
            */
            #endregion

            return true;
        }

        /// <summary>
        /// Save AnalysisInfo.csv File and Use Total Data to Get Signal Trace Area
        /// </summary>
        /// <returns></returns>
        private bool ReGetSignalAreaByTotalData()
        {
            if (m_eGetSignalAreaMethod != GetSignalAreaMethod.UseBaseDifferEdgeDetect)
                return true;

            if (ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 2)
            {
                SaveAnalysisInfoFile_SingleData(MainConstantParameter.m_sDATATYPE_ANALYSIS, "AnalysisInfo");
            }

            if ((m_eUseBaseDifferEdgeDetectMethod == UseBaseDifferEdgeDetectMethod.UseConvolutionG || m_eUseBaseDifferEdgeDetectMethod == UseBaseDifferEdgeDetectMethod.UseConvolutionGxGy) &&
                m_bUseTotalDataGetSignalTrace == true)
            {
                bool[] bHorizontalFlag_Array = new bool[m_nTXTraceNumber];
                bool[] bVerticalFlag_Array = new bool[m_nRXTraceNumber];
                Array.Clear(bHorizontalFlag_Array, 0, bHorizontalFlag_Array.Length);
                Array.Clear(bVerticalFlag_Array, 0, bVerticalFlag_Array.Length);

                for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                {
                    int nSignalTraceCount = 0;

                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_bTraceHorizontalFlag_Array[nTXIndex] == true)
                            nSignalTraceCount++;
                    }

                    if (nSignalTraceCount > m_cDataInfo_List.Count / 2)
                        bHorizontalFlag_Array[nTXIndex] = true;
                    else
                        bHorizontalFlag_Array[nTXIndex] = false;
                }

                for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                {
                    int nSignalTraceCount = 0;

                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_bTraceVerticalFlag_Array[nRXIndex] == true)
                            nSignalTraceCount++;
                    }

                    if (nSignalTraceCount > m_cDataInfo_List.Count / 2)
                        bVerticalFlag_Array[nRXIndex] = true;
                    else
                        bVerticalFlag_Array[nRXIndex] = false;
                }

                m_bTotalTraceHorizontalFlag_Array = new bool[m_nTXTraceNumber];
                m_bTotalTraceVerticalFlag_Array = new bool[m_nRXTraceNumber];
                m_bTotalTraceHorizontalFlag_Array = bHorizontalFlag_Array;
                m_bTotalTraceVerticalFlag_Array = bVerticalFlag_Array;

                if (ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 2)
                {
                    SaveAnalysisInfoFile_TotalData(MainConstantParameter.m_sDATATYPE_ANALYSIS, "AnalysisInfo");
                }

                bool bGetSignalTraceFlag = false;
                bool bGetLCMNoiseTraceFlag = false;

                for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                {
                    if (bVerticalFlag_Array[nTraceIndex] == true)
                        bGetSignalTraceFlag = true;
                    else
                        bGetLCMNoiseTraceFlag = true;
                }

                if (bGetSignalTraceFlag == false)
                {
                    m_sErrorMessage = string.Format("Signal Area Not Found");
                    return false;
                }
                else if (bGetLCMNoiseTraceFlag == false)
                {
                    m_sErrorMessage = string.Format("LCM Noise Area Not Found");
                    return false;
                }

                if (ReadADCDataByTotalData(bHorizontalFlag_Array, bVerticalFlag_Array) == false)
                    return false;
            }

            return true;
        }

        private bool ReadADCDataByTotalData(bool[] bHorizontalFlag_Array, bool[] bVerticalFlag_Array)
        {
            double dSignalAreaRatio_Horizontal = Math.Round(m_dSignalAreaPercentage_Horizontal / 100.0, 4, MidpointRounding.AwayFromZero);
            double dSignalAreaRatio_Vertical = Math.Round(m_dSignalAreaPercentage_Vertical / 100.0, 4, MidpointRounding.AwayFromZero);

            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_ADC);

            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, m_cDataInfo_List[nDataIndex].m_sADCFileName);
                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);
                List<int[,]> nFrameData_List = new List<int[,]>();

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                if (GetFrameData(ref nFrameData_List, srFile, sFileName, ReadDataType.ADC) == false)
                    return false;

                int[,,] nBASEMinusADC_Array = new int[nFrameData_List.Count, m_nTXTraceNumber, m_nRXTraceNumber];
                int[,,] nOBASEMinusADC_Array = new int[nFrameData_List.Count, m_nTXTraceNumber, m_nRXTraceNumber];

                if (ComputeDVData(ref nBASEMinusADC_Array, nDataIndex, nFrameData_List, m_nRXTraceNumber, m_nTXTraceNumber, MainConstantParameter.m_sDATATYPE_BASEMinusADC) == false)
                    return false;

                if (ComputeDVData(ref nOBASEMinusADC_Array, nDataIndex, nFrameData_List, m_nRXTraceNumber, m_nTXTraceNumber, MainConstantParameter.m_sDATATYPE_OBASEMinusADC) == false)
                    return false;

                List<int[,]> nFrameRXDifferData_List = new List<int[,]>();
                List<int[,]> nSignalFrameData_List = new List<int[,]>();
                int[,,] nDVFrameData_Array = nOBASEMinusADC_Array;

                //計算有TX方向Signal區域與有RX方向Signal區域之各自的Frame數
                int nTXSignalAreaFrameCount = 0;
                int nRXSignalAreaFrameCount = 0;

                bool[,] bFrameVerticalFlag_Array = new bool[nDVFrameData_Array.GetLength(0), m_nRXTraceNumber];
                bool[,] bFrameHorizontalFlag_Array = new bool[nDVFrameData_Array.GetLength(0), m_nTXTraceNumber];
                Array.Clear(bFrameVerticalFlag_Array, 0, bFrameVerticalFlag_Array.Length);
                Array.Clear(bFrameHorizontalFlag_Array, 0, bFrameHorizontalFlag_Array.Length);

                for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (bHorizontalFlag_Array[nTXIndex] == true)
                            bFrameHorizontalFlag_Array[nFrameIndex, nTXIndex] = true;
                    }

                    for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                    {
                        if (bVerticalFlag_Array[nRXIndex] == true)
                            bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] = true;
                    }
                }

                //輸出Signal區域之Frame Data，Signal區域填入其DV值，非Signal區域填入0
                for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                {
                    int[,] nFrameData_Array = new int[m_nTXTraceNumber, m_nRXTraceNumber];

                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                        {
                            if (bFrameHorizontalFlag_Array[nFrameIndex, nTXIndex] == true &&
                                bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                            {
                                nFrameData_Array[nTXIndex, nRXIndex] = nDVFrameData_Array[nFrameIndex, nTXIndex, nRXIndex];
                            }
                            else
                                nFrameData_Array[nTXIndex, nRXIndex] = 0;
                        }
                    }

                    nSignalFrameData_List.Add(nFrameData_Array);
                }

                m_cDataInfo_List[nDataIndex].m_bFrameHorizontalFlag_Array = bFrameHorizontalFlag_Array;
                m_cDataInfo_List[nDataIndex].m_bFrameVerticalFlag_Array = bFrameVerticalFlag_Array;

                for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
                {
                    for (int nFlagIndex = 0; nFlagIndex < bFrameHorizontalFlag_Array.GetLength(1); nFlagIndex++)
                    {
                        if (bFrameHorizontalFlag_Array[nFrameIndex, nFlagIndex] == true)
                        {
                            nTXSignalAreaFrameCount++;
                            break;
                        }
                    }

                    for (int nFlagIndex = 0; nFlagIndex < bFrameVerticalFlag_Array.GetLength(1); nFlagIndex++)
                    {
                        if (bFrameVerticalFlag_Array[nFrameIndex, nFlagIndex] == true)
                        {
                            nRXSignalAreaFrameCount++;
                            break;
                        }
                    }
                }

                if (ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 1 || ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 2)
                {
                    if (SaveParticularData(nDataIndex, MainConstantParameter.m_sDATATYPE_ANALYSIS, "SignalArea", nSignalFrameData_List, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                        return false;
                }

                //計算TX方向與RX方向有Signal區域之Frame數比例值，若比例值低於fSignalAreaRatio則輸出Error
                double dTXFrameRatio = Math.Round((double)nTXSignalAreaFrameCount / nDVFrameData_Array.GetLength(0), 2, MidpointRounding.AwayFromZero);
                double dRXFrameRatio = Math.Round((double)nRXSignalAreaFrameCount / nDVFrameData_Array.GetLength(0), 2, MidpointRounding.AwayFromZero);

                if (dTXFrameRatio < dSignalAreaRatio_Vertical || dRXFrameRatio < dSignalAreaRatio_Horizontal)
                {
                    m_sErrorMessage = string.Format("Signal Area Frame Not Enough in Frequency={0:0.000}KHz(TX(Ver):{1} RX(Hor):{2})",
                                                    m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000"),
                                                    nTXSignalAreaFrameCount,
                                                    nRXSignalAreaFrameCount);
                    return false;
                }

                if (ComputeReferenceData_TotalData(nDataIndex, nBASEMinusADC_Array, nOBASEMinusADC_Array) == false)
                    return false;
            }

            return true;
        }

        private bool SaveFrameData(int nDataIndex, string sDataType, string sFileType, int[,] nFrameData_Array, int nTXTraceNumber, int nRXTraceNumber)
        {
            bool bErrorFlag = false;
            int nFrameNumber = 1;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                            sw.WriteLine(nRXIndex + 1);
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                                sw.Write(string.Format("{0},", nFrameData_Array[nTXIndex, nRXIndex]));
                            else
                                sw.WriteLine(nFrameData_Array[nTXIndex, nRXIndex]);
                        }
                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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

        private bool SaveFrameAndHorVerData(int nDataIndex, string sDataType, string sFileType, uint[,] nFrameData_Array, int nTXTraceNumber, int nRXTraceNumber,
                                            ConvGStdReference cConvGStdReference_Horizontal = null, ConvGStdReference cConvGStdReference_Vertical = null,
                                            List<KMeansMethod.Cluster> cHorizontalCluster_List = null, List<KMeansMethod.Cluster> cVerticalCluster_List = null)
        {
            bool bErrorFlag = false;
            int nFrameNumber = 1;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                            sw.Write(nRXIndex + 1);
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine(",,OriginalStd,NearMeanStd,,Threshold,Std,Mean,Min,Median,MeanUnderMedian");

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                                sw.Write(string.Format("{0},", nFrameData_Array[nTXIndex, nRXIndex]));
                            else
                                sw.Write(nFrameData_Array[nTXIndex, nRXIndex]);
                        }

                        if (nTXIndex == 0)
                        {
                            sw.WriteLine(string.Format(",,{0},{1},,{2},{3},{4},{5},{6},{7}",
                                                       cConvGStdReference_Horizontal.m_dStd_Original_Array[nTXIndex],
                                                       cConvGStdReference_Horizontal.m_dStd_NearMean_Array[nTXIndex],
                                                       cConvGStdReference_Horizontal.m_dThreshold,
                                                       cConvGStdReference_Horizontal.m_dStd,
                                                       cConvGStdReference_Horizontal.m_dMean,
                                                       cConvGStdReference_Horizontal.m_dMin,
                                                       cConvGStdReference_Horizontal.m_dMedian,
                                                       cConvGStdReference_Horizontal.m_dMeanUnderMedian));
                        }
                        else
                        {
                            sw.WriteLine(string.Format(",,{0},{1}", 
                                                       cConvGStdReference_Horizontal.m_dStd_Original_Array[nTXIndex],
                                                       cConvGStdReference_Horizontal.m_dStd_NearMean_Array[nTXIndex]));
                        }
                    }


                    sw.WriteLine();

                    sw.Write("OriginalStd,,");

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex < nRXTraceNumber - 1)
                            sw.Write(string.Format("{0},", cConvGStdReference_Vertical.m_dStd_Original_Array[nRXIndex]));
                        else
                            sw.WriteLine(cConvGStdReference_Vertical.m_dStd_Original_Array[nRXIndex]);
                    }

                    sw.Write("NearMeanStd,,");

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex < nRXTraceNumber - 1)
                            sw.Write(string.Format("{0},", cConvGStdReference_Vertical.m_dStd_NearMean_Array[nRXIndex]));
                        else
                            sw.WriteLine(cConvGStdReference_Vertical.m_dStd_NearMean_Array[nRXIndex]);
                    }

                    sw.WriteLine();

                    sw.WriteLine("Threshold,,{0}", cConvGStdReference_Vertical.m_dThreshold);
                    sw.WriteLine("Std,,{0}", cConvGStdReference_Vertical.m_dStd);
                    sw.WriteLine("Mean,,{0}", cConvGStdReference_Vertical.m_dMean);
                    sw.WriteLine("Min,,{0}", cConvGStdReference_Vertical.m_dMin);
                    sw.WriteLine("Median,,{0}", cConvGStdReference_Vertical.m_dMedian);
                    sw.WriteLine("MeanUnderMedian,,{0}", cConvGStdReference_Vertical.m_dMeanUnderMedian);

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();

                    if (cHorizontalCluster_List != null && cVerticalCluster_List != null)
                    {
                        sw.WriteLine("Horizontal Cluster");

                        for (int nClusterIndex = 0; nClusterIndex < cHorizontalCluster_List.Count; nClusterIndex++)
                        {
                            for (int nValueIndex = 0; nValueIndex < cHorizontalCluster_List[nClusterIndex].dDataPoints_List.Count; nValueIndex++)
                            {
                                sw.Write("{0},", cHorizontalCluster_List[nClusterIndex].dDataPoints_List[nValueIndex]);
                            }

                            sw.WriteLine();
                        }

                        sw.WriteLine();

                        sw.WriteLine("Vertical Cluster");

                        for (int nClusterIndex = 0; nClusterIndex < cVerticalCluster_List.Count; nClusterIndex++)
                        {
                            for (int nValueIndex = 0; nValueIndex < cVerticalCluster_List[nClusterIndex].dDataPoints_List.Count; nValueIndex++)
                            {
                                sw.Write("{0},", cVerticalCluster_List[nClusterIndex].dDataPoints_List[nValueIndex]);
                            }

                            sw.WriteLine();
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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

        private bool SaveFrameAndHorizontalData(int nDataIndex, string sDataType, string sFileType, int[,] nFrameData_Array, int nTXTraceNumber, int nRXTraceNumber, ConvGStdReference cConvGStdReference_Horizontal = null)
        {
            bool bErrorFlag = false;
            int nFrameNumber = 1;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                            sw.Write(nRXIndex + 1);
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine(",,OriginalStd,NearMeanStd,,Threshold,Std,Min");

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                                sw.Write(string.Format("{0},", nFrameData_Array[nTXIndex, nRXIndex]));
                            else
                                sw.Write(nFrameData_Array[nTXIndex, nRXIndex]);
                        }

                        if (nTXIndex == 0)
                        {
                            sw.WriteLine(string.Format(",,{0},{1},,{2},{3},{4}",
                                                       cConvGStdReference_Horizontal.m_dStd_Original_Array[nTXIndex],
                                                       cConvGStdReference_Horizontal.m_dStd_NearMean_Array[nTXIndex],
                                                       cConvGStdReference_Horizontal.m_dThreshold,
                                                       cConvGStdReference_Horizontal.m_dStd,
                                                       cConvGStdReference_Horizontal.m_dMin));
                        }
                        else
                        {
                            sw.WriteLine(string.Format(",,{0},{1}", 
                                                       cConvGStdReference_Horizontal.m_dStd_Original_Array[nTXIndex],
                                                       cConvGStdReference_Horizontal.m_dStd_NearMean_Array[nTXIndex]));
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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

        private bool SaveFrameAndVerticalData(int nDataIndex, string sDataType, string sFileType, int[,] nFrameData_Array, int nTXTraceNumber, int nRXTraceNumber, ConvGStdReference cConvGStdReference_Vertical = null)
        {
            bool bErrorFlag = false;
            int nFrameNumber = 1;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                            sw.Write(nRXIndex + 1);
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine();

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                                sw.Write(string.Format("{0},", nFrameData_Array[nTXIndex, nRXIndex]));
                            else
                                sw.Write(nFrameData_Array[nTXIndex, nRXIndex]);
                        }

                        sw.WriteLine();
                    }


                    sw.WriteLine();

                    sw.Write("OriginalStd,,");

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex < nRXTraceNumber - 1)
                            sw.Write(string.Format("{0},", cConvGStdReference_Vertical.m_dStd_Original_Array[nRXIndex]));
                        else
                            sw.WriteLine(cConvGStdReference_Vertical.m_dStd_Original_Array[nRXIndex]);
                    }

                    sw.Write("NearMeanStd,,");

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex < nRXTraceNumber - 1)
                            sw.Write(string.Format("{0},", cConvGStdReference_Vertical.m_dStd_NearMean_Array[nRXIndex]));
                        else
                            sw.WriteLine(cConvGStdReference_Vertical.m_dStd_NearMean_Array[nRXIndex]);
                    }

                    sw.WriteLine();

                    sw.WriteLine("Threshold,,{0}", cConvGStdReference_Vertical.m_dThreshold);
                    sw.WriteLine("Std,,{0}", cConvGStdReference_Vertical.m_dStd);
                    sw.WriteLine("Min,,{0}", cConvGStdReference_Vertical.m_dMin);

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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

        private bool SaveAnalysisInfoFile_SingleData(string sDataType, string sFileType)
        {
            bool bErrorFlag = false;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            string sFileName = string.Format("{0}", sFileType);
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("TX Signal Trace");

                sw.WriteLine("Index,PH1,PH2,Frequency,Trace");

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
                    int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
                    double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

                    sw.Write(string.Format("{0},{1},{2},{3}", (nDataIndex + 1), nPH1, nPH2, dFrequency));

                    for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_bTraceHorizontalFlag_Array[nTraceIndex] == true)
                        {
                            sw.Write(",{0}", (nTraceIndex + 1));
                        }
                    }

                    sw.WriteLine();
                }

                sw.WriteLine();

                sw.WriteLine("RX Signal Trace");

                sw.WriteLine("Index,PH1,PH2,Frequency,Trace");

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
                    int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
                    double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

                    sw.Write(string.Format("{0},{1},{2},{3}", (nDataIndex + 1), nPH1, nPH2, dFrequency));

                    for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_bTraceVerticalFlag_Array[nTraceIndex] == true)
                        {
                            sw.Write(",{0}", (nTraceIndex + 1));
                        }
                    }

                    sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error", sFileName);
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

        private bool SaveAnalysisInfoFile_TotalData(string sDataType, string sFileType)
        {
            bool bErrorFlag = false;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            string sFileName = string.Format("{0}", sFileType);
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            if (File.Exists(sFilePath) == false)
            {
                m_sErrorMessage = string.Format("{0}.csv File Not Exist", sFileName);
                bErrorFlag = true;
                return bErrorFlag;
            }

            FileStream fs = new FileStream(sFilePath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine();

                sw.Write("TX Total Signal Trace");

                for (int nTraceIndex = 0; nTraceIndex < m_nTXTraceNumber; nTraceIndex++)
                {
                    if (m_bTotalTraceHorizontalFlag_Array[nTraceIndex] == true)
                    {
                        sw.Write(",{0}", (nTraceIndex + 1));
                    }
                }

                sw.WriteLine();

                sw.WriteLine();

                sw.Write("RX Total Signal Trace");

                for (int nTraceIndex = 0; nTraceIndex < m_nRXTraceNumber; nTraceIndex++)
                {
                    if (m_bTotalTraceVerticalFlag_Array[nTraceIndex] == true)
                    {
                        sw.Write(",{0}", (nTraceIndex + 1));
                    }
                }

                sw.WriteLine();
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error", sFileName);
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

        private bool InitializeDetermineSignalAreaFlag(double[,] dStd_Array, double[] dStdThreshold_Array, bool bSignalFlag, int nFrameIndex, int nTraceIndex, int nTraceNumber, int nFrameCount)
        {
            if (dStd_Array[nFrameIndex, nTraceIndex] > dStdThreshold_Array[nFrameIndex])
            {
                if (nTraceIndex == 0)
                {
                    if (dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex] &&
                        dStd_Array[nFrameIndex, nTraceIndex + 2] > dStdThreshold_Array[nFrameIndex])
                    {
                        if (nFrameIndex == 0)
                        {
                            if (dStd_Array[nFrameIndex + 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex + 1])
                                bSignalFlag = true;
                        }
                        else if (nFrameIndex == nFrameCount - 1)
                        {
                            if (dStd_Array[nFrameIndex - 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex - 1])
                                bSignalFlag = true;
                        }
                        else
                        {
                            if (dStd_Array[nFrameIndex + 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex + 1] ||
                                dStd_Array[nFrameIndex - 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex - 1])
                                bSignalFlag = true;
                        }
                    }
                }
                else if (nTraceIndex == 1)
                {
                    if ((dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex]) ||
                        (dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex + 2] > dStdThreshold_Array[nFrameIndex]))
                    {
                        if (nFrameIndex == 0)
                        {
                            if (dStd_Array[nFrameIndex + 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex + 1])
                                bSignalFlag = true;
                        }
                        else if (nFrameIndex == nFrameCount - 1)
                        {
                            if (dStd_Array[nFrameIndex - 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex - 1])
                                bSignalFlag = true;
                        }
                        else
                        {
                            if (dStd_Array[nFrameIndex + 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex + 1] ||
                                dStd_Array[nFrameIndex - 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex - 1])
                                bSignalFlag = true;
                        }
                    }
                }
                else if (nTraceIndex == nTraceNumber - 1)
                {
                    if (dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex] &&
                        dStd_Array[nFrameIndex, nTraceIndex - 2] > dStdThreshold_Array[nFrameIndex])
                    {
                        if (nFrameIndex == 0)
                        {
                            if (dStd_Array[nFrameIndex + 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex + 1])
                                bSignalFlag = true;
                        }
                        else if (nFrameIndex == nFrameCount - 1)
                        {
                            if (dStd_Array[nFrameIndex - 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex - 1])
                                bSignalFlag = true;
                        }
                        else
                        {
                            if (dStd_Array[nFrameIndex + 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex + 1] ||
                                dStd_Array[nFrameIndex - 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex - 1])
                                bSignalFlag = true;
                        }
                    }
                }
                else if (nTraceIndex == nTraceNumber - 2)
                {
                    if ((dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex]) ||
                        (dStd_Array[nFrameIndex, nTraceIndex - 2] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex]))
                    {
                        if (nFrameIndex == 0)
                        {
                            if (dStd_Array[nFrameIndex + 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex + 1])
                                bSignalFlag = true;
                        }
                        else if (nFrameIndex == nFrameCount - 1)
                        {
                            if (dStd_Array[nFrameIndex - 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex - 1])
                                bSignalFlag = true;
                        }
                        else
                        {
                            if (dStd_Array[nFrameIndex + 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex + 1] ||
                                dStd_Array[nFrameIndex - 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex - 1])
                                bSignalFlag = true;
                        }
                    }
                }
                else
                {
                    if ((dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex]) ||
                        (dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex - 2] > dStdThreshold_Array[nFrameIndex]) ||
                        (dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex + 2] > dStdThreshold_Array[nFrameIndex]))
                    {
                        if (nFrameIndex == 0)
                        {
                            if (dStd_Array[nFrameIndex + 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex + 1])
                                bSignalFlag = true;
                        }
                        else if (nFrameIndex == nFrameCount - 1)
                        {
                            if (dStd_Array[nFrameIndex - 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex - 1])
                                bSignalFlag = true;
                        }
                        else
                        {
                            if (dStd_Array[nFrameIndex + 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex + 1] ||
                                dStd_Array[nFrameIndex - 1, nTraceIndex] > dStdThreshold_Array[nFrameIndex - 1])
                                bSignalFlag = true;
                        }
                    }
                }
            }

            return bSignalFlag;
        }

        private bool InitializeDetermineSignalAreaFlag_NoDebounce(double[,] dStd_Array, double[] dStdThreshold_Array, bool bSignalFlag, int nFrameIndex, int nTraceIndex, int nTraceNumber, int nFrameCount)
        {
            if (dStd_Array[nFrameIndex, nTraceIndex] > dStdThreshold_Array[nFrameIndex])
            {
                if (nTraceIndex == 0)
                {
                    if (dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex] &&
                        dStd_Array[nFrameIndex, nTraceIndex + 2] > dStdThreshold_Array[nFrameIndex])
                        bSignalFlag = true;
                }
                else if (nTraceIndex == 1)
                {
                    if ((dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex]) ||
                        (dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex + 2] > dStdThreshold_Array[nFrameIndex]))
                        bSignalFlag = true;
                }
                else if (nTraceIndex == nTraceNumber - 1)
                {
                    if (dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex] &&
                        dStd_Array[nFrameIndex, nTraceIndex - 2] > dStdThreshold_Array[nFrameIndex])
                        bSignalFlag = true;
                }
                else if (nTraceIndex == nTraceNumber - 2)
                {
                    if ((dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex]) ||
                        (dStd_Array[nFrameIndex, nTraceIndex - 2] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex]))
                        bSignalFlag = true;
                }
                else
                {
                    if ((dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex]) ||
                        (dStd_Array[nFrameIndex, nTraceIndex - 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex - 2] > dStdThreshold_Array[nFrameIndex]) ||
                        (dStd_Array[nFrameIndex, nTraceIndex + 1] > dStdThreshold_Array[nFrameIndex] && dStd_Array[nFrameIndex, nTraceIndex + 2] > dStdThreshold_Array[nFrameIndex]))
                        bSignalFlag = true;
                }
            }

            return bSignalFlag;
        }

        private void ExcludeAllNegativeValueTrace(ref bool[,] bHorizontal_Array, bool[,] bVertical_Array, List<int[,]> nDVData_List, int nFrameIndex, int nTraceIndex, bool bTurnFalse = true)
        {
            if (bHorizontal_Array[nFrameIndex, nTraceIndex] == true)
            {
                bool bAllNegativeFlag = true;

                for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                {
                    if (bVertical_Array[nFrameIndex, nRXIndex] == true)
                    {
                        if (nDVData_List[nFrameIndex][nTraceIndex, nRXIndex] > 0)
                        {
                            bAllNegativeFlag = false;
                            break;
                        }
                    }
                }

                if (bTurnFalse == true)
                {
                    if (bAllNegativeFlag == true)
                        bHorizontal_Array[nFrameIndex, nTraceIndex] = false;
                }
                else
                {
                    if (bAllNegativeFlag == false)
                        bHorizontal_Array[nFrameIndex, nTraceIndex] = true;
                }
            }
        }

        private bool ComputeACNoiseAreaAndReferenceValue(int nDataIndex, int[,,] nBASEMinusADC_Array)
        {
            bool bGetValueFlag = false;

            List<long> lACNoiseValue_List = new List<long>();
            int[,,] nDVFrameData_Array = nBASEMinusADC_Array;
            bool[,,] bACNoiseData_Array = new bool[nDVFrameData_Array.GetLength(0), m_nTXTraceNumber, m_nRXTraceNumber];
            Array.Clear(bACNoiseData_Array, 0, bACNoiseData_Array.Length);
            bool[,] bFrameVerticalFlag_Array = m_cDataInfo_List[nDataIndex].m_bFrameVerticalFlag_Array;
            bool[,] bFrameHorizontalFlag_Array = m_cDataInfo_List[nDataIndex].m_bFrameHorizontalFlag_Array;

            for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
            {
                for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        if (bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == true && bFrameHorizontalFlag_Array[nFrameIndex, nTXIndex] == false)
                        {
                            lACNoiseValue_List.Add(nDVFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]);
                            bACNoiseData_Array[nFrameIndex, nTXIndex, nRXIndex] = true;
                            bGetValueFlag = true;
                        }
                    }
                }
            }

            if (ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 1 || ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 2)
            {
                if (SaveNoiseData(nDataIndex, "ACNoiseArea", MainConstantParameter.m_sDATATYPE_ANALYSIS, nDVFrameData_Array, bACNoiseData_Array, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                    return false;
            }

            if (bGetValueFlag == true)
            {
                double dMean = Math.Round(lACNoiseValue_List.Average(), 2, MidpointRounding.AwayFromZero);
                double dStd = Math.Round(MathMethod.ComputeStd(lACNoiseValue_List), 2, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseMean = dMean;
                m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseStd = dStd;
            }

            double dRoundPositiveReference = ComputeNoiseReference(m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseMean,
                                                                   m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseStd, true);

            double dRoundNegativeReference = ComputeNoiseReference(m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseMean,
                                                                   m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseStd, false);

            m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoisePositiveReference = dRoundPositiveReference;
            m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseNegativeReference = dRoundNegativeReference;

            string sMessage = string.Format("Analysis : {0}/{1}", m_nProgressIndex + 1, m_nAnalysisCount);

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, m_nProgressIndex + 1, sMessage);
            });

            if (m_cfrmParent.m_bExecute == false)
                return false;

            m_nProgressIndex++;

            return true;
        }

        private double ComputeNoiseReference(double dMean, double dStd, bool bPositive)
        {
            double dReference = 0.0;

            if (bPositive == true)
                dReference = dMean + 3 * dStd;
            else
                dReference = dMean - 3 * dStd;

            double dRoundReference = 0.0;

            if (m_nSNRMethodType == m_nSNRMethod_Origin)
            {
                if (bPositive == true)
                {
                    if (dReference < 0.0)
                        dReference = 0.0;
                }
                else
                {
                    if (dReference > 0.0)
                        dReference = 0.0;
                }

                dRoundReference = Math.Round(dReference, 2, MidpointRounding.AwayFromZero);

                if (dRoundReference == 0.0)
                    dRoundReference = 0.01;
            }
            else if (m_nSNRMethodType == m_nSNRMethod_Sqrt)
            {
                dRoundReference = Math.Round(dReference, 2, MidpointRounding.AwayFromZero);

                if (dRoundReference == 0.0)
                    dRoundReference = 0.01;
            }

            return dRoundReference;
        }

        private bool ComputeNoiseAreaAndReferenceValue(int nDataIndex, int[,,] nBASEMinusADC_Array)
        {
            bool bGetACValueFlag = false;
            bool bGetLCMValueFlag = false;
            List<int> nACNoiseValue_List = new List<int>();
            List<int> nLCMNoiseValue_List = new List<int>();
            int[,,] nDVFrameData_Array = nBASEMinusADC_Array;
            bool[,,] bNoiseData_Array = new bool[nDVFrameData_Array.GetLength(0), m_nTXTraceNumber, m_nRXTraceNumber];
            Array.Clear(bNoiseData_Array, 0, bNoiseData_Array.Length);
            bool[,] bFrameVerticalFlag_Array = m_cDataInfo_List[nDataIndex].m_bFrameVerticalFlag_Array;
            bool[,] bFrameHorizontalFlag_Array = m_cDataInfo_List[nDataIndex].m_bFrameHorizontalFlag_Array;

            for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
            {
                for (int nRXIndex = 0; nRXIndex < m_nRXTraceNumber; nRXIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < m_nTXTraceNumber; nTXIndex++)
                    {
                        //if (bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == true && bFrameHorizontalFlag_Array[nFrameIndex, nTXIndex] == false)
                        if (bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                        {
                            nACNoiseValue_List.Add(nDVFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]);
                            bNoiseData_Array[nFrameIndex, nTXIndex, nRXIndex] = true;
                            bGetACValueFlag = true;
                        }
                        else if (bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == false)
                        {
                            nLCMNoiseValue_List.Add(nDVFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]);
                            bNoiseData_Array[nFrameIndex, nTXIndex, nRXIndex] = true;
                            bGetLCMValueFlag = true;
                        }
                    }
                }
            }

            if (ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 1 || ParamFingerAutoTuning.m_nACFRSaveAnalysisData == 2)
            {
                /*
                if (SaveNoiseData(nDataIndex, "NoiseArea", MainConstantParameter.m_sDATATYPE_ANALYSIS, nDVFrameData_Array, bNoiseData_Array, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                    return false;
                */

                if (SaveLCMOrACNoiseData(nDataIndex, "NoiseArea_LCM", MainConstantParameter.m_sDATATYPE_ANALYSIS, nDVFrameData_Array, m_nTXTraceNumber, m_nRXTraceNumber) == false)
                    return false;

                if (SaveLCMOrACNoiseData(nDataIndex, "NoiseArea_AC", MainConstantParameter.m_sDATATYPE_ANALYSIS, nDVFrameData_Array, m_nTXTraceNumber, m_nRXTraceNumber, false) == false)
                    return false;
            }

            if (bGetACValueFlag == true)
            {
                double dMean = Math.Round(nACNoiseValue_List.Average(), 2, MidpointRounding.AwayFromZero);
                double dStd = Math.Round(MathMethod.ComputeStd(nACNoiseValue_List), 2, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseMean = dMean;
                m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseStd = dStd;
            }

            double dRoundACPositiveReference = ComputeNoiseReference(m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseMean,
                                                                     m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseStd, true);

            double dRoundACNegativeReference = ComputeNoiseReference(m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseMean,
                                                                     m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseStd, false);

            m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoisePositiveReference = dRoundACPositiveReference;
            m_cDataInfo_List[nDataIndex].m_cACNoiseReferenceData.m_dNoiseNegativeReference = dRoundACNegativeReference;

            if (bGetLCMValueFlag == true)
            {
                double dMean = Math.Round(nLCMNoiseValue_List.Average(), 2, MidpointRounding.AwayFromZero);
                double dStd = Math.Round(MathMethod.ComputeStd(nLCMNoiseValue_List), 2, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_cLCMNoiseReferenceData.m_dNoiseMean = dMean;
                m_cDataInfo_List[nDataIndex].m_cLCMNoiseReferenceData.m_dNoiseStd = dStd;
            }

            double dRoundLCMPositiveReference = ComputeNoiseReference(m_cDataInfo_List[nDataIndex].m_cLCMNoiseReferenceData.m_dNoiseMean,
                                                                      m_cDataInfo_List[nDataIndex].m_cLCMNoiseReferenceData.m_dNoiseStd, true);

            double dRoundLCMNegativeReference = ComputeNoiseReference(m_cDataInfo_List[nDataIndex].m_cLCMNoiseReferenceData.m_dNoiseMean,
                                                                      m_cDataInfo_List[nDataIndex].m_cLCMNoiseReferenceData.m_dNoiseStd, false);

            m_cDataInfo_List[nDataIndex].m_cLCMNoiseReferenceData.m_dNoisePositiveReference = dRoundLCMPositiveReference;
            m_cDataInfo_List[nDataIndex].m_cLCMNoiseReferenceData.m_dNoiseNegativeReference = dRoundLCMNegativeReference;

            string sMessage = string.Format("Analysis : {0}/{1}", m_nProgressIndex + 1, m_nAnalysisCount);

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, m_nProgressIndex + 1, sMessage);
            });

            if (m_cfrmParent.m_bExecute == false)
                return false;

            m_nProgressIndex++;

            return true;
        }

        private bool ComputeSignalReferenceValue(int nDataIndex, int[,,] nOBASEMinusADC_Array)
        {
            int[,,] nDVFrameData_Array = nOBASEMinusADC_Array;
            bool[,] bFrameVerticalFlag_Array = m_cDataInfo_List[nDataIndex].m_bFrameVerticalFlag_Array;
            bool[,] bFrameHorizontalFlag_Array = m_cDataInfo_List[nDataIndex].m_bFrameHorizontalFlag_Array;

            double dTotalAreaReferenceValue = 0.0;
            int nTotalAreaCount = 0;
            bool bGetSignalAreaFlag = false;

            for (int nFrameIndex = 0; nFrameIndex < nDVFrameData_Array.GetLength(0); nFrameIndex++)
            {
                int nTXSignalTraceIndex = -1;
                List<SignalTraceInfo> cTXSignalTraceInfo_List = ClassifySignalArea(ref nTXSignalTraceIndex, bFrameHorizontalFlag_Array, m_nTXTraceNumber, nFrameIndex);

                int nRXSignalTraceIndex = -1;
                List<SignalTraceInfo> cRXSignalTraceInfo_List = ClassifySignalArea(ref nRXSignalTraceIndex, bFrameVerticalFlag_Array, m_nRXTraceNumber, nFrameIndex);

                if (nTXSignalTraceIndex > -1 && nRXSignalTraceIndex > -1)
                {
                    bGetSignalAreaFlag = true;
                    int nSignalAreaNumber = 0;
                    List<SignalAreaInfo> cSignalAreaInfo_List = new List<SignalAreaInfo>();

                    for (int nTXIndex = 0; nTXIndex < cTXSignalTraceInfo_List.Count; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < cRXSignalTraceInfo_List.Count; nRXIndex++)
                        {
                            SignalAreaInfo cSignalAreaInfo = new SignalAreaInfo();
                            cSignalAreaInfo.m_cTXSignalTraceInfo = cTXSignalTraceInfo_List[nTXIndex];
                            cSignalAreaInfo.m_cRXSignalTraceInfo = cRXSignalTraceInfo_List[nRXIndex];
                            cSignalAreaInfo.m_nNumber = nSignalAreaNumber;
                            cSignalAreaInfo_List.Add(cSignalAreaInfo);
                            nSignalAreaNumber++;
                        }
                    }

                    double dSignalAreaReferenceValue = -1.0;

                    for (int nAreaIndex = 0; nAreaIndex < cSignalAreaInfo_List.Count; nAreaIndex++)
                    {
                        SignalAreaInfo cSignalAreaInfo = cSignalAreaInfo_List[nAreaIndex];

                        #region Method 1
                        /*
                            double dAreaMean = 0.0;
                            int nAreaCount = 0;

                            for (int nY = CSArInfo.m_TXTraceInfo.m_nStart; nY <= CSArInfo.m_TXTraceInfo.m_nEnd; nY++)
                            {
                                for (int nX = CSArInfo.m_RXTraceInfo.m_nStart; nX <= CSArInfo.m_RXTraceInfo.m_nEnd; nX++)
                                {
                                    if (nDVFrameData_List[nFrameIdx][nY, nX] > 0)
                                    {
                                        dAreaMean += nDVFrameData_List[nFrameIdx][nY, nX];
                                        nAreaCount++;
                                    }
                                }
                            }

                            dAreaMean = Math.Round(dAreaMean / nAreaCount, 2, MidpointRounding.AwayFromZero);

                            int nTXNumberator = 0;
                            int nRXNumberator = 0;
                            int nDenominator = 0;

                            for (int nY = CSArInfo.m_TXTraceInfo.m_nStart; nY <= CSArInfo.m_TXTraceInfo.m_nEnd; nY++)
                            {
                                for (int nX = CSArInfo.m_RXTraceInfo.m_nStart; nX <= CSArInfo.m_RXTraceInfo.m_nEnd; nX++)
                                {
                                    if (nDVFrameData_List[nFrameIdx][nY, nX] > dAreaMean)
                                    {
                                        nTXNumberator += nDVFrameData_List[nFrameIdx][nY, nX] * nY;
                                        nRXNumberator += nDVFrameData_List[nFrameIdx][nY, nX] * nX;
                                        nDenominator += nDVFrameData_List[nFrameIdx][nY, nX];
                                    }
                                }
                            }

                            double dCG_TX = Math.Round((double)nTXNumberator / nDenominator, 1, MidpointRounding.AwayFromZero);
                            double dCG_RX = Math.Round((double)nRXNumberator / nDenominator, 1, MidpointRounding.AwayFromZero);

                            int nCGStart_TX = (int)fCG_TX;
                            int nCGEnd_TX = (int)fCG_TX + 1;
                            int nCGStart_RX = (int)fCG_RX;
                            int nCGEnd_RX = (int)fCG_RX + 1;
                            */
                        #endregion

                        #region Method 2
                        int nSQUARECELLCOUNT = 3;

                        int nMinCellCount = (cSignalAreaInfo.m_cTXSignalTraceInfo.m_nEnd - cSignalAreaInfo.m_cTXSignalTraceInfo.m_nStart + 1) < (cSignalAreaInfo.m_cRXSignalTraceInfo.m_nEnd - cSignalAreaInfo.m_cRXSignalTraceInfo.m_nStart + 1) ?
                                            cSignalAreaInfo.m_cTXSignalTraceInfo.m_nEnd - cSignalAreaInfo.m_cTXSignalTraceInfo.m_nStart + 1 : cSignalAreaInfo.m_cRXSignalTraceInfo.m_nEnd - cSignalAreaInfo.m_cRXSignalTraceInfo.m_nStart + 1;

                        if (nMinCellCount >= nSQUARECELLCOUNT)
                            nMinCellCount = nSQUARECELLCOUNT;

                        int nEnd_Y = (nMinCellCount < nSQUARECELLCOUNT) ? cSignalAreaInfo.m_cTXSignalTraceInfo.m_nEnd - 1 : cSignalAreaInfo.m_cTXSignalTraceInfo.m_nEnd - nSQUARECELLCOUNT + 1;
                        int nEnd_X = (nMinCellCount < nSQUARECELLCOUNT) ? cSignalAreaInfo.m_cRXSignalTraceInfo.m_nEnd - 1 : cSignalAreaInfo.m_cRXSignalTraceInfo.m_nEnd - nSQUARECELLCOUNT + 1;

                        double dMaxSquareValue = 0.0;

                        int nMaxSquareStart_X = 0;
                        int nMaxSquareStart_Y = 0;

                        for (int nY = cSignalAreaInfo.m_cTXSignalTraceInfo.m_nStart; nY <= nEnd_Y; nY++)
                        {
                            for (int nX = cSignalAreaInfo.m_cRXSignalTraceInfo.m_nStart; nX <= nEnd_X; nX++)
                            {
                                double dSquareValue = 0.0;
                                int nSquareCount = 0;
                                for (int nCell_X = nX; nCell_X < nX + nMinCellCount - 1; nCell_X++)
                                {
                                    for (int nCell_Y = nY; nCell_Y < nY + nMinCellCount - 1; nCell_Y++)
                                    {
                                        dSquareValue += nDVFrameData_Array[nFrameIndex, nY, nX];
                                        nSquareCount++;
                                    }
                                }

                                dSquareValue = Math.Round(dSquareValue / nSquareCount, 3, MidpointRounding.AwayFromZero);

                                if (nY == cSignalAreaInfo.m_cTXSignalTraceInfo.m_nStart && nX == cSignalAreaInfo.m_cRXSignalTraceInfo.m_nStart)
                                {
                                    dMaxSquareValue = dSquareValue;
                                    nMaxSquareStart_X = nX;
                                    nMaxSquareStart_Y = nY;
                                }
                                else if (dSquareValue > dMaxSquareValue)
                                {
                                    dMaxSquareValue = dSquareValue;
                                    nMaxSquareStart_X = nX;
                                    nMaxSquareStart_Y = nY;
                                }
                            }
                        }

                        double dMaxAreaMean = 0.0;
                        int nMaxAreaCount = 0;

                        if (nMinCellCount == nSQUARECELLCOUNT)
                        {
                            int nTXNumberator = 0;
                            int nRXNumberator = 0;
                            int nDenominator = 0;

                            int nNegativeMinValue = 0;

                            for (int nY = nMaxSquareStart_Y; nY <= nMaxSquareStart_Y + nMinCellCount - 1; nY++)
                            {
                                for (int nX = nMaxSquareStart_X; nX <= nMaxSquareStart_X + nMinCellCount - 1; nX++)
                                {
                                    if (nDVFrameData_Array[nFrameIndex, nY, nX] < 0 &&
                                        nDVFrameData_Array[nFrameIndex, nY, nX] < nNegativeMinValue)
                                        nNegativeMinValue = nDVFrameData_Array[nFrameIndex, nY, nX];
                                }
                            }

                            for (int nY = nMaxSquareStart_Y; nY <= nMaxSquareStart_Y + nMinCellCount - 1; nY++)
                            {
                                for (int nX = nMaxSquareStart_X; nX <= nMaxSquareStart_X + nMinCellCount - 1; nX++)
                                {
                                    int nOffsetValue = nDVFrameData_Array[nFrameIndex, nY, nX] - nNegativeMinValue;

                                    nTXNumberator += nOffsetValue * nY;
                                    nRXNumberator += nOffsetValue * nX;
                                    nDenominator += nOffsetValue;
                                }
                            }

                            double dCG_TX = Math.Round((double)nTXNumberator / nDenominator, 2, MidpointRounding.AwayFromZero);
                            double dCG_RX = Math.Round((double)nRXNumberator / nDenominator, 2, MidpointRounding.AwayFromZero);

                            int nCGStart_TX = (int)dCG_TX;
                            int nCGEnd_TX = (int)dCG_TX + 1;
                            int nCGStart_RX = (int)dCG_RX;
                            int nCGEnd_RX = (int)dCG_RX + 1;

                            for (int nY = nCGStart_TX; nY <= nCGEnd_TX; nY++)
                            {
                                for (int nX = nCGStart_RX; nX <= nCGEnd_RX; nX++)
                                {
                                    dMaxAreaMean += nDVFrameData_Array[nFrameIndex, nY, nX];
                                    nMaxAreaCount++;
                                }
                            }
                        }
                        else
                        {
                            for (int nY = nMaxSquareStart_Y; nY <= nMaxSquareStart_Y + nMinCellCount - 1; nY++)
                            {
                                for (int nX = nMaxSquareStart_X; nX <= nMaxSquareStart_X + nMinCellCount - 1; nX++)
                                {
                                    dMaxAreaMean += nDVFrameData_Array[nFrameIndex, nY, nX];
                                    nMaxAreaCount++;
                                }
                            }
                        }
                        #endregion

                        dMaxAreaMean = Math.Round(dMaxAreaMean / nMaxAreaCount, 2, MidpointRounding.AwayFromZero);

                        if (nAreaIndex == 0)
                            dSignalAreaReferenceValue = dMaxAreaMean;
                        else if (dMaxAreaMean > dSignalAreaReferenceValue)
                            dSignalAreaReferenceValue = dMaxAreaMean;
                    }

                    dTotalAreaReferenceValue += dSignalAreaReferenceValue;
                    nTotalAreaCount++;
                }
            }

            if (bGetSignalAreaFlag == false)
            {
                m_sErrorMessage = string.Format("No Signal Area in Frequency={0:0.000}KHz", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000"));
                return false;
            }

            dTotalAreaReferenceValue = Math.Round(dTotalAreaReferenceValue / nTotalAreaCount, 3, MidpointRounding.AwayFromZero);

            m_cDataInfo_List[nDataIndex].m_dSignalReferenceValue = dTotalAreaReferenceValue;

            if (m_cfrmParent.m_bExecute == false)
                return false;

            return true;
        }

        private List<SignalTraceInfo> ClassifySignalArea(ref int nSignalTraceIndex, bool[,] bFlag_Array, int nTraceNumber, int nFrameIndex)
        {
            bool bGetAreaFlag = false;
            List<SignalTraceInfo> cSignalTraceInfo_List = new List<SignalTraceInfo>();

            for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
            {
                if (bGetAreaFlag == false)
                {
                    if (bFlag_Array[nFrameIndex, nTraceIndex] == true)
                    {
                        SignalTraceInfo cSignalTraceInfo = new SignalTraceInfo();
                        cSignalTraceInfo.m_nStart = nTraceIndex;
                        cSignalTraceInfo_List.Add(cSignalTraceInfo);
                        nSignalTraceIndex++;
                        bGetAreaFlag = true;
                    }
                }
                else
                {
                    if (bFlag_Array[nFrameIndex, nTraceIndex] == false)
                    {
                        cSignalTraceInfo_List[nSignalTraceIndex].m_nEnd = nTraceIndex - 1;
                        bGetAreaFlag = false;
                    }
                    else if (nTraceIndex == nTraceNumber - 1)
                    {
                        cSignalTraceInfo_List[nSignalTraceIndex].m_nEnd = nTraceIndex;
                        bGetAreaFlag = false;
                    }
                }
            }

            return cSignalTraceInfo_List;
        }

        private bool ComputeSNRValue()
        {
            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                double dRawSNRValue_SignalToACNoise = 0.0;

                if (m_nSNRMethodType == m_nSNRMethod_Origin)
                {
                    m_cDataInfo_List[nDataIndex].m_dRawSignalValue_SignalToACNoise = cDataInfo.m_dSignalReferenceValue + cDataInfo.m_cACNoiseReferenceData.m_dNoiseNegativeReference;
                    m_cDataInfo_List[nDataIndex].m_dRawNoiseValue_SignalToACNoise = cDataInfo.m_cACNoiseReferenceData.m_dNoisePositiveReference;

                    dRawSNRValue_SignalToACNoise = Math.Round((cDataInfo.m_dSignalReferenceValue + cDataInfo.m_cACNoiseReferenceData.m_dNoiseNegativeReference) /
                                                               cDataInfo.m_cACNoiseReferenceData.m_dNoisePositiveReference, 3, MidpointRounding.AwayFromZero);
                }
                else if (m_nSNRMethodType == m_nSNRMethod_Sqrt)
                {
                    m_cDataInfo_List[nDataIndex].m_dRawSignalValue_SignalToACNoise = cDataInfo.m_dSignalReferenceValue;
                    double dSqrtValue = Math.Sqrt(cDataInfo.m_cACNoiseReferenceData.m_dNoisePositiveReference * cDataInfo.m_cACNoiseReferenceData.m_dNoisePositiveReference +
                                                  cDataInfo.m_cACNoiseReferenceData.m_dNoiseNegativeReference * cDataInfo.m_cACNoiseReferenceData.m_dNoiseNegativeReference);
                    dSqrtValue = Math.Round(dSqrtValue, 3, MidpointRounding.AwayFromZero);
                    m_cDataInfo_List[nDataIndex].m_dRawNoiseValue_SignalToACNoise = dSqrtValue;

                    dRawSNRValue_SignalToACNoise = Math.Round(cDataInfo.m_dSignalReferenceValue / dSqrtValue, 3, MidpointRounding.AwayFromZero);
                }

                if (dRawSNRValue_SignalToACNoise <= 0)
                    dRawSNRValue_SignalToACNoise = 0.001;

                m_cDataInfo_List[nDataIndex].m_dRawSNRValue_SignalToACNoise = dRawSNRValue_SignalToACNoise;

                double dACSNRValue = Math.Round(20 * Math.Log10(dRawSNRValue_SignalToACNoise), 3, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_dSNRValue_SignalToACNoise = dACSNRValue;

                if (ParamFingerAutoTuning.m_nACFRModeType != 1)
                {
                    double dRawSNRValue_SignalToLCMNoise = 0.0;

                    if (m_nSNRMethodType == m_nSNRMethod_Origin)
                    {
                        m_cDataInfo_List[nDataIndex].m_dRawSignalValue_SignalToLCMNoise = cDataInfo.m_dSignalReferenceValue + cDataInfo.m_cPreReportInfo.m_dNoiseNegativeReference;
                        m_cDataInfo_List[nDataIndex].m_dRawNoiseValue_SignalToLCMNoise = cDataInfo.m_cPreReportInfo.m_dNoisePositiveReference;

                        dRawSNRValue_SignalToLCMNoise = Math.Round((cDataInfo.m_dSignalReferenceValue + cDataInfo.m_cPreReportInfo.m_dNoiseNegativeReference) /
                                                                    cDataInfo.m_cPreReportInfo.m_dNoisePositiveReference, 3, MidpointRounding.AwayFromZero);
                    }
                    else if (m_nSNRMethodType == m_nSNRMethod_Sqrt)
                    {
                        m_cDataInfo_List[nDataIndex].m_dRawSignalValue_SignalToLCMNoise = cDataInfo.m_dSignalReferenceValue;
                        double dSqrtValue = Math.Sqrt(cDataInfo.m_cPreReportInfo.m_dNoisePositiveReference * cDataInfo.m_cPreReportInfo.m_dNoisePositiveReference +
                                                      cDataInfo.m_cPreReportInfo.m_dNoiseNegativeReference * cDataInfo.m_cPreReportInfo.m_dNoiseNegativeReference);
                        dSqrtValue = Math.Round(dSqrtValue, 3, MidpointRounding.AwayFromZero);
                        m_cDataInfo_List[nDataIndex].m_dRawNoiseValue_SignalToLCMNoise = dSqrtValue;

                        dRawSNRValue_SignalToLCMNoise = Math.Round(cDataInfo.m_dSignalReferenceValue / dSqrtValue, 3, MidpointRounding.AwayFromZero);
                    }

                    if (dRawSNRValue_SignalToLCMNoise <= 0)
                        dRawSNRValue_SignalToLCMNoise = 0.001;

                    m_cDataInfo_List[nDataIndex].m_dRawSNRValue_SignalToLCMNoise = dRawSNRValue_SignalToLCMNoise;

                    double dLCMSNRValue = Math.Round(20 * Math.Log10(dRawSNRValue_SignalToLCMNoise), 3, MidpointRounding.AwayFromZero);

                    m_cDataInfo_List[nDataIndex].m_dSNRValue_SignalToLCMNoise = dLCMSNRValue;
                }
                else
                {
                    double dRawSNRValue_SignalToLCMNoise = 0.0;

                    if (m_nSNRMethodType == m_nSNRMethod_Origin)
                    {
                        m_cDataInfo_List[nDataIndex].m_dRawSignalValue_SignalToLCMNoise = cDataInfo.m_dSignalReferenceValue + cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseNegativeReference;
                        m_cDataInfo_List[nDataIndex].m_dRawNoiseValue_SignalToLCMNoise = cDataInfo.m_cLCMNoiseReferenceData.m_dNoisePositiveReference;

                        dRawSNRValue_SignalToLCMNoise = Math.Round((cDataInfo.m_dSignalReferenceValue + cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseNegativeReference) /
                                                                    cDataInfo.m_cLCMNoiseReferenceData.m_dNoisePositiveReference, 3, MidpointRounding.AwayFromZero);
                    }
                    else if (m_nSNRMethodType == m_nSNRMethod_Sqrt)
                    {
                        m_cDataInfo_List[nDataIndex].m_dRawSignalValue_SignalToLCMNoise = cDataInfo.m_dSignalReferenceValue;
                        double dSqrtValue = Math.Sqrt(cDataInfo.m_cLCMNoiseReferenceData.m_dNoisePositiveReference * cDataInfo.m_cLCMNoiseReferenceData.m_dNoisePositiveReference +
                                                      cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseNegativeReference * cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseNegativeReference);
                        dSqrtValue = Math.Round(dSqrtValue, 3, MidpointRounding.AwayFromZero);
                        m_cDataInfo_List[nDataIndex].m_dRawNoiseValue_SignalToLCMNoise = dSqrtValue;

                        dRawSNRValue_SignalToLCMNoise = Math.Round(cDataInfo.m_dSignalReferenceValue / dSqrtValue, 3, MidpointRounding.AwayFromZero);
                    }

                    if (dRawSNRValue_SignalToLCMNoise <= 0)
                        dRawSNRValue_SignalToLCMNoise = 0.001;

                    m_cDataInfo_List[nDataIndex].m_dRawSNRValue_SignalToLCMNoise = dRawSNRValue_SignalToLCMNoise;

                    double dLCMSNRValue = Math.Round(20 * Math.Log10(dRawSNRValue_SignalToLCMNoise), 3, MidpointRounding.AwayFromZero);

                    m_cDataInfo_List[nDataIndex].m_dSNRValue_SignalToLCMNoise = dLCMSNRValue;
                }

                if (m_cfrmParent.m_bExecute == false)
                    return false;
            }

            return true;
        }

        private bool ComputeCompositeSNRValue()
        {
            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                double fSNRSum = cDataInfo.m_dRawSNRValue_SignalToACNoise + cDataInfo.m_dRawSNRValue_SignalToLCMNoise;

                double fCompositeRawSNR = Math.Round(cDataInfo.m_dRawSNRValue_SignalToACNoise * (cDataInfo.m_dRawSNRValue_SignalToLCMNoise / fSNRSum) +
                                                     cDataInfo.m_dRawSNRValue_SignalToLCMNoise * (cDataInfo.m_dRawSNRValue_SignalToACNoise / fSNRSum), 3, MidpointRounding.AwayFromZero);

                double fCompositeSNRValue = Math.Round(20 * Math.Log10(fCompositeRawSNR), 3, MidpointRounding.AwayFromZero);

                m_cDataInfo_List[nDataIndex].m_dRawSNRValue_Compostite = fCompositeRawSNR;
                m_cDataInfo_List[nDataIndex].m_dSNRValue_Compostite = fCompositeSNRValue;

                if (m_cfrmParent.m_bExecute == false)
                    return false;
            }

            return true;
        }

        private bool SaveConvolutionData(int nDataIndex, string sDataType, string sFileType, List<int[,]> nFrameData_List, int nTXTraceNumber, int nRXTraceNumber)
        {
            bool bErrorFlag = false;
            int nFrameNumber = nFrameData_List.Count;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                            sw.WriteLine(string.Format("{0}", nRXIndex + 1));
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                                sw.Write(string.Format("{0},", nFrameData_List[nFrameIndex][nTXIndex, nRXIndex]));
                            else
                                sw.WriteLine(string.Format("{0}", nFrameData_List[nFrameIndex][nTXIndex, nRXIndex]));
                        }
                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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

        private bool SaveParticularData(int nDataIndex, string sDataType, string sFileType, List<int[,]> nFrameData_List, int nTXTraceNumber, int nRXTraceNumber)
        {
            bool bErrorFlag = false;
            int nFrameNumber = nFrameData_List.Count;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            bool[,] bHorizontalFlag_Array = m_cDataInfo_List[nDataIndex].m_bFrameHorizontalFlag_Array;
            bool[,] bVerticalFlag_Array = m_cDataInfo_List[nDataIndex].m_bFrameVerticalFlag_Array;

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                            sw.WriteLine(nRXIndex + 1);
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                            {
                                if (bHorizontalFlag_Array[nFrameIndex, nTXIndex] == true && bVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                                    sw.Write(string.Format("{0},", nFrameData_List[nFrameIndex][nTXIndex, nRXIndex]));
                                else
                                    sw.Write(",");
                            }
                            else
                            {
                                if (bHorizontalFlag_Array[nFrameIndex, nTXIndex] == true && bVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                                    sw.WriteLine(nFrameData_List[nFrameIndex][nTXIndex, nRXIndex]);
                                else
                                    sw.WriteLine();
                            }
                        }
                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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
        private bool SaveParticularData(int nDataIndex, string sDataType, string sFileType, List<uint[,]> nFrameData_List, double[,] dHorizontalStd_Array, double[,] dVerticalStd_Array, 
                                        double[,] dVerticlaMean_Array, double[,] dVerticalMeanDiffer_Array, List<VerMeanDifferInfo> cVerticalMeanDifferInfo_List, 
                                        double[] dFrameStd_Array, double[] dFrameStdHorizontalTH_Array, double[] dFrameStdVerticalTH_Array, int nTXTrace, int nRXTrace)
        {
            bool bErrorFlag = false;
            int nFrameNumber = nFrameData_List.Count;

            string sDataTypeDirName = sDataType;
            string sDataTypeDirPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDataTypeDirName);

            if (Directory.Exists(sDataTypeDirPath) == false)
                Directory.CreateDirectory(sDataTypeDirPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dWorkingFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sDataFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileType, dWorkingFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sDataFilePath = string.Format(@"{0}\{1}.csv", sDataTypeDirPath, sDataFileName);

            FileStream fs = new FileStream(sDataFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int i = 0; i < nFrameNumber; i++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", i));

                    for (int nX = 0; nX < nRXTrace; nX++)
                    {
                        if (nX == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nX + 1));
                        }
                        else if (nX == nRXTrace - 1)
                            sw.WriteLine(string.Format("{0},,RXSTD", nX + 1));
                        else
                            sw.Write(string.Format("{0},", nX + 1));
                    }

                    sw.WriteLine();

                    for (int nY = 0; nY < nTXTrace; nY++)
                    {
                        sw.Write(string.Format("{0},,", nY + 1));

                        for (int nX = 0; nX < nRXTrace; nX++)
                        {
                            if (nX < nRXTrace - 1)
                                sw.Write(string.Format("{0},", nFrameData_List[i][nY, nX]));
                            else
                                sw.WriteLine(string.Format("{0},,{1}", nFrameData_List[i][nY, nX], dHorizontalStd_Array[i, nY]));
                        }
                    }

                    sw.WriteLine();

                    for (int nX = 0; nX < nRXTrace; nX++)
                    {
                        if (nX == 0)
                            sw.Write(string.Format("TXSTD,,{0},", dVerticalStd_Array[i, nX]));
                        else if (nX == nRXTrace - 1)
                            sw.WriteLine(string.Format("{0},,{1},{2},{3}", dVerticalStd_Array[i, nX], dFrameStd_Array[i], dFrameStdHorizontalTH_Array[i], dFrameStdVerticalTH_Array[i]));
                        else
                            sw.Write(string.Format("{0},", dVerticalStd_Array[i, nX]));
                    }

                    for (int nX = 0; nX < nRXTrace; nX++)
                    {
                        if (nX == 0)
                            sw.Write(string.Format("TXMean,,{0},", dVerticlaMean_Array[i, nX]));
                        else if (nX == nRXTrace - 1)
                            sw.WriteLine(dVerticlaMean_Array[i, nX]);
                        else
                            sw.Write(string.Format("{0},", dVerticlaMean_Array[i, nX]));
                    }

                    for (int nX = 0; nX < nRXTrace - 1; nX++)
                    {
                        if (nX == 0)
                            sw.Write(string.Format("TXMeanDiffer,,{0},", dVerticalMeanDiffer_Array[i, nX]));
                        else if (nX == nRXTrace - 2)
                            sw.WriteLine(dVerticalMeanDiffer_Array[i, nX]);
                        else
                            sw.Write(string.Format("{0},", dVerticalMeanDiffer_Array[i, nX]));
                    }

                    sw.Write("TXDVMean,,");

                    for (int nX = 0; nX < nRXTrace; nX++)
                    {
                        if (cVerticalMeanDifferInfo_List[i].nTrace == 0)
                        {
                            if (nX == cVerticalMeanDifferInfo_List[i].nTrace)
                                sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[i].fCurMeanValue));
                            else if (nX == cVerticalMeanDifferInfo_List[i].nTrace + 1)
                                sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[i].fNextMeanValue));
                            else
                            {
                                if (nX == nRXTrace - 1)
                                    sw.WriteLine();
                                else
                                    sw.Write(",");
                            }
                        }
                        else if (cVerticalMeanDifferInfo_List[i].nTrace == nRXTrace - 1)
                        {
                            if (nX == cVerticalMeanDifferInfo_List[i].nTrace - 1)
                                sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[i].fPreMeanValue));
                            else if (nX == cVerticalMeanDifferInfo_List[i].nTrace)
                                sw.WriteLine(cVerticalMeanDifferInfo_List[i].fCurMeanValue);
                            else
                                sw.Write(",");
                        }
                        else
                        {
                            if (nX == cVerticalMeanDifferInfo_List[i].nTrace - 1)
                                sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[i].fPreMeanValue));
                            else if (nX == cVerticalMeanDifferInfo_List[i].nTrace)
                                sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[i].fCurMeanValue));
                            else if (nX == cVerticalMeanDifferInfo_List[i].nTrace + 1)
                            {
                                if (nX == nRXTrace - 1)
                                    sw.WriteLine(cVerticalMeanDifferInfo_List[i].fNextMeanValue);
                                else
                                    sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[i].fNextMeanValue));
                            }
                            else
                            {
                                if (nX == nRXTrace - 1)
                                    sw.WriteLine();
                                else
                                    sw.Write(",");
                            }
                        }
                    }

                    if (i < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1}KHz", sDataType, dWorkingFrequency.ToString());
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

        private bool SaveParticularData(int nDataIndex, string sDataType, string sFileType, List<uint[,]> nFrameData_List, double[,] dVerticalMean_Array, 
                                        List<VerticalMeanDifferInfo> cVerticalMeanDifferInfo_List, int nTXTraceNumber, int nRXTraceNumber)
        {
            bool bError = false;
            int nFrameNumber = nFrameData_List.Count;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                            sw.WriteLine(nRXIndex + 1);
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                                sw.Write(string.Format("{0},", nFrameData_List[nFrameIndex][nTXIndex, nRXIndex]));
                            else
                                sw.WriteLine(nFrameData_List[nFrameIndex][nTXIndex, nRXIndex]);
                        }
                    }

                    sw.WriteLine();

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                            sw.Write(string.Format("RXMean,,{0},", dVerticalMean_Array[nFrameIndex, nRXIndex]));
                        else if (nRXIndex == nRXTraceNumber - 1)
                            sw.WriteLine(dVerticalMean_Array[nFrameIndex, nRXIndex]);
                        else
                            sw.Write(string.Format("{0},", dVerticalMean_Array[nFrameIndex, nRXIndex]));
                    }

                    sw.Write("RXDVMean,,");

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (cVerticalMeanDifferInfo_List[nFrameIndex].m_nTrace == 0)
                        {
                            if (nRXIndex == cVerticalMeanDifferInfo_List[nFrameIndex].m_nTrace)
                                sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[nFrameIndex].m_dCurrentMeanValue));
                            else if (nRXIndex == cVerticalMeanDifferInfo_List[nFrameIndex].m_nTrace + 1)
                                sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[nFrameIndex].m_dNextMeanValue));
                            else
                            {
                                if (nRXIndex == nRXTraceNumber - 1)
                                    sw.WriteLine();
                                else
                                    sw.Write(",");
                            }
                        }
                        else if (cVerticalMeanDifferInfo_List[nFrameIndex].m_nTrace == nRXTraceNumber - 1)
                        {
                            if (nRXIndex == cVerticalMeanDifferInfo_List[nFrameIndex].m_nTrace - 1)
                                sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[nFrameIndex].m_dPreviousMeanValue));
                            else if (nRXIndex == cVerticalMeanDifferInfo_List[nFrameIndex].m_nTrace)
                                sw.WriteLine(cVerticalMeanDifferInfo_List[nFrameIndex].m_dCurrentMeanValue);
                            else
                                sw.Write(",");
                        }
                        else
                        {
                            if (nRXIndex == cVerticalMeanDifferInfo_List[nFrameIndex].m_nTrace - 1)
                                sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[nFrameIndex].m_dPreviousMeanValue));
                            else if (nRXIndex == cVerticalMeanDifferInfo_List[nFrameIndex].m_nTrace)
                                sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[nFrameIndex].m_dCurrentMeanValue));
                            else if (nRXIndex == cVerticalMeanDifferInfo_List[nFrameIndex].m_nTrace + 1)
                            {
                                if (nRXIndex == nRXTraceNumber - 1)
                                    sw.WriteLine(cVerticalMeanDifferInfo_List[nFrameIndex].m_dNextMeanValue);
                                else
                                    sw.Write(string.Format("{0},", cVerticalMeanDifferInfo_List[nFrameIndex].m_dNextMeanValue));
                            }
                            else
                            {
                                if (nRXIndex == nRXTraceNumber - 1)
                                    sw.WriteLine();
                                else
                                    sw.Write(",");
                            }
                        }
                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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
        
        private bool SaveParticularDifferData(int nDataIndex, string sDataType, string sFileType, List<long[,]> lFrameDifferData_List, double[,] dDifferStd_Array, 
                                              double[] dFrameDifferStd_Array, int nTXTraceNumber, int nRXTraceNumber, int nDifferType)
        {
            bool bErrorFlag = false;
            int nFrameNumber = lFrameDifferData_List.Count;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                        {
                            if (nDifferType == m_nDIFFERTYPE_RX)
                                sw.WriteLine(string.Format("{0},,RXDifferSTD", nRXIndex + 1));
                            else
                                sw.WriteLine(string.Format("{0}", nRXIndex + 1));
                        }
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                                sw.Write(string.Format("{0},", lFrameDifferData_List[nFrameIndex][nTXIndex, nRXIndex]));
                            else
                            {
                                if (nDifferType == m_nDIFFERTYPE_RX)
                                    sw.WriteLine(string.Format("{0},,{1}", lFrameDifferData_List[nFrameIndex][nTXIndex, nRXIndex], dDifferStd_Array[nFrameIndex, nTXIndex]));
                                else
                                    sw.WriteLine(string.Format("{0}", lFrameDifferData_List[nFrameIndex][nTXIndex, nRXIndex]));
                            }
                        }
                    }

                    sw.WriteLine();

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            if (nDifferType == m_nDIFFERTYPE_TX)
                                sw.Write(string.Format("TXDifferSTD,,{0},", dDifferStd_Array[nFrameIndex, nRXIndex]));
                            else
                                sw.Write(",,,");
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                        {
                            if (nDifferType == m_nDIFFERTYPE_TX)
                                sw.WriteLine(string.Format("{0},,{1}", dDifferStd_Array[nFrameIndex, nRXIndex], dFrameDifferStd_Array[nFrameIndex]));
                            else
                                sw.WriteLine(string.Format(",,{0}", dFrameDifferStd_Array[nFrameIndex]));
                        }
                        else
                        {
                            if (nDifferType == m_nDIFFERTYPE_TX)
                                sw.Write(string.Format("{0},", dDifferStd_Array[nFrameIndex, nRXIndex]));
                            else
                                sw.Write(",");
                        }

                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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

        private bool SaveParticularDifferData(int nDataIndex, string sDataType, string sFileType, List<int[,]> nFrameDifferData_List, double[,] dDifferStd_Array, 
                                              double[] dFrameDifferStd_Array, int nTXTraceNumber, int nRXTraceNumber, int nDifferType)
        {
            bool bErrorFlag = false;
            int nFrameNumber = nFrameDifferData_List.Count;

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            string sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileType, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                        {
                            if (nDifferType == m_nDIFFERTYPE_RX)
                                sw.WriteLine(string.Format("{0},,RXDifferVerSTD", nRXIndex + 1));
                            else
                                sw.WriteLine(string.Format("{0}", nRXIndex + 1));
                        }
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                                sw.Write(string.Format("{0},", nFrameDifferData_List[nFrameIndex][nTXIndex, nRXIndex]));
                            else
                            {
                                if (nDifferType == m_nDIFFERTYPE_RX)
                                    sw.WriteLine(string.Format("{0},,{1}", nFrameDifferData_List[nFrameIndex][nTXIndex, nRXIndex], dDifferStd_Array[nFrameIndex, nTXIndex]));
                                else
                                    sw.WriteLine(string.Format("{0}", nFrameDifferData_List[nFrameIndex][nTXIndex, nRXIndex]));
                            }
                        }
                    }

                    sw.WriteLine();

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            if (nDifferType == m_nDIFFERTYPE_TX_RXDiffer)
                                sw.Write(string.Format("RXDifferSTD,,{0},", dDifferStd_Array[nFrameIndex, nRXIndex]));
                            else
                                sw.Write(",,,");
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                        {
                            if (nDifferType == m_nDIFFERTYPE_TX_RXDiffer)
                                sw.WriteLine(string.Format("{0},,{1}", dDifferStd_Array[nFrameIndex, nRXIndex], dFrameDifferStd_Array[nFrameIndex]));
                            else
                                sw.WriteLine(string.Format(",,{0}", dFrameDifferStd_Array[nFrameIndex]));
                        }
                        else
                        {
                            if (nDifferType == m_nDIFFERTYPE_TX_RXDiffer)
                                sw.Write(string.Format("{0},", dDifferStd_Array[nFrameIndex, nRXIndex]));
                            else
                                sw.Write(",");
                        }

                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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

        private bool SaveNoiseData(int nDataIndex, string sFileName, string sDataType, int[,,] nFrameData_Array, bool[,,] bNoiseData_Array, int nTXTraceNumber, int nRXTraceNumber)
        {
            bool bErrorFlag = false;
            int nFrameNumber = nFrameData_Array.GetLength(0);

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileName, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                            sw.WriteLine(string.Format("{0}", nRXIndex + 1));
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                            {
                                if (bNoiseData_Array[nFrameIndex, nTXIndex, nRXIndex] == true)
                                    sw.Write(string.Format("{0},", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]));
                                else
                                    sw.Write(",");
                            }
                            else
                            {
                                if (bNoiseData_Array[nFrameIndex, nTXIndex, nRXIndex] == true)
                                    sw.WriteLine(string.Format("{0}", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]));
                                else
                                    sw.WriteLine();
                            }
                        }
                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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

        private bool SaveLCMOrACNoiseData(int nDataIndex, string sFileName, string sDataType, int[,,] nFrameData_Array, int nTXTraceNumber, int nRXTraceNumber, bool bLCMNoiseFlag = true)
        {
            bool bErrorFlag = false;
            int nFrameNumber = nFrameData_Array.GetLength(0);

            string sDirectoryName = sDataType;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = m_cDataInfo_List[nDataIndex].m_nPH1Value;
            int nPH2 = m_cDataInfo_List[nDataIndex].m_nPH2Value;
            double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;

            sFileName = string.Format("{0}_{1:0.000}_{2}_{3}", sFileName, dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2.ToString("x2").ToUpper());
            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            bool[,] bFrameHorizontalFlag_Array = m_cDataInfo_List[nDataIndex].m_bFrameHorizontalFlag_Array;
            bool[,] bFrameVerticalFlag_Array = m_cDataInfo_List[nDataIndex].m_bFrameVerticalFlag_Array;

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                    {
                        if (nRXIndex == 0)
                        {
                            sw.Write(",,");
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                        }
                        else if (nRXIndex == nRXTraceNumber - 1)
                            sw.WriteLine(string.Format("{0}", nRXIndex + 1));
                        else
                            sw.Write(string.Format("{0},", nRXIndex + 1));
                    }

                    sw.WriteLine();

                    for (int nTXIndex = 0; nTXIndex < nTXTraceNumber; nTXIndex++)
                    {
                        sw.Write(string.Format("{0},,", nTXIndex + 1));

                        for (int nRXIndex = 0; nRXIndex < nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < nRXTraceNumber - 1)
                            {
                                if (bLCMNoiseFlag == true)
                                {
                                    if (bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == false)
                                        sw.Write(string.Format("{0},", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]));
                                    else
                                        sw.Write(",");
                                }
                                else
                                {
                                    if (bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                                        sw.Write(string.Format("{0},", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]));
                                    else
                                        sw.Write(",");
                                }
                            }
                            else
                            {
                                if (bLCMNoiseFlag == true)
                                {
                                    if (bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == false)
                                        sw.WriteLine(string.Format("{0}", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]));
                                    else
                                        sw.WriteLine();
                                }
                                else
                                {
                                    if (bFrameVerticalFlag_Array[nFrameIndex, nRXIndex] == true)
                                        sw.WriteLine(string.Format("{0}", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]));
                                    else
                                        sw.WriteLine();
                                }
                            }
                        }
                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save {0} Data Error in Frequency={1:0.000}KHz", sDataType, dFrequency.ToString("0.000"));
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

            string sTitleName = "SNR(dB) Distribution By Frequency";
            string sFilePath = string.Format(@"{0}\RefValueChart.jpg", sDirectoryPath);

            double dBestRankLB = 0.0;
            if (ParamFingerAutoTuning.m_nACFRModeType == 1)
            {
                m_nCompareOperator = m_nCOMPARE_Normalize;
                m_cDataInfo_List.Sort(new DataInfoComparer());

                if (m_cDataInfo_List.Count < ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber)
                    dBestRankLB = m_cDataInfo_List[m_cDataInfo_List.Count - 1].m_dSNRValue_Compostite;
                else
                    dBestRankLB = m_cDataInfo_List[ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber - 1].m_dSNRValue_Compostite;
            }

            m_nCompareOperator = m_nCOMPARE_Frequency;
            m_cDataInfo_List.Sort(new DataInfoComparer());

            Chart cChart = new Chart();

            if (ParamFingerAutoTuning.m_nACFRModeType != 1)
            {
                Series cSeries_Composite = new Series("Composite SNR(dB) vs. Frequency");
                Series cSeries_AC = new Series("AC SNR(dB) vs. Frequency");
                Series cSeries_LCM = new Series("LCM SNR(dB) vs. Frequency");

                //Show Line Chart
                var cChartArea = new ChartArea();
                cChart.ChartAreas.Add(cChartArea);
                cChart.Width = 1500;
                cChart.Height = 500;
                cChart.Legends.Add("Legend");
                cChart.Legends["Legend"].Font = new Font("Times New Roman", 10);
                cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
                cChart.Titles.Add(sTitleName);
                cChart.Titles[0].Font = new Font("Times New Roman", 18);
                cChart.ChartAreas[0].AxisY.Title = "SNR Value(dB)";
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
                cChart.ChartAreas[0].AxisX.Interval = 1;
                cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
                cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

                if (m_cDataInfo_List.Count > 10)
                    cChart.ChartAreas[0].AxisX.LabelStyle.Angle = 90;

                cSeries_Composite.ChartType = SeriesChartType.Column;
                cSeries_Composite.Color = Color.Blue;

                cSeries_AC.ChartType = SeriesChartType.Column;
                cSeries_AC.Color = Color.Orange;

                cSeries_LCM.ChartType = SeriesChartType.Column;
                cSeries_LCM.Color = Color.Green;

                string[] sFrequency_Array = new string[m_cDataInfo_List.Count];
                double[] dSNR_Composite_Array = new double[m_cDataInfo_List.Count];
                double[] dSNR_AC_Array = new double[m_cDataInfo_List.Count];
                double[] dSNR_LCM_Array = new double[m_cDataInfo_List.Count];

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                    sFrequency_Array[nDataIndex] = Convert.ToString(cDataInfo.m_dFrequency);
                    dSNR_Composite_Array[nDataIndex] = cDataInfo.m_dSNRValue_Compostite;
                    dSNR_AC_Array[nDataIndex] = cDataInfo.m_dSNRValue_SignalToACNoise;
                    dSNR_LCM_Array[nDataIndex] = cDataInfo.m_dSNRValue_SignalToLCMNoise;
                }

                cSeries_Composite.Points.DataBindXY(sFrequency_Array, dSNR_Composite_Array);
                cSeries_AC.Points.DataBindXY(sFrequency_Array, dSNR_AC_Array);
                cSeries_LCM.Points.DataBindXY(sFrequency_Array, dSNR_LCM_Array);

                cChart.Series.Add(cSeries_Composite);
                cChart.Series.Add(cSeries_AC);
                cChart.Series.Add(cSeries_LCM);
            }
            else
            {
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

                Series cSeries_Composite = new Series("Composite SNR(dB) vs. Frequency");
                Series cSeries_AC = new Series("AC SNR(dB) vs. Frequency");
                Series cSeries_LCM = new Series("LCM SNR(dB) vs. Frequency");

                //Show Line Chart
                var cChartArea = new ChartArea();
                cChart.ChartAreas.Add(cChartArea);
                cChart.Width = 1500;
                cChart.Height = 500;
                cChart.Legends.Add("Legend");
                cChart.Legends["Legend"].Font = new Font("Times New Roman", 10);
                cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
                cChart.Titles.Add(sTitleName);
                cChart.Titles[0].Font = new Font("Times New Roman", 18);
                cChart.ChartAreas[0].AxisY.Title = "SNR Value(dB)";
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

                if (m_nChartSNRValueHB != m_nChartSNRValueLB)
                {
                    if (m_nChartSNRValueHB < m_nChartSNRValueLB)
                    {
                        int nValue = m_nChartSNRValueHB;
                        m_nChartSNRValueHB = m_nChartSNRValueLB;
                        m_nChartSNRValueLB = nValue;
                    }

                    cChart.ChartAreas[0].AxisY.Maximum = m_nChartSNRValueHB;
                    cChart.ChartAreas[0].AxisY.Minimum = m_nChartSNRValueLB;
                }

                if (m_nChartSNRValueInterval > 0)
                    cChart.ChartAreas[0].AxisY.Interval = m_nChartSNRValueInterval;

                cSeries_Composite.ChartType = SeriesChartType.Line;
                cSeries_Composite.MarkerStyle = MarkerStyle.Circle;
                cSeries_Composite.MarkerSize = 5;
                cSeries_Composite.IsValueShownAsLabel = false;
                cSeries_Composite.Color = Color.Blue;

                cSeries_AC.ChartType = SeriesChartType.Line;
                cSeries_AC.MarkerStyle = MarkerStyle.Circle;
                cSeries_AC.MarkerSize = 5;
                cSeries_AC.IsValueShownAsLabel = false;
                cSeries_AC.Color = Color.Green;

                cSeries_LCM.ChartType = SeriesChartType.Line;
                cSeries_LCM.MarkerStyle = MarkerStyle.Circle;
                cSeries_LCM.MarkerSize = 5;
                cSeries_LCM.IsValueShownAsLabel = false;
                cSeries_LCM.Color = Color.Orange;

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];
                    var cLine_Composite = cSeries_Composite.Points[cSeries_Composite.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_dSNRValue_Compostite)];
                    var cLine_AC = cSeries_AC.Points[cSeries_AC.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_dSNRValue_SignalToACNoise)];
                    var cLine_LCM = cSeries_LCM.Points[cSeries_LCM.Points.AddXY(cDataInfo.m_dFrequency, cDataInfo.m_dSNRValue_SignalToLCMNoise)];

                    if (cDataInfo.m_dSNRValue_Compostite >= dBestRankLB)
                    {
                        cLine_Composite.MarkerColor = Color.Red;
                        cLine_Composite.Label = string.Format("{0}KHz", cDataInfo.m_dFrequency);
                        cLine_Composite.IsValueShownAsLabel = true;
                    }
                    else
                        cLine_Composite.MarkerColor = Color.Blue;

                    cLine_AC.MarkerColor = Color.Green;
                    cLine_LCM.MarkerColor = Color.Orange;

                    cLine_Composite.Color = Color.Blue;
                    cLine_AC.Color = Color.Green;
                    cLine_LCM.Color = Color.Orange;
                }

                cChart.Series.Add(cSeries_Composite);
                cChart.Series.Add(cSeries_AC);
                cChart.Series.Add(cSeries_LCM);
            }

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
            string sFilePath = string.Format(@"{0}\Analysis.csv", m_sLogDirectoryPath);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string[] sColumnHeader_Rank_Array = new string[] 
            { 
                "Rank", "PH1", "PH2", "Frequency(KHz)", "Signal RefValue", "", 
                "ACNoise Mean", "ACNoise Std", "ACNoise PosRef", "ACNoise NegRef", "AC SignalPart", "AC NoisePart", "AC RawSNR", "AC SNR(dB)", "", 
                "LCMNoise Mean", "LCMNoise Std", "LCMNoise PosRef", "LCMNoise NegRef", "LCM SignalPart", "LCM NoisePart", "LCM RawSNR", "LCMN SNR(dB)", "", 
                "Composite RawSNR", "Composite SNR(dB)" 
            };

            string[] sColumnHeader_Frequency_Array = new string[] 
            { 
                "PH1", "PH2", "Frequency(KHz)", "Signal RefValue", "", 
                "ACNoise Mean", "ACNoise Std", "ACNoise PosRef", "ACNoise NegRef", "AC SignalPart", "AC NoisePart", "AC RawSNR","AC SNR(dB)", "", 
                "LCMNoise Mean", "LCMNoise Std", "LCMNoise PosRef", "LCMNoise NegRef", "LCM SignalPart", "LCM NoisePart", "LCM RawSNR", "LCMN SNR(dB)", "", 
                "Composite RawSNR","Composite SNR(dB)" 
            };

            if (ParamFingerAutoTuning.m_nACFRModeType == 1)
            {
                sColumnHeader_Rank_Array = new string[] 
                { 
                    "Rank", "PH1", "PH2", "Frequency(KHz)", "DFT_NUM", "SuggestDFT_NUM", "Signal RefValue", "", 
                    "ACNoise Mean", "ACNoise Std", "ACNoise PosRef", "ACNoise NegRef", "AC SignalPart", "AC NoisePart", "AC RawSNR", "AC SNR(dB)", "", 
                    "LCMNoise Mean", "LCMNoise Std", "LCMNoise PosRef", "LCMNoise NegRef", "LCM SignalPart", "LCM NoisePart", "LCM RawSNR", "LCMN SNR(dB)", "", 
                    "Composite RawSNR", "Composite SNR(dB)" 
                };

                sColumnHeader_Frequency_Array = new string[] 
                { 
                    "PH1", "PH2", "Frequency(KHz)", "DFT_NUM", "SuggestDFT_NUM", "Signal RefValue", "", 
                    "ACNoise Mean", "ACNoise Std", "ACNoise PosRef", "ACNoise NegRef", "AC SignalPart", "AC NoisePart", "AC RawSNR", "AC SNR(dB)", "", 
                    "LCMNoise Mean", "LCMNoise Std", "LCMNoise PosRef", "LCMNoise NegRef", "LCM SignalPart", "LCM NoisePart", "LCM RawSNR", "LCMN SNR(dB)", "", 
                    "Composite RawSNR", "Composite SNR(dB)" 
                };
            }

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

                    if (ParamFingerAutoTuning.m_nACFRModeType == 1)
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                        sw.Write(string.Format("{0},", cDataInfo.m_nSuggestDFT_NUM.ToString()));
                    }

                    sw.Write(string.Format("{0},", cDataInfo.m_dSignalReferenceValue.ToString("0.000")));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoiseMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoiseStd.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoisePositiveReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoiseNegativeReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSignalValue_SignalToACNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawNoiseValue_SignalToACNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSNRValue_SignalToACNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dSNRValue_SignalToACNoise.ToString("0.000")));
                    sw.Write(",");

                    if (ParamFingerAutoTuning.m_nACFRModeType != 1)
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoiseMean.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoiseStd.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoisePositiveReference.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoiseNegativeReference.ToString("0.000")));
                        
                    }
                    else
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseMean.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseStd.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoisePositiveReference.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseNegativeReference.ToString("0.000")));
                    }

                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSignalValue_SignalToLCMNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawNoiseValue_SignalToLCMNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSNRValue_SignalToLCMNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dSNRValue_SignalToLCMNoise.ToString("0.000")));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSNRValue_Compostite.ToString("0.000")));
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_dSNRValue_Compostite.ToString("0.000")));
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

                    if (ParamFingerAutoTuning.m_nACFRModeType == 1)
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                        sw.Write(string.Format("{0},", cDataInfo.m_nSuggestDFT_NUM.ToString()));
                    }

                    sw.Write(string.Format("{0},", cDataInfo.m_dSignalReferenceValue.ToString("0.000")));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoiseMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoiseStd.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoisePositiveReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoiseNegativeReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSignalValue_SignalToACNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawNoiseValue_SignalToACNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSNRValue_SignalToACNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dSNRValue_SignalToACNoise.ToString("0.000")));
                    sw.Write(",");

                    if (ParamFingerAutoTuning.m_nACFRModeType != 1)
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoiseMean.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoiseStd.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoisePositiveReference.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoiseNegativeReference.ToString("0.000")));
                    }
                    else
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseMean.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseStd.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoisePositiveReference.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseNegativeReference.ToString("0.000")));
                    }

                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSignalValue_SignalToLCMNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawNoiseValue_SignalToLCMNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSNRValue_SignalToLCMNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dSNRValue_SignalToLCMNoise.ToString("0.000")));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSNRValue_Compostite.ToString("0.000")));
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_dSNRValue_Compostite.ToString("0.000")));
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
            string sFilePath = string.Format(@"{0}\Report.csv", m_sLogDirectoryPath);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string[] sColumnHeader_FrequencyRank_Array = new string[] 
            { 
                "Rank", "Frequency(KHz)", "PH1+PH2", "Minimum PH1", "Minimum PH2", "Signal RefValue", 
                "AC Noise Mean", "AC Noise Std", "AC Noise PosRef", "AC Noise NegRef", "AC RawSNR", "AC SNR(dB)",
                "LCM Noise Mean", "LCM Noise Std", "LCM Noise PosRef", "LCM Noise NegRef", "LCM RawSNR", "LCM SNR(dB)", 
                "Composite RawSNR", "Composite SNR(dB)" 
            };

            if (ParamFingerAutoTuning.m_nACFRModeType == 1)
            {
                sColumnHeader_FrequencyRank_Array = new string[] 
                { 
                    "Rank", "Frequency(KHz)", "PH1+PH2", "Minimum PH1", "Minimum PH2", "DFT_NUM", "SuggestDFT_NUM", "Signal RefValue", 
                    "AC Noise Mean", "AC Noise Std", "AC Noise PosRef", "AC Noise NegRef", "AC RawSNR", "AC SNR(dB)",
                    "LCM Noise Mean", "LCM Noise Std", "LCM Noise PosRef", "LCM Noise NegRef", "LCM RawSNR", "LCM SNR(dB)", 
                    "Composite RawSNR", "Composite SNR(dB)" 
                };
            }

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

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    DataInfo cDataInfo = m_cDataInfo_List[nDataIndex];

                    int nPH1PH2Sum = cDataInfo.m_nPH1Value + cDataInfo.m_nPH2Value;

                    sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("0x{0},", nPH1PH2Sum.ToString("x2").ToUpper()));
                    sw.Write(string.Format("0x{0},", m_nMinPH1.ToString("x2").ToUpper()));
                    sw.Write(string.Format("0x{0},", m_nPH2LB.ToString("x2").ToUpper()));

                    if (ParamFingerAutoTuning.m_nACFRModeType == 1)
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                        sw.Write(string.Format("{0},", cDataInfo.m_nSuggestDFT_NUM.ToString()));
                    }

                    sw.Write(string.Format("{0},", cDataInfo.m_dSignalReferenceValue.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoiseMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoiseStd.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoisePositiveReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_cACNoiseReferenceData.m_dNoiseNegativeReference.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSNRValue_SignalToACNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dSNRValue_SignalToACNoise.ToString("0.000")));

                    if (ParamFingerAutoTuning.m_nACFRModeType != 1)
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoiseMean.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoiseStd.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoisePositiveReference.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cPreReportInfo.m_dNoiseNegativeReference.ToString("0.000")));
                    }
                    else
                    {
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseMean.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseStd.ToString("0.00")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoisePositiveReference.ToString("0.000")));
                        sw.Write(string.Format("{0},", cDataInfo.m_cLCMNoiseReferenceData.m_dNoiseNegativeReference.ToString("0.000")));
                    }

                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSNRValue_SignalToLCMNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dSNRValue_SignalToLCMNoise.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawSNRValue_Compostite.ToString("0.000")));
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_dSNRValue_Compostite.ToString("0.000")));
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

        #region Convolution Method
        private int[,] Alloc2Int(int nRow, int nColumn)
        {
            int[,] nData_Array = null;

            nData_Array = new int[nRow, nColumn];

            return nData_Array;
        }

        private void Free2Int(int[,] nData_Array)
        {
            nData_Array = null;
        }

        private void Print2Int(string sMessage, float[,] fData_Array, int nRow, int nColumn)
        {
            Console.WriteLine(sMessage);

            for (int nRowIndex = 0; nRowIndex < nRow; nRowIndex++)
            {
                for (int nColumnIndex = 0; nColumnIndex < nColumn; nColumnIndex++)
                    Console.Write(fData_Array[nRowIndex, nColumnIndex].ToString() + "   ");

                Console.WriteLine("");
            }

            Console.WriteLine("");
        }

        private int[,] Flip90_Idx2(int[,] nInput_Array, int nRow, int nColumn)
        {
            int nRowIndex, nColumnIndex;
            int nValue_1, nValue_2;
            int[,] nOutput_Array = Alloc2Int(nRow, nColumn);

            for (nRowIndex = 0; nRowIndex < nRow; nRowIndex++)
            {
                nValue_1 = nRow - 1 - nRowIndex;

                for (nColumnIndex = 0; nColumnIndex < nColumn; nColumnIndex++)
                {
                    nValue_2 = nColumn - 1 - nColumnIndex;
                    nOutput_Array[nRowIndex, nColumnIndex] = nInput_Array[nValue_1, nValue_2];
                }
            }

            return nOutput_Array;
        }

        private int[,] ComputeConvolution(int[,] nInput_Array, int r, int c, int[,] k, int m, int n)
        {
            int x, y, i, j, q, w;

            int[,] nH_Array = Flip90_Idx2(k, m, n);
            int nCenterR = Convert.ToInt32(Math.Floor((m + 1) / 2.0));
            int nCenterC = Convert.ToInt32(Math.Floor((n + 1) / 2.0));
            int nLeft = nCenterC - 1;
            int nRight = n - nCenterC;
            int nTop = nCenterR - 1;
            int nBottom = m - nCenterR;

            int[,] nRep_Array = Alloc2Int(r + nTop + nBottom, c + nLeft + nRight);

            for (x = nTop; x < r + nTop; x++)
            {
                for (y = nLeft; y < c + nLeft; y++)
                    nRep_Array[x, y] = nInput_Array[x - nTop, y - nLeft];
            }

            int[,] nOutput_Array = Alloc2Int(r, c);

            for (x = 0; x < r; x++)
            {
                for (y = 0; y < c; y++)
                {
                    for (i = 0; i < m; i++)
                    {
                        for (j = 0; j < n; j++)
                        {
                            q = x;
                            w = y;
                            nOutput_Array[x, y] = nOutput_Array[x, y] + (nRep_Array[i + q, j + w] * nH_Array[i, j]);
                        }
                    }
                }
            }

            Free2Int(nRep_Array);

            return nOutput_Array;
        }
        #endregion

        private int[,] Get2DArrayFrom3DArray(int[,,] n3DData_Array, int nFrameIndex)
        {
            // 獲取二維 Array 的尺寸
            int nWidth = n3DData_Array.GetLength(1);
            int nHeight = n3DData_Array.GetLength(2);

            // 創建一個新的二維 Array 並從三維 Array 複製資料
            int[,] n2DData_Array = new int[nWidth, nHeight];

            for (int i = 0; i < nWidth; i++)
            {
                for (int j = 0; j < nHeight; j++)
                {
                    n2DData_Array[i, j] = n3DData_Array[nFrameIndex, i, j];
                }
            }

            return n2DData_Array;
        }
    }

    static class ExtendMethod
    {
        public static int GetMedian(this IEnumerable<int> source)
        {
            // Create a copy of the input, and sort the copy
            int[] nTemp_Array = source.ToArray();
            Array.Sort(nTemp_Array);
            int nCount = nTemp_Array.Length;

            if (nCount == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (nCount % 2 == 0)
            {
                // count is even, average two middle elements
                int nA = nTemp_Array[nCount / 2 - 1];
                int nB = nTemp_Array[nCount / 2];
                int nValue = (int)Math.Round(((double)nA + nB) / 2, 0, MidpointRounding.AwayFromZero);
                return nValue;
            }
            else
            {
                // count is odd, return the middle element
                return nTemp_Array[nCount / 2];
            }
        }

        public static int GetMode(this IEnumerable<int> nData_List)
        {
            // Initialize the return value
            int nMode = default(int);

            // Test for a null reference and an empty list
            if (nData_List != null && nData_List.Count() > 0)
            {
                // Store the number of occurences for each element
                Dictionary<int, int> dictCounts = new Dictionary<int, int>();

                // Add one to the count for the occurence of a character
                foreach (int nElement in nData_List)
                {
                    if (dictCounts.ContainsKey(nElement))
                        dictCounts[nElement]++;
                    else
                        dictCounts.Add(nElement, 1);
                }
                // Loop through the counts of each element and find the 
                // element that occurred most often
                int nMax = 0;

                foreach (KeyValuePair<int, int> structCount in dictCounts)
                {
                    if (structCount.Value > nMax)
                    {
                        // Update the mode
                        nMode = structCount.Key;
                        nMax = structCount.Value;
                    }
                }
            }

            return nMode;
        }
    }

    public class KMeansMethod
    {
        // 定義群集
        public class Cluster
        {
            public double dCentroid { get; set; }
            public List<double> dDataPoints_List { get; set; }
        }

        // K-Means分群算法
        public static List<Cluster> KMeans(double[] dDataPoints_Array, int k, int maxIterations = 100)
        {
            Random cRandom = new Random();
            List<Cluster> cClusters_List = new List<Cluster>();

            // 隨機初始化k個中心點
            for (int i = 0; i < k; i++)
            {
                int nRandomIndex = cRandom.Next(dDataPoints_Array.Length);
                double dRandomPoint = dDataPoints_Array[nRandomIndex];
                cClusters_List.Add(new Cluster { dCentroid = dRandomPoint, dDataPoints_List = new List<double>() });
            }

            // 疊代更新中心點
            for (int nIteration = 0; nIteration < maxIterations; nIteration++)
            {
                // 清空每個群集的點清
                foreach (var varCluster in cClusters_List)
                {
                    varCluster.dDataPoints_List.Clear();
                }

                // 將每個數據點分配到最近的中心點
                foreach (var varDataPoint in dDataPoints_Array)
                {
                    Cluster nearestCluster = cClusters_List.OrderBy(cluster => Math.Abs(varDataPoint - cluster.dCentroid)).First();
                    nearestCluster.dDataPoints_List.Add(varDataPoint);
                }

                // 更新每個群集的中心點
                bool bConverged = true;

                foreach (var varCluster in cClusters_List)
                {
                    double dOldCentroid = varCluster.dCentroid;
                    double dNewCentroid = varCluster.dDataPoints_List.Average();
                    varCluster.dCentroid = dNewCentroid;

                    if (Math.Abs(dOldCentroid - dNewCentroid) > 0.001)
                    {
                        bConverged = false;
                    }
                }

                if (bConverged)
                {
                    break;
                }
            }

            return cClusters_List;
        }
    }
}
