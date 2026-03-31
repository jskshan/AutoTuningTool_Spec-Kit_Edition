using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class frmFullScreen : Form
    {
        public bool m_bRecordStateFlag = false;
        public Color m_colorBackColor = Color.Black;
        public Color m_colorForeColor = Color.White;
        private frmMain m_cfrmMain;
        public string m_sProgressState = "";
        public string m_sStepState = "";
        public int m_nPatternType = MainConstantParameter.m_nDRAWTYPE_NONE;
        public bool m_bDrawLineByUserFlag = false;
        public Bitmap m_bmPictureImage = null;

        public frmFullScreen(bool bRecordStateFlag, 
                             Color colorBackColor, 
                             frmMain cfrmMain, 
                             string sProgressState = "", 
                             string sStepState = "",
                             int nPatternType = MainConstantParameter.m_nDRAWTYPE_NONE,
                             bool bDrawLineByUserFlag = false, 
                             Bitmap bmPictureImage = null)
        {
            InitializeComponent();

            m_bRecordStateFlag = bRecordStateFlag;
            m_colorBackColor = colorBackColor;
            m_sProgressState = sProgressState;
            m_sStepState = sStepState;
            m_nPatternType = nPatternType;
            m_cfrmMain = cfrmMain;
            m_bDrawLineByUserFlag = bDrawLineByUserFlag;
            m_bmPictureImage = bmPictureImage;

            m_colorForeColor = Color.FromArgb(m_colorBackColor.A, 255 - m_colorBackColor.R, 255 - m_colorBackColor.G, 255 - m_colorBackColor.B);
        }

        private void frmFullScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();

                if (m_bRecordStateFlag == true)
                    m_cfrmMain.SetNewPatternButton(true);
            }
        }

        private void toolstripmenuitemExit_Click(object sender, EventArgs e)
        {
            Close();

            if (m_bRecordStateFlag == true)
                m_cfrmMain.SetNewPatternButton(true);
        }

        private void frmFullScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                menustripContext.Show(e.X, e.Y);
            }
        }

        private void frmFullScreen_Load(object sender, EventArgs e)
        {
            double dReduceLength = 0.0;

            picPattern.Left = 0;
            picPattern.Top = 0;
            picPattern.Width = this.Width;
            picPattern.Height = this.Height;
            picPattern.Image = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppPArgb);
            picPattern.BackgroundImage = m_bmPictureImage;
            picPattern.BackgroundImageLayout = ImageLayout.Stretch;

            using (Graphics g = Graphics.FromImage(picPattern.Image))
            {
                DrawLine(ref dReduceLength, g, m_colorForeColor, 3.0f);
                g.Flush();
            }

            if (m_bRecordStateFlag == true && ParamAutoTuning.m_bDisplayProgressStatus == true)
            {
                int nLength = 0;

                int nWidth = this.Width;
                int nHeight = this.Height;

                string sOutputMessage = m_sStepState + Environment.NewLine + m_sProgressState;

                if (m_sStepState == "")
                    sOutputMessage = m_sProgressState;

                lblProgressStatus.Font = new Font("Times", 30, FontStyle.Bold);
                lblProgressStatus.BackColor = System.Drawing.Color.Transparent;

                if (m_bmPictureImage != null)
                    lblProgressStatus.ForeColor = Color.Red;
                else
                    lblProgressStatus.ForeColor = m_colorForeColor;

                lblProgressStatus.Text = sOutputMessage;

                int nLabelWidth = lblProgressStatus.Width;
                int nLabelHeight = lblProgressStatus.Height;

                if (m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLINE || 
                    m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLINE ||
                    m_nPatternType == MainConstantParameter.m_nDRAWTYPE_CENTERPOINT ||
                    m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALLONGLINE || 
                    m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALLONGLINE ||
                    m_nPatternType == MainConstantParameter.m_nDRAWTYPE_HORIZONTALMIDDLELONGLINE || 
                    m_nPatternType == MainConstantParameter.m_nDRAWTYPE_VERTICALMIDDLELONGLINE)
                    nLength = Convert.ToInt32(Math.Ceiling(dReduceLength)) + (int)(nLabelHeight / 2);

                int nSettingWidth = (int)(nWidth / 2) - (int)(nLabelWidth / 2);
                int nSettingHeight = (int)(nHeight / 2) - (int)(nLabelHeight / 2) - nLength;

                lblProgressStatus.Location = new Point(nSettingWidth, nSettingHeight);
                lblProgressStatus.Parent = picPattern;
                lblProgressStatus.BackColor = Color.Transparent;
                lblProgressStatus.BringToFront();
            }

            picPattern.Refresh();
        }

        protected void DrawLine(ref double dReduceLength, Graphics g, Color colorLineColor, float fPenSize)
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

                    g.DrawLine(new Pen(colorLineColor, fPenSize), nTopLineStartXCoord, nTopLineStartYCoord, nTopLineEndXCoord, nTopLineEndYCoord);
                    g.DrawLine(new Pen(colorLineColor, fPenSize), nBottomLineStartXCoord, nBottomLineStartYCoord, nBottomLineEndXCoord, nBottomLineEndYCoord);
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

                dReduceLength = dYHalfLength;

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

        public void SetTimeAndReportNumberText(double dTime, int nReportNumber)
        {
            lblReportNumber.Font = new Font("Times", 14, FontStyle.Bold);
            lblReportNumber.BackColor = System.Drawing.Color.Transparent;

            if (m_bmPictureImage != null)
                lblReportNumber.ForeColor = Color.Red;
            else
                lblReportNumber.ForeColor = m_colorForeColor;

            string sMessage = string.Format("Time : {0:0.000}s", dTime);
            sMessage += Environment.NewLine + string.Format("Report Number : {0}", nReportNumber);

            lblReportNumber.Text = sMessage;

            lblReportNumber.Location = new Point(picPattern.Left, picPattern.Top);
            lblReportNumber.Parent = picPattern;
            lblReportNumber.BackColor = Color.Transparent;
            lblReportNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblReportNumber.BringToFront();
        }

        public void SetFrameNumberText(string sFrameMessage)
        {
            lblReportNumber.Font = new Font("Times", 14, FontStyle.Bold);
            lblReportNumber.BackColor = System.Drawing.Color.Transparent;

            if (m_bmPictureImage != null)
                lblReportNumber.ForeColor = Color.Red;
            else
                lblReportNumber.ForeColor = m_colorForeColor;

            string sMessage = sFrameMessage;

            lblReportNumber.Text = sMessage;

            lblReportNumber.Location = new Point(picPattern.Left, picPattern.Top);
            lblReportNumber.Parent = picPattern;
            lblReportNumber.BackColor = Color.Transparent;
            lblReportNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblReportNumber.BringToFront();
        }
    }
}
