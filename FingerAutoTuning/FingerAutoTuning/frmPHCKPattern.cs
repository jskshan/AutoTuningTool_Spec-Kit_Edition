using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    public partial class frmPHCKPattern : Form
    {
        protected ParamEnvironment m_APsetting = new ParamEnvironment();

        #region btnPattern handler
        public delegate void btnPatternEnableHandler(object sender, frmMain.btnPatternEnableEventArgs e);
        public event btnPatternEnableHandler btnPatternHandler;
        #endregion

        /*
        #region ScreenSizeLbl handler
        public delegate void ScreenSizeLblOutputHandler(object sender, frmParamSetting.ScreenSizeLblEventArgs e);
        public event ScreenSizeLblOutputHandler ScreenSizeLblHandler;
        #endregion
        */

        //public bool bRecordStatus = false;
        //public bool bPreviewSetting = false;
        public string sProgressStatus = "";
        public string sStepStatus = "";
        //public int nPatternType = FormMain.NONETYPE;
        //public bool bDrawLineByUser = false;
        public byte[] byteEDIDData = null;
        public EDIDInfo cEDIDInformation = null;
        protected Bitmap m_bmpDrawing;
        //public frmPHCKPattern(bool RecordStatus, byte[] EDIDData, EDIDInfo EDIDInformation, string ProgressStatus = "", string StepStatus = "", int PatternType = FormMain.NONETYPE, bool DrawLineByUser = false, ParamEnvironment APsetting = null, bool PreviewSetting = false)
        public frmPHCKPattern(string ProgressStatus, string StepStatus, byte[] EDIDData, EDIDInfo EDIDInformation)
        {
            InitializeComponent();

            //bRecordStatus = RecordStatus;
            byteEDIDData = EDIDData;
            cEDIDInformation = EDIDInformation;
            sProgressStatus = ProgressStatus;
            sStepStatus = StepStatus;
            //nPatternType = PatternType;
            //bDrawLineByUser = DrawLineByUser;
            //bPreviewSetting = PreviewSetting;
            /*
            if (APsetting != null)
                m_APsetting = APsetting;
            */

        }

        private void OnFrmLoad(object sender, EventArgs e)
        {
            /*
            if (bPreviewSetting == false)
                xLoadSetting();
            else
                m_APsetting.GetParam();
            */

            //Let the Windows Full Screen
            Left = 0;
            Top = 0;
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;

            //set the Picture Box Width and Height
            picPattern.Left = 0;
            picPattern.Top = 0;
            picPattern.Width = Width;
            picPattern.Height = Height;
            picPattern.Image = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            m_bmpDrawing = new Bitmap(Width, Height);

            Graphics g = Graphics.FromImage(m_bmpDrawing);
            g.Clear(Color.Transparent);
            g = Graphics.FromImage(picPattern.Image);
            g.Clear(Color.White);

            DrawPattern(g);

            /*using (Graphics g = Graphics.FromImage(picPattern.Image))
            {
                DrawPattern(g);
            }*/
        }

        /// <summary>
        /// 讀取ini中的環境設定
        /// </summary>
        private void LoadSetting()
        {
            string sIniPath = string.Format(@"{0}\ini\Setting.ini", Application.StartupPath);
            m_APsetting.Load(sIniPath);
        }

        /// <summary>
        /// Draw the patterns
        /// </summary>
        /// <param name="g"></param>
        protected void DrawPattern(Graphics g)
        {
            double dReduceLength = 0.0;

            g.Clear(Color.White);
            DrawRect(g, 2.0f, 1.0f, false);
            DrawRect(g, 10.0f, 2.0f);
            DrawRect(g, 50.0f, 3.0f);
            //DrawRect(g, 50.0f, 4.0f);

            DrawColorRect(g);

            DrawString(g, dReduceLength);

            //DrawLine(ref dReduceLength, g, 50.0f, 3.0f);

            /*
            if (bRecordStatus == true && ParamAutoTuning.m_bDisplayProgressStatus == true)
                DrawString(g, dReduceLength);
            */

            g.Flush();
        }

        /// <summary>
        /// Draw the rectangle pattern with specific size
        /// </summary>
        /// <param name="g"></param>
        /// <param name="fMinmeter"></param>
        /// <param name="fPenSize"></param>
        /// <param name="bSetBlackFlag"></param>
        protected void DrawRect(Graphics g, float fMinmeter, float fPenSize, bool bSetBlackFlag = true)
        {
            Color clrLineColor = Color.Black;

            if (bSetBlackFlag == false)
                clrLineColor = ParamFingerAutoTuning.m_clrPHCKGrayLineColor;

            float fXmmPixels = Width / m_APsetting.m_fScreenX;
            float fYmmPixels = Height / m_APsetting.m_fScreenY;

            //Darw 50mm rectangle
            float fRectWidth = (fXmmPixels * fMinmeter);
            float fRectHeight = (fYmmPixels * fMinmeter);
            int nYRectCount = (int)(Height / fRectHeight);
            int nXRectCount = (int)(Width / fRectWidth);
            if (Height % fRectHeight != 0)
                nYRectCount++;

            if (Width % fRectWidth != 0)
                nXRectCount++;

            for (int nY = 0; nY < nYRectCount; nY++)
            {
                for (int nX = 0; nX < nXRectCount; nX++)
                {
                    RectangleF rcRect = new RectangleF(nX * fRectWidth, nY * fRectHeight, fRectWidth, fRectHeight);
                    g.DrawRectangle(new Pen(clrLineColor, fPenSize), rcRect.Left, rcRect.Top, rcRect.Width, rcRect.Height);
                }
            }
        }

        /*
        protected void DrawLine(ref double dReduceLength, Graphics g, float fMinmeter, float fPenSize)
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

                    g.DrawLine(new Pen(Color.Red, fPenSize), TopLineStartXCoord, TopLineStartYCoord, TopLineEndXCoord, TopLineEndYCoord);
                    g.DrawLine(new Pen(Color.Red, fPenSize), BottomLineStartXCoord, BottomLineStartYCoord, BottomLineEndXCoord, BottomLineEndYCoord);
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

                dReduceLength = YHalfLength * 2;

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

        protected void DrawColorRect(Graphics g)
        {
            //Draw the left Rectangle
            Random Rand = new Random();
            int nX = (int)(Width * 0.01f);
            int nY = (int)(Height * 0.1f);
            /*
            int nX = 0;
            int nY = 0;
            */

            float fXmmPixels = Width / m_APsetting.m_fScreenX;
            float fYmmPixels = Height / m_APsetting.m_fScreenY;
            /*
            float fXmmPixels = (float)Width / frmMain.m_nScreenSizeWidth;
            float fYmmPixels = (float)Height / frmMain.m_nScreenSizeHeight;
            */

            int nWidth = (int)(fXmmPixels * 60);
            int nHeight = (int)(fYmmPixels * 60);
            /*
            int nWidth = (int)(fXmmPixels * (frmMain.m_nScreenSizeWidth / 2));
            int nHeight = (int)(fYmmPixels * (frmMain.m_nScreenSizeHeight / 2));
            */

            Rectangle rcRect = new Rectangle(nX, nY, nWidth, nHeight);
            //g.FillRectangle(new SolidBrush(Color.FromArgb(m_APsetting.m_nLeftColor)), rcRect);
            g.FillRectangle(new SolidBrush(Color.Green), rcRect);

            nX = ((int)(Width * 0.55f));
            nY = (int)(Height * 0.550f);

            rcRect.X = nX;
            rcRect.Y = nY;
            rcRect.Width = nWidth;
            rcRect.Height = nHeight;

            //g.FillRectangle(new SolidBrush(Color.FromArgb(m_APsetting.m_nRightColor)), rcRect);
            g.FillRectangle(new SolidBrush(Color.Yellow), rcRect);
        }

        protected void DrawString(Graphics g, double dReduceLength)
        {
            string sOutputStr = sStepStatus + Environment.NewLine + sProgressStatus;

            Font newFont = new Font("Times", 30, FontStyle.Bold);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            float dLength = 0.0f;

            g.DrawString(sOutputStr, newFont, Brushes.Red, new RectangleF(this.Left, this.Top, this.Width, this.Height - dLength), sf);
        }

        public void SetFrameNumberText(string sDataType, int nFrameIndex, int nFrameCount)
        {
            FrameNumberLbl.Font = new Font("Times", 14, FontStyle.Bold);
            FrameNumberLbl.BackColor = System.Drawing.Color.Transparent;
            FrameNumberLbl.ForeColor = Color.Red;

            FrameNumberLbl.Text = string.Format("{0} Frame : {1} / {2}", sDataType, nFrameIndex, nFrameCount);

            FrameNumberLbl.Location = new Point(picPattern.Left, picPattern.Top);
            FrameNumberLbl.Parent = picPattern;
            FrameNumberLbl.BackColor = Color.White; //Color.Transparent;
            FrameNumberLbl.BringToFront();
        }

        public void SetMessageText(string sMessage)
        {
            FrameNumberLbl.Font = new Font("Times", 14, FontStyle.Bold);
            FrameNumberLbl.BackColor = System.Drawing.Color.Transparent;
            FrameNumberLbl.ForeColor = Color.Red;

            FrameNumberLbl.Text = sMessage;

            FrameNumberLbl.Location = new Point(picPattern.Left, picPattern.Top);
            FrameNumberLbl.Parent = picPattern;
            FrameNumberLbl.BackColor = Color.White; //Color.Transparent;
            FrameNumberLbl.BringToFront();
        }

        private Rectangle m_PreTestMsgRect = new Rectangle();
        public void SetFrameNumberText_Another(string sDataType, int nFrameIndex, int nFrameCount)
        {
            if (m_bmpDrawing == null)
                return;

            /*
            string sMessage = string.Format("Time : {0:0.000}s", dTime);
            sMessage += Environment.NewLine + string.Format("Report Number : {0}", nReportNumber);
            */
            string sMessage = string.Format("{0} Frame : {1} / {2}", sDataType, nFrameIndex, nFrameCount);

            using (Graphics g = Graphics.FromImage(m_bmpDrawing))
            {
                Font font = new Font("Times", 14, FontStyle.Bold);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                SizeF StringSize = g.MeasureString(sMessage, font);
                Rectangle StringRect = new Rectangle();

                //StringRect.X = (int)(((picPattern.Width / 2.0f) - (StringSize.Width / 2.0f)) + 1.0f);
                //StringRect.Y = (int)((picPattern.Height * 0.15f) + 1.0f);
                StringRect.X = picPattern.Left;
                StringRect.Y = picPattern.Top;
                StringRect.Width = (int)(StringSize.Width + 1.0f);
                StringRect.Height = (int)(StringSize.Height + 1.0f);

                Rectangle rcUnion = Rectangle.Union(StringRect, m_PreTestMsgRect);
                m_PreTestMsgRect = StringRect;

                //Clear screen and draw the string
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.FillRectangle(new SolidBrush(Color.White), m_PreTestMsgRect);
                //g.DrawString(sMessage, font, new SolidBrush(Color.Red), StringRect);
                g.DrawString(sMessage, font, Brushes.Red, new RectangleF(picPattern.Left, picPattern.Top, StringSize.Width, StringSize.Height), sf);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;

                g.Flush();

                lock (picPattern)
                {
                    picPattern.Invalidate(rcUnion);
                }
            }

            picPattern.Refresh();
        }

        private void OnFrmMainKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();

                if (btnPatternHandler != null)
                    btnPatternHandler(this, new frmMain.btnPatternEnableEventArgs(true));
            }
        }

        private void OnMenuOptionsClick(object sender, EventArgs e)
        {
            /*
            frmPHCKPatternOption option = new frmPHCKPatternOption(m_APsetting, byteEDIDData, cEDIDInformation, bPreviewSetting);
            option.TopMost = true;
            if (option.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (bPreviewSetting == false)
                    xLoadSetting();
                else
                    m_APsetting.GetParam();

                using (Graphics g = Graphics.FromImage(picPattern.Image))
                {
                    xDrawPattern(g);
                    picPattern.Refresh();
                }

                ScreenSizeLblHandler(this, new frmParamSetting.ScreenSizeLblEventArgs(FormMain.sScreenSize));
            }
            */
        }

        private void OnMenuExitClick(object sender, EventArgs e)
        {
            Close();

            if (btnPatternHandler != null)
                btnPatternHandler(this, new frmMain.btnPatternEnableEventArgs(true));
        }

        private void OnPatternMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(e.X, e.Y);
            }
        }

        private void picPattern_Paint(object sender, PaintEventArgs e)
        {
            if (m_bmpDrawing != null)
                e.Graphics.DrawImageUnscaled(m_bmpDrawing, 0, 0);
        }
    }

    public class ParamEnvironment
    {
        /// <summary>
        /// Store the physical screen size (mm)
        /// </summary>
        public int m_nScreenIdx = 0;
        public float m_fScreenX = 256.80f;
        public float m_fScreenY = 144.45f;
        public int m_nLeftColor = 0;
        public int m_nRightColor = 0;

        public ParamEnvironment()
        {

        }

        public void GetParam()
        {
            /*
            m_nScreenIdx = ParamAutoTuning.m_nScreenIndex;
            m_fScreenX = (float)ParamAutoTuning.m_fScreenWidth;
            m_fScreenY = (float)ParamAutoTuning.m_fScreenHeight;
            m_nLeftColor = ParamAutoTuning.m_nLeftColor;
            m_nRightColor = ParamAutoTuning.m_nRightColor;
            */
        }

        public void Load(string sIniPath)
        {
            /*
            ctmRegister.GetParamValue(ref m_nScreenIdx, "PHCK Pattern Setting", "ScreenIndex", "0", sIniPath, false);
            ctmRegister.GetParamValue(ref m_fScreenX, "PHCK Pattern Setting", "ScreenWidth", "256.80", sIniPath);
            ctmRegister.GetParamValue(ref m_fScreenY, "PHCK Pattern Setting", "ScreenHeight", "144.45", sIniPath);

            ctmRegister.GetParamValue(ref m_nLeftColor, "PHCK Pattern Setting", "LeftColor", "0xFF008000", sIniPath, true);
            ctmRegister.GetParamValue(ref m_nRightColor, "PHCK Pattern Setting", "RightColor", "0xFFFFFF00", sIniPath, true);
            */
        }

        public void Save(string sIniPath)
        {
            /*
            ctmRegister.IniWritValue("PHCK Pattern Setting", "ScreenIndex", m_nScreenIdx.ToString("D"), sIniPath);

            ctmRegister.IniWritValue("PHCK Pattern Setting", "ScreenWidth", m_fScreenX.ToString("F2"), sIniPath);
            ctmRegister.IniWritValue("PHCK Pattern Setting", "ScreenHeight", m_fScreenY.ToString("F2"), sIniPath);

            ctmRegister.IniWritValue("PHCK Pattern Setting", "LeftColor", m_nLeftColor.ToString("x8"), sIniPath);
            ctmRegister.IniWritValue("PHCK Pattern Setting", "RightColor", m_nRightColor.ToString("x8"), sIniPath);
            */
        }
    }
}
