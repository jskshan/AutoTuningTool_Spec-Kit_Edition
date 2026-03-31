namespace FingerAutoTuning
{
    partial class ctrlTestItemCheckBox
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
            this.cbItemEnable = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbItemEnable
            // 
            this.cbItemEnable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.cbItemEnable.AutoSize = true;
            this.cbItemEnable.Location = new System.Drawing.Point(3, 3);
            this.cbItemEnable.Name = "cbItemEnable";
            this.cbItemEnable.Size = new System.Drawing.Size(15, 14);
            this.cbItemEnable.TabIndex = 0;
            this.cbItemEnable.UseVisualStyleBackColor = true;
            // 
            // ctrlTestItemCheckBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbItemEnable);
            this.Name = "ctrlTestItemCheckBox";
            this.Size = new System.Drawing.Size(220, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbItemEnable;
    }
}
