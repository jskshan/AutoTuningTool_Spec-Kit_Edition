using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    public class SetFrequencyItemList
    {
        private frmMain m_cfrmMain = null;

        private const int m_nVALUETYPE_Hex = 0;
        private const int m_nVALUETYPE_Int = 1;

        private List<FrequencyItem> m_cFreqencyItem_List = new List<FrequencyItem>();

        private string m_sErrorMessage = "";
        public string ErrorMessage
        {
            get { return m_sErrorMessage; }
        }

        private bool m_bPassFlag = true;
        public bool PassFlag
        {
            get { return m_bPassFlag; }
        }

        public SetFrequencyItemList(frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;

            m_cFreqencyItem_List.Clear();
        }

        private class SelfInfo
        {
            public int m_nPH1 = 0;
            public int m_nPH2E_LAT = 0;
            public int m_nPH2E_LMT = 0;
            public int m_nPH2_LAT = 0;
            public int m_nPH2 = 0;

            public int m_nDFT_NUM = 0;
            public int m_nGain = -1;
            public int m_nCAL = 0;
            public int m_nCAG = -1;
            public int m_nIQ_BSH = -1;
            public double m_dSmapleTime = 0.0;
        }

        public List<FrequencyItem> SetFrequencyItem_FRPH1()
        {
            double dFrequency_LB = ParamFingerAutoTuning.m_dFRPH1ScanFrequencyLB;
            double dFrequency_HB = ParamFingerAutoTuning.m_dFRPH1ScanFrequencyHB;
            int nPH1FixedValue = ParamFingerAutoTuning.m_nFRPH1FixedPH1;
            int nPH2IntervalValue = 1;

            int nPH1PH2_HB = ElanConvert.Convert2PH1PH2SumInt(dFrequency_LB);
            double fPH1PH2_LB = ElanConvert.Convert2PH1PH2SumDouble(dFrequency_HB);
            int nPH1PH2_LB = (int)fPH1PH2_LB;

            if (nPH1PH2_LB < fPH1PH2_LB)
                nPH1PH2_LB = nPH1PH2_LB + 1;

            //Create the PH1/PH2 List
            List<int> nPH1_List = new List<int>();
            List<int> nPH2_List = new List<int>();

            int nPH1Value = nPH1FixedValue;

            int nPH2_LB = nPH1PH2_LB - nPH1Value;
            int nPH2_HB = nPH1PH2_HB - nPH1Value;

            for (int nPH2Value = nPH2_LB; nPH2Value <= nPH2_HB; nPH2Value += nPH2IntervalValue)
            {
                nPH1_List.Add(nPH1Value);
                nPH2_List.Add(nPH2Value);
            }

            if ((nPH2_HB - nPH2_LB) % nPH2IntervalValue != 0)
            {
                nPH1_List.Add(nPH1Value);
                nPH2_List.Add(nPH2_HB);
            }

            int nSetIndex = 0;

            for (int nPH1Index = 0; nPH1Index < nPH1_List.Count; nPH1Index++)
            {
                int nPH1SetValue = nPH1_List[nPH1Index];
                int nPH2SetValue = nPH2_List[nPH1Index];

                if (nPH1SetValue > nPH2SetValue)
                    continue;

                double dFrequency = ElanConvert.Convert2Frequency(nPH1SetValue, nPH2SetValue);

                if (dFrequency < dFrequency_LB || dFrequency > dFrequency_HB)
                    continue;

                FrequencyItem cFrequencyItem = new FrequencyItem();
                cFrequencyItem.m_nSetIndex = nSetIndex;
                cFrequencyItem.m_nPH1 = nPH1SetValue;
                cFrequencyItem.m_nPH2 = nPH2SetValue;
                cFrequencyItem.m_nADCTestFrame = ParamFingerAutoTuning.m_nFRPH1TestFrame;
                cFrequencyItem.m_dFrequency = dFrequency;

                m_cFreqencyItem_List.Add(cFrequencyItem);

                nSetIndex++;
            }

            if (ParamFingerAutoTuning.m_nFRPH1FrequencySortType == 1)
                m_cFreqencyItem_List.Reverse();

            return m_cFreqencyItem_List;
        }

        private List<int> ReadSkipFrequencyList(string sSkipFrequencyPath)
        {
            List<int> nSkipPH1PH2Sum_List = new List<int>();

            if (!File.Exists(sSkipFrequencyPath))
                return nSkipPH1PH2Sum_List;

            using (StreamReader srFile = new StreamReader(sSkipFrequencyPath, Encoding.Default))
            {
                string sLine = "";
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSplit_Array = sLine.Split(',');

                    if (sSplit_Array.Length >= 2 && ElanConvert.IsInt(sSplit_Array[1]))
                    {
                        nSkipPH1PH2Sum_List.Add(Convert.ToInt32(sSplit_Array[1]));
                    }
                }
            }

            return nSkipPH1PH2Sum_List;
        }

        public List<FrequencyItem> SetFrequencyItem_FRPH2_ACFR(frmMain.FlowStep cFlowStep, string sFrequencyPath, string sSkipFrequencyPath)
        {
            /*
            double dFrequency_LB = ParamFingerAutoTuning.m_fFRTestFreqLB;
            double dFrequency_HB = ParamFingerAutoTuning.m_fFRTestFreqHB;
            */

            int nFRPH2MinPH2 = -1;
            int nFRPH2FixedPH1 = -1;
            bool bEnableTXn = false;
            int nEnableTXn = -1;
            string sStepDirectoryPath = "";
            string sStepDirectoryName = "";
            string sFRPH1Name = StringConvert.m_dictMainStepMappingTable[MainStep.FrequencyRank_Phase1];
            string sFRPH1CodeName = StringConvert.m_dictMainStepCodeNameMappingTable[MainStep.FrequencyRank_Phase1];

            if (ParamFingerAutoTuning.m_nSkipFRPH1Step != 1)
            {
                if (File.Exists(m_cfrmMain.m_sResultListFilePath) == true)
                {
                    IniFileFormat.GetStepDirectoryInfo(ref sStepDirectoryPath, ref sStepDirectoryName, m_cfrmMain, MainStep.FrequencyRank_Phase1);

                    string sFRPH1Path = string.Format(@"{0}\{1}", sStepDirectoryPath, sFRPH1CodeName);

                    if (Directory.Exists(sFRPH1Path) == false)
                    {
                        m_sErrorMessage = string.Format("\"{0}\" Log Not Exist. Please Run \"{1}\" Step First", sFRPH1CodeName, sFRPH1Name);
                        m_bPassFlag = false;
                        return m_cFreqencyItem_List;
                    }
                }
                else
                {
                    m_sErrorMessage = string.Format("ResultList File Not Exist. Please Run \"{0}\" Step First", sFRPH1Name);
                    m_bPassFlag = false;
                    return m_cFreqencyItem_List;
                }

                string sFRPH1ReportPath = string.Format(@"{0}\{1}\Report.csv", sStepDirectoryPath, sFRPH1CodeName);

                if (File.Exists(sFRPH1ReportPath) == true)
                {
                    string sLine = "";

                    StreamReader srFile = new StreamReader(sFRPH1ReportPath, Encoding.Default);

                    try
                    {
                        while ((sLine = srFile.ReadLine()) != null)
                        {
                            string[] sSplit_Array = sLine.Split(',');

                            if (sSplit_Array.Length >= 3)
                            {
                                if (GetFileParameterHexValue(ref nFRPH2FixedPH1, sSplit_Array, "PH1") == false)
                                {
                                    m_bPassFlag = false;
                                    return m_cFreqencyItem_List;
                                }
                                else if (GetFileParameterHexValue(ref nFRPH2MinPH2, sSplit_Array, "PH2") == false)
                                {
                                    m_bPassFlag = false;
                                    return m_cFreqencyItem_List;
                                }
                            }
                            else if (sSplit_Array.Length >= 2)
                            {
                                if (GetFileBoolValue(ref bEnableTXn, ref nEnableTXn, sSplit_Array, "EnableTXn") == false)
                                {
                                    m_bPassFlag = false;
                                    return m_cFreqencyItem_List;
                                }
                            }

                            if (nFRPH2FixedPH1 > -1 && nFRPH2MinPH2 > -1 && nEnableTXn > -1)
                                break;
                        }
                    }
                    finally
                    {
                        srFile.Close();
                    }
                }
                else
                {
                    m_sErrorMessage = string.Format("\"{0}\" Report File Not Exist. Please Run \"{1}\" Step First", sFRPH1CodeName, sFRPH1Name);
                    m_bPassFlag = false;
                    return m_cFreqencyItem_List;
                }
            }
            else
            {
                nFRPH2FixedPH1 = ParamFingerAutoTuning.m_nManualFixedPH1;
                nFRPH2MinPH2 = ParamFingerAutoTuning.m_nManualMinimumPH2;
                nEnableTXn = ParamFingerAutoTuning.m_nManualEnableTXn;
            }

            // 使用新的函式讀取 Skip 清單
            List<int> nSkipPH1PH2Sum_List = ReadSkipFrequencyList(sSkipFrequencyPath);

            if (File.Exists(sFrequencyPath) == true)
            {
                string sLine = "";
                List<int> nPH1PH2Sum_List = new List<int>();

                StreamReader srFile = new StreamReader(sFrequencyPath, Encoding.Default);

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplit_Array = sLine.Split(',');

                        if (sSplit_Array.Length >= 2)
                        {
                            if (ElanConvert.IsInt(sSplit_Array[1]) == true)
                                nPH1PH2Sum_List.Add(Convert.ToInt32(sSplit_Array[1]));
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }

                nPH1PH2Sum_List = nPH1PH2Sum_List.Distinct().ToList();

                int nPH1PH2Sum = nFRPH2FixedPH1 + nFRPH2MinPH2;

                //Create the PH1/PH2 List
                List<int> nPH1_List = new List<int>();
                List<int> nPH2_List = new List<int>();

                for (int nIndex = 0; nIndex < nPH1PH2Sum_List.Count; nIndex++)
                {
                    // 如果此值在 Skip 清單中，則跳過
                    if (nSkipPH1PH2Sum_List.Contains(nPH1PH2Sum_List[nIndex]))
                        continue;

                    if (nPH1PH2Sum_List[nIndex] >= nPH1PH2Sum)
                    {
                        int nPH1Value = nPH1PH2Sum_List[nIndex] - nFRPH2MinPH2;
                        int nPH2Value = nFRPH2MinPH2;

                        nPH1_List.Add(nPH1Value);
                        nPH2_List.Add(nPH2Value);
                    }
                }

                for (int nPH1Index = 0; nPH1Index < nPH1_List.Count; nPH1Index++)
                {
                    int nPH1SetValue = nPH1_List[nPH1Index];
                    int nPH2SetValue = nPH2_List[nPH1Index];

                    double dFrequency = ElanConvert.Convert2Frequency(nPH1SetValue, nPH2SetValue);

                    /*
                    if (dFrequency < dFrequencyLB || dFrequency > dFrequencyHB)
                        continue;
                    */

                    FrequencyItem cFrequencyItem = new FrequencyItem();
                    cFrequencyItem.m_nSetIndex = nPH1Index;
                    cFrequencyItem.m_nPH1 = nPH1SetValue;
                    cFrequencyItem.m_nPH2 = nPH2SetValue;
                    cFrequencyItem.m_bEnableTXn = bEnableTXn;

                    if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2)
                        cFrequencyItem.m_nADCTestFrame = ParamFingerAutoTuning.m_nFRPH2ADCTestFrame;
                    else if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                        cFrequencyItem.m_nADCTestFrame = ParamFingerAutoTuning.m_nACFRADCTestFrame;

                    cFrequencyItem.m_nDVTestFrame = ParamFingerAutoTuning.m_nFRPH2DVTestFrame;
                    cFrequencyItem.m_dFrequency = dFrequency;

                    m_cFreqencyItem_List.Add(cFrequencyItem);
                }
            }

            return m_cFreqencyItem_List;
        }

        public List<FrequencyItem> SetFrequencyItem_ACFR(string m_sACFrequencyPath, string sSkipFrequencyPath)
        {
            int nTXTraceNumber = 0;
            int nRXTraceNumber = 0;
            int nFRPH2MinPH2 = -1;
            int nFRPH2FixedPH1 = -1;
            bool bEnableTXn = false;
            int nEnableTXn = -1;
            string sStepDirectoryPath = "";
            string sStepDirectoryName = "";
            string sFRPH2Name = StringConvert.m_dictMainStepMappingTable[MainStep.FrequencyRank_Phase2];
            string sFRPH2CodeName = StringConvert.m_dictMainStepCodeNameMappingTable[MainStep.FrequencyRank_Phase2];

            bool bGetRankIndex = false;
            List<int> nSelectRankIndex_List = new List<int>();

            // 使用新的函式讀取 Skip 清單
            List<int> nSkipPH1PH2Sum_List = ReadSkipFrequencyList(sSkipFrequencyPath);

            if (File.Exists(m_sACFrequencyPath) == true)
            {
                string sLine = "";

                StreamReader srFile = new StreamReader(m_sACFrequencyPath, Encoding.Default);

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplit_Array = sLine.Split(',');

                        if (sSplit_Array.Length >= 1)
                        {
                            if (ElanConvert.IsInt(sSplit_Array[0]) == true)
                                nSelectRankIndex_List.Add(Convert.ToInt32(sSplit_Array[0]));
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }

                bGetRankIndex = true;
            }

            if (File.Exists(m_cfrmMain.m_sResultListFilePath) == true)
            {
                IniFileFormat.GetStepDirectoryInfo(ref sStepDirectoryPath, ref sStepDirectoryName, m_cfrmMain, MainStep.FrequencyRank_Phase2);

                string sFRPH2Path = string.Format(@"{0}\{1}", sStepDirectoryPath, sFRPH2CodeName);

                if (Directory.Exists(sFRPH2Path) == false)
                {
                    m_sErrorMessage = string.Format("\"{0}\" Log Not Exist. Please Run \"{1}\" Step First", sFRPH2CodeName, sFRPH2Name);
                    m_bPassFlag = false;
                    return m_cFreqencyItem_List;
                }
            }
            else
            {
                m_sErrorMessage = string.Format("ResultList File Not Exist. Please Run \"{0}\" Step First", sFRPH2Name);
                m_bPassFlag = false;
                return m_cFreqencyItem_List;
            }

            string sFRPH2ReportPath = string.Format(@"{0}\{1}\Report.csv", sStepDirectoryPath, sFRPH2CodeName);

            if (File.Exists(sFRPH2ReportPath) == true)
            {
                string sLine = "";

                StreamReader srFile = new StreamReader(sFRPH2ReportPath, Encoding.Default);

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplit_Array = sLine.Split(',');

                        if (sSplit_Array.Length >= 2)
                        {
                            if (GetFileParameterValue(ref nTXTraceNumber, sSplit_Array, "TXTraceNumber", m_nVALUETYPE_Int) == false)
                            {
                                m_bPassFlag = false;
                                return m_cFreqencyItem_List;
                            }
                            else if (GetFileParameterValue(ref nRXTraceNumber, sSplit_Array, "RXTraceNumber", m_nVALUETYPE_Int) == false)
                            {
                                m_bPassFlag = false;
                                return m_cFreqencyItem_List;
                            }
                            else if (GetFileParameterValue(ref nFRPH2FixedPH1, sSplit_Array, "MinPH1(Hex)", m_nVALUETYPE_Hex) == false)
                            {
                                m_bPassFlag = false;
                                return m_cFreqencyItem_List;
                            }
                            else if (GetFileParameterValue(ref nFRPH2MinPH2, sSplit_Array, "PH2LB(Hex)", m_nVALUETYPE_Hex) == false)
                            {
                                m_bPassFlag = false;
                                return m_cFreqencyItem_List;
                            }
                            else if (GetFileBoolValue(ref bEnableTXn, ref nEnableTXn, sSplit_Array, "EnableTXn") == false)
                            {
                                m_bPassFlag = false;
                                return m_cFreqencyItem_List;
                            }
                        }

                        if (nTXTraceNumber > 0 && nRXTraceNumber > 0 && nFRPH2FixedPH1 > -1 && nFRPH2MinPH2 > -1 && nEnableTXn > -1)
                            break;
                    }
                }
                finally
                {
                    srFile.Close();
                }
            }
            else
            {
                m_sErrorMessage = string.Format("\"{0}\" Report File Not Exist. Please Run \"{1}\" Step First", sFRPH2CodeName, sFRPH2Name);
                m_bPassFlag = false;
                return m_cFreqencyItem_List;
            }

            DataTable datatableData = null;

            try
            {
                datatableData = StringConvert.ConvertCsvToDataTable(sFRPH2ReportPath, "Frequency Rank:");
            }
            catch
            {
                m_sErrorMessage = string.Format("Get Data Table Error in \"{0}\" Report File. Please Run \"{1}\" Step First", sFRPH2CodeName, sFRPH2Name);
                m_bPassFlag = false;
                return m_cFreqencyItem_List;
            }

            string[] sColumnTitle_Array = new string[] { "Rank", "Frequency(KHz)", "PH1+PH2" };

            for (int nIndex = 0; nIndex < sColumnTitle_Array.Length; nIndex++)
            {
                if (datatableData.Columns.Contains(sColumnTitle_Array[nIndex]) == false)
                {
                    m_sErrorMessage = string.Format("Get Column Title:{0} Error", sColumnTitle_Array[nIndex]);
                    m_bPassFlag = false;
                    return m_cFreqencyItem_List;
                }
            }

            for (int nRowIndex = 0; nRowIndex < datatableData.Rows.Count; nRowIndex++)
            {
                int nRankIndex = Convert.ToInt32(datatableData.Rows[nRowIndex]["Rank"]);

                bool bMath = false;

                if (bGetRankIndex == true)
                {
                    for (int nDataIndex = 0; nDataIndex < nSelectRankIndex_List.Count; nDataIndex++)
                    {
                        if (nRankIndex == nSelectRankIndex_List[nDataIndex])
                        {
                            bMath = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (nRankIndex <= ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber)
                        bMath = true;
                }

                if (bMath == true)
                {
                    double dFrequency = Convert.ToDouble(datatableData.Rows[nRowIndex]["Frequency(KHz)"]);
                    string sPH1PH2Sum = datatableData.Rows[nRowIndex]["PH1+PH2"].ToString();

                    string sToken = "0x";
                    int nTokenIndex = sPH1PH2Sum.IndexOf(sToken);
                    int nPH1PH2Sum = 0;

                    if (nTokenIndex < 0)
                    {
                        m_sErrorMessage = string.Format("Get PH1+PH2 Value Error(Index={0})", nRowIndex + 1);
                        m_bPassFlag = false;
                        return m_cFreqencyItem_List;
                    }
                    else
                    {
                        string sPH1PH2SumValue = sPH1PH2Sum.Substring(nTokenIndex + sToken.Length);

                        if (ElanConvert.IsHexadecimal(sPH1PH2SumValue) == false)
                        {
                            m_sErrorMessage = string.Format("Get PH1+PH2 Value Format Error(Index={0})", nRowIndex + 1);
                            m_bPassFlag = false;
                            return m_cFreqencyItem_List;
                        }
                        else
                            nPH1PH2Sum = Convert.ToInt32(sPH1PH2SumValue, 16);
                    }

                    // 如果此值在 Skip 清單中，則跳過
                    if (nSkipPH1PH2Sum_List.Contains(nPH1PH2Sum))
                        continue;

                    int nPH1Value = nPH1PH2Sum - nFRPH2MinPH2;

                    FrequencyItem cItem = new FrequencyItem();
                    cItem.m_nSetIndex = nRowIndex;
                    cItem.m_nTXTraceNumber = nTXTraceNumber;
                    cItem.m_nRXTraceNumber = nRXTraceNumber;
                    cItem.m_nPH1 = nPH1Value;
                    cItem.m_nPH2 = nFRPH2MinPH2;
                    cItem.m_bEnableTXn = bEnableTXn;
                    cItem.m_nADCTestFrame = ParamFingerAutoTuning.m_nACFRADCTestFrame;
                    cItem.m_dFrequency = dFrequency;

                    m_cFreqencyItem_List.Add(cItem);
                }
            }

            return m_cFreqencyItem_List;
        }

        public List<FrequencyItem> SetFrequencyItem_SelfFS(string sFrequencyPath)
        {
            string sSelfFSName = StringConvert.m_dictMainStepMappingTable[MainStep.Self_FrequencySweep];
            string sSelfFSCodeName = StringConvert.m_dictMainStepCodeNameMappingTable[MainStep.Self_FrequencySweep];

            if (File.Exists(sFrequencyPath) == true)
            {
                string sLine = "";
                List<SelfInfo> cSelfInfo_List = new List<SelfInfo>();

                StreamReader srFile = new StreamReader(sFrequencyPath, Encoding.Default);

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplit_Array = sLine.Split('=');

                        if (sSplit_Array.Length > 2)
                        {
                            if (sSplit_Array[0] == "Name")
                                continue;
                            else if (sSplit_Array[0] == "CommandFile")
                                continue;
                            else if (sSplit_Array[0] == "GetReportSequence")
                                continue;
                        }

                        sSplit_Array = sLine.Split(',');

                        if (sSplit_Array.Length >= 8)
                        {
                            SelfInfo cSelfInfo = new SelfInfo();

                            if (ElanConvert.IsInt(sSplit_Array[1]) == true)
                                cSelfInfo.m_nPH1 = Convert.ToInt32(sSplit_Array[1]);

                            if (ElanConvert.IsInt(sSplit_Array[2]) == true)
                                cSelfInfo.m_nPH2E_LAT = Convert.ToInt32(sSplit_Array[2]);

                            if (ElanConvert.IsInt(sSplit_Array[3]) == true)
                                cSelfInfo.m_nPH2E_LMT = Convert.ToInt32(sSplit_Array[3]);

                            if (ElanConvert.IsInt(sSplit_Array[4]) == true)
                                cSelfInfo.m_nPH2_LAT = Convert.ToInt32(sSplit_Array[4]);

                            if (ElanConvert.IsInt(sSplit_Array[5]) == true)
                                cSelfInfo.m_nPH2 = Convert.ToInt32(sSplit_Array[5]);

                            if (ElanConvert.IsInt(sSplit_Array[6]) == true)
                                cSelfInfo.m_nDFT_NUM = Convert.ToInt32(sSplit_Array[6]);

                            if (ElanConvert.IsInt(sSplit_Array[7]) == true)
                                cSelfInfo.m_nGain = Convert.ToInt32(sSplit_Array[7]);

                            if (ElanConvert.IsInt(sSplit_Array[8]) == true)
                                cSelfInfo.m_nCAL = Convert.ToInt32(sSplit_Array[8]);

                            if (ElanConvert.IsInt(sSplit_Array[9]) == true)
                                cSelfInfo.m_nCAG = Convert.ToInt32(sSplit_Array[9]);

                            if (ElanConvert.IsInt(sSplit_Array[10]) == true)
                                cSelfInfo.m_nIQ_BSH = Convert.ToInt32(sSplit_Array[10]);

                            if (ElanConvert.IsDouble(sSplit_Array[11]) == true)
                                cSelfInfo.m_dSmapleTime = Convert.ToDouble(sSplit_Array[11]);

                            cSelfInfo_List.Add(cSelfInfo);
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }

                for (int nSetIndex = 0; nSetIndex < cSelfInfo_List.Count; nSetIndex++)
                {
                    SelfInfo cSelfInfo = cSelfInfo_List[nSetIndex];
                    int nPH1 = cSelfInfo.m_nPH1;
                    int nPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cSelfInfo.m_nPH2E_LAT, cSelfInfo.m_nPH2E_LMT, cSelfInfo.m_nPH2_LAT, cSelfInfo.m_nPH2);

                    double dFrequency = ElanConvert.Convert2Frequency(nPH1, nPH2Sum);

                    FrequencyItem cFrequencyItem = new FrequencyItem();
                    cFrequencyItem.m_nSetIndex = nSetIndex;
                    cFrequencyItem.m_n_SELF_PH1 = cSelfInfo.m_nPH1;
                    cFrequencyItem.m_n_SELF_PH2E_LAT = cSelfInfo.m_nPH2E_LAT;
                    cFrequencyItem.m_n_SELF_PH2E_LMT = cSelfInfo.m_nPH2E_LMT;
                    cFrequencyItem.m_n_SELF_PH2_LAT = cSelfInfo.m_nPH2_LAT;
                    cFrequencyItem.m_n_SELF_PH2 = cSelfInfo.m_nPH2;

                    cFrequencyItem.m_nDVTestFrame = ParamFingerAutoTuning.m_nSelfFSTestFrame;
                    cFrequencyItem.m_dSelf_Frequency = dFrequency;

                    cFrequencyItem.m_nSelf_DFT_NUM = cSelfInfo.m_nDFT_NUM;
                    cFrequencyItem.m_nSelf_Gain = cSelfInfo.m_nGain;
                    cFrequencyItem.m_nSelf_CAL = cSelfInfo.m_nCAL;
                    cFrequencyItem.m_nSelf_CAG = cSelfInfo.m_nCAG;
                    cFrequencyItem.m_nSelf_IQ_BSH = cSelfInfo.m_nIQ_BSH;
                    cFrequencyItem.m_dSelf_SampleTime = cSelfInfo.m_dSmapleTime;

                    m_cFreqencyItem_List.Add(cFrequencyItem);
                }
            }

            return m_cFreqencyItem_List;
        }

        public List<FrequencyItem> SetFrequencyItem_SelfPNS(string sFrequencyPath)
        {
            string sSelfPNSName = StringConvert.m_dictMainStepMappingTable[MainStep.Self_NCPNCNSweep];
            string sSelfPNSCodeName = StringConvert.m_dictMainStepCodeNameMappingTable[MainStep.Self_NCPNCNSweep];

            int nValueLB = ParamFingerAutoTuning.m_nSelfPNSNCPNCNValueLB;
            int nValueHB = ParamFingerAutoTuning.m_nSelfPNSNCPNCNValueHB;

            if (nValueLB > nValueHB)
            {
                int nSwitchValue = nValueLB;
                nValueLB = nValueHB;
                nValueHB = nSwitchValue;
            }

            if (File.Exists(sFrequencyPath) == true)
            {
                string sLine = "";
                List<SelfInfo> cSelfInfo_List = new List<SelfInfo>();

                StreamReader srFile = new StreamReader(sFrequencyPath, Encoding.Default);

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplit_Array = sLine.Split('=');

                        if (sSplit_Array.Length > 2)
                        {
                            if (sSplit_Array[0] == "Name")
                                continue;
                            else if (sSplit_Array[0] == "CommandFile")
                                continue;
                        }

                        sSplit_Array = sLine.Split(',');

                        if (sSplit_Array.Length >= 8)
                        {
                            SelfInfo cSelfInfo = new SelfInfo();

                            if (ElanConvert.IsInt(sSplit_Array[1]) == true)
                                cSelfInfo.m_nPH1 = Convert.ToInt32(sSplit_Array[1]);

                            if (ElanConvert.IsInt(sSplit_Array[2]) == true)
                                cSelfInfo.m_nPH2E_LAT = Convert.ToInt32(sSplit_Array[2]);

                            if (ElanConvert.IsInt(sSplit_Array[3]) == true)
                                cSelfInfo.m_nPH2E_LMT = Convert.ToInt32(sSplit_Array[3]);

                            if (ElanConvert.IsInt(sSplit_Array[4]) == true)
                                cSelfInfo.m_nPH2_LAT = Convert.ToInt32(sSplit_Array[4]);

                            if (ElanConvert.IsInt(sSplit_Array[5]) == true)
                                cSelfInfo.m_nPH2 = Convert.ToInt32(sSplit_Array[5]);

                            if (ElanConvert.IsInt(sSplit_Array[6]) == true)
                                cSelfInfo.m_nDFT_NUM = Convert.ToInt32(sSplit_Array[6]);

                            if (ElanConvert.IsInt(sSplit_Array[7]) == true)
                                cSelfInfo.m_nGain = Convert.ToInt32(sSplit_Array[7]);

                            if (ElanConvert.IsDouble(sSplit_Array[8]) == true)
                                cSelfInfo.m_dSmapleTime = Convert.ToDouble(sSplit_Array[8]);

                            cSelfInfo_List.Add(cSelfInfo);
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }

                for(int nSetIndex = 0; nSetIndex < cSelfInfo_List.Count; nSetIndex++)
                {
                    SelfInfo cSelfInfo = cSelfInfo_List[nSetIndex];
                    int nPH1 = cSelfInfo.m_nPH1;
                    int nPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cSelfInfo.m_nPH2E_LAT, cSelfInfo.m_nPH2E_LMT, cSelfInfo.m_nPH2_LAT, cSelfInfo.m_nPH2);

                    double dFrequency = ElanConvert.Convert2Frequency(nPH1, nPH2Sum);

                    for (int nValueIndex = nValueLB; nValueIndex <= nValueHB; nValueIndex++)
                    {
                        FrequencyItem cFrequencyItem = new FrequencyItem();
                        cFrequencyItem.m_nSetIndex = nSetIndex;
                        cFrequencyItem.m_n_SELF_PH1 = cSelfInfo.m_nPH1;
                        cFrequencyItem.m_n_SELF_PH2E_LAT = cSelfInfo.m_nPH2E_LAT;
                        cFrequencyItem.m_n_SELF_PH2E_LMT = cSelfInfo.m_nPH2E_LMT;
                        cFrequencyItem.m_n_SELF_PH2_LAT = cSelfInfo.m_nPH2_LAT;
                        cFrequencyItem.m_n_SELF_PH2 = cSelfInfo.m_nPH2;

                        cFrequencyItem.m_nDVTestFrame = ParamFingerAutoTuning.m_nSelfPNSTestFrame;
                        cFrequencyItem.m_dSelf_Frequency = dFrequency;

                        cFrequencyItem.m_nSelf_DFT_NUM = cSelfInfo.m_nDFT_NUM;
                        cFrequencyItem.m_nSelf_Gain = cSelfInfo.m_nGain;
                        cFrequencyItem.m_dSelf_SampleTime = cSelfInfo.m_dSmapleTime;

                        cFrequencyItem.m_nSelf_NCP = nValueIndex;
                        cFrequencyItem.m_nSelf_NCN = nValueIndex;

                        m_cFreqencyItem_List.Add(cFrequencyItem);
                    }
                }
            }

            return m_cFreqencyItem_List;
        }

        private bool GetFileParameterHexValue(ref int nValue, string[] sSplit_Array, string sParameterName)
        {
            if (sSplit_Array[0] == sParameterName)
            {
                string sToken = "0x";
                int nTokenIndex = sSplit_Array[2].IndexOf(sToken);

                if (nTokenIndex < 0)
                {
                    m_sErrorMessage = string.Format("Get {0} Value Error", sParameterName);
                    return false;
                }
                else
                {
                    string sValue = sSplit_Array[2].Substring(nTokenIndex + sToken.Length);

                    if (ElanConvert.IsHexadecimal(sValue) == false)
                    {
                        m_sErrorMessage = string.Format("Get {0} Value Format Error", sParameterName);
                        return false;
                    }
                    else
                        nValue = Convert.ToInt32(sValue, 16);
                }
            }

            return true;
        }

       
        private bool GetFileParameterValue(ref int nValue, string[] sSplit_Array, string sParameterName, int nValueType)
        {
            if (sSplit_Array[0] == sParameterName)
            {
                string sValue = sSplit_Array[1];

                if (nValueType == m_nVALUETYPE_Hex)
                {
                    if (ElanConvert.IsHexadecimal(sValue) == false)
                    {
                        m_sErrorMessage = string.Format("Get {0} Value Format Error", sParameterName);
                        return false;
                    }
                    else
                        nValue = Convert.ToInt32(sValue, 16);
                }
                else if (nValueType == m_nVALUETYPE_Int)
                {
                    if (ElanConvert.IsInt(sValue) == false)
                    {
                        m_sErrorMessage = string.Format("Get {0} Value Format Error", sParameterName);
                        return false;
                    }
                    else
                        nValue = Convert.ToInt32(sValue);
                }
            }

            return true;
        }

        private bool GetFileBoolValue(ref bool bValue, ref int nValue, string[] sSplit_Array, string sParameterName)
        {
            if (sSplit_Array[0] == sParameterName)
            {
                string sValue = sSplit_Array[1];

                if (ElanConvert.IsInt(sValue) == false)
                {
                    m_sErrorMessage = string.Format("Get {0} Value Format Error", sParameterName);
                    return false;
                }
                else
                {
                    nValue = Convert.ToInt32(sValue);

                    if (nValue == 1)
                        bValue = true;
                    else
                        bValue = false;
                }
            }

            return true;
        }
    }
}
