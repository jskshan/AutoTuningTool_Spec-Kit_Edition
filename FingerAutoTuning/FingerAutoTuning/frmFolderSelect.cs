using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FingerAutoTuning
{
    public partial class frmFolderSelect : Form
    {
        private frmMain m_cfrmMain = null;
        private string sMainStep = "";
        private bool bSelectPath = false;

        public frmFolderSelect(frmMain cfrmMain)
        {
            InitializeComponent();

            m_cfrmMain = cfrmMain;
        }

        public void FolderSelectLoad(MainStep CurStep)
        {
            bSelectPath = false;
            MaximizeBox = false;

            sMainStep = StringConvert.m_dictMainStepMappingTable[CurStep];

            if (m_cfrmMain.m_sDefaultFolderPath != "" && Directory.Exists(m_cfrmMain.m_sDefaultFolderPath) == true)
                SelectPathTbx.Text = m_cfrmMain.m_sDefaultFolderPath;
            else
                SelectPathTbx.Text = Application.StartupPath;

            SelectPathTbx.Select(SelectPathTbx.Text.Length, 0);
        }

        private void SelectBtn_Click(object sender, EventArgs e)
        {
            DialogResult Folder_PathResult = DialogResult.No;
            FolderBrowserDialog Folder_Path = new FolderBrowserDialog();

            Folder_Path.Description = string.Format("Please Select \"{0}\" Step Folder", sMainStep);

            if (m_cfrmMain.m_sDefaultFolderPath != "" && Directory.Exists(m_cfrmMain.m_sDefaultFolderPath) == true)
                Folder_Path.SelectedPath = m_cfrmMain.m_sDefaultFolderPath;
            else
                Folder_Path.SelectedPath = Application.StartupPath;

            Folder_Path.ShowNewFolderButton = false;

            Folder_PathResult = Folder_Path.ShowDialog();

            if (Folder_PathResult == DialogResult.Cancel)
            {
            }
            else if (Folder_PathResult == DialogResult.OK)
            {
                if (Folder_Path != null)
                {
                    SelectPathTbx.Text = Folder_Path.SelectedPath;
                    SelectPathTbx.Select(SelectPathTbx.Text.Length, 0);
                }
                else
                {
                }
            }
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            m_cfrmMain.m_sDefaultFolderPath = SelectPathTbx.Text;
            bSelectPath = true;

            this.Close();
        }

        private void frmFolderSelect_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (bSelectPath == false)
                m_cfrmMain.m_sDefaultFolderPath = "";
        }
    }
}
