using System;
using System.Windows.Forms;
using System.IO;

namespace MPPPenAutoTuning
{
    public partial class frmFolderSelect : Form
    {
        private frmMain m_cfrmMain;

        private string m_sMainStep = "";
        private bool m_bSelectPathFlag = false;

        public frmFolderSelect(frmMain cfrmMain, MainTuningStep eMainStep)
        {
            InitializeComponent();

            MaximizeBox = false;

            m_cfrmMain = cfrmMain;

            m_sMainStep = StringConvert.m_dictMainStepMappingTable[eMainStep];
            m_bSelectPathFlag = false;

            if (m_cfrmMain.m_sDefaultFolderPath != "" && Directory.Exists(m_cfrmMain.m_sDefaultFolderPath) == true)
                tbxFolderPath.Text = m_cfrmMain.m_sDefaultFolderPath;
            else
                tbxFolderPath.Text = Application.StartupPath;

            tbxFolderPath.Select(tbxFolderPath.Text.Length, 0);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult drFolderSelectResult = DialogResult.No;
            FolderBrowserDialog fbdFolderPath = new FolderBrowserDialog();

            fbdFolderPath.Description = string.Format("Please Select \"{0}\" Step Folder", m_sMainStep);

            if (m_cfrmMain.m_sDefaultFolderPath != "" && Directory.Exists(m_cfrmMain.m_sDefaultFolderPath) == true)
                fbdFolderPath.SelectedPath = m_cfrmMain.m_sDefaultFolderPath;
            else
                fbdFolderPath.SelectedPath = Application.StartupPath;

            fbdFolderPath.ShowNewFolderButton = false;

            drFolderSelectResult = fbdFolderPath.ShowDialog();

            if (drFolderSelectResult == DialogResult.Cancel)
            {
            }
            else if (drFolderSelectResult == DialogResult.OK)
            {
                if (fbdFolderPath != null)
                {
                    tbxFolderPath.Text = fbdFolderPath.SelectedPath;
                    tbxFolderPath.Select(tbxFolderPath.Text.Length, 0);
                }
                else
                {
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_cfrmMain.m_sDefaultFolderPath = tbxFolderPath.Text;
            m_bSelectPathFlag = true;

            this.Close();
        }

        private void frmFolderSelect_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_bSelectPathFlag == false)
                m_cfrmMain.m_sDefaultFolderPath = "";
        }
    }
}
