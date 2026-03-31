namespace FingerAutoTuning
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusstripMessage = new System.Windows.Forms.StatusStrip();
            this.toolstripprogressbarProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolstripstatuslabelMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolstripstatuslabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menustripMain = new System.Windows.Forms.MenuStrip();
            this.toolstripmenuitemSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemStep = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemParameter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemTestFrequency = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemOther = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemMultiAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemGetIPAddress = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemFeedback = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemRedmineTask = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlStep = new System.Windows.Forms.Panel();
            this.lblStep = new System.Windows.Forms.Label();
            this.lblStepItem = new System.Windows.Forms.Label();
            this.rtbxMessage = new System.Windows.Forms.RichTextBox();
            this.pnlResultMessage = new System.Windows.Forms.Panel();
            this.lblSecondaryMessage = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblCurrentStep = new System.Windows.Forms.Label();
            this.tctrlResult = new System.Windows.Forms.TabControl();
            this.tpgFRPH1 = new System.Windows.Forms.TabPage();
            this.btnFRPH1Chart = new System.Windows.Forms.Button();
            this.rtbxFRPH1 = new System.Windows.Forms.RichTextBox();
            this.tpgFRPH2 = new System.Windows.Forms.TabPage();
            this.btnFRPH2Chart = new System.Windows.Forms.Button();
            this.dgvFRPH2 = new System.Windows.Forms.DataGridView();
            this.tpgACFR = new System.Windows.Forms.TabPage();
            this.btnACFRChart = new System.Windows.Forms.Button();
            this.dgvACFR = new System.Windows.Forms.DataGridView();
            this.tpgRawADCS = new System.Windows.Forms.TabPage();
            this.dgvRawADCS = new System.Windows.Forms.DataGridView();
            this.tpgSelfFS = new System.Windows.Forms.TabPage();
            this.btnSelfFSChart = new System.Windows.Forms.Button();
            this.tpgSelfPNS = new System.Windows.Forms.TabPage();
            this.rtbxSelfPNS = new System.Windows.Forms.RichTextBox();
            this.splitcontainerMain = new System.Windows.Forms.SplitContainer();
            this.splitcontainerRight = new System.Windows.Forms.SplitContainer();
            this.btnNewPattern = new System.Windows.Forms.Button();
            this.btnNewReset = new System.Windows.Forms.Button();
            this.lblLoadData = new System.Windows.Forms.Label();
            this.btnNewStop = new System.Windows.Forms.Button();
            this.rjtbtnLoadData = new RJCodeAdvance.RJControls.RJToggleButton();
            this.btnNewStart = new System.Windows.Forms.Button();
            this.statusstripMessage.SuspendLayout();
            this.menustripMain.SuspendLayout();
            this.pnlStep.SuspendLayout();
            this.pnlResultMessage.SuspendLayout();
            this.tctrlResult.SuspendLayout();
            this.tpgFRPH1.SuspendLayout();
            this.tpgFRPH2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFRPH2)).BeginInit();
            this.tpgACFR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvACFR)).BeginInit();
            this.tpgRawADCS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRawADCS)).BeginInit();
            this.tpgSelfFS.SuspendLayout();
            this.tpgSelfPNS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainerMain)).BeginInit();
            this.splitcontainerMain.Panel1.SuspendLayout();
            this.splitcontainerMain.Panel2.SuspendLayout();
            this.splitcontainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainerRight)).BeginInit();
            this.splitcontainerRight.Panel1.SuspendLayout();
            this.splitcontainerRight.Panel2.SuspendLayout();
            this.splitcontainerRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusstripMessage
            // 
            this.statusstripMessage.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusstripMessage.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusstripMessage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripprogressbarProgressBar,
            this.toolstripstatuslabelMessage,
            this.toolstripstatuslabelStatus});
            this.statusstripMessage.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusstripMessage.Location = new System.Drawing.Point(0, 576);
            this.statusstripMessage.Name = "statusstripMessage";
            this.statusstripMessage.Padding = new System.Windows.Forms.Padding(1, 0, 18, 0);
            this.statusstripMessage.Size = new System.Drawing.Size(932, 26);
            this.statusstripMessage.TabIndex = 5;
            this.statusstripMessage.Text = "statusStrip1";
            // 
            // toolstripprogressbarProgressBar
            // 
            this.toolstripprogressbarProgressBar.Name = "toolstripprogressbarProgressBar";
            this.toolstripprogressbarProgressBar.Size = new System.Drawing.Size(125, 20);
            // 
            // toolstripstatuslabelMessage
            // 
            this.toolstripstatuslabelMessage.Name = "toolstripstatuslabelMessage";
            this.toolstripstatuslabelMessage.Size = new System.Drawing.Size(0, 21);
            // 
            // toolstripstatuslabelStatus
            // 
            this.toolstripstatuslabelStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolstripstatuslabelStatus.Name = "toolstripstatuslabelStatus";
            this.toolstripstatuslabelStatus.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolstripstatuslabelStatus.Size = new System.Drawing.Size(0, 21);
            // 
            // menustripMain
            // 
            this.menustripMain.AllowMerge = false;
            this.menustripMain.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menustripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menustripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuitemSetting,
            this.toolstripmenuitemOther});
            this.menustripMain.Location = new System.Drawing.Point(0, 0);
            this.menustripMain.Name = "menustripMain";
            this.menustripMain.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menustripMain.Size = new System.Drawing.Size(932, 24);
            this.menustripMain.TabIndex = 6;
            this.menustripMain.Text = "menuStrip1";
            // 
            // toolstripmenuitemSetting
            // 
            this.toolstripmenuitemSetting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuitemStep,
            this.toolstripmenuitemParameter,
            this.toolstripmenuitemTestFrequency});
            this.toolstripmenuitemSetting.Name = "toolstripmenuitemSetting";
            this.toolstripmenuitemSetting.Size = new System.Drawing.Size(59, 20);
            this.toolstripmenuitemSetting.Text = "Setting";
            // 
            // toolstripmenuitemStep
            // 
            this.toolstripmenuitemStep.Name = "toolstripmenuitemStep";
            this.toolstripmenuitemStep.Size = new System.Drawing.Size(164, 22);
            this.toolstripmenuitemStep.Text = "Step";
            this.toolstripmenuitemStep.Click += new System.EventHandler(this.toolstripmenuitemStep_Click);
            // 
            // toolstripmenuitemParameter
            // 
            this.toolstripmenuitemParameter.Name = "toolstripmenuitemParameter";
            this.toolstripmenuitemParameter.Size = new System.Drawing.Size(164, 22);
            this.toolstripmenuitemParameter.Text = "Parameter";
            this.toolstripmenuitemParameter.Click += new System.EventHandler(this.toolstripmenuitemParameter_Click);
            // 
            // toolstripmenuitemTestFrequency
            // 
            this.toolstripmenuitemTestFrequency.Name = "toolstripmenuitemTestFrequency";
            this.toolstripmenuitemTestFrequency.Size = new System.Drawing.Size(164, 22);
            this.toolstripmenuitemTestFrequency.Text = "Test Frequency";
            this.toolstripmenuitemTestFrequency.Click += new System.EventHandler(this.toolstripmenuitemTestFrequency_Click);
            // 
            // toolstripmenuitemOther
            // 
            this.toolstripmenuitemOther.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuitemMultiAnalysis,
            this.toolstripmenuitemGetIPAddress,
            this.toolstripmenuitemFeedback,
            this.toolstripmenuitemRedmineTask});
            this.toolstripmenuitemOther.Name = "toolstripmenuitemOther";
            this.toolstripmenuitemOther.Size = new System.Drawing.Size(52, 20);
            this.toolstripmenuitemOther.Text = "Other";
            // 
            // toolstripmenuitemMultiAnalysis
            // 
            this.toolstripmenuitemMultiAnalysis.Name = "toolstripmenuitemMultiAnalysis";
            this.toolstripmenuitemMultiAnalysis.Size = new System.Drawing.Size(161, 22);
            this.toolstripmenuitemMultiAnalysis.Text = "MultiAnalysis";
            this.toolstripmenuitemMultiAnalysis.Click += new System.EventHandler(this.toolstripmenuitemMultiAnalysis_Click);
            // 
            // toolstripmenuitemGetIPAddress
            // 
            this.toolstripmenuitemGetIPAddress.Name = "toolstripmenuitemGetIPAddress";
            this.toolstripmenuitemGetIPAddress.Size = new System.Drawing.Size(161, 22);
            this.toolstripmenuitemGetIPAddress.Text = "Get IP Address";
            this.toolstripmenuitemGetIPAddress.Click += new System.EventHandler(this.toolstripmenuitemGetIPAddress_Click);
            // 
            // toolstripmenuitemFeedback
            // 
            this.toolstripmenuitemFeedback.Name = "toolstripmenuitemFeedback";
            this.toolstripmenuitemFeedback.Size = new System.Drawing.Size(161, 22);
            this.toolstripmenuitemFeedback.Text = "Feedback";
            this.toolstripmenuitemFeedback.Visible = false;
            this.toolstripmenuitemFeedback.Click += new System.EventHandler(this.toolstripmenuitemFeedback_Click);
            // 
            // toolstripmenuitemRedmineTask
            // 
            this.toolstripmenuitemRedmineTask.Name = "toolstripmenuitemRedmineTask";
            this.toolstripmenuitemRedmineTask.Size = new System.Drawing.Size(161, 22);
            this.toolstripmenuitemRedmineTask.Text = "Redmine Task";
            this.toolstripmenuitemRedmineTask.Click += new System.EventHandler(this.toolstripmenuitemRedmineTask_Click);
            // 
            // pnlStep
            // 
            this.pnlStep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlStep.BackColor = System.Drawing.Color.LightYellow;
            this.pnlStep.Controls.Add(this.lblStep);
            this.pnlStep.Controls.Add(this.lblStepItem);
            this.pnlStep.Location = new System.Drawing.Point(4, 2);
            this.pnlStep.Margin = new System.Windows.Forms.Padding(4);
            this.pnlStep.Name = "pnlStep";
            this.pnlStep.Size = new System.Drawing.Size(178, 198);
            this.pnlStep.TabIndex = 23;
            // 
            // lblStep
            // 
            this.lblStep.AutoSize = true;
            this.lblStep.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStep.Location = new System.Drawing.Point(1, 1);
            this.lblStep.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(44, 21);
            this.lblStep.TabIndex = 22;
            this.lblStep.Text = "Step";
            // 
            // lblStepItem
            // 
            this.lblStepItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStepItem.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStepItem.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepItem.Location = new System.Drawing.Point(4, 30);
            this.lblStepItem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStepItem.Name = "lblStepItem";
            this.lblStepItem.Size = new System.Drawing.Size(170, 38);
            this.lblStepItem.TabIndex = 25;
            this.lblStepItem.Text = "StepItemLbl";
            this.lblStepItem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStepItem.Visible = false;
            // 
            // rtbxMessage
            // 
            this.rtbxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbxMessage.Location = new System.Drawing.Point(4, 205);
            this.rtbxMessage.Margin = new System.Windows.Forms.Padding(4);
            this.rtbxMessage.Name = "rtbxMessage";
            this.rtbxMessage.Size = new System.Drawing.Size(177, 346);
            this.rtbxMessage.TabIndex = 29;
            this.rtbxMessage.Text = "";
            this.rtbxMessage.TextChanged += new System.EventHandler(this.MessageRtbx_TextChanged);
            // 
            // pnlResultMessage
            // 
            this.pnlResultMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlResultMessage.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pnlResultMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlResultMessage.Controls.Add(this.lblSecondaryMessage);
            this.pnlResultMessage.Controls.Add(this.lblStatus);
            this.pnlResultMessage.Location = new System.Drawing.Point(1, 75);
            this.pnlResultMessage.Margin = new System.Windows.Forms.Padding(0);
            this.pnlResultMessage.Name = "pnlResultMessage";
            this.pnlResultMessage.Size = new System.Drawing.Size(745, 82);
            this.pnlResultMessage.TabIndex = 28;
            // 
            // lblSecondaryMessage
            // 
            this.lblSecondaryMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSecondaryMessage.Font = new System.Drawing.Font("Times New Roman", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecondaryMessage.Location = new System.Drawing.Point(0, 52);
            this.lblSecondaryMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSecondaryMessage.Name = "lblSecondaryMessage";
            this.lblSecondaryMessage.Size = new System.Drawing.Size(744, 26);
            this.lblSecondaryMessage.TabIndex = 1;
            this.lblSecondaryMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSecondaryMessage.Resize += new System.EventHandler(this.lblErrorMessage_Resize);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(0, -1);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(744, 50);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCurrentStep
            // 
            this.lblCurrentStep.BackColor = System.Drawing.Color.PaleTurquoise;
            this.lblCurrentStep.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCurrentStep.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentStep.Location = new System.Drawing.Point(1, 7);
            this.lblCurrentStep.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentStep.Name = "lblCurrentStep";
            this.lblCurrentStep.Size = new System.Drawing.Size(251, 64);
            this.lblCurrentStep.TabIndex = 30;
            this.lblCurrentStep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tctrlResult
            // 
            this.tctrlResult.Controls.Add(this.tpgFRPH1);
            this.tctrlResult.Controls.Add(this.tpgFRPH2);
            this.tctrlResult.Controls.Add(this.tpgACFR);
            this.tctrlResult.Controls.Add(this.tpgRawADCS);
            this.tctrlResult.Controls.Add(this.tpgSelfFS);
            this.tctrlResult.Controls.Add(this.tpgSelfPNS);
            this.tctrlResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tctrlResult.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tctrlResult.Location = new System.Drawing.Point(0, 0);
            this.tctrlResult.Margin = new System.Windows.Forms.Padding(0);
            this.tctrlResult.Name = "tctrlResult";
            this.tctrlResult.SelectedIndex = 0;
            this.tctrlResult.Size = new System.Drawing.Size(748, 390);
            this.tctrlResult.TabIndex = 32;
            // 
            // tpgFRPH1
            // 
            this.tpgFRPH1.Controls.Add(this.btnFRPH1Chart);
            this.tpgFRPH1.Controls.Add(this.rtbxFRPH1);
            this.tpgFRPH1.Location = new System.Drawing.Point(4, 26);
            this.tpgFRPH1.Margin = new System.Windows.Forms.Padding(4);
            this.tpgFRPH1.Name = "tpgFRPH1";
            this.tpgFRPH1.Padding = new System.Windows.Forms.Padding(4);
            this.tpgFRPH1.Size = new System.Drawing.Size(740, 360);
            this.tpgFRPH1.TabIndex = 0;
            this.tpgFRPH1.Text = "FrequencyRank PH1";
            this.tpgFRPH1.UseVisualStyleBackColor = true;
            // 
            // btnFRPH1Chart
            // 
            this.btnFRPH1Chart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFRPH1Chart.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFRPH1Chart.ForeColor = System.Drawing.Color.Blue;
            this.btnFRPH1Chart.Image = ((System.Drawing.Image)(resources.GetObject("btnFRPH1Chart.Image")));
            this.btnFRPH1Chart.Location = new System.Drawing.Point(645, 312);
            this.btnFRPH1Chart.Margin = new System.Windows.Forms.Padding(4);
            this.btnFRPH1Chart.Name = "btnFRPH1Chart";
            this.btnFRPH1Chart.Size = new System.Drawing.Size(90, 40);
            this.btnFRPH1Chart.TabIndex = 2;
            this.btnFRPH1Chart.Text = "Chart";
            this.btnFRPH1Chart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFRPH1Chart.UseVisualStyleBackColor = true;
            this.btnFRPH1Chart.Click += new System.EventHandler(this.btnFRPH1Chart_Click);
            // 
            // rtbxFRPH1
            // 
            this.rtbxFRPH1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbxFRPH1.BackColor = System.Drawing.Color.PaleTurquoise;
            this.rtbxFRPH1.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxFRPH1.Location = new System.Drawing.Point(4, 4);
            this.rtbxFRPH1.Margin = new System.Windows.Forms.Padding(4);
            this.rtbxFRPH1.Name = "rtbxFRPH1";
            this.rtbxFRPH1.ReadOnly = true;
            this.rtbxFRPH1.Size = new System.Drawing.Size(732, 301);
            this.rtbxFRPH1.TabIndex = 0;
            this.rtbxFRPH1.Text = "";
            // 
            // tpgFRPH2
            // 
            this.tpgFRPH2.Controls.Add(this.btnFRPH2Chart);
            this.tpgFRPH2.Controls.Add(this.dgvFRPH2);
            this.tpgFRPH2.Location = new System.Drawing.Point(4, 26);
            this.tpgFRPH2.Margin = new System.Windows.Forms.Padding(4);
            this.tpgFRPH2.Name = "tpgFRPH2";
            this.tpgFRPH2.Padding = new System.Windows.Forms.Padding(4);
            this.tpgFRPH2.Size = new System.Drawing.Size(740, 360);
            this.tpgFRPH2.TabIndex = 1;
            this.tpgFRPH2.Text = "FrequencyRank PH2";
            this.tpgFRPH2.UseVisualStyleBackColor = true;
            // 
            // btnFRPH2Chart
            // 
            this.btnFRPH2Chart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFRPH2Chart.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFRPH2Chart.ForeColor = System.Drawing.Color.Blue;
            this.btnFRPH2Chart.Image = ((System.Drawing.Image)(resources.GetObject("btnFRPH2Chart.Image")));
            this.btnFRPH2Chart.Location = new System.Drawing.Point(645, 314);
            this.btnFRPH2Chart.Margin = new System.Windows.Forms.Padding(4);
            this.btnFRPH2Chart.Name = "btnFRPH2Chart";
            this.btnFRPH2Chart.Size = new System.Drawing.Size(90, 40);
            this.btnFRPH2Chart.TabIndex = 1;
            this.btnFRPH2Chart.Text = "Chart";
            this.btnFRPH2Chart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFRPH2Chart.UseVisualStyleBackColor = true;
            this.btnFRPH2Chart.Click += new System.EventHandler(this.btnFRPH2Chart_Click);
            // 
            // dgvFRPH2
            // 
            this.dgvFRPH2.AllowUserToAddRows = false;
            this.dgvFRPH2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFRPH2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFRPH2.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgvFRPH2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFRPH2.Location = new System.Drawing.Point(0, 0);
            this.dgvFRPH2.Margin = new System.Windows.Forms.Padding(4);
            this.dgvFRPH2.Name = "dgvFRPH2";
            this.dgvFRPH2.RowTemplate.Height = 24;
            this.dgvFRPH2.Size = new System.Drawing.Size(740, 310);
            this.dgvFRPH2.TabIndex = 0;
            this.dgvFRPH2.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvFRPH2_CellFormatting);
            this.dgvFRPH2.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvFRPH2_CellPainting);
            this.dgvFRPH2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvFRPH2_Scroll);
            this.dgvFRPH2.SizeChanged += new System.EventHandler(this.dgvFRPH2_SizeChanged);
            // 
            // tpgACFR
            // 
            this.tpgACFR.Controls.Add(this.btnACFRChart);
            this.tpgACFR.Controls.Add(this.dgvACFR);
            this.tpgACFR.Location = new System.Drawing.Point(4, 26);
            this.tpgACFR.Margin = new System.Windows.Forms.Padding(4);
            this.tpgACFR.Name = "tpgACFR";
            this.tpgACFR.Size = new System.Drawing.Size(740, 360);
            this.tpgACFR.TabIndex = 2;
            this.tpgACFR.Text = "AC FrequencyRank";
            this.tpgACFR.UseVisualStyleBackColor = true;
            // 
            // btnACFRChart
            // 
            this.btnACFRChart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnACFRChart.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnACFRChart.ForeColor = System.Drawing.Color.Blue;
            this.btnACFRChart.Image = ((System.Drawing.Image)(resources.GetObject("btnACFRChart.Image")));
            this.btnACFRChart.Location = new System.Drawing.Point(645, 314);
            this.btnACFRChart.Margin = new System.Windows.Forms.Padding(4);
            this.btnACFRChart.Name = "btnACFRChart";
            this.btnACFRChart.Size = new System.Drawing.Size(90, 40);
            this.btnACFRChart.TabIndex = 2;
            this.btnACFRChart.Text = "Chart";
            this.btnACFRChart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnACFRChart.UseVisualStyleBackColor = true;
            this.btnACFRChart.Click += new System.EventHandler(this.btnACFRChart_Click);
            // 
            // dgvACFR
            // 
            this.dgvACFR.AllowUserToAddRows = false;
            this.dgvACFR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvACFR.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvACFR.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgvACFR.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvACFR.Location = new System.Drawing.Point(0, 0);
            this.dgvACFR.Margin = new System.Windows.Forms.Padding(4);
            this.dgvACFR.Name = "dgvACFR";
            this.dgvACFR.RowTemplate.Height = 24;
            this.dgvACFR.Size = new System.Drawing.Size(740, 310);
            this.dgvACFR.TabIndex = 1;
            this.dgvACFR.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvACFR_CellFormatting);
            this.dgvACFR.SizeChanged += new System.EventHandler(this.dgvACFR_SizeChanged);
            // 
            // tpgRawADCS
            // 
            this.tpgRawADCS.Controls.Add(this.dgvRawADCS);
            this.tpgRawADCS.Location = new System.Drawing.Point(4, 26);
            this.tpgRawADCS.Name = "tpgRawADCS";
            this.tpgRawADCS.Size = new System.Drawing.Size(740, 360);
            this.tpgRawADCS.TabIndex = 5;
            this.tpgRawADCS.Text = "Raw ADC Sweep";
            this.tpgRawADCS.UseVisualStyleBackColor = true;
            // 
            // dgvRawADCS
            // 
            this.dgvRawADCS.AllowUserToAddRows = false;
            this.dgvRawADCS.AllowUserToDeleteRows = false;
            this.dgvRawADCS.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRawADCS.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgvRawADCS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRawADCS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRawADCS.Location = new System.Drawing.Point(0, 0);
            this.dgvRawADCS.Margin = new System.Windows.Forms.Padding(4);
            this.dgvRawADCS.Name = "dgvRawADCS";
            this.dgvRawADCS.ReadOnly = true;
            this.dgvRawADCS.RowTemplate.Height = 24;
            this.dgvRawADCS.Size = new System.Drawing.Size(740, 360);
            this.dgvRawADCS.TabIndex = 1;
            this.dgvRawADCS.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvRawADCS_CellFormatting);
            this.dgvRawADCS.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvRawADCS_CellPainting);
            this.dgvRawADCS.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvRawADCS_Scroll);
            this.dgvRawADCS.Sorted += new System.EventHandler(this.dgvRawADCS_Sorted);
            this.dgvRawADCS.SizeChanged += new System.EventHandler(this.dgvRawADCS_SizeChanged);
            // 
            // tpgSelfFS
            // 
            this.tpgSelfFS.Controls.Add(this.btnSelfFSChart);
            this.tpgSelfFS.Location = new System.Drawing.Point(4, 26);
            this.tpgSelfFS.Name = "tpgSelfFS";
            this.tpgSelfFS.Size = new System.Drawing.Size(740, 360);
            this.tpgSelfFS.TabIndex = 3;
            this.tpgSelfFS.Text = "Self FrequencySweep";
            this.tpgSelfFS.UseVisualStyleBackColor = true;
            // 
            // btnSelfFSChart
            // 
            this.btnSelfFSChart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelfFSChart.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelfFSChart.ForeColor = System.Drawing.Color.Blue;
            this.btnSelfFSChart.Image = ((System.Drawing.Image)(resources.GetObject("btnSelfFSChart.Image")));
            this.btnSelfFSChart.Location = new System.Drawing.Point(645, 312);
            this.btnSelfFSChart.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelfFSChart.Name = "btnSelfFSChart";
            this.btnSelfFSChart.Size = new System.Drawing.Size(90, 40);
            this.btnSelfFSChart.TabIndex = 3;
            this.btnSelfFSChart.Text = "Chart";
            this.btnSelfFSChart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSelfFSChart.UseVisualStyleBackColor = true;
            this.btnSelfFSChart.Click += new System.EventHandler(this.btnSelfFSChart_Click);
            // 
            // tpgSelfPNS
            // 
            this.tpgSelfPNS.Controls.Add(this.rtbxSelfPNS);
            this.tpgSelfPNS.Location = new System.Drawing.Point(4, 26);
            this.tpgSelfPNS.Name = "tpgSelfPNS";
            this.tpgSelfPNS.Size = new System.Drawing.Size(740, 360);
            this.tpgSelfPNS.TabIndex = 4;
            this.tpgSelfPNS.Text = "Self NCPNCNSweep";
            this.tpgSelfPNS.UseVisualStyleBackColor = true;
            // 
            // rtbxSelfPNS
            // 
            this.rtbxSelfPNS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbxSelfPNS.BackColor = System.Drawing.Color.PaleTurquoise;
            this.rtbxSelfPNS.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxSelfPNS.Location = new System.Drawing.Point(4, 4);
            this.rtbxSelfPNS.Margin = new System.Windows.Forms.Padding(4);
            this.rtbxSelfPNS.Name = "rtbxSelfPNS";
            this.rtbxSelfPNS.ReadOnly = true;
            this.rtbxSelfPNS.Size = new System.Drawing.Size(732, 301);
            this.rtbxSelfPNS.TabIndex = 1;
            this.rtbxSelfPNS.Text = "";
            // 
            // splitcontainerMain
            // 
            this.splitcontainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitcontainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitcontainerMain.Location = new System.Drawing.Point(0, 24);
            this.splitcontainerMain.Margin = new System.Windows.Forms.Padding(4);
            this.splitcontainerMain.Name = "splitcontainerMain";
            // 
            // splitcontainerMain.Panel1
            // 
            this.splitcontainerMain.Panel1.Controls.Add(this.pnlStep);
            this.splitcontainerMain.Panel1.Controls.Add(this.rtbxMessage);
            // 
            // splitcontainerMain.Panel2
            // 
            this.splitcontainerMain.Panel2.Controls.Add(this.splitcontainerRight);
            this.splitcontainerMain.Size = new System.Drawing.Size(932, 552);
            this.splitcontainerMain.SplitterDistance = 183;
            this.splitcontainerMain.SplitterWidth = 1;
            this.splitcontainerMain.TabIndex = 34;
            // 
            // splitcontainerRight
            // 
            this.splitcontainerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitcontainerRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitcontainerRight.IsSplitterFixed = true;
            this.splitcontainerRight.Location = new System.Drawing.Point(0, 0);
            this.splitcontainerRight.Name = "splitcontainerRight";
            this.splitcontainerRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitcontainerRight.Panel1
            // 
            this.splitcontainerRight.Panel1.Controls.Add(this.lblCurrentStep);
            this.splitcontainerRight.Panel1.Controls.Add(this.btnNewPattern);
            this.splitcontainerRight.Panel1.Controls.Add(this.pnlResultMessage);
            this.splitcontainerRight.Panel1.Controls.Add(this.btnNewReset);
            this.splitcontainerRight.Panel1.Controls.Add(this.lblLoadData);
            this.splitcontainerRight.Panel1.Controls.Add(this.btnNewStop);
            this.splitcontainerRight.Panel1.Controls.Add(this.rjtbtnLoadData);
            this.splitcontainerRight.Panel1.Controls.Add(this.btnNewStart);
            // 
            // splitcontainerRight.Panel2
            // 
            this.splitcontainerRight.Panel2.Controls.Add(this.tctrlResult);
            this.splitcontainerRight.Size = new System.Drawing.Size(748, 552);
            this.splitcontainerRight.SplitterDistance = 158;
            this.splitcontainerRight.TabIndex = 40;
            // 
            // btnNewPattern
            // 
            this.btnNewPattern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewPattern.FlatAppearance.BorderSize = 0;
            this.btnNewPattern.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewPattern.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewPattern.ForeColor = System.Drawing.Color.Purple;
            this.btnNewPattern.Image = ((System.Drawing.Image)(resources.GetObject("btnNewPattern.Image")));
            this.btnNewPattern.Location = new System.Drawing.Point(668, 5);
            this.btnNewPattern.Name = "btnNewPattern";
            this.btnNewPattern.Size = new System.Drawing.Size(74, 68);
            this.btnNewPattern.TabIndex = 39;
            this.btnNewPattern.Text = "Pattern";
            this.btnNewPattern.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNewPattern.UseVisualStyleBackColor = true;
            this.btnNewPattern.Click += new System.EventHandler(this.btnNewPattern_Click);
            this.btnNewPattern.MouseEnter += new System.EventHandler(this.btnNewPattern_MouseEnter);
            this.btnNewPattern.MouseLeave += new System.EventHandler(this.btnNewPattern_MouseLeave);
            this.btnNewPattern.MouseHover += new System.EventHandler(this.btnNewPattern_MouseHover);
            this.btnNewPattern.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnNewPattern_MouseMove);
            // 
            // btnNewReset
            // 
            this.btnNewReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewReset.FlatAppearance.BorderSize = 0;
            this.btnNewReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewReset.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewReset.ForeColor = System.Drawing.Color.Green;
            this.btnNewReset.Image = ((System.Drawing.Image)(resources.GetObject("btnNewReset.Image")));
            this.btnNewReset.Location = new System.Drawing.Point(588, 5);
            this.btnNewReset.Name = "btnNewReset";
            this.btnNewReset.Size = new System.Drawing.Size(74, 68);
            this.btnNewReset.TabIndex = 38;
            this.btnNewReset.Text = "Reset";
            this.btnNewReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNewReset.UseVisualStyleBackColor = true;
            this.btnNewReset.Click += new System.EventHandler(this.btnNewReset_Click);
            this.btnNewReset.MouseEnter += new System.EventHandler(this.btnNewReset_MouseEnter);
            this.btnNewReset.MouseLeave += new System.EventHandler(this.btnNewReset_MouseLeave);
            this.btnNewReset.MouseHover += new System.EventHandler(this.btnNewReset_MouseHover);
            this.btnNewReset.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnNewReset_MouseMove);
            // 
            // lblLoadData
            // 
            this.lblLoadData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLoadData.AutoSize = true;
            this.lblLoadData.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoadData.Location = new System.Drawing.Point(285, 49);
            this.lblLoadData.Name = "lblLoadData";
            this.lblLoadData.Size = new System.Drawing.Size(74, 19);
            this.lblLoadData.TabIndex = 34;
            this.lblLoadData.Text = "Load Data";
            // 
            // btnNewStop
            // 
            this.btnNewStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewStop.FlatAppearance.BorderSize = 0;
            this.btnNewStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewStop.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewStop.ForeColor = System.Drawing.Color.Red;
            this.btnNewStop.Image = ((System.Drawing.Image)(resources.GetObject("btnNewStop.Image")));
            this.btnNewStop.Location = new System.Drawing.Point(508, 5);
            this.btnNewStop.Name = "btnNewStop";
            this.btnNewStop.Size = new System.Drawing.Size(74, 68);
            this.btnNewStop.TabIndex = 37;
            this.btnNewStop.Text = "Stop";
            this.btnNewStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNewStop.UseVisualStyleBackColor = true;
            this.btnNewStop.Click += new System.EventHandler(this.btnNewStop_Click);
            this.btnNewStop.MouseEnter += new System.EventHandler(this.btnNewStop_MouseEnter);
            this.btnNewStop.MouseLeave += new System.EventHandler(this.btnNewStop_MouseLeave);
            this.btnNewStop.MouseHover += new System.EventHandler(this.btnNewStop_MouseHover);
            this.btnNewStop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnNewStop_MouseMove);
            // 
            // rjtbtnLoadData
            // 
            this.rjtbtnLoadData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rjtbtnLoadData.AutoSize = true;
            this.rjtbtnLoadData.Location = new System.Drawing.Point(378, 49);
            this.rjtbtnLoadData.MinimumSize = new System.Drawing.Size(45, 22);
            this.rjtbtnLoadData.Name = "rjtbtnLoadData";
            this.rjtbtnLoadData.OffBackColor = System.Drawing.Color.Gray;
            this.rjtbtnLoadData.OffToggleColor = System.Drawing.Color.Gainsboro;
            this.rjtbtnLoadData.OnBackColor = System.Drawing.Color.MediumSlateBlue;
            this.rjtbtnLoadData.OnToggleColor = System.Drawing.Color.WhiteSmoke;
            this.rjtbtnLoadData.Size = new System.Drawing.Size(45, 22);
            this.rjtbtnLoadData.TabIndex = 35;
            this.rjtbtnLoadData.UseVisualStyleBackColor = true;
            this.rjtbtnLoadData.CheckedChanged += new System.EventHandler(this.rjtbtnLoadData_CheckedChanged);
            // 
            // btnNewStart
            // 
            this.btnNewStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewStart.FlatAppearance.BorderSize = 0;
            this.btnNewStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewStart.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewStart.ForeColor = System.Drawing.Color.Navy;
            this.btnNewStart.Image = ((System.Drawing.Image)(resources.GetObject("btnNewStart.Image")));
            this.btnNewStart.Location = new System.Drawing.Point(428, 5);
            this.btnNewStart.Name = "btnNewStart";
            this.btnNewStart.Size = new System.Drawing.Size(74, 68);
            this.btnNewStart.TabIndex = 36;
            this.btnNewStart.Text = "Start";
            this.btnNewStart.UseVisualStyleBackColor = true;
            this.btnNewStart.Click += new System.EventHandler(this.btnNewStart_Click);
            this.btnNewStart.MouseEnter += new System.EventHandler(this.btnNewStart_MouseEnter);
            this.btnNewStart.MouseLeave += new System.EventHandler(this.btnNewStart_MouseLeave);
            this.btnNewStart.MouseHover += new System.EventHandler(this.btnNewStart_MouseHover);
            this.btnNewStart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnNewStart_MouseMove);
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(932, 602);
            this.Controls.Add(this.splitcontainerMain);
            this.Controls.Add(this.statusstripMessage);
            this.Controls.Add(this.menustripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menustripMain;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.statusstripMessage.ResumeLayout(false);
            this.statusstripMessage.PerformLayout();
            this.menustripMain.ResumeLayout(false);
            this.menustripMain.PerformLayout();
            this.pnlStep.ResumeLayout(false);
            this.pnlStep.PerformLayout();
            this.pnlResultMessage.ResumeLayout(false);
            this.tctrlResult.ResumeLayout(false);
            this.tpgFRPH1.ResumeLayout(false);
            this.tpgFRPH2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFRPH2)).EndInit();
            this.tpgACFR.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvACFR)).EndInit();
            this.tpgRawADCS.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRawADCS)).EndInit();
            this.tpgSelfFS.ResumeLayout(false);
            this.tpgSelfPNS.ResumeLayout(false);
            this.splitcontainerMain.Panel1.ResumeLayout(false);
            this.splitcontainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainerMain)).EndInit();
            this.splitcontainerMain.ResumeLayout(false);
            this.splitcontainerRight.Panel1.ResumeLayout(false);
            this.splitcontainerRight.Panel1.PerformLayout();
            this.splitcontainerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainerRight)).EndInit();
            this.splitcontainerRight.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusstripMessage;
        private System.Windows.Forms.MenuStrip menustripMain;
        private System.Windows.Forms.Panel pnlStep;
        private System.Windows.Forms.Label lblStep;
        private System.Windows.Forms.Label lblStepItem;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemSetting;
        private System.Windows.Forms.RichTextBox rtbxMessage;
        private System.Windows.Forms.Panel pnlResultMessage;
        private System.Windows.Forms.Label lblSecondaryMessage;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemStep;
        private System.Windows.Forms.ToolStripStatusLabel toolstripstatuslabelStatus;
        private System.Windows.Forms.Label lblCurrentStep;
        private System.Windows.Forms.ToolStripProgressBar toolstripprogressbarProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolstripstatuslabelMessage;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemTestFrequency;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemParameter;
        private System.Windows.Forms.TabControl tctrlResult;
        private System.Windows.Forms.TabPage tpgFRPH1;
        private System.Windows.Forms.TabPage tpgFRPH2;
        private System.Windows.Forms.RichTextBox rtbxFRPH1;
        private System.Windows.Forms.DataGridView dgvFRPH2;
        private System.Windows.Forms.Button btnFRPH2Chart;
        private System.Windows.Forms.Button btnFRPH1Chart;
        private System.Windows.Forms.TabPage tpgACFR;
        private System.Windows.Forms.DataGridView dgvACFR;
        private System.Windows.Forms.Button btnACFRChart;
        private System.Windows.Forms.SplitContainer splitcontainerMain;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemOther;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemMultiAnalysis;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemGetIPAddress;
        private System.Windows.Forms.TabPage tpgSelfFS;
        private System.Windows.Forms.Button btnSelfFSChart;
        private System.Windows.Forms.TabPage tpgSelfPNS;
        private System.Windows.Forms.RichTextBox rtbxSelfPNS;
        private RJCodeAdvance.RJControls.RJToggleButton rjtbtnLoadData;
        private System.Windows.Forms.Label lblLoadData;
        private System.Windows.Forms.Button btnNewStart;
        private System.Windows.Forms.Button btnNewStop;
        private System.Windows.Forms.Button btnNewReset;
        private System.Windows.Forms.Button btnNewPattern;
        private System.Windows.Forms.SplitContainer splitcontainerRight;
        private System.Windows.Forms.TabPage tpgRawADCS;
        private System.Windows.Forms.DataGridView dgvRawADCS;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemFeedback;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemRedmineTask;
    }
}

