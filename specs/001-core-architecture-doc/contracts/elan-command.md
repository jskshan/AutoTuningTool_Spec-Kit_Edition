# Contract: ElanCommand — 命令結構與映射

**模組**: FingerAutoTuning + MPPPenAutoTuning | **FR**: FR-009, FR-010, FR-011

---

## ElanCommand 命令集

### 檔案位置

| 模組 | 檔案 |
|------|------|
| FingerAutoTuning | `FingerAutoTuning/Class/ElanCommand.cs` |
| MPPPenAutoTuning | `MPPPenAutoTuning/Class/ElanCommand.cs` |

### ElanCommandType 列舉統計

| 分類 | FingerAutoTuning | MPPPenAutoTuning |
|------|------------------|------------------|
| 無參數命令 | 56 | 67 |
| 單參數命令 | 2 (`SetPH1`, `SetPH2`) | 2 (`SetPH1`, `SetPH2`) |
| 雙參數命令 (H, L) | 27 | 35 |
| **合計** | **85** | **104** |

### 命令組織模式

```
ElanCommandType 列舉
├── 無參數命令: Get* (讀取), Reset*, Enable*, Disable*, Set*（特殊無參數 Set）, Stop*
├── 單參數命令: SetPH1, SetPH2（頻率參數）
└── 雙參數命令: Set*（HighByte + LowByte 封裝）
```

---

## ICValueTargetType 映射

### 介面簽名

```csharp
public enum ICValueTargetType { NA, PH1, PH2, ReportNumber, P0_TH, ... }  // 29 值 (Finger)

public static Dictionary<ICValueTargetType, ElanCommandType> dictSetCommandMappingTable;
public static Dictionary<ICValueTargetType, ElanCommandType> dictGetCommandMappingTable;
```

### 映射規則

- 每個 `ICValueTargetType` 值都有一對一的 Set/Get 命令對應
- `NA → ElanCommandType.NA`（空操作）
- Set 映射: `ICValueTargetType.PH1 → ElanCommandType.SetPH1`
- Get 映射: `ICValueTargetType.PH1 → ElanCommandType.GetPH1`
- 所有映射在靜態初始化時定義完成

### 轉換方法

```csharp
// 命令 → byte[] 轉換
public static byte[] ConvertCommandToByte(ElanCommandType eCommandType, int nParameter = 0)
// 回傳: byte[6] 命令陣列

// 回覆 → int 解析
public static int ConvertToICGetValue(ElanCommandType eCommandType, byte[] byteGetBuffer_Array)
// 回傳: 解析後的整數值
```

---

## ElanCommand_Gen8 擴展

### 檔案位置

| 模組 | 檔案 |
|------|------|
| FingerAutoTuning | `FingerAutoTuning/Class/ElanCommand_Gen8.cs` |
| MPPPenAutoTuning | `MPPPenAutoTuning/Class/ElanCommand_Gen8.cs` |

### 命令資料結構

```csharp
public class SendCommandInfo
{
    public List<CommandInfo> cCommandInfo_List = new List<CommandInfo>();
}

public class CommandInfo
{
    public byte[] byteCommand_Array;
    public int nDelayTime;  // 預設 ParamFingerAutoTuning.m_nGen8SendCommandDelayTime
}

public class WriteCommandInfo
{
    public string sParameterName;
    public byte byteType, byteClass;
    public byte byteAddress1_H, byteAddress1_L;
    public byte byteAddress2_H, byteAddress2_L;
    public byte byteValue1_H, byteValue1_L;
    public byte byteValue2_H, byteValue2_L;
}

public class ReadCommandInfo
{
    public string sParameterName;
    public byte byteType, byteClass;
    public byte byteAddress1_H, byteAddress1_L;
    public byte byteAddress2_H, byteAddress2_L;
}
```

### ParameterType 列舉

| 分組 | FingerAutoTuning (29) | MPPPenAutoTuning (37) |
|------|----------------------|----------------------|
| Mutual | 7: `_MS_PH1` ~ `PKT_WC` | 包含額外 MPP 特有參數 |
| Raw ADC | 10: `_MS_BIN_FIRCOEF_SEL_TAP_NUM` ~ `_MS_ANA_TP_CTL_07` | — |
| Self | 12: `_SELF_PH1` ~ `_SELF_IQ_BSH_GP0_GP1` | — |
| MPP Pen 專有 | — | 含 `MPP_SP_NUM` ~ `_Pen_MS_CT_ADC_SH_LMT` |

### DataType / ParameterClass 常數

```csharp
public class DataType
{
    public const byte byteRead_8009_AFE = 0x67;
    public const byte byteWrite_8009_AFE = 0x68;
    public const byte byteRead_902_DataPath = 0x69;
    public const byte byteWrite_902_DataPath = 0x6A;
}

public class ParameterClass
{
    public const byte byteMutual_AFE_Para_Addr = 0x11;   // 互電容 AFE 參數
    public const byte byteSelf_RX_AFE_Para_Addr = 0x21;  // 自電容 RX AFE 參數
    public const byte byteSelf_TX_AFE_Para_Addr = 0x22;  // 自電容 TX AFE 參數
    // ... (共 15 個常數)
}
```

---

## 擴展指南 — 新增 ElanCommand

### Checklist

1. **新增列舉值** — 在 `ElanCommandType` 中的對應分類區段新增
2. **新增映射** — 若需要 ICValueTargetType，同時在 `dictSetCommandMappingTable` 和 `dictGetCommandMappingTable` 新增映射
3. **更新轉換方法** — 在 `ConvertCommandToByte()` 和 `ConvertToICGetValue()` 中新增 byte 編碼/解碼邏輯
4. **雙模組同步** — FingerAutoTuning 和 MPPPenAutoTuning 各有獨立的 ElanCommand.cs，需確認是否需要同步新增

---

## 連動檔案清單

| 操作 | 必須修改的檔案 |
|------|---------------|
| 新增命令 | `ElanCommand.cs` (列舉+映射+轉換) |
| 新增 Gen8 參數 | `ElanCommand_Gen8.cs` (ParameterType+WriteCommandInfo 初始化) |
| 跨模組同步 | `FingerAutoTuning/Class/ElanCommand.cs` + `MPPPenAutoTuning/Class/ElanCommand.cs` |
