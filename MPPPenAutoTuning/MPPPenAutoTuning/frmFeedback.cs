using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Threading;

namespace MPPPenAutoTuning
{
    public partial class frmFeedback : Form
    {
        private bool m_bErrorFlag = false;
        public string m_sIPAddress = "192.168.55.214";  //"192.168.56.23";
        public int m_nPort = 5000;
        public string m_sRouteName = "MPPPen_AutoTuning";

        public frmFeedback()
        {
            InitializeComponent();

            // 設定 TextBox 的 MaxLength 為 5
            tbxEmployeeNo.MaxLength = 5;

            // 啟用 RichTextBox 的拖放功能
            rtbxDescription.AllowDrop = true;

            // 處理 DragEnter 事件
            rtbxDescription.DragEnter += new DragEventHandler(rtbxDescription_DragEnter);
        }

        private void frmFeedback_Load(object sender, EventArgs e)
        {
            cbxFeedbackType.SelectedIndex = 0;

            lblState.ForeColor = Color.Blue;
            lblState.Text = "Ready";
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            InitializeState();

            ThreadPool.QueueUserWorkItem(new WaitCallback(MainProcess), null);
        }

        private void InitializeState()
        {
            m_bErrorFlag = false;

            this.Invoke((MethodInvoker)delegate
            {
                btnSelectUploadFile.Enabled = false;
                btnDeleteUploadFile.Enabled = false;
                btnSubmit.Enabled = false;

                cbxFeedbackType.Enabled = false;

                tbxEmployeeNo.Enabled = false;
                rtbxDescription.Enabled = false;
                cklbxUploadFile.Enabled = false;

                toolStripMenuItemSetting.Enabled = false;
            });

            OutputState("Executing");
        }

        private void OutputState(string sMessage)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (m_bErrorFlag == true)
                    lblState.ForeColor = Color.Red;
                else
                    lblState.ForeColor = Color.Blue;

                lblState.Text = sMessage;
            });
        }

        private void ResetController()
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnSelectUploadFile.Enabled = true;
                btnDeleteUploadFile.Enabled = true;
                btnSubmit.Enabled = true;

                cbxFeedbackType.Enabled = true;

                tbxEmployeeNo.Enabled = true;
                rtbxDescription.Enabled = true;
                cklbxUploadFile.Enabled = true;

                toolStripMenuItemSetting.Enabled = true;
            });
        }

        private void MainProcess(object objParameter)
        {
            CheckInformationValid();

            UploadAndRecordFeedback();

            OutputComplete();

            ResetController();
        }

        private void CheckInformationValid()
        {
            if (m_bErrorFlag == true)
                return;

            string sEmployeeNo = "";

            this.Invoke((MethodInvoker)delegate
            {
                sEmployeeNo = tbxEmployeeNo.Text;
            });

            bool bIsNumericFlag = sEmployeeNo.All(char.IsDigit);

            if (bIsNumericFlag == false)
            {
                m_bErrorFlag = true;
                OutputState(string.Format("Error : Employee No({0}) is Invalid", sEmployeeNo));
                return;
            }

            string sDescription = "";

            this.Invoke((MethodInvoker)delegate
            {
                sDescription = rtbxDescription.Text;
            });

            bool bIsNullFlag = string.IsNullOrWhiteSpace(sDescription);

            if (bIsNullFlag == true)
            {
                m_bErrorFlag = true;
                OutputState("Error : Description is Null");
                return;
            }

            List<string> sFilePath_List = new List<string>();

            for (int nIndex = 0; nIndex < cklbxUploadFile.Items.Count; nIndex++)
                sFilePath_List.Add(cklbxUploadFile.Items[nIndex].ToString());

            // 建立 HashSet 來存檔案名稱
            HashSet<string> sFileName_HashSet = new HashSet<string>();

            foreach (string sFilePath in sFilePath_List)
            {
                // 取得每個檔案路徑的檔案名稱
                string sFileName = Path.GetFileName(sFilePath);

                // 將檔案名稱加入 HashSet
                if (!sFileName_HashSet.Add(sFileName))
                {
                    m_bErrorFlag = true;
                    OutputState(string.Format("Error : File Name({0}) is Repeat", sFileName));
                    return;
                }
            }
        }

        private void UploadAndRecordFeedback()
        {
            if (m_bErrorFlag == true)
                return;

            string sRequest_EmployeeNo = "";
            string sRequest_FeedbackType = "";
            string sRequest_Description = "";
            string sResponse_Success = "";

            this.Invoke((MethodInvoker)delegate
            {
                sRequest_EmployeeNo = tbxEmployeeNo.Text;
                sRequest_FeedbackType = cbxFeedbackType.SelectedItem.ToString();
                sRequest_Description = rtbxDescription.Text;
            });

            List<string> sFilePath_List = new List<string>();

            for (int nIndex = 0; nIndex < cklbxUploadFile.Items.Count; nIndex++)
                sFilePath_List.Add(cklbxUploadFile.Items[nIndex].ToString());

            string sUrl = string.Format(@"http://{0}:{1}/{2}/Upload_And_Record_Feedback", m_sIPAddress, m_nPort.ToString("D"), m_sRouteName);

            HttpWebRequest cRequest = (HttpWebRequest)WebRequest.Create(sUrl);
            cRequest.Method = "POST";
            string sBoundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            cRequest.ContentType = string.Format("multipart/form-data; boundary={0}", sBoundary);
            /*
            cRequest.AllowWriteStreamBuffering = false; // 禁用緩衝區以節省記憶體

            long lTotalContentLength = 0;

            foreach (string sFilePath in sFilePath_List)
            {
                long lContentLength = new FileInfo(sFilePath).Length;
                lTotalContentLength += lContentLength;
            }

            cRequest.ContentLength = lTotalContentLength;
            //cRequest.SendChunked = true;
            */

            // 寫入請求流
            try
            {
                using (Stream cRequestStream = cRequest.GetRequestStream())
                {
                    byte[] byteBoundaryBytes_Array = Encoding.UTF8.GetBytes("--" + sBoundary + "\r\n");

                    // 準備 JSON 資料 (屬性參數)
                    var varJsonData = new
                    {
                        employee_no = sRequest_EmployeeNo,
                        feedback_type = sRequest_FeedbackType,
                        description = sRequest_Description
                    };
                    string sJson = JsonConvert.SerializeObject(varJsonData);
                    string sJsonPartHeader = "Content-Disposition: form-data; name=\"data\"\r\nContent-Type: application/json\r\n\r\n";

                    // 寫入 JSON 部分
                    cRequestStream.Write(byteBoundaryBytes_Array, 0, byteBoundaryBytes_Array.Length);
                    cRequestStream.Write(Encoding.UTF8.GetBytes(sJsonPartHeader), 0, Encoding.UTF8.GetByteCount(sJsonPartHeader));
                    cRequestStream.Write(Encoding.UTF8.GetBytes(sJson), 0, Encoding.UTF8.GetByteCount(sJson));
                    cRequestStream.Write(Encoding.UTF8.GetBytes("\r\n"), 0, Encoding.UTF8.GetByteCount("\r\n"));

                    // 添加檔案部分
                    foreach (string sFilePath in sFilePath_List)
                    {

                        cRequestStream.Write(byteBoundaryBytes_Array, 0, byteBoundaryBytes_Array.Length);

                        string sFileName = Path.GetFileName(sFilePath);

                        string sFileHeader = string.Format("Content-Disposition: form-data; name=\"files\"; filename=\"{0}\"\r\n", sFileName);

                        /*
                        string sExtension = Path.GetExtension(sFilePath);

                        if (sExtension == ".csv")
                            sFileHeader += "Content-Type: text/csv\r\n\r\n";  // 假設檔案是 CSV 格式
                        else if (sExtension == ".png")
                            sFileHeader += "Content-Type: image/png\r\n\r\n";  // 假設檔案是 PNG 格式
                        else if (sExtension == ".jpg")
                            sFileHeader += "Content-Type: image/jpg\r\n\r\n";  // 假設檔案是 JPG 格式
                        else if (sExtension == ".jpeg")
                            sFileHeader += "Content-Type: image/jpeg\r\n\r\n";  // 假設檔案是 JPEG 格式
                        */

                        sFileHeader += "Content-Type: application/octet-stream\r\n\r\n";
                        cRequestStream.Write(Encoding.UTF8.GetBytes(sFileHeader), 0, Encoding.UTF8.GetByteCount(sFileHeader));

                        using (FileStream fileStream = new FileStream(sFilePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] byteBuffer_Array = new byte[8192]; // 定義緩衝區大小
                            int nBytesRead;

                            while ((nBytesRead = fileStream.Read(byteBuffer_Array, 0, byteBuffer_Array.Length)) > 0)
                            {
                                cRequestStream.Write(byteBuffer_Array, 0, nBytesRead);
                            }
                        }

                        byte[] byteNewlineBytes_Array = Encoding.UTF8.GetBytes("\r\n");
                        cRequestStream.Write(byteNewlineBytes_Array, 0, byteNewlineBytes_Array.Length);
                    }

                    // 結束邊界
                    byte[] byteTrailer_Array = Encoding.UTF8.GetBytes("--" + sBoundary + "--\r\n");
                    cRequestStream.Write(byteTrailer_Array, 0, byteTrailer_Array.Length);
                }

            }
            catch (Exception ex)
            {
                string sError = ex.Message;

                m_bErrorFlag = true;
                OutputState(string.Format("Error : {0}", sError));
                return;
            }

            try
            {
                //response
                var varHttpResponse = (HttpWebResponse)cRequest.GetResponse();

                using (var varStreamReader = new StreamReader(varHttpResponse.GetResponseStream()))
                {
                    string sResult = varStreamReader.ReadToEnd();

                    JObject jResult = JObject.Parse(sResult);

                    sResponse_Success = (string)jResult["success"];
                }
            }
            catch (Exception ex)
            {
                string sError = ex.Message;

                m_bErrorFlag = true;
                OutputState(string.Format("Error : {0}", sError));
                return;
            }
        }

        private void OutputComplete()
        {
            if (m_bErrorFlag == false)
                OutputState("Complete");
        }

        private void btnSelectUploadFile_Click(object sender, EventArgs e)
        {
            // 創建 OpenFileDialog 的實例
            OpenFileDialog ofdUploadFile = new OpenFileDialog();

            // 設置對話框的選項
            //ofdUploadFile.InitialDirectory = "C:\\";       // 預設的文件夾路徑
            ofdUploadFile.Filter = "All files (*.*)|*.*"; ;    // 設定可選的文件類型
            ofdUploadFile.FilterIndex = 1;                   // 預設的過濾器選項
            ofdUploadFile.Multiselect = true;
            ofdUploadFile.RestoreDirectory = true;           // 關閉對話框後還原到預設路徑

            // 顯示對話框並檢查是否選擇了文件
            if (ofdUploadFile.ShowDialog() == DialogResult.OK)
            {
                // 取得文件路徑
                string[] sFilePath_Array = ofdUploadFile.FileNames;

                foreach (string sFilePath in sFilePath_Array)
                    cklbxUploadFile.Items.Add(sFilePath);
            }
        }

        private void btnDeleteUploadFile_Click(object sender, EventArgs e)
        {
            for (int nIndex = cklbxUploadFile.Items.Count - 1; nIndex >= 0; nIndex--)
            {
                if (cklbxUploadFile.GetItemChecked(nIndex) == true)
                    cklbxUploadFile.Items.RemoveAt(nIndex);
            }
        }

        private void toolStripMenuItemParameter_Click(object sender, EventArgs e)
        {
            frmFeedbackParameterSetting cfrmFeedbackParameterSetting = new frmFeedbackParameterSetting(this);
            //cfrmFeedbackParameterSetting.TopMost = true;

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmFeedbackParameterSetting.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmFeedbackParameterSetting.Height / 2);

            if (this.IsMdiChild == true)
            {
                nLocationX = (int)((this.MdiParent.Left + this.MdiParent.Right) / 2) - (int)(cfrmFeedbackParameterSetting.Width / 2);
                nLocationY = (int)((this.MdiParent.Top + this.MdiParent.Bottom) / 2) - (int)(cfrmFeedbackParameterSetting.Height / 2);
            }

            cfrmFeedbackParameterSetting.StartPosition = FormStartPosition.Manual;
            cfrmFeedbackParameterSetting.Location = new Point(nLocationX, nLocationY);

            if (cfrmFeedbackParameterSetting.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void rtbxDescription_KeyDown(object sender, KeyEventArgs e)
        {
            // 如果用戶按下 Ctrl+V 貼上操作
            if (e.Control && e.KeyCode == Keys.V)
            {
                // 判斷剪貼簿中的資料格式，如果不是文字就阻止操作
                if (Clipboard.ContainsImage())
                {
                    // 阻止貼上圖片的操作
                    e.Handled = true;
                }
            }
        }

        private void rtbxDescription_DragEnter(object sender, DragEventArgs e)
        {
            // 檢查拖放的內容是否為文字
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;  // 允許文字拖放
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // 如果拖入的是檔案（例如圖片），阻止拖放
                e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;  // 阻止非文字內容的拖放
            }
        }

        private void tbxEmployeeNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 檢查是否為數字或控制鍵（如 Backspace）
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // 阻止非數字的輸入
            }
        }
    }
}
