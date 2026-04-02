# Contract: MPPPenAutoTuning AnalysisFlow — 分析流程層

**模組**: `MPPPenAutoTuning` | **層級**: 分析流程層 | **FR**: FR-003, FR-004

---

## 介面簽名

### 基類: `AnalysisFlow` (`AnalysisFlow/AnalysisFlow_Raw.cs`)

```csharp
namespace MPPPenAutoTuning
{
    public class AnalysisFlow
    {
        // ── 唯一 virtual 方法 ──
        public virtual void LoadAnalysisParameter()

        // ── protected 非虛方法 ──
        protected void InitializeParameter(FlowStep cFlowStep)
    }
}
```

### 子類慣例方法 (convention-based, non-virtual)

各子類自行定義以下公開方法，基類中無對應的 virtual 宣告：

```csharp
public void SetFileDirectory()          // 設定檔案目錄
public void CheckDirectoryIsValid()     // 驗證目錄有效性
public void GetData()                   // 取得資料
public void ComputeAndOutputResult()    // 計算並輸出結果
```

---

## 16 個子類分類對照表

> 注：分類為文件歸納，程式碼中無此分類定義。分派邏輯來源為 `Class/DataAnalysis.cs`。

### 噪音類 (Noise) — 3 個

| # | 類別名稱 | 繼承自 | MainTuningStep | 分派條件 |
|---|----------|--------|----------------|----------|
| 1 | `AnalysisFlow_Noise` | `AnalysisFlow` | NO(1) | 預設（非 Gen8、非 TestMode） |
| 2 | `AnalysisFlow_Noise_Gen8` | `AnalysisFlow_Noise` | NO(1) | `nICSolutionType == Gen8` |
| 3 | `AnalysisFlow_Noise_TestMode` | `AnalysisFlow` | NO(1) | `m_nNoiseDataType == 1` |

### 傾角噪音類 (Tilt Noise) — 2 個

| # | 類別名稱 | 繼承自 | MainTuningStep | SubTuningStep |
|---|----------|--------|----------------|---------------|
| 4 | `AnalysisFlow_TiltNoise` | `AnalysisFlow` | TILTNO(2) | TILTNO_PTHF(6), TILTNO_BHF(7)（預設，非 Gen8） |
| 5 | `AnalysisFlow_TiltNoise_Gen8` | `AnalysisFlow_TiltNoise` | TILTNO(2) | `nICSolutionType == Gen8` |

### 增益調校類 (Gain Tuning) — 2 個

| # | 類別名稱 | 繼承自 | MainTuningStep | SubTuningStep |
|---|----------|--------|----------------|---------------|
| 6 | `AnalysisFlow_DigiGainTuning` | `AnalysisFlow` | DIGIGAINTUNING(3) | DIGIGAIN(17) |
| 7 | `AnalysisFlow_TPGainTuning` | `AnalysisFlow` | TPGAINTUNING(4) | TP_GAIN(18) |

### 峰值檢查類 (Peak Check) — 1 個

| # | 類別名稱 | 繼承自 | MainTuningStep | SubTuningStep |
|---|----------|--------|----------------|---------------|
| 8 | `AnalysisFlow_PeakCheck` | `AnalysisFlow` | PEAKCHECKTUNING(5) | PCHOVER_1ST(14), PCHOVER_2ND(15), PCCONTACT(16) |

### 數位調校類 (Digital Tuning) — 2 個

| # | 類別名稱 | 繼承自 | MainTuningStep | SubTuningStep |
|---|----------|--------|----------------|---------------|
| 9 | `AnalysisFlow_DTNormal` | `AnalysisFlow` | DIGITALTUNING(6) | HOVER_1ST(1), HOVER_2ND(2), CONTACT(3) |
| 10 | `AnalysisFlow_DTTRxS` | `AnalysisFlow` | DIGITALTUNING(6) | HOVERTRxS(4), CONTACTTRxS(5) |

### 傾角調校類 (Tilt Tuning) — 1 個

| # | 類別名稱 | 繼承自 | MainTuningStep | SubTuningStep |
|---|----------|--------|----------------|---------------|
| 11 | `AnalysisFlow_TiltTuning` | `AnalysisFlow` | TILTTUNING(7) | TILTTUNING_PTHF(8), TILTTUNING_BHF(9) |

### 壓力類 (Pressure) — 3 個

| # | 類別名稱 | 繼承自 | MainTuningStep | SubTuningStep |
|---|----------|--------|----------------|---------------|
| 12 | `AnalysisFlow_PressureSetting` | `AnalysisFlow` | PRESSURETUNING(8) | PRESSURESETTING |
| 13 | `AnalysisFlow_PressureProtect` | `AnalysisFlow` | PRESSURETUNING(8) | PRESSUREPROTECT |
| 14 | `AnalysisFlow_PressureTable` | `AnalysisFlow` | PRESSURETUNING(8) | PRESSURETABLE(12) |

### 線性度類 (Linearity) — 1 個

| # | 類別名稱 | 繼承自 | MainTuningStep | SubTuningStep |
|---|----------|--------|----------------|---------------|
| 15 | `AnalysisFlow_LinearityTable` | `AnalysisFlow` | LINEARITYTUNING(9) | LINEARITYTABLE(13) |

### 後備類 (Fallback) — 1 個

| # | 類別名稱 | 繼承自 | MainTuningStep | SubTuningStep |
|---|----------|--------|----------------|---------------|
| 16 | `AnalysisFlow_Else` | `AnalysisFlow` | (其他) | fallback（未被上述條件匹配） |

### 分派策略摘要

- **條件式分派 (IC Solution Type)**：NO / TILTNO 步驟根據 Gen8/非 Gen8 選擇不同子類
- **Sub-step 分派**：DIGITALTUNING / PRESSURETUNING 根據 SubTuningStep 值進一步分派
- **特殊邏輯**：NO 步驟額外檢查 `m_nNoiseDataType` 判斷 TestMode
- **繼承鏈**：13 個直接繼承 `AnalysisFlow`，3 個間接繼承（`Noise_Gen8` ← `Noise`、`TiltNoise_Gen8` ← `TiltNoise`）

---

## 行為契約

### 與 FingerAutoTuning 的關鍵差異

| 面向 | FingerAutoTuning | MPPPenAutoTuning |
|------|------------------|------------------|
| 基類虛擬方法 | 4 個 virtual + 1 non-virtual | 1 個 virtual + 1 protected non-virtual |
| 子類方法綁定 | 虛擬方法覆寫（compile-time polymorphism） | 慣例方法（explicit downcast in caller） |
| 分派維度 | 1 維（MainStep） | 3 維（MainTuningStep + SubTuningStep + nICSolutionType） |
| 協調層 | AppCore → DataAnalysis | ProcessFlow → DataAnalysis |
| 子類數量 | 6 | 16 |

### 執行順序

```
1. ProcessFlow 選擇模式（Single/Client/Server/GoDraw/LoadData）
2. DataAnalysis 根據 MainTuningStep + SubTuningStep + nICSolutionType 建構 AnalysisFlow 子類
3. LoadAnalysisParameter() → 載入參數
4. SetFileDirectory() → 設定目錄
5. CheckDirectoryIsValid() → 驗證
6. GetData() → 取得資料
7. ComputeAndOutputResult() → 計算輸出
```

---

## 擴展指南 — 新增 MPPPenAutoTuning AnalysisFlow

### Checklist

1. **建立檔案** `AnalysisFlow/AnalysisFlow_<NewName>.cs`
   - 命名空間：`namespace MPPPenAutoTuning`
   - 繼承：`public class AnalysisFlow_<NewName> : AnalysisFlow`

2. **實作方法**
   - 覆寫 `LoadAnalysisParameter()`
   - 定義 `SetFileDirectory()`, `CheckDirectoryIsValid()`, `GetData()`, `ComputeAndOutputResult()`

3. **註冊分派** — 修改 `DataAnalysis.cs`
   - 在 MainTuningStep/SubTuningStep 分派邏輯中新增對應分支
   - 若為 Gen8 IC，需在 `nICSolutionType == m_nICSOLUTIONTYPE_GEN8` 分支中處理

4. **更新 .csproj** — 在 `MPPPenAutoTuning.csproj` 中 Include 新檔案

---

## 連動檔案清單

| 操作 | 必須修改的檔案 |
|------|---------------|
| 新增 AnalysisFlow | `AnalysisFlow/AnalysisFlow_<Name>.cs` (新建)、`DataAnalysis.cs`、`MPPPenAutoTuning.csproj` |
| 修改基類 | `AnalysisFlow/AnalysisFlow_Raw.cs`、所有 16 個子類 |
| 新增 SubTuningStep | `ParameterProperties.cs`、`DataAnalysis.cs`、`StringConvert.cs` 映射表 |
