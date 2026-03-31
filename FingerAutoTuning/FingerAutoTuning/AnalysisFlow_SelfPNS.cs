using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FingerAutoTuningParameter;
using System.Drawing;

namespace FingerAutoTuning
{
    class AnalysisFlow_SelfPNS : AnalysisFlow
    {
        private const int m_nCTModeNumber = 4;

        private const string m_sDATATYPE_RAWPMEAN = "RawPMean";
        private const string m_sDATATYPE_RAWPSTD = "RawPStd";
        private const string m_sDATATYPE_RAWPMAX = "RawPMax";
        private const string m_sDATATYPE_RAWPMIN = "RawPMin";
        private const string m_sDATATYPE_RAWNMEAN = "RawNMean";
        private const string m_sDATATYPE_RAWNSTD = "RawNStd";
        private const string m_sDATATYPE_RAWNMAX = "RawNMax";
        private const string m_sDATATYPE_RAWNMIN = "RawNMin";

        private const string m_sKVALUETYPE_NCP = "NCP Value";
        private const string m_sKVALUETYPE_NCN = "NCN Value";

        private string[] m_sDataType_Array = new string[]
        {
            m_sDATATYPE_RAWPMEAN,
            m_sDATATYPE_RAWPSTD,
            m_sDATATYPE_RAWPMAX,
            m_sDATATYPE_RAWPMIN,
            m_sDATATYPE_RAWNMEAN,
            m_sDATATYPE_RAWNSTD,
            m_sDATATYPE_RAWNMAX,
            m_sDATATYPE_RAWNMIN
        };

        private string[] m_sKValueType_Array = new string[]
        {
            m_sKVALUETYPE_NCP,
            m_sKVALUETYPE_NCN
        };

        private int m_nTraceNumber_RX = 0;
        private int m_nTraceNumber_TX = 0;

        private enum KValueState
        {
            NA,
            GetKValue,
            NoKValue
        }

        private KValueState m_eGetKValue = KValueState.NA;

        private int m_n_MS_MM_RX0Count = 42;
        private int m_nStartTrace = 1;

        public class DataInfo
        {
            public string m_sFileName = "";
            public int m_nSetIndex = -1;
            public int m_n_SELF_PH1 = 0;
            public int m_n_SELF_PH2E_LAT = 0;
            public int m_n_SELF_PH2E_LMT = 0;
            public int m_n_SELF_PH2_LAT = 0;
            public int m_n_SELF_PH2 = 0;
            public int m_nSelf_DFT_NUM = 0;
            public int m_nSelf_Gain = -1;
            public int m_nRXTraceNumber = 0;
            public int m_nTXTraceNumber = 0;
            public double m_dFrequency = 0.0;
            public bool m_bGetKValue = false;
            public int m_nNCPValue = -1;
            public int m_nNCNValue = -1;

            public List<int[,]> m_nRawPNData_List = new List<int[,]>();
            public List<int[]> m_nADCData_List = new List<int[]>();
            public int[] m_nNCPValue_Array = null;
            public int[] m_nNCNValue_Array = null;

            public StatisticData m_cStatisticData = new StatisticData();
        }

        public class InfoDataComparer : IComparer<DataInfo>
        {
            public int Compare(DataInfo cDataInfo1, DataInfo cDataInfo2)
            {
                if (m_nCompareOperator == m_nCOMPARE_Frequency)
                {
                    if (cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency) != 0)
                        return cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency);
                    else if (cDataInfo1.m_nSetIndex.CompareTo(cDataInfo2.m_nSetIndex) != 0)
                        return cDataInfo1.m_nSetIndex.CompareTo(cDataInfo2.m_nSetIndex);
                    else if (cDataInfo1.m_n_SELF_PH2.CompareTo(cDataInfo2.m_n_SELF_PH2) != 0)
                        return cDataInfo1.m_n_SELF_PH2.CompareTo(cDataInfo2.m_n_SELF_PH2);
                    else if (cDataInfo1.m_n_SELF_PH2E_LMT.CompareTo(cDataInfo2.m_n_SELF_PH2E_LMT) != 0)
                        return cDataInfo1.m_n_SELF_PH2E_LMT.CompareTo(cDataInfo2.m_n_SELF_PH2E_LMT);
                    else if (cDataInfo1.m_nSelf_DFT_NUM.CompareTo(cDataInfo2.m_nSelf_DFT_NUM) != 0)
                        return cDataInfo1.m_nSelf_DFT_NUM.CompareTo(cDataInfo2.m_nSelf_DFT_NUM);
                    else if (cDataInfo1.m_nNCPValue.CompareTo(cDataInfo2.m_nNCPValue) != 0)
                        return cDataInfo1.m_nNCPValue.CompareTo(cDataInfo2.m_nNCPValue);
                    else if (cDataInfo1.m_nNCNValue.CompareTo(cDataInfo2.m_nNCNValue) != 0)
                        return cDataInfo1.m_nNCNValue.CompareTo(cDataInfo2.m_nNCNValue);
                    else
                        return cDataInfo1.m_n_SELF_PH1.CompareTo(cDataInfo2.m_n_SELF_PH1);
                }
                else if (m_nCompareOperator == m_nCOMPARE_SetIndex)
                {
                    if (cDataInfo1.m_nSetIndex.CompareTo(cDataInfo2.m_nSetIndex) != 0)
                        return cDataInfo1.m_nSetIndex.CompareTo(cDataInfo2.m_nSetIndex);
                    else if (cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency) != 0)
                        return cDataInfo1.m_dFrequency.CompareTo(cDataInfo2.m_dFrequency);
                    else if (cDataInfo1.m_n_SELF_PH2.CompareTo(cDataInfo2.m_n_SELF_PH2) != 0)
                        return cDataInfo1.m_n_SELF_PH2.CompareTo(cDataInfo2.m_n_SELF_PH2);
                    else if (cDataInfo1.m_n_SELF_PH2E_LMT.CompareTo(cDataInfo2.m_n_SELF_PH2E_LMT) != 0)
                        return cDataInfo1.m_n_SELF_PH2E_LMT.CompareTo(cDataInfo2.m_n_SELF_PH2E_LMT);
                    else if (cDataInfo1.m_nSelf_DFT_NUM.CompareTo(cDataInfo2.m_nSelf_DFT_NUM) != 0)
                        return cDataInfo1.m_nSelf_DFT_NUM.CompareTo(cDataInfo2.m_nSelf_DFT_NUM);
                    else if (cDataInfo1.m_nNCPValue.CompareTo(cDataInfo2.m_nNCPValue) != 0)
                        return cDataInfo1.m_nNCPValue.CompareTo(cDataInfo2.m_nNCPValue);
                    else if (cDataInfo1.m_nNCNValue.CompareTo(cDataInfo2.m_nNCNValue) != 0)
                        return cDataInfo1.m_nNCNValue.CompareTo(cDataInfo2.m_nNCNValue);
                    else
                        return cDataInfo1.m_n_SELF_PH1.CompareTo(cDataInfo2.m_n_SELF_PH1);
                }
                else
                    return cDataInfo1.m_nSetIndex.CompareTo(cDataInfo2.m_nSetIndex);
            }
        }

        private List<DataInfo> m_cDataInfo_RX_List = null;
        private List<DataInfo> m_cDataInfo_TX_List = null;

        public class StatisticData
        {
            public List<double> m_dRawPStd_List = new List<double>();
            public List<double> m_dRawPMean_List = new List<double>();
            public List<int> m_nRawPMax_List = new List<int>();
            public List<int> m_nRawPMin_List = new List<int>();

            public List<double> m_dRawNStd_List = new List<double>();
            public List<double> m_dRawNMean_List = new List<double>();
            public List<int> m_nRawNMax_List = new List<int>();
            public List<int> m_nRawNMin_List = new List<int>();
        }

        private class ChartColor
        {
            public List<int> m_nColorR_List = new List<int>();
            public List<int> m_nColorG_List = new List<int>();
            public List<int> m_nColorB_List = new List<int>();
        }

        public class SetDataInfo
        {
            public int m_nSetIndex = -1;
            public double m_dFrequency = 0.0;
            public int m_n_SELF_PH1 = 0;
            public int m_n_SELF_PH2E_LAT = 0;
            public int m_n_SELF_PH2E_LMT = 0;
            public int m_n_SELF_PH2_LAT = 0;
            public int m_n_SELF_PH2 = 0;
            public int m_nSelf_DFT_NUM = 0;
            public int m_nSelf_Gain = -1;
            public int m_nRXTraceNumber = 0;
            public int m_nTXTraceNumber = 0;

            public int m_nNCPMax = -1;
            public int m_nNCPMin = -1;
            public int m_nNCNMax = -1;
            public int m_nNCNMin = -1;

            public double[,] m_dRawPMeanSlope_Array = null;
            public double[,] m_dRawNMeanSlope_Array = null;

            public int[] m_nFitNCP_Array = null;
            public int[] m_nFitNCN_Array = null;
        }

        private List<SetDataInfo> m_cSetDataInfo_RX_List = null;
        private List<SetDataInfo> m_cSetDataInfo_TX_List = null;

        public AnalysisFlow_SelfPNS(frmMain.FlowStep cFlowStep, string sLogDirectoryPath, string sH5LogDirectoryPath, bool bGenerateH5Data, frmMain cfrmParent, string sProjectName)
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
            m_n_MS_MM_RX0Count = ParamFingerAutoTuning.m_nSelfPNS_MS_MM_RX0Count;
            m_nStartTrace = ParamFingerAutoTuning.m_nSelfPNSStartTrace;
        }

        public override void InitializeSourceDataList()
        {
            m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_RawData);
        }

        public override bool MainFlow(ref string sErrorMessage)
        {
            if (GetDataCount() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (ReadRawData() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (SaveRawPNData() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (AnalysisData() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (SaveStatisticFile() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (SaveAnalysisFile() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (SaveNCPNCNTable() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            if (SaveLineChart() == false)
            {
                SetErrorMessage(ref sErrorMessage);
                return false;
            }

            return true;
        }

        private bool ReadRawData()
        {
            string sDataPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_RawData);

            m_nAnalysisCount = m_nTotalFileCount + 1;

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.InitialstatusstripMessage(m_nAnalysisCount, "Data Analysis...");
            });

            foreach (string sFilePath in Directory.EnumerateFiles(sDataPath, "*.csv", SearchOption.TopDirectoryOnly))
            {
                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);
                Self_FileCheckInfo cSelf_FileCheckInfo = new Self_FileCheckInfo();
                List<int[,]> nRawPNData_List = new List<int[,]>();
                List<int[]> nADCData_List = new List<int[]>();

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                if (CheckFileInfo(ref cSelf_FileCheckInfo, srFile, sFileName) == false)
                    return false;

                if (CheckFileInfoIdentical(cSelf_FileCheckInfo, sFileName, MainConstantParameter.m_sDATATYPE_RawData) == false)
                    return false;

                int nSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LAT, cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT,
                                                                    cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT, cSelf_FileCheckInfo.m_nRead_SELF_PH2);
                double dFrequncy = ElanConvert.Convert2Frequency(cSelf_FileCheckInfo.m_nRead_SELF_PH1, nSelfPH2Sum);

                DataInfo cDataInfo = new DataInfo();
                cDataInfo.m_sFileName = sFileName;
                cDataInfo.m_nSetIndex = cSelf_FileCheckInfo.m_nSetIndex;
                cDataInfo.m_dFrequency = dFrequncy;
                cDataInfo.m_n_SELF_PH1 = cSelf_FileCheckInfo.m_nRead_SELF_PH1;
                cDataInfo.m_n_SELF_PH2E_LMT = cSelf_FileCheckInfo.m_nRead_SELF_PH2E_LMT;
                cDataInfo.m_n_SELF_PH2_LAT = cSelf_FileCheckInfo.m_nRead_SELF_PH2_LAT;
                cDataInfo.m_n_SELF_PH2 = cSelf_FileCheckInfo.m_nRead_SELF_PH2;
                cDataInfo.m_nSelf_DFT_NUM = cSelf_FileCheckInfo.m_nReadSelf_DFT_NUM;
                cDataInfo.m_nSelf_Gain = cSelf_FileCheckInfo.m_nReadSelf_Gain;
                cDataInfo.m_nRXTraceNumber = cSelf_FileCheckInfo.m_nRXTraceNumber;
                cDataInfo.m_nTXTraceNumber = cSelf_FileCheckInfo.m_nTXTraceNumber;
                cDataInfo.m_bGetKValue = cSelf_FileCheckInfo.m_bGetKValue;
                cDataInfo.m_nNCPValue = cSelf_FileCheckInfo.m_nNCPValue;
                cDataInfo.m_nNCNValue = cSelf_FileCheckInfo.m_nNCNValue;

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

                int[] nNCPValue_Array = new int[cSelf_FileCheckInfo.m_nRXTraceNumber];
                int[] nNCNValue_Array = new int[cSelf_FileCheckInfo.m_nRXTraceNumber];

                srFile = new StreamReader(sFilePath, Encoding.Default);

                if (GetFrameData(ref nRawPNData_List, ref nADCData_List, ref nNCPValue_Array, ref nNCNValue_Array, cSelf_FileCheckInfo, srFile, sFileName) == false)
                    return false;

                if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                {
                    m_cDataInfo_RX_List[nListIndex].m_nRawPNData_List = nRawPNData_List;
                    m_cDataInfo_RX_List[nListIndex].m_nADCData_List = nADCData_List;
                    m_cDataInfo_RX_List[nListIndex].m_nNCPValue_Array = nNCPValue_Array;
                    m_cDataInfo_RX_List[nListIndex].m_nNCPValue_Array = nNCNValue_Array;
                }
                else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                {
                    m_cDataInfo_TX_List[nListIndex].m_nRawPNData_List = nRawPNData_List;
                    m_cDataInfo_TX_List[nListIndex].m_nADCData_List = nADCData_List;
                    m_cDataInfo_TX_List[nListIndex].m_nNCPValue_Array = nNCPValue_Array;
                    m_cDataInfo_TX_List[nListIndex].m_nNCPValue_Array = nNCNValue_Array;
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
                        else if (sSubString_Array[0] == "SetIndex")
                            Int32.TryParse(sSubString_Array[1], out cSelf_FileCheckInfo.m_nSetIndex);
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
                        else if (sSubString_Array[0] == "SelfNCPValue")
                            Int32.TryParse(sSubString_Array[1], out cSelf_FileCheckInfo.m_nNCPValue);
                        else if (sSubString_Array[0] == "SelfNCNValue")
                        {
                            Int32.TryParse(sSubString_Array[1], out cSelf_FileCheckInfo.m_nNCNValue);
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

            return true;
        }

        private bool CheckFileInfoIdentical(Self_FileCheckInfo cSelf_FileCheckInfo, string sFileName, string sDataType)
        {
            List<DataInfo> cDataInfo_List = null;

            if (cSelf_FileCheckInfo.m_eTraceType == TraceType.RX)
                cDataInfo_List = m_cDataInfo_RX_List;
            else if (cSelf_FileCheckInfo.m_eTraceType == TraceType.TX)
                cDataInfo_List = m_cDataInfo_TX_List;

            if (cDataInfo_List == null)
                return true;

            for (int nDataIndex = 0; nDataIndex < cDataInfo_List.Count; nDataIndex++)
            {
                string sMatchFileName = "";

                switch (sDataType)
                {
                    case MainConstantParameter.m_sDATATYPE_RawData:
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
                    nSelfPH2Sum == nCompareSelfPH1PH2Sum &&
                    nSelfDFT_NUM == nCompareSelfDFT_NUM &&
                    cSelf_FileCheckInfo.m_nNCPValue == cDataInfo_List[nDataIndex].m_nNCPValue &&
                    cSelf_FileCheckInfo.m_nNCNValue == cDataInfo_List[nDataIndex].m_nNCNValue)
                {
                    m_sErrorMessage = string.Format("Self PH1, PH2Sum, Sum and NCP/NCN Unique Check Error in {0} and {1}[Value1=0x{2} Value2=0x{3}]", sFileName,
                                                    sMatchFileName,
                                                    nCompareSelfPH1PH2Sum.ToString("x4").ToUpper(),
                                                    nSelfPH1PH2Sum.ToString("x4").ToUpper());
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

        private bool GetFrameData(ref List<int[,]> nRawPNData_List, ref List<int[]> nADCData_List, ref int[] nNCPValue_Array, ref int[] nNCNValue_Array,
                                  Self_FileCheckInfo cSelf_FileCheckInfo, StreamReader srFile, string sFileName)
        {
            bool bGetFrameData = false;
            bool bGetKValue = false;
            int[,] nSingleRawPNData_Array = null;
            int[] nSingleADCData_Array = null;
            int nTXCount = 0;
            string sLine = "";

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSplitData_Array = sLine.Split(',');

                    if (sSplitData_Array.Length >= 2)
                    {
                        if (sSplitData_Array[0] == "Frame")
                        {
                            bGetFrameData = true;
                            nTXCount = 0;
                            nSingleRawPNData_Array = new int[cSelf_FileCheckInfo.m_nSetSelf_DFT_NUM, cSelf_FileCheckInfo.m_nRXTraceNumber];
                            nSingleADCData_Array = new int[cSelf_FileCheckInfo.m_nRXTraceNumber];
                            continue;
                        }
                    }

                    if (bGetFrameData == true)
                    {
                        if (nTXCount == cSelf_FileCheckInfo.m_nTXTraceNumber)
                        {
                            if (sSplitData_Array.Length >= cSelf_FileCheckInfo.m_nRXTraceNumber)
                            {
                                for (int nRXIndex = 0; nRXIndex < cSelf_FileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                                    nSingleADCData_Array[nRXIndex] = Convert.ToInt32(sSplitData_Array[nRXIndex]);
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
                                        if (sSplitData_Array.Length >= cSelf_FileCheckInfo.m_nRXTraceNumber)
                                        {
                                            for (int nRXIndex = 0; nRXIndex < cSelf_FileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                                                nNCNValue_Array[nRXIndex] = Convert.ToInt32(sSplitData_Array[nRXIndex]);

                                            nTXCount++;
                                        }
                                    }
                                    else if (nTXCount == 1)
                                    {
                                        if (sSplitData_Array.Length >= cSelf_FileCheckInfo.m_nRXTraceNumber)
                                        {
                                            for (int nRXIndex = 0; nRXIndex < cSelf_FileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                                                nNCPValue_Array[nRXIndex] = Convert.ToInt32(sSplitData_Array[nRXIndex]);

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

                                if (sSplitData_Array.Length >= cSelf_FileCheckInfo.m_nRXTraceNumber)
                                {
                                    for (int nRXIndex = 0; nRXIndex < cSelf_FileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                                        nSingleRawPNData_Array[nTXIndex, nRXIndex] = Convert.ToInt32(sSplitData_Array[nRXIndex]);

                                    nTXCount++;
                                }

                                if (nTXCount == cSelf_FileCheckInfo.m_nTXTraceNumber)
                                {
                                    nRawPNData_List.Add(nSingleRawPNData_Array);
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

            if (nRawPNData_List == null || nRawPNData_List.Count == 0)
            {
                m_sErrorMessage = string.Format("Read Self Raw PN Data Error in {0}[Count:{1}]", sFileName, nRawPNData_List.Count);
                return false;
            }

            if (nADCData_List == null || nADCData_List.Count == 0)
            {
                m_sErrorMessage = string.Format("Read Self ADC Data Error in {0}[Count:{1}]", sFileName, nADCData_List.Count);
                return false;
            }

            return true;
        }

        private bool SaveRawPNData()
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
                    if (WriteRawPNData(cDataInfo, eTraceType) == false)
                        return false;
                }
            }

            return true;
        }

        private bool WriteRawPNData(DataInfo cDataInfo, TraceType eTraceType)
        {
            bool bError = false;
            int nFrameNumber = cDataInfo.m_nRawPNData_List.Count;

            string sDirectoryName = MainConstantParameter.m_sDATATYPE_ANALYSIS;
            string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDirectoryName);

            if (Directory.Exists(sDirectoryPath) == false)
                Directory.CreateDirectory(sDirectoryPath);

            int nPH1 = cDataInfo.m_n_SELF_PH1;
            int nPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cDataInfo.m_n_SELF_PH2E_LAT, cDataInfo.m_n_SELF_PH2E_LMT, cDataInfo.m_n_SELF_PH2_LAT, cDataInfo.m_n_SELF_PH2);
            double dFrequency = cDataInfo.m_dFrequency;

            string sFileName = string.Format("RawPN_{0}_{1}_{2}_{3}_P{4:00}N{5:00}", dFrequency.ToString("0.000"), nPH1.ToString("x2").ToUpper(), nPH2Sum.ToString("x2").ToUpper(),
                                             eTraceType.ToString(), cDataInfo.m_nNCPValue, cDataInfo.m_nNCNValue);

            string sFilePath = string.Format(@"{0}\{1}.csv", sDirectoryPath, sFileName);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("RawPNData");

                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    for (int nTXIndex = 0; nTXIndex < cDataInfo.m_nSelf_DFT_NUM; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < cDataInfo.m_nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex == cDataInfo.m_nRXTraceNumber - 1)
                                sw.WriteLine(cDataInfo.m_nRawPNData_List[nFrameIndex][nTXIndex, nRXIndex]);
                            else
                                sw.Write(string.Format("{0},", cDataInfo.m_nRawPNData_List[nFrameIndex][nTXIndex, nRXIndex]));
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save RawPN Data Error in {0} Frequency={1}KHz(FileName:{2})", eTraceType.ToString(), dFrequency.ToString(), sFileName);
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

        private bool AnalysisData()
        {
            ComputeStatisticData();

            GetSetDataInfo();

            ComputeFitNCPNCNData();

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
                    for (int nTraceIndex = 0; nTraceIndex < cDataInfo.m_nRXTraceNumber; nTraceIndex++)
                    {
                        List<int> nRawPData_List = new List<int>();
                        List<int> nRawNData_List = new List<int>();

                        for (int nFrameIndex = 0; nFrameIndex < cDataInfo.m_nRawPNData_List.Count; nFrameIndex++)
                        {
                            for (int nTXIndex = 0; nTXIndex < cDataInfo.m_nSelf_DFT_NUM; nTXIndex++)
                            {
                                int nModeValue = nTXIndex % (m_nCTModeNumber * 2);

                                if (nModeValue < m_nCTModeNumber)
                                    nRawPData_List.Add(cDataInfo.m_nRawPNData_List[nFrameIndex][nTXIndex, nTraceIndex]);
                                else
                                    nRawNData_List.Add(cDataInfo.m_nRawPNData_List[nFrameIndex][nTXIndex, nTraceIndex]);
                            }
                        }

                        double dRawPMean = Math.Round(nRawPData_List.Average(), 3, MidpointRounding.AwayFromZero);
                        double dRawPStd = Math.Round(MathMethod.ComputeStd(nRawPData_List), 3, MidpointRounding.AwayFromZero);
                        int nRawPMax = nRawPData_List.Max();
                        int nRawPMin = nRawPData_List.Min();

                        double dRawNMean = Math.Round(nRawNData_List.Average(), 3, MidpointRounding.AwayFromZero);
                        double dRawNStd = Math.Round(MathMethod.ComputeStd(nRawNData_List), 3, MidpointRounding.AwayFromZero);
                        int nRawNMax = nRawNData_List.Max();
                        int nRawNMin = nRawNData_List.Min();

                        cDataInfo.m_cStatisticData.m_dRawPMean_List.Add(dRawPMean);
                        cDataInfo.m_cStatisticData.m_dRawPStd_List.Add(dRawPStd);
                        cDataInfo.m_cStatisticData.m_nRawPMax_List.Add(nRawPMax);
                        cDataInfo.m_cStatisticData.m_nRawPMin_List.Add(nRawPMin);

                        cDataInfo.m_cStatisticData.m_dRawNMean_List.Add(dRawNMean);
                        cDataInfo.m_cStatisticData.m_dRawNStd_List.Add(dRawNStd);
                        cDataInfo.m_cStatisticData.m_nRawNMax_List.Add(nRawNMax);
                        cDataInfo.m_cStatisticData.m_nRawNMin_List.Add(nRawNMin);
                    }
                }
            }
        }

        private void GetSetDataInfo()
        {
            m_nCompareOperator = m_nCOMPARE_SetIndex;

            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();

                if (eTraceType == TraceType.RX)
                {
                    m_cDataInfo_RX_List.Sort(new InfoDataComparer());
                    cDataInfo_List = m_cDataInfo_RX_List;
                }
                else if (eTraceType == TraceType.TX)
                {
                    m_cDataInfo_TX_List.Sort(new InfoDataComparer());
                    cDataInfo_List = m_cDataInfo_TX_List;
                }

                int nCurrentSetIndex = -1;

                foreach (DataInfo cDataInfo in cDataInfo_List)
                {
                    int nSetIndex = cDataInfo.m_nSetIndex;

                    if (nSetIndex > nCurrentSetIndex)
                    {
                        SetDataInfo cSetDataInfo = new SetDataInfo();
                        cSetDataInfo.m_nSetIndex = nSetIndex;
                        cSetDataInfo.m_dFrequency = cDataInfo.m_dFrequency;
                        cSetDataInfo.m_n_SELF_PH1 = cDataInfo.m_n_SELF_PH1;
                        cSetDataInfo.m_n_SELF_PH2E_LAT = cDataInfo.m_n_SELF_PH2E_LAT;
                        cSetDataInfo.m_n_SELF_PH2E_LMT = cDataInfo.m_n_SELF_PH2E_LMT;
                        cSetDataInfo.m_n_SELF_PH2_LAT = cDataInfo.m_n_SELF_PH2_LAT;
                        cSetDataInfo.m_n_SELF_PH2 = cDataInfo.m_n_SELF_PH2;
                        cSetDataInfo.m_nSelf_DFT_NUM = cDataInfo.m_nSelf_DFT_NUM;
                        cSetDataInfo.m_nSelf_Gain = cDataInfo.m_nSelf_Gain;
                        cSetDataInfo.m_nRXTraceNumber = cDataInfo.m_nRXTraceNumber;
                        cSetDataInfo.m_nTXTraceNumber = cDataInfo.m_nTXTraceNumber;

                        cSetDataInfo.m_nNCPMax = cDataInfo_List.Where(x => x.m_nSetIndex == nSetIndex).Max(x => x.m_nNCPValue);
                        cSetDataInfo.m_nNCPMin = cDataInfo_List.Where(x => x.m_nSetIndex == nSetIndex).Min(x => x.m_nNCPValue);
                        cSetDataInfo.m_nNCNMax = cDataInfo_List.Where(x => x.m_nSetIndex == nSetIndex).Max(x => x.m_nNCNValue);
                        cSetDataInfo.m_nNCNMin = cDataInfo_List.Where(x => x.m_nSetIndex == nSetIndex).Min(x => x.m_nNCNValue);

                        if (eTraceType == TraceType.RX)
                        {
                            if (m_cSetDataInfo_RX_List == null)
                                m_cSetDataInfo_RX_List = new List<SetDataInfo>();

                            m_cSetDataInfo_RX_List.Add(cSetDataInfo);
                        }
                        else if (eTraceType == TraceType.TX)
                        {
                            if (m_cSetDataInfo_TX_List == null)
                                m_cSetDataInfo_TX_List = new List<SetDataInfo>();

                            m_cSetDataInfo_TX_List.Add(cSetDataInfo);
                        }

                        nCurrentSetIndex = nSetIndex;
                    }
                    else
                        continue;
                }

                List<SetDataInfo> cSetDataInfo_List = new List<SetDataInfo>();

                if (eTraceType == TraceType.RX)
                {
                    cSetDataInfo_List = m_cSetDataInfo_RX_List;
                }
                else if (eTraceType == TraceType.TX)
                {
                    cSetDataInfo_List = m_cSetDataInfo_TX_List;
                }

                foreach (SetDataInfo cSetDataInfo in cSetDataInfo_List)
                {
                    List<DataInfo> cDataInfo_SetIndex_List = cDataInfo_List.FindAll(x => x.m_nSetIndex == cSetDataInfo.m_nSetIndex);

                    cSetDataInfo.m_dRawPMeanSlope_Array = new double[cSetDataInfo.m_nNCPMax - cSetDataInfo.m_nNCPMin, cSetDataInfo.m_nRXTraceNumber];
                    cSetDataInfo.m_dRawNMeanSlope_Array = new double[cSetDataInfo.m_nNCNMax - cSetDataInfo.m_nNCNMin, cSetDataInfo.m_nRXTraceNumber];

                    cDataInfo_SetIndex_List.Sort(new InfoDataComparer());

                    for (int nDataIndex = 1; nDataIndex < cDataInfo_SetIndex_List.Count; nDataIndex++)
                    {
                        for (int nTraceIndex = 0; nTraceIndex < cSetDataInfo.m_nRXTraceNumber; nTraceIndex++)
                        {
                            double dRawPMeanSlope = cDataInfo_SetIndex_List[nDataIndex].m_cStatisticData.m_dRawPMean_List[nTraceIndex] -
                                                    cDataInfo_SetIndex_List[nDataIndex - 1].m_cStatisticData.m_dRawPMean_List[nTraceIndex];

                            double dRawNMeanSlope = cDataInfo_SetIndex_List[nDataIndex].m_cStatisticData.m_dRawNMean_List[nTraceIndex] -
                                                    cDataInfo_SetIndex_List[nDataIndex - 1].m_cStatisticData.m_dRawNMean_List[nTraceIndex];

                            cSetDataInfo.m_dRawPMeanSlope_Array[nDataIndex - 1, nTraceIndex] = dRawPMeanSlope;
                            cSetDataInfo.m_dRawNMeanSlope_Array[nDataIndex - 1, nTraceIndex] = dRawNMeanSlope;
                        }
                    }
                }
            }
        }

        private void ComputeFitNCPNCNData()
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<SetDataInfo> cSetDataInfo_List = new List<SetDataInfo>();

                if (eTraceType == TraceType.RX)
                {
                    cSetDataInfo_List = m_cSetDataInfo_RX_List;
                }
                else if (eTraceType == TraceType.TX)
                {
                    cSetDataInfo_List = m_cSetDataInfo_TX_List;
                }

                foreach (SetDataInfo cSetDataInfo in cSetDataInfo_List)
                {
                    cSetDataInfo.m_nFitNCP_Array = new int[cSetDataInfo.m_nRXTraceNumber];
                    cSetDataInfo.m_nFitNCN_Array = new int[cSetDataInfo.m_nRXTraceNumber];

                    for (int nTraceIndex = 0; nTraceIndex < cSetDataInfo.m_nRXTraceNumber; nTraceIndex++)
                    {
                        double dRawPMinSlope = 0.0;
                        int nFitNCP = 0;
                        double dRawNMaxSlope = 0.0;
                        int nFitNCN = 0;

                        for (int nSlopeIndex = 0; nSlopeIndex < cSetDataInfo.m_nNCPMax - cSetDataInfo.m_nNCPMin; nSlopeIndex++)
                        {
                            if (nSlopeIndex == 0)
                            {
                                dRawPMinSlope = cSetDataInfo.m_dRawPMeanSlope_Array[nSlopeIndex, nTraceIndex];
                                nFitNCP = nSlopeIndex;
                            }
                            else
                            {
                                if (cSetDataInfo.m_dRawPMeanSlope_Array[nSlopeIndex, nTraceIndex] < dRawPMinSlope)
                                {
                                    dRawPMinSlope = cSetDataInfo.m_dRawPMeanSlope_Array[nSlopeIndex, nTraceIndex];
                                    nFitNCP = nSlopeIndex;
                                }
                            }
                        }

                        for (int nSlopeIndex = 0; nSlopeIndex < cSetDataInfo.m_nNCNMax - cSetDataInfo.m_nNCNMin; nSlopeIndex++)
                        {
                            if (nSlopeIndex == 0)
                            {
                                dRawNMaxSlope = cSetDataInfo.m_dRawNMeanSlope_Array[nSlopeIndex, nTraceIndex];
                                nFitNCN = nSlopeIndex;
                            }
                            else
                            {
                                if (cSetDataInfo.m_dRawNMeanSlope_Array[nSlopeIndex, nTraceIndex] < dRawNMaxSlope)
                                {
                                    dRawNMaxSlope = cSetDataInfo.m_dRawNMeanSlope_Array[nSlopeIndex, nTraceIndex];
                                    nFitNCN = nSlopeIndex;
                                }
                            }
                        }

                        nFitNCP = nFitNCP + 1 + cSetDataInfo.m_nNCPMin;
                        nFitNCN = nFitNCN + 1 + cSetDataInfo.m_nNCNMin;

                        cSetDataInfo.m_nFitNCP_Array[nTraceIndex] = nFitNCP;
                        cSetDataInfo.m_nFitNCN_Array[nTraceIndex] = nFitNCN;
                    }
                }
            }
        }

        private bool SaveStatisticFile()
        {
            bool bError = false;

            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();
                List<SetDataInfo> cSetDataInfo_List = new List<SetDataInfo>();
                int nTraceNumber = 0;

                if (eTraceType == TraceType.RX)
                {
                    cDataInfo_List = m_cDataInfo_RX_List;
                    cSetDataInfo_List = m_cSetDataInfo_RX_List;
                    nTraceNumber = m_nTraceNumber_RX;
                }
                else if (eTraceType == TraceType.TX)
                {
                    cDataInfo_List = m_cDataInfo_TX_List;
                    cSetDataInfo_List = m_cSetDataInfo_TX_List;
                    nTraceNumber = m_nTraceNumber_TX;
                }

                m_nCompareOperator = m_nCOMPARE_SetIndex;
                cDataInfo_List.Sort(new InfoDataComparer());

                string sReportFilePath = string.Format(@"{0}\Statistic_{1}.csv", m_sLogDirectoryPath, eTraceType.ToString());

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
                    "NCP", 
                    "NCN", 
                    "" 
                };
                List<string> sColumnHeader_RawPMeanSlope_List = new List<string>() { 
                    "Index", 
                    "PH1", 
                    "PH2E_LMT", 
                    "PH2_LAT", 
                    "PH2", 
                    "Frequency(KHz)", 
                    "DFT_NUM", 
                    "Gain", 
                    "NCP", 
                    "" 
                };
                List<string> sColumnHeader_RawNMeanSlope_List = new List<string>() { 
                    "Index", 
                    "PH1", 
                    "PH2E_LMT", 
                    "PH2_LAT", 
                    "PH2", 
                    "Frequency(KHz)", 
                    "DFT_NUM", 
                    "Gain", 
                    "NCN", 
                    "" 
                };

                for (int nTraceIndex = 1; nTraceIndex <= nTraceNumber; nTraceIndex++)
                {
                    sColumnHeader_List.Add(nTraceIndex.ToString());
                    sColumnHeader_RawPMeanSlope_List.Add(nTraceIndex.ToString());
                    sColumnHeader_RawNMeanSlope_List.Add(nTraceIndex.ToString());
                }

                try
                {
                    Write_Tool_Information(sw, m_sFILETYPE_STATISTIC);

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

                    sw.WriteLine();
                    WriteSlopeValueData(sw, sColumnHeader_RawPMeanSlope_List, cSetDataInfo_List, "Raw P Mean Slope", true);
                    WriteSlopeValueData(sw, sColumnHeader_RawNMeanSlope_List, cSetDataInfo_List, "Raw N Mean Slope", false);
                }
                catch (IOException ex)
                {
                    string sMessage = ex.Message;

                    m_sErrorMessage = string.Format("Save {0} Statistic Data Error", eTraceType.ToString());
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

        private bool SaveAnalysisFile()
        {
            bool bError = false;

            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<SetDataInfo> cSetDataInfo_List = new List<SetDataInfo>();
                int nTraceNumber = 0;

                if (eTraceType == TraceType.RX)
                {
                    cSetDataInfo_List = m_cSetDataInfo_RX_List;
                    nTraceNumber = m_nTraceNumber_RX;
                }
                else if (eTraceType == TraceType.TX)
                {
                    cSetDataInfo_List = m_cSetDataInfo_TX_List;
                    nTraceNumber = m_nTraceNumber_TX;
                }

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
                    "MaxNCP", 
                    "MinNCP",
                    "MaxNCN", 
                    "MinNCN", 
                    "" 
                };

                for (int nTraceIndex = 1; nTraceIndex <= nTraceNumber; nTraceIndex++)
                    sColumnHeader_List.Add(nTraceIndex.ToString());

                try
                {
                    Write_Tool_Information(sw, m_sFILETYPE_ANALYSIS);

                    WriteFitData(sw, sColumnHeader_List, cSetDataInfo_List, "Fit NCP Value", true);
                    WriteFitData(sw, sColumnHeader_List, cSetDataInfo_List, "Fit NCN Value", false);
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

                if (sDataType == m_sDATATYPE_RAWPMEAN)
                    dValue_List = cDataInfo.m_cStatisticData.m_dRawPMean_List;
                else if (sDataType == m_sDATATYPE_RAWPSTD)
                    dValue_List = cDataInfo.m_cStatisticData.m_dRawPStd_List;
                else if (sDataType == m_sDATATYPE_RAWPMAX)
                    nValue_List = cDataInfo.m_cStatisticData.m_nRawPMax_List;
                else if (sDataType == m_sDATATYPE_RAWPMIN)
                    nValue_List = cDataInfo.m_cStatisticData.m_nRawPMin_List;
                else if (sDataType == m_sDATATYPE_RAWNMEAN)
                    dValue_List = cDataInfo.m_cStatisticData.m_dRawNMean_List;
                else if (sDataType == m_sDATATYPE_RAWNSTD)
                    dValue_List = cDataInfo.m_cStatisticData.m_dRawNStd_List;
                else if (sDataType == m_sDATATYPE_RAWNMAX)
                    nValue_List = cDataInfo.m_cStatisticData.m_nRawNMax_List;
                else if (sDataType == m_sDATATYPE_RAWNMIN)
                    nValue_List = cDataInfo.m_cStatisticData.m_nRawNMin_List;

                sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2.ToString("x2").ToUpper()));
                sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_DFT_NUM.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_Gain.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nNCPValue.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nNCNValue.ToString()));

                sw.Write(",");

                for (int nTraceIndex = 0; nTraceIndex < cDataInfo.m_nRXTraceNumber; nTraceIndex++)
                {
                    if (sDataType == m_sDATATYPE_RAWPMAX ||
                        sDataType == m_sDATATYPE_RAWPMIN ||
                        sDataType == m_sDATATYPE_RAWNMAX ||
                        sDataType == m_sDATATYPE_RAWNMIN)
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

                if (sDataType == m_sKVALUETYPE_NCP)
                    nValue_Array = cDataInfo.m_nNCPValue_Array;
                else if (sDataType == m_sKVALUETYPE_NCN)
                    nValue_Array = cDataInfo.m_nNCPValue_Array;

                sw.Write(string.Format("{0},", (nDataIndex + 1).ToString()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cDataInfo.m_n_SELF_PH2.ToString("x2").ToUpper()));
                sw.Write(string.Format("{0},", cDataInfo.m_dFrequency.ToString("0.000")));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_DFT_NUM.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nSelf_Gain.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nNCPValue.ToString()));
                sw.Write(string.Format("{0},", cDataInfo.m_nNCNValue.ToString()));

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

        private void WriteSlopeValueData(StreamWriter sw, List<string> sColumnHeader_List, List<SetDataInfo> cSetDataInfo_List, string sDataType, bool bLineSpace)
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

            for (int nDataIndex = 0; nDataIndex < cSetDataInfo_List.Count; nDataIndex++)
            {
                SetDataInfo cSetDataInfo = cSetDataInfo_List[nDataIndex];

                double[,] dValue_Array = null;
                int nKValueCount = 0;
                int nMinKValue = 0;

                if (sDataType == "Raw P Mean Slope")
                {
                    dValue_Array = cSetDataInfo.m_dRawPMeanSlope_Array;
                    nKValueCount = cSetDataInfo.m_nNCPMax - cSetDataInfo.m_nNCPMin;
                    nMinKValue = cSetDataInfo.m_nNCPMin;
                }
                else if (sDataType == "Raw N Mean Slope")
                {
                    dValue_Array = cSetDataInfo.m_dRawNMeanSlope_Array;
                    nKValueCount = cSetDataInfo.m_nNCNMax - cSetDataInfo.m_nNCNMin;
                    nMinKValue = cSetDataInfo.m_nNCNMin;
                }

                for (int nKValueIndex = 0; nKValueIndex < nKValueCount; nKValueIndex++)
                {
                    if (nKValueIndex == 0)
                    {
                        sw.Write(string.Format("{0},", cSetDataInfo.m_nSetIndex));
                        sw.Write(string.Format("0x{0},", cSetDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper()));
                        sw.Write(string.Format("0x{0},", cSetDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                        sw.Write(string.Format("0x{0},", cSetDataInfo.m_n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                        sw.Write(string.Format("0x{0},", cSetDataInfo.m_n_SELF_PH2.ToString("x2").ToUpper()));
                        sw.Write(string.Format("{0},", cSetDataInfo.m_dFrequency.ToString("0.000")));
                        sw.Write(string.Format("{0},", cSetDataInfo.m_nSelf_DFT_NUM.ToString()));
                        sw.Write(string.Format("{0},", cSetDataInfo.m_nSelf_Gain.ToString()));
                    }
                    else
                    {
                        sw.Write(",,,,,,,,");
                    }

                    sw.Write(string.Format("{0},", (nKValueIndex + 1 + nMinKValue).ToString()));

                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < cSetDataInfo.m_nRXTraceNumber; nTraceIndex++)
                    {
                        if (nTraceIndex == cSetDataInfo.m_nRXTraceNumber - 1)
                            sw.WriteLine(string.Format("{0}", dValue_Array[nKValueIndex, nTraceIndex]));
                        else
                            sw.Write(string.Format("{0},", dValue_Array[nKValueIndex, nTraceIndex]));
                    }
                }
            }

            if (bLineSpace == true)
                sw.WriteLine();
        }

        private void WriteFitData(StreamWriter sw, List<string> sColumnHeader_List, List<SetDataInfo> cSetDataInfo_List, string sDataType, bool bLineSpace)
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

            for (int nDataIndex = 0; nDataIndex < cSetDataInfo_List.Count; nDataIndex++)
            {
                SetDataInfo cSetDataInfo = cSetDataInfo_List[nDataIndex];

                int[] nValue_Array = null;

                if (sDataType == "Fit NCP Value")
                    nValue_Array = cSetDataInfo.m_nFitNCP_Array;
                else if (sDataType == "Fit NCN Value")
                    nValue_Array = cSetDataInfo.m_nFitNCN_Array;

                sw.Write(string.Format("{0},", cSetDataInfo.m_nSetIndex));
                sw.Write(string.Format("0x{0},", cSetDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cSetDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cSetDataInfo.m_n_SELF_PH2_LAT.ToString("x2").ToUpper()));
                sw.Write(string.Format("0x{0},", cSetDataInfo.m_n_SELF_PH2.ToString("x2").ToUpper()));
                sw.Write(string.Format("{0},", cSetDataInfo.m_dFrequency.ToString("0.000")));
                sw.Write(string.Format("{0},", cSetDataInfo.m_nSelf_DFT_NUM.ToString()));
                sw.Write(string.Format("{0},", cSetDataInfo.m_nSelf_Gain.ToString()));
                sw.Write(string.Format("{0},", cSetDataInfo.m_nNCPMax.ToString()));
                sw.Write(string.Format("{0},", cSetDataInfo.m_nNCPMin.ToString()));
                sw.Write(string.Format("{0},", cSetDataInfo.m_nNCNMax.ToString()));
                sw.Write(string.Format("{0},", cSetDataInfo.m_nNCNMin.ToString()));

                sw.Write(",");

                for (int nTraceIndex = 0; nTraceIndex < cSetDataInfo.m_nRXTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex == cSetDataInfo.m_nRXTraceNumber - 1)
                        sw.WriteLine(string.Format("{0}", nValue_Array[nTraceIndex]));
                    else
                        sw.Write(string.Format("{0},", nValue_Array[nTraceIndex]));
                }
            }

            if (bLineSpace == true)
                sw.WriteLine();
        }

        private bool SaveNCPNCNTable()
        {
            bool bError = false;

            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<SetDataInfo> cSetDataInfo_List = new List<SetDataInfo>();
                int nTraceNumber = 0;

                if (eTraceType == TraceType.RX)
                {
                    cSetDataInfo_List = m_cSetDataInfo_RX_List;
                    nTraceNumber = m_nTraceNumber_RX;
                }
                else if (eTraceType == TraceType.TX)
                {
                    cSetDataInfo_List = m_cSetDataInfo_TX_List;
                    nTraceNumber = m_nTraceNumber_TX;
                }

                string sReportFilePath = string.Format(@"{0}\NCPNCNTable_{1}.txt", m_sLogDirectoryPath, eTraceType.ToString());

                FileStream fs = new FileStream(sReportFilePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                try
                {
                    for (int nDataIndex = 0; nDataIndex < cSetDataInfo_List.Count; nDataIndex++)
                    {
                        SetDataInfo cSetDataInfo = cSetDataInfo_List[nDataIndex];

                        sw.WriteLine("====================");
                        sw.WriteLine(string.Format("SetIndex = {0}", cSetDataInfo.m_nSetIndex));
                        sw.WriteLine("====================");
                        sw.WriteLine(string.Format("Frequency(KHz) = {0}", cSetDataInfo.m_dFrequency));
                        sw.WriteLine(string.Format("SELF_PH1 = 0x{0}", cSetDataInfo.m_n_SELF_PH1.ToString("X2")));
                        sw.WriteLine(string.Format("SELF_PH2E_LMT = 0x{0}", cSetDataInfo.m_n_SELF_PH2E_LMT.ToString("X2")));
                        sw.WriteLine(string.Format("SELF_PH2_LAT = 0x{0}", cSetDataInfo.m_n_SELF_PH2_LAT.ToString("X2")));
                        sw.WriteLine(string.Format("SELF_PH2 = 0x{0}", cSetDataInfo.m_n_SELF_PH2.ToString("X2")));
                        sw.WriteLine(string.Format("SELF_DFT_NUM = {0}", cSetDataInfo.m_nSelf_DFT_NUM));
                        sw.WriteLine(string.Format("SELF_SELGM = {0}", cSetDataInfo.m_nSelf_Gain));

                        sw.WriteLine();
                        sw.WriteLine("Data:");
                        sw.WriteLine("//_MS_MM_RX0");

                        int nTraceIndex = 0;

                        for (int nPinIndex = 0; nPinIndex < m_n_MS_MM_RX0Count; nPinIndex++)
                        {
                            if (nPinIndex < m_nStartTrace)
                                sw.WriteLine("0x0000,");
                            else
                            {
                                if (nTraceIndex >= cSetDataInfo.m_nRXTraceNumber)
                                    sw.WriteLine("0x0000,");
                                else
                                {
                                    int nNCP = cSetDataInfo.m_nFitNCP_Array[nTraceIndex] << 9;
                                    int nNCN = cSetDataInfo.m_nFitNCN_Array[nTraceIndex] << 4;
                                    int n_MS_MM_RX0 = nNCP + nNCN;

                                    sw.WriteLine(string.Format("0x{0:0000},", n_MS_MM_RX0.ToString("X4")));

                                    nTraceIndex++;
                                }
                            }
                        }

                        if (nDataIndex < cSetDataInfo_List.Count - 1)
                            sw.WriteLine();
                    }
                }
                catch (IOException ex)
                {
                    string sMessage = ex.Message;

                    m_sErrorMessage = string.Format("Save {0} NCPNCNTable Data Error", eTraceType.ToString());
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

        private bool SaveLineChart()
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                List<DataInfo> cDataInfo_List = new List<DataInfo>();
                List<SetDataInfo> cSetDataInfo_List = new List<SetDataInfo>();
                int nTraceNumber = 0;

                if (eTraceType == TraceType.RX)
                {
                    cDataInfo_List = m_cDataInfo_RX_List;
                    cSetDataInfo_List = m_cSetDataInfo_RX_List;
                    nTraceNumber = m_nTraceNumber_RX;
                }
                else if (eTraceType == TraceType.TX)
                {
                    cDataInfo_List = m_cDataInfo_TX_List;
                    cSetDataInfo_List = m_cSetDataInfo_TX_List;
                    nTraceNumber = m_nTraceNumber_TX;
                }

                m_nCompareOperator = m_nCOMPARE_SetIndex;
                cDataInfo_List.Sort(new InfoDataComparer());

                string sDirectoryPath = string.Format(@"{0}\{1}\{2}", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART, eTraceType.ToString());

                if (Directory.Exists(sDirectoryPath) == false)
                    Directory.CreateDirectory(sDirectoryPath);

                foreach (SetDataInfo cSetDataInfo in cSetDataInfo_List)
                {
                    if (CreateLineChartByNCPNCN(eTraceType, sDirectoryPath, nTraceNumber, cSetDataInfo, cDataInfo_List) == false)
                        return false;
                }
            }

            return true;
        }

        private bool CreateLineChartByNCPNCN(TraceType eTraceType, string sDirectoryPath, int nTraceNumber, SetDataInfo cSetDataInfo, List<DataInfo> cDataInfo_List)
        {
            List<DataInfo> cDataInfo_SetIndex_List = cDataInfo_List.FindAll(x => x.m_nSetIndex == cSetDataInfo.m_nSetIndex);

            for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
            {
                string sTitleName = string.Format("Raw P/N Mean, Max, Min By NCP/NCN_Trace{0}", nTraceIndex + 1);
                string sYValueTitle = "Raw P/N Value";
                string sFileName = string.Format("RawPNChartByNCPNCN_Trace{0}_{1}", nTraceIndex + 1, cSetDataInfo.m_nSetIndex);

                double dMax = 0.0;
                double dMin = 0.0;
                GetMaxMinValue(ref dMax, ref dMin, nTraceIndex, cDataInfo_SetIndex_List);

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

                string[] sDataType_Array = new string[] 
                { 
                    m_sDATATYPE_RAWPMEAN,
                    m_sDATATYPE_RAWPMAX,                        
                    m_sDATATYPE_RAWPMIN,          
                    m_sDATATYPE_RAWNMEAN,                                
                    m_sDATATYPE_RAWNMAX,
                    m_sDATATYPE_RAWNMIN 
                };
                
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
                cChart.ChartAreas[0].AxisY.Title = sYValueTitle;
                cChart.ChartAreas[0].AxisY.TitleFont = new Font("Times New Roman", 14);
                cChart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Times New Roman", 12);
                cChart.ChartAreas[0].AxisX.Title = "NCP/NCN Value";
                cChart.ChartAreas[0].AxisX.TitleFont = new Font("Times New Roman", 14);
                cChart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Times New Roman", 12);
                cChart.ChartAreas[0].AxisX.Interval = 2;
                cChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                cChart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;
                cChart.Titles[0].Position.X = 50;
                cChart.Titles[0].Position.Height = 10;
                cChart.ChartAreas[0].Position.X = 0;
                cChart.ChartAreas[0].Position.Y = 50;
                cChart.ChartAreas[0].Position.Width = 77;
                cChart.ChartAreas[0].Position.Height = 90;
                cChart.ChartAreas[0].AxisX.Minimum = (cSetDataInfo.m_nNCPMin < cSetDataInfo.m_nNCNMin) ? cSetDataInfo.m_nNCPMin : cSetDataInfo.m_nNCNMin;
                cChart.ChartAreas[0].AxisX.Maximum = (cSetDataInfo.m_nNCPMax > cSetDataInfo.m_nNCNMax) ? cSetDataInfo.m_nNCPMax : cSetDataInfo.m_nNCNMax;
                cChart.ChartAreas[0].AxisX.Interval = 5;

                if (nValueLength > 1)
                    cChart.ChartAreas[0].AxisY.Minimum = nYMinimumValue;

                cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
                cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                cChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
                cChart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

                /*
                cChart.Legends["Legend"].Position.X = 78;
                cChart.Legends["Legend"].Position.Y = 0;
                cChart.Legends["Legend"].Position.Width = 22;
                */

                foreach (string sDataType in sDataType_Array)
                {
                    Series cSeries = new Series(sDataType);

                    if (sDataType == m_sDATATYPE_RAWPMEAN ||
                        sDataType == m_sDATATYPE_RAWNMEAN)
                    {
                        cSeries.ChartType = SeriesChartType.Line;
                        cSeries.IsValueShownAsLabel = false;
                        cSeries.MarkerStyle = MarkerStyle.Circle;
                        cSeries.BorderWidth = 3;
                        cSeries.MarkerSize = 10;

                        if (sDataType == m_sDATATYPE_RAWPMEAN)
                            cSeries.Color = Color.Blue;
                        else if (sDataType == m_sDATATYPE_RAWNMEAN)
                            cSeries.Color = Color.Red;
                    }
                    else
                    {
                        cSeries.ChartType = SeriesChartType.Line;
                        cSeries.IsValueShownAsLabel = false;
                        cSeries.BorderDashStyle = ChartDashStyle.Dash;
                        cSeries.BorderWidth = 3;

                        if (sDataType == m_sDATATYPE_RAWPMAX ||
                            sDataType == m_sDATATYPE_RAWPMIN)
                            cSeries.Color = Color.LightBlue;
                        else if (sDataType == m_sDATATYPE_RAWNMAX ||
                                 sDataType == m_sDATATYPE_RAWNMIN)
                            cSeries.Color = Color.Pink;
                    }

                    for (int nDataIndex = 0; nDataIndex < cDataInfo_SetIndex_List.Count; nDataIndex++)
                    {
                        DataInfo cDataInfo = cDataInfo_SetIndex_List[nDataIndex];

                        if (sDataType == m_sDATATYPE_RAWPMEAN)
                            cSeries.Points.AddXY(cDataInfo.m_nNCPValue, cDataInfo.m_cStatisticData.m_dRawPMean_List[nTraceIndex]);
                        else if (sDataType == m_sDATATYPE_RAWNMEAN)
                            cSeries.Points.AddXY(cDataInfo.m_nNCNValue, cDataInfo.m_cStatisticData.m_dRawNMean_List[nTraceIndex]);
                        else if (sDataType == m_sDATATYPE_RAWPMAX)
                            cSeries.Points.AddXY(cDataInfo.m_nNCPValue, cDataInfo.m_cStatisticData.m_nRawPMax_List[nTraceIndex]);
                        else if (sDataType == m_sDATATYPE_RAWPMIN)
                            cSeries.Points.AddXY(cDataInfo.m_nNCPValue, cDataInfo.m_cStatisticData.m_nRawPMin_List[nTraceIndex]);
                        else if (sDataType == m_sDATATYPE_RAWNMAX)
                            cSeries.Points.AddXY(cDataInfo.m_nNCNValue, cDataInfo.m_cStatisticData.m_nRawNMax_List[nTraceIndex]);
                        else if (sDataType == m_sDATATYPE_RAWNMIN)
                            cSeries.Points.AddXY(cDataInfo.m_nNCNValue, cDataInfo.m_cStatisticData.m_nRawNMin_List[nTraceIndex]);
                    }

                    cChart.Series.Add(cSeries);
                }

                /*
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
                */

                try
                {
                    cChart.SaveImage(sFilePath, ChartImageFormat.Jpeg);
                }
                catch
                {
                    m_sErrorMessage = string.Format("Save Chart File Error[{0}]", sFileName);
                    return false;
                }
            }

            return true;
        }

        private void GetMaxMinValue(ref double dMax, ref double dMin, int nTraceIndex, List<DataInfo> cDataInfo_List)
        {
            for (int nDataIndex = 0; nDataIndex < cDataInfo_List.Count; nDataIndex++)
            {
                int nRawPMax = cDataInfo_List[nDataIndex].m_cStatisticData.m_nRawPMax_List[nTraceIndex];
                int nRawPMin = cDataInfo_List[nDataIndex].m_cStatisticData.m_nRawPMin_List[nTraceIndex];
                int nRawNMax = cDataInfo_List[nDataIndex].m_cStatisticData.m_nRawNMax_List[nTraceIndex];
                int nRawNMin = cDataInfo_List[nDataIndex].m_cStatisticData.m_nRawNMin_List[nTraceIndex];

                if (nRawPMax > nRawNMax)
                {
                    if (nDataIndex == 0)
                    {
                        dMax = nRawPMax;
                    }
                    else
                    {
                        if (nRawPMax > dMax)
                            dMax = nRawPMax;
                    }
                }
                else
                {
                    if (nDataIndex == 0)
                    {
                        dMax = nRawNMax;
                    }
                    else
                    {
                        if (nRawNMax > dMax)
                            dMax = nRawNMax;
                    }
                }

                if (nRawPMin < nRawNMin)
                {
                    if (nDataIndex == 0)
                    {
                        dMin = nRawPMin;
                    }
                    else
                    {
                        if (nRawPMin < dMin)
                            dMin = nRawPMin;
                    }
                }
                else
                {
                    if (nDataIndex == 0)
                    {
                        dMin = nRawNMin;
                    }
                    else
                    {
                        if (nRawNMin < dMin)
                            dMin = nRawNMin;
                    }
                }
            }
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
    }
}
