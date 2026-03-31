namespace MPPPenAutoTuning
{
    partial class frmDFT_NUMAndCoeffConverter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDFT_NUMAndCoeffConverter));
            this.lblPH1 = new System.Windows.Forms.Label();
            this.tbxPH1 = new System.Windows.Forms.TextBox();
            this.tbxPH2 = new System.Windows.Forms.TextBox();
            this.lblPH2 = new System.Windows.Forms.Label();
            this.btnConvert = new System.Windows.Forms.Button();
            this.tbxSKIP_NUM = new System.Windows.Forms.TextBox();
            this.lblSKIP_NUM = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPH1
            // 
            this.lblPH1.AutoSize = true;
            this.lblPH1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPH1.Location = new System.Drawing.Point(75, 27);
            this.lblPH1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPH1.Name = "lblPH1";
            this.lblPH1.Size = new System.Drawing.Size(43, 21);
            this.lblPH1.TabIndex = 0;
            this.lblPH1.Text = "PH1";
            // 
            // tbxPH1
            // 
            this.tbxPH1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxPH1.Location = new System.Drawing.Point(148, 24);
            this.tbxPH1.Margin = new System.Windows.Forms.Padding(2);
            this.tbxPH1.Name = "tbxPH1";
            this.tbxPH1.Size = new System.Drawing.Size(76, 29);
            this.tbxPH1.TabIndex = 1;
            // 
            // tbxPH2
            // 
            this.tbxPH2.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxPH2.Location = new System.Drawing.Point(148, 65);
            this.tbxPH2.Margin = new System.Windows.Forms.Padding(2);
            this.tbxPH2.Name = "tbxPH2";
            this.tbxPH2.Size = new System.Drawing.Size(76, 29);
            this.tbxPH2.TabIndex = 3;
            // 
            // lblPH2
            // 
            this.lblPH2.AutoSize = true;
            this.lblPH2.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPH2.Location = new System.Drawing.Point(75, 68);
            this.lblPH2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPH2.Name = "lblPH2";
            this.lblPH2.Size = new System.Drawing.Size(43, 21);
            this.lblPH2.TabIndex = 2;
            this.lblPH2.Text = "PH2";
            // 
            // btnConvert
            // 
            this.btnConvert.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConvert.Location = new System.Drawing.Point(90, 233);
            this.btnConvert.Margin = new System.Windows.Forms.Padding(2);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(117, 55);
            this.btnConvert.TabIndex = 4;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // tbxSKIP_NUM
            // 
            this.tbxSKIP_NUM.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxSKIP_NUM.Location = new System.Drawing.Point(148, 110);
            this.tbxSKIP_NUM.Margin = new System.Windows.Forms.Padding(2);
            this.tbxSKIP_NUM.Name = "tbxSKIP_NUM";
            this.tbxSKIP_NUM.Size = new System.Drawing.Size(76, 29);
            this.tbxSKIP_NUM.TabIndex = 6;
            // 
            // lblSKIP_NUM
            // 
            this.lblSKIP_NUM.AutoSize = true;
            this.lblSKIP_NUM.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSKIP_NUM.Location = new System.Drawing.Point(14, 113);
            this.lblSKIP_NUM.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSKIP_NUM.Name = "lblSKIP_NUM";
            this.lblSKIP_NUM.Size = new System.Drawing.Size(103, 21);
            this.lblSKIP_NUM.TabIndex = 5;
            this.lblSKIP_NUM.Text = "SKIP_NUM";
            // 
            // frmDFT_NUMAndCoeffConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 315);
            this.Controls.Add(this.tbxSKIP_NUM);
            this.Controls.Add(this.lblSKIP_NUM);
            this.Controls.Add(this.tbxPH1);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.tbxPH2);
            this.Controls.Add(this.lblPH2);
            this.Controls.Add(this.lblPH1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmDFT_NUMAndCoeffConverter";
            this.Text = "DFT_NUM & Coeff Converter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPH1;
        private System.Windows.Forms.TextBox tbxPH1;
        private System.Windows.Forms.TextBox tbxPH2;
        private System.Windows.Forms.Label lblPH2;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.TextBox tbxSKIP_NUM;
        private System.Windows.Forms.Label lblSKIP_NUM;
    }
}