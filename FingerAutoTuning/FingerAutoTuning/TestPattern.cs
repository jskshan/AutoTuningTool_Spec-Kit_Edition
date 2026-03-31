using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using Elan;

namespace FingerAutoTuning
{
    /// <summary>
    /// A class to store the report information
    /// </summary>
    public class ElanReport
    {
        public enum ReportType
        {
            Finger = 0,
            ActivePen
        }

        public enum ReportMode
        {
            Up = 0,
            Down,
            Hover,
            Contact
        }

        private float m_fX = 0;
        /// <summary>
        /// X (Unit:Pixels)
        /// </summary>
        public float X
        {
            get { return m_fX; }
        }

        private float m_fY = 0;
        /// <summary>
        /// Y (Unit:Pixels)
        /// </summary>
        public float Y
        {
            get { return m_fY; }
        }

        private int m_nPressure;
        public int Pressure
        {
            get { return m_nPressure; }
        }

        private long m_nTick = 0;
        /// <summary>
        /// The tick time from test start.
        /// </summary>
        public long Tick
        {
            get { return m_nTick; }
        }

        private ReportType m_Type;
        public ReportType reportType
        {
            get { return m_Type; }
        }

        private ReportMode m_Mode;
        public ReportMode Mode
        {
            get { return m_Mode; }
        }

        public ElanReport(float fX, float fY, int nPressure, long nTick, ReportType Type, ReportMode Mode)
        {
            m_fX = fX;
            m_fY = fY;
            m_nPressure = nPressure;
            m_nTick = nTick;
            m_Type = Type;
            m_Mode = Mode;
        }

        public PointF toPointmm(float fmmXPixels, float fmmYPixels)
        {
            return new PointF((float)Math.Round(X / fmmXPixels, 3, MidpointRounding.AwayFromZero), (float)Math.Round(Y / fmmYPixels, 3, MidpointRounding.AwayFromZero));
        }
    }

    /// <summary>
    /// A base class to declare the public variable and function.
    /// All the parameter of linearity test need inherit this class.
    /// </summary>
    [Serializable]
    public class PatternParamBase : ISerializable
    {
        protected const int FILE_TYPE_UNKNOW = 0;
        protected const int FILE_TYPE_INI = 1;
        protected const int FILE_TYPE_XML = 2;

        /// <summary>
        /// Pattern Type
        /// </summary>
        public int m_nPatternType = -1;

        protected int m_nFileType = FILE_TYPE_UNKNOW;

        /// <summary>
        /// The monitor size that user set...
        /// </summary>
        public float m_fMonitorWidth = 0.0f;
        public float m_fMonitorHeight = 0.0f;
        protected string m_sPath = "";

        /// <summary>
        /// Which type report uses to do test.
        /// 0 : Finger
        /// 1 : Active Pen Contact
        /// 2 : Active Pen Hover
        /// </summary>
        public int m_nReportType = 0;

        public PatternParamBase()
        {
        }

        public PatternParamBase(SerializationInfo info, StreamingContext context)
        {
            m_nPatternType = (int)info.GetValue("m_nPatternType", typeof(int));
            m_fMonitorWidth = (float)info.GetValue("m_fMonitorWidth", typeof(float));
            m_fMonitorHeight = (float)info.GetValue("m_fMonitorHeight", typeof(float));
            m_nReportType = (int)info.GetValue("m_nReportType", typeof(int));
        }

        public PatternParamBase(float fMonitorWidth, float fMonitorHeight)
        {
            m_fMonitorWidth = fMonitorWidth;
            m_fMonitorHeight = fMonitorHeight;
        }

        //Declare the basic function that load the parameter data from file
        public void LoadParameter(string sFileName)
        {
            m_sPath = sFileName;
            m_nFileType = FILE_TYPE_XML;
            //Clear the XML to load the last update data.
            //clsElanXML.Clear();
            LoadParameter();
        }

        /// <summary>
        /// Load the parameter from ini file(Use in script mode)
        /// </summary>
        /// <param name="sFileName"></param>
        /// <param name="sGroupName"></param>
        public void LoadParameter(string sFileName, string sGroupName)
        {
            m_sPath = sFileName;
            m_nFileType = FILE_TYPE_INI;
            LoadParameterByGroup(sGroupName);
        }

        /// <summary>
        /// All the children class need implement this function
        /// </summary>
        protected virtual void LoadParameter()
        {

        }

        /// <summary>
        /// All the children class need implement this function
        /// </summary>
        protected virtual void LoadParameterByGroup(string sGroupName)
        {

        }

        #region Declare the general functions for loading the parameter value.

        public void GetParamValue(ref int nValue, string sGroupName, string sParamName,
                                                                    string sDefault, bool bHex = false)
        {
            string sIniValue = ReadValue(sGroupName, sParamName, sDefault);

            try
            {
                if (bHex == true)
                {
                    if (sIniValue == "-1" || sIniValue == "")
                        nValue = -1;
                    else
                        nValue = Int32.Parse(sIniValue, System.Globalization.NumberStyles.HexNumber);
                }
                else
                    nValue = Convert.ToInt32(sIniValue);
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParamName, sIniValue, e.Message.ToString());
                Console.WriteLine(sState);
            }
        }

        // 宣告取得Parameter Value
        protected void GetParamValue(ref bool bValue, string sGroupName, string sParamName, string sDefault)
        {
            string sIniValue = ReadValue(sGroupName, sParamName, sDefault);
            try
            {
                bValue = Convert.ToInt32(sIniValue) >= 1;
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParamName, sIniValue, e.Message.ToString());
                Console.WriteLine(sState);
            }
        }

        public void GetParamValue(ref float fValue, string sGroupName, string sParamName, string sDefault)
        {
            string sIniValue = ReadValue(sGroupName, sParamName, sDefault);
            if (float.TryParse(sIniValue, out fValue) == false)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //Error digit format", sGroupName, sParamName, sIniValue);
                Console.WriteLine(sState);
            }
        }

        /// <summary>
        /// 從參數設定檔讀取參數
        /// </summary>
        /// <param name="Section">Group Name</param>
        /// <param name="Key">Parameter Name</param>
        /// <param name="Default">預設值</param>
        /// <returns>回傳讀取的結果</returns>
        protected string ReadValue(string Section, string Key, string Default = "")
        {
            if (m_nFileType == FILE_TYPE_XML)
                return XmlReadValue(Section, Key, Default);
            else
                return Default;
        }

        protected string XmlReadValue(string Section, string Key, string Default = "")
        {
            clsElanXML ElanProp = clsElanXML.GetInstance(m_sPath, false);
            if (ElanProp == null)
                return Default;

            string sValue = ElanProp.GetValue(Section.Replace(' ', '_'), Key.Replace(' ', '_'));
            if (sValue == null)
                return Default;

            return sValue;
        }
        #endregion

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            /*info.AddValue("m_nPatternType", m_nPatternType, typeof(int));
            info.AddValue("m_fMonitorWidth", m_fMonitorWidth, typeof(float));
            info.AddValue("m_fMonitorHeight", m_fMonitorHeight, typeof(float));
            info.AddValue("m_nReportType", m_nReportType, typeof(int));*/
        }
    }

    /// <summary>
    /// A structure to store the result of test item
    /// </summary>
    public struct Result
    {
        public float fMaxErr;
        public float fAvgErr;
        public float fRMSErr;

        public List<float> ErrStatisticList;

        /// <summary>
        /// The point number of test item
        /// </summary>
        public int nPtNum;
    }

    /// <summary>
    /// Declare a result item that shows on the UI
    /// </summary>
    public class ResultItem
    {
        public string m_sTitle = "";
        public string m_sContent = "";
        public Color m_clrFont = Color.Black;

        public ResultItem(string sTitle, string sContent)
        {
            m_sTitle = sTitle;
            m_sContent = sContent;
        }
    }

    ////////////////////////////////////////////////////////////////////
    // 
    // Structure
    //      TestPattern
    //          |__ TestItems

    /// <summary>
    /// Store the test pattern data
    ///  - 3 methods to create pattern
    ///     * Create pattern from linearity parameter
    ///     * Create pattern from byte array
    ///     * Create pattern from file
    /// </summary>
    public class TestPattern : IDisposable
    {
        public delegate void OnTestComplete();

        public OnTestComplete OnTestCompleteFuncPtr = null;

        protected bool m_bCurMoveComplete = false;

        protected PatternParamBase m_cTestParam;
        public PatternParamBase TestParam
        {
            get { return m_cTestParam; }
        }

        protected frmMain.FlowStep m_cFlowStep;
        public frmMain.FlowStep FlowStep
        {
            get { return m_cFlowStep; }
        }

        protected List<TestItem> m_ItemList = new List<TestItem>();
        protected float m_fPhysicalWidth = 0.0f;
        protected float m_fPhysicalHeight = 0.0f;

        protected int m_nCurTestItemIdx = -1;
        protected TestItem m_CurTestItem = null;
        protected bool m_bTestFinish = false;
        public bool TestFinish
        {
            get { return m_bTestFinish; }
        }

        /// <summary>
        /// A bitmap that draw the information on it.
        /// </summary>
        protected Bitmap m_bmpDrawLayer = null;
        public Bitmap bmpDrawLayer
        {
            set { m_bmpDrawLayer = value; }
        }

        /// <summary>
        /// The physical width of screen
        /// </summary>
        public float PhysicalWidth
        {
            get { return m_fPhysicalWidth; }
        }

        /// <summary>
        /// The physcial height of screen
        /// </summary>
        public float PhysicalHeight
        {
            get { return m_fPhysicalHeight; }
        }

        public TestPattern(PatternParamBase cParamBase, frmMain.FlowStep cFlowStep)
        {
            m_cTestParam = cParamBase;
            m_cFlowStep = cFlowStep;

            m_fPhysicalHeight = m_cTestParam.m_fMonitorHeight;
            m_fPhysicalWidth = m_cTestParam.m_fMonitorWidth;

            //Create Pattern
            CreatePattern();
        }

        /// <summary>
        /// Create the pattern base on the Param
        /// </summary>
        public virtual void CreatePattern()
        {

        }

        /// <summary>
        /// Get the number of test items.
        /// </summary>
        /// <returns></returns>
        public int GetItemCount()
        {
            return m_ItemList.Count();
        }

        /// <summary>
        /// Get the specific item
        /// </summary>
        /// <param name="nIdx"></param>
        /// <returns></returns>
        public TestItem GetItem(int nIdx)
        {
            if (nIdx >= m_ItemList.Count)
                return null;

            return m_ItemList[nIdx];
        }

        public virtual void DrawPattern(float fmmPixelX, float fmmPIxelY)
        {
            throw new System.Exception("Non-implement virtual function : \"DrawPattern(Graphics g)\".");
        }

        public void Dispose()
        {
            DoDispose();
        }

        protected virtual void DoDispose()
        {

        }

        protected RectangleF Rect2Pixels(RectangleF rcOrg, float fmmPixelX, float fmmPixelY)
        {
            RectangleF rcTransfer = new RectangleF();

            rcTransfer.X = rcOrg.X * fmmPixelX;
            rcTransfer.Y = rcOrg.Y * fmmPixelY;
            rcTransfer.Width = rcOrg.Width * fmmPixelX;
            rcTransfer.Height = rcOrg.Height * fmmPixelY;

            return rcTransfer;
        }

        public virtual void DoTest(ElanReport CurReport, float fmmPixelX, float fmmPixelY)
        {
            if (m_bmpDrawLayer == null)
                return;

            lock (m_bmpDrawLayer)
            {
                using (Graphics g = Graphics.FromImage(m_bmpDrawLayer))
                {
                    if (m_CurTestItem == null)
                    {
                        m_CurTestItem = GetItem(0);
                        m_nCurTestItemIdx = 0;
                    }
                    else if (m_CurTestItem.TestDone == true && m_bCurMoveComplete == true)
                    {
                        #region Check the result and get the next test item.

                        //m_CurTestItem.CheckResult(g, fmmPixelX, fmmPixelY);

                        m_nCurTestItemIdx++;
                        if (m_nCurTestItemIdx >= GetItemCount())
                        {
                            m_bTestFinish = true;
                            return;
                        }
                        #endregion
                    }

                    m_CurTestItem.DoTest(g, CurReport, fmmPixelX, fmmPixelY);
                }
            }
        }

        public ResultItem[] GetResultItems()
        {
            foreach (TestItem CurItem in m_ItemList)
                return CurItem.GetResultItems();

            return null;
        }
    }

    /// <summary>
    /// The basic unit of test pattern.
    /// </summary>
    public class TestItem
    {
        public delegate void CallbackFunc(int nMessage);

        public CallbackFunc m_CallbackFunc = null;

        protected Result m_Result;

        /// <summary>
        /// 是否測試結束的flag
        /// </summary>
        protected bool m_bTestDone = false;
        public bool TestDone
        {
            get { return m_bTestDone; }
        }

        protected bool m_bPenOn = false;
        public bool PenOn
        {
            get { return m_bPenOn; }
        }

        #region Test Result

        public float MaxErr
        {
            get { return m_Result.fMaxErr; }
        }

        public float AvgErr
        {
            get { return m_Result.fAvgErr; }
        }

        public float RMSErr
        {
            get { return m_Result.fMaxErr; }
        }

        #endregion

        /// <summary>
        /// Record the tick that the item start testing.
        /// </summary>
        public long m_nStartTick;

        public TestItem()
        {
        }

        /// <summary>
        /// Draw the test item.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clrItem"></param>
        /// <param name="fmmPixelX"></param>
        /// <param name="fmmPixelY"></param>
        public virtual void DrawItem(Graphics g, Color clrItem, float fmmPixelX, float fmmPixelY)
        {

        }

        /// <summary>
        /// Input report and do test.
        /// </summary>
        /// <param name="g">The graphics that use to draw something on screen</param>
        /// <param name="CurReport">The report that current input.</param>
        /// <param name="nReportType">The report type that use to do test</param>
        /// <param name="fmmPixelX">The pixels number of 1mm.(Width)</param>
        /// <param name="fmmPixelY">The pixels number of 1mm.(Height)</param>
        /// <returns></returns>
        public virtual bool DoTest(Graphics g, ElanReport CurReport, float fmmPixelX, float fmmPixelY)
        {
            return false;
        }

        /// <summary>
        /// Check the cruurent report type is matched the current setting.
        /// </summary>
        /// <param name="CurReport">The current report</param>
        /// <param name="nReportType">The report type that current setting</param>
        /// <returns></returns>
        protected bool IsMatchReportType(ElanReport CurReport, int nReportType)
        {
            if (nReportType == 0) //只處理Finger
            {
                if (CurReport.reportType == ElanReport.ReportType.ActivePen)
                    return false;
                else if (CurReport.Mode != ElanReport.ReportMode.Down)
                    return false;
            }
            else if (nReportType == 1) //只處理Active Pen Contact
            {
                if (CurReport.reportType == ElanReport.ReportType.Finger)
                    return false;
                else if (CurReport.Mode != ElanReport.ReportMode.Contact)
                    return false;
            }
            else if (nReportType == 2) //只處理Active Pen Hover
            {
                if (CurReport.reportType == ElanReport.ReportType.Finger)
                    return false;
                else if (CurReport.Mode != ElanReport.ReportMode.Hover)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Check all the distance and  the result
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public virtual void CheckResult(Graphics g, float fmmPixelX, float fmmPixelY)
        {
        }

        public virtual ResultItem[] GetResultItems()
        {
            return null;
        }

        /// <summary>
        /// Convert the test result to byte array
        /// </summary>
        /// <returns></returns>
        public byte[] ResultToBytes()
        {
            return GetResultBytes();
        }

        /// <summary>
        /// Set the test result form byte array
        /// </summary>
        /// <param name="arrByteData"></param>
        public void BytesToResult(byte[] arrByteData)
        {
            m_Result = ResultFromBytes(arrByteData);
        }

        /// <summary>
        /// Convert struct to byte array
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        protected byte[] GetResultBytes()
        {
            int size = Marshal.SizeOf(m_Result);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(m_Result, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// Convert the byte array to string
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        protected Result ResultFromBytes(byte[] arr)
        {
            Result result = new Result();

            int size = Marshal.SizeOf(result);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            result = (Result)Marshal.PtrToStructure(ptr, result.GetType());
            Marshal.FreeHGlobal(ptr);

            return result;
        }

        /// <summary>
        /// Initialize all the variable in the test result object.
        /// </summary>
        protected void InitializeResult()
        {
            m_Result = new Result();
            m_Result.fMaxErr = 0.0f;
            m_Result.fAvgErr = 0.0f;
            m_Result.fRMSErr = 0.0f;
            //建立一個List,共有四個elements,分別統計> 0.5mm, > 0.3mm, > 0.25mm > 0.2mm與 < 0.2mm的點數
            m_Result.ErrStatisticList = new List<float>() { 0, 0, 0, 0, 0 };
            m_Result.nPtNum = 0;

        }

        /// <summary>
        /// Do the error rate statistic
        /// </summary>
        /// <param name="fDistance"></param>
        public void DoStatistic(float fDistance)
        {
            if (fDistance > 0.5f)
                m_Result.ErrStatisticList[0]++;

            if (fDistance > 0.3f)
                m_Result.ErrStatisticList[1]++;

            if (fDistance > 0.25f)
                m_Result.ErrStatisticList[2]++;

            if (fDistance > 0.2f)
                m_Result.ErrStatisticList[3]++;
            else
                m_Result.ErrStatisticList[4]++;
        }

        protected PointF Pt2Pixels(PointF ptOrg, float fmmPixelX, float fmmPixelY)
        {
            PointF ptTransfer = new PointF();
            ptTransfer.X = ptOrg.X * fmmPixelX;
            ptTransfer.Y = ptOrg.Y * fmmPixelY;

            return ptTransfer;
        }
    }
}
