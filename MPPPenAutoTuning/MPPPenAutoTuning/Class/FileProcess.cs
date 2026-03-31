using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
	class FileProcess
	{
        private frmMain m_cfrmMain;

        private SubTuningStep[] m_eSubStep_Array = new SubTuningStep[] 
        { 
            SubTuningStep.NO, 
            SubTuningStep.TILTNO_BHF,
            SubTuningStep.DIGIGAIN,
            SubTuningStep.TP_GAIN,
            SubTuningStep.PCCONTACT,
            SubTuningStep.CONTACTTRxS,
            SubTuningStep.TILTTUNING_BHF,
            SubTuningStep.PRESSURETABLE,
            SubTuningStep.LINEARITYTABLE 
        };

        public const int m_nMAINSTEP_NOISE             = 0x0001;
        public const int m_nMAINSTEP_TILTNOISE         = 0x0002;
        public const int m_nMAINSTEP_DIGIGAINTUNING    = 0x0080;
        public const int m_nMAINSTEP_TPGAINTUNING      = 0x0100;
        public const int m_nMAINSTEP_PEAKCHEACKTUNING  = 0x0040;
        public const int m_nMAINSTEP_DIGITALTUNING     = 0x0004;
        public const int m_nMAINSTEP_TILTTUNING        = 0x0008;
        public const int m_nMAINSTEP_PRESSURETUNING    = 0x0010;
        public const int m_nMAINSTEP_LINEARITYTUNING   = 0x0020;
        public static int m_nCheckStepFlag             = 0xFFFF;

        public static Dictionary<SubTuningStep, string> m_dictFlowStepNameMappingTable = new Dictionary<SubTuningStep,string>()
        { 
            {SubTuningStep.NO,              "Noise"},
            {SubTuningStep.TILTNO_BHF,      "Tilt Noise"},
            {SubTuningStep.DIGIGAIN,        "DigiGain Tuning"},
            {SubTuningStep.TP_GAIN,         "TP_Gain Tuning"},
            {SubTuningStep.PCCONTACT,       "PeakCheck Tuning"},
            {SubTuningStep.CONTACTTRxS,     "Digital Tuning"},
            {SubTuningStep.TILTTUNING_BHF,  "Tilt Tuning"},
            {SubTuningStep.PRESSURETABLE,   "Pressure Tuning"},
            {SubTuningStep.LINEARITYTABLE,  "Linearity Tuning"}
        };

        public const int m_nGETDIRINFO_ERROR = 0;
        public const int m_nGETDIRINFO_SUCCESS = 1;
        public const int m_nGETDIRINFO_DGTNOTEXIST = 2;

        /*
        private class StepDirectoryInfo
        {
            public string m_sStepDirectoryParameter = "";
            public string m_sStepFileDirectoryPath = "";
            public string m_sStepFileDirectoryName = "";
        }
        */

        public FileProcess(frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
        }

        /// <summary>
        /// 從參數設定檔讀取參數
        /// </summary>
        /// <param name="sSection">Group Name</param>
        /// <param name="sKey">Parameter Name</param>
        /// <param name="sPath">File Path</param>
        /// <param name="sDefault">預設值</param>
        /// <returns>回傳讀取的結果</returns>
        public static string ReadValue(string sSection, string sKey, string sFilePath, string sDefault = "")
        {
            return IniReadValue(sSection, sKey, sFilePath, sDefault);
        }

        /// <summary>
        /// 將Parameter數值寫入參數設定檔
        /// </summary>
        /// <param name="sSection">Group Name</param>
        /// <param name="sKey">Parameter Name</param>
        /// <param name="sValue">參數數值</param>
        /// <param name="sPath">File Path</param>
        /// <param name="bAlwaysWrite">Always Write</param>
        /// <param name="bSpace">Space</param>
        public static void WriteValue(string sSection, string sKey, string sValue, string sFilePath, bool bAlwaysWrite = true, bool bSpace = true)
        {
            IniWriteValue(sSection, sKey, sValue, sFilePath, bAlwaysWrite, bSpace);  
        }

        private static string IniReadValue(string sSection, string sKey, string sFilePath, string sDefault = "")
        {
            StringBuilder sb = new StringBuilder(255);
            int nResult = Win32.GetPrivateProfileString(sSection, sKey, sDefault, sb, 255, sFilePath);

            if (sb != null)
                return sb.ToString();

            return sDefault;
        }

        private static void IniWriteValue(string sSection, string sKey, string sValue, string sFilePath, bool bAlwaysWrite = true, bool bSpace = true)
        {
            if (bAlwaysWrite == false)
            {
                StringBuilder sb = new StringBuilder(255);
                int nResult = Win32.GetPrivateProfileString(sSection, sKey, "DataNotExist!\\[N/A]", sb, 255, sFilePath);

                if (sb != null)
                {
                    if (sb.ToString() == "DataNotExist!\\[N/A]")
                        return;
                }
                else
                    return;
            }

            if (bSpace == true)
                sValue = string.Format(" {0}", sValue);

            Win32.WritePrivateProfileString(sSection, sKey, sValue, sFilePath);
        }

        public int GetFlowStepDirectory(ref bool bGetStepInfo_DigiGainTuning, ref string sErrorMessage, FlowStep cFlowStep, int nStepFileFlag = 0xFFFF)
        {
            bGetStepInfo_DigiGainTuning = false;

            if (cFlowStep.m_eSubStep != SubTuningStep.HOVER_1ST && cFlowStep.m_eSubStep != SubTuningStep.TILTTUNING_PTHF &&
                cFlowStep.m_eSubStep != SubTuningStep.PRESSURESETTING && cFlowStep.m_eSubStep != SubTuningStep.LINEARITYTABLE &&
                cFlowStep.m_eSubStep != SubTuningStep.PCHOVER_1ST && cFlowStep.m_eSubStep != SubTuningStep.TILTNO_PTHF &&
                cFlowStep.m_eSubStep != SubTuningStep.DIGIGAIN && cFlowStep.m_eSubStep != SubTuningStep.TP_GAIN &&
                cFlowStep.m_eSubStep != SubTuningStep.HOVERTRxS && cFlowStep.m_eSubStep != SubTuningStep.CONTACTTRxS)
                return m_nGETDIRINFO_SUCCESS;

            string sFileDirectoryPath = "";
            string sRecordDirectoryName = "";

            CheckState cCheckState = new CheckState(m_cfrmMain);

            switch (cFlowStep.m_eSubStep)
            {
                case SubTuningStep.HOVER_1ST:
                case SubTuningStep.HOVERTRxS:
                case SubTuningStep.CONTACTTRxS:
                    if ((nStepFileFlag & m_nMAINSTEP_NOISE) != 0)
                    {
                        bool bResultFlag = GetStepDirInfo(ref sErrorMessage, ref sFileDirectoryPath, ref sRecordDirectoryName, MainTuningStep.NO);

                        m_cfrmMain.m_sFileDirectoryPath_Noise = sFileDirectoryPath;
                        m_cfrmMain.m_sRecordDirectoryName_Noise = sRecordDirectoryName;

                        if (bResultFlag == false)
                            return m_nGETDIRINFO_ERROR;
                    }

                    if (cFlowStep.m_eSubStep == SubTuningStep.HOVERTRxS || cFlowStep.m_eSubStep == SubTuningStep.CONTACTTRxS)
                    {
                        string sStepCodeName = StringConvert.m_dictSubStepCNMappingTable[SubTuningStep.NO];
                        m_cfrmMain.m_sReferenceFilePath_Noise = string.Format(@"{0}\{1}({2})\{3}\{4}", m_cfrmMain.m_sFileDirectoryPath_Noise, SpecificText.m_sResultText, sStepCodeName, SpecificText.m_sReferenceText, SpecificText.m_sReferenceFileName);
                    }

                    break;
                case SubTuningStep.TILTTUNING_PTHF:
                    if ((nStepFileFlag & m_nMAINSTEP_TILTNOISE) != 0)
                    {
                        bool bResultFlag = GetStepDirInfo(ref sErrorMessage, ref sFileDirectoryPath, ref sRecordDirectoryName, MainTuningStep.TILTNO);

                        m_cfrmMain.m_sFileDirectoryPath_TN = sFileDirectoryPath;
                        m_cfrmMain.m_sRecordDirectoryName_TN = sRecordDirectoryName;

                        if (bResultFlag == false)
                            return m_nGETDIRINFO_ERROR;
                    }

                    if ((nStepFileFlag & m_nMAINSTEP_DIGITALTUNING) != 0)
                    {
                        bool bResultFlag = GetStepDirInfo(ref sErrorMessage, ref sFileDirectoryPath, ref sRecordDirectoryName, MainTuningStep.DIGITALTUNING);

                        m_cfrmMain.m_sFileDirectoryPath_DT = sFileDirectoryPath;
                        m_cfrmMain.m_sRecordDirectoryName_DT = sRecordDirectoryName;

                        if (bResultFlag == false)
                            return m_nGETDIRINFO_ERROR;
                    }

                    break;
                case SubTuningStep.PRESSURESETTING:
                    if ((nStepFileFlag & m_nMAINSTEP_DIGITALTUNING) != 0)
                    {
                        bool bResultFlag = GetStepDirInfo(ref sErrorMessage, ref sFileDirectoryPath, ref sRecordDirectoryName, MainTuningStep.DIGITALTUNING);

                        m_cfrmMain.m_sFileDirectoryPath_DT = sFileDirectoryPath;
                        m_cfrmMain.m_sRecordDirectoryName_DT = sRecordDirectoryName;

                        if (bResultFlag == false)
                            return m_nGETDIRINFO_ERROR;
                    }

                    break;
                case SubTuningStep.LINEARITYTABLE:
                    if ((nStepFileFlag & m_nMAINSTEP_DIGITALTUNING) != 0)
                    {
                        bool bResultFlag = GetStepDirInfo(ref sErrorMessage, ref sFileDirectoryPath, ref sRecordDirectoryName, MainTuningStep.DIGITALTUNING);

                        m_cfrmMain.m_sFileDirectoryPath_DT = sFileDirectoryPath;
                        m_cfrmMain.m_sRecordDirectoryName_DT = sRecordDirectoryName;

                        if (bResultFlag == false)
                            return m_nGETDIRINFO_ERROR;
                    }

                    if (cCheckState.CheckIndependentStep(cFlowStep.m_eMainStep, cFlowStep.m_eSubStep) != CheckState.m_nSTEPSTATE_INDEPENDENT &&
                        ParamAutoTuning.m_nLTUseTP_GainCompensate != MainConstantParameter.m_nLTCOMPENSATE_DISABLE)
                    {
                        if ((nStepFileFlag & m_nMAINSTEP_TPGAINTUNING) != 0)
                        {
                            bool bResultFlag = GetStepDirInfo(ref sErrorMessage, ref sFileDirectoryPath, ref sRecordDirectoryName, MainTuningStep.TPGAINTUNING);

                            m_cfrmMain.m_sFileDirectoryPath_TPGT = sFileDirectoryPath;
                            m_cfrmMain.m_sRecordDirectoryName_TPGT = sRecordDirectoryName;

                            if (bResultFlag == false)
                                return m_nGETDIRINFO_ERROR;
                        }

                        string sStepCodeName = StringConvert.m_dictSubStepCNMappingTable[SubTuningStep.TP_GAIN];
                        m_cfrmMain.m_sReferenceFilePath_TPGT = string.Format(@"{0}\{1}({2})\{3}\{4}", m_cfrmMain.m_sFileDirectoryPath_TPGT, SpecificText.m_sResultText, sStepCodeName, SpecificText.m_sReferenceText, SpecificText.m_sReferenceFileName);
                    }

                    break;
                case SubTuningStep.PCHOVER_1ST:
                case SubTuningStep.TILTNO_PTHF:
                case SubTuningStep.DIGIGAIN:
                case SubTuningStep.TP_GAIN:
                    if ((nStepFileFlag & m_nMAINSTEP_NOISE) != 0)
                    {
                        bool bResultFlag = GetStepDirInfo(ref sErrorMessage, ref sFileDirectoryPath, ref sRecordDirectoryName, MainTuningStep.NO);

                        m_cfrmMain.m_sFileDirectoryPath_Noise = sFileDirectoryPath;
                        m_cfrmMain.m_sRecordDirectoryName_Noise = sRecordDirectoryName;

                        if (bResultFlag == false)
                            return m_nGETDIRINFO_ERROR;
                    }

                    if (cFlowStep.m_eSubStep == SubTuningStep.TILTNO_PTHF)
                    {
                        string sStepCodeName = StringConvert.m_dictSubStepCNMappingTable[SubTuningStep.NO];
                        m_cfrmMain.m_sTotalResultWRFilePath_Noise = string.Format(@"{0}\{1}({2})\{3}", m_cfrmMain.m_sFileDirectoryPath_Noise, SpecificText.m_sResultText, sStepCodeName, SpecificText.m_sResultFileName);
                    }

                    break;
                default:
                    break;
            }

            if (cCheckState.CheckSetDigiGain(cFlowStep.m_eMainStep, cFlowStep.m_eSubStep) == CheckState.m_nSETDIGIGAIN_COMPUTEVALUE)
            {
                if ((nStepFileFlag & m_nMAINSTEP_DIGIGAINTUNING) != 0)
                {
                    bool bResultFlag = GetStepDirInfo(ref sErrorMessage, ref sFileDirectoryPath, ref sRecordDirectoryName, MainTuningStep.DIGIGAINTUNING);

                    m_cfrmMain.m_sFileDirectoryPath_DGT = sFileDirectoryPath;
                    m_cfrmMain.m_sRecordDirectoryName_DGT = sRecordDirectoryName;

                    if (bResultFlag == false)
                        return m_nGETDIRINFO_DGTNOTEXIST;

                    bGetStepInfo_DigiGainTuning = true;
                }
            }

            return m_nGETDIRINFO_SUCCESS;
        }

        public bool ClearResultListData(SubTuningStep eSubStep, bool bFirstSetFlag = false)
        {
            if (eSubStep != SubTuningStep.NO && eSubStep != SubTuningStep.TILTNO_PTHF &&
                eSubStep != SubTuningStep.HOVER_1ST && eSubStep != SubTuningStep.TILTTUNING_PTHF &&
                eSubStep != SubTuningStep.PRESSURESETTING && eSubStep != SubTuningStep.LINEARITYTABLE &&
                eSubStep != SubTuningStep.PCHOVER_1ST && eSubStep != SubTuningStep.DIGIGAIN &&
                eSubStep != SubTuningStep.TP_GAIN)
                return true;

            if (File.Exists(m_cfrmMain.m_sResultListFilePath) == true)
            {
                string[] sFlowStepName_Array = null;

                switch (eSubStep)
                {
                    case SubTuningStep.NO:
                        sFlowStepName_Array = new string[] 
                        { 
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.NO],
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.TILTNO],
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.DIGITALTUNING],
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.TILTTUNING]
                        };
                        break;
                    case SubTuningStep.TILTNO_PTHF:
                        sFlowStepName_Array = new string[] 
                        { 
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.TILTNO],
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.TILTTUNING]
                        };
                        break;
                    case SubTuningStep.HOVER_1ST:
                        sFlowStepName_Array = new string[] 
                        { 
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.DIGITALTUNING],
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.TILTTUNING]
                        };
                        break;
                    case SubTuningStep.TILTTUNING_PTHF:
                        sFlowStepName_Array = new string[] 
                        { 
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.TILTTUNING]
                        };
                        break;
                    case SubTuningStep.PRESSURESETTING:
                        sFlowStepName_Array = new string[] 
                        { 
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.PRESSURETUNING]
                        };
                        break;
                    case SubTuningStep.LINEARITYTABLE:
                        sFlowStepName_Array = new string[] 
                        { 
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.LINEARITYTUNING]
                        };
                        break;
                    case SubTuningStep.PCHOVER_1ST:
                        sFlowStepName_Array = new string[] 
                        { 
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.PEAKCHECKTUNING]
                        };
                        break;
                    case SubTuningStep.DIGIGAIN:
                        sFlowStepName_Array = new string[] 
                        { 
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.DIGIGAINTUNING]
                        };
                        break;
                    case SubTuningStep.TP_GAIN:
                        sFlowStepName_Array = new string[] 
                        { 
                            StringConvert.m_dictMainStepMappingTable[MainTuningStep.TPGAINTUNING]
                        };
                        break;
                    default:
                        break;
                }

                if (sFlowStepName_Array != null)
                {
                    for (int nStepIndex = 0; nStepIndex < sFlowStepName_Array.Length; nStepIndex++)
                        WriteValue("Step Directory", sFlowStepName_Array[nStepIndex], "", m_cfrmMain.m_sResultListFilePath, true, false);
                }
                else
                {
                    if (bFirstSetFlag == false)
                        return false;
                }
            }
            else
            {
                if (bFirstSetFlag == false)
                    return false;
            }

            return true;
        }

        public void WriteResultListTxtFile(SubTuningStep eSubStep, int nSubStepState)
        {
            if ((nSubStepState & MainConstantParameter.m_nSTEPLOCATION_LAST) == 0)
                return;

            if (Directory.Exists(m_cfrmMain.m_sResultListDirectoryPath) == false)
                Directory.CreateDirectory(m_cfrmMain.m_sResultListDirectoryPath);

            for (int nStepIndex = 0; nStepIndex < m_eSubStep_Array.Length; nStepIndex++)
            {
                string sStepName = m_dictFlowStepNameMappingTable[m_eSubStep_Array[nStepIndex]];

                if (eSubStep == m_eSubStep_Array[nStepIndex])
                    WriteValue("Step Directory", sStepName, m_cfrmMain.m_sRecordDirectoryName, m_cfrmMain.m_sResultListFilePath, true, false);
                else
                {
                    if (CheckParameterExist("Step Directory", sStepName, m_cfrmMain.m_sResultListFilePath) == false)
                        WriteValue("Step Directory", sStepName, "", m_cfrmMain.m_sResultListFilePath, true, false);
                }
            }
        }

        private bool CheckParameterExist(string sSection, string sKey, string sFilePath)
        {
            StringBuilder sb = new StringBuilder(255);
            int nResult = Win32.GetPrivateProfileString(sSection, sKey, "DataNotExist!\\[N/A]", sb, 255, sFilePath);

            if (sb != null)
            {
                if (sb.ToString() == "DataNotExist!\\[N/A]")
                    return false;
            }
            else
                return false;

            return true;
        }

        private bool GetStepDirInfo(ref string sErrorMessage, ref string sStepFileDirectoryPath, ref string sStepRecentProjectDirectoryName, MainTuningStep eMainStep)
        {
            string sMainStep = StringConvert.m_dictMainStepMappingTable[eMainStep];
            string sDirectoryName = ReadValue("Step Directory", sMainStep, m_cfrmMain.m_sResultListFilePath, "");

            sStepFileDirectoryPath = string.Format(@"{0}\{1}", m_cfrmMain.m_sLogDirecotryPath, sDirectoryName);
            sStepRecentProjectDirectoryName = sDirectoryName;

            if (sDirectoryName == "" || sDirectoryName == null)
            {
                sErrorMessage = string.Format("No \"{0}\" Step Result Directory", sMainStep);
                return false;
            }

            if (Directory.Exists(sStepFileDirectoryPath) == false)
            {
                string sFileDirectoryPath = string.Format(@"{0}\ErrorBackUp\{1}", m_cfrmMain.m_sLogDirecotryPath, sDirectoryName);

                if (Directory.Exists(sFileDirectoryPath) == true)
                    sStepFileDirectoryPath = sFileDirectoryPath;
                else
                {
                    sErrorMessage = string.Format("\"{0}\" Step Result Directory Not Exist", sMainStep);
                    return false;
                }
            }

            return true;
        }
	}
}