namespace MPPPenAutoTuning
{
    partial class frmSummarizeLogData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSummarizeLogData));
            this.lblDataPath = new System.Windows.Forms.Label();
            this.tbxDataPath = new System.Windows.Forms.TextBox();
            this.btnSelectDataPath = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.gbxMessage = new System.Windows.Forms.GroupBox();
            this.rtbxMessage = new System.Windows.Forms.RichTextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.tbxOutputPath = new System.Windows.Forms.TextBox();
            this.btnSelectOutputPath = new System.Windows.Forms.Button();
            this.ckbxOutputFolderName = new System.Windows.Forms.CheckBox();
            this.tbxOutputFolderName = new System.Windows.Forms.TextBox();
            this.ckbxIncludeMaxValueInChart = new System.Windows.Forms.CheckBox();
            this.gbxMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDataPath
            // 
            this.lblDataPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDataPath.AutoSize = true;
            this.lblDataPath.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDataPath.Location = new System.Drawing.Point(27, 377);
            this.lblDataPath.Name = "lblDataPath";
            this.lblDataPath.Size = new System.Drawing.Size(82, 21);
            this.lblDataPath.TabIndex = 0;
            this.lblDataPath.Text = "Data Path";
            // 
            // tbxDataPath
            // 
            this.tbxDataPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxDataPath.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxDataPath.Location = new System.Drawing.Point(122, 373);
            this.tbxDataPath.Name = "tbxDataPath";
            this.tbxDataPath.Size = new System.Drawing.Size(563, 29);
            this.tbxDataPath.TabIndex = 2;
            this.tbxDataPath.TextChanged += new System.EventHandler(this.tbxDataPath_TextChanged);
            // 
            // btnSelectDataPath
            // 
            this.btnSelectDataPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectDataPath.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectDataPath.Location = new System.Drawing.Point(691, 367);
            this.btnSelectDataPath.Name = "btnSelectDataPath";
            this.btnSelectDataPath.Size = new System.Drawing.Size(88, 41);
            this.btnSelectDataPath.TabIndex = 1;
            this.btnSelectDataPath.Text = "Select";
            this.btnSelectDataPath.UseVisualStyleBackColor = true;
            this.btnSelectDataPath.Click += new System.EventHandler(this.btnSelectDataPath_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStart.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(349, 530);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(107, 45);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // gbxMessage
            // 
            this.gbxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxMessage.Controls.Add(this.rtbxMessage);
            this.gbxMessage.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxMessage.Location = new System.Drawing.Point(4, 104);
            this.gbxMessage.Name = "gbxMessage";
            this.gbxMessage.Size = new System.Drawing.Size(775, 261);
            this.gbxMessage.TabIndex = 9;
            this.gbxMessage.TabStop = false;
            this.gbxMessage.Text = "Message";
            // 
            // rtbxMessage
            // 
            this.rtbxMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxMessage.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxMessage.Location = new System.Drawing.Point(3, 22);
            this.rtbxMessage.Name = "rtbxMessage";
            this.rtbxMessage.Size = new System.Drawing.Size(769, 236);
            this.rtbxMessage.TabIndex = 8;
            this.rtbxMessage.Text = "";
            this.rtbxMessage.TextChanged += new System.EventHandler(this.rtbxMessage_TextChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.BackColor = System.Drawing.Color.PaleTurquoise;
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(5, 2);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(774, 57);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.Location = new System.Drawing.Point(5, 66);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(774, 31);
            this.pbProgress.TabIndex = 7;
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOutputPath.AutoSize = true;
            this.lblOutputPath.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutputPath.Location = new System.Drawing.Point(9, 423);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(100, 21);
            this.lblOutputPath.TabIndex = 7;
            this.lblOutputPath.Text = "Output Path";
            // 
            // tbxOutputPath
            // 
            this.tbxOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxOutputPath.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxOutputPath.Location = new System.Drawing.Point(122, 418);
            this.tbxOutputPath.Name = "tbxOutputPath";
            this.tbxOutputPath.Size = new System.Drawing.Size(563, 29);
            this.tbxOutputPath.TabIndex = 4;
            this.tbxOutputPath.TextChanged += new System.EventHandler(this.tbxOutputPath_TextChanged);
            // 
            // btnSelectOutputPath
            // 
            this.btnSelectOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectOutputPath.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectOutputPath.Location = new System.Drawing.Point(691, 413);
            this.btnSelectOutputPath.Name = "btnSelectOutputPath";
            this.btnSelectOutputPath.Size = new System.Drawing.Size(88, 41);
            this.btnSelectOutputPath.TabIndex = 3;
            this.btnSelectOutputPath.Text = "Select";
            this.btnSelectOutputPath.UseVisualStyleBackColor = true;
            this.btnSelectOutputPath.Click += new System.EventHandler(this.btnSelectOutputPath_Click);
            // 
            // ckbxOutputFolderName
            // 
            this.ckbxOutputFolderName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbxOutputFolderName.AutoSize = true;
            this.ckbxOutputFolderName.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckbxOutputFolderName.Location = new System.Drawing.Point(334, 465);
            this.ckbxOutputFolderName.Name = "ckbxOutputFolderName";
            this.ckbxOutputFolderName.Size = new System.Drawing.Size(183, 25);
            this.ckbxOutputFolderName.TabIndex = 10;
            this.ckbxOutputFolderName.Text = "Output Folder Name";
            this.ckbxOutputFolderName.UseVisualStyleBackColor = true;
            this.ckbxOutputFolderName.CheckedChanged += new System.EventHandler(this.ckbxOutputFolderName_CheckedChanged);
            // 
            // tbxOutputFolderName
            // 
            this.tbxOutputFolderName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxOutputFolderName.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxOutputFolderName.Location = new System.Drawing.Point(525, 462);
            this.tbxOutputFolderName.Name = "tbxOutputFolderName";
            this.tbxOutputFolderName.Size = new System.Drawing.Size(252, 29);
            this.tbxOutputFolderName.TabIndex = 11;
            // 
            // ckbxIncludeMaxValueInChart
            // 
            this.ckbxIncludeMaxValueInChart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbxIncludeMaxValueInChart.AutoSize = true;
            this.ckbxIncludeMaxValueInChart.Checked = true;
            this.ckbxIncludeMaxValueInChart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbxIncludeMaxValueInChart.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckbxIncludeMaxValueInChart.Location = new System.Drawing.Point(334, 500);
            this.ckbxIncludeMaxValueInChart.Name = "ckbxIncludeMaxValueInChart";
            this.ckbxIncludeMaxValueInChart.Size = new System.Drawing.Size(231, 25);
            this.ckbxIncludeMaxValueInChart.TabIndex = 12;
            this.ckbxIncludeMaxValueInChart.Text = "Include Max Value in Chart";
            this.ckbxIncludeMaxValueInChart.UseVisualStyleBackColor = true;
            // 
            // frmSummarizeLogData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 581);
            this.Controls.Add(this.ckbxIncludeMaxValueInChart);
            this.Controls.Add(this.tbxOutputFolderName);
            this.Controls.Add(this.ckbxOutputFolderName);
            this.Controls.Add(this.btnSelectOutputPath);
            this.Controls.Add(this.tbxOutputPath);
            this.Controls.Add(this.lblOutputPath);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.gbxMessage);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnSelectDataPath);
            this.Controls.Add(this.tbxDataPath);
            this.Controls.Add(this.lblDataPath);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSummarizeLogData";
            this.Text = "Summarize Log Data(Multi-SKU)";
            this.Load += new System.EventHandler(this.frmSummarizeLogData_Load);
            this.gbxMessage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDataPath;
        private System.Windows.Forms.TextBox tbxDataPath;
        private System.Windows.Forms.Button btnSelectDataPath;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox gbxMessage;
        private System.Windows.Forms.RichTextBox rtbxMessage;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.TextBox tbxOutputPath;
        private System.Windows.Forms.Button btnSelectOutputPath;
        private System.Windows.Forms.CheckBox ckbxOutputFolderName;
        private System.Windows.Forms.TextBox tbxOutputFolderName;
        private System.Windows.Forms.CheckBox ckbxIncludeMaxValueInChart;
    }
}