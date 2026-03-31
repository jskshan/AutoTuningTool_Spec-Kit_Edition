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
    public partial class frmPatternMenu : Form
    {
        protected frmPattern m_frmParent = null;

        protected bool m_bRetryEnable = true;
        protected bool m_bFormClose = false;

        protected bool m_bMoveFunctionWindow = false;
        protected Point m_ptPreMoveLocation = new Point();

        public frmPatternMenu(frmPattern frmParent, bool bRetryEnable = true)
        {
            InitializeComponent();

            m_frmParent = frmParent;
            m_bRetryEnable = bRetryEnable;

            if (m_bRetryEnable == true)
                RetryBtn.Enabled = true;
            else
                RetryBtn.Enabled = false;
        }

        public void SetCaption(string sCaption)
        {
            LabelFontResize(CollectStepPatternLbl, sCaption);
        }

        public void LabelFontResize(Label Lbl, string sStr)
        {
            int LblWidth = Lbl.Width;
            float fSize = 20;
            FontFamily ff = new FontFamily(Lbl.Font.Name);

            Graphics gh = this.CreateGraphics();
            SizeF sf = gh.MeasureString(sStr, Lbl.Font);
            while (sf.Width > LblWidth)
            {
                fSize -= 0.01F;
                Lbl.Font = new Font(ff, fSize, FontStyle.Bold, GraphicsUnit.World);
                sf = gh.MeasureString(sStr, Lbl.Font);
            }

            Lbl.Text = sStr;
        }

        public void DisplayStatus(string sStatusStr)
        {
            if (m_bFormClose == true)
                return;

            if (StatusLbl.InvokeRequired)
            {
                StatusLbl.Invoke((MethodInvoker)delegate
                {
                    StatusLbl.Text = sStatusStr;
                });
            }
            else
                StatusLbl.Text = sStatusStr;
        }

        public void DisplayTimeAndReportNumber(double dTime, bool bDisplayTime = false)
        {
            if (m_bFormClose == true)
                return;

            int nReportNumber = m_frmParent.m_byteData_List.Count;

            string sMessage = "";

            if (bDisplayTime == true)
                sMessage = string.Format("Time : {0:0.000}s", dTime) + Environment.NewLine;

            sMessage += string.Format("ReportNumber : {0}", nReportNumber);

            if (ReportNumberLbl.InvokeRequired)
            {
                try
                {
                    if (ReportNumberLbl.IsDisposed == false)
                    {
                        ReportNumberLbl.Invoke((MethodInvoker)delegate
                        {
                            ReportNumberLbl.Text = sMessage;
                        });
                    }
                }
                catch
                { }
            }
            else
            {
                try
                {
                    if (ReportNumberLbl.IsDisposed == false)
                        ReportNumberLbl.Text = sMessage;
                }
                catch
                { }
            }
        }

        private void OnfrmClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            m_bFormClose = true;
        }

        /*
        private void OnbtnSaveClick(object sender, EventArgs e)
        {
            if (m_frmParent != null)
                m_frmParent.SaveLog();
        }
        */

        private void OnbtnRetryClick(object sender, EventArgs e)
        {
            m_frmParent.m_bTestRetry = true;

            if (m_frmParent != null)
                m_frmParent.StartTestProcedure();
        }

        /*
        private void OnbtnOptionClick(object sender, EventArgs e)
        {
            if (m_frmParent != null)
                m_frmParent.ShowOption();
        }
        */

        private void OnbtnExitClick(object sender, EventArgs e)
        {
            if (m_frmParent.m_bSaveLogComplete == false)
            {
                frmMain.m_bCollectFlowError = true;
                frmMain.m_sCollectFlowErrorMessage = "Collect Flow Not Complete";
            }

            if (m_frmParent != null)
                m_frmParent.Close();

            Close();
        }

        public void ExitFlow()
        {
            if (m_frmParent != null)
            {
                if (m_frmParent.InvokeRequired)
                {
                    m_frmParent.Invoke((MethodInvoker)delegate
                    {
                        m_frmParent.Close();
                    });
                }
                else
                    m_frmParent.Close();
            }

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        Close();
                    });
                }
                catch
                { }
            }
            else
            {
                try
                {
                    Close();
                }
                catch
                { }
            }
        }

        private void OnfrmLoad(object sender, EventArgs e)
        {
            SetupResultInfoGridView();
        }

        /// <summary>
        /// Show the result on the UI
        /// </summary>
        /// <param name="ResultItems"></param>
        public void ShowResultOnUI(ResultItem[] ResultItems)
        {
            if (ResultItems == null)
                return;

            ResultInfoGridView.Visible = true;
            ResultInfoGridView.Rows.Clear();
            ResultInfoGridView.RowCount = ResultItems.Length;
            int nHeight = 0;
            for (int i = 0; i < ResultItems.Length; i++)
            {
                ResultInfoGridView.Rows[i].Cells[0].Value = ResultItems[i].m_sTitle;
                ResultInfoGridView.Rows[i].Cells[1].Value = ResultItems[i].m_sContent;
                ResultInfoGridView.Rows[i].ReadOnly = true;
                nHeight += ResultInfoGridView.Rows[i].Height;
            }

            ResultInfoGridView.Height = nHeight + 2;
            ResultInfoGridView.CurrentCell = null;
            ResultInfoGridView.Update();
            Height = ResultInfoGridView.Bottom;
        }

        public void HideInfoGridView()
        {
            ResultInfoGridView.Visible = false;
            Height = ResultInfoGridView.Top - 2;
        }

        /// <summary>
        /// Set up the gridview properties
        /// </summary>
        private void SetupResultInfoGridView()
        {
            float[] ColumnRatioSet = { 0.5f, 0.5f };

            ResultInfoGridView.Location = new Point(panelTool.Left, panelTool.Bottom + 2);
            ResultInfoGridView.Visible = false;
            ResultInfoGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            ResultInfoGridView.MultiSelect = false;
            ResultInfoGridView.AutoGenerateColumns = true;
            ResultInfoGridView.AllowUserToResizeRows = false;
            ResultInfoGridView.AllowUserToResizeColumns = false;
            ResultInfoGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            //Hide the header
            ResultInfoGridView.RowHeadersVisible = false;
            ResultInfoGridView.ColumnHeadersVisible = false;
            ResultInfoGridView.ColumnCount = 2;

            //Set Column Header and Properties
            for (int i = 0; i < ResultInfoGridView.ColumnCount; i++)
            {
                ResultInfoGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                ResultInfoGridView.Columns[i].Width = (int)(ResultInfoGridView.Width * ColumnRatioSet[i]);
            }
        }

        private void OnlbTestNameMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bMoveFunctionWindow = true;
        }

        private void OnlbTestNameMouseMove(object sender, MouseEventArgs e)
        {
            if (m_bMoveFunctionWindow == true)
            {
                Point ptCurOnScreen = PointToScreen(new Point(e.X, e.Y));
                Console.WriteLine("OrgX = {0}, OrgY = {1}, ScreenX = {2}, ScreenY = {3}", e.X, e.Y, ptCurOnScreen.X, ptCurOnScreen.Y);

                if (m_ptPreMoveLocation.X > 0 && m_ptPreMoveLocation.Y > 0)
                {
                    int nXMotion = ptCurOnScreen.X - m_ptPreMoveLocation.X;
                    int nYMotion = ptCurOnScreen.Y - m_ptPreMoveLocation.Y;

                    Left += nXMotion;
                    Top += nYMotion;
                }

                m_ptPreMoveLocation = new Point(ptCurOnScreen.X, ptCurOnScreen.Y);
            }
        }

        private void OnlbTestNameMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bMoveFunctionWindow = false;
                m_ptPreMoveLocation = new Point(-1, -1);
            }
        }
    }
}
