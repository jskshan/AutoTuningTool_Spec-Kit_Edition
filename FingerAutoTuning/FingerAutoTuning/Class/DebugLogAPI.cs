using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FingerAutoTuning
{
    class DebugLogAPI
    {
        private string sDebugLogFolderPath = null;
        private string sLogFilePath = null;
        private FileStream fsLogFileStream = null;
        private StreamWriter swWriteLogFileStream = null;

        public void WriteLogToBuffer(string sMessage)
        {
            string sWriteMessage = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
            sWriteMessage += string.Format(" | {0}", sMessage);
            frmMain.m_bqsDebugLogBuffer.Enqueue(sWriteMessage);

            return;
        }

        public void WriteLogToFile()
        {
            sDebugLogFolderPath = string.Format(@"{0}\{1}\DebugLog", Application.StartupPath, frmMain.m_sAPMainDirectoryName);

            if (Directory.Exists(sDebugLogFolderPath) == false)
                Directory.CreateDirectory(sDebugLogFolderPath);

            sLogFilePath = string.Format(@"{0}\DebugLog_{1}.txt", sDebugLogFolderPath, frmMain.m_sProgramStartTime.ToString());
            fsLogFileStream = new FileStream(sLogFilePath, FileMode.Append);
            swWriteLogFileStream = new StreamWriter(fsLogFileStream);

            while (true)
            {
                if (frmMain.m_bqsDebugLogBuffer.Count > 0)
                {
                    string sDebugLogBuffer = "";

                    if (frmMain.m_bqsDebugLogBuffer.Dequeue(50, ref sDebugLogBuffer))
                    {
                        swWriteLogFileStream.WriteLine(sDebugLogBuffer);
                        swWriteLogFileStream.Flush();
                    }
                }

                Thread.Sleep(10);
            }
        }

        public void CloseLogFile()
        {
            while (frmMain.m_bqsDebugLogBuffer.Count > 0)
            {
                string sLogBuffer = "";
                if (frmMain.m_bqsDebugLogBuffer.Dequeue(50, ref sLogBuffer))
                {
                    swWriteLogFileStream.WriteLine(sLogBuffer);
                }
            }

            swWriteLogFileStream.Flush();
            swWriteLogFileStream.Close();
            fsLogFileStream.Close();
        }
    }
}
