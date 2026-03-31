# Agent Operational Guidelines

## 中文回答規則

- 回覆請使用繁體中文。
- 保留專業術語、程式碼與技術名詞的英文原文。
- 舉例時先中文說明，再附英文指令或程式碼。
- 在執行任何動作（讀檔、搜尋、修改、測試等）之前，先以一句話說明預計進行的步驟；完成後再簡述結果。

## 文件導向流程

### 目錄結構規範 (通用化分類)

為解決文件混亂，請依據 **「讀者視角」** 與 **「抽象層級」** 進行分類：

- **`docs/README.md`**: **[地圖]** 專案全域索引與詳細目錄結構說明（必須隨時保持最新）。
- **`docs/specs/`**: **[需求面] 做什麼 (What)**
  - 定義：功能規格、User Stories、開發任務清單。
  - *判斷：在寫任何程式碼之前，就能先寫好的內容。*
- **`docs/system/`**: **[架構面] 系統全景 (System Overview)**
  - 定義：系統架構圖、資料流向、專案結構說明、技術選型決策。
  - *判斷：跨越單一檔案，描述模組間互動關係或全域概念的內容。*
- **`docs/modules/`**: **[實作面] 模組細節 (Implementation Details)**
  - 定義：特定子系統實作原理、API 介面定義、演算法說明、資料庫 Schema。
  - *判斷：針對特定 `.py` 檔、類別或函數的深入技術解說。*
- **`docs/manuals/`**: **[使用面] 操作手冊 (User Manuals)**
  - 定義：安裝教學、環境設定、CLI 指令參考、參數調整指南。
  - *判斷：寫給終端使用者或測試人員看的「如何執行」說明。*
- **`docs/reference/`**: **[參考面] 外部資料 (References)**
  - 定義：論文、調研筆記、競品分析、實驗數據。

### 連動更新機制 (Chain Reaction)

文件不是孤島，程式碼變更時務必觸發以下連鎖更新：

1.  **新增功能/檔案時**：
    - **必更**: `docs/README.md` (更新目錄索引)。
    - **必更**: `docs/system/project_structure.md` (若有) (更新專案結構樹)。
    - **檢查**: 是否需要新增 `specs/` 下的功能定義？
2.  **修改核心邏輯時**：
    - **必更**: `docs/modules/` 下對應的技術文件 (如 API 變更、演算法調整)。
    - **檢查**: `docs/manuals/` 中的操作參數是否改變？
3.  **架構重構時**：
    - **必更**: `docs/system/` 下的架構圖與資料流說明。

### 子專案文件管理 (Sub-projects)

針對獨立性較高的子專案（如 `standalone_metrics_calculator`），採用 **聯邦式管理 (Federated Strategy)**：

- **自主結構**: 子專案應擁有獨立的 `docs/` 目錄，且內部結構應盡量遵循主專案規範（`specs`, `modules`, `manuals`...），以降低切換成本。
- **索引掛載**: 主專案的 `docs/README.md` 必須建立「子專案專區」，提供指向子專案文件的連結（Mount Points）。
- **對齊原則**:
  - 若子專案實作與主專案相同的邏輯（如 Metrics），**兩者文件內容應保持邏輯一致**。
  - 但**不強制**實體檔案同步（不需 Single Source of Truth），允許子專案針對自身實作細節維護獨立文件。

### 閱讀與寫作原則

- **先讀後做**: 進入任務前，優先檢索 `docs/` 確認既有規範。
- **原子化更新**: 盡量針對單一主題建立或更新文件，避免建立內容雜亂的巨型文件。
- **通用性優先**: 撰寫技術文件時，盡量將實作細節與設計理念分開，使設計理念可被複用。

## 回應與交付標準

- 重要輸出請提供受影響檔案路徑與關鍵差異，避免張貼完整大檔。
- 若測試或驗證受限，需註明限制並提出可行的驗證方式。
- 修改程式碼時盡量降低影響範圍，非必要請勿大幅重構既有邏輯。
- 新增 function 或功能時須在程式碼中加入中文註解，說明用途與關鍵邏輯。
- 結束回報需包含完成事項、相關檔案與建議下一步。

## LLM_tasks 任務管理規範

### 任務使用方式

- 主要目錄用途：`current_tasks`（進行中）、`pending_tasks`（待確認）、`paused_tasks`（暫停）、`completed_tasks`（已完成）、`archived_tasks`（歷史）、`reference_docs`（參考資料）、`implementation_records`（實作紀錄）。
- 新建任務檔案使用命名格式 `task_MMDD_序號_短描述.md`（例如 `task_0905_01_tasklab_callback_implementation.md`），序號依當日任務順序遞增。
- 任務檔案建議欄位：`Task ID`、`Title`、`Priority`、`Status`、`Owner`、`Created`、`Due`、`Description`、`Deliverables`、`Validation Steps`、`Dependencies`、`Notes`。
- 任務狀態流程：建立任務後放入 `current_tasks`；若待補資訊，轉至 `pending_tasks` 並註記需求；如遇阻礙，轉至 `paused_tasks` 並描述原因；完成驗證後移至 `completed_tasks`，在檔案底部新增 Completion Log（日期、執行者、備註）；定期將無後續需求的任務移至 `archived_tasks`。
- 建議建立腳本協助快速產生任務模板、檢查重複 `Task ID`、統計各資料夾任務數量，以利 LLM 自動化引用。

### 臨時測試檔案規範 (LLM_test)

#### 強制執行流程
1. **產生任何 tmp_*.py 檔案前**，必須先檢查 `LLM_test/tmp/` 是否有舊檔案需清理。
2. **測試檔案必須存放於 `LLM_test/tmp/`**，禁止放在專案根目錄。
3. **測試完成立即處理**：
  - 有價值 → 整合至正式測試 → 刪除 tmp 檔
  - 無價值 → 直接刪除
  - 需保留 → 移至 `LLM_test/archive/YYYY-MM/` + 說明原因

#### 命名規範
- 格式: `tmp_YYMMDD_功能描述.py`
- 範例: `tmp_251020_excel_query_test.py`

#### 檢查點
- 每次產生 tmp 檔案時，先報告 `LLM_test/tmp/` 當前檔案數量與最舊檔案日期
- 若 tmp/ 超過 10 個檔案，必須先清理才能繼續
- 測試完成後必須明確告知使用者該檔案的處理方式（整合/刪除/歸檔）

### 文件整理與實作紀錄

#### 實作紀錄管理
- 任務完成後，將實作總結與相關 bugfix 文件統一存放在 `LLM_tasks/implementation_records/` 下。
- 目錄結構：`implementation_records/YYYY-MM/MMDD_feature_name/`（例如 `2025-10/1013_statistics_dashboard/`）。
- 每個實作紀錄目錄包含：
  - `IMPLEMENTATION_SUMMARY.md` - 實作總結（固定命名）
  - `BUGFIX_YYMMDD_NN.md` - 相關 bug 修復紀錄（例如 `BUGFIX_251013_01.md`）
  - 相關測試報告、截圖或其他支援文件
- 歷史專案總結文件（如 `PROJECT_COMPLETION_SUMMARY.md`、`STAGE_X_SUMMARY.md` 等）移至 `implementation_records/archive/` 保存。
- 完成實作紀錄後，需更新 `implementation_records/README.md` 索引，方便查找。

#### 文件命名規範
| 文件類型 | 命名格式 | 範例  |
| --- | --- | --- |
| 任務規劃 | `task_MMDD_NN_description.md` | `task_1013_02_statistics_dashboard_enhancement.md` |
| 實作總結 | `IMPLEMENTATION_SUMMARY.md` | （固定名稱，置於實作紀錄目錄） |
| Bug 修復 | `BUGFIX_YYMMDD_NN.md` | `BUGFIX_251013_01.md` |
| 階段總結 | `STAGE_N_description.md` | `STAGE_6_7_COMPLETION_SUMMARY.md` |
| 專案總結 | `PROJECT_*.md` | `PROJECT_COMPLETION_SUMMARY.md` |

#### 標準任務完成流程
1. **任務規劃**: 建立 `LLM_tasks/current_tasks/task_MMDD_NN_feature_name.md`
2. **開發過程**: 編寫程式碼、測試驗證、紀錄 bug
3. **任務完成**:
  - 移動任務文件到 `completed_tasks/`
  - 建立實作紀錄目錄: `implementation_records/YYYY-MM/MMDD_feature_name/`
  - 產生 `IMPLEMENTATION_SUMMARY.md` 與相關 `BUGFIX_*.md`
  - 更新 `implementation_records/README.md` 索引
4. **文件維護**:
  - 如涉及新功能，更新 `docs/features/` 相關文件
  - 如涉及 API 變更，更新 `docs/architecture/api/` 相關文件
  - 必要時更新 `README.md` 或 `QUICK_START.md`

#### 檔案歸檔與清理原則
- **即時整理**: 任務確認完成後，Agent 應立即將任務檔移至 `completed_tasks/`，避免 `current_tasks/` 堆積。
- **按需歸檔**: 當 `completed_tasks/` 或 `implementation_records/` 檔案數量過多（如超過 20 個）導致檢索困難時，Agent 可主動詢問使用者是否將舊檔案移至 `archived_tasks/` 或 `archive/` 目錄。
- **專注當下**: 除非使用者要求，否則不需執行特定時間點（如每月底）的例行維護，將重點放在保持當前工作目錄的整潔。

## Git 工作流與規範

### Commit 訊息格式 (Conventional Commits)

採用 **Conventional Commits** 規範，格式為：
```
<type>: <繁體中文摘要>

變更項目：
- <檔案或模組>: <變更說明>
- <檔案或模組>: <變更說明>
```

#### 類型前綴 (Type Prefix)
| 類型 | 用途 | 範例 |
|------|------|------|
| `feat` | 新增功能 | `feat: 新增使用者登入驗證功能` |
| `fix` | 修復 Bug | `fix: 修正資料匯出時的編碼問題` |
| `refactor` | 重構程式碼（不影響功能） | `refactor: 重構資料處理模組結構` |
| `docs` | 文件變更 | `docs: 更新 API 使用說明` |
| `style` | 程式碼格式調整（不影響邏輯） | `style: 統一縮排與命名風格` |
| `test` | 測試相關 | `test: 新增單元測試案例` |
| `chore` | 雜項（建置、工具設定等） | `chore: 更新依賴套件版本` |
| `perf` | 效能優化 | `perf: 優化資料查詢效率` |

#### 完整 Commit 訊息範例
```
feat: 新增觸控面板雜訊分析功能

變更項目：
- noise_analyzer.py: 新增 NoiseAnalyzer 類別與頻譜分析方法
- test_noise.py: 新增對應單元測試
- docs/modules/noise_analysis.md: 新增模組技術文件
- requirements.txt: 新增 scipy 依賴
```
```
fix: 修正 Excel 匯出中文亂碼問題

變更項目：
- excel_exporter.py: 修正編碼設定為 UTF-8-BOM
- utils/encoding.py: 新增編碼檢測輔助函式
```

#### Commit 執行流程
僅輸入 `Git Commit` 時，依序執行：
1. **分析變更**: 執行 `git status` 與 `git diff --staged` 檢視所有已暫存的變更。
2. **判定類型**: 根據變更性質選擇適當的 type 前綴。
3. **產生訊息**: 
   - 第一行：`<type>: <繁體中文摘要>`（50 字元內）
   - 空一行
   - 第三行起：`變更項目：` 後逐條列出修改內容
4. **確認執行**: 向使用者展示完整訊息，獲得確認後才執行 `git commit`。

#### 變更項目列表規範
- 每個修改的檔案或模組獨立一行
- 格式：`- <檔案名稱>: <簡述變更內容>`
- 相關檔案可合併描述（如多個測試檔案）
- 刪除的檔案標註 `[已刪除]`，新增的檔案標註 `[新增]`（選用）

#### 複合變更處理
- 若單次提交包含多種類型變更，以**主要變更**的類型為準。
- 若變更過於複雜，建議拆分為多個 commit。

### Windows 環境操作注意事項
- **中文 Commit 訊息處理 (防亂碼)**:
  - **問題**: 在 Windows PowerShell 直接使用 `git commit -m "中文"` 常導致亂碼。
  - **規範**: 包含中文或多行訊息時，**嚴禁**使用 `-m` 參數。
  - **程序**:
    1. 使用 `write_file` 建立暫存檔 `.git_commit_msg_temp`（確保 UTF-8 編碼）。
    2. 執行提交: `git commit -F .git_commit_msg_temp`。
    3. 提交後立即刪除暫存檔。

## 專案健康度檢查 (響應式)

- **觸發時機**: 僅在使用者明確下達「定期檢查」或類似指令時執行。
- **執行內容**:
  1. **文件一致性**: 檢查 `docs/` 與程式碼的同步狀態（路徑、API、配置）。
  2. **任務狀態盤點**:
     - 掃描 `current_tasks` 與 `paused_tasks`。
     - **智能狀態偵測**: 比對任務描述與近期 **程式碼變更** 及 **`docs/` 文件更新**（Git Log 或檔案修改時間）。若發現任務相關功能已實作或文件已撰寫/更新，**自動將其移至** `completed_tasks`。
  3. **根目錄整潔化**:
     - **腳本清理**: 
       - 分析根目錄下 `test_*.py` 與 `tmp_*.py`。
       - **刪除**: 判定為臨時性、實驗性或無長期價值的腳本（如極短程式碼、單純 print 測試）。
       - **歸檔**: 具備完整測試結構或功能的腳本，依性質移至 `tests/` 或 `LLM_test/tmp/`。
     - **文件歸檔 (智慧整合)**: 
       - 處理根目錄下的 `*.md`（排除 Agent 設定檔與 README.md）。
       - **整合**: 若內容屬於現有文件的更新或補充，將其**合併**至 `docs/` 對應文件中，並刪除原檔。
       - **移動**: 若為全新獨立文件，則移至 `docs/` 下適當分類目錄。
  4. **報告產出**: 彙整上述文件異常、任務移動紀錄與檔案整理結果；若無異常則回報「系統健康」。