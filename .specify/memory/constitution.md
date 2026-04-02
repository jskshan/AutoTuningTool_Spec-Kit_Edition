# AutoTuningTool Constitution

> ELAN 觸控螢幕自動調校工具 — 專案治理原則與開發規範。
> 本 constitution 為所有 AI Agent 操作本專案時的最高指引，與 `AGENTS.md` 互補共存。

## Core Principles

### I. 技術棧約束 (Technology Constraints)

- **語言/框架**：C# / .NET Framework 4.8 / Windows Forms
- **平台目標**：x86 (32-bit)，不支援 x64 或 AnyCPU
- **開發環境**：Visual Studio 2019+，Windows 10/11
- **主解決方案**：`AutoTuning_NewUI.sln`
- **版本**：2.0.0.0-beta1
- **建置組態**：
  | 組態 | 定義符號 | 說明 |
  |------|---------|------|
  | `Debug\|x86` | `TRACE`, `DEBUG`, `_USE_VC2010` | 偵錯版本 |
  | `Release\|x86` | `TRACE`, `_USE_VC2010` | 正式發布版本 |
  | `Release_9F07_Socket\|x86` | `TRACE`, `_USE_VC2010`, `_USE_9F07_SOCKET` | DirectTouch 模式（隱藏 MPP Pen） |

### II. 模組化架構 (Modular Architecture)

專案採三層模組化設計，各層職責明確：

```
AutoTuning_NewUI (主程式入口, WinExe)
├── FingerAutoTuning (手指觸控調校模組, Class Library)
│   ├── AppCore — 執行環境初始化、裝置連線、FW 命令下達
│   ├── DataAnalysis — 根據 MainStep 列舉分派至對應 AnalysisFlow
│   ├── AnalysisFlow_* — 7 個分析流程 (ACFR, FRPH1, FRPH2, Raw, RawADCS, SelfFS, SelfPNS)
│   └── DataSave — 報告檔名生成與資料儲存
│
└── MPPPenAutoTuning (MPP 觸控筆調校模組, Class Library)
    ├── ProcessFlow_* — 流程模式管理
    ├── AnalysisFlow/* — 16+ 分析流程
    └── Interface/ — 硬體抽象層
```

- 修改 AnalysisFlow / ProcessFlow 時，必須檢查 `DataAnalysis.cs` 中的 `MainStep` 列舉與分派邏輯
- 修改通訊層時，必須同步檢查 FingerAutoTuning 與 MPPPenAutoTuning 兩個模組

### III. 最小影響原則 (Minimal Impact)

- 修改程式碼時盡量降低影響範圍，非必要不大幅重構
- 不修改 `*.Designer.cs` — 由 Visual Studio Windows Forms Designer 自動產生
- 不自行新增條件編譯符號 — 使用 `#if _USE_9F07_SOCKET` 等既有符號
- 新增 method 或功能時須加入中文 `///` 註解

### IV. 連動更新機制 (Cascading Updates)

程式碼變更時務必觸發以下連鎖更新：

1. **新增功能/檔案**: 必更 `README.md` + 對應 `.csproj`
2. **修改 AnalysisFlow / ProcessFlow**: 檢查 `DataAnalysis.cs` MainStep 分派、`DataSave.cs` 檔名規則
3. **修改 ELAN 命令集/通訊層**: 同步兩個模組的對應實作
4. **修改 NuGet 套件版本**: 同步 `packages.config`、`.csproj`、`App.config` assemblyBinding

### V. 硬體依賴限制 (Hardware Dependencies)

- AutoTuningTool 需要 ELAN 觸控面板硬體裝置才能執行完整功能測試
- 若測試或驗證受限（缺少硬體），需在回報中註明限制並提出可行的驗證方式
- Spec 的 acceptance criteria 應明確區分「可自動驗證」與「需硬體驗證」項目

## 編碼規範 (Coding Standards)

### 命名慣例
- 類別、方法、屬性：PascalCase（如 `DataAnalysis`、`GetFrameData`）
- 私有欄位：`m_` 前綴 + camelCase（如 `m_cAppCore`、`m_bConnectFlag`）
- 常數與列舉值：PascalCase
- 區域變數與參數：camelCase

### 檔案命名慣例
- 分析流程：`AnalysisFlow_<名稱>.cs`
- 表單：`frm<名稱>.cs` + `frm<名稱>.Designer.cs` + `frm<名稱>.resx`
- 自訂控制項：`ctrl<名稱>.cs` + `ctrl<名稱>.Designer.cs`
- ELAN 命令集：`ElanCommand.cs` / `ElanCommand_Gen8.cs`
- AppCore 細分模組：`AppCore/<動詞+名詞>.cs`
- 流程管理：`ProcessFlow_<模式>.cs`

### NuGet 套件管理
- 修改 `packages.config` 後需同步更新 `.csproj` 參考路徑
- 還原套件：`./restore-packages.ps1`

## 語言與回應規範 (Communication Standards)

- 回覆使用**繁體中文**
- 保留專業術語、程式碼與技術名詞的英文原文
- 執行動作前先以一句話說明步驟；完成後簡述結果
- 重要輸出提供受影響檔案路徑與關鍵差異
- 結束回報包含：完成事項、相關檔案路徑、建議下一步

## Git 工作流 (Git Workflow)

- 採用 **Conventional Commits** 規範：`<type>: <繁體中文摘要>`
- 類型前綴：`feat` / `fix` / `refactor` / `docs` / `style` / `test` / `chore` / `perf`
- Commit body 列出變更項目：`- <檔案>: <說明>`（新增標註 `[新增]`、刪除標註 `[已刪除]`）
- Windows 環境中文 commit 嚴禁使用 `-m` 參數，改用 `-F .git_commit_msg_temp`（UTF-8 暫存檔）
- 複合變更以主要變更類型為準，過於複雜則拆分多個 commit

## Spec-Driven Development 工作流

本專案採用 GitHub Spec-Kit 進行 Specification-Driven Development：

1. `/speckit.constitution` — 建立/更新本 constitution
2. `/speckit.specify` — 定義功能需求（聚焦 what & why，不限定技術實作）
3. `/speckit.clarify` — 釐清模糊需求（建議在 plan 前執行）
4. `/speckit.plan` — 制定技術實作計畫（指定 C# / .NET Framework 4.8 / WinForms / x86）
5. `/speckit.analyze` — 交叉一致性分析（plan 後、implement 前）
6. `/speckit.tasks` — 產生可執行任務清單
7. `/speckit.implement` — 執行實作

### SDD 品質 Gate
- Plan 中必須明確記載技術約束（.NET Framework 4.8、x86、硬體依賴）
- 驗收標準須包含「三組態 MSBuild 建置成功」
- 涉及硬體操作的 task 須標註「需硬體驗證」

## Governance

- 本 constitution 為 AI Agent 操作的最高指引
- 與 `AGENTS.md` 互補：constitution 定義治理原則，AGENTS.md 提供詳細操作指南
- 修改 constitution 須同步更新 AGENTS.md 中的對應描述
- 所有 spec/plan/implementation 須遵循本 constitution 規範
- 不確定的架構決策須先以 `/speckit.clarify` 釐清再行實作

**Version**: 1.0.0 | **Ratified**: 2026-04-02 | **Last Amended**: 2026-04-02
