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
    public class AnalysisFlow_FRPH1 : AnalysisFlow
    {
        private int m_nFixedPH1 = 0x10;
        //private double m_dUniformityLimitLB = 50.0;
        //private double m_dCurveFitFrequencyLB = 0.0;
        private double m_dRecordRegionFrequencyLB = 80.0;
        private double m_dRecordRegionFrequencyHB = 250.0;
        private double m_dShiftValue = 0.999;
        private double m_dUniformityLeastLB = 5.0;
        private int m_nUniformityDataCountLB = 4;
        private bool m_bEnableTXn = false;

        public class DataInfo
        {
            public string m_sFileName = "";
            public int m_nSetIndex = -1;
            public int m_nPH1Value = 0;
            public int m_nPH2Value = 0;
            public int m_nPH3Value = 0;
            public int m_nDFT_NUMValue = 0;
            public int m_nRXTraceNumber = 0;
            public int m_nTXTraceNumber = 0;
            public int m_nFWIP_Option = 0;
            public double m_dFrequency = 0.0;
            public double m_dSingleFrameUniformityMin = 0.0;
            public double m_dSingleFrameUniformityMean = 0.0;
            public double m_dUniformityMean = 0.0;
            //public double m_dUniformityStd = 0.0;
        }

        public class DataInfoComparer : IComparer<DataInfo>
        {
            public int Compare(DataInfo cDataInfo1, DataInfo cDataInfo2)
            {
                return cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency);
            }
        }

        private List<DataInfo> m_cDataInfo_List = new List<DataInfo>();

        public class SatnRegEdgePctLogFitData
        {
            public double[] m_dLogFitCoefficient_Array;

            public List<double> m_dLogFitRatioValue_X_List = new List<double>();
            public List<double> m_dLogFitRatioValue_Y_List = new List<double>();
        }

        private SatnRegEdgePctLogFitData m_cSatnRegEdgePctLogFitData = new SatnRegEdgePctLogFitData();

        public class ResultData
        {
            public double m_dExtremeMaxUniformity = 0.0;
            public double m_dExtremeMaxFrequency = 0.0;
            public double m_dRecordRegionCurveFitMaxUniformity = 0.0;
            public double m_dRecordRegionCurveFitMaxFrequency = 0.0;
            public double m_dRecordRegionRealMaxUniformity = 0.0;

            public double m_dTotalMaxUniformity = 0.0;

            public double m_dSaturationRatio = 0.82;
            public double m_dSaturationUniformityLB = 0.0;
            public double m_dSaturationMaxFrequency = 0.0;
            public int m_nSaturationMinPH2 = 0;
        }

        private ResultData m_cResultData = new ResultData();

        public class CurveFitData
        {
            public List<double> m_dUniformityData_List = new List<double>();

            public double[] m_dCoefficient_Array;
        }

        private CurveFitData m_cCurveFitData = new CurveFitData();

        public class NormalData
        {
            public List<double> m_dFrequencyData_List = new List<double>();
            public List<double> m_dUniformityData_List = new List<double>();
        }

        private NormalData m_cTotalAndRecordData = new NormalData();

        public class SkipFrequencyData
        {
            public double m_dFrequency = 0.0;
            public int m_nPH1PH2Sum = 0;
            public string m_sMessage = "";
        }

        private List<SkipFrequencyData> m_cSkipFrequencyData_List = new List<SkipFrequencyData>();

        private bool m_bWarningOccurred = false;

        private string m_sSkipFreqSetFilePath = "";

        public AnalysisFlow_FRPH1(frmMain.FlowStep cFlowStep, string sLogDirectoryPath, string sH5LogDirectoryPath, bool bGenerateH5Data, frmMain cfrmParent, string sProjectName)
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
            m_nFixedPH1 = ParamFingerAutoTuning.m_nFRPH1FixedPH1;

            m_dRecordRegionFrequencyLB = ParamFingerAutoTuning.m_dFRPH1ScanFrequencyLB;
            m_dRecordRegionFrequencyHB = ParamFingerAutoTuning.m_dFRPH1ScanFrequencyHB;
        }

        public override void InitializeSourceDataList()
        {
            m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
        }

        public void GetSkipFreqSetFilePath(string sSkipFreqSetFilePath)
        {
            m_sSkipFreqSetFilePath = sSkipFreqSetFilePath;
        }

        public override bool MainFlow(ref string sErrorMessage)
        {
            if (GetDataCount() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (ReadData() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            bool bAnalysisErrorFlag = false;

            if (AnalysisData() == false)
            {
                //SetErrorMessage(ref sErrorMessage);
                //return false;
                bAnalysisErrorFlag = true;
            }

            if (SaveAnalysisFile(bAnalysisErrorFlag) == false)
            {
                CopyDataToH5Directory();
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (SaveUnifomrityChartFile() == false)
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

            if (SaveSkipFreqSetFile() == false)
            {
                CopyDataToH5Directory();
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (CheckUniformityValueCalculationError() == false)
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

        private bool ReadData()
        {
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_ADC);

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

                if (CheckFileInfoIdentical(cFileCheckInfo, sFileName) == false)
                    return false;

                double dFrequncy = ElanConvert.Convert2Frequency(cFileCheckInfo.m_nReadPH1, cFileCheckInfo.m_nReadPH2);

                DataInfo cDataInfo = new DataInfo();
                cDataInfo.m_sFileName = sFileName;
                cDataInfo.m_nSetIndex = cFileCheckInfo.m_nSetIndex;
                cDataInfo.m_dFrequency = dFrequncy;
                cDataInfo.m_nPH1Value = cFileCheckInfo.m_nReadPH1;
                cDataInfo.m_nPH2Value = cFileCheckInfo.m_nReadPH2;
                cDataInfo.m_nPH3Value = cFileCheckInfo.m_nReadPH3;
                cDataInfo.m_nDFT_NUMValue = cFileCheckInfo.m_nReadDFT_NUM;
                cDataInfo.m_nRXTraceNumber = cFileCheckInfo.m_nRXTraceNumber;
                cDataInfo.m_nTXTraceNumber = cFileCheckInfo.m_nTXTraceNumber;
                cDataInfo.m_nFWIP_Option = cFileCheckInfo.m_nFWIP_Option;
                m_cDataInfo_List.Add(cDataInfo);
                int nListIndex = m_cDataInfo_List.Count - 1;

                srFile = new StreamReader(sFilePath, Encoding.Default);

                if (GetFrameData(ref nFrameData_List, cFileCheckInfo, srFile, sFileName) == false)
                    return false;

                if (ComputeUniformityData(nFrameData_List, nListIndex, cFileCheckInfo, sFileName) == false)
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

            return true;
        }

        private bool CheckFileInfoIdentical(FileCheckInfo cFileCheckInfo, string sFileName)
        {
            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                if (m_cDataInfo_List[nDataIndex].m_nPH1Value != cFileCheckInfo.m_nReadPH1)
                {
                    m_sErrorMessage = string.Format("PH1 Unique Check Error in {0} and {1}[Value1=0x{2} Value2=0x{3}]", sFileName,
                                                    m_cDataInfo_List[nDataIndex].m_sFileName,
                                                    cFileCheckInfo.m_nReadPH1.ToString("x2").ToUpper(),
                                                    m_cDataInfo_List[nDataIndex].m_nPH1Value.ToString("x2").ToUpper());
                    return false;
                }

                if (m_cDataInfo_List[nDataIndex].m_nPH2Value == cFileCheckInfo.m_nReadPH2)
                {
                    m_sErrorMessage = string.Format("PH2 Unique Check Error in {0} and {1}[Value=0x{2}]", sFileName,
                                                    m_cDataInfo_List[nDataIndex].m_sFileName,
                                                    cFileCheckInfo.m_nReadPH2.ToString("x2").ToUpper());
                    return false;
                }

                if (m_cDataInfo_List[nDataIndex].m_nTXTraceNumber != cFileCheckInfo.m_nTXTraceNumber)
                {
                    m_sErrorMessage = string.Format("TXTraceNumber Identical Check Error in {0} and {1}[Number1={2} Number2={3}]", sFileName,
                                                    m_cDataInfo_List[nDataIndex].m_sFileName,
                                                    cFileCheckInfo.m_nTXTraceNumber,
                                                    m_cDataInfo_List[nDataIndex].m_nTXTraceNumber);
                    return false;
                }

                if (m_cDataInfo_List[nDataIndex].m_nRXTraceNumber != cFileCheckInfo.m_nRXTraceNumber)
                {
                    m_sErrorMessage = string.Format("RXTraceNumber Identical Check Error in {0} and {1}[Number1={2} Number2={3}]", sFileName,
                                                    m_cDataInfo_List[nDataIndex].m_sFileName,
                                                    cFileCheckInfo.m_nRXTraceNumber,
                                                    m_cDataInfo_List[nDataIndex].m_nRXTraceNumber);
                    return false;
                }

                if (m_cDataInfo_List[nDataIndex].m_nFWIP_Option != cFileCheckInfo.m_nFWIP_Option)
                {
                    m_sErrorMessage = string.Format("FWIP_Option Identical Check Error in {0} and {1}[Value1=0x{2} Value2=0x{3}]", sFileName,
                                                    m_cDataInfo_List[nDataIndex].m_sFileName,
                                                    cFileCheckInfo.m_nFWIP_Option.ToString("x4").ToUpper(),
                                                    m_cDataInfo_List[nDataIndex].m_nFWIP_Option.ToString("x4").ToUpper());
                    return false;
                }
            }

            return true;
        }

        private bool GetFrameData(ref List<int[,]> nFrameData_List, FileCheckInfo cFileCheckInfo, StreamReader srFile, string sFileName)
        {
            bool bGetFrameDataFlag = false;
            int[,] nSingleFrame_Array = null;
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
                        }
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

        private bool ComputeUniformityData(List<int[,]> nFrameData_List, int nListIndex, FileCheckInfo cFileCheckInfo, string sFileName)
        {
            int nSingleFrameMaxValue = 0;

            //List<double> dUniformity_List = new List<double>();
            double dSingleFrameUnformityMin = 0.0;
            double dSingleFrameUnformityMean = 0.0;

            for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
            {
                int nMaxValue = 0;
                int nMinValue = 0;

                for (int nTXIndex = 1; nTXIndex < cFileCheckInfo.m_nTXTraceNumber - 1; nTXIndex++)
                {
                    for (int nRXIndex = 1; nRXIndex < cFileCheckInfo.m_nRXTraceNumber - 1; nRXIndex++)
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
                    }
                }

                double dUniformity = Math.Round(((double)nMinValue / nMaxValue) * 100, 2, MidpointRounding.AwayFromZero);

                if (nFrameIndex == 0)
                    dSingleFrameUnformityMin = dUniformity;
                else
                {
                    if (dUniformity < dSingleFrameUnformityMin)
                        dSingleFrameUnformityMin = dUniformity;
                }

                dSingleFrameUnformityMean += dUniformity;
                //dUniformity_List.Add(dUniformity);

                if (nFrameIndex == 0)
                    nSingleFrameMaxValue = nMaxValue;
                else
                {
                    if (nMaxValue > nSingleFrameMaxValue)
                        nSingleFrameMaxValue = nMaxValue;
                }
            }

            dSingleFrameUnformityMean = Math.Round(dSingleFrameUnformityMean / nFrameData_List.Count, 2, MidpointRounding.AwayFromZero);

            m_cDataInfo_List[nListIndex].m_dSingleFrameUniformityMin = dSingleFrameUnformityMin;
            m_cDataInfo_List[nListIndex].m_dSingleFrameUniformityMean = dSingleFrameUnformityMean;

            int[,] nMeanFrame_Array = new int[cFileCheckInfo.m_nTXTraceNumber, cFileCheckInfo.m_nRXTraceNumber];

            for (int nTXIndex = 0; nTXIndex < cFileCheckInfo.m_nTXTraceNumber; nTXIndex++)
            {
                for (int nRXIndex = 0; nRXIndex < cFileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                {
                    int nSumValue = 0;

                    for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
                        nSumValue += nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];

                    int nMeanValue = (int)Math.Round((double)nSumValue / nFrameData_List.Count, 0, MidpointRounding.AwayFromZero);
                    nMeanFrame_Array[nTXIndex, nRXIndex] = nMeanValue;
                }
            }

            int nMeanFrameMaxValue = 0;
            int nMeanFrameMinValue = 0;

            for (int nTXIndex = 1; nTXIndex < cFileCheckInfo.m_nTXTraceNumber - 1; nTXIndex++)
            {
                for (int nRXIndex = 1; nRXIndex < cFileCheckInfo.m_nRXTraceNumber - 1; nRXIndex++)
                {
                    if (nTXIndex == 1 && nRXIndex == 1)
                    {
                        nMeanFrameMaxValue = nMeanFrame_Array[nTXIndex, nRXIndex];
                        nMeanFrameMinValue = nMeanFrame_Array[nTXIndex, nRXIndex];
                    }
                    else
                    {
                        if (nMeanFrame_Array[nTXIndex, nRXIndex] > nMeanFrameMaxValue)
                            nMeanFrameMaxValue = nMeanFrame_Array[nTXIndex, nRXIndex];

                        if (nMeanFrame_Array[nTXIndex, nRXIndex] < nMeanFrameMinValue)
                            nMeanFrameMinValue = nMeanFrame_Array[nTXIndex, nRXIndex];
                    }
                }
            }

            double dUnformityMean = Math.Round(((double)nMeanFrameMinValue / nMeanFrameMaxValue) * 100, 2, MidpointRounding.AwayFromZero);
            m_cDataInfo_List[nListIndex].m_dUniformityMean = dUnformityMean;

            /*
            double dUniformityStd = Math.Round(MathMethod.GetSD(dUniformity_List), 3, MidpointRounding.AwayFromZero);
            m_cDataInfo_List[nListIndex].m_dUniformityStd = dUniformityStd;
            */

            string sWarningMessage = "";
            string sMessage = "";

            if (Double.IsNaN(dUnformityMean) == true)
            {
                sWarningMessage = string.Format("Get Uniformity Value({0}) Error in Frequency={1:0.000}KHz", dUnformityMean, m_cDataInfo_List[nListIndex].m_dFrequency.ToString("0.000"));
                OutputMessage(string.Format("Warning : {0}", sWarningMessage), true);
                
                sMessage = string.Format("Get Uniformity Value({0}) Error", dUnformityMean);

                SkipFrequencyData cSkipFrequencyData = new SkipFrequencyData();
                cSkipFrequencyData.m_dFrequency = m_cDataInfo_List[nListIndex].m_dFrequency;
                cSkipFrequencyData.m_nPH1PH2Sum = m_cDataInfo_List[nListIndex].m_nPH1Value + m_cDataInfo_List[nListIndex].m_nPH2Value;
                cSkipFrequencyData.m_sMessage = sMessage;
                m_cSkipFrequencyData_List.Add(cSkipFrequencyData);
                m_bWarningOccurred = true;
            }
            else
            {
                if (dUnformityMean <= 0.0)
                {
                    sWarningMessage = string.Format("Get Negative Uniformity Value={0:0.00}, ADC Min Value={1}, ADC Max Value={2} Error in Frequency={3:0.000}KHz", dUnformityMean.ToString("0.00"), nMeanFrameMinValue, nMeanFrameMaxValue, m_cDataInfo_List[nListIndex].m_dFrequency.ToString("0.000"));
                    sMessage = string.Format("Get Negative Uniformity Value={0:0.00}, ADC Min Value={1}, ADC Max Value={2} Error", dUnformityMean.ToString("0.00"), nMeanFrameMinValue, nMeanFrameMaxValue);
                }
                else if (nMeanFrameMinValue < 0 && nMeanFrameMaxValue < 0)
                {
                    sWarningMessage = string.Format("Get Negative ADC Min Value({0}) and ADC Max Value({1}) Error in Frequency={2:0.000}KHz", nMeanFrameMinValue, nMeanFrameMaxValue, m_cDataInfo_List[nListIndex].m_dFrequency.ToString("0.000"));
                    sMessage = string.Format("Get Negative ADC Min Value({0}) and ADC Max Value({1}) Error", nMeanFrameMinValue, nMeanFrameMaxValue);
                }

                if (dUnformityMean <= 0.0 || (nMeanFrameMinValue < 0 && nMeanFrameMaxValue < 0))
                {
                    OutputMessage(string.Format("Warning : {0}", sWarningMessage), true);

                    SkipFrequencyData cSkipFrequencyData = new SkipFrequencyData();
                    cSkipFrequencyData.m_dFrequency = m_cDataInfo_List[nListIndex].m_dFrequency;
                    cSkipFrequencyData.m_nPH1PH2Sum = m_cDataInfo_List[nListIndex].m_nPH1Value + m_cDataInfo_List[nListIndex].m_nPH2Value;
                    cSkipFrequencyData.m_sMessage = sMessage;
                    m_cSkipFrequencyData_List.Add(cSkipFrequencyData);
                    m_bWarningOccurred = true;
                }
                else
                {
                    int nADCHighBoundary = (int)Math.Round(32767 * 0.95, 0, MidpointRounding.AwayFromZero);

                    if (nSingleFrameMaxValue >= nADCHighBoundary)
                    {
                        sWarningMessage = string.Format("Get ADC Value({0}) Over High Boundary({1}) Error in Frequency={2:0.000}KHz", nSingleFrameMaxValue, nADCHighBoundary, m_cDataInfo_List[nListIndex].m_dFrequency.ToString("0.000"));
                        OutputMessage(string.Format("Warning : {0}", sWarningMessage), true);

                        sMessage = string.Format("Get ADC Value({0}) Over High Boundary({1}) Error", nSingleFrameMaxValue, nADCHighBoundary);

                        SkipFrequencyData cSkipFrequencyData = new SkipFrequencyData();
                        cSkipFrequencyData.m_dFrequency = m_cDataInfo_List[nListIndex].m_dFrequency;
                        cSkipFrequencyData.m_nPH1PH2Sum = m_cDataInfo_List[nListIndex].m_nPH1Value + m_cDataInfo_List[nListIndex].m_nPH2Value;
                        cSkipFrequencyData.m_sMessage = sMessage;
                        m_cSkipFrequencyData_List.Add(cSkipFrequencyData);
                        m_bWarningOccurred = true;
                    }
                }
            }

            #region Just Test
            /*
            double dUniformityLB = 50.0;

            if (dUnformityMean < dUniformityLB)
            {
                sWarningMessage = string.Format("Get Uniformity Value({0}) Under Low Boundary({1}) Error in Frequency={2:0.000}KHz", dUnformityMean.ToString("0.00"), dUniformityLB.ToString("0.00"), m_cDataInfo_List[nListIndex].m_dFrequency.ToString("0.000"));
                OutputMessage(string.Format("Warning : {0}", sWarningMessage), true);

                sMessage = string.Format("Get Uniformity Value({0}) Under Low Boundary({1}) Error", dUnformityMean.ToString("0.00"), dUnformityMean.ToString("0.00"));

                SkipFrequencyData cSkipFrequencyData = new SkipFrequencyData();
                cSkipFrequencyData.m_dFrequency = m_cDataInfo_List[nListIndex].m_dFrequency;
                cSkipFrequencyData.m_nPH1PH2Sum = m_cDataInfo_List[nListIndex].m_nPH1Value + m_cDataInfo_List[nListIndex].m_nPH2Value;
                cSkipFrequencyData.m_sMessage = sMessage;
                m_cSkipFrequencyData_List.Add(cSkipFrequencyData);
                m_bWarningOccurred = true;
            }
            */
            #endregion

            return true;
        }

        private bool AnalysisData()
        {
            SetFrequencyAndUniformityData();

            if (SetAndCheckValidData() == false)
                return false;

            ComputeCurveFitCoefficient();

            ComputeUniformityCurveFit();

            ComputeSaturationRegionEdgePercentageLogFit();

            ComputeExtremeMaxValue();

            ComputeRecordRegionCurveFitMaxValue();

            ComputeRecordRegionRealMaxUniformity();

            ComputeTotalMaxUniformity();

            ComputeSaturationRegionUniformityLBAndMaxFrequencyInfo();

            return true;
        }

        private void SetFrequencyAndUniformityData()
        {
            m_cDataInfo_List.Sort(new DataInfoComparer());

            int nDataIndex = 0;

            while(true)
            {
                double dFrequency = nDataIndex * 10.0;

                if (dFrequency >= m_dRecordRegionFrequencyLB)
                    break;
                
                m_cTotalAndRecordData.m_dFrequencyData_List.Add(dFrequency);
                m_cTotalAndRecordData.m_dUniformityData_List.Add(0.0);

                nDataIndex++;
            }

            for (nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                double dFrequency = m_cDataInfo_List[nDataIndex].m_dFrequency;
                double dUniformity = m_cDataInfo_List[nDataIndex].m_dUniformityMean;

                if (dFrequency >= m_dRecordRegionFrequencyLB && dFrequency <= m_dRecordRegionFrequencyHB)
                {
                    m_cTotalAndRecordData.m_dFrequencyData_List.Add(dFrequency);
                    m_cTotalAndRecordData.m_dUniformityData_List.Add(dUniformity);
                }
            }
        }

        private bool SetAndCheckValidData()
        {
            for (int nDataIndex = m_cTotalAndRecordData.m_dFrequencyData_List.Count - 1; nDataIndex >= 0; nDataIndex--)
            {
                double dFrequency = m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex];
                double dUniformity = m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex];

                if ((dFrequency >= m_dRecordRegionFrequencyLB && dFrequency <= m_dRecordRegionFrequencyHB) &&
                    dUniformity < m_dUniformityLeastLB)
                {
                    m_cTotalAndRecordData.m_dFrequencyData_List.RemoveAt(nDataIndex);
                    m_cTotalAndRecordData.m_dUniformityData_List.RemoveAt(nDataIndex);
                }
            }

            if (m_cTotalAndRecordData.m_dUniformityData_List.Count < m_nUniformityDataCountLB)
            {
                m_sErrorMessage = string.Format("No Enough Valid Data[Valid:{0} LB:{1}]", m_cTotalAndRecordData.m_dUniformityData_List.Count, m_nUniformityDataCountLB);
                return false;
            }

            return true;
        }

        private void ComputeCurveFitCoefficient()
        {
            List<double> dFrequencyData_List = new List<double>();
            List<double> dUniformityData_List = new List<double>();

            for (int nDataIndex = 0; nDataIndex < m_cTotalAndRecordData.m_dFrequencyData_List.Count; nDataIndex++)
            {
                double dFrequency = m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex];
                double dUniformity = m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex];

                if (dFrequency >= m_dRecordRegionFrequencyLB && dFrequency <= m_dRecordRegionFrequencyHB)
                {
                    dFrequencyData_List.Add(dFrequency);
                    dUniformityData_List.Add(dUniformity);
                }
            }

            double[] dCoefficient_Array = PolyFit(dFrequencyData_List.ToArray(), dUniformityData_List.ToArray(), 3);

            if (dCoefficient_Array[0] > 100.0)
            {
                dFrequencyData_List.Add(0.0);
                dUniformityData_List.Add(100.0);

                dCoefficient_Array = PolyFit(dFrequencyData_List.ToArray(), dUniformityData_List.ToArray(), 3);
            }

            m_cCurveFitData.m_dCoefficient_Array = dCoefficient_Array;
        }

        /// <summary>
        /// 計算nOrder階多項式係數
        /// </summary>
        /// <param name="dX_Array"></param>
        /// <param name="dY_Array"></param>
        /// <param name="nOrder"></param>
        /// <returns></returns>
        private double[] PolyFit(double[] dX_Array, double[] dY_Array, int nOrder)
        {
            double[] dCoefficient_Array = Fit.Polynomial(dX_Array, dY_Array, nOrder);

            return dCoefficient_Array;
        }

        /// <summary>
        /// 計算均勻度(Uniformity)的三次擬合曲線(0~250KHz)
        /// </summary>
        private void ComputeUniformityCurveFit()
        {
            double[] dUniformityCurveFit_Array = ComputeCurveFitArray(m_cTotalAndRecordData.m_dFrequencyData_List.ToArray(), m_cCurveFitData.m_dCoefficient_Array);

            m_cCurveFitData.m_dUniformityData_List = dUniformityCurveFit_Array.ToList();
        }

        /// <summary>
        /// 計算頻率陣列的擬合曲線數值(均勻度擬合曲線數值(UniformityCurveFit))
        /// </summary>
        /// <param name="dFrequency_Array"></param>
        /// <param name="dCoefficient_Array"></param>
        /// <returns></returns>
        private double[] ComputeCurveFitArray(double[] dFrequency_Array, double[] dCoefficient_Array)
        {
            double[] dCurveFit_Array = new double[dFrequency_Array.Length];

            for (int nFreqencyIndex = 0; nFreqencyIndex < dFrequency_Array.Length; nFreqencyIndex++)
            {
                double dPolyFitValue = ComputePolyFitValue(dFrequency_Array[nFreqencyIndex], dCoefficient_Array);
                dCurveFit_Array[nFreqencyIndex] = dPolyFitValue;
            }

            return dCurveFit_Array;
        }

        /// <summary>
        /// 計算多項式方程式數值
        /// </summary>
        /// <param name="dFreuqnecyValue"></param>
        /// <param name="dCoefficient_Array"></param>
        /// <returns></returns>
        private double ComputePolyFitValue(double dFreuqnecyValue, double[] dCoefficient_Array)
        {
            double dFitValue = 0.0;

            for (int nValueIndex = 0; nValueIndex < dCoefficient_Array.Length; nValueIndex++)
                dFitValue += dCoefficient_Array[nValueIndex] * Math.Pow(dFreuqnecyValue, nValueIndex);

            return Math.Round(dFitValue, 2, MidpointRounding.AwayFromZero);
        }

        private void ComputeSaturationRegionEdgePercentageLogFit()
        {
            double[] dEstimateMaxUniformity_Array = new double[5] { 100, 89.74, 92.65, 90.64, 89.47 };
            double[] dRecordRegionCurveFitMaxUniformity_Array = new double[5] { 74.25, 89.74, 92.57, 90.64, 88.08 };
            double[] dEstimateSaturationRegionUniformityLB_Array = new double[5] { 71.5, 73, 73, 73, 73 };

            double[] dRatio_EstimateMaxToRecordRegionCurveFitMax_Array = new double[5];
            double[] dRatio_EstimateSaturationRegionLBToRecordRegionCurveFitMax_Array = new double[5];

            for (int nDataIndex = 0; nDataIndex < dEstimateMaxUniformity_Array.Length; nDataIndex++)
            {
                double dRatio_EstimateMaxToRecordRegionCurveFitMax = dEstimateMaxUniformity_Array[nDataIndex] / dRecordRegionCurveFitMaxUniformity_Array[nDataIndex];
                double dRatio_EstimateSaturationRegionLBToRecordRegionCurveFitMax = dEstimateSaturationRegionUniformityLB_Array[nDataIndex] / dRecordRegionCurveFitMaxUniformity_Array[nDataIndex];

                dRatio_EstimateMaxToRecordRegionCurveFitMax_Array[nDataIndex] = dRatio_EstimateMaxToRecordRegionCurveFitMax - m_dShiftValue;
                dRatio_EstimateSaturationRegionLBToRecordRegionCurveFitMax_Array[nDataIndex] = dRatio_EstimateSaturationRegionLBToRecordRegionCurveFitMax;
            }

            double[] dLogFitCoefficient_Array = LogarithmFit(dRatio_EstimateMaxToRecordRegionCurveFitMax_Array, dRatio_EstimateSaturationRegionLBToRecordRegionCurveFitMax_Array);
            //double[] dPolyFitCoefficient_Array = PolyFit(dRatio_EstimateMaxToRecordRegionCurveFitMax_Array, dRatio_EstimateSaturationRegionLBToRecordRegionCurveFitMax_Array, 2);

            m_cSatnRegEdgePctLogFitData.m_dLogFitCoefficient_Array = dLogFitCoefficient_Array;

            #region Compute Saturation Region Edge Percentage Log Fit List Data
            double dXValue = 0.0;

            while (dXValue < 2.0)
            {
                dXValue = dXValue + 0.001;
                dXValue = Math.Round(dXValue, 3);
                m_cSatnRegEdgePctLogFitData.m_dLogFitRatioValue_X_List.Add(dXValue);
            }

            for (int nValueIndex = 0; nValueIndex < m_cSatnRegEdgePctLogFitData.m_dLogFitRatioValue_X_List.Count; nValueIndex++)
            {
                double dYValue = ComputeLogFitValue(m_cSatnRegEdgePctLogFitData.m_dLogFitRatioValue_X_List[nValueIndex], m_cSatnRegEdgePctLogFitData.m_dLogFitCoefficient_Array);
                m_cSatnRegEdgePctLogFitData.m_dLogFitRatioValue_Y_List.Add(dYValue);
            }
            #endregion
        }

        /// <summary>
        /// Least-Squares fitting the points (x,y) to a logarithm y : x -> a + b*ln(x),
        /// returning a function y' for the best fitting line.
        /// </summary>
        private double[] LogarithmFit(double[] dX_Array, double[] dY_Array)
        {
            var vParameterValue = Fit.Logarithm(dX_Array, dY_Array);
            var vAValue = vParameterValue.Item1;
            var vBValue = vParameterValue.Item2;
            double[] dCoefficient_Array = new double[] { vAValue, vBValue };

            return dCoefficient_Array;
        }

        /// <summary>
        /// 計算對數曲線的數值
        /// </summary>
        /// <param name="dRatioValue"></param>
        /// <param name="dCoefficient_Array"></param>
        /// <returns></returns>
        private double ComputeLogFitValue(double dRatioValue, double[] dCoefficient_Array)
        {
            double dFitValue = 0.0;

            dRatioValue = dRatioValue - m_dShiftValue;

            dFitValue = dCoefficient_Array[0] + dCoefficient_Array[1] * Math.Log(dRatioValue);

            return Math.Ceiling(dFitValue * 100) / 100;
        }

        private void ComputeExtremeMaxValue()
        {
            double dExtremeMaxUniformity = 0.0;
            double dExtremeMaxFrequency = 0.0;

            double[] dCoefficient_Array = m_cCurveFitData.m_dCoefficient_Array;

            if (dCoefficient_Array.Length == 4)
            {
                /*
                double dA = dCoefficient_Array[3] * 3;
                double dB = dCoefficient_Array[2] * 2;
                double dC = dCoefficient_Array[1] * 1;

                double dFirstSolutionValue = ((-1 * dB) + Math.Sqrt((dB * dB) - (4 * dA * dC))) / (2 * dA);
                double dSecondSolutionValue = ((-1 * dB) - Math.Sqrt((dB * dB) - (4 * dA * dC))) / (2 * dA);
                */

                double dA = dCoefficient_Array[3];
                double dB = dCoefficient_Array[2];
                double dC = dCoefficient_Array[1];

                double dFirstSolutionValue = ((-1 * dB) + Math.Sqrt((dB * dB) - (3 * dA * dC))) / (3 * dA);
                double dSecondSolutionValue = ((-1 * dB) - Math.Sqrt((dB * dB) - (3 * dA * dC))) / (3 * dA);

                double dMaxFrequency = m_cTotalAndRecordData.m_dFrequencyData_List.Max();

                double dFirstValue = 0.0;
                double dSecondValue = 0.0;

                #region New Version : Fix for Mistaken to Get Minima Value to Calculate Extreme Max Value 2023.09.07
                double dD = dCoefficient_Array[0];
                double dTheta = 4 * ((dB * dB) - (3 * dA * dC));

                double dMaxValue = m_cCurveFitData.m_dUniformityData_List.Max();
                int nMaxValueIndex = m_cCurveFitData.m_dUniformityData_List.FindIndex(x => x == dMaxValue);
                double dMaxValueFreq = m_cTotalAndRecordData.m_dFrequencyData_List[nMaxValueIndex];

                if ((dA > 0 && dTheta <= 0) || (dA < 0 && dTheta <= 0))
                {
                    dExtremeMaxUniformity = Math.Round(dMaxValue, 3, MidpointRounding.AwayFromZero);
                    dExtremeMaxFrequency = dMaxValueFreq;
                }
                else
                {
                    dFirstValue = ComputePolyFitValue(dFirstSolutionValue, dCoefficient_Array);
                    dSecondValue = ComputePolyFitValue(dSecondSolutionValue, dCoefficient_Array);

                    if (dFirstValue > dSecondValue)
                    {
                        if (dFirstSolutionValue >= 0 && dFirstSolutionValue <= dMaxFrequency)
                        {
                            dExtremeMaxUniformity = dFirstValue;
                            dExtremeMaxUniformity = Math.Round(dExtremeMaxUniformity, 3, MidpointRounding.AwayFromZero);
                            dExtremeMaxFrequency = Math.Round(dFirstSolutionValue, 3, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            dExtremeMaxUniformity = Math.Round(dMaxValue, 3, MidpointRounding.AwayFromZero);
                            dExtremeMaxFrequency = dMaxValueFreq;
                        }
                    }
                    else
                    {
                        if (dSecondSolutionValue >= 0 && dSecondSolutionValue <= dMaxFrequency)
                        {
                            dExtremeMaxUniformity = dSecondValue;
                            dExtremeMaxUniformity = Math.Round(dExtremeMaxUniformity, 3, MidpointRounding.AwayFromZero);
                            dExtremeMaxFrequency = Math.Round(dSecondSolutionValue, 3, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            dExtremeMaxUniformity = Math.Round(dMaxValue, 3, MidpointRounding.AwayFromZero);
                            dExtremeMaxFrequency = dMaxValueFreq;
                        }
                    }
                }
                #endregion

                #region Old Version : Mistaken to Get Minima Value to Calculate Extreme Max Value 2023.09.07
                /*
                if (dFirstSolutionValue >= 0 && dFirstSolutionValue <= dMaxFrequency)
                {
                    dFirstValue = ComputePolyFitValue(dFirstSolutionValue, dCoefficient_Array);

                    if (dFirstValue < 0.0)
                        dFirstValue = 0.0;
                }

                if (dSecondSolutionValue >= 0 && dSecondSolutionValue <= dMaxFrequency)
                {
                    dSecondValue = ComputePolyFitValue(dSecondSolutionValue, dCoefficient_Array);

                    if (dSecondValue < 0.0)
                        dSecondValue = 0.0;
                }

                if (dFirstValue > 0.0 || dSecondValue > 0.0)
                {
                    dExtremeMaxUniformity = (dFirstValue > dSecondValue) ? dFirstValue : dSecondValue;
                    dExtremeMaxUniformity = Math.Round(dExtremeMaxUniformity, 3, MidpointRounding.AwayFromZero);

                    if (dFirstValue > dSecondValue)
                        dExtremeMaxFrequency = Math.Round(dFirstSolutionValue, 3, MidpointRounding.AwayFromZero);
                    else
                        dExtremeMaxFrequency = Math.Round(dSecondSolutionValue, 3, MidpointRounding.AwayFromZero);

                }
                else
                {
                    dExtremeMaxUniformity = Math.Round(dCoefficient_Array[0], 3, MidpointRounding.AwayFromZero);
                    dExtremeMaxFrequency = 0.0;
                }
                */
                #endregion

                if (dExtremeMaxUniformity > 100.0)
                    dExtremeMaxUniformity = 100.0;
            }
            else if (dCoefficient_Array.Length == 3)
            {
                double dA = dCoefficient_Array[2];
                double dB = dCoefficient_Array[1];

                double dSolutionValue = (-1 * dB) / (2 * dA);

                double dMaxFrequency = m_cTotalAndRecordData.m_dFrequencyData_List.Max();

                double dValue = 0.0;

                if (dSolutionValue >= 0 && dSolutionValue <= dMaxFrequency)
                {
                    dValue = ComputePolyFitValue(dSolutionValue, dCoefficient_Array);

                    if (dValue < 0.0)
                        dValue = 0.0;
                }

                if (dValue > 0.0)
                {
                    dExtremeMaxUniformity = Math.Round(dValue, 3, MidpointRounding.AwayFromZero);
                    dExtremeMaxFrequency = Math.Round(dSolutionValue, 3, MidpointRounding.AwayFromZero);
                }
                else
                {
                    dExtremeMaxUniformity = Math.Round(dCoefficient_Array[0], 3, MidpointRounding.AwayFromZero);
                    dExtremeMaxFrequency = 0.0;
                }

                if (dExtremeMaxUniformity > 100.0)
                    dExtremeMaxUniformity = 100.0;
            }

            m_cResultData.m_dExtremeMaxUniformity = dExtremeMaxUniformity;
            m_cResultData.m_dExtremeMaxFrequency = dExtremeMaxFrequency;
        }

        /// <summary>
        /// 利用3階Uniformity擬合曲線方程式計算80KHz以上頻率之最大參考值
        /// </summary>
        private void ComputeRecordRegionCurveFitMaxValue()
        {
            double dCurveFitMaxUniformity = 0.0;
            double dCurveFitMaxFrequency = 0.0;
            bool bGetMaxValueFlag = false;

            for (int nDataIndex = 0; nDataIndex < m_cTotalAndRecordData.m_dFrequencyData_List.Count; nDataIndex++)
            {
                double dFrequency = m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex];

                if (bGetMaxValueFlag == false)
                {
                    if (dFrequency >= m_dRecordRegionFrequencyLB)
                    {
                        dCurveFitMaxUniformity = m_cCurveFitData.m_dUniformityData_List[nDataIndex];
                        dCurveFitMaxFrequency = dFrequency;
                        bGetMaxValueFlag = true;
                    }
                }
                else
                {
                    if (dFrequency >= m_dRecordRegionFrequencyLB)
                    {
                        if (m_cCurveFitData.m_dUniformityData_List[nDataIndex] > dCurveFitMaxUniformity)
                        {
                            dCurveFitMaxUniformity = m_cCurveFitData.m_dUniformityData_List[nDataIndex];
                            dCurveFitMaxFrequency = dFrequency;
                        }
                    }
                }
            }

            dCurveFitMaxUniformity = Math.Round(dCurveFitMaxUniformity, 3, MidpointRounding.AwayFromZero);

            m_cResultData.m_dRecordRegionCurveFitMaxUniformity = dCurveFitMaxUniformity;
            m_cResultData.m_dRecordRegionCurveFitMaxFrequency = dCurveFitMaxFrequency;

            if (m_cResultData.m_dExtremeMaxUniformity <= dCurveFitMaxUniformity)
            {
                m_cResultData.m_dExtremeMaxUniformity = dCurveFitMaxUniformity;
                m_cResultData.m_dExtremeMaxFrequency = dCurveFitMaxFrequency;
            }
        }

        private void ComputeRecordRegionRealMaxUniformity()
        {
            double dMaxUniformity = Math.Round(m_cTotalAndRecordData.m_dUniformityData_List.Max(), 3, MidpointRounding.AwayFromZero);
            m_cResultData.m_dRecordRegionRealMaxUniformity = dMaxUniformity;
        }

        private void ComputeTotalMaxUniformity()
        {
            double dMaxUniformity = 0.0;

            if (m_cResultData.m_dExtremeMaxUniformity > m_cResultData.m_dRecordRegionCurveFitMaxUniformity)
                dMaxUniformity = m_cResultData.m_dExtremeMaxUniformity;
            else
                dMaxUniformity = m_cResultData.m_dRecordRegionCurveFitMaxUniformity;

            if (m_cResultData.m_dRecordRegionRealMaxUniformity > dMaxUniformity)
                dMaxUniformity = m_cResultData.m_dRecordRegionRealMaxUniformity;

            if (dMaxUniformity > 100.0)
                dMaxUniformity = 100.0;

            m_cResultData.m_dTotalMaxUniformity = dMaxUniformity;
        }

        private void ComputeSaturationRegionUniformityLBAndMaxFrequencyInfo()
        {
            if (ParamFingerAutoTuning.m_dFRPH1UniformityLB < 0.0)
            {
                double dExtremeMaxToRecordRegionCurveFitMaxRatio = m_cResultData.m_dExtremeMaxUniformity / m_cResultData.m_dRecordRegionCurveFitMaxUniformity;
                m_cResultData.m_dSaturationRatio = ComputeLogFitIntValue(dExtremeMaxToRecordRegionCurveFitMaxRatio, m_cSatnRegEdgePctLogFitData.m_dLogFitCoefficient_Array);

                double dSaturationUniformityLB = Math.Round(m_cResultData.m_dRecordRegionCurveFitMaxUniformity * m_cResultData.m_dSaturationRatio, 3, MidpointRounding.AwayFromZero);

                if (ParamFingerAutoTuning.m_dFRPH1LimitHB < 0.0)
                    m_cResultData.m_dSaturationUniformityLB = dSaturationUniformityLB;
                else
                {
                    if (dSaturationUniformityLB > ParamFingerAutoTuning.m_dFRPH1LimitHB)
                        m_cResultData.m_dSaturationUniformityLB = ParamFingerAutoTuning.m_dFRPH1LimitHB;
                    else
                        m_cResultData.m_dSaturationUniformityLB = dSaturationUniformityLB;
                }
            }
            else
                m_cResultData.m_dSaturationUniformityLB = ParamFingerAutoTuning.m_dFRPH1UniformityLB;

            /*
            if (m_cResultData.m_dSaturationUniformityLB < dSaturationUniformityLB)
                m_cResultData.m_dSaturationUniformityLB = dSaturationUniformityLB;
            */

            double dSaturationMaxFrequency = 0.0;

            if (ParamFingerAutoTuning.m_dFRPH1FitFrequencyLB < 0.0)
            {
                for (int nDataIndex = 0; nDataIndex < m_cTotalAndRecordData.m_dFrequencyData_List.Count; nDataIndex++)
                {
                    double dFrequency = m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex];

                    if (dFrequency < m_dRecordRegionFrequencyLB)
                        continue;

                    if (m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex] >= m_cResultData.m_dSaturationUniformityLB)
                    {
                        if (dFrequency > dSaturationMaxFrequency)
                            dSaturationMaxFrequency = dFrequency;
                    }
                }
            }
            else
            {
                double dMinUniformity = 0.0;
                bool bGetMinUniformityFlag = false;

                for (int nDataIndex = 0; nDataIndex < m_cTotalAndRecordData.m_dFrequencyData_List.Count; nDataIndex++)
                {
                    double dFrequency = m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex];

                    if (dFrequency < m_dRecordRegionFrequencyLB)
                        continue;

                    if (dFrequency <= ParamFingerAutoTuning.m_dFRPH1FitFrequencyLB)
                    {
                        if (dFrequency > dSaturationMaxFrequency)
                        {
                            dSaturationMaxFrequency = dFrequency;

                            if (bGetMinUniformityFlag == false)
                            {
                                dMinUniformity = m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex];
                                bGetMinUniformityFlag = true;
                            }
                            else
                            {
                                if (m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex] < dMinUniformity)
                                    dMinUniformity = m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex];
                            }
                        }
                    }
                    else
                    {
                        if (m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex] >= m_cResultData.m_dSaturationUniformityLB)
                        {
                            if (dFrequency > dSaturationMaxFrequency)
                                dSaturationMaxFrequency = dFrequency;
                        }
                    }
                }

                if (dSaturationMaxFrequency <= ParamFingerAutoTuning.m_dFRPH1FitFrequencyLB)
                    m_cResultData.m_dSaturationUniformityLB = dMinUniformity;
            }

            for (int nDataIndex = 0; nDataIndex < m_cTotalAndRecordData.m_dFrequencyData_List.Count; nDataIndex++)
            {
                double dFrequency = m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex];

                if (dFrequency < m_dRecordRegionFrequencyLB)
                    continue;

                if (dFrequency == dSaturationMaxFrequency)
                {
                    if (nDataIndex < m_cTotalAndRecordData.m_dFrequencyData_List.Count - 1)
                    {
                        int nCurrentPH2 = ElanConvert.Convert2PH2(dFrequency, m_nFixedPH1);
                        int nNextPH2 = ElanConvert.Convert2PH2(m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex + 1], m_nFixedPH1);

                        if ((nCurrentPH2 - nNextPH2) > 1)
                        {
                            int nPartNumber = nCurrentPH2 - nNextPH2;

                            for (int nPartIndex = 1; nPartIndex < nPartNumber; nPartIndex++)
                            {
                                double dFreqency = ElanConvert.Convert2Frequency(m_nFixedPH1, (nCurrentPH2 - nPartIndex));

                                double dCurrentUniformity = m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex];
                                double dNextUniformity = m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex + 1];

                                double dCurrentFrequency = dFrequency;
                                double dNextFrequency = m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex + 1];

                                double fEstimateUniformityValue = dCurrentUniformity - (((dCurrentUniformity - dNextUniformity) * (dCurrentFrequency - dFreqency)) / (dCurrentFrequency - dNextFrequency));

                                if (fEstimateUniformityValue >= m_cResultData.m_dSaturationUniformityLB)
                                    dSaturationMaxFrequency = dFreqency;
                            }
                        }
                    }
                }
            }

            m_cResultData.m_dSaturationMaxFrequency = dSaturationMaxFrequency;
            m_cResultData.m_nSaturationMinPH2 = ElanConvert.Convert2PH2(dSaturationMaxFrequency, m_nFixedPH1);
        }

        private double ComputeLogFitIntValue(double dRatioValue, double[] dCoefficient_Array)
        {
            double dFitValue = 0.0;

            dRatioValue = dRatioValue - m_dShiftValue;

            dFitValue = dCoefficient_Array[0] + dCoefficient_Array[1] * Math.Log(dRatioValue);

            return Math.Ceiling(dFitValue * 100) / 100;
        }

        private bool SaveAnalysisFile(bool bAnalysisErrorFlag)
        {
            bool bErrorFlag = false;
            string sAnalysisFilePath = string.Format(@"{0}\Analysis.csv", m_sLogDirectoryPath);

            FileStream fs = new FileStream(sAnalysisFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETYPE_ANALYSIS);

                if (bAnalysisErrorFlag == false)
                {
                    sw.WriteLine("CurveFit Coefficient");

                    for (int nValueIndex = 0; nValueIndex < m_cCurveFitData.m_dCoefficient_Array.Length; nValueIndex++)
                    {
                        if (nValueIndex < m_cCurveFitData.m_dCoefficient_Array.Length - 1)
                            sw.Write(string.Format("X^{0},", nValueIndex));
                        else
                            sw.WriteLine(string.Format("X^{0}", nValueIndex));
                    }

                    for (int nValueIndex = 0; nValueIndex < m_cCurveFitData.m_dCoefficient_Array.Length; nValueIndex++)
                    {
                        if (nValueIndex < m_cCurveFitData.m_dCoefficient_Array.Length - 1)
                            sw.Write(string.Format("{0},", m_cCurveFitData.m_dCoefficient_Array[nValueIndex]));
                        else
                            sw.WriteLine(m_cCurveFitData.m_dCoefficient_Array[nValueIndex]);
                    }

                    sw.WriteLine();

                    sw.WriteLine("Extreme Max Vlaue,Extreme Max Frequency,CurveFit Max Value,CurveFit Max Frequency,Reality Max Value");

                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4}",
                                               m_cResultData.m_dExtremeMaxUniformity,
                                               m_cResultData.m_dExtremeMaxFrequency,
                                               m_cResultData.m_dRecordRegionCurveFitMaxUniformity,
                                               m_cResultData.m_dRecordRegionCurveFitMaxFrequency,
                                               m_cResultData.m_dRecordRegionRealMaxUniformity));

                    sw.WriteLine();

                    sw.WriteLine("Saturation Region Percentage");

                    sw.WriteLine(string.Format("{0}", m_cResultData.m_dSaturationRatio * 100));

                    sw.WriteLine();

                    sw.WriteLine("Saturation Region Uniformity LB");

                    sw.WriteLine(m_cResultData.m_dSaturationUniformityLB);

                    sw.WriteLine();

                    sw.WriteLine("Saturation Region Max Frequneny,PH1,PH2");

                    sw.WriteLine(string.Format("{0},0x{1},0x{2}",
                                               m_cResultData.m_dSaturationMaxFrequency, 
                                               m_nFixedPH1.ToString("x2").ToUpper(),
                                               m_cResultData.m_nSaturationMinPH2.ToString("x2").ToUpper()));

                    sw.WriteLine();

                    sw.WriteLine("Curve Data");

                    sw.WriteLine("Frequency,UniformityValue,FitValue");

                    for (int nDataIndex = 0; nDataIndex < m_cTotalAndRecordData.m_dFrequencyData_List.Count; nDataIndex++)
                    {
                        if (m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex] < m_dRecordRegionFrequencyLB)
                        {
                            sw.WriteLine(string.Format("{0},,{1}",
                                                       m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex],
                                                       m_cCurveFitData.m_dUniformityData_List[nDataIndex]));
                        }
                        else
                        {
                            sw.WriteLine(string.Format("{0},{1},{2}",
                                                       m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex],
                                                       m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex],
                                                       m_cCurveFitData.m_dUniformityData_List[nDataIndex]));
                        }
                    }
                }
                else
                {
                    if (m_sErrorMessage.IndexOf("No Enough Valid Data") >= 0)
                    {
                        sw.WriteLine("Curve Data");

                        sw.WriteLine("Frequency,UniformityValue");

                        for (int nDataIndex = 0; nDataIndex < m_cTotalAndRecordData.m_dFrequencyData_List.Count; nDataIndex++)
                        {
                            if (m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex] < m_dRecordRegionFrequencyLB)
                            {
                                sw.WriteLine(string.Format("{0},", m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex]));
                            }
                            else
                            {
                                sw.WriteLine(string.Format("{0},{1}",
                                                           m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex],
                                                           m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex]));
                            }
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

            if (bErrorFlag == true || bAnalysisErrorFlag == true)
                return false;
            else
                return true;
        }

        private bool SaveUnifomrityChartFile()
        {
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            if (m_bGenerateH5Data == true)
            {
                string sH5DirectoryPath = string.Format(@"{0}\{1}", m_sH5LogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART);

                if (Directory.Exists(sH5DirectoryPath) == false)
                    Directory.CreateDirectory(sH5DirectoryPath);
            }

            List<double> dFrequencyData_List = new List<double>();
            List<double> dUniformityData_List = new List<double>();

            for (int nDataIndex = 0; nDataIndex < m_cTotalAndRecordData.m_dFrequencyData_List.Count; nDataIndex++)
            {
                double dFrequency = m_cTotalAndRecordData.m_dFrequencyData_List[nDataIndex];

                if (dFrequency >= m_dRecordRegionFrequencyLB)
                {
                    dFrequencyData_List.Add(dFrequency);
                    dUniformityData_List.Add(m_cTotalAndRecordData.m_dUniformityData_List[nDataIndex]);
                }
            }

            List<double> dFitFrequencyData_List = new List<double>();
            dFitFrequencyData_List = m_cTotalAndRecordData.m_dFrequencyData_List;
            List<double> dFitUniformityData_List = new List<double>();
            dFitUniformityData_List = m_cCurveFitData.m_dUniformityData_List;

            bool bCompleteFlag = SaveChart(dFrequencyData_List, 
                                           dUniformityData_List, 
                                           dFitFrequencyData_List, 
                                           dFitUniformityData_List,
                                           m_cResultData.m_dSaturationMaxFrequency, 
                                           m_cResultData.m_dSaturationUniformityLB, 
                                           sDirectoryPath);

            return true;
        }

        private bool SaveChart(List<double> dFrequencyData_List, List<double> dUniformityData_List, List<double> dFitFrequencyData_List, List<double> dFitUniformityData_List, 
                               double dMaxFrequency, double dUniformityLB, string sFolderPath)
        {
            bool bError = false;
            string sTitleName = "ADC Uniformity Chart";
            string sFilePath = string.Format(@"{0}\UniformityChart.jpg", sFolderPath);

            int nYMax = 100;
            int nYMin = (int)(dUniformityData_List.Min() / 10) * 10;

            if (nYMin > dUniformityLB)
                nYMin = (int)(dUniformityLB / 10) * 10;

            int nYInterval = 10;

            double dFrequencyData_List_Max = dFrequencyData_List.Max();

            if (dFrequencyData_List_Max % 10 > 0.0)
                dFrequencyData_List_Max = dFrequencyData_List_Max + 10.0;

            if (m_dRecordRegionFrequencyHB > dFrequencyData_List_Max)
                dFrequencyData_List_Max = m_dRecordRegionFrequencyHB;

            int nXMax = (int)(dFrequencyData_List_Max / 10) * 10;
            int nXMin = (int)(dFrequencyData_List.Min() / 10) * 10; //0;
            int nXInterval = 10;

            Chart cChart = new Chart();
            var vChartArea = new ChartArea();
            cChart.ChartAreas.Add(vChartArea);
            cChart.Width = 1500;
            cChart.Height = 800;
            cChart.Legends.Add("Legend");
            cChart.Legends["Legend"].Font = new Font("Times New Roman", 10);
            cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
            cChart.Titles.Add(sTitleName);
            cChart.Titles[0].Font = new Font("Times New Roman", 18);
            Series cSeries = new Series("ADC Uniformity");
            cSeries.ChartType = SeriesChartType.Line;
            cSeries.Color = Color.Blue;
            cSeries.BorderColor = Color.Black;
            cSeries.MarkerStyle = MarkerStyle.Circle;
            cSeries.BorderWidth = 2;
            cSeries.MarkerSize = 5;
            cSeries.CustomProperties = "PointWidth=1";
            cChart.ChartAreas[0].AxisY.Maximum = nYMax;
            cChart.ChartAreas[0].AxisY.Minimum = nYMin;
            cChart.ChartAreas[0].AxisY.Interval = nYInterval;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.ChartAreas[0].AxisY.Title = "Uniformity(%)";
            cChart.ChartAreas[0].AxisY.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Times New Roman", 11);
            cChart.ChartAreas[0].AxisX.Maximum = nXMax;
            cChart.ChartAreas[0].AxisX.Minimum = nXMin;
            cChart.ChartAreas[0].AxisX.Interval = nXInterval;
            //cChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cChart.ChartAreas[0].AxisX.Title = "Frequency(KHz)";
            cChart.ChartAreas[0].AxisX.TitleFont = new Font("Times New Roman", 12);
            cChart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Times New Roman", 10);
            cSeries.IsValueShownAsLabel = false;
            cChart.ChartAreas[0].AxisX.IsLabelAutoFit = false;
            cChart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;

            for (int nDataIndex = 0; nDataIndex < dUniformityData_List.Count; nDataIndex++)
            {
                var vLine = cSeries.Points[cSeries.Points.AddXY(dFrequencyData_List[nDataIndex], dUniformityData_List[nDataIndex])];

                if (dFrequencyData_List[nDataIndex] <= dMaxFrequency)
                    vLine.MarkerColor = Color.Red;
                else
                    vLine.MarkerColor = Color.Black;

            }
            cChart.Series.Add(cSeries);

            /*
            Series cCurveFitSeries = new Series("Curve Fit");
            cCurveFitSeries.ChartType = SeriesChartType.Line;
            cCurveFitSeries.Color = Color.Red;
            cCurveFitSeries.BorderColor = Color.Black;
            //cCurveFitSeries.MarkerStyle = MarkerStyle.Circle;
            cCurveFitSeries.BorderWidth = 2;
            //cCurveFitSeries.MarkerSize = 5;
            cCurveFitSeries.CustomProperties = "PointWidth=1";
            for (int nDataIndex = 0; nDataIndex < listFitUniformityData.Count; nDataIndex++)
                cCurveFitSeries.Points.AddXY(dFitFrequencyData_List[nDataIndex], dFitUniformityData_List[nDataIndex]);
            cChart.Series.Add(cCurveFitSeries);
            */

            string sSaturationMaxFrequencyLine = string.Format("Saturation Region MaxFrequency({0}KHz)", dMaxFrequency);
            cChart.Series.Add(sSaturationMaxFrequencyLine);
            cChart.Series[sSaturationMaxFrequencyLine].Points.Add(new DataPoint(dMaxFrequency, 0));
            cChart.Series[sSaturationMaxFrequencyLine].Points.Add(new DataPoint(dMaxFrequency, 100));
            cChart.Series[sSaturationMaxFrequencyLine].ChartType = SeriesChartType.Line;
            cChart.Series[sSaturationMaxFrequencyLine].BorderWidth = 2;
            cChart.Series[sSaturationMaxFrequencyLine].Color = Color.Green;

            string sSaturationUniformityLBLine = string.Format("Saturation Region UnformityLB({0}%)", dUniformityLB);
            cChart.Series.Add(sSaturationUniformityLBLine);
            cChart.Series[sSaturationUniformityLBLine].Points.Add(new DataPoint(nXMin, dUniformityLB));
            cChart.Series[sSaturationUniformityLBLine].Points.Add(new DataPoint(nXMax, dUniformityLB));
            cChart.Series[sSaturationUniformityLBLine].ChartType = SeriesChartType.Line;
            cChart.Series[sSaturationUniformityLBLine].BorderWidth = 2;
            cChart.Series[sSaturationUniformityLBLine].Color = Color.Orange;

            try
            {
                cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
            }
            catch
            {
                m_sErrorMessage = "Save Chart File Error";
                bError = true;
            }

            if (bError == true)
                return false;
            else
                return true;
        }

        private bool SaveReportFile()
        {
            int nTXnBitValue = m_cDataInfo_List[0].m_nFWIP_Option & 0x0100;

            m_bEnableTXn = (nTXnBitValue > 0) ? true : false;

            bool bError = false;
            string sReportFilePath = string.Format(@"{0}\Report.csv", m_sLogDirectoryPath);

            FileStream fs = new FileStream(sReportFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETYPE_REPORT);

                // sw.WriteLine("Option Record:");
                sw.WriteLine("Option Record");

                if (m_bEnableTXn == true)
                    sw.WriteLine("EnableTXn,1");
                else
                    sw.WriteLine("EnableTXn,0");

                sw.WriteLine();

                // sw.WriteLine("Result:");
                sw.WriteLine("Result");

                sw.WriteLine(string.Format("Frequency,<=,{0}KHz", m_cResultData.m_dSaturationMaxFrequency));
                sw.WriteLine(string.Format("PH1,=,0x{0}", m_nFixedPH1.ToString("x2").ToUpper()));
                sw.WriteLine(string.Format("PH2,>=,0x{0}", m_cResultData.m_nSaturationMinPH2.ToString("x2").ToUpper()));
                sw.WriteLine(string.Format("Uniformity,>=,{0}%", m_cResultData.m_dSaturationUniformityLB));

                if (m_bWarningOccurred)
                {
                    sw.WriteLine();

                    sw.WriteLine("Warning");

                    for (int index = 0; index < m_cSkipFrequencyData_List.Count; index++)
                    {
                        sw.WriteLine(string.Format("Frequency={0:0.000}KHz,PH1+PH2={1},Message={2}", m_cSkipFrequencyData_List[index].m_dFrequency.ToString("0.000"), m_cSkipFrequencyData_List[index].m_nPH1PH2Sum.ToString("d"), m_cSkipFrequencyData_List[index].m_sMessage));
                    }
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = "Save Report Data Error";
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

        private bool SaveSkipFreqSetFile()
        {
            if (!m_bWarningOccurred)
                return true;

            bool bError = false;
            string sSkipFreqSetFilePath = m_sSkipFreqSetFilePath;

            FileStream fs = new FileStream(sSkipFreqSetFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for (int index = 0; index < m_cSkipFrequencyData_List.Count; index++)
                {
                    sw.WriteLine(string.Format("{0:0.000},{1},{2}", m_cSkipFrequencyData_List[index].m_dFrequency.ToString("0.000"), m_cSkipFrequencyData_List[index].m_nPH1PH2Sum.ToString("d"), m_cSkipFrequencyData_List[index].m_sMessage));
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = "Save Skip Frequency Set Error";
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
