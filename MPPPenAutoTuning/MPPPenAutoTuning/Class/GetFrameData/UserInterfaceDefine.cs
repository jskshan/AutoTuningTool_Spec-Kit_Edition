using System;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Elan
{
    public static class ExtensionMethods
    {
        public static void DoubleBuffered(Control dgvControl, bool bSettingFlag)
        {
            Type dgvType = dgvControl.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgvControl, bSettingFlag, null);
        }
    }

    public class UserInterfaceDefine
    {
        public enum TabPagesGroup
        {
            Page_Connect = 0,
            Page_AllpoinTest,
            Page_TraceTest,
            Page_ParameterTuning,
            Page_Linearity,
            Page_ReportRecorder,
            Page_DFTView,
            Page_Tools,
            Page_Misc
        }

        public enum SettingPage
        {
            Setting_Connect = 0,
            Setting_AllpointTest,
            Setting_TraceTest,
            Setting_DFTView,
            Setting_Misc
        }

        public enum InterfaceType
        {
            IF_HID_OVER_I2C_TP = 0,
            IF_SPI_MA_RISING_HALF_CYCLE = 1,
            IF_SPI_MA_FALLING_HALF_CYCLE = 2,
            IF_SPI_MA_RISING = 3,
            IF_SPI_MA_FALLING = 4,
            IF_TDDI_SPI_MA_RISING_HALF_CYCLE = 5,
            IF_TDDI_SPI_MA_FALLING_HALF_CYCLE = 6,
            IF_TDDI_SPI_MA_RISING = 7,
            IF_TDDI_SPI_MA_FALLING = 8
            /*
            IF_USB = 0,
            IF_HID_OVER_I2C_TP = 1,
            IF_HID_OVER_I2C_PoTP = 2,
            IF_I2C = 3,
            IF_I2C_TDDI = 4,
            IF_SPI_MA_RISING_HALF_CYCLE = 5,
            IF_SPI_MA_FALLING_HALF_CYCLE = 6,
            IF_SPI_MA_RISING = 7,
            IF_SPI_MA_FALLING = 8,
            IF_SPI_PRECISE = 9,
            IF_TDDI_SPI_MA_RISING_HALF_CYCLE = 10,
            IF_TDDI_SPI_MA_FALLING_HALF_CYCLE = 11,
            IF_TDDI_SPI_MA_RISING = 12,
            IF_TDDI_SPI_MA_FALLING = 13
            */
        }

        public enum RawDataType
        {
            Type_dV = 0, // 0x04
            Type_ADC,    // 0x03
            Type_Base,   // 0x02   
            Type_Noise,  // 0x05
            Type_Combo,
            Type_Offset, // 0x00   
            Type_MMTPGA, // 0x07
            Type_Reserve_1
        }

        public const int Offset_RawData_Index = 0x00;
        public const int MMTPGA_RawData_Index = 0x07;

        public enum RawDataSaveType
        { 
            ResultRawData = 1,
            AllFramesData
        }

        public enum PowerVoltage
        {
            Vol_50 = 0,
            Vol_33,
            Vol_30,
            Vol_28,
            Vol_18
        }

        public enum ColorDef
        {
            Color_Black = 0,
            Color_Red,
            Color_LightGreen,
            Color_Yellow,
            Color_Blue,
            Color_Cyan,
            Color_Magenta,
            Color_White
        }

        //J2++
        /// <summary>
        /// The compensated color array
        /// </summary>
        public static Color[] COMPENSATED_COLOR_ARRAY = 
        { 
            Color.Black, 
            Color.Red,
            Color.LightGreen,
            Color.Yellow,
            Color.Blue,
            Color.Cyan,
            Color.Magenta,
            Color.White
        };

        /// <summary>
        /// Convert the index to data type
        /// </summary>
        /// <param name="nTypeIndex"></param>
        /// <returns></returns>
        public static int Index2DateType(int nTypeIndex)
        {
            if (nTypeIndex == (int)RawDataType.Type_dV) //DV
                return ElanDefine.RAWDATA_dV;
            else if (nTypeIndex == (int)RawDataType.Type_ADC) //ADC
                return ElanDefine.RAWDATA_ADC;
            else if (nTypeIndex == (int)RawDataType.Type_Base) //Base
                return ElanDefine.RAWDATA_BASE;
            else if (nTypeIndex == (int)RawDataType.Type_Noise) //Noise
                return ElanDefine.RAWDATA_NOISE;
            else if (nTypeIndex == (int)RawDataType.Type_Offset) //Offset
                return ElanDefine.RAWDATA_OFFSET;
            else if (nTypeIndex == (int)RawDataType.Type_MMTPGA) //MMTPGA
                return ElanDefine.RAWDATA_MMTPGA;

            return 0;
        }

        /// <summary>
        /// Convert the data type to index
        /// </summary>
        /// <param name="nDataType"></param>
        /// <returns></returns>
        public static int DataType2Index(int nDataType)
        {
            if (nDataType == ElanDefine.RAWDATA_dV) //DV
                return (int)RawDataType.Type_dV;
            else if (nDataType == ElanDefine.RAWDATA_ADC) //ADC
                return (int)RawDataType.Type_ADC;
            else if (nDataType == ElanDefine.RAWDATA_BASE) //Base
                return (int)RawDataType.Type_Base;
            else if (nDataType == ElanDefine.RAWDATA_NOISE) //Noise
                return (int)RawDataType.Type_Noise;
            else if (nDataType == ElanDefine.RAWDATA_OFFSET) //Offset
                return (int)RawDataType.Type_Offset;
            else if (nDataType == ElanDefine.RAWDATA_MMTPGA) //MMTPGA
                return (int)RawDataType.Type_MMTPGA;

            return 0;
        }
        //J2--

        public const int DefaultBackgroundColor = 0x00faff;//Color.FromArgb(0, 250, 255)
        public const int Finger_1_Color = 0xff0000;//Red, Green, Blue 
        public const int Finger_2_Color = 0x0080ff;
        public const int Finger_3_Color = 0x008040;
        public const int Finger_4_Color = 0xff0080;
        public const int Finger_5_Color = 0xff8000;
        public const int Finger_6_Color = 0x004080;
        public const int Finger_7_Color = 0x00ff80;
        public const int Finger_8_Color = 0x808040;
        public const int Finger_9_Color = 0x400000;
        public const int Finger_10_Color = 0x0000ff;

        public enum StatType
        { 
            STAT_NONE = 0,
            STAT_MAX,
            STAT_MIN,
            STAT_MAX_SUB_MIN
        }

        public enum MirrorType
        { 
            Mirror_None = 0,
            Mirror_XDir,
            Mirror_YDir
        }

        //J2++
        public const int DEFAULT_FINGER_OSR = 64;
        public const int DEFAULT_PEN_OSR = 256;
        //J2--

        public enum OriginPosition
        { 
            Org_LeftTop = 0,
            Org_LeftBottom,
            Org_RightTop,
            Org_RightBottom
        }

        public enum PenInfoType
        {
            Pressure = 0
        }

        public enum LinearityRawDataType
        {
            RX_TYPE = 0,
            TX_TYPE
        };

        public enum LinearityPartsOfOneIndex
        {
            OPTION_64PARTS = 0,
            OPTION_256PARTS
        }

        public const int MAX_CCVPDDATA_LENGTH = 2;

        public enum DFTViewType
        { 
            DFTNoneView = 0,
            DFTGridView = 0x01,
            DFTChartView = 0x02,
            DFTFreqView = 0x04,
            DFTResampledView = 0x08
        }

        public static bool ContainDFTViewType(UserInterfaceDefine.DFTViewType eType, UserInterfaceDefine.DFTViewType eNeeded)
        {
            if ((eType & eNeeded) != 0)
                return true;

            return false;
        }

        public enum ReportModeSelect
        { 
            PEN_MODE = 0,
            FINGER_MODE,
            PEN_PTP_MODE,
            FINGER_PTP_MODE,
            MAX_MODE_COUNT
        }

        public enum FilterPenType
        { 
            FILTER_HOVER = 0,
            FILTER_TIPDOWN,
            MAX_FILTER_COUNT
        }

        public enum EktMergeRule
        { 
            REPLACE = 0,
            APPEND,
            INSERT
        }

        public const int DELAY_TIME_PER_FRAME = 10; //10ms
        public const string DEFAULT_EMPTY_DEVPATH = "NoTouch Device";

        public const int MAX_CHAR_VALUE = 26;
    }   //UserInterfaceDefine
}