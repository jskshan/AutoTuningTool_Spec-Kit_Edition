using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        #region Chrome Remote Client 連線

        /// <summary>
        /// 連接Chrome Remote Client,設定位址及Port,若已連接則先斷線,建立Socket連接並等待回調完成,若啟用則執行Finger Report測試
        /// </summary>
        /// <returns>Socket連接成功、回調完成且所有流程完成回傳true;連接失敗、使用者中斷執行或Finger Report測試中斷則回傳false並設定連接狀態為false</returns>
        private bool ConnectChromeRemoteClient()
        {
            m_nDeviceIndex = 0;
            m_bSocketWaitCallback = false;

            int nAddress = 0;
            //Int32.TryParse(tbPID.Text, out nPort);
            int nPort = ParamFingerAutoTuning.m_nPort == 0 ? ElanDefine.DEFAULT_SOCKET_PORT : ParamFingerAutoTuning.m_nPort;

            OutputMessage(string.Format("-Address={0}, Port={1}, DeviceIndex={2}", nAddress, nPort, m_nDeviceIndex));

            //Do disconnect...
            if (m_bSocketConnected)
                ElanTouchSwitch.Disconnect(m_bSocketConnectType);

            OutputMessage("-Start Socket Event CallBack");

            // 建立 Socket 連線
            int nResultFlag = EstablishSocketConnection(0, nPort);

            //if (nRet != ElanTouch_Socket.TP_SUCCESS)
            if (!ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType))
            {
                m_bTPConnected = false;
                m_bSocketConnected = false;
                m_sErrorMessage = "Start Socket Server Failed";
                return false;
            }

            m_bTPConnected = true;
            m_bSocketConnected = true;

            /*
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunSSHRemoteSocket), null);

            while (m_bSocketWaitCallback == false)
            {
                if (m_cfrmParent.m_bExecute == false)
                {
                    m_bTPConnected = false;
                    m_bSocketConnected = false;
                    return false;
                }

                Thread.Sleep(10);
            }
            */

            // 等待 Callback
            int nCount = 0;
            while (!m_bSocketWaitCallback)
            {
                if (!m_cfrmParent.m_bExecute)
                {
                    m_bTPConnected = false;
                    m_bSocketConnected = false;
                    return false;
                }

                if (nCount % 100 == 0)
                    OutputMessage(string.Format("-Wait Socket Callback[Count={0}]", nCount));

                try
                {
                    nCount++;
                }
                catch (OverflowException)
                {
                    nCount = 0;
                }

                Thread.Sleep(20);
            }

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show(m_cfrmParent, "Continue to Connect Chrome Device");
            });

            // 處理 Finger Report 測試
            if (ParamFingerAutoTuning.m_nFingerReportTest == 1)
            {
                SaveFingerReport();
                if (!m_cfrmParent.m_bExecute) return false;
            }

            return true;
        }

        #endregion
    }
}
