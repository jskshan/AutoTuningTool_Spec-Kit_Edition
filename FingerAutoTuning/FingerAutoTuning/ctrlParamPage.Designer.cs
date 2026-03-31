using System.Windows.Forms;
namespace FingerAutoTuning
{
    partial class ctrlParamPage
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
            this.ParamItemGridView = new System.Windows.Forms.DataGridView();
            this.lbDescription = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ParamItemGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // ParamItemGridView
            // 
            this.ParamItemGridView.AllowUserToAddRows = false;
            this.ParamItemGridView.AllowUserToDeleteRows = false;
            this.ParamItemGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ParamItemGridView.BackgroundColor = System.Drawing.Color.White;
            this.ParamItemGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.ParamItemGridView.Location = new System.Drawing.Point(0, 0);
            this.ParamItemGridView.Name = "ParamItemGridView";
            this.ParamItemGridView.RowHeadersVisible = false;
            this.ParamItemGridView.RowTemplate.Height = 24;
            this.ParamItemGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ParamItemGridView.Size = new System.Drawing.Size(619, 337);
            this.ParamItemGridView.TabIndex = 0;
            this.ParamItemGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.ParamItemGridView_CellBeginEdit);
            this.ParamItemGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.ParamItemGridView_CellEndEdit);
            this.ParamItemGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ParamItemGridView_CellMouseClick);
            this.ParamItemGridView.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ParamItemGridView_CellMouseDown);
            this.ParamItemGridView.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.ParamItemGridView_CellMouseEnter);
            this.ParamItemGridView.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.ParamItemGridView_CellMouseLeave);
            this.ParamItemGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.ParamItemGridView_CellPainting);
            this.ParamItemGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ParamItemGridView_CellValueChanged);
            this.ParamItemGridView.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.ParamItemGridView_ColumnWidthChanged);
            this.ParamItemGridView.CurrentCellChanged += new System.EventHandler(this.ParamItemGridView_CurrentCellChanged);
            this.ParamItemGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ParamItemGridView_EditingControlShowing);
            this.ParamItemGridView.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.ParamItemGridView_RowPrePaint);
            // 
            // lbDescription
            // 
            this.lbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDescription.BackColor = System.Drawing.Color.LightGray;
            this.lbDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbDescription.Location = new System.Drawing.Point(0, 340);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(619, 72);
            this.lbDescription.TabIndex = 1;
            // 
            // ctrlParamPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbDescription);
            this.Controls.Add(this.ParamItemGridView);
            this.Name = "ctrlParamPage";
            this.Size = new System.Drawing.Size(619, 412);
            ((System.ComponentModel.ISupportInitialize)(this.ParamItemGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView ParamItemGridView;
        private Label lbDescription;
    }
}
