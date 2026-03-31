namespace FingerAutoTuning
{
    partial class frmGetIPAddress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGetIPAddress));
            this.IPAddressLbl = new System.Windows.Forms.Label();
            this.IPAddressTbx = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // IPAddressLbl
            // 
            this.IPAddressLbl.AutoSize = true;
            this.IPAddressLbl.Font = new System.Drawing.Font("Times", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPAddressLbl.Location = new System.Drawing.Point(4, 45);
            this.IPAddressLbl.Name = "IPAddressLbl";
            this.IPAddressLbl.Size = new System.Drawing.Size(93, 21);
            this.IPAddressLbl.TabIndex = 0;
            this.IPAddressLbl.Text = "IP Address";
            // 
            // IPAddressTbx
            // 
            this.IPAddressTbx.Font = new System.Drawing.Font("Times", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPAddressTbx.Location = new System.Drawing.Point(103, 44);
            this.IPAddressTbx.Name = "IPAddressTbx";
            this.IPAddressTbx.ReadOnly = true;
            this.IPAddressTbx.Size = new System.Drawing.Size(229, 26);
            this.IPAddressTbx.TabIndex = 26;
            this.IPAddressTbx.TabStop = false;
            // 
            // frmGetIPAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 122);
            this.Controls.Add(this.IPAddressTbx);
            this.Controls.Add(this.IPAddressLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGetIPAddress";
            this.Text = "Get IP Address";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label IPAddressLbl;
        private System.Windows.Forms.TextBox IPAddressTbx;
    }
}