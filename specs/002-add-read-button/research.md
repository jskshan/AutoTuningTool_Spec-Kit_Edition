# Research: 新增 btnRead 讀取按鈕至 FingerAutoTuning 主視窗

**Feature**: 002-add-read-button  
**Date**: 2026-04-07

## Research Task 1: WinForms 動態新增按鈕的最佳實踐

### 背景

Constitution 規定「不修改 `*.Designer.cs`」，因此需在程式碼中動態建立 `btnRead` 控制項，而非透過 Visual Studio 設計器拖拉方式。

### Decision: 在 `frmMain` 建構子或 Load 事件中，以程式碼方式建立 Button 並加入 Controls 集合

### Rationale

- 完全避免接觸 `frmMain.Designer.cs`，消除設計器損壞風險
- WinForms `Button` 物件可在 `InitializeComponent()` 呼叫後，於建構子尾段或 `Form_Load` 事件中以程式碼建立
- 程式碼方式建立控制項是 WinForms 標準做法，所有屬性（位置、大小、文字、樣式、事件）皆可透過程式碼設定

### Alternatives Considered

| 方案 | 說明 | 排除原因 |
|------|------|---------|
| 透過 VS Designer 新增 | 在設計器中拖拉按鈕 | 會修改 `frmMain.Designer.cs`，違反 Constitution III |
| 使用 UserControl 封裝 | 建立獨立的 UserControl 再嵌入 | 過度工程，僅一個按鈕不值得新增檔案 |

---

## Research Task 2: btnRead 按鈕放置位置

### 背景

現有 4 個主要按鈕排列於控制面板右上角（Y=5），X 範圍 428~742（`btnNewPattern` 結束於 X=742）。需要找到適合 `btnRead` 的位置。

### Decision: 將 `btnRead` 放置在現有 4 個按鈕列的右側，位於 (748, 5)，尺寸保持一致 74×68

### Rationale

- 與現有按鈕列視覺連貫，遵循既有佈局模式
- `btnNewPattern` 結束位置為 668+74=742，留 6px 間距後從 748 開始
- 使用相同的 `Anchor = Top | Right` 確保縮放行為一致
- 控制面板右側的 `splitcontainerRight.Panel1` 寬度為 748px，在 `Anchor = Top | Right` 下空間足夠

### Alternatives Considered

| 方案 | 說明 | 排除原因 |
|------|------|---------|
| 放在控制面板下方 | 獨立一行顯示 | 浪費垂直空間，與主要按鈕風格不一致 |
| 放在左側面板 | 加入左側導航區 | 左側為步驟/資訊顯示區，不適合放操作按鈕 |
| 放在 Tab 結果區 | 加入某個 TabPage | 脫離主要操作區，不符合使用者預期 |

---

## Research Task 3: MessageBox 模態行為

### 背景

Spec 指定訊息視窗需為模態對話框，以防止重複點擊。

### Decision: 使用 `MessageBox.Show("Read")` 實現

### Rationale

- `MessageBox.Show()` 為 WinForms 內建的模態對話框，自動阻擋父視窗操作直到使用者關閉
- 預設含「確定」按鈕，符合 FR-003 關閉方式需求
- 無需額外建立自訂 Form，最簡實作方式
- 模態特性天然防止快速連擊產生多個彈窗（Edge Case 已解決）

### Alternatives Considered

| 方案 | 說明 | 排除原因 |
|------|------|---------|
| 自訂 Form 彈窗 | 建立 `frmReadMessage.cs` | 過度工程，MessageBox 完全滿足需求 |
| 非模態 Dialog | 使用 `Show()` 而非 `ShowDialog()` | 無法阻擋重複點擊，不符合 spec edge case |

---

## Summary

所有 Technical Context 中的項目均已研究完畢，無 NEEDS CLARIFICATION 項目。實作方案為：

1. **建立方式**: 在 `frmMain.cs` 建構子中以程式碼動態建立 `btnRead`
2. **放置位置**: (748, 5)，尺寸 74×68，與現有按鈕列對齊
3. **彈窗方式**: `MessageBox.Show("Read")` 模態對話框
