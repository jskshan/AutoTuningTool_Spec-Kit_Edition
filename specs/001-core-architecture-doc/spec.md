# Feature Specification: Brownfield Architecture Documentation — Core Modules

**Feature Branch**: `001-core-architecture-doc`  
**Created**: 2026-04-02  
**Status**: Approved  
**Input**: User description: "為 AutoTuningTool 核心模組（AnalysisFlow、DataAnalysis、通訊層）建立正式架構規格，作為後續 SDD 開發的基礎上下文。"

## User Scenarios & Testing *(mandatory)*

<!--
  本 feature 為 Brownfield 架構文件化，對象為 AI Agent 和開發者。
  User stories 描述的是 "文件使用者" 的場景，而非終端使用者操作。
-->

### User Story 1 — AI Agent 查詢分析流程契約 (Priority: P1)

作為一名 AI Agent（如 GitHub Copilot），在收到「新增一種 AnalysisFlow」的請求時，我需要查閱核心架構規格來了解：現有哪些分析流程、基類定義了哪些虛擬方法、如何在 DataAnalysis 中註冊新流程。這樣我才能產生符合既有慣例的程式碼。

**Why this priority**: 新增 AnalysisFlow 是最頻繁的擴展場景，缺乏架構規格會導致 AI 產生不一致的程式碼。

**Independent Test** (驗證類型：自動): 可驗證 — 提供 spec 給 AI Agent 後，要求其描述「新增 AnalysisFlow 的步驟」，輸出應包含：繼承基類、實作虛擬方法、在 DataAnalysis 註冊分派、更新 .csproj。

**Acceptance Scenarios**:

1. **Given** 架構 spec 已建立, **When** AI Agent 被要求新增 AnalysisFlow, **Then** Agent 能正確列出須繼承的基類、須實作的虛擬方法、須修改的 DataAnalysis 分派邏輯
2. **Given** 架構 spec 已建立, **When** AI Agent 被要求新增 AnalysisFlow, **Then** Agent 能列出所有必須連動更新的檔案（.csproj、DataAnalysis.cs、README.md）

---

### User Story 2 — 開發者理解通訊層架構 (Priority: P2)

作為一名開發者，在需要修改 ELAN 觸控面板通訊邏輯時，我需要查閱架構規格來了解：ElanCommand 命令列舉與映射表結構、ElanTouch/ElanTouch_Socket 的傳輸介面差異、Gen8 命令的擴展模式。

**Why this priority**: 通訊層修改影響 FingerAutoTuning 與 MPPPenAutoTuning 兩個模組，錯誤的修改可能導致硬體通訊失敗。

**Independent Test** (驗證類型：自動): 可驗證 — 開發者閱讀 spec 後能回答：「ElanCommand 分為哪幾類」「Gen8 擴展使用什麼資料結構」「HID/Socket/I2C 的介面常數分別是什麼」。

**Acceptance Scenarios**:

1. **Given** 通訊層 spec 區段已建立, **When** 開發者查閱, **Then** 能找到 ElanCommandType 的完整分類與數量（無參數/單參數/雙參數命令）
2. **Given** 通訊層 spec 區段已建立, **When** 開發者需修改 Socket 傳輸, **Then** 能確認需同步更新 ElanTouch.cs 和 ElanTouch_Socket.cs

---

### User Story 3 — AI Agent 理解雙模組架構差異 (Priority: P3)

作為一名 AI Agent，在收到跨模組修改請求時，我需要了解 FingerAutoTuning 與 MPPPenAutoTuning 的架構差異：前者用 AppCore 調度、後者用 ProcessFlow partial class；前者 7 個 AnalysisFlow、後者 16 個；分派機制的基準不同。

**Why this priority**: 跨模組同步是 constitution 要求的連動更新機制，理解差異才能正確同步。

**Independent Test** (驗證類型：自動): 可驗證 — AI Agent 閱讀 spec 後能正確回答「修改 ElanCommand 後，兩個模組分別需要更新哪些路徑」。

**Acceptance Scenarios**:

1. **Given** 雙模組架構 spec 已建立, **When** AI Agent 被要求同步修改通訊層, **Then** Agent 能準確列出兩個模組各自的 ElanCommand.cs 路徑
2. **Given** 雙模組架構 spec 已建立, **When** AI Agent 被要求新增 MPPPen 的分析流程, **Then** Agent 不會套用 FingerAutoTuning 的 AppCore 模式，而是使用 ProcessFlow 模式

---

### Edge Cases

- 當 AnalysisFlow 同時涉及 Gen8 和非 Gen8 IC 時，DataAnalysis 分派如何處理？（MPPPenAutoTuning 以 `nICSolutionType` 參數區分）
- 當 `Release_9F07_Socket` 組態隱藏 MPP Pen 模組時，相關 spec 區段是否仍然適用？（是，spec 記錄完整架構，組態差異在 constitution 已說明）
- 報表檔名（DataSave）在 Self 模式與 Mutual 模式的規則差異是否需要在 spec 中區分？（是，兩者有不同的參數組合邏輯）

## Requirements *(mandatory)*

### Functional Requirements

#### A. AnalysisFlow 分析流程層

- **FR-001**: Spec 必須記錄 FingerAutoTuning 的 AnalysisFlow 基類介面，包含所有虛擬方法簽名：`InitializeParameter()`、`InitializeSourceDataList()`、`LoadAnalysisParameter()`、`MainFlow(ref string sErrorMessage) → bool`；以及非虛方法 `GetDataCount() → bool`
- **FR-002**: Spec 必須列出 FingerAutoTuning 的 AnalysisFlow 基類（`AnalysisFlow_Raw.cs`）及 6 個分派目標實作，對應 MainStep 列舉值：FRPH1(FrequencyRank_Phase1=1)、FRPH2(FrequencyRank_Phase2=2)、ACFR(AC_FrequencyRank=3)、RawADCS(Raw_ADC_Sweep=4)、SelfFS(Self_FrequencySweep=5)、SelfPNS(Self_NCPNCNSweep=6)。MainStep.Else=7 無對應分派實作
- **FR-003**: Spec 必須記錄 MPPPenAutoTuning 的 AnalysisFlow 基類介面：唯一的 virtual 方法為 `LoadAnalysisParameter()`；`InitializeParameter(FlowStep)` 為 protected 非虛方法。子類慣例方法（convention-based, non-virtual）包括 `SetFileDirectory()`、`CheckDirectoryIsValid()`、`GetData()`、`ComputeAndOutputResult()`
- **FR-004**: Spec 必須列出 MPPPenAutoTuning 全部 16 個 AnalysisFlow 實作及其分類（噪音/傾角/調校/數位/壓力/線性度/其他）

#### B. DataAnalysis 分派層

- **FR-005**: Spec 必須記錄 FingerAutoTuning 的 `DataAnalysis.ExecuteMainWorkFlow()` 完整方法簽名與參數說明
- **FR-006**: Spec 必須記錄 FingerAutoTuning 的 MainStep 列舉完整值（FrequencyRank_Phase1=1 至 Else=7）以及對應的 if-else 分派邏輯
- **FR-007**: Spec 必須記錄 MPPPenAutoTuning 的 DataAnalysis 分派機制：以 `MainTuningStep` + `SubTuningStep` + `nICSolutionType` 三維分派
- **FR-008**: Spec 必須記錄 MPPPenAutoTuning 的 ProcessFlow partial class 模式（7 個檔案：MainFlow、SingleMode、ClientMode、ServerMode、GoDrawMode、LoadDataMode、CommonFunction）

#### C. 通訊與命令層

- **FR-009**: Spec 必須記錄 ElanCommandType 列舉的分類結構：FingerAutoTuning 版本共 85 個（56 無參數 + 2 單參數 + 27 雙參數）；MPPPenAutoTuning 版本共 104 個（67 無參數 + 2 單參數 + 35 雙參數）
- **FR-010**: Spec 必須記錄 ElanCommand 的靜態映射表：`dictSetCommandMappingTable` 和 `dictGetCommandMappingTable` 的對應關係
- **FR-011**: Spec 必須記錄 ElanCommand_Gen8 的擴展結構：`ParameterType` 列舉（FingerAutoTuning 29 個、MPPPenAutoTuning 37 個）、`SendCommandInfo`/`WriteCommandInfo` 資料結構
- **FR-012**: Spec 必須記錄通訊傳輸層：ElanTouch（LibTouch.dll）與 ElanTouch_Socket（LibTouch_Socket.dll）的介面常數（HID=1、I2C=8、SPI=9）

#### D. 資料儲存層

- **FR-013**: Spec 必須記錄 DataSave 的報表檔名生成規則：Mutual 模式範本、Self 模式範本（含 NCP/NCN）、RawADCS 模式範本
- **FR-014**: Spec 必須記錄 DataSave 的 CSV Frame 資料檔名規則：Mutual/Self/RawADCS 各自的參數組合

#### E. 硬體抽象層（MPPPenAutoTuning 專有）

- **FR-015**: Spec 必須記錄 `IHardware` 介面定義（Connect/Disconnect）及其實作者（InputDevice、RobotAPI、GoDrawAPI）
- **FR-016**: Spec 必須記錄 `IRS232Device` 介面與 HW 實作：ForceGauge（力量計）、LinearTable（線性表）

#### F. 應用核心協調層

- **FR-017**: Spec 必須記錄 FingerAutoTuning 的 `AppCore` 調度層職責：初始化連線、韌體命令下達、DataAnalysis 調用入口，以及其與 `frmMain` 和 `DataAnalysis` 的三層調用關係（frmMain → AppCore → DataAnalysis → AnalysisFlow）

### Key Entities

- **AnalysisFlow**: 分析流程的基類抽象，定義頻率掃描/資料分析/結果輸出的標準介面。FingerAutoTuning 有 7 個實作，MPPPenAutoTuning 有 16 個實作。
- **MainStep / MainTuningStep**: 流程步驟列舉，作為 DataAnalysis 分派至對應 AnalysisFlow 的識別鍵。
- **ElanCommand**: ELAN 觸控 IC 的韌體命令集，以列舉 + 靜態映射表組織，區分 Get/Set 配對命令。
- **ElanCommand_Gen8**: Gen8 世代 IC 的擴展命令結構，使用 `WriteCommandInfo` 複合資料結構取代簡單列舉。
- **DataSave**: 報表與資料檔案的命名/儲存引擎，根據掃描模式（Mutual/Self/RawADCS）和參數組合生成唯一檔名。
- **ProcessFlow**: MPPPenAutoTuning 特有的 partial class 流程管理架構，支援單機/客戶端/伺服器/GoDrawRobot/離線分析五種執行模式。
- **TraceInfo**: 觸控面板的 RX/TX 追蹤矩陣資訊結構，包含晶片數量、追蹤數、軸分佈。
- **FileCheckInfo / RawADCS_FileCheckInfo / Self_FileCheckInfo**: AnalysisFlow 內部的資料驗證結構，用於檢查掃描資料檔案的參數完整性。

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: AI Agent 使用此 spec 作為上下文時，能在 `/speckit.plan` 階段正確產出引用的類別名稱、方法簽名均存在於程式碼中的技術實作計畫
- **SC-003**: Spec 中描述的所有類別名稱、方法簽名、列舉值，與實際程式碼 100% 吻合（可透過 grep 驗證）
- **SC-004**: 開發者或 AI Agent 閱讀 spec 後，能列出 ≥4 個「新增 AnalysisFlow 需修改的檔案路徑」

## Assumptions

- 本 spec 記錄的是現有程式碼的架構事實，不包含任何新功能設計
- FingerAutoTuning 和 MPPPenAutoTuning 的程式碼在 spec 撰寫期間不會發生結構性變更
- 硬體通訊層（ElanTouch）的 native DLL（LibTouch.dll / LibTouch_Socket.dll）為黑盒，spec 僅記錄 P/Invoke 簽名，不涉及 DLL 內部實作
- Gen8 命令結構的完整參數列表以程式碼中 `ParameterType` 列舉為準，spec 記錄分類與結構而非逐一列舉所有值
- DataSave 的檔名規則以程式碼邏輯為準，spec 記錄範本而非窮舉所有組合
