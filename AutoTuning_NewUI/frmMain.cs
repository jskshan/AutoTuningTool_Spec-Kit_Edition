using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Globalization;
using RJCodeAdvance.RJControls;
using FingerAutoTuning;
using MPPPenAutoTuning;

namespace AutoTuning_NewUI
{
    public partial class frmMain : Form
    {
        // Release the capture of the window
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        // Send a Windows message
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        //float fFirstWidth;
        //float fFirstHeight;

        // Field
        private int m_nBorderSize = 2;
        // Keep form size when it is minimized and restored.Since the form is resized because it takes into account the size of the title bar and borders.
        private Size m_structFormSize; 
        private bool m_bIsCanDraw = false;
        private Point m_structLocation;
        private Point m_structLocation_BeforeMaximum;

        FingerAutoTuning.frmMain m_cfrmFingerAutoTuning;
        MPPPenAutoTuning.frmMain m_cfrmMPPPenAutoTuning;

        public static string m_sAPVersion = null;
        private string m_sAPTitle = "";

        private enum WindowType
        {
            Maximized,
            Normal
        }

        private WindowType m_eWindowType = WindowType.Normal;

        /// <summary>
        /// Constructor for frmMain, executed when the program starts
        /// </summary>
        public frmMain()
        {
            // Initialize window
            InitializeComponent();

            // Set button image size
            SetButtonImageSize();

            /*
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            Resolution objFormResizer = new Resolution();
            objFormResizer.ResizeForm(this, screenHeight, screenWidth);
            */

            //fFirstWidth = this.Size.Width;
            //fFirstHeight = this.Size.Height;

            // Get last build time of the program
            DateTime dtLastBuild = RetrieveLinkerTimestamp();
            //DateTime dtLastBuild = File.GetCreationTime(GetType().Assembly.Location);
            //var dtLastBuild = GetBuildDate(Assembly.GetExecutingAssembly());
            //dtLastBuild = dtLastBuild.ToLocalTime();
            //DateTime dtLastBuild = AssemblyExtensions.GetLinkerTime(GetType().Assembly);

            // Get current program's file version and build time
            // string sFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
            string path = Assembly.GetExecutingAssembly().Location;
            string sFileVersion = FileVersionInfo.GetVersionInfo(path).ProductVersion; // ← 讀取 InformationalVersion
            string sLastBuild = dtLastBuild.ToString("yyyyMMdd-HHmmss");

            // Combine to form program version string
            m_sAPVersion = string.Format("{0} ({1})", sFileVersion, sLastBuild);
#if _USE_9F07_SOCKET
            m_sAPTitle = string.Format("AutoTuningTool_DirectTouch V{0}", m_sAPVersion);
#else
            m_sAPTitle = string.Format("AutoTuningTool V{0}", m_sAPVersion);
#endif

            // Set window title and title bar text
            this.Text = m_sAPTitle;
            lblTitle.Text = m_sAPTitle;

            // Collapse menu
            CollapseMenu();

            // Border Size
            this.Padding = new Padding(m_nBorderSize);
            // Border Color
            this.BackColor = Color.FromArgb(98, 102, 244);

#if _USE_9F07_SOCKET
            btnMPPPen.Visible = false;
#endif
        }

        /// <summary>
        /// Set the size of the button images
        /// </summary>
        private void SetButtonImageSize()
        {
            // Get the Finger Type button image and resize it
            Bitmap btFingerImage = btnFinger.Image as Bitmap;
            Bitmap btFingerResize = new Bitmap(btFingerImage, new Size(30, 35));
            btnFinger.Image = btFingerResize;
            btnFinger.ImageAlign = ContentAlignment.MiddleLeft;
            btnFinger.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnFinger.TextAlign = ContentAlignment.MiddleRight;

            // Get the MPP Pen Type button image and resize it
            Bitmap btMPPPenImage = btnMPPPen.Image as Bitmap;
            Bitmap btMPPPenResize = new Bitmap(btMPPPenImage, new Size(30, 35));
            btnMPPPen.Image = btMPPPenResize;
            btnMPPPen.ImageAlign = ContentAlignment.MiddleLeft;
            btnMPPPen.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnMPPPen.TextAlign = ContentAlignment.MiddleRight;
        }

        /// <summary>
        /// Retrieves the linker timestamp of the current assembly
        /// </summary>
        /// <returns>The build time of the assembly</returns>
        private DateTime RetrieveLinkerTimestamp()
        {
            // Get the path of the current assembly
            string sFilePath = Assembly.GetCallingAssembly().Location;
            // Offset of the PE header
            const int nPeHeaderOffset = 60;
            // Offset of the linker timestamp
            const int nLinkerTimestampOffset = 8;
            // Size of the data stream buffer
            byte[] byteData_Array = new byte[2048];
            // Stream reader
            Stream streamData = null;

            try
            {
                // Open the stream of the assembly
                streamData = new FileStream(sFilePath, FileMode.Open, FileAccess.Read);
                // Read the data into the buffer
                streamData.Read(byteData_Array, 0, 2048);
            }
            finally
            {
                // Close the stream
                if (streamData != null)
                {
                    streamData.Close();
                }
            }

            // Calculate the position of the PE header data
            int nConvertData = BitConverter.ToInt32(byteData_Array, nPeHeaderOffset);
            // Calculate the seconds since January 1, 1970 until the linker timestamp
            int nSecondsSince1970 = BitConverter.ToInt32(byteData_Array, nConvertData + nLinkerTimestampOffset);
            // Convert the seconds to UTC time
            DateTime dtBuildTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dtBuildTime = dtBuildTime.AddSeconds(nSecondsSince1970);
            // Adjust the UTC time to local time
            dtBuildTime = dtBuildTime.AddHours(8);

            // Return the assembly build time
            return dtBuildTime;
        }

        /// <summary>
        ///Get the build date and time of the assembly based on the given Assembly object
        /// </summary>
        /// <param name="cAssembly">The assembly object to retrieve build date and time from</param>
        /// <returns>The build date and time of the assembly, or current time if it can't be retrieved</returns>
        private DateTime GetBuildDate(Assembly cAssembly)
        {
            // Default build date and time is current time
            DateTime dtBuildTime = DateTime.Now;
            // The prefix of the version information for build date and time
            const string sBuildVersionMetadataPrefix = "+build";

            // Get the version information attribute
            var vAttribute = cAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (vAttribute.InformationalVersion != null)
            {
                // Get the build date and time from the version information
                var vValue = vAttribute.InformationalVersion;
                var vIndex = vValue.IndexOf(sBuildVersionMetadataPrefix);

                if (vIndex > 0)
                {
                    // Get the string representation of the build date and time
                    vValue = vValue.Substring(vIndex + sBuildVersionMetadataPrefix.Length);

                    // Convert the string representation to DateTime object
                    if (DateTime.TryParseExact(vValue, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtBuildTime))
                        return dtBuildTime;
                }
            }

            return dtBuildTime;
        }

        /// <summary>
        /// Recieve Windows Message
        /// </summary>
        /// <param name="m">Windows Message</param>
        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x0083;   //Standar Title Bar - Snap Window
            const int WM_SYSCOMMAND = 0x0112;   //System Message - Window Command
            const int SC_MINIMIZE = 0xF020;     //Minimize form (Before)
            const int SC_RESTORE = 0xF120;      //Restore form (Before)
            const int WM_NCHITTEST = 0x0084;    //Win32, Mouse Input Notification: Determine what part of the window corresponds to a point, allows to resize the form.
            const int resizeAreaSize = 10;      //Resize Window Size

            #region Form Resize
            //Resize/WM_NCHITTEST values
            const int HTCLIENT = 1;         //Represents the client area of the window
            const int HTLEFT = 10;          //Left border of a window, allows resize horizontally to the left
            const int HTRIGHT = 11;         //Right border of a window, allows resize horizontally to the right
            const int HTTOP = 12;           //Upper-horizontal border of a window, allows resize vertically up
            const int HTTOPLEFT = 13;       //Upper-left corner of a window border, allows resize diagonally to the left
            const int HTTOPRIGHT = 14;      //Upper-right corner of a window border, allows resize diagonally to the right
            const int HTBOTTOM = 15;        //Lower-horizontal border of a window, allows resize vertically down
            const int HTBOTTOMLEFT = 16;    //Lower-left corner of a window border, allows resize diagonally to the left
            const int HTBOTTOMRIGHT = 17;   //Lower-right corner of a window border, allows resize diagonally to the right
                                            ///<Doc> More Information: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest </Doc>
            if (m.Msg == WM_NCHITTEST)
            { 
                //If the windows m is WM_NCHITTEST
                base.WndProc(ref m);

                //Resize the form if it is in normal state
                if (this.WindowState == FormWindowState.Normal)
                {
                    //If the result of the m (mouse pointer) is in the client area of the window
                    if ((int)m.Result == HTCLIENT)
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32());  //Gets screen point coordinates(X and Y coordinate of the pointer)                           
                        Point clientPoint = this.PointToClient(screenPoint);    //Computes the location of the screen point into client coordinates                          

                        //If the pointer is at the top of the form (within the resize area- X coordinate)
                        if (clientPoint.Y <= resizeAreaSize)   
                        {
                            //If the pointer is at the coordinate X=0 or less than the resizing area(X=10) in
                            if (clientPoint.X <= resizeAreaSize)    
                                m.Result = (IntPtr)HTTOPLEFT;   //Resize diagonally to the left
                            //If the pointer is at the coordinate X=11 or less than the width of the form(X=Form.Width-resizeArea)
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))
                                m.Result = (IntPtr)HTTOP;   //Resize vertically up
                            //Resize diagonally to the right
                            else
                                m.Result = (IntPtr)HTTOPRIGHT;
                        }
                        //If the pointer is inside the form at the Y coordinate(discounting the resize area size)
                        else if (clientPoint.Y <= (this.Size.Height - resizeAreaSize))
                        {
                            //Resize horizontally to the left
                            if (clientPoint.X <= resizeAreaSize)
                                m.Result = (IntPtr)HTLEFT;
                            //Resize horizontally to the right
                            else if (clientPoint.X > (this.Width - resizeAreaSize))
                                m.Result = (IntPtr)HTRIGHT;
                        }
                        else
                        {
                            //Resize diagonally to the left
                            if (clientPoint.X <= resizeAreaSize)
                                m.Result = (IntPtr)HTBOTTOMLEFT;
                            //Resize vertically down
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))
                                m.Result = (IntPtr)HTBOTTOM;
                            //Resize diagonally to the right
                            else 
                                m.Result = (IntPtr)HTBOTTOMRIGHT;
                        }
                    }
                }
                return;
            }
            #endregion

            // Remove border and keep snap window
            if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                return;
            }

            // Keep form size when it is minimized and restored. Since the form is resized because it takes into account the size of the title bar and borders.
            if (m.Msg == WM_SYSCOMMAND)
            {
                /// <see cref="https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand"/>
                /// Quote:
                /// In WM_SYSCOMMAND messages, the four low - order bits of the wParam parameter 
                /// are used internally by the system.To obtain the correct result when testing 
                /// the value of wParam, an application must combine the value 0xFFF0 with the 
                /// wParam value by using the bitwise AND operator.
                int wParam = (m.WParam.ToInt32() & 0xFFF0);

                // Before
                if (wParam == SC_MINIMIZE)
                    m_structFormSize = this.ClientSize;

                // Restored form(Before)
                if (wParam == SC_RESTORE)
                    this.Size = m_structFormSize;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// When the main form's size is changed, execute the following code
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The event</param>
        private void frmMain_Resize(object sender, EventArgs e)
        {
            // Adjust the form size
            AdjustForm();
        }

        /// <summary>
        /// Adjust the form size
        /// </summary>
        private void AdjustForm()
        {
            switch (this.WindowState)
            {
                // If the window is maximized
                case FormWindowState.Maximized:
                    // Set the padding on the top, left, right, and bottom sides
                    this.Padding = new Padding(8, 8, 8, 0);
                    break;
                // If the window is restored
                case FormWindowState.Normal:
                    // If the size of the top border is not equal to the specified size
                    if (this.Padding.Top != m_nBorderSize)
                    {
                        // Set the padding on the top, left, right, and bottom sides to the specified size
                        this.Padding = new Padding(m_nBorderSize);
                    }

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Collapse the menu
        /// </summary>
        private void CollapseMenu()
        {
            // Collapse the menu
            if (this.pnlMenu.Width > 60)
            {
                pnlMenu.Width = 60;
                picbxIcon.Visible = false;
                // Dock the button to the top
                btnMenu.Dock = DockStyle.Top;
                // Set the button's background image
                btnMenu.BackgroundImage = Properties.Resources.drop_down_menu;
                // Set the button's background image layout
                btnMenu.BackgroundImageLayout = ImageLayout.Zoom;

                // Loop through the buttons in the menu
                foreach (Button btnControl in pnlMenu.Controls.OfType<Button>())
                {
                    // Clear the button text
                    btnControl.Text = "";
                    // Set the button's image alignment
                    btnControl.ImageAlign = ContentAlignment.MiddleCenter;
                    // Set the button's content padding
                    btnControl.Padding = new Padding(0);
                }
            }
            // Expand the menu
            else
            { 
                pnlMenu.Width = 150;
                picbxIcon.Visible = true;
                // Undock the button
                btnMenu.Dock = DockStyle.None;
                // Set the button's left position
                btnMenu.Left = 150 - btnMenu.Width;
                // Set the button's background image
                btnMenu.BackgroundImage = Properties.Resources.arrow_left;
                //btnMenu.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipY);
                // Set the button's background image layout
                btnMenu.BackgroundImageLayout = ImageLayout.Zoom;

                // Loop through the buttons in the menu
                foreach (Button btnControl in pnlMenu.Controls.OfType<Button>())
                {
                    // Set the button's text with a space and the value of the Tag property
                    btnControl.Text = "   " + btnControl.Tag.ToString();
                    // Set the button's image alignment
                    btnControl.ImageAlign = ContentAlignment.MiddleLeft;
                    // Set the button's content padding
                    btnControl.Padding = new Padding(10, 0, 0, 0);
                }
            }
        }

        /// <summary>
        ///  Mouse down event for "pnlTitleBar" control
        /// </summary>
        /// <param name="sender">Control object</param>
        /// <param name="e">Mouse event</param>
        private void pnlTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            // If the mouse click count is not 1, drawing is not allowed
            if (e.Clicks != 1)
            {
                m_bIsCanDraw = false;
            }
            else
            {
                m_bIsCanDraw = true;
            }

            // If drawing is allowed
            if (m_bIsCanDraw == true)
            {
                // Release mouse capture
                ReleaseCapture();
                // Send move window message
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        /// <summary>
        /// Mouse double-click event for "pnlTitleBar" control
        /// </summary>
        /// <param name="sender">Control object</param>
        /// <param name="e">Mouse event</param>
        private void pnlTitleBar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // If the window is in normal size
            if (m_eWindowType == WindowType.Normal)
            {
                // Save the current form size and location
                m_structFormSize = this.ClientSize;
                m_structLocation = this.Location;
                m_structLocation_BeforeMaximum = this.Location;
                //this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                //this.WindowState = FormWindowState.Maximized;

                // Set the window height and width to the working area of the primary screen
                this.Height = Screen.PrimaryScreen.WorkingArea.Height;
                this.Width = Screen.PrimaryScreen.WorkingArea.Width;
                // Set the window location to the working area of the primary screen
                this.Location = Screen.PrimaryScreen.WorkingArea.Location;
                // Change the window type to maximized
                m_eWindowType = WindowType.Maximized;
            }
            // If the window is already maximized
            else 
            {
                // Restore the window to its normal size and location
                this.WindowState = FormWindowState.Normal;
                this.Size = m_structFormSize;
                this.Location = m_structLocation_BeforeMaximum;
                // Change the window type to normal
                m_eWindowType = WindowType.Normal;
            }
        }

        /// <summary>
        /// Mouse Move event for the "pnlTitleBar" control
        /// </summary>
        /// <param name="sender">Control object</param>
        /// <param name="e">Mouse event</param>
        private void pnlTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            // If the left mouse button is pressed and the title bar can be drawn
            if (e.Button == MouseButtons.Left & m_bIsCanDraw)
            {
                // Perform drag and drop operation
                this.DoDragDrop(this, DragDropEffects.Move);
                // Disable IsCanDraw method until the next drag and drop operation
                m_bIsCanDraw = false;
            }
        }

        /// <summary>
        /// Event for size change of "pnlContent" control
        /// </summary>
        /// <param name="sender">Control object</param>
        /// <param name="e">Event arguments</param>
        private void pnlContent_SizeChanged(object sender, EventArgs e)
        {
            // If pnlContent contains m_cfrmFingerAutoTuning control
            if (pnlContent.Controls.Contains(m_cfrmFingerAutoTuning) == true)
            {
                // Adjust the position and size of m_cfrmFingerAutoTuning to match pnlContent
                m_cfrmFingerAutoTuning.Top = pnlContent.Top;
                m_cfrmFingerAutoTuning.Left = pnlContent.Left;
                m_cfrmFingerAutoTuning.Width = pnlContent.Width;
                m_cfrmFingerAutoTuning.Height = pnlContent.Height;
            }

            // If pnlContent contains m_cfrmMPPPenAutoTuning control
            if (pnlContent.Controls.Contains(m_cfrmMPPPenAutoTuning) == true)
            {
                // Adjust the position and size of m_cfrmMPPPenAutoTuning to match pnlContent
                m_cfrmMPPPenAutoTuning.Top = pnlContent.Top;
                m_cfrmMPPPenAutoTuning.Left = pnlContent.Left;
                m_cfrmMPPPenAutoTuning.Width = pnlContent.Width;
                m_cfrmMPPPenAutoTuning.Height = pnlContent.Height;
            }
        }

        /// <summary>
        /// Mouse down event for "lblTitle" control
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event arguments</param>
        private void lblTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks != 1)
            {
                // If the number of clicks is not 1, disable drawing the title bar
                m_bIsCanDraw = false;
            }
            else
            {
                // If the number of clicks is 1, enable drawing the title bar
                m_bIsCanDraw = true;
            }

            if (m_bIsCanDraw == true)
            {
                // Release the capture of the mouse by the application to allow dragging the window
                ReleaseCapture();
                // Send a message to drag the window
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        /// <summary>
        /// Mouse move event for "lblTitle" control
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event arguments</param>
        private void lblTitle_MouseMove(object sender, MouseEventArgs e)
        {
            // If left button is pressed and title bar can be drawn
            if (e.Button == MouseButtons.Left & m_bIsCanDraw)
            {
                // Start window dragging
                this.DoDragDrop(this, DragDropEffects.Move);
                // Disable IsCanDraw until next drag operation
                m_bIsCanDraw = false;
            }
        }

        /// <summary>
        /// Mouse Double-click event for "lblTitle" control
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event arguments</param>
        private void lblTitle_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (m_eWindowType == WindowType.Normal)
            {
                // Save the original size and location of the window
                m_structFormSize = this.ClientSize;
                m_structLocation = this.Location;
                m_structLocation_BeforeMaximum = this.Location;
                //this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                //this.WindowState = FormWindowState.Maximized;

                // Maximize the window to the size of the screen
                this.Height = Screen.PrimaryScreen.WorkingArea.Height;
                this.Width = Screen.PrimaryScreen.WorkingArea.Width;
                this.Location = Screen.PrimaryScreen.WorkingArea.Location;
                m_eWindowType = WindowType.Maximized;
            }
            else
            {
                // Restore the window to its original size and location
                this.WindowState = FormWindowState.Normal;
                this.Size = m_structFormSize;
                this.Location = m_structLocation_BeforeMaximum;
                m_eWindowType = WindowType.Normal;
            }
        }

        /// <summary>
        /// Control event for when the size of the "frmMain" form changes
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event arguments</param>
        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            /*
            float size1 = this.Size.Width / fFirstWidth;
            float size2 = this.Size.Height / fFirstHeight;

            SizeF scale = new SizeF(size1, size2);
            fFirstWidth = this.Size.Width;
            fFirstHeight = this.Size.Height;

            foreach (Control control in this.Controls)
            {
                control.Font = new Font(control.Font.FontFamily, control.Font.Size * ((size1 + size2) / 2));
                control.Scale(scale);
            }
            */
        }

        /// <summary>
        /// Control event for "frmMain" form activation
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">Event arguments</param>
        private void frmMain_Activated(object sender, EventArgs e)
        {
            // If the window is minimized
            if (this.WindowState == FormWindowState.Minimized)
            {
                // Restore the size and location based on the window's previous state
                if (m_eWindowType == WindowType.Normal)
                {
                    this.Size = m_structFormSize;
                    this.Location = m_structLocation;
                }
                else
                {
                    m_structFormSize = this.ClientSize;
                    m_structLocation = this.Location;
                    //this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                    //this.WindowState = FormWindowState.Maximized;

                    // Maximize the window to the size of the screen
                    this.Height = Screen.PrimaryScreen.WorkingArea.Height;
                    this.Width = Screen.PrimaryScreen.WorkingArea.Width;
                    this.Location = Screen.PrimaryScreen.WorkingArea.Location;
                }
            }
        }

        /// <summary>
        /// Control event for when the "frmMain" form is closed. Exits the application when the main form is closed. If there is an error when exiting the application, no action is taken.
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event arguments</param>
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Environment.Exit(Environment.ExitCode);
            }
            catch
            { 
            }
        }

        /// <summary>
        /// Set the background color of the button
        /// </summary>
        /// <param name="btnSelectControl">The selected button control component</param>
        private void SetButtonBackColor(Button btnSelectControl)
        {
            // Declare a button array
            Button[] btnFeature_Array = new Button[]
            {
                btnFinger,
                btnMPPPen
            };

            // Iterate through the button array and set the background color
            foreach (Button btnControl in btnFeature_Array)
            {
                if (btnControl.Tag == btnSelectControl.Tag)
                    btnControl.BackColor = Color.Purple;
                else
                    btnControl.BackColor = Color.FromArgb(92, 102, 244);
            }
        }

        /// <summary>
        /// Event handler for "Finger" button click event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event</param>
        private void btnFinger_Click(object sender, EventArgs e)
        {
            // Set button background color
            SetButtonBackColor(btnFinger);

            // If MPPPen window exists, remove it
            if (pnlContent.Controls.Contains(m_cfrmMPPPenAutoTuning) == true)
            {
                pnlContent.Controls.Remove(m_cfrmMPPPenAutoTuning);
                //m_cfrmMPPPenAutoTuning = null;
            }

            // If Finger window doesn't exist, create it
            if (m_cfrmFingerAutoTuning == null)
            {
                m_cfrmFingerAutoTuning = new FingerAutoTuning.frmMain(m_sAPVersion, m_sAPTitle, true);
                m_cfrmFingerAutoTuning.MdiParent = this;
                m_cfrmFingerAutoTuning.TopLevel = false;
                m_cfrmFingerAutoTuning.TopMost = true;
                m_cfrmFingerAutoTuning.FormBorderStyle = FormBorderStyle.None;
                m_cfrmFingerAutoTuning.Top = pnlContent.Top;
                m_cfrmFingerAutoTuning.Left = pnlContent.Left;
                m_cfrmFingerAutoTuning.Width = pnlContent.Width;
                m_cfrmFingerAutoTuning.Height = pnlContent.Height;
                m_cfrmFingerAutoTuning.BringToFront();

                //m_cfrmFingerAutoTuning.AutoScroll = true;
                /*
                m_cfrmFingerAutoTuning.WindowState = FormWindowState.Maximized;
                m_cfrmFingerAutoTuning.MaximizeBox = false;
                m_cfrmFingerAutoTuning.MinimizeBox = false;
                m_cfrmFingerAutoTuning.BringToFront();
                m_cfrmFingerAutoTuning.ControlBox = false;
                m_cfrmFingerAutoTuning.FormClosed += new FormClosedEventHandler(FingerForm_FormClosed);
                */
                pnlContent.Controls.Add(m_cfrmFingerAutoTuning);
                m_cfrmFingerAutoTuning.Show();
            }
            // If Finger window already exists, show it
            else
            {
                //m_cfrmFingerAutoTuning = null;
                m_cfrmFingerAutoTuning.MdiParent = this;
                m_cfrmFingerAutoTuning.TopLevel = false;
                m_cfrmFingerAutoTuning.TopMost = true;
                m_cfrmFingerAutoTuning.FormBorderStyle = FormBorderStyle.None;
                m_cfrmFingerAutoTuning.Top = pnlContent.Top;
                m_cfrmFingerAutoTuning.Left = pnlContent.Left;
                m_cfrmFingerAutoTuning.Width = pnlContent.Width;
                m_cfrmFingerAutoTuning.Height = pnlContent.Height;
                m_cfrmFingerAutoTuning.BringToFront();

                pnlContent.Controls.Add(m_cfrmFingerAutoTuning);
                m_cfrmFingerAutoTuning.Show();
            }

            // Set title text
            lblTitle.Text = string.Format("{0}_Finger(V{1})", m_sAPTitle, m_cfrmFingerAutoTuning.m_sFileVersion);
        }

        /// <summary>
        /// Event handler for "MPP Pen" button click event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event</param>
        private void btnMPPPen_Click(object sender, EventArgs e)
        {
            // Set the background color of the button
            SetButtonBackColor(btnMPPPen);

            // Check if the "m_cfrmFingerAutoTuning" form exists and remove it if it does
            if (pnlContent.Controls.Contains(m_cfrmFingerAutoTuning) == true)
            {
                pnlContent.Controls.Remove(m_cfrmFingerAutoTuning);
                //m_cfrmFingerAutoTuning = null;
            }

            // Create the "m_cfrmMPPPenAutoTuning" form if it doesn't exist
            if (m_cfrmMPPPenAutoTuning == null)
            {
                m_cfrmMPPPenAutoTuning = new MPPPenAutoTuning.frmMain(m_sAPVersion, m_sAPTitle, true);
                m_cfrmMPPPenAutoTuning.MdiParent = this;
                m_cfrmMPPPenAutoTuning.TopLevel = false;
                m_cfrmMPPPenAutoTuning.TopMost = true;
                m_cfrmMPPPenAutoTuning.FormBorderStyle = FormBorderStyle.None;
                m_cfrmMPPPenAutoTuning.Top = pnlContent.Top;
                m_cfrmMPPPenAutoTuning.Left = pnlContent.Left;
                m_cfrmMPPPenAutoTuning.Width = pnlContent.Width;
                m_cfrmMPPPenAutoTuning.Height = pnlContent.Height;
                m_cfrmMPPPenAutoTuning.BringToFront();

                //m_cfrmMPPPenAutoTuning.AutoScroll = true;
                /*
                m_cfrmMPPPenAutoTuning.WindowState = FormWindowState.Maximized;
                m_cfrmMPPPenAutoTuning.MaximizeBox = false;
                m_cfrmMPPPenAutoTuning.MinimizeBox = false;
                m_cfrmMPPPenAutoTuning.BringToFront();
                m_cfrmMPPPenAutoTuning.ControlBox = false;
                m_cfrmMPPPenAutoTuning.FormClosed += new FormClosedEventHandler(MSPenForm_FormClosed);
                */
                pnlContent.Controls.Add(m_cfrmMPPPenAutoTuning);
                m_cfrmMPPPenAutoTuning.Show();
            }
            // If the "m_cfrmMPPPenAutoTuning" form exists, show it
            else
            {
                //m_cfrmMPPPenAutoTuning = null;
                m_cfrmMPPPenAutoTuning.MdiParent = this;
                m_cfrmMPPPenAutoTuning.TopLevel = false;
                m_cfrmMPPPenAutoTuning.TopMost = true;
                m_cfrmMPPPenAutoTuning.FormBorderStyle = FormBorderStyle.None;
                m_cfrmMPPPenAutoTuning.Top = pnlContent.Top;
                m_cfrmMPPPenAutoTuning.Left = pnlContent.Left;
                m_cfrmMPPPenAutoTuning.Width = pnlContent.Width;
                m_cfrmMPPPenAutoTuning.Height = pnlContent.Height;


                pnlContent.Controls.Add(m_cfrmMPPPenAutoTuning);
                m_cfrmMPPPenAutoTuning.Show();
            }

            // Set the title text
            lblTitle.Text = string.Format("{0}_MPP Pen(V{1})", m_sAPTitle, m_cfrmMPPPenAutoTuning.m_sFileVersion);
        }

        /// <summary>
        /// Event handler for the "Minimize" button click event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event</param>
        private void btnMinimum_Click(object sender, EventArgs e)
        {
            // Save the current size and location of the form
            m_structFormSize = this.ClientSize;
            m_structLocation = this.Location;

            // Set the form's state to minimized
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Event handler for the "Maximum" button click event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event</param>
        private void btnMaximum_Click(object sender, EventArgs e)
        {
            if (m_eWindowType == WindowType.Normal)
            {
                // Save the current size and location of the form
                m_structFormSize = this.ClientSize;
                m_structLocation = this.Location;
                m_structLocation_BeforeMaximum = this.Location;
                //this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                //this.WindowState = FormWindowState.Maximized;

                // Set the size of the form to the size of the screen's working area
                this.Height = Screen.PrimaryScreen.WorkingArea.Height;
                this.Width = Screen.PrimaryScreen.WorkingArea.Width;
                // Set the location of the form to the location of the screen's working area
                this.Location = Screen.PrimaryScreen.WorkingArea.Location;
                // Set the window type to "Maximized"
                m_eWindowType = WindowType.Maximized;
            }
            else
            {
                // Set the window state to "Normal"
                this.WindowState = FormWindowState.Normal;
                // Restore the size and location of the form
                this.Size = m_structFormSize;
                this.Location = m_structLocation_BeforeMaximum;
                // Set the window type to "Normal"
                m_eWindowType = WindowType.Normal;
            }
        }

        /// <summary>
        /// Event handler for the "Close" button click event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Event handler for the "Menu" button click event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Event</param>
        private void btnMenu_Click(object sender, EventArgs e)
        {
            CollapseMenu();
        }

        private void frmMain_Move(object sender, EventArgs e)
        {
            m_structLocation = this.Location;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //獲取主屏幕的大小
            int nScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int nScreenHeight = Screen.PrimaryScreen.Bounds.Height;

            if (nScreenWidth < this.Width)
            {
                this.Width = nScreenWidth;
                this.Left = 0;
            }

            if (nScreenHeight < this.Height)
            {
                this.Height = nScreenHeight;
                this.Top = 0;
            }
        }
    }

    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the linker time of an assembly
        /// </summary>
        /// <param name="cAssembly">The assembly to get the linker time for</param>
        /// <returns>The linker time of the assembly</returns>
        public static DateTime GetLinkerTime(this Assembly assembly)
        {
            // Returns the local time of the last write time of the assembly's location file
            return File.GetLastWriteTime(assembly.Location).ToLocalTime();
        }
    }

    public class Resolution
    {
        float m_fHeightRatio = new float();
        float m_fWidthRatio = new float();
        int m_nStandardHeight, m_nStandardWidth;

        /// <summary>
        /// Resize the form and its child controls based on the current screen resolution and the original design size
        /// </summary>
        /// <param name="cfrmForm">The form to be resized</param>
        /// <param name="nDesignerHeight">The original design height of the form</param>
        /// <param name="nDesignerWidth">The original design width of the form</param>
        public void ResizeForm(Form cfrmForm, int nDesignerHeight, int nDesignerWidth)
        {
            // Store the designer's height and width of the form
            m_nStandardHeight = nDesignerHeight;
            m_nStandardWidth = nDesignerWidth;

            // Get the current screen resolution
            int nPresentHeight = Screen.PrimaryScreen.WorkingArea.Height;    //.Bounds.Height;
            int nPresentWidth = Screen.PrimaryScreen.Bounds.Width;

            // Calculate the scaling ratio
            m_fHeightRatio = (float)((float)nPresentHeight / (float)m_nStandardHeight);
            m_fWidthRatio = (float)((float)nPresentWidth / (float)m_nStandardWidth);

            // Set the form's automatic scaling mode
            cfrmForm.AutoScaleMode = AutoScaleMode.None;

            // Scale the size of the form and its child controls
            cfrmForm.Scale(new SizeF(m_fWidthRatio, m_fHeightRatio));

            // Recursively scale all child controls
            foreach (Control cControl in cfrmForm.Controls)
            {
                if (cControl.HasChildren == true)
                {
                    ResizeControl(cControl);
                }
                else
                {
                    // Set the font size of the control
                    cControl.Font = new Font(cControl.Font.FontFamily, cControl.Font.Size * m_fHeightRatio, cControl.Font.Style, cControl.Font.Unit, ((byte)(0)));
                }
            }

            // Set the font size of the form
            cfrmForm.Font = new Font(cfrmForm.Font.FontFamily, cfrmForm.Font.Size * m_fHeightRatio, cfrmForm.Font.Style, cfrmForm.Font.Unit, ((byte)(0)));
        }

        /// <summary>
        /// Resizes a control and its child controls to match the current screen size
        /// </summary>
        /// </summary>
        /// <param name="cControl">The control to resize</param>
        private void ResizeControl(Control cControl)
        {
            // Check if the control has child controls
            if (cControl.HasChildren == true)
            {
                // Recursively search child controls
                foreach (Control cChildrenControl in cControl.Controls)
                {
                    if (cChildrenControl.HasChildren == true)
                    {
                        // If the child control has child controls, recursively resize them
                        ResizeControl(cChildrenControl);
                    }
                    else
                    {
                        // If the child control has no child controls, adjust the font size
                        cChildrenControl.Font = new Font(cChildrenControl.Font.FontFamily, cChildrenControl.Font.Size * m_fHeightRatio, cChildrenControl.Font.Style, cChildrenControl.Font.Unit, ((byte)(0)));
                    }
                }

                // Adjust the font size of the parent control
                cControl.Font = new Font(cControl.Font.FontFamily, cControl.Font.Size * m_fHeightRatio, cControl.Font.Style, cControl.Font.Unit, ((byte)(0)));
            }
            else
            {
                // If the control has no child controls, adjust the font size
                cControl.Font = new Font(cControl.Font.FontFamily, cControl.Font.Size * m_fHeightRatio, cControl.Font.Style, cControl.Font.Unit, ((byte)(0)));
            }
        }
    }
}
