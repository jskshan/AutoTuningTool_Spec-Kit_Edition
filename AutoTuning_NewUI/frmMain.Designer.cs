namespace AutoTuning_NewUI
{
    partial class frmMain
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

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.btnMPPPen = new System.Windows.Forms.Button();
            this.btnFinger = new System.Windows.Forms.Button();
            this.pnlBar = new System.Windows.Forms.Panel();
            this.picbxIcon = new System.Windows.Forms.PictureBox();
            this.btnMenu = new System.Windows.Forms.Button();
            this.pnlTitleBar = new System.Windows.Forms.Panel();
            this.btnMaximum = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMinimum = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlDesktop = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlMenu.SuspendLayout();
            this.pnlBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxIcon)).BeginInit();
            this.pnlTitleBar.SuspendLayout();
            this.pnlDesktop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMenu
            // 
            this.pnlMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(102)))), ((int)(((byte)(244)))));
            this.pnlMenu.Controls.Add(this.btnMPPPen);
            this.pnlMenu.Controls.Add(this.btnFinger);
            this.pnlMenu.Controls.Add(this.pnlBar);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMenu.Margin = new System.Windows.Forms.Padding(4);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(150, 706);
            this.pnlMenu.TabIndex = 0;
            // 
            // btnMPPPen
            // 
            this.btnMPPPen.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMPPPen.FlatAppearance.BorderSize = 0;
            this.btnMPPPen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMPPPen.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMPPPen.ForeColor = System.Drawing.Color.White;
            this.btnMPPPen.Image = ((System.Drawing.Image)(resources.GetObject("btnMPPPen.Image")));
            this.btnMPPPen.Location = new System.Drawing.Point(0, 194);
            this.btnMPPPen.Name = "btnMPPPen";
            this.btnMPPPen.Size = new System.Drawing.Size(150, 48);
            this.btnMPPPen.TabIndex = 4;
            this.btnMPPPen.Tag = "MPP Pen";
            this.btnMPPPen.Text = "MPP Pen";
            this.btnMPPPen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMPPPen.UseVisualStyleBackColor = true;
            this.btnMPPPen.Click += new System.EventHandler(this.btnMPPPen_Click);
            // 
            // btnFinger
            // 
            this.btnFinger.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnFinger.FlatAppearance.BorderSize = 0;
            this.btnFinger.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFinger.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFinger.ForeColor = System.Drawing.Color.White;
            this.btnFinger.Image = ((System.Drawing.Image)(resources.GetObject("btnFinger.Image")));
            this.btnFinger.Location = new System.Drawing.Point(0, 146);
            this.btnFinger.Name = "btnFinger";
            this.btnFinger.Size = new System.Drawing.Size(150, 48);
            this.btnFinger.TabIndex = 3;
            this.btnFinger.Tag = "Finger";
            this.btnFinger.Text = "Finger";
            this.btnFinger.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFinger.UseVisualStyleBackColor = true;
            this.btnFinger.Click += new System.EventHandler(this.btnFinger_Click);
            // 
            // pnlBar
            // 
            this.pnlBar.Controls.Add(this.picbxIcon);
            this.pnlBar.Controls.Add(this.btnMenu);
            this.pnlBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBar.Location = new System.Drawing.Point(0, 0);
            this.pnlBar.Margin = new System.Windows.Forms.Padding(4);
            this.pnlBar.Name = "pnlBar";
            this.pnlBar.Size = new System.Drawing.Size(150, 146);
            this.pnlBar.TabIndex = 0;
            // 
            // picbxIcon
            // 
            this.picbxIcon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picbxIcon.BackgroundImage")));
            this.picbxIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.picbxIcon.Location = new System.Drawing.Point(0, 0);
            this.picbxIcon.Name = "picbxIcon";
            this.picbxIcon.Size = new System.Drawing.Size(62, 46);
            this.picbxIcon.TabIndex = 1;
            this.picbxIcon.TabStop = false;
            // 
            // btnMenu
            // 
            this.btnMenu.BackgroundImage = global::AutoTuning_NewUI.Properties.Resources.drop_down_menu;
            this.btnMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnMenu.FlatAppearance.BorderSize = 0;
            this.btnMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenu.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMenu.ForeColor = System.Drawing.Color.White;
            this.btnMenu.Location = new System.Drawing.Point(101, 1);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(48, 45);
            this.btnMenu.TabIndex = 0;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // pnlTitleBar
            // 
            this.pnlTitleBar.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pnlTitleBar.Controls.Add(this.btnMaximum);
            this.pnlTitleBar.Controls.Add(this.btnClose);
            this.pnlTitleBar.Controls.Add(this.btnMinimum);
            this.pnlTitleBar.Controls.Add(this.lblTitle);
            this.pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.Location = new System.Drawing.Point(150, 0);
            this.pnlTitleBar.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTitleBar.Name = "pnlTitleBar";
            this.pnlTitleBar.Size = new System.Drawing.Size(1034, 25);
            this.pnlTitleBar.TabIndex = 1;
            this.pnlTitleBar.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pnlTitleBar_MouseDoubleClick);
            this.pnlTitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTitleBar_MouseDown);
            this.pnlTitleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlTitleBar_MouseMove);
            // 
            // btnMaximum
            // 
            this.btnMaximum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMaximum.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnMaximum.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMaximum.BackgroundImage")));
            this.btnMaximum.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnMaximum.FlatAppearance.BorderSize = 0;
            this.btnMaximum.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaximum.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaximum.ForeColor = System.Drawing.Color.White;
            this.btnMaximum.Location = new System.Drawing.Point(937, 0);
            this.btnMaximum.Margin = new System.Windows.Forms.Padding(0);
            this.btnMaximum.Name = "btnMaximum";
            this.btnMaximum.Size = new System.Drawing.Size(49, 25);
            this.btnMaximum.TabIndex = 7;
            this.btnMaximum.UseVisualStyleBackColor = false;
            this.btnMaximum.Click += new System.EventHandler(this.btnMaximum_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(74)))), ((int)(((byte)(130)))));
            this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(985, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(49, 25);
            this.btnClose.TabIndex = 6;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMinimum
            // 
            this.btnMinimum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimum.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnMinimum.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMinimum.BackgroundImage")));
            this.btnMinimum.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnMinimum.FlatAppearance.BorderSize = 0;
            this.btnMinimum.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimum.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMinimum.ForeColor = System.Drawing.Color.White;
            this.btnMinimum.Location = new System.Drawing.Point(889, 0);
            this.btnMinimum.Margin = new System.Windows.Forms.Padding(0);
            this.btnMinimum.Name = "btnMinimum";
            this.btnMinimum.Size = new System.Drawing.Size(49, 25);
            this.btnMinimum.TabIndex = 1;
            this.btnMinimum.UseVisualStyleBackColor = false;
            this.btnMinimum.Click += new System.EventHandler(this.btnMinimum_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(4, 2);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(78, 19);
            this.lblTitle.TabIndex = 5;
            this.lblTitle.Text = "AutoTuning";
            this.lblTitle.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseDoubleClick);
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseDown);
            this.lblTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseMove);
            // 
            // pnlDesktop
            // 
            this.pnlDesktop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            this.pnlDesktop.Controls.Add(this.pnlContent);
            this.pnlDesktop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDesktop.Location = new System.Drawing.Point(150, 25);
            this.pnlDesktop.Margin = new System.Windows.Forms.Padding(4);
            this.pnlDesktop.Name = "pnlDesktop";
            this.pnlDesktop.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.pnlDesktop.Size = new System.Drawing.Size(1034, 681);
            this.pnlDesktop.TabIndex = 2;
            // 
            // pnlContent
            // 
            this.pnlContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlContent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pnlContent.Location = new System.Drawing.Point(0, 0);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(4);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(1034, 681);
            this.pnlContent.TabIndex = 0;
            this.pnlContent.SizeChanged += new System.EventHandler(this.pnlContent_SizeChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 706);
            this.Controls.Add(this.pnlDesktop);
            this.Controls.Add(this.pnlTitleBar);
            this.Controls.Add(this.pnlMenu);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.Text = "AutoTuning NewUI";
            this.Activated += new System.EventHandler(this.frmMain_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.Move += new System.EventHandler(this.frmMain_Move);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.pnlMenu.ResumeLayout(false);
            this.pnlBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbxIcon)).EndInit();
            this.pnlTitleBar.ResumeLayout(false);
            this.pnlTitleBar.PerformLayout();
            this.pnlDesktop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.Panel pnlTitleBar;
        private System.Windows.Forms.Panel pnlDesktop;
        private System.Windows.Forms.Panel pnlBar;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Button btnFinger;
        private System.Windows.Forms.Button btnMPPPen;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Button btnMaximum;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimum;
        private System.Windows.Forms.PictureBox picbxIcon;
    }
}

