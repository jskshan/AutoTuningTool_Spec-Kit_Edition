# Contract: DataAnalysis 分派層 — 雙模組對照

**FR**: FR-005, FR-006, FR-007, FR-008, FR-017

---

## FingerAutoTuning — 單維分派

### DataAnalysis.ExecuteMainWorkFlow()

```csharp
// DataAnalysis.cs Line 17-19
public bool ExecuteMainWorkFlow(
    ref string m_sErrorMessage,      // 錯誤訊息（傳址）
    frmMain.FlowStep cFlowStep,      // 流程步驟（含 MainStep）
    string sLogDirectoryPath,         // 日誌目錄
    string sH5LogDirectoryPath,       // H5 資料目錄
    bool bGenerateH5Data,             // 是否生成 H5
    frmMain cfrmParent,               // 父表單
    string sProjectName,              // 專案名稱
    string sSkipFreqSetFilePath)      // 跳過頻率設定檔
```

### MainStep 列舉 ↔ AnalysisFlow 分派表

| MainStep | 值 | AnalysisFlow 子類 | 特殊處理 |
|----------|-----|-------------------|---------|
| `FrequencyRank_Phase1` | 1 | `AnalysisFlow_FRPH1` | 額外呼叫 `GetSkipFreqSetFilePath()` |
| `FrequencyRank_Phase2` | 2 | `AnalysisFlow_FRPH2` | — |
| `AC_FrequencyRank` | 3 | `AnalysisFlow_ACFR` | — |
| `Raw_ADC_Sweep` | 4 | `AnalysisFlow_RawADCS` | — |
| `Self_FrequencySweep` | 5 | `AnalysisFlow_SelfFS` | — |
| `Self_NCPNCNSweep` | 6 | `AnalysisFlow_SelfPNS` | — |
| `Else` | 7 | *(無分派)* | — |

### 分派模式細節

兩段式 if-else 模式（`DataAnalysis.cs` Line 31-96）：

**第一段**: 根據 `cFlowStep.m_eStep` 建構 AnalysisFlow 子類實例：
```csharp
if (m_eStep == MainStep.FrequencyRank_Phase1)
    m_cAnalysisFlowProcess = new AnalysisFlow_FRPH1(cFlowStep, sLogDir, ...);
else if (m_eStep == MainStep.FrequencyRank_Phase2)
    m_cAnalysisFlowProcess = new AnalysisFlow_FRPH2(cFlowStep, sLogDir, ...);
// ... 共 6 個分支
```

**中間**: 呼叫基類 `LoadAnalysisParameter()`：
```csharp
m_cAnalysisFlowProcess.LoadAnalysisParameter();
```

**第二段**: 向下轉型並呼叫子類方法：
```csharp
if (m_eStep == MainStep.FrequencyRank_Phase1)
{
    AnalysisFlow_FRPH1 cFlowItem = (AnalysisFlow_FRPH1)m_cAnalysisFlowProcess;
    cFlowItem.LoadAnalysisParameter();
    cFlowItem.GetSkipFreqSetFilePath(sSkipFreqSetFilePath);  // FRPH1 特殊
    bFlowComplete = cFlowItem.MainFlow(ref m_sErrorMessage);
}
// ... 共 6 個分支
```

> 注意：`LoadAnalysisParameter()` 被呼叫兩次 — 基類一次（Line 42）、子類一次（各分支內）。

---

## MPPPenAutoTuning — 三維分派

### 三維分派機制

```
MainTuningStep (11 值) × SubTuningStep (20 值) × nICSolutionType (None/Gen8)
```

**分派層級**:
1. **MainTuningStep** — 決定大類（噪音/傾角/增益/調校/壓力/線性度）
2. **SubTuningStep** — 決定子步驟（Hover 1st/2nd、Contact、TRxS 等）
3. **nICSolutionType** — 決定是否使用 Gen8 命令結構

### ProcessFlow 模式管理 (FR-008)

7 個 partial class 組成單一 `ProcessFlow` 類別：

| 檔案 | 主要入口方法 | 說明 |
|------|-------------|------|
| `ProcessFlow_MainFlow.cs` | `RunMainProcessThread(object)` | 啟動主+監聽線程 |
| `ProcessFlow_SingleMode.cs` | `RunFakeRobotThread(object)` | 單機模式（無實體線測機） |
| `ProcessFlow_ClientMode.cs` | `RunSocketRobotThread(object)` | 客戶端模式（連接線測機 Server） |
| `ProcessFlow_ServerMode.cs` | `RunServerListenFlow()` | 伺服器模式（監聽客戶端連線） |
| `ProcessFlow_GoDrawMode.cs` | `RunGoDrawRobotThread(object)` | 寫字機 GoDraw 模式 |
| `ProcessFlow_LoadDataMode.cs` | `RunLoadDataFlow()` | 離線資料載入分析 |
| `ProcessFlow_CommonFunction.cs` | *(欄位宣告/共用函式)* | 共用狀態管理 |

### AppCore 協調層 (FR-017)

**調用鏈**: `frmMain → AppCore → DataAnalysis → AnalysisFlow`

AppCore (`FingerAutoTuning/AppCore.cs`) 為 partial class，擔任以下協調職責：

| 職責 | 關鍵方法/物件 |
|------|-------------|
| 裝置連線 | `m_cInputDevice` (InputDevice) + `m_cElanSSHClient` (SSH) |
| 分析分派 | `m_cDataAnalysis` (DataAnalysis) |
| 資料儲存 | `m_cSaveData` (SaveData) |
| 幀管理 | `m_cFrameMgr` (FrameMgr) |
| 流程狀態 | `m_cFlowStep_List`, `m_bStartRecord`, `m_bEnterTestMode` |

---

## 雙模組架構差異對照

| 比較項目 | FingerAutoTuning | MPPPenAutoTuning |
|----------|------------------|------------------|
| **協調層** | AppCore (partial class) | ProcessFlow (7 partial class) |
| **分派器** | DataAnalysis.ExecuteMainWorkFlow() | DataAnalysis（三維分派） |
| **分派維度** | 1 維 (MainStep) | 3 維 (Main+Sub+ICType) |
| **流程步驟列舉** | MainStep (7 值) | MainTuningStep (11 值) + SubTuningStep (20 值) |
| **流程模式** | 單一模式 | 5 種模式 (Single/Client/Server/GoDraw/LoadData) |
| **AnalysisFlow 子類** | 6 個分派目標 | 16 個 |
| **基類 virtual 方法** | 4 個 | 1 個 |
| **FlowStep 定義位置** | `frmMain.cs` 內部類別 | `ParameterProperties.cs` 獨立類別 |

---

## 連動檔案清單

| 操作 | FingerAutoTuning | MPPPenAutoTuning |
|------|------------------|------------------|
| 新增分析流程 | `DataAnalysis.cs`, `frmMain.cs` (MainStep), `.csproj` | `DataAnalysis.cs`, `ParameterProperties.cs`, `.csproj` |
| 修改分派邏輯 | `DataAnalysis.cs` | `DataAnalysis.cs`, `ProcessFlow_*.cs` |
| 新增流程模式 | *(N/A — 單一模式)* | 新增 `ProcessFlow_<Mode>.cs`, `ProcessFlow_CommonFunction.cs` |
