using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using FingerAutoTuning;
using System.Linq;
using System.IO;
using Elan;

namespace FingerAutoTuningParameter
{
    public class ParamBase
    {
        protected const int nFILE_TYPE_UNKNOW = 0;
        protected const int nFILE_TYPE_INI = 1;
        protected const int nFILE_TYPE_XML = 2;
#if _USE_9F07_SOCKET
        protected const int nSTEP_NUMBER = 3;
#else
        protected const int nSTEP_NUMBER = 4;    //5;
#endif

        protected static string m_sSettingFilePath = "";
        protected static string m_sDefaultFilePath = "";
        protected static int m_nFileType = nFILE_TYPE_UNKNOW;
        public static bool m_bDataFormatError = false;

        public const string m_sStepName_FrequencyRank_Phase1 = "FrequencyRank Phase1";
        public const string m_sStepName_FrequencyRank_Phase2 = "FrequencyRank Phase2";
        public const string m_sStepName_AC_FrequencyRank = "AC FrequencyRank";
        public const string m_sStepName_Raw_ADC_Sweep = "Raw ADC Sweep";
        public const string m_sStepName_Self_FrequencySweep = "Self FrequencySweep";
        public const string m_sStepName_Self_NCPNCNSweep = "Self NCPNCNSweep";

        private const string m_sStepParameterName_FrequencyRank_Phase1 = "FrequencyRank_Phase1";
        private const string m_sStepParameterName_FrequencyRank_Phase2 = "FrequencyRank_Phase2";
        private const string m_sStepParameterName_AC_FrequencyRank = "AC_FrequencyRank";
        private const string m_sStepParameterName_Raw_ADC_Sweep = "Raw_ADC_Sweep";
        private const string m_sStepParameterName_Self_FrequencySweep = "Self_FrequencySweep";
        private const string m_sStepParameterName_Self_NCPNCNSweep = "Self_NCPNCNSweep";

        public static string[] m_sStepNameSet_Array = 
        {
            m_sStepName_FrequencyRank_Phase1,
            m_sStepName_FrequencyRank_Phase2,
            m_sStepName_AC_FrequencyRank,
#if !_USE_9F07_SOCKET
            m_sStepName_Raw_ADC_Sweep,
#endif
            //m_sStepName_Self_FrequencySweep,
            //m_sStepName_Self_NCPNCNSweep
        };

        public static string[] m_sStepParameterNameSet_Array = 
        {
            m_sStepParameterName_FrequencyRank_Phase1,
            m_sStepParameterName_FrequencyRank_Phase2,
            m_sStepParameterName_AC_FrequencyRank,
#if !_USE_9F07_SOCKET
            m_sStepParameterName_Raw_ADC_Sweep,
#endif
            //m_sStepParameterName_Self_FrequencySweep,
            //m_sStepParameterName_Self_NCPNCNSweep
        };

        public static MainStep[] m_eStepSet_Array = 
        {
            MainStep.FrequencyRank_Phase1,
            MainStep.FrequencyRank_Phase2,
            MainStep.AC_FrequencyRank,
#if !_USE_9F07_SOCKET
            MainStep.Raw_ADC_Sweep,
#endif
            //MainStep.Self_FrequencySweep,
            //MainStep.Self_NCPNCNSweep
        };

        protected static void JudgeFileType(string m_sFileName)
        {
            string[] sToken_Array = m_sFileName.Split('.');

            if (sToken_Array == null)
            {
                m_nFileType = nFILE_TYPE_UNKNOW;
                return;
            }

            if (sToken_Array[sToken_Array.Length - 1].Equals("ini") == true)
                m_nFileType = nFILE_TYPE_INI;
            else
                m_nFileType = nFILE_TYPE_XML;
        }

        /// <summary>
        /// 從參數設定檔讀取參數
        /// </summary>
        /// <param name="Section">Group Name</param>
        /// <param name="Key">Parameter Name</param>
        /// <param name="Default">預設值</param>
        /// <returns>回傳讀取的結果</returns>
        protected static string ReadValue(string Section, string Key, string Default = "")
        {
            if (m_nFileType == nFILE_TYPE_INI)
                return IniReadValue(Section, Key, Default);
            else if (m_nFileType == nFILE_TYPE_XML)
                return XmlReadValue(Section, Key, Default);
            else
                return Default;
        }

        /// <summary>
        /// 將Parameter數值寫入參數設定檔
        /// </summary>
        /// <param name="Section">Group Name</param>
        /// <param name="Key">Parameter Name</param>
        /// <param name="Value">參數數值</param>
        protected static void WriteValue(string Section, string Key, string Value, bool bAlwaysWrite = true, bool bHex = false)
        {
            if (bHex == true)
                Value = string.Format("0x{0}", Value);

            if (m_nFileType == nFILE_TYPE_INI)
                IniWriteValue(Section, Key, Value, bAlwaysWrite);
        }

        protected static void WriteValue(string Section, string Key, bool Value, bool bAlwaysWrite = true)
        {
            if (m_nFileType == nFILE_TYPE_INI)
            {
                if (Value == true)
                    IniWriteValue(Section, Key, "1", bAlwaysWrite);
                else
                    IniWriteValue(Section, Key, "0", bAlwaysWrite);
            }
        }

        private static void IniWriteValue(string Section, string Key, string Value, bool bAlwaysWrite = true, bool bSpace = true)
        {
            if (bAlwaysWrite == false)
            {
                StringBuilder sb = new StringBuilder(255);
                int nValue = Win32.GetPrivateProfileString(Section, Key, "DataNotExist![N/A]", sb, 255, m_sSettingFilePath);

                if (sb != null)
                {
                    if (sb.ToString() == "DataNotExist![N/A]")
                        return;
                }
                else
                    return;
            }

            if (bSpace == true)
                Value = string.Format(" {0}", Value);

            Win32.WritePrivateProfileString(Section, Key, Value, m_sSettingFilePath);
        }

        private static string IniReadValue(string Section, string Key, string Default = "")
        {
            StringBuilder sb = new StringBuilder(255);
            int nValue = Win32.GetPrivateProfileString(Section, Key, Default, sb, 255, m_sSettingFilePath);

            if (sb != null)
                return sb.ToString();

            return Default;
        }

        // add by J2 2013/04/02 可指定特定的ini檔案讀取資料
        protected static string IniReadValue(string Section, string Key, string sPath, string Default)
        {
            StringBuilder sb = new StringBuilder(255);

            int nValue = Win32.GetPrivateProfileString(Section, Key, Default, sb, 255, sPath);

            if (sb != null)
                return sb.ToString();

            return Default;
        }

        private static string XmlReadValue(string Section, string Key, string Default = "")
        {
            string sOutputValue = "";

            clsElanXML cElanProp = clsElanXML.GetInstance(m_sSettingFilePath, true);

            if (cElanProp == null)
            {
                sOutputValue = Default;

                if (File.Exists(m_sDefaultFilePath) == true)
                {
                    StringBuilder sb = new StringBuilder(255);
                    int nValue = Win32.GetPrivateProfileString(Section, Key, "DataNotExist!\\[N/A]", sb, 255, m_sDefaultFilePath);

                    if (sb != null)
                    {
                        if (sb.ToString() != "DataNotExist!\\[N/A]")
                            sOutputValue = sb.ToString();
                    }
                }

                return sOutputValue;
            }

            string sValue = cElanProp.GetValue(Section.Replace(' ', '_'), Key.Replace(' ', '_'));

            if (sValue == null)
                sOutputValue = Default;
            else
                sOutputValue = sValue;

            string sDefaultOverrideValue = ReadDefaultOverrideValue(Section, Key, m_sDefaultFilePath);

            if (sDefaultOverrideValue != null)
                sOutputValue = sDefaultOverrideValue;

            return sOutputValue;
        }

        /// <summary>
        /// 讀取 Default.ini 中的 override 值；若存在則依既有設計覆蓋 setting 檔結果。
        /// </summary>
        private static string ReadDefaultOverrideValue(string sSection, string sKey, string sDefaultFilePath)
        {
            if (File.Exists(sDefaultFilePath) == false)
                return null;

            StringBuilder sb = new StringBuilder(255);
            int nValue = Win32.GetPrivateProfileString(sSection, sKey, "DataNotExist!\\[N/A]", sb, 255, sDefaultFilePath);

            if (sb == null)
                return null;

            if (sb.ToString() == "DataNotExist!\\[N/A]")
                return null;

            return sb.ToString();
        }

        // 傳入Parameter Name與Index，傳回Parameter_Index
        protected static string GetParam(string sParameter, int nValue)
        {
            if (nValue == 0)
                return sParameter;
            else
                return string.Format("{0}_{1}", sParameter, (nValue + 1));
        }

        // 宣告取得Parameter Value
        protected static void GetParamValue(ref bool bValue, string sGroupName, string sParameterName, string sDefault)
        {
            string sIniValue = ReadValue(sGroupName, sParameterName, sDefault);

            try
            {
                bValue = Convert.ToInt32(sIniValue) >= 1;
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, e.Message.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatError = true;

                if (sDefault == "1")
                    bValue = true;
                else
                    bValue = false;
            }
        }

        protected static void GetParamValue(ref int nValue, string sGroupName, string sParameterName, string sDefault, bool bHex = false)
        {
            string sIniValue = ReadValue(sGroupName, sParameterName, sDefault);

            try
            {
                if (bHex == true)
                {
                    if (sIniValue == "-1" || sIniValue == "")
                        nValue = -1;
                    else
                    {
                        sIniValue = sIniValue.Replace("0x", "");
                        sIniValue = sIniValue.Replace("0X", "");
                        nValue = Int32.Parse(sIniValue, System.Globalization.NumberStyles.HexNumber);
                    }
                }
                else
                    nValue = Convert.ToInt32(sIniValue);
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, e.Message.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatError = true;

                if (bHex == true)
                {
                    sDefault = sDefault.Replace("0x", "");
                    sDefault = sDefault.Replace("0X", "");
                    nValue = Int32.Parse(sDefault, System.Globalization.NumberStyles.HexNumber);
                }
                else
                    nValue = Convert.ToInt32(sDefault);
            }
        }

        protected static void GetParamValue(ref short nValue, string sGroupName, string sParameterName, string sDefault, bool bHex = false)
        {
            string sIniValue = ReadValue(sGroupName, sParameterName, sDefault);

            try
            {
                if (bHex == true)
                    nValue = Int16.Parse(sIniValue, System.Globalization.NumberStyles.HexNumber);
                else
                    nValue = Convert.ToInt16(sIniValue);
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, e.Message.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatError = true;

                if (bHex == true)
                    nValue = Int16.Parse(sDefault, System.Globalization.NumberStyles.HexNumber);
                else
                    nValue = Convert.ToInt16(sDefault);
            }
        }

        protected static void GetParamValue(ref double dValue, string sGroupName, string sParameterName, string sDefault)
        {
            string sIniValue = ReadValue(sGroupName, sParameterName, sDefault);

            try
            {
                dValue = ElanConvert.Convert2Double(sIniValue, false);
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, e.Message.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatError = true;
                dValue = ElanConvert.Convert2Double(sDefault, false);
            }
        }

        protected static void GetParamValue(ref Color colorValue, string sGroupName, string sParameterName, string sDefault)
        {
            string sIniValue = ReadValue(sGroupName, sParameterName, sDefault);

            try
            {
                string[] sTokens_Array = sIniValue.Split(',');
                int nR = int.Parse(sTokens_Array[0]);
                int nG = int.Parse(sTokens_Array[1]);
                int nB = int.Parse(sTokens_Array[2]);
                colorValue = Color.FromArgb(nR, nG, nB);
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, e.Message.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatError = true;
                string[] sTokens_Array = sDefault.Split(',');
                int nR = int.Parse(sTokens_Array[0]);
                int nG = int.Parse(sTokens_Array[1]);
                int nB = int.Parse(sTokens_Array[2]);
                colorValue = Color.FromArgb(nR, nG, nB);
            }
        }

        protected static void GetParameterListValue(ref List<int> nParameterValue_List, string sGroupName, string sParameterName, string sDefault)
        {
            //Process the special Trace Number and Trace High boundary
            if (nParameterValue_List == null)
                nParameterValue_List = new List<int>();
            else
                nParameterValue_List.Clear();

            string[] sToken_Array = ReadValue(sGroupName, sParameterName, sDefault).Split(',');

            for (int nIndex = 0; nIndex < sToken_Array.Length; nIndex++)
            {
                int nTraceNumber = 0;

                if (int.TryParse(sToken_Array[nIndex], out nTraceNumber) == false)
                    nTraceNumber = -1;

                nParameterValue_List.Add(nTraceNumber);
            }
        }

        protected static void GetParameterListValue(ref List<float> nParameterValue_List, string sGroupName, string sParameterName, string sDefault)
        {
            //Process the special Trace Number and Trace High boundary
            if (nParameterValue_List == null)
                nParameterValue_List = new List<float>();
            else
                nParameterValue_List.Clear();

            string[] sToken_Array = ReadValue(sGroupName, sParameterName, sDefault).Split(',');

            for (int nIndex = 0; nIndex < sToken_Array.Length; nIndex++)
            {
                float fValue = 0.0f;

                fValue = Covert2Float((string)sToken_Array[nIndex]);

                nParameterValue_List.Add(fValue);
            }
        }

        protected static void GetParamValueByMathCalculate(ref UInt16 nValue, string sFilePath, string sGroupName, string sParamName, string sDefault)
        {
            bool bErrorFlag = false;
            string sErrorMessage = "";
            string sIniValue = IniReadValue(sGroupName, sParamName, sFilePath, sDefault);

            string sRPNExperssion = StringConvert.ConvertStringToRPN(ref bErrorFlag, ref sErrorMessage, StringConvert.InsertBlank(sIniValue));

            if (bErrorFlag == false)
            {
                nValue = StringConvert.GetCalculateResult(sRPNExperssion);
            }
            else
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParamName, sIniValue, sErrorMessage.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatError = true;

                sRPNExperssion = StringConvert.ConvertStringToRPN(ref bErrorFlag, ref sErrorMessage, StringConvert.InsertBlank(sDefault));
                nValue = StringConvert.GetCalculateResult(sRPNExperssion);
            }
        }

        private static float Covert2Float(string sValue)
        {
            StringBuilder sb = new StringBuilder(255);
            float fReturnValue = 0.0f;
            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            string sReplacedIniValue = sValue.Replace(".", sb.ToString());
            float.TryParse(sReplacedIniValue, out fReturnValue);
            return fReturnValue;
        }
    }

    public class ParamFingerAutoTuning : ParamBase
    {
        public static StepSettingParameter[] m_StepSettingParameter_Array = new StepSettingParameter[nSTEP_NUMBER];

        public class StepSettingParameter
        {
            public string m_sStepName = "";
            public string m_sStepParamName = "";
            public MainStep m_Step = MainStep.Else;
            public bool m_bEnable = false;
        }

        public static string m_sProjectName = "";

        public static Int32 m_nUSBVID = 0x04F3;
        public static Int32 m_nUSBPID = 0x0000;
        public static Int32 m_nDVDD = 33;
        public static Int32 m_nVIO = 33;
        public static Int32 m_nI2CAddress = 0x20;
        public static Int32 m_nNormalLength = 0x3F;
        public static string m_sSocketType = MainConstantParameter.m_sSOCKET_WINDOWS;
        public static string m_sInterfaceType = MainConstantParameter.m_sINTERFACE_HIDOVERI2C;
        public static int m_nInterfaceType = (int)UserInterfaceDefine.InterfaceType.IF_HID_OVER_I2C_TP;
        public static int m_nSPICommandLength = 10;
        public static int m_nPort = 9344;
        public static int m_nGetDataTimeout = 1000;
        public static int m_nGetParameterTimeout = 1000;
        public static int m_nReadBulkRAMDataValueExtraDelayTime = 1000;
        public static int m_nGetDataRetryCount = 6;
        public static int m_nNormalDelayTime = 0;
        public static int m_nDeviceConnectTestCount = 10;
        public static int m_nDisableSetSPICommandLength = 0;
        public static int m_nRunMultiTest = 0;

#if _USE_9F07_SOCKET
        public static int m_nUseICSolutionType_9F07 = 1;
        public static int m_nNormalDelayTime_9F07 = 100;
        public static int m_nExitTestModeDelayTime_9F07 = 1000;
#endif

        public static string m_sAndroidSocketType = MainConstantParameter.m_sANDROIDSOCKET_CLIENT;
        public static int m_nAfUpdateSHFileDelayTime = 1000;
        public static int m_nKeepWakeUpIntervalTime = 10000;
        public static int m_nAndroidDisableGetReport = 0;

        public static int m_nChromeGetDataRetryCount = 11;
        public static int m_nChromeGetDataTimeout = 10000;
        public static int m_nChromeGetDataDelayTime = 1000;

        public static string m_sChromeSSHSocketServerRemoteClientIPAddress = "192.168.0.1";
        public static string m_sChromeSSHSocketServerUserName = "root";
        public static string m_sChromeSSHSocketServerPassword = "test0000";
        public static int m_nChromeSSHSocketServerDebugMode = 0;

        public static string m_sSSHSocketServerRemoteClientIPAddress = "10.11.99.1";
        public static string m_sSSHSocketServerUserName = "root";
        public static string m_sSSHSocketServerPassword = "wvwhBnPynK";
        public static int m_nSSHSocketServerDebugMode = 0;
        public static int m_nSSHSocketServerNormalDelayTime = 500;
        public static int m_nSSHSocketServerGetDataTimeout = 5000;
        public static int m_nSSHSocketServerGetParameterTimeout = 2000;

        public static int m_nScreenSizeWidth = 0;
        public static int m_nScreenSizeHeight = 0;
        public static int m_nPatternType = 0;
        public static Color m_clrCustomizeScreenColor = Color.White;
        public static int m_nBackLightValue = 0;
        public static string m_sPatternPicPath = "";
        public static Color m_clrPHCKGrayLineColor = Color.FromArgb(144, 144, 144);

        //[Gen8 Command Setting]
        public static int m_nCommandScriptType = 0;
        public static string m_sUserDefinedFilePath = "";
        public static int m_nEnableCheckUserDefinedFormat = 0;

        //[Gen8 Setting]
        //public static int m_nGen8SPICommandLength = 10;
        public static int m_nGen8SendCommandDelayTime = 100;
        public static int m_nGen8GetACKTimeout = 1000;
        public static int m_nGen8GetFirstBaseDelay = 0;
        public static int m_nGen8GetFirstBaseDelayTime = 5000;
        public static int m_nGen8GetDataDelayTime = 0;
        public static int m_nGen8JustSetSelfNCPNCN = 0;
        public static int m_nGen8GetBaseType = 0;
        public static int m_nGen8SetUpdateBaseType = 5;
        public static int m_nGen8UseADCForBaseFrame = 10;
        public static int m_nGen8UpdateBaseDelayTime = 1000;
        public static int m_nGen8SetGetDataInfoType = 0;
        public static int m_nGen8ReadBulkRAMDataValue = 2;
        public static int m_nGen8SetTransferTestModeViaHID = 0;
        public static int m_nGen8GetBaseUseSkipFrame = 0;
        public static int m_nGen8ReconnectDelayTime = 3000;

        public static int m_nGen8DisablePenFunction = 0;
        public static int m_nGen8SetPenFunctionDelayTime = 2000;
        public static int m_nGen8GetAnalogAfterSwitchTestMode = 1;
        public static int m_nGen8SetDCDCEnable = -1;

        public static int m_nDisableSetFWOption = 0;
        public static int m_nDisableSetAnalogParameter = 0;
        public static int m_nDisableIdleMode = 0;
        public static int m_nDisableTxn_Scan = 0;
        public static int m_nDisableUpdateBase = 0;
        public static int m_nDisablePen_Scan = 0;
        public static int m_nDisableAddADCOffset = 0;
        public static int m_nDisableReK = 0;
        public static int m_nReKTimeout = 8000;
        public static int m_nFingerReportTest = 0;
        public static int m_nSaveNormalRawDataType = 0;
        public static int m_nGenerateH5FileData = 0;
        public static int m_nKeepDoNotReset = 0;
        public static int m_nDisableReset = 0;
        public static int m_nSkipFrame = 2;
        public static int m_nEnableUseNewMethod = 1;
        public static int m_nIgnoreAnomalyUniformity = 1;

        public static int m_nSkipFRPH1Step = 0;
        public static int m_nManualFixedPH1 = 0x10;
        public static int m_nManualMinimumPH2 = 0x10;
        public static int m_nManualEnableTXn = 0;

        public static int m_nEnableContinueFlow = 0;
        public static int m_nDisableDisconnect = 0;

        public static int m_nIdealScanTime = 120;
        public static int m_nEnableSetSum = 0;

        public static int m_nFRPH1FixedPH1 = 0x10;
        public static double m_dFRPH1ScanFrequencyLB = 80.0;
        public static double m_dFRPH1ScanFrequencyHB = 250.0;
        public static int m_nFRPH1FrequencySortType = 1;
        public static int m_nFRPH1TestFrame = 50;
        public static double m_dFRPH1UniformityLB = -1.0;
        public static double m_dFRPH1LimitHB = -1.0;
        public static double m_dFRPH1FitFrequencyLB = -1.0;

        public static int m_nFRPH2DataType = 0;
        public static double m_dFRPH2TestFreqLB = 100.0;
        public static double m_dFRPH2TestFreqHB = 250.0;
        public static int m_nFRPH2ADCTestFrame = 300;
        public static int m_nFRPH2DVTestFrame = 300;
        public static int m_nFRPH2ComputeRXTraceReference = 0;
        public static int m_nFRPH2PenEffectRXTrace = 0;
        public static int m_nFRPH2MinPH1 = 0x10;
        public static int m_nFRPH2NormalizeType = 1;
        public static int m_nFRPH2ChartReferenceValueHB = 0;
        public static int m_nFRPH2ChartReferenceValueLB = 0;
        public static int m_nFRPH2ChartReferenceValueInterval = 0;
        public static int m_nFRPH2RecordRecentADCMeanForBase = 0;
        public static int m_nFRPH2RecentADCMeanForBaseFrameNumber = 10;

        public static int m_nFRPH2ACFRBestRankNumber = 10;
        public static int m_nFRPH2ACFRGetBaseUseSkipFrame = 0;
        public static int m_nFRPH2ACFRGetBaseType = 0;
        public static int m_nFRPH2ACFRUseADCForBaseFrame = 10;

        public static int m_nACFRModeType = 1;
        public static int m_nACFRSNRMethodType = 1;
        public static int m_nACFRSaveAnalysisData = 1;
        public static int m_nACFRADCTestFrame = 300;
        public static int m_nACFRMinPH1 = 0x10;
        public static int m_nACFRMultiFrameCount_Hor = 10;
        //public static double m_dACFRSTDTHRatio_Hor = 0.7;
        //public static double m_dACFRSTDTHRatio_Ver = 1.0;
        public static double m_dACFRSignalAreaPtg_Hor = 70.0;
        public static double m_dACFRSignalAreaPtg_Ver = 70.0;
        public static double m_dACFRNonSignalAreaPtg = 30.0;
        public static int m_nACFRGetSignalAreaMethod = 1;
        public static int m_nACFRUseBaseDifferEdgeDetectMethod = 0;
        public static int m_nACFRUseTotalDataGetSignalTrace = 0;
        public static int m_nACFRFilterMaskType = 8;
        public static double m_dACFRThresholdStdRatio = 1.0;
        public static int m_nACFRSignalRXStartTrace = 0;
        public static int m_nACFRSignalRXEndTrace = 0;
        public static int m_nACFRSignalTXStartTrace = 0;
        public static int m_nACFRSignalTXEndTrace = 0;
        public static int m_nACFRChartSNRValueHB = 0;
        public static int m_nACFRChartSNRValueLB = 0;
        public static int m_nACFRChartSNRValueInterval = 0;

        public static int m_nRawADCSTPReg_Offset_7318 = 0x9000;
        public static int m_nRawADCSTPReg_Offset_7315 = 0x8000;
        public static int m_nRawADCSTPReg_Offset_6315 = 0x8000;
        public static int m_nRawADCSGen8FixedFIRCOEF_SEL = 0;
        public static int m_nRawADCSGen6or7FixedFIRTB = 16384;
        public static int m_nRawADCSFixedFIR_TAP_NUM = 1;
        //public static int m_nRawADCSFixedSELGM = -1;
        public static int m_nRawADCSSELCLB = 2;
        public static int m_nRawADCSSELCHB = 3;
        public static int m_nRawADCSVSELLB_Gen8 = 0;
        public static int m_nRawADCSVSELHB_Gen8 = 0;
        public static int m_nRawADCSVSELLB_7318 = 2;
        public static int m_nRawADCSVSELHB_7318 = 3;
        public static int m_nRawADCSVSELLB_7315 = 3;
        public static int m_nRawADCSVSELHB_7315 = 3;
        public static int m_nRawADCSVSELLB_6315 = 3;
        public static int m_nRawADCSVSELHB_6315 = 3;
        public static int m_nRawADCSLGLB = 0;
        public static int m_nRawADCSLGHB = 2;
        public static int m_nRawADCSSELGMLB = 0;
        public static int m_nRawADCSSELGMHB = 1;
        public static int m_nRawADCSFitADCLB = 0;
        public static int m_nRawADCSFitADCHB = 10000;
        public static int m_nRawADCSADCTestFrame = 10;
        public static int m_nRawADCSGen7EnableHWTXN = 0;
        public static int m_nRawADCSGen6or7EnableFWTX4 = 0;

        public static int m_nSelfTraceType = 0;

        public static int m_nSelfFSGetDataType = 0;
        public static int m_nSelfFSGetReportType = 0;
        public static int m_nSelfFSGetReportSequence = 0;
        public static int m_nSelfFSGetReportByte = 0;
        public static int m_nSelfFSGetSignalReport = 0;
        public static int m_nSelfFSDisplayWarning = 1;
        public static int m_nSelfFSRepeatCount = 0;
        public static int m_nSelfFSTestFrame = 100;
        public static int m_nSelfFSSum = -1;
        public static int m_nSelfFSGain = -1;
        public static int m_nSelfFSCAG = -1;
        public static int m_nSelfFSIQ_BSH = -1;
        public static int m_nSelfFSGetKValue = 0;
        public static int m_nSelfFSRunKSequence = 0;
        public static string m_sSelfFS_MS_MM_RX0FileName = "";
        public static int m_nSelfFSNCPValueLB = 0;
        public static int m_nSelfFSNCPValueHB = 31;
        public static int m_nSelfFSNCNValueLB = 0;
        public static int m_nSelfFSNCNValueHB = 31;
        public static int m_nSelfFSGetReportTimeout = 0;
        public static int m_nSelfFSReportNumber = 1000;
        public static int m_nSelfFSReportSignedValue = 0;
        public static int m_nSelfFSRunCALData = 0;
        public static int m_nSelfFSCALValue = 15;
        public static int m_nSelfFSCALStartTrace = 0;
        public static int m_nSelfFSCALStartTraceNumber = 0;
        public static int m_nSelfFSCALEndTraceNumber = 0;

        public static int m_nSelfPNSTestFrame = 10;
        public static int m_nSelfPNSGetKValue = 0;
        public static int m_nSelfPNSNCPNCNValueLB = 0;
        public static int m_nSelfPNSNCPNCNValueHB = 31;
        public static int m_nSelfPNS_MS_MM_RX0Count = 42;
        public static int m_nSelfPNSStartTrace = 1;

        public static int m_nSelfFPStandardPH1PH2Sum = 111;
        public static int m_nSelfFPStandardSum = 128;
        public static int m_nSelfFPPH2_LMT_LB = 160;

        public static StackType[] m_StackType = new StackType[3];

        public static UInt16 m_nRelativeAddress_MS_PH1 = 0x009E;
        public static UInt16 m_nRelativeAddress_MS_PH2 = 0x009F;
        public static UInt16 m_nRelativeAddress_MS_PH3 = 0x00A0;
        public static UInt16 m_nRelativeAddress_MS_AFE_DFT_NUM = 0x009C;
        public static UInt16 m_nRelativeAddress_MS_AFE_SP_NUM = 0x009A;
        public static UInt16 m_nRelativeAddress_MS_AFE_EFFECT_NUM = 0x009B;
        public static UInt16 m_nRelativeAddress_PKT_WC = 0x0004;

        public static UInt16 m_nRelativeAddress_MS_BIN_FIRCOEF_SEL_TAP_NUM = 0x0024;
        public static UInt16 m_nRelativeAddress_MS_IQ_BSH_GP0_GP1 = 0x0010;
        public static UInt16 m_nRelativeAddress_MS_ANA_TP_CTL_01 = 0x00DB;
        public static UInt16 m_nRelativeAddress_MS_ANA_TP_CTL_01_2 = 0x00E2;
        public static UInt16 m_nRelativeAddress_MS_ANA_CTL_04 = 0x0127;
        public static UInt16 m_nRelativeAddress_MS_ANA_CTL_04_2 = 0x012C;
        public static UInt16 m_nRelativeAddress_MS_ANA_TP_CTL_06 = 0x00E0;
        public static UInt16 m_nRelativeAddress_MS_ANA_TP_CTL_06_2 = 0x00E7;
        public static UInt16 m_nRelativeAddress_MS_ANA_TP_CTL_07 = 0x00E8;

        public class StackType
        {
            public string m_sTypeName = "";
            public double m_fLB = 0.0;
            public double m_fHB = 0.0;
        }

        public static void LoadParam(string sSettingPath, string sDefaultPath)
        {
            m_sSettingFilePath = sSettingPath;
            m_sDefaultFilePath = sDefaultPath;

            JudgeFileType(sSettingPath);

            LoadStepSettingParam(ref m_StepSettingParameter_Array);
            LoadStackType(ref m_StackType);

            m_sProjectName = ReadValue("AutoTuning SET", "ProjectName", "");

            GetParamValue(ref m_nUSBVID, "AutoTuning SET", "USB_VID", "04F3", true);
            GetParamValue(ref m_nUSBPID, "AutoTuning SET", "USB_PID", "0000", true);
            string sDVDD = ReadValue("AutoTuning SET", "DVDD", "3.3V");
            SetDVDD(sDVDD);
            string sVIO = ReadValue("AutoTuning SET", "VIO", "3.3V");
            SetVIO(sVIO);
            GetParamValue(ref m_nI2CAddress, "AutoTuning SET", "I2CAddress", "20", true);
            GetParamValue(ref m_nNormalLength, "AutoTuning SET", "NormalLength", "3F", true);
            m_sSocketType = ReadValue("AutoTuning SET", "SocketType", MainConstantParameter.m_sSOCKET_WINDOWS);
            m_sInterfaceType = ReadValue("AutoTuning SET", "InterfaceType", MainConstantParameter.m_sINTERFACE_HIDOVERI2C);
            m_nInterfaceType = SetInterface(m_sInterfaceType);
            GetParamValue(ref m_nSPICommandLength, "AutoTuning SET", "SPICommandLength", "10");
            GetParamValue(ref m_nPort, "AutoTuning SET", "Port", "9344");
            GetParamValue(ref m_nGetDataTimeout, "AutoTuning SET", "GetDataTimeout", "1000");
            GetParamValue(ref m_nGetParameterTimeout, "AutoTuning SET", "GetParameterTimeout", "1000");
            GetParamValue(ref m_nReadBulkRAMDataValueExtraDelayTime, "AutoTuning SET", "ReadBulkRAMDataValueExtraDelayTime", "1000");
            GetParamValue(ref m_nGetDataRetryCount, "AutoTuning SET", "GetDataRetryCount", "6");
            GetParamValue(ref m_nNormalDelayTime, "AutoTuning SET", "NormalDelayTime", "0");
            GetParamValue(ref m_nDeviceConnectTestCount, "AutoTuning SET", "DeviceConnectTestCount", "10");
            GetParamValue(ref m_nDisableSetSPICommandLength, "AutoTuning SET", "DisableSetSPICommandLength", "0");
            GetParamValue(ref m_nRunMultiTest, "AutoTuning SET", "RunMultiTest", "0");

#if _USE_9F07_SOCKET
            GetParamValue(ref m_nUseICSolutionType_9F07, "AutoTuning SET", "UseICSolutionType_9F07", "1");
            GetParamValue(ref m_nNormalDelayTime_9F07, "AutoTuning SET", "NormalDelayTime_9F07", "100");
            GetParamValue(ref m_nExitTestModeDelayTime_9F07, "AutoTuning SET", "ExitTestModeDelayTime_9F07", "1000");
#endif

            m_sAndroidSocketType = ReadValue("AutoTuning SET", "AndroidSocketType", MainConstantParameter.m_sANDROIDSOCKET_CLIENT);
            GetParamValue(ref m_nAfUpdateSHFileDelayTime, "AutoTuning SET", "AfUpdateSHFileDelayTime", "1000");
            GetParamValue(ref m_nKeepWakeUpIntervalTime, "AutoTuning SET", "KeepWakeUpIntervalTime", "10000");
            GetParamValue(ref m_nAndroidDisableGetReport, "AutoTuning SET", "AndroidDisableGetReport", "0");

            GetParamValue(ref m_nChromeGetDataRetryCount, "AutoTuning SET", "ChromeGetDataRetryCount", "11");
            GetParamValue(ref m_nChromeGetDataTimeout, "AutoTuning SET", "ChromeGetDataTimeout", "10000");
            GetParamValue(ref m_nChromeGetDataDelayTime, "AutoTuning SET", "ChromeGetDataDelayTime", "1000");

            m_sChromeSSHSocketServerRemoteClientIPAddress = ReadValue("AutoTuning SET", "ChromeSSHSocketServerRemoteClientIPAddress", "192.168.0.1");
            m_sChromeSSHSocketServerUserName = ReadValue("AutoTuning SET", "ChromeSSHSocketServerUserName", "root");
            m_sChromeSSHSocketServerPassword = ReadValue("AutoTuning SET", "ChromeSSHSocketServerPassword", "test0000");
            GetParamValue(ref m_nChromeSSHSocketServerDebugMode, "AutoTuning SET", "ChromeSSHSocketServerDebugMode", "0");

            m_sSSHSocketServerRemoteClientIPAddress = ReadValue("AutoTuning SET", "SSHSocketServerRemoteClientIPAddress", "10.11.99.1");
            m_sSSHSocketServerUserName = ReadValue("AutoTuning SET", "SSHSocketServerUserName", "root");
            m_sSSHSocketServerPassword = ReadValue("AutoTuning SET", "SSHSocketServerPassword", "wvwhBnPynK");
            GetParamValue(ref m_nSSHSocketServerDebugMode, "AutoTuning SET", "SSHSocketServerDebugMode", "0");
            GetParamValue(ref m_nSSHSocketServerNormalDelayTime, "AutoTuning SET", "SSHSocketServerNormalDelayTime", "500");
            GetParamValue(ref m_nSSHSocketServerGetDataTimeout, "AutoTuning SET", "SSHSocketServerGetDataTimeout", "5000");
            GetParamValue(ref m_nSSHSocketServerGetParameterTimeout, "AutoTuning SET", "SSHSocketServerGetParameterTimeout", "2000");

            GetParamValue(ref m_nScreenSizeWidth, "AutoTuning SET", "ScreenSizeWidth", "294");
            GetParamValue(ref m_nScreenSizeHeight, "AutoTuning SET", "ScreenSizeHeight", "166");
            GetParamValue(ref m_nPatternType, "AutoTuning SET", "PatternType", "1");
            GetParamValue(ref m_clrCustomizeScreenColor, "AutoTuning SET", "CustomizeScreenColor", "255,255,255");
            GetParamValue(ref m_nBackLightValue, "AutoTuning SET", "BackLightValue", "-1");
            m_sPatternPicPath = ReadValue("AutoTuning SET", "PatternPicPath", "");
            GetParamValue(ref m_clrPHCKGrayLineColor, "AutoTuning SET", "PHCKGrayLineColor", "144,144,144");

            GetParamValue(ref m_nCommandScriptType, "AutoTuning SET", "CommandScriptType", "0");
            m_sUserDefinedFilePath = ReadValue("AutoTuning SET", "UserDefinedFilePath", "");
            GetParamValue(ref m_nEnableCheckUserDefinedFormat, "AutoTuning SET", "EnableCheckUserDefinedFormat", "0");

            //GetParamValue(ref m_nGen8SPICommandLength, "AutoTuning SET", "Gen8SPICommandLength", "10");
            GetParamValue(ref m_nGen8SendCommandDelayTime, "AutoTuning SET", "Gen8SendCommandDelayTime", "100");
            GetParamValue(ref m_nGen8GetACKTimeout, "AutoTuning SET", "Gen8GetACKTimeout", "1000");
            GetParamValue(ref m_nGen8GetFirstBaseDelay, "AutoTuning SET", "Gen8GetFirstBaseDelay", "0");
            GetParamValue(ref m_nGen8GetFirstBaseDelayTime, "AutoTuning SET", "Gen8GetFirstBaseDelayTime", "5000");
            GetParamValue(ref m_nGen8GetDataDelayTime, "AutoTuning SET", "Gen8GetDataDelayTime", "0");
            GetParamValue(ref m_nGen8JustSetSelfNCPNCN, "AutoTuning SET", "Gen8JustSetSelfNCPNCN", "0");
            GetParamValue(ref m_nGen8GetBaseType, "AutoTuning SET", "Gen8GetBaseType", "0");
            GetParamValue(ref m_nGen8SetUpdateBaseType, "AutoTuning SET", "Gen8SetUpdateBaseType", "5");
            GetParamValue(ref m_nGen8UseADCForBaseFrame, "AutoTuning SET", "Gen8UseADCForBaseFrame", "10");
            GetParamValue(ref m_nGen8UpdateBaseDelayTime, "AutoTuning SET", "Gen8UpdateBaseDelayTime", "1000");
            GetParamValue(ref m_nGen8SetGetDataInfoType, "AutoTuning SET", "Gen8SetGetDataInfoType", "0");
            GetParamValue(ref m_nGen8ReadBulkRAMDataValue, "AutoTuning SET", "Gen8ReadBulkRAMDataValue", "1");
            GetParamValue(ref m_nGen8SetTransferTestModeViaHID, "AutoTuning SET", "Gen8SetTransferTestModeViaHID", "1");
            GetParamValue(ref m_nGen8GetBaseUseSkipFrame, "AutoTuning SET", "Gen8GetBaseUseSkipFrame", "0");
            GetParamValue(ref m_nGen8ReconnectDelayTime, "AutoTuning SET", "Gen8ReconnectDelayTime", "3000");
            GetParamValue(ref m_nGen8DisablePenFunction, "AutoTuning SET", "Gen8DisablePenFunction", "0");
            GetParamValue(ref m_nGen8SetPenFunctionDelayTime, "AutoTuning SET", "Gen8SetPenFunctionDelayTime", "2000");
            GetParamValue(ref m_nGen8GetAnalogAfterSwitchTestMode, "AutoTuning SET", "Gen8GetAnalogAfterSwitchTestMode", "1");
            GetParamValue(ref m_nGen8SetDCDCEnable, "AutoTuning SET", "Gen8SetDCDCEnable", "-1");

            GetParamValue(ref m_nDisableSetFWOption, "AutoTuning SET", "DisableSetFWOption", "0");
            GetParamValue(ref m_nDisableSetAnalogParameter, "AutoTuning SET", "DisableSetAnalogParameter", "0");
            GetParamValue(ref m_nDisableIdleMode, "AutoTuning SET", "DisableIdleMode", "0");
            GetParamValue(ref m_nDisableTxn_Scan, "AutoTuning SET", "DisableTxn_Scan", "0");
            GetParamValue(ref m_nDisableUpdateBase, "AutoTuning SET", "DisableUpdateBase", "0");
            GetParamValue(ref m_nDisablePen_Scan, "AutoTuning SET", "DisablePen_Scan", "0");
            GetParamValue(ref m_nDisableAddADCOffset, "AutoTuning SET", "DisableAddADCOffset", "0");
            GetParamValue(ref m_nDisableReK, "AutoTuning SET", "DisableReK", "0");
            GetParamValue(ref m_nReKTimeout, "AutoTuning SET", "ReKTimeout", "8000");
            GetParamValue(ref m_nFingerReportTest, "AutoTuning SET", "FingerReportTest", "0");
            GetParamValue(ref m_nSaveNormalRawDataType, "AutoTuning SET", "SaveNormalRawDataType", "0");
            GetParamValue(ref m_nGenerateH5FileData, "AutoTuning SET", "GenerateH5FileData", "0");
            GetParamValue(ref m_nKeepDoNotReset, "AutoTuning SET", "KeepDoNotReset", "0");
            GetParamValue(ref m_nDisableReset, "AutoTuning SET", "DisableReset", "0");
            GetParamValue(ref m_nSkipFrame, "AutoTuning SET", "SkipFrame", "2");
            GetParamValue(ref m_nEnableUseNewMethod, "AutoTuning SET", "EnableUseNewMethod", "1");
            GetParamValue(ref m_nIgnoreAnomalyUniformity, "AutoTuning SET", "IgnoreAnomalyUniformity", "1");

            GetParamValue(ref m_nEnableContinueFlow, "AutoTuning SET", "EnableContinueFlow", "0");
            GetParamValue(ref m_nDisableDisconnect, "AutoTuning SET", "DisableDisconnect", "0");

            GetParamValue(ref m_nIdealScanTime, "AutoTuning SET", "IdealScanTime", "120");
            GetParamValue(ref m_nEnableSetSum, "AutoTuning SET", "EnableSetSum", "0");

            GetParamValue(ref m_nSkipFRPH1Step, "AutoTuning SET", "SkipFRPH1Step", "0");
            GetParamValue(ref m_nManualFixedPH1, "AutoTuning SET", "ManualFixedPH1", "10", true);
            GetParamValue(ref m_nManualMinimumPH2, "AutoTuning SET", "ManualMinimumPH2", "10", true);
            GetParamValue(ref m_nManualEnableTXn, "AutoTuning SET", "ManualEnableTXn", "0");

            GetParamValue(ref m_nFRPH1FixedPH1, "AutoTuning SET", "FRPH1FixedPH1", "10", true);
            GetParamValue(ref m_dFRPH1ScanFrequencyLB, "AutoTuning SET", "FRPH1ScanFreqLB", "80.0");
            GetParamValue(ref m_dFRPH1ScanFrequencyHB, "AutoTuning SET", "FRPH1ScanFreqHB", "250.0");
            GetParamValue(ref m_nFRPH1FrequencySortType, "AutoTuning SET", "FRPH1FrequencySortType", "0");
            GetParamValue(ref m_nFRPH1TestFrame, "AutoTuning SET", "FRPH1TestFrame", "50");
            GetParamValue(ref m_dFRPH1UniformityLB, "AutoTuning SET", "FRPH1UniformityLB", "-1.0");
            GetParamValue(ref m_dFRPH1LimitHB, "AutoTuning SET", "FRPH1LimitHB", "-1.0");

            GetParamValue(ref m_nFRPH2NormalizeType, "AutoTuning SET", "FRPH2NormalizeType", "1");
            GetParamValue(ref m_nFRPH2DataType, "AutoTuning SET", "FRPH2DataType", "0");
            GetParamValue(ref m_dFRPH2TestFreqLB, "AutoTuning SET", "FRPH2TestFreqLB", "100.0");
            GetParamValue(ref m_dFRPH2TestFreqHB, "AutoTuning SET", "FRPH2TestFreqHB", "250.0");
            GetParamValue(ref m_nFRPH2ADCTestFrame, "AutoTuning SET", "FRPH2ADCTestFrame", "300");
            GetParamValue(ref m_nFRPH2DVTestFrame, "AutoTuning SET", "FRPH2DVTestFrame", "300");
            GetParamValue(ref m_nFRPH2ComputeRXTraceReference, "AutoTuning SET", "FRPH2ComputeRXTraceReference", "0");
            GetParamValue(ref m_nFRPH2PenEffectRXTrace, "AutoTuning SET", "FRPH2PenEffectRXTrace", "0");
            GetParamValue(ref m_nFRPH2ChartReferenceValueHB, "AutoTuning SET", "FRPH2ChartReferenceValueHB", "0");
            GetParamValue(ref m_nFRPH2ChartReferenceValueLB, "AutoTuning SET", "FRPH2ChartReferenceValueLB", "0");
            GetParamValue(ref m_nFRPH2ChartReferenceValueInterval, "AutoTuning SET", "FRPH2ChartReferenceValueInterval", "0");
            GetParamValue(ref m_nFRPH2RecordRecentADCMeanForBase, "AutoTuning SET", "FRPH2RecordRecentADCMeanForBase", "0");
            GetParamValue(ref m_nFRPH2RecentADCMeanForBaseFrameNumber, "AutoTuning SET", "FRPH2RecentADCMeanForBaseFrameNumber", "10");

            GetParamValue(ref m_nFRPH2MinPH1, "AutoTuning SET", "FRPH2MinPH1", "10", true);
            
            GetParamValue(ref m_nFRPH2ACFRBestRankNumber, "AutoTuning SET", "FRPH2ACFRBestRankNumber", "10");
            GetParamValue(ref m_nFRPH2ACFRGetBaseType, "AutoTuning SET", "FRPH2ACFRGetBaseType", "0");
            GetParamValue(ref m_nFRPH2ACFRGetBaseUseSkipFrame, "AutoTuning SET", "FRPH2ACFRGetBaseUseSkipFrame", "0");
            GetParamValue(ref m_nFRPH2ACFRUseADCForBaseFrame, "AutoTuning SET", "FRPH2ACFRUseADCForBaseFrame", "10");

            //GetParamValue(ref m_nACFRModeType, "AutoTuning SET", "ACFRModeType", "1");
            GetParamValue(ref m_nACFRSNRMethodType, "AutoTuning SET", "ACFRSNRMethodType", "1");
            GetParamValue(ref m_nACFRSaveAnalysisData, "AutoTuning SET", "ACFRSaveAnalysisData", "1");
            GetParamValue(ref m_nACFRADCTestFrame, "AutoTuning SET", "ACFRADCTestFrame", "300");
            GetParamValue(ref m_nACFRMinPH1, "AutoTuning SET", "ACFRMinPH1", "10", true);
            GetParamValue(ref m_nACFRGetSignalAreaMethod, "AutoTuning SET", "ACFRGetSignalAreaMethod", "1");
            GetParamValue(ref m_nACFRUseBaseDifferEdgeDetectMethod, "AutoTuning SET", "ACFRUseBaseDifferEdgeDetectMethod", "1");
            GetParamValue(ref m_nACFRUseTotalDataGetSignalTrace, "AutoTuning SET", "ACFRUseTotalDataGetSignalTrace", "0");
            GetParamValue(ref m_nACFRFilterMaskType, "AutoTuning SET", "ACFRFilterMaskType", "8");
            GetParamValue(ref m_dACFRThresholdStdRatio, "AutoTuning SET", "ACFRThresholdStdRatio", "1.0");
            GetParamValue(ref m_nACFRMultiFrameCount_Hor, "AutoTuning SET", "ACFRMultiFrameCount_Hor", "10");
            //GetParamValue(ref m_dACFRSTDTHRatio_Hor, "AutoTuning SET", "ACFRSTDTHRatio_Hor", "0.7");
            //GetParamValue(ref m_dACFRSTDTHRatio_Ver, "AutoTuning SET", "ACFRSTDTHRatio_Ver", "1.0");
            GetParamValue(ref m_dACFRSignalAreaPtg_Hor, "AutoTuning SET", "ACFRSignalAreaPtg_Hor", "70.0");
            GetParamValue(ref m_dACFRSignalAreaPtg_Ver, "AutoTuning SET", "ACFRSignalAreaPtg_Ver", "70.0");
            GetParamValue(ref m_dACFRNonSignalAreaPtg, "AutoTuning SET", "ACFRNonSignalAreaPtg", "30.0");
            GetParamValue(ref m_nACFRSignalRXStartTrace, "AutoTuning SET", "ACFRSignalRXStartTrace", "0");
            GetParamValue(ref m_nACFRSignalRXEndTrace, "AutoTuning SET", "ACFRSignalRXEndTrace", "0");
            GetParamValue(ref m_nACFRSignalTXStartTrace, "AutoTuning SET", "ACFRSignalTXStartTrace", "0");
            GetParamValue(ref m_nACFRSignalTXEndTrace, "AutoTuning SET", "ACFRSignalTXEndTrace", "0");
            GetParamValue(ref m_nACFRChartSNRValueHB, "AutoTuning SET", "ACFRChartSNRValueHB", "0");
            GetParamValue(ref m_nACFRChartSNRValueLB, "AutoTuning SET", "ACFRChartSNRValueLB", "0");
            GetParamValue(ref m_nACFRChartSNRValueInterval, "AutoTuning SET", "ACFRChartSNRValueInterval", "0");

            GetParamValue(ref m_nRawADCSTPReg_Offset_7318, "AutoTuning SET", "RawADCSTPReg_Offset_7318", "9000", true);
            GetParamValue(ref m_nRawADCSTPReg_Offset_7315, "AutoTuning SET", "RawADCSTPReg_Offset_7315", "8000", true);
            GetParamValue(ref m_nRawADCSTPReg_Offset_6315, "AutoTuning SET", "RawADCSTPReg_Offset_6315", "8000", true);
            GetParamValue(ref m_nRawADCSGen8FixedFIRCOEF_SEL, "AutoTuning SET", "RawADCSGen8FixedFIRCOEF_SEL", "0");
            GetParamValue(ref m_nRawADCSGen6or7FixedFIRTB, "AutoTuning SET", "RawADCSGen6or7FixedFIRTB", "16384");
            GetParamValue(ref m_nRawADCSFixedFIR_TAP_NUM, "AutoTuning SET", "RawADCSFixedFIR_TAP_NUM", "1");
            //GetParamValue(ref m_nRawADCSFixedSELGM, "AutoTuning SET", "RawADCSFixedSELGM", "-1");
            GetParamValue(ref m_nRawADCSSELCLB, "AutoTuning SET", "RawADCSSELCLB", "2");
            GetParamValue(ref m_nRawADCSSELCHB, "AutoTuning SET", "RawADCSSELCHB", "3");
            GetParamValue(ref m_nRawADCSVSELLB_Gen8, "AutoTuning SET", "RawADCSVSELLB_Gen8", "0");
            GetParamValue(ref m_nRawADCSVSELHB_Gen8, "AutoTuning SET", "RawADCSVSELHB_Gen8", "0");
            GetParamValue(ref m_nRawADCSVSELLB_7318, "AutoTuning SET", "RawADCSVSELLB_7318", "2");
            GetParamValue(ref m_nRawADCSVSELHB_7318, "AutoTuning SET", "RawADCSVSELHB_7318", "3");
            GetParamValue(ref m_nRawADCSVSELLB_7315, "AutoTuning SET", "RawADCSVSELLB_7315", "3");
            GetParamValue(ref m_nRawADCSVSELHB_7315, "AutoTuning SET", "RawADCSVSELHB_7315", "3");
            GetParamValue(ref m_nRawADCSVSELLB_6315, "AutoTuning SET", "RawADCSVSELLB_6315", "3");
            GetParamValue(ref m_nRawADCSVSELHB_6315, "AutoTuning SET", "RawADCSVSELHB_6315", "3");
            GetParamValue(ref m_nRawADCSLGLB, "AutoTuning SET", "RawADCSLGLB", "0");
            GetParamValue(ref m_nRawADCSLGHB, "AutoTuning SET", "RawADCSLGHB", "2");
            GetParamValue(ref m_nRawADCSSELGMLB, "AutoTuning SET", "RawADCSSELGMLB", "0");
            GetParamValue(ref m_nRawADCSSELGMHB, "AutoTuning SET", "RawADCSSELGMHB", "1");
            GetParamValue(ref m_nRawADCSFitADCLB, "AutoTuning SET", "RawADCSFitADCLB", "3000");
            GetParamValue(ref m_nRawADCSFitADCHB, "AutoTuning SET", "RawADCSFitADCHB", "6000");
            GetParamValue(ref m_nRawADCSADCTestFrame, "AutoTuning SET", "RawADCSADCTestFrame", "10");
            GetParamValue(ref m_nRawADCSGen7EnableHWTXN, "AutoTuning SET", "RawADCSGen7EnableHWTXN", "0");
            GetParamValue(ref m_nRawADCSGen6or7EnableFWTX4, "AutoTuning SET", "RawADCSGen6or7EnableFWTX4", "0");

            GetParamValue(ref m_nSelfTraceType, "AutoTuning SET", "SelfTraceType", "0");

            GetParamValue(ref m_nSelfFSGetDataType, "AutoTuning SET", "SelfFSGetDataType", "0");
            GetParamValue(ref m_nSelfFSGetReportType, "AutoTuning SET", "SelfFSGetReportType", "0");
            GetParamValue(ref m_nSelfFSGetReportSequence, "AutoTuning SET", "SelfFSGetReportSequence", "0");
            GetParamValue(ref m_nSelfFSGetReportByte, "AutoTuning SET", "SelfFSGetReportByte", "01");
            GetParamValue(ref m_nSelfFSGetSignalReport, "AutoTuning SET", "SelfFSGetSignalReport", "0");
            GetParamValue(ref m_nSelfFSDisplayWarning, "AutoTuning SET", "SelfFSDisplayWarning", "1");
            GetParamValue(ref m_nSelfFSRepeatCount, "AutoTuning SET", "SelfFSRepeatCount", "0");
            GetParamValue(ref m_nSelfFSTestFrame, "AutoTuning SET", "SelfFSTestFrame", "100");
            GetParamValue(ref m_nSelfFSSum, "AutoTuning SET", "SelfFSSum", "-1");
            GetParamValue(ref m_nSelfFSGain, "AutoTuning SET", "SelfFSGain", "-1");
            GetParamValue(ref m_nSelfFSCAG, "AutoTuning SET", "SelfFSCAG", "-1");
            GetParamValue(ref m_nSelfFSIQ_BSH, "AutoTuning SET", "SelfFSIQ_BSH", "-1");
            GetParamValue(ref m_nSelfFSGetKValue, "AutoTuning SET", "SelfFSGetKValue", "0");
            GetParamValue(ref m_nSelfFSRunKSequence, "AutoTuning SET", "SelfFSRunKSequence", "0");
            m_sSelfFS_MS_MM_RX0FileName = ReadValue("AutoTuning SET", "SelfFS_MS_MM_RX0FileName", "");
            GetParamValue(ref m_nSelfFSNCPValueLB, "AutoTuning SET", "SelfFSNCPValueLB", "0");
            GetParamValue(ref m_nSelfFSNCPValueHB, "AutoTuning SET", "SelfFSNCPValueHB", "31");
            GetParamValue(ref m_nSelfFSNCNValueLB, "AutoTuning SET", "SelfFSNCNValueLB", "0");
            GetParamValue(ref m_nSelfFSNCNValueHB, "AutoTuning SET", "SelfFSNCNValueHB", "31");
            GetParamValue(ref m_nSelfFSGetReportTimeout, "AutoTuning SET", "SelfFSGetReportTimeout", "10000");
            GetParamValue(ref m_nSelfFSReportNumber, "AutoTuning SET", "SelfFSReportNumber", "1000");
            GetParamValue(ref m_nSelfFSReportSignedValue, "AutoTuning SET", "SelfFSReportSignedValue", "0");
            GetParamValue(ref m_nSelfFSRunCALData, "AutoTuning SET", "SelfFSRunCALData", "0");
            GetParamValue(ref m_nSelfFSCALValue, "AutoTuning SET", "SelfFSCALValue", "15");
            GetParamValue(ref m_nSelfFSCALStartTrace, "AutoTuning SET", "SelfFSCALStartTrace", "1");
            GetParamValue(ref m_nSelfFSCALStartTraceNumber, "AutoTuning SET", "SelfFSCALStartTraceNumber", "0");
            GetParamValue(ref m_nSelfFSCALEndTraceNumber, "AutoTuning SET", "SelfFSCALEndTraceNumber", "0");

            GetParamValue(ref m_nSelfPNSTestFrame, "AutoTuning SET", "SelfPNSTestFrame", "10");
            GetParamValue(ref m_nSelfPNSGetKValue, "AutoTuning SET", "SelfPNSGetKValue", "0");
            GetParamValue(ref m_nSelfPNSNCPNCNValueLB, "AutoTuning SET", "SelfPNSNCPNCNValueLB", "0");
            GetParamValue(ref m_nSelfPNSNCPNCNValueHB, "AutoTuning SET", "SelfPNSNCPNCNValueHB", "31");
            GetParamValue(ref m_nSelfPNS_MS_MM_RX0Count, "AutoTuning SET", "SelfPNS_MS_MM_RX0Count", "42");
            GetParamValue(ref m_nSelfPNSStartTrace, "AutoTuning SET", "SelfPNSStartTrace", "1");

            GetParamValue(ref m_nSelfFPStandardPH1PH2Sum, "AutoTuning SET", "SelfFPStandardPH1PH2Sum", "111");
            GetParamValue(ref m_nSelfFPStandardSum, "AutoTuning SET", "SelfFPStandardSum", "128");
            GetParamValue(ref m_nSelfFPPH2_LMT_LB, "AutoTuning SET", "SelfFPPH2_LMT_LB", "160");
        }

        public static void LoadGen8FWParameterAddress(ICSolutionType eICSolutionType, string sIniPath)
        {
            JudgeFileType(sIniPath);

            m_nRelativeAddress_MS_PH1 = 0x009E;
            m_nRelativeAddress_MS_PH2 = 0x009F;
            m_nRelativeAddress_MS_PH3 = 0x00A0;
            m_nRelativeAddress_MS_AFE_DFT_NUM = 0x009C;
            m_nRelativeAddress_MS_AFE_SP_NUM = 0x009A;
            m_nRelativeAddress_MS_AFE_EFFECT_NUM = 0x009B;
            m_nRelativeAddress_PKT_WC = 0x0004;
            m_nRelativeAddress_MS_BIN_FIRCOEF_SEL_TAP_NUM = 0x0024;
            m_nRelativeAddress_MS_IQ_BSH_GP0_GP1 = 0x0010;
            m_nRelativeAddress_MS_ANA_TP_CTL_01 = 0x00DB;
            m_nRelativeAddress_MS_ANA_TP_CTL_01_2 = 0x00E2;
            m_nRelativeAddress_MS_ANA_CTL_04 = 0x0127;
            m_nRelativeAddress_MS_ANA_CTL_04_2 = 0x012C;
            m_nRelativeAddress_MS_ANA_TP_CTL_06 = 0x00E0;
            m_nRelativeAddress_MS_ANA_TP_CTL_06_2 = 0x00E7;
            m_nRelativeAddress_MS_ANA_TP_CTL_07 = 0x00E8;

            if (File.Exists(sIniPath) == false)
                return;

            string sGroupName = "8F18";

            if (eICSolutionType == ICSolutionType.Solution_8F09)
                sGroupName = "8F09";
            else if (eICSolutionType == ICSolutionType.Solution_8F11)
                sGroupName = "8F11";
            else if (eICSolutionType == ICSolutionType.Solution_8F18)
                sGroupName = "8F18";

            GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_PH1, sIniPath, sGroupName, "_MS_PH1", "0");
            GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_PH2, sIniPath, sGroupName, "_MS_PH2", "0");
            GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_PH3, sIniPath, sGroupName, "_MS_PH3", "0");
            GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_AFE_DFT_NUM, sIniPath, sGroupName, "_MS_AFE_DFT_NUM", "0");
            GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_AFE_SP_NUM, sIniPath, sGroupName, "_MS_AFE_SP_NUM", "0");
            GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_AFE_EFFECT_NUM, sIniPath, sGroupName, "_MS_AFE_EFFECT_NUM", "0");
            GetParamValueByMathCalculate(ref m_nRelativeAddress_PKT_WC, sIniPath, sGroupName, "PKT_WC", "0");

            GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_BIN_FIRCOEF_SEL_TAP_NUM, sIniPath, sGroupName, "_MS_BIN_FIRCOEF_SEL_TAP_NUM", "0");
            GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_IQ_BSH_GP0_GP1, sIniPath, sGroupName, "_MS_IQ_BSH_GP0_GP1", "0");
            GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_ANA_TP_CTL_01, sIniPath, sGroupName, "_MS_ANA_TP_CTL_01", "0");

            if (eICSolutionType == ICSolutionType.Solution_8F18)
                GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_ANA_TP_CTL_01_2, sIniPath, sGroupName, "_MS_ANA_TP_CTL_01_2", "0");

            GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_ANA_CTL_04, sIniPath, sGroupName, "_MS_ANA_CTL_04", "0");

            if (eICSolutionType == ICSolutionType.Solution_8F18)
            {
                GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_ANA_CTL_04_2, sIniPath, sGroupName, "_MS_ANA_CTL_04_2", "0");
                GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_ANA_TP_CTL_06, sIniPath, sGroupName, "_MS_ANA_TP_CTL_06", "0");
                GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_ANA_TP_CTL_06_2, sIniPath, sGroupName, "_MS_ANA_TP_CTL_06_2", "0");
                GetParamValueByMathCalculate(ref m_nRelativeAddress_MS_ANA_TP_CTL_07, sIniPath, sGroupName, "_MS_ANA_TP_CTL_07", "0");
            }
        }

        public static void SetScreenSize()
        {
            if (frmMain.m_byteEDIDData_Array == null || frmMain.m_byteEDIDData_Array.Length == 0)
            {
                frmMain.m_nScreenSizeWidth = m_nScreenSizeWidth;
                frmMain.m_nScreenSizeHeight = m_nScreenSizeHeight;
            }
        }

        public static void LoadStepSettingParam(ref StepSettingParameter[] cItem_Array)
        {
            for (int nIndex = 0; nIndex < nSTEP_NUMBER; nIndex++)
            {
                if (cItem_Array[nIndex] == null)
                    cItem_Array[nIndex] = new StepSettingParameter();

                cItem_Array[nIndex].m_sStepName = m_sStepNameSet_Array[nIndex];
                cItem_Array[nIndex].m_sStepParamName = m_sStepParameterNameSet_Array[nIndex];
                cItem_Array[nIndex].m_Step = m_eStepSet_Array[nIndex];
                GetParamValue(ref cItem_Array[nIndex].m_bEnable, "TEST ITEM", m_sStepParameterNameSet_Array[nIndex], "0");
            }
        }

        public static void LoadStackType(ref StackType[] cItem_Array)
        {
            string[] sStackTypeName_Array = new string[] 
            {
                "Normal",
                "Oncell",
                //"Self"
            };
            double[] dStackTypeFrequencyLB_Array = new double[] 
            { 
                100.0, 
                70.0, 
                26.0 
            };
            double[] dStackTypeFrequencyHB_Array = new double[] 
            { 
                250.0, 
                250.0, 
                98.0 
            };

            for (int nIndex = 0; nIndex < sStackTypeName_Array.Length; nIndex++)
            {
                if (cItem_Array[nIndex] == null)
                    cItem_Array[nIndex] = new StackType();

                cItem_Array[nIndex].m_sTypeName = sStackTypeName_Array[nIndex];
                cItem_Array[nIndex].m_fLB = dStackTypeFrequencyLB_Array[nIndex];
                cItem_Array[nIndex].m_fHB = dStackTypeFrequencyHB_Array[nIndex];
            }
        }

        private static int SetInterface(string sInterfaceType)
        {
            int nInterfaceType = (int)UserInterfaceDefine.InterfaceType.IF_HID_OVER_I2C_TP;

            if (sInterfaceType == MainConstantParameter.m_sINTERFACE_HIDOVERI2C)
                nInterfaceType = (int)UserInterfaceDefine.InterfaceType.IF_HID_OVER_I2C_TP;
            else if (sInterfaceType == MainConstantParameter.m_sINTERFACE_USB)
                nInterfaceType = (int)UserInterfaceDefine.InterfaceType.IF_USB;
            else if (sInterfaceType == MainConstantParameter.m_sINTERFACE_I2C)
                nInterfaceType = (int)UserInterfaceDefine.InterfaceType.IF_I2C;
            else if (sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_RISING_HALF)
                nInterfaceType = (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE;
            else if (sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_RISING)
                nInterfaceType = (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING;
            else if (sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING_HALF)
                nInterfaceType = (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE;

            else if (sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING)
                nInterfaceType = (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING;

            return nInterfaceType;
        }

        private static void SetDVDD(string sValue)
        {
            if (sValue == "2.8V")
                m_nDVDD = 28;
            else if (sValue == "3.0V")
                m_nDVDD = 30;
            else if (sValue == "3.3V")
                m_nDVDD = 33;
            else if (sValue == "5.0V")
                m_nDVDD = 50;
            else
                m_nDVDD = 33;
        }

        private static void SetVIO(string sValue)
        {
            if (sValue == "1.8V")
                m_nVIO = 18;
            else if (sValue == "2.8V")
                m_nVIO = 28;
            else if (sValue == "3.0V")
                m_nVIO = 30;
            else if (sValue == "3.3V")
                m_nVIO = 33;
            else if (sValue == "5.0V")
                m_nVIO = 50;
            else
                m_nVIO = 33;
        }
    }
}

