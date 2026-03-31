using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    partial class frmMain
    {
        public bool CheckModeState(string sModeState)
        {
            bool bCheckFlag = false;

            this.Invoke((MethodInvoker)delegate
            {
                if (cbxModeState.SelectedItem.ToString() == sModeState)
                    bCheckFlag = true;
                else
                    bCheckFlag = false;
            });

            return bCheckFlag;
        }

        public string GetModeState()
        {
            string sModeState = MainConstantParameter.m_sMODE_SINGLE;

            this.Invoke((MethodInvoker)delegate
            {
                sModeState = cbxModeState.SelectedItem.ToString();
            });

            return sModeState;
        }

        public string GetModeName()
        {
            if (m_nModeFlag == MainConstantParameter.m_nMODE_SINGLE)
                return MainConstantParameter.m_sMODE_SINGLE;
            else if (m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                return MainConstantParameter.m_sMODE_CLIENT;
            else if (m_nModeFlag == MainConstantParameter.m_nMODE_SERVER)
                return MainConstantParameter.m_sMODE_SERVER;
            else if (m_nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
                return MainConstantParameter.m_sMODE_GODRAW;
            else if (m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
                return MainConstantParameter.m_sMODE_LOADDATA;
            else
                return MainConstantParameter.m_sMODE_SINGLE;
        }

        public void SetNewConnectButton(bool bEnableFlag)
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnNewConnect.Enabled = bEnableFlag;
            });
        }

        public void SetNewStartButton(bool bEnableFlag)
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnNewStart.Enabled = bEnableFlag;
            });
        }

        public void SetNewStopButton(bool bEnableFlag)
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnNewStop.Enabled = bEnableFlag;
            });
        }

        public void SetChartButton(bool bEnableFlag, string sFolderPath = "")
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnChart.Enabled = bEnableFlag;
                btnChart.Tag = sFolderPath;
            });
        }

        public void SetNewPatternButton(bool bEnableFlag)
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnNewPattern.Enabled = bEnableFlag;
            });
        }

        public void SetNewDrawButton(bool bEnableFlag)
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnNewDraw.Enabled = bEnableFlag;
            });
        }

        public void SetNewDrawButton(int nButtonFlag)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (nButtonFlag == MainConstantParameter.m_nDRAWSTATE_READYTODRAW)
                {
                    m_nDrawButtonStateFlag = MainConstantParameter.m_nDRAWSTATE_READYTODRAW;
                    btnNewDraw.Image = Properties.Resources.play;
                    Bitmap btDrawImage = btnNewDraw.Image as Bitmap;
                    Bitmap btDrawResize = new Bitmap(btDrawImage, new Size(18, 18));
                    btnNewDraw.Font = new Font("Times New Roman", 11);
                    btnNewDraw.Image = btDrawResize;
                    btnNewDraw.ImageAlign = ContentAlignment.MiddleLeft;
                    btnNewDraw.TextImageRelation = TextImageRelation.ImageBeforeText;
                    btnNewDraw.TextAlign = ContentAlignment.MiddleRight;
                    btnNewDraw.Text = "Start Draw";
                    btnNewDraw.Enabled = true;
                }
                else if (nButtonFlag == MainConstantParameter.m_nDRAWSTATE_STARTTODRAW)
                {
                    m_nDrawButtonStateFlag = MainConstantParameter.m_nDRAWSTATE_STARTTODRAW;
                    btnNewDraw.Image = Properties.Resources.forbidden_2;
                    Bitmap btDrawImage = btnNewDraw.Image as Bitmap;
                    Bitmap btDrawResize = new Bitmap(btDrawImage, new Size(18, 18));
                    btnNewDraw.Font = new Font("Times New Roman", 10);
                    btnNewDraw.Image = btDrawResize;
                    btnNewDraw.ImageAlign = ContentAlignment.MiddleLeft;
                    btnNewDraw.TextImageRelation = TextImageRelation.ImageBeforeText;
                    btnNewDraw.TextAlign = ContentAlignment.MiddleRight;
                    btnNewDraw.Text = "Finish Draw";
                    btnNewDraw.Enabled = true;
                }
                else if (nButtonFlag == MainConstantParameter.m_nDRAWSTATE_FINISH)
                {
                    m_nDrawButtonStateFlag = MainConstantParameter.m_nDRAWSTATE_FINISH;
                    btnNewDraw.Image = Properties.Resources.play;
                    Bitmap btDrawImage = btnNewDraw.Image as Bitmap;
                    Bitmap btDrawResize = new Bitmap(btDrawImage, new Size(18, 18));
                    btnNewDraw.Font = new Font("Times New Roman", 11);
                    btnNewDraw.Image = btDrawResize;
                    btnNewDraw.ImageAlign = ContentAlignment.MiddleLeft;
                    btnNewDraw.TextImageRelation = TextImageRelation.ImageBeforeText;
                    btnNewDraw.TextAlign = ContentAlignment.MiddleRight;
                    btnNewDraw.Text = "Start Draw";
                    btnNewDraw.Enabled = false;
                }
            });
        }

        public void SetNewPatternAndNewDrawButton(bool bEnableFlag)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (bEnableFlag == false)
                {
                    btnNewPattern.FlatAppearance.BorderSize = 0;
                    btnNewPattern.FlatAppearance.BorderColor = Color.Blue;
                    btnNewPattern.BackColor = Color.Transparent;
                }

                btnNewPattern.Enabled = bEnableFlag;

                if (bEnableFlag == false)
                {
                    btnNewDraw.FlatAppearance.BorderSize = 0;
                    btnNewDraw.FlatAppearance.BorderColor = Color.Black;
                    btnNewDraw.BackColor = Color.Transparent;
                }

                btnNewDraw.Enabled = bEnableFlag;
            });
        }

        private void OutputRichTextBox(TabControl tcTabControl, TabPage tpgTabPage, RichTextBox rtbxRichTextBox, string sMessage)
        {
            this.Invoke((MethodInvoker)delegate
            {
                tpgTabPage.Parent = tcTabControl;
                rtbxRichTextBox.Text = sMessage;
            });
        }

        public void SetTopMost(bool bTopMostFlag, string sMessage)
        {
            if (bTopMostFlag == true)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.TopMost = true;
                });

                ShowMessageBox(string.Format("{0}!", sMessage), frmMessageBox.m_sError);
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.TopMost = false;
                });
            }
        }

        public void SetStepLabelBackColor(MainTuningStep eMainStep, bool bStepLabelClearFlag = false)
        {
            this.Invoke((MethodInvoker)delegate
            {
                for (int nItemIndex = 0; nItemIndex < m_lblTestItem_Array.Length; nItemIndex++)
                {
                    if (m_lblTestItem_Array[nItemIndex] != null)
                    {
                        if (bStepLabelClearFlag == false)
                        {
                            if (eMainStep == (MainTuningStep)(m_lblTestItem_Array[nItemIndex].Tag))
                                m_lblTestItem_Array[nItemIndex].BackColor = Color.LightGreen;
                            else
                                m_lblTestItem_Array[nItemIndex].BackColor = SystemColors.Control;
                        }
                        else
                            m_lblTestItem_Array[nItemIndex].BackColor = SystemColors.Control;
                    }
                }
            });
        }

        public void OutputStatusAndErrorMessageLabel(string sResultMessage, string sErrorMessage, Color colorForeColor, bool bOnlyChangelblStatusFlag = false)
        {
            if (sResultMessage == null)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                lblStatus.Text = sResultMessage;
                lblStatus.ForeColor = colorForeColor;

                if (bOnlyChangelblStatusFlag == false)
                {
                    lblErrorMessage.Text = sErrorMessage;
                    lblErrorMessage.ForeColor = colorForeColor;
                }
            });
        }

        public void SetModeStateComboBoxAndSettingToolStripMenuItem(bool bEnableFlag)
        {
            ToolStripMenuItem[] toolstripmenuitemSetting_Array = new ToolStripMenuItem[5] 
            { 
                toolstripmenuitemStepSetting,
                toolstripmenuitemFlowSetting,
                toolstripmenuitemParameterSetting,
                toolstripmenuitemFrequencySetting,
                toolstripmenuitemGoDrawController 
            };

            this.Invoke((MethodInvoker)delegate
            {
                cbxModeState.Enabled = bEnableFlag;

                for (int nControlIndex = 0; nControlIndex < toolstripmenuitemSetting_Array.Length; nControlIndex++)
                    toolstripmenuitemSetting_Array[nControlIndex].Enabled = bEnableFlag;
            });
        }

        public void WriteDebugLog(string sMessage)
        {
            m_cDebugLog.WriteLogToBuffer(sMessage);
        }

        public void OutputMessage(string sMessage, bool bWarning = false)
        {
            if (sMessage == null) 
                return;

            this.Invoke((MethodInvoker)delegate
            {
                if (bWarning == true)
                    rtbxMessage.SelectionColor = Color.Red;

                rtbxMessage.AppendText(sMessage + Environment.NewLine);
                WriteDebugLog(sMessage);
            });
        }

        public void OutputMainStatusStrip(string sStatus, int nCurrentCount, int nTotalCount = 0, int nStatusFlag = m_nOtherFlag)
        {
            if (nStatusFlag == m_nStepOutputFlag)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    toolstripstatuslblStep.Text = sStatus;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (nStatusFlag == m_nInitialFlag)
                    {
                        toolstripprogressbarMain.Value = 0;
                        toolstripprogressbarMain.Maximum = nTotalCount;
                    }
                    else if (nStatusFlag == m_nMaximumFlag)
                        toolstripprogressbarMain.Value = toolstripprogressbarMain.Maximum;
                    else
                        toolstripprogressbarMain.Value = nCurrentCount;

                    if (sStatus == "Error")
                        toolstripstatuslblProgress.Text = "Error";
                    else
                        toolstripstatuslblProgress.Text = string.Format("{0} / {1} ({2})", toolstripprogressbarMain.Value, toolstripprogressbarMain.Maximum, sStatus);
                });
            }
        }

        public void OutputStateMessage(string sMessage, bool bResultOutputFlag = false, bool bResetOutputFlag = false, bool bStepOutputOnlyFlag = false, bool bTimerCountStopFlag = false)
        {
            if (bStepOutputOnlyFlag == true)
            {
                OutputStatusAndErrorMessageLabel("Execute", string.Format("Step : {0}", sMessage), Color.Blue);
                OutputMainStatusStrip(string.Format("Step : {0} , ResultDirectory : {1}", sMessage, m_sRecordDirectoryName), 0, 0, m_nStepOutputFlag);
            }
            else
            {
                //Output Message Status
                if (bResetOutputFlag == true)
                {
                    if (bTimerCountStopFlag == true)
                        m_swCostTime.Stop();

                    OutputStatusAndErrorMessageLabel("Ready", "", Color.Blue);
                    m_bStartExecuteFlag = false;
                }
                //Complete Status
                else if (m_bErrorFlag == false)
                {
                    SetFlowStepResult(true);

                    ClearAndSetFlowStepResultList();

                    if (bResultOutputFlag == true)
                    {
                        m_swCostTime.Stop();

                        TimeSpan tsDiffer = m_swCostTime.Elapsed;

                        int nDayToHourOffset = tsDiffer.Days * 24;
                        int nRealHours = tsDiffer.Hours + nDayToHourOffset;

                        m_nTotalHours = nRealHours;
                        m_nTotalMinutes = tsDiffer.Minutes;
                        m_nTotalSeconds = tsDiffer.Seconds;

                        string sRealHour = nRealHours.ToString().PadLeft(2, '0');
                        string sMinute = tsDiffer.Minutes.ToString().PadLeft(2, '0');
                        string sSecond = tsDiffer.Seconds.ToString().PadLeft(2, '0');

                        OutputMessage(string.Format("-CostTime={0}hr:{1}m:{2}s", sRealHour, sMinute, sSecond));

                        OutputStatusAndErrorMessageLabel(string.Format("Complete [CostTime={0}hr:{1}m:{2}s]", sRealHour, sMinute, sSecond), "", Color.Green);

                        m_bStartExecuteFlag = false;
                    }

                    m_sRecordDirectoryPath = m_sFileDirectoryPath;
                }
                //Error Status
                else
                {
                    m_bResultErrorFlag = true;
                    //RemoveResultDirectory();

                    SetFlowStepResult(false, sMessage);

                    ClearAndSetFlowStepResultList();

                    m_swCostTime.Stop();

                    TimeSpan tsDiffer = m_swCostTime.Elapsed;

                    int nDayToHourOffset = tsDiffer.Days * 24;
                    int nRealHours = tsDiffer.Hours + nDayToHourOffset;

                    m_nTotalHours = nRealHours;
                    m_nTotalMinutes = tsDiffer.Minutes;
                    m_nTotalSeconds = tsDiffer.Seconds;

                    string sRealHour = nRealHours.ToString().PadLeft(2, '0');
                    string sMinute = tsDiffer.Minutes.ToString().PadLeft(2, '0');
                    string sSecond = tsDiffer.Seconds.ToString().PadLeft(2, '0');

                    OutputMessage(string.Format("-CostTime={0}hr:{1}m:{2}s", sRealHour, sMinute, sSecond));

                    OutputStatusAndErrorMessageLabel(string.Format("Error [CostTime={0}hr:{1}m:{2}s]", sRealHour, sMinute, sSecond), string.Format("({0})", sMessage), Color.Red);

                    OutputMainStatusStrip("Error", 0, 1);
                    RemoveResultDirectory();
                    m_bStartExecuteFlag = false;
                }
            }
        }

        public void SetFlowStepResult(bool bResultFlag, string sMessage = "")
        {
            if (m_cCurrentFlowStep != null)
            {
                bool bErrorFlag = false;

                for (int nStepIndex = 0; nStepIndex < m_cFlowStep_List.Count; nStepIndex++)
                {
                    if (m_cFlowStep_List[nStepIndex].m_eMainStep == m_cCurrentFlowStep.m_eMainStep &&
                        m_cFlowStep_List[nStepIndex].m_eSubStep == m_cCurrentFlowStep.m_eSubStep)
                    {
                        if (bResultFlag == true)
                            m_cFlowStep_List[nStepIndex].m_bStepErrorFlag = false;
                        else
                        {
                            m_cFlowStep_List[nStepIndex].m_bStepErrorFlag = true;
                            m_cFlowStep_List[nStepIndex].m_sStepErrorMessage = sMessage;
                            bErrorFlag = true;
                        }

                        if (bResultFlag == false && bErrorFlag == true)
                            m_cFlowStep_List[nStepIndex].m_bStepErrorFlag = true;
                    }
                }
            }
        }

        public void ClearAndSetFlowStepResultList()
        {
            m_cFlowStepResult_List.Clear();
            m_cFlowStepResult_List.AddRange(m_cFlowStep_List);
        }

        private void OutputResultDataGridView(DataTable datatableData)
        {
            this.Invoke((MethodInvoker)delegate
            {
                while (dgvNoiseRank.Rows.Count != 0)
                    dgvNoiseRank.Rows.RemoveAt(0);

                dgvNoiseRank.Columns.Clear();
                dgvNoiseRank.DataSource = datatableData;
                dgvNoiseRank.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNoiseRank.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 11);
                dgvNoiseRank.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvNoiseRank.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNoiseRank.RowsDefaultCellStyle.Font = new Font("Times New Roman", (float)10);
                dgvNoiseRank.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                foreach (DataGridViewColumn dgvcColumn in dgvNoiseRank.Columns)
                {
                    if (dgvcColumn.Name == SpecificText.m_sException_Message)
                        dgvcColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    else
                        dgvcColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                dgvNoiseRank.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;

                // Grid Back ground Color
                dgvNoiseRank.BackgroundColor = Color.LightSteelBlue;

                // Grid Back Color
                dgvNoiseRank.RowsDefaultCellStyle.BackColor = Color.AliceBlue;

                // GridColumnStylesCollection Alternate Rows Back Color
                dgvNoiseRank.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

                // Column Header Back Color & Fore Color
                dgvNoiseRank.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
                dgvNoiseRank.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                for (int nIndex = 0; nIndex < dgvNoiseRank.Columns.Count; nIndex++)
                {
                    dgvNoiseRank.Columns[nIndex].SortMode = DataGridViewColumnSortMode.NotSortable;

                    if (nIndex == dgvNoiseRank.Columns.Count - 1)
                        dgvNoiseRank.Columns[nIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }

                if (dgvNoiseRank.Columns.Contains(SpecificText.m_sException_Message) == true)
                {
                    for (int nIndex = 0; nIndex < dgvNoiseRank.Rows.Count; nIndex++)
                    {
                        if (dgvNoiseRank[SpecificText.m_sException_Message, nIndex].Value.ToString() != "")
                        {
                            if (dgvNoiseRank[SpecificText.m_sException_Message, nIndex].Value.ToString().Contains("Max Value") == true && dgvNoiseRank[SpecificText.m_sException_Message, nIndex].Value.ToString().Contains("Over Warning") == true)
                                dgvNoiseRank[SpecificText.m_sException_Message, nIndex].Style.ForeColor = Color.DarkOrange;
                            else
                                dgvNoiseRank[SpecificText.m_sException_Message, nIndex].Style.ForeColor = Color.Red;
                        }
                    }
                }

                dgvNoiseRank.AutoResizeColumns();
            });
        }

        private void OutputResultDataGridView_TiltTuning(TabControl tcTabControl, TabPage tpgTabPage, DataTable datatableData)
        {
            string[] sParameterName_Array = new string[] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sPTHF_H, 
                SpecificText.m_sPTHF_V, 
                SpecificText.m_sBHF_H, 
                SpecificText.m_sBHF_V, 
                SpecificText.m_sTotalScore 
            };

            string[] sErrorParameterName_Array = new string[] 
            { 
                SpecificText.m_sResult, 
                SpecificText.m_sPTHFExceptionMessage, 
                SpecificText.m_sBHFExceptionMessage 
            };

            DataGridViewContentAlignment[] dgvcaErrorParameterAlignment_Array = new DataGridViewContentAlignment[] 
            { 
                DataGridViewContentAlignment.MiddleCenter,
                DataGridViewContentAlignment.MiddleLeft,
                DataGridViewContentAlignment.MiddleLeft 
            };

            this.Invoke((MethodInvoker)delegate
            {
                tpgTabPage.Parent = tcTabControl;

                while (dgvTTRank.Rows.Count != 0)
                    dgvTTRank.Rows.RemoveAt(0);

                dgvTTRank.Columns.Clear();
                dgvTTRank.DataSource = datatableData;
                dgvTTRank.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvTTRank.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", (float)11);
                dgvTTRank.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvTTRank.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvTTRank.RowsDefaultCellStyle.Font = new Font("Times New Roman", (float)10);
                dgvTTRank.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                foreach (DataGridViewColumn dgvcColumn in dgvTTRank.Columns)
                {
                    if (dgvcColumn.Name == SpecificText.m_sException_Message)
                        dgvcColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    else
                        dgvcColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                dgvTTRank.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;

                // Grid Back ground Color
                dgvTTRank.BackgroundColor = Color.LightSteelBlue;

                // Grid Back Color
                dgvTTRank.RowsDefaultCellStyle.BackColor = Color.AliceBlue;

                // GridColumnStylesCollection Alternate Rows Back Color
                dgvTTRank.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

                // Column Header Back Color & Fore Color
                dgvTTRank.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
                dgvTTRank.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                for (int nColumnIndex = 0; nColumnIndex < dgvTTRank.Columns.Count; nColumnIndex++)
                {
                    dgvTTRank.Columns[nColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;

                    for (int nErrorParamIndex = 0; nErrorParamIndex < sErrorParameterName_Array.Length; nErrorParamIndex++)
                    {
                        if (dgvTTRank.Columns[nColumnIndex].HeaderText == sErrorParameterName_Array[nErrorParamIndex])
                            dgvTTRank.Columns[nColumnIndex].DefaultCellStyle.Alignment = dgvcaErrorParameterAlignment_Array[nErrorParamIndex];
                    }
                }

                for (int nParamIndex = 0; nParamIndex < sParameterName_Array.Length; nParamIndex++)
                {
                    if (dgvTTRank.Columns.Contains(sParameterName_Array[nParamIndex]) == true)
                    {
                        for (int nRowIndex = 0; nRowIndex < dgvTTRank.Rows.Count; nRowIndex++)
                        {
                            if (dgvTTRank[sParameterName_Array[nParamIndex], nRowIndex].Value.ToString() == "N/A")
                                dgvTTRank[sParameterName_Array[nParamIndex], nRowIndex].Style.ForeColor = Color.Red;
                        }
                    }
                }

                for (int nErrorParamIndex = 0; nErrorParamIndex < sErrorParameterName_Array.Length; nErrorParamIndex++)
                {
                    if (dgvTTRank.Columns.Contains(sErrorParameterName_Array[nErrorParamIndex]) == true)
                    {
                        for (int nRowIndex = 0; nRowIndex < dgvTTRank.Rows.Count; nRowIndex++)
                        {
                            if (dgvTTRank[sErrorParameterName_Array[nErrorParamIndex], nRowIndex].Value.ToString() != "")
                                dgvTTRank[sErrorParameterName_Array[nErrorParamIndex], nRowIndex].Style.ForeColor = Color.Red;
                        }
                    }
                }

                dgvTTRank.AutoResizeColumns();
            });
        }

        private void OutputRankDataGridView_TiltNoise(TabControl tcTabControl, TabPage tpgTabPage, DataGridView dgvDataGridView, DataTable datatableData)
        {
            this.Invoke((MethodInvoker)delegate
            {
                tpgTabPage.Parent = tcTabControl;

                while (dgvDataGridView.Rows.Count != 0)
                    dgvDataGridView.Rows.RemoveAt(0);

                dgvDataGridView.Columns.Clear();
                dgvDataGridView.DataSource = datatableData;
                dgvDataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 11);
                dgvDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvDataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvDataGridView.RowsDefaultCellStyle.Font = new Font("Times New Roman", (float)10);
                dgvDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                foreach (DataGridViewColumn dgvcColumn in dgvDataGridView.Columns)
                {
                    if (dgvcColumn.Name == SpecificText.m_sException_Message)
                        dgvcColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    else
                        dgvcColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                dgvDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;

                // Grid Back ground Color
                dgvDataGridView.BackgroundColor = Color.LightSteelBlue;

                // Grid Back Color
                dgvDataGridView.RowsDefaultCellStyle.BackColor = Color.AliceBlue;

                // GridColumnStylesCollection Alternate Rows Back Color
                dgvDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

                // Column Header Back Color & Fore Color
                dgvDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
                dgvDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                for (int nColumnIndex = 0; nColumnIndex < dgvDataGridView.Columns.Count; nColumnIndex++)
                {
                    dgvDataGridView.Columns[nColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;

                    if (nColumnIndex == dgvDataGridView.Columns.Count - 1)
                        dgvDataGridView.Columns[nColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }

                if (dgvDataGridView.Columns.Contains(SpecificText.m_sException_Message) == true)
                {
                    for (int nIndex = 0; nIndex < dgvDataGridView.Rows.Count; nIndex++)
                    {
                        if (dgvDataGridView[SpecificText.m_sException_Message, nIndex].Value.ToString() != "")
                        {
                            if (dgvDataGridView[SpecificText.m_sException_Message, nIndex].Value.ToString().Contains("Max Value") == true && dgvDataGridView[SpecificText.m_sException_Message, nIndex].Value.ToString().Contains("Over Warning") == true)
                                dgvDataGridView[SpecificText.m_sException_Message, nIndex].Style.ForeColor = Color.DarkOrange;
                            else
                                dgvDataGridView[SpecificText.m_sException_Message, nIndex].Style.ForeColor = Color.Red;
                        }
                    }
                }

                dgvDataGridView.AutoResizeColumns();
            });
        }

        private void OutputResultListView(TabControl tcTabControl,
                                          TabPage tpgTabPage,
                                          ListView lvListView,
                                          DataTable datatableData,
                                          string[] sParameterName_Array,
                                          string[] sParameterValue_Array,
                                          string[] sCanbeNA_Array = null,
                                          int nListType = m_nCommonList)
        {
            List<string> sNoneHex_List = new List<string>() 
            { 
                SpecificText.m_sFileName, 
                SpecificText.m_sPH1, 
                SpecificText.m_sPH2, 
                SpecificText.m_sFrequency_KHz, 
                SpecificText.m_sErrorMessage 
            };

            this.Invoke((MethodInvoker)delegate
            {
                tpgTabPage.Parent = tcTabControl;

                lvListView.View = View.Details;
                lvListView.BeginUpdate();

                lvListView.Columns.Add("Information", 120, HorizontalAlignment.Left);
                lvListView.Columns.Add("Value", 120, HorizontalAlignment.Left);
                lvListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                lvListView.ShowGroups = true;

                for (int nRowIndex = 0; nRowIndex < datatableData.Rows.Count; nRowIndex++)
                {
                    ListViewGroup lvgGroup = new ListViewGroup();
                    lvgGroup.Header = string.Format("Rank : {0}", datatableData.Rows[nRowIndex][SpecificText.m_sRanking].ToString());
                    lvgGroup.HeaderAlignment = HorizontalAlignment.Left;
                    lvListView.Groups.Add(lvgGroup);

                    for (int nParamIndex = 0; nParamIndex < sParameterName_Array.Length; nParamIndex++)
                    {
                        ListViewItem lviItem = new ListViewItem();
                        lviItem.UseItemStyleForSubItems = false;
                        lviItem.Text = string.Format("{0} = ", sParameterName_Array[nParamIndex]);
                        lviItem.Font = new Font("Times New Roman", 11F, FontStyle.Regular);

                        string sValue = datatableData.Rows[nRowIndex][sParameterValue_Array[nParamIndex]].ToString();
                        string sDisplayValue = sValue;

                        if (ElanConvert.CheckIsInt(sValue) == true && sNoneHex_List.Contains(sParameterName_Array[nParamIndex]) == false)
                        {
                            int nValue = 0;
                            Int32.TryParse(sValue, out nValue);
                            sDisplayValue = string.Format("{0} (0x{1})", sDisplayValue, nValue.ToString("x4").ToUpper());
                        }

                        lviItem.SubItems.Add(sDisplayValue).Name = "Value";
                        //lviItem.SubItems["Value"].Font = new Font("Times New Roman", 11, FontStyle.Regular);

                        if (sParameterName_Array[nParamIndex].Contains(SpecificText.m_sErrorMessage) == true && sValue != "")
                            lviItem.SubItems["Value"].ForeColor = Color.Red;

                        if (sCanbeNA_Array != null)
                        {
                            for (int nCanbrNAIndex = 0; nCanbrNAIndex < sCanbeNA_Array.Length; nCanbrNAIndex++)
                            {
                                if (sParameterName_Array[nParamIndex] == sCanbeNA_Array[nCanbrNAIndex])
                                {
                                    if (ElanConvert.CheckIsInt(sValue) == true)
                                    {
                                        if (Convert.ToInt32(sValue) < 0)
                                        {
                                            sValue = "N/A";
                                            lviItem.SubItems["Value"].ForeColor = Color.Red;
                                        }
                                    }
                                    else
                                    {
                                        sValue = "N/A";
                                        lviItem.SubItems["Value"].ForeColor = Color.Red;
                                    }

                                    break;
                                }
                            }
                        }

                        lvgGroup.Items.Add(lviItem);
                        lvListView.Items.Add(lviItem);
                    }
                }

                SetListViewWidth(lvListView, nListType);
                lvListView.EndUpdate();
            });
        }

        private void SetListViewWidth(ListView lvListView, int nListType = m_nCommonList)
        {
            double dParamNameColWidthPercent = 0.1;
            double dParamValueColWidthPercent = 0.9;

            int nWidth = 0;
            int nHeaderIndex = 0;

            foreach (ColumnHeader chHeader in lvListView.Columns)
            {
                if (nHeaderIndex == 0)
                    chHeader.Width = (int)(lvListView.Width * dParamNameColWidthPercent);
                else
                    chHeader.Width = (int)(lvListView.Width * dParamValueColWidthPercent);

                nWidth = chHeader.Width;

                //column items greatest width
                chHeader.Width = -1;

                if (nWidth > chHeader.Width)
                    chHeader.Width = nWidth;

                // column header width
                chHeader.Width = -2;

                if (nWidth > chHeader.Width)
                    chHeader.Width = nWidth;

                nHeaderIndex++;
            }
        }

        private void ModifyResultListView(TabControl tcTabControl,
                                          TabPage tpgTabPage,
                                          ListView lvListView,
                                          DataTable datatableData,
                                          string[] sParameterName_Array,
                                          string[] sParameterValue_Array,
                                          string[] sParameterMove_Array,
                                          string[] sCanbeNA_Array = null)
        {
            int nGroupIndex = 0;
            int nRowIndex = 0;

            this.Invoke((MethodInvoker)delegate
            {
                tpgTabPage.Parent = tcTabControl;

                for (int nParamIndex = 0; nParamIndex < sParameterName_Array.Length; nParamIndex++)
                {
                    ListViewItem lviItem = new ListViewItem();
                    lviItem.UseItemStyleForSubItems = false;
                    lviItem.Text = string.Format("{0} = ", sParameterName_Array[nParamIndex]);
                    lvListView.BeginUpdate();

                    lviItem.Group = lvListView.Groups[nGroupIndex];

                    string sValue = datatableData.Rows[nRowIndex][sParameterValue_Array[nParamIndex]].ToString();

                    lviItem.SubItems.Add(sValue).Name = "Value";

                    if (sParameterName_Array[nParamIndex].Contains(SpecificText.m_sErrorMessage) == true && sValue != "")
                        lviItem.SubItems["Value"].ForeColor = Color.Red;

                    if (sCanbeNA_Array != null)
                    {
                        for (int nCanbeNAIndex = 0; nCanbeNAIndex < sCanbeNA_Array.Length; nCanbeNAIndex++)
                        {
                            if (sParameterName_Array[nParamIndex] == sCanbeNA_Array[nCanbeNAIndex])
                            {
                                if (ElanConvert.CheckIsInt(sValue) == true)
                                {
                                    if (Convert.ToInt32(sValue) < 0)
                                    {
                                        sValue = "N/A";
                                        lviItem.SubItems["Value"].ForeColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    sValue = "N/A";
                                    lviItem.SubItems["Value"].ForeColor = Color.Red;
                                }

                                break;
                            }
                        }
                    }

                    lvListView.Items.Add(lviItem);
                    lvListView.EndUpdate();
                }

                lvListView.BeginUpdate();

                for (int nMoveIndex = 0; nMoveIndex < sParameterMove_Array.Length; nMoveIndex++)
                {
                    ListViewItem lviAboveItem = lvListView.Groups[nGroupIndex].ListView.FindItemWithText(string.Format("{0} = ", sParameterMove_Array[nMoveIndex]), false, 0);
                    ListViewItem lviMoveItem = lvListView.Groups[nGroupIndex].ListView.FindItemWithText(string.Format("{0} = ", sParameterName_Array[nMoveIndex]), false, 0);
                    int nAboveItemIndex = lviAboveItem.Index;
                    int nMoveItemIndex = lviMoveItem.Index;
                    ListViewItem[] arrlviInterItem = new ListViewItem[nMoveItemIndex - nAboveItemIndex - 1];
                    ListViewItem[] arrlviSecondInterItem = null;
                    int nItemCount = lvListView.Groups[nGroupIndex].Items.Count;
                    bool bLastFlag = false;

                    if (nMoveItemIndex < lvListView.Groups[nGroupIndex].Items.Count)
                    {
                        arrlviSecondInterItem = new ListViewItem[nItemCount - nMoveItemIndex];
                        bLastFlag = true;
                    }

                    int nCount = nMoveItemIndex - nAboveItemIndex - 2;

                    for (int nItemIndex = nMoveItemIndex - 1; nItemIndex > nAboveItemIndex; nItemIndex--)
                    {
                        arrlviInterItem[nCount] = lvListView.Groups[nGroupIndex].Items[nItemIndex];
                        lvListView.Groups[nGroupIndex].Items.RemoveAt(nItemIndex);
                        lvListView.Items.RemoveAt(nItemIndex);
                        nCount--;
                    }

                    if (bLastFlag == false)
                    {
                        nCount = nItemCount - nMoveItemIndex - 1;
                        nItemCount = lvListView.Groups[nGroupIndex].Items.Count;
                        ListViewItem lviSecondAboveItem = lvListView.Groups[nGroupIndex].ListView.FindItemWithText(string.Format("{0} = ", sParameterName_Array[nMoveIndex]), false, 0);
                        int nSecondAboveItemIndex = lviSecondAboveItem.Index;

                        for (int nItemIndex = nItemCount - 1; nItemIndex > nSecondAboveItemIndex; nItemIndex--)
                        {
                            arrlviSecondInterItem[nCount] = lvListView.Groups[nGroupIndex].Items[nItemIndex];
                            lvListView.Groups[nGroupIndex].Items.RemoveAt(nItemIndex);
                            lvListView.Items.RemoveAt(nItemIndex);
                            nCount--;
                        }
                    }

                    for (int nItemIndex = 0; nItemIndex < arrlviInterItem.Length; nItemIndex++)
                    {
                        lvListView.Groups[nGroupIndex].Items.Add(arrlviInterItem[nItemIndex]);
                        lvListView.Items.Add(arrlviInterItem[nItemIndex]);
                    }

                    if (bLastFlag == false)
                    {
                        for (int nItemIndex = 0; nItemIndex < arrlviSecondInterItem.Length; nItemIndex++)
                        {
                            lvListView.Groups[nGroupIndex].Items.Add(arrlviSecondInterItem[nItemIndex]);
                            lvListView.Items.Add(arrlviSecondInterItem[nItemIndex]);
                        }
                    }
                }

                lvListView.EndUpdate();
            });
        }

        public void OutputCostTimeGroupBox()
        {
            List<MainStepCostTimeInfo> cMainStep_List = new List<MainStepCostTimeInfo>();
            GetMainStepCostTimeInfo(ref cMainStep_List);

            int nLabelCount = cMainStep_List.Count;
            Label[] lblControl_Array = new Label[nLabelCount];

            int nHeight = 25;

            this.Invoke((MethodInvoker)delegate
            {
                for (int nLabelIndex = 0; nLabelIndex < nLabelCount; nLabelIndex++)
                {
                    lblControl_Array[nLabelIndex] = new Label();
                    gbxCostTime.Controls.Add(lblControl_Array[nLabelIndex]);
                    lblControl_Array[nLabelIndex].Text = cMainStep_List[nLabelIndex].m_sMainStepName;
                    lblControl_Array[nLabelIndex].Location = new Point(8, nHeight);
                    lblControl_Array[nLabelIndex].Size = new Size(180, 30);

                    lblControl_Array[nLabelIndex] = new Label();
                    gbxCostTime.Controls.Add(lblControl_Array[nLabelIndex]);
                    lblControl_Array[nLabelIndex].Text = cMainStep_List[nLabelIndex].m_sMainStepCostTime;
                    lblControl_Array[nLabelIndex].Location = new Point(198, nHeight);
                    lblControl_Array[nLabelIndex].Size = new Size(150, 30);

                    nHeight += 25;
                }
            });
        }

        private void GetMainStepCostTimeInfo(ref List<MainStepCostTimeInfo> cMainStep_List)
        {
            List<MainTuningStep> eMainStep_List = new List<MainTuningStep>();

            for (int nStepIndex = 0; nStepIndex < m_cFlowStep_List.Count; nStepIndex++)
            {
                if ((m_cFlowStep_List[nStepIndex].m_nSubStepState & MainConstantParameter.m_nSTEPLOCATION_LAST) != 0 ||
                    m_cFlowStep_List[nStepIndex].m_bLastStep == true)
                {
                    int nFindIndex = eMainStep_List.FindIndex(x => x == m_cFlowStep_List[nStepIndex].m_eMainStep);

                    if (nFindIndex == -1)
                    {
                        MainStepCostTimeInfo cMainStep_1 = new MainStepCostTimeInfo();
                        MainStepCostTimeInfo cMainStep_2 = new MainStepCostTimeInfo();

                        string sMainStepString = StringConvert.m_dictMainStepMappingTable[m_cFlowStep_List[nStepIndex].m_eMainStep];
                        cMainStep_2.m_eMainStep = cMainStep_1.m_eMainStep = m_cFlowStep_List[nStepIndex].m_eMainStep;
                        cMainStep_2.m_sMainStepName = cMainStep_1.m_sMainStepName = sMainStepString;

                        int nHours = m_cFlowStep_List[nStepIndex].m_nHours;
                        int nMinutes = m_cFlowStep_List[nStepIndex].m_nMinutes;
                        int nSeconds = m_cFlowStep_List[nStepIndex].m_nSeconds;

                        string sCostTime = "";

                        if (nHours < 0 || nMinutes < 0 || nSeconds < 0)
                            sCostTime += "N/A";
                        else
                        {
                            string sHour = nHours.ToString().PadLeft(2, '0');
                            string sMinute = nMinutes.ToString().PadLeft(2, '0');
                            string sSecond = nSeconds.ToString().PadLeft(2, '0');

                            sCostTime += string.Format("{0}hr:{1}m:{2}s", sHour, sMinute, sSecond);
                        }

                        cMainStep_1.m_sMainStepCostTime = string.Format(": {0}", sCostTime);
                        cMainStep_2.m_sMainStepCostTime = sCostTime;
                        cMainStep_List.Add(cMainStep_1);
                        eMainStep_List.Add(m_cFlowStep_List[nStepIndex].m_eMainStep);
                        m_cMainStepCostTimeInfo_List.Add(cMainStep_2);
                    }
                }
            }
        }

        private void OutputResultRichTextBox(TabControl tcTabControl,
                                             TabPage tpgTabPage,
                                             RichTextBox rtbxResult,
                                             DataTable datatableData,
                                             string[] sParameterName_Array,
                                             string[] sParameterValue_Array,
                                             string[] sCanbeNA_Array = null,
                                             int nListType = m_nCommonList,
                                             SubTuningStep eSubStep = SubTuningStep.ELSE)
        {
            this.Invoke((MethodInvoker)delegate
            {
                tpgTabPage.Parent = tcTabControl;

                for (int nDataIndex = 0; nDataIndex < datatableData.Rows.Count; nDataIndex++)
                {
                    bool bLastData = (nDataIndex == datatableData.Rows.Count - 1) ? true : false;

                    rtbxResult.SelectionColor = Color.Blue;
                    rtbxResult.AppendText(string.Format("== Rank : {0} ==========", datatableData.Rows[nDataIndex][SpecificText.m_sRanking].ToString()) + Environment.NewLine);

                    SetRichTextBoxData(rtbxResult, datatableData, sParameterName_Array, sParameterValue_Array, sCanbeNA_Array, bLastData, false, nDataIndex, eSubStep);

                    if (bLastData == false)
                        rtbxResult.AppendText(Environment.NewLine);
                }
            });
        }

        private void ModifyResultRichTextBox(TabControl tcTabControl,
                                             TabPage tpgTabPage,
                                             RichTextBox rtbxResult,
                                             DataTable datatableData,
                                             string[] sParameterName_Array,
                                             string[] sParameterValue_Array,
                                             string[] sParameterMove_Array,
                                             string[] sCanbeNA_Array = null)
        {
            this.Invoke((MethodInvoker)delegate
            {
                tpgTabPage.Parent = tcTabControl;

                SetRichTextBoxData(rtbxResult, datatableData, sParameterName_Array, sParameterValue_Array, sCanbeNA_Array, true, true);
            });
        }

        private void SetRichTextBoxData(RichTextBox rtbxResult,
                                        DataTable datatableDataTable,
                                        string[] sParameterName_Array,
                                        string[] sParameterValue_Array,
                                        string[] sCanbeNA_Array,
                                        bool bLastDataFlag,
                                        bool bModifyFlag = false,
                                        int nDataIndex = 0,
                                        SubTuningStep eSubStep = SubTuningStep.ELSE)
        {
            if (bModifyFlag == true)
            {
                string sLineString = Environment.NewLine;
                rtbxResult.AppendText(sLineString);
            }

            for (int nParamIndex = 0; nParamIndex < sParameterName_Array.Length; nParamIndex++)
            {
                string sSymbol = "";

                if (m_sEqualSymbol_List.Contains(sParameterName_Array[nParamIndex]))
                    sSymbol = "=";
                else
                    sSymbol = "\t.EQU\t";

                string sValue = datatableDataTable.Rows[nDataIndex][sParameterValue_Array[nParamIndex]].ToString();

                if (sParameterName_Array[nParamIndex].Contains(SpecificText.m_sErrorMessage) == true && sValue != "")
                    rtbxResult.SelectionColor = Color.Red;
                else
                    rtbxResult.SelectionColor = Color.Black;

                if (m_sEqualSymbol_List.Contains(sParameterName_Array[nParamIndex]) == false)
                {
                    if (CheckValueOutOfRange(eSubStep, sParameterValue_Array[nParamIndex], sValue) == true)
                        rtbxResult.SelectionColor = Color.Red;
                    else
                        rtbxResult.SelectionColor = Color.Black;
                }

                if (sCanbeNA_Array != null)
                {
                    foreach (string sCanbeNA in sCanbeNA_Array)
                    {
                        if (sParameterName_Array[nParamIndex] == sCanbeNA)
                        {
                            if (ElanConvert.CheckIsInt(sValue) == true)
                            {
                                if (Convert.ToInt32(sValue) < 0)
                                {
                                    sValue = "N/A";
                                    rtbxResult.SelectionColor = Color.Red;
                                }
                            }
                            else
                            {
                                sValue = "N/A";
                                rtbxResult.SelectionColor = Color.Red;
                            }

                            break;
                        }
                    }
                }

                if (ElanConvert.CheckIsInt(sValue) == true && m_sEqualSymbol_List.Contains(sParameterName_Array[nParamIndex]) == false)
                {
                    int nValue = 0;
                    Int32.TryParse(sValue, out nValue);
                    sValue = string.Format("0x{1}", sValue, nValue.ToString("x").ToUpper());
                }

                string sText = string.Format("{0} {1} {2}", sParameterName_Array[nParamIndex], sSymbol, sValue);

                if (bLastDataFlag == false || nParamIndex < sParameterName_Array.Length - 1)
                    sText += Environment.NewLine;

                rtbxResult.AppendText(sText);
            }
        }

        private bool CheckValueOutOfRange(SubTuningStep eSubStep, string sParameterName, string sValue)
        {
            if (eSubStep == SubTuningStep.DIGIGAIN)
            {
                string[] sSymbol_Array = sValue.Split('*');

                if (sSymbol_Array != null && sSymbol_Array.Length >= 2)
                {
                    int nHighBoundary = ParamAutoTuning.m_nDGTDigiGainScaleHB;
                    int nLowBoundary = ParamAutoTuning.m_nDGTDigiGainScaleLB;
                    int nDigiGainScale = Convert.ToInt32(sSymbol_Array[0]);

                    if (nDigiGainScale > nHighBoundary || nDigiGainScale < nLowBoundary)
                        return true;
                }
            }

            return false;
        }

        public void SetResultMessagePanelFocus()
        {
            this.Invoke((MethodInvoker)delegate
            {
                pnlResultMessage.Focus();
            });
        }
    }
}
