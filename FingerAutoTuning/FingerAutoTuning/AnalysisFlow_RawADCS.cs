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
    public class AnalysisFlow_RawADCS : AnalysisFlow
    {
        private ICGenerationType m_eICGenerationType = ICGenerationType.None;
        private ICSolutionType m_eICSolutionType = ICSolutionType.NA;

        private int m_nFitADCLB = 3000;
        private int m_nFitADCHB = 6000;

        private const int m_nIQ_BSHLB = 0;
        private const int m_nIQ_BSHHB = 42;

        public class DataInfo
        {
            public string m_sFileName = "";
            public ICGenerationType m_eICGenerationType = ICGenerationType.None;
            public ICSolutionType m_eICSolutionType = ICSolutionType.NA;
            public bool m_bGen7EnableHWTXN = false;
            public bool m_bGen6or7EnableFWTX4 = false;
            public int m_nSetIndex = -1;
            public int m_nSELCValue = -1;
            public int m_nVSELValue = -1;
            public int m_nLGValue = -1;
            public int m_nSELGMValue = -1;
            public int m_nIQ_BSH_0Value = 0;
            public int m_nDFT_NUMValue = 0;
            public int m_nRXTraceNumber = 0;
            public int m_nTXTraceNumber = 0;

            public double m_dMeanMinusADC_Mean = 0.0;
            public double m_dMeanMinusADC_Std = 0.0;
            public int m_nMeanMinusADC_Max = 0;
            public int m_nMeanMinusADC_Min = 0;
            public double m_dMeanMinusADC_NormalizeStdPCT = 0.0;

            public double m_dADCMean = 0.0;
            public uint m_nADCMax = 0;
            public double m_dRawADC = 0.0;
            public double m_dRawADCPercentage = 0.0;

            public int m_nSuggestIQ_BSH_0 = 0;

            public double m_dRealityADCMean = 0.0;
            public uint m_nRealityADCMax = 0;
        }

        public class DataInfoComparer : IComparer<DataInfo>
        {
            public int Compare(DataInfo cDataInfo1, DataInfo cDataInfo2)
            {
                if (m_nCompareOperator == m_nCOMPARE_RawADC50PCT)
                    return Math.Abs(cDataInfo1.m_dRawADCPercentage - 50.0).CompareTo(Math.Abs(cDataInfo2.m_dRawADCPercentage - 50.0));
                else if (m_nCompareOperator == m_nCOMPARE_NormalizeDifferStdPCT)
                    return cDataInfo1.m_dMeanMinusADC_NormalizeStdPCT.CompareTo(cDataInfo2.m_dMeanMinusADC_NormalizeStdPCT);
                else if (m_nCompareOperator == m_nCOMPARE_RawADCPCT)
                    return cDataInfo1.m_dRawADCPercentage.CompareTo(cDataInfo2.m_dRawADCPercentage);
                else
                    return 1;
            }
        }

        private List<DataInfo> m_cDataInfo_List = new List<DataInfo>();

        public AnalysisFlow_RawADCS(frmMain.FlowStep cFlowStep, string sLogDirectoryPath, string sH5LogDirectoryPath, bool bGenerateH5Data, frmMain cfrmParent, string sProjectName)
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
            m_nFitADCLB = ParamFingerAutoTuning.m_nRawADCSFitADCLB;
            m_nFitADCHB = ParamFingerAutoTuning.m_nRawADCSFitADCHB;
        }

        public override void InitializeSourceDataList()
        {
            m_sSourceData_List.Add(MainConstantParameter.m_sDATATYPE_ADC);
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

            if (SaveAnalysisFile() == false)
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
                RawADCS_FileCheckInfo cFileCheckInfo = new RawADCS_FileCheckInfo();
                List<uint[,]> nFrameData_List = new List<uint[,]>();

                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                if (CheckFileInfo(ref cFileCheckInfo, srFile, sFileName) == false)
                    return false;

                if (CheckFileInfoIdentical(cFileCheckInfo, sFileName) == false)
                    return false;

                DataInfo cDataInfo = new DataInfo();
                cDataInfo.m_sFileName = sFileName;
                cDataInfo.m_eICGenerationType = cFileCheckInfo.m_eICGenerationType;
                cDataInfo.m_eICSolutionType = cFileCheckInfo.m_eICSolutionType;
                cDataInfo.m_bGen7EnableHWTXN = cFileCheckInfo.m_bGen7EnableHWTXN;
                cDataInfo.m_bGen6or7EnableFWTX4 = cFileCheckInfo.m_bGen6or7EnableFWTX4;
                cDataInfo.m_nSetIndex = cFileCheckInfo.m_nSetIndex;
                cDataInfo.m_nSELCValue = cFileCheckInfo.m_nReadSELC;
                cDataInfo.m_nVSELValue = cFileCheckInfo.m_nReadVSEL;
                cDataInfo.m_nLGValue = cFileCheckInfo.m_nReadLG;
                cDataInfo.m_nSELGMValue = cFileCheckInfo.m_nReadSELGM;
                cDataInfo.m_nDFT_NUMValue = cFileCheckInfo.m_nReadDFT_NUM;
                cDataInfo.m_nIQ_BSH_0Value = cFileCheckInfo.m_nReadIQ_BSH_0;
                cDataInfo.m_nRXTraceNumber = cFileCheckInfo.m_nRXTraceNumber;
                cDataInfo.m_nTXTraceNumber = cFileCheckInfo.m_nTXTraceNumber;
                m_cDataInfo_List.Add(cDataInfo);
                int nListIndex = m_cDataInfo_List.Count - 1;
                uint[,] nMeanData_Array = new uint[cFileCheckInfo.m_nTXTraceNumber, cFileCheckInfo.m_nRXTraceNumber];

                if (nListIndex == 0)
                {
                    m_eICGenerationType = cFileCheckInfo.m_eICGenerationType;
                    m_eICSolutionType = cFileCheckInfo.m_eICSolutionType;
                }
                else if (m_eICGenerationType != cFileCheckInfo.m_eICGenerationType)
                {
                    m_sErrorMessage = string.Format("Read IC Generation Type Not Match in {0}", sFileName);
                    return false;
                }
                else if (m_eICSolutionType != cFileCheckInfo.m_eICSolutionType)
                {
                    m_sErrorMessage = string.Format("Read IC Solution Type Not Match in {0}", sFileName);
                    return false;
                }

                srFile = new StreamReader(sFilePath, Encoding.Default);

                if (GetFrameData(ref nFrameData_List, cFileCheckInfo, srFile, sFileName) == false)
                    return false;

                ComputeMeanData(ref nMeanData_Array, nFrameData_List, nListIndex, cFileCheckInfo, sFileName);

                ComputeReferenceData(nMeanData_Array, nFrameData_List, nListIndex, cFileCheckInfo, sFileName);

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

        private bool CheckFileInfo(ref RawADCS_FileCheckInfo cFileCheckInfo, StreamReader srFile, string sFileName)
        {
            string sLine = "";
            bool bGetICGenerationTypeFlag = false;

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSubString_Array = sLine.Split(',');

                    if (sSubString_Array.Length >= 2)
                    {
                        if (sSubString_Array[0] == "ICGenerationType")
                        {
                            bGetICGenerationTypeFlag = true;
                            int nICGenerationType = (int)ICGenerationType.None;
                            Int32.TryParse(sSubString_Array[1], out nICGenerationType);

                            if (nICGenerationType == (int)ICGenerationType.None)
                                cFileCheckInfo.m_eICGenerationType = ICGenerationType.None;
                            else if (nICGenerationType == (int)ICGenerationType.Gen8)
                                cFileCheckInfo.m_eICGenerationType = ICGenerationType.Gen8;
                            else if (nICGenerationType == (int)ICGenerationType.Gen7)
                                cFileCheckInfo.m_eICGenerationType = ICGenerationType.Gen7;
                            else if (nICGenerationType == (int)ICGenerationType.Gen6)
                                cFileCheckInfo.m_eICGenerationType = ICGenerationType.Gen6;
                            else if (nICGenerationType == (int)ICGenerationType.Gen9)
                                cFileCheckInfo.m_eICGenerationType = ICGenerationType.Gen9;
                            else if (nICGenerationType == (int)ICGenerationType.Other)
                                cFileCheckInfo.m_eICGenerationType = ICGenerationType.Other;
                        }
                        else if (sSubString_Array[0] == "ICSolutionType")
                        {
                            int nICSolutionType = (int)ICSolutionType.NA;
                            Int32.TryParse(sSubString_Array[1], out nICSolutionType);

                            if (nICSolutionType == (int)ICSolutionType.NA)
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.NA;
                            else if (nICSolutionType == (int)ICSolutionType.Solution_8F09)
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.Solution_8F09;
                            else if (nICSolutionType == (int)ICSolutionType.Solution_8F11)
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.Solution_8F11;
                            else if (nICSolutionType == (int)ICSolutionType.Solution_8F18)
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.Solution_8F18;
                            else if (nICSolutionType == (int)ICSolutionType.Solution_9F07)
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.Solution_9F07;
                            else if (nICSolutionType == (int)ICSolutionType.Solution_7318)
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.Solution_7318;
                            else if (nICSolutionType == (int)ICSolutionType.Solution_7315)
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.Solution_7315;
                            else if (nICSolutionType == (int)ICSolutionType.Solution_6315)
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.Solution_6315;
                            else if (nICSolutionType == (int)ICSolutionType.Solution_6308)
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.Solution_6308;
                            else if (nICSolutionType == (int)ICSolutionType.Solution_5015M)
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.Solution_5015M;
                            else
                                cFileCheckInfo.m_eICSolutionType = ICSolutionType.NA;
                        }
                        else if (sSubString_Array[0] == "Gen7EnableHWTXN")
                        {
                            int nValue = Convert.ToInt32(sSubString_Array[1]);

                            if (nValue == 1)
                                cFileCheckInfo.m_bGen7EnableHWTXN = true;
                            else
                                cFileCheckInfo.m_bGen7EnableHWTXN = false;
                        }
                        else if (sSubString_Array[0] == "Gen7EnableFWTX4" || sSubString_Array[0] == "Gen6or7EnableFWTX4")
                        {
                            int nValue = Convert.ToInt32(sSubString_Array[1]);

                            if (nValue == 1)
                                cFileCheckInfo.m_bGen6or7EnableFWTX4 = true;
                            else
                                cFileCheckInfo.m_bGen6or7EnableFWTX4 = false;
                        }
                        else if (sSubString_Array[0] == "TXTraceNumber")
                            Int32.TryParse(sSubString_Array[1], out cFileCheckInfo.m_nTXTraceNumber);
                        else if (sSubString_Array[0] == "RXTraceNumber")
                            Int32.TryParse(sSubString_Array[1], out cFileCheckInfo.m_nRXTraceNumber);
                        else if (sSubString_Array[0] == "SetIndex")
                            cFileCheckInfo.m_nSetIndex = Convert.ToInt32(sSubString_Array[1]);
                        else if (sSubString_Array[0] == "Set_SELC(Hex)")
                            cFileCheckInfo.m_nSetSELC = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Set_VSEL(Hex)")
                            cFileCheckInfo.m_nSetVSEL = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Set_LG(Hex)")
                            cFileCheckInfo.m_nSetLG = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Set_SELGM(Hex)")
                            cFileCheckInfo.m_nSetSELGM = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_SELC(Hex)")
                            cFileCheckInfo.m_nReadSELC = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_VSEL(Hex)")
                            cFileCheckInfo.m_nReadVSEL = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_LG(Hex)")
                            cFileCheckInfo.m_nReadLG = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_SELGM(Hex)")
                            cFileCheckInfo.m_nReadSELGM = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_IQ_BSH(Hex)")
                            cFileCheckInfo.m_nReadIQ_BSH_0 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "Read_DFT_NUM(Hex)")
                            cFileCheckInfo.m_nReadDFT_NUM = Convert.ToInt32(sSubString_Array[1], 16);
                    }

                    if (cFileCheckInfo.m_nSetSELC > -1 && cFileCheckInfo.m_nReadSELC > -1 &&
                        cFileCheckInfo.m_nSetVSEL > -1 && cFileCheckInfo.m_nReadVSEL > -1 &&
                        cFileCheckInfo.m_nSetLG > -1 && cFileCheckInfo.m_nReadLG > -1 &&
                        cFileCheckInfo.m_nSetSELGM > -1 && cFileCheckInfo.m_nReadSELGM > -1 &&
                        cFileCheckInfo.m_nReadIQ_BSH_0 > -1 && cFileCheckInfo.m_nReadDFT_NUM > -1 &&
                        cFileCheckInfo.m_nTXTraceNumber > -1 && cFileCheckInfo.m_nRXTraceNumber > -1)
                        break;
                }
            }
            finally
            {
                srFile.Close();
            }

            if (bGetICGenerationTypeFlag == false)
            {
                m_sErrorMessage = string.Format("Read \"ICGenerationType\" Not Exist in {0}", sFileName);
                return false;
            }

            if (cFileCheckInfo.m_nSetSELC == -1 || cFileCheckInfo.m_nReadSELC == -1 ||
                cFileCheckInfo.m_nSetSELC != cFileCheckInfo.m_nReadSELC)
            {
                m_sErrorMessage = string.Format("Read SELC Error in {0}[Set:{1} Read:{2}]", sFileName, cFileCheckInfo.m_nSetSELC, cFileCheckInfo.m_nReadSELC);
                return false;
            }

            if (cFileCheckInfo.m_nSetVSEL == -1 || cFileCheckInfo.m_nReadVSEL == -1 ||
                cFileCheckInfo.m_nSetVSEL != cFileCheckInfo.m_nReadVSEL)
            {
                m_sErrorMessage = string.Format("Read VSEL Error in {0}[Set:{1} Read:{2}]", sFileName, cFileCheckInfo.m_nSetVSEL, cFileCheckInfo.m_nReadVSEL);
                return false;
            }

            if (cFileCheckInfo.m_eICGenerationType == ICGenerationType.Gen6 ||
                cFileCheckInfo.m_eICGenerationType == ICGenerationType.Gen7 ||
                (cFileCheckInfo.m_eICGenerationType == ICGenerationType.Gen8 && cFileCheckInfo.m_eICSolutionType == ICSolutionType.Solution_8F18))
            {
                if (cFileCheckInfo.m_nSetLG == -1 || cFileCheckInfo.m_nReadLG == -1 || cFileCheckInfo.m_nSetLG != cFileCheckInfo.m_nReadLG)
                {
                    m_sErrorMessage = string.Format("Read LG Error in {0}[Set:{1} Read:{2}]", sFileName, cFileCheckInfo.m_nSetLG, cFileCheckInfo.m_nReadLG);
                    return false;
                }
            }

            if (cFileCheckInfo.m_nSetSELGM == -1 || cFileCheckInfo.m_nReadSELGM == -1 ||
                cFileCheckInfo.m_nSetSELGM != cFileCheckInfo.m_nReadSELGM)
            {
                m_sErrorMessage = string.Format("Read SELGM Error in {0}[Set:{1} Read:{2}]", sFileName, cFileCheckInfo.m_nSetSELGM, cFileCheckInfo.m_nReadSELGM);
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

            if (cFileCheckInfo.m_nReadIQ_BSH_0 == -1)
            {
                m_sErrorMessage = string.Format("Read IQ_BSH_0 Error in {0}[Value={1}]", sFileName, cFileCheckInfo.m_nReadIQ_BSH_0);
                return false;
            }

            if (cFileCheckInfo.m_nReadDFT_NUM == -1)
            {
                m_sErrorMessage = string.Format("Read DFT_NUM Error in {0}[Value={1}]", sFileName, cFileCheckInfo.m_nReadDFT_NUM);
                return false;
            }

            return true;
        }

        private bool CheckFileInfoIdentical(RawADCS_FileCheckInfo cFileCheckInfo, string sFileName)
        {
            for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
            {
                if (m_cDataInfo_List[nDataIndex].m_nIQ_BSH_0Value != cFileCheckInfo.m_nReadIQ_BSH_0)
                {
                    m_sErrorMessage = string.Format("IQ_BSH Unique Check Error in {0} and {1}[Value1={2} Value2={3}]", 
                                                    sFileName,
                                                    m_cDataInfo_List[nDataIndex].m_sFileName,
                                                    cFileCheckInfo.m_nReadIQ_BSH_0,
                                                    m_cDataInfo_List[nDataIndex].m_nIQ_BSH_0Value);
                    return false;
                }

                if (m_cDataInfo_List[nDataIndex].m_nDFT_NUMValue != cFileCheckInfo.m_nReadDFT_NUM)
                {
                    m_sErrorMessage = string.Format("DFT_NUM Unique Check Error in {0} and {1}[Value1={2} Value2={3}]", 
                                                    sFileName,
                                                    m_cDataInfo_List[nDataIndex].m_sFileName,
                                                    cFileCheckInfo.m_nReadDFT_NUM,
                                                    m_cDataInfo_List[nDataIndex].m_nDFT_NUMValue);
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
            }

            return true;
        }

        private bool GetFrameData(ref List<uint[,]> nFrameData_List, RawADCS_FileCheckInfo cFileCheckInfo, StreamReader srFile, string sFileName)
        {
            bool bGetFrameDataFlag = false;
            uint[,] nSingleFrame_Array = null;
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
                            nSingleFrame_Array = new uint[cFileCheckInfo.m_nTXTraceNumber, cFileCheckInfo.m_nRXTraceNumber];
                            continue;
                        }
                    }

                    if (bGetFrameDataFlag == true)
                    {
                        if (sSplit_Array.Length >= cFileCheckInfo.m_nRXTraceNumber)
                        {
                            for (int nRXIndex = 0; nRXIndex < cFileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                            {
                                if (Convert.ToInt32(sSplit_Array[nRXIndex]) < 0)
                                    nSingleFrame_Array[nTXCount, nRXIndex] = (uint)(Convert.ToInt32(sSplit_Array[nRXIndex]) + 65536);
                                else
                                    nSingleFrame_Array[nTXCount, nRXIndex] = Convert.ToUInt32(sSplit_Array[nRXIndex]);
                            }

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

        private void ComputeMeanData(ref uint[,] nMeanData_Array, List<uint[,]> nFrameData_List, int nListIndex, RawADCS_FileCheckInfo cFileCheckInfo, string sFileName)
        {
            for (int nTXIndex = 0; nTXIndex < cFileCheckInfo.m_nTXTraceNumber; nTXIndex++)
            {
                for (int nRXIndex = 0; nRXIndex < cFileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                {
                    uint nSumValue = 0;

                    for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
                        nSumValue += nFrameData_List[nFrameIndex][nTXIndex, nRXIndex];

                    uint nMeanValue = (uint)Math.Round((double)nSumValue / nFrameData_List.Count, 0, MidpointRounding.AwayFromZero);
                    nMeanData_Array[nTXIndex, nRXIndex] = nMeanValue;
                }
            }
        }

        private void ComputeReferenceData(uint[,] nMeanData_Array, List<uint[,]> nFrameData_List, int nListIndex, RawADCS_FileCheckInfo cFileCheckInfo, string sFileName)
        {
            List<long> lFrameValue_List = new List<long>();
            int nMaxValue = 0;
            int nMinValue = 0;

            for (int nFrameIndex = 0; nFrameIndex < nFrameData_List.Count; nFrameIndex++)
            {
                for (int nTXIndex = 0; nTXIndex < cFileCheckInfo.m_nTXTraceNumber; nTXIndex++)
                {
                    for (int nRXIndex = 0; nRXIndex < cFileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                    {
                        int nMeanMinusADCValue = (int)(nMeanData_Array[nTXIndex, nRXIndex] - nFrameData_List[nFrameIndex][nTXIndex, nRXIndex]);
                        lFrameValue_List.Add(nMeanMinusADCValue);

                        if (nFrameIndex == 0 && nTXIndex == 0 && nRXIndex == 0)
                        {
                            nMaxValue = nMeanMinusADCValue;
                            nMinValue = nMeanMinusADCValue;
                        }
                        else
                        {
                            if (nMeanMinusADCValue < nMinValue)
                                nMinValue = nMeanMinusADCValue;

                            if (nMeanMinusADCValue > nMaxValue)
                                nMaxValue = nMeanMinusADCValue;
                        }
                    }
                }
            }

            double dMeanValue = Math.Round(lFrameValue_List.Average(), 2, MidpointRounding.AwayFromZero);
            double dStdValue = Math.Round(MathMethod.ComputeStd(lFrameValue_List), 2, MidpointRounding.AwayFromZero);
            m_cDataInfo_List[nListIndex].m_dMeanMinusADC_Mean = dMeanValue;
            m_cDataInfo_List[nListIndex].m_dMeanMinusADC_Std = dStdValue;
            m_cDataInfo_List[nListIndex].m_nMeanMinusADC_Max = nMaxValue;
            m_cDataInfo_List[nListIndex].m_nMeanMinusADC_Min = nMinValue;

            long lSumValue = 0;
            uint nADCMaxValue = 0;
            int nValueCount = cFileCheckInfo.m_nTXTraceNumber * cFileCheckInfo.m_nRXTraceNumber;

            for (int nTXIndex = 0; nTXIndex < cFileCheckInfo.m_nTXTraceNumber; nTXIndex++)
            {
                for (int nRXIndex = 0; nRXIndex < cFileCheckInfo.m_nRXTraceNumber; nRXIndex++)
                {
                    lSumValue += nMeanData_Array[nTXIndex, nRXIndex];

                    if (nTXIndex == 0 && nRXIndex == 0)
                        nADCMaxValue = nMeanData_Array[nTXIndex, nRXIndex];
                    else
                    {
                        if (nMeanData_Array[nTXIndex, nRXIndex] > nADCMaxValue)
                            nADCMaxValue = nMeanData_Array[nTXIndex, nRXIndex];
                    }
                }
            }

            double dADCMeanValue = Math.Round((double)lSumValue / nValueCount, 2, MidpointRounding.AwayFromZero);
            double dRealityADCMeanValue = dADCMeanValue;
            m_cDataInfo_List[nListIndex].m_dRealityADCMean = dADCMeanValue;
            m_cDataInfo_List[nListIndex].m_nRealityADCMax = nADCMaxValue;

            if (cFileCheckInfo.m_eICGenerationType == ICGenerationType.Gen7)
            {
                if (cFileCheckInfo.m_bGen7EnableHWTXN == false && cFileCheckInfo.m_bGen6or7EnableFWTX4 == true)
                {
                    dADCMeanValue = Math.Round(dADCMeanValue / 2.0, 2, MidpointRounding.AwayFromZero);
                    nADCMaxValue = (uint)Math.Round((double)nADCMaxValue / 2.0, 0, MidpointRounding.AwayFromZero);
                }
            }
            else if (cFileCheckInfo.m_eICGenerationType == ICGenerationType.Gen6)
            {
                if (cFileCheckInfo.m_bGen6or7EnableFWTX4 == true)
                {
                    dADCMeanValue = Math.Round(dADCMeanValue / 2.0, 2, MidpointRounding.AwayFromZero);
                    nADCMaxValue = (uint)Math.Round((double)nADCMaxValue / 2.0, 0, MidpointRounding.AwayFromZero);
                }
            }

            m_cDataInfo_List[nListIndex].m_dADCMean = dADCMeanValue;
            m_cDataInfo_List[nListIndex].m_nADCMax = nADCMaxValue;

            double dNormalizeStdPCT = Math.Round((dStdValue / dADCMeanValue) * 100.0, 3, MidpointRounding.AwayFromZero);
            m_cDataInfo_List[nListIndex].m_dMeanMinusADC_NormalizeStdPCT = dNormalizeStdPCT;

            if (cFileCheckInfo.m_eICGenerationType == ICGenerationType.Gen6)
            {
                double dRawADC = Math.Round((((double)nADCMaxValue * Math.Pow(2, cFileCheckInfo.m_nReadIQ_BSH_0 - 14)) / cFileCheckInfo.m_nReadDFT_NUM) * 1.0, 2, MidpointRounding.AwayFromZero);
                m_cDataInfo_List[nListIndex].m_dRawADC = dRawADC;

                double dRawADCPercentage = Math.Round((dRawADC / (2048 * 1.0)) * 100.0, 2, MidpointRounding.AwayFromZero);
                m_cDataInfo_List[nListIndex].m_dRawADCPercentage = dRawADCPercentage;
            }
            else
            {
                double dRawADC = Math.Round((((double)nADCMaxValue * Math.Pow(2, cFileCheckInfo.m_nReadIQ_BSH_0 - 16)) / cFileCheckInfo.m_nReadDFT_NUM) * 4.0, 2, MidpointRounding.AwayFromZero);
                m_cDataInfo_List[nListIndex].m_dRawADC = dRawADC;

                double dRawADCPercentage = Math.Round((dRawADC / (2048 * 4.0)) * 100.0, 2, MidpointRounding.AwayFromZero);
                m_cDataInfo_List[nListIndex].m_dRawADCPercentage = dRawADCPercentage;
            }

            int nSuggestIQ_BSH_0 = cFileCheckInfo.m_nReadIQ_BSH_0;
            double dConvertADCMean = dRealityADCMeanValue;
            double dPreviousADCMean = dRealityADCMeanValue;

            if (dRealityADCMeanValue > m_nFitADCHB)
            {
                for (int nIQ_BSHIndex = cFileCheckInfo.m_nReadIQ_BSH_0 + 1; nIQ_BSHIndex <= m_nIQ_BSHHB; nIQ_BSHIndex++)
                {
                    dConvertADCMean = Math.Round(dConvertADCMean / 2.0, 2, MidpointRounding.AwayFromZero);

                    if (dConvertADCMean <= m_nFitADCHB)
                    {
                        if (dConvertADCMean >= m_nFitADCLB)
                        {
                            nSuggestIQ_BSH_0 = nIQ_BSHIndex;
                            break;
                        }
                        else
                        {
                            double dPreviousDiffer = dPreviousADCMean - m_nFitADCHB;
                            double dCurrentDiffer = m_nFitADCLB - dConvertADCMean;

                            if (dPreviousDiffer <= dCurrentDiffer)
                            {
                                nSuggestIQ_BSH_0 = nIQ_BSHIndex - 1;
                                break;
                            }
                            else
                            {
                                nSuggestIQ_BSH_0 = nIQ_BSHIndex;
                                break;
                            }
                        }
                    }
                    else
                    {
                        dPreviousADCMean = dConvertADCMean;
                    }
                }
            }
            else if (dRealityADCMeanValue < m_nFitADCLB)
            {
                for (int nIQ_BSHIndex = cFileCheckInfo.m_nReadIQ_BSH_0 - 1; nIQ_BSHIndex >= m_nIQ_BSHLB; nIQ_BSHIndex--)
                {
                    dConvertADCMean = Math.Round(dConvertADCMean * 2.0, 2, MidpointRounding.AwayFromZero);

                    if (dConvertADCMean >= m_nFitADCLB)
                    {
                        if (dConvertADCMean <= m_nFitADCHB)
                        {
                            nSuggestIQ_BSH_0 = nIQ_BSHIndex;
                            break;
                        }
                        else
                        {
                            double dPreviousDiffer = m_nFitADCLB - dPreviousADCMean;
                            double dCurrentDiffer = dConvertADCMean - m_nFitADCHB;

                            if (dPreviousDiffer <= dCurrentDiffer)
                            {
                                nSuggestIQ_BSH_0 = nIQ_BSHIndex + 1;
                                break;
                            }
                            else
                            {
                                nSuggestIQ_BSH_0 = nIQ_BSHIndex;
                                break;
                            }
                        }
                    }
                    else
                    {
                        dPreviousADCMean = dConvertADCMean;
                    }
                }
            }

            m_cDataInfo_List[nListIndex].m_nSuggestIQ_BSH_0 = nSuggestIQ_BSH_0;
        }

        private bool SaveAnalysisFile()
        {
            bool bErrorFlag = false;
            string sAnalysisFilePath = string.Format(@"{0}\Analysis.csv", m_sLogDirectoryPath);

            string[] sColumnHeader_Rank_Array = new string[] {
                "Rank", 
                "SELC", 
                "REG_VSEL", 
                "LG", 
                "SELGM", 
                "Suggest IQ_BSH", 
                "IQ_BSH", 
                "DFT_NUM", 
                "", 
                "ADC Mean", 
                "ADC Max", 
                "RawADC", 
                "RawADC PCT(%)", 
                "", 
                "Normalize Differ Std(%)", 
                "Differ Mean", 
                "Differ Std", 
                "Differ Max", 
                "Differ Min"
            };

            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                sColumnHeader_Rank_Array[1] = "c_MS_SELC";
                sColumnHeader_Rank_Array[2] = "c_MS_REG_VSEL";
                sColumnHeader_Rank_Array[3] = "c_MS_LG";
                sColumnHeader_Rank_Array[4] = "c_MS_SELGM";
                sColumnHeader_Rank_Array[5] = "Suggest c_MS_IQ_BSH_0";
                sColumnHeader_Rank_Array[6] = "c_MS_IQ_BSH_0";
            }
            else if (m_eICGenerationType == ICGenerationType.Gen7)
            {
                sColumnHeader_Rank_Array[1] = "_SELC";
                sColumnHeader_Rank_Array[2] = "_VF_VSEL";
                sColumnHeader_Rank_Array[3] = "_LG";
                sColumnHeader_Rank_Array[4] = "_SELGM";
                sColumnHeader_Rank_Array[5] = "Suggest _IQ_BSH0";
                sColumnHeader_Rank_Array[6] = "_IQ_BSH0";
            }
            else if (m_eICGenerationType == ICGenerationType.Gen6)
            {
                sColumnHeader_Rank_Array[1] = "_SELC";
                sColumnHeader_Rank_Array[2] = "_VF_VSEL";
                sColumnHeader_Rank_Array[3] = "_LG";
                sColumnHeader_Rank_Array[4] = "_SELGM";
                sColumnHeader_Rank_Array[5] = "Suggest _IQ_BSH";
                sColumnHeader_Rank_Array[6] = "_IQ_BSH";
            }

            FileStream fs = new FileStream(sAnalysisFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETYPE_ANALYSIS);

                m_nCompareOperator = m_nCOMPARE_RawADC50PCT;
                m_cDataInfo_List.Sort(new DataInfoComparer());

                sw.WriteLine("Raw ADC Approach 50 PCT Rank Information");

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
                    sw.Write(string.Format("{0},", cDataInfo.m_nSELCValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nVSELValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nLGValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSELGMValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSuggestIQ_BSH_0.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nIQ_BSH_0Value.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_dRealityADCMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nRealityADCMax.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawADC.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawADCPercentage.ToString("0.00")));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_NormalizeStdPCT.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_Mean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_Std.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nMeanMinusADC_Max.ToString()));
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_nMeanMinusADC_Min.ToString()));
                }

                sw.WriteLine();

                m_nCompareOperator = m_nCOMPARE_NormalizeDifferStdPCT;
                m_cDataInfo_List.Sort(new DataInfoComparer());

                sw.WriteLine("Differ Stdev Rank Information");

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
                    sw.Write(string.Format("{0},", cDataInfo.m_nSELCValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nVSELValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nLGValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSELGMValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSuggestIQ_BSH_0.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nIQ_BSH_0Value.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_dRealityADCMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nRealityADCMax.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawADC.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawADCPercentage.ToString("0.00")));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_NormalizeStdPCT.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_Mean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_Std.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nMeanMinusADC_Max.ToString()));
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_nMeanMinusADC_Min.ToString()));
                }

                sw.WriteLine();

                m_nCompareOperator = m_nCOMPARE_RawADCPCT;
                m_cDataInfo_List.Sort(new DataInfoComparer());

                sw.WriteLine("Raw ADC PCT Rank Information");

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
                    sw.Write(string.Format("{0},", cDataInfo.m_nSELCValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nVSELValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nLGValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSELGMValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSuggestIQ_BSH_0.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nIQ_BSH_0Value.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_dRealityADCMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nRealityADCMax.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawADC.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawADCPercentage.ToString("0.00")));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_NormalizeStdPCT.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_Mean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_Std.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nMeanMinusADC_Max.ToString()));
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_nMeanMinusADC_Min.ToString()));
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
            bool bErrorFlag = false;
            string sAnalysisFilePath = string.Format(@"{0}\Report.csv", m_sLogDirectoryPath);

            string[] sColumnHeader_Rank_Array = new string[] 
            {
                "Rank", 
                "SELC", 
                "REG_VSEL", 
                "LG", 
                "SELGM", 
                "Suggest IQ_BSH", 
                "IQ_BSH", "DFT_NUM", 
                "", 
                "ADC Mean", 
                "ADC Max", 
                "RawADC", 
                "RawADC PCT(%)", 
                "", 
                "Normalize Differ Std(%)", 
                "Differ Mean", 
                "Differ Std", 
                "Differ Max", 
                "Differ Min"
            };

            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                sColumnHeader_Rank_Array[1] = "c_MS_SELC";
                sColumnHeader_Rank_Array[2] = "c_MS_REG_VSEL";
                sColumnHeader_Rank_Array[3] = "c_MS_LG";
                sColumnHeader_Rank_Array[4] = "c_MS_SELGM";
                sColumnHeader_Rank_Array[5] = "Suggest c_MS_IQ_BSH_0";
                sColumnHeader_Rank_Array[6] = "c_MS_IQ_BSH_0";
            }
            else if (m_eICGenerationType == ICGenerationType.Gen7)
            {
                sColumnHeader_Rank_Array[1] = "_SELC";
                sColumnHeader_Rank_Array[2] = "_VF_VSEL";
                sColumnHeader_Rank_Array[3] = "_LG";
                sColumnHeader_Rank_Array[4] = "_SELGM";
                sColumnHeader_Rank_Array[5] = "Suggest _IQ_BSH0";
                sColumnHeader_Rank_Array[6] = "_IQ_BSH0";
            }
            else if (m_eICGenerationType == ICGenerationType.Gen6)
            {
                sColumnHeader_Rank_Array[1] = "_SELC";
                sColumnHeader_Rank_Array[2] = "_VF_VSEL";
                sColumnHeader_Rank_Array[3] = "_LG";
                sColumnHeader_Rank_Array[4] = "_SELGM";
                sColumnHeader_Rank_Array[5] = "Suggest _IQ_BSH";
                sColumnHeader_Rank_Array[6] = "_IQ_BSH";
            }

            FileStream fs = new FileStream(sAnalysisFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            
            try
            {
                Write_Tool_Information(sw, m_sFILETYPE_REPORT);

                m_nCompareOperator = m_nCOMPARE_RawADC50PCT;
                m_cDataInfo_List.Sort(new DataInfoComparer());

                sw.WriteLine("Raw ADC PCT Rank Information");

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
                    sw.Write(string.Format("{0},", cDataInfo.m_nSELCValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nVSELValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nLGValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSELGMValue.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nSuggestIQ_BSH_0.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nIQ_BSH_0Value.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_nDFT_NUMValue.ToString()));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_dRealityADCMean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nRealityADCMax.ToString()));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawADC.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dRawADCPercentage.ToString("0.00")));
                    sw.Write(",");
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_NormalizeStdPCT.ToString("0.000")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_Mean.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_dMeanMinusADC_Std.ToString("0.00")));
                    sw.Write(string.Format("{0},", cDataInfo.m_nMeanMinusADC_Max.ToString()));
                    sw.WriteLine(string.Format("{0}", cDataInfo.m_nMeanMinusADC_Min.ToString()));
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
    }
}
