# Tasks: 新增 btnRead 讀取按鈕至 FingerAutoTuning 主視窗

**Feature Branch**: `002-add-read-button`  
**Design Documents**: spec.md ✅ | plan.md ✅ | research.md ✅ | data-model.md ✅  
**Tech Stack**: C# / .NET Framework 4.8 / WinForms / x86  
**Platform**: Windows 10/11 (無硬體依賴，全部 task 可在無裝置環境下驗收)

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no incomplete dependencies)
- **[Story]**: User story label (US1) — only in User Story phases
- 所有路徑均為相對於 repo root 的路徑

---

## Phase 1: Setup

**Purpose**: 確認基線建置成功，保障後續修改品質

- [ ] T001 在開發環境執行 `msbuild AutoTuning_NewUI.sln /p:Configuration=Debug /p:Platform=x86` 驗證三組態（Debug\|x86、Release\|x86、Release_9F07_Socket\|x86）基線建置全數通過，確認無既有錯誤

**Checkpoint**: 三組態建置通過 → 可進入 US1 實作

---

## Phase 2: User Story 1 — 點擊 Read 按鈕顯示訊息視窗 (Priority: P1) 🎯 MVP

**Goal**: 在 `frmMain` 主視窗按鈕列末尾新增可見且可互動的 `btnRead` 按鈕，點擊後以模態 `MessageBox` 顯示「Read」文字，關閉訊息視窗後主視窗功能不受影響。

**Independent Test**: 啟動 FingerAutoTuning 主視窗 → 確認 `btnNewPattern` 右側出現「Read」按鈕 → 點擊按鈕 → 確認模態訊息視窗顯示「Read」文字 → 關閉視窗 → 確認主視窗所有按鈕仍可正常操作

### Implementation for User Story 1

- [ ] T002 [US1] 在 `FingerAutoTuning/FingerAutoTuning/frmMain.cs` 類別層級宣告 `private Button btnRead;` 私有欄位，並在建構子 `InitializeComponent()` 呼叫後以程式碼初始化控制項所有屬性（Name="btnRead", Text="Read", Location=new Point(748,5), Size=new Size(74,68), FlatStyle=FlatStyle.Flat, FlatAppearance.BorderSize=0, ForeColor=Color.Black, Anchor=AnchorStyles.Top\|AnchorStyles.Right, Visible=true, Enabled=true），將其加入 `splitcontainerRight.Panel1.Controls`，並綁定 `btnRead.Click += new EventHandler(this.btnRead_Click)`
- [ ] T003 [US1] 在 `FingerAutoTuning/FingerAutoTuning/frmMain.cs` 實作 `private void btnRead_Click(object sender, EventArgs e)` 事件處理方法，方法內容為 `MessageBox.Show("Read")`；並在方法宣告上方加入中文 `///` XML 文件註解，說明「點擊 btnRead 按鈕時觸發，彈出顯示 Read 文字的模態訊息視窗」

**Checkpoint**: `btnRead` 按鈕顯示於主視窗，點擊後正確彈出模態訊息視窗顯示「Read」，User Story 1 驗收場景 1~3 全數通過

---

## Phase 3: Polish & Cross-Cutting Concerns

**Purpose**: Constitution 連動更新機制 + 三組態最終建置驗證（SDD 品質 Gate）

- [ ] T004 [P] 更新 `README.md`，於 FingerAutoTuning 主視窗功能描述中新增 `btnRead` 按鈕說明（遵循 Constitution IV. 連動更新機制：新增功能必更 README.md）
- [ ] T005 執行三組態建置驗證：`msbuild AutoTuning_NewUI.sln /p:Configuration=Debug /p:Platform=x86`、`/p:Configuration=Release /p:Platform=x86`、`/p:Configuration=Release_9F07_Socket /p:Platform=x86`，確認全數通過零錯誤（Constitution SDD 品質 Gate）

---

## Dependencies

```
T001 (基線建置驗證)
  └── T002 [US1] (btnRead 欄位宣告 + 控制項初始化 + 事件綁定)
        └── T003 [US1] (btnRead_Click 事件處理實作)
                ├── T004 [P]  更新 README.md（不同檔案，可與 T005 並行）
                └── T005      三組態最終建置驗證
```

## Parallel Execution

T003 完成後，T004 與 T005 可並行執行（操作不同檔案/資源）：

```
T003 完成
  ├── [執行者 A] T004 — 更新 README.md
  └── [執行者 B] T005 — 三組態建置驗證
```

## Implementation Strategy

**MVP Scope**: 僅需完成 T001 → T002 → T003，即可交付完整可驗收的 MVP

| Task | 檔案 | 預估複雜度 | 硬體依賴 |
|------|------|-----------|---------|
| T001 | `AutoTuning_NewUI.sln` | 低（執行建置） | 不需要 |
| T002 | `FingerAutoTuning/FingerAutoTuning/frmMain.cs` | 低（約 15 行程式碼） | 不需要 |
| T003 | `FingerAutoTuning/FingerAutoTuning/frmMain.cs` | 低（約 5 行程式碼含註解） | 不需要 |
| T004 | `README.md` | 低（文件更新） | 不需要 |
| T005 | `AutoTuning_NewUI.sln` | 低（執行建置） | 不需要 |

**建議執行順序**: T001 → T002 → T003 → T004 ∥ T005
