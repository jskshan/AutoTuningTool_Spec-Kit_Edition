# Contract: Communication Layer — 通訊傳輸層

**模組**: FingerAutoTuning + MPPPenAutoTuning | **FR**: FR-012, FR-015, FR-016

---

## ElanTouch 通訊介面

### 檔案位置

| 模組 | 檔案 | Native DLL |
|------|------|-----------|
| FingerAutoTuning | `FingerAutoTuning/Class/ElanTouch.cs` | `LibTouch.dll` |
| MPPPenAutoTuning | `MPPPenAutoTuning/Class/ElanTouch.cs` | `LibTouch.dll` |
| Socket 模式 | `ElanTouch_Socket.cs` | `LibTouch_Socket.dll` |

### 介面常數

```csharp
public const int INTERFACE_WIN_HID = 1;         // Windows HID 直接連接
public const int INTERFACE_WIN_BRIDGE_I2C = 8;   // HID-over-I2C Bridge
public const int INTERFACE_WIN_BRIDGE_SPI = 9;   // SPI Bridge
```

### SPI 模式常數

```csharp
public const int INTF_TYPE_SPI_MA_FALLING = 0;
public const int INTF_TYPE_SPI_MA_RISING = 2;
public const int INTF_TYPE_SPI_MA_RISING_HALF = 4;
public const int INTF_TYPE_SPI_MA_FALLING_HALF = 6;
```

### TP_INTERFACE_TYPE 列舉

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

### 錯誤碼常數

```csharp
public static int TP_SUCCESS = 0x0000;
public static int TP_ERR_COMMAND_NOT_SUPPORT = 0x0001;
public static int TP_ERR_DEVICE_BUSY = 0x0002;
public static int TP_ERR_IO_PENDING = 0x0003;
public static int TP_ERR_DATA_PATTEN = 0x0005;
public static int TP_ERR_CONNECT_NO_HELLO_PACKEY = 0x1002;
public static int TP_ERR_NOT_FOUND_DEVICE = 0x1004;
public static int TP_TESTMODE_GET_RAWDATA_FAIL = 0x3001;
public static int TP_ERR_CHK_MSG = 0xFFFF;
```

---

## TraceInfo 結構

```csharp
public const int MAX_CHIP_NUM = 4;

public struct TraceInfo
{
    public int nChipNum;         // IC 晶片數量（1-4）
    public int nXTotal;          // 總 RX 追蹤數
    public int nYTotal;          // 總 TX 追蹤數
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHIP_NUM)]
    public int[] XAxis;          // 各晶片 RX 分佈
    public int nPartialNum;      // 部份追蹤數

    public TraceInfo(int nMaxChipNum)
    public int GetRXTraceNum(TraceMode Mode)
    public int GetTXTraceNum(TraceMode Mode)
}
```

### TraceMode 列舉 (位元旗標)

```csharp
public enum TraceMode
{
    Mutual    = 0x01,    // 互電容
    Self      = 0x02,    // 自電容
    Partial   = 0x04,    // 部份追蹤
    ComboSelf = 0x08,    // 組合自電容
    Combo2Self = 0x10    // 雙自電容
}
```

### GetRXTraceNum 邏輯

| 條件 | 計算方式 |
|------|----------|
| Mutual + ChipNum=2 | `nXTotal - nPartialNum` |
| Mutual + ChipNum=3 | `nXTotal - (nPartialNum * 2)` |
| Mutual + 其他 | `nXTotal` |
| + Self | `+1`（或 Combo2Self → `+2`） |
| + Partial + ChipNum=2 | `+ nPartialNum` |
| + Partial + ChipNum=3 | `+ (nPartialNum * 2)` |

---

## Callback 機制

```csharp
// P/Invoke callback 委派（StdCall 呼叫慣例）
public delegate void PFUNC_OUT_REPORT_CALLBACK(IntPtr pReportBuffer, int nReportLen);
public delegate void PFUNC_IN_REPORT_CALLBACK(IntPtr pReportBuffer, int nReportLen);
public delegate void PFUNC_SOCKET_EVENT_CALLBACK(int nEventID);
```

---

## MPPPenAutoTuning 硬體抽象層 (FR-015, FR-016)

### IHardware 介面

```csharp
public interface IHardware
{
    bool Connect();
    bool Disconnect();
}
```

**實作者**:

| 實作類別 | 說明 |
|----------|------|
| `InputDevice` | HID 觸控裝置（對應 ElanTouch） |
| `RobotAPI` | 線測機 Robot XYZ 控制 |
| `GoDrawAPI` | 寫字機 GoDraw 控制 |

### IRS232Device 介面

```csharp
public interface IRS232Device
{
    bool Connect(string portName, int baudRate);
    bool Disconnect();
}
```

**實作者**:

| 實作類別 | 說明 |
|----------|------|
| `ForceGauge` | 力量計（壓力測量） |
| `LinearTable` | 線性表（線性度量測） |

---

## nICSolutionType 分支

```csharp
// ProcessFlow_CommonFunction.cs
private int m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_NONE;

// 條件判斷模式
if (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
{
    // 使用 ElanCommand_Gen8 的 WriteCommandInfo 結構
}
else
{
    // 使用 ElanCommand 的 ConvertCommandToByte()
}
```

---

## 連動檔案清單

| 操作 | 必須修改的檔案 |
|------|---------------|
| 修改 HID 介面 | `ElanTouch.cs` (兩個模組) |
| 修改 Socket 傳輸 | `ElanTouch_Socket.cs` (兩個模組) |
| 新增介面類型 | `ElanTouch.cs` (常數 + TP_INTERFACE_TYPE 列舉) |
| 修改 TraceInfo | `ElanTouch.cs` — 影響所有 AnalysisFlow 的 RX/TX 計算 |
| 修改硬體介面 | `Interface/IHardware.cs` + 所有實作者 |
