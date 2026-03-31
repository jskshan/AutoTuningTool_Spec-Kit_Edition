namespace MPPPenAutoTuning
{
    partial class frmPHCKPattern
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
            this.picPattern = new System.Windows.Forms.PictureBox();
            this.menustripContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolstripmenuitemOption = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripmenuitemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.lblReportNumber = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picPattern)).BeginInit();
            this.menustripContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // picPattern
            // 
            this.picPattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPattern.Location = new System.Drawing.Point(0, 0);
            this.picPattern.Name = "picPattern";
            this.picPattern.Size = new System.Drawing.Size(284, 262);
            this.picPattern.TabIndex = 0;
            this.picPattern.TabStop = false;
            this.picPattern.Paint += new System.Windows.Forms.PaintEventHandler(this.picPattern_Paint);
            this.picPattern.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnPatternMouseDown);
            // 
            // menustripContext
            // 
            this.menustripContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripmenuitemOption,
            this.toolstripmenuitemExit});
            this.menustripContext.Name = "contextMenuStrip";
            this.menustripContext.Size = new System.Drawing.Size(120, 48);
            // 
            // toolstripmenuitemOption
            // 
            this.toolstripmenuitemOption.Name = "toolstripmenuitemOption";
            this.toolstripmenuitemOption.Size = new System.Drawing.Size(119, 22);
            this.toolstripmenuitemOption.Text = "Options";
            this.toolstripmenuitemOption.Click += new System.EventHandler(this.OnMenuOptionsClick);
            // 
            // toolstripmenuitemExit
            // 
            this.toolstripmenuitemExit.Name = "toolstripmenuitemExit";
            this.toolstripmenuitemExit.Size = new System.Drawing.Size(119, 22);
            this.toolstripmenuitemExit.Text = "Exit";
            this.toolstripmenuitemExit.Click += new System.EventHandler(this.OnMenuExitClick);
            // 
            // lblReportNumber
            // 
            this.lblReportNumber.AutoSize = true;
            this.lblReportNumber.Location = new System.Drawing.Point(0, 0);
            this.lblReportNumber.Name = "lblReportNumber";
            this.lblReportNumber.Size = new System.Drawing.Size(0, 12);
            this.lblReportNumber.TabIndex = 1;
            // 
            // frmPHCKPattern
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.lblReportNumber);
            this.Controls.Add(this.picPattern);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmPHCKPattern";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.OnFrmLoad);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFrmMainKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.picPattern)).EndInit();
            this.menustripContext.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picPattern;
        private System.Windows.Forms.ContextMenuStrip menustripContext;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemOption;
        private System.Windows.Forms.ToolStripMenuItem toolstripmenuitemExit;
        private System.Windows.Forms.Label lblReportNumber;
    }
}