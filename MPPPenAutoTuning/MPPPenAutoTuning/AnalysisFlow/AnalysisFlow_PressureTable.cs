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
    public class AnalysisFlow_PressureTable : AnalysisFlow
    {
        private int m_nPTValidReportNumber = 1;
        private int m_nPTStartSkipReportNumber = 150;
        private int m_nPTEndSkipReportNumber = 150;

        private int m_nDataValueCount = 5;

        private int[] m_nPressureWeight_Array = new int[ParamAutoTuning.m_nPRESSURE_DATA_NUMBER];
        private double[] m_dPressureReferencePower_Array = new double[ParamAutoTuning.m_nPRESSURE_DATA_NUMBER];

        private List<byte> m_byteReport_List = new List<byte>();

        public class DataValue
        {
            //public int m_nThreshold = -1;
        }

        //private List<DataValue> m_cDataValue_List = null; 

        public class WeightInfoAndValue
        {
            public SubTuningStep m_eSubStep = SubTuningStep.ELSE;
            public int m_nSettingPH1 = -1;
            public int m_nSettingPH2 = -1;
            public int m_nReadPH1 = -1;
            public int m_nReadPH2 = -1;
            public double m_dFrequency = -1;

            public int m_nRankIndex = -1;
            public int m_nPTPenVersion = 0;

            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public int m_nLineCount = -1;

            public string m_sWeight = "";
            public int m_nWeight = -1;
            public int m_nRealityWeight = -1;

            public int m_nMinBin7Value = -1;
            public int m_nMinBin8Value = -1;
            public int m_nMinBin9Value = -1;

            public List<List<byte>> m_byteOriginalData_List = new List<List<byte>>();
            public List<List<int>> m_nOriginalData_List = new List<List<int>>();
            public List<List<int>> m_nData_List = new List<List<int>>();
            public List<int> m_nBin7Pwr_List = new List<int>();
            public List<int> m_nBin8Pwr_List = new List<int>();
            public List<int> m_nBin9Pwr_List = new List<int>();
            public List<int> m_nPressureMaxDFTRX_List = new List<int>();
            public List<int> m_nPressurePower_List = new List<int>();
        }

        private WeightInfoAndValue[] m_cWeightInfoAndValue_Array = null;

        private void ClearDataArray()
        {
            m_byteReport_List.Clear();

            Array.Clear(m_dPressureReferencePower_Array, 0, m_dPressureReferencePower_Array.Length);
        }

        public AnalysisFlow_PressureTable(FlowStep cFlowStep, frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
            m_sFileDirectoryPath = m_cfrmMain.m_sFileDirectoryPath;
            m_sRecordProjectName = m_cfrmMain.m_sRecordProjectName;

            InitializeParameter(cFlowStep);

            //m_cDataValue_List = new List<DataValue>();
            m_cWeightInfoAndValue_Array = new WeightInfoAndValue[ParamAutoTuning.m_nPRESSURE_DATA_NUMBER - 1];

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

            m_nPTValidReportNumber = ParamAutoTuning.m_nPTValidReportNumber;
            m_nPTStartSkipReportNumber = ParamAutoTuning.m_nPTStartSkipReportNumber;
            m_nPTEndSkipReportNumber = ParamAutoTuning.m_nPTEndSkipReportNumber;

            m_nNormalReportDataLength = 11;
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

        public void GetData(List<PressureTuningParameter> cParameter_List)
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
                //m_cDataValue_List.Add(cDataValue);
                int nFileIndex = m_cDataInfo_List.Count - 1;
                cDataInfo = m_cDataInfo_List[nFileIndex];

                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);

                //Folder
                m_sIntegrationFolderPath = string.Format(@"{0}\{1}\Integration Data", m_sResultFolderPath, sFileName);
                string sComputeFolderPath = string.Format(@"{0}\{1}\Compute Data", m_sResultFolderPath, sFileName);
                string sProcessFolderPath = string.Format(@"{0}\{1}\Process Data", m_sResultFolderPath, sFileName);
                Directory.CreateDirectory(m_sIntegrationFolderPath);
                Directory.CreateDirectory(sComputeFolderPath);
                Directory.CreateDirectory(sProcessFolderPath);

                //File
                m_sIntegrationFilePath = string.Format(@"{0}\Integration Data.csv", m_sIntegrationFolderPath);
                string sRawDataFilePath = string.Format(@"{0}\Raw Data.csv", sProcessFolderPath);
                string sComputeFilePath = string.Format(@"{0}\Compute Data.csv", sComputeFolderPath);
                string sValidDataFilePath = string.Format(@"{0}\Valid Data.csv", sComputeFolderPath);
                string sPressurePwrFilePath = string.Format(@"{0}\PressurePwr Data.csv", sComputeFolderPath);
                string sMinimumBinFilePath = string.Format(@"{0}\MinimumBin Data.csv", sComputeFolderPath);

                int nLineCounter = 0;
                string sLine = "";

                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                int nRankIndex = -1;

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

                        PressureTuningParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2 && x.m_nRankIndex == cDataInfo.m_nRankIndex);

                        if (cParameter != null)
                        {
                            nRankIndex = cParameter.m_nRankIndex;

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

                int nApproachLB = 0;
                int nApproachHB = 0;
                int nWeightIndex = 0;

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

                        if (nLineCounter > nApproachHB)
                        {
                            for (int nIndex = 0; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
                            {
                                if (m_cWeightInfoAndValue_Array[nIndex].m_nLineCount > nLineCounter)
                                {
                                    if (m_cWeightInfoAndValue_Array[nIndex].m_nLineCount < nApproachHB)
                                        nApproachHB = m_cWeightInfoAndValue_Array[nIndex].m_nLineCount;
                                }
                                else
                                {
                                    if (m_cWeightInfoAndValue_Array[nIndex].m_nLineCount > nApproachLB)
                                    {
                                        nApproachLB = m_cWeightInfoAndValue_Array[nIndex].m_nLineCount;
                                        nWeightIndex = nIndex;
                                    }
                                }
                            }
                        }

                        string[] sSplit_Array = sLine.Split(m_charDELIMITER);

                        if (sSplit_Array.Length != m_nReportDataLength + 1)
                        {
                            cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0008;

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

                            for (int nIndex = 0; nIndex <= 10; nIndex++)
                                byteData_Array[nIndex] = Convert.ToByte(sSplit_Array[nIndex], 16);

                            if (byteData_Array[0] != 0x07 ||
                                byteData_Array[2] != 0xFF || 
                                byteData_Array[3] != 0xFF ||
                                byteData_Array[4] != 0xFF || 
                                byteData_Array[5] != 0xFF ||
                                byteData_Array[9] != 0xFA || 
                                byteData_Array[10] != 0xFA)
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
                        }
                        catch
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

                        for (int nIndex = 0; nIndex < sSplit_Array.Length - 1; nIndex++)
                        {
                            byte byteRawData = Convert.ToByte(sSplit_Array[nIndex], 16);
                            m_byteReport_List.Add(byteRawData);
                        }

                        m_cWeightInfoAndValue_Array[nWeightIndex].m_byteOriginalData_List.Add(new List<byte>(m_byteReport_List));
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

                ConvertPressureValueData();

                for (int nIndex = 0; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
                {
                    if (m_cWeightInfoAndValue_Array[nIndex].m_nData_List.Count == 0 || m_cWeightInfoAndValue_Array[nIndex].m_nData_List.Count < m_nPTValidReportNumber)
                    {
                        cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0020;

                        m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get Valid Data Too Few[Weight={0}]({1}<LB:{2}) In {3} File!", m_cWeightInfoAndValue_Array[nIndex].m_sWeight, Convert.ToString(m_cWeightInfoAndValue_Array[nIndex].m_nData_List.Count), Convert.ToString(m_nPTValidReportNumber), Path.GetFileName(sFilePath));
                        m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get Valid Data Too Few[Weight={0}]({1}<LB:{2})", m_cWeightInfoAndValue_Array[nIndex].m_sWeight, Convert.ToString(m_cWeightInfoAndValue_Array[nIndex].m_nData_List.Count), Convert.ToString(m_nPTValidReportNumber));
                        m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                        m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                        m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                        cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                        break;
                    }
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteRawDataFile("Pressure Raw Data", sRawDataFilePath, true);

                WriteRawDataFile("Pressure Compute Data", sComputeFilePath, false);

                ConvertPressureInfoData();

                WriteValidDataFile("Pressure Valid Data", sValidDataFilePath);

                if (ComputePressureReferencePower(cDataInfo.m_nPTPenVersion) == false)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0040;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Compute Pressure Reference Power Error In {0} File!", Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = "Compute Pressure Reference Power Error";
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WritePressurePowerReferenceDataFile("Pressure Power Reference Data", sPressurePwrFilePath);

                ComputeMinBinValue();

                WriteMinBinValueDataFile("Pressure Minimum Bin Data", sMinimumBinFilePath);

                if (cDataInfo.m_sRecordErrorCode != "")
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0080;

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

            if (ParamAutoTuning.m_nFlowMethodType != 1 && m_nValidFileCount < 1)
            {
                m_sErrorMessage = "No Enough Data. Data Analysis Error!!";
                OutputMessage("No Enough Data. Data Analysis Error!!");
                m_bErrorFlag = true;
                return;
            }
        }

        public void ComputeAndOutputResult()
        {
            if (m_bErrorFlag == false)
                ComputePressureTable();

            OutputResultData();
        }

        protected override void GetFileInfoFromReportData(DataInfo cDataInfo, string sFilePath)
        {
            long lGetInfoFlag = 0;
            string sLine = "";

            WeightInfoAndValue cWeightInfoAndValue = null;
            int nInfoFlag = 0;

            cDataInfo.m_sFileName = Path.GetFileName(sFilePath);

            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            int nWeightState = -1;
            int nLineCount = 0;
            bool bGetWeightInfoFlag = false;

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    for (int nWeightIndex = 0; nWeightIndex < m_cWeightInfoAndValue_Array.Length; nWeightIndex++)
                    {
                        if (sLine == string.Format("====={0}g=====", ParamAutoTuning.m_nPressureWeight_Array[nWeightIndex].ToString()))
                        {
                            if (bGetWeightInfoFlag == false && cWeightInfoAndValue != null)
                            {
                                m_cWeightInfoAndValue_Array[nWeightState] = cWeightInfoAndValue;
                                nInfoFlag |= (0x01 << nWeightState);
                            }

                            nWeightState = nWeightIndex;
                            cWeightInfoAndValue = new WeightInfoAndValue();
                            cWeightInfoAndValue.m_nLineCount = nLineCount;
                            lGetInfoFlag = 0;
                            bGetWeightInfoFlag = false;
                            break;
                        }
                    }

                    if (nWeightState >= 0)
                    {
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_SUBSTEP, sLine, 0x000001, m_nINFOTYPE_TUNINGSTEP);
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_SETTINGPH1, sLine, 0x000002, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_SETTINGPH2, sLine, 0x000004, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_READPH1, sLine, 0x000008, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_READPH2, sLine, 0x000010, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_FREQUENCY, sLine, 0x000020, m_nINFOTYPE_DOUBLE);
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_RANKINDEX, sLine, 0x000040, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_PRESSUREWEIGHT, sLine, 0x000100, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_REALITYWEIGHT, sLine, 0x000200, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_RXTRACENUMBER, sLine, 0x000400, m_nINFOTYPE_INT);
                        GetFileInfo(ref lGetInfoFlag, cWeightInfoAndValue, StringConvert.m_sRECORD_TXTRACENUMBER, sLine, 0x000800, m_nINFOTYPE_INT);

                        if (lGetInfoFlag == 0x00FFF)
                        {
                            m_cWeightInfoAndValue_Array[nWeightState] = cWeightInfoAndValue;
                            nInfoFlag |= (0x01 << nWeightState);
                            bGetWeightInfoFlag = true;
                        }
                    }

                    if (nInfoFlag == 0x7FFF)
                        break;

                    nLineCount++;
                }

                if (bGetWeightInfoFlag == false && cWeightInfoAndValue != null)
                {
                    m_cWeightInfoAndValue_Array[nWeightState] = cWeightInfoAndValue;
                    nInfoFlag |= (0x01 << nWeightState);
                }
            }
            finally
            {
                srFile.Close();
            }
        }

        private void GetFileInfo(ref long lGetInfoFlag, WeightInfoAndValue cWeightInfoAndValue, string sParameterName, string sLine, long lInfoFlag, int nValueType)
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
                                    cWeightInfoAndValue.m_eSubStep = eSubStep;

                                break;
                            case m_nINFOTYPE_INT:
                                int nValue = 0;

                                if (sParameterName == StringConvert.m_sRECORD_RANKINDEX ||
                                    sParameterName == StringConvert.m_sRECORD_PTPENVERSION ||
                                    sParameterName == StringConvert.m_sRECORD_PRESSUREWEIGHT ||
                                    sParameterName == StringConvert.m_sRECORD_REALITYWEIGHT)
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
                                    cWeightInfoAndValue.m_nSettingPH1 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_SETTINGPH2)
                                    cWeightInfoAndValue.m_nSettingPH2 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_READPH1)
                                    cWeightInfoAndValue.m_nReadPH1 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_READPH2)
                                    cWeightInfoAndValue.m_nReadPH2 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RANKINDEX)
                                    cWeightInfoAndValue.m_nRankIndex = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_PTPENVERSION)
                                    cWeightInfoAndValue.m_nPTPenVersion = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_PRESSUREWEIGHT)
                                {
                                    cWeightInfoAndValue.m_sWeight = string.Format("{0}g", sValue);
                                    cWeightInfoAndValue.m_nWeight = nValue;
                                }
                                else if (sParameterName == StringConvert.m_sRECORD_REALITYWEIGHT)
                                    cWeightInfoAndValue.m_nRealityWeight = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RXTRACENUMBER)
                                    cWeightInfoAndValue.m_nRXTraceNumber = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_TXTRACENUMBER)
                                    cWeightInfoAndValue.m_nTXTraceNumber = nValue;

                                break;
                            case m_nINFOTYPE_DOUBLE:
                                double dValue = 0;

                                if (Double.TryParse(sValue, out dValue) == true)
                                {
                                    if (sParameterName == StringConvert.m_sRECORD_FREQUENCY)
                                        cWeightInfoAndValue.m_dFrequency = dValue;
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

        protected override bool CheckInfoIsCorrect(ref string sErrorMessage, DataInfo cDataInfo)
        {
            int nFirstIndex = 0;

            if (m_cWeightInfoAndValue_Array[nFirstIndex].m_eSubStep != m_eSubStep)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_SUBSTEP, m_cWeightInfoAndValue_Array[nFirstIndex].m_sWeight));
            else
            {
                for (int nIndex = nFirstIndex + 1; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
                {
                    if (m_cWeightInfoAndValue_Array[nFirstIndex].m_eSubStep != m_cWeightInfoAndValue_Array[nIndex].m_eSubStep)
                        SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_SUBSTEP, m_cWeightInfoAndValue_Array[nIndex].m_sWeight));
                }

                cDataInfo.m_eSubStep = m_cWeightInfoAndValue_Array[nFirstIndex].m_eSubStep;
            }

            if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nSettingPH1 < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_SETTINGPH1, m_cWeightInfoAndValue_Array[nFirstIndex].m_sWeight));
            else
            {
                for (int nIndex = nFirstIndex + 1; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
                {
                    if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nSettingPH1 != m_cWeightInfoAndValue_Array[nIndex].m_nSettingPH1)
                        SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_SETTINGPH1, m_cWeightInfoAndValue_Array[nIndex].m_sWeight));
                }

                cDataInfo.m_nSettingPH1 = m_cWeightInfoAndValue_Array[nFirstIndex].m_nSettingPH1;
            }

            if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nReadPH1 < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_READPH1, m_cWeightInfoAndValue_Array[nFirstIndex].m_sWeight));
            else
            {
                for (int nIndex = nFirstIndex + 1; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
                {
                    if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nReadPH1 != m_cWeightInfoAndValue_Array[nIndex].m_nReadPH1)
                        SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_READPH1, m_cWeightInfoAndValue_Array[nIndex].m_sWeight));
                }

                cDataInfo.m_nReadPH1 = m_cWeightInfoAndValue_Array[nFirstIndex].m_nReadPH1;
            }

            if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nSettingPH2 < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_SETTINGPH2, m_cWeightInfoAndValue_Array[nFirstIndex].m_sWeight));
            else
            {
                for (int nIndex = nFirstIndex + 1; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
                {
                    if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nSettingPH2 != m_cWeightInfoAndValue_Array[nIndex].m_nSettingPH2)
                        SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_SETTINGPH2, m_cWeightInfoAndValue_Array[nIndex].m_sWeight));
                }

                cDataInfo.m_nSettingPH2 = m_cWeightInfoAndValue_Array[nFirstIndex].m_nSettingPH2;
            }

            if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nReadPH2 < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_READPH2, m_cWeightInfoAndValue_Array[nFirstIndex].m_sWeight));
            else
            {
                for (int nIndex = nFirstIndex + 1; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
                {
                    if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nReadPH2 != m_cWeightInfoAndValue_Array[nIndex].m_nReadPH2)
                        SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_READPH2, m_cWeightInfoAndValue_Array[nIndex].m_sWeight));
                }

                cDataInfo.m_nReadPH2 = m_cWeightInfoAndValue_Array[nFirstIndex].m_nReadPH2;
            }

            if (m_cWeightInfoAndValue_Array[nFirstIndex].m_dFrequency > ParamAutoTuning.m_nFrequencyHB || m_cWeightInfoAndValue_Array[nFirstIndex].m_dFrequency < ParamAutoTuning.m_nFrequencyLB)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_FREQUENCY, m_cWeightInfoAndValue_Array[nFirstIndex].m_sWeight));
            else
            {
                for (int nIndex = nFirstIndex + 1; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
                {
                    if (m_cWeightInfoAndValue_Array[nFirstIndex].m_dFrequency != m_cWeightInfoAndValue_Array[nIndex].m_dFrequency)
                        SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_FREQUENCY, m_cWeightInfoAndValue_Array[nIndex].m_sWeight));
                }

                cDataInfo.m_dFrequency = m_cWeightInfoAndValue_Array[nFirstIndex].m_dFrequency;
            }

            CheckGeneralInfoIsCorrect(ref sErrorMessage, cDataInfo);

            if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nRankIndex < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_RANKINDEX, m_cWeightInfoAndValue_Array[nFirstIndex].m_sWeight));
            else
            {
                for (int nIndex = nFirstIndex + 1; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
                {
                    if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nRankIndex != m_cWeightInfoAndValue_Array[nIndex].m_nRankIndex)
                        SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_RANKINDEX, m_cWeightInfoAndValue_Array[nIndex].m_sWeight));
                }

                cDataInfo.m_nRankIndex = m_cWeightInfoAndValue_Array[nFirstIndex].m_nRankIndex;
            }

            for (int nIndex = nFirstIndex + 1; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
            {
                if (m_cWeightInfoAndValue_Array[nFirstIndex].m_nPTPenVersion != m_cWeightInfoAndValue_Array[nIndex].m_nPTPenVersion)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_PTPENVERSION, m_cWeightInfoAndValue_Array[nIndex].m_sWeight));
            }

            cDataInfo.m_nPTPenVersion = m_cWeightInfoAndValue_Array[nFirstIndex].m_nPTPenVersion;

            for (int nIndex = 0; nIndex < m_cWeightInfoAndValue_Array.Length; nIndex++)
            {
                if (m_cWeightInfoAndValue_Array[nIndex].m_nRealityWeight <= 0)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\"[Weight={1}] Format Error", StringConvert.m_sRECORD_REALITYWEIGHT, m_cWeightInfoAndValue_Array[nIndex].m_sWeight));

                if (m_cWeightInfoAndValue_Array[nIndex].m_nLineCount < 0)
                    SetErrorMessage(ref sErrorMessage, string.Format("Get Data[Weight={0}] Location Error", m_cWeightInfoAndValue_Array[nIndex].m_sWeight));
            }

            cDataInfo.m_nRXTraceNumber = m_cWeightInfoAndValue_Array[nFirstIndex].m_nRXTraceNumber;
            cDataInfo.m_nTXTraceNumber = m_cWeightInfoAndValue_Array[nFirstIndex].m_nTXTraceNumber;

            if (sErrorMessage != "")
                return false;

            return true;
        }

        private void ConvertPressureValueData()
        {
            int nPressurePwrDataLocationByte = 19;

            for (int nWeightIndex = 0; nWeightIndex < m_cWeightInfoAndValue_Array.Length; nWeightIndex++)
            {
                for (int nDataIndex = 0; nDataIndex < m_cWeightInfoAndValue_Array[nWeightIndex].m_byteOriginalData_List.Count; nDataIndex++)
                {
                    if (//m_cWeightInfoAndValue_Array[nWeightIndex].m_byteOriginalData_List[nDataIndex][1] != 0x03 ||
                        (m_cWeightInfoAndValue_Array[nWeightIndex].m_byteOriginalData_List[nDataIndex][nPressurePwrDataLocationByte] == 0x00 &&
                         m_cWeightInfoAndValue_Array[nWeightIndex].m_byteOriginalData_List[nDataIndex][nPressurePwrDataLocationByte + 1] == 0x00))
                        continue;

                    int[] nData_Array = new int[m_nDataValueCount];

                    for (int nValueIndex = 0; nValueIndex < m_nDataValueCount; nValueIndex++)
                    {
                        nData_Array[nValueIndex] = m_cWeightInfoAndValue_Array[nWeightIndex].m_byteOriginalData_List[nDataIndex][2 * nValueIndex + m_nNormalReportDataLength] * 256 +
                                                   m_cWeightInfoAndValue_Array[nWeightIndex].m_byteOriginalData_List[nDataIndex][2 * nValueIndex + m_nNormalReportDataLength + 1];
                    }

                    m_cWeightInfoAndValue_Array[nWeightIndex].m_nOriginalData_List.Add(new List<int>(nData_Array));
                }

                for (int nDataIndex = 0; nDataIndex < m_cWeightInfoAndValue_Array[nWeightIndex].m_nOriginalData_List.Count; nDataIndex++)
                {
                    if (nDataIndex < m_nPTStartSkipReportNumber || nDataIndex > m_cWeightInfoAndValue_Array[nWeightIndex].m_nOriginalData_List.Count - m_nPTEndSkipReportNumber - 1)
                        continue;

                    m_cWeightInfoAndValue_Array[nWeightIndex].m_nData_List.Add(m_cWeightInfoAndValue_Array[nWeightIndex].m_nOriginalData_List[nDataIndex]);
                }
            }
        }

        private void ConvertPressureInfoData()
        {
            int nBin7DataByteLocation = 0;
            int nBin8DataByteLocation = 1;
            int nBin9DataByteLocation = 2;
            int nPressMaxDFTRxDataByteLocation = 3;
            int nPressurePwrDataByteLocation = 4;

            for (int nWeightIndex = 0; nWeightIndex < m_cWeightInfoAndValue_Array.Length; nWeightIndex++)
            {
                for (int nDataIndex = 0; nDataIndex < m_cWeightInfoAndValue_Array[nWeightIndex].m_nData_List.Count; nDataIndex++)
                {
                    m_cWeightInfoAndValue_Array[nWeightIndex].m_nBin7Pwr_List.Add(m_cWeightInfoAndValue_Array[nWeightIndex].m_nData_List[nDataIndex][nBin7DataByteLocation]);
                    m_cWeightInfoAndValue_Array[nWeightIndex].m_nBin8Pwr_List.Add(m_cWeightInfoAndValue_Array[nWeightIndex].m_nData_List[nDataIndex][nBin8DataByteLocation]);
                    m_cWeightInfoAndValue_Array[nWeightIndex].m_nBin9Pwr_List.Add(m_cWeightInfoAndValue_Array[nWeightIndex].m_nData_List[nDataIndex][nBin9DataByteLocation]);
                    m_cWeightInfoAndValue_Array[nWeightIndex].m_nPressureMaxDFTRX_List.Add(m_cWeightInfoAndValue_Array[nWeightIndex].m_nData_List[nDataIndex][nPressMaxDFTRxDataByteLocation]);
                    m_cWeightInfoAndValue_Array[nWeightIndex].m_nPressurePower_List.Add(m_cWeightInfoAndValue_Array[nWeightIndex].m_nData_List[nDataIndex][nPressurePwrDataByteLocation]);
                }
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

        private void WriteRawDataFile(string sTitleName, string sFilePath, bool bOriginalDataFlag = true)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                for (int nWeightIndex = 0; nWeightIndex < m_cWeightInfoAndValue_Array.Length; nWeightIndex++)
                {
                    sw.WriteLine(string.Format("{0}", m_cWeightInfoAndValue_Array[nWeightIndex].m_sWeight));

                    List<List<int>> nData_List = new List<List<int>>();

                    if (bOriginalDataFlag == true)
                        nData_List = m_cWeightInfoAndValue_Array[nWeightIndex].m_nOriginalData_List;
                    else
                        nData_List = m_cWeightInfoAndValue_Array[nWeightIndex].m_nData_List;

                    int nDataLength = nData_List.Count;

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        string sText = "";

                        int nDataNumber = nData_List[nDataIndex].Count;

                        for (int nValueIndex = 0; nValueIndex < nDataNumber; nValueIndex++)
                        {
                            sText += string.Format("{0}", Convert.ToInt32(nData_List[nDataIndex][nValueIndex]));

                            if (nValueIndex < nDataNumber - 1)
                                sText += ",";
                        }

                        sw.WriteLine(sText);
                    }

                    if (nWeightIndex < m_cWeightInfoAndValue_Array.Length)
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

        private void WriteValidDataFile(string sTitleName, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                for (int nWeightIndex = 0; nWeightIndex < m_cWeightInfoAndValue_Array.Length; nWeightIndex++)
                {
                    sw.WriteLine(string.Format("{0}", m_cWeightInfoAndValue_Array[nWeightIndex].m_sWeight));

                    sw.WriteLine("Bin7Pwr,Bin8Pwr,Bin9Pwr,PressMaxDFTRx,PressurePwr");

                    int nDataLength = m_cWeightInfoAndValue_Array[nWeightIndex].m_nPressurePower_List.Count;

                    for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                    {
                        sw.Write(string.Format("{0},", m_cWeightInfoAndValue_Array[nWeightIndex].m_nBin7Pwr_List[nDataIndex]));
                        sw.Write(string.Format("{0},", m_cWeightInfoAndValue_Array[nWeightIndex].m_nBin8Pwr_List[nDataIndex]));
                        sw.Write(string.Format("{0},", m_cWeightInfoAndValue_Array[nWeightIndex].m_nBin9Pwr_List[nDataIndex]));
                        sw.Write(string.Format("{0},", m_cWeightInfoAndValue_Array[nWeightIndex].m_nPressureMaxDFTRX_List[nDataIndex]));
                        sw.WriteLine(string.Format("{0}", m_cWeightInfoAndValue_Array[nWeightIndex].m_nPressurePower_List[nDataIndex]));
                    }

                    if (nWeightIndex < m_cWeightInfoAndValue_Array.Length)
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

        private void WritePressurePowerReferenceDataFile(string sTitleName, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                sw.WriteLine();
                sw.WriteLine("Weight,Power");

                for (int nWeightIndex = 0; nWeightIndex < m_nPressureWeight_Array.Length; nWeightIndex++)
                    sw.WriteLine(string.Format("{0}g,{1}", m_nPressureWeight_Array[nWeightIndex], m_dPressureReferencePower_Array[nWeightIndex]));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WriteMinBinValueDataFile(string sTitleName, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                sw.WriteLine();
                sw.WriteLine("Weight,MinBin7,MinBin8,MinBin9");

                for (int nWeightIndex = 0; nWeightIndex < m_cWeightInfoAndValue_Array.Length; nWeightIndex++)
                {
                    sw.WriteLine(string.Format("{0}g,{1},{2},{3}", 
                                               m_cWeightInfoAndValue_Array[nWeightIndex].m_nRealityWeight,
                                               m_cWeightInfoAndValue_Array[nWeightIndex].m_nMinBin7Value,
                                               m_cWeightInfoAndValue_Array[nWeightIndex].m_nMinBin8Value,
                                               m_cWeightInfoAndValue_Array[nWeightIndex].m_nMinBin9Value));
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void WritePressureLevelDataFile(string sTitleName, int[] nPressureLevel_Array, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                for (int nLevelIndex = 0; nLevelIndex < nPressureLevel_Array.Length; nLevelIndex++)
                    sw.WriteLine(nPressureLevel_Array[nLevelIndex]);
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
            string[] sValueTypeName_Array = new string[9] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sFlowStep, 
                SpecificText.m_sSettingPH1, 
                SpecificText.m_sSettingPH2, 
                SpecificText.m_sReadPH1, 
                SpecificText.m_sReadPH2, 
                SpecificText.m_sFrequency, 
                SpecificText.m_sErrorMessage 
            };

            string[] sFWParameterTypeName_Array = new string[6] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                SpecificText.m_sFrequency, 
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
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_eSubStep.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));

                            string sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sErrorMessage);

                            if (ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode != "")
                                sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sRecordErrorMessage);

                            sw.WriteLine(sErrorMessage);

                            break;
                        }
                    }
                }

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
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));

                            string sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sErrorMessage);

                            if (ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode != "")
                                sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sRecordErrorMessage);

                            sw.WriteLine(sErrorMessage);

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
                FileProcess cFileProcess = new FileProcess(m_cfrmMain);
                cFileProcess.WriteResultListTxtFile(m_eSubStep, m_nSubStepState);
            }

            OutputMessage("Analysis Complete!!");
        }

        private void ComputePressureTable()
        {
            GetPressureTable cGetPressureTable = new GetPressureTable();

            cGetPressureTable.SetWeight(m_nPressureWeight_Array);
            cGetPressureTable.SetPower(m_dPressureReferencePower_Array);
            //cGetPressureTable.Start(m_sResultFolderPath);

            int[] nPressureLevel_Array = cGetPressureTable.Start(m_sResultFolderPath);

            string sPressureLevelFilePath = string.Format(@"{0}\PressureLevel Data.csv", m_sResultFolderPath);
            WritePressureLevelDataFile("Pressure Level", nPressureLevel_Array, sPressureLevelFilePath);
        }

        private bool ComputePressureReferencePower(int nPTPenVersion)
        {
            int n25gIndex = Array.FindIndex(ParamAutoTuning.m_nPressureWeight_Array, n => n == 25);

            if (n25gIndex < 0 || n25gIndex > ParamAutoTuning.m_nPressureWeight_Array.Length - 1)
                return false;

            int n50gIndex = Array.FindIndex(ParamAutoTuning.m_nPressureWeight_Array, n => n == 50);

            if (n50gIndex < 0 || n50gIndex > ParamAutoTuning.m_nPressureWeight_Array.Length - 1)
                return false;

            for (int nInfoIndex = 0; nInfoIndex < m_cWeightInfoAndValue_Array.Length; nInfoIndex++)
            {
                if (m_cWeightInfoAndValue_Array[nInfoIndex].m_nWeight == ParamAutoTuning.m_nPressureWeight_Array[n25gIndex])
                {
                    double dMinValue = m_cWeightInfoAndValue_Array[nInfoIndex].m_nPressurePower_List.Min();

                    int nWeightIndex = Array.FindIndex(ParamAutoTuning.m_nRealPressureWeight_Array, n => n == m_cWeightInfoAndValue_Array[nInfoIndex].m_nWeight);

                    if (nWeightIndex < 0 || nWeightIndex > ParamAutoTuning.m_nRealPressureWeight_Array.Length - 1)
                        return false;

                    m_dPressureReferencePower_Array[nWeightIndex] = dMinValue;
                }
                else
                {
                    double dMeanValue = 0.0;

                    int nDataCount = m_cWeightInfoAndValue_Array[nInfoIndex].m_nPressurePower_List.Count;

                    for (int mIndex = 0; mIndex < nDataCount; mIndex++)
                        dMeanValue += m_cWeightInfoAndValue_Array[nInfoIndex].m_nPressurePower_List[mIndex];

                    dMeanValue = Math.Round(dMeanValue / nDataCount, 0, MidpointRounding.AwayFromZero);

                    int nWeightIndex = Array.FindIndex(ParamAutoTuning.m_nRealPressureWeight_Array, n => n == m_cWeightInfoAndValue_Array[nInfoIndex].m_nWeight);

                    if (nWeightIndex < 0 || nWeightIndex > ParamAutoTuning.m_nRealPressureWeight_Array.Length - 1)
                        return false;

                    m_dPressureReferencePower_Array[nWeightIndex] = dMeanValue;
                }
            }

            n25gIndex = Array.FindIndex(ParamAutoTuning.m_nRealPressureWeight_Array, n => n == 25);

            if (n25gIndex < 0 || n25gIndex > ParamAutoTuning.m_nRealPressureWeight_Array.Length - 1)
                return false;

            n50gIndex = Array.FindIndex(ParamAutoTuning.m_nRealPressureWeight_Array, n => n == 50);

            if (n50gIndex < 0 || n50gIndex > ParamAutoTuning.m_nRealPressureWeight_Array.Length - 1)
                return false;

            double d10gRefValue = Math.Round(m_dPressureReferencePower_Array[n25gIndex] - (m_dPressureReferencePower_Array[n50gIndex] - m_dPressureReferencePower_Array[n25gIndex]) / 2, 0, MidpointRounding.AwayFromZero);

            int n10gIndex = Array.FindIndex(ParamAutoTuning.m_nRealPressureWeight_Array, n => n == 10);

            if (n10gIndex < 0 || n10gIndex > ParamAutoTuning.m_nRealPressureWeight_Array.Length - 1)
                return false;

            for (int nWeightIndex = n10gIndex + 1; nWeightIndex < ParamAutoTuning.m_nRealPressureWeight_Array.Length; nWeightIndex++)
            {
                for (int nInfoIndex = 0; nInfoIndex < m_cWeightInfoAndValue_Array.Length; nInfoIndex++)
                {
                    if (ParamAutoTuning.m_nRealPressureWeight_Array[nWeightIndex] == m_cWeightInfoAndValue_Array[nInfoIndex].m_nWeight)
                    {
                        if (ParamAutoTuning.m_nPTPenVersion == 1 || nPTPenVersion == 1)
                        {
                            switch (m_cWeightInfoAndValue_Array[nInfoIndex].m_nWeight)
                            {
                                case 25:
                                    m_nPressureWeight_Array[nWeightIndex] = m_cWeightInfoAndValue_Array[nInfoIndex].m_nRealityWeight + ParamAutoTuning.m_nPTExtraIncWeight_25G;
                                    break;
                                case 50:
                                    m_nPressureWeight_Array[nWeightIndex] = m_cWeightInfoAndValue_Array[nInfoIndex].m_nRealityWeight + ParamAutoTuning.m_nPTExtraIncWeight_50G;
                                    break;
                                case 75:
                                    m_nPressureWeight_Array[nWeightIndex] = m_cWeightInfoAndValue_Array[nInfoIndex].m_nRealityWeight + ParamAutoTuning.m_nPTExtraIncWeight_75G;
                                    break;
                                case 100:
                                    m_nPressureWeight_Array[nWeightIndex] = m_cWeightInfoAndValue_Array[nInfoIndex].m_nRealityWeight + ParamAutoTuning.m_nPTExtraIncWeight_100G;
                                    break;
                                default:
                                    m_nPressureWeight_Array[nWeightIndex] = m_cWeightInfoAndValue_Array[nInfoIndex].m_nRealityWeight;
                                    break;
                            }
                        }
                        else
                            m_nPressureWeight_Array[nWeightIndex] = m_cWeightInfoAndValue_Array[nInfoIndex].m_nRealityWeight;
                        break;
                    }
                }
            }

            if (ParamAutoTuning.m_nPTPenVersion == 1 || nPTPenVersion == 1)
            {
                int n75gIndex = Array.FindIndex(ParamAutoTuning.m_nRealPressureWeight_Array, n => n == 75);

                if (n75gIndex < 0 || n75gIndex > ParamAutoTuning.m_nRealPressureWeight_Array.Length - 1)
                    return false;

                int n100gIndex = Array.FindIndex(ParamAutoTuning.m_nRealPressureWeight_Array, n => n == 100);

                if (n100gIndex < 0 || n100gIndex > ParamAutoTuning.m_nRealPressureWeight_Array.Length - 1)
                    return false;

                double[] dWeight_Array = new double[4] 
                { 
                    m_nPressureWeight_Array[n25gIndex], 
                    m_nPressureWeight_Array[n50gIndex], 
                    m_nPressureWeight_Array[n75gIndex], 
                    m_nPressureWeight_Array[n100gIndex] 
                };

                double[] dReferenceValue_Array = new double[4] 
                { 
                    m_dPressureReferencePower_Array[n25gIndex], 
                    m_dPressureReferencePower_Array[n50gIndex], 
                    m_dPressureReferencePower_Array[n75gIndex], 
                    m_dPressureReferencePower_Array[n100gIndex] 
                };

                double[] dPolyFitCoeff_Array = GetPolyFitCoeff(dWeight_Array, dReferenceValue_Array, 2);

                int n25gReferenceValue = (int)ComputePolyFitValue(25, dPolyFitCoeff_Array);
                int n50gReferenceValue = (int)ComputePolyFitValue(50, dPolyFitCoeff_Array);

                d10gRefValue = Math.Round(n25gReferenceValue - (double)(n50gReferenceValue - n25gReferenceValue) / 2, 0, MidpointRounding.AwayFromZero);

                m_nPressureWeight_Array[n10gIndex] = 10;
            }
            else
                m_nPressureWeight_Array[n10gIndex] = (int)Math.Round(20 * ((double)m_nPressureWeight_Array[n25gIndex] / (double)m_nPressureWeight_Array[n50gIndex]), 0, MidpointRounding.AwayFromZero);

            m_dPressureReferencePower_Array[n10gIndex] = d10gRefValue;

            return true;
        }

        private void ComputeMinBinValue()
        {
            for (int nWeightIndex = 0; nWeightIndex < m_cWeightInfoAndValue_Array.Length; nWeightIndex++)
            {
                int nMinBin7Value = m_cWeightInfoAndValue_Array[nWeightIndex].m_nBin7Pwr_List.Min();
                int nMinBin8Value = m_cWeightInfoAndValue_Array[nWeightIndex].m_nBin8Pwr_List.Min();
                int nMinBin9Value = m_cWeightInfoAndValue_Array[nWeightIndex].m_nBin9Pwr_List.Min();

                m_cWeightInfoAndValue_Array[nWeightIndex].m_nMinBin7Value = nMinBin7Value;
                m_cWeightInfoAndValue_Array[nWeightIndex].m_nMinBin8Value = nMinBin8Value;
                m_cWeightInfoAndValue_Array[nWeightIndex].m_nMinBin9Value = nMinBin9Value;
            }
        }

        private double[] GetPolyFitCoeff(double[] dXAxis_Array, double[] dYAxis_Array, int nOrder)
        {
            double[] dPolyFitCoeff_Array = Fit.Polynomial(dXAxis_Array, dYAxis_Array, nOrder);

            return dPolyFitCoeff_Array;
        }

        private double ComputePolyFitValue(double dIncreaseOrderValue, double[] dCoeff_Array)
        {
            double dPolyFitValue = 0.0;

            for (int nCoeffIndex = 0; nCoeffIndex < dCoeff_Array.Length; nCoeffIndex++)
                dPolyFitValue += dCoeff_Array[nCoeffIndex] * Math.Pow(dIncreaseOrderValue, nCoeffIndex);

            return Math.Round(dPolyFitValue, 2, MidpointRounding.AwayFromZero);
        }
    }
}
