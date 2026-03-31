using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FingerAutoTuning
{
    /// <summary>
    /// 此Function用於避免作業系統設定的浮點數符號不同,導致字串轉換
    /// 後的數值錯誤
    /// ex:system define ','
    ///    3.15 轉換後會變成315,須把3.15的'.'以','取代
    /// </summary>
    public class ElanConvert
    {
        public static double Convert2Double(string sValue, bool bUseTryParse = true)
        {
            StringBuilder sb = new StringBuilder(255);
            double dValue = 0.0;
            string sReplacedIniValue = "";

            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            sReplacedIniValue = sValue.Replace(".", sb.ToString());

            if (bUseTryParse == true)
                Double.TryParse(sReplacedIniValue, out dValue);
            else
                dValue = Double.Parse(sReplacedIniValue);

            return dValue;
        }

        public static float Convert2Float(string sValue, bool bUseTryParse = true)
        {
            StringBuilder sb = new StringBuilder(255);
            float fValue = 0.0f;
            string sReplacedIniValue = "";

            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            sReplacedIniValue = sValue.Replace(".", sb.ToString());

            if (bUseTryParse == true)
                float.TryParse(sReplacedIniValue, out fValue);
            else
                fValue = float.Parse(sReplacedIniValue);

            return fValue;
        }

        public static string Convert2String(string sValue)
        {
            StringBuilder sb = new StringBuilder(255);
            string sConvertedValue = "";

            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            sConvertedValue = sValue.Replace(sb.ToString(), ".");

            return sConvertedValue;
        }

        public static double TryDoubleConvert(string sValue)
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

        public static int TryIntConvert(string sValue)
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

        public static double DoubleRoundUp(double dInputValue, int nNumber)
        {
            double dFloorValue = Math.Floor(dInputValue);

            if ((dInputValue - dFloorValue) > 0) //判斷input是否為整數
            {
                if ((dInputValue - Math.Round(dInputValue, nNumber)) != 0) //判斷所要取的位數是否存在
                {
                    //利用四捨五入的方法判斷是否要進位，若取的下一位數大於等於5則不用進位
                    if (Convert.ToInt32(dInputValue * Math.Pow(10, nNumber + 1) % 10) < 5)
                    {
                        return Math.Round(dInputValue, nNumber, MidpointRounding.AwayFromZero) + Math.Pow(0.1, nNumber);
                    }
                    else
                    {
                        return Math.Round(dInputValue, nNumber, MidpointRounding.AwayFromZero);
                    }
                }
                else
                {
                    return dInputValue;
                }
            }
            else
            {
                return dInputValue;
            }
        }

        public static bool IsInteger(string sValue)
        {
            try
            {
                const string PATTERN = @"^\d*$";
                return System.Text.RegularExpressions.Regex.IsMatch(sValue, PATTERN);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 驗證整數
        /// </summary>
        /// <param name="sValue">要驗證的字串</param>
        /// <returns>驗證通過返回true</returns>
        public static bool IsInt(string sValue)
        {
            int nNumber;

            bool bSuccess = Int32.TryParse(sValue, out nNumber);

            return bSuccess;
        }

        public static string ByteArrayToCharString(byte[] byteData_Array)
        {
            StringBuilder sbHexValue = new StringBuilder(byteData_Array.Length * 10);

            foreach (byte byteData in byteData_Array)
            {
                //if (byteData == '\0')
                //    break;
                sbHexValue.AppendFormat("{0}", Convert.ToChar(byteData));
            }

            return sbHexValue.ToString();
        }

        /// <summary>
        /// 檢查文件名是否合法，文字名中不能包含字符\/:*?"<>|
        /// </summary>
        /// <param name="sFileName">文件名，不包含路徑</param>
        /// <returns></returns>
        public static bool IsValidFileName(string sFileName)
        {
            bool bIsValid = true;
            string sErrorChar = "\\/:*?\"<>|";
            if (string.IsNullOrEmpty(sFileName))
            {
                bIsValid = false;
            }
            else
            {
                for (int nIndex = 0; nIndex < sErrorChar.Length; nIndex++)
                {
                    if (sFileName.Contains(sErrorChar[nIndex].ToString()))
                    {
                        bIsValid = false;
                        break;
                    }
                }
            }
            return bIsValid;
        }

        public static FileStream ConvertToFileStream(ref string sErrorMessage, string sPath)
        {
            FileStream fsFile = null;

            try
            {
                fsFile = new FileStream(sPath, FileMode.Create);
            }
            catch (PathTooLongException ex)
            {
                string sMessage = ex.Message;

                string sPathFileName = Path.GetFileName(sPath);
                sErrorMessage = string.Format("File Path Too Long({0})", sPathFileName);
            }

            return fsFile;
        }

        /// <summary>
        /// 判断是否十六进制格式字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsHexadecimal(string sValue)
        {
            const string PATTERN = @"[A-Fa-f0-9]+$";
            bool bValid = System.Text.RegularExpressions.Regex.IsMatch(sValue, PATTERN);

            if (bValid == true)
            {
                // 长度判断 长度+1 与3的余数为0表示长度符合要求
                if ((sValue.Length + 1) % 3 != 0)
                {
                    bValid = false;
                }
                else
                {
                    // 空格判断，空格数为长度+1 与3的余数
                    if (sValue.Length > 2)
                    {

                        string[] sSplit_Array = sValue.Split(new char[] { ' ' });

                        //分离后，得到空格数
                        int nSpaceCount = sSplit_Array.Length - 1;

                        //实际空格数
                        int nValue = ((sValue.Length + 1) / 3) - 1;

                        // 获得字符串中所有的空格数
                        int nCount = Regex.Matches(sValue, @" ").Count;

                        if (nSpaceCount != nValue || nCount != nSpaceCount)
                        {
                            bValid = false;
                        }
                        else
                        {
                            for (int nIndex = 0; nIndex < sValue.Length; nIndex++)
                            {
                                if ((nIndex + 1) % 3 == 0)
                                {
                                    // 判断 2 5 8 11 .. 位置是否为空格
                                    if (sValue[nIndex] != ' ')
                                    {
                                        bValid = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (sValue.Length < 2)
                    {
                        bValid = false;
                    }
                }
            }

            return bValid;
        }

        public static bool CheckIsHexaDecimal(string sValue)
        {
            try
            {
                const string PATTERN = @"([^A-Fa-f0-9]|\s+?)+";
                return !System.Text.RegularExpressions.Regex.IsMatch(sValue, PATTERN);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDouble(string sValue)
        {
            try
            {
                double dValue = Convert.ToDouble(sValue);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static double Convert2TXFrequency(int nPH1Value, int nPH2Value)
        {
            double dFrequency = MainConstantParameter.m_nTPCLOCK_63xx / (nPH1Value + nPH2Value + 2) / 2;

            return dFrequency;
        }

        public static double Convert2Frequency(int nPH1Value, int nPH2Value)
        {
            double dFrequency = Math.Round((double)MainConstantParameter.m_nTPCLOCK_63xx / ((nPH1Value + nPH2Value + 2) * 2 * 1000), 3, MidpointRounding.AwayFromZero);

            return dFrequency;
        }

        public static double ConvertSum2Frequency(int nSumValue, int nRoundDigit = 3)
        {
            double dFrequency = 0.0;

            if (nRoundDigit < 0)
                dFrequency = (double)MainConstantParameter.m_nTPCLOCK_63xx / ((nSumValue + 2) * 2 * 1000);
            else
                dFrequency = Math.Round((double)MainConstantParameter.m_nTPCLOCK_63xx / ((nSumValue + 2) * 2 * 1000), nRoundDigit, MidpointRounding.AwayFromZero);

            return dFrequency;
        }

        public static int Convert2PH2(double dFrequency, int nPH1Value)
        {
            int nValue = (int)Math.Round((MainConstantParameter.m_nTPCLOCK_63xx / (dFrequency * 1000 * 2)) - 2 - nPH1Value, 0, MidpointRounding.AwayFromZero);

            return nValue;
        }

        public static int Convert2PH1PH2SumInt(double dFrequency)
        {
            int nValue = (int)((MainConstantParameter.m_nTPCLOCK_63xx / (dFrequency * 2 * 1000)) - 2);

            return nValue;
        }

        public static double Convert2PH1PH2SumDouble(double dFrequency)
        {
            double dValue = (MainConstantParameter.m_nTPCLOCK_63xx / (dFrequency * 2 * 1000)) - 2;

            return dValue;
        }

        public static int Convert2SuggestDFT_NUM(int nPH1Value, int nPH2Value, int nTXTraceNumber, int nIdealScanTime)
        {
            //Compute the TX frequency
            double dTXFrequency = Convert2TXFrequency(nPH1Value, nPH2Value);

            //Compute the Sum
            int nSumValue = (int)(1.0f / ((1.0f / dTXFrequency) * nTXTraceNumber) / nIdealScanTime);

            return nSumValue;
        }

        public static int Convert2SelfPH2SumInt(int nPH2E_LAT, int nPH2E_LMT, int nPH2_LAT, int nPH2)
        {
            int nPH2Sum = nPH2E_LAT + nPH2E_LMT + (nPH2 - nPH2E_LMT);

            return nPH2Sum;
        }

        /// <summary>
        /// 驗證是否為整數
        /// </summary>
        /// <param name="sValue">要驗證的字串</param>
        /// <returns>驗證通過返回true，若否返回false</returns>
        public static bool CheckIsInt(string sValue)
        {
            int nNumber;
            bool bSuccess = Int32.TryParse(sValue, out nNumber);

            return bSuccess;
        }
    }
}
