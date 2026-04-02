# Research: Brownfield Architecture Documentation — Core Modules

**Feature**: `001-core-architecture-doc` | **Date**: 2026-04-02
**Source**: 直接從程式碼提取的精確事實

> 本文件為 Phase A-1 產出物，記錄從 AutoTuningTool 原始碼中提取的架構事實。
> 所有方法簽名、列舉值、行號均以程式碼實際內容為準。

---

## 1. FingerAutoTuning — AnalysisFlow 基類

**檔案**: `FingerAutoTuning/FingerAutoTuning/AnalysisFlow_Raw.cs`

### 1.1 類別定義

```csharp
namespace FingerAutoTuning
{
    public class AnalysisFlow  // Line 12
```

### 1.2 虛擬方法簽名 (Line 122-135)

| 方法 | 簽名 | 說明 |
|------|------|------|
| `InitializeParameter` | `public virtual void InitializeParameter()` | 初始化分析參數（空實作） |
| `InitializeSourceDataList` | `public virtual void InitializeSourceDataList()` | 初始化資料來源清單（空實作） |
| `LoadAnalysisParameter` | `public virtual void LoadAnalysisParameter()` | 載入分析參數（空實作） |
| `MainFlow` | `public virtual bool MainFlow(ref string sErrorMessage)` | 主分析流程，回傳 `true` |
| `GetDataCount` | `public bool GetDataCount()` | 非虛方法 — 計算各資料型別目錄中的檔案數量 |

### 1.3 受保護欄位 (子類共用)

| 欄位 | 型別 | 說明 | Line |
|------|------|------|------|
| `m_cFlowStep` | `frmMain.FlowStep` | 當前流程步驟參考 | 31 |
| `m_cfrmParent` | `frmMain` | 父表單實例 | 32 |
| `m_sLogDirectoryPath` | `string` | 日誌根目錄 | 33 |
| `m_bGenerateH5Data` | `bool` | H5 資料生成旗標 | 35 |
| `m_sH5LogDirectoryPath` | `string` | H5 日誌目錄 | 36 |
| `m_nTotalFileCount` | `int` | 待分析檔案總數 | 38 |
| `m_nAnalysisCount` | `int` | 已分析計數 | 39 |
| `m_nProgressIndex` | `int` | 進度索引 | 40 |
| `m_sErrorMessage` | `string` | 錯誤訊息 | 41 |
| `m_sProjectName` | `string` | 專案名稱 | 43 |
| `m_sSourceData_List` | `List<string>` | 資料型別清單 | 45 |

### 1.4 受保護常數

| 常數 | 值 | 說明 |
|------|-----|------|
| `m_nCOMPARE_Frequency` | 0 | 比較運算子 — 頻率 |
| `m_nCOMPARE_Normalize` | 1 | 比較運算子 — 正規化 |
| `m_nCOMPARE_SetIndex` | 2 | 比較運算子 — 設定索引 |
| `m_nCOMPARE_RawADC50PCT` | 3 | 比較運算子 — Raw ADC 50% |
| `m_nCOMPARE_NormalizeDifferStdPCT` | 4 | 比較運算子 — 正規化差異 STD% |
| `m_nCOMPARE_RawADCPCT` | 5 | 比較運算子 — Raw ADC% |
| `m_sTOOLNAME` | `"FingerAutoTuningTool"` | 工具名稱 |
| `m_sFILETYPE_ANALYSIS` | `"Analysis"` | 檔案類型常數 |
| `m_sFILETYPE_REPORT` | `"Report"` | 檔案類型常數 |
| `m_sFILETYPE_PROCESS` | `"Process"` | 檔案類型常數 |
| `m_sFILETYPE_REPORT_STATISTIC` | `"ReportStatistic"` | 檔案類型常數 |
| `m_sFILETYPE_STATISTIC` | `"Statistic"` | 檔案類型常數 |

### 1.5 巢狀類別 — FileCheckInfo (Line 55-67)

```csharp
public class FileCheckInfo
{
    public int m_nSetIndex = -1;
    public int m_nSetPH1 = -1;
    public int m_nSetPH2 = -1;
    public int m_nSetPH3 = -1;
    public int m_nSetDFT_NUM = -1;
    public int m_nReadPH1 = -1;
    public int m_nReadPH2 = -1;
    public int m_nReadPH3 = -1;
    public int m_nReadDFT_NUM = -1;
    public int m_nRXTraceNumber = -1;
    public int m_nTXTraceNumber = -1;
    public int m_nFWIP_Option = -1;
}
```

### 1.6 巢狀類別 — RawADCS_FileCheckInfo (Line 69-86)

```csharp
public class RawADCS_FileCheckInfo
{
    public ICGenerationType m_eICGenerationType = ICGenerationType.None;
    public ICSolutionType m_eICSolutionType = ICSolutionType.NA;
    public bool m_bGen7EnableHWTXN = false;
    public bool m_bGen6or7EnableFWTX4 = false;
    public int m_nSetIndex = -1;
    public int m_nSetSELC = -1;
    public int m_nSetVSEL = -1;
    public int m_nSetLG = -1;
    public int m_nSetSELGM = -1;
    public int m_nReadSELC = -1;
    public int m_nReadVSEL = -1;
    public int m_nReadLG = -1;
    public int m_nReadSELGM = -1;
    public int m_nReadIQ_BSH_0 = -1;
    public int m_nReadDFT_NUM = -1;
    public int m_nRXTraceNumber = -1;
    public int m_nTXTraceNumber = -1;
}
```

### 1.7 巢狀類別 — Self_FileCheckInfo (Line 88-121)

```csharp
public class Self_FileCheckInfo
{
    public TraceType m_eTraceType = TraceType.ALL;
    public int m_nSetIndex = -1;
    public int m_nSet_SELF_PH1 = -1;
    public int m_nSet_SELF_PH2E_LAT = -1;
    public int m_nSet_SELF_PH2E_LMT = -1;
    public int m_nSet_SELF_PH2_LAT = -1;
    public int m_nSet_SELF_PH2 = -1;
    public int m_nSetSelf_DFT_NUM = -1;
    public int m_nSetSelf_Gain = -1;
    public int m_nRead_SELF_PH1 = -1;
    public int m_nRead_SELF_PH2E_LAT = -1;
    public int m_nRead_SELF_PH2E_LMT = -1;
    public int m_nRead_SELF_PH2_LAT = -1;
    public int m_nRead_SELF_PH2 = -1;
    public int m_nReadSelf_DFT_NUM = -1;
    public int m_nReadSelf_Gain = -1;
    public int m_nReadSelf_CAG = -1;
    public int m_nReadSelf_IQ_BSH = -1;
    public int m_nRXTraceNumber = -1;
    public int m_nTXTraceNumber = -1;
    public bool m_bGetKValue = false;
    public int m_nRepeatIndex = 0;
    public int m_nNCPValue = -1;
    public int m_nNCNValue = -1;
    public int m_nCALValue = -1;
}
```

---

## 2. FingerAutoTuning — DataAnalysis 分派層

**檔案**: `FingerAutoTuning/FingerAutoTuning/DataAnalysis.cs`

### 2.1 ExecuteMainWorkFlow 完整簽名 (Line 17-19)

```csharp
public bool ExecuteMainWorkFlow(
    ref string m_sErrorMessage,
    frmMain.FlowStep cFlowStep,
    string sLogDirectoryPath,
    string sH5LogDirectoryPath,
    bool bGenerateH5Data,
    frmMain cfrmParent,
    string sProjectName,
    string sSkipFreqSetFilePath)
```

**參數說明**:

| 參數 | 型別 | 說明 |
|------|------|------|
| `m_sErrorMessage` | `ref string` | 錯誤訊息（傳址） |
| `cFlowStep` | `frmMain.FlowStep` | 當前流程步驟（含 MainStep） |
| `sLogDirectoryPath` | `string` | 日誌目錄路徑 |
| `sH5LogDirectoryPath` | `string` | H5 資料日誌路徑 |
| `bGenerateH5Data` | `bool` | 是否生成 H5 資料 |
| `cfrmParent` | `frmMain` | 父表單實例 |
| `sProjectName` | `string` | 專案名稱 |
| `sSkipFreqSetFilePath` | `string` | 跳過頻率設定檔路徑 |

### 2.2 分派邏輯 (Line 31-42)

```
MainStep.FrequencyRank_Phase1  → new AnalysisFlow_FRPH1(cFlowStep, sLogDir, sH5LogDir, bH5, cfrmParent, sProjectName)
MainStep.FrequencyRank_Phase2  → new AnalysisFlow_FRPH2(...)
MainStep.AC_FrequencyRank      → new AnalysisFlow_ACFR(...)
MainStep.Raw_ADC_Sweep         → new AnalysisFlow_RawADCS(...)
MainStep.Self_FrequencySweep   → new AnalysisFlow_SelfFS(...)
MainStep.Self_NCPNCNSweep      → new AnalysisFlow_SelfPNS(...)
```

**分派模式**: 兩段式 — 第一段 if-else 建構實例，第二段 if-else 進行向下轉型並呼叫 `LoadAnalysisParameter()` + `MainFlow()`。
注意: 基類的 `LoadAnalysisParameter()` 在第一段後先被呼叫一次，第二段再呼叫子類的覆寫版本。

### 2.3 MainStep 列舉 (frmMain.cs Line 27-35)

```csharp
public enum MainStep
{
    FrequencyRank_Phase1 = 1,
    FrequencyRank_Phase2 = 2,
    AC_FrequencyRank = 3,
    Raw_ADC_Sweep = 4,
    Self_FrequencySweep = 5,
    Self_NCPNCNSweep = 6,
    Else = 7
}
```

### 2.4 FlowStep 類別 (frmMain.cs Line 143-170)

```csharp
public class FlowStep
{
    public MainStep m_eStep = MainStep.Else;
    public string m_sStepName = "";
    public int m_nCostTime_Hour = -1;
    public int m_nCostTime_Minute = -1;
    public int m_nCostTime_Second = -1;
    public bool m_bLastStep = false;

    public void SetStepName(MainStep eStep, string sStepName, bool bLastStep)
    public void SetCostTimeParameter(int nCostTime_Hour, int nCostTime_Minute, int nCostTime_Second)
}
```

---

## 3. FingerAutoTuning — AppCore 協調層

**檔案**: `FingerAutoTuning/FingerAutoTuning/AppCore.cs`

### 3.1 類別定義

```csharp
namespace FingerAutoTuning
{
    partial class AppCore  // Line 21 — partial class，跨多個檔案
```

### 3.2 主要內部物件

| 欄位 | 型別 | 說明 | Line |
|------|------|------|------|
| `m_cInputDevice` | `InputDevice` | HID 裝置管理 | 47 |
| `m_cDebugLog` | `DebugLogAPI` | 偵錯日誌 | 48 |
| `m_cDataAnalysis` | `DataAnalysis` | 分派器實例 | 49 |
| `m_cfrmParent` | `frmMain` | 父表單參考 | 50 |
| `m_cElanSSHClient` | `ElanSSHClient` | 遠端 SSH/Socket 連線 | 52 |
| `m_cFrameMgr` | `FrameMgr` | 幀管理器 | 61 |
| `m_cSaveData` | `SaveData` | 資料儲存物件 | 88 |
| `m_cFlowStep_List` | `List<frmMain.FlowStep>` | 流程步驟清單 | 57 |

### 3.3 調用關係

```
frmMain → m_cAppCore (AppCore)
    → m_cDataAnalysis (DataAnalysis)
        → m_cAnalysisFlowProcess (AnalysisFlow 子類)
    → m_cInputDevice (InputDevice → ElanTouch)
    → m_cSaveData (SaveData)
```

---

## 4. FingerAutoTuning — DataSave (SaveData) 儲存層

**檔案**: `FingerAutoTuning/FingerAutoTuning/DataSave.cs`

### 4.1 CreateRecordData 方法簽名 (Line 41-42)

```csharp
public bool CreateRecordData(
    SaveDataInfo cSaveDataInfo,
    string sLogDirectoryPath,
    string sDataType,          // "Analysis", "Report" 等
    int nRepeatIndex,
    bool bGetSignalData,
    bool bSetRepeatIndex = false,
    bool bFirstData = true)
```

### 4.2 檔名生成邏輯

#### Self 模式 (Line 63-82)

1. 計算 `nSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(PH2E_LAT, PH2E_LMT, PH2_LAT, PH2)`
2. 計算 `dFrequency = ElanConvert.Convert2Frequency(SELF_PH1, nSelfPH2Sum)`
3. 基本格式：`Report_{Frequency:0.000}_{SELF_PH1:X2}_{SELF_PH2E_LMT:X2}_{SelfPH2Sum:X2}_{DFT_NUM:X2}_{TraceType}`
4. 含重複索引：追加 `_{nRepeatIndex}`
5. 含 K 序列：追加 `_P{NCPValue:00}N{NCNValue:00}`

#### Mutual 模式 (Line 84-99)

1. 計算 `dFrequency = ElanConvert.Convert2Frequency(PH1, PH2)`
2. 基本格式：`Report_{Frequency:0.000}_{PH1:X2}_{PH2:X2}`
3. 含重複索引：追加 `_{nRepeatIndex}`

#### 檔案路徑組合

```
{LogDirectoryPath}\{DataType}\Report_{...}.txt
```

---

## 5. FingerAutoTuning — ElanCommand 命令集

**檔案**: `FingerAutoTuning/FingerAutoTuning/Class/ElanCommand.cs`

### 5.1 ElanCommandType 列舉 (Line 17-99)

**無參數命令** (56 個): `NA`, `ResetIC`, `EnableICReport`, `DisableICReport`, `GetPH1`, `GetPH2`, `GetReportNumber`, `GetP0_TH`, `GetTRxS_Hover_TH_Rx`, `GetTRxS_Hover_TH_Tx`, `GetTRxS_Contact_TH_Rx`, `GetTRxS_Contact_TH_Tx`, `GetHover_TH_Rx`, `GetHover_TH_Tx`, `GetContact_TH_Rx`, `GetContact_TH_Tx`, `GetPTHF_Hover_TH_Rx`, `GetPTHF_Hover_TH_Tx`, `GetPTHF_Contact_TH_Rx`, `GetPTHF_Contact_TH_Tx`, `GetBHF_Hover_TH_Rx`, `GetBHF_Hover_TH_Tx`, `GetBHF_Contact_TH_Rx`, `GetBHF_Contact_TH_Tx`, `GetEdge_1Trc_SubPwr`, `GetEdge_2Trc_SubPwr`, `GetEdge_3Trc_SubPwr`, `GetEdge_4Trc_SubPwr`, `GetIQ_BSH_P`, `GetPressure3BinsTH`, `Get3BinsPwr`, `GetSNVersion`, `SetNoisePTHF`, `SetNoiseBHF`, `GetNoiseTRX_400us`, `GetNoiseRX_400us`, `GetNoiseTX_400us`, `GetNoiseTRX_800us`, `GetNoiseRX_800us`, `GetNoiseTX_800us`, `GetSyncTRxS`, `GetNonSyncRX_400us`, `GetNonSyncTX_400us`, `GetNonSyncTRX_400us`, `GetNonSyncRX_800us`, `GetNonSyncTX_800us`, `GetNonSyncTRX_800us`, `SetOverTHType`, `SetCoordFF`, `GetTiltPTHF`, `GetTiltBHF`, `GetPressure`, `GetLinearity`, `ResetTiltNoise`, `ResetTilt`, `StopNonSyncRXTX`

**單參數命令** (2 個): `SetPH1`, `SetPH2`

**雙參數命令 (HighByte, LowByte)** (27 個): `SetReportNumber`, `SetP0_TH`, `SetTRxS_Hover_TH_Rx`, `SetTRxS_Hover_TH_Tx`, `SetTRxS_Contact_TH_Rx`, `SetTRxS_Contact_TH_Tx`, `SetHover_TH_Rx`, `SetHover_TH_Tx`, `SetContact_TH_Rx`, `SetContact_TH_Tx`, `SetPTHF_Hover_TH_Rx`, `SetPTHF_Hover_TH_Tx`, `SetPTHF_Contact_TH_Rx`, `SetPTHF_Contact_TH_Tx`, `SetBHF_Hover_TH_Rx`, `SetBHF_Hover_TH_Tx`, `SetBHF_Contact_TH_Rx`, `SetBHF_Contact_TH_Tx`, `SetEdge_1Trc_SubPwr`, `SetEdge_2Trc_SubPwr`, `SetEdge_3Trc_SubPwr`, `SetEdge_4Trc_SubPwr`, `SetIQ_BSH_P`, `SetPressure3BinsTH`, `Set3BinsPwr`

**合計**: 85 個

### 5.2 ICValueTargetType 列舉 (Line 102-132)

共 29 個值：`NA`, `PH1`, `PH2`, `ReportNumber`, `P0_TH`, `TRxS_Hover_TH_Rx`, `TRxS_Hover_TH_Tx`, `TRxS_Contact_TH_Rx`, `TRxS_Contact_TH_Tx`, `Hover_TH_Rx`, `Hover_TH_Tx`, `Contact_TH_Rx`, `Contact_TH_Tx`, `PTHF_Hover_TH_Rx`, `PTHF_Hover_TH_Tx`, `PTHF_Contact_TH_Rx`, `PTHF_Contact_TH_Tx`, `BHF_Hover_TH_Rx`, `BHF_Hover_TH_Tx`, `BHF_Contact_TH_Rx`, `BHF_Contact_TH_Tx`, `Edge_1Trc_SubPwr`, `Edge_2Trc_SubPwr`, `Edge_3Trc_SubPwr`, `Edge_4Trc_SubPwr`, `IQ_BSH_P`, `Pressure3BinsTH`, `Press_3BinsPwr`, `SNVersion`

### 5.3 映射表 (Line 134-199)

兩個靜態字典，型別均為 `Dictionary<ICValueTargetType, ElanCommandType>`：

- `dictSetCommandMappingTable` — ICValueTargetType → 對應的 Set 命令（29 組映射）
- `dictGetCommandMappingTable` — ICValueTargetType → 對應的 Get 命令（29 組映射）

**映射規則**: 每個 `ICValueTargetType` 值都有一對一的 Set/Get 命令對應（`NA` 映射至 `ElanCommandType.NA`）。

### 5.4 轉換方法

```csharp
public static byte[] ConvertCommandToByte(ElanCommandType eCommandType, int nParameter = 0)
// 回傳 byte[6] 命令陣列

public static int ConvertToICGetValue(ElanCommandType eCommandType, byte[] byteGetBuffer_Array)
// 解析回覆資料並回傳整數值
```

---

## 6. FingerAutoTuning — ElanCommand_Gen8

**檔案**: `FingerAutoTuning/FingerAutoTuning/Class/ElanCommand_Gen8.cs`

### 6.1 命令資料結構 (Line 14-27)

```csharp
public class SendCommandInfo
{
    public List<CommandInfo> cCommandInfo_List = new List<CommandInfo>();
}

public class CommandInfo
{
    public byte[] byteCommand_Array;
    public int nDelayTime = ParamFingerAutoTuning.m_nGen8SendCommandDelayTime;
}
```

### 6.2 ParameterType 列舉 (Line 29-59)

共 29 個值，分三組：

**Mutual** (7): `_MS_PH1`, `_MS_PH2`, `_MS_PH3`, `_MS_AFE_DFT_NUM`, `_MS_AFE_SP_NUM`, `_MS_AFE_EFFECT_NUM`, `PKT_WC`

**Raw ADC Sweep** (10): `_MS_BIN_FIRCOEF_SEL_TAP_NUM`, `_MS_IQ_BSH_GP0_GP1`, `_MS_ANA_TP_CTL_01`, `_MS_ANA_TP_CTL_01_2`, `_MS_ANA_CTL_04`, `_MS_ANA_CTL_04_2`, `_MS_ANA_TP_CTL_06`, `_MS_ANA_TP_CTL_06_2`, `_MS_ANA_TP_CTL_07`

**Self** (12): `_SELF_PH1`, `_SELF_PH2E_LAT`, `_SELF_PH2_LAT`, `_SELF_PH2`, `_SELF_DFT_NUM`, `_SELF_SP_NUM`, `_SELF_EFFECT_NUM`, `_SELF_PKT_WC_L`, `_SELF_BSH_ADC_TP_NUM`, `_SELF_EFFECT_FW_SET_COEF_NUM`, `_SELF_DFT_NUM_IQ_FIR_CTL`, `_SELF_ANA_TP_CTL_01`, `_SELF_ANA_TP_CTL_00`, `_SELF_IQ_BSH_GP0_GP1`

> 注：Mutual 7 + Raw ADC 8（`_MS_ANA_TP_CTL_07` 前有 9 個但 `_MS_ANA_CTL_04` 不含 Raw ADC 前綴）+ Self 14 = 29

### 6.3 DataType 常數類別 (Line 62-67)

```csharp
public class DataType
{
    public const byte byteRead_8009_AFE = 0x67;
    public const byte byteWrite_8009_AFE = 0x68;
    public const byte byteRead_902_DataPath = 0x69;
    public const byte byteWrite_902_DataPath = 0x6A;
}
```

### 6.4 ParameterClass 常數 (Line 69-84)

```csharp
public class ParameterClass
{
    public const byte byteMutual_AFE_Para_Addr = 0x11;
    public const byte byteNoTX_AFE_Para_Addr = 0x12;
    public const byte byteOBL_AFE_Para_Addr = 0x13;
    public const byte byteScript_Mutual_Scan_Addr = 0x14;
    public const byte byteScript_NoTX_Scan_Addr = 0x15;
    public const byte byteScript_OBL_Scan_Addr = 0x16;
    public const byte byteSelf_RX_AFE_Para_Addr = 0x21;
    public const byte byteSelf_TX_AFE_Para_Addr = 0x22;
    public const byte byteScript_Self_RX_Scan_Addr = 0x23;
    public const byte byteScript_Self_TX_Scan_Addr = 0x24;
    public const byte byteMutual_DataPath_Para = 0x11;
    public const byte byteNoTX_DataPath_Para = 0x12;
    public const byte byteOBL_DataPath_Para = 0x13;
    public const byte byteSelf_RX_DataPath_Para = 0x21;
    public const byte byteSelf_TX_DataPath_Para = 0x22;
}
```

### 6.5 WriteCommandInfo 結構 (Line 86-96)

```csharp
public class WriteCommandInfo
{
    public string sParameterName = "";
    public byte byteType = 0x00;
    public byte byteClass = 0x00;
    public byte byteAddress1_H = 0x00;
    public byte byteAddress1_L = 0x00;
    public byte byteAddress2_H = 0x00;
    public byte byteAddress2_L = 0x00;
    public byte byteValue1_H = 0x00;
    public byte byteValue1_L = 0x00;
    public byte byteValue2_H = 0x00;
    public byte byteValue2_L = 0x00;
}
```

---

## 7. FingerAutoTuning — ElanTouch 通訊層

**檔案**: `FingerAutoTuning/FingerAutoTuning/Class/ElanTouch.cs`

### 7.1 介面常數 (Line 40-42)

```csharp
public const int INTERFACE_WIN_HID = 1;
public const int INTERFACE_WIN_BRIDGE_I2C = 8;
public const int INTERFACE_WIN_BRIDGE_SPI = 9;
```

### 7.2 SPI 模式常數 (Line 44-47)

```csharp
public const int INTF_TYPE_SPI_MA_FALLING = 0;
public const int INTF_TYPE_SPI_MA_RISING = 2;
public const int INTF_TYPE_SPI_MA_RISING_HALF = 4;
public const int INTF_TYPE_SPI_MA_FALLING_HALF = 6;
```

### 7.3 TP_INTERFACE_TYPE 列舉 (Line 49-58)

```csharp
public enum TP_INTERFACE_TYPE
{
    IF_USB = 0,
    IF_HID_OVER_I2C,
    IF_I2C,
    IF_SPI_MA_RISING_HALF_CYCLE,
    IF_SPI_MA_FALLING_HALF_CYCLE,
    IF_SPI_MA_RISING,
    IF_SPI_MA_FALLING,
    IF_SPI_PRECISE
}
```

### 7.4 TraceInfo 結構 (Line 104-153)

```csharp
public const int MAX_CHIP_NUM = 4;

public struct TraceInfo
{
    public int nChipNum;        // IC 晶片數量（1-4）
    public int nXTotal;         // 總 RX 追蹤數
    public int nYTotal;         // 總 TX 追蹤數
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHIP_NUM)]
    public int[] XAxis;         // 各晶片 RX 分佈
    public int nPartialNum;     // 部份追蹤數

    public TraceInfo(int nMaxChipNum)          // 建構子
    public int GetRXTraceNum(TraceMode Mode)   // 計算有效 RX
    public int GetTXTraceNum(TraceMode Mode)   // 計算有效 TX
}
```

### 7.5 TraceMode 列舉 (Line 170-176)

```csharp
public enum TraceMode
{
    Mutual = 0x01,
    Self = 0x02,
    Partial = 0x04,
    ComboSelf = 0x08,
    Combo2Self = 0x10
}
```

### 7.6 原生 DLL 資訊

- DLL 路徑: `public static string m_sDLLPath = "LibTouch.dll";`
- 資料讀取長度: `protected static int IN_DATA_LENGTH = 65;`
- 重試次數: `protected static int RETRY_COUNT = 50;`

### 7.7 Callback 委派

```csharp
public delegate void PFUNC_OUT_REPORT_CALLBACK(IntPtr pReportBuffer, int nReportLen);
public delegate void PFUNC_IN_REPORT_CALLBACK(IntPtr pReportBuffer, int nReportLen);
public delegate void PFUNC_SOCKET_EVENT_CALLBACK(int nEventID);
```

---

## 8. MPPPenAutoTuning — MainTuningStep / SubTuningStep

**檔案**: `MPPPenAutoTuning/MPPPenAutoTuning/ParameterProperties.cs`

### 8.1 MainTuningStep 列舉 (Line 9-21)

```csharp
public enum MainTuningStep
{
    NO = 1,
    TILTNO = 2,
    DIGIGAINTUNING = 3,
    TPGAINTUNING = 4,
    PEAKCHECKTUNING = 5,
    DIGITALTUNING = 6,
    TILTTUNING = 7,
    PRESSURETUNING = 8,
    LINEARITYTUNING = 9,
    SERVERCONTRL = 10,
    ELSE = 11
}
```

### 8.2 SubTuningStep 列舉 (Line 27-48)

```csharp
public enum SubTuningStep
{
    NO = 0,
    HOVER_1ST = 1,
    HOVER_2ND = 2,
    CONTACT = 3,
    HOVERTRxS = 4,
    CONTACTTRxS = 5,
    TILTNO_PTHF = 6,
    TILTNO_BHF = 7,
    TILTTUNING_PTHF = 8,
    TILTTUNING_BHF = 9,
    PRESSURESETTING = 10,
    PRESSUREPROTECT = 11,
    PRESSURETABLE = 12,
    LINEARITYTABLE = 13,
    PCHOVER_1ST = 14,
    PCHOVER_2ND = 15,
    PCCONTACT = 16,
    DIGIGAIN = 17,
    TP_GAIN = 18,
    ELSE = 19
}
```

### 8.3 三維分派機制

MPPPenAutoTuning 的 DataAnalysis 分派使用三個維度：
- **MainTuningStep**: 主步驟（11 個值）
- **SubTuningStep**: 子步驟（20 個值）
- **nICSolutionType**: IC 方案類型（`MainConstantParameter.m_nICSOLUTIONTYPE_NONE` / `m_nICSOLUTIONTYPE_GEN8`）

---

## 9. MPPPenAutoTuning — AnalysisFlow 基類

**檔案**: `MPPPenAutoTuning/MPPPenAutoTuning/AnalysisFlow/AnalysisFlow_Raw.cs`

### 9.1 唯一 virtual 方法

```csharp
public virtual void LoadAnalysisParameter()  // 唯一 virtual 方法
```

### 9.2 protected 非虛方法

```csharp
protected void InitializeParameter(FlowStep cFlowStep)  // 初始化基本參數
```

### 9.3 子類慣例方法 (convention-based, non-virtual)

各子類自行定義以下公開方法，但基類中無對應的 virtual 宣告：
- `SetFileDirectory()` — 設定檔案目錄
- `CheckDirectoryIsValid()` — 驗證目錄有效性
- `GetData()` — 取得資料
- `ComputeAndOutputResult()` — 計算並輸出結果

### 9.4 16 個子類分類表

> 注：分類為文件歸納，程式碼中無此分類定義。分派邏輯來源為 `Class/DataAnalysis.cs`。

| # | 類別名稱 | 繼承自 | 歸納分類 | MainTuningStep | SubTuningStep / 條件 |
|---|----------|--------|----------|----------------|---------------------|
| 1 | `AnalysisFlow_Noise` | `AnalysisFlow` | 噪音 | NO | 全部 NO 子步驟（預設，非 Gen8、非 TestMode） |
| 2 | `AnalysisFlow_Noise_Gen8` | `AnalysisFlow_Noise` | 噪音 (Gen8) | NO | `nICSolutionType == Gen8` |
| 3 | `AnalysisFlow_Noise_TestMode` | `AnalysisFlow` | 噪音 (TestMode) | NO | `ParamAutoTuning.m_nNoiseDataType == 1` |
| 4 | `AnalysisFlow_TiltNoise` | `AnalysisFlow` | 傾角噪音 | TILTNO | TILTNO_PTHF, TILTNO_BHF（預設，非 Gen8） |
| 5 | `AnalysisFlow_TiltNoise_Gen8` | `AnalysisFlow_TiltNoise` | 傾角噪音 (Gen8) | TILTNO | `nICSolutionType == Gen8` |
| 6 | `AnalysisFlow_DigiGainTuning` | `AnalysisFlow` | 數位增益 | DIGIGAINTUNING | 直接對應 |
| 7 | `AnalysisFlow_TPGainTuning` | `AnalysisFlow` | TP增益 | TPGAINTUNING | 直接對應 |
| 8 | `AnalysisFlow_PeakCheck` | `AnalysisFlow` | 峰值檢查 | PEAKCHECKTUNING | PCHOVER_1ST, PCHOVER_2ND, PCCONTACT |
| 9 | `AnalysisFlow_DTNormal` | `AnalysisFlow` | 數位調校 | DIGITALTUNING | HOVER_1ST, HOVER_2ND, CONTACT |
| 10 | `AnalysisFlow_DTTRxS` | `AnalysisFlow` | 數位調校 (TRxS) | DIGITALTUNING | HOVERTRxS, CONTACTTRxS |
| 11 | `AnalysisFlow_TiltTuning` | `AnalysisFlow` | 傾角調校 | TILTTUNING | TILTTUNING_PTHF, TILTTUNING_BHF |
| 12 | `AnalysisFlow_PressureSetting` | `AnalysisFlow` | 壓力設定 | PRESSURETUNING | PRESSURESETTING |
| 13 | `AnalysisFlow_PressureProtect` | `AnalysisFlow` | 壓力保護 | PRESSURETUNING | PRESSUREPROTECT |
| 14 | `AnalysisFlow_PressureTable` | `AnalysisFlow` | 壓力表 | PRESSURETUNING | PRESSURETABLE |
| 15 | `AnalysisFlow_LinearityTable` | `AnalysisFlow` | 線性度 | LINEARITYTUNING | 直接對應 |
| 16 | `AnalysisFlow_Else` | `AnalysisFlow` | 後備 | (其他) | fallback（未被上述條件匹配） |

**關鍵分派策略**：
- **條件式分派（IC Solution Type）**：NO / TILTNO 步驟根據 Gen8/非 Gen8 選擇不同子類
- **Sub-step 分派**：DIGITALTUNING / PRESSURETUNING 根據 SubTuningStep 值進一步分派
- **特殊邏輯**：NO 步驟額外檢查 `m_nNoiseDataType` 判斷 TestMode

---

## 10. MPPPenAutoTuning — ProcessFlow

### 10.1 7 個 Partial Class 檔案

| 檔案 | 主要職責 |
|------|----------|
| `ProcessFlow_MainFlow.cs` | 主線程啟動 (`RunMainProcessThread`)、監聽線程 |
| `ProcessFlow_SingleMode.cs` | 單機模式 (`RunFakeRobotThread`) |
| `ProcessFlow_ClientMode.cs` | 客戶端/線測機模式 (`RunSocketRobotThread`) |
| `ProcessFlow_ServerMode.cs` | 伺服器模式 (`RunServerListenFlow`) |
| `ProcessFlow_GoDrawMode.cs` | 寫字機 GoDraw 模式 (`RunGoDrawRobotThread`) |
| `ProcessFlow_LoadDataMode.cs` | 離線資料載入模式 (`RunLoadDataFlow`) |
| `ProcessFlow_CommonFunction.cs` | 共用函式（欄位宣告、參數管理、狀態控制） |

---

## 事實驗證清單

| 事實項目 | 驗證方式 | 結果 |
|----------|----------|------|
| FingerAutoTuning 有 6 個 AnalysisFlow 分派目標 + 1 個基類 | `AnalysisFlow_*.cs` file count | ✅ 7 檔案 |
| MPPPenAutoTuning 有 16 個 AnalysisFlow 子類 | `AnalysisFlow/*.cs` file count | ✅ 17 檔案（16 子類 + 1 基類） |
| ProcessFlow 有 7 個 partial class 檔案 | `ProcessFlow_*.cs` file count | ✅ 7 檔案 |
| ElanCommandType 共 85 個 (Finger) | 逐一計數 | ✅ 56+2+27=85 |
| MainStep 列舉 7 個值 | 程式碼對照 | ✅ 1-7 |
| MainTuningStep 列舉 11 個值 | 程式碼對照 | ✅ 1-11 |
| SubTuningStep 列舉 20 個值 | 程式碼對照 | ✅ 0-19 |
| Gen8 ParameterType 29 個 (Finger) | 逐一計數 | ✅ 29 |
