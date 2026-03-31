using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Data;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nStepIndex"></param>
        /// <param name="bFlowComplete"></param>
        private void SetOutputResult(int nStepIndex, bool bFlowComplete)
        {
            SetStepCostTime(nStepIndex, true);

            RunOutputResult(bFlowComplete);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bFlowComplete"></param>
        private void RunOutputResult(bool bFlowComplete)
        {
            if (m_cfrmParent.m_bLoadData == false)
            {
                if (ParamFingerAutoTuning.m_nKeepDoNotReset == 1 && m_bKeepNotReset == true)
                {
                    OutputResult(bFlowComplete, m_bLastRun);
                    RunFinishAndRecovery();
                }
                else
                {
                    RunFinishAndRecovery();
                    OutputResult(bFlowComplete, m_bLastRun);
                }
            }
            else
                OutputResult(bFlowComplete, m_bLastRun);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nStepIndex"></param>
        /// <param name="bSetLastStep"></param>
        private void SetStepCostTime(int nStepIndex, bool bSetLastStep = false)
        {
            if (bSetLastStep == true)
                m_cFlowStep_List[nStepIndex].m_bLastStep = true;

            TimeSpan tsTimeSpan = m_swSingleStep.Elapsed;
            TimeSpan tsStepTimeSpan = tsTimeSpan - m_tsPreviousStepTimeSpan;

            m_tsPreviousStepTimeSpan = tsTimeSpan;

            int nDayToHourOffset = tsStepTimeSpan.Days * 24;
            int nRealHours = tsStepTimeSpan.Hours + nDayToHourOffset;

            m_cFlowStep_List[nStepIndex].SetCostTimeParameter(nRealHours, tsStepTimeSpan.Minutes, tsStepTimeSpan.Seconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bFlowComplete"></param>
        /// <param name="bLastRun"></param>
        private void OutputResult(bool bFlowComplete, bool bLastRun)
        {
            if (m_bCopyDataError == true || m_bMoveDataError == true)
            {
                //Directory.Delete(m_cfrmParent.m_sRecordLogDirectoryPath, true);
                DeleteDirectory(m_cfrmParent.m_sRecordLogDirectoryPath);

                while (Directory.Exists(m_cfrmParent.m_sRecordLogDirectoryPath))
                    Thread.Sleep(10);

                if (m_bGenerateH5Data == true)
                {
                    DeleteDirectory(m_cfrmParent.m_sH5RecordLogDirectoryPath);

                    while (Directory.Exists(m_cfrmParent.m_sH5RecordLogDirectoryPath))
                        Thread.Sleep(10);
                }
            }

            if (bFlowComplete == false || m_sErrorMessage != "")
            {
                m_cfrmParent.Invoke((MethodInvoker)delegate
                {
                    OutputMessage(string.Format("[Result]Error : {0}", m_sErrorMessage));
                    OutputCostTime();
                    SetAndOutputTotalCostTime();

                    if (m_bCopyDataError == false && m_bMoveDataError == false)
                        WriteResultTxt(false, m_sErrorMessage);

                    OutputlblStatus("Error", m_sErrorMessage, Color.Red, false);
                    m_cfrmParent.SetButton(frmMain.FlowState.Finish);
                    m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, "Finish");
                });
            }
            else
            {
                m_cfrmParent.Invoke((MethodInvoker)delegate
                {
                    OutputMessage("[Result]Complete");
                    OutputCostTime();
                    SetAndOutputTotalCostTime();

                    if (m_bCopyDataError == false && m_bMoveDataError == false)
                        WriteResultTxt(true);

                    OutputlblStatus("Complete", "", Color.Green, false, true);

                    if (bLastRun == true)
                        m_cfrmParent.SetButton(frmMain.FlowState.Finish);

                    m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, "Finish");
                });

                m_cfrmParent.m_nCurrentExecuteIndex = 0;
            }

            m_swSingleStep.Stop();
        }

       /// <summary>
       /// Depth-first recursive delete, with handling for descendant 
       /// directories open in Windows Explorer.
       /// </summary>
       /// <param name="sDirectoryPath"></param>
        private static void DeleteDirectory(string sDirectoryPath)
        {
            foreach (string sSubDirectoryPath in Directory.GetDirectories(sDirectoryPath))
            {
                DeleteDirectory(sSubDirectoryPath);
            }

            try
            {
                Directory.Delete(sDirectoryPath, true);
            }
            catch (IOException)
            {
                Directory.Delete(sDirectoryPath, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(sDirectoryPath, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OutputCostTime()
        {
            bool bLastStep = false;

            for (int nStepIndex = m_nStartIndex; nStepIndex < m_cFlowStep_List.Count; nStepIndex++)
            {
                if ((bLastStep == false) ||
                    (m_cFlowStep_List[nStepIndex].m_nCostTime_Hour != -1 ||
                     m_cFlowStep_List[nStepIndex].m_nCostTime_Minute != -1 ||
                     m_cFlowStep_List[nStepIndex].m_nCostTime_Second != -1))
                {
                    string sStepCostTime = string.Format("[CostTime]{0} Step = {1}hr:{2}m:{3}s",
                                                         m_cFlowStep_List[nStepIndex].m_sStepName,
                                                         m_cFlowStep_List[nStepIndex].m_nCostTime_Hour.ToString().PadLeft(2, '0'),
                                                         m_cFlowStep_List[nStepIndex].m_nCostTime_Minute.ToString().PadLeft(2, '0'),
                                                         m_cFlowStep_List[nStepIndex].m_nCostTime_Second.ToString().PadLeft(2, '0'));

                    OutputMessage(sStepCostTime);
                }
                else
                {
                    string sStepCostTime = string.Format("[CostTime]{0} Step = N/A", m_cFlowStep_List[nStepIndex].m_sStepName);

                    OutputMessage(sStepCostTime);
                }


                if (m_cFlowStep_List[nStepIndex].m_bLastStep == true)
                    bLastStep = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetAndOutputTotalCostTime()
        {
            TimeSpan tsTimeSpan = m_swSingleStep.Elapsed;

            int nDayToHourOffset = tsTimeSpan.Days * 24;
            int nRealHours = tsTimeSpan.Hours + nDayToHourOffset;

            m_cTotalTimerCount.SetTimerParameter(nRealHours, tsTimeSpan.Minutes, tsTimeSpan.Seconds);

            string sTotalCostTime = string.Format("[CostTime]Total Cost Time = {0}hr:{1}m:{2}s",
                                                  m_cTotalTimerCount.m_nHours.ToString().PadLeft(2, '0'),
                                                  m_cTotalTimerCount.m_nMinutes.ToString().PadLeft(2, '0'),
                                                  m_cTotalTimerCount.m_nSeconds.ToString().PadLeft(2, '0'));
            OutputMessage(sTotalCostTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bComplete"></param>
        /// <param name="sErrorMessage"></param>
        private void WriteResultTxt(bool bComplete, string sErrorMessage = "")
        {
            string sDirectoryPath = m_cfrmParent.m_sRecordLogDirectoryPath;

            if (m_cfrmParent.m_sRecordLogDirectoryPath == m_cfrmParent.m_sLogDirectoryPath)
            {
                sDirectoryPath = string.Format(@"{0}\{1}", m_cfrmParent.m_sLogDirectoryPath, "Other");

                Directory.CreateDirectory(sDirectoryPath);

                while (Directory.Exists(sDirectoryPath) == false)
                    Thread.Sleep(10);
            }

            if (Directory.Exists(sDirectoryPath) == true)
            {
                string sResultFilePath = string.Format(@"{0}\Result.txt", sDirectoryPath);

                FileStream fs = new FileStream(sResultFilePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                try
                {
                    sw.WriteLine("[Result]");

                    if (bComplete == true)
                        sw.WriteLine("Result = Complete");
                    else
                    {
                        sw.WriteLine("Result = Error");
                        sw.WriteLine(string.Format("ErrorMessage = {0}", sErrorMessage));
                    }

                    sw.WriteLine();
                    sw.WriteLine("[CostTime]");

                    bool bLastStep = false;

                    for (int nStepIndex = m_nStartIndex; nStepIndex < m_cFlowStep_List.Count; nStepIndex++)
                    {
                        if ((bLastStep == false) ||
                            (m_cFlowStep_List[nStepIndex].m_nCostTime_Hour != -1 ||
                             m_cFlowStep_List[nStepIndex].m_nCostTime_Minute != -1 ||
                             m_cFlowStep_List[nStepIndex].m_nCostTime_Second != -1))
                        {
                            string sStepCostTime = string.Format("{0} Step = {1}hr:{2}m:{3}s",
                                                                 m_cFlowStep_List[nStepIndex].m_sStepName,
                                                                 m_cFlowStep_List[nStepIndex].m_nCostTime_Hour.ToString().PadLeft(2, '0'),
                                                                 m_cFlowStep_List[nStepIndex].m_nCostTime_Minute.ToString().PadLeft(2, '0'),
                                                                 m_cFlowStep_List[nStepIndex].m_nCostTime_Second.ToString().PadLeft(2, '0'));

                            sw.WriteLine(sStepCostTime);
                        }
                        else
                        {
                            string sStepCostTime = string.Format("{0} Step = N/A", m_cFlowStep_List[nStepIndex].m_sStepName);

                            sw.WriteLine(sStepCostTime);
                        }


                        if (m_cFlowStep_List[nStepIndex].m_bLastStep == true)
                            bLastStep = true;
                    }

                    string sTotalCostTime = string.Format("Total Cost Time = {0}hr:{1}m:{2}s",
                                                          m_cTotalTimerCount.m_nHours.ToString().PadLeft(2, '0'),
                                                          m_cTotalTimerCount.m_nMinutes.ToString().PadLeft(2, '0'),
                                                          m_cTotalTimerCount.m_nSeconds.ToString().PadLeft(2, '0'));

                    sw.WriteLine(sTotalCostTime);
                }
                catch (IOException ex)
                {
                    string sMessage = ex.Message;
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }

                if (m_bGenerateH5Data == true)
                {
                    string sSourceFilePath = sResultFilePath;
                    string sDestinationFilePath = string.Format(@"{0}\Result.txt", m_cfrmParent.m_sH5RecordLogDirectoryPath);

                    AppCoreDefine.FileProcessFlow(AppCoreDefine.FileProcess.Copy, sSourceFilePath, sDestinationFilePath, false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <returns></returns>
        private bool OutputResultData(frmMain.FlowStep cFlowStep)
        {
            if (m_sErrorMessage != "")
                return false;

            if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase1)
            {
                string sReportFilePath = string.Format(@"{0}\Report.csv", m_sLogDirectoryPath);

                if (File.Exists(sReportFilePath) == false)
                {
                    m_sErrorMessage = "Read Report File Error";
                    return false;
                }

                int nResultLineCount = 0;
                int nWarningLineCount = 0;
                bool bGetResultData = false;
                bool bGetWarningData = false;
                string sLine = "";
                string sOutputResultMessage = "";
                string sOutputWarningMessage = "";

                StreamReader srReadFile = new StreamReader(sReportFilePath, Encoding.Default);

                try
                {
                    while ((sLine = srReadFile.ReadLine()) != null)
                    {
                        // if (sLine == "Result:")
                        if (sLine.Contains("Result"))
                        {
                            bGetResultData = true;
                            bGetWarningData = false;
                        }
                        else if (sLine.Contains("Warning"))
                        {
                            bGetWarningData = true;
                            bGetResultData = false;
                        }

                        if (bGetResultData)
                        {
                            if (nResultLineCount > 0 && !string.IsNullOrWhiteSpace(sLine))
                                sOutputResultMessage += string.Format("({0})", nResultLineCount);

                            if (!string.IsNullOrWhiteSpace(sLine))
                            {
                                string[] sSubString_Array = sLine.Split(',');

                                for (int nIndex = 0; nIndex < sSubString_Array.Length; nIndex++)
                                {
                                    if (nIndex < sSubString_Array.Length - 1)
                                        sOutputResultMessage += string.Format("{0} ", sSubString_Array[nIndex]);
                                    else
                                        sOutputResultMessage += sSubString_Array[nIndex];
                                }
                            }

                            sOutputResultMessage += Environment.NewLine;

                            nResultLineCount++;
                        }

                        if (bGetWarningData)
                        {
                            if (nWarningLineCount > 0 && !string.IsNullOrWhiteSpace(sLine))
                                sOutputWarningMessage += string.Format("({0})", nWarningLineCount);

                            if (!string.IsNullOrWhiteSpace(sLine))
                            {
                                string[] sSubString_Array = sLine.Split(',');

                                for (int nIndex = 0; nIndex < sSubString_Array.Length; nIndex++)
                                {
                                    if (nIndex < sSubString_Array.Length - 1)
                                        sOutputWarningMessage += string.Format("{0}, ", sSubString_Array[nIndex]);
                                    else
                                        sOutputWarningMessage += sSubString_Array[nIndex];
                                }
                            }

                            sOutputWarningMessage += Environment.NewLine;

                            nWarningLineCount++;
                        }
                    }
                }
                finally
                {
                    srReadFile.Close();
                }

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.OutputrtbxFRPH1(sOutputResultMessage, sOutputWarningMessage);
                    m_cfrmParent.EnableButton(frmMain.ButtonType.btnFRPH1Chart, true);
                });
            }
            else if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase2)
            {
                string sReportFilePath = string.Format(@"{0}\Report.csv", m_sLogDirectoryPath);

                if (File.Exists(sReportFilePath) == false)
                {
                    m_sErrorMessage = "Read Report File Error";
                    return false;
                }

                int nMinPH1 = -1;
                int nPH2LB = -1;

                if (GetAnalogParameter(ref nMinPH1, ref nPH2LB, sReportFilePath) == false)
                {
                    m_sErrorMessage = "Read Analog Parameter Error in Report File";
                    return false;
                }

                DataTable datatableFrequencyRank = StringConvert.ConvertCsvToDataTable(sReportFilePath, "Frequency Rank:");

                string[] sRemove_Array = new string[] 
                { 
                    "Signal RefValue", 
                    "Noise Mean", 
                    "Noise Std", 
                    "Noise PosRef", 
                    "Noise NegRef",
                    "Noise RefValue", 
                    "RawSNR RefValue" 
                };

                for (int nIndex = 0; nIndex < sRemove_Array.Length; nIndex++)
                {
                    if (datatableFrequencyRank.Columns.Contains(sRemove_Array[nIndex]) == true)
                        datatableFrequencyRank.Columns.Remove(sRemove_Array[nIndex]);
                }

                if (ParamFingerAutoTuning.m_nFRPH2NormalizeType == 1)
                    datatableFrequencyRank.Columns.Add("ColorGradient");

                //datatableFrequencyRank.Columns["Frequency(KHz)"].ColumnName = "Frequency";

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    //m_cfrmParent.SetDetailDGVBind(datatableFRDataTable, nMinPH1, nPH2LB);
                    m_cfrmParent.OutputDataGridView(frmMain.DataGridViewType.dgvFRPH2, datatableFrequencyRank);
                    m_cfrmParent.EnableButton(frmMain.ButtonType.btnFRPH2Chart, true);
                });
            }
            else if (cFlowStep.m_eStep == MainStep.AC_FrequencyRank)
            {
                string sReportFilePath = string.Format(@"{0}\Report.csv", m_sLogDirectoryPath);

                if (File.Exists(sReportFilePath) == false)
                {
                    m_sErrorMessage = "Read Report File Error";
                    return false;
                }

                DataTable datatableFrequencyRank = StringConvert.ConvertCsvToDataTable(sReportFilePath, "Frequency Rank:");

                string[] sRemove_Array = new string[] 
                { 
                    "Signal RefValue", 
                    "AC Noise Mean", 
                    "AC Noise Std", 
                    "AC Noise PosRef", 
                    "AC Noise NegRef",
                    "LCM Noise Mean", 
                    "LCM Noise Std", 
                    "LCM Noise PosRef", 
                    "LCM Noise NegRef" 
                };

                for (int nIndex = 0; nIndex < sRemove_Array.Length; nIndex++)
                {
                    if (datatableFrequencyRank.Columns.Contains(sRemove_Array[nIndex]) == true)
                        datatableFrequencyRank.Columns.Remove(sRemove_Array[nIndex]);
                }

                datatableFrequencyRank.Columns.Add("ColorGradient");

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.OutputDataGridView(frmMain.DataGridViewType.dgvACFR, datatableFrequencyRank);
                    m_cfrmParent.EnableButton(frmMain.ButtonType.btnACFRChart, true);
                });
            }
            else if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                string sReportFilePath = string.Format(@"{0}\Report.csv", m_sLogDirectoryPath);

                if (File.Exists(sReportFilePath) == false)
                {
                    m_sErrorMessage = "Read Report File Error";
                    return false;
                }

                DataTable datatableRawADCSweep = StringConvert.ConvertCsvToDataTable(sReportFilePath, "Raw ADC PCT Rank Information", true);

                string[] sRemove_Array = new string[] 
                { 
                    "c_MS_IQ_BSH_0",
                    "_IQ_BSH0",
                    "_IQ_BSH",
                    "DFT_NUM",
                    "Column1",
                    "Column2"
                };

                for (int nIndex = 0; nIndex < sRemove_Array.Length; nIndex++)
                {
                    if (datatableRawADCSweep.Columns.Contains(sRemove_Array[nIndex]) == true)
                        datatableRawADCSweep.Columns.Remove(sRemove_Array[nIndex]);
                }

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.OutputDataGridView(frmMain.DataGridViewType.dgvRawADCS, datatableRawADCSweep);
                });
            }
            else if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep)
            {
                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.EnableButton(frmMain.ButtonType.btnSelfFSChart, true);
                });
            }
            else if (cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                string sOutputMessage = "";

                foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
                {
                    string sTableFilePath = string.Format(@"{0}\NCPNCNTable_{1}.txt", m_sLogDirectoryPath, eTraceType.ToString());

                    if (File.Exists(sTableFilePath) == false)
                    {
                        m_sErrorMessage = "Read Table File Error";
                        return false;
                    }

                    string sLine = "";

                    StreamReader srReadFile = new StreamReader(sTableFilePath, Encoding.Default);

                    try
                    {
                        while ((sLine = srReadFile.ReadLine()) != null)
                            sOutputMessage += sLine + Environment.NewLine;
                    }
                    finally
                    {
                        srReadFile.Close();
                    }
                }

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmParent.OutputrtbxSelfPNS(sOutputMessage);
                });
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMinPH1"></param>
        /// <param name="nPH2LB"></param>
        /// <param name="sFilePath"></param>
        /// <returns></returns>
        private bool GetAnalogParameter(ref int nMinPH1, ref int nPH2LB, string sFilePath)
        {
            string sLine = "";

            StreamReader srReadFile = new StreamReader(sFilePath, Encoding.Default);

            try
            {
                while ((sLine = srReadFile.ReadLine()) != null)
                {
                    string[] sSubString_Array = sLine.Split(',');

                    if (sSubString_Array.Length >= 2)
                    {
                        if (sSubString_Array[0] == "MinPH1(Hex)")
                            nMinPH1 = Convert.ToInt32(sSubString_Array[1], 16);
                        else if (sSubString_Array[0] == "PH2LB(Hex)")
                            nPH2LB = Convert.ToInt32(sSubString_Array[1], 16);
                    }

                    if (nMinPH1 > -1 && nPH2LB > -1)
                        break;
                }
            }
            finally
            {
                srReadFile.Close();
            }

            if (nMinPH1 == -1 || nPH2LB == -1)
                return false;

            return true;
        }
    }
}
