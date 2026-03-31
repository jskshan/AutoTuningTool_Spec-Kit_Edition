namespace FingerAutoTuning
{
    partial class ctrlMAChart
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

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.gbxSetting = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.lblToken1 = new System.Windows.Forms.Label();
            this.lblToken2 = new System.Windows.Forms.Label();
            this.tbxMaxValue = new System.Windows.Forms.TextBox();
            this.tbxMaxFrequency = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.tbxMinValue = new System.Windows.Forms.TextBox();
            this.lblFrequency = new System.Windows.Forms.Label();
            this.tbxMinFrequency = new System.Windows.Forms.TextBox();
            this.lblChartHeight = new System.Windows.Forms.Label();
            this.tbxChartHeight = new System.Windows.Forms.TextBox();
            this.lblChartWidth = new System.Windows.Forms.Label();
            this.tbxChartWidth = new System.Windows.Forms.TextBox();
            this.splitContainerMajor = new System.Windows.Forms.SplitContainer();
            this.pbxChart = new System.Windows.Forms.PictureBox();
            this.gbxSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMajor)).BeginInit();
            this.splitContainerMajor.Panel1.SuspendLayout();
            this.splitContainerMajor.Panel2.SuspendLayout();
            this.splitContainerMajor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxChart)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxSetting
            // 
            this.gbxSetting.Controls.Add(this.btnSave);
            this.gbxSetting.Controls.Add(this.btnGenerate);
            this.gbxSetting.Controls.Add(this.lblToken1);
            this.gbxSetting.Controls.Add(this.lblToken2);
            this.gbxSetting.Controls.Add(this.tbxMaxValue);
            this.gbxSetting.Controls.Add(this.tbxMaxFrequency);
            this.gbxSetting.Controls.Add(this.lblValue);
            this.gbxSetting.Controls.Add(this.tbxMinValue);
            this.gbxSetting.Controls.Add(this.lblFrequency);
            this.gbxSetting.Controls.Add(this.tbxMinFrequency);
            this.gbxSetting.Controls.Add(this.lblChartHeight);
            this.gbxSetting.Controls.Add(this.tbxChartHeight);
            this.gbxSetting.Controls.Add(this.lblChartWidth);
            this.gbxSetting.Controls.Add(this.tbxChartWidth);
            this.gbxSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxSetting.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxSetting.Location = new System.Drawing.Point(0, 0);
            this.gbxSetting.Margin = new System.Windows.Forms.Padding(2);
            this.gbxSetting.Name = "gbxSetting";
            this.gbxSetting.Padding = new System.Windows.Forms.Padding(2);
            this.gbxSetting.Size = new System.Drawing.Size(746, 108);
            this.gbxSetting.TabIndex = 1;
            this.gbxSetting.TabStop = false;
            this.gbxSetting.Text = "Setting";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(656, 57);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(77, 29);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(656, 21);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(2);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(77, 29);
            this.btnGenerate.TabIndex = 13;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // lblToken1
            // 
            this.lblToken1.AutoSize = true;
            this.lblToken1.Location = new System.Drawing.Point(453, 26);
            this.lblToken1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblToken1.Name = "lblToken1";
            this.lblToken1.Size = new System.Drawing.Size(18, 19);
            this.lblToken1.TabIndex = 12;
            this.lblToken1.Text = "~";
            // 
            // lblToken2
            // 
            this.lblToken2.AutoSize = true;
            this.lblToken2.Location = new System.Drawing.Point(452, 64);
            this.lblToken2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblToken2.Name = "lblToken2";
            this.lblToken2.Size = new System.Drawing.Size(18, 19);
            this.lblToken2.TabIndex = 11;
            this.lblToken2.Text = "~";
            // 
            // tbxMaxValue
            // 
            this.tbxMaxValue.Location = new System.Drawing.Point(472, 62);
            this.tbxMaxValue.Margin = new System.Windows.Forms.Padding(2);
            this.tbxMaxValue.Name = "tbxMaxValue";
            this.tbxMaxValue.Size = new System.Drawing.Size(85, 26);
            this.tbxMaxValue.TabIndex = 10;
            // 
            // tbxMaxFrequency
            // 
            this.tbxMaxFrequency.Location = new System.Drawing.Point(472, 22);
            this.tbxMaxFrequency.Margin = new System.Windows.Forms.Padding(2);
            this.tbxMaxFrequency.Name = "tbxMaxFrequency";
            this.tbxMaxFrequency.Size = new System.Drawing.Size(85, 26);
            this.tbxMaxFrequency.TabIndex = 8;
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(314, 64);
            this.lblValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(42, 19);
            this.lblValue.TabIndex = 7;
            this.lblValue.Text = "Value";
            // 
            // tbxMinValue
            // 
            this.tbxMinValue.Location = new System.Drawing.Point(365, 62);
            this.tbxMinValue.Margin = new System.Windows.Forms.Padding(2);
            this.tbxMinValue.Name = "tbxMinValue";
            this.tbxMinValue.Size = new System.Drawing.Size(85, 26);
            this.tbxMinValue.TabIndex = 6;
            // 
            // lblFrequency
            // 
            this.lblFrequency.AutoSize = true;
            this.lblFrequency.Location = new System.Drawing.Point(251, 26);
            this.lblFrequency.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFrequency.Name = "lblFrequency";
            this.lblFrequency.Size = new System.Drawing.Size(112, 19);
            this.lblFrequency.TabIndex = 5;
            this.lblFrequency.Text = "Frequency(KHz)";
            // 
            // tbxMinFrequency
            // 
            this.tbxMinFrequency.Location = new System.Drawing.Point(365, 22);
            this.tbxMinFrequency.Margin = new System.Windows.Forms.Padding(2);
            this.tbxMinFrequency.Name = "tbxMinFrequency";
            this.tbxMinFrequency.Size = new System.Drawing.Size(85, 26);
            this.tbxMinFrequency.TabIndex = 4;
            // 
            // lblChartHeight
            // 
            this.lblChartHeight.AutoSize = true;
            this.lblChartHeight.Location = new System.Drawing.Point(7, 64);
            this.lblChartHeight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblChartHeight.Name = "lblChartHeight";
            this.lblChartHeight.Size = new System.Drawing.Size(86, 19);
            this.lblChartHeight.TabIndex = 3;
            this.lblChartHeight.Text = "Chart Height";
            // 
            // tbxChartHeight
            // 
            this.tbxChartHeight.Location = new System.Drawing.Point(94, 62);
            this.tbxChartHeight.Margin = new System.Windows.Forms.Padding(2);
            this.tbxChartHeight.Name = "tbxChartHeight";
            this.tbxChartHeight.Size = new System.Drawing.Size(85, 26);
            this.tbxChartHeight.TabIndex = 2;
            // 
            // lblChartWidth
            // 
            this.lblChartWidth.AutoSize = true;
            this.lblChartWidth.Location = new System.Drawing.Point(7, 26);
            this.lblChartWidth.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblChartWidth.Name = "lblChartWidth";
            this.lblChartWidth.Size = new System.Drawing.Size(83, 19);
            this.lblChartWidth.TabIndex = 1;
            this.lblChartWidth.Text = "Chart Width";
            // 
            // tbxChartWidth
            // 
            this.tbxChartWidth.Location = new System.Drawing.Point(94, 22);
            this.tbxChartWidth.Margin = new System.Windows.Forms.Padding(2);
            this.tbxChartWidth.Name = "tbxChartWidth";
            this.tbxChartWidth.Size = new System.Drawing.Size(85, 26);
            this.tbxChartWidth.TabIndex = 0;
            // 
            // splitContainerMajor
            // 
            this.splitContainerMajor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainerMajor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMajor.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerMajor.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMajor.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainerMajor.Name = "splitContainerMajor";
            this.splitContainerMajor.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMajor.Panel1
            // 
            this.splitContainerMajor.Panel1.Controls.Add(this.pbxChart);
            // 
            // splitContainerMajor.Panel2
            // 
            this.splitContainerMajor.Panel2.Controls.Add(this.gbxSetting);
            this.splitContainerMajor.Size = new System.Drawing.Size(750, 475);
            this.splitContainerMajor.SplitterDistance = 360;
            this.splitContainerMajor.SplitterWidth = 3;
            this.splitContainerMajor.TabIndex = 3;
            // 
            // pbxChart
            // 
            this.pbxChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbxChart.Location = new System.Drawing.Point(0, 0);
            this.pbxChart.Margin = new System.Windows.Forms.Padding(2);
            this.pbxChart.Name = "pbxChart";
            this.pbxChart.Size = new System.Drawing.Size(746, 356);
            this.pbxChart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxChart.TabIndex = 0;
            this.pbxChart.TabStop = false;
            // 
            // ctrlMAChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerMajor);
            this.Name = "ctrlMAChart";
            this.Size = new System.Drawing.Size(750, 475);
            this.Load += new System.EventHandler(this.ctrlMAChart_Load);
            this.gbxSetting.ResumeLayout(false);
            this.gbxSetting.PerformLayout();
            this.splitContainerMajor.Panel1.ResumeLayout(false);
            this.splitContainerMajor.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMajor)).EndInit();
            this.splitContainerMajor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbxSetting;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblToken1;
        private System.Windows.Forms.Label lblToken2;
        private System.Windows.Forms.TextBox tbxMaxValue;
        private System.Windows.Forms.TextBox tbxMaxFrequency;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox tbxMinValue;
        private System.Windows.Forms.Label lblFrequency;
        private System.Windows.Forms.TextBox tbxMinFrequency;
        private System.Windows.Forms.Label lblChartHeight;
        private System.Windows.Forms.TextBox tbxChartHeight;
        private System.Windows.Forms.Label lblChartWidth;
        private System.Windows.Forms.TextBox tbxChartWidth;
        private System.Windows.Forms.SplitContainer splitContainerMajor;
        private System.Windows.Forms.PictureBox pbxChart;
    }
}
