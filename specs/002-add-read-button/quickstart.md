# Quickstart: 新增 btnRead 讀取按鈕至 FingerAutoTuning 主視窗

**Feature**: 002-add-read-button  
**Date**: 2026-04-07

## 前置條件

- Visual Studio 2019+ 已安裝
- .NET Framework 4.8 SDK 已安裝
- 已切換至 `002-add-read-button` branch

## 建置與執行

### 1. 還原 NuGet 套件

```powershell
./restore-packages.ps1
```

### 2. 建置（三組態驗證）

```powershell
# Debug 版本
msbuild AutoTuning_NewUI.sln /p:Configuration=Debug /p:Platform=x86

# Release 版本
msbuild AutoTuning_NewUI.sln /p:Configuration=Release /p:Platform=x86

# DirectTouch 版本
msbuild AutoTuning_NewUI.sln /p:Configuration=Release_9F07_Socket /p:Platform=x86
```

### 3. 執行驗證

```powershell
# 啟動 Debug 版本
.\AutoTuning_NewUI\bin\Debug\AutoTuning.exe
```

### 4. 驗收步驟

1. 啟動應用程式後，進入 FingerAutoTuning 主視窗
2. 確認主視窗右上角按鈕列末尾出現「Read」按鈕
3. 點擊「Read」按鈕
4. 驗證彈出模態訊息視窗，內容顯示「Read」
5. 點擊「確定」關閉訊息視窗
6. 確認主視窗狀態不受影響，可繼續操作其他按鈕

## 受影響檔案

| 檔案 | 操作 | 說明 |
|------|------|------|
| `FingerAutoTuning/FingerAutoTuning/frmMain.cs` | 修改 | 新增 btnRead 控制項初始化與 Click 事件處理 |
| `README.md` | 修改 | 更新功能描述（按連動更新機制） |
