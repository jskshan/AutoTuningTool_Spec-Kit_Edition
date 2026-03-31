namespace MPPPenAutoTuning
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusstripMain = new System.Windows.Forms.StatusStrip();
            this.toolstripprogressbarMain = new System.Windows.Forms.ToolStripProgressBar();
            this.toolstripstatuslblProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolstripstatuslblStep = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolstripmenuitemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.menustripTop = new System.Windows.Forms.MenuStrip();
            this.toolstripmenuitemSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemStepSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemFlowSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemParameterSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemFrequencySetting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemSummarizeLogData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemGoDrawController = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemDFTNUMAndCoeffConverter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemOther = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemFeedback = new System.Windows.Forms.ToolStripMenuItem();
            this.splitcontainerTopRegion = new System.Windows.Forms.SplitContainer();
            this.pnlStep = new System.Windows.Forms.Panel();
            this.StepLbl = new System.Windows.Forms.Label();
            this.lblStepItem = new System.Windows.Forms.Label();
            this.picbxSplitLine = new System.Windows.Forms.PictureBox();
            this.btnNewDraw = new System.Windows.Forms.Button();
            this.btnNewStop = new System.Windows.Forms.Button();
            this.btnNewPattern = new System.Windows.Forms.Button();
            this.btnNewStart = new System.Windows.Forms.Button();
            this.btnNewConnect = new System.Windows.Forms.Button();
            this.cbxModeState = new System.Windows.Forms.ComboBox();
            this.lblModeState = new System.Windows.Forms.Label();
            this.pnlResultMessage = new System.Windows.Forms.Panel();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlMode = new System.Windows.Forms.Panel();
            this.lblMode = new System.Windows.Forms.Label();
            this.splitcontainerOutput = new System.Windows.Forms.SplitContainer();
            this.gbxMessage = new System.Windows.Forms.GroupBox();
            this.rtbxMessage = new System.Windows.Forms.RichTextBox();
            this.btnChart = new System.Windows.Forms.Button();
            this.tcResult = new System.Windows.Forms.TabControl();
            this.tpgNoiseResult = new System.Windows.Forms.TabPage();
            this.gbxNoiseResult = new System.Windows.Forms.GroupBox();
            this.dgvNoiseRank = new System.Windows.Forms.DataGridView();
            this.tpgTNResult = new System.Windows.Forms.TabPage();
            this.gbxTNResult = new System.Windows.Forms.GroupBox();
            this.tcTNResult = new System.Windows.Forms.TabControl();
            this.tpgTNPTHF = new System.Windows.Forms.TabPage();
            this.rtbxTNPTHFResultList = new System.Windows.Forms.RichTextBox();
            this.tpgTNPTHFRank = new System.Windows.Forms.TabPage();
            this.dgvTNPTHFRank = new System.Windows.Forms.DataGridView();
            this.tpgTNBHF = new System.Windows.Forms.TabPage();
            this.rtbxTNBHFResultList = new System.Windows.Forms.RichTextBox();
            this.tpgTNBHFRank = new System.Windows.Forms.TabPage();
            this.dgvTNBHFRank = new System.Windows.Forms.DataGridView();
            this.tpgTNPreliminaryList = new System.Windows.Forms.TabPage();
            this.rtbxTNPreliminaryList = new System.Windows.Forms.RichTextBox();
            this.tpgTNTotalRank = new System.Windows.Forms.TabPage();
            this.dgvTNTotalRank = new System.Windows.Forms.DataGridView();
            this.tpgDGTResult = new System.Windows.Forms.TabPage();
            this.gbxDGTResult = new System.Windows.Forms.GroupBox();
            this.tcDGTResult = new System.Windows.Forms.TabControl();
            this.tpgDGTTotalList = new System.Windows.Forms.TabPage();
            this.rtbxDGTTotalList = new System.Windows.Forms.RichTextBox();
            this.tpgTPGTResult = new System.Windows.Forms.TabPage();
            this.gbxTPGTResult = new System.Windows.Forms.GroupBox();
            this.tcTPGTResult = new System.Windows.Forms.TabControl();
            this.tpgTPGTTotalList = new System.Windows.Forms.TabPage();
            this.rtbxTPGTTotalList = new System.Windows.Forms.RichTextBox();
            this.tpgPCTResult = new System.Windows.Forms.TabPage();
            this.gbxPCTResult = new System.Windows.Forms.GroupBox();
            this.tcPCTResult = new System.Windows.Forms.TabControl();
            this.tpgPCHover_1st = new System.Windows.Forms.TabPage();
            this.rtbxPCTHover_1stResultList = new System.Windows.Forms.RichTextBox();
            this.tpgPCHover_2nd = new System.Windows.Forms.TabPage();
            this.rtbxPCTHover_2ndResultList = new System.Windows.Forms.RichTextBox();
            this.tpgPCContact = new System.Windows.Forms.TabPage();
            this.rtbxPCTContactResultList = new System.Windows.Forms.RichTextBox();
            this.tpgPCTotalList = new System.Windows.Forms.TabPage();
            this.rtbxPCTTotalList = new System.Windows.Forms.RichTextBox();
            this.tpgDTResult = new System.Windows.Forms.TabPage();
            this.gbxDTResult = new System.Windows.Forms.GroupBox();
            this.tcDTResult = new System.Windows.Forms.TabControl();
            this.tpgDTHover_1st = new System.Windows.Forms.TabPage();
            this.rtbxDTHover_1stResultList = new System.Windows.Forms.RichTextBox();
            this.tpgDTHover_2nd = new System.Windows.Forms.TabPage();
            this.rtbxDTHover_2ndResultList = new System.Windows.Forms.RichTextBox();
            this.tpgDTContact = new System.Windows.Forms.TabPage();
            this.rtbxDTContactResultList = new System.Windows.Forms.RichTextBox();
            this.tpgDTPreliminaryList = new System.Windows.Forms.TabPage();
            this.rtbxDTPreliminaryList = new System.Windows.Forms.RichTextBox();
            this.tpgDTHoverTRxS = new System.Windows.Forms.TabPage();
            this.rtbxDTHoverTRxSResultList = new System.Windows.Forms.RichTextBox();
            this.tpgDTContactTRxS = new System.Windows.Forms.TabPage();
            this.rtbxDTContactTRxSResultList = new System.Windows.Forms.RichTextBox();
            this.tpgDTTotalList = new System.Windows.Forms.TabPage();
            this.rtbxDTTotalList = new System.Windows.Forms.RichTextBox();
            this.tpgTTResult = new System.Windows.Forms.TabPage();
            this.gbxTTResult = new System.Windows.Forms.GroupBox();
            this.tcTTResult = new System.Windows.Forms.TabControl();
            this.tpgTTPTHF = new System.Windows.Forms.TabPage();
            this.rtbxTTPTHFResultList = new System.Windows.Forms.RichTextBox();
            this.tpgTTBHF = new System.Windows.Forms.TabPage();
            this.rtbxTTBHFResultList = new System.Windows.Forms.RichTextBox();
            this.tpgTTRank = new System.Windows.Forms.TabPage();
            this.dgvTTRank = new System.Windows.Forms.DataGridView();
            this.tpgTTTotalList = new System.Windows.Forms.TabPage();
            this.rtbxTTTotalList = new System.Windows.Forms.RichTextBox();
            this.tpgPTResult = new System.Windows.Forms.TabPage();
            this.gbxPTResult = new System.Windows.Forms.GroupBox();
            this.tcPTResult = new System.Windows.Forms.TabControl();
            this.tpgPTTotalList = new System.Windows.Forms.TabPage();
            this.rtbxPTTotalList = new System.Windows.Forms.RichTextBox();
            this.tpgPTPressureTable = new System.Windows.Forms.TabPage();
            this.rtbxPTPressureTable = new System.Windows.Forms.RichTextBox();
            this.tpgLTResult = new System.Windows.Forms.TabPage();
            this.gbxLTResult = new System.Windows.Forms.GroupBox();
            this.tcLTResult = new System.Windows.Forms.TabControl();
            this.tpgLTTotalList = new System.Windows.Forms.TabPage();
            this.rtbxLTTotalList = new System.Windows.Forms.RichTextBox();
            this.tpgLTRX5TTable = new System.Windows.Forms.TabPage();
            this.rtbxRX5TTable = new System.Windows.Forms.RichTextBox();
            this.tpgLTTX5TTable = new System.Windows.Forms.TabPage();
            this.rtbxTX5TTable = new System.Windows.Forms.RichTextBox();
            this.tpgLTRX3TTable = new System.Windows.Forms.TabPage();
            this.rtbxRX3TTable = new System.Windows.Forms.RichTextBox();
            this.tpgLTTX3TTable = new System.Windows.Forms.TabPage();
            this.rtbxTX3TTable = new System.Windows.Forms.RichTextBox();
            this.tpgLTRX2TLETable = new System.Windows.Forms.TabPage();
            this.rtbxRX2TLETable = new System.Windows.Forms.RichTextBox();
            this.tpgLTTX2TLETable = new System.Windows.Forms.TabPage();
            this.rtbxTX2TLETable = new System.Windows.Forms.RichTextBox();
            this.tpgLTRX2THETable = new System.Windows.Forms.TabPage();
            this.rtbxRX2THETable = new System.Windows.Forms.RichTextBox();
            this.tpgTX2THETable = new System.Windows.Forms.TabPage();
            this.rtbxTX2THETable = new System.Windows.Forms.RichTextBox();
            this.tpgCostTime = new System.Windows.Forms.TabPage();
            this.gbxCostTime = new System.Windows.Forms.GroupBox();
            this.toolstripmenuitemRedmineTask = new System.Windows.Forms.ToolStripMenuItem();
            this.statusstripMain.SuspendLayout();
            this.menustripTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainerTopRegion)).BeginInit();
            this.splitcontainerTopRegion.Panel1.SuspendLayout();
            this.splitcontainerTopRegion.Panel2.SuspendLayout();
            this.splitcontainerTopRegion.SuspendLayout();
            this.pnlStep.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxSplitLine)).BeginInit();
            this.pnlResultMessage.SuspendLayout();
            this.pnlMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainerOutput)).BeginInit();
            this.splitcontainerOutput.Panel1.SuspendLayout();
            this.splitcontainerOutput.Panel2.SuspendLayout();
            this.splitcontainerOutput.SuspendLayout();
            this.gbxMessage.SuspendLayout();
            this.tcResult.SuspendLayout();
            this.tpgNoiseResult.SuspendLayout();
            this.gbxNoiseResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNoiseRank)).BeginInit();
            this.tpgTNResult.SuspendLayout();
            this.gbxTNResult.SuspendLayout();
            this.tcTNResult.SuspendLayout();
            this.tpgTNPTHF.SuspendLayout();
            this.tpgTNPTHFRank.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTNPTHFRank)).BeginInit();
            this.tpgTNBHF.SuspendLayout();
            this.tpgTNBHFRank.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTNBHFRank)).BeginInit();
            this.tpgTNPreliminaryList.SuspendLayout();
            this.tpgTNTotalRank.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTNTotalRank)).BeginInit();
            this.tpgDGTResult.SuspendLayout();
            this.gbxDGTResult.SuspendLayout();
            this.tcDGTResult.SuspendLayout();
            this.tpgDGTTotalList.SuspendLayout();
            this.tpgTPGTResult.SuspendLayout();
            this.gbxTPGTResult.SuspendLayout();
            this.tcTPGTResult.SuspendLayout();
            this.tpgTPGTTotalList.SuspendLayout();
            this.tpgPCTResult.SuspendLayout();
            this.gbxPCTResult.SuspendLayout();
            this.tcPCTResult.SuspendLayout();
            this.tpgPCHover_1st.SuspendLayout();
            this.tpgPCHover_2nd.SuspendLayout();
            this.tpgPCContact.SuspendLayout();
            this.tpgPCTotalList.SuspendLayout();
            this.tpgDTResult.SuspendLayout();
            this.gbxDTResult.SuspendLayout();
            this.tcDTResult.SuspendLayout();
            this.tpgDTHover_1st.SuspendLayout();
            this.tpgDTHover_2nd.SuspendLayout();
            this.tpgDTContact.SuspendLayout();
            this.tpgDTPreliminaryList.SuspendLayout();
            this.tpgDTHoverTRxS.SuspendLayout();
            this.tpgDTContactTRxS.SuspendLayout();
            this.tpgDTTotalList.SuspendLayout();
            this.tpgTTResult.SuspendLayout();
            this.gbxTTResult.SuspendLayout();
            this.tcTTResult.SuspendLayout();
            this.tpgTTPTHF.SuspendLayout();
            this.tpgTTBHF.SuspendLayout();
            this.tpgTTRank.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTTRank)).BeginInit();
            this.tpgTTTotalList.SuspendLayout();
            this.tpgPTResult.SuspendLayout();
            this.gbxPTResult.SuspendLayout();
            this.tcPTResult.SuspendLayout();
            this.tpgPTTotalList.SuspendLayout();
            this.tpgPTPressureTable.SuspendLayout();
            this.tpgLTResult.SuspendLayout();
            this.gbxLTResult.SuspendLayout();
            this.tcLTResult.SuspendLayout();
            this.tpgLTTotalList.SuspendLayout();
            this.tpgLTRX5TTable.SuspendLayout();
            this.tpgLTTX5TTable.SuspendLayout();
            this.tpgLTRX3TTable.SuspendLayout();
            this.tpgLTTX3TTable.SuspendLayout();
            this.tpgLTRX2TLETable.SuspendLayout();
            this.tpgLTTX2TLETable.SuspendLayout();
            this.tpgLTRX2THETable.SuspendLayout();
            this.tpgTX2THETable.SuspendLayout();
            this.tpgCostTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusstripMain
            // 
            this.statusstripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusstripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripprogressbarMain,
            this.toolstripstatuslblProgress,
            this.toolstripstatuslblStep});
            this.statusstripMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusstripMain.Location = new System.Drawing.Point(0, 744);
            this.statusstripMain.Name = "statusstripMain";
            this.statusstripMain.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusstripMain.Size = new System.Drawing.Size(918, 22);
            this.statusstripMain.TabIndex = 18;
            // 
            // toolstripprogressbarMain
            // 
            this.toolstripprogressbarMain.Name = "toolstripprogressbarMain";
            this.toolstripprogressbarMain.Size = new System.Drawing.Size(200, 16);
            // 
            // toolstripstatuslblProgress
            // 
            this.toolstripstatuslblProgress.Name = "toolstripstatuslblProgress";
            this.toolstripstatuslblProgress.Size = new System.Drawing.Size(0, 17);
            // 
            // toolstripstatuslblStep
            // 
            this.toolstripstatuslblStep.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolstripstatuslblStep.Name = "toolstripstatuslblStep";
            this.toolstripstatuslblStep.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolstripstatuslblStep.Size = new System.Drawing.Size(0, 17);
            // 
            // toolstripmenuitemHelp
            // 
            this.toolstripmenuitemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuitemAbout});
            this.toolstripmenuitemHelp.Name = "toolstripmenuitemHelp";
            this.toolstripmenuitemHelp.Size = new System.Drawing.Size(45, 20);
            this.toolstripmenuitemHelp.Text = "Help";
            // 
            // toolstripmenuitemAbout
            // 
            this.toolstripmenuitemAbout.Name = "toolstripmenuitemAbout";
            this.toolstripmenuitemAbout.Size = new System.Drawing.Size(110, 22);
            this.toolstripmenuitemAbout.Text = "About";
            // 
            // menustripTop
            // 
            this.menustripTop.AllowMerge = false;
            this.menustripTop.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menustripTop.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menustripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuitemSetting,
            this.toolstripmenuitemHelp,
            this.toolToolStripMenuItem,
            this.toolstripmenuitemOther});
            this.menustripTop.Location = new System.Drawing.Point(0, 0);
            this.menustripTop.Name = "menustripTop";
            this.menustripTop.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menustripTop.Size = new System.Drawing.Size(918, 24);
            this.menustripTop.TabIndex = 16;
            this.menustripTop.Text = "menuStrip1";
            // 
            // toolstripmenuitemSetting
            // 
            this.toolstripmenuitemSetting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuitemStepSetting,
            this.toolstripmenuitemFlowSetting,
            this.toolstripmenuitemParameterSetting,
            this.toolstripmenuitemFrequencySetting});
            this.toolstripmenuitemSetting.Name = "toolstripmenuitemSetting";
            this.toolstripmenuitemSetting.Size = new System.Drawing.Size(59, 20);
            this.toolstripmenuitemSetting.Text = "Setting";
            // 
            // toolstripmenuitemStepSetting
            // 
            this.toolstripmenuitemStepSetting.Name = "toolstripmenuitemStepSetting";
            this.toolstripmenuitemStepSetting.Size = new System.Drawing.Size(179, 22);
            this.toolstripmenuitemStepSetting.Text = "Step Setting";
            this.toolstripmenuitemStepSetting.Click += new System.EventHandler(this.toolstripmenuitemStepSetting_Click);
            // 
            // toolstripmenuitemFlowSetting
            // 
            this.toolstripmenuitemFlowSetting.Name = "toolstripmenuitemFlowSetting";
            this.toolstripmenuitemFlowSetting.Size = new System.Drawing.Size(179, 22);
            this.toolstripmenuitemFlowSetting.Text = "Flow Setting";
            this.toolstripmenuitemFlowSetting.Click += new System.EventHandler(this.toolstripmenuitemFlowSetting_Click);
            // 
            // toolstripmenuitemParameterSetting
            // 
            this.toolstripmenuitemParameterSetting.Name = "toolstripmenuitemParameterSetting";
            this.toolstripmenuitemParameterSetting.Size = new System.Drawing.Size(179, 22);
            this.toolstripmenuitemParameterSetting.Text = "Parameter Setting";
            this.toolstripmenuitemParameterSetting.Click += new System.EventHandler(this.toolstripmenuitemParameterSetting_Click);
            // 
            // toolstripmenuitemFrequencySetting
            // 
            this.toolstripmenuitemFrequencySetting.Name = "toolstripmenuitemFrequencySetting";
            this.toolstripmenuitemFrequencySetting.Size = new System.Drawing.Size(179, 22);
            this.toolstripmenuitemFrequencySetting.Text = "Frequency Setting";
            this.toolstripmenuitemFrequencySetting.Click += new System.EventHandler(this.toolstripmenuitemFrequencySetting_Click);
            // 
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuitemSummarizeLogData,
            this.toolstripmenuitemGoDrawController,
            this.toolstripmenuitemDFTNUMAndCoeffConverter});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            this.toolToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.toolToolStripMenuItem.Text = "Tool";
            // 
            // toolstripmenuitemSummarizeLogData
            // 
            this.toolstripmenuitemSummarizeLogData.Name = "toolstripmenuitemSummarizeLogData";
            this.toolstripmenuitemSummarizeLogData.Size = new System.Drawing.Size(265, 22);
            this.toolstripmenuitemSummarizeLogData.Text = "Summarize Log Data(Multi-SKU)";
            this.toolstripmenuitemSummarizeLogData.Click += new System.EventHandler(this.toolstripmenuitemSummarizeLogData_Click);
            // 
            // toolstripmenuitemGoDrawController
            // 
            this.toolstripmenuitemGoDrawController.Name = "toolstripmenuitemGoDrawController";
            this.toolstripmenuitemGoDrawController.Size = new System.Drawing.Size(265, 22);
            this.toolstripmenuitemGoDrawController.Text = "GoDraw Controller";
            this.toolstripmenuitemGoDrawController.Click += new System.EventHandler(this.toolstripmenuitemGoDrawController_Click);
            // 
            // toolstripmenuitemDFTNUMAndCoeffConverter
            // 
            this.toolstripmenuitemDFTNUMAndCoeffConverter.Name = "toolstripmenuitemDFTNUMAndCoeffConverter";
            this.toolstripmenuitemDFTNUMAndCoeffConverter.Size = new System.Drawing.Size(265, 22);
            this.toolstripmenuitemDFTNUMAndCoeffConverter.Text = "DFT_NUM & Coeff Converter";
            this.toolstripmenuitemDFTNUMAndCoeffConverter.Click += new System.EventHandler(this.toolstripmenuitemDFTNUMAndCoeffConverter_Click);
            // 
            // toolstripmenuitemOther
            // 
            this.toolstripmenuitemOther.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuitemFeedback,
            this.toolstripmenuitemRedmineTask});
            this.toolstripmenuitemOther.Name = "toolstripmenuitemOther";
            this.toolstripmenuitemOther.Size = new System.Drawing.Size(52, 20);
            this.toolstripmenuitemOther.Text = "Other";
            // 
            // toolstripmenuitemFeedback
            // 
            this.toolstripmenuitemFeedback.Name = "toolstripmenuitemFeedback";
            this.toolstripmenuitemFeedback.Size = new System.Drawing.Size(156, 22);
            this.toolstripmenuitemFeedback.Text = "Feedback";
            this.toolstripmenuitemFeedback.Visible = false;
            this.toolstripmenuitemFeedback.Click += new System.EventHandler(this.toolstripmenuitemFeedback_Click);
            // 
            // splitcontainerTopRegion
            // 
            this.splitcontainerTopRegion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitcontainerTopRegion.IsSplitterFixed = true;
            this.splitcontainerTopRegion.Location = new System.Drawing.Point(1, 26);
            this.splitcontainerTopRegion.Name = "splitcontainerTopRegion";
            // 
            // splitcontainerTopRegion.Panel1
            // 
            this.splitcontainerTopRegion.Panel1.Controls.Add(this.pnlStep);
            // 
            // splitcontainerTopRegion.Panel2
            // 
            this.splitcontainerTopRegion.Panel2.Controls.Add(this.picbxSplitLine);
            this.splitcontainerTopRegion.Panel2.Controls.Add(this.btnNewDraw);
            this.splitcontainerTopRegion.Panel2.Controls.Add(this.btnNewStop);
            this.splitcontainerTopRegion.Panel2.Controls.Add(this.btnNewPattern);
            this.splitcontainerTopRegion.Panel2.Controls.Add(this.btnNewStart);
            this.splitcontainerTopRegion.Panel2.Controls.Add(this.btnNewConnect);
            this.splitcontainerTopRegion.Panel2.Controls.Add(this.cbxModeState);
            this.splitcontainerTopRegion.Panel2.Controls.Add(this.lblModeState);
            this.splitcontainerTopRegion.Panel2.Controls.Add(this.pnlResultMessage);
            this.splitcontainerTopRegion.Panel2.Controls.Add(this.pnlMode);
            this.splitcontainerTopRegion.Size = new System.Drawing.Size(917, 194);
            this.splitcontainerTopRegion.SplitterDistance = 137;
            this.splitcontainerTopRegion.TabIndex = 24;
            // 
            // pnlStep
            // 
            this.pnlStep.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlStep.BackColor = System.Drawing.Color.LightYellow;
            this.pnlStep.Controls.Add(this.StepLbl);
            this.pnlStep.Controls.Add(this.lblStepItem);
            this.pnlStep.Location = new System.Drawing.Point(0, 0);
            this.pnlStep.Name = "pnlStep";
            this.pnlStep.Size = new System.Drawing.Size(137, 194);
            this.pnlStep.TabIndex = 22;
            // 
            // StepLbl
            // 
            this.StepLbl.AutoSize = true;
            this.StepLbl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StepLbl.Location = new System.Drawing.Point(1, 1);
            this.StepLbl.Name = "StepLbl";
            this.StepLbl.Size = new System.Drawing.Size(37, 19);
            this.StepLbl.TabIndex = 22;
            this.StepLbl.Text = "Step";
            // 
            // lblStepItem
            // 
            this.lblStepItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStepItem.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStepItem.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepItem.Location = new System.Drawing.Point(3, 24);
            this.lblStepItem.Name = "lblStepItem";
            this.lblStepItem.Size = new System.Drawing.Size(131, 23);
            this.lblStepItem.TabIndex = 25;
            this.lblStepItem.Text = "StepItemLbl";
            this.lblStepItem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStepItem.Visible = false;
            // 
            // picbxSplitLine
            // 
            this.picbxSplitLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picbxSplitLine.Image = ((System.Drawing.Image)(resources.GetObject("picbxSplitLine.Image")));
            this.picbxSplitLine.InitialImage = ((System.Drawing.Image)(resources.GetObject("picbxSplitLine.InitialImage")));
            this.picbxSplitLine.Location = new System.Drawing.Point(639, 0);
            this.picbxSplitLine.Name = "picbxSplitLine";
            this.picbxSplitLine.Size = new System.Drawing.Size(22, 98);
            this.picbxSplitLine.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picbxSplitLine.TabIndex = 35;
            this.picbxSplitLine.TabStop = false;
            // 
            // btnNewDraw
            // 
            this.btnNewDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewDraw.FlatAppearance.BorderSize = 0;
            this.btnNewDraw.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewDraw.Image = global::MPPPenAutoTuning.Properties.Resources.play;
            this.btnNewDraw.Location = new System.Drawing.Point(662, 56);
            this.btnNewDraw.Name = "btnNewDraw";
            this.btnNewDraw.Size = new System.Drawing.Size(110, 38);
            this.btnNewDraw.TabIndex = 24;
            this.btnNewDraw.Text = "Start Draw";
            this.btnNewDraw.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNewDraw.UseVisualStyleBackColor = true;
            this.btnNewDraw.Click += new System.EventHandler(this.btnNewDraw_Click);
            this.btnNewDraw.MouseEnter += new System.EventHandler(this.btnNewDraw_MouseEnter);
            this.btnNewDraw.MouseLeave += new System.EventHandler(this.btnNewDraw_MouseLeave);
            // 
            // btnNewStop
            // 
            this.btnNewStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewStop.Enabled = false;
            this.btnNewStop.FlatAppearance.BorderSize = 0;
            this.btnNewStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewStop.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewStop.ForeColor = System.Drawing.Color.Red;
            this.btnNewStop.Image = ((System.Drawing.Image)(resources.GetObject("btnNewStop.Image")));
            this.btnNewStop.Location = new System.Drawing.Point(540, 12);
            this.btnNewStop.Name = "btnNewStop";
            this.btnNewStop.Size = new System.Drawing.Size(88, 82);
            this.btnNewStop.TabIndex = 34;
            this.btnNewStop.Text = "Stop";
            this.btnNewStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNewStop.UseVisualStyleBackColor = true;
            this.btnNewStop.Click += new System.EventHandler(this.btnNewStop_Click);
            this.btnNewStop.MouseEnter += new System.EventHandler(this.btnNewStop_MouseEnter);
            this.btnNewStop.MouseLeave += new System.EventHandler(this.btnNewStop_MouseLeave);
            this.btnNewStop.MouseHover += new System.EventHandler(this.btnNewStop_MouseHover);
            this.btnNewStop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnNewStop_MouseMove);
            // 
            // btnNewPattern
            // 
            this.btnNewPattern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewPattern.FlatAppearance.BorderSize = 0;
            this.btnNewPattern.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewPattern.ForeColor = System.Drawing.Color.Blue;
            this.btnNewPattern.Image = ((System.Drawing.Image)(resources.GetObject("btnNewPattern.Image")));
            this.btnNewPattern.Location = new System.Drawing.Point(662, 11);
            this.btnNewPattern.Name = "btnNewPattern";
            this.btnNewPattern.Size = new System.Drawing.Size(110, 38);
            this.btnNewPattern.TabIndex = 23;
            this.btnNewPattern.Text = "  Pattern";
            this.btnNewPattern.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNewPattern.UseVisualStyleBackColor = true;
            this.btnNewPattern.Click += new System.EventHandler(this.btnNewPattern_Click);
            this.btnNewPattern.MouseEnter += new System.EventHandler(this.btnNewPattern_MouseEnter);
            this.btnNewPattern.MouseLeave += new System.EventHandler(this.btnNewPattern_MouseLeave);
            // 
            // btnNewStart
            // 
            this.btnNewStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewStart.FlatAppearance.BorderSize = 0;
            this.btnNewStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewStart.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewStart.ForeColor = System.Drawing.Color.Navy;
            this.btnNewStart.Image = ((System.Drawing.Image)(resources.GetObject("btnNewStart.Image")));
            this.btnNewStart.Location = new System.Drawing.Point(446, 12);
            this.btnNewStart.Name = "btnNewStart";
            this.btnNewStart.Size = new System.Drawing.Size(88, 82);
            this.btnNewStart.TabIndex = 33;
            this.btnNewStart.Text = "Start";
            this.btnNewStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNewStart.UseVisualStyleBackColor = true;
            this.btnNewStart.Click += new System.EventHandler(this.btnNewStart_Click);
            this.btnNewStart.MouseEnter += new System.EventHandler(this.btnNewStart_MouseEnter);
            this.btnNewStart.MouseLeave += new System.EventHandler(this.btnNewStart_MouseLeave);
            this.btnNewStart.MouseHover += new System.EventHandler(this.btnNewStart_MouseHover);
            this.btnNewStart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnNewStart_MouseMove);
            // 
            // btnNewConnect
            // 
            this.btnNewConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewConnect.BackColor = System.Drawing.SystemColors.Control;
            this.btnNewConnect.FlatAppearance.BorderSize = 0;
            this.btnNewConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewConnect.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewConnect.ForeColor = System.Drawing.Color.Green;
            this.btnNewConnect.Image = ((System.Drawing.Image)(resources.GetObject("btnNewConnect.Image")));
            this.btnNewConnect.Location = new System.Drawing.Point(352, 12);
            this.btnNewConnect.Name = "btnNewConnect";
            this.btnNewConnect.Size = new System.Drawing.Size(88, 82);
            this.btnNewConnect.TabIndex = 32;
            this.btnNewConnect.Text = "Connect";
            this.btnNewConnect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNewConnect.UseVisualStyleBackColor = false;
            this.btnNewConnect.Click += new System.EventHandler(this.btnNewConnect_Click);
            this.btnNewConnect.MouseEnter += new System.EventHandler(this.btnNewConnect_MouseEnter);
            this.btnNewConnect.MouseLeave += new System.EventHandler(this.btnNewConnect_MouseLeave);
            this.btnNewConnect.MouseHover += new System.EventHandler(this.btnNewConnect_MouseHover);
            this.btnNewConnect.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnNewConnect_MouseMove);
            // 
            // cbxModeState
            // 
            this.cbxModeState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxModeState.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxModeState.FormattingEnabled = true;
            this.cbxModeState.Items.AddRange(new object[] {
            "Server",
            "Client",
            "GoDraw",
            "Single",
            "LoadData"});
            this.cbxModeState.Location = new System.Drawing.Point(271, 10);
            this.cbxModeState.Name = "cbxModeState";
            this.cbxModeState.Size = new System.Drawing.Size(129, 28);
            this.cbxModeState.TabIndex = 31;
            this.cbxModeState.SelectedIndexChanged += new System.EventHandler(this.cbxModeState_SelectedIndexChanged);
            // 
            // lblModeState
            // 
            this.lblModeState.AutoSize = true;
            this.lblModeState.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModeState.Location = new System.Drawing.Point(202, 13);
            this.lblModeState.Name = "lblModeState";
            this.lblModeState.Size = new System.Drawing.Size(63, 21);
            this.lblModeState.TabIndex = 30;
            this.lblModeState.Text = "Mode :";
            // 
            // pnlResultMessage
            // 
            this.pnlResultMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlResultMessage.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pnlResultMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlResultMessage.Controls.Add(this.lblErrorMessage);
            this.pnlResultMessage.Controls.Add(this.lblStatus);
            this.pnlResultMessage.Location = new System.Drawing.Point(2, 100);
            this.pnlResultMessage.Name = "pnlResultMessage";
            this.pnlResultMessage.Size = new System.Drawing.Size(771, 91);
            this.pnlResultMessage.TabIndex = 25;
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblErrorMessage.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblErrorMessage.Location = new System.Drawing.Point(0, 50);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new System.Drawing.Size(770, 37);
            this.lblErrorMessage.TabIndex = 1;
            this.lblErrorMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Font = new System.Drawing.Font("Times New Roman", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(0, -1);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(770, 55);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlMode
            // 
            this.pnlMode.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pnlMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMode.Controls.Add(this.lblMode);
            this.pnlMode.Location = new System.Drawing.Point(3, 1);
            this.pnlMode.Name = "pnlMode";
            this.pnlMode.Size = new System.Drawing.Size(193, 46);
            this.pnlMode.TabIndex = 24;
            // 
            // lblMode
            // 
            this.lblMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblMode.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMode.Location = new System.Drawing.Point(1, 4);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(187, 36);
            this.lblMode.TabIndex = 0;
            this.lblMode.Text = "Mode";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitcontainerOutput
            // 
            this.splitcontainerOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitcontainerOutput.Location = new System.Drawing.Point(1, 221);
            this.splitcontainerOutput.Name = "splitcontainerOutput";
            // 
            // splitcontainerOutput.Panel1
            // 
            this.splitcontainerOutput.Panel1.Controls.Add(this.gbxMessage);
            this.splitcontainerOutput.Panel1.Controls.Add(this.btnChart);
            // 
            // splitcontainerOutput.Panel2
            // 
            this.splitcontainerOutput.Panel2.Controls.Add(this.tcResult);
            this.splitcontainerOutput.Size = new System.Drawing.Size(917, 520);
            this.splitcontainerOutput.SplitterDistance = 199;
            this.splitcontainerOutput.TabIndex = 23;
            // 
            // gbxMessage
            // 
            this.gbxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxMessage.Controls.Add(this.rtbxMessage);
            this.gbxMessage.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxMessage.Location = new System.Drawing.Point(4, 1);
            this.gbxMessage.Name = "gbxMessage";
            this.gbxMessage.Size = new System.Drawing.Size(195, 484);
            this.gbxMessage.TabIndex = 25;
            this.gbxMessage.TabStop = false;
            this.gbxMessage.Text = "Message";
            // 
            // rtbxMessage
            // 
            this.rtbxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbxMessage.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxMessage.Location = new System.Drawing.Point(5, 22);
            this.rtbxMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtbxMessage.Name = "rtbxMessage";
            this.rtbxMessage.ReadOnly = true;
            this.rtbxMessage.Size = new System.Drawing.Size(184, 455);
            this.rtbxMessage.TabIndex = 24;
            this.rtbxMessage.Text = "";
            this.rtbxMessage.TextChanged += new System.EventHandler(this.rtbxBoxMessage_TextChanged);
            // 
            // btnChart
            // 
            this.btnChart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChart.Enabled = false;
            this.btnChart.FlatAppearance.BorderSize = 0;
            this.btnChart.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChart.ForeColor = System.Drawing.Color.Blue;
            this.btnChart.Image = ((System.Drawing.Image)(resources.GetObject("btnChart.Image")));
            this.btnChart.Location = new System.Drawing.Point(118, 484);
            this.btnChart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnChart.Name = "btnChart";
            this.btnChart.Size = new System.Drawing.Size(80, 35);
            this.btnChart.TabIndex = 24;
            this.btnChart.Text = "Chart";
            this.btnChart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnChart.UseVisualStyleBackColor = true;
            this.btnChart.Click += new System.EventHandler(this.btnChart_Click);
            this.btnChart.MouseEnter += new System.EventHandler(this.btnChart_MouseEnter);
            this.btnChart.MouseLeave += new System.EventHandler(this.btnChart_MouseLeave);
            // 
            // tcResult
            // 
            this.tcResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcResult.Controls.Add(this.tpgNoiseResult);
            this.tcResult.Controls.Add(this.tpgTNResult);
            this.tcResult.Controls.Add(this.tpgDGTResult);
            this.tcResult.Controls.Add(this.tpgTPGTResult);
            this.tcResult.Controls.Add(this.tpgPCTResult);
            this.tcResult.Controls.Add(this.tpgDTResult);
            this.tcResult.Controls.Add(this.tpgTTResult);
            this.tcResult.Controls.Add(this.tpgPTResult);
            this.tcResult.Controls.Add(this.tpgLTResult);
            this.tcResult.Controls.Add(this.tpgCostTime);
            this.tcResult.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcResult.Location = new System.Drawing.Point(0, 0);
            this.tcResult.Name = "tcResult";
            this.tcResult.SelectedIndex = 0;
            this.tcResult.Size = new System.Drawing.Size(714, 521);
            this.tcResult.TabIndex = 25;
            this.tcResult.SelectedIndexChanged += new System.EventHandler(this.tcResult_SelectedIndexChanged);
            // 
            // tpgNoiseResult
            // 
            this.tpgNoiseResult.Controls.Add(this.gbxNoiseResult);
            this.tpgNoiseResult.Location = new System.Drawing.Point(4, 26);
            this.tpgNoiseResult.Name = "tpgNoiseResult";
            this.tpgNoiseResult.Size = new System.Drawing.Size(706, 491);
            this.tpgNoiseResult.TabIndex = 0;
            this.tpgNoiseResult.Text = "Noise";
            this.tpgNoiseResult.UseVisualStyleBackColor = true;
            // 
            // gbxNoiseResult
            // 
            this.gbxNoiseResult.Controls.Add(this.dgvNoiseRank);
            this.gbxNoiseResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxNoiseResult.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxNoiseResult.Location = new System.Drawing.Point(0, 0);
            this.gbxNoiseResult.Name = "gbxNoiseResult";
            this.gbxNoiseResult.Size = new System.Drawing.Size(706, 491);
            this.gbxNoiseResult.TabIndex = 24;
            this.gbxNoiseResult.TabStop = false;
            this.gbxNoiseResult.Text = "Result";
            // 
            // dgvNoiseRank
            // 
            this.dgvNoiseRank.AllowUserToAddRows = false;
            this.dgvNoiseRank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvNoiseRank.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvNoiseRank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNoiseRank.Location = new System.Drawing.Point(4, 20);
            this.dgvNoiseRank.Name = "dgvNoiseRank";
            this.dgvNoiseRank.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvNoiseRank.RowTemplate.Height = 24;
            this.dgvNoiseRank.Size = new System.Drawing.Size(696, 465);
            this.dgvNoiseRank.TabIndex = 23;
            // 
            // tpgTNResult
            // 
            this.tpgTNResult.Controls.Add(this.gbxTNResult);
            this.tpgTNResult.Location = new System.Drawing.Point(4, 26);
            this.tpgTNResult.Name = "tpgTNResult";
            this.tpgTNResult.Size = new System.Drawing.Size(706, 491);
            this.tpgTNResult.TabIndex = 2;
            this.tpgTNResult.Text = "Tilt Noise";
            this.tpgTNResult.UseVisualStyleBackColor = true;
            // 
            // gbxTNResult
            // 
            this.gbxTNResult.Controls.Add(this.tcTNResult);
            this.gbxTNResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxTNResult.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxTNResult.Location = new System.Drawing.Point(0, 0);
            this.gbxTNResult.Name = "gbxTNResult";
            this.gbxTNResult.Size = new System.Drawing.Size(706, 491);
            this.gbxTNResult.TabIndex = 26;
            this.gbxTNResult.TabStop = false;
            this.gbxTNResult.Text = "Result";
            // 
            // tcTNResult
            // 
            this.tcTNResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcTNResult.Controls.Add(this.tpgTNPTHF);
            this.tcTNResult.Controls.Add(this.tpgTNPTHFRank);
            this.tcTNResult.Controls.Add(this.tpgTNBHF);
            this.tcTNResult.Controls.Add(this.tpgTNBHFRank);
            this.tcTNResult.Controls.Add(this.tpgTNPreliminaryList);
            this.tcTNResult.Controls.Add(this.tpgTNTotalRank);
            this.tcTNResult.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcTNResult.Location = new System.Drawing.Point(3, 18);
            this.tcTNResult.Name = "tcTNResult";
            this.tcTNResult.SelectedIndex = 0;
            this.tcTNResult.Size = new System.Drawing.Size(700, 470);
            this.tcTNResult.TabIndex = 1;
            this.tcTNResult.SelectedIndexChanged += new System.EventHandler(this.tcTNResult_SelectedIndexChanged);
            // 
            // tpgTNPTHF
            // 
            this.tpgTNPTHF.Controls.Add(this.rtbxTNPTHFResultList);
            this.tpgTNPTHF.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tpgTNPTHF.Location = new System.Drawing.Point(4, 26);
            this.tpgTNPTHF.Name = "tpgTNPTHF";
            this.tpgTNPTHF.Size = new System.Drawing.Size(692, 440);
            this.tpgTNPTHF.TabIndex = 0;
            this.tpgTNPTHF.Text = "PTHF";
            this.tpgTNPTHF.UseVisualStyleBackColor = true;
            // 
            // rtbxTNPTHFResultList
            // 
            this.rtbxTNPTHFResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTNPTHFResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTNPTHFResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTNPTHFResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxTNPTHFResultList.Name = "rtbxTNPTHFResultList";
            this.rtbxTNPTHFResultList.ReadOnly = true;
            this.rtbxTNPTHFResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxTNPTHFResultList.TabIndex = 27;
            this.rtbxTNPTHFResultList.Text = "";
            // 
            // tpgTNPTHFRank
            // 
            this.tpgTNPTHFRank.Controls.Add(this.dgvTNPTHFRank);
            this.tpgTNPTHFRank.Location = new System.Drawing.Point(4, 26);
            this.tpgTNPTHFRank.Name = "tpgTNPTHFRank";
            this.tpgTNPTHFRank.Size = new System.Drawing.Size(692, 440);
            this.tpgTNPTHFRank.TabIndex = 3;
            this.tpgTNPTHFRank.Text = "PTHF_Rank";
            this.tpgTNPTHFRank.UseVisualStyleBackColor = true;
            // 
            // dgvTNPTHFRank
            // 
            this.dgvTNPTHFRank.AllowUserToAddRows = false;
            this.dgvTNPTHFRank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTNPTHFRank.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvTNPTHFRank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTNPTHFRank.Location = new System.Drawing.Point(0, 0);
            this.dgvTNPTHFRank.Name = "dgvTNPTHFRank";
            this.dgvTNPTHFRank.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvTNPTHFRank.RowTemplate.Height = 24;
            this.dgvTNPTHFRank.Size = new System.Drawing.Size(692, 440);
            this.dgvTNPTHFRank.TabIndex = 24;
            // 
            // tpgTNBHF
            // 
            this.tpgTNBHF.Controls.Add(this.rtbxTNBHFResultList);
            this.tpgTNBHF.Location = new System.Drawing.Point(4, 26);
            this.tpgTNBHF.Name = "tpgTNBHF";
            this.tpgTNBHF.Size = new System.Drawing.Size(692, 440);
            this.tpgTNBHF.TabIndex = 1;
            this.tpgTNBHF.Text = "BHF";
            this.tpgTNBHF.UseVisualStyleBackColor = true;
            // 
            // rtbxTNBHFResultList
            // 
            this.rtbxTNBHFResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTNBHFResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTNBHFResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTNBHFResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxTNBHFResultList.Name = "rtbxTNBHFResultList";
            this.rtbxTNBHFResultList.ReadOnly = true;
            this.rtbxTNBHFResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxTNBHFResultList.TabIndex = 28;
            this.rtbxTNBHFResultList.Text = "";
            // 
            // tpgTNBHFRank
            // 
            this.tpgTNBHFRank.Controls.Add(this.dgvTNBHFRank);
            this.tpgTNBHFRank.Location = new System.Drawing.Point(4, 26);
            this.tpgTNBHFRank.Name = "tpgTNBHFRank";
            this.tpgTNBHFRank.Size = new System.Drawing.Size(692, 440);
            this.tpgTNBHFRank.TabIndex = 4;
            this.tpgTNBHFRank.Text = "BHF_Rank";
            this.tpgTNBHFRank.UseVisualStyleBackColor = true;
            // 
            // dgvTNBHFRank
            // 
            this.dgvTNBHFRank.AllowUserToAddRows = false;
            this.dgvTNBHFRank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTNBHFRank.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvTNBHFRank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTNBHFRank.Location = new System.Drawing.Point(0, 0);
            this.dgvTNBHFRank.Name = "dgvTNBHFRank";
            this.dgvTNBHFRank.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvTNBHFRank.RowTemplate.Height = 24;
            this.dgvTNBHFRank.Size = new System.Drawing.Size(692, 440);
            this.dgvTNBHFRank.TabIndex = 25;
            // 
            // tpgTNPreliminaryList
            // 
            this.tpgTNPreliminaryList.Controls.Add(this.rtbxTNPreliminaryList);
            this.tpgTNPreliminaryList.Location = new System.Drawing.Point(4, 26);
            this.tpgTNPreliminaryList.Name = "tpgTNPreliminaryList";
            this.tpgTNPreliminaryList.Size = new System.Drawing.Size(692, 440);
            this.tpgTNPreliminaryList.TabIndex = 2;
            this.tpgTNPreliminaryList.Text = "Preliminary List";
            this.tpgTNPreliminaryList.UseVisualStyleBackColor = true;
            // 
            // rtbxTNPreliminaryList
            // 
            this.rtbxTNPreliminaryList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTNPreliminaryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTNPreliminaryList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTNPreliminaryList.Location = new System.Drawing.Point(0, 0);
            this.rtbxTNPreliminaryList.Name = "rtbxTNPreliminaryList";
            this.rtbxTNPreliminaryList.ReadOnly = true;
            this.rtbxTNPreliminaryList.Size = new System.Drawing.Size(692, 440);
            this.rtbxTNPreliminaryList.TabIndex = 29;
            this.rtbxTNPreliminaryList.Text = "";
            // 
            // tpgTNTotalRank
            // 
            this.tpgTNTotalRank.Controls.Add(this.dgvTNTotalRank);
            this.tpgTNTotalRank.Location = new System.Drawing.Point(4, 26);
            this.tpgTNTotalRank.Name = "tpgTNTotalRank";
            this.tpgTNTotalRank.Size = new System.Drawing.Size(692, 440);
            this.tpgTNTotalRank.TabIndex = 5;
            this.tpgTNTotalRank.Text = "Total_Rank";
            this.tpgTNTotalRank.UseVisualStyleBackColor = true;
            // 
            // dgvTNTotalRank
            // 
            this.dgvTNTotalRank.AllowUserToAddRows = false;
            this.dgvTNTotalRank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTNTotalRank.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvTNTotalRank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTNTotalRank.Location = new System.Drawing.Point(0, 0);
            this.dgvTNTotalRank.Name = "dgvTNTotalRank";
            this.dgvTNTotalRank.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvTNTotalRank.RowTemplate.Height = 24;
            this.dgvTNTotalRank.Size = new System.Drawing.Size(692, 440);
            this.dgvTNTotalRank.TabIndex = 26;
            // 
            // tpgDGTResult
            // 
            this.tpgDGTResult.Controls.Add(this.gbxDGTResult);
            this.tpgDGTResult.Location = new System.Drawing.Point(4, 26);
            this.tpgDGTResult.Name = "tpgDGTResult";
            this.tpgDGTResult.Size = new System.Drawing.Size(706, 491);
            this.tpgDGTResult.TabIndex = 8;
            this.tpgDGTResult.Text = "DigiGain Tuning";
            this.tpgDGTResult.UseVisualStyleBackColor = true;
            // 
            // gbxDGTResult
            // 
            this.gbxDGTResult.Controls.Add(this.tcDGTResult);
            this.gbxDGTResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxDGTResult.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxDGTResult.Location = new System.Drawing.Point(0, 0);
            this.gbxDGTResult.Name = "gbxDGTResult";
            this.gbxDGTResult.Size = new System.Drawing.Size(706, 491);
            this.gbxDGTResult.TabIndex = 28;
            this.gbxDGTResult.TabStop = false;
            this.gbxDGTResult.Text = "Result";
            // 
            // tcDGTResult
            // 
            this.tcDGTResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcDGTResult.Controls.Add(this.tpgDGTTotalList);
            this.tcDGTResult.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcDGTResult.Location = new System.Drawing.Point(4, 18);
            this.tcDGTResult.Name = "tcDGTResult";
            this.tcDGTResult.SelectedIndex = 0;
            this.tcDGTResult.Size = new System.Drawing.Size(700, 470);
            this.tcDGTResult.TabIndex = 1;
            // 
            // tpgDGTTotalList
            // 
            this.tpgDGTTotalList.Controls.Add(this.rtbxDGTTotalList);
            this.tpgDGTTotalList.Location = new System.Drawing.Point(4, 26);
            this.tpgDGTTotalList.Name = "tpgDGTTotalList";
            this.tpgDGTTotalList.Size = new System.Drawing.Size(692, 440);
            this.tpgDGTTotalList.TabIndex = 2;
            this.tpgDGTTotalList.Text = "Total List";
            this.tpgDGTTotalList.UseVisualStyleBackColor = true;
            // 
            // rtbxDGTTotalList
            // 
            this.rtbxDGTTotalList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxDGTTotalList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxDGTTotalList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxDGTTotalList.Location = new System.Drawing.Point(0, 0);
            this.rtbxDGTTotalList.Name = "rtbxDGTTotalList";
            this.rtbxDGTTotalList.ReadOnly = true;
            this.rtbxDGTTotalList.Size = new System.Drawing.Size(692, 440);
            this.rtbxDGTTotalList.TabIndex = 26;
            this.rtbxDGTTotalList.Text = "";
            // 
            // tpgTPGTResult
            // 
            this.tpgTPGTResult.Controls.Add(this.gbxTPGTResult);
            this.tpgTPGTResult.Location = new System.Drawing.Point(4, 26);
            this.tpgTPGTResult.Name = "tpgTPGTResult";
            this.tpgTPGTResult.Size = new System.Drawing.Size(706, 491);
            this.tpgTPGTResult.TabIndex = 9;
            this.tpgTPGTResult.Text = "TP_Gain Tuning";
            this.tpgTPGTResult.UseVisualStyleBackColor = true;
            // 
            // gbxTPGTResult
            // 
            this.gbxTPGTResult.Controls.Add(this.tcTPGTResult);
            this.gbxTPGTResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxTPGTResult.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxTPGTResult.Location = new System.Drawing.Point(0, 0);
            this.gbxTPGTResult.Name = "gbxTPGTResult";
            this.gbxTPGTResult.Size = new System.Drawing.Size(706, 491);
            this.gbxTPGTResult.TabIndex = 29;
            this.gbxTPGTResult.TabStop = false;
            this.gbxTPGTResult.Text = "Result";
            // 
            // tcTPGTResult
            // 
            this.tcTPGTResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcTPGTResult.Controls.Add(this.tpgTPGTTotalList);
            this.tcTPGTResult.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcTPGTResult.Location = new System.Drawing.Point(4, 18);
            this.tcTPGTResult.Name = "tcTPGTResult";
            this.tcTPGTResult.SelectedIndex = 0;
            this.tcTPGTResult.Size = new System.Drawing.Size(700, 470);
            this.tcTPGTResult.TabIndex = 1;
            // 
            // tpgTPGTTotalList
            // 
            this.tpgTPGTTotalList.Controls.Add(this.rtbxTPGTTotalList);
            this.tpgTPGTTotalList.Location = new System.Drawing.Point(4, 26);
            this.tpgTPGTTotalList.Name = "tpgTPGTTotalList";
            this.tpgTPGTTotalList.Size = new System.Drawing.Size(692, 440);
            this.tpgTPGTTotalList.TabIndex = 2;
            this.tpgTPGTTotalList.Text = "Total List";
            this.tpgTPGTTotalList.UseVisualStyleBackColor = true;
            // 
            // rtbxTPGTTotalList
            // 
            this.rtbxTPGTTotalList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTPGTTotalList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTPGTTotalList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTPGTTotalList.Location = new System.Drawing.Point(0, 0);
            this.rtbxTPGTTotalList.Name = "rtbxTPGTTotalList";
            this.rtbxTPGTTotalList.ReadOnly = true;
            this.rtbxTPGTTotalList.Size = new System.Drawing.Size(692, 440);
            this.rtbxTPGTTotalList.TabIndex = 26;
            this.rtbxTPGTTotalList.Text = "";
            // 
            // tpgPCTResult
            // 
            this.tpgPCTResult.Controls.Add(this.gbxPCTResult);
            this.tpgPCTResult.Location = new System.Drawing.Point(4, 26);
            this.tpgPCTResult.Name = "tpgPCTResult";
            this.tpgPCTResult.Size = new System.Drawing.Size(706, 491);
            this.tpgPCTResult.TabIndex = 7;
            this.tpgPCTResult.Text = "PeakCheck Tuning";
            this.tpgPCTResult.UseVisualStyleBackColor = true;
            // 
            // gbxPCTResult
            // 
            this.gbxPCTResult.Controls.Add(this.tcPCTResult);
            this.gbxPCTResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxPCTResult.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxPCTResult.Location = new System.Drawing.Point(0, 0);
            this.gbxPCTResult.Name = "gbxPCTResult";
            this.gbxPCTResult.Size = new System.Drawing.Size(706, 491);
            this.gbxPCTResult.TabIndex = 27;
            this.gbxPCTResult.TabStop = false;
            this.gbxPCTResult.Text = "Result";
            // 
            // tcPCTResult
            // 
            this.tcPCTResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcPCTResult.Controls.Add(this.tpgPCHover_1st);
            this.tcPCTResult.Controls.Add(this.tpgPCHover_2nd);
            this.tcPCTResult.Controls.Add(this.tpgPCContact);
            this.tcPCTResult.Controls.Add(this.tpgPCTotalList);
            this.tcPCTResult.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcPCTResult.Location = new System.Drawing.Point(4, 18);
            this.tcPCTResult.Name = "tcPCTResult";
            this.tcPCTResult.SelectedIndex = 0;
            this.tcPCTResult.Size = new System.Drawing.Size(700, 470);
            this.tcPCTResult.TabIndex = 1;
            // 
            // tpgPCHover_1st
            // 
            this.tpgPCHover_1st.Controls.Add(this.rtbxPCTHover_1stResultList);
            this.tpgPCHover_1st.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tpgPCHover_1st.Location = new System.Drawing.Point(4, 26);
            this.tpgPCHover_1st.Name = "tpgPCHover_1st";
            this.tpgPCHover_1st.Size = new System.Drawing.Size(692, 440);
            this.tpgPCHover_1st.TabIndex = 0;
            this.tpgPCHover_1st.Text = "Hover_1st";
            this.tpgPCHover_1st.UseVisualStyleBackColor = true;
            // 
            // rtbxPCTHover_1stResultList
            // 
            this.rtbxPCTHover_1stResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxPCTHover_1stResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxPCTHover_1stResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxPCTHover_1stResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxPCTHover_1stResultList.Name = "rtbxPCTHover_1stResultList";
            this.rtbxPCTHover_1stResultList.ReadOnly = true;
            this.rtbxPCTHover_1stResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxPCTHover_1stResultList.TabIndex = 28;
            this.rtbxPCTHover_1stResultList.Text = "";
            // 
            // tpgPCHover_2nd
            // 
            this.tpgPCHover_2nd.Controls.Add(this.rtbxPCTHover_2ndResultList);
            this.tpgPCHover_2nd.Location = new System.Drawing.Point(4, 26);
            this.tpgPCHover_2nd.Name = "tpgPCHover_2nd";
            this.tpgPCHover_2nd.Size = new System.Drawing.Size(692, 440);
            this.tpgPCHover_2nd.TabIndex = 1;
            this.tpgPCHover_2nd.Text = "Hover_2nd";
            this.tpgPCHover_2nd.UseVisualStyleBackColor = true;
            // 
            // rtbxPCTHover_2ndResultList
            // 
            this.rtbxPCTHover_2ndResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxPCTHover_2ndResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxPCTHover_2ndResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxPCTHover_2ndResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxPCTHover_2ndResultList.Name = "rtbxPCTHover_2ndResultList";
            this.rtbxPCTHover_2ndResultList.ReadOnly = true;
            this.rtbxPCTHover_2ndResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxPCTHover_2ndResultList.TabIndex = 29;
            this.rtbxPCTHover_2ndResultList.Text = "";
            // 
            // tpgPCContact
            // 
            this.tpgPCContact.Controls.Add(this.rtbxPCTContactResultList);
            this.tpgPCContact.Location = new System.Drawing.Point(4, 26);
            this.tpgPCContact.Name = "tpgPCContact";
            this.tpgPCContact.Size = new System.Drawing.Size(692, 440);
            this.tpgPCContact.TabIndex = 3;
            this.tpgPCContact.Text = "Contact";
            this.tpgPCContact.UseVisualStyleBackColor = true;
            // 
            // rtbxPCTContactResultList
            // 
            this.rtbxPCTContactResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxPCTContactResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxPCTContactResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxPCTContactResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxPCTContactResultList.Name = "rtbxPCTContactResultList";
            this.rtbxPCTContactResultList.ReadOnly = true;
            this.rtbxPCTContactResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxPCTContactResultList.TabIndex = 30;
            this.rtbxPCTContactResultList.Text = "";
            // 
            // tpgPCTotalList
            // 
            this.tpgPCTotalList.Controls.Add(this.rtbxPCTTotalList);
            this.tpgPCTotalList.Location = new System.Drawing.Point(4, 26);
            this.tpgPCTotalList.Name = "tpgPCTotalList";
            this.tpgPCTotalList.Size = new System.Drawing.Size(692, 440);
            this.tpgPCTotalList.TabIndex = 2;
            this.tpgPCTotalList.Text = "Total List";
            this.tpgPCTotalList.UseVisualStyleBackColor = true;
            // 
            // rtbxPCTTotalList
            // 
            this.rtbxPCTTotalList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxPCTTotalList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxPCTTotalList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxPCTTotalList.Location = new System.Drawing.Point(0, 0);
            this.rtbxPCTTotalList.Name = "rtbxPCTTotalList";
            this.rtbxPCTTotalList.ReadOnly = true;
            this.rtbxPCTTotalList.Size = new System.Drawing.Size(692, 440);
            this.rtbxPCTTotalList.TabIndex = 30;
            this.rtbxPCTTotalList.Text = "";
            // 
            // tpgDTResult
            // 
            this.tpgDTResult.Controls.Add(this.gbxDTResult);
            this.tpgDTResult.Location = new System.Drawing.Point(4, 26);
            this.tpgDTResult.Name = "tpgDTResult";
            this.tpgDTResult.Size = new System.Drawing.Size(706, 491);
            this.tpgDTResult.TabIndex = 1;
            this.tpgDTResult.Text = "Digital Tuning";
            this.tpgDTResult.UseVisualStyleBackColor = true;
            // 
            // gbxDTResult
            // 
            this.gbxDTResult.Controls.Add(this.tcDTResult);
            this.gbxDTResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxDTResult.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxDTResult.Location = new System.Drawing.Point(0, 0);
            this.gbxDTResult.Name = "gbxDTResult";
            this.gbxDTResult.Size = new System.Drawing.Size(706, 491);
            this.gbxDTResult.TabIndex = 27;
            this.gbxDTResult.TabStop = false;
            this.gbxDTResult.Text = "Result";
            // 
            // tcDTResult
            // 
            this.tcDTResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcDTResult.Controls.Add(this.tpgDTHover_1st);
            this.tcDTResult.Controls.Add(this.tpgDTHover_2nd);
            this.tcDTResult.Controls.Add(this.tpgDTContact);
            this.tcDTResult.Controls.Add(this.tpgDTPreliminaryList);
            this.tcDTResult.Controls.Add(this.tpgDTHoverTRxS);
            this.tcDTResult.Controls.Add(this.tpgDTContactTRxS);
            this.tcDTResult.Controls.Add(this.tpgDTTotalList);
            this.tcDTResult.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcDTResult.Location = new System.Drawing.Point(4, 18);
            this.tcDTResult.Name = "tcDTResult";
            this.tcDTResult.SelectedIndex = 0;
            this.tcDTResult.Size = new System.Drawing.Size(700, 470);
            this.tcDTResult.TabIndex = 1;
            // 
            // tpgDTHover_1st
            // 
            this.tpgDTHover_1st.Controls.Add(this.rtbxDTHover_1stResultList);
            this.tpgDTHover_1st.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tpgDTHover_1st.Location = new System.Drawing.Point(4, 26);
            this.tpgDTHover_1st.Name = "tpgDTHover_1st";
            this.tpgDTHover_1st.Size = new System.Drawing.Size(692, 440);
            this.tpgDTHover_1st.TabIndex = 0;
            this.tpgDTHover_1st.Text = "Hover_1st";
            this.tpgDTHover_1st.UseVisualStyleBackColor = true;
            // 
            // rtbxDTHover_1stResultList
            // 
            this.rtbxDTHover_1stResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxDTHover_1stResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxDTHover_1stResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxDTHover_1stResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxDTHover_1stResultList.Name = "rtbxDTHover_1stResultList";
            this.rtbxDTHover_1stResultList.ReadOnly = true;
            this.rtbxDTHover_1stResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxDTHover_1stResultList.TabIndex = 31;
            this.rtbxDTHover_1stResultList.Text = "";
            // 
            // tpgDTHover_2nd
            // 
            this.tpgDTHover_2nd.Controls.Add(this.rtbxDTHover_2ndResultList);
            this.tpgDTHover_2nd.Location = new System.Drawing.Point(4, 26);
            this.tpgDTHover_2nd.Name = "tpgDTHover_2nd";
            this.tpgDTHover_2nd.Size = new System.Drawing.Size(692, 440);
            this.tpgDTHover_2nd.TabIndex = 1;
            this.tpgDTHover_2nd.Text = "Hover_2nd";
            this.tpgDTHover_2nd.UseVisualStyleBackColor = true;
            // 
            // rtbxDTHover_2ndResultList
            // 
            this.rtbxDTHover_2ndResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxDTHover_2ndResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxDTHover_2ndResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxDTHover_2ndResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxDTHover_2ndResultList.Name = "rtbxDTHover_2ndResultList";
            this.rtbxDTHover_2ndResultList.ReadOnly = true;
            this.rtbxDTHover_2ndResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxDTHover_2ndResultList.TabIndex = 32;
            this.rtbxDTHover_2ndResultList.Text = "";
            // 
            // tpgDTContact
            // 
            this.tpgDTContact.Controls.Add(this.rtbxDTContactResultList);
            this.tpgDTContact.Location = new System.Drawing.Point(4, 26);
            this.tpgDTContact.Name = "tpgDTContact";
            this.tpgDTContact.Size = new System.Drawing.Size(692, 440);
            this.tpgDTContact.TabIndex = 3;
            this.tpgDTContact.Text = "Contact";
            this.tpgDTContact.UseVisualStyleBackColor = true;
            // 
            // rtbxDTContactResultList
            // 
            this.rtbxDTContactResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxDTContactResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxDTContactResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxDTContactResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxDTContactResultList.Name = "rtbxDTContactResultList";
            this.rtbxDTContactResultList.ReadOnly = true;
            this.rtbxDTContactResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxDTContactResultList.TabIndex = 33;
            this.rtbxDTContactResultList.Text = "";
            // 
            // tpgDTPreliminaryList
            // 
            this.tpgDTPreliminaryList.Controls.Add(this.rtbxDTPreliminaryList);
            this.tpgDTPreliminaryList.Location = new System.Drawing.Point(4, 26);
            this.tpgDTPreliminaryList.Name = "tpgDTPreliminaryList";
            this.tpgDTPreliminaryList.Size = new System.Drawing.Size(692, 440);
            this.tpgDTPreliminaryList.TabIndex = 2;
            this.tpgDTPreliminaryList.Text = "Preliminary List";
            this.tpgDTPreliminaryList.UseVisualStyleBackColor = true;
            // 
            // rtbxDTPreliminaryList
            // 
            this.rtbxDTPreliminaryList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxDTPreliminaryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxDTPreliminaryList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxDTPreliminaryList.Location = new System.Drawing.Point(0, 0);
            this.rtbxDTPreliminaryList.Name = "rtbxDTPreliminaryList";
            this.rtbxDTPreliminaryList.ReadOnly = true;
            this.rtbxDTPreliminaryList.Size = new System.Drawing.Size(692, 440);
            this.rtbxDTPreliminaryList.TabIndex = 33;
            this.rtbxDTPreliminaryList.Text = "";
            // 
            // tpgDTHoverTRxS
            // 
            this.tpgDTHoverTRxS.Controls.Add(this.rtbxDTHoverTRxSResultList);
            this.tpgDTHoverTRxS.Location = new System.Drawing.Point(4, 26);
            this.tpgDTHoverTRxS.Name = "tpgDTHoverTRxS";
            this.tpgDTHoverTRxS.Size = new System.Drawing.Size(692, 440);
            this.tpgDTHoverTRxS.TabIndex = 4;
            this.tpgDTHoverTRxS.Text = "HoverTRxS";
            this.tpgDTHoverTRxS.UseVisualStyleBackColor = true;
            // 
            // rtbxDTHoverTRxSResultList
            // 
            this.rtbxDTHoverTRxSResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxDTHoverTRxSResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxDTHoverTRxSResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxDTHoverTRxSResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxDTHoverTRxSResultList.Name = "rtbxDTHoverTRxSResultList";
            this.rtbxDTHoverTRxSResultList.ReadOnly = true;
            this.rtbxDTHoverTRxSResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxDTHoverTRxSResultList.TabIndex = 33;
            this.rtbxDTHoverTRxSResultList.Text = "";
            // 
            // tpgDTContactTRxS
            // 
            this.tpgDTContactTRxS.Controls.Add(this.rtbxDTContactTRxSResultList);
            this.tpgDTContactTRxS.Location = new System.Drawing.Point(4, 26);
            this.tpgDTContactTRxS.Name = "tpgDTContactTRxS";
            this.tpgDTContactTRxS.Size = new System.Drawing.Size(692, 440);
            this.tpgDTContactTRxS.TabIndex = 5;
            this.tpgDTContactTRxS.Text = "ContactTRxS";
            this.tpgDTContactTRxS.UseVisualStyleBackColor = true;
            // 
            // rtbxDTContactTRxSResultList
            // 
            this.rtbxDTContactTRxSResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxDTContactTRxSResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxDTContactTRxSResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxDTContactTRxSResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxDTContactTRxSResultList.Name = "rtbxDTContactTRxSResultList";
            this.rtbxDTContactTRxSResultList.ReadOnly = true;
            this.rtbxDTContactTRxSResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxDTContactTRxSResultList.TabIndex = 34;
            this.rtbxDTContactTRxSResultList.Text = "";
            // 
            // tpgDTTotalList
            // 
            this.tpgDTTotalList.Controls.Add(this.rtbxDTTotalList);
            this.tpgDTTotalList.Location = new System.Drawing.Point(4, 26);
            this.tpgDTTotalList.Name = "tpgDTTotalList";
            this.tpgDTTotalList.Size = new System.Drawing.Size(692, 440);
            this.tpgDTTotalList.TabIndex = 6;
            this.tpgDTTotalList.Text = "Total List";
            this.tpgDTTotalList.UseVisualStyleBackColor = true;
            // 
            // rtbxDTTotalList
            // 
            this.rtbxDTTotalList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxDTTotalList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxDTTotalList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxDTTotalList.Location = new System.Drawing.Point(0, 0);
            this.rtbxDTTotalList.Name = "rtbxDTTotalList";
            this.rtbxDTTotalList.ReadOnly = true;
            this.rtbxDTTotalList.Size = new System.Drawing.Size(692, 440);
            this.rtbxDTTotalList.TabIndex = 34;
            this.rtbxDTTotalList.Text = "";
            // 
            // tpgTTResult
            // 
            this.tpgTTResult.Controls.Add(this.gbxTTResult);
            this.tpgTTResult.Location = new System.Drawing.Point(4, 26);
            this.tpgTTResult.Name = "tpgTTResult";
            this.tpgTTResult.Size = new System.Drawing.Size(706, 491);
            this.tpgTTResult.TabIndex = 3;
            this.tpgTTResult.Text = "Tilt Tuning";
            this.tpgTTResult.UseVisualStyleBackColor = true;
            // 
            // gbxTTResult
            // 
            this.gbxTTResult.Controls.Add(this.tcTTResult);
            this.gbxTTResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxTTResult.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxTTResult.Location = new System.Drawing.Point(0, 0);
            this.gbxTTResult.Name = "gbxTTResult";
            this.gbxTTResult.Size = new System.Drawing.Size(706, 491);
            this.gbxTTResult.TabIndex = 25;
            this.gbxTTResult.TabStop = false;
            this.gbxTTResult.Text = "Result";
            // 
            // tcTTResult
            // 
            this.tcTTResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcTTResult.Controls.Add(this.tpgTTPTHF);
            this.tcTTResult.Controls.Add(this.tpgTTBHF);
            this.tcTTResult.Controls.Add(this.tpgTTRank);
            this.tcTTResult.Controls.Add(this.tpgTTTotalList);
            this.tcTTResult.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcTTResult.Location = new System.Drawing.Point(4, 18);
            this.tcTTResult.Name = "tcTTResult";
            this.tcTTResult.SelectedIndex = 0;
            this.tcTTResult.Size = new System.Drawing.Size(700, 470);
            this.tcTTResult.TabIndex = 2;
            this.tcTTResult.SelectedIndexChanged += new System.EventHandler(this.tcTTResult_SelectedIndexChanged);
            // 
            // tpgTTPTHF
            // 
            this.tpgTTPTHF.Controls.Add(this.rtbxTTPTHFResultList);
            this.tpgTTPTHF.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tpgTTPTHF.Location = new System.Drawing.Point(4, 26);
            this.tpgTTPTHF.Name = "tpgTTPTHF";
            this.tpgTTPTHF.Size = new System.Drawing.Size(692, 440);
            this.tpgTTPTHF.TabIndex = 0;
            this.tpgTTPTHF.Text = "PTHF";
            this.tpgTTPTHF.UseVisualStyleBackColor = true;
            // 
            // rtbxTTPTHFResultList
            // 
            this.rtbxTTPTHFResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTTPTHFResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTTPTHFResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTTPTHFResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxTTPTHFResultList.Name = "rtbxTTPTHFResultList";
            this.rtbxTTPTHFResultList.ReadOnly = true;
            this.rtbxTTPTHFResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxTTPTHFResultList.TabIndex = 32;
            this.rtbxTTPTHFResultList.Text = "";
            // 
            // tpgTTBHF
            // 
            this.tpgTTBHF.Controls.Add(this.rtbxTTBHFResultList);
            this.tpgTTBHF.Location = new System.Drawing.Point(4, 26);
            this.tpgTTBHF.Name = "tpgTTBHF";
            this.tpgTTBHF.Size = new System.Drawing.Size(692, 440);
            this.tpgTTBHF.TabIndex = 1;
            this.tpgTTBHF.Text = "BHF";
            this.tpgTTBHF.UseVisualStyleBackColor = true;
            // 
            // rtbxTTBHFResultList
            // 
            this.rtbxTTBHFResultList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTTBHFResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTTBHFResultList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTTBHFResultList.Location = new System.Drawing.Point(0, 0);
            this.rtbxTTBHFResultList.Name = "rtbxTTBHFResultList";
            this.rtbxTTBHFResultList.ReadOnly = true;
            this.rtbxTTBHFResultList.Size = new System.Drawing.Size(692, 440);
            this.rtbxTTBHFResultList.TabIndex = 33;
            this.rtbxTTBHFResultList.Text = "";
            // 
            // tpgTTRank
            // 
            this.tpgTTRank.Controls.Add(this.dgvTTRank);
            this.tpgTTRank.Location = new System.Drawing.Point(4, 26);
            this.tpgTTRank.Name = "tpgTTRank";
            this.tpgTTRank.Size = new System.Drawing.Size(692, 440);
            this.tpgTTRank.TabIndex = 2;
            this.tpgTTRank.Text = "Rank";
            this.tpgTTRank.UseVisualStyleBackColor = true;
            // 
            // dgvTTRank
            // 
            this.dgvTTRank.AllowUserToAddRows = false;
            this.dgvTTRank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTTRank.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvTTRank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTTRank.Location = new System.Drawing.Point(0, 0);
            this.dgvTTRank.Name = "dgvTTRank";
            this.dgvTTRank.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvTTRank.RowTemplate.Height = 24;
            this.dgvTTRank.Size = new System.Drawing.Size(692, 440);
            this.dgvTTRank.TabIndex = 23;
            // 
            // tpgTTTotalList
            // 
            this.tpgTTTotalList.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTTTotalList.Controls.Add(this.rtbxTTTotalList);
            this.tpgTTTotalList.Location = new System.Drawing.Point(4, 26);
            this.tpgTTTotalList.Name = "tpgTTTotalList";
            this.tpgTTTotalList.Size = new System.Drawing.Size(692, 440);
            this.tpgTTTotalList.TabIndex = 3;
            this.tpgTTTotalList.Text = "Total List";
            // 
            // rtbxTTTotalList
            // 
            this.rtbxTTTotalList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTTTotalList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTTTotalList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTTTotalList.Location = new System.Drawing.Point(0, 0);
            this.rtbxTTTotalList.Name = "rtbxTTTotalList";
            this.rtbxTTTotalList.ReadOnly = true;
            this.rtbxTTTotalList.Size = new System.Drawing.Size(692, 440);
            this.rtbxTTTotalList.TabIndex = 34;
            this.rtbxTTTotalList.Text = "";
            // 
            // tpgPTResult
            // 
            this.tpgPTResult.Controls.Add(this.gbxPTResult);
            this.tpgPTResult.Location = new System.Drawing.Point(4, 26);
            this.tpgPTResult.Name = "tpgPTResult";
            this.tpgPTResult.Size = new System.Drawing.Size(706, 491);
            this.tpgPTResult.TabIndex = 4;
            this.tpgPTResult.Text = "Pressure Tuning";
            this.tpgPTResult.UseVisualStyleBackColor = true;
            // 
            // gbxPTResult
            // 
            this.gbxPTResult.Controls.Add(this.tcPTResult);
            this.gbxPTResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxPTResult.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxPTResult.Location = new System.Drawing.Point(0, 0);
            this.gbxPTResult.Name = "gbxPTResult";
            this.gbxPTResult.Size = new System.Drawing.Size(706, 491);
            this.gbxPTResult.TabIndex = 26;
            this.gbxPTResult.TabStop = false;
            this.gbxPTResult.Text = "Result";
            // 
            // tcPTResult
            // 
            this.tcPTResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcPTResult.Controls.Add(this.tpgPTTotalList);
            this.tcPTResult.Controls.Add(this.tpgPTPressureTable);
            this.tcPTResult.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcPTResult.Location = new System.Drawing.Point(4, 18);
            this.tcPTResult.Name = "tcPTResult";
            this.tcPTResult.SelectedIndex = 0;
            this.tcPTResult.Size = new System.Drawing.Size(700, 470);
            this.tcPTResult.TabIndex = 1;
            // 
            // tpgPTTotalList
            // 
            this.tpgPTTotalList.Controls.Add(this.rtbxPTTotalList);
            this.tpgPTTotalList.Location = new System.Drawing.Point(4, 26);
            this.tpgPTTotalList.Name = "tpgPTTotalList";
            this.tpgPTTotalList.Size = new System.Drawing.Size(692, 440);
            this.tpgPTTotalList.TabIndex = 0;
            this.tpgPTTotalList.Text = "Total List";
            this.tpgPTTotalList.UseVisualStyleBackColor = true;
            // 
            // rtbxPTTotalList
            // 
            this.rtbxPTTotalList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxPTTotalList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxPTTotalList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxPTTotalList.Location = new System.Drawing.Point(0, 0);
            this.rtbxPTTotalList.Name = "rtbxPTTotalList";
            this.rtbxPTTotalList.ReadOnly = true;
            this.rtbxPTTotalList.Size = new System.Drawing.Size(692, 440);
            this.rtbxPTTotalList.TabIndex = 34;
            this.rtbxPTTotalList.Text = "";
            // 
            // tpgPTPressureTable
            // 
            this.tpgPTPressureTable.Controls.Add(this.rtbxPTPressureTable);
            this.tpgPTPressureTable.Location = new System.Drawing.Point(4, 26);
            this.tpgPTPressureTable.Name = "tpgPTPressureTable";
            this.tpgPTPressureTable.Size = new System.Drawing.Size(692, 440);
            this.tpgPTPressureTable.TabIndex = 1;
            this.tpgPTPressureTable.Text = "Pressure Table";
            this.tpgPTPressureTable.UseVisualStyleBackColor = true;
            // 
            // rtbxPTPressureTable
            // 
            this.rtbxPTPressureTable.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxPTPressureTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxPTPressureTable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxPTPressureTable.Location = new System.Drawing.Point(0, 0);
            this.rtbxPTPressureTable.Name = "rtbxPTPressureTable";
            this.rtbxPTPressureTable.Size = new System.Drawing.Size(692, 440);
            this.rtbxPTPressureTable.TabIndex = 0;
            this.rtbxPTPressureTable.Text = "";
            this.rtbxPTPressureTable.WordWrap = false;
            // 
            // tpgLTResult
            // 
            this.tpgLTResult.Controls.Add(this.gbxLTResult);
            this.tpgLTResult.Location = new System.Drawing.Point(4, 26);
            this.tpgLTResult.Name = "tpgLTResult";
            this.tpgLTResult.Size = new System.Drawing.Size(706, 491);
            this.tpgLTResult.TabIndex = 6;
            this.tpgLTResult.Text = "Linearity Tuning";
            this.tpgLTResult.UseVisualStyleBackColor = true;
            // 
            // gbxLTResult
            // 
            this.gbxLTResult.Controls.Add(this.tcLTResult);
            this.gbxLTResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxLTResult.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxLTResult.Location = new System.Drawing.Point(0, 0);
            this.gbxLTResult.Name = "gbxLTResult";
            this.gbxLTResult.Size = new System.Drawing.Size(706, 491);
            this.gbxLTResult.TabIndex = 27;
            this.gbxLTResult.TabStop = false;
            this.gbxLTResult.Text = "Result";
            // 
            // tcLTResult
            // 
            this.tcLTResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcLTResult.Controls.Add(this.tpgLTTotalList);
            this.tcLTResult.Controls.Add(this.tpgLTRX5TTable);
            this.tcLTResult.Controls.Add(this.tpgLTTX5TTable);
            this.tcLTResult.Controls.Add(this.tpgLTRX3TTable);
            this.tcLTResult.Controls.Add(this.tpgLTTX3TTable);
            this.tcLTResult.Controls.Add(this.tpgLTRX2TLETable);
            this.tcLTResult.Controls.Add(this.tpgLTTX2TLETable);
            this.tcLTResult.Controls.Add(this.tpgLTRX2THETable);
            this.tcLTResult.Controls.Add(this.tpgTX2THETable);
            this.tcLTResult.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcLTResult.Location = new System.Drawing.Point(4, 18);
            this.tcLTResult.Name = "tcLTResult";
            this.tcLTResult.SelectedIndex = 0;
            this.tcLTResult.Size = new System.Drawing.Size(700, 470);
            this.tcLTResult.TabIndex = 1;
            // 
            // tpgLTTotalList
            // 
            this.tpgLTTotalList.Controls.Add(this.rtbxLTTotalList);
            this.tpgLTTotalList.Location = new System.Drawing.Point(4, 26);
            this.tpgLTTotalList.Name = "tpgLTTotalList";
            this.tpgLTTotalList.Size = new System.Drawing.Size(692, 440);
            this.tpgLTTotalList.TabIndex = 9;
            this.tpgLTTotalList.Text = "Total List";
            this.tpgLTTotalList.UseVisualStyleBackColor = true;
            // 
            // rtbxLTTotalList
            // 
            this.rtbxLTTotalList.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxLTTotalList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxLTTotalList.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxLTTotalList.Location = new System.Drawing.Point(0, 0);
            this.rtbxLTTotalList.Name = "rtbxLTTotalList";
            this.rtbxLTTotalList.ReadOnly = true;
            this.rtbxLTTotalList.Size = new System.Drawing.Size(692, 440);
            this.rtbxLTTotalList.TabIndex = 34;
            this.rtbxLTTotalList.Text = "";
            // 
            // tpgLTRX5TTable
            // 
            this.tpgLTRX5TTable.Controls.Add(this.rtbxRX5TTable);
            this.tpgLTRX5TTable.Location = new System.Drawing.Point(4, 26);
            this.tpgLTRX5TTable.Name = "tpgLTRX5TTable";
            this.tpgLTRX5TTable.Size = new System.Drawing.Size(692, 440);
            this.tpgLTRX5TTable.TabIndex = 1;
            this.tpgLTRX5TTable.Text = "RX 5T";
            this.tpgLTRX5TTable.UseVisualStyleBackColor = true;
            // 
            // rtbxRX5TTable
            // 
            this.rtbxRX5TTable.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxRX5TTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxRX5TTable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxRX5TTable.Location = new System.Drawing.Point(0, 0);
            this.rtbxRX5TTable.Name = "rtbxRX5TTable";
            this.rtbxRX5TTable.Size = new System.Drawing.Size(692, 440);
            this.rtbxRX5TTable.TabIndex = 2;
            this.rtbxRX5TTable.Text = "";
            this.rtbxRX5TTable.WordWrap = false;
            // 
            // tpgLTTX5TTable
            // 
            this.tpgLTTX5TTable.Controls.Add(this.rtbxTX5TTable);
            this.tpgLTTX5TTable.Location = new System.Drawing.Point(4, 26);
            this.tpgLTTX5TTable.Name = "tpgLTTX5TTable";
            this.tpgLTTX5TTable.Size = new System.Drawing.Size(692, 440);
            this.tpgLTTX5TTable.TabIndex = 2;
            this.tpgLTTX5TTable.Text = "TX 5T";
            this.tpgLTTX5TTable.UseVisualStyleBackColor = true;
            // 
            // rtbxTX5TTable
            // 
            this.rtbxTX5TTable.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTX5TTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTX5TTable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTX5TTable.Location = new System.Drawing.Point(0, 0);
            this.rtbxTX5TTable.Name = "rtbxTX5TTable";
            this.rtbxTX5TTable.Size = new System.Drawing.Size(692, 440);
            this.rtbxTX5TTable.TabIndex = 1;
            this.rtbxTX5TTable.Text = "";
            this.rtbxTX5TTable.WordWrap = false;
            // 
            // tpgLTRX3TTable
            // 
            this.tpgLTRX3TTable.Controls.Add(this.rtbxRX3TTable);
            this.tpgLTRX3TTable.Location = new System.Drawing.Point(4, 26);
            this.tpgLTRX3TTable.Name = "tpgLTRX3TTable";
            this.tpgLTRX3TTable.Size = new System.Drawing.Size(692, 440);
            this.tpgLTRX3TTable.TabIndex = 3;
            this.tpgLTRX3TTable.Text = "RX 3T";
            this.tpgLTRX3TTable.UseVisualStyleBackColor = true;
            // 
            // rtbxRX3TTable
            // 
            this.rtbxRX3TTable.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxRX3TTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxRX3TTable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxRX3TTable.Location = new System.Drawing.Point(0, 0);
            this.rtbxRX3TTable.Name = "rtbxRX3TTable";
            this.rtbxRX3TTable.Size = new System.Drawing.Size(692, 440);
            this.rtbxRX3TTable.TabIndex = 2;
            this.rtbxRX3TTable.Text = "";
            this.rtbxRX3TTable.WordWrap = false;
            // 
            // tpgLTTX3TTable
            // 
            this.tpgLTTX3TTable.Controls.Add(this.rtbxTX3TTable);
            this.tpgLTTX3TTable.Location = new System.Drawing.Point(4, 26);
            this.tpgLTTX3TTable.Name = "tpgLTTX3TTable";
            this.tpgLTTX3TTable.Size = new System.Drawing.Size(692, 440);
            this.tpgLTTX3TTable.TabIndex = 4;
            this.tpgLTTX3TTable.Text = "TX 3T";
            this.tpgLTTX3TTable.UseVisualStyleBackColor = true;
            // 
            // rtbxTX3TTable
            // 
            this.rtbxTX3TTable.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTX3TTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTX3TTable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTX3TTable.Location = new System.Drawing.Point(0, 0);
            this.rtbxTX3TTable.Name = "rtbxTX3TTable";
            this.rtbxTX3TTable.Size = new System.Drawing.Size(692, 440);
            this.rtbxTX3TTable.TabIndex = 3;
            this.rtbxTX3TTable.Text = "";
            this.rtbxTX3TTable.WordWrap = false;
            // 
            // tpgLTRX2TLETable
            // 
            this.tpgLTRX2TLETable.Controls.Add(this.rtbxRX2TLETable);
            this.tpgLTRX2TLETable.Location = new System.Drawing.Point(4, 26);
            this.tpgLTRX2TLETable.Name = "tpgLTRX2TLETable";
            this.tpgLTRX2TLETable.Size = new System.Drawing.Size(692, 440);
            this.tpgLTRX2TLETable.TabIndex = 5;
            this.tpgLTRX2TLETable.Text = "RX 2TLE";
            this.tpgLTRX2TLETable.UseVisualStyleBackColor = true;
            // 
            // rtbxRX2TLETable
            // 
            this.rtbxRX2TLETable.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxRX2TLETable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxRX2TLETable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxRX2TLETable.Location = new System.Drawing.Point(0, 0);
            this.rtbxRX2TLETable.Name = "rtbxRX2TLETable";
            this.rtbxRX2TLETable.Size = new System.Drawing.Size(692, 440);
            this.rtbxRX2TLETable.TabIndex = 4;
            this.rtbxRX2TLETable.Text = "";
            this.rtbxRX2TLETable.WordWrap = false;
            // 
            // tpgLTTX2TLETable
            // 
            this.tpgLTTX2TLETable.Controls.Add(this.rtbxTX2TLETable);
            this.tpgLTTX2TLETable.Location = new System.Drawing.Point(4, 26);
            this.tpgLTTX2TLETable.Name = "tpgLTTX2TLETable";
            this.tpgLTTX2TLETable.Size = new System.Drawing.Size(692, 440);
            this.tpgLTTX2TLETable.TabIndex = 6;
            this.tpgLTTX2TLETable.Text = "TX 2TLE";
            this.tpgLTTX2TLETable.UseVisualStyleBackColor = true;
            // 
            // rtbxTX2TLETable
            // 
            this.rtbxTX2TLETable.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTX2TLETable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTX2TLETable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTX2TLETable.Location = new System.Drawing.Point(0, 0);
            this.rtbxTX2TLETable.Name = "rtbxTX2TLETable";
            this.rtbxTX2TLETable.Size = new System.Drawing.Size(692, 440);
            this.rtbxTX2TLETable.TabIndex = 5;
            this.rtbxTX2TLETable.Text = "";
            this.rtbxTX2TLETable.WordWrap = false;
            // 
            // tpgLTRX2THETable
            // 
            this.tpgLTRX2THETable.Controls.Add(this.rtbxRX2THETable);
            this.tpgLTRX2THETable.Location = new System.Drawing.Point(4, 26);
            this.tpgLTRX2THETable.Name = "tpgLTRX2THETable";
            this.tpgLTRX2THETable.Size = new System.Drawing.Size(692, 440);
            this.tpgLTRX2THETable.TabIndex = 7;
            this.tpgLTRX2THETable.Text = "RX 2THE";
            this.tpgLTRX2THETable.UseVisualStyleBackColor = true;
            // 
            // rtbxRX2THETable
            // 
            this.rtbxRX2THETable.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxRX2THETable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxRX2THETable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxRX2THETable.Location = new System.Drawing.Point(0, 0);
            this.rtbxRX2THETable.Name = "rtbxRX2THETable";
            this.rtbxRX2THETable.Size = new System.Drawing.Size(692, 440);
            this.rtbxRX2THETable.TabIndex = 6;
            this.rtbxRX2THETable.Text = "";
            this.rtbxRX2THETable.WordWrap = false;
            // 
            // tpgTX2THETable
            // 
            this.tpgTX2THETable.Controls.Add(this.rtbxTX2THETable);
            this.tpgTX2THETable.Location = new System.Drawing.Point(4, 26);
            this.tpgTX2THETable.Name = "tpgTX2THETable";
            this.tpgTX2THETable.Size = new System.Drawing.Size(692, 440);
            this.tpgTX2THETable.TabIndex = 8;
            this.tpgTX2THETable.Text = "TX 2THE";
            this.tpgTX2THETable.UseVisualStyleBackColor = true;
            // 
            // rtbxTX2THETable
            // 
            this.rtbxTX2THETable.BackColor = System.Drawing.Color.LightCyan;
            this.rtbxTX2THETable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxTX2THETable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxTX2THETable.Location = new System.Drawing.Point(0, 0);
            this.rtbxTX2THETable.Name = "rtbxTX2THETable";
            this.rtbxTX2THETable.Size = new System.Drawing.Size(692, 440);
            this.rtbxTX2THETable.TabIndex = 7;
            this.rtbxTX2THETable.Text = "";
            this.rtbxTX2THETable.WordWrap = false;
            // 
            // tpgCostTime
            // 
            this.tpgCostTime.Controls.Add(this.gbxCostTime);
            this.tpgCostTime.Location = new System.Drawing.Point(4, 26);
            this.tpgCostTime.Name = "tpgCostTime";
            this.tpgCostTime.Size = new System.Drawing.Size(706, 491);
            this.tpgCostTime.TabIndex = 5;
            this.tpgCostTime.Text = "Cost Time";
            this.tpgCostTime.UseVisualStyleBackColor = true;
            // 
            // gbxCostTime
            // 
            this.gbxCostTime.BackColor = System.Drawing.Color.LightCyan;
            this.gbxCostTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxCostTime.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxCostTime.Location = new System.Drawing.Point(0, 0);
            this.gbxCostTime.Name = "gbxCostTime";
            this.gbxCostTime.Size = new System.Drawing.Size(706, 491);
            this.gbxCostTime.TabIndex = 28;
            this.gbxCostTime.TabStop = false;
            this.gbxCostTime.Text = "Cost Time";
            // 
            // toolstripmenuitemRedmineTask
            // 
            this.toolstripmenuitemRedmineTask.Name = "toolstripmenuitemRedmineTask";
            this.toolstripmenuitemRedmineTask.Size = new System.Drawing.Size(156, 22);
            this.toolstripmenuitemRedmineTask.Text = "Redmine Task";
            this.toolstripmenuitemRedmineTask.Click += new System.EventHandler(this.toolstripmenuitemRedmineTask_Click);
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(918, 766);
            this.Controls.Add(this.splitcontainerOutput);
            this.Controls.Add(this.statusstripMain);
            this.Controls.Add(this.menustripTop);
            this.Controls.Add(this.splitcontainerTopRegion);
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.Text = "AutoTunning";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.statusstripMain.ResumeLayout(false);
            this.statusstripMain.PerformLayout();
            this.menustripTop.ResumeLayout(false);
            this.menustripTop.PerformLayout();
            this.splitcontainerTopRegion.Panel1.ResumeLayout(false);
            this.splitcontainerTopRegion.Panel2.ResumeLayout(false);
            this.splitcontainerTopRegion.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainerTopRegion)).EndInit();
            this.splitcontainerTopRegion.ResumeLayout(false);
            this.pnlStep.ResumeLayout(false);
            this.pnlStep.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxSplitLine)).EndInit();
            this.pnlResultMessage.ResumeLayout(false);
            this.pnlMode.ResumeLayout(false);
            this.splitcontainerOutput.Panel1.ResumeLayout(false);
            this.splitcontainerOutput.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainerOutput)).EndInit();
            this.splitcontainerOutput.ResumeLayout(false);
            this.gbxMessage.ResumeLayout(false);
            this.tcResult.ResumeLayout(false);
            this.tpgNoiseResult.ResumeLayout(false);
            this.gbxNoiseResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNoiseRank)).EndInit();
            this.tpgTNResult.ResumeLayout(false);
            this.gbxTNResult.ResumeLayout(false);
            this.tcTNResult.ResumeLayout(false);
            this.tpgTNPTHF.ResumeLayout(false);
            this.tpgTNPTHFRank.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTNPTHFRank)).EndInit();
            this.tpgTNBHF.ResumeLayout(false);
            this.tpgTNBHFRank.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTNBHFRank)).EndInit();
            this.tpgTNPreliminaryList.ResumeLayout(false);
            this.tpgTNTotalRank.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTNTotalRank)).EndInit();
            this.tpgDGTResult.ResumeLayout(false);
            this.gbxDGTResult.ResumeLayout(false);
            this.tcDGTResult.ResumeLayout(false);
            this.tpgDGTTotalList.ResumeLayout(false);
            this.tpgTPGTResult.ResumeLayout(false);
            this.gbxTPGTResult.ResumeLayout(false);
            this.tcTPGTResult.ResumeLayout(false);
            this.tpgTPGTTotalList.ResumeLayout(false);
            this.tpgPCTResult.ResumeLayout(false);
            this.gbxPCTResult.ResumeLayout(false);
            this.tcPCTResult.ResumeLayout(false);
            this.tpgPCHover_1st.ResumeLayout(false);
            this.tpgPCHover_2nd.ResumeLayout(false);
            this.tpgPCContact.ResumeLayout(false);
            this.tpgPCTotalList.ResumeLayout(false);
            this.tpgDTResult.ResumeLayout(false);
            this.gbxDTResult.ResumeLayout(false);
            this.tcDTResult.ResumeLayout(false);
            this.tpgDTHover_1st.ResumeLayout(false);
            this.tpgDTHover_2nd.ResumeLayout(false);
            this.tpgDTContact.ResumeLayout(false);
            this.tpgDTPreliminaryList.ResumeLayout(false);
            this.tpgDTHoverTRxS.ResumeLayout(false);
            this.tpgDTContactTRxS.ResumeLayout(false);
            this.tpgDTTotalList.ResumeLayout(false);
            this.tpgTTResult.ResumeLayout(false);
            this.gbxTTResult.ResumeLayout(false);
            this.tcTTResult.ResumeLayout(false);
            this.tpgTTPTHF.ResumeLayout(false);
            this.tpgTTBHF.ResumeLayout(false);
            this.tpgTTRank.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTTRank)).EndInit();
            this.tpgTTTotalList.ResumeLayout(false);
            this.tpgPTResult.ResumeLayout(false);
            this.gbxPTResult.ResumeLayout(false);
            this.tcPTResult.ResumeLayout(false);
            this.tpgPTTotalList.ResumeLayout(false);
            this.tpgPTPressureTable.ResumeLayout(false);
            this.tpgLTResult.ResumeLayout(false);
            this.gbxLTResult.ResumeLayout(false);
            this.tcLTResult.ResumeLayout(false);
            this.tpgLTTotalList.ResumeLayout(false);
            this.tpgLTRX5TTable.ResumeLayout(false);
            this.tpgLTTX5TTable.ResumeLayout(false);
            this.tpgLTRX3TTable.ResumeLayout(false);
            this.tpgLTTX3TTable.ResumeLayout(false);
            this.tpgLTRX2TLETable.ResumeLayout(false);
            this.tpgLTTX2TLETable.ResumeLayout(false);
            this.tpgLTRX2THETable.ResumeLayout(false);
            this.tpgTX2THETable.ResumeLayout(false);
            this.tpgCostTime.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusstripMain;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemHelp;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemAbout;
        private System.Windows.Forms.MenuStrip menustripTop;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemSetting;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemFlowSetting;
        private System.Windows.Forms.ToolStripProgressBar toolstripprogressbarMain;
        private System.Windows.Forms.ToolStripStatusLabel toolstripstatuslblProgress;
        private System.Windows.Forms.SplitContainer splitcontainerOutput;
        private System.Windows.Forms.Button btnChart;
        private System.Windows.Forms.GroupBox gbxMessage;
        private System.Windows.Forms.RichTextBox rtbxMessage;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemStepSetting;
        private System.Windows.Forms.GroupBox gbxNoiseResult;
        private System.Windows.Forms.DataGridView dgvNoiseRank;
        private System.Windows.Forms.SplitContainer splitcontainerTopRegion;
        private System.Windows.Forms.Panel pnlStep;
        private System.Windows.Forms.Label lblStepItem;
        private System.Windows.Forms.Label StepLbl;
        private System.Windows.Forms.ToolStripStatusLabel toolstripstatuslblStep;
        private System.Windows.Forms.TabControl tcResult;
        private System.Windows.Forms.TabPage tpgNoiseResult;
        private System.Windows.Forms.TabPage tpgDTResult;
        private System.Windows.Forms.Panel pnlMode;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.TabPage tpgTNResult;
        private System.Windows.Forms.TabPage tpgTTResult;
        private System.Windows.Forms.GroupBox gbxTNResult;
        private System.Windows.Forms.TabControl tcTNResult;
        private System.Windows.Forms.TabPage tpgTNPTHF;
        private System.Windows.Forms.TabPage tpgTNBHF;
        private System.Windows.Forms.TabPage tpgTNPreliminaryList;
        private System.Windows.Forms.DataGridView dgvTTRank;
        private System.Windows.Forms.GroupBox gbxTTResult;
        private System.Windows.Forms.TabControl tcTTResult;
        private System.Windows.Forms.TabPage tpgTTPTHF;
        private System.Windows.Forms.TabPage tpgTTBHF;
        private System.Windows.Forms.TabPage tpgTTRank;
        private System.Windows.Forms.TabPage tpgTTTotalList;
        private System.Windows.Forms.Panel pnlResultMessage;
        private System.Windows.Forms.Label lblErrorMessage;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemParameterSetting;
        private System.Windows.Forms.TabPage tpgPTResult;
        private System.Windows.Forms.GroupBox gbxPTResult;
        private System.Windows.Forms.RichTextBox rtbxPTPressureTable;
        private System.Windows.Forms.TabControl tcPTResult;
        private System.Windows.Forms.TabPage tpgPTTotalList;
        private System.Windows.Forms.TabPage tpgPTPressureTable;
        private System.Windows.Forms.TabPage tpgCostTime;
        private System.Windows.Forms.GroupBox gbxCostTime;
        private System.Windows.Forms.TabPage tpgLTResult;
        private System.Windows.Forms.GroupBox gbxLTResult;
        private System.Windows.Forms.TabControl tcLTResult;
        private System.Windows.Forms.TabPage tpgLTRX5TTable;
        private System.Windows.Forms.TabPage tpgLTTX5TTable;
        private System.Windows.Forms.RichTextBox rtbxTX5TTable;
        private System.Windows.Forms.TabPage tpgLTRX3TTable;
        private System.Windows.Forms.RichTextBox rtbxRX3TTable;
        private System.Windows.Forms.TabPage tpgLTTX3TTable;
        private System.Windows.Forms.RichTextBox rtbxTX3TTable;
        private System.Windows.Forms.TabPage tpgLTRX2TLETable;
        private System.Windows.Forms.RichTextBox rtbxRX2TLETable;
        private System.Windows.Forms.TabPage tpgLTTX2TLETable;
        private System.Windows.Forms.RichTextBox rtbxTX2TLETable;
        private System.Windows.Forms.TabPage tpgLTRX2THETable;
        private System.Windows.Forms.RichTextBox rtbxRX2THETable;
        private System.Windows.Forms.TabPage tpgTX2THETable;
        private System.Windows.Forms.RichTextBox rtbxTX2THETable;
        private System.Windows.Forms.RichTextBox rtbxRX5TTable;
        private System.Windows.Forms.TabPage tpgLTTotalList;
        private System.Windows.Forms.TabPage tpgPCTResult;
        private System.Windows.Forms.GroupBox gbxPCTResult;
        private System.Windows.Forms.TabControl tcPCTResult;
        private System.Windows.Forms.TabPage tpgPCHover_1st;
        private System.Windows.Forms.TabPage tpgPCHover_2nd;
        private System.Windows.Forms.TabPage tpgPCContact;
        private System.Windows.Forms.TabPage tpgPCTotalList;
        private System.Windows.Forms.GroupBox gbxDTResult;
        private System.Windows.Forms.TabControl tcDTResult;
        private System.Windows.Forms.TabPage tpgDTHover_1st;
        private System.Windows.Forms.TabPage tpgDTHover_2nd;
        private System.Windows.Forms.TabPage tpgDTContact;
        private System.Windows.Forms.TabPage tpgDTPreliminaryList;
        private System.Windows.Forms.TabPage tpgDTHoverTRxS;
        private System.Windows.Forms.TabPage tpgDTContactTRxS;
        private System.Windows.Forms.TabPage tpgDTTotalList;
        private System.Windows.Forms.TabPage tpgTNPTHFRank;
        private System.Windows.Forms.DataGridView dgvTNPTHFRank;
        private System.Windows.Forms.TabPage tpgTNBHFRank;
        private System.Windows.Forms.DataGridView dgvTNBHFRank;
        private System.Windows.Forms.TabPage tpgTNTotalRank;
        private System.Windows.Forms.DataGridView dgvTNTotalRank;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemFrequencySetting;
        private System.Windows.Forms.TabPage tpgDGTResult;
        private System.Windows.Forms.GroupBox gbxDGTResult;
        private System.Windows.Forms.TabControl tcDGTResult;
        private System.Windows.Forms.TabPage tpgDGTTotalList;
        private System.Windows.Forms.RichTextBox rtbxDGTTotalList;
        private System.Windows.Forms.RichTextBox rtbxTNPTHFResultList;
        private System.Windows.Forms.RichTextBox rtbxTNBHFResultList;
        private System.Windows.Forms.RichTextBox rtbxTNPreliminaryList;
        private System.Windows.Forms.RichTextBox rtbxPCTHover_1stResultList;
        private System.Windows.Forms.RichTextBox rtbxPCTHover_2ndResultList;
        private System.Windows.Forms.RichTextBox rtbxPCTContactResultList;
        private System.Windows.Forms.RichTextBox rtbxPCTTotalList;
        private System.Windows.Forms.RichTextBox rtbxDTHover_1stResultList;
        private System.Windows.Forms.RichTextBox rtbxDTHover_2ndResultList;
        private System.Windows.Forms.RichTextBox rtbxDTContactResultList;
        private System.Windows.Forms.RichTextBox rtbxDTPreliminaryList;
        private System.Windows.Forms.RichTextBox rtbxDTHoverTRxSResultList;
        private System.Windows.Forms.RichTextBox rtbxDTContactTRxSResultList;
        private System.Windows.Forms.RichTextBox rtbxDTTotalList;
        private System.Windows.Forms.RichTextBox rtbxTTPTHFResultList;
        private System.Windows.Forms.RichTextBox rtbxTTBHFResultList;
        private System.Windows.Forms.RichTextBox rtbxTTTotalList;
        private System.Windows.Forms.RichTextBox rtbxPTTotalList;
        private System.Windows.Forms.RichTextBox rtbxLTTotalList;
        private System.Windows.Forms.TabPage tpgTPGTResult;
        private System.Windows.Forms.GroupBox gbxTPGTResult;
        private System.Windows.Forms.TabControl tcTPGTResult;
        private System.Windows.Forms.TabPage tpgTPGTTotalList;
        private System.Windows.Forms.RichTextBox rtbxTPGTTotalList;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemGoDrawController;
        private System.Windows.Forms.ComboBox cbxModeState;
        private System.Windows.Forms.Label lblModeState;
        private System.Windows.Forms.Button btnNewConnect;
        private System.Windows.Forms.Button btnNewStart;
        private System.Windows.Forms.Button btnNewStop;
        private System.Windows.Forms.Button btnNewPattern;
        private System.Windows.Forms.Button btnNewDraw;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemDFTNUMAndCoeffConverter;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemSummarizeLogData;
        private System.Windows.Forms.PictureBox picbxSplitLine;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemOther;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemFeedback;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemRedmineTask;
    }
}

