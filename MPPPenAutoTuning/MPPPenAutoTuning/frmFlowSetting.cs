using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class frmFlowSetting : Form
    {
        private frmMain m_cfrmMain;

        private string m_sFlowFileName;
        private string m_sFlowFilePath;
        private string m_sAllFlowFilePath;

        private CheckBox m_ckbxAllSelectHeader = new CheckBox();
        private CheckBox m_ckbxSettingHeader = new CheckBox();

        private const string m_sAllFlowFileName = "flow_All.txt";

        private List<List<string>> m_sAllFlowData_List = new List<List<string>>();
        private List<List<bool>> m_bAllFlowData_List = new List<List<bool>>();

        private List<List<string>> m_sFlowData_List = new List<List<string>>();
        private List<List<bool>> m_bFlowData_List = new List<List<bool>>();

        private bool m_bAllSelectFormatErrorFlag = false;
        private bool m_bSettingFormatErrorFlag = false;

        private string m_sAllSelectErrorMessage = "";
        private string m_sSettingErrorMessage = "";

        private bool m_bBeginEditFlag = false;

        private int m_nMPPVersionIndex = -1;

        private int m_nFrequencyLB = ParamAutoTuning.m_nFrequencyLB;
        private int m_nFrequencyHB = ParamAutoTuning.m_nFrequencyHB;

        public bool m_bSaveFlag = false;

        private enum RobotState
        {
            NO
        }

        private enum RecordState
        {
            NTRX
        }

        private enum FlowFileInfo
        {
            RobotState = 0,
            RecordState = 1,
            PH1 = 2,
            PH2 = 3
        }

        private enum DataGridViewCellInfo : int
        {
            CheckBox = 0,
            RobotState = 1,
            RecordState = 2,
            PH1 = 3,
            PH2 = 4,
            Frequency = 5
        }

        private const int m_nCheckBoxIndex = (int)DataGridViewCellInfo.CheckBox;
        private const int m_nRobotStateIndex = (int)DataGridViewCellInfo.RobotState;
        private const int m_nRecordStateIndex = (int)DataGridViewCellInfo.RecordState;
        private const int m_nPH1Index = (int)DataGridViewCellInfo.PH1;
        private const int m_nPH2Index = (int)DataGridViewCellInfo.PH2;
        private const int m_nFrequencyIndex = (int)DataGridViewCellInfo.Frequency;

        public frmFlowSetting(frmMain cfrmMain)
        {
            InitializeComponent();

            m_cfrmMain = cfrmMain;
            m_sFlowFileName = m_cfrmMain.m_sSubTuningStepFileName_Array[(int)SubTuningStep.NO];
            m_sFlowFilePath = string.Format(@"{0}\{1}", m_cfrmMain.m_sFlowDirectoryPath, m_sFlowFileName);
            m_sAllFlowFilePath = string.Format(@"{0}\{1}", m_cfrmMain.m_sFlowDirectoryPath, m_sAllFlowFileName);

            m_bAllSelectFormatErrorFlag = false;
            m_bSettingFormatErrorFlag = false;

            m_sAllSelectErrorMessage = "";
            m_sSettingErrorMessage = "";

            //cbxPH1.SelectedIndex = cbxPH1.FindStringExact("08");
            //cbxMPP_Version.SelectedIndex = cbxMPP_Version.FindStringExact("151");
            cbxPH1.SelectedIndex = cbxPH1.FindStringExact("10");
            cbxMPP_Version.SelectedIndex = cbxMPP_Version.FindStringExact("180");
            tbxFrequencyLB.Text = m_nFrequencyLB.ToString("D");

            SetFrequencyItem();

            m_bBeginEditFlag = false;
            m_bSaveFlag = false;
        }

        private void SetFrequencyItem()
        {
            int nCurrentFrequencyLB = ParamAutoTuning.m_nFrequencyLB;

            if (cbxMPP_Version.SelectedIndex == 0)
                nCurrentFrequencyLB = ParamAutoTuning.m_nFrequencyLB;
            else if (cbxMPP_Version.SelectedIndex == 1)
                nCurrentFrequencyLB = ParamAutoTuning.m_nFrequencyLB_MPP180;

            SetFrequencyFlowItem(nCurrentFrequencyLB);

            m_nMPPVersionIndex = cbxMPP_Version.SelectedIndex;

            SetFrequencyTextBox();
        }

        private void frmFlowSetting_Load(object sender, EventArgs e)
        {
            if (File.Exists(m_sAllFlowFilePath) == false)
                LoadAllSelectFlow(true);
            else
                LoadAllSelectFlow(false, m_sAllFlowFilePath);

            if (File.Exists(m_sFlowFilePath) == false)
                File.Create(m_sFlowFilePath);

            LoadSettingFlow(m_sFlowFilePath);
        }

        public void ShowFormatErrorMessage()
        {
            if (m_bAllSelectFormatErrorFlag == true)
            {
                ShowMessageBox(m_sAllSelectErrorMessage, frmMessageBox.m_sError);
            }

            if (m_bSettingFormatErrorFlag == true)
            {
                ShowMessageBox(m_sSettingErrorMessage, frmMessageBox.m_sError);
            }
        }

        public void LoadAllSelectFlow(bool bDefault, string sAllFlowFilePath = null)
        {
            dgvAllSelectFlow.AlternatingRowsDefaultCellStyle.BackColor = Color.PaleTurquoise;      //奇數列顏色
            dgvAllSelectFlow.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            AllSelectRobotState.Items.AddRange(Enum.GetNames(typeof(RobotState)));
            AllSelectRobotState.Items.Add("N/A");
            AllSelectRecordState.Items.AddRange(Enum.GetNames(typeof(RecordState)));
            AllSelectRecordState.Items.Add("N/A");

            CreateCheckBox(dgvAllSelectFlow, m_ckbxAllSelectHeader);

            m_sAllFlowData_List.Clear();
            m_bAllFlowData_List.Clear();

            if (bDefault == false)
            {
                List<string> sAllFlow_List = new List<string>(File.ReadAllLines(sAllFlowFilePath));

                if (sAllFlow_List.Count <= 0)
                {
                    m_sAllSelectErrorMessage += string.Format("No Flow in {0}", m_sAllFlowFileName) + Environment.NewLine;
                    m_bAllSelectFormatErrorFlag = true;
                    return;
                }

                for (int nLineIndex = 0; nLineIndex < sAllFlow_List.Count; nLineIndex++)
                {
                    string[] sData_Array = sAllFlow_List[nLineIndex].Split(',');
                    byte byteErrorFlag = 0x00;
                    List<string> sRowData_List = new List<string>();
                    List<bool> bRowData_List = new List<bool>();

                    for (int nRowIndex = 0; nRowIndex < Enum.GetNames(typeof(FlowFileInfo)).Length; nRowIndex++)
                    {
                        if (sData_Array.Length - 1 < nRowIndex)
                        {
                            sRowData_List.Add("N/A");
                            bRowData_List.Add(true);
                            m_sAllSelectErrorMessage += string.Format("Format Length Error in Line {0} in {1}", nLineIndex + 1, m_sAllFlowFileName) + Environment.NewLine;
                            byteErrorFlag |= 0x04;
                            m_bAllSelectFormatErrorFlag = true;
                        }
                        else
                        {
                            int nMarkFlag = 0x00;
                            bool bErrorFlag = false;
                            string sErrorItem = "";

                            switch (nRowIndex)
                            {
                                case (int)FlowFileInfo.RobotState:
                                    if (Enum.IsDefined(typeof(RobotState), sData_Array[nRowIndex]) == false)
                                    {
                                        nMarkFlag = 0x01;
                                        bErrorFlag = true;
                                        sErrorItem = FlowFileInfo.RobotState.ToString();
                                    }

                                    break;
                                case (int)FlowFileInfo.RecordState:
                                    if (Enum.IsDefined(typeof(RecordState), sData_Array[nRowIndex]) == false)
                                    {
                                        nMarkFlag = 0x01;
                                        bErrorFlag = true;
                                        sErrorItem = FlowFileInfo.RecordState.ToString();
                                    }

                                    break;
                                case (int)FlowFileInfo.PH1:
                                    if (ElanConvert.CheckIsInt(sData_Array[nRowIndex]) == false)
                                    {
                                        nMarkFlag = 0x02;
                                        bErrorFlag = true;
                                        sErrorItem = FlowFileInfo.PH1.ToString();
                                    }

                                    break;
                                case (int)FlowFileInfo.PH2:
                                    if (ElanConvert.CheckIsInt(sData_Array[nRowIndex]) == false)
                                    {
                                        nMarkFlag = 0x02;
                                        bErrorFlag = true;
                                        sErrorItem = FlowFileInfo.PH2.ToString();
                                    }

                                    break;
                                default:
                                    break;
                            }

                            if (bErrorFlag == false)
                            {
                                sRowData_List.Add(sData_Array[nRowIndex]);
                                bRowData_List.Add(false);
                            }
                            else
                            {
                                if (nMarkFlag == 0x01)
                                    sRowData_List.Add("N/A");
                                else if (nMarkFlag == 0x02)
                                    sRowData_List.Add(sData_Array[nRowIndex]);

                                bRowData_List.Add(true);
                                m_sAllSelectErrorMessage += string.Format("{0} Value Error in Line {1} in {2}", sErrorItem, nLineIndex + 1, m_sAllFlowFileName) + Environment.NewLine;
                                byteErrorFlag |= (byte)nMarkFlag;
                                m_bAllSelectFormatErrorFlag = true;
                            }
                        }
                    }

                    if ((byteErrorFlag & 0x02) == 0)
                    {
                        int nPH1 = Convert.ToInt32(sRowData_List[(int)FlowFileInfo.PH1]);
                        int nPH2 = Convert.ToInt32(sRowData_List[(int)FlowFileInfo.PH2]);
                        string sFrequency = ElanConvert.ComputeFrequnecyToString(nPH1, nPH2);
                        sRowData_List.Add(sFrequency);

                        if (ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2) > m_nFrequencyLB &&
                            ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2) < m_nFrequencyHB)
                            bRowData_List.Add(false);
                        else
                        {
                            bRowData_List.Add(true);
                            m_sSettingErrorMessage += string.Format("Frequency Over Range in Line {0} in {1}", nLineIndex + 1, m_sAllFlowFileName) + Environment.NewLine;
                            m_bSettingFormatErrorFlag = true;
                        }
                    }
                    else if (byteErrorFlag != 0x00)
                    {
                        sRowData_List.Add("N/A");
                        bRowData_List.Add(true);
                    }

                    m_sAllFlowData_List.Add(new List<string>(sRowData_List));
                    m_bAllFlowData_List.Add(new List<bool>(bRowData_List));
                }
            }
            else
            {
                string sRobotState = RobotState.NO.ToString();
                string sRecordState = RecordState.NTRX.ToString();

                int nPH1 = Convert.ToInt32(cbxPH1.SelectedItem.ToString());
                int nPH2 = (int)(MainConstantParameter.m_nICCLOCKFREQUENCY / (m_nFrequencyLB * 1000)) - nPH1 - 2;

                int nMinusValue = 0;

                while (ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2 - nMinusValue) > m_nFrequencyLB &&
                       ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2 - nMinusValue) < m_nFrequencyHB)
                {
                    string sPH1 = nPH1.ToString().PadLeft(2, '0');
                    string sPH2 = (nPH2 - nMinusValue).ToString().PadLeft(2, '0');
                    List<string> sRowData_List = new List<string>();
                    List<bool> bRowData_List = new List<bool>();
                    sRowData_List.Add(sRobotState);
                    bRowData_List.Add(false);
                    sRowData_List.Add(sRecordState);
                    bRowData_List.Add(false);
                    sRowData_List.Add(sPH1);
                    bRowData_List.Add(false);
                    sRowData_List.Add(sPH2);
                    bRowData_List.Add(false);

                    string sFrequency = ElanConvert.ComputeFrequnecyToString(nPH1, nPH2 - nMinusValue);
                    sRowData_List.Add(sFrequency);
                    bRowData_List.Add(false);

                    m_sAllFlowData_List.Add(new List<string>(sRowData_List));
                    m_bAllFlowData_List.Add(new List<bool>(bRowData_List));
                    nMinusValue++;
                }
            }

            int nCount = m_sAllFlowData_List.Count;

            if (nCount > 0)
            {
                for (int nSetIndex = 0; nSetIndex < nCount; nSetIndex++)
                {
                    dgvAllSelectFlow.Rows.Add();
                    dgvAllSelectFlow.Rows[nSetIndex].HeaderCell.Value = (nSetIndex + 1).ToString();

                    dgvAllSelectFlow.Rows[nSetIndex].Cells[0].Value = false;
                    DataGridViewCellTagObject CkgxTagObject = new DataGridViewCellTagObject("false", false);
                    dgvAllSelectFlow.Rows[nSetIndex].Cells[0].Tag = CkgxTagObject;

                    for (int nInfoIndex = m_nRobotStateIndex; nInfoIndex <= Enum.GetValues(typeof(DataGridViewCellInfo)).Cast<int>().Max(); nInfoIndex++)
                    {
                        dgvAllSelectFlow.Rows[nSetIndex].Cells[nInfoIndex].Value = m_sAllFlowData_List[nSetIndex][nInfoIndex - 1];
                        DataGridViewCellTagObject TagObject = new DataGridViewCellTagObject(m_sAllFlowData_List[nSetIndex][nInfoIndex - 1],
                                                                                            m_bAllFlowData_List[nSetIndex][nInfoIndex - 1]);
                        dgvAllSelectFlow.Rows[nSetIndex].Cells[nInfoIndex].Tag = TagObject;

                        if (((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[nSetIndex].Cells[nInfoIndex].Tag).m_bErrorFlag == true)
                            dgvAllSelectFlow.Rows[nSetIndex].Cells[nInfoIndex].Style.BackColor = Color.Red;
                    }
                }
            }

            bool bHasIdenticalFlag = CheckSettingFlowRowsIdentical(dgvAllSelectFlow, false);

            if (bHasIdenticalFlag == true)
            {
                m_sAllSelectErrorMessage += "Some All Select Flow has the Same PH1 & PH2" + Environment.NewLine;
                m_bAllSelectFormatErrorFlag = true;
            }

            bool bAllCheckedFlag = true;

            foreach (DataGridViewRow dgvrControl in dgvAllSelectFlow.Rows)
            {
                if ((bool)dgvrControl.Cells[m_nCheckBoxIndex].Value == false)
                {
                    bAllCheckedFlag = false;
                    break;
                }
            }

            if (bAllCheckedFlag == true)
                m_ckbxAllSelectHeader.Checked = true;
        }

        public void LoadSettingFlow(string sFlowPath)
        {
            //SettingFlowDataGridView.EndEdit();
            dgvSettingFlow.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;      //奇數列顏色
            dgvSettingFlow.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            SettingRobotState.Items.AddRange(Enum.GetNames(typeof(RobotState)));
            SettingRobotState.Items.Add("N/A");
            SettingRecordState.Items.AddRange(Enum.GetNames(typeof(RecordState)));
            SettingRecordState.Items.Add("N/A");

            CreateCheckBox(dgvSettingFlow, m_ckbxSettingHeader);

            m_sFlowData_List.Clear();
            m_bFlowData_List.Clear();

            List<string> sFlow_List = new List<string>(File.ReadAllLines(sFlowPath));

            if (sFlow_List.Count <= 0)
            {
                m_sSettingErrorMessage += string.Format("No Flow in {0}", m_sFlowFileName) + Environment.NewLine;
                m_bSettingFormatErrorFlag = true;
                return;
            }

            for (int nLineIndex = 0; nLineIndex < sFlow_List.Count; nLineIndex++)
            {
                string[] sData_Array = sFlow_List[nLineIndex].Split(',');
                byte byteErrorFlag = 0x00;
                List<string> sRowData_List = new List<string>();
                List<bool> bRowData_List = new List<bool>();

                for (int nRowIndex = 0; nRowIndex < Enum.GetNames(typeof(FlowFileInfo)).Length; nRowIndex++)
                {
                    if (sData_Array.Length - 1 < nRowIndex)
                    {
                        sRowData_List.Add("N/A");
                        bRowData_List.Add(true);
                        m_sSettingErrorMessage += string.Format("Format Length Error in Line {0} in {1}", nLineIndex + 1, m_sFlowFileName) + Environment.NewLine;
                        byteErrorFlag |= 0x04;

                        if (nRowIndex == 2 || nRowIndex == 3)
                            byteErrorFlag |= 0x02;

                        m_bSettingFormatErrorFlag = true;
                    }
                    else
                    {
                        int nMarkFlag = 0x00;
                        bool bErrorFlag = false;
                        string sErrorItem = "";

                        switch (nRowIndex)
                        {
                            case (int)FlowFileInfo.RobotState:
                                if (Enum.IsDefined(typeof(RobotState), sData_Array[nRowIndex]) == false)
                                {
                                    nMarkFlag = 0x01;
                                    bErrorFlag = true;
                                    sErrorItem = FlowFileInfo.RobotState.ToString();
                                }

                                break;
                            case (int)FlowFileInfo.RecordState:
                                if (Enum.IsDefined(typeof(RecordState), sData_Array[nRowIndex]) == false)
                                {
                                    nMarkFlag = 0x01;
                                    bErrorFlag = true;
                                    sErrorItem = FlowFileInfo.RecordState.ToString();
                                }

                                break;
                            case (int)FlowFileInfo.PH1:
                                if (ElanConvert.CheckIsInt(sData_Array[nRowIndex]) == false)
                                {
                                    nMarkFlag = 0x02;
                                    bErrorFlag = true;
                                    sErrorItem = FlowFileInfo.PH1.ToString();
                                }

                                break;
                            case (int)FlowFileInfo.PH2:
                                if (ElanConvert.CheckIsInt(sData_Array[nRowIndex]) == false)
                                {
                                    nMarkFlag = 0x02;
                                    bErrorFlag = true;
                                    sErrorItem = FlowFileInfo.PH2.ToString();
                                }

                                break;
                            default:
                                break;
                        }

                        if (bErrorFlag == false)
                        {
                            sRowData_List.Add(sData_Array[nRowIndex]);
                            bRowData_List.Add(false);
                        }
                        else
                        {
                            if (nMarkFlag == 0x01)
                                sRowData_List.Add("N/A");
                            else if (nMarkFlag == 0x02)
                                sRowData_List.Add(sData_Array[nRowIndex]);

                            bRowData_List.Add(true);
                            m_sAllSelectErrorMessage += string.Format("{0} Value Error in Line {1} in {2}", sErrorItem, nLineIndex + 1, m_sFlowFileName) + Environment.NewLine;
                            byteErrorFlag |= (byte)nMarkFlag;
                            m_bAllSelectFormatErrorFlag = true;
                        }
                    }
                }

                if ((byteErrorFlag & 0x02) == 0)
                {
                    int nPH1 = Convert.ToInt32(sRowData_List[(int)FlowFileInfo.PH1]);
                    int nPH2 = Convert.ToInt32(sRowData_List[(int)FlowFileInfo.PH2]);

                    string sFrequency = ElanConvert.ComputeFrequnecyToString(nPH1, nPH2);
                    sRowData_List.Add(sFrequency);

                    if (ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2) > m_nFrequencyLB &&
                        ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2) < m_nFrequencyHB)
                        bRowData_List.Add(false);
                    else
                    {
                        bRowData_List.Add(true);
                        m_sSettingErrorMessage += string.Format("Frequency Over Range in Line {0} in {1}", nLineIndex + 1, m_sFlowFileName) + Environment.NewLine;
                        m_bSettingFormatErrorFlag = true;
                    }
                }
                else if (byteErrorFlag != 0x00)
                {
                    sRowData_List.Add("N/A");
                    bRowData_List.Add(true);
                }

                m_sFlowData_List.Add(new List<string>(sRowData_List));
                m_bFlowData_List.Add(new List<bool>(bRowData_List));
            }

            int nCount = m_sFlowData_List.Count;

            if (nCount > 0)
            {
                for (int nSetIndex = 0; nSetIndex < nCount; nSetIndex++)
                {
                    dgvSettingFlow.Rows.Add();
                    dgvSettingFlow.Rows[nSetIndex].HeaderCell.Value = (nSetIndex + 1).ToString();

                    dgvSettingFlow.Rows[nSetIndex].Cells[0].Value = false;
                    DataGridViewCellTagObject CkgxTagObject = new DataGridViewCellTagObject("false", false);
                    dgvSettingFlow.Rows[nSetIndex].Cells[0].Tag = CkgxTagObject;

                    for (int nInfoIndex = m_nRobotStateIndex; nInfoIndex <= Enum.GetValues(typeof(DataGridViewCellInfo)).Cast<int>().Max(); nInfoIndex++)
                    {
                        dgvSettingFlow.Rows[nSetIndex].Cells[nInfoIndex].Value = m_sFlowData_List[nSetIndex][nInfoIndex - 1];
                        DataGridViewCellTagObject TagObject = new DataGridViewCellTagObject(m_sFlowData_List[nSetIndex][nInfoIndex - 1],
                                                                                            m_bFlowData_List[nSetIndex][nInfoIndex - 1]);
                        dgvSettingFlow.Rows[nSetIndex].Cells[nInfoIndex].Tag = TagObject;

                        if (dgvSettingFlow.Rows[nSetIndex].Cells[nInfoIndex].Tag != null && ((DataGridViewCellTagObject)dgvSettingFlow.Rows[nSetIndex].Cells[nInfoIndex].Tag).m_bErrorFlag == true)
                            dgvSettingFlow.Rows[nSetIndex].Cells[nInfoIndex].Style.BackColor = Color.Red;
                    }
                }
            }

            bool bHasIdenticalFlag = CheckSettingFlowRowsIdentical(dgvSettingFlow);

            if (bHasIdenticalFlag == true)
            {
                m_sSettingErrorMessage += "Some Setting Flow has the Same PH1 & PH2" + Environment.NewLine;
                m_bSettingFormatErrorFlag = true;
            }
        }

        private void CreateCheckBox(DataGridView dgvControl, CheckBox cbxControl)
        {
            //建立個矩形，等下計算 CheckBox 嵌入 GridView 的位置
            Rectangle structRectangle = dgvControl.GetCellDisplayRectangle(0, -1, true);
            structRectangle.X = structRectangle.Location.X + structRectangle.Width / 4 - 15;
            structRectangle.Y = structRectangle.Location.Y + (structRectangle.Height / 2 - 7);

            cbxControl.Name = "Item";
            cbxControl.Size = new Size(15, 15);
            cbxControl.Location = structRectangle.Location;
            //全選要設定的事件
            cbxControl.Click += new EventHandler(checkboxHeader_CheckedChanged);

            //將 CheckBox 加入到 dataGridView
            dgvControl.Controls.Add(cbxControl);
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            int nRowCount = dgvSettingFlow.Rows.Count;

            foreach (DataGridViewRow dgvrControl in dgvAllSelectFlow.Rows)
            {
                if (dgvrControl.Cells[m_nCheckBoxIndex].Value != null && (bool)dgvrControl.Cells[m_nCheckBoxIndex].Value)
                {
                    bool bOverlappedFlag = false;

                    for (int Index = 0; Index < nRowCount; Index++)
                    {
                        if ((dgvSettingFlow.Rows[Index].Cells[m_nPH1Index].Value.ToString() == dgvrControl.Cells[m_nPH1Index].Value.ToString()) &&
                            (dgvSettingFlow.Rows[Index].Cells[m_nPH2Index].Value.ToString() == dgvrControl.Cells[m_nPH2Index].Value.ToString()))
                            bOverlappedFlag = true;
                    }

                    if (bOverlappedFlag == false)
                    {
                        dgvSettingFlow.Rows.Add();
                        dgvSettingFlow.Rows[nRowCount].HeaderCell.Value = (nRowCount + 1).ToString();
                        for (int nInfoIndex = m_nRobotStateIndex; nInfoIndex <= Enum.GetValues(typeof(DataGridViewCellInfo)).Cast<int>().Max(); nInfoIndex++)
                        {
                            dgvSettingFlow.Rows[nRowCount].Cells[nInfoIndex].Value = dgvrControl.Cells[nInfoIndex].Value;
                            DataGridViewCellTagObject TagObject = new DataGridViewCellTagObject(((DataGridViewCellTagObject)dgvrControl.Cells[nInfoIndex].Tag).m_sTagValue,
                                ((DataGridViewCellTagObject)dgvrControl.Cells[nInfoIndex].Tag).m_bErrorFlag);
                            dgvSettingFlow.Rows[nRowCount].Cells[nInfoIndex].Tag = TagObject;
                        
                            if (dgvSettingFlow.Rows[nRowCount].Cells[nInfoIndex].Value != null)
                            {
                                if (dgvSettingFlow.Rows[nRowCount].Cells[nInfoIndex].Tag != null && ((DataGridViewCellTagObject)dgvSettingFlow.Rows[nRowCount].Cells[nInfoIndex].Tag).m_bErrorFlag == true)
                                    dgvSettingFlow.Rows[nRowCount].Cells[nInfoIndex].Style.BackColor = Color.Red;
                                else
                                {
                                    if (nRowCount % 2 == 1)
                                        dgvSettingFlow.Rows[nRowCount].Cells[nInfoIndex].Style.BackColor = Color.LightCyan;
                                    else
                                        dgvSettingFlow.Rows[nRowCount].Cells[nInfoIndex].Style.BackColor = Color.White;
                                }
                            }
                        }

                        nRowCount++;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (File.Exists(m_sFlowFilePath) == false)
                File.Create(m_sFlowFilePath);

			int nRowCount = dgvSettingFlow.Rows.Count;

            if (nRowCount <= 0)
            {
                ShowMessage("No Setting Flow Data! Please Check It.", "No Setting Flow Data Error Warning");
                return;
            }
			
            foreach (DataGridViewRow dgvrControl in dgvSettingFlow.Rows)
            {
                for (int nColumnIndex = m_nRobotStateIndex; nColumnIndex <= Enum.GetValues(typeof(DataGridViewCellInfo)).Cast<int>().Max(); nColumnIndex++)
                {
                    if (dgvrControl.Cells[nColumnIndex].Value == null ||
                        ((DataGridViewCellTagObject)dgvrControl.Cells[nColumnIndex].Tag).m_sTagValue == "N/A" ||
                        ((DataGridViewCellTagObject)dgvrControl.Cells[nColumnIndex].Tag).m_bErrorFlag == true ||
                        ((DataGridViewCellTagObject)dgvrControl.Cells[nColumnIndex].Tag).m_bIdenticalFlag == true)
                    {
                        ShowMessage("Setting Flow Data Format Error! Please Check It.", "Setting Flow Data Format Error Warning");
                        return;
                    }
                }
            }

            StreamWriter sw = new StreamWriter(m_sFlowFilePath, false);

            foreach (DataGridViewRow dgvrControl in dgvSettingFlow.Rows)
            {
                for (int nColumnIndex = m_nRobotStateIndex; nColumnIndex <= m_nPH2Index; nColumnIndex++)
                {
                    if (nColumnIndex == m_nPH2Index)
                        sw.WriteLine(string.Format("{0}", dgvrControl.Cells[nColumnIndex].Value.ToString()));
                    else
                        sw.Write(string.Format("{0},", dgvrControl.Cells[nColumnIndex].Value.ToString()));
                }
            }

            sw.Close();

            ShowMessageBox("Save Complete!!", frmMessageBox.m_sMessage);

            m_bSaveFlag = true;
            m_bAllSelectFormatErrorFlag = false;
            m_bSettingFormatErrorFlag = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            dgvSettingFlow.Rows.Add();

            DataGridViewComboBoxColumn cbxRobotState = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn cbxRecordState = new DataGridViewComboBoxColumn();

            cbxRobotState.DefaultCellStyle.NullValue = RobotState.NO.ToString();
            cbxRecordState.DefaultCellStyle.NullValue = RecordState.NTRX.ToString();

            int nRowCount = dgvSettingFlow.Rows.Count;
            dgvSettingFlow.Rows[nRowCount - 1].HeaderCell.Value = nRowCount.ToString();
            dgvSettingFlow.Rows[nRowCount - 1].HeaderCell.Tag = nRowCount.ToString();

            for (int nInfoIndex = m_nRobotStateIndex; nInfoIndex < Enum.GetValues(typeof(DataGridViewCellInfo)).Cast<int>().Max(); nInfoIndex++)
            {
                switch(nInfoIndex)
                {
                    case m_nRobotStateIndex:
                        dgvSettingFlow.Rows[nRowCount - 1].Cells[nInfoIndex].Value = cbxRobotState.DefaultCellStyle.NullValue;
                        DataGridViewCellTagObject dgvcoRobotState = new DataGridViewCellTagObject(cbxRobotState.DefaultCellStyle.NullValue.ToString(), false);
                        dgvSettingFlow.Rows[nRowCount - 1].Cells[nInfoIndex].Tag = dgvcoRobotState;
                        break;
                    case m_nRecordStateIndex:
                        dgvSettingFlow.Rows[nRowCount - 1].Cells[nInfoIndex].Value = cbxRecordState.DefaultCellStyle.NullValue;
                        DataGridViewCellTagObject dgvcoRecordState = new DataGridViewCellTagObject(cbxRecordState.DefaultCellStyle.NullValue.ToString(), false);
                        dgvSettingFlow.Rows[nRowCount - 1].Cells[nInfoIndex].Tag = dgvcoRecordState;
                        break;
                    case m_nPH1Index:
                    case m_nPH2Index:
                    case m_nFrequencyIndex:
                        dgvSettingFlow.Rows[nRowCount - 1].Cells[nInfoIndex].Value = "N/A";
                        DataGridViewCellTagObject dgvctoControl = new DataGridViewCellTagObject("N/A", true);
                        dgvSettingFlow.Rows[nRowCount - 1].Cells[m_nPH1Index].Tag = dgvctoControl;
                        break;
                    default:
                        break;
                }
            }

            for (int nColumnIndex = 0; nColumnIndex < dgvSettingFlow.ColumnCount; nColumnIndex++)
            {
                if (dgvSettingFlow.Rows[nRowCount - 1].Cells[nColumnIndex].Value != null)
                {
                    if (dgvSettingFlow.Rows[nRowCount - 1].Cells[nColumnIndex].Tag != null && ((DataGridViewCellTagObject)dgvSettingFlow.Rows[nRowCount - 1].Cells[nColumnIndex].Tag).m_bErrorFlag == true)
                        dgvSettingFlow.Rows[nRowCount - 1].Cells[nColumnIndex].Style.BackColor = Color.Red;
                    else
                    {
                        if (nRowCount - 1 % 2 == 1)
                            dgvSettingFlow.Rows[nRowCount - 1].Cells[nColumnIndex].Style.BackColor = Color.LightCyan;
                        else
                            dgvSettingFlow.Rows[nRowCount - 1].Cells[nColumnIndex].Style.BackColor = Color.White;
                    }
                }
            }

            dgvSettingFlow.CurrentCell = dgvSettingFlow.Rows[dgvSettingFlow.Rows.Count - 1].Cells[m_nCheckBoxIndex];
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int nRowCount = dgvSettingFlow.Rows.Count;

            for (int nRowIndex = nRowCount - 1; nRowIndex >= 0; nRowIndex--)
            {
                if (dgvSettingFlow.Rows[nRowIndex].Cells[m_nCheckBoxIndex].Value != null && (bool)dgvSettingFlow.Rows[nRowIndex].Cells[m_nCheckBoxIndex].Value)
                    dgvSettingFlow.Rows.Remove(dgvSettingFlow.Rows[nRowIndex]);
            }

            nRowCount = dgvSettingFlow.Rows.Count;
            int nDataIndex = 0;

            foreach (DataGridViewRow dgvrControl in dgvSettingFlow.Rows)
            {
                dgvrControl.HeaderCell.Value = (nDataIndex + 1).ToString();
                nDataIndex++;
            }

            ((CheckBox)dgvSettingFlow.Controls.Find("Item", true)[0]).Checked = false;

            for (int nRowIndex = 0; nRowIndex < nRowCount; nRowIndex++)
            {
                for (int nColumnIndex = 0; nColumnIndex < dgvSettingFlow.ColumnCount; nColumnIndex++)
                {
                    if (dgvSettingFlow.Rows[nRowIndex].Cells[nColumnIndex].Value != null)
                    {
                        if (dgvSettingFlow.Rows[nRowIndex].Cells[nColumnIndex].Tag != null && ((DataGridViewCellTagObject)dgvSettingFlow.Rows[nRowIndex].Cells[nColumnIndex].Tag).m_bErrorFlag == true)
                            dgvSettingFlow.Rows[nRowIndex].Cells[nColumnIndex].Style.BackColor = Color.Red;
                        else
                        {
                            if (nRowIndex % 2 == 1)
                                dgvSettingFlow.Rows[nRowIndex].Cells[nColumnIndex].Style.BackColor = Color.LightCyan;
                            else
                                dgvSettingFlow.Rows[nRowIndex].Cells[nColumnIndex].Style.BackColor = Color.White;
                        }
                    }
                }
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            while (dgvSettingFlow.Rows.Count != 0)
                dgvSettingFlow.Rows.RemoveAt(0);

            LoadSettingFlow(m_sFlowFilePath);

            ((CheckBox)dgvSettingFlow.Controls.Find("Item", true)[0]).Checked = false;
        }

        private void dgvAllSelectFlow_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            bool bCorrespondValueErrorFlag = false;
            bool bOverRangeFlag = false;

            SetDgvCellColor(dgvAllSelectFlow, e);

            if (m_bBeginEditFlag == false)
                return;

            if (e.ColumnIndex == m_nPH1Index || e.ColumnIndex == m_nPH2Index)
            {
                bool bErrorFlag = false;

                if (e.ColumnIndex == m_nPH1Index &&
                    (dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value == null || 
                    dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value.ToString().Length > 2 ||
                    ElanConvert.CheckIsInt(dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value.ToString()) == false))
                {
                    ShowMessageBox("PH1 Value Setting Error", frmMessageBox.m_sError);
                    ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Tag).m_bErrorFlag = true;
                    bErrorFlag = true;
                }
                else
                    ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Tag).m_bErrorFlag = false;

                if (e.ColumnIndex == m_nPH2Index &&
                    (dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value == null ||
                    dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value.ToString().Length > 2 ||
                    ElanConvert.CheckIsInt(dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value.ToString()) == false))
                {
                    ShowMessageBox("PH2 Value Setting Error", frmMessageBox.m_sError);
                    ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Tag).m_bErrorFlag = true;
                    bErrorFlag = true;
                }
                else
                    ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Tag).m_bErrorFlag = false;

                if (bErrorFlag == true)
                {
                    if (dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                        ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_sTagValue = dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                    dgvSettingFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Value = "N/A";
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Tag).m_sTagValue = "N/A";
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Tag).m_bErrorFlag = true;

                    SetDgvCellColor(dgvAllSelectFlow, e);
                    SetDgvCellColor(dgvAllSelectFlow, e, m_nFrequencyIndex);
                    return;
                }
                else
                {
                    dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().PadLeft(2, '0');

                    int nPH1 = 0;
                    int nPH2 = 0;

                    if (e.ColumnIndex == m_nPH1Index)
                    {
                        nPH1 = Convert.ToInt32(dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value.ToString());

                        if (dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value == null || 
                            ElanConvert.CheckIsInt(dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value.ToString()) == false)
                            bCorrespondValueErrorFlag = true;
                        else
                            nPH2 = Convert.ToInt32(dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value.ToString());
                    }
                    else if (e.ColumnIndex == m_nPH2Index)
                    {
                        if (dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value == null || 
                            ElanConvert.CheckIsInt(dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value.ToString()) == false)
                            bCorrespondValueErrorFlag = true;
                        else
                            nPH1 = Convert.ToInt32(dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value.ToString());

                        nPH2 = Convert.ToInt32(dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value.ToString());
                    }

                    if (bCorrespondValueErrorFlag == true)
                    {
                        ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Tag).m_bErrorFlag = true;
                        SetDgvCellColor(dgvAllSelectFlow, e, m_nFrequencyIndex);
                    }
                    else
                    {
                        if (ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2) < m_nFrequencyLB ||
                            ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2) > m_nFrequencyHB)
                            bOverRangeFlag = true;

                        string sFrequency = ElanConvert.ComputeFrequnecyToString(nPH1, nPH2);
                        dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Value = sFrequency;

                        if (bOverRangeFlag == true)
                        {
                            ShowMessageBox("Frequency Over Range", frmMessageBox.m_sError);
                            ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = true;
                            ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Tag).m_bErrorFlag = true;
                        }
                        else
                        {
                            ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = true;
                            ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Tag).m_bErrorFlag = false;
                        }

                        SetDgvCellColor(dgvAllSelectFlow, e, m_nFrequencyIndex);
                    }
                }
            }
            else if (e.ColumnIndex == m_nRobotStateIndex)
            {
                if (Enum.IsDefined(typeof(RobotState), dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                    ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = false;
                else
                    ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = true;
            }
            else if (e.ColumnIndex == m_nRecordStateIndex)
            {
                if (Enum.IsDefined(typeof(RecordState), dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                    ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = false;
                else
                    ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = true;
            }

            if (e.ColumnIndex >= m_nRobotStateIndex)
                ((DataGridViewCellTagObject)dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_sTagValue = dgvAllSelectFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            SetDgvCellColor(dgvAllSelectFlow, e);
            m_bBeginEditFlag = false;
        }

        private void dgvSettingFlow_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            bool bCorrespondValueErrorFlag = false;
            bool bOverRangeFlag = false;
            string sMessage = "";

            SetDgvCellColor(dgvSettingFlow, e);

            if (m_bBeginEditFlag == false)
                return;

            if (e.ColumnIndex == m_nPH1Index || e.ColumnIndex == m_nPH2Index)
            {
                bool bErrorFlag = false;
                if (e.ColumnIndex == m_nPH1Index &&
                    (dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value == null ||
                    ElanConvert.CheckIsInt(dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value.ToString()) == false))
                {
                    sMessage += "PH1 Value Setting Error" + Environment.NewLine;
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Tag).m_bErrorFlag = true;
                    bErrorFlag = true;
                }
                else
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Tag).m_bErrorFlag = false;

                if (e.ColumnIndex == m_nPH2Index &&
                    (dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value == null ||
                    ElanConvert.CheckIsInt(dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value.ToString()) == false))
                {
                    sMessage += "PH2 Value Setting Error" + Environment.NewLine;
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Tag).m_bErrorFlag = true;
                    bErrorFlag = true;
                }
                else
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Tag).m_bErrorFlag = false;

                if (bErrorFlag == true)
                {
                    if (dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                        ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_sTagValue = dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                    dgvSettingFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Value = "N/A";
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Tag).m_sTagValue = "N/A";
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Tag).m_bErrorFlag = true;

                    SetDgvCellColor(dgvSettingFlow, e);
                    SetDgvCellColor(dgvSettingFlow, e, m_nFrequencyIndex);
                    ShowMessageBox(sMessage, frmMessageBox.m_sError);
                    return;
                }
                else
                {
                    dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().PadLeft(2, '0');

                    int nPH1 = 0;
                    int nPH2 = 0;

                    if (e.ColumnIndex == m_nPH1Index)
                    {
                        nPH1 = Convert.ToInt32(dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value.ToString());

                        if (dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value == null || 
                            ElanConvert.CheckIsInt(dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value.ToString()) == false)
                            bCorrespondValueErrorFlag = true;
                        else
                            nPH2 = Convert.ToInt32(dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value.ToString());
                    }
                    else if (e.ColumnIndex == m_nPH2Index)
                    {
                        if (dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value == null || 
                            ElanConvert.CheckIsInt(dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value.ToString()) == false)
                            bCorrespondValueErrorFlag = true;
                        else
                            nPH1 = Convert.ToInt32(dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH1Index].Value.ToString());

                        nPH2 = Convert.ToInt32(dgvSettingFlow.Rows[e.RowIndex].Cells[m_nPH2Index].Value.ToString());
                    }

                    if (bCorrespondValueErrorFlag == true)
                    {
                        ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[5].Tag).m_bErrorFlag = true;
                        SetDgvCellColor(dgvSettingFlow, e, m_nFrequencyIndex);
                    }
                    else
                    {
                        if (ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2) < ParamAutoTuning.m_nFrequencyLB ||
                            ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2) > ParamAutoTuning.m_nFrequencyHB)
                            bOverRangeFlag = true;

                        string sFrequency = ElanConvert.ComputeFrequnecyToString(nPH1, nPH2);
                        dgvSettingFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Value = sFrequency;

                        if (bOverRangeFlag == true)
                        {
                            sMessage += "Frequency Over Range" + Environment.NewLine;
                            ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = true;
                            ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Tag).m_bErrorFlag = true;
                        }
                        else
                        {
                            ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = false;
                            ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[m_nFrequencyIndex].Tag).m_bErrorFlag = false;
                        }

                        SetDgvCellColor(dgvSettingFlow, e, m_nFrequencyIndex);
                    }
                }
            }
            else if (e.ColumnIndex == m_nRobotStateIndex)
            {
                if (Enum.IsDefined(typeof(RobotState), dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = false;
                else
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = true;
            }
            else if (e.ColumnIndex == m_nRecordStateIndex)
            {
                if (Enum.IsDefined(typeof(RecordState), dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = false;
                else
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag = true;
            }

            if (e.ColumnIndex >= m_nRobotStateIndex)
                ((DataGridViewCellTagObject)dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_sTagValue = dgvSettingFlow.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            SetDgvCellColor(dgvSettingFlow, e);

            bool bHasIdenticalFlag = CheckSettingFlowRowsIdentical(dgvSettingFlow, true, true, e);

            if (bHasIdenticalFlag == true)
                sMessage += "Same PH1 & PH2 in Other Setting Flow" + Environment.NewLine;

            if (sMessage != "")
            {
                ShowMessageBox(sMessage);
            }

            m_bBeginEditFlag = false;
        }

        private void checkboxHeader_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbxControl = (CheckBox)sender;
            DataGridView dgvParentControl = (DataGridView)(cbxControl.Parent);
            dgvParentControl.EndEdit();

            foreach (DataGridViewRow dgvrControl in dgvParentControl.Rows)
                dgvrControl.Cells[0].Value = ((CheckBox)dgvParentControl.Controls.Find("Item", true)[0]).Checked;
        }

        private void dgvAllSelectFlow_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == m_nCheckBoxIndex)
                SetCheckBoxCheckedState(sender, e);
        }

        private void dgvAllSelectFlow_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == m_nCheckBoxIndex)
                SetCheckBoxCheckedState(sender, e);
        }

        private void dgvSettingFlow_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == m_nCheckBoxIndex)
                SetCheckBoxCheckedState(sender, e);
        }

        private void dgvSettingFlow_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == m_nCheckBoxIndex)
                SetCheckBoxCheckedState(sender, e);
        }

        private void SetCheckBoxCheckedState(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgvControl = (DataGridView)sender;
            dgvControl.EndEdit();

            if ((bool)dgvControl.Rows[e.RowIndex].Cells[m_nCheckBoxIndex].Value == false)
                ((CheckBox)dgvControl.Controls.Find("Item", true)[0]).Checked = false;
        }

        private void dgvAllSelectFlow_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            SetDataGridViewRowBackColor(sender, e);
        }

        private void SettingFlowDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            SetDataGridViewRowBackColor(sender, e);
        }

        private void SetDataGridViewRowBackColor(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridView dgvControl = (DataGridView)sender;

            if (e.RowIndex >= 0)
            {
                if (e.RowIndex % 2 == 1)
                {
                    if (dgvControl.Name == dgvAllSelectFlow.Name)
                        dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.PaleTurquoise;
                    else if (dgvControl.Name == dgvSettingFlow.Name)
                        dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightCyan;
                }
                else
                    dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            }
        }

        private void cbxPH1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvAllSelectFlow.EndEdit();
            string sHexPH1 = cbxPH1.SelectedItem.ToString();
            int nPH1 = Convert.ToInt32(sHexPH1);
            int nPH2 = (int)(MainConstantParameter.m_nICCLOCKFREQUENCY / (m_nFrequencyLB * 1000)) - nPH1 - 2;

            int nMinusValue = 0;

            foreach (DataGridViewRow dgvrControl in dgvAllSelectFlow.Rows)
            {
                double dValue = ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2 - nMinusValue);

                if (dValue > m_nFrequencyLB && dValue < m_nFrequencyHB)
                {
                    string sPH1 = nPH1.ToString();
                    string sPH2 = (nPH2 - nMinusValue).ToString();
                    dgvrControl.Cells[m_nPH1Index].Value = sPH1.PadLeft(2, '0');
                    dgvrControl.Cells[m_nPH2Index].Value = sPH2.PadLeft(2, '0');
                    dgvrControl.Cells[m_nFrequencyIndex].Value = dValue.ToString();
                    nMinusValue++;
                }
            }
        }

        private void cbxMPP_Version_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_nMPPVersionIndex == -1 || m_nMPPVersionIndex == cbxMPP_Version.SelectedIndex)
            {
                m_nMPPVersionIndex = cbxMPP_Version.SelectedIndex;
                return;
            }

            SetFrequencyItem();
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            int nFrequencyLB = Convert.ToInt32(tbxFrequencyLB.Text);

            SetFrequencyFlowItem(nFrequencyLB);
        }

        private void SetFrequencyTextBox()
        {
            tbxFrequencyLB.Text = m_nFrequencyLB.ToString("D");
        }

        private void SetFrequencyFlowItem(int nFrequencyLB)
        {
            int nRowCount;

            if ((nFrequencyLB - m_nFrequencyLB) > 0)
            {
                nRowCount = dgvAllSelectFlow.Rows.Count;

                for (int nRowIndex = nRowCount - 1; nRowIndex >= 0; nRowIndex--)
                {
                    if (dgvAllSelectFlow.Rows[nRowIndex].Cells[m_nPH1Index].Value == null ||
                        ElanConvert.CheckIsInt(dgvAllSelectFlow.Rows[nRowIndex].Cells[m_nPH1Index].Value.ToString()) == false)
                        continue;

                    if (dgvAllSelectFlow.Rows[nRowIndex].Cells[m_nPH2Index].Value == null ||
                        ElanConvert.CheckIsInt(dgvAllSelectFlow.Rows[nRowIndex].Cells[m_nPH2Index].Value.ToString()) == false)
                        continue;

                    int nPH1 = Convert.ToInt32(dgvAllSelectFlow.Rows[nRowIndex].Cells[m_nPH1Index].Value.ToString());
                    int nPH2 = Convert.ToInt32(dgvAllSelectFlow.Rows[nRowIndex].Cells[m_nPH2Index].Value.ToString());

                    double dFrequencyValue = ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2);

                    if (dFrequencyValue < nFrequencyLB || dFrequencyValue > m_nFrequencyHB)
                        dgvAllSelectFlow.Rows.Remove(dgvAllSelectFlow.Rows[nRowIndex]);
                }
            }

            if ((nFrequencyLB - m_nFrequencyLB) < 0)
            {
                int nLB = nFrequencyLB;
                int nHB = m_nFrequencyLB;

                int nPH1 = Convert.ToInt32(cbxPH1.SelectedItem.ToString());
                int nPH2 = (int)Math.Ceiling(((double)MainConstantParameter.m_nICCLOCKFREQUENCY / (nHB * 1000)) - nPH1 - 2);

                int nPlusValue = 0;

                while (ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2 + nPlusValue) > nLB &&
                       ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2 + nPlusValue) < nHB)
                {
                    double dFrequencyValue = ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2 + nPlusValue);

                    DataGridViewRow dgvrControl = new DataGridViewRow();
                    dgvrControl.CreateCells(dgvAllSelectFlow);

                    dgvrControl.Cells[m_nCheckBoxIndex].Value = false;
                    DataGridViewCellTagObject dgvctoCheckBox = new DataGridViewCellTagObject("false", false);
                    dgvrControl.Cells[m_nCheckBoxIndex].Tag = dgvctoCheckBox;

                    DataGridViewComboBoxColumn cbxRobotState = new DataGridViewComboBoxColumn();
                    DataGridViewComboBoxColumn cbxRecordState = new DataGridViewComboBoxColumn();

                    cbxRobotState.DefaultCellStyle.NullValue = RobotState.NO.ToString();
                    cbxRecordState.DefaultCellStyle.NullValue = RecordState.NTRX.ToString();

                    dgvrControl.Cells[m_nRobotStateIndex].Value = cbxRobotState.DefaultCellStyle.NullValue;
                    dgvrControl.Cells[m_nRecordStateIndex].Value = cbxRecordState.DefaultCellStyle.NullValue;
                    dgvrControl.Cells[m_nPH1Index].Value = nPH1.ToString().PadLeft(2, '0');
                    dgvrControl.Cells[m_nPH2Index].Value = (nPH2 + nPlusValue).ToString().PadLeft(2, '0');
                    dgvrControl.Cells[m_nFrequencyIndex].Value = dFrequencyValue.ToString("#0.000");

                    DataGridViewCellTagObject dgvctoRobotState = new DataGridViewCellTagObject(cbxRobotState.DefaultCellStyle.NullValue.ToString(), false);
                    dgvrControl.Cells[m_nRobotStateIndex].Tag = dgvctoRobotState;
                    DataGridViewCellTagObject dgvctoRecordState = new DataGridViewCellTagObject(cbxRecordState.DefaultCellStyle.NullValue.ToString(), false);
                    dgvrControl.Cells[m_nRecordStateIndex].Tag = dgvctoRecordState;
                    DataGridViewCellTagObject dgvctoPH1 = new DataGridViewCellTagObject(nPH1.ToString().PadLeft(2, '0'), false);
                    dgvrControl.Cells[m_nPH1Index].Tag = dgvctoPH1;
                    DataGridViewCellTagObject dgvctoPH2 = new DataGridViewCellTagObject((nPH2 + nPlusValue).ToString().PadLeft(2, '0'), false);
                    dgvrControl.Cells[m_nPH2Index].Tag = dgvctoPH2;
                    DataGridViewCellTagObject dgvctoFrequency = new DataGridViewCellTagObject(dFrequencyValue.ToString("#0.000"), false);
                    dgvrControl.Cells[m_nFrequencyIndex].Tag = dgvctoFrequency;

                    dgvAllSelectFlow.Rows.Insert(0, dgvrControl);
                    nPlusValue++;
                }

                if (nPlusValue > 0)
                    m_ckbxAllSelectHeader.Checked = false;
            }

            int nDataIndex = 0;

            foreach (DataGridViewRow dgvrControl in dgvAllSelectFlow.Rows)
            {
                dgvrControl.HeaderCell.Value = (nDataIndex + 1).ToString();

                for (int nColumnIndex = Enum.GetValues(typeof(DataGridViewCellInfo)).Cast<int>().Min();
                     nColumnIndex <= Enum.GetValues(typeof(DataGridViewCellInfo)).Cast<int>().Max(); nColumnIndex++)
                {
                    if (nColumnIndex >= m_nRobotStateIndex)
                    {
                        if (dgvrControl.Cells[nColumnIndex].Value == null)
                            dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.Red;
                        else
                        {
                            if (dgvrControl.Cells[nColumnIndex].Tag != null && ((DataGridViewCellTagObject)dgvrControl.Cells[nColumnIndex].Tag).m_bErrorFlag == true ||
                                dgvrControl.Cells[nColumnIndex].Value.ToString() == "N/A")
                                dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.Red;
                            else
                            {
                                if (nDataIndex % 2 == 1)
                                    dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.PaleTurquoise;
                                else
                                    dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.White;
                            }
                        }
                    }
                    else
                    {
                        if (dgvrControl.Cells[nColumnIndex].Value == null)
                            dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.Red;
                        else
                        {
                            if (nDataIndex % 2 == 1)
                                dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.PaleTurquoise;
                            else
                                dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.White;
                        }
                    }
                }

                nDataIndex++;
            }

            m_ckbxAllSelectHeader.Checked = false;

            nRowCount = dgvSettingFlow.Rows.Count;

            for (int nRowIndex = nRowCount - 1; nRowIndex >= 0; nRowIndex--)
            {
                if (dgvSettingFlow.Rows[nRowIndex].Cells[m_nPH1Index].Value == null ||
                    ElanConvert.CheckIsInt(dgvSettingFlow.Rows[nRowIndex].Cells[m_nPH1Index].Value.ToString()) == false)
                    continue;

                if (dgvSettingFlow.Rows[nRowIndex].Cells[m_nPH2Index].Value == null ||
                    ElanConvert.CheckIsInt(dgvSettingFlow.Rows[nRowIndex].Cells[m_nPH2Index].Value.ToString()) == false)
                    continue;

                int nPH1 = Convert.ToInt32(dgvSettingFlow.Rows[nRowIndex].Cells[m_nPH1Index].Value.ToString());
                int nPH2 = Convert.ToInt32(dgvSettingFlow.Rows[nRowIndex].Cells[m_nPH2Index].Value.ToString());

                double dFrequencyValue = ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2);

                if (dFrequencyValue < nFrequencyLB || dFrequencyValue > m_nFrequencyHB)
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[nRowIndex].Cells[m_nFrequencyIndex].Tag).m_bErrorFlag = true;
                else
                    ((DataGridViewCellTagObject)dgvSettingFlow.Rows[nRowIndex].Cells[m_nFrequencyIndex].Tag).m_bErrorFlag = false;
            }

            nDataIndex = 0;

            foreach (DataGridViewRow dgvrControl in dgvSettingFlow.Rows)
            {
                dgvrControl.HeaderCell.Value = (nDataIndex + 1).ToString();

                for (int nColumnIndex = Enum.GetValues(typeof(DataGridViewCellInfo)).Cast<int>().Min();
                     nColumnIndex <= Enum.GetValues(typeof(DataGridViewCellInfo)).Cast<int>().Max(); nColumnIndex++)
                {
                    if (nColumnIndex >= 1)
                    {
                        if (dgvrControl.Cells[nColumnIndex].Value == null)
                            dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.Red;
                        else
                        {
                            if (dgvrControl.Cells[nColumnIndex].Tag != null && ((DataGridViewCellTagObject)dgvrControl.Cells[nColumnIndex].Tag).m_bErrorFlag == true ||
                                dgvrControl.Cells[nColumnIndex].Value.ToString() == "N/A")
                                dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.Red;
                            else
                            {
                                if (nDataIndex % 2 == 1)
                                    dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.LightCyan;
                                else
                                    dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.White;
                            }
                        }
                    }
                    else
                    {
                        if (nDataIndex % 2 == 1)
                            dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.LightCyan;
                        else
                            dgvrControl.Cells[nColumnIndex].Style.BackColor = Color.White;
                    }
                }

                nDataIndex++;
            }

            m_nFrequencyLB = nFrequencyLB;
        }

        private void SetDgvCellColor(DataGridView dgvControl, DataGridViewCellEventArgs e, int nCellColumnNumber = -1)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= m_nRobotStateIndex)
            {
                if (nCellColumnNumber > -1)
                {
                    if (dgvControl.Rows[e.RowIndex].Cells[nCellColumnNumber].Value == null)
                        dgvControl.Rows[e.RowIndex].Cells[nCellColumnNumber].Style.BackColor = Color.Red;
                    else
                    {
                        if (dgvControl.Rows[e.RowIndex].Cells[nCellColumnNumber].Tag != null && ((DataGridViewCellTagObject)dgvControl.Rows[e.RowIndex].Cells[nCellColumnNumber].Tag).m_bErrorFlag == true ||
                            dgvControl.Rows[e.RowIndex].Cells[nCellColumnNumber].Value.ToString() == "N/A")
                            dgvControl.Rows[e.RowIndex].Cells[nCellColumnNumber].Style.BackColor = Color.Red;
                        else
                        {
                            if (e.RowIndex % 2 == 1)
                                dgvControl.Rows[e.RowIndex].Cells[nCellColumnNumber].Style.BackColor = Color.LightCyan;
                            else
                                dgvControl.Rows[e.RowIndex].Cells[nCellColumnNumber].Style.BackColor = Color.White;
                        }
                    }
                }
                else
                {
                    if (dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                        dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                    else
                    {
                        if (dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null && ((DataGridViewCellTagObject)dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag).m_bErrorFlag == true ||
                            dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "N/A")
                            dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                        else
                        {
                            if (e.RowIndex % 2 == 1)
                                dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightCyan;
                            else
                                dgvControl.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                        }
                    }
                }
            }
        }

        private bool CheckSettingFlowRowsIdentical(DataGridView dgvControl, bool bSettingFlow = true, bool bAppointCell = false, DataGridViewCellEventArgs e = null)
        {
            bool bHasIdenticalFlag = false;

            foreach (DataGridViewRow dgvrControl in dgvControl.Rows)
            {
                bool bIdenticalFlag = false;
                foreach (DataGridViewRow dgvrCompareControl in dgvControl.Rows)
                {
                    if (dgvrCompareControl.Index == dgvrControl.Index)
                        continue;

                    if (dgvrControl.Cells[m_nPH1Index].Value.ToString() == dgvrCompareControl.Cells[m_nPH1Index].Value.ToString() &&
                        dgvrControl.Cells[m_nPH2Index].Value.ToString() == dgvrCompareControl.Cells[m_nPH2Index].Value.ToString())
                    {
                        if (bAppointCell == true)
                        {
                            if (e.RowIndex == dgvrControl.Index && (e.ColumnIndex == m_nPH1Index || e.ColumnIndex == m_nPH2Index))
                                bHasIdenticalFlag = true;
                        }
                        else
                            bHasIdenticalFlag = true;

                        bIdenticalFlag = true;
                    }
                }

                ((DataGridViewCellTagObject)dgvrControl.Cells[m_nPH1Index].Tag).m_bIdenticalFlag = bIdenticalFlag;
                ((DataGridViewCellTagObject)dgvrControl.Cells[4].Tag).m_bIdenticalFlag = bIdenticalFlag;
            }

            foreach (DataGridViewRow dgvrControl in dgvControl.Rows)
            {
                for (int nIndex = m_nPH1Index; nIndex <= m_nPH2Index; nIndex++)
                {
                    if (dgvrControl.Cells[nIndex].Value == null || dgvrControl.Cells[nIndex].Tag == null ||
                        ((DataGridViewCellTagObject)dgvrControl.Cells[nIndex].Tag).m_bIdenticalFlag == true ||
                        ((DataGridViewCellTagObject)dgvrControl.Cells[nIndex].Tag).m_bErrorFlag == true ||
                        dgvrControl.Cells[nIndex].Value.ToString() == "N/A")
                        dgvrControl.Cells[nIndex].Style.BackColor = Color.Red;
                    else
                    {
                        if (dgvrControl.Index % 2 == 1)
                        {
                            if (bSettingFlow == true)
                                dgvrControl.Cells[nIndex].Style.BackColor = Color.LightCyan;
                            else
                                dgvrControl.Cells[nIndex].Style.BackColor = Color.PaleTurquoise;
                        }
                        else
                            dgvrControl.Cells[nIndex].Style.BackColor = Color.White;
                    }
                }
            }

            return bHasIdenticalFlag;
        }

        private void dgvAllSelectFlow_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            m_bBeginEditFlag = true;
        }

        private void dgvSettingFlow_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            m_bBeginEditFlag = true;
        }

        private void dgvAllSelectFlow_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            Rectangle structRectangle = SetRectangle((DataGridView)sender);
            m_ckbxAllSelectHeader.Location = structRectangle.Location;
        }

        private void dgvSettingFlow_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            Rectangle structRectangle = SetRectangle((DataGridView)sender);
            m_ckbxSettingHeader.Location = structRectangle.Location;
        }

        private Rectangle SetRectangle(DataGridView dgvControl)
        {
            Rectangle structRectangle = dgvControl.GetCellDisplayRectangle(0, -1, true);
            structRectangle.X = structRectangle.Location.X + structRectangle.Width / 4 - 15;
            structRectangle.Y = structRectangle.Location.Y + (structRectangle.Height / 2 - 7);
            return structRectangle;
        }

        private void ShowMessage(string sDescription, string sTitle)
        {
            ShowMessageBox(sDescription, frmMessageBox.m_sError);
        }

        private void tbxFrequencyLB_TextChanged(object sender, EventArgs e)
        {
            TextBox tbxControl = sender as TextBox;

            if (tbxControl.Text.Trim() == "" || tbxControl.Text == "-")
                return;

            try
            {
                int nValue = Convert.ToInt32(tbxControl.Text.ToString());
            }
            catch
            {
                ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                tbxControl.Text = Convert.ToString(m_nFrequencyLB);
            }
        }

        private frmMessageBox.ReturnStatus ShowMessageBox(string sMessage, string sTitle = frmMessageBox.m_sWarining, string sConfirmButton = "OK")
        {
            frmMessageBox cfrmMessageBox = new frmMessageBox(sTitle, sConfirmButton);
            cfrmMessageBox.ShowMessage(sMessage);

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmMessageBox.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmMessageBox.Height / 2);

            cfrmMessageBox.StartPosition = FormStartPosition.Manual;
            cfrmMessageBox.Location = new Point(nLocationX, nLocationY);

            cfrmMessageBox.ShowDialog();

            return cfrmMessageBox.m_eReturnStatus;
        }
    }

    public class DataGridViewCellTagObject
    {
        public string m_sTagValue { get; set; }
        public bool m_bErrorFlag { get; set; }
        public bool m_bIdenticalFlag = false;

        public DataGridViewCellTagObject(string sTagValue, bool bErrorFlag)
        {
            m_sTagValue = sTagValue;
            m_bErrorFlag = bErrorFlag;
        }
    }
}


