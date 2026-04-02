# Implementation Plan: Brownfield Architecture Documentation — Core Modules

**Branch**: `001-core-architecture-doc` | **Date**: 2026-04-02 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/001-core-architecture-doc/spec.md`

## Summary

為 AutoTuningTool 核心模組（AnalysisFlow、DataAnalysis、通訊層、DataSave、AppCore、ProcessFlow）建立正式架構規格文件。本 feature 不修改任何程式碼，僅產出文件，記錄現有程式碼的介面契約、分派邏輯與通訊結構，供 AI Agent 和開發者作為後續 SDD 開發的基礎上下文。

## Technical Context

**Language/Version**: C# / .NET Framework 4.8  
**Primary Dependencies**: Windows Forms, EPPlus 6.2.6, FontAwesome.Sharp 5.15.4  
**Storage**: 檔案系統（CSV/Excel 報表）  
**Testing**: 自動驗證（grep 符號一致性比對）  
**Target Platform**: Windows x86 (32-bit)  
**Project Type**: Desktop app (WinForms) — Brownfield 架構文件化  
**Constraints**: 不修改任何程式碼、不需硬體裝置  

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| 原則 | 狀態 | 說明 |
|------|------|------|
| II. 模組化架構 | ✅ 通過 | 計畫涵蓋 FingerAutoTuning + MPPPenAutoTuning + 通訊層 + AppCore |
| III. 最小影響 | ✅ 通過 | 不修改任何程式碼，僅新增文件 |
| IV. 連動更新 | ✅ 通過 | 新增文件後將更新 README.md |
| V. 硬體依賴 | ✅ 通過 | 所有驗證步驟皆為自動驗證，不需硬體 |

## Project Structure

### Documentation (this feature)

```text
specs/001-core-architecture-doc/
├── spec.md              # 功能規格（已完成）
├── plan.md              # 本文件 — 技術實作計畫
├── research.md          # Phase A-1: 原始碼事實收集
├── data-model.md        # Phase A-3: 關鍵資料結構文件
├── quickstart.md        # Phase A-4: 快速參考指南
├── contracts/           # Phase A-2: 介面契約文件
│   ├── finger-analysisflow.md
│   ├── mpp-analysisflow.md
│   ├── data-analysis-dispatch.md
│   ├── elan-command.md
│   └── communication-layer.md
└── tasks.md             # Phase 2 output (/speckit.tasks)
```

### Source Code (repository root — 唯讀參考)

```text
FingerAutoTuning/FingerAutoTuning/
├── AnalysisFlow_Raw.cs          # 基類定義
├── AnalysisFlow_FRPH1.cs        # 分派目標 (MainStep=1)
├── AnalysisFlow_FRPH2.cs        # 分派目標 (MainStep=2)
├── AnalysisFlow_ACFR.cs         # 分派目標 (MainStep=3)
├── AnalysisFlow_RawADCS.cs      # 分派目標 (MainStep=4)
├── AnalysisFlow_SelfFS.cs       # 分派目標 (MainStep=5)
├── AnalysisFlow_SelfPNS.cs      # 分派目標 (MainStep=6)
├── AppCore.cs                   # 協調層
├── DataAnalysis.cs              # 分派層
├── DataSave.cs                  # 報表儲存層
├── frmMain.cs                   # FlowStep/MainStep 定義
└── Class/
    ├── ElanCommand.cs           # 命令列舉/映射
    ├── ElanCommand_Gen8.cs      # Gen8 擴展結構
    └── ElanTouch.cs             # HID/I2C/SPI 通訊

MPPPenAutoTuning/MPPPenAutoTuning/
├── AnalysisFlow/
│   └── AnalysisFlow_Raw.cs      # 基類定義（16 個子類）
├── ProcessFlow_MainFlow.cs      # partial class (7 個檔案)
├── ProcessFlow_SingleMode.cs
├── ProcessFlow_ClientMode.cs
├── ProcessFlow_ServerMode.cs
├── ProcessFlow_GoDrawMode.cs
├── ProcessFlow_LoadDataMode.cs
├── ProcessFlow_CommonFunction.cs
├── DataAnalysis.cs              # 三維分派
├── ParameterProperties.cs       # MainTuningStep/SubTuningStep
└── Class/
    ├── ElanCommand.cs           # 命令列舉 (104 個)
    └── ElanCommand_Gen8.cs      # Gen8 擴展 (37 ParameterType)
```

## Implementation Steps

### Phase A: 架構文件產出（核心交付物）

#### Step 1: 建立 research.md — 原始碼事實收集
- **前置依賴**：無
- **產出**：`specs/001-core-architecture-doc/research.md`
- **工作內容**：從程式碼中逐模組提取精確事實，含方法簽名、列舉值、行號對照

#### Step 2: 建立 contracts/ — 介面契約文件
- **前置依賴**：Step 1（research.md 事實驗證）
- **產出**：5 個 contract 檔案
- **格式**：介面簽名 → 行為契約 → 擴展指南 → 連動檔案清單

#### Step 3: 建立 data-model.md — 資料結構文件
- **前置依賴**：與 Step 2 平行
- **產出**：`specs/001-core-architecture-doc/data-model.md`
- **工作內容**：記錄 FlowStep、SaveDataInfo、FileCheckInfo、TraceInfo、命令結構等

#### Step 4: 建立 quickstart.md — 快速參考指南
- **前置依賴**：Step 2 + Step 3
- **產出**：`specs/001-core-architecture-doc/quickstart.md`
- **工作內容**：操作 checklist（新增 AnalysisFlow、新增 ElanCommand、修改通訊層）

### Phase B: 連動更新

#### Step 5: 更新 spec.md Status + README.md
- **前置依賴**：Phase A 全部完成
- **產出**：修改 spec.md Status → Approved、更新 README.md 專案結構

## Verification

1. **符號一致性**：contracts/ 中引用的類別名稱、方法簽名、列舉值，可透過 `grep -r` 驗證 100% 存在於程式碼中（SC-003）
2. **FR 完整性**：逐條比對 FR-001 ~ FR-017，確認每個 FR 在 research.md + contracts/ 中都有對應區段
3. **快速參考測試**：AI Agent 使用 quickstart.md 後能列出 ≥4 個「新增 AnalysisFlow 需修改的檔案」（SC-004）

## Decisions

- **三層文件結構**（research → contracts → quickstart）而非單一巨型文件：各文件有明確使用者（research 供驗證、contracts 供參考、quickstart 供操作）
- **不逐一列舉命令值**：僅記錄分類結構與數量，精確值以程式碼為準（符合 spec Assumptions）
- **兩模組獨立 contract**：FingerAutoTuning 與 MPPPenAutoTuning 架構差異足夠大（virtual vs convention-based），分開記錄
- **MPPPen 16 個 AnalysisFlow 分類**：標註為「文件歸納分類」，附 DataAnalysis.cs 分派邏輯作為事實依據

## Complexity Tracking

> 無 Constitution Check 違規，此區段無需填寫。
