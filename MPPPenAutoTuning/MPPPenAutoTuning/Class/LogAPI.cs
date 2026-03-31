using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BlockingQueue;
using System.Threading;
using System.Windows.Forms;

namespace MPPPenAutoTuning
{
    public class LogAPI
    {
        private frmMain m_cfrmMain;

        private string m_sDebugLogFolderPath = null;
        private string m_sLogFilePath = null;
        private FileStream m_fsLog = null;
        private StreamWriter m_swLog = null;

        public LogAPI(frmMain cfrmMian)
        {
            m_cfrmMain = cfrmMian;
        }

        public void WriteLogToBuffer(string sMessage)
        {
            string sRecordData = string.Format("{0} | {1}", DateTime.Now.ToString("MM-dd HH:mm:ss.fff"), sMessage);
            m_cfrmMain.m_qsLogBuffer.Enqueue(sRecordData);
            return;
        }

        public void WriteLogToFileThread()
        {
            m_sDebugLogFolderPath = string.Format(@"{0}\\{1}\\DebugLog", Application.StartupPath, frmMain.m_sAPMainDirectoryName);

            if (Directory.Exists(m_sDebugLogFolderPath) == false)
                Directory.CreateDirectory(m_sDebugLogFolderPath);

            m_sLogFilePath = string.Format(@"{0}\\DebugLog_{1}.txt", m_sDebugLogFolderPath, m_cfrmMain.m_sProgramEnterTime);
            m_fsLog = new FileStream(m_sLogFilePath, FileMode.Append);
            m_swLog = new StreamWriter(m_fsLog);

            while (true)
            {
                if (m_cfrmMain.m_qsLogBuffer.Count > 0)
                {
                    string sLogBuffer = "";

                    if (m_cfrmMain.m_qsLogBuffer.Dequeue(50, ref sLogBuffer))
                    {
                        m_swLog.WriteLine(sLogBuffer);
                        m_swLog.Flush();
                    }
                }

                Thread.Sleep(10);
                //Thread.Sleep(1000);
            }
        }

        public void CloseLogFile()
        {
            while (m_cfrmMain.m_qsLogBuffer.Count > 0)
            {
                string sLogBuffer = "";

                if (m_cfrmMain.m_qsLogBuffer.Dequeue(50, ref sLogBuffer))
                {
                    m_swLog.WriteLine(sLogBuffer);
                }
            }

            m_swLog.Flush();
            m_swLog.Close();
            m_fsLog.Close();
        }
    }
}
