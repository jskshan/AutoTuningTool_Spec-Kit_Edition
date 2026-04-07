# AutoTuningTool — Codebase Architecture Manifest

> 本文件為 AI Agent 讀取專案架構的結構化摘要。
> 更新日期：2026-04-07 | 版本：2.0.0.0-beta1

---

## 1. 專案概述

**AutoTuningTool** 是 ELAN（義隆電子）觸控螢幕自動調校工具，用於在生產線或工程環境中，透過 ELAN IC 命令集自動執行觸控參數的量測與調校。

| 項目 | 說明 |
|------|------|
| 語言 / 框架 | C# / .NET Framework 4.8 / Windows Forms |
| 平台目標 | x86 (32-bit) |
| 方案檔 | `AutoTuning_NewUI.sln` |
| 產出執行檔 | `AutoTuning.exe` |

---

## 2. 解決方案結構

```
AutoTuning_NewUI.sln
├── AutoTuning_NewUI/          # 主程式 (WinExe) — 入口與殼層
│   ├── frmMain.cs             # 主視窗，管理子應用切換
│   └── Program.cs             # 進入點 (STAThread)
│
├── FingerAutoTuning/          # 手指觸控調校模組 (Class Library)
│   ├── AppCore.cs + AppCore/  # 核心邏輯 (partial class, 22+1 檔)
│   ├── DataAnalysis.cs        # 分析流程分派器
│   ├── DataSave.cs            # 報告資料儲存
│   ├── AnalysisFlow_*.cs (7)  # 7 條分析流程
│   ├── frm*.cs (20+)          # UI 表單
│   ├── ctrl*.cs (4)           # 自訂控制項
│   ├── Class/                 # ELAN 通訊層 + 基礎設施
│   └── ParamBase.cs + ParamMgr.cs  # 參數管理
│
├── MPPPenAutoTuning/          # MPP 觸控筆調校模組 (Class Library)
│   ├── frmMain.cs + Partials  # 主 UI (partial class, 3 檔)
│   ├── ProcessFlow*.cs (7)    # 流程控制 (partial class, 7 檔)
│   ├── ConnectFlow.cs         # 連線流程
│   ├── DataAnalysis.cs        # 分析流程分派器
│   ├── AnalysisFlow/ (17)     # 17 條分析流程
│   ├── Class/                 # 通訊、演算法、API
│   │   ├── HW/               # 硬體抽象
│   │   ├── GetFrameData/     # Frame 資料取得
│   │   ├── Algorithm/        # 壓力演算法等
│   │   └── API/              # RobotAPI, GoDrawAPI, SocketAPI
│   └── Interface/             # IHardware, IRS232Device
│
├── packages/                  # NuGet 套件
├── icon/                      # UI 圖示資源
└── runtime/                   # Runtime 套件檔案
```

---

## 3. 專案間參考關係

```
AutoTuning_NewUI (WinExe)
    ├──► FingerAutoTuning (Class Library)
    └──► MPPPenAutoTuning (Class Library)

FingerAutoTuning 和 MPPPenAutoTuning 彼此無直接參考。
共享概念（如 ElanCommand、FrameMgr）各自擁有獨立副本。
```

**AutoTuning_NewUI 使用方式：**
```csharp
FingerAutoTuning.frmMain m_cfrmFingerAutoTuning;  // 手指觸控子視窗
MPPPenAutoTuning.frmMain m_cfrmMPPPenAutoTuning;  // MPP Pen 子視窗
```

---

## 4. 建置組態

| 組態 | 定義符號 | 說明 |
|------|---------|------|
| `Debug\|x86` | `TRACE`, `DEBUG`, `_USE_VC2010` | 偵錯版本 |
| `Release\|x86` | `TRACE`, `_USE_VC2010` | 正式發布版本 |
| `Release_9F07_Socket\|x86` | `TRACE`, `_USE_VC2010`, `_USE_9F07_SOCKET` | DirectTouch 模式（隱藏 MPP Pen） |

**MSBuild 路徑：** `C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe`

---

## 5. 核心架構模式

### 5.1 分析流程分派 (Strategy Pattern)

兩個模組均使用 `DataAnalysis` 類別作為流程分派器，根據列舉值選擇對應的 `AnalysisFlow_*` 實作：

- **FingerAutoTuning**: `MainStep` → 7 條流程 (FRPH1, FRPH2, ACFR, RawADCS, SelfFS, SelfPNS, Raw)
- **MPPPenAutoTuning**: `MainTuningStep` → 17 條流程 (Noise, TiltNoise, DigiGain, TPGain, PeakCheck, Digital, Tilt, Pressure, Linearity 等)

### 5.2 Partial Class 拆分

- **FingerAutoTuning.AppCore**: 1 主檔 + 22 子檔（按職責拆分：連線、資料取得、命令執行、參數管理等）
- **MPPPenAutoTuning.ProcessFlow**: 7 個 partial 檔（按運行模式拆分：Single、Server、Client、GoDraw、LoadData、MainFlow、CommonFunction）
- **MPPPenAutoTuning.frmMain**: 3 個 partial 檔（EventHandlers、Parameters、Designer）

### 5.3 ELAN IC 通訊層

兩模組各自維護通訊層，支援多種連線介面：

| 通訊方式 | FingerAutoTuning | MPPPenAutoTuning |
|----------|:---:|:---:|
| USB HID | ✓ | ✓ |
| I2C Bridge SPI | ✓ | ✓ |
| Socket (DirectTouch) | ✓ (`_USE_9F07_SOCKET`) | - |
| SSH Socket | ✓ | - |
| Chrome Remote | ✓ | - |
| Robot Socket | - | ✓ |
| Server Socket | - | ✓ |

### 5.4 參數管理

- **FingerAutoTuning**: `ParamBase` (INI/XML 讀寫) + `ParamMgr` (ParamTestItem → ParamGroup → ParamItem 階層)
- **MPPPenAutoTuning**: `ParameterBase` (類似 ParamBase)

---

## 6. 關鍵列舉定義

### FingerAutoTuning

```csharp
enum MainStep {
    FrequencyRank_Phase1, FrequencyRank_Phase2,
    AC_FrequencyRank, Raw_ADC_Sweep,
    Self_FrequencySweep, Self_NCPNCNSweep, Else
}
```

### MPPPenAutoTuning

```csharp
enum MainTuningStep {
    NO, TILTNO, DIGIGAINTUNING, TPGAINTUNING,
    PEAKCHECKTUNING, DIGITALTUNING, TILTTUNING,
    PRESSURETUNING, LINEARITYTUNING, SERVERCONTRL, ELSE
}

enum SubTuningStep {
    NO, HOVER_1ST, HOVER_2ND, CONTACT, PRESSURETABLE, ...
}
```

---

## 7. 外部相依套件

| 套件 | 版本 | 用途 |
|------|------|------|
| EPPlus | 6.2.6 | Excel 報表產生 |
| FontAwesome.Sharp | 5.15.4 | UI 圖示 |
| RJCodeAdvance.RJControls | 1.0.0 | 自訂 WinForms 控制項 |
| Microsoft.IO.RecyclableMemoryStream | 1.4.1 | 記憶體串流效能優化 |
| System.Resources.Extensions | 4.7.1 | 資源延伸 |

---

## 8. 資料流概述

### FingerAutoTuning 資料流
```
使用者選擇測試步驟
  → AppCore.ExecuteMainWorkFlow()
    → ConnectDevice() → EnterTestMode()
    → RecordData() / GetFrameData()
    → DataAnalysis.ExecuteMainWorkFlow()
      → AnalysisFlow_*.Execute()
    → DataSave.CreateRecordData()
    → OutputResult()
```

### MPPPenAutoTuning 資料流
```
使用者選擇測試步驟 + Robot 模式
  → ProcessFlow.RunMainProcessThread()
    → ConnectFlow → 連線設備
    → Robot 控制 (Single/Server/Client/GoDraw)
    → 執行觸控筆動作 → 收集報告資料
    → DataAnalysis.LoadData()
      → AnalysisFlow_*.Execute()
    → 輸出調校結果
```

---

## 9. 檔案約定

| 模式 | 命名範例 |
|------|---------|
| 分析流程 | `AnalysisFlow_<Name>.cs` |
| 表單 | `frm<Name>.cs` + `frm<Name>.Designer.cs` + `frm<Name>.resx` |
| 自訂控制項 | `ctrl<Name>.cs` + `ctrl<Name>.Designer.cs` |
| AppCore 子模組 | `AppCore/<VerbNoun>.cs` (如 `GetFrameData.cs`) |
| ProcessFlow 子模組 | `ProcessFlow_<Mode>.cs` (如 `ProcessFlow_SingleMode.cs`) |
| ELAN 命令 | `ElanCommand.cs` / `ElanCommand_Gen8.cs` |

---

## 10. 條件編譯與功能分支

| 符號 | 組態 | 影響範圍 |
|------|------|---------|
| `_USE_9F07_SOCKET` | Release_9F07_Socket | AutoTuning_NewUI: 隱藏 MPP Pen 按鈕; FingerAutoTuning: 啟用 Socket 連線模式、變更測試步驟數 |
| `_USE_VC2010` | 全部 | VC2010 DLL 相容性 |
| `DEBUG` | Debug | 僅偵錯時啟用的日誌/斷言 |
