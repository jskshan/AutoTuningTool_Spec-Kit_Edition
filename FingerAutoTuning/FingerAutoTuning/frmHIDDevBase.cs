using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using Elan;

namespace FingerAutoTuning
{
    public partial class frmHIDDevBase : Form
    {
        protected IntPtr m_HIDEventHandle = IntPtr.Zero;

        private const int FILE_TYPE_UNKNOW = 0;
        private const int FILE_TYPE_INI = 1;
        private const int FILE_TYPE_XML = 2;

        protected string m_sPath = "";
        protected int m_nFileType = FILE_TYPE_UNKNOW;
        public bool m_bDataFormatError = false;

        public frmHIDDevBase()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            //Register the Event Handler
            m_HIDEventHandle = Win32.RegisterForUsbEvents(Handle, Win32.HIDGuid);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            //Release the handle
            if (m_HIDEventHandle != IntPtr.Zero)
                Win32.UnregisterForUsbEvents(m_HIDEventHandle);
            base.OnHandleDestroyed(e);
        }

        public void SetDevice()
        {

        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_DEVICECHANGE)	// we got a device change message! A USB device was inserted or removed
                HandleDevChangeEvent(m);

            base.WndProc(ref m);
        }

        protected void HandleDevChangeEvent(Message m)
        {
            switch (m.WParam.ToInt32())	// Check the W parameter to see if a device was inserted or removed
            {
                case Win32.DEVICE_ARRIVAL:	// inserted

                    DeviceArrive(m);

                    break;
                case Win32.DEVICE_REMOVECOMPLETE:	// removed
                    UInt32 nPID = 0;
                    UInt32 nVID = 0;
                    if (ElanTouch.GetID(ref nVID, ref nPID, 0) == ElanTouch.TP_SUCCESS)
                    {
                        if (GetRemovedDevPID(m.LParam) != nPID)
                            break;

                        DeviceRemove(m);
                    }

                    break;
            }
        }

        protected virtual void DeviceArrive(Message m)
        {
        }

        protected virtual void DeviceRemove(Message m)
        {
        }

        /// <summary>
        /// Get the removed device PID
        /// </summary>
        /// <param name="LParam">The LParam form message</param>
        /// <returns>PID</returns>
        private int GetRemovedDevPID(IntPtr LParam)
        {
            Win32.DEV_BROADCAST_HDR DevBroadcast = (Win32.DEV_BROADCAST_HDR)Marshal.PtrToStructure(LParam,
                                                                                        typeof(Win32.DEV_BROADCAST_HDR));
            if (DevBroadcast.dbch_DeviceType == 5)
            {
                Win32.DEV_BROADCAST_DEVICEINTERFACE devInterface = (Win32.DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(LParam,
                                                                                typeof(Win32.DEV_BROADCAST_DEVICEINTERFACE));

                string[] TokenArray = devInterface.dbcc_name.Split('&');
                int nRemoveDevPID = -1;
                foreach (string sToken in TokenArray)
                {
                    if (sToken.ToUpper().Contains("PID_") == true)
                    {
                        string sPID = sToken.Split('_')[1];
                        Int32.TryParse(sPID, System.Globalization.NumberStyles.HexNumber, null, out nRemoveDevPID);
                    }
                }

                UInt32 nPID = 0;
                UInt32 nVID = 0;
                ElanTouch.GetID(ref nVID, ref nPID, 0);

                Console.WriteLine(string.Format("Removed Device PID:{0}, Current Device PID:{1}", nRemoveDevPID, nPID));
                return nRemoveDevPID;
            }

            return -1;
        }
    }

    public static class ExtensionMethods
    {
        public static void DoubleBuffered(DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
}
