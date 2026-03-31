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
        /// <summary>
        /// 啟動 Android 裝置保持喚醒功能
        /// </summary>
        /// <remarks>
        /// 執行條件檢查：
        /// <list type="bullet">
        /// <item>排除 Android ARM Tool 連線類型（不需要保持喚醒）</item>
        /// <item>僅在非載入資料模式 (m_bLoadData = false) 下執行</item>
        /// <item>僅適用於 Android Remote Client 連線類型</item>
        /// <item>需設定保持喚醒間隔時間 (m_nKeepWakeUpIntervalTime > 0)</item>
        /// </list>
        /// <para>符合條件時，會建立並啟動獨立執行緒持續執行 RunKeepWakeUp 方法</para>
        /// <para>此功能用於防止 Android 裝置在長時間測試過程中進入休眠狀態，確保連線穩定</para>
        /// </remarks>
        private void KeepAndroidWakeUp()
        {
#if _USE_9F07_SOCKET
            /*
            if (m_eICGenerationType == ICGenerationType.Gen9)
                return;
            */
#endif

            if (ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_ARMTOOL)
                return;

            if (m_cfrmParent.m_bLoadData == false &&
                ParamFingerAutoTuning.m_sSocketType == MainConstantParameter.m_sSOCKET_ANDROID_REMOTECLIENT &&
                ParamFingerAutoTuning.m_nKeepWakeUpIntervalTime > 0)
            {
                m_tsKeepWakeUp = new ThreadStart(RunKeepWakeUp);
                m_tKeepWakeUp = new Thread(m_tsKeepWakeUp);
                //m_tKeepWakeUp.IsBackground = true;
                m_tKeepWakeUp.Start();
            }
        }

        /// <summary>
        /// 執行 Android 裝置保持喚醒的背景執行緒
        /// </summary>
        /// <remarks>
        /// 執行流程：
        /// <list type="number">
        /// <item>檢查 KeepWakeUp.bat 批次檔是否存在，不存在則退出</item>
        /// <item>清除舊的 Debug Log 檔案</item>
        /// <item>等待 Socket 連線建立完成 (m_bSocketConnected = true)</item>
        /// <item>持續執行保持喚醒命令直到連線中斷
        ///   <list type="bullet">
        ///   <item><strong>非 Gen9</strong>: 透過 m_cRunServerMgr.KeepWakeUp() 執行</item>
        ///   <item><strong>Gen9</strong>: 透過 ExecKeepWakeUp() 執行批次檔</item>
        ///   </list>
        /// </item>
        /// <item>執行成功後延遲指定間隔時間 (m_nKeepWakeUpIntervalTime)，失敗則延遲 1 秒後重試</item>
        /// </list>
        /// <para>此方法在獨立執行緒中運行，定期發送喚醒命令防止 Android 裝置休眠</para>
        /// </remarks>
        private void RunKeepWakeUp()
        {
            if (File.Exists(string.Format(@"{0}\RemoteClient\KeepWakeUp.bat", Application.StartupPath)) == false)
                return;

            string sDebugLogFilePath = string.Format(@"{0}\{1}\DebugLog\DebugLog{2}.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName, ElanBatchProcess.m_sFILETYPE_KEEPWAKEUPPROC);

            while (File.Exists(sDebugLogFilePath) == true)
            {
                File.Delete(sDebugLogFilePath);
                Thread.Sleep(10);
            }

            while (m_bSocketConnected == false)
            {
                if (m_bSocketConnected == true && m_bTPConnected == true)
                    break;

                Thread.Sleep(10);
            }

            //while (m_bSocketConnected == true && m_bTPConnected == true)
            while (m_bSocketConnected == true)
            {
                if (m_eICGenerationType != ICGenerationType.Gen9)
                {
                    if (m_cRunServerMgr.KeepWakeUp() == false)
                        Thread.Sleep(1000);
                    else
                        Thread.Sleep(ParamFingerAutoTuning.m_nKeepWakeUpIntervalTime);
                }
#if _USE_9F07_SOCKET
                else
                {
                    if (ExecKeepWakeUp() == false)
                        Thread.Sleep(1000);
                    else
                        Thread.Sleep(ParamFingerAutoTuning.m_nKeepWakeUpIntervalTime);
                }
#endif
            }
        }

#if _USE_9F07_SOCKET
        /// <summary>
        /// 執行 Gen9 (9F07) 的保持喚醒批次檔
        /// </summary>
        /// <remarks>
        /// 執行流程：
        /// <list type="number">
        /// <item>啟動 KeepWakeUp.bat 批次檔處理程序</item>
        /// <item>持續讀取批次檔輸出文字 (5 秒逾時)
        ///   <list type="bullet">
        ///   <item>若讀取失敗或返回 null，停止處理程序並返回 false</item>
        ///   <item>若輸出為空字串，繼續等待</item>
        ///   <item>若輸出包含 "KeepWakeUp finish"，表示完成並跳出迴圈</item>
        ///   </list>
        /// </item>
        /// <item>停止批次檔處理程序並返回 true 表示成功</item>
        /// </list>
        /// <para>此方法專門用於 Gen9 (9F07) 世代，透過執行批次檔與 Android 裝置通訊以維持喚醒狀態</para>
        /// <para>批次檔路徑：RemoteClient\KeepWakeUp.bat</para>
        /// </remarks>
        /// <returns>true: 執行成功; false: 執行失敗或處理程序異常</returns>
        private bool ExecKeepWakeUp()
        {
            ElanBatchProcess_9F07 cKeepWakeUpProcess = new ElanBatchProcess_9F07(@"RemoteClient\KeepWakeUp.bat", "", "", true);
            cKeepWakeUpProcess.Start(true, ElanBatchProcess_9F07.m_sFILETYPE_KEEPWAKEUPPROC);

            while (true)
            {
                string sOutputTxt = "";

                if (cKeepWakeUpProcess.ReadOutputText(ref sOutputTxt, 5000) == false)
                    continue;

                if (sOutputTxt == null)
                {
                    cKeepWakeUpProcess.Stop();
                    return false;
                }

                if (sOutputTxt == "")
                    continue;

                /*
                if (m_OutputTextEvent != null)
                    m_OutputTextEvent(sOutputTxt);
                */

                if (sOutputTxt.IndexOf("KeepWakeUp finish", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    /*
                    if (m_OutputTextEvent != null)
                        m_OutputTextEvent("KeepWakeUp finished.");
                    */

                    break;
                }
            }

            cKeepWakeUpProcess.Stop();
            return true;
        }
#endif
    }
}
