using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
	class StringConvert
	{
        public static Dictionary<string, int> m_dictOperatorLevel;

        public static Dictionary<MainStep, string> m_dictMainStepMappingTable = new Dictionary<MainStep, string>()
        {
            {MainStep.FrequencyRank_Phase1,    "FrequencyRank Phase1"},
            {MainStep.FrequencyRank_Phase2,    "FrequencyRank Phase2"},
            {MainStep.AC_FrequencyRank,        "AC FrequencyRank"},
            {MainStep.Raw_ADC_Sweep,           "Raw ADC Sweep"},
            {MainStep.Self_FrequencySweep,     "Self FrequencySweep"},
            {MainStep.Self_NCPNCNSweep,        "Self NCPNCNSweep"},
            {MainStep.Else,                    "Else"}
        };

        public static Dictionary<MainStep, string> m_dictMainStepCodeNameMappingTable = new Dictionary<MainStep, string>()
        {
            {MainStep.FrequencyRank_Phase1,    "FR PH1"},
            {MainStep.FrequencyRank_Phase2,    "FR PH2"},
            {MainStep.AC_FrequencyRank,        "AC FR"},
            {MainStep.Raw_ADC_Sweep,           "RawADC S"},
            {MainStep.Self_FrequencySweep,     "Self FS"},
            {MainStep.Self_NCPNCNSweep,        "Self PNS"},
            {MainStep.Else,                    "Else"}
        };

        public static double ComputeFrequnecyToDouble(int nPH1, int nPH2)
        {
            if (nPH1 <= 0 || nPH2 <= 0 || nPH1 > 100 || nPH2 > 100)
                return 0;

            double dPenScanFrequency = Math.Round(((double)frmMain.m_nWorkingFrequency / (double)((nPH1 + nPH2 + 2) * 1000)), 3, MidpointRounding.AwayFromZero);
            return dPenScanFrequency;
        }

        /// <summary>
        /// 將Csv讀入DataTable
        /// </summary>
        /// <param name="sFilePath">csv檔案路徑</param>
        /// <param name="sTitleName"></param>
        /// <param name="bRawADCSStep"></param>
        public static DataTable ConvertCsvToDataTable(string sFilePath, string sTitleName, bool bRawADCSStep = false)
        {
            DataTable datatableData = new DataTable();
            String sCsvSplitToken = "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)";
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default, false);
            int nIndex = 0;
            bool bGetTitle = false;
            bool bGetColumnHeader = false;

            try
            {
                srFile.Peek();

                while (srFile.Peek() > 0)
                {
                    string sLine = srFile.ReadLine();

                    if (sLine == sTitleName)
                    {
                        bGetTitle = true;
                        continue;
                    }

                    if (bGetTitle == true)
                    {
                        if (bGetColumnHeader == false) //如果是欄位行，則自動加入欄位。
                        {
                            MatchCollection cMatchCollection = Regex.Matches(sLine, sCsvSplitToken);

                            foreach (Match cMatch in cMatchCollection)
                            {
                                if (bRawADCSStep == true)
                                {
                                    if (cMatch.Value == "")
                                        datatableData.Columns.Add(cMatch.Value, typeof(string)); //增加列標題
                                    else
                                        datatableData.Columns.Add(cMatch.Value, typeof(double)); //增加列標題
                                }
                                else
                                    datatableData.Columns.Add(cMatch.Value); //增加列標題
                            }

                            bGetColumnHeader = true;
                        }
                        else
                        {
                            MatchCollection cMatchCollection = Regex.Matches(sLine, "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
                            nIndex = 0;
                            DataRow drData = datatableData.NewRow();

                            foreach (Match cMatch in cMatchCollection)
                            {
                                drData[nIndex] = cMatch.Value;
                                nIndex++;
                            }

                            datatableData.Rows.Add(drData);  //DataTable 增加一行     
                        }
                    }
                }
            }
            finally
            {
                srFile.Close();
            }

            return datatableData;
        }

        private static double ComputeValueByOperator(double dLeftValue, double dRightValue, char charOperator)
        {
            switch (charOperator)
            {
                case '+':
                    return dLeftValue + dRightValue;
                case '-':
                    return dLeftValue - dRightValue;
                case '*':
                    return dLeftValue * dRightValue;
                case '/':
                    return dLeftValue / dRightValue;
            }

            return 0;
        }

        public static UInt16 GetCalculateResult(string sSourceString)
        {
            Stack<string> stacksData = new Stack<string>();
            var varArray = sSourceString.Split(' ');

            for (int nStringIndex = 0; nStringIndex < varArray.Length; nStringIndex++)
            {
                string sCurrent = varArray[nStringIndex];

                if (ElanConvert.CheckIsInt(sCurrent) == true)
                {
                    stacksData.Push(sCurrent);
                }
                else if (GetOperatorLevel.ContainsKey(sCurrent))
                {
                    double dRight = double.Parse(stacksData.Pop());
                    double dLeft = double.Parse(stacksData.Pop());
                    stacksData.Push(ComputeValueByOperator(dLeft, dRight, sCurrent[0]).ToString());
                }
            }

            UInt16 nValue = (UInt16)(double.Parse(stacksData.Pop()));

            return nValue;
        }

        public static string ConvertStringToRPN(ref bool bErrorFlag, ref string sErrorMessage, string sSourceString)
        {
            bErrorFlag = false;
            sErrorMessage = "";
            StringBuilder sbResult = new StringBuilder();
            Stack<string> stacksData = new Stack<string>();
            string[] sSplit_Array = sSourceString.Split(' ');

            for (int nStringIndex = 0; nStringIndex < sSplit_Array.Length; nStringIndex++)
            {
                string sCurrent = sSplit_Array[nStringIndex];

                bool bHexFlag_1 = sCurrent.Contains("0x");
                bool bHexFlag_2 = sCurrent.Contains("0X");

                if (bHexFlag_1 == true || bHexFlag_2 == true)
                {
                    sCurrent = sCurrent.Replace("0x", "");
                    sCurrent = sCurrent.Replace("0X", "");

                    try
                    {
                        int nValue = Int32.Parse(sCurrent, System.Globalization.NumberStyles.HexNumber);
                        sbResult.Append(nValue.ToString() + " ");
                    }
                    catch (Exception ex)
                    {
                        bErrorFlag = true;
                        sErrorMessage = ex.Message.ToString();
                        return sbResult.ToString();
                    }
                }
                else if (ElanConvert.CheckIsInt(sCurrent) == true)
                {
                    sbResult.Append(sCurrent + " ");
                }
                else if (GetOperatorLevel.ContainsKey(sCurrent))
                {
                    if (stacksData.Count > 0)
                    {
                        var varPrevious = stacksData.Peek();

                        if (varPrevious == "(")
                        {
                            stacksData.Push(sCurrent);
                            continue;
                        }
                        if (sCurrent == "(")
                        {
                            stacksData.Push(sCurrent);
                            continue;
                        }
                        if (sCurrent == ")")
                        {
                            while (stacksData.Count > 0 && stacksData.Peek() != "(")
                            {
                                sbResult.Append(stacksData.Pop() + " ");
                            }

                            //Pop the "("
                            stacksData.Pop();
                            continue;
                        }
                        if (GetOperatorLevel[sCurrent] < GetOperatorLevel[varPrevious])
                        {
                            while (stacksData.Count > 0)
                            {
                                var varTop = stacksData.Pop();

                                if (varTop != "(" && varTop != ")")
                                {
                                    sbResult.Append(varTop + " ");
                                }
                                else
                                {
                                    break;
                                }
                            }
                            stacksData.Push(sCurrent);
                        }
                        else
                        {
                            stacksData.Push(sCurrent);
                        }
                    }
                    else
                    {
                        stacksData.Push(sCurrent);
                    }
                }
                else
                {
                    bErrorFlag = true;
                    sErrorMessage = "Error String Format";
                    return sbResult.ToString();
                }
            }

            if (stacksData.Count > 0)
            {
                while (stacksData.Count > 0)
                {
                    var varTop = stacksData.Pop();

                    if (varTop != "(" && varTop != ")")
                    {
                        sbResult.Append(varTop + " ");
                    }
                }
            }

            return sbResult.ToString();
        }

        public static string InsertBlank(string sSourceString)
        {
            StringBuilder sbResult = new StringBuilder();
            var varArray = sSourceString.ToCharArray();

            foreach (var vValue in varArray)
            {
                if (GetOperatorLevel.ContainsKey(vValue.ToString()))
                {
                    sbResult.Append(" ");
                    sbResult.Append(vValue.ToString());
                    sbResult.Append(" ");
                }
                else
                {
                    sbResult.Append(vValue);
                }
            }

            return sbResult.ToString();
        }

        //運算符字典 方便查詢運算符優先級
        private static Dictionary<string, int> GetOperatorLevel
        {
            get
            {
                if (m_dictOperatorLevel == null)
                {
                    m_dictOperatorLevel = new Dictionary<string, int>();
                    m_dictOperatorLevel.Add("+", 0);
                    m_dictOperatorLevel.Add("-", 0);
                    m_dictOperatorLevel.Add("(", 1);
                    m_dictOperatorLevel.Add("*", 1);
                    m_dictOperatorLevel.Add("/", 1);
                    m_dictOperatorLevel.Add(")", 0);
                }

                return m_dictOperatorLevel;
            }
        }
    }
}

