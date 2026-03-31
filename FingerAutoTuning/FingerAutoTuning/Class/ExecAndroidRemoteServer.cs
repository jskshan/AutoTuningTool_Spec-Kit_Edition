using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Elan
{
    /// <summary>
    /// This class to setup and run the android remote server.
    /// </summary>
    public class ExecAndroidRemoteServer : BaseExecServer
    {
        /// <summary>
        /// Store the interface of TP
        /// </summary>
        private int m_nInterface = 0;

        private bool m_bI2CSysfs = false;

        /// <summary>
        /// Constructor to create instance.
        /// </summary>
        /// <param name="nInterface"></param>
        /// <param name="bDebugMode"></param>
        /// <param name="bI2CSysfs"></param>
        public ExecAndroidRemoteServer(int nInterface, bool bDebugMode, bool bI2CSysfs)
        {
            m_nInterface = nInterface;
            m_bDebugMode = bDebugMode;

            if (m_nInterface == (int)UserInterfaceDefine.InterfaceType.IF_I2C ||
                m_nInterface == (int)UserInterfaceDefine.InterfaceType.IF_I2C_TDDI)
                m_bI2CSysfs = bI2CSysfs;
            else
                m_bI2CSysfs = false;
        }

        public override void UpdateSHFile()
        {
            RunUpdateSHFile();
        }

        /// <summary>
        /// Start executing remote server.
        /// </summary>
        /// <returns></returns>
        public override bool Start()
        {
            //UpdateSHFile();
            return ExecRemoteServer();
        }

        /// <summary>
        /// Terminate remote server.
        /// </summary>
        public override void Terminate()
        {
            KeepWakeUpStop();
            CloseProcess();
        }

        #region Write the correct interface and parameter into sh file
        private void RunUpdateSHFile()
        {
            string sBusNumber = "";

            if (m_bI2CSysfs == true)
                sBusNumber = GetSysfsBusNumber();

            FileStream fsScript = new FileStream(string.Format(@"{0}\RemoteClient\Data\RunRemoteServer.sh", Application.StartupPath), FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] byteScriptData_Array = new byte[fsScript.Length];
            fsScript.Read(byteScriptData_Array, 0, byteScriptData_Array.Length);
            fsScript.Close();

            List<byte> byteScriptData_List = new List<byte>(byteScriptData_Array);
            #region Find the command line
            int nFindCommandIndex = -1;

            nFindCommandIndex = LastOfBytes(byteScriptData_List, new byte[] { (byte)'-', (byte)'t' });
            int nIFTypeCharIndex = nFindCommandIndex + 3;

            if (byteScriptData_List[nIFTypeCharIndex] >= '0' && byteScriptData_List[nIFTypeCharIndex] <= '9')
            {
                if (m_nInterface == (int)UserInterfaceDefine.InterfaceType.IF_I2C)
                    byteScriptData_List[nIFTypeCharIndex] = (byte)'3';
                else if (m_nInterface == (int)UserInterfaceDefine.InterfaceType.IF_HID_OVER_I2C_TP)
                    byteScriptData_List[nIFTypeCharIndex] = (byte)'6';
                else if (m_nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING ||
                         m_nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE ||
                         m_nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING ||
                         m_nInterface == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE)
                    byteScriptData_List[nIFTypeCharIndex] = (byte)'9';
            }
            else
            {
                Console.WriteLine("[Error] Interface parameter not found.");
                return;
            }
            #endregion

            #region I2C Sysfs mode
            for (int i = nFindCommandIndex; i < byteScriptData_List.Count; i++)
            {
                if (i >= byteScriptData_List.Count - 1)
                    continue;

                if (m_bI2CSysfs == false)
                {
                    if (byteScriptData_List[i] == '-' && byteScriptData_List[i + 1] == 's')
                    {
                        //Remove the char "-s "
                        //byteScriptData_List.RemoveRange(i, 3);

                        //Remove the bus number char
                        int nRemoveCount = 4;

                        for (int nStart = i + 3; nStart < byteScriptData_List.Count; nStart++)
                        {
                            if (byteScriptData_List[nStart] != ' ' &&
                                (byteScriptData_List[nStart] != 0x0a || byteScriptData_List[nStart + 1] != 0x0a))
                                nRemoveCount++;
                            else
                                break;
                        }

                        byteScriptData_List.RemoveRange(i - 1, nRemoveCount);
                        break;
                    }
                }
                else
                {
                    if (byteScriptData_List[i] == '-' && byteScriptData_List[i + 1] == 's')
                    {
                        char[] charData_Array = sBusNumber.ToArray();
                        List<byte> byteCurrentBusNumberChar_List = new List<byte>();
                        List<byte> byteInFileBusNumberChar_List = new List<byte>();

                        #region Find the bus number
                        for (int nIndex = i + 3; nIndex < byteScriptData_List.Count; nIndex++)
                        {
                            if (byteScriptData_List[nIndex] == ' ' ||
                                (byteScriptData_List[nIndex] == 0x0a && byteScriptData_List[nIndex + 1] == 0x0a))
                                break;

                            byteInFileBusNumberChar_List.Add(byteScriptData_List[nIndex]);
                        }

                        foreach (char charData in charData_Array)
                            byteCurrentBusNumberChar_List.Add((byte)charData);

                        byteScriptData_List.RemoveRange(i + 3, byteInFileBusNumberChar_List.Count);
                        byteScriptData_List.InsertRange(i + 3, byteCurrentBusNumberChar_List);

                        #endregion
                        break;
                    }
                    else if (byteScriptData_List[i] == 0x0a && byteScriptData_List[i + 1] == 0x0a)
                    {
                        List<byte> byteSysfsChar_List = new List<byte>();
                        //Insert the bus number
                        char[] charData_Array = sBusNumber.ToArray();

                        byteSysfsChar_List.Add((byte)' ');
                        byteSysfsChar_List.Add((byte)'-');
                        byteSysfsChar_List.Add((byte)'s');
                        byteSysfsChar_List.Add((byte)' ');

                        foreach (char charData in charData_Array)
                            byteSysfsChar_List.Add((byte)charData);

                        byteScriptData_List.InsertRange(i, byteSysfsChar_List);
                        break;
                    }
                }
            }
            #endregion

            #region Debug mode
            int nDgbCharIndex = IndexOfBytes(byteScriptData_List, new byte[] { (byte)'-', (byte)'d' }, nFindCommandIndex);

            if (m_bDebugMode == false)
            {
                if (nDgbCharIndex != -1)
                    byteScriptData_List.RemoveRange(nDgbCharIndex - 1, 3);
            }
            else
            {
                if (nDgbCharIndex == -1)
                {
                    nDgbCharIndex = IndexOfBytes(byteScriptData_List, new byte[] { 0x0a, 0x0a }, nFindCommandIndex);
                    byteScriptData_List.InsertRange(nDgbCharIndex, new byte[] { (byte)' ', (byte)'-', (byte)'d' });
                }
            }
            #endregion

            fsScript = new FileStream(string.Format(@"{0}\RemoteClient\Data\RunRemoteServer.sh", Application.StartupPath), FileMode.Create, FileAccess.Write, FileShare.Read);
            fsScript.Write(byteScriptData_List.ToArray(), 0, byteScriptData_List.Count);
            fsScript.Close();

            ElanBatchProcess cUpdateScriptFile = new ElanBatchProcess(@"RemoteClient/adb.exe",
                                                                      string.Format("push RemoteClient/Data/RunRemoteServer.sh /data/local/tmp/"),
                                                                      "",
                                                                      false);
            cUpdateScriptFile.Start();

            while (true)
            {
                string sOutputText = "";

                if (cUpdateScriptFile.IsRunning == false)
                    break;

                if (cUpdateScriptFile.ReadOutputText(ref sOutputText, 1000) == false)
                    continue;
            }

            cUpdateScriptFile.Stop();
            cUpdateScriptFile = null;
        }

        private string GetSysfsBusNumber()
        {
            string sBusNumber = "";

            ElanBatchProcess cGetBusNumberProcess = new ElanBatchProcess(@"RemoteClient\adb", "shell ls /sys/bus/i2c/devices", "", true);
            cGetBusNumberProcess.Start();

            while (true)
            {
                string sOutputText = "";

                if (cGetBusNumberProcess.ReadOutputText(ref sOutputText, 1000) == false)
                    continue;

                if (sOutputText == "" || sOutputText == null)
                    continue;

                if (m_OutputTextEvent != null)
                    m_OutputTextEvent(sOutputText);

                int nBusEndIndex = sOutputText.IndexOf("-0010", StringComparison.CurrentCultureIgnoreCase);
                int nSpaceIndex = sOutputText.LastIndexOf(' ');

                if (nSpaceIndex > nBusEndIndex)
                {
                    int nPreviousSpaceIndex = 0;

                    while (nPreviousSpaceIndex != -1)
                    {
                        nSpaceIndex = sOutputText.IndexOf(' ', nPreviousSpaceIndex);

                        if (nSpaceIndex > nBusEndIndex)
                        {
                            nSpaceIndex = nPreviousSpaceIndex - 1;
                            break;
                        }

                        nPreviousSpaceIndex = nSpaceIndex + 1;
                    }
                }

                if (nBusEndIndex >= 0)
                {
                    if (nSpaceIndex == -1)
                        nSpaceIndex = 0;
                    else
                        nSpaceIndex += 1;

                    sBusNumber = sOutputText.Substring(nSpaceIndex, nBusEndIndex - nSpaceIndex);

                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent(string.Format("Find the sysfs bus number = {0}", sBusNumber));

                    break;
                }
            }

            return sBusNumber;
        }
        #endregion

        /// <summary>
        /// Execute the android remote server
        ///     1.Check the remote server ap and library is existed in device.
        ///     2.Check the Python environment in device.
        ///     3.Execute python remote server ap and return the result
        /// </summary>
        /// <returns></returns>
        private bool ExecRemoteServer()
        {
            AndroidUsbErrorCode eErrorCode;

            while ((eErrorCode = RunAndroidRemoteServer(m_bDebugMode)) != AndroidUsbErrorCode.Success)
            {
                //Python environment is not exist on the device
                if (eErrorCode == AndroidUsbErrorCode.PythonEnvironmentNotReady)
                {
                    MessageBox.Show("The python is not install. Please click \"OK\" to install python environment.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (m_AndroidRemoteServerEvent != null)
                        m_AndroidRemoteServerEvent(AndroidUsbErrorCode.PythonEnvironmentNotReady, "Setup the python environment, please wait...");

                    RunInstallAndSetupPythonEnvironment();
                }
                //The library not install
                else if (eErrorCode == AndroidUsbErrorCode.RemoteAPNotInstall)
                {
                    MessageBox.Show("The remote AP is not install. Please click \"OK\" to install Remote AP", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (m_AndroidRemoteServerEvent != null)
                        m_AndroidRemoteServerEvent(eErrorCode, "Installing the AP, please wait...");

                    RunInstallRemoteAP();
                }
                //The socket error, kill all the adb.exe and re-launch remote server
                else if (eErrorCode == AndroidUsbErrorCode.SocketError)
                {
                    if (m_AndroidRemoteServerEvent != null)
                        m_AndroidRemoteServerEvent(eErrorCode, "Kill all the adb.exe, Rerun socket server...");

                    Process[] cADBProcess_Array = Process.GetProcessesByName("adb");

                    foreach (Process cCurentProcess in cADBProcess_Array)
                        cCurentProcess.Kill();
                }
                else //Execute the remote server with unknow error.
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Execute the remote server
        /// </summary>
        /// <returns></returns>
        private AndroidUsbErrorCode RunAndroidRemoteServer(bool bDebugMode)
        {
            m_cRunServerBatchProcess = new ElanBatchProcess(@"RemoteClient\RunServer.bat", "", "", false);
            m_cRunServerBatchProcess.ExitCallbackFunc = AndroidRemoteServerExit;
            m_cRunServerBatchProcess.Start(bDebugMode);

            //Get the process start tick
            long lStartTick = DateTime.Now.Ticks;

            while (true)
            {
                long lTimeIntervalTick = DateTime.Now.Ticks - lStartTick;

                if ((lTimeIntervalTick / 10000) > 15000)
                {
                    m_sRunRemoteBatchErrorText = "Run remote server timeout...";
                    CloseProcess();
                    return AndroidUsbErrorCode.Timeout;
                }

                string sOutputData = "";

                if (m_cRunServerBatchProcess.ReadOutputText(ref sOutputData, 1000) == false)
                    continue;

                if (sOutputData == null)
                    continue;

                if (sOutputData == "")
                    continue;

                //Hide the running cmd window
                m_cRunServerBatchProcess.HideWindow();

                //Show the text on the output window
                if (m_OutputTextEvent != null)
                    m_OutputTextEvent(sOutputData);

                //The server run success, break the while loop.
                if (sOutputData.Equals("Waiting for remote connect...") == true)
                    break;

                //Python environment is not ready...
                if (sOutputData.IndexOf("qpython-android5.sh", StringComparison.CurrentCultureIgnoreCase) >= 0 &&
                    sOutputData.IndexOf("not found", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("Python environment is unavailable.");

                    CloseProcess();
                    return AndroidUsbErrorCode.PythonEnvironmentNotReady;
                }

                //Another error happen...
                if (sOutputData.IndexOf("No such file or directory", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    m_sRunRemoteBatchErrorText = sOutputData;

                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("Run the remote server fail. Remote AP not install");

                    CloseProcess();
                    return AndroidUsbErrorCode.RemoteAPNotInstall;
                }

                //Another error happen...
                if (sOutputData.IndexOf("socket.error", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    m_sRunRemoteBatchErrorText = sOutputData;

                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("Run the remote server fail. Some sokcet error happen..");

                    CloseProcess();
                    return AndroidUsbErrorCode.SocketError;
                }

                //Another error happen...
                if (sOutputData.IndexOf("error", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    m_sRunRemoteBatchErrorText = sOutputData;

                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("Run the remote server fail. Some fatal error happen.");

                    CloseProcess();
                    return AndroidUsbErrorCode.RunRemoteServerError;
                }
            }

            if (m_OutputTextEvent != null)
                m_OutputTextEvent("Remote server is running...");

            return AndroidUsbErrorCode.Success;
        }

        /// <summary>
        /// When the run android remote server batch file is exit, call this funciton.
        /// </summary>
        private void AndroidRemoteServerExit()
        {
            if (m_cRunServerBatchProcess != null)
            {
                m_cRunServerBatchProcess.Stop();
            }

            if (m_AndroidRemoteServerEvent != null)
                m_AndroidRemoteServerEvent(AndroidUsbErrorCode.Terminated, "The remote server has been terminated.");
        }

        /// <summary>
        /// Execute the install procedure of remote AP
        /// </summary>
        /// <returns></returns>
        private bool RunInstallRemoteAP()
        {
            bool bInstallSuccess = false;
            ElanBatchProcess cSetupPythonProcess = new ElanBatchProcess(@"RemoteClient\InstallRemoteAP.bat", "", "", true);
            cSetupPythonProcess.Start();

            while (true)
            {
                string sOutputText = "";

                if (cSetupPythonProcess.ReadOutputText(ref sOutputText, 1000) == false)
                    continue;

                if (sOutputText == "")
                    continue;

                if (m_OutputTextEvent != null)
                    m_OutputTextEvent(sOutputText);

                if (sOutputText.IndexOf("Install finish", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    bInstallSuccess = true;

                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("Remote AP install finished.");

                    break;
                }
            }

            return bInstallSuccess;
        }

        /// <summary>
        /// Execute the installation procedure of python and setup the python environment
        /// </summary>
        /// <returns></returns>
        private bool RunInstallAndSetupPythonEnvironment()
        {
            bool bInstallSuccess = false;
            ElanBatchProcess cSetupPythonProcess = new ElanBatchProcess(@"RemoteClient\SetupQPython3Env.bat", "", "", true);
            cSetupPythonProcess.Start();

            while (true)
            {
                string sOutputText = "";

                if (cSetupPythonProcess.ReadOutputText(ref sOutputText, 1000) == false)
                    continue;

                if (sOutputText == "")
                    continue;

                if (m_OutputTextEvent != null)
                    m_OutputTextEvent(sOutputText);

                if (sOutputText.IndexOf("Setup QPython finish", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    bInstallSuccess = true;

                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("Wait the qpython environment setting...");
                    
                    MessageBox.Show("The python install is complete. Please check the python terminal window on android device. If it shows, click the \"OK\" button to continue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("Qpython environment set finished.");

                    break;
                }
            }

            return bInstallSuccess;
        }

        /// <summary>
        /// Close the process that current run
        /// </summary>
        private void CloseProcess()
        {
            if (m_cRunServerBatchProcess != null)
            {
                m_cRunServerBatchProcess.ExitCallbackFunc = null;
                m_cRunServerBatchProcess.Stop();
                m_cRunServerBatchProcess = null;

                Process[] cADBProcess_Array = Process.GetProcessesByName("adb");

                foreach (Process cCurrentProcess in cADBProcess_Array)
                {
                    cCurrentProcess.Dispose();
                    cCurrentProcess.Close();

                    try
                    {
                        cCurrentProcess.CloseMainWindow();
                        cCurrentProcess.Kill();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    public class BaseExecServer
    {
        /// <summary>
        /// The error code that execute android remote server.
        /// </summary>
        public enum AndroidUsbErrorCode
        {
            Success = 0,
            RunRemoteServerError,
            RemoteAPNotInstall,
            SocketError,
            PythonEnvironmentNotReady,
            Timeout,
            Terminated
        }

        public delegate void EventFuncPtr(AndroidUsbErrorCode eErrorCode, string sOutputText);
        public delegate void OutputTextFuncPtr(string sOutputText);

        #region Fields
        /// <summary>
        /// A callback function to output the command line content
        /// </summary>
        public OutputTextFuncPtr m_OutputTextEvent = null;

        /// <summary>
        /// User can assign a function to handle the event as follow:
        ///  -Run Remote Server Error.
        ///  -Remote AP Not Install
        ///  -Socket Error
        ///  -Python Environment Not Ready
        ///  -Timeout
        ///  -Terminated
        /// </summary>
        public EventFuncPtr m_AndroidRemoteServerEvent = null;

        protected string m_sRunRemoteBatchErrorText = "";
        /// <summary>
        /// Store the error description.
        /// </summary>
        public string ErrorText
        {
            get { return m_sRunRemoteBatchErrorText; }
        }

        /// <summary>
        /// Store the float of debug mode.
        /// </summary>
        protected bool m_bDebugMode = false;

        //A process to run android keep wake up process.
        protected ElanBatchProcess m_cKeepWakeUpProcess = null;

        //A process to run android remote server.
        protected ElanBatchProcess m_cRunServerBatchProcess = null;
        #endregion

        /// <summary>
        /// Start executing remote server.
        /// </summary>
        /// <returns></returns>
        public virtual bool Start()
        {
            return false;
        }

        /// <summary>
        /// Terminate remote server.
        /// </summary>
        public virtual void Terminate()
        {

        }

        /// <summary>
        /// Update SH File
        /// </summary>
        public virtual void UpdateSHFile()
        {

        }

        /// <summary>
        /// Keep Wake Up
        /// </summary>
        /// <returns></returns>
        public bool KeepWakeUp()
        {
            return ExecKeepWakeUp();
        }

        private bool ExecKeepWakeUp()
        {
            m_cKeepWakeUpProcess = new ElanBatchProcess(@"RemoteClient\KeepWakeUp.bat", "", "", true);
            m_cKeepWakeUpProcess.Start(true, ElanBatchProcess.m_sFILETYPE_KEEPWAKEUPPROC);

            while (true)
            {
                string sOutputTxt = "";

                if (m_cKeepWakeUpProcess.ReadOutputText(ref sOutputTxt, 5000) == false)
                    continue;

                if (sOutputTxt == null)
                {
                    m_cKeepWakeUpProcess.Stop();
                    return false;
                }

                if (sOutputTxt == "")
                    continue;

                if (m_OutputTextEvent != null)
                    m_OutputTextEvent(sOutputTxt);

                if (sOutputTxt.IndexOf("KeepWakeUp finish", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("KeepWakeUp finished.");

                    break;
                }
            }

            m_cKeepWakeUpProcess.Stop();
            return true;
        }

        public void KeepWakeUpStop()
        {
            if (m_cKeepWakeUpProcess != null)
            {
                m_cKeepWakeUpProcess.ExitCallbackFunc = null;
                m_cKeepWakeUpProcess.Stop();
            }
        }

        protected int IndexOfBytes(List<byte> byteData_List, byte[] byteValue_Array, int nStartIndex)
        {
            int nMatchedIndex = -1;
            int nValueLength = byteValue_Array.Length;

            for (int i = nStartIndex; i < byteData_List.Count - nValueLength; i++)
            {
                for (int nIndex = 0; nIndex < nValueLength; nIndex++)
                {
                    nMatchedIndex = i;

                    if (byteValue_Array[nIndex] != byteData_List[i + nIndex])
                    {
                        nMatchedIndex = -1;
                        break;
                    }
                }

                if (nMatchedIndex != -1)
                    break;
            }

            return nMatchedIndex;
        }

        protected int LastOfBytes(List<byte> byteData_List, byte[] byteValue_Array)
        {
            int nStartIndex = 0;
            int nLastMatchedIndex = -1;

            while (true)
            {
                nStartIndex = IndexOfBytes(byteData_List, byteValue_Array, nStartIndex);

                if (nStartIndex == -1)
                    break;

                nLastMatchedIndex = nStartIndex;
                nStartIndex += 1;
            }

            return nLastMatchedIndex;
        }
    }
}

