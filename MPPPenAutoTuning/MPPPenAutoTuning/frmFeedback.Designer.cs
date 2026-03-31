namespace MPPPenAutoTuning
{
    partial class frmFeedback
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFeedback));
            this.lblEmployeeNo = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.tbxEmployeeNo = new System.Windows.Forms.TextBox();
            this.lblState = new System.Windows.Forms.Label();
            this.lblUploadFile = new System.Windows.Forms.Label();
            this.btnSelectUploadFile = new System.Windows.Forms.Button();
            this.cklbxUploadFile = new System.Windows.Forms.CheckedListBox();
            this.btnDeleteUploadFile = new System.Windows.Forms.Button();
            this.lblDescription = new System.Windows.Forms.Label();
            this.rtbxDescription = new System.Windows.Forms.RichTextBox();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemParameter = new System.Windows.Forms.ToolStripMenuItem();
            this.lblFeedbackType = new System.Windows.Forms.Label();
            this.cbxFeedbackType = new System.Windows.Forms.ComboBox();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblEmployeeNo
            // 
            this.lblEmployeeNo.AutoSize = true;
            this.lblEmployeeNo.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmployeeNo.Location = new System.Drawing.Point(14, 107);
            this.lblEmployeeNo.Name = "lblEmployeeNo";
            this.lblEmployeeNo.Size = new System.Drawing.Size(112, 21);
            this.lblEmployeeNo.TabIndex = 1;
            this.lblEmployeeNo.Text = "Employee No";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSubmit.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Location = new System.Drawing.Point(171, 615);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(135, 42);
            this.btnSubmit.TabIndex = 11;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // tbxEmployeeNo
            // 
            this.tbxEmployeeNo.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxEmployeeNo.Location = new System.Drawing.Point(152, 103);
            this.tbxEmployeeNo.Name = "tbxEmployeeNo";
            this.tbxEmployeeNo.Size = new System.Drawing.Size(121, 29);
            this.tbxEmployeeNo.TabIndex = 2;
            this.tbxEmployeeNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxEmployeeNo_KeyPress);
            // 
            // lblState
            // 
            this.lblState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblState.BackColor = System.Drawing.Color.LightCyan;
            this.lblState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblState.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblState.Location = new System.Drawing.Point(12, 28);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(464, 71);
            this.lblState.TabIndex = 0;
            this.lblState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUploadFile
            // 
            this.lblUploadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUploadFile.AutoSize = true;
            this.lblUploadFile.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUploadFile.Location = new System.Drawing.Point(14, 453);
            this.lblUploadFile.Name = "lblUploadFile";
            this.lblUploadFile.Size = new System.Drawing.Size(97, 21);
            this.lblUploadFile.TabIndex = 7;
            this.lblUploadFile.Text = "Upload File";
            // 
            // btnSelectUploadFile
            // 
            this.btnSelectUploadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectUploadFile.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectUploadFile.Location = new System.Drawing.Point(131, 448);
            this.btnSelectUploadFile.Name = "btnSelectUploadFile";
            this.btnSelectUploadFile.Size = new System.Drawing.Size(77, 30);
            this.btnSelectUploadFile.TabIndex = 8;
            this.btnSelectUploadFile.Text = "Select";
            this.btnSelectUploadFile.UseVisualStyleBackColor = true;
            this.btnSelectUploadFile.Click += new System.EventHandler(this.btnSelectUploadFile_Click);
            // 
            // cklbxUploadFile
            // 
            this.cklbxUploadFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cklbxUploadFile.CheckOnClick = true;
            this.cklbxUploadFile.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cklbxUploadFile.FormattingEnabled = true;
            this.cklbxUploadFile.HorizontalScrollbar = true;
            this.cklbxUploadFile.Location = new System.Drawing.Point(13, 481);
            this.cklbxUploadFile.Name = "cklbxUploadFile";
            this.cklbxUploadFile.Size = new System.Drawing.Size(463, 130);
            this.cklbxUploadFile.TabIndex = 10;
            // 
            // btnDeleteUploadFile
            // 
            this.btnDeleteUploadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteUploadFile.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteUploadFile.Location = new System.Drawing.Point(399, 448);
            this.btnDeleteUploadFile.Name = "btnDeleteUploadFile";
            this.btnDeleteUploadFile.Size = new System.Drawing.Size(77, 30);
            this.btnDeleteUploadFile.TabIndex = 9;
            this.btnDeleteUploadFile.Text = "Delete";
            this.btnDeleteUploadFile.UseVisualStyleBackColor = true;
            this.btnDeleteUploadFile.Click += new System.EventHandler(this.btnDeleteUploadFile_Click);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(14, 170);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(96, 21);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description";
            // 
            // rtbxDescription
            // 
            this.rtbxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbxDescription.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxDescription.Location = new System.Drawing.Point(12, 194);
            this.rtbxDescription.Name = "rtbxDescription";
            this.rtbxDescription.Size = new System.Drawing.Size(464, 245);
            this.rtbxDescription.TabIndex = 6;
            this.rtbxDescription.Text = "";
            this.rtbxDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbxDescription_KeyDown);
            // 
            // menuStripMain
            // 
            this.menuStripMain.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSetting});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(484, 27);
            this.menuStripMain.TabIndex = 10;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // toolStripMenuItemSetting
            // 
            this.toolStripMenuItemSetting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemParameter});
            this.toolStripMenuItemSetting.Name = "toolStripMenuItemSetting";
            this.toolStripMenuItemSetting.Size = new System.Drawing.Size(62, 23);
            this.toolStripMenuItemSetting.Text = "Setting";
            // 
            // toolStripMenuItemParameter
            // 
            this.toolStripMenuItemParameter.Name = "toolStripMenuItemParameter";
            this.toolStripMenuItemParameter.Size = new System.Drawing.Size(140, 24);
            this.toolStripMenuItemParameter.Text = "Parameter";
            this.toolStripMenuItemParameter.Click += new System.EventHandler(this.toolStripMenuItemParameter_Click);
            // 
            // lblFeedbackType
            // 
            this.lblFeedbackType.AutoSize = true;
            this.lblFeedbackType.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFeedbackType.Location = new System.Drawing.Point(14, 141);
            this.lblFeedbackType.Name = "lblFeedbackType";
            this.lblFeedbackType.Size = new System.Drawing.Size(126, 21);
            this.lblFeedbackType.TabIndex = 3;
            this.lblFeedbackType.Text = "Feedback Type";
            // 
            // cbxFeedbackType
            // 
            this.cbxFeedbackType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFeedbackType.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxFeedbackType.FormattingEnabled = true;
            this.cbxFeedbackType.ItemHeight = 21;
            this.cbxFeedbackType.Items.AddRange(new object[] {
            "Suggest",
            "Issue",
            "Other"});
            this.cbxFeedbackType.Location = new System.Drawing.Point(152, 139);
            this.cbxFeedbackType.Name = "cbxFeedbackType";
            this.cbxFeedbackType.Size = new System.Drawing.Size(121, 29);
            this.cbxFeedbackType.TabIndex = 12;
            // 
            // frmFeedback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 661);
            this.Controls.Add(this.cbxFeedbackType);
            this.Controls.Add(this.lblFeedbackType);
            this.Controls.Add(this.rtbxDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.btnDeleteUploadFile);
            this.Controls.Add(this.cklbxUploadFile);
            this.Controls.Add(this.btnSelectUploadFile);
            this.Controls.Add(this.lblUploadFile);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.tbxEmployeeNo);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.lblEmployeeNo);
            this.Controls.Add(this.menuStripMain);
            this.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmFeedback";
            this.Text = "Feedback";
            this.Load += new System.EventHandler(this.frmFeedback_Load);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEmployeeNo;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.TextBox tbxEmployeeNo;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Label lblUploadFile;
        private System.Windows.Forms.Button btnSelectUploadFile;
        private System.Windows.Forms.CheckedListBox cklbxUploadFile;
        private System.Windows.Forms.Button btnDeleteUploadFile;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.RichTextBox rtbxDescription;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetting;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemParameter;
        private System.Windows.Forms.Label lblFeedbackType;
        private System.Windows.Forms.ComboBox cbxFeedbackType;
    }
}