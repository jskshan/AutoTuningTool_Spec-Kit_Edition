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

## 17 個子類分類對照表

> 注：分類為文件歸納，程式碼中無此分類定義。SubTuningStep 對應為事實依據。

### 噪音類 (Noise) — 5 個

| # | 類別名稱 | MainTuningStep | SubTuningStep |
|---|----------|----------------|---------------|
| 1 | `AnalysisFlow_NoiseHover1stNO` | NO(1) | HOVER_1ST(1) |
| 2 | `AnalysisFlow_NoiseHover2ndNO` | NO(1) | HOVER_2ND(2) |
| 3 | `AnalysisFlow_NoiseContactNO` | NO(1) | CONTACT(3) |
| 4 | `AnalysisFlow_NoiseTRxSHoverNO` | NO(1) | HOVERTRxS(4) |
| 5 | `AnalysisFlow_NoiseTRxSContactNO` | NO(1) | CONTACTTRxS(5) |

### 傾角噪音類 (Tilt Noise) — 2 個

| # | 類別名稱 | MainTuningStep | SubTuningStep |
|---|----------|----------------|---------------|
| 6 | `AnalysisFlow_TiltNoisePTHF` | TILTNO(2) | TILTNO_PTHF(6) |
| 7 | `AnalysisFlow_TiltNoiseBHF` | TILTNO(2) | TILTNO_BHF(7) |

### 傾角調校類 (Tilt Tuning) — 2 個

| # | 類別名稱 | MainTuningStep | SubTuningStep |
|---|----------|----------------|---------------|
| 8 | `AnalysisFlow_TiltTuningPTHF` | TILTTUNING(7) | TILTTUNING_PTHF(8) |
| 9 | `AnalysisFlow_TiltTuningBHF` | TILTTUNING(7) | TILTTUNING_BHF(9) |

### 增益調校類 (Gain Tuning) — 2 個

| # | 類別名稱 | MainTuningStep | SubTuningStep |
|---|----------|----------------|---------------|
| 10 | `AnalysisFlow_DigiGainTuning` | DIGIGAINTUNING(3) | DIGIGAIN(17) |
| 11 | `AnalysisFlow_TPGainTuning` | TPGAINTUNING(4) | TP_GAIN(18) |

### 峰值檢查類 (Peak Check) — 3 個

| # | 類別名稱 | MainTuningStep | SubTuningStep |
|---|----------|----------------|---------------|
| 12 | `AnalysisFlow_PeakCheckHover1st` | PEAKCHECKTUNING(5) | PCHOVER_1ST(14) |
| 13 | `AnalysisFlow_PeakCheckHover2nd` | PEAKCHECKTUNING(5) | PCHOVER_2ND(15) |
| 14 | `AnalysisFlow_PeakCheckContact` | PEAKCHECKTUNING(5) | PCCONTACT(16) |

### 數位調校類 (Digital Tuning) — 1 個

| # | 類別名稱 | MainTuningStep | SubTuningStep |
|---|----------|----------------|---------------|
| 15 | `AnalysisFlow_DigitalTuning` | DIGITALTUNING(6) | — (依子步驟變化) |

### 壓力類 (Pressure) — 1 個

| # | 類別名稱 | MainTuningStep | SubTuningStep |
|---|----------|----------------|---------------|
| 16 | `AnalysisFlow_PressureTuning` | PRESSURETUNING(8) | PRESSURETABLE(12) |

### 線性度類 (Linearity) — 1 個

| # | 類別名稱 | MainTuningStep | SubTuningStep |
|---|----------|----------------|---------------|
| 17 | `AnalysisFlow_LinearityTuning` | LINEARITYTUNING(9) | LINEARITYTABLE(13) |

---

## 行為契約

### 與 FingerAutoTuning 的關鍵差異

| 面向 | FingerAutoTuning | MPPPenAutoTuning |
|------|------------------|------------------|
| 基類虛擬方法 | 4 個 virtual + 1 non-virtual | 1 個 virtual + 1 protected non-virtual |
| 子類方法綁定 | 虛擬方法覆寫（compile-time polymorphism） | 慣例方法（explicit downcast in caller） |
| 分派維度 | 1 維（MainStep） | 3 維（MainTuningStep + SubTuningStep + nICSolutionType） |
| 協調層 | AppCore → DataAnalysis | ProcessFlow → DataAnalysis |
| 子類數量 | 6 | 17 |

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
| 修改基類 | `AnalysisFlow/AnalysisFlow_Raw.cs`、所有 17 個子類 |
| 新增 SubTuningStep | `ParameterProperties.cs`、`DataAnalysis.cs`、`StringConvert.cs` 映射表 |
