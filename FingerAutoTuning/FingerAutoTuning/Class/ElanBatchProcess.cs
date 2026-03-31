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
    public class ElanBatchProcess
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

        private Process m_cProcess = null;

        private bool m_bRunning = false;
        /// <summary>
        /// The process state.
        /// </summary>
        public bool IsRunning
        {
            get { return m_bRunning; }
        }

        private BlockingQueue<string> m_bqTextQueue = new BlockingQueue<string>();

        private ctmLog m_cLogDebugInfo = null;

        public ElanBatchProcess(string sFileName, string sArgument, string sWorkingDirectory, bool bHideWindow)
        {
            m_bRunning = false;

            m_cProcess = new Process();
            ProcessStartInfo psStartInfo = new ProcessStartInfo(sFileName, sArgument);
            psStartInfo.WorkingDirectory = sWorkingDirectory;
            psStartInfo.UseShellExecute = false;
            psStartInfo.RedirectStandardOutput = true;
            psStartInfo.RedirectStandardError = true;
            psStartInfo.CreateNoWindow = bHideWindow;

            //Assign the start information
            m_cProcess.StartInfo = psStartInfo;

            //Set the standard output and error output event function
            m_cProcess.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceived);
            m_cProcess.ErrorDataReceived += new DataReceivedEventHandler(ErrorDataReceived);
        }

        /// <summary>
        /// Start the process
        /// </summary>
        public void Start(bool bOutputDebugMessage = false, string sFileType = m_sFILETYPE_MAINPROC)
        {
            if (m_bRunning == true)
            {
                Console.WriteLine("[Warning] m_Process is running");
                return;
            }

            if (m_cProcess == null)
            {
                Console.WriteLine("[Error] m_Process is null");
                return;
            }

            m_bqTextQueue.Clear();

            //if (bOutputDebugMessage == true)
            if (true)
            {
                //ElanFunc.CreateLogFolder();

                m_cLogDebugInfo = new ctmLog();

                m_cLogDebugInfo.Start(string.Format(@"{0}\{1}\DebugLog\DebugLog{2}.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName, sFileType));
            }

            m_bRunning = m_cProcess.Start();
            m_cProcess.BeginOutputReadLine();
            m_cProcess.BeginErrorReadLine();

            ThreadPool.QueueUserWorkItem(new WaitCallback(CheckRunningThread));
        }

        private void CheckRunningThread(object objParameter)
        {
            m_cProcess.WaitForExit();

            //Free all the resouse
            m_bRunning = false;
            m_bqTextQueue.Clear();

            lock (this)
            {
                if (m_cLogDebugInfo != null)
                {
                    m_cLogDebugInfo.Stop();
                    m_cLogDebugInfo = null;
                }
            }

            //Call the function
            if (m_ExitCallbackFuncPtr != null)
                m_ExitCallbackFuncPtr();

            if (m_cProcess.HasExited == false)
            {
                m_cProcess.Kill();
            }
        }

        public void HideWindow()
        {
            if (m_cProcess == null || m_bRunning == false)
                return;

            if (m_cProcess.StartInfo.CreateNoWindow == true)
                return;

            try
            {
                FingerAutoTuning.Win32.ShowWindow(m_cProcess.MainWindowHandle, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Happn in hide process window. Error Message : {0}", ex.Message.ToString());
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
                if (m_cProcess.StartInfo.CreateNoWindow == false)
                    m_cProcess.CloseMainWindow();

                m_cProcess.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error] Exception happen and error code is :{0}", ex.Message.ToString());
            }

            lock (this)
            {
                if (m_cLogDebugInfo != null)
                {
                    m_cLogDebugInfo.Stop();
                    m_cLogDebugInfo = null;
                }
            }

            m_bRunning = false;
        }

        public void Stop(char charKey)
        {
            m_cProcess.WaitForInputIdle();
            IntPtr nptrH = m_cProcess.MainWindowHandle;
            //SetForegroundWindow(h);
            SendKeys.SendWait("k");
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            m_bqTextQueue.Enqueue(e.Data);

            lock (this)
            {
                if (m_cLogDebugInfo != null)
                    m_cLogDebugInfo.WriteLog(e.Data);
            }
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            m_bqTextQueue.Enqueue(e.Data);

            lock (this)
            {
                if (m_cLogDebugInfo != null)
                    m_cLogDebugInfo.WriteLog(e.Data);
            }
        }

        /// <summary>
        /// Read the output data that the batfile executing...
        /// </summary>
        /// <param name="sReturnData"></param>
        /// <param name="nTimeout"></param>
        /// <returns></returns>
        public bool ReadOutputText(ref string sReturnData, int nTimeout)
        {
            return m_bqTextQueue.Dequeue(nTimeout, ref sReturnData);
        }
    }

    public class BlockingQueue<T>
    {
        Queue<T> _queue = new Queue<T>();
        Semaphore _sem = new Semaphore(0, Int32.MaxValue);

        private int QueueCount()
        {
            int nCount = 0;

            lock (_queue)
            {
                nCount = _queue.Count;
            }
            return nCount;
        }

        public int Count
        {
            get
            {
                return QueueCount();
            }
        }

        public void Enqueue(T item)
        {
            lock (_queue)
            {
                _queue.Enqueue(item);
            }

            _sem.Release();
        }

        public void EnqueueAll(T[] pBuf, int num, int offset)
        {
            lock (_queue)
            {
                for (int i = 0; i < num; ++i)
                {
                    _queue.Enqueue(pBuf[i + offset]);
                }
            }
            _sem.Release(num);
        }

        public bool Dequeue(int timeout, ref T rValue)
        {
            if (_sem.WaitOne(timeout, true) == false)
                return false;                           // Timeout

            if (_queue.Count <= 0)
                return false;

            lock (_queue)
            {
                rValue = _queue.Dequeue();
            }

            return true;
        }

        public bool DequeueAll(int timeout, T[] pBuf, int num)
        {
            for (int i = 0; i < num; ++i)
            {
                if (_sem.WaitOne(timeout, true) == false)
                    return false;                           // Timeout

                lock (_queue)
                {
                    if (_queue.Count > 0)
                        pBuf[i] = _queue.Dequeue();
                    else
                        return false;
                }
            }

            return true;
        }

        public void Clear()
        {
            _sem.Release();

            _sem = new Semaphore(0, Int32.MaxValue);

            lock (_queue)
            {
                _queue.Clear();
            }
        }
    }
}
