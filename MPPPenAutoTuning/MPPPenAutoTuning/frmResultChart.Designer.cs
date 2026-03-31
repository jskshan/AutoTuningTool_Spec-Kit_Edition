namespace MPPPenAutoTuning
{
    partial class frmResultChart
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmResultChart));
            this.tcNoise = new System.Windows.Forms.TabControl();
            this.tpgNoiseTraceLineChart = new System.Windows.Forms.TabPage();
            this.lblNoiseTraceLineChartData = new System.Windows.Forms.Label();
            this.cbxNoiseTraceLineChartData = new System.Windows.Forms.ComboBox();
            this.scNoiseTraceLineChart = new System.Windows.Forms.SplitContainer();
            this.picbxNoiseRXTrace = new System.Windows.Forms.PictureBox();
            this.picbxNoiseTXTrace = new System.Windows.Forms.PictureBox();
            this.tpgNoiseFrequencyLineChart = new System.Windows.Forms.TabPage();
            this.ckbxNoiseFreqChartIncludeMaxValue = new System.Windows.Forms.CheckBox();
            this.tcNoiseFrequency = new System.Windows.Forms.TabControl();
            this.tpgNoiseRXFrequency = new System.Windows.Forms.TabPage();
            this.scNoiseRXFreqLineChart = new System.Windows.Forms.SplitContainer();
            this.picbxNoiseRXInnerFreq = new System.Windows.Forms.PictureBox();
            this.picbxNoiseRXEdgeFreq = new System.Windows.Forms.PictureBox();
            this.tpgNoiseTXFrequency = new System.Windows.Forms.TabPage();
            this.scNoiseTXFreqLineChart = new System.Windows.Forms.SplitContainer();
            this.picbxNoiseTXInnerFreq = new System.Windows.Forms.PictureBox();
            this.picbxNoiseTXEdgeFreq = new System.Windows.Forms.PictureBox();
            this.tpgNoiseTotalFrequency = new System.Windows.Forms.TabPage();
            this.picbxNoiseTotalFreq = new System.Windows.Forms.PictureBox();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpgNoise = new System.Windows.Forms.TabPage();
            this.tpgTiltNoise = new System.Windows.Forms.TabPage();
            this.tcTiltNoise = new System.Windows.Forms.TabControl();
            this.tpgTNTraceLineChart = new System.Windows.Forms.TabPage();
            this.lblTNTraceLineChartData = new System.Windows.Forms.Label();
            this.cbxTNTraceLineChartData = new System.Windows.Forms.ComboBox();
            this.scTNTraceLineChart = new System.Windows.Forms.SplitContainer();
            this.picbxTNRXTrace = new System.Windows.Forms.PictureBox();
            this.picbxTNTXTrace = new System.Windows.Forms.PictureBox();
            this.tpgTNFrequencyLineChart = new System.Windows.Forms.TabPage();
            this.ckbxTNFrequencyLineIncludeMaxValue = new System.Windows.Forms.CheckBox();
            this.tcTNFrequency = new System.Windows.Forms.TabControl();
            this.tpgTNPTHFRXFreq = new System.Windows.Forms.TabPage();
            this.scTNPTHFRXFreqLineChart = new System.Windows.Forms.SplitContainer();
            this.picbxTNPTHFRXInnerFreq = new System.Windows.Forms.PictureBox();
            this.picbxTNPTHFRXEdgeFreq = new System.Windows.Forms.PictureBox();
            this.tpgTNPTHFTXFreq = new System.Windows.Forms.TabPage();
            this.scTNPTHFTXFreqLineChart = new System.Windows.Forms.SplitContainer();
            this.picbxTNPTHFTXInnerFreq = new System.Windows.Forms.PictureBox();
            this.picbxTNPTHFTXEdgeFreq = new System.Windows.Forms.PictureBox();
            this.tpgTNPTHFFreq = new System.Windows.Forms.TabPage();
            this.picbxTNPTHFFreq = new System.Windows.Forms.PictureBox();
            this.tpgTNBHFRXFreq = new System.Windows.Forms.TabPage();
            this.scTNBHFRXFreqLineChart = new System.Windows.Forms.SplitContainer();
            this.picbxTNBHFRXInnerFreq = new System.Windows.Forms.PictureBox();
            this.picbxTNBHFRXEdgeFreq = new System.Windows.Forms.PictureBox();
            this.tpgTNBHFTXFreq = new System.Windows.Forms.TabPage();
            this.scTNBHFTXFreqLineChart = new System.Windows.Forms.SplitContainer();
            this.picbxTNBHFTXInnerFreq = new System.Windows.Forms.PictureBox();
            this.picbxTNBHFTXEdgeFreq = new System.Windows.Forms.PictureBox();
            this.tpgTNBHFFreq = new System.Windows.Forms.TabPage();
            this.picbxTNBHFFreq = new System.Windows.Forms.PictureBox();
            this.tpgTNRXFreq = new System.Windows.Forms.TabPage();
            this.scTNTotalRXLineChart = new System.Windows.Forms.SplitContainer();
            this.picbxTNTotalRXInnerFreq = new System.Windows.Forms.PictureBox();
            this.picbxTNTotalRXEdgeFreq = new System.Windows.Forms.PictureBox();
            this.tpgTNFreq = new System.Windows.Forms.TabPage();
            this.scTNTotalTXFreqLineChart = new System.Windows.Forms.SplitContainer();
            this.picbxTNTotalTXInnerFreq = new System.Windows.Forms.PictureBox();
            this.picbxTNTotalTXEdgeFreq = new System.Windows.Forms.PictureBox();
            this.tpgTNTotalFreq = new System.Windows.Forms.TabPage();
            this.picbxTNTotalFreq = new System.Windows.Forms.PictureBox();
            this.tpgCompositeLineChart = new System.Windows.Forms.TabPage();
            this.ckbxTNCompositeLineIncludeMaxValue = new System.Windows.Forms.CheckBox();
            this.picbxCompositeLineChart = new System.Windows.Forms.PictureBox();
            this.tpgDigitalTuning = new System.Windows.Forms.TabPage();
            this.tcDigitalTuning = new System.Windows.Forms.TabControl();
            this.tpgDTHover1stHisto = new System.Windows.Forms.TabPage();
            this.lblDTHover1stDataName = new System.Windows.Forms.Label();
            this.cbxDTHover1stDataName = new System.Windows.Forms.ComboBox();
            this.scDTHover1stHisto = new System.Windows.Forms.SplitContainer();
            this.picbxDTHover1stRXHisto = new System.Windows.Forms.PictureBox();
            this.picbxDTHover1stTXHisto = new System.Windows.Forms.PictureBox();
            this.tpgDTHover2ndHisto = new System.Windows.Forms.TabPage();
            this.scDTHover2ndHisto = new System.Windows.Forms.SplitContainer();
            this.picbxDTHover2ndRXHisto = new System.Windows.Forms.PictureBox();
            this.picbxDTHover2ndTXHisto = new System.Windows.Forms.PictureBox();
            this.lblDTHover2ndDataName = new System.Windows.Forms.Label();
            this.cbxDTHover2ndDataName = new System.Windows.Forms.ComboBox();
            this.tpgDTContactHisto = new System.Windows.Forms.TabPage();
            this.lblDTContactDataName = new System.Windows.Forms.Label();
            this.cbxDTContactDataName = new System.Windows.Forms.ComboBox();
            this.scDTContactHisto = new System.Windows.Forms.SplitContainer();
            this.picbxDTContactRXHisto = new System.Windows.Forms.PictureBox();
            this.picbxDTContactTXHisto = new System.Windows.Forms.PictureBox();
            this.tpgDTHoverTRxS = new System.Windows.Forms.TabPage();
            this.lblDTHoverTRxSDataName = new System.Windows.Forms.Label();
            this.cbxDTHoverTRxSDataName = new System.Windows.Forms.ComboBox();
            this.scDTHoverTRxS = new System.Windows.Forms.SplitContainer();
            this.picbxDTHoverTRxSRX = new System.Windows.Forms.PictureBox();
            this.picbxDTHoverTRxSTX = new System.Windows.Forms.PictureBox();
            this.tpgDTContactTRxS = new System.Windows.Forms.TabPage();
            this.lblDTContactTRxSDataName = new System.Windows.Forms.Label();
            this.cbxDTContactTRxSDataName = new System.Windows.Forms.ComboBox();
            this.scDTContactTRxS = new System.Windows.Forms.SplitContainer();
            this.picbxDTContactTRxSRX = new System.Windows.Forms.PictureBox();
            this.picbxDTContactTRxSTX = new System.Windows.Forms.PictureBox();
            this.tcNoise.SuspendLayout();
            this.tpgNoiseTraceLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scNoiseTraceLineChart)).BeginInit();
            this.scNoiseTraceLineChart.Panel1.SuspendLayout();
            this.scNoiseTraceLineChart.Panel2.SuspendLayout();
            this.scNoiseTraceLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseRXTrace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseTXTrace)).BeginInit();
            this.tpgNoiseFrequencyLineChart.SuspendLayout();
            this.tcNoiseFrequency.SuspendLayout();
            this.tpgNoiseRXFrequency.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scNoiseRXFreqLineChart)).BeginInit();
            this.scNoiseRXFreqLineChart.Panel1.SuspendLayout();
            this.scNoiseRXFreqLineChart.Panel2.SuspendLayout();
            this.scNoiseRXFreqLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseRXInnerFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseRXEdgeFreq)).BeginInit();
            this.tpgNoiseTXFrequency.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scNoiseTXFreqLineChart)).BeginInit();
            this.scNoiseTXFreqLineChart.Panel1.SuspendLayout();
            this.scNoiseTXFreqLineChart.Panel2.SuspendLayout();
            this.scNoiseTXFreqLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseTXInnerFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseTXEdgeFreq)).BeginInit();
            this.tpgNoiseTotalFrequency.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseTotalFreq)).BeginInit();
            this.tcMain.SuspendLayout();
            this.tpgNoise.SuspendLayout();
            this.tpgTiltNoise.SuspendLayout();
            this.tcTiltNoise.SuspendLayout();
            this.tpgTNTraceLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTNTraceLineChart)).BeginInit();
            this.scTNTraceLineChart.Panel1.SuspendLayout();
            this.scTNTraceLineChart.Panel2.SuspendLayout();
            this.scTNTraceLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNRXTrace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTXTrace)).BeginInit();
            this.tpgTNFrequencyLineChart.SuspendLayout();
            this.tcTNFrequency.SuspendLayout();
            this.tpgTNPTHFRXFreq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTNPTHFRXFreqLineChart)).BeginInit();
            this.scTNPTHFRXFreqLineChart.Panel1.SuspendLayout();
            this.scTNPTHFRXFreqLineChart.Panel2.SuspendLayout();
            this.scTNPTHFRXFreqLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNPTHFRXInnerFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNPTHFRXEdgeFreq)).BeginInit();
            this.tpgTNPTHFTXFreq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTNPTHFTXFreqLineChart)).BeginInit();
            this.scTNPTHFTXFreqLineChart.Panel1.SuspendLayout();
            this.scTNPTHFTXFreqLineChart.Panel2.SuspendLayout();
            this.scTNPTHFTXFreqLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNPTHFTXInnerFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNPTHFTXEdgeFreq)).BeginInit();
            this.tpgTNPTHFFreq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNPTHFFreq)).BeginInit();
            this.tpgTNBHFRXFreq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTNBHFRXFreqLineChart)).BeginInit();
            this.scTNBHFRXFreqLineChart.Panel1.SuspendLayout();
            this.scTNBHFRXFreqLineChart.Panel2.SuspendLayout();
            this.scTNBHFRXFreqLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNBHFRXInnerFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNBHFRXEdgeFreq)).BeginInit();
            this.tpgTNBHFTXFreq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTNBHFTXFreqLineChart)).BeginInit();
            this.scTNBHFTXFreqLineChart.Panel1.SuspendLayout();
            this.scTNBHFTXFreqLineChart.Panel2.SuspendLayout();
            this.scTNBHFTXFreqLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNBHFTXInnerFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNBHFTXEdgeFreq)).BeginInit();
            this.tpgTNBHFFreq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNBHFFreq)).BeginInit();
            this.tpgTNRXFreq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTNTotalRXLineChart)).BeginInit();
            this.scTNTotalRXLineChart.Panel1.SuspendLayout();
            this.scTNTotalRXLineChart.Panel2.SuspendLayout();
            this.scTNTotalRXLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTotalRXInnerFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTotalRXEdgeFreq)).BeginInit();
            this.tpgTNFreq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTNTotalTXFreqLineChart)).BeginInit();
            this.scTNTotalTXFreqLineChart.Panel1.SuspendLayout();
            this.scTNTotalTXFreqLineChart.Panel2.SuspendLayout();
            this.scTNTotalTXFreqLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTotalTXInnerFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTotalTXEdgeFreq)).BeginInit();
            this.tpgTNTotalFreq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTotalFreq)).BeginInit();
            this.tpgCompositeLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxCompositeLineChart)).BeginInit();
            this.tpgDigitalTuning.SuspendLayout();
            this.tcDigitalTuning.SuspendLayout();
            this.tpgDTHover1stHisto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDTHover1stHisto)).BeginInit();
            this.scDTHover1stHisto.Panel1.SuspendLayout();
            this.scDTHover1stHisto.Panel2.SuspendLayout();
            this.scDTHover1stHisto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHover1stRXHisto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHover1stTXHisto)).BeginInit();
            this.tpgDTHover2ndHisto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDTHover2ndHisto)).BeginInit();
            this.scDTHover2ndHisto.Panel1.SuspendLayout();
            this.scDTHover2ndHisto.Panel2.SuspendLayout();
            this.scDTHover2ndHisto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHover2ndRXHisto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHover2ndTXHisto)).BeginInit();
            this.tpgDTContactHisto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDTContactHisto)).BeginInit();
            this.scDTContactHisto.Panel1.SuspendLayout();
            this.scDTContactHisto.Panel2.SuspendLayout();
            this.scDTContactHisto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTContactRXHisto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTContactTXHisto)).BeginInit();
            this.tpgDTHoverTRxS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDTHoverTRxS)).BeginInit();
            this.scDTHoverTRxS.Panel1.SuspendLayout();
            this.scDTHoverTRxS.Panel2.SuspendLayout();
            this.scDTHoverTRxS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHoverTRxSRX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHoverTRxSTX)).BeginInit();
            this.tpgDTContactTRxS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDTContactTRxS)).BeginInit();
            this.scDTContactTRxS.Panel1.SuspendLayout();
            this.scDTContactTRxS.Panel2.SuspendLayout();
            this.scDTContactTRxS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTContactTRxSRX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTContactTRxSTX)).BeginInit();
            this.SuspendLayout();
            // 
            // tcNoise
            // 
            this.tcNoise.Controls.Add(this.tpgNoiseTraceLineChart);
            this.tcNoise.Controls.Add(this.tpgNoiseFrequencyLineChart);
            this.tcNoise.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcNoise.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcNoise.Location = new System.Drawing.Point(3, 3);
            this.tcNoise.Name = "tcNoise";
            this.tcNoise.SelectedIndex = 0;
            this.tcNoise.Size = new System.Drawing.Size(1181, 598);
            this.tcNoise.TabIndex = 0;
            // 
            // tpgNoiseTraceLineChart
            // 
            this.tpgNoiseTraceLineChart.BackColor = System.Drawing.SystemColors.Control;
            this.tpgNoiseTraceLineChart.Controls.Add(this.lblNoiseTraceLineChartData);
            this.tpgNoiseTraceLineChart.Controls.Add(this.cbxNoiseTraceLineChartData);
            this.tpgNoiseTraceLineChart.Controls.Add(this.scNoiseTraceLineChart);
            this.tpgNoiseTraceLineChart.Location = new System.Drawing.Point(4, 26);
            this.tpgNoiseTraceLineChart.Name = "tpgNoiseTraceLineChart";
            this.tpgNoiseTraceLineChart.Padding = new System.Windows.Forms.Padding(3);
            this.tpgNoiseTraceLineChart.Size = new System.Drawing.Size(1173, 568);
            this.tpgNoiseTraceLineChart.TabIndex = 0;
            this.tpgNoiseTraceLineChart.Text = "Trace Line Chart";
            // 
            // lblNoiseTraceLineChartData
            // 
            this.lblNoiseTraceLineChartData.AutoSize = true;
            this.lblNoiseTraceLineChartData.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoiseTraceLineChartData.Location = new System.Drawing.Point(6, 6);
            this.lblNoiseTraceLineChartData.Name = "lblNoiseTraceLineChartData";
            this.lblNoiseTraceLineChartData.Size = new System.Drawing.Size(117, 19);
            this.lblNoiseTraceLineChartData.TabIndex = 1;
            this.lblNoiseTraceLineChartData.Text = "Line Chart Data : ";
            // 
            // cbxNoiseTraceLineChartData
            // 
            this.cbxNoiseTraceLineChartData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxNoiseTraceLineChartData.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxNoiseTraceLineChartData.FormattingEnabled = true;
            this.cbxNoiseTraceLineChartData.Location = new System.Drawing.Point(136, 3);
            this.cbxNoiseTraceLineChartData.Name = "cbxNoiseTraceLineChartData";
            this.cbxNoiseTraceLineChartData.Size = new System.Drawing.Size(151, 27);
            this.cbxNoiseTraceLineChartData.TabIndex = 2;
            this.cbxNoiseTraceLineChartData.SelectedIndexChanged += new System.EventHandler(this.cbxNoiseTraceLineChartData_SelectedIndexChanged);
            // 
            // scNoiseTraceLineChart
            // 
            this.scNoiseTraceLineChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scNoiseTraceLineChart.IsSplitterFixed = true;
            this.scNoiseTraceLineChart.Location = new System.Drawing.Point(1, 32);
            this.scNoiseTraceLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.scNoiseTraceLineChart.Name = "scNoiseTraceLineChart";
            this.scNoiseTraceLineChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scNoiseTraceLineChart.Panel1
            // 
            this.scNoiseTraceLineChart.Panel1.Controls.Add(this.picbxNoiseRXTrace);
            // 
            // scNoiseTraceLineChart.Panel2
            // 
            this.scNoiseTraceLineChart.Panel2.Controls.Add(this.picbxNoiseTXTrace);
            this.scNoiseTraceLineChart.Size = new System.Drawing.Size(1168, 530);
            this.scNoiseTraceLineChart.SplitterDistance = 260;
            this.scNoiseTraceLineChart.SplitterWidth = 3;
            this.scNoiseTraceLineChart.TabIndex = 1;
            this.scNoiseTraceLineChart.Resize += new System.EventHandler(this.scNoiseTraceLineChart_Resize);
            // 
            // picbxNoiseRXTrace
            // 
            this.picbxNoiseRXTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxNoiseRXTrace.Location = new System.Drawing.Point(0, 0);
            this.picbxNoiseRXTrace.Margin = new System.Windows.Forms.Padding(2);
            this.picbxNoiseRXTrace.Name = "picbxNoiseRXTrace";
            this.picbxNoiseRXTrace.Size = new System.Drawing.Size(1168, 260);
            this.picbxNoiseRXTrace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxNoiseRXTrace.TabIndex = 0;
            this.picbxNoiseRXTrace.TabStop = false;
            // 
            // picbxNoiseTXTrace
            // 
            this.picbxNoiseTXTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxNoiseTXTrace.Location = new System.Drawing.Point(0, 0);
            this.picbxNoiseTXTrace.Margin = new System.Windows.Forms.Padding(2);
            this.picbxNoiseTXTrace.Name = "picbxNoiseTXTrace";
            this.picbxNoiseTXTrace.Size = new System.Drawing.Size(1168, 267);
            this.picbxNoiseTXTrace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxNoiseTXTrace.TabIndex = 0;
            this.picbxNoiseTXTrace.TabStop = false;
            // 
            // tpgNoiseFrequencyLineChart
            // 
            this.tpgNoiseFrequencyLineChart.Controls.Add(this.ckbxNoiseFreqChartIncludeMaxValue);
            this.tpgNoiseFrequencyLineChart.Controls.Add(this.tcNoiseFrequency);
            this.tpgNoiseFrequencyLineChart.Location = new System.Drawing.Point(4, 26);
            this.tpgNoiseFrequencyLineChart.Name = "tpgNoiseFrequencyLineChart";
            this.tpgNoiseFrequencyLineChart.Padding = new System.Windows.Forms.Padding(3);
            this.tpgNoiseFrequencyLineChart.Size = new System.Drawing.Size(1173, 568);
            this.tpgNoiseFrequencyLineChart.TabIndex = 1;
            this.tpgNoiseFrequencyLineChart.Text = "Frequency Line Chart";
            this.tpgNoiseFrequencyLineChart.UseVisualStyleBackColor = true;
            // 
            // ckbxNoiseFreqChartIncludeMaxValue
            // 
            this.ckbxNoiseFreqChartIncludeMaxValue.AutoSize = true;
            this.ckbxNoiseFreqChartIncludeMaxValue.Checked = true;
            this.ckbxNoiseFreqChartIncludeMaxValue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbxNoiseFreqChartIncludeMaxValue.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckbxNoiseFreqChartIncludeMaxValue.Location = new System.Drawing.Point(7, 4);
            this.ckbxNoiseFreqChartIncludeMaxValue.Name = "ckbxNoiseFreqChartIncludeMaxValue";
            this.ckbxNoiseFreqChartIncludeMaxValue.Size = new System.Drawing.Size(141, 23);
            this.ckbxNoiseFreqChartIncludeMaxValue.TabIndex = 1;
            this.ckbxNoiseFreqChartIncludeMaxValue.Text = "Include Max Value";
            this.ckbxNoiseFreqChartIncludeMaxValue.UseVisualStyleBackColor = true;
            this.ckbxNoiseFreqChartIncludeMaxValue.CheckedChanged += new System.EventHandler(this.ckbxNoiseFreqChartIncludeMaxValue_CheckedChanged);
            // 
            // tcNoiseFrequency
            // 
            this.tcNoiseFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcNoiseFrequency.Controls.Add(this.tpgNoiseRXFrequency);
            this.tcNoiseFrequency.Controls.Add(this.tpgNoiseTXFrequency);
            this.tcNoiseFrequency.Controls.Add(this.tpgNoiseTotalFrequency);
            this.tcNoiseFrequency.Location = new System.Drawing.Point(3, 27);
            this.tcNoiseFrequency.Name = "tcNoiseFrequency";
            this.tcNoiseFrequency.SelectedIndex = 0;
            this.tcNoiseFrequency.Size = new System.Drawing.Size(1167, 538);
            this.tcNoiseFrequency.TabIndex = 0;
            // 
            // tpgNoiseRXFrequency
            // 
            this.tpgNoiseRXFrequency.BackColor = System.Drawing.SystemColors.Control;
            this.tpgNoiseRXFrequency.Controls.Add(this.scNoiseRXFreqLineChart);
            this.tpgNoiseRXFrequency.Location = new System.Drawing.Point(4, 26);
            this.tpgNoiseRXFrequency.Name = "tpgNoiseRXFrequency";
            this.tpgNoiseRXFrequency.Padding = new System.Windows.Forms.Padding(3);
            this.tpgNoiseRXFrequency.Size = new System.Drawing.Size(1159, 508);
            this.tpgNoiseRXFrequency.TabIndex = 0;
            this.tpgNoiseRXFrequency.Text = "RX";
            // 
            // scNoiseRXFreqLineChart
            // 
            this.scNoiseRXFreqLineChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scNoiseRXFreqLineChart.IsSplitterFixed = true;
            this.scNoiseRXFreqLineChart.Location = new System.Drawing.Point(3, 3);
            this.scNoiseRXFreqLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.scNoiseRXFreqLineChart.Name = "scNoiseRXFreqLineChart";
            this.scNoiseRXFreqLineChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scNoiseRXFreqLineChart.Panel1
            // 
            this.scNoiseRXFreqLineChart.Panel1.Controls.Add(this.picbxNoiseRXInnerFreq);
            // 
            // scNoiseRXFreqLineChart.Panel2
            // 
            this.scNoiseRXFreqLineChart.Panel2.Controls.Add(this.picbxNoiseRXEdgeFreq);
            this.scNoiseRXFreqLineChart.Size = new System.Drawing.Size(1153, 502);
            this.scNoiseRXFreqLineChart.SplitterDistance = 247;
            this.scNoiseRXFreqLineChart.SplitterWidth = 3;
            this.scNoiseRXFreqLineChart.TabIndex = 2;
            this.scNoiseRXFreqLineChart.Resize += new System.EventHandler(this.scNoiseRXFreqLineChart_Resize);
            // 
            // picbxNoiseRXInnerFreq
            // 
            this.picbxNoiseRXInnerFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxNoiseRXInnerFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxNoiseRXInnerFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxNoiseRXInnerFreq.Name = "picbxNoiseRXInnerFreq";
            this.picbxNoiseRXInnerFreq.Size = new System.Drawing.Size(1153, 247);
            this.picbxNoiseRXInnerFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxNoiseRXInnerFreq.TabIndex = 0;
            this.picbxNoiseRXInnerFreq.TabStop = false;
            // 
            // picbxNoiseRXEdgeFreq
            // 
            this.picbxNoiseRXEdgeFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxNoiseRXEdgeFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxNoiseRXEdgeFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxNoiseRXEdgeFreq.Name = "picbxNoiseRXEdgeFreq";
            this.picbxNoiseRXEdgeFreq.Size = new System.Drawing.Size(1153, 252);
            this.picbxNoiseRXEdgeFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxNoiseRXEdgeFreq.TabIndex = 0;
            this.picbxNoiseRXEdgeFreq.TabStop = false;
            // 
            // tpgNoiseTXFrequency
            // 
            this.tpgNoiseTXFrequency.BackColor = System.Drawing.SystemColors.Control;
            this.tpgNoiseTXFrequency.Controls.Add(this.scNoiseTXFreqLineChart);
            this.tpgNoiseTXFrequency.Location = new System.Drawing.Point(4, 26);
            this.tpgNoiseTXFrequency.Name = "tpgNoiseTXFrequency";
            this.tpgNoiseTXFrequency.Padding = new System.Windows.Forms.Padding(3);
            this.tpgNoiseTXFrequency.Size = new System.Drawing.Size(1159, 508);
            this.tpgNoiseTXFrequency.TabIndex = 1;
            this.tpgNoiseTXFrequency.Text = "TX";
            // 
            // scNoiseTXFreqLineChart
            // 
            this.scNoiseTXFreqLineChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scNoiseTXFreqLineChart.IsSplitterFixed = true;
            this.scNoiseTXFreqLineChart.Location = new System.Drawing.Point(3, 3);
            this.scNoiseTXFreqLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.scNoiseTXFreqLineChart.Name = "scNoiseTXFreqLineChart";
            this.scNoiseTXFreqLineChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scNoiseTXFreqLineChart.Panel1
            // 
            this.scNoiseTXFreqLineChart.Panel1.Controls.Add(this.picbxNoiseTXInnerFreq);
            // 
            // scNoiseTXFreqLineChart.Panel2
            // 
            this.scNoiseTXFreqLineChart.Panel2.Controls.Add(this.picbxNoiseTXEdgeFreq);
            this.scNoiseTXFreqLineChart.Size = new System.Drawing.Size(1153, 502);
            this.scNoiseTXFreqLineChart.SplitterDistance = 247;
            this.scNoiseTXFreqLineChart.SplitterWidth = 3;
            this.scNoiseTXFreqLineChart.TabIndex = 3;
            this.scNoiseTXFreqLineChart.Resize += new System.EventHandler(this.scNoiseTXFreqLineChart_Resize);
            // 
            // picbxNoiseTXInnerFreq
            // 
            this.picbxNoiseTXInnerFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxNoiseTXInnerFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxNoiseTXInnerFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxNoiseTXInnerFreq.Name = "picbxNoiseTXInnerFreq";
            this.picbxNoiseTXInnerFreq.Size = new System.Drawing.Size(1153, 247);
            this.picbxNoiseTXInnerFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxNoiseTXInnerFreq.TabIndex = 0;
            this.picbxNoiseTXInnerFreq.TabStop = false;
            // 
            // picbxNoiseTXEdgeFreq
            // 
            this.picbxNoiseTXEdgeFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxNoiseTXEdgeFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxNoiseTXEdgeFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxNoiseTXEdgeFreq.Name = "picbxNoiseTXEdgeFreq";
            this.picbxNoiseTXEdgeFreq.Size = new System.Drawing.Size(1153, 252);
            this.picbxNoiseTXEdgeFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxNoiseTXEdgeFreq.TabIndex = 0;
            this.picbxNoiseTXEdgeFreq.TabStop = false;
            // 
            // tpgNoiseTotalFrequency
            // 
            this.tpgNoiseTotalFrequency.BackColor = System.Drawing.SystemColors.Control;
            this.tpgNoiseTotalFrequency.Controls.Add(this.picbxNoiseTotalFreq);
            this.tpgNoiseTotalFrequency.Location = new System.Drawing.Point(4, 26);
            this.tpgNoiseTotalFrequency.Name = "tpgNoiseTotalFrequency";
            this.tpgNoiseTotalFrequency.Size = new System.Drawing.Size(1159, 508);
            this.tpgNoiseTotalFrequency.TabIndex = 2;
            this.tpgNoiseTotalFrequency.Text = "Total";
            // 
            // picbxNoiseTotalFreq
            // 
            this.picbxNoiseTotalFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxNoiseTotalFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxNoiseTotalFreq.Name = "picbxNoiseTotalFreq";
            this.picbxNoiseTotalFreq.Size = new System.Drawing.Size(1159, 508);
            this.picbxNoiseTotalFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxNoiseTotalFreq.TabIndex = 0;
            this.picbxNoiseTotalFreq.TabStop = false;
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpgNoise);
            this.tcMain.Controls.Add(this.tpgTiltNoise);
            this.tcMain.Controls.Add(this.tpgDigitalTuning);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1195, 636);
            this.tcMain.TabIndex = 1;
            // 
            // tpgNoise
            // 
            this.tpgNoise.Controls.Add(this.tcNoise);
            this.tpgNoise.Location = new System.Drawing.Point(4, 28);
            this.tpgNoise.Name = "tpgNoise";
            this.tpgNoise.Padding = new System.Windows.Forms.Padding(3);
            this.tpgNoise.Size = new System.Drawing.Size(1187, 604);
            this.tpgNoise.TabIndex = 0;
            this.tpgNoise.Text = "Noise";
            this.tpgNoise.UseVisualStyleBackColor = true;
            // 
            // tpgTiltNoise
            // 
            this.tpgTiltNoise.Controls.Add(this.tcTiltNoise);
            this.tpgTiltNoise.Location = new System.Drawing.Point(4, 28);
            this.tpgTiltNoise.Name = "tpgTiltNoise";
            this.tpgTiltNoise.Padding = new System.Windows.Forms.Padding(3);
            this.tpgTiltNoise.Size = new System.Drawing.Size(1187, 604);
            this.tpgTiltNoise.TabIndex = 2;
            this.tpgTiltNoise.Text = "Tilt Noise";
            this.tpgTiltNoise.UseVisualStyleBackColor = true;
            // 
            // tcTiltNoise
            // 
            this.tcTiltNoise.Controls.Add(this.tpgTNTraceLineChart);
            this.tcTiltNoise.Controls.Add(this.tpgTNFrequencyLineChart);
            this.tcTiltNoise.Controls.Add(this.tpgCompositeLineChart);
            this.tcTiltNoise.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcTiltNoise.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcTiltNoise.Location = new System.Drawing.Point(3, 3);
            this.tcTiltNoise.Name = "tcTiltNoise";
            this.tcTiltNoise.SelectedIndex = 0;
            this.tcTiltNoise.Size = new System.Drawing.Size(1181, 598);
            this.tcTiltNoise.TabIndex = 1;
            // 
            // tpgTNTraceLineChart
            // 
            this.tpgTNTraceLineChart.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTNTraceLineChart.Controls.Add(this.lblTNTraceLineChartData);
            this.tpgTNTraceLineChart.Controls.Add(this.cbxTNTraceLineChartData);
            this.tpgTNTraceLineChart.Controls.Add(this.scTNTraceLineChart);
            this.tpgTNTraceLineChart.Location = new System.Drawing.Point(4, 26);
            this.tpgTNTraceLineChart.Name = "tpgTNTraceLineChart";
            this.tpgTNTraceLineChart.Padding = new System.Windows.Forms.Padding(3);
            this.tpgTNTraceLineChart.Size = new System.Drawing.Size(1173, 568);
            this.tpgTNTraceLineChart.TabIndex = 0;
            this.tpgTNTraceLineChart.Text = "Trace Line Chart";
            // 
            // lblTNTraceLineChartData
            // 
            this.lblTNTraceLineChartData.AutoSize = true;
            this.lblTNTraceLineChartData.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTNTraceLineChartData.Location = new System.Drawing.Point(6, 6);
            this.lblTNTraceLineChartData.Name = "lblTNTraceLineChartData";
            this.lblTNTraceLineChartData.Size = new System.Drawing.Size(117, 19);
            this.lblTNTraceLineChartData.TabIndex = 1;
            this.lblTNTraceLineChartData.Text = "Line Chart Data : ";
            // 
            // cbxTNTraceLineChartData
            // 
            this.cbxTNTraceLineChartData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTNTraceLineChartData.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxTNTraceLineChartData.FormattingEnabled = true;
            this.cbxTNTraceLineChartData.Location = new System.Drawing.Point(136, 3);
            this.cbxTNTraceLineChartData.Name = "cbxTNTraceLineChartData";
            this.cbxTNTraceLineChartData.Size = new System.Drawing.Size(151, 27);
            this.cbxTNTraceLineChartData.TabIndex = 2;
            this.cbxTNTraceLineChartData.SelectedIndexChanged += new System.EventHandler(this.cbxTNTraceLineChartData_SelectedIndexChanged);
            // 
            // scTNTraceLineChart
            // 
            this.scTNTraceLineChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scTNTraceLineChart.IsSplitterFixed = true;
            this.scTNTraceLineChart.Location = new System.Drawing.Point(1, 32);
            this.scTNTraceLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.scTNTraceLineChart.Name = "scTNTraceLineChart";
            this.scTNTraceLineChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTNTraceLineChart.Panel1
            // 
            this.scTNTraceLineChart.Panel1.Controls.Add(this.picbxTNRXTrace);
            // 
            // scTNTraceLineChart.Panel2
            // 
            this.scTNTraceLineChart.Panel2.Controls.Add(this.picbxTNTXTrace);
            this.scTNTraceLineChart.Size = new System.Drawing.Size(1168, 530);
            this.scTNTraceLineChart.SplitterDistance = 260;
            this.scTNTraceLineChart.SplitterWidth = 3;
            this.scTNTraceLineChart.TabIndex = 1;
            this.scTNTraceLineChart.Resize += new System.EventHandler(this.scTNTraceLineChart_Resize);
            // 
            // picbxTNRXTrace
            // 
            this.picbxTNRXTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNRXTrace.Location = new System.Drawing.Point(0, 0);
            this.picbxTNRXTrace.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNRXTrace.Name = "picbxTNRXTrace";
            this.picbxTNRXTrace.Size = new System.Drawing.Size(1168, 260);
            this.picbxTNRXTrace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNRXTrace.TabIndex = 0;
            this.picbxTNRXTrace.TabStop = false;
            // 
            // picbxTNTXTrace
            // 
            this.picbxTNTXTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNTXTrace.Location = new System.Drawing.Point(0, 0);
            this.picbxTNTXTrace.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNTXTrace.Name = "picbxTNTXTrace";
            this.picbxTNTXTrace.Size = new System.Drawing.Size(1168, 267);
            this.picbxTNTXTrace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNTXTrace.TabIndex = 0;
            this.picbxTNTXTrace.TabStop = false;
            // 
            // tpgTNFrequencyLineChart
            // 
            this.tpgTNFrequencyLineChart.Controls.Add(this.ckbxTNFrequencyLineIncludeMaxValue);
            this.tpgTNFrequencyLineChart.Controls.Add(this.tcTNFrequency);
            this.tpgTNFrequencyLineChart.Location = new System.Drawing.Point(4, 26);
            this.tpgTNFrequencyLineChart.Name = "tpgTNFrequencyLineChart";
            this.tpgTNFrequencyLineChart.Padding = new System.Windows.Forms.Padding(3);
            this.tpgTNFrequencyLineChart.Size = new System.Drawing.Size(1173, 568);
            this.tpgTNFrequencyLineChart.TabIndex = 1;
            this.tpgTNFrequencyLineChart.Text = "Frequency Line Chart";
            this.tpgTNFrequencyLineChart.UseVisualStyleBackColor = true;
            // 
            // ckbxTNFrequencyLineIncludeMaxValue
            // 
            this.ckbxTNFrequencyLineIncludeMaxValue.AutoSize = true;
            this.ckbxTNFrequencyLineIncludeMaxValue.Checked = true;
            this.ckbxTNFrequencyLineIncludeMaxValue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbxTNFrequencyLineIncludeMaxValue.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckbxTNFrequencyLineIncludeMaxValue.Location = new System.Drawing.Point(6, 3);
            this.ckbxTNFrequencyLineIncludeMaxValue.Name = "ckbxTNFrequencyLineIncludeMaxValue";
            this.ckbxTNFrequencyLineIncludeMaxValue.Size = new System.Drawing.Size(141, 23);
            this.ckbxTNFrequencyLineIncludeMaxValue.TabIndex = 2;
            this.ckbxTNFrequencyLineIncludeMaxValue.Text = "Include Max Value";
            this.ckbxTNFrequencyLineIncludeMaxValue.UseVisualStyleBackColor = true;
            this.ckbxTNFrequencyLineIncludeMaxValue.CheckedChanged += new System.EventHandler(this.ckbxTNFrequencyLineIncludeMaxValue_CheckedChanged);
            // 
            // tcTNFrequency
            // 
            this.tcTNFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcTNFrequency.Controls.Add(this.tpgTNPTHFRXFreq);
            this.tcTNFrequency.Controls.Add(this.tpgTNPTHFTXFreq);
            this.tcTNFrequency.Controls.Add(this.tpgTNPTHFFreq);
            this.tcTNFrequency.Controls.Add(this.tpgTNBHFRXFreq);
            this.tcTNFrequency.Controls.Add(this.tpgTNBHFTXFreq);
            this.tcTNFrequency.Controls.Add(this.tpgTNBHFFreq);
            this.tcTNFrequency.Controls.Add(this.tpgTNRXFreq);
            this.tcTNFrequency.Controls.Add(this.tpgTNFreq);
            this.tcTNFrequency.Controls.Add(this.tpgTNTotalFreq);
            this.tcTNFrequency.Location = new System.Drawing.Point(3, 26);
            this.tcTNFrequency.Name = "tcTNFrequency";
            this.tcTNFrequency.SelectedIndex = 0;
            this.tcTNFrequency.Size = new System.Drawing.Size(1167, 539);
            this.tcTNFrequency.TabIndex = 0;
            // 
            // tpgTNPTHFRXFreq
            // 
            this.tpgTNPTHFRXFreq.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTNPTHFRXFreq.Controls.Add(this.scTNPTHFRXFreqLineChart);
            this.tpgTNPTHFRXFreq.Location = new System.Drawing.Point(4, 26);
            this.tpgTNPTHFRXFreq.Name = "tpgTNPTHFRXFreq";
            this.tpgTNPTHFRXFreq.Padding = new System.Windows.Forms.Padding(3);
            this.tpgTNPTHFRXFreq.Size = new System.Drawing.Size(1159, 509);
            this.tpgTNPTHFRXFreq.TabIndex = 0;
            this.tpgTNPTHFRXFreq.Text = "PTHF RX";
            // 
            // scTNPTHFRXFreqLineChart
            // 
            this.scTNPTHFRXFreqLineChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTNPTHFRXFreqLineChart.IsSplitterFixed = true;
            this.scTNPTHFRXFreqLineChart.Location = new System.Drawing.Point(3, 3);
            this.scTNPTHFRXFreqLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.scTNPTHFRXFreqLineChart.Name = "scTNPTHFRXFreqLineChart";
            this.scTNPTHFRXFreqLineChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTNPTHFRXFreqLineChart.Panel1
            // 
            this.scTNPTHFRXFreqLineChart.Panel1.Controls.Add(this.picbxTNPTHFRXInnerFreq);
            // 
            // scTNPTHFRXFreqLineChart.Panel2
            // 
            this.scTNPTHFRXFreqLineChart.Panel2.Controls.Add(this.picbxTNPTHFRXEdgeFreq);
            this.scTNPTHFRXFreqLineChart.Size = new System.Drawing.Size(1153, 503);
            this.scTNPTHFRXFreqLineChart.SplitterDistance = 248;
            this.scTNPTHFRXFreqLineChart.SplitterWidth = 3;
            this.scTNPTHFRXFreqLineChart.TabIndex = 2;
            this.scTNPTHFRXFreqLineChart.Resize += new System.EventHandler(this.scTNPTHFRXFreqLineChart_Resize);
            // 
            // picbxTNPTHFRXInnerFreq
            // 
            this.picbxTNPTHFRXInnerFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNPTHFRXInnerFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNPTHFRXInnerFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNPTHFRXInnerFreq.Name = "picbxTNPTHFRXInnerFreq";
            this.picbxTNPTHFRXInnerFreq.Size = new System.Drawing.Size(1153, 248);
            this.picbxTNPTHFRXInnerFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNPTHFRXInnerFreq.TabIndex = 0;
            this.picbxTNPTHFRXInnerFreq.TabStop = false;
            // 
            // picbxTNPTHFRXEdgeFreq
            // 
            this.picbxTNPTHFRXEdgeFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNPTHFRXEdgeFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNPTHFRXEdgeFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNPTHFRXEdgeFreq.Name = "picbxTNPTHFRXEdgeFreq";
            this.picbxTNPTHFRXEdgeFreq.Size = new System.Drawing.Size(1153, 252);
            this.picbxTNPTHFRXEdgeFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNPTHFRXEdgeFreq.TabIndex = 0;
            this.picbxTNPTHFRXEdgeFreq.TabStop = false;
            // 
            // tpgTNPTHFTXFreq
            // 
            this.tpgTNPTHFTXFreq.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTNPTHFTXFreq.Controls.Add(this.scTNPTHFTXFreqLineChart);
            this.tpgTNPTHFTXFreq.Location = new System.Drawing.Point(4, 26);
            this.tpgTNPTHFTXFreq.Name = "tpgTNPTHFTXFreq";
            this.tpgTNPTHFTXFreq.Padding = new System.Windows.Forms.Padding(3);
            this.tpgTNPTHFTXFreq.Size = new System.Drawing.Size(1159, 509);
            this.tpgTNPTHFTXFreq.TabIndex = 1;
            this.tpgTNPTHFTXFreq.Text = "PTHF TX";
            // 
            // scTNPTHFTXFreqLineChart
            // 
            this.scTNPTHFTXFreqLineChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTNPTHFTXFreqLineChart.IsSplitterFixed = true;
            this.scTNPTHFTXFreqLineChart.Location = new System.Drawing.Point(3, 3);
            this.scTNPTHFTXFreqLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.scTNPTHFTXFreqLineChart.Name = "scTNPTHFTXFreqLineChart";
            this.scTNPTHFTXFreqLineChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTNPTHFTXFreqLineChart.Panel1
            // 
            this.scTNPTHFTXFreqLineChart.Panel1.Controls.Add(this.picbxTNPTHFTXInnerFreq);
            // 
            // scTNPTHFTXFreqLineChart.Panel2
            // 
            this.scTNPTHFTXFreqLineChart.Panel2.Controls.Add(this.picbxTNPTHFTXEdgeFreq);
            this.scTNPTHFTXFreqLineChart.Size = new System.Drawing.Size(1153, 503);
            this.scTNPTHFTXFreqLineChart.SplitterDistance = 248;
            this.scTNPTHFTXFreqLineChart.SplitterWidth = 3;
            this.scTNPTHFTXFreqLineChart.TabIndex = 3;
            this.scTNPTHFTXFreqLineChart.Resize += new System.EventHandler(this.scTNPTHFTXFreqLineChart_Resize);
            // 
            // picbxTNPTHFTXInnerFreq
            // 
            this.picbxTNPTHFTXInnerFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNPTHFTXInnerFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNPTHFTXInnerFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNPTHFTXInnerFreq.Name = "picbxTNPTHFTXInnerFreq";
            this.picbxTNPTHFTXInnerFreq.Size = new System.Drawing.Size(1153, 248);
            this.picbxTNPTHFTXInnerFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNPTHFTXInnerFreq.TabIndex = 0;
            this.picbxTNPTHFTXInnerFreq.TabStop = false;
            // 
            // picbxTNPTHFTXEdgeFreq
            // 
            this.picbxTNPTHFTXEdgeFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNPTHFTXEdgeFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNPTHFTXEdgeFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNPTHFTXEdgeFreq.Name = "picbxTNPTHFTXEdgeFreq";
            this.picbxTNPTHFTXEdgeFreq.Size = new System.Drawing.Size(1153, 252);
            this.picbxTNPTHFTXEdgeFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNPTHFTXEdgeFreq.TabIndex = 0;
            this.picbxTNPTHFTXEdgeFreq.TabStop = false;
            // 
            // tpgTNPTHFFreq
            // 
            this.tpgTNPTHFFreq.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTNPTHFFreq.Controls.Add(this.picbxTNPTHFFreq);
            this.tpgTNPTHFFreq.Location = new System.Drawing.Point(4, 26);
            this.tpgTNPTHFFreq.Name = "tpgTNPTHFFreq";
            this.tpgTNPTHFFreq.Size = new System.Drawing.Size(1159, 509);
            this.tpgTNPTHFFreq.TabIndex = 2;
            this.tpgTNPTHFFreq.Text = "PTHF";
            // 
            // picbxTNPTHFFreq
            // 
            this.picbxTNPTHFFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNPTHFFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNPTHFFreq.Name = "picbxTNPTHFFreq";
            this.picbxTNPTHFFreq.Size = new System.Drawing.Size(1159, 509);
            this.picbxTNPTHFFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNPTHFFreq.TabIndex = 0;
            this.picbxTNPTHFFreq.TabStop = false;
            // 
            // tpgTNBHFRXFreq
            // 
            this.tpgTNBHFRXFreq.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTNBHFRXFreq.Controls.Add(this.scTNBHFRXFreqLineChart);
            this.tpgTNBHFRXFreq.Location = new System.Drawing.Point(4, 26);
            this.tpgTNBHFRXFreq.Name = "tpgTNBHFRXFreq";
            this.tpgTNBHFRXFreq.Padding = new System.Windows.Forms.Padding(3);
            this.tpgTNBHFRXFreq.Size = new System.Drawing.Size(1159, 509);
            this.tpgTNBHFRXFreq.TabIndex = 3;
            this.tpgTNBHFRXFreq.Text = "BHF RX";
            // 
            // scTNBHFRXFreqLineChart
            // 
            this.scTNBHFRXFreqLineChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTNBHFRXFreqLineChart.IsSplitterFixed = true;
            this.scTNBHFRXFreqLineChart.Location = new System.Drawing.Point(3, 3);
            this.scTNBHFRXFreqLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.scTNBHFRXFreqLineChart.Name = "scTNBHFRXFreqLineChart";
            this.scTNBHFRXFreqLineChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTNBHFRXFreqLineChart.Panel1
            // 
            this.scTNBHFRXFreqLineChart.Panel1.Controls.Add(this.picbxTNBHFRXInnerFreq);
            // 
            // scTNBHFRXFreqLineChart.Panel2
            // 
            this.scTNBHFRXFreqLineChart.Panel2.Controls.Add(this.picbxTNBHFRXEdgeFreq);
            this.scTNBHFRXFreqLineChart.Size = new System.Drawing.Size(1153, 503);
            this.scTNBHFRXFreqLineChart.SplitterDistance = 248;
            this.scTNBHFRXFreqLineChart.SplitterWidth = 3;
            this.scTNBHFRXFreqLineChart.TabIndex = 0;
            this.scTNBHFRXFreqLineChart.Resize += new System.EventHandler(this.scTNBHFRXFreqLineChart_Resize);
            // 
            // picbxTNBHFRXInnerFreq
            // 
            this.picbxTNBHFRXInnerFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNBHFRXInnerFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNBHFRXInnerFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNBHFRXInnerFreq.Name = "picbxTNBHFRXInnerFreq";
            this.picbxTNBHFRXInnerFreq.Size = new System.Drawing.Size(1153, 248);
            this.picbxTNBHFRXInnerFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNBHFRXInnerFreq.TabIndex = 1;
            this.picbxTNBHFRXInnerFreq.TabStop = false;
            // 
            // picbxTNBHFRXEdgeFreq
            // 
            this.picbxTNBHFRXEdgeFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNBHFRXEdgeFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNBHFRXEdgeFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNBHFRXEdgeFreq.Name = "picbxTNBHFRXEdgeFreq";
            this.picbxTNBHFRXEdgeFreq.Size = new System.Drawing.Size(1153, 252);
            this.picbxTNBHFRXEdgeFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNBHFRXEdgeFreq.TabIndex = 1;
            this.picbxTNBHFRXEdgeFreq.TabStop = false;
            // 
            // tpgTNBHFTXFreq
            // 
            this.tpgTNBHFTXFreq.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTNBHFTXFreq.Controls.Add(this.scTNBHFTXFreqLineChart);
            this.tpgTNBHFTXFreq.Location = new System.Drawing.Point(4, 26);
            this.tpgTNBHFTXFreq.Name = "tpgTNBHFTXFreq";
            this.tpgTNBHFTXFreq.Padding = new System.Windows.Forms.Padding(3);
            this.tpgTNBHFTXFreq.Size = new System.Drawing.Size(1159, 509);
            this.tpgTNBHFTXFreq.TabIndex = 4;
            this.tpgTNBHFTXFreq.Text = "BHF TX";
            // 
            // scTNBHFTXFreqLineChart
            // 
            this.scTNBHFTXFreqLineChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTNBHFTXFreqLineChart.Location = new System.Drawing.Point(3, 3);
            this.scTNBHFTXFreqLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.scTNBHFTXFreqLineChart.Name = "scTNBHFTXFreqLineChart";
            this.scTNBHFTXFreqLineChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTNBHFTXFreqLineChart.Panel1
            // 
            this.scTNBHFTXFreqLineChart.Panel1.Controls.Add(this.picbxTNBHFTXInnerFreq);
            // 
            // scTNBHFTXFreqLineChart.Panel2
            // 
            this.scTNBHFTXFreqLineChart.Panel2.Controls.Add(this.picbxTNBHFTXEdgeFreq);
            this.scTNBHFTXFreqLineChart.Size = new System.Drawing.Size(1153, 503);
            this.scTNBHFTXFreqLineChart.SplitterDistance = 248;
            this.scTNBHFTXFreqLineChart.SplitterWidth = 3;
            this.scTNBHFTXFreqLineChart.TabIndex = 0;
            this.scTNBHFTXFreqLineChart.Resize += new System.EventHandler(this.scTNBHFTXFreqLineChart_Resize);
            // 
            // picbxTNBHFTXInnerFreq
            // 
            this.picbxTNBHFTXInnerFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNBHFTXInnerFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNBHFTXInnerFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNBHFTXInnerFreq.Name = "picbxTNBHFTXInnerFreq";
            this.picbxTNBHFTXInnerFreq.Size = new System.Drawing.Size(1153, 248);
            this.picbxTNBHFTXInnerFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNBHFTXInnerFreq.TabIndex = 1;
            this.picbxTNBHFTXInnerFreq.TabStop = false;
            // 
            // picbxTNBHFTXEdgeFreq
            // 
            this.picbxTNBHFTXEdgeFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNBHFTXEdgeFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNBHFTXEdgeFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNBHFTXEdgeFreq.Name = "picbxTNBHFTXEdgeFreq";
            this.picbxTNBHFTXEdgeFreq.Size = new System.Drawing.Size(1153, 252);
            this.picbxTNBHFTXEdgeFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNBHFTXEdgeFreq.TabIndex = 1;
            this.picbxTNBHFTXEdgeFreq.TabStop = false;
            // 
            // tpgTNBHFFreq
            // 
            this.tpgTNBHFFreq.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTNBHFFreq.Controls.Add(this.picbxTNBHFFreq);
            this.tpgTNBHFFreq.Location = new System.Drawing.Point(4, 26);
            this.tpgTNBHFFreq.Name = "tpgTNBHFFreq";
            this.tpgTNBHFFreq.Size = new System.Drawing.Size(1159, 509);
            this.tpgTNBHFFreq.TabIndex = 5;
            this.tpgTNBHFFreq.Text = "BHF";
            // 
            // picbxTNBHFFreq
            // 
            this.picbxTNBHFFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNBHFFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNBHFFreq.Name = "picbxTNBHFFreq";
            this.picbxTNBHFFreq.Size = new System.Drawing.Size(1159, 509);
            this.picbxTNBHFFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNBHFFreq.TabIndex = 1;
            this.picbxTNBHFFreq.TabStop = false;
            // 
            // tpgTNRXFreq
            // 
            this.tpgTNRXFreq.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTNRXFreq.Controls.Add(this.scTNTotalRXLineChart);
            this.tpgTNRXFreq.Location = new System.Drawing.Point(4, 26);
            this.tpgTNRXFreq.Name = "tpgTNRXFreq";
            this.tpgTNRXFreq.Padding = new System.Windows.Forms.Padding(3);
            this.tpgTNRXFreq.Size = new System.Drawing.Size(1159, 509);
            this.tpgTNRXFreq.TabIndex = 6;
            this.tpgTNRXFreq.Text = "Total RX";
            // 
            // scTNTotalRXLineChart
            // 
            this.scTNTotalRXLineChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTNTotalRXLineChart.Location = new System.Drawing.Point(3, 3);
            this.scTNTotalRXLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.scTNTotalRXLineChart.Name = "scTNTotalRXLineChart";
            this.scTNTotalRXLineChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTNTotalRXLineChart.Panel1
            // 
            this.scTNTotalRXLineChart.Panel1.Controls.Add(this.picbxTNTotalRXInnerFreq);
            // 
            // scTNTotalRXLineChart.Panel2
            // 
            this.scTNTotalRXLineChart.Panel2.Controls.Add(this.picbxTNTotalRXEdgeFreq);
            this.scTNTotalRXLineChart.Size = new System.Drawing.Size(1153, 503);
            this.scTNTotalRXLineChart.SplitterDistance = 248;
            this.scTNTotalRXLineChart.SplitterWidth = 3;
            this.scTNTotalRXLineChart.TabIndex = 0;
            this.scTNTotalRXLineChart.Resize += new System.EventHandler(this.scTNTotalRXLineChart_Resize);
            // 
            // picbxTNTotalRXInnerFreq
            // 
            this.picbxTNTotalRXInnerFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNTotalRXInnerFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNTotalRXInnerFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNTotalRXInnerFreq.Name = "picbxTNTotalRXInnerFreq";
            this.picbxTNTotalRXInnerFreq.Size = new System.Drawing.Size(1153, 248);
            this.picbxTNTotalRXInnerFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNTotalRXInnerFreq.TabIndex = 2;
            this.picbxTNTotalRXInnerFreq.TabStop = false;
            // 
            // picbxTNTotalRXEdgeFreq
            // 
            this.picbxTNTotalRXEdgeFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNTotalRXEdgeFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNTotalRXEdgeFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNTotalRXEdgeFreq.Name = "picbxTNTotalRXEdgeFreq";
            this.picbxTNTotalRXEdgeFreq.Size = new System.Drawing.Size(1153, 252);
            this.picbxTNTotalRXEdgeFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNTotalRXEdgeFreq.TabIndex = 2;
            this.picbxTNTotalRXEdgeFreq.TabStop = false;
            // 
            // tpgTNFreq
            // 
            this.tpgTNFreq.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTNFreq.Controls.Add(this.scTNTotalTXFreqLineChart);
            this.tpgTNFreq.Location = new System.Drawing.Point(4, 26);
            this.tpgTNFreq.Name = "tpgTNFreq";
            this.tpgTNFreq.Padding = new System.Windows.Forms.Padding(3);
            this.tpgTNFreq.Size = new System.Drawing.Size(1159, 509);
            this.tpgTNFreq.TabIndex = 7;
            this.tpgTNFreq.Text = "Total TX";
            // 
            // scTNTotalTXFreqLineChart
            // 
            this.scTNTotalTXFreqLineChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTNTotalTXFreqLineChart.Location = new System.Drawing.Point(3, 3);
            this.scTNTotalTXFreqLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.scTNTotalTXFreqLineChart.Name = "scTNTotalTXFreqLineChart";
            this.scTNTotalTXFreqLineChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTNTotalTXFreqLineChart.Panel1
            // 
            this.scTNTotalTXFreqLineChart.Panel1.Controls.Add(this.picbxTNTotalTXInnerFreq);
            // 
            // scTNTotalTXFreqLineChart.Panel2
            // 
            this.scTNTotalTXFreqLineChart.Panel2.Controls.Add(this.picbxTNTotalTXEdgeFreq);
            this.scTNTotalTXFreqLineChart.Size = new System.Drawing.Size(1153, 503);
            this.scTNTotalTXFreqLineChart.SplitterDistance = 248;
            this.scTNTotalTXFreqLineChart.SplitterWidth = 3;
            this.scTNTotalTXFreqLineChart.TabIndex = 0;
            this.scTNTotalTXFreqLineChart.Resize += new System.EventHandler(this.scTNTotalTXFreqLineChart_Resize);
            // 
            // picbxTNTotalTXInnerFreq
            // 
            this.picbxTNTotalTXInnerFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNTotalTXInnerFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNTotalTXInnerFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNTotalTXInnerFreq.Name = "picbxTNTotalTXInnerFreq";
            this.picbxTNTotalTXInnerFreq.Size = new System.Drawing.Size(1153, 248);
            this.picbxTNTotalTXInnerFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNTotalTXInnerFreq.TabIndex = 2;
            this.picbxTNTotalTXInnerFreq.TabStop = false;
            // 
            // picbxTNTotalTXEdgeFreq
            // 
            this.picbxTNTotalTXEdgeFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNTotalTXEdgeFreq.Location = new System.Drawing.Point(0, 0);
            this.picbxTNTotalTXEdgeFreq.Margin = new System.Windows.Forms.Padding(2);
            this.picbxTNTotalTXEdgeFreq.Name = "picbxTNTotalTXEdgeFreq";
            this.picbxTNTotalTXEdgeFreq.Size = new System.Drawing.Size(1153, 252);
            this.picbxTNTotalTXEdgeFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNTotalTXEdgeFreq.TabIndex = 2;
            this.picbxTNTotalTXEdgeFreq.TabStop = false;
            // 
            // tpgTNTotalFreq
            // 
            this.tpgTNTotalFreq.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTNTotalFreq.Controls.Add(this.picbxTNTotalFreq);
            this.tpgTNTotalFreq.Location = new System.Drawing.Point(4, 26);
            this.tpgTNTotalFreq.Name = "tpgTNTotalFreq";
            this.tpgTNTotalFreq.Padding = new System.Windows.Forms.Padding(3);
            this.tpgTNTotalFreq.Size = new System.Drawing.Size(1159, 509);
            this.tpgTNTotalFreq.TabIndex = 8;
            this.tpgTNTotalFreq.Text = "Total";
            // 
            // picbxTNTotalFreq
            // 
            this.picbxTNTotalFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxTNTotalFreq.Location = new System.Drawing.Point(3, 3);
            this.picbxTNTotalFreq.Name = "picbxTNTotalFreq";
            this.picbxTNTotalFreq.Size = new System.Drawing.Size(1153, 503);
            this.picbxTNTotalFreq.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxTNTotalFreq.TabIndex = 2;
            this.picbxTNTotalFreq.TabStop = false;
            // 
            // tpgCompositeLineChart
            // 
            this.tpgCompositeLineChart.Controls.Add(this.ckbxTNCompositeLineIncludeMaxValue);
            this.tpgCompositeLineChart.Controls.Add(this.picbxCompositeLineChart);
            this.tpgCompositeLineChart.Location = new System.Drawing.Point(4, 26);
            this.tpgCompositeLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.tpgCompositeLineChart.Name = "tpgCompositeLineChart";
            this.tpgCompositeLineChart.Size = new System.Drawing.Size(1173, 568);
            this.tpgCompositeLineChart.TabIndex = 2;
            this.tpgCompositeLineChart.Text = "Composite Line Chart";
            this.tpgCompositeLineChart.UseVisualStyleBackColor = true;
            // 
            // ckbxTNCompositeLineIncludeMaxValue
            // 
            this.ckbxTNCompositeLineIncludeMaxValue.AutoSize = true;
            this.ckbxTNCompositeLineIncludeMaxValue.Checked = true;
            this.ckbxTNCompositeLineIncludeMaxValue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbxTNCompositeLineIncludeMaxValue.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckbxTNCompositeLineIncludeMaxValue.Location = new System.Drawing.Point(6, 3);
            this.ckbxTNCompositeLineIncludeMaxValue.Name = "ckbxTNCompositeLineIncludeMaxValue";
            this.ckbxTNCompositeLineIncludeMaxValue.Size = new System.Drawing.Size(141, 23);
            this.ckbxTNCompositeLineIncludeMaxValue.TabIndex = 3;
            this.ckbxTNCompositeLineIncludeMaxValue.Text = "Include Max Value";
            this.ckbxTNCompositeLineIncludeMaxValue.UseVisualStyleBackColor = true;
            this.ckbxTNCompositeLineIncludeMaxValue.CheckedChanged += new System.EventHandler(this.ckbxTNCompositeLineIncludeMaxValue_CheckedChanged);
            // 
            // picbxCompositeLineChart
            // 
            this.picbxCompositeLineChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picbxCompositeLineChart.Location = new System.Drawing.Point(0, 27);
            this.picbxCompositeLineChart.Margin = new System.Windows.Forms.Padding(2);
            this.picbxCompositeLineChart.Name = "picbxCompositeLineChart";
            this.picbxCompositeLineChart.Size = new System.Drawing.Size(1173, 541);
            this.picbxCompositeLineChart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxCompositeLineChart.TabIndex = 0;
            this.picbxCompositeLineChart.TabStop = false;
            // 
            // tpgDigitalTuning
            // 
            this.tpgDigitalTuning.Controls.Add(this.tcDigitalTuning);
            this.tpgDigitalTuning.Location = new System.Drawing.Point(4, 28);
            this.tpgDigitalTuning.Name = "tpgDigitalTuning";
            this.tpgDigitalTuning.Padding = new System.Windows.Forms.Padding(3);
            this.tpgDigitalTuning.Size = new System.Drawing.Size(1187, 604);
            this.tpgDigitalTuning.TabIndex = 1;
            this.tpgDigitalTuning.Text = "Digital Tuning";
            this.tpgDigitalTuning.UseVisualStyleBackColor = true;
            // 
            // tcDigitalTuning
            // 
            this.tcDigitalTuning.Controls.Add(this.tpgDTHover1stHisto);
            this.tcDigitalTuning.Controls.Add(this.tpgDTHover2ndHisto);
            this.tcDigitalTuning.Controls.Add(this.tpgDTContactHisto);
            this.tcDigitalTuning.Controls.Add(this.tpgDTHoverTRxS);
            this.tcDigitalTuning.Controls.Add(this.tpgDTContactTRxS);
            this.tcDigitalTuning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcDigitalTuning.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcDigitalTuning.Location = new System.Drawing.Point(3, 3);
            this.tcDigitalTuning.Name = "tcDigitalTuning";
            this.tcDigitalTuning.SelectedIndex = 0;
            this.tcDigitalTuning.Size = new System.Drawing.Size(1181, 598);
            this.tcDigitalTuning.TabIndex = 4;
            // 
            // tpgDTHover1stHisto
            // 
            this.tpgDTHover1stHisto.BackColor = System.Drawing.SystemColors.Control;
            this.tpgDTHover1stHisto.Controls.Add(this.lblDTHover1stDataName);
            this.tpgDTHover1stHisto.Controls.Add(this.cbxDTHover1stDataName);
            this.tpgDTHover1stHisto.Controls.Add(this.scDTHover1stHisto);
            this.tpgDTHover1stHisto.Location = new System.Drawing.Point(4, 26);
            this.tpgDTHover1stHisto.Name = "tpgDTHover1stHisto";
            this.tpgDTHover1stHisto.Padding = new System.Windows.Forms.Padding(3);
            this.tpgDTHover1stHisto.Size = new System.Drawing.Size(1173, 568);
            this.tpgDTHover1stHisto.TabIndex = 0;
            this.tpgDTHover1stHisto.Text = "Hover_1st Histogram";
            // 
            // lblDTHover1stDataName
            // 
            this.lblDTHover1stDataName.AutoSize = true;
            this.lblDTHover1stDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDTHover1stDataName.Location = new System.Drawing.Point(4, 5);
            this.lblDTHover1stDataName.Name = "lblDTHover1stDataName";
            this.lblDTHover1stDataName.Size = new System.Drawing.Size(86, 19);
            this.lblDTHover1stDataName.TabIndex = 3;
            this.lblDTHover1stDataName.Text = "Data Name :";
            // 
            // cbxDTHover1stDataName
            // 
            this.cbxDTHover1stDataName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDTHover1stDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxDTHover1stDataName.FormattingEnabled = true;
            this.cbxDTHover1stDataName.Location = new System.Drawing.Point(99, 2);
            this.cbxDTHover1stDataName.Name = "cbxDTHover1stDataName";
            this.cbxDTHover1stDataName.Size = new System.Drawing.Size(240, 27);
            this.cbxDTHover1stDataName.TabIndex = 4;
            this.cbxDTHover1stDataName.SelectedIndexChanged += new System.EventHandler(this.cbxDTHover1stDataName_SelectedIndexChanged);
            // 
            // scDTHover1stHisto
            // 
            this.scDTHover1stHisto.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scDTHover1stHisto.IsSplitterFixed = true;
            this.scDTHover1stHisto.Location = new System.Drawing.Point(3, 32);
            this.scDTHover1stHisto.Margin = new System.Windows.Forms.Padding(2);
            this.scDTHover1stHisto.Name = "scDTHover1stHisto";
            this.scDTHover1stHisto.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scDTHover1stHisto.Panel1
            // 
            this.scDTHover1stHisto.Panel1.Controls.Add(this.picbxDTHover1stRXHisto);
            // 
            // scDTHover1stHisto.Panel2
            // 
            this.scDTHover1stHisto.Panel2.Controls.Add(this.picbxDTHover1stTXHisto);
            this.scDTHover1stHisto.Size = new System.Drawing.Size(1168, 530);
            this.scDTHover1stHisto.SplitterDistance = 260;
            this.scDTHover1stHisto.SplitterWidth = 3;
            this.scDTHover1stHisto.TabIndex = 2;
            this.scDTHover1stHisto.Resize += new System.EventHandler(this.scDTHover1stHisto_Resize);
            // 
            // picbxDTHover1stRXHisto
            // 
            this.picbxDTHover1stRXHisto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxDTHover1stRXHisto.Location = new System.Drawing.Point(0, 0);
            this.picbxDTHover1stRXHisto.Margin = new System.Windows.Forms.Padding(2);
            this.picbxDTHover1stRXHisto.Name = "picbxDTHover1stRXHisto";
            this.picbxDTHover1stRXHisto.Size = new System.Drawing.Size(1168, 260);
            this.picbxDTHover1stRXHisto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxDTHover1stRXHisto.TabIndex = 0;
            this.picbxDTHover1stRXHisto.TabStop = false;
            // 
            // picbxDTHover1stTXHisto
            // 
            this.picbxDTHover1stTXHisto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxDTHover1stTXHisto.Location = new System.Drawing.Point(0, 0);
            this.picbxDTHover1stTXHisto.Margin = new System.Windows.Forms.Padding(2);
            this.picbxDTHover1stTXHisto.Name = "picbxDTHover1stTXHisto";
            this.picbxDTHover1stTXHisto.Size = new System.Drawing.Size(1168, 267);
            this.picbxDTHover1stTXHisto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxDTHover1stTXHisto.TabIndex = 0;
            this.picbxDTHover1stTXHisto.TabStop = false;
            // 
            // tpgDTHover2ndHisto
            // 
            this.tpgDTHover2ndHisto.BackColor = System.Drawing.SystemColors.Control;
            this.tpgDTHover2ndHisto.Controls.Add(this.scDTHover2ndHisto);
            this.tpgDTHover2ndHisto.Controls.Add(this.lblDTHover2ndDataName);
            this.tpgDTHover2ndHisto.Controls.Add(this.cbxDTHover2ndDataName);
            this.tpgDTHover2ndHisto.Location = new System.Drawing.Point(4, 26);
            this.tpgDTHover2ndHisto.Name = "tpgDTHover2ndHisto";
            this.tpgDTHover2ndHisto.Padding = new System.Windows.Forms.Padding(3);
            this.tpgDTHover2ndHisto.Size = new System.Drawing.Size(1173, 568);
            this.tpgDTHover2ndHisto.TabIndex = 1;
            this.tpgDTHover2ndHisto.Text = "Hover_2nd Histogram";
            // 
            // scDTHover2ndHisto
            // 
            this.scDTHover2ndHisto.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scDTHover2ndHisto.IsSplitterFixed = true;
            this.scDTHover2ndHisto.Location = new System.Drawing.Point(3, 32);
            this.scDTHover2ndHisto.Margin = new System.Windows.Forms.Padding(2);
            this.scDTHover2ndHisto.Name = "scDTHover2ndHisto";
            this.scDTHover2ndHisto.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scDTHover2ndHisto.Panel1
            // 
            this.scDTHover2ndHisto.Panel1.Controls.Add(this.picbxDTHover2ndRXHisto);
            // 
            // scDTHover2ndHisto.Panel2
            // 
            this.scDTHover2ndHisto.Panel2.Controls.Add(this.picbxDTHover2ndTXHisto);
            this.scDTHover2ndHisto.Size = new System.Drawing.Size(1168, 530);
            this.scDTHover2ndHisto.SplitterDistance = 260;
            this.scDTHover2ndHisto.SplitterWidth = 3;
            this.scDTHover2ndHisto.TabIndex = 7;
            this.scDTHover2ndHisto.Resize += new System.EventHandler(this.scDTHover2ndHisto_Resize);
            // 
            // picbxDTHover2ndRXHisto
            // 
            this.picbxDTHover2ndRXHisto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxDTHover2ndRXHisto.Location = new System.Drawing.Point(0, 0);
            this.picbxDTHover2ndRXHisto.Margin = new System.Windows.Forms.Padding(2);
            this.picbxDTHover2ndRXHisto.Name = "picbxDTHover2ndRXHisto";
            this.picbxDTHover2ndRXHisto.Size = new System.Drawing.Size(1168, 260);
            this.picbxDTHover2ndRXHisto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxDTHover2ndRXHisto.TabIndex = 0;
            this.picbxDTHover2ndRXHisto.TabStop = false;
            // 
            // picbxDTHover2ndTXHisto
            // 
            this.picbxDTHover2ndTXHisto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxDTHover2ndTXHisto.Location = new System.Drawing.Point(0, 0);
            this.picbxDTHover2ndTXHisto.Margin = new System.Windows.Forms.Padding(2);
            this.picbxDTHover2ndTXHisto.Name = "picbxDTHover2ndTXHisto";
            this.picbxDTHover2ndTXHisto.Size = new System.Drawing.Size(1168, 267);
            this.picbxDTHover2ndTXHisto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxDTHover2ndTXHisto.TabIndex = 0;
            this.picbxDTHover2ndTXHisto.TabStop = false;
            // 
            // lblDTHover2ndDataName
            // 
            this.lblDTHover2ndDataName.AutoSize = true;
            this.lblDTHover2ndDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDTHover2ndDataName.Location = new System.Drawing.Point(4, 5);
            this.lblDTHover2ndDataName.Name = "lblDTHover2ndDataName";
            this.lblDTHover2ndDataName.Size = new System.Drawing.Size(86, 19);
            this.lblDTHover2ndDataName.TabIndex = 5;
            this.lblDTHover2ndDataName.Text = "Data Name :";
            // 
            // cbxDTHover2ndDataName
            // 
            this.cbxDTHover2ndDataName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDTHover2ndDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxDTHover2ndDataName.FormattingEnabled = true;
            this.cbxDTHover2ndDataName.Location = new System.Drawing.Point(99, 2);
            this.cbxDTHover2ndDataName.Name = "cbxDTHover2ndDataName";
            this.cbxDTHover2ndDataName.Size = new System.Drawing.Size(240, 27);
            this.cbxDTHover2ndDataName.TabIndex = 6;
            this.cbxDTHover2ndDataName.SelectedIndexChanged += new System.EventHandler(this.cbxDTHover2ndDataName_SelectedIndexChanged);
            // 
            // tpgDTContactHisto
            // 
            this.tpgDTContactHisto.BackColor = System.Drawing.SystemColors.Control;
            this.tpgDTContactHisto.Controls.Add(this.lblDTContactDataName);
            this.tpgDTContactHisto.Controls.Add(this.cbxDTContactDataName);
            this.tpgDTContactHisto.Controls.Add(this.scDTContactHisto);
            this.tpgDTContactHisto.Location = new System.Drawing.Point(4, 26);
            this.tpgDTContactHisto.Name = "tpgDTContactHisto";
            this.tpgDTContactHisto.Size = new System.Drawing.Size(1173, 568);
            this.tpgDTContactHisto.TabIndex = 2;
            this.tpgDTContactHisto.Text = "Contact Histogram";
            // 
            // lblDTContactDataName
            // 
            this.lblDTContactDataName.AutoSize = true;
            this.lblDTContactDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDTContactDataName.Location = new System.Drawing.Point(4, 5);
            this.lblDTContactDataName.Name = "lblDTContactDataName";
            this.lblDTContactDataName.Size = new System.Drawing.Size(86, 19);
            this.lblDTContactDataName.TabIndex = 5;
            this.lblDTContactDataName.Text = "Data Name :";
            // 
            // cbxDTContactDataName
            // 
            this.cbxDTContactDataName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDTContactDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxDTContactDataName.FormattingEnabled = true;
            this.cbxDTContactDataName.Location = new System.Drawing.Point(99, 2);
            this.cbxDTContactDataName.Name = "cbxDTContactDataName";
            this.cbxDTContactDataName.Size = new System.Drawing.Size(240, 27);
            this.cbxDTContactDataName.TabIndex = 6;
            this.cbxDTContactDataName.SelectedIndexChanged += new System.EventHandler(this.cbxDTContactDataName_SelectedIndexChanged);
            // 
            // scDTContactHisto
            // 
            this.scDTContactHisto.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scDTContactHisto.IsSplitterFixed = true;
            this.scDTContactHisto.Location = new System.Drawing.Point(3, 32);
            this.scDTContactHisto.Margin = new System.Windows.Forms.Padding(2);
            this.scDTContactHisto.Name = "scDTContactHisto";
            this.scDTContactHisto.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scDTContactHisto.Panel1
            // 
            this.scDTContactHisto.Panel1.Controls.Add(this.picbxDTContactRXHisto);
            // 
            // scDTContactHisto.Panel2
            // 
            this.scDTContactHisto.Panel2.Controls.Add(this.picbxDTContactTXHisto);
            this.scDTContactHisto.Size = new System.Drawing.Size(1168, 530);
            this.scDTContactHisto.SplitterDistance = 260;
            this.scDTContactHisto.SplitterWidth = 3;
            this.scDTContactHisto.TabIndex = 3;
            this.scDTContactHisto.Resize += new System.EventHandler(this.scDTContactHisto_Resize);
            // 
            // picbxDTContactRXHisto
            // 
            this.picbxDTContactRXHisto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxDTContactRXHisto.Location = new System.Drawing.Point(0, 0);
            this.picbxDTContactRXHisto.Margin = new System.Windows.Forms.Padding(2);
            this.picbxDTContactRXHisto.Name = "picbxDTContactRXHisto";
            this.picbxDTContactRXHisto.Size = new System.Drawing.Size(1168, 260);
            this.picbxDTContactRXHisto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxDTContactRXHisto.TabIndex = 0;
            this.picbxDTContactRXHisto.TabStop = false;
            // 
            // picbxDTContactTXHisto
            // 
            this.picbxDTContactTXHisto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxDTContactTXHisto.Location = new System.Drawing.Point(0, 0);
            this.picbxDTContactTXHisto.Margin = new System.Windows.Forms.Padding(2);
            this.picbxDTContactTXHisto.Name = "picbxDTContactTXHisto";
            this.picbxDTContactTXHisto.Size = new System.Drawing.Size(1168, 267);
            this.picbxDTContactTXHisto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxDTContactTXHisto.TabIndex = 0;
            this.picbxDTContactTXHisto.TabStop = false;
            // 
            // tpgDTHoverTRxS
            // 
            this.tpgDTHoverTRxS.BackColor = System.Drawing.SystemColors.Control;
            this.tpgDTHoverTRxS.Controls.Add(this.lblDTHoverTRxSDataName);
            this.tpgDTHoverTRxS.Controls.Add(this.cbxDTHoverTRxSDataName);
            this.tpgDTHoverTRxS.Controls.Add(this.scDTHoverTRxS);
            this.tpgDTHoverTRxS.Location = new System.Drawing.Point(4, 26);
            this.tpgDTHoverTRxS.Margin = new System.Windows.Forms.Padding(2);
            this.tpgDTHoverTRxS.Name = "tpgDTHoverTRxS";
            this.tpgDTHoverTRxS.Size = new System.Drawing.Size(1173, 568);
            this.tpgDTHoverTRxS.TabIndex = 3;
            this.tpgDTHoverTRxS.Text = "HoverTRxS";
            // 
            // lblDTHoverTRxSDataName
            // 
            this.lblDTHoverTRxSDataName.AutoSize = true;
            this.lblDTHoverTRxSDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDTHoverTRxSDataName.Location = new System.Drawing.Point(4, 5);
            this.lblDTHoverTRxSDataName.Name = "lblDTHoverTRxSDataName";
            this.lblDTHoverTRxSDataName.Size = new System.Drawing.Size(86, 19);
            this.lblDTHoverTRxSDataName.TabIndex = 6;
            this.lblDTHoverTRxSDataName.Text = "Data Name :";
            // 
            // cbxDTHoverTRxSDataName
            // 
            this.cbxDTHoverTRxSDataName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDTHoverTRxSDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxDTHoverTRxSDataName.FormattingEnabled = true;
            this.cbxDTHoverTRxSDataName.Location = new System.Drawing.Point(99, 2);
            this.cbxDTHoverTRxSDataName.Name = "cbxDTHoverTRxSDataName";
            this.cbxDTHoverTRxSDataName.Size = new System.Drawing.Size(240, 27);
            this.cbxDTHoverTRxSDataName.TabIndex = 7;
            this.cbxDTHoverTRxSDataName.SelectedIndexChanged += new System.EventHandler(this.cbxDTHoverTRxSDataName_SelectedIndexChanged);
            // 
            // scDTHoverTRxS
            // 
            this.scDTHoverTRxS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scDTHoverTRxS.IsSplitterFixed = true;
            this.scDTHoverTRxS.Location = new System.Drawing.Point(3, 32);
            this.scDTHoverTRxS.Margin = new System.Windows.Forms.Padding(2);
            this.scDTHoverTRxS.Name = "scDTHoverTRxS";
            this.scDTHoverTRxS.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scDTHoverTRxS.Panel1
            // 
            this.scDTHoverTRxS.Panel1.Controls.Add(this.picbxDTHoverTRxSRX);
            // 
            // scDTHoverTRxS.Panel2
            // 
            this.scDTHoverTRxS.Panel2.Controls.Add(this.picbxDTHoverTRxSTX);
            this.scDTHoverTRxS.Size = new System.Drawing.Size(1168, 530);
            this.scDTHoverTRxS.SplitterDistance = 260;
            this.scDTHoverTRxS.SplitterWidth = 3;
            this.scDTHoverTRxS.TabIndex = 5;
            this.scDTHoverTRxS.Resize += new System.EventHandler(this.scDTHoverTRxS_Resize);
            // 
            // picbxDTHoverTRxSRX
            // 
            this.picbxDTHoverTRxSRX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxDTHoverTRxSRX.Location = new System.Drawing.Point(0, 0);
            this.picbxDTHoverTRxSRX.Margin = new System.Windows.Forms.Padding(2);
            this.picbxDTHoverTRxSRX.Name = "picbxDTHoverTRxSRX";
            this.picbxDTHoverTRxSRX.Size = new System.Drawing.Size(1168, 260);
            this.picbxDTHoverTRxSRX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxDTHoverTRxSRX.TabIndex = 0;
            this.picbxDTHoverTRxSRX.TabStop = false;
            // 
            // picbxDTHoverTRxSTX
            // 
            this.picbxDTHoverTRxSTX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxDTHoverTRxSTX.Location = new System.Drawing.Point(0, 0);
            this.picbxDTHoverTRxSTX.Margin = new System.Windows.Forms.Padding(2);
            this.picbxDTHoverTRxSTX.Name = "picbxDTHoverTRxSTX";
            this.picbxDTHoverTRxSTX.Size = new System.Drawing.Size(1168, 267);
            this.picbxDTHoverTRxSTX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxDTHoverTRxSTX.TabIndex = 0;
            this.picbxDTHoverTRxSTX.TabStop = false;
            // 
            // tpgDTContactTRxS
            // 
            this.tpgDTContactTRxS.BackColor = System.Drawing.SystemColors.Control;
            this.tpgDTContactTRxS.Controls.Add(this.lblDTContactTRxSDataName);
            this.tpgDTContactTRxS.Controls.Add(this.cbxDTContactTRxSDataName);
            this.tpgDTContactTRxS.Controls.Add(this.scDTContactTRxS);
            this.tpgDTContactTRxS.Location = new System.Drawing.Point(4, 26);
            this.tpgDTContactTRxS.Margin = new System.Windows.Forms.Padding(2);
            this.tpgDTContactTRxS.Name = "tpgDTContactTRxS";
            this.tpgDTContactTRxS.Size = new System.Drawing.Size(1173, 568);
            this.tpgDTContactTRxS.TabIndex = 4;
            this.tpgDTContactTRxS.Text = "ContactTRxS";
            // 
            // lblDTContactTRxSDataName
            // 
            this.lblDTContactTRxSDataName.AutoSize = true;
            this.lblDTContactTRxSDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDTContactTRxSDataName.Location = new System.Drawing.Point(4, 5);
            this.lblDTContactTRxSDataName.Name = "lblDTContactTRxSDataName";
            this.lblDTContactTRxSDataName.Size = new System.Drawing.Size(86, 19);
            this.lblDTContactTRxSDataName.TabIndex = 9;
            this.lblDTContactTRxSDataName.Text = "Data Name :";
            // 
            // cbxDTContactTRxSDataName
            // 
            this.cbxDTContactTRxSDataName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDTContactTRxSDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxDTContactTRxSDataName.FormattingEnabled = true;
            this.cbxDTContactTRxSDataName.Location = new System.Drawing.Point(99, 2);
            this.cbxDTContactTRxSDataName.Name = "cbxDTContactTRxSDataName";
            this.cbxDTContactTRxSDataName.Size = new System.Drawing.Size(240, 27);
            this.cbxDTContactTRxSDataName.TabIndex = 10;
            this.cbxDTContactTRxSDataName.SelectedIndexChanged += new System.EventHandler(this.cbxDTContactTRxSDataName_SelectedIndexChanged);
            // 
            // scDTContactTRxS
            // 
            this.scDTContactTRxS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scDTContactTRxS.IsSplitterFixed = true;
            this.scDTContactTRxS.Location = new System.Drawing.Point(3, 32);
            this.scDTContactTRxS.Margin = new System.Windows.Forms.Padding(2);
            this.scDTContactTRxS.Name = "scDTContactTRxS";
            this.scDTContactTRxS.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scDTContactTRxS.Panel1
            // 
            this.scDTContactTRxS.Panel1.Controls.Add(this.picbxDTContactTRxSRX);
            // 
            // scDTContactTRxS.Panel2
            // 
            this.scDTContactTRxS.Panel2.Controls.Add(this.picbxDTContactTRxSTX);
            this.scDTContactTRxS.Size = new System.Drawing.Size(1168, 530);
            this.scDTContactTRxS.SplitterDistance = 260;
            this.scDTContactTRxS.SplitterWidth = 3;
            this.scDTContactTRxS.TabIndex = 8;
            this.scDTContactTRxS.Resize += new System.EventHandler(this.scDTContactTRxS_Resize);
            // 
            // picbxDTContactTRxSRX
            // 
            this.picbxDTContactTRxSRX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxDTContactTRxSRX.Location = new System.Drawing.Point(0, 0);
            this.picbxDTContactTRxSRX.Margin = new System.Windows.Forms.Padding(2);
            this.picbxDTContactTRxSRX.Name = "picbxDTContactTRxSRX";
            this.picbxDTContactTRxSRX.Size = new System.Drawing.Size(1168, 260);
            this.picbxDTContactTRxSRX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxDTContactTRxSRX.TabIndex = 0;
            this.picbxDTContactTRxSRX.TabStop = false;
            // 
            // picbxDTContactTRxSTX
            // 
            this.picbxDTContactTRxSTX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picbxDTContactTRxSTX.Location = new System.Drawing.Point(0, 0);
            this.picbxDTContactTRxSTX.Margin = new System.Windows.Forms.Padding(2);
            this.picbxDTContactTRxSTX.Name = "picbxDTContactTRxSTX";
            this.picbxDTContactTRxSTX.Size = new System.Drawing.Size(1168, 267);
            this.picbxDTContactTRxSTX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxDTContactTRxSTX.TabIndex = 0;
            this.picbxDTContactTRxSTX.TabStop = false;
            // 
            // frmResultChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1195, 636);
            this.Controls.Add(this.tcMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmResultChart";
            this.Text = "Chart";
            this.Load += new System.EventHandler(this.frmChart_Load);
            this.tcNoise.ResumeLayout(false);
            this.tpgNoiseTraceLineChart.ResumeLayout(false);
            this.tpgNoiseTraceLineChart.PerformLayout();
            this.scNoiseTraceLineChart.Panel1.ResumeLayout(false);
            this.scNoiseTraceLineChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scNoiseTraceLineChart)).EndInit();
            this.scNoiseTraceLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseRXTrace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseTXTrace)).EndInit();
            this.tpgNoiseFrequencyLineChart.ResumeLayout(false);
            this.tpgNoiseFrequencyLineChart.PerformLayout();
            this.tcNoiseFrequency.ResumeLayout(false);
            this.tpgNoiseRXFrequency.ResumeLayout(false);
            this.scNoiseRXFreqLineChart.Panel1.ResumeLayout(false);
            this.scNoiseRXFreqLineChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scNoiseRXFreqLineChart)).EndInit();
            this.scNoiseRXFreqLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseRXInnerFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseRXEdgeFreq)).EndInit();
            this.tpgNoiseTXFrequency.ResumeLayout(false);
            this.scNoiseTXFreqLineChart.Panel1.ResumeLayout(false);
            this.scNoiseTXFreqLineChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scNoiseTXFreqLineChart)).EndInit();
            this.scNoiseTXFreqLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseTXInnerFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseTXEdgeFreq)).EndInit();
            this.tpgNoiseTotalFrequency.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxNoiseTotalFreq)).EndInit();
            this.tcMain.ResumeLayout(false);
            this.tpgNoise.ResumeLayout(false);
            this.tpgTiltNoise.ResumeLayout(false);
            this.tcTiltNoise.ResumeLayout(false);
            this.tpgTNTraceLineChart.ResumeLayout(false);
            this.tpgTNTraceLineChart.PerformLayout();
            this.scTNTraceLineChart.Panel1.ResumeLayout(false);
            this.scTNTraceLineChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scTNTraceLineChart)).EndInit();
            this.scTNTraceLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNRXTrace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTXTrace)).EndInit();
            this.tpgTNFrequencyLineChart.ResumeLayout(false);
            this.tpgTNFrequencyLineChart.PerformLayout();
            this.tcTNFrequency.ResumeLayout(false);
            this.tpgTNPTHFRXFreq.ResumeLayout(false);
            this.scTNPTHFRXFreqLineChart.Panel1.ResumeLayout(false);
            this.scTNPTHFRXFreqLineChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scTNPTHFRXFreqLineChart)).EndInit();
            this.scTNPTHFRXFreqLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNPTHFRXInnerFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNPTHFRXEdgeFreq)).EndInit();
            this.tpgTNPTHFTXFreq.ResumeLayout(false);
            this.scTNPTHFTXFreqLineChart.Panel1.ResumeLayout(false);
            this.scTNPTHFTXFreqLineChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scTNPTHFTXFreqLineChart)).EndInit();
            this.scTNPTHFTXFreqLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNPTHFTXInnerFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNPTHFTXEdgeFreq)).EndInit();
            this.tpgTNPTHFFreq.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNPTHFFreq)).EndInit();
            this.tpgTNBHFRXFreq.ResumeLayout(false);
            this.scTNBHFRXFreqLineChart.Panel1.ResumeLayout(false);
            this.scTNBHFRXFreqLineChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scTNBHFRXFreqLineChart)).EndInit();
            this.scTNBHFRXFreqLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNBHFRXInnerFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNBHFRXEdgeFreq)).EndInit();
            this.tpgTNBHFTXFreq.ResumeLayout(false);
            this.scTNBHFTXFreqLineChart.Panel1.ResumeLayout(false);
            this.scTNBHFTXFreqLineChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scTNBHFTXFreqLineChart)).EndInit();
            this.scTNBHFTXFreqLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNBHFTXInnerFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNBHFTXEdgeFreq)).EndInit();
            this.tpgTNBHFFreq.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNBHFFreq)).EndInit();
            this.tpgTNRXFreq.ResumeLayout(false);
            this.scTNTotalRXLineChart.Panel1.ResumeLayout(false);
            this.scTNTotalRXLineChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scTNTotalRXLineChart)).EndInit();
            this.scTNTotalRXLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTotalRXInnerFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTotalRXEdgeFreq)).EndInit();
            this.tpgTNFreq.ResumeLayout(false);
            this.scTNTotalTXFreqLineChart.Panel1.ResumeLayout(false);
            this.scTNTotalTXFreqLineChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scTNTotalTXFreqLineChart)).EndInit();
            this.scTNTotalTXFreqLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTotalTXInnerFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTotalTXEdgeFreq)).EndInit();
            this.tpgTNTotalFreq.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxTNTotalFreq)).EndInit();
            this.tpgCompositeLineChart.ResumeLayout(false);
            this.tpgCompositeLineChart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxCompositeLineChart)).EndInit();
            this.tpgDigitalTuning.ResumeLayout(false);
            this.tcDigitalTuning.ResumeLayout(false);
            this.tpgDTHover1stHisto.ResumeLayout(false);
            this.tpgDTHover1stHisto.PerformLayout();
            this.scDTHover1stHisto.Panel1.ResumeLayout(false);
            this.scDTHover1stHisto.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDTHover1stHisto)).EndInit();
            this.scDTHover1stHisto.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHover1stRXHisto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHover1stTXHisto)).EndInit();
            this.tpgDTHover2ndHisto.ResumeLayout(false);
            this.tpgDTHover2ndHisto.PerformLayout();
            this.scDTHover2ndHisto.Panel1.ResumeLayout(false);
            this.scDTHover2ndHisto.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDTHover2ndHisto)).EndInit();
            this.scDTHover2ndHisto.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHover2ndRXHisto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHover2ndTXHisto)).EndInit();
            this.tpgDTContactHisto.ResumeLayout(false);
            this.tpgDTContactHisto.PerformLayout();
            this.scDTContactHisto.Panel1.ResumeLayout(false);
            this.scDTContactHisto.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDTContactHisto)).EndInit();
            this.scDTContactHisto.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTContactRXHisto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTContactTXHisto)).EndInit();
            this.tpgDTHoverTRxS.ResumeLayout(false);
            this.tpgDTHoverTRxS.PerformLayout();
            this.scDTHoverTRxS.Panel1.ResumeLayout(false);
            this.scDTHoverTRxS.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDTHoverTRxS)).EndInit();
            this.scDTHoverTRxS.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHoverTRxSRX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTHoverTRxSTX)).EndInit();
            this.tpgDTContactTRxS.ResumeLayout(false);
            this.tpgDTContactTRxS.PerformLayout();
            this.scDTContactTRxS.Panel1.ResumeLayout(false);
            this.scDTContactTRxS.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDTContactTRxS)).EndInit();
            this.scDTContactTRxS.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTContactTRxSRX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxDTContactTRxSTX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcNoise;
        private System.Windows.Forms.TabPage tpgNoiseTraceLineChart;
        private System.Windows.Forms.SplitContainer scNoiseTraceLineChart;
        private System.Windows.Forms.Label lblNoiseTraceLineChartData;
        private System.Windows.Forms.ComboBox cbxNoiseTraceLineChartData;
        private System.Windows.Forms.PictureBox picbxNoiseRXTrace;
        private System.Windows.Forms.PictureBox picbxNoiseTXTrace;
        private System.Windows.Forms.TabPage tpgNoiseFrequencyLineChart;
        private System.Windows.Forms.TabControl tcNoiseFrequency;
        private System.Windows.Forms.TabPage tpgNoiseRXFrequency;
        private System.Windows.Forms.SplitContainer scNoiseRXFreqLineChart;
        private System.Windows.Forms.PictureBox picbxNoiseRXInnerFreq;
        private System.Windows.Forms.PictureBox picbxNoiseRXEdgeFreq;
        private System.Windows.Forms.TabPage tpgNoiseTXFrequency;
        private System.Windows.Forms.SplitContainer scNoiseTXFreqLineChart;
        private System.Windows.Forms.PictureBox picbxNoiseTXInnerFreq;
        private System.Windows.Forms.PictureBox picbxNoiseTXEdgeFreq;
        private System.Windows.Forms.TabPage tpgNoiseTotalFrequency;
        private System.Windows.Forms.PictureBox picbxNoiseTotalFreq;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpgNoise;
        private System.Windows.Forms.TabPage tpgDigitalTuning;
        private System.Windows.Forms.TabControl tcDigitalTuning;
        private System.Windows.Forms.TabPage tpgDTHover1stHisto;
        private System.Windows.Forms.TabPage tpgDTHover2ndHisto;
        private System.Windows.Forms.Label lblDTHover1stDataName;
        private System.Windows.Forms.ComboBox cbxDTHover1stDataName;
        private System.Windows.Forms.SplitContainer scDTHover1stHisto;
        private System.Windows.Forms.PictureBox picbxDTHover1stRXHisto;
        private System.Windows.Forms.PictureBox picbxDTHover1stTXHisto;
        private System.Windows.Forms.Label lblDTHover2ndDataName;
        private System.Windows.Forms.ComboBox cbxDTHover2ndDataName;
        private System.Windows.Forms.SplitContainer scDTHover2ndHisto;
        private System.Windows.Forms.PictureBox picbxDTHover2ndRXHisto;
        private System.Windows.Forms.PictureBox picbxDTHover2ndTXHisto;
        private System.Windows.Forms.TabPage tpgDTContactHisto;
        private System.Windows.Forms.Label lblDTContactDataName;
        private System.Windows.Forms.ComboBox cbxDTContactDataName;
        private System.Windows.Forms.SplitContainer scDTContactHisto;
        private System.Windows.Forms.PictureBox picbxDTContactRXHisto;
        private System.Windows.Forms.PictureBox picbxDTContactTXHisto;
        private System.Windows.Forms.TabPage tpgTiltNoise;
        private System.Windows.Forms.TabControl tcTiltNoise;
        private System.Windows.Forms.TabPage tpgTNTraceLineChart;
        private System.Windows.Forms.Label lblTNTraceLineChartData;
        private System.Windows.Forms.ComboBox cbxTNTraceLineChartData;
        private System.Windows.Forms.SplitContainer scTNTraceLineChart;
        private System.Windows.Forms.PictureBox picbxTNRXTrace;
        private System.Windows.Forms.PictureBox picbxTNTXTrace;
        private System.Windows.Forms.TabPage tpgTNFrequencyLineChart;
        private System.Windows.Forms.TabControl tcTNFrequency;
        private System.Windows.Forms.TabPage tpgTNPTHFRXFreq;
        private System.Windows.Forms.SplitContainer scTNPTHFRXFreqLineChart;
        private System.Windows.Forms.PictureBox picbxTNPTHFRXInnerFreq;
        private System.Windows.Forms.PictureBox picbxTNPTHFRXEdgeFreq;
        private System.Windows.Forms.TabPage tpgTNPTHFTXFreq;
        private System.Windows.Forms.SplitContainer scTNPTHFTXFreqLineChart;
        private System.Windows.Forms.PictureBox picbxTNPTHFTXInnerFreq;
        private System.Windows.Forms.PictureBox picbxTNPTHFTXEdgeFreq;
        private System.Windows.Forms.TabPage tpgTNPTHFFreq;
        private System.Windows.Forms.PictureBox picbxTNPTHFFreq;
        private System.Windows.Forms.TabPage tpgTNBHFRXFreq;
        private System.Windows.Forms.TabPage tpgTNBHFTXFreq;
        private System.Windows.Forms.TabPage tpgTNBHFFreq;
        private System.Windows.Forms.SplitContainer scTNBHFRXFreqLineChart;
        private System.Windows.Forms.PictureBox picbxTNBHFRXInnerFreq;
        private System.Windows.Forms.PictureBox picbxTNBHFRXEdgeFreq;
        private System.Windows.Forms.SplitContainer scTNBHFTXFreqLineChart;
        private System.Windows.Forms.PictureBox picbxTNBHFFreq;
        private System.Windows.Forms.PictureBox picbxTNBHFTXInnerFreq;
        private System.Windows.Forms.PictureBox picbxTNBHFTXEdgeFreq;
        private System.Windows.Forms.TabPage tpgTNRXFreq;
        private System.Windows.Forms.SplitContainer scTNTotalRXLineChart;
        private System.Windows.Forms.PictureBox picbxTNTotalRXInnerFreq;
        private System.Windows.Forms.PictureBox picbxTNTotalRXEdgeFreq;
        private System.Windows.Forms.TabPage tpgTNFreq;
        private System.Windows.Forms.SplitContainer scTNTotalTXFreqLineChart;
        private System.Windows.Forms.PictureBox picbxTNTotalTXInnerFreq;
        private System.Windows.Forms.PictureBox picbxTNTotalTXEdgeFreq;
        private System.Windows.Forms.TabPage tpgTNTotalFreq;
        private System.Windows.Forms.PictureBox picbxTNTotalFreq;
        private System.Windows.Forms.TabPage tpgDTHoverTRxS;
        private System.Windows.Forms.Label lblDTHoverTRxSDataName;
        private System.Windows.Forms.ComboBox cbxDTHoverTRxSDataName;
        private System.Windows.Forms.SplitContainer scDTHoverTRxS;
        private System.Windows.Forms.PictureBox picbxDTHoverTRxSRX;
        private System.Windows.Forms.PictureBox picbxDTHoverTRxSTX;
        private System.Windows.Forms.TabPage tpgDTContactTRxS;
        private System.Windows.Forms.Label lblDTContactTRxSDataName;
        private System.Windows.Forms.ComboBox cbxDTContactTRxSDataName;
        private System.Windows.Forms.SplitContainer scDTContactTRxS;
        private System.Windows.Forms.PictureBox picbxDTContactTRxSRX;
        private System.Windows.Forms.PictureBox picbxDTContactTRxSTX;
        private System.Windows.Forms.TabPage tpgCompositeLineChart;
        private System.Windows.Forms.PictureBox picbxCompositeLineChart;
        private System.Windows.Forms.CheckBox ckbxNoiseFreqChartIncludeMaxValue;
        private System.Windows.Forms.CheckBox ckbxTNFrequencyLineIncludeMaxValue;
        private System.Windows.Forms.CheckBox ckbxTNCompositeLineIncludeMaxValue;
    }
}