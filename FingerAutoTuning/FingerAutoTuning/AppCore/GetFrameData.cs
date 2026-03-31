using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
#if DISABLE_V1_0_2_6
#if _USE_9F07_SOCKET
        private bool GetData_9F07(GetDataInfo cGetDataInfo)
        {
            int nFrameIndex = 0;
            //bool bRetryFlag = false;
            //int nFrameValue = 0;
            //int nRetryCount = 0;

            if (cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE ||
                cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_KPKN ||
                cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_OBASE)
                cGetDataInfo.m_nFrameNumber = 1;

            int nRXTotalTraceNumber = cGetDataInfo.m_nRXTraceNumber;
            int nTXTotalTraceNumber = cGetDataInfo.m_nTXTraceNumber;
            //int nRETRYCOUNTTIMES = 10;
            int nTimeout = ParamFingerAutoTuning.m_nGetDataTimeout;
            UserInterfaceDefine.RawDataType eRawDataType = UserInterfaceDefine.RawDataType.Type_ADC;
            //byte byteScanType = AppCoreDefine.m_byteMODE_MUTUAL;

            if (cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE ||
                cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_OBASE)
                eRawDataType = UserInterfaceDefine.RawDataType.Type_Base;
            else if (cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_ADC)
                eRawDataType = UserInterfaceDefine.RawDataType.Type_ADC;
            else
                eRawDataType = UserInterfaceDefine.RawDataType.Type_dV;

            int nFrameCount = cGetDataInfo.m_nFrameNumber;
            int nSkipFrameCount = ParamFingerAutoTuning.m_nSkipFrame;

            if (cGetDataInfo.m_sStageMessage != "")
                cGetDataInfo.m_sStageMessage = string.Format("{0} ", cGetDataInfo.m_sStageMessage);

            string sMessage = string.Format("Get {0} Data[FrameCount={1}]", cGetDataInfo.m_sDataType, cGetDataInfo.m_nFrameNumber);
            OutputMessage(string.Format("-{0}", sMessage));

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                SetDisplayFrameNumber(string.Format("Get {0}", cGetDataInfo.m_sDataType), 0, nFrameCount);
            });

            int nRawDataLength = cGetDataInfo.m_nRXTraceNumber * cGetDataInfo.m_nTXTraceNumber * 2;
            byte[] byteDataBuffer_Array = new byte[nRawDataLength + 1];

            byte byteDataType = 0x00;

            if (eRawDataType == UserInterfaceDefine.RawDataType.Type_dV)
                byteDataType = 0x00;
            else if (eRawDataType == UserInterfaceDefine.RawDataType.Type_ADC)
                byteDataType = 0x01;
            else if (eRawDataType == UserInterfaceDefine.RawDataType.Type_Base)
                byteDataType = 0x02;

            //ElanTouchSwitch.SendDevCommand(new byte[] { 0x55, 0x56, 0x00, byteDataType }, 0, m_bSocketConnectType);
            SetGetDataModeEnable_9F07(true, byteDataType);

            if (cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_ADC || cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_DV)
            {
                while (true)
                {
                    if (m_cfrmParent.m_bExecute == false)
                    {
                        SetGetDataModeEnable_9F07(false);
                        return false;
                    }

                    int nResultFlag = ElanTouchSwitch.ReadDevData(byteDataBuffer_Array, 0, m_bSocketConnectType, 10);

                    if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                        continue;

                    byte nCategory = byteDataBuffer_Array[0];

                    if (nCategory != 0xFC)
                        continue;

                    nFrameIndex++;


                    string sProgressMessage = string.Format("{0}Set : {1}/{2} ({3} Skip Frame : {4}/{5})",
                                                            cGetDataInfo.m_sStageMessage,
                                                            cGetDataInfo.m_nListIndex,
                                                            cGetDataInfo.m_nListCount,
                                                            cGetDataInfo.m_sDataType,
                                                            nFrameIndex,
                                                            nSkipFrameCount);

                    if ((nFrameIndex % 50) == 0 || nFrameIndex >= nSkipFrameCount)
                    {
                        m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                        {
                            m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, sProgressMessage);
                            SetDisplayFrameNumber(string.Format("Skip {0}", cGetDataInfo.m_sDataType), nFrameIndex, nSkipFrameCount);
                        });
                    }

                    m_cDebugLog.WriteLogToBuffer(sProgressMessage);

                    if (nFrameIndex >= nSkipFrameCount)
                        break;
                }
            }

            //long nStartTick = DateTime.Now.Ticks;
            nFrameIndex = 0;

            while (true)
            {
                if (m_cfrmParent.m_bExecute == false)
                {
                    SetGetDataModeEnable_9F07(false);
                    return false;
                }

                int nResultFlag = ElanTouchSwitch.ReadDevData(byteDataBuffer_Array, 0, m_bSocketConnectType, 10);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                    continue;

                byte[] byteRawFrame_Array = new byte[nRawDataLength];
                int[,] n2DFrame_Array = new int[cGetDataInfo.m_nTXTraceNumber, cGetDataInfo.m_nRXTraceNumber];
                byte nCategory = byteDataBuffer_Array[0];
                long nTimeStamp = ((DateTime.Now.Ticks - 0) / (10 * 1000));
                int nCount = 0;

                if (nCategory != 0xFC)
                    continue;

                Array.Clear(byteRawFrame_Array, 0, byteRawFrame_Array.Length);
                Array.Clear(n2DFrame_Array, 0, n2DFrame_Array.Length);
                Array.Copy(byteDataBuffer_Array, 1, byteRawFrame_Array, 0, nRawDataLength);

                //Put the new frame to queue
                for (int nY = 1; nY <= cGetDataInfo.m_nTXTraceNumber; nY++)
                {
                    for (int nX = 1; nX <= cGetDataInfo.m_nRXTraceNumber; nX++)
                    {
                        cGetDataInfo.m_nFrameData_Array[nFrameIndex, nY, nX] = (short)(byteRawFrame_Array[nCount * 2] | (byteRawFrame_Array[nCount * 2 + 1] << 8));
                        nCount++;
                    }
                }

                nFrameIndex++;

                string sProgressMessage = string.Format("{0}Set : {1}/{2} ({3} Get Frame : {4}/{5})",
                                                        cGetDataInfo.m_sStageMessage,
                                                        cGetDataInfo.m_nListIndex,
                                                        cGetDataInfo.m_nListCount,
                                                        cGetDataInfo.m_sDataType,
                                                        nFrameIndex,
                                                        nFrameCount);

                if ((nFrameIndex % 50) == 0 || nFrameIndex >= nFrameCount)
                {
                    m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                    {
                        m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, sProgressMessage);
                        SetDisplayFrameNumber(string.Format("Get {0}", cGetDataInfo.m_sDataType), nFrameIndex, nFrameCount);
                    });
                }

                m_cDebugLog.WriteLogToBuffer(sProgressMessage);

                if (nFrameIndex >= nFrameCount)
                {
                    SetGetDataModeEnable_9F07(false);
                    break;
                }
            }

            return true;
        }
#endif

        private bool GetData(GetDataInfo cGetDataInfo)
        {
            bool bRetryFlag = false;
            int nFrameValue = 0;
            int nRetryCount = 0;
            int nFrameCount = cGetDataInfo.m_nFrameNumber;

            if (cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE ||
                cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_KPKN ||
                cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_OBASE)
                cGetDataInfo.m_nFrameNumber = 1;

            int nRXTotalTraceNumber = cGetDataInfo.m_nRXTraceNumber;
            int nTXTotalTraceNumber = cGetDataInfo.m_nTXTraceNumber;
            int nRETRYCOUNTTIMES = 10;
            int nTimeout = ParamFingerAutoTuning.m_nGetDataTimeout;



            UserInterfaceDefine.RawDataType eRawDataType = UserInterfaceDefine.RawDataType.Type_ADC;
            byte byteScanType = AppCoreDefine.m_byteMODE_MUTUAL;

            SetGetDataParameter(ref nRETRYCOUNTTIMES, ref nTimeout, ref eRawDataType, ref byteScanType, ref nRXTotalTraceNumber, ref nTXTotalTraceNumber,
                                cGetDataInfo.m_sDataType, cGetDataInfo.m_nRXTraceNumber, cGetDataInfo.m_nTXTraceNumber, cGetDataInfo.m_bGetSelf);

            if (cGetDataInfo.m_sStageMessage != "")
                cGetDataInfo.m_sStageMessage = string.Format("{0} ", cGetDataInfo.m_sStageMessage);

            int[,] nFrameBuffer_Array = null;
            long nStartTime = 0;

            string sRealDataType = cGetDataInfo.m_sDataType;

            if (cGetDataInfo.m_bRawADCSweep == true)
            {
                sRealDataType = string.Format("{0}(Noise)", cGetDataInfo.m_sDataType);

                if (m_eICGenerationType == ICGenerationType.Gen7 && ParamFingerAutoTuning.m_nRawADCSGen7EnableHWTXN == 0)
                    sRealDataType = cGetDataInfo.m_sDataType;
                else if (m_eICGenerationType == ICGenerationType.Gen6)
                    sRealDataType = cGetDataInfo.m_sDataType;
            }

            string sMessage = string.Format("Get {0} Data[FrameCount={1}]", sRealDataType, cGetDataInfo.m_nFrameNumber);

            if (cGetDataInfo.m_bGetSelf == true && cGetDataInfo.m_bSetKParameter == true)
                sMessage = string.Format("{0} (NCP={1} NCN={2})", sMessage, m_nSelfNCPValue, m_nSelfNCNValue);

            OutputMessage(string.Format("-{0}", sMessage));

            //Declare a temp buffer get 1-d array from c++ function
            int[] nDataBuffer_Array = new int[(cGetDataInfo.m_nTXTraceNumber + 1) * (cGetDataInfo.m_nRXTraceNumber + 1)];

            if (cGetDataInfo.m_bGetSelf == true)
                nDataBuffer_Array = new int[(cGetDataInfo.m_nTXTraceNumber + 2) * (cGetDataInfo.m_nRXTraceNumber + 2)];

            for (int nRetryIndex = 0; nRetryIndex <= nRETRYCOUNTTIMES; nRetryIndex++)
            {
                if (m_cfrmParent.m_bExecute == false)
                    return false;

                bRetryFlag = false;

                if (nRetryIndex > 0)
                {
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ReConnect);

                    if (m_eICGenerationType == ICGenerationType.Gen8)
                    {
                        if (cGetDataInfo.m_bRawADCSweep == true)
                        {
                            if (SetAndGetFWParameter(cGetDataInfo.m_cFlowStep, null, cGetDataInfo.m_cRawADCSweepItem) == false)
                                return false;
                        }
                        else
                        {
                            if (SetAndGetFWParameter(cGetDataInfo.m_cFlowStep, cGetDataInfo.m_cFrequencyItem) == false)
                                return false;
                        }
                    }
                }

                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    if ((cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE ||
                         cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_OBASE) &&
                        cGetDataInfo.m_bGetBaseDelay == true &&
                        ParamFingerAutoTuning.m_nGen8GetFirstBaseDelay == 1)
                    {
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                        Thread.Sleep(ParamFingerAutoTuning.m_nGen8GetFirstBaseDelayTime);
                    }
                }

                if (m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nGen8GetDataDelayTime > 0)
                    Thread.Sleep(ParamFingerAutoTuning.m_nGen8GetDataDelayTime);

                if (m_eICGenerationType != ICGenerationType.Gen8)
                {
                    if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterKPKNCommand, cGetDataInfo.m_sDataType);

                    if (cGetDataInfo.m_bRawADCSweep == false)
                    {
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data);

                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);
                    }

                    if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                        Thread.Sleep(2000);
                    else
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_TransferTestModeViaHID);
                }
                else
                {
                    if (ParamFingerAutoTuning.m_nGen8SetGetDataInfoType == 1)
                    {
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterKPKNCommand, cGetDataInfo.m_sDataType);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data, null, cGetDataInfo.m_bGetSelf);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data, null, cGetDataInfo.m_bGetSelf);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_TransferTestModeViaHID);
                        Thread.Sleep(30);
                    }
                    else
                    {
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);
                        Thread.Sleep(30);
                    }
                }

                /*
                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_SSHSOCKETSERVER)
                    Thread.Sleep(1000);
                */

                #region Skip n Frame
                if (cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_DV ||
                    cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_ADC ||
                    (m_eICGenerationType == ICGenerationType.Gen8 &&
                     cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                     ParamFingerAutoTuning.m_nGen8GetBaseUseSkipFrame == 1) ||
                     (m_eICGenerationType != ICGenerationType.Gen8 &&
                     cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                     ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseUseSkipFrame == 1))
                {
                    nFrameCount = ParamFingerAutoTuning.m_nSkipFrame;

                    if (cGetDataInfo.m_bUseNewMethod == true)
                        nFrameCount = 1;

                    string sDataType = cGetDataInfo.m_sDataType;

                    if ((m_eICGenerationType == ICGenerationType.Gen8 &&
                        cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                        ParamFingerAutoTuning.m_nGen8GetBaseUseSkipFrame == 1) ||
                        (m_eICGenerationType != ICGenerationType.Gen8 &&
                        cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                        ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseUseSkipFrame == 1))
                        sDataType = MainConstantParameter.m_sDATATYPE_ADC;

                    m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                    {
                        SetDisplayFrameNumber(string.Format("Skip {0}", sDataType), 0, nFrameCount);
                    });

                    for (int nFrameIndex = 0; nFrameIndex < nFrameCount; nFrameIndex++)
                    {
                        if (m_cfrmParent.m_bExecute == false)
                            return false;

                        bool bGetData = false;
                        bool bSetUseNewMethodSuccess = true;

                        for (int nSubRetryIndex = 0; nSubRetryIndex <= 3; nSubRetryIndex++)
                        {
                            if (m_cfrmParent.m_bExecute == false)
                                return false;

                            if (cGetDataInfo.m_bUseNewMethod == true)
                            {
                                if (m_eICGenerationType == ICGenerationType.Gen8)
                                    bSetUseNewMethodSuccess = SetUseNewMethod_Gen8(ParamFingerAutoTuning.m_nSkipFrame, 3000);
                                else
                                    bSetUseNewMethodSuccess = SetUseNewMethod(ParamFingerAutoTuning.m_nSkipFrame, 3000);

                                if (bSetUseNewMethodSuccess == false)
                                    break;
                            }

                            Array.Clear(nDataBuffer_Array, 0, nDataBuffer_Array.Length);
                            int nResult = GetDataMainStep(ref nDataBuffer_Array, ref nRetryCount, ref bGetData, cGetDataInfo, sDataType,
                                                          cGetDataInfo.m_nRXTraceNumber, cGetDataInfo.m_nTXTraceNumber, byteScanType,
                                                          nTimeout, cGetDataInfo.m_nListIndex, nFrameIndex, nRetryIndex, true);

                            //if (nResult == ElanTouch.TP_SUCCESS)
                            if (ElanTouchSwitch.CheckTPState(nResult, m_bSocketConnectType) == true)
                            {
                                bGetData = true;
                                break;
                            }

                            Thread.Sleep(10);
                        }

                        if (bGetData == false)
                        {
                            nRetryCount++;

                            if (nRetryCount <= nRETRYCOUNTTIMES)
                            {
                                bRetryFlag = true;
                                break;
                            }
                        }

                        string sProgressMessage = string.Format("{0}Set : {1}/{2} ({3} Skip Frame : {4}/{5})",
                                                                cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex,
                                                                cGetDataInfo.m_nListCount, sDataType,
                                                                nFrameIndex + 1, nFrameCount);

                        if (cGetDataInfo.m_bGetSelf == true && cGetDataInfo.m_bSetKParameter == true)
                            sProgressMessage = string.Format("{0} (NCP={1} NCN={2})", sProgressMessage, m_nSelfNCPValue, m_nSelfNCNValue);

                        m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                        {
                            m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, sProgressMessage);
                            SetDisplayFrameNumber(string.Format("Skip {0}", sDataType), nFrameIndex + 1, nFrameCount);
                        });

                        m_cDebugLog.WriteLogToBuffer(sProgressMessage);

                        if (bSetUseNewMethodSuccess == false || bGetData == false)
                        {
                            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);

                            if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, sDataType);

                            if (bSetUseNewMethodSuccess == false)
                                m_sErrorMessage = string.Format("Set UseNewMethod Fail in {0}Set:{1}", cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex);
                            else if (bGetData == false)
                                m_sErrorMessage = string.Format("Get {0} Data Fail in {1}Set:{2}", sDataType, cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex);

                            return false;
                        }

                        Thread.Sleep(10);
                    }
                }
                #endregion

                if (bRetryFlag == true)
                    continue;

                if (m_eICGenerationType == ICGenerationType.Gen8 &&
                    cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                    ParamFingerAutoTuning.m_nGen8GetBaseUseSkipFrame == 1)
                {
                    if (ParamFingerAutoTuning.m_nGen8SetGetDataInfoType == 1)
                    {
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, MainConstantParameter.m_sDATATYPE_ADC);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data, null, cGetDataInfo.m_bGetSelf);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data, null, cGetDataInfo.m_bGetSelf);
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_TransferTestModeViaHID);
                        Thread.Sleep(30);
                    }
                    else
                    {
                        if (cGetDataInfo.m_bRawADCSweep == false)
                        {
                            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);

                            if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, MainConstantParameter.m_sDATATYPE_ADC);

                            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);

                            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                                Thread.Sleep(2000);
                            else
                                Thread.Sleep(30);
                        }
                    }
                }

                if (m_eICGenerationType != ICGenerationType.Gen8 &&
                    cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                    ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseUseSkipFrame == 1)
                {
                    if (cGetDataInfo.m_bRawADCSweep == false)
                    {
                        if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                        {
                            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, MainConstantParameter.m_sDATATYPE_ADC);
                        }

                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);

                        if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                            Thread.Sleep(2000);
                        else
                            Thread.Sleep(30);
                    }
                }

                #region Get n Frames
                if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                {
                    m_bqcFrameQueue.Clear();

                    //Get the start tick. Use to compute the timestamp.
                    nStartTime = DateTime.Now.Ticks;

                    Thread.Sleep(ElanDefine.TIME_100MS);
                }

                nFrameCount = cGetDataInfo.m_nFrameNumber;

                if (cGetDataInfo.m_bUseNewMethod == true)
                    nFrameCount = 1;

                m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                {
                    SetDisplayFrameNumber(string.Format("Get {0}", cGetDataInfo.m_sDataType), 0, nFrameCount);
                });

                for (int nFrameIndex = nFrameValue; nFrameIndex < nFrameCount; nFrameIndex++)
                {
                    if (m_cfrmParent.m_bExecute == false)
                        return false;

                    bool bGetData = false;
                    bool bSetUseNewMethodSuccess = true;

                    for (int nSubRetryIndex = 0; nSubRetryIndex <= 3; nSubRetryIndex++)
                    {
                        if (m_cfrmParent.m_bExecute == false)
                            return false;

                        if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                        {
                            if (cGetDataInfo.m_bGetSelf == true)
                                nFrameBuffer_Array = new int[cGetDataInfo.m_nTXTraceNumber + 2, cGetDataInfo.m_nRXTraceNumber + 2];
                            else
                                nFrameBuffer_Array = new int[cGetDataInfo.m_nTXTraceNumber + 1, cGetDataInfo.m_nRXTraceNumber + 1];

                            Array.Clear(nFrameBuffer_Array, 0, nFrameBuffer_Array.Length);
                        }

                        if (cGetDataInfo.m_bUseNewMethod == true)
                        {
                            if (m_eICGenerationType == ICGenerationType.Gen8)
                                bSetUseNewMethodSuccess = SetUseNewMethod_Gen8(cGetDataInfo.m_nFrameNumber, 3000);
                            else
                                bSetUseNewMethodSuccess = SetUseNewMethod(cGetDataInfo.m_nFrameNumber, 3000);

                            if (bSetUseNewMethodSuccess == false)
                                break;
                        }

                        Array.Clear(nDataBuffer_Array, 0, nDataBuffer_Array.Length);
                        int nResult = GetDataMainStep(ref nDataBuffer_Array, ref nRetryCount, ref bGetData, cGetDataInfo, cGetDataInfo.m_sDataType,
                                                      nRXTotalTraceNumber, nTXTotalTraceNumber, byteScanType, nTimeout, cGetDataInfo.m_nListIndex,
                                                      nFrameIndex, nRetryIndex);

                        //if (nResult == ElanTouch.TP_SUCCESS)
                        if (ElanTouchSwitch.CheckTPState(nResult, m_bSocketConnectType) == true)
                        {
                            bGetData = true;
                            break;
                        }

                        Thread.Sleep(10);
                    }

                    if (bGetData == false)
                    {
                        nRetryCount++;

                        if (nRetryCount <= nRETRYCOUNTTIMES)
                        {
                            bRetryFlag = true;
                            nFrameValue = nFrameIndex;
                            break;
                        }
                    }

                    string sProgressMessage = string.Format("{0}Set : {1}/{2} ({3} Get Frame : {4}/{5})", cGetDataInfo.m_sStageMessage,
                                                        cGetDataInfo.m_nListIndex, cGetDataInfo.m_nListCount, cGetDataInfo.m_sDataType,
                                                        nFrameIndex + 1, nFrameCount);

                    if (cGetDataInfo.m_bGetSelf == true && cGetDataInfo.m_bSetKParameter == true)
                        sProgressMessage = string.Format("{0} (NCP={1} NCN={2})", sProgressMessage, m_nSelfNCPValue, m_nSelfNCNValue);

                    m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                    {
                        m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, sProgressMessage);
                        SetDisplayFrameNumber(string.Format("Get {0}", cGetDataInfo.m_sDataType), nFrameIndex + 1, nFrameCount);
                    });

                    m_cDebugLog.WriteLogToBuffer(sProgressMessage);

                    if (bSetUseNewMethodSuccess == false || bGetData == false)
                    {
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);

                        if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, cGetDataInfo.m_sDataType);

                        if (bSetUseNewMethodSuccess == false)
                            m_sErrorMessage = string.Format("Set UseNewMethod Fail in {0}Set:{1}", cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex);
                        else if (bGetData == false)
                            m_sErrorMessage = string.Format("Get {0} Data Fail in {1}Set:{2}", cGetDataInfo.m_sDataType, cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex);

                        return false;
                    }

                    if (cGetDataInfo.m_bGetSelf == true)
                        CopyArray(nDataBuffer_Array, cGetDataInfo.m_nFrameData_Array, nFrameIndex, cGetDataInfo.m_nRXTraceNumber + 1, cGetDataInfo.m_nTXTraceNumber + 1, cGetDataInfo.m_nRXTraceNumber + 2);
                    else
                        CopyArray(nDataBuffer_Array, cGetDataInfo.m_nFrameData_Array, nFrameIndex, cGetDataInfo.m_nRXTraceNumber, cGetDataInfo.m_nTXTraceNumber, cGetDataInfo.m_nRXTraceNumber + 1);

                    if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                    {
                        int nXLength = cGetDataInfo.m_nRXTraceNumber;
                        int nYLength = cGetDataInfo.m_nTXTraceNumber;

                        if (cGetDataInfo.m_bGetSelf == true)
                        {
                            nXLength = cGetDataInfo.m_nRXTraceNumber + 1;
                            nYLength = cGetDataInfo.m_nTXTraceNumber + 1;
                        }

                        CopyArray(nDataBuffer_Array, nFrameBuffer_Array, nXLength, nYLength, nXLength + 1);

                        //Get the timestamp
                        long nTimeInterval = ((DateTime.Now.Ticks - nStartTime) / 10000);
                        m_bqcFrameQueue.Enqueue(new Frame(nFrameBuffer_Array, nXLength, nYLength, cGetDataInfo.m_bGetSelf, eRawDataType, nTimeInterval));

                        Thread.Sleep(ElanDefine.TIME_1MS * 10);
                    }
                    else
                        Thread.Sleep(20);
                }
                #endregion

                if (bRetryFlag == false)
                    break;
            }

            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);

            if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, cGetDataInfo.m_sDataType);

            if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
            {
                int nTotalCount = m_bqcFrameQueue.Count;
                m_cFrameMgr.Clear();

                for (int i = 0; i < nTotalCount; i++)
                {
                    Frame cFrame = new Frame();

                    if (m_bqcFrameQueue.Dequeue(ElanDefine.TIME_100MS, ref cFrame) == false)
                        continue;

                    m_cFrameMgr.Add(cFrame);
                }
            }

            return true;
        }

        private void SetGetDataParameter(ref int nRetryCount, ref int nTimeout, ref UserInterfaceDefine.RawDataType eRawDataType, ref byte byteScanType,
                                         ref int nRXTotalTraceNumber, ref int nTXTotalTraceNumber, string sDataType, int nRXTraceNumber, int nTXTraceNumber,
                                         bool bGetSelf)
        {
            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
                nRetryCount = ParamFingerAutoTuning.m_nChromeGetDataRetryCount;
            else
                nRetryCount = ParamFingerAutoTuning.m_nGetDataRetryCount;

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
                nTimeout = ParamFingerAutoTuning.m_nChromeGetDataTimeout;
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                nTimeout = ParamFingerAutoTuning.m_nSSHSocketServerGetDataTimeout;

            if (sDataType == MainConstantParameter.m_sDATATYPE_BASE ||
                sDataType == MainConstantParameter.m_sDATATYPE_OBASE)
                eRawDataType = UserInterfaceDefine.RawDataType.Type_Base;
            else if (sDataType == MainConstantParameter.m_sDATATYPE_ADC)
                eRawDataType = UserInterfaceDefine.RawDataType.Type_ADC;
            else
                eRawDataType = UserInterfaceDefine.RawDataType.Type_dV;

            if (bGetSelf == true)
                byteScanType = AppCoreDefine.m_byteMODE_MUTUALSELF;
            else
                byteScanType = AppCoreDefine.m_byteMODE_MUTUAL;
        }

        private void GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep eGetDataRelatedStep, string sDataType = null, bool bSelfModeFlag = false)
        {
            byte[] byteCommand_Array;

            switch (eGetDataRelatedStep)
            {
                case AppCoreDefine.GetDataRelatedStep.Step_EnterKPKNCommand:
                    if (sDataType == MainConstantParameter.m_sDATATYPE_KPKN)
                    {
                        OutputMessage("-Set Enter KPKN Command(0x54, 0x27, 0x01, 0x01)");
                        byteCommand_Array = new byte[] { 0x54, 0x27, 0x01, 0x01 };
                        SendDevCommand(byteCommand_Array);
                        Thread.Sleep(m_nNormalDelayTime);
                    }
                    break;
                case AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data:
                    if (m_eICGenerationType == ICGenerationType.Gen8)
                    {
                        if (ParamFingerAutoTuning.m_nGen8ReadBulkRAMDataValue >= 0)
                        {
                            OutputMessage(string.Format("-Set Read Bulk RAM Data Value(0x54, 0xBC, 0x00, 0x{0})", ParamFingerAutoTuning.m_nGen8ReadBulkRAMDataValue.ToString("X2")));
                            //byteCommand_Array = new byte[] { 0x54, 0xBC, 0x00, 0x02 };
                            byteCommand_Array = new byte[] { 0x54, 0xBC, 0x00, (byte)ParamFingerAutoTuning.m_nGen8ReadBulkRAMDataValue };

                            SendDevCommand(byteCommand_Array);
                            Thread.Sleep(m_nNormalDelayTime);
                            Thread.Sleep(ParamFingerAutoTuning.m_nReadBulkRAMDataValueExtraDelayTime);
                        }
                        else
                        {
                            byte byteComboMode = 0x01;

                            if (bSelfModeFlag == true)
                                byteComboMode = 0x02;

                            OutputMessage(string.Format("-Set Read Bulk RAM Data Value(0x54, 0xBC, 0x00, 0x{0})", byteComboMode.ToString("X2")));
                            byteCommand_Array = new byte[] { 0x54, 0xBC, 0x00, byteComboMode };

                            SendDevCommand(byteCommand_Array);
                            Thread.Sleep(m_nNormalDelayTime);
                            Thread.Sleep(ParamFingerAutoTuning.m_nReadBulkRAMDataValueExtraDelayTime);
                        }
                    }
                    else
                    {
                        OutputMessage("-Set Read Bulk RAM Data Value(0x54, 0xBC, 0x00, 0x00)");
                        byteCommand_Array = new byte[] { 0x54, 0xBC, 0x00, 0x00 };

                        SendDevCommand(byteCommand_Array);
                        Thread.Sleep(m_nNormalDelayTime);
                        Thread.Sleep(ParamFingerAutoTuning.m_nReadBulkRAMDataValueExtraDelayTime);
                    }
                    break;
                case AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode:
                    SetTestModeEnable(true);
                    break;
                case AppCoreDefine.GetDataRelatedStep.Step_ReConnect:
                    SetTestModeEnable(false);

                    if (m_eICGenerationType == ICGenerationType.Gen8)
                    {
                        SetReset();
                        Thread.Sleep(ParamFingerAutoTuning.m_nGen8ReconnectDelayTime);
                    }

                    ReconnectToTP();

                    Thread.Sleep(m_nNormalDelayTime);

                    SetPenFunctionEnable_8F18(false);

                    SetTestModeEnable(true);
                    break;
                case AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand:
                    if (sDataType == MainConstantParameter.m_sDATATYPE_KPKN)
                    {
                        OutputMessage("-Set Exit KPKN Command(0x54, 0x27, 0x00, 0x01)");
                        byteCommand_Array = new byte[] { 0x54, 0x27, 0x00, 0x01 };
                        SendDevCommand(byteCommand_Array);
                        Thread.Sleep(m_nNormalDelayTime);
                    }
                    break;
                case AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode:
                    SetTestModeEnable(false);
                    break;
                case AppCoreDefine.GetDataRelatedStep.Step_TransferTestModeViaHID:
                    if (m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nGen8SetTransferTestModeViaHID == 1)
                    {
                        OutputMessage("-Send TransferTestModeViaHID Command : 0x2D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00");
                        ElanTouchSwitch.TransferTestModeViaHID(m_nDeviceIndex, m_bSocketConnectType);
                        Thread.Sleep(m_nNormalDelayTime);

                        //string sSendCommand = "-Send TransferTestModeViaHID Command : 0x2D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00";
                        //m_cDebugLog.WriteLogToBuffer(sSendCommand);
                        Thread.Sleep(30);
                    }
                    break;
                default:
                    break;
            }
        }

        private int GetDataMainStep(ref int[] nDataBuffer_Array, ref int nRetryCount, ref bool bGetData, GetDataInfo cGetDataInfo, string sDataType, int nRXTotalTraceNumber,
                                    int nTXTotalTraceNumber, byte bTraceType, int nTimeout, int nListIndex, int nFrameIndex, int nRetryIndex, bool bSkipData = false)
        {
            int nResult = 0;

            if (cGetDataInfo.m_bUseNewMethod == true)
            {
                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    nResult = ElanTouchSwitch.GetFrameData(
                        ElanTouchSwitch.m_nDATA_ADC, ref nDataBuffer_Array, nRXTotalTraceNumber, cGetDataInfo.m_nTXTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                        m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                    );
                }
                else
                {
                    //nResult = ElanTouch.GetBase1DArray(nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, nRXTotalTraceNumber + 1, bTraceType, nTimeout, m_nDeviceIndex);
                    nResult = ElanTouchSwitch.GetFrameData(
                        ElanTouchSwitch.m_nDATA_BASE, ref nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                        m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                    );
                }
            }
            else
            {
                switch (sDataType)
                {
                    case MainConstantParameter.m_sDATATYPE_DV:
                    case MainConstantParameter.m_sDATATYPE_RawData:
                        //nResult = ElanTouch.GetDV1DArray(nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, nRXTotalTraceNumber + 1, bTraceType, nTimeout, m_nDeviceIndex);
                        nResult = ElanTouchSwitch.GetFrameData(
                            ElanTouchSwitch.m_nDATA_DV, ref nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                            m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                        );
                        break;
                    case MainConstantParameter.m_sDATATYPE_ADC:
                        if (cGetDataInfo.m_bRawADCSweep == true)
                        {
                            if (m_eICGenerationType == ICGenerationType.Gen8)
                            {
                                nResult = ElanTouchSwitch.GetFrameData(
                                    ElanTouchSwitch.m_nDATA_Noise, ref nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                                    m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                                );
                            }
                            else if (m_eICGenerationType == ICGenerationType.Gen7)
                            {
                                if (ParamFingerAutoTuning.m_nRawADCSGen7EnableHWTXN == 0)
                                {
                                    nResult = ElanTouchSwitch.GetFrameData(
                                        ElanTouchSwitch.m_nDATA_ADC, ref nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                                        m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                                    );
                                }
                                else
                                {
                                    nResult = ElanTouchSwitch.GetFrameData(
                                        ElanTouchSwitch.m_nDATA_Noise, ref nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                                        m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                                    );
                                }
                            }
                            else if (m_eICGenerationType == ICGenerationType.Gen6)
                            {
                                nResult = ElanTouchSwitch.GetFrameData(
                                    ElanTouchSwitch.m_nDATA_ADC, ref nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                                    m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                                );
                            }
                        }
                        else
                        {
                            //nResult = ElanTouch.GetADC1DArray(nDataBuffer_Array, nRXTotalTraceNumber, cGetDataInfo.m_nTXTraceNumber, nRXTotalTraceNumber + 1, bTraceType, nTimeout, m_nDeviceIndex);
                            nResult = ElanTouchSwitch.GetFrameData(
                                ElanTouchSwitch.m_nDATA_ADC, ref nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                                m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                            );
                        }
                        break;
                    case MainConstantParameter.m_sDATATYPE_BASE:
                    case MainConstantParameter.m_sDATATYPE_OBASE:
                        //nResult = ElanTouch.GetBase1DArray(nDataBuffer_Array, nRXTotalTraceNumber, cGetDataInfo.m_nTXTraceNumber, nRXTotalTraceNumber + 1, bTraceType, nTimeout, m_nDeviceIndex);
                        nResult = ElanTouchSwitch.GetFrameData(
                            ElanTouchSwitch.m_nDATA_BASE, ref nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                            m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                        );
                        break;
                    default:
                        //nResult = ElanTouch.GetDV1DArray(nDataBuffer_Array, nRXTotalTraceNumber, cGetDataInfo.m_nTXTraceNumber, nRXTotalTraceNumber + 1, bTraceType, nTimeout, m_nDeviceIndex);
                        nResult = ElanTouchSwitch.GetFrameData(
                            ElanTouchSwitch.m_nDATA_DV, ref nDataBuffer_Array, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                            m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                        );
                        break;
                }
            }

            //if (nResult == ElanTouch.TP_SUCCESS)
            if (ElanTouchSwitch.CheckTPState(nResult, m_bSocketConnectType) == true)
            {
                nRetryCount = 0;
                bGetData = true;
            }
            else
            {
                string sMessage = string.Format("Get {0} Data Error in {1}Frequency Set:{2} in Frame:{3}(Count={4})[ErrorCode=0x{5}]", sDataType, cGetDataInfo.m_sStageMessage,
                                                nListIndex, nFrameIndex + 1, nRetryIndex, nResult.ToString("X4"));

                if (bSkipData == true)
                {
                    sMessage = string.Format("Get Skip {0} Data Error in {1}Frequency Set:{2} in Frame:{3}(Count={4})[ErrorCode=0x{5}]", sDataType, cGetDataInfo.m_sStageMessage,
                                             nListIndex, nFrameIndex + 1, nRetryIndex, nResult.ToString("X4"));
                }

                OutputMessage(string.Format("-{0}", sMessage));

                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER)
                {
                    SendTestCommand();
                    Thread.Sleep(ParamFingerAutoTuning.m_nChromeGetDataDelayTime);
                }
            }

            return nResult;
        }
#else
        #region 主要取得資料方法

        /// <summary>
        /// 執行資料擷取的主要流程,根據不同的測試步驟取得並儲存各種類型的感測器資料(ADC、BASE、OBASE、DV、Raw P/N等),支援多世代IC(Gen6/Gen7/Gen9)
        /// </summary>
        /// <param name="cFlowStep">當前執行的流程步驟,用於判斷要執行的測試類型</param>
        /// <param name="nBrightness">螢幕亮度設定值</param>
        /// <param name="cFrequencyItem">頻率項目設定資訊</param>
        /// <param name="nRXTraceNumber">接收端(RX)追蹤線數量</param>
        /// <param name="nTXTraceNumber">發送端(TX)追蹤線數量</param>
        /// <param name="nExecuteIndex">當前執行的索引編號(從0開始)</param>
        /// <param name="nTotalCount">總執行次數</param>
        /// <param name="eRecordState">記錄狀態,預設為NORMAL</param>
        /// <param name="sStageMessage">階段訊息文字,預設為空字串</param>
        /// <param name="bGetFirstDataFlag">是否為取得第一筆資料的旗標,預設為false</param>
        /// <param name="cRawADCSweepItem">Raw ADC掃描項目參數,僅在Raw_ADC_Sweep步驟使用,預設為null</param>
        /// <returns>執行成功回傳true;若資料擷取或儲存失敗則執行螢幕重置流程並回傳false</returns>
        private bool GetDataMainFlow(
            frmMain.FlowStep cFlowStep, int nBrightness, FrequencyItem cFrequencyItem, int nRXTraceNumber, int nTXTraceNumber,
            int nExecuteIndex, int nTotalCount, AppCoreDefine.RecordState eRecordState = AppCoreDefine.RecordState.NORMAL, string sStageMessage = "",
            bool bGetFirstDataFlag = false, RawADCSweepItem cRawADCSweepItem = null)
        {
            OutputMessage("-Start Get Data");

            SaveDataInfo cSaveDataInfo = null;

            int[, ,] nFrame_Array = null;
            int[, ,] nBASEFrame_Array = null;
            int[, ,] nADCFrame_Array = null;

            #region Get ADC Data(Raw ADC Sweep)
            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                //Get ADC Data
                if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_ADC))
                {
                    string sDataType = string.Format("{0}(Noise)", MainConstantParameter.m_sDATATYPE_ADC);

                    if (m_eICGenerationType == ICGenerationType.Gen7)
                    {
                        if (ParamFingerAutoTuning.m_nRawADCSGen7EnableHWTXN == 0)
                            sDataType = MainConstantParameter.m_sDATATYPE_ADC;
                    }
                    else if (m_eICGenerationType == ICGenerationType.Gen6)
                    {
                        sDataType = MainConstantParameter.m_sDATATYPE_ADC;
                    }

                    OutputMessage(string.Format("-Get {0} Data", sDataType));

                    bool bUseNewMethod = false;

                    if (m_eICGenerationType != ICGenerationType.Gen9)
                    {
                        //Get ADC Frame
                        //Allocate memory to store frame trace
                        nFrame_Array = new int[cRawADCSweepItem.m_nADCTestFrame, nTXTraceNumber + 1, nRXTraceNumber + 1];
                    }
#if _USE_9F07_SOCKET
                    else
                    {
                        //Get ADC Frame
                        //Allocate memory to store frame trace
                        nFrame_Array = new int[cRawADCSweepItem.m_nADCTestFrame, nTXTraceNumber + 1, nRXTraceNumber + 1];
                    }
#endif

                    Array.Clear(nFrame_Array, 0, nFrame_Array.Length);

                    GetDataInfo cGetDataInfo = new GetDataInfo();
                    cGetDataInfo.m_cFlowStep = cFlowStep;
                    cGetDataInfo.m_cRawADCSweepItem = cRawADCSweepItem;
                    cGetDataInfo.m_sDataType = MainConstantParameter.m_sDATATYPE_ADC;
                    cGetDataInfo.m_nFrameData_Array = nFrame_Array;
                    cGetDataInfo.m_nFrameNumber = cRawADCSweepItem.m_nADCTestFrame;
                    cGetDataInfo.m_nRXTraceNumber = nRXTraceNumber;
                    cGetDataInfo.m_nTXTraceNumber = nTXTraceNumber;
                    cGetDataInfo.m_nListIndex = nExecuteIndex + 1;
                    cGetDataInfo.m_nListCount = nTotalCount;
                    cGetDataInfo.m_sStageMessage = sStageMessage;
                    cGetDataInfo.m_bUseNewMethod = bUseNewMethod;
                    cGetDataInfo.m_bRawADCSweep = true;

                    if (m_eICGenerationType != ICGenerationType.Gen9)
                    {
                        if (GetData(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#if _USE_9F07_SOCKET
                    else
                    {
                        if (GetData_9F07(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#endif

                    cSaveDataInfo = SetSaveDataInfo(cFlowStep, cRawADCSweepItem, MainConstantParameter.m_sDATATYPE_ADC, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber, bUseNewMethod);

                    if (SaveData(MainConstantParameter.m_sDATATYPE_ADC, cSaveDataInfo, nFrame_Array, true, true) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }

                    if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                    {
                        int nRawDataType = ElanDefine.RAWDATA_NOISE;

                        if (m_eICGenerationType == ICGenerationType.Gen7)
                        {
                            if (ParamFingerAutoTuning.m_nRawADCSGen7EnableHWTXN == 0)
                                nRawDataType = ElanDefine.RAWDATA_ADC;
                        }
                        else if (m_eICGenerationType == ICGenerationType.Gen6)
                        {
                            nRawDataType = ElanDefine.RAWDATA_ADC;
                        }

                        if (SaveNormalRawDataType(cRawADCSweepItem, MainConstantParameter.m_sDATATYPE_RAW_ADC, nRawDataType, nExecuteIndex + 1, m_sLogDirectoryPath) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }

                    nADCFrame_Array = nFrame_Array;
                }

                return true;
            }
            #endregion

            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep && ParamFingerAutoTuning.m_nSelfFSGetDataType == 1)
            {
                if (RunGetReportData(cSaveDataInfo, cFlowStep, nBrightness, cFrequencyItem, nRXTraceNumber, nTXTraceNumber, nExecuteIndex) == false)
                    return false;
            }

            bool bGetSelfFlag = (cFlowStep.m_eStep == MainStep.Self_FrequencySweep || cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep) ? true : false;

            #region Get OBASE & BASE Data(UseBaseAtFirst)
            if (SetGetBaseType(cFlowStep) == MainConstantParameter.m_nGetBaseType_UseBaseAtFirst)
            {
                //Get OBASE & BASE Data
                if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASE) || m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASE))
                {
                    OutputMessage("-Get Base Data At First");

                    string sRecordDataType = "";
                    string sRawRecordDataType = "";

                    if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASE))
                    {
                        sRecordDataType = MainConstantParameter.m_sDATATYPE_BASE;
                        sRawRecordDataType = MainConstantParameter.m_sDATATYPE_RAW_BASE;
                    }
                    else if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASE))
                    {
                        sRecordDataType = MainConstantParameter.m_sDATATYPE_OBASE;
                        sRawRecordDataType = MainConstantParameter.m_sDATATYPE_RAW_OBASE;
                    }

                    //Get BASE Frame
                    //Allocate memory to store frame trace
                    nFrame_Array = new int[1, nTXTraceNumber + 1, nRXTraceNumber + 1];

                    Array.Clear(nFrame_Array, 0, nFrame_Array.Length);

                    GetDataInfo cGetDataInfo = new GetDataInfo();
                    cGetDataInfo.m_cFlowStep = cFlowStep;
                    cGetDataInfo.m_cFrequencyItem = cFrequencyItem;
                    cGetDataInfo.m_sDataType = sRecordDataType;
                    cGetDataInfo.m_nFrameData_Array = nFrame_Array;
                    cGetDataInfo.m_nFrameNumber = cFrequencyItem.m_nADCTestFrame;
                    cGetDataInfo.m_nRXTraceNumber = nRXTraceNumber;
                    cGetDataInfo.m_nTXTraceNumber = nTXTraceNumber;
                    cGetDataInfo.m_nListIndex = nExecuteIndex + 1;
                    cGetDataInfo.m_nListCount = nTotalCount;
                    cGetDataInfo.m_sStageMessage = sStageMessage;
                    cGetDataInfo.m_bGetBaseDelay = bGetFirstDataFlag;

                    if (m_eICGenerationType != ICGenerationType.Gen9)
                    {
                        if (GetData(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#if _USE_9F07_SOCKET
                    else
                    {
                        if (GetData_9F07(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#endif

                    cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, sRecordDataType, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber);

                    if (SaveData(sRecordDataType, cSaveDataInfo, nFrame_Array) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }

                    if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                    {
                        if (SaveNormalRawDataType(cFrequencyItem, sRawRecordDataType, ElanDefine.RAWDATA_BASE, nExecuteIndex + 1, m_sLogDirectoryPath) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }

                    nBASEFrame_Array = nFrame_Array;
                }
            }
            #endregion
            #region Get OBASE & BASE Data(UseADCAtFirst)
            else if (SetGetBaseType(cFlowStep) == MainConstantParameter.m_nGetBaseType_UseADCAtFirst)
            {
                //Get OBASE & BASE Data
                if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASE) || m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASE))
                {
                    OutputMessage("-Get ADC Data For Base At First");

                    string sRecordDataType = "";
                    //string sRawRecordDataType = "";

                    if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASE))
                    {
                        sRecordDataType = MainConstantParameter.m_sDATATYPE_BASE;
                        //sRawRecordDataType = MainConstantParameter.m_sDATATYPE_RAW_BASE;
                    }
                    else if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASE))
                    {
                        sRecordDataType = MainConstantParameter.m_sDATATYPE_OBASE;
                        //sRawRecordDataType = MainConstantParameter.m_sDATATYPE_RAW_OBASE;
                    }

                    bool bUseNewMethod = false;

                    int nUseADCForBaseFrame = ParamFingerAutoTuning.m_nFRPH2ACFRUseADCForBaseFrame;

                    if (m_eICGenerationType == ICGenerationType.Gen8)
                        nUseADCForBaseFrame = ParamFingerAutoTuning.m_nGen8UseADCForBaseFrame;

                    //Get ADC Frame
                    //Allocate memory to store frame trace
                    nFrame_Array = new int[nUseADCForBaseFrame, nTXTraceNumber + 1, nRXTraceNumber + 1];

                    Array.Clear(nFrame_Array, 0, nFrame_Array.Length);

                    GetDataInfo cGetDataInfo = new GetDataInfo();
                    cGetDataInfo.m_cFlowStep = cFlowStep;
                    cGetDataInfo.m_cFrequencyItem = cFrequencyItem;
                    cGetDataInfo.m_sDataType = MainConstantParameter.m_sDATATYPE_ADC;
                    cGetDataInfo.m_nFrameData_Array = nFrame_Array;
                    cGetDataInfo.m_nFrameNumber = nUseADCForBaseFrame;
                    cGetDataInfo.m_nRXTraceNumber = nRXTraceNumber;
                    cGetDataInfo.m_nTXTraceNumber = nTXTraceNumber;
                    cGetDataInfo.m_nListIndex = nExecuteIndex + 1;
                    cGetDataInfo.m_nListCount = nTotalCount;
                    cGetDataInfo.m_sStageMessage = sStageMessage;
                    cGetDataInfo.m_bUseNewMethod = bUseNewMethod;

                    int[, ,] nMeanFrame_Array = new int[1, nTXTraceNumber + 1, nRXTraceNumber + 1];
                    Array.Clear(nMeanFrame_Array, 0, nMeanFrame_Array.Length);

                    if (m_eICGenerationType != ICGenerationType.Gen9)
                    {
                        if (GetData(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#if _USE_9F07_SOCKET
                    else
                    {
                        if (GetData_9F07(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#endif

                    for (int nFrameIndex = 0; nFrameIndex < nUseADCForBaseFrame; nFrameIndex++)
                    {
                        for (int nRXIndex = 1; nRXIndex < m_nRXTraceNumber + 1; nRXIndex++)
                        {
                            for (int nTXIndex = 1; nTXIndex < m_nTXTraceNumber + 1; nTXIndex++)
                                nMeanFrame_Array[0, nTXIndex, nRXIndex] += nFrame_Array[nFrameIndex, nTXIndex, nRXIndex];
                        }
                    }

                    for (int nRXIndex = 1; nRXIndex < m_nRXTraceNumber + 1; nRXIndex++)
                    {
                        for (int nTXIndex = 1; nTXIndex < m_nTXTraceNumber + 1; nTXIndex++)
                            nMeanFrame_Array[0, nTXIndex, nRXIndex] = (int)Math.Round((double)nMeanFrame_Array[0, nTXIndex, nRXIndex] / nUseADCForBaseFrame, 0, MidpointRounding.AwayFromZero);
                    }

                    nFrame_Array = new int[1, nTXTraceNumber + 1, nRXTraceNumber + 1];
                    nFrame_Array = nMeanFrame_Array;

                    cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, sRecordDataType, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber);

                    if (SaveData(sRecordDataType, cSaveDataInfo, nFrame_Array) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }

                    if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                    {
                        /*
                        if (SaveNormalRawDataType(cFrequencyItem, sRawRecordDataType, ElanDef.RAWDATA_BASE, nExecuteIndex + 1, m_sLogDirectoryPath) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                        */
                    }

                    nBASEFrame_Array = nFrame_Array;
                }
            }
            #endregion

            #region Get ADC Data
            //Get ADC Data
            if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_ADC))
            {
                OutputMessage("-Get ADC Data");

                bool bUseNewMethod = false;

                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    if (cFlowStep.m_eStep == MainStep.FrequencyRank_Phase1 && ParamFingerAutoTuning.m_nEnableUseNewMethod == 1 &&
                         ((m_eICGenerationType == ICGenerationType.Gen8 && m_eICSolutionType == ICSolutionType.Solution_8F18) ||
                         m_eICGenerationType != ICGenerationType.Gen8))
                    {
                        OutputMessage("-Use New Method Get ADC Data");

                        //Get ADC Frame
                        //Allocate memory to store frame trace
                        nFrame_Array = new int[1, nTXTraceNumber + 1, nRXTraceNumber + 1];
                        bUseNewMethod = true;
                    }
                    else
                    {
                        //Get ADC Frame
                        //Allocate memory to store frame trace
                        //nFrame_Array = new int[cFrequencyItem.m_nADCTestFrame, nTXTraceNumber + 1 + 1, nRXTraceNumber + 1 + 1];
                        nFrame_Array = new int[cFrequencyItem.m_nADCTestFrame, nTXTraceNumber + 1, nRXTraceNumber + 1];
                    }
                }
#if _USE_9F07_SOCKET
                else
                {
                    //Get ADC Frame
                    //Allocate memory to store frame trace
                    nFrame_Array = new int[cFrequencyItem.m_nADCTestFrame, nTXTraceNumber + 1, nRXTraceNumber + 1];
                }
#endif

                Array.Clear(nFrame_Array, 0, nFrame_Array.Length);

                GetDataInfo cGetDataInfo = new GetDataInfo();
                cGetDataInfo.m_cFlowStep = cFlowStep;
                cGetDataInfo.m_cFrequencyItem = cFrequencyItem;
                cGetDataInfo.m_sDataType = MainConstantParameter.m_sDATATYPE_ADC;
                cGetDataInfo.m_nFrameData_Array = nFrame_Array;
                cGetDataInfo.m_nFrameNumber = cFrequencyItem.m_nADCTestFrame;
                cGetDataInfo.m_nRXTraceNumber = nRXTraceNumber;
                cGetDataInfo.m_nTXTraceNumber = nTXTraceNumber;
                cGetDataInfo.m_nListIndex = nExecuteIndex + 1;
                cGetDataInfo.m_nListCount = nTotalCount;
                cGetDataInfo.m_sStageMessage = sStageMessage;
                cGetDataInfo.m_bUseNewMethod = bUseNewMethod;
                //cGetDataInfo.m_bGetSelf = true;

                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    if (GetData(cGetDataInfo) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }
#if _USE_9F07_SOCKET
                else
                {
                    if (GetData_9F07(cGetDataInfo) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }
#endif

                cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, MainConstantParameter.m_sDATATYPE_ADC, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber, bUseNewMethod);

                if (SaveData(MainConstantParameter.m_sDATATYPE_ADC, cSaveDataInfo, nFrame_Array) == false)
                {
                    RunScreenResetFlow(nBrightness);
                    return false;
                }

                if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                {
                    if (SaveNormalRawDataType(cFrequencyItem, MainConstantParameter.m_sDATATYPE_RAW_ADC, ElanDefine.RAWDATA_ADC, nExecuteIndex + 1, m_sLogDirectoryPath) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }

                nADCFrame_Array = nFrame_Array;
            }
            #endregion

            #region Get OBASE & BASE Data(UseBaseAfterGetADC)
            if (SetGetBaseType(cFlowStep) == MainConstantParameter.m_nGetBaseType_UseBaseAfterGetADC)
            {
                //Get OBASE & BASE Data
                if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASE) || m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASE))
                {
                    OutputMessage("-Get Base Data After Get ADC");

                    string sRecordDataType = "";
                    string sRawRecordDataType = "";

                    if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASE))
                    {
                        sRecordDataType = MainConstantParameter.m_sDATATYPE_BASE;
                        sRawRecordDataType = MainConstantParameter.m_sDATATYPE_RAW_BASE;
                    }
                    else if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASE))
                    {
                        sRecordDataType = MainConstantParameter.m_sDATATYPE_OBASE;
                        sRawRecordDataType = MainConstantParameter.m_sDATATYPE_RAW_OBASE;
                    }

                    //Get BASE Frame
                    //Allocate memory to store frame trace
                    nFrame_Array = new int[1, nTXTraceNumber + 1, nRXTraceNumber + 1];

                    Array.Clear(nFrame_Array, 0, nFrame_Array.Length);

                    GetDataInfo cGetDataInfo = new GetDataInfo();
                    cGetDataInfo.m_cFlowStep = cFlowStep;
                    cGetDataInfo.m_cFrequencyItem = cFrequencyItem;
                    cGetDataInfo.m_sDataType = sRecordDataType;
                    cGetDataInfo.m_nFrameData_Array = nFrame_Array;
                    cGetDataInfo.m_nFrameNumber = cFrequencyItem.m_nADCTestFrame;
                    cGetDataInfo.m_nRXTraceNumber = nRXTraceNumber;
                    cGetDataInfo.m_nTXTraceNumber = nTXTraceNumber;
                    cGetDataInfo.m_nListIndex = nExecuteIndex + 1;
                    cGetDataInfo.m_nListCount = nTotalCount;
                    cGetDataInfo.m_sStageMessage = sStageMessage;
                    cGetDataInfo.m_bGetBaseDelay = bGetFirstDataFlag;

                    if (m_eICGenerationType != ICGenerationType.Gen9)
                    {
                        if (GetData(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#if _USE_9F07_SOCKET
                    else
                    {
                        if (GetData_9F07(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#endif

                    cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, sRecordDataType, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber);

                    if (SaveData(sRecordDataType, cSaveDataInfo, nFrame_Array) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }

                    if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                    {
                        if (SaveNormalRawDataType(cFrequencyItem, sRawRecordDataType, ElanDefine.RAWDATA_BASE, nExecuteIndex + 1, m_sLogDirectoryPath) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }

                    nBASEFrame_Array = nFrame_Array;
                }
            }
            #endregion
            #region Get OBASE & BASE Data(UseADCAfterGetADC)
            else if (SetGetBaseType(cFlowStep) == MainConstantParameter.m_nGetBaseType_UseADCAfterGetADC)
            {
                //Get OBASE & BASE Data
                if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASE) || m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASE))
                {
                    OutputMessage("-Get ADC Data For Base After Get ADC");

                    string sRecordDataType = "";
                    //string sRawRecordDataType = "";

                    if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASE))
                    {
                        sRecordDataType = MainConstantParameter.m_sDATATYPE_BASE;
                        //sRawRecordDataType = MainConstantParameter.m_sDATATYPE_RAW_BASE;
                    }
                    else if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASE))
                    {
                        sRecordDataType = MainConstantParameter.m_sDATATYPE_OBASE;
                        //sRawRecordDataType = MainConstantParameter.m_sDATATYPE_RAW_OBASE;
                    }

                    bool bUseNewMethod = false;

                    int nUseADCForBaseFrame = ParamFingerAutoTuning.m_nFRPH2ACFRUseADCForBaseFrame;

                    if (m_eICGenerationType == ICGenerationType.Gen8)
                        nUseADCForBaseFrame = ParamFingerAutoTuning.m_nGen8UseADCForBaseFrame;

                    //Get ADC Frame
                    //Allocate memory to store frame trace
                    nFrame_Array = new int[nUseADCForBaseFrame, nTXTraceNumber + 1, nRXTraceNumber + 1];

                    Array.Clear(nFrame_Array, 0, nFrame_Array.Length);

                    GetDataInfo cGetDataInfo = new GetDataInfo();
                    cGetDataInfo.m_cFlowStep = cFlowStep;
                    cGetDataInfo.m_cFrequencyItem = cFrequencyItem;
                    cGetDataInfo.m_sDataType = MainConstantParameter.m_sDATATYPE_ADC;
                    cGetDataInfo.m_nFrameData_Array = nFrame_Array;
                    cGetDataInfo.m_nFrameNumber = nUseADCForBaseFrame;
                    cGetDataInfo.m_nRXTraceNumber = nRXTraceNumber;
                    cGetDataInfo.m_nTXTraceNumber = nTXTraceNumber;
                    cGetDataInfo.m_nListIndex = nExecuteIndex + 1;
                    cGetDataInfo.m_nListCount = nTotalCount;
                    cGetDataInfo.m_sStageMessage = sStageMessage;
                    cGetDataInfo.m_bUseNewMethod = bUseNewMethod;

                    int[, ,] nMeanFrame_Array = new int[1, nTXTraceNumber + 1, nRXTraceNumber + 1];
                    Array.Clear(nMeanFrame_Array, 0, nMeanFrame_Array.Length);

                    if (m_eICGenerationType != ICGenerationType.Gen9)
                    {
                        if (GetData(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#if _USE_9F07_SOCKET
                    else
                    {
                        if (GetData_9F07(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#endif

                    for (int nFrameIndex = 0; nFrameIndex < nUseADCForBaseFrame; nFrameIndex++)
                    {
                        for (int nRXIndex = 1; nRXIndex < m_nRXTraceNumber + 1; nRXIndex++)
                        {
                            for (int nTXIndex = 1; nTXIndex < m_nTXTraceNumber + 1; nTXIndex++)
                                nMeanFrame_Array[0, nTXIndex, nRXIndex] += nFrame_Array[nFrameIndex, nTXIndex, nRXIndex];
                        }
                    }

                    for (int nRXIndex = 1; nRXIndex < m_nRXTraceNumber + 1; nRXIndex++)
                    {
                        for (int nTXIndex = 1; nTXIndex < m_nTXTraceNumber + 1; nTXIndex++)
                            nMeanFrame_Array[0, nTXIndex, nRXIndex] = (int)Math.Round((double)nMeanFrame_Array[0, nTXIndex, nRXIndex] / nUseADCForBaseFrame, 0, MidpointRounding.AwayFromZero);
                    }

                    nFrame_Array = new int[1, nTXTraceNumber + 1, nRXTraceNumber + 1];
                    nFrame_Array = nMeanFrame_Array;

                    cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, sRecordDataType, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber);

                    if (SaveData(sRecordDataType, cSaveDataInfo, nFrame_Array) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }

                    if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                    {
                        /*
                        if (SaveNormalRawDataType(cFrequencyItem, sRawRecordDataType, ElanDef.RAWDATA_BASE, nExecuteIndex + 1, m_sLogDirectoryPath) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                        */
                    }

                    nBASEFrame_Array = nFrame_Array;
                }
            }
            #endregion
            #region Get OBASE & BASE Data(UseNewMethodGetADC)
            else if (SetGetBaseType(cFlowStep) == MainConstantParameter.m_nGetBaseType_UseNewMethodGetADC)
            {
                //Get OBASE & BASE Data
                if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASE) || m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASE))
                {
                    OutputMessage("-Use New Method Get ADC Data For Base");

                    string sRecordDataType = "";
                    //string sRawRecordDataType = "";

                    if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASE))
                    {
                        sRecordDataType = MainConstantParameter.m_sDATATYPE_BASE;
                        //sRawRecordDataType = MainConstantParameter.m_sDATATYPE_RAW_BASE;
                    }
                    else if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASE))
                    {
                        sRecordDataType = MainConstantParameter.m_sDATATYPE_OBASE;
                        //sRawRecordDataType = MainConstantParameter.m_sDATATYPE_RAW_OBASE;
                    }

                    //Get ADC Frame
                    //Allocate memory to store frame trace
                    nFrame_Array = new int[1, nTXTraceNumber + 1, nRXTraceNumber + 1];
                    bool bUseNewMethod = true;

                    int nUseADCForBaseFrame = ParamFingerAutoTuning.m_nFRPH2ACFRUseADCForBaseFrame;

                    if (m_eICGenerationType == ICGenerationType.Gen8)
                        nUseADCForBaseFrame = ParamFingerAutoTuning.m_nGen8UseADCForBaseFrame;

                    Array.Clear(nFrame_Array, 0, nFrame_Array.Length);

                    GetDataInfo cGetDataInfo = new GetDataInfo();
                    cGetDataInfo.m_cFlowStep = cFlowStep;
                    cGetDataInfo.m_cFrequencyItem = cFrequencyItem;
                    cGetDataInfo.m_sDataType = MainConstantParameter.m_sDATATYPE_ADC;
                    cGetDataInfo.m_nFrameData_Array = nFrame_Array;
                    cGetDataInfo.m_nFrameNumber = nUseADCForBaseFrame;
                    cGetDataInfo.m_nRXTraceNumber = nRXTraceNumber;
                    cGetDataInfo.m_nTXTraceNumber = nTXTraceNumber;
                    cGetDataInfo.m_nListIndex = nExecuteIndex + 1;
                    cGetDataInfo.m_nListCount = nTotalCount;
                    cGetDataInfo.m_sStageMessage = sStageMessage;
                    cGetDataInfo.m_bUseNewMethod = bUseNewMethod;

                    int[, ,] nMeanFrame_Array = new int[1, nTXTraceNumber + 1, nRXTraceNumber + 1];
                    Array.Clear(nMeanFrame_Array, 0, nMeanFrame_Array.Length);

                    if (m_eICGenerationType != ICGenerationType.Gen9)
                    {
                        if (GetData(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#if _USE_9F07_SOCKET
                    else
                    {
                        if (GetData_9F07(cGetDataInfo) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                    }
#endif

                    cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, sRecordDataType, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber);

                    if (SaveData(sRecordDataType, cSaveDataInfo, nFrame_Array) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }

                    if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                    {
                        /*
                        if (SaveNormalRawDataType(cFrequencyItem, sRawRecordDataType, ElanDef.RAWDATA_BASE, nExecuteIndex + 1, m_sLogDirectoryPath) == false)
                        {
                            RunScreenResetFlow(nBrightness);
                            return false;
                        }
                        */
                    }

                    nBASEFrame_Array = nFrame_Array;
                }
            }
            #endregion

            #region Get BASE-ADC Data
            //Get BASE-ADC Data
            if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_BASEMinusADC))
            {
                int[, ,] nBASEMinusADCFrame_Array = new int[cFrequencyItem.m_nADCTestFrame, nTXTraceNumber + 1, nRXTraceNumber + 1];

                if (nBASEFrame_Array == null)
                {
                    if (GetFileData(ref nBASEFrame_Array, cFrequencyItem, MainConstantParameter.m_sDATATYPE_BASE, nExecuteIndex + 1) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }

                if (nADCFrame_Array == null)
                {
                    if (GetFileData(ref nADCFrame_Array, cFrequencyItem, MainConstantParameter.m_sDATATYPE_ADC, nExecuteIndex + 1) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }

                ComputeBASEMinusADC(ref nBASEMinusADCFrame_Array, nBASEFrame_Array, nADCFrame_Array, cFrequencyItem.m_nADCTestFrame, nRXTraceNumber, nTXTraceNumber);

                cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, MainConstantParameter.m_sDATATYPE_BASEMinusADC, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber);

                if (SaveData(MainConstantParameter.m_sDATATYPE_BASEMinusADC, cSaveDataInfo, nBASEMinusADCFrame_Array) == false)
                {
                    RunScreenResetFlow(nBrightness);
                    return false;
                }

                if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                {
                    if (SaveNormalRawDataType(cFrequencyItem, MainConstantParameter.m_sDATATYPE_RAW_BASEMinusADC, ElanDefine.RAWDATA_dV, nExecuteIndex + 1, m_sLogDirectoryPath) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }
            }
            #endregion

            #region Get OBASE-ADC Data
            //Get OBASE-ADC Data
            if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_OBASEMinusADC))
            {
                int[, ,] nBASEMinusADCFrame_Array = new int[cFrequencyItem.m_nADCTestFrame, nTXTraceNumber + 1, nRXTraceNumber + 1];
                int[, ,] nOBASEFrame_Array = null;

                if (GetFileData(ref nOBASEFrame_Array, cFrequencyItem, MainConstantParameter.m_sDATATYPE_OBASE, nExecuteIndex + 1) == false)
                {
                    RunScreenResetFlow(nBrightness);
                    return false;
                }

                if (nADCFrame_Array == null)
                {
                    if (GetFileData(ref nADCFrame_Array, cFrequencyItem, MainConstantParameter.m_sDATATYPE_ADC, nExecuteIndex + 1) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }

                ComputeBASEMinusADC(ref nBASEMinusADCFrame_Array, nOBASEFrame_Array, nADCFrame_Array, cFrequencyItem.m_nADCTestFrame, nRXTraceNumber, nTXTraceNumber);

                cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, MainConstantParameter.m_sDATATYPE_OBASEMinusADC, m_sLogDirectoryPath, nExecuteIndex + 1,
                                                nTXTraceNumber, nRXTraceNumber);

                if (SaveData(MainConstantParameter.m_sDATATYPE_OBASEMinusADC, cSaveDataInfo, nBASEMinusADCFrame_Array) == false)
                {
                    RunScreenResetFlow(nBrightness);
                    return false;
                }

                if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                {
                    if (SaveNormalRawDataType(cFrequencyItem, MainConstantParameter.m_sDATATYPE_RAW_OBASEMinusADC, ElanDefine.RAWDATA_dV, nExecuteIndex + 1, m_sLogDirectoryPath) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }
            }
            #endregion

            #region Get DV Data
            //Get DV Data
            if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_DV))
            {
                OutputMessage("-Get DV Data");

                bool bSetKParameter = false;
                //Get DV Frame
                //Allocate memory to store frame trace
                if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep)
                {
                    if (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2)
                        SetENCAL_SFEnable(true);

                    nFrame_Array = new int[cFrequencyItem.m_nDVTestFrame, nTXTraceNumber + 2, nRXTraceNumber + 2];

                    if (ParamFingerAutoTuning.m_nSelfFSRunKSequence == 1 || ParamFingerAutoTuning.m_nSelfFSRunKSequence == 2)
                        bSetKParameter = true;
                }
                else
                    nFrame_Array = new int[cFrequencyItem.m_nDVTestFrame, nTXTraceNumber + 1, nRXTraceNumber + 1];

                Array.Clear(nFrame_Array, 0, nFrame_Array.Length);

                GetDataInfo cGetDataInfo = new GetDataInfo();
                cGetDataInfo.m_cFlowStep = cFlowStep;
                cGetDataInfo.m_cFrequencyItem = cFrequencyItem;
                cGetDataInfo.m_sDataType = MainConstantParameter.m_sDATATYPE_DV;
                cGetDataInfo.m_nFrameData_Array = nFrame_Array;
                cGetDataInfo.m_nFrameNumber = cFrequencyItem.m_nDVTestFrame;
                cGetDataInfo.m_nRXTraceNumber = nRXTraceNumber;
                cGetDataInfo.m_nTXTraceNumber = nTXTraceNumber;
                cGetDataInfo.m_nListIndex = nExecuteIndex + 1;
                cGetDataInfo.m_nListCount = nTotalCount;
                cGetDataInfo.m_sStageMessage = sStageMessage;
                cGetDataInfo.m_bGetSelf = bGetSelfFlag;
                cGetDataInfo.m_bSetKParameter = bSetKParameter;

                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    if (GetData(cGetDataInfo) == false)
                    {
                        RunScreenResetFlow(nBrightness);

                        if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep &&
                            (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
                            SetENCAL_SFEnable(false);

                        return false;
                    }
                }
#if _USE_9F07_SOCKET
                else
                {
                    if (GetData_9F07(cGetDataInfo) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }
#endif

                cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, MainConstantParameter.m_sDATATYPE_DV, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber);

                if (SaveData(MainConstantParameter.m_sDATATYPE_DV, cSaveDataInfo, nFrame_Array) == false)
                {
                    RunScreenResetFlow(nBrightness);

                    if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep &&
                        (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
                        SetENCAL_SFEnable(false);

                    return false;
                }

                if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                {
                    if (SaveNormalRawDataType(cFrequencyItem, MainConstantParameter.m_sDATATYPE_RAW_DV, ElanDefine.RAWDATA_dV, nExecuteIndex + 1, m_sLogDirectoryPath, bGetSelfFlag) == false)
                    {
                        RunScreenResetFlow(nBrightness);

                        if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep &&
                            (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
                            SetENCAL_SFEnable(false);

                        return false;
                    }
                }

                if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep &&
                    (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))
                    SetENCAL_SFEnable(false);
            }
            #endregion

            #region Get Raw P/N Data
            //Get Raw P/N Data
            if (m_sGetData_List.Contains(MainConstantParameter.m_sDATATYPE_RawData))
            {
                OutputMessage("-Get Raw P/N Data");

                bool bSetKParameter = false;

                //Get Raw P/N Frame
                //Allocate memory to store frame trace
                if (cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                {
                    nFrame_Array = new int[cFrequencyItem.m_nDVTestFrame, nTXTraceNumber + 2, nRXTraceNumber + 2];
                    bSetKParameter = true;
                }
                else
                    nFrame_Array = new int[cFrequencyItem.m_nDVTestFrame, nTXTraceNumber + 1, nRXTraceNumber + 1];

                Array.Clear(nFrame_Array, 0, nFrame_Array.Length);

                GetDataInfo cGetDataInfo = new GetDataInfo();
                cGetDataInfo.m_cFlowStep = cFlowStep;
                cGetDataInfo.m_cFrequencyItem = cFrequencyItem;
                cGetDataInfo.m_sDataType = MainConstantParameter.m_sDATATYPE_DV;
                cGetDataInfo.m_nFrameData_Array = nFrame_Array;
                cGetDataInfo.m_nFrameNumber = cFrequencyItem.m_nDVTestFrame;
                cGetDataInfo.m_nRXTraceNumber = nRXTraceNumber;
                cGetDataInfo.m_nTXTraceNumber = nTXTraceNumber;
                cGetDataInfo.m_nListIndex = nExecuteIndex + 1;
                cGetDataInfo.m_nListCount = nTotalCount;
                cGetDataInfo.m_sStageMessage = sStageMessage;
                cGetDataInfo.m_bGetSelf = bGetSelfFlag;
                cGetDataInfo.m_bSetKParameter = bSetKParameter;

                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    if (GetData(cGetDataInfo) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }
#if _USE_9F07_SOCKET
                else
                {
                    if (GetData_9F07(cGetDataInfo) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }
#endif

                cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, MainConstantParameter.m_sDATATYPE_RawData, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber);

                if (SaveData(MainConstantParameter.m_sDATATYPE_RawData, cSaveDataInfo, nFrame_Array) == false)
                {
                    RunScreenResetFlow(nBrightness);
                    return false;
                }

                if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                {
                    if (SaveNormalRawDataType(cFrequencyItem, MainConstantParameter.m_sDATATYPE_RawData, ElanDefine.RAWDATA_dV, nExecuteIndex + 1, m_sLogDirectoryPath, bGetSelfFlag) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        return false;
                    }
                }
            }
            #endregion

            return true;
        }

        /// <summary>
        /// 根據IC世代類型和參數設定,決定Base資料的取得方式(時機與類型)
        /// </summary>
        /// <param name="cFlowStep">當前執行的流程步驟</param>
        /// <returns>回傳Base資料取得類型代碼:UseBaseAtFirst(先取Base)、UseBaseAfterGetADC(取ADC後取Base)、UseADCAfterGetADC(取ADC後用ADC)、UseNewMethodGetADC(使用新方法取ADC)、UseADCAtFirst(先取ADC)</returns>
        private int SetGetBaseType(frmMain.FlowStep cFlowStep)
        {
            if (m_eICGenerationType != ICGenerationType.Gen8)
            {
                if (ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseType == 0)
                    return MainConstantParameter.m_nGetBaseType_UseBaseAtFirst;
                else if (ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseType == 1)
                    return MainConstantParameter.m_nGetBaseType_UseBaseAfterGetADC;
                else if (ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseType == 2)
                    return MainConstantParameter.m_nGetBaseType_UseADCAfterGetADC;
                else if (ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseType == 3)
                {
                    if (ParamFingerAutoTuning.m_nEnableUseNewMethod != 1)
                        return MainConstantParameter.m_nGetBaseType_UseADCAfterGetADC;
                    else
                        return MainConstantParameter.m_nGetBaseType_UseNewMethodGetADC;
                }
                else if (ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseType == 4)
                    return MainConstantParameter.m_nGetBaseType_UseADCAtFirst;
            }
            else if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                if (ParamFingerAutoTuning.m_nGen8GetBaseType == 0)
                {
                    if (m_nNoResetBaseFunctionFlag_Gen8 == 1)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                        {
                            if (ParamFingerAutoTuning.m_nEnableUseNewMethod != 1)
                                return MainConstantParameter.m_nGetBaseType_UseADCAfterGetADC;
                            else
                                return MainConstantParameter.m_nGetBaseType_UseNewMethodGetADC;
                        }
                        else
                            return MainConstantParameter.m_nGetBaseType_UseADCAfterGetADC;
                    }
                    else
                        return MainConstantParameter.m_nGetBaseType_UseBaseAtFirst;
                }
                else if (ParamFingerAutoTuning.m_nGen8GetBaseType == 1)
                {
                    if (m_nNoResetBaseFunctionFlag_Gen8 == 1)
                    {
                        if (m_eICSolutionType == ICSolutionType.Solution_8F18)
                        {
                            if (ParamFingerAutoTuning.m_nEnableUseNewMethod != 1)
                                return MainConstantParameter.m_nGetBaseType_UseADCAfterGetADC;
                            else
                                return MainConstantParameter.m_nGetBaseType_UseNewMethodGetADC;
                        }
                        else
                            return MainConstantParameter.m_nGetBaseType_UseADCAfterGetADC;
                    }
                    else
                        return MainConstantParameter.m_nGetBaseType_UseBaseAfterGetADC;
                }
                else if (ParamFingerAutoTuning.m_nGen8GetBaseType == 2 || (ParamFingerAutoTuning.m_nGen8GetBaseType == 3 && m_eICSolutionType != ICSolutionType.Solution_8F18))
                    return MainConstantParameter.m_nGetBaseType_UseADCAfterGetADC;
                else if (ParamFingerAutoTuning.m_nGen8GetBaseType == 3 && m_eICSolutionType == ICSolutionType.Solution_8F18)
                {
                    if (ParamFingerAutoTuning.m_nEnableUseNewMethod != 1)
                        return MainConstantParameter.m_nGetBaseType_UseADCAfterGetADC;
                    else
                        return MainConstantParameter.m_nGetBaseType_UseNewMethodGetADC;
                }
                else if (ParamFingerAutoTuning.m_nGen8GetBaseType == 4)
                    return MainConstantParameter.m_nGetBaseType_UseADCAtFirst;
            }

            return MainConstantParameter.m_nGetBaseType_UseBaseAtFirst;
        }

        /// <summary>
        /// 取得感測器資料的主要方法,執行完整的資料擷取流程,包含重試機制、參數設定、Frame跳過與取得、測試模式進出等處理
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含流程步驟、資料類型、Frame數量、RX/TX追蹤線數量、索引、訊息等參數</param>
        /// <returns>資料擷取成功回傳true;若使用者中斷執行、重試失敗或資料取得失敗則回傳false</returns>
        private bool GetData(GetDataInfo cGetDataInfo)
        {
            bool bRetryFlag = false;
            int nFrameValue = 0;
            int nRetryCount = 0;
            int nFrameCount = cGetDataInfo.m_nFrameNumber;

            // 針對特定資料類型，設定 Frame 數量為 1
            if (IsFrameNumberOne(cGetDataInfo.m_sDataType))
                cGetDataInfo.m_nFrameNumber = 1;

            // 初始化參數
            int nRXTotalTraceNumber = cGetDataInfo.m_nRXTraceNumber;
            int nTXTotalTraceNumber = cGetDataInfo.m_nTXTraceNumber;
            int nRETRYCOUNTTIMES = 10;
            int nTimeout = ParamFingerAutoTuning.m_nGetDataTimeout;
            UserInterfaceDefine.RawDataType eRawDataType = UserInterfaceDefine.RawDataType.Type_ADC;
            byte byteScanType = AppCoreDefine.m_byteMODE_MUTUAL;

            // 設定取得資料的參數
            SetGetDataParameter(
                ref nRETRYCOUNTTIMES, ref nTimeout, ref eRawDataType, ref byteScanType, 
                ref nRXTotalTraceNumber, ref nTXTotalTraceNumber,
                cGetDataInfo.m_sDataType, cGetDataInfo.m_nRXTraceNumber, 
                cGetDataInfo.m_nTXTraceNumber, cGetDataInfo.m_bGetSelf
            );

            if (cGetDataInfo.m_sStageMessage != "")
                cGetDataInfo.m_sStageMessage = string.Format("{0} ", cGetDataInfo.m_sStageMessage);

            // 準備資料緩衝區和訊息
            int[,] nFrameBuffer_Array = null;
            long nStartTime = 0;
            string sRealDataType = GetRealDataType(cGetDataInfo);
            string sMessage = BuildGetDataMessage(cGetDataInfo, sRealDataType);

            OutputMessage(string.Format("-{0}", sMessage));

            // 宣告暫存緩衝區
            int[] nDataBuffer_Array = CreateDataBuffer(cGetDataInfo);

            // 主要重試迴圈
            for (int nRetryIndex = 0; nRetryIndex <= nRETRYCOUNTTIMES; nRetryIndex++)
            {
                if (!m_cfrmParent.m_bExecute) return false;

                bRetryFlag = false;

                // 處理重試時的重新連線
                if (nRetryIndex > 0)
                {
                    if (!HandleRetryReconnect(cGetDataInfo)) return false;
                }

                // Gen8 特殊處理：取得 Base 資料前的延遲
                if (!HandleGen8BaseDelay(cGetDataInfo)) return false;

                // Gen8 取得資料延遲
                if (m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nGen8GetDataDelayTime > 0)
                    Thread.Sleep(ParamFingerAutoTuning.m_nGen8GetDataDelayTime);

                // 進入測試模式前的準備
                PrepareBeforeGetData(cGetDataInfo);

                // 跳過 n 個 Frame
                if (ShouldSkipFrames(cGetDataInfo))
                {
                    if (!SkipFrames(cGetDataInfo, ref bRetryFlag, ref nRetryCount, nRETRYCOUNTTIMES, nDataBuffer_Array, byteScanType, nTimeout))
                        return false;
                }

                if (bRetryFlag) continue;

                // 處理跳過 Frame 後的設定
                HandleAfterSkipFrames(cGetDataInfo);

                // 取得 n 個 Frames
                if (!GetFrames(cGetDataInfo, ref bRetryFlag, ref nFrameValue, ref nRetryCount, 
                              nRETRYCOUNTTIMES, nDataBuffer_Array, nRXTotalTraceNumber, 
                              nTXTotalTraceNumber, byteScanType, nTimeout, eRawDataType, 
                              ref nFrameBuffer_Array, ref nStartTime))
                    return false;

                if (!bRetryFlag) break;
            }

            // 退出測試模式
            ExitTestMode(cGetDataInfo.m_sDataType);

            // 處理儲存的資料
            if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
            {
                TransferQueueToFrameManager();
            }

            return true;
        }

#if _USE_9F07_SOCKET
        /// <summary>
        /// 取得感測器資料的9F07版本實作,針對Gen9 IC使用Socket通訊方式進行資料擷取,包含Frame跳過與實際資料取得流程
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含流程步驟、資料類型、Frame數量、RX/TX追蹤線數量、索引、訊息等參數</param>
        /// <returns>資料擷取成功回傳true;若使用者中斷執行、跳過Frame失敗或資料取得失敗則回傳false</returns>
        private bool GetData_9F07(GetDataInfo cGetDataInfo)
        {
            int nFrameIndex = 0;

            if (IsFrameNumberOne(cGetDataInfo.m_sDataType))
                cGetDataInfo.m_nFrameNumber = 1;

            int nRXTotalTraceNumber = cGetDataInfo.m_nRXTraceNumber;
            int nTXTotalTraceNumber = cGetDataInfo.m_nTXTraceNumber;
            int nTimeout = ParamFingerAutoTuning.m_nGetDataTimeout;
            UserInterfaceDefine.RawDataType eRawDataType = DetermineRawDataType(cGetDataInfo.m_sDataType);

            int nFrameCount = cGetDataInfo.m_nFrameNumber;
            int nSkipFrameCount = ParamFingerAutoTuning.m_nSkipFrame;

            if (cGetDataInfo.m_sStageMessage != "")
                cGetDataInfo.m_sStageMessage = string.Format("{0} ", cGetDataInfo.m_sStageMessage);

            string sMessage = string.Format("Get {0} Data[FrameCount={1}]", cGetDataInfo.m_sDataType, cGetDataInfo.m_nFrameNumber);
            OutputMessage(string.Format("-{0}", sMessage));

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                SetDisplayFrameNumber(string.Format("Get {0}", cGetDataInfo.m_sDataType), 0, nFrameCount);
            });

            // 準備資料緩衝區
            int nRawDataLength = cGetDataInfo.m_nRXTraceNumber * cGetDataInfo.m_nTXTraceNumber * 2;
            byte[] byteDataBuffer_Array = new byte[nRawDataLength + 1];
            byte byteDataType = GetDataTypeByte_9F07(eRawDataType);

            // 啟用取得資料模式
            SetGetDataModeEnable_9F07(true, byteDataType);

            try
            {
                // 跳過 Frame (僅針對 ADC 和 DV)
                if (cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_ADC || 
                    cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_DV)
                {
                    if (!SkipFrames_9F07(cGetDataInfo, byteDataBuffer_Array, nRawDataLength, nSkipFrameCount, ref nFrameIndex))
                    {
                        SetGetDataModeEnable_9F07(false);
                        return false;
                    }
                }

                // 取得實際資料
                nFrameIndex = 0;

                if (!GetActualFrames_9F07(cGetDataInfo, byteDataBuffer_Array, nRawDataLength, nFrameCount, ref nFrameIndex))
                {
                    SetGetDataModeEnable_9F07(false);
                    return false;
                }
            }
            catch
            {
                SetGetDataModeEnable_9F07(false);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 跳過指定數量的Frame資料 (9F07版本),持續讀取並捨棄資料直到達到指定的跳過Frame數量,用於穩定資料擷取前的暖機
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含資料類型、索引、總數及階段訊息等</param>
        /// <param name="byteDataBuffer_Array">用於接收資料的位元組緩衝區陣列</param>
        /// <param name="nRawDataLength">原始資料長度(未使用於此方法)</param>
        /// <param name="nSkipFrameCount">需要跳過的Frame總數量</param>
        /// <param name="nFrameIndex">目前已跳過的Frame索引,以參考方式傳遞並更新</param>
        /// <returns>成功跳過所有Frame回傳true;若使用者中斷執行則回傳false</returns>
        private bool SkipFrames_9F07(GetDataInfo cGetDataInfo, byte[] byteDataBuffer_Array, int nRawDataLength, int nSkipFrameCount, ref int nFrameIndex)
        {
            while (true)
            {
                if (!m_cfrmParent.m_bExecute) return false;

                int nResultFlag = ElanTouchSwitch.ReadDevData(byteDataBuffer_Array, 0, m_bSocketConnectType, 10);

                if (!ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType))
                    continue;

                byte nCategory = byteDataBuffer_Array[0];
                if (nCategory != 0xFC) continue;

                nFrameIndex++;

                string sProgressMessage = string.Format("{0}Set : {1}/{2} ", cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex, cGetDataInfo.m_nListCount) +
                                         string.Format("({0} Skip Frame : {1}/{2})", cGetDataInfo.m_sDataType, nFrameIndex, nSkipFrameCount);

                if ((nFrameIndex % 50) == 0 || nFrameIndex >= nSkipFrameCount)
                {
                    // 複製值到本地變數
                    int localFrameIndex = nFrameIndex;
                    int localSkipFrameCount = nSkipFrameCount;

                    m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                    {
                        m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, sProgressMessage);
                        SetDisplayFrameNumber(string.Format("Skip {0}", cGetDataInfo.m_sDataType), localFrameIndex, localSkipFrameCount);
                    });
                }

                m_cDebugLog.WriteLogToBuffer(sProgressMessage);

                if (nFrameIndex >= nSkipFrameCount)
                    break;
            }

            return true;
        }

        /// <summary>
        /// 取得實際的Frame資料 (9F07版本),持續讀取指定數量的Frame並將位元組資料轉換為二維陣列格式儲存,同時更新進度顯示
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含Frame資料陣列、RX/TX追蹤線數量、資料類型、索引等參數</param>
        /// <param name="byteDataBuffer_Array">用於接收資料的位元組緩衝區陣列</param>
        /// <param name="nRawDataLength">原始資料的位元組長度</param>
        /// <param name="nFrameCount">需要取得的Frame總數量</param>
        /// <param name="nFrameIndex">目前已取得的Frame索引,以參考方式傳遞並更新</param>
        /// <returns>成功取得所有Frame回傳true;若使用者中斷執行則回傳false</returns>
        private bool GetActualFrames_9F07(GetDataInfo cGetDataInfo, byte[] byteDataBuffer_Array, int nRawDataLength, int nFrameCount, ref int nFrameIndex)
        {
            while (true)
            {
                if (!m_cfrmParent.m_bExecute) return false;

                int nResultFlag = ElanTouchSwitch.ReadDevData(byteDataBuffer_Array, 0, m_bSocketConnectType, 10);

                if (!ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType))
                    continue;

                byte nCategory = byteDataBuffer_Array[0];
                if (nCategory != 0xFC) continue;

                // 複製資料到 Frame Array
                byte[] byteRawFrame_Array = new byte[nRawDataLength];
                Array.Clear(byteRawFrame_Array, 0, byteRawFrame_Array.Length);
                Array.Copy(byteDataBuffer_Array, 1, byteRawFrame_Array, 0, nRawDataLength);

                // 轉換為 2D 陣列
                int nCount = 0;
                for (int nY = 1; nY <= cGetDataInfo.m_nTXTraceNumber; nY++)
                {
                    for (int nX = 1; nX <= cGetDataInfo.m_nRXTraceNumber; nX++)
                    {
                        cGetDataInfo.m_nFrameData_Array[nFrameIndex, nY, nX] = 
                            (short)(byteRawFrame_Array[nCount * 2] | (byteRawFrame_Array[nCount * 2 + 1] << 8));
                        nCount++;
                    }
                }

                nFrameIndex++;

                string sProgressMessage = string.Format("{0}Set : {1}/{2} ", cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex, cGetDataInfo.m_nListCount) +
                                         string.Format("({0} Get Frame : {1}/{2})", cGetDataInfo.m_sDataType, nFrameIndex, nFrameCount);

                if ((nFrameIndex % 50) == 0 || nFrameIndex >= nFrameCount)
                {
                    // 複製值到本地變數
                    int localFrameIndex = nFrameIndex;
                    int localFrameCount = nFrameCount;

                    m_cfrmParent.BeginInvoke((MethodInvoker)delegate
                    {
                        m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, sProgressMessage);
                        SetDisplayFrameNumber(string.Format("Get {0}", cGetDataInfo.m_sDataType), localFrameIndex, localFrameCount);
                    });
                }

                m_cDebugLog.WriteLogToBuffer(sProgressMessage);

                if (nFrameIndex >= nFrameCount)
                {
                    SetGetDataModeEnable_9F07(false);
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// 將資料類型列舉轉換為對應的位元組值 (9F07版本),用於通訊協定中指定要取得的資料類型
        /// </summary>
        /// <param name="eRawDataType">原始資料類型列舉(Type_dV、Type_ADC或Type_Base)</param>
        /// <returns>回傳對應的位元組值:0x00(dV)、0x01(ADC)、0x02(Base),預設為0x00</returns>
        private byte GetDataTypeByte_9F07(UserInterfaceDefine.RawDataType eRawDataType)
        {
            if (eRawDataType == UserInterfaceDefine.RawDataType.Type_dV)
                return 0x00;
            else if (eRawDataType == UserInterfaceDefine.RawDataType.Type_ADC)
                return 0x01;
            else if (eRawDataType == UserInterfaceDefine.RawDataType.Type_Base)
                return 0x02;
            
            return 0x00;
        }
#endif

        #endregion

        #region 參數設定與判斷

        /// <summary>
        /// 根據連線類型、資料類型及掃描模式設定資料擷取的相關參數,包含重試次數、超時時間、原始資料類型及掃描類型
        /// </summary>
        /// <param name="nRetryCount">重試次數,依據連線類型(Chrome或一般)設定不同值,以參考方式傳遞並更新</param>
        /// <param name="nTimeout">超時時間(毫秒),依據連線類型(Chrome、SSH或一般)設定不同值,以參考方式傳遞並更新</param>
        /// <param name="eRawDataType">原始資料類型列舉,根據資料類型字串決定,以參考方式傳遞並更新</param>
        /// <param name="byteScanType">掃描類型位元組,根據是否為Self模式設定為MUTUAL或MUTUALSELF,以參考方式傳遞並更新</param>
        /// <param name="nRXTotalTraceNumber">RX追蹤線總數量,以參考方式傳遞(此方法中未修改)</param>
        /// <param name="nTXTotalTraceNumber">TX追蹤線總數量,以參考方式傳遞(此方法中未修改)</param>
        /// <param name="sDataType">資料類型字串(如ADC、DV、BASE等),用於判斷原始資料類型</param>
        /// <param name="nRXTraceNumber">RX追蹤線數量(此方法中未使用)</param>
        /// <param name="nTXTraceNumber">TX追蹤線數量(此方法中未使用)</param>
        /// <param name="bGetSelf">是否取得Self模式資料的旗標,true時設定為MUTUALSELF掃描類型</param>
        private void SetGetDataParameter(
            ref int nRetryCount, ref int nTimeout, 
            ref UserInterfaceDefine.RawDataType eRawDataType, ref byte byteScanType,
            ref int nRXTotalTraceNumber, ref int nTXTotalTraceNumber, 
            string sDataType, int nRXTraceNumber, int nTXTraceNumber, bool bGetSelf)
        {
            // 設定重試次數
            if (IsChromeSocketType())
                nRetryCount = ParamFingerAutoTuning.m_nChromeGetDataRetryCount;
            else
                nRetryCount = ParamFingerAutoTuning.m_nGetDataRetryCount;

            // 設定超時時間
            if (IsChromeSocketType())
                nTimeout = ParamFingerAutoTuning.m_nChromeGetDataTimeout;
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                nTimeout = ParamFingerAutoTuning.m_nSSHSocketServerGetDataTimeout;

            // 設定資料類型
            eRawDataType = DetermineRawDataType(sDataType);

            // 設定掃描類型
            byteScanType = bGetSelf ? AppCoreDefine.m_byteMODE_MUTUALSELF : AppCoreDefine.m_byteMODE_MUTUAL;
        }

        /// <summary>
        /// 判斷是否為 Chrome Socket 類型
        /// </summary>
        private bool IsChromeSocketType()
        {
            return ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT ||
                   ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER;
        }

        /// <summary>
        /// 判斷指定的資料類型是否應該只取得單一Frame(Frame數量為1)
        /// </summary>
        /// <param name="sDataType">資料類型字串(BASE、KPKN或OBASE)</param>
        /// <returns>若資料類型為BASE、KPKN或OBASE則回傳true,表示只需取得1個Frame;否則回傳false</returns>
        private bool IsFrameNumberOne(string sDataType)
        {
            return sDataType == MainConstantParameter.m_sDATATYPE_BASE ||
                   sDataType == MainConstantParameter.m_sDATATYPE_KPKN ||
                   sDataType == MainConstantParameter.m_sDATATYPE_OBASE;
        }

        /// <summary>
        /// 根據資料類型字串判斷並回傳對應的原始資料類型列舉值
        /// </summary>
        /// <param name="sDataType">資料類型字串(如BASE、OBASE、ADC、DV等)</param>
        /// <returns>回傳對應的RawDataType列舉:Type_Base(BASE/OBASE)、Type_ADC(ADC)或Type_dV(其他類型,預設值)</returns>
        private UserInterfaceDefine.RawDataType DetermineRawDataType(string sDataType)
        {
            if (sDataType == MainConstantParameter.m_sDATATYPE_BASE ||
                sDataType == MainConstantParameter.m_sDATATYPE_OBASE)
                return UserInterfaceDefine.RawDataType.Type_Base;
            else if (sDataType == MainConstantParameter.m_sDATATYPE_ADC)
                return UserInterfaceDefine.RawDataType.Type_ADC;
            else
                return UserInterfaceDefine.RawDataType.Type_dV;
        }

        /// <summary>
        /// 判斷在取得資料前是否需要跳過Frame,用於穩定資料擷取,適用於DV、ADC及特定條件下的BASE資料類型
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含資料類型等參數</param>
        /// <returns>若資料類型為DV或ADC,或在特定IC世代(Gen8/非Gen8)且參數啟用時的BASE類型,則回傳true表示需要跳過Frame;否則回傳false</returns>
        private bool ShouldSkipFrames(GetDataInfo cGetDataInfo)
        {
            return cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_DV ||
                   cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_ADC ||
                   (m_eICGenerationType == ICGenerationType.Gen8 &&
                    cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                    ParamFingerAutoTuning.m_nGen8GetBaseUseSkipFrame == 1) ||
                   (m_eICGenerationType != ICGenerationType.Gen8 &&
                    cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                    ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseUseSkipFrame == 1);
        }

        /// <summary>
        /// 取得真實的資料類型顯示名稱,針對Raw ADC Sweep模式在特定IC世代下添加"(Noise)"標註
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含資料類型及Raw ADC Sweep旗標</param>
        /// <returns>回傳資料類型字串,若為Raw ADC Sweep且非Gen6或Gen7(未啟用HWTXN)則添加"(Noise)"後綴;否則回傳原始資料類型</returns>
        private string GetRealDataType(GetDataInfo cGetDataInfo)
        {
            string sRealDataType = cGetDataInfo.m_sDataType;

            if (cGetDataInfo.m_bRawADCSweep == true)
            {
                sRealDataType = string.Format("{0}(Noise)", cGetDataInfo.m_sDataType);

                if (m_eICGenerationType == ICGenerationType.Gen7 && ParamFingerAutoTuning.m_nRawADCSGen7EnableHWTXN == 0)
                    sRealDataType = cGetDataInfo.m_sDataType;
                else if (m_eICGenerationType == ICGenerationType.Gen6)
                    sRealDataType = cGetDataInfo.m_sDataType;
            }

            return sRealDataType;
        }

        /// <summary>
        /// 建立取得資料時的訊息字串,包含資料類型、Frame數量,若為Self模式且設定K參數時則額外顯示NCP及NCN值
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含Frame數量、Self模式旗標及K參數設定旗標</param>
        /// <param name="sRealDataType">真實的資料類型名稱字串</param>
        /// <returns>回傳格式化的訊息字串,格式為"Get {資料類型} Data[FrameCount={數量}]",Self模式時額外加上"(NCP={值} NCN={值})"</returns>
        private string BuildGetDataMessage(GetDataInfo cGetDataInfo, string sRealDataType)
        {
            string sMessage = string.Format("Get {0} Data[FrameCount={1}]", sRealDataType, cGetDataInfo.m_nFrameNumber);

            if (cGetDataInfo.m_bGetSelf == true && cGetDataInfo.m_bSetKParameter == true)
                sMessage = string.Format("{0} (NCP={1} NCN={2})", sMessage, m_nSelfNCPValue, m_nSelfNCNValue);

            return sMessage;
        }

        /// <summary>
        /// 根據掃描模式建立適當大小的資料緩衝區陣列,Self模式需要額外的空間(+2),一般模式為+1
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含RX/TX追蹤線數量及Self模式旗標</param>
        /// <returns>回傳整數陣列作為資料緩衝區,大小為(TX+2)*(RX+2)(Self模式)或(TX+1)*(RX+1)(一般模式)</returns>
        private int[] CreateDataBuffer(GetDataInfo cGetDataInfo)
        {
            int[] nDataBuffer_Array;

            if (cGetDataInfo.m_bGetSelf == true)
                nDataBuffer_Array = new int[(cGetDataInfo.m_nTXTraceNumber + 2) * (cGetDataInfo.m_nRXTraceNumber + 2)];
            else
                nDataBuffer_Array = new int[(cGetDataInfo.m_nTXTraceNumber + 1) * (cGetDataInfo.m_nRXTraceNumber + 1)];

            return nDataBuffer_Array;
        }

        #endregion

        #region 取得資料流程處理

        /// <summary>
        /// 處理資料擷取失敗時的重試重新連線流程,執行重新連線並針對Gen8 IC重新設定韌體參數
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含流程步驟、頻率項目、Raw ADC Sweep項目及旗標</param>
        /// <returns>Gen8 IC且韌體參數設定成功,或非Gen8 IC時回傳true;韌體參數設定失敗則回傳false</returns>
        private bool HandleRetryReconnect(GetDataInfo cGetDataInfo)
        {
            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ReConnect);

            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                if (cGetDataInfo.m_bRawADCSweep == true)
                {
                    if (!SetAndGetFWParameter(cGetDataInfo.m_cFlowStep, null, cGetDataInfo.m_cRawADCSweepItem))
                        return false;
                }
                else
                {
                    if (!SetAndGetFWParameter(cGetDataInfo.m_cFlowStep, cGetDataInfo.m_cFrequencyItem))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 處理Gen8 IC在取得Base或OBASE資料前的延遲等待,用於確保資料穩定性,執行退出測試模式並等待指定時間
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含資料類型及取得Base延遲旗標</param>
        /// <returns>固定回傳true(此方法不會失敗)</returns>
        private bool HandleGen8BaseDelay(GetDataInfo cGetDataInfo)
        {
            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                if ((cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE ||
                     cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_OBASE) &&
                    cGetDataInfo.m_bGetBaseDelay == true &&
                    ParamFingerAutoTuning.m_nGen8GetFirstBaseDelay == 1)
                {
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                    Thread.Sleep(ParamFingerAutoTuning.m_nGen8GetFirstBaseDelayTime);
                }
            }

            return true;
        }

        /// <summary>
        /// 在取得資料前執行測試模式進入的準備流程,依據IC世代(Gen8/非Gen8)、連線類型及參數設定執行不同的初始化步驟序列
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含資料類型、Raw ADC Sweep旗標及Self模式旗標</param>
        private void PrepareBeforeGetData(GetDataInfo cGetDataInfo)
        {
            if (m_eICGenerationType != ICGenerationType.Gen8)
            {
                if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterKPKNCommand, cGetDataInfo.m_sDataType);

                if (cGetDataInfo.m_bRawADCSweep == false)
                {
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data);
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);
                }

                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    Thread.Sleep(2000);
                else
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_TransferTestModeViaHID);
            }
            else
            {
                if (ParamFingerAutoTuning.m_nGen8SetGetDataInfoType == 1)
                {
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterKPKNCommand, cGetDataInfo.m_sDataType);
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data, null, cGetDataInfo.m_bGetSelf);
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data, null, cGetDataInfo.m_bGetSelf);
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_TransferTestModeViaHID);
                    Thread.Sleep(30);
                }
                else
                {
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);
                    Thread.Sleep(30);
                }
            }

            /*
            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_SSHSOCKETSERVER)
                Thread.Sleep(1000);
            */
        }

        /// <summary>
        /// 跳過指定數量的Frame資料以穩定資料擷取,包含重試機制、新方法設定及進度更新,失敗時執行測試模式退出
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含資料類型、RX/TX追蹤線數量、索引及新方法使用旗標</param>
        /// <param name="bRetryFlag">重試旗標,當需要重試時設為true,以參考方式傳遞並更新</param>
        /// <param name="nRetryCount">目前重試次數,以參考方式傳遞並更新</param>
        /// <param name="nRETRYCOUNTTIMES">最大重試次數限制</param>
        /// <param name="nDataBuffer_Array">用於接收資料的緩衝區陣列</param>
        /// <param name="byteScanType">掃描類型位元組(MUTUAL或MUTUALSELF)</param>
        /// <param name="nTimeout">取得資料的超時時間(毫秒)</param>
        /// <returns>成功跳過所有Frame回傳true;若使用者中斷執行、新方法設定失敗或資料取得失敗則回傳false</returns>
        private bool SkipFrames(
            GetDataInfo cGetDataInfo, ref bool bRetryFlag, ref int nRetryCount, 
            int nRETRYCOUNTTIMES, int[] nDataBuffer_Array, byte byteScanType, int nTimeout)
        {
            int nFrameCount = ParamFingerAutoTuning.m_nSkipFrame;

            if (cGetDataInfo.m_bUseNewMethod == true)
                nFrameCount = 1;

            string sDataType = GetSkipFrameDataType(cGetDataInfo);

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                SetDisplayFrameNumber(string.Format("Skip {0}", sDataType), 0, nFrameCount);
            });

            for (int nFrameIndex = 0; nFrameIndex < nFrameCount; nFrameIndex++)
            {
                if (!m_cfrmParent.m_bExecute) return false;

                bool bGetData = false;
                bool bSetUseNewMethodSuccess = true;

                for (int nSubRetryIndex = 0; nSubRetryIndex <= 3; nSubRetryIndex++)
                {
                    if (!m_cfrmParent.m_bExecute) return false;

                    // 使用新方法設定
                    if (cGetDataInfo.m_bUseNewMethod == true)
                    {
                        bSetUseNewMethodSuccess = SetUseNewMethodByGeneration(ParamFingerAutoTuning.m_nSkipFrame, 3000);
                        if (!bSetUseNewMethodSuccess) break;
                    }

                    Array.Clear(nDataBuffer_Array, 0, nDataBuffer_Array.Length);
                    int nResult = GetDataMainStep(ref nDataBuffer_Array, ref nRetryCount, ref bGetData, cGetDataInfo, sDataType,
                                                  cGetDataInfo.m_nRXTraceNumber, cGetDataInfo.m_nTXTraceNumber, byteScanType,
                                                  nTimeout, cGetDataInfo.m_nListIndex, nFrameIndex, 0, true);

                    if (ElanTouchSwitch.CheckTPState(nResult, m_bSocketConnectType) == true)
                    {
                        bGetData = true;
                        break;
                    }

                    Thread.Sleep(10);
                }

                if (!bGetData)
                {
                    nRetryCount++;

                    if (nRetryCount <= nRETRYCOUNTTIMES)
                    {
                        bRetryFlag = true;
                        break;
                    }
                }

                // 更新進度訊息
                UpdateSkipFrameProgress(cGetDataInfo, sDataType, nFrameIndex, nFrameCount);

                if (!bSetUseNewMethodSuccess || !bGetData)
                {
                    ExitTestModeWithError(cGetDataInfo, sDataType, bSetUseNewMethodSuccess);
                    return false;
                }

                Thread.Sleep(10);
            }

            return true;
        }

        /// <summary>
        /// 取得跳過Frame時實際使用的資料類型,當BASE類型在特定IC世代且參數啟用時會轉換為ADC類型進行跳過
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含資料類型</param>
        /// <returns>回傳資料類型字串,若為BASE且符合Gen8或非Gen8的特定參數設定則回傳ADC;否則回傳原始資料類型</returns>
        private string GetSkipFrameDataType(GetDataInfo cGetDataInfo)
        {
            string sDataType = cGetDataInfo.m_sDataType;

            if ((m_eICGenerationType == ICGenerationType.Gen8 &&
                cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                ParamFingerAutoTuning.m_nGen8GetBaseUseSkipFrame == 1) ||
                (m_eICGenerationType != ICGenerationType.Gen8 &&
                cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseUseSkipFrame == 1))
                sDataType = MainConstantParameter.m_sDATATYPE_ADC;

            return sDataType;
        }

        /// <summary>
        /// 根據IC世代類型選擇並執行對應的新方法設定,Gen8使用專屬方法,其他世代使用通用方法
        /// </summary>
        /// <param name="nFrameNumber">要設定的Frame數量</param>
        /// <param name="nTimeout">設定操作的超時時間(毫秒)</param>
        /// <returns>新方法設定成功回傳true;設定失敗則回傳false</returns>
        private bool SetUseNewMethodByGeneration(int nFrameNumber, int nTimeout)
        {
            if (m_eICGenerationType == ICGenerationType.Gen8)
                return SetUseNewMethod_Gen8(nFrameNumber, nTimeout);
            else
                return SetUseNewMethod(nFrameNumber, nTimeout);
        }

        /// <summary>
        /// 更新跳過Frame的進度訊息並顯示於UI及記錄至Log,包含當前執行索引、Frame進度,Self模式時額外顯示NCP/NCN參數
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含階段訊息、索引、總數、Self模式旗標及K參數設定旗標</param>
        /// <param name="sDataType">資料類型字串</param>
        /// <param name="nFrameIndex">當前Frame索引(從0開始)</param>
        /// <param name="nFrameCount">總Frame數量</param>
        private void UpdateSkipFrameProgress(GetDataInfo cGetDataInfo, string sDataType, int nFrameIndex, int nFrameCount)
        {
            string sProgressMessage = string.Format("{0}Set : {1}/{2} ", cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex, cGetDataInfo.m_nListCount) +
                                     string.Format("({0} Skip Frame : {1}/{2})", sDataType, nFrameIndex + 1, nFrameCount);

            if (cGetDataInfo.m_bGetSelf == true && cGetDataInfo.m_bSetKParameter == true)
                sProgressMessage = string.Format("{0} (NCP={1} NCN={2})", sProgressMessage, m_nSelfNCPValue, m_nSelfNCNValue);

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, sProgressMessage);
                SetDisplayFrameNumber(string.Format("Skip {0}", sDataType), nFrameIndex + 1, nFrameCount);
            });

            m_cDebugLog.WriteLogToBuffer(sProgressMessage);
        }

        /// <summary>
        /// 處理跳過Frame後針對BASE資料類型的特殊設定,依據IC世代(Gen8/非Gen8)及參數啟用狀態執行對應的後續處理
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含資料類型</param>
        private void HandleAfterSkipFrames(GetDataInfo cGetDataInfo)
        {
            if (m_eICGenerationType == ICGenerationType.Gen8 &&
                cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                ParamFingerAutoTuning.m_nGen8GetBaseUseSkipFrame == 1)
            {
                HandleGen8AfterSkipFrames(cGetDataInfo);
            }

            if (m_eICGenerationType != ICGenerationType.Gen8 &&
                cGetDataInfo.m_sDataType == MainConstantParameter.m_sDATATYPE_BASE &&
                ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseUseSkipFrame == 1)
            {
                HandleNonGen8AfterSkipFrames(cGetDataInfo);
            }
        }

        /// <summary>
        /// 處理Gen8 IC在跳過Frame後針對BASE資料類型的測試模式切換與參數設定,依據取得資料資訊類型及連線類型執行不同的步驟序列
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含Self模式旗標及Raw ADC Sweep旗標</param>
        private void HandleGen8AfterSkipFrames(GetDataInfo cGetDataInfo)
        {
            if (ParamFingerAutoTuning.m_nGen8SetGetDataInfoType == 1)
            {
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, MainConstantParameter.m_sDATATYPE_ADC);
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data, null, cGetDataInfo.m_bGetSelf);
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data, null, cGetDataInfo.m_bGetSelf);
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_TransferTestModeViaHID);
                Thread.Sleep(30);
            }
            else
            {
                if (cGetDataInfo.m_bRawADCSweep == false)
                {
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);

                    if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                        GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, MainConstantParameter.m_sDATATYPE_ADC);

                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);

                    if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                        Thread.Sleep(2000);
                    else
                        Thread.Sleep(30);
                }
            }
        }

        /// <summary>
        /// 處理非Gen8 IC在跳過Frame後針對BASE資料類型的測試模式切換,依據連線類型執行退出KPKN命令與測試模式重新進入
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含Raw ADC Sweep旗標</param>
        private void HandleNonGen8AfterSkipFrames(GetDataInfo cGetDataInfo)
        {
            if (cGetDataInfo.m_bRawADCSweep == false)
            {
                if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                {
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);
                    GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, MainConstantParameter.m_sDATATYPE_ADC);
                }

                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);

                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                    Thread.Sleep(2000);
                else
                    Thread.Sleep(30);
            }
        }

        /// <summary>
        /// 取得指定數量的Frame資料,包含重試機制、新方法設定、進度更新、資料複製及佇列儲存,失敗時執行測試模式退出
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含資料類型、Frame數量、RX/TX追蹤線數量、索引及新方法使用旗標</param>
        /// <param name="bRetryFlag">重試旗標,當需要重試時設為true,以參考方式傳遞並更新</param>
        /// <param name="nFrameValue">當前Frame值,用於記錄重試時的起始Frame索引,以參考方式傳遞並更新</param>
        /// <param name="nRetryCount">目前重試次數,以參考方式傳遞並更新</param>
        /// <param name="nRETRYCOUNTTIMES">最大重試次數限制</param>
        /// <param name="nDataBuffer_Array">用於接收單一Frame資料的緩衝區陣列</param>
        /// <param name="nRXTotalTraceNumber">RX追蹤線總數量</param>
        /// <param name="nTXTotalTraceNumber">TX追蹤線總數量</param>
        /// <param name="byteScanType">掃描類型位元組(MUTUAL或MUTUALSELF)</param>
        /// <param name="nTimeout">取得資料的超時時間(毫秒)</param>
        /// <param name="eRawDataType">原始資料類型列舉,用於儲存至佇列時的類型識別</param>
        /// <param name="nFrameBuffer_Array">Frame緩衝區陣列,用於儲存Normal Raw Data時的暫存,以參考方式傳遞並更新</param>
        /// <param name="nStartTime">開始時間戳記(Ticks),用於計算時間戳記,以參考方式傳遞並更新</param>
        /// <returns>成功取得所有Frame回傳true;若使用者中斷執行、新方法設定失敗或資料取得失敗則回傳false</returns>
        private bool GetFrames(
            GetDataInfo cGetDataInfo, ref bool bRetryFlag, ref int nFrameValue, 
            ref int nRetryCount, int nRETRYCOUNTTIMES, int[] nDataBuffer_Array,
            int nRXTotalTraceNumber, int nTXTotalTraceNumber, byte byteScanType, 
            int nTimeout, UserInterfaceDefine.RawDataType eRawDataType,
            ref int[,] nFrameBuffer_Array, ref long nStartTime)
        {
            #region Get n Frames
            if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
            {
                m_bqcFrameQueue.Clear();

                //Get the start tick. Use to compute the timestamp.
                nStartTime = DateTime.Now.Ticks;

                Thread.Sleep(ElanDefine.TIME_100MS);
            }

            int nFrameCount = cGetDataInfo.m_nFrameNumber;

            if (cGetDataInfo.m_bUseNewMethod == true)
                nFrameCount = 1;

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                SetDisplayFrameNumber(string.Format("Get {0}", cGetDataInfo.m_sDataType), 0, nFrameCount);
            });

            for (int nFrameIndex = nFrameValue; nFrameIndex < nFrameCount; nFrameIndex++)
            {
                if (!m_cfrmParent.m_bExecute) return false;

                bool bGetData = false;
                bool bSetUseNewMethodSuccess = true;

                for (int nSubRetryIndex = 0; nSubRetryIndex <= 3; nSubRetryIndex++)
                {
                    if (!m_cfrmParent.m_bExecute) return false;

                    if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                    {
                        nFrameBuffer_Array = CreateFrameBuffer(cGetDataInfo);
                        Array.Clear(nFrameBuffer_Array, 0, nFrameBuffer_Array.Length);
                    }

                    if (cGetDataInfo.m_bUseNewMethod == true)
                    {
                        bSetUseNewMethodSuccess = SetUseNewMethodByGeneration(cGetDataInfo.m_nFrameNumber, 3000);
                        if (!bSetUseNewMethodSuccess) break;
                    }

                    Array.Clear(nDataBuffer_Array, 0, nDataBuffer_Array.Length);
                    int nResult = GetDataMainStep(
                        ref nDataBuffer_Array, ref nRetryCount, ref bGetData, cGetDataInfo, cGetDataInfo.m_sDataType,
                        nRXTotalTraceNumber, nTXTotalTraceNumber, byteScanType, nTimeout, cGetDataInfo.m_nListIndex,
                        nFrameIndex, 0
                    );

                    if (ElanTouchSwitch.CheckTPState(nResult, m_bSocketConnectType) == true)
                    {
                        bGetData = true;
                        break;
                    }

                    Thread.Sleep(10);
                }

                if (!bGetData)
                {
                    nRetryCount++;

                    if (nRetryCount <= nRETRYCOUNTTIMES)
                    {
                        bRetryFlag = true;
                        nFrameValue = nFrameIndex;
                        break;
                    }
                }

                // 更新進度訊息
                UpdateGetFrameProgress(cGetDataInfo, nFrameIndex, nFrameCount);

                if (!bSetUseNewMethodSuccess || !bGetData)
                {
                    ExitTestModeWithError(cGetDataInfo, cGetDataInfo.m_sDataType, bSetUseNewMethodSuccess);
                    return false;
                }

                // 複製資料
                CopyFrameData(cGetDataInfo, nDataBuffer_Array, nFrameIndex);

                // 處理儲存資料
                if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                {
                    SaveFrameToQueue(cGetDataInfo, nDataBuffer_Array, nFrameBuffer_Array, eRawDataType, nStartTime);
                    Thread.Sleep(ElanDefine.TIME_1MS * 10);
                }
                else
                {
                    Thread.Sleep(20);
                }
            }
            #endregion

            return true;
        }

        /// <summary>
        /// 根據掃描模式建立適當大小的Frame緩衝區二維陣列,Self模式需要額外的空間(+2),一般模式為+1
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含RX/TX追蹤線數量及Self模式旗標</param>
        /// <returns>回傳二維整數陣列作為Frame緩衝區,大小為[TX+2, RX+2](Self模式)或[TX+1, RX+1](一般模式)</returns>
        private int[,] CreateFrameBuffer(GetDataInfo cGetDataInfo)
        {
            if (cGetDataInfo.m_bGetSelf == true)
                return new int[cGetDataInfo.m_nTXTraceNumber + 2, cGetDataInfo.m_nRXTraceNumber + 2];
            else
                return new int[cGetDataInfo.m_nTXTraceNumber + 1, cGetDataInfo.m_nRXTraceNumber + 1];
        }

        /// <summary>
        /// 更新取得Frame的進度訊息並顯示於UI及記錄至Log,包含當前執行索引、Frame進度,Self模式時額外顯示NCP/NCN參數
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含階段訊息、資料類型、索引、總數、Self模式旗標及K參數設定旗標</param>
        /// <param name="nFrameIndex">當前Frame索引(從0開始)</param>
        /// <param name="nFrameCount">總Frame數量</param>
        private void UpdateGetFrameProgress(GetDataInfo cGetDataInfo, int nFrameIndex, int nFrameCount)
        {
            string sProgressMessage = string.Format("{0}Set : {1}/{2} ", cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex, cGetDataInfo.m_nListCount) +
                                     string.Format("({0} Get Frame : {1}/{2})", cGetDataInfo.m_sDataType, nFrameIndex + 1, nFrameCount);

            if (cGetDataInfo.m_bGetSelf == true && cGetDataInfo.m_bSetKParameter == true)
                sProgressMessage = string.Format("{0} (NCP={1} NCN={2})", sProgressMessage, m_nSelfNCPValue, m_nSelfNCNValue);

            m_cfrmParent.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmParent.UpdatestatusstripMessage(frmMain.ProgressUpdate.JustLabel, 0, sProgressMessage);
                SetDisplayFrameNumber(string.Format("Get {0}", cGetDataInfo.m_sDataType), nFrameIndex + 1, nFrameCount);
            });

            m_cDebugLog.WriteLogToBuffer(sProgressMessage);
        }

        /// <summary>
        /// 將單一Frame的緩衝區資料複製到Frame資料陣列中,根據Self模式或一般模式使用不同的陣列大小參數
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含Frame資料陣列、RX/TX追蹤線數量及Self模式旗標</param>
        /// <param name="nDataBuffer_Array">來源資料緩衝區一維陣列</param>
        /// <param name="nFrameIndex">目標Frame索引,指定要複製到三維陣列的哪一個Frame</param>
        private void CopyFrameData(GetDataInfo cGetDataInfo, int[] nDataBuffer_Array, int nFrameIndex)
        {
            if (cGetDataInfo.m_bGetSelf == true)
                CopyArray(nDataBuffer_Array, cGetDataInfo.m_nFrameData_Array, nFrameIndex, 
                         cGetDataInfo.m_nRXTraceNumber + 1, cGetDataInfo.m_nTXTraceNumber + 1, 
                         cGetDataInfo.m_nRXTraceNumber + 2);
            else
                CopyArray(nDataBuffer_Array, cGetDataInfo.m_nFrameData_Array, nFrameIndex, 
                         cGetDataInfo.m_nRXTraceNumber, cGetDataInfo.m_nTXTraceNumber, 
                         cGetDataInfo.m_nRXTraceNumber + 1);
        }

        /// <summary>
        /// 將Frame資料儲存到佇列中,包含資料複製、時間戳記計算及Frame物件建立入列,用於Normal Raw Data類型的儲存處理
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含RX/TX追蹤線數量及Self模式旗標</param>
        /// <param name="nDataBuffer_Array">來源資料緩衝區一維陣列</param>
        /// <param name="nFrameBuffer_Array">Frame緩衝區二維陣列,用於暫存複製的資料</param>
        /// <param name="eRawDataType">原始資料類型列舉,用於Frame物件的類型識別</param>
        /// <param name="nStartTime">開始時間戳記(Ticks),用於計算相對時間間隔</param>
        private void SaveFrameToQueue(
            GetDataInfo cGetDataInfo, int[] nDataBuffer_Array, int[,] nFrameBuffer_Array,
            UserInterfaceDefine.RawDataType eRawDataType, long nStartTime)
        {
            int nXLength = cGetDataInfo.m_nRXTraceNumber;
            int nYLength = cGetDataInfo.m_nTXTraceNumber;

            if (cGetDataInfo.m_bGetSelf == true)
            {
                nXLength = cGetDataInfo.m_nRXTraceNumber + 1;
                nYLength = cGetDataInfo.m_nTXTraceNumber + 1;
            }

            CopyArray(nDataBuffer_Array, nFrameBuffer_Array, nXLength, nYLength, nXLength + 1);

            //Get the timestamp
            long nTimeInterval = ((DateTime.Now.Ticks - nStartTime) / 10000);
            m_bqcFrameQueue.Enqueue(new Frame(nFrameBuffer_Array, nXLength, nYLength, cGetDataInfo.m_bGetSelf, eRawDataType, nTimeInterval));
        }

        /// <summary>
        /// 退出測試模式並根據失敗原因設定對應的錯誤訊息,用於資料擷取失敗時的清理與錯誤記錄
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含階段訊息及索引</param>
        /// <param name="sDataType">資料類型字串,用於錯誤訊息描述</param>
        /// <param name="bSetUseNewMethodSuccess">新方法設定是否成功的旗標,false時表示設定失敗,true時表示取得資料失敗</param>
        private void ExitTestModeWithError(GetDataInfo cGetDataInfo, string sDataType, bool bSetUseNewMethodSuccess)
        {
            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);

            if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, sDataType);

            if (!bSetUseNewMethodSuccess)
                m_sErrorMessage = string.Format("Set UseNewMethod Fail in {0}Set:{1}", cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex);
            else
                m_sErrorMessage = string.Format("Get {0} Data Fail in {1}Set:{2}", sDataType, cGetDataInfo.m_sStageMessage, cGetDataInfo.m_nListIndex);
        }

        /// <summary>
        /// 退出測試模式並執行KPKN命令退出步驟(非SSH Socket Server連線類型時)
        /// </summary>
        /// <param name="sDataType">資料類型字串,用於退出KPKN命令的參數</param>
        private void ExitTestMode(string sDataType)
        {
            GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);

            if (ParamFingerAutoTuning.m_sSocketType != MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand, sDataType);
        }

        /// <summary>
        /// 將佇列中的所有Frame資料依序轉移到Frame Manager進行管理,清空Manager後逐一取出佇列資料並加入
        /// </summary>
        private void TransferQueueToFrameManager()
        {
            int nTotalCount = m_bqcFrameQueue.Count;
            m_cFrameMgr.Clear();

            for (int index = 0; index < nTotalCount; index++)
            {
                Frame cFrame = new Frame();

                if (m_bqcFrameQueue.Dequeue(ElanDefine.TIME_100MS, ref cFrame) == false)
                    continue;

                m_cFrameMgr.Add(cFrame);
            }
        }

        #endregion

        #region 取得資料相關步驟

        /// <summary>
        /// 執行資料擷取相關的各種步驟操作,包含進入/退出測試模式、KPKN命令設定、讀取Bulk RAM資料、重新連線及HID傳輸等
        /// </summary>
        /// <param name="eGetDataRelatedStep">要執行的步驟類型列舉</param>
        /// <param name="sDataType">資料類型字串,用於判斷是否需要執行KPKN命令,預設為null</param>
        /// <param name="bSelfModeFlag">Self模式旗標,用於設定讀取Bulk RAM資料時的模式,預設為false</param>
        private void GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep eGetDataRelatedStep, string sDataType = null, bool bSelfModeFlag = false)
        {
            byte[] byteCommand_Array;

            switch (eGetDataRelatedStep)
            {
                case AppCoreDefine.GetDataRelatedStep.Step_EnterKPKNCommand:
                    if (sDataType == MainConstantParameter.m_sDATATYPE_KPKN)
                    {
                        OutputMessage("-Set Enter KPKN Command(0x54, 0x27, 0x01, 0x01)");
                        byteCommand_Array = new byte[] { 0x54, 0x27, 0x01, 0x01 };
                        SendDevCommand(byteCommand_Array);
                        Thread.Sleep(m_nNormalDelayTime);
                    }
                    break;

                case AppCoreDefine.GetDataRelatedStep.Step_SetRead_Bulk_RAM_Data:
                    SetReadBulkRAMData(bSelfModeFlag);
                    break;

                case AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode:
                    SetTestModeEnable(true);
                    break;

                case AppCoreDefine.GetDataRelatedStep.Step_ReConnect:
                    HandleReconnect();
                    break;

                case AppCoreDefine.GetDataRelatedStep.Step_ExitKPKNCommand:
                    if (sDataType == MainConstantParameter.m_sDATATYPE_KPKN)
                    {
                        OutputMessage("-Set Exit KPKN Command(0x54, 0x27, 0x00, 0x01)");
                        byteCommand_Array = new byte[] { 0x54, 0x27, 0x00, 0x01 };
                        SendDevCommand(byteCommand_Array);
                        Thread.Sleep(m_nNormalDelayTime);
                    }
                    break;

                case AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode:
                    SetTestModeEnable(false);
                    break;

                case AppCoreDefine.GetDataRelatedStep.Step_TransferTestModeViaHID:
                    TransferTestModeViaHID();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 設定讀取Bulk RAM資料的命令,根據IC世代(Gen8/非Gen8)及Self模式發送對應的命令位元組,並執行必要的延遲等待
        /// </summary>
        /// <param name="bSelfModeFlag">Self模式旗標,Gen8時用於判斷發送0x02(Self)或0x01(Mutual)模式命令</param>
        private void SetReadBulkRAMData(bool bSelfModeFlag)
        {
            byte[] byteCommand_Array;

            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                if (ParamFingerAutoTuning.m_nGen8ReadBulkRAMDataValue >= 0)
                {
                    OutputMessage(string.Format("-Set Read Bulk RAM Data Value(0x54, 0xBC, 0x00, 0x{0})", ParamFingerAutoTuning.m_nGen8ReadBulkRAMDataValue.ToString("X2")));
                    byteCommand_Array = new byte[] { 0x54, 0xBC, 0x00, (byte)ParamFingerAutoTuning.m_nGen8ReadBulkRAMDataValue };

                    SendDevCommand(byteCommand_Array);
                    Thread.Sleep(m_nNormalDelayTime);
                    Thread.Sleep(ParamFingerAutoTuning.m_nReadBulkRAMDataValueExtraDelayTime);
                }
                else
                {
                    byte byteComboMode = bSelfModeFlag ? (byte)0x02 : (byte)0x01;

                    OutputMessage(string.Format("-Set Read Bulk RAM Data Value(0x54, 0xBC, 0x00, 0x{0})", byteComboMode.ToString("X2")));
                    byteCommand_Array = new byte[] { 0x54, 0xBC, 0x00, byteComboMode };

                    SendDevCommand(byteCommand_Array);
                    Thread.Sleep(m_nNormalDelayTime);
                    Thread.Sleep(ParamFingerAutoTuning.m_nReadBulkRAMDataValueExtraDelayTime);
                }
            }
            else
            {
                OutputMessage("-Set Read Bulk RAM Data Value(0x54, 0xBC, 0x00, 0x00)");
                byteCommand_Array = new byte[] { 0x54, 0xBC, 0x00, 0x00 };

                SendDevCommand(byteCommand_Array);
                Thread.Sleep(m_nNormalDelayTime);
                Thread.Sleep(ParamFingerAutoTuning.m_nReadBulkRAMDataValueExtraDelayTime);
            }
        }

        /// <summary>
        /// 處理與觸控面板的重新連線流程,包含退出測試模式、重置(Gen8)、重新連線、筆功能停用(8F18)及重新進入測試模式
        /// </summary>
        private void HandleReconnect()
        {
            SetTestModeEnable(false);

            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                SetReset();
                Thread.Sleep(ParamFingerAutoTuning.m_nGen8ReconnectDelayTime);
            }

            ReconnectToTP();

            Thread.Sleep(m_nNormalDelayTime);

            SetPenFunctionEnable_8F18(false);

            SetTestModeEnable(true);
        }

        /// <summary>
        /// 透過HID介面傳輸測試模式命令,僅在Gen8 IC且參數啟用時執行,發送固定的命令位元組序列並等待處理完成
        /// </summary>
        private void TransferTestModeViaHID()
        {
            if (m_eICGenerationType == ICGenerationType.Gen8 && ParamFingerAutoTuning.m_nGen8SetTransferTestModeViaHID == 1)
            {
                OutputMessage("-Send TransferTestModeViaHID Command : 0x2D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00");
                ElanTouchSwitch.TransferTestModeViaHID(m_nDeviceIndex, m_bSocketConnectType);
                Thread.Sleep(m_nNormalDelayTime);

                //string sSendCommand = "-Send TransferTestModeViaHID Command : 0x2D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00";
                //m_cDebugLog.WriteLogToBuffer(sSendCommand);
                Thread.Sleep(30);
            }
        }

        #endregion

        #region 取得資料主要步驟

        /// <summary>
        /// 執行取得資料的主要步驟,根據是否使用新方法選擇對應的資料擷取方式,並處理擷取結果的成功或失敗狀態
        /// </summary>
        /// <param name="nDataBuffer_Array">用於接收資料的緩衝區陣列,以參考方式傳遞並更新</param>
        /// <param name="nRetryCount">目前重試次數,成功時重置為0,以參考方式傳遞並更新</param>
        /// <param name="bGetData">資料擷取成功旗標,成功時設為true,以參考方式傳遞並更新</param>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含流程步驟、資料類型等參數</param>
        /// <param name="sDataType">資料類型字串</param>
        /// <param name="nRXTotalTraceNumber">RX追蹤線總數量</param>
        /// <param name="nTXTotalTraceNumber">TX追蹤線總數量</param>
        /// <param name="bTraceType">追蹤類型位元組(實際為掃描類型,MUTUAL或MUTUALSELF)</param>
        /// <param name="nTimeout">取得資料的超時時間(毫秒)</param>
        /// <param name="nListIndex">當前執行的索引編號</param>
        /// <param name="nFrameIndex">當前Frame索引</param>
        /// <param name="nRetryIndex">當前重試索引</param>
        /// <param name="bSkipData">是否為跳過資料模式,預設為false,用於錯誤訊息描述</param>
        /// <returns>回傳資料擷取結果代碼,用於檢查觸控面板狀態</returns>
        private int GetDataMainStep(ref int[] nDataBuffer_Array, ref int nRetryCount, ref bool bGetData, 
                                   GetDataInfo cGetDataInfo, string sDataType, int nRXTotalTraceNumber,
                                   int nTXTotalTraceNumber, byte bTraceType, int nTimeout, int nListIndex, 
                                   int nFrameIndex, int nRetryIndex, bool bSkipData = false)
        {
            int nResult = 0;

            if (cGetDataInfo.m_bUseNewMethod == true)
            {
                nResult = GetDataByNewMethod(ref nDataBuffer_Array, cGetDataInfo, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout);
            }
            else
            {
                nResult = GetDataByDataType(ref nDataBuffer_Array, cGetDataInfo, sDataType, nRXTotalTraceNumber, nTXTotalTraceNumber, bTraceType, nTimeout);
            }

            if (ElanTouchSwitch.CheckTPState(nResult, m_bSocketConnectType) == true)
            {
                nRetryCount = 0;
                bGetData = true;
            }
            else
            {
                HandleGetDataError(cGetDataInfo, sDataType, nListIndex, nFrameIndex, nRetryIndex, nResult, bSkipData);
            }

            return nResult;
        }

        /// <summary>
        /// 使用新方法取得Frame資料,根據IC世代(Gen8使用ADC資料類型,其他世代使用BASE資料類型)呼叫對應的資料擷取函式
        /// </summary>
        /// <param name="nDataBuffer_Array">用於接收資料的緩衝區陣列,以參考方式傳遞並更新</param>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含TX追蹤線數量及Self模式旗標</param>
        /// <param name="nRXTotalTraceNumber">RX追蹤線總數量</param>
        /// <param name="nTXTotalTraceNumber">TX追蹤線總數量</param>
        /// <param name="bTraceType">追蹤類型位元組(實際為掃描類型,MUTUAL或MUTUALSELF)</param>
        /// <param name="nTimeout">取得資料的超時時間(毫秒)</param>
        /// <returns>回傳資料擷取結果代碼,用於檢查觸控面板狀態</returns>
        private int GetDataByNewMethod(
            ref int[] nDataBuffer_Array, GetDataInfo cGetDataInfo, 
            int nRXTotalTraceNumber, int nTXTotalTraceNumber, 
            byte bTraceType, int nTimeout)
        {
            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                return ElanTouchSwitch.GetFrameData(
                    ElanTouchSwitch.m_nDATA_ADC, ref nDataBuffer_Array, nRXTotalTraceNumber, 
                    cGetDataInfo.m_nTXTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                    m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                );
            }
            else
            {
                return ElanTouchSwitch.GetFrameData(
                    ElanTouchSwitch.m_nDATA_BASE, ref nDataBuffer_Array, nRXTotalTraceNumber, 
                    nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                    m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                );
            }
        }

        /// <summary>
        /// 根據資料類型字串選擇並呼叫對應的Frame資料擷取函式,支援DV、RawData、ADC、BASE及OBASE等資料類型
        /// </summary>
        /// <param name="nDataBuffer_Array">用於接收資料的緩衝區陣列,以參考方式傳遞並更新</param>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含Self模式旗標等參數</param>
        /// <param name="sDataType">資料類型字串,用於判斷要擷取的資料類型</param>
        /// <param name="nRXTotalTraceNumber">RX追蹤線總數量</param>
        /// <param name="nTXTotalTraceNumber">TX追蹤線總數量</param>
        /// <param name="bTraceType">追蹤類型位元組(實際為掃描類型,MUTUAL或MUTUALSELF)</param>
        /// <param name="nTimeout">取得資料的超時時間(毫秒)</param>
        /// <returns>回傳資料擷取結果代碼,用於檢查觸控面板狀態</returns>
        private int GetDataByDataType(
            ref int[] nDataBuffer_Array, GetDataInfo cGetDataInfo, 
            string sDataType, int nRXTotalTraceNumber, int nTXTotalTraceNumber, 
            byte bTraceType, int nTimeout)
        {
            switch (sDataType)
            {
                case MainConstantParameter.m_sDATATYPE_DV:
                case MainConstantParameter.m_sDATATYPE_RawData:
                    return ElanTouchSwitch.GetFrameData(
                        ElanTouchSwitch.m_nDATA_DV, ref nDataBuffer_Array, nRXTotalTraceNumber, 
                        nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                        m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                    );

                case MainConstantParameter.m_sDATATYPE_ADC:
                    return GetADCData(ref nDataBuffer_Array, cGetDataInfo, nRXTotalTraceNumber, 
                                    nTXTotalTraceNumber, bTraceType, nTimeout);

                case MainConstantParameter.m_sDATATYPE_BASE:
                case MainConstantParameter.m_sDATATYPE_OBASE:
                    return ElanTouchSwitch.GetFrameData(
                        ElanTouchSwitch.m_nDATA_BASE, ref nDataBuffer_Array, nRXTotalTraceNumber, 
                        nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                        m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                    );

                default:
                    return ElanTouchSwitch.GetFrameData(
                        ElanTouchSwitch.m_nDATA_DV, ref nDataBuffer_Array, nRXTotalTraceNumber, 
                        nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                        m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                    );
            }
        }

        /// <summary>
        /// 取得ADC資料,根據是否為Raw ADC Sweep模式及IC世代(Gen6/Gen7/Gen8)選擇使用ADC或Noise資料類型進行擷取
        /// </summary>
        /// <param name="nDataBuffer_Array">用於接收資料的緩衝區陣列,以參考方式傳遞並更新</param>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含Raw ADC Sweep旗標及Self模式旗標</param>
        /// <param name="nRXTotalTraceNumber">RX追蹤線總數量</param>
        /// <param name="nTXTotalTraceNumber">TX追蹤線總數量</param>
        /// <param name="bTraceType">追蹤類型位元組(實際為掃描類型,MUTUAL或MUTUALSELF)</param>
        /// <param name="nTimeout">取得資料的超時時間(毫秒)</param>
        /// <returns>回傳資料擷取結果代碼,用於檢查觸控面板狀態</returns>
        private int GetADCData(
            ref int[] nDataBuffer_Array, GetDataInfo cGetDataInfo, 
            int nRXTotalTraceNumber, int nTXTotalTraceNumber, 
            byte bTraceType, int nTimeout)
        {
            if (cGetDataInfo.m_bRawADCSweep == true)
            {
                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    return ElanTouchSwitch.GetFrameData(
                        ElanTouchSwitch.m_nDATA_Noise, ref nDataBuffer_Array, nRXTotalTraceNumber, 
                        nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                        m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                    );
                }
                else if (m_eICGenerationType == ICGenerationType.Gen7)
                {
                    if (ParamFingerAutoTuning.m_nRawADCSGen7EnableHWTXN == 0)
                    {
                        return ElanTouchSwitch.GetFrameData(
                            ElanTouchSwitch.m_nDATA_ADC, ref nDataBuffer_Array, nRXTotalTraceNumber, 
                            nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                            m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                        );
                    }
                    else
                    {
                        return ElanTouchSwitch.GetFrameData(
                            ElanTouchSwitch.m_nDATA_Noise, ref nDataBuffer_Array, nRXTotalTraceNumber, 
                            nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                            m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                        );
                    }
                }
                else if (m_eICGenerationType == ICGenerationType.Gen6)
                {
                    return ElanTouchSwitch.GetFrameData(
                        ElanTouchSwitch.m_nDATA_ADC, ref nDataBuffer_Array, nRXTotalTraceNumber, 
                        nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                        m_bSocketConnectType, cGetDataInfo.m_bGetSelf
                    );
                }
            }
        
            return ElanTouchSwitch.GetFrameData(
                ElanTouchSwitch.m_nDATA_ADC, ref nDataBuffer_Array, nRXTotalTraceNumber, 
                nTXTotalTraceNumber, bTraceType, nTimeout, m_nDeviceIndex,
                m_bSocketConnectType, cGetDataInfo.m_bGetSelf
            );
        }

        /// <summary>
        /// 處理取得資料時發生的錯誤,輸出包含資料類型、階段、索引、Frame編號及錯誤代碼的詳細錯誤訊息,Chrome連線時額外發送測試命令並延遲
        /// </summary>
        /// <param name="cGetDataInfo">資料擷取資訊物件,包含階段訊息</param>
        /// <param name="sDataType">資料類型字串</param>
        /// <param name="nListIndex">當前執行的索引編號</param>
        /// <param name="nFrameIndex">當前Frame索引(從0開始)</param>
        /// <param name="nRetryIndex">當前重試索引</param>
        /// <param name="nResult">錯誤結果代碼,以十六進位格式顯示</param>
        /// <param name="bSkipData">是否為跳過資料模式,true時訊息前綴為"Get Skip",false時為"Get"</param>
        private void HandleGetDataError(
            GetDataInfo cGetDataInfo, string sDataType, int nListIndex, 
            int nFrameIndex, int nRetryIndex, int nResult, bool bSkipData)
        {
            string sPrefix = bSkipData ? "Get Skip" : "Get";
            string sMessage = string.Format("{0} {1} Data Error in {2}Frequency Set:{3} ", sPrefix, sDataType, cGetDataInfo.m_sStageMessage, nListIndex) +
                            string.Format("in Frame:{0}(Count={1})[ErrorCode=0x{2}]", nFrameIndex + 1, nRetryIndex, nResult.ToString("X4"));

            OutputMessage(string.Format("-{0}", sMessage));

            if (IsChromeSocketType())
            {
                SendTestCommand();
                Thread.Sleep(ParamFingerAutoTuning.m_nChromeGetDataDelayTime);
            }
        }

        /// <summary>
        /// 執行取得報告資料的流程,啟用報告模式後依據重複次數取得Base報告資料,若參數啟用則額外取得Signal報告資料,完成後關閉報告模式
        /// </summary>
        /// <param name="cSaveDataInfo">儲存資料資訊物件</param>
        /// <param name="cFlowStep">當前執行的流程步驟</param>
        /// <param name="nBrightness">螢幕亮度設定值</param>
        /// <param name="cFrequencyItem">頻率項目設定資訊</param>
        /// <param name="nRXTraceNumber">接收端(RX)追蹤線數量</param>
        /// <param name="nTXTraceNumber">發送端(TX)追蹤線數量</param>
        /// <param name="nExecuteIndex">當前執行的索引編號(從0開始)</param>
        /// <returns>所有報告資料取得成功回傳true;若任一報告資料取得失敗則回傳false</returns>
        private bool RunGetReportData(
            SaveDataInfo cSaveDataInfo, frmMain.FlowStep cFlowStep, int nBrightness, FrequencyItem cFrequencyItem, int nRXTraceNumber,
            int nTXTraceNumber, int nExecuteIndex)
        {
            /*
            if (bGetFirstDataFlag == true)
                Thread.Sleep(5000);
            */

            SetReportEnable(true);

            for (int nRepeatIndex = 0; nRepeatIndex <= ParamFingerAutoTuning.m_nSelfFSRepeatCount; nRepeatIndex++)
            {
                OutputMessage(string.Format("-Get Base Report Data(RepeatIndex={0})", nRepeatIndex));

                if (GetReportData(cSaveDataInfo, cFlowStep, nBrightness, cFrequencyItem, nRXTraceNumber, nTXTraceNumber, nExecuteIndex, nRepeatIndex) == false)
                    return false;

                if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1)
                {
                    OutputMessage(string.Format("-Get Signal Report Data(RepeatIndex={0})", nRepeatIndex));

                    if (GetReportData(cSaveDataInfo, cFlowStep, nBrightness, cFrequencyItem, nRXTraceNumber, nTXTraceNumber, nExecuteIndex, nRepeatIndex, true) == false)
                        return false;
                }
            }

            SetReportEnable(false);
            //m_byteReport_List = null;

            return true;
        }

        /// <summary>
        /// 取得並儲存報告資料(Base或Signal),依據報告類型參數執行單一或分段(Odd/Even或Forward/Backward)取得流程,包含重試機制、資料驗證及停止檢查
        /// </summary>
        /// <param name="cSaveDataInfo">儲存資料資訊物件</param>
        /// <param name="cFlowStep">當前執行的流程步驟</param>
        /// <param name="nBrightness">螢幕亮度設定值,用於失敗時的螢幕重置</param>
        /// <param name="cFrequencyItem">頻率項目設定資訊</param>
        /// <param name="nRXTraceNumber">接收端(RX)追蹤線數量</param>
        /// <param name="nTXTraceNumber">發送端(TX)追蹤線數量</param>
        /// <param name="nExecuteIndex">當前執行的索引編號(從0開始)</param>
        /// <param name="nRepeatIndex">當前重複執行的索引編號</param>
        /// <param name="bGetSignalData">是否取得Signal資料旗標,true時取得Signal報告並啟用ENCAL_SF,false時取得Base報告,預設為false</param>
        /// <returns>所有報告資料取得、儲存及停止檢查成功回傳true;若使用者中斷執行、資料建立失敗、未取得任何報告或停止檢查失敗則回傳false</returns>
        private bool GetReportData(
            SaveDataInfo cSaveDataInfo, frmMain.FlowStep cFlowStep, int nBrightness, FrequencyItem cFrequencyItem, int nRXTraceNumber,
            int nTXTraceNumber, int nExecuteIndex, int nRepeatIndex, bool bGetSignalData = false)
        {
            int nGetReportRetryCount = 3;
            int nStopReportRetryCount = 5;

            string sRecordDescribe = "";
            string sState = "";
            string sDataType = "";

            bool bSetRepeatIndex = false;
            bool bSetSelfCAL = false;

            if (ParamFingerAutoTuning.m_nSelfFSRepeatCount > 0)
                bSetRepeatIndex = true;

            if (bGetSignalData == true)
                SetENCAL_SFEnable(true);
            else
                SetENCAL_SFEnable(false);

            if (ParamFingerAutoTuning.m_nSelfFSGetReportType != 1)
            {
                if (bGetSignalData == true)
                {
                    sRecordDescribe = "Signal";
                    sState = "(S)";
                    sDataType = MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL;
                    bSetSelfCAL = true;
                }
                else
                {
                    sRecordDescribe = "Base";
                    sState = "";
                    sDataType = MainConstantParameter.m_sDATATYPE_REPORTMODE;
                }

                if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1 && ParamFingerAutoTuning.m_nSelfFSDisplayWarning == 1)
                    ShowfrmWarningMessage(string.Format("Start Record {0} Report Data(RepeatIndex={1})", sRecordDescribe, nRepeatIndex));

                if (m_eICGenerationType == ICGenerationType.Gen8 &&
                    ((bGetSignalData == false) || (bGetSignalData == true && (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2))) &&
                    nRepeatIndex == 0)
                    Thread.Sleep(ParamFingerAutoTuning.m_nGen8GetDataDelayTime);

                //m_byteReport_List = null;
                //m_byteReport_List = new List<byte[]>();

                /*
                StartSaveRecordData(MainConstantParameter.m_sDATATYPE_REPORTMODE, string.Format("Record_{0}", nExecuteIndex + 1));
                m_nRecordCount = 0;
                */
                cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, sDataType, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber, bSetSelfCAL: bSetSelfCAL);
                m_cSaveData = new SaveData(m_cfrmParent, m_sProjectName);

                if (m_cSaveData.CreateRecordData(cSaveDataInfo, m_sLogDirectoryPath, sDataType, nRepeatIndex, bGetSignalData, bSetRepeatIndex) == false)
                {
                    RunScreenResetFlow(nBrightness);
                    //m_byteReport_List = null;

                    if (bGetSignalData == true)
                        SetENCAL_SFEnable(false);

                    return false;
                }

                for (int nRetryIndex = 0; nRetryIndex < nGetReportRetryCount; nRetryIndex++)
                {
                    GetReportData(nRetryIndex);

                    //if (m_byteReport_List.Count >= ParamFingerAutoTuning.m_nSelfFSReportNumber)
                    if (m_cSaveData.GetRecordCount() >= ParamFingerAutoTuning.m_nSelfFSReportNumber)
                        break;

                    if (m_cfrmParent.m_bExecute == false)
                    {
                        //EndSaveRecordData();
                        m_cSaveData.CloseRecordData();
                        m_cSaveData = null;
                        RunScreenResetFlow(nBrightness);
                        //m_byteReport_List = null;

                        if (bGetSignalData == true)
                            SetENCAL_SFEnable(false);

                        return false;
                    }
                }

                if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1 && ParamFingerAutoTuning.m_nSelfFSDisplayWarning == 1)
                    ShowfrmWarningMessage(string.Format("Finish Record {0} Report Data(RepeatIndex={1})", sRecordDescribe, nRepeatIndex));

                //if (m_byteReport_List.Count == 0)
                if (m_cSaveData.GetRecordCount() == 0)
                {
                    m_sErrorMessage = string.Format("Get No Report{0} Data Error in Frequency Set:{1}(RepeatIndex={2})", sState, nExecuteIndex + 1, nRepeatIndex);

                    m_cSaveData.CloseRecordData();
                    m_cSaveData = null;
                    RunScreenResetFlow(nBrightness);
                    //m_byteReport_List = null;

                    if (bGetSignalData == true)
                        SetENCAL_SFEnable(false);

                    return false;
                }

                //EndSaveRecordData();
                m_cSaveData.CloseRecordData();

                /*
                cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, sDataType, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber);

                if (SaveReportData(sDataType, cSaveDataInfo, m_byteReport_List, nRepeatIndex) == false)
                {
                    RunScreenResetFlow(nBrightness);
                    m_byteReport_List = null;
                    return false;
                }
                */

                for (int nRetryIndex = 0; nRetryIndex < nStopReportRetryCount; nRetryIndex++)
                {
                    //m_byteReport_List = null;
                    //m_byteReport_List = new List<byte[]>();

                    if (CheckReportExist(nRetryIndex) == false)
                        break;

                    if (nRetryIndex == nStopReportRetryCount - 1)
                    {
                        m_sErrorMessage = string.Format("Stop Get Report{0} Data Error in Frequency Set:{1}(RepeatIndex={2})", sState, nExecuteIndex + 1, nRepeatIndex);

                        RunScreenResetFlow(nBrightness);
                        //m_byteReport_List = null;

                        if (bGetSignalData == true)
                            SetENCAL_SFEnable(false);

                        return false;
                    }

                    Thread.Sleep(100);
                }

                m_cSaveData = null;
            }
            else
            {
                if (bGetSignalData == true)
                {
                    sRecordDescribe = "Signal";
                    sState = "(S)";
                    sDataType = MainConstantParameter.m_sDATATYPE_REPORTMODE_SIGNAL;
                    bSetSelfCAL = true;
                }
                else
                {
                    sRecordDescribe = "Base";
                    sState = "";
                    sDataType = MainConstantParameter.m_sDATATYPE_REPORTMODE;
                }

                string[] sTracePart_Array = new string[] { MainConstantParameter.m_sOddTrace, MainConstantParameter.m_sEvenTrace };

                if (m_nSelfGetReportSequence < 0)
                    m_nSelfGetReportSequence = ParamFingerAutoTuning.m_nSelfFSGetReportSequence;

                if (m_nSelfGetReportSequence == 1)
                    sTracePart_Array = new string[] { MainConstantParameter.m_sEvenTrace, MainConstantParameter.m_sOddTrace };
                else if (m_nSelfGetReportSequence == 2)
                    sTracePart_Array = new string[] { MainConstantParameter.m_sForwardTrace, MainConstantParameter.m_sBackwardTrace };
                else if (m_nSelfGetReportSequence == 3)
                    sTracePart_Array = new string[] { MainConstantParameter.m_sBackwardTrace, MainConstantParameter.m_sForwardTrace };

                for (int nPartIndex = 0; nPartIndex < sTracePart_Array.Length; nPartIndex++)
                {
                    string sTracePart = sTracePart_Array[nPartIndex];
                    bool bFirstData = (nPartIndex == 0) ? true : false;

                    if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1 && nPartIndex == 0 && ParamFingerAutoTuning.m_nSelfFSDisplayWarning == 1)
                        ShowfrmWarningMessage(string.Format("Start Record {0} Report Data(RepeatIndex={1})", sRecordDescribe, nRepeatIndex));

                    if (m_eICGenerationType == ICGenerationType.Gen8 && bGetSignalData == false && nRepeatIndex == 0 && nPartIndex == 0)
                        Thread.Sleep(ParamFingerAutoTuning.m_nGen8GetDataDelayTime);

                    //m_byteReport_List = null;
                    //m_byteReport_List = new List<byte[]>();

                    cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, sDataType, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber, false, sTracePart, bSetSelfCAL: bSetSelfCAL);
                    m_cSaveData = new SaveData(m_cfrmParent, m_sProjectName);

                    if (m_cSaveData.CreateRecordData(cSaveDataInfo, m_sLogDirectoryPath, sDataType, nRepeatIndex, bGetSignalData, bSetRepeatIndex, bFirstData) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        //m_byteReport_List = null;

                        if (bGetSignalData == true)
                            SetENCAL_SFEnable(false);

                        return false;
                    }

                    for (int nRetryIndex = 0; nRetryIndex < nGetReportRetryCount; nRetryIndex++)
                    {
                        GetReportData(nRetryIndex, sTracePart);

                        //if (m_byteReport_List.Count >= ParamFingerAutoTuning.m_nSelfFSReportNumber)
                        if (m_cSaveData.GetRecordCount() >= ParamFingerAutoTuning.m_nSelfFSReportNumber)
                            break;

                        if (m_cfrmParent.m_bExecute == false)
                        {
                            m_cSaveData.CloseRecordData();
                            m_cSaveData = null;
                            RunScreenResetFlow(nBrightness);
                            //m_byteReport_List = null;

                            if (bGetSignalData == true)
                                SetENCAL_SFEnable(false);

                            return false;
                        }
                    }

                    if (ParamFingerAutoTuning.m_nSelfFSGetSignalReport == 1 && nPartIndex == sTracePart_Array.Length - 1 && ParamFingerAutoTuning.m_nSelfFSDisplayWarning == 1)
                        ShowfrmWarningMessage(string.Format("Finish Record {0} Report Data(RepeatIndex={1})", sRecordDescribe, nRepeatIndex));

                    //if (m_byteReport_List.Count == 0)
                    if (m_cSaveData.GetRecordCount() == 0)
                    {
                        m_sErrorMessage = string.Format("Get No Report{0} {1} Data Error in Frequency Set:{2}(RepeatIndex={3})", sState, sTracePart, nExecuteIndex + 1, nRepeatIndex);

                        m_cSaveData.CloseRecordData();
                        m_cSaveData = null;
                        RunScreenResetFlow(nBrightness);
                        //m_byteReport_List = null;

                        if (bGetSignalData == true)
                            SetENCAL_SFEnable(false);

                        return false;
                    }

                    m_cSaveData.CloseRecordData();

                    /*
                    cSaveDataInfo = SetSaveDataInfo(cFlowStep, cFrequencyItem, sDataType, m_sLogDirectoryPath, nExecuteIndex + 1, nTXTraceNumber, nRXTraceNumber, false, sTracePart);

                    if (SaveReportData(sDataType, cSaveDataInfo, m_byteReport_List, nRepeatIndex, bGetSignalData, bFirstData) == false)
                    {
                        RunScreenResetFlow(nBrightness);
                        m_byteReport_List = null;
                        return false;
                    }
                    */

                    for (int nRetryIndex = 0; nRetryIndex < nStopReportRetryCount; nRetryIndex++)
                    {
                        //m_byteReport_List = null;
                        //m_byteReport_List = new List<byte[]>();

                        if (CheckReportExist(nRetryIndex) == false)
                            break;

                        if (nRetryIndex == nStopReportRetryCount - 1)
                        {
                            m_sErrorMessage = string.Format("Stop Get Report{0} Data Error in Frequency Set:{1}(RepeatIndex={2})", sState, nExecuteIndex + 1, nRepeatIndex);

                            RunScreenResetFlow(nBrightness);
                            //m_byteReport_List = null;

                            if (bGetSignalData == true)
                                SetENCAL_SFEnable(false);

                            return false;
                        }

                        Thread.Sleep(100);
                    }

                    m_cSaveData = null;
                }
            }

            if (bGetSignalData == true)
                SetENCAL_SFEnable(false);

            return true;
        }

        /// <summary>
        /// 開始儲存記錄資料,建立RecordData目錄並創建檔案串流及寫入器以記錄報告資料(此方法已註解,功能已由SaveData類別取代)
        /// </summary>
        /// <param name="sDataType">資料類型字串</param>
        /// <param name="sFileName">檔案名稱(不含副檔名)</param>
        /*
        private void StartSaveRecordData(string sDataType, string sFileName)
        {
            string sDataTypeDirectoryPath = string.Format(@"{0}\{1}", m_sLogDirectoryPath, "RecordData");

            if (Directory.Exists(sDataTypeDirectoryPath) == false)
                Directory.CreateDirectory(sDataTypeDirectoryPath);

            m_fs = new FileStream(string.Format(@"{0}\{1}.txt", sDataTypeDirectoryPath, sFileName), FileMode.Create);

            //Create new file to log the report data
            lock (m_objRecordLocker)
            {
                m_swRecordLog = new StreamWriter(m_fs);
            }
        }
        */

        /// <summary>
        /// 結束儲存記錄資料,清空緩衝區並關閉寫入器及檔案串流,釋放相關資源(此方法已註解,功能已由SaveData類別取代)
        /// </summary>
        /*
        private void EndSaveRecordData()
        {
            lock (m_objRecordLocker)
            {
                m_swRecordLog.Flush();
                m_swRecordLog.Close();
                m_swRecordLog = null;
                m_fs.Close();
                m_fs = null;
            }
        }
        */

        /// <summary>
        /// 設定並建立儲存資料資訊物件,包含路徑、索引、韌體版本、Frame數量、頻率參數、Self/Mutual模式參數、RX/TX追蹤線數量等完整資訊
        /// </summary>
        /// <param name="cFlowStep">當前執行的流程步驟,用於判斷Self或Mutual模式</param>
        /// <param name="cFrequencyItem">頻率項目設定資訊,包含各項測試參數</param>
        /// <param name="sDataType">資料類型字串,用於判斷Frame數量</param>
        /// <param name="sLogDirectoryPath">記錄檔目錄路徑</param>
        /// <param name="nListIndex">列表索引編號(未使用,實際使用FrequencyItem的SetIndex)</param>
        /// <param name="nTXTraceNumber">發送端(TX)追蹤線數量</param>
        /// <param name="nRXTraceNumber">接收端(RX)追蹤線數量</param>
        /// <param name="bUseNewMethod">是否使用新方法旗標,true時Frame數量固定為1,預設為false</param>
        /// <param name="sSelfTracePart">Self模式的追蹤部分(AllTrace/OddTrace/EvenTrace/ForwardTrace/BackwardTrace),預設為AllTrace</param>
        /// <param name="bSetSelfCAL">是否設定Self CAL值旗標,僅在Self Frequency Sweep且啟用CAL時有效,預設為true</param>
        /// <returns>回傳設定完成的SaveDataInfo物件</returns>
        private SaveDataInfo SetSaveDataInfo(
            frmMain.FlowStep cFlowStep, FrequencyItem cFrequencyItem, string sDataType, string sLogDirectoryPath, int nListIndex, int nTXTraceNumber,
            int nRXTraceNumber, bool bUseNewMethod = false, string sSelfTracePart = MainConstantParameter.m_sAllTrace, bool bSetSelfCAL = true)
        {
            SaveDataInfo cSaveDataInfo = new SaveDataInfo();

            cSaveDataInfo.m_sLogDirectoryPath = sLogDirectoryPath;
            cSaveDataInfo.m_nListIndex = cFrequencyItem.m_nSetIndex;    //nListIndex;
            cSaveDataInfo.m_nFWVersion = m_nFWVersion;

            if (sDataType == MainConstantParameter.m_sDATATYPE_BASE ||
                sDataType == MainConstantParameter.m_sDATATYPE_KPKN ||
                sDataType == MainConstantParameter.m_sDATATYPE_OBASE)
                cSaveDataInfo.m_nFrameNumber = 1;
            else if (sDataType == MainConstantParameter.m_sDATATYPE_DV ||
                     sDataType == MainConstantParameter.m_sDATATYPE_RawData)
                cSaveDataInfo.m_nFrameNumber = cFrequencyItem.m_nDVTestFrame;
            else
                cSaveDataInfo.m_nFrameNumber = cFrequencyItem.m_nADCTestFrame;

            if (bUseNewMethod == true)
                cSaveDataInfo.m_nFrameNumber = 1;

            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                cSaveDataInfo.m_sSelfTraceType = m_eSelfTraceType.ToString();
                cSaveDataInfo.m_n_SELF_PH1 = cFrequencyItem.m_n_SELF_PH1;
                cSaveDataInfo.m_n_SELF_PH2E_LAT = cFrequencyItem.m_n_SELF_PH2E_LAT;
                cSaveDataInfo.m_n_SELF_PH2E_LMT = cFrequencyItem.m_n_SELF_PH2E_LMT;
                cSaveDataInfo.m_n_SELF_PH2_LAT = cFrequencyItem.m_n_SELF_PH2_LAT;
                cSaveDataInfo.m_n_SELF_PH2 = cFrequencyItem.m_n_SELF_PH2;
                cSaveDataInfo.m_nSelf_DFT_NUM = m_cSelfParameter.m_nDFT_NUM;
                cSaveDataInfo.m_dSelf_SampleTime = cFrequencyItem.m_dSelf_SampleTime;
                cSaveDataInfo.m_nSelf_Gain = m_cSelfParameter.m_nGain;
                cSaveDataInfo.m_nSelf_CAG = m_cSelfParameter.m_nCAG;
                cSaveDataInfo.m_nSelf_IQ_BSH = m_cSelfParameter.m_nIQ_BSH;

                cSaveDataInfo.m_nRead_SELF_PH1 = m_cReadParameter.m_n_SELF_PH1;
                cSaveDataInfo.m_nRead_SELF_PH2E_LAT = m_cReadParameter.m_n_SELF_PH2E_LAT;
                cSaveDataInfo.m_nRead_SELF_PH2E_LMT = m_cReadParameter.m_n_SELF_PH2E_LMT;
                cSaveDataInfo.m_nRead_SELF_PH2_LAT = m_cReadParameter.m_n_SELF_PH2_LAT;
                cSaveDataInfo.m_nRead_SELF_PH2 = m_cReadParameter.m_n_SELF_PH2;
                cSaveDataInfo.m_nReadSelf_DFT_NUM = m_cReadParameter.m_nSelf_DFT_NUM;
                cSaveDataInfo.m_nReadSelf_Gain = m_cReadParameter.m_nSelf_Gain;
                cSaveDataInfo.m_nReadSelf_CAG = m_cReadParameter.m_nSelf_CAG;
                cSaveDataInfo.m_nReadSelf_IQ_BSH = m_cReadParameter.m_nSelf_IQ_BSH;
                cSaveDataInfo.m_bGetSelf = true;

                if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep)
                {
                    if (ParamFingerAutoTuning.m_nSelfFSGetKValue == 1)
                        cSaveDataInfo.m_bGetSelfKValue = true;

                    cSaveDataInfo.m_sSelfTracePart = sSelfTracePart;

                    if (ParamFingerAutoTuning.m_nSelfFSRunKSequence == 1 || ParamFingerAutoTuning.m_nSelfFSRunKSequence == 2)
                    {
                        cSaveDataInfo.m_nSelfNCPValue = m_nSelfNCPValue;
                        cSaveDataInfo.m_nSelfNCNValue = m_nSelfNCNValue;
                        cSaveDataInfo.m_bSetSelfKSequence = true;
                    }

                    if (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2)
                    {
                        if (bSetSelfCAL == true)
                        {
                            cSaveDataInfo.m_bSetSelfCAL = true;
                            cSaveDataInfo.m_nSelfCALValue = m_cSelfParameter.m_nCAL;
                        }
                        else
                            cSaveDataInfo.m_nSelfCALValue = 0;
                    }
                }
                else if (cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                {
                    if (ParamFingerAutoTuning.m_nSelfPNSGetKValue == 1)
                        cSaveDataInfo.m_bGetSelfKValue = true;

                    cSaveDataInfo.m_nSelfNCPValue = m_nSelfNCPValue;
                    cSaveDataInfo.m_nSelfNCNValue = m_nSelfNCNValue;
                    cSaveDataInfo.m_bSetSelfKSequence = true;
                }
            }
            else
            {
                cSaveDataInfo.m_nPH1 = cFrequencyItem.m_nPH1;
                cSaveDataInfo.m_nPH2 = cFrequencyItem.m_nPH2;
                cSaveDataInfo.m_nPH3 = cFrequencyItem.m_nPH2;
                cSaveDataInfo.m_nDFT_NUM = cFrequencyItem.m_nDFT_NUM;

                cSaveDataInfo.m_nReadPH1 = m_cReadParameter.m_nPH1;
                cSaveDataInfo.m_nReadPH2 = m_cReadParameter.m_nPH2;
                cSaveDataInfo.m_nReadPH3 = m_cReadParameter.m_nPH3;
                cSaveDataInfo.m_nReadDFT_NUM = m_cReadParameter.m_n_MS_DFT_NUM;
            }

            cSaveDataInfo.m_nTXTraceNumber = nTXTraceNumber;
            cSaveDataInfo.m_nRXTraceNumber = nRXTraceNumber;

            cSaveDataInfo.m_nReadProjectOption = m_nReadProjectOption;
            cSaveDataInfo.m_nReadFWIPOption = m_nReadFWIPOption;

            return cSaveDataInfo;
        }

        /// <summary>
        /// 設定並建立Raw ADC Sweep模式的儲存資料資訊物件,包含路徑、索引、韌體版本、Frame數量、FIR參數(依IC世代不同)、增益參數、RX/TX追蹤線數量等完整資訊
        /// </summary>
        /// <param name="cFlowStep">當前執行的流程步驟,用於判斷是否為Raw ADC Sweep模式</param>
        /// <param name="cRawADCSweepItem">Raw ADC掃描項目設定資訊,包含各項測試參數</param>
        /// <param name="sDataType">資料類型字串,用於判斷Frame數量</param>
        /// <param name="sLogDirectoryPath">記錄檔目錄路徑</param>
        /// <param name="nListIndex">列表索引編號(未使用,實際使用RawADCSweepItem的SetIndex)</param>
        /// <param name="nTXTraceNumber">發送端(TX)追蹤線數量</param>
        /// <param name="nRXTraceNumber">接收端(RX)追蹤線數量</param>
        /// <param name="bUseNewMethod">是否使用新方法旗標,true時Frame數量固定為1,預設為false</param>
        /// <returns>回傳設定完成的SaveDataInfo物件</returns>
        private SaveDataInfo SetSaveDataInfo(
            frmMain.FlowStep cFlowStep, RawADCSweepItem cRawADCSweepItem, string sDataType, string sLogDirectoryPath, int nListIndex, int nTXTraceNumber,
            int nRXTraceNumber, bool bUseNewMethod = false)
        {
            SaveDataInfo cSaveDataInfo = new SaveDataInfo();

            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
                cSaveDataInfo.m_bRawADCSweep = true;

            cSaveDataInfo.m_sLogDirectoryPath = sLogDirectoryPath;
            cSaveDataInfo.m_nListIndex = cRawADCSweepItem.m_nSetIndex;    //nListIndex;
            cSaveDataInfo.m_nFWVersion = m_nFWVersion;

            if (sDataType == MainConstantParameter.m_sDATATYPE_ADC)
                cSaveDataInfo.m_nFrameNumber = cRawADCSweepItem.m_nADCTestFrame;
            else
                cSaveDataInfo.m_nFrameNumber = cRawADCSweepItem.m_nADCTestFrame;

            if (bUseNewMethod == true)
                cSaveDataInfo.m_nFrameNumber = 1;

            if (cFlowStep.m_eStep == MainStep.Raw_ADC_Sweep)
            {
                cSaveDataInfo.m_nFIR_TAP_NUM = cRawADCSweepItem.m_nFIR_TAP_NUM;
                cSaveDataInfo.m_nReadFIR_TAP_NUM = m_cReadParameter.m_n_MS_FIR_TAP_NUM;

                if (m_eICGenerationType == ICGenerationType.Gen8)
                {
                    cSaveDataInfo.m_nFIRCOEF_SEL = cRawADCSweepItem.m_nFIRCOEF_SEL;
                    cSaveDataInfo.m_nReadFIRCOEF_SEL = m_cReadParameter.m_n_MS_FIRCOEF_SEL;
                }
                else if (m_eICGenerationType == ICGenerationType.Gen7)
                {
                    cSaveDataInfo.m_nFIRTB = cRawADCSweepItem.m_nFIRTB;
                    cSaveDataInfo.m_nReadFIRTB = m_cReadParameter.m_n_MS_FIRTB;
                }
                else if (m_eICGenerationType == ICGenerationType.Gen6)
                {
                    cSaveDataInfo.m_nFIRTB = cRawADCSweepItem.m_nFIRTB;
                    cSaveDataInfo.m_nReadFIRTB = m_cReadParameter.m_n_MS_FIRTB;
                }

                cSaveDataInfo.m_nDFT_NUM = m_cOriginParameter.m_n_MS_DFT_NUM;
                cSaveDataInfo.m_nIQ_BSH_0 = m_cOriginParameter.m_n_MS_IQ_BSH;
                cSaveDataInfo.m_nSELC = cRawADCSweepItem.m_nSELC;
                cSaveDataInfo.m_nVSEL = cRawADCSweepItem.m_nVSEL;
                cSaveDataInfo.m_nLG = cRawADCSweepItem.m_nLG;
                cSaveDataInfo.m_nSELGM = cRawADCSweepItem.m_nSELGM;

                cSaveDataInfo.m_nReadSELC = m_cReadParameter.m_n_MS_SELC;
                cSaveDataInfo.m_nReadVSEL = m_cReadParameter.m_n_MS_VSEL;
                cSaveDataInfo.m_nReadLG = m_cReadParameter.m_n_MS_LG;
                cSaveDataInfo.m_nReadSELGM = m_cReadParameter.m_n_MS_SELGM;
            }

            cSaveDataInfo.m_nTXTraceNumber = nTXTraceNumber;
            cSaveDataInfo.m_nRXTraceNumber = nRXTraceNumber;

            cSaveDataInfo.m_nReadProjectOption = m_nReadProjectOption;
            cSaveDataInfo.m_nReadFWIPOption = m_nReadFWIPOption;

            return cSaveDataInfo;
        }

        /// <summary>
        /// 儲存Frame資料至CSV檔案,並根據資料類型更新對應項目的檔案路徑,若啟用H5格式則額外轉換為HDF5檔案
        /// </summary>
        /// <param name="sDataType">資料類型字串(OBASE/BASE/ADC等),用於判斷儲存路徑及更新對應項目</param>
        /// <param name="cSaveDataInfo">儲存資料資訊物件,包含儲存所需的完整參數</param>
        /// <param name="nFrameData_Array">三維Frame資料陣列[Frame索引, TX, RX]</param>
        /// <param name="bRawADCSweepItem">是否為Raw ADC Sweep項目旗標,true時更新RawADCSweepItem列表,false時更新FrequencyItem列表,預設為false</param>
        /// <param name="bUnsigedData">是否為無符號資料旗標,預設為false</param>
        /// <returns>資料儲存成功(及H5轉換成功,若啟用)回傳true;儲存失敗或H5轉換失敗則回傳false並設定錯誤訊息</returns>
        private bool SaveData(string sDataType, SaveDataInfo cSaveDataInfo, int[, ,] nFrameData_Array, bool bRawADCSweepItem = false, bool bUnsigedData = false)
        {
            bool bCompleteFlag = false;

            SaveData cSaveData = new SaveData(m_cfrmParent, m_sProjectName);
            bCompleteFlag = cSaveData.SaveFrameData(cSaveDataInfo, sDataType, nFrameData_Array, m_eICGenerationType, m_eICSolutionType, bUnsigedData);

            switch (sDataType)
            {
                case MainConstantParameter.m_sDATATYPE_OBASE:
                    //m_cFreqencyItem_List[cSaveDataInfo.m_nListIndex - 1].m_sOBASEFilePath = cSaveData.DataFilePath;
                    m_cFreqencyItem_List[cSaveDataInfo.m_nListIndex].m_sOBASEFilePath = cSaveData.DataFilePath;
                    break;
                case MainConstantParameter.m_sDATATYPE_BASE:
                    //m_cFreqencyItem_List[cSaveDataInfo.m_nListIndex - 1].m_sBASEFilePath = cSaveData.DataFilePath;
                    m_cFreqencyItem_List[cSaveDataInfo.m_nListIndex].m_sBASEFilePath = cSaveData.DataFilePath;
                    break;
                case MainConstantParameter.m_sDATATYPE_ADC:
                    if (bRawADCSweepItem == true)
                        m_cRawADCSweepItem_List[cSaveDataInfo.m_nListIndex].m_sADCFilePath = cSaveData.DataFilePath;
                    else
                    {
                        //m_cFreqencyItem_List[cSaveDataInfo.m_nListIndex - 1].m_sADCFilePath = cSaveData.DataFilePath;
                        m_cFreqencyItem_List[cSaveDataInfo.m_nListIndex].m_sADCFilePath = cSaveData.DataFilePath;
                    }

                    break;
                default:
                    break;
            }

            if (bCompleteFlag == false)
            {
                m_sErrorMessage = cSaveData.ErrorMessage;
                return bCompleteFlag;
            }

            //執行Python所寫"HDF5ConvertTool"將CSV Raw Data檔轉換為H5檔
            if (m_bGenerateH5Data == true)
            {
                string sCsvFilePath = cSaveData.DataFilePath;
                string sH5DirectoryPath = string.Format(@"{0}\{1}", m_sH5LogDirectoryPath, sDataType);

                if (Directory.Exists(sH5DirectoryPath) == false)
                    Directory.CreateDirectory(sH5DirectoryPath);

                bCompleteFlag = ConvertHDF5Data(sCsvFilePath, sH5DirectoryPath);
            }

            return bCompleteFlag;
        }

        /// <summary>
        /// 儲存報告資料至檔案並更新對應項目的檔案路徑(此方法已註解,功能已整合至GetReportData方法中的即時儲存流程)
        /// </summary>
        /// <param name="sDataType">資料類型字串(OBASE/BASE/ADC等),用於判斷儲存路徑及更新對應項目</param>
        /// <param name="cSaveDataInfo">儲存資料資訊物件,包含儲存所需的完整參數</param>
        /// <param name="byteReport_List">報告資料位元組陣列列表</param>
        /// <param name="nRepeatIndex">當前重複執行的索引編號</param>
        /// <param name="bGetSignalData">是否為Signal資料旗標,預設為false</param>
        /// <param name="bFirstData">是否為第一筆資料旗標,預設為true</param>
        /// <returns>資料儲存成功回傳true;儲存失敗則回傳false並設定錯誤訊息</returns>
        /*
        private bool SaveReportData(
            string sDataType, SaveDataInfo cSaveDataInfo, List<byte[]> byteReport_List, int nRepeatIndex, bool bGetSignalData = false,
            bool bFirstData = true)
        {
            bool bCompleteFlag = false;
            bool bSetRepeatIndex = false;

            if (ParamFingerAutoTuning.m_nSelfFSRepeatCount > 0)
                bSetRepeatIndex = true;

            SaveData cSaveData = new SaveData(m_cfrmParent, m_sProjectName);

            bCompleteFlag = cSaveData.SaveReportData(cSaveDataInfo, sDataType, byteReport_List, nRepeatIndex, bGetSignalData, bSetRepeatIndex, bFirstData);

            switch (sDataType)
            {
                case MainConstantParameter.m_sDATATYPE_OBASE:
                    m_cFreqencyItem_List[cSaveDataInfo.m_nListIndex - 1].m_sOBASEFilePath = cSaveData.DataFilePath;
                    break;
                case MainConstantParameter.m_sDATATYPE_BASE:
                    m_cFreqencyItem_List[cSaveDataInfo.m_nListIndex - 1].m_sBASEFilePath = cSaveData.DataFilePath;
                    break;
                case MainConstantParameter.m_sDATATYPE_ADC:
                    m_cFreqencyItem_List[cSaveDataInfo.m_nListIndex - 1].m_sADCFilePath = cSaveData.DataFilePath;
                    break;
                default:
                    break;
            }

            if (bCompleteFlag == false)
            {
                m_sErrorMessage = cSaveData.ErrorMessage;
                return bCompleteFlag;
            }

            return bCompleteFlag;
        }
        */

        /// <summary>
        /// 儲存Normal Raw Data類型的資料至CSV檔案,根據Self/Mutual模式及頻率參數產生檔案名稱,使用FrameManager進行儲存
        /// </summary>
        /// <param name="cFrequencyItem">頻率項目設定資訊,包含PH1、PH2等參數用於計算頻率及命名</param>
        /// <param name="sDataType">資料類型字串,用於目錄及檔案命名</param>
        /// <param name="nDataType">原始資料類型代碼(如RAWDATA_ADC、RAWDATA_dV等)</param>
        /// <param name="nListIndex">列表索引編號,用於錯誤訊息</param>
        /// <param name="sLogDirectoryPath">記錄檔目錄路徑</param>
        /// <param name="bGetSelf">是否為Self模式旗標,true時使用Self參數計算頻率及命名,預設為false</param>
        /// <returns>資料儲存成功回傳true;儲存失敗則回傳false並設定錯誤訊息</returns>
        private bool SaveNormalRawDataType(FrequencyItem cFrequencyItem, string sDataType, int nDataType, int nListIndex, string sLogDirectoryPath, bool bGetSelf = false)
        {
            int nFrameNumber = cFrequencyItem.m_nADCTestFrame;

            string sDataTypeDirectoryName = sDataType;
            string sDataTypeDirectoryPath = string.Format(@"{0}\{1}", sLogDirectoryPath, sDataTypeDirectoryName);

            if (Directory.Exists(sDataTypeDirectoryPath) == false)
                Directory.CreateDirectory(sDataTypeDirectoryPath);

            double dFrequency = 0.0;
            string sDataFileName = "";

            if (bGetSelf == true)
            {
                int nSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cFrequencyItem.m_n_SELF_PH2E_LAT, cFrequencyItem.m_n_SELF_PH2E_LMT, cFrequencyItem.m_n_SELF_PH2_LAT, cFrequencyItem.m_n_SELF_PH2);
                dFrequency = ElanConvert.Convert2Frequency(cFrequencyItem.m_n_SELF_PH1, nSelfPH2Sum);
                sDataFileName = string.Format("{0}_{1}_{2}_{3}_{4}", sDataTypeDirectoryName, dFrequency.ToString("0.000"),
                                              cFrequencyItem.m_n_SELF_PH1.ToString("x2").ToUpper(), nSelfPH2Sum.ToString("x2").ToUpper(),
                                              m_eSelfTraceType.ToString());
            }
            else
            {
                dFrequency = ElanConvert.Convert2Frequency(cFrequencyItem.m_nPH1, cFrequencyItem.m_nPH2);
                sDataFileName = string.Format("{0}_{1}_{2}_{3}", sDataTypeDirectoryName, dFrequency.ToString("0.000"),
                                              cFrequencyItem.m_nPH1.ToString("x2").ToUpper(), cFrequencyItem.m_nPH2.ToString("x2").ToUpper());
            }

            string sDataFilePath = string.Format(@"{0}\{1}.CSV", sDataTypeDirectoryPath, sDataFileName);

            //int nDataType = ElanDef.RAWDATA_ADC;
            //Assign the self mode is not necessary. Even the data include the self mode.
            int nColorLow = 1;
            int nColorLevel = 1;

            if (m_bSocketConnectType == false)
            {
                ElanTouch.TraceMode Mode = ElanTouch.GetTraceMode(m_nICType, bSelf: false);//ElanTouch.TraceMode.Mutual | ElanTouch.TraceMode.Partial;

                if (m_cFrameMgr.Save(sDataFilePath, m_structTraceInfo, nDataType, m_nICType, nColorLow, nColorLevel, Mode) == false)
                {
                    m_sErrorMessage = string.Format("Save Record Data Error in Frequency Set:{0}", nListIndex);
                    return false;
                }
            }
            else
            {
                ElanTouch_Socket.TraceMode Mode = ElanTouch_Socket.GetTraceMode(m_nICType, bSelf: false);//ElanTouch_Socket.TraceMode.Mutual | ElanTouch_Socket.TraceMode.Partial;

                if (m_cFrameMgr.Save(sDataFilePath, m_structTraceInfo_Socket, nDataType, m_nICType, nColorLow, nColorLevel, Mode) == false)
                {
                    m_sErrorMessage = string.Format("Save Record Data Error in Frequency Set:{0}", nListIndex);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 儲存Raw ADC Sweep的Normal Raw Data類型資料至CSV檔案,根據SELC、VSEL、LG參數產生檔案名稱,使用FrameManager進行儲存
        /// </summary>
        /// <param name="cRawADCSweepItem">Raw ADC掃描項目設定資訊,包含SELC、VSEL、LG等參數用於命名</param>
        /// <param name="sDataType">資料類型字串,用於目錄及檔案命名</param>
        /// <param name="nDataType">原始資料類型代碼(如RAWDATA_ADC、RAWDATA_Noise等)</param>
        /// <param name="nListIndex">列表索引編號,用於錯誤訊息</param>
        /// <param name="sLogDirectoryPath">記錄檔目錄路徑</param>
        /// <returns>資料儲存成功回傳true;儲存失敗則回傳false並設定錯誤訊息</returns>
        private bool SaveNormalRawDataType(RawADCSweepItem cRawADCSweepItem, string sDataType, int nDataType, int nListIndex, string sLogDirectoryPath)
        {
            int nFrameNumber = cRawADCSweepItem.m_nADCTestFrame;

            string sDataTypeDirectoryName = sDataType;
            string sDataTypeDirectoryPath = string.Format(@"{0}\{1}", sLogDirectoryPath, sDataTypeDirectoryName);

            if (Directory.Exists(sDataTypeDirectoryPath) == false)
                Directory.CreateDirectory(sDataTypeDirectoryPath);

            string sDataFileName = string.Format("{0}_{1}_{2}_{3}", sDataTypeDirectoryName, cRawADCSweepItem.m_nSELC, cRawADCSweepItem.m_nVSEL, cRawADCSweepItem.m_nLG);
            string sDataFilePath = string.Format(@"{0}\{1}.CSV", sDataTypeDirectoryPath, sDataFileName);

            //int nDataType = ElanDef.RAWDATA_ADC;
            //Assign the self mode is not necessary. Even the data include the self mode.
            int nColorLow = 1;
            int nColorLevel = 1;

            if (m_bSocketConnectType == false)
            {
                ElanTouch.TraceMode Mode = ElanTouch.GetTraceMode(m_nICType, bSelf: false);//ElanTouch.TraceMode.Mutual | ElanTouch.TraceMode.Partial;

                if (m_cFrameMgr.Save(sDataFilePath, m_structTraceInfo, nDataType, m_nICType, nColorLow, nColorLevel, Mode) == false)
                {
                    m_sErrorMessage = string.Format("Save Record Data Error in Frequency Set:{0}", nListIndex);
                    return false;
                }
            }
            else
            {
                ElanTouch_Socket.TraceMode Mode = ElanTouch_Socket.GetTraceMode(m_nICType, bSelf: false);//ElanTouch_Socket.TraceMode.Mutual | ElanTouch_Socket.TraceMode.Partial;

                if (m_cFrameMgr.Save(sDataFilePath, m_structTraceInfo_Socket, nDataType, m_nICType, nColorLow, nColorLevel, Mode) == false)
                {
                    m_sErrorMessage = string.Format("Save Record Data Error in Frequency Set:{0}", nListIndex);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 計算BASE減去ADC的差值並儲存至結果陣列,若啟用Normal Raw Data儲存則將每個Frame加入佇列並轉移至Frame Manager
        /// </summary>
        /// <param name="nBASEMinusADCFrame_Array">用於儲存BASE-ADC結果的三維陣列[Frame, TX, RX],以參考方式傳遞並更新</param>
        /// <param name="nBASEFrame_Array">BASE資料的三維陣列[Frame, TX, RX],僅使用第0個Frame</param>
        /// <param name="nADCFrame_Array">ADC資料的三維陣列[Frame, TX, RX]</param>
        /// <param name="nFrameNumber">要處理的Frame數量</param>
        /// <param name="nRXTraceNumber">接收端(RX)追蹤線數量</param>
        /// <param name="nTXTraceNumber">發送端(TX)追蹤線數量</param>
        /// <param name="bGetSelf">是否為Self模式旗標,用於Frame物件建立,預設為false</param>
        private void ComputeBASEMinusADC(
           ref int[, ,] nBASEMinusADCFrame_Array, int[, ,] nBASEFrame_Array, int[, ,] nADCFrame_Array, int nFrameNumber, int nRXTraceNumber,
           int nTXTraceNumber, bool bGetSelf = false)
        {
            int[,] nFrameBuffer_Array = null;
            //long nStartTime = 0;

            if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
            {
                m_bqcFrameQueue.Clear();

                //Get the start tick. Use to compute the timestamp.
                //nStartTime = DateTime.Now.Ticks;
            }

            for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
            {
                if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                {
                    nFrameBuffer_Array = new int[nTXTraceNumber + 1, nRXTraceNumber + 1];
                    Array.Clear(nFrameBuffer_Array, 0, nFrameBuffer_Array.Length);
                }

                for (int nTXIndex = 1; nTXIndex <= nTXTraceNumber; nTXIndex++)
                {
                    for (int nRXIndex = 1; nRXIndex <= nRXTraceNumber; nRXIndex++)
                    {
                        int nBASEMinusADCValue = nBASEFrame_Array[0, nTXIndex, nRXIndex] - nADCFrame_Array[nFrameIndex, nTXIndex, nRXIndex];
                        nBASEMinusADCFrame_Array[nFrameIndex, nTXIndex, nRXIndex] = nBASEMinusADCValue;

                        if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                            nFrameBuffer_Array[nTXIndex - 1, nRXIndex - 1] = nBASEMinusADCValue;
                    }
                }

                if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
                {
                    //Get the timestamp
                    //long nTimeInterval = ((DateTime.Now.Ticks - nStartTime) / 10000);
                    long nTimeInterval = 300 * (nFrameIndex + 1);
                    m_bqcFrameQueue.Enqueue(new Frame(nFrameBuffer_Array, nRXTraceNumber, nTXTraceNumber, bGetSelf, UserInterfaceDefine.RawDataType.Type_dV, nTimeInterval));
                }
            }

            if (ParamFingerAutoTuning.m_nSaveNormalRawDataType == 1)
            {
                int nTotalCount = m_bqcFrameQueue.Count;
                m_cFrameMgr.Clear();

                for (int nFrameIndex = 0; nFrameIndex < nTotalCount; nFrameIndex++)
                {
                    Frame cFrame = new Frame();

                    if (m_bqcFrameQueue.Dequeue(ElanDefine.TIME_100MS, ref cFrame) == false)
                        continue;

                    m_cFrameMgr.Add(cFrame);
                }
            }
        }

        /// <summary>
        /// 將一維來源陣列的資料複製到三維目標陣列的指定Frame索引中,依據BaseLength計算一維陣列的索引位置進行轉換
        /// </summary>
        /// <param name="nSource_Array">來源一維整數陣列</param>
        /// <param name="nDestination_Array">目標三維整數陣列[Frame, Y, X]</param>
        /// <param name="nDestinationIndex">目標Frame索引,指定要複製到三維陣列的哪一個Frame</param>
        /// <param name="nXLength">X軸(RX)的資料長度</param>
        /// <param name="nYLength">Y軸(TX)的資料長度</param>
        /// <param name="nBaseLength">基礎長度,用於計算一維陣列索引(通常為nXLength+1)</param>
        private void CopyArray(int[] nSource_Array, int[, ,] nDestination_Array, int nDestinationIndex, int nXLength, int nYLength, int nBaseLength)
        {
            for (int nYIndex = 1; nYIndex <= nYLength; nYIndex++)
            {
                for (int nXIndex = 1; nXIndex <= nXLength; nXIndex++)
                {
                    int nIndex = nYIndex * nBaseLength + nXIndex;
                    nDestination_Array[nDestinationIndex, nYIndex, nXIndex] = (short)nSource_Array[nIndex];
                }
            }
        }

        /// <summary>
        /// 將一維來源陣列的資料複製到二維目標陣列中,依據BaseLength計算一維陣列的索引位置,並將索引從1-based轉換為0-based
        /// </summary>
        /// <param name="nSource_Array">來源一維整數陣列</param>
        /// <param name="nDestination_Array">目標二維整數陣列[Y, X]</param>
        /// <param name="nXLength">X軸(RX)的資料長度</param>
        /// <param name="nYLength">Y軸(TX)的資料長度</param>
        /// <param name="nBaseLength">基礎長度,用於計算一維陣列索引(通常為nXLength+1)</param>
        private void CopyArray(int[] nSource_Array, int[,] nDestination_Array, int nXLength, int nYLength, int nBaseLength)
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

        /// <summary>
        /// 從CSV檔案讀取Frame資料,驗證檔案中的PH1、PH2及追蹤線數量與頻率項目是否匹配,並將資料載入至三維陣列中
        /// </summary>
        /// <param name="nFrameData_Array">用於儲存讀取資料的三維陣列[Frame, TX, RX],以參考方式傳遞並更新</param>
        /// <param name="cFrequencyItem">頻率項目設定資訊,包含PH1、PH2、TX/RX追蹤線數量及檔案路徑</param>
        /// <param name="sDataType">資料類型字串(OBASE/BASE/ADC),用於判斷檔案路徑及處理邏輯</param>
        /// <param name="nListIndex">列表索引編號,用於錯誤訊息</param>
        /// <returns>資料讀取成功且參數匹配回傳true;檔案參數不匹配、讀取失敗或資料不完整則回傳false並設定錯誤訊息</returns>
        private bool GetFileData(ref int[, ,] nFrameData_Array, FrequencyItem cFrequencyItem, string sDataType, int nListIndex)
        {
            int nTXTraceNumber = -1;
            int nRXTraceNumber = -1;
            int nReadPH1 = -1;
            int nReadPH2 = -1;
            int nFrameIndex = 0;
            string sFilePath = "";

            switch (sDataType)
            {
                case MainConstantParameter.m_sDATATYPE_OBASE:
                    sFilePath = cFrequencyItem.m_sOBASEFilePath;
                    break;
                case MainConstantParameter.m_sDATATYPE_BASE:
                    sFilePath = cFrequencyItem.m_sBASEFilePath;
                    break;
                case MainConstantParameter.m_sDATATYPE_ADC:
                    sFilePath = cFrequencyItem.m_sADCFilePath;
                    break;
            }

            bool bBASEType = false;

            if (sDataType == MainConstantParameter.m_sDATATYPE_OBASE || sDataType == MainConstantParameter.m_sDATATYPE_BASE)
                bBASEType = true;

            string sLine = "";
            StreamReader srReadFile = new StreamReader(sFilePath, Encoding.Default);

            try
            {
                while ((sLine = srReadFile.ReadLine()) != null)
                {
                    string[] sSubString_Array = sLine.Split(',');

                    if (sSubString_Array.Length >= 2)
                    {
                        switch (sSubString_Array[0])
                        {
                            case "TXTraceNumber":
                                Int32.TryParse(sSubString_Array[1], out nTXTraceNumber);
                                break;
                            case "RXTraceNumber":
                                Int32.TryParse(sSubString_Array[1], out nRXTraceNumber);
                                break;
                            case "ReadPH1(Hex)":
                                nReadPH1 = Convert.ToInt32(sSubString_Array[1], 16);
                                break;
                            case "ReadPH2(Hex)":
                                nReadPH2 = Convert.ToInt32(sSubString_Array[1], 16);
                                break;
                            case "Frame":
                                Int32.TryParse(sSubString_Array[1], out nFrameIndex);
                                break;
                            default:
                                break;
                        }
                    }

                    if (bBASEType == true)
                    {
                        if (nTXTraceNumber > -1 && nRXTraceNumber > -1 &&
                            nReadPH1 > -1 && nReadPH2 > -1)
                            break;
                    }
                }
            }
            finally
            {
                srReadFile.Close();
            }

            if (cFrequencyItem.m_nPH1 == nReadPH1 && cFrequencyItem.m_nPH2 == nReadPH2 &&
                (ParamFingerAutoTuning.m_nACFRModeType == 1 ||
                 (ParamFingerAutoTuning.m_nACFRModeType != 1 && cFrequencyItem.m_nTXTraceNumber == nTXTraceNumber && cFrequencyItem.m_nRXTraceNumber == nRXTraceNumber)))
            {
                nFrameData_Array = new int[nFrameIndex + 1, nTXTraceNumber + 1, nRXTraceNumber + 1];
                bool bGetFrameData = false;
                bool bGetDataFinish = false;
                int nTXCount = 1;
                int nMFIndex = 0;

                srReadFile = new StreamReader(sFilePath, Encoding.Default);

                try
                {
                    while ((sLine = srReadFile.ReadLine()) != null)
                    {
                        string[] sSubString_Array = sLine.Split(',');

                        if (sSubString_Array.Length >= 2)
                        {
                            if (sSubString_Array[0] == "Frame")
                            {
                                bGetFrameData = true;
                                nTXCount = 1;
                                continue;
                            }
                        }

                        if (bGetFrameData == true)
                        {
                            if (sSubString_Array.Length >= nRXTraceNumber)
                            {
                                for (int nRXIndex = 1; nRXIndex <= nRXTraceNumber; nRXIndex++)
                                    nFrameData_Array[nMFIndex, nTXCount, nRXIndex] = Convert.ToInt32(sSubString_Array[nRXIndex - 1]);

                                nTXCount++;
                            }

                            if (nTXCount > nTXTraceNumber)
                            {
                                if (bBASEType == false)
                                    nMFIndex++;

                                bGetFrameData = false;
                                bGetDataFinish = true;

                                if (bBASEType == true)
                                    break;
                            }
                        }
                    }
                }
                finally
                {
                    srReadFile.Close();
                }

                if (nFrameData_Array == null || bGetDataFinish == false)
                {
                    m_sErrorMessage = string.Format("Get {0} Frame Error in Frequency Set:{1}", sDataType, nListIndex);
                    return false;
                }
            }
            else
            {
                m_sErrorMessage = string.Format("Get {0} TP Info Not Match in Frequency Set:{1}", sDataType, nListIndex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 從CSV檔案的StreamReader讀取所有Frame資料並儲存至列表中,每個Frame為一個二維陣列(此方法已註解,功能已由其他方法取代)
        /// </summary>
        /// <param name="nFrameData_List">用於儲存所有Frame的二維陣列列表,以參考方式傳遞並更新</param>
        /// <param name="srFile">已開啟的StreamReader物件,用於讀取檔案內容</param>
        /// <param name="nTXTraceNumber">發送端(TX)追蹤線數量</param>
        /// <param name="nRXTraceNumber">接收端(RX)追蹤線數量</param>
        /// <param name="sFileName">檔案名稱,用於錯誤訊息</param>
        /// <returns>成功讀取至少一個Frame回傳true;未讀取到任何Frame則回傳false並設定錯誤訊息</returns>
        /*
        private bool GetFrameData(ref List<int[,]> nFrameData_List, StreamReader srFile, int nTXTraceNumber, int nRXTraceNumber, string sFileName)
        {
            bool bGetFrameData = false;
            int[,] nSingleFrameData_Array = null;
            int nTXCount = 0;
            string sLine = "";

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    string[] sSplitData_Array = sLine.Split(',');

                    if (sSplitData_Array.Length >= 2)
                    {
                        if (sSplitData_Array[0] == "Frame")
                        {
                            bGetFrameData = true;
                            nTXCount = 0;
                            nSingleFrameData_Array = new int[nTXTraceNumber, nRXTraceNumber];
                            continue;
                        }
                    }

                    if (bGetFrameData == true)
                    {
                        if (sSplitData_Array.Length >= nRXTraceNumber)
                        {
                            for (int nTraceIndex = 0; nTraceIndex < nRXTraceNumber; nTraceIndex++)
                                nSingleFrameData_Array[nTXCount, nTraceIndex] = Convert.ToInt32(sSplitData_Array[nTraceIndex]);

                            nTXCount++;
                        }

                        if (nTXCount == nTXTraceNumber)
                        {
                            nFrameData_List.Add(nSingleFrameData_Array);
                            bGetFrameData = false;
                        }
                    }
                }
            }
            finally
            {
                srFile.Close();
            }

            if (nFrameData_List == null || nFrameData_List.Count == 0)
            {
                m_sErrorMessage = string.Format("Read Data Frame Error in {0}[Count:{1}]", sFileName, nFrameData_List.Count);
                return false;
            }

            return true;
        }
        */

        /// <summary>
        /// 更新全螢幕或圖案視窗上的Frame數量顯示文字,根據圖案類型及圖案圖片路徑選擇對應的視窗進行更新
        /// </summary>
        /// <param name="sDataType">資料類型字串,用於顯示文字描述</param>
        /// <param name="nFrameIndex">當前Frame索引</param>
        /// <param name="nFrameCount">總Frame數量</param>
        private void SetDisplayFrameNumber(string sDataType, int nFrameIndex, int nFrameCount)
        {
            if (File.Exists(ParamFingerAutoTuning.m_sPatternPicPath) == true ||
                ParamFingerAutoTuning.m_nPatternType == 2)
            {
                if (m_cfrmFullScreen != null)
                    m_cfrmFullScreen.SetFrameNumberText(sDataType, nFrameIndex, nFrameCount);
            }
            else if (ParamFingerAutoTuning.m_nPatternType == 1)
            {
                if (m_cfrmPHCKPattern != null)
                    m_cfrmPHCKPattern.SetFrameNumberText(sDataType, nFrameIndex, nFrameCount);
            }
        }

        #endregion
#endif
    }
}
