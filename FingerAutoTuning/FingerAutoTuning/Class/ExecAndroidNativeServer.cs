using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace Elan
{
    class ExecAndroidNativeServer : BaseExecServer
    {
        /// <summary>
        /// Constructor to create instance.
        /// </summary>
        /// <param name="nInterface"></param>
        /// <param name="bDebugMode"></param>
        public ExecAndroidNativeServer(bool bDebugMode)
        {
            m_bDebugMode = bDebugMode;
        }

        /// <summary>
        /// Start executing remote server.
        /// </summary>
        /// <returns></returns>
        public override bool Start()
        {
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

            while ((eErrorCode = RunAndroidSPIRemoteServer(m_bDebugMode)) != AndroidUsbErrorCode.Success)
            {
                //Python environment is not exist on the device
                if (eErrorCode == AndroidUsbErrorCode.PythonEnvironmentNotReady)
                {
                    MessageBox.Show("The python is not install. Please click \"OK\" to install python environment.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (m_AndroidRemoteServerEvent != null)
                        m_AndroidRemoteServerEvent(AndroidUsbErrorCode.PythonEnvironmentNotReady, "Setup the python environment, please wait...");

                    RunInstallAndSetupPythonEnvironment();
                }
                //The Native ap not install
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

                    foreach (Process cCurrentProcess in cADBProcess_Array)
                        cCurrentProcess.Kill();
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
        private AndroidUsbErrorCode RunAndroidSPIRemoteServer(bool bDebugMode)
        {
            m_cRunServerBatchProcess = new ElanBatchProcess(@"RemoteClient\RunSPIServer.bat", "", "", false);
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

                if (sOutputData.IndexOf("Waiting for Connections") != -1)
                    return AndroidUsbErrorCode.Success;
                else if (sOutputData.IndexOf("No such file or directory") != -1)
                {
                    m_sRunRemoteBatchErrorText = sOutputData;

                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("No native socket ap in device. Need to do install...");

                    CloseProcess();
                    return AndroidUsbErrorCode.RemoteAPNotInstall;
                }
                //Another error happen...
                else if (sOutputData.IndexOf("error", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    m_sRunRemoteBatchErrorText = sOutputData;

                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("Run the remote server fail. Some fatal error happen.");

                    CloseProcess();
                    return AndroidUsbErrorCode.RunRemoteServerError;
                }
            }

            /*
            if (m_OutputTextEvent != null)
                m_OutputTextEvent("Remote server is running...");

            return AndroidUsbErrorCode.Success;
            */
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
            ElanBatchProcess cSetupPythonProcess = new ElanBatchProcess(@"RemoteClient\InstallNativeServerAP.bat", "", "", true);
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
}
