namespace MPPPenAutoTuning
{
    partial class frmFlowSetting
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFlowSetting));
            this.btnDelete = new System.Windows.Forms.Button();
            this.gbxAllSelectFlow = new System.Windows.Forms.GroupBox();
            this.gbxFrequency = new System.Windows.Forms.GroupBox();
            this.btnSet = new System.Windows.Forms.Button();
            this.lblFrequencyLB = new System.Windows.Forms.Label();
            this.tbxFrequencyLB = new System.Windows.Forms.TextBox();
            this.cbxMPP_Version = new System.Windows.Forms.ComboBox();
            this.lblMPP_Version = new System.Windows.Forms.Label();
            this.cbxPH1 = new System.Windows.Forms.ComboBox();
            this.lblPH1 = new System.Windows.Forms.Label();
            this.dgvAllSelectFlow = new System.Windows.Forms.DataGridView();
            this.AllSelectItem = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AllSelectRobotState = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AllSelectRecordState = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AllSelectPH1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllSelectPH2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllSelectFrequency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnReload = new System.Windows.Forms.Button();
            this.AddBtn = new System.Windows.Forms.Button();
            this.gbxSettingFlow = new System.Windows.Forms.GroupBox();
            this.dgvSettingFlow = new System.Windows.Forms.DataGridView();
            this.SettingItem = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SettingRobotState = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.SettingRecordState = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.SettingPH1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SettingPH2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SettingFrequency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnMerge = new System.Windows.Forms.Button();
            this.picbxArrowHead = new System.Windows.Forms.PictureBox();
            this.gbxAllSelectFlow.SuspendLayout();
            this.gbxFrequency.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllSelectFlow)).BeginInit();
            this.gbxSettingFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSettingFlow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxArrowHead)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Location = new System.Drawing.Point(789, 57);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(130, 31);
            this.btnDelete.TabIndex = 14;
            this.btnDelete.Text = "Delete Line";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // gbxAllSelectFlow
            // 
            this.gbxAllSelectFlow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxAllSelectFlow.Controls.Add(this.gbxFrequency);
            this.gbxAllSelectFlow.Controls.Add(this.cbxMPP_Version);
            this.gbxAllSelectFlow.Controls.Add(this.lblMPP_Version);
            this.gbxAllSelectFlow.Controls.Add(this.cbxPH1);
            this.gbxAllSelectFlow.Controls.Add(this.lblPH1);
            this.gbxAllSelectFlow.Controls.Add(this.dgvAllSelectFlow);
            this.gbxAllSelectFlow.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxAllSelectFlow.ForeColor = System.Drawing.Color.Black;
            this.gbxAllSelectFlow.Location = new System.Drawing.Point(6, 3);
            this.gbxAllSelectFlow.Name = "gbxAllSelectFlow";
            this.gbxAllSelectFlow.Size = new System.Drawing.Size(929, 283);
            this.gbxAllSelectFlow.TabIndex = 12;
            this.gbxAllSelectFlow.TabStop = false;
            this.gbxAllSelectFlow.Text = "All Select Flow";
            // 
            // gbxFrequency
            // 
            this.gbxFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxFrequency.Controls.Add(this.btnSet);
            this.gbxFrequency.Controls.Add(this.lblFrequencyLB);
            this.gbxFrequency.Controls.Add(this.tbxFrequencyLB);
            this.gbxFrequency.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxFrequency.Location = new System.Drawing.Point(750, 101);
            this.gbxFrequency.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbxFrequency.Name = "gbxFrequency";
            this.gbxFrequency.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbxFrequency.Size = new System.Drawing.Size(175, 97);
            this.gbxFrequency.TabIndex = 12;
            this.gbxFrequency.TabStop = false;
            this.gbxFrequency.Text = SpecificText.m_sFrequency;
            // 
            // btnSet
            // 
            this.btnSet.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSet.Location = new System.Drawing.Point(97, 58);
            this.btnSet.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(76, 28);
            this.btnSet.TabIndex = 12;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // lblFrequencyLB
            // 
            this.lblFrequencyLB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFrequencyLB.AutoSize = true;
            this.lblFrequencyLB.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFrequencyLB.Location = new System.Drawing.Point(2, 26);
            this.lblFrequencyLB.Name = "lblFrequencyLB";
            this.lblFrequencyLB.Size = new System.Drawing.Size(104, 17);
            this.lblFrequencyLB.TabIndex = 8;
            this.lblFrequencyLB.Text = "Frequency LB : ";
            // 
            // tbxFrequencyLB
            // 
            this.tbxFrequencyLB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxFrequencyLB.Location = new System.Drawing.Point(107, 21);
            this.tbxFrequencyLB.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbxFrequencyLB.Name = "tbxFrequencyLB";
            this.tbxFrequencyLB.Size = new System.Drawing.Size(64, 26);
            this.tbxFrequencyLB.TabIndex = 9;
            this.tbxFrequencyLB.TextChanged += new System.EventHandler(this.tbxFrequencyLB_TextChanged);
            // 
            // cbxMPP_Version
            // 
            this.cbxMPP_Version.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxMPP_Version.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMPP_Version.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxMPP_Version.FormattingEnabled = true;
            this.cbxMPP_Version.Items.AddRange(new object[] {
            "151",
            "180"});
            this.cbxMPP_Version.Location = new System.Drawing.Point(843, 64);
            this.cbxMPP_Version.Name = "cbxMPP_Version";
            this.cbxMPP_Version.Size = new System.Drawing.Size(82, 27);
            this.cbxMPP_Version.TabIndex = 7;
            this.cbxMPP_Version.SelectedIndexChanged += new System.EventHandler(this.cbxMPP_Version_SelectedIndexChanged);
            // 
            // lblMPP_Version
            // 
            this.lblMPP_Version.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMPP_Version.AutoSize = true;
            this.lblMPP_Version.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMPP_Version.Location = new System.Drawing.Point(745, 70);
            this.lblMPP_Version.Name = "lblMPP_Version";
            this.lblMPP_Version.Size = new System.Drawing.Size(102, 17);
            this.lblMPP_Version.TabIndex = 6;
            this.lblMPP_Version.Text = "MPP_Version : ";
            // 
            // cbxPH1
            // 
            this.cbxPH1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxPH1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPH1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxPH1.FormattingEnabled = true;
            this.cbxPH1.Items.AddRange(new object[] {
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.cbxPH1.Location = new System.Drawing.Point(843, 23);
            this.cbxPH1.Name = "cbxPH1";
            this.cbxPH1.Size = new System.Drawing.Size(82, 27);
            this.cbxPH1.TabIndex = 5;
            this.cbxPH1.SelectedIndexChanged += new System.EventHandler(this.cbxPH1_SelectedIndexChanged);
            // 
            // lblPH1
            // 
            this.lblPH1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPH1.AutoSize = true;
            this.lblPH1.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPH1.Location = new System.Drawing.Point(746, 28);
            this.lblPH1.Name = "lblPH1";
            this.lblPH1.Size = new System.Drawing.Size(46, 17);
            this.lblPH1.TabIndex = 4;
            this.lblPH1.Text = "PH1 : ";
            // 
            // dgvAllSelectFlow
            // 
            this.dgvAllSelectFlow.AllowUserToAddRows = false;
            this.dgvAllSelectFlow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAllSelectFlow.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAllSelectFlow.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAllSelectFlow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAllSelectFlow.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AllSelectItem,
            this.AllSelectRobotState,
            this.AllSelectRecordState,
            this.AllSelectPH1,
            this.AllSelectPH2,
            this.AllSelectFrequency});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAllSelectFlow.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvAllSelectFlow.Location = new System.Drawing.Point(6, 23);
            this.dgvAllSelectFlow.Name = "dgvAllSelectFlow";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAllSelectFlow.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvAllSelectFlow.RowHeadersWidth = 55;
            this.dgvAllSelectFlow.RowTemplate.Height = 24;
            this.dgvAllSelectFlow.Size = new System.Drawing.Size(738, 254);
            this.dgvAllSelectFlow.TabIndex = 3;
            this.dgvAllSelectFlow.RowHeadersWidthChanged += new System.EventHandler(this.dgvAllSelectFlow_RowHeadersWidthChanged);
            this.dgvAllSelectFlow.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvAllSelectFlow_CellBeginEdit);
            this.dgvAllSelectFlow.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAllSelectFlow_CellContentClick);
            this.dgvAllSelectFlow.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAllSelectFlow_CellContentDoubleClick);
            this.dgvAllSelectFlow.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAllSelectFlow_CellEndEdit);
            this.dgvAllSelectFlow.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAllSelectFlow_CellValidated);
            // 
            // AllSelectItem
            // 
            this.AllSelectItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AllSelectItem.FillWeight = 20F;
            this.AllSelectItem.HeaderText = "Item";
            this.AllSelectItem.Name = "AllSelectItem";
            this.AllSelectItem.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.AllSelectItem.Width = 75;
            // 
            // AllSelectRobotState
            // 
            this.AllSelectRobotState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.AllSelectRobotState.DefaultCellStyle = dataGridViewCellStyle2;
            this.AllSelectRobotState.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.AllSelectRobotState.DisplayStyleForCurrentCellOnly = true;
            this.AllSelectRobotState.FillWeight = 20F;
            this.AllSelectRobotState.HeaderText = "Robot State";
            this.AllSelectRobotState.Name = "AllSelectRobotState";
            this.AllSelectRobotState.Width = 136;
            // 
            // AllSelectRecordState
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.AllSelectRecordState.DefaultCellStyle = dataGridViewCellStyle3;
            this.AllSelectRecordState.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.AllSelectRecordState.DisplayStyleForCurrentCellOnly = true;
            this.AllSelectRecordState.FillWeight = 20F;
            this.AllSelectRecordState.HeaderText = "Record State";
            this.AllSelectRecordState.Name = "AllSelectRecordState";
            this.AllSelectRecordState.Width = 134;
            // 
            // AllSelectPH1
            // 
            this.AllSelectPH1.FillWeight = 15F;
            this.AllSelectPH1.HeaderText = SpecificText.m_sPH1;
            this.AllSelectPH1.Name = "AllSelectPH1";
            this.AllSelectPH1.ReadOnly = true;
            this.AllSelectPH1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.AllSelectPH1.Width = 101;
            // 
            // AllSelectPH2
            // 
            this.AllSelectPH2.FillWeight = 15F;
            this.AllSelectPH2.HeaderText = SpecificText.m_sPH2;
            this.AllSelectPH2.Name = "AllSelectPH2";
            this.AllSelectPH2.ReadOnly = true;
            this.AllSelectPH2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.AllSelectPH2.Width = 101;
            // 
            // AllSelectFrequency
            // 
            this.AllSelectFrequency.FillWeight = 20F;
            this.AllSelectFrequency.HeaderText = SpecificText.m_sFrequency_KHz;
            this.AllSelectFrequency.Name = "AllSelectFrequency";
            this.AllSelectFrequency.ReadOnly = true;
            this.AllSelectFrequency.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.AllSelectFrequency.Width = 134;
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReload.Location = new System.Drawing.Point(789, 94);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(130, 31);
            this.btnReload.TabIndex = 22;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // AddBtn
            // 
            this.AddBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddBtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddBtn.Location = new System.Drawing.Point(789, 20);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(130, 31);
            this.AddBtn.TabIndex = 20;
            this.AddBtn.Text = "Add Line";
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // gbxSettingFlow
            // 
            this.gbxSettingFlow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxSettingFlow.Controls.Add(this.dgvSettingFlow);
            this.gbxSettingFlow.Controls.Add(this.btnReload);
            this.gbxSettingFlow.Controls.Add(this.btnSave);
            this.gbxSettingFlow.Controls.Add(this.btnDelete);
            this.gbxSettingFlow.Controls.Add(this.AddBtn);
            this.gbxSettingFlow.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxSettingFlow.ForeColor = System.Drawing.Color.Black;
            this.gbxSettingFlow.Location = new System.Drawing.Point(6, 320);
            this.gbxSettingFlow.Name = "gbxSettingFlow";
            this.gbxSettingFlow.Size = new System.Drawing.Size(929, 284);
            this.gbxSettingFlow.TabIndex = 16;
            this.gbxSettingFlow.TabStop = false;
            this.gbxSettingFlow.Text = "Setting Flow";
            // 
            // dgvSettingFlow
            // 
            this.dgvSettingFlow.AllowUserToAddRows = false;
            this.dgvSettingFlow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSettingFlow.BackgroundColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSettingFlow.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvSettingFlow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSettingFlow.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SettingItem,
            this.SettingRobotState,
            this.SettingRecordState,
            this.SettingPH1,
            this.SettingPH2,
            this.SettingFrequency});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSettingFlow.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgvSettingFlow.Location = new System.Drawing.Point(6, 28);
            this.dgvSettingFlow.Name = "dgvSettingFlow";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSettingFlow.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvSettingFlow.RowHeadersWidth = 55;
            this.dgvSettingFlow.RowTemplate.Height = 24;
            this.dgvSettingFlow.Size = new System.Drawing.Size(738, 254);
            this.dgvSettingFlow.TabIndex = 4;
            this.dgvSettingFlow.RowHeadersWidthChanged += new System.EventHandler(this.dgvSettingFlow_RowHeadersWidthChanged);
            this.dgvSettingFlow.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.SettingFlowDataGridView_CellBeginEdit);
            this.dgvSettingFlow.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSettingFlow_CellContentClick);
            this.dgvSettingFlow.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSettingFlow_CellContentDoubleClick);
            this.dgvSettingFlow.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSettingFlow_CellEndEdit);
            this.dgvSettingFlow.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSettingFlow_CellValidated);
            // 
            // SettingItem
            // 
            this.SettingItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SettingItem.FillWeight = 20F;
            this.SettingItem.HeaderText = "Item";
            this.SettingItem.Name = "SettingItem";
            this.SettingItem.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SettingItem.Width = 75;
            // 
            // SettingRobotState
            // 
            this.SettingRobotState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.SettingRobotState.DefaultCellStyle = dataGridViewCellStyle7;
            this.SettingRobotState.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.SettingRobotState.DisplayStyleForCurrentCellOnly = true;
            this.SettingRobotState.FillWeight = 20F;
            this.SettingRobotState.HeaderText = "Robot State";
            this.SettingRobotState.Name = "SettingRobotState";
            this.SettingRobotState.Width = 136;
            // 
            // SettingRecordState
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.SettingRecordState.DefaultCellStyle = dataGridViewCellStyle8;
            this.SettingRecordState.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.SettingRecordState.FillWeight = 20F;
            this.SettingRecordState.HeaderText = "Record State";
            this.SettingRecordState.Name = "SettingRecordState";
            this.SettingRecordState.Width = 134;
            // 
            // SettingPH1
            // 
            this.SettingPH1.FillWeight = 15F;
            this.SettingPH1.HeaderText = SpecificText.m_sPH1;
            this.SettingPH1.Name = SpecificText.m_sSettingPH1;
            this.SettingPH1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SettingPH1.Width = 101;
            // 
            // SettingPH2
            // 
            this.SettingPH2.FillWeight = 15F;
            this.SettingPH2.HeaderText = SpecificText.m_sPH2;
            this.SettingPH2.Name = SpecificText.m_sSettingPH2;
            this.SettingPH2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SettingPH2.Width = 101;
            // 
            // SettingFrequency
            // 
            this.SettingFrequency.FillWeight = 20F;
            this.SettingFrequency.HeaderText = SpecificText.m_sFrequency_KHz;
            this.SettingFrequency.Name = "SettingFrequency";
            this.SettingFrequency.ReadOnly = true;
            this.SettingFrequency.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SettingFrequency.Width = 134;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(789, 190);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(130, 40);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnMerge
            // 
            this.btnMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMerge.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMerge.Location = new System.Drawing.Point(795, 289);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(130, 40);
            this.btnMerge.TabIndex = 17;
            this.btnMerge.Text = "Merge";
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // picbxArrowHead
            // 
            this.picbxArrowHead.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.picbxArrowHead.Image = ((System.Drawing.Image)(resources.GetObject("picbxArrowHead.Image")));
            this.picbxArrowHead.Location = new System.Drawing.Point(345, 286);
            this.picbxArrowHead.Name = "picbxArrowHead";
            this.picbxArrowHead.Size = new System.Drawing.Size(91, 45);
            this.picbxArrowHead.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picbxArrowHead.TabIndex = 19;
            this.picbxArrowHead.TabStop = false;
            // 
            // frmFlowSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 612);
            this.Controls.Add(this.picbxArrowHead);
            this.Controls.Add(this.btnMerge);
            this.Controls.Add(this.gbxSettingFlow);
            this.Controls.Add(this.gbxAllSelectFlow);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFlowSetting";
            this.Text = "Flow Setting";
            this.Load += new System.EventHandler(this.frmFlowSetting_Load);
            this.gbxAllSelectFlow.ResumeLayout(false);
            this.gbxAllSelectFlow.PerformLayout();
            this.gbxFrequency.ResumeLayout(false);
            this.gbxFrequency.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllSelectFlow)).EndInit();
            this.gbxSettingFlow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSettingFlow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbxArrowHead)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.GroupBox gbxAllSelectFlow;
        private System.Windows.Forms.DataGridView dgvAllSelectFlow;
        private System.Windows.Forms.GroupBox gbxSettingFlow;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.PictureBox picbxArrowHead;
        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.DataGridView dgvSettingFlow;
        private System.Windows.Forms.ComboBox cbxPH1;
        private System.Windows.Forms.Label lblPH1;
        private System.Windows.Forms.ComboBox cbxMPP_Version;
        private System.Windows.Forms.Label lblMPP_Version;
        private System.Windows.Forms.DataGridViewCheckBoxColumn AllSelectItem;
        private System.Windows.Forms.DataGridViewComboBoxColumn AllSelectRobotState;
        private System.Windows.Forms.DataGridViewComboBoxColumn AllSelectRecordState;
        private System.Windows.Forms.DataGridViewTextBoxColumn AllSelectPH1;
        private System.Windows.Forms.DataGridViewTextBoxColumn AllSelectPH2;
        private System.Windows.Forms.DataGridViewTextBoxColumn AllSelectFrequency;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SettingItem;
        private System.Windows.Forms.DataGridViewComboBoxColumn SettingRobotState;
        private System.Windows.Forms.DataGridViewComboBoxColumn SettingRecordState;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingPH1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingPH2;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingFrequency;
        private System.Windows.Forms.TextBox tbxFrequencyLB;
        private System.Windows.Forms.Label lblFrequencyLB;
        private System.Windows.Forms.GroupBox gbxFrequency;
        private System.Windows.Forms.Button btnSet;
    }
}