using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace Elan
{
    /// <summary>
    /// A class to manage the frames.
    /// functions:
    ///  1.Store the frame
    ///  2.Support to Load or Save the frames to log
    /// </summary>
    public class FrameMgr
    {
        protected List<Frame> m_cFrame_List = new List<Frame>();
        protected int[,] m_nStatMaxRawData_Array = null;
        protected int[,] m_nStatMinRawData_Array = null;
        protected int[,] m_nStatMaxSubMinRawData_Array = null;

        /// <summary>
        /// Get the frame number.
        /// </summary>
        public int Count
        {
            get { return m_cFrame_List.Count; }
        }

        public int DataType
        {
            get
            {
                if (m_cFrame_List.Count > 0)
                    return (int)m_cFrame_List[0].FrameType;

                return -1;
            }
        }

        public FrameMgr()
        {
        }

        //J2++
        /// <summary>
        /// Get the IC Type form input file
        /// </summary>
        /// <param name="sFilePath"></param>
        /// <returns></returns>
        public int GetICType(string sFilePath)
        {
            if (File.Exists(sFilePath) == false)
                return 0;

            int nICType = 0;

            using (FileStream fs = new FileStream(sFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                StreamReader sr = new StreamReader(fs);
                string sLine = "", sTemp;
                string[] sPart_Array;

                //Read Info at 1st title
                if ((sLine = sr.ReadLine()) != null)
                {
                    sTemp = sLine.Substring(sLine.IndexOf("]") + 1);
                    sPart_Array = sTemp.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    int[] nInfo_Array = new int[sPart_Array.Length];

                    for (int i = 0; i < sPart_Array.Length; i++)
                        nInfo_Array[i] = Convert.ToInt32(sPart_Array[i], 10);

                    nICType = nInfo_Array[10];
                }

                sr.Close();
            }

            return nICType;
        }
        //J2--

        /// <summary>
        /// Save all the frames in frame manger into file.
        /// </summary>
        /// <param name="sFileName"></param>
        /// <param name="structTraceInfo"></param>
        /// <param name="nDataType"></param>
        /// <param name="nICType"></param>
        /// <param name="nColorLow"></param>
        /// <param name="nColorLevel"></param>
        /// <param name="eTraceMode"></param>
        /// <param name="nStartIndex">The start index of frame</param>
        /// <param name="nInterval">The frames number that needs to skips.</param>
        /// <returns></returns>
        public bool Save(string sFileName,
                         ElanTouch.TraceInfo structTraceInfo,
                         int nDataType,
                         int nICType,
                         int nColorLow,
                         int nColorLevel,
                         ElanTouch.TraceMode eTraceMode,
                         int nStartIndex = 0,
                         int nInterval = 1)
        {
            using (FileStream fs = new FileStream(sFileName, FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                StringBuilder sb = new StringBuilder();
                Frame cFrame = null;
                int nFrameIndex = 0;

                //Save the header information to file.
                SaveHeader(ref sw, structTraceInfo, nDataType, nICType, nColorLow, nColorLevel, eTraceMode);

                nFrameIndex = nStartIndex;

                do
                {
                    try
                    {
                        cFrame = GetFrame(nFrameIndex);
                        cFrame.Save(sw, (int)(nFrameIndex / nInterval));
                        nFrameIndex += nInterval;
                    }
                    catch
                    {
                        return false;
                    }
                } while (nFrameIndex < Count);

                sw.Flush();
                sw.Close();
            }

            return true;
        }

        /// <summary>
        /// Save the header information to log file.
        /// </summary>
        /// <param name="sw">The object to do stream writer.</param>
        /// <param name="structTraceInfo">Use to write the trace information in header.</param>
        /// <param name="nDataType">The type of test mode data(DV, Base, ADC,...)</param>
        /// <param name="nICType">IC Type(53xx, 62xx,...)</param>
        /// <param name="nColorLow">The start value of color</param>
        /// <param name="nColorLevel">The inverval of color</param>
        /// <param name="traceMode">Include the data of frame.</param>
        private void SaveHeader(ref StreamWriter sw,
                                ElanTouch.TraceInfo structTraceInfo,
                                int nDataType,
                                int nICType,
                                int nColorLow,
                                int nColorLevel,
                                ElanTouch.TraceMode eTraceMode)
        {
            StringBuilder sb = new StringBuilder();
            int nRXTraceNumber = structTraceInfo.GetRXTraceNum(eTraceMode);
            int nTXTraceNumber = structTraceInfo.GetTXTraceNum(eTraceMode);

            //Write Info at 1st title
            sb.Append("[Info]");
            //ChipNum
            sb.Append(structTraceInfo.nChipNum.ToString("d"));
            sb.Append(",");
            //TotalXTrace
            sb.Append(nRXTraceNumber.ToString("d"));
            sb.Append(",");
            //TotalYTrace
            sb.Append(nTXTraceNumber.ToString("d"));
            sb.Append(",");
            //ColorLow 
            sb.Append(nColorLow.ToString("d"));
            sb.Append(",");
            //Level
            sb.Append(nColorLevel.ToString("d"));
            sb.Append(",");
            //TestMode
            sb.Append(nDataType.ToString("d"));
            sb.Append(",");
            //XTrace[Master]
            sb.Append(structTraceInfo.XAxis[(int)ElanDefine.ChipType.MASTER_CHIP].ToString("d"));
            sb.Append(",");
            //XTrace[Slave1]
            sb.Append(structTraceInfo.XAxis[(int)ElanDefine.ChipType.SLAVE1_CHIP].ToString("d"));
            sb.Append(",");
            //XTrace[Slave2]
            sb.Append(structTraceInfo.XAxis[(int)ElanDefine.ChipType.SLAVE2_CHIP].ToString("d"));
            sb.Append(",");
            //PartialNum
            sb.Append(structTraceInfo.nPartialNum.ToString("d"));
            sb.Append(",");
            //IC_Type
            sb.Append(nICType.ToString("d"));
            sb.Append(",");
            //Others
            sb.Append("0,0,0,0,");
            //Writes a string followed by a line terminator to the text stream
            sw.WriteLine(sb.ToString());
        }

        public bool SaveFrames(string sFilePath, int nStartIndex = 0, int nInterval = 1)
        {
            using (FileStream fs = new FileStream(sFilePath, FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                StringBuilder sb = new StringBuilder();
                Frame cFrame = null;
                int nFrameIndex = 0;

                nFrameIndex = nStartIndex;

                do
                {
                    try
                    {
                        cFrame = GetFrame(nFrameIndex);
                        cFrame.SaveFrame(sw, (int)(nFrameIndex / nInterval));
                        nFrameIndex += nInterval;
                    }
                    catch
                    {
                        return false;
                    }
                } while (nFrameIndex < Count);

                sw.Flush();
                sw.Close();
            }

            return true;
        }

        public void Add(Frame cNewFrame, int nStatisticType = (int)UserInterfaceDefine.StatType.STAT_NONE)
        {
            if ((int)UserInterfaceDefine.StatType.STAT_NONE != nStatisticType)
                StatisticFrame(cNewFrame, nStatisticType);

            m_cFrame_List.Add(cNewFrame);
        }

        public Frame GetFrame(int nIndex)
        {
            if (nIndex >= Count || nIndex < 0)
                return null;

            return m_cFrame_List[nIndex];
        }

        public int[,] GetStatisticFrame(int nStatisticIndex)
        {
            if ((null == m_nStatMaxRawData_Array) || (null == m_nStatMinRawData_Array) || (null == m_nStatMaxSubMinRawData_Array))
                return null;

            if ((int)UserInterfaceDefine.StatType.STAT_MAX == nStatisticIndex)
                return m_nStatMaxRawData_Array;
            else if ((int)UserInterfaceDefine.StatType.STAT_MIN == nStatisticIndex)
                return m_nStatMinRawData_Array;
            else if ((int)UserInterfaceDefine.StatType.STAT_MAX_SUB_MIN == nStatisticIndex)
                return m_nStatMaxSubMinRawData_Array;

            return null;    //UserInterfaceDefine.StatType.STAT_NONE
        }

        public int[,,] GetFrames()
        {
            if (m_cFrame_List.Count <= 0)
                return null;

            int[,,] nFrame_Array = new int[m_cFrame_List.Count, m_cFrame_List[0].YLength, m_cFrame_List[0].XLength];

            for (int nFrameIndex = 0; nFrameIndex < m_cFrame_List.Count; nFrameIndex++)
            {
                int[,] nCurrentFrame_Array = m_cFrame_List[nFrameIndex].GetFrame();

                for (int nYIndex = 0; nYIndex < m_cFrame_List[0].YLength; nYIndex++)
                {
                    for (int nXIndex = 0; nXIndex < m_cFrame_List[0].XLength; nXIndex++)
                    {
                        nFrame_Array[nFrameIndex, nYIndex, nXIndex] = nCurrentFrame_Array[nYIndex, nXIndex];
                    }
                }
            }

            return nFrame_Array;
        }

        /// <summary>
        /// Clear all the frames store in the frame manager
        /// </summary>
        public void Clear()
        {
            m_cFrame_List.Clear();
            m_nStatMaxRawData_Array = m_nStatMinRawData_Array = m_nStatMaxSubMinRawData_Array = null;
        }

        private void InitialStatisticFrame(int nXLength, int nYLength)
        {
            for (int nY = 0; nY < nYLength; nY++)
            {
                for (int nX = 0; nX < nXLength; nX++)
                {
                    m_nStatMaxRawData_Array[nY, nX] = Int32.MinValue;
                    m_nStatMinRawData_Array[nY, nX] = Int32.MaxValue;
                    m_nStatMaxSubMinRawData_Array[nY, nX] = 0;
                }
            }
        }

        private void StatisticFrame(Frame cNewFrame, int nStatisticType)
        {
            int[,] nCurrentRawData_Array = cNewFrame.GetFrame();

            if (m_cFrame_List.Count == 0)
            {
                m_nStatMaxRawData_Array = new int[cNewFrame.YLength, cNewFrame.XLength];
                m_nStatMinRawData_Array = new int[cNewFrame.YLength, cNewFrame.XLength];
                m_nStatMaxSubMinRawData_Array = new int[cNewFrame.YLength, cNewFrame.XLength];
                InitialStatisticFrame(cNewFrame.XLength, cNewFrame.YLength);
            }

            if ((null == m_nStatMaxRawData_Array) || (null == m_nStatMinRawData_Array) || (null == m_nStatMaxSubMinRawData_Array))
                return;

            if ((int)UserInterfaceDefine.StatType.STAT_MAX == nStatisticType ||
                (int)UserInterfaceDefine.StatType.STAT_MAX_SUB_MIN == nStatisticType)
            {
                for (int nY = 0; nY < cNewFrame.YLength; nY++)
                {
                    for (int nX = 0; nX < cNewFrame.XLength; nX++)
                    {
                        if (nCurrentRawData_Array[nY, nX] >= m_nStatMaxRawData_Array[nY, nX])
                            m_nStatMaxRawData_Array[nY, nX] = nCurrentRawData_Array[nY, nX];
                    }
                }
            }   //STAT_MAX

            if ((int)UserInterfaceDefine.StatType.STAT_MIN == nStatisticType ||
                (int)UserInterfaceDefine.StatType.STAT_MAX_SUB_MIN == nStatisticType)
            {
                for (int nY = 0; nY < cNewFrame.YLength; nY++)
                {
                    for (int nX = 0; nX < cNewFrame.XLength; nX++)
                    {
                        if (nCurrentRawData_Array[nY, nX] <= m_nStatMinRawData_Array[nY, nX])
                            m_nStatMinRawData_Array[nY, nX] = nCurrentRawData_Array[nY, nX];
                    }
                }
            }   //STAT_MIN

            if ((int)UserInterfaceDefine.StatType.STAT_MAX_SUB_MIN == nStatisticType)
            {
                for (int nY = 0; nY < cNewFrame.YLength; nY++)
                {
                    for (int nX = 0; nX < cNewFrame.XLength; nX++)
                    {
                        m_nStatMaxSubMinRawData_Array[nY, nX] = (m_nStatMaxRawData_Array[nY, nX] - m_nStatMinRawData_Array[nY, nX]);
                    }
                }
            }   //if ((int)UserInterfaceDefine.StatType.STAT_MAX_SUB_MIN == nStatisticType)
        }
    }

    /// <summary>
    /// A basic class to store the data
    /// </summary>
    public class Frame
    {
        int[,] m_nFrameData_Array = null;

        UserInterfaceDefine.RawDataType m_nType;
        protected int m_nX;
        protected int m_nY;
        protected bool m_bSelfData = false;

        public int XLength
        {
            get { return m_nX; }
        }

        public int YLength
        {
            get { return m_nY; }
        }

        /// <summary>
        /// Record the time stamp when get frame (ms)
        /// </summary>
        protected long m_nTimeStamp;

        public long TimeStamp
        {
            get { return m_nTimeStamp; }
        }

        public Frame()
        {
            m_nX = m_nY = 0;
            m_nTimeStamp = 0;
        }

        public Frame(int[,] nFrameData_Array, int nX, int nY, bool bSelfData, UserInterfaceDefine.RawDataType nType, long nTimeStamp)
        {
            m_nFrameData_Array = nFrameData_Array;
            m_nType = nType;
            m_nX = nX;
            m_nY = nY;
            m_bSelfData = bSelfData;
            m_nTimeStamp = nTimeStamp;
        }

        /// <summary>
        /// Constructor
        /// Establish the frame data from string.
        /// </summary>
        /// <param name="sFrameData"></param>
        /// <param name="nX"></param>
        /// <param name="nY"></param>
        /// <param name="nType"></param>
        public Frame(string sData, int nXLen, int nYLen, bool bSelfData, UserInterfaceDefine.RawDataType nType)
        {
            m_nType = nType;
            m_bSelfData = bSelfData;
            m_nX = nXLen;
            m_nY = nYLen;
            int nStartIdx = 2;
            string[] sToken_Array = sData.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //Allocate the memory to stroe the frame data
            m_nFrameData_Array = new int[nYLen, nXLen];
            Array.Clear(m_nFrameData_Array, 0, m_nFrameData_Array.Length);

            //Get the timestamp
            long.TryParse(sToken_Array[1], out m_nTimeStamp);

            //Check the Self flag
            if (m_bSelfData == true)
            {
                nXLen--;
                nYLen--;
            }

            //Parse Mutual Data
            for (int nY = 0; nY < nYLen; nY++)
            {
                for (int nX = 0; nX < nXLen; nX++)
                {
                    byte bLow = 0;
                    byte bHigh = 0;
                    byte.TryParse(sToken_Array[nStartIdx], out bLow);
                    byte.TryParse(sToken_Array[nStartIdx + 1], out bHigh);

                    m_nFrameData_Array[nY, nX] = (short)((bHigh << 8) | bLow);

                    nStartIdx += 2;
                }
            }

            while (sToken_Array[nStartIdx] != "-32768" && sToken_Array[nStartIdx] != "-32767")
                nStartIdx++;

            nStartIdx++;

            //Parse RX Self Data
            if (m_bSelfData == false)
                return;

            for (int nX = 0; nX < nXLen; nX++)
            {
                byte bLow = 0;
                byte bHigh = 0;
                byte.TryParse(sToken_Array[nStartIdx], out bLow);
                byte.TryParse(sToken_Array[nStartIdx + 1], out bHigh);

                m_nFrameData_Array[nYLen, nX] = (short)((bHigh << 8) | bLow);

                nStartIdx += 2;
            }

            //Parse TX Self Data
            for (int nY = 0; nY < nYLen; nY++)
            {
                byte bLow = 0;
                byte bHigh = 0;
                byte.TryParse(sToken_Array[nStartIdx], out bLow);
                byte.TryParse(sToken_Array[nStartIdx + 1], out bHigh);

                m_nFrameData_Array[nY, nXLen] = (short)((bHigh << 8) | bLow);

                nStartIdx += 2;
            }
        }

        public int[,] GetFrame()
        {
            return m_nFrameData_Array;
        }

        public UserInterfaceDefine.RawDataType FrameType
        {
            get { return m_nType; }
        }

        /// <summary>
        /// Save the data to stream
        /// </summary>
        /// <param name="sw"></param>
        public void Save(StreamWriter sw, int nIndex)
        {
            StringBuilder sb = new StringBuilder();

            //Index
            sb.Append(nIndex.ToString("d"));
            sb.Append(",");

            //TimeStamp
            sb.Append(m_nTimeStamp.ToString(""));
            sb.Append(",");

            int nXLength = m_nX;
            int nYLength = m_nY;

            if (m_bSelfData == true)
            {
                nXLength--;
                nYLength--;
            }

            //Save the mutual data
            for (int nY = 0; nY < nYLength; nY++)
            {
                for (int nX = 0; nX < nXLength; nX++)
                {
                    sb.Append(String.Format("{0:d}", m_nFrameData_Array[nY, nX] & 0xff));
                    sb.Append(",");

                    sb.Append(String.Format("{0:d}", (m_nFrameData_Array[nY, nX] & 0xff00) >> 8));
                    sb.Append(",");
                }
            }

            sb.Append("-32768,");

            if (m_bSelfData == true)
            {
                //Save RX Self Data
                for (int nX = 0; nX < nXLength; nX++)
                {
                    sb.Append(String.Format("{0:d}", m_nFrameData_Array[nYLength, nX] & 0xff));
                    sb.Append(",");

                    sb.Append(String.Format("{0:d}", (m_nFrameData_Array[nYLength, nX] & 0xff00) >> 8));
                    sb.Append(",");
                }

                //Save TX Self Data
                for (int nY = 0; nY < nYLength; nY++)
                {
                    sb.Append(String.Format("{0:d}", m_nFrameData_Array[nY, nXLength] & 0xff));
                    sb.Append(",");

                    sb.Append(String.Format("{0:d}", (m_nFrameData_Array[nY, nXLength] & 0xff00) >> 8));
                    sb.Append(",");
                }

                sb.Append("-32767");
            }

            sw.WriteLine(sb.ToString());
            sw.Flush();
        }   //Save

        public void SaveFrame(StreamWriter sw, int nIndex)
        {
            StringBuilder sb = new StringBuilder();

            //Index
            sb.Append("Frame,");
            sb.Append(nIndex.ToString("d"));
            sw.WriteLine(sb.ToString());
            sw.Flush();

            int nXLength = m_nX;
            int nYLength = m_nY;

            if (m_bSelfData == true)
            {
                nXLength--;
                nYLength--;
            }

            //Save the mutual data
            for (int nY = 0; nY < nYLength; nY++)
            {
                sb.Clear();

                for (int nX = 0; nX < nXLength; nX++)
                {
                    sb.Append(String.Format("{0:d}", m_nFrameData_Array[nY, nX]));

                    if ((nXLength - 1) != nX)
                        sb.Append(",");
                }

                sw.WriteLine(sb.ToString());
                sw.Flush();
            }

            sb.Clear();
            sw.WriteLine("");
            sw.Flush();
        }   //SaveFrame
    }
}
