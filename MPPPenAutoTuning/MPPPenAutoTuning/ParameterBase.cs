using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using MPPPenAutoTuning;
using System.Linq;
using System.IO;
using Elan;

namespace MPPPenAutoTuningParameter
{
    public class ParameterBase
    {
        private const int m_nFILE_TYPE_UNKNOW = 0;
        private const int m_nFILE_TYPE_INI = 1;
        private const int m_nFILE_TYPE_XML = 2;
        protected const int m_nSTEP_NUMBER = 10;

        public const int m_nPRESSURE_DATA_NUMBER = 15;

        protected static string m_sSettingFilePath = "";
        protected static string m_sDefaultFilePath = "";
        private static int m_nFileType = m_nFILE_TYPE_UNKNOW;
        protected static bool m_bDataFormatErrorFlag = false;

        public static string[] m_sStepSettingNameSet_Array = new string[]
        { 
            "Noise",
            "Tilt Noise",
            "DigiGain Tuning",
            "TP_Gain Tuning",
            "PeakCheck Tuning",
            "Digital Tuning",
            "Tilt Tuning",
            "Pressure Tuning",
            "Linearity Tuning",
            "Server Control" 
        };

        public static string[] m_sStepSettingParamNameSet_Array = new string[] 
        { 
            "Noise",
            "TiltNoise",
            "DigiGainTuning",
            "TP_GainTuning",
            "PeakCheckTuning",
            "DigitalTuning",
            "TiltTuning",
            "PressureTuning",
            "LinearityTuning",
            "ServerControl" 
        };

        public static MainTuningStep[] m_eMainTuningStepSet_Array = new MainTuningStep[] 
        { 
            MainTuningStep.NO,
            MainTuningStep.TILTNO,
            MainTuningStep.DIGIGAINTUNING,
            MainTuningStep.TPGAINTUNING,
            MainTuningStep.PEAKCHECKTUNING,
            MainTuningStep.DIGITALTUNING,
            MainTuningStep.TILTTUNING,
            MainTuningStep.PRESSURETUNING,
            MainTuningStep.LINEARITYTUNING,
            MainTuningStep.SERVERCONTRL 
        };

        protected static void JudgeFileType(string m_sFileName)
        {
            string[] sToken_Array = m_sFileName.Split('.');

            if (sToken_Array == null)
            {
                m_nFileType = m_nFILE_TYPE_UNKNOW;
                return;
            }

            if (sToken_Array[sToken_Array.Length - 1].Equals("ini") == true)
                m_nFileType = m_nFILE_TYPE_INI;
            else
                m_nFileType = m_nFILE_TYPE_XML;
        }

        /// <summary>
        /// 從參數設定檔讀取參數
        /// </summary>
        /// <param name="sSection">Group Name</param>
        /// <param name="sKey">Parameter Name</param>
        /// <param name="sDefault">預設值</param>
        /// <returns>回傳讀取的結果</returns>
        protected static string ReadValue(string sSection, string sKey, string sDefault = "")
        {
            if (m_nFileType == m_nFILE_TYPE_INI)
                return IniReadValue(sSection, sKey, sDefault);
            else
                return sDefault;
        }

        /// <summary>
        /// 將Parameter數值寫入參數設定檔
        /// </summary>
        /// <param name="sSection">Group Name</param>
        /// <param name="sKey">Parameter Name</param>
        /// <param name="sValue">參數數值</param>
        protected static void WriteValue(string sSection, string sKey, string sValue, bool bAlwaysWrite = true, bool bHex = false)
        {
            if (bHex == true)
                sValue = string.Format("0x{0}", sValue);

            if (m_nFileType == m_nFILE_TYPE_INI)
                IniWriteValue(sSection, sKey, sValue, bAlwaysWrite);
        }

        protected static void WriteValue(string sSection, string sKey, bool sValue, bool bAlwaysWrite = true)
        {
            if (m_nFileType == m_nFILE_TYPE_INI)
            {
                if (sValue == true)
                    IniWriteValue(sSection, sKey, "1", bAlwaysWrite);
                else
                    IniWriteValue(sSection, sKey, "0", bAlwaysWrite);
            }
        }

        private static void IniWriteValue(string sSection, string sKey, string sValue, bool bAlwaysWrite = true, bool bSpace = true)
        {
            if (bAlwaysWrite == false)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = Win32.GetPrivateProfileString(sSection, sKey, "DataNotExist![N/A]", temp, 255, m_sSettingFilePath);

                if (temp != null)
                {
                    if (temp.ToString() == "DataNotExist![N/A]")
                        return;
                }
                else
                    return;
            }

            if (bSpace == true)
                sValue = string.Format(" {0}", sValue);

            Win32.WritePrivateProfileString(sSection, sKey, sValue, m_sSettingFilePath);
        }

        private static string IniReadValue(string sSection, string sKey, string sDefault = "")
        {
            string sOutputValue = "";

            StringBuilder temp = new StringBuilder(255);
            int i = Win32.GetPrivateProfileString(sSection, sKey, sDefault, temp, 255, m_sSettingFilePath);

            if (temp != null)
                sOutputValue = temp.ToString();
            else
                sOutputValue = sDefault;

            if (File.Exists(m_sDefaultFilePath) == true)
            {
                StringBuilder sb = new StringBuilder(255);
                int nValue = Win32.GetPrivateProfileString(sSection, sKey, "DataNotExist!\\[N/A]", sb, 255, m_sDefaultFilePath);

                if (sb != null)
                {
                    if (sb.ToString() != "DataNotExist!\\[N/A]")
                        sOutputValue = sb.ToString();
                }
            }

            return sOutputValue;
        }

        protected static string IniReadValue(string sSection, string sKey, string sPath, string sDefault)
        {
            string sOutputValue = "";

            StringBuilder temp = new StringBuilder(255);
            int i = Win32.GetPrivateProfileString(sSection, sKey, sDefault, temp, 255, sPath);

            if (temp != null)
                sOutputValue = temp.ToString();
            else
                sOutputValue = sDefault;

            if (File.Exists(m_sDefaultFilePath) == true)
            {
                StringBuilder sb = new StringBuilder(255);
                int nValue = Win32.GetPrivateProfileString(sSection, sKey, "DataNotExist!\\[N/A]", sb, 255, m_sDefaultFilePath);

                if (sb != null)
                {
                    if (sb.ToString() != "DataNotExist!\\[N/A]")
                        sOutputValue = sb.ToString();
                }
            }

            return sOutputValue;
        }

        // 傳入Parameter Name與Index，傳回Parameter_Index
        protected static string GetParameter(string sParameter, int nParameterIndex)
        {
            if (nParameterIndex == 0)
                return sParameter;
            else
                return string.Format("{0}_{1}", sParameter, (nParameterIndex + 1));
        }

        // 宣告取得Parameter Value
        protected static void GetParameterValue(ref bool bValue, string sGroupName, string sParameterName, string sDefault)
        {
            string sIniValue = ReadValue(sGroupName, sParameterName, sDefault);

            try
            {
                bValue = Convert.ToInt32(sIniValue) >= 1;
            }
            catch (Exception ex)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, ex.Message.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatErrorFlag = true;

                if (sDefault == "1")
                    bValue = true;
                else
                    bValue = false;
            }
        }

        protected static void GetParameterValue(ref int nValue, string sGroupName, string sParameterName, string sDefault, bool bHex = false)
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
            catch (Exception ex)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, ex.Message.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatErrorFlag = true;

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

        protected static void GetParameterValue(ref short nValue, string sGroupName, string sParameterName, string sDefault, bool bHex = false)
        {
            string sIniValue = ReadValue(sGroupName, sParameterName, sDefault);

            try
            {
                if (bHex == true)
                    nValue = Int16.Parse(sIniValue, System.Globalization.NumberStyles.HexNumber);
                else
                    nValue = Convert.ToInt16(sIniValue);
            }
            catch (Exception ex)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, ex.Message.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatErrorFlag = true;

                if (bHex == true)
                    nValue = Int16.Parse(sDefault, System.Globalization.NumberStyles.HexNumber);
                else
                    nValue = Convert.ToInt16(sDefault);
            }

        }

        protected static void GetParameterValue(ref double dValue, string sGroupName, string sParameterName, string sDefault)
        {
            string sIniValue = ReadValue(sGroupName, sParameterName, sDefault);

            try
            {
                dValue = ElanConvert.ConvertStringToDouble(sIniValue, false);
            }
            catch (Exception ex)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, ex.Message.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatErrorFlag = true;
                dValue = ElanConvert.ConvertStringToDouble(sDefault, false);
            }
        }

        protected static void GetParameterValue(ref Color colorValue, string sGroupName, string sParameterName, string sDefault)
        {
            string sIniValue = ReadValue(sGroupName, sParameterName, sDefault);

            try
            {
                string[] sToken_Array = sIniValue.Split(',');
                int nR = int.Parse(sToken_Array[0]);
                int nG = int.Parse(sToken_Array[1]);
                int nB = int.Parse(sToken_Array[2]);
                colorValue = Color.FromArgb(nR, nG, nB);
            }
            catch (Exception ex)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, ex.Message.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatErrorFlag = true;

                string[] sToken_Array = sDefault.Split(',');
                int nR = int.Parse(sToken_Array[0]);
                int nG = int.Parse(sToken_Array[1]);
                int nB = int.Parse(sToken_Array[2]);
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

            for (int nTokenIndex = 0; nTokenIndex < sToken_Array.Length; nTokenIndex++)
            {
                int nTraceNumber = 0;

                if (int.TryParse(sToken_Array[nTokenIndex], out nTraceNumber) == false)
                    nTraceNumber = -1;

                nParameterValue_List.Add(nTraceNumber);
            }
        }

        protected static void GetParameterListValue(ref List<float> fParameterValue_List, string sGroupName, string sParameterName, string sDefault)
        {
            //Process the special Trace Number and Trace High boundary
            if (fParameterValue_List == null)
                fParameterValue_List = new List<float>();
            else
                fParameterValue_List.Clear();

            string[] sToken_Array = ReadValue(sGroupName, sParameterName, sDefault).Split(',');

            for (int nTokenIndex = 0; nTokenIndex < sToken_Array.Length; nTokenIndex++)
            {
                float fValue = 0.0f;

                fValue = ConvertToFloat((string)sToken_Array[nTokenIndex]);

                fParameterValue_List.Add(fValue);
            }
        }

        protected static void GetParameterValueByMathCalculate(ref int nValue, string sGroupName, string sParameterName, string sDefault)
        {
            bool bErrorFlag = false;
            string sErrorMessage = "";
            string sIniValue = ReadValue(sGroupName, sParameterName, sDefault);

            string sRPNExperssion = StringConvert.ConvertStringToRPN(ref bErrorFlag, ref sErrorMessage, StringConvert.InsertBlank(sIniValue));

            if (bErrorFlag == false)
            {
                nValue = StringConvert.GetCalculateResult(sRPNExperssion);
            }
            else
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, sErrorMessage.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatErrorFlag = true;

                sRPNExperssion = StringConvert.ConvertStringToRPN(ref bErrorFlag, ref sErrorMessage, StringConvert.InsertBlank(sDefault));
                nValue = StringConvert.GetCalculateResult(sRPNExperssion);
            }
        }

        protected static void GetParameterValueByMathCalculate(ref UInt16 nValue, string sFilePath, string sGroupName, string sParameterName, string sDefault)
        {
            bool bErrorFlag = false;
            string sErrorMessage = "";
            string sIniValue = IniReadValue(sGroupName, sParameterName, sFilePath, sDefault);

            string sRPNExperssion = StringConvert.ConvertStringToRPN(ref bErrorFlag, ref sErrorMessage, StringConvert.InsertBlank(sIniValue));

            if (bErrorFlag == false)
            {
                nValue = StringConvert.GetCalculateResult_Uint16(sRPNExperssion);
            }
            else
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, sErrorMessage.ToString());
                MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bDataFormatErrorFlag = true;

                sRPNExperssion = StringConvert.ConvertStringToRPN(ref bErrorFlag, ref sErrorMessage, StringConvert.InsertBlank(sDefault));
                nValue = StringConvert.GetCalculateResult_Uint16(sRPNExperssion);
            }
        }

        private static float ConvertToFloat(string sValue)
        {
            StringBuilder sb = new StringBuilder(255);
            float fReturnValue = 0.0f;
            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            string sReplacedIniValue = sValue.Replace(".", sb.ToString());
            float.TryParse(sReplacedIniValue, out fReturnValue);
            return fReturnValue;
        }
    }

    public class ParamAutoTuning : ParameterBase
    {
        private static frmMain m_cfrmMain = null;

        public static void GetMainForm(frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
        }

        //[Main Setting]
        public static int m_nVersionType = 0;

        //[LoadData Mode Setting]
        public static bool m_bSkipPreviousStepCheck = false;

        //[Step Setting]
        public static StepSettingParameter[] m_cStepSettingParameter_Array = new StepSettingParameter[m_nSTEP_NUMBER];

        public class StepSettingParameter
        {
            public string m_sStepName = "";
            public string m_sStepParameterName = "";
            public MainTuningStep m_eTuningStep = MainTuningStep.ELSE; 
            public bool m_bEnable = false;
        }

        //[Connect Setting]
        public static Int16 m_nUSBVID = 0x04F3;
        public static Int16 m_nUSBPID = 0x0000;
        public static Int16 m_nDVDD = 33;
        public static Int16 m_nVIO = 33;
        public static Int16 m_nI2CAddress = 0x20;
        public static Int16 m_nNormalLength = 0x3F;
        //public static string m_sCOMPort = "";

        public static int m_nWindowsSPIInterfaceType = (int)UserInterfaceDefine.InterfaceType.IF_HID_OVER_I2C_TP;
        public static int m_nWindowsSPICommandLength = 10;

        //[Flow Method Setting]
        public static int m_nFlowMethodType = 0;

        //[Network Setting]
        public static string m_sServerIP = "";
        public static string m_sClientIP = "";
        public static int m_nPort = 0;

        public static int m_nServerReadTimeout = 0;
        public static int m_nServerWriteTimeout = 0;
        public static int m_nServerWriteDelayTime = 0;
        public static int m_nClientReadTimeout = 0;
        public static int m_nClientWriteTimeout = 0;
        public static int m_nClientSendingDelayTime = 0;
        public static int m_nClientWriteDelayTime = 0;

        //[Coordinate Setting]
        public static double m_dStartXAxisCoordinate = 0.0;
        public static double m_dStartYAxisCoordinate = 0.0;
        public static double m_dEndXAxisCoordinate = 0.0;
        public static double m_dEndYAxisCoordinate = 0.0;
        public static double m_dContactZAxisCoordinate = 0.0;
        //public static double m_dContactPressZAxisCoordinate = 0.0;
        public static double m_dHoverHeight_DT1st = 0.0;
        public static double m_dHoverHeight_DT2nd = 0.0;
        public static double m_dHoverHeight_PP = 0.0;
        public static double m_dHoverHeight_PT = 0.0;
        public static double m_dHoverHeight_PCT1st = 0.0;
        public static double m_dHoverHeight_PCT2nd = 0.0;
        public static double m_dPushDownZAxisCoordinate_DGT = 0.0;
        public static double m_dRobotMovingTimeout = 0.0;
        public static double m_dLTHorShiftYAxisCoordinate = 0.0;
        public static double m_dLTVerShiftXAxisCoordinate = 0.0;
        public static int m_nPTInitialWeight = 0;
        public static double m_dPTIni1stZDropScale = 0.0;
        public static double m_dPTIni2ndZDropScale = 0.0;
        public static double m_dPT1stZDropScale = 0.0;
        public static double m_dPT2ndZDropScale = 0.0;
        public static int m_nMaxLoadingWeight = 0;
        public static double m_dPTFGDetectPowerOnZDropScale = 0.0;
        public static int m_dPTFGDetectPowerOnMaxWeight = 0;

        //[Speed Setting]
        public static double m_dDGTDrawingSpeed = 0.0;
        public static double m_dTPGTDrawingSpeed = 0.0;
        public static double m_dPCTDrawingSpeed = 0.0;
        public static double m_dDTDrawingSpeed = 0.0;
        public static double m_dTTDrawingSpeed = 0.0;
        public static double m_dTTSlantDrawingSpeed = 0.0;
        public static double m_dLTDrawingSpeed = 0.0;

        //[GoDraw Controller]
        public static string m_sGoDrawCtrlrCOMPort = "NA";
        public static double m_dGoDrawCtrlrSpeed = 5.0;
        public static int m_nGoDrawCtrlrMaxCoordinateX = 310;
        public static int m_nGoDrawCtrlrMaxCoordinateY = 200;
        public static int m_nGoDrawCtrlrMaxServoValue = 30000;
        public static int m_nGoDrawCtrlrMinServoValue = 3000;
        public static int m_nGoDrawCtrlrDistance = 10;
        public static int m_nGoDrawCtrlrTopServoValue = 27000;
        public static int m_nGoDrawCtrlrHoverServoValue = 22000;
        public static int m_nGoDrawCtrlrContactServoValue = 15000;
        public static int m_nGoDrawCtrlrDestinationCoordinateX = 0;
        public static int m_nGoDrawCtrlrDestinationCoordinateY = 0;
        public static int m_nGoDrawCtrlrMoveType = 0;
        public static int m_nGoDrawCtrlrEstimateType = 0;
        public static double m_dGoDrawCtrlrMinSpeed = 0.5;
        public static double m_dGoDrawCtrlrMaxSpeed = 200.0;
        public static double m_dGoDrawCtrlrCauseDelayMaxSpeed = 2.5;
        public static double m_dGoDrawCtrlrEstimateDelay_Speed1 = 0.5;
        public static double m_dGoDrawCtrlrEstimateDelay_Speed2 = 2.0;
        public static int m_nGoDrawCtrlrEstimateDelay_Distance1 = 0;
        public static int m_nGoDrawCtrlrEstimateDelay_Time1 = 0;
        public static int m_nGoDrawCtrlrEstimateDelay_Distance2 = 300;
        public static int m_nGoDrawCtrlrEstimateDelay_Time2_Speed1 = 6000;
        public static int m_nGoDrawCtrlrEstimateDelay_Time2_Speed2 = 1000;

        //[GoDraw Coordinate Setting]
        public static int m_nGoDrawStartXAxisCoordinate = 0;
        public static int m_nGoDrawStartYAxisCoordinate = 0;
        public static int m_nGoDrawEndXAxisCoordinate = 0;
        public static int m_nGoDrawEndYAxisCoordinate = 0;
        public static int m_nGoDrawTopZServoValue = 27000;
        public static int m_nGoDrawContactZServoValue = 15000;
        public static int m_nGoDrawHoverZServoValue_DT1st = 22000;
        public static int m_nGoDrawHoverZServoValue_DT2nd = 23000;
        public static int m_nGoDrawHoverZServoValue_PCT1st = 16000;
        public static int m_nGoDrawHoverZServoValue_PCT2nd = 17000;
        public static int m_nGoDrawPushDownZServoValue_DGT = -1000;
        public static int m_nGoDrawLTHorShiftYAxisCoordinate = 50;
        public static int m_nGoDrawLTVerShiftXAxisCoordinate = 50;
        public static int m_nGoDrawDelayTime = 1000;

        //[TPGT Coordinate Setting]
        public static double m_dTPGTHorizontalStartXAxisCoordinate = 0.0;
        public static double m_dTPGTHorizontalStartYAxisCoordinate = 0.0;
        public static double m_dTPGTHorizontalEndXAxisCoordinate = 0.0;
        public static double m_dTPGTHorizontalEndYAxisCoordinate = 0.0;
        public static double m_dTPGTVerticalStartXAxisCoordinate = 0.0;
        public static double m_dTPGTVerticalStartYAxisCoordinate = 0.0;
        public static double m_dTPGTVerticalEndXAxisCoordinate = 0.0;
        public static double m_dTPGTVerticalEndYAxisCoordinate = 0.0;
        public static double m_dTPGTContactZAxisCoordinate = 0.0;

        //[TPGT GoDraw Coordinate Setting]
        public static int m_nGoDrawTPGTHorizontalStartXAxisCoordinate = 0;
        public static int m_nGoDrawTPGTHorizontalStartYAxisCoordinate = 0;
        public static int m_nGoDrawTPGTHorizontalEndXAxisCoordinate = 0;
        public static int m_nGoDrawTPGTHorizontalEndYAxisCoordinate = 0;
        public static int m_nGoDrawTPGTVerticalStartXAxisCoordinate = 0;
        public static int m_nGoDrawTPGTVerticalStartYAxisCoordinate = 0;
        public static int m_nGoDrawTPGTVerticalEndXAxisCoordinate = 0;
        public static int m_nGoDrawTPGTVerticalEndYAxisCoordinate = 0;
        public static int m_nGoDrawTPGTContactZServoValue = 15000;

        //[Project Information Setting]
        public static string m_sProjectName = "";
        public static int m_nFWTypeIndex = 0;

        //[FW Check Setting]
        public static int m_nFWCheckVersion = 0;

        //[Frequency Boundary Setting]
        public static int m_nFrequencyLB = 326;
        public static int m_nFrequencyLB_MPP180 = 510;
        public static int m_nFrequencyHB = 762;

        //[Color/Pattern Setting]
        public static int m_nDisplayType = 0;
        public static bool m_bDisplayReportNumber = true;
        public static bool m_bDisplayProgressStatus = true;
        public static int m_nColorSelectIndex = 0;
        public static int m_nDisplayColor = 0;
        public static int m_nPatternType = 0;
        public static string m_sManualPatternPath = "";

        //[Gen8 Command Setting]
        public static int m_nGen8AFEType = 0;      //0:NA,  1:DT Mode,  2:CT Mode
        public static int m_nGen8FilterType = 0;    //0:NA,  1:Disable Filter,  2:0~300KHz LPF,  3:0~75KHz LPF
        public static int m_nGen8CommandScriptType = 0;
        public static string m_sGen8UserDefinedPath = "";
        public static int m_nGen8EnableCheckUserDefinedFormat = 0;
        public static int m_nGen8SendCommandDelayTime = 100;

        //[Command Flow Setting]
        public static int m_nEnableReK = 0;

        //[Gen8 Setting]
        public static int m_nGen8TraceType = 0;     //0:RX,  1:TX,  2:RX & TX
        public static int m_nGen8RealTraceNumber = 0;
        public static int m_nGen8BeaconRowNumber = 20;
        public static int m_nGen8BHFRowNumber = 15;
        public static int m_nGen8PTHFRowNumber = 15;
        public static int m_nGen8ReportDataLength = 65;
        public static int m_nGen8GetAnalogAfterSwitchTestMode = 1;
        public static int m_nGen8SKIP_NUM = 12;
        public static int m_nGen8ProjectOptionDisableValue = 0x0001;
        public static int m_nGen8FWIPOptionDisableValue = 0x0000;

        //[Report Data Setting]
        public static int m_nReportDataLength = 0;
        public static int m_nShiftStartByte = 0;
        public static int m_nShiftByteNumber = 0;

        //[Noise & Tilt Noise Step Setting]
        public static int m_nNoiseDataType = 0;
        public static int m_nNoiseFrameNumber = 1000;
        public static int m_nNoiseReportNumber = 10000;
        public static int m_nNoiseValidReportNumber = 9000;
        public static int m_nNoiseProcessReportNumber = -1;
        public static double m_dNoiseTimeout = 0.0;
        public static double m_dNoiseNoReportInterruptTime = 0.0;
        public static int m_nNoiseDisplayReportRate = 0;
        public static int m_nNoiseGetDistributionData = 0;

        //[DGT Step Setting]
        public static int m_nDGTDrawType = 0;
        /*
        public static int m_nDGTDefaultScaleDigiGain_P0 = 0;
        public static int m_nDGTDefaultScaleDigiGain_Beacon_Rx = 0;
        public static int m_nDGTDefaultScaleDigiGain_Beacon_Tx = 0;
        public static int m_nDGTDefaultScaleDigiGain_PTHF_Rx = 0;
        public static int m_nDGTDefaultScaleDigiGain_PTHF_Tx = 0;
        public static int m_nDGTDefaultScaleDigiGain_BHF_Rx = 0;
        public static int m_nDGTDefaultScaleDigiGain_BHF_Tx = 0;
        */
        public static int m_nDGTRXValidReportNumber = 0;
        public static int m_nDGTTXValidReportNumber = 0;
        public static int m_nDGTMultiplyValue = 16384;
        public static int m_nDGTDividValue = 100;

        //[TPGT Step Setting]
        public static int m_nTPGTRXValidReportNumber = 0;
        public static int m_nTPGTTXValidReportNumber = 0;
        public static int m_nTPGTDisplayMessage = 1;
        public static int m_nTPGTVAngle = 45;
        public static int m_nTPGTHorizontalRAngle = 90;
        public static int m_nTPGTVerticalRAngle = 0;

        //[Normal Step Setting]
        public static int m_nNormalValidReportNumber = 150;
        public static double m_dNormal800to400PwrRatio = 0.5;

        //[TRxS Step Setting]
        public static int m_nTRxSRXValidReportNumber = 150;
        public static int m_nTRxSTXValidReportNumber = 70;

        //[TT Step Setting]
        public static int m_nTTValidTipTraceNumber = 5;
        public static int m_nTTRXValidReportNumber = 150;
        public static int m_nTTTXValidReportNumber = 70;

        //[PT Step Setting]
        public static int m_nPPValidReportNumber = 100;
        public static int m_nPTValidReportNumber = 1;
        public static int m_nPTStartSkipReportNumber = 150;
        public static int m_nPTEndSkipReportNumber = 150;
        public static double m_dPPRecordTime = 10.0;
        public static double m_dPTRecordTime = 10.0;
        public static int m_nPTMaxOffsetDiffer = 0;
        public static int m_nPTFirstOffsetWeight = 0;
        public static int m_nPTLastOffsetWeight = 0;
        public static int m_nPTPenVersion = 0;
        public static int m_nPTMaxWeightDiffer = 0;
        public static int m_nPTExtraIncWeight_25G = 0;
        public static int m_nPTExtraIncWeight_50G = 0;
        public static int m_nPTExtraIncWeight_75G = 0;
        public static int m_nPTExtraIncWeight_100G = 0;

        //[Other Setting]
        public static int m_nAutoTune_P0_detect_time_Index = 0;
        public static double m_dStartDelayTime = 0.0;
        public static int m_nRecordDataRetryCount = 0;
        public static double m_fDrawLineTimeout = 0.0;
        public static int m_nSetDigiGain = 0;
        public static int m_n5TRawDataType = 1;
        public static int m_nGetCPUType = 1;

        //[PHCK Pattern Setting]
        public static int m_nScreenIndex = 0;
        public static double m_dScreenWidth = 0.0;
        public static double m_dScreenHeight = 0.0;
        public static int m_nLeftColor = 0;
        public static int m_nRightColor = 0;
        public static int m_nGrayLineColor = 0;

        //[Analysis Common Setting]
        //public static int m_nEdgeTraceNumber = 0;
        public static int m_nPartNumber = 20;

        //[Ranking Flow Setting]
        public static int m_nFlowStep = 0;

        //[Ranking Skip Frame Number Setting]
        public static int m_nStartSkipReportCount = 0;
        public static int m_nLastSkipReportCount = 0;

        //[Ranking Report Length Setting]
        public static int m_nNormalReportDataLength = 0;
        public static int m_nRTXTraceNumberByte = 0;
        public static int m_nAutoTuningInfoByte = 0;
        public static int m_nTraceTypeBit = 0;
        public static int m_nDataTypeBit = 0;
        public static int m_nExecuteTypeBit = 0;
        public static int m_nDataSectionByte = 0;

        //[Ranking Compare Setting]
        public static double m_dInnerReferenceValueHB = 0.0;
        public static double m_dEdgeSS_OFF_InnerRXWeightingPercent = 0.0;
        public static double m_dEdgeSS_OFF_InnerTXWeightingPercent = 0.0;
        public static double m_dEdgeSS_OFF_EdgeRXWeightingPercent = 0.0;
        public static double m_dEdgeSS_OFF_EdgeTXWeightingPercent = 0.0;
        public static double m_dMaxMinusMeanValueOverWarningStdevMagHB = -1.0;  //20.0;
        public static double m_dMaxValueOverWarningAbsValueHB = -1.0;

        //[Normal Analysis Setting]
        //public static int m_nSkipDataPartNumber = 0;
        //public static int m_nStraightUsefulDataNumber = 0;
        public static int m_nNormalFilterRXValidReportNumber = 150;
        public static int m_nNormalFilterTXValidReportNumber = 70;
        public static int m_nContactStepFilterType = 0;
        public static int m_nContactStepFilterRXValue = -1;
        public static int m_nContactStepFilterTXValue = -1;

        //[DGT Analysis Setting]
        public static int m_nDGTCompensatePower = 0;
        public static int m_nDGTDigiGainScaleHB = 0;
        public static int m_nDGTDigiGainScaleLB = 0;

        //[TPGT Analysis Setting]
        public static int m_nTPGTTXStartPin = 0;
        public static int m_nTPGTTXEndPin = 47;
        public static int m_nTPGTRXStartPin = 0;
        public static int m_nTPGTRXEndPin = 83;
        public static int m_nTPGTGainRatio = 8192;
        public static int m_nTPGTEdgeProcess = 1;

        //[PCT Analysis Setting]
        public static double m_dPCTPeakCheckRatio = 0.0;
        public static double m_dPCTPeakCheckRatio5T = 0.0;
        public static double m_dPCTPeakCheckRatio3T = 0.0;

        //[DT Analysis Setting]
        public static int m_nDTValidReportEdgeNumber = 4;
        public static int m_nDTDisplayChartDetailValue = 0;

        public static double m_dDTNormalHoverTHRatio_RX = 0.0;
        public static double m_dDTNormalHoverTHRatio_TX = 0.0;
        public static double m_dDTNormalContactTHRatio_RX = 0.0;
        public static double m_dDTNormalContactTHRatio_TX = 0.0;
        public static double m_dDTP0THCompRatio_800us = 0.0;
        public static double m_dDTThresholdRatio_RX = 1.0;
        public static double m_dDTThresholdRatio_TX = 1.0;
        public static int m_nDTSkipCompareThreshold = 0;

        public static int m_nDT7318TRxSSpecificReportType = 0;

        //[TT Analysis Setting]
        public static int m_nTTPolyFitOrder = 4;
        public static int m_nTTValidReportEdgeNumber = 4;
        public static double m_dTTPTHFHoverRatio_RX = 0.0;
        public static double m_dTTPTHFHoverRatio_TX = 0.0;
        public static double m_dTTBHFHoverRatio_RX = 0.0;
        public static double m_dTTBHFHoverRatio_TX = 0.0;

        public static int[] m_nPressureLightessWeight_Array = new int[1] 
        { 
            10 
        };

        public static int[] m_nPressureWeight_Array = new int[m_nPRESSURE_DATA_NUMBER - 1] 
        { 
            25, 
            50, 
            75, 
            100, 
            125, 
            150, 
            175, 
            200, 
            225, 
            250, 
            275, 
            300, 
            325, 
            350 
        };

        public static int[] m_nRealPressureWeight_Array = new int[m_nPressureLightessWeight_Array.Length + m_nPressureWeight_Array.Length];

        //[PT Analysis Setting]
        public static int m_nPTIQ_BSH_P_HB = 16;
        public static int m_nPTIQ_BSH_P_LB = 11;
        public static int m_nPressMaxDFTRxRefValueHB = 0x3000;
        public static int m_nPressMaxDFTRxRefValueLB = 0x1000;

        public static int m_nPTValidReportEdgeNumber = 4;

        //[LT Analysis Setting]
        public static int m_nLTFirstEdgeAreaValidNumber = 3;
        public static int m_nLTLastEdgeAreaValidNumber = 3;
        public static int m_nLTUseTP_GainCompensate = 1;

        //[Select Setting]
        public static int m_nTNFrequencyNumber = 0;
        public static int m_nOtherFrequencyNumber = 0;

        //[Process Setting]
        public static int m_nSendFWParamType = 0;
        public static int m_nProcessType = 0;
        public static int m_nFixedPH1 = 0;
        public static int m_nFixedPH2 = 0;

        //[FW Parameter Setting]
        public static int m_nParameter_cActivePen_FM_P0_TH = -1;

        //Default FWParameter
        //Finger
        //public static int m_nDefault_AP_pPeakThrdshold = 0x7FFF;

        //ActivePen_Edge_Pwr_Proc
        public static int m_nDefault_cActivePen_FM_Detect_Edge_1Trc_SubPwr = 0;
        public static int m_nDefault_cActivePen_FM_Detect_Edge_2Trc_SubPwr = 0;
        public static int m_nDefault_cActivePen_FM_Detect_Edge_3Trc_SubPwr = 0;
        public static int m_nDefault_cActivePen_FM_Detect_Edge_4Trc_SubPwr = 0;

        //Digital Gain
        public static int m_nDefault_cActivePen_DigiGain_P0 = 16384;
        public static int m_nDefault_cActivePen_DigiGain_Beacon_Rx = 16384;
        public static int m_nDefault_cActivePen_DigiGain_Beacon_Tx = 16384;
        public static int m_nDefault_cActivePen_DigiGain_PTHF_Rx = 16384;
        public static int m_nDefault_cActivePen_DigiGain_PTHF_Tx = 16384;
        public static int m_nDefault_cActivePen_DigiGain_BHF_Rx = 16384;
        public static int m_nDefault_cActivePen_DigiGain_BHF_Tx = 16384;

        //Pen Detect THD
        public static int m_nDefault_cActivePen_FM_P0_TH = 0x7FFF;

        //Beacon THD For 800us Scan
        public static int m_nDefault_cActivePen_TRxS_Beacon_Contact_TH_Rx = 0xF800;
        public static int m_nDefault_cActivePen_TRxS_Beacon_Contact_TH_Tx = 0xF800;
        public static int m_nDefault_cActivePen_TRxS_Beacon_Hover_TH_Rx = 0xF800;
        public static int m_nDefault_cActivePen_TRxS_Beacon_Hover_TH_Tx = 0xF800;

        //Beacon THD For 400us Scan
        public static int m_nDefault_cActivePen_Beacon_Contact_TH_Rx = 0xF800;
        public static int m_nDefault_cActivePen_Beacon_Contact_TH_Tx = 0xF800;
        public static int m_nDefault_cActivePen_Beacon_Hover_TH_Rx = 0xF800;
        public static int m_nDefault_cActivePen_Beacon_Hover_TH_Tx = 0xF800;

        //Port Type HF THD
        public static int m_nDefault_cActivePen_PTHF_Contact_TH_Rx = 0x7FFF;
        public static int m_nDefault_cActivePen_PTHF_Contact_TH_Tx = 0x7FFF;
        public static int m_nDefault_cActivePen_PTHF_Hover_TH_Rx = 0x7FFF;
        public static int m_nDefault_cActivePen_PTHF_Hover_TH_Tx = 0x7FFF;

        //Beacon HF THD
        public static int m_nDefault_cActivePen_BHF_Contact_TH_Rx = 0x7FFF;
        public static int m_nDefault_cActivePen_BHF_Contact_TH_Tx = 0x7FFF;
        public static int m_nDefault_cActivePen_BHF_Hover_TH_Rx = 0x7FFF;
        public static int m_nDefault_cActivePen_BHF_Hover_TH_Tx = 0x7FFF;

        //Port Type HI HF and Beacon HI HF THD
        public static int m_nDefault_cActivePen_Pen_HI_HF_THD = 0x7FFF;


        //Gen 8 FW Parameter Relative Address
        public static UInt16 m_nRelativeAddress_PH1_Beacon = 0x0004;
        public static UInt16 m_nRelativeAddress_PH2_Beacon = 0x0005;
        public static UInt16 m_nRelativeAddress_PH3_Beacon = 0x0006;
        public static UInt16 m_nRelativeAddress_MPP_SP_NUM = 0x0000;
        public static UInt16 m_nRelativeAddress_MPP_EFFECT_NUM = 0x0001;
        public static UInt16 m_nRelativeAddress_MPP_DFT_NUM = 0x0002;
        public static UInt16 m_nRelativeAddress_TX_PPI_H = 0x0002;
        public static UInt16 m_nRelativeAddress_TX_PPI_L = 0x0004;
        public static UInt16 m_nRelativeAddress_PEN_MS_BSH_ADC_TP_NUM = 0x000E;
        public static UInt16 m_nRelativeAddress_PEN_MS_EFFECT_FW_SET_COEF_NUM = 0x000F;
        public static UInt16 m_nRelativeAddress_PEN_MS_DFT_NUM_IQ_FIR_CTL = 0x0012;
        public static UInt16 m_nRelativeAddress_PEN_MS_COS_INC_0 = 0x001A;
        public static UInt16 m_nRelativeAddress_PEN_MS_SIN_INC_0 = 0x001F;
        public static UInt16 m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00 = 0x0070;
        public static UInt16 m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02 = 0x0072;
        public static UInt16 m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03 = 0x0073;
        public static UInt16 m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04 = 0x0074;
        public static UInt16 m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00_2 = 0x00E1;
        public static UInt16 m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02_2 = 0x00E3;
        public static UInt16 m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03_2 = 0x00E4;
        public static UInt16 m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04_2 = 0x00E5;
        public static UInt16 m_nRelativeAddress_Pen_MS_ANA_PH_CTL_00 = 0x0094;
        public static UInt16 m_nRelativeAddress_Pen_MS_M_S_CTL = 0x00AA;
        public static UInt16 m_nRelativeAddress_Pen_MS_CKS_CTL = 0x00A3;
        public static UInt16 m_nRelativeAddress_Pen_MS_CT_ADC_SH_LMT = 0x00A6;
        public static UInt16 m_nRelativeAddress_TX_PPI_H_S0 = 0x001F;
        public static UInt16 m_nRelativeAddress_TX_PPI_L_S0 = 0x0021;
        public static UInt16 m_nRelativeAddress_TX_PPI_H_S1 = 0x001F;
        public static UInt16 m_nRelativeAddress_TX_PPI_L_S1 = 0x0021;
        public static UInt16 m_nRelativeAddress_TX_PPI_H_S2 = 0x001F;
        public static UInt16 m_nRelativeAddress_TX_PPI_L_S2 = 0x0021;
        public static UInt16 m_nRelativeAddress_TX_PPI_H_S3 = 0x001F;
        public static UInt16 m_nRelativeAddress_TX_PPI_L_S3 = 0x0021;

        public static void LoadParameter()
        {
            m_sSettingFilePath = m_cfrmMain.m_sIniPath;
            m_sDefaultFilePath = m_cfrmMain.m_sDefaultIniPath;

            JudgeFileType(m_sSettingFilePath);

            GetParameterValue(ref m_nVersionType, "Main Setting", "VersionType", "5");

            GetParameterValue(ref m_bSkipPreviousStepCheck, "LoadData Mode Setting", "SkipPreStepCheck", "0");

            LoadStepSettingParameter();

            GetParameterValue(ref m_nUSBVID, "Connect Setting", "USB_VID", "04F3", true);
            GetParameterValue(ref m_nUSBPID, "Connect Setting", "USB_PID", "0000", true);
            int nDVDD = 2;
            GetParameterValue(ref nDVDD, "Connect Setting", "DVDD", "2");
            SetDVDD(nDVDD);
            int nVIO = 3;
            GetParameterValue(ref nVIO, "Connect Setting", "VIO", "3");
            SetVIO(nVIO);
            GetParameterValue(ref m_nI2CAddress, "Connect Setting", "I2CAddress", "20", true);
            GetParameterValue(ref m_nNormalLength, "Connect Setting", "NormalLength", "3F", true);
            //m_sCOMPort = ReadValue("Connect Setting", "COMPort", "AutoSet");

            GetParameterValue(ref m_nWindowsSPIInterfaceType, "Connect Setting", "WindowsSPIInterfaceType", "0");
            GetParameterValue(ref m_nWindowsSPICommandLength, "Connect Setting", "WindowsSPICommandLength", "10");

            GetParameterValue(ref m_nFlowMethodType, "Flow Method Setting", "FlowMethodType", "0");

            m_sServerIP = ReadValue("Network Setting", "ServerIP", "192.168.0.1");
            m_sClientIP = ReadValue("Network Setting", "ClientIP", "192.168.0.2");
            GetParameterValue(ref m_nPort, "Network Setting", "Port", "8800");

            GetParameterValue(ref m_nServerReadTimeout, "Network Setting", "ServerReadTimeout", "1000");
            GetParameterValue(ref m_nServerWriteTimeout, "Network Setting", "ServerWriteTimeout", "-1");
            GetParameterValue(ref m_nServerWriteDelayTime, "Network Setting", "ServerWriteDelayTime", "-1");
            GetParameterValue(ref m_nClientReadTimeout, "Network Setting", "ClientReadTimeout", "-1");
            GetParameterValue(ref m_nClientWriteTimeout, "Network Setting", "ClientWriteTimeout", "-1");
            GetParameterValue(ref m_nClientSendingDelayTime, "Network Setting", "ClientSendingDelayTime", "200");
            GetParameterValue(ref m_nClientWriteDelayTime, "Network Setting", "ClientWriteDelayTime", "200");

            GetParameterValue(ref m_dStartXAxisCoordinate, "Coordinate Setting", "StartXAxisCoordinate", "0.00");
            GetParameterValue(ref m_dStartYAxisCoordinate, "Coordinate Setting", "StartYAxisCoordinate", "0.00");
            GetParameterValue(ref m_dEndXAxisCoordinate, "Coordinate Setting", "EndXAxisCoordinate", "0.00");
            GetParameterValue(ref m_dEndYAxisCoordinate, "Coordinate Setting", "EndYAxisCoordinate", "0.00");
            GetParameterValue(ref m_dContactZAxisCoordinate, "Coordinate Setting", "ContactZAxisCoordinate", "0.00");
            //GetParameterValue(ref m_dContactPressZAxisCoordinate, "Coordinate Setting", "ContactPressZAxisCoordinate", "0.00");
            GetParameterValue(ref m_dHoverHeight_DT1st, "Coordinate Setting", "HoverHeight_DT1st", "6.00");
            GetParameterValue(ref m_dHoverHeight_DT2nd, "Coordinate Setting", "HoverHeight_DT2nd", "8.00");
            //GetParameterValue(ref m_dHoverHeight_PP, "Coordinate Setting", "HoverHeight_PP", "1.00");
            GetParameterValue(ref m_dHoverHeight_PT, "Coordinate Setting", "HoverHeight_PT", "2.00");
            GetParameterValue(ref m_dHoverHeight_PCT1st, "Coordinate Setting", "HoverHeight_PCT1st", "1.00");
            GetParameterValue(ref m_dHoverHeight_PCT2nd, "Coordinate Setting", "HoverHeight_PCT2nd", "2.00");
            GetParameterValue(ref m_dPushDownZAxisCoordinate_DGT, "Coordinate Setting", "PushDownZAxisCoordinate_DGT", "1.00");
            GetParameterValue(ref m_dRobotMovingTimeout, "Coordinate Setting", "RobotMovingTimeout", "30");
            GetParameterValue(ref m_dLTHorShiftYAxisCoordinate, "Coordinate Setting", "LTHorShiftYAxisCoordinate", "50.00");
            GetParameterValue(ref m_dLTVerShiftXAxisCoordinate, "Coordinate Setting", "LTVerShiftXAxisCoordinate", "50.00");
            GetParameterValue(ref m_nPTInitialWeight, "Coordinate Setting", "PTInitialWeight", "150");
            GetParameterValue(ref m_dPTIni1stZDropScale, "Coordinate Setting", "PTIni1stZDropScale", "0.6000");
            GetParameterValue(ref m_dPTIni2ndZDropScale, "Coordinate Setting", "PTIni2ndZDropScale", "0.1000");
            GetParameterValue(ref m_dPT1stZDropScale, "Coordinate Setting", "PT1stZDropScale", "0.0500");
            GetParameterValue(ref m_dPT2ndZDropScale, "Coordinate Setting", "PT2ndZDropScale", "0.0050");
            GetParameterValue(ref m_nMaxLoadingWeight, "Coordinate Setting", "MaxLoadingWeight", "150");
            GetParameterValue(ref m_dPTFGDetectPowerOnZDropScale, "Coordinate Setting", "PTFGDetectPowerOnZDropScale", "0.5000");
            GetParameterValue(ref m_dPTFGDetectPowerOnMaxWeight, "Coordinate Setting", "PTFGDetectPowerOnMaxWeight", "400");

            GetParameterValue(ref m_dDGTDrawingSpeed, "Speed Setting", "DGTDrawingSpeed", "0.50");
            GetParameterValue(ref m_dTPGTDrawingSpeed, "Speed Setting", "TPGTDrawingSpeed", "0.50");
            GetParameterValue(ref m_dPCTDrawingSpeed, "Speed Setting", "PCTDrawingSpeed", "10.00");
            GetParameterValue(ref m_dDTDrawingSpeed, "Speed Setting", "DTDrawingSpeed", "10.00");
            GetParameterValue(ref m_dTTDrawingSpeed, "Speed Setting", "TTDrawingSpeed", "0.50");
            GetParameterValue(ref m_dTTSlantDrawingSpeed, "Speed Setting", "TTSlantDrawingSpeed", "10.00");
            GetParameterValue(ref m_dLTDrawingSpeed, "Speed Setting", "LTDrawingSpeed", "0.10");

            m_sGoDrawCtrlrCOMPort = ReadValue("GoDraw Controller", "COMPort", "NA");
            GetParameterValue(ref m_dGoDrawCtrlrSpeed, "GoDraw Controller", "Speed", "5.0");
            GetParameterValue(ref m_nGoDrawCtrlrMaxCoordinateX, "GoDraw Controller", "MaxCoordinateX", "310");
            GetParameterValue(ref m_nGoDrawCtrlrMaxCoordinateY, "GoDraw Controller", "MaxCoordinateY", "200");
            GetParameterValue(ref m_nGoDrawCtrlrMaxServoValue, "GoDraw Controller", "MaxServoValue", "30000");
            GetParameterValue(ref m_nGoDrawCtrlrMinServoValue, "GoDraw Controller", "MinServoValue", "3000");
            GetParameterValue(ref m_nGoDrawCtrlrDistance, "GoDraw Controller", "Distance", "10");
            GetParameterValue(ref m_nGoDrawCtrlrTopServoValue, "GoDraw Controller", "TopServoValue", "26000");
            GetParameterValue(ref m_nGoDrawCtrlrHoverServoValue, "GoDraw Controller", "HoverServoValue", "10000");
            GetParameterValue(ref m_nGoDrawCtrlrContactServoValue, "GoDraw Controller", "ContactServoValue", "3000");
            GetParameterValue(ref m_nGoDrawCtrlrDestinationCoordinateX, "GoDraw Controller", "DestinationCoordinateX", "0");
            GetParameterValue(ref m_nGoDrawCtrlrDestinationCoordinateY, "GoDraw Controller", "DestinationCoordinateY", "0");
            GetParameterValue(ref m_nGoDrawCtrlrMoveType, "GoDraw Controller", "MoveType", "0");
            GetParameterValue(ref m_nGoDrawCtrlrEstimateType, "GoDraw Controller", "EstimateType", "1");
            GetParameterValue(ref m_dGoDrawCtrlrMinSpeed, "GoDraw Controller", "MinSpeed", "0.5");
            GetParameterValue(ref m_dGoDrawCtrlrMaxSpeed, "GoDraw Controller", "MaxSpeed", "200.0");
            GetParameterValue(ref m_dGoDrawCtrlrCauseDelayMaxSpeed, "GoDraw Controller", "CauseDelayMaxSpeed", "3.0");
            GetParameterValue(ref m_dGoDrawCtrlrEstimateDelay_Speed1, "GoDraw Controller", "EstimateDelay_Speed1", "0.5");
            GetParameterValue(ref m_dGoDrawCtrlrEstimateDelay_Speed2, "GoDraw Controller", "EstimateDelay_Speed2", "2.0");
            GetParameterValue(ref m_nGoDrawCtrlrEstimateDelay_Distance1, "GoDraw Controller", "EstimateDelay_Distance1", "0");
            GetParameterValue(ref m_nGoDrawCtrlrEstimateDelay_Time1, "GoDraw Controller", "EstimateDelay_Time1", "0");
            GetParameterValue(ref m_nGoDrawCtrlrEstimateDelay_Distance2, "GoDraw Controller", "EstimateDelay_Distance2", "300");
            GetParameterValue(ref m_nGoDrawCtrlrEstimateDelay_Time2_Speed1, "GoDraw Controller", "EstimateDelay_Time2_Speed1", "6530");
            GetParameterValue(ref m_nGoDrawCtrlrEstimateDelay_Time2_Speed2, "GoDraw Controller", "EstimateDelay_Time2_Speed2", "1000");

            GetParameterValue(ref m_nGoDrawStartXAxisCoordinate, "GoDraw Coordinate Setting", "StartXAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawStartYAxisCoordinate, "GoDraw Coordinate Setting", "StartYAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawEndXAxisCoordinate, "GoDraw Coordinate Setting", "EndXAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawEndYAxisCoordinate, "GoDraw Coordinate Setting", "EndYAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawTopZServoValue, "GoDraw Coordinate Setting", "TopZServoValue", "26000");
            GetParameterValue(ref m_nGoDrawContactZServoValue, "GoDraw Coordinate Setting", "ContactZServoValue", "3000");
            GetParameterValue(ref m_nGoDrawHoverZServoValue_DT1st, "GoDraw Coordinate Setting", "HoverZServoValue_DT1st", "9000");
            GetParameterValue(ref m_nGoDrawHoverZServoValue_DT2nd, "GoDraw Coordinate Setting", "HoverZServoValue_DT2nd", "10000");
            GetParameterValue(ref m_nGoDrawHoverZServoValue_PCT1st, "GoDraw Coordinate Setting", "HoverZServoValue_PCT1st", "5000");
            GetParameterValue(ref m_nGoDrawHoverZServoValue_PCT2nd, "GoDraw Coordinate Setting", "HoverZServoValue_PCT2nd", "6000");
            GetParameterValue(ref m_nGoDrawPushDownZServoValue_DGT, "GoDraw Coordinate Setting", "PushDownZServoValue_DGT", "-1000");
            GetParameterValue(ref m_nGoDrawLTHorShiftYAxisCoordinate, "GoDraw Coordinate Setting", "LTHorShiftYAxisCoordinate", "50");
            GetParameterValue(ref m_nGoDrawLTVerShiftXAxisCoordinate, "GoDraw Coordinate Setting", "LTVerShiftXAxisCoordinate", "50");
            GetParameterValue(ref m_nGoDrawDelayTime, "GoDraw Coordinate Setting", "DelayTime", "1000");

            GetParameterValue(ref m_dTPGTHorizontalStartXAxisCoordinate, "TPGT Coordinate Setting", "TPGTHorizontalStartXAxisCoordinate", "0.00");
            GetParameterValue(ref m_dTPGTHorizontalStartYAxisCoordinate, "TPGT Coordinate Setting", "TPGTHorizontalStartYAxisCoordinate", "0.00");
            GetParameterValue(ref m_dTPGTHorizontalEndXAxisCoordinate, "TPGT Coordinate Setting", "TPGTHorizontalEndXAxisCoordinate", "0.00");
            GetParameterValue(ref m_dTPGTHorizontalEndYAxisCoordinate, "TPGT Coordinate Setting", "TPGTHorizontalEndYAxisCoordinate", "0.00");
            GetParameterValue(ref m_dTPGTVerticalStartXAxisCoordinate, "TPGT Coordinate Setting", "TPGTVerticalStartXAxisCoordinate", "0.00");
            GetParameterValue(ref m_dTPGTVerticalStartYAxisCoordinate, "TPGT Coordinate Setting", "TPGTVerticalStartYAxisCoordinate", "0.00");
            GetParameterValue(ref m_dTPGTVerticalEndXAxisCoordinate, "TPGT Coordinate Setting", "TPGTVerticalEndXAxisCoordinate", "0.00");
            GetParameterValue(ref m_dTPGTVerticalEndYAxisCoordinate, "TPGT Coordinate Setting", "TPGTVerticalEndYAxisCoordinate", "0.00");
            GetParameterValue(ref m_dTPGTContactZAxisCoordinate, "TPGT Coordinate Setting", "TPGTContactZAxisCoordinate", "0.00");

            GetParameterValue(ref m_nGoDrawTPGTHorizontalStartXAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTHorizontalStartXAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawTPGTHorizontalStartYAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTHorizontalStartYAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawTPGTHorizontalEndXAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTHorizontalEndXAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawTPGTHorizontalEndYAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTHorizontalEndYAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawTPGTVerticalStartXAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTVerticalStartXAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawTPGTVerticalStartYAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTVerticalStartYAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawTPGTVerticalEndXAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTVerticalEndXAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawTPGTVerticalEndYAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTVerticalEndYAxisCoordinate", "0");
            GetParameterValue(ref m_nGoDrawTPGTContactZServoValue, "TPGT GoDraw Coordinate Setting", "TPGTContactZServoValue", "3000");

            m_sProjectName = ReadValue("Project Information Setting", "ProjectName", "");
            GetParameterValue(ref m_nFWTypeIndex, "Project Information Setting", "FWTypeIndex", "0");

            GetParameterValue(ref m_nFWCheckVersion, "FW Check Setting", "FWCheckVersion", "A1", true);

            GetParameterValue(ref m_nFrequencyLB, "Frequency Boundary Setting", "FrequencyLB", "326");
            GetParameterValue(ref m_nFrequencyLB_MPP180, "Frequency Boundary Setting", "FrequencyLB_MPP180", "510");
            GetParameterValue(ref m_nFrequencyHB, "Frequency Boundary Setting", "FrequencyHB", "762");

            GetParameterValue(ref m_nDisplayType, "Color/Pattern Setting", "DisplayType", "1");
            GetParameterValue(ref m_bDisplayReportNumber, "Color/Pattern Setting", "DisplayReportNumber", "1");
            GetParameterValue(ref m_bDisplayProgressStatus, "Color/Pattern Setting", "DisplayProgressStatus", "1");
            GetParameterValue(ref m_nColorSelectIndex, "Color/Pattern Setting", "ColorSelectIndex", "0");
            GetParameterValue(ref m_nDisplayColor, "Color/Pattern Setting", "DisplayColor", "00000000", true);
            GetParameterValue(ref m_nPatternType, "Color/Pattern Setting", "PatternType", "0");
            m_sManualPatternPath = ReadValue("Color/Pattern Setting", "ManualPatternPath", "");

            GetParameterValue(ref m_nGen8AFEType, "Gen8 Command Setting", "Gen8AFEType", "0");
            GetParameterValue(ref m_nGen8FilterType, "Gen8 Command Setting", "Gen8FilterType", "0");
            GetParameterValue(ref m_nGen8CommandScriptType, "Gen8 Command Setting", "CommandScriptType", "0");
            m_sGen8UserDefinedPath = ReadValue("Gen8 Command Setting", "UserDefinedPath", "");
            GetParameterValue(ref m_nGen8EnableCheckUserDefinedFormat, "Gen8 Command Setting", "EnableCheckUserDefinedFormat", "0");
            GetParameterValue(ref m_nGen8SendCommandDelayTime, "Gen8 Command Setting", "SendCommandDelayTime", "100");
            GetParameterValue(ref m_nGen8GetAnalogAfterSwitchTestMode, "Gen8 Command Setting", "GetAnalogAfterSwitchTestMode", "1");

            GetParameterValue(ref m_nEnableReK, "Command Flow Setting", "EnableReK", "0");

            GetParameterValue(ref m_nGen8TraceType, "Gen8 Setting", "Gen8TraceType", "2");
            GetParameterValue(ref m_nGen8RealTraceNumber, "Gen8 Setting", "Gen8RealTraceNumber", "0");
            GetParameterValue(ref m_nGen8BeaconRowNumber, "Gen8 Setting", "Gen8BeaconRowNumber", "20");
            GetParameterValue(ref m_nGen8BHFRowNumber, "Gen8 Setting", "Gen8BHFRowNumber", "15");
            GetParameterValue(ref m_nGen8PTHFRowNumber, "Gen8 Setting", "Gen8PTHFRowNumber", "15");
            GetParameterValue(ref m_nGen8ReportDataLength, "Gen8 Setting", "Gen8ReportDataLength", "65");
            GetParameterValue(ref m_nGen8SKIP_NUM, "Gen8 Setting", "Gen8SKIP_NUM", "12");
            GetParameterValue(ref m_nGen8ProjectOptionDisableValue, "Gen8 Setting", "Gen8ProjectOptionDisableValue", "0001", true);
            GetParameterValue(ref m_nGen8FWIPOptionDisableValue, "Gen8 Setting", "Gen8FWIPOptionDisableValue", "0000", true);

            GetParameterValue(ref m_nReportDataLength, "Report Data Setting", "ReportDataLength", "62");
            GetParameterValue(ref m_nShiftStartByte, "Report Data Setting", "ShiftStartByte", "0");
            GetParameterValue(ref m_nShiftByteNumber, "Report Data Setting", "ShiftByteNumber", "0");

            GetParameterValue(ref m_nNoiseDataType, "Noise & Tilt Noise Step Setting", "NoiseDataType", "0");
            GetParameterValue(ref m_nNoiseFrameNumber, "Noise & Tilt Noise Step Setting", "NoiseFrameNumber", "1000");
            GetParameterValue(ref m_nNoiseReportNumber, "Noise & Tilt Noise Step Setting", "NoiseReportNumber", "6000");
            GetParameterValue(ref m_nNoiseValidReportNumber, "Noise & Tilt Noise Step Setting", "NoiseValidReportNumber", "5000");
            GetParameterValue(ref m_nNoiseProcessReportNumber, "Noise & Tilt Noise Step Setting", "NoiseProcessReportNumber", "-1");
            GetParameterValue(ref m_dNoiseTimeout, "Noise & Tilt Noise Step Setting", "NoiseTimeout", "240");
            GetParameterValue(ref m_dNoiseNoReportInterruptTime, "Noise & Tilt Noise Step Setting", "NoiseNoReportInterruptTime", "5");
            GetParameterValue(ref m_nNoiseDisplayReportRate, "Noise & Tilt Noise Step Setting", "NoiseDisplayReportRate", "0");
            GetParameterValue(ref m_nNoiseGetDistributionData, "Noise & Tilt Noise Step Setting", "NoiseGetDistributionData", "0");

            GetParameterValue(ref m_nDGTDrawType, "DGT Step Setting", "DGTDrawType", "0");
            /*
            GetParameterValue(ref m_nDGTDefaultScaleDigiGain_P0, "DGT Step Setting", "DGTDefaultScaleDigiGain_P0", "100");
            GetParameterValue(ref m_nDGTDefaultScaleDigiGain_Beacon_Rx, "DGT Step Setting", "DGTDefaultScaleDigiGain_Beacon_Rx", "100");
            GetParameterValue(ref m_nDGTDefaultScaleDigiGain_Beacon_Tx, "DGT Step Setting", "DGTDefaultScaleDigiGain_Beacon_Tx", "100");
            GetParameterValue(ref m_nDGTDefaultScaleDigiGain_PTHF_Rx, "DGT Step Setting", "DGTDefaultScaleDigiGain_PTHF_Rx", "100");
            GetParameterValue(ref m_nDGTDefaultScaleDigiGain_PTHF_Tx, "DGT Step Setting", "DGTDefaultScaleDigiGain_PTHF_Tx", "100");
            GetParameterValue(ref m_nDGTDefaultScaleDigiGain_BHF_Rx, "DGT Step Setting", "DGTDefaultScaleDigiGain_BHF_Rx", "100");
            GetParameterValue(ref m_nDGTDefaultScaleDigiGain_BHF_Tx, "DGT Step Setting", "DGTDefaultScaleDigiGain_BHF_Tx", "100");
            */
            GetParameterValue(ref m_nDGTRXValidReportNumber, "DGT Step Setting", "DGTRXValidReportNumber", "1500");
            GetParameterValue(ref m_nDGTTXValidReportNumber, "DGT Step Setting", "DGTTXValidReportNumber", "1500");
            GetParameterValue(ref m_nDGTMultiplyValue, "DGT Step Setting", "DGTMultiplyValue", "16384");
            GetParameterValue(ref m_nDGTDividValue, "DGT Step Setting", "DGTDividValue", "100");

            GetParameterValue(ref m_nTPGTRXValidReportNumber, "TPGT Step Setting", "TPGTRXValidReportNumber", "1500");
            GetParameterValue(ref m_nTPGTTXValidReportNumber, "TPGT Step Setting", "TPGTTXValidReportNumber", "1500");
            GetParameterValue(ref m_nTPGTVAngle, "TPGT Step Setting", "TPGTVAngle", "45");
            GetParameterValue(ref m_nTPGTHorizontalRAngle, "TPGT Step Setting", "TPGTHorizontalRAngle", "90");
            GetParameterValue(ref m_nTPGTVerticalRAngle, "TPGT Step Setting", "TPGTVerticalRAngle", "0");
            GetParameterValue(ref m_nTPGTDisplayMessage, "TPGT Step Setting", "TPGTDisplayMessage", "1");

            GetParameterValue(ref m_nNormalValidReportNumber, "Normal Step Setting", "NormalValidReportNumber", "150");
            GetParameterValue(ref m_dNormal800to400PwrRatio, "Normal Step Setting", "Normal800to400PwrRatio", "0.5");

            GetParameterValue(ref m_nTRxSRXValidReportNumber, "TRxS Step Setting", "TRxSRXValidReportNumber", "150");
            GetParameterValue(ref m_nTRxSTXValidReportNumber, "TRxS Step Setting", "TRxSTXValidReportNumber", "70");

            GetParameterValue(ref m_nTTValidTipTraceNumber, "TT Step Setting", "TTValidTipTraceNumber", "5");
            GetParameterValue(ref m_nTTRXValidReportNumber, "TT Step Setting", "TTRXValidReportNumber", "150");
            GetParameterValue(ref m_nTTTXValidReportNumber, "TT Step Setting", "TTTXValidReportNumber", "70");

            //GetParameterValue(ref m_nPPValidReportNumber, "PT Step Setting", "PPValidReportNumber", "100");
            GetParameterValue(ref m_nPTValidReportNumber, "PT Step Setting", "PTValidReportNumber", "1");
            GetParameterValue(ref m_nPTStartSkipReportNumber, "PT Step Setting", "PTStartSkipReportNumber", "150");
            GetParameterValue(ref m_nPTEndSkipReportNumber, "PT Step Setting", "PTEndSkipReportNumber", "150");
            //GetParameterValue(ref m_dPPRecordTime, "PT Step Setting", "PPRecordTime", "10.0");
            GetParameterValue(ref m_dPTRecordTime, "PT Step Setting", "PTRecordTime", "10.0");
            GetParameterValue(ref m_nPTMaxOffsetDiffer, "PT Step Setting", "PTMaxOffsetDiffer", "5");
            GetParameterValue(ref m_nPTFirstOffsetWeight, "PT Step Setting", "PTFirstOffsetWeight", "0");
            GetParameterValue(ref m_nPTLastOffsetWeight, "PT Step Setting", "PTLastOffsetWeight", "0");
            GetParameterValue(ref m_nPTPenVersion, "PT Step Setting", "PTPenVersion", "0");
            GetParameterValue(ref m_nPTMaxWeightDiffer, "PT Step Setting", "PTMaxWeightDiffer", "5");
            GetParameterValue(ref m_nPTExtraIncWeight_25G, "PT Step Setting", "PTExtraIncWeight_25G", "9");
            GetParameterValue(ref m_nPTExtraIncWeight_50G, "PT Step Setting", "PTExtraIncWeight_50G", "2");
            GetParameterValue(ref m_nPTExtraIncWeight_75G, "PT Step Setting", "PTExtraIncWeight_75G", "1");
            GetParameterValue(ref m_nPTExtraIncWeight_100G, "PT Step Setting", "PTExtraIncWeight_100G", "0");

            GetParameterValue(ref m_nAutoTune_P0_detect_time_Index, "Other Setting", "AutoTune_P0_detect_time_Index", "0");
            GetParameterValue(ref m_dStartDelayTime, "Other Setting", "StartDelayTime", "3");
            GetParameterValue(ref m_nRecordDataRetryCount, "Other Setting", "RecordDataRetryCount", "3");
            GetParameterValue(ref m_fDrawLineTimeout, "Other Setting", "DrawLineTimeout", "600");
            GetParameterValue(ref m_n5TRawDataType, "Other Setting", "5TRawDataType", "1");
            GetParameterValue(ref m_nGetCPUType, "Other Setting", "GetCPUType", "1");

            GetParameterValue(ref m_nScreenIndex, "PHCK Pattern Setting", "ScreenIndex", "1");
            GetParameterValue(ref m_dScreenWidth, "PHCK Pattern Setting", "ScreenWidth", "294.44");
            GetParameterValue(ref m_dScreenHeight, "PHCK Pattern Setting", "ScreenHeight", "165.62");
            GetParameterValue(ref m_nLeftColor, "PHCK Pattern Setting", "LeftColor", "ff008000", true);
            GetParameterValue(ref m_nRightColor, "PHCK Pattern Setting", "RightColor", "ffffff00", true);
            GetParameterValue(ref m_nGrayLineColor, "PHCK Pattern Setting", "GrayLineColor", "ff909090", true);

            GetParameterValue(ref m_nFlowStep, "Ranking Flow Setting", SpecificText.m_sFlowStep, "1");

            //GetParameterValue(ref m_nEdgeTraceNumber, "Analysis Common Setting", "EdgeTraceNumber", "4");
            GetParameterValue(ref m_nPartNumber, "Analysis Common Setting", "PartNumber", "20");

            GetParameterValue(ref m_nStartSkipReportCount, "Ranking Skip Frame Number Setting", "StartSkipReportCount", "20");
            GetParameterValue(ref m_nLastSkipReportCount, "Ranking Skip Frame Number Setting", "LastSkipReportCount", "20");

            GetParameterValue(ref m_nNormalReportDataLength, "Ranking Report Length Setting", "NormalReportDataLegth", "14");
            GetParameterValue(ref m_nRTXTraceNumberByte, "Ranking Report Length Setting", "RTXTraceNumberByte", "10");
            GetParameterValue(ref m_nAutoTuningInfoByte, "Ranking Report Length Setting", "AutoTuningInfoByte", "13");
            GetParameterValue(ref m_nTraceTypeBit, "Ranking Report Length Setting", "TraceTypeBit", "1");
            GetParameterValue(ref m_nDataTypeBit, "Ranking Report Length Setting", "DataTypeBit", "2");
            GetParameterValue(ref m_nExecuteTypeBit, "Ranking Report Length Setting", "ExecuteTypeBit", "3");
            GetParameterValue(ref m_nDataSectionByte, "Ranking Report Length Setting", "DataSectionByte", "14");

            GetParameterValue(ref m_dInnerReferenceValueHB, "Ranking Compare Setting", "InnerReferenceValueHB", "1000");
            GetParameterValue(ref m_dEdgeSS_OFF_InnerRXWeightingPercent, "Ranking Compare Setting", "EdgeSS_OFF_InnerRXWeightingPercent", "30");
            GetParameterValue(ref m_dEdgeSS_OFF_InnerTXWeightingPercent, "Ranking Compare Setting", "EdgeSS_OFF_InnerTXWeightingPercent", "30");
            GetParameterValue(ref m_dEdgeSS_OFF_EdgeRXWeightingPercent, "Ranking Compare Setting", "EdgeSS_OFF_EdgeRXWeightingPercent", "20");
            GetParameterValue(ref m_dEdgeSS_OFF_EdgeTXWeightingPercent, "Ranking Compare Setting", "EdgeSS_OFF_EdgeTXWeightingPercent", "20");
            GetParameterValue(ref m_dMaxMinusMeanValueOverWarningStdevMagHB, "Ranking Compare Setting", "MaxMinusMeanValueOverWarningStdevMagHB", "-1");
            GetParameterValue(ref m_dMaxValueOverWarningAbsValueHB, "Ranking Compare Setting", "MaxValueOverWarningAbsValueHB", "-1");

            //GetParameterValue(ref m_nSkipDataPartNumber, "Normal Analysis Setting", "SkipDataPartNumber", "3");
            //GetParameterValue(ref m_nStraightUsefulDataNumber, "Normal Analysis Setting", "StraightUsefulDataNumber", "12");
            GetParameterValue(ref m_nNormalFilterRXValidReportNumber, "Normal Analysis Setting", "NormalFilterRXValidReportNumber", "150");
            GetParameterValue(ref m_nNormalFilterTXValidReportNumber, "Normal Analysis Setting", "NormalFilterTXValidReportNumber", "70");
            GetParameterValue(ref m_nContactStepFilterType, "Normal Analysis Setting", "ContactStepFilterType", "3");
            GetParameterValue(ref m_nContactStepFilterRXValue, "Normal Analysis Setting", "ContactStepFilterRXValue", "-1");
            GetParameterValue(ref m_nContactStepFilterTXValue, "Normal Analysis Setting", "ContactStepFilterTXValue", "-1");

            GetParameterValue(ref m_nDGTCompensatePower, "DGT Analysis Setting", "DGTCompensatePower", "24576");
            GetParameterValue(ref m_nDGTDigiGainScaleHB, "DGT Analysis Setting", "DGTDigiGainScaleHB", "395");
            GetParameterValue(ref m_nDGTDigiGainScaleLB, "DGT Analysis Setting", "DGTDigiGainScaleLB", "5");

            GetParameterValue(ref m_nTPGTTXStartPin, "TPGT Analysis Setting", "TPGTTXStartPin", "0");
            GetParameterValue(ref m_nTPGTTXEndPin, "TPGT Analysis Setting", "TPGTTXEndPin", "47");
            GetParameterValue(ref m_nTPGTRXStartPin, "TPGT Analysis Setting", "TPGTRXStartPin", "0");
            GetParameterValue(ref m_nTPGTRXEndPin, "TPGT Analysis Setting", "TPGTRXEndPin", "83");
            GetParameterValue(ref m_nTPGTGainRatio, "TPGT Analysis Setting", "TPGTGainRatio", "8192");
            GetParameterValue(ref m_nTPGTEdgeProcess, "TPGT Analysis Setting", "TPGTEdgeProcess", "1");

            GetParameterValue(ref m_dPCTPeakCheckRatio, "PCT Analysis Setting", "PCTPeakCheckRatio", "1.1");
            GetParameterValue(ref m_dPCTPeakCheckRatio5T, "PCT Analysis Setting", "PCTPeakCheckRatio5T", "0.9");
            GetParameterValue(ref m_dPCTPeakCheckRatio3T, "PCT Analysis Setting", "PCTPeakCheckRatio3T", "0.4");

            GetParameterValue(ref m_nDTValidReportEdgeNumber, "DT Analysis Setting", "DTValidReportEdgeNumber", "4");
            GetParameterValue(ref m_nDTDisplayChartDetailValue, "DT Analysis Setting", "DTDisplayChartDetailValue", "0");

            GetParameterValue(ref m_dDTNormalHoverTHRatio_RX, "DT Analysis Setting", "DTNormalHoverTHRatio_RX", "0.38");
            GetParameterValue(ref m_dDTNormalHoverTHRatio_TX, "DT Analysis Setting", "DTNormalHoverTHRatio_TX", "0.38");
            GetParameterValue(ref m_dDTNormalContactTHRatio_RX, "DT Analysis Setting", "DTNormalContactTHRatio_RX", "0.3");
            GetParameterValue(ref m_dDTNormalContactTHRatio_TX, "DT Analysis Setting", "DTNormalContactTHRatio_TX", "0.3");
            GetParameterValue(ref m_dDTP0THCompRatio_800us, "DT Analysis Setting", "DTP0THCompRatio_800us", "0.8");
            GetParameterValue(ref m_dDTThresholdRatio_RX, "DT Analysis Setting", "DTThresholdRatio_RX", "1.0");
            GetParameterValue(ref m_dDTThresholdRatio_TX, "DT Analysis Setting", "DTThresholdRatio_TX", "1.0");
            GetParameterValue(ref m_nDTSkipCompareThreshold, "DT Analysis Setting", "DTSkipCompareThreshold", "0");

            GetParameterValue(ref m_nDT7318TRxSSpecificReportType, "DT Analysis Setting", "DT7318TRxSSpecificReportType", "0");

            GetParameterValue(ref m_nTTPolyFitOrder, "TT Analysis Setting", "TTPolyFitOrder", "4");
            GetParameterValue(ref m_nTTValidReportEdgeNumber, "TT Analysis Setting", "TTValidReportEdgeNumber", "4");
            GetParameterValue(ref m_dTTPTHFHoverRatio_RX, "TT Analysis Setting", "TTPTHFHoverRatio_RX", "0.6");
            GetParameterValue(ref m_dTTPTHFHoverRatio_TX, "TT Analysis Setting", "TTPTHFHoverRatio_TX", "0.6");
            GetParameterValue(ref m_dTTBHFHoverRatio_RX, "TT Analysis Setting", "TTBHFHoverRatio_RX", "0.6");
            GetParameterValue(ref m_dTTBHFHoverRatio_TX, "TT Analysis Setting", "TTBHFHoverRatio_TX", "0.6");

            //GetParameterValue(ref m_nPTValidReportEdgeNumber, "PT Analysis Setting", "PTValidReportEdgeNumber", "4");
            GetParameterValue(ref m_nPTIQ_BSH_P_HB, "PT Analysis Setting", "PTIQ_BSH_P_HB", "16");
            GetParameterValue(ref m_nPTIQ_BSH_P_LB, "PT Analysis Setting", "PTIQ_BSH_P_LB", "11");
            GetParameterValue(ref m_nPressMaxDFTRxRefValueHB, "PT Analysis Setting", "PressMaxDFTRxRefValueHB", "0x3000", true);
            GetParameterValue(ref m_nPressMaxDFTRxRefValueLB, "PT Analysis Setting", "PressMaxDFTRxRefValueLB", "0x1000", true);

            GetParameterValue(ref m_nLTFirstEdgeAreaValidNumber, "LT Analysis Setting", "LTFirstEdgeAreaValidNumber", "3");
            GetParameterValue(ref m_nLTLastEdgeAreaValidNumber, "LT Analysis Setting", "LTLastEdgeAreaValidNumber", "3");
            GetParameterValue(ref m_nLTUseTP_GainCompensate, "LT Analysis Setting", "LTUseTP_GainCompensate", "1");

            GetParameterValue(ref m_nTNFrequencyNumber, "Select Setting", "TNFrequencyNumber", "-1");
            GetParameterValue(ref m_nOtherFrequencyNumber, "Select Setting", "OtherFrequencyNumber", "5");

            GetParameterValue(ref m_nSetDigiGain, "Other Setting", "SetDigiGain", "1");

            GetParameterValue(ref m_nSendFWParamType, "Process Setting", "SendFWParamType", "0");
            GetParameterValue(ref m_nProcessType, "Process Setting", "ProcessType", "0");
            GetParameterValue(ref m_nFixedPH1, "Process Setting", "FixedPH1", "0");
            GetParameterValue(ref m_nFixedPH2, "Process Setting", "FixedPH2", "0");

            GetParameterValue(ref m_nParameter_cActivePen_FM_P0_TH, "FW Parameter Setting", SpecificText.m_scActivePen_FM_P0_TH, "-1");

            m_nPressureLightessWeight_Array.CopyTo(m_nRealPressureWeight_Array, 0);
            m_nPressureWeight_Array.CopyTo(m_nRealPressureWeight_Array, m_nPressureLightessWeight_Array.Length);
        }

        public static void LoadDefaultFWParameter()
        {
            //m_sPath = m_cfrmMain.m_sDefaultFWParameterIniPath;

            m_sSettingFilePath = m_cfrmMain.m_sDefaultFWParameterIniPath;

            JudgeFileType(m_sSettingFilePath);

            //GetParameterValueByMathCalculate(ref m_nDefault_AP_pPeakThrdshold, "Finger", "_AP_pPeakThrdshold", "0x7FFF");

            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_FM_Detect_Edge_1Trc_SubPwr, "ActivePen_Edge_Pwr_Proc", SpecificText.m_scActivePen_FM_Detect_Edge_1Trc_SubPwr, "0");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_FM_Detect_Edge_2Trc_SubPwr, "ActivePen_Edge_Pwr_Proc", SpecificText.m_scActivePen_FM_Detect_Edge_2Trc_SubPwr, "0");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_FM_Detect_Edge_3Trc_SubPwr, "ActivePen_Edge_Pwr_Proc", SpecificText.m_scActivePen_FM_Detect_Edge_3Trc_SubPwr, "0");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_FM_Detect_Edge_4Trc_SubPwr, "ActivePen_Edge_Pwr_Proc", SpecificText.m_scActivePen_FM_Detect_Edge_4Trc_SubPwr, "0");

            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_DigiGain_P0, "Digital Gain", SpecificText.m_scActivePen_DigiGain_P0, "100*16384/100");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_DigiGain_Beacon_Rx, "Digital Gain", SpecificText.m_scActivePen_DigiGain_Beacon_Rx, "100*16384/100");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_DigiGain_Beacon_Tx, "Digital Gain", SpecificText.m_scActivePen_DigiGain_Beacon_Tx, "100*16384/100");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_DigiGain_PTHF_Rx, "Digital Gain", SpecificText.m_scActivePen_DigiGain_PTHF_Rx, "100*16384/100");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_DigiGain_PTHF_Tx, "Digital Gain", SpecificText.m_scActivePen_DigiGain_PTHF_Tx, "100*16384/100");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_DigiGain_BHF_Rx, "Digital Gain", SpecificText.m_scActivePen_DigiGain_BHF_Rx, "100*16384/100");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_DigiGain_BHF_Tx, "Digital Gain", SpecificText.m_scActivePen_DigiGain_BHF_Tx, "100*16384/100");

            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_FM_P0_TH, "Pen Detect THD", SpecificText.m_scActivePen_FM_P0_TH, "0x7FFF");

            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_TRxS_Beacon_Contact_TH_Rx, "Beacon THD For 800us Scan", SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Rx, "0xF800");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_TRxS_Beacon_Contact_TH_Tx, "Beacon THD For 800us Scan", SpecificText.m_scActivePen_TRxS_Beacon_Contact_TH_Tx, "0xF800");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_TRxS_Beacon_Hover_TH_Rx, "Beacon THD For 800us Scan", SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Rx, "0xF800");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_TRxS_Beacon_Hover_TH_Tx, "Beacon THD For 800us Scan", SpecificText.m_scActivePen_TRxS_Beacon_Hover_TH_Tx, "0xF800");

            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_Beacon_Contact_TH_Rx, "Beacon THD For 400us Scan", SpecificText.m_scActivePen_Beacon_Contact_TH_Rx, "0xF800");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_Beacon_Contact_TH_Tx, "Beacon THD For 400us Scan", SpecificText.m_scActivePen_Beacon_Contact_TH_Tx, "0xF800");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_Beacon_Hover_TH_Rx, "Beacon THD For 400us Scan", SpecificText.m_scActivePen_Beacon_Hover_TH_Rx, "0xF800");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_Beacon_Hover_TH_Tx, "Beacon THD For 400us Scan", SpecificText.m_scActivePen_Beacon_Hover_TH_Tx, "0xF800");

            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_PTHF_Contact_TH_Rx, "Port Type HF THD", SpecificText.m_scActivePen_PTHF_Contact_TH_Rx, "0x7FFF");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_PTHF_Contact_TH_Tx, "Port Type HF THD", SpecificText.m_scActivePen_PTHF_Contact_TH_Tx, "0x7FFF");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_PTHF_Hover_TH_Rx, "Port Type HF THD", SpecificText.m_scActivePen_PTHF_Hover_TH_Rx, "0x7FFF");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_PTHF_Hover_TH_Tx, "Port Type HF THD", SpecificText.m_scActivePen_PTHF_Hover_TH_Tx, "0x7FFF");

            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_BHF_Contact_TH_Rx, "Beacon HF THD", SpecificText.m_scActivePen_BHF_Contact_TH_Rx, "0x7FFF");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_BHF_Contact_TH_Tx, "Beacon HF THD", SpecificText.m_scActivePen_BHF_Contact_TH_Tx, "0x7FFF");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_BHF_Hover_TH_Rx, "Beacon HF THD", SpecificText.m_scActivePen_BHF_Hover_TH_Rx, "0x7FFF");
            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_BHF_Hover_TH_Tx, "Beacon HF THD", SpecificText.m_scActivePen_BHF_Hover_TH_Tx, "0x7FFF");

            GetParameterValueByMathCalculate(ref m_nDefault_cActivePen_Pen_HI_HF_THD, "Port Type HI HF and Beacon HI HF THD", "cActivePen_Pen_HI_HF_THD", "0x7FFF");
        }

        public static void LoadSpecificParameter(string sType, string sParameter)
        {
            m_sSettingFilePath = m_cfrmMain.m_sIniPath;

            JudgeFileType(m_sSettingFilePath);

            if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_GENERAL)
            {
                if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_GENERAL)
                {
                    m_sGoDrawCtrlrCOMPort = ReadValue("GoDraw Controller", "COMPort", "NA");
                    GetParameterValue(ref m_dGoDrawCtrlrSpeed, "GoDraw Controller", "Speed", "5.0");
                    GetParameterValue(ref m_nGoDrawCtrlrMaxCoordinateX, "GoDraw Controller", "MaxCoordinateX", "310");
                    GetParameterValue(ref m_nGoDrawCtrlrMaxCoordinateY, "GoDraw Controller", "MaxCoordinateY", "200");
                }
            }
            else if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_NORMALXYAXIS)
            {
                if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_START)
                {
                    GetParameterValue(ref m_nGoDrawStartXAxisCoordinate, "GoDraw Coordinate Setting", "StartXAxisCoordinate", "0");
                    GetParameterValue(ref m_nGoDrawStartYAxisCoordinate, "GoDraw Coordinate Setting", "StartYAxisCoordinate", "0");
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_END)
                {
                    GetParameterValue(ref m_nGoDrawEndXAxisCoordinate, "GoDraw Coordinate Setting", "EndXAxisCoordinate", "0");
                    GetParameterValue(ref m_nGoDrawEndYAxisCoordinate, "GoDraw Coordinate Setting", "EndYAxisCoordinate", "0");
                }
            }
            else if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_NORMALZAXIS)
            {
                if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_TOP)
                {
                    GetParameterValue(ref m_nGoDrawTopZServoValue, "GoDraw Coordinate Setting", "TopZServoValue", "26000");
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_CONTACT)
                {
                    GetParameterValue(ref m_nGoDrawContactZServoValue, "GoDraw Coordinate Setting", "ContactZServoValue", "3000");
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_DT_HOVER1ST)
                {
                    GetParameterValue(ref m_nGoDrawHoverZServoValue_DT1st, "GoDraw Coordinate Setting", "HoverZServoValue_DT1st", "9000");
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_DT_HOVER2ND)
                {
                    GetParameterValue(ref m_nGoDrawHoverZServoValue_DT2nd, "GoDraw Coordinate Setting", "HoverZServoValue_DT2nd", "10000");
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_PCT_HOVER1ST)
                {
                    GetParameterValue(ref m_nGoDrawHoverZServoValue_PCT1st, "GoDraw Coordinate Setting", "HoverZServoValue_PCT1st", "5000");
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_PCT_HOVER2ND)
                {
                    GetParameterValue(ref m_nGoDrawHoverZServoValue_PCT2nd, "GoDraw Coordinate Setting", "HoverZServoValue_PCT2nd", "6000");
                }
            }
            else if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_TPGTXYAXIS)
            {
                if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_HORIZONTALSTART)
                {
                    GetParameterValue(ref m_nGoDrawTPGTHorizontalStartXAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTHorizontalStartXAxisCoordinate", "0");
                    GetParameterValue(ref m_nGoDrawTPGTHorizontalStartYAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTHorizontalStartYAxisCoordinate", "0");
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_HORIZONTALEND)
                {
                    GetParameterValue(ref m_nGoDrawTPGTHorizontalEndXAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTHorizontalEndXAxisCoordinate", "0");
                    GetParameterValue(ref m_nGoDrawTPGTHorizontalEndYAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTHorizontalEndYAxisCoordinate", "0");
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_VERTICALSTART)
                {
                    GetParameterValue(ref m_nGoDrawTPGTVerticalStartXAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTVerticalStartXAxisCoordinate", "0");
                    GetParameterValue(ref m_nGoDrawTPGTVerticalStartYAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTVerticalStartYAxisCoordinate", "0");
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_VERTICALEND)
                {
                    GetParameterValue(ref m_nGoDrawTPGTVerticalEndXAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTVerticalEndXAxisCoordinate", "0");
                    GetParameterValue(ref m_nGoDrawTPGTVerticalEndYAxisCoordinate, "TPGT GoDraw Coordinate Setting", "TPGTVerticalEndYAxisCoordinate", "0");
                }
            }
            else if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_TPGTZAXIS)
            {
                if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_CONTACT)
                {
                    GetParameterValue(ref m_nGoDrawTPGTContactZServoValue, "TPGT GoDraw Coordinate Setting", "TPGTContactZServoValue", "3000");
                }
            }
        }

        public static void SaveParameter()
        {
            m_sSettingFilePath = m_cfrmMain.m_sIniPath;
            m_sDefaultFilePath = m_cfrmMain.m_sDefaultIniPath;

            JudgeFileType(m_sSettingFilePath);

            WriteValue("Main Setting", "VersionType", m_nVersionType.ToString("D"));

            WriteValue("LoadData Mode Setting", "SkipPreStepCheck", m_bSkipPreviousStepCheck);

            WriteValue("Connect Setting", "USB_VID", m_nUSBVID.ToString("X4"));
            WriteValue("Connect Setting", "USB_PID", m_nUSBPID.ToString("X4"));

            if (m_nDVDD == 28)
                WriteValue("Connect Setting", "DVDD", "0");
            else if (m_nDVDD == 30)
                WriteValue("Connect Setting", "DVDD", "1");
            else if (m_nDVDD == 33)
                WriteValue("Connect Setting", "DVDD", "2");
            else if (m_nDVDD == 50)
                WriteValue("Connect Setting", "DVDD", "3");

            if (m_nVIO == 18)
                WriteValue("Connect Setting", "VIO", "0");
            else if (m_nVIO == 28)
                WriteValue("Connect Setting", "VIO", "1");
            else if (m_nVIO == 30)
                WriteValue("Connect Setting", "VIO", "2");
            else if (m_nVIO == 33)
                WriteValue("Connect Setting", "VIO", "3");
            else if (m_nVIO == 50)
                WriteValue("Connect Setting", "VIO", "4");

            WriteValue("Connect Setting", "I2CAddress", m_nI2CAddress.ToString("X2"));
            WriteValue("Connect Setting", "NormalLength", m_nNormalLength.ToString("X2"));
            //WriteValue("Connect Setting", "COMPort", m_sCOMPort);

            WriteValue("Flow Method Setting", "FlowMethodType", m_nFlowMethodType.ToString("D"));

            WriteValue("Network Setting", "ServerIP", m_sServerIP);
            WriteValue("Network Setting", "ClientIP", m_sClientIP);
            WriteValue("Network Setting", "Port", m_nPort.ToString("D"));

            WriteValue("Network Setting", "ServerReadTimeout", m_nServerReadTimeout.ToString("D"), false);
            WriteValue("Network Setting", "ServerWriteTimeout", m_nServerWriteTimeout.ToString("D"), false);
            WriteValue("Network Setting", "ServerDelayTime", m_nServerWriteDelayTime.ToString("D"), false);
            WriteValue("Network Setting", "ClientReadTimeout", m_nClientReadTimeout.ToString("D"), false);
            WriteValue("Network Setting", "ClientWriteTimeout", m_nClientWriteTimeout.ToString("D"), false);
            WriteValue("Network Setting", "ClientSendingDelayTime", m_nClientSendingDelayTime.ToString("D"), false);
            WriteValue("Network Setting", "ClientDelayTime", m_nClientWriteDelayTime.ToString("D"), false);

            WriteValue("Coordinate Setting", "StartXAxisCoordinate", m_dStartXAxisCoordinate.ToString("F2"));
            WriteValue("Coordinate Setting", "StartYAxisCoordinate", m_dStartYAxisCoordinate.ToString("F2"));
            WriteValue("Coordinate Setting", "EndXAxisCoordinate", m_dEndXAxisCoordinate.ToString("F2"));
            WriteValue("Coordinate Setting", "EndYAxisCoordinate", m_dEndYAxisCoordinate.ToString("F2"));
            WriteValue("Coordinate Setting", "ContactZAxisCoordinate", m_dContactZAxisCoordinate.ToString("F2"));
            //WriteValue("Coordinate Setting", "ContactPressZAxisCoordinate", m_dContactPressZAxisCoordinate.ToString("F2"), false);
            WriteValue("Coordinate Setting", "HoverHeight_DT1st", m_dHoverHeight_DT1st.ToString("F2"));
            WriteValue("Coordinate Setting", "HoverHeight_DT2nd", m_dHoverHeight_DT2nd.ToString("F2"));
            //WriteValue("Coordinate Setting", "HoverHeight_PP", m_dHoverHeight_PP.ToString("F2"));
            WriteValue("Coordinate Setting", "HoverHeight_PT", m_dHoverHeight_PT.ToString("F2"));
            WriteValue("Coordinate Setting", "HoverHeight_PCT1st", m_dHoverHeight_PCT1st.ToString("F2"));
            WriteValue("Coordinate Setting", "HoverHeight_PCT2nd", m_dHoverHeight_PCT2nd.ToString("F2"));
            WriteValue("Coordinate Setting", "PushDownZAxisCoordinate_DGT", m_dPushDownZAxisCoordinate_DGT.ToString("F2"));
            WriteValue("Coordinate Setting", "RobotMovingTimeout", m_dRobotMovingTimeout.ToString());
            WriteValue("Coordinate Setting", "LTHorShiftYAxisCoordinate", m_dLTHorShiftYAxisCoordinate.ToString("F2"));
            WriteValue("Coordinate Setting", "LTVerShiftXAxisCoordinate", m_dLTVerShiftXAxisCoordinate.ToString("F2"));
            WriteValue("Coordinate Setting", "PTInitialWeight", m_nPTInitialWeight.ToString("D"), false);
            WriteValue("Coordinate Setting", "PTIni1stZDropScale", m_dPTIni1stZDropScale.ToString("F4"), false);
            WriteValue("Coordinate Setting", "PTIni2ndZDropScale", m_dPTIni2ndZDropScale.ToString("F4"), false);
            WriteValue("Coordinate Setting", "PT1stZDropScale", m_dPT1stZDropScale.ToString("F4"), false);
            WriteValue("Coordinate Setting", "PT2ndZDropScale", m_dPT2ndZDropScale.ToString("F4"), false);
            WriteValue("Coordinate Setting", "MaxLoadingWeight", m_nMaxLoadingWeight.ToString("D"), false);
            WriteValue("Coordinate Setting", "PTFGDetectPowerOnZDropScale", m_dPTFGDetectPowerOnZDropScale.ToString("F4"), false);
            WriteValue("Coordinate Setting", "PTFGDetectPowerOnMaxWeight", m_dPTFGDetectPowerOnMaxWeight.ToString("D"), false);

            WriteValue("Speed Setting", "DGTDrawingSpeed", m_dDGTDrawingSpeed.ToString("F1"));
            WriteValue("Speed Setting", "TPGTDrawingSpeed", m_dTPGTDrawingSpeed.ToString("F1"));
            WriteValue("Speed Setting", "PCTDrawingSpeed", m_dPCTDrawingSpeed.ToString("F1"));
            WriteValue("Speed Setting", "DTDrawingSpeed", m_dDTDrawingSpeed.ToString("F1"));
            WriteValue("Speed Setting", "TTDrawingSpeed", m_dTTDrawingSpeed.ToString("F1"));
            WriteValue("Speed Setting", "TTSlantDrawingSpeed", m_dTTSlantDrawingSpeed.ToString("F1"));
            WriteValue("Speed Setting", "LTDrawingSpeed", m_dLTDrawingSpeed.ToString("F1"));

            WriteValue("GoDraw Controller", "COMPort", m_sGoDrawCtrlrCOMPort);
            WriteValue("GoDraw Controller", "Speed", m_dGoDrawCtrlrSpeed.ToString("F1"));
            WriteValue("GoDraw Controller", "MaxCoordinateX", m_nGoDrawCtrlrMaxCoordinateX.ToString("D"));
            WriteValue("GoDraw Controller", "MaxCoordinateY", m_nGoDrawCtrlrMaxCoordinateY.ToString("D"));
            WriteValue("GoDraw Controller", "MaxServoValue", m_nGoDrawCtrlrMaxServoValue.ToString("D"));
            WriteValue("GoDraw Controller", "MinServoValue", m_nGoDrawCtrlrMinServoValue.ToString("D"));
            WriteValue("GoDraw Controller", "Distance", m_nGoDrawCtrlrDistance.ToString("D"));
            WriteValue("GoDraw Controller", "TopServoValue", m_nGoDrawCtrlrTopServoValue.ToString("D"));
            WriteValue("GoDraw Controller", "HoverServoValue", m_nGoDrawCtrlrHoverServoValue.ToString("D"));
            WriteValue("GoDraw Controller", "ContactServoValue", m_nGoDrawCtrlrContactServoValue.ToString("D"));
            WriteValue("GoDraw Controller", "DestinationCoordinateX", m_nGoDrawCtrlrDestinationCoordinateX.ToString("D"));
            WriteValue("GoDraw Controller", "DestinationCoordinateY", m_nGoDrawCtrlrDestinationCoordinateY.ToString("D"));
            WriteValue("GoDraw Controller", "MoveType", m_nGoDrawCtrlrMoveType.ToString("D"));
            WriteValue("GoDraw Controller", "EstimateType", m_nGoDrawCtrlrEstimateType.ToString("D"));
            WriteValue("GoDraw Controller", "MinSpeed", m_dGoDrawCtrlrMinSpeed.ToString("F1"));
            WriteValue("GoDraw Controller", "MaxSpeed", m_dGoDrawCtrlrMaxSpeed.ToString("F1"));
            WriteValue("GoDraw Controller", "CauseDelayMaxSpeed", m_dGoDrawCtrlrCauseDelayMaxSpeed.ToString("F1"));
            WriteValue("GoDraw Controller", "EstimateDelay_Speed1", m_dGoDrawCtrlrEstimateDelay_Speed1.ToString("F1"));
            WriteValue("GoDraw Controller", "EstimateDelay_Speed2", m_dGoDrawCtrlrEstimateDelay_Speed2.ToString("F1"));
            WriteValue("GoDraw Controller", "EstimateDelay_Distance1", m_nGoDrawCtrlrEstimateDelay_Distance1.ToString("D"));
            WriteValue("GoDraw Controller", "EstimateDelay_Time1", m_nGoDrawCtrlrEstimateDelay_Time1.ToString("D"));
            WriteValue("GoDraw Controller", "EstimateDelay_Distance2", m_nGoDrawCtrlrEstimateDelay_Distance2.ToString("D"));
            WriteValue("GoDraw Controller", "EstimateDelay_Time2_Speed1", m_nGoDrawCtrlrEstimateDelay_Time2_Speed1.ToString("D"));
            WriteValue("GoDraw Controller", "EstimateDelay_Time2_Speed2", m_nGoDrawCtrlrEstimateDelay_Time2_Speed2.ToString("D"));

            WriteValue("GoDraw Coordinate Setting", "StartXAxisCoordinate", m_nGoDrawStartXAxisCoordinate.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "StartYAxisCoordinate", m_nGoDrawStartYAxisCoordinate.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "EndXAxisCoordinate", m_nGoDrawEndXAxisCoordinate.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "EndYAxisCoordinate", m_nGoDrawEndYAxisCoordinate.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "TopZServoValue", m_nGoDrawTopZServoValue.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "ContactZServoValue", m_nGoDrawContactZServoValue.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "HoverZServoValue_DT1st", m_nGoDrawHoverZServoValue_DT1st.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "HoverZServoValue_DT2nd", m_nGoDrawHoverZServoValue_DT2nd.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "HoverZServoValue_PCT1st", m_nGoDrawHoverZServoValue_PCT1st.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "HoverZServoValue_PCT2nd", m_nGoDrawHoverZServoValue_PCT2nd.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "PushDownZServoValue_DGT", m_nGoDrawPushDownZServoValue_DGT.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "LTHorShiftYAxisCoordinate", m_nGoDrawLTHorShiftYAxisCoordinate.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "LTVerShiftXAxisCoordinate", m_nGoDrawLTVerShiftXAxisCoordinate.ToString("D"));
            WriteValue("GoDraw Coordinate Setting", "DelayTime", m_nGoDrawDelayTime.ToString("D"));

            WriteValue("TPGT Coordinate Setting", "TPGTHorizontalStartXAxisCoordinate", m_dTPGTHorizontalStartXAxisCoordinate.ToString("F2"));
            WriteValue("TPGT Coordinate Setting", "TPGTHorizontalStartYAxisCoordinate", m_dTPGTHorizontalStartYAxisCoordinate.ToString("F2"));
            WriteValue("TPGT Coordinate Setting", "TPGTHorizontalEndXAxisCoordinate", m_dTPGTHorizontalEndXAxisCoordinate.ToString("F2"));
            WriteValue("TPGT Coordinate Setting", "TPGTHorizontalEndYAxisCoordinate", m_dTPGTHorizontalEndYAxisCoordinate.ToString("F2"));
            WriteValue("TPGT Coordinate Setting", "TPGTVerticalStartXAxisCoordinate", m_dTPGTVerticalStartXAxisCoordinate.ToString("F2"));
            WriteValue("TPGT Coordinate Setting", "TPGTVerticalStartYAxisCoordinate", m_dTPGTVerticalStartYAxisCoordinate.ToString("F2"));
            WriteValue("TPGT Coordinate Setting", "TPGTVerticalEndXAxisCoordinate", m_dTPGTVerticalEndXAxisCoordinate.ToString("F2"));
            WriteValue("TPGT Coordinate Setting", "TPGTVerticalEndYAxisCoordinate", m_dTPGTVerticalEndYAxisCoordinate.ToString("F2"));
            WriteValue("TPGT Coordinate Setting", "TPGTContactZAxisCoordinate", m_dTPGTContactZAxisCoordinate.ToString("F2"));

            WriteValue("TPGT GoDraw Coordinate Setting", "TPGTHorizontalStartXAxisCoordinate", m_nGoDrawTPGTHorizontalStartXAxisCoordinate.ToString("D"));
            WriteValue("TPGT GoDraw Coordinate Setting", "TPGTHorizontalStartYAxisCoordinate", m_nGoDrawTPGTHorizontalStartYAxisCoordinate.ToString("D"));
            WriteValue("TPGT GoDraw Coordinate Setting", "TPGTHorizontalEndXAxisCoordinate", m_nGoDrawTPGTHorizontalEndXAxisCoordinate.ToString("D"));
            WriteValue("TPGT GoDraw Coordinate Setting", "TPGTHorizontalEndYAxisCoordinate", m_nGoDrawTPGTHorizontalEndYAxisCoordinate.ToString("D"));
            WriteValue("TPGT GoDraw Coordinate Setting", "TPGTVerticalStartXAxisCoordinate", m_nGoDrawTPGTVerticalStartXAxisCoordinate.ToString("D"));
            WriteValue("TPGT GoDraw Coordinate Setting", "TPGTVerticalStartYAxisCoordinate", m_nGoDrawTPGTVerticalStartYAxisCoordinate.ToString("D"));
            WriteValue("TPGT GoDraw Coordinate Setting", "TPGTVerticalEndXAxisCoordinate", m_nGoDrawTPGTVerticalEndXAxisCoordinate.ToString("D"));
            WriteValue("TPGT GoDraw Coordinate Setting", "TPGTVerticalEndYAxisCoordinate", m_nGoDrawTPGTVerticalEndYAxisCoordinate.ToString("D"));
            WriteValue("TPGT GoDraw Coordinate Setting", "TPGTContactZServoValue", m_nGoDrawTPGTContactZServoValue.ToString("D"));

            WriteValue("Project Information Setting", "ProjectName", m_sProjectName);
            WriteValue("Project Information Setting", "FWTypeIndex", m_nFWTypeIndex.ToString("D")); 

            WriteValue("FW Check Setting", "FWCheckVersion", m_nFWCheckVersion.ToString("x2").ToUpper());

            WriteValue("Frequency Boundary Setting", "FrequencyLB", m_nFrequencyLB.ToString("D"));
            WriteValue("Frequency Boundary Setting", "FrequencyLB_MPP180", m_nFrequencyLB_MPP180.ToString("D"));
            WriteValue("Frequency Boundary Setting", "FrequencyHB", m_nFrequencyHB.ToString("D"));

            WriteValue("Color/Pattern Setting", "DisplayType", m_nDisplayType.ToString("D"));
            WriteValue("Color/Pattern Setting", "DisplayReportNumber", m_bDisplayReportNumber);
            WriteValue("Color/Pattern Setting", "DisplayProgressStatus", m_bDisplayProgressStatus);
            WriteValue("Color/Pattern Setting", "ColorSelectIndex", m_nColorSelectIndex.ToString("D"));
            WriteValue("Color/Pattern Setting", "DisplayColor", m_nDisplayColor.ToString("x8").ToUpper(), true, true);
            WriteValue("Color/Pattern Setting", "PatternType", m_nPatternType.ToString("D"));
            WriteValue("Color/Pattern Setting", "ManualPatternPath", m_sManualPatternPath);

            WriteValue("PHCK Pattern Setting", "ScreenIndex", m_nScreenIndex.ToString("D"));
            WriteValue("PHCK Pattern Setting", "ScreenWidth", m_dScreenWidth.ToString("F2"));
            WriteValue("PHCK Pattern Setting", "ScreenHeight", m_dScreenHeight.ToString("F2"));
            WriteValue("PHCK Pattern Setting", "LeftColor", m_nLeftColor.ToString("x8").ToUpper(), true, true);
            WriteValue("PHCK Pattern Setting", "RightColor", m_nRightColor.ToString("x8").ToUpper(), true, true);
            WriteValue("PHCK Pattern Setting", "GrayLineColor", m_nGrayLineColor.ToString("x8").ToUpper(), true, true);

            WriteValue("Gen8 Command Setting", "Gen8AFEType", m_nGen8AFEType.ToString("D"));
            WriteValue("Gen8 Command Setting", "Gen8FilterType", m_nGen8FilterType.ToString("D"));
            WriteValue("Gen8 Command Setting", "CommandScriptType", m_nGen8CommandScriptType.ToString("D"));
            WriteValue("Gen8 Command Setting", "UserDefinedPath", m_sGen8UserDefinedPath);
            WriteValue("Gen8 Command Setting", "SendCommandDelayTime", m_nGen8SendCommandDelayTime.ToString("D"));

            WriteValue("Command Flow Setting", "EnableReK", m_nEnableReK.ToString("D"));

            WriteValue("Gen8 Setting", "Gen8TraceType", m_nGen8TraceType.ToString("D"));
            WriteValue("Gen8 Setting", "Gen8RealTraceNumber", m_nGen8RealTraceNumber.ToString("D"));

            WriteValue("Report Data Setting", "ReportDataLength", m_nReportDataLength.ToString("D"), false);
            WriteValue("Report Data Setting", "ShiftStartByte", m_nShiftStartByte.ToString("D"), false);
            WriteValue("Report Data Setting", "ShiftByteNumber", m_nShiftByteNumber.ToString("D"), false);

            WriteValue("Noise & Tilt Noise Step Setting", "NoiseDataType", m_nNoiseDataType.ToString("D"));
            WriteValue("Noise & Tilt Noise Step Setting", "NoiseFrameNumber", m_nNoiseFrameNumber.ToString("D"));
            WriteValue("Noise & Tilt Noise Step Setting", "NoiseReportNumber", m_nNoiseReportNumber.ToString("D"));
            WriteValue("Noise & Tilt Noise Step Setting", "NoiseValidReportNumber", m_nNoiseValidReportNumber.ToString("D"));
            WriteValue("Noise & Tilt Noise Step Setting", "NoiseProcessReportNumber", m_nNoiseProcessReportNumber.ToString("D"));
            WriteValue("Noise & Tilt Noise Step Setting", "NoiseTimeout", m_dNoiseTimeout.ToString());
            WriteValue("Noise & Tilt Noise Step Setting", "NoiseNoReportInterruptTime", m_dNoiseNoReportInterruptTime.ToString());
            WriteValue("Noise & Tilt Noise Step Setting", "NoiseDisplayReportRate", m_nNoiseDisplayReportRate.ToString());

            WriteValue("DGT Step Setting", "DGTDrawType", m_nDGTDrawType.ToString("D"));
            /*
            WriteValue("DGT Step Setting", "DGTDefaultScaleDigiGain_P0", m_nDGTDefaultScaleDigiGain_P0.ToString("D"));
            WriteValue("DGT Step Setting", "DGTDefaultScaleDigiGain_Beacon_Rx", m_nDGTDefaultScaleDigiGain_Beacon_Rx.ToString("D"));
            WriteValue("DGT Step Setting", "DGTDefaultScaleDigiGain_Beacon_Tx", m_nDGTDefaultScaleDigiGain_Beacon_Tx.ToString("D"));
            WriteValue("DGT Step Setting", "DGTDefaultScaleDigiGain_PTHF_Rx", m_nDGTDefaultScaleDigiGain_PTHF_Rx.ToString("D"));
            WriteValue("DGT Step Setting", "DGTDefaultScaleDigiGain_PTHF_Tx", m_nDGTDefaultScaleDigiGain_PTHF_Tx.ToString("D"));
            WriteValue("DGT Step Setting", "DGTDefaultScaleDigiGain_BHF_Rx", m_nDGTDefaultScaleDigiGain_BHF_Rx.ToString("D"));
            WriteValue("DGT Step Setting", "DGTDefaultScaleDigiGain_BHF_Tx", m_nDGTDefaultScaleDigiGain_BHF_Tx.ToString("D"));
            */
            WriteValue("DGT Step Setting", "DGTRXValidReportNumber", m_nDGTRXValidReportNumber.ToString("D"));
            WriteValue("DGT Step Setting", "DGTTXValidReportNumber", m_nDGTTXValidReportNumber.ToString("D"));
            WriteValue("DGT Step Setting", "DGTMultiplyValue", m_nDGTMultiplyValue.ToString("D"));
            WriteValue("DGT Step Setting", "DGTDividValue", m_nDGTDividValue.ToString("D"));

            WriteValue("TPGT Step Setting", "TPGTRXValidReportNumber", m_nTPGTRXValidReportNumber.ToString("D"));
            WriteValue("TPGT Step Setting", "TPGTTXValidReportNumber", m_nTPGTTXValidReportNumber.ToString("D"));
            WriteValue("TPGT Step Setting", "TPGTVAngle", m_nTPGTVAngle.ToString("D"));
            WriteValue("TPGT Step Setting", "TPGTHorizontalRAngle", m_nTPGTHorizontalRAngle.ToString("D"));
            WriteValue("TPGT Step Setting", "TPGTVerticalRAngle", m_nTPGTVerticalRAngle.ToString("D"));
            WriteValue("TPGT Step Setting", "TPGTDisplayMessage", m_nTPGTDisplayMessage.ToString("D"));

            WriteValue("Normal Step Setting", "NormalValidReportNumber", m_nNormalValidReportNumber.ToString("D"));

            WriteValue("Normal Step Setting", "Normal800to400PwrRatio", m_dNormal800to400PwrRatio.ToString("F2"));

            WriteValue("TRxS Step Setting", "TRxSRXValidReportNumber", m_nTRxSRXValidReportNumber.ToString("D"));
            WriteValue("TRxS Step Setting", "TRxSTXValidReportNumber", m_nTRxSTXValidReportNumber.ToString("D"));

            WriteValue("TT Step Setting", "TTValidTipTraceNumber", m_nTTValidTipTraceNumber.ToString("D"));
            WriteValue("TT Step Setting", "TTRXValidReportNumber", m_nTTRXValidReportNumber.ToString("D"));
            WriteValue("TT Step Setting", "TTTXValidReportNumber", m_nTTTXValidReportNumber.ToString("D"));

            //WriteValue("PT Step Setting", "PPValidReportNumber", m_nPPValidReportNumber.ToString("D"));
            WriteValue("PT Step Setting", "PTValidReportNumber", m_nPTValidReportNumber.ToString("D"));
            WriteValue("PT Step Setting", "PTStartSkipReportNumber", m_nPTStartSkipReportNumber.ToString("D"));
            WriteValue("PT Step Setting", "PTEndSkipReportNumber", m_nPTEndSkipReportNumber.ToString("D"));
            //WriteValue("PT Step Setting", "PPRecordTime", m_dPPRecordTime.ToString());
            WriteValue("PT Step Setting", "PTRecordTime", m_dPTRecordTime.ToString());
            WriteValue("PT Step Setting", "PTMaxOffsetDiffer", m_nPTMaxOffsetDiffer.ToString("D"));
            WriteValue("PT Step Setting", "PTFirstOffsetWeight", m_nPTFirstOffsetWeight.ToString("D"), false);
            WriteValue("PT Step Setting", "PTLastOffsetWeight", m_nPTLastOffsetWeight.ToString("D"), false);
            WriteValue("PT Step Setting", "PTPenVersion", m_nPTPenVersion.ToString("D"), false);
            WriteValue("PT Step Setting", "PTMaxWeightDiffer", m_nPTMaxWeightDiffer.ToString("D"));
            WriteValue("PT Step Setting", "PTExtraIncWeight_25G", m_nPTExtraIncWeight_25G.ToString("D"), false);
            WriteValue("PT Step Setting", "PTExtraIncWeight_50G", m_nPTExtraIncWeight_50G.ToString("D"), false);
            WriteValue("PT Step Setting", "PTExtraIncWeight_75G", m_nPTExtraIncWeight_75G.ToString("D"), false);
            WriteValue("PT Step Setting", "PTExtraIncWeight_100G", m_nPTExtraIncWeight_100G.ToString("D"), false);

            WriteValue("Other Setting", "AutoTune_P0_detect_time_Index", m_nAutoTune_P0_detect_time_Index.ToString("D"), false);
            WriteValue("Other Setting", "StartDelayTime", m_dStartDelayTime.ToString());
            WriteValue("Other Setting", "RecordDataRetryCount", m_nRecordDataRetryCount.ToString("D"));
            WriteValue("Other Setting", "DrawLineTimeout", m_fDrawLineTimeout.ToString());
            WriteValue("Other Setting", "5TRawDataType", m_n5TRawDataType.ToString("D"));

            //WriteValue("Analysis Common Setting", "EdgeTraceNumber", m_nEdgeTraceNumber.ToString("D"));
            WriteValue("Analysis Common Setting", "PartNumber", m_nPartNumber.ToString("D"));

            WriteValue("Ranking Flow Setting", SpecificText.m_sFlowStep, m_nFlowStep.ToString("D"));

            WriteValue("Ranking Skip Frame Number Setting", "StartSkipReportCount", m_nStartSkipReportCount.ToString("D"));
            WriteValue("Ranking Skip Frame Number Setting", "LastSkipReportCount", m_nLastSkipReportCount.ToString("D"));

            WriteValue("Ranking Report Length Setting", "ReportDataLength", m_nReportDataLength.ToString("D"));
            WriteValue("Ranking Report Length Setting", "NormalReportDataLegth", m_nNormalReportDataLength.ToString("D"));
            WriteValue("Ranking Report Length Setting", "RTXTraceNumberByte", m_nRTXTraceNumberByte.ToString("D"));
            WriteValue("Ranking Report Length Setting", "AutoTuningInfoByte", m_nAutoTuningInfoByte.ToString("D"));
            WriteValue("Ranking Report Length Setting", "TraceTypeBit", m_nTraceTypeBit.ToString("D"));
            WriteValue("Ranking Report Length Setting", "DataTypeBit", m_nDataTypeBit.ToString("D"));
            WriteValue("Ranking Report Length Setting", "ExecuteTypeBit", m_nExecuteTypeBit.ToString("D"));
            WriteValue("Ranking Report Length Setting", "DataSectionByte", m_nDataSectionByte.ToString("D"));

            WriteValue("Ranking Compare Setting", "InnerReferenceValueHB", m_dInnerReferenceValueHB.ToString());
            WriteValue("Ranking Compare Setting", "EdgeSS_OFF_InnerRXWeightingPercent", m_dEdgeSS_OFF_InnerRXWeightingPercent.ToString());
            WriteValue("Ranking Compare Setting", "EdgeSS_OFF_InnerTXWeightingPercent", m_dEdgeSS_OFF_InnerTXWeightingPercent.ToString());
            WriteValue("Ranking Compare Setting", "EdgeSS_OFF_EdgeRXWeightingPercent", m_dEdgeSS_OFF_EdgeRXWeightingPercent.ToString());
            WriteValue("Ranking Compare Setting", "EdgeSS_OFF_EdgeTXWeightingPercent", m_dEdgeSS_OFF_EdgeTXWeightingPercent.ToString());
            WriteValue("Ranking Compare Setting", "MaxMinusMeanValueOverWarningStdevMagHB", m_dMaxMinusMeanValueOverWarningStdevMagHB.ToString());
            WriteValue("Ranking Compare Setting", "MaxValueOverWarningAbsValueHB", m_dMaxValueOverWarningAbsValueHB.ToString());

            //WriteValue("Normal Analysis Setting", "SkipDataPartNumber", m_nSkipDataPartNumber.ToString("D"));
            //WriteValue("Normal Analysis Setting", "StraightUsefulDataNumber", m_nStraightUsefulDataNumber.ToString("D"), false);
            WriteValue("Normal Analysis Setting", "NormalFilterRXValidReportNumber", m_nNormalFilterRXValidReportNumber.ToString("D"));
            WriteValue("Normal Analysis Setting", "NormalFilterTXValidReportNumber", m_nNormalFilterTXValidReportNumber.ToString("D"));
            WriteValue("Normal Analysis Setting", "ContactStepFilterType", m_nContactStepFilterType.ToString("D"));
            WriteValue("Normal Analysis Setting", "ContactStepFilterRXValue", m_nContactStepFilterRXValue.ToString("D"));
            WriteValue("Normal Analysis Setting", "ContactStepFilterTXValue", m_nContactStepFilterTXValue.ToString("D"));

            WriteValue("DGT Analysis Setting", "DGTCompensatePower", m_nDGTCompensatePower.ToString("D"));
            WriteValue("DGT Analysis Setting", "DGTDigiGainScaleHB", m_nDGTDigiGainScaleHB.ToString("D"));
            WriteValue("DGT Analysis Setting", "DGTDigiGainScaleLB", m_nDGTDigiGainScaleLB.ToString("D"));
            
            WriteValue("TPGT Analysis Setting", "TPGTTXStartPin", m_nTPGTTXStartPin.ToString("D"));
            WriteValue("TPGT Analysis Setting", "TPGTTXEndPin", m_nTPGTTXEndPin.ToString("D"));
            WriteValue("TPGT Analysis Setting", "TPGTRXStartPin", m_nTPGTRXStartPin.ToString("D"));
            WriteValue("TPGT Analysis Setting", "TPGTRXEndPin", m_nTPGTRXEndPin.ToString("D"));
            WriteValue("TPGT Analysis Setting", "TPGTGainRatio", m_nTPGTGainRatio.ToString("D"));
            WriteValue("TPGT Analysis Setting", "TPGTEdgeProcess", m_nTPGTEdgeProcess.ToString("D"));

            WriteValue("PCT Analysis Setting", "PCTPeakCheckRatio", m_dPCTPeakCheckRatio.ToString("F2"));
            WriteValue("PCT Analysis Setting", "PCTPeakCheckRatio5T", m_dPCTPeakCheckRatio5T.ToString("F2"));
            WriteValue("PCT Analysis Setting", "PCTPeakCheckRatio3T", m_dPCTPeakCheckRatio3T.ToString("F2"));

            WriteValue("DT Analysis Setting", "DTValidReportEdgeNumber", m_nDTValidReportEdgeNumber.ToString("D"));
            WriteValue("DT Analysis Setting", "DTDisplayChartDetailValue", m_nDTDisplayChartDetailValue.ToString("D"));

            WriteValue("DT Analysis Setting", "DTNormalHoverTHRatio_RX", m_dDTNormalHoverTHRatio_RX.ToString("F2"));
            WriteValue("DT Analysis Setting", "DTNormalHoverTHRatio_TX", m_dDTNormalHoverTHRatio_TX.ToString("F2"));
            WriteValue("DT Analysis Setting", "DTNormalContactTHRatio_RX", m_dDTNormalContactTHRatio_RX.ToString("F2"));
            WriteValue("DT Analysis Setting", "DTNormalContactTHRatio_TX", m_dDTNormalContactTHRatio_TX.ToString("F2"));
            WriteValue("DT Analysis Setting", "DTP0THCompRatio_800us", m_dDTP0THCompRatio_800us.ToString("F2"));
            WriteValue("DT Analysis Setting", "DTThresholdRatio_RX", m_dDTThresholdRatio_RX.ToString("F2"));
            WriteValue("DT Analysis Setting", "DTThresholdRatio_TX", m_dDTThresholdRatio_TX.ToString("F2"));
            WriteValue("DT Analysis Setting", "DTSkipCompareThreshold", m_nDTSkipCompareThreshold.ToString("D"));

            WriteValue("DT Analysis Setting", "DT7318TRxSSpecificReportType", m_nDT7318TRxSSpecificReportType.ToString("D"));

            WriteValue("TT Analysis Setting", "TTPolyFitOrder", m_nTTPolyFitOrder.ToString("D"));
            WriteValue("TT Analysis Setting", "TTValidReportEdgeNumber", m_nTTValidReportEdgeNumber.ToString("D"));
            WriteValue("TT Analysis Setting", "TTPTHFHoverRatio_RX", m_dTTPTHFHoverRatio_RX.ToString("F2"));
            WriteValue("TT Analysis Setting", "TTPTHFHoverRatio_TX", m_dTTPTHFHoverRatio_TX.ToString("F2"));
            WriteValue("TT Analysis Setting", "TTBHFHoverRatio_RX", m_dTTBHFHoverRatio_RX.ToString("F2"));
            WriteValue("TT Analysis Setting", "TTBHFHoverRatio_TX", m_dTTBHFHoverRatio_TX.ToString("F2"));

            //WriteValue("PT Analysis Setting", "PTValidReportEdgeNumber", m_nPTValidReportEdgeNumber.ToString("D"));
            WriteValue("PT Analysis Setting", "PTIQ_BSH_P_HB", m_nPTIQ_BSH_P_HB.ToString("D"));
            WriteValue("PT Analysis Setting", "PTIQ_BSH_P_LB", m_nPTIQ_BSH_P_LB.ToString("D"));
            WriteValue("PT Analysis Setting", "PressMaxDFTRxRefValueHB", m_nPressMaxDFTRxRefValueHB.ToString("x4").ToUpper(), true, true);
            WriteValue("PT Analysis Setting", "PressMaxDFTRxRefValueLB", m_nPressMaxDFTRxRefValueLB.ToString("x4").ToUpper(), true, true);

            WriteValue("LT Analysis Setting", "LTFirstEdgeAreaValidNumber", m_nLTFirstEdgeAreaValidNumber.ToString("D"));
            WriteValue("LT Analysis Setting", "LTLastEdgeAreaValidNumber", m_nLTLastEdgeAreaValidNumber.ToString("D"));
            WriteValue("LT Analysis Setting", "LTUseTP_GainCompensate ", m_nLTUseTP_GainCompensate.ToString("D"));

            WriteValue("Select Setting", "TNFrequencyNumber", m_nTNFrequencyNumber.ToString("D"));
            WriteValue("Select Setting", "OtherFrequencyNumber", m_nOtherFrequencyNumber.ToString("D"));

            WriteValue("Other Setting", "SetDigiGain", m_nSetDigiGain.ToString("D"));
            
            WriteValue("Process Setting", "SendFWParamType", m_nSendFWParamType.ToString("D"));
            WriteValue("Process Setting", "ProcessType", m_nProcessType.ToString("D"));
            WriteValue("Process Setting", "FixedPH1", m_nFixedPH1.ToString("D"));
            WriteValue("Process Setting", "FixedPH2", m_nFixedPH2.ToString("D"));

            WriteValue("FW Parameter Setting", SpecificText.m_scActivePen_FM_P0_TH, m_nParameter_cActivePen_FM_P0_TH.ToString("D"));
        }

        public static string SaveSpecificParameter(string sType, string sParameter, GoDrawControllerParameter cGoDrawCtrlParameter)
        {
            m_sSettingFilePath = m_cfrmMain.m_sIniPath;

            string sMessage = "Save Parameter :" + Environment.NewLine;

            if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_GENERAL)
            {
                if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_GENERAL)
                {
                    WriteValue("GoDraw Controller", "COMPort", cGoDrawCtrlParameter.m_sCOMPPort);
                    sMessage += string.Format("\"{0}\" = {1}", "COM Port", cGoDrawCtrlParameter.m_sCOMPPort) + Environment.NewLine;
                    WriteValue("GoDraw Controller", "Speed", cGoDrawCtrlParameter.m_dSpeed.ToString("F1"));
                    sMessage += string.Format("\"{0}\" = {1}", "Speed", cGoDrawCtrlParameter.m_dSpeed.ToString("F1")) + Environment.NewLine;
                    WriteValue("GoDraw Controller", "MaxCoordinateX", cGoDrawCtrlParameter.m_nMaxCoordinateX.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "Max X", cGoDrawCtrlParameter.m_nMaxCoordinateX.ToString("D")) + Environment.NewLine;
                    WriteValue("GoDraw Controller", "MaxCoordinateY", cGoDrawCtrlParameter.m_nMaxCoordinateY.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "Max Y", cGoDrawCtrlParameter.m_nMaxCoordinateY.ToString("D"));
                }
            }
            else if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_NORMALXYAXIS)
            {
                if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_START)
                {
                    WriteValue("GoDraw Coordinate Setting", "StartXAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "Start X", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D")) + Environment.NewLine;
                    WriteValue("GoDraw Coordinate Setting", "StartYAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "Start Y", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_END)
                {
                    WriteValue("GoDraw Coordinate Setting", "EndXAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "End X", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D")) + Environment.NewLine;
                    WriteValue("GoDraw Coordinate Setting", "EndYAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "End Y", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                }
            }
            else if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_NORMALZAXIS)
            {
                if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_TOP)
                {
                    WriteValue("GoDraw Coordinate Setting", "TopZServoValue", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "Top Servo Value", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_CONTACT)
                {
                    WriteValue("GoDraw Coordinate Setting", "ContactZServoValue", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "Contact Servo Value", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_DT_HOVER1ST)
                {
                    WriteValue("GoDraw Coordinate Setting", "HoverZServoValue_DT1st", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "DigitalTuning Hover1st Servo Value", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_DT_HOVER2ND)
                {
                    WriteValue("GoDraw Coordinate Setting", "HoverZServoValue_DT2nd", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "DigitalTuning Hover2nd Servo Value", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_PCT_HOVER1ST)
                {
                    WriteValue("GoDraw Coordinate Setting", "HoverZServoValue_PCT1st", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "PeakCheckTuning Hover1st Servo Value", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_PCT_HOVER2ND)
                {
                    WriteValue("GoDraw Coordinate Setting", "HoverZServoValue_PCT2nd", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "PeakCheckTuning Hover2nd Servo Value", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                }
            }
            else if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_TPGTXYAXIS)
            {
                if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_HORIZONTALSTART)
                {
                    WriteValue("TPGT GoDraw Coordinate Setting", "TPGTHorizontalStartXAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "HorizontalLine Start X", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D")) + Environment.NewLine;
                    WriteValue("TPGT GoDraw Coordinate Setting", "TPGTHorizontalStartYAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "HorizontalLine Start Y", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_HORIZONTALEND)
                {
                    WriteValue("TPGT GoDraw Coordinate Setting", "TPGTHorizontalEndXAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "HorizontalLine End X", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D")) + Environment.NewLine;
                    WriteValue("TPGT GoDraw Coordinate Setting", "TPGTHorizontalEndYAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "HorizontalLine End Y", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_VERTICALSTART)
                {
                    WriteValue("TPGT GoDraw Coordinate Setting", "TPGTVerticalStartXAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "VerticalLine Start X", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D")) + Environment.NewLine;
                    WriteValue("TPGT GoDraw Coordinate Setting", "TPGTVerticalStartYAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "VerticalLine Start Y", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                }
                else if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_XYAXIS_VERTICALEND)
                {
                    WriteValue("TPGT GoDraw Coordinate Setting", "TPGTVerticalEndXAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "VerticalLine End X", cGoDrawCtrlParameter.m_nCoordinateX.ToString("D")) + Environment.NewLine;
                    WriteValue("TPGT GoDraw Coordinate Setting", "TPGTVerticalEndYAxisCoordinate", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "VerticalLine End Y", cGoDrawCtrlParameter.m_nCoordinateY.ToString("D")) + Environment.NewLine;
                }
            }
            else if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_TPGTZAXIS)
            {
                if (sParameter == MainConstantParameter.m_sGODRAWCTRLPARAMETER_ZAXIS_CONTACT)
                {
                    WriteValue("TPGT GoDraw Coordinate Setting", "TPGTContactZServoValue", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                    sMessage += string.Format("\"{0}\" = {1}", "Contact Servo Value", cGoDrawCtrlParameter.m_nServoValueZ.ToString("D"));
                }
            }

            return sMessage;
        }

        public static void LoadGen8FWParameterAddress(int nAutoTuneVersion)
        {
            m_sSettingFilePath = m_cfrmMain.m_sGen8FWParameterAddressIniPath;

            JudgeFileType(m_sSettingFilePath);

            if (File.Exists(m_sSettingFilePath) == false)
                return;

            string sGroupName = "8F09";

            if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F09)
                sGroupName = "8F09";
            else if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F11)
                sGroupName = "8F11";
            else if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
                sGroupName = "8F18";

            if (nAutoTuneVersion > 0)
                sGroupName = string.Format("{0}_{1}", sGroupName, nAutoTuneVersion.ToString());

            SetGen8FWDefaultValue();

            GetParameterValueByMathCalculate(ref m_nRelativeAddress_PH1_Beacon, m_sSettingFilePath, sGroupName, "PH1_Beacon", m_nRelativeAddress_PH1_Beacon.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_PH2_Beacon, m_sSettingFilePath, sGroupName, "PH2_Beacon", m_nRelativeAddress_PH2_Beacon.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_PH3_Beacon, m_sSettingFilePath, sGroupName, "PH3_Beacon", m_nRelativeAddress_PH3_Beacon.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_MPP_SP_NUM, m_sSettingFilePath, sGroupName, "MPP_SP_NUM", m_nRelativeAddress_MPP_SP_NUM.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_MPP_EFFECT_NUM, m_sSettingFilePath, sGroupName, "MPP_EFFECT_NUM", m_nRelativeAddress_MPP_EFFECT_NUM.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_MPP_DFT_NUM, m_sSettingFilePath, sGroupName, "MPP_DFT_NUM", m_nRelativeAddress_MPP_DFT_NUM.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_TX_PPI_H, m_sSettingFilePath, sGroupName, "TX_PPI_H", m_nRelativeAddress_TX_PPI_H.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_TX_PPI_L, m_sSettingFilePath, sGroupName, "TX_PPI_L", m_nRelativeAddress_TX_PPI_L.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_PEN_MS_BSH_ADC_TP_NUM, m_sSettingFilePath, sGroupName, "_PEN_MS_BSH_ADC_TP_NUM", m_nRelativeAddress_PEN_MS_BSH_ADC_TP_NUM.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_PEN_MS_EFFECT_FW_SET_COEF_NUM, m_sSettingFilePath, sGroupName, "_PEN_MS_EFFECT_FW_SET_COEF_NUM", m_nRelativeAddress_PEN_MS_EFFECT_FW_SET_COEF_NUM.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_PEN_MS_DFT_NUM_IQ_FIR_CTL, m_sSettingFilePath, sGroupName, "_PEN_MS_DFT_NUM_IQ_FIR_CTL", m_nRelativeAddress_PEN_MS_DFT_NUM_IQ_FIR_CTL.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_PEN_MS_COS_INC_0, m_sSettingFilePath, sGroupName, "_PEN_MS_COS_INC_0", m_nRelativeAddress_PEN_MS_COS_INC_0.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_PEN_MS_SIN_INC_0, m_sSettingFilePath, sGroupName, "_PEN_MS_SIN_INC_0", m_nRelativeAddress_PEN_MS_SIN_INC_0.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00, m_sSettingFilePath, sGroupName, "_Pen_MS_ANA_TP_CTL_00", m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02, m_sSettingFilePath, sGroupName, "_Pen_MS_ANA_TP_CTL_02", m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03, m_sSettingFilePath, sGroupName, "_Pen_MS_ANA_TP_CTL_03", m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04, m_sSettingFilePath, sGroupName, "_Pen_MS_ANA_TP_CTL_04", m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_ANA_PH_CTL_00, m_sSettingFilePath, sGroupName, "_Pen_MS_ANA_PH_CTL_00", m_nRelativeAddress_Pen_MS_ANA_PH_CTL_00.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_M_S_CTL, m_sSettingFilePath, sGroupName, "_Pen_MS_M_S_CTL", m_nRelativeAddress_Pen_MS_M_S_CTL.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_CKS_CTL, m_sSettingFilePath, sGroupName, "_Pen_MS_CKS_CTL", m_nRelativeAddress_Pen_MS_CKS_CTL.ToString());
            GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_CT_ADC_SH_LMT, m_sSettingFilePath, sGroupName, "_Pen_MS_CT_ADC_SH_LMT", m_nRelativeAddress_Pen_MS_CT_ADC_SH_LMT.ToString());

            if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
            {
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00_2, m_sSettingFilePath, sGroupName, "_Pen_MS_ANA_TP_CTL_00_2", m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00_2.ToString());
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02_2, m_sSettingFilePath, sGroupName, "_Pen_MS_ANA_TP_CTL_02_2", m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02_2.ToString());
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03_2, m_sSettingFilePath, sGroupName, "_Pen_MS_ANA_TP_CTL_03_2", m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03_2.ToString());
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04_2, m_sSettingFilePath, sGroupName, "_Pen_MS_ANA_TP_CTL_04_2", m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04_2.ToString());

                GetParameterValueByMathCalculate(ref m_nRelativeAddress_TX_PPI_H_S0, m_sSettingFilePath, sGroupName, "TX_PPI_H_S0", m_nRelativeAddress_TX_PPI_H.ToString());
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_TX_PPI_L_S0, m_sSettingFilePath, sGroupName, "TX_PPI_L_S0", m_nRelativeAddress_TX_PPI_L.ToString());
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_TX_PPI_H_S1, m_sSettingFilePath, sGroupName, "TX_PPI_H_S1", m_nRelativeAddress_TX_PPI_H.ToString());
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_TX_PPI_L_S1, m_sSettingFilePath, sGroupName, "TX_PPI_L_S1", m_nRelativeAddress_TX_PPI_L.ToString());
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_TX_PPI_H_S2, m_sSettingFilePath, sGroupName, "TX_PPI_H_S2", m_nRelativeAddress_TX_PPI_H.ToString());
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_TX_PPI_L_S2, m_sSettingFilePath, sGroupName, "TX_PPI_L_S2", m_nRelativeAddress_TX_PPI_L.ToString());
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_TX_PPI_H_S3, m_sSettingFilePath, sGroupName, "TX_PPI_H_S3", m_nRelativeAddress_TX_PPI_H.ToString());
                GetParameterValueByMathCalculate(ref m_nRelativeAddress_TX_PPI_L_S3, m_sSettingFilePath, sGroupName, "TX_PPI_L_S3", m_nRelativeAddress_TX_PPI_L.ToString());
            }
        }

        public static void LoadStepSettingParameter()
        {
            m_sSettingFilePath = m_cfrmMain.m_sIniPath;

            for (int nIndex = 0; nIndex < m_nSTEP_NUMBER; nIndex++)
            {
                if (m_cStepSettingParameter_Array[nIndex] == null)
                    m_cStepSettingParameter_Array[nIndex] = new StepSettingParameter();

                m_cStepSettingParameter_Array[nIndex].m_sStepName = m_sStepSettingNameSet_Array[nIndex];
                m_cStepSettingParameter_Array[nIndex].m_sStepParameterName = m_sStepSettingParamNameSet_Array[nIndex];
                m_cStepSettingParameter_Array[nIndex].m_eTuningStep = m_eMainTuningStepSet_Array[nIndex];

                if (m_cStepSettingParameter_Array[nIndex].m_eTuningStep != MainTuningStep.SERVERCONTRL)
                    GetParameterValue(ref m_cStepSettingParameter_Array[nIndex].m_bEnable, "Step Setting", m_sStepSettingParamNameSet_Array[nIndex], "0");
            }
        }

        public static void SetStepSettingParameter(StepSettingParameter[] cStepSettingParameter_Array)
        {
            m_sSettingFilePath = m_cfrmMain.m_sIniPath;

            for (int nIndex = 0; nIndex < m_nSTEP_NUMBER; nIndex++)
            {
                if (cStepSettingParameter_Array[nIndex].m_eTuningStep != MainTuningStep.SERVERCONTRL)
                    WriteValue("Step Setting", cStepSettingParameter_Array[nIndex].m_sStepParameterName, cStepSettingParameter_Array[nIndex].m_bEnable);
            }
        }

        private static void SetGen8FWDefaultValue()
        {
            if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F09)
            {
                m_nRelativeAddress_PH1_Beacon = 0x0004;
                m_nRelativeAddress_PH2_Beacon = 0x0005;
                m_nRelativeAddress_PH3_Beacon = 0x0006;
                m_nRelativeAddress_MPP_SP_NUM = 0x0000;
                m_nRelativeAddress_MPP_EFFECT_NUM = 0x0001;
                m_nRelativeAddress_MPP_DFT_NUM = 0x0002;
                m_nRelativeAddress_TX_PPI_L = 0x0004;
                m_nRelativeAddress_PEN_MS_BSH_ADC_TP_NUM = 0x000E;
                m_nRelativeAddress_PEN_MS_EFFECT_FW_SET_COEF_NUM = 0x000F;
                m_nRelativeAddress_PEN_MS_DFT_NUM_IQ_FIR_CTL = 0x0012;
                m_nRelativeAddress_PEN_MS_COS_INC_0 = 0x001A;
                m_nRelativeAddress_PEN_MS_SIN_INC_0 = 0x001F;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00 = 0x0070;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02 = 0x0072;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03 = 0x0073;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04 = 0x0074;
                m_nRelativeAddress_Pen_MS_ANA_PH_CTL_00 = 0x0094;
                m_nRelativeAddress_Pen_MS_M_S_CTL = 0x00AA;
                m_nRelativeAddress_Pen_MS_CKS_CTL = 0x00A3;
                m_nRelativeAddress_Pen_MS_CT_ADC_SH_LMT = 0x00A6;
            }
            else if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F11)
            {
                m_nRelativeAddress_PH1_Beacon = 0x0004;
                m_nRelativeAddress_PH2_Beacon = 0x0005;
                m_nRelativeAddress_PH3_Beacon = 0x0006;
                m_nRelativeAddress_MPP_SP_NUM = 0x0000;
                m_nRelativeAddress_MPP_EFFECT_NUM = 0x0001;
                m_nRelativeAddress_MPP_DFT_NUM = 0x0002;
                m_nRelativeAddress_TX_PPI_L = 0x0004;
                m_nRelativeAddress_PEN_MS_BSH_ADC_TP_NUM = 0x000E;
                m_nRelativeAddress_PEN_MS_EFFECT_FW_SET_COEF_NUM = 0x000F;
                m_nRelativeAddress_PEN_MS_DFT_NUM_IQ_FIR_CTL = 0x0012;
                m_nRelativeAddress_PEN_MS_COS_INC_0 = 0x001A;
                m_nRelativeAddress_PEN_MS_SIN_INC_0 = 0x001F;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00 = 0x007F;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02 = 0x0081;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03 = 0x0082;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04 = 0x0083;
                m_nRelativeAddress_Pen_MS_ANA_PH_CTL_00 = 0x00AA;
                m_nRelativeAddress_Pen_MS_M_S_CTL = 0x00C5;
                m_nRelativeAddress_Pen_MS_CKS_CTL = 0x00BE;
                m_nRelativeAddress_Pen_MS_CT_ADC_SH_LMT = 0x00C1;
            }
            else if (MainConstantParameter.m_eGen8ICSolution == Gen8Solution.Solution_8F18)
            {
                m_nRelativeAddress_PH1_Beacon = 0x0004;
                m_nRelativeAddress_PH2_Beacon = 0x0005;
                m_nRelativeAddress_PH3_Beacon = 0x0006;
                m_nRelativeAddress_MPP_SP_NUM = 0x0000;
                m_nRelativeAddress_MPP_EFFECT_NUM = 0x0001;
                m_nRelativeAddress_MPP_DFT_NUM = 0x0002;
                m_nRelativeAddress_TX_PPI_L = 0x0004;
                m_nRelativeAddress_PEN_MS_BSH_ADC_TP_NUM = 0x000E;
                m_nRelativeAddress_PEN_MS_EFFECT_FW_SET_COEF_NUM = 0x000F;
                m_nRelativeAddress_PEN_MS_DFT_NUM_IQ_FIR_CTL = 0x0012;
                m_nRelativeAddress_PEN_MS_COS_INC_0 = 0x001A;
                m_nRelativeAddress_PEN_MS_SIN_INC_0 = 0x001F;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00 = 0x00DA;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02 = 0x00DC;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03 = 0x00DD;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04 = 0x00DE;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_00_2 = 0x00E1;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_02_2 = 0x00E3;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_03_2 = 0x00E4;
                m_nRelativeAddress_Pen_MS_ANA_TP_CTL_04_2 = 0x00E5;
                m_nRelativeAddress_Pen_MS_ANA_PH_CTL_00 = 0x012F;
                m_nRelativeAddress_Pen_MS_M_S_CTL = 0x0162;
                m_nRelativeAddress_Pen_MS_CKS_CTL = 0x0159;
                m_nRelativeAddress_Pen_MS_CT_ADC_SH_LMT = 0x015C;
            }
        }

        private static void SetDVDD(int nIndex)
        {
            if (nIndex == 0)
                m_nDVDD = 28;
            else if (nIndex == 1)
                m_nDVDD = 30;
            else if (nIndex == 2)
                m_nDVDD = 33;
            else if (nIndex == 3)
                m_nDVDD = 50;
            else
                m_nDVDD = 33;
        }

        private static void SetVIO(int nIndex)
        {
            if (nIndex == 0)
                m_nVIO = 18;
            else if (nIndex == 1)
                m_nVIO = 28;
            else if (nIndex == 2)
                m_nVIO = 30;
            else if (nIndex == 3)
                m_nVIO = 33;
            else if (nIndex == 4)
                m_nVIO = 50;
            else
                m_nVIO = 33;
        }
    }
}
