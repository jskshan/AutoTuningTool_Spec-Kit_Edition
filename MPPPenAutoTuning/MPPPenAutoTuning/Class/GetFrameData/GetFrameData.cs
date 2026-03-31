using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Elan;

namespace MPPPenAutoTuning
{
    public class GetFrameData
    {
        private frmMain m_cfrmMain = null;
        private ProcessFlow m_cProcessFlow = null;

        private FrameMgr m_cFrameMgr = new FrameMgr();
        private BlockingQueue.BlockingQueue<Frame> m_cFrameQueue = new BlockingQueue.BlockingQueue<Frame>();
        private ElanTouch.TraceInfo m_structTraceInfo;
        private int m_nICType = 0;
        private string m_sICType = "";

        private int m_nTXTraceNumber = 0;
        private int m_nRXTraceNumber = 0;
        private int m_nFrameNumber = 300;
        private int m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_NONE;
        private int m_nDeviceIndex = -1;

        private int[,,] m_nFrame_Array;

        //private bool m_bEnterTestModeFlag = false;
        private string m_sErrorMessage = "";
        private string m_sFrameMessage = "";
        private int m_nFrameCount = 0;
        private GetDataState m_eGetDataState = GetDataState.GetDataState_NA;

        public enum RelatedStep
        {
            Step_SetRawDataCount,
            Step_EnterTestMode,
            Step_Reconnect,
            Step_ExitTestMode,
            Step_TransferTestModeViaHID
        };

        public enum GetDataState
        {
            GetDataState_NA,
            GetDataState_Success,
            GetDataState_ReconnectError,
            GetDataState_ForceStop,
            GetDataState_GetError,
            GetDataState_SaveError
        }

        public GetFrameData(frmMain cfrmMain,
                            ProcessFlow cProcessFlow,
                            int nTXTraceNumber,
                            int nRXTraceNumber,
                            int nICSolutionType,
                            int nDeviceIndex,
                            ElanTouch.TraceInfo structTraceInfo,
                            int nFrameNumber,
                            uint nFWVersion)
        {
            m_cfrmMain = cfrmMain;
            m_cProcessFlow = cProcessFlow;
            m_nTXTraceNumber = nTXTraceNumber;
            m_nRXTraceNumber = nRXTraceNumber;
            m_nICSolutionType = nICSolutionType;
            m_nDeviceIndex = nDeviceIndex;
            m_structTraceInfo = structTraceInfo;
            m_nFrameNumber = nFrameNumber;

            m_nFrame_Array = new int[nFrameNumber, m_nTXTraceNumber + 1, m_nRXTraceNumber + 1];
            Array.Clear(m_nFrame_Array, 0, m_nFrame_Array.Length);

            GetICTypeAndInfo(nFWVersion);
        }

        private void GetICTypeAndInfo(uint nFWVersion)
        {
            string sText = "";
            string sSubVersion = "";
            m_nICType = ParseICType(nFWVersion >> 8, ref sSubVersion);

            if (m_nICType == 0)
            {
                sText = "0";
                return;
            }

            int nNumber = 0;

            /*
            if (m_nICType == 3741056)
            {
                m_OutputQueue.Enqueue(new OutputItem("[IC Type] 3915 x " + m_TraceInfo.nChipNum));
            }
            else if (m_nICType == 25365)
            {
                m_OutputQueue.Enqueue(new OutputItem("[IC Type] 6315 x " + m_TraceInfo.nChipNum));
            }
            else if (m_nICType == 471378)
            {
                m_OutputQueue.Enqueue(new OutputItem("[IC Type] 7315 x " + m_TraceInfo.nChipNum));
            }
            else if (m_nICType == 471426)
            {
                m_OutputQueue.Enqueue(new OutputItem("[IC Type] 7318 x " + m_TraceInfo.nChipNum));
            }
            else
            {
                m_OutputQueue.Enqueue(new OutputItem("[IC Type] " + m_nIC_Type.ToString("X4") + " x " + m_TraceInfo.nChipNum));
            }
            */

            if (m_nICType == 20501)
            {
                sText = string.Format("{0}{1}", m_nICType.ToString("X"), sSubVersion);
            }
            else if (m_nICType == 3741056)
            {
                sText = string.Format("{0}{1}", "3915", sSubVersion);
            }
            else if (m_nICType == 25365)
            {
#if _USE_VC2010
                sText = string.Format("{0}{1}", nNumber, sSubVersion);
#else
                sText = $"{nNumber}{sSubVersion}";
#endif
            }
            else if (m_nICType == 471378)
            {
                sText = 29461.ToString("x");
            }
            else if (m_nICType == 471426)
            {
                sText = 29464.ToString("x");
            }
            else
            {
                sText = string.Format("{0}", m_nICType.ToString("X"));

                if (25352 == m_nICType || 29461 == m_nICType || 29464 == m_nICType)
                {
                    m_nICType = 25365;
                }
                else if (36617 == m_nICType || 36625 == m_nICType)
                {
                    m_nICType = 590336;
                }
            }

            m_sICType = sText;
            return;
        }

        private int ParseICType(uint nSolutionID, ref string sSubVersion)
        {
            //int nNumber = 0;
            sSubVersion = "";

            switch (nSolutionID)
            {
                case 0u:
                case 1u:
                case 2u:
                case 144u:
                    return 13056;
                case 16u:
                case 17u:
                case 18u:
                case 19u:
                    return 14592;
                case 20u:
                    sSubVersion = "M";
                    return 14613;
                case 21u:
                    sSubVersion = "P";
                    return 3741056;
                case 32u:
                    return 14624;
                case 48u:
                    return 12897;
                case 64u:
                case 96u:
                case 112u:
                case 128u:
                    return 12616;
                case 80u:
                case 81u:
                case 82u:
                case 83u:
                    return 20992;
                case 85u:
                case 240u:
                    return 12880;
                case 86u:
                case 87u:
                case 88u:
                    return 21266;
                case 89u:
                    sSubVersion = "M";
                    return 20501;
                case 97u:
                case 98u:
                    return 25365;
                case 99u:
                    return 25352;
                case 100u:
                    return 29461;
                case 101u:
                    return 471378;
                case 102u:
                case 160u:
                    return 12896;
                case 103u:
                    return 29464;
                case 104u:
                    return 471426;
                case 129u:
                    return 36617;
                case 130u:
                    return 36625;
                case 161u:
                    return 12888;
                case 192u:
                    return 26112;
                case 176u:
                    return 26400;
                case 208u:
                    return 26416;
                case 224u:
                    return 26448;
                case 127u:
                    return 28672;
                default:
                    return 0;
            }
        }

        public GetDataState GetdVFrameData(string sStageMessage = "", bool bGetSelfFlag = true)
        {
            m_eGetDataState = GetDataState.GetDataState_NA;
            int nRetryCount = 0;
            int nFrameNumber = m_nFrameNumber;

            int nRXTotalTraceNumber = m_nRXTraceNumber;
            int nTXTotalTraceNumber = m_nTXTraceNumber;
            int nRETRYCOUNTDOWN = 6;
            int nTimeout = 1000;
            UserInterfaceDefine.RawDataType eRawDataType = UserInterfaceDefine.RawDataType.Type_dV;
            byte bTraceType = 0x01;

            if (sStageMessage != "")
                sStageMessage = string.Format("{0} ", sStageMessage);

            int[,] nFrameBuffer_Array = null;
            long nStartTick = 0;

            string sMessage = string.Format("-Get dV Data[FrameNumber={0}]", nFrameNumber);
            OutputMessage(sMessage);

            //Declare a temp buffer get 1-d array from c++ function
            int[] nDataBuffer_Array = null;

            if (bGetSelfFlag == true)
                nDataBuffer_Array = new int[(m_nTXTraceNumber + 2) * (m_nRXTraceNumber + 2)];
            else
                nDataBuffer_Array = new int[(m_nTXTraceNumber + 1) * (m_nRXTraceNumber + 1)];

            GetDataRelatedStep(RelatedStep.Step_ExitTestMode);
            GetDataRelatedStep(RelatedStep.Step_SetRawDataCount);
            GetDataRelatedStep(RelatedStep.Step_EnterTestMode);
            GetDataRelatedStep(RelatedStep.Step_TransferTestModeViaHID);

            #region Skip 2 Frame
            nFrameNumber = 2;
            m_nFrameCount = 0;

            /*
            m_cfrmMain.BeginInvoke((MethodInvoker)delegate
            {
                SetDisplayFrameNumber(string.Format("Skip {0}", sDataType), 0, nFrameCount);
            });
            */

            m_sFrameMessage = string.Format("Skip dV Frame Data({0}/{1})", 0, nFrameNumber);

            m_cfrmMain.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmMain.SetFrameNumber(m_sFrameMessage);
            });

            OutputMessage(string.Format("-{0}", m_sFrameMessage));

            for (int i = 0; i < nFrameNumber; i++)
            {
                bool bGetDataFlag = false;
                int nRetryTimes = nRetryCount;

                for (int nRetryIndex = nRetryTimes; nRetryIndex <= nRETRYCOUNTDOWN; nRetryIndex++)
                {
                    if (m_cfrmMain.m_bForceStopFlowEnableFlag == true)
                    {
                        m_eGetDataState = GetDataState.GetDataState_ForceStop;
                        return m_eGetDataState;
                    }

                    if (nRetryIndex > nRetryTimes)
                    {
                        m_cProcessFlow.Set_Test_Mode(false);

                        bool bResultFlag = m_cProcessFlow.RunElanDeviceReConnect();

                        if (bResultFlag == false)
                        {
                            m_sErrorMessage = "Reconnect Error";
                            m_eGetDataState = GetDataState.GetDataState_ReconnectError;
                            return m_eGetDataState;
                        }

                        Thread.Sleep(1000);

                        m_cProcessFlow.Set_Test_Mode(true);

                        GetDataRelatedStep(RelatedStep.Step_TransferTestModeViaHID);
                    }

                    for (int nSubRetryIndex = 0; nSubRetryIndex <= 3; nSubRetryIndex++)
                    {
                        if (m_cfrmMain.m_bForceStopFlowEnableFlag == true)
                        {
                            m_eGetDataState = GetDataState.GetDataState_ForceStop;
                            return m_eGetDataState;
                        }

                        Array.Clear(nDataBuffer_Array, 0, nDataBuffer_Array.Length);

                        int nResult = GetDataMainStep(ref nDataBuffer_Array, ref nRetryCount, ref bGetDataFlag, sStageMessage, m_nRXTraceNumber, m_nTXTraceNumber, bTraceType, 
                                                      nTimeout, i, nRetryIndex, true);

                        if (nResult == ElanTouch.TP_SUCCESS)
                        {
                            bGetDataFlag = true;
                            break;
                        }

                        Thread.Sleep(10);
                    }

                    if (bGetDataFlag == false)
                    {
                        nRetryCount++;
                        if (nRetryCount > nRETRYCOUNTDOWN)
                            break;
                    }
                    else
                        break;
                }

                //string sProgressString = string.Format("{0}Set : {1}/{2} ({3} Skip Frame : {4}/{5})", sStageStr, nListIndex, nListCount, sDataType, i + 1, nFrameCount);

                /*
                m_cfrmMain.BeginInvoke((MethodInvoker)delegate
                {
                    SetDisplayFrameNumber(string.Format("Skip {0}", sDataType), i + 1, nFrameCount);
                });
                */

                m_sFrameMessage = string.Format("Skip dV Frame Data({0}/{1})", i + 1, nFrameNumber);

                m_cfrmMain.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmMain.SetFrameNumber(m_sFrameMessage);
                });

                OutputMessage(string.Format("-{0}", m_sFrameMessage));

                if (bGetDataFlag == false)
                {
                    GetDataRelatedStep(RelatedStep.Step_ExitTestMode);
                    m_sErrorMessage = "Get dV Data Fail";
                    m_eGetDataState = GetDataState.GetDataState_GetError;
                    return m_eGetDataState;
                }

                Thread.Sleep(10);
            }
            #endregion

            #region Get n Frames
            nRetryCount = 0;

            m_cFrameQueue.Clear();
            //Get the start tick. Use to compute the timestamp.
            nStartTick = DateTime.Now.Ticks;
            Thread.Sleep(ElanDefine.TIME_100MS);

            nFrameNumber = m_nFrameNumber;

            /*
            m_cfrmMain.BeginInvoke((MethodInvoker)delegate
            {
                SetDisplayFrameNumber(string.Format("Get {0}", sDataType), 0, nFrameCount);
            });
            */

            m_sFrameMessage = string.Format("Get dV Frame Data({0}/{1})", 0, nFrameNumber);

            m_cfrmMain.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmMain.SetFrameNumber(m_sFrameMessage);
            });

            OutputMessage(string.Format("-{0}", m_sFrameMessage));

            for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
            {
                bool bGetDataFlag = false;
                int nRetryTimes = nRetryCount;

                for (int nRetryIndex = nRetryTimes; nRetryIndex <= nRETRYCOUNTDOWN; nRetryIndex++)
                {
                    //When the flag set to true, break current process.
                    if (m_cfrmMain.m_bForceStopFlowEnableFlag == true)
                    {
                        m_eGetDataState = GetDataState.GetDataState_ForceStop;
                        return m_eGetDataState;
                    }

                    if (nRetryIndex > nRetryTimes)
                    {
                        m_cProcessFlow.Set_Test_Mode(false);

                        bool bResultFlag = m_cProcessFlow.RunElanDeviceReConnect();

                        if (bResultFlag == false)
                        {
                            m_sErrorMessage = "Reconnect Error";
                            m_eGetDataState = GetDataState.GetDataState_ReconnectError;
                            return m_eGetDataState;
                        }

                        Thread.Sleep(1000);

                        m_cProcessFlow.Set_Test_Mode(true);

                        GetDataRelatedStep(RelatedStep.Step_TransferTestModeViaHID);
                    }

                    for (int nSubRetryIndex = 0; nSubRetryIndex <= 3; nSubRetryIndex++)
                    {
                        //When the flag set to true, break current process.
                        if (m_cfrmMain.m_bForceStopFlowEnableFlag == true)
                            return GetDataState.GetDataState_ForceStop;

                        if (bGetSelfFlag == true)
                            nFrameBuffer_Array = new int[m_nTXTraceNumber + 2, m_nRXTraceNumber + 2];
                        else
                            nFrameBuffer_Array = new int[m_nTXTraceNumber + 1, m_nRXTraceNumber + 1];

                        Array.Clear(nFrameBuffer_Array, 0, nFrameBuffer_Array.Length);

                        Array.Clear(nDataBuffer_Array, 0, nDataBuffer_Array.Length);
                        int nResult = GetDataMainStep(ref nDataBuffer_Array, ref nRetryCount, ref bGetDataFlag, sStageMessage, m_nRXTraceNumber, m_nTXTraceNumber, bTraceType,
                                                      nTimeout, nFrameIndex, nRetryIndex, false);

                        if (nResult == ElanTouch.TP_SUCCESS)
                        {
                            bGetDataFlag = true;
                            break;
                        }

                        Thread.Sleep(10);
                    }

                    if (bGetDataFlag == false)
                    {
                        nRetryCount++;
                        if (nRetryCount > nRETRYCOUNTDOWN)
                            break;
                    }
                    else
                        break;
                }

                //string sProgressString = string.Format("{0}Set : {1}/{2} ({3} Get Frame : {4}/{5})", sStageStr, nListIndex, nListCount, sDataType, nFrameIndex + 1, nFrameCount);

                /*
                m_cfrmMain.BeginInvoke((MethodInvoker)delegate
                {
                    SetDisplayFrameNumber(string.Format("{0} Get", sDataType), nFrameIndex + 1, nFrameCount);
                });
                */

                m_sFrameMessage = string.Format("Get dV Frame Data({0}/{1})", nFrameIndex + 1, nFrameNumber);

                m_cfrmMain.BeginInvoke((MethodInvoker)delegate
                {
                    m_cfrmMain.SetFrameNumber(m_sFrameMessage);
                });

                OutputMessage(string.Format("-{0}", m_sFrameMessage));

                if (bGetDataFlag == false)
                {
                    GetDataRelatedStep(RelatedStep.Step_ExitTestMode);
                    m_sErrorMessage = "Get dV Data Fail";
                    m_eGetDataState = GetDataState.GetDataState_GetError;
                    return m_eGetDataState;
                }

                if (bGetSelfFlag == true)
                    ArrayCopy(nDataBuffer_Array, m_nFrame_Array, nFrameIndex, m_nRXTraceNumber, m_nTXTraceNumber, m_nRXTraceNumber + 2);
                else
                    ArrayCopy(nDataBuffer_Array, m_nFrame_Array, nFrameIndex, m_nRXTraceNumber, m_nTXTraceNumber, m_nRXTraceNumber + 1);

                int nXLength = m_nRXTraceNumber;
                int nYLength = m_nTXTraceNumber;

                if (bGetSelfFlag == true)
                {
                    nXLength = m_nRXTraceNumber + 1;
                    nYLength = m_nTXTraceNumber + 1;
                }

                ArrayCopy(nDataBuffer_Array, nFrameBuffer_Array, nXLength, nYLength, nXLength + 1);

                //Get the timestamp
                long nTimeStamp = ((DateTime.Now.Ticks - nStartTick) / 10000);
                m_cFrameQueue.Enqueue(new Frame(nFrameBuffer_Array, nXLength, nYLength, bGetSelfFlag, eRawDataType, nTimeStamp));
                Thread.Sleep(ElanDefine.TIME_1MS * 10);

                m_nFrameCount = nFrameIndex + 1;
            }
            #endregion

            GetDataRelatedStep(RelatedStep.Step_ExitTestMode);

            int nTotalCount = m_cFrameQueue.Count;
            m_cFrameMgr.Clear();

            for (int nFrameIndex = 0; nFrameIndex < nTotalCount; nFrameIndex++)
            {
                Frame cFrame = new Frame();

                if (m_cFrameQueue.Dequeue(ElanDefine.TIME_100MS, ref cFrame) == false)
                    continue;

                m_cFrameMgr.Add(cFrame);
            }

            m_eGetDataState = GetDataState.GetDataState_Success;
            return m_eGetDataState;
        }

        private void GetDataRelatedStep(RelatedStep eRelatedStep)
        {
            switch (eRelatedStep)
            {
                case RelatedStep.Step_SetRawDataCount:
                    byte[] byteCommand_Array;

                    if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    {
                        byteCommand_Array = new byte[] 
                        { 
                            0x54, 
                            0xBC, 
                            0x00, 
                            0x02 
                        };
                    }
                    else
                    {
                        byteCommand_Array = new byte[] 
                        { 
                            0x54, 
                            0xBC, 
                            0x00, 
                            0x00 
                        };
                    }

                    SendDevCommand(byteCommand_Array);
                    Thread.Sleep(1000);
                    break;
                case RelatedStep.Step_EnterTestMode:
                    m_cProcessFlow.Set_Test_Mode(true);
                    break;
                case RelatedStep.Step_Reconnect:
                    m_cProcessFlow.Set_Test_Mode(false);

                    m_cProcessFlow.RunElanDeviceReConnect();

                    Thread.Sleep(1000);

                    m_cProcessFlow.Set_Test_Mode(true);
                    break;
                case RelatedStep.Step_ExitTestMode:
                    m_cProcessFlow.Set_Test_Mode(false);
                    break;
                case RelatedStep.Step_TransferTestModeViaHID:
                    if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    {
                        ElanTouch.TransferTestModeViaHID(m_nDeviceIndex);
                        Thread.Sleep(1000);

                        string sSendCommand = "-Send TransferTestModeViaHID Cmd : 0x2D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00";
                        WriteDebugLog(sSendCommand);
                    }

                    break;
                default:
                    break;
            }
        }

        private void SendDevCommand(byte[] byteCommand_Array)
        {
            ElanTouch.SendDevCommand(byteCommand_Array, byteCommand_Array.Length, 1000, m_nDeviceIndex);

            string sSendCommand = "-Send Cmd :";

            for (int nIndex = 0; nIndex < byteCommand_Array.Length; nIndex++)
                sSendCommand += string.Format(" {0}", byteCommand_Array[nIndex].ToString("X2"));

            WriteDebugLog(sSendCommand);
        }

        public string GetErrorMessage()
        {
            return m_sErrorMessage;
        }

        public string GetFrameMessage()
        {
            return m_sFrameMessage;
        }

        public int GetFrameCount()
        {
            return m_nFrameCount;
        }

        public GetDataState GetGetDataState()
        {
            return m_eGetDataState;
        }

        private int GetDataMainStep(ref int[] nDataBuffer_Array, 
                                    ref int nRetryCount, 
                                    ref bool bGetDataFlag, 
                                    string sStageMessage, 
                                    int nRXTotalTrNumber, 
                                    int nTXTotalTrNumber,
                                    byte bTraceType, 
                                    int nTimeout, 
                                    int nFrameIndex, 
                                    int nRetryIndex, 
                                    bool bSkipDataFlag = false, 
                                    bool bGetSelfFlag = true)
        {
            int nResult = 0;

            int nBaseLength = nRXTotalTrNumber + 1;

            if (bGetSelfFlag == true)
                nBaseLength = nRXTotalTrNumber + 2;

            nResult = ElanTouch.GetDV1DArray(nDataBuffer_Array, nRXTotalTrNumber, nTXTotalTrNumber, nBaseLength, bTraceType, nTimeout, m_nDeviceIndex);

            if (nResult == ElanTouch.TP_SUCCESS)
            {
                nRetryCount = 0;
                bGetDataFlag = true;
            }
            else
            {
                string sMessage = string.Format("Get dV Data Error in {0} Frame:{1}(Count={2})[ErrorCode={3}]", sStageMessage, nFrameIndex + 1, nRetryIndex, nResult);

                if (bSkipDataFlag == true)
                    sMessage = string.Format("Get Skip dV Data Error in {0} Frame:{1}(Count={2})[ErrorCode={3}]", sStageMessage, nFrameIndex + 1, nRetryIndex, nResult);

                WriteDebugLog(sMessage);
            }

            return nResult;
        }

        public GetDataState SaveRecordData(string sDataFilePath)
        {
            //int nDataType = ElanDef.RAWDATA_ADC;
            //Assign the self mode is not necessary. Even the data include the self mode.
            int nColorLow = 1;
            int nColorLevel = 1;

            ElanTouch.TraceMode eMode = ElanTouch.GetTraceMode(m_nICType, false);    //ElanTouch.TraceMode.Mutual | ElanTouch.TraceMode.Partial;

            if (m_cFrameMgr.Save(sDataFilePath, m_structTraceInfo, ElanDefine.RAWDATA_dV, m_nICType, nColorLow, nColorLevel, eMode) == false)
            {
                m_sErrorMessage = "Save Record Data Error";
                m_eGetDataState = GetDataState.GetDataState_SaveError;
                return m_eGetDataState;
            }

            m_eGetDataState = GetDataState.GetDataState_Success;
            return m_eGetDataState;
        }

        public GetDataState SaveFrameData(string sDataFilePath, int nRealTraceNumber)
        {
            FileStream fs = new FileStream(sDataFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                for(int nFrameIndex = 0; nFrameIndex < m_nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex + 1));

                    for (int nTXTraceIndex = 1; nTXTraceIndex <= m_nTXTraceNumber; nTXTraceIndex++)
                    {
                        for (int nRXTraceIndex = 1; nRXTraceIndex <= nRealTraceNumber; nRXTraceIndex++)
                        {
                            if (nRXTraceIndex < nRealTraceNumber)
                                sw.Write(string.Format("{0},", m_nFrame_Array[nFrameIndex, nTXTraceIndex, nRXTraceIndex]));
                            else
                                sw.WriteLine(m_nFrame_Array[nFrameIndex, nTXTraceIndex, nRXTraceIndex]);
                        }
                    }

                    if (nFrameIndex < m_nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = "Save Frame Data Error";
                m_eGetDataState = GetDataState.GetDataState_SaveError;
                return m_eGetDataState;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            m_eGetDataState = GetDataState.GetDataState_Success;
            return m_eGetDataState;
        }

        private void ArrayCopy(int[] nSource_Array, int[,,] nDestination_Array, int nDestinationFrameIndex, int nXLength, int nYLength, int nBaseLength)
        {
            for (int nYIndex = 1; nYIndex <= nYLength; nYIndex++)
            {
                for (int nXIndex = 1; nXIndex <= nXLength; nXIndex++)
                {
                    int nIndex = nYIndex * nBaseLength + nXIndex;
                    nDestination_Array[nDestinationFrameIndex, nYIndex, nXIndex] = (short)nSource_Array[nIndex];
                }
            }
        }

        private void ArrayCopy(int[] nSource_Array, int[,] nDestination_Array, int nXLength, int nYLength, int nBaseLength)
        {
            for (int nYIndex = 1; nYIndex <= nYLength; nYIndex++)
            {
                for (int nXIndex = 1; nXIndex <= nXLength; nXIndex++)
                {
                    int nIndex = nYIndex * nBaseLength + nXIndex;
                    nDestination_Array[nYIndex - 1, nXIndex - 1] = (short)nSource_Array[nIndex];
                }
            }
        }

        private void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmMain.OutputMessage(sMessage, bWarning);
        }

        private void WriteDebugLog(string sMessage)
        {
            m_cfrmMain.WriteDebugLog(sMessage);
        }
    }
}
