namespace FingerAutoTuning
{
    partial class frmPatternMenu
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
            this.CollectStepPatternLbl = new System.Windows.Forms.Label();
            this.ExitBtn = new System.Windows.Forms.Button();
            this.RetryBtn = new System.Windows.Forms.Button();
            this.panelTool = new System.Windows.Forms.Panel();
            this.StatusLbl = new System.Windows.Forms.Label();
            this.ReportNumberLbl = new System.Windows.Forms.Label();
            this.ResultInfoGridView = new System.Windows.Forms.DataGridView();
            this.panelTool.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultInfoGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // CollectStepPatternLbl
            // 
            this.CollectStepPatternLbl.BackColor = System.Drawing.SystemColors.HotTrack;
            this.CollectStepPatternLbl.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CollectStepPatternLbl.ForeColor = System.Drawing.Color.White;
            this.CollectStepPatternLbl.Location = new System.Drawing.Point(0, 1);
            this.CollectStepPatternLbl.Name = "CollectStepPatternLbl";
            this.CollectStepPatternLbl.Size = new System.Drawing.Size(243, 40);
            this.CollectStepPatternLbl.TabIndex = 2;
            this.CollectStepPatternLbl.Text = "Step/PatternLabel";
            this.CollectStepPatternLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CollectStepPatternLbl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnlbTestNameMouseDown);
            this.CollectStepPatternLbl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnlbTestNameMouseMove);
            this.CollectStepPatternLbl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnlbTestNameMouseUp);
            // 
            // ExitBtn
            // 
            this.ExitBtn.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExitBtn.Location = new System.Drawing.Point(135, 143);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(90, 40);
            this.ExitBtn.TabIndex = 2;
            this.ExitBtn.Text = "Exit";
            this.ExitBtn.UseVisualStyleBackColor = true;
            this.ExitBtn.Click += new System.EventHandler(this.OnbtnExitClick);
            // 
            // RetryBtn
            // 
            this.RetryBtn.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RetryBtn.Location = new System.Drawing.Point(19, 143);
            this.RetryBtn.Name = "RetryBtn";
            this.RetryBtn.Size = new System.Drawing.Size(90, 40);
            this.RetryBtn.TabIndex = 1;
            this.RetryBtn.Text = "Retry";
            this.RetryBtn.UseVisualStyleBackColor = true;
            this.RetryBtn.Click += new System.EventHandler(this.OnbtnRetryClick);
            // 
            // panelTool
            // 
            this.panelTool.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelTool.Controls.Add(this.StatusLbl);
            this.panelTool.Controls.Add(this.ReportNumberLbl);
            this.panelTool.Controls.Add(this.CollectStepPatternLbl);
            this.panelTool.Controls.Add(this.ExitBtn);
            this.panelTool.Controls.Add(this.RetryBtn);
            this.panelTool.Location = new System.Drawing.Point(0, 0);
            this.panelTool.Name = "panelTool";
            this.panelTool.Size = new System.Drawing.Size(243, 185);
            this.panelTool.TabIndex = 2;
            // 
            // StatusLbl
            // 
            this.StatusLbl.BackColor = System.Drawing.Color.LightGreen;
            this.StatusLbl.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLbl.ForeColor = System.Drawing.Color.Black;
            this.StatusLbl.Location = new System.Drawing.Point(0, 45);
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(243, 35);
            this.StatusLbl.TabIndex = 5;
            this.StatusLbl.Text = "StatusLabel";
            this.StatusLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ReportNumberLbl
            // 
            this.ReportNumberLbl.BackColor = System.Drawing.SystemColors.Info;
            this.ReportNumberLbl.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReportNumberLbl.ForeColor = System.Drawing.Color.Black;
            this.ReportNumberLbl.Location = new System.Drawing.Point(0, 85);
            this.ReportNumberLbl.Name = "ReportNumberLbl";
            this.ReportNumberLbl.Size = new System.Drawing.Size(243, 55);
            this.ReportNumberLbl.TabIndex = 4;
            this.ReportNumberLbl.Text = "ReportNumberLabel";
            this.ReportNumberLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ResultInfoGridView
            // 
            this.ResultInfoGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.ResultInfoGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ResultInfoGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResultInfoGridView.Location = new System.Drawing.Point(0, 191);
            this.ResultInfoGridView.Name = "ResultInfoGridView";
            this.ResultInfoGridView.RowTemplate.Height = 24;
            this.ResultInfoGridView.Size = new System.Drawing.Size(243, 208);
            this.ResultInfoGridView.TabIndex = 5;
            // 
            // frmPatternMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 402);
            this.Controls.Add(this.ResultInfoGridView);
            this.Controls.Add(this.panelTool);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmPatternMenu";
            this.Opacity = 0.9D;
            this.Text = "frmPatterMenu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnfrmClosing);
            this.Load += new System.EventHandler(this.OnfrmLoad);
            this.panelTool.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResultInfoGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label CollectStepPatternLbl;
        private System.Windows.Forms.Button ExitBtn;
        private System.Windows.Forms.Button RetryBtn;
        private System.Windows.Forms.Panel panelTool;
        private System.Windows.Forms.DataGridView ResultInfoGridView;
        private System.Windows.Forms.Label ReportNumberLbl;
        private System.Windows.Forms.Label StatusLbl;
    }
}