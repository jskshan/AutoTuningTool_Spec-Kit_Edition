# Specification Analysis Report

**Feature**: `001-core-architecture-doc` | **Date**: 2026-04-02  
**Artifacts**: spec.md (Approved), plan.md, tasks.md (21 tasks)

---

## Findings

| ID | Category | Severity | Location(s) | Summary | Recommendation |
|----|----------|----------|-------------|---------|----------------|
| F1 | Inconsistency | HIGH | spec.md FR-001, research.md §1.2 | FR-001 將 `GetDataCount()` 列入「所有虛擬方法簽名」，但程式碼中 `GetDataCount()` 為 `public bool`（非虛方法），缺少 `virtual` 修飾 | 修正 FR-001：將 `GetDataCount()` 從虛擬方法列表中移出，改為獨立的「非虛方法」說明 |
| F2 | Inconsistency | HIGH | tasks.md Phase 4 header, Phase 5 header | Phase 4 header 寫「覆蓋 FR: FR-009~FR-012」，但 T016 實際實作 FR-015/FR-016（IHardware/IRS232Device）；Phase 5 header 宣稱覆蓋 FR-015/FR-016 但無對應實作任務 | 將 FR-015/FR-016 從 Phase 5 覆蓋清單移至 Phase 4，Phase 5 改為「FR-013, FR-014」 |
| G1 | Coverage | MEDIUM | spec.md FR-013/FR-014, contracts/ | DataSave 檔名生成規則（FR-013: 報表檔名、FR-014: CSV Frame 檔名）無專屬 contract 文件。相關內容僅分散在 research.md §4 與 quickstart.md 連動更新表中 | 新增 `contracts/data-save.md` 專屬契約，或在 `data-model.md` §4 中擴充完整檔名模板（Mutual/Self/RawADCS） |
| F3 | Inconsistency | MEDIUM | tasks.md Phase 5 US3 vs spec.md US3 | US3 主題為「AI Agent 理解雙模組架構差異」，但 Phase 5 將 FR-013/FR-014（DataSave 檔名規則）指派給 US3，與模組比較主題無直接關聯 | 將 FR-013/FR-014 的覆蓋責任移至 US2（或 Phase 2 Foundational），Phase 5 專注於跨模組整合 |
| U1 | Underspecification | MEDIUM | data-model.md §4, spec.md Key Entities | `SaveDataInfo` 被列為 Key Entity，但 data-model.md 僅展示 `CreateRecordData()` 方法簽名，未列出 `SaveDataInfo` 的欄位定義 | 在 data-model.md §4 補充 SaveDataInfo 欄位清單，或加註「欄位細節以程式碼為準，不在文件範圍」 |
| F4 | Inconsistency | MEDIUM | plan.md vs tasks.md | plan.md 使用「Phase A (Steps 1-4) + Phase B (Step 5)」結構，tasks.md 重組為「Phase 1-6」user-story-aligned 結構，兩者無對照關係說明 | 在 tasks.md 開頭加入 Plan↔Tasks 階段對照表（Phase A = Tasks Phase 1+2+3+4+5, Phase B = Tasks Phase 6） |
| A1 | Ambiguity | LOW | spec.md Success Criteria | SC 編號從 SC-001 跳至 SC-003（缺 SC-002），暗示曾有合併或刪除但未留下紀錄 | 加註「SC-002 已合併至 SC-001」或重新連續編號 |
| D1 | Terminology | LOW → ✅ RESOLVED | constitution.md §II, spec.md FR-004 | Constitution 寫「16+ 分析流程」，經 SC-003 驗證後 spec.md 已修正為 16 個，與「16+」精確匹配 | 無需修正 — 「16+」與實際 16 個子類完全相容 |
| F5 | Inconsistency | LOW | plan.md Step 2, tasks.md Phase 3+4 | plan.md 將所有 contracts/ 視為單一「Step 2」；tasks.md 拆分為 Phase 3 (US1) + Phase 4 (US2) | 可接受的 user-story-aligned 細化，無需修正 |

---

## Coverage Summary Table

| Requirement Key | Has Task? | Task IDs | Notes |
|-----------------|-----------|----------|-------|
| FR-001 | ✅ | T003, T012 | |
| FR-002 | ✅ | T003, T012 | |
| FR-003 | ✅ | T010, T013 | |
| FR-004 | ✅ | T010, T013 | |
| FR-005 | ✅ | T004, T014 | |
| FR-006 | ✅ | T004, T014 | |
| FR-007 | ✅ | T010, T014 | |
| FR-008 | ✅ | T010, T014 | |
| FR-009 | ✅ | T007, T015 | |
| FR-010 | ✅ | T007, T015 | |
| FR-011 | ✅ | T008, T015 | |
| FR-012 | ✅ | T009, T016 | |
| FR-013 | ⚠️ | T006, T017 | 無 contract 文件覆蓋（僅 research + quickstart 提及） |
| FR-014 | ⚠️ | T006, T017 | 無 contract 文件覆蓋（僅 research + quickstart 提及） |
| FR-015 | ✅ | T016 | 實際在 US2/T016 實作，但 tasks.md 歸屬標頭錯誤 (F2) |
| FR-016 | ✅ | T016 | 同上 |
| FR-017 | ✅ | T005, T014 | |
| SC-001 | — | — | 後置成果指標，不需可構建任務 |
| SC-003 | ✅ | T020 | grep 驗證 |
| SC-004 | ✅ | T021 | 快速參考測試 |

---

## Constitution Alignment Issues

**無 CRITICAL 違規。** 所有 Constitution MUST 原則均已遵循：

| 原則 | 狀態 | 備註 |
|------|------|------|
| I. 技術棧約束 | ✅ | 本 feature 不修改程式碼 |
| II. 模組化架構 | ✅ | 兩模組均有覆蓋；D1 為 LOW 術語差異 |
| III. 最小影響 | ✅ | 僅新增文件 |
| IV. 連動更新 | ✅ | T019 更新 README.md |
| V. 硬體依賴 | ✅ | 所有驗證為自動驗證 |

---

## Unmapped Tasks

所有 21 個任務均有明確對應：

| Task IDs | 歸屬 |
|----------|------|
| T001, T002 | Setup infrastructure（不需 FR 對應） |
| T003-T011 | Foundational（事實提取，供 FR 覆蓋） |
| T012-T014 | US1（FR-001~FR-008, FR-017） |
| T015-T016 | US2（FR-009~FR-012, FR-015, FR-016） |
| T017 | US3（FR-013, FR-014 + 跨模組整合） |
| T018-T019 | Constitution IV 連動更新 |
| T020-T021 | SC-003, SC-004 驗證 |

---

## Metrics

| 指標 | 數值 |
|------|------|
| Total Requirements (FR) | 17 |
| Total Success Criteria (SC) | 3（SC-001 為後置指標，已排除） |
| Total Tasks | 21 |
| FR Coverage % | **100%**（17/17 FR 均有 ≥1 task） |
| Contract Coverage % | **88%**（15/17 FR 有 contract 覆蓋；FR-013/FR-014 缺少） |
| Ambiguity Count | 1 (LOW) |
| Duplication Count | 0 |
| Critical Issues Count | **0** |
| HIGH Issues | **2** |
| MEDIUM Issues | **4** |
| LOW Issues | **2** (A1, F5；D1 已解決) |

---

## Next Actions

### 已修正項目

| ID | Severity | 修正內容 | 檔案 |
|----|----------|----------|------|
| F1 | HIGH | FR-001 將 `GetDataCount()` 從虛擬方法列表移出，改為「非虛方法」獨立說明 | spec.md |
| F2 | HIGH | Phase 4 覆蓋 FR 加入 FR-013~FR-016；Phase 5 改為「跨 US1/US2 整合」 | tasks.md |
| G1 | MEDIUM | data-model.md §4 擴充 DataSave 檔名模板（Mutual/Self/RawADCS） | data-model.md |
| F3 | MEDIUM | FR-013/FR-014 覆蓋責任移至 US2/T016b；Phase 5 不再承接新 FR | tasks.md |
| U1 | MEDIUM | data-model.md §4 補充 SaveDataInfo 完整欄位定義（3 組：共用/Mutual/RawADCS/Self） | data-model.md |
| F4 | MEDIUM | tasks.md 加入 Plan↔Tasks 階段對照表 | tasks.md |

### 未修正項目（LOW，非阻塞）

7. **A1** — SC 編號跳號（SC-002 缺失），可加註或重新排序
8. **D1** — ✅ 已自動解決：SC-003 驗證期間發現 MPP AnalysisFlow 實際為 16 個子類，spec.md 已修正為 16，與 Constitution「16+」精確匹配
9. **F5** — plan.md vs tasks.md 階段粒度差異（已透過 F4 對照表緩解）

---

## Remediation Offer

所有 HIGH 與 MEDIUM 問題已修正完成。剩餘 3 個 LOW 問題為非阻塞性質，可於後續維護時處理。
