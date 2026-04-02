# Agent Operational Guidelines — AutoTuningTool

> 本文件為 AI Agent（如 GitHub Copilot）操作本專案時的行為指引。

## 專案基本資訊

| 項目 | 內容 |
|------|------|
| 專案名稱 | AutoTuningTool (ELAN 觸控螢幕自動調校工具) |
| 語言/框架 | C# / .NET Framework 4.8 / Windows Forms |
| 平台目標 | x86 (32-bit) |
| 開發環境 | Visual Studio 2019+，Windows 10/11 |
| 主解決方案 | `AutoTuning_NewUI.sln` |
| 版本 | 2.0.0.0-beta1 |

### 專案結構概要

```
AutoTuningTool/
├── AutoTuning_NewUI/          # 主程式入口 (WinExe)
├── FingerAutoTuning/          # 手指觸控調校模組 (Class Library)
├── MPPPenAutoTuning/          # MPP 觸控筆調校模組 (Class Library)
├── packages/                  # NuGet 套件
├── icon/                      # UI 圖示資源
└── runtime/                   # Runtime 套件檔案
```

### 建置組態

| 組態 | 定義符號 | 說明 |
|------|---------|------|
| `Debug\|x86` | `TRACE`, `DEBUG`, `_USE_VC2010` | 偵錯版本 |
| `Release\|x86` | `TRACE`, `_USE_VC2010` | 正式發布版本 |
| `Release_9F07_Socket\|x86` | `TRACE`, `_USE_VC2010`, `_USE_9F07_SOCKET` | DirectTouch 模式（隱藏 MPP Pen） |

---

## 中文回答規則

- 回覆請使用**繁體中文**。
- 保留專業術語、程式碼與技術名詞的英文原文（如 `AnalysisFlow`、`AppCore`、`DataSave`）。
- 舉例時先中文說明，再附英文指令或程式碼。
- 在執行任何動作（讀檔、搜尋、修改、建置等）之前，先以一句話說明預計進行的步驟；完成後再簡述結果。

---

## 回應與交付標準

- 重要輸出請提供受影響檔案路徑與關鍵差異，避免張貼完整大檔。
- 若測試或驗證受限（如缺少硬體裝置），需註明限制並提出可行的驗證方式。
- 修改程式碼時盡量降低影響範圍，非必要請勿大幅重構既有邏輯。
- 新增 method 或功能時須在程式碼中加入中文 `///` 註解，說明用途與關鍵邏輯。
- 結束回報需包含：完成事項、相關檔案路徑、建議下一步。

---

## C# 編碼規範

### 命名慣例
- 類別、方法、屬性：PascalCase（如 `DataAnalysis`、`GetFrameData`）。
- 私有欄位：`m_` 前綴 + camelCase（如 `m_cAppCore`、`m_bConnectFlag`），與專案既有風格一致。
- 常數與列舉值：PascalCase。
- 區域變數與參數：camelCase。

### 檔案命名慣例
本專案已建立的命名模式，新增檔案時應遵循：
- 分析流程：`AnalysisFlow_<名稱>.cs`
- 表單：`frm<名稱>.cs` + `frm<名稱>.Designer.cs` + `frm<名稱>.resx`
- 自訂控制項：`ctrl<名稱>.cs` + `ctrl<名稱>.Designer.cs`
- ELAN 命令集：`ElanCommand.cs` / `ElanCommand_Gen8.cs`
- AppCore 細分模組：`AppCore/<動詞+名詞>.cs`（如 `GetFrameData.cs`、`ConnectAndroidDevice.cs`）
- 流程管理：`ProcessFlow_<模式>.cs`（如 `ProcessFlow_SingleMode.cs`）

### 程式碼修改原則
- **不修改 `*.Designer.cs`**：這些檔案由 Visual Studio Windows Forms Designer 自動產生，手動編輯可能導致設計器無法開啟。
- **條件編譯**：使用 `#if _USE_9F07_SOCKET` 等既有符號進行功能分支，勿自行新增定義符號。
- **NuGet 套件**：修改 `packages.config` 後，需同步更新 `.csproj` 中的參考路徑。還原套件可用 `./restore-packages.ps1`。

---

## 連動更新機制

程式碼變更時務必觸發以下連鎖更新：

1. **新增功能/檔案時**：
   - **必更**：`README.md`（更新專案結構與功能描述）。
   - **必更**：對應 `.csproj` 檔案（確認新檔案已被 Include）。
2. **修改 AnalysisFlow / ProcessFlow 時**：
   - **檢查**：是否需要同步更新 `DataAnalysis.cs` 中的 `MainStep` 列舉或分派邏輯。
   - **檢查**：是否影響 `DataSave.cs` 的檔名生成規則。
3. **修改 ELAN 命令集或通訊層時**：
   - **檢查**：是否需要同步更新 FingerAutoTuning 與 MPPPenAutoTuning 兩個模組中的對應實作。
4. **修改 NuGet 套件版本時**：
   - **必更**：`packages.config` 與 `.csproj` 中的參考路徑。
   - **必更**：`App.config` 中的 `assemblyBinding` 重定向（若有版本變更）。

---

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
| `feat` | 新增功能 | `feat: 新增 MPP Pen 傾角雜訊 Gen8 分析流程` |
| `fix` | 修復 Bug | `fix: 修正 DataSave 在 Self 模式下檔名重複問題` |
| `refactor` | 重構程式碼（不影響功能） | `refactor: 拆分 AppCore 連線模組為獨立檔案` |
| `docs` | 文件變更 | `docs: 更新 README.md 專案結構說明` |
| `style` | 程式碼格式調整（不影響邏輯） | `style: 統一 AnalysisFlow 命名空間宣告` |
| `test` | 測試相關 | `test: 新增 DataSave 檔名格式驗證測試` |
| `chore` | 雜項（建置、工具設定等） | `chore: 更新 EPPlus 套件至 6.2.6` |
| `perf` | 效能優化 | `perf: 優化 FrameMgr 記憶體配置` |

#### 完整 Commit 訊息範例
```
feat: 新增 Raw ADCS 掃描分析流程

變更項目：
- AnalysisFlow_RawADCS.cs: [新增] Raw ADCS 掃描分析流程實作
- DataAnalysis.cs: 新增 RawADCS 分派邏輯至 MainStep 列舉
- FingerAutoTuning.csproj: 加入新檔案參考
- README.md: 更新分析流程列表
```

#### Commit 執行流程
僅輸入 `Git Commit` 時，依序執行：
1. **分析變更**：執行 `git status` 與 `git diff --staged` 檢視所有已暫存的變更。
2. **判定類型**：根據變更性質選擇適當的 type 前綴。
3. **產生訊息**：
   - 第一行：`<type>: <繁體中文摘要>`（50 字元內）
   - 空一行
   - 第三行起：`變更項目：` 後逐條列出修改內容
4. **確認執行**：向使用者展示完整訊息，獲得確認後才執行 `git commit`。

#### 變更項目列表規範
- 每個修改的檔案或模組獨立一行
- 格式：`- <檔案名稱>: <簡述變更內容>`
- 相關檔案可合併描述（如多個 Designer.cs + resx）
- 刪除的檔案標註 `[已刪除]`，新增的檔案標註 `[新增]`

#### 複合變更處理
- 若單次提交包含多種類型變更，以**主要變更**的類型為準。
- 若變更過於複雜，建議拆分為多個 commit。

### Windows 環境操作注意事項
- **中文 Commit 訊息處理 (防亂碼)**：
  - **問題**：在 Windows PowerShell 直接使用 `git commit -m "中文"` 常導致亂碼。
  - **規範**：包含中文或多行訊息時，**嚴禁**使用 `-m` 參數。
  - **程序**：
    1. 建立暫存檔 `.git_commit_msg_temp`（確保 UTF-8 編碼）。
    2. 執行提交：`git commit -F .git_commit_msg_temp`。
    3. 提交後立即刪除暫存檔。

---

## 建置與還原

### NuGet 套件還原
```powershell
# 一鍵還原所有套件（含子專案）
./restore-packages.ps1
```

### 建置指令（命令列）
```powershell
# 使用 MSBuild 建置 Release 版本
msbuild AutoTuning_NewUI.sln /p:Configuration=Release /p:Platform=x86

# 建置 DirectTouch 模式
msbuild AutoTuning_NewUI.sln /p:Configuration=Release_9F07_Socket /p:Platform=x86
```

### 產出路徑
```
AutoTuning_NewUI\bin\Debug\AutoTuning.exe
AutoTuning_NewUI\bin\Release\AutoTuning.exe
AutoTuning_NewUI\bin\Release_9F07_Socket\AutoTuning.exe
```