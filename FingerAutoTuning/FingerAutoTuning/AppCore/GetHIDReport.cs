using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HIDRawInputHandler(object sender, InputDevice.HIDDeviceEventArgs e)
        {
            if (m_bStartRecord == true)
            {
                if (e.m_Buffer[0] != 0x01)
                    return;

                lock (m_objReportLocker)
                {
                    if (ParamFingerAutoTuning.m_nFingerReportTest == 1)
                    {
                        if (m_swReportLog != null)
                        {
                            StringBuilder sbContent = new StringBuilder();

                            foreach (byte byteValue in e.m_Buffer)
                                sbContent.Append(string.Format("{0:X2} ", byteValue));

                            m_swReportLog.WriteLine(sbContent.ToString());
                            m_swReportLog.Flush();
                        }
                    }

                    if (ParamFingerAutoTuning.m_nSelfFSGetDataType == 1)
                    {
                        //m_listbyteReport.Add(e.m_Buffer);
                        m_cSaveData.WriteReportData(e.m_Buffer);
                    }
                }

                /*
                lock (m_objRecordLocker)
                {
                    if (m_swRecordLog != null)
                    {
                        StringBuilder sbContent = new StringBuilder();

                        foreach (byte byteValue in e.m_Buffer)
                            sbContent.Append(string.Format("{0:X2} ", byteValue));

                        m_swRecordLog.WriteLine(sbContent.ToString());
                        m_swRecordLog.Flush();

                        m_nRecordCount++;
                    }
                }
                */
            }
            /*
            if (m_bStartRecord == true)
                m_qFIFO.EnqueueAll(e.m_Buffer, e.m_Buffer.Length, 0);
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objParameter"></param>
        protected void OnInputReportReceive(object objParameter)
        {
            //OutputMessage("-Execute OnInputReportReceive Thread");

            byte[] byteBuffer_Array = new byte[67];

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT)
                byteBuffer_Array = new byte[165];

            long nStartTime = DateTime.Now.Ticks;
            int nReportCount = 0;

            while (m_bRecvSocketFingerReport)
            {
                if (m_bStartRecord == true)
                {
                    #region Update the report rate
                    long nTimeInterval = (DateTime.Now.Ticks - nStartTime) / 10000;

                    if (nTimeInterval >= 1000)
                    {
                        lock (this)
                        {
                            m_cfrmParent.Invoke((MethodInvoker)delegate
                            {
                                /*
                                if (m_frmCurTestCase != null)
                                    m_frmCurTestCase.UpdateReportRate((int)(nReportCount * (1000.0f / nTimeInterval)));
                                */
                            });
                            nReportCount = 0;
                            nStartTime = DateTime.Now.Ticks;
                        }
                    }
                    #endregion

                    int nResultFlag = ElanTouchSwitch.ReadDevData(byteBuffer_Array, 0, m_bSocketConnectType, 10);

                    //if (ElanTouch.ReadDevData(byteBuffer_Array, byteBuffer_Array.Length, 10, 0) != ElanTouch.TP_SUCCESS)
                    if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                        continue;

                    if (byteBuffer_Array[0] != 0x01)
                        continue;

                    nReportCount++;

                    lock (m_objReportLocker)
                    {
                        if (ParamFingerAutoTuning.m_nFingerReportTest == 1)
                        {
                            if (m_swReportLog != null)
                            {
                                /*
                                if (ParamFingerAutoTuning.m_sOSType == MainConstantParameter.OS_CHROME)
                                {
                                    if (byteBuffer_Array[0] != 0x62)
                                        continue;
                                }
                                */

                                StringBuilder sbContent = new StringBuilder();

                                foreach (byte byteBuffer in byteBuffer_Array)
                                    sbContent.Append(string.Format("{0:X2} ", byteBuffer));

                                m_swReportLog.WriteLine(sbContent.ToString());
                                m_swReportLog.Flush();
                            }
                        }

                        if (ParamFingerAutoTuning.m_nSelfFSGetDataType == 1)
                        {
                            //m_byteReport_List.Add(byteBuffer_Array);
                            m_cSaveData.WriteReportData(byteBuffer_Array);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFlowStep"></param>
        /// <returns></returns>
        private bool RegistHIDDevice(frmMain.FlowStep cFlowStep)
        {
#if _USE_9F07_SOCKET
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return true;
#endif

            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep && ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_WINDOWS)
            {
                if (ParamFingerAutoTuning.m_nSelfFSGetDataType == 1 && m_bRegistHIDDevice == false)
                {
                    if (m_cInputDevice.RegisterHIDDevice(ParamFingerAutoTuning.m_nUSBVID, ParamFingerAutoTuning.m_nUSBPID) == false)
                    {
                        m_sErrorMessage = "Register TP Device Error";
                        return false;
                    }

                    m_cInputDevice.HIDHandler -= HIDRawInputHandler;
                    m_cInputDevice.HIDHandler += HIDRawInputHandler;
                }
            }

            return true;
        }
    }
}
