using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;

namespace FingerAutoTuning
{
    public class FreeDrawPattern : TestPattern
    {
        public FreeDrawPattern(PatternParamBase cParamBase, frmMain.FlowStep cFlowStep, Bitmap bmpDrawLayer)
            : base(cParamBase, cFlowStep)
        {
            m_cTestParam = cParamBase;
            m_cFlowStep = cFlowStep;
        }

        public override void CreatePattern()
        {
            m_ItemList.Add(new FreeDrawTestItem());
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
                }
            }
        }
    }

    [Serializable]
    public class FreeDrawTestItem : TestItem
    {
        /// <summary>
        /// A array to store the color that mark the distance.
        /// </summary>
        private Color[] m_ColorMarkerArray = new Color[] {Color.FromArgb(0, 128, 0),
                                                          Color.FromArgb(0, 255, 0),
                                                          Color.FromArgb(128, 255, 0),
                                                          Color.FromArgb(255, 255, 0),
                                                          Color.FromArgb(255, 128, 0),
                                                          Color.FromArgb(255, 0, 0)};

        private int m_nIdx = 0;
        public int nIdx
        {
            get { return m_nIdx; }
        }
        /// <summary>
        /// Record the item create tick
        /// </summary>
        public long m_nCreateTick;

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

        public FreeDrawTestItem()
        {
            InitializeResult();
        }

        public override void DrawItem(Graphics g, Color clrItem, float fmmPixelX, float fmmPixelY)
        {
            //Do nothing 
            return;
            /*
            PointF ptDrawStart = new PointF();
            PointF ptDrawEnd = new PointF();
            Pen LinePen = new Pen(clrItem, 3.0f);
            LinePen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            LinePen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

            g.DrawLine(LinePen, ptDrawStart, ptDrawEnd);
            */
        }

        public override bool DoTest(Graphics g, ElanReport cCurReport, float fmmPixelX, float fmmPixelY)
        {
            if (cCurReport.Mode != ElanReport.ReportMode.Contact &&
                cCurReport.Mode != ElanReport.ReportMode.Hover &&
                cCurReport.Mode != ElanReport.ReportMode.Down &&
                cCurReport.Mode != ElanReport.ReportMode.Up)
            {
                if (m_ptList.Count <= 0)
                    return false;
                else
                {
                    m_bTestDone = true;
                    return false;
                }
            }

            PointF ptCur = cCurReport.toPointmm(fmmPixelX, fmmPixelY);

            m_ptList.Add(cCurReport);

            return true;
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
                    DrawResult(g, ptCur, fDistance, i);*/
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
                    clrMarker = m_ColorMarkerArray[0];
                }
                else if (fDistance >= 0.2f && fDistance < 0.25f)
                {
                    clrMarker = m_ColorMarkerArray[1];
                    fLineSize = fLineSize * 2.0f;
                }
                else if (fDistance >= 0.25f && fDistance < 0.3f)
                {
                    clrMarker = m_ColorMarkerArray[2];
                    fLineSize = fLineSize * 3.0f;
                }
                else if (fDistance >= 0.3f && fDistance < 0.5f)
                {
                    clrMarker = m_ColorMarkerArray[3];
                    fLineSize = fLineSize * 4.0f;
                }
                else
                {
                    clrMarker = m_ColorMarkerArray[4];
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
