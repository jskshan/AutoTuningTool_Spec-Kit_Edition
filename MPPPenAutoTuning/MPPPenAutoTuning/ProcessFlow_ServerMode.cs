using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MPPPenAutoTuning
{
    public partial class ProcessFlow
    {
        /// <summary>
        /// 啟動Server端流程
        /// </summary>
        private void RunServerListenFlow()
        {
            //Socket Start Listening
            m_tsSocket = new ThreadStart(RunServerListenThread);
            m_tSocket = new Thread(m_tsSocket);
            m_tSocket.IsBackground = true;

            m_tSocket.Start();
        }

        /// <summary>
        /// 啟動Server端監聽
        /// </summary>
        private void RunServerListenThread()
        {
            m_cSocket.RunServerStarting();
            m_cSocket.RunServerListening();
            m_cRobot.ClosePort();

            if (m_bFormCloseFlag == false)
            {
                if (m_bForceStopFlag == true)
                {
                    OutputStateMessage(m_sErrorMessage);
                    OutputMessage(string.Format("-{0}", m_sErrorMessage));
                }
                else
                    OutputStateMessage(m_sErrorMessage, false, true, false, true);

                SetNewConnectButton(true);
                SetNewStopButton(false);
                SetModeStateComboBoxAndSettingToolStripMenuItem(true);
            }

            if (m_tRobot != null && m_tRobot.IsAlive == true)
            {
                m_tRobot.Abort();
                m_tRobot.Join();
                m_tRobot = null;
            }

            if (m_tListenThread.IsAlive == true)
            {
                m_tListenThread.Abort();
                m_tListenThread.Join();
                m_tListenThread = null;
            }
        }
    }
}
