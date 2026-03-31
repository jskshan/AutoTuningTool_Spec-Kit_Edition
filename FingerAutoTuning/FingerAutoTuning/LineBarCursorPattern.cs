using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    class LineBarCursorPattern : TestPattern
    {
        #region Define the constant value
        /// <summary>
        /// The slash length(mm)
        /// </summary>
        protected const float SLASH_LENGTH = 110.0f;
        protected const float TEST_SLASH_LENGTH = 1000.0f;

        /// <summary>
        /// The slash ratio
        /// </summary>
        protected const double SLASH_LENGTH_RATIO = 1.0;
        protected const double TEST_SLASH_LENGTH_RATIO = 1.0; //0.97;
        #endregion

        protected RectangleF m_rcSlashRgn = new RectangleF();
        protected RectangleF m_rcTestRgn = new RectangleF();
        protected float m_fCurMoveMaxDistance = 0.0f;

        protected bool m_bStartDispProgressThread = false;
        protected float m_fmmPixelX = 0.0f;
        protected float m_fmmPixelY = 0.0f;

        protected frmPatternMenu m_cfrmPatternMenu = null;
        protected frmPattern m_cfrmPattern = null;

        public LineBarCursorPattern(PatternParamBase cParamBase, frmMain.FlowStep cFlowStep, Bitmap bmpDrawLayer, frmPattern cfrmPatternParent, frmPatternMenu cfrmPatternMenuParent)
            : base(cParamBase, cFlowStep)
        {
            m_cTestParam = cParamBase;
            m_cFlowStep = cFlowStep;
            m_cfrmPattern = cfrmPatternParent;
            m_cfrmPatternMenu = cfrmPatternMenuParent;
        }

        public override void CreatePattern()
        {
            /*
            if (m_cFlowStep.CollectStep != MainCollectStep.PENNOISE)
            {
                //m_rcSlashRgn = GetPartialRect(SLASH_LENGTH);
                //m_rcTestRgn = GetPartialRect(TEST_SLASH_LENGTH);
                m_rcSlashRgn = GetPartialRect(SLASH_LENGTH_RATIO);
                m_rcTestRgn = GetPartialRect(TEST_SLASH_LENGTH_RATIO);
                m_fCurMoveMaxDistance = SetCurMoveMaxDistance(m_rcTestRgn);

                m_ItemList.Add(new LineBarCursorTestItem(m_rcSlashRgn, m_rcTestRgn));
            }
            else
                m_ItemList.Add(new LineBarCursorTestItem(m_rcSlashRgn, m_rcTestRgn, false));
            */
        }

        /*
        protected RectangleF xGetPartialRect(float fSize)
        {
            RectangleF rcMonitor = new RectangleF(0.0f, 0.0f, m_fPhysicalWidth, m_fPhysicalHeight);
            RectangleF rcPartial = new RectangleF();
            double fDiagonalSize = Math.Sqrt((rcMonitor.Right - rcMonitor.Left) * (rcMonitor.Right - rcMonitor.Left) + (rcMonitor.Bottom - rcMonitor.Top) * (rcMonitor.Bottom - rcMonitor.Top));
            double fTestLengthRatio = 1;    //fSize / fDiagonalSize;
            double fStartPosRatio = (1 - fTestLengthRatio) / 2.0f;

            rcPartial.X = 0;    //(float)(m_fPhysicalWidth * fStartPosRatio);
            rcPartial.Y = 0;    //(float)(m_fPhysicalHeight * fStartPosRatio);
            rcPartial.Width = (float)(m_fPhysicalWidth * fTestLengthRatio);
            rcPartial.Height = (float)(m_fPhysicalHeight * fTestLengthRatio);

            return rcPartial;
        }
        */

        protected RectangleF GetPartialRect(double dRatio)
        {
            RectangleF rcMonitor = new RectangleF(0.0f, 0.0f, m_fPhysicalWidth, m_fPhysicalHeight);
            RectangleF rcPartial = new RectangleF();
            double dDiagonalSize = Math.Sqrt((rcMonitor.Right - rcMonitor.Left) * (rcMonitor.Right - rcMonitor.Left) + (rcMonitor.Bottom - rcMonitor.Top) * (rcMonitor.Bottom - rcMonitor.Top));
            double dTestLengthRatio = dRatio;   //fSize / fDiagonalSize;
            double dStartPosRatio = (1 - dTestLengthRatio) / 2.0f;

            rcPartial.X = (float)(m_fPhysicalWidth * dStartPosRatio);
            rcPartial.Y = (float)(m_fPhysicalHeight * dStartPosRatio);
            rcPartial.Width = (float)(m_fPhysicalWidth * dTestLengthRatio);
            rcPartial.Height = (float)(m_fPhysicalHeight * dTestLengthRatio);

            return rcPartial;
        }

        protected float SetCurMoveMaxDistance(RectangleF rcPartial)
        {
            double dCurMoveMaxDistance = Math.Sqrt((rcPartial.Right - rcPartial.Left) * (rcPartial.Right - rcPartial.Left) + (rcPartial.Bottom - rcPartial.Top) * (rcPartial.Bottom - rcPartial.Top));

            return (float)dCurMoveMaxDistance;
        }

        protected PointF GetMovePt(PointF ptStartPos, float fMoveSize)
        {
            RectangleF rcMonitor = new RectangleF(0.0f, 0.0f, m_fPhysicalWidth, m_fPhysicalHeight);
            PointF ptPartial = new PointF();
            double dDiagonalSize = Math.Sqrt((rcMonitor.Right - rcMonitor.Left) * (rcMonitor.Right - rcMonitor.Left) + (rcMonitor.Bottom - rcMonitor.Top) * (rcMonitor.Bottom - rcMonitor.Top));
            double dStartLength = Math.Sqrt((ptStartPos.X - rcMonitor.Left) * (ptStartPos.X - rcMonitor.Left) + (ptStartPos.Y - rcMonitor.Top) * (ptStartPos.Y - rcMonitor.Top));
            double dCurMovePtRatio = (dStartLength + fMoveSize) / dDiagonalSize;

            ptPartial.X = (float)(m_fPhysicalWidth * dCurMovePtRatio);
            ptPartial.Y = (float)(m_fPhysicalHeight * dCurMovePtRatio);

            return ptPartial;
        }

        public override void DrawPattern(float fmmPixelX, float fmmPIxelY)
        {
            if (m_bmpDrawLayer == null)
                return;

            lock (m_bmpDrawLayer)
            {
                using (Graphics g = Graphics.FromImage(m_bmpDrawLayer))
                {
                    g.Clear(Color.Transparent);
                    m_ItemList[0].DrawItem(g, Color.FromArgb(100, 0, 0, 0), fmmPixelX, fmmPIxelY);
                }
            }
        }

        public override void DoTest(ElanReport cCurReport, float fmmPixelX, float fmmPixelY)
        {
            if (m_bmpDrawLayer == null)
                return;

            m_cfrmPattern.m_bSaveLogComplete = false;

            lock (m_bmpDrawLayer)
            {
                using (Graphics g = Graphics.FromImage(m_bmpDrawLayer))
                {
                    if (m_CurTestItem == null)
                    {
                        m_CurTestItem = GetItem(0);
                        m_nCurTestItemIdx = 0;

                        m_fmmPixelX = fmmPixelX;
                        m_fmmPixelY = fmmPixelY;
                    }
                    else if (m_CurTestItem.TestDone == true && m_bCurMoveComplete == true)
                    {
                        #region Check the result and get the next test item.
                        //m_CurTestItem.CheckResult(g, fmmPixelX, fmmPixelY);

                        m_nCurTestItemIdx++;

                        if (m_nCurTestItemIdx >= GetItemCount())
                        {
                            m_bTestFinish = true;
                            m_bStartDispProgressThread = false;
                            return;
                        }
                        #endregion
                    }

                    #region Is the test starting?
                    bool bTestStart = false;
                    /*
                    if (m_FlowStep.CollectStep != MainCollectStep.PENNOISE)
                    {
                        if (m_FlowStep.CollectStep == MainCollectStep.PENNORMALSCAN_HOVER ||
                            m_FlowStep.CollectStep == MainCollectStep.PENNORMALSCAN_CONTACT)
                        {
                            if (CurReport.Mode == ElanReport.ReportMode.Contact ||
                                CurReport.Mode == ElanReport.ReportMode.Hover ||
                                CurReport.Mode == ElanReport.ReportMode.Down ||
                                CurReport.Mode == ElanReport.ReportMode.Up)
                            {
                                if (m_frmPattern.m_bGetReport == true)
                                    bTestStart = true;
                            }
                        }
                        else
                        {
                            if (CurReport.Mode == ElanReport.ReportMode.Contact ||
                                CurReport.Mode == ElanReport.ReportMode.Hover ||
                                CurReport.Mode == ElanReport.ReportMode.Down ||
                                CurReport.Mode == ElanReport.ReportMode.Up)
                                bTestStart = true;
                        }
                    }
                    else
                        bTestStart = true;
                    */

                    if (bTestStart == true && m_bStartDispProgressThread == false)
                    {
                        /*
                        //1.檢查交點是否在測試範圍內
                        //2.檢查Pen Report是否在開始測試範圍內
                        //計算目前的report與斜線的交點
                        PointF CrossPt = xGetCrossPt(CurReport);
                        PointF CrossPtmm = new PointF(CrossPt.X / m_fmmPixelX, CrossPt.Y / m_fmmPixelY);
                        //Compute the distance form report point to cross point
                        float fDistance = (float)Math.Sqrt(((CurReport.X - CrossPt.X) * (CurReport.X - CrossPt.X)) + ((CurReport.Y - CrossPt.Y) * (CurReport.Y - CrossPt.Y)));
                        //Compute the distance form start point.
                        float fDistancemmFromStartPt = (float)Math.Sqrt(((m_rcSlashRgn.Left - CrossPtmm.X) * (m_rcSlashRgn.Left - CrossPtmm.X)) +
                                                                        ((m_rcSlashRgn.Top - CrossPtmm.Y) * (m_rcSlashRgn.Top - CrossPtmm.Y)));

                        //交點是否在斜線範圍內
                        if (m_rcSlashRgn.Contains(CrossPtmm) == true)
                        {
                            //1.Report離交點距離小於5mm
                            //2.交點離斜線起始點小於10mm
                            if (fDistance < (10.0f / 2.0f * m_fmmPixelX) && fDistancemmFromStartPt < 10.0f)
                            {
                                m_frmPattern.m_qFIFO.Clear();
                                m_frmPattern.m_byteData_List.Clear();
                                m_bStartDispProgressThread = true;
                                m_bCurMoveComplete = false;
                                ThreadPool.QueueUserWorkItem(ShowTestProgressThreadFunc);
                            }
                        }
                        */

                        m_cfrmPattern.m_qFIFO.Clear();
                        m_cfrmPattern.m_byteData_List.Clear();
                        m_bStartDispProgressThread = true;
                        m_bCurMoveComplete = false;
                        ThreadPool.QueueUserWorkItem(ShowTestProgressThreadFunc);

                        return;
                    }
                    #endregion

                    m_CurTestItem.DoTest(g, cCurReport, fmmPixelX, fmmPixelY);
                }
            }
        }

        protected PointF GetCrossPt(ElanReport CurReport)
        {
            float fSlope = ((LineBarCursorTestItem)m_CurTestItem).Slope;
            float fConstant = ((LineBarCursorTestItem)m_CurTestItem).ConstValue;

            //取得交點
            float fConstValue = CurReport.Y - ((-1 / fSlope) * CurReport.X);
            float fCrossX = (int)((fConstValue - fConstant) / (fSlope + 1 / fSlope));
            float fCrossY = (int)(fSlope * fCrossX + fConstant);

            return new PointF(fCrossX, fCrossY);
        }

        protected void ShowTestProgressThreadFunc(object objParam)
        {
            long nStart = DateTime.Now.Ticks;
            //long nCurrent = 0;
            //int nCostTime = 0;
            //double dTimeCounter = 0.0;

            m_cfrmPattern.m_bTestRetry = false;

            m_cfrmPatternMenu.DisplayStatus("Record Start");

            double dSlantLength = Math.Sqrt((frmMain.m_nScreenSizeWidth * frmMain.m_nScreenSizeWidth + frmMain.m_nScreenSizeHeight * frmMain.m_nScreenSizeHeight));
            //double dCompleteTime = 0.0;

            /*
            if (m_cFlowStep.CollectStep == MainCollectStep.PENNORMALSCAN_HOVER ||
                m_cFlowStep.CollectStep == MainCollectStep.PENNORMALSCAN_CONTACT)
                dCompleteTime = dSlantLength / ParamFingerAutoTuning.m_nPNSMotionSpeed;
            else if (m_cFlowStep.CollectStep == MainCollectStep.PENTRxSSCAN_HOVER ||
                     m_cFlowStep.CollectStep == MainCollectStep.PENTRxSSCAN_CONTACT)
                dCompleteTime = dSlantLength / ParamFingerAutoTuning.m_nPTSMotionSpeed;
            */

            LineBarCursorParam cParam = (LineBarCursorParam)m_cTestParam;
            //3mm / sec.
            //float fMotionDistance = Param.m_nMotionSpeed / 50.0f;
            /*
            float fMotionDistance = 0.0f;
            if (m_cFlowStep.CollectStep == MainCollectStep.PENNORMALSCAN_HOVER ||
                m_cFlowStep.CollectStep == MainCollectStep.PENNORMALSCAN_CONTACT)
                fMotionDistance = ParamRawDataCollect.m_nPNSMotionSpeed / 50.0f;
            else if (m_cFlowStep.CollectStep == MainCollectStep.PENTRxSSCAN_HOVER ||
                     m_cFlowStep.CollectStep == MainCollectStep.PENTRxSSCAN_CONTACT)
                fMotionDistance = ParamRawDataCollect.m_nPTSMotionSpeed / 50.0f;
            else
                fMotionDistance = Param.m_nMotionSpeed / 50.0f;
            */

            //float fCurMoveDistance = 0.0f;
            PointF ptTestStart = new PointF(m_rcTestRgn.Left, m_rcTestRgn.Top);
            int nPreReportNumber = 0;
            int nCurReportNumber = 0;
            int nNoReportCount = 0;

            //int nTotalReportNumber = 0;
            //int nTimeOut = 0;

            /*
            if (m_cFlowStep.CollectStep == MainCollectStep.PENNOISE)
            {
                nTotalReportNumber = ParamFingerAutoTuning.m_nPNReportNumber * 6;
                nTimeOut = (int)(ParamFingerAutoTuning.m_fPNTimeout * 1000);
            }
            */

            while (m_bStartDispProgressThread == true)
            {
                if (m_bCurMoveComplete == true)
                {
                    if (m_CurTestItem.PenOn == false)
                    {
                        m_bTestFinish = true;
                        m_cfrmPattern.m_bTestFinish = true;
                        break;
                    }

                    nCurReportNumber = m_cfrmPattern.m_byteData_List.Count;

                    if (nCurReportNumber == nPreReportNumber)
                        nNoReportCount++;
                    else
                        nNoReportCount = 0;

                    if (nNoReportCount > 3)
                    {
                        m_bTestFinish = true;
                        m_cfrmPattern.m_bTestFinish = true;
                        break;
                    }
                }
                else
                {
                    /*
                    if (m_cFlowStep.CollectStep != MainCollectStep.PENNOISE)
                    {
                        PointF rcMoveStart = xGetMovePt(ptTestStart, fCurMoveDistance);

                        nCurrent = DateTime.Now.Ticks;
                        nCostTime = (int)((nCurrent - nStart) / 10000);
                        dTimeCounter = Math.Round((double)nCostTime / 1000, 3, MidpointRounding.AwayFromZero);

                        fCurMoveDistance = (float)(dSlantLength * (dTimeCounter / dCompleteTime));

                        //fCurMoveDistance += fMotionDistance;
                        PointF rcMoveEnd = xGetMovePt(ptTestStart, fCurMoveDistance);

                        lock (m_bmpDrawLayer)
                        {
                            using (Graphics g = Graphics.FromImage(m_bmpDrawLayer))
                            {
                                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                                g.DrawLine(new Pen(Color.FromArgb(100, Color.Blue), 10.0f * m_fmmPixelX), rcMoveStart.X * m_fmmPixelX, rcMoveStart.Y * m_fmmPixelY, rcMoveEnd.X * m_fmmPixelX, rcMoveEnd.Y * m_fmmPixelY);
                                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                            }
                        }

                        //if (fCurMoveDistance > TEST_SLASH_LENGTH)
                        if (fCurMoveDistance > m_fCurMoveMaxDistance)
                        {
                            m_bCurMoveComplete = true;

                            if (m_CurTestItem.PenOn == false)
                            {
                                m_bTestFinish = true;
                                m_cfrmPattern.m_bTestFinish = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        nCurrent = DateTime.Now.Ticks;
                        nCostTime = (int)((nCurrent - nStart) / 10000);
                        dTimeCounter = Math.Round((double)nCostTime / 1000, 3, MidpointRounding.AwayFromZero);

                        if (m_cfrmPattern.DataArray.Count >= nTotalReportNumber ||
                            nCostTime > nTimeOut)
                        {
                            m_cfrmPatternMenu.DisplayTimeAndReportNumber(dTimeCounter, true);
                            nPreReportNumber = m_frmPattern.DataArray.Count;

                            m_bTestFinish = true;
                            m_cfrmPattern.m_bTestFinish = true;
                            break;
                        }
                    }
                    */
                }
                

                /*
                if (m_cFlowStep.CollectStep != MainCollectStep.PENNOISE)
                    m_cfrmPatternMenu.DisplayTimeAndReportNumber(0);
                else
                    m_cfrmPatternMenu.DisplayTimeAndReportNumber(dTimeCounter, true);
                */

                nPreReportNumber = m_cfrmPattern.m_byteData_List.Count;

                Thread.Sleep(20);
            }

            if (m_cfrmPattern.m_bTestRetry == false)
            {
                if (m_cfrmPatternMenu.IsDisposed == false)
                {
                    m_cfrmPatternMenu.Invoke((MethodInvoker)delegate
                    {
                        m_cfrmPatternMenu.DisplayStatus("Record Finish");
                        //m_cfrmPatternMenu.ShowResultOnUI(m_CurTestItem.GetResultItems());
                    });
                }

                bool bReporDataCheck = true;

                if (m_cfrmPattern.IsDisposed == false && m_cfrmPattern.m_bFormClose == false)
                {
                    m_cfrmPattern.Invoke((MethodInvoker)delegate
                    {
                        bReporDataCheck = m_cfrmPattern.CheckReportData();

                        /*
                        if (bReporDataCheck == true)
                            m_cfrmPattern.SaveLog();
                        */
                        m_cfrmPattern.SaveLog();
                    });
                }

                if (m_cfrmPatternMenu.IsDisposed == false)
                {
                    if (bReporDataCheck == false || frmMain.m_bCollectFlowError == true)
                        m_cfrmPatternMenu.DisplayStatus("Record Error");
                    else
                        m_cfrmPatternMenu.DisplayStatus("Record Saved");
                }

                if (m_cfrmPattern.m_bSaveLogComplete == false)
                {
                    if (frmMain.m_bCollectFlowError == false)
                    {
                        frmMain.m_bCollectFlowError = true;
                        frmMain.m_sCollectFlowErrorMessage = "Collect Flow Not Complete";
                    }
                }

                if (m_cfrmPatternMenu.IsDisposed == false)
                    m_cfrmPatternMenu.ExitFlow();
            }
        }

        protected override void DoDispose()
        {
            m_bStartDispProgressThread = false;
            m_bTestFinish = true;
        }
    }

    [Serializable]
    public class LineBarCursorParam : PatternParamBase
    {
        public int m_nMotionSpeed = 0;

        public LineBarCursorParam(float fMonitorWidth, float fMonitorHeight)
            : base(fMonitorWidth, fMonitorHeight)
        {
            m_nPatternType = (int)PatternType.TX8VerTest;
        }

        protected override void LoadParameter()
        {
            LoadParameter("Diagonal Set");
        }

        protected override void LoadParameterByGroup(string sGroupName)
        {
            //GetParamValue(ref m_nMotionSpeed, sGroupName, "Speed", "10");
            m_nMotionSpeed = 10;
        }
    }

    [Serializable]
    public class LineBarCursorTestItem : TestItem
    {
        /// <summary>
        /// A array to store the color that mark the distance.
        /// </summary>
        private Color[] m_clrMarker_Array = new Color[] 
        {
            Color.FromArgb(0, 128, 0),
            Color.FromArgb(0, 255, 0),
            Color.FromArgb(128, 255, 0),
            Color.FromArgb(255, 255, 0),
            Color.FromArgb(255, 128, 0),
            Color.FromArgb(255, 0, 0)
        };

        private int m_nIndex = 0;
        public int Index
        {
            get { return m_nIndex; }
        }
        /// <summary>
        /// Record the item create tick
        /// </summary>
        public long m_nCreateTick;

        #region
        PointF m_ptDownLeftTop = new PointF();
        PointF m_ptDownRightBottom = new PointF();
        PointF m_ptUpLeftTop = new PointF();
        PointF m_ptUpRightBottom = new PointF();
        protected float m_fSlope = 0.0f;
        public float Slope
        {
            get { return m_fSlope; }
        }
        protected float m_fConstValue = 0.0f;
        public float ConstValue
        {
            get { return m_fConstValue; }
        }
        RectangleF m_rcSlashRgn;
        RectangleF m_rcTestRgn;
        #endregion

        #region The criteria of test item
        /// <summary>
        /// The threshold of distance.(unit:mm)
        /// </summary>
        protected float m_fDistanceHB = 0.0f;
        /// <summary>
        /// The threshold uses to check the result of linearity test
        /// </summary>
        public float Spec
        {
            get { return m_fDistanceHB; }
        }
        protected RectangleF m_rcBoundary;

        #endregion

        protected int m_nPtCount = 0;
        protected int m_nOverSpecPtCount = 0;
        protected float m_fMaxDistance = 0.0f;
        protected float m_fAverageDistance = 0.0f;

        /// <summary>
        /// Record all the report that use to do test
        /// </summary>
        private List<ElanReport> m_ptList = new List<ElanReport>();

        public LineBarCursorTestItem(RectangleF SlashRgn, RectangleF rcTestRgn, bool bCreateLinePattern = true)
        {
            InitializeResult();

            if (bCreateLinePattern == true)
            {
                m_rcSlashRgn = SlashRgn;
                m_rcTestRgn = rcTestRgn;
                CreateLinePattern(SlashRgn);
            }
        }

        //建立線性測試的pattern
        private void CreateLinePattern(RectangleF rcBoundary)
        {
            float fDiagonal = 0.0f;
            float fMaxDelta = 0.0f;

            //Compute the Diagonal(mm)
            fDiagonal = (float)Math.Sqrt((double)(rcBoundary.Width * rcBoundary.Width) + (rcBoundary.Height * rcBoundary.Height));
            //Compute the max delta(mm)
            fMaxDelta = 5.0f;

            //Compute the Slope
            m_fSlope = ((float)(rcBoundary.Top - rcBoundary.Bottom) / (float)(rcBoundary.Left - rcBoundary.Right));
            m_fConstValue = rcBoundary.Bottom - (m_fSlope * rcBoundary.Right);
            //Compute Left-Top to Right-Bottom diagonal
            float fDownLeftTopConstVal = ComputeDown(m_fSlope, fMaxDelta, rcBoundary, rcBoundary, ref m_ptDownLeftTop, ref m_ptDownRightBottom, false);
            float fUpLeftTopConstVal = ComputeUp(m_fSlope, fMaxDelta, rcBoundary, rcBoundary, ref m_ptUpLeftTop, ref m_ptUpRightBottom, false);
        }

        //傳入Slope(斜率), 斜邊, Client, ptStart, ptEnd
        float ComputeDown(float fSlope, float fMaxDelta, RectangleF rcClient, RectangleF rcBoundary,
                          ref PointF ptStart, ref PointF ptEnd, bool bRightTop)
        {
            //取得角度
            //float fAngle = (float)(atan((-1/fSlope)) * (180 / PI));
            float fAngle = (float)(Math.Atan((-1 / fSlope)));

            //取得向下位移的(x, y)
            float fy = 0.0f;
            float fx = 0.0f;
            float fConstValue = 0.0f;

            if (bRightTop)
            {
                fy = rcClient.Top + (fMaxDelta / 2) * (float)Math.Sin(fAngle);
                fx = rcClient.Right + (fMaxDelta / 2) * (float)Math.Cos(fAngle);

                //利用y = mx + b這個等式計算 b = y - mx
                fConstValue = fy - (fSlope * fx);

                //利用y = mx + b計算真正的y
                fy = fSlope * rcBoundary.Right + fConstValue;

                ptStart.X = rcBoundary.Right;
            }
            else
            {
                fy = rcClient.Top - (fMaxDelta / 2) * (float)Math.Sin(fAngle);
                fx = rcClient.Left - (fMaxDelta / 2) * (float)Math.Cos(fAngle);

                //利用y = mx + b這個等式計算 b = y - mx
                fConstValue = fy - (fSlope * fx);

                //利用y = mx + b計算真正的y
                fy = fSlope * rcBoundary.Left + fConstValue;

                ptStart.X = rcBoundary.Left;
            }

            ptStart.Y = fy;

            //End
            ptEnd = new PointF((float)(rcBoundary.Bottom - fConstValue) / fSlope, (float)rcBoundary.Bottom);

            return fConstValue;
        }

        //傳入Slope(斜率), 斜邊, Client, ptStart, ptEnd
        float ComputeUp(float fSlope, float fMaxDelta, RectangleF rcClient, RectangleF rcBoundary,
                        ref PointF ptStart, ref PointF ptEnd, bool bRightTop)
        {
            //取得角度
            //float fAngle = (float)(atan((-1/fSlope)) * (180 / PI));
            float fAngle = (float)(Math.Atan((-1 / fSlope)));

            //取得向下位移的(x, y)
            float fy = 0.0f;
            float fx = 0.0f;
            float fConstValue = 0.0f;

            if (bRightTop)
            {
                fy = rcClient.Top - ((fMaxDelta / 2) * (float)Math.Sin(fAngle));
                fx = rcClient.Right - (fMaxDelta / 2) * (float)Math.Cos(fAngle);

                //利用y = mx + b這個等式計算 b = y - mx
                fConstValue = fy - (fSlope * fx);

                //利用y = mx + b計算真正的x
                ptStart.X = (rcBoundary.Top - fConstValue) / fSlope;

                fy = (float)rcBoundary.Top;

                ptStart.Y = fy;

                //End
                ptEnd = new PointF((float)rcBoundary.Left, rcBoundary.Left * fSlope + fConstValue);
            }
            else
            {
                fy = rcClient.Top + (fMaxDelta / 2) * (float)Math.Sin(fAngle);
                fx = rcClient.Left + (fMaxDelta / 2) * (float)Math.Cos(fAngle);

                //利用y = mx + b這個等式計算 b = y - mx
                fConstValue = fy - (fSlope * fx);

                //利用y = mx + b計算真正的x
                ptStart.X = (rcBoundary.Top - fConstValue) / fSlope;

                fy = (float)rcBoundary.Top;

                ptStart.Y = fy;

                //End
                ptEnd = new PointF((float)rcBoundary.Right, rcBoundary.Right * fSlope + fConstValue);
            }

            return fConstValue;
        }

        public override void DrawItem(Graphics g, Color clrItem, float fmmPixelX, float fmmPixelY)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            PointF ptDrawStart = Pt2Pixels(new PointF(m_rcSlashRgn.Left, m_rcSlashRgn.Top), fmmPixelX, fmmPixelY);
            PointF ptDrawEnd = Pt2Pixels(new PointF(m_rcSlashRgn.Right, m_rcSlashRgn.Bottom), fmmPixelX, fmmPixelY);
            Pen LinePen = new Pen(clrItem, 10.0f * fmmPixelX);
            g.DrawLine(LinePen, ptDrawStart, ptDrawEnd);

            LinePen = new Pen(Color.FromArgb(100, 240, 240, 240), 9.0f * fmmPixelX);
            ptDrawStart = Pt2Pixels(new PointF(m_rcTestRgn.Left, m_rcTestRgn.Top), fmmPixelX, fmmPixelY);
            ptDrawEnd = Pt2Pixels(new PointF(m_rcTestRgn.Right, m_rcTestRgn.Bottom), fmmPixelX, fmmPixelY);
            g.DrawLine(LinePen, ptDrawStart, ptDrawEnd);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            return;
        }

        public override bool DoTest(Graphics g, ElanReport CurReport, float fmmPixelX, float fmmPixelY)
        {
            /*if (CurReport.Mode != ElanReport.ReportMode.Contact ||
                xComputeDistanceFromCrossPt(CurReport) >= 10.0f / 2.0f * fmmPixelX)*/
            if (CurReport.Mode != ElanReport.ReportMode.Contact &&
                CurReport.Mode != ElanReport.ReportMode.Hover &&
                CurReport.Mode != ElanReport.ReportMode.Down &&
                CurReport.Mode != ElanReport.ReportMode.Up)
            {
                m_bPenOn = false;

                if (m_ptList.Count <= 0)
                    return false;
                else
                {
                    m_bTestDone = true;
                    return false;
                }
            }
            else
            {
                m_bPenOn = true;
                m_bTestDone = false;
            }

            PointF ptCurmm = CurReport.toPointmm(fmmPixelX, fmmPixelY);

            if (m_rcSlashRgn.Contains(ptCurmm) == false)
                return false;

            PointF ptCur = CurReport.toPointmm(fmmPixelX, fmmPixelY);

            m_ptList.Add(CurReport);

            return true;
        }

        protected float ComputeDistanceFromCrossPt(ElanReport CurReport)
        {
            //取得交點
            float fConstValue = CurReport.Y - ((-1 / m_fSlope) * CurReport.X);
            float fCrossX = (int)((fConstValue - m_fConstValue) / (m_fSlope + 1 / m_fSlope));
            float fCrossY = (int)(m_fSlope * fCrossX + m_fConstValue);
            //Compute the distance form report point to cross point
            float fDistance = (float)Math.Sqrt(((CurReport.X - fCrossX) * (CurReport.X - fCrossX)) + ((CurReport.Y - fCrossY) * (CurReport.Y - fCrossY)));

            return fDistance;
        }

        public override void CheckResult(Graphics g, float fmmPixelX, float fmmPixelY)
        {
            if (m_ptList.Count <= 0)
                return;

            PointF ptMaxDistance = CheckRegResult(g, fmmPixelX, fmmPixelY);

            if (ptMaxDistance == null)
                return;

            //Set the information of test reulst
            m_Result.nPtNum = m_ptList.Count;
            m_Result.fAvgErr = (float)Math.Round(m_fAverageDistance, 2, MidpointRounding.AwayFromZero);

            for (int i = 0; i < m_Result.ErrStatisticList.Count; i++)
            {
                m_Result.ErrStatisticList[i] = (float)Math.Round((m_Result.ErrStatisticList[i] / (float)m_Result.nPtNum * 100.0f), 2, MidpointRounding.AwayFromZero);
            }
        }

        public override ResultItem[] GetResultItems()
        {
            List<ResultItem> cItem_List = new List<ResultItem>();

            cItem_List.Add(new ResultItem("Max error", string.Format("{0} (mm)", m_Result.fMaxErr)));
            cItem_List.Add(new ResultItem("Average error", string.Format("{0} (mm)", m_Result.fAvgErr)));
            cItem_List.Add(new ResultItem("RMS error", string.Format("{0} (mm)", m_Result.fRMSErr)));
            cItem_List.Add(new ResultItem("Error > 0.5mm", string.Format("{0}%", m_Result.ErrStatisticList[0])));
            cItem_List.Add(new ResultItem("Error > 0.3mm", string.Format("{0}%", m_Result.ErrStatisticList[1])));
            cItem_List.Add(new ResultItem("Error > 0.25mm", string.Format("{0}%", m_Result.ErrStatisticList[2])));
            cItem_List.Add(new ResultItem("Error > 0.2mm", string.Format("{0}%", m_Result.ErrStatisticList[3])));
            cItem_List.Add(new ResultItem("Error <= 0.2mm", string.Format("{0}%", m_Result.ErrStatisticList[4])));

            return cItem_List.ToArray();
        }

        /// <summary>
        /// Compute the Regression line and compute the distance of point to the line. 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="fmmPixelX"></param>
        /// <param name="fmmPixelY"></param>
        private PointF CheckRegResult(Graphics g, float fmmPixelX, float fmmPixelY)
        {
            Pen RegLinePen = new Pen(Color.FromArgb(180, Color.Blue), 1.0f);
            PointF ptStart = new PointF();
            PointF ptEnd = new PointF();
            PointF ptRegStart;
            PointF ptRegEnd;

            GetTerminalPoints(ref ptStart, ref ptEnd, fmmPixelX, fmmPixelY);

            //計算線性回歸
            //Y = aX+b
            float fXSum = 0.0f;
            float fXXSum = 0.0f;
            float fYSum = 0.0f;
            float fXYSum = 0.0f;
            float fXAvg = 0.0f;
            float fYAvg = 0.0f;
            float fSlope = 0.0f;

            //計算斜率
            if (ptStart.X - ptEnd.X != 0)
                fSlope = (ptStart.Y - ptEnd.Y) / (ptStart.X - ptEnd.X);
            else
                fSlope = 100.0f;

            if (fSlope <= 1.0f)  //Horizontal
            {
                foreach (ElanReport ptCur in m_ptList)
                {
                    PointF ptmmCur = ptCur.toPointmm(fmmPixelX, fmmPixelY);
                    fXSum += ptmmCur.X;
                    fXXSum += (ptmmCur.X * ptmmCur.X);
                    fYSum += ptmmCur.Y;
                    fXYSum += (ptmmCur.X * ptmmCur.Y);
                }
            }
            else    //Vertical
            {
                foreach (ElanReport ptCur in m_ptList)
                {
                    PointF ptmmCur = ptCur.toPointmm(fmmPixelX, fmmPixelY);
                    fXSum += ptmmCur.Y;
                    fXXSum += (ptmmCur.Y * ptmmCur.Y);
                    fYSum += ptmmCur.X;
                    fXYSum += (ptmmCur.Y * ptmmCur.X);
                }
            }

            fXAvg = fXSum / m_ptList.Count;
            fYAvg = fYSum / m_ptList.Count;

            float fb = (m_ptList.Count * fXYSum - fXSum * fYSum) / (m_ptList.Count * fXXSum - fXSum * fXSum);
            float fa = fYAvg - fb * fXAvg;

            if (fSlope <= 1.0f)  //Horizontal
            {
                ptRegStart = new PointF(ptStart.X, fa + fb * ptStart.X);
                ptRegEnd = new PointF(ptEnd.X, fa + fb * ptEnd.X);
            }
            else    //Vertical
            {
                ptRegStart = new PointF(fb * ptStart.Y + fa, ptStart.Y);
                ptRegEnd = new PointF(fb * ptEnd.Y + fa, ptEnd.Y);
            }

            PointF ptMaxDistance = new PointF();

            //回歸線
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float fDistanceSquareSum = 0.0f;

            for (int i = 0; i < m_ptList.Count; i++)
            {
                ElanReport ptCur = m_ptList[i];
                float fDistance = ComputeDistance(ptRegStart, ptRegEnd, ptCur.toPointmm(fmmPixelX, fmmPixelY));

                DoStatistic(fDistance);

                //Compute the RMS Error
                fDistanceSquareSum += fDistance * fDistance;

                //Check the spec. of distance
                if (fDistance > m_fDistanceHB)
                    m_nOverSpecPtCount++;

                if (fDistance > m_Result.fMaxErr)
                {
                    m_fMaxDistance = m_Result.fMaxErr = fDistance;
                    ptMaxDistance = ptCur.toPointmm(fmmPixelX, fmmPixelY);
                }

                m_fAverageDistance += fDistance;

                //Draw the line and mark with specific color.
                /*if (i >= 1)
                    xDrawResult(g, ptCur, fDistance, i);*/
            }

            //Compute the average distance
            m_fAverageDistance = m_fAverageDistance / (float)m_ptList.Count;
            m_Result.fRMSErr = (float)Math.Sqrt(fDistanceSquareSum / (float)m_ptList.Count);
            m_Result.fRMSErr = (float)Math.Round(m_Result.fRMSErr, 2, MidpointRounding.AwayFromZero);
            Console.WriteLine("RegStartPt:({0}, {1}), RegEndPt:({2}, {3}), ptCur:({4}, {5}), Distance : {6}",
                                ptRegStart.X, ptRegStart.Y, ptRegEnd.X, ptRegEnd.Y, ptMaxDistance.X, ptMaxDistance.Y,
                                m_Result.fMaxErr);

            //繪製回歸線,以及點到回歸線最大距離
            RegLinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawLine(RegLinePen, new PointF(ptRegStart.X * fmmPixelX, ptRegStart.Y * fmmPixelY),
                                new PointF(ptRegEnd.X * fmmPixelX, ptRegEnd.Y * fmmPixelY));

            /*PointF ptCross = new PointF();
            ptCross = xComputeCrossPt(ptRegStart, ptRegEnd, ptMaxDistance);
            g.DrawLine(new Pen(Color.Red, 2.0f), new PointF(ptMaxDistance.X * fmmPixelX, ptMaxDistance.Y * fmmPixelY),
                                new PointF(ptCross.X * fmmPixelX, ptCross.Y * fmmPixelY));*/

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.Flush();

            return ptMaxDistance;
        }

        /// <summary>
        /// Find the start point and end point.
        ///  規則:所有點中找出左上跟右下兩個端點
        /// </summary>
        /// <param name="ptStart"></param>
        /// <param name="ptEnd"></param>
        private void GetTerminalPoints(ref PointF ptStart, ref PointF ptEnd, float fmmPixelX, float fmmPixelY)
        {
            for (int i = 0; i < m_ptList.Count; i++)
            {
                PointF CurReport = m_ptList[i].toPointmm(fmmPixelX, fmmPixelY);

                if (i == 0)
                {
                    ptStart.X = CurReport.X;
                    ptEnd.X = CurReport.X;
                    ptStart.Y = CurReport.Y;
                    ptEnd.Y = CurReport.Y;
                }
                else
                {
                    if (ptStart.X > CurReport.X)
                        ptStart.X = CurReport.X;

                    if (ptEnd.X < CurReport.X)
                        ptEnd.X = CurReport.X;

                    if (ptStart.Y > CurReport.Y)
                        ptStart.Y = CurReport.Y;

                    if (ptEnd.Y < CurReport.Y)
                        ptEnd.Y = CurReport.Y;
                }
            }
        }

        /// <summary>
        /// 繪製該點與回歸線距離,使用不同顏色與粗細標示
        /// </summary>
        /// <param name="g"></param>
        /// <param name="ptCur"></param>
        /// <param name="fDistance"></param>
        /// <param name="i"></param>
        private void DrawResult(Graphics g, ElanReport ptCur, float fDistance, int i)
        {
            ElanReport ptPre = m_ptList[i - 1];

            if (ptPre.Mode != ElanReport.ReportMode.Up && ptCur.Mode != ElanReport.ReportMode.Up)
            {
                Pen penResult = null;
                Color clrMarker;
                float fLineSize = 0.5f;

                if (fDistance < 0.2f)
                {
                    clrMarker = m_clrMarker_Array[0];
                }
                else if (fDistance >= 0.2f && fDistance < 0.25f)
                {
                    clrMarker = m_clrMarker_Array[1];
                    fLineSize = fLineSize * 2.0f;
                }
                else if (fDistance >= 0.25f && fDistance < 0.3f)
                {
                    clrMarker = m_clrMarker_Array[2];
                    fLineSize = fLineSize * 3.0f;
                }
                else if (fDistance >= 0.3f && fDistance < 0.5f)
                {
                    clrMarker = m_clrMarker_Array[3];
                    fLineSize = fLineSize * 4.0f;
                }
                else
                {
                    clrMarker = m_clrMarker_Array[4];
                    fLineSize = fLineSize * 5.0f;
                }

                penResult = new Pen(clrMarker, fLineSize);
                g.DrawLine(penResult, ptPre.X, ptPre.Y, ptCur.X, ptCur.Y);
            }

        }

        //Compute the cross point from point to line.
        PointF ComputeCrossPt(PointF ptStart, PointF ptEnd, PointF ptCur)
        {
            float A = (ptStart.Y - ptEnd.Y) / (ptStart.X - ptEnd.X);
            float B = -1;
            float C = ptStart.Y - A * ptStart.X;

            PointF ptCross = new PointF();
            ptCross.X = (B * B * ptCur.X - A * B * ptCur.Y - A * C) / (A * A + B * B);
            ptCross.Y = (-A * B * ptCur.X + A * A * ptCur.Y - B * C) / (A * A + B * B);

            return ptCross;
        }

        /// <summary>
        /// compute the distance form points to line.
        /// </summary>
        /// <param name="ptStart"></param>
        /// <param name="ptEnd"></param>
        /// <param name="ptCur"></param>
        /// <returns>回傳距離(四捨五入到小數點第二位)</returns>
        float ComputeDistance(PointF ptStart, PointF ptEnd, PointF ptCur)
        {
            /*float A = (ptEnd.Y - ptStart.Y);
            float B = (ptStart.X - ptEnd.X);
            float C = ptEnd.X * ptStart.Y + ptStart.X * ptEnd.Y;*/

            float A = (ptStart.Y - ptEnd.Y) / (ptStart.X - ptEnd.X);
            float B = -1;
            float C = ptStart.Y - A * ptStart.X;

            float fDistance = (float)Math.Abs(((A * ptCur.X + B * ptCur.Y + C) / Math.Sqrt(A * A + B * B)));
            fDistance = (float)Math.Round(fDistance, 2, MidpointRounding.AwayFromZero);

            return fDistance;
        }
    }
}

