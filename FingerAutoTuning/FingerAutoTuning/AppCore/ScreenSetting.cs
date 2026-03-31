using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sStepName"></param>
        /// <param name="nExecuteIndex"></param>
        /// <param name="nCount"></param>
        public void DisplayPattern(string sStepName, int nExecuteIndex, int nCount)
        {
            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_WINDOWS &&
                    m_bSocketConnectType == false)
                {
                    if (File.Exists(ParamFingerAutoTuning.m_sPatternPicPath) == true ||
                        ParamFingerAutoTuning.m_nPatternType == 2 || ParamFingerAutoTuning.m_nPatternType == 3)
                    {
                        string sStepState = sStepName;
                        string sProgressState = string.Format("{0}/{1}", nExecuteIndex + 1, nCount);

                        //Get the all screen
                        Screen[] scScreen_Array = Screen.AllScreens;
                        Color clrDisplayColor = Color.Black;

                        if (ParamFingerAutoTuning.m_nPatternType == 3)
                            clrDisplayColor = ParamFingerAutoTuning.m_clrCustomizeScreenColor;

                        Bitmap bmPatternPicture = null;

                        if (File.Exists(ParamFingerAutoTuning.m_sPatternPicPath) == true)
                            bmPatternPicture = new Bitmap(ParamFingerAutoTuning.m_sPatternPicPath);

                        m_cfrmFullScreen = new frmFullScreen(sProgressState, sStepState, clrDisplayColor, bmPatternPicture);
                        m_cfrmFullScreen.btnPatternHandler += m_cfrmParent.btnPatternEnableHandler;

                        m_cfrmFullScreen.BackColor = clrDisplayColor;

                        if (scScreen_Array.Length > 1)
                        {
                            m_cfrmFullScreen.Location = scScreen_Array[1].Bounds.Location;
                            m_cfrmFullScreen.StartPosition = FormStartPosition.Manual;
                            m_cfrmFullScreen.Location = new Point(scScreen_Array[1].Bounds.Location.X, scScreen_Array[1].Bounds.Location.Y);
                        }

                        m_cfrmFullScreen.Show();
                        m_cfrmFullScreen.TopMost = true;
                        m_bStartShowFullScreen = true;

                        m_cfrmParent.btnPatternEnable(false);
                        m_cfrmFullScreen.Focus();
                    }
                    else if (ParamFingerAutoTuning.m_nPatternType == 1)
                    {
                        string sStepState = sStepName;
                        string sProgressState = string.Format("{0}/{1}", nExecuteIndex + 1, nCount);
                        m_cfrmPHCKPattern = new frmPHCKPattern(sProgressState, sStepState, frmMain.m_byteEDIDData_Array, frmMain.m_cEDIDInformation);
                        m_cfrmPHCKPattern.btnPatternHandler += m_cfrmParent.btnPatternEnableHandler;
                        m_cfrmPHCKPattern.Show();
                        //m_cfrmPHCKPattern.TopMost = true;
                        m_bStartShowFullScreen = true;

                        m_cfrmParent.btnPatternEnable(false);
                        m_cfrmPHCKPattern.Focus();
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void DisableMonitor()
        {
            if (ParamFingerAutoTuning.m_nPatternType == 4 && ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_WINDOWS)
            {
                bool bCheck = false;

                while (bCheck == false)
                {
                    bool bSendMessageComplete = false;

                    /*
                    Thread tSendMessage = new Thread(() =>
                    {
                        m_frmParent.Invoke((MethodInvoker)delegate
                        {
                            SendMessage(m_cfrmParent.Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, (int)AppCoreDefine.MonitorState.MONITOR_OFF);
                            bSendMessageComplete = true;
                        });
                    });
                    tSendMessage.IsBackground = true;
                    tSendMessage.Start();
                    */
                    m_cfrmParent.Invoke((MethodInvoker)delegate
                    {
                        SendMessage(m_cfrmParent.Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, (int)AppCoreDefine.MonitorState.MONITOR_OFF);
                        bSendMessageComplete = true;
                    });

                    long nStartTime = DateTime.Now.Ticks;
                    long nCurrentTime = 0;
                    int nCostTime = 0;
                    bool bStopFlag = false;

                    while (bSendMessageComplete == false)
                    {
                        nCurrentTime = DateTime.Now.Ticks;
                        nCostTime = (int)((nCurrentTime - nStartTime) / 10000);

                        if (nCostTime > 4000)
                        {
                            bStopFlag = true;
                            break;
                        }

                        Thread.Sleep(100);
                    }

                    /*
                    if (bStopFlag == true)
                    {
                        if (tSendMessage != null || tSendMessage.IsAlive == true)
                        {
                            tSendMessage.Abort();
                            tSendMessage.Join();
                        }
                    }
                    else
                        bCheck = true;
                    */
                    if (bStopFlag == false)
                        bCheck = true;

                    if (bSendMessageComplete == true)
                    {
                        m_cfrmParent.Invoke((MethodInvoker)delegate
                        {
                            m_cfrmParent.btnPatternEnable(true);
                        });
                    }
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void HidePattern()
        {
            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                while (m_bStartShowFullScreen == true)
                {
                    m_cfrmParent.btnPatternEnable(false);

                    if (File.Exists(ParamFingerAutoTuning.m_sPatternPicPath) == true ||
                        ParamFingerAutoTuning.m_nPatternType == 2)
                    {
                        if (m_cfrmFullScreen != null)
                        {
                            m_cfrmFullScreen.Hide();
                            m_cfrmFullScreen.Dispose();
                            m_cfrmFullScreen = null;
                            m_bStartShowFullScreen = false;
                        }
                    }
                    else if (ParamFingerAutoTuning.m_nPatternType == 1)
                    {
                        if (m_cfrmPHCKPattern != null)
                        {
                            m_cfrmPHCKPattern.Hide();
                            m_cfrmPHCKPattern.Dispose();
                            m_cfrmPHCKPattern = null;
                            m_bStartShowFullScreen = false;
                        }
                    }

                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMessage"></param>
        private void ShowfrmWarningMessage(string sMessage)
        {
            m_cfrmParent.Invoke((MethodInvoker)delegate
            {
                frmWarningMessage m_cfrmWarningMessage = new frmWarningMessage();

                int nLocationX = (int)((m_cfrmParent.Left + m_cfrmParent.Right) / 2) - (int)(m_cfrmWarningMessage.Width / 2);
                int nLocationY = (int)((m_cfrmParent.Top + m_cfrmParent.Bottom) / 2) - (int)(m_cfrmWarningMessage.Height / 2);

                if (m_cfrmParent.IsMdiChild == true)
                {
                    nLocationX = (int)((m_cfrmParent.MdiParent.Left + m_cfrmParent.MdiParent.Right) / 2) - (int)(m_cfrmWarningMessage.Width / 2);
                    nLocationY = (int)((m_cfrmParent.MdiParent.Top + m_cfrmParent.MdiParent.Bottom) / 2) - (int)(m_cfrmWarningMessage.Height / 2);
                }

                if (m_cfrmParent.m_bParentFormFlag == true)
                {
                    nLocationX = (int)((m_cfrmParent.ParentForm.Left + m_cfrmParent.ParentForm.Right) / 2) - (int)(m_cfrmWarningMessage.Width / 2);
                    nLocationY = (int)((m_cfrmParent.ParentForm.Top + m_cfrmParent.ParentForm.Bottom) / 2) - (int)(m_cfrmWarningMessage.Height / 2);
                }

                m_cfrmWarningMessage.StartPosition = FormStartPosition.Manual;
                m_cfrmWarningMessage.Location = new Point(nLocationX, nLocationY);

                m_cfrmWarningMessage.WarningMessageLoad(sMessage);

                if (m_cfrmWarningMessage.ShowDialog() == DialogResult.Cancel)
                    return;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int SetBrightness()
        {
            int nBrightness = -1;

            if (ParamFingerAutoTuning.m_nBackLightValue > -1 && ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_WINDOWS)
            {
                try
                {
                    nBrightness = GetBrightness();
                }
                catch
                {
                }
            }

            if (nBrightness > -1)
                SetBrightness((byte)ParamFingerAutoTuning.m_nBackLightValue);

            return nBrightness;
        }

        /// <summary>
        /// Get the Actual Percentage of Brightness
        /// </summary>
        /// <returns></returns>
        private int GetBrightness()
        {
            //define scope (namespace)
            System.Management.ManagementScope s = new System.Management.ManagementScope("root\\WMI");

            //define query
            System.Management.SelectQuery q = new System.Management.SelectQuery("WmiMonitorBrightness");

            //output current brightness
            System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(s, q);

            System.Management.ManagementObjectCollection moc = mos.Get();

            //store result
            byte byteBrightness = 0;

            foreach (System.Management.ManagementObject o in moc)
            {
                byteBrightness = (byte)o.GetPropertyValue("CurrentBrightness");
                break; //only work on the first object
            }

            moc.Dispose();
            mos.Dispose();

            return (int)byteBrightness;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteTargetBrightness"></param>
        private void SetBrightness(byte byteTargetBrightness)
        {
            //define scope (namespace)
            System.Management.ManagementScope s = new System.Management.ManagementScope("root\\WMI");

            //define query
            System.Management.SelectQuery q = new System.Management.SelectQuery("WmiMonitorBrightnessMethods");

            //output current brightness
            System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(s, q);

            System.Management.ManagementObjectCollection moc = mos.Get();

            foreach (System.Management.ManagementObject o in moc)
            {
                o.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, byteTargetBrightness }); //note the reversed order - won't work otherwise!
                break; //only work on the first object
            }

            moc.Dispose();
            mos.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nBrightness"></param>
        /// <param name="bErrorFlag"></param>
        private void RunScreenResetFlow(int nBrightness, bool bErrorFlag = true)
        {
            if (ParamFingerAutoTuning.m_nPatternType == 4)
            {
                bool bCheck = false;

                while (bCheck == false)
                {
                    bool bSendMsgComplete = false;

                    /*
                    Thread tSendMessage = new Thread(() =>
                    {
                        m_cfrmParent.Invoke((MethodInvoker)delegate
                        {
                            SendMessage(m_frmParent.Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, (int)AppCoreDefine.MonitorState.MONITOR_ON);
                            bSendMsgComplete = true;
                        });
                    });
                    tSendMessage.IsBackground = true;
                    tSendMessage.Start();
                    */

                    m_cfrmParent.Invoke((MethodInvoker)delegate
                    {
                        SendMessage(m_cfrmParent.Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, (int)AppCoreDefine.MonitorState.MONITOR_ON);
                        bSendMsgComplete = true;
                    });

                    long nStartTime = DateTime.Now.Ticks;
                    long nCurrentTime = 0;
                    int nCostTime = 0;
                    bool bStopFlag = false;

                    while (bSendMsgComplete == false)
                    {
                        nCurrentTime = DateTime.Now.Ticks;
                        nCostTime = (int)((nCurrentTime - nStartTime) / 10000);

                        if (nCostTime > 4000)
                        {
                            bStopFlag = true;
                            break;
                        }

                        Thread.Sleep(100);
                    }

                    /*
                    if (bStopFlag == true)
                    {
                        if (tSendMessage != null || tSendMessage.IsAlive == true)
                        {
                            tSendMessage.Abort();
                            tSendMessage.Join();
                        }
                    }
                    else
                        bCheck = true;
                    */
                    if (bStopFlag == false)
                        bCheck = true;

                    if (bSendMsgComplete == true)
                    {
                        m_cfrmParent.Invoke((MethodInvoker)delegate
                        {
                            m_cfrmParent.btnPatternEnable(false);
                        });
                    }
                }

                MouseMoveEvent();
                /*
                if (bErrorFlag == true)
                    MouseMoveEvent();
                */
            }

            HidePattern();

            if (ParamFingerAutoTuning.m_nBackLightValue > -1)
            {
                int nSetCount = 0;
                bool bCheckFlag = false;

                while (bCheckFlag == false)
                {
                    if (nBrightness > -1)
                    {
                        SetBrightness((byte)nBrightness);

                        int nGetCount = 0;

                        while (true)
                        {
                            try
                            {
                                int nCheckBrightness = GetBrightness();

                                if (nBrightness == nCheckBrightness)
                                    bCheckFlag = true;
                            }
                            catch
                            {
                            }

                            if (bCheckFlag == true)
                                break;
                            else if (nGetCount >= 3)
                                break;

                            Thread.Sleep(10);
                            nGetCount++;
                        }

                        if (bCheckFlag == false)
                            nSetCount++;
                        else if (nGetCount >= 3)
                            break;
                    }
                    else
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void MouseMoveEvent()
        {
            string sMouseCursorX = Cursor.Position.X.ToString();  // get current cursor position
            string sMouseCursorY = Cursor.Position.Y.ToString();

            int nMouseCursor_X = Int16.Parse(sMouseCursorX);
            int nMouseCursor_Y = Int16.Parse(sMouseCursorY);
            mouse_event(MOUSEEVENTF_LEFTDOWN, nMouseCursor_X, nMouseCursor_Y, 0, 0);//make left button down
            mouse_event(MOUSEEVENTF_LEFTUP, nMouseCursor_X, nMouseCursor_Y, 0, 0);//make left button up
        }
    }
}
