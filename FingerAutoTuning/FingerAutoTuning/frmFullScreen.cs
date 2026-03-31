using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FingerAutoTuning
{
    public partial class frmFullScreen : Form
    {
        #region btnPattern handler
        public delegate void btnPatternEnableHandler(object sender, frmMain.btnPatternEnableEventArgs e);
        public event btnPatternEnableHandler btnPatternHandler;
        #endregion

        //public bool bRecordStatus = false;
        public Color cBackColor = Color.Black;
        public Color cForeColor = Color.White;
        public string sProgressStatus = "";
        public string sStepStatus = "";
        //public int nPatternType = FormMain.NONETYPE;
        //public bool bDrawLineByUser = false;
        public Bitmap BPictureImage = null;
        //public frmFullScreen(bool RecordStatus, Color BackColor, string ProgressStatus = "", string StepStatus = "", int PatternType = FormMain.NONETYPE, bool DrawLineByUser = false, Bitmap PictureImage = null)
        public frmFullScreen(string ProgressStatus, string StepStatus, Color BackColor, Bitmap PictureImage = null)
        {
            InitializeComponent();
            //bRecordStatus = RecordStatus;
            cBackColor = BackColor;
            sProgressStatus = ProgressStatus;
            sStepStatus = StepStatus;
            //nPatternType = PatternType;
            //bDrawLineByUser = DrawLineByUser;
            BPictureImage = PictureImage;

            cForeColor = Color.FromArgb(cBackColor.A, 255 - cBackColor.R, 255 - cBackColor.G, 255 - cBackColor.B);
        }

        //Modify by Jeffery(V1.0.0.0.B14 20180301)
        private void frmFullScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();

                if (btnPatternHandler != null)
                    btnPatternHandler(this, new frmMain.btnPatternEnableEventArgs(true));
            }
        }

        //Modify by Jeffery(V1.0.0.0.B14 20180301)
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();

            if (btnPatternHandler != null)
                btnPatternHandler(this, new frmMain.btnPatternEnableEventArgs(true));
        }

        private void frmFullScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(e.X, e.Y);
            }
        }

        private void frmFullScreen_Load(object sender, EventArgs e)
        {
            //double dReduceLength = 0.0;

            picPattern.Left = 0;
            picPattern.Top = 0;
            picPattern.Width = this.Width;
            picPattern.Height = this.Height;
            picPattern.Image = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppPArgb);
            picPattern.BackgroundImage = BPictureImage;
            picPattern.BackgroundImageLayout = ImageLayout.Stretch;

            using (Graphics g = Graphics.FromImage(picPattern.Image))
            {
                //DrawLine(ref dReduceLength, g, cForeColor, 50.0f, 3.0f);
                g.Flush();
            }

            int nLength = 0;

            int Width = this.Width;
            int Height = this.Height;

            string sOutputStr = sStepStatus + Environment.NewLine + sProgressStatus;

            ProgressStatusLbl.Font = new Font("Times", 30, FontStyle.Bold);
            ProgressStatusLbl.BackColor = System.Drawing.Color.Transparent;
            if (BPictureImage != null)
                ProgressStatusLbl.ForeColor = Color.Red;
            else
                ProgressStatusLbl.ForeColor = Color.Red;
            ProgressStatusLbl.Text = sOutputStr;

            int LabelWidth = ProgressStatusLbl.Width;
            int LabelHeight = ProgressStatusLbl.Height;

            /*
            if (nPatternType == FormMain.HORIZONTALLINETYPE || nPatternType == FormMain.VERTICALLINETYPE ||
                nPatternType == FormMain.CENTERPOINTTYPE ||
                nPatternType == FormMain.HORIZONTALLONGLINETYPE || nPatternType == FormMain.VERTICALLONGLINETYPE)
                nLength = Convert.ToInt32(Math.Ceiling(dReduceLength)) + (int)(LabelHeight / 2);
            */

            int SettingWidth = (int)(Width / 2) - (int)(LabelWidth / 2);
            int SettingHeight = (int)(Height / 2) - (int)(LabelHeight / 2) - nLength;

            ProgressStatusLbl.Location = new Point(SettingWidth, SettingHeight);
            ProgressStatusLbl.Parent = picPattern;
            ProgressStatusLbl.BackColor = Color.Transparent;
            ProgressStatusLbl.BringToFront();

            picPattern.Refresh();
        }

        /*
        protected void DrawLine(ref double dReduceLength, Graphics g, Color LineColor, float fMinmeter, float fPenSize)
        {
            if (nPatternType == FormMain.SLANTLINETYPE)
            {
                if (bDrawLineByUser == true)
                {
                    int TopLineStartXCoord = (int)(picPattern.Width * 0.1);
                    int TopLineStartYCoord = 0;
                    int TopLineEndXCoord = picPattern.Width;
                    int TopLineEndYCoord = (int)(picPattern.Height * 0.9);

                    int BottomLineStartXCoord = 0;
                    int BottomLineStartYCoord = (int)(picPattern.Height * 0.1);
                    int BottomLineEndXCoord = (int)(picPattern.Width * 0.9);
                    int BottomLineEndYCoord = picPattern.Height;

                    g.DrawLine(new Pen(LineColor, fPenSize), TopLineStartXCoord, TopLineStartYCoord, TopLineEndXCoord, TopLineEndYCoord);
                    g.DrawLine(new Pen(LineColor, fPenSize), BottomLineStartXCoord, BottomLineStartYCoord, BottomLineEndXCoord, BottomLineEndYCoord);
                }
            }
            else if (nPatternType == FormMain.HORIZONTALLINETYPE || nPatternType == FormMain.VERTICALLINETYPE ||
                     nPatternType == FormMain.HORIZONTALLONGLINETYPE || nPatternType == FormMain.VERTICALLONGLINETYPE)
            {
                double XHalfLength = 0.0;
                double YHalfLength = 0.0;

                if (nPatternType == FormMain.HORIZONTALLINETYPE || nPatternType == FormMain.HORIZONTALLONGLINETYPE)
                {
                    double XHalfLengthRatio = 15 / ParamAutoTuning.m_fScreenWidth;
                    XHalfLength = picPattern.Width * XHalfLengthRatio;
                    YHalfLength = picPattern.Height * 0.005;
                }
                else if (nPatternType == FormMain.VERTICALLINETYPE || nPatternType == FormMain.VERTICALLONGLINETYPE)
                {
                    XHalfLength = picPattern.Width * 0.005;
                    double YHalfLengthRatio = 15 / ParamAutoTuning.m_fScreenHeight;
                    YHalfLength = picPattern.Height * YHalfLengthRatio;
                }

                dReduceLength = YHalfLength;

                if (bDrawLineByUser == true)
                {
                    if (nPatternType == FormMain.HORIZONTALLINETYPE || nPatternType == FormMain.VERTICALLINETYPE)
                    {
                        double CenterXCoord = Math.Round((double)picPattern.Width / 2, 2, MidpointRounding.AwayFromZero);
                        double CenterYCoord = Math.Round((double)picPattern.Height / 2, 2, MidpointRounding.AwayFromZero);

                        float LeftTopXCoord = (float)Math.Round(CenterXCoord - XHalfLength, 2, MidpointRounding.AwayFromZero);
                        float LeftTopYCoord = (float)Math.Round(CenterYCoord - YHalfLength, 2, MidpointRounding.AwayFromZero);
                        float RightTopXCoord = (float)Math.Round(CenterXCoord + XHalfLength, 2, MidpointRounding.AwayFromZero);
                        float RightTopYCoord = (float)Math.Round(CenterYCoord - YHalfLength, 2, MidpointRounding.AwayFromZero);
                        float LeftBottomXCoord = (float)Math.Round(CenterXCoord - XHalfLength, 2, MidpointRounding.AwayFromZero);
                        float LeftBottomYCoord = (float)Math.Round(CenterYCoord + YHalfLength, 2, MidpointRounding.AwayFromZero);
                        float RightBottomXCoord = (float)Math.Round(CenterXCoord + XHalfLength, 2, MidpointRounding.AwayFromZero);
                        float RightBottomYCoord = (float)Math.Round(CenterYCoord + YHalfLength, 2, MidpointRounding.AwayFromZero);

                        g.DrawLine(new Pen(Color.Red, fPenSize), LeftTopXCoord, LeftTopYCoord, RightTopXCoord, RightTopYCoord);
                        g.DrawLine(new Pen(Color.Red, fPenSize), LeftTopXCoord, LeftTopYCoord, LeftBottomXCoord, LeftBottomYCoord);
                        g.DrawLine(new Pen(Color.Red, fPenSize), LeftBottomXCoord, LeftBottomYCoord, RightBottomXCoord, RightBottomYCoord);
                        g.DrawLine(new Pen(Color.Red, fPenSize), RightTopXCoord, RightTopYCoord, RightBottomXCoord, RightBottomYCoord);
                    }
                    else if (nPatternType == FormMain.HORIZONTALLONGLINETYPE)
                    {
                        double YShiftLengthRatio = ParamAutoTuning.m_fLTHorShiftYAxisCoord / ParamAutoTuning.m_fScreenHeight;
                        double YShiftLength = picPattern.Height * YShiftLengthRatio;

                        float LeftTopXCoord = 0;
                        float LeftTopYCoord = (float)Math.Round(YShiftLength - YHalfLength, 2, MidpointRounding.AwayFromZero);
                        float RightTopXCoord = picPattern.Width;
                        float RightTopYCoord = (float)Math.Round(YShiftLength - YHalfLength, 2, MidpointRounding.AwayFromZero);
                        float LeftBottomXCoord = 0;
                        float LeftBottomYCoord = (float)Math.Round(YShiftLength + YHalfLength, 2, MidpointRounding.AwayFromZero);
                        float RightBottomXCoord = picPattern.Width;
                        float RightBottomYCoord = (float)Math.Round(YShiftLength + YHalfLength, 2, MidpointRounding.AwayFromZero);

                        g.DrawLine(new Pen(Color.Red, fPenSize), LeftTopXCoord, LeftTopYCoord, RightTopXCoord, RightTopYCoord);
                        g.DrawLine(new Pen(Color.Red, fPenSize), LeftBottomXCoord, LeftBottomYCoord, RightBottomXCoord, RightBottomYCoord);
                    }
                    else if (nPatternType == FormMain.VERTICALLONGLINETYPE)
                    {
                        double XShiftLengthRatio = ParamAutoTuning.m_fLTVerShiftXAxisCoord / ParamAutoTuning.m_fScreenWidth;
                        double XShiftLength = picPattern.Width * XShiftLengthRatio;

                        float LeftTopXCoord = (float)Math.Round(XShiftLength - XHalfLength, 2, MidpointRounding.AwayFromZero);
                        float LeftTopYCoord = 0;
                        float RightTopXCoord = (float)Math.Round(XShiftLength + XHalfLength, 2, MidpointRounding.AwayFromZero);
                        float RightTopYCoord = 0;
                        float LeftBottomXCoord = (float)Math.Round(XShiftLength - XHalfLength, 2, MidpointRounding.AwayFromZero);
                        float LeftBottomYCoord = picPattern.Height;
                        float RightBottomXCoord = (float)Math.Round(XShiftLength + XHalfLength, 2, MidpointRounding.AwayFromZero);
                        float RightBottomYCoord = picPattern.Height;

                        g.DrawLine(new Pen(Color.Red, fPenSize), LeftTopXCoord, LeftTopYCoord, LeftBottomXCoord, LeftBottomYCoord);
                        g.DrawLine(new Pen(Color.Red, fPenSize), RightTopXCoord, RightTopYCoord, RightBottomXCoord, RightBottomYCoord);
                    }
                }
            }
            else if (nPatternType == FormMain.CENTERPOINTTYPE)
            {
                int XHalfLength = (int)(picPattern.Width * 0.02);
                int YHalfLength = (int)(picPattern.Height * 0.02);

                dReduceLength = YHalfLength * 2;

                if (bDrawLineByUser == true)
                {
                    double CenterXCoord = Math.Round((double)picPattern.Width / 2, 2, MidpointRounding.AwayFromZero);
                    double CenterYCoord = Math.Round((double)picPattern.Height / 2, 2, MidpointRounding.AwayFromZero);

                    float LeftTopXCoord = (float)Math.Round(CenterXCoord - XHalfLength, 2, MidpointRounding.AwayFromZero);
                    float LeftTopYCoord = (float)Math.Round(CenterYCoord - YHalfLength, 2, MidpointRounding.AwayFromZero);
                    float RightTopXCoord = (float)Math.Round(CenterXCoord + XHalfLength, 2, MidpointRounding.AwayFromZero);
                    float RightTopYCoord = (float)Math.Round(CenterYCoord - YHalfLength, 2, MidpointRounding.AwayFromZero);
                    float LeftBottomXCoord = (float)Math.Round(CenterXCoord - XHalfLength, 2, MidpointRounding.AwayFromZero);
                    float LeftBottomYCoord = (float)Math.Round(CenterYCoord + YHalfLength, 2, MidpointRounding.AwayFromZero);
                    float RightBottomXCoord = (float)Math.Round(CenterXCoord + XHalfLength, 2, MidpointRounding.AwayFromZero);
                    float RightBottomYCoord = (float)Math.Round(CenterYCoord + YHalfLength, 2, MidpointRounding.AwayFromZero);

                    g.DrawLine(new Pen(Color.Red, fPenSize), LeftTopXCoord, LeftTopYCoord, RightTopXCoord, RightTopYCoord);
                    g.DrawLine(new Pen(Color.Red, fPenSize), LeftTopXCoord, LeftTopYCoord, LeftBottomXCoord, LeftBottomYCoord);
                    g.DrawLine(new Pen(Color.Red, fPenSize), LeftBottomXCoord, LeftBottomYCoord, RightBottomXCoord, RightBottomYCoord);
                    g.DrawLine(new Pen(Color.Red, fPenSize), RightTopXCoord, RightTopYCoord, RightBottomXCoord, RightBottomYCoord);
                }
            }
        }
        */

        public void SetFrameNumberText(string sDataType, int nFrameIndex, int nFrameCount)
        {
            ReportNumberLbl.Font = new Font("Times", 14, FontStyle.Bold);
            ReportNumberLbl.BackColor = System.Drawing.Color.Transparent;

            if (BPictureImage != null)
                ReportNumberLbl.ForeColor = Color.Red;
            else
                ReportNumberLbl.ForeColor = cForeColor;

            /*
            string sMessage = string.Format("Time : {0:0.000}s", dTime);
            sMessage += Environment.NewLine + string.Format("Report Number : {0}", nReportNumber);
            */
            string sMessage = string.Format("{0} Frame : {1} / {2}", sDataType, nFrameIndex, nFrameCount);

            ReportNumberLbl.Text = sMessage;

            ReportNumberLbl.Location = new Point(picPattern.Left, picPattern.Top);
            ReportNumberLbl.Parent = picPattern;
            ReportNumberLbl.BackColor = Color.Transparent;
            ReportNumberLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            ReportNumberLbl.BringToFront();
        }

        public void SetMessageText(string sMessage)
        {
            ReportNumberLbl.Font = new Font("Times", 14, FontStyle.Bold);
            ReportNumberLbl.BackColor = System.Drawing.Color.Transparent;

            if (BPictureImage != null)
                ReportNumberLbl.ForeColor = Color.Red;
            else
                ReportNumberLbl.ForeColor = cForeColor;

            ReportNumberLbl.Text = sMessage;

            ReportNumberLbl.Location = new Point(picPattern.Left, picPattern.Top);
            ReportNumberLbl.Parent = picPattern;
            ReportNumberLbl.BackColor = Color.Transparent;
            ReportNumberLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            ReportNumberLbl.BringToFront();
        }
    }
}
