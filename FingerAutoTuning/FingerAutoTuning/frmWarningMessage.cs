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
    public partial class frmWarningMessage : Form
    {
        string sMsgStr = "";

        public frmWarningMessage()
        {
            InitializeComponent();
        }

        public void WarningMessageLoad(string MsgStr)
        {
            sMsgStr = MsgStr;

            lblMessage.Text = sMsgStr;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblMessage_Resize(object sender, EventArgs e)
        {
            lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        }
    }
}
