using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    public partial class frmParamSetting : Form
    {
        private frmMain m_cfrmParent = null;

        protected ParamTestItemMgr m_TestItemMgr;
        protected Control m_ctrlActive = null;

        //private string sParamFilePath = string.Format(@"{0}\{1}\ini\Setting.dat", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
        private string sFormatFilePath = string.Format(@"{0}\{1}\ini\Format.dat", Application.StartupPath, frmMain.m_sAPMainDirectoryName);

        public frmParamSetting(frmMain cfrmParent)
        {
            InitializeComponent();

            m_cfrmParent = cfrmParent;

            AddPropSettingCtrl();
        }

        private bool AddPropSettingCtrl()
        {
            ParamTestItem CurPropItem = null;
            ctrlParamPage ctrlParamSetting = new ctrlParamPage(AccessMode.Guest, m_cfrmParent.m_cFlowStep_List);

            //Create new Noise Sweep Properties
            //m_Prop = new NoiseSweepProperties();

            //Load the properties and get properties item
            m_TestItemMgr = new ParamTestItemMgr(m_cfrmParent, m_cfrmParent.m_sSettingFilePath, m_cfrmParent.m_sDefaultFilePath);
            m_TestItemMgr.Load(sFormatFilePath);
            CurPropItem = m_TestItemMgr.GetTestItem("Parameter Setting");

            if (CurPropItem == null)
            {
                Console.WriteLine("[ERROR] Get Test Item Fail!!");
                return false;
            }

            //Create new properties setting page
            ctrlParamSetting.Left = 0;
            ctrlParamSetting.Top = 0;
            ctrlParamSetting.Width = plContent.Width;
            ctrlParamSetting.Height = plContent.Height;
            ctrlParamSetting.Visible = true;

            ctrlParamSetting.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            ctrlParamSetting.SetupGridView(2, plContent.Width);
            ctrlParamSetting.SetData(CurPropItem, 0);

            plContent.Controls.Add(ctrlParamSetting);
            m_ctrlActive = ctrlParamSetting;

            return true;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            //Update the data and store the data to file
            ((ctrlParamPage)m_ctrlActive).UpdateData();
            if (m_TestItemMgr != null)
                m_TestItemMgr.Save(sFormatFilePath);

            ParamFingerAutoTuning.LoadParam(m_cfrmParent.m_sSettingFilePath, m_cfrmParent.m_sDefaultFilePath);
            ParamFingerAutoTuning.SetScreenSize();

            MessageBox.Show("Save Complete");
        }
    }
}
