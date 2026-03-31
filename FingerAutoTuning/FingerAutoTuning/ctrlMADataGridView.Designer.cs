namespace FingerAutoTuning
{
    partial class ctrlMADataGridView
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
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
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvRank = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRank)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvRank
            // 
            this.dgvRank.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRank.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRank.Location = new System.Drawing.Point(0, 0);
            this.dgvRank.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvRank.Name = "dgvRank";
            this.dgvRank.ReadOnly = true;
            this.dgvRank.RowHeadersWidth = 51;
            this.dgvRank.RowTemplate.Height = 27;
            this.dgvRank.Size = new System.Drawing.Size(750, 475);
            this.dgvRank.TabIndex = 0;
            this.dgvRank.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvRank_CellFormatting);
            this.dgvRank.SizeChanged += new System.EventHandler(this.dgvRank_SizeChanged);
            // 
            // ctrlMADataGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvRank);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ctrlMADataGridView";
            this.Size = new System.Drawing.Size(750, 475);
            this.Load += new System.EventHandler(this.ctrlMADataGridView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRank)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvRank;
    }
}
