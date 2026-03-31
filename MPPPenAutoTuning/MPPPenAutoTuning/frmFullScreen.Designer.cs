namespace MPPPenAutoTuning
{
    partial class frmFullScreen
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
            this.menustripContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolstripmenuitemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.lblProgressStatus = new System.Windows.Forms.Label();
            this.picPattern = new System.Windows.Forms.PictureBox();
            this.lblReportNumber = new System.Windows.Forms.Label();
            this.menustripContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPattern)).BeginInit();
            this.SuspendLayout();
            // 
            // menustripContext
            // 
            this.menustripContext.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menustripContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuitemExit});
            this.menustripContext.Name = "contextMenuStrip";
            this.menustripContext.Size = new System.Drawing.Size(95, 26);
            // 
            // toolstripmenuitemExit
            // 
            this.toolstripmenuitemExit.Name = "toolstripmenuitemExit";
            this.toolstripmenuitemExit.Size = new System.Drawing.Size(94, 22);
            this.toolstripmenuitemExit.Text = "Exit";
            this.toolstripmenuitemExit.Click += new System.EventHandler(this.toolstripmenuitemExit_Click);
            // 
            // lblProgressStatus
            // 
            this.lblProgressStatus.AutoSize = true;
            this.lblProgressStatus.Font = new System.Drawing.Font("Times New Roman", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgressStatus.Location = new System.Drawing.Point(0, 0);
            this.lblProgressStatus.Name = "lblProgressStatus";
            this.lblProgressStatus.Size = new System.Drawing.Size(0, 45);
            this.lblProgressStatus.TabIndex = 1;
            this.lblProgressStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picPattern
            // 
            this.picPattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPattern.Location = new System.Drawing.Point(0, 0);
            this.picPattern.Name = "picPattern";
            this.picPattern.Size = new System.Drawing.Size(292, 266);
            this.picPattern.TabIndex = 2;
            this.picPattern.TabStop = false;
            this.picPattern.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmFullScreen_MouseDown);
            // 
            // lblReportNumber
            // 
            this.lblReportNumber.AutoSize = true;
            this.lblReportNumber.Location = new System.Drawing.Point(3, 3);
            this.lblReportNumber.Name = "lblReportNumber";
            this.lblReportNumber.Size = new System.Drawing.Size(0, 12);
            this.lblReportNumber.TabIndex = 3;
            // 
            // frmFullScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.lblReportNumber);
            this.Controls.Add(this.picPattern);
            this.Controls.Add(this.lblProgressStatus);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmFullScreen";
            this.Text = "frmFullScreen";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmFullScreen_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmFullScreen_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmFullScreen_MouseDown);
            this.menustripContext.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPattern)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip menustripContext;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemExit;
        private System.Windows.Forms.Label lblProgressStatus;
        private System.Windows.Forms.PictureBox picPattern;
        private System.Windows.Forms.Label lblReportNumber;

    }
}