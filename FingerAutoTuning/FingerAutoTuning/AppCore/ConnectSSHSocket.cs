using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        #region SSH Socket 連線

        /// <summary>
        /// 連接SSH Socket(Chrome或Other類型),包含重試機制、Socket連接、SSH連線建立及等待連線完成的完整流程
        /// </summary>
        /// <returns>SSH Socket連接成功並完成回調回傳true;連接失敗或重試次數用盡則回傳false並設定錯誤訊息</returns>
        private bool ConnectSSHSocket()
        {
            bool bConnectFailFlag = true;

            for (int nRetryIndex = 0; nRetryIndex <= 3; nRetryIndex++)
            {
                DisconnectElanSSHClient();
                ElanTouchSwitch.SetGetParameterTimeout();
                m_nDeviceIndex = 0;
                m_bSocketWaitCallback = false;

                int nAddress = 0;
                int nPort = ParamFingerAutoTuning.m_nPort == 0 ? ElanDefine.DEFAULT_SOCKET_PORT : ParamFingerAutoTuning.m_nPort;

                OutputMessage(string.Format("-Address={0}, Port={1}, DeviceIndex={2}", nAddress, nPort, m_nDeviceIndex));

                if (m_bSocketConnected)
                    ElanTouchSwitch.Disconnect(m_bSocketConnectType);

                OutputMessage("-Start Socket Event CallBack");

                // 建立 Socket 連線
                int nResultFlag = EstablishSocketConnection(0, nPort);

                if (!ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType))
                {
                    m_bTPConnected = false;
                    m_bSocketConnected = false;
                    m_sErrorMessage = "Start Socket Server Failed";
                    return false;
                }

                m_bSocketConnected = true;
                Thread.Sleep(100);

                // 等待 SSH Remote Socket 連線
                if (WaitForSSHConnection())
                {
                    bConnectFailFlag = false;
                    break;
                }
            }

            if (bConnectFailFlag)
            {
                m_sErrorMessage = "Connect to TP by SSH Remote Socket Fail";
                return false;
            }

            Thread.Sleep(2000);
            return true;
        }

        /// <summary>
        /// 重新連接SSH Socket,包含重試機制、斷開現有連線、Socket重新連接、SSH連線建立及等待連線完成的完整流程
        /// </summary>
        /// <returns>SSH Socket重新連接成功並完成回調回傳true;連接失敗、使用者中斷執行或重試次數用盡則回傳false並設定錯誤訊息</returns>
        private bool ReconnectSSHSocket()
        {
            bool bConnectFailFlag = true;

            for (int nRetryIndex = 0; nRetryIndex <= 3; nRetryIndex++)
            {
                DisconnectElanSSHClient();
                ElanTouchSwitch.SetGetParameterTimeout();
                m_nDeviceIndex = 0;
                m_bSocketWaitCallback = false;

                int nAddress = 0;
                int nPort = ParamFingerAutoTuning.m_nPort == 0 ?
                    ElanDefine.DEFAULT_SOCKET_PORT :
                    ParamFingerAutoTuning.m_nPort;

                OutputMessage(string.Format("-Address={0}, Port={1}, DeviceIndex={2}",
                    nAddress, nPort, m_nDeviceIndex));

                // 斷開現有連線
                if (m_bSocketConnected)
                    ElanTouchSwitch.Disconnect(m_bSocketConnectType);

                OutputMessage("-Start Socket Event CallBack");

                // 建立 Socket 連線
                int nResultFlag = EstablishSocketConnection(nAddress, nPort);

                if (!ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType))
                {
                    m_bTPConnected = false;
                    m_bSocketConnected = false;
                    m_sErrorMessage = "Start Socket Server Failed";
                    return false;
                }

                m_bSocketConnected = true;
                Thread.Sleep(100);

                // 等待 SSH Remote Socket 連線
                int nLoopRetryCount = 0;

                while (!m_bSocketWaitCallback)
                {
                    if (!m_cfrmParent.m_bExecute)
                    {
                        m_bTPConnected = false;
                        m_bSocketConnected = false;
                        return false;
                    }

                    RunSSHRemoteSocket();
                    m_bTPConnected = true;
                    nLoopRetryCount++;
                    Thread.Sleep(100);

                    if (nLoopRetryCount > 10) break;
                }

                if (m_bSocketWaitCallback)
                {
                    bConnectFailFlag = false;
                    break;
                }
            }

            if (bConnectFailFlag)
            {
                m_sErrorMessage = "Connect to TP by SSH Remote Socket Fail";
                return false;
            }

            Thread.Sleep(2000);
            return true;
        }

        /// <summary>
        /// 等待SSH連線完成,執行SSH Remote Socket並等待回調完成,包含重試機制及執行中斷處理
        /// </summary>
        /// <returns>回調完成回傳true;使用者中斷執行或重試次數用盡則回傳false並設定連接狀態為false</returns>
        private bool WaitForSSHConnection()
        {
            int nLoopRetryCount = 0;

            while (!m_bSocketWaitCallback)
            {
                if (!m_cfrmParent.m_bExecute)
                {
                    m_bTPConnected = false;
                    m_bSocketConnected = false;
                    return false;
                }

                RunSSHRemoteSocket();
                m_bTPConnected = true;
                nLoopRetryCount++;
                Thread.Sleep(100);

                if (nLoopRetryCount > 10) break;
            }

            return m_bSocketWaitCallback;
        }

        /// <summary>
        /// 執行SSH Remote Socket,根據Socket類型取得SSH參數(IP、使用者名稱、密碼),建立SSH連線並在遠端主機上執行Client AP連接至本地Host
        /// </summary>
        private void RunSSHRemoteSocket()
        {
            string sRemoteClientIPAddress, sSSHUserName, sSSHPassword;
            bool bDebugLogFlag;

            // 根據 Socket 類型取得參數
            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
            {
                sRemoteClientIPAddress = ParamFingerAutoTuning.m_sChromeSSHSocketServerRemoteClientIPAddress;
                sSSHUserName = ParamFingerAutoTuning.m_sChromeSSHSocketServerUserName;
                sSSHPassword = ParamFingerAutoTuning.m_sChromeSSHSocketServerPassword;
                bDebugLogFlag = ParamFingerAutoTuning.m_nChromeSSHSocketServerDebugMode == 1;
            }
            else
            {
                sRemoteClientIPAddress = ParamFingerAutoTuning.m_sSSHSocketServerRemoteClientIPAddress;
                sSSHUserName = ParamFingerAutoTuning.m_sSSHSocketServerUserName;
                sSSHPassword = ParamFingerAutoTuning.m_sSSHSocketServerPassword;
                bDebugLogFlag = ParamFingerAutoTuning.m_nSSHSocketServerDebugMode == 1;
            }

            // 取得本地 IP
            string sHostIPAddress = m_bGetLocalIPAddressFlag ?
                m_sLocalIPAddress :
                (m_sLocalIPAddress = GetLocalIPAddress(sRemoteClientIPAddress));
            m_bGetLocalIPAddressFlag = true;

            OutputMessage(string.Format("-Try to Do SSH Logon, IP Address = {0}", sRemoteClientIPAddress));

            // 建立 SSH 連線
            m_cElanSSHClient = new ElanSSHClient(sRemoteClientIPAddress, sSSHUserName, sSSHPassword);

            if (m_cElanSSHClient.Connect(bDebugLogFlag))
            {
                OutputMessage("-SSH Logon Success");
                OutputMessage(string.Format("-Try to Run Client AP to Connect to Host. Host IP Address = {0}", sHostIPAddress));

                if (!m_cElanSSHClient.EstablishRemoteConnect(sHostIPAddress, ElanDefine.DEFAULT_SOCKET_PORT.ToString(), ParamFingerAutoTuning.m_nInterfaceType))
                {
                    OutputMessage(string.Format("-Run Client AP Failed. Fail Reason = {0}", m_cElanSSHClient.FailReason));
                    return;
                }

                OutputMessage("-Run Client AP Succeed.");
            }
            else
            {
                OutputMessage("-Run SSH Logon Failed.");
            }
        }

        /// <summary>
        /// 斷開SSH客戶端連線,若SSH客戶端存在則執行斷線、釋放物件並重置TP連接狀態,等待1秒完成清理
        /// </summary>
        public void DisconnectElanSSHClient()
        {
            if (m_cElanSSHClient != null)
            {
                OutputMessage("[State]Disconnect Elan SSH Client");
                m_cElanSSHClient.Disconnect();
                m_cElanSSHClient = null;
                m_bTPConnected = false;
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 取得本地IP位址,收集所有IPv4位址,若有多個則與客戶端IP位址比對前三碼選擇同網段的IP
        /// </summary>
        /// <param name="sClientIPAddress">客戶端IP位址,用於網段比對</param>
        /// <returns>回傳本地IP位址字串,無IP位址時回傳空字串</returns>
        public string GetLocalIPAddress(string sClientIPAddress)
        {
            var vHost = Dns.GetHostEntry(Dns.GetHostName());
            List<string> sHostIP_List = new List<string>();

            // 收集所有 IPv4 位址
            foreach (var vIP in vHost.AddressList)
            {
                if (vIP.AddressFamily == AddressFamily.InterNetwork)
                {
                    sHostIP_List.Add(vIP.ToString());
                }
            }

            // 無 IP 位址
            if (sHostIP_List.Count <= 0)
                return "";

            // 只有一個 IP 位址
            if (sHostIP_List.Count == 1)
                return sHostIP_List[0];

            // 多個 IP 位址：比對前三碼
            string[] sToken_Array = sClientIPAddress.Split('.');

            if (sToken_Array.Length < 4)
                return sHostIP_List[0];

            foreach (string sCurrentIPAddress in sHostIP_List)
            {
                string[] sCurrentToken_Array = sCurrentIPAddress.Split('.');

                bool bMatchedFlag = true;

                for (int index = 0; index < 3; index++)
                {
                    if (sCurrentToken_Array[index].Equals(sToken_Array[index]) == false)
                    {
                        bMatchedFlag = false;
                        break;
                    }
                }

                if (bMatchedFlag)
                    return sCurrentIPAddress;
            }

            return sHostIP_List[0];
        }

        #endregion
    }
}
