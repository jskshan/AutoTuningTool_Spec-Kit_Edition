using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FingerAutoTuningParameter;
using System.Reflection;

namespace FingerAutoTuning
{
    public partial class ctrlMADataGridView : UserControl
    {
        private frmMultiAnalysis.StepEnum m_eCurentStep;
        private string m_sFilePath;
        private string m_sAnalysisErrorMessage = "";

        private DataTable m_cFRDataTable = null;

        int m_nACFRDataGridViewTotalWidth = 0;

        public ctrlMADataGridView(frmMultiAnalysis.StepEnum eCurrentStep, string sFilePath)
        {
            InitializeComponent();

            m_eCurentStep = eCurrentStep;
            m_sFilePath = sFilePath;

            dgvRank.MakeDoubleBuffered(true);
        }

        private bool SetDataTable()
        {
            try
            {
                m_cFRDataTable = StringConvert.ConvertCsvToDataTable(m_sFilePath, "Frequency Rank:");
            }
            catch
            {
                m_sAnalysisErrorMessage = "Get File Data Error!";
                return false;
            }

            m_cFRDataTable.Columns.Add("ColorGradient");

            return true;
        }

        private void ctrlMADataGridView_Load(object sender, EventArgs e)
        {
            if (SetDataTable() == false)
            {
                MessageBox.Show(m_sAnalysisErrorMessage);
                return;
            }

            OutputDataGridView();
        }

        private void OutputDataGridView()
        {
            while (dgvRank.Rows.Count != 0)
                dgvRank.Rows.RemoveAt(0);

            dgvRank.Columns.Clear();
            dgvRank.DataSource = m_cFRDataTable;
            dgvRank.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvRank.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10);
            dgvRank.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRank.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRank.RowsDefaultCellStyle.Font = new Font("Times New Roman", 12);
            dgvRank.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;

            //Grid Back ground Color
            dgvRank.BackgroundColor = Color.LightSteelBlue;

            //Grid Back Color
            dgvRank.RowsDefaultCellStyle.BackColor = Color.AliceBlue;

            //GridColumnStylesCollection Alternate Rows Backcolr
            dgvRank.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

            // Auto generated here set to tru or false.
            //dgvRank.AutoGenerateColumns = false;
            //ShanuDGV.DefaultCellStyle.Font = new Font("Verdana", 10.25f, FontStyle.Regular);
            //ShanuDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 11, FontStyle.Regular);

            //Column Header back Color
            dgvRank.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            //
            dgvRank.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //header Visisble
            //dgvRank.EnableHeadersVisualStyles = false;

            // Enable the row header
            //dgvRank.RowHeadersVisible = false;

            // to Hide the Last Empty row here we use false.
            dgvRank.AllowUserToAddRows = false;

            if (m_eCurentStep == frmMultiAnalysis.StepEnum.FrequencyRank_Phase2)
            {
                dgvRank.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                for (int nIndex = 0; nIndex < dgvRank.Columns.Count; nIndex++)
                    dgvRank.Columns[nIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            if (m_eCurentStep == frmMultiAnalysis.StepEnum.AC_FrequencyRank)
            {
                dgvRank.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                for (int nIndex = 0; nIndex < dgvRank.Columns.Count; nIndex++)
                {
                    dgvRank.Columns[nIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                for (int nIndex = 0; nIndex < dgvRank.Columns.Count; nIndex++)
                {
                    dgvRank.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    int nColumnWidth = dgvRank.Columns[nIndex].Width;
                    dgvRank.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvRank.Columns[nIndex].Width = nColumnWidth;
                }

                m_nACFRDataGridViewTotalWidth = 0;

                for (int nIndex = 0; nIndex < dgvRank.Columns.Count; nIndex++)
                {
                    int nColumnWidth = dgvRank.Columns[nIndex].Width;
                    m_nACFRDataGridViewTotalWidth += nColumnWidth;
                }

                if (dgvRank.Width > m_nACFRDataGridViewTotalWidth)
                {
                    for (int nIndex = 0; nIndex < dgvRank.Columns.Count; nIndex++)
                    {
                        dgvRank.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    dgvRank.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                else
                {
                    for (int nIndex = 0; nIndex < dgvRank.Columns.Count; nIndex++)
                    {
                        dgvRank.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        int nColumnWidth = dgvRank.Columns[nIndex].Width;
                        dgvRank.Columns[nIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        dgvRank.Columns[nIndex].Width = nColumnWidth;
                    }
                }
            }

            dgvRank.AutoResizeColumns();
        }

        private void dgvRank_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (m_eCurentStep == frmMultiAnalysis.StepEnum.FrequencyRank_Phase2)
            {
                dgv.Columns["ColorGradient"].Width = 15;
                dgv.Columns["ColorGradient"].HeaderText = "";

                DataGridViewRow dgvr = dgv.Rows[e.RowIndex];    // get you required index
                                                                // check the cell value under your specific column and then you can toggle your colors

                if (e.RowIndex < ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber)
                    dgvr.DefaultCellStyle.BackColor = Color.LightPink;

                double dPercentageValue = Convert.ToDouble(dgvr.Cells["Reference Value"].Value);

                double dRatio = dPercentageValue / 100.0;

                int nRedValue = 0;
                int nGreenValue = 0;
                int nBlueValue = 0;

                if (dRatio >= 0.5)
                {
                    nRedValue = (int)Math.Round(((-255 * (dRatio - 0.5)) / 0.5) + 255, 0, MidpointRounding.AwayFromZero);
                    nGreenValue = (int)Math.Round(((-127 * (dRatio - 0.5)) / 0.5) + 255, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    nRedValue = 255;
                    nGreenValue = (int)Math.Round((255 * dRatio) / 0.5, 0, MidpointRounding.AwayFromZero);
                }

                dgvr.Cells["ColorGradient"].Style.BackColor = Color.FromArgb(nRedValue, nGreenValue, nBlueValue);
            }
            else if (m_eCurentStep == frmMultiAnalysis.StepEnum.AC_FrequencyRank)
            {
                double dMaxValue = 0.0;

                for (int nIndex = 0; nIndex < dgv.Rows.Count; nIndex++)
                {
                    double dValue = Convert.ToDouble(dgv.Rows[nIndex].Cells["Composite SNR(dB)"].Value);

                    if (nIndex == 0)
                        dMaxValue = dValue;
                    else if (dValue > dMaxValue)
                        dMaxValue = dValue;

                    double dACSNRValue = Convert.ToDouble(dgv.Rows[nIndex].Cells["AC SNR(dB)"].Value);

                    if (dACSNRValue < 0)
                        dgv.Rows[nIndex].Cells["AC SNR(dB)"].Style.BackColor = Color.Red;

                    double dLCMSNRValue = Convert.ToDouble(dgv.Rows[nIndex].Cells["LCM SNR(dB)"].Value);

                    if (dLCMSNRValue < 0)
                        dgv.Rows[nIndex].Cells["LCM SNR(dB)"].Style.BackColor = Color.Red;
                }

                dgv.Columns["ColorGradient"].Width = 15;
                dgv.Columns["ColorGradient"].HeaderText = "";

                DataGridViewRow dgvr = dgv.Rows[e.RowIndex];    // get you required index
                                                                // check the cell value under your specific column and then you can toggle your colors

                if (e.RowIndex < ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber)
                    dgvr.DefaultCellStyle.BackColor = Color.LightPink;

                double dSNRValue = Convert.ToDouble(dgvr.Cells["Composite SNR(dB)"].Value);

                double dRatio = dSNRValue / dMaxValue;

                int nRedValue = 0;
                int nGreenValue = 0;
                int nBlueValue = 0;

                if (dSNRValue < 0)
                {
                    nRedValue = 0;
                    nGreenValue = 255;
                    nBlueValue = 255;
                    dgvr.Cells["Composite SNR(dB)"].Style.BackColor = Color.Red;
                }
                else if (dRatio >= 0.5)
                {
                    nRedValue = (int)Math.Round(((-255 * (dRatio - 0.5)) / 0.5) + 255, 0, MidpointRounding.AwayFromZero);
                    nGreenValue = (int)Math.Round(((-127 * (dRatio - 0.5)) / 0.5) + 255, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    nRedValue = 255;
                    nGreenValue = (int)Math.Round((255 * dRatio) / 0.5, 0, MidpointRounding.AwayFromZero);
                }

                dgvr.Cells["ColorGradient"].Style.BackColor = Color.FromArgb(nRedValue, nGreenValue, nBlueValue);
            }
        }

        private void dgvRank_SizeChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (m_eCurentStep == frmMultiAnalysis.StepEnum.AC_FrequencyRank)
            {
                if (dgv.Width > m_nACFRDataGridViewTotalWidth)
                {
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                else
                {
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        int nColumnWidth = dgv.Columns[i].Width;
                        dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        dgv.Columns[i].Width = nColumnWidth;
                    }
                }

                dgv.AutoResizeColumns();
            }
        }
    }
}
