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
    public partial class frmMessageBox : Form
    {
        private string m_sTitle = "";
        private string m_sConfirmButton = "";

        public const string m_sWarining = "Warining";
        public const string m_sError = "Error";
        public const string m_sMessage = "Message";

        public enum ReturnStatus
        {
            Confirm,
            Close
        }

        public ReturnStatus m_eReturnStatus = ReturnStatus.Close;

        public frmMessageBox(string sTitle, string sConfirmButton)
        {
            InitializeComponent();

            m_sTitle = sTitle;
            m_sConfirmButton = sConfirmButton;

            this.Text = m_sTitle;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            m_eReturnStatus = ReturnStatus.Confirm;
            this.Close();
        }

        public void ShowMessage(string sMessage)
        {
            lblMessage.Text = sMessage;

            if (m_sTitle == m_sError)
                lblMessage.ForeColor = Color.Red;
        }
    }
}
