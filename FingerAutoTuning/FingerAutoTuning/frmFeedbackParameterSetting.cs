using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FingerAutoTuning
{
    public partial class frmFeedbackParameterSetting : Form
    {
        private frmFeedback m_cfrmFeedback;

        public frmFeedbackParameterSetting(frmFeedback cfrmFeedback)
        {
            InitializeComponent();

            m_cfrmFeedback = cfrmFeedback;
        }

        private void frmFeedbackParameterSetting_Load(object sender, EventArgs e)
        {
            tbxIPAddress.Text = m_cfrmFeedback.m_sIPAddress;
            tbxPort.Text = Convert.ToString(m_cfrmFeedback.m_nPort);
            tbxRouteName.Text = m_cfrmFeedback.m_sRouteName;
        }

        private void frmFeedbackParameterSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_cfrmFeedback.m_sIPAddress = tbxIPAddress.Text;
            m_cfrmFeedback.m_nPort = Convert.ToInt32(tbxPort.Text);
            m_cfrmFeedback.m_sRouteName = tbxRouteName.Text;
        }
    }
}
