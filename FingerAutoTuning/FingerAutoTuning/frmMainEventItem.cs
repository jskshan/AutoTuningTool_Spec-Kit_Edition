using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Reflection;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class frmMain
    {
        Helper.DGVHelper objDGVHelper = new Helper.DGVHelper();
        DataGridView dgvDetailFRPH2 = new DataGridView();

        int ndgvACFRTotalWidth = 0;

        class OrderDetailBind
        {
            public static List<OrderDetailBind> m_listcOrderDetailBind = new List<OrderDetailBind>();
            public int m_nIndex { get; set; }
            public double m_dFrequency { get; set; }
            public int m_nSuggestSum { get; set; }
            public string m_nPH1 { get; set; }
            public string m_nPH2 { get; set; }

            public OrderDetailBind(int nIndex, double dFrequency, int nSuggestSum, int nPH1, int nPH2)
            {
                m_nIndex = nIndex;
                m_dFrequency = dFrequency;
                m_nSuggestSum = nSuggestSum;
                m_nPH1 = string.Format("0x{0}", nPH1.ToString("x2").ToUpper());
                m_nPH2 = string.Format("0x{0}", nPH2.ToString("x2").ToUpper());
            }
        }

        public void OutputlblStatus(string sResultMessage, string sErrorMessage, Color colorForeColor, bool bOnlyChangelblStatus = false, bool bResetFlag = false)
        {
            if (sResultMessage == null) return;

            OutputlblStatus_Emb(sResultMessage, colorForeColor);

            if (bOnlyChangelblStatus == false)
                OutputlblErrorMessage_Emb(sErrorMessage, colorForeColor);

            if (bResetFlag == true)
            {
                m_bReset = true;
                m_nCurrentStepIndex = 0;
                m_nCurrentExecuteIndex = 0;
            }
        }

        private void OutputlblStatus_Emb(string sResultMessage, Color colorForeColor)
        {
            lblStatus.Text = sResultMessage;
            lblStatus.ForeColor = colorForeColor;
        }

        private void OutputlblErrorMessage_Emb(string sErrorMessage, Color colorForeColor)
        {
            ResizelblErrorMessageFont(sErrorMessage);
            lblSecondaryMessage.Text = sErrorMessage;
            lblSecondaryMessage.ForeColor = colorForeColor;
        }

        public enum FlowState
        {
            Initial,
            Ready,
            Start,
            Finish,
            StopDisable,
            StopEnable
        }

        public void SetButton(FlowState eFlowState)
        {
            if (eFlowState == FlowState.Initial)
            {
                menustripMainEnableOption(true);
                //ButtonOnOffOption(btnStart, "StartBtn_Enable", true);
                //ButtonOnOffOption(btnStop, "StopBtn_Disable", false);
                //ButtonOnOffOption(btnReset, "ResetBtn_Enable", true);
                //ButtonOnOffOption(btnPattern, "PatternBtn_Disable", false);
                btnNewStart.Enabled = true;
                btnNewStop.Enabled = false;
                btnNewReset.Enabled = true;
                btnNewPattern.Enabled = false;
                //cbxLoadDataEnableOption(true);
                rjtbtnLoadDataEnableOption(true);
            }
            else if (eFlowState == FlowState.Ready || eFlowState == FlowState.Finish)
            {
                menustripMainEnableOption(true);
                //ButtonOnOffOption(btnStart, "StartBtn_Enable", true);
                //ButtonOnOffOption(btnStop, "StopBtn_Disable", false);
                //ButtonOnOffOption(btnReset, "ResetBtn_Enable", true);
                //ButtonOnOffOption(btnPattern, "PatternBtn_Disable", false);
                btnNewStart.Enabled = true;
                btnNewStop.Enabled = false;
                btnNewReset.Enabled = true;
                btnNewPattern.Enabled = false;
                //cbxLoadDataEnableOption(true);
                rjtbtnLoadDataEnableOption(true);
            }
            else if (eFlowState == FlowState.Start)
            {
                //ButtonOnOffOption(btnStart, "StartBtn_Disable", false);
                //ButtonOnOffOption(btnStop, "StopBtn_Enable", true);
                //ButtonOnOffOption(btnReset, "ResetBtn_Disable", false);
                //ButtonOnOffOption(btnPattern, "PatternBtn_Disable", false);
                btnNewStart.Enabled = false;
                btnNewStop.Enabled = true;
                btnNewReset.Enabled = false;
                btnNewPattern.Enabled = false;
                menustripMainEnableOption(false);
                //cbxLoadDataEnableOption(false);
                rjtbtnLoadDataEnableOption(false);
            }
            else if (eFlowState == FlowState.StopDisable)
            {
                //ButtonOnOffOption(btnStop, "StopBtn_Disable", false);
                btnNewStop.Enabled = false;
            }
            else if (eFlowState == FlowState.StopEnable)
            {
                //ButtonOnOffOption(btnStop, "StopBtn_Enable", true);
                btnNewStop.Enabled = true;
            }
        }

        private void menustripMainEnableOption(bool bEnable)
        {
            menustripMain.Enabled = bEnable;
        }

        private void ButtonOnOffOption(Button Btn, string PictureName, bool bEnable = true)
        {
            ResourceManager rm = new ResourceManager(typeof(global::FingerAutoTuning.Properties.Resources));
            Btn.BackgroundImage = rm.GetObject(PictureName) as Image;
            MakeButtonTransparent(Btn);

            this.Invoke((MethodInvoker)delegate
            {
                Btn.Enabled = bEnable;
            });
        }

        /*
        private void cbxLoadDataEnableOption(bool bEnable)
        {
            cbxLoadData.Enabled = bEnable;
        }
        */

        private void rjtbtnLoadDataEnableOption(bool bEnable)
        {
            rjtbtnLoadData.Enabled = bEnable;
        }

        public void InitialstatusstripMessage(int nProgessBarMaxValue, string sInitStr)
        {
            toolstripprogressbarProgressBar.Value = 0;
            toolstripprogressbarProgressBar.Maximum = nProgessBarMaxValue;

            toolstripstatuslabelMessage.Text = sInitStr;
        }

        public enum ProgressUpdate
        {
            Total,
            JustLabel
        }

        public void UpdatestatusstripMessage(ProgressUpdate eProgressUpdate, int nProgessBarValue, string sMessage)
        {
            if (eProgressUpdate == ProgressUpdate.Total)
            {
                toolstripprogressbarProgressBar.Value = nProgessBarValue;
                toolstripstatuslabelMessage.Text = sMessage;
                lblSecondaryMessage.Text = sMessage;
                lblSecondaryMessage.ForeColor = Color.Blue;
            }
            else if (eProgressUpdate == ProgressUpdate.JustLabel)
            {
                toolstripstatuslabelMessage.Text = sMessage;
                lblSecondaryMessage.Text = sMessage;
                lblSecondaryMessage.ForeColor = Color.Blue;
            }
        }

        public void SetlblStepBackColor(MainStep eStep, bool bClearlblStep = false)
        {
            for (int nIndex = 0; nIndex < m_lblStepItem_Array.Length; nIndex++)
            {
                if (m_lblStepItem_Array[nIndex] != null)
                {
                    if (bClearlblStep == false)
                    {
                        if (eStep == (MainStep)(m_lblStepItem_Array[nIndex].Tag))
                            m_lblStepItem_Array[nIndex].BackColor = Color.LightGreen;
                        else
                            m_lblStepItem_Array[nIndex].BackColor = SystemColors.Control;
                    }
                    else
                        m_lblStepItem_Array[nIndex].BackColor = SystemColors.Control;
                }
            }
        }

        public void OutputMessage(string sMessage, bool bWarning = false)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (bWarning == true)
                    rtbxMessage.SelectionColor = Color.Red;

                rtbxMessage.AppendText(sMessage + Environment.NewLine);
            });

            //WriteDebugLog(sMessage);
        }

        public void ResizelblCurrentStepFont(string sMessage)
        {
            int nWidth = lblCurrentStep.Width;
            float fSize = 20;
            FontFamily ff = new FontFamily(lblCurrentStep.Font.Name);

            Graphics gh = this.CreateGraphics();
            SizeF sf = gh.MeasureString(sMessage, lblCurrentStep.Font);

            while (sf.Width > nWidth)
            {
                fSize -= 1.0F;
                lblCurrentStep.Font = new Font(ff, fSize, FontStyle.Bold, GraphicsUnit.World);
                sf = gh.MeasureString(sMessage, lblCurrentStep.Font);
            }

            lblCurrentStep.Text = sMessage;
        }

        public class btnPatternEnableEventArgs : EventArgs
        {
            public bool bEnable = false;

            public btnPatternEnableEventArgs(bool Enable)
            {
                bEnable = Enable;
            }
        }

        public void btnPatternEnableHandler(object sender, btnPatternEnableEventArgs e)
        {
            btnPatternEnable(e.bEnable);
        }

        public void btnPatternEnable(bool bEnable)
        {
            if (bEnable == true)
                //ButtonOnOffOption(btnPattern, "PatternBtn_Enable", true);
                btnNewPattern.Enabled = true;
            else
                //ButtonOnOffOption(btnPattern, "PatternBtn_Disable", false);
                btnNewPattern.Enabled = false;

            lblStatus.Focus();
        }

        public void OutputtoolstripstatuslabelStatus(string sMessage)
        {
            toolstripstatuslabelStatus.Text = sMessage;
        }

        public void OutputrtbxFRPH1(string sResultMessage, string sWarningMessage)
        {
            rtbxFRPH1.Text = sResultMessage;
            rtbxFRPH1.Text += sWarningMessage;

            SetrtbxFRPH1Message();
        }

        private void SetrtbxFRPH1Message()
        {
            // 先將文字按照換行符號分割成陣列
            string[] lines = rtbxFRPH1.Text.Split(new string[] { "\n" }, StringSplitOptions.None);

            bool bOnMark = false;
            bool bFirstLine = true;
            char[] charKeyChar_Array = new char[] { '<', '>', '=' };
            bool bGetResultData = false;
            bool bGetWarningData = false;

            int nCurrentPosition = 0; // 追蹤目前在整個文字中的位置

            // 逐行處理
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string currentLine = lines[lineIndex];

                // 檢查此行是否包含 "Result" 或 "Warning"
                if (currentLine.Contains("Result"))
                {
                    bGetResultData = true;
                    bGetWarningData = false;
                    bFirstLine = true;
                }
                else if (currentLine.Contains("Warning"))
                {
                    bGetResultData = false;
                    bGetWarningData = true;
                }

                // 處理此行的每個字元
                for (int nTextIndex = 0; nTextIndex < currentLine.Length; nTextIndex++)
                {
                    bool bKeyChar = false;
                    Color colorFontColor = Color.Blue;

                    if (bGetResultData)
                    {
                        for (int nIndex = 0; nIndex < charKeyChar_Array.Length; nIndex++)
                        {
                            if (currentLine[nTextIndex] == charKeyChar_Array[nIndex])
                            {
                                bOnMark = true;
                                bKeyChar = true;
                            }
                        }

                        if (bFirstLine == true)
                            colorFontColor = Color.Blue;
                        if (bOnMark == true && bKeyChar == false)
                            colorFontColor = Color.Red;

                        if (bFirstLine == true || (bOnMark == true && bKeyChar == false))
                        {
                            rtbxFRPH1.Select(nCurrentPosition + nTextIndex, 1);
                            rtbxFRPH1.SelectionColor = colorFontColor;
                            float fSize = rtbxFRPH1.Font.Size;
                            FontFamily ff = new FontFamily(rtbxFRPH1.Font.Name);
                            rtbxFRPH1.SelectionFont = new Font(ff, fSize, FontStyle.Bold);
                        }
                    }
                    else if (bGetWarningData)
                    {
                        rtbxFRPH1.Select(nCurrentPosition + nTextIndex, 1);
                        rtbxFRPH1.SelectionColor = Color.Red;
                        float fSize = rtbxFRPH1.Font.Size;
                        FontFamily ff = new FontFamily(rtbxFRPH1.Font.Name);
                        rtbxFRPH1.SelectionFont = new Font(ff, fSize, FontStyle.Bold);
                    }
                }

                // 換行後重置標記
                bOnMark = false;
                bFirstLine = false;

                // 更新位置 (加上此行長度 + 換行符號長度)
                nCurrentPosition += currentLine.Length + 1; // +1 是換行符號
            }
        }

        public void OutputrtbxSelfPNS(string sMessage)
        {
            rtbxSelfPNS.Text = sMessage;
        }

        public enum ButtonType
        {
            btnFRPH1Chart,
            btnFRPH2Chart,
            btnACFRChart,
            btnSelfFSChart
        }

        public void EnableButton(ButtonType eButtonType, bool bEnable)
        {
            if (eButtonType == ButtonType.btnFRPH1Chart)
                btnFRPH1Chart.Enabled = bEnable;
            else if (eButtonType == ButtonType.btnFRPH2Chart)
                btnFRPH2Chart.Enabled = bEnable;
            else if (eButtonType == ButtonType.btnACFRChart)
                btnACFRChart.Enabled = bEnable;
            else if (eButtonType == ButtonType.btnSelfFSChart)
                btnSelfFSChart.Enabled = bEnable;
        }

        public void ResizelblErrorMessageFont(string sMessage)
        {
            int nLabelWidth = lblSecondaryMessage.Width;
            //float fSize = 24;
            FontFamily ff = new FontFamily(lblSecondaryMessage.Font.Name);

            Graphics gh = this.CreateGraphics();
            SizeF sf = gh.MeasureString(sMessage, lblSecondaryMessage.Font);

            /*
            while (sf.Width > nLabelWidth)
            {
                fSize -= 1.0F;

                try
                {
                    lblErrorMessage.Font = new Font(ff, fSize, FontStyle.Bold, GraphicsUnit.World);
                    sf = gh.MeasureString(sMessage, lblErrorMessage.Font);
                }
                catch
                {
                }
            }
            */
        }

        public enum DataGridViewType
        {
            dgvFRPH2,
            dgvACFR,
            dgvRawADCS
        }

        public void OutputDataGridView(DataGridViewType eDataGridViewType, DataTable datatableData)
        {
            if (eDataGridViewType == DataGridViewType.dgvFRPH2)
            {
                while (dgvFRPH2.Rows.Count != 0)
                    dgvFRPH2.Rows.RemoveAt(0);

                dgvFRPH2.Columns.Clear();
                dgvFRPH2.DataSource = datatableData;
                dgvFRPH2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvFRPH2.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10);
                dgvFRPH2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvFRPH2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvFRPH2.RowsDefaultCellStyle.Font = new Font("Times New Roman", 12);
                dgvFRPH2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;

                //Grid Back ground Color
                dgvFRPH2.BackgroundColor = Color.LightSteelBlue;

                //Grid Back Color
                dgvFRPH2.RowsDefaultCellStyle.BackColor = Color.AliceBlue;

                //GridColumnStylesCollection Alternate Rows Backcolr
                dgvFRPH2.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

                // Auto generated here set to tru or false.
                //dgvFRPH2.AutoGenerateColumns = false;
                //ShanuDGV.DefaultCellStyle.Font = new Font("Verdana", 10.25f, FontStyle.Regular);
                //ShanuDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 11, FontStyle.Regular);

                //Column Header back Color
                dgvFRPH2.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
                //
                dgvFRPH2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                //header Visisble
                //dgvFRPH2.EnableHeadersVisualStyles = false;

                // Enable the row header
                //dgvFRPH2.RowHeadersVisible = false;

                // to Hide the Last Empty row here we use false.
                dgvFRPH2.AllowUserToAddRows = false;

                for (int nIndex = 0; nIndex < dgvFRPH2.Columns.Count; nIndex++)
                {
                    dgvFRPH2.Columns[nIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                /*
                for (int i = 0; i < dgvFRPH2.Rows.Count; i++)
                {
                    if (i < ParamFingerAutoTuning.m_nFRPH2BestRankNumber)
                        dgvFRPH2.Rows[i].DefaultCellStyle.BackColor = Color.LightPink;
                    else
                        break;
                }
                */

                dgvFRPH2.AutoResizeColumns();
                //dgvFRPH2.Update();
                //dgvFRPH2.Refresh();

                /*
                InitializeMasterGrid(datatableData);

                InitializeDetailGrid();
                */
            }
            else if (eDataGridViewType == DataGridViewType.dgvACFR)
            {
                while (dgvACFR.Rows.Count != 0)
                    dgvACFR.Rows.RemoveAt(0);

                dgvACFR.Columns.Clear();
                dgvACFR.DataSource = datatableData;
                dgvACFR.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvACFR.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10);
                dgvACFR.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvACFR.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvACFR.RowsDefaultCellStyle.Font = new Font("Times New Roman", 12);
                dgvACFR.CellBorderStyle = DataGridViewCellBorderStyle.Sunken;

                //Grid Back ground Color
                dgvACFR.BackgroundColor = Color.LightSteelBlue;

                //Grid Back Color
                dgvACFR.RowsDefaultCellStyle.BackColor = Color.AliceBlue;

                //GridColumnStylesCollection Alternate Rows Backcolr
                dgvACFR.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

                //Column Header back Color
                dgvACFR.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
                //
                dgvACFR.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                // to Hide the Last Empty row here we use false.
                dgvACFR.AllowUserToAddRows = false;

                for (int nColumnIndex = 0; nColumnIndex < dgvACFR.Columns.Count; nColumnIndex++)
                {
                    dgvACFR.Columns[nColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                for (int nColumnIndex = 0; nColumnIndex < dgvACFR.Columns.Count; nColumnIndex++)
                {
                    dgvACFR.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    int nWidth = dgvACFR.Columns[nColumnIndex].Width;
                    dgvACFR.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvACFR.Columns[nColumnIndex].Width = nWidth;
                }

                ndgvACFRTotalWidth = 0;

                for (int nColumnIndex = 0; nColumnIndex < dgvACFR.Columns.Count; nColumnIndex++)
                {
                    int nWidth = dgvACFR.Columns[nColumnIndex].Width;
                    ndgvACFRTotalWidth += nWidth;
                }

                if (dgvACFR.Width > ndgvACFRTotalWidth)
                {
                    for (int nColumnIndex = 0; nColumnIndex < dgvACFR.Columns.Count; nColumnIndex++)
                    {
                        dgvACFR.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    dgvACFR.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                else
                {
                    for (int nColumnIndex = 0; nColumnIndex < dgvACFR.Columns.Count; nColumnIndex++)
                    {
                        dgvACFR.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        int nWidth = dgvACFR.Columns[nColumnIndex].Width;
                        dgvACFR.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        dgvACFR.Columns[nColumnIndex].Width = nWidth;
                    }
                }

                dgvACFR.AutoResizeColumns();
            }
            else if (eDataGridViewType == DataGridViewType.dgvRawADCS)
            {
                while (dgvRawADCS.Rows.Count != 0)
                    dgvRawADCS.Rows.RemoveAt(0);

                dgvRawADCS.Columns.Clear();
                dgvRawADCS.DataSource = datatableData;
                dgvRawADCS.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRawADCS.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 9.0f);
                dgvRawADCS.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvRawADCS.RowsDefaultCellStyle.Font = new Font("Times New Roman", 9.5f);
                dgvRawADCS.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;

                //Grid Back ground Color
                dgvRawADCS.BackgroundColor = Color.LightSteelBlue;

                //Grid Back Color
                dgvRawADCS.RowsDefaultCellStyle.BackColor = Color.AliceBlue;

                //GridColumnStylesCollection Alternate Rows Backcolr
                dgvRawADCS.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

                // Auto generated here set to tru or false.
                //dgvRawADCS.AutoGenerateColumns = false;
                //ShanuDGV.DefaultCellStyle.Font = new Font("Verdana", 10.25f, FontStyle.Regular);
                //ShanuDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 11, FontStyle.Regular);

                //Column Header back Color
                dgvRawADCS.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
                //
                dgvRawADCS.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                //header Visisble
                //dgvRawADCS.EnableHeadersVisualStyles = false;

                // Enable the row header
                //dgvRawADCS.RowHeadersVisible = false;

                // to Hide the Last Empty row here we use false.
                dgvRawADCS.AllowUserToAddRows = false;

                for (int nIndex = 0; nIndex < dgvRawADCS.Columns.Count; nIndex++)
                {
                    dgvRawADCS.Columns[nIndex].SortMode = DataGridViewColumnSortMode.Automatic;
                }

                //dgvRawADCS.AutoResizeColumns();

                int nColumnCount = dgvRawADCS.Columns.Count;
                int nColumnWidth = (int)Math.Ceiling((double)dgvRawADCS.Width / nColumnCount);

                //int nRankIndex = 0;
                int nSELCIndex = 1;
                int nVSELIndex = 2;
                int nLGIndex = 3;
                int nSELGMIndex = 4;
                int nSuggestIQ_BSHIndex = 5;

                List<int> listADCMeanIndex = new List<int>();
                List<int> listADCMaxIndex = new List<int>();

                for (int nColumnIndex = 0; nColumnIndex < dgvRawADCS.Columns.Count; nColumnIndex++)
                {
                    //if (dgvRawADCS.Columns[nColumnIndex].HeaderText == "Rank")
                    //    nRankIndex = nColumnIndex;
                    if (dgvRawADCS.Columns[nColumnIndex].HeaderText.Contains("_SELC") == true)
                        nSELCIndex = nColumnIndex;
                    else if (dgvRawADCS.Columns[nColumnIndex].HeaderText.Contains("_VSEL") == true)
                        nVSELIndex = nColumnIndex;
                    else if (dgvRawADCS.Columns[nColumnIndex].HeaderText.Contains("_LG") == true)
                        nLGIndex = nColumnIndex;
                    else if (dgvRawADCS.Columns[nColumnIndex].HeaderText.Contains("_SELGM") == true)
                        nSELGMIndex = nColumnIndex;
                    else if (dgvRawADCS.Columns[nColumnIndex].HeaderText.Contains("_IQ_BSH") == true)
                        nSuggestIQ_BSHIndex = nColumnIndex;

                    // 記錄 ADC Mean 和 ADC Max 欄位的索引
                    if (dgvRawADCS.Columns[nColumnIndex].HeaderText.Contains("ADC Mean") == true)
                        listADCMeanIndex.Add(nColumnIndex);
                    else if (dgvRawADCS.Columns[nColumnIndex].HeaderText.Contains("ADC Max") == true)
                        listADCMaxIndex.Add(nColumnIndex);
                }

                for (int nColumnIndex = 0; nColumnIndex < dgvRawADCS.Columns.Count; nColumnIndex++)
                {
                    dgvRawADCS.Columns[nColumnIndex].Width = nColumnWidth;

                    /*
                    dgvRawADCS.EnableHeadersVisualStyles = false;

                    if (nColumnIndex == nRankIndex)
                    {
                        dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.BackColor = Color.LightCyan;
                        dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.ForeColor = Color.Black;
                    }
                    else if (nColumnIndex == nSELCIndex || nColumnIndex == nVSELIndex || nColumnIndex == nLGIndex || nColumnIndex == nSELGMIndex)
                    {
                        dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.BackColor = Color.LightGreen;
                        dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.ForeColor = Color.Black;
                    }
                    else if (nColumnIndex == nSuggestIQ_BSHIndex)
                    {
                        dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.BackColor = Color.LightYellow;
                        dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.ForeColor = Color.Black;
                    }
                    else
                    {
                        dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.BackColor = Color.LightGray;
                        dgvRawADCS.Columns[nColumnIndex].HeaderCell.Style.ForeColor = Color.Black;
                    }
                    */

                    for (int nRowIndex = 0; nRowIndex < dgvRawADCS.Rows.Count; nRowIndex++)
                    {
                        //dgvRawADCS.Rows[nRowIndex].Cells[nRankIndex].Style.BackColor = Color.LightCyan;
                        dgvRawADCS.Rows[nRowIndex].Cells[nSELCIndex].Style.BackColor = Color.LightGreen;
                        dgvRawADCS.Rows[nRowIndex].Cells[nVSELIndex].Style.BackColor = Color.LightGreen;
                        dgvRawADCS.Rows[nRowIndex].Cells[nLGIndex].Style.BackColor = Color.LightGreen;
                        dgvRawADCS.Rows[nRowIndex].Cells[nSELGMIndex].Style.BackColor = Color.LightGreen;
                        dgvRawADCS.Rows[nRowIndex].Cells[nSuggestIQ_BSHIndex].Style.BackColor = Color.LightYellow;

                        // 當 ADC Mean 欄位數值為 32767 時，將文字顯示為紅色
                        foreach (int nADCMeanIndex in listADCMeanIndex)
                        {
                            if (dgvRawADCS.Rows[nRowIndex].Cells[nADCMeanIndex].Value != null)
                            {
                                int nValue = 0;
                                if (int.TryParse(dgvRawADCS.Rows[nRowIndex].Cells[nADCMeanIndex].Value.ToString(), out nValue))
                                {
                                    if (nValue == 32767)
                                        dgvRawADCS.Rows[nRowIndex].Cells[nADCMeanIndex].Style.ForeColor = Color.Red;
                                }
                            }
                        }

                        // 當 ADC Max 欄位數值為 32767 時，將文字顯示為紅色
                        foreach (int nADCMaxIndex in listADCMaxIndex)
                        {
                            if (dgvRawADCS.Rows[nRowIndex].Cells[nADCMaxIndex].Value != null)
                            {
                                int nValue = 0;
                                if (int.TryParse(dgvRawADCS.Rows[nRowIndex].Cells[nADCMaxIndex].Value.ToString(), out nValue))
                                {
                                    if (nValue == 32767)
                                        dgvRawADCS.Rows[nRowIndex].Cells[nADCMaxIndex].Style.ForeColor = Color.Red;
                                }
                            }
                        }
                    }
                }

                //dgvRawADCS.Update();
                //dgvRawADCS.Refresh();

                /*
                InitializeMasterGrid(Dt);

                InitializeDetailGrid();
                */
            }
        }

        // to generate Master Datagridview with your coding
        public void InitializeMasterGrid(DataTable datatableData)
        {
            //First generate the grid Layout Design
            Helper.DGVHelper.Layouts(dgvFRPH2, Color.LightSteelBlue, Color.AliceBlue, Color.WhiteSmoke, false, Color.SteelBlue, false, false, false);

            //Set Height,width and add panel to your selected control
            //Helper.DGVHelper.Generategrid(FRPH1Dgv, pnlShanuGrid, 1000, 600, 10, 10);

            // Color Image Column creation
            Helper.DGVHelper.Templatecolumn(dgvFRPH2, ControlTypes.ImageColumn, "Info", "", "", true, 100, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvFRPH2, ControlTypes.BoundColumn, "Rank", "Rank", "Rank", true, 90, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvFRPH2, ControlTypes.BoundColumn, "Frequency", "Frequency(KHz)", "Frequency(KHz)", true, 80, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvFRPH2, ControlTypes.BoundColumn, "Suggest SUM", "Suggest SUM", "Suggest SUM", true, 320, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvFRPH2, ControlTypes.BoundColumn, "Signal RefValue", "Signal RefValue", "Signal RefValue", true, 140, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvFRPH2, ControlTypes.BoundColumn, "Noise RefValue", "Noise RefValue", "Noise RefValue", true, 120, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvFRPH2, ControlTypes.BoundColumn, "RawSNR RefValue", "RawSNR RefValue", "RawSNR RefValue", true, 120, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvFRPH2, ControlTypes.BoundColumn, "Reference Value", "Reference Value", "Reference Value", true, 120, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);


            //Convert the List to DataTable
            DataTable detailTableList = ConvertListToDataTable(OrderDetailBind.m_listcOrderDetailBind);

            // Image Colum Click Event - In  this method we create an event for cell click and we will display the Detail grid with result.

            objDGVHelper.DGVMasterGridClickEvents(dgvFRPH2, dgvDetailFRPH2, dgvFRPH2.Columns["Info"].Index, EventTypes.cellContentClick, ControlTypes.ImageColumn, detailTableList, "Frequency");

            // Bind data to DGV.
            dgvFRPH2.DataSource = datatableData;

            dgvFRPH2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvFRPH2.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12);
            dgvFRPH2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvFRPH2.RowsDefaultCellStyle.Font = new Font("Times New Roman", 12);
            dgvFRPH2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            for (int nColumnIndex = 0; nColumnIndex < dgvFRPH2.Columns.Count; nColumnIndex++)
            {
                dgvFRPH2.Columns[nColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            //dgvFRPH2.AutoResizeColumns();

            int nLastColumnIndex = dgvFRPH2.Columns.Count - 1;

            for (int nColumnIndex = 0; nColumnIndex < dgvFRPH2.Columns.Count; nColumnIndex++)
            {
                if (nLastColumnIndex > 0)
                    dgvFRPH2.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                else
                    dgvFRPH2.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            for (int nColumnIndex = 0; nColumnIndex < dgvFRPH2.Columns.Count; nColumnIndex++)
            {
                int nWidth = dgvFRPH2.Columns[nColumnIndex].Width;
                dgvFRPH2.Columns[nColumnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvFRPH2.Columns[nColumnIndex].Width = nWidth;
            }

            for (int nRowIndex = 0; nRowIndex < dgvFRPH2.Rows.Count; nRowIndex++)
            {
                if (nRowIndex < ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber)
                    dgvFRPH2.Rows[nRowIndex].DefaultCellStyle.BackColor = Color.LightPink;
                else
                    break;
            }
        }

        // to generate Detail Datagridview with your coding
        public void InitializeDetailGrid()
        {
            //First generate the grid Layout Design
            Helper.DGVHelper.Layouts(dgvDetailFRPH2, Color.Peru, Color.Wheat, Color.Tan, false, Color.Sienna, false, false, false);

            //Set Height,width and add panel to your selected control
            Helper.DGVHelper.Generategrid(dgvDetailFRPH2, tpgFRPH2, 800, 200, 10, 10);

            // Color Dialog Column creation
            Helper.DGVHelper.Templatecolumn(dgvDetailFRPH2, ControlTypes.BoundColumn, "Index", "Index", "Index", true, 90, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvDetailFRPH2, ControlTypes.BoundColumn, "Frequency", "Frequency(KHz)", "Frequency(KHz)", true, 80, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvDetailFRPH2, ControlTypes.BoundColumn, "Suggest SUM", "Suggest SUM", "Suggest SUM", true, 80, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvDetailFRPH2, ControlTypes.BoundColumn, "PH1", "PH1", "PH1", true, 160, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            // BoundColumn creation
            Helper.DGVHelper.Templatecolumn(dgvDetailFRPH2, ControlTypes.BoundColumn, "PH2", "PH2", "PH2", true, 160, DataGridViewTriState.True, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter, Color.Transparent, null, "", "", Color.Black);

            objDGVHelper.DGVDetailGridClickEvents(dgvDetailFRPH2);

            dgvDetailFRPH2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetailFRPH2.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12);
            dgvDetailFRPH2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetailFRPH2.RowsDefaultCellStyle.Font = new Font("Times New Roman", 12);
            dgvDetailFRPH2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvDetailFRPH2.AutoResizeColumns();
        }

        public void SetDataGridViewSet(DataTable datatableData, int nMinPH1, int nMinPH2)
        {
            for (int nRowIndex = 0; nRowIndex < datatableData.Rows.Count; nRowIndex++)
            {
                double fFrequency = Convert.ToDouble(datatableData.Rows[nRowIndex]["Frequency"]);
                int nSuggestSum = Convert.ToInt32(datatableData.Rows[nRowIndex]["SuggestSum"]);

                int nPH1PH2Sum = ElanConvert.Convert2PH1PH2SumInt(fFrequency);

                int nPH2Value = nMinPH2;
                int nPH1Value = nPH1PH2Sum - nPH2Value;
                int nRankIndex = 1;

                while (nPH1Value >= nMinPH1)
                {
                    if (nPH1Value <= nPH2Value)
                    {
                        OrderDetailBind cOrderDetailBind = new OrderDetailBind(nRankIndex, fFrequency, nSuggestSum, nPH1Value, nPH2Value);
                        OrderDetailBind.m_listcOrderDetailBind.Add(cOrderDetailBind);
                        nRankIndex++;
                    }

                    nPH2Value++;
                    nPH1Value--;
                }
            }
        }

        //List to Data Table Convert
        private static DataTable ConvertListToDataTable<T>(IEnumerable<T> Data_List)
        {
            Type type = typeof(T);
            var typeproperties = type.GetProperties();

            DataTable datatableData = new DataTable();

            foreach (PropertyInfo propInfo in typeproperties)
            {
                datatableData.Columns.Add(new DataColumn(propInfo.Name, propInfo.PropertyType));
            }

            foreach (T listItem in Data_List)
            {
                object[] objValue_Array = new object[typeproperties.Length];

                for (int nPropertyIndex = 0; nPropertyIndex < typeproperties.Length; nPropertyIndex++)
                {
                    objValue_Array[nPropertyIndex] = typeproperties[nPropertyIndex].GetValue(listItem, null);
                }

                datatableData.Rows.Add(objValue_Array);
            }

            return datatableData;
        }

        public void OutputDebugLog(string sMessage)
        {
            WriteDebugLog(sMessage);
        }
    }
}
