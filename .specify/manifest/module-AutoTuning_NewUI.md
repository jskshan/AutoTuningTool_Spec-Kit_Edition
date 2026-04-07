# Module Manifest: AutoTuning_NewUI

> 主程式入口（殼層）的類別、方法簽名與檔案對照表。
> 更新日期：2026-04-07 | 命名空間：`AutoTuning_NewUI`

---

## 1. 檔案清單

| 檔案 | 類別 | 說明 |
|------|------|------|
| `Program.cs` | `Program` (static) | 應用程式進入點 |
| `frmMain.cs` | `frmMain : Form` | 主視窗，管理子模組切換 |
| `frmMain.Designer.cs` | `frmMain` (partial) | 設計器自動產生 |
| `Properties/AssemblyInfo.cs` | — | 組件版本 2.0.0.0-beta1 |

---

## 2. 核心類別簽名

### Program

```csharp
static class Program
{
    [STAThread]
    static void Main();
    // 全域異常處理：CurrentDomain.UnhandledException
}
```

### frmMain（主視窗殼層）

```csharp
public partial class frmMain : Form
{
    // 子模組表單
    private FingerAutoTuning.frmMain m_cfrmFingerAutoTuning;
    private MPPPenAutoTuning.frmMain m_cfrmMPPPenAutoTuning;

    // 版本資訊
    private string m_sAPVersion;

    // 視窗類型列舉
    private enum WindowType { ... }
    private WindowType m_eWindowType;

    // 方法
    public frmMain();
    private void SetButtonImageSize();
    private DateTime RetrieveLinkerTimestamp();
    private DateTime GetBuildDate(Assembly assembly);
    private void SetTag(Control control);
    private void SetControls(float newX, float newY, Control control);

    // 條件編譯
    #if _USE_9F07_SOCKET
    // 隱藏 MPP Pen 按鈕，僅顯示 Finger 模組
    #endif

    // UI 按鈕
    private Button btnFinger;     // 切換至 FingerAutoTuning
    private Button btnMPPPen;     // 切換至 MPPPenAutoTuning
    private Button btnRead;       // 讀取功能
}
```

---

## 3. 組態與版本

| 項目 | 值 |
|------|-----|
| AssemblyVersion | 2.0.0.0 |
| FileVersion | 2.0.0.0 |
| InformationalVersion | 2.0.0.0-beta1 |
| OutputType | WinExe |
| 目標框架 | .NET Framework 4.8 |
| 平台目標 | x86 |

---

## 4. 職責說明

AutoTuning_NewUI 是純殼層專案，職責限於：

1. **模組切換**：透過按鈕在 FingerAutoTuning 和 MPPPenAutoTuning 之間切換
2. **版本管理**：顯示應用版本與建置時間
3. **全域異常處理**：捕捉未處理例外
4. **條件編譯**：`_USE_9F07_SOCKET` 時隱藏 MPP Pen 入口

所有實際調校邏輯、通訊、資料分析均委託給 FingerAutoTuning 與 MPPPenAutoTuning 模組。
