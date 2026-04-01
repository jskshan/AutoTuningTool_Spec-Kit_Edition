using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        #region Android 裝置連線

        /// <summary>
        /// 連接Android裝置,清除舊的Debug Log檔案,根據Socket類型執行Socket安裝或直接連接,若啟用則執行Finger Report測試
        /// </summary>
        /// <returns>連接成功且所有流程完成回傳true;Socket安裝失敗、連接失敗或使用者中斷執行則回傳false並設定TP連接狀態為false</returns>
        private bool ConnectAndroidDevice()
        {
            string sDebugLogFilePath = string.Format(@"{0}\{1}\DebugLog\DebugLog{2}.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName, ElanBatchProcess.m_sFILETYPE_MAINPROC); ;

            // 清除舊的 Debug Log
            while (File.Exists(sDebugLogFilePath))
            {
                File.Delete(sDebugLogFilePath);
                Thread.Sleep(10);
            }

            // 判斷是否需要執行 Socket 安裝
            if (Directory.Exists(string.Format(@"{0}\RemoteClient", Application.StartupPath)) &&
                ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL)
            {
                OutputMessage("-Connect To Android Device...");

                if (!RunSocketInstall())
                {
                    m_bTPConnected = false;
                    return false;
                }
            }
            else
            {
                if (!ConnectSocket())
                {
                    m_bTPConnected = false;
                    return false;
                }

                OutputMessage("-Connect Socket Finish");
            }

            // 處理 Finger Report 測試
            if (ParamFingerAutoTuning.m_nFingerReportTest == 1)
            {
                SaveFingerReport();
                if (!m_cfrmParent.m_bExecute) return false;
            }

            return true;
        }

        /// <summary>
        /// 重新連接Android裝置,若已連接則等待後關閉伺服器,再執行Socket連接
        /// </summary>
        /// <returns>重新連接成功回傳true;連接失敗則回傳false並設定TP連接狀態為false</returns>
        private bool ReconnectAndroidDevice()
        {
            if (m_bSocketConnected)
                Thread.Sleep(100);

            CloseAndroidRemoteServer();

            if (!ConnectSocket())
            {
                m_bTPConnected = false;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 關閉Android遠端伺服器處理程序,終止伺服器管理器、清除事件訂閱並重置連線狀態
        /// </summary>
        public void CloseAndroidRemoteServer()
        {
            if (m_cRunServerMgr != null)
            {
                m_cRunServerMgr.Terminate();
                m_cRunServerMgr.m_AndroidRemoteServerEvent = null;
                m_cRunServerMgr.m_OutputTextEvent = null;
                m_cRunServerMgr = null;
                m_bSocketConnected = false;
            }
        }

        /// <summary>
        /// 執行Socket安裝,根據介面類型判斷是SPI介面則執行Native Socket伺服器,否則執行Python Socket伺服器
        /// </summary>
        /// <returns>Socket伺服器啟動成功回傳true;啟動失敗則回傳false</returns>
        private bool RunSocketInstall()
        {
            bool bIsSPI = ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_RISING_HALF ||
                          ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_RISING ||
                          ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING_HALF ||
                          ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING;

            return bIsSPI ? RunNativeSocketServer() : RunPythonSocketServer();
        }

        /// <summary>
        /// 執行Python Socket伺服器,先終止現有的伺服器管理器,根據介面類型(I2C或HIDOverI2C)建立對應的Android遠端伺服器實例並執行
        /// </summary>
        /// <returns>遠端Socket伺服器執行成功回傳true;執行失敗則回傳false</returns>
        private bool RunPythonSocketServer()
        {
            TerminateRunServerManager();

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                if (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_I2C)
                    m_cRunServerMgr = new ExecAndroidRemoteServer(3, false, false);
                else if (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_HIDOVERI2C)
                    m_cRunServerMgr = new ExecAndroidRemoteServer(1, false, false);
            });

            return ExecuteRemoteServer("Remote Socket");
        }

        /// <summary>
        /// 執行Native Socket伺服器,先終止現有的伺服器管理器,建立Android Native伺服器實例並執行SPI Socket
        /// </summary>
        /// <returns>遠端SPI Socket伺服器執行成功回傳true;執行失敗則回傳false</returns>
        private bool RunNativeSocketServer()
        {
            TerminateRunServerManager();

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                m_cRunServerMgr = new ExecAndroidNativeServer(false);
            });

            return ExecuteRemoteServer("Remote SPI Socket");
        }

        /// <summary>
        /// 執行遠端伺服器的共用邏輯,設定事件處理、更新SH檔案(非SPI類型)、啟動伺服器並連接Socket
        /// </summary>
        /// <param name="sServerType">伺服器類型字串,用於訊息顯示及錯誤處理</param>
        /// <returns>伺服器啟動成功且Socket連接成功回傳true;啟動失敗或連接失敗則回傳false並設定錯誤訊息</returns>
        private bool ExecuteRemoteServer(string sServerType)
        {
            m_cRunServerMgr.m_AndroidRemoteServerEvent = OnAndroidRemoteServerEvent;
            m_cRunServerMgr.m_OutputTextEvent = OnAndroidRemoteServerOutputText;

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                m_cfrmParent.SetButton(frmMain.FlowState.StopDisable);
            });

            if (sServerType.Contains("SPI"))
            {
                // Native Server 不需要更新 SH 檔案
            }
            else
            {
                m_cRunServerMgr.UpdateSHFile();
                Thread.Sleep(ParamFingerAutoTuning.m_nAfUpdateSHFileDelayTime);
                OutputMessage("-Update SH File Success");
            }

            if (!m_cRunServerMgr.Start())
            {
                MessageBox.Show(m_cRunServerMgr.ErrorText, string.Format("Run {0} Error", sServerType), MessageBoxButtons.OK, MessageBoxIcon.Error);

                m_cfrmParent.Invoke((MethodInvoker)delegate
                {
                    m_cfrmParent.SetButton(frmMain.FlowState.StopEnable);
                });

                m_sErrorMessage = string.Format("Run {0} Error({1})", sServerType, m_cRunServerMgr.ErrorText);
                return false;
            }

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                m_cfrmParent.SetButton(frmMain.FlowState.StopEnable);
            });

            OutputMessage(string.Format("-Execute {0} Server Success", sServerType));

            return ConnectSocket();
        }

        /// <summary>
        /// 終止伺服器管理器,終止執行中的伺服器、清除事件訂閱並釋放管理器物件
        /// </summary>
        private void TerminateRunServerManager()
        {
            if (m_cRunServerMgr != null)
            {
                m_cRunServerMgr.Terminate();
                m_cRunServerMgr.m_AndroidRemoteServerEvent = null;
                m_cRunServerMgr.m_OutputTextEvent = null;
                m_cRunServerMgr = null;
            }
        }

        /// <summary>
        /// 建立Socket連接,設定Port及位址,註冊Callback事件,根據Android Socket類型(Server/Client)執行對應的連接邏輯,若啟用則啟動接收Finger Report執行緒
        /// </summary>
        /// <returns>Socket連接成功回傳true並設定TP及Socket連接狀態為true;連接失敗則回傳false並設定錯誤訊息</returns>
        private bool ConnectSocket()
        {
            int nPort = ParamFingerAutoTuning.m_nPort == 0 ? 9344 : ParamFingerAutoTuning.m_nPort;
            int nAddress = 0;

            if (m_bSocketConnected)
                ElanTouchSwitch.Disconnect(m_bSocketConnectType);

            // 設定 Callback
            if (ParamFingerAutoTuning.m_sAndroidSocketType == MainConstantParameter.m_sANDROIDSOCKET_SERVER)
            {
                if (m_bSocketConnectType == false)
                    ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(AndroidSocketEvent);
                else
                    ElanTouch_Socket.m_SocketEventCallBack = new ElanTouch_Socket.PFUNC_SOCKET_EVENT_CALLBACK(AndroidSocketEvent);
            }
            else
            {
                nAddress = 0x7F000001; // 127.0.0.1
            }

            // 建立連線
            int nResultFlag = m_bSocketConnectType == false ?
                ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack) :
                ElanTouch_Socket.ConnectSocket(nAddress, nPort, ElanTouch_Socket.m_SocketEventCallBack);

            if (!ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType))
            {
                string sErrorMessage = ParamFingerAutoTuning.m_sAndroidSocketType == MainConstantParameter.m_sANDROIDSOCKET_SERVER ? "Start Socket Server Failed" : "Connect Server Failed";
                OutputMessage(string.Format("-{0}", sErrorMessage));
                m_sErrorMessage = sErrorMessage;
                return false;
            }

            m_bTPConnected = true;
            m_bSocketConnected = true;

            // 更新狀態訊息
            if (ParamFingerAutoTuning.m_sAndroidSocketType == MainConstantParameter.m_sANDROIDSOCKET_SERVER)
            {
                OutputMessage("-Socket Server has been started. Waiting Remote Device Connect...");
            }
            else
            {
                OutputMessage("-The Connection of Remoted Device has Established");

                if (ParamFingerAutoTuning.m_nAndroidDisableGetReport == 0)
                {
                    m_bRecvSocketFingerReport = true;
                    ThreadPool.QueueUserWorkItem(OnInputReportReceive);
                }
            }

            return true;
        }

        #endregion

        #region 事件處理

        /// <summary>
        /// 處理Android遠端伺服器事件,當伺服器終止時執行終止處理,其他情況輸出事件文字至控制台
        /// </summary>
        /// <param name="ErrorCode">Android USB錯誤代碼列舉</param>
        /// <param name="sText">事件文字訊息</param>
        private void OnAndroidRemoteServerEvent(ExecAndroidRemoteServer.AndroidUsbErrorCode ErrorCode, string sText)
        {
            if (ErrorCode == ExecAndroidRemoteServer.AndroidUsbErrorCode.Terminated)
                OnAndroidRemoteServerTerminated();
            else
                Console.WriteLine(sText);

            //m_OutputQueue.Enqueue(new OutputItem(sText));
        }

        /// <summary>
        /// 處理Android遠端伺服器輸出文字事件,將文字訊息輸出至控制台
        /// </summary>
        /// <param name="sText">輸出文字訊息</param>
        private void OnAndroidRemoteServerOutputText(string sText)
        {
            Console.WriteLine(sText);
            //m_OutputQueue.Enqueue(new OutputItem(sText));
        }

        /// <summary>
        /// 處理Android Socket事件,EventID為1時表示遠端裝置已連接並啟動Finger Report接收執行緒,其他情況則停止接收並等待裝置連接
        /// </summary>
        /// <param name="nEventID">事件識別碼,1表示裝置已連接,其他值表示等待連接</param>
        private void AndroidSocketEvent(int nEventID)
        {
            switch (nEventID)
            {
                case 1:
                    m_cfrmParent.Invoke((MethodInvoker)delegate
                    {
                        //btnStartTest.Enabled = true;
                        //lbStatus.Text = "Remoted Device connected";
                        OutputMessage("-Remoted Device Connected");

                        //Socket 狀況下需啟動Thread讀取Finger Report
                        m_bRecvSocketFingerReport = true;
                        //Start a thread to receive the input report.(from socket)
                        ThreadPool.QueueUserWorkItem(OnInputReportReceive);
                    });
                    break;
                default:
                    m_bRecvSocketFingerReport = false;
                    Thread.Sleep(33);

                    m_cfrmParent.Invoke((MethodInvoker)delegate
                    {
                        //btnStartTest.Enabled = false;
                        //lbStatus.Text = "Socket Server has been started. Waiting remote device connect...";
                        OutputMessage("-Socket Server has been Started. Waiting Remote Device Connect...");
                    });
                    break;
            }
        }

        /// <summary>
        /// 處理Android遠端伺服器終止時的清理工作,根據連接類型取消註冊HID回調函式、清除回調事件並輸出無裝置連接訊息
        /// </summary>
        public void OnAndroidRemoteServerTerminated()
        {
            /*
            m_bStartDoOutput = false;
            m_bDevConnected = false;
            m_bContacted = false;
            */

            //Unregister the callback function to receive the report
            if (m_bSocketConnectType == false)
            {
                ElanTouch.InputRawUnRegHIDCallback();
                Thread.Sleep(100);
                ElanTouch.m_InHIDReceiveCallBack = null;
                ElanTouch.m_OutReportCallBack = null;
                ElanTouch.m_InReportCallBack = null;

                ElanTouch.OutReportUnRegCallback();
                ElanTouch.InReportUnRegCallback();
            }
            else
            {
                ElanTouch_Socket.InputRawUnRegHIDCallback();
                Thread.Sleep(100);
                ElanTouch_Socket.m_InHIDReceiveCallBack = null;
                ElanTouch_Socket.m_OutReportCallBack = null;
                ElanTouch_Socket.m_InReportCallBack = null;

                ElanTouch_Socket.OutReportUnRegCallback();
                ElanTouch_Socket.InReportUnRegCallback();
            }

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                //btnStartSocket.Enabled = false;
                //lbStatus.Text = "No Device has been connected";
                OutputMessage("-No Device has been Connected");
            });
        }

        #endregion

#if _USE_9F07_SOCKET
        #region Gen9 (9F07) 連線

        /// <summary>
        /// 連接Gen9裝置,根據Socket類型判斷是否支援,僅支援Android Remote Client及ARM Tool類型,其他類型回傳錯誤
        /// </summary>
        /// <returns>Socket類型為Android且連接成功回傳true;不支援的Socket類型或連接失敗則回傳false並設定錯誤訊息</returns>
        private bool ConnectGen9Device()
        {
            switch (ParamFingerAutoTuning.m_sSocketType)
            {
                case MainConstantParameter.m_sSOCKET_WINDOWS:
                    OutputMessage("-No Support 9F07 by Windows OS Type");
                    m_sErrorMessage = "No Support 9F07 by Windows OS Type";
                    return false;

                case MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT:
                    OutputMessage("-No Support 9F07 by Chrome(Previously) OS Type");
                    m_sErrorMessage = "No Support 9F07 by Chrome(Previously) OS Type";
                    return false;

                case MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER:
                    OutputMessage("-No Support 9F07 by Chrome OS Type / SSH Socket Server");
                    m_sErrorMessage = "No Support 9F07 by Chrome OS Type / SSH Socket Server";
                    return false;

                case MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT:
                case MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL:
                    return ConnectGen9Android();

                default:
                    return false;
            }
        }

        /// <summary>
        /// 連接Gen9 Android裝置,根據是否有RemoteClient目錄決定連接方式,建立Socket連接後從手機裝置讀取para.json檔案,包含重試機制
        /// </summary>
        /// <returns>Socket連接成功且para.json檔案讀取成功回傳true;連接失敗、使用者中斷執行或重試後仍無法讀取檔案則回傳false並設定錯誤訊息</returns>
        private bool ConnectGen9Android()
        {
            // Reset Flow
            bool bResultFlag = false;

            if (Directory.Exists(string.Format(@"{0}\RemoteClient", Application.StartupPath)) &&
                ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL)
            {
                m_cfrmParent.Invoke((MethodInvoker)delegate
                {
                    bResultFlag = ConnectSocket_9F07();
                });
            }
            else
            {
                bResultFlag = ConnectSocket_9F07();
            }

            if (!bResultFlag) return false;

            Thread.Sleep(1000);

            // Connect Flow - 取得 para.json
            ADBFileIO adbFileIO = new ADBFileIO(@"/data/local/tmp/thp/para.json", string.Format(@"{0}\para.json", Application.StartupPath));

            int nRetryCount = 10;
            for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
            {
                if (!m_cfrmParent.m_bExecute) return false;

                if (!adbFileIO.Pull())
                {
                    OutputMessage(string.Format("-No para.json file in phone device(RetryCount={0})", nRetryIndex));

                    if (nRetryIndex == nRetryCount - 1)
                    {
                        m_sErrorMessage = "No para.json file in phone device";
                        return false;
                    }

                    Thread.Sleep(500);
                    continue;
                }

                OutputMessage("-Get para.json file from phone device");
                break;
            }

            return true;
        }

        /// <summary>
        /// 建立9F07的Socket連接,設定預設Port及位址,若已連接則先斷線,註冊In/Out Report回調函式,建立連接後若為SPI介面則設定命令長度
        /// </summary>
        /// <returns>Socket連接成功回傳true並設定Socket連接狀態;連接失敗則回傳false並設定錯誤訊息</returns>
        private bool ConnectSocket_9F07()
        {
            int nPort = ElanDefine.DEFAULT_SOCKET_PORT_9F07;
            int nAddress = 0x7F000001; // 127.0.0.1

            if (m_bSocketConnected)
            {
                ElanTouchSwitch.Disconnect(m_bSocketConnectType);
                Thread.Sleep(ElanDefine.TIME_100MS);
            }

            // 設定 Callback
            if (ElanTouchSwitch.CheckOutReportCallBackIsNull(m_bSocketConnectType))
                ElanTouchSwitch.SetOutReportRegCallback(this, m_bSocketConnectType);

            if (ElanTouchSwitch.CheckInReportCallBackIsNull(m_bSocketConnectType))
                ElanTouchSwitch.SetInReportRegCallback(this, m_bSocketConnectType);

            // 建立連線
            int nResultFlag = ElanTouchSwitch.ConnectSocket(nAddress, nPort, m_bSocketConnectType);

            if (!ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType))
            {
                m_sErrorMessage = "Connect Server Failed";
                OutputMessage("-Connect Server Failed");
                return false;
            }

            // 設定 SPI Command Length
            if (IsSPIInterface(ParamFingerAutoTuning.m_nInterfaceType))
                ElanTouchSwitch.SetSPICmdLength(m_bSocketConnectType, ParamFingerAutoTuning.m_nSPICommandLength);

            m_bSocketConnected = true;
            OutputMessage("-Remote Device Connected");

            return true;
        }

        /// <summary>
        /// Out Report回調函式,接收並處理輸出報告資料,將IntPtr緩衝區複製至位元組陣列並轉換為十六進位字串格式(目前已註解輸出至佇列的功能)
        /// </summary>
        /// <param name="pReportBuffer">指向報告緩衝區的指標</param>
        /// <param name="nReportLen">報告資料長度</param>
        public void OutReportCallbackFunc(IntPtr pReportBuffer, int nReportLen)
        {
            /*
            if (m_bPlaying == true)
                return;
            */

            byte[] OutReportBuffer = new byte[nReportLen];
            Array.Clear(OutReportBuffer, 0, OutReportBuffer.Length);
            Marshal.Copy(pReportBuffer, OutReportBuffer, 0, nReportLen);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nReportLen; i++)
                sb.Append(string.Format(" {0}", OutReportBuffer[i].ToString("X2")));

            /*
            Invoke((MethodInvoker)delegate
            {
                m_OutputQueue.Enqueue(new OutputItem(sb.ToString(), OutputItem.OutputType.OUT));
            });
            */
        }

        /// <summary>
        /// In Report回調函式,接收並處理輸入報告資料,將IntPtr緩衝區複製至位元組陣列並轉換為十六進位字串格式(目前已註解輸出至佇列及回應佇列的功能)
        /// </summary>
        /// <param name="pReportBuffer">指向報告緩衝區的指標</param>
        /// <param name="nReportLen">報告資料長度,若小於等於0則直接返回</param>
        public void InReportCallbackFunc(IntPtr pReportBuffer, int nReportLen)
        {
            if (nReportLen <= 0)
                return;

            byte[] OutReportBuffer = new byte[nReportLen];
            Array.Clear(OutReportBuffer, 0, OutReportBuffer.Length);
            Marshal.Copy(pReportBuffer, OutReportBuffer, 0, nReportLen);
            byte nCategory = OutReportBuffer[0];

            /*
            #region keep the response from server.(Not iclude the report and raw data)
            if (nCategory != 0xFC && nCategory != 0x01)
                m_ResponseQueue.Enqueue(OutReportBuffer);
            #endregion
            */

            /*
            //J2 Modified. Don't show the report data on output when user starts a report recording action.
            if (m_bPlaying == true || m_bStartLinearity == true || m_bStartReportRecorder == true)
                return;
            */

            //J2++
            /*
            APSetting Param = APSetting.GetInstance();
            if (Param.m_TraceTestParam.EnableShowRawOnOutput == false && m_frmTraceTest != null)
                return;
            */
            //J2--

            try
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < nReportLen; i++)
                    sb.Append(string.Format(" {0}", OutReportBuffer[i].ToString("X2")));

                /*
                Invoke((MethodInvoker)delegate
                {
                    m_OutputQueue.Enqueue(new OutputItem(sb.ToString(), OutputItem.OutputType.IN));
                });
                */
            }
            catch
            {
                Console.WriteLine("Receive Report fail");
            }
        }

        /// <summary>
        /// 取得追蹤線資訊,包含RX及TX追蹤線數量,根據Socket連接類型初始化對應的TraceInfo結構並設定全域追蹤線數量變數
        /// </summary>
        /// <returns>成功取得追蹤線資訊回傳true;取得失敗則回傳false</returns>
        private bool GetTraceInfo()
        {
            short nTXTrace = -1;
            short nRXTrace = -1;

            if (!ElanDirectTochMainFlow.GetTraceInfo(ref nRXTrace, ref nTXTrace))
                return false;

            if (m_bSocketConnectType == true)
            {
                m_structTraceInfo_Socket = new ElanTouch_Socket.TraceInfo(1)
                {
                    nXTotal = nRXTrace,
                    nYTotal = nTXTrace,
                    nPartialNum = 0,
                    XAxis = new int[] { nRXTrace, 0, 0, 0 }
                };
            }
            else
            {
                m_structTraceInfo = new ElanTouch.TraceInfo(1)
                {
                    nXTotal = nRXTrace,
                    nYTotal = nTXTrace,
                    nPartialNum = 0,
                    XAxis = new int[] { nRXTrace, 0, 0, 0 }
                };
            }

            m_nRXTraceNumber = nRXTrace;
            m_nTXTraceNumber = nTXTrace;

            return true;
        }

        /// <summary>
        /// Socket連接執行緒函式(9F07版本),在UI執行緒中執行Socket連接(目前已註解自動啟動遠端伺服器的相關程式碼)
        /// </summary>
        /// <param name="objParam">執行緒參數物件(目前未使用)</param>
        private void SocketConnectThreadFunc(object objParam)
        {
            #region Run the remote server
            //When the socket type is android-usb, need to run the remote server on android device.
            //int nSocketType = 0;
            //int nInterface = 0;
            //Invoke((MethodInvoker)delegate
            //{
            //    nSocketType = cboSocketType.SelectedIndex;
            //    nInterface = cboInterface.SelectedIndex;
            //});

            //if (nSocketType == 1)
            //{
            //    APSetting apSet = APSetting.GetInstance();

            //    if (true == apSet.m_ConnectParam.EnableAutoRunSocketServer)
            //    {
            //        if (IsSPIInterface(nInterface))
            //        {
            //            if (xRunNativeSocketServer(apSet) == false)
            //                return;
            //        }
            //        else
            //        {
            //            if (xRunPythonSocketServer(apSet, nInterface) == false)
            //                return;
            //        }
            //    }
            //}
            #endregion

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                ConnectSocket_9F07();
                //btnConnectReset.Enabled = true;
            });
        }

        #endregion
#endif
    }
}
