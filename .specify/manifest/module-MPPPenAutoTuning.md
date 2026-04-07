# Module Manifest: MPPPenAutoTuning

> MPP 觸控筆調校模組的類別、方法簽名與檔案對照表。
> 更新日期：2026-04-07 | 命名空間：`MPPPenAutoTuning`

---

## 1. 檔案清單

### 根目錄 (`MPPPenAutoTuning/MPPPenAutoTuning/`)

| 檔案 | 類別 | 說明 |
|------|------|------|
| `frmMain.cs` | `frmMain : Form` (partial) | 模組主視窗 |
| `frmMain.EventHandlers.cs` | `frmMain` (partial) | 事件處理 |
| `frmMain.Parameters.cs` | `frmMain` (partial) | 參數管理 |
| `ProcessFlow_MainFlow.cs` | `ProcessFlow` (partial) | 主流程入口 |
| `ProcessFlow_SingleMode.cs` | `ProcessFlow` (partial) | 單機模式 (Fake Robot) |
| `ProcessFlow_ServerMode.cs` | `ProcessFlow` (partial) | Server 端監聽模式 |
| `ProcessFlow_ClientMode.cs` | `ProcessFlow` (partial) | Client 端 Socket Robot |
| `ProcessFlow_GoDrawMode.cs` | `ProcessFlow` (partial) | GoDraw Robot 模式 |
| `ProcessFlow_LoadDataMode.cs` | `ProcessFlow` (partial) | 載入歷史資料模式 |
| `ProcessFlow_CommonFunction.cs` | `ProcessFlow` (partial) | 共用函式 |
| `ConnectFlow.cs` | `ConnectFlow` | 設備連線流程 |
| `ParameterBase.cs` | `ParameterBase` | 參數讀寫基類（INI/XML） |
| `ParameterProperties.cs` | — | 列舉定義 (MainTuningStep, SubTuningStep 等) |
| `RedmineProcess.cs` | — | Redmine 整合 |
| `clsListViewEdit.cs` | — | ListView 編輯輔助 |
| `Program.cs` | `Program` | 進入點 |

### 分析流程 (`AnalysisFlow/`)

| 檔案 | 類別 | 繼承自 | 對應步驟 |
|------|------|--------|---------|
| `AnalysisFlow_Raw.cs` | `AnalysisFlow` (base) + `ReferenceValue` | — | 基類 |
| `AnalysisFlow_Noise.cs` | `AnalysisFlow_Noise` | `AnalysisFlow` | NO (Hover/Contact) |
| `AnalysisFlow_Noise_Gen8.cs` | `AnalysisFlow_Noise_Gen8` | `AnalysisFlow_Noise` | NO (Gen8 IC) |
| `AnalysisFlow_Noise_TestMode.cs` | `AnalysisFlow_Noise_TestMode` | `AnalysisFlow` | NO (TestMode) |
| `AnalysisFlow_TiltNoise.cs` | `AnalysisFlow_TiltNoise` | `AnalysisFlow` | TILTNO |
| `AnalysisFlow_TiltNoise_Gen8.cs` | `AnalysisFlow_TiltNoise_Gen8` | `AnalysisFlow_TiltNoise` | TILTNO (Gen8) |
| `AnalysisFlow_DigiGainTuning.cs` | `AnalysisFlow_DigiGainTuning` | `AnalysisFlow` | DIGIGAINTUNING |
| `AnalysisFlow_TPGainTuning.cs` | `AnalysisFlow_TPGainTuning` | `AnalysisFlow` | TPGAINTUNING |
| `AnalysisFlow_PeakCheck.cs` | `AnalysisFlow_PeakCheck` | `AnalysisFlow` | PEAKCHECKTUNING |
| `AnalysisFlow_DTNormal.cs` | `AnalysisFlow_DTNormal` | `AnalysisFlow` | DIGITALTUNING (Normal) |
| `AnalysisFlow_DTTRxS.cs` | `AnalysisFlow_DTTRxS` | `AnalysisFlow` | DIGITALTUNING (TRxS) |
| `AnalysisFlow_TiltTuning.cs` | `AnalysisFlow_TiltTuning` | `AnalysisFlow` | TILTTUNING |
| `AnalysisFlow_PressureSetting.cs` | `AnalysisFlow_PressureSetting` | `AnalysisFlow` | PRESSURETUNING (Setting) |
| `AnalysisFlow_PressureProtect.cs` | `AnalysisFlow_PressureProtect` | `AnalysisFlow` | PRESSURETUNING (Protect) |
| `AnalysisFlow_PressureTable.cs` | `AnalysisFlow_PressureTable` | `AnalysisFlow` | PRESSURETUNING (Table) |
| `AnalysisFlow_LinearityTable.cs` | `AnalysisFlow_LinearityTable` | `AnalysisFlow` | LINEARITYTUNING |
| `AnalysisFlow_Else.cs` | `AnalysisFlow_Else` | `AnalysisFlow` | ELSE |

### 基礎設施 (`Class/`)

| 檔案 | 類別 | 說明 |
|------|------|------|
| `DataAnalysis.cs` | `DataAnalysis` | 分析流程分派器 |
| `ElanCommand.cs` | `ElanCommand` | 命令列舉與對應表 |
| `ElanCommand_Gen8.cs` | — | Gen8 特化命令 |
| `ElanTouch.cs` | `ElanTouch` | 觸控通訊驅動 |
| `ElanTouch_Old_1.cs` | — | 舊版通訊 (保留) |
| `ElanConvert.cs` | `ElanConvert` | 數值轉換 |
| `InputDevice.cs` | `InputDevice` (sealed) | Raw Input 管理 |
| `BlockingQueue.cs` | `BlockingQueue<T>` | 泛型阻塞佇列 |
| `MathMethod.cs` | — | 數學運算 |
| `StringConvert.cs` | — | 字串轉換 |
| `Win32API.cs` | — | Windows API P/Invoke |
| `LogAPI.cs` | `LogAPI` | 日誌記錄 |
| `FileProcess.cs` | — | 檔案處理 |
| `CheckFlowFile.cs` | — | 流程檔案驗證 |
| `RegisterValue.cs` | — | 暫存器值管理 |
| `RecordFlowInfo.cs` | — | 流程記錄 |
| `RecordSetInfo.cs` | — | 設定記錄 |
| `ComputeDFT_NUMAndCoefficient.cs` | — | DFT 計算 |
| `ServerSocketAPI.cs` | `ServerSocketObject` | Server Socket (已註解) |
| `SocketAPI.cs` | `SocketAPI` | Client Socket 通訊 |

### 硬體/API (`Class/HW/`, `Class/API/`, `Class/GetFrameData/`, `Class/Algorithm/`)

| 檔案 | 類別 | 說明 |
|------|------|------|
| `HW/HW_ForceGauge_SHIMPO_FGP05.cs` | `HW_ForceGauge_SHIMPO_FGP05` | SHIMPO 力規驅動 |
| `HW/HW_LT_DT_500F.cs` | `HW_LT_DT_500F` | LT 位移感測器驅動 |
| `API/RobotAPI.cs` | `RobotAPI` | 機械臂控制 |
| `API/RS232.cs` | — | RS232 串口 |
| `GetFrameData/GetFrameData.cs` | — | Frame 資料取得 |
| `GetFrameData/ElanDefine.cs` | `ElanDefine` | 全域常數 |
| `GetFrameData/FrameMgr.cs` | `FrameMgr` | Frame 管理 |
| `GetFrameData/UserInterfaceDefine.cs` | — | UI 定義 |
| `Algorithm/PressureAlgorithm.cs` | — | 壓力演算法 |
| `GetPressureTable.cs` | — | 壓力表取得 |

### 介面 (`Interface/`)

| 檔案 | 介面 | 說明 |
|------|------|------|
| `IHardware.cs` | `IHardware` | `Connect()`, `Disconnect()` |
| `IRS232Device.cs` | `IRS232Device` | `Connect()`, `Disconnect()`, `TestDevice()` |

---

## 2. 核心類別簽名

### frmMain（模組主視窗）

```csharp
public partial class frmMain : Form
{
    // 元件持有
    public RobotAPI m_cRobot;
    public InputDevice m_cInputDevice;
    public DataAnalysis m_cAnalysis;
    public ProcessFlow m_cProcessFlow;
    public SocketAPI m_cSocket;
    public LogAPI m_cDebugLog;
    public GoDrawAPI m_cGoDrawRobot;

    // 連線狀態
    public bool m_bRobotConnectFlag;
    public bool m_bDeviceConnectFlag;

    // 目錄路徑（依步驟區分）
    public string m_sFileDirectoryPath;
    public string m_sFileDirectoryPath_Noise;
    public string m_sFileDirectoryPath_TN;
    public string m_sFileDirectoryPath_DGT;
    public string m_sFileDirectoryPath_TPGT;
    public string m_sFileDirectoryPath_DT;

    // 常數
    public const string m_sAPMainDirectoryName = "MPP Pen";
}
```

### ProcessFlow（流程控制 — partial class，共 7 檔）

```csharp
public partial class ProcessFlow
{
    // === 主入口 (ProcessFlow_MainFlow.cs) ===
    public void RunMainProcessThread(object objParameter);
    private void RunListenThread();

    // === 單機模式 (ProcessFlow_SingleMode.cs) ===
    private void RunFakeRobotThread(object objRobotInfo);
    private void WaitGetDataComplete(RobotParameter, bool);
    private void WaitDrawFinish(RobotParameter);
    private bool CheckReportDataIsValid(...);

    // === Server 模式 (ProcessFlow_ServerMode.cs) ===
    private void RunServerListenFlow();
    private void RunServerListenThread();

    // === Client 模式 (ProcessFlow_ClientMode.cs) ===
    private void RunSocketRobotThread(object objRobotInfo);
    private void ComputeXYCoordByLTRobot(...);
    private void ComputeSpeedAndZCoordByLTRobot(...);
    private void WaitRobotStop(...);

    // === GoDraw 模式 (ProcessFlow_GoDrawMode.cs) ===
    // GoDraw 機器人流程

    // === 載入資料 (ProcessFlow_LoadDataMode.cs) ===
    // 歷史資料載入與重新分析

    // === 共用 (ProcessFlow_CommonFunction.cs) ===
    // 通用流程控制函式

    // 執行緒
    private ThreadStart m_tsMainThread;
    private Thread m_tMainThread;
    private ThreadStart m_tsListenThread;
    private Thread m_tListenThread;
    private bool m_bLastRetryFlag;
}
```

### ConnectFlow（連線流程）

```csharp
public class ConnectFlow
{
    public ConnectFlow(frmMain cfrmMain);

    // 巢狀類別
    public class FinishFlowParameter
    {
        public bool m_bErrorFlag;
        public bool m_bStateMessageFlag;
        public bool m_bShowMessageBoxFlag;
        public bool m_bConnectButtonEnableFlag;
        public SubTuningStep m_eSubStep;
        public void InitializeParameter();
    }

    // 執行緒
    private Thread m_tMainThread;
    private Thread m_tListenThread;
    private string m_sErrorMessage;
}
```

### DataAnalysis（分析流程分派器）

```csharp
public class DataAnalysis
{
    private frmMain m_cfrmMain;
    private AnalysisFlow m_cAnalysisFlow;
    public string m_sErrorMessage;
    public string m_sResultFilePath;

    public bool LoadData(FlowStep cFlowStep, int nICSolutionType, bool bDebugModeFlag = false);

    // 分派邏輯：
    // MainTuningStep.NO            → Noise / Noise_Gen8 / Noise_TestMode
    // MainTuningStep.TILTNO        → TiltNoise / TiltNoise_Gen8
    // MainTuningStep.DIGIGAINTUNING → DigiGainTuning
    // MainTuningStep.TPGAINTUNING  → TPGainTuning
    // MainTuningStep.PEAKCHECKTUNING → PeakCheck
    // MainTuningStep.DIGITALTUNING → DTNormal / DTTRxS
    // MainTuningStep.TILTTUNING    → TiltTuning
    // MainTuningStep.PRESSURETUNING → PressureSetting / PressureProtect / PressureTable
    // MainTuningStep.LINEARITYTUNING → LinearityTable
    // else                          → Else
}
```

### RobotAPI（機械臂控制）

```csharp
public class RobotAPI
{
    public enum RobotCode {
        Stop, ReturnOriginal, SetAbsoluteMove, GetPosition,
        SetLineAbsoluteMove, SetSpeed, IsMoving,
        SetWeight, GetWeight, SetWeightCalibration, SetAbsoluteZ
    }

    private HW_ForceGauge_SHIMPO_FGP05 m_cFG;  // 力規
    private HW_LT_DT_500F m_cLT;                // 位移感測器

    public void SetSocketRobot(RobotCode, ref string, ...);
}
```

### GoDrawAPI（GoDraw 機器人）

```csharp
public class GoDrawAPI
{
    private double m_dSpeed;
    private int m_nMaxCoordinateX, m_nMaxCoordinateY;
    private int m_nMaxServoValue, m_nMinServoValue;
    private int m_nTopServoValue, m_nHoverServoValue, m_nContactServoValue;
    private UdpClient m_ucUdpClient;
}
```

---

## 3. 關鍵列舉定義

```csharp
// ParameterProperties.cs
public enum MainTuningStep
{
    NO = 1, TILTNO = 2, DIGIGAINTUNING = 3,
    TPGAINTUNING = 4, PEAKCHECKTUNING = 5,
    DIGITALTUNING = 6, TILTTUNING = 7,
    PRESSURETUNING = 8, LINEARITYTUNING = 9,
    SERVERCONTRL = 10, ELSE = 11
}

public enum SubTuningStep
{
    NO = 0, HOVER_1ST = 1, HOVER_2ND = 2, CONTACT = 3,
    HOVERTRxS = 4, CONTACTTRxS = 5,
    TILTNO_PTHF = 6, TILTNO_BHF = 7,
    TILTTUNING_PTHF = 8, TILTTUNING_BHF = 9,
    PRESSURESETTING = 10, PRESSUREPROTECT = 11,
    PRESSURETABLE = 12, LINEARITYTABLE = 13,
    PCHOVER_1ST = 14, PCHOVER_2ND = 15, PCCONTACT = 16,
    DIGIGAIN = 17, TP_GAIN = 18, ELSE = 19
}

public enum FlowRobot
{
    NO, HOVERLINE, HOVERPOINT_CEN,
    TOUCHLINE, TOUCHLINE_HOR, TOUCHLINE_VER, TOUCHPOINT_CEN
}
```

---

## 4. ParameterBase

```csharp
public class ParameterBase
{
    protected const int m_nSTEP_NUMBER = 10;
    public const int m_nPRESSURE_DATA_NUMBER = 15;

    // 步驟名稱
    public static string[] m_sStepSettingNameSet_Array = {
        "Noise", "Tilt Noise", "DigiGain Tuning", "TP_Gain Tuning",
        "PeakCheck Tuning", "Digital Tuning", "Tilt Tuning",
        "Pressure Tuning", "Linearity Tuning", "Server Control"
    };

    public static MainTuningStep[] m_eMainTuningStepSet_Array = {
        NO, TILTNO, DIGIGAINTUNING, TPGAINTUNING,
        PEAKCHECKTUNING, DIGITALTUNING, TILTTUNING,
        PRESSURETUNING, LINEARITYTUNING, SERVERCONTRL
    };

    protected static string ReadValue(string sSection, string sKey, string sDefault);
    protected static void JudgeFileType(string sFileName);
}
```

---

## 5. 介面定義

```csharp
// Interface/IHardware.cs
namespace UserInterface
{
    interface IHardware { bool Connect(); bool Disconnect(); }
}

// Interface/IRS232Device.cs
namespace UserInterface
{
    interface IRS232Device { bool Connect(); bool Disconnect(); bool TestDevice(); }
}
```
