using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using BlockingQueue;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    /* ***************************************************************
     * Define the basic test pattern and test item object.
     * ***************************************************************/

    public enum PatternType
    {
        Point = 0,
        Horizontal,
        Vertical,
        Diagonal,
        Acer,
        TX8FingerAccuracyTest,
        TX8HoverTest,
        TX8EdgyTest,
        TX8HorTest,     //Horizontal Test
        TX8VerTest,     //Vertical Test
        TX8DiagTest,     //Diagonal Test
        TX8NoiseTest,
        ChromeGridAccuracyTest,
        ChromeTapAccuracyTest,
        ChromeHorTest,
        ChromeVerTest,
        ChromeDiagTest,
        ChromeResolutionTest,
        ChromeHysteresisTest,
        ChromeReversedTest,
        Normal,
        PHCKPattern,
    }

    public partial class frmPattern : Form
    {
        protected frmMain m_cfrmMain = null;
        protected frmPatternMenu m_cfrmPatternMenu = null;

        public bool m_bSaveLogComplete = false;
        //private bool m_bGetRXReport = false;
        //private bool m_bGetTXReport = false;
        //public bool m_bGetReport = false;
        //private bool m_bGetFirstReport = true;

        public EDIDInfo m_cEDIDInformation;
        public frmMain.FlowStep m_cFlowStep;

        #region Constant value declare
        protected const int MARGIN = 10;
        //protected static readonly string[] PATTERN_NAME_ARRAY = { "Normal", "PHCK Pattern" };
        #endregion

        //一個pixel包含多少pen的實體座標長度
        protected float m_fPenRatioX = 0.0f;
        protected float m_fPenRatioY = 0.0f;

        //紀錄X/Y方向1mm包含多少像素
        protected float m_fmmPixelX = 0.0f;
        protected float m_fmmPixelY = 0.0f;

        #region Declare the test pattern
        protected int m_nPatternID = 0;
        protected TestPattern m_cTestPattern = null;
        public bool m_bTestFinish = false;
        public bool m_bTestStart = false;
        public bool m_bFormClose = false;
        public bool m_bTestRetry = false;
        #endregion

        #region Declare the bitmap layer to draw information
        protected Bitmap m_bmpReportLayer = null;
        protected Bitmap m_bmpInfoLayer = null;
        #endregion

        /// <summary>
        /// Run the refresh thread?
        /// </summary>
        protected bool m_bRefresh = false;

        #region Define the variable to get the screen orientation
        protected ScreenOrientation m_PreScreenOrientation = ScreenOrientation.Angle0;

        public bool Portrait
        {
            get
            {
                return SystemInformation.ScreenOrientation == ScreenOrientation.Angle90 || SystemInformation.ScreenOrientation == ScreenOrientation.Angle270;
            }
        }
        #endregion

        public BlockingQueue.BlockingQueue<byte> m_qFIFO = new BlockingQueue.BlockingQueue<byte>();
        public byte[] m_pBuf = null;
        public byte[] m_pTmpRXBuf = null;
        public byte[] m_pTmpTXBuf = null;

        List<byte> m_byteReport_List = new List<byte>();
        public List<List<byte>> m_byteData_List = new List<List<byte>>();

        public frmPattern(EDIDInfo cEDIDInformation, frmMain.FlowStep cFlowStep, frmMain cfrmParent)
        {
            InitializeComponent();

            m_cEDIDInformation = cEDIDInformation;
            m_cFlowStep = cFlowStep;
            m_cfrmMain = cfrmParent;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            /*
            if (m_FlowStep.CollectStep == MainCollectStep.PENNOISE)
                m_cfrmPatternMenu = new frmPatternMenu(this, false);
            else
                m_cfrmPatternMenu = new frmPatternMenu(this);
            */
            m_cfrmPatternMenu.TopMost = true;
            m_cfrmPatternMenu.Location = new Point(Width - m_cfrmPatternMenu.Width - MARGIN, MARGIN);
            m_cfrmPatternMenu.Show();

            /*
            m_pBuf = new byte[ParamFingerAutoTuning.m_nReportDataLength];
            m_pTmpRXBuf = new byte[ParamFingerAutoTuning.m_nReportDataLength];
            m_pTmpTXBuf = new byte[ParamFingerAutoTuning.m_nReportDataLength];
            */

            //Set the windows properties
            WindowState = FormWindowState.Maximized;

            //Create the bitmap
            pbCanvas.Image = new Bitmap(Width, Height);
            m_bmpReportLayer = new Bitmap(Width, Height);
            m_bmpInfoLayer = new Bitmap(Width, Height);

            //Which pixels in mm.(X/Y)
            if (StartTestProcedure() == false)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ExitAPProcedure));
                return;
            }

            //Set the test name
            m_cfrmPatternMenu.SetCaption(m_cFlowStep.m_sStepName);
            m_cfrmPatternMenu.DisplayStatus("Record Ready");
            /*
            if (m_cFlowStep.CollectStep != MainCollectStep.PENNOISE)
                m_cfrmPatternMenu.DisplayTimeAndReportNumber(0);
            else
                m_cfrmPatternMenu.DisplayTimeAndReportNumber(0, true);
            */

            //Start a thread to update the canvas
            m_bRefresh = true;
            ThreadPool.QueueUserWorkItem(Refresh);
        }

        /// <summary>
        /// Start a test procedure
        /// </summary>
        public bool StartTestProcedure()
        {
            m_qFIFO.Clear();
            m_byteData_List.Clear();

            /*
            float fScreenWidth = m_cEDIDInformation.EDIDInfo_Width;
            float fScreenHeight = m_cEDIDInformation.EDIDInfo_Height;
            */
            float fScreenWidth = frmMain.m_nScreenSizeWidth;
            float fScreenHeight = frmMain.m_nScreenSizeHeight;

            ComputePenLogical2ScreenRatio(fScreenWidth, fScreenHeight);

            #region Create the test pattern
            m_bTestFinish = false;
            CreatePattern(fScreenWidth, fScreenHeight);
            #endregion

            InitializePattern(PatternType.PHCKPattern, true);

            //Hide the test result
            if (m_cfrmPatternMenu != null)
                m_cfrmPatternMenu.HideInfoGridView();

            /*
            if (m_cFlowStep.CollectStep == MainCollectStep.PENNOISE)
                m_cTestPattern.DoTest(null, m_fmmPixelX, m_fmmPixelY);
            else
            {
                m_cfrmPatternMenu.DisplayStatus("Record Ready");
                m_cfrmPatternMenu.DisplayTimeAndReportNumber(0);
            }
            */

            return true;
        }

        protected void CreatePattern(float fScreenWidth, float fScreenHeight)
        {
            if (m_cTestPattern != null)
                m_cTestPattern.Dispose();

            string sParamDataFilePath = string.Format(@"{0}\ini\ParamData.dat", Application.StartupPath);
            PatternParamBase cNewParamBase = null;

            /*
            if (Portrait == true)
                cNewParamBase = new ElanLinearityParamBase(fScreenHeight, fScreenWidth);
            else
                cNewParamBase = new ElanLinearityParamBase(fScreenWidth, fScreenHeight);

            cNewParamBase.LoadParameter(sParamDataFilePath);
            m_cTestPattern = new FreeDrawPattern(cNewParamBase, m_bmpInfoLayer);
            */

            if (Portrait == true)
                cNewParamBase = new LineBarCursorParam(fScreenHeight, fScreenWidth);
            else
                cNewParamBase = new LineBarCursorParam(fScreenWidth, fScreenHeight);

            cNewParamBase.LoadParameter(sParamDataFilePath);
            m_cTestPattern = new LineBarCursorPattern(cNewParamBase, m_cFlowStep, m_bmpInfoLayer, this, m_cfrmPatternMenu);

            m_cTestPattern.bmpDrawLayer = m_bmpInfoLayer;
        }

        protected void InitializePattern(PatternType ePatternType, bool bShowGridLine)
        {
            if (m_bmpInfoLayer != null)
                m_cTestPattern.DrawPattern(m_fmmPixelX, m_fmmPixelY);

            if (m_bmpReportLayer != null)
            {
                using (Graphics g = Graphics.FromImage(m_bmpReportLayer))
                    g.Clear(Color.Transparent);
            }

            switch(ePatternType)
            {
                case PatternType.Normal:
                    if (bShowGridLine == true)
                    {
                        lock (m_bmpReportLayer)
                        {
                            using (Graphics g = Graphics.FromImage(m_bmpReportLayer))
                            {
                                DrawRect(g, Color.FromArgb(20, 0, 0, 0), 50.0f, 3.0f);
                                DrawRect(g, Color.FromArgb(20, 0, 0, 0), 10.0f, 2.0f);
                                DrawRect(g, Color.FromArgb(20, 0, 0, 0), 2.0f, 1.0f);
                            }
                        }
                    }
                    break;
                case PatternType.PHCKPattern:
                    lock (m_bmpReportLayer)
                    {
                        using (Graphics g = Graphics.FromImage(m_bmpReportLayer))
                        {
                            g.Clear(Color.White);
                            DrawRect(g, Color.Black, 50.0f, 3.0f);
                            DrawRect(g, Color.Black, 10.0f, 2.0f);
                            DrawRect(g, Color.Black, 2.0f, 1.0f);
                            DrawColorRect(g);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Draw the rectangle pattern with specific size
        /// </summary>
        /// <param name="g"></param>
        /// <param name="fMinmeter"></param>
        /// <param name="fPenSize"></param>
        protected void DrawRect(Graphics g, Color cColor, float fMinmeter, float fPenSize)
        {
            //Darw 50mm rectangle
            float fRectWidth = (m_fmmPixelX * fMinmeter);
            float fRectHeight = (m_fmmPixelY * fMinmeter);
            int nYRectCount = (int)(Height / fRectHeight);
            int nXRectCount = (int)(Width / fRectWidth);
            //Pen penLine = new Pen(Color.FromArgb(20, 0, 0, 0), fPenSize);
            Pen penLine = new Pen(cColor, fPenSize);

            if (Height % fRectHeight != 0)
                nYRectCount++;

            if (Width % fRectWidth != 0)
                nXRectCount++;

            for (int nY = 0; nY < nYRectCount; nY++)
            {
                for (int nX = 0; nX < nXRectCount; nX++)
                {
                    RectangleF rcRect = new RectangleF(nX * fRectWidth, nY * fRectHeight, fRectWidth, fRectHeight);
                    g.DrawRectangle(penLine, rcRect.Left, rcRect.Top, rcRect.Width, rcRect.Height);
                }
            }
        }

        protected void DrawColorRect(Graphics g)
        {
            //Draw the left Rectangle
            //Random cRandom = new Random();
            int nX = (int)(Width * 0.01f);
            int nY = (int)(Height * 0.1f);
            /*
            float fXmmPixels = Width / m_cEDIDInformation.EDIDInfo_Width;
            float fYmmPixels = Height / m_cEDIDInformation.EDIDInfo_Height;
            */
            float fXmmPixels = Width / frmMain.m_nScreenSizeWidth;
            float fYmmPixels = Height / frmMain.m_nScreenSizeHeight;

            int nWidth = (int)(fXmmPixels * 60);
            int nHeight = (int)(fYmmPixels * 60);

            Rectangle rcRect = new Rectangle(nX, nY, nWidth, nHeight);
            g.FillRectangle(new SolidBrush(Color.Green), rcRect);

            nX = ((int)(Width * 0.55f));
            nY = (int)(Height * 0.550f);

            rcRect.X = nX;
            rcRect.Y = nY;
            rcRect.Width = nWidth;
            rcRect.Height = nHeight;

            g.FillRectangle(new SolidBrush(Color.Yellow), rcRect);
        }

        private void ExitAPProcedure(object objParam)
        {
            Invoke((MethodInvoker)delegate
            {
                Close();
            });
        }

        private void StopRefreshThread()
        {
            m_bRefresh = false;
            Thread.Sleep(30);
        }

        /// <summary>
        /// Compute the ratio that logical position convert to screen position
        /// </summary>
        public void ComputePenLogical2ScreenRatio(float fScreenWidth, float fScreenHeight)
        {
            if (Portrait == true)
            {
                m_fmmPixelX = (float)Width / fScreenHeight;
                m_fmmPixelY = (float)Height / fScreenWidth;

                m_fPenRatioX = (float)Width / (float)frmMain.m_nPenLogicalYMax;
                m_fPenRatioY = (float)Height / (float)frmMain.m_nPenLogicalXMax;
            }
            else
            {
                m_fmmPixelX = (float)Width / fScreenWidth;
                m_fmmPixelY = (float)Height / fScreenHeight;

                m_fPenRatioX = (float)Width / (float)frmMain.m_nPenLogicalXMax;
                m_fPenRatioY = (float)Height / (float)frmMain.m_nPenLogicalYMax;
            }
        }

        protected ElanReport ProcessPenReport(Graphics g, byte[] ReportData)
        {
            PointF ptClientPos = new PointF(-1.0f, -1.0f);
            ElanReport cCurReport;

            /*
            #region Parse the pen report
            ElanTouch.EMC_REPORT_PEN PenReport;
            IntPtr pPenReport = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ElanTouch.EMC_REPORT_PEN)));
            int nReport = ElanTouch.ParserReportPen(ReportData, ReportData.Length, pPenReport);
            PenReport = (ElanTouch.EMC_REPORT_PEN)Marshal.PtrToStructure(pPenReport, typeof(ElanTouch.EMC_REPORT_PEN));
            Marshal.FreeHGlobal(pPenReport);

            //Convert the Pen position the clinet postion
            ptClientPos = PenPosMap2Screen(PenReport.uiPosX, PenReport.uiPosY);
            #endregion

            if (PenReport.bTip == true || PenReport.bErase == true)
            {
                cCurReport = new ElanReport(ptClientPos.X, ptClientPos.Y, (int)PenReport.uiTipPressure, 0, ElanReport.ReportType.ActivePen, ElanReport.ReportMode.Contact);
            }
            else if (PenReport.bInRange == true)
            {
                cCurReport = new ElanReport(ptClientPos.X, ptClientPos.Y, (int)PenReport.uiTipPressure, 0, ElanReport.ReportType.ActivePen, ElanReport.ReportMode.Hover);
            }
            else
            {
                cCurReport = new ElanReport(ptClientPos.X, ptClientPos.Y, (int)PenReport.uiTipPressure, 0, ElanReport.ReportType.ActivePen, ElanReport.ReportMode.Up);
            }

            //Draw the report point that use to do test
            if (PenReport.bTip == true || PenReport.bErase == true)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawEllipse(new Pen(Color.Red, 1.0f), ptClientPos.X, ptClientPos.Y, 3.0f, 3.0f);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
            else if (PenReport.bInRange)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawEllipse(new Pen(Color.Orange, 1.0f), ptClientPos.X, ptClientPos.Y, 3.0f, 3.0f);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
            
            g.Flush();
            */

            cCurReport = new ElanReport(ptClientPos.X, ptClientPos.Y, (int)0, 0, ElanReport.ReportType.ActivePen, ElanReport.ReportMode.Contact);

            return cCurReport;
        }

        /// <summary>
        /// Map the pen logical X/Y to Screen X/Y
        /// </summary>
        /// <param name="nX"></param>
        /// <param name="nY"></param>
        /// <returns></returns>
        public PointF PenPosMap2Screen(uint nX, uint nY)
        {
            PointF ptMappedReport = new PointF();

            if (SystemInformation.ScreenOrientation == ScreenOrientation.Angle90)
            {
                ptMappedReport.X = Width - nY * m_fPenRatioX;
                ptMappedReport.Y = nX * m_fPenRatioY;
            }
            else if (SystemInformation.ScreenOrientation == ScreenOrientation.Angle180)
            {
                ptMappedReport.X = Width - nX * m_fPenRatioX;
                ptMappedReport.Y = Height - nY * m_fPenRatioY;
            }
            else if (SystemInformation.ScreenOrientation == ScreenOrientation.Angle270)
            {
                ptMappedReport.X = nY * m_fPenRatioX;
                ptMappedReport.Y = Height - nX * m_fPenRatioY;
            }
            else
            {
                ptMappedReport.X = nX * m_fPenRatioX;
                ptMappedReport.Y = nY * m_fPenRatioY;
            }

            return ptMappedReport;
        }

        private void Refresh(object obj)
        {
            try
            {
                while (m_bRefresh)
                {
                    pbCanvas.Invoke((MethodInvoker)delegate
                    {
                        lock (pbCanvas)
                        {
                            pbCanvas.Invalidate();
                        }
                    });

                    Thread.Sleep(20);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Disable the gesture that system default
        /// </summary>
        private void DisableTouchGesture()
        {
            try
            {
                RegistryKey key;
                key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\EdgeUI");
                key.Close();
                key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\EdgeUI");
                key.SetValue("AllowEdgeSwipe", (Int32)0);
                key.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void OnfrmKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                StartTestProcedure();
            }
        }

        private void OnfrmClosed(object sender, FormClosedEventArgs e)
        {
            m_bFormClose = true;

            if (m_bSaveLogComplete == false)
            {
                if (frmMain.m_bCollectFlowError == false)
                {
                    frmMain.m_bCollectFlowError = true;
                    frmMain.m_sCollectFlowErrorMessage = "Collect Flow Not Complete";
                }
            }

            if (m_cfrmPatternMenu != null)
                m_cfrmPatternMenu.Close();

            StopRefreshThread();

            /*if (m_bTPConnected == true)
            {
                ElanTouch.Disconnect();
                m_bTPConnected = false;
            }
            */

            Win32.TimeEndPeriod(1);
        }

        public void ShowOption()
        {
            /*
            frmOption OptionDlg = new frmOption(TEST_ITEM_NAME_ARRAY[m_nPatternID]);
            
            if (OptionDlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            StartTestProcedure();
            */
        }

        public bool CheckReportData()
        {
            if (m_cTestPattern == null || m_bTestFinish == false)
                return false;

            /*
            if (m_cFlowStep.CollectStep == MainCollectStep.PENNOISE)
            {
                List<byte> Tmp_ReportData = new List<byte>();

                int RX_Sec1_Count = 0;
                int RX_Sec2_Count = 0;
                int RX_Sec3_Count = 0;
                int RX_Sec4_Count = 0;

                int TX_Sec1_Count = 0;
                int TX_Sec2_Count = 0;

                bool bGetRXTraceNumber = false;
                bool bGetTXTraceNumber = false;
                int RX_TraceNumber = 0;
                int TX_TraceNumber = 0;

                int DataLength = DataArray.Count;

                for (int nIndex = 0; nIndex < DataLength; nIndex++)
                {
                    Tmp_ReportData.Clear();
                    Tmp_ReportData = new List<byte>(DataArray[nIndex]);

                    int ReportLength = Tmp_ReportData.Count;
                    if (ReportLength != ParamFingerAutoTuning.m_nReportDataLength)
                        continue;

                    if (ParamFingerAutoTuning.m_nShiftStartByte > 0 && ParamFingerAutoTuning.m_nShiftByteNumber > 0)
                        Tmp_ReportData.RemoveRange(ParamFingerAutoTuning.m_nShiftStartByte - 1, ParamFingerAutoTuning.m_nShiftByteNumber);

                    if (Tmp_ReportData[0] != 0x07 || Tmp_ReportData[1] != 0x01 ||
                        Tmp_ReportData[2] != 0xFF || Tmp_ReportData[3] != 0xFF ||
                        Tmp_ReportData[4] != 0xFF || Tmp_ReportData[5] != 0xFF)
                        continue;

                    int TraceTypeBitValue = Convert.ToInt32(Math.Pow(2, ParamFingerAutoTuning.m_nPNTraceTypeBit - 1));
                    int DataTypeBitValue = Convert.ToInt32(Math.Pow(2, ParamFingerAutoTuning.m_nPNDataTypeBit - 1));
                    int ExecuteTypeBitValue = Convert.ToInt32(Math.Pow(2, ParamFingerAutoTuning.m_nPNExecuteTypeBit - 1));
                    int DataSectionValue = Convert.ToInt32(Tmp_ReportData[ParamFingerAutoTuning.m_nPNDataSectionByte - 1]);
                    if ((Tmp_ReportData[ParamFingerAutoTuning.m_nPNAutoTuningInfoByte - 1] & TraceTypeBitValue) == 0)
                    {
                        if ((Tmp_ReportData[ParamFingerAutoTuning.m_nPNAutoTuningInfoByte - 1] & DataTypeBitValue) != 1 && (Tmp_ReportData[ParamFingerAutoTuning.m_nPNAutoTuningInfoByte - 1] & ExecuteTypeBitValue) == 0)
                        {
                            int WordCount = Tmp_ReportData[ParamFingerAutoTuning.m_nPNRTXTraceNumberByte - 1];
                            if (bGetRXTraceNumber == false)
                            {
                                RX_TraceNumber = WordCount;
                                bGetRXTraceNumber = true;
                            }
                            else if (RX_TraceNumber != WordCount)
                                continue;

                            if (DataSectionValue == 0)
                                RX_Sec1_Count++;
                            else if (DataSectionValue == 1)
                                RX_Sec2_Count++;
                            else if (DataSectionValue == 2)
                                RX_Sec3_Count++;
                            else if (DataSectionValue == 3)
                                RX_Sec4_Count++;
                        }
                    }
                    else
                    {
                        if ((Tmp_ReportData[ParamFingerAutoTuning.m_nPNAutoTuningInfoByte - 1] & DataTypeBitValue) != 1 && (Tmp_ReportData[ParamFingerAutoTuning.m_nPNAutoTuningInfoByte - 1] & ExecuteTypeBitValue) == 0)
                        {
                            int WordCount = Tmp_ReportData[ParamFingerAutoTuning.m_nPNRTXTraceNumberByte - 1];
                            if (bGetTXTraceNumber == false)
                            {
                                TX_TraceNumber = WordCount;
                                bGetTXTraceNumber = true;
                            }
                            else if (TX_TraceNumber != WordCount)
                                continue;

                            if (DataSectionValue == 0)
                                TX_Sec1_Count++;
                            else if (DataSectionValue == 1)
                                TX_Sec2_Count++;
                        }
                    }
                }

                int RX_SectionCount = (int)(RX_TraceNumber / 24);
                int RX_SectionRem = RX_TraceNumber % 24;
                if (RX_SectionRem > 0)
                    RX_SectionCount = RX_SectionCount + 1;

                int RX_DataCount = RX_Sec1_Count;
                if (RX_SectionCount >= 2)
                    RX_DataCount = Math.Min(RX_DataCount, RX_Sec2_Count);
                if (RX_SectionCount >= 3)
                    RX_DataCount = Math.Min(RX_DataCount, RX_Sec3_Count);
                if (RX_SectionCount >= 4)
                    RX_DataCount = Math.Min(RX_DataCount, RX_Sec4_Count);

                int TX_SectionCount = (int)(TX_TraceNumber / 24);
                int TX_SectionRem = TX_TraceNumber % 24;
                if (TX_SectionRem > 0)
                    TX_SectionCount = TX_SectionCount + 1;

                int TX_DataCount = TX_Sec1_Count;
                if (TX_SectionCount >= 2)
                    TX_DataCount = Math.Min(TX_DataCount, TX_Sec2_Count);

                if (RX_DataCount < ParamFingerAutoTuning.m_nPNValidReportNumber || RX_DataCount == 0)
                {
                    frmMain.m_bCollectFlowError = true;
                    frmMain.m_sCollectFlowErrorMsg = string.Format("No Enough RX Report Section Data({0} < LB:{1})",
                                                                   RX_DataCount, ParamFingerAutoTuning.m_nPNValidReportNumber);

                    MessageBox.Show(frmMain.m_sCollectFlowErrorMsg);
                    return false;
                }

                if (TX_DataCount < ParamFingerAutoTuning.m_nPNValidReportNumber || TX_DataCount == 0)
                {
                    frmMain.m_bCollectFlowError = true;
                    frmMain.m_sCollectFlowErrorMsg = string.Format("No Enough TX Report Section Data({0} < LB:{1})",
                                                                   TX_DataCount, ParamFingerAutoTuning.m_nPNValidReportNumber);

                    MessageBox.Show(frmMain.m_sCollectFlowErrorMsg);
                    return false;
                }
            }
            else if (m_FlowStep.CollectStep == MainCollectStep.PENNORMALSCAN_CONTACT ||
                     m_FlowStep.CollectStep == MainCollectStep.PENNORMALSCAN_HOVER)
            {
                List<byte> Tmp_ReportData = new List<byte>();

                List<List<int>> OrgDataArray_Rx = new List<List<int>>();
                List<List<int>> OrgDataArray_Tx = new List<List<int>>();
                List<int> OrgIndexArray_Rx = new List<int>();
                List<int> OrgIndexArray_Tx = new List<int>();
                List<int> DataArrayList_Rx = new List<int>();
                List<int> DataArrayList_Tx = new List<int>();

                int ValidReportDataCount = 0;

                int NormalReportDataLength = 14;
                int SectionNumber = 24;
                int IndexByte = 12;
                int TraceTypeByte = 13;
                int TraceNumberByte = 10;
                //int StraightUsefulDataNumber = ParamAutoTuning.m_nStraightUsefulDataNumber;

                bool bGetRXTraceNumber = false;
                bool bGetTXTraceNumber = false;
                int RX_TraceNumber = 0;
                int TX_TraceNumber = 0;
                int LeftBoundary = ParamFingerAutoTuning.m_nPNSValidReportEdgeNumber - 1;
                int RightBoundary = SectionNumber - ParamFingerAutoTuning.m_nPNSValidReportEdgeNumber;
                int DataLength = DataArray.Count;

                int RXTypeValue = 0x96;
                int TXTypeValue = 0x97;

                for (int nIndex = 0; nIndex < DataLength; nIndex++)
                {
                    Tmp_ReportData.Clear();
                    Tmp_ReportData = new List<byte>(DataArray[nIndex]);

                    int ReportLength = Tmp_ReportData.Count;
                    if (ReportLength != ParamFingerAutoTuning.m_nReportDataLength)
                        continue;

                    if (ParamFingerAutoTuning.m_nShiftStartByte > 0 && ParamFingerAutoTuning.m_nShiftByteNumber > 0)
                        Tmp_ReportData.RemoveRange(ParamFingerAutoTuning.m_nShiftStartByte - 1, ParamFingerAutoTuning.m_nShiftByteNumber);

                    if (Tmp_ReportData[0] != 0x07 || //Tmp_ReportData[1] != 0x01 ||
                        Tmp_ReportData[2] != 0xFF || Tmp_ReportData[3] != 0xFF ||
                        Tmp_ReportData[4] != 0xFF || Tmp_ReportData[5] != 0xFF)
                        continue;

                    int TraceTypeIntData = Tmp_ReportData[TraceTypeByte - 1];
                    int IndexData = Tmp_ReportData[IndexByte - 1];

                    if (TraceTypeIntData == RXTypeValue)
                    {
                        int Tmp_RXTraceNumber = Tmp_ReportData[TraceNumberByte - 1];
                        if (bGetRXTraceNumber == false)
                        {
                            RX_TraceNumber = Tmp_RXTraceNumber;
                            bGetRXTraceNumber = true;
                        }
                        else
                        {
                            if (Tmp_RXTraceNumber != RX_TraceNumber)
                                continue;
                        }

                        if (IndexData > LeftBoundary && IndexData < RightBoundary)
                            ValidReportDataCount++;

                        int[] Tmp_DataArray = new int[SectionNumber];
                        for (int i = 0; i < SectionNumber; i++)
                            Tmp_DataArray[i] = (Tmp_ReportData[2 * i + NormalReportDataLength] * 256 + Tmp_ReportData[2 * i + NormalReportDataLength + 1]);

                        OrgDataArray_Rx.Add(new List<int>(Tmp_DataArray));

                        if (IndexData > (int)((SectionNumber / 2) - 1) && IndexData < RX_TraceNumber - (int)(SectionNumber / 2))
                            IndexData = (int)((SectionNumber / 2) - 1);
                        else if (IndexData >= RX_TraceNumber - (int)(SectionNumber / 2))
                        {
                            int TmpValue = RX_TraceNumber - IndexData;
                            IndexData = SectionNumber - TmpValue;
                        }

                        OrgIndexArray_Rx.Add(IndexData);
                    }
                    else if (TraceTypeIntData == TXTypeValue)
                    {
                        int Tmp_TXTraceNumber = Tmp_ReportData[TraceNumberByte - 1];
                        if (bGetTXTraceNumber == false)
                        {
                            TX_TraceNumber = Tmp_TXTraceNumber;
                            bGetTXTraceNumber = true;
                        }
                        else
                        {
                            if (Tmp_TXTraceNumber != TX_TraceNumber)
                                continue;
                        }

                        int[] Tmp_DataArray = new int[SectionNumber];
                        for (int i = 0; i < SectionNumber; i++)
                            Tmp_DataArray[i] = (Tmp_ReportData[2 * i + NormalReportDataLength] * 256 + Tmp_ReportData[2 * i + NormalReportDataLength + 1]);

                        OrgDataArray_Tx.Add(new List<int>(Tmp_DataArray));

                        if (IndexData > (int)((SectionNumber / 2) - 1) && IndexData < TX_TraceNumber - (int)(SectionNumber / 2))
                            IndexData = (int)((SectionNumber / 2) - 1);
                        else if (IndexData >= TX_TraceNumber - (int)(SectionNumber / 2))
                        {
                            int TmpValue = TX_TraceNumber - IndexData;
                            IndexData = SectionNumber - TmpValue;
                        }

                        OrgIndexArray_Tx.Add(IndexData);
                    }
                }

                for (int nIndex = 0; nIndex < OrgDataArray_Rx.Count; nIndex++)
                {
                    if (OrgIndexArray_Rx[nIndex] <= LeftBoundary || OrgIndexArray_Rx[nIndex] >= RightBoundary)
                        continue;

                    int nMaxValue = OrgDataArray_Rx[nIndex].Max();

                    if (OrgDataArray_Rx[nIndex][OrgIndexArray_Rx[nIndex]] == nMaxValue)
                        DataArrayList_Rx.Add(OrgDataArray_Rx[nIndex][OrgIndexArray_Rx[nIndex]]);
                }

                for (int nIndex = 0; nIndex < OrgDataArray_Tx.Count; nIndex++)
                {
                    if (OrgIndexArray_Tx[nIndex] <= LeftBoundary || OrgIndexArray_Tx[nIndex] >= RightBoundary)
                        continue;

                    int nMaxValue = OrgDataArray_Tx[nIndex].Max();

                    if (OrgDataArray_Tx[nIndex][OrgIndexArray_Tx[nIndex]] == nMaxValue)
                        DataArrayList_Tx.Add(OrgDataArray_Tx[nIndex][OrgIndexArray_Tx[nIndex]]);
                }

                if (ValidReportDataCount < ParamFingerAutoTuning.m_nPNSValidReportNumber || ValidReportDataCount == 0)
                {
                    frmMain.m_bCollectFlowError = true;
                    frmMain.m_sCollectFlowErrorMsg = string.Format("No Enough Report Data({0} < LB:{1})",
                                                                   ValidReportDataCount, ParamFingerAutoTuning.m_nPNSValidReportNumber);

                    MessageBox.Show(frmMain.m_sCollectFlowErrorMsg);
                    return false;
                }

                if (DataArrayList_Rx.Count < ParamFingerAutoTuning.m_nPNSRXValidReportNumber || DataArrayList_Rx.Count == 0)
                {
                    frmMain.m_bCollectFlowError = true;
                    frmMain.m_sCollectFlowErrorMsg = string.Format("No Enough RX Report Data({0} < LB:{1})",
                                                                   DataArrayList_Rx.Count, ParamFingerAutoTuning.m_nPNSRXValidReportNumber);

                    MessageBox.Show(frmMain.m_sCollectFlowErrorMsg);
                    return false;
                }

                if (DataArrayList_Tx.Count < ParamFingerAutoTuning.m_nPNSTXValidReportNumber || DataArrayList_Tx.Count == 0)
                {
                    frmMain.m_bCollectFlowError = true;
                    frmMain.m_sCollectFlowErrorMsg = string.Format("No Enough TX Report Data({0} < LB:{1})",
                                                                   DataArrayList_Tx.Count, ParamFingerAutoTuning.m_nPNSTXValidReportNumber);

                    MessageBox.Show(frmMain.m_sCollectFlowErrorMsg);
                    return false;
                }
            }
            else if (m_FlowStep.CollectStep == MainCollectStep.PENTRxSSCAN_CONTACT ||
                     m_FlowStep.CollectStep == MainCollectStep.PENTRxSSCAN_HOVER)
            {
                List<byte> Tmp_ReportData = new List<byte>();

                List<List<int>> OrgDataArray_Rx = new List<List<int>>();
                List<List<int>> OrgDataArray_Tx = new List<List<int>>();
                List<int> OrgIndexArray_Rx = new List<int>();
                List<int> OrgIndexArray_Tx = new List<int>();

                int NormalReportDataLength = 14;
                int SectionNumber = 24;
                int TraceTypeByte = 13;
                int TraceIndexByte = 14;

                int DataLength = DataArray.Count;

                for (int nIndex = 0; nIndex < DataLength; nIndex++)
                {
                    Tmp_ReportData.Clear();
                    Tmp_ReportData = new List<byte>(DataArray[nIndex]);

                    int ReportLength = Tmp_ReportData.Count;
                    if (ReportLength != ParamFingerAutoTuning.m_nReportDataLength)
                        continue;

                    if (ParamFingerAutoTuning.m_nShiftStartByte > 0 && ParamFingerAutoTuning.m_nShiftByteNumber > 0)
                        Tmp_ReportData.RemoveRange(ParamFingerAutoTuning.m_nShiftStartByte - 1, ParamFingerAutoTuning.m_nShiftByteNumber);

                    if (Tmp_ReportData[0] != 0x07 ||
                        Tmp_ReportData[2] != 0xFF || Tmp_ReportData[3] != 0xFF ||
                        Tmp_ReportData[4] != 0xFF || Tmp_ReportData[5] != 0xFF)
                        continue;

                    int TraceTypeIntData = Tmp_ReportData[TraceTypeByte - 1];
                    int IndexData = Tmp_ReportData[TraceIndexByte - 1];

                    if ((TraceTypeIntData & 0x0F) == 0x08)
                    {
                        int[] Tmp_DataArray = new int[SectionNumber];
                        for (int i = 0; i < SectionNumber; i++)
                            Tmp_DataArray[i] = (Tmp_ReportData[2 * i + NormalReportDataLength] * 256 + Tmp_ReportData[2 * i + NormalReportDataLength + 1]);

                        OrgDataArray_Rx.Add(new List<int>(Tmp_DataArray));
                        OrgIndexArray_Rx.Add(IndexData);
                    }
                    else if ((TraceTypeIntData & 0x0F) == 0x00)
                    {
                        int[] Tmp_DataArray = new int[SectionNumber];
                        for (int i = 0; i < SectionNumber; i++)
                            Tmp_DataArray[i] = (Tmp_ReportData[2 * i + NormalReportDataLength] * 256 + Tmp_ReportData[2 * i + NormalReportDataLength + 1]);

                        OrgDataArray_Tx.Add(new List<int>(Tmp_DataArray));
                        OrgIndexArray_Tx.Add(IndexData);
                    }
                }

                if (OrgDataArray_Rx.Count < ParamFingerAutoTuning.m_nPTSRXValidReportNumber || OrgDataArray_Rx.Count == 0)
                {
                    frmMain.m_bCollectFlowError = true;
                    frmMain.m_sCollectFlowErrorMsg = string.Format("No Enough RX Report Data({0} < LB:{1})",
                                                                   OrgDataArray_Rx.Count, ParamFingerAutoTuning.m_nPTSRXValidReportNumber);

                    MessageBox.Show(frmMain.m_sCollectFlowErrorMsg);
                    return false;
                }

                if (OrgDataArray_Tx.Count < ParamFingerAutoTuning.m_nPTSTXValidReportNumber || OrgDataArray_Tx.Count == 0)
                {
                    frmMain.m_bCollectFlowError = true;
                    frmMain.m_sCollectFlowErrorMsg = string.Format("No Enough TX Report Data({0} < LB:{1})",
                                                                   OrgDataArray_Tx.Count, ParamFingerAutoTuning.m_nPTSTXValidReportNumber);

                    MessageBox.Show(frmMain.m_sCollectFlowErrorMsg);
                    return false;
                }
            }
            */

            return true;
        }

        public void SaveLog()
        {
            if (m_cTestPattern == null || m_bTestFinish == false)
                return;

            int nDataLength = m_byteData_List.Count;
            bool bSaveLogComplete = false;

            /*
            string sFileName = string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMddhhmmss"));
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Filter = "Log File(*.txt)|*.txt";
            ofd.FileName = sFileName;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            */

            string sFilePath = string.Format(@"{0}\{1}.txt", m_cfrmMain.m_sRecordLogDirectoryPath, m_cFlowStep.m_sStepName);

            FileStream fs = new FileStream(sFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter(fs);

            /*
            ResultItem[] ItemArray = m_cTestPattern.GetResultItems();
            foreach (ResultItem CurItem in ItemArray)
            {
                sw.WriteLine("{0}, {1}", CurItem.m_sTitle, CurItem.m_sContent);
            }
            */

            try
            {
                for(int nIndex = 0; nIndex < nDataLength; nIndex++)
                {
                    string sReportString = "";

                    int nReportLength = m_byteData_List[nIndex].Count;

                    for(int mIndex = 0; mIndex < nReportLength; mIndex++)
                        sReportString += m_byteData_List[nIndex][mIndex].ToString("X2") + " ";

                    sw.WriteLine(sReportString);
                }

                bSaveLogComplete = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bSaveLogComplete == false)
            {
                if (frmMain.m_bCollectFlowError == false)
                {
                    frmMain.m_bCollectFlowError = true;
                    frmMain.m_sCollectFlowErrorMessage = "Record Log Save Error";
                }

                MessageBox.Show("Record Log Save Error!!");
            }
            else
            {
                if (frmMain.m_bCollectFlowError == false)
                    MessageBox.Show("Record Log Save Complete!!");

                m_bSaveLogComplete = true;
            }
        }

        public void StoreDataArray(byte[] byteData_Array)
        {
            m_byteReport_List.Clear();

            foreach (byte byteData in byteData_Array)
                m_byteReport_List.Add(byteData);

            m_byteData_List.Add(new List<byte>(m_byteReport_List));
        }

        private void OnCanvsPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.WhiteSmoke);
            e.Graphics.DrawImageUnscaled(m_bmpReportLayer, 0, 0);

            lock (m_bmpInfoLayer)
            {
                e.Graphics.DrawImageUnscaled(m_bmpInfoLayer, 0, 0);
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            //Create the bitmap
            pbCanvas.Image = new Bitmap(Width, Height);
            m_bmpReportLayer = new Bitmap(Width, Height);
            m_bmpInfoLayer = new Bitmap(Width, Height);

            //Set the menu location
            if (m_cfrmPatternMenu != null)
                m_cfrmPatternMenu.Location = new Point(Width - m_cfrmPatternMenu.Width - MARGIN, MARGIN);

            if (m_PreScreenOrientation != SystemInformation.ScreenOrientation)
            {
                m_PreScreenOrientation = SystemInformation.ScreenOrientation;
                StartTestProcedure();
            }
        }   

        public void HIDRawInputHandler(object sender, InputDevice.HIDDeviceEventArgs e)
        {
            if (m_bTestFinish == true)
                return;

            if (e.m_Buffer[0] != frmMain.m_nPenReportId)
                return;

            if (m_cTestPattern == null)
                return;

            if (m_cTestPattern.GetItemCount() == 0)
                return;

            /*
            if (e.m_Buffer.Length == ParamFingerAutoTuning.m_nReportDataLength)
            {
                if (m_cFlowStep.CollectStep == MainCollectStep.PENNORMALSCAN_HOVER || m_cFlowStep.CollectStep == MainCollectStep.PENNORMALSCAN_CONTACT)
                {
                    int RXTypeValue = 0x96;
                    int TXTypeValue = 0x97;

                    if (e.m_Buffer[0] == 0x07 && //e.m_Buffer[1] == 0x01 &&
                        e.m_Buffer[2] == 0xFF && e.m_Buffer[3] == 0xFF &&
                        e.m_Buffer[4] == 0xFF && e.m_Buffer[5] == 0xFF)
                    {
                        if (e.m_Buffer[12] == RXTypeValue)
                        {
                            m_bGetRXReport = !m_bGetRXReport;
                            m_pTmpRXBuf = e.m_Buffer;
                        }
                        else if (e.m_Buffer[12] == TXTypeValue)
                        {
                            m_bGetTXReport = !m_bGetTXReport;
                            m_pTmpTXBuf = e.m_Buffer;
                        }
                        else
                        {
                            m_bGetRXReport = false;
                            m_bGetTXReport = false;
                        } 
                    }
                    else
                    {
                        m_bGetRXReport = false;
                        m_bGetTXReport = false;
                    }

                    if (m_bGetRXReport == true && m_bGetTXReport == true)
                        m_bGetReport = true;

                    if (m_bGetReport == true)
                    {
                        if (m_bGetFirstReport == true)
                        {
                            SetBufArray(m_pTmpRXBuf);
                            SetBufArray(m_pTmpTXBuf);
                            m_bGetFirstReport = false;
                        }
                        else
                            SetBufArray(e.m_Buffer);
                    }
                }
                else
                    SetBufArray(e.m_Buffer);
            }
            */

            #region Process the pen report
            lock (m_bmpReportLayer)
            {
                using (Graphics g = Graphics.FromImage(m_bmpReportLayer))
                {
                    ElanReport cCurReport = ProcessPenReport(g, e.m_Buffer);
                    /*
                    if (m_cFlowStep.CollectStep != MainCollectStep.PENNOISE)
                        m_cTestPattern.DoTest(cCurReport, m_fmmPixelX, m_fmmPixelY);
                    */
                    if (m_cTestPattern.TestFinish == true)
                    {
                        if (m_cfrmPatternMenu.IsDisposed == false)
                        {
                            m_cfrmPatternMenu.DisplayStatus("Record Finish");
                            //m_cfrmPatternMenu.ShowResultOnUI(m_cTestPattern.GetResultItems());
                        }

                        bool bReporDataCheck = true;

                        if (this.IsDisposed == false)
                        {
                            bReporDataCheck = CheckReportData();

                            /*
                            if (bReporDataCheck == true)
                                SaveLog();
                            */
                            SaveLog();
                        }

                        if (m_cfrmPatternMenu.IsDisposed == false)
                        {
                            if (bReporDataCheck == false || frmMain.m_bCollectFlowError == true)
                                m_cfrmPatternMenu.DisplayStatus("Record Error");
                            else
                                m_cfrmPatternMenu.DisplayStatus("Record Saved");
                        }

                        if (m_bSaveLogComplete == false)
                        {
                            if (frmMain.m_bCollectFlowError == false)
                            {
                                frmMain.m_bCollectFlowError = true;
                                frmMain.m_sCollectFlowErrorMessage = "Collect Flow Not Complete";
                            }
                        }

                        if (m_cfrmPatternMenu.IsDisposed == false)
                            m_cfrmPatternMenu.ExitFlow();

                        m_bTestFinish = true;
                    }
                }
            }
            #endregion
        }

        private void SetBufArray(byte[] byteBuffer_Array)
        {
            /*
            m_qFIFO.EnqueueAll(byteBuffer_Array, ParamFingerAutoTuning.m_nReportDataLength, 0);

            if (m_qFIFO.DequeueAll(5000, m_pBuf, ParamFingerAutoTuning.m_nReportDataLength) == true)
                StoreDataArray(m_pBuf);
            */
        }
    }
}
