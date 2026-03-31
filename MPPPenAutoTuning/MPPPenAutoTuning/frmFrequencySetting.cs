using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MPPPenAutoTuning
{
    public partial class frmFrequencySetting : Form
    {
        private frmMain m_cfrmMain;
        private string m_sFlowFileName;
        private string m_sFlowFilePath;
        private string[] m_sFlowFileName_Array;

        private CheckBox m_ckbxFrequencySetHeader = new CheckBox();

        private string m_sTotalRankFilePath = "";
        private string m_sTotalRankFileName = "";

        private SubTuningStep[] m_eSubStep_Array = new SubTuningStep[] 
        { 
            SubTuningStep.HOVER_1ST,
            SubTuningStep.PCHOVER_1ST,
            SubTuningStep.DIGIGAIN,
            SubTuningStep.TP_GAIN 
        };

        private List<FrequencyInfo> m_cTotalRankSet_List = new List<FrequencyInfo>();
        private List<FrequencyInfo> m_cSelectSet_List = new List<FrequencyInfo>();

        private enum TotalRankInfo : int
        {
            Rank = 0,
            PH1 = 1,
            PH2 = 2
        }

        private enum FlowFileInfo : int
        {
            Rank = 0,
            RobotType = 1,
            RecordType = 2,
            PH1 = 3,
            PH2 = 4
        }

        private enum DataGridViewCellInfo : int
        {
            CheckBox = 0,
            Rank = 1,
            PH1 = 2,
            PH2 = 3,
            Frequency = 4
        }

        private const int m_nCheckBoxIndex = (int)DataGridViewCellInfo.CheckBox;

        public class FrequencyInfo
        {
            public int m_nRank = -1;
            public int m_nPH1 = -1;
            public int m_nPH2 = -1;
            public double m_dFrequency = 0.0;
        }

        public frmFrequencySetting(frmMain cfrmMain)
        {
            InitializeComponent();

            m_cfrmMain = cfrmMain;
            m_sFlowFileName = m_cfrmMain.m_sSubTuningStepFileName_Array[(int)SubTuningStep.HOVER_1ST];
            m_sFlowFilePath = string.Format(@"{0}\{1}", m_cfrmMain.m_sFlowDirectoryPath, m_sFlowFileName);
            m_sTotalRankFilePath = m_cfrmMain.m_sTotalRankFilePath;
            m_sTotalRankFileName = m_cfrmMain.m_sTotalRankFileName;

            m_sFlowFileName_Array = new string[]
            {
                m_cfrmMain.m_sSubTuningStepFileName_Array[(int)SubTuningStep.HOVER_1ST],
                m_cfrmMain.m_sSubTuningStepFileName_Array[(int)SubTuningStep.PCHOVER_1ST],
                m_cfrmMain.m_sSubTuningStepFileName_Array[(int)SubTuningStep.DIGIGAIN],
                m_cfrmMain.m_sSubTuningStepFileName_Array[(int)SubTuningStep.TP_GAIN]
            };

            dgvFrequencySet.AlternatingRowsDefaultCellStyle.BackColor = Color.PaleTurquoise;    //奇數列顏色
            dgvFrequencySet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dgvFrequencySet_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            DataGridView dgvControl = (DataGridView)sender;
            Rectangle structRectangle = dgvControl.GetCellDisplayRectangle(0, -1, true);
            structRectangle.X = structRectangle.Location.X + structRectangle.Width / 4 - 15;
            structRectangle.Y = structRectangle.Location.Y + (structRectangle.Height / 2 - 7);

            m_ckbxFrequencySetHeader.Location = structRectangle.Location;
        }

        private void frmFrequencySetting_Load(object sender, EventArgs e)
        {
            if (File.Exists(m_sTotalRankFilePath) == false)
                ShowErrorMessage("No Frequency Rank Data. Please Run \"Noise\" Step First!!");
            else
                LoadFrequencySet();
        }

        public void LoadFrequencySet()
        {
            //建立個矩形，等下計算 CheckBox 嵌入 GridView 的位置
            Rectangle structRectangle = dgvFrequencySet.GetCellDisplayRectangle(0, -1, true);
            structRectangle.X = structRectangle.Location.X + structRectangle.Width / 4 - 15;
            structRectangle.Y = structRectangle.Location.Y + (structRectangle.Height / 2 - 7);

            m_ckbxFrequencySetHeader.Name = "Item";
            m_ckbxFrequencySetHeader.Size = new Size(15, 15);
            m_ckbxFrequencySetHeader.Location = structRectangle.Location;
            //全選要設定的事件
            m_ckbxFrequencySetHeader.Click += new EventHandler(checkboxHeader_CheckedChanged);

            //將 CheckBox 加入到 dataGridView
            dgvFrequencySet.Controls.Add(m_ckbxFrequencySetHeader);

            m_cTotalRankSet_List.Clear();
            m_cSelectSet_List.Clear();

            if (GetTotalRankSet() == false)
                return;

            if (File.Exists(m_sFlowFilePath) == true)
                GetSelectSet();

            SetFrequencySetDataGridViewData();

            ComputeSelectNumber();
        }

        private bool GetTotalRankSet()
        {
            TotalRankInfo[] eUsedInfo_Array = new TotalRankInfo[] 
            { 
                TotalRankInfo.Rank, 
                TotalRankInfo.PH1, 
                TotalRankInfo.PH2 
            };

            List<string> sTotalRankData_List = new List<string>(File.ReadAllLines(m_sTotalRankFilePath));

            if (sTotalRankData_List.Count <= 0)
            {
                ShowErrorMessage(string.Format("No Frequency in {0}", m_sTotalRankFileName));
                return false;
            }

            for (int nLineIndex = 0; nLineIndex < sTotalRankData_List.Count; nLineIndex++)
            {
                string[] sData_Array = sTotalRankData_List[nLineIndex].Split(',');
                FrequencyInfo cFrequencyInfo = new FrequencyInfo();

                for (int nInfoIndex = 0; nInfoIndex < eUsedInfo_Array.Length; nInfoIndex++)
                {
                    if (sData_Array.Length - 1 < nInfoIndex)
                    {
                        ShowErrorMessage(string.Format("Format Length Error in Line {0} in {1}", nLineIndex + 1, m_sTotalRankFileName));
                        return false;
                    }
                    else
                    {
                        if (ElanConvert.CheckIsInt(sData_Array[Convert.ToInt32(eUsedInfo_Array[nInfoIndex])]) == false)
                        {
                            ShowErrorMessage(string.Format("{0} Error in Line {1} in {2}", eUsedInfo_Array[nInfoIndex].ToString(), nLineIndex + 1, m_sTotalRankFileName));
                            return false;
                            
                        }

                        int nValue = Convert.ToInt32(sData_Array[Convert.ToInt32(eUsedInfo_Array[nInfoIndex])]);

                        switch (eUsedInfo_Array[nInfoIndex])
                        {
                            case TotalRankInfo.Rank:
                                cFrequencyInfo.m_nRank = nValue;
                                break;
                            case TotalRankInfo.PH1:
                                cFrequencyInfo.m_nPH1 = nValue;
                                break;
                            case TotalRankInfo.PH2:
                                cFrequencyInfo.m_nPH2 = nValue;
                                break;
                            default:
                                break;
                        }
                    }
                }

                cFrequencyInfo.m_dFrequency = ElanConvert.ComputeFrequnecyToDouble(cFrequencyInfo.m_nPH1, cFrequencyInfo.m_nPH2);
                m_cTotalRankSet_List.Add(cFrequencyInfo);
            }

            return true;
        }

        private void GetSelectSet()
        {
            FlowFileInfo[] eUsedInfo_Array = new FlowFileInfo[] 
            { 
                FlowFileInfo.Rank, 
                FlowFileInfo.PH1, 
                FlowFileInfo.PH2 
            };

            List<string> sSelectData_List = new List<string>(File.ReadAllLines(m_sFlowFilePath));

            for (int nLineIndex = 0; nLineIndex < sSelectData_List.Count; nLineIndex++)
            {
                string[] sData_Array = sSelectData_List[nLineIndex].Split(',');
                FrequencyInfo cFrequencyInfo = new FrequencyInfo();

                for (int nInfoIndex = 0; nInfoIndex < eUsedInfo_Array.Length; nInfoIndex++)
                {
                    if (ElanConvert.CheckIsInt(sData_Array[Convert.ToInt32(eUsedInfo_Array[nInfoIndex])]) == false)
                        continue;

                    int nValue = Convert.ToInt32(sData_Array[Convert.ToInt32(eUsedInfo_Array[nInfoIndex])]);

                    switch (eUsedInfo_Array[nInfoIndex])
                    {
                        case FlowFileInfo.Rank:
                            cFrequencyInfo.m_nRank = nValue;
                            break;
                        case FlowFileInfo.PH1:
                            cFrequencyInfo.m_nPH1 = nValue;
                            break;
                        case FlowFileInfo.PH2:
                            cFrequencyInfo.m_nPH2 = nValue;
                            break;
                        default:
                            break;
                    }
                }

                m_cSelectSet_List.Add(cFrequencyInfo);
            }
        }

        private void SetFrequencySetDataGridViewData()
        {
            int nCount = m_cTotalRankSet_List.Count;

            if (nCount > 0)
            {
                for (int nSetIndex = 0; nSetIndex < nCount; nSetIndex++)
                {
                    dgvFrequencySet.Rows.Add();
                    dgvFrequencySet.Rows[nSetIndex].HeaderCell.Value = (nSetIndex + 1).ToString();

                    for (int nInfoIndex = (int)DataGridViewCellInfo.Rank; nInfoIndex <= Enum.GetValues(typeof(DataGridViewCellInfo)).Cast<int>().Max(); nInfoIndex++)
                    {
                        string sValue = "";

                        switch(nInfoIndex)
                        {
                            case (int)DataGridViewCellInfo.Rank:
                                sValue = m_cTotalRankSet_List[nSetIndex].m_nRank.ToString();
                                break;
                            case (int)DataGridViewCellInfo.PH1:
                                sValue = m_cTotalRankSet_List[nSetIndex].m_nPH1.ToString();
                                break;
                            case (int)DataGridViewCellInfo.PH2:
                                sValue = m_cTotalRankSet_List[nSetIndex].m_nPH2.ToString();
                                break;
                            case (int)DataGridViewCellInfo.Frequency:
                                sValue = m_cTotalRankSet_List[nSetIndex].m_dFrequency.ToString("0.000");
                                break;
                            default:
                                break;
                        }

                        dgvFrequencySet.Rows[nSetIndex].Cells[nInfoIndex].Value = sValue;
                    }

                    int nRank = m_cTotalRankSet_List[nSetIndex].m_nRank;
                    int nPH1 = m_cTotalRankSet_List[nSetIndex].m_nPH1;
                    int nPH2 = m_cTotalRankSet_List[nSetIndex].m_nPH2;

                    dgvFrequencySet.Rows[nSetIndex].Cells[m_nCheckBoxIndex].Value = false;

                    foreach (FrequencyInfo cFrequencyInfo in m_cSelectSet_List)
                    {
                        if (cFrequencyInfo.m_nRank == nRank &&
                            cFrequencyInfo.m_nPH1 == nPH1 &&
                            cFrequencyInfo.m_nPH2 == nPH2)
                        {
                            dgvFrequencySet.Rows[nSetIndex].Cells[m_nCheckBoxIndex].Value = true;
                            break;
                        }
                    }
                }
            }
        }

        private void ComputeSelectNumber()
        {
            int nSelectCount = 0;

            foreach (DataGridViewRow dgvrControl in dgvFrequencySet.Rows)
            {
                if ((bool)dgvrControl.Cells[m_nCheckBoxIndex].Value == true)
                    nSelectCount++;
            }

            lblSelectNumber.Text = string.Format("Select Number : {0}", nSelectCount);
        }

        private void checkboxHeader_CheckedChanged(object sender, EventArgs e)
        {
            dgvFrequencySet.EndEdit();

            foreach (DataGridViewRow dgvrControl in dgvFrequencySet.Rows)
                dgvrControl.Cells[m_nCheckBoxIndex].Value = ((CheckBox)dgvFrequencySet.Controls.Find("Item", true)[0]).Checked;

            ComputeSelectNumber();
        }

        private void dgvFrequencySet_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == m_nCheckBoxIndex)
            {
                dgvFrequencySet.EndEdit();
                ComputeSelectNumber();
            }
        }

        private void dgvFrequencySet_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == m_nCheckBoxIndex)
            {
                dgvFrequencySet.EndEdit();
                ComputeSelectNumber();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (int nStepIndex = 0; nStepIndex < m_eSubStep_Array.Length; nStepIndex++)
            {
                string sFlowFilePath = string.Format(@"{0}\{1}", m_cfrmMain.m_sFlowDirectoryPath, m_cfrmMain.m_sSubTuningStepFileName_Array[(int)m_eSubStep_Array[nStepIndex]]);

                FileStream fs = new FileStream(sFlowFilePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                int nSetIndex = 0;

                try
                {
                    foreach (DataGridViewRow dgvrControl in dgvFrequencySet.Rows)
                    {
                        if ((bool)dgvrControl.Cells[m_nCheckBoxIndex].Value == true)
                        {
                            sw.Write(string.Format("{0},", m_cTotalRankSet_List[nSetIndex].m_nRank.ToString()));

                            if (m_eSubStep_Array[nStepIndex] == SubTuningStep.DIGIGAIN)
                            {
                                sw.Write(string.Format("{0},", FlowRobot.TOUCHLINE.ToString()));
                                sw.Write(string.Format("{0},", FlowRecord.DIGIGAIN.ToString()));
                            }
                            else if (m_eSubStep_Array[nStepIndex] == SubTuningStep.TP_GAIN)
                            {
                                sw.Write(string.Format("{0},", FlowRobot.TOUCHLINE.ToString()));
                                sw.Write(string.Format("{0},", FlowRecord.TP_GAIN.ToString()));
                            }
                            else
                            {
                                sw.Write(string.Format("{0},", FlowRobot.HOVERLINE.ToString()));
                                sw.Write(string.Format("{0},", FlowRecord.TRX.ToString()));
                            }

                            sw.Write(string.Format("{0},", m_cTotalRankSet_List[nSetIndex].m_nPH1.ToString().PadLeft(2, '0')));
                            sw.WriteLine(string.Format("{0}", m_cTotalRankSet_List[nSetIndex].m_nPH2.ToString().PadLeft(2, '0')));
                        }

                        nSetIndex++;
                    }
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }

            ShowMessageBox("Save Complete!!", frmMessageBox.m_sMessage);
        }

        private void ShowErrorMessage(string sMessage)
        {
            ShowMessageBox(sMessage, frmMessageBox.m_sError);
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
}
