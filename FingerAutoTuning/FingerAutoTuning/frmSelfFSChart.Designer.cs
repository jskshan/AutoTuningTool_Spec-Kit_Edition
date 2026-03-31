namespace FingerAutoTuning
{
    partial class frmSelfFSChart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelfFSChart));
            this.lblTraceType = new System.Windows.Forms.Label();
            this.cbxTraceType = new System.Windows.Forms.ComboBox();
            this.cbxValueType = new System.Windows.Forms.ComboBox();
            this.lblValueType = new System.Windows.Forms.Label();
            this.picbxChart = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picbxChart)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTraceType
            // 
            this.lblTraceType.AutoSize = true;
            this.lblTraceType.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTraceType.Location = new System.Drawing.Point(9, 9);
            this.lblTraceType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTraceType.Name = "lblTraceType";
            this.lblTraceType.Size = new System.Drawing.Size(95, 21);
            this.lblTraceType.TabIndex = 0;
            this.lblTraceType.Text = "Trace Type";
            // 
            // cbxTraceType
            // 
            this.cbxTraceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTraceType.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxTraceType.FormattingEnabled = true;
            this.cbxTraceType.Location = new System.Drawing.Point(112, 6);
            this.cbxTraceType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbxTraceType.Name = "cbxTraceType";
            this.cbxTraceType.Size = new System.Drawing.Size(77, 28);
            this.cbxTraceType.TabIndex = 1;
            this.cbxTraceType.SelectedIndexChanged += new System.EventHandler(this.cbxTraceType_SelectedIndexChanged);
            // 
            // cbxValueType
            // 
            this.cbxValueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxValueType.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxValueType.FormattingEnabled = true;
            this.cbxValueType.Location = new System.Drawing.Point(341, 6);
            this.cbxValueType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbxValueType.Name = "cbxValueType";
            this.cbxValueType.Size = new System.Drawing.Size(313, 28);
            this.cbxValueType.TabIndex = 3;
            this.cbxValueType.SelectedIndexChanged += new System.EventHandler(this.cbxValueType_SelectedIndexChanged);
            // 
            // lblValueType
            // 
            this.lblValueType.AutoSize = true;
            this.lblValueType.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValueType.Location = new System.Drawing.Point(238, 9);
            this.lblValueType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblValueType.Name = "lblValueType";
            this.lblValueType.Size = new System.Drawing.Size(93, 21);
            this.lblValueType.TabIndex = 2;
            this.lblValueType.Text = "Value Type";
            // 
            // picbxChart
            // 
            this.picbxChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picbxChart.Location = new System.Drawing.Point(10, 39);
            this.picbxChart.Name = "picbxChart";
            this.picbxChart.Size = new System.Drawing.Size(1165, 613);
            this.picbxChart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbxChart.TabIndex = 4;
            this.picbxChart.TabStop = false;
            // 
            // frmSelfFSChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.picbxChart);
            this.Controls.Add(this.cbxValueType);
            this.Controls.Add(this.lblValueType);
            this.Controls.Add(this.cbxTraceType);
            this.Controls.Add(this.lblTraceType);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmSelfFSChart";
            this.Text = "Self FrequencySweep Chart";
            ((System.ComponentModel.ISupportInitialize)(this.picbxChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTraceType;
        private System.Windows.Forms.ComboBox cbxTraceType;
        private System.Windows.Forms.ComboBox cbxValueType;
        private System.Windows.Forms.Label lblValueType;
        private System.Windows.Forms.PictureBox picbxChart;
    }
}