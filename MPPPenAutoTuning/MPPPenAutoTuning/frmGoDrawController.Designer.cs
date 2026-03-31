namespace MPPPenAutoTuning
{
    partial class frmGoDrawController
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGoDrawController));
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.pnlMessage = new System.Windows.Forms.Panel();
            this.lblCoordinate = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.gbxXYMoveControl = new System.Windows.Forms.GroupBox();
            this.btnMove = new System.Windows.Forms.Button();
            this.tbxDestinationCoordinateY = new System.Windows.Forms.TextBox();
            this.lblDestinationCoordinateY = new System.Windows.Forms.Label();
            this.tbxDestinationCoordinateX = new System.Windows.Forms.TextBox();
            this.lblDestinationCoordInateX = new System.Windows.Forms.Label();
            this.gbxZControl = new System.Windows.Forms.GroupBox();
            this.lblServoValue = new System.Windows.Forms.Label();
            this.cbxServoValueInterval = new System.Windows.Forms.ComboBox();
            this.lblServoValueInterval = new System.Windows.Forms.Label();
            this.dudZServoValueScroll = new System.Windows.Forms.DomainUpDown();
            this.btnZUpDown = new System.Windows.Forms.Button();
            this.btnContact = new System.Windows.Forms.Button();
            this.tbxContactServoValue = new System.Windows.Forms.TextBox();
            this.btnHover = new System.Windows.Forms.Button();
            this.lblContactServoValue = new System.Windows.Forms.Label();
            this.btnTop = new System.Windows.Forms.Button();
            this.tbxHoverServoValue = new System.Windows.Forms.TextBox();
            this.lblTopServoValue = new System.Windows.Forms.Label();
            this.lblHoverServoValue = new System.Windows.Forms.Label();
            this.tbxTopServoValue = new System.Windows.Forms.TextBox();
            this.gbxProcessMessage = new System.Windows.Forms.GroupBox();
            this.rtbxProcessMessage = new System.Windows.Forms.RichTextBox();
            this.lblMaxCoordinateX = new System.Windows.Forms.Label();
            this.tbxMaxCoordinateY = new System.Windows.Forms.TextBox();
            this.lblMaxCoordinateY = new System.Windows.Forms.Label();
            this.tbxCOMPort = new System.Windows.Forms.TextBox();
            this.lblCOMPort = new System.Windows.Forms.Label();
            this.gbxXYActionControl = new System.Windows.Forms.GroupBox();
            this.tbxDistance = new System.Windows.Forms.TextBox();
            this.lblDistance = new System.Windows.Forms.Label();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.gbxSave = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbxParameter = new System.Windows.Forms.ComboBox();
            this.lblParameter = new System.Windows.Forms.Label();
            this.cbxType = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.gbxGeneral = new System.Windows.Forms.GroupBox();
            this.tbxSpeed = new System.Windows.Forms.TextBox();
            this.tbxMaxCoordinateX = new System.Windows.Forms.TextBox();
            this.pnlMessage.SuspendLayout();
            this.gbxXYMoveControl.SuspendLayout();
            this.gbxZControl.SuspendLayout();
            this.gbxProcessMessage.SuspendLayout();
            this.gbxXYActionControl.SuspendLayout();
            this.gbxSave.SuspendLayout();
            this.gbxGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(1032, 260);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(125, 76);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnHome
            // 
            this.btnHome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHome.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHome.Location = new System.Drawing.Point(1032, 352);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(125, 76);
            this.btnHome.TabIndex = 9;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // pnlMessage
            // 
            this.pnlMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMessage.BackColor = System.Drawing.Color.LightCyan;
            this.pnlMessage.Controls.Add(this.lblCoordinate);
            this.pnlMessage.Controls.Add(this.lblMessage);
            this.pnlMessage.Location = new System.Drawing.Point(3, 5);
            this.pnlMessage.Name = "pnlMessage";
            this.pnlMessage.Size = new System.Drawing.Size(1161, 100);
            this.pnlMessage.TabIndex = 10;
            // 
            // lblCoordinate
            // 
            this.lblCoordinate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblCoordinate.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoordinate.Location = new System.Drawing.Point(0, 61);
            this.lblCoordinate.Name = "lblCoordinate";
            this.lblCoordinate.Size = new System.Drawing.Size(1161, 39);
            this.lblCoordinate.TabIndex = 1;
            this.lblCoordinate.Text = "X = 0 mm, Y = 0 mm, Z = ? Servo Value ";
            this.lblCoordinate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMessage
            // 
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMessage.Font = new System.Drawing.Font("Times New Roman", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(0, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(1161, 56);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Ready";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpeed.Location = new System.Drawing.Point(6, 63);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(156, 27);
            this.lblSpeed.TabIndex = 11;
            this.lblSpeed.Text = "Speed(mm/s) : ";
            // 
            // gbxXYMoveControl
            // 
            this.gbxXYMoveControl.Controls.Add(this.btnMove);
            this.gbxXYMoveControl.Controls.Add(this.tbxDestinationCoordinateY);
            this.gbxXYMoveControl.Controls.Add(this.lblDestinationCoordinateY);
            this.gbxXYMoveControl.Controls.Add(this.tbxDestinationCoordinateX);
            this.gbxXYMoveControl.Controls.Add(this.lblDestinationCoordInateX);
            this.gbxXYMoveControl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxXYMoveControl.Location = new System.Drawing.Point(274, 327);
            this.gbxXYMoveControl.Name = "gbxXYMoveControl";
            this.gbxXYMoveControl.Size = new System.Drawing.Size(200, 199);
            this.gbxXYMoveControl.TabIndex = 13;
            this.gbxXYMoveControl.TabStop = false;
            this.gbxXYMoveControl.Text = "X, Y Move Control";
            // 
            // btnMove
            // 
            this.btnMove.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMove.Location = new System.Drawing.Point(69, 117);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(125, 76);
            this.btnMove.TabIndex = 34;
            this.btnMove.Text = "Move";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // tbxDestinationCoordinateY
            // 
            this.tbxDestinationCoordinateY.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxDestinationCoordinateY.Location = new System.Drawing.Point(100, 74);
            this.tbxDestinationCoordinateY.Name = "tbxDestinationCoordinateY";
            this.tbxDestinationCoordinateY.Size = new System.Drawing.Size(75, 33);
            this.tbxDestinationCoordinateY.TabIndex = 33;
            // 
            // lblDestinationCoordinateY
            // 
            this.lblDestinationCoordinateY.AutoSize = true;
            this.lblDestinationCoordinateY.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDestinationCoordinateY.Location = new System.Drawing.Point(6, 79);
            this.lblDestinationCoordinateY.Name = "lblDestinationCoordinateY";
            this.lblDestinationCoordinateY.Size = new System.Drawing.Size(86, 25);
            this.lblDestinationCoordinateY.TabIndex = 32;
            this.lblDestinationCoordinateY.Text = "Y(mm) :";
            // 
            // tbxDestinationCoordinateX
            // 
            this.tbxDestinationCoordinateX.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxDestinationCoordinateX.Location = new System.Drawing.Point(100, 30);
            this.tbxDestinationCoordinateX.Name = "tbxDestinationCoordinateX";
            this.tbxDestinationCoordinateX.Size = new System.Drawing.Size(75, 33);
            this.tbxDestinationCoordinateX.TabIndex = 31;
            // 
            // lblDestinationCoordInateX
            // 
            this.lblDestinationCoordInateX.AutoSize = true;
            this.lblDestinationCoordInateX.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDestinationCoordInateX.Location = new System.Drawing.Point(6, 35);
            this.lblDestinationCoordInateX.Name = "lblDestinationCoordInateX";
            this.lblDestinationCoordInateX.Size = new System.Drawing.Size(85, 25);
            this.lblDestinationCoordInateX.TabIndex = 30;
            this.lblDestinationCoordInateX.Text = "X(mm) :";
            // 
            // gbxZControl
            // 
            this.gbxZControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxZControl.Controls.Add(this.lblServoValue);
            this.gbxZControl.Controls.Add(this.cbxServoValueInterval);
            this.gbxZControl.Controls.Add(this.lblServoValueInterval);
            this.gbxZControl.Controls.Add(this.dudZServoValueScroll);
            this.gbxZControl.Controls.Add(this.btnZUpDown);
            this.gbxZControl.Controls.Add(this.btnContact);
            this.gbxZControl.Controls.Add(this.tbxContactServoValue);
            this.gbxZControl.Controls.Add(this.btnHover);
            this.gbxZControl.Controls.Add(this.lblContactServoValue);
            this.gbxZControl.Controls.Add(this.btnTop);
            this.gbxZControl.Controls.Add(this.tbxHoverServoValue);
            this.gbxZControl.Controls.Add(this.lblTopServoValue);
            this.gbxZControl.Controls.Add(this.lblHoverServoValue);
            this.gbxZControl.Controls.Add(this.tbxTopServoValue);
            this.gbxZControl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxZControl.Location = new System.Drawing.Point(274, 528);
            this.gbxZControl.Name = "gbxZControl";
            this.gbxZControl.Size = new System.Drawing.Size(890, 199);
            this.gbxZControl.TabIndex = 14;
            this.gbxZControl.TabStop = false;
            this.gbxZControl.Text = "Z Control";
            // 
            // lblServoValue
            // 
            this.lblServoValue.AutoSize = true;
            this.lblServoValue.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServoValue.Location = new System.Drawing.Point(328, 31);
            this.lblServoValue.Name = "lblServoValue";
            this.lblServoValue.Size = new System.Drawing.Size(138, 27);
            this.lblServoValue.TabIndex = 39;
            this.lblServoValue.Text = "Servo Value :";
            // 
            // cbxServoValueInterval
            // 
            this.cbxServoValueInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxServoValueInterval.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxServoValueInterval.FormattingEnabled = true;
            this.cbxServoValueInterval.Items.AddRange(new object[] {
            "100",
            "1000",
            "5000"});
            this.cbxServoValueInterval.Location = new System.Drawing.Point(174, 27);
            this.cbxServoValueInterval.Name = "cbxServoValueInterval";
            this.cbxServoValueInterval.Size = new System.Drawing.Size(101, 34);
            this.cbxServoValueInterval.TabIndex = 38;
            // 
            // lblServoValueInterval
            // 
            this.lblServoValueInterval.AutoSize = true;
            this.lblServoValueInterval.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServoValueInterval.Location = new System.Drawing.Point(6, 31);
            this.lblServoValueInterval.Name = "lblServoValueInterval";
            this.lblServoValueInterval.Size = new System.Drawing.Size(160, 27);
            this.lblServoValueInterval.TabIndex = 37;
            this.lblServoValueInterval.Text = "Servo Interval :";
            // 
            // dudZServoValueScroll
            // 
            this.dudZServoValueScroll.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dudZServoValueScroll.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dudZServoValueScroll.Location = new System.Drawing.Point(481, 28);
            this.dudZServoValueScroll.Name = "dudZServoValueScroll";
            this.dudZServoValueScroll.ReadOnly = true;
            this.dudZServoValueScroll.Size = new System.Drawing.Size(119, 34);
            this.dudZServoValueScroll.TabIndex = 36;
            this.dudZServoValueScroll.TextChanged += new System.EventHandler(this.dudZServoValueScroll_TextChanged);
            // 
            // btnZUpDown
            // 
            this.btnZUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZUpDown.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZUpDown.Location = new System.Drawing.Point(757, 117);
            this.btnZUpDown.Name = "btnZUpDown";
            this.btnZUpDown.Size = new System.Drawing.Size(125, 76);
            this.btnZUpDown.TabIndex = 12;
            this.btnZUpDown.Text = "Up/Down";
            this.btnZUpDown.UseVisualStyleBackColor = true;
            this.btnZUpDown.Click += new System.EventHandler(this.btnZUpDown_Click);
            // 
            // btnContact
            // 
            this.btnContact.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnContact.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnContact.Location = new System.Drawing.Point(610, 117);
            this.btnContact.Name = "btnContact";
            this.btnContact.Size = new System.Drawing.Size(125, 76);
            this.btnContact.TabIndex = 11;
            this.btnContact.Text = "Contact";
            this.btnContact.UseVisualStyleBackColor = true;
            this.btnContact.Click += new System.EventHandler(this.btnContact_Click);
            // 
            // tbxContactServoValue
            // 
            this.tbxContactServoValue.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxContactServoValue.Location = new System.Drawing.Point(174, 160);
            this.tbxContactServoValue.Name = "tbxContactServoValue";
            this.tbxContactServoValue.Size = new System.Drawing.Size(100, 34);
            this.tbxContactServoValue.TabIndex = 25;
            // 
            // btnHover
            // 
            this.btnHover.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHover.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHover.Location = new System.Drawing.Point(473, 117);
            this.btnHover.Name = "btnHover";
            this.btnHover.Size = new System.Drawing.Size(125, 76);
            this.btnHover.TabIndex = 10;
            this.btnHover.Text = "Hover";
            this.btnHover.UseVisualStyleBackColor = true;
            this.btnHover.Click += new System.EventHandler(this.btnHover_Click);
            // 
            // lblContactServoValue
            // 
            this.lblContactServoValue.AutoSize = true;
            this.lblContactServoValue.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContactServoValue.Location = new System.Drawing.Point(6, 164);
            this.lblContactServoValue.Name = "lblContactServoValue";
            this.lblContactServoValue.Size = new System.Drawing.Size(161, 27);
            this.lblContactServoValue.TabIndex = 24;
            this.lblContactServoValue.Text = "Contact Servo :";
            // 
            // btnTop
            // 
            this.btnTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTop.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTop.Location = new System.Drawing.Point(333, 117);
            this.btnTop.Name = "btnTop";
            this.btnTop.Size = new System.Drawing.Size(125, 76);
            this.btnTop.TabIndex = 9;
            this.btnTop.Text = "Top";
            this.btnTop.UseVisualStyleBackColor = true;
            this.btnTop.Click += new System.EventHandler(this.btnTop_Click);
            // 
            // tbxHoverServoValue
            // 
            this.tbxHoverServoValue.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxHoverServoValue.Location = new System.Drawing.Point(174, 117);
            this.tbxHoverServoValue.Name = "tbxHoverServoValue";
            this.tbxHoverServoValue.Size = new System.Drawing.Size(100, 34);
            this.tbxHoverServoValue.TabIndex = 23;
            // 
            // lblTopServoValue
            // 
            this.lblTopServoValue.AutoSize = true;
            this.lblTopServoValue.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTopServoValue.Location = new System.Drawing.Point(6, 76);
            this.lblTopServoValue.Name = "lblTopServoValue";
            this.lblTopServoValue.Size = new System.Drawing.Size(121, 27);
            this.lblTopServoValue.TabIndex = 20;
            this.lblTopServoValue.Text = "Top Servo :";
            // 
            // lblHoverServoValue
            // 
            this.lblHoverServoValue.AutoSize = true;
            this.lblHoverServoValue.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHoverServoValue.Location = new System.Drawing.Point(6, 121);
            this.lblHoverServoValue.Name = "lblHoverServoValue";
            this.lblHoverServoValue.Size = new System.Drawing.Size(145, 27);
            this.lblHoverServoValue.TabIndex = 22;
            this.lblHoverServoValue.Text = "Hover Servo :";
            // 
            // tbxTopServoValue
            // 
            this.tbxTopServoValue.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxTopServoValue.Location = new System.Drawing.Point(174, 72);
            this.tbxTopServoValue.Name = "tbxTopServoValue";
            this.tbxTopServoValue.Size = new System.Drawing.Size(100, 34);
            this.tbxTopServoValue.TabIndex = 21;
            // 
            // gbxProcessMessage
            // 
            this.gbxProcessMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbxProcessMessage.Controls.Add(this.rtbxProcessMessage);
            this.gbxProcessMessage.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxProcessMessage.Location = new System.Drawing.Point(3, 111);
            this.gbxProcessMessage.Name = "gbxProcessMessage";
            this.gbxProcessMessage.Size = new System.Drawing.Size(268, 616);
            this.gbxProcessMessage.TabIndex = 15;
            this.gbxProcessMessage.TabStop = false;
            this.gbxProcessMessage.Text = "Message";
            // 
            // rtbxProcessMessage
            // 
            this.rtbxProcessMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxProcessMessage.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxProcessMessage.Location = new System.Drawing.Point(3, 26);
            this.rtbxProcessMessage.Name = "rtbxProcessMessage";
            this.rtbxProcessMessage.ReadOnly = true;
            this.rtbxProcessMessage.Size = new System.Drawing.Size(262, 587);
            this.rtbxProcessMessage.TabIndex = 0;
            this.rtbxProcessMessage.Text = "";
            this.rtbxProcessMessage.TextChanged += new System.EventHandler(this.rtbxProcessMessage_TextChanged);
            // 
            // lblMaxCoordinateX
            // 
            this.lblMaxCoordinateX.AutoSize = true;
            this.lblMaxCoordinateX.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxCoordinateX.Location = new System.Drawing.Point(335, 22);
            this.lblMaxCoordinateX.Name = "lblMaxCoordinateX";
            this.lblMaxCoordinateX.Size = new System.Drawing.Size(149, 27);
            this.lblMaxCoordinateX.TabIndex = 16;
            this.lblMaxCoordinateX.Text = "Max X(mm) : ";
            // 
            // tbxMaxCoordinateY
            // 
            this.tbxMaxCoordinateY.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxMaxCoordinateY.Location = new System.Drawing.Point(497, 58);
            this.tbxMaxCoordinateY.Name = "tbxMaxCoordinateY";
            this.tbxMaxCoordinateY.Size = new System.Drawing.Size(100, 34);
            this.tbxMaxCoordinateY.TabIndex = 19;
            // 
            // lblMaxCoordinateY
            // 
            this.lblMaxCoordinateY.AutoSize = true;
            this.lblMaxCoordinateY.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxCoordinateY.Location = new System.Drawing.Point(335, 63);
            this.lblMaxCoordinateY.Name = "lblMaxCoordinateY";
            this.lblMaxCoordinateY.Size = new System.Drawing.Size(148, 27);
            this.lblMaxCoordinateY.TabIndex = 18;
            this.lblMaxCoordinateY.Text = "Max Y(mm) : ";
            // 
            // tbxCOMPort
            // 
            this.tbxCOMPort.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxCOMPort.Location = new System.Drawing.Point(173, 17);
            this.tbxCOMPort.Name = "tbxCOMPort";
            this.tbxCOMPort.Size = new System.Drawing.Size(100, 34);
            this.tbxCOMPort.TabIndex = 27;
            // 
            // lblCOMPort
            // 
            this.lblCOMPort.AutoSize = true;
            this.lblCOMPort.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCOMPort.Location = new System.Drawing.Point(6, 22);
            this.lblCOMPort.Name = "lblCOMPort";
            this.lblCOMPort.Size = new System.Drawing.Size(131, 27);
            this.lblCOMPort.TabIndex = 26;
            this.lblCOMPort.Text = "COM Port : ";
            // 
            // gbxXYActionControl
            // 
            this.gbxXYActionControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxXYActionControl.Controls.Add(this.tbxDistance);
            this.gbxXYActionControl.Controls.Add(this.lblDistance);
            this.gbxXYActionControl.Controls.Add(this.btnLeft);
            this.gbxXYActionControl.Controls.Add(this.btnRight);
            this.gbxXYActionControl.Controls.Add(this.btnUp);
            this.gbxXYActionControl.Controls.Add(this.btnDown);
            this.gbxXYActionControl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxXYActionControl.Location = new System.Drawing.Point(480, 327);
            this.gbxXYActionControl.Name = "gbxXYActionControl";
            this.gbxXYActionControl.Size = new System.Drawing.Size(538, 199);
            this.gbxXYActionControl.TabIndex = 28;
            this.gbxXYActionControl.TabStop = false;
            this.gbxXYActionControl.Text = "X, Y Action Control";
            // 
            // tbxDistance
            // 
            this.tbxDistance.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxDistance.Location = new System.Drawing.Point(166, 32);
            this.tbxDistance.Name = "tbxDistance";
            this.tbxDistance.Size = new System.Drawing.Size(87, 33);
            this.tbxDistance.TabIndex = 31;
            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDistance.Location = new System.Drawing.Point(6, 36);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(153, 25);
            this.lblDistance.TabIndex = 30;
            this.lblDistance.Text = "Distance(mm) : ";
            // 
            // btnLeft
            // 
            this.btnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeft.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLeft.Location = new System.Drawing.Point(127, 117);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(125, 76);
            this.btnLeft.TabIndex = 12;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRight.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRight.Location = new System.Drawing.Point(404, 117);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(125, 76);
            this.btnRight.TabIndex = 11;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUp.Location = new System.Drawing.Point(267, 34);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(125, 76);
            this.btnUp.TabIndex = 10;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDown.Location = new System.Drawing.Point(267, 117);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(125, 76);
            this.btnDown.TabIndex = 9;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.ForeColor = System.Drawing.Color.Red;
            this.btnStop.Location = new System.Drawing.Point(1032, 444);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(125, 76);
            this.btnStop.TabIndex = 29;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // gbxSave
            // 
            this.gbxSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxSave.Controls.Add(this.btnSave);
            this.gbxSave.Controls.Add(this.cbxParameter);
            this.gbxSave.Controls.Add(this.lblParameter);
            this.gbxSave.Controls.Add(this.cbxType);
            this.gbxSave.Controls.Add(this.lblType);
            this.gbxSave.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxSave.Location = new System.Drawing.Point(274, 209);
            this.gbxSave.Name = "gbxSave";
            this.gbxSave.Size = new System.Drawing.Size(744, 112);
            this.gbxSave.TabIndex = 30;
            this.gbxSave.TabStop = false;
            this.gbxSave.Text = "Save";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(610, 51);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(125, 57);
            this.btnSave.TabIndex = 32;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbxParameter
            // 
            this.cbxParameter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxParameter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxParameter.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxParameter.FormattingEnabled = true;
            this.cbxParameter.Items.AddRange(new object[] {
            "X, Y Axis",
            "Z Axis"});
            this.cbxParameter.Location = new System.Drawing.Point(129, 69);
            this.cbxParameter.Name = "cbxParameter";
            this.cbxParameter.Size = new System.Drawing.Size(305, 30);
            this.cbxParameter.TabIndex = 42;
            // 
            // lblParameter
            // 
            this.lblParameter.AutoSize = true;
            this.lblParameter.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParameter.Location = new System.Drawing.Point(6, 73);
            this.lblParameter.Name = "lblParameter";
            this.lblParameter.Size = new System.Drawing.Size(112, 25);
            this.lblParameter.TabIndex = 41;
            this.lblParameter.Text = "Parameter :";
            // 
            // cbxType
            // 
            this.cbxType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxType.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxType.FormattingEnabled = true;
            this.cbxType.Items.AddRange(new object[] {
            "X, Y Axis(mm)",
            "Z Axis(ServoValue)"});
            this.cbxType.Location = new System.Drawing.Point(129, 29);
            this.cbxType.Name = "cbxType";
            this.cbxType.Size = new System.Drawing.Size(305, 30);
            this.cbxType.TabIndex = 40;
            this.cbxType.SelectedIndexChanged += new System.EventHandler(this.cbxType_SelectedIndexChanged);
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Font = new System.Drawing.Font("Times New Roman", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblType.Location = new System.Drawing.Point(6, 31);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(68, 25);
            this.lblType.TabIndex = 35;
            this.lblType.Text = "Type :";
            // 
            // gbxGeneral
            // 
            this.gbxGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxGeneral.Controls.Add(this.lblSpeed);
            this.gbxGeneral.Controls.Add(this.tbxSpeed);
            this.gbxGeneral.Controls.Add(this.lblMaxCoordinateX);
            this.gbxGeneral.Controls.Add(this.tbxCOMPort);
            this.gbxGeneral.Controls.Add(this.lblMaxCoordinateY);
            this.gbxGeneral.Controls.Add(this.tbxMaxCoordinateX);
            this.gbxGeneral.Controls.Add(this.lblCOMPort);
            this.gbxGeneral.Controls.Add(this.tbxMaxCoordinateY);
            this.gbxGeneral.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxGeneral.Location = new System.Drawing.Point(274, 111);
            this.gbxGeneral.Name = "gbxGeneral";
            this.gbxGeneral.Size = new System.Drawing.Size(744, 100);
            this.gbxGeneral.TabIndex = 31;
            this.gbxGeneral.TabStop = false;
            this.gbxGeneral.Text = "General";
            // 
            // tbxSpeed
            // 
            this.tbxSpeed.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxSpeed.Location = new System.Drawing.Point(173, 58);
            this.tbxSpeed.Name = "tbxSpeed";
            this.tbxSpeed.Size = new System.Drawing.Size(100, 34);
            this.tbxSpeed.TabIndex = 12;
            // 
            // tbxMaxCoordinateX
            // 
            this.tbxMaxCoordinateX.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxMaxCoordinateX.Location = new System.Drawing.Point(497, 17);
            this.tbxMaxCoordinateX.Name = "tbxMaxCoordinateX";
            this.tbxMaxCoordinateX.Size = new System.Drawing.Size(100, 34);
            this.tbxMaxCoordinateX.TabIndex = 17;
            // 
            // frmGoDrawController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1169, 733);
            this.Controls.Add(this.gbxGeneral);
            this.Controls.Add(this.gbxSave);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.gbxXYActionControl);
            this.Controls.Add(this.gbxProcessMessage);
            this.Controls.Add(this.gbxZControl);
            this.Controls.Add(this.gbxXYMoveControl);
            this.Controls.Add(this.pnlMessage);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.btnConnect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmGoDrawController";
            this.Text = "GoDraw Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGoDrawController_FormClosing);
            this.Load += new System.EventHandler(this.frmGoDrawController_Load);
            this.pnlMessage.ResumeLayout(false);
            this.gbxXYMoveControl.ResumeLayout(false);
            this.gbxXYMoveControl.PerformLayout();
            this.gbxZControl.ResumeLayout(false);
            this.gbxZControl.PerformLayout();
            this.gbxProcessMessage.ResumeLayout(false);
            this.gbxXYActionControl.ResumeLayout(false);
            this.gbxXYActionControl.PerformLayout();
            this.gbxSave.ResumeLayout(false);
            this.gbxSave.PerformLayout();
            this.gbxGeneral.ResumeLayout(false);
            this.gbxGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Panel pnlMessage;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.GroupBox gbxXYMoveControl;
        private System.Windows.Forms.GroupBox gbxZControl;
        private System.Windows.Forms.Button btnZUpDown;
        private System.Windows.Forms.Button btnContact;
        private System.Windows.Forms.Button btnHover;
        private System.Windows.Forms.Button btnTop;
        private System.Windows.Forms.GroupBox gbxProcessMessage;
        private System.Windows.Forms.RichTextBox rtbxProcessMessage;
        private System.Windows.Forms.Label lblMaxCoordinateX;
        private System.Windows.Forms.TextBox tbxMaxCoordinateY;
        private System.Windows.Forms.Label lblMaxCoordinateY;
        private System.Windows.Forms.TextBox tbxContactServoValue;
        private System.Windows.Forms.Label lblContactServoValue;
        private System.Windows.Forms.TextBox tbxHoverServoValue;
        private System.Windows.Forms.Label lblHoverServoValue;
        private System.Windows.Forms.TextBox tbxTopServoValue;
        private System.Windows.Forms.Label lblTopServoValue;
        private System.Windows.Forms.TextBox tbxCOMPort;
        private System.Windows.Forms.Label lblCOMPort;
        private System.Windows.Forms.Label lblCoordinate;
        private System.Windows.Forms.TextBox tbxDestinationCoordinateY;
        private System.Windows.Forms.Label lblDestinationCoordinateY;
        private System.Windows.Forms.TextBox tbxDestinationCoordinateX;
        private System.Windows.Forms.Label lblDestinationCoordInateX;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.GroupBox gbxXYActionControl;
        private System.Windows.Forms.TextBox tbxDistance;
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblServoValue;
        private System.Windows.Forms.ComboBox cbxServoValueInterval;
        private System.Windows.Forms.Label lblServoValueInterval;
        private System.Windows.Forms.DomainUpDown dudZServoValueScroll;
        private System.Windows.Forms.GroupBox gbxSave;
        private System.Windows.Forms.ComboBox cbxParameter;
        private System.Windows.Forms.Label lblParameter;
        private System.Windows.Forms.ComboBox cbxType;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox gbxGeneral;
        private System.Windows.Forms.TextBox tbxMaxCoordinateX;
        private System.Windows.Forms.TextBox tbxSpeed;
    }
}