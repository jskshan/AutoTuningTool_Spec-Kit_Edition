using System;
using System.Drawing;

namespace Elan
{
    public class ElanDefine
    {
        public const int UNIT_1K = 1000;
        public const int SIZE_1K = 1024;
        public const int SIZE_1M = (SIZE_1K * SIZE_1K);
        public const int SIZE_1PAGE = 132;
        public const int SIZE_PAGE_DATA_LENGTH = 128;//byte 2 ~ 128 : remove address:2 bytes & checksum:2 bytes
        public const int SIZE_1PAGE_WORD = 64;

        public const int TIME_1MS = 1;
        public const int TIME_1SEC = (TIME_1MS * UNIT_1K);
        public const int TIME_4SEC = (TIME_1MS * UNIT_1K * 4);
        public const int TIME_100MS = (TIME_1MS * 100);

        public const int FILE_SIZE_LIMIT = 2 * SIZE_1M;

        public const int BRIDGE_CMD = 0x18;
        public const int ELAN_VID = 0x4f3;
        public const int DEFAULT_SOCKET_PORT = 9344;
        public const int HIDoverI2C_PID = 0x07;
        public const int Recovery_PID = 0x0b;
        public const int DEFAULT_MASTER_ADDRESS = 0x20;
        public const int DEFAULT_NORMAL_LENGTH = 0x3f;
        //J2++
        public const int DEFAULT_PARTIAL_NUM = 2;
        //J2--

        public const int DESKTOP_USAGE_PAGE = 0x0D;
        //J2++
        public const int I2C_USAGE_PAGE = 0x0C;
        //J2--
        public const int VENDOR_USAGE_PAGE = 0xFF00;

        public const int DESKTOP_USAGE = 0x00;
        public const int VENDOR_USAGE = 0x00;

        public const int RAWDATA_NORMAL_COUNT = 0;
        /// <summary>
        /// Set it to get the self data when the ic type is 6315
        /// </summary>
        public const int RAWDATA_COMBO_SELF_COUNT = 2;
        public const int RAWDATA_COMBO_COUNT = 3;

        public const int MAX_USB_PACKAGE_LENGTH = 116;
        public const int MAX_I2C_PACKAGE_LENGTH = 65;
        public const int MAX_LINEARITY_PACKAGE_LENGTH = 32;
        public const int MAX_SOCKET_DATA_LENGTH = 65;

        public const int FINGER_REPORT_ID = 0x01;
        public const int PEN_REPORT_ID = 0x07;
        public const int I2C_REPORT_HEADER = 0x00;
        public const int I2C_FINGER_REPORT_ID = 0x62;
        public const int I2C_FINGER_BUFFER_REPORT_ID = 0x63;
        public const int LINEAR_REPORT_ID = 0x17;
        public const int ITOUCH_REPORT_ID = 0x40;
        public const int PTP_REPORT_ID = 0x04;
        public const int FINGER_ANDROID_REPORT_ID = 0x40;
#if(_USE_HUAWEI)
        //Modified by J2
        public const int FINGER_HUAWEI_REPORT_ID = 0x1A;
#endif
        public const int PEN_ANDROID_REPORT_ID = 0x0E;
        public const int FINGER_I2C_REPORT_ID = 0x62;

        // Alan 20170221
        public const int LASTPAGE_ADDR_63XX = 0xFFC0;
        public const int LASTPAGE_ADDR_52XX = 0x7FC0;
        public const int LASTPAGE_ADDR_USB = 0x5FC0;
        public const int LASTPAGE_ADDR_I2C = 0x3FC0;
        public const int LASTPAGE_EXT_ADDR_TDDI = 0x5FC0;//TDDI
        public const int FIRSTPAGE_EXT_ADDRESS_TDDI = 0x5800;//TDDI
        //~Alan 20170221

        public const int Gen39P_ICType = 391580;//P ASCII is 80
        public const int Gen5M_ICType = 5015;
        public const int Gen52_ICType = 5200;
        public const int Gen53_ICType = 5312;
        public const int Gen63_ICType = 6315;
        public const int Gen6308_ICType = 6308;
        public const int Gen7315_ICType = 7315;
        public const int Gen7315_2chips_ICType = 73152;
        public const int Gen7318_ICType = 7318;
        public const int Gen66_ICType = 6600;//TDDI
        public const int Gen672_ICType = 6720;//TDDI
        public const int Gen673_ICType = 6730;//TDDI
        public const int Gen675_ICType = 6750;//TDDI
        public const int Gen7318_2chips_ICType = 471378;
        public const int Gen902_ICType = 36617;

        public const int Gen63Remark5515_ICType = 5515;
        public const int BCLOVER_REMARK = 0x60;

        public const int VOL_50V = 50;
        public const int VOL_33V = 33;
        public const int VOL_30V = 30;
        public const int VOL_28V = 28;
        public const int VOL_18V = 18;

        // USB or FastUSB:1 
        // I2C or FastI2C:2 
        // SPI:3~6 
        // SPI MA(TP is SL Hi Rising Edge Out) - Half Cycle:3
        // SPI MA(TP is SL Hi Falling Edge Out)- Half Cycle:4
        // SPI MA(TP is SL Hi Rising Edge Out):5
        // SPI MA(TP is SL Hi Falling Edge Out):6
        // Hid over I2C: 8 
        // SPI for iTouch: 9
        // I2C for TDDI: 11
        // TDDI SPI MA(TP is SL Hi Rising Edge Out) - Half Cycle: 12
        // TDDI SPI MA(TP is SL Hi Falling Edge Out)- Half Cycle: 13
        // TDDI SPI MA(TP is SL Hi Rising Edge Out): 14
        // TDDI SPI MA(TP is SL Hi Falling Edge Out): 15
        public const int INTERFACE_USB = 1;
        public const int INTERFACE_I2C = 2;
        public const int INTERFACE_SPI_MA_RISING_HALF_CYCLE = 3;
        public const int INTERFACE_SPI_MA_FALLING_HALF_CYCLE = 4;
        public const int INTERFACE_SPI_MA_RISING = 5;
        public const int INTERFACE_SPI_MA_FALLING = 6;
        public const int INTERFACE_HIDoverI2C = 8;
        public const int INTERFACE_ITOUCH = 9;
        public const int INTERFACE_TDDI_I2C = 11;
        public const int INTERFACE_TDDI_SPI_MA_RISING_HALF_CYCLE = 12;
        public const int INTERFACE_TDDI_SPI_MA_FALLING_HALF_CYCLE = 13;
        public const int INTERFACE_TDDI_SPI_MA_RISING = 14;
        public const int INTERFACE_TDDI_SPI_MA_FALLING = 15;

        public const int RAWDATA_OFFSET = 0;
        public const int RAWDATA_BASE = 2;
        public const int RAWDATA_ADC = 3;
        public const int RAWDATA_dV = 4;
        public const int RAWDATA_NOISE = 5;
        public const int RAWDATA_MMTPGA = 7;

        public enum ChipType{
	        MASTER_CHIP = 0,
	        SLAVE1_CHIP,
	        SLAVE2_CHIP,
	        SLAVE3_CHIP,
	        MAX_CHIP_COUNT
        };

        public const int MAX_PENDATA_LENGTH_V1 = 8;
        public const int MAX_PENDATA_LENGTH_V2 = 10;
        public const int MAX_PENDATA_LENGTH_V3 = 21;

        public enum PenFeatureBitField {
            InRange = 0,
            TipSwitch,
            BarrelSwitch,
            Invert,
            Eraser,
            Reserved,
            Pressure_LSB,
            Pressure_HSB
        }

        public const int PEN_MOVE_ID = 0x00;
        public const int PEN_INRNAGE_ID = 0x01;
        public const int PEN_INFO_ID = 0x03;

        public enum Linearity3TrsReportField
        {
            ReportID = 0,
            Reserved1,
            RXM0_LSB,
            RXM0_MSB,
            RXM1_LSB,
            RXM1_MSB,
            RXM2_LSB,
            RXM2_MSB,
            RXDev_LSB,
            RXDev_MSB,
            RX_Index,
	        TXM0_LSB = 18,
	        TXM0_MSB,
            TXM1_LSB,
            TXM1_MSB,
            TXM2_LSB,
            TXM2_MSB,
            TXDev_LSB,
            TXDev_MSB,
            TX_Index
        }

        public enum Linearity5TrsReportField
        {
            ReportID = 0,
            Reserved1,
            RXM0_LSB,
            RXM0_MSB,
            RXM1_LSB,
            RXM1_MSB,
            RXM2_LSB,
            RXM2_MSB,
            RXM3_LSB,
            RXM3_MSB,
            RXM4_LSB,
            RXM4_MSB,
            RXDev_LSB,
            RXDev_MSB,
            RX_Index,
            TXM0_LSB = 16,
            TXM0_MSB,
            TXM1_LSB,
            TXM1_MSB,
            TXM2_LSB,
            TXM2_MSB,
            TXM3_LSB,
            TXM3_MSB,
            TXM4_LSB,
            TXM4_MSB,
            TXDev_LSB,
            TXDev_MSB,
            TX_Index
        }

        public enum LinearityDataFormat
        { 
            DataFormat3Trs = 0,
            DataFormat5Trs
        }

        public const int PENDATA_OVERFLOW = 0x80;
        public const int PENDATA_OVERINDEX = 0xFF;

        public const int TOTAL_EACH_INDEX_64PARTS = 64;
        public const int TOTAL_EACH_INDEX_256PARTS = 256;
        public const int BASE_VALUE_64PARTS = 1024;
        public const int BASE_VALUE_256PARTS = 256;

        public const int REPORT_DATA_TYPE_RX = 0x01;
        public const int REPORT_DATA_TYPE_TX = 0x02;

        public enum EEPROMDataField
        {
            byLogicX_L = 0, //0
	        byLogicX_H, //1
	        byLogicY_L, //2
	        byLogicY_H, //3
	        byPhyMaxX_L, //4
	        byPhyMaxX_H, //5
	        byPhyMaxY_L, //6
	        byPhyMaxY_H, //7
	        byX_Gap,     //8
	        byY_Gap,     //9
	        byTPVersion_L,  //10
	        byTPVersion_H,  //11
	        byMaxFingers,   //12
	        byLogicX_L_Pen, //13
	        byLogicX_H_Pen, //14
	        byLogicY_L_Pen, //15
	        byLogicY_H_Pen, //16
	        byPen_PressureLevel_L,  //17
	        byPen_PressureLevel_H,  //18
	        byPen_FSK_Debug_Descriptor_Enable,  //19
	        byPTPMode_Enable,   //20
            byEnableVendorButtonCount,  //21
            byPenFSKCount,      //22
            byCenterScantime_Descriptor_Enable,//Center & Scantime Descriptor Enable:23
            byEnableHuaweiAddByte, //24
            byHuaweiByteCount, //25
            byVendorButtonCount, //26
            ByEPOutCount,        //27
            Max_EEPROMData_Count
        }

        //J2 ++
        /// <summary>
        /// Input the interface index that user selected on the UI.
        /// </summary>
        /// <param name="nSelInterfaceIdx"></param>
        /// <returns></returns>
        /*
        public static int GetInterfaceID(int nSelectInterfaceIdx)
        {
            if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_USB)
                return INTERFACE_USB;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_HID_OVER_I2C_TP ||
                     nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_HID_OVER_I2C_PoTP)
                return INTERFACE_HIDoverI2C;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_I2C ||
                     nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_I2C_TDDI)
                return INTERFACE_I2C;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING_HALF_CYCLE)
                return INTERFACE_SPI_MA_RISING_HALF_CYCLE;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING_HALF_CYCLE)
                return INTERFACE_SPI_MA_FALLING_HALF_CYCLE;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_RISING)
                return INTERFACE_SPI_MA_RISING;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_SPI_MA_FALLING)
                return INTERFACE_SPI_MA_FALLING;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_SPI_PRECISE)
                return INTERFACE_ITOUCH;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING_HALF_CYCLE)
                return INTERFACE_TDDI_SPI_MA_RISING_HALF_CYCLE;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING_HALF_CYCLE)
                return INTERFACE_TDDI_SPI_MA_FALLING_HALF_CYCLE;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_RISING)
                return INTERFACE_TDDI_SPI_MA_RISING;
            else if (nSelectInterfaceIdx == (int)UserInterfaceDefine.InterfaceType.IF_TDDI_SPI_MA_FALLING)
                return INTERFACE_TDDI_SPI_MA_FALLING;

            return -1;
        }
        */
        //J2 --

        public const int ERR_PARAMTUNE_VER_FW = -1;
        public const int ERR_PARAMTUNE_VER_INTERFACE = -2;
        public const int ERR_PARAMTUNE_VER_SOLUTIONID = -3;
        public const int ERR_PARAMTUNE_VER_PARAMVALUE = -4;

    }// ElanDef

}
