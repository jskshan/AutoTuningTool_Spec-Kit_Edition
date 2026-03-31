using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    /// <summary>
    /// 此Function用於避免作業系統設定的浮點數符號不同,導致字串轉換
    /// 後的數值錯誤
    /// ex:system define ','
    ///    3.15 轉換後會變成315,須把3.15的'.'以','取代
    /// </summary>
    public class ElanConvert
    {
        public static double ConvertStringToDouble(string sValue, bool bUseTryParse = true)
        {
            StringBuilder sb = new StringBuilder(255);
            double dReturnValue = 0.0f;
            string sReplacedValue = "";

            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            sReplacedValue = sValue.Replace(".", sb.ToString());

            if (bUseTryParse == true)
                Double.TryParse(sReplacedValue, out dReturnValue);
            else
                dReturnValue = Double.Parse(sReplacedValue);

            return dReturnValue;
        }

        public static float ConvertStringToFloat(string sValue, bool bUseTryParse = true)
        {
            StringBuilder sb = new StringBuilder(255);
            float fReturnValue = 0.0f;
            string sReplacedValue = "";

            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            sReplacedValue = sValue.Replace(".", sb.ToString());

            if (bUseTryParse == true)
                float.TryParse(sReplacedValue, out fReturnValue);
            else
                fReturnValue = float.Parse(sReplacedValue);

            return fReturnValue;
        }

        public static string ConvertToString(string sValue)
        {
            StringBuilder sb = new StringBuilder(255);
            string sConvertValue = "";

            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            sConvertValue = sValue.Replace(sb.ToString(), ".");

            return sConvertValue;
        }

        public static double TryConvertStringToDouble(string sValue)
        {
            if (String.IsNullOrEmpty(sValue))
                return 0.00;
            try
            {
                double dValue = Math.Round(Convert.ToDouble(sValue), 2, MidpointRounding.AwayFromZero);
                return dValue;
            }
            catch (Exception)
            {
                return 0.00;
            }
        }

        public static int TryConvertStringToInt(string sValue)
        {
            if (String.IsNullOrEmpty(sValue))
                return 0;
            try
            {
                int nValue = Convert.ToInt32(sValue);
                return nValue;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static short TryConvertHexStringToShort(string sValue)
        {
            if (String.IsNullOrEmpty(sValue))
                return 0;
            try
            {
                short nValue = Convert.ToInt16(sValue, 16);
                return nValue;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static double SetDoubleValueRoundUp(double dInputValue, int nDecimalPointNumber)
        {
            double dFloorValue = Math.Floor(dInputValue);

            if ((dInputValue - dFloorValue) > 0) //判斷dInputValue是否為整數
            {
                if ((dInputValue - Math.Round(dInputValue, nDecimalPointNumber)) != 0) //判斷所要取的位數是否存在
                {
                    //利用四捨五入的方法判斷是否要進位，若取的下一位數大於等於5則不用進位
                    if (Convert.ToInt32(dInputValue * Math.Pow(10, nDecimalPointNumber + 1) % 10) < 5)
                        return Math.Round(dInputValue, nDecimalPointNumber, MidpointRounding.AwayFromZero) + Math.Pow(0.1, nDecimalPointNumber);
                    else
                        return Math.Round(dInputValue, nDecimalPointNumber, MidpointRounding.AwayFromZero);
                }
                else
                    return dInputValue;
            }
            else
                return dInputValue;
        }

        public static bool CheckIsHexDecimal(string sValue)
        {
            try
            {
                const string sPATTERN = @"([^A-Fa-f0-9]|\s+?)+";
                return !System.Text.RegularExpressions.Regex.IsMatch(sValue, sPATTERN);
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckIsInteger(string sValue)
        {
            try
            {
                const string sPATTERN = @"^\d*$";
                return System.Text.RegularExpressions.Regex.IsMatch(sValue, sPATTERN);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 驗證是否為整數
        /// </summary>
        /// <param name="sValue">要驗證的字串</param>
        /// <returns>驗證通過返回true，若否返回false</returns>
        public static bool CheckIsInt(string sValue)
        {
            int nNumber;
            bool bSuccessFlag = Int32.TryParse(sValue, out nNumber);

            return bSuccessFlag;  
        }

        public static string ConvertByteArrayToCharString(byte[] byteValue_Array)
        {
            StringBuilder sbHexValue = new StringBuilder(byteValue_Array.Length * 10);

            foreach (byte byteValue in byteValue_Array)
            {
                /*
                if (byteValue == '\0')
                    break;
                */

                sbHexValue.AppendFormat("{0}", Convert.ToChar(byteValue));
            }
            return sbHexValue.ToString();
        }

        /// <summary>
        /// 檢查文件名是否合法，文字名中不能包含字符\/:*?"<>|
        /// </summary>
        /// <param name="sFileName">文件名，不包含路徑</param>
        /// <returns></returns>
        public static bool CheckIsValidFileName(string sFileName)
        {
            bool bIsValidFlag = true;
            string sErrorChar = "\\/:*?\"<>|";

            if (string.IsNullOrEmpty(sFileName))
                bIsValidFlag = false;
            else
            {
                for (int nIndex = 0; nIndex < sErrorChar.Length; nIndex++)
                {
                    if (sFileName.Contains(sErrorChar[nIndex].ToString()))
                    {
                        bIsValidFlag = false;
                        break;
                    }
                }
            }

            return bIsValidFlag;
        }

        public static FileStream SetFileStream(ref string sErrorMessage, string sFilePath)
        {
            FileStream fsFile = null;

            try
            {
                fsFile = new FileStream(sFilePath, FileMode.Create);
            }
            catch
            {
                string sFileName = Path.GetFileName(sFilePath);
                sErrorMessage = string.Format("File Path Too Long({0})", sFileName);
            }

            return fsFile;
        }

        /// <summary>
        /// 使用PH1, PH2計算掃描頻率，並輸出掃描頻率字串
        /// </summary>
        /// <param name="nPH1">PH1數值</param>
        /// <param name="nPH2">PH2數值</param>
        /// <returns>掃描頻率字串</returns>
        public static string ComputeFrequnecyToString(int nPH1, int nPH2)
        {
            if (nPH1 == -1 || nPH2 == -1)
                return Convert.ToString(-1);

            double dPenScanFrequency = Math.Round(((double)MainConstantParameter.m_nICCLOCKFREQUENCY / (double)((nPH1 + nPH2 + 2) * 1000)), 3, MidpointRounding.AwayFromZero);

            return dPenScanFrequency.ToString("0.000");
        }

        /// <summary>
        /// 使用PH1, PH2計算掃描頻率，並輸出掃描頻率double數值
        /// </summary>
        /// <param name="nPH1">PH1數值</param>
        /// <param name="nPH2">PH2數值</param>
        /// <returns>掃描頻率double數值</returns>
        public static double ComputeFrequnecyToDouble(int nPH1, int nPH2)
        {
            if (nPH1 == -1 || nPH2 == -1)
                return -1;

            double dPenScanFrequency = Math.Round(((double)MainConstantParameter.m_nICCLOCKFREQUENCY / (double)((nPH1 + nPH2 + 2) * 1000)), 3, MidpointRounding.AwayFromZero);

            return dPenScanFrequency;
        }

        /// <summary>
        /// 使用DigiGainScale, MultiplyValue, DividValue計算DigiGain，並輸出DigiGain int數值
        /// </summary>
        /// <param name="nDigiGainScale">DigiGainScale</param>
        /// <returns></returns>
        public static int ConvertScaleToDigiGain(int nDigiGainScale)
        {
            int nDigiGainValue = (int)((double)nDigiGainScale * ParamAutoTuning.m_nDGTMultiplyValue / ParamAutoTuning.m_nDGTDividValue);

            return nDigiGainValue;
        }

        public static int ConvertDigiGainToScale(int nDigiGainValue)
        {
            int nDigiGainScale = (int)((double)nDigiGainValue * ParamAutoTuning.m_nDGTDividValue / ParamAutoTuning.m_nDGTMultiplyValue);

            return nDigiGainScale;
        }
    }
}
