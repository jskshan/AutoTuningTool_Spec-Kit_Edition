using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MPPPenAutoTuning
{
    public partial class frmDFT_NUMAndCoeffConverter : Form
    {
        private frmMain m_cfrmMain;

        private int m_nPH1 = 10;
        private int m_nPH2 = 42;
        private int m_nSKIP_NUM = 12;

        public frmDFT_NUMAndCoeffConverter(frmMain cfrmMain)
        {
            InitializeComponent();

            m_cfrmMain = cfrmMain;

            tbxPH1.Text = Convert.ToString(m_nPH1);
            tbxPH2.Text = Convert.ToString(m_nPH2);
            tbxSKIP_NUM.Text = Convert.ToString(m_nSKIP_NUM);
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (CheckValueIsValid(ref m_nPH1, tbxPH1, SpecificText.m_sPH1) == false)
                return;

            if (CheckValueIsValid(ref m_nPH2, tbxPH2, SpecificText.m_sPH2) == false)
                return;

            if (CheckValueIsValid(ref m_nSKIP_NUM, tbxSKIP_NUM, SpecificText.m_sSKIP_NUM) == false)
                return;

            //Compute & Check DFT_NUM & Coefficient
            ComputeDFT_NUMAndCoefficient cComputeDFT_NUMAndCoeff = new ComputeDFT_NUMAndCoefficient();
            cComputeDFT_NUMAndCoeff.WriteDFT_NUMAndCoefficientFile(m_nPH1, m_nPH2, m_nSKIP_NUM);

            ShowMessageBox("Convert Complete!!", frmMessageBox.m_sMessage);
        }

        private bool CheckValueIsValid(ref int nValue, TextBox tbxControl, string sValueName)
        {
            try
            {
                nValue = Convert.ToInt32(tbxControl.Text);
            }
            catch
            {
                ShowMessageBox(string.Format("{0} Value Error", sValueName), frmMessageBox.m_sError);
                return false;
            }

            return true;
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
