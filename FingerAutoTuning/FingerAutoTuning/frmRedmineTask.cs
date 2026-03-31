using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    public partial class frmRedmineTask : Form
    {
        /*
        // Windows API 常數
        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;
        private const uint VK_SNAPSHOT = 0x2C;

        // 事件委派定義
        public delegate void PrintScreenEventHandler(object sender, EventArgs e);
        public event PrintScreenEventHandler OnPrintScreen;
        public event PrintScreenEventHandler OnAltPrintScreen;
        public event PrintScreenEventHandler OnCtrlPrintScreen;

        // Windows API 函數宣告
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Windows API 結構和函數
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        */

        private frmMain m_cfrmMain = null;

        private string m_sUserName = "";   // UserName
        private string m_sPassword = "";   // Passwrod

        private string m_sAPIKey = ""; // API Key Token

        private int m_nAssignToID = 827;

        private string m_sSubject = ""; //"Redmine Create Issue Test(Redmine新增Issue測試)";
        private string m_sProjectName = "";
        private string m_sToolVersion = "";
        private string[] m_sDescriptionTest_Array = new string[] {};    //{ "This is Redmine Create Issue Test", "這是Redmine新增Issue測試" };
        private string m_sDescription = "";
        private string m_sSocketType = MainConstantParameter.m_sSOCKET_WINDOWS;
        private string m_sInterface = MainConstantParameter.m_sINTERFACE_HIDOVERI2C;
        private string m_sICSolution = "";

        private string m_sRealityDescription = "";

        private bool m_bProcessCompleteFlag = true;
        private const int m_nDisplayExecuteInternalTime =  10;

        private int m_nLoginType = 0;   // 0:Use UserName & Password  1:Use API Key

        private string m_sScreenShotFilePath = "";
        private string m_sScreenShotFileName = "";
        private bool m_bGetScreenShotFlag = false;

        // 儲存路徑設定
        private string m_sScreebShotFolderPath = string.Format(@"{0}\Screenshot", Application.StartupPath);   //Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        private CheckBox m_ckbxItem = new CheckBox();

        private string m_sSelectDirectoryPath = "";

        public class FileInforamtion
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public string Description { get; set; }
        }

        private Dictionary<string, int> m_dictAssignTo = new Dictionary<string, int>()
        {
            { "Jeffery Yang(楊志聖)", 827 },
            { "None", -1 }
        };

        private Dictionary<string, string> m_dictSocketType = new Dictionary<string, string>()
        {
            { MainConstantParameter.m_sSOCKET_WINDOWS,                  MainConstantParameter.m_sSOCKET_WINDOWS },
            { MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT,     MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT },
            { MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL,          MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL },
            { MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER,   MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER },
            { MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT,      MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT },
            { MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER,    MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER }
        };

        private Dictionary<string, string> m_dictInterface = new Dictionary<string, string>()
        {
            { MainConstantParameter.m_sINTERFACE_HIDOVERI2C,            MainConstantParameter.m_sINTERFACE_HIDOVERI2C },
            { MainConstantParameter.m_sINTERFACE_I2C,                   MainConstantParameter.m_sINTERFACE_I2C },
            { MainConstantParameter.m_sINTERFACE_USB,                   MainConstantParameter.m_sINTERFACE_USB },
            { MainConstantParameter.m_sINTERFACE_SPI_MA_RISING_HALF,    MainConstantParameter.m_sINTERFACE_SPI_MA_RISING_HALF },
            { MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING_HALF,   MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING_HALF },
            { MainConstantParameter.m_sINTERFACE_SPI_MA_RISING,         MainConstantParameter.m_sINTERFACE_SPI_MA_RISING },
            { MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING,        MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING }
        };

        private const string m_nSIZETYPE_B = "B";
        private const string m_nSIZETYPE_KB = "KB";
        private const string m_nSIZETYPE_MB = "MB";
        private const string m_nSIZETYPE_GB = "GB";
        private const string m_nSIZETYPE_TB = "TB";

        public frmRedmineTask(frmMain cfrmMain)
        {
            InitializeComponent();

            m_sScreenShotFilePath = "";
            m_sScreenShotFileName = "";
            m_bGetScreenShotFlag = false;

            /*
            RegisterPrintScreenKeys();

            // 設定事件處理器
            OnPrintScreen += (sender, e) =>
            {
                OutputMessage("Push Print Screen");
            };

            OnAltPrintScreen += (sender, e) =>
            {
                OutputMessage("Push Alt + Print Screen");
            };

            OnCtrlPrintScreen += (sender, e) =>
            {
                OutputMessage("Push Ctrl + Print Screen");
            };
            */

            m_cfrmMain = cfrmMain;

            lblStatus.Text = "Ready";
            lblStatus.ForeColor = Color.Blue;

            cbxLoginType.SelectedIndex = m_nLoginType;

            tbxUserName.Text = m_sUserName;
            tbxPassword.Text = m_sPassword;

            tbxAPIKey.Text = m_sAPIKey;

            SetLoginType();

            SetTextBoxPasswordChar();

            cbxAssignTo.Items.Clear();

            foreach (KeyValuePair<string, int> sAssignTo in m_dictAssignTo)
            {
                cbxAssignTo.Items.Add(sAssignTo.Key);
            }

            cbxAssignTo.SelectedIndex = 0;

            tbxSubject.Text = m_sSubject;

            GetConnectInfoData();

            tbxProjectName.Text = m_sProjectName;
            tbxToolVersion.Text = m_cfrmMain.m_sAPTotalVersion;

            int nSocketTypeSelectedIndex = 0;
            int nSocketTypeIndex = 0;

            foreach (KeyValuePair<string, string> sSocketType in m_dictSocketType)
            {
                if (sSocketType.Key == m_sSocketType)
                    nSocketTypeSelectedIndex = nSocketTypeIndex;

                cbxSocketType.Items.Add(sSocketType.Key);

                nSocketTypeIndex++;
            }

            cbxSocketType.SelectedIndex = nSocketTypeSelectedIndex;

            int nInterfaceSelectedIndex = 0;
            int nInterfaceIndex = 0;

            foreach (KeyValuePair<string, string> sInterface in m_dictInterface)
            {
                if (sInterface.Key == m_sInterface)
                    nInterfaceSelectedIndex = nInterfaceIndex;

                cbxInterface.Items.Add(sInterface.Key);

                nInterfaceIndex++;
            }

            cbxInterface.SelectedIndex = nInterfaceSelectedIndex;

            tbxICSolution.Text = m_sICSolution;
            
            // 可以在點擊連結時處理事件
            rtbxMessage.LinkClicked += new LinkClickedEventHandler(rtbxMessage_LinkClicked);

            rtbxDescription.HandleCreated += rtbxDescription_HandleCreated;
            dgvUploadFile.Rows.Clear();
            dgvUploadFile.HandleCreated += dgvUploadFile_HandleCreated;

            SetupDescriptionContextMenu();

            m_bProcessCompleteFlag = true;
        }

        private void GetConnectInfoData()
        {
            if (File.Exists(m_cfrmMain.m_sConnectInfoFilePath) == true)
            {
                string sProjectName = ReadValue("Connect Information", "ProjectName", m_cfrmMain.m_sConnectInfoFilePath, "");

                if (sProjectName.Trim() != "")
                    m_sProjectName = sProjectName;
                else
                    m_sProjectName = ParamFingerAutoTuning.m_sProjectName;

                string sSocketType = ReadValue("Connect Information", "SocketType", m_cfrmMain.m_sConnectInfoFilePath, "");

                if (sSocketType.Trim() != "")
                    m_sSocketType = sSocketType;
                else
                    m_sSocketType = ParamFingerAutoTuning.m_sSocketType;

                string sInterface = IniFileFormat.ReadValue("Connect Information", "Interface", m_cfrmMain.m_sConnectInfoFilePath, "");

                if (sInterface.Trim() != "")
                    m_sInterface = sInterface;
                else
                    m_sInterface = ParamFingerAutoTuning.m_sInterfaceType;

                string sICSolution = ReadValue("Connect Information", "ICSolution", m_cfrmMain.m_sConnectInfoFilePath, "");

                if (sICSolution.Trim() != "")
                    m_sICSolution = sICSolution;
                else
                    m_sICSolution = m_cfrmMain.m_sICSolutionName;
            }
            else
            {
                m_sProjectName = ParamFingerAutoTuning.m_sProjectName;
                m_sSocketType = ParamFingerAutoTuning.m_sSocketType;
                m_sInterface = ParamFingerAutoTuning.m_sInterfaceType;
                m_sICSolution = m_cfrmMain.m_sICSolutionName;
            }
        }

        private string ReadValue(string Section, string Key, string m_sPath, string Default = "")
        {
            return IniReadValue(Section, Key, m_sPath, Default);
        }

        private string IniReadValue(string Section, string Key, string m_sPath, string Default = "")
        {
            StringBuilder temp = new StringBuilder(255);
            int i = Win32.GetPrivateProfileString(Section, Key, Default, temp, 255, m_sPath);

            if (temp != null)
                return temp.ToString();

            return Default;
        }

        private void SetupDescriptionContextMenu()
        {
            ContextMenuStrip cContextMenu = new ContextMenuStrip();

            // 剪下
            ToolStripMenuItem cCutItem = new ToolStripMenuItem("Cut");
            cCutItem.Click += (sender, e) => rtbxDescription.Cut();

            // 複製
            ToolStripMenuItem cCopyItem = new ToolStripMenuItem("Copy");
            cCopyItem.Click += (sender, e) => rtbxDescription.Copy();

            // 貼上
            ToolStripMenuItem cPasteItem = new ToolStripMenuItem("Paste");
            cPasteItem.Click += (sender, e) => rtbxDescription.Paste();

            // 添加所有項目
            cContextMenu.Items.AddRange(new ToolStripItem[]
            {
                cCutItem,
                cCopyItem,
                cPasteItem
            });

            rtbxDescription.ContextMenuStrip = cContextMenu;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            InitializeState();

            if (CheckSettingData() == false)
                return;

            ThreadPool.QueueUserWorkItem(new WaitCallback(MainProcess), null);

            ThreadPool.QueueUserWorkItem(new WaitCallback(DisplayExecuteStatusLabel), null);
        }

        private void InitializeState()
        {
            m_bProcessCompleteFlag = false;

            rtbxMessage.Clear();

            SetEnableCotroller(false);

            if (m_nLoginType == 0)
            {
                m_sUserName = tbxUserName.Text;
                m_sPassword = tbxPassword.Text;
            }
            else if (m_nLoginType == 1)
            {
                m_sAPIKey = tbxAPIKey.Text;
            }

            m_nAssignToID = (int)m_dictAssignTo[cbxAssignTo.SelectedItem.ToString()];

            m_sSubject = string.Format("[Finger]{0}", tbxSubject.Text);   //string.Format("[Finger]{0}_{1}", tbxSubject.Text, DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            m_sProjectName = tbxProjectName.Text;
            m_sToolVersion = tbxToolVersion.Text;

            m_sSocketType = m_dictSocketType[cbxSocketType.SelectedItem.ToString()];
            m_sInterface = m_dictInterface[cbxInterface.SelectedItem.ToString()];
            m_sICSolution = tbxICSolution.Text;

            m_sDescription = rtbxDescription.Text;

            m_sRealityDescription = string.Format("Project(SKU) Name : {0}", m_sProjectName) + Environment.NewLine;
            m_sRealityDescription += string.Format("Tool Version : {0}", m_sToolVersion) + Environment.NewLine;
            m_sRealityDescription += string.Format("Socket Type : {0}", m_sSocketType) + Environment.NewLine;
            m_sRealityDescription += string.Format("Interface : {0}", m_sInterface) + Environment.NewLine;
            m_sRealityDescription += string.Format("IC Solution : {0}", m_sICSolution) + Environment.NewLine;
            m_sRealityDescription += Environment.NewLine;
            m_sRealityDescription += "Description : " + Environment.NewLine;
            m_sRealityDescription += m_sDescription;
        }

        private bool CheckSettingData()
        {
            if (m_nLoginType == 0)
            {
                if (string.IsNullOrWhiteSpace(m_sUserName) == true)
                {
                    OutputMessage("No UserName. Please Fill in the UserName", Color.Red);
                    OutputStatus("Error", "No UserName", Color.Red);
                    lblUserName.ForeColor = Color.Red;
                    SetEnableCotroller(true);
                    return false;
                }
                else
                    lblUserName.ForeColor = SystemColors.ControlText;

                if (string.IsNullOrWhiteSpace(m_sPassword) == true)
                {
                    OutputMessage("No Password. Please Fill in the Password", Color.Red);
                    OutputStatus("Error", "No Password", Color.Red);
                    lblPassword.ForeColor = Color.Red;
                    SetEnableCotroller(true);
                    return false;
                }
                else
                    lblPassword.ForeColor = SystemColors.ControlText;
            }
            else if (m_nLoginType == 1)
            {
                if (string.IsNullOrWhiteSpace(m_sAPIKey) == true)
                {
                    OutputMessage("No API Key. Please Fill in the API Key", Color.Red);
                    OutputStatus("Error", "No API Key", Color.Red);
                    lblAPIKey.ForeColor = Color.Red;
                    SetEnableCotroller(true);
                    return false;
                }
                else
                    lblAPIKey.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrWhiteSpace(m_sSubject) == true)
            {
                OutputMessage("No Subject. Please Fill in the Subject", Color.Red);
                OutputStatus("Error", "No Subject", Color.Red);
                lblSubject.ForeColor = Color.Red;
                SetEnableCotroller(true);
                return false;
            }
            else
                lblSubject.ForeColor = SystemColors.ControlText;

            if (string.IsNullOrWhiteSpace(m_sProjectName) == true)
            {
                OutputMessage("No Project(SKU) Name. Please Fill in the Project(SKU) Name", Color.Red);
                OutputStatus("Error", "No Project(SKU) Name", Color.Red);
                lblProjectName.ForeColor = Color.Red;
                SetEnableCotroller(true);
                return false;
            }
            else
                lblProjectName.ForeColor = SystemColors.ControlText;

            if (string.IsNullOrWhiteSpace(m_sToolVersion) == true)
            {
                OutputMessage("No Tool Version. Please Fill in the Tool Version", Color.Red);
                OutputStatus("Error", "No Tool Version", Color.Red);
                lblToolVersion.ForeColor = Color.Red;
                SetEnableCotroller(true);
                return false;
            }
            else
                lblToolVersion.ForeColor = SystemColors.ControlText;

            if (string.IsNullOrWhiteSpace(m_sICSolution) == true)
            {
                OutputMessage("No IC Solution. Please Fill in the IC Solution", Color.Red);
                OutputStatus("Error", "No IC Solution", Color.Red);
                lblICSolution.ForeColor = Color.Red;
                SetEnableCotroller(true);
                return false;
            }
            else
                lblToolVersion.ForeColor = SystemColors.ControlText;

            if (string.IsNullOrWhiteSpace(m_sDescription) == true)
            {
                OutputMessage("No Description. Please Fill in the Description", Color.Red);
                OutputStatus("Error", "No Description", Color.Red);
                lblDescription.ForeColor = Color.Red;
                SetEnableCotroller(true);
                return false;
            }
            else
                lblDescription.ForeColor = SystemColors.ControlText;

            List<string> sUploadFilePath_List = new List<string>();

            for (int nIndex = 0; nIndex < dgvUploadFile.Rows.Count; nIndex++)
                sUploadFilePath_List.Add(dgvUploadFile.Rows[nIndex].Cells[1].Value.ToString());

            // 建立 HashSet 來存檔案名稱
            HashSet<string> sFileName_HashSet = new HashSet<string>();

            foreach (string sFilePath in sUploadFilePath_List)
            {
                if (File.Exists(sFilePath) == false)
                {
                    OutputMessage(string.Format("File Path({0}) is Not Exist", sFilePath), Color.Red);
                    OutputStatus("Error", "File Path is Not Exist", Color.Red);
                    SetEnableCotroller(true);
                    return false;
                }

                // 取得每個檔案路徑的檔案名稱
                string sFileName = Path.GetFileName(sFilePath);

                // 將檔案名稱加入 HashSet
                if (!sFileName_HashSet.Add(sFileName))
                {
                    OutputMessage(string.Format("File Name({0}) is Repeat", sFileName), Color.Red);
                    OutputStatus("Error", "File Name is Repeat", Color.Red);
                    SetEnableCotroller(true);
                    return false;
                }

                FileInfo cFileInfo = new FileInfo(sFilePath);
                decimal structSizeNumber = 0;
                string sSuffix = m_nSIZETYPE_B;
                GetFormatFileSize(ref structSizeNumber, ref sSuffix, cFileInfo.Length);
                double dSizeNumber = (double)structSizeNumber;

                if (dSizeNumber >= 50.0 && (sSuffix == m_nSIZETYPE_MB || sSuffix == m_nSIZETYPE_GB || sSuffix == m_nSIZETYPE_TB))
                {
                    OutputMessage(string.Format("File Name({0}) Size >= 50MB", sFileName), Color.Red);
                    OutputStatus("Error", "File Size >= 50MB", Color.Red);
                    SetEnableCotroller(true);
                    return false;
                }
            }

            return true;
        }

        private void MainProcess(object objParameter)
        {
            RedmineProcess cRedmineProcess = new RedmineProcess(this);

            if (m_nLoginType == 0)
            {
                cRedmineProcess.SetLoginType(m_nLoginType);
                cRedmineProcess.SetUserNameAndPassword(m_sUserName, m_sPassword);
            }
            else if (m_nLoginType == 1)
            {
                cRedmineProcess.SetLoginType(m_nLoginType);
                cRedmineProcess.SetAPIKey(m_sAPIKey);
            }

            cRedmineProcess.ResetErrorFlag();

            List<FileInforamtion> cUploadFileInformation_List = new List<FileInforamtion>();

            for (int nIndex = 0; nIndex < dgvUploadFile.Rows.Count; nIndex++)
            {
                FileInforamtion cFileInforamtion = new FileInforamtion();
                cFileInforamtion.FileName = Path.GetFileName(dgvUploadFile.Rows[nIndex].Cells[1].Value.ToString());
                cFileInforamtion.FilePath = dgvUploadFile.Rows[nIndex].Cells[1].Value.ToString();
                cFileInforamtion.Description = dgvUploadFile.Rows[nIndex].Cells[2].Value.ToString();
                cUploadFileInformation_List.Add(cFileInforamtion);
            }

            cRedmineProcess.SetUploadFileInformationList(cUploadFileInformation_List);

            //cRedmineProcess.ConnectRedmineByRedmineNetApi();
            //cRedmineProcess.GetIssueJournalsByRedmineNetApi();
            //cRedmineProcess.CreateRedmineIssueByRedmineNetApi(m_sSubject, m_sRealityDescription);

            //cRedmineProcess.ConnectRedmineByNet();
            //cRedmineProcess.GetIssueJournalsByNet();
            //cRedmineProcess.CreateRedmineIssueByNet(m_sSubject, m_sRealityDescription);

            cRedmineProcess.CreateIssueBySendPOSTAPI(m_sSubject, m_sRealityDescription, m_nAssignToID);

            m_bProcessCompleteFlag = true;
            Thread.Sleep(10);
            
            if (cRedmineProcess.m_bErrorFlag == false)
                OutputStatus("Complete", "", Color.Green);
            else
                OutputStatus("Error", "Create Redmine Issue Error", Color.Red);

            SetEnableCotroller(true);
        }

        private void DisplayExecuteStatusLabel(object objParameter)
        {
            int nPartCount = 0;
            int nSecondPart = 1000 / m_nDisplayExecuteInternalTime;

            while (m_bProcessCompleteFlag == false)
            {
                if (nPartCount == 0)
                    OutputStatus("Execute.", "", Color.Blue);
                else if (nPartCount == nSecondPart * 1)
                    OutputStatus("Execute..", "", Color.Blue);
                else if (nPartCount == nSecondPart * 2)
                    OutputStatus("Execute...", "", Color.Blue);

                Thread.Sleep(10);
                nPartCount++;

                if (nPartCount >= nSecondPart * 3)
                    nPartCount = 0;
            }
        }

        private void btnSelectUploadFile_Click(object sender, EventArgs e)
        {
            // 創建 OpenFileDialog 的實例
            OpenFileDialog ofdUploadFile = new OpenFileDialog();

            // 設置對話框的選項
            if (Directory.Exists(m_sSelectDirectoryPath) == false)
            {
                if (Directory.Exists(m_cfrmMain.m_sLogDirectoryPath) == false)
                    ofdUploadFile.InitialDirectory = string.Format(@"{0}\{1}", Application.StartupPath, frmMain.m_sAPMainDirectoryName);       // 預設的文件夾路徑
                else
                    ofdUploadFile.InitialDirectory = m_cfrmMain.m_sLogDirectoryPath;
            }
            else
                ofdUploadFile.InitialDirectory = m_sSelectDirectoryPath;

            ofdUploadFile.Filter = "All files (*.*)|*.*";   //"Acceptable Files (*.txt;*.csv;*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.png;*.jpg;*.jpeg;*.gif;*.zip;*.7z;*.rar;*.json;*.ppt;*.pptx)|*.txt;*.csv;*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.png;*.jpg;*.jpeg;*.gif;*.zip;*.7z;*.rar;*.json;*.ppt;*.pptx|txt (*.txt)|*.txt|csv (*.csv)|*.csv|pdf (*.pdf)|*.pdf|doc (*.doc)|*.doc|*.pdf|docx (*.docx)|*.docx|xls (*.xls)|*.xls|xlsx (*.xlsx)|*.xlsx|png (*.png)|*.png|jpg (*.jpg)|*.jpg|jpeg (*.jpeg)|*.jpeg|gif (*.gif)|*.gif|zip (*.zip)|*.zip|7z (*.7z)|*.7z|*.zip|rar (*.rar)|*.rar|json (*.json)|*.json|ppt (*.ppt)|*.ppt|pptx (*.pptx)|*.pptx";    //"All files (*.*)|*.*";    // 設定可選的文件類型
            ofdUploadFile.FilterIndex = 1;                   // 預設的過濾器選項
            ofdUploadFile.Multiselect = true;
            ofdUploadFile.RestoreDirectory = true;           // 關閉對話框後還原到預設路徑

            // 顯示對話框並檢查是否選擇了文件
            if (ofdUploadFile.ShowDialog() == DialogResult.OK)
            {
                // 取得文件路徑
                string[] sFilePath_Array = ofdUploadFile.FileNames;
                m_sSelectDirectoryPath = Path.GetDirectoryName(ofdUploadFile.FileName);

                foreach (string sFilePath in sFilePath_Array)
                {
                    bool bRepeatFlag = false;

                    foreach (DataGridViewRow dgvrRow in dgvUploadFile.Rows)
                    {
                        string sExistFilePath = dgvrRow.Cells[1].Value.ToString();

                        if (sFilePath == sExistFilePath)
                        {
                            bRepeatFlag = true;
                            break;
                        }
                    }

                    if (bRepeatFlag == false)
                    {
                        // 取得每個檔案路徑的檔案名稱
                        string sFileName = Path.GetFileName(sFilePath);

                        FileInfo cFileInfo = new FileInfo(sFilePath);
                        decimal structSizeNumber = 0;
                        string sSuffix = m_nSIZETYPE_B;
                        GetFormatFileSize(ref structSizeNumber, ref sSuffix, cFileInfo.Length);
                        double dSizeNumber = (double)structSizeNumber;

                        if (dSizeNumber >= 50.0 && (sSuffix == m_nSIZETYPE_MB || sSuffix == m_nSIZETYPE_GB || sSuffix == m_nSIZETYPE_TB))
                        {
                            MessageBox.Show(string.Format("File Name({0}) Size >= 50MB", sFileName));
                            break;
                        }

                        int nDataCount = dgvUploadFile.Rows.Count;

                        DataGridViewRow dgvrRow = new DataGridViewRow();
                        dgvrRow.CreateCells(dgvUploadFile);
                        dgvrRow.HeaderCell.Value = (nDataCount + 1).ToString();
                        dgvrRow.Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        dgvrRow.Cells[1].Value = sFilePath;
                        dgvrRow.Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        dgvrRow.Cells[2].Value = Path.GetFileName(sFilePath);
                        dgvUploadFile.Rows.Add(dgvrRow);
                    }
                }
            }
        }

        private string GetFormatFileSize(ref decimal structSizeNumber, ref string sSuffix, long lByte)
        {
            string[] sSuffix_Array = { m_nSIZETYPE_B, m_nSIZETYPE_KB, m_nSIZETYPE_MB, m_nSIZETYPE_GB, m_nSIZETYPE_TB };
            int nCounter = 0;
            decimal structNumber = lByte;

            while (Math.Round(structNumber / 1024) >= 1)
            {
                structNumber = structNumber / 1024;
                nCounter++;
            }

            structSizeNumber = structNumber;
            sSuffix = sSuffix_Array[nCounter];

            return string.Format("{0:n2} {1}", structNumber, sSuffix_Array[nCounter]);
        }


        private void btnDeleteUploadFile_Click(object sender, EventArgs e)
        {
            int nUploadFileCount = dgvUploadFile.Rows.Count;

            for (int nRowIndex = nUploadFileCount - 1; nRowIndex >= 0; nRowIndex--)
            {
                if (dgvUploadFile.Rows[nRowIndex].Cells[0].Value != null && (bool)dgvUploadFile.Rows[nRowIndex].Cells[0].Value)
                    dgvUploadFile.Rows.Remove(dgvUploadFile.Rows[nRowIndex]);
            }

            int nDataIndex = 0;

            foreach (DataGridViewRow dgvrRow in dgvUploadFile.Rows)
            {
                dgvrRow.HeaderCell.Value = (nDataIndex + 1).ToString();
                nDataIndex++;
            }

            ((CheckBox)dgvUploadFile.Controls.Find("Item", true)[0]).Checked = false;
        }

        public void SetEnableCotroller(bool bEnableFlag)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                cbxLoginType.Enabled = bEnableFlag;

                tbxUserName.Enabled = bEnableFlag;
                tbxPassword.Enabled = bEnableFlag;

                tbxAPIKey.Enabled = bEnableFlag;

                ckbxHide.Enabled = bEnableFlag;

                cbxAssignTo.Enabled = bEnableFlag;

                tbxSubject.Enabled = bEnableFlag;

                tbxProjectName.Enabled = bEnableFlag;
                tbxToolVersion.Enabled = bEnableFlag;

                cbxSocketType.Enabled = bEnableFlag;
                cbxInterface.Enabled = bEnableFlag;
                tbxICSolution.Enabled = bEnableFlag;

                rtbxDescription.Enabled = bEnableFlag;

                btnSelectUploadFile.Enabled = bEnableFlag;
                btnDeleteUploadFile.Enabled = bEnableFlag;
                dgvUploadFile.Enabled = bEnableFlag;

                btnSubmit.Enabled = bEnableFlag;
            });
        }

        public void OutputMessage(string sMessage)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                // 移動到文字尾端
                rtbxMessage.SelectionStart = rtbxMessage.TextLength;
                rtbxMessage.SelectionLength = 0;

                // 設定顏色並加入文字
                rtbxMessage.SelectionColor = Color.Black;
                rtbxMessage.ForeColor = Color.Black;
                rtbxMessage.Font = new Font("Times New Roman", (float)9.75);
                rtbxMessage.AppendText(sMessage + Environment.NewLine);
            });
        }

        public void OutputMessage(string sMessage, Color clrSectionColor)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                // 移動到文字尾端
                rtbxMessage.SelectionStart = rtbxMessage.TextLength;
                rtbxMessage.SelectionLength = 0;

                // 設定顏色並加入文字
                rtbxMessage.SelectionColor = clrSectionColor;
                rtbxMessage.ForeColor = clrSectionColor;
                rtbxMessage.Font = new Font("Times New Roman", (float)9.75);
                rtbxMessage.AppendText(sMessage + Environment.NewLine);
            });
        }

        public void OutputDescription(string sMessage)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                // 移動到文字尾端
                rtbxDescription.SelectionStart = rtbxDescription.TextLength;
                rtbxDescription.SelectionLength = 0;

                // 設定顏色並加入文字
                rtbxDescription.SelectionColor = Color.Black;
                rtbxDescription.Font = new Font("Times New Roman", (float)9.75);
                rtbxDescription.AppendText(sMessage + Environment.NewLine);
            });
        }

        public void OutputStatus(string sStatus, string sStatusMessage, Color clrForeColor)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                lblStatus.ForeColor = clrForeColor;
                lblStatus.Text = sStatus;
                lblStatusMessage.ForeColor = clrForeColor;
                lblStatusMessage.Text = sStatusMessage;
            });
        }

        private void rtbxMessage_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            // 開啟預設瀏覽器並導向連結
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void ckbxHide_CheckedChanged(object sender, EventArgs e)
        {
            SetTextBoxPasswordChar();
        }

        private void SetLoginType()
        {
            if (m_nLoginType == 0)
            {
                lblUserName.Visible = true;
                tbxUserName.Visible = true;
                lblPassword.Visible = true;
                tbxPassword.Visible = true;

                lblAPIKey.Visible = false;
                tbxAPIKey.Visible = false;
            }
            else if (m_nLoginType == 1)
            {
                lblUserName.Visible = false;
                tbxUserName.Visible = false;
                lblPassword.Visible = false;
                tbxPassword.Visible = false;

                lblAPIKey.Visible = true;
                tbxAPIKey.Visible = true;
            }
        }

        private void SetTextBoxPasswordChar()
        {
            if (ckbxHide.Checked == true)
            {
                tbxPassword.PasswordChar = '*';
                tbxAPIKey.PasswordChar = '*';
            }
            else
            {
                tbxPassword.PasswordChar = '\0';
                tbxAPIKey.PasswordChar = '\0';
            }
        }

        private void cbxLoginType_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_nLoginType = cbxLoginType.SelectedIndex;

            SetLoginType();
        }

        private void frmRedmineTask_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        /*
        private void RegisterPrintScreenKeys()
        {
            try
            {
                // 註冊單獨的 Print Screen
                RegisterHotKey(this.Handle, 1, 0, VK_SNAPSHOT);

                // 註冊 Alt + Print Screen
                RegisterHotKey(this.Handle, 2, MOD_ALT, VK_SNAPSHOT);

                // 註冊 Ctrl + Print Screen
                RegisterHotKey(this.Handle, 3, MOD_CONTROL, VK_SNAPSHOT);

                // 註冊 Ctrl + Shift + Alt + Print Screen
                RegisterHotKey(this.Handle, 2, MOD_CONTROL | MOD_SHIFT | MOD_ALT, VK_SNAPSHOT);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Regist Hot Key Fail : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void WndProc(ref Message m)
        {
            // 處理熱鍵訊息
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();

                switch (id)
                {
                    case 1: // Print Screen
                        CaptureScreen();
                        OnPrintScreen.Invoke(this, EventArgs.Empty);
                        break;

                    case 2: // Alt + Print Screen & Ctrl + Shift + Alt + Print Screen
                        CaptureActiveWindow();
                        OnAltPrintScreen.Invoke(this, EventArgs.Empty);
                        break;

                    case 3: // Ctrl + Print Screen
                        CaptureScreenToClipboard();
                        OnCtrlPrintScreen.Invoke(this, EventArgs.Empty);
                        break;
                }
            }

            base.WndProc(ref m);
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 取消註冊所有熱鍵
            UnregisterHotKey(this.Handle, 1);
            UnregisterHotKey(this.Handle, 2);
            UnregisterHotKey(this.Handle, 3);
            base.OnFormClosing(e);
        }

        private void CaptureScreen()
        {
            try
            {
                // 取得所有螢幕的總範圍
                Rectangle structBounds = Screen.PrimaryScreen.Bounds;

                // 建立新的 Bitmap
                using (Bitmap cBitmap = new Bitmap(structBounds.Width, structBounds.Height))
                {
                    // 建立 Graphics 物件並複製螢幕內容
                    using (Graphics g = Graphics.FromImage(cBitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, structBounds.Size);
                    }

                    // 儲存圖片
                    //string sFileName = string.Format("screenshot_{0}.png", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                    //string sFilePath = Path.Combine(m_sScreebShotFolderPath, sFileName);
                    //cBitmap.Save(sFilePath, ImageFormat.Png);

                    // 複製到剪貼簿
                    Clipboard.SetImage(cBitmap);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Print Screen Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CaptureActiveWindow()
        {
            try
            {
                // 取得作用中視窗的大小和位置
                IntPtr structHandle = GetForegroundWindow();

                if (structHandle == IntPtr.Zero)
                    return;

                RECT structRect;
                GetWindowRect(structHandle, out structRect);
                int nWidth = structRect.Right - structRect.Left;
                int nHeight = structRect.Bottom - structRect.Top;

                // 建立新的 Bitmap
                using (Bitmap cBitmap = new Bitmap(nWidth, nHeight))
                {
                    using (Graphics g = Graphics.FromImage(cBitmap))
                    {
                        g.CopyFromScreen(new Point(structRect.Left, structRect.Top), Point.Empty, new Size(nWidth, nHeight));
                    }

                    // 儲存圖片
                    //string sFileName = string.Format("screenshot_{0}.png", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                    //string sFilePath = Path.Combine(m_sScreebShotFolderPath, sFileName);
                    //cBitmap.Save(sFilePath, ImageFormat.Png);

                    // 複製到剪貼簿
                    Clipboard.SetImage(cBitmap);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Print Screen Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CaptureScreenToClipboard()
        {
            try
            {
                // 取得螢幕範圍
                Rectangle structBounds = Screen.PrimaryScreen.Bounds;

                // 建立新的 Bitmap
                using (Bitmap cBitmap = new Bitmap(structBounds.Width, structBounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(cBitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, structBounds.Size);
                    }

                    // 只複製到剪貼簿，不儲存檔案
                    Clipboard.SetImage(cBitmap);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Copy from Clipboard Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */

        private void rtbxDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                e.Handled = true; // 阻止預設貼上行為

                // 檢查剪貼簿是否包含文字
                if (Clipboard.ContainsText())
                {
                    rtbxDescription.SelectedText = Clipboard.GetText();
                }
                else if (Clipboard.ContainsImage())
                {
                    m_sScreenShotFilePath = "";
                    m_sScreenShotFileName = "";
                    m_bGetScreenShotFlag = false;

                    try
                    {
                        using (Image cImage = Clipboard.GetImage())
                        {
                            if (cImage != null)
                            {
                                if (Directory.Exists(m_sScreebShotFolderPath) == false)
                                    Directory.CreateDirectory(m_sScreebShotFolderPath);

                                string sFileName = string.Format("screenshot_{0}.png", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                                string sFilePath = Path.Combine(m_sScreebShotFolderPath, sFileName);
                                cImage.Save(sFilePath, ImageFormat.Png);
                                m_sScreenShotFilePath = sFilePath;
                                m_sScreenShotFileName = sFileName;
                                m_bGetScreenShotFlag = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Save Print Screen Image from Cipboard Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (m_bGetScreenShotFlag == true && File.Exists(m_sScreenShotFilePath) == true)
                    {
                        rtbxDescription.SelectedText = string.Format("!{0}!", m_sScreenShotFileName);

                        bool bRepeatFlag = false;

                        foreach (DataGridViewRow dgvrRow in dgvUploadFile.Rows)
                        {
                            string sExistFilePath = dgvrRow.Cells[1].Value.ToString();

                            if (m_sScreenShotFilePath == sExistFilePath)
                            {
                                bRepeatFlag = true;
                                break;
                            }
                        }

                        if (bRepeatFlag == false)
                        {
                            int nDataCount = dgvUploadFile.Rows.Count;

                            DataGridViewRow dgvrRow = new DataGridViewRow();
                            dgvrRow.CreateCells(dgvUploadFile);
                            dgvrRow.HeaderCell.Value = (nDataCount + 1).ToString();
                            dgvrRow.Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            dgvrRow.Cells[1].Value = m_sScreenShotFilePath;
                            dgvrRow.Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            dgvrRow.Cells[2].Value = Path.GetFileName(m_sScreenShotFilePath);
                            dgvUploadFile.Rows.Add(dgvrRow);
                        }
                    }
                }
            }
        }

        private void InitialdgvUploadFile()
        {
            dgvUploadFile.AlternatingRowsDefaultCellStyle.BackColor = Color.PaleTurquoise;      //奇數列顏色
            //dgvUploadFile.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvUploadFile.Columns[0].Width = 80;
            dgvUploadFile.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            // 根據欄位內容調整
            int nTotalWidth = dgvUploadFile.Width;

            // 設定不同欄位的百分比
            int nFilePathColumnWidth = (int)(nTotalWidth * 0.6); // 60%

            if (dgvUploadFile.Columns[1].Width > nFilePathColumnWidth)
            {
                dgvUploadFile.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvUploadFile.Columns[1].Width = nFilePathColumnWidth;
            }
            else
            {
                dgvUploadFile.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            dgvUploadFile.Columns[1].ReadOnly = true;
            dgvUploadFile.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            //建立個矩形，等下計算 CheckBox 嵌入 GridView 的位置
            Rectangle structRect = dgvUploadFile.GetCellDisplayRectangle(0, -1, true);
            structRect.X = structRect.Location.X + structRect.Width / 4 - 10;
            structRect.Y = structRect.Location.Y + (structRect.Height / 2 - 7);

            m_ckbxItem.Name = "Item";
            m_ckbxItem.Size = new Size(15, 15);
            m_ckbxItem.Location = structRect.Location;
            //全選要設定的事件
            m_ckbxItem.Click += new EventHandler(ckbxCheck_CheckedChanged);

            //將 CheckBox 加入到 dataGridView
            dgvUploadFile.Controls.Add(m_ckbxItem);

            ((CheckBox)dgvUploadFile.Controls.Find("Item", true)[0]).Checked = false;
        }

        private void ckbxCheck_CheckedChanged(object sender, EventArgs e)
        {
            dgvUploadFile.EndEdit();

            foreach (DataGridViewRow dgvrRow in dgvUploadFile.Rows)
                dgvrRow.Cells[0].Value = ((CheckBox)dgvUploadFile.Controls.Find("Item", true)[0]).Checked;
        }

        private void dgvUploadFile_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            Rectangle structRect = dgvUploadFile.GetCellDisplayRectangle(0, -1, true);
            structRect.X = structRect.Location.X + structRect.Width / 4 - 15;
            structRect.Y = structRect.Location.Y + (structRect.Height / 2 - 7);

            m_ckbxItem.Location = structRect.Location;
        }

        private void dgvUploadFile_Scroll(object sender, ScrollEventArgs e)
        {
            Rectangle structRect = dgvUploadFile.GetCellDisplayRectangle(0, -1, true);
            structRect.X = structRect.Location.X + structRect.Width / 4 - 15;
            structRect.Y = structRect.Location.Y + (structRect.Height / 2 - 7);

            if (structRect.X < 0)
            {
                structRect = dgvUploadFile.GetCellDisplayRectangle(0, -1, true);
                structRect.X = structRect.Location.X + structRect.Width / 4 - 15;
                structRect.Y = structRect.Location.Y + (structRect.Height / 2 - 7);
            }

            m_ckbxItem.Location = structRect.Location;
        }

        private void dgvUploadFile_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                dgvUploadFile.EndEdit();

                if ((bool)dgvUploadFile.Rows[e.RowIndex].Cells[0].Value == false)
                    ((CheckBox)dgvUploadFile.Controls.Find("Item", true)[0]).Checked = false;
            }
        }

        private void dgvUploadFile_HandleCreated(object sender, EventArgs e)
        {
            InitialdgvUploadFile();
        }

        private void rtbxDescription_HandleCreated(object sender, EventArgs e)
        {
            for (int nIndex = 0; nIndex < m_sDescriptionTest_Array.Length; nIndex++)
                OutputDescription(m_sDescriptionTest_Array[nIndex]);
        }

        private void rtbxMessage_TextChanged(object sender, EventArgs e)
        {
            //Scrolls the contents of the control to the current caret position.
            rtbxMessage.ScrollToCaret();
        }

        private void dgvUploadFile_SizeChanged(object sender, EventArgs e)
        {
            int nTotalWidth = dgvUploadFile.Width;

            // 設定不同欄位的百分比
            int nFilePathColumnWidth = (int)(nTotalWidth * 0.6); // 60%

            if (dgvUploadFile.Columns[1].Width > nFilePathColumnWidth)
            {
                dgvUploadFile.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvUploadFile.Columns[1].Width = nFilePathColumnWidth;
            }
            else
            {
                dgvUploadFile.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void dgvUploadFile_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            /*
            if (e.ColumnIndex == 1 && e.RowIndex >= 0) // 確保是數據單元格
            {
                string sCellText = e.Value.ToString();
                int nMaxLength = 100;

                if (sCellText.Length > nMaxLength)
                {
                    dgvUploadFile[e.ColumnIndex, e.RowIndex].Value = "..." + sCellText.Substring(sCellText.Length - nMaxLength);
                }
            }
            */
        }

        private void dgvUploadFile_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                // 顯示原始的完整文字
                var vOriginalValue = dgvUploadFile.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                e.ToolTipText = vOriginalValue.ToString();
            }
        }
    }
}
