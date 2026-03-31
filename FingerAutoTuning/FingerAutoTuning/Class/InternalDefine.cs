using System;
using System.Drawing;

namespace Elan
{
    public class InternalDefine
    {
        public enum RawDataFileInfoField
        {
            ChipNum = 0,
            RX_TraceNum, // X-axis
            TX_TraceNum, // Y-axis
            ColorLow,
            ColorLevel,
            DataTypeIndex,
            RX_TraceMaster,
            RX_TraceSlave1,
            RX_TraceSlave2,
            PartialNum,
            IC_Type
        }

        public struct ELAN_AVG_INFO
        {
            public byte byThisIndex;
            public long dwTotalCount;
            public int[] wTraceNum;
            public long[] dwTraceValue; //must use signed long
        };

        public enum EdidInfoField
        {
            Manufacturer = 7, // 2 bytes
            Product = 9, // 2 bytes
            SN = 11, // 4 bytes
            Date = 15, // 2 bytes
            Version = 17, // 1 byte
            Reversion = 18 // 1 byte
        }

        public const int PEN_PRESSURE_UNTIP = -1;

        public struct ELAN_SELECTED_CELLRANGE
        {
            public int nFrameIndex;
            public Point LeftTop;
            public Point RightBottom;
        }
    }
}
