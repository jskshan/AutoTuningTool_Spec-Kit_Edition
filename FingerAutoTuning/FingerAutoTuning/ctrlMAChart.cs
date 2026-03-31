using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    public partial class ctrlMAChart : UserControl
    {
        private frmMultiAnalysis.StepEnum m_eCurrentStep;
        private frmMultiAnalysis.AnalysisInfo m_cAnalysisInfo;
        private Chart m_cChart;
        private string m_sFilePath;
        private string m_sAnalysisErrorMessage = "";

        private int m_nChartWidth = 1500;
        private int m_nChartHeight = 500;
        private double m_dFrequencyLB = 0.0;
        private double m_dFrequencyHB = 0.0;
        private double m_dValueLB = 0.0;
        private double m_dValueHB = 0.0;

        public ctrlMAChart(frmMultiAnalysis.StepEnum eCurrentStep, frmMultiAnalysis.AnalysisInfo cAnalysisInfo)
        {
            InitializeComponent();

            m_eCurrentStep = eCurrentStep;
            m_cAnalysisInfo = cAnalysisInfo;
        }

        private void ctrlMAChart_Load(object sender, EventArgs e)
        {
            GenerateChart();
            SetTextBox();

            if (SaveChart("") == false)
            {
                MessageBox.Show(m_sAnalysisErrorMessage);
                return;
            }

            DisplayChart();
        }

        private void GenerateChart(bool bInitialChart = true)
        {
            string sValueName = "";

            if (m_eCurrentStep == frmMultiAnalysis.StepEnum.FrequencyRank_Phase2)
                sValueName = "ReferenceValue";
            else if (m_eCurrentStep == frmMultiAnalysis.StepEnum.AC_FrequencyRank)
                sValueName = "SNR(dB)";

            string sTitleName = string.Format("{0} Distribution By Frequency", sValueName);

            double dBestRankLB = 0.0;
            frmMultiAnalysis.m_nCompareOperator = frmMultiAnalysis.m_nCOMPARE_SCORE;
            m_cAnalysisInfo.m_cFrequencyAnalysisValue_List.Sort(new frmMultiAnalysis.SortDataComparer());

            if (m_cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count < ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber)
                dBestRankLB = m_cAnalysisInfo.m_cFrequencyAnalysisValue_List[m_cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count - 1].m_dScore;
            else
                dBestRankLB = m_cAnalysisInfo.m_cFrequencyAnalysisValue_List[ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber - 1].m_dScore;

            int nValueInterval = 10;

            if (bInitialChart == true)
            {
                if (m_eCurrentStep == frmMultiAnalysis.StepEnum.FrequencyRank_Phase2)
                {
                    double dMaxValue = m_cAnalysisInfo.m_cFrequencyAnalysisValue_List[0].m_dScore;
                    m_dValueHB = (int)(dMaxValue / nValueInterval) * nValueInterval + nValueInterval;

                    double dMinValue = m_cAnalysisInfo.m_cFrequencyAnalysisValue_List[m_cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count - 1].m_dScore;
                    m_dValueLB = (int)(dMinValue / nValueInterval) * nValueInterval - nValueInterval;
                }
                else if (m_eCurrentStep == frmMultiAnalysis.StepEnum.AC_FrequencyRank)
                {
                    double dMaxValue = 0.0;
                    double dMinValue = 0.0;
                    bool bFirstFlag = true;

                    foreach (frmMultiAnalysis.FrequencyAnalysisValue cFrequencyAnalysisValue in m_cAnalysisInfo.m_cFrequencyAnalysisValue_List)
                    {
                        if (bFirstFlag == true)
                        {
                            double dValue = Math.Max(cFrequencyAnalysisValue.m_dACScore, cFrequencyAnalysisValue.m_dLCMScore);
                            dValue = Math.Max(dValue, cFrequencyAnalysisValue.m_dScore);
                            dMaxValue = dValue;

                            dValue = Math.Min(cFrequencyAnalysisValue.m_dACScore, cFrequencyAnalysisValue.m_dLCMScore);
                            dValue = Math.Min(dValue, cFrequencyAnalysisValue.m_dScore);
                            dMinValue = dValue;
                            bFirstFlag = false;
                        }
                        else
                        {
                            double dValue = Math.Max(cFrequencyAnalysisValue.m_dACScore, cFrequencyAnalysisValue.m_dLCMScore);
                            dValue = Math.Max(dValue, cFrequencyAnalysisValue.m_dScore);

                            if (dValue > dMaxValue)
                                dMaxValue = dValue;

                            dValue = Math.Min(cFrequencyAnalysisValue.m_dACScore, cFrequencyAnalysisValue.m_dLCMScore);
                            dValue = Math.Min(dValue, cFrequencyAnalysisValue.m_dScore);

                            if (dValue < dMinValue)
                                dMinValue = dValue;
                        }
                    }

                    m_dValueHB = (int)(dMaxValue / nValueInterval) * nValueInterval + nValueInterval;
                    m_dValueLB = (int)(dMinValue / nValueInterval) * nValueInterval - nValueInterval;
                }
            }

            frmMultiAnalysis.m_nCompareOperator = frmMultiAnalysis.m_nCOMPARE_FREQUENCY;
            m_cAnalysisInfo.m_cFrequencyAnalysisValue_List.Sort(new frmMultiAnalysis.SortDataComparer());

            int nFreqInterval = 5;

            if (bInitialChart == true)
            {
                double dMaxFrequency = 0.0;
                double dMinFrequency = 0.0;

                for (int nIndex = 0; nIndex < m_cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count; nIndex++)
                {
                    frmMultiAnalysis.FrequencyAnalysisValue cFrequencyAnalysisValue = m_cAnalysisInfo.m_cFrequencyAnalysisValue_List[nIndex];

                    if (nIndex == 0)
                    {
                        dMaxFrequency = cFrequencyAnalysisValue.m_dFrequency;
                        dMinFrequency = cFrequencyAnalysisValue.m_dFrequency;
                    }
                    else
                    {
                        if (cFrequencyAnalysisValue.m_dFrequency > dMaxFrequency)
                            dMaxFrequency = cFrequencyAnalysisValue.m_dFrequency;

                        if (cFrequencyAnalysisValue.m_dFrequency < dMinFrequency)
                            dMinFrequency = cFrequencyAnalysisValue.m_dFrequency;
                    }
                }

                int nMinFrequency = (int)(dMinFrequency / nFreqInterval) * nFreqInterval;
                m_dFrequencyLB = nMinFrequency;

                double dDiffer = dMaxFrequency - (double)nMinFrequency;
                int nPart = (int)(dDiffer / nFreqInterval);

                if (dDiffer % nFreqInterval != 0)
                    nPart++;

                int nMaxFrequency = nMinFrequency + nPart * nFreqInterval;
                m_dFrequencyHB = nMaxFrequency;
            }

            Series cSeries = new Series(string.Format("{0} vs. Frequency", sValueName));
            Series cSeries_AC = null;
            Series cSeries_LCM = null;

            if (m_eCurrentStep == frmMultiAnalysis.StepEnum.AC_FrequencyRank)
            {
                cSeries_AC = new Series("AC SNR(dB) vs. Frequency");
                cSeries_LCM = new Series("LCM SNR(dB) vs. Frequency");
            }

            //Show Line Chart
            m_cChart = new Chart();
            var varChartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            m_cChart.ChartAreas.Add(varChartArea);
            m_cChart.Width = m_nChartWidth;
            m_cChart.Height = m_nChartHeight;
            m_cChart.Legends.Add("Legend");
            m_cChart.Legends["Legend"].Font = new Font("Times New Roman", 10);
            m_cChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
            m_cChart.Titles.Add(sTitleName);
            m_cChart.Titles[0].Font = new Font("Times New Roman", 18);
            m_cChart.ChartAreas[0].AxisY.Title = sValueName;
            m_cChart.ChartAreas[0].AxisY.TitleFont = new Font("Times New Roman", 12);
            m_cChart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Times New Roman", 11);
            m_cChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            m_cChart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            m_cChart.ChartAreas[0].AxisY.Minimum = m_dValueLB;
            m_cChart.ChartAreas[0].AxisY.Maximum = m_dValueHB;
            m_cChart.ChartAreas[0].AxisY.Interval = nValueInterval;
            m_cChart.ChartAreas[0].AxisX.Title = "Frequency(KHz)";
            m_cChart.ChartAreas[0].AxisX.TitleFont = new Font("Times New Roman", 12);
            m_cChart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Times New Roman", 10);
            //m_cChart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //m_cChart.ChartAreas[0].AxisX.IsLabelAutoFit = false;
            //m_cChart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;
            m_cChart.ChartAreas[0].AxisX.Minimum = m_dFrequencyLB;
            m_cChart.ChartAreas[0].AxisX.Maximum = m_dFrequencyHB;
            m_cChart.ChartAreas[0].AxisX.Interval = nFreqInterval;
            m_cChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            m_cChart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            cSeries.ChartType = SeriesChartType.Line;
            cSeries.MarkerStyle = MarkerStyle.Circle;
            cSeries.MarkerSize = 5;
            cSeries.IsValueShownAsLabel = false;
            cSeries.Color = Color.Blue;

            if (m_eCurrentStep == frmMultiAnalysis.StepEnum.AC_FrequencyRank)
            {
                cSeries_AC.ChartType = SeriesChartType.Line;
                cSeries_AC.MarkerStyle = MarkerStyle.Circle;
                cSeries_AC.MarkerSize = 5;
                cSeries_AC.IsValueShownAsLabel = false;
                cSeries_AC.Color = Color.Green;

                cSeries_LCM.ChartType = SeriesChartType.Line;
                cSeries_LCM.MarkerStyle = MarkerStyle.Circle;
                cSeries_LCM.MarkerSize = 5;
                cSeries_LCM.IsValueShownAsLabel = false;
                cSeries_LCM.Color = Color.Orange;
            }

            for (int nIndex = 0; nIndex < m_cAnalysisInfo.m_cFrequencyAnalysisValue_List.Count; nIndex++)
            {
                frmMultiAnalysis.FrequencyAnalysisValue cFrequencyAnalysisValue = m_cAnalysisInfo.m_cFrequencyAnalysisValue_List[nIndex];
                DataPoint cLine = cSeries.Points[cSeries.Points.AddXY(cFrequencyAnalysisValue.m_dFrequency, cFrequencyAnalysisValue.m_dScore)];
                DataPoint cLine_AC = null;
                DataPoint cLine_LCM = null;

                if (m_eCurrentStep == frmMultiAnalysis.StepEnum.AC_FrequencyRank)
                {
                    cLine_AC = cSeries_AC.Points[cSeries_AC.Points.AddXY(cFrequencyAnalysisValue.m_dFrequency, cFrequencyAnalysisValue.m_dACScore)];
                    cLine_LCM = cSeries_LCM.Points[cSeries_LCM.Points.AddXY(cFrequencyAnalysisValue.m_dFrequency, cFrequencyAnalysisValue.m_dLCMScore)];
                }

                int nFindIndex = m_cAnalysisInfo.m_cFrequencyAnalysisValue_List.FindIndex(x => x.m_dFrequency == cFrequencyAnalysisValue.m_dFrequency);

                if (cFrequencyAnalysisValue.m_dScore >= dBestRankLB)
                {
                    cLine.MarkerColor = Color.Red;
                    cLine.Label = string.Format("{0}KHz", cFrequencyAnalysisValue.m_dFrequency);
                    cLine.IsValueShownAsLabel = true;
                }
                else
                    cLine.MarkerColor = Color.Blue;

                cLine.Color = Color.Blue;

                if (m_eCurrentStep == frmMultiAnalysis.StepEnum.AC_FrequencyRank)
                {
                    cLine_AC.MarkerColor = Color.Green;
                    cLine_LCM.MarkerColor = Color.Orange;

                    cLine_AC.Color = Color.Green;
                    cLine_LCM.Color = Color.Orange;
                }
            }

            m_cChart.Series.Add(cSeries);

            if (m_eCurrentStep == frmMultiAnalysis.StepEnum.AC_FrequencyRank)
            {
                m_cChart.Series.Add(cSeries_AC);
                m_cChart.Series.Add(cSeries_LCM);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.InitialDirectory = Application.StartupPath;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FilterIndex = 1;
            //saveFileDialog.CheckFileExists = true;
            saveFileDialog.CheckPathExists = true;

            DialogResult dr = saveFileDialog.ShowDialog();

            if (dr != System.Windows.Forms.DialogResult.OK)
                return;

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog.FileName != "")
            {
                if (SaveChart(saveFileDialog.FileName, false) == false)
                    MessageBox.Show(m_sAnalysisErrorMessage);
            }
        }

        private bool SaveChart(string sSaveFilePath, bool bFixedPath = true)
        {
            bool bErrorFlag = false;

            if (bFixedPath == true)
            {
                string sDirecotryPath = string.Format(@"{0}\{1}\Img", Application.StartupPath, frmMain.m_sAPMainDirectoryName);

                if (Directory.Exists(sDirecotryPath) == false)
                    Directory.CreateDirectory(sDirecotryPath);

                m_sFilePath = string.Format(@"{0}\Chart.jpg", sDirecotryPath);
                sSaveFilePath = m_sFilePath;
            }

            try
            {
                m_cChart.SaveImage(sSaveFilePath, ChartImageFormat.Jpeg);
            }
            catch
            {
                m_sAnalysisErrorMessage = "Save Chart File Error!";
                bErrorFlag = true;
            }

            if (bFixedPath == false && bErrorFlag == false)
                MessageBox.Show("Save Chart File Complete!");

            return !bErrorFlag;
        }

        private void DisplayChart()
        {
            FileStream fs = new FileStream(m_sFilePath, FileMode.Open, FileAccess.Read);
            pbxChart.Image = System.Drawing.Image.FromStream(fs);
            fs.Close();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            SetParamValue();

            GenerateChart(false);

            if (SaveChart("") == false)
                return;

            DisplayChart();
        }

        private void SetTextBox()
        {
            tbxChartWidth.Text = m_nChartWidth.ToString("D");
            tbxChartHeight.Text = m_nChartHeight.ToString("D");

            tbxMinFrequency.Text = m_dFrequencyLB.ToString();
            tbxMaxFrequency.Text = m_dFrequencyHB.ToString();

            tbxMinValue.Text = m_dValueLB.ToString();
            tbxMaxValue.Text = m_dValueHB.ToString();
        }

        private void SetParamValue()
        {
            SetIntegerParameter(ref m_nChartWidth, tbxChartWidth.Text);
            SetIntegerParameter(ref m_nChartHeight, tbxChartHeight.Text);

            SetDoubleParameter(ref m_dFrequencyLB, tbxMinFrequency.Text);
            SetDoubleParameter(ref m_dFrequencyHB, tbxMaxFrequency.Text);

            if (m_dFrequencyLB > m_dFrequencyHB)
            {
                double dValue = m_dFrequencyLB;
                m_dFrequencyLB = m_dFrequencyHB;
                m_dFrequencyHB = dValue;
            }

            SetDoubleParameter(ref m_dValueLB, tbxMinValue.Text, true);
            SetDoubleParameter(ref m_dValueHB, tbxMaxValue.Text, true);

            if (m_dValueLB > m_dValueHB)
            {
                double dValue = m_dFrequencyLB;
                m_dValueLB = m_dValueHB;
                m_dValueHB = dValue;
            }
        }

        private void SetIntegerParameter(ref int nOutputValue, string sInputString)
        {
            if (ElanConvert.IsInt(sInputString))
            {
                int nValue = 0;
                Int32.TryParse(sInputString, out nValue);

                if (nValue > 0)
                    nOutputValue = nValue;
            }
        }

        private void SetDoubleParameter(ref double dOutputValue, string sInputString, bool bNegative = false)
        {
            if (ElanConvert.IsDouble(sInputString))
            {
                double dValue = 0.0;
                Double.TryParse(sInputString, out dValue);

                if (bNegative == false)
                {
                    if (dValue > 0)
                        dOutputValue = dValue;
                }
                else
                    dOutputValue = dValue;
            }
        }
    }
}
