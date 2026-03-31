# AutoTuningTool

AutoTuningTool 是一款用於觸控螢幕 (Touch Screen) 自動調校的 Windows 桌面應用程式，支援手指觸控 (Finger) 與 MPP 觸控筆 (MPP Pen) 的參數調校與測試分析。

## 工程文件入口

若要以 Spec Coding 方式快速理解與修改本專案，請先閱讀：

- [docs/README.md](docs/README.md)

建議閱讀順序：根 README → docs/README → docs/system → docs/specs → docs/modules → docs/manuals。

## 專案概述

本專案為 ELAN 觸控螢幕自動調校工具，提供完整的頻率掃描、數據分析、參數調整等功能，協助工程師進行觸控面板的調校工作。主程式 (`AutoTuning_NewUI`) 作為統一入口，依使用者選擇切換至手指或 MPP 筆調校模組；兩個調校模組皆以 Class Library 形式供主程式嵌入載入。

## 專案結構

```
AutoTuningTool/
├── AutoTuning_NewUI.sln               # 主解決方案檔案
├── restore-packages.ps1               # Windows 一鍵還原 NuGet 套件腳本
├── run-datasave-smoke-test.ps1        # Finger DataSave 檔名 smoke test
├── run-mpp-excel-smoke-test.ps1       # MPP Excel 輸出檔名 smoke test
├── AutoTuning_NewUI/                  # 主程式入口 (Windows Forms WinExe)
│   ├── frmMain.cs                     # 主視窗（含 Finger / MPP Pen 切換）
│   ├── Program.cs                     # 程式進入點
│   ├── packages.config                # NuGet 套件清單
│   └── Properties/                    # 組件資訊與資源
├── FingerAutoTuning/                  # 手指觸控調校子專案
│   └── FingerAutoTuning/
│       ├── AnalysisFlow_ACFR.cs       # AC 頻率排序分析流程
│       ├── AnalysisFlow_FRPH1.cs      # 頻率排序 Phase 1 分析流程
│       ├── AnalysisFlow_FRPH2.cs      # 頻率排序 Phase 2 分析流程
│       ├── AnalysisFlow_Raw.cs        # Raw ADC 掃描分析流程
│       ├── AnalysisFlow_RawADCS.cs    # Raw ADCS 掃描分析流程
│       ├── AnalysisFlow_SelfFS.cs     # Self 頻率掃描分析流程
│       ├── AnalysisFlow_SelfPNS.cs    # Self NCP/NCN 掃描分析流程
│       ├── AppCore.cs                 # 核心流程調度
│       ├── AppCore/                   # AppCore 功能細分模組
│       │   ├── ConnectAndroidDevice.cs
│       │   ├── ConnectChromeRemoteClient.cs
│       │   ├── ConnectSSHSocket.cs
│       │   ├── ConnectWindowsDevice.cs
│       │   ├── ConvertHDF5Data.cs
│       │   ├── GetFrameData.cs
│       │   ├── GetFrequencyData.cs
│       │   ├── RecordData.cs
│       │   └── ...
│       ├── Class/                     # 底層工具類別
│       │   ├── ElanCommand*.cs        # ELAN FW 命令集 (Gen6/7/8)
│       │   ├── ElanTouch*.cs          # 裝置通訊層 (HID/Socket)
│       │   ├── ElanSSHClient.cs       # SSH 連線
│       │   ├── ADBFileIO.cs           # ADB 檔案存取
│       │   ├── FrameMgr.cs            # 封包管理
│       │   ├── MathMethod.cs          # 數學輔助
│       │   └── ...
│       ├── DataAnalysis.cs            # 數據分析邏輯
│       ├── DataSave.cs                # 數據儲存
│       ├── frmMain.cs                 # 手指調校主視窗
│       ├── Helper/DGVHelper.cs        # DataGridView 輔助
│       └── ...
├── MPPPenAutoTuning/                  # MPP 觸控筆調校子專案
│   └── MPPPenAutoTuning/
│       ├── AnalysisFlow/              # 各分析流程
│       │   ├── AnalysisFlow_DigiGainTuning.cs
│       │   ├── AnalysisFlow_DTNormal.cs
│       │   ├── AnalysisFlow_DTTRxS.cs
│       │   ├── AnalysisFlow_LinearityTable.cs
│       │   ├── AnalysisFlow_Noise.cs
│       │   ├── AnalysisFlow_Noise_Gen8.cs
│       │   ├── AnalysisFlow_Noise_TestMode.cs
│       │   ├── AnalysisFlow_PeakCheck.cs
│       │   ├── AnalysisFlow_PressureProtect.cs
│       │   ├── AnalysisFlow_PressureSetting.cs
│       │   ├── AnalysisFlow_PressureTable.cs
│       │   ├── AnalysisFlow_Raw.cs
│       │   ├── AnalysisFlow_TiltNoise.cs
│       │   ├── AnalysisFlow_TiltNoise_Gen8.cs
│       │   ├── AnalysisFlow_TiltTuning.cs
│       │   └── AnalysisFlow_TPGainTuning.cs
│       ├── Class/                     # 底層工具類別
│       │   ├── API/RS232.cs           # RS-232 串列介面
│       │   ├── Algorithm/             # 演算法
│       │   ├── HW/                    # 硬體儀器驅動
│       │   │   ├── HW_ForceGauge_SHIMPO_FGP05.cs
│       │   │   └── HW_LT_DT_500F.cs
│       │   ├── ElanCommand*.cs        # ELAN FW 命令集
│       │   ├── ElanTouch*.cs          # 裝置通訊層
│       │   ├── GetFrameData/          # 資料幀管理
│       │   ├── GoDrawAPI.cs           # GoDraw 機械手臂 API
│       │   ├── RobotAPI.cs            # 機器人控制 API
│       │   ├── ServerSocketAPI.cs     # Socket 伺服器 API
│       │   └── ...
│       ├── ConnectFlow.cs             # 連線流程管理
│       ├── ProcessFlow_*.cs           # 調校主流程 (單機/Client/Server/GoDraw/載入資料)
│       ├── frmMain.cs                 # MPP 筆調校主視窗
│       └── ...
├── packages/                          # NuGet 套件
│   ├── EPPlus.6.2.6/
│   ├── FontAwesome.Sharp.5.15.4/
│   ├── Microsoft.IO.RecyclableMemoryStream.1.4.1/
│   └── RJCodeAdvance.RJControls.1.0.0/
└── icon/                              # 介面圖示資源 (PNG)
```

## 主要功能

### 主程式 (AutoTuning_NewUI)
- 統一視窗框架，內嵌切換 Finger / MPP Pen 兩模組
- 自訂無邊框視窗（最大化、最小化、縮放邊框）
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

**連線與硬體支援：**
- 多模式流程：單機 / Client / Server / GoDraw / 載入歷史資料
- GoDraw 機械手臂控制（GoDrawAPI）
- 力量計整合（SHIMPO FGP05 / LT DT-500F）
- RS-232 串列介面
- Socket Server/Client 模式
- HID 裝置連線

**報表輸出：**
- Excel 報表輸出（EPPlus 6.2.6）

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
| FontAwesome.Sharp | 5.15.4 | 圖示字型 | 全部 |
| RJCodeAdvance.RJControls | 1.0.0 | 自訂 UI 控制項 | AutoTuning_NewUI |
| EPPlus | 6.2.6 | Excel 檔案處理 | MPPPenAutoTuning |
| EPPlus.Interfaces | 6.1.1 | EPPlus 介面定義 | MPPPenAutoTuning |
| EPPlus.System.Drawing | 6.1.1 | EPPlus 繪圖支援 | MPPPenAutoTuning |
| Microsoft.IO.RecyclableMemoryStream | 1.4.1 | 可回收記憶體串流 | MPPPenAutoTuning |
| MathNet.Numerics | 4.5.1 | 數學運算函式庫 | FingerAutoTuning / MPPPenAutoTuning |
| Newtonsoft.Json | - | JSON 序列化 | FingerAutoTuning / MPPPenAutoTuning |
| Renci.SshNet | - | SSH 連接 | FingerAutoTuning |
| Redmine.Net.Api | - | Redmine API 整合 | FingerAutoTuning / MPPPenAutoTuning |

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

### 快速還原 NuGet 套件

若為新環境 `git clone` 後尚未存在 `packages/`，可在專案根目錄直接執行：

```powershell
./restore-packages.ps1
```

此腳本會：

1. 優先使用系統已安裝的 `nuget.exe`
2. 若本機沒有 `nuget.exe`，自動下載到使用者本機快取目錄
3. 依序還原主 solution 與子專案 solution 所需的 `packages.config` 套件

### 重跑 DataSave smoke test

若要驗證 FingerAutoTuning 的 DataSave 在一般模式、Self 模式與 Raw ADC Sweep 模式下仍輸出既有 txt / csv 檔名，可在專案根目錄執行：

```powershell
./run-datasave-smoke-test.ps1
```

此腳本會：

1. 預設先建置 `AutoTuning_NewUI.sln` 的 `Release|x86`。
2. 自動切換到 32-bit PowerShell，以載入 x86 的 `FingerAutoTuning.dll`。
3. 實際呼叫 `DataSave` 產生暫存輸出並比對代表案例檔名。
4. 預設清理測試暫存輸出；若要保留產物，可加上 `-KeepArtifacts`。

若只想重跑既有建置產物上的檢查，可使用：

```powershell
./run-datasave-smoke-test.ps1 -SkipBuild
```

### 重跑 MPP Excel smoke test

若要驗證 MPPPenAutoTuning 的 summary Excel 輸出仍維持既有資料夾與檔名規則，可在專案根目錄執行：

```powershell
./run-mpp-excel-smoke-test.ps1
```

此腳本會實際驗證：

1. 自動 output 資料夾命名規則：`output_<來源資料夾前三段>`
2. 自訂 output 資料夾命名規則
3. `Log Summary.xlsx`
4. `df_all.xlsx`
5. `df_all_M.xlsx`

若只想重跑驗證、略過重建：

```powershell
./run-mpp-excel-smoke-test.ps1 -SkipBuild
```

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
