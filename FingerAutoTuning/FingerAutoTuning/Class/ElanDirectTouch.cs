using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Elan
{
    public class ElanDirectTouch
    {
        private const string DllPath = "LibTouch_Socket.dll";
        private const int BULK_OUTPUT_PACKET_LENGTH = 1024;

        public const int AFE_REPORT_ID = 0x03;
        public const int AFE_BULK_REPORT_ID = 0x13;

        public const int AFE_WRITE_CMD_HEADER_ID = 0x05;
        public const int AFE_READ_CMD_HEADER_ID = 0x06;

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern int Connect(int nVID, int nPID, int nInterface, int nVdd, int nVio, int nI2CAdr, int nI2CLength, int nTDDISPICLKRate, IntPtr pInterfaceParam);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern int Disconnect();

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern int DVIS_HIDovSPI_SetOutputReport(int iReportID, byte[] pDataOut, int iDataByte, int nTimeoutMS = -1, int nUSBDevIdx = 0);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern int DVIS_HIDovSPI_GetInputReport(int iReportID, byte[] pDataOut, int iDataByte, int iTimeoutMS = -1, int iUSBDevIdx = 0);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern int DVIS_HIDovSPI_IntInputReport(ref int iReportID, ref int iContentLen, byte[] pBuf, int iBufByteSz, int nTimeoutMS = 2500, int nUSBDevIdx = 0);

        public static int SPIWrite(int iReportID, IntPtr DataBuffer, int iDataByte, int nTimeoutMS = 2500, int nUSBDevIdx = 0)
        {
            byte[] ReportBuffer = new byte[iDataByte];
            Array.Clear(ReportBuffer, 0, ReportBuffer.Length);
            Marshal.Copy(DataBuffer, ReportBuffer, 0, iDataByte);

            int nRet = DVIS_HIDovSPI_SetOutputReport(iReportID, ReportBuffer, iDataByte, nTimeoutMS, nUSBDevIdx);
            Thread.Sleep(1);

            return nRet;
        }

        public static int SPIRead(int nInRptID, ref int nOutRptID, ref int iContentLen, IntPtr DataBuffer, int iBufByteSz, int nTimeoutMS, int nUSBDevIdx)
        {
            int nRet = 0;
            byte[] ReportBuffer = new byte[iBufByteSz];
            Array.Clear(ReportBuffer, 0, ReportBuffer.Length);

            nRet = DVIS_HIDovSPI_IntInputReport(ref nOutRptID, ref iContentLen, ReportBuffer, iBufByteSz, nTimeoutMS, nUSBDevIdx);

            Marshal.Copy(ReportBuffer, 0, DataBuffer, iContentLen);

            return nRet;
        }

        public static int AFERead(IntPtr CmdBuffer, int iDataByte, ref int nOutRptID, ref int iContentLen, IntPtr DataBuffer, int iBufByteSz, int nTimeoutMS, int nUSBDevIdx)
        {
            byte[] ReportBuffer = new byte[iDataByte];

            Array.Clear(ReportBuffer, 0, ReportBuffer.Length);

            Marshal.Copy(CmdBuffer, ReportBuffer, 0, iDataByte);

            int nRet = AFE_RD(ReportBuffer, iDataByte, ref nOutRptID, ref iContentLen, DataBuffer, iBufByteSz, nTimeoutMS, nUSBDevIdx);

            return nRet;
        }

        public static int AFE_WR_Single(ushort Para_Reg_Addr, ushort Para_Data)
        {
            int iRet = 0;
            int iReportID = AFE_REPORT_ID;
            byte[] dataOutID_03 = new byte[64];

            dataOutID_03[0] = (byte)(Para_Reg_Addr & 0xFF);
            dataOutID_03[1] = (byte)(Para_Reg_Addr >> 8);
            dataOutID_03[2] = (byte)(0x01);
            dataOutID_03[3] = (byte)(0x00);
            dataOutID_03[4] = (byte)(Para_Data & 0xFF);
            dataOutID_03[5] = (byte)(Para_Data >> 8);

            iRet = ElanDirectTouch.DVIS_HIDovSPI_SetOutputReport(iReportID, dataOutID_03, dataOutID_03.Length);

            return iRet;
        }

        public static int AFE_WR(ushort Para_Reg_Addr, ushort[] Para_RAM_Addr, int Para_Data_Len)
        {
            int iRet = 0;
            int iReportID = AFE_REPORT_ID;
            int j = 0;
            byte[] dataOutID_03 = new byte[64];

            while (Para_Data_Len > 0)
            {
                dataOutID_03[0] = (byte)(Para_Reg_Addr & 0xFF);
                dataOutID_03[1] = (byte)(Para_Reg_Addr >> 8);
                if (Para_Data_Len > 30)
                {
                    dataOutID_03[2] = (byte)(30);
                }
                else
                {
                    dataOutID_03[2] = (byte)(Para_Data_Len);
                }
                dataOutID_03[3] = (byte)(0x00);

                for (int i = 0; i < 60; i += 2)
                {
                    if (j < Para_Data_Len)
                    {
                        dataOutID_03[i + 4] = (byte)(Para_RAM_Addr[j] & 0xFF);
                        dataOutID_03[i + 5] = (byte)(Para_RAM_Addr[j++] >> 8);
                    }
                    else
                    {
                        dataOutID_03[i + 4] = (byte)0;
                        dataOutID_03[i + 5] = (byte)0;
                    }
                }

                iRet = DVIS_HIDovSPI_SetOutputReport(iReportID, dataOutID_03, dataOutID_03.Length);

                Para_Reg_Addr += 60;
                Para_Data_Len -= 30;
            }

            return iRet;
        }

        public static int AFE_WR_BULK(ushort Address, ushort[] DataBufferWord, int DataBufferWordLength)
        {
            int iRet = 0;
            int iReportID = AFE_BULK_REPORT_ID;
            int nProcessDataCount = 0;
            int nPutDataCount = 0;
            byte[] DataOutputBuffer = new byte[BULK_OUTPUT_PACKET_LENGTH];
            ushort nRamStartAddr = Address;

            while (nProcessDataCount < DataBufferWordLength)
            {
                Array.Clear(DataOutputBuffer, 0, DataOutputBuffer.Length);

                DataOutputBuffer[0] = (byte)((nRamStartAddr + (nProcessDataCount * 2)) & 0xFF);
                DataOutputBuffer[1] = (byte)((nRamStartAddr + (nProcessDataCount * 2)) >> 8);
                DataOutputBuffer[2] = (byte)(DataOutputBuffer.Length & 0xFF);
                DataOutputBuffer[3] = (byte)(DataOutputBuffer.Length >> 8);
                nPutDataCount = 4;

                while (true)
                {
                    DataOutputBuffer[nPutDataCount++] = (byte)(DataBufferWord[nProcessDataCount] & 0xFF);
                    DataOutputBuffer[nPutDataCount++] = (byte)(DataBufferWord[nProcessDataCount++] >> 8);

                    if (nPutDataCount >= BULK_OUTPUT_PACKET_LENGTH)
                        break;

                    if (nProcessDataCount >= DataBufferWordLength)
                        break;
                }

                iRet = DVIS_HIDovSPI_SetOutputReport(iReportID, DataOutputBuffer, DataOutputBuffer.Length);
                Thread.Sleep(1);
                if (iRet != 0)
                    return iRet;
            }

            return iRet;
        }

        public static int AFE_RD(byte[] byDataBuf, int iDataByte, ref int nOutRptID, ref int iContentLen, IntPtr OutDataBuffer, int iBufByteSz, int nTimeoutMS, int nUSBDevIdx)
        {
            byte[] ReportBuffer = new byte[iBufByteSz];
            int nRet = 0;

            Array.Clear(ReportBuffer, 0, ReportBuffer.Length);

            nRet = DVIS_HIDovSPI_GetInputReport(AFE_REPORT_ID, byDataBuf, iDataByte, nTimeoutMS, nUSBDevIdx);
            if (nRet != 0)
                return nRet;

            nRet = DVIS_HIDovSPI_IntInputReport(ref nOutRptID, ref iContentLen, ReportBuffer, iBufByteSz, nTimeoutMS, nUSBDevIdx);

            Marshal.Copy(ReportBuffer, 0, OutDataBuffer, iContentLen);

            return nRet;
        }
    }

    public class ElanDirectTochMainFlow
    {
        [DllImport("libElanAlgorithm.dll")]
        public static extern bool Initialize();

        [DllImport("libElanAlgorithm.dll")]
        public static extern void Uninitialize();

        [DllImport("libElanAlgorithm.dll")]
        public static extern bool LoadParameter();

        [DllImport("libElanAlgorithm.dll")]
        public static extern bool GetTraceInfo(ref short nXTraceNum, ref short nYTraceNum);

        [DllImport("libElanAlgorithm.dll")]
        public static extern bool SetTraceInfo(short nXTraceNum, short nYTraceNum);

        [DllImport("libElanAlgorithm.dll")]
        public static extern bool ProcessRawData(ushort[] RawDataArray, ref ushort FrameCount, short[] dV, short[] Base, short[] XAxisArray, short[] YAxisArray, ref ushort nFingerStatus);

        ////////////////////////////////////////////////////////
        //Callback function pointer declare
        ////////////////////////////////////////////////////////
        #region Simulate main flow
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int FUNCPTR_DVIS_HIDovSPI_SetOutputReport(int iReportID, IntPtr pDataOut, int iDataByte, int nTimeoutMS = -1, int nUSBDevIdx = 0);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int FUNCPTR_DVIS_HIDovSPI_IntInputReport(int InRptID, ref int nOutRptID, ref int iContentLen, IntPtr pBuf, int iBufByteSz, int nTimeoutMS = 2500, int nUSBDevIdx = 0);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int FUNCPTR_DVIS_HIDovSPI_AFERead(IntPtr pCmdBuffer, int iDataByte, ref int nOutRptID, ref int iContentLen, IntPtr pDataBuf, int iBufBytgeSz, int nTimeoutMS = 2500, int nUSBDevIdx = 0);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void FUNCPTR_TEST_MODE_DATA(IntPtr pADC, IntPtr pdV, IntPtr pBase, IntPtr pReport, int nRawDataLength, int nReportDataLength);

        [DllImport("libElanAlgorithm.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SimulateMainFlow(FUNCPTR_DVIS_HIDovSPI_SetOutputReport pBridgeWriteFuncPtr,
                                                         FUNCPTR_DVIS_HIDovSPI_IntInputReport pBridgeReadFuncPtr,
                                                         FUNCPTR_DVIS_HIDovSPI_AFERead pBridgeAFEReadFuncPtr,
                                                         FUNCPTR_TEST_MODE_DATA pSendFrameReportDataFuncPtr);

        [DllImport("libElanAlgorithm.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void StopMainFlow();
        #endregion

        //[DllImport("libElanTouchAlgorithm.dll")]
        //public static extern ushort AlgorithmInit(int X, int Y, int BufferLength);

        //[DllImport("libElanTouchAlgorithm.dll")]
        //public static extern bool AlgorithmPrintString(StringBuilder Message);

        //[DllImport("libElanTouchAlgorithm.dll")]
        //public static extern void Algorithm_GetTmpBuf(int[] AP_Labbuf);

        //[DllImport("libElanTouchAlgorithm.dll")]
        //public static extern short AlgorithmOperation(byte[] AP_RawBuf, short[] dv_in, short[] baseData, short[] IIRX, short[] IIRY, int DataSourceMode);
    }
}
