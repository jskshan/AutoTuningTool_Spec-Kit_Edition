namespace FingerAutoTuning
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
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FrameNumberLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picPattern)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
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
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(122, 48);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Visible = false;
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.OnMenuOptionsClick);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnMenuExitClick);
            // 
            // FrameNumberLbl
            // 
            this.FrameNumberLbl.AutoSize = true;
            this.FrameNumberLbl.Location = new System.Drawing.Point(0, 0);
            this.FrameNumberLbl.Name = "FrameNumberLbl";
            this.FrameNumberLbl.Size = new System.Drawing.Size(0, 12);
            this.FrameNumberLbl.TabIndex = 1;
            // 
            // frmPHCKPattern
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.FrameNumberLbl);
            this.Controls.Add(this.picPattern);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmPHCKPattern";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.OnFrmLoad);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFrmMainKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.picPattern)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picPattern;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label FrameNumberLbl;
    }
}