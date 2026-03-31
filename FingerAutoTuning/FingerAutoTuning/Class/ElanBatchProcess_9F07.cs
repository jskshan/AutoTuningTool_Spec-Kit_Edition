using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using FingerAutoTuning;

namespace Elan
{
    public class ElanBatchProcess_9F07
    {
        public const string m_sFILETYPE_MAINPROC = "_SocketMainProc";
        public const string m_sFILETYPE_KEEPWAKEUPPROC = "_KeepWakeUpProc";

        /// <summary>
        /// A callback function declare. Use to alarm the ap that batch file exit.
        /// </summary>
        public delegate void ExitCallbackFuncPtr();

        private ExitCallbackFuncPtr m_ExitCallbackFuncPtr = null;
        /// <summary>
        /// User assign a function to it and call it when the batch file exit.
        /// </summary>
        public ExitCallbackFuncPtr ExitCallbackFunc
        {
            set { m_ExitCallbackFuncPtr = value; }
        }

        private Process m_Process = null;

        private bool m_bRunning = false;
        /// <summary>
        /// The process state.
        /// </summary>
        public bool IsRunning
        {
            get { return m_bRunning; }
        }

        private BlockingQueue<string> m_TextQueue = new BlockingQueue<string>();

        private ctmLog m_LogDebugInfo = null;

        public ElanBatchProcess_9F07(string sFileName, string sArgument, string sWorkingDirectory, bool bHideWindow)
        {
            m_bRunning = false;

            m_Process = new Process();
            ProcessStartInfo StartInfo = new ProcessStartInfo(sFileName, sArgument);
            StartInfo.WorkingDirectory = sWorkingDirectory;
            StartInfo.UseShellExecute = false;
            StartInfo.RedirectStandardOutput = true;
            StartInfo.RedirectStandardError = true;
            StartInfo.CreateNoWindow = bHideWindow;

            //Assign the start information
            m_Process.StartInfo = StartInfo;

            //Set the standard output and error output event function
            m_Process.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceived);
            m_Process.ErrorDataReceived += new DataReceivedEventHandler(ErrorDataReceived);
        }

        /// <summary>
        /// Start the process
        /// </summary>
        public void Start(bool bOutputDebugMessage = false, string sFileType = m_sFILETYPE_KEEPWAKEUPPROC)
        {
            if (m_bRunning == true)
            {
                Console.WriteLine("[Warning] m_Process is running");
                return;
            }

            if (m_Process == null)
            {
                Console.WriteLine("[Error] m_Process is null");
                return;
            }

            m_TextQueue.Clear();

            if (bOutputDebugMessage == true)
            {
                //ElanFunc.CreateLogFolder();

                m_LogDebugInfo = new ctmLog();

                m_LogDebugInfo.Start(string.Format(@"{0}\{1}\DebugLog\DebugLog{2}.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName, sFileType));
            }

            m_bRunning = m_Process.Start();
            m_Process.BeginOutputReadLine();
            m_Process.BeginErrorReadLine();

            ThreadPool.QueueUserWorkItem(new WaitCallback(CheckRunningThread));
        }

        private void CheckRunningThread(object objParam)
        {
            m_Process.WaitForExit();

            //Free all the resouse
            m_bRunning = false;
            m_TextQueue.Clear();
            lock (this)
            {
                if (m_LogDebugInfo != null)
                {
                    m_LogDebugInfo.Stop();
                    m_LogDebugInfo = null;
                }
            }

            //Call the function
            if (m_ExitCallbackFuncPtr != null)
                m_ExitCallbackFuncPtr();

            if (m_Process.HasExited == false)
            {
                m_Process.Kill();
            }
        }

        public void HideWindow()
        {
            if (m_Process == null || m_bRunning == false)
                return;

            if (m_Process.StartInfo.CreateNoWindow == true)
                return;

            try
            {
                FingerAutoTuning.Win32.ShowWindow(m_Process.MainWindowHandle, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Happn in hide process cmd window. Error Message : {0}", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Stop this process
        /// </summary>
        public void Stop()
        {
            if (m_bRunning == false)
                return;

            try
            {
                if (m_Process.StartInfo.CreateNoWindow == false)
                    m_Process.CloseMainWindow();
                m_Process.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error] Exception happen and error code is :{0}", ex.Message.ToString());
            }

            lock (this)
            {
                if (m_LogDebugInfo != null)
                {
                    m_LogDebugInfo.Stop();
                    m_LogDebugInfo = null;
                }
            }

            m_bRunning = false;
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            m_TextQueue.Enqueue(e.Data);
            lock (this)
            {
                if (m_LogDebugInfo != null)
                    m_LogDebugInfo.WriteLog(e.Data);
            }
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            m_TextQueue.Enqueue(e.Data);
            lock (this)
            {
                if (m_LogDebugInfo != null)
                    m_LogDebugInfo.WriteLog(e.Data);
            }
        }

        /// <summary>
        /// Read the output data that the batfile executing...
        /// </summary>
        /// <param name="sRetData"></param>
        /// <param name="nTimeout"></param>
        /// <returns></returns>
        public bool ReadOutputText(ref string sRetData, int nTimeout)
        {
            return m_TextQueue.Dequeue(nTimeout, ref sRetData);
        }
    }
}
