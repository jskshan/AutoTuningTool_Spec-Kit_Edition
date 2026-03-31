using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
#if DISABLE_V1_0_2_6
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ConnectToTP()
        {
            if (m_bTPConnected == true)
            {
                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                        DisconnectElanSSHClient();
                }
            }

            #region Connect to TP
            if (m_bTPConnected == false)
            {
                OutputMessage("[State]Connect to TP");

                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    {
                        bool bConnectFailFlag = true;

                        for (int nRetryIndex = 0; nRetryIndex <= 3; nRetryIndex++)
                        {
                            DisconnectElanSSHClient();

                            ElanTouchSwitch.SetGetParameterTimeout();

                            m_nDeviceIndex = 0;

                            int nPort = 0;
                            int nAddress = 0;

                            m_bSocketWaitCallback = false;

                            //Int32.TryParse(tbPID.Text, out nPort);
                            nPort = ParamFingerAutoTuning.m_nPort;

                            if (nPort == 0)
                                nPort = ElanDefine.DEFAULT_SOCKET_PORT;

                            OutputMessage(string.Format("-Address={0}, Port={1}, DeviceIndex={2}", nAddress, nPort, m_nDeviceIndex));

                            //Do disconnect...
                            if (m_bSocketConnected == true)
                                //ElanTouch.Disconnect();
                                ElanTouchSwitch.Disconnect(m_bSocketConnectType);

                            OutputMessage("-Start Socket Event CallBack");
                            //Set the callback function for socket server.
                            //ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                            //int nRet = ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);

                            int nResultFlag;

                            if (m_bSocketConnectType == false)
                            {
                                ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                                nResultFlag = ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);
                            }
                            else
                            {
                                ElanTouch_Socket.m_SocketEventCallBack = new ElanTouch_Socket.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                                nResultFlag = ElanTouch_Socket.ConnectSocket(nAddress, nPort, ElanTouch_Socket.m_SocketEventCallBack);
                            }

                            //if (nRet != ElanTouch_Socket.TP_SUCCESS)
                            if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                            {
                                m_bTPConnected = false;
                                m_bSocketConnected = false;
                                m_sErrorMessage = "Start Socket Server Failed";
                                return false;
                            }
                            else
                            {
                                m_bSocketConnected = true;
                            }

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

                            Thread.Sleep(100);

                            int nLoopRetryCount = 0;

                            while (m_bSocketWaitCallback == false)
                            {
                                if (m_cfrmParent.m_bExecute == false)
                                {
                                    m_bTPConnected = false;
                                    m_bSocketConnected = false;
                                    return false;
                                }
                                else
                                {
                                    RunSSHRemoteSocket();
                                    m_bTPConnected = true;
                                }

                                nLoopRetryCount++;

                                Thread.Sleep(100);

                                if (nLoopRetryCount > 10)
                                {
                                    break;
                                }
                            }

                            if (m_bSocketWaitCallback == true)
                            {
                                bConnectFailFlag = false;
                                break;
                            }
                        }

                        if (bConnectFailFlag == true)
                        {
                            m_sErrorMessage = "Connect to TP by SSH Remote Socket Fail";
                            return false;
                        }

                        Thread.Sleep(2000);
                    }
                    else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_WINDOWS)
                    {
                        //MessageOutput("Connect to TP Device");

                        int nVID = ParamFingerAutoTuning.m_nUSBVID;
                        int nPID = ParamFingerAutoTuning.m_nUSBPID;
                        int nDVDD = ParamFingerAutoTuning.m_nDVDD;
                        int nVIO = ParamFingerAutoTuning.m_nVIO;
                        int nI2CAddress = ParamFingerAutoTuning.m_nI2CAddress;
                        int nNormalLength = ParamFingerAutoTuning.m_nNormalLength;

                        if (ConnectTP(nVID, nPID, nDVDD, nVIO, nI2CAddress, nNormalLength) == false)
                        {
                            //MessageBox.Show("No Valid Device Found. Please Press Ok to Exit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //m_bCollectFlowError = true;
                            //m_sCollectFlowErrorMsg = "No Valid TP Device Found";
                            //MessageOutput("No Valid TP Device Found");

                            m_bTPConnected = false;
                            m_sErrorMessage = "Disconnect to TP Device";

                            return false;
                        }

                        if (ParamFingerAutoTuning.m_nFingerReportTest == 1)
                        {
                            if (m_cInputDevice.RegisterHIDDevice(ParamFingerAutoTuning.m_nUSBVID, ParamFingerAutoTuning.m_nUSBPID) == false)
                            {
                                m_sErrorMessage = "Register TP Device Error";
                                return false;
                            }

                            m_bRegistHIDDevice = true;

                            m_cInputDevice.HIDHandler -= HIDRawInputHandler;
                            m_cInputDevice.HIDHandler += HIDRawInputHandler;

                            SaveFingerReport();

                            #region Mark It
                            /*
                            List<List<byte>> DataArray = new List<List<byte>>();

                            m_bStartRecord = true;
                            while (m_frmParent.m_bExecute == true)
                            {
                                byte[] m_pBuf = new byte[116];
                                if (m_qFIFO.DequeueAll(5000, m_pBuf, 116) == false)
                                    continue;

                                List<byte> ReportArray = new List<byte>();

                                ReportArray.Clear();

                                foreach (byte Raw in m_pBuf)
                                    ReportArray.Add(Raw);

                                DataArray.Add(new List<byte>(ReportArray));
                                continue;
                            }
                            m_bStartRecord = false;

                            FileStream fs = new FileStream(@"FingerReport.txt", FileMode.Create);
                            m_swReportLog = new StreamWriter(fs);

                            int DataLength = DataArray.Count;

                            try
                            {
                                for (int nIndex = 0; nIndex < DataLength; nIndex++)
                                {
                                    string saveString = "";

                                    int ReportLength = DataArray[nIndex].Count;

                                    for (int mIndex = 0; mIndex < ReportLength; mIndex++)
                                        saveString += DataArray[nIndex][mIndex].ToString("X2") + " ";

                                    m_swReportLog.WriteLine(saveString);
                                }
                            }
                            catch (IOException ex)
                            {
                                m_sErrMsg = "Save Record Data Error";
                                return false;
                            }
                            finally
                            {
                                m_swReportLog.Flush();
                                m_swReportLog.Close();
                                fs.Close();
                            }
                            */
                            #endregion

                            if (m_cfrmParent.m_bExecute == false)
                                return false;
                        }
                    }
                    else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL)
                    {
                        string sDebugLogFilePath = string.Format(@"{0}\{1}\DebugLog\DebugLog{2}.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName, ElanBatchProcess.m_sFILETYPE_MAINPROC);

                        while (File.Exists(sDebugLogFilePath) == true)
                        {
                            File.Delete(sDebugLogFilePath);
                            Thread.Sleep(10);
                        }

                        //Check the folder exist or not.
                        //btnStartSocket.Enabled = false;
                        if (Directory.Exists(string.Format(@"{0}\RemoteClient", Application.StartupPath)) == true && ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL)
                        {
                            //lbStatus.Text = "Connect to android device...";
                            OutputMessage("-Connect To Android Device...");
                            //ThreadPool.QueueUserWorkItem(SocketConnectThreadFunc);
                            if (RunSocketInstall() == false)
                            {
                                m_bTPConnected = false;
                                return false;
                            }
                        }
                        else
                        {
                            if (ConnectSocket() == false)
                            {
                                m_bTPConnected = false;
                                return false;
                            }

                            OutputMessage("-Connect Socket Finish");
                        }

                        if (ParamFingerAutoTuning.m_nFingerReportTest == 1)
                        {
                            SaveFingerReport();

                            if (m_cfrmParent.m_bExecute == false)
                                return false;
                        }
                    }
                    else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT)
                    {
                        m_nDeviceIndex = 0;

                        int nPort = 0;
                        int nAddress = 0;

                        m_bSocketWaitCallback = false;

                        //Int32.TryParse(tbPID.Text, out nPort);
                        nPort = ParamFingerAutoTuning.m_nPort;

                        if (nPort == 0)
                            nPort = ElanDefine.DEFAULT_SOCKET_PORT;

                        OutputMessage(string.Format("-Address={0}, Port={1}, DeviceIndex={2}", nAddress, nPort, m_nDeviceIndex));

                        //Do disconnect...
                        if (m_bSocketConnected == true)
                            //ElanTouch.Disconnect();
                            ElanTouchSwitch.Disconnect(m_bSocketConnectType);

                        OutputMessage("-Start Socket Event CallBack");
                        //Set the callback function for socket server.
                        //ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                        //int nRet = ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);

                        int nResultFlag;

                        if (m_bSocketConnectType == false)
                        {
                            ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                            nResultFlag = ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);
                        }
                        else
                        {
                            ElanTouch_Socket.m_SocketEventCallBack = new ElanTouch_Socket.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                            nResultFlag = ElanTouch_Socket.ConnectSocket(nAddress, nPort, ElanTouch_Socket.m_SocketEventCallBack);
                        }

                        //if (nRet != ElanTouch_Socket.TP_SUCCESS)
                        if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                        {
                            m_bTPConnected = false;
                            m_bSocketConnected = false;
                            m_sErrorMessage = "Start Socket Server Failed";
                            return false;
                        }
                        else
                        {
                            m_bTPConnected = true;
                            m_bSocketConnected = true;
                        }

                        int nCount = 0;

                        while (m_bSocketWaitCallback == false)
                        {
                            if (m_cfrmParent.m_bExecute == false)
                            {
                                m_bTPConnected = false;
                                m_bSocketConnected = false;
                                return false;
                            }
                            /*
                            else if (nCount >= 1000)
                            {
                                m_bTPConnected = false;
                                m_bSocketConnected = false;
                                m_sErrMsg = "Waiting Socket Server Error";
                                return false;
                            }
                            */
                            else
                            {
                                if (nCount % 100 == 0)
                                    OutputMessage(string.Format("-Wait Socket Callback[Count={0}]", nCount));

                                try
                                {
                                    nCount++;
                                }
                                catch (OverflowException ex)
                                {
                                    string sMessage = ex.Message;

                                    nCount = 0;
                                }

                                Thread.Sleep(20);
                            }
                        }

                        //Thread.Sleep(1000);
                        m_cfrmParent.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show(m_cfrmParent, "Continue to Connect Chrome Device");
                        });

                        if (ParamFingerAutoTuning.m_nFingerReportTest == 1)
                        {
                            SaveFingerReport();

                            if (m_cfrmParent.m_bExecute == false)
                                return false;
                        }
                    }

                    SetSPICommandLength();

                    if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    {
                        //Check Device Connected Test
                        if (CheckChromeConnectTest() == false)
                            return false;
                    }

                    if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    {
                        Thread.Sleep(2000);
                        SetReset(bCheckCompleteFlag: true);
                    }

                    SetTestModeEnable(false, bForceFlag: true);
                }
#if _USE_9F07_SOCKET
                else
                {
                    /*
                    string sChromebookIPAddr = APSetting.GetInstance().m_ConnectParam.RemoteClientIPAddress;

                    if (cboSocketType.SelectedIndex == 0 && sChromebookIPAddr.Equals("0.0.0.0") == false)
                    {
                        ThreadPool.QueueUserWorkItem(xRunChromebookClientAPThread);
                    }
                    else
                    {
                        //Check the folder exist or not.
                        if (Directory.Exists(string.Format(@"{0}\RemoteClient", Application.StartupPath)) == true)
                            ThreadPool.QueueUserWorkItem(SocketConnectThreadFunc);
                        else
                        {
                            xConnectSocket();
                            btnConnectReset.Enabled = true;
                        }
                    }
                    */

                    if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_WINDOWS)
                    {
                        OutputMessage("-No Support 9F07 by Windows OS Type");
                        m_sErrorMessage = "No Support 9F07 by Windows OS Type";
                        return false;
                    }
                    else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT)
                    {
                        OutputMessage("-No Support 9F07 by Chrome(Previously) OS Type");
                        m_sErrorMessage = "No Support 9F07 by Chrome(Previously) OS Type";
                        return false;
                    }
                    else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
                    {
                        OutputMessage("-No Support 9F07 by Chrome OS Type / SSH Socket Server");
                        m_sErrorMessage = "No Support 9F07 by Chrome OS Type / SSH Socket Server";
                        return false;
                    }
                    else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL)
                    {
                        #region Reset Flow
                        bool bResultFlag = false;

                        //Check the folder exist or not.
                        if (Directory.Exists(string.Format(@"{0}\RemoteClient", Application.StartupPath)) == true && ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL)
                        {
                            //ThreadPool.QueueUserWorkItem(SocketConnectThreadFunc);
                            m_cfrmParent.Invoke((MethodInvoker)delegate
                            {
                                bResultFlag = ConnectSocket_9F07();
                            });
                        }
                        else
                        {
                            bResultFlag = ConnectSocket_9F07();
                        }

                        if (bResultFlag == false)
                            return false;

                        Thread.Sleep(1000);
                        #endregion

                        #region Connect Flow
                        ADBFileIO adbFileIO = new ADBFileIO(@"/data/local/tmp/thp/para.json", string.Format(@"{0}\para.json", Application.StartupPath));

                        int nRetryCount = 10;

                        for (int nRetryIndex = 0; nRetryIndex < nRetryCount; nRetryIndex++)
                        {
                            if (m_cfrmParent.m_bExecute == false)
                                return false;

                            if (adbFileIO.Pull() == false)
                            {
                                OutputMessage(string.Format("-No para.json file in phone device(RetryCount={0})", nRetryIndex));

                                if (nRetryIndex == nRetryCount - 1)
                                {
                                    m_sErrorMessage = "No para.json file in phone device";
                                    return false;
                                }
                                else
                                {
                                    Thread.Sleep(500);
                                    continue;
                                }
                            }
                            else
                            {
                                OutputMessage("-Get para.json file from phone device");
                                break;
                            }
                        }
                        #endregion
                    }
                }
#endif

                m_bTPConnected = true;

            }
            #endregion

            return true;
        }

#if _USE_9F07_SOCKET
        /// <summary>
        /// A thread to try to run a socket.
        /// </summary>
        private void SocketConnectThreadFunc(object objParam)
        {
            // define NewADB = false
            //APSetting Param = APSetting.GetInstance();
            //Param.m_ConnectParam.NewADB = false;
            //Param.Save();

            //xCheckRemoteServer();

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

        /// <summary>
        /// Start the Socket Server
        /// </summary>
        /// <returns></returns>
        private bool ConnectSocket_9F07()
        {
            int nPort = 0;
            int nAddress = 0;

            if (nPort == 0)
                nPort = ElanDefine.DEFAULT_SOCKET_PORT_9F07;

            //Do disconnect...
            if (m_bSocketConnected == true)
            {
                ElanTouchSwitch.Disconnect(m_bSocketConnectType);
                Thread.Sleep(ElanDefine.TIME_100MS);
            }

            /*
            //Start a thread to export the report and message to output list
            //J2 20180119 ++
            if (m_bStartDoOutput == false)
            {
                m_bStartDoOutput = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(OutputItem2OutputList));
            }
            //J2--

            if (cboSocketType.SelectedIndex == 0)
            {
                //Set the callback function for socket server.
                ElanTouch_Socket.m_SocketEventCallBack = new ElanTouch_Socket.PFUNC_SOCKET_EVENT_CALLBACK(SocketEvent);
            }
            else
            {
                //Set the 127.0.0.1 to IP Address when the user sets to execute socket client.
                nAddress = 0x7F000001;
            }
            */

            //Set the 127.0.0.1 to IP Address when the user sets to execute socket client.
            nAddress = 0x7F000001;

            //Set the callback function
            if (ElanTouchSwitch.CheckOutReportCallBackIsNull(m_bSocketConnectType) == true)
            {
                /*
                ElanTouch_Socket.m_OutReportCallBack = new ElanTouch_Socket.PFUNC_OUT_REPORT_CALLBACK(OutReportCallbackFunc);
                ElanTouch_Socket.OutReportRegCallback(ElanTouch_Socket.m_OutReportCallBack);
                */
                ElanTouchSwitch.SetOutReportRegCallback(this, m_bSocketConnectType);
            }

            if (ElanTouchSwitch.CheckInReportCallBackIsNull(m_bSocketConnectType) == true)
            {
                /*
                ElanTouch_Socket.m_InReportCallBack = new ElanTouch_Socket.PFUNC_IN_REPORT_CALLBACK(InReportCallbackFunc);
                ElanTouch_Socket.InReportRegCallback(ElanTouch_Socket.m_InReportCallBack);
                */
                ElanTouchSwitch.SetInReportRegCallback(this, m_bSocketConnectType);
            }

            //Establish the socket connect
            int nResultFlag = ElanTouchSwitch.ConnectSocket(nAddress, nPort, m_bSocketConnectType);

            if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
            {
                m_sErrorMessage = "Connect Server Failed";
                OutputMessage("-Connect Server Failed");
                return false;
            }

            // if it is SPI interface, need to set SPI command length
            // if it is not SPI interface, don't modify it
            if (IsSPIInterface(ParamFingerAutoTuning.m_nInterfaceType) == true)
                ElanTouchSwitch.SetSPICmdLength(m_bSocketConnectType, ParamFingerAutoTuning.m_nSPICommandLength);

            m_bSocketConnected = true;

            OutputMessage("-Remote Device Connected");

            return true;
        }

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
                sb.Append(string.Format(" {0:X2}", OutReportBuffer[i]));

            /*
            Invoke((MethodInvoker)delegate
            {
                m_OutputQueue.Enqueue(new OutputItem(sb.ToString(), OutputItem.OutputType.OUT));
            });
            */
        }

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
                    sb.Append(string.Format(" {0:X2}", OutReportBuffer[i]));

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

        private bool GetTraceInfo()
        {
            short nTXTrace = -1;
            short nRXTrace = -1;

            if (ElanDirectTochMainFlow.GetTraceInfo(ref nRXTrace, ref nTXTrace) == true)
            {
                if (m_bSocketConnectType == true)
                {
                    m_structTraceInfo_Socket = new ElanTouch_Socket.TraceInfo(1);
                    m_structTraceInfo_Socket.nXTotal = nRXTrace;
                    m_structTraceInfo_Socket.nYTotal = nTXTrace;
                    m_structTraceInfo_Socket.nPartialNum = 0;
                    m_structTraceInfo_Socket.XAxis = new int[] { nRXTrace, 0, 0, 0 };
                }
                else
                {
                    m_structTraceInfo = new ElanTouch.TraceInfo(1);
                    m_structTraceInfo.nXTotal = nRXTrace;
                    m_structTraceInfo.nYTotal = nTXTrace;
                    m_structTraceInfo.nPartialNum = 0;
                    m_structTraceInfo.XAxis = new int[] { nRXTrace, 0, 0, 0 };
                }

                m_nRXTraceNumber = nRXTrace;
                m_nTXTraceNumber = nTXTrace;
            }
            else
                return false;

            return true;
        }

        private bool IsSPIInterface(int nInterface)
        {
            return (nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING ||
                    nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE ||
                    nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING ||
                    nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE);
        }
#endif

        /// <summary>
        /// Connect to TP Device
        /// </summary>
        /// <returns></returns>
        private bool ConnectTP(int nAssignVID, int nAssignPID, int nDVDD, int nVIO, int nI2CAddress, int nNormalLength, bool bReconnect = false)
        {
            if (bReconnect == false)
            {
                //ElanTouch.Disconnect();
                ElanTouchSwitch.Disconnect(m_bSocketConnectType);
            }
            else
            {
                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    //ElanTouch.Disconnect();
                    ElanTouchSwitch.Disconnect(m_bSocketConnectType);
                }
            }

            /*
            int nInterface = ElanTouch.INTERFACE_WIN_HID;

            if (((int)UserInterfaceDef.InterfaceType.IF_I2C == ParamFingerAutoTuning.m_nWindowsSPIInterfaceType) ||
                ((int)UserInterfaceDef.InterfaceType.IF_I2C_TDDI == ParamFingerAutoTuning.m_nWindowsSPIInterfaceType))
                nInterface = ElanTouch.INTERFACE_WIN_BRIDGE_I2C;
            else if (((int)UserInterfaceDef.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE == ParamFingerAutoTuning.m_nWindowsSPIInterfaceType) ||
                     ((int)UserInterfaceDef.InterfaceType.IF_TDDI_SPI_MA_RISING_HALF_CYCLE == ParamFingerAutoTuning.m_nWindowsSPIInterfaceType))
                nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_RISING_HALF << 4);
            else if (((int)UserInterfaceDef.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE == ParamFingerAutoTuning.m_nWindowsSPIInterfaceType) ||
                     ((int)UserInterfaceDef.InterfaceType.IF_TDDI_SPI_MA_FALLING_HALF_CYCLE == ParamFingerAutoTuning.m_nWindowsSPIInterfaceType))
                nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_FALLING_HALF << 4);
            else if (((int)UserInterfaceDef.InterfaceType.IF_SPI_MA_RISING == ParamFingerAutoTuning.m_nWindowsSPIInterfaceType) ||
                     ((int)UserInterfaceDef.InterfaceType.IF_TDDI_SPI_MA_RISING == ParamFingerAutoTuning.m_nWindowsSPIInterfaceType))
                nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_RISING << 4);
            else if (((int)UserInterfaceDef.InterfaceType.IF_SPI_MA_FALLING == ParamFingerAutoTuning.m_nWindowsSPIInterfaceType) ||
                     ((int)UserInterfaceDef.InterfaceType.IF_TDDI_SPI_MA_FALLING == ParamFingerAutoTuning.m_nWindowsSPIInterfaceType))
                nInterface = (ElanTouch.INTERFACE_WIN_BRIDGE_SPI) + (int)(ElanTouch.INTF_TYPE_SPI_MA_FALLING << 4);
            else
                nInterface = ElanTouch.INTERFACE_WIN_HID;
            */
            int nInterface = ElanTouchSwitch.SetInterface(m_bSocketConnectType);

            /*
            if (ElanTouch.Connect(nAssignVID, nAssignPID, nInterface) != ElanTouch.TP_SUCCESS)
            {
                return false;
            }
            */
            if (ElanTouchSwitch.Connect(nAssignVID, nAssignPID, nInterface, nDVDD, nVIO, nI2CAddress, nNormalLength, m_bSocketConnectType) == false)
                return false;

            //Get Device Index
            int nSelectedDeviceIndex = -1;

            nSelectedDeviceIndex = GetDeviceIndex();

            if (nSelectedDeviceIndex == -1)
            {
                //ElanTouch.Disconnect();
                ElanTouchSwitch.Disconnect(m_bSocketConnectType);
                return false;
            }
            else
            {
                m_nDeviceIndex = nSelectedDeviceIndex;
                //MessageBox.Show(string.Format("DeviceIndex={0}", nDeviceIndex));
            }

            m_bTPConnected = true;

            return true;
        }

        private int GetDeviceIndex()
        {
            //When the same PID
            List<string> sElanDevice_List = new List<string>();
            int nSIZE_1K = 1024;
            int nSelectedDeviceIndex = -1;
            string sDEFAULT_EMPTY_DEVPATH = "NoTouch Device";
            byte[] bytePathName_Array = new byte[nSIZE_1K];
            IntPtr npPair = Marshal.AllocHGlobal(nSIZE_1K);

            //List all HID Device Path
            sElanDevice_List.Clear();
            //int nDeviceCount = ElanTouch.GetDeviceCount();
            int nDeviceCount = ElanTouchSwitch.GetDeviceCount(m_bSocketConnectType);
            //MessageBox.Show(string.Format("DeviceCount: {0}", nDeviceCount));

            for (int nDeviceIndex = 0; nDeviceIndex < nDeviceCount; nDeviceIndex++)
            {
                //if (ElanTouch.TP_SUCCESS == ElanTouch.GetHIDDevPath(pPair, cPathName.Length, i))
                if (ElanTouchSwitch.GetHIDDevPath(npPair, bytePathName_Array.Length, nDeviceIndex, m_bSocketConnectType) == true)
                {
                    Marshal.Copy(npPair, bytePathName_Array, 0, nSIZE_1K);
                    string sPath = ElanConvert.ByteArrayToCharString(bytePathName_Array);
                    sElanDevice_List.Add(sPath);
                }
                else
                    sElanDevice_List.Add(sDEFAULT_EMPTY_DEVPATH);
            }

            Marshal.FreeHGlobal(npPair);

            //Check Path List and select first correct Path
            //Fix bug that empty path at first device

            string sEmptyPath = sDEFAULT_EMPTY_DEVPATH;

            for (int nDeviceIndex = 0; nDeviceIndex < sElanDevice_List.Count; nDeviceIndex++)
            {
                //Zero: This instance has the same position in the sort order as value.
                if (sEmptyPath.CompareTo(sElanDevice_List[nDeviceIndex]) != 0)
                {
                    nSelectedDeviceIndex = nDeviceIndex;
                    //MessageBox.Show(string.Format("Get Device Index Success!![{0}]", nSelectedDeviceIndex));
                }
            }

            return nSelectedDeviceIndex;
        }

        /// <summary>
        /// A thread to try to run a socket.
        /// </summary>
        private bool RunSocketInstall()
        {
            if (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_RISING_HALF ||
                ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_RISING ||
                ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING_HALF ||
                ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING)
            {
                if (RunNativeSocketServer() == false)
                {
                    m_bTPConnected = false;
                    return false;
                }
            }
            else
            {
                if (RunPythonSocketServer() == false)
                {
                    m_bTPConnected = false;
                    return false;
                }
            }

            return true;
        }

        private bool RunPythonSocketServer()
        {
            #region Run the remote server
            if (m_cRunServerMgr != null)
            {
                m_cRunServerMgr.Terminate();
                m_cRunServerMgr.m_AndroidRemoteServerEvent = null;
                m_cRunServerMgr.m_OutputTextEvent = null;
                m_cRunServerMgr = null;
            }

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                if (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_I2C)    //I2C
                    m_cRunServerMgr = new ExecAndroidRemoteServer(3, false, false);
                else if (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_HIDOVERI2C)     //HID Over I2C
                    m_cRunServerMgr = new ExecAndroidRemoteServer(1, false, false);
            });

            m_cRunServerMgr.m_AndroidRemoteServerEvent = OnAndroidRemoteServerEvent;
            m_cRunServerMgr.m_OutputTextEvent = OnAndroidRemoteServerOutputText;

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                m_cfrmParent.SetButton(frmMain.FlowState.StopDisable);
            });

            m_cRunServerMgr.UpdateSHFile();

            Thread.Sleep(ParamFingerAutoTuning.m_nAfUpdateSHFileDelayTime);
            OutputMessage("-Update SH File Success");

            if (m_cRunServerMgr.Start() == false)
            {
                MessageBox.Show(m_cRunServerMgr.ErrorText, "Run Remote Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                m_cfrmParent.Invoke((MethodInvoker)delegate
                {
                    m_cfrmParent.SetButton(frmMain.FlowState.StopEnable);
                });

                m_sErrorMessage = string.Format("Run Remote Socket Error({0})", m_cRunServerMgr.ErrorText);
                return false;
            }
            #endregion

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                m_cfrmParent.SetButton(frmMain.FlowState.StopEnable);
            });

            OutputMessage("-Execute Remote Server Success");

            if (ConnectSocket() == false)
                return false;

            return true;
        }

        private bool RunNativeSocketServer()
        {
            if (m_cRunServerMgr != null)
            {
                m_cRunServerMgr.Terminate();
                m_cRunServerMgr.m_AndroidRemoteServerEvent = null;
                m_cRunServerMgr.m_OutputTextEvent = null;
                m_cRunServerMgr = null;
            }

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                m_cRunServerMgr = new ExecAndroidNativeServer(false);
            });

            m_cRunServerMgr.m_AndroidRemoteServerEvent = OnAndroidRemoteServerEvent;
            m_cRunServerMgr.m_OutputTextEvent = OnAndroidRemoteServerOutputText;

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                m_cfrmParent.SetButton(frmMain.FlowState.StopDisable);
            });

            if (m_cRunServerMgr.Start() == false)
            {
                MessageBox.Show(m_cRunServerMgr.ErrorText, "Run Remote SPI Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                m_cfrmParent.Invoke((MethodInvoker)delegate
                {
                    m_cfrmParent.SetButton(frmMain.FlowState.StopEnable);
                });

                m_sErrorMessage = string.Format("Run Remote SPI Socket Error({0})", m_cRunServerMgr.ErrorText);

                return false;
            }

            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                m_cfrmParent.SetButton(frmMain.FlowState.StopEnable);
            });

            OutputMessage("-Execute Remote SPI Server Success");

            if (ConnectSocket() == false)
                return false;

            return true;
        }

        /// <summary>
        /// Start the Socket Server
        /// </summary>
        /// <returns></returns>
        private bool ConnectSocket()
        {
            int nPort = 0;
            int nAddress = 0;

            //Int32.TryParse(tbPort.Text, out nPort);
            nPort = ParamFingerAutoTuning.m_nPort;

            if (nPort == 0)
                nPort = 9344;

            //Do disconnect...
            if (m_bSocketConnected == true)
                //ElanTouch.Disconnect();
                ElanTouchSwitch.Disconnect(m_bSocketConnectType);

            if (ParamFingerAutoTuning.m_sAndroidSocketType == MainConstantParameter.m_sANDROIDSOCKET_SERVER)    //Server
            {
                //Set the callback function for socket server.
                //ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(AndroidSocketEvent);

                if (m_bSocketConnectType == false)
                    ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(AndroidSocketEvent);
                else
                    ElanTouch_Socket.m_SocketEventCallBack = new ElanTouch_Socket.PFUNC_SOCKET_EVENT_CALLBACK(AndroidSocketEvent);
            }
            else                                    //Client
            {
                //Set the 127.0.0.1 to IP Address when the user sets to execute socket client.
                nAddress = 0x7F000001;
            }

            //Establish the socket connect
            //int nRet = ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);

            int nResultFlag;

            if (m_bSocketConnectType == false)
                nResultFlag = ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);
            else
                nResultFlag = ElanTouch_Socket.ConnectSocket(nAddress, nPort, ElanTouch_Socket.m_SocketEventCallBack);

            //if (nRet != ElanTouch_Socket.TP_SUCCESS)
            if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
            {
                if (ParamFingerAutoTuning.m_sAndroidSocketType == MainConstantParameter.m_sANDROIDSOCKET_SERVER)
                {
                    //lbStatus.Text = "Start Socket Server Failed.";
                    OutputMessage("-Start Socket Server Failed");
                    m_sErrorMessage = "Start Socket Server Failed";
                }
                else
                {
                    //lbStatus.Text = "Connect Server Failed.";
                    OutputMessage("-Connect Server Failed");
                    m_sErrorMessage = "Connect Server Failed";
                }

                return false;
            }

            m_bTPConnected = true;
            m_bSocketConnected = true;

            //Set the callback function
            //ElanTouch.m_OutReportCallBack = new ElanTouch.PFUNC_OUT_REPORT_CALLBACK(OutReportCallbackFunc);
            //ElanTouch.OutReportRegCallback(ElanTouch.m_OutReportCallBack);

            //ElanTouch.m_InReportCallBack = new ElanTouch.PFUNC_IN_REPORT_CALLBACK(InReportCallbackFunc);
            //ElanTouch.InReportRegCallback(ElanTouch.m_InReportCallBack);

            //Update the device status
            if (ParamFingerAutoTuning.m_sAndroidSocketType == MainConstantParameter.m_sANDROIDSOCKET_SERVER)
            {
                //lbStatus.Text = "Socket Server has been started. Waiting remote device connect...";
                OutputMessage("-Socket Server has been started. Waiting Remote Device Connect...");
                //btnDisconnect.Enabled = true;
            }
            else
            {
                //btnStartTest.Enabled = true;
                //btnDisconnect.Enabled = true;
                //lbStatus.Text = "The connection of remoted device has established.";
                OutputMessage("-The Connection of Remoted Device has Established");

                if (ParamFingerAutoTuning.m_nAndroidDisableGetReport == 0)
                {
                    //Socket 狀況下需啟動Thread讀取Finger Report
                    m_bRecvSocketFingerReport = true;
                    //Start a thread to receive the input report.(from socket)
                    ThreadPool.QueueUserWorkItem(OnInputReportReceive);

                    //OutputMessage("-Run OnInputReportReceive Thread");
                }
            }

            return true;
        }

        private void OnAndroidRemoteServerEvent(ExecAndroidRemoteServer.AndroidUsbErrorCode ErrorCode, string sText)
        {
            if (ErrorCode == ExecAndroidRemoteServer.AndroidUsbErrorCode.Terminated)
                OnAndroidRemoteServerTerminated();
            else
                Console.WriteLine(sText);
            //m_OutputQueue.Enqueue(new OutputItem(sText));
        }

        private void OnAndroidRemoteServerOutputText(string sText)
        {
            Console.WriteLine(sText);
            //m_OutputQueue.Enqueue(new OutputItem(sText));
        }

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

        private void ChromeSocketEvent(int nEventID)
        {
            switch (nEventID)
            {
                case 1:
                    m_bSocketWaitCallback = true;
                    m_cfrmParent.Invoke((MethodInvoker)delegate
                    {
                        /*
                        btnConnectTP.Enabled = true;
                        btnReK.Enabled = true;
                        SetDeviceStatus(string.Format("Remoted Device connected"));
                        */
                        OutputMessage("-Remoted Device Connected");
                        //MessageBox.Show("Remoted Device Connected");
                        m_bRecvSocketFingerReport = true;
                        ThreadPool.QueueUserWorkItem(OnInputReportReceive);
                    });
                    break;
                default:
                    m_bSocketWaitCallback = false;
                    m_bRecvSocketFingerReport = false;
                    m_cfrmParent.Invoke((MethodInvoker)delegate
                    {
                        /*
                        btnConnectTP.Enabled = false;
                        btnReK.Enabled = false;
                        SetDeviceStatus(string.Format("Socket Server has been started. Waiting remote device connect..."));
                        */
                        OutputMessage("-Socket Server has been Started. Waiting Remote Device Connect...");
                        //MessageBox.Show("Socket Server has been Started. Waiting Remote Device Connect...");
                    });
                    break;
            }
        }

        /// <summary>
        /// When the run android remote server batch file is exit, call this funciton.
        /// </summary>
        public void OnAndroidRemoteServerTerminated()
        {
            /*m_bStartDoOutput = false;
            m_bDevConnected = false;
            m_bContacted = false;*/

            //Unregister the callback function to receive the report
            /*
            ElanTouch.InputRawUnRegHIDCallback();
            Thread.Sleep(100);
            ElanTouch.m_InHIDReceiveCallBack = null;
            ElanTouch.m_OutReportCallBack = null;
            ElanTouch.m_InReportCallBack = null;

            ElanTouch.OutReportUnRegCallback();
            ElanTouch.InReportUnRegCallback();
            */

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

        /// <summary>
        /// 
        /// </summary>
        private void SetSPICommandLength()
        {
            if (ParamFingerAutoTuning.m_nDisableSetSPICommandLength == 1)
                return;

            if (((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE == ParamFingerAutoTuning.m_nInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING_HALF_CYCLE == ParamFingerAutoTuning.m_nInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE == ParamFingerAutoTuning.m_nInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING_HALF_CYCLE == ParamFingerAutoTuning.m_nInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING == ParamFingerAutoTuning.m_nInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING == ParamFingerAutoTuning.m_nInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING == ParamFingerAutoTuning.m_nInterfaceType) ||
                ((int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING == ParamFingerAutoTuning.m_nInterfaceType))
            {
                /*
                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    OutputMessage(string.Format("-Set SPI Command Length({0})", ParamFingerAutoTuning.m_nGen8SPICommandLength));
                    ElanTouchSwitch.SetSPICmdLength(m_bSocketConnectType, ParamFingerAutoTuning.m_nGen8SPICommandLength);
                }
                else
                {
                    OutputMessage(string.Format("-Set SPI Command Length({0})", ParamFingerAutoTuning.m_nSPICommandLength));
                    ElanTouchSwitch.SetSPICmdLength(m_bSocketConnectType, ParamFingerAutoTuning.m_nSPICommandLength);
                }
                */
                OutputMessage(string.Format("-Set SPI Command Length({0})", ParamFingerAutoTuning.m_nSPICommandLength));
                ElanTouchSwitch.SetSPICmdLength(m_bSocketConnectType, ParamFingerAutoTuning.m_nSPICommandLength);

                Thread.Sleep(m_nNormalDelayTime);
            }
        }

        /// <summary>
        /// Use the ssh client to logon chromebook. Execute the client ap.
        /// </summary>
        //private void xRunChromebookClientAPThread(object objParam)
        //private void RunSSHRemoteSocket(object objParam)
        private void RunSSHRemoteSocket()
        {
            string sRemoteClientIPAddress = ParamFingerAutoTuning.m_sChromeSSHSocketServerRemoteClientIPAddress;
            string sSSHUserName = ParamFingerAutoTuning.m_sChromeSSHSocketServerUserName;
            string sSSHPassword = ParamFingerAutoTuning.m_sChromeSSHSocketServerPassword;

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
            {
                sRemoteClientIPAddress = ParamFingerAutoTuning.m_sChromeSSHSocketServerRemoteClientIPAddress;
                sSSHUserName = ParamFingerAutoTuning.m_sChromeSSHSocketServerUserName;
                sSSHPassword = ParamFingerAutoTuning.m_sChromeSSHSocketServerPassword;
            }
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
            {
                sRemoteClientIPAddress = ParamFingerAutoTuning.m_sSSHSocketServerRemoteClientIPAddress;
                sSSHUserName = ParamFingerAutoTuning.m_sSSHSocketServerUserName;
                sSSHPassword = ParamFingerAutoTuning.m_sSSHSocketServerPassword;
            }

            string sHostIPAddress = "";

            if (m_bGetLocalIPAddressFlag == true)
                sHostIPAddress = m_sLocalIPAddress;
            else
            {
                sHostIPAddress = GetLocalIPAddress(sRemoteClientIPAddress);
                m_sLocalIPAddress = sHostIPAddress;
                m_bGetLocalIPAddressFlag = true;
            }

            int nInterface = ParamFingerAutoTuning.m_nInterfaceType;

            bool bDebugLogFlag = false;

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
                bDebugLogFlag = ParamFingerAutoTuning.m_nChromeSSHSocketServerDebugMode == 1 ? true : false;
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                bDebugLogFlag = ParamFingerAutoTuning.m_nSSHSocketServerDebugMode == 1 ? true : false;

            /*
            Invoke((MethodInvoker)delegate
            {
                nInterface = cboInterface.SelectedIndex;
            });
            */

            /*
            Invoke((MethodInvoker)delegate
            {
                m_OutputQueue.Enqueue(new OutputItem(string.Format("Try to do Ssh logon, ip Address = {0}", Param.RemoteClientIPAddress), OutputItem.OutputType.TEXT));
            });
            */
            OutputMessage(string.Format("-Try to Do SSH Logon, IP Address = {0}", sRemoteClientIPAddress));

            m_cElanSSHClient = new ElanSSHClient(sRemoteClientIPAddress, sSSHUserName, sSSHPassword);

            if (m_cElanSSHClient.Connect(bDebugLogFlag) == true)
            {
                /*
                Invoke((MethodInvoker)delegate
                {
                    m_OutputQueue.Enqueue(new OutputItem("Ssh logon success", OutputItem.OutputType.TEXT));
                    m_OutputQueue.Enqueue(new OutputItem(string.Format("Try to run client ap to connec to host. Host IP address = {0}", sHostIPAddress), OutputItem.OutputType.TEXT));
                });
                */
                OutputMessage("-SSH Logon Success");
                OutputMessage(string.Format("-Try to Run Client AP to Connect to Host. Host IP Address = {0}", sHostIPAddress));

                if (m_cElanSSHClient.EstablishRemoteConnect(sHostIPAddress, ElanDefine.DEFAULT_SOCKET_PORT.ToString(), nInterface) == false)
                {
                    /*
                    Invoke((MethodInvoker)delegate
                    {
                        m_OutputQueue.Enqueue(new OutputItem(string.Format("Run client ap failed. Fail reason = {0}", m_ChromeClient.FailReason), OutputItem.OutputType.WARNING));
                        btnConnectReset.Enabled = true;
                    });
                    */
                    OutputMessage(string.Format("-Run Client AP Failed. Fail Reason = {0}", m_cElanSSHClient.FailReason));

                    return;
                }

                /*
                Invoke((MethodInvoker)delegate
                {
                    m_OutputQueue.Enqueue(new OutputItem("Run client ap succeed.", OutputItem.OutputType.TEXT));
                    btnConnectReset.Enabled = true;
                });
                */
                OutputMessage("-Run Client AP Succeed.");
            }
            else
            {
                /*
                Invoke((MethodInvoker)delegate
                {
                    m_OutputQueue.Enqueue(new OutputItem("Run Ssh logon failed.", OutputItem.OutputType.WARNING));
                    btnConnectReset.Enabled = true;
                });
                */
                OutputMessage("-Run SSH Logon Failed.");
            }
        }

        /// <summary>
        /// 1.傳入Chrome端的IP Address
        /// 2.檢查Host端的IP Address,若大於1組,則比對IP Address前三碼
        /// 3.前三碼吻合則回傳ip address,若都不吻合,固定回傳第一個ip address
        /// </summary>
        /// <param name="sClientIPAddress"></param>
        /// <returns></returns>
        public string GetLocalIPAddress(string sClientIPAddress)
        {
            var vHost = Dns.GetHostEntry(Dns.GetHostName());
            List<string> sHostIP_List = new List<string>();

            foreach (var vIP in vHost.AddressList)
            {
                if (vIP.AddressFamily == AddressFamily.InterNetwork)
                {
                    sHostIP_List.Add(vIP.ToString());
                }
            }

            if (sHostIP_List.Count <= 0)
                return "";
            else if (sHostIP_List.Count == 1)
                return sHostIP_List[0];
            else
            {
                string[] sToken_Array = sClientIPAddress.Split('.');

                if (sToken_Array.Length < 4)
                    return sHostIP_List[0];

                foreach (string sCurrentIPAddress in sHostIP_List)
                {
                    string[] sCurrentToken_Array = sCurrentIPAddress.Split('.');

                    bool bMatchedFlag = true;

                    for (int i = 0; i < 3; i++)
                    {
                        if (sCurrentToken_Array[i].Equals(sToken_Array[i]) == false)
                        {
                            bMatchedFlag = false;
                            break;
                        }
                    }

                    if (bMatchedFlag == true)
                        return sCurrentIPAddress;
                }

                return sHostIP_List[0];
            }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckChromeConnectTest()
        {
            //Device Connected Test
            int nRetryCount = ParamFingerAutoTuning.m_nDeviceConnectTestCount;

            while (nRetryCount > 0 && m_cfrmParent.m_bExecute == true)
            {
                byte[] byteCommand_Array = new byte[] { 0x53, 0xF0, 0x00, 0x01 };

                OutputMessage("-Send Get FW_ID Test Command(0x53, 0xF0, 0x00, 0x01)");

                SendDevCommand(byteCommand_Array);

                byte[] byteData_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];
                //int nFWIDRet = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_nDevIdx);
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType);

                //if (nFWIDRet != ElanTouch.TP_SUCCESS)
                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                {
                    OutputMessage(string.Format("-Send Get FW_ID Test Command Error(0x{0})", nResultFlag.ToString("X4")));
                    m_cDebugLog.WriteLogToBuffer(string.Format("-TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(1000);
                    nRetryCount--;

                    if (nRetryCount <= 0)
                    {
                        m_sErrorMessage = "Send Get FW_ID Test Command Error";
                        return false;
                    }

                    if (m_cfrmParent.m_bExecute == false)
                        return false;

                    continue;
                }
                else
                {
                    OutputMessage("-Send Get FW_ID Test Command Success");
                    string sMessage = "-Get ACK :";

                    for (int nIndex = 0; nIndex < byteData_Array.Length; nIndex++)
                        sMessage += string.Format(" {0}", byteData_Array[nIndex].ToString("X2"));

                    m_cDebugLog.WriteLogToBuffer(sMessage);

                    if (m_cfrmParent.m_bExecute == false)
                        return false;

                    break;
                }
            }

            return true;
        }

        private bool ReconnectToTP()
        {
            if (m_eICGenerationType != ICGenerationType.Gen9)
            {
                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    DisconnectElanSSHClient();
            }

            OutputMessage("[State]Reconnect to TP");

            if (m_eICGenerationType != ICGenerationType.Gen9)
            {
        #region Reconnect to TP
                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                {
                    bool bConnectFailFlag = true;

                    for (int nRetryIndex = 0; nRetryIndex <= 3; nRetryIndex++)
                    {
                        ElanTouchSwitch.SetGetParameterTimeout();

                        m_nDeviceIndex = 0;

                        int nPort = 0;
                        int nAddress = 0;

                        m_bSocketWaitCallback = false;

                        //Int32.TryParse(tbPID.Text, out nPort);
                        nPort = ParamFingerAutoTuning.m_nPort;

                        if (nPort == 0)
                            nPort = ElanDefine.DEFAULT_SOCKET_PORT;

                        OutputMessage(string.Format("-Address={0}, Port={1}, DeviceIndex={2}", nAddress, nPort, m_nDeviceIndex));

                        //Do disconnect...
                        if (m_bSocketConnected == true)
                            //ElanTouch.Disconnect();
                            ElanTouchSwitch.Disconnect(m_bSocketConnectType);

                        OutputMessage("-Start Socket Event CallBack");
                        //Set the callback function for socket server.
                        //ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                        //int nRet = ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);

                        int nResultFlag;

                        if (m_bSocketConnectType == false)
                        {
                            ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                            nResultFlag = ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);
                        }
                        else
                        {
                            ElanTouch_Socket.m_SocketEventCallBack = new ElanTouch_Socket.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                            nResultFlag = ElanTouch_Socket.ConnectSocket(nAddress, nPort, ElanTouch_Socket.m_SocketEventCallBack);
                        }

                        //if (nRet != ElanTouch_Socket.TP_SUCCESS)
                        if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                        {
                            m_bTPConnected = false;
                            m_bSocketConnected = false;
                            m_sErrorMessage = "Start Socket Server Failed";
                            return false;
                        }
                        else
                        {
                            m_bSocketConnected = true;
                        }

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

                        Thread.Sleep(100);

                        int nLoopRetryCount = 0;

                        while (m_bSocketWaitCallback == false)
                        {
                            if (m_cfrmParent.m_bExecute == false)
                            {
                                m_bTPConnected = false;
                                m_bSocketConnected = false;
                                return false;
                            }
                            else
                            {
                                RunSSHRemoteSocket();
                                m_bTPConnected = true;
                            }

                            nLoopRetryCount++;

                            Thread.Sleep(100);

                            if (nLoopRetryCount > 10)
                            {
                                break;
                            }
                        }

                        if (m_bSocketWaitCallback == true)
                        {
                            bConnectFailFlag = false;
                            break;
                        }
                    }

                    if (bConnectFailFlag == true)
                    {
                        m_sErrorMessage = "Connect to TP by SSH Remote Socket Fail";
                        return false;
                    }

                    Thread.Sleep(2000);
                }
                else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_WINDOWS)
                {
                    //OutputMessage("Connect to TP Device");

                    int nVID = ParamFingerAutoTuning.m_nUSBVID;
                    int nPID = ParamFingerAutoTuning.m_nUSBPID;
                    int nDVDD = ParamFingerAutoTuning.m_nDVDD;
                    int nVIO = ParamFingerAutoTuning.m_nVIO;
                    int nI2CAddress = ParamFingerAutoTuning.m_nI2CAddress;
                    int nNormalLength = ParamFingerAutoTuning.m_nNormalLength;

                    if (ConnectTP(nVID, nPID, nDVDD, nVIO, nI2CAddress, nNormalLength, true) == false)
                    {
                        //MessageBox.Show("No Valid Device Found. Please Press Ok to Exit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //m_bCollectFlowError = true;
                        //m_sCollectFlowErrorMsg = "No Valid TP Device Found";
                        //OutputMessage("No Valid TP Device Found");

                        m_bTPConnected = false;
                        return false;
                    }
                }
                else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL)
                {
                    if (m_bSocketConnected == true)
                        Thread.Sleep(100);

                    CloseAndroidRemoteServer();

                    if (ConnectSocket() == false)
                    {
                        m_bTPConnected = false;
                        return false;
                    }
                }
                else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT)
                { }
                #endregion

                SetSPICommandLength();

                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    Thread.Sleep(2000);

                SetTestModeEnable(false, bForceFlag: true);

                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data);
            }

            return true;
        }
#else
        #region 主要連線方法

        /// <summary>
        /// 連接到觸控面板裝置,呼叫整合方法執行初次連接流程
        /// </summary>
        /// <returns>連接成功回傳true;連接失敗則回傳false</returns>
        private bool ConnectToTP()
        {
            return ConnectOrReconnectToTP(isReconnect: false);
        }

        /// <summary>
        /// 重新連接到觸控面板裝置,呼叫整合方法執行重新連接流程
        /// </summary>
        /// <returns>重新連接成功回傳true;連接失敗則回傳false</returns>
        private bool ReconnectToTP()
        {
            return ConnectOrReconnectToTP(isReconnect: true);
        }

        /// <summary>
        /// 連接或重新連接到觸控面板裝置,根據是否為重新連接及Socket類型執行對應的連接流程,完成後設定SPI命令長度、執行測試並關閉測試模式
        /// </summary>
        /// <param name="isReconnect">是否為重新連接旗標,true時執行重新連接流程,false時執行初次連接流程</param>
        /// <returns>連接成功並完成所有設定回傳true並設定TP連接狀態為true;連接失敗或測試失敗則回傳false</returns>
        private bool ConnectOrReconnectToTP(bool isReconnect)
        {
            // 先斷開現有連線
            if (m_bTPConnected && m_eICGenerationType != ICGenerationType.Gen9)
            {
                if (IsSSHSocketType())
                    DisconnectElanSSHClient();
            }

            if (m_bTPConnected && !isReconnect) return true;

            OutputMessage(isReconnect ? "[State]Reconnect to TP" : "[State]Connect to TP");

            #region Gen9 以外的連線處理
            if (m_eICGenerationType != ICGenerationType.Gen9)
            {
                bool bConnectSuccess = false;

                switch (ParamFingerAutoTuning.m_sSocketType)
                {
                    case MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER:
                    case MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER:
                        bConnectSuccess = isReconnect ? 
                            ReconnectSSHSocket() : 
                            ConnectSSHSocket();
                        break;

                    case MainConstantParameter.m_sSOCKET_WINDOWS:
                        bConnectSuccess = ConnectWindowsDevice();
                        break;

                    case MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT:
                    case MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL:
                        bConnectSuccess = isReconnect ? 
                            ReconnectAndroidDevice() : 
                            ConnectAndroidDevice();
                        break;

                    case MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT:
                        bConnectSuccess = ConnectChromeRemoteClient();
                        break;

                    default:
                        bConnectSuccess = false;
                        break;
                }

                if (!bConnectSuccess) return false;

                // 後處理設定
                SetSPICommandLength();

                if (IsChromeOrSSHSocketType())
                {
                    if (!CheckChromeConnectTest()) return false;
                }

                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                {
                    Thread.Sleep(2000);
                    SetReset(bCheckCompleteFlag: true);
                }

                SetTestModeEnable(false, bForceFlag: true);

                // Reconnect 專用處理
                if (isReconnect && ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                {
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data);
                }
            }
            #endregion

#if _USE_9F07_SOCKET
            #region Gen9 (9F07) 連線處理
            else
            {
                if (!ConnectGen9Device()) return false;
            }
            #endregion
#endif

            m_bTPConnected = true;
            return true;
        }

        #endregion

        #region 事件處理

        /// <summary>
        /// 處理Chrome Socket事件,EventID為1時表示遠端裝置已連接、設定回調完成旗標並啟動Finger Report接收執行緒,其他情況則重置旗標並等待裝置連接
        /// </summary>
        /// <param name="nEventID">事件識別碼,1表示裝置已連接,其他值表示等待連接</param>
        private void ChromeSocketEvent(int nEventID)
        {
            switch (nEventID)
            {
                case 1:
                    m_bSocketWaitCallback = true;
                    m_cfrmParent.Invoke((MethodInvoker)delegate
                    {
                        /*
                        btnConnectTP.Enabled = true;
                        btnReK.Enabled = true;
                        SetDeviceStatus(string.Format("Remoted Device connected"));
                        */
                        OutputMessage("-Remoted Device Connected");
                        // MessageBox.Show("Remoted Device Connected");
                        m_bRecvSocketFingerReport = true;
                        ThreadPool.QueueUserWorkItem(OnInputReportReceive);
                    });
                    break;
                default:
                    m_bSocketWaitCallback = false;
                    m_bRecvSocketFingerReport = false;
                    m_cfrmParent.Invoke((MethodInvoker)delegate
                    {
                        /*
                        btnConnectTP.Enabled = false;
                        btnReK.Enabled = false;
                        SetDeviceStatus(string.Format("Socket Server has been started. Waiting remote device connect..."));
                        */
                        OutputMessage("-Socket Server has been Started. Waiting Remote Device Connect...");
                        //MessageBox.Show("Socket Server has been Started. Waiting Remote Device Connect...");
                    });
                    break;
            }
        }

        #endregion

        #region 輔助方法

        /// <summary>
        /// 建立Socket連線,根據Socket連接類型註冊對應的Chrome Socket事件回調函式並執行連接
        /// </summary>
        /// <param name="nAddress">連接位址</param>
        /// <param name="nPort">連接埠號</param>
        /// <returns>回傳連接結果代碼,用於檢查觸控面板狀態</returns>
        private int EstablishSocketConnection(int nAddress, int nPort)
        {
            //Set the callback function for socket server.
            //ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
            //int nRet = ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);

            if (m_bSocketConnectType == false)
            {
                ElanTouch.m_SocketEventCallBack = new ElanTouch.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                return ElanTouch.ConnectSocket(nAddress, nPort, ElanTouch.m_SocketEventCallBack);
            }
            else
            {
                ElanTouch_Socket.m_SocketEventCallBack = new ElanTouch_Socket.PFUNC_SOCKET_EVENT_CALLBACK(ChromeSocketEvent);
                return ElanTouch_Socket.ConnectSocket(nAddress, nPort, ElanTouch_Socket.m_SocketEventCallBack);
            }
        }

        /// <summary>
        /// 設定SPI命令長度,若為SPI或TDDI SPI介面且未停用設定功能,則根據參數設定命令長度
        /// </summary>
        private void SetSPICommandLength()
        {
            if (ParamFingerAutoTuning.m_nDisableSetSPICommandLength == 1)
                return;

            if (IsSPIInterface(ParamFingerAutoTuning.m_nInterfaceType) ||
                ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING_HALF_CYCLE ||
                ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING_HALF_CYCLE ||
                ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING ||
                ParamFingerAutoTuning.m_nInterfaceType == (int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING)
            {
                /*
                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    OutputMessage(string.Format("-Set SPI Command Length({0})", ParamFingerAutoTuning.m_nGen8SPICommandLength));
                    ElanTouchSwitch.SetSPICmdLength(m_bSocketConnectType, ParamFingerAutoTuning.m_nGen8SPICommandLength);
                }
                else
                {
                    OutputMessage(string.Format("-Set SPI Command Length({0})", ParamFingerAutoTuning.m_nSPICommandLength));
                    ElanTouchSwitch.SetSPICmdLength(m_bSocketConnectType, ParamFingerAutoTuning.m_nSPICommandLength);
                }
                */
                OutputMessage(string.Format("-Set SPI Command Length({0})", ParamFingerAutoTuning.m_nSPICommandLength));
                ElanTouchSwitch.SetSPICmdLength(m_bSocketConnectType, ParamFingerAutoTuning.m_nSPICommandLength);

                Thread.Sleep(m_nNormalDelayTime);
            }
        }

        /// <summary>
        /// 檢查Chrome連線測試,發送取得FW_ID測試命令並讀取回應,包含重試機制驗證裝置連接是否正常
        /// </summary>
        /// <returns>測試命令發送及回應成功回傳true;重試次數用盡或使用者中斷執行則回傳false並設定錯誤訊息</returns>
        private bool CheckChromeConnectTest()
        {
            //Device Connected Test
            int nRetryCount = ParamFingerAutoTuning.m_nDeviceConnectTestCount;

            while (nRetryCount > 0 && m_cfrmParent.m_bExecute == true)
            {
                byte[] byteCommand_Array = new byte[] { 0x53, 0xF0, 0x00, 0x01 };

                OutputMessage("-Send Get FW_ID Test Command(0x53, 0xF0, 0x00, 0x01)");

                SendDevCommand(byteCommand_Array);

                byte[] byteData_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];
                //int nFWIDRet = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_nDevIdx);
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType);

                //if (nFWIDRet != ElanTouch.TP_SUCCESS)
                if (!ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType))
                {
                    OutputMessage(string.Format("-Send Get FW_ID Test Command Error(0x{0})", nResultFlag.ToString("X4")));
                    m_cDebugLog.WriteLogToBuffer(string.Format("-TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(1000);
                    nRetryCount--;

                    if (nRetryCount <= 0)
                    {
                        m_sErrorMessage = "Send Get FW_ID Test Command Error";
                        return false;
                    }

                    if (!m_cfrmParent.m_bExecute)
                        return false;

                    continue;
                }
                else
                {
                    OutputMessage("-Send Get FW_ID Test Command Success");
                    string sMessage = "-Get ACK :";

                    for (int nIndex = 0; nIndex < byteData_Array.Length; nIndex++)
                        sMessage += string.Format(" {0}", byteData_Array[nIndex].ToString("X2"));

                    m_cDebugLog.WriteLogToBuffer(sMessage);

                    if (!m_cfrmParent.m_bExecute)
                        return false;

                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// 檢查當前Socket類型是否為SSH Socket類型(Chrome SSH或Other SSH)
        /// </summary>
        /// <returns>若為SSH Socket類型回傳true,否則回傳false</returns>
        private bool IsSSHSocketType()
        {
            return ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER ||
                   ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER;
        }

        /// <summary>
        /// 檢查當前Socket類型是否為Chrome或SSH Socket類型(Chrome Remote Client、Chrome SSH或Other SSH)
        /// </summary>
        /// <returns>若為Chrome或SSH Socket類型回傳true,否則回傳false</returns>
        private bool IsChromeOrSSHSocketType()
        {
            return ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT ||
                   ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER ||
                   ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER;
        }

        /// <summary>
        /// 檢查指定的介面類型是否為SPI介面(包含Rising/Falling及Half Cycle變體)
        /// </summary>
        /// <param name="nInterface">介面類型代碼</param>
        /// <returns>若為SPI介面回傳true,否則回傳false</returns>
        private bool IsSPIInterface(int nInterface)
        {
            return nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING ||
                   nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE ||
                   nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING ||
                   nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE;
        }

        /// <summary>
        /// 將連線資訊(專案名稱、Socket類型、介面類型、IC方案)寫入至INI格式的連線資訊檔案
        /// </summary>
        /// <returns>固定回傳true(目前無失敗處理)</returns>
        private bool WriteConnectInfoFile()
        {
            IniFileFormat.WriteValue("Connect Information", "ProjectName", ParamFingerAutoTuning.m_sProjectName, m_cfrmParent.m_sConnectInfoFilePath, true, false);
            IniFileFormat.WriteValue("Connect Information", "SocketType", ParamFingerAutoTuning.m_sSocketType, m_cfrmParent.m_sConnectInfoFilePath, true, false);
            IniFileFormat.WriteValue("Connect Information", "Interface", ParamFingerAutoTuning.m_sInterfaceType, m_cfrmParent.m_sConnectInfoFilePath, true, false);
            IniFileFormat.WriteValue("Connect Information", "ICSolution", m_cfrmParent.m_sICSolutionName, m_cfrmParent.m_sConnectInfoFilePath, true, false);

            return true;
        }

        #endregion
#endif
    }
}
