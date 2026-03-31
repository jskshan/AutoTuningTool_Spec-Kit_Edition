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
    public partial class frmWarningMessage : Form
    {
        private string m_sMessage = "";

        public frmWarningMessage()
        {
            InitializeComponent();
        }

        public void LoadWarningMessage(string sMessage)
        {
            m_sMessage = sMessage;

            lblMessage.Text = m_sMessage;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lblMessage_Resize(object sender, EventArgs e)
        {
            lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        }
    }
}
