using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nRetryIndex"></param>
        /// <param name="sTracePart"></param>
        private void GetReportData(int nRetryIndex, string sTracePart = MainConstantParameter.m_sAllTrace)
        {
            OutputMessage(string.Format("-Get Report Data(RetryIndex={0})", nRetryIndex));

            byte byteType = 0x01;

            if (ParamFingerAutoTuning.m_nSelfFSGetReportType != 1)
                byteType = Convert.ToByte(ParamFingerAutoTuning.m_nSelfFSGetReportByte);
            else
            {
                switch (sTracePart)
                {
                    case MainConstantParameter.m_sOddTrace:
                        byteType = 0x01;
                        break;
                    case MainConstantParameter.m_sEvenTrace:
                        byteType = 0x02;
                        break;
                    case MainConstantParameter.m_sForwardTrace:
                        byteType = 0x03;
                        break;
                    case MainConstantParameter.m_sBackwardTrace:
                        byteType = 0x04;
                        break;
                    default:
                        break;
                }
            }

            SendGetSelfReportData(true, byteType);

            m_bStartRecord = true;

            int nTimeout = ParamFingerAutoTuning.m_nSelfFSGetReportTimeout;

            if (ParamFingerAutoTuning.m_nSelfFSGetReportTimeout <= 0)
                nTimeout = 3600000;

            long lStartTime = DateTime.Now.Ticks;
            long lIntervalStartTime = lStartTime;
            long lCurrentTime = 0;
            int nCostTime = 0;
            int nReportCount = 0;

            while (nCostTime <= nTimeout)
            {
                lCurrentTime = DateTime.Now.Ticks;
                nCostTime = (int)((lCurrentTime - lStartTime) / 10000);

                #region Update the Report Rate
                long nTimeInterval = (lCurrentTime - lIntervalStartTime) / 10000;

                if (nTimeInterval >= 200)
                {
                    lock (this)
                    {
                        int nTotalReport = m_cSaveData.GetRecordCount(); //m_listbyteReport.Count;
                        int nIntervalCount = nTotalReport - nReportCount;

                        int nMiniSecond = nCostTime % 1000;
                        int nSecond = nCostTime / 1000;

                        string sMessage = string.Format("Report Count:{0}, Rate:{1}Hz, Time:{2}.{3:000}s", nTotalReport,
                                                        (int)(nIntervalCount * (1000.0f / nTimeInterval)), nSecond, nMiniSecond);

                        if (ParamFingerAutoTuning.m_nSelfFSRunKSequence == 1 ||
                            ParamFingerAutoTuning.m_nSelfFSRunKSequence == 2)
                            sMessage = string.Format("{0} (NCP={1} NCN={2})", sMessage, m_nSelfNCPValue, m_nSelfNCNValue);

                        m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                        {
                            m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, sMessage);
                            SetDisplayMessage(sMessage);
                            //OutputMessage(string.Format("-{0}", sMessage));
                        });

                        lIntervalStartTime = lCurrentTime;  //DateTime.Now.Ticks;
                        nReportCount = nTotalReport;

                        if (nTotalReport >= ParamFingerAutoTuning.m_nSelfFSReportNumber)
                            break;

                        if (m_cfrmParent.m_bExecute == false)
                            break;
                    }
                }
                #endregion

                Thread.Sleep(20);
            }

            //Finish the log procedure
            m_bStartRecord = false;

            SetTestModeEnable(true);
            SendGetSelfReportData(false);
            Thread.Sleep(100);
            SendGetSelfReportData(false);
            SetTestModeEnable(false);
            SetReportEnable(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nRetryIndex"></param>
        /// <returns></returns>
        private bool CheckReportExist(int nRetryIndex)
        {
            OutputMessage(string.Format("-Check Report Exist(RetryIndex={0})", nRetryIndex));

            m_bStartRecord = true;
            m_cSaveData.ResetRecordCount();

            int nTimeout = 1000;
            long lStartTime = DateTime.Now.Ticks;
            long lIntervalStartTime = lStartTime;
            long lCurrentTime = 0;
            int nCostTime = 0;
            bool bReportExist = false;

            while (nCostTime <= nTimeout)
            {
                lCurrentTime = DateTime.Now.Ticks;
                nCostTime = (int)((lCurrentTime - lStartTime) / 10000);

                #region Update the Report Rate
                long nTimeInterval = (lCurrentTime - lIntervalStartTime) / 10000;

                if (nTimeInterval >= 200)
                {
                    lock (this)
                    {
                        int nRecordCount = m_cSaveData.GetRecordCount();

                        if (nRecordCount > 0)
                        {
                            bReportExist = true;
                            break;
                        }

                        if (m_cfrmParent.m_bExecute == false)
                            break;
                    }
                }
                #endregion

                Thread.Sleep(20);
            }

            //Finish the log procedure
            m_bStartRecord = false;

            if (bReportExist == true)
            {
                OutputMessage("-Report Data Exist");

                SetTestModeEnable(true);
                SendGetSelfReportData(false);
                Thread.Sleep(100);
                SendGetSelfReportData(false);
                SetTestModeEnable(false);

                return true;
            }
            else
                OutputMessage("-Report Data Not Exist");

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveFingerReport()
        {
            FileStream fs = new FileStream(@"FingerReport.txt", FileMode.Create);

            //Start to log the data
            m_bStartRecord = true;

            //Create new file to log the report data
            lock (m_objReportLocker)
            {
                m_swReportLog = new StreamWriter(fs);
            }

            while (m_cfrmParent.m_bExecute == true)
            {
                Thread.Sleep(20);
            }

            //Finish the log procedure
            m_bStartRecord = false;

            lock (m_objReportLocker)
            {
                m_swReportLog.Flush();
                m_swReportLog.Close();
                m_swReportLog = null;
                fs.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMessage"></param>
        private void SetDisplayMessage(string sMessage)
        {
            if (File.Exists(ParamFingerAutoTuning.m_sPatternPicPath) == true ||
                ParamFingerAutoTuning.m_nPatternType == 2)
            {
                if (m_cfrmFullScreen != null)
                    m_cfrmFullScreen.SetMessageText(sMessage);
            }
            else if (ParamFingerAutoTuning.m_nPatternType == 1)
            {
                if (m_cfrmPHCKPattern != null)
                    m_cfrmPHCKPattern.SetMessageText(sMessage);
            }
        }
    }
}
