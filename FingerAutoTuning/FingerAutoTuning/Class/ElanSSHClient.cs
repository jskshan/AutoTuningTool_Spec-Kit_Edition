using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elan;
using System.Windows.Forms;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Threading;
using FingerAutoTuning;
using FingerAutoTuningParameter;

namespace Elan
{
    public class ElanSSHClient
    {
        private string m_sREMOTE_CLIENT_DIR = "/usr/local/tmp";
        private const string m_sREMOTE_CLIENT_AP = "touch_data_agent";

        #region Fields
        /// <summary>
        /// The ssh client to send command and get the report data
        /// </summary>
        protected SshClient m_cSSHClient = null;
        protected ctmLog m_cDebugLog = null;

        protected BlockingQueue<string> m_qInputQueue = new BlockingQueue<string>();

        protected string m_sRemoteIPAddress = "";
        protected string m_sUserName = "";
        protected string m_sPassword = "";

        protected ShellStream m_cSSHStream = null;
        protected StreamReader m_cReadStream = null;
        protected StreamWriter m_cWriteStream = null;

        protected bool m_bReadStream = false;
        protected string m_sFailReason = "";
        /// <summary>
        /// The Fail Reason.
        /// </summary>
        public string FailReason
        {
            get { return m_sFailReason; }
        }
        #endregion

        public ElanSSHClient(string sIPAddress, string sUsername, string sPassword)
        {
            m_sRemoteIPAddress = sIPAddress;
            m_sUserName = sUsername;
            m_sPassword = sPassword;

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
                m_sREMOTE_CLIENT_DIR = "/usr/local/tmp";
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                m_sREMOTE_CLIENT_DIR = "/home/root/elan";
        }

        public bool Connect(bool bDebugFlag = false)
        {
            if (m_cSSHClient != null && m_cSSHClient.IsConnected)
                return false;

            if (bDebugFlag == true)
            {
                m_cDebugLog = new ctmLog();
                m_cDebugLog.Start(string.Format(@"{0}\Log\ChromeRemoteDebugLog.txt", Application.StartupPath));
            }

            try
            {
                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
                {
                    var vConnectionInfo = new KeyboardInteractiveConnectionInfo(m_sRemoteIPAddress, m_sUserName);
                    vConnectionInfo.AuthenticationPrompt += delegate(object sender, AuthenticationPromptEventArgs e)
                    {
                        foreach (var vPrompt in e.Prompts)
                            vPrompt.Response = m_sPassword;
                    };

                    m_cSSHClient = new SshClient(vConnectionInfo);
                }
                else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    m_cSSHClient = new SshClient(m_sRemoteIPAddress, m_sUserName, m_sPassword);

                m_cSSHClient.Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            m_cSSHStream = m_cSSHClient.CreateShellStream("", 80, 40, 80, 40, 1024 * 1024);

            m_bReadStream = true;
            m_cReadStream = new StreamReader(m_cSSHStream);
            m_cWriteStream = new StreamWriter(m_cSSHStream);

            //Create a thread to read the stream from ssh client.
            ThreadPool.QueueUserWorkItem(ReadStream, (object)m_cReadStream);

            m_cWriteStream.AutoFlush = true;

            return true;
        }

        private void WriteDebugMessage(string sMessage)
        {
            if (m_cDebugLog != null)
                m_cDebugLog.WriteLog(sMessage);

            Console.WriteLine(sMessage);
        }

        public bool EstablishRemoteConnect(string sHostIPAddress, string sPortNumber, int nInterface)
        {
            string sLine = "";
            string sInterface = "";
            string sCommandEstablishRemoteConnect = "";

            if (nInterface == (int)UserInterfaceDefine.InterfaceType.IF_I2C)
                sInterface = "5";
            else if (nInterface == (int)UserInterfaceDefine.InterfaceType.IF_HID_OVER_I2C_TP)
                sInterface = "4";
            else if (nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING ||
                     nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE ||
                     nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING ||
                     nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE)
                sInterface = "9";

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
            {
                if (nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING ||
                    nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE ||
                    nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING ||
                    nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE)
                    sInterface = "11";
            }

            sCommandEstablishRemoteConnect = string.Format("./touch_data_agent -t {0} -p 0 -s 0 -a {1} -n {2}", sInterface, sHostIPAddress, sPortNumber);

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
            {
                if (sInterface.Equals("9") == true)
                    sCommandEstablishRemoteConnect += " -i 0";
            }
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
            {
                if (sInterface.Equals("11") == true)
                    sCommandEstablishRemoteConnect += " -b 0 -i 0";
            }

            if (EnterRemoteClientFolder() == false)
            {
                WriteDebugMessage("Can't enter the temp folder");
                return false;
            }

            if (CheckRemoteAPExist() == false)
            {
                WriteDebugMessage(string.Format("{0} is not exist.", m_sREMOTE_CLIENT_AP));

                if (DoUploadRemoteAPFlow() == false)
                {
                    WriteDebugMessage("Upload remote client ap flow failed.");
                    return false;
                }
            }

            WriteDebugMessage(string.Format("[SSHClient] Send command to establish the remote connect, CMD : {0}", sCommandEstablishRemoteConnect));
            WriteStream(sCommandEstablishRemoteConnect, m_cWriteStream, m_cSSHStream);
            bool bSuccessFlag = false;

            while (true)
            {
                if (m_qInputQueue.Dequeue(1000, ref sLine) == false)
                    break;

                if (sLine.IndexOf("Service starts & will stop after key 'q' is pressed...") != -1)
                    bSuccessFlag = true;

                WriteDebugMessage(sLine);
            }

            return bSuccessFlag;
        }

        /// <summary>
        /// Enter to remote client folder
        /// </summary>
        /// <returns></returns>
        private bool EnterRemoteClientFolder()
        {
            bool bSuccessFlag = true;
            string sLine = "";
            string sCommandGotoTmpFolder = string.Format("cd {0}", m_sREMOTE_CLIENT_DIR);

            WriteDebugMessage(string.Format("[SSHClient] Send command to enter folder, CMD : {0}", sCommandGotoTmpFolder));
            WriteStream(sCommandGotoTmpFolder, m_cWriteStream, m_cSSHStream);

            while (true)
            {
                if (m_qInputQueue.Dequeue(100, ref sLine) == false)
                    break;

                if (sLine.IndexOf("No such file or directory") != -1)
                    bSuccessFlag = false;

                WriteDebugMessage(sLine);
            }

            return bSuccessFlag;
        }

        /// <summary>
        /// Check the remote ap exist
        /// </summary>
        /// <returns></returns>
        private bool CheckRemoteAPExist()
        {
            bool bSuccessFlag = true;
            string sLine = "";
            string sCommandCheckAPExist = string.Format("ls {0}", m_sREMOTE_CLIENT_AP);

            WriteDebugMessage(string.Format("[SSHClient] Send command to check ap exist, CMD : {0}", sCommandCheckAPExist));
            WriteStream(sCommandCheckAPExist, m_cWriteStream, m_cSSHStream);

            while (true)
            {
                if (m_qInputQueue.Dequeue(100, ref sLine) == false)
                    break;

                if (sLine.IndexOf("No such file or directory") != -1)
                    bSuccessFlag = false;

                WriteDebugMessage(sLine);
            }

            return bSuccessFlag;
        }

        private bool DoUploadRemoteAPFlow()
        {
            string sUploadFile = string.Format(@"{0}\RemoteClient\{1}", Application.StartupPath, m_sREMOTE_CLIENT_AP);
            string sDestinationFile = string.Format(@"{0}/{1}", m_sREMOTE_CLIENT_DIR, m_sREMOTE_CLIENT_AP);
            ElanSCPClient UploadFlow = new ElanSCPClient(m_sRemoteIPAddress, m_sUserName, m_sPassword);
            bool bSuccessFlag = true;
            string sLine = "";
            string sChangeAccessRight = string.Format("chmod 777 ./{0}", m_sREMOTE_CLIENT_AP);

            WriteDebugMessage("Do upload remote client ap flow...");

            if (UploadFlow.DoUpload(sUploadFile, sDestinationFile) == false)
                return false;

            WriteDebugMessage(string.Format("[SSHClient] Change the access right, CMD : {0}", sChangeAccessRight));
            WriteStream(sChangeAccessRight, m_cWriteStream, m_cSSHStream);

            while (true)
            {
                if (m_qInputQueue.Dequeue(100, ref sLine) == false)
                    break;

                if (sLine.IndexOf("No such file or directory") != -1)
                    bSuccessFlag = false;

                WriteDebugMessage(sLine);
            }

            return bSuccessFlag;
        }


        public void Disconnect()
        {
            if (m_cSSHClient.IsConnected == false)
                return;

            m_bReadStream = false;
            Thread.Sleep(20);
            m_cSSHClient.Disconnect();

            m_cSSHStream = null;
            m_cReadStream = null;
            m_cWriteStream = null;

            if (m_cDebugLog != null)
            {
                m_cDebugLog.Stop();
                m_cDebugLog = null;
            }
        }

        private void WriteStream(string sCommand, StreamWriter srWriter, ShellStream cStream)
        {
            srWriter.WriteLine(sCommand);
        }

        /// <summary>
        /// Read the stream from ssh
        /// </summary>
        /// <param name="objState"></param>
        /// <returns></returns>
        private void ReadStream(object objState)
        {
            StreamReader srReader = (StreamReader)objState;
            string sLine = "";
            Queue<string> qReportDataQueue = new Queue<string>();

            while (m_bReadStream == true)
            {
                sLine = srReader.ReadLine();

                if (sLine == null)
                {
                    Thread.Sleep(10);
                    continue;
                }

                m_qInputQueue.Enqueue(sLine);
                //Console.WriteLine(sLine);

                continue;
            }
        }
    }

    public class ElanSCPClient
    {
        #region Fields
        /// <summary>
        /// The ssh client to send command and get the report data
        /// </summary>
        protected ScpClient m_scpClient = null;

        protected string m_sHostName = "";
        protected string m_sUserName = "";
        protected string m_sPassword = "";

        protected bool m_bReadStream = false;
        #endregion

        public ElanSCPClient(string sHostName, string sUserName, string sPassword)
        {
            m_sHostName = sHostName;
            m_sUserName = sUserName;
            m_sPassword = sPassword;
        }

        /// <summary>
        /// Upload the remote client ap into chromebook via scp client.
        /// </summary>
        /// <param name="sSourceFile"></param>
        /// <param name="sDestinationPath"></param>
        /// <returns></returns>
        public bool DoUpload(string sSourceFile, string sDestinationPath)
        {
            if (m_scpClient != null && m_scpClient.IsConnected)
                return false;

            try
            {
                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
                {
                    var vConnectionInfo = new KeyboardInteractiveConnectionInfo(m_sHostName, m_sUserName);
                    vConnectionInfo.AuthenticationPrompt += delegate(object sender, AuthenticationPromptEventArgs e)
                    {
                        foreach (var vPrompt in e.Prompts)
                            vPrompt.Response = m_sPassword;
                    };

                    m_scpClient = new ScpClient(vConnectionInfo);
                }
                else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    m_scpClient = new ScpClient(m_sHostName, m_sUserName, m_sPassword);

                Thread.Sleep(1000);

                m_scpClient.Connect();

                Thread.Sleep(1000);

                FileInfo cUploadFileInfo = new FileInfo(sSourceFile);
                m_scpClient.Upload(cUploadFileInfo, sDestinationPath);

                Thread.Sleep(1000);

                m_scpClient.Disconnect();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }


            return true;
        }

        public void Disconnect()
        {
            if (m_scpClient.IsConnected == false)
                return;

            m_bReadStream = false;
            m_scpClient.Disconnect();

        }

        private void WriteStream(string sCommand, StreamWriter srWriter, ShellStream cStream)
        {
            srWriter.WriteLine(sCommand);
        }

        /// <summary>
        /// Read the stream from ssh
        /// </summary>
        /// <param name="objState"></param>
        /// <returns></returns>
        private void ReadStream(object objState)
        {
            StreamReader srReader = (StreamReader)objState;
            string sLine = "";
            Queue<string> qReportDataQueue = new Queue<string>();

            while (m_bReadStream == true)
            {
                sLine = srReader.ReadLine();

                if (sLine == null)
                    continue;

                Console.WriteLine(sLine);

                continue;
            }
        }
    }
}
