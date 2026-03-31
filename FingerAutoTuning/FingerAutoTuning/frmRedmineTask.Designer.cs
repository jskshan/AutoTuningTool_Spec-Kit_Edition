namespace FingerAutoTuning
{
    partial class frmRedmineTask
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRedmineTask));
            this.btnSubmit = new System.Windows.Forms.Button();
            this.lblAPIKey = new System.Windows.Forms.Label();
            this.tbxAPIKey = new System.Windows.Forms.TextBox();
            this.rtbxMessage = new System.Windows.Forms.RichTextBox();
            this.tbxSubject = new System.Windows.Forms.TextBox();
            this.lblSubject = new System.Windows.Forms.Label();
            this.lblAssignedTo = new System.Windows.Forms.Label();
            this.cbxAssignTo = new System.Windows.Forms.ComboBox();
            this.splitContainerHorizontalLayer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainerHorizontalLayer2 = new System.Windows.Forms.SplitContainer();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblStatusMessage = new System.Windows.Forms.Label();
            this.gbxMessage = new System.Windows.Forms.GroupBox();
            this.gbxSetting = new System.Windows.Forms.GroupBox();
            this.cbxInterface = new System.Windows.Forms.ComboBox();
            this.lblInterface = new System.Windows.Forms.Label();
            this.cbxSocketType = new System.Windows.Forms.ComboBox();
            this.lblSocketType = new System.Windows.Forms.Label();
            this.lblICSolution = new System.Windows.Forms.Label();
            this.tbxICSolution = new System.Windows.Forms.TextBox();
            this.tcCreateIssueData = new System.Windows.Forms.TabControl();
            this.tpDescription = new System.Windows.Forms.TabPage();
            this.lblDescription = new System.Windows.Forms.Label();
            this.rtbxDescription = new System.Windows.Forms.RichTextBox();
            this.tpUploadFile = new System.Windows.Forms.TabPage();
            this.dgvUploadFile = new System.Windows.Forms.DataGridView();
            this.ColumnItem = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnFilePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSelectUploadFile = new System.Windows.Forms.Button();
            this.btnDeleteUploadFile = new System.Windows.Forms.Button();
            this.lblToolVersion = new System.Windows.Forms.Label();
            this.tbxToolVersion = new System.Windows.Forms.TextBox();
            this.cbxLoginType = new System.Windows.Forms.ComboBox();
            this.lblLoginType = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.tbxPassword = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.tbxUserName = new System.Windows.Forms.TextBox();
            this.ckbxHide = new System.Windows.Forms.CheckBox();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.tbxProjectName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontalLayer1)).BeginInit();
            this.splitContainerHorizontalLayer1.Panel1.SuspendLayout();
            this.splitContainerHorizontalLayer1.Panel2.SuspendLayout();
            this.splitContainerHorizontalLayer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontalLayer2)).BeginInit();
            this.splitContainerHorizontalLayer2.Panel1.SuspendLayout();
            this.splitContainerHorizontalLayer2.Panel2.SuspendLayout();
            this.splitContainerHorizontalLayer2.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.gbxMessage.SuspendLayout();
            this.gbxSetting.SuspendLayout();
            this.tcCreateIssueData.SuspendLayout();
            this.tpDescription.SuspendLayout();
            this.tpUploadFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUploadFile)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSubmit.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Location = new System.Drawing.Point(469, 446);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(105, 33);
            this.btnSubmit.TabIndex = 27;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // lblAPIKey
            // 
            this.lblAPIKey.AutoSize = true;
            this.lblAPIKey.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAPIKey.Location = new System.Drawing.Point(8, 45);
            this.lblAPIKey.Name = "lblAPIKey";
            this.lblAPIKey.Size = new System.Drawing.Size(62, 15);
            this.lblAPIKey.TabIndex = 6;
            this.lblAPIKey.Text = "API Key : ";
            // 
            // tbxAPIKey
            // 
            this.tbxAPIKey.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxAPIKey.Location = new System.Drawing.Point(92, 42);
            this.tbxAPIKey.Name = "tbxAPIKey";
            this.tbxAPIKey.Size = new System.Drawing.Size(544, 22);
            this.tbxAPIKey.TabIndex = 7;
            // 
            // rtbxMessage
            // 
            this.rtbxMessage.BackColor = System.Drawing.SystemColors.Window;
            this.rtbxMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxMessage.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxMessage.Location = new System.Drawing.Point(3, 18);
            this.rtbxMessage.Name = "rtbxMessage";
            this.rtbxMessage.ReadOnly = true;
            this.rtbxMessage.Size = new System.Drawing.Size(1053, 72);
            this.rtbxMessage.TabIndex = 30;
            this.rtbxMessage.Text = "";
            this.rtbxMessage.TextChanged += new System.EventHandler(this.rtbxMessage_TextChanged);
            // 
            // tbxSubject
            // 
            this.tbxSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxSubject.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxSubject.Location = new System.Drawing.Point(92, 68);
            this.tbxSubject.Name = "tbxSubject";
            this.tbxSubject.Size = new System.Drawing.Size(959, 22);
            this.tbxSubject.TabIndex = 9;
            // 
            // lblSubject
            // 
            this.lblSubject.AutoSize = true;
            this.lblSubject.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubject.Location = new System.Drawing.Point(8, 73);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(56, 15);
            this.lblSubject.TabIndex = 8;
            this.lblSubject.Text = "Subject : ";
            // 
            // lblAssignedTo
            // 
            this.lblAssignedTo.AutoSize = true;
            this.lblAssignedTo.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAssignedTo.Location = new System.Drawing.Point(8, 101);
            this.lblAssignedTo.Name = "lblAssignedTo";
            this.lblAssignedTo.Size = new System.Drawing.Size(82, 15);
            this.lblAssignedTo.TabIndex = 10;
            this.lblAssignedTo.Text = "Assigned to : ";
            // 
            // cbxAssignTo
            // 
            this.cbxAssignTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAssignTo.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxAssignTo.FormattingEnabled = true;
            this.cbxAssignTo.Location = new System.Drawing.Point(92, 96);
            this.cbxAssignTo.Name = "cbxAssignTo";
            this.cbxAssignTo.Size = new System.Drawing.Size(182, 23);
            this.cbxAssignTo.TabIndex = 11;
            // 
            // splitContainerHorizontalLayer1
            // 
            this.splitContainerHorizontalLayer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerHorizontalLayer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerHorizontalLayer1.Name = "splitContainerHorizontalLayer1";
            this.splitContainerHorizontalLayer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerHorizontalLayer1.Panel1
            // 
            this.splitContainerHorizontalLayer1.Panel1.Controls.Add(this.splitContainerHorizontalLayer2);
            // 
            // splitContainerHorizontalLayer1.Panel2
            // 
            this.splitContainerHorizontalLayer1.Panel2.Controls.Add(this.gbxSetting);
            this.splitContainerHorizontalLayer1.Size = new System.Drawing.Size(1059, 641);
            this.splitContainerHorizontalLayer1.SplitterDistance = 157;
            this.splitContainerHorizontalLayer1.TabIndex = 13;
            // 
            // splitContainerHorizontalLayer2
            // 
            this.splitContainerHorizontalLayer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerHorizontalLayer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerHorizontalLayer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainerHorizontalLayer2.Name = "splitContainerHorizontalLayer2";
            this.splitContainerHorizontalLayer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerHorizontalLayer2.Panel1
            // 
            this.splitContainerHorizontalLayer2.Panel1.Controls.Add(this.pnlStatus);
            // 
            // splitContainerHorizontalLayer2.Panel2
            // 
            this.splitContainerHorizontalLayer2.Panel2.Controls.Add(this.gbxMessage);
            this.splitContainerHorizontalLayer2.Size = new System.Drawing.Size(1059, 157);
            this.splitContainerHorizontalLayer2.SplitterDistance = 60;
            this.splitContainerHorizontalLayer2.TabIndex = 14;
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.LightCyan;
            this.pnlStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Controls.Add(this.lblStatusMessage);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlStatus.Location = new System.Drawing.Point(0, 0);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(1059, 60);
            this.pnlStatus.TabIndex = 14;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStatus.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1057, 37);
            this.lblStatus.TabIndex = 28;
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatusMessage
            // 
            this.lblStatusMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblStatusMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatusMessage.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusMessage.Location = new System.Drawing.Point(0, 37);
            this.lblStatusMessage.Name = "lblStatusMessage";
            this.lblStatusMessage.Size = new System.Drawing.Size(1057, 21);
            this.lblStatusMessage.TabIndex = 29;
            this.lblStatusMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbxMessage
            // 
            this.gbxMessage.Controls.Add(this.rtbxMessage);
            this.gbxMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxMessage.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxMessage.Location = new System.Drawing.Point(0, 0);
            this.gbxMessage.Name = "gbxMessage";
            this.gbxMessage.Size = new System.Drawing.Size(1059, 93);
            this.gbxMessage.TabIndex = 10;
            this.gbxMessage.TabStop = false;
            this.gbxMessage.Text = "Message";
            // 
            // gbxSetting
            // 
            this.gbxSetting.Controls.Add(this.cbxInterface);
            this.gbxSetting.Controls.Add(this.lblInterface);
            this.gbxSetting.Controls.Add(this.cbxSocketType);
            this.gbxSetting.Controls.Add(this.lblSocketType);
            this.gbxSetting.Controls.Add(this.lblICSolution);
            this.gbxSetting.Controls.Add(this.tbxICSolution);
            this.gbxSetting.Controls.Add(this.tcCreateIssueData);
            this.gbxSetting.Controls.Add(this.lblToolVersion);
            this.gbxSetting.Controls.Add(this.tbxToolVersion);
            this.gbxSetting.Controls.Add(this.cbxLoginType);
            this.gbxSetting.Controls.Add(this.lblLoginType);
            this.gbxSetting.Controls.Add(this.lblPassword);
            this.gbxSetting.Controls.Add(this.tbxPassword);
            this.gbxSetting.Controls.Add(this.lblUserName);
            this.gbxSetting.Controls.Add(this.tbxUserName);
            this.gbxSetting.Controls.Add(this.ckbxHide);
            this.gbxSetting.Controls.Add(this.lblProjectName);
            this.gbxSetting.Controls.Add(this.tbxProjectName);
            this.gbxSetting.Controls.Add(this.lblAssignedTo);
            this.gbxSetting.Controls.Add(this.cbxAssignTo);
            this.gbxSetting.Controls.Add(this.btnSubmit);
            this.gbxSetting.Controls.Add(this.lblAPIKey);
            this.gbxSetting.Controls.Add(this.tbxAPIKey);
            this.gbxSetting.Controls.Add(this.lblSubject);
            this.gbxSetting.Controls.Add(this.tbxSubject);
            this.gbxSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxSetting.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxSetting.Location = new System.Drawing.Point(0, 0);
            this.gbxSetting.Name = "gbxSetting";
            this.gbxSetting.Size = new System.Drawing.Size(1059, 480);
            this.gbxSetting.TabIndex = 9;
            this.gbxSetting.TabStop = false;
            this.gbxSetting.Text = "Setting";
            // 
            // cbxInterface
            // 
            this.cbxInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxInterface.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxInterface.FormattingEnabled = true;
            this.cbxInterface.Location = new System.Drawing.Point(375, 175);
            this.cbxInterface.Name = "cbxInterface";
            this.cbxInterface.Size = new System.Drawing.Size(182, 23);
            this.cbxInterface.TabIndex = 19;
            // 
            // lblInterface
            // 
            this.lblInterface.AutoSize = true;
            this.lblInterface.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInterface.Location = new System.Drawing.Point(306, 180);
            this.lblInterface.Name = "lblInterface";
            this.lblInterface.Size = new System.Drawing.Size(63, 15);
            this.lblInterface.TabIndex = 18;
            this.lblInterface.Text = "Interface : ";
            // 
            // cbxSocketType
            // 
            this.cbxSocketType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSocketType.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxSocketType.FormattingEnabled = true;
            this.cbxSocketType.Location = new System.Drawing.Point(92, 175);
            this.cbxSocketType.Name = "cbxSocketType";
            this.cbxSocketType.Size = new System.Drawing.Size(182, 23);
            this.cbxSocketType.TabIndex = 17;
            // 
            // lblSocketType
            // 
            this.lblSocketType.AutoSize = true;
            this.lblSocketType.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSocketType.Location = new System.Drawing.Point(8, 180);
            this.lblSocketType.Name = "lblSocketType";
            this.lblSocketType.Size = new System.Drawing.Size(82, 15);
            this.lblSocketType.TabIndex = 16;
            this.lblSocketType.Text = "Socket Type : ";
            // 
            // lblICSolution
            // 
            this.lblICSolution.AutoSize = true;
            this.lblICSolution.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblICSolution.Location = new System.Drawing.Point(577, 180);
            this.lblICSolution.Name = "lblICSolution";
            this.lblICSolution.Size = new System.Drawing.Size(76, 15);
            this.lblICSolution.TabIndex = 20;
            this.lblICSolution.Text = "IC Solution : ";
            // 
            // tbxICSolution
            // 
            this.tbxICSolution.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxICSolution.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxICSolution.Location = new System.Drawing.Point(659, 176);
            this.tbxICSolution.Name = "tbxICSolution";
            this.tbxICSolution.Size = new System.Drawing.Size(392, 22);
            this.tbxICSolution.TabIndex = 21;
            // 
            // tcCreateIssueData
            // 
            this.tcCreateIssueData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcCreateIssueData.Controls.Add(this.tpDescription);
            this.tcCreateIssueData.Controls.Add(this.tpUploadFile);
            this.tcCreateIssueData.Location = new System.Drawing.Point(6, 200);
            this.tcCreateIssueData.Name = "tcCreateIssueData";
            this.tcCreateIssueData.SelectedIndex = 0;
            this.tcCreateIssueData.Size = new System.Drawing.Size(1047, 247);
            this.tcCreateIssueData.TabIndex = 25;
            // 
            // tpDescription
            // 
            this.tpDescription.Controls.Add(this.lblDescription);
            this.tpDescription.Controls.Add(this.rtbxDescription);
            this.tpDescription.Location = new System.Drawing.Point(4, 24);
            this.tpDescription.Name = "tpDescription";
            this.tpDescription.Padding = new System.Windows.Forms.Padding(3);
            this.tpDescription.Size = new System.Drawing.Size(1039, 219);
            this.tpDescription.TabIndex = 0;
            this.tpDescription.Text = "Description";
            this.tpDescription.UseVisualStyleBackColor = true;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(1, 5);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(78, 15);
            this.lblDescription.TabIndex = 22;
            this.lblDescription.Text = "Description : ";
            // 
            // rtbxDescription
            // 
            this.rtbxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbxDescription.Location = new System.Drawing.Point(3, 26);
            this.rtbxDescription.Name = "rtbxDescription";
            this.rtbxDescription.Size = new System.Drawing.Size(1033, 190);
            this.rtbxDescription.TabIndex = 23;
            this.rtbxDescription.Text = "";
            this.rtbxDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbxDescription_KeyDown);
            // 
            // tpUploadFile
            // 
            this.tpUploadFile.Controls.Add(this.dgvUploadFile);
            this.tpUploadFile.Controls.Add(this.btnSelectUploadFile);
            this.tpUploadFile.Controls.Add(this.btnDeleteUploadFile);
            this.tpUploadFile.Location = new System.Drawing.Point(4, 24);
            this.tpUploadFile.Name = "tpUploadFile";
            this.tpUploadFile.Padding = new System.Windows.Forms.Padding(3);
            this.tpUploadFile.Size = new System.Drawing.Size(1039, 219);
            this.tpUploadFile.TabIndex = 1;
            this.tpUploadFile.Text = "Upload File";
            this.tpUploadFile.UseVisualStyleBackColor = true;
            // 
            // dgvUploadFile
            // 
            this.dgvUploadFile.AllowUserToAddRows = false;
            this.dgvUploadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUploadFile.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvUploadFile.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvUploadFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUploadFile.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnItem,
            this.ColumnFilePath,
            this.ColumnDescription});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvUploadFile.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvUploadFile.Location = new System.Drawing.Point(4, 34);
            this.dgvUploadFile.Name = "dgvUploadFile";
            this.dgvUploadFile.RowTemplate.Height = 24;
            this.dgvUploadFile.Size = new System.Drawing.Size(1026, 207);
            this.dgvUploadFile.TabIndex = 26;
            this.dgvUploadFile.RowHeadersWidthChanged += new System.EventHandler(this.dgvUploadFile_RowHeadersWidthChanged);
            this.dgvUploadFile.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUploadFile_CellContentClick);
            this.dgvUploadFile.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvUploadFile_CellPainting);
            this.dgvUploadFile.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.dgvUploadFile_CellToolTipTextNeeded);
            this.dgvUploadFile.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvUploadFile_Scroll);
            this.dgvUploadFile.SizeChanged += new System.EventHandler(this.dgvUploadFile_SizeChanged);
            // 
            // ColumnItem
            // 
            this.ColumnItem.HeaderText = "Item";
            this.ColumnItem.Name = "ColumnItem";
            // 
            // ColumnFilePath
            // 
            this.ColumnFilePath.HeaderText = "File Path";
            this.ColumnFilePath.Name = "ColumnFilePath";
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.HeaderText = "Description";
            this.ColumnDescription.Name = "ColumnDescription";
            // 
            // btnSelectUploadFile
            // 
            this.btnSelectUploadFile.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectUploadFile.Location = new System.Drawing.Point(6, 4);
            this.btnSelectUploadFile.Name = "btnSelectUploadFile";
            this.btnSelectUploadFile.Size = new System.Drawing.Size(70, 27);
            this.btnSelectUploadFile.TabIndex = 24;
            this.btnSelectUploadFile.Text = "Select";
            this.btnSelectUploadFile.UseVisualStyleBackColor = true;
            this.btnSelectUploadFile.Click += new System.EventHandler(this.btnSelectUploadFile_Click);
            // 
            // btnDeleteUploadFile
            // 
            this.btnDeleteUploadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteUploadFile.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteUploadFile.Location = new System.Drawing.Point(957, 4);
            this.btnDeleteUploadFile.Name = "btnDeleteUploadFile";
            this.btnDeleteUploadFile.Size = new System.Drawing.Size(70, 27);
            this.btnDeleteUploadFile.TabIndex = 25;
            this.btnDeleteUploadFile.Text = "Delete";
            this.btnDeleteUploadFile.UseVisualStyleBackColor = true;
            this.btnDeleteUploadFile.Click += new System.EventHandler(this.btnDeleteUploadFile_Click);
            // 
            // lblToolVersion
            // 
            this.lblToolVersion.AutoSize = true;
            this.lblToolVersion.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToolVersion.Location = new System.Drawing.Point(8, 153);
            this.lblToolVersion.Name = "lblToolVersion";
            this.lblToolVersion.Size = new System.Drawing.Size(83, 15);
            this.lblToolVersion.TabIndex = 14;
            this.lblToolVersion.Text = "Tool Version : ";
            // 
            // tbxToolVersion
            // 
            this.tbxToolVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxToolVersion.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxToolVersion.Location = new System.Drawing.Point(133, 149);
            this.tbxToolVersion.Name = "tbxToolVersion";
            this.tbxToolVersion.Size = new System.Drawing.Size(918, 22);
            this.tbxToolVersion.TabIndex = 15;
            // 
            // cbxLoginType
            // 
            this.cbxLoginType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLoginType.FormattingEnabled = true;
            this.cbxLoginType.Items.AddRange(new object[] {
            "Use UserName & Password",
            "Use API Key"});
            this.cbxLoginType.Location = new System.Drawing.Point(92, 16);
            this.cbxLoginType.Name = "cbxLoginType";
            this.cbxLoginType.Size = new System.Drawing.Size(182, 23);
            this.cbxLoginType.TabIndex = 1;
            this.cbxLoginType.SelectedIndexChanged += new System.EventHandler(this.cbxLoginType_SelectedIndexChanged);
            // 
            // lblLoginType
            // 
            this.lblLoginType.AutoSize = true;
            this.lblLoginType.Location = new System.Drawing.Point(8, 20);
            this.lblLoginType.Name = "lblLoginType";
            this.lblLoginType.Size = new System.Drawing.Size(77, 15);
            this.lblLoginType.TabIndex = 0;
            this.lblLoginType.Text = "Login Type : ";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.Location = new System.Drawing.Point(358, 45);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(71, 15);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password  : ";
            // 
            // tbxPassword
            // 
            this.tbxPassword.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxPassword.Location = new System.Drawing.Point(436, 42);
            this.tbxPassword.Name = "tbxPassword";
            this.tbxPassword.Size = new System.Drawing.Size(200, 22);
            this.tbxPassword.TabIndex = 5;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.Location = new System.Drawing.Point(8, 45);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(74, 15);
            this.lblUserName.TabIndex = 2;
            this.lblUserName.Text = "UserName  : ";
            // 
            // tbxUserName
            // 
            this.tbxUserName.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxUserName.Location = new System.Drawing.Point(92, 42);
            this.tbxUserName.Name = "tbxUserName";
            this.tbxUserName.Size = new System.Drawing.Size(200, 22);
            this.tbxUserName.TabIndex = 3;
            // 
            // ckbxHide
            // 
            this.ckbxHide.AutoSize = true;
            this.ckbxHide.Checked = true;
            this.ckbxHide.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbxHide.Location = new System.Drawing.Point(642, 44);
            this.ckbxHide.Name = "ckbxHide";
            this.ckbxHide.Size = new System.Drawing.Size(51, 19);
            this.ckbxHide.TabIndex = 31;
            this.ckbxHide.Text = "Hide";
            this.ckbxHide.UseVisualStyleBackColor = true;
            this.ckbxHide.CheckedChanged += new System.EventHandler(this.ckbxHide_CheckedChanged);
            // 
            // lblProjectName
            // 
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProjectName.Location = new System.Drawing.Point(8, 127);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(119, 15);
            this.lblProjectName.TabIndex = 12;
            this.lblProjectName.Text = "Project(SKU) Name : ";
            // 
            // tbxProjectName
            // 
            this.tbxProjectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxProjectName.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxProjectName.Location = new System.Drawing.Point(133, 123);
            this.tbxProjectName.Name = "tbxProjectName";
            this.tbxProjectName.Size = new System.Drawing.Size(918, 22);
            this.tbxProjectName.TabIndex = 13;
            // 
            // frmRedmineTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 641);
            this.Controls.Add(this.splitContainerHorizontalLayer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRedmineTask";
            this.Text = "Redmine Task";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRedmineTask_FormClosing);
            this.splitContainerHorizontalLayer1.Panel1.ResumeLayout(false);
            this.splitContainerHorizontalLayer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontalLayer1)).EndInit();
            this.splitContainerHorizontalLayer1.ResumeLayout(false);
            this.splitContainerHorizontalLayer2.Panel1.ResumeLayout(false);
            this.splitContainerHorizontalLayer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontalLayer2)).EndInit();
            this.splitContainerHorizontalLayer2.ResumeLayout(false);
            this.pnlStatus.ResumeLayout(false);
            this.gbxMessage.ResumeLayout(false);
            this.gbxSetting.ResumeLayout(false);
            this.gbxSetting.PerformLayout();
            this.tcCreateIssueData.ResumeLayout(false);
            this.tpDescription.ResumeLayout(false);
            this.tpDescription.PerformLayout();
            this.tpUploadFile.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUploadFile)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label lblAPIKey;
        private System.Windows.Forms.TextBox tbxAPIKey;
        private System.Windows.Forms.RichTextBox rtbxMessage;
        private System.Windows.Forms.TextBox tbxSubject;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.Label lblAssignedTo;
        private System.Windows.Forms.ComboBox cbxAssignTo;
        private System.Windows.Forms.SplitContainer splitContainerHorizontalLayer1;
        private System.Windows.Forms.SplitContainer splitContainerHorizontalLayer2;
        private System.Windows.Forms.GroupBox gbxMessage;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox gbxSetting;
        private System.Windows.Forms.RichTextBox rtbxDescription;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.TextBox tbxProjectName;
        private System.Windows.Forms.CheckBox ckbxHide;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox tbxPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox tbxUserName;
        private System.Windows.Forms.Label lblLoginType;
        private System.Windows.Forms.ComboBox cbxLoginType;
        private System.Windows.Forms.Label lblToolVersion;
        private System.Windows.Forms.TextBox tbxToolVersion;
        private System.Windows.Forms.Button btnDeleteUploadFile;
        private System.Windows.Forms.Button btnSelectUploadFile;
        private System.Windows.Forms.DataGridView dgvUploadFile;
        private System.Windows.Forms.TabControl tcCreateIssueData;
        private System.Windows.Forms.TabPage tpDescription;
        private System.Windows.Forms.TabPage tpUploadFile;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFilePath;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDescription;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatusMessage;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.ComboBox cbxSocketType;
        private System.Windows.Forms.Label lblSocketType;
        private System.Windows.Forms.Label lblICSolution;
        private System.Windows.Forms.TextBox tbxICSolution;
        private System.Windows.Forms.ComboBox cbxInterface;
        private System.Windows.Forms.Label lblInterface;
    }
}