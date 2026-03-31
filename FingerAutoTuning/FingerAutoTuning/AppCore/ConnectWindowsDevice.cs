using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        #region Windows 裝置連線

        /// <summary>
        /// 連接Windows裝置,使用USB VID/PID及I2C參數建立連接,若啟用Finger Report測試則註冊HID裝置並儲存報告
        /// </summary>
        /// <returns>連接成功且所有流程完成回傳true;連接失敗、註冊失敗或使用者中斷執行則回傳false並設定錯誤訊息</returns>
        private bool ConnectWindowsDevice()
        {
            int nVID = ParamFingerAutoTuning.m_nUSBVID;
            int nPID = ParamFingerAutoTuning.m_nUSBPID;
            int nDVDD = ParamFingerAutoTuning.m_nDVDD;
            int nVIO = ParamFingerAutoTuning.m_nVIO;
            int nI2CAddress = ParamFingerAutoTuning.m_nI2CAddress;
            int nNormalLength = ParamFingerAutoTuning.m_nNormalLength;

            if (!ConnectTP(nVID, nPID, nDVDD, nVIO, nI2CAddress, nNormalLength))
            {
                //MessageBox.Show("No Valid Device Found. Please Press Ok to Exit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //m_bCollectFlowError = true;
                //m_sCollectFlowErrorMsg = "No Valid TP Device Found";
                //OutputMessage("-No Valid TP Device Found");

                m_bTPConnected = false;
                m_sErrorMessage = "Disconnect to TP Device";
                return false;
            }

            // 處理 Finger Report 測試
            if (ParamFingerAutoTuning.m_nFingerReportTest == 1)
            {
                if (!RegisterAndSaveFingerReport()) return false;
                if (!m_cfrmParent.m_bExecute) return false;
            }

            return true;
        }

        /// <summary>
        /// 連接到觸控面板裝置,設定介面類型並使用指定的USB及I2C參數建立連接,取得有效的裝置索引後設定TP連接狀態
        /// </summary>
        /// <param name="nAssignVID">USB Vendor ID</param>
        /// <param name="nAssignPID">USB Product ID</param>
        /// <param name="nDVDD">DVDD電壓值</param>
        /// <param name="nVIO">VIO電壓值</param>
        /// <param name="nI2CAddress">I2C位址</param>
        /// <param name="nNormalLength">一般命令長度</param>
        /// <param name="bReconnect">是否為重新連接旗標,true時且非Gen8則不執行斷線,預設為false</param>
        /// <returns>連接成功並取得有效裝置索引回傳true並設定TP連接狀態;連接失敗或無有效裝置則回傳false</returns>
        private bool ConnectTP(int nAssignVID, int nAssignPID, int nDVDD, int nVIO, int nI2CAddress, int nNormalLength, bool bReconnect = false)
        {
            if (!bReconnect || m_eICGenerationType == ICGenerationType.Gen8)
            {
                ElanTouchSwitch.Disconnect(m_bSocketConnectType);
            }

            int nInterface = ElanTouchSwitch.SetInterface(m_bSocketConnectType);

            if (!ElanTouchSwitch.Connect(nAssignVID, nAssignPID, nInterface, nDVDD, nVIO, nI2CAddress, nNormalLength, m_bSocketConnectType))
                return false;

            // 取得裝置索引
            int nSelectedDeviceIndex = GetDeviceIndex();

            if (nSelectedDeviceIndex == -1)
            {
                ElanTouchSwitch.Disconnect(m_bSocketConnectType);
                return false;
            }

            m_nDeviceIndex = nSelectedDeviceIndex;
            m_bTPConnected = true;
            return true;
        }

        /// <summary>
        /// 取得裝置索引,列舉所有HID裝置路徑並回傳第一個有效裝置的索引編號
        /// </summary>
        /// <returns>回傳第一個有效裝置的索引編號;若無有效裝置則回傳-1</returns>
        private int GetDeviceIndex()
        {
            List<string> sElanDevice_List = new List<string>();
            int nSIZE_1K = 1024;
            string sDEFAULT_EMPTY_DEVPATH = "NoTouch Device";
            byte[] bytePathName_Array = new byte[nSIZE_1K];
            IntPtr npPair = Marshal.AllocHGlobal(nSIZE_1K);

            try
            {
                int nDeviceCount = ElanTouchSwitch.GetDeviceCount(m_bSocketConnectType);

                for (int nDeviceIndex = 0; nDeviceIndex < nDeviceCount; nDeviceIndex++)
                {
                    if (ElanTouchSwitch.GetHIDDevPath(npPair, bytePathName_Array.Length, nDeviceIndex, m_bSocketConnectType))
                    {
                        Marshal.Copy(npPair, bytePathName_Array, 0, nSIZE_1K);
                        string sPath = ElanConvert.ByteArrayToCharString(bytePathName_Array);
                        sElanDevice_List.Add(sPath);
                    }
                    else
                    {
                        sElanDevice_List.Add(sDEFAULT_EMPTY_DEVPATH);
                    }
                }

                // 找到第一個有效的裝置路徑
                for (int nDeviceIndex = 0; nDeviceIndex < sElanDevice_List.Count; nDeviceIndex++)
                {
                    if (sDEFAULT_EMPTY_DEVPATH.CompareTo(sElanDevice_List[nDeviceIndex]) != 0)
                    {
                        return nDeviceIndex;
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(npPair);
            }

            return -1;
        }

        /// <summary>
        /// 註冊並儲存Finger Report,註冊指定VID/PID的HID裝置、設定HID處理事件並執行Finger Report儲存流程
        /// </summary>
        /// <returns>註冊成功回傳true;註冊失敗則回傳false並設定錯誤訊息</returns>
        private bool RegisterAndSaveFingerReport()
        {
            if (!m_cInputDevice.RegisterHIDDevice(ParamFingerAutoTuning.m_nUSBVID, ParamFingerAutoTuning.m_nUSBPID))
            {
                m_sErrorMessage = "Register TP Device Error";
                return false;
            }

            m_bRegistHIDDevice = true;
            m_cInputDevice.HIDHandler -= HIDRawInputHandler;
            m_cInputDevice.HIDHandler += HIDRawInputHandler;

            SaveFingerReport();

            #region Mark It
            /*
            List<List<byte>> DataArray = new List<List<byte>>();

            m_bStartRecord = true;
            
            while (m_frmParent.m_bExecute == true)
            {
                byte[] m_pBuf = new byte[116];
                
                if (m_qFIFO.DequeueAll(5000, m_pBuf, 116) == false)
                    continue;

                List<byte> ReportArray = new List<byte>();

                ReportArray.Clear();

                foreach (byte Raw in m_pBuf)
                    ReportArray.Add(Raw);

                DataArray.Add(new List<byte>(ReportArray));
                continue;
            }
            m_bStartRecord = false;

            FileStream fs = new FileStream(@"FingerReport.txt", FileMode.Create);
            m_swReportLog = new StreamWriter(fs);

            int DataLength = DataArray.Count;

            try
            {
                for (int nIndex = 0; nIndex < DataLength; nIndex++)
                {
                    string saveString = "";

                    int ReportLength = DataArray[nIndex].Count;

                    for (int mIndex = 0; mIndex < ReportLength; mIndex++)
                        saveString += DataArray[nIndex][mIndex].ToString("X2") + " ";

                    m_swReportLog.WriteLine(saveString);
                }
            }
            catch (IOException ex)
            {
                m_sErrMsg = "Save Record Data Error";
                return false;
            }
            finally
            {
                m_swReportLog.Flush();
                m_swReportLog.Close();
                fs.Close();
            }
            */
            #endregion

            return true;
        }

        #endregion
    }
}
