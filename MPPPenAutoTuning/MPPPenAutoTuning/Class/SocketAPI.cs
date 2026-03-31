using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Net.NetworkInformation;    //Test Method 11
using System.Runtime.InteropServices;   //Test Method 13
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class SocketAPI
    {
        private frmMain m_cfrmMain;
        private ProcessFlow m_cProcessFlow;

        #region Test Method 13
        /*
        [DllImport("wininet.dll", EntryPoint = "InternetFetConnectedState")]
        public extern static bool InternetGetConnectedState(out int conState, int reder);
        public static bool IsConnectedToInternet()
        {
            int nDesc = 0;
            return InternetGetConnectedState(out nDesc, 0);
        }
        */
        #endregion

        private bool m_bForceStopFlag = false;
        private bool m_bServerListeningStartFlag = false;
        private bool m_bDisconnectFlag = false;
        private bool m_bClientSocketConnectFlag = false;
        //private int m_nDisconnectCount = 0;

        public bool m_bServerDisconnectFlag = false;

        private RobotAPI m_cRobot = null;
        //private AppCore m_cAppCore = null;

        //private int m_nPort = 8800;
        private TcpListener m_tcplServerSocket = null;
        private TcpClient m_tcpcClientSocket = new TcpClient();

        //public EventHandler m_ehReceiveData = null;

        private static ManualResetEvent m_mreSendDone = new ManualResetEvent(false);
        private static ManualResetEvent m_mreReceiveDone = new ManualResetEvent(false);

        public EventHandler m_ehDataReceive = null;
        public EventHandler m_ehConnectNotify = null;

        private int m_nSocketErrorFlag = 0;
        private int m_nClientErrorFlag = 0;

        public SocketAPI(ref RobotAPI cRobot, frmMain cfrmMain)
        {
            m_cRobot = cRobot;
            m_cfrmMain = cfrmMain;
        }

        public void SetProcessFlow()
        {
            m_cProcessFlow = m_cfrmMain.m_cProcessFlow;
        }

        /*
        public void SetAppCore(ref AppCore cAppCore)
        {
            m_cAppCore = cAppCore;
        }
        */

        public void RunServerStarting()
        {
            ServicePointManager.Expect100Continue = false; 

            if (m_bServerListeningStartFlag == true)
            {
                if (m_tcplServerSocket != null)
                    m_tcplServerSocket.Stop();

                m_tcplServerSocket = null;
            }

            //Set the TcpListener
            //m_tcplServerSocket = new TcpListener(IPAddress.Any, port);
            m_tcplServerSocket = new TcpListener(IPAddress.Any, ParamAutoTuning.m_nPort);

            //Start Listening for Client Requests
            m_tcplServerSocket.Start();

            m_bServerListeningStartFlag = true;
            m_nSocketErrorFlag = 0;
        }

        public void RunServerListening()
        {
            m_bForceStopFlag = false;
            m_bDisconnectFlag = false;
            byte[] byteData_Array = new byte[1024];
            string sData;
            bool bSocketClient = true;

            while (m_bForceStopFlag == false)
            {
                if (!m_tcplServerSocket.Pending())
                {
                }
                else
                {
                    OutputMessage("-Server Socket Start");

                    m_tcpcClientSocket = m_tcplServerSocket.AcceptTcpClient();

                    OutputMessage("-Client Connect");

                    #region Test Method 11
                    if (RunNetworkPinConnectTest(1) == true)
                        bSocketClient = true;
                    else
                    {
                        bSocketClient = false;
                        m_bDisconnectFlag = true;
                        m_bForceStopFlag = true;
                        m_nSocketErrorFlag |= 0x01;
                    }

                    //bSocketClient = true;
                    m_bClientSocketConnectFlag = true;
                    //m_nDisconnectCount = 0;
                    #endregion

                    bool bConnectSuccessFlag = false;

                    #region Test Method 8
                    /*
                    Thread thread = new Thread(() =>
                    {
                        while (true)
                        {
                            string id = Guid.NewGuid().ToString();

                            AsyncCallback asynccallback = new AsyncCallback(GetShow);
                            IAsyncResult result = serverSocket.BeginAcceptSocket(asynccallback, id);
                            Socket socket = serverSocket.EndAcceptSocket(result);
                            result.AsyncWaitHandle.Close();

                            socket.Send(Encoding.UTF8.GetBytes(id));
 
                            Thread read_data = new Thread((obj) =>
                            {
                                Socket _s = obj as Socket;
                                if (_s != null)
                                {
                                    byte[] Newbytes = new byte[1024];
                                    int i;
                                    try
                                    {
                                        do
                                        {
                                            i = _s.Receive(Newbytes);
                                            if (i > 0)
                                                Console.WriteLine(id + ">>> " + Encoding.UTF8.GetString(Newbytes, 0, i));
                                        } while (i > 0);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Exit：【" + id + "】");
                                        DisconnectFlag = true;
                                        _s.Close();
                                    }
                                }
                            });
                            read_data.Start(socket);
                        }
                    });
                    thread.Start();
                    */
                    #endregion

                    //Infinite loop for listen client
                    while (bSocketClient)
                    {
                        if (m_bForceStopFlag == true)
                        {
                            m_bDisconnectFlag = true;
                            bSocketClient = false;
                            m_nSocketErrorFlag |= 0x04;
                        }
                        else
                        {
                            if (bConnectSuccessFlag == false || (bConnectSuccessFlag == true && CheckClientConnect() == true))
                            {
                                NetworkStream nsNetworkStream = null;
                                Array.Clear(byteData_Array, 0, byteData_Array.Length);

                                //Loop to receive all the data sent by the client
                                bool bDisconnectFlag = false;
                                int nValue = -1;

                                #region Other Try Method 2
                                //int nIndex = 0;
                                #endregion

                                try
                                {
                                    //Get a stream object for reading and writing
                                    nsNetworkStream = m_tcpcClientSocket.GetStream();

                                    #region Test Method 6
                                    if (ParamAutoTuning.m_nServerReadTimeout >= 0)
                                        nsNetworkStream.ReadTimeout = ParamAutoTuning.m_nServerReadTimeout;
                                    #endregion
                                }
                                catch(Exception ex)
                                {
                                    m_bDisconnectFlag = true;
                                    bDisconnectFlag = true;
                                    m_nSocketErrorFlag |= 0x02;

                                    OutputMessage(string.Format("-Client Disconnect[{0}]", ex.Message));
                                }

                                try
                                {
                                    nValue = nsNetworkStream.Read(byteData_Array, 0, byteData_Array.Length);

                                    #region Other Try Method 1
                                    /*
                                    IAsyncResult read = stream.BeginRead(bytes, 0, bytes.Length, new AsyncCallback(SendCallback), null);
                                    read.AsyncWaitHandle.WaitOne();
                                    stream.EndRead(read);
                                    */
                                    #endregion

                                    #region Other Try Method 2
                                    /*
                                    do
                                    {
                                        byte[] buff = new byte[1];
                                        stream.Read(buff, 0, 1);
                                        bytes[nIndex] = buff[0];
                                        nIndex++;
                                    }
                                    while (stream.DataAvailable || nIndex >= bytes.Length);
                                    */
                                    #endregion
                                }
                                catch (IOException ex)
                                {
                                    string sMessage = ex.Message;

                                    #region Test Method 6
                                    continue;
                                    /*
                                    stream.Close();
                                    bDisconnectFlag = true;
                                    */
                                    #endregion
                                }
                                catch (NullReferenceException ex)
                                {
                                    m_bDisconnectFlag = true;
                                    m_bForceStopFlag = true;
                                    bDisconnectFlag = true;
                                    m_nSocketErrorFlag |= 0x02;

                                    OutputMessage(string.Format("-Client Disconnect[{0}]", ex.Message));
                                }

                                if (bDisconnectFlag == false)
                                {
                                    string sRobotEcho = "";

                                    sData = System.Text.Encoding.ASCII.GetString(byteData_Array, 0, nValue);

                                    #region Other Try Method 1
                                    //data = System.Text.Encoding.ASCII.GetString(bytes);
                                    #endregion

                                    OutputMessage(string.Format("-From Client : {0}", sData));

                                    //TODO: Add robot control function for Server/Client version
                                    if (sData != "close client" && sData != "client stop" && sData != "client reconnect")
                                    { 
                                        m_cRobot.RunRobotCommand(sData, ref sRobotEcho);
                                    }

                                    /*
                                    if (sData != "close client") 
                                    { 
                                        sRobotEcho = "ok"; 
                                    }
                                    */

                                    /*
                                    if (sData == "IV") 
                                    { 
                                        sRobotEcho = "1"; 
                                    }
                                    */

                                    OutputMessage(string.Format("-To Client: {0}", sRobotEcho));

                                    if (sData == "close client")
                                    {
                                        bSocketClient = false;
                                        sRobotEcho = "closed";
                                        m_bForceStopFlag = true;
                                    }
                                    else if (sData == "client reconnect")
                                    {
                                        bSocketClient = false;
                                        sRobotEcho = "reconnect";
                                    }
                                    else if (sData == "client stop")
                                    {
                                        sRobotEcho = "stop";
                                        m_cfrmMain.SetNewConnectButton(false);
                                        m_cfrmMain.SetNewStopButton(false);
                                    }

                                    if (ParamAutoTuning.m_nServerWriteDelayTime >= 0)
                                        Thread.Sleep(ParamAutoTuning.m_nServerWriteDelayTime);

                                    //Response
                                    byte[] byteResponse_Array = System.Text.Encoding.ASCII.GetBytes(sRobotEcho);

                                    if (nsNetworkStream.CanWrite == true)
                                    {
                                        nsNetworkStream.Write(byteResponse_Array, 0, byteResponse_Array.Length);

                                        if (ParamAutoTuning.m_nServerWriteTimeout >= 0)
                                            nsNetworkStream.WriteTimeout = ParamAutoTuning.m_nServerWriteTimeout;

                                        #region Other Try Method 1
                                        /*
                                        IAsyncResult write = stream.BeginWrite(responseByte, 0, responseByte.Length, new AsyncCallback(SendCallback), null);
                                        write.AsyncWaitHandle.WaitOne();
                                        stream.EndWrite(write);
                                        */
                                        #endregion
                                    }

                                    nsNetworkStream.Flush();
                                    OutputMessage("-Response OK");
                                    bConnectSuccessFlag = true;
                                }
                            }
                            else
                            {
                                if (m_bDisconnectFlag == true)
                                {
                                    m_bForceStopFlag = true;
                                    bSocketClient = false;
                                }
                            }
                        }
                    }

                    #region Test Method 4
                    /*
                    bool isConnected = true;
                    NetworkStream stream = clientSocket.GetStream();
                    
                    while (clientSocket.Connected && isConnected && !ForceStopFlag)
                    {
                        stream.ReadTimeout = 60000;
                        clientSocket.Client.SendTimeout = 1000;
                        int bytesRead = 0;
                        
                        try
                        {
                            bytesRead = stream.Read(bytes, 0, bytes.Length);
                            
                            if (bytesRead == 1)
                            {
                                string robotEcho = "";

                                data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);

                                OutputMessage("-From Client : " + data);

                                // TODO: add robot control function for Server/Client version
                                if (data != "close client") { userRobot.Robot_Com(data, ref robotEcho); }
                                //if (data != "close client") { robotEcho = "ok"; }
                                //if (data == "IV") { robotEcho = "1"; }

                                OutputMessage("-To Client: " + robotEcho);

                                if (data == "close client")
                                {
                                    socketClient = false;
                                    robotEcho = "closed";
                                }

                                stream.ReadTimeout = 1000;

                                // response
                                byte[] responseByte = System.Text.Encoding.ASCII.GetBytes(robotEcho);
                                stream.Write(responseByte, 0, responseByte.Length);
                                stream.Flush();
                                OutputMessage("-Response OK"));
                            }

                            Thread.Sleep(10);
                        }
                        catch
                        {
                            bytesRead = 0;
                            isConnected = false;
                        }
                    }
                    */
                    #endregion

                    m_tcpcClientSocket.Close();
                    m_bClientSocketConnectFlag = false;
                    OutputMessage("-Client Disconnect");
                }
            }

            m_tcplServerSocket.Stop();
            //m_tcplServerSocket = null;

            if (m_bDisconnectFlag == true)
            {
                string sErrorMessage = "";
                string sErrorFlag = m_nSocketErrorFlag.ToString("x2").ToUpper();

                if ((m_nSocketErrorFlag & 0x04) != 0)
                    sErrorMessage = "Stop Button Pressed by User";
                else if ((m_nSocketErrorFlag & 0x01) != 0)
                    sErrorMessage = string.Format("Client Disconnect[0x{0}]. Please Check the Network and Close Client's Firewall", sErrorFlag);
                else if ((m_nSocketErrorFlag & 0x02) != 0)
                    sErrorMessage = string.Format("Client Disconnect[0x{0}]. Please Check the Network", sErrorFlag);

                /*
                Thread tForceStop = new Thread(() =>
                {
                    m_cAppCore.ForceStop(sErrorMessage);
                });
                
                tForceStop.IsBackground = true;
                tForceStop.Start();
                */

                //m_cAppCore.ForceStop(sErrorMessage);
                m_cProcessFlow.m_sErrorMessage = sErrorMessage;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
            }
        }

        public bool RunClientConnect()
        {
            m_bServerDisconnectFlag = false;

            ServicePointManager.Expect100Continue = false; 

            if (m_tcpcClientSocket.Connected == true)
            {
                string sCloseMessage = "client reconnect";
                RunClientSending(ref sCloseMessage);
                m_tcpcClientSocket.Close();
            }
            else
                m_tcpcClientSocket.Close();

            m_tcpcClientSocket = new TcpClient();

            m_tcpcClientSocket.NoDelay = true;

            //var varResult = m_tcpcClientSocket.BeginConnect(ParamAutoTuning.m_sServerIP, port, null, null);
            var varResult = m_tcpcClientSocket.BeginConnect(ParamAutoTuning.m_sServerIP, ParamAutoTuning.m_nPort, null, null);
            varResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(1000));

            if (!m_tcpcClientSocket.Connected)
            {
                return false;
            }

            /*
            try
            {
                m_tcpcClientSocket.Connect(ParamAutoTuning.m_sServerIP, port);
                //m_tcpcClientSocket.Connect("127.0.0.1", port);
            }
            catch (SocketException ex)
            {
                return false;
            }
            */

            OutputMessage("-Connect to Server");

            m_nClientErrorFlag = 0;

            return true;
        }

        public void RunClientSending(ref string sSendMessage)
        {
            if (m_tcpcClientSocket.Connected == false)
                return;

            #region Marked It.
            /*
            NetworkStream serverStream = m_tcpcClientSocket.GetStream();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(messageSend);

            try
            {
                serverStream.Write(outStream, 0, outStream.Length);
                if (ParamAutoTuning.m_nClientWriteTimeout >= 0)
                    serverStream.WriteTimeout = ParamAutoTuning.m_nClientWriteTimeout;
                serverStream.Flush();

                byte[] inStream = new byte[200];
                serverStream.Read(inStream, 0, inStream.Length);
                if (ParamAutoTuning.m_nClientReadTimeout >= 0)
                    serverStream.ReadTimeout = ParamAutoTuning.m_nClientReadTimeout;

                messageSend = System.Text.Encoding.ASCII.GetString(inStream);
            }
            catch (IOException ex)
            {
                serverStream.Close();
                m_tcpcClientSocket.Close();

                //Thread tForceStop = new Thread(() =>
                //{
                //    m_cAppCore.ForceStop("Server Disconnect. Please Check the Network");
                //});
                //tForceStop.IsBackground = true;
                //tForceStop.Start();
             
                m_cProcessFlow.m_sErrorMessage = "Server Disconnect. Please Check the Network";
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(20);
                return;
            }
            */
            #endregion

            bool bSendingSuccessFlag = true;
            int nRetryCount = 0;
            string sTempSendMessage = sSendMessage;
            byte[] byteInputStream_Array = new byte[1024];

            NetworkStream nsServerStream = m_tcpcClientSocket.GetStream();

            while (nRetryCount <= 3)
            {
                int nErrorFlag = 0;

                if (nRetryCount > 0)
                    Thread.Sleep(ParamAutoTuning.m_nClientSendingDelayTime);

                try
                {
                    byte[] arrbyteOutputStream = Encoding.ASCII.GetBytes(sTempSendMessage);
                    nErrorFlag++;

                    nsServerStream.Flush();
                    nErrorFlag++;

                    nsServerStream.Write(arrbyteOutputStream, 0, arrbyteOutputStream.Length);

                    if (ParamAutoTuning.m_nClientWriteTimeout >= 0)
                        nsServerStream.WriteTimeout = ParamAutoTuning.m_nClientWriteTimeout;

                    #region Other Try Method 1
                    /*
                    IAsyncResult write = serverStream.BeginWrite(outStream, 0, outStream.Length, new AsyncCallback(SendCallback), null);
                    write.AsyncWaitHandle.WaitOne();
                    serverStream.EndWrite(write);
                    */
                    #endregion

                    nErrorFlag++;

                    nsServerStream.Flush();
                    nErrorFlag++;

                    if (ParamAutoTuning.m_nClientWriteDelayTime >= 0)
                        Thread.Sleep(ParamAutoTuning.m_nClientWriteDelayTime);

                    Array.Clear(byteInputStream_Array, 0, byteInputStream_Array.Length);

                    #region Other Try Method 2
                    //int nIndex = 0;
                    #endregion

                    #region Mark It
                    /*
                    int nDetectCount = 0;
                    
                    while (!nsServerStream.DataAvailable)
                    {
                        Thread.Sleep(200);
                        nDetectCount++;

                        if (nDetectCount == 5)
                            break;
                    }
                    */
                    #endregion

                    nsServerStream.Read(byteInputStream_Array, 0, byteInputStream_Array.Length);

                    if (ParamAutoTuning.m_nClientReadTimeout >= 0)
                        nsServerStream.ReadTimeout = ParamAutoTuning.m_nClientReadTimeout;

                    #region Other Try Method 1
                    /*
                    IAsyncResult read = serverStream.BeginRead(inStream, 0, inStream.Length, new AsyncCallback(SendCallback), null);
                    read.AsyncWaitHandle.WaitOne();
                    serverStream.EndRead(read);
                    */
                    #endregion

                    #region Other Try Method 2
                    /*
                    do
                    {
                        byte[] buff = new byte[1];
                        serverStream.Read(buff, 0, 1);
                        inStream[nIndex] = buff[0];
                        nIndex++;
                    }
                    while (serverStream.DataAvailable || nIndex >= inStream.Length);
                    */
                    #endregion

                    nsServerStream.Flush();
                    nErrorFlag++;

                    sSendMessage = Encoding.ASCII.GetString(byteInputStream_Array);
                    nErrorFlag++;

                    bSendingSuccessFlag = true;
                    m_nClientErrorFlag = 0;
                    break;
                }
                catch (Exception ex)
                {
                    if (nsServerStream != null)
                        nsServerStream.Close();

                    nsServerStream = null;
                    bSendingSuccessFlag = false;
                    nRetryCount++;

                    if (m_nClientErrorFlag == 0)
                    {
                        if (nErrorFlag == 0)
                            m_nClientErrorFlag |= 0x01;
                        else if (nErrorFlag == 1)
                            m_nClientErrorFlag |= 0x02;
                        else if (nErrorFlag == 2)
                            m_nClientErrorFlag |= 0x04;
                        else if (nErrorFlag == 3)
                            m_nClientErrorFlag |= 0x08;
                        else if (nErrorFlag == 4)
                            m_nClientErrorFlag |= 0x10;
                        else if (nErrorFlag == 5)
                            m_nClientErrorFlag |= 0x20;
                        else
                            m_nClientErrorFlag |= 0x40;
                    }

                    m_bServerDisconnectFlag = true;

                    OutputMessage(string.Format("-Server Disconnect[{0}]", ex.Message));
                }
            }

            if (bSendingSuccessFlag == false)
            {
                //serverStream.Close();
                m_tcpcClientSocket.Close();

                string sErrorFlag = m_nClientErrorFlag.ToString("x2").ToUpper();

                /*
                Thread tForceStop = new Thread(() =>
                {
                    m_cAppCore.ForceStop(string.Format("Server Disconnect[0x{0}]. Please Check the Network", sErrorFlag), true, false, true);
                });

                tForceStop.IsBackground = true;
                tForceStop.Start();
                */

                m_bServerDisconnectFlag = true;

                m_cProcessFlow.m_sErrorMessage = string.Format("Server Disconnect[0x{0}]. Please Check the Network", sErrorFlag);
                m_cProcessFlow.m_cFinishFlowParameter.m_bOutputMessageFlag = true;
                m_cProcessFlow.m_cFinishFlowParameter.m_bCloseAPFlag = false;
                m_cfrmMain.m_bInterruptFlag = true;
                Thread.Sleep(10);
                return;
            }

            Thread.Sleep(ParamAutoTuning.m_nClientSendingDelayTime);
        }

        public bool CheckClientSocketConnect() 
        { 
            return m_bClientSocketConnectFlag; 
        }

        public bool GetClientConnect()
        {
            if (m_tcpcClientSocket.Connected == true)
                return true;
            else
                return false;
        }

        public bool CheckClientConnect()
        {
            #region Test Method 1
            /*
            byte[] testRecByte = new byte[1];

            try
            {
                bool bDisconnect = true;
                bool bPreConnected = clientSocket.Connected;

                if (clientSocket.Connected && clientSocket.Client.Poll(0, SelectMode.SelectRead))
                    bDisconnect = clientSocket.Client.Receive(testRecByte, SocketFlags.Peek) == 0;

                if (bDisconnect == true && bPreConnected == true)
                    DisconnectFlag = true;

                return !bDisconnect;
            }
            catch (SocketException ex)
            {
                return false;
            }
            */
            #endregion 

            #region Test Method 2
            // This is how you can determine whether a socket is still connected.
            /*
            bool bDisconnect = true;
            bool blockingState = clientSocket.Client.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                clientSocket.Client.Blocking = false;
                clientSocket.Client.Send(tmp, 0, 0);
                bDisconnect = false;
            }
            catch (SocketException ex)
            {
                // 10035 == WSAEWOULDBLOCK
                if (ex.NativeErrorCode.Equals(10035))
                    bDisconnect = false;
                else
                    bDisconnect = true;
            }
            finally
            {
                clientSocket.Client.Blocking = blockingState;
            }

            return !bDisconnect;
            
            //if (clientSocket.Connected == false)
                //return false;
            //else
                //return !bDisconnect;
            */
            #endregion

            #region Test Method 3
            /*
            serverSocket.BeginAcceptSocket(new AsyncCallback(AcceptCallback), serverSocket);
            return !DisconnectFlag;
            */
            #endregion

            #region Test Method 5
            /*
            try
            {
                byte[] tmp = new byte[1];
                clientSocket.Client.SendTimeout = 1000;
                clientSocket.Client.Send(tmp);
                Console.WriteLine("Connected!");
                return true;
            }
            catch(SocketException ex)
            {
                Console.WriteLine("disconnected!");
                DisconnectFlag = true;
                return false;
            }

            try
            {
                if (clientSocket != null && clientSocket.Client != null && clientSocket.Client.Connected)
                {
                    // As the documentation:
                    // When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                    // -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                    // -or- true if data is available for reading; 
                    // -or- true if the connection has been closed, reset, or terminated; 
                    // otherwise, returns false

                    // Detect if client disconnected
                    if (clientSocket.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        clientSocket.Client.ReceiveTimeout = 1000;
                        if (clientSocket.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected
                            Console.WriteLine("disconnected");
                            DisconnectFlag = true;
                            return false;
                        }
                        else
                        {
                            Console.WriteLine("connected");
                            return true;
                        }
                    }
                    Console.WriteLine("connected");
                    DisconnectFlag = true;
                    return true;
                }
                else
                {
                    Console.WriteLine("disconnected");
                    DisconnectFlag = true;
                    return false;
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("disconnected");
                DisconnectFlag = true;
                return false;
            }
            */
            #endregion

            #region Test Method 7
            /*
            byte[] testByte = new byte[1];
            bool closed = false;

            try
            {
                //使用Peek測試連線是否仍存在
                if (clientSocket.Connected && clientSocket.Client.Poll(0, SelectMode.SelectRead))
                    closed = clientSocket.Client.Receive(testByte, SocketFlags.Peek) == 0;
            }
            catch (SocketException ex)
            {
                closed = true;
            }

            if (closed == true)
                DisconnectFlag = true;
            return !closed;
            */
            #endregion

            #region Test Method 9
            /*
            byte[] testByte = new byte[1];
            
            if (clientSocket.Client.Poll(-1, SelectMode.SelectRead))
            {
                int nRead = clientSocket.Client.Receive(testByte, SocketFlags.Peek);
                if (nRead == 0)
                {
                    DisconnectFlag = true;
                    return false;
                }
                else
                    return true;
            }
            
            return false;
            */
            #endregion

            #region Test Method 10
            /*
            ServerSocketObject Tmp_ServerSocket = new ServerSocketObject();
            Tmp_ServerSocket.BeginAccept();
            if (Tmp_ServerSocket.IsConntect == false)
                DisconnectFlag = true;
            return !DisconnectFlag;
            */
            #endregion

            #region Test Method 11
            bool bConnectFlag = RunNetworkPinConnectTest();

            if (bConnectFlag == false)
                m_nSocketErrorFlag |= 0x02;

            return bConnectFlag;
            #endregion

            #region Test Method 12
            /*
            bool bClose = true;
            byte[] testByte = new byte[1];
            try
            {
                if (clientSocket.Connected && clientSocket.Client.Poll(0, SelectMode.SelectRead))
                    bClose = clientSocket.Client.Receive(testByte, SocketFlags.Peek) == 0;
            }
            catch
            {
                DisconnectFlag = true;
                bClose = true;
            }

            //if (bClose == true)
                //m_nDisconnectCount++;
            //else
                //m_nDisconnectCount = 0;

            //if (m_nDisconnectCount >= 10)
                //m_nDisconnectCount = true;

            return !bClose;
            */
            #endregion

            #region Test Method 13
            /*
            if (IsConnectedToInternet())
            {
                return true;
            }
            else
            {
                DisconnectFlag = true;
                return false;
            }
            */
            #endregion
        }

        public void RunSocketClose()
        {
            try
            {
                if (m_tcpcClientSocket.Connected == true)
                    m_tcpcClientSocket.Close();
            }
            catch
            {
            }

            if (m_tcplServerSocket != null)
                m_tcplServerSocket.Stop();
        }

        public void RunForceStop()
        {
            m_bForceStopFlag = true;
            m_nSocketErrorFlag |= 0x04;

            RunSocketClose();
        }

        public bool RunNetworkPinConnectTest(int nRetryCount = 4)
        {
            bool bConnectFlag = false;

            Ping cPing = new Ping();

            for (int nCountIndex = 0; nCountIndex < nRetryCount; nCountIndex++)
            {
                try
                {
                    if (cPing.Send(ParamAutoTuning.m_sClientIP, 1000).Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        bConnectFlag = true;
                        break;
                    }
                }
                catch
                {
                    bConnectFlag = false;
                    break;
                }
            }

            if (bConnectFlag == false)
            {
                m_bDisconnectFlag = true;
                return false;
            }
            else
                return true;
        }

        #region Test Method 3
        //State object for reading client data asynchronously
        public class StateObject
        {
            //Client  Socket.
            public Socket m_socketWorkSocket = null;
            //Size of receive buffer.
            public const int m_nBUFFERSIZE = 1024;
            //Receive buffer.
            public byte[] byteBuffer_Array = new byte[m_nBUFFERSIZE];
            //Received data string.
            //public StringBuilder m_sb = new StringBuilder();
        }

        public void AcceptCallback(IAsyncResult iarResult)
        {
            //Get the socket that handles the client request.
            TcpListener tcplListener = (TcpListener)iarResult.AsyncState;

            try
            {
                m_tcpcClientSocket.Client = tcplListener.EndAcceptSocket(iarResult);
            }
            catch
            {
                return;
            }

            //Create the State Object.
            StateObject cState = new StateObject();
            cState.m_socketWorkSocket = m_tcpcClientSocket.Client;

            //Start Receive the data from client
            m_tcpcClientSocket.Client.BeginReceive(cState.byteBuffer_Array, 0, StateObject.m_nBUFFERSIZE, 0, new AsyncCallback(ReadCallback), cState);

            //Wait another client connect again
            tcplListener.BeginAcceptSocket(new AsyncCallback(AcceptCallback), tcplListener);
        }

        public void ReadCallback(IAsyncResult iarResult)
        {
            String sContent = String.Empty;

            //Retrieve the state object and the handler socket
            //from the asynchronous state object.
            StateObject cState = (StateObject)iarResult.AsyncState;
            Socket scoketHandler = cState.m_socketWorkSocket;

            //Server disconnect
            if (scoketHandler.Connected == false)
            {
                //Console.WriteLine("[debug]Client Socket Disconnected!!");
                //Console.WriteLine("Waiting for a connection...");
                //m_SocketListener.BeginAccept(new AsyncCallback(AcceptCallback), m_SocketListener);
                Console.WriteLine("[debug]Server Disconnected!!");
                return;
            }

            //Read data from the client socket. 
            int nBytesRead = scoketHandler.EndReceive(iarResult);

            if (nBytesRead > 0)
            {
                sContent = Encoding.ASCII.GetString(cState.byteBuffer_Array, 0, nBytesRead);

                Console.WriteLine("[Debug] Receive data:" + sContent);

                //Receive next data from client
                scoketHandler.BeginReceive(cState.byteBuffer_Array, 0, StateObject.m_nBUFFERSIZE, 0, new AsyncCallback(ReadCallback), cState);
            }
            else
            {
                m_bDisconnectFlag = true;
                Console.WriteLine("[Debug] Client socket dissconnected.");
            }
        }
        #endregion

        #region Test Method 8
        private static void GetShow(IAsyncResult iarResult)
        {
            string sID = iarResult.AsyncState as string;
            Console.WriteLine("Enter：【" + sID + "】");
        }
        #endregion

        #region Test Method Else
        /*
        public bool Connected
        {
            get
            {
                if (clientSocket == null)
                {
                    DisconnectFlag = true;
                    return false;
                }
                try
                {
                    if ((clientSocket.Client.Poll(20, SelectMode.SelectRead)) && (clientSocket.Client.Available == 0))
                    {
                        DisconnectFlag = true;
                        return false;
                    }
                }
                //catch (SocketException ex)
                //{
                //    if (e.NativeErrorCode != 10035)
                //        return false;
                //}
                catch
                {
                    DisconnectFlag = true;
                    return false;
                }
                return true;
            }
        }
        */
        #endregion

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                //Retrieve the state object and the client socket 
                //from the asynchronous state object.
                StateObject cState = (StateObject)ar.AsyncState;
                Socket socketClient = cState.m_socketWorkSocket;

                if (socketClient.Connected == false)
                {
                    Console.WriteLine("[Debug] Socket disconnected");
                    m_mreReceiveDone.Set();
                    return;
                }

                //Read data from the remote device.
                int nBytesRead = socketClient.EndReceive(ar);

                if (nBytesRead > 0)
                {
                    //Get the rest of the data.
                    socketClient.BeginReceive(cState.byteBuffer_Array, 0, StateObject.m_nBUFFERSIZE, 0, new AsyncCallback(ReceiveCallback), cState);

                    if (m_ehDataReceive != null)
                        m_ehDataReceive(this, new ctmSocketArgs(cState.byteBuffer_Array, nBytesRead));
                }
                else //Server Disconnected
                {
                    if (m_ehConnectNotify != null)
                        m_ehConnectNotify(this, new SocketNotifyArgs(SocketNotifyArgs.m_nID_DISCONNECTED));

                    Console.WriteLine("[Debug] Socket Server disconnected");
                    m_mreReceiveDone.Set();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public class ctmSocketArgs : EventArgs
        {
            private byte[] m_byteData_Array;

            public byte[] Data
            {
                get { return m_byteData_Array; }
            }

            public int Length
            {
                get { return m_byteData_Array.Length; }
            }

            public ctmSocketArgs(byte[] byteData_Array, int nRecvDataLength)
            {
                m_byteData_Array = new Byte[nRecvDataLength];
                Array.Copy(byteData_Array, m_byteData_Array, nRecvDataLength);
            }
        }

        private static void SendCallback(IAsyncResult iarResult)
        {
            try
            {
                //Retrieve the socket from the state object.
                Socket socketClient = (Socket)iarResult.AsyncState;

                //Complete sending the data to the remote device.
                int nSent = socketClient.EndSend(iarResult);
                Console.WriteLine("Sent {0} bytes to server.", nSent);

                // Signal that all bytes have been sent.
                m_mreSendDone.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public class SocketNotifyArgs : EventArgs
        {
            public static int m_nID_CONNECTED = 0x001;
            public static int m_nID_DISCONNECTED = 0x002;
            public static int m_nID_CONNECT_ERROR = 0x003;

            private int m_nStateCode = 0;

            public int StateCode
            {
                get { return m_nStateCode; }
            }

            public SocketNotifyArgs(int nStateCode)
            {
                m_nStateCode = nStateCode;
            }
        }

        private void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmMain.OutputMessage(sMessage, bWarning);
        }
    }
}