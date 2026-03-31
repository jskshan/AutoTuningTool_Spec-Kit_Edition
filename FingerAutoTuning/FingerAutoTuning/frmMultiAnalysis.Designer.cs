namespace FingerAutoTuning
{
    partial class frmMultiAnalysis
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMultiAnalysis));
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerMinor1 = new System.Windows.Forms.SplitContainer();
            this.clbxFile = new System.Windows.Forms.CheckedListBox();
            this.lblStep = new System.Windows.Forms.Label();
            this.cbxStep = new System.Windows.Forms.ComboBox();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnRemoveFile = new System.Windows.Forms.Button();
            this.splitContainerMinor2 = new System.Windows.Forms.SplitContainer();
            this.lblData = new System.Windows.Forms.Label();
            this.cbxData = new System.Windows.Forms.ComboBox();
            this.pnlData = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMinor1)).BeginInit();
            this.splitContainerMinor1.Panel1.SuspendLayout();
            this.splitContainerMinor1.Panel2.SuspendLayout();
            this.splitContainerMinor1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMinor2)).BeginInit();
            this.splitContainerMinor2.Panel1.SuspendLayout();
            this.splitContainerMinor2.Panel2.SuspendLayout();
            this.splitContainerMinor2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerMinor1);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerMinor2);
            this.splitContainerMain.Size = new System.Drawing.Size(1014, 522);
            this.splitContainerMain.SplitterDistance = 313;
            this.splitContainerMain.TabIndex = 0;
            this.splitContainerMain.Resize += new System.EventHandler(this.splitContainerMain_Resize);
            // 
            // splitContainerMinor1
            // 
            this.splitContainerMinor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMinor1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMinor1.Name = "splitContainerMinor1";
            this.splitContainerMinor1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMinor1.Panel1
            // 
            this.splitContainerMinor1.Panel1.Controls.Add(this.clbxFile);
            this.splitContainerMinor1.Panel1.Controls.Add(this.lblStep);
            this.splitContainerMinor1.Panel1.Controls.Add(this.cbxStep);
            // 
            // splitContainerMinor1.Panel2
            // 
            this.splitContainerMinor1.Panel2.Controls.Add(this.btnAddFile);
            this.splitContainerMinor1.Panel2.Controls.Add(this.btnStart);
            this.splitContainerMinor1.Panel2.Controls.Add(this.btnSelectAll);
            this.splitContainerMinor1.Panel2.Controls.Add(this.btnRemoveFile);
            this.splitContainerMinor1.Size = new System.Drawing.Size(313, 522);
            this.splitContainerMinor1.SplitterDistance = 431;
            this.splitContainerMinor1.TabIndex = 8;
            this.splitContainerMinor1.Resize += new System.EventHandler(this.splitContainerMinor1_Resize);
            // 
            // clbxFile
            // 
            this.clbxFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.clbxFile.CheckOnClick = true;
            this.clbxFile.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clbxFile.FormattingEnabled = true;
            this.clbxFile.HorizontalScrollbar = true;
            this.clbxFile.Location = new System.Drawing.Point(3, 48);
            this.clbxFile.Name = "clbxFile";
            this.clbxFile.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.clbxFile.Size = new System.Drawing.Size(310, 344);
            this.clbxFile.TabIndex = 6;
            this.clbxFile.Resize += new System.EventHandler(this.clbxFile_Resize);
            // 
            // lblStep
            // 
            this.lblStep.AutoSize = true;
            this.lblStep.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStep.Location = new System.Drawing.Point(4, 12);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(37, 19);
            this.lblStep.TabIndex = 1;
            this.lblStep.Text = "Step";
            // 
            // cbxStep
            // 
            this.cbxStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxStep.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxStep.FormattingEnabled = true;
            this.cbxStep.Location = new System.Drawing.Point(42, 8);
            this.cbxStep.Name = "cbxStep";
            this.cbxStep.Size = new System.Drawing.Size(184, 27);
            this.cbxStep.TabIndex = 0;
            this.cbxStep.SelectedIndexChanged += new System.EventHandler(this.cbxStep_SelectedIndexChanged);
            // 
            // btnAddFile
            // 
            this.btnAddFile.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddFile.Location = new System.Drawing.Point(3, 6);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(75, 28);
            this.btnAddFile.TabIndex = 3;
            this.btnAddFile.Text = "Add";
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(71, 39);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(92, 40);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectAll.Location = new System.Drawing.Point(80, 6);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 28);
            this.btnSelectAll.TabIndex = 7;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnRemoveFile
            // 
            this.btnRemoveFile.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveFile.Location = new System.Drawing.Point(156, 6);
            this.btnRemoveFile.Name = "btnRemoveFile";
            this.btnRemoveFile.Size = new System.Drawing.Size(75, 28);
            this.btnRemoveFile.TabIndex = 4;
            this.btnRemoveFile.Text = "Remove";
            this.btnRemoveFile.UseVisualStyleBackColor = true;
            this.btnRemoveFile.Click += new System.EventHandler(this.btnRemoveFile_Click);
            // 
            // splitContainerMinor2
            // 
            this.splitContainerMinor2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMinor2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMinor2.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMinor2.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainerMinor2.Name = "splitContainerMinor2";
            this.splitContainerMinor2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMinor2.Panel1
            // 
            this.splitContainerMinor2.Panel1.Controls.Add(this.lblData);
            this.splitContainerMinor2.Panel1.Controls.Add(this.cbxData);
            // 
            // splitContainerMinor2.Panel2
            // 
            this.splitContainerMinor2.Panel2.Controls.Add(this.pnlData);
            this.splitContainerMinor2.Size = new System.Drawing.Size(697, 522);
            this.splitContainerMinor2.SplitterDistance = 46;
            this.splitContainerMinor2.SplitterWidth = 3;
            this.splitContainerMinor2.TabIndex = 8;
            // 
            // lblData
            // 
            this.lblData.AutoSize = true;
            this.lblData.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData.Location = new System.Drawing.Point(2, 11);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(38, 19);
            this.lblData.TabIndex = 6;
            this.lblData.Text = "Data";
            // 
            // cbxData
            // 
            this.cbxData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxData.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxData.FormattingEnabled = true;
            this.cbxData.Location = new System.Drawing.Point(46, 8);
            this.cbxData.Name = "cbxData";
            this.cbxData.Size = new System.Drawing.Size(121, 27);
            this.cbxData.TabIndex = 0;
            this.cbxData.SelectedIndexChanged += new System.EventHandler(this.cbxData_SelectedIndexChanged);
            // 
            // pnlData
            // 
            this.pnlData.BackColor = System.Drawing.SystemColors.Control;
            this.pnlData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlData.Location = new System.Drawing.Point(0, 0);
            this.pnlData.Name = "pnlData";
            this.pnlData.Size = new System.Drawing.Size(697, 473);
            this.pnlData.TabIndex = 7;
            // 
            // frmMultiAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 522);
            this.Controls.Add(this.splitContainerMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMultiAnalysis";
            this.Text = "frmMultiAnalysis";
            this.Load += new System.EventHandler(this.frmMultiAnalysis_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerMinor1.Panel1.ResumeLayout(false);
            this.splitContainerMinor1.Panel1.PerformLayout();
            this.splitContainerMinor1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMinor1)).EndInit();
            this.splitContainerMinor1.ResumeLayout(false);
            this.splitContainerMinor2.Panel1.ResumeLayout(false);
            this.splitContainerMinor2.Panel1.PerformLayout();
            this.splitContainerMinor2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMinor2)).EndInit();
            this.splitContainerMinor2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnRemoveFile;
        private System.Windows.Forms.Button btnAddFile;
        private System.Windows.Forms.Label lblStep;
        private System.Windows.Forms.ComboBox cbxStep;
        private System.Windows.Forms.Panel pnlData;
        private System.Windows.Forms.Label lblData;
        private System.Windows.Forms.ComboBox cbxData;
        private System.Windows.Forms.CheckedListBox clbxFile;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.SplitContainer splitContainerMinor1;
        private System.Windows.Forms.SplitContainer splitContainerMinor2;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}