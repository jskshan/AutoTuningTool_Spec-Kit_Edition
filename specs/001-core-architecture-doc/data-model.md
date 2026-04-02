# Data Model: 核心資料結構參考

**Feature**: `001-core-architecture-doc` | **FR**: FR-013, FR-014

> 本文件記錄兩個模組中跨層共用的關鍵資料結構。
> 所有型別簽名與欄位清單以程式碼實際內容為準。

---

## 1. FlowStep — 流程步驟容器

### FingerAutoTuning (`frmMain.cs` L143-170)

```csharp
public class FlowStep
{
    public MainStep m_eStep = MainStep.Else;
    public string   m_sStepName = "";
    public int      m_nCostTime_Hour = -1;
    public int      m_nCostTime_Minute = -1;
    public int      m_nCostTime_Second = -1;
    public bool     m_bLastStep = false;

    public void SetStepName(MainStep eStep, string sStepName, bool bLastStep);
    public void SetCostTimeParameter(int nCostTime_Hour, int nCostTime_Minute, int nCostTime_Second);
}
```

**使用方式**: `frmMain.GenerateFlowStep()` 根據 `ParamFingerAutoTuning.m_StepSettingParameter_Array` 生成 `List<FlowStep>`，由 `AppCore.m_cFlowStep_List` 持有，逐一傳入 `DataAnalysis.ExecuteMainWorkFlow()`。

### MPPPenAutoTuning

MPPPenAutoTuning 的 FlowStep 定義於模組內部 `frmMain.cs`，結構相似但使用 `MainTuningStep` + `SubTuningStep` 雙維度。

---

## 2. MainStep / MainTuningStep / SubTuningStep — 分派列舉

### FingerAutoTuning — MainStep (`frmMain.cs` L27-35)

```csharp
public enum MainStep
{
    FrequencyRank_Phase1    = 1,
    FrequencyRank_Phase2    = 2,
    AC_FrequencyRank        = 3,
    Raw_ADC_Sweep           = 4,
    Self_FrequencySweep     = 5,
    Self_NCPNCNSweep        = 6,
    Else                    = 7
}
```

### MPPPenAutoTuning — MainTuningStep (`ParameterProperties.cs` L9-21)

```csharp
public enum MainTuningStep
{
    NO = 1, TILTNO = 2, DIGIGAINTUNING = 3, TPGAINTUNING = 4,
    PEAKCHECKTUNING = 5, DIGITALTUNING = 6, TILTTUNING = 7,
    PRESSURETUNING = 8, LINEARITYTUNING = 9, SERVERCONTRL = 10,
    ELSE = 11
}
```

### MPPPenAutoTuning — SubTuningStep (`ParameterProperties.cs` L27-48)

```csharp
public enum SubTuningStep
{
    NO = 0, HOVER_1ST = 1, HOVER_2ND = 2, CONTACT = 3,
    HOVERTRxS = 4, CONTACTTRxS = 5, TILTNO_PTHF = 6, TILTNO_BHF = 7,
    TILTTUNING_PTHF = 8, TILTTUNING_BHF = 9, PRESSURESETTING = 10,
    PRESSUREPROTECT = 11, PRESSURETABLE = 12, LINEARITYTABLE = 13,
    PCHOVER_1ST = 14, PCHOVER_2ND = 15, PCCONTACT = 16,
    DIGIGAIN = 17, TP_GAIN = 18, ELSE = 19
}
```

### 分派維度比較

| 項目 | FingerAutoTuning | MPPPenAutoTuning |
|------|------------------|------------------|
| 維度數 | 1 (`MainStep`) | 3 (`MainTuningStep` × `SubTuningStep` × `nICSolutionType`) |
| 分派入口 | `DataAnalysis.ExecuteMainWorkFlow()` | `DataAnalysis.ExecuteMainWorkFlow()` |
| 分派模式 | 兩段式 if-else (建構 → 轉型呼叫) | 巢狀 if-else (MainTuningStep → SubTuningStep → nICSolutionType) |

---

## 3. FileCheckInfo 結構家族

三個巢狀類別定義在 `AnalysisFlow_Raw.cs` 中（FingerAutoTuning），用於記錄分析前的參數校驗資訊。

### FileCheckInfo (L55-67)

```csharp
public class FileCheckInfo
{
    public int m_nSetIndex, m_nSetPH1, m_nSetPH2, m_nSetPH3;
    public int m_nSetDFT_NUM;
    public int m_nReadPH1, m_nReadPH2, m_nReadPH3;
    public int m_nReadDFT_NUM;
    public int m_nRXTraceNumber, m_nTXTraceNumber;
    public int m_nFWIP_Option;
    // 所有欄位預設值 = -1
}
```

**用途**: Mutual 模式（FRPH1, FRPH2, ACFR）的檔案參數校驗，追蹤 Set/Read 參數差異。

### RawADCS_FileCheckInfo (L69-86)

```csharp
public class RawADCS_FileCheckInfo
{
    public ICGenerationType m_eICGenerationType = ICGenerationType.None;
    public ICSolutionType   m_eICSolutionType = ICSolutionType.NA;
    public bool m_bGen7EnableHWTXN, m_bGen6or7EnableFWTX4;
    public int  m_nSetIndex, m_nSetSELC, m_nSetVSEL, m_nSetLG, m_nSetSELGM;
    public int  m_nReadSELC, m_nReadVSEL, m_nReadLG, m_nReadSELGM;
    public int  m_nReadIQ_BSH_0, m_nReadDFT_NUM;
    public int  m_nRXTraceNumber, m_nTXTraceNumber;
}
```

**用途**: Raw ADC Sweep 模式專用，含 IC 世代/方案類型判斷與 AFE 參數。

### Self_FileCheckInfo (L88-121)

```csharp
public class Self_FileCheckInfo
{
    public TraceType m_eTraceType = TraceType.ALL;
    public int  m_nSetIndex, m_nSet_SELF_PH1;
    public int  m_nSet_SELF_PH2E_LAT, m_nSet_SELF_PH2E_LMT;
    public int  m_nSet_SELF_PH2_LAT, m_nSet_SELF_PH2;
    public int  m_nSetSelf_DFT_NUM, m_nSetSelf_Gain;
    public int  m_nRead_SELF_PH1;
    public int  m_nRead_SELF_PH2E_LAT, m_nRead_SELF_PH2E_LMT;
    public int  m_nRead_SELF_PH2_LAT, m_nRead_SELF_PH2;
    public int  m_nReadSelf_DFT_NUM, m_nReadSelf_Gain, m_nReadSelf_CAG, m_nReadSelf_IQ_BSH;
    public int  m_nRXTraceNumber, m_nTXTraceNumber;
    public bool m_bGetKValue;
    public int  m_nRepeatIndex, m_nNCPValue, m_nNCNValue, m_nCALValue;
}
```

**用途**: Self 模式（SelfFS, SelfPNS）專用，含 PH2 複合計算參數與 K 值序列追蹤。

---

## 4. SaveDataInfo — 儲存參數容器

**檔案**: `FingerAutoTuning/FingerAutoTuning/DataSave.cs` L997-1075

### 4.1 欄位定義

```csharp
public class SaveDataInfo
{
    // 共用
    public string m_sLogDirectoryPath;
    public int    m_nListIndex;
    public uint   m_nFWVersion;
    public int    m_nFrameNumber;

    // Mutual 模式
    public int m_nPH1, m_nPH2, m_nPH3, m_nDFT_NUM;
    public int m_nTXTraceNumber, m_nRXTraceNumber;
    public int m_nReadPH1, m_nReadPH2, m_nReadPH3, m_nReadDFT_NUM;

    // RawADCS 模式
    public bool m_bRawADCSweep;
    public int  m_nFIRCOEF_SEL, m_nFIRTB, m_nFIR_TAP_NUM;
    public int  m_nSELGM, m_nIQ_BSH_0, m_nSELC, m_nVSEL, m_nLG;
    public int  m_nReadFIRCOEF_SEL, m_nReadFIRTB, m_nReadFIR_TAP_NUM;
    public int  m_nReadSELGM, m_nReadSELC, m_nReadVSEL, m_nReadLG;
    public int  m_nReadProjectOption, m_nReadFWIPOption;

    // Self 模式
    public string m_sSelfTraceType, m_sSelfTracePart;
    public int  m_n_SELF_PH1, m_n_SELF_PH2E_LAT, m_n_SELF_PH2E_LMT;
    public int  m_n_SELF_PH2_LAT, m_n_SELF_PH2;
    public int  m_nSelf_DFT_NUM, m_nSelf_Gain, m_nSelf_CAG, m_nSelf_IQ_BSH;
    public int  m_nRead_SELF_PH1, m_nRead_SELF_PH2E_LAT, m_nRead_SELF_PH2E_LMT;
    public int  m_nRead_SELF_PH2_LAT, m_nRead_SELF_PH2;
    public int  m_nReadSelf_DFT_NUM, m_nReadSelf_Gain, m_nReadSelf_CAG, m_nReadSelf_IQ_BSH;
    public double m_dSelf_SampleTime;
    public bool m_bGetSelf, m_bGetSelfKValue, m_bSetSelfKSequence, m_bSetSelfCAL;
    public int  m_nSelfNCPValue, m_nSelfNCNValue, m_nSelfCALValue;
}
```

### 4.2 CreateRecordData 方法簽名

```csharp
public bool CreateRecordData(
    SaveDataInfo cSaveDataInfo,
    string sLogDirectoryPath,
    string sDataType,              // "Analysis", "Report", "Process", "ReportStatistic", "Statistic"
    int nRepeatIndex,
    bool bGetSignalData,
    bool bSetRepeatIndex = false,
    bool bFirstData = true)
```

`SaveDataInfo` 封裝當前量測結果的完整參數，由各 AnalysisFlow 子類在 `MainFlow()` 中建構後傳入 `DataSave.CreateRecordData()`。

### 4.3 檔名生成規則 (FR-013, FR-014)

#### Mutual 模式

```
Frequency = Convert2Frequency(PH1, PH2)
檔名: Report_{Frequency:0.000}_{PH1:X2}_{PH2:X2}[_{nRepeatIndex}]
```

#### Self 模式

```
SelfPH2Sum = Convert2SelfPH2SumInt(PH2E_LAT, PH2E_LMT, PH2_LAT, PH2)
Frequency  = Convert2Frequency(SELF_PH1, SelfPH2Sum)
檔名: Report_{Frequency:0.000}_{SELF_PH1:X2}_{SELF_PH2E_LMT:X2}_{SelfPH2Sum:X2}_{DFT_NUM:X2}_{TraceType}[_{nRepeatIndex}]
K 序列追加: _P{NCPValue:00}N{NCNValue:00}
```

#### 檔案路徑組合

```
{LogDirectoryPath}\{DataType}\Report_{...}.txt
DataType = "Analysis" | "Report" | "Process" | "ReportStatistic" | "Statistic"
```

---

## 5. TraceInfo — 觸控追蹤資訊

**檔案**: `ElanTouch.cs` (兩個模組)

```csharp
public struct TraceInfo
{
    public int   nChipNum;        // IC 晶片數量（1-4）
    public int   nXTotal;         // 總 RX 追蹤數
    public int   nYTotal;         // 總 TX 追蹤數
    public int[] XAxis;           // [MAX_CHIP_NUM=4] 各晶片 RX 分佈
    public int   nPartialNum;     // 部份追蹤數

    public int GetRXTraceNum(TraceMode Mode);
    public int GetTXTraceNum(TraceMode Mode);
}
```

**特性**: 使用 `MarshalAsAttribute(UnmanagedType.ByValArray)` 支援 P/Invoke 與 Native DLL 互操作。

---

## 6. ElanCommand 命令結構

### 標準命令 — byte[6] 格式

```
ConvertCommandToByte(ElanCommandType, int nParameter = 0) → byte[6]
```

- **無參數**: 指令碼填入固定位元組
- **單參數**: 指令碼 + nParameter 直接填入
- **雙參數**: 指令碼 + HighByte + LowByte（自動拆分 nParameter）

### Gen8 命令結構

```csharp
// 寫入命令
WriteCommandInfo { sParameterName, byteType, byteClass,
    byteAddress1_H/L, byteAddress2_H/L, byteValue1_H/L, byteValue2_H/L }

// 讀取命令
ReadCommandInfo { sParameterName, byteType, byteClass,
    byteAddress1_H/L, byteAddress2_H/L }

// 命令批次
SendCommandInfo { List<CommandInfo> cCommandInfo_List }
CommandInfo { byte[] byteCommand_Array, int nDelayTime }
```

### 命令路由

```
nICSolutionType == GEN8  →  ElanCommand_Gen8 (WriteCommandInfo / ReadCommandInfo)
nICSolutionType != GEN8  →  ElanCommand (ConvertCommandToByte / ConvertToICGetValue)
```

---

## 7. TraceMode — 追蹤模式旗標

```csharp
[Flags]
public enum TraceMode
{
    Mutual     = 0x01,    // 互電容
    Self       = 0x02,    // 自電容
    Partial    = 0x04,    // 部份追蹤
    ComboSelf  = 0x08,    // 組合自電容
    Combo2Self = 0x10     // 雙自電容
}
```

**使用位置**: `TraceInfo.GetRXTraceNum(TraceMode)` / `GetTXTraceNum(TraceMode)` 以位元旗標組合傳入。

---

## 8. AnalysisFlow 受保護常數

定義在 `AnalysisFlow_Raw.cs` (FingerAutoTuning) 基類中，所有子類共用：

| 常數群組 | 數量 | 說明 |
|----------|------|------|
| `m_nCOMPARE_*` | 6 | 比較運算模式（Frequency, Normalize, SetIndex, RawADC50PCT, …） |
| `m_sTOOLNAME` | 1 | `"FingerAutoTuningTool"` |
| `m_sFILETYPE_*` | 5 | 檔案類型字串（Analysis, Report, Process, ReportStatistic, Statistic） |

---

## 跨層資料流圖

```
frmMain
  │  GenerateFlowStep() → List<FlowStep>
  ▼
AppCore
  │  m_cFlowStep_List → iterate
  ▼
DataAnalysis.ExecuteMainWorkFlow(FlowStep, ...)
  │  FlowStep.m_eStep → MainStep switch
  ▼
AnalysisFlow 子類
  │  LoadAnalysisParameter() → FileCheckInfo / RawADCS_FileCheckInfo / Self_FileCheckInfo
  │  MainFlow()
  │    ├── ElanCommand / ElanCommand_Gen8 → byte[] → ElanTouch (TraceInfo, TraceMode)
  │    └── DataSave.CreateRecordData(SaveDataInfo, ...) → .txt 檔案
  ▼
Output: {LogDirectoryPath}\{DataType}\Report_{...}.txt
```
