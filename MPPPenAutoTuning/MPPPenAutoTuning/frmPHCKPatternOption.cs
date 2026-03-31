using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class frmPHCKPatternOption : Form
    {
        private frmMain m_cfrmMain;

        private ParamEnvironment m_cAPSetting = null;
        private DataTable m_datatableScreenSizeTable = new DataTable("ScreenSizeItem");

        private bool m_bPreviewSettingFlag = false;

        private byte[] m_byteEDIDData_Array = null;
        private EDIDInfo m_cEDIDInformation = null;

        public frmPHCKPatternOption(frmMain cfrmMain, ParamEnvironment cAPSetting, byte[] byteEDIDData_Array, EDIDInfo cEDIDInformation, bool bPreviewSettingFlag = false)
        {
            InitializeComponent();

            m_cfrmMain = cfrmMain;

            m_datatableScreenSizeTable.Columns.Add("Name");
            m_datatableScreenSizeTable.Columns.Add("Width");
            m_datatableScreenSizeTable.Columns.Add("Height");

            m_cAPSetting = cAPSetting;
            m_byteEDIDData_Array = byteEDIDData_Array;
            m_cEDIDInformation = cEDIDInformation;
            m_bPreviewSettingFlag = bPreviewSettingFlag;
        }

        private void OnFrmLoad(object sender, EventArgs e)
        {
            string sScreenSizeListPath = string.Format(@"{0}\{1}\ini\ScreenList.xml", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
            m_datatableScreenSizeTable.ReadXml(sScreenSizeListPath);

            if (m_byteEDIDData_Array == null || m_byteEDIDData_Array.Length == 0)
            {
                DataRow[] drScreenSize_Array = m_datatableScreenSizeTable.Select("Name = 'AutoSet'");

                for (int nSizeIndex = 0; nSizeIndex < drScreenSize_Array.Length; nSizeIndex++)
                {
                    int nRowIndex = m_datatableScreenSizeTable.Rows.IndexOf(drScreenSize_Array[nSizeIndex]);
                    m_datatableScreenSizeTable.Rows.RemoveAt(nRowIndex);
                    drScreenSize_Array = m_datatableScreenSizeTable.Select("Name = 'AutoSet'");
                    nSizeIndex = 0;
                }
            }

            foreach (DataRow drScreenSize in m_datatableScreenSizeTable.Rows)
            {
                if (drScreenSize.ItemArray[0].ToString() == "AutoSet")
                {
                    DataRow[] drScreenSize_Array = m_datatableScreenSizeTable.Select("Name = 'AutoSet'");

                    for (int nSizeIndex = 0; nSizeIndex < drScreenSize_Array.Length; nSizeIndex++)
                    {
                        int nRowIndex = m_datatableScreenSizeTable.Rows.IndexOf(drScreenSize_Array[nSizeIndex]);
                        m_datatableScreenSizeTable.Rows[nRowIndex]["Name"] = string.Format("AutoSet ({0}\")", m_cEDIDInformation.dScreenSize);
                        m_datatableScreenSizeTable.Rows[nRowIndex]["Width"] = m_cEDIDInformation.nEDIDInfo_Width;
                        m_datatableScreenSizeTable.Rows[nRowIndex]["Height"] = m_cEDIDInformation.nEDIDInfo_Height;
                        cbScreenSize.Items.Add(drScreenSize.ItemArray[0]);
                    }
                }
                else
                    cbScreenSize.Items.Add(drScreenSize.ItemArray[0]);
            }

            cbScreenSize.SelectedIndex = m_cAPSetting.m_nScreenIdx;

            //tbWidth.Text = m_cAPSetting.m_fScreenX.ToString();
            //tbHeight.Text = m_cAPSetting.m_fScreenY.ToString();
            tbWidth.Text = (string)m_datatableScreenSizeTable.Rows[cbScreenSize.SelectedIndex].ItemArray[1];
            tbHeight.Text = (string)m_datatableScreenSizeTable.Rows[cbScreenSize.SelectedIndex].ItemArray[2];

            plLeftRectColor.BackColor = Color.FromArgb(m_cAPSetting.m_nLeftColor);
            plRightRectColor.BackColor = Color.FromArgb(m_cAPSetting.m_nRightColor);
            plGrayLineColor.BackColor = Color.FromArgb(m_cAPSetting.m_nGrayLineColor);
        }

        private void OnOKClick(object sender, EventArgs e)
        {
            m_cAPSetting.m_nScreenIdx = cbScreenSize.SelectedIndex;
            float.TryParse(tbWidth.Text, out m_cAPSetting.m_fScreenX);
            float.TryParse(tbHeight.Text, out m_cAPSetting.m_fScreenY);

            m_cAPSetting.m_nLeftColor = plLeftRectColor.BackColor.ToArgb();
            m_cAPSetting.m_nRightColor = plRightRectColor.BackColor.ToArgb();
            m_cAPSetting.m_nGrayLineColor = plGrayLineColor.BackColor.ToArgb();

            if (m_bPreviewSettingFlag == false)
            {
                string sIniPath = string.Format(@"{0}\ini\Setting.ini", Application.StartupPath);
                m_cAPSetting.Save(sIniPath);
            }

            ParamAutoTuning.m_nScreenIndex = m_cAPSetting.m_nScreenIdx;
            ParamAutoTuning.m_dScreenWidth = Math.Round(m_cAPSetting.m_fScreenX, 2, MidpointRounding.AwayFromZero);
            ParamAutoTuning.m_dScreenHeight = Math.Round(m_cAPSetting.m_fScreenY, 2, MidpointRounding.AwayFromZero);
            ParamAutoTuning.m_nLeftColor = m_cAPSetting.m_nLeftColor;
            ParamAutoTuning.m_nRightColor = m_cAPSetting.m_nRightColor;
            ParamAutoTuning.m_nGrayLineColor = m_cAPSetting.m_nGrayLineColor;

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();

            if (cbScreenSize.SelectedItem.ToString().Contains("AutoSet") == true)
                m_cfrmMain.m_sScreenSize = string.Format("{0}\"", Convert.ToString(m_cEDIDInformation.dScreenSize));
            else
                m_cfrmMain.m_sScreenSize = cbScreenSize.SelectedItem.ToString();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void OnplLeftRectColorClick(object sender, EventArgs e)
        {
            plLeftRectColor.BackColor = SetColor(plLeftRectColor.BackColor);
        }

        private void OnpRightRectColorClick(object sender, EventArgs e)
        {
            plRightRectColor.BackColor = SetColor(plRightRectColor.BackColor);
        }

        private Color SetColor(Color colorCurrentColor)
        {
            ColorDialog cDialog = new ColorDialog();
            cDialog.FullOpen = true;
            cDialog.Color = colorCurrentColor;

            if (cDialog.ShowDialog() == DialogResult.OK)
                return cDialog.Color;

            return colorCurrentColor;
        }

        private void cbScreenSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_datatableScreenSizeTable.Rows.Count <= 0)
                return;

            tbWidth.Text = (string)m_datatableScreenSizeTable.Rows[cbScreenSize.SelectedIndex].ItemArray[1];
            tbHeight.Text = (string)m_datatableScreenSizeTable.Rows[cbScreenSize.SelectedIndex].ItemArray[2];
        }
    }
}
