# Tasks: Brownfield Architecture Documentation — Core Modules

**Input**: Design documents from `/specs/001-core-architecture-doc/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/, quickstart.md

**Tests**: 未在 spec 中明確要求 TDD，故不包含獨立測試任務。驗證透過 Phase 6 的自動比對完成。

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Feature spec directory**: `specs/001-core-architecture-doc/`
- **FingerAutoTuning source (read-only)**: `FingerAutoTuning/FingerAutoTuning/`
- **MPPPenAutoTuning source (read-only)**: `MPPPenAutoTuning/MPPPenAutoTuning/`
- 本 feature 僅產出文件，不修改任何已有程式碼

## Plan ↔ Tasks 階段對照

| plan.md | tasks.md | 說明 |
|---------|----------|------|
| Phase A Step 1 (research.md) | Phase 2: T003-T010 | 原始碼事實收集 |
| Phase A Step 2 (contracts/) | Phase 3: T012-T014 + Phase 4: T015-T016b | 介面契約（依 User Story 拆分） |
| Phase A Step 3 (data-model.md) | Phase 2: T011 | 資料結構文件 |
| Phase A Step 4 (quickstart.md) | Phase 5: T017 | 快速參考指南 |
| Phase B Step 5 (status + README) | Phase 6: T018-T021 | 連動更新與驗證 |
| — | Phase 1: T001-T002 | Setup（plan.md 未列為獨立步驟） |

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: 建立 feature 目錄結構與實作計畫

- [ ] T001 建立 feature 目錄 `specs/001-core-architecture-doc/` 與子目錄 `contracts/`
- [ ] T002 建立 `specs/001-core-architecture-doc/plan.md` 實作計畫，定義 Phase A/B 交付步驟與驗證標準

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: 從程式碼提取架構事實，作為所有 User Story 契約文件的事實基礎

**⚠️ CRITICAL**: 所有 User Story 的契約文件均依賴此階段產出的事實資料

- [ ] T003 建立 `specs/001-core-architecture-doc/research.md` — 從 FingerAutoTuning 提取 AnalysisFlow 基類定義、虛擬方法簽名、受保護欄位、巢狀類別 (FileCheckInfo/RawADCS_FileCheckInfo/Self_FileCheckInfo)，來源檔案：`FingerAutoTuning/FingerAutoTuning/AnalysisFlow_Raw.cs`
- [ ] T004 將 DataAnalysis 分派邏輯事實補入 `specs/001-core-architecture-doc/research.md` — 提取 ExecuteMainWorkFlow 簽名、MainStep 列舉、兩段式 if-else 分派模式，來源檔案：`FingerAutoTuning/FingerAutoTuning/DataAnalysis.cs`、`FingerAutoTuning/FingerAutoTuning/frmMain.cs`
- [ ] T005 將 AppCore 協調層事實補入 `specs/001-core-architecture-doc/research.md` — 提取內部物件清單、調用關係鏈，來源檔案：`FingerAutoTuning/FingerAutoTuning/AppCore.cs`
- [ ] T006 將 DataSave 儲存層事實補入 `specs/001-core-architecture-doc/research.md` — 提取 CreateRecordData 簽名、Self/Mutual/RawADCS 檔名模板，來源檔案：`FingerAutoTuning/FingerAutoTuning/DataSave.cs`
- [ ] T007 [P] 將 ElanCommand 命令集事實補入 `specs/001-core-architecture-doc/research.md` — 提取 ElanCommandType (85值)、ICValueTargetType (29值)、映射表、轉換方法，來源檔案：`FingerAutoTuning/FingerAutoTuning/Class/ElanCommand.cs`
- [ ] T008 [P] 將 ElanCommand_Gen8 事實補入 `specs/001-core-architecture-doc/research.md` — 提取 ParameterType (29值)、SendCommandInfo/WriteCommandInfo 結構、DataType/ParameterClass 常數，來源檔案：`FingerAutoTuning/FingerAutoTuning/Class/ElanCommand_Gen8.cs`
- [ ] T009 [P] 將 ElanTouch 通訊層事實補入 `specs/001-core-architecture-doc/research.md` — 提取介面常數 (HID=1/I2C=8/SPI=9)、TraceInfo 結構、TraceMode 列舉、callback 委派，來源檔案：`FingerAutoTuning/FingerAutoTuning/Class/ElanTouch.cs`
- [ ] T010 [P] 將 MPPPenAutoTuning 事實補入 `specs/001-core-architecture-doc/research.md` — 提取 MainTuningStep (11值)、SubTuningStep (20值)、AnalysisFlow 基類 (1 virtual 方法)、17 個子類分類表、ProcessFlow 7 檔案，來源檔案：`MPPPenAutoTuning/MPPPenAutoTuning/ParameterProperties.cs`、`MPPPenAutoTuning/MPPPenAutoTuning/AnalysisFlow/AnalysisFlow_Raw.cs`、`MPPPenAutoTuning/MPPPenAutoTuning/ProcessFlow_*.cs`
- [ ] T011 建立 `specs/001-core-architecture-doc/data-model.md` — 記錄跨層共用資料結構：FlowStep、MainStep/MainTuningStep/SubTuningStep、FileCheckInfo 家族、SaveDataInfo、TraceInfo、ElanCommand 結構、TraceMode、跨層資料流圖

**Checkpoint**: research.md + data-model.md 完成 — 事實基礎就緒，可開始各 User Story 的契約文件

---

## Phase 3: User Story 1 — AI Agent 查詢分析流程契約 (Priority: P1) 🎯 MVP

**Goal**: 產出 AnalysisFlow 與 DataAnalysis 分派的介面契約文件，讓 AI Agent 能正確理解如何新增分析流程

**Independent Test**: AI Agent 閱讀 contracts/ 後，能列出「新增 AnalysisFlow 的 5 個步驟」（繼承基類 → 實作虛擬方法 → 註冊分派 → 更新 .csproj → 更新 README）

**覆蓋 FR**: FR-001, FR-002, FR-003, FR-004, FR-005, FR-006, FR-007, FR-008, FR-017

### Implementation for User Story 1

- [ ] T012 [P] [US1] 建立 `specs/001-core-architecture-doc/contracts/finger-analysisflow.md` — FingerAutoTuning AnalysisFlow 介面契約：基類虛擬方法簽名 (FR-001)、6 個分派目標對照表 (FR-002)、建構子模式、行為契約、擴展指南 checklist、連動檔案清單
- [ ] T013 [P] [US1] 建立 `specs/001-core-architecture-doc/contracts/mpp-analysisflow.md` — MPPPenAutoTuning AnalysisFlow 介面契約：唯一 virtual 方法 (FR-003)、convention-based 方法清單、17 個子類分類表 (FR-004)、與 FingerAutoTuning 差異對比、擴展指南 checklist
- [ ] T014 [US1] 建立 `specs/001-core-architecture-doc/contracts/data-analysis-dispatch.md` — 雙模組分派邏輯契約：FingerAutoTuning ExecuteMainWorkFlow 簽名與參數 (FR-005)、MainStep 列舉與兩段式 if-else (FR-006)、MPPPenAutoTuning 三維分派 (FR-007)、ProcessFlow 7 檔案表 (FR-008)、AppCore 協調層 (FR-017)、雙模組架構比較表

**Checkpoint**: US1 契約完成 — AI Agent 可查詢完整的 AnalysisFlow 新增流程

---

## Phase 4: User Story 2 — 開發者理解通訊層架構 (Priority: P2)

**Goal**: 產出 ElanCommand 命令結構與通訊傳輸層的介面契約，讓開發者理解命令映射與硬體通訊機制

**Independent Test**: 開發者閱讀後能回答「ElanCommand 分為哪幾類」「Gen8 使用什麼資料結構」「HID/I2C/SPI 介面常數」

**覆蓋 FR**: FR-009, FR-010, FR-011, FR-012, FR-013, FR-014, FR-015, FR-016

### Implementation for User Story 2

- [ ] T015 [P] [US2] 建立 `specs/001-core-architecture-doc/contracts/elan-command.md` — ElanCommand 命令結構契約：ElanCommandType 分類統計 (FR-009)、ICValueTargetType 映射規則與轉換方法 (FR-010)、ElanCommand_Gen8 擴展結構 (FR-011)、雙模組命令數量差異、擴展指南 checklist
- [ ] T016 [P] [US2] 建立 `specs/001-core-architecture-doc/contracts/communication-layer.md` — 通訊傳輸層契約：ElanTouch 介面常數與 TP_INTERFACE_TYPE (FR-012)、TraceInfo 結構與 TraceMode 位元旗標、callback 機制、IHardware/IRS232Device 介面 (FR-015, FR-016)、nICSolutionType 分支邏輯、連動檔案清單
- [ ] T016b [P] [US2] 擴充 `specs/001-core-architecture-doc/data-model.md` §4 — DataSave 檔名生成規則：Mutual 模式範本、Self 模式範本（含 NCP/NCN）、RawADCS 模式範本 (FR-013, FR-014)

**Checkpoint**: US2 契約完成 — 開發者可查詢完整的通訊層架構

---

## Phase 5: User Story 3 — AI Agent 理解雙模組架構差異 (Priority: P3)

**Goal**: 產出開發速查手冊與跨模組比較，確保 AI Agent 和開發者不會混淆兩模組的架構模式

**Independent Test**: AI Agent 閱讀後不會將 FingerAutoTuning 的 AppCore 模式套用至 MPPPenAutoTuning，且能列出 ≥4 個「新增 AnalysisFlow 需修改的檔案」(SC-004)

**覆蓋 FR**: 跨 US1/US2 整合（quickstart.md 為綜合速查手冊，不承接新 FR）

### Implementation for User Story 3

- [ ] T017 [US3] 建立 `specs/001-core-architecture-doc/quickstart.md` — 開發速查手冊：新增 FingerAutoTuning AnalysisFlow 步驟 (含程式碼範本)、新增 MPPPenAutoTuning AnalysisFlow 步驟、新增 ElanCommand 步驟、通訊層修改 checklist、DataSave 檔名規則參考、連動更新總覽矩陣、常見問題 (雙模組差異比較表)

**Checkpoint**: 全部文件產出完成 — 3 個 User Story 均可獨立驗證

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: 連動更新與一致性驗證

- [ ] T018 更新 `specs/001-core-architecture-doc/spec.md` — 將 Status 從 Draft 改為 Approved
- [ ] T019 更新 `README.md` — 在專案結構中加入 `specs/001-core-architecture-doc/` 子目錄描述（含 spec.md、plan.md、research.md、data-model.md、quickstart.md、contracts/）
- [ ] T020 執行符號一致性驗證 (SC-003) — 使用 grep 比對 contracts/ 中引用的類別名稱、方法簽名、列舉值是否 100% 存在於程式碼中
- [ ] T021 執行快速參考測試 (SC-004) — 驗證 AI Agent 閱讀 quickstart.md 後能列出 ≥4 個「新增 AnalysisFlow 需修改的檔案路徑」

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories
- **User Stories (Phase 3-5)**: All depend on Foundational phase completion
  - US1 and US2 can proceed in parallel (different contract files, no dependencies)
  - US3 depends on US1 + US2 completion (quickstart.md references both modules' contracts)
- **Polish (Phase 6)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Phase 2 — No dependencies on other stories
- **User Story 2 (P2)**: Can start after Phase 2 — No dependencies on other stories, can run in parallel with US1
- **User Story 3 (P3)**: Depends on US1 + US2 — quickstart.md synthesizes both modules' patterns into unified developer guide

### Within Each Phase

- Phase 2: T003→T004→T005→T006 (sequential FingerAutoTuning facts), T007/T008/T009/T010 can parallel with each other after T003
- Phase 3: T012 and T013 can parallel (different files), T014 depends on T012+T013 (references both contracts)
- Phase 4: T015 and T016 can fully parallel
- Phase 6: T018 and T019 can parallel, T020 and T021 depend on all prior phases

### Parallel Opportunities

- **Phase 2**: T007, T008, T009, T010 can all run in parallel (independent source files)
- **Phase 3+4**: US1 and US2 can run fully in parallel since they produce different contract files
- **Phase 3 internal**: T012 (finger) and T013 (mpp) can parallel
- **Phase 4 internal**: T015 (elan-command) and T016 (communication) can parallel
- **Phase 6**: T018 (spec.md) and T019 (README.md) can parallel

---

## Parallel Example: User Story 1 + User Story 2

```
# Phase 2 完成後，US1 和 US2 可同時開始：

# 同時啟動 US1 的兩個平行任務：
Task T012: contracts/finger-analysisflow.md
Task T013: contracts/mpp-analysisflow.md

# 同時啟動 US2 的兩個平行任務：
Task T015: contracts/elan-command.md
Task T016: contracts/communication-layer.md

# T012+T013 完成後：
Task T014: contracts/data-analysis-dispatch.md （參考 finger + mpp 兩份契約）

# US1+US2 全部完成後：
Task T017: quickstart.md （整合所有模組）
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (research.md + data-model.md)
3. Complete Phase 3: User Story 1 (finger-analysisflow + mpp-analysisflow + data-analysis-dispatch)
4. **STOP and VALIDATE**: 驗證 AI Agent 能列出「新增 AnalysisFlow 的步驟」
5. MVP 已交付 — AnalysisFlow 架構文件可用

### Incremental Delivery

1. Complete Setup + Foundational → 事實基礎就緒
2. Add User Story 1 → 驗證 AnalysisFlow 契約 → MVP 交付
3. Add User Story 2 → 驗證通訊層契約 → 第二增量交付
4. Add User Story 3 → 驗證跨模組比較 → 完整交付
5. Polish → 符號一致性驗證 + 狀態更新

### Suggested MVP Scope

以 User Story 1 (Phase 3) 為 MVP — 涵蓋最頻繁的擴展場景（新增 AnalysisFlow），交付後立即可用。

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- 本 feature 為純文件產出，不修改任何程式碼
- 所有 contracts/ 中的事實必須與 research.md 一致
- Commit after each phase or logical group
- Stop at any checkpoint to validate story independently
