# Contract: FingerAutoTuning AnalysisFlow — 分析流程層

**模組**: `FingerAutoTuning` | **層級**: 分析流程層 | **FR**: FR-001, FR-002

---

## 介面簽名

### 基類: `AnalysisFlow` (`AnalysisFlow_Raw.cs`)

```csharp
namespace FingerAutoTuning
{
    public class AnalysisFlow
    {
        // ── 虛擬方法（子類必須覆寫）──
        public virtual void InitializeParameter()
        public virtual void InitializeSourceDataList()
        public virtual void LoadAnalysisParameter()
        public virtual bool MainFlow(ref string sErrorMessage)

        // ── 非虛方法 ──
        public bool GetDataCount()  // 計算待分析檔案總數
    }
}
```

### 建構子模式（子類共用）

所有子類使用相同的六參數建構子：

```csharp
public AnalysisFlow_XXXX(
    frmMain.FlowStep cFlowStep,
    string sLogDirectoryPath,
    string sH5LogDirectoryPath,
    bool bGenerateH5Data,
    frmMain cfrmParent,
    string sProjectName)
```

---

## 6 個分派目標實作

| 子類 | 檔案 | MainStep | 值 |
|------|------|----------|----|
| `AnalysisFlow_FRPH1` | `AnalysisFlow_FRPH1.cs` | `FrequencyRank_Phase1` | 1 |
| `AnalysisFlow_FRPH2` | `AnalysisFlow_FRPH2.cs` | `FrequencyRank_Phase2` | 2 |
| `AnalysisFlow_ACFR` | `AnalysisFlow_ACFR.cs` | `AC_FrequencyRank` | 3 |
| `AnalysisFlow_RawADCS` | `AnalysisFlow_RawADCS.cs` | `Raw_ADC_Sweep` | 4 |
| `AnalysisFlow_SelfFS` | `AnalysisFlow_SelfFS.cs` | `Self_FrequencySweep` | 5 |
| `AnalysisFlow_SelfPNS` | `AnalysisFlow_SelfPNS.cs` | `Self_NCPNCNSweep` | 6 |

> `MainStep.Else = 7` 無對應分派實作。

---

## 行為契約

### 前置條件

1. `DataAnalysis.ExecuteMainWorkFlow()` 被呼叫時，`cFlowStep.m_eStep` 必須為 `MainStep` 列舉值 1-6 之一
2. `sLogDirectoryPath` 必須為有效的目錄路徑，內含對應 DataType 的子目錄
3. `cfrmParent` (frmMain) 實例必須已初始化完成

### 執行順序

```
1. 建構子 → 初始化基類欄位
2. LoadAnalysisParameter() → 載入外部參數（被呼叫兩次：基類一次 + 子類一次）
3. [FRPH1 特殊] GetSkipFreqSetFilePath() → 載入跳過頻率設定
4. MainFlow(ref sErrorMessage) → 執行核心分析邏輯
```

### 後置條件

1. 回傳 `true` 表示分析完成（不代表無告警）
2. 回傳 `false` 表示發生錯誤，`sErrorMessage` 包含錯誤訊息
3. `GetDataCount()` 在 `MainFlow` 前被呼叫，設定 `m_nTotalFileCount`

---

## 擴展指南 — 新增 FingerAutoTuning AnalysisFlow

### Checklist

1. **建立檔案** `AnalysisFlow_<NewName>.cs`
   - 命名空間：`namespace FingerAutoTuning`
   - 繼承：`public class AnalysisFlow_<NewName> : AnalysisFlow`
   - 六參數建構子（與現有子類相同模式）

2. **覆寫虛擬方法**
   - `InitializeParameter()` — 初始化分析所需參數
   - `InitializeSourceDataList()` — 設定 `m_sSourceData_List` 資料型別
   - `LoadAnalysisParameter()` — 從 `ParamFingerAutoTuning` 載入外部參數
   - `MainFlow(ref string sErrorMessage)` — 實作核心分析邏輯

3. **註冊分派** — 修改 `DataAnalysis.cs`
   - 在 `MainStep` 列舉中新增對應值（或使用 `Else`）
   - 在 `ExecuteMainWorkFlow()` 中新增兩段 if-else：建構 + 呼叫

4. **更新 .csproj** — 在 `FingerAutoTuning.csproj` 中 Include 新檔案
5. **更新 README.md** — 在分析流程清單中新增項目

---

## 連動檔案清單

| 操作 | 必須修改的檔案 |
|------|---------------|
| 新增 AnalysisFlow | `AnalysisFlow_<Name>.cs` (新建)、`DataAnalysis.cs`、`FingerAutoTuning.csproj`、`README.md` |
| 修改基類虛擬方法 | `AnalysisFlow_Raw.cs`、所有 6 個子類 |
| 修改 MainStep 列舉 | `frmMain.cs`、`DataAnalysis.cs` |
