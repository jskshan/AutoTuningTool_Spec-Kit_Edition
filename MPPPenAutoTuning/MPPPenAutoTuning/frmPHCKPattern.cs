using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class frmPHCKPattern : Form
    {
        private ParamEnvironment m_cAPsetting;

        public bool m_bRecordStateFlag = false;
        public bool m_bPreviewSettingFlag = false;
        private frmMain m_cfrmMain;
        public string m_sProgressState = "";
        public string m_sStepState = "";
        public int m_nPatternType = MainConstantParameter.m_nDRAWTYPE_NONE;
        public bool m_bDrawLineByUserFlag = false;
        public byte[] m_byteEDIDData_Array = null;
        public EDIDInfo m_cEDIDInformation = null;
        private Bitmap m_bmDrawing;

        public frmPHCKPattern(bool bRecordStateFlag, 
                              byte[] byteEDIDData_Array, 
                              EDIDInfo cEDIDInformation, 
                              frmMain cfrmMain, 
                              string sProgressState = "", 
                              string sStepState = "",
                              int nPatternType = MainConstantParameter.m_nDRAWTYPE_NONE,
                              bool bDrawLineByUserFlag = false, 
                              ParamEnvironment cAPsetting = null,
                              bool bPreviewSettingFlag = false)
        {
            InitializeComponent();

            m_bRecordStateFlag = bRecordStateFlag;
            m_byteEDIDData_Array = byteEDIDData_Array;
            m_cEDIDInformation = cEDIDInformation;
            m_cfrmMain = cfrmMain;
            m_sProgressState = sProgressState;
            m_sStepState = sStepState;
            m_nPatternType = nPatternType;
            m_bDrawLineByUserFlag = bDrawLineByUserFlag;
            m_bPreviewSettingFlag = bPreviewSettingFlag;

            if (cAPsetting != null)
                m_cAPsetting = cAPsetting;
            else
                m_cAPsetting = new ParamEnvironment(m_cfrmMain);
        }

        private void OnFrmLoad(object sender, EventArgs e)
        {
            if (m_bPreviewSettingFlag == false)
            {
                LoadSetting();
                toolstripmenuitemOption.Visible = true;
            }
            else
            {
                m_cAPsetting.GetParameter();
                toolstripmenuitemOption.Visible = false;
            }

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
            m_bmDrawing = new Bitmap(Width, Height);

            Graphics g = Graphics.FromImage(m_bmDrawing);
            g.Clear(Color.Transparent);
            g = Graphics.FromImage(picPattern.Image);
            g.Clear(Color.White);

            DrawPattern(g);

            /*
            using (Graphics g = Graphics.FromImage(picPattern.Image))
            {
                DrawPattern(g);
            }
            */
        }

        /// <summary>
        /// 讀取ini中的環境設定
        /// </summary>
        private void LoadSetting()
        {
            string sIniPath = string.Format(@"{0}\ini\Setting.ini", Application.StartupPath);
            m_cAPsetting.LoadParameter(sIniPath);
        }

        /// <summary>
        /// Draw the patterns
        /// </summary>
        /// <param name="g"></param>
        protected void DrawPattern(Graphics g)
        {
            double dReduceLength = 0.0;

            g.Clear(Color.White);
            DrawRectangle(g, 2.0f, 1.0f, false);
            DrawRectangle(g, 10.0f, 2.0f);
            DrawRectangle(g, 50.0f, 3.0f);

            DrawColorRectangle(g);

            DrawLine(ref dReduceLength, g, 3.0f);

            if (m_bRecordStateFlag == true && ParamAutoTuning.m_bDisplayProgressStatus == true)
                DrawString(g, dReduceLength);

            g.Flush();
        }

        /// <summary>
        /// Draw the rectangle pattern with specific size
        /// </summary>
        /// <param name="g"></param>
        /// <param name="fMinimeterValue"></param>
        /// <param name="fPenSize"></param>
        /// <param name="bSetBlackFlag"></param>
        protected void DrawRectangle(Graphics g, float fMinimeterValue, float fPenSize, bool bSetBlackFlag = true)
        {
            Color clrLineColor = Color.Black;

            if (bSetBlackFlag == false)
                clrLineColor = Color.FromArgb(m_cAPsetting.m_nGrayLineColor);

            float fXmmPixels = Width / m_cAPsetting.m_fScreenX;
            float fYmmPixels = Height / m_cAPsetting.m_fScreenY;

            //Darw 50mm rectangle
            float fRectangleWidth = (fXmmPixels * fMinimeterValue);
            float fRectangleHeight = (fYmmPixels * fMinimeterValue);
            int nYRectangleCount = (int)(Height / fRectangleHeight);
            int nXRectangleCount = (int)(Width / fRectangleWidth);

            if (Height % fRectangleHeight != 0)
                nYRectangleCount++;

            if (Width % fRectangleWidth != 0)
                nXRectangleCount++;

            for (int nYIndex = 0; nYIndex < nYRectangleCount; nYIndex++)
            {
                for (int nXIndex = 0; nXIndex < nXRectangleCount; nXIndex++)
                {
                    RectangleF structRectangle = new RectangleF(nXIndex * fRectangleWidth, nYIndex * fRectangleHeight, fRectangleWidth, fRectangleHeight);
                    g.DrawRectangle(new Pen(clrLineColor, fPenSize), structRectangle.Left, structRectangle.Top, structRectangle.Width, structRectangle.Height);
                }
            }
        }

        protected void DrawLine(ref double dReduceLength, Graphics g, float fPenSize)
        {
            if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_SLANTLINE)
            {
                if (m_bDrawLineByUserFlag == true)
                {
                    int nTopLineStartXCoord = (int)(picPattern.Width * 0.1);
                    int nTopLineStartYCoord = 0;
                    int nTopLineEndXCoord = picPattern.Width;
                    int nTopLineEndYCoord = (int)(picPattern.Height * 0.9);

                    int nBottomLineStartXCoord = 0;
                    int nBottomLineStartYCoord = (int)(picPattern.Height * 0.1);
                    int nBottomLineEndXCoord = (int)(picPattern.Width * 0.9);
                    int nBottomLineEndYCoord = picPattern.Height;

                    g.DrawLine(new Pen(Color.Red, fPenSize), nTopLineStartXCoord, nTopLineStartYCoord, nTopLineEndXCoord, nTopLineEndYCoord);
                    g.DrawLine(new Pen(Color.Red, fPenSize), nBottomLineStartXCoord, nBottomLineStartYCoord, nBottomLineEndXCoord, nBottomLineEndYCoord);
                }
            }
            else if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLINE || 
                     m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLINE ||
                     m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLONGLINE || 
                     m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLONGLINE ||
                     m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALMIDDLELONGLINE || 
                     m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALMIDDLELONGLINE)
            {
                double dXHalfLength = 0.0;
                double dYHalfLength = 0.0;

                if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLINE || 
                    m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLONGLINE ||
                    m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALMIDDLELONGLINE)
                {
                    double dXHalfLengthRatio = 15 / ParamAutoTuning.m_dScreenWidth;
                    dXHalfLength = picPattern.Width * dXHalfLengthRatio;
                    dYHalfLength = picPattern.Height * 0.005;
                }
                else if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLINE || 
                         m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLONGLINE ||
                         m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALMIDDLELONGLINE)
                {
                    dXHalfLength = picPattern.Width * 0.005;
                    double dYHalfLengthRatio = 15 / ParamAutoTuning.m_dScreenHeight;
                    dYHalfLength = picPattern.Height * dYHalfLengthRatio;
                }

                dReduceLength = dYHalfLength * 2;

                if (m_bDrawLineByUserFlag == true)
                {
                    if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLINE || 
                        m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLINE)
                    {
                        double dCenterXCoord = Math.Round((double)picPattern.Width / 2, 2, MidpointRounding.AwayFromZero);
                        double dCenterYCoord = Math.Round((double)picPattern.Height / 2, 2, MidpointRounding.AwayFromZero);

                        float fLeftTopXCoord = (float)Math.Round(dCenterXCoord - dXHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fLeftTopYCoord = (float)Math.Round(dCenterYCoord - dYHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fRightTopXCoord = (float)Math.Round(dCenterXCoord + dXHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fRightTopYCoord = (float)Math.Round(dCenterYCoord - dYHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fLeftBottomXCoord = (float)Math.Round(dCenterXCoord - dXHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fLeftBottomYCoord = (float)Math.Round(dCenterYCoord + dYHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fRightBottomXCoord = (float)Math.Round(dCenterXCoord + dXHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fRightBottomYCoord = (float)Math.Round(dCenterYCoord + dYHalfLength, 2, MidpointRounding.AwayFromZero);

                        g.DrawLine(new Pen(Color.Red, fPenSize), fLeftTopXCoord, fLeftTopYCoord, fRightTopXCoord, fRightTopYCoord);
                        g.DrawLine(new Pen(Color.Red, fPenSize), fLeftTopXCoord, fLeftTopYCoord, fLeftBottomXCoord, fLeftBottomYCoord);
                        g.DrawLine(new Pen(Color.Red, fPenSize), fLeftBottomXCoord, fLeftBottomYCoord, fRightBottomXCoord, fRightBottomYCoord);
                        g.DrawLine(new Pen(Color.Red, fPenSize), fRightTopXCoord, fRightTopYCoord, fRightBottomXCoord, fRightBottomYCoord);
                    }
                    else if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLONGLINE ||
                             m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALMIDDLELONGLINE)
                    {
                        double dYShiftLengthRatio = 0.5;

                        if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLONGLINE)
                            dYShiftLengthRatio = ParamAutoTuning.m_dLTHorShiftYAxisCoordinate / ParamAutoTuning.m_dScreenHeight;

                        double dYShiftLength = picPattern.Height * dYShiftLengthRatio;

                        float fLeftTopXCoord = 0;
                        float fLeftTopYCoord = (float)Math.Round(dYShiftLength - dYHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fRightTopXCoord = picPattern.Width;
                        float fRightTopYCoord = (float)Math.Round(dYShiftLength - dYHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fLeftBottomXCoord = 0;
                        float fLeftBottomYCoord = (float)Math.Round(dYShiftLength + dYHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fRightBottomXCoord = picPattern.Width;
                        float fRightBottomYCoord = (float)Math.Round(dYShiftLength + dYHalfLength, 2, MidpointRounding.AwayFromZero);

                        g.DrawLine(new Pen(Color.Red, fPenSize), fLeftTopXCoord, fLeftTopYCoord, fRightTopXCoord, fRightTopYCoord);
                        g.DrawLine(new Pen(Color.Red, fPenSize), fLeftBottomXCoord, fLeftBottomYCoord, fRightBottomXCoord, fRightBottomYCoord);
                    }
                    else if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLONGLINE ||
                             m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALMIDDLELONGLINE)
                    {
                        double dXShiftLengthRatio = 0.5;

                        if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLONGLINE)
                            dXShiftLengthRatio = ParamAutoTuning.m_dLTVerShiftXAxisCoordinate / ParamAutoTuning.m_dScreenWidth;

                        double dXShiftLength = picPattern.Width * dXShiftLengthRatio;

                        float fLeftTopXCoord = (float)Math.Round(dXShiftLength - dXHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fLeftTopYCoord = 0;
                        float fRightTopXCoord = (float)Math.Round(dXShiftLength + dXHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fRightTopYCoord = 0;
                        float fLeftBottomXCoord = (float)Math.Round(dXShiftLength - dXHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fLeftBottomYCoord = picPattern.Height;
                        float fRightBottomXCoord = (float)Math.Round(dXShiftLength + dXHalfLength, 2, MidpointRounding.AwayFromZero);
                        float fRightBottomYCoord = picPattern.Height;

                        g.DrawLine(new Pen(Color.Red, fPenSize), fLeftTopXCoord, fLeftTopYCoord, fLeftBottomXCoord, fLeftBottomYCoord);
                        g.DrawLine(new Pen(Color.Red, fPenSize), fRightTopXCoord, fRightTopYCoord, fRightBottomXCoord, fRightBottomYCoord);
                    }
                }
            }
            else if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_CENTERPOINT)
            {
                int nXHalfLength = (int)(picPattern.Width * 0.02);
                int nYHalfLength = (int)(picPattern.Height * 0.02);

                dReduceLength = nYHalfLength * 2;

                if (m_bDrawLineByUserFlag == true)
                {
                    double dCenterXCoord = Math.Round((double)picPattern.Width / 2, 2, MidpointRounding.AwayFromZero);
                    double dCenterYCoord = Math.Round((double)picPattern.Height / 2, 2, MidpointRounding.AwayFromZero);

                    float fLeftTopXCoord = (float)Math.Round(dCenterXCoord - nXHalfLength, 2, MidpointRounding.AwayFromZero);
                    float fLeftTopYCoord = (float)Math.Round(dCenterYCoord - nYHalfLength, 2, MidpointRounding.AwayFromZero);
                    float fRightTopXCoord = (float)Math.Round(dCenterXCoord + nXHalfLength, 2, MidpointRounding.AwayFromZero);
                    float fRightTopYCoord = (float)Math.Round(dCenterYCoord - nYHalfLength, 2, MidpointRounding.AwayFromZero);
                    float fLeftBottomXCoord = (float)Math.Round(dCenterXCoord - nXHalfLength, 2, MidpointRounding.AwayFromZero);
                    float fLeftBottomYCoord = (float)Math.Round(dCenterYCoord + nYHalfLength, 2, MidpointRounding.AwayFromZero);
                    float fRightBottomXCoord = (float)Math.Round(dCenterXCoord + nXHalfLength, 2, MidpointRounding.AwayFromZero);
                    float fRightBottomYCoord = (float)Math.Round(dCenterYCoord + nYHalfLength, 2, MidpointRounding.AwayFromZero);

                    g.DrawLine(new Pen(Color.Red, fPenSize), fLeftTopXCoord, fLeftTopYCoord, fRightTopXCoord, fRightTopYCoord);
                    g.DrawLine(new Pen(Color.Red, fPenSize), fLeftTopXCoord, fLeftTopYCoord, fLeftBottomXCoord, fLeftBottomYCoord);
                    g.DrawLine(new Pen(Color.Red, fPenSize), fLeftBottomXCoord, fLeftBottomYCoord, fRightBottomXCoord, fRightBottomYCoord);
                    g.DrawLine(new Pen(Color.Red, fPenSize), fRightTopXCoord, fRightTopYCoord, fRightBottomXCoord, fRightBottomYCoord);
                }
            }
        }

        protected void DrawColorRectangle(Graphics g)
        { 
            //Draw the left Rectangle
            //Random cRandom = new Random();
            int nX = (int)(Width * 0.01f);
            int nY = (int)(Height * 0.1f);
            float fXmmPixels = Width / m_cAPsetting.m_fScreenX;
            float fYmmPixels = Height / m_cAPsetting.m_fScreenY;

            int nWidth = (int)(fXmmPixels * 60);
            int nHeight = (int)(fYmmPixels * 60);

            Rectangle structRectangle = new Rectangle(nX, nY, nWidth, nHeight);
            g.FillRectangle(new SolidBrush(Color.FromArgb(m_cAPsetting.m_nLeftColor)), structRectangle);

            nX = ((int)(Width * 0.55f));
            nY = (int)(Height * 0.550f);

            structRectangle.X = nX;
            structRectangle.Y = nY;
            structRectangle.Width = nWidth;
            structRectangle.Height = nHeight;

            g.FillRectangle(new SolidBrush(Color.FromArgb(m_cAPsetting.m_nRightColor)), structRectangle);
        }

        protected void DrawString(Graphics g, double dReduceLength)
        {
            string sOutputMessage = m_sStepState + Environment.NewLine + m_sProgressState;

            if (m_sStepState == "")
                sOutputMessage = m_sProgressState;

            Font newFont = new Font("Times", 30, FontStyle.Bold);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            float dLength = 0.0f;

            if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLINE || 
                m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLINE ||
                m_nPatternType == MainConstantParameter.m_nDRAWTYPE_CENTERPOINT ||
                m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLONGLINE || 
                m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLONGLINE ||
                m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALMIDDLELONGLINE || 
                m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALMIDDLELONGLINE)
            {
                SizeF size = new SizeF();
                size = g.MeasureString(sOutputMessage, newFont);
                dLength = (float)dReduceLength + size.Height;
            }

            g.DrawString(sOutputMessage, newFont, Brushes.Red, new RectangleF(this.Left, this.Top, this.Width, this.Height - dLength), sf);
        }

        public void SetTimeAndReportNumberText(double dTime, int nReportNumber)
        {
            string sMessage = string.Format("Time : {0:0.000}s", dTime);
            sMessage += Environment.NewLine + string.Format("Report Number : {0}", nReportNumber);

            lblReportNumber.Font = new Font("Times", 14, FontStyle.Bold);
            lblReportNumber.BackColor = System.Drawing.Color.Transparent;
            lblReportNumber.ForeColor = Color.Red;

            lblReportNumber.Text = sMessage;

            lblReportNumber.Location = new Point(picPattern.Left, picPattern.Top);
            lblReportNumber.Parent = picPattern;
            lblReportNumber.BackColor = Color.White;
            lblReportNumber.BringToFront();
        }

        public void SetFrameNumberText(string sFrameMessage)
        {
            string sMessage = sFrameMessage;

            lblReportNumber.Font = new Font("Times", 14, FontStyle.Bold);
            lblReportNumber.BackColor = System.Drawing.Color.Transparent;
            lblReportNumber.ForeColor = Color.Red;

            lblReportNumber.Text = sMessage;

            lblReportNumber.Location = new Point(picPattern.Left, picPattern.Top);
            lblReportNumber.Parent = picPattern;
            lblReportNumber.BackColor = Color.White;
            lblReportNumber.BringToFront();
        }

        private void OnFrmMainKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();

                if (m_bRecordStateFlag == true)
                    m_cfrmMain.SetNewPatternButton(true);
            }
        }

        private void OnMenuOptionsClick(object sender, EventArgs e)
        {
            frmPHCKPatternOption cfrmPHCKPatternOption = new frmPHCKPatternOption(m_cfrmMain, m_cAPsetting, m_byteEDIDData_Array, m_cEDIDInformation, m_bPreviewSettingFlag);

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmPHCKPatternOption.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmPHCKPatternOption.Height / 2);

            cfrmPHCKPatternOption.StartPosition = FormStartPosition.Manual;
            cfrmPHCKPatternOption.Location = new Point(nLocationX, nLocationY);

            cfrmPHCKPatternOption.TopMost = true;

            if (cfrmPHCKPatternOption.ShowDialog() == DialogResult.OK)
            {
                if (m_bPreviewSettingFlag == false)
                    LoadSetting();
                else
                    m_cAPsetting.GetParameter();

                using (Graphics g = Graphics.FromImage(picPattern.Image))
                {
                    DrawPattern(g);
                    picPattern.Refresh();
                }

                m_cfrmMain.m_cfrmParameterSetting.OutputScreenSizeLabel(m_cfrmMain.m_sScreenSize);
            }
        }

        private void OnMenuExitClick(object sender, EventArgs e)
        {
            Close();

            if (m_bRecordStateFlag == true)
                m_cfrmMain.SetNewPatternButton(true);
        }

        private void OnPatternMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                menustripContext.Show(e.X, e.Y);
            }
        }

        private void picPattern_Paint(object sender, PaintEventArgs e)
        {
            if (m_bmDrawing != null)
                e.Graphics.DrawImageUnscaled(m_bmDrawing, 0, 0);
        }
    }

    public class ParamEnvironment
    {
        private frmMain m_cfrmMain;

        /// <summary>
        /// Store the physical screen size (mm)
        /// </summary>
        public int m_nScreenIdx = 0;
        public float m_fScreenX = 256.80f;
        public float m_fScreenY = 144.45f;
        public int m_nLeftColor = 0;
        public int m_nRightColor = 0;
        public int m_nGrayLineColor = 0;

        public ParamEnvironment(frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
        }

        public void GetParameter()
        {
            m_nScreenIdx = ParamAutoTuning.m_nScreenIndex;
            m_fScreenX = (float)ParamAutoTuning.m_dScreenWidth;
            m_fScreenY = (float)ParamAutoTuning.m_dScreenHeight;
            m_nLeftColor = ParamAutoTuning.m_nLeftColor;
            m_nRightColor = ParamAutoTuning.m_nRightColor;
            m_nGrayLineColor = ParamAutoTuning.m_nGrayLineColor;
        }

        public void LoadParameter(string sIniPath)
        {
            RegisterValue.GetParameterValue(ref m_nScreenIdx, "PHCK Pattern Setting", "ScreenIndex", "0", sIniPath, false, m_cfrmMain);
            RegisterValue.GetParameterValue(ref m_fScreenX, "PHCK Pattern Setting", "ScreenWidth", "256.80", sIniPath, m_cfrmMain);
            RegisterValue.GetParameterValue(ref m_fScreenY, "PHCK Pattern Setting", "ScreenHeight", "144.45", sIniPath, m_cfrmMain);

            RegisterValue.GetParameterValue(ref m_nLeftColor, "PHCK Pattern Setting", "LeftColor", "0xFF008000", sIniPath, true, m_cfrmMain);
            RegisterValue.GetParameterValue(ref m_nRightColor, "PHCK Pattern Setting", "RightColor", "0xFFFFFF00", sIniPath, true, m_cfrmMain);
            RegisterValue.GetParameterValue(ref m_nGrayLineColor, "PHCK Pattern Setting", "GrayLineColor", "0xFF909090", sIniPath, true, m_cfrmMain);
        }

        public void Save(string sIniPath)
        {
            RegisterValue.IniWritValue("PHCK Pattern Setting", "ScreenIndex", m_nScreenIdx.ToString("D"), sIniPath);

            RegisterValue.IniWritValue("PHCK Pattern Setting", "ScreenWidth", m_fScreenX.ToString("F2"), sIniPath);
            RegisterValue.IniWritValue("PHCK Pattern Setting", "ScreenHeight", m_fScreenY.ToString("F2"), sIniPath);

            RegisterValue.IniWritValue("PHCK Pattern Setting", "LeftColor", m_nLeftColor.ToString("x8"), sIniPath);
            RegisterValue.IniWritValue("PHCK Pattern Setting", "RightColor", m_nRightColor.ToString("x8"), sIniPath);
            RegisterValue.IniWritValue("PHCK Pattern Setting", "GrayLineColor", m_nGrayLineColor.ToString("x8"), sIniPath);
        }
    }
}
