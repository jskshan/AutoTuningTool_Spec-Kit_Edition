using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elan;
using System.Windows.Forms;
using System.IO;

namespace FingerAutoTuning
{
    class ADBFileIO
    {
        #region Fields
        private string m_sDevFilePath = "";
        private string m_sHostFilePath = "";
        #endregion

        public ADBFileIO(string sDevFilePath, string sHostFilePath)
        {
            m_sDevFilePath = sDevFilePath;
            m_sHostFilePath = sHostFilePath;
        }

        public bool Push()
        {
            ElanBatchProcess_9F07 AdbPush = new ElanBatchProcess_9F07(string.Format(@"{0}\RemoteClient\adb.exe", Application.StartupPath), string.Format("push \"{0}\" {1}", m_sHostFilePath, m_sDevFilePath), "", true);
            string sLine = "";
            bool bPushResult = false;

            AdbPush.Start(false);

            while (true)
            {
                if (AdbPush.ReadOutputText(ref sLine, 1000) == false)
                {
                    if (AdbPush.IsRunning == false)
                        break;

                    continue;
                }

                if (sLine == null)
                    continue;

                if (sLine.IndexOf("pushed") != -1)
                {
                    bPushResult = true;
                    break;
                }

                Console.WriteLine(sLine);
            }

            return bPushResult;
        }

        public bool Pull()
        {
            ElanBatchProcess_9F07 AdbPull = new ElanBatchProcess_9F07(string.Format(@"{0}\RemoteClient\adb.exe", Application.StartupPath), string.Format("pull {0} \"{1}\"", m_sDevFilePath, m_sHostFilePath), "", true);
            string sLine = "";
            bool bPullResult = false;

            AdbPull.Start(false);

            while (true)
            {
                if (AdbPull.ReadOutputText(ref sLine, 1000) == false)
                {
                    if (AdbPull.IsRunning == false)
                        break;

                    continue;
                }

                if (sLine == null)
                    continue;

                if (sLine.IndexOf("pulled") != -1)
                {
                    bPullResult = true;
                    break;
                }

                Console.WriteLine(sLine);
            }

            return bPullResult;
        }
    }
}
