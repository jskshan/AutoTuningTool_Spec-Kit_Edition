# Module Manifest: FingerAutoTuning

> 手指觸控調校模組的類別、方法簽名與檔案對照表。
> 更新日期：2026-04-07 | 命名空間：`FingerAutoTuning`

---

## 1. 檔案清單

### 根目錄 (`FingerAutoTuning/FingerAutoTuning/`)

| 檔案 | 類別 | 說明 |
|------|------|------|
| `frmMain.cs` | `frmMain : Form` (partial) | 模組主視窗，管理流程、UI、狀態 |
| `AppCore.cs` | `AppCore` (partial) | 核心應用邏輯主檔 |
| `DataAnalysis.cs` | `DataAnalysis` | 分析流程分派器 |
| `DataSave.cs` | `SaveData` | 報告資料儲存（TXT） |
| `AnalysisFlow_Raw.cs` | `AnalysisFlow` (base) | 分析流程基類 |
| `AnalysisFlow_FRPH1.cs` | `AnalysisFlow_FRPH1 : AnalysisFlow` | 頻率排名第 1 階段 |
| `AnalysisFlow_FRPH2.cs` | `AnalysisFlow_FRPH2 : AnalysisFlow` | 頻率排名第 2 階段 |
| `AnalysisFlow_ACFR.cs` | `AnalysisFlow_ACFR : AnalysisFlow` | AC 頻率排名 |
| `AnalysisFlow_RawADCS.cs` | `AnalysisFlow_RawADCS : AnalysisFlow` | Raw ADC 掃描 |
| `AnalysisFlow_SelfFS.cs` | `AnalysisFlow_SelfFS : AnalysisFlow` | Self 頻率掃描 |
| `AnalysisFlow_SelfPNS.cs` | `AnalysisFlow_SelfPNS : AnalysisFlow` | Self NCP/NCN 掃描 |
| `ParamBase.cs` | `ParamBase` | 參數檔案讀寫基類 (INI/XML) |
| `ParamMgr.cs` | `ParamTestItemMgr` | 參數管理 (TestItem → Group → Item) |
| `ParamProperties.cs` | — | 參數屬性定義 |
| `clsElanXML.cs` | — | ELAN XML 操作 |
| `FreeDrawPatterncs.cs` | `FreeDrawPattern : TestPattern` | 自由繪製測試圖案 |
| `TestPattern.cs` | `ElanReport`, `PatternParamBase` | 測試圖案基類 |
| `LineBarCursorPattern.cs` | — | 線條/條形遊標圖案 |
| `RedmineProcess.cs` | — | Redmine 任務整合 |
| `Program.cs` | `Program` | 程式進入點（獨立測試用） |

### 表單 (`frm*.cs`)

| 檔案 | 類別 | 說明 |
|------|------|------|
| `frmFRPH1Chart.cs` | `frmFRPH1Chart : Form` | FRPH1 圖表 |
| `frmFRPH2Chart.cs` | `frmFRPH2Chart : Form` | FRPH2 圖表 |
| `frmACFRChart.cs` | `frmACFRChart : Form` | ACFR 圖表 |
| `frmSelfFSChart.cs` | `frmSelfFSChart : Form` | SelfFS 圖表 |
| `frmFeedback.cs` | `frmFeedback : Form` | 使用者回饋 |
| `frmFeedbackParameterSetting.cs` | `frmFeedbackParameterSetting : Form` | 回饋參數設定 |
| `frmFolderSelect.cs` | `frmFolderSelect : Form` | 資料夾選擇 |
| `frmFullScreen.cs` | `frmFullScreen : Form` | 全螢幕顯示 |
| `frmGetIAPAddress.cs` | `frmGetIAPAddress : Form` | IAP 位址取得 |
| `frmHIDDevBase.cs` | `frmHIDDevBase : Form` | HID 設備基類 |
| `frmMainEventItem.cs` | — | 主視窗事件項目 |
| `frmMultiAnalysis.cs` | `frmMultiAnalysis : Form` | 多重分析 |
| `frmParamSetting.cs` | `frmParamSetting : Form` | 參數設定 |
| `frmPattern.cs` | `frmPattern : Form` | 測試圖案 |
| `frmPatternMenu.cs` | `frmPatternMenu : Form` | 圖案選單 |
| `frmPHCKPattern.cs` | `frmPHCKPattern : Form` | PH 校驗圖案 |
| `frmRedmineTask.cs` | `frmRedmineTask : Form` | Redmine 任務 |
| `frmStepSetting.cs` | `frmStepSetting : Form` | 步驟設定 |
| `frmTestFreqSetting.cs` | `frmTestFreqSetting : Form` | 測試頻率設定 |
| `frmWarningMessage.cs` | `frmWarningMessage : Form` | 警告訊息 |

### 自訂控制項 (`ctrl*.cs`)

| 檔案 | 類別 | 說明 |
|------|------|------|
| `ctrlMAChart.cs` | `ctrlMAChart : UserControl` | 多軸圖表 |
| `ctrlMADataGridView.cs` | `ctrlMADataGridView : UserControl` | 資料網格 |
| `ctrlParamPage.cs` | `ctrlParamPage : UserControl` | 參數頁面 |
| `ctrlTestItemCheckBox.cs` | `ctrlTestItemCheckBox : UserControl` | 測試項目勾選 |

### AppCore 子模組 (`AppCore/`) — partial class AppCore

| 檔案 | 主要方法 | 說明 |
|------|---------|------|
| `ConnectDevice.cs` | `private bool ConnectToTP()` | 統一連線入口 |
| `ConnectWindowsDevice.cs` | `private bool ConnectWindowsDevice()` | Windows 設備連線 |
| `ConnectAndroidDevice.cs` | `private bool ConnectAndroidDevice()` | Android 設備連線 |
| `ConnectSSHSocket.cs` | `private bool ConnectSSHSocket()` | SSH Socket 連線 |
| `ConnectChromeRemoteClient.cs` | `private bool ConnectChromeRemoteClient()` | Chrome 遠端連線 |
| `GetFrameData.cs` | `private bool GetData_9F07(GetDataInfo)` | Frame 資料取得 |
| `GetFrequencyData.cs` | `private bool GetFreqeuncyItemListInfo(FlowStep)` | 頻率資料取得 |
| `GetFWParameter.cs` | `private bool GetFWParameter(FlowStep, bool, bool)` | 讀取韌體參數 |
| `GetHIDReport.cs` | `private bool RegistHIDDevice(FlowStep)` | 註冊 HID 設備 |
| `GetReportData.cs` | `private void GetReportData(int, string)` | 取得報告資料 |
| `GetAndSetFWOption.cs` | `private bool GetAndSetOptions(ref short, ref short, ref short)` | 取得/設定韌體選項 |
| `SetFWParameter.cs` | `private SetState SetFWParameter(FlowStep, FWParameter, int, ...)` | 設定韌體參數 |
| `SetAndGetFWParameter.cs` | `private bool SetAndGetFWParameter(FlowStep, FrequencyItem, RawADCSweepItem)` | 設定並讀回韌體參數 |
| `ExecuteCommand.cs` | `private bool GetRXTXTraceNumber(ref int, TraceType)` | 執行 ELAN 命令 |
| `RecordData.cs` | `private bool RecordData(FlowStep)` | 記錄/採集 Frame 資料 |
| `OutputResult.cs` | `private void SetOutputResult(int, bool)` | 輸出最終結果 |
| `CheckFWAnalogParameter.cs` | `private bool CheckAnalogParameter(FlowStep, bool, FrequencyItem, RawADCSweepItem)` | 類比參數校驗 |
| `ConvertHDF5Data.cs` | `private bool ConvertHDF5Data(string, string)` | 轉換 HDF5 資料 |
| `KeepAndroidWakeUp.cs` | `private void KeepAndroidWakeUp()` | 保持 Android 喚醒 |
| `LoadLogsAndEnsureRecordDir.cs` | `private bool CheckStepFolderExist(FlowStep)` | 確保記錄目錄 |
| `MoveLogData.cs` | `private bool MovePreviousData(FlowStep)` | 搬移歷史資料 |
| `ScreenSetting.cs` | `private void HidePattern()` | 螢幕/圖案控制 |

### 基礎設施 (`Class/`)

| 檔案 | 類別 | 說明 |
|------|------|------|
| `ElanDefine.cs` | `ElanDefine` | 全域常數（IC 類型、大小、協定） |
| `ElanCommand.cs` | `ElanCommand` | 命令列舉與對應表 |
| `ElanCommand_Gen8.cs` | — | Gen8 IC 特化命令 |
| `ElanTouch.cs` | `ElanTouch` | 觸控通訊驅動 (P/Invoke) |
| `ElanTouch_Socket.cs` | `ElanTouch_Socket` | Socket 通訊層 |
| `ElanSSHClient.cs` | `ElanSSHClient` | SSH 用戶端 |
| `ElanDirectTouch.cs` | — | DirectTouch 介面 |
| `ElanConvert.cs` | `ElanConvert` | 數值/浮點數轉換 |
| `FrameMgr.cs` | `FrameMgr` | Frame 管理與儲存 |
| `AppCoreDefine.cs` | `AppCoreDefine` | 列舉/結構定義 |
| `InputDevice.cs` | `InputDevice` (sealed) | Raw Input 設備管理 |
| `DebugLogAPI.cs` | `DebugLogAPI` | 偵錯日誌 |
| `BlockingQueue.cs` | `BlockingQueue<T>` | 泛型阻塞佇列 |
| `MathMethod.cs` | — | 數學運算 |
| `StringConvert.cs` | — | 字串轉換 |
| `Win32API.cs` | — | Windows API P/Invoke |
| `UserInterfaceDefine.cs` | — | UI 定義 |
| `IniFileFormat.cs` | — | INI 檔案讀寫 |

### 輔助 (`Helper/`)

| 檔案 | 說明 |
|------|------|
| `DGVHelper.cs` | DataGridView 輔助工具 |

---

## 2. 核心類別簽名

### frmMain（模組主視窗）

```csharp
// 定義於 frmMain.cs
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

public partial class frmMain : Form
{
    // 巢狀類別
    public class FlowStep { string Name; double CostTime; ... }

    // 元件持有
    private AppCore m_cAppCore;
    private InputDevice m_cInputDevice;
    private DebugLogAPI m_cDebugLog;
    private DataAnalysis m_cDataAnalysis;

    // 圖表表單
    private frmFRPH1Chart m_cfrmFRPH1Chart;
    private frmFRPH2Chart m_cfrmFRPH2Chart;
    private frmACFRChart m_cfrmACFRChart;
    private frmSelfFSChart m_cfrmSelfFSChart;

    // 流程控制
    public List<FlowStep> m_cFlowStep_List;
    public bool m_bExecute;
    public bool m_bReset;
    public bool m_bLoadData;
    public static bool m_bCollectFlowError;

    // 路徑
    public string m_sSettingFilePath;
    public string m_sDefaultFilePath;
    public static string m_sGen8FWParameterAddressIniPath;

    // 常數
    public const string m_sAPMainDirectoryName = "Finger";
}
```

### AppCore（核心邏輯 — partial class，共 23 檔）

```csharp
partial class AppCore
{
    // === 建構函式 (AppCore.cs) ===
    public AppCore(ref InputDevice, ref DebugLogAPI, ref DataAnalysis, frmMain);

    // === 主入口 (AppCore.cs) ===
    public void ExecuteMainWorkFlow(object objParameter);

    // === 連線 (AppCore/Connect*.cs) ===
    private bool ConnectToTP();
    private bool ConnectWindowsDevice();
    private bool ConnectAndroidDevice();
    private bool ConnectSSHSocket();
    private bool ConnectChromeRemoteClient();

    // === 資料取得 (AppCore/Get*.cs) ===
    private bool GetData_9F07(GetDataInfo);
    private bool GetFreqeuncyItemListInfo(FlowStep);
    private bool GetFWParameter(FlowStep, bool bGetOrigin, bool bNotDisableScanMode);
    private bool RegistHIDDevice(FlowStep);
    private void GetReportData(int nRetryIndex, string sTracePart);
    private bool GetAndSetOptions(ref short, ref short, ref short);

    // === 參數設定 (AppCore/Set*.cs) ===
    private SetState SetFWParameter(FlowStep, FWParameter, int nReKTimeout, ...);
    private bool SetAndGetFWParameter(FlowStep, FrequencyItem, RawADCSweepItem);

    // === 執行/記錄 (AppCore/*.cs) ===
    private bool GetRXTXTraceNumber(ref int, TraceType);
    private bool RecordData(FlowStep);
    private void SetOutputResult(int, bool);
    private bool CheckAnalogParameter(FlowStep, bool, FrequencyItem, RawADCSweepItem);
    private bool ConvertHDF5Data(string, string);
    private void KeepAndroidWakeUp();
    private bool CheckStepFolderExist(FlowStep);
    private bool MovePreviousData(FlowStep);
    private void HidePattern();

    // === 關鍵狀態欄位 ===
    public bool m_bFlowComplete;
    public string m_sErrorMessage;
    public string m_sStepName;
    public int m_nCurrentExecuteIndex;
}
```

### DataAnalysis（流程分派器）

```csharp
class DataAnalysis
{
    private AnalysisFlow m_cAnalysisFlowProcess;
    private frmMain m_cfrmParent;
    private MainStep m_eStep;

    public bool ExecuteMainWorkFlow(
        ref string sErrorMessage,
        frmMain.FlowStep cFlowStep,
        string sLogDirectoryPath,
        string sH5LogDirectoryPath,
        bool bGenerateH5Data,
        frmMain cfrmParent,
        string sProjectName,
        string sSkipFreqSetFilePath);
    // 內部根據 MainStep 分派至對應 AnalysisFlow_* 類別
}
```

### AnalysisFlow（分析流程基類）

```csharp
public class AnalysisFlow
{
    // 比較運算常數
    protected const int m_nCOMPARE_Frequency = 0;
    protected const int m_nCOMPARE_Normalize = 1;
    protected const int m_nCOMPARE_SetIndex = 2;
    protected const int m_nCOMPARE_RawADC50PCT = 3;

    // 巢狀型別
    public enum ReadDataType { Base, ADC, dV }
    public class FileCheckInfo { ... }

    // 欄位
    protected frmMain.FlowStep m_cFlowStep;
    protected frmMain m_cfrmParent;
    protected string m_sLogDirectoryPath;
    protected string m_sErrorMessage;
    protected List<string> m_sSourceData_List;

    // 虛擬方法（子類別覆寫）
    public virtual void InitializeParameter();
    public virtual void InitializeSourceDataList();
    public virtual void LoadAnalysisParameter();
    public virtual bool MainFlow(ref string sErrorMessage);

    // 公開方法
    public bool GetDataCount();
    public void SetErrorMessage(ref string);
    public void CopyDataToH5Directory();
}
```

### SaveData（資料儲存）

```csharp
public class SaveData
{
    protected const string m_sToolName = "FingerAutoTuningTool";
    public string ErrorMessage { get; }
    public string DataFilePath { get; }

    public SaveData(frmMain cfrmMain, string sProjectName);

    public bool CreateRecordData(
        SaveDataInfo cSaveDataInfo,
        string sLogDirectoryPath,
        string sDataType,
        int nRepeatIndex,
        bool bGetSignalData,
        bool bSetRepeatIndex = false,
        bool bFirstData = true);
}
```

---

## 3. 分析流程子類別

| 子類別 | 對應 MainStep | 特有巢狀型別 |
|--------|-------------|------------|
| `AnalysisFlow_FRPH1` | FrequencyRank_Phase1 | `DataInfo`, `SatnRegEdgePctLogFitData`, `ResultData` |
| `AnalysisFlow_FRPH2` | FrequencyRank_Phase2 | `DataInfo`, `StatisticData`, `RXTraceReference` |
| `AnalysisFlow_ACFR` | AC_FrequencyRank | `FilterMaskType` enum, `GetSignalAreaMethod`, `UseBaseDifferEdgeDetectMethod` |
| `AnalysisFlow_RawADCS` | Raw_ADC_Sweep | `DataInfo` (ADC Mean/Max/RawADC%/IQ_BSH) |
| `AnalysisFlow_SelfFS` | Self_FrequencySweep | K 值設定 (KP/KN), 多種 DataType |
| `AnalysisFlow_SelfPNS` | Self_NCPNCNSweep | K 值設定 (NCP/NCN), Raw P/N Mean/Std |

---

## 4. ELAN 通訊層

### ElanDefine（常數定義）

```csharp
public class ElanDefine
{
    // 尺寸
    public const int UNIT_1K = 1000;
    public const int SIZE_1K = 1024;
    public const int SIZE_1PAGE = 132;

    // 協定
    public const int ELAN_VID = 0x4f3;
    public const int DEFAULT_SOCKET_PORT = 9344;
    public const int FINGER_REPORT_ID = 0x01;
    public const int PEN_REPORT_ID = 0x07;

    // IC 類型
    public const int Gen39P_ICType = 391580;
    public const int Gen5M_ICType = 5015;
    public const int Gen63_ICType = 6315;
    public const int Gen7315_ICType = 7315;
    public const int Gen66_ICType = 6600;
    public const int Gen672_ICType = 6720;
    public const int Gen902_ICType = 36617;
    // ... 更多 IC 類型
}
```

### ElanTouch（通訊驅動）

```csharp
public class ElanTouch
{
    // 錯誤碼
    public static int TP_SUCCESS = 0x0000;
    public static int TP_ERR_DEVICE_BUSY = 0x0002;
    public static int TP_ERR_NOT_FOUND_DEVICE = 0x1004;
    public static int TP_TESTMODE_GET_RAWDATA_FAIL = 0x3001;

    // 介面類型
    public enum TP_INTERFACE_TYPE {
        IF_USB, IF_HID_OVER_I2C, IF_I2C,
        IF_SPI_MA_RISING_HALF_CYCLE, IF_SPI_MA_FALLING_HALF_CYCLE,
        IF_SPI_MA_RISING, IF_SPI_MA_FALLING, IF_SPI_PRECISE
    }

    // Callback 委派
    public delegate void PFUNC_OUT_REPORT_CALLBACK(IntPtr, int);
    public delegate void PFUNC_IN_REPORT_CALLBACK(IntPtr, int);
    public delegate void PFUNC_SOCKET_EVENT_CALLBACK(int);

    // TraceInfo 結構
    public struct TraceInfo {
        public int nChipNum;
        public int nXTotal;
        public int nYTotal;
        public int[] XAxis;   // [MAX_CHIP_NUM]
        public int nPartialNum;
        public int GetRXTraceNum(TraceMode);
    }
}
```

### ElanCommand（命令列舉）

```csharp
public class ElanCommand
{
    public enum ElanCommandType {
        NA, ResetIC, EnableICReport, DisableICReport,
        GetPH1, GetPH2, GetReportNumber, GetP0_TH,
        GetTRxS_Hover_TH_Rx, GetTRxS_Hover_TH_Tx,
        GetTRxS_Contact_TH_Rx, GetTRxS_Contact_TH_Tx,
        GetHover_TH_Rx, GetHover_TH_Tx,
        GetContact_TH_Rx, GetContact_TH_Tx,
        GetIQ_BSH_P, GetPressure3BinsTH,
        // Set 系列 ...
        SetPH1, SetPH2, SetReportNumber,
        // 更多 (50+ 個命令)
    }

    public enum ICValueTargetType {
        PH1, PH2, ReportNumber, P0_TH,
        TRxS_Hover_TH_Rx, TRxS_Hover_TH_Tx,
        Contact_TH_Rx, Contact_TH_Tx,
        // ... 更多
    }

    public static Dictionary<ICValueTargetType, ElanCommandType> dictSetCommandMappingTable;
}
```

---

## 5. 參數管理

### ParamBase

```csharp
public class ParamBase
{
    // 靜態路徑
    protected static string m_sSettingFilePath;
    protected static string m_sDefaultFilePath;
    public static bool m_bDataFormatError;

    // 步驟名稱常數
    public const string m_sStepName_FrequencyRank_Phase1 = "FrequencyRank Phase1";
    public const string m_sStepName_FrequencyRank_Phase2 = "FrequencyRank Phase2";
    public const string m_sStepName_AC_FrequencyRank = "AC FrequencyRank";
    public const string m_sStepName_Raw_ADC_Sweep = "Raw ADC Sweep";
    public const string m_sStepName_Self_FrequencySweep = "Self FrequencySweep";
    public const string m_sStepName_Self_NCPNCNSweep = "Self NCPNCNSweep";

    // INI/XML 讀寫
    protected static string ReadValue(string section, string key, string defaultValue);
    protected static void WriteValue(string section, string key, string value, bool, bool);
}
```

### ParamTestItemMgr

```csharp
// 階層：ParamTestItemMgr → ParamTestItem → ParamGroup → ParamItem
public enum ParamType { ADCRatio = 1, DiscMeanRatio = 2, Other = 3 }
public enum AccessMode { Admin = 1, ElanEngineer, Guest, None }

public class ParamTestItemMgr
{
    public ParamTestItemMgr(frmMain, string sParamFileName, string sDefaultFileName);
    public void Save(string sFileName);
    public ParamItem[] GetParamValue(string sTestItemName, string sGroupName, string sParamName);
}
```

---

## 6. AppCoreDefine（共用定義）

```csharp
// 命名空間: Elan
public enum MonitorState { MONITOR_ON = -1, MONITOR_OFF = 2, MONITOR_STANDBY = 1 }
public enum RecordState { NORMAL, ACFRFIRSTSTAGE, ACFRSECONDSTAGE }
public enum FileProcess { Move, Copy, Delete }

public class FWParameter
{
    public int PH1, PH2, PH3;
    public int MS_DFT_NUM, MS_IQ_BSH, MS_SELC;
    // ... 更多韌體參數欄位
}

public static bool FileProcessFlow(FileProcess, string source, string dest, bool overwrite);
```
