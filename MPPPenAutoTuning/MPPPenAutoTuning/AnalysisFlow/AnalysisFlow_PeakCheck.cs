using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class AnalysisFlow_PeakCheck : AnalysisFlow
    {
        private enum TracePowerDefine : int
        {
            Pwr_1TR = 0,
            Pwr_2TR = 1,
            Pwr_3TR = 2,
            Pwr_4TR = 3,
            Pwr_5TR = 4
        }

        private int m_nNormalFilterRXValidReportNumber = 150;
        private int m_nNormalFilterTXValidReportNumber = 70;

        private const int m_nPeakCheckPowerCount = 5;

        private double m_dPeakCheckRatio = 1.1;    // 1.2~1.4 
        private double m_dPeakCheckRatio5T = 0.9;  // 0.8~1.2
        private double m_dPeakCheckRatio3T = 0.4;  // 0.4~0.6

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
            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public int m_nNoiseRXInnerMax = -1;
            public int m_nFilterRXValue = -1;

            public int m_nNoiseDigiGain_Beacon_Rx = -1;

            public int m_nPenPeak1Tr = 0;
            public int m_nPenPeak2Tr = 0;
            public int m_nPenPeak3Tr = 0;
            public int m_nPenPeak4Tr = 0;
            public int m_nPenPeak5Tr = 0;

            public int m_nPenPeak_1Traces_Th = 0;
            public int m_nPenPeak_2Traces_Th = 0;
            public int m_nPenPeakWidth_Th = 0;
            public int m_nPenPeak_4Traces_Th = 0;
            public int m_nPenPeak_5Traces_Th = 0;
            public int m_nPenPeak_5Traces_PeakPwr_Th = 0;
            public int m_nPenPeak_Th = 0;
            public int m_nPenPeakCheck_AreaUP_Pwr_TH = 0;

            public int m_nPenPeak_1Traces_Th_H1st = 0;
            public int m_nPenPeak_2Traces_Th_H1st = 0;
            public int m_nPenPeakWidth_Th_H1st = 0;
            public int m_nPenPeak_4Traces_Th_H1st = 0;
            public int m_nPenPeak_5Traces_Th_H1st = 0;
            public int m_nPenPeak_5Traces_PeakPwr_Th_H1st = 0;
            public int m_nPenPeak_Th_H1st = 0;
            public int m_nPenPeakCheck_AreaUP_Pwr_TH_H1st = 0;

            public int m_nPenPeak_1Traces_Th_H2nd = 0;
            public int m_nPenPeak_2Traces_Th_H2nd = 0;
            public int m_nPenPeakWidth_Th_H2nd = 0;
            public int m_nPenPeak_4Traces_Th_H2nd = 0;
            public int m_nPenPeak_5Traces_Th_H2nd = 0;
            public int m_nPenPeak_5Traces_PeakPwr_Th_H2nd = 0;
            public int m_nPenPeak_Th_H2nd = 0;
            public int m_nPenPeakCheck_AreaUP_Pwr_TH_H2nd = 0;

            public int m_nFPenPeak_1Traces_Th = 0;
            public int m_nFPenPeak_2Traces_Th = 0;
            public int m_nFPenPeakWidth_Th = 0;
            public int m_nFPenPeak_4Traces_Th = 0;
            public int m_nFPenPeak_5Traces_Th = 0;
            public int m_nFPenPeak_5Traces_PeakPwr_Th = 0;
            public int m_nFPenPeak_Th = 0;
            public int m_nFPenPeakCheck_AreaUP_Pwr_TH = 0;
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

        public AnalysisFlow_PeakCheck(FlowStep cFlowStep, frmMain cfrmMain)
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

            m_dPeakCheckRatio = ParamAutoTuning.m_dPCTPeakCheckRatio;
            m_dPeakCheckRatio5T = ParamAutoTuning.m_dPCTPeakCheckRatio5T;
            m_dPeakCheckRatio3T = ParamAutoTuning.m_dPCTPeakCheckRatio3T;
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

        public void GetData(List<PeakCheckTuningParameter> cParameter_List)
        {
            //取得資料夾內所有檔案
            string[] sValidDataFilePath_Array = GetValidReportDataFile(cParameter_List);

            if (sValidDataFilePath_Array == null || sValidDataFilePath_Array.Length == 0)
            {
                m_sErrorMessage = "No Valid Data!!";
                OutputMessage("No Valid Data!!");
                m_bErrorFlag = true;
                return;
            }

            int nFileCount = sValidDataFilePath_Array.Length;

            OutputMainStatusStrip("Analysing...", 0, nFileCount, frmMain.m_nInitialFlag);

            foreach (string sFilePath in sValidDataFilePath_Array)
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
                string sProcessFolderPath = string.Format(@"{0}\{1}\Process Data", m_sResultFolderPath, sFileName);
                Directory.CreateDirectory(m_sIntegrationFolderPath);
                Directory.CreateDirectory(sComputeFolderPath);
                Directory.CreateDirectory(sProcessFolderPath);

                m_sIntegrationFilePath = string.Format(@"{0}\Integration Data.csv", m_sIntegrationFolderPath);
                string sRXDataFilePath = string.Format(@"{0}\RX Raw Data.csv", sProcessFolderPath);
                string sTXDataFilePath = string.Format(@"{0}\TX Raw Data.csv", sProcessFolderPath);
                string sRXComputeFilePath = string.Format(@"{0}\RX Compute Data.csv", sComputeFolderPath);
                string sTXComputeFilePath = string.Format(@"{0}\TX Compute Data.csv", sComputeFolderPath);
                string sRXValidFilePath = string.Format(@"{0}\RX Valid Data.csv", sComputeFolderPath);
                string sRXPwrSWAPFilePath = string.Format(@"{0}\RX PwrSWAP Data.csv", sComputeFolderPath);

                int nLineCounter = 0;
                string sLine = "";
                m_nErrorFlag = 0;
                m_bReadReportDataErrorFlag = false;

                int nRankIndex = -1;
                int nRXFilterValue = -1;

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
                    cDataValue.m_nFilterRXValue = nRXFilterValue = cDataValue.m_nNoiseRXInnerMax;
                }
                else
                {
                    if (CheckAnalogParameterIsIdentical() == true)
                    {
                        bool bCheckFlag = false;

                        PeakCheckTuningParameter cParameter = cParameter_List.Find(x => x.m_nPH1 == cDataInfo.m_nReadPH1 && x.m_nPH2 == cDataInfo.m_nReadPH2);

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

                            cDataValue.m_nFilterRXValue = nRXFilterValue = cParameter.m_nNoiseRXInnerMax;
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

                if (m_eSubStep == SubTuningStep.PCCONTACT)
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

                int nLeftBoundary = 2;
                int nRightBoundary = (nSectionNumber - 1) - 2;

                for (int nIndex = 0; nIndex < nDataLength; nIndex++)
                {
                    int nTraceTypeIntData = m_byteData_List[nIndex][nTraceTypeByte - 1];
                    int nIndexData = m_byteData_List[nIndex][nIndexByte - 1];

                    //Rx Data
                    if (nTraceTypeIntData == MainConstantParameter.m_nGETDATATYPE_NONESYNCRX_400US || nTraceTypeIntData == MainConstantParameter.m_nGETDATATYPE_NONESYNCRX_800US)
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
                    else if (nTraceTypeIntData == MainConstantParameter.m_nGETDATATYPE_NONESYNCTX_400US || nTraceTypeIntData == MainConstantParameter.m_nGETDATATYPE_NONESYNCTX_800US)
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

                WriteProcessData(string.Format("RX {0} Data", m_sSubStepCodeName), m_nRXOriginalData_List, sRXDataFilePath);
                WriteProcessData(string.Format("TX {0} Data", m_sSubStepCodeName), m_nTXOriginalData_List, sTXDataFilePath);

                m_nRXData_List = ConvertProcessData(m_nRXOriginalData_List);
                m_nTXData_List = ConvertProcessData(m_nTXOriginalData_List);

                WriteComputeDataFile("RX Compute Data", m_nRXData_List, sRXComputeFilePath);
                WriteComputeDataFile("TX Compute Data", m_nTXData_List, sTXComputeFilePath);

                ComputeFilterValueByDigiGainRatio(ref nRXFilterValue, nFileIndex);

                List<List<int>> nRXValidData_List = GetValidData(m_nRXData_List, m_nRXOriginalIndex_List, nLeftBoundary, nRightBoundary, nRXFilterValue);

                int[,] nRXValidData_Array = ConvertListDataTo2DArray(nRXValidData_List);

                if (nRXValidData_List.Count < m_nNormalFilterRXValidReportNumber || nRXValidData_List.Count == 0)
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0200;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("Get RX Valid Data Too Few({0}<LB:{1}) In {2} File!", Convert.ToString(nRXValidData_List.Count), Convert.ToString(m_nNormalFilterRXValidReportNumber), Path.GetFileName(sFilePath));
                    m_cErrorInfo.m_sRecordErrorMessage = string.Format("Get RX Valid Data Too Few({0}<LB:{1})", Convert.ToString(nRXValidData_List.Count), Convert.ToString(m_nNormalFilterRXValidReportNumber));
                    m_cErrorInfo.m_sSaveFilePath = m_sIntegrationFilePath;
                    m_cErrorInfo.m_nErrorFlag = m_nErrorFlag;
                    m_cErrorInfo.m_sCurrnetFilePath = sFilePath;

                    cDataInfo.m_sErrorMessage = RunProcessErrorFlow();
                }

                if (m_bReadReportDataErrorFlag == true)
                    continue;

                WriteValidDataFile("RX Valid Data", nRXValidData_Array, sRXValidFilePath);

                int[,] nRXPowerSWAP_Array = ComputePowerSWAP(nRXValidData_Array);

                WritePwrSWAPDataFile("RX PwrSWAP Data", nRXPowerSWAP_Array, sRXPwrSWAPFilePath);

                ProcessPeakCheckCompare(nRXPowerSWAP_Array, nFileIndex);

                SetTraceNumber(nFileIndex);

                AddDataFileNameAndRankIndex(nFileIndex, nRankIndex);

                if (cDataInfo.m_sRecordErrorCode != "")
                {
                    cDataInfo.m_nErrorFlag = m_nErrorFlag |= 0x0400;

                    m_cErrorInfo.m_sPrintErrorMessage = string.Format("{0} In {1} File!", m_cDataInfo_List[nFileIndex].m_sRecordErrorMessage, Path.GetFileName(sFilePath));
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
            if (ParamAutoTuning.m_nFlowMethodType != 1 && m_eSubStep == SubTuningStep.PCCONTACT)
            {
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
            if (m_bErrorFlag == false)
            {
                ComputePenPeakParameter();

                if (m_eSubStep == SubTuningStep.PCCONTACT)
                    ComputeFinalPenPeakParameter();
            }

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
                    //GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISETXINNERMAX, sLine, 0x000200, m_nINFOTYPE_INT);
                    //GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISERXINMAXPLUS3INMAXSTD, sLine,0x000400, m_nINFOTYPE_INT);
                    //GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_1TR, sLine, 0x000800, m_nINFOTYPE_INT);
                    //GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_2TR, sLine, 0x001000, m_nINFOTYPE_INT);
                    //GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_3TR, sLine, 0x002000, m_nINFOTYPE_INT);
                    //GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_4TR, sLine, 0x004000, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX, sLine, 0x000200, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataValue, StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_RX, sLine, 0x000400, m_nINFOTYPE_INT);

                    if (lGetInfoFlag == 0x00007FF)
                        break;

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

                                if (sParameterName == StringConvert.m_sRECORD_NOISERXINNERMAX)
                                    cDataValue.m_nNoiseRXInnerMax = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_RX)
                                    cDataValue.m_nNoiseDigiGain_Beacon_Rx = nValue;

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
            }

            if (cDataInfo.m_nP0_DetectTime_Index < 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_P0_DETECT_TIME));

            if (sErrorMessage != "")
                return false;

            return true;
        }

        private List<List<int>> ConvertProcessData(List<List<int>> nInputData_List)
        {
            List<List<int>> byteData_List = new List<List<int>>();

            for (int nIndex = 0; nIndex < nInputData_List.Count; nIndex++)
                byteData_List.Add(nInputData_List[nIndex]);

            return byteData_List;
        }

        private List<List<int>> GetValidData(List<List<int>> nData_List, List<int> nIndex_List, int nLeftBoundary, int nRightBoundary, int nFilterRXValue)
        {
            List<List<int>> nResultData_List = new List<List<int>>();

            for (int nDataIndex = 0; nDataIndex < nData_List.Count; nDataIndex++)
            {
                if (nIndex_List[nDataIndex] < nLeftBoundary || nIndex_List[nDataIndex] > nRightBoundary)
                    continue;

                if (nData_List[nDataIndex][nIndex_List[nDataIndex]] <= nFilterRXValue)
                    continue;

                List<int> nSingleData_List = new List<int>();

                int nStartIndex = nIndex_List[nDataIndex] - 2;
                int nEndIndex = nIndex_List[nDataIndex] - 2 + m_nPeakCheckPowerCount - 1;

                for (int nValueIndex = nStartIndex; nValueIndex <= nEndIndex; nValueIndex++)
                    nSingleData_List.Add(nData_List[nDataIndex][nValueIndex]);

                nResultData_List.Add(nSingleData_List);
            }

            return nResultData_List;
        }

        private int[,] ConvertListDataTo2DArray(List<List<int>> nData_List)
        {
            int[,] nResultData_Array = new int[nData_List.Count, m_nPeakCheckPowerCount];

            for (int nDataIndex = 0; nDataIndex < nData_List.Count; nDataIndex++)
            {
                for (int nPowerIndex = 0; nPowerIndex < m_nPeakCheckPowerCount; nPowerIndex++)
                    nResultData_Array[nDataIndex, nPowerIndex] = nData_List[nDataIndex][nPowerIndex];
            }

            return nResultData_Array;
        }

        private int[,] ComputePowerSWAP(int[,] nData_Array)
        {
            int[,] nPwrSWAP_Array = new int[nData_Array.GetLength(0), m_nPeakCheckPowerCount];

            for (int nIndex = 0; nIndex < nData_Array.GetLength(0); nIndex++)
            {
                //PenPeak1Tr
                nPwrSWAP_Array[nIndex, 0] = nData_Array[nIndex, 2];

                //PenPeak2Tr, PenPeak3Tr
                if (nData_Array[nIndex, 1] > nData_Array[nIndex, 3])
                {
                    nPwrSWAP_Array[nIndex, 1] = nData_Array[nIndex, 1];
                    nPwrSWAP_Array[nIndex, 2] = nData_Array[nIndex, 3];
                }
                else
                {
                    nPwrSWAP_Array[nIndex, 1] = nData_Array[nIndex, 3];
                    nPwrSWAP_Array[nIndex, 2] = nData_Array[nIndex, 1];
                }

                //PenPeak4Tr, PenPeak5Tr
                if (nData_Array[nIndex, 0] > nData_Array[nIndex, 4])
                {
                    nPwrSWAP_Array[nIndex, 3] = nData_Array[nIndex, 0];
                    nPwrSWAP_Array[nIndex, 4] = nData_Array[nIndex, 4];
                }
                else
                {
                    nPwrSWAP_Array[nIndex, 3] = nData_Array[nIndex, 4];
                    nPwrSWAP_Array[nIndex, 4] = nData_Array[nIndex, 0];
                }
            }

            return nPwrSWAP_Array;
        }

        private void ProcessPeakCheckCompare(int[,] nData_Array, int nFileIndex)
        {
            for (int nPowerIndex = 0; nPowerIndex < m_nPeakCheckPowerCount; nPowerIndex++)
            {
                List<int> nPower_List = new List<int>();

                for (int nDataIndex = 0; nDataIndex < nData_Array.GetLength(0); nDataIndex++)
                    nPower_List.Add(nData_Array[nDataIndex, nPowerIndex]);

                int nMaxValue = nPower_List.Max();

                switch (nPowerIndex)
                {
                    case (int)TracePowerDefine.Pwr_1TR:
                        m_cDataValue_List[nFileIndex].m_nPenPeak1Tr = nMaxValue;
                        break;
                    case (int)TracePowerDefine.Pwr_2TR:
                        m_cDataValue_List[nFileIndex].m_nPenPeak2Tr = nMaxValue;
                        break;
                    case (int)TracePowerDefine.Pwr_3TR:
                        m_cDataValue_List[nFileIndex].m_nPenPeak3Tr = nMaxValue;
                        break;
                    case (int)TracePowerDefine.Pwr_4TR:
                        m_cDataValue_List[nFileIndex].m_nPenPeak4Tr = nMaxValue;
                        break;
                    case (int)TracePowerDefine.Pwr_5TR:
                        m_cDataValue_List[nFileIndex].m_nPenPeak5Tr = nMaxValue;
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetTraceNumber(int nFileIndex)
        {
            m_cDataValue_List[nFileIndex].m_nRXTraceNumber = m_nRXTraceNumber;
            m_cDataValue_List[nFileIndex].m_nTXTraceNumber = m_nTXTraceNumber;
        }

        private void WriteValidDataFile(string sTitleName, int[,] nProcessData_Array, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                int nDataLength = nProcessData_Array.GetLength(0);
                int nPowerLength = nProcessData_Array.GetLength(1);

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    string sText = "";

                    for (int nPowerIndex = 0; nPowerIndex < nPowerLength; nPowerIndex++)
                    {
                        if (nPowerIndex < nPowerLength - 1)
                            sText += string.Format("{0},", nProcessData_Array[nDataIndex, nPowerIndex]);
                        else
                            sText += string.Format("{0}", nProcessData_Array[nDataIndex, nPowerIndex]);
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

        private void WritePwrSWAPDataFile(string sTitleName, int[,] nProcessData_Array, string sFilePath)
        {
            string[] sPwrSWAPTitle_Array = new string[m_nPeakCheckPowerCount] 
            { 
                SpecificText.m_sPenPeak1Tr, 
                SpecificText.m_sPenPeak2Tr, 
                SpecificText.m_sPenPeak3Tr, 
                SpecificText.m_sPenPeak4Tr, 
                SpecificText.m_sPenPeak5Tr 
            };

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("{0}", sTitleName));

                string sText = "";

                for (int nColumnIndex = 0; nColumnIndex < sPwrSWAPTitle_Array.Length; nColumnIndex++)
                {
                    if (nColumnIndex < sPwrSWAPTitle_Array.Length - 1)
                        sText += string.Format("{0},", sPwrSWAPTitle_Array[nColumnIndex]);
                    else
                        sText += string.Format("{0}", sPwrSWAPTitle_Array[nColumnIndex]);
                }

                sw.WriteLine(sText);

                int nDataLength = nProcessData_Array.GetLength(0);
                int nPowerLength = nProcessData_Array.GetLength(1);

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    sText = "";

                    for (int nPowerIndex = 0; nPowerIndex < nPowerLength; nPowerIndex++)
                    {
                        if (nPowerIndex < nPowerLength - 1)
                            sText += string.Format("{0},", nProcessData_Array[nDataIndex, nPowerIndex]);
                        else
                            sText += string.Format("{0}", nProcessData_Array[nDataIndex, nPowerIndex]);
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
                        sText += string.Format("{0}", Convert.ToInt32(nProcessData_List[nDataIndex][nTraceIndex]));

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
            string[] sValueTypeName_Array = new string[24] 
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
                SpecificText.m_sPenPeak1Tr, 
                SpecificText.m_sPenPeak2Tr, 
                SpecificText.m_sPenPeak3Tr, 
                SpecificText.m_sPenPeak4Tr, 
                SpecificText.m_sPenPeak5Tr,
                SpecificText.m_sPenPeak_1Traces_Th, 
                SpecificText.m_sPenPeak_2Traces_Th, 
                SpecificText.m_sPenPeakWidth_Th, 
                SpecificText.m_sPenPeak_4Traces_Th, 
                SpecificText.m_sPenPeak_5Traces_Th,
                SpecificText.m_sPenPeak_5Traces_PeakPwr_Th, 
                SpecificText.m_sPenPeak_Th, 
                SpecificText.m_sPenPeakCheck_AreaUP_Pwr_TH, 
                SpecificText.m_sErrorMessage 
            };

            string[] sFWParameterTypeName_Array = null;

            if (m_eSubStep == SubTuningStep.PCCONTACT)
            {
                sFWParameterTypeName_Array = new string[15] 
                { 
                    SpecificText.m_sRanking, 
                    SpecificText.m_sFileName, 
                    SpecificText.m_sPH1, 
                    SpecificText.m_sPH2, 
                    SpecificText.m_sFrequency, 
                    SpecificText.m_sRXTraceNumber, 
                    SpecificText.m_sTXTraceNumber, 
                    SpecificText.m_sPenPeak_Th, 
                    SpecificText.m_sPenPeakWidth_Th, 
                    SpecificText.m_sPenPeak_2Traces_Th, 
                    SpecificText.m_sPenPeak_1Traces_Th, 
                    SpecificText.m_sPenPeak_4Traces_Th,
                    SpecificText.m_sPenPeak_5Traces_Th, 
                    SpecificText.m_sPenPeak_5Traces_PeakPwr_Th, 
                    SpecificText.m_sPenPeakCheck_AreaUP_Pwr_TH 
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
                            {
                                if (m_cDataInfo_List[nDataIndex].m_nP0_DetectTime_Index == 1)
                                    sP0_Detect_Time = SpecificText.m_sP0_Detect_Time_800;
                                else
                                    sP0_Detect_Time = SpecificText.m_sP0_Detect_Time_400;
                            }

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
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak1Tr.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak2Tr.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak3Tr.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak4Tr.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak5Tr.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak_1Traces_Th.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak_2Traces_Th.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeakWidth_Th.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak_4Traces_Th.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak_5Traces_Th.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak_5Traces_PeakPwr_Th.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeak_Th.ToString()));
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nPenPeakCheck_AreaUP_Pwr_TH.ToString()));

                            string sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sErrorMessage);

                            if (ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode != "")
                                sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sRecordErrorMessage);

                            sw.WriteLine(sErrorMessage);

                            break;
                        }
                    }
                }

                if (m_eSubStep == SubTuningStep.PCCONTACT)
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
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                                sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nRXTraceNumber.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nTXTraceNumber.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nFPenPeak_Th.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nFPenPeakWidth_Th.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nFPenPeak_2Traces_Th.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nFPenPeak_1Traces_Th.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nFPenPeak_4Traces_Th.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nFPenPeak_5Traces_Th.ToString()));
                                sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nFPenPeak_5Traces_PeakPwr_Th.ToString()));
                                sw.WriteLine(string.Format("{0}", m_cDataValue_List[nDataIndex].m_nFPenPeakCheck_AreaUP_Pwr_TH.ToString()));

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
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                                sw.Write(string.Format("{0},", FlowRobot.TOUCHLINE.ToString()));
                                sw.Write(string.Format("{0},", FlowRecord.TRX.ToString()));
                                sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                                sw.WriteLine(string.Format("{0}", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));
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
                if (m_eSubStep == SubTuningStep.PCHOVER_2ND)
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

        private void ComputePenPeakParameter()
        {
            foreach (DataValue cDataValue in m_cDataValue_List)
            {
                //PenPeak_1Traces_Th
                cDataValue.m_nPenPeak_1Traces_Th = (int)Math.Round(cDataValue.m_nPenPeak1Tr * m_dPeakCheckRatio, 0, MidpointRounding.AwayFromZero);

                //PenPeak_2Traces_Th
                cDataValue.m_nPenPeak_2Traces_Th = (int)Math.Round(cDataValue.m_nPenPeak2Tr * m_dPeakCheckRatio, 0, MidpointRounding.AwayFromZero);

                //PenPeakWidth_Th
                cDataValue.m_nPenPeakWidth_Th = (int)Math.Round(cDataValue.m_nPenPeak3Tr * m_dPeakCheckRatio, 0, MidpointRounding.AwayFromZero);

                //PenPeak_4Traces_Th 
                cDataValue.m_nPenPeak_4Traces_Th = (int)Math.Round(cDataValue.m_nPenPeak4Tr * m_dPeakCheckRatio, 0, MidpointRounding.AwayFromZero);

                //PenPeak_5Traces_Th 
                cDataValue.m_nPenPeak_5Traces_Th = (int)Math.Round(cDataValue.m_nPenPeak5Tr * m_dPeakCheckRatio, 0, MidpointRounding.AwayFromZero);

                //PenPeak_5Traces_PeakPwr_Th 
                cDataValue.m_nPenPeak_5Traces_PeakPwr_Th = (int)Math.Round(cDataValue.m_nPenPeak1Tr * m_dPeakCheckRatio5T, 0, MidpointRounding.AwayFromZero);

                //PenPeak_Th
                cDataValue.m_nPenPeak_Th = (int)Math.Round(cDataValue.m_nPenPeak1Tr * m_dPeakCheckRatio3T, 0, MidpointRounding.AwayFromZero);

                //PenPeakCheck_AreaUP_Pwr_TH
                cDataValue.m_nPenPeakCheck_AreaUP_Pwr_TH = (int)Math.Round((double)(cDataValue.m_nPenPeak_1Traces_Th + cDataValue.m_nPenPeak_2Traces_Th) / 2, 0, MidpointRounding.AwayFromZero);
            }
        }

        private void ComputeFinalPenPeakParameter()
        {
            foreach (DataValue cDataValue in m_cDataValue_List)
            {
                //PenPeak_1Traces_Th
                cDataValue.m_nFPenPeak_1Traces_Th = ComputeMaxValue(cDataValue.m_nPenPeak_1Traces_Th_H1st, cDataValue.m_nPenPeak_1Traces_Th_H2nd, cDataValue.m_nPenPeak_1Traces_Th);

                //PenPeak_2Traces_Th
                cDataValue.m_nFPenPeak_2Traces_Th = ComputeMaxValue(cDataValue.m_nPenPeak_2Traces_Th_H1st, cDataValue.m_nPenPeak_2Traces_Th_H2nd, cDataValue.m_nPenPeak_2Traces_Th);

                //PenPeakWidth_Th
                cDataValue.m_nFPenPeakWidth_Th = ComputeMaxValue(cDataValue.m_nPenPeakWidth_Th_H1st, cDataValue.m_nPenPeakWidth_Th_H2nd, cDataValue.m_nPenPeakWidth_Th);

                //PenPeak_4Traces_Th
                cDataValue.m_nFPenPeak_4Traces_Th = ComputeMaxValue(cDataValue.m_nPenPeak_4Traces_Th_H1st, cDataValue.m_nPenPeak_4Traces_Th_H2nd, cDataValue.m_nPenPeak_4Traces_Th);

                //PenPeak_5Traces_Th
                cDataValue.m_nFPenPeak_5Traces_Th = ComputeMaxValue(cDataValue.m_nPenPeak_5Traces_Th_H1st, cDataValue.m_nPenPeak_5Traces_Th_H2nd, cDataValue.m_nPenPeak_5Traces_Th);

                //PenPeak_5Traces_PeakPwr_Th
                cDataValue.m_nFPenPeak_5Traces_PeakPwr_Th = ComputeMaxValue(cDataValue.m_nPenPeak_5Traces_PeakPwr_Th_H1st, cDataValue.m_nPenPeak_5Traces_PeakPwr_Th_H2nd, 
                                                                            cDataValue.m_nPenPeak_5Traces_PeakPwr_Th);

                //PenPeak_Th
                cDataValue.m_nFPenPeak_Th = ComputeMaxValue(cDataValue.m_nPenPeak_Th_H1st, cDataValue.m_nPenPeak_Th_H2nd, cDataValue.m_nPenPeak_Th);

                //PenPeakCheck_AreaUP_Pwr_TH
                cDataValue.m_nFPenPeakCheck_AreaUP_Pwr_TH = ComputeMaxValue(cDataValue.m_nPenPeakCheck_AreaUP_Pwr_TH_H1st, cDataValue.m_nPenPeakCheck_AreaUP_Pwr_TH_H2nd, 
                                                                            cDataValue.m_nPenPeakCheck_AreaUP_Pwr_TH);
            }
        }

        private int ComputeMaxValue(int nValue_1, int nValue_2, int nValue_3)
        {
            int nMaxValue = 0;

            nMaxValue = Math.Max(nValue_1, nValue_2);
            nMaxValue = Math.Max(nMaxValue, nValue_3);

            return nMaxValue;
        }

        private bool GetPreviousStepValue(List<PeakCheckTuningParameter> cParameter_List, DataInfo cDataInfo, DataValue cDataValue)
        {
            bool bErrorFlag = true;

            for (int nParameterIndex = 0; nParameterIndex < cParameter_List.Count; nParameterIndex++)
            {
                if (cParameter_List[nParameterIndex].m_nPH1 == cDataInfo.m_nReadPH1 &&
                    cParameter_List[nParameterIndex].m_nPH2 == cDataInfo.m_nReadPH2)
                {
                    int nGetParameterFlag = 0;

                    if (cParameter_List[nParameterIndex].m_nPenPeak_1Traces_Th_PCH1st != -1)
                    {
                        cDataValue.m_nPenPeak_1Traces_Th_H1st = cParameter_List[nParameterIndex].m_nPenPeak_1Traces_Th_PCH1st;
                        nGetParameterFlag |= 0x0001;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_2Traces_Th_PCH1st != -1)
                    {
                        cDataValue.m_nPenPeak_2Traces_Th_H1st = cParameter_List[nParameterIndex].m_nPenPeak_2Traces_Th_PCH1st;
                        nGetParameterFlag |= 0x0002;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeakWidth_Th_PCH1st != -1)
                    {
                        cDataValue.m_nPenPeakWidth_Th_H1st = cParameter_List[nParameterIndex].m_nPenPeakWidth_Th_PCH1st;
                        nGetParameterFlag |= 0x0004;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_4Traces_Th_PCH1st != -1)
                    {
                        cDataValue.m_nPenPeak_4Traces_Th_H1st = cParameter_List[nParameterIndex].m_nPenPeak_4Traces_Th_PCH1st;
                        nGetParameterFlag |= 0x0008;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_5Traces_Th_PCH1st != -1)
                    {
                        cDataValue.m_nPenPeak_5Traces_Th_H1st = cParameter_List[nParameterIndex].m_nPenPeak_5Traces_Th_PCH1st;
                        nGetParameterFlag |= 0x0010;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_5Traces_PeakPwr_Th_PCH1st != -1)
                    {
                        cDataValue.m_nPenPeak_5Traces_PeakPwr_Th_H1st = cParameter_List[nParameterIndex].m_nPenPeak_5Traces_PeakPwr_Th_PCH1st;
                        nGetParameterFlag |= 0x0020;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_Th_PCH1st != -1)
                    {
                        cDataValue.m_nPenPeak_Th_H1st = cParameter_List[nParameterIndex].m_nPenPeak_Th_PCH1st;
                        nGetParameterFlag |= 0x0040;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeakCheck_AreaUP_Pwr_TH_PCH1st != -1)
                    {
                        cDataValue.m_nPenPeak_5Traces_PeakPwr_Th_H1st = cParameter_List[nParameterIndex].m_nPenPeakCheck_AreaUP_Pwr_TH_PCH1st;
                        nGetParameterFlag |= 0x0080;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_1Traces_Th_PCH2nd != -1)
                    {
                        cDataValue.m_nPenPeak_1Traces_Th_H2nd = cParameter_List[nParameterIndex].m_nPenPeak_1Traces_Th_PCH2nd;
                        nGetParameterFlag |= 0x0100;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_2Traces_Th_PCH2nd != -1)
                    {
                        cDataValue.m_nPenPeak_2Traces_Th_H2nd = cParameter_List[nParameterIndex].m_nPenPeak_2Traces_Th_PCH2nd;
                        nGetParameterFlag |= 0x0200;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeakWidth_Th_PCH2nd != -1)
                    {
                        cDataValue.m_nPenPeakWidth_Th_H2nd = cParameter_List[nParameterIndex].m_nPenPeakWidth_Th_PCH2nd;
                        nGetParameterFlag |= 0x0400;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_4Traces_Th_PCH2nd != -1)
                    {
                        cDataValue.m_nPenPeak_4Traces_Th_H2nd = cParameter_List[nParameterIndex].m_nPenPeak_4Traces_Th_PCH2nd;
                        nGetParameterFlag |= 0x0800;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_5Traces_Th_PCH2nd != -1)
                    {
                        cDataValue.m_nPenPeak_5Traces_Th_H2nd = cParameter_List[nParameterIndex].m_nPenPeak_5Traces_Th_PCH2nd;
                        nGetParameterFlag |= 0x1000;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_5Traces_PeakPwr_Th_PCH2nd != -1)
                    {
                        cDataValue.m_nPenPeak_5Traces_PeakPwr_Th_H2nd = cParameter_List[nParameterIndex].m_nPenPeak_5Traces_PeakPwr_Th_PCH2nd;
                        nGetParameterFlag |= 0x2000;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeak_Th_PCH2nd != -1)
                    {
                        cDataValue.m_nPenPeak_Th_H2nd = cParameter_List[nParameterIndex].m_nPenPeak_Th_PCH2nd;
                        nGetParameterFlag |= 0x4000;
                    }

                    if (cParameter_List[nParameterIndex].m_nPenPeakCheck_AreaUP_Pwr_TH_PCH2nd != -1)
                    {
                        cDataValue.m_nPenPeak_5Traces_PeakPwr_Th_H2nd = cParameter_List[nParameterIndex].m_nPenPeakCheck_AreaUP_Pwr_TH_PCH2nd;
                        nGetParameterFlag |= 0x8000;
                    }

                    if (nGetParameterFlag == 0xFFFF)
                        bErrorFlag = false;

                    if (cDataValue.m_nNoiseDigiGain_Beacon_Rx == -1)
                        cDataValue.m_nNoiseDigiGain_Beacon_Rx = cParameter_List[nParameterIndex].m_nNoiseDigiGain_Beacon_Rx;

                    break;
                }
            }

            return !bErrorFlag;
        }

        private void ComputeFilterValueByDigiGainRatio(ref int nRXFilterValue, int nFileIndex)
        {
            if (m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Rx != -1 && m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Rx != -1)
            {
                double dDigiGainRatio = (double)m_cDataInfo_List[nFileIndex].m_nDigiGain_Beacon_Rx / (double)m_cDataValue_List[nFileIndex].m_nNoiseDigiGain_Beacon_Rx;
                dDigiGainRatio = (dDigiGainRatio < 0) ? 0 : dDigiGainRatio;
                nRXFilterValue = (int)(nRXFilterValue * dDigiGainRatio);
                m_cDataValue_List[nFileIndex].m_nFilterRXValue = nRXFilterValue;
            }
        }
    }
}
