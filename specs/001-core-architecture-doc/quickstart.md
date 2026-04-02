# Quickstart: 核心模組開發速查手冊

**Feature**: `001-core-architecture-doc`

> 開發者快速參考指南 — 新增或修改觸控調校分析流程時的步驟清單與注意事項。

---

## 1. 新增 FingerAutoTuning AnalysisFlow

**目標**: 新增一個手指觸控分析流程子類。

### Step-by-Step

| # | 動作 | 檔案 | 說明 |
|---|------|------|------|
| 1 | 新增子類檔案 | `FingerAutoTuning/AnalysisFlow_<Name>.cs` | 繼承 `AnalysisFlow`，覆寫 `InitializeParameter()`、`InitializeSourceDataList()`、`LoadAnalysisParameter()`、`MainFlow()` |
| 2 | 新增 MainStep 值 | `frmMain.cs` → `MainStep` 列舉 | 在 `Else = 7` 之前插入新值 |
| 3 | 新增分派邏輯 | `DataAnalysis.cs` → `ExecuteMainWorkFlow()` | 兩段式：第一段 if-else 建構實例，第二段 if-else 向下轉型並呼叫 |
| 4 | 加入 csproj | `FingerAutoTuning.csproj` | `<Compile Include="AnalysisFlow_<Name>.cs" />` |
| 5 | 更新文件 | `README.md` | 更新分析流程列表 |

### 程式碼範本

```csharp
namespace FingerAutoTuning
{
    /// <summary>
    /// <Name> 分析流程 — [用途說明]
    /// </summary>
    public class AnalysisFlow_<Name> : AnalysisFlow
    {
        public AnalysisFlow_<Name>(
            frmMain.FlowStep cFlowStep,
            string sLogDirectoryPath,
            string sH5LogDirectoryPath,
            bool bGenerateH5Data,
            frmMain cfrmParent,
            string sProjectName)
            : base(cFlowStep, sLogDirectoryPath, sH5LogDirectoryPath,
                   bGenerateH5Data, cfrmParent, sProjectName)
        { }

        public override void InitializeParameter() { /* 初始化參數 */ }
        public override void InitializeSourceDataList() { /* 設定資料來源 */ }
        public override void LoadAnalysisParameter() { /* 載入分析參數 */ }
        public override bool MainFlow(ref string sErrorMessage) { /* 主流程 */ return true; }
    }
}
```

---

## 2. 新增 MPPPenAutoTuning AnalysisFlow

**目標**: 新增一個 MPP 觸控筆分析流程子類。

### Step-by-Step

| # | 動作 | 檔案 | 說明 |
|---|------|------|------|
| 1 | 新增子類檔案 | `MPPPenAutoTuning/AnalysisFlow/AnalysisFlow_<Name>.cs` | 繼承 `AnalysisFlow`，覆寫 `LoadAnalysisParameter()`，實作 convention 方法 |
| 2 | 新增步驟列舉值 | `ParameterProperties.cs` → `MainTuningStep` 或 `SubTuningStep` | 視分類需求決定 |
| 3 | 新增分派邏輯 | `DataAnalysis.cs` → `ExecuteMainWorkFlow()` | 三維判斷：MainTuningStep → SubTuningStep → nICSolutionType |
| 4 | 加入 csproj | `MPPPenAutoTuning.csproj` | `<Compile Include="AnalysisFlow\AnalysisFlow_<Name>.cs" />` |

### Convention 方法清單

```csharp
public override void LoadAnalysisParameter() { }  // 唯一 virtual 覆寫
public void SetFileDirectory() { }                 // 設定檔案目錄
public bool CheckDirectoryIsValid() { }            // 驗證目錄
public void GetData() { }                          // 取得資料
public void ComputeAndOutputResult() { }           // 計算並輸出結果
```

---

## 3. 新增 ElanCommand

**目標**: 為 IC 新增一個讀寫命令。

### Step-by-Step

| # | 動作 | 檔案 | 說明 |
|---|------|------|------|
| 1 | 新增列舉值 | `ElanCommand.cs` → `ElanCommandType` | 放在對應分類區段（無參/單參/雙參） |
| 2 | 新增映射 | `ElanCommand.cs` → `dictSetCommandMappingTable` + `dictGetCommandMappingTable` | 若需要 ICValueTargetType，同時新增 |
| 3 | 更新轉換 | `ElanCommand.cs` → `ConvertCommandToByte()` + `ConvertToICGetValue()` | 新增 byte 編碼/解碼 case |
| 4 | 雙模組同步 | `FingerAutoTuning/Class/ElanCommand.cs` + `MPPPenAutoTuning/Class/ElanCommand.cs` | 確認是否需同步 |

### Gen8 命令

若需支援 Gen8 IC：

| # | 動作 | 檔案 |
|---|------|------|
| 1 | 新增 ParameterType 值 | `ElanCommand_Gen8.cs` → `ParameterType` 列舉 |
| 2 | 建立 WriteCommandInfo 初始化 | `ElanCommand_Gen8.cs` → 對應的初始化方法 |
| 3 | 設定 DataType + ParameterClass | 使用既有常數或新增 |

---

## 4. 修改通訊層

### ElanTouch 修改 Checklist

- [ ] 確認修改影響範圍（HID / I2C / SPI）
- [ ] 若修改 TraceInfo，檢查所有 `GetRXTraceNum()` / `GetTXTraceNum()` 呼叫端
- [ ] 若新增介面類型，更新 `TP_INTERFACE_TYPE` 列舉
- [ ] 確認 `LibTouch.dll` native DLL 是否需要配套更新
- [ ] 雙模組同步：FingerAutoTuning + MPPPenAutoTuning 各有獨立的 `ElanTouch.cs`

### Socket 模式 Checklist

- [ ] `ElanTouch_Socket.cs` — 使用 `LibTouch_Socket.dll`
- [ ] 受條件編譯控制：`#if _USE_9F07_SOCKET`
- [ ] 建置組態：`Release_9F07_Socket|x86`

---

## 5. 連動更新總覽

每次程式碼變更後，依據以下矩陣檢查連動更新需求：

| 變更範圍 | 必查項目 |
|----------|----------|
| 新增 AnalysisFlow | MainStep/SubTuningStep 列舉、DataAnalysis 分派、csproj、README |
| 修改 AnalysisFlow | DataSave 檔名規則是否受影響 |
| 新增 ElanCommand | 雙模組同步、ICValueTargetType 映射 |
| 修改 ElanTouch | 雙模組同步、所有 TraceInfo 消費端 |
| 修改 ProcessFlow | 共用函式 (`ProcessFlow_CommonFunction.cs`) 是否受影響 |
| 修改 NuGet 套件 | `packages.config` + `.csproj` 參考路徑 + `App.config` assemblyBinding |
| 修改 AppCore | 確認 partial class 各檔案間的影響 |

---

## 6. 常見問題

### Q: FingerAutoTuning 和 MPPPenAutoTuning 的 AnalysisFlow 差異是什麼？

| 項目 | FingerAutoTuning | MPPPenAutoTuning |
|------|------------------|------------------|
| Virtual 方法 | 4 個 (`InitializeParameter`, `InitializeSourceDataList`, `LoadAnalysisParameter`, `MainFlow`) | 1 個 (`LoadAnalysisParameter`) |
| 分派維度 | 1 (`MainStep`) | 3 (`MainTuningStep` × `SubTuningStep` × `nICSolutionType`) |
| 方法呼叫 | 透過 virtual override | 混合 virtual + convention-based |
| 子類數量 | 6 | 17 |

### Q: Gen8 和非 Gen8 命令如何切換？

透過 `nICSolutionType` 判斷：
- `== MainConstantParameter.m_nICSOLUTIONTYPE_GEN8` → 使用 `ElanCommand_Gen8`
- 其他 → 使用 `ElanCommand`

### Q: 條件編譯符號有哪些？

| 符號 | 組態 | 效果 |
|------|------|------|
| `DEBUG` | Debug\|x86 | 偵錯模式 |
| `_USE_VC2010` | 所有組態 | VC++ 2010 相容模式 |
| `_USE_9F07_SOCKET` | Release_9F07_Socket\|x86 | DirectTouch/Socket 模式（隱藏 MPP Pen tab） |
