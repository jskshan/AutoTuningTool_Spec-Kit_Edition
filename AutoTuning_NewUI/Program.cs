using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
//using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace AutoTuning_NewUI
{
    static class Program
    {
        /*
        [DllImport("Shcore.dll")]
        static extern int SetProcessDpiAwareness(int PROCESS_DPI_AWARENESS);

        // According to https://msdn.microsoft.com/en-us/library/windows/desktop/dn280512(v=vs.85).aspx
        private enum DpiAwareness
        {
            None = 0,
            SystemAware = 1,
            PerMonitorAware = 2
        }
        */

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // check process existed
            string ProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] p = Process.GetProcessesByName(ProcessName);
            if (p.Length > 1)
            {
                MessageBox.Show("AutoTuningTool AP already existed");
            }
            else
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //SetProcessDpiAwareness((int)DpiAwareness.PerMonitorAware);

                Application.Run(new frmMain());
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                File.AppendAllText(string.Format(@"{0}\\APCrash.txt", Application.StartupPath), e.ExceptionObject.ToString(), Encoding.UTF8);
            }
            catch
            {

            }
        }
    }
}
