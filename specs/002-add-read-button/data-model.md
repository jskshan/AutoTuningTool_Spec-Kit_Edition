# Data Model: 新增 btnRead 讀取按鈕至 FingerAutoTuning 主視窗

**Feature**: 002-add-read-button  
**Date**: 2026-04-07

## Entities

### Button: btnRead

此功能不涉及持久化資料模型。唯一的「實體」為 UI 控制項本身。

| 屬性 | 型別 | 值 | 說明 |
|------|------|-----|------|
| Name | string | `"btnRead"` | 控制項名稱，遵循既有 `btn` 前綴慣例 |
| Text | string | `"Read"` | 按鈕顯示文字 |
| Location | Point | `(748, 5)` | 位於現有 4 個主要按鈕列右側 |
| Size | Size | `(74, 68)` | 與既有主要按鈕尺寸一致 |
| FlatStyle | FlatStyle | `Flat` | 與既有按鈕樣式一致 |
| FlatAppearance.BorderSize | int | `0` | 無框線，與既有按鈕一致 |
| ForeColor | Color | `Color.Black` | 文字顏色 |
| Anchor | AnchorStyles | `Top \| Right` | 鎖定右上角，支援表單縮放 |
| Visible | bool | `true` | 初始即可見 (FR-004) |
| Enabled | bool | `true` | 初始即可互動 (FR-004) |

### Event: btnRead_Click

| 屬性 | 說明 |
|------|------|
| 觸發條件 | 使用者點擊 `btnRead` 按鈕 |
| 行為 | 呼叫 `MessageBox.Show("Read")` 顯示模態訊息視窗 |
| 回傳 | `void`，無副作用 |

## Relationships

- `btnRead` → `frmMain.Controls` 集合（父容器為 `splitcontainerRight.Panel1`）
- `btnRead.Click` → `btnRead_Click` 事件處理程序

## State Transitions

此功能無狀態轉換。`btnRead` 不參與 `frmMainEventItem.FlowState` 的按鈕狀態管理流程（Start/Stop/Reset/Pattern 專用），始終保持 `Enabled = true`。

## Validation Rules

- 無輸入驗證需求（按鈕為觸發型控制項，無使用者輸入）
