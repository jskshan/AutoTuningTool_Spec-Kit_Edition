/*
using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net;

public class ServerSocketObject
{
    public Socket ClientToServerSocket;
    private Socket ListenSocket;
    private IPEndPoint ServerIPEndPoint;
    public ManualResetEvent BeginAccpetControl = new ManualResetEvent(false);
    //==========================
    private Boolean IsBeginAccept = true;
    public List<byte> Reply_Message = new List<byte>();
    public bool IsConntect = true;
    private int BufferSize = 1024;
    private byte[] buffer = new byte[1024];

    public ServerSocketObject()
    {
        string LocalIP = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
        ServerIPEndPoint = new IPEndPoint(IPAddress.Parse(LocalIP), 2266);
        ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ListenSocket.Bind(ServerIPEndPoint);
        ListenSocket.Listen(1);  //ip pool 設為 1
        ListenSocket.IOControl(IOControlCode.KeepAliveValues, KeepAlive(1, 1000, 1000), null);
        //將這個Socket使用<b style="background: rgb(255, 102, 255); color: rgb(0, 0, 0);">keep</b>-alive來保持長連線
        //KeepAlive函數參數說明: onOff:是否開啟<b style="background: rgb(255, 102, 255); color: rgb(0, 0, 0);">Keep</b>-Alive(開 1/ 關 0) ,
        //keepAliveTime:當開啟<b style="background: rgb(255, 102, 255); color: rgb(0, 0, 0);">keep</b>-Alive後經過多久時間(ms)開啟偵測
        //keepAliveInterval: 多久偵測一次(ms)
    }
    private byte[] KeepAlive(int onOff, int keepAliveTime, int keepAliveInterval)
    {
        byte[] Tmp_buffer = new byte[12];
        BitConverter.GetBytes(onOff).CopyTo(Tmp_buffer, 0);
        BitConverter.GetBytes(keepAliveTime).CopyTo(Tmp_buffer, 4);
        BitConverter.GetBytes(keepAliveInterval).CopyTo(Tmp_buffer, 8);
        return Tmp_buffer;
    }
    public void BeginAccept()
    {
        while (IsBeginAccept)
        {
            BeginAccpetControl.Reset();
            ListenSocket.BeginAccept(new AsyncCallback(BeginAcceptCallBack), ListenSocket);
            BeginAccpetControl.WaitOne(); //等待Clinet...
        }
        //===================
    }
    private void BeginAcceptCallBack(IAsyncResult state)
    {
        Socket Listener = (Socket)state.AsyncState;
        ClientToServerSocket = Listener.EndAccept(state); //Client連線成功
        ClientToServerSocket.BeginReceive(buffer, 0, BufferSize, 0, new AsyncCallback(ReceivedCallBack), ClientToServerSocket);
        //連線成功後 開始接收Server所傳遞的資料
        BeginAccpetControl.Set();
    }
    private void ReceivedCallBack(IAsyncResult ar)
    {
        Socket state = (Socket)ar.AsyncState;
        if (state.Connected == true)
        {
            try
            {
                int bytesRead = state.EndReceive(ar);
                if (bytesRead > 0) //當 bytesRead大於0時表示Server傳遞資料過來 等於0時代表Client"正常"斷線
                {
                    for (int num = 0; num < bytesRead; num++)
                    {
                        Reply_Message.Add(buffer[num]); //收集資料
                    }
                    state.BeginReceive(buffer, 0, BufferSize, 0, new AsyncCallback(ReceivedCallBack), ClientToServerSocket);
                }
                else
                {
                    //處理Client端"正常"斷線的事件
                }
            }
            catch (Exception ex)
            {
                //這裡就是當設定好<b style="background: rgb(255, 102, 255); color: rgb(0, 0, 0);">Keep</b>-alive後, 假設Clinet端發生"不正常斷線(網路異常)"時, 將會
                //跑進來這個Exception裡,再加以處理
                state.Shutdown(SocketShutdown.Both);
                state.Close();
            }
        }
        else
        {
            IsConntect = false;
            state.Close();
        }
    }
}
*/
