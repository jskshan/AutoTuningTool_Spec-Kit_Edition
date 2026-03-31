using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        /// <summary>
        /// 轉換 HDF5 資料格式
        /// </summary>
        /// <param name="sCsvFilePath">來源 CSV 檔案完整路徑</param>
        /// <param name="sH5DirPath">HDF5 輸出目錄路徑</param>
        /// <returns>true: 轉換成功; false: 轉換失敗</returns>
        private bool ConvertHDF5Data(string sCsvFilePath, string sH5DirPath)
        {
            bool bErrorFlag = false;
            int nErrorCode = 0;
            /*
            bool bSystemType = Environment.Is64BitOperatingSystem;
            string sToolName = "";
            
            if (bSystemType == true)
                sToolName = "HDF5ConvertTool.exe";
            else
                sToolName = "HDF5ConvertTool.exe";
            */
            string sFormatType = Convert.ToString(5);

            string sOutputText = string.Empty;
            string sErrorMessage = string.Empty;
            Process ConvertHDF5Proc = new Process();
            ProcessStartInfo ConvertHDF5ProcStartInfo = new ProcessStartInfo();
            ConvertHDF5Proc.StartInfo.FileName = string.Format(@"{0}\HDF5ConvertTool\HDF5ConvertTool.exe", Application.StartupPath);
            //ConvertHDF5Proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //ProcessWindowStyle.Normal;
            ConvertHDF5Proc.StartInfo.Arguments = string.Format("-f {0} -c \"{1}\" -h \"{2}\"", sFormatType, sCsvFilePath, sH5DirPath);
            ConvertHDF5Proc.StartInfo.UseShellExecute = false;
            ConvertHDF5Proc.StartInfo.RedirectStandardOutput = true;
            ConvertHDF5Proc.StartInfo.RedirectStandardError = true;
            ConvertHDF5Proc.StartInfo.CreateNoWindow = true;

            try
            {
                ConvertHDF5Proc.Start();
                ConvertHDF5Proc.WaitForExit();
                nErrorCode = ConvertHDF5Proc.ExitCode;
                sOutputText = ConvertHDF5Proc.StandardOutput.ReadToEnd();
                //sOutputText = sOutputText.Replace(Environment.NewLine, string.Empty);
                sErrorMessage = ConvertHDF5Proc.StandardError.ReadToEnd();
            }
            catch (Exception ex)
            {
                string sMessage = ex.Message;

                nErrorCode = -999;
                sErrorMessage = "Execute HDF5 Exe File Error";

                Process[] ProcArray = Process.GetProcesses();
                foreach (Process proc in ProcArray)
                {
                    if (proc.ProcessName == "HDF5ConvertTool")
                    {
                        proc.Dispose();
                        proc.Close();

                        try
                        {
                            proc.CloseMainWindow();
                            proc.Kill();
                        }
                        catch
                        {
                        }
                    }
                }
            }

            string[] sOutputText_Split = Regex.Split(sOutputText, "\r\n");

            if (nErrorCode != 0 ||
                sOutputText_Split.Contains("Generate Finish!") == false ||
                sErrorMessage != string.Empty)
                bErrorFlag = true;

            if (bErrorFlag == true)
            {
                m_sErrorMessage = string.Format("Convert HDF5 File Error(ErrorCode:{0})", nErrorCode.ToString());
                MessageBox.Show(sErrorMessage);
                //MessageBox.Show(nErrorCode.ToString());
                return false;
            }
            else
                return true;
        }
    }
}
