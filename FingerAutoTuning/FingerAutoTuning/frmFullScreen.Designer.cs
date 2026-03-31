namespace FingerAutoTuning
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
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProgressStatusLbl = new System.Windows.Forms.Label();
            this.picPattern = new System.Windows.Forms.PictureBox();
            this.ReportNumberLbl = new System.Windows.Forms.Label();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPattern)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(97, 26);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ProgressStatusLbl
            // 
            this.ProgressStatusLbl.AutoSize = true;
            this.ProgressStatusLbl.Font = new System.Drawing.Font("Times New Roman", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProgressStatusLbl.Location = new System.Drawing.Point(0, 0);
            this.ProgressStatusLbl.Name = "ProgressStatusLbl";
            this.ProgressStatusLbl.Size = new System.Drawing.Size(0, 45);
            this.ProgressStatusLbl.TabIndex = 1;
            this.ProgressStatusLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            // ReportNumberLbl
            // 
            this.ReportNumberLbl.AutoSize = true;
            this.ReportNumberLbl.Location = new System.Drawing.Point(3, 3);
            this.ReportNumberLbl.Name = "ReportNumberLbl";
            this.ReportNumberLbl.Size = new System.Drawing.Size(0, 12);
            this.ReportNumberLbl.TabIndex = 3;
            // 
            // frmFullScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.ReportNumberLbl);
            this.Controls.Add(this.picPattern);
            this.Controls.Add(this.ProgressStatusLbl);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmFullScreen";
            this.Text = "frmFullScreen";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmFullScreen_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmFullScreen_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmFullScreen_MouseDown);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPattern)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label ProgressStatusLbl;
        private System.Windows.Forms.PictureBox picPattern;
        private System.Windows.Forms.Label ReportNumberLbl;
    }
}