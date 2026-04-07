# Implementation Plan: 新增 btnRead 讀取按鈕至 FingerAutoTuning 主視窗

**Branch**: `002-add-read-button` | **Date**: 2026-04-07 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/002-add-read-button/spec.md`

## Summary

在 FingerAutoTuning 模組的主視窗 `frmMain` 上新增一個名為 `btnRead` 的按鈕，點擊後以 `MessageBox.Show("Read")` 彈出模態訊息視窗。此功能為純 UI 互動，不涉及硬體通訊或資料讀取。實作方式為在 `frmMain.cs` 中新增按鈕控制項宣告、事件處理程序，並透過 Designer 機制完成 UI 佈局。

## Technical Context

**Language/Version**: C# / .NET Framework 4.8  
**Primary Dependencies**: System.Windows.Forms (內建)  
**Storage**: N/A  
**Testing**: 手動 UI 測試（啟動主視窗 → 點擊按鈕 → 驗證彈窗）；三組態 MSBuild 建置成功  
**Target Platform**: Windows 10/11, x86 (32-bit)  
**Project Type**: desktop-app (WinForms)  
**Performance Goals**: 訊息視窗 <1 秒內彈出  
**Constraints**: 不修改 `*.Designer.cs`；遵循既有按鈕風格（FlatStyle、Anchor）  
**Scale/Scope**: 單一按鈕 + 單一 MessageBox，影響範圍極小

### 既有 frmMain 結構分析

- **表單尺寸**: ClientSize 932×602 pixels
- **主要佈局**: `splitcontainerMain`（左 170px / 右 748px）→ `splitcontainerRight`（上 158px 控制面板 / 下 390px Tab 結果區）
- **既有主要按鈕** (控制面板右上角，Y=5):
  | 按鈕 | 位置 | 尺寸 | 說明 |
  |------|------|------|------|
  | `btnNewStart` | (428, 5) | 74×68 | 啟動 |
  | `btnNewStop` | (508, 5) | 74×68 | 停止 |
  | `btnNewReset` | (588, 5) | 74×68 | 重置 |
  | `btnNewPattern` | (668, 5) | 74×68 | 圖案 |
- **按鈕風格**: `FlatStyle.Flat`、`FlatAppearance.BorderSize = 0`、`Anchor = Top | Right`
- **事件連線方式**: Designer.cs 中以 `this.btnXxx.Click += new EventHandler(this.btnXxx_Click)` 宣告

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| 原則 | 狀態 | 說明 |
|------|------|------|
| I. 技術棧約束 | ✅ PASS | C# / .NET Framework 4.8 / WinForms / x86，完全符合 |
| II. 模組化架構 | ✅ PASS | 僅修改 FingerAutoTuning 模組內的 `frmMain`，不跨模組 |
| III. 最小影響原則 | ✅ PASS | 新增按鈕與事件處理，不重構既有邏輯；不修改 Designer.cs（透過程式碼動態建立控制項） |
| IV. 連動更新機制 | ✅ PASS | 新增功能需更新 `README.md`；無新檔案故 `.csproj` 無需修改 |
| V. 硬體依賴限制 | ✅ PASS | 純 UI 功能，不涉及硬體通訊，可完全在無硬體環境下驗證 |
| 編碼規範 | ✅ PASS | 按鈕命名 `btnRead` 符合既有慣例、事件處理 `btnRead_Click` 符合 PascalCase |
| SDD 品質 Gate | ✅ PASS | 三組態建置成功可驗證、不需硬體驗證 |

**GATE 結果: 全數通過，無違規項目。**

## Project Structure

### Documentation (this feature)

```text
specs/002-add-read-button/
├── plan.md              # 本檔案
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit.tasks 產出)
```

### Source Code (受影響檔案)

```text
FingerAutoTuning/
└── FingerAutoTuning/
    ├── frmMain.cs              # 新增 btnRead 控制項初始化 + Click 事件處理
    └── frmMain.Designer.cs     # [不修改] 由 VS Designer 管理
```

**Structure Decision**: 此功能影響範圍極小，僅需修改 `frmMain.cs` 一個檔案。按鈕以程式碼方式在建構子或 `InitializeComponent()` 後動態建立，避免手動編輯 Designer.cs。不新增任何檔案，不需調整 `.csproj`。

## Complexity Tracking

> 無 Constitution 違規，此區塊不適用。
