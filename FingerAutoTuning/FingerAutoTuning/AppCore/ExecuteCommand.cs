using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    partial class AppCore
    {
        /// <summary>
        /// 取得 RX 或 TX Trace 數量
        /// </summary>
        /// <param name="nTraceNumber">輸出參數，回傳取得的 Trace 數量</param>
        /// <param name="eTraceType">Trace 類型（TraceType.RX 或 TraceType.TX）</param>
        /// <returns>true: 成功取得有效的 Trace 數量（> 0）; false: 取得失敗或數量無效（<= 0）</returns>
        private bool GetRXTXTraceNumber(ref int nTraceNumber, TraceType eTraceType)
        {
            int nNumber = 0;

            string sTraceTypeName = "RX";

            if (eTraceType == TraceType.RX)
                sTraceTypeName = "RX";
            else if (eTraceType == TraceType.TX)
                sTraceTypeName = "TX";

            for (int nRetryIndex = 0; nRetryIndex <= 4; nRetryIndex++)
            {
                if (eTraceType == TraceType.RX)
                    //nNumber = ElanTouch.GetRXTrace(2, 1000, m_nDeviceIndex);
                    nNumber = ElanTouchSwitch.GetRXTXTrace(true, m_nDeviceIndex, m_bSocketConnectType);
                else if (eTraceType == TraceType.TX)
                    //nNumber = ElanTouch.GetTXTrace(2, 1000, m_nDeviceIndex);
                    nNumber = ElanTouchSwitch.GetRXTXTrace(false, m_nDeviceIndex, m_bSocketConnectType);
                //nNumber = GetTraceNumberBySpecificCommand(eTraceType);

                if (nRetryIndex == 0)
                    OutputMessage(string.Format("-Get {0}TraceNumber={1}", sTraceTypeName, nNumber));
                else
                    OutputMessage(string.Format("-Get {0}TraceNumber={1}(RetryCount={2})", sTraceTypeName, nNumber, nRetryIndex - 1));

                Thread.Sleep(m_nNormalDelayTime);

                if (nNumber > 0)
                    break;

                if (m_cfrmParent.m_bExecute == false)
                    break;
            }

            nTraceNumber = nNumber;

            //Check the Trace number
            if (nNumber <= 0)
            {
                m_sErrorMessage = string.Format("Get {0} Trace Error", sTraceTypeName);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 透過特定命令取得 Trace 數量
        /// </summary>
        /// <param name="eTraceType">Trace 類型（TraceType.RX 或 TraceType.TX）</param>
        /// <returns>Trace 數量（> 0 表示成功，0 表示失敗或無效）</returns>
        private int GetTraceNumberBySpecificCommand(TraceType eTraceType)
        {
            int nNumber = 0;

            byte[] byteCommand_Array = new byte[] { 0x5B, 0x00, 0x00, 0x00, 0x00, 0x00 };
            OutputMessage("-Get Trace Number(0x5B, 0x00, 0x00, 0x00, 0x00, 0x00)");
            SendDevCommand(byteCommand_Array);

            byte[] byteData_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];

            //int nResultFlag = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_nDevIdx);
            int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType);

            //if (nResultFlag == ElanTouch.TP_SUCCESS)
            if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == true)
            {
                string sMessage = "-Get ACK :";

                for (int nIndex = 0; nIndex < byteData_Array.Length; nIndex++)
                    sMessage += string.Format(" {0}", byteData_Array[nIndex].ToString("X2"));

                m_cDebugLog.WriteLogToBuffer(sMessage);

                if (byteData_Array[0] == 0x9B)
                {
                    if (eTraceType == TraceType.RX)
                        nNumber = Convert.ToInt32(byteData_Array[2]);
                    else if (eTraceType == TraceType.TX)
                        nNumber = Convert.ToInt32(byteData_Array[3]);
                }
            }
            else
            {
                OutputMessage(string.Format("-Get Trace Number Error(0x{0})", nResultFlag.ToString("X4")));
                m_cDebugLog.WriteLogToBuffer(string.Format("-TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
            }

            return nNumber;
        }

        /// <summary>
        /// 設定測試模式開關
        /// </summary>
        /// <param name="bEnable">true: 進入測試模式; false: 退出測試模式</param>
        /// <param name="bOutputMessage">是否輸出日誌訊息（預設 true）</param>
        /// <param name="bForceFlag">是否強制執行，忽略當前狀態檢查（預設 false）</param>
        private void SetTestModeEnable(bool bEnable, bool bOutputMessage = true, bool bForceFlag = false)
        {
#if _USE_9F07_SOCKET
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return;
#endif

            if (bEnable == true)
            {
                if (m_bEnterTestMode == true && bForceFlag == false)
                    return;

                if (bOutputMessage == true)
                    OutputMessage("-Enter Test Mode(0x55, 0x55, 0x55, 0x55)");

                ElanTouchSwitch.EnableTestMode(true, m_nDeviceIndex, m_bSocketConnectType);
                m_bEnterTestMode = true;
            }
            else
            {
                if (m_bEnterTestMode == false && bForceFlag == false)
                    return;

                if (bOutputMessage == true)
                    OutputMessage("-Exit Test Mode(0xA5, 0xA5, 0xA5, 0xA5)");

                ElanTouchSwitch.EnableTestMode(false, m_nDeviceIndex, m_bSocketConnectType);
                m_bEnterTestMode = false;
            }

            Thread.Sleep(m_nNormalDelayTime);
        }

#if _USE_9F07_SOCKET
        /// <summary>
        /// 設定資料取得模式開關（Gen9 9F07 專用）
        /// </summary>
        /// <remarks>
        /// 控制 Gen9 (9F07) IC 進入或退出資料取得模式，用於收集感測資料：
        /// <list type="number">
        /// <item><strong>進入資料取得模式（bEnable = true）</strong>:
        ///   <list type="bullet">
        ///   <item>輸出訊息: "-Enter Get Data Mode"（若 bOutputMessage = true）</item>
        ///   <item>發送命令: 0x55, 0x56, 0x00, {byteDataType}（4 位元組）</item>
        ///   <item>命令格式:
        ///     <para>[0-1]: 0x55, 0x56 - 進入資料取得模式的標頭</para>
        ///     <para>[2]: 0x00 - 保留位元組</para>
        ///     <para>[3]: byteDataType - 資料類型參數，指定要收集的資料種類</para>
        ///   </item>
        ///   <item>延遲 100 毫秒，確保 IC 完成模式切換</item>
        ///   </list>
        /// </item>
        /// <item><strong>退出資料取得模式（bEnable = false）</strong>:
        ///   <list type="bullet">
        ///   <item>輸出訊息: "-Exit Get Data Mode"（若 bOutputMessage = true）</item>
        ///   <item>發送命令: 0xA5, 0xA5, 0xA5, 0xA5（4 個連續的 0xA5）</item>
        ///   <item>延遲時間: ParamFingerAutoTuning.m_nExitTestModeDelayTime_9F07（可配置的退出延遲）</item>
        ///   <item>退出命令與一般測試模式退出命令相同</item>
        ///   </list>
        /// </item>
        /// </list>
        /// <para>Gen9 (9F07) 特性：</para>
        /// <list type="bullet">
        /// <item>Gen9 使用不同的資料取得機制，與舊世代的測試模式不同</item>
        /// <item>進入命令使用 0x55, 0x56 而非 0x55, 0x55, 0x55, 0x55</item>
        /// <item>支援透過 byteDataType 參數指定不同的資料類型</item>
        /// <item>退出延遲時間可調整，適應不同的硬體響應時間</item>
        /// </list>
        /// <para>byteDataType 參數說明（根據實作而定）：</para>
        /// <list type="bullet">
        /// <item>0x00: 預設資料類型（可能是原始資料）</item>
        /// <item>其他值: 特定資料類型（如差分資料、基線資料等）</item>
        /// </list>
        /// <para>此方法僅適用於 Gen9 (9F07) IC，舊世代使用 SetTestModeEnable 方法</para>
        /// </remarks>
        /// <param name="bEnable">true: 進入資料取得模式; false: 退出資料取得模式</param>
        /// <param name="byteDataType">資料類型參數，指定要收集的資料種類（預設 0x00）</param>
        /// <param name="bOutputMessage">是否輸出日誌訊息（預設 true）</param>
        private void SetGetDataModeEnable_9F07(bool bEnable, byte byteDataType = 0x00, bool bOutputMessage = true)
        {
            if (bEnable == true)
            {
                if (bOutputMessage == true)
                    OutputMessage("-Enter Get Data Mode");

                ElanTouchSwitch.SendDevCommand(new byte[] { 0x55, 0x56, 0x00, byteDataType }, 0, m_bSocketConnectType);
                Thread.Sleep(100);
            }
            else
            {
                if (bOutputMessage == true)
                    OutputMessage("-Exit Get Data Mode");

                ElanTouchSwitch.SendDevCommand(new byte[] { 0xA5, 0xA5, 0xA5, 0xA5 }, 0, m_bSocketConnectType);
                Thread.Sleep(ParamFingerAutoTuning.m_nExitTestModeDelayTime_9F07);
            }
        }

        /// <summary>
        /// 設定掃描模式開關
        /// </summary>
        /// <remarks>
        /// 控制 IC 的掃描模式啟用或停用，用於控制感測器是否持續掃描觸控訊號：
        /// <list type="number">
        /// <item><strong>停用掃描模式（bDisable = true）</strong>:
        ///   <list type="bullet">
        ///   <item>輸出訊息: "-Disable Scan Mode"（若 bOutputMessage = true）</item>
        ///   <item>發送命令: 0x54, 0xD1, 0x00, 0x55（4 位元組）</item>
        ///   <item>命令格式:
        ///     <para>[0]: 0x54 - 寫入命令標頭</para>
        ///     <para>[1]: 0xD1 - 掃描模式控制暫存器位址</para>
        ///     <para>[2]: 0x00 - 高位元組（通常為 0）</para>
        ///     <para>[3]: 0x55 - 停用掃描模式的控制值</para>
        ///   </item>
        ///   <item>更新狀態: 設定 m_bDisableScanMode_9F07 = true</item>
        ///   </list>
        /// </item>
        /// <item><strong>啟用掃描模式（bDisable = false）</strong>:
        ///   <list type="bullet">
        ///   <item>輸出訊息: "-Enable Scan Mode"（若 bOutputMessage = true）</item>
        ///   <item>發送命令: 0x54, 0xD1, 0x00, 0xA5（4 位元組）</item>
        ///   <item>命令格式:
        ///     <para>[0]: 0x54 - 寫入命令標頭</para>
        ///     <para>[1]: 0xD1 - 掃描模式控制暫存器位址</para>
        ///     <para>[2]: 0x00 - 高位元組（通常為 0）</para>
        ///     <para>[3]: 0xA5 - 啟用掃描模式的控制值</para>
        ///   </item>
        ///   <item>更新狀態: 設定 m_bDisableScanMode_9F07 = false</item>
        ///   </list>
        /// </item>
        /// <item><strong>延遲處理</strong>:
        ///   <list type="bullet">
        ///   <item>命令發送後延遲 m_nNormalDelayTime，確保 IC 完成模式切換</item>
        ///   </list>
        /// </item>
        /// </list>
        /// <para>掃描模式說明：</para>
        /// <list type="bullet">
        /// <item>掃描模式啟用時，IC 持續掃描觸控面板並產生觸控資料</item>
        /// <item>停用掃描模式可用於：
        ///   <list type="number">
        ///   <item>節省電力（不需要觸控功能時）</item>
        ///   <item>進行參數設定或校正時避免干擾</item>
        ///   <item>收集特定時間點的靜態資料</item>
        ///   </list>
        /// </item>
        /// <item>控制值 0x55 和 0xA5 是常見的啟用/停用控制模式</item>
        /// </list>
        /// <para>變數命名注意：</para>
        /// <list type="bullet">
        /// <item>m_bDisableScanMode_9F07 雖然命名包含 "_9F07"，但此方法適用於多個 IC 世代</item>
        /// <item>變數名稱可能源自最初針對 Gen9 (9F07) 的開發</item>
        /// </list>
        /// </remarks>
        /// <param name="bDisable">true: 停用掃描模式; false: 啟用掃描模式</param>
        /// <param name="bOutputMessage">是否輸出日誌訊息（預設 true）</param>
        private void SetScanModeDisable(bool bDisable, bool bOutputMessage = true)
        {
            if (bDisable == true)
            {
                if (bOutputMessage == true)
                    OutputMessage("-Disable Scan Mode");

                ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xD1, 0x00, 0x55 }, 0, m_bSocketConnectType);
                m_bDisableScanMode_9F07 = true;
            }
            else
            {
                if (bOutputMessage == true)
                    OutputMessage("-Enable Scan Mode");

                ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xD1, 0x00, 0xA5 }, 0, m_bSocketConnectType);
                m_bDisableScanMode_9F07 = false;
            }

            Thread.Sleep(m_nNormalDelayTime);
        }

        /// <summary>
        /// 跳過未使用的回應資料
        /// </summary>
        /// <remarks>
        /// 清空裝置緩衝區中殘留的回應資料，確保後續讀取的是最新的資料：
        /// <list type="number">
        /// <item><strong>持續讀取緩衝區</strong>:
        ///   <list type="bullet">
        ///   <item>建立緩衝區陣列（大小為 ElanCommand.m_nIN_DATA_LENGTH）</item>
        ///   <item>使用無限迴圈持續呼叫 ReadDevData 讀取資料</item>
        ///   <item>每次讀取都檢查操作狀態（CheckTPState）</item>
        ///   </list>
        /// </item>
        /// <item><strong>判斷結束條件</strong>:
        ///   <list type="bullet">
        ///   <item>若 CheckTPState 返回 true（讀取成功）: 繼續讀取下一筆資料</item>
        ///   <item>若 CheckTPState 返回 false（無資料或讀取失敗）: 跳出迴圈</item>
        ///   <item>當緩衝區清空時，ReadDevData 會返回失敗狀態，此時即可停止</item>
        ///   </list>
        /// </item>
        /// <item><strong>延遲處理</strong>:
        ///   <list type="bullet">
        ///   <item>清空完成後延遲 m_nNormalDelayTime，確保緩衝區穩定</item>
        ///   </list>
        /// </item>
        /// </list>
        /// <para>使用時機：</para>
        /// <list type="bullet">
        /// <item>發送命令前清空舊資料，避免誤讀先前的回應</item>
        /// <item>模式切換後清空過渡期產生的無效資料</item>
        /// <item>錯誤恢復時清除異常狀態下累積的資料</item>
        /// <item>重新開始資料收集流程前確保乾淨的起始狀態</item>
        /// </list>
        /// <para>設計考量：</para>
        /// <list type="bullet">
        /// <item>使用迴圈而非固定次數讀取，確保完全清空不定量的殘留資料</item>
        /// <item>不檢查讀取的資料內容，因為這些資料都將被丟棄</item>
        /// <item>透過讀取失敗狀態判斷緩衝區已清空，而非設定固定次數</item>
        /// </list>
        /// <para>此方法對於確保資料同步和避免資料污染非常重要</para>
        /// </remarks>
        private void SkipNoUsedResponseData()
        {
            byte[] byteBuffer_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];

            while (true)
            {
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteBuffer_Array, 0, m_bSocketConnectType);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == true)
                    continue;
                else
                    break;
            }

            Thread.Sleep(m_nNormalDelayTime);
        }
#endif

        /// <summary>
        /// 設定 Report 功能開關
        /// </summary>
        /// <param name="bEnable">true: 啟用 Report 功能; false: 停用 Report 功能</param>
        /// <param name="bOutputMessage">是否輸出日誌訊息（預設 true）</param>
        private void SetReportEnable(bool bEnable, bool bOutputMessage = true)
        {
#if _USE_9F07_SOCKET
            /*
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return;
            */
#endif

            if (bEnable == true)
            {
                if (bOutputMessage == true)
                    OutputMessage("-Enable Report(0x54, 0xCA, 0x00, 0x00)");

                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    //ElanTouch.DisableTPReport(true, 1000, m_nDeviceIndex);
                    ElanTouchSwitch.DisableTPReport(false, m_nDeviceIndex, m_bSocketConnectType);
                    m_bDisableReport = false;
                }
#if _USE_9F07_SOCKET
                else
                {

                    ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xCA, 0x00, 0x00 }, 0, m_bSocketConnectType);
                    m_bDisableReport = false;
                }
#endif
            }
            else
            {
                if (bOutputMessage == true)
                    OutputMessage("-Disable Report(0x54, 0xCA, 0x00, 0x01)");

                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    //ElanTouch.DisableTPReport(true, 1000, m_nDeviceIndex);
                    ElanTouchSwitch.DisableTPReport(true, m_nDeviceIndex, m_bSocketConnectType);
                    m_bDisableReport = true;
                }
#if _USE_9F07_SOCKET
                else
                {
                    ElanTouchSwitch.SendDevCommand(new byte[] { 0x54, 0xCA, 0x00, 0x01 }, 0, m_bSocketConnectType);
                    m_bDisableReport = true;
                }
#endif
            }

            Thread.Sleep(m_nNormalDelayTime);
        }

        /// <summary>
        /// 設定 Gen8 8F18 方案的 DCDC 開關
        /// </summary>
        /// <param name="bOutputMessage">是否輸出日誌訊息（預設 true）</param>
        /// <returns>true: DCDC 設定成功或不需要設定; false: 設定失敗</returns>
        private bool SetDCDCEnable_8F18(bool bOutputMessage = true)
        {
#if _USE_9F07_SOCKET
            /*
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return true;
            */
#endif
            if (ParamFingerAutoTuning.m_nGen8SetDCDCEnable != 0 && ParamFingerAutoTuning.m_nGen8SetDCDCEnable != 1)
                return true;

            if (m_eICGenerationType != ICGenerationType.Gen8)
                return true;

            int nRetryCount = 3;
            bool bResultFlag = false;

            SetTestModeEnable(true);

            if (ParamFingerAutoTuning.m_nGen8SetDCDCEnable == 0)
            {
                if (bOutputMessage == true)
                    OutputMessage("-Set DCDC Disable");
            }
            else if (ParamFingerAutoTuning.m_nGen8SetDCDCEnable == 1)
            {
                if (bOutputMessage == true)
                    OutputMessage("-Set DCDC Enable");
            }

            int nGetOrgValue_HB = 0x00;
            int nGetOrgValue_LB = 0x00;

            byte[] byteCommand_Array = new byte[] { 0x67, 0x00, 0x00, 0x00, 0x4C, 0x00, 0x00, 0x00, 0x00, 0x00 };

            if (bOutputMessage == true)
                OutputMessage("-Get Original DCDC Value(0x67, 0x00, 0x00, 0x00, 0x4C, 0x00, 0x00, 0x00, 0x00, 0x00)");

            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

            while (nRetryCount > 0)
            {
                byte[] byteData_Array = new byte[ElanCommand_Gen8.m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType, ParamFingerAutoTuning.m_nGen8GetACKTimeout);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                {
                    m_cfrmParent.OutputDebugLog(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(1000);
                    bResultFlag = false;
                    break;
                }

                string sGetACK = "Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                m_cfrmParent.OutputDebugLog(sGetACK);

                if (byteData_Array[0] == 0x67 && byteData_Array[4] == 0x4C && byteData_Array[5] == 0x00)
                {
                    nGetOrgValue_HB = byteData_Array[8];
                    nGetOrgValue_LB = byteData_Array[9];
                    bResultFlag = true;
                    break;
                }

                nRetryCount--;
            }

            if (bResultFlag == false)
            {
                m_sErrorMessage = "Get Original DCDC Value Error";
                SetTestModeEnable(false);
                return false;
            }

            int nSetValue_HB = 0x00;
            int nSetValue_LB = 0x00;

            if (ParamFingerAutoTuning.m_nGen8SetDCDCEnable == 0)
            {
                nSetValue_HB = nGetOrgValue_HB & 0xFF;
                nSetValue_LB = nGetOrgValue_LB & 0x7F;
            }
            else if (ParamFingerAutoTuning.m_nGen8SetDCDCEnable == 1)
            {
                nSetValue_HB = nGetOrgValue_HB | 0x00;
                nSetValue_LB = nGetOrgValue_LB | 0x80;
            }

            nRetryCount = 3;

            int nGetNewValue_HB = 0x00;
            int nGetNewValue_LB = 0x00;

            byteCommand_Array = new byte[] { 0x68, 0x00, 0x00, 0x00, 0x4C, 0x00, 0x00, 0x00, (byte)nSetValue_HB, (byte)nSetValue_LB };

            if (bOutputMessage == true)
                OutputMessage(string.Format("-Set DCDC Value(0x68, 0x00, 0x00, 0x00, 0x4C, 0x00, 0x00, 0x00, 0x{0}, 0x{1})", nSetValue_HB.ToString("X2"), nSetValue_LB.ToString("X2")));

            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

            byteCommand_Array = new byte[] { 0x67, 0x00, 0x00, 0x00, 0x4C, 0x00, 0x00, 0x00, 0x00, 0x00 };

            if (bOutputMessage == true)
                OutputMessage("-Get Set DCDC Value(0x67, 0x00, 0x00, 0x00, 0x4C, 0x00, 0x00, 0x00, 0x00, 0x00)");

            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

            while (nRetryCount > 0)
            {
                byte[] byteData_Array = new byte[ElanCommand_Gen8.m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType, ParamFingerAutoTuning.m_nGen8GetACKTimeout);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                {
                    m_cfrmParent.OutputDebugLog(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(1000);
                    bResultFlag = false;
                    break;
                }

                string sGetACK = "Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                m_cfrmParent.OutputDebugLog(sGetACK);

                if (byteData_Array[0] == 0x67 && byteData_Array[4] == 0x4C && byteData_Array[5] == 0x00)
                {
                    nGetNewValue_HB = byteData_Array[8];
                    nGetNewValue_LB = byteData_Array[9];
                    bResultFlag = true;
                    break;
                }

                nRetryCount--;
            }

            if (bResultFlag == false)
            {
                m_sErrorMessage = "Get New Setting DCDC Value Error";
                SetTestModeEnable(false);
                return false;
            }

            SetTestModeEnable(false);

            int nDCDCBit = (nGetNewValue_LB & 0x80) >> 7;

            if (ParamFingerAutoTuning.m_nGen8SetDCDCEnable == 0 && nDCDCBit != 0)
            {
                m_sErrorMessage = "Set DCDC Disable Error";
                return false;
            }
            else if (ParamFingerAutoTuning.m_nGen8SetDCDCEnable == 1 && nDCDCBit != 1)
            {
                m_sErrorMessage = "Set DCDC Enable Error";
                return false;
            }

            nGetOrgValue_HB = nGetOrgValue_HB & 0x3F;
            nGetOrgValue_LB = nGetOrgValue_LB & 0x7F;

            nGetNewValue_HB = nGetNewValue_HB & 0x3F;
            nGetNewValue_LB = nGetNewValue_LB & 0x7F;

            if (nGetOrgValue_HB != nGetNewValue_HB || nGetOrgValue_LB != nGetNewValue_LB)
            {
                m_sErrorMessage = "Set Address:0x4C00 Value Error";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 設定Gen8 IC的快速更新Base功能,進入測試模式後發送0x6D命令,退出測試模式並依據參數決定延遲時間
        /// </summary>
        /// <param name="bOutputMessage">是否輸出訊息旗標,true時輸出設定訊息,預設為true</param>
        /// <param name="bSetUpdateBaseDelayTime">是否使用UpdateBase延遲時間旗標,true時使用UpdateBase延遲,false時使用一般命令延遲,預設為true</param>
        private void SetFast_UpdateBase_Gen8(bool bOutputMessage = true, bool bSetUpdateBaseDelayTime = true)
        {
            if (bOutputMessage == true)
                OutputMessage("-Set Fast UpdateBase(Generation 8)");

            SetTestModeEnable(true);

            byte[] byteCommand_Array = new byte[] { 0x6D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            if (bOutputMessage == true)
                OutputMessage("-Set Fast UpdateBase(0x6D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00)");

            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

            SetTestModeEnable(false);

            if (bSetUpdateBaseDelayTime == true)
                Thread.Sleep(ParamFingerAutoTuning.m_nGen8UpdateBaseDelayTime);
            else
                Thread.Sleep(ParamFingerAutoTuning.m_nGen8SendCommandDelayTime);
        }

        /// <summary>
        /// 設定 Gen8 的基線重置（Base Reset）
        /// </summary>
        /// <param name="bOutputMessage">是否輸出日誌訊息（預設 true）</param>
        /// <param name="bSetUpdateBaseDelayTime">是否使用基線更新延遲時間（預設 true），false 則使用一般命令延遲</param>
        /// <returns>1: 成功; 0: 失敗; -1: 演算法基線幀數為 0</returns>
        private int SetReset_Base_Gen8(bool bOutputMessage = true, bool bSetUpdateBaseDelayTime = true)
        {
            if (bOutputMessage == true)
                OutputMessage("-Set Reset Base(Generation 8)");

            int nRetryCount = 3;
            bool bResultFlag = false;

            int nAlgorithmFrameNumber = 0;

            SetTestModeEnable(false);

            byte[] byteCommand_Array = new byte[] { 0x53, 0xC4, 0x00, 0x00 };

            while (nRetryCount > 0)
            {
                if (bOutputMessage == true)
                    OutputMessage("-Get Algorithm Base Frame Number(0x53, 0xC4, 0x00, 0x00)");

                ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

                byte[] byteData_Array = new byte[ElanCommand_Gen8.m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType, ParamFingerAutoTuning.m_nGen8GetACKTimeout);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                {
                    m_cfrmParent.OutputDebugLog(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(200);
                    bResultFlag = false;
                    nRetryCount--;
                    continue;
                }

                string sGetACK = "Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                m_cfrmParent.OutputDebugLog(sGetACK);

                if (byteData_Array[0] == 0x52 && byteData_Array[1] == 0xC4)
                {
                    int nFrameNumber = ((byteData_Array[2] & 0x00FF) << 8) + (byteData_Array[3] & 0x00FF);
                    nAlgorithmFrameNumber = nFrameNumber;
                    bResultFlag = true;
                    break;
                }

                Thread.Sleep(100);
                nRetryCount--;
            }

            if (bResultFlag == false)
            {
                m_sErrorMessage = "Get Algorithm Base Frame Number Error";
                return 0;
            }
            else if (nAlgorithmFrameNumber == 0)
                return -1;

            byteCommand_Array = new byte[] { 0x54, 0xC4, 0x00, 0x00 };

            if (bOutputMessage == true)
                OutputMessage("-Set Reset Base(0x54, 0xC4, 0x00, 0x00)");

            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

            if (bSetUpdateBaseDelayTime == true)
                Thread.Sleep(ParamFingerAutoTuning.m_nGen8UpdateBaseDelayTime);
            else
                Thread.Sleep(ParamFingerAutoTuning.m_nGen8SendCommandDelayTime);

            nRetryCount = 20;

            while (nRetryCount > 0)
            {
                byteCommand_Array = new byte[] { 0x53, 0xC5, 0x00, 0x00 };

                if (bOutputMessage == true)
                    OutputMessage("-Get Base Frame Count(0x53, 0xC5, 0x00, 0x00)");

                ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

                byte[] byteData_Array = new byte[ElanCommand_Gen8.m_nIN_DATA_LENGTH];
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType, ParamFingerAutoTuning.m_nGen8GetACKTimeout);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                {
                    m_cfrmParent.OutputDebugLog(string.Format("TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                    Thread.Sleep(200);
                    bResultFlag = false;
                    nRetryCount--;
                    continue;
                }

                string sGetACK = "Get ACK :";

                for (int nByteIndex = 0; nByteIndex < byteData_Array.Length; nByteIndex++)
                    sGetACK += string.Format(" {0}", byteData_Array[nByteIndex].ToString("X2"));

                m_cfrmParent.OutputDebugLog(sGetACK);

                if (byteData_Array[0] == 0x52 && byteData_Array[1] == 0xC5)
                {
                    int nFrameCount = ((byteData_Array[2] & 0x00FF) << 8) + (byteData_Array[3] & 0x00FF);

                    if (nFrameCount >= nAlgorithmFrameNumber)
                    {
                        bResultFlag = true;
                        break;
                    }
                }

                Thread.Sleep(20);
                nRetryCount--;
            }

            if (bResultFlag == false)
            {
                m_sErrorMessage = "Get Base Frame Count Error";
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// 取得韌體版本並判斷 IC 世代與方案類型
        /// </summary>
        /// <returns>true: 成功取得並解析韌體版本; false: 讀取失敗或版本無效</returns>
        private bool GetFW_Version()
        {
#if _USE_9F07_SOCKET
            if (m_eICGenerationType == ICGenerationType.Gen9)
            {
                m_cfrmParent.m_sICSolutionName = "9F07";
                OutputMessage("-Get IC Type : 9F07");
                return true;
            }
#endif

            UInt32 nFWVersion = 0;
            bool bSuccessFlag = false;

            for (int nRetryIndex = 0; nRetryIndex <= 4; nRetryIndex++)
            {
                bSuccessFlag = ElanTouchSwitch.GetFWVersion(ref nFWVersion, m_nDeviceIndex, m_bSocketConnectType);

                if (nRetryIndex == 0)
                    OutputMessage(string.Format("-Get FW_Version=0x{0}", nFWVersion.ToString("X4").ToUpper()));
                else
                    OutputMessage(string.Format("-Get FW_Version=0x{0}(RetryCount={1})", nFWVersion.ToString("X4").ToUpper(), nRetryIndex - 1));

                Thread.Sleep(m_nNormalDelayTime);

                if (m_cfrmParent.m_bExecute == false)
                    break;

                if (bSuccessFlag == true)
                    break;
            }

            if (bSuccessFlag == true)
            {
                m_nFWVersion = nFWVersion;
                int nFWVersion_HighByte = ((int)nFWVersion & 0xFF00) >> 8;
                int nGen8Type = (nFWVersion_HighByte & 0xF0) >> 4;

                //Check the FW Version
                if (nFWVersion == 0x0000)
                {
                    m_sErrorMessage = string.Format("Get FW_Version Value Error(FW_Version=0x{0})", nFWVersion.ToString("X4"));
                    m_eICGenerationType = ICGenerationType.None;
                    return false;
                }
                else if (nGen8Type == 0x8)
                {
                    m_eICGenerationType = ICGenerationType.Gen8;
                    m_nICType = ElanDefine.Gen902_ICType;
                    m_eICSolutionType = MainConstantParameter.SetICSolutionType_Gen8(nFWVersion_HighByte);
                    string sSolutionName = MainConstantParameter.GetICSolutionType_Gen8(m_eICSolutionType);
                    m_cfrmParent.m_sICSolutionName = string.Format("Generation 8({0})", sSolutionName);
                    OutputMessage(string.Format("-Get IC Type : Generation 8({0})", sSolutionName));

                    ParamFingerAutoTuning.LoadGen8FWParameterAddress(m_eICSolutionType, frmMain.m_sGen8FWParameterAddressIniPath);
                    OutputMessage("-Load Generation 8 FW Parameter Address Finish");
                }
                else if (nFWVersion_HighByte == 0x64 || nFWVersion_HighByte == 0x65 ||
                         nFWVersion_HighByte == 0x67 || nFWVersion_HighByte == 0x68)
                {
                    m_eICGenerationType = ICGenerationType.Gen7;
                    m_eICSolutionType = MainConstantParameter.SetICSolutionType_Gen7(nFWVersion_HighByte);
                    string sSolutionName = MainConstantParameter.GetICSolutionType_Gen7(m_eICSolutionType);
                    m_cfrmParent.m_sICSolutionName = string.Format("Generation 7({0})", sSolutionName);
                    OutputMessage(string.Format("-Get IC Type : Generation 7({0})", sSolutionName));
                }
                else if (nFWVersion_HighByte == 0x59 || nFWVersion_HighByte == 0x61 ||
                         nFWVersion_HighByte == 0x62 || nFWVersion_HighByte == 0x63)
                {
                    m_eICGenerationType = ICGenerationType.Gen6;
                    m_eICSolutionType = MainConstantParameter.SetICSolutionType_Gen6(nFWVersion_HighByte);
                    string sSolutionName = MainConstantParameter.GetICSolutionType_Gen6(m_eICSolutionType);
                    m_cfrmParent.m_sICSolutionName = string.Format("Generation 6({0})", sSolutionName);
                    OutputMessage(string.Format("-Get IC Type : Generation 6({0})", sSolutionName));
                }
                else
                {
                    m_eICGenerationType = ICGenerationType.Other;
                    m_cfrmParent.m_sICSolutionName = "Other";
                    OutputMessage("-Get IC Type : Other");
                }

                return true;
            }
            else
            {
                m_sErrorMessage = "Get FW_Version Error";
                m_cfrmParent.m_sICSolutionName = "";
                m_eICGenerationType = ICGenerationType.None;
                return false;
            }
        }

        /// <summary>
        /// 設定 IC 重置（Reset）
        /// </summary>
        /// <param name="bSetDelayFlag">是否額外延遲 3000 毫秒（預設 true），僅在 bCheckCompleteFlag=false 時有效</param>
        /// <param name="bCheckCompleteFlag">是否等待並驗證重置完成訊號（預設 false）</param>
        /// <param name="bOutputMessageFlag">是否輸出日誌訊息（預設 true）</param>
        private void SetReset(bool bSetDelayFlag = true, bool bCheckCompleteFlag = false, bool bOutputMessageFlag = true)
        {
#if _USE_9F07_SOCKET
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return;
#endif

            int nCount = 0;
            int nLimitCount = 10;

            if (bOutputMessageFlag == true)
                OutputMessage("-Set Reset(0x77, 0x77, 0x77, 0x77)");

            byte[] byteCommand_Array = new byte[] { 0x77, 0x77, 0x77, 0x77 };
            SendDevCommand(byteCommand_Array);

            if (bCheckCompleteFlag == true)
            {
                byte[] byteData_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];

                while (nCount <= nLimitCount)
                {
                    int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType, 3000);

                    if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == true)
                    {
                        string sResponseMessage = string.Format("-Get Reset Response(0x{0}, 0x{1}, 0x{2}, 0x{3})", byteData_Array[0].ToString("X2"), byteData_Array[1].ToString("X2"), byteData_Array[2].ToString("X2"), byteData_Array[3].ToString("X2"));
                        OutputMessage(sResponseMessage);

                        if (byteData_Array[0] == 0x55 && byteData_Array[1] == 0x55 && byteData_Array[2] == 0x55 && byteData_Array[3] == 0x55)
                        {
                            break;
                        }
                        else
                        {
                            nCount++;
                            continue;
                        }
                    }
                    else
                        nCount++;
                }
            }
            else
            {
                Thread.Sleep(m_nNormalDelayTime);

                if (bSetDelayFlag == true)
                    Thread.Sleep(3000);
            }

            m_bEnterTestMode = false;
        }

        /// <summary>
        /// 發送測試命令（取得 FW ID）
        /// </summary>
        private void SendTestCommand()
        {
            SetTestModeEnable(false);
            Thread.Sleep(1000);

            byte[] byteCommand_Array = new byte[] { 0x53, 0xF0, 0x00, 0x01 };
            OutputMessage("-Send Test Command(Get FWID(0x53, 0xF0, 0x00, 0x01))");

            SendDevCommand(byteCommand_Array);

            byte[] byteData_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];
            //int nResultFlag = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_nDeviceIndex);
            int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType);

            //if (nResultFlag != ElanTouch.TP_SUCCESS)
            if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                m_cDebugLog.WriteLogToBuffer(string.Format("Send Test Command Error.TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
            else
            {
                string sMessage = "Send Test Command Success.Get ACK :";

                for (int nDataIndex = 0; nDataIndex < byteData_Array.Length; nDataIndex++)
                    sMessage += string.Format(" {0}", byteData_Array[nDataIndex].ToString("X2"));

                m_cDebugLog.WriteLogToBuffer(sMessage);
            }

            SetTestModeEnable(true);
        }

        /// <summary>
        /// 發送檢查命令並讀取 16 位元回應值
        /// </summary>
        /// <param name="byteCommand_Array">要發送的命令位元組陣列</param>
        /// <param name="byteSymbol">命令符號/暫存器位址，用於驗證回應是否對應此命令</param>
        /// <returns>16 位元回應值（成功時），或 0（失敗或使用者中止時）</returns>
        private int SendCheckCommand(byte[] byteCommand_Array, byte byteSymbol)
        {
            int nCount = 0;
            // int nLimitCount = 10;
            int nLimitCount = 2;
            int nOutputValue = 0;

            SendDevCommand(byteCommand_Array);

            byte[] byteData_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];

            while (nCount <= nLimitCount)
            {
                if (!m_cfrmParent.m_bExecute)
                    return nOutputValue;

                //int nResultFlag = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, 1000, m_nDeviceIndex);
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType);

                //if (nResultFlag == ElanTouch.TP_SUCCESS)
                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == true)
                {
                    if (byteData_Array[0] == 0x52 && byteData_Array[1] == byteSymbol)
                    {
                        nOutputValue = (byteData_Array[2] << 8) | (byteData_Array[3]);
                        break;
                    }
                    else
                    {
                        nCount++;
                        continue;
                    }
                }
                else
                    nCount++;
            }

            return nOutputValue;
        }

        /// <summary>
        /// 執行 ReK (Re-Calibration) 重新校正流程
        /// </summary>
        /// <param name="nReKTimeout">ReK 流程的逾時時間（毫秒）</param>
        /// <param name="bSetOriginFlag">是否為原始模式設定（影響使用者中止檢查，預設 false）</param>
        /// <returns>SetState 枚舉：Success（成功），Error（失敗或使用者中止），或其他錯誤狀態</returns>
        private AppCoreDefine.SetState SetReK(int nReKTimeout, bool bSetOriginFlag = false)
        {
            if (ParamFingerAutoTuning.m_nDisableReK != 1 && (m_eICGenerationType == ICGenerationType.Other || m_eICGenerationType == ICGenerationType.Gen7 || m_eICGenerationType == ICGenerationType.Gen6))
            {
                if (m_cfrmParent.m_bExecute == false && bSetOriginFlag == false)
                    return AppCoreDefine.SetState.Error;

                //OutputMessage("-Set ReK");

                if ((ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT ||
                     ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL) &&
                    (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_RISING_HALF ||
                     ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_RISING ||
                     ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING_HALF ||
                     ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_SPI_MA_FALLING))
                {
                    AppCoreDefine.SetState eSetState = SendCheckReK(nReKTimeout);
                    //OutputMessage(string.Format("-Get ReK Reply({0})", bSetState));

                    if (eSetState != AppCoreDefine.SetState.Success)
                        return eSetState;
                }
                else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER | ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
                {
                    AppCoreDefine.SetState eSetState = SendCheckReK(nReKTimeout);
                    //OutputMessage(string.Format("-Get ReK Reply({0})", bSetState));

                    if (eSetState != AppCoreDefine.SetState.Success)
                        return eSetState;
                }
                else
                {
                    //if (ElanTouch.ReK(nReKTimeout, false, m_nDeviceIndex) != ElanTouch.TP_SUCCESS)
                    /*
                    if (ElanTouchSwitch.SetReK(nReKTimeout, m_nDeviceIndex, m_bSocketConnectType) == false)
                        return AppCoreDefine.SetState.Error;
                    */

                    AppCoreDefine.SetState eSetState = SendCheckReK(nReKTimeout);
                    //OutputMessage(string.Format("-Get ReK Reply({0})", bSetState));

                    if (eSetState != AppCoreDefine.SetState.Success)
                        return eSetState;
                }

                Thread.Sleep(500);
            }

            return AppCoreDefine.SetState.Success;
        }

        /// <summary>
        /// 發送並檢查 ReK (Re-Calibration) 重新校正流程
        /// </summary>
        /// <param name="nReKTimeout">ReK 流程的逾時時間（毫秒），建議值 1000-5000</param>
        /// <returns>SetState 枚舉：Success（ReK 成功），Reset（IC 重置），Error（ReK 失敗或逾時），Running（使用者中止）</returns>
        private AppCoreDefine.SetState SendCheckReK(int nReKTimeout)
        {
            int nCount = 0;
            // int nLimitCount = 10;
            int nLimitCount = 2;

            OutputMessage("-Set ReK(0x54, 0xC0, 0xE1, 0x5A)");
            byte[] byteCommand_Array = new byte[] { 0x54, 0xC0, 0xE1, 0x5A };
            SendDevCommand(byteCommand_Array);
            Thread.Sleep(m_nNormalDelayTime);

            OutputMessage("-Set ReK(0x54, 0x29, 0x00, 0x01)");
            byteCommand_Array = new byte[] { 0x54, 0x29, 0x00, 0x01 };
            SendDevCommand(byteCommand_Array);

            byte[] byteData_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];

            while (nCount <= nLimitCount)
            {
                if (!m_cfrmParent.m_bExecute)
                    return AppCoreDefine.SetState.Running;

                //int nResultFlag = ElanTouch.ReadDevData(byteData_Array, byteData_Array.Length, nReKTimeout, m_nDevIdx);
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteData_Array, m_nDeviceIndex, m_bSocketConnectType, nReKTimeout);

                //if (nResultFlag == ElanTouch.TP_SUCCESS)
                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == true)
                {
                    string sResponseMessage = string.Format("-Get ReK Response(0x{0}, 0x{1}, 0x{2}, 0x{3})", byteData_Array[0].ToString("X2"), byteData_Array[1].ToString("X2"), byteData_Array[2].ToString("X2"), byteData_Array[3].ToString("X2"));
                    OutputMessage(sResponseMessage);

                    if (byteData_Array[0] == 0x66 && byteData_Array[1] == 0x66 && byteData_Array[2] == 0x66 && byteData_Array[3] == 0x66)
                    {
                        return AppCoreDefine.SetState.Success;
                    }
                    else if (byteData_Array[0] == 0x55 && byteData_Array[1] == 0x55 && byteData_Array[2] == 0x55 && byteData_Array[3] == 0x55)
                    {
                        return AppCoreDefine.SetState.Reset;
                    }
                    else if (byteData_Array[0] == 0xFF && byteData_Array[1] == 0xFF && byteData_Array[2] == 0xFF && byteData_Array[3] == 0xFF)
                    {
                        return AppCoreDefine.SetState.Error;
                    }
                    else
                    {
                        nCount++;
                        continue;
                    }
                }
                else
                    nCount++;
            }

            return AppCoreDefine.SetState.Error;
        }

        /// <summary>
        /// 取得觸控面板（TP）資訊
        /// </summary>
        /// <param name="cFlowStep">流程步驟物件（此參數未使用，可能為保留參數）</param>
        /// <returns>true: 成功取得 TP 資訊; false: 取得失敗</returns>
        private bool GetTPInformation(frmMain.FlowStep cFlowStep)
        {
            if (m_bSocketConnectType == false)
            {
                IntPtr npTraceInfo = Marshal.AllocHGlobal(Marshal.SizeOf(m_structTraceInfo));
                if (ElanTouch.GetTraceInfo(npTraceInfo, ElanDefine.DEFAULT_PARTIAL_NUM, ElanDefine.TIME_1SEC, m_nDeviceIndex) != ElanTouch.TP_SUCCESS)
                    return false;

                m_structTraceInfo = (ElanTouch.TraceInfo)Marshal.PtrToStructure(npTraceInfo, typeof(ElanTouch.TraceInfo));
                Marshal.FreeHGlobal(npTraceInfo);
            }
            else
            {
                IntPtr pTraceInfo = Marshal.AllocHGlobal(Marshal.SizeOf(m_structTraceInfo_Socket));
                if (ElanTouch_Socket.GetTraceInfo(pTraceInfo, ElanDefine.DEFAULT_PARTIAL_NUM, ElanDefine.TIME_1SEC, m_nDeviceIndex) != ElanTouch_Socket.TP_SUCCESS)
                    return false;

                m_structTraceInfo_Socket = (ElanTouch_Socket.TraceInfo)Marshal.PtrToStructure(pTraceInfo, typeof(ElanTouch_Socket.TraceInfo));
                Marshal.FreeHGlobal(pTraceInfo);
            }

            return true;
        }

        /// <summary>
        /// 設定 Trace 資訊（配置資料收集的維度）
        /// </summary>
        /// <param name="cFlowStep">流程步驟物件，用於判斷流程類型和參數</param>
        /// <param name="nExecuteIndex">執行索引（此參數未使用，可能為保留參數）</param>
        private void SetTraceInformation(frmMain.FlowStep cFlowStep, int nExecuteIndex)
        {
            if (cFlowStep.m_eStep == MainStep.Self_FrequencySweep ||
                cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
            {
                string sMessage = "";

                #region Mark It.
                /*
                if (ParamFingerAutoTuning.m_nSelfFSGetDataType != 1 ||
                    cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep)
                {
                    int nXValue = m_nRXTraceNumber;

                    if (m_eSelfTraceType == TraceType.RX)
                        nXValue = m_nRXTraceNumber;
                    else if (m_eSelfTraceType == TraceType.TX)
                        nXValue = m_nTXTraceNumber;

                    int nYValue = m_cSelfParameter.m_nSum;

                    if ((cFlowStep.m_eStep == MainStep.Self_FrequencySweep && ParamFingerAutoTuning.m_nSelfFSGetKValue == 1) ||
                        (cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep && ParamFingerAutoTuning.m_nSelfPNSGetKValue == 1))
                        nYValue += 2;

                    SetTraceInformation(nXValue, nYValue);
                    m_nRXTraceNumber = nXValue;
                    m_nTXTraceNumber = nYValue;

                    if (m_bSocketConnectType == false)
                        m_structTraceInfo.nYTotal = nYValue;
                    else
                        m_structTraceInfo_Socket.nYTotal = nYValue;

                    sMessage = string.Format("-Set TraceInfo X={0} Y={1}", nXValue, nYValue);
                    OutputMessage(sMessage);
                }
                */
                #endregion

                int nXValue = m_nRXTraceNumber;

                if (m_eSelfTraceType == TraceType.RX)
                    nXValue = m_nRXTraceNumber;
                else if (m_eSelfTraceType == TraceType.TX)
                    nXValue = m_nTXTraceNumber;

                int nYValue = m_cSelfParameter.m_nDFT_NUM;

                if ((cFlowStep.m_eStep == MainStep.Self_FrequencySweep && ParamFingerAutoTuning.m_nSelfFSGetKValue == 1) ||
                    (cFlowStep.m_eStep == MainStep.Self_NCPNCNSweep && ParamFingerAutoTuning.m_nSelfPNSGetKValue == 1))
                    nYValue += 2;

                SetTraceInformation(nXValue, nYValue);
                m_nRXTraceNumber = nXValue;
                m_nTXTraceNumber = nYValue;

                if (m_bSocketConnectType == false)
                    m_structTraceInfo.nYTotal = nYValue;
                else
                    m_structTraceInfo_Socket.nYTotal = nYValue;

                sMessage = string.Format("-Set TraceInfo X={0} Y={1}", nXValue, nYValue);
                OutputMessage(sMessage);
            }
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
            {
                SetTraceInformation(m_nRXTraceNumber, m_nTXTraceNumber);

                if (m_bSocketConnectType == false)
                    m_structTraceInfo.nYTotal = m_nTXTraceNumber;
                else
                    m_structTraceInfo_Socket.nYTotal = m_nTXTraceNumber;

                string sMessage = string.Format("-Set TraceInfo X={0} Y={1}", m_nRXTraceNumber, m_nTXTraceNumber);
                OutputMessage(sMessage);
            }
        }

        /// <summary>
        /// 設定 Trace 資訊並同步到底層函式庫
        /// </summary>
        /// <param name="nRXTraceNumber">X 軸 Trace 數量（RX 線數）</param>
        /// <param name="nTXTraceNumber">Y 軸 Trace 數量（TX 線數或 DFT 數量）</param>
        private void SetTraceInformation(int nRXTraceNumber, int nTXTraceNumber)
        {
            int nChipNumber = 1;

            if (m_eICGenerationType == ICGenerationType.Gen8)
                nChipNumber = 1;
            else if (m_eICGenerationType == ICGenerationType.Gen7)
                nChipNumber = 1;
            else if (m_eICGenerationType == ICGenerationType.Gen6)
                nChipNumber = 1;
            else if (m_eICGenerationType == ICGenerationType.Other)
                nChipNumber = 1;

            if (m_bSocketConnectType == false)
            {
                m_structTraceInfo.nYTotal = nTXTraceNumber;

                if (nChipNumber > 0)
                {
                    //m_structTraceInfo.nXTotal += nRXTraceNumber;
                    m_structTraceInfo.XAxis[0] = nRXTraceNumber;
                }

                if (nChipNumber > 1)
                {
                    //m_structTraceInfo.nXTotal += nRXTraceNumber;
                    m_structTraceInfo.XAxis[1] = nRXTraceNumber;
                }

                if (nChipNumber > 2)
                {
                    m_structTraceInfo.nXTotal += nRXTraceNumber;
                    m_structTraceInfo.XAxis[2] = nRXTraceNumber;
                }

                m_structTraceInfo.nPartialNum = 0;
                m_structTraceInfo.nChipNum = nChipNumber;

                // Set the trace information to library
                IntPtr npTraceInfo = Marshal.AllocHGlobal(Marshal.SizeOf(m_structTraceInfo));
                Marshal.StructureToPtr(m_structTraceInfo, npTraceInfo, true);
                ElanTouch.SetTraceInfo(npTraceInfo);
                Marshal.FreeHGlobal(npTraceInfo);
            }
            else
            {
                m_structTraceInfo_Socket.nYTotal = nTXTraceNumber;

                if (nChipNumber > 0)
                {
                    //m_structTraceInfo_Socket.nXTotal += nRXTraceNumber;
                    m_structTraceInfo_Socket.XAxis[0] = nRXTraceNumber;
                }

                if (nChipNumber > 1)
                {
                    //m_structTraceInfo_Socket.nXTotal += nRXTraceNumber;
                    m_structTraceInfo_Socket.XAxis[1] = nRXTraceNumber;
                }

                if (nChipNumber > 2)
                {
                    //m_structTraceInfo_Socket.nXTotal += nRXTraceNumber;
                    m_structTraceInfo_Socket.XAxis[2] = nRXTraceNumber;
                }

                m_structTraceInfo_Socket.nPartialNum = 0;
                m_structTraceInfo_Socket.nChipNum = nChipNumber;

                // Set the trace information to library
                IntPtr npTraceInfo = Marshal.AllocHGlobal(Marshal.SizeOf(m_structTraceInfo_Socket));
                Marshal.StructureToPtr(m_structTraceInfo_Socket, npTraceInfo, true);
                ElanTouch_Socket.SetTraceInfo(npTraceInfo);
                Marshal.FreeHGlobal(npTraceInfo);
            }
        }

        /// <summary>
        /// 設定使用新方法進行資料收集
        /// </summary>
        /// <param name="nFrameCount">幀數參數，指定要收集的資料幀數</param>
        /// <param name="nTimeout">逾時時間（毫秒），此參數在當前實作中未使用</param>
        /// <returns>true: 設定成功; false: 設定失敗（3 次重試後仍失敗）</returns>
        private bool SetUseNewMethod(int nFrameCount, int nTimeout)
        {
            int nRetryTimes = 3;
            bool bResult = false;

            if (SetPollingCommand() == false)
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);

            for (int nRetryIndex = 0; nRetryIndex < nRetryTimes; nRetryIndex++)
            {
                byte byteFrameCount = (byte)(nFrameCount & 0xFF);
                byte[] byteCommand_Array = new byte[] { 0x54, 0xCD, 0x80, byteFrameCount };
                OutputMessage(string.Format("-Send UseNewMethod Command : 0x54, 0xCD, 0x80, 0x{0}", byteFrameCount.ToString("X2")));
                SendDevCommand(byteCommand_Array);

                byte[] byteBuffer_Array = new byte[ElanCommand.m_nIN_DATA_LENGTH];

                /*
                if (SetPollingCommand() == true)
                    m_bEnablePollingDummyCommand = true;

                //啟動Thread進行Polling Dummy Command動作
                ThreadPool.QueueUserWorkItem(new WaitCallback(PollingDummyCommand));

                if (m_qFIFO.DequeueAll(3000, byteBuffer_Array, ElanCommand.IN_DATA_LENGTH) == false)
                {
                    //離開Thread
                    m_bEnablePollingDummyCommand = false;
                }
                else
                {
                    //離開Thread
                    m_bEnablePollingDummyCommand = false;
                    bResult = true;
                    break;
                }
                */

                OutputMessage("-Send Polling Command : 0x58, 0xC2, 0x00, 0x00, 0x00, 0x00");
                SendDevCommand(new byte[] { 0x58, 0xC2, 0x00, 0x00, 0x00, 0x00 });

                //int nResultFlag = ElanTouch.ReadDevData(byteBuffer_Array, byteBuffer_Array.Length, nTimeout, m_nDeviceIndex);
                int nResultFlag = ElanTouchSwitch.ReadDevData(byteBuffer_Array, m_nDeviceIndex, m_bSocketConnectType);

                //if (nResultFlag != ElanTouch.TP_SUCCESS)
                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                    m_cDebugLog.WriteLogToBuffer(string.Format("Send Polling Command Error.TP ERRORCODE : 0x{0}", nResultFlag.ToString("X4")));
                else
                {
                    string sGetACK = "Send Polling Command Success.Get ACK :";
                    for (int nByteIndex = 0; nByteIndex < byteBuffer_Array.Length; nByteIndex++)
                        sGetACK += string.Format(" {0}", byteBuffer_Array[nByteIndex].ToString("X2"));
                    m_cDebugLog.WriteLogToBuffer(sGetACK);

                    bResult = true;
                    break;
                }

                Thread.Sleep(100);
            }

            if (SetPollingCommand() == false)
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);

            return bResult;
        }

        /// <summary>
        /// 判斷是否支援輪詢命令（Polling Command）
        /// </summary>
        /// <returns>true: 支援輪詢命令; false: 不支援輪詢命令（I2C 介面）</returns>
        private bool SetPollingCommand()
        {
            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_WINDOWS)
            {
                if (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_I2C)
                    return false;
                else
                    return true;
            }
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL)
            {
                if (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_I2C)
                    return false;
                else
                    return true;
            }
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_REMOTECLIENT)
            {
                if (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_I2C)
                    return false;
                else
                    return true;
            }
            else if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_CHROME_SSHSOCKETSERVER || ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_OTHER_SSHSOCKETSERVER)
            {
                if (ParamFingerAutoTuning.m_sInterfaceType == MainConstantParameter.m_sINTERFACE_I2C)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }


        /// <summary>
        /// 不斷傳送Dummy Command的執行緒函式,每100毫秒(計數到10)發送一次Polling Command(0x58, 0xC2, 0x00, 0x00, 0x00, 0x00),直到停用旗標設定為false(此方法已註解)
        /// </summary>
        /// <param name="objStateInfo">執行緒狀態資訊物件(未使用)</param>
        /*
        private void SetPollingDummyCommand(Object objStateInfo)
        {
            int nCount = 0;

            while (m_bEnablePollingDummyCommand)
            {
                if (nCount == 10)
                {
                    OutputMessage("-Send Polling Command : 0x58, 0xC2, 0x00, 0x00, 0x00, 0x00");
                    SendDevCommand(new byte[] { 0x58, 0xC2, 0x00, 0x00, 0x00, 0x00 });
                    nCount = 0;
                }
                
                nCount++;
                Thread.Sleep(10);
            }
        }
        */

        /// <summary>
        /// 設定使用新方法進行資料收集（Gen8 專用版本）
        /// </summary>
        /// <param name="nFrameCount">幀數參數，指定要收集的資料幀數</param>
        /// <param name="nTimeout">逾時時間（毫秒），此參數在當前實作中未直接使用</param>
        /// <returns>true: 設定成功; false: 設定失敗（3 次重試後仍失敗或掃描狀態異常）</returns>
        private bool SetUseNewMethod_Gen8(int nFrameCount, int nTimeout)
        {
            int nRetryTimes = 3;
            bool bResult = false;
            int nErrorCode = 0;

            if (SetPollingCommand() == false)
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_ExitTestMode);

            for (int nRetryIndex = 0; nRetryIndex < nRetryTimes; nRetryIndex++)
            {
                byte byteFrameCount = (byte)(nFrameCount & 0xFF);
                OutputMessage(string.Format("-Send UseNewMethod Command : 0x54, 0xCD, 0x80, 0x{0}", byteFrameCount.ToString("X2")));
                byte[] byteCommand_Array = new byte[] { 0x54, 0xCD, 0x80, byteFrameCount };

                if (CheckScanStatus_Gen8() == false)
                {
                    nErrorCode = 0;
                    bResult = false;
                    continue;
                }

                SendDevCommand(byteCommand_Array);

                /*
                if (SetPollingCommand() == true)
                    m_bEnablePollingDummyCommand = true;

                //啟動Thread進行Polling Dummy Command動作
                ThreadPool.QueueUserWorkItem(new WaitCallback(PollingDummyCommand));

                if (m_qFIFO.DequeueAll(3000, byteBuffer_Array, ElanCommand.IN_DATA_LENGTH) == false)
                {
                    //離開Thread
                    m_bEnablePollingDummyCommand = false;
                }
                else
                {
                    //離開Thread
                    m_bEnablePollingDummyCommand = false;
                    bResult = true;
                    break;
                }
                */

                if (GetChangeModeACK_Gen8(3, ref nErrorCode, 1000) != true)
                {
                    bResult = false;
                }
                else
                {
                    bResult = true;
                    break;
                }

                Thread.Sleep(100);
            }

            if (SetPollingCommand() == false)
                GetDataRelatedStep(AppCoreDefine.GetDataRelatedStep.Step_EnterTestMode);

            return bResult;
        }

        /// <summary>
        /// 檢查 Gen8 IC 的掃描狀態
        /// </summary>
        /// <returns>true: 掃描狀態就緒（nStatue = 1）; false: 狀態檢查失敗或重試次數超過 100 次</returns>
        private bool CheckScanStatus_Gen8()
        {
            bool nResult = true;
            int nRetry = 0;
            int nStatue = 0;

            while (true)
            {
                if (GetScanStatus_Gen8(ref nStatue) == false || nRetry > 100)
                {
                    nResult = false;
                    break;
                }

                if (nStatue == 1)
                    break;

                nRetry++;
            }

            return nResult;
        }

        /// <summary>
        /// 取得 Gen8 IC 的掃描狀態
        /// </summary>
        /// <param name="nValue">輸出參數，返回掃描狀態值（通常 0=未就緒, 1=就緒）</param>
        /// <param name="nDelay">命令發送後的延遲時間（毫秒），預設 10 毫秒</param>
        /// <returns>true: 成功讀取狀態值; false: 讀取失敗</returns>
        private bool GetScanStatus_Gen8(ref int nValue, int nDelay = 10)
        {
            byte[] byteBuffer_Array = new byte[63];
            byte[] byteCommand_Array = new byte[] { 0x53, 0xCE, 0x00, 0x00 };
            OutputMessage("-Get Scan Status Command : 0x53, 0xCE, 0x00, 0x00");
            SendDevCommand(byteCommand_Array);
            Thread.Sleep(nDelay);

            int nResultFlag = ElanTouchSwitch.ReadDevData(byteBuffer_Array, m_nDeviceIndex, m_bSocketConnectType);

            if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                return false;

            nValue = byteBuffer_Array[3];

            return true;
        }

        /// <summary>
        /// 取得 Gen8 IC 的模式切換確認（Change Mode ACK）
        /// </summary>
        /// <param name="nRetry">重試次數，傳遞給 GetBaseACK_Gen8 使用（通常為 3）</param>
        /// <param name="nErrorCode">輸出參數，返回錯誤代碼（1=基線確認失敗）</param>
        /// <param name="nTimeOut">逾時時間（毫秒），此參數在當前實作中未直接使用</param>
        /// <returns>true: 模式切換確認成功; false: 確認失敗（初次讀取失敗或基線確認失敗）</returns>
        private bool GetChangeModeACK_Gen8(int nRetry, ref int nErrorCode, int nTimeOut)
        {
            byte[] byteBuffer_Array = new byte[63];
            OutputMessage("-Get Change Mode ACK Command : 0x58, 0x02, 0x00, 0x00, 0x00, 0x10");
            byte[] byteCommand_Array = new byte[] { 0x58, 0x02, 0x00, 0x00, 0x00, 0x10 };

            SendDevCommand(byteCommand_Array);
            Thread.Sleep(10);

            int nResultFlag = ElanTouchSwitch.ReadDevData(byteBuffer_Array, m_nDeviceIndex, m_bSocketConnectType);

            if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                return false;

            if (GetBaseACK_Gen8(nRetry, 10, 1000) != 0)
            {
                nErrorCode = 1;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 取得 Gen8 IC 的基線確認（Base ACK）
        /// </summary>
        /// <param name="nRetry">重試次數（實際嘗試次數為 nRetry + 1）</param>
        /// <param name="nDelayTime">每次嘗試前的延遲時間（毫秒）</param>
        /// <param name="nTimeOut">逾時時間（毫秒），當前實作中未使用</param>
        /// <returns>0: 成功接收到基線確認訊號; 1: 失敗，未在重試次數內接收到確認訊號</returns>
        protected int GetBaseACK_Gen8(int nRetry, int nDelayTime, int nTimeOut)
        {
            int nResult = 1;
            int nTryCount = 0;
            byte[] byteBuffer_Array = new byte[63];

            while (nTryCount <= nRetry)
            {
                nTryCount += 1;
                byteBuffer_Array = new byte[63];
                Thread.Sleep(nDelayTime);

                int nResultFlag = ElanTouchSwitch.ReadDevData(byteBuffer_Array, m_nDeviceIndex, m_bSocketConnectType);

                if (ElanTouchSwitch.CheckTPState(nResultFlag, m_bSocketConnectType) == false)
                    continue;

                if (byteBuffer_Array[0] == 0x42 && byteBuffer_Array[1] == 0x41 && byteBuffer_Array[2] == 0x53 && byteBuffer_Array[3] == 0x45)
                {
                    nResult = 0;
                    break;
                }
            }

            return nResult;
        }

        /// <summary>
        /// 設定 Gen8 8F18 方案的觸控筆功能開關
        /// </summary>
        /// <param name="bEnableFlag">true: 啟用觸控筆功能; false: 停用觸控筆功能（預設 true）</param>
        /// <param name="bFinishFlag">是否為完成階段，true 時跳過某些延遲（預設 false）</param>
        /// <param name="bDisableExitTestMode">是否停用測試模式切換，true 時跳過模式管理（預設 false）</param>
        private void SetPenFunctionEnable_8F18(bool bEnableFlag = true, bool bFinishFlag = false, bool bDisableExitTestMode = false)
        {
#if _USE_9F07_SOCKET
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return;
#endif

            if (ParamFingerAutoTuning.m_nGen8DisablePenFunction == 0)
                return;

            if (m_eICGenerationType == ICGenerationType.Gen8)
            {
                OutputMessage(string.Format("-Set Pen Function {0}", (bEnableFlag == true) ? "Eable" : "Disable"));

                if (bDisableExitTestMode == false && m_bEnterTestMode == false)
                {
                    if (ParamFingerAutoTuning.m_nGen8DisablePenFunction == 1)
                        SetTestModeEnable(false);
                    else if (ParamFingerAutoTuning.m_nGen8DisablePenFunction == 2)
                        SetTestModeEnable(true);

                    //Thread.Sleep(m_nNormalDelayTime);

                    if (bFinishFlag == false)
                        Thread.Sleep(ParamFingerAutoTuning.m_nGen8SetPenFunctionDelayTime);
                }

                byte[] byteCommand_Array;

                if (bEnableFlag == true)
                    byteCommand_Array = new byte[] { 0x54, 0x2A, 0x01, 0x01 };
                else
                    byteCommand_Array = new byte[] { 0x54, 0x2A, 0x00, 0x01 };

                SendDevCommand(byteCommand_Array);
                Thread.Sleep(m_nNormalDelayTime);

                //if (bFinishFlag == false)
                //    Thread.Sleep(1000);

                if (bEnableFlag == true)
                    byteCommand_Array = new byte[] { 0x54, 0x2E, 0x01, 0x01 };
                else
                    byteCommand_Array = new byte[] { 0x54, 0x2E, 0x00, 0x01 };

                SendDevCommand(byteCommand_Array);
                Thread.Sleep(m_nNormalDelayTime);

                //if (bFinishFlag == false)
                //    Thread.Sleep(1000);

                if (bEnableFlag == true)
                    byteCommand_Array = new byte[] { 0x54, 0x2A, 0x0F, 0x01 };
                else
                    byteCommand_Array = new byte[] { 0x54, 0x2A, 0x0E, 0x01 };

                SendDevCommand(byteCommand_Array);
                Thread.Sleep(m_nNormalDelayTime);

                if (ParamFingerAutoTuning.m_nGen8DisablePenFunction == 2)
                    SetTestModeEnable(false);

                if (bEnableFlag == false)
                    m_bSetPenFunctionOFF_8F18 = true;
                else
                    m_bSetPenFunctionOFF_8F18 = false;
            }
        }

        /// <summary>
        /// 設定 ENCAL_SF（Self 校正使能）功能開關
        /// </summary>
        /// <param name="bEnable">true: 啟用 ENCAL_SF（設定 bit 10）; false: 停用 ENCAL_SF（清除 bit 10）</param>RetryClaude can make mistakes. Please double-check responses. Sonnet 4.5
        private void SetENCAL_SFEnable(bool bEnable)
        {
            if (ParamFingerAutoTuning.m_nSelfFSRunCALData == 1 || ParamFingerAutoTuning.m_nSelfFSRunCALData == 2)
            {
                int nSendCommandDelayTime_Gen8 = ParamFingerAutoTuning.m_nGen8SendCommandDelayTime;

                SetTestModeEnable(true);

                int nANA_TP_CTL_00_H = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_H;
                int nANA_TP_CTL_00_L = m_cOriginParameter.m_n_SELF_ANA_TP_CTL_00_L;

                int nNewANA_TP_CTL_00_H = nANA_TP_CTL_00_H;
                int nNewANA_TP_CTL_00_L = 0;

                if (bEnable == true)
                    nNewANA_TP_CTL_00_L = nANA_TP_CTL_00_L | 0x0400;
                else
                    nNewANA_TP_CTL_00_L = nANA_TP_CTL_00_L & 0xFBFF;

                if (m_cSelfParameter.m_nCAG >= 0)
                    nNewANA_TP_CTL_00_L = (nNewANA_TP_CTL_00_L & 0xFFC7) | m_cSelfParameter.m_nCAG << 3;

                int nNewANA_TP_CTL_00_H_HighByte = (nNewANA_TP_CTL_00_H & 0xFF00) >> 8;
                int nNewANA_TP_CTL_00_H_LowByte = nNewANA_TP_CTL_00_H & 0x00FF;
                int nNewANA_TP_CTL_00_L_HighByte = (nNewANA_TP_CTL_00_L & 0xFF00) >> 8;
                int nNewANA_TP_CTL_00_L_LowByte = nNewANA_TP_CTL_00_L & 0x00FF;

                byte byteTraceType = 0x21;

                if (m_eSelfTraceType == TraceType.RX)
                    byteTraceType = 0x21;
                else if (m_eSelfTraceType == TraceType.TX)
                    byteTraceType = 0x22;

                byte[] byteCommand_Array = new byte[] 
                { 
                    0x68, 
                    byteTraceType, 
                    0x00, 
                    0x00, 
                    0x00, 
                    0x70,
                    (byte)nNewANA_TP_CTL_00_H_HighByte, 
                    (byte)nNewANA_TP_CTL_00_H_LowByte,
                    (byte)nNewANA_TP_CTL_00_L_HighByte, 
                    (byte)nNewANA_TP_CTL_00_L_LowByte 
                };

                SendDevCommand(byteCommand_Array);
                Thread.Sleep(nSendCommandDelayTime_Gen8);

                SetTestModeEnable(false);
            }
        }

        /// <summary>
        /// 發送裝置命令
        /// </summary>
        /// <param name="byteCommand_Array">要發送的命令位元組陣列</param>
        private void SendDevCommand(byte[] byteCommand_Array)
        {
            //ElanTouch.SendDevCommand(byteCommand_Array, byteCommand_Array.Length, 1000, m_nDeviceIndex);
            ElanTouchSwitch.SendDevCommand(byteCommand_Array, m_nDeviceIndex, m_bSocketConnectType);

            string sSendCommand = "-Send Command :";

            for (int nIndex = 0; nIndex < byteCommand_Array.Length; nIndex++)
                sSendCommand += string.Format(" {0}", byteCommand_Array[nIndex].ToString("X2"));

            m_cDebugLog.WriteLogToBuffer(sSendCommand);
        }

        /// <summary>
        /// 發送取得 Self 報告資料命令
        /// </summary>
        /// <param name="bGetFlag">true: 啟動 Self 報告資料發送; false: 停止 Self 報告資料發送</param>
        /// <param name="byteType">報告資料類型（預設 0x01），影響資料格式和內容</param>
        private void SendGetSelfReportData(bool bGetFlag, byte byteType = 0x01)
        {
            if (bGetFlag == true)
            {
                byte[] byteCommand_Array = new byte[] { 0x54, 0x8F, 0x00, byteType };
                OutputMessage("-Send Get Self Report Command");
                SendDevCommand(byteCommand_Array);
            }
            else
            {
                byte[] byteCommand_Array = new byte[] { 0x54, 0x8F, 0x00, 0x00 };
                OutputMessage("-Send Stop Get Self Report Command");
                SendDevCommand(byteCommand_Array);
            }
        }
    }
}
