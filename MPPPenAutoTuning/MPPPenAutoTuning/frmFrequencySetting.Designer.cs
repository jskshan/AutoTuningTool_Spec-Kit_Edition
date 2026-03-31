namespace MPPPenAutoTuning
{
    partial class frmFrequencySetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFrequencySetting));
            this.gbxFrequencySet = new System.Windows.Forms.GroupBox();
            this.dgvFrequencySet = new System.Windows.Forms.DataGridView();
            this.AllSelectItem = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllSelectPH1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllSelectPH2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllSelectFrequency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblSelectNumber = new System.Windows.Forms.Label();
            this.gbxFrequencySet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFrequencySet)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxFrequencySet
            // 
            this.gbxFrequencySet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxFrequencySet.Controls.Add(this.dgvFrequencySet);
            this.gbxFrequencySet.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxFrequencySet.Location = new System.Drawing.Point(12, 12);
            this.gbxFrequencySet.Name = "gbxFrequencySet";
            this.gbxFrequencySet.Size = new System.Drawing.Size(869, 510);
            this.gbxFrequencySet.TabIndex = 0;
            this.gbxFrequencySet.TabStop = false;
            this.gbxFrequencySet.Text = "Frequency Set";
            // 
            // dgvFrequencySet
            // 
            this.dgvFrequencySet.AllowUserToAddRows = false;
            this.dgvFrequencySet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFrequencySet.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFrequencySet.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFrequencySet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFrequencySet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AllSelectItem,
            this.Column1,
            this.AllSelectPH1,
            this.AllSelectPH2,
            this.AllSelectFrequency});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFrequencySet.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFrequencySet.Location = new System.Drawing.Point(7, 34);
            this.dgvFrequencySet.Margin = new System.Windows.Forms.Padding(4);
            this.dgvFrequencySet.Name = "dgvFrequencySet";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFrequencySet.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvFrequencySet.RowHeadersWidth = 55;
            this.dgvFrequencySet.RowTemplate.Height = 24;
            this.dgvFrequencySet.Size = new System.Drawing.Size(855, 467);
            this.dgvFrequencySet.TabIndex = 4;
            this.dgvFrequencySet.RowHeadersWidthChanged += new System.EventHandler(this.dgvFrequencySet_RowHeadersWidthChanged);
            this.dgvFrequencySet.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFrequencySet_CellContentClick);
            this.dgvFrequencySet.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFrequencySet_CellContentDoubleClick);
            // 
            // AllSelectItem
            // 
            this.AllSelectItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AllSelectItem.FillWeight = 20F;
            this.AllSelectItem.HeaderText = "Item";
            this.AllSelectItem.Name = "AllSelectItem";
            this.AllSelectItem.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.AllSelectItem.Width = 75;
            // 
            // Column1
            // 
            this.Column1.FillWeight = 15F;
            this.Column1.HeaderText = "Rank";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 101;
            // 
            // AllSelectPH1
            // 
            this.AllSelectPH1.FillWeight = 15F;
            this.AllSelectPH1.HeaderText = SpecificText.m_sPH1;
            this.AllSelectPH1.Name = "AllSelectPH1";
            this.AllSelectPH1.ReadOnly = true;
            this.AllSelectPH1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.AllSelectPH1.Width = 101;
            // 
            // AllSelectPH2
            // 
            this.AllSelectPH2.FillWeight = 15F;
            this.AllSelectPH2.HeaderText = SpecificText.m_sPH2;
            this.AllSelectPH2.Name = "AllSelectPH2";
            this.AllSelectPH2.ReadOnly = true;
            this.AllSelectPH2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.AllSelectPH2.Width = 101;
            // 
            // AllSelectFrequency
            // 
            this.AllSelectFrequency.FillWeight = 20F;
            this.AllSelectFrequency.HeaderText = SpecificText.m_sFrequency_KHz;
            this.AllSelectFrequency.Name = "AllSelectFrequency";
            this.AllSelectFrequency.ReadOnly = true;
            this.AllSelectFrequency.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.AllSelectFrequency.Width = 134;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(961, 465);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(126, 49);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblSelectNumber
            // 
            this.lblSelectNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelectNumber.AutoSize = true;
            this.lblSelectNumber.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectNumber.Location = new System.Drawing.Point(888, 59);
            this.lblSelectNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSelectNumber.Name = "lblSelectNumber";
            this.lblSelectNumber.Size = new System.Drawing.Size(134, 21);
            this.lblSelectNumber.TabIndex = 7;
            this.lblSelectNumber.Text = "Select Number : ";
            // 
            // frmFrequencySetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 526);
            this.Controls.Add(this.lblSelectNumber);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gbxFrequencySet);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFrequencySetting";
            this.Text = "Frequency Setting";
            this.Load += new System.EventHandler(this.frmFrequencySetting_Load);
            this.gbxFrequencySet.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFrequencySet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxFrequencySet;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridView dgvFrequencySet;
        private System.Windows.Forms.DataGridViewCheckBoxColumn AllSelectItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn AllSelectPH1;
        private System.Windows.Forms.DataGridViewTextBoxColumn AllSelectPH2;
        private System.Windows.Forms.DataGridViewTextBoxColumn AllSelectFrequency;
        private System.Windows.Forms.Label lblSelectNumber;
    }
}