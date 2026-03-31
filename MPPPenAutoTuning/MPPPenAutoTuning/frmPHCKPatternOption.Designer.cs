namespace MPPPenAutoTuning
{
    partial class frmPHCKPatternOption
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
            this.tbWidth = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbScreenSize = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbHeight = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.plRightRectColor = new System.Windows.Forms.Panel();
            this.plLeftRectColor = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.plGrayLineColor = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbWidth
            // 
            this.tbWidth.Location = new System.Drawing.Point(125, 45);
            this.tbWidth.Name = "tbWidth";
            this.tbWidth.ReadOnly = true;
            this.tbWidth.Size = new System.Drawing.Size(60, 22);
            this.tbWidth.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(85, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Width";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cbScreenSize);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbHeight);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbWidth);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(223, 101);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Screen Size";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "Screen Size";
            // 
            // cbScreenSize
            // 
            this.cbScreenSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScreenSize.FormattingEnabled = true;
            this.cbScreenSize.Location = new System.Drawing.Point(78, 19);
            this.cbScreenSize.Name = "cbScreenSize";
            this.cbScreenSize.Size = new System.Drawing.Size(130, 20);
            this.cbScreenSize.TabIndex = 6;
            this.cbScreenSize.SelectedIndexChanged += new System.EventHandler(this.cbScreenSize_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(191, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "mm";
            // 
            // tbHeight
            // 
            this.tbHeight.Location = new System.Drawing.Point(125, 73);
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.ReadOnly = true;
            this.tbHeight.Size = new System.Drawing.Size(60, 22);
            this.tbHeight.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(85, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "Height";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "mm";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(22, 251);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(79, 27);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOKClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(137, 251);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(79, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.plGrayLineColor);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.plRightRectColor);
            this.groupBox2.Controls.Add(this.plLeftRectColor);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(12, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(223, 126);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rectangle Color";
            // 
            // plRightRectColor
            // 
            this.plRightRectColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.plRightRectColor.Location = new System.Drawing.Point(147, 55);
            this.plRightRectColor.Name = "plRightRectColor";
            this.plRightRectColor.Size = new System.Drawing.Size(31, 29);
            this.plRightRectColor.TabIndex = 4;
            this.plRightRectColor.Click += new System.EventHandler(this.OnpRightRectColorClick);
            // 
            // plLeftRectColor
            // 
            this.plLeftRectColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.plLeftRectColor.Location = new System.Drawing.Point(147, 22);
            this.plLeftRectColor.Name = "plLeftRectColor";
            this.plLeftRectColor.Size = new System.Drawing.Size(31, 29);
            this.plLeftRectColor.TabIndex = 3;
            this.plLeftRectColor.Click += new System.EventHandler(this.OnplLeftRectColorClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "Right Rectangle Color";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "Left Rectangle Color";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "Gray Line Color";
            // 
            // plGrayLineColor
            // 
            this.plGrayLineColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.plGrayLineColor.Location = new System.Drawing.Point(147, 89);
            this.plGrayLineColor.Name = "plGrayLineColor";
            this.plGrayLineColor.Size = new System.Drawing.Size(31, 29);
            this.plGrayLineColor.TabIndex = 5;
            // 
            // frmPHCKPatternOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 286);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmPHCKPatternOption";
            this.Text = "Options";
            this.Load += new System.EventHandler(this.OnFrmLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel plRightRectColor;
        private System.Windows.Forms.Panel plLeftRectColor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbScreenSize;
        private System.Windows.Forms.Panel plGrayLineColor;
        private System.Windows.Forms.Label label7;
    }
}