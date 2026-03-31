using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MPPPenAutoTuning
{
    public partial class frmWarningMessage_2Selection : Form
    {
        private string m_sMessage = "";

        public frmWarningMessage_2Selection()
        {
            InitializeComponent();
        }

        public void Load_Warning_Message(string sMessage)
        {
            m_sMessage = sMessage;

            lblMessage.Text = m_sMessage;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            Close();
        }

        private void lblMessage_Resize(object sender, EventArgs e)
        {
            lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        }
    }
}
