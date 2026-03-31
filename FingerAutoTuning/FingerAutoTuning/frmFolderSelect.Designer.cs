namespace FingerAutoTuning
{
    partial class frmFolderSelect
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
            this.FolderPathLbl = new System.Windows.Forms.Label();
            this.SelectPathTbx = new System.Windows.Forms.TextBox();
            this.SelectBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FolderPathLbl
            // 
            this.FolderPathLbl.AutoSize = true;
            this.FolderPathLbl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FolderPathLbl.Location = new System.Drawing.Point(6, 27);
            this.FolderPathLbl.Name = "FolderPathLbl";
            this.FolderPathLbl.Size = new System.Drawing.Size(87, 19);
            this.FolderPathLbl.TabIndex = 0;
            this.FolderPathLbl.Text = "Folder Path :";
            // 
            // SelectPathTbx
            // 
            this.SelectPathTbx.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectPathTbx.Location = new System.Drawing.Point(100, 26);
            this.SelectPathTbx.Name = "SelectPathTbx";
            this.SelectPathTbx.Size = new System.Drawing.Size(373, 22);
            this.SelectPathTbx.TabIndex = 1;
            // 
            // SelectBtn
            // 
            this.SelectBtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectBtn.Location = new System.Drawing.Point(479, 23);
            this.SelectBtn.Name = "SelectBtn";
            this.SelectBtn.Size = new System.Drawing.Size(77, 26);
            this.SelectBtn.TabIndex = 2;
            this.SelectBtn.Text = "Select";
            this.SelectBtn.UseVisualStyleBackColor = true;
            this.SelectBtn.Click += new System.EventHandler(this.SelectBtn_Click);
            // 
            // OKBtn
            // 
            this.OKBtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OKBtn.Location = new System.Drawing.Point(466, 62);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(90, 32);
            this.OKBtn.TabIndex = 3;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // frmFolderSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 106);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.SelectBtn);
            this.Controls.Add(this.SelectPathTbx);
            this.Controls.Add(this.FolderPathLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmFolderSelect";
            this.Text = "Folder Select";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmFolderSelect_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label FolderPathLbl;
        private System.Windows.Forms.TextBox SelectPathTbx;
        private System.Windows.Forms.Button SelectBtn;
        private System.Windows.Forms.Button OKBtn;
    }
}