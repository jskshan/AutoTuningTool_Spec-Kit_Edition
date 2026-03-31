using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using FingerAutoTuning;
using FingerAutoTuningParameter;

namespace Elan
{
    class ElanCommand_Gen6or7
    {
        private int m_nTPReg_Offset_7318 = ParamFingerAutoTuning.m_nRawADCSTPReg_Offset_7318;
        private int m_nTPReg_Offset_7315 = ParamFingerAutoTuning.m_nRawADCSTPReg_Offset_7315;
        private int m_nTPReg_Offset_6315 = ParamFingerAutoTuning.m_nRawADCSTPReg_Offset_6315;

        public class SendCommandInfo
        {
            public List<CommandInfo> cCommandInfo_List = new List<CommandInfo>();
        }

        public class CommandInfo
        {
            //public byte[] byteCommand_Array;
            public int nDelayTime = 100;
        }

        public enum ParameterType
        {
            //Raw ADC Sweep
            _MS_FIRTB,
            _MS_FIR_TAP_NUM,
            _MS_DFT_NUM,
            _IQ_BSH,
            _MS_ANA_CTL_2,
            _MS_ANA_CTL_3,
            _MS_ANA_CTL_4,
            _MS_ANA_CTL_5,
            _MS_ANA_CTL_6,
            _MS_ANA_CTL_8,
        }

        public class DataType
        {
            public const byte byteRead      = 0x96;
            public const byte byteWrite     = 0x97;
            public const byte byteReadACK   = 0x95;
        }

        public class WriteCommandInfo
        {
            public string sParameterName = "";
            public byte byteType = 0x00;
            public byte byteAddress_H = 0x00;
            public byte byteAddress_L = 0x00;
            public byte byteValue_H = 0x00;
            public byte byteValue_L = 0x00;
        }

        public class ReadCommandInfo
        {
            public string sParameterName = "";
            public byte byteType = 0x00;
            public byte byteAddress_H = 0x00;
            public byte byteAddress_L = 0x00;
        }

        private frmMain m_cfrmParent = null;
        public static int m_nIN_DATA_LENGTH = 65;

        private int m_nDeviceIndex = 0;
        private bool m_bSocketConnectType = false;

        public WriteCommandInfo m_cWriteCommandInfo = new WriteCommandInfo();
        public ReadCommandInfo m_cReadCommandInfo = new ReadCommandInfo();

        private string m_sErrorMessage = "";

        private ICGenerationType m_eICGenerationType = ICGenerationType.None;
        private ICSolutionType m_eICSolutionType = ICSolutionType.NA;

        public ElanCommand_Gen6or7(frmMain cfrmParent, int nDeviceIndex, bool bSocket, ICGenerationType eICGenerationType, ICSolutionType eICSolutionType)
        {
            m_cfrmParent = cfrmParent;
            m_nDeviceIndex = nDeviceIndex;
            m_bSocketConnectType = bSocket;

            m_eICGenerationType = eICGenerationType;
            m_eICSolutionType = eICSolutionType;
        }

        private string GetParameterName(ParameterType eParameterType)
        {
            string sParameterName = "";

            switch (eParameterType)
            {
                case ParameterType._MS_FIRTB:
                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                            sParameterName = "_MS_FIRTB";
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                            sParameterName = "_MS_FIRTB";
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                            sParameterName = "_MS_FIRTB";
                    }

                    break;
                case ParameterType._MS_FIR_TAP_NUM:
                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                            sParameterName = "_MS_FIR_TAP_NUM";
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                            sParameterName = "_MS_FIR_TAP_NUM";
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                            sParameterName = "_MS_FIR_TAP_NUM";
                    }

                    break;
                case ParameterType._MS_DFT_NUM:
                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                            sParameterName = "_MS_DFT_NUM";
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                            sParameterName = "_MS_DFT_NUM";
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                            sParameterName = "_MS_DFT_NUM";
                    }

                    break;
                case ParameterType._IQ_BSH:
                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                            sParameterName = "_MS_IQ_BSH_GP0";
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                            sParameterName = "_MS_IQ_BSH_GP0";
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                            sParameterName = "_IQ_BSH";
                    }

                    break;
                case ParameterType._MS_ANA_CTL_2:
                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                            sParameterName = "_MS_ANA_CTL_2";
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                            sParameterName = "_MS_ANA_CTL_2";
                    }

                    break;
                case ParameterType._MS_ANA_CTL_3:
                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                            sParameterName = "_MS_ANA_CTL_3";
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                            sParameterName = "_MS_ANA_CTL_3";
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                            sParameterName = "_MS_ANA_CTL_3";
                    }

                    break;
                case ParameterType._MS_ANA_CTL_4:
                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                            sParameterName = "_MS_ANA_CTL_4";
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                            sParameterName = "_MS_ANA_CTL_4";
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                            sParameterName = "_MS_ANA_CTL_4";
                    }

                    break;
                case ParameterType._MS_ANA_CTL_5:
                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                            sParameterName = "_MS_ANA_CTL_5";
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                            sParameterName = "_MS_ANA_CTL_5";
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                            sParameterName = "_MS_ANA_CTL_5";
                    }

                    break;
                case ParameterType._MS_ANA_CTL_6:
                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                            sParameterName = "_MS_ANA_CTL_6";
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                            sParameterName = "_MS_ANA_CTL_6";
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                            sParameterName = "_MS_ANA_CTL_6";
                    }

                    break;
                case ParameterType._MS_ANA_CTL_8:
                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                            sParameterName = "_MS_ANA_CTL_8";
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                            sParameterName = "_MS_ANA_CTL_8";
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                            sParameterName = "_MS_ANA_CTL_8";
                    }

                    break;
                default:
                    break;
            }

            return sParameterName;
        }

        private void SetWriteCommandInfo(ParameterType eParameterType, int nValue = 0)
        {
            m_cWriteCommandInfo = new WriteCommandInfo();

            byte byteHighByte = 0x00;
            byte byteLowByte = 0x00;
            int nAddress = 0x0000;

            m_cWriteCommandInfo.sParameterName = GetParameterName(eParameterType);
            m_cWriteCommandInfo.byteType = DataType.byteWrite;

            switch (eParameterType)
            {
                case ParameterType._MS_FIRTB:
                    nAddress = m_nTPReg_Offset_7318 + 0x027C;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x027C;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x0258;
                        }
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x0125;
                        }
                    }

                    m_cWriteCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_FIR_TAP_NUM:
                    nAddress = m_nTPReg_Offset_7318 + 0x002D;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x002D;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x002A;
                        }
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x002D;
                        }
                    }

                    m_cWriteCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_DFT_NUM:
                    nAddress = m_nTPReg_Offset_7318 + 0x002C;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x002C;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x0029;
                        }
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x002C;
                        }
                    }

                    m_cWriteCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._IQ_BSH:
                    nAddress = m_nTPReg_Offset_7318 + 0x0025;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x0025;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x0022;
                        }
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x0028;
                        }
                    }

                    m_cWriteCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_2:
                    nAddress = m_nTPReg_Offset_7318 + 0x0040;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x0040;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x003D;
                        }
                    }

                    m_cWriteCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_3:
                    nAddress = m_nTPReg_Offset_7318 + 0x0041;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x0041;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x003E;
                        }
                    }

                    m_cWriteCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_4:
                    nAddress = m_nTPReg_Offset_6315 + 0x003E;

                    if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x003E;
                        }
                    }

                    m_cWriteCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_5:
                    nAddress = m_nTPReg_Offset_7318 + 0x0043;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x0043;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x0040;
                        }
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x003F;
                        }
                    }

                    m_cWriteCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_6:
                    nAddress = m_nTPReg_Offset_7318 + 0x0044;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x0044;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x0041;
                        }
                    }

                    m_cWriteCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_8:
                    
                    nAddress = m_nTPReg_Offset_6315 + 0x0042;

                    if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x0042;
                        }
                    }

                    m_cWriteCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cWriteCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                default:
                    break;
            }

            byteHighByte = (byte)((nValue & 0xFF00) >> 8);
            byteLowByte = (byte)(nValue & 0xFF);

            m_cWriteCommandInfo.byteValue_H = byteHighByte;
            m_cWriteCommandInfo.byteValue_L = byteLowByte;
        }

        private void SetReadCommandInfo(ParameterType eParameterType)
        {
            m_cReadCommandInfo = new ReadCommandInfo();

            int nAddress = 0x0000;

            m_cReadCommandInfo.sParameterName = GetParameterName(eParameterType);
            m_cReadCommandInfo.byteType = DataType.byteRead;

            switch (eParameterType)
            {
                case ParameterType._MS_FIRTB:
                    nAddress = m_nTPReg_Offset_7318 + 0x027C;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x027C;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x0258;
                        }
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x0125;
                        }
                    }

                    m_cReadCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_FIR_TAP_NUM:
                    nAddress = m_nTPReg_Offset_7318 + 0x002D;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x002D;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x002A;
                        }
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x002D;
                        }
                    }

                    m_cReadCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_DFT_NUM:
                    nAddress = m_nTPReg_Offset_7318 + 0x002C;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x002C;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x0029;
                        }
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x002C;
                        }
                    }

                    m_cReadCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._IQ_BSH:
                    nAddress = m_nTPReg_Offset_7318 + 0x0025;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x0025;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x0022;
                        }
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x0028;
                        }
                    }

                    m_cReadCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_2:
                    nAddress = m_nTPReg_Offset_7318 + 0x0040;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x0040;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x003D;
                        }
                    }

                    m_cReadCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_3:
                    nAddress = m_nTPReg_Offset_7318 + 0x0041;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x0041;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x003E;
                        }
                    }

                    m_cReadCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_4:
                    nAddress = m_nTPReg_Offset_6315 + 0x003E;

                    if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x003E;
                        }
                    }

                    m_cReadCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_5:
                    nAddress = m_nTPReg_Offset_7318 + 0x0043;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x0043;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x0040;
                        }
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x003F;
                        }
                    }

                    m_cReadCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_6:
                    nAddress = m_nTPReg_Offset_7318 + 0x0044;

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_7318)
                        {
                            nAddress = m_nTPReg_Offset_7318 + 0x0044;
                        }
                        else if (m_eICSolutionType == ICSolutionType.Solution_7315)
                        {
                            nAddress = m_nTPReg_Offset_7315 + 0x0041;
                        }
                    }

                    m_cReadCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                case ParameterType._MS_ANA_CTL_8:
                    nAddress = m_nTPReg_Offset_6315 + 0x0042;

                    if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_6315 || m_eICSolutionType == ICSolutionType.Solution_5015M)
                        {
                            nAddress = m_nTPReg_Offset_6315 + 0x0042;
                        }
                    }

                    m_cReadCommandInfo.byteAddress_H = (byte)((nAddress & 0xFF00) >> 8);
                    m_cReadCommandInfo.byteAddress_L = (byte)(nAddress & 0x00FF);
                    break;
                default:
                    break;
            }
        }

        public void SendWriteCommand(ParameterType eParameterType, int nValue = 0)
        {
            SetWriteCommandInfo(eParameterType, nValue);
            SendWriteCommand();
        }

        private void SendWriteCommand()
        {
            string sParameterName = m_cWriteCommandInfo.sParameterName;
            byte byteType = m_cWriteCommandInfo.byteType;
            byte byteAddress_H = m_cWriteCommandInfo.byteAddress_H;
            byte byteAddress_L = m_cWriteCommandInfo.byteAddress_L;
            byte byteValue_H = m_cWriteCommandInfo.byteValue_H;
            byte byteValue_L = m_cWriteCommandInfo.byteValue_L;

            byte[] byteCommand_Array = new byte[] 
            { 
                byteType,
                byteAddress_H, 
                byteAddress_L, 
                byteValue_H, 
                byteValue_L, 
                0x01
            };

            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

            /*
            string sMessage = string.Format("-Set {0}(Word=0x{1}{2})", sParameterName, byteAddress_H.ToString("X2"), byteAddress_L.ToString("X2"));
            m_cfrmParent.MessageOutput(sMessage);
            */

            string sSendCommand = "";

            for (int nByteIndex = 0; nByteIndex < byteCommand_Array.Length; nByteIndex++)
                sSendCommand += string.Format(" {0}", byteCommand_Array[nByteIndex].ToString("X2"));

            m_cfrmParent.OutputDebugLog(string.Format("Send Write Command :{0}", sSendCommand));

            Thread.Sleep(20);
        }

        public bool ReadData(ref int nValue, ParameterType eParameterType, int nTimeout = 1000)
        {
            nValue = -1;

            bool bResultFlag = false;
            int nRetryCount = 50;

            SetReadCommandInfo(eParameterType);
            SendReadCommand();

            while (nRetryCount > 0)
            {
                byte[] byteData_Array = new byte[m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType, 1000);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                {
                    m_cfrmParent.OutputDebugLog(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(nTimeout);
                    bResultFlag = false;
                    break;
                }

                string sGetACK = "Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                m_cfrmParent.OutputDebugLog(sGetACK);

                if (byteData_Array[0] == DataType.byteReadACK && byteData_Array[1] == m_cReadCommandInfo.byteAddress_H && byteData_Array[2] == m_cReadCommandInfo.byteAddress_L)
                {
                    nValue = (ushort)(byteData_Array[3] << 8 | byteData_Array[4]);
                    bResultFlag = true;
                    break;
                }

                nRetryCount--;
            }

            Thread.Sleep(20);
            return bResultFlag;
        }

        private void SendReadCommand()
        {
            string sParameterName = m_cReadCommandInfo.sParameterName;
            byte byteType = m_cReadCommandInfo.byteType;
            byte byteAddress_H = m_cReadCommandInfo.byteAddress_H;
            byte byteAddress_L = m_cReadCommandInfo.byteAddress_L;

            byte[] byteCommand_Array = new byte[] 
            { 
                byteType,  
                byteAddress_H, 
                byteAddress_L,
                0x00, 
                0x00, 
                0x01
            };

            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);
            //Thread.Sleep(20);

            string sSendCommand = "";

            for (int nByteIndex = 0; nByteIndex < byteCommand_Array.Length; nByteIndex++)
                sSendCommand += string.Format(" {0}", byteCommand_Array[nByteIndex].ToString("X2"));

            m_cfrmParent.OutputDebugLog(string.Format("Send Read Command :{0}", sSendCommand));
        }

        public string GetErrorMessage()
        {
            return m_sErrorMessage;
        }
    }
}
