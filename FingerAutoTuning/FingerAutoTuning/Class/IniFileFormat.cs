using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.Threading;

namespace FingerAutoTuning
{
    class IniFileFormat
    {
        public static MainStep[] eStep_Array = new MainStep[] 
        { 
            MainStep.FrequencyRank_Phase1,
            MainStep.FrequencyRank_Phase2,
            MainStep.AC_FrequencyRank,
            MainStep.Raw_ADC_Sweep,
            MainStep.Self_FrequencySweep,
            MainStep.Self_NCPNCNSweep 
        };

        //private const int SDStepFlag        = 0x0001;
        //private const int FRStepFlag        = 0x0002;
        //private const int nCheckStepFlag    = 0xFFFF;

        /*
        class StepDirectoryInfo
        {
            public string sStepDirectoryParameter = "";
            public string sStepFileDirectoryPath = "";
            public string sStepFileDirectoryName = "";
        }
        */

        /// <summary>
        /// 從參數設定檔讀取參數
        /// </summary>
        /// <param name="Section">Group Name</param>
        /// <param name="Key">Parameter Name</param>
        /// <param name="Default">預設值</param>
        /// <returns>回傳讀取的結果</returns>
        public static string ReadValue(string Section, string Key, string m_sPath, string Default = "")
        {
            return IniReadValue(Section, Key, m_sPath, Default);
        }

        /// <summary>
        /// 將Parameter數值寫入參數設定檔
        /// </summary>
        /// <param name="Section">Group Name</param>
        /// <param name="Key">Parameter Name</param>
        /// <param name="Value">參數數值</param>
        public static void WriteValue(string Section, string Key, string Value, string m_sPath, bool bAlwaysWrite = true, bool bSpace = true)
        {
            IniWriteValue(Section, Key, Value, m_sPath, bAlwaysWrite, bSpace);
        }

        private static string IniReadValue(string Section, string Key, string m_sPath, string Default = "")
        {
            StringBuilder temp = new StringBuilder(255);
            int i = Win32.GetPrivateProfileString(Section, Key, Default, temp, 255, m_sPath);

            if (temp != null)
                return temp.ToString();

            return Default;
        }

        private static void IniWriteValue(string Section, string Key, string Value, string m_sPath, bool bAlwaysWrite = true, bool bSpace = true)
        {
            if (bAlwaysWrite == false)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = Win32.GetPrivateProfileString(Section, Key, "DataNotExist!\\[N/A]", temp, 255, m_sPath);

                if (temp != null)
                {
                    if (temp.ToString() == "DataNotExist!\\[N/A]")
                        return;
                }
                else
                    return;
            }

            if (bSpace == true)
                Value = string.Format(" {0}", Value);

            Win32.WritePrivateProfileString(Section, Key, Value, m_sPath);
        }

        /*
        private static bool GetStepDirectory(ref string sErrorMessage, MainStep FlowStep, int nStepFileFlag = 0xFFFF)
        {
            if (FlowStep == SubTuningStep.HOVERLINE_1ST)
            {
                if ((nStepFileFlag & NoiseStepFlag) != 0)
                {
                    if (GetStepDirInfo(ref sErrorMessage, ref FormMain.sNoiseStepFileDirectory, ref FormMain.sNoiseStepRecPrjDirName, MainTuningStep.NO) == false)
                        return false;
                }
            }
            else if (SubStep == SubTuningStep.TILTTUNING_PTHF)
            {
                if ((nStepFileFlag & TiltNoiseStepFlag) != 0)
                {
                    if (GetStepDirInfo(ref sErrorMessage, ref FormMain.sTiltNoiseStepFileDirectory, ref FormMain.sTiltNoiseStepRecPrjDirName, MainTuningStep.TILTNO) == false)
                        return false;
                }

                if ((nStepFileFlag & DigitalTuningStepFlag) != 0)
                {
                    if (GetStepDirInfo(ref sErrorMessage, ref FormMain.sDTStepFileDirectory, ref FormMain.sDTStepRecPrjDirName, MainTuningStep.DIGITIALTUNING) == false)
                        return false;
                }
            }
            else if (SubStep == SubTuningStep.PRESSURESETTING || SubStep == SubTuningStep.LINEARITYTABLE)
            {
                if ((nStepFileFlag & DigitalTuningStepFlag) != 0)
                {
                    if (GetStepDirInfo(ref sErrorMessage, ref FormMain.sDTStepFileDirectory, ref FormMain.sDTStepRecPrjDirName, MainTuningStep.DIGITIALTUNING) == false)
                        return false;
                }
            }

            return true;
        }
        */

        /*
        public static bool Move_ResultList_Data(MainStep CurFlowStep)
        {
            if (File.Exists(frmMain.m_sResultListFilePath) == true)
            {
                MainStep[] MoveFlowStep = null;
                if (CurFlowStep == MainStep.SATURATIONEDGE_DETECT)
                {
                    MoveFlowStep = new MainStep[] { MainStep.SATURATIONEDGE_DETECT,
                                                    MainStep.FREQUENCY_RANK };
                }
                else if (CurFlowStep == MainStep.FREQUENCY_RANK)
                {
                    MoveFlowStep = new MainStep[] { MainStep.FREQUENCY_RANK };
                }

                if (MoveFlowStep != null)
                {
                    for (int nIndex = 0; nIndex < MoveFlowStep.Length; nIndex++)
                    {
                        string sStepFileDirPath = "";
                        string sStepRecPrjDirName = "";

                        GetStepDirInfo(ref sStepFileDirPath, ref sStepRecPrjDirName, MoveFlowStep[nIndex]);

                        string sFlowStepName = FlowStepNameMappingTable[MoveFlowStep[nIndex]];
                        string sMoveDirPath = string.Format(@"{0}\{1}", sStepFileDirPath, sFlowStepName);

                        if (Directory.Exists(sMoveDirPath) == true)
                        {
                            string sBackUpDirPath = string.Format(@"{0}\BackUp\{1}", frmMain.m_sLogDirectory, sStepRecPrjDirName);

                            if (Directory.Exists(sBackUpDirPath) == false)
                                Directory.CreateDirectory(sBackUpDirPath);

                            while (Directory.Exists(sBackUpDirPath) == false)
                                Thread.Sleep(10);

                            string sDestDirPath = string.Format(@"{0}\{1}", sBackUpDirPath, sFlowStepName);

                            if (Directory.Exists(sDestDirPath) == true)
                            {
                                Directory.Delete(sDestDirPath, true);
                                while (Directory.Exists(sDestDirPath))
                                    Thread.Sleep(10);
                            }

                            Directory.Move(sMoveDirPath, sDestDirPath);

                            if (Directory.GetFileSystemEntries(sStepFileDirPath).Length == 0)
                            {
                                if (Directory.Exists(sStepFileDirPath) == true)
                                {
                                    Directory.Delete(sStepFileDirPath, true);
                                    while (Directory.Exists(sStepFileDirPath))
                                        Thread.Sleep(10);
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
        */

        /*
        public static void Write_ResultListTxt_Data(MainStep CurFlowStep)
        {
            for (int nIndex = 0; nIndex < FlowStepArray.Length; nIndex++)
            {
                string sFlowStepName = FlowStepNameMappingTable[FlowStepArray[nIndex]];

                if (CurFlowStep == FlowStepArray[nIndex])
                    WriteValue("Step Directory", sFlowStepName, frmMain.m_sRecordLogDirectoryName, frmMain.m_sResultListFilePath, true, false);
                else
                {
                    if (CheckParamExist("Step Directory", sFlowStepName, frmMain.m_sResultListFilePath) == false)
                        WriteValue("Step Directory", sFlowStepName, "", frmMain.m_sResultListFilePath, true, false);
                }
            }
        }
        */

        public static bool CheckParameterExist(string Section, string Key, string m_sPath)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = Win32.GetPrivateProfileString(Section, Key, "DataNotExist!\\[N/A]", temp, 255, m_sPath);

            if (temp != null)
            {
                if (temp.ToString() == "DataNotExist!\\[N/A]")
                    return false;
            }
            else
                return false;

            return true;
        }

        public static void GetStepDirectoryInfo(ref string sStepDirectoryPath, ref string sStepDirectoryName, frmMain cfrmMain, MainStep eStep)
        {
            string sStep = StringConvert.m_dictMainStepMappingTable[eStep];

            string sDirectoryName = ReadValue("Step Directory", sStep, cfrmMain.m_sResultListFilePath, "");

            sStepDirectoryPath = string.Format(@"{0}\{1}", cfrmMain.m_sCurrentLogDirectoryPath, sDirectoryName);
            sStepDirectoryName = sDirectoryName;
        }

        /*
        private static bool GetStepDirInfo(ref string sErrorMessage, ref string StepFileDirectory, ref string StepRecPrjDirName, frmMain cfrmMain, MainStep CurFlowStep)
        {
            string sMainStep = FlowStepNameMappingTable[CurFlowStep];

            string sDirName = ReadValue("Step Directory", sMainStep, cfrmMain.m_sResultListFilePath, "");

            StepFileDirectory = string.Format(@"{0}\{1}", cfrmMain.m_sLogDirectory, sDirName);
            StepRecPrjDirName = sDirName;

            if (sDirName == "" || sDirName == null)
            {
                sErrorMessage = string.Format("No \"{0}\" Step Result Directory", sMainStep);
                return false;
            }

            return true;
        }
        */
    }
}
