# AutoTuningTool

AutoTuningTool 是一款用於觸控螢幕 (Touch Screen) 自動調校的 Windows 桌面應用程式，支援手指觸控 (Finger) 與 MPP 觸控筆 (MPP Pen) 的參數調校與測試分析。

## 專案概述

本專案為 ELAN 觸控螢幕自動調校工具，提供完整的頻率掃描、數據分析、參數調整等功能，協助工程師進行觸控面板的調校工作。主程式 (`AutoTuning_NewUI`) 作為統一入口，依使用者選擇切換至手指或 MPP 筆調校模組；兩個調校模組皆以 Class Library 形式供主程式嵌入載入。

## 專案結構

```
AutoTuningTool/
├── AutoTuning_NewUI.sln               # 主解決方案（整合三個子專案）
├── AGENTS.md                          # Agent 操作規範
├── restore-packages.ps1               # Windows 一鍵還原 NuGet 套件腳本
├── AutoTuningTool Changelog.docx      # 專案變更日誌
│
├── .specify/                          # Spec-Kit SDD 核心目錄
│   ├── memory/constitution.md         # 專案治理原則（最高指引）
│   ├── templates/                     # Spec / Plan / Tasks 模板
│   ├── scripts/powershell/            # SDD 自動化腳本
│   └── specs/                         # Feature 規格（按功能分支）
│
├── .github/prompts/                   # Copilot Slash Commands (SDD)
│
├── AutoTuning_NewUI/                  # 主程式入口 (Windows Forms WinExe)
│   ├── frmMain.cs                     # 主視窗（含 Finger / MPP Pen 切換）
│   ├── frmMain.Designer.cs            # 主視窗 UI 佈局
│   ├── Program.cs                     # 程式進入點（含單一實例檢查）
│   ├── App.config                     # 組件繫結重定向設定
│   ├── packages.config                # NuGet 套件清單
│   ├── Properties/                    # 組件資訊與資源
│   └── Resources/                     # 內嵌資源
│
├── FingerAutoTuning/                  # 手指觸控調校子專案
│   ├── FingerAutoTuning.sln           # 子專案獨立解決方案
│   └── FingerAutoTuning/
│       ├── AppCore.cs                 # 核心流程調度
│       ├── AppCore/                   # AppCore 功能細分模組
│       │   ├── ConnectAndroidDevice.cs    # ADB 連線
│       │   ├── ConnectChromeRemoteClient.cs # Chrome Remote 調試
│       │   ├── ConnectSSHSocket.cs        # SSH Socket 連線
│       │   ├── ConnectWindowsDevice.cs    # Windows 本機 HID
│       │   ├── ConvertHDF5Data.cs         # HDF5 資料轉換
│       │   ├── GetFrameData.cs            # 幀資料擷取
│       │   ├── GetFrequencyData.cs        # 頻率資料
│       │   ├── RecordData.cs              # 資料錄製
│       │   ├── CheckFWAnalogParameter.cs  # FW 類比參數檢查
│       │   ├── ExecuteCommand.cs          # 命令執行
│       │   ├── GetAndSetFWOption.cs       # FW 選項讀寫
│       │   ├── GetFWParameter.cs          # 讀取 FW 參數
│       │   ├── SetFWParameter.cs          # 設定 FW 參數
│       │   ├── SetAndGetFWParameter.cs    # FW 參數雙向操作
│       │   ├── GetHIDReport.cs            # HID Report 擷取
│       │   ├── GetReportData.cs           # Report 資料擷取
│       │   ├── OutputResult.cs            # 結果輸出
│       │   ├── ScreenSetting.cs           # 螢幕設定
│       │   ├── KeepAndroidWakeUp.cs       # Android 保持喚醒
│       │   ├── LoadLogsAndEnsureRecordDir.cs # 日誌載入與目錄管理
│       │   └── MoveLogData.cs             # 日誌資料搬移
│       ├── AnalysisFlow_ACFR.cs       # AC 頻率排序分析流程
│       ├── AnalysisFlow_FRPH1.cs      # 頻率排序 Phase 1 分析流程
│       ├── AnalysisFlow_FRPH2.cs      # 頻率排序 Phase 2 分析流程
│       ├── AnalysisFlow_Raw.cs        # Raw ADC 掃描分析流程
│       ├── AnalysisFlow_RawADCS.cs    # Raw ADCS 掃描分析流程
│       ├── AnalysisFlow_SelfFS.cs     # Self 頻率掃描分析流程
│       ├── AnalysisFlow_SelfPNS.cs    # Self NCP/NCN 掃描分析流程
│       ├── DataAnalysis.cs            # 數據分析邏輯（策略模式分派）
│       ├── DataSave.cs                # 數據儲存（含檔名規則）
│       ├── Class/                     # 底層工具類別
│       │   ├── ElanCommand.cs             # ELAN FW 命令集（通用）
│       │   ├── ElanCommand_Gen6or7.cs     # Gen6/7 命令集
│       │   ├── ElanCommand_Gen8.cs        # Gen8 命令集
│       │   ├── ElanTouch.cs               # 裝置通訊層（HID）
│       │   ├── ElanTouch_Socket.cs        # 裝置通訊層（Socket）
│       │   ├── ElanSSHClient.cs           # SSH 連線
│       │   ├── ElanDirectTouch.cs         # DirectTouch 通訊
│       │   ├── ADBFileIO.cs               # ADB 檔案存取
│       │   ├── FrameMgr.cs                # 封包管理
│       │   ├── BlockingQueue.cs           # 非同步佇列
│       │   ├── MathMethod.cs              # 數學輔助
│       │   ├── ElanConvert..cs            # 格式轉換
│       │   ├── ElanBatchProcess.cs        # 批次處理
│       │   ├── ElanBatchProcess_9F07.cs   # 9F07 批次處理
│       │   ├── InputDevice.cs             # 裝置抽象層
│       │   ├── DebugLogAPI.cs             # 偵錯日誌
│       │   └── ...
│       ├── frmMain.cs                 # 手指調校主視窗
│       ├── frmFRPH1Chart.cs           # Phase 1 分析圖表
│       ├── frmFRPH2Chart.cs           # Phase 2 分析圖表
│       ├── frmACFRChart.cs            # AC 頻率排序圖表
│       ├── frmSelfFSChart.cs          # Self 頻率掃描圖表
│       ├── frmParamSetting.cs         # 參數設定
│       ├── frmStepSetting.cs          # 流程步驟設定
│       ├── frmMultiAnalysis.cs        # 多檔案批次分析
│       ├── frmPattern.cs             # 模式選擇
│       ├── frmRedmineTask.cs          # Redmine 追蹤整合
│       ├── frmFeedback.cs            # 回饋設定
│       ├── ctrlMAChart.cs             # 繪圖控制項
│       ├── ctrlMADataGridView.cs      # 資料表格控制項
│       ├── ctrlParamPage.cs           # 參數頁籤
│       ├── ctrlTestItemCheckBox.cs    # 測試項目核選方塊
│       ├── Helper/DGVHelper.cs        # DataGridView 輔助
│       └── ...
│
├── MPPPenAutoTuning/                  # MPP 觸控筆調校子專案
│   ├── MPPPenAutoTuning.sln           # 子專案獨立解決方案
│   ├── AutoTuningAP C# ChangeLog.doc  # MPP 模組變更日誌
│   └── MPPPenAutoTuning/
│       ├── AnalysisFlow/              # 各分析流程
│       │   ├── AnalysisFlow_DigiGainTuning.cs   # 數位增益調校
│       │   ├── AnalysisFlow_DTNormal.cs         # DT 一般模式
│       │   ├── AnalysisFlow_DTTRxS.cs           # DT TRxS 模式
│       │   ├── AnalysisFlow_LinearityTable.cs   # 線性度表格
│       │   ├── AnalysisFlow_Noise.cs            # 雜訊分析
│       │   ├── AnalysisFlow_Noise_Gen8.cs       # 雜訊分析（Gen8）
│       │   ├── AnalysisFlow_Noise_TestMode.cs   # 雜訊分析（測試模式）
│       │   ├── AnalysisFlow_PeakCheck.cs        # 峰值檢查
│       │   ├── AnalysisFlow_PressureProtect.cs  # 壓力保護
│       │   ├── AnalysisFlow_PressureSetting.cs  # 壓力設定
│       │   ├── AnalysisFlow_PressureTable.cs    # 壓力表格
│       │   ├── AnalysisFlow_Raw.cs              # Raw 資料分析
│       │   ├── AnalysisFlow_TiltNoise.cs        # 傾角雜訊
│       │   ├── AnalysisFlow_TiltNoise_Gen8.cs   # 傾角雜訊（Gen8）
│       │   ├── AnalysisFlow_TiltTuning.cs       # 傾角調校
│       │   ├── AnalysisFlow_TPGainTuning.cs     # TP 增益調校
│       │   └── AnalysisFlow_Else.cs             # 其他分析
│       ├── Class/                     # 底層工具類別
│       │   ├── API/RS232.cs               # RS-232 串列介面
│       │   ├── Algorithm/                 # 演算法
│       │   │   └── PressureAlgorithm.cs   # 壓力演算法
│       │   ├── HW/                        # 硬體儀器驅動
│       │   │   ├── HW_ForceGauge_SHIMPO_FGP05.cs  # SHIMPO 力量計
│       │   │   └── HW_LT_DT_500F.cs              # LT 線性位移計
│       │   ├── GetFrameData/              # 資料幀管理
│       │   │   ├── GetFrameData.cs
│       │   │   ├── FrameMgr.cs
│       │   │   └── ElanDefine.cs
│       │   ├── ElanCommand.cs             # ELAN FW 命令集
│       │   ├── ElanCommand_Gen8.cs        # Gen8 命令集
│       │   ├── ElanTouch.cs               # 裝置通訊層
│       │   ├── ElanHID.cs                 # HID 裝置通訊
│       │   ├── GoDrawAPI.cs               # GoDraw 機械手臂 API
│       │   ├── RobotAPI.cs                # 機器人控制 API
│       │   ├── ServerSocketAPI.cs         # Socket 伺服器 API
│       │   ├── SocketAPI.cs               # Socket 通訊
│       │   ├── DataAnalysis.cs            # 資料分析
│       │   └── ...
│       ├── Interface/                 # 介面定義
│       │   ├── IHardware.cs               # 硬體介面
│       │   └── IRS232Device.cs            # RS-232 裝置介面
│       ├── ConnectFlow.cs             # 連線流程管理
│       ├── ProcessFlow_MainFlow.cs    # 調校主流程
│       ├── ProcessFlow_SingleMode.cs  # 單機模式流程
│       ├── ProcessFlow_ClientMode.cs  # Client 模式流程
│       ├── ProcessFlow_ServerMode.cs  # Server 模式流程
│       ├── ProcessFlow_GoDrawMode.cs  # GoDraw 機械手臂模式
│       ├── ProcessFlow_LoadDataMode.cs # 載入歷史資料模式
│       ├── ProcessFlow_CommonFunction.cs # 通用函數
│       ├── frmMain.cs                 # MPP 筆調校主視窗
│       ├── frmResultChart.cs          # 結果圖表
│       ├── frmParameterSetting.cs     # 參數設定
│       ├── frmFrequencySetting.cs     # 頻率設定
│       ├── frmStepSetting.cs          # 流程步驟設定
│       ├── frmGoDrawController.cs     # 機械手臂控制面板
│       ├── frmRedmineTask.cs          # Redmine 整合
│       ├── frmSummerizeLogData.cs     # 日誌彙總
│       ├── frmFlowSetting.cs          # 流程設定
│       └── ...
│
├── packages/                          # NuGet 套件（共用）
│   ├── EPPlus.6.2.6/
│   ├── EPPlus.Interfaces.6.1.1/
│   ├── EPPlus.System.Drawing.6.1.1/
│   ├── FontAwesome.Sharp.5.15.4/
│   ├── Microsoft.IO.RecyclableMemoryStream.1.4.1/
│   ├── RJCodeAdvance.RJControls.1.0.0/
│   ├── System.Buffers.4.5.1/
│   ├── System.Memory.4.5.4/
│   ├── System.Numerics.Vectors.4.5.0/
│   ├── System.Resources.Extensions.4.7.1/
│   └── System.Runtime.CompilerServices.Unsafe.4.5.3/
├── runtime/                           # NuGet Runtime 套件檔案
└── icon/                              # 介面圖示資源 (PNG)
```

## 主要功能

### 主程式 (AutoTuning_NewUI)
- 統一視窗框架，內嵌切換 Finger / MPP Pen 兩模組
- 自訂無邊框視窗（最大化、最小化、縮放邊框），採用 FontAwesome 圖示與 RJControls 控制項
- 單一實例執行保護（防止多個程式副本同時運行）
- 全域異常處理機制（`AppDomain.CurrentDomain.UnhandledException`）
- 支援 `Release_9F07_Socket` 組態（DirectTouch 模式，隱藏 MPP Pen 功能）
- 顯示版本號與組建時間（讀取 PE Header Linker Timestamp）

### 手指觸控調校 (FingerAutoTuning)

| 分析流程 | 說明 |
|---------|------|
| Frequency Rank Phase 1 (FRPH1) | 頻率排序第一階段 |
| Frequency Rank Phase 2 (FRPH2) | 頻率排序第二階段 |
| AC Frequency Rank (ACFR) | AC 頻率排序，支援 Sobel/Roberts 等多種濾波遮罩 |
| Raw ADC Sweep | Raw ADC 原始資料掃描 |
| Raw ADCS Sweep | Raw ADCS 掃描 |
| Self Frequency Sweep (SelfFS) | Self 電容頻率掃描 |
| Self NCP/NCN Sweep (SelfPNS) | Self NCP/NCN 參數掃描 |

**連線支援：**
- Android 裝置（ADB / Socket）
- Windows 裝置（HID USB）
- SSH Socket 連線
- Chrome Remote Client
- HDF5 資料格式轉換

**其他功能：**
- 批次處理（ElanBatchProcess / ElanBatchProcess_9F07）
- 支援 Gen6/7/8 ELAN FW 命令集
- Redmine 任務整合
- Multi-frame 數據分析與 SNR 計算
- 多檔案批次分析（frmMultiAnalysis）
- 參數管理（ParamMgr / ParamBase / ParamProperties）
- 測試模式選擇（TestPattern / FreeDrawPattern）

### MPP 觸控筆調校 (MPPPenAutoTuning)

| 分析流程 | 說明 |
|---------|------|
| Noise | 雜訊分析（標準 / Gen8 / 測試模式） |
| Tilt Noise | 傾斜雜訊分析（標準 / Gen8） |
| DigiGain Tuning | 數位增益調校 |
| TP Gain Tuning | TP 增益調校 |
| Pressure Table | 壓力曲線表格調校 |
| Pressure Setting | 壓力參數設定 |
| Pressure Protect | 壓力保護設定 |
| Linearity Table | 線性度表格校正 |
| Peak Check | 峰值檢查 |
| DT Normal / DTTRxS | DirectTouch 一般 / TRxS 模式分析 |
| Tilt Tuning | 傾斜角調校 |
| Raw Data | 原始資料擷取 |

**流程模式：**
- 單機模式（SingleMode）— 本機獨立調校
- Client/Server 模式 — 遠端協同調校
- GoDraw 模式 — 搭配 GoDraw 機械手臂自動化
- 載入資料模式（LoadDataMode）— 匯入歷史資料重新分析

**連線與硬體支援：**
- GoDraw 機械手臂控制（GoDrawAPI）
- 機器人控制介面（RobotAPI）
- 力量計整合（SHIMPO FGP05 / LT DT-500F）
- RS-232 串列介面
- Socket Server/Client 模式
- HID 裝置連線
- 硬體介面抽象（IHardware / IRS232Device）

**報表輸出：**
- Excel 報表輸出（EPPlus 6.2.6）
- 日誌彙總（frmSummerizeLogData）

## 系統需求

| 項目 | 規格 |
|------|------|
| 作業系統 | Windows 10 / 11 |
| 開發環境 | Visual Studio 2019 或更新版本 |
| .NET Framework | 4.8 |
| 平台目標 | x86 |

## 相依套件 (NuGet)

| 套件名稱 | 版本 | 說明 | 使用模組 |
|---------|------|------|---------|
| FontAwesome.Sharp | 5.15.4 | 圖示字型 | AutoTuning_NewUI / FingerAutoTuning / MPPPenAutoTuning |
| RJCodeAdvance.RJControls | 1.0.0 | 自訂 UI 控制項 | AutoTuning_NewUI / FingerAutoTuning / MPPPenAutoTuning |
| EPPlus | 6.2.6 | Excel 檔案處理 | MPPPenAutoTuning |
| EPPlus.Interfaces | 6.1.1 | EPPlus 介面定義 | MPPPenAutoTuning |
| EPPlus.System.Drawing | 6.1.1 | EPPlus 繪圖支援 | MPPPenAutoTuning |
| Microsoft.IO.RecyclableMemoryStream | 1.4.1 | 可回收記憶體串流 | MPPPenAutoTuning |
| System.Buffers | 4.5.1 | 緩衝區管理 | AutoTuning_NewUI |
| System.Memory | 4.5.4 | 記憶體管理擴充 | AutoTuning_NewUI |
| System.Numerics.Vectors | 4.5.0 | 向量運算 | AutoTuning_NewUI |
| System.Resources.Extensions | 4.7.1 | 資源擴充 | AutoTuning_NewUI |
| System.Runtime.CompilerServices.Unsafe | 4.5.3 | Unsafe 記憶體存取 | AutoTuning_NewUI |

## 建置組態

| 組態 | 平台 | 說明 |
|------|------|------|
| `Debug\|x86` | x86 | 偵錯版本，含完整除錯符號 |
| `Release\|x86` | x86 | 正式發布版本 |
| `Release_9F07_Socket\|x86` | x86 | DirectTouch 模式（停用 MPP Pen 按鈕，啟用 `_USE_9F07_SOCKET` 定義） |

### 建置步驟

1. 使用 Visual Studio 開啟 `AutoTuning_NewUI.sln`
2. 還原 NuGet 套件（可執行 `./restore-packages.ps1`，或於 Visual Studio 右鍵方案 → 還原 NuGet 套件）
3. 選擇目標建置組態
4. 建置解決方案（`Ctrl+Shift+B`）

> **提示**：各子專案亦提供獨立的 `.sln` 檔案（`FingerAutoTuning/FingerAutoTuning.sln`、`MPPPenAutoTuning/MPPPenAutoTuning.sln`），可單獨開啟進行開發與偵錯。

### 快速還原 NuGet 套件

若為新環境 `git clone` 後尚未存在 `packages/`，可在專案根目錄直接執行：

```powershell
./restore-packages.ps1
```

此腳本會：

1. 優先使用系統已安裝的 `nuget.exe`
2. 若本機沒有 `nuget.exe`，自動下載到使用者本機快取目錄
3. 依序還原主 solution 與子專案 solution 所需的 `packages.config` 套件

## 執行方式

建置完成後，執行檔位於：

```
AutoTuning_NewUI\bin\Release\AutoTuning.exe
AutoTuning_NewUI\bin\Debug\AutoTuning.exe
AutoTuning_NewUI\bin\Release_9F07_Socket\AutoTuning.exe
```

## 版本資訊

- **目前版本**: 2.0.0.0-beta1
- 版本資訊從組件 `AssemblyInformationalVersion` 屬性讀取
- 程式標題格式：`AutoTuningTool V<版本號> (<組建時間 yyyyMMdd-HHmmss>)`

## Git 儲存庫

```bash
# 複製現有儲存庫
git clone http://git.emc.com.tw/strd2-ae/autotuningtool.git

# 或將本地儲存庫推送至遠端
cd existing_repo
git remote add origin http://git.emc.com.tw/strd2-ae/autotuningtool.git
git branch -M master
git push -uf origin master
```

## 授權條款

Copyright © 2022 ELAN Microelectronics Corp. 保留所有權利。
