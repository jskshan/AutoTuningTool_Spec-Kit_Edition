using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using FingerAutoTuningParameter;
using Elan;

namespace FingerAutoTuning
{
    public class AnalysisFlow
    {
        protected const int m_nCOMPARE_Frequency                = 0;
        protected const int m_nCOMPARE_Normalize                = 1;
        protected const int m_nCOMPARE_SetIndex                 = 2;
        protected const int m_nCOMPARE_RawADC50PCT              = 3;
        protected const int m_nCOMPARE_NormalizeDifferStdPCT    = 4;
        protected const int m_nCOMPARE_RawADCPCT                = 5;
        protected static int m_nCompareOperator = m_nCOMPARE_Frequency;

        protected const string m_sTOOLNAME = "FingerAutoTuningTool";

        protected const string m_sFILETYPE_ANALYSIS = "Analysis";
        protected const string m_sFILETYPE_REPORT = "Report";
        protected const string m_sFILETYPE_PROCESS = "Process";
        protected const string m_sFILETYPE_REPORT_STATISTIC = "ReportStatistic";
        protected const string m_sFILETYPE_STATISTIC = "Statistic";

        protected frmMain.FlowStep m_cFlowStep = null;
        protected frmMain m_cfrmParent = null;
        protected string m_sLogDirectoryPath = "";

        protected bool m_bGenerateH5Data = false;
        protected string m_sH5LogDirectoryPath = "";

        protected int m_nTotalFileCount = 0;
        protected int m_nAnalysisCount = 0;
        protected int m_nProgressIndex = 0;
        protected string m_sErrorMessage = "";

        protected string m_sProjectName = "";

        protected List<string> m_sSourceData_List = new List<string>();

        protected enum ReadDataType
        {
            Base,
            ADC,
            dV
        }

        public class FileCheckInfo
        {
            public int m_nSetIndex = -1;
            public int m_nSetPH1 = -1;
            public int m_nSetPH2 = -1;
            public int m_nSetPH3 = -1;
            public int m_nSetDFT_NUM = -1;
            public int m_nReadPH1 = -1;
            public int m_nReadPH2 = -1;
            public int m_nReadPH3 = -1;
            public int m_nReadDFT_NUM = -1;
            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;
            public int m_nFWIP_Option = -1;
        }

        public class RawADCS_FileCheckInfo
        {
            public ICGenerationType m_eICGenerationType = ICGenerationType.None;
            public ICSolutionType m_eICSolutionType = ICSolutionType.NA;

            public bool m_bGen7EnableHWTXN = false;
            public bool m_bGen6or7EnableFWTX4 = false;

            public int m_nSetIndex = -1;
            public int m_nSetSELC = -1;
            public int m_nSetVSEL = -1;
            public int m_nSetLG = -1;
            public int m_nSetSELGM = -1;
            public int m_nReadSELC = -1;
            public int m_nReadVSEL = -1;
            public int m_nReadLG = -1;
            public int m_nReadSELGM = -1;
            public int m_nReadIQ_BSH_0 = -1;
            public int m_nReadDFT_NUM = -1;
            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;
        }

        public class Self_FileCheckInfo
        {
            public TraceType m_eTraceType = TraceType.ALL;

            public int m_nSetIndex = -1;
            public int m_nSet_SELF_PH1 = -1;
            public int m_nSet_SELF_PH2E_LAT = -1;
            public int m_nSet_SELF_PH2E_LMT = -1;
            public int m_nSet_SELF_PH2_LAT = -1;
            public int m_nSet_SELF_PH2 = -1;
            public int m_nSetSelf_DFT_NUM = -1;
            public int m_nSetSelf_Gain = -1;
            public int m_nRead_SELF_PH1 = -1;
            public int m_nRead_SELF_PH2E_LAT = -1;
            public int m_nRead_SELF_PH2E_LMT = -1;
            public int m_nRead_SELF_PH2_LAT = -1;
            public int m_nRead_SELF_PH2 = -1;
            public int m_nReadSelf_DFT_NUM = -1;
            public int m_nReadSelf_Gain = -1;
            public int m_nReadSelf_CAG = -1;
            public int m_nReadSelf_IQ_BSH = -1;
            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;
            public bool m_bGetKValue = false;
            public int m_nRepeatIndex = 0;
            public int m_nNCPValue = -1;
            public int m_nNCNValue = -1;
            public int m_nCALValue = -1;
        }

        public virtual void InitializeParameter()
        {
        }

        public virtual void InitializeSourceDataList()
        {
        }

        public virtual void LoadAnalysisParameter()
        {
        }

        public virtual bool MainFlow(ref string sErrorMessage)
        {
            return true;
        }

        public bool GetDataCount()
        {
            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.InitialstatusstripMessage(0, "Data Analysis...");
            });

            foreach (string sDataType in m_sSourceData_List)
            {
                if (sDataType != MainConstantParameter.m_sDATATYPE_PREREPORT)
                {
                    string sDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sDataType);

                    if (Directory.Exists(sDirectoryPath) == false)
                    {
                        m_sErrorMessage = string.Format("{0} Log Directory Not Exist", sDataType);
                        return false;
                    }

                    int nFileCount = 0;

                    if (sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE ||
                        sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL)
                        nFileCount = Directory.GetFiles(sDirectoryPath, "*.txt").Count();
                    else
                        nFileCount = Directory.GetFiles(sDirectoryPath, "*.csv").Count();

                    if (nFileCount <= 0)
                    {
                        m_sErrorMessage = string.Format("No File in {0} Log Directory", sDataType);
                        return false;
                    }

                    foreach (string sOtherDataType in m_sSourceData_List)
                    {
                        if (sOtherDataType != sDataType)
                        {
                            string sOtherDataPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, sOtherDataType);

                            if (Directory.Exists(sOtherDataPath) == false)
                                continue;

                            int nOtherFileCount = 0;

                            if (sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE ||
                                sDataType == MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL)
                                nOtherFileCount = Directory.GetFiles(sDirectoryPath, "*.txt").Count();
                            else
                                nOtherFileCount = Directory.GetFiles(sDirectoryPath, "*.csv").Count();

                            if (nOtherFileCount != nFileCount)
                            {
                                m_sErrorMessage = string.Format("{0} and {1} File Count Not Match", sDataType, sOtherDataType);
                                return false;
                            }
                        }
                    }

                    m_nTotalFileCount += nFileCount;

                    if (m_cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                    {
                        if (sDataType == MainConstantParameter.m_sDATATYPE_BASE)
                            m_nTotalFileCount += nFileCount * 2;
                    }
                }
                else
                {
                    string sPreReportPath = string.Format(@"{0}\{1}\{2}.csv", m_sLogDirectoryPath, sDataType, sDataType);

                    if (File.Exists(sPreReportPath) == false)
                    {
                        m_sErrorMessage = string.Format("{0}\"{1}.csv File Not Exist", sDataType, sDataType);
                        return false;
                    }

                    m_nTotalFileCount++;
                }
            }

            return true;
        }

        public void SetErrorMessage(ref string sErrorMessage)
        {
            if (m_sErrorMessage != "")
                sErrorMessage = m_sErrorMessage;
        }

        public void CopyDataToH5Directory()
        {
            if (m_bGenerateH5Data == true)
            {
                string[] sSourceFilePath_Array = null;
                string[] sDestinationFilePath_Array = null;

                if (m_cFlowStep.m_eStep == MainStep.FrequencyRank_Phase1)
                {
                    sSourceFilePath_Array = new string[] 
                    { 
                        string.Format(@"{0}\Analysis.csv", m_sLogDirectoryPath),
                        string.Format(@"{0}\{1}\UniformityChart.jpg", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART),
                        string.Format(@"{0}\Report.csv", m_sLogDirectoryPath) 
                    };

                    sDestinationFilePath_Array = new string[] 
                    { 
                        string.Format(@"{0}\Analysis.csv", m_sH5LogDirectoryPath),
                        string.Format(@"{0}\{1}\UniformityChart.jpg", m_sH5LogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART),
                        string.Format(@"{0}\Report.csv", m_sH5LogDirectoryPath) 
                    };
                }
                else if (m_cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2)
                {
                    sSourceFilePath_Array = new string[] 
                    { 
                        string.Format(@"{0}\Analysis.csv", m_sLogDirectoryPath),
                        string.Format(@"{0}\{1}\RefValueChart.jpg", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART),
                        string.Format(@"{0}\Report.csv", m_sLogDirectoryPath) 
                    };

                    sDestinationFilePath_Array = new string[] 
                    { 
                        string.Format(@"{0}\Analysis.csv", m_sH5LogDirectoryPath),
                        string.Format(@"{0}\{1}\RefValueChart.jpg", m_sH5LogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART),
                        string.Format(@"{0}\Report.csv", m_sH5LogDirectoryPath) 
                    };
                }
                else if (m_cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
                {
                    sSourceFilePath_Array = new string[] 
                    { 
                        string.Format(@"{0}\Analysis.csv", m_sLogDirectoryPath),
                        string.Format(@"{0}\{1}\RefValueChart.jpg", m_sLogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART),
                        string.Format(@"{0}\Report.csv", m_sLogDirectoryPath) 
                    };

                    sDestinationFilePath_Array = new string[] 
                    { 
                        string.Format(@"{0}\Analysis.csv", m_sH5LogDirectoryPath),
                        string.Format(@"{0}\{1}\RefValueChart.jpg", m_sH5LogDirectoryPath, MainConstantParameter.m_sDATATYPE_CHART),
                        string.Format(@"{0}\Report.csv", m_sH5LogDirectoryPath) 
                    };
                }

                for (int nDataIndex = 0; nDataIndex < sSourceFilePath_Array.Length; nDataIndex++)
                {
                    if (File.Exists(sSourceFilePath_Array[nDataIndex]) == false)
                        continue;

                    AppCoreDefine.FileProcessFlow(AppCoreDefine.FileProcess.Copy, sSourceFilePath_Array[nDataIndex], sDestinationFilePath_Array[nDataIndex], false);
                }
            }
        }

        protected void UpdateProgressBar()
        {
            string sMessage = string.Format("Analysis : {0}/{1}", m_nAnalysisCount, m_nAnalysisCount);

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.Total, m_nAnalysisCount, sMessage);
            });
        }

        protected void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.OutputMessage(sMessage, bWarning);
            });

            m_cfrmParent.OutputDebugLog(sMessage);
        }

        protected void Write_Tool_Information(StreamWriter sw, string sFileType)
        {
            sw.WriteLine(string.Format("ToolName,{0}", m_sTOOLNAME));
            sw.WriteLine(string.Format("AutoTuningToolVersion,{0}", m_cfrmParent.m_sParentAPVersion));
            sw.WriteLine(string.Format("FingerAutoTuningVersion,{0}", m_cfrmParent.m_sAPVersion));
            sw.WriteLine(string.Format("ProjectName,{0}", m_sProjectName));
            sw.WriteLine(string.Format("FileType,{0}", sFileType));
            sw.WriteLine("=====================================================");
        }
    }
}
