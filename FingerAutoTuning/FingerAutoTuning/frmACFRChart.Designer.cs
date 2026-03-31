namespace FingerAutoTuning
{
    partial class frmACFRChart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmACFRChart));
            this.ChartPicbx = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ChartPicbx)).BeginInit();
            this.SuspendLayout();
            // 
            // ChartPicbx
            // 
            this.ChartPicbx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ChartPicbx.Location = new System.Drawing.Point(9, 9);
            this.ChartPicbx.Name = "ChartPicbx";
            this.ChartPicbx.Size = new System.Drawing.Size(1165, 444);
            this.ChartPicbx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ChartPicbx.TabIndex = 0;
            this.ChartPicbx.TabStop = false;
            // 
            // frmACFRChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 461);
            this.Controls.Add(this.ChartPicbx);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmACFRChart";
            this.Text = "AC FrequencyRank Chart";
            ((System.ComponentModel.ISupportInitialize)(this.ChartPicbx)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ChartPicbx;
    }
}