namespace MPPPenAutoTuning
{
    partial class frmParameterSetting
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmParameterSetting));
            this.tctrlMain = new System.Windows.Forms.TabControl();
            this.tpgNormal = new System.Windows.Forms.TabPage();
            this.gbxProjectInfo = new System.Windows.Forms.GroupBox();
            this.gbxConnectSetting = new System.Windows.Forms.GroupBox();
            this.gbxElanBridgeSetting = new System.Windows.Forms.GroupBox();
            this.cbxVIO = new System.Windows.Forms.ComboBox();
            this.lblVIO = new System.Windows.Forms.Label();
            this.cbxDVDD = new System.Windows.Forms.ComboBox();
            this.lblDVDD = new System.Windows.Forms.Label();
            this.lblI2CAddress = new System.Windows.Forms.Label();
            this.lblNormalLegth = new System.Windows.Forms.Label();
            this.tbxI2CAddress = new System.Windows.Forms.TextBox();
            this.tbxNormalLength = new System.Windows.Forms.TextBox();
            this.lblVID = new System.Windows.Forms.Label();
            this.tbxVID = new System.Windows.Forms.TextBox();
            this.lblPID = new System.Windows.Forms.Label();
            this.tbxPID = new System.Windows.Forms.TextBox();
            this.cbxFWType = new System.Windows.Forms.ComboBox();
            this.lblFWType = new System.Windows.Forms.Label();
            this.tbxProjectName = new System.Windows.Forms.TextBox();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.tpgProcess = new System.Windows.Forms.TabPage();
            this.gbxProcess = new System.Windows.Forms.GroupBox();
            this.gbxIndependentStepSetting = new System.Windows.Forms.GroupBox();
            this.lblFixedPH1 = new System.Windows.Forms.Label();
            this.tbxFixedPH1 = new System.Windows.Forms.TextBox();
            this.tbxFixedPH2 = new System.Windows.Forms.TextBox();
            this.lblFixedPH2 = new System.Windows.Forms.Label();
            this.lblNotIncludeMessage = new System.Windows.Forms.Label();
            this.cbxProcessType = new System.Windows.Forms.ComboBox();
            this.lblProcessType = new System.Windows.Forms.Label();
            this.tpgCoordinate = new System.Windows.Forms.TabPage();
            this.tcCoordinateSetting = new System.Windows.Forms.TabControl();
            this.tpgLTRobotCoordinate = new System.Windows.Forms.TabPage();
            this.gbxNormalCoordinate = new System.Windows.Forms.GroupBox();
            this.LblHoverHeight_PCT2nd = new System.Windows.Forms.Label();
            this.tbxHoverHeight_PCT2nd = new System.Windows.Forms.TextBox();
            this.LblHoverHeight_PCT1st = new System.Windows.Forms.Label();
            this.tbxHoverHeight_PCT1st = new System.Windows.Forms.TextBox();
            this.lblHoverHeight_DT2nd = new System.Windows.Forms.Label();
            this.tbxHoverHeight_DT2nd = new System.Windows.Forms.TextBox();
            this.lblHoverHeight_DT1st = new System.Windows.Forms.Label();
            this.tbxHoverHeight_DT1st = new System.Windows.Forms.TextBox();
            this.lblContactZCoord = new System.Windows.Forms.Label();
            this.lblEndYCoord = new System.Windows.Forms.Label();
            this.lblEndXCoord = new System.Windows.Forms.Label();
            this.lblEnd = new System.Windows.Forms.Label();
            this.lblStartYCoord = new System.Windows.Forms.Label();
            this.lblStartXCoord = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.tbxZ = new System.Windows.Forms.TextBox();
            this.tbxY2 = new System.Windows.Forms.TextBox();
            this.tbxX2 = new System.Windows.Forms.TextBox();
            this.tbxY1 = new System.Windows.Forms.TextBox();
            this.tbxX1 = new System.Windows.Forms.TextBox();
            this.gbxTPGTCoordinate = new System.Windows.Forms.GroupBox();
            this.gbxTPGTVerticalLine = new System.Windows.Forms.GroupBox();
            this.lblTPGTVerticalStart = new System.Windows.Forms.Label();
            this.tbxTPGTVerticalStartXCoord = new System.Windows.Forms.TextBox();
            this.lblTPGTVerticalEndYCoord = new System.Windows.Forms.Label();
            this.tbxTPGTVerticalStartYCoord = new System.Windows.Forms.TextBox();
            this.lblTPGTVerticalEndXCoord = new System.Windows.Forms.Label();
            this.tbxTPGTVerticalEndXCoord = new System.Windows.Forms.TextBox();
            this.lblTPGTVerticalEnd = new System.Windows.Forms.Label();
            this.tbxTPGTVerticalEndYCoord = new System.Windows.Forms.TextBox();
            this.lblTPGTVerticalStartYCoord = new System.Windows.Forms.Label();
            this.lblTPGTVerticalStartXCoord = new System.Windows.Forms.Label();
            this.gbxTPGTHorizontalLine = new System.Windows.Forms.GroupBox();
            this.lblTPGTHorizontalStart = new System.Windows.Forms.Label();
            this.tbxTPGTHorizontalStartXCoord = new System.Windows.Forms.TextBox();
            this.lblTPGTHorizontalEndYCoord = new System.Windows.Forms.Label();
            this.tbxTPGTHorizontalStartYCoord = new System.Windows.Forms.TextBox();
            this.lblTPGTHorizontalEndXCood = new System.Windows.Forms.Label();
            this.tbxTPGTHorizontalEndXCoord = new System.Windows.Forms.TextBox();
            this.lblTPGTHorizontalEnd = new System.Windows.Forms.Label();
            this.tbxTPGTHorizontalEndYCoord = new System.Windows.Forms.TextBox();
            this.lblTPGTHorizontalStartYCoord = new System.Windows.Forms.Label();
            this.lblTPGTHorizontalStartXCoord = new System.Windows.Forms.Label();
            this.lblTPGTContactZCoord = new System.Windows.Forms.Label();
            this.tbxTPGTContactZCoord = new System.Windows.Forms.TextBox();
            this.tpgGoDrawCoordinate = new System.Windows.Forms.TabPage();
            this.gbxGoDrawNormalCoordinate = new System.Windows.Forms.GroupBox();
            this.lblGoDrawTopZServoValue = new System.Windows.Forms.Label();
            this.tbxGoDrawTopZServoValue = new System.Windows.Forms.TextBox();
            this.lblGoDrawHoverZServoValue_PCT2nd = new System.Windows.Forms.Label();
            this.tbxGoDrawHoverZServoValue_PCT2nd = new System.Windows.Forms.TextBox();
            this.lblGoDrawHoverZServoValue_PCT1st = new System.Windows.Forms.Label();
            this.tbxGoDrawHoverZServoValue_PCT1st = new System.Windows.Forms.TextBox();
            this.lblGoDrawHoverZServoValue_DT2nd = new System.Windows.Forms.Label();
            this.tbxGoDrawHoverZServoValue_DT2nd = new System.Windows.Forms.TextBox();
            this.lblGoDrawHoverZServoValue_DT1st = new System.Windows.Forms.Label();
            this.tbxGoDrawHoverZServoValue_DT1st = new System.Windows.Forms.TextBox();
            this.lblGoDrawContactZServoValue = new System.Windows.Forms.Label();
            this.lblGoDrawEndYCoord = new System.Windows.Forms.Label();
            this.lblGoDrawEndXCoord = new System.Windows.Forms.Label();
            this.lblGoDrawEnd = new System.Windows.Forms.Label();
            this.lblGoDrawStartYCoord = new System.Windows.Forms.Label();
            this.lblGoDrawStartXCoord = new System.Windows.Forms.Label();
            this.lblGoDrawStart = new System.Windows.Forms.Label();
            this.tbxGoDrawContactZServoValue = new System.Windows.Forms.TextBox();
            this.tbxGoDrawEndYCoord = new System.Windows.Forms.TextBox();
            this.tbxGoDrawEndXCoord = new System.Windows.Forms.TextBox();
            this.tbxGoDrawStartYCoord = new System.Windows.Forms.TextBox();
            this.tbxGoDrawStartXCoord = new System.Windows.Forms.TextBox();
            this.gbxGoDrawTPGTCoordinate = new System.Windows.Forms.GroupBox();
            this.gbxGoDrawTPGTVerticalLine = new System.Windows.Forms.GroupBox();
            this.lblGoDrawTPGTVerticalStart = new System.Windows.Forms.Label();
            this.tbxGoDrawTPGTVerticalStartXCoord = new System.Windows.Forms.TextBox();
            this.lblGoDrawTPGTVerticalEndYCoord = new System.Windows.Forms.Label();
            this.tbxGoDrawTPGTVerticalStartYCoord = new System.Windows.Forms.TextBox();
            this.lblGoDrawTPGTVerticalEndXCoord = new System.Windows.Forms.Label();
            this.tbxGoDrawTPGTVerticalEndXCoord = new System.Windows.Forms.TextBox();
            this.lblGoDrawTPGTVerticalEnd = new System.Windows.Forms.Label();
            this.tbxGoDrawTPGTVerticalEndYCoord = new System.Windows.Forms.TextBox();
            this.lblGoDrawTPGTVerticalStartYCoord = new System.Windows.Forms.Label();
            this.lblGoDrawTPGTVerticalStartXCoord = new System.Windows.Forms.Label();
            this.gbxGoDrawTPGTHorizontalLine = new System.Windows.Forms.GroupBox();
            this.lblGoDrawTPGTHorizontalStart = new System.Windows.Forms.Label();
            this.tbxGoDrawTPGTHorizontalStartXCoord = new System.Windows.Forms.TextBox();
            this.lblGoDrawTPGTHorizontalEndYCoord = new System.Windows.Forms.Label();
            this.tbxGoDrawTPGTHorizontalStartYCoord = new System.Windows.Forms.TextBox();
            this.lblGoDrawTPGTHorizontalEndXCoord = new System.Windows.Forms.Label();
            this.tbxGoDrawTPGTHorizontalEndXCoord = new System.Windows.Forms.TextBox();
            this.lblGoDrawTPGTHorizontalEnd = new System.Windows.Forms.Label();
            this.tbxGoDrawTPGTHorizontalEndYCoord = new System.Windows.Forms.TextBox();
            this.lblGoDrawTPGTHorizontalStartYCoord = new System.Windows.Forms.Label();
            this.lblGoDrawTPGTHorizontalStartXCoord = new System.Windows.Forms.Label();
            this.lblGoDrawTPGTContactZServoValue = new System.Windows.Forms.Label();
            this.tbxGoDrawTPGTContactZServoValue = new System.Windows.Forms.TextBox();
            this.tpgSpeed = new System.Windows.Forms.TabPage();
            this.gbxSpeedSetting = new System.Windows.Forms.GroupBox();
            this.tbxTTSlantSpeed = new System.Windows.Forms.TextBox();
            this.lblTTSlantSpeed = new System.Windows.Forms.Label();
            this.lblTPGTSpeed = new System.Windows.Forms.Label();
            this.tbxTPGTSpeed = new System.Windows.Forms.TextBox();
            this.tbxLTSpeed = new System.Windows.Forms.TextBox();
            this.lblLTSpeed = new System.Windows.Forms.Label();
            this.lblPCTSpeed = new System.Windows.Forms.Label();
            this.tbxPCTSpeed = new System.Windows.Forms.TextBox();
            this.lblDGTSpeed = new System.Windows.Forms.Label();
            this.tbxDGTSpeed = new System.Windows.Forms.TextBox();
            this.tbxTTSpeed = new System.Windows.Forms.TextBox();
            this.lblTTSpeed = new System.Windows.Forms.Label();
            this.lblDTSpeed = new System.Windows.Forms.Label();
            this.tbxDTSpeed = new System.Windows.Forms.TextBox();
            this.tpgColorPattern = new System.Windows.Forms.TabPage();
            this.rbtnDisable = new System.Windows.Forms.RadioButton();
            this.rbtnMonitorOFF = new System.Windows.Forms.RadioButton();
            this.chbxDisplayTimeAndReportNumber = new System.Windows.Forms.CheckBox();
            this.chbxDisplayProgressStatus = new System.Windows.Forms.CheckBox();
            this.gbxColorPattern = new System.Windows.Forms.GroupBox();
            this.lblScreenSize = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.picbxManualPattern = new System.Windows.Forms.PictureBox();
            this.btnPHCKPatternOption = new System.Windows.Forms.Button();
            this.rbtnManual = new System.Windows.Forms.RadioButton();
            this.rbtnPHCKPattern = new System.Windows.Forms.RadioButton();
            this.tbxPatternPath = new System.Windows.Forms.TextBox();
            this.pnlDisplay = new System.Windows.Forms.Panel();
            this.btnColorSelect = new System.Windows.Forms.Button();
            this.btnPatternPath = new System.Windows.Forms.Button();
            this.cbxColorSelect = new System.Windows.Forms.ComboBox();
            this.rbtnPattern = new System.Windows.Forms.RadioButton();
            this.rbtnColor = new System.Windows.Forms.RadioButton();
            this.tpgGen8Command = new System.Windows.Forms.TabPage();
            this.gbxCommandSetting = new System.Windows.Forms.GroupBox();
            this.cbxGen8FilterType = new System.Windows.Forms.ComboBox();
            this.lblGen8FilterType = new System.Windows.Forms.Label();
            this.cbxGen8AFEType = new System.Windows.Forms.ComboBox();
            this.lblGen8AFEType = new System.Windows.Forms.Label();
            this.btnUserDefinedSelect = new System.Windows.Forms.Button();
            this.lblUserDefinedPath = new System.Windows.Forms.Label();
            this.tbxUserDeifnedPath = new System.Windows.Forms.TextBox();
            this.cbxCommandScriptType = new System.Windows.Forms.ComboBox();
            this.lblCommandScriptType = new System.Windows.Forms.Label();
            this.tpgOtherSetting = new System.Windows.Forms.TabPage();
            this.tctrlOtherSetting = new System.Windows.Forms.TabControl();
            this.tpgNoiseAndTNStep = new System.Windows.Forms.TabPage();
            this.tbxNoiseMaxValueOverWarningAbsValueHB = new System.Windows.Forms.TextBox();
            this.lblNoiseMaxValueOverWarningAbsValueHB = new System.Windows.Forms.Label();
            this.tbxNoiseNoReportInterruptTime = new System.Windows.Forms.TextBox();
            this.lblNoiseNoReportInterruptTime = new System.Windows.Forms.Label();
            this.tbxNoiseInnerRefValueHB = new System.Windows.Forms.TextBox();
            this.lblNoiseInnerRefValueHB = new System.Windows.Forms.Label();
            this.tbxNoiseProcessReportNumber = new System.Windows.Forms.TextBox();
            this.lblNoiseProcessReportNumber = new System.Windows.Forms.Label();
            this.tbxNoiseValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblNoiseValidReportNumber = new System.Windows.Forms.Label();
            this.tbxNoiseTimeout = new System.Windows.Forms.TextBox();
            this.lblNoiseTimeout = new System.Windows.Forms.Label();
            this.tbxNoiseReportNumber = new System.Windows.Forms.TextBox();
            this.lblNoiseReportNumber = new System.Windows.Forms.Label();
            this.tabDGTStep = new System.Windows.Forms.TabPage();
            this.tbxDGTDigiGainScaleHB = new System.Windows.Forms.TextBox();
            this.lblDGTDigiGainScaleHB = new System.Windows.Forms.Label();
            this.tbxDGTDigiGainScaleLB = new System.Windows.Forms.TextBox();
            this.lblDGTDigiGainScaleLB = new System.Windows.Forms.Label();
            this.tbxDGTTXValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblDGTTXValidReportNumber = new System.Windows.Forms.Label();
            this.tbxDGTRXValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblDGTRXValidReportNumber = new System.Windows.Forms.Label();
            this.tbxDGTCompensatePower = new System.Windows.Forms.TextBox();
            this.lblDGTCompensatePower = new System.Windows.Forms.Label();
            this.tpgTPGTStep = new System.Windows.Forms.TabPage();
            this.cbxTPGTDisplayMessage = new System.Windows.Forms.ComboBox();
            this.lblTPGTDisaplayMessage = new System.Windows.Forms.Label();
            this.tbxTPGTVerticalRAngle = new System.Windows.Forms.TextBox();
            this.lblTPGTVerticalRAngle = new System.Windows.Forms.Label();
            this.tbxTPGTHorizontalRAngle = new System.Windows.Forms.TextBox();
            this.lblTPGTHorizontalRAngle = new System.Windows.Forms.Label();
            this.tbxTPGTVAngle = new System.Windows.Forms.TextBox();
            this.lblTPGTVAngle = new System.Windows.Forms.Label();
            this.tbxTPGTGainRatio = new System.Windows.Forms.TextBox();
            this.lblTPGTGainRatio = new System.Windows.Forms.Label();
            this.tbxTPGTRXEndPin = new System.Windows.Forms.TextBox();
            this.lblTPGTRXEndPin = new System.Windows.Forms.Label();
            this.tbxTPGTRXStartPin = new System.Windows.Forms.TextBox();
            this.lblTPGTRXStartPin = new System.Windows.Forms.Label();
            this.tbxTPGTTXEndPin = new System.Windows.Forms.TextBox();
            this.lblTPGTTXEndPin = new System.Windows.Forms.Label();
            this.tbxTPGTTXStartPin = new System.Windows.Forms.TextBox();
            this.lblTPGTTXStartPin = new System.Windows.Forms.Label();
            this.tbxTPGTTXValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblTPGTTXValidReportNumber = new System.Windows.Forms.Label();
            this.tbxTPGTRXValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblTPGTRXValidReportNumber = new System.Windows.Forms.Label();
            this.tpgDTAndPCTStep = new System.Windows.Forms.TabPage();
            this.tbxNormal800to400PwrRatio = new System.Windows.Forms.TextBox();
            this.lblNormal800to400PwrRatio = new System.Windows.Forms.Label();
            this.tbxTRxSTXValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblTRxSTXValidReportNumber = new System.Windows.Forms.Label();
            this.tbxTRxSRXValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblTRxSRXValidReportNumber = new System.Windows.Forms.Label();
            this.tbxNormalFilterTXValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblNormalFilterTXValidReportNumber = new System.Windows.Forms.Label();
            this.tbxNormalFilterRXValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblNormalFilterRXValidReportNumber = new System.Windows.Forms.Label();
            this.tbxNormalValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblNormalValidReportNumber = new System.Windows.Forms.Label();
            this.tpgPCTStep = new System.Windows.Forms.TabPage();
            this.tbxPeakCheckRatio3T = new System.Windows.Forms.TextBox();
            this.lblPeakCheckRatio3T = new System.Windows.Forms.Label();
            this.tbxPeakCheckRatio5T = new System.Windows.Forms.TextBox();
            this.lblPeakCheckRatio5T = new System.Windows.Forms.Label();
            this.tbxPeakCheckRatio = new System.Windows.Forms.TextBox();
            this.lblPeakCheckRatio = new System.Windows.Forms.Label();
            this.tpgDigitalTuningStep = new System.Windows.Forms.TabPage();
            this.cbxDT7318TRxSSpecificReportType = new System.Windows.Forms.ComboBox();
            this.lblDT7318TRxSSpecificReportType = new System.Windows.Forms.Label();
            this.cbxDTSkipCompareThreshold = new System.Windows.Forms.ComboBox();
            this.lblDTSkipCompareThreshold = new System.Windows.Forms.Label();
            this.tbxDTThresholdRatio_TX = new System.Windows.Forms.TextBox();
            this.lblDTThresholdRatio_TX = new System.Windows.Forms.Label();
            this.tbxDTThresholdRatio_RX = new System.Windows.Forms.TextBox();
            this.lblDTThresholdRatio_RX = new System.Windows.Forms.Label();
            this.cbxDTDisplayChartDetailValue = new System.Windows.Forms.ComboBox();
            this.lblDTDisplayChartDetailValue = new System.Windows.Forms.Label();
            this.tbxNormalContactTHRatio_TX = new System.Windows.Forms.TextBox();
            this.lblNormalContactTHRatio_TX = new System.Windows.Forms.Label();
            this.tbxNormalContactTHRatio_RX = new System.Windows.Forms.TextBox();
            this.lblNormalContactTHRatio_RX = new System.Windows.Forms.Label();
            this.tbxNormalHoverTHRatio_TX = new System.Windows.Forms.TextBox();
            this.lblNormalHoverTHRatio_TX = new System.Windows.Forms.Label();
            this.tbxNormalHoverTHRatio_RX = new System.Windows.Forms.TextBox();
            this.lblNormalHoverTHRatio_RX = new System.Windows.Forms.Label();
            this.tpgTiltTuningStep = new System.Windows.Forms.TabPage();
            this.tbxTTTXValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblTTTXValidReportNumber = new System.Windows.Forms.Label();
            this.tbxTTRXValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblTTRXValidReportNumber = new System.Windows.Forms.Label();
            this.tbxTTValidTipTraceNumber = new System.Windows.Forms.TextBox();
            this.lblTTValidTipTraceNumber = new System.Windows.Forms.Label();
            this.tpgPTStep = new System.Windows.Forms.TabPage();
            this.cbxPTPenVersion = new System.Windows.Forms.ComboBox();
            this.lblPTPenVersion = new System.Windows.Forms.Label();
            this.tbxPTRecordTime = new System.Windows.Forms.TextBox();
            this.lblPTRecordTime = new System.Windows.Forms.Label();
            this.tbxPTEndSkipReportNumber = new System.Windows.Forms.TextBox();
            this.lblPTEndSkipReportNumber = new System.Windows.Forms.Label();
            this.tbxPTStartSkipReportNumber = new System.Windows.Forms.TextBox();
            this.lblPTStartSkipReportNumber = new System.Windows.Forms.Label();
            this.tbxPTValidReportNumber = new System.Windows.Forms.TextBox();
            this.lblPTValidReportNumber = new System.Windows.Forms.Label();
            this.tpgLTStep = new System.Windows.Forms.TabPage();
            this.cbxLTTP_GainCompensate = new System.Windows.Forms.ComboBox();
            this.lblLTTP_GainCompensate = new System.Windows.Forms.Label();
            this.tpgFWParameterSetting = new System.Windows.Forms.TabPage();
            this.tbxParameter_cActivePen_FM_P0_TH = new System.Windows.Forms.TextBox();
            this.lblParameter_cAcitvePen_FM_P0_TH = new System.Windows.Forms.Label();
            this.tpgOther = new System.Windows.Forms.TabPage();
            this.cbx5TRawDataType = new System.Windows.Forms.ComboBox();
            this.lbl5TRawDataType = new System.Windows.Forms.Label();
            this.cbxSetDigiGain = new System.Windows.Forms.ComboBox();
            this.lblSetDigiGain = new System.Windows.Forms.Label();
            this.tbxOtherFreqNumber = new System.Windows.Forms.TextBox();
            this.lblOtherFreqNumber = new System.Windows.Forms.Label();
            this.tbxTNFreqNumber = new System.Windows.Forms.TextBox();
            this.lblTNFreqNumber = new System.Windows.Forms.Label();
            this.cbxAutoTune_P0_detect_time = new System.Windows.Forms.ComboBox();
            this.lblAutoTune_P0_detect_time = new System.Windows.Forms.Label();
            this.tbxDrawLineTimeout = new System.Windows.Forms.TextBox();
            this.lblDrawLineTimeout = new System.Windows.Forms.Label();
            this.tbxRecordDataRetryCount = new System.Windows.Forms.TextBox();
            this.lblRecordDataRetryCount = new System.Windows.Forms.Label();
            this.tbxStartDelayTime = new System.Windows.Forms.TextBox();
            this.lblStartDelayTime = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.gbxNoiseMaxValueOverWarningHB = new System.Windows.Forms.GroupBox();
            this.lblNoiseMaxMinusMeanValueOverWarningStdevMagHB = new System.Windows.Forms.Label();
            this.tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB = new System.Windows.Forms.TextBox();
            this.tctrlMain.SuspendLayout();
            this.tpgNormal.SuspendLayout();
            this.gbxProjectInfo.SuspendLayout();
            this.gbxConnectSetting.SuspendLayout();
            this.gbxElanBridgeSetting.SuspendLayout();
            this.tpgProcess.SuspendLayout();
            this.gbxProcess.SuspendLayout();
            this.gbxIndependentStepSetting.SuspendLayout();
            this.tpgCoordinate.SuspendLayout();
            this.tcCoordinateSetting.SuspendLayout();
            this.tpgLTRobotCoordinate.SuspendLayout();
            this.gbxNormalCoordinate.SuspendLayout();
            this.gbxTPGTCoordinate.SuspendLayout();
            this.gbxTPGTVerticalLine.SuspendLayout();
            this.gbxTPGTHorizontalLine.SuspendLayout();
            this.tpgGoDrawCoordinate.SuspendLayout();
            this.gbxGoDrawNormalCoordinate.SuspendLayout();
            this.gbxGoDrawTPGTCoordinate.SuspendLayout();
            this.gbxGoDrawTPGTVerticalLine.SuspendLayout();
            this.gbxGoDrawTPGTHorizontalLine.SuspendLayout();
            this.tpgSpeed.SuspendLayout();
            this.gbxSpeedSetting.SuspendLayout();
            this.tpgColorPattern.SuspendLayout();
            this.gbxColorPattern.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxManualPattern)).BeginInit();
            this.tpgGen8Command.SuspendLayout();
            this.gbxCommandSetting.SuspendLayout();
            this.tpgOtherSetting.SuspendLayout();
            this.tctrlOtherSetting.SuspendLayout();
            this.tpgNoiseAndTNStep.SuspendLayout();
            this.tabDGTStep.SuspendLayout();
            this.tpgTPGTStep.SuspendLayout();
            this.tpgDTAndPCTStep.SuspendLayout();
            this.tpgPCTStep.SuspendLayout();
            this.tpgDigitalTuningStep.SuspendLayout();
            this.tpgTiltTuningStep.SuspendLayout();
            this.tpgPTStep.SuspendLayout();
            this.tpgLTStep.SuspendLayout();
            this.tpgFWParameterSetting.SuspendLayout();
            this.tpgOther.SuspendLayout();
            this.gbxNoiseMaxValueOverWarningHB.SuspendLayout();
            this.SuspendLayout();
            // 
            // tctrlMain
            // 
            this.tctrlMain.Controls.Add(this.tpgNormal);
            this.tctrlMain.Controls.Add(this.tpgProcess);
            this.tctrlMain.Controls.Add(this.tpgCoordinate);
            this.tctrlMain.Controls.Add(this.tpgSpeed);
            this.tctrlMain.Controls.Add(this.tpgColorPattern);
            this.tctrlMain.Controls.Add(this.tpgGen8Command);
            this.tctrlMain.Controls.Add(this.tpgOtherSetting);
            this.tctrlMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.tctrlMain.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tctrlMain.Location = new System.Drawing.Point(0, 0);
            this.tctrlMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tctrlMain.Name = "tctrlMain";
            this.tctrlMain.SelectedIndex = 0;
            this.tctrlMain.Size = new System.Drawing.Size(886, 496);
            this.tctrlMain.TabIndex = 18;
            this.tctrlMain.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tctrlMain_Selecting);
            // 
            // tpgNormal
            // 
            this.tpgNormal.BackColor = System.Drawing.SystemColors.Control;
            this.tpgNormal.Controls.Add(this.gbxProjectInfo);
            this.tpgNormal.Location = new System.Drawing.Point(4, 26);
            this.tpgNormal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tpgNormal.Name = "tpgNormal";
            this.tpgNormal.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tpgNormal.Size = new System.Drawing.Size(878, 466);
            this.tpgNormal.TabIndex = 0;
            this.tpgNormal.Text = "Normal Setting";
            // 
            // gbxProjectInfo
            // 
            this.gbxProjectInfo.Controls.Add(this.gbxConnectSetting);
            this.gbxProjectInfo.Controls.Add(this.cbxFWType);
            this.gbxProjectInfo.Controls.Add(this.lblFWType);
            this.gbxProjectInfo.Controls.Add(this.tbxProjectName);
            this.gbxProjectInfo.Controls.Add(this.lblProjectName);
            this.gbxProjectInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxProjectInfo.Location = new System.Drawing.Point(3, 4);
            this.gbxProjectInfo.Name = "gbxProjectInfo";
            this.gbxProjectInfo.Size = new System.Drawing.Size(872, 458);
            this.gbxProjectInfo.TabIndex = 10;
            this.gbxProjectInfo.TabStop = false;
            this.gbxProjectInfo.Text = "Project Information Setting";
            // 
            // gbxConnectSetting
            // 
            this.gbxConnectSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxConnectSetting.Controls.Add(this.gbxElanBridgeSetting);
            this.gbxConnectSetting.Controls.Add(this.lblVID);
            this.gbxConnectSetting.Controls.Add(this.tbxVID);
            this.gbxConnectSetting.Controls.Add(this.lblPID);
            this.gbxConnectSetting.Controls.Add(this.tbxPID);
            this.gbxConnectSetting.Location = new System.Drawing.Point(3, 94);
            this.gbxConnectSetting.Name = "gbxConnectSetting";
            this.gbxConnectSetting.Size = new System.Drawing.Size(863, 358);
            this.gbxConnectSetting.TabIndex = 21;
            this.gbxConnectSetting.TabStop = false;
            this.gbxConnectSetting.Text = "Connect Setting";
            // 
            // gbxElanBridgeSetting
            // 
            this.gbxElanBridgeSetting.Controls.Add(this.cbxVIO);
            this.gbxElanBridgeSetting.Controls.Add(this.lblVIO);
            this.gbxElanBridgeSetting.Controls.Add(this.cbxDVDD);
            this.gbxElanBridgeSetting.Controls.Add(this.lblDVDD);
            this.gbxElanBridgeSetting.Controls.Add(this.lblI2CAddress);
            this.gbxElanBridgeSetting.Controls.Add(this.lblNormalLegth);
            this.gbxElanBridgeSetting.Controls.Add(this.tbxI2CAddress);
            this.gbxElanBridgeSetting.Controls.Add(this.tbxNormalLength);
            this.gbxElanBridgeSetting.Location = new System.Drawing.Point(10, 94);
            this.gbxElanBridgeSetting.Name = "gbxElanBridgeSetting";
            this.gbxElanBridgeSetting.Size = new System.Drawing.Size(419, 144);
            this.gbxElanBridgeSetting.TabIndex = 31;
            this.gbxElanBridgeSetting.TabStop = false;
            this.gbxElanBridgeSetting.Text = "Elan Bridge Setting";
            // 
            // cbxVIO
            // 
            this.cbxVIO.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxVIO.FormattingEnabled = true;
            this.cbxVIO.Items.AddRange(new object[] {
            "1.8V",
            "2.8V",
            "3.0V",
            "3.3V",
            "5.0V"});
            this.cbxVIO.Location = new System.Drawing.Point(270, 32);
            this.cbxVIO.Name = "cbxVIO";
            this.cbxVIO.Size = new System.Drawing.Size(109, 25);
            this.cbxVIO.TabIndex = 34;
            // 
            // lblVIO
            // 
            this.lblVIO.AutoSize = true;
            this.lblVIO.Location = new System.Drawing.Point(212, 36);
            this.lblVIO.Name = "lblVIO";
            this.lblVIO.Size = new System.Drawing.Size(42, 17);
            this.lblVIO.TabIndex = 33;
            this.lblVIO.Text = "VIO :";
            // 
            // cbxDVDD
            // 
            this.cbxDVDD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDVDD.FormattingEnabled = true;
            this.cbxDVDD.Items.AddRange(new object[] {
            "2.8V",
            "3.0V",
            "3.3V",
            "5.0V"});
            this.cbxDVDD.Location = new System.Drawing.Point(79, 31);
            this.cbxDVDD.Name = "cbxDVDD";
            this.cbxDVDD.Size = new System.Drawing.Size(109, 25);
            this.cbxDVDD.TabIndex = 32;
            // 
            // lblDVDD
            // 
            this.lblDVDD.AutoSize = true;
            this.lblDVDD.Location = new System.Drawing.Point(6, 35);
            this.lblDVDD.Name = "lblDVDD";
            this.lblDVDD.Size = new System.Drawing.Size(59, 17);
            this.lblDVDD.TabIndex = 31;
            this.lblDVDD.Text = "DVDD :";
            // 
            // lblI2CAddress
            // 
            this.lblI2CAddress.AutoSize = true;
            this.lblI2CAddress.Location = new System.Drawing.Point(6, 77);
            this.lblI2CAddress.Name = "lblI2CAddress";
            this.lblI2CAddress.Size = new System.Drawing.Size(128, 17);
            this.lblI2CAddress.TabIndex = 27;
            this.lblI2CAddress.Text = "I2C Address (Hex) :";
            // 
            // lblNormalLegth
            // 
            this.lblNormalLegth.AutoSize = true;
            this.lblNormalLegth.Location = new System.Drawing.Point(6, 112);
            this.lblNormalLegth.Name = "lblNormalLegth";
            this.lblNormalLegth.Size = new System.Drawing.Size(136, 17);
            this.lblNormalLegth.TabIndex = 29;
            this.lblNormalLegth.Text = "Normal Legth (Hex) :";
            // 
            // tbxI2CAddress
            // 
            this.tbxI2CAddress.Location = new System.Drawing.Point(153, 73);
            this.tbxI2CAddress.Name = "tbxI2CAddress";
            this.tbxI2CAddress.Size = new System.Drawing.Size(64, 25);
            this.tbxI2CAddress.TabIndex = 28;
            this.tbxI2CAddress.Text = "20";
            // 
            // tbxNormalLength
            // 
            this.tbxNormalLength.Location = new System.Drawing.Point(153, 108);
            this.tbxNormalLength.Name = "tbxNormalLength";
            this.tbxNormalLength.Size = new System.Drawing.Size(64, 25);
            this.tbxNormalLength.TabIndex = 30;
            this.tbxNormalLength.Text = "3F";
            // 
            // lblVID
            // 
            this.lblVID.AutoSize = true;
            this.lblVID.Location = new System.Drawing.Point(7, 33);
            this.lblVID.Name = "lblVID";
            this.lblVID.Size = new System.Drawing.Size(81, 17);
            this.lblVID.TabIndex = 25;
            this.lblVID.Text = "VID (Hex) :";
            // 
            // tbxVID
            // 
            this.tbxVID.Location = new System.Drawing.Point(95, 30);
            this.tbxVID.Name = "tbxVID";
            this.tbxVID.Size = new System.Drawing.Size(81, 25);
            this.tbxVID.TabIndex = 26;
            this.tbxVID.Text = "04F3";
            // 
            // lblPID
            // 
            this.lblPID.AutoSize = true;
            this.lblPID.Location = new System.Drawing.Point(7, 67);
            this.lblPID.Name = "lblPID";
            this.lblPID.Size = new System.Drawing.Size(79, 17);
            this.lblPID.TabIndex = 23;
            this.lblPID.Text = "PID (Hex) :";
            // 
            // tbxPID
            // 
            this.tbxPID.Location = new System.Drawing.Point(95, 63);
            this.tbxPID.Name = "tbxPID";
            this.tbxPID.Size = new System.Drawing.Size(81, 25);
            this.tbxPID.TabIndex = 24;
            this.tbxPID.Text = "0";
            // 
            // cbxFWType
            // 
            this.cbxFWType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFWType.FormattingEnabled = true;
            this.cbxFWType.Items.AddRange(new object[] {
            "Normal FW",
            "AutoTuning FW"});
            this.cbxFWType.Location = new System.Drawing.Point(81, 63);
            this.cbxFWType.Name = "cbxFWType";
            this.cbxFWType.Size = new System.Drawing.Size(139, 25);
            this.cbxFWType.TabIndex = 20;
            // 
            // lblFWType
            // 
            this.lblFWType.AutoSize = true;
            this.lblFWType.Location = new System.Drawing.Point(6, 67);
            this.lblFWType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFWType.Name = "lblFWType";
            this.lblFWType.Size = new System.Drawing.Size(70, 17);
            this.lblFWType.TabIndex = 19;
            this.lblFWType.Text = "FW Type :";
            // 
            // tbxProjectName
            // 
            this.tbxProjectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxProjectName.Location = new System.Drawing.Point(111, 23);
            this.tbxProjectName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxProjectName.Name = "tbxProjectName";
            this.tbxProjectName.Size = new System.Drawing.Size(741, 25);
            this.tbxProjectName.TabIndex = 18;
            // 
            // lblProjectName
            // 
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Location = new System.Drawing.Point(6, 25);
            this.lblProjectName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(97, 17);
            this.lblProjectName.TabIndex = 17;
            this.lblProjectName.Text = "Project Name :";
            // 
            // tpgProcess
            // 
            this.tpgProcess.BackColor = System.Drawing.SystemColors.Control;
            this.tpgProcess.Controls.Add(this.gbxProcess);
            this.tpgProcess.Location = new System.Drawing.Point(4, 26);
            this.tpgProcess.Margin = new System.Windows.Forms.Padding(2);
            this.tpgProcess.Name = "tpgProcess";
            this.tpgProcess.Size = new System.Drawing.Size(878, 466);
            this.tpgProcess.TabIndex = 4;
            this.tpgProcess.Text = "Process Setting";
            // 
            // gbxProcess
            // 
            this.gbxProcess.Controls.Add(this.gbxIndependentStepSetting);
            this.gbxProcess.Controls.Add(this.lblNotIncludeMessage);
            this.gbxProcess.Controls.Add(this.cbxProcessType);
            this.gbxProcess.Controls.Add(this.lblProcessType);
            this.gbxProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxProcess.Location = new System.Drawing.Point(0, 0);
            this.gbxProcess.Name = "gbxProcess";
            this.gbxProcess.Size = new System.Drawing.Size(878, 466);
            this.gbxProcess.TabIndex = 11;
            this.gbxProcess.TabStop = false;
            this.gbxProcess.Text = "Process Setting";
            // 
            // gbxIndependentStepSetting
            // 
            this.gbxIndependentStepSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxIndependentStepSetting.Controls.Add(this.lblFixedPH1);
            this.gbxIndependentStepSetting.Controls.Add(this.tbxFixedPH1);
            this.gbxIndependentStepSetting.Controls.Add(this.tbxFixedPH2);
            this.gbxIndependentStepSetting.Controls.Add(this.lblFixedPH2);
            this.gbxIndependentStepSetting.Location = new System.Drawing.Point(10, 55);
            this.gbxIndependentStepSetting.Margin = new System.Windows.Forms.Padding(2);
            this.gbxIndependentStepSetting.Name = "gbxIndependentStepSetting";
            this.gbxIndependentStepSetting.Padding = new System.Windows.Forms.Padding(2);
            this.gbxIndependentStepSetting.Size = new System.Drawing.Size(404, 405);
            this.gbxIndependentStepSetting.TabIndex = 26;
            this.gbxIndependentStepSetting.TabStop = false;
            this.gbxIndependentStepSetting.Text = "Independent Setting";
            // 
            // lblFixedPH1
            // 
            this.lblFixedPH1.AutoSize = true;
            this.lblFixedPH1.Location = new System.Drawing.Point(5, 30);
            this.lblFixedPH1.Name = "lblFixedPH1";
            this.lblFixedPH1.Size = new System.Drawing.Size(78, 17);
            this.lblFixedPH1.TabIndex = 21;
            this.lblFixedPH1.Text = "Fixed PH1 :";
            // 
            // tbxFixedPH1
            // 
            this.tbxFixedPH1.Location = new System.Drawing.Point(112, 27);
            this.tbxFixedPH1.Name = "tbxFixedPH1";
            this.tbxFixedPH1.Size = new System.Drawing.Size(81, 25);
            this.tbxFixedPH1.TabIndex = 22;
            this.tbxFixedPH1.Text = "0";
            // 
            // tbxFixedPH2
            // 
            this.tbxFixedPH2.Location = new System.Drawing.Point(112, 58);
            this.tbxFixedPH2.Name = "tbxFixedPH2";
            this.tbxFixedPH2.Size = new System.Drawing.Size(81, 25);
            this.tbxFixedPH2.TabIndex = 24;
            this.tbxFixedPH2.Text = "0";
            // 
            // lblFixedPH2
            // 
            this.lblFixedPH2.AutoSize = true;
            this.lblFixedPH2.Location = new System.Drawing.Point(5, 60);
            this.lblFixedPH2.Name = "lblFixedPH2";
            this.lblFixedPH2.Size = new System.Drawing.Size(78, 17);
            this.lblFixedPH2.TabIndex = 23;
            this.lblFixedPH2.Text = "Fixed PH2 :";
            // 
            // lblNotIncludeMessage
            // 
            this.lblNotIncludeMessage.AutoSize = true;
            this.lblNotIncludeMessage.Location = new System.Drawing.Point(317, 30);
            this.lblNotIncludeMessage.Name = "lblNotIncludeMessage";
            this.lblNotIncludeMessage.Size = new System.Drawing.Size(416, 17);
            this.lblNotIncludeMessage.TabIndex = 25;
            this.lblNotIncludeMessage.Text = "(Not Include \"Noise\", \"PeakCheck Tuning\" and \"Digitial Tuning\" Step)";
            // 
            // cbxProcessType
            // 
            this.cbxProcessType.AutoCompleteCustomSource.AddRange(new string[] {
            "0 [100 - (Value/Max)]",
            "1 [N/A]"});
            this.cbxProcessType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProcessType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxProcessType.FormattingEnabled = true;
            this.cbxProcessType.Items.AddRange(new object[] {
            "Normal",
            "Independent Step"});
            this.cbxProcessType.Location = new System.Drawing.Point(114, 26);
            this.cbxProcessType.Name = "cbxProcessType";
            this.cbxProcessType.Size = new System.Drawing.Size(193, 25);
            this.cbxProcessType.TabIndex = 20;
            // 
            // lblProcessType
            // 
            this.lblProcessType.AutoSize = true;
            this.lblProcessType.Location = new System.Drawing.Point(7, 30);
            this.lblProcessType.Name = "lblProcessType";
            this.lblProcessType.Size = new System.Drawing.Size(95, 17);
            this.lblProcessType.TabIndex = 19;
            this.lblProcessType.Text = "Process Type :";
            // 
            // tpgCoordinate
            // 
            this.tpgCoordinate.BackColor = System.Drawing.SystemColors.Control;
            this.tpgCoordinate.Controls.Add(this.tcCoordinateSetting);
            this.tpgCoordinate.Location = new System.Drawing.Point(4, 26);
            this.tpgCoordinate.Name = "tpgCoordinate";
            this.tpgCoordinate.Size = new System.Drawing.Size(878, 466);
            this.tpgCoordinate.TabIndex = 3;
            this.tpgCoordinate.Text = "Coordinate Setting";
            // 
            // tcCoordinateSetting
            // 
            this.tcCoordinateSetting.Controls.Add(this.tpgLTRobotCoordinate);
            this.tcCoordinateSetting.Controls.Add(this.tpgGoDrawCoordinate);
            this.tcCoordinateSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcCoordinateSetting.Location = new System.Drawing.Point(0, 0);
            this.tcCoordinateSetting.Margin = new System.Windows.Forms.Padding(2);
            this.tcCoordinateSetting.Name = "tcCoordinateSetting";
            this.tcCoordinateSetting.SelectedIndex = 0;
            this.tcCoordinateSetting.Size = new System.Drawing.Size(878, 466);
            this.tcCoordinateSetting.TabIndex = 12;
            // 
            // tpgLTRobotCoordinate
            // 
            this.tpgLTRobotCoordinate.BackColor = System.Drawing.SystemColors.Control;
            this.tpgLTRobotCoordinate.Controls.Add(this.gbxNormalCoordinate);
            this.tpgLTRobotCoordinate.Controls.Add(this.gbxTPGTCoordinate);
            this.tpgLTRobotCoordinate.Location = new System.Drawing.Point(4, 26);
            this.tpgLTRobotCoordinate.Margin = new System.Windows.Forms.Padding(2);
            this.tpgLTRobotCoordinate.Name = "tpgLTRobotCoordinate";
            this.tpgLTRobotCoordinate.Padding = new System.Windows.Forms.Padding(2);
            this.tpgLTRobotCoordinate.Size = new System.Drawing.Size(870, 436);
            this.tpgLTRobotCoordinate.TabIndex = 0;
            this.tpgLTRobotCoordinate.Text = "LT Robot Coordinate";
            // 
            // gbxNormalCoordinate
            // 
            this.gbxNormalCoordinate.Controls.Add(this.LblHoverHeight_PCT2nd);
            this.gbxNormalCoordinate.Controls.Add(this.tbxHoverHeight_PCT2nd);
            this.gbxNormalCoordinate.Controls.Add(this.LblHoverHeight_PCT1st);
            this.gbxNormalCoordinate.Controls.Add(this.tbxHoverHeight_PCT1st);
            this.gbxNormalCoordinate.Controls.Add(this.lblHoverHeight_DT2nd);
            this.gbxNormalCoordinate.Controls.Add(this.tbxHoverHeight_DT2nd);
            this.gbxNormalCoordinate.Controls.Add(this.lblHoverHeight_DT1st);
            this.gbxNormalCoordinate.Controls.Add(this.tbxHoverHeight_DT1st);
            this.gbxNormalCoordinate.Controls.Add(this.lblContactZCoord);
            this.gbxNormalCoordinate.Controls.Add(this.lblEndYCoord);
            this.gbxNormalCoordinate.Controls.Add(this.lblEndXCoord);
            this.gbxNormalCoordinate.Controls.Add(this.lblEnd);
            this.gbxNormalCoordinate.Controls.Add(this.lblStartYCoord);
            this.gbxNormalCoordinate.Controls.Add(this.lblStartXCoord);
            this.gbxNormalCoordinate.Controls.Add(this.lblStart);
            this.gbxNormalCoordinate.Controls.Add(this.tbxZ);
            this.gbxNormalCoordinate.Controls.Add(this.tbxY2);
            this.gbxNormalCoordinate.Controls.Add(this.tbxX2);
            this.gbxNormalCoordinate.Controls.Add(this.tbxY1);
            this.gbxNormalCoordinate.Controls.Add(this.tbxX1);
            this.gbxNormalCoordinate.Location = new System.Drawing.Point(5, 6);
            this.gbxNormalCoordinate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxNormalCoordinate.Name = "gbxNormalCoordinate";
            this.gbxNormalCoordinate.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxNormalCoordinate.Size = new System.Drawing.Size(358, 279);
            this.gbxNormalCoordinate.TabIndex = 9;
            this.gbxNormalCoordinate.TabStop = false;
            this.gbxNormalCoordinate.Text = "Normal Coordinate";
            // 
            // LblHoverHeight_PCT2nd
            // 
            this.LblHoverHeight_PCT2nd.AutoSize = true;
            this.LblHoverHeight_PCT2nd.Location = new System.Drawing.Point(3, 214);
            this.LblHoverHeight_PCT2nd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblHoverHeight_PCT2nd.Name = "LblHoverHeight_PCT2nd";
            this.LblHoverHeight_PCT2nd.Size = new System.Drawing.Size(274, 17);
            this.LblHoverHeight_PCT2nd.TabIndex = 33;
            this.LblHoverHeight_PCT2nd.Text = "Hover Height(PeakCheckTuning Hover2nd) :";
            // 
            // tbxHoverHeight_PCT2nd
            // 
            this.tbxHoverHeight_PCT2nd.Location = new System.Drawing.Point(278, 211);
            this.tbxHoverHeight_PCT2nd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxHoverHeight_PCT2nd.Name = "tbxHoverHeight_PCT2nd";
            this.tbxHoverHeight_PCT2nd.Size = new System.Drawing.Size(52, 25);
            this.tbxHoverHeight_PCT2nd.TabIndex = 32;
            this.tbxHoverHeight_PCT2nd.Text = "2.00";
            // 
            // LblHoverHeight_PCT1st
            // 
            this.LblHoverHeight_PCT1st.AutoSize = true;
            this.LblHoverHeight_PCT1st.Location = new System.Drawing.Point(3, 185);
            this.LblHoverHeight_PCT1st.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblHoverHeight_PCT1st.Name = "LblHoverHeight_PCT1st";
            this.LblHoverHeight_PCT1st.Size = new System.Drawing.Size(270, 17);
            this.LblHoverHeight_PCT1st.TabIndex = 31;
            this.LblHoverHeight_PCT1st.Text = "Hover Height(PeakCheckTuning Hover1st) :";
            // 
            // tbxHoverHeight_PCT1st
            // 
            this.tbxHoverHeight_PCT1st.Location = new System.Drawing.Point(278, 182);
            this.tbxHoverHeight_PCT1st.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxHoverHeight_PCT1st.Name = "tbxHoverHeight_PCT1st";
            this.tbxHoverHeight_PCT1st.Size = new System.Drawing.Size(52, 25);
            this.tbxHoverHeight_PCT1st.TabIndex = 30;
            this.tbxHoverHeight_PCT1st.Text = "1.00";
            // 
            // lblHoverHeight_DT2nd
            // 
            this.lblHoverHeight_DT2nd.AutoSize = true;
            this.lblHoverHeight_DT2nd.Location = new System.Drawing.Point(3, 155);
            this.lblHoverHeight_DT2nd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHoverHeight_DT2nd.Name = "lblHoverHeight_DT2nd";
            this.lblHoverHeight_DT2nd.Size = new System.Drawing.Size(244, 17);
            this.lblHoverHeight_DT2nd.TabIndex = 27;
            this.lblHoverHeight_DT2nd.Text = "Hover Height(DigitalTuning Hover2nd) :";
            // 
            // tbxHoverHeight_DT2nd
            // 
            this.tbxHoverHeight_DT2nd.Location = new System.Drawing.Point(278, 153);
            this.tbxHoverHeight_DT2nd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxHoverHeight_DT2nd.Name = "tbxHoverHeight_DT2nd";
            this.tbxHoverHeight_DT2nd.Size = new System.Drawing.Size(52, 25);
            this.tbxHoverHeight_DT2nd.TabIndex = 26;
            this.tbxHoverHeight_DT2nd.Text = "7.00";
            // 
            // lblHoverHeight_DT1st
            // 
            this.lblHoverHeight_DT1st.AutoSize = true;
            this.lblHoverHeight_DT1st.Location = new System.Drawing.Point(3, 126);
            this.lblHoverHeight_DT1st.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHoverHeight_DT1st.Name = "lblHoverHeight_DT1st";
            this.lblHoverHeight_DT1st.Size = new System.Drawing.Size(240, 17);
            this.lblHoverHeight_DT1st.TabIndex = 24;
            this.lblHoverHeight_DT1st.Text = "Hover Height(DigitalTuning Hover1st) :";
            // 
            // tbxHoverHeight_DT1st
            // 
            this.tbxHoverHeight_DT1st.Location = new System.Drawing.Point(278, 123);
            this.tbxHoverHeight_DT1st.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxHoverHeight_DT1st.Name = "tbxHoverHeight_DT1st";
            this.tbxHoverHeight_DT1st.Size = new System.Drawing.Size(52, 25);
            this.tbxHoverHeight_DT1st.TabIndex = 23;
            this.tbxHoverHeight_DT1st.Text = "5.00";
            // 
            // lblContactZCoord
            // 
            this.lblContactZCoord.AutoSize = true;
            this.lblContactZCoord.Location = new System.Drawing.Point(3, 85);
            this.lblContactZCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContactZCoord.Name = "lblContactZCoord";
            this.lblContactZCoord.Size = new System.Drawing.Size(74, 17);
            this.lblContactZCoord.TabIndex = 22;
            this.lblContactZCoord.Text = "Contact Z :";
            // 
            // lblEndYCoord
            // 
            this.lblEndYCoord.AutoSize = true;
            this.lblEndYCoord.Location = new System.Drawing.Point(220, 55);
            this.lblEndYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEndYCoord.Name = "lblEndYCoord";
            this.lblEndYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblEndYCoord.TabIndex = 21;
            this.lblEndYCoord.Text = "Y=";
            // 
            // lblEndXCoord
            // 
            this.lblEndXCoord.AutoSize = true;
            this.lblEndXCoord.Location = new System.Drawing.Point(134, 55);
            this.lblEndXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEndXCoord.Name = "lblEndXCoord";
            this.lblEndXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblEndXCoord.TabIndex = 20;
            this.lblEndXCoord.Text = "X=";
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(5, 55);
            this.lblEnd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(127, 17);
            this.lblEnd.TabIndex = 19;
            this.lblEnd.Text = "End(Bottom-Right) :";
            // 
            // lblStartYCoord
            // 
            this.lblStartYCoord.AutoSize = true;
            this.lblStartYCoord.Location = new System.Drawing.Point(220, 24);
            this.lblStartYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStartYCoord.Name = "lblStartYCoord";
            this.lblStartYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblStartYCoord.TabIndex = 18;
            this.lblStartYCoord.Text = "Y=";
            // 
            // lblStartXCoord
            // 
            this.lblStartXCoord.AutoSize = true;
            this.lblStartXCoord.Location = new System.Drawing.Point(134, 24);
            this.lblStartXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStartXCoord.Name = "lblStartXCoord";
            this.lblStartXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblStartXCoord.TabIndex = 17;
            this.lblStartXCoord.Text = "X=";
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(5, 24);
            this.lblStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(105, 17);
            this.lblStart.TabIndex = 16;
            this.lblStart.Text = "Start(Top-Left) :";
            // 
            // tbxZ
            // 
            this.tbxZ.Location = new System.Drawing.Point(79, 82);
            this.tbxZ.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxZ.Name = "tbxZ";
            this.tbxZ.Size = new System.Drawing.Size(52, 25);
            this.tbxZ.TabIndex = 4;
            this.tbxZ.Text = "22.00";
            // 
            // tbxY2
            // 
            this.tbxY2.Location = new System.Drawing.Point(248, 51);
            this.tbxY2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxY2.Name = "tbxY2";
            this.tbxY2.Size = new System.Drawing.Size(52, 25);
            this.tbxY2.TabIndex = 3;
            this.tbxY2.Text = "271.50";
            // 
            // tbxX2
            // 
            this.tbxX2.Location = new System.Drawing.Point(160, 51);
            this.tbxX2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxX2.Name = "tbxX2";
            this.tbxX2.Size = new System.Drawing.Size(52, 25);
            this.tbxX2.TabIndex = 2;
            this.tbxX2.Text = "397.38";
            // 
            // tbxY1
            // 
            this.tbxY1.Location = new System.Drawing.Point(248, 20);
            this.tbxY1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxY1.Name = "tbxY1";
            this.tbxY1.Size = new System.Drawing.Size(52, 25);
            this.tbxY1.TabIndex = 1;
            this.tbxY1.Text = "87.48";
            // 
            // tbxX1
            // 
            this.tbxX1.Location = new System.Drawing.Point(160, 20);
            this.tbxX1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxX1.Name = "tbxX1";
            this.tbxX1.Size = new System.Drawing.Size(52, 25);
            this.tbxX1.TabIndex = 0;
            this.tbxX1.Text = "110.65";
            // 
            // gbxTPGTCoordinate
            // 
            this.gbxTPGTCoordinate.Controls.Add(this.gbxTPGTVerticalLine);
            this.gbxTPGTCoordinate.Controls.Add(this.gbxTPGTHorizontalLine);
            this.gbxTPGTCoordinate.Controls.Add(this.lblTPGTContactZCoord);
            this.gbxTPGTCoordinate.Controls.Add(this.tbxTPGTContactZCoord);
            this.gbxTPGTCoordinate.Location = new System.Drawing.Point(370, 6);
            this.gbxTPGTCoordinate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxTPGTCoordinate.Name = "gbxTPGTCoordinate";
            this.gbxTPGTCoordinate.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxTPGTCoordinate.Size = new System.Drawing.Size(392, 279);
            this.gbxTPGTCoordinate.TabIndex = 11;
            this.gbxTPGTCoordinate.TabStop = false;
            this.gbxTPGTCoordinate.Text = "TP_Gain Coordinate";
            // 
            // gbxTPGTVerticalLine
            // 
            this.gbxTPGTVerticalLine.Controls.Add(this.lblTPGTVerticalStart);
            this.gbxTPGTVerticalLine.Controls.Add(this.tbxTPGTVerticalStartXCoord);
            this.gbxTPGTVerticalLine.Controls.Add(this.lblTPGTVerticalEndYCoord);
            this.gbxTPGTVerticalLine.Controls.Add(this.tbxTPGTVerticalStartYCoord);
            this.gbxTPGTVerticalLine.Controls.Add(this.lblTPGTVerticalEndXCoord);
            this.gbxTPGTVerticalLine.Controls.Add(this.tbxTPGTVerticalEndXCoord);
            this.gbxTPGTVerticalLine.Controls.Add(this.lblTPGTVerticalEnd);
            this.gbxTPGTVerticalLine.Controls.Add(this.tbxTPGTVerticalEndYCoord);
            this.gbxTPGTVerticalLine.Controls.Add(this.lblTPGTVerticalStartYCoord);
            this.gbxTPGTVerticalLine.Controls.Add(this.lblTPGTVerticalStartXCoord);
            this.gbxTPGTVerticalLine.Location = new System.Drawing.Point(6, 108);
            this.gbxTPGTVerticalLine.Margin = new System.Windows.Forms.Padding(2);
            this.gbxTPGTVerticalLine.Name = "gbxTPGTVerticalLine";
            this.gbxTPGTVerticalLine.Padding = new System.Windows.Forms.Padding(2);
            this.gbxTPGTVerticalLine.Size = new System.Drawing.Size(323, 85);
            this.gbxTPGTVerticalLine.TabIndex = 24;
            this.gbxTPGTVerticalLine.TabStop = false;
            this.gbxTPGTVerticalLine.Text = "Vertical Line";
            // 
            // lblTPGTVerticalStart
            // 
            this.lblTPGTVerticalStart.AutoSize = true;
            this.lblTPGTVerticalStart.Location = new System.Drawing.Point(4, 23);
            this.lblTPGTVerticalStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTVerticalStart.Name = "lblTPGTVerticalStart";
            this.lblTPGTVerticalStart.Size = new System.Drawing.Size(105, 17);
            this.lblTPGTVerticalStart.TabIndex = 16;
            this.lblTPGTVerticalStart.Text = "Start(Top-Left) :";
            // 
            // tbxTPGTVerticalStartXCoord
            // 
            this.tbxTPGTVerticalStartXCoord.Location = new System.Drawing.Point(160, 19);
            this.tbxTPGTVerticalStartXCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTPGTVerticalStartXCoord.Name = "tbxTPGTVerticalStartXCoord";
            this.tbxTPGTVerticalStartXCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxTPGTVerticalStartXCoord.TabIndex = 0;
            this.tbxTPGTVerticalStartXCoord.Text = "110.65";
            // 
            // lblTPGTVerticalEndYCoord
            // 
            this.lblTPGTVerticalEndYCoord.AutoSize = true;
            this.lblTPGTVerticalEndYCoord.Location = new System.Drawing.Point(220, 54);
            this.lblTPGTVerticalEndYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTVerticalEndYCoord.Name = "lblTPGTVerticalEndYCoord";
            this.lblTPGTVerticalEndYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblTPGTVerticalEndYCoord.TabIndex = 21;
            this.lblTPGTVerticalEndYCoord.Text = "Y=";
            // 
            // tbxTPGTVerticalStartYCoord
            // 
            this.tbxTPGTVerticalStartYCoord.Location = new System.Drawing.Point(248, 19);
            this.tbxTPGTVerticalStartYCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTPGTVerticalStartYCoord.Name = "tbxTPGTVerticalStartYCoord";
            this.tbxTPGTVerticalStartYCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxTPGTVerticalStartYCoord.TabIndex = 1;
            this.tbxTPGTVerticalStartYCoord.Text = "87.48";
            // 
            // lblTPGTVerticalEndXCoord
            // 
            this.lblTPGTVerticalEndXCoord.AutoSize = true;
            this.lblTPGTVerticalEndXCoord.Location = new System.Drawing.Point(134, 54);
            this.lblTPGTVerticalEndXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTVerticalEndXCoord.Name = "lblTPGTVerticalEndXCoord";
            this.lblTPGTVerticalEndXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblTPGTVerticalEndXCoord.TabIndex = 20;
            this.lblTPGTVerticalEndXCoord.Text = "X=";
            // 
            // tbxTPGTVerticalEndXCoord
            // 
            this.tbxTPGTVerticalEndXCoord.Location = new System.Drawing.Point(160, 50);
            this.tbxTPGTVerticalEndXCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTPGTVerticalEndXCoord.Name = "tbxTPGTVerticalEndXCoord";
            this.tbxTPGTVerticalEndXCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxTPGTVerticalEndXCoord.TabIndex = 2;
            this.tbxTPGTVerticalEndXCoord.Text = "397.38";
            // 
            // lblTPGTVerticalEnd
            // 
            this.lblTPGTVerticalEnd.AutoSize = true;
            this.lblTPGTVerticalEnd.Location = new System.Drawing.Point(4, 54);
            this.lblTPGTVerticalEnd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTVerticalEnd.Name = "lblTPGTVerticalEnd";
            this.lblTPGTVerticalEnd.Size = new System.Drawing.Size(127, 17);
            this.lblTPGTVerticalEnd.TabIndex = 19;
            this.lblTPGTVerticalEnd.Text = "End(Bottom-Right) :";
            // 
            // tbxTPGTVerticalEndYCoord
            // 
            this.tbxTPGTVerticalEndYCoord.Location = new System.Drawing.Point(248, 50);
            this.tbxTPGTVerticalEndYCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTPGTVerticalEndYCoord.Name = "tbxTPGTVerticalEndYCoord";
            this.tbxTPGTVerticalEndYCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxTPGTVerticalEndYCoord.TabIndex = 3;
            this.tbxTPGTVerticalEndYCoord.Text = "271.50";
            // 
            // lblTPGTVerticalStartYCoord
            // 
            this.lblTPGTVerticalStartYCoord.AutoSize = true;
            this.lblTPGTVerticalStartYCoord.Location = new System.Drawing.Point(220, 23);
            this.lblTPGTVerticalStartYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTVerticalStartYCoord.Name = "lblTPGTVerticalStartYCoord";
            this.lblTPGTVerticalStartYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblTPGTVerticalStartYCoord.TabIndex = 18;
            this.lblTPGTVerticalStartYCoord.Text = "Y=";
            // 
            // lblTPGTVerticalStartXCoord
            // 
            this.lblTPGTVerticalStartXCoord.AutoSize = true;
            this.lblTPGTVerticalStartXCoord.Location = new System.Drawing.Point(134, 23);
            this.lblTPGTVerticalStartXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTVerticalStartXCoord.Name = "lblTPGTVerticalStartXCoord";
            this.lblTPGTVerticalStartXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblTPGTVerticalStartXCoord.TabIndex = 17;
            this.lblTPGTVerticalStartXCoord.Text = "X=";
            // 
            // gbxTPGTHorizontalLine
            // 
            this.gbxTPGTHorizontalLine.Controls.Add(this.lblTPGTHorizontalStart);
            this.gbxTPGTHorizontalLine.Controls.Add(this.tbxTPGTHorizontalStartXCoord);
            this.gbxTPGTHorizontalLine.Controls.Add(this.lblTPGTHorizontalEndYCoord);
            this.gbxTPGTHorizontalLine.Controls.Add(this.tbxTPGTHorizontalStartYCoord);
            this.gbxTPGTHorizontalLine.Controls.Add(this.lblTPGTHorizontalEndXCood);
            this.gbxTPGTHorizontalLine.Controls.Add(this.tbxTPGTHorizontalEndXCoord);
            this.gbxTPGTHorizontalLine.Controls.Add(this.lblTPGTHorizontalEnd);
            this.gbxTPGTHorizontalLine.Controls.Add(this.tbxTPGTHorizontalEndYCoord);
            this.gbxTPGTHorizontalLine.Controls.Add(this.lblTPGTHorizontalStartYCoord);
            this.gbxTPGTHorizontalLine.Controls.Add(this.lblTPGTHorizontalStartXCoord);
            this.gbxTPGTHorizontalLine.Location = new System.Drawing.Point(6, 20);
            this.gbxTPGTHorizontalLine.Margin = new System.Windows.Forms.Padding(2);
            this.gbxTPGTHorizontalLine.Name = "gbxTPGTHorizontalLine";
            this.gbxTPGTHorizontalLine.Padding = new System.Windows.Forms.Padding(2);
            this.gbxTPGTHorizontalLine.Size = new System.Drawing.Size(323, 85);
            this.gbxTPGTHorizontalLine.TabIndex = 23;
            this.gbxTPGTHorizontalLine.TabStop = false;
            this.gbxTPGTHorizontalLine.Text = "Horizontal Line";
            // 
            // lblTPGTHorizontalStart
            // 
            this.lblTPGTHorizontalStart.AutoSize = true;
            this.lblTPGTHorizontalStart.Location = new System.Drawing.Point(4, 23);
            this.lblTPGTHorizontalStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTHorizontalStart.Name = "lblTPGTHorizontalStart";
            this.lblTPGTHorizontalStart.Size = new System.Drawing.Size(105, 17);
            this.lblTPGTHorizontalStart.TabIndex = 16;
            this.lblTPGTHorizontalStart.Text = "Start(Top-Left) :";
            // 
            // tbxTPGTHorizontalStartXCoord
            // 
            this.tbxTPGTHorizontalStartXCoord.Location = new System.Drawing.Point(160, 19);
            this.tbxTPGTHorizontalStartXCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTPGTHorizontalStartXCoord.Name = "tbxTPGTHorizontalStartXCoord";
            this.tbxTPGTHorizontalStartXCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxTPGTHorizontalStartXCoord.TabIndex = 0;
            this.tbxTPGTHorizontalStartXCoord.Text = "110.65";
            // 
            // lblTPGTHorizontalEndYCoord
            // 
            this.lblTPGTHorizontalEndYCoord.AutoSize = true;
            this.lblTPGTHorizontalEndYCoord.Location = new System.Drawing.Point(220, 54);
            this.lblTPGTHorizontalEndYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTHorizontalEndYCoord.Name = "lblTPGTHorizontalEndYCoord";
            this.lblTPGTHorizontalEndYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblTPGTHorizontalEndYCoord.TabIndex = 21;
            this.lblTPGTHorizontalEndYCoord.Text = "Y=";
            // 
            // tbxTPGTHorizontalStartYCoord
            // 
            this.tbxTPGTHorizontalStartYCoord.Location = new System.Drawing.Point(248, 19);
            this.tbxTPGTHorizontalStartYCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTPGTHorizontalStartYCoord.Name = "tbxTPGTHorizontalStartYCoord";
            this.tbxTPGTHorizontalStartYCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxTPGTHorizontalStartYCoord.TabIndex = 1;
            this.tbxTPGTHorizontalStartYCoord.Text = "87.48";
            // 
            // lblTPGTHorizontalEndXCood
            // 
            this.lblTPGTHorizontalEndXCood.AutoSize = true;
            this.lblTPGTHorizontalEndXCood.Location = new System.Drawing.Point(134, 54);
            this.lblTPGTHorizontalEndXCood.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTHorizontalEndXCood.Name = "lblTPGTHorizontalEndXCood";
            this.lblTPGTHorizontalEndXCood.Size = new System.Drawing.Size(26, 17);
            this.lblTPGTHorizontalEndXCood.TabIndex = 20;
            this.lblTPGTHorizontalEndXCood.Text = "X=";
            // 
            // tbxTPGTHorizontalEndXCoord
            // 
            this.tbxTPGTHorizontalEndXCoord.Location = new System.Drawing.Point(160, 50);
            this.tbxTPGTHorizontalEndXCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTPGTHorizontalEndXCoord.Name = "tbxTPGTHorizontalEndXCoord";
            this.tbxTPGTHorizontalEndXCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxTPGTHorizontalEndXCoord.TabIndex = 2;
            this.tbxTPGTHorizontalEndXCoord.Text = "397.38";
            // 
            // lblTPGTHorizontalEnd
            // 
            this.lblTPGTHorizontalEnd.AutoSize = true;
            this.lblTPGTHorizontalEnd.Location = new System.Drawing.Point(4, 54);
            this.lblTPGTHorizontalEnd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTHorizontalEnd.Name = "lblTPGTHorizontalEnd";
            this.lblTPGTHorizontalEnd.Size = new System.Drawing.Size(127, 17);
            this.lblTPGTHorizontalEnd.TabIndex = 19;
            this.lblTPGTHorizontalEnd.Text = "End(Bottom-Right) :";
            // 
            // tbxTPGTHorizontalEndYCoord
            // 
            this.tbxTPGTHorizontalEndYCoord.Location = new System.Drawing.Point(248, 50);
            this.tbxTPGTHorizontalEndYCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTPGTHorizontalEndYCoord.Name = "tbxTPGTHorizontalEndYCoord";
            this.tbxTPGTHorizontalEndYCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxTPGTHorizontalEndYCoord.TabIndex = 3;
            this.tbxTPGTHorizontalEndYCoord.Text = "271.50";
            // 
            // lblTPGTHorizontalStartYCoord
            // 
            this.lblTPGTHorizontalStartYCoord.AutoSize = true;
            this.lblTPGTHorizontalStartYCoord.Location = new System.Drawing.Point(220, 23);
            this.lblTPGTHorizontalStartYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTHorizontalStartYCoord.Name = "lblTPGTHorizontalStartYCoord";
            this.lblTPGTHorizontalStartYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblTPGTHorizontalStartYCoord.TabIndex = 18;
            this.lblTPGTHorizontalStartYCoord.Text = "Y=";
            // 
            // lblTPGTHorizontalStartXCoord
            // 
            this.lblTPGTHorizontalStartXCoord.AutoSize = true;
            this.lblTPGTHorizontalStartXCoord.Location = new System.Drawing.Point(134, 23);
            this.lblTPGTHorizontalStartXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTHorizontalStartXCoord.Name = "lblTPGTHorizontalStartXCoord";
            this.lblTPGTHorizontalStartXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblTPGTHorizontalStartXCoord.TabIndex = 17;
            this.lblTPGTHorizontalStartXCoord.Text = "X=";
            // 
            // lblTPGTContactZCoord
            // 
            this.lblTPGTContactZCoord.AutoSize = true;
            this.lblTPGTContactZCoord.Location = new System.Drawing.Point(11, 202);
            this.lblTPGTContactZCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTContactZCoord.Name = "lblTPGTContactZCoord";
            this.lblTPGTContactZCoord.Size = new System.Drawing.Size(74, 17);
            this.lblTPGTContactZCoord.TabIndex = 22;
            this.lblTPGTContactZCoord.Text = "Contact Z :";
            // 
            // tbxTPGTContactZCoord
            // 
            this.tbxTPGTContactZCoord.Location = new System.Drawing.Point(87, 199);
            this.tbxTPGTContactZCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTPGTContactZCoord.Name = "tbxTPGTContactZCoord";
            this.tbxTPGTContactZCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxTPGTContactZCoord.TabIndex = 4;
            this.tbxTPGTContactZCoord.Text = "22.00";
            // 
            // tpgGoDrawCoordinate
            // 
            this.tpgGoDrawCoordinate.BackColor = System.Drawing.SystemColors.Control;
            this.tpgGoDrawCoordinate.Controls.Add(this.gbxGoDrawNormalCoordinate);
            this.tpgGoDrawCoordinate.Controls.Add(this.gbxGoDrawTPGTCoordinate);
            this.tpgGoDrawCoordinate.Location = new System.Drawing.Point(4, 26);
            this.tpgGoDrawCoordinate.Margin = new System.Windows.Forms.Padding(2);
            this.tpgGoDrawCoordinate.Name = "tpgGoDrawCoordinate";
            this.tpgGoDrawCoordinate.Padding = new System.Windows.Forms.Padding(2);
            this.tpgGoDrawCoordinate.Size = new System.Drawing.Size(870, 436);
            this.tpgGoDrawCoordinate.TabIndex = 1;
            this.tpgGoDrawCoordinate.Text = "GoDraw Coordinate";
            // 
            // gbxGoDrawNormalCoordinate
            // 
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawTopZServoValue);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.tbxGoDrawTopZServoValue);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawHoverZServoValue_PCT2nd);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.tbxGoDrawHoverZServoValue_PCT2nd);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawHoverZServoValue_PCT1st);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.tbxGoDrawHoverZServoValue_PCT1st);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawHoverZServoValue_DT2nd);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.tbxGoDrawHoverZServoValue_DT2nd);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawHoverZServoValue_DT1st);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.tbxGoDrawHoverZServoValue_DT1st);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawContactZServoValue);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawEndYCoord);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawEndXCoord);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawEnd);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawStartYCoord);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawStartXCoord);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.lblGoDrawStart);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.tbxGoDrawContactZServoValue);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.tbxGoDrawEndYCoord);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.tbxGoDrawEndXCoord);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.tbxGoDrawStartYCoord);
            this.gbxGoDrawNormalCoordinate.Controls.Add(this.tbxGoDrawStartXCoord);
            this.gbxGoDrawNormalCoordinate.Location = new System.Drawing.Point(5, 6);
            this.gbxGoDrawNormalCoordinate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxGoDrawNormalCoordinate.Name = "gbxGoDrawNormalCoordinate";
            this.gbxGoDrawNormalCoordinate.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxGoDrawNormalCoordinate.Size = new System.Drawing.Size(358, 279);
            this.gbxGoDrawNormalCoordinate.TabIndex = 12;
            this.gbxGoDrawNormalCoordinate.TabStop = false;
            this.gbxGoDrawNormalCoordinate.Text = "Normal Coordinate";
            // 
            // lblGoDrawTopZServoValue
            // 
            this.lblGoDrawTopZServoValue.AutoSize = true;
            this.lblGoDrawTopZServoValue.Location = new System.Drawing.Point(3, 83);
            this.lblGoDrawTopZServoValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTopZServoValue.Name = "lblGoDrawTopZServoValue";
            this.lblGoDrawTopZServoValue.Size = new System.Drawing.Size(88, 17);
            this.lblGoDrawTopZServoValue.TabIndex = 35;
            this.lblGoDrawTopZServoValue.Text = "Top Z Servo :";
            // 
            // tbxGoDrawTopZServoValue
            // 
            this.tbxGoDrawTopZServoValue.Location = new System.Drawing.Point(117, 81);
            this.tbxGoDrawTopZServoValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawTopZServoValue.Name = "tbxGoDrawTopZServoValue";
            this.tbxGoDrawTopZServoValue.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawTopZServoValue.TabIndex = 34;
            this.tbxGoDrawTopZServoValue.Text = "26000";
            // 
            // lblGoDrawHoverZServoValue_PCT2nd
            // 
            this.lblGoDrawHoverZServoValue_PCT2nd.AutoSize = true;
            this.lblGoDrawHoverZServoValue_PCT2nd.Location = new System.Drawing.Point(3, 233);
            this.lblGoDrawHoverZServoValue_PCT2nd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawHoverZServoValue_PCT2nd.Name = "lblGoDrawHoverZServoValue_PCT2nd";
            this.lblGoDrawHoverZServoValue_PCT2nd.Size = new System.Drawing.Size(269, 17);
            this.lblGoDrawHoverZServoValue_PCT2nd.TabIndex = 33;
            this.lblGoDrawHoverZServoValue_PCT2nd.Text = "Hover Servo(PeakCheckTuning Hover2nd) :";
            // 
            // tbxGoDrawHoverZServoValue_PCT2nd
            // 
            this.tbxGoDrawHoverZServoValue_PCT2nd.Location = new System.Drawing.Point(274, 230);
            this.tbxGoDrawHoverZServoValue_PCT2nd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawHoverZServoValue_PCT2nd.Name = "tbxGoDrawHoverZServoValue_PCT2nd";
            this.tbxGoDrawHoverZServoValue_PCT2nd.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawHoverZServoValue_PCT2nd.TabIndex = 32;
            this.tbxGoDrawHoverZServoValue_PCT2nd.Text = "12000";
            // 
            // lblGoDrawHoverZServoValue_PCT1st
            // 
            this.lblGoDrawHoverZServoValue_PCT1st.AutoSize = true;
            this.lblGoDrawHoverZServoValue_PCT1st.Location = new System.Drawing.Point(3, 203);
            this.lblGoDrawHoverZServoValue_PCT1st.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawHoverZServoValue_PCT1st.Name = "lblGoDrawHoverZServoValue_PCT1st";
            this.lblGoDrawHoverZServoValue_PCT1st.Size = new System.Drawing.Size(265, 17);
            this.lblGoDrawHoverZServoValue_PCT1st.TabIndex = 31;
            this.lblGoDrawHoverZServoValue_PCT1st.Text = "Hover Servo(PeakCheckTuning Hover1st) :";
            // 
            // tbxGoDrawHoverZServoValue_PCT1st
            // 
            this.tbxGoDrawHoverZServoValue_PCT1st.Location = new System.Drawing.Point(274, 201);
            this.tbxGoDrawHoverZServoValue_PCT1st.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawHoverZServoValue_PCT1st.Name = "tbxGoDrawHoverZServoValue_PCT1st";
            this.tbxGoDrawHoverZServoValue_PCT1st.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawHoverZServoValue_PCT1st.TabIndex = 30;
            this.tbxGoDrawHoverZServoValue_PCT1st.Text = "11000";
            // 
            // lblGoDrawHoverZServoValue_DT2nd
            // 
            this.lblGoDrawHoverZServoValue_DT2nd.AutoSize = true;
            this.lblGoDrawHoverZServoValue_DT2nd.Location = new System.Drawing.Point(3, 174);
            this.lblGoDrawHoverZServoValue_DT2nd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawHoverZServoValue_DT2nd.Name = "lblGoDrawHoverZServoValue_DT2nd";
            this.lblGoDrawHoverZServoValue_DT2nd.Size = new System.Drawing.Size(239, 17);
            this.lblGoDrawHoverZServoValue_DT2nd.TabIndex = 27;
            this.lblGoDrawHoverZServoValue_DT2nd.Text = "Hover Servo(DigitalTuning Hover2nd) :";
            // 
            // tbxGoDrawHoverZServoValue_DT2nd
            // 
            this.tbxGoDrawHoverZServoValue_DT2nd.Location = new System.Drawing.Point(274, 171);
            this.tbxGoDrawHoverZServoValue_DT2nd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawHoverZServoValue_DT2nd.Name = "tbxGoDrawHoverZServoValue_DT2nd";
            this.tbxGoDrawHoverZServoValue_DT2nd.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawHoverZServoValue_DT2nd.TabIndex = 26;
            this.tbxGoDrawHoverZServoValue_DT2nd.Text = "15000";
            // 
            // lblGoDrawHoverZServoValue_DT1st
            // 
            this.lblGoDrawHoverZServoValue_DT1st.AutoSize = true;
            this.lblGoDrawHoverZServoValue_DT1st.Location = new System.Drawing.Point(3, 144);
            this.lblGoDrawHoverZServoValue_DT1st.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawHoverZServoValue_DT1st.Name = "lblGoDrawHoverZServoValue_DT1st";
            this.lblGoDrawHoverZServoValue_DT1st.Size = new System.Drawing.Size(235, 17);
            this.lblGoDrawHoverZServoValue_DT1st.TabIndex = 24;
            this.lblGoDrawHoverZServoValue_DT1st.Text = "Hover Servo(DigitalTuning Hover1st) :";
            // 
            // tbxGoDrawHoverZServoValue_DT1st
            // 
            this.tbxGoDrawHoverZServoValue_DT1st.Location = new System.Drawing.Point(274, 142);
            this.tbxGoDrawHoverZServoValue_DT1st.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawHoverZServoValue_DT1st.Name = "tbxGoDrawHoverZServoValue_DT1st";
            this.tbxGoDrawHoverZServoValue_DT1st.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawHoverZServoValue_DT1st.TabIndex = 23;
            this.tbxGoDrawHoverZServoValue_DT1st.Text = "14000";
            // 
            // lblGoDrawContactZServoValue
            // 
            this.lblGoDrawContactZServoValue.AutoSize = true;
            this.lblGoDrawContactZServoValue.Location = new System.Drawing.Point(3, 112);
            this.lblGoDrawContactZServoValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawContactZServoValue.Name = "lblGoDrawContactZServoValue";
            this.lblGoDrawContactZServoValue.Size = new System.Drawing.Size(112, 17);
            this.lblGoDrawContactZServoValue.TabIndex = 22;
            this.lblGoDrawContactZServoValue.Text = "Contact Z Servo :";
            // 
            // lblGoDrawEndYCoord
            // 
            this.lblGoDrawEndYCoord.AutoSize = true;
            this.lblGoDrawEndYCoord.Location = new System.Drawing.Point(220, 55);
            this.lblGoDrawEndYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawEndYCoord.Name = "lblGoDrawEndYCoord";
            this.lblGoDrawEndYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblGoDrawEndYCoord.TabIndex = 21;
            this.lblGoDrawEndYCoord.Text = "Y=";
            // 
            // lblGoDrawEndXCoord
            // 
            this.lblGoDrawEndXCoord.AutoSize = true;
            this.lblGoDrawEndXCoord.Location = new System.Drawing.Point(134, 55);
            this.lblGoDrawEndXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawEndXCoord.Name = "lblGoDrawEndXCoord";
            this.lblGoDrawEndXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblGoDrawEndXCoord.TabIndex = 20;
            this.lblGoDrawEndXCoord.Text = "X=";
            // 
            // lblGoDrawEnd
            // 
            this.lblGoDrawEnd.AutoSize = true;
            this.lblGoDrawEnd.Location = new System.Drawing.Point(5, 55);
            this.lblGoDrawEnd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawEnd.Name = "lblGoDrawEnd";
            this.lblGoDrawEnd.Size = new System.Drawing.Size(127, 17);
            this.lblGoDrawEnd.TabIndex = 19;
            this.lblGoDrawEnd.Text = "End(Bottom-Right) :";
            // 
            // lblGoDrawStartYCoord
            // 
            this.lblGoDrawStartYCoord.AutoSize = true;
            this.lblGoDrawStartYCoord.Location = new System.Drawing.Point(220, 24);
            this.lblGoDrawStartYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawStartYCoord.Name = "lblGoDrawStartYCoord";
            this.lblGoDrawStartYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblGoDrawStartYCoord.TabIndex = 18;
            this.lblGoDrawStartYCoord.Text = "Y=";
            // 
            // lblGoDrawStartXCoord
            // 
            this.lblGoDrawStartXCoord.AutoSize = true;
            this.lblGoDrawStartXCoord.Location = new System.Drawing.Point(134, 24);
            this.lblGoDrawStartXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawStartXCoord.Name = "lblGoDrawStartXCoord";
            this.lblGoDrawStartXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblGoDrawStartXCoord.TabIndex = 17;
            this.lblGoDrawStartXCoord.Text = "X=";
            // 
            // lblGoDrawStart
            // 
            this.lblGoDrawStart.AutoSize = true;
            this.lblGoDrawStart.Location = new System.Drawing.Point(5, 24);
            this.lblGoDrawStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawStart.Name = "lblGoDrawStart";
            this.lblGoDrawStart.Size = new System.Drawing.Size(105, 17);
            this.lblGoDrawStart.TabIndex = 16;
            this.lblGoDrawStart.Text = "Start(Top-Left) :";
            // 
            // tbxGoDrawContactZServoValue
            // 
            this.tbxGoDrawContactZServoValue.Location = new System.Drawing.Point(117, 110);
            this.tbxGoDrawContactZServoValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawContactZServoValue.Name = "tbxGoDrawContactZServoValue";
            this.tbxGoDrawContactZServoValue.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawContactZServoValue.TabIndex = 4;
            this.tbxGoDrawContactZServoValue.Text = "3000";
            // 
            // tbxGoDrawEndYCoord
            // 
            this.tbxGoDrawEndYCoord.Location = new System.Drawing.Point(248, 51);
            this.tbxGoDrawEndYCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawEndYCoord.Name = "tbxGoDrawEndYCoord";
            this.tbxGoDrawEndYCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawEndYCoord.TabIndex = 3;
            this.tbxGoDrawEndYCoord.Text = "0";
            // 
            // tbxGoDrawEndXCoord
            // 
            this.tbxGoDrawEndXCoord.Location = new System.Drawing.Point(160, 51);
            this.tbxGoDrawEndXCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawEndXCoord.Name = "tbxGoDrawEndXCoord";
            this.tbxGoDrawEndXCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawEndXCoord.TabIndex = 2;
            this.tbxGoDrawEndXCoord.Text = "0";
            // 
            // tbxGoDrawStartYCoord
            // 
            this.tbxGoDrawStartYCoord.Location = new System.Drawing.Point(248, 20);
            this.tbxGoDrawStartYCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawStartYCoord.Name = "tbxGoDrawStartYCoord";
            this.tbxGoDrawStartYCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawStartYCoord.TabIndex = 1;
            this.tbxGoDrawStartYCoord.Text = "0";
            // 
            // tbxGoDrawStartXCoord
            // 
            this.tbxGoDrawStartXCoord.Location = new System.Drawing.Point(160, 20);
            this.tbxGoDrawStartXCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawStartXCoord.Name = "tbxGoDrawStartXCoord";
            this.tbxGoDrawStartXCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawStartXCoord.TabIndex = 0;
            this.tbxGoDrawStartXCoord.Text = "0";
            // 
            // gbxGoDrawTPGTCoordinate
            // 
            this.gbxGoDrawTPGTCoordinate.Controls.Add(this.gbxGoDrawTPGTVerticalLine);
            this.gbxGoDrawTPGTCoordinate.Controls.Add(this.gbxGoDrawTPGTHorizontalLine);
            this.gbxGoDrawTPGTCoordinate.Controls.Add(this.lblGoDrawTPGTContactZServoValue);
            this.gbxGoDrawTPGTCoordinate.Controls.Add(this.tbxGoDrawTPGTContactZServoValue);
            this.gbxGoDrawTPGTCoordinate.Location = new System.Drawing.Point(370, 6);
            this.gbxGoDrawTPGTCoordinate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxGoDrawTPGTCoordinate.Name = "gbxGoDrawTPGTCoordinate";
            this.gbxGoDrawTPGTCoordinate.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxGoDrawTPGTCoordinate.Size = new System.Drawing.Size(392, 279);
            this.gbxGoDrawTPGTCoordinate.TabIndex = 13;
            this.gbxGoDrawTPGTCoordinate.TabStop = false;
            this.gbxGoDrawTPGTCoordinate.Text = "TP_Gain Coordinate";
            // 
            // gbxGoDrawTPGTVerticalLine
            // 
            this.gbxGoDrawTPGTVerticalLine.Controls.Add(this.lblGoDrawTPGTVerticalStart);
            this.gbxGoDrawTPGTVerticalLine.Controls.Add(this.tbxGoDrawTPGTVerticalStartXCoord);
            this.gbxGoDrawTPGTVerticalLine.Controls.Add(this.lblGoDrawTPGTVerticalEndYCoord);
            this.gbxGoDrawTPGTVerticalLine.Controls.Add(this.tbxGoDrawTPGTVerticalStartYCoord);
            this.gbxGoDrawTPGTVerticalLine.Controls.Add(this.lblGoDrawTPGTVerticalEndXCoord);
            this.gbxGoDrawTPGTVerticalLine.Controls.Add(this.tbxGoDrawTPGTVerticalEndXCoord);
            this.gbxGoDrawTPGTVerticalLine.Controls.Add(this.lblGoDrawTPGTVerticalEnd);
            this.gbxGoDrawTPGTVerticalLine.Controls.Add(this.tbxGoDrawTPGTVerticalEndYCoord);
            this.gbxGoDrawTPGTVerticalLine.Controls.Add(this.lblGoDrawTPGTVerticalStartYCoord);
            this.gbxGoDrawTPGTVerticalLine.Controls.Add(this.lblGoDrawTPGTVerticalStartXCoord);
            this.gbxGoDrawTPGTVerticalLine.Location = new System.Drawing.Point(6, 108);
            this.gbxGoDrawTPGTVerticalLine.Margin = new System.Windows.Forms.Padding(2);
            this.gbxGoDrawTPGTVerticalLine.Name = "gbxGoDrawTPGTVerticalLine";
            this.gbxGoDrawTPGTVerticalLine.Padding = new System.Windows.Forms.Padding(2);
            this.gbxGoDrawTPGTVerticalLine.Size = new System.Drawing.Size(323, 85);
            this.gbxGoDrawTPGTVerticalLine.TabIndex = 24;
            this.gbxGoDrawTPGTVerticalLine.TabStop = false;
            this.gbxGoDrawTPGTVerticalLine.Text = "Vertical Line";
            // 
            // lblGoDrawTPGTVerticalStart
            // 
            this.lblGoDrawTPGTVerticalStart.AutoSize = true;
            this.lblGoDrawTPGTVerticalStart.Location = new System.Drawing.Point(4, 23);
            this.lblGoDrawTPGTVerticalStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTVerticalStart.Name = "lblGoDrawTPGTVerticalStart";
            this.lblGoDrawTPGTVerticalStart.Size = new System.Drawing.Size(105, 17);
            this.lblGoDrawTPGTVerticalStart.TabIndex = 16;
            this.lblGoDrawTPGTVerticalStart.Text = "Start(Top-Left) :";
            // 
            // tbxGoDrawTPGTVerticalStartXCoord
            // 
            this.tbxGoDrawTPGTVerticalStartXCoord.Location = new System.Drawing.Point(160, 19);
            this.tbxGoDrawTPGTVerticalStartXCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawTPGTVerticalStartXCoord.Name = "tbxGoDrawTPGTVerticalStartXCoord";
            this.tbxGoDrawTPGTVerticalStartXCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawTPGTVerticalStartXCoord.TabIndex = 0;
            this.tbxGoDrawTPGTVerticalStartXCoord.Text = "0";
            // 
            // lblGoDrawTPGTVerticalEndYCoord
            // 
            this.lblGoDrawTPGTVerticalEndYCoord.AutoSize = true;
            this.lblGoDrawTPGTVerticalEndYCoord.Location = new System.Drawing.Point(220, 54);
            this.lblGoDrawTPGTVerticalEndYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTVerticalEndYCoord.Name = "lblGoDrawTPGTVerticalEndYCoord";
            this.lblGoDrawTPGTVerticalEndYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblGoDrawTPGTVerticalEndYCoord.TabIndex = 21;
            this.lblGoDrawTPGTVerticalEndYCoord.Text = "Y=";
            // 
            // tbxGoDrawTPGTVerticalStartYCoord
            // 
            this.tbxGoDrawTPGTVerticalStartYCoord.Location = new System.Drawing.Point(248, 19);
            this.tbxGoDrawTPGTVerticalStartYCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawTPGTVerticalStartYCoord.Name = "tbxGoDrawTPGTVerticalStartYCoord";
            this.tbxGoDrawTPGTVerticalStartYCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawTPGTVerticalStartYCoord.TabIndex = 1;
            this.tbxGoDrawTPGTVerticalStartYCoord.Text = "0";
            // 
            // lblGoDrawTPGTVerticalEndXCoord
            // 
            this.lblGoDrawTPGTVerticalEndXCoord.AutoSize = true;
            this.lblGoDrawTPGTVerticalEndXCoord.Location = new System.Drawing.Point(134, 54);
            this.lblGoDrawTPGTVerticalEndXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTVerticalEndXCoord.Name = "lblGoDrawTPGTVerticalEndXCoord";
            this.lblGoDrawTPGTVerticalEndXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblGoDrawTPGTVerticalEndXCoord.TabIndex = 20;
            this.lblGoDrawTPGTVerticalEndXCoord.Text = "X=";
            // 
            // tbxGoDrawTPGTVerticalEndXCoord
            // 
            this.tbxGoDrawTPGTVerticalEndXCoord.Location = new System.Drawing.Point(160, 50);
            this.tbxGoDrawTPGTVerticalEndXCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawTPGTVerticalEndXCoord.Name = "tbxGoDrawTPGTVerticalEndXCoord";
            this.tbxGoDrawTPGTVerticalEndXCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawTPGTVerticalEndXCoord.TabIndex = 2;
            this.tbxGoDrawTPGTVerticalEndXCoord.Text = "0";
            // 
            // lblGoDrawTPGTVerticalEnd
            // 
            this.lblGoDrawTPGTVerticalEnd.AutoSize = true;
            this.lblGoDrawTPGTVerticalEnd.Location = new System.Drawing.Point(4, 54);
            this.lblGoDrawTPGTVerticalEnd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTVerticalEnd.Name = "lblGoDrawTPGTVerticalEnd";
            this.lblGoDrawTPGTVerticalEnd.Size = new System.Drawing.Size(127, 17);
            this.lblGoDrawTPGTVerticalEnd.TabIndex = 19;
            this.lblGoDrawTPGTVerticalEnd.Text = "End(Bottom-Right) :";
            // 
            // tbxGoDrawTPGTVerticalEndYCoord
            // 
            this.tbxGoDrawTPGTVerticalEndYCoord.Location = new System.Drawing.Point(248, 50);
            this.tbxGoDrawTPGTVerticalEndYCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawTPGTVerticalEndYCoord.Name = "tbxGoDrawTPGTVerticalEndYCoord";
            this.tbxGoDrawTPGTVerticalEndYCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawTPGTVerticalEndYCoord.TabIndex = 3;
            this.tbxGoDrawTPGTVerticalEndYCoord.Text = "0";
            // 
            // lblGoDrawTPGTVerticalStartYCoord
            // 
            this.lblGoDrawTPGTVerticalStartYCoord.AutoSize = true;
            this.lblGoDrawTPGTVerticalStartYCoord.Location = new System.Drawing.Point(220, 23);
            this.lblGoDrawTPGTVerticalStartYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTVerticalStartYCoord.Name = "lblGoDrawTPGTVerticalStartYCoord";
            this.lblGoDrawTPGTVerticalStartYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblGoDrawTPGTVerticalStartYCoord.TabIndex = 18;
            this.lblGoDrawTPGTVerticalStartYCoord.Text = "Y=";
            // 
            // lblGoDrawTPGTVerticalStartXCoord
            // 
            this.lblGoDrawTPGTVerticalStartXCoord.AutoSize = true;
            this.lblGoDrawTPGTVerticalStartXCoord.Location = new System.Drawing.Point(134, 23);
            this.lblGoDrawTPGTVerticalStartXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTVerticalStartXCoord.Name = "lblGoDrawTPGTVerticalStartXCoord";
            this.lblGoDrawTPGTVerticalStartXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblGoDrawTPGTVerticalStartXCoord.TabIndex = 17;
            this.lblGoDrawTPGTVerticalStartXCoord.Text = "X=";
            // 
            // gbxGoDrawTPGTHorizontalLine
            // 
            this.gbxGoDrawTPGTHorizontalLine.Controls.Add(this.lblGoDrawTPGTHorizontalStart);
            this.gbxGoDrawTPGTHorizontalLine.Controls.Add(this.tbxGoDrawTPGTHorizontalStartXCoord);
            this.gbxGoDrawTPGTHorizontalLine.Controls.Add(this.lblGoDrawTPGTHorizontalEndYCoord);
            this.gbxGoDrawTPGTHorizontalLine.Controls.Add(this.tbxGoDrawTPGTHorizontalStartYCoord);
            this.gbxGoDrawTPGTHorizontalLine.Controls.Add(this.lblGoDrawTPGTHorizontalEndXCoord);
            this.gbxGoDrawTPGTHorizontalLine.Controls.Add(this.tbxGoDrawTPGTHorizontalEndXCoord);
            this.gbxGoDrawTPGTHorizontalLine.Controls.Add(this.lblGoDrawTPGTHorizontalEnd);
            this.gbxGoDrawTPGTHorizontalLine.Controls.Add(this.tbxGoDrawTPGTHorizontalEndYCoord);
            this.gbxGoDrawTPGTHorizontalLine.Controls.Add(this.lblGoDrawTPGTHorizontalStartYCoord);
            this.gbxGoDrawTPGTHorizontalLine.Controls.Add(this.lblGoDrawTPGTHorizontalStartXCoord);
            this.gbxGoDrawTPGTHorizontalLine.Location = new System.Drawing.Point(6, 20);
            this.gbxGoDrawTPGTHorizontalLine.Margin = new System.Windows.Forms.Padding(2);
            this.gbxGoDrawTPGTHorizontalLine.Name = "gbxGoDrawTPGTHorizontalLine";
            this.gbxGoDrawTPGTHorizontalLine.Padding = new System.Windows.Forms.Padding(2);
            this.gbxGoDrawTPGTHorizontalLine.Size = new System.Drawing.Size(323, 85);
            this.gbxGoDrawTPGTHorizontalLine.TabIndex = 23;
            this.gbxGoDrawTPGTHorizontalLine.TabStop = false;
            this.gbxGoDrawTPGTHorizontalLine.Text = "Horizontal Line";
            // 
            // lblGoDrawTPGTHorizontalStart
            // 
            this.lblGoDrawTPGTHorizontalStart.AutoSize = true;
            this.lblGoDrawTPGTHorizontalStart.Location = new System.Drawing.Point(4, 23);
            this.lblGoDrawTPGTHorizontalStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTHorizontalStart.Name = "lblGoDrawTPGTHorizontalStart";
            this.lblGoDrawTPGTHorizontalStart.Size = new System.Drawing.Size(105, 17);
            this.lblGoDrawTPGTHorizontalStart.TabIndex = 16;
            this.lblGoDrawTPGTHorizontalStart.Text = "Start(Top-Left) :";
            // 
            // tbxGoDrawTPGTHorizontalStartXCoord
            // 
            this.tbxGoDrawTPGTHorizontalStartXCoord.Location = new System.Drawing.Point(160, 19);
            this.tbxGoDrawTPGTHorizontalStartXCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawTPGTHorizontalStartXCoord.Name = "tbxGoDrawTPGTHorizontalStartXCoord";
            this.tbxGoDrawTPGTHorizontalStartXCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawTPGTHorizontalStartXCoord.TabIndex = 0;
            this.tbxGoDrawTPGTHorizontalStartXCoord.Text = "0";
            // 
            // lblGoDrawTPGTHorizontalEndYCoord
            // 
            this.lblGoDrawTPGTHorizontalEndYCoord.AutoSize = true;
            this.lblGoDrawTPGTHorizontalEndYCoord.Location = new System.Drawing.Point(220, 54);
            this.lblGoDrawTPGTHorizontalEndYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTHorizontalEndYCoord.Name = "lblGoDrawTPGTHorizontalEndYCoord";
            this.lblGoDrawTPGTHorizontalEndYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblGoDrawTPGTHorizontalEndYCoord.TabIndex = 21;
            this.lblGoDrawTPGTHorizontalEndYCoord.Text = "Y=";
            // 
            // tbxGoDrawTPGTHorizontalStartYCoord
            // 
            this.tbxGoDrawTPGTHorizontalStartYCoord.Location = new System.Drawing.Point(248, 19);
            this.tbxGoDrawTPGTHorizontalStartYCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawTPGTHorizontalStartYCoord.Name = "tbxGoDrawTPGTHorizontalStartYCoord";
            this.tbxGoDrawTPGTHorizontalStartYCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawTPGTHorizontalStartYCoord.TabIndex = 1;
            this.tbxGoDrawTPGTHorizontalStartYCoord.Text = "0";
            // 
            // lblGoDrawTPGTHorizontalEndXCoord
            // 
            this.lblGoDrawTPGTHorizontalEndXCoord.AutoSize = true;
            this.lblGoDrawTPGTHorizontalEndXCoord.Location = new System.Drawing.Point(134, 54);
            this.lblGoDrawTPGTHorizontalEndXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTHorizontalEndXCoord.Name = "lblGoDrawTPGTHorizontalEndXCoord";
            this.lblGoDrawTPGTHorizontalEndXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblGoDrawTPGTHorizontalEndXCoord.TabIndex = 20;
            this.lblGoDrawTPGTHorizontalEndXCoord.Text = "X=";
            // 
            // tbxGoDrawTPGTHorizontalEndXCoord
            // 
            this.tbxGoDrawTPGTHorizontalEndXCoord.Location = new System.Drawing.Point(160, 50);
            this.tbxGoDrawTPGTHorizontalEndXCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawTPGTHorizontalEndXCoord.Name = "tbxGoDrawTPGTHorizontalEndXCoord";
            this.tbxGoDrawTPGTHorizontalEndXCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawTPGTHorizontalEndXCoord.TabIndex = 2;
            this.tbxGoDrawTPGTHorizontalEndXCoord.Text = "0";
            // 
            // lblGoDrawTPGTHorizontalEnd
            // 
            this.lblGoDrawTPGTHorizontalEnd.AutoSize = true;
            this.lblGoDrawTPGTHorizontalEnd.Location = new System.Drawing.Point(4, 54);
            this.lblGoDrawTPGTHorizontalEnd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTHorizontalEnd.Name = "lblGoDrawTPGTHorizontalEnd";
            this.lblGoDrawTPGTHorizontalEnd.Size = new System.Drawing.Size(127, 17);
            this.lblGoDrawTPGTHorizontalEnd.TabIndex = 19;
            this.lblGoDrawTPGTHorizontalEnd.Text = "End(Bottom-Right) :";
            // 
            // tbxGoDrawTPGTHorizontalEndYCoord
            // 
            this.tbxGoDrawTPGTHorizontalEndYCoord.Location = new System.Drawing.Point(248, 50);
            this.tbxGoDrawTPGTHorizontalEndYCoord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawTPGTHorizontalEndYCoord.Name = "tbxGoDrawTPGTHorizontalEndYCoord";
            this.tbxGoDrawTPGTHorizontalEndYCoord.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawTPGTHorizontalEndYCoord.TabIndex = 3;
            this.tbxGoDrawTPGTHorizontalEndYCoord.Text = "0";
            // 
            // lblGoDrawTPGTHorizontalStartYCoord
            // 
            this.lblGoDrawTPGTHorizontalStartYCoord.AutoSize = true;
            this.lblGoDrawTPGTHorizontalStartYCoord.Location = new System.Drawing.Point(220, 23);
            this.lblGoDrawTPGTHorizontalStartYCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTHorizontalStartYCoord.Name = "lblGoDrawTPGTHorizontalStartYCoord";
            this.lblGoDrawTPGTHorizontalStartYCoord.Size = new System.Drawing.Size(27, 17);
            this.lblGoDrawTPGTHorizontalStartYCoord.TabIndex = 18;
            this.lblGoDrawTPGTHorizontalStartYCoord.Text = "Y=";
            // 
            // lblGoDrawTPGTHorizontalStartXCoord
            // 
            this.lblGoDrawTPGTHorizontalStartXCoord.AutoSize = true;
            this.lblGoDrawTPGTHorizontalStartXCoord.Location = new System.Drawing.Point(134, 23);
            this.lblGoDrawTPGTHorizontalStartXCoord.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTHorizontalStartXCoord.Name = "lblGoDrawTPGTHorizontalStartXCoord";
            this.lblGoDrawTPGTHorizontalStartXCoord.Size = new System.Drawing.Size(26, 17);
            this.lblGoDrawTPGTHorizontalStartXCoord.TabIndex = 17;
            this.lblGoDrawTPGTHorizontalStartXCoord.Text = "X=";
            // 
            // lblGoDrawTPGTContactZServoValue
            // 
            this.lblGoDrawTPGTContactZServoValue.AutoSize = true;
            this.lblGoDrawTPGTContactZServoValue.Location = new System.Drawing.Point(11, 202);
            this.lblGoDrawTPGTContactZServoValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGoDrawTPGTContactZServoValue.Name = "lblGoDrawTPGTContactZServoValue";
            this.lblGoDrawTPGTContactZServoValue.Size = new System.Drawing.Size(112, 17);
            this.lblGoDrawTPGTContactZServoValue.TabIndex = 22;
            this.lblGoDrawTPGTContactZServoValue.Text = "Contact Z Servo :";
            // 
            // tbxGoDrawTPGTContactZServoValue
            // 
            this.tbxGoDrawTPGTContactZServoValue.Location = new System.Drawing.Point(125, 199);
            this.tbxGoDrawTPGTContactZServoValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxGoDrawTPGTContactZServoValue.Name = "tbxGoDrawTPGTContactZServoValue";
            this.tbxGoDrawTPGTContactZServoValue.Size = new System.Drawing.Size(52, 25);
            this.tbxGoDrawTPGTContactZServoValue.TabIndex = 4;
            this.tbxGoDrawTPGTContactZServoValue.Text = "9500";
            // 
            // tpgSpeed
            // 
            this.tpgSpeed.BackColor = System.Drawing.SystemColors.Control;
            this.tpgSpeed.Controls.Add(this.gbxSpeedSetting);
            this.tpgSpeed.Location = new System.Drawing.Point(4, 26);
            this.tpgSpeed.Margin = new System.Windows.Forms.Padding(2);
            this.tpgSpeed.Name = "tpgSpeed";
            this.tpgSpeed.Size = new System.Drawing.Size(878, 466);
            this.tpgSpeed.TabIndex = 5;
            this.tpgSpeed.Text = "Speed Setting";
            // 
            // gbxSpeedSetting
            // 
            this.gbxSpeedSetting.Controls.Add(this.tbxTTSlantSpeed);
            this.gbxSpeedSetting.Controls.Add(this.lblTTSlantSpeed);
            this.gbxSpeedSetting.Controls.Add(this.lblTPGTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.tbxTPGTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.tbxLTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.lblLTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.lblPCTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.tbxPCTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.lblDGTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.tbxDGTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.tbxTTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.lblTTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.lblDTSpeed);
            this.gbxSpeedSetting.Controls.Add(this.tbxDTSpeed);
            this.gbxSpeedSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxSpeedSetting.Location = new System.Drawing.Point(0, 0);
            this.gbxSpeedSetting.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxSpeedSetting.Name = "gbxSpeedSetting";
            this.gbxSpeedSetting.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxSpeedSetting.Size = new System.Drawing.Size(878, 466);
            this.gbxSpeedSetting.TabIndex = 10;
            this.gbxSpeedSetting.TabStop = false;
            this.gbxSpeedSetting.Text = "Speed Setting";
            // 
            // tbxTTSlantSpeed
            // 
            this.tbxTTSlantSpeed.Location = new System.Drawing.Point(550, 146);
            this.tbxTTSlantSpeed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTTSlantSpeed.Name = "tbxTTSlantSpeed";
            this.tbxTTSlantSpeed.Size = new System.Drawing.Size(52, 25);
            this.tbxTTSlantSpeed.TabIndex = 53;
            this.tbxTTSlantSpeed.Text = "10";
            // 
            // lblTTSlantSpeed
            // 
            this.lblTTSlantSpeed.AutoSize = true;
            this.lblTTSlantSpeed.Location = new System.Drawing.Point(326, 148);
            this.lblTTSlantSpeed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTTSlantSpeed.Name = "lblTTSlantSpeed";
            this.lblTTSlantSpeed.Size = new System.Drawing.Size(205, 17);
            this.lblTTSlantSpeed.TabIndex = 52;
            this.lblTTSlantSpeed.Text = "Tilt Tuning Slant Speed(mm/sec) :";
            // 
            // lblTPGTSpeed
            // 
            this.lblTPGTSpeed.AutoSize = true;
            this.lblTPGTSpeed.Location = new System.Drawing.Point(6, 54);
            this.lblTPGTSpeed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTPGTSpeed.Name = "lblTPGTSpeed";
            this.lblTPGTSpeed.Size = new System.Drawing.Size(207, 17);
            this.lblTPGTSpeed.TabIndex = 51;
            this.lblTPGTSpeed.Text = "TP_Gain Tuning Speed(mm/sec) :";
            // 
            // tbxTPGTSpeed
            // 
            this.tbxTPGTSpeed.Location = new System.Drawing.Point(230, 50);
            this.tbxTPGTSpeed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTPGTSpeed.Name = "tbxTPGTSpeed";
            this.tbxTPGTSpeed.Size = new System.Drawing.Size(52, 25);
            this.tbxTPGTSpeed.TabIndex = 50;
            this.tbxTPGTSpeed.Text = "0.5";
            // 
            // tbxLTSpeed
            // 
            this.tbxLTSpeed.Location = new System.Drawing.Point(230, 178);
            this.tbxLTSpeed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxLTSpeed.Name = "tbxLTSpeed";
            this.tbxLTSpeed.Size = new System.Drawing.Size(52, 25);
            this.tbxLTSpeed.TabIndex = 49;
            this.tbxLTSpeed.Text = "0.1";
            // 
            // lblLTSpeed
            // 
            this.lblLTSpeed.AutoSize = true;
            this.lblLTSpeed.Location = new System.Drawing.Point(6, 180);
            this.lblLTSpeed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLTSpeed.Name = "lblLTSpeed";
            this.lblLTSpeed.Size = new System.Drawing.Size(206, 17);
            this.lblLTSpeed.TabIndex = 48;
            this.lblLTSpeed.Text = "Linearity Tuning Speed(mm/sec) :";
            // 
            // lblPCTSpeed
            // 
            this.lblPCTSpeed.AutoSize = true;
            this.lblPCTSpeed.Location = new System.Drawing.Point(6, 86);
            this.lblPCTSpeed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPCTSpeed.Name = "lblPCTSpeed";
            this.lblPCTSpeed.Size = new System.Drawing.Size(222, 17);
            this.lblPCTSpeed.TabIndex = 47;
            this.lblPCTSpeed.Text = "PeakCheck Tuning Speed(mm/sec) :";
            // 
            // tbxPCTSpeed
            // 
            this.tbxPCTSpeed.Location = new System.Drawing.Point(230, 82);
            this.tbxPCTSpeed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxPCTSpeed.Name = "tbxPCTSpeed";
            this.tbxPCTSpeed.Size = new System.Drawing.Size(52, 25);
            this.tbxPCTSpeed.TabIndex = 46;
            this.tbxPCTSpeed.Text = "10";
            // 
            // lblDGTSpeed
            // 
            this.lblDGTSpeed.AutoSize = true;
            this.lblDGTSpeed.Location = new System.Drawing.Point(6, 22);
            this.lblDGTSpeed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDGTSpeed.Name = "lblDGTSpeed";
            this.lblDGTSpeed.Size = new System.Drawing.Size(205, 17);
            this.lblDGTSpeed.TabIndex = 45;
            this.lblDGTSpeed.Text = "DigiGain Tuning Speed(mm/sec) :";
            // 
            // tbxDGTSpeed
            // 
            this.tbxDGTSpeed.Location = new System.Drawing.Point(230, 18);
            this.tbxDGTSpeed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxDGTSpeed.Name = "tbxDGTSpeed";
            this.tbxDGTSpeed.Size = new System.Drawing.Size(52, 25);
            this.tbxDGTSpeed.TabIndex = 44;
            this.tbxDGTSpeed.Text = "0.5";
            // 
            // tbxTTSpeed
            // 
            this.tbxTTSpeed.Location = new System.Drawing.Point(230, 146);
            this.tbxTTSpeed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTTSpeed.Name = "tbxTTSpeed";
            this.tbxTTSpeed.Size = new System.Drawing.Size(52, 25);
            this.tbxTTSpeed.TabIndex = 43;
            this.tbxTTSpeed.Text = "0.5";
            // 
            // lblTTSpeed
            // 
            this.lblTTSpeed.AutoSize = true;
            this.lblTTSpeed.Location = new System.Drawing.Point(6, 148);
            this.lblTTSpeed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTTSpeed.Name = "lblTTSpeed";
            this.lblTTSpeed.Size = new System.Drawing.Size(172, 17);
            this.lblTTSpeed.TabIndex = 42;
            this.lblTTSpeed.Text = "Tilt Tuning Speed(mm/sec) :";
            // 
            // lblDTSpeed
            // 
            this.lblDTSpeed.AutoSize = true;
            this.lblDTSpeed.Location = new System.Drawing.Point(6, 116);
            this.lblDTSpeed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDTSpeed.Name = "lblDTSpeed";
            this.lblDTSpeed.Size = new System.Drawing.Size(192, 17);
            this.lblDTSpeed.TabIndex = 41;
            this.lblDTSpeed.Text = "Digital Tuning Speed(mm/sec) :";
            // 
            // tbxDTSpeed
            // 
            this.tbxDTSpeed.Location = new System.Drawing.Point(230, 114);
            this.tbxDTSpeed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxDTSpeed.Name = "tbxDTSpeed";
            this.tbxDTSpeed.Size = new System.Drawing.Size(52, 25);
            this.tbxDTSpeed.TabIndex = 40;
            this.tbxDTSpeed.Text = "10";
            // 
            // tpgColorPattern
            // 
            this.tpgColorPattern.BackColor = System.Drawing.SystemColors.Control;
            this.tpgColorPattern.Controls.Add(this.rbtnDisable);
            this.tpgColorPattern.Controls.Add(this.rbtnMonitorOFF);
            this.tpgColorPattern.Controls.Add(this.chbxDisplayTimeAndReportNumber);
            this.tpgColorPattern.Controls.Add(this.chbxDisplayProgressStatus);
            this.tpgColorPattern.Controls.Add(this.gbxColorPattern);
            this.tpgColorPattern.Controls.Add(this.rbtnPattern);
            this.tpgColorPattern.Controls.Add(this.rbtnColor);
            this.tpgColorPattern.Location = new System.Drawing.Point(4, 26);
            this.tpgColorPattern.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tpgColorPattern.Name = "tpgColorPattern";
            this.tpgColorPattern.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tpgColorPattern.Size = new System.Drawing.Size(878, 466);
            this.tpgColorPattern.TabIndex = 1;
            this.tpgColorPattern.Text = "Color/Pattern Setting";
            // 
            // rbtnDisable
            // 
            this.rbtnDisable.AutoSize = true;
            this.rbtnDisable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnDisable.Location = new System.Drawing.Point(11, 14);
            this.rbtnDisable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbtnDisable.Name = "rbtnDisable";
            this.rbtnDisable.Size = new System.Drawing.Size(72, 23);
            this.rbtnDisable.TabIndex = 14;
            this.rbtnDisable.Text = "Disable";
            this.rbtnDisable.UseVisualStyleBackColor = true;
            this.rbtnDisable.CheckedChanged += new System.EventHandler(this.rbtnDisable_CheckedChanged);
            // 
            // rbtnMonitorOFF
            // 
            this.rbtnMonitorOFF.AutoSize = true;
            this.rbtnMonitorOFF.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnMonitorOFF.Location = new System.Drawing.Point(257, 14);
            this.rbtnMonitorOFF.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbtnMonitorOFF.Name = "rbtnMonitorOFF";
            this.rbtnMonitorOFF.Size = new System.Drawing.Size(110, 23);
            this.rbtnMonitorOFF.TabIndex = 13;
            this.rbtnMonitorOFF.Text = "Monitor OFF";
            this.rbtnMonitorOFF.UseVisualStyleBackColor = true;
            this.rbtnMonitorOFF.Visible = false;
            // 
            // chbxDisplayTimeAndReportNumber
            // 
            this.chbxDisplayTimeAndReportNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chbxDisplayTimeAndReportNumber.AutoSize = true;
            this.chbxDisplayTimeAndReportNumber.Checked = true;
            this.chbxDisplayTimeAndReportNumber.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxDisplayTimeAndReportNumber.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbxDisplayTimeAndReportNumber.Location = new System.Drawing.Point(631, 3);
            this.chbxDisplayTimeAndReportNumber.Name = "chbxDisplayTimeAndReportNumber";
            this.chbxDisplayTimeAndReportNumber.Size = new System.Drawing.Size(232, 23);
            this.chbxDisplayTimeAndReportNumber.TabIndex = 12;
            this.chbxDisplayTimeAndReportNumber.Text = "Display Time and Report Number";
            this.chbxDisplayTimeAndReportNumber.UseVisualStyleBackColor = true;
            // 
            // chbxDisplayProgressStatus
            // 
            this.chbxDisplayProgressStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chbxDisplayProgressStatus.AutoSize = true;
            this.chbxDisplayProgressStatus.Checked = true;
            this.chbxDisplayProgressStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxDisplayProgressStatus.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbxDisplayProgressStatus.Location = new System.Drawing.Point(631, 28);
            this.chbxDisplayProgressStatus.Name = "chbxDisplayProgressStatus";
            this.chbxDisplayProgressStatus.Size = new System.Drawing.Size(171, 23);
            this.chbxDisplayProgressStatus.TabIndex = 11;
            this.chbxDisplayProgressStatus.Text = "Display Progress Status";
            this.chbxDisplayProgressStatus.UseVisualStyleBackColor = true;
            // 
            // gbxColorPattern
            // 
            this.gbxColorPattern.Controls.Add(this.lblScreenSize);
            this.gbxColorPattern.Controls.Add(this.btnPreview);
            this.gbxColorPattern.Controls.Add(this.picbxManualPattern);
            this.gbxColorPattern.Controls.Add(this.btnPHCKPatternOption);
            this.gbxColorPattern.Controls.Add(this.rbtnManual);
            this.gbxColorPattern.Controls.Add(this.rbtnPHCKPattern);
            this.gbxColorPattern.Controls.Add(this.tbxPatternPath);
            this.gbxColorPattern.Controls.Add(this.pnlDisplay);
            this.gbxColorPattern.Controls.Add(this.btnColorSelect);
            this.gbxColorPattern.Controls.Add(this.btnPatternPath);
            this.gbxColorPattern.Controls.Add(this.cbxColorSelect);
            this.gbxColorPattern.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbxColorPattern.Location = new System.Drawing.Point(3, 64);
            this.gbxColorPattern.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxColorPattern.Name = "gbxColorPattern";
            this.gbxColorPattern.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbxColorPattern.Size = new System.Drawing.Size(872, 398);
            this.gbxColorPattern.TabIndex = 10;
            this.gbxColorPattern.TabStop = false;
            this.gbxColorPattern.Text = "Color / Pattern";
            // 
            // lblScreenSize
            // 
            this.lblScreenSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblScreenSize.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScreenSize.Location = new System.Drawing.Point(446, 65);
            this.lblScreenSize.Name = "lblScreenSize";
            this.lblScreenSize.Size = new System.Drawing.Size(260, 21);
            this.lblScreenSize.TabIndex = 16;
            this.lblScreenSize.Text = "Screen Size : ";
            this.lblScreenSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Location = new System.Drawing.Point(758, 93);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(94, 30);
            this.btnPreview.TabIndex = 15;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // picbxManualPattern
            // 
            this.picbxManualPattern.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picbxManualPattern.Location = new System.Drawing.Point(7, 48);
            this.picbxManualPattern.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picbxManualPattern.Name = "picbxManualPattern";
            this.picbxManualPattern.Size = new System.Drawing.Size(121, 54);
            this.picbxManualPattern.TabIndex = 14;
            this.picbxManualPattern.TabStop = false;
            // 
            // btnPHCKPatternOption
            // 
            this.btnPHCKPatternOption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPHCKPatternOption.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPHCKPatternOption.Location = new System.Drawing.Point(713, 61);
            this.btnPHCKPatternOption.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPHCKPatternOption.Name = "btnPHCKPatternOption";
            this.btnPHCKPatternOption.Size = new System.Drawing.Size(140, 30);
            this.btnPHCKPatternOption.TabIndex = 13;
            this.btnPHCKPatternOption.Text = "PHCK Pattern Option";
            this.btnPHCKPatternOption.UseVisualStyleBackColor = true;
            this.btnPHCKPatternOption.Click += new System.EventHandler(this.btnPHCKPatternOption_Click);
            // 
            // rbtnManual
            // 
            this.rbtnManual.AutoSize = true;
            this.rbtnManual.Location = new System.Drawing.Point(130, 24);
            this.rbtnManual.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbtnManual.Name = "rbtnManual";
            this.rbtnManual.Size = new System.Drawing.Size(70, 21);
            this.rbtnManual.TabIndex = 12;
            this.rbtnManual.Text = "Manual";
            this.rbtnManual.UseVisualStyleBackColor = true;
            this.rbtnManual.CheckedChanged += new System.EventHandler(this.rbtnManual_CheckedChanged);
            // 
            // rbtnPHCKPattern
            // 
            this.rbtnPHCKPattern.AutoSize = true;
            this.rbtnPHCKPattern.Checked = true;
            this.rbtnPHCKPattern.Location = new System.Drawing.Point(9, 24);
            this.rbtnPHCKPattern.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbtnPHCKPattern.Name = "rbtnPHCKPattern";
            this.rbtnPHCKPattern.Size = new System.Drawing.Size(114, 21);
            this.rbtnPHCKPattern.TabIndex = 11;
            this.rbtnPHCKPattern.TabStop = true;
            this.rbtnPHCKPattern.Text = "PHCK Pattern";
            this.rbtnPHCKPattern.UseVisualStyleBackColor = true;
            this.rbtnPHCKPattern.CheckedChanged += new System.EventHandler(this.rbtnPHCKPattern_CheckedChanged);
            // 
            // tbxPatternPath
            // 
            this.tbxPatternPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxPatternPath.Location = new System.Drawing.Point(141, 63);
            this.tbxPatternPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxPatternPath.Name = "tbxPatternPath";
            this.tbxPatternPath.Size = new System.Drawing.Size(565, 25);
            this.tbxPatternPath.TabIndex = 7;
            // 
            // pnlDisplay
            // 
            this.pnlDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDisplay.Location = new System.Drawing.Point(10, 60);
            this.pnlDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlDisplay.Name = "pnlDisplay";
            this.pnlDisplay.Size = new System.Drawing.Size(112, 28);
            this.pnlDisplay.TabIndex = 10;
            // 
            // btnColorSelect
            // 
            this.btnColorSelect.Location = new System.Drawing.Point(252, 61);
            this.btnColorSelect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnColorSelect.Name = "btnColorSelect";
            this.btnColorSelect.Size = new System.Drawing.Size(94, 30);
            this.btnColorSelect.TabIndex = 9;
            this.btnColorSelect.Text = "Color Select";
            this.btnColorSelect.UseVisualStyleBackColor = true;
            this.btnColorSelect.Click += new System.EventHandler(this.btnColorSelect_Click);
            // 
            // btnPatternPath
            // 
            this.btnPatternPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPatternPath.Location = new System.Drawing.Point(758, 60);
            this.btnPatternPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPatternPath.Name = "btnPatternPath";
            this.btnPatternPath.Size = new System.Drawing.Size(94, 30);
            this.btnPatternPath.TabIndex = 1;
            this.btnPatternPath.Text = "Pattern Select";
            this.btnPatternPath.UseVisualStyleBackColor = true;
            this.btnPatternPath.Click += new System.EventHandler(this.btnPatternPath_Click);
            // 
            // cbxColorSelect
            // 
            this.cbxColorSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxColorSelect.FormattingEnabled = true;
            this.cbxColorSelect.Items.AddRange(new object[] {
            "No Color",
            "Manual",
            "Black",
            "White",
            "Red",
            "Dark Green",
            "Blue",
            "Green",
            "Yellow",
            "Cyan",
            "Megenta"});
            this.cbxColorSelect.Location = new System.Drawing.Point(133, 63);
            this.cbxColorSelect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbxColorSelect.Name = "cbxColorSelect";
            this.cbxColorSelect.Size = new System.Drawing.Size(111, 25);
            this.cbxColorSelect.TabIndex = 8;
            this.cbxColorSelect.SelectedIndexChanged += new System.EventHandler(this.cbxColorSelect_SelectedIndexChanged);
            // 
            // rbtnPattern
            // 
            this.rbtnPattern.AutoSize = true;
            this.rbtnPattern.Checked = true;
            this.rbtnPattern.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnPattern.Location = new System.Drawing.Point(171, 14);
            this.rbtnPattern.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbtnPattern.Name = "rbtnPattern";
            this.rbtnPattern.Size = new System.Drawing.Size(70, 23);
            this.rbtnPattern.TabIndex = 9;
            this.rbtnPattern.TabStop = true;
            this.rbtnPattern.Text = "Pattern";
            this.rbtnPattern.UseVisualStyleBackColor = true;
            this.rbtnPattern.CheckedChanged += new System.EventHandler(this.rbtnPattern_CheckedChanged);
            // 
            // rbtnColor
            // 
            this.rbtnColor.AutoSize = true;
            this.rbtnColor.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnColor.Location = new System.Drawing.Point(95, 14);
            this.rbtnColor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbtnColor.Name = "rbtnColor";
            this.rbtnColor.Size = new System.Drawing.Size(62, 23);
            this.rbtnColor.TabIndex = 8;
            this.rbtnColor.Text = "Color";
            this.rbtnColor.UseVisualStyleBackColor = true;
            this.rbtnColor.CheckedChanged += new System.EventHandler(this.rbtnColor_CheckedChanged);
            // 
            // tpgGen8Command
            // 
            this.tpgGen8Command.BackColor = System.Drawing.SystemColors.Control;
            this.tpgGen8Command.Controls.Add(this.gbxCommandSetting);
            this.tpgGen8Command.Location = new System.Drawing.Point(4, 26);
            this.tpgGen8Command.Margin = new System.Windows.Forms.Padding(2);
            this.tpgGen8Command.Name = "tpgGen8Command";
            this.tpgGen8Command.Size = new System.Drawing.Size(878, 466);
            this.tpgGen8Command.TabIndex = 6;
            this.tpgGen8Command.Text = "Gen8 Command Setting";
            // 
            // gbxCommandSetting
            // 
            this.gbxCommandSetting.Controls.Add(this.cbxGen8FilterType);
            this.gbxCommandSetting.Controls.Add(this.lblGen8FilterType);
            this.gbxCommandSetting.Controls.Add(this.cbxGen8AFEType);
            this.gbxCommandSetting.Controls.Add(this.lblGen8AFEType);
            this.gbxCommandSetting.Controls.Add(this.btnUserDefinedSelect);
            this.gbxCommandSetting.Controls.Add(this.lblUserDefinedPath);
            this.gbxCommandSetting.Controls.Add(this.tbxUserDeifnedPath);
            this.gbxCommandSetting.Controls.Add(this.cbxCommandScriptType);
            this.gbxCommandSetting.Controls.Add(this.lblCommandScriptType);
            this.gbxCommandSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxCommandSetting.Location = new System.Drawing.Point(0, 0);
            this.gbxCommandSetting.Name = "gbxCommandSetting";
            this.gbxCommandSetting.Size = new System.Drawing.Size(878, 466);
            this.gbxCommandSetting.TabIndex = 12;
            this.gbxCommandSetting.TabStop = false;
            this.gbxCommandSetting.Text = "Command Setting";
            // 
            // cbxGen8FilterType
            // 
            this.cbxGen8FilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxGen8FilterType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxGen8FilterType.FormattingEnabled = true;
            this.cbxGen8FilterType.Items.AddRange(new object[] {
            "NA",
            "Disable Filter",
            "0~300KHz LPF",
            "0~75KHz LPF"});
            this.cbxGen8FilterType.Location = new System.Drawing.Point(91, 57);
            this.cbxGen8FilterType.Name = "cbxGen8FilterType";
            this.cbxGen8FilterType.Size = new System.Drawing.Size(132, 25);
            this.cbxGen8FilterType.TabIndex = 27;
            // 
            // lblGen8FilterType
            // 
            this.lblGen8FilterType.AutoSize = true;
            this.lblGen8FilterType.Location = new System.Drawing.Point(8, 59);
            this.lblGen8FilterType.Name = "lblGen8FilterType";
            this.lblGen8FilterType.Size = new System.Drawing.Size(78, 17);
            this.lblGen8FilterType.TabIndex = 26;
            this.lblGen8FilterType.Text = "Filter Type :";
            // 
            // cbxGen8AFEType
            // 
            this.cbxGen8AFEType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxGen8AFEType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxGen8AFEType.FormattingEnabled = true;
            this.cbxGen8AFEType.Items.AddRange(new object[] {
            "NA",
            "DT Mode",
            "CT Mode"});
            this.cbxGen8AFEType.Location = new System.Drawing.Point(91, 22);
            this.cbxGen8AFEType.Name = "cbxGen8AFEType";
            this.cbxGen8AFEType.Size = new System.Drawing.Size(132, 25);
            this.cbxGen8AFEType.TabIndex = 25;
            // 
            // lblGen8AFEType
            // 
            this.lblGen8AFEType.AutoSize = true;
            this.lblGen8AFEType.Location = new System.Drawing.Point(8, 25);
            this.lblGen8AFEType.Name = "lblGen8AFEType";
            this.lblGen8AFEType.Size = new System.Drawing.Size(76, 17);
            this.lblGen8AFEType.TabIndex = 24;
            this.lblGen8AFEType.Text = "AFE Type :";
            // 
            // btnUserDefinedSelect
            // 
            this.btnUserDefinedSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUserDefinedSelect.Location = new System.Drawing.Point(756, 131);
            this.btnUserDefinedSelect.Margin = new System.Windows.Forms.Padding(2);
            this.btnUserDefinedSelect.Name = "btnUserDefinedSelect";
            this.btnUserDefinedSelect.Size = new System.Drawing.Size(81, 32);
            this.btnUserDefinedSelect.TabIndex = 23;
            this.btnUserDefinedSelect.Text = "Select";
            this.btnUserDefinedSelect.UseVisualStyleBackColor = true;
            this.btnUserDefinedSelect.Click += new System.EventHandler(this.btnUserDefinedSelect_Click);
            // 
            // lblUserDefinedPath
            // 
            this.lblUserDefinedPath.AutoSize = true;
            this.lblUserDefinedPath.Location = new System.Drawing.Point(7, 138);
            this.lblUserDefinedPath.Name = "lblUserDefinedPath";
            this.lblUserDefinedPath.Size = new System.Drawing.Size(126, 17);
            this.lblUserDefinedPath.TabIndex = 22;
            this.lblUserDefinedPath.Text = "User Defined Path :";
            // 
            // tbxUserDeifnedPath
            // 
            this.tbxUserDeifnedPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxUserDeifnedPath.Location = new System.Drawing.Point(136, 135);
            this.tbxUserDeifnedPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxUserDeifnedPath.Name = "tbxUserDeifnedPath";
            this.tbxUserDeifnedPath.Size = new System.Drawing.Size(603, 25);
            this.tbxUserDeifnedPath.TabIndex = 21;
            // 
            // cbxCommandScriptType
            // 
            this.cbxCommandScriptType.AutoCompleteCustomSource.AddRange(new string[] {
            "0 [100 - (Value/Max)]",
            "1 [N/A]"});
            this.cbxCommandScriptType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCommandScriptType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxCommandScriptType.FormattingEnabled = true;
            this.cbxCommandScriptType.Items.AddRange(new object[] {
            "Normal",
            "Normal + UserDefined",
            "UserDefined"});
            this.cbxCommandScriptType.Location = new System.Drawing.Point(164, 91);
            this.cbxCommandScriptType.Name = "cbxCommandScriptType";
            this.cbxCommandScriptType.Size = new System.Drawing.Size(193, 25);
            this.cbxCommandScriptType.TabIndex = 20;
            // 
            // lblCommandScriptType
            // 
            this.lblCommandScriptType.AutoSize = true;
            this.lblCommandScriptType.Location = new System.Drawing.Point(7, 94);
            this.lblCommandScriptType.Name = "lblCommandScriptType";
            this.lblCommandScriptType.Size = new System.Drawing.Size(146, 17);
            this.lblCommandScriptType.TabIndex = 19;
            this.lblCommandScriptType.Text = "Command Script Type :";
            // 
            // tpgOtherSetting
            // 
            this.tpgOtherSetting.BackColor = System.Drawing.SystemColors.Control;
            this.tpgOtherSetting.Controls.Add(this.tctrlOtherSetting);
            this.tpgOtherSetting.Location = new System.Drawing.Point(4, 26);
            this.tpgOtherSetting.Name = "tpgOtherSetting";
            this.tpgOtherSetting.Size = new System.Drawing.Size(878, 466);
            this.tpgOtherSetting.TabIndex = 2;
            this.tpgOtherSetting.Text = "Other Setting";
            // 
            // tctrlOtherSetting
            // 
            this.tctrlOtherSetting.Controls.Add(this.tpgNoiseAndTNStep);
            this.tctrlOtherSetting.Controls.Add(this.tabDGTStep);
            this.tctrlOtherSetting.Controls.Add(this.tpgTPGTStep);
            this.tctrlOtherSetting.Controls.Add(this.tpgDTAndPCTStep);
            this.tctrlOtherSetting.Controls.Add(this.tpgPCTStep);
            this.tctrlOtherSetting.Controls.Add(this.tpgDigitalTuningStep);
            this.tctrlOtherSetting.Controls.Add(this.tpgTiltTuningStep);
            this.tctrlOtherSetting.Controls.Add(this.tpgPTStep);
            this.tctrlOtherSetting.Controls.Add(this.tpgLTStep);
            this.tctrlOtherSetting.Controls.Add(this.tpgFWParameterSetting);
            this.tctrlOtherSetting.Controls.Add(this.tpgOther);
            this.tctrlOtherSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tctrlOtherSetting.Location = new System.Drawing.Point(0, 0);
            this.tctrlOtherSetting.Name = "tctrlOtherSetting";
            this.tctrlOtherSetting.SelectedIndex = 0;
            this.tctrlOtherSetting.Size = new System.Drawing.Size(878, 466);
            this.tctrlOtherSetting.TabIndex = 0;
            this.tctrlOtherSetting.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tctrlOtherSetting_Selecting);
            // 
            // tpgNoiseAndTNStep
            // 
            this.tpgNoiseAndTNStep.AutoScroll = true;
            this.tpgNoiseAndTNStep.BackColor = System.Drawing.SystemColors.Control;
            this.tpgNoiseAndTNStep.Controls.Add(this.gbxNoiseMaxValueOverWarningHB);
            this.tpgNoiseAndTNStep.Controls.Add(this.tbxNoiseNoReportInterruptTime);
            this.tpgNoiseAndTNStep.Controls.Add(this.lblNoiseNoReportInterruptTime);
            this.tpgNoiseAndTNStep.Controls.Add(this.tbxNoiseInnerRefValueHB);
            this.tpgNoiseAndTNStep.Controls.Add(this.lblNoiseInnerRefValueHB);
            this.tpgNoiseAndTNStep.Controls.Add(this.tbxNoiseProcessReportNumber);
            this.tpgNoiseAndTNStep.Controls.Add(this.lblNoiseProcessReportNumber);
            this.tpgNoiseAndTNStep.Controls.Add(this.tbxNoiseValidReportNumber);
            this.tpgNoiseAndTNStep.Controls.Add(this.lblNoiseValidReportNumber);
            this.tpgNoiseAndTNStep.Controls.Add(this.tbxNoiseTimeout);
            this.tpgNoiseAndTNStep.Controls.Add(this.lblNoiseTimeout);
            this.tpgNoiseAndTNStep.Controls.Add(this.tbxNoiseReportNumber);
            this.tpgNoiseAndTNStep.Controls.Add(this.lblNoiseReportNumber);
            this.tpgNoiseAndTNStep.Location = new System.Drawing.Point(4, 26);
            this.tpgNoiseAndTNStep.Name = "tpgNoiseAndTNStep";
            this.tpgNoiseAndTNStep.Padding = new System.Windows.Forms.Padding(3);
            this.tpgNoiseAndTNStep.Size = new System.Drawing.Size(870, 436);
            this.tpgNoiseAndTNStep.TabIndex = 0;
            this.tpgNoiseAndTNStep.Text = "Noise & Tilt Noise";
            // 
            // tbxNoiseMaxValueOverWarningAbsValueHB
            // 
            this.tbxNoiseMaxValueOverWarningAbsValueHB.Location = new System.Drawing.Point(368, 64);
            this.tbxNoiseMaxValueOverWarningAbsValueHB.Name = "tbxNoiseMaxValueOverWarningAbsValueHB";
            this.tbxNoiseMaxValueOverWarningAbsValueHB.Size = new System.Drawing.Size(81, 25);
            this.tbxNoiseMaxValueOverWarningAbsValueHB.TabIndex = 24;
            this.tbxNoiseMaxValueOverWarningAbsValueHB.Text = "-1";
            // 
            // lblNoiseMaxValueOverWarningAbsValueHB
            // 
            this.lblNoiseMaxValueOverWarningAbsValueHB.AutoSize = true;
            this.lblNoiseMaxValueOverWarningAbsValueHB.Location = new System.Drawing.Point(6, 68);
            this.lblNoiseMaxValueOverWarningAbsValueHB.Name = "lblNoiseMaxValueOverWarningAbsValueHB";
            this.lblNoiseMaxValueOverWarningAbsValueHB.Size = new System.Drawing.Size(283, 17);
            this.lblNoiseMaxValueOverWarningAbsValueHB.TabIndex = 23;
            this.lblNoiseMaxValueOverWarningAbsValueHB.Text = "Max Value Over Warning Absolute Value HB :";
            // 
            // tbxNoiseNoReportInterruptTime
            // 
            this.tbxNoiseNoReportInterruptTime.Location = new System.Drawing.Point(253, 185);
            this.tbxNoiseNoReportInterruptTime.Name = "tbxNoiseNoReportInterruptTime";
            this.tbxNoiseNoReportInterruptTime.Size = new System.Drawing.Size(81, 25);
            this.tbxNoiseNoReportInterruptTime.TabIndex = 22;
            this.tbxNoiseNoReportInterruptTime.Text = "5";
            // 
            // lblNoiseNoReportInterruptTime
            // 
            this.lblNoiseNoReportInterruptTime.AutoSize = true;
            this.lblNoiseNoReportInterruptTime.Location = new System.Drawing.Point(4, 189);
            this.lblNoiseNoReportInterruptTime.Name = "lblNoiseNoReportInterruptTime";
            this.lblNoiseNoReportInterruptTime.Size = new System.Drawing.Size(241, 17);
            this.lblNoiseNoReportInterruptTime.TabIndex = 21;
            this.lblNoiseNoReportInterruptTime.Text = "Noise No Report Interrupt Time  (sec) :";
            // 
            // tbxNoiseInnerRefValueHB
            // 
            this.tbxNoiseInnerRefValueHB.Location = new System.Drawing.Point(252, 150);
            this.tbxNoiseInnerRefValueHB.Name = "tbxNoiseInnerRefValueHB";
            this.tbxNoiseInnerRefValueHB.Size = new System.Drawing.Size(81, 25);
            this.tbxNoiseInnerRefValueHB.TabIndex = 20;
            this.tbxNoiseInnerRefValueHB.Text = "1000";
            // 
            // lblNoiseInnerRefValueHB
            // 
            this.lblNoiseInnerRefValueHB.AutoSize = true;
            this.lblNoiseInnerRefValueHB.Location = new System.Drawing.Point(4, 154);
            this.lblNoiseInnerRefValueHB.Name = "lblNoiseInnerRefValueHB";
            this.lblNoiseInnerRefValueHB.Size = new System.Drawing.Size(204, 17);
            this.lblNoiseInnerRefValueHB.TabIndex = 19;
            this.lblNoiseInnerRefValueHB.Text = "Noise InnerReferenceValue HB :";
            // 
            // tbxNoiseProcessReportNumber
            // 
            this.tbxNoiseProcessReportNumber.Location = new System.Drawing.Point(251, 79);
            this.tbxNoiseProcessReportNumber.Name = "tbxNoiseProcessReportNumber";
            this.tbxNoiseProcessReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxNoiseProcessReportNumber.TabIndex = 9;
            this.tbxNoiseProcessReportNumber.Text = "-1";
            // 
            // lblNoiseProcessReportNumber
            // 
            this.lblNoiseProcessReportNumber.AutoSize = true;
            this.lblNoiseProcessReportNumber.Location = new System.Drawing.Point(4, 84);
            this.lblNoiseProcessReportNumber.Name = "lblNoiseProcessReportNumber";
            this.lblNoiseProcessReportNumber.Size = new System.Drawing.Size(196, 17);
            this.lblNoiseProcessReportNumber.TabIndex = 8;
            this.lblNoiseProcessReportNumber.Text = "Noise Process Report Number :";
            // 
            // tbxNoiseValidReportNumber
            // 
            this.tbxNoiseValidReportNumber.Location = new System.Drawing.Point(250, 45);
            this.tbxNoiseValidReportNumber.Name = "tbxNoiseValidReportNumber";
            this.tbxNoiseValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxNoiseValidReportNumber.TabIndex = 7;
            this.tbxNoiseValidReportNumber.Text = "5000";
            // 
            // lblNoiseValidReportNumber
            // 
            this.lblNoiseValidReportNumber.AutoSize = true;
            this.lblNoiseValidReportNumber.Location = new System.Drawing.Point(4, 49);
            this.lblNoiseValidReportNumber.Name = "lblNoiseValidReportNumber";
            this.lblNoiseValidReportNumber.Size = new System.Drawing.Size(178, 17);
            this.lblNoiseValidReportNumber.TabIndex = 6;
            this.lblNoiseValidReportNumber.Text = "Noise Valid Report Number :";
            // 
            // tbxNoiseTimeout
            // 
            this.tbxNoiseTimeout.Location = new System.Drawing.Point(252, 115);
            this.tbxNoiseTimeout.Name = "tbxNoiseTimeout";
            this.tbxNoiseTimeout.Size = new System.Drawing.Size(81, 25);
            this.tbxNoiseTimeout.TabIndex = 3;
            this.tbxNoiseTimeout.Text = "240";
            // 
            // lblNoiseTimeout
            // 
            this.lblNoiseTimeout.AutoSize = true;
            this.lblNoiseTimeout.Location = new System.Drawing.Point(4, 119);
            this.lblNoiseTimeout.Name = "lblNoiseTimeout";
            this.lblNoiseTimeout.Size = new System.Drawing.Size(134, 17);
            this.lblNoiseTimeout.TabIndex = 2;
            this.lblNoiseTimeout.Text = "Noise Timeout (sec) :";
            // 
            // tbxNoiseReportNumber
            // 
            this.tbxNoiseReportNumber.Location = new System.Drawing.Point(250, 10);
            this.tbxNoiseReportNumber.Name = "tbxNoiseReportNumber";
            this.tbxNoiseReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxNoiseReportNumber.TabIndex = 1;
            this.tbxNoiseReportNumber.Text = "6000";
            // 
            // lblNoiseReportNumber
            // 
            this.lblNoiseReportNumber.AutoSize = true;
            this.lblNoiseReportNumber.Location = new System.Drawing.Point(4, 14);
            this.lblNoiseReportNumber.Name = "lblNoiseReportNumber";
            this.lblNoiseReportNumber.Size = new System.Drawing.Size(145, 17);
            this.lblNoiseReportNumber.TabIndex = 0;
            this.lblNoiseReportNumber.Text = "Noise Report Number :";
            // 
            // tabDGTStep
            // 
            this.tabDGTStep.BackColor = System.Drawing.SystemColors.Control;
            this.tabDGTStep.Controls.Add(this.tbxDGTDigiGainScaleHB);
            this.tabDGTStep.Controls.Add(this.lblDGTDigiGainScaleHB);
            this.tabDGTStep.Controls.Add(this.tbxDGTDigiGainScaleLB);
            this.tabDGTStep.Controls.Add(this.lblDGTDigiGainScaleLB);
            this.tabDGTStep.Controls.Add(this.tbxDGTTXValidReportNumber);
            this.tabDGTStep.Controls.Add(this.lblDGTTXValidReportNumber);
            this.tabDGTStep.Controls.Add(this.tbxDGTRXValidReportNumber);
            this.tabDGTStep.Controls.Add(this.lblDGTRXValidReportNumber);
            this.tabDGTStep.Controls.Add(this.tbxDGTCompensatePower);
            this.tabDGTStep.Controls.Add(this.lblDGTCompensatePower);
            this.tabDGTStep.Location = new System.Drawing.Point(4, 26);
            this.tabDGTStep.Margin = new System.Windows.Forms.Padding(2);
            this.tabDGTStep.Name = "tabDGTStep";
            this.tabDGTStep.Size = new System.Drawing.Size(870, 436);
            this.tabDGTStep.TabIndex = 7;
            this.tabDGTStep.Text = "DigiGain Tuning";
            // 
            // tbxDGTDigiGainScaleHB
            // 
            this.tbxDGTDigiGainScaleHB.Location = new System.Drawing.Point(173, 98);
            this.tbxDGTDigiGainScaleHB.Name = "tbxDGTDigiGainScaleHB";
            this.tbxDGTDigiGainScaleHB.Size = new System.Drawing.Size(81, 25);
            this.tbxDGTDigiGainScaleHB.TabIndex = 37;
            this.tbxDGTDigiGainScaleHB.Text = "395";
            // 
            // lblDGTDigiGainScaleHB
            // 
            this.lblDGTDigiGainScaleHB.AutoSize = true;
            this.lblDGTDigiGainScaleHB.Location = new System.Drawing.Point(4, 101);
            this.lblDGTDigiGainScaleHB.Name = "lblDGTDigiGainScaleHB";
            this.lblDGTDigiGainScaleHB.Size = new System.Drawing.Size(127, 17);
            this.lblDGTDigiGainScaleHB.TabIndex = 36;
            this.lblDGTDigiGainScaleHB.Text = "DigiGain Scale HB :";
            // 
            // tbxDGTDigiGainScaleLB
            // 
            this.tbxDGTDigiGainScaleLB.Location = new System.Drawing.Point(173, 130);
            this.tbxDGTDigiGainScaleLB.Name = "tbxDGTDigiGainScaleLB";
            this.tbxDGTDigiGainScaleLB.Size = new System.Drawing.Size(81, 25);
            this.tbxDGTDigiGainScaleLB.TabIndex = 35;
            this.tbxDGTDigiGainScaleLB.Text = "5";
            // 
            // lblDGTDigiGainScaleLB
            // 
            this.lblDGTDigiGainScaleLB.AutoSize = true;
            this.lblDGTDigiGainScaleLB.Location = new System.Drawing.Point(4, 132);
            this.lblDGTDigiGainScaleLB.Name = "lblDGTDigiGainScaleLB";
            this.lblDGTDigiGainScaleLB.Size = new System.Drawing.Size(125, 17);
            this.lblDGTDigiGainScaleLB.TabIndex = 34;
            this.lblDGTDigiGainScaleLB.Text = "DigiGain Scale LB :";
            // 
            // tbxDGTTXValidReportNumber
            // 
            this.tbxDGTTXValidReportNumber.Location = new System.Drawing.Point(173, 36);
            this.tbxDGTTXValidReportNumber.Name = "tbxDGTTXValidReportNumber";
            this.tbxDGTTXValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxDGTTXValidReportNumber.TabIndex = 29;
            this.tbxDGTTXValidReportNumber.Text = "1500";
            // 
            // lblDGTTXValidReportNumber
            // 
            this.lblDGTTXValidReportNumber.AutoSize = true;
            this.lblDGTTXValidReportNumber.Location = new System.Drawing.Point(4, 38);
            this.lblDGTTXValidReportNumber.Name = "lblDGTTXValidReportNumber";
            this.lblDGTTXValidReportNumber.Size = new System.Drawing.Size(163, 17);
            this.lblDGTTXValidReportNumber.TabIndex = 28;
            this.lblDGTTXValidReportNumber.Text = "TX Valid Report Number :";
            // 
            // tbxDGTRXValidReportNumber
            // 
            this.tbxDGTRXValidReportNumber.Location = new System.Drawing.Point(173, 5);
            this.tbxDGTRXValidReportNumber.Name = "tbxDGTRXValidReportNumber";
            this.tbxDGTRXValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxDGTRXValidReportNumber.TabIndex = 27;
            this.tbxDGTRXValidReportNumber.Text = "1500";
            // 
            // lblDGTRXValidReportNumber
            // 
            this.lblDGTRXValidReportNumber.AutoSize = true;
            this.lblDGTRXValidReportNumber.Location = new System.Drawing.Point(4, 7);
            this.lblDGTRXValidReportNumber.Name = "lblDGTRXValidReportNumber";
            this.lblDGTRXValidReportNumber.Size = new System.Drawing.Size(164, 17);
            this.lblDGTRXValidReportNumber.TabIndex = 26;
            this.lblDGTRXValidReportNumber.Text = "RX Valid Report Number :";
            // 
            // tbxDGTCompensatePower
            // 
            this.tbxDGTCompensatePower.Location = new System.Drawing.Point(173, 67);
            this.tbxDGTCompensatePower.Name = "tbxDGTCompensatePower";
            this.tbxDGTCompensatePower.Size = new System.Drawing.Size(81, 25);
            this.tbxDGTCompensatePower.TabIndex = 25;
            this.tbxDGTCompensatePower.Text = "24576";
            // 
            // lblDGTCompensatePower
            // 
            this.lblDGTCompensatePower.AutoSize = true;
            this.lblDGTCompensatePower.Location = new System.Drawing.Point(4, 70);
            this.lblDGTCompensatePower.Name = "lblDGTCompensatePower";
            this.lblDGTCompensatePower.Size = new System.Drawing.Size(131, 17);
            this.lblDGTCompensatePower.TabIndex = 24;
            this.lblDGTCompensatePower.Text = "Compensate Power :";
            // 
            // tpgTPGTStep
            // 
            this.tpgTPGTStep.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTPGTStep.Controls.Add(this.cbxTPGTDisplayMessage);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTDisaplayMessage);
            this.tpgTPGTStep.Controls.Add(this.tbxTPGTVerticalRAngle);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTVerticalRAngle);
            this.tpgTPGTStep.Controls.Add(this.tbxTPGTHorizontalRAngle);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTHorizontalRAngle);
            this.tpgTPGTStep.Controls.Add(this.tbxTPGTVAngle);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTVAngle);
            this.tpgTPGTStep.Controls.Add(this.tbxTPGTGainRatio);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTGainRatio);
            this.tpgTPGTStep.Controls.Add(this.tbxTPGTRXEndPin);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTRXEndPin);
            this.tpgTPGTStep.Controls.Add(this.tbxTPGTRXStartPin);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTRXStartPin);
            this.tpgTPGTStep.Controls.Add(this.tbxTPGTTXEndPin);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTTXEndPin);
            this.tpgTPGTStep.Controls.Add(this.tbxTPGTTXStartPin);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTTXStartPin);
            this.tpgTPGTStep.Controls.Add(this.tbxTPGTTXValidReportNumber);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTTXValidReportNumber);
            this.tpgTPGTStep.Controls.Add(this.tbxTPGTRXValidReportNumber);
            this.tpgTPGTStep.Controls.Add(this.lblTPGTRXValidReportNumber);
            this.tpgTPGTStep.Location = new System.Drawing.Point(4, 26);
            this.tpgTPGTStep.Margin = new System.Windows.Forms.Padding(2);
            this.tpgTPGTStep.Name = "tpgTPGTStep";
            this.tpgTPGTStep.Size = new System.Drawing.Size(870, 436);
            this.tpgTPGTStep.TabIndex = 9;
            this.tpgTPGTStep.Text = "TP_Gain Tuning";
            // 
            // cbxTPGTDisplayMessage
            // 
            this.cbxTPGTDisplayMessage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTPGTDisplayMessage.FormattingEnabled = true;
            this.cbxTPGTDisplayMessage.Items.AddRange(new object[] {
            "Disable",
            "Enable"});
            this.cbxTPGTDisplayMessage.Location = new System.Drawing.Point(174, 162);
            this.cbxTPGTDisplayMessage.Margin = new System.Windows.Forms.Padding(2);
            this.cbxTPGTDisplayMessage.Name = "cbxTPGTDisplayMessage";
            this.cbxTPGTDisplayMessage.Size = new System.Drawing.Size(96, 25);
            this.cbxTPGTDisplayMessage.TabIndex = 51;
            // 
            // lblTPGTDisaplayMessage
            // 
            this.lblTPGTDisaplayMessage.AutoSize = true;
            this.lblTPGTDisaplayMessage.Location = new System.Drawing.Point(4, 163);
            this.lblTPGTDisaplayMessage.Name = "lblTPGTDisaplayMessage";
            this.lblTPGTDisaplayMessage.Size = new System.Drawing.Size(116, 17);
            this.lblTPGTDisaplayMessage.TabIndex = 50;
            this.lblTPGTDisaplayMessage.Text = "Display Message :";
            // 
            // tbxTPGTVerticalRAngle
            // 
            this.tbxTPGTVerticalRAngle.Location = new System.Drawing.Point(174, 130);
            this.tbxTPGTVerticalRAngle.Name = "tbxTPGTVerticalRAngle";
            this.tbxTPGTVerticalRAngle.Size = new System.Drawing.Size(81, 25);
            this.tbxTPGTVerticalRAngle.TabIndex = 49;
            this.tbxTPGTVerticalRAngle.Text = "0";
            // 
            // lblTPGTVerticalRAngle
            // 
            this.lblTPGTVerticalRAngle.AutoSize = true;
            this.lblTPGTVerticalRAngle.Location = new System.Drawing.Point(4, 132);
            this.lblTPGTVerticalRAngle.Name = "lblTPGTVerticalRAngle";
            this.lblTPGTVerticalRAngle.Size = new System.Drawing.Size(112, 17);
            this.lblTPGTVerticalRAngle.TabIndex = 48;
            this.lblTPGTVerticalRAngle.Text = "Vertical R Angle :";
            // 
            // tbxTPGTHorizontalRAngle
            // 
            this.tbxTPGTHorizontalRAngle.Location = new System.Drawing.Point(174, 98);
            this.tbxTPGTHorizontalRAngle.Name = "tbxTPGTHorizontalRAngle";
            this.tbxTPGTHorizontalRAngle.Size = new System.Drawing.Size(81, 25);
            this.tbxTPGTHorizontalRAngle.TabIndex = 47;
            this.tbxTPGTHorizontalRAngle.Text = "90";
            // 
            // lblTPGTHorizontalRAngle
            // 
            this.lblTPGTHorizontalRAngle.AutoSize = true;
            this.lblTPGTHorizontalRAngle.Location = new System.Drawing.Point(4, 101);
            this.lblTPGTHorizontalRAngle.Name = "lblTPGTHorizontalRAngle";
            this.lblTPGTHorizontalRAngle.Size = new System.Drawing.Size(127, 17);
            this.lblTPGTHorizontalRAngle.TabIndex = 46;
            this.lblTPGTHorizontalRAngle.Text = "Horizontal R Angle :";
            // 
            // tbxTPGTVAngle
            // 
            this.tbxTPGTVAngle.Location = new System.Drawing.Point(174, 67);
            this.tbxTPGTVAngle.Name = "tbxTPGTVAngle";
            this.tbxTPGTVAngle.Size = new System.Drawing.Size(81, 25);
            this.tbxTPGTVAngle.TabIndex = 45;
            this.tbxTPGTVAngle.Text = "45";
            // 
            // lblTPGTVAngle
            // 
            this.lblTPGTVAngle.AutoSize = true;
            this.lblTPGTVAngle.Location = new System.Drawing.Point(4, 70);
            this.lblTPGTVAngle.Name = "lblTPGTVAngle";
            this.lblTPGTVAngle.Size = new System.Drawing.Size(64, 17);
            this.lblTPGTVAngle.TabIndex = 44;
            this.lblTPGTVAngle.Text = "V Angle :";
            // 
            // tbxTPGTGainRatio
            // 
            this.tbxTPGTGainRatio.Location = new System.Drawing.Point(422, 130);
            this.tbxTPGTGainRatio.Name = "tbxTPGTGainRatio";
            this.tbxTPGTGainRatio.Size = new System.Drawing.Size(81, 25);
            this.tbxTPGTGainRatio.TabIndex = 43;
            this.tbxTPGTGainRatio.Text = "8192";
            // 
            // lblTPGTGainRatio
            // 
            this.lblTPGTGainRatio.AutoSize = true;
            this.lblTPGTGainRatio.Location = new System.Drawing.Point(315, 132);
            this.lblTPGTGainRatio.Name = "lblTPGTGainRatio";
            this.lblTPGTGainRatio.Size = new System.Drawing.Size(77, 17);
            this.lblTPGTGainRatio.TabIndex = 42;
            this.lblTPGTGainRatio.Text = "Gain Ratio :";
            // 
            // tbxTPGTRXEndPin
            // 
            this.tbxTPGTRXEndPin.Location = new System.Drawing.Point(422, 98);
            this.tbxTPGTRXEndPin.Name = "tbxTPGTRXEndPin";
            this.tbxTPGTRXEndPin.Size = new System.Drawing.Size(81, 25);
            this.tbxTPGTRXEndPin.TabIndex = 41;
            this.tbxTPGTRXEndPin.Text = "83";
            // 
            // lblTPGTRXEndPin
            // 
            this.lblTPGTRXEndPin.AutoSize = true;
            this.lblTPGTRXEndPin.Location = new System.Drawing.Point(315, 101);
            this.lblTPGTRXEndPin.Name = "lblTPGTRXEndPin";
            this.lblTPGTRXEndPin.Size = new System.Drawing.Size(85, 17);
            this.lblTPGTRXEndPin.TabIndex = 40;
            this.lblTPGTRXEndPin.Text = "RX End Pin :";
            // 
            // tbxTPGTRXStartPin
            // 
            this.tbxTPGTRXStartPin.Location = new System.Drawing.Point(422, 67);
            this.tbxTPGTRXStartPin.Name = "tbxTPGTRXStartPin";
            this.tbxTPGTRXStartPin.Size = new System.Drawing.Size(81, 25);
            this.tbxTPGTRXStartPin.TabIndex = 39;
            this.tbxTPGTRXStartPin.Text = "0";
            // 
            // lblTPGTRXStartPin
            // 
            this.lblTPGTRXStartPin.AutoSize = true;
            this.lblTPGTRXStartPin.Location = new System.Drawing.Point(315, 70);
            this.lblTPGTRXStartPin.Name = "lblTPGTRXStartPin";
            this.lblTPGTRXStartPin.Size = new System.Drawing.Size(90, 17);
            this.lblTPGTRXStartPin.TabIndex = 38;
            this.lblTPGTRXStartPin.Text = "RX Start Pin :";
            // 
            // tbxTPGTTXEndPin
            // 
            this.tbxTPGTTXEndPin.Location = new System.Drawing.Point(422, 36);
            this.tbxTPGTTXEndPin.Name = "tbxTPGTTXEndPin";
            this.tbxTPGTTXEndPin.Size = new System.Drawing.Size(81, 25);
            this.tbxTPGTTXEndPin.TabIndex = 37;
            this.tbxTPGTTXEndPin.Text = "47";
            // 
            // lblTPGTTXEndPin
            // 
            this.lblTPGTTXEndPin.AutoSize = true;
            this.lblTPGTTXEndPin.Location = new System.Drawing.Point(315, 38);
            this.lblTPGTTXEndPin.Name = "lblTPGTTXEndPin";
            this.lblTPGTTXEndPin.Size = new System.Drawing.Size(84, 17);
            this.lblTPGTTXEndPin.TabIndex = 36;
            this.lblTPGTTXEndPin.Text = "TX End Pin :";
            // 
            // tbxTPGTTXStartPin
            // 
            this.tbxTPGTTXStartPin.Location = new System.Drawing.Point(422, 5);
            this.tbxTPGTTXStartPin.Name = "tbxTPGTTXStartPin";
            this.tbxTPGTTXStartPin.Size = new System.Drawing.Size(81, 25);
            this.tbxTPGTTXStartPin.TabIndex = 35;
            this.tbxTPGTTXStartPin.Text = "0";
            // 
            // lblTPGTTXStartPin
            // 
            this.lblTPGTTXStartPin.AutoSize = true;
            this.lblTPGTTXStartPin.Location = new System.Drawing.Point(315, 7);
            this.lblTPGTTXStartPin.Name = "lblTPGTTXStartPin";
            this.lblTPGTTXStartPin.Size = new System.Drawing.Size(89, 17);
            this.lblTPGTTXStartPin.TabIndex = 34;
            this.lblTPGTTXStartPin.Text = "TX Start Pin :";
            // 
            // tbxTPGTTXValidReportNumber
            // 
            this.tbxTPGTTXValidReportNumber.Location = new System.Drawing.Point(173, 36);
            this.tbxTPGTTXValidReportNumber.Name = "tbxTPGTTXValidReportNumber";
            this.tbxTPGTTXValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxTPGTTXValidReportNumber.TabIndex = 33;
            this.tbxTPGTTXValidReportNumber.Text = "1500";
            // 
            // lblTPGTTXValidReportNumber
            // 
            this.lblTPGTTXValidReportNumber.AutoSize = true;
            this.lblTPGTTXValidReportNumber.Location = new System.Drawing.Point(4, 38);
            this.lblTPGTTXValidReportNumber.Name = "lblTPGTTXValidReportNumber";
            this.lblTPGTTXValidReportNumber.Size = new System.Drawing.Size(163, 17);
            this.lblTPGTTXValidReportNumber.TabIndex = 32;
            this.lblTPGTTXValidReportNumber.Text = "TX Valid Report Number :";
            // 
            // tbxTPGTRXValidReportNumber
            // 
            this.tbxTPGTRXValidReportNumber.Location = new System.Drawing.Point(173, 5);
            this.tbxTPGTRXValidReportNumber.Name = "tbxTPGTRXValidReportNumber";
            this.tbxTPGTRXValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxTPGTRXValidReportNumber.TabIndex = 31;
            this.tbxTPGTRXValidReportNumber.Text = "1500";
            // 
            // lblTPGTRXValidReportNumber
            // 
            this.lblTPGTRXValidReportNumber.AutoSize = true;
            this.lblTPGTRXValidReportNumber.Location = new System.Drawing.Point(4, 7);
            this.lblTPGTRXValidReportNumber.Name = "lblTPGTRXValidReportNumber";
            this.lblTPGTRXValidReportNumber.Size = new System.Drawing.Size(164, 17);
            this.lblTPGTRXValidReportNumber.TabIndex = 30;
            this.lblTPGTRXValidReportNumber.Text = "RX Valid Report Number :";
            // 
            // tpgDTAndPCTStep
            // 
            this.tpgDTAndPCTStep.BackColor = System.Drawing.SystemColors.Control;
            this.tpgDTAndPCTStep.Controls.Add(this.tbxNormal800to400PwrRatio);
            this.tpgDTAndPCTStep.Controls.Add(this.lblNormal800to400PwrRatio);
            this.tpgDTAndPCTStep.Controls.Add(this.tbxTRxSTXValidReportNumber);
            this.tpgDTAndPCTStep.Controls.Add(this.lblTRxSTXValidReportNumber);
            this.tpgDTAndPCTStep.Controls.Add(this.tbxTRxSRXValidReportNumber);
            this.tpgDTAndPCTStep.Controls.Add(this.lblTRxSRXValidReportNumber);
            this.tpgDTAndPCTStep.Controls.Add(this.tbxNormalFilterTXValidReportNumber);
            this.tpgDTAndPCTStep.Controls.Add(this.lblNormalFilterTXValidReportNumber);
            this.tpgDTAndPCTStep.Controls.Add(this.tbxNormalFilterRXValidReportNumber);
            this.tpgDTAndPCTStep.Controls.Add(this.lblNormalFilterRXValidReportNumber);
            this.tpgDTAndPCTStep.Controls.Add(this.tbxNormalValidReportNumber);
            this.tpgDTAndPCTStep.Controls.Add(this.lblNormalValidReportNumber);
            this.tpgDTAndPCTStep.Location = new System.Drawing.Point(4, 26);
            this.tpgDTAndPCTStep.Name = "tpgDTAndPCTStep";
            this.tpgDTAndPCTStep.Size = new System.Drawing.Size(870, 436);
            this.tpgDTAndPCTStep.TabIndex = 2;
            this.tpgDTAndPCTStep.Text = "Digital & PeakCheck Tuning";
            // 
            // tbxNormal800to400PwrRatio
            // 
            this.tbxNormal800to400PwrRatio.Location = new System.Drawing.Point(563, 10);
            this.tbxNormal800to400PwrRatio.Name = "tbxNormal800to400PwrRatio";
            this.tbxNormal800to400PwrRatio.Size = new System.Drawing.Size(81, 25);
            this.tbxNormal800to400PwrRatio.TabIndex = 19;
            this.tbxNormal800to400PwrRatio.Text = "0.5";
            // 
            // lblNormal800to400PwrRatio
            // 
            this.lblNormal800to400PwrRatio.AutoSize = true;
            this.lblNormal800to400PwrRatio.Location = new System.Drawing.Point(385, 13);
            this.lblNormal800to400PwrRatio.Name = "lblNormal800to400PwrRatio";
            this.lblNormal800to400PwrRatio.Size = new System.Drawing.Size(172, 17);
            this.lblNormal800to400PwrRatio.TabIndex = 18;
            this.lblNormal800to400PwrRatio.Text = "Normal 800to400PwrRatio :";
            // 
            // tbxTRxSTXValidReportNumber
            // 
            this.tbxTRxSTXValidReportNumber.Location = new System.Drawing.Point(215, 121);
            this.tbxTRxSTXValidReportNumber.Name = "tbxTRxSTXValidReportNumber";
            this.tbxTRxSTXValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxTRxSTXValidReportNumber.TabIndex = 17;
            this.tbxTRxSTXValidReportNumber.Text = "70";
            // 
            // lblTRxSTXValidReportNumber
            // 
            this.lblTRxSTXValidReportNumber.AutoSize = true;
            this.lblTRxSTXValidReportNumber.Location = new System.Drawing.Point(4, 125);
            this.lblTRxSTXValidReportNumber.Name = "lblTRxSTXValidReportNumber";
            this.lblTRxSTXValidReportNumber.Size = new System.Drawing.Size(201, 17);
            this.lblTRxSTXValidReportNumber.TabIndex = 16;
            this.lblTRxSTXValidReportNumber.Text = "TRxS TX Valid Report Number :";
            // 
            // tbxTRxSRXValidReportNumber
            // 
            this.tbxTRxSRXValidReportNumber.Location = new System.Drawing.Point(215, 93);
            this.tbxTRxSRXValidReportNumber.Name = "tbxTRxSRXValidReportNumber";
            this.tbxTRxSRXValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxTRxSRXValidReportNumber.TabIndex = 15;
            this.tbxTRxSRXValidReportNumber.Text = "150";
            // 
            // lblTRxSRXValidReportNumber
            // 
            this.lblTRxSRXValidReportNumber.AutoSize = true;
            this.lblTRxSRXValidReportNumber.Location = new System.Drawing.Point(4, 97);
            this.lblTRxSRXValidReportNumber.Name = "lblTRxSRXValidReportNumber";
            this.lblTRxSRXValidReportNumber.Size = new System.Drawing.Size(202, 17);
            this.lblTRxSRXValidReportNumber.TabIndex = 14;
            this.lblTRxSRXValidReportNumber.Text = "TRxS RX Valid Report Number :";
            // 
            // tbxNormalFilterTXValidReportNumber
            // 
            this.tbxNormalFilterTXValidReportNumber.Location = new System.Drawing.Point(258, 65);
            this.tbxNormalFilterTXValidReportNumber.Name = "tbxNormalFilterTXValidReportNumber";
            this.tbxNormalFilterTXValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxNormalFilterTXValidReportNumber.TabIndex = 13;
            this.tbxNormalFilterTXValidReportNumber.Text = "150";
            // 
            // lblNormalFilterTXValidReportNumber
            // 
            this.lblNormalFilterTXValidReportNumber.AutoSize = true;
            this.lblNormalFilterTXValidReportNumber.Location = new System.Drawing.Point(4, 69);
            this.lblNormalFilterTXValidReportNumber.Name = "lblNormalFilterTXValidReportNumber";
            this.lblNormalFilterTXValidReportNumber.Size = new System.Drawing.Size(245, 17);
            this.lblNormalFilterTXValidReportNumber.TabIndex = 12;
            this.lblNormalFilterTXValidReportNumber.Text = "Normal Filter TX Valid Report Number :";
            // 
            // tbxNormalFilterRXValidReportNumber
            // 
            this.tbxNormalFilterRXValidReportNumber.Location = new System.Drawing.Point(258, 37);
            this.tbxNormalFilterRXValidReportNumber.Name = "tbxNormalFilterRXValidReportNumber";
            this.tbxNormalFilterRXValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxNormalFilterRXValidReportNumber.TabIndex = 11;
            this.tbxNormalFilterRXValidReportNumber.Text = "150";
            // 
            // lblNormalFilterRXValidReportNumber
            // 
            this.lblNormalFilterRXValidReportNumber.AutoSize = true;
            this.lblNormalFilterRXValidReportNumber.Location = new System.Drawing.Point(4, 41);
            this.lblNormalFilterRXValidReportNumber.Name = "lblNormalFilterRXValidReportNumber";
            this.lblNormalFilterRXValidReportNumber.Size = new System.Drawing.Size(246, 17);
            this.lblNormalFilterRXValidReportNumber.TabIndex = 10;
            this.lblNormalFilterRXValidReportNumber.Text = "Normal Filter RX Valid Report Number :";
            // 
            // tbxNormalValidReportNumber
            // 
            this.tbxNormalValidReportNumber.Location = new System.Drawing.Point(198, 10);
            this.tbxNormalValidReportNumber.Name = "tbxNormalValidReportNumber";
            this.tbxNormalValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxNormalValidReportNumber.TabIndex = 9;
            this.tbxNormalValidReportNumber.Text = "150";
            // 
            // lblNormalValidReportNumber
            // 
            this.lblNormalValidReportNumber.AutoSize = true;
            this.lblNormalValidReportNumber.Location = new System.Drawing.Point(4, 13);
            this.lblNormalValidReportNumber.Name = "lblNormalValidReportNumber";
            this.lblNormalValidReportNumber.Size = new System.Drawing.Size(188, 17);
            this.lblNormalValidReportNumber.TabIndex = 8;
            this.lblNormalValidReportNumber.Text = "Normal Valid Report Number :";
            // 
            // tpgPCTStep
            // 
            this.tpgPCTStep.BackColor = System.Drawing.SystemColors.Control;
            this.tpgPCTStep.Controls.Add(this.tbxPeakCheckRatio3T);
            this.tpgPCTStep.Controls.Add(this.lblPeakCheckRatio3T);
            this.tpgPCTStep.Controls.Add(this.tbxPeakCheckRatio5T);
            this.tpgPCTStep.Controls.Add(this.lblPeakCheckRatio5T);
            this.tpgPCTStep.Controls.Add(this.tbxPeakCheckRatio);
            this.tpgPCTStep.Controls.Add(this.lblPeakCheckRatio);
            this.tpgPCTStep.Location = new System.Drawing.Point(4, 26);
            this.tpgPCTStep.Name = "tpgPCTStep";
            this.tpgPCTStep.Size = new System.Drawing.Size(870, 436);
            this.tpgPCTStep.TabIndex = 6;
            this.tpgPCTStep.Text = "PeakCheck Tuning";
            // 
            // tbxPeakCheckRatio3T
            // 
            this.tbxPeakCheckRatio3T.Location = new System.Drawing.Point(143, 114);
            this.tbxPeakCheckRatio3T.Name = "tbxPeakCheckRatio3T";
            this.tbxPeakCheckRatio3T.Size = new System.Drawing.Size(52, 25);
            this.tbxPeakCheckRatio3T.TabIndex = 21;
            this.tbxPeakCheckRatio3T.Text = "0.4";
            // 
            // lblPeakCheckRatio3T
            // 
            this.lblPeakCheckRatio3T.AutoSize = true;
            this.lblPeakCheckRatio3T.Location = new System.Drawing.Point(4, 118);
            this.lblPeakCheckRatio3T.Name = "lblPeakCheckRatio3T";
            this.lblPeakCheckRatio3T.Size = new System.Drawing.Size(130, 17);
            this.lblPeakCheckRatio3T.TabIndex = 20;
            this.lblPeakCheckRatio3T.Text = "PeakCheckRatio3T :";
            // 
            // tbxPeakCheckRatio5T
            // 
            this.tbxPeakCheckRatio5T.Location = new System.Drawing.Point(143, 60);
            this.tbxPeakCheckRatio5T.Name = "tbxPeakCheckRatio5T";
            this.tbxPeakCheckRatio5T.Size = new System.Drawing.Size(52, 25);
            this.tbxPeakCheckRatio5T.TabIndex = 19;
            this.tbxPeakCheckRatio5T.Text = "0.9";
            // 
            // lblPeakCheckRatio5T
            // 
            this.lblPeakCheckRatio5T.AutoSize = true;
            this.lblPeakCheckRatio5T.Location = new System.Drawing.Point(4, 64);
            this.lblPeakCheckRatio5T.Name = "lblPeakCheckRatio5T";
            this.lblPeakCheckRatio5T.Size = new System.Drawing.Size(130, 17);
            this.lblPeakCheckRatio5T.TabIndex = 18;
            this.lblPeakCheckRatio5T.Text = "PeakCheckRatio5T :";
            // 
            // tbxPeakCheckRatio
            // 
            this.tbxPeakCheckRatio.Location = new System.Drawing.Point(144, 10);
            this.tbxPeakCheckRatio.Name = "tbxPeakCheckRatio";
            this.tbxPeakCheckRatio.Size = new System.Drawing.Size(52, 25);
            this.tbxPeakCheckRatio.TabIndex = 17;
            this.tbxPeakCheckRatio.Text = "1.1";
            // 
            // lblPeakCheckRatio
            // 
            this.lblPeakCheckRatio.AutoSize = true;
            this.lblPeakCheckRatio.Location = new System.Drawing.Point(4, 13);
            this.lblPeakCheckRatio.Name = "lblPeakCheckRatio";
            this.lblPeakCheckRatio.Size = new System.Drawing.Size(114, 17);
            this.lblPeakCheckRatio.TabIndex = 16;
            this.lblPeakCheckRatio.Text = "PeakCheckRatio :";
            // 
            // tpgDigitalTuningStep
            // 
            this.tpgDigitalTuningStep.BackColor = System.Drawing.SystemColors.Control;
            this.tpgDigitalTuningStep.Controls.Add(this.cbxDT7318TRxSSpecificReportType);
            this.tpgDigitalTuningStep.Controls.Add(this.lblDT7318TRxSSpecificReportType);
            this.tpgDigitalTuningStep.Controls.Add(this.cbxDTSkipCompareThreshold);
            this.tpgDigitalTuningStep.Controls.Add(this.lblDTSkipCompareThreshold);
            this.tpgDigitalTuningStep.Controls.Add(this.tbxDTThresholdRatio_TX);
            this.tpgDigitalTuningStep.Controls.Add(this.lblDTThresholdRatio_TX);
            this.tpgDigitalTuningStep.Controls.Add(this.tbxDTThresholdRatio_RX);
            this.tpgDigitalTuningStep.Controls.Add(this.lblDTThresholdRatio_RX);
            this.tpgDigitalTuningStep.Controls.Add(this.cbxDTDisplayChartDetailValue);
            this.tpgDigitalTuningStep.Controls.Add(this.lblDTDisplayChartDetailValue);
            this.tpgDigitalTuningStep.Controls.Add(this.tbxNormalContactTHRatio_TX);
            this.tpgDigitalTuningStep.Controls.Add(this.lblNormalContactTHRatio_TX);
            this.tpgDigitalTuningStep.Controls.Add(this.tbxNormalContactTHRatio_RX);
            this.tpgDigitalTuningStep.Controls.Add(this.lblNormalContactTHRatio_RX);
            this.tpgDigitalTuningStep.Controls.Add(this.tbxNormalHoverTHRatio_TX);
            this.tpgDigitalTuningStep.Controls.Add(this.lblNormalHoverTHRatio_TX);
            this.tpgDigitalTuningStep.Controls.Add(this.tbxNormalHoverTHRatio_RX);
            this.tpgDigitalTuningStep.Controls.Add(this.lblNormalHoverTHRatio_RX);
            this.tpgDigitalTuningStep.Location = new System.Drawing.Point(4, 26);
            this.tpgDigitalTuningStep.Name = "tpgDigitalTuningStep";
            this.tpgDigitalTuningStep.Size = new System.Drawing.Size(870, 436);
            this.tpgDigitalTuningStep.TabIndex = 3;
            this.tpgDigitalTuningStep.Text = "Digital Tuning";
            // 
            // cbxDT7318TRxSSpecificReportType
            // 
            this.cbxDT7318TRxSSpecificReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDT7318TRxSSpecificReportType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxDT7318TRxSSpecificReportType.FormattingEnabled = true;
            this.cbxDT7318TRxSSpecificReportType.Items.AddRange(new object[] {
            "Disable",
            "Enable"});
            this.cbxDT7318TRxSSpecificReportType.Location = new System.Drawing.Point(233, 155);
            this.cbxDT7318TRxSSpecificReportType.Name = "cbxDT7318TRxSSpecificReportType";
            this.cbxDT7318TRxSSpecificReportType.Size = new System.Drawing.Size(103, 25);
            this.cbxDT7318TRxSSpecificReportType.TabIndex = 27;
            // 
            // lblDT7318TRxSSpecificReportType
            // 
            this.lblDT7318TRxSSpecificReportType.AutoSize = true;
            this.lblDT7318TRxSSpecificReportType.Location = new System.Drawing.Point(5, 160);
            this.lblDT7318TRxSSpecificReportType.Name = "lblDT7318TRxSSpecificReportType";
            this.lblDT7318TRxSSpecificReportType.Size = new System.Drawing.Size(209, 17);
            this.lblDT7318TRxSSpecificReportType.TabIndex = 26;
            this.lblDT7318TRxSSpecificReportType.Text = "7318 TRxS Specific Report Type :";
            // 
            // cbxDTSkipCompareThreshold
            // 
            this.cbxDTSkipCompareThreshold.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDTSkipCompareThreshold.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxDTSkipCompareThreshold.FormattingEnabled = true;
            this.cbxDTSkipCompareThreshold.Items.AddRange(new object[] {
            "Disable",
            "Enable"});
            this.cbxDTSkipCompareThreshold.Location = new System.Drawing.Point(502, 118);
            this.cbxDTSkipCompareThreshold.Name = "cbxDTSkipCompareThreshold";
            this.cbxDTSkipCompareThreshold.Size = new System.Drawing.Size(103, 25);
            this.cbxDTSkipCompareThreshold.TabIndex = 25;
            // 
            // lblDTSkipCompareThreshold
            // 
            this.lblDTSkipCompareThreshold.AutoSize = true;
            this.lblDTSkipCompareThreshold.Location = new System.Drawing.Point(320, 123);
            this.lblDTSkipCompareThreshold.Name = "lblDTSkipCompareThreshold";
            this.lblDTSkipCompareThreshold.Size = new System.Drawing.Size(160, 17);
            this.lblDTSkipCompareThreshold.TabIndex = 24;
            this.lblDTSkipCompareThreshold.Text = "Skip Compare Threshold :";
            // 
            // tbxDTThresholdRatio_TX
            // 
            this.tbxDTThresholdRatio_TX.Location = new System.Drawing.Point(473, 47);
            this.tbxDTThresholdRatio_TX.Name = "tbxDTThresholdRatio_TX";
            this.tbxDTThresholdRatio_TX.Size = new System.Drawing.Size(52, 25);
            this.tbxDTThresholdRatio_TX.TabIndex = 23;
            this.tbxDTThresholdRatio_TX.Text = "1.0";
            // 
            // lblDTThresholdRatio_TX
            // 
            this.lblDTThresholdRatio_TX.AutoSize = true;
            this.lblDTThresholdRatio_TX.Location = new System.Drawing.Point(320, 50);
            this.lblDTThresholdRatio_TX.Name = "lblDTThresholdRatio_TX";
            this.lblDTThresholdRatio_TX.Size = new System.Drawing.Size(135, 17);
            this.lblDTThresholdRatio_TX.TabIndex = 22;
            this.lblDTThresholdRatio_TX.Text = "Threshold Ratio_TX :";
            // 
            // tbxDTThresholdRatio_RX
            // 
            this.tbxDTThresholdRatio_RX.Location = new System.Drawing.Point(473, 10);
            this.tbxDTThresholdRatio_RX.Name = "tbxDTThresholdRatio_RX";
            this.tbxDTThresholdRatio_RX.Size = new System.Drawing.Size(52, 25);
            this.tbxDTThresholdRatio_RX.TabIndex = 21;
            this.tbxDTThresholdRatio_RX.Text = "1.0";
            // 
            // lblDTThresholdRatio_RX
            // 
            this.lblDTThresholdRatio_RX.AutoSize = true;
            this.lblDTThresholdRatio_RX.Location = new System.Drawing.Point(320, 13);
            this.lblDTThresholdRatio_RX.Name = "lblDTThresholdRatio_RX";
            this.lblDTThresholdRatio_RX.Size = new System.Drawing.Size(136, 17);
            this.lblDTThresholdRatio_RX.TabIndex = 20;
            this.lblDTThresholdRatio_RX.Text = "Threshold Ratio_RX :";
            // 
            // cbxDTDisplayChartDetailValue
            // 
            this.cbxDTDisplayChartDetailValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDTDisplayChartDetailValue.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxDTDisplayChartDetailValue.FormattingEnabled = true;
            this.cbxDTDisplayChartDetailValue.Items.AddRange(new object[] {
            "Disable",
            "Enable"});
            this.cbxDTDisplayChartDetailValue.Location = new System.Drawing.Point(502, 81);
            this.cbxDTDisplayChartDetailValue.Name = "cbxDTDisplayChartDetailValue";
            this.cbxDTDisplayChartDetailValue.Size = new System.Drawing.Size(103, 25);
            this.cbxDTDisplayChartDetailValue.TabIndex = 19;
            // 
            // lblDTDisplayChartDetailValue
            // 
            this.lblDTDisplayChartDetailValue.AutoSize = true;
            this.lblDTDisplayChartDetailValue.Location = new System.Drawing.Point(320, 86);
            this.lblDTDisplayChartDetailValue.Name = "lblDTDisplayChartDetailValue";
            this.lblDTDisplayChartDetailValue.Size = new System.Drawing.Size(172, 17);
            this.lblDTDisplayChartDetailValue.TabIndex = 18;
            this.lblDTDisplayChartDetailValue.Text = "Display Chart Detail Value :";
            // 
            // tbxNormalContactTHRatio_TX
            // 
            this.tbxNormalContactTHRatio_TX.Location = new System.Drawing.Point(209, 119);
            this.tbxNormalContactTHRatio_TX.Name = "tbxNormalContactTHRatio_TX";
            this.tbxNormalContactTHRatio_TX.Size = new System.Drawing.Size(52, 25);
            this.tbxNormalContactTHRatio_TX.TabIndex = 17;
            this.tbxNormalContactTHRatio_TX.Text = "0.45";
            // 
            // lblNormalContactTHRatio_TX
            // 
            this.lblNormalContactTHRatio_TX.AutoSize = true;
            this.lblNormalContactTHRatio_TX.Location = new System.Drawing.Point(4, 123);
            this.lblNormalContactTHRatio_TX.Name = "lblNormalContactTHRatio_TX";
            this.lblNormalContactTHRatio_TX.Size = new System.Drawing.Size(199, 17);
            this.lblNormalContactTHRatio_TX.TabIndex = 16;
            this.lblNormalContactTHRatio_TX.Text = "Normal Contact_TH Ratio_TX :";
            // 
            // tbxNormalContactTHRatio_RX
            // 
            this.tbxNormalContactTHRatio_RX.Location = new System.Drawing.Point(209, 82);
            this.tbxNormalContactTHRatio_RX.Name = "tbxNormalContactTHRatio_RX";
            this.tbxNormalContactTHRatio_RX.Size = new System.Drawing.Size(52, 25);
            this.tbxNormalContactTHRatio_RX.TabIndex = 15;
            this.tbxNormalContactTHRatio_RX.Text = "0.45";
            // 
            // lblNormalContactTHRatio_RX
            // 
            this.lblNormalContactTHRatio_RX.AutoSize = true;
            this.lblNormalContactTHRatio_RX.Location = new System.Drawing.Point(4, 86);
            this.lblNormalContactTHRatio_RX.Name = "lblNormalContactTHRatio_RX";
            this.lblNormalContactTHRatio_RX.Size = new System.Drawing.Size(200, 17);
            this.lblNormalContactTHRatio_RX.TabIndex = 14;
            this.lblNormalContactTHRatio_RX.Text = "Normal Contact_TH Ratio_RX :";
            // 
            // tbxNormalHoverTHRatio_TX
            // 
            this.tbxNormalHoverTHRatio_TX.Location = new System.Drawing.Point(209, 46);
            this.tbxNormalHoverTHRatio_TX.Name = "tbxNormalHoverTHRatio_TX";
            this.tbxNormalHoverTHRatio_TX.Size = new System.Drawing.Size(52, 25);
            this.tbxNormalHoverTHRatio_TX.TabIndex = 13;
            this.tbxNormalHoverTHRatio_TX.Text = "0.45";
            // 
            // lblNormalHoverTHRatio_TX
            // 
            this.lblNormalHoverTHRatio_TX.AutoSize = true;
            this.lblNormalHoverTHRatio_TX.Location = new System.Drawing.Point(4, 50);
            this.lblNormalHoverTHRatio_TX.Name = "lblNormalHoverTHRatio_TX";
            this.lblNormalHoverTHRatio_TX.Size = new System.Drawing.Size(190, 17);
            this.lblNormalHoverTHRatio_TX.TabIndex = 12;
            this.lblNormalHoverTHRatio_TX.Text = "Normal Hover_TH Ratio_TX :";
            // 
            // tbxNormalHoverTHRatio_RX
            // 
            this.tbxNormalHoverTHRatio_RX.Location = new System.Drawing.Point(210, 10);
            this.tbxNormalHoverTHRatio_RX.Name = "tbxNormalHoverTHRatio_RX";
            this.tbxNormalHoverTHRatio_RX.Size = new System.Drawing.Size(52, 25);
            this.tbxNormalHoverTHRatio_RX.TabIndex = 11;
            this.tbxNormalHoverTHRatio_RX.Text = "0.45";
            // 
            // lblNormalHoverTHRatio_RX
            // 
            this.lblNormalHoverTHRatio_RX.AutoSize = true;
            this.lblNormalHoverTHRatio_RX.Location = new System.Drawing.Point(4, 13);
            this.lblNormalHoverTHRatio_RX.Name = "lblNormalHoverTHRatio_RX";
            this.lblNormalHoverTHRatio_RX.Size = new System.Drawing.Size(191, 17);
            this.lblNormalHoverTHRatio_RX.TabIndex = 10;
            this.lblNormalHoverTHRatio_RX.Text = "Normal Hover_TH Ratio_RX :";
            // 
            // tpgTiltTuningStep
            // 
            this.tpgTiltTuningStep.BackColor = System.Drawing.SystemColors.Control;
            this.tpgTiltTuningStep.Controls.Add(this.tbxTTTXValidReportNumber);
            this.tpgTiltTuningStep.Controls.Add(this.lblTTTXValidReportNumber);
            this.tpgTiltTuningStep.Controls.Add(this.tbxTTRXValidReportNumber);
            this.tpgTiltTuningStep.Controls.Add(this.lblTTRXValidReportNumber);
            this.tpgTiltTuningStep.Controls.Add(this.tbxTTValidTipTraceNumber);
            this.tpgTiltTuningStep.Controls.Add(this.lblTTValidTipTraceNumber);
            this.tpgTiltTuningStep.Location = new System.Drawing.Point(4, 26);
            this.tpgTiltTuningStep.Name = "tpgTiltTuningStep";
            this.tpgTiltTuningStep.Size = new System.Drawing.Size(870, 436);
            this.tpgTiltTuningStep.TabIndex = 4;
            this.tpgTiltTuningStep.Text = "Tilt Tuning";
            // 
            // tbxTTTXValidReportNumber
            // 
            this.tbxTTTXValidReportNumber.Location = new System.Drawing.Point(176, 114);
            this.tbxTTTXValidReportNumber.Name = "tbxTTTXValidReportNumber";
            this.tbxTTTXValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxTTTXValidReportNumber.TabIndex = 7;
            this.tbxTTTXValidReportNumber.Text = "70";
            // 
            // lblTTTXValidReportNumber
            // 
            this.lblTTTXValidReportNumber.AutoSize = true;
            this.lblTTTXValidReportNumber.Location = new System.Drawing.Point(4, 118);
            this.lblTTTXValidReportNumber.Name = "lblTTTXValidReportNumber";
            this.lblTTTXValidReportNumber.Size = new System.Drawing.Size(163, 17);
            this.lblTTTXValidReportNumber.TabIndex = 6;
            this.lblTTTXValidReportNumber.Text = "TX Valid Report Number :";
            // 
            // tbxTTRXValidReportNumber
            // 
            this.tbxTTRXValidReportNumber.Location = new System.Drawing.Point(176, 60);
            this.tbxTTRXValidReportNumber.Name = "tbxTTRXValidReportNumber";
            this.tbxTTRXValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxTTRXValidReportNumber.TabIndex = 5;
            this.tbxTTRXValidReportNumber.Text = "150";
            // 
            // lblTTRXValidReportNumber
            // 
            this.lblTTRXValidReportNumber.AutoSize = true;
            this.lblTTRXValidReportNumber.Location = new System.Drawing.Point(4, 64);
            this.lblTTRXValidReportNumber.Name = "lblTTRXValidReportNumber";
            this.lblTTRXValidReportNumber.Size = new System.Drawing.Size(164, 17);
            this.lblTTRXValidReportNumber.TabIndex = 4;
            this.lblTTRXValidReportNumber.Text = "RX Valid Report Number :";
            // 
            // tbxTTValidTipTraceNumber
            // 
            this.tbxTTValidTipTraceNumber.Location = new System.Drawing.Point(176, 9);
            this.tbxTTValidTipTraceNumber.Name = "tbxTTValidTipTraceNumber";
            this.tbxTTValidTipTraceNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxTTValidTipTraceNumber.TabIndex = 3;
            this.tbxTTValidTipTraceNumber.Text = "5";
            // 
            // lblTTValidTipTraceNumber
            // 
            this.lblTTValidTipTraceNumber.AutoSize = true;
            this.lblTTValidTipTraceNumber.Location = new System.Drawing.Point(4, 13);
            this.lblTTValidTipTraceNumber.Name = "lblTTValidTipTraceNumber";
            this.lblTTValidTipTraceNumber.Size = new System.Drawing.Size(156, 17);
            this.lblTTValidTipTraceNumber.TabIndex = 2;
            this.lblTTValidTipTraceNumber.Text = "Valid Tip Trace Number :";
            // 
            // tpgPTStep
            // 
            this.tpgPTStep.BackColor = System.Drawing.SystemColors.Control;
            this.tpgPTStep.Controls.Add(this.cbxPTPenVersion);
            this.tpgPTStep.Controls.Add(this.lblPTPenVersion);
            this.tpgPTStep.Controls.Add(this.tbxPTRecordTime);
            this.tpgPTStep.Controls.Add(this.lblPTRecordTime);
            this.tpgPTStep.Controls.Add(this.tbxPTEndSkipReportNumber);
            this.tpgPTStep.Controls.Add(this.lblPTEndSkipReportNumber);
            this.tpgPTStep.Controls.Add(this.tbxPTStartSkipReportNumber);
            this.tpgPTStep.Controls.Add(this.lblPTStartSkipReportNumber);
            this.tpgPTStep.Controls.Add(this.tbxPTValidReportNumber);
            this.tpgPTStep.Controls.Add(this.lblPTValidReportNumber);
            this.tpgPTStep.Location = new System.Drawing.Point(4, 26);
            this.tpgPTStep.Name = "tpgPTStep";
            this.tpgPTStep.Size = new System.Drawing.Size(870, 436);
            this.tpgPTStep.TabIndex = 5;
            this.tpgPTStep.Text = "Pressure Tuning";
            // 
            // cbxPTPenVersion
            // 
            this.cbxPTPenVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPTPenVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxPTPenVersion.FormattingEnabled = true;
            this.cbxPTPenVersion.Items.AddRange(new object[] {
            "1.51",
            "2.0"});
            this.cbxPTPenVersion.Location = new System.Drawing.Point(425, 9);
            this.cbxPTPenVersion.Name = "cbxPTPenVersion";
            this.cbxPTPenVersion.Size = new System.Drawing.Size(103, 25);
            this.cbxPTPenVersion.TabIndex = 21;
            // 
            // lblPTPenVersion
            // 
            this.lblPTPenVersion.AutoSize = true;
            this.lblPTPenVersion.Location = new System.Drawing.Point(332, 14);
            this.lblPTPenVersion.Name = "lblPTPenVersion";
            this.lblPTPenVersion.Size = new System.Drawing.Size(86, 17);
            this.lblPTPenVersion.TabIndex = 20;
            this.lblPTPenVersion.Text = "Pen Version :";
            // 
            // tbxPTRecordTime
            // 
            this.tbxPTRecordTime.Location = new System.Drawing.Point(183, 11);
            this.tbxPTRecordTime.Name = "tbxPTRecordTime";
            this.tbxPTRecordTime.Size = new System.Drawing.Size(81, 25);
            this.tbxPTRecordTime.TabIndex = 19;
            this.tbxPTRecordTime.Text = "10";
            // 
            // lblPTRecordTime
            // 
            this.lblPTRecordTime.AutoSize = true;
            this.lblPTRecordTime.Location = new System.Drawing.Point(4, 14);
            this.lblPTRecordTime.Name = "lblPTRecordTime";
            this.lblPTRecordTime.Size = new System.Drawing.Size(107, 17);
            this.lblPTRecordTime.TabIndex = 18;
            this.lblPTRecordTime.Text = "Record Time(s) :";
            // 
            // tbxPTEndSkipReportNumber
            // 
            this.tbxPTEndSkipReportNumber.Location = new System.Drawing.Point(183, 121);
            this.tbxPTEndSkipReportNumber.Name = "tbxPTEndSkipReportNumber";
            this.tbxPTEndSkipReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxPTEndSkipReportNumber.TabIndex = 13;
            this.tbxPTEndSkipReportNumber.Text = "150";
            // 
            // lblPTEndSkipReportNumber
            // 
            this.lblPTEndSkipReportNumber.AutoSize = true;
            this.lblPTEndSkipReportNumber.Location = new System.Drawing.Point(4, 125);
            this.lblPTEndSkipReportNumber.Name = "lblPTEndSkipReportNumber";
            this.lblPTEndSkipReportNumber.Size = new System.Drawing.Size(163, 17);
            this.lblPTEndSkipReportNumber.TabIndex = 12;
            this.lblPTEndSkipReportNumber.Text = "End Skip Report Number :";
            // 
            // tbxPTStartSkipReportNumber
            // 
            this.tbxPTStartSkipReportNumber.Location = new System.Drawing.Point(183, 83);
            this.tbxPTStartSkipReportNumber.Name = "tbxPTStartSkipReportNumber";
            this.tbxPTStartSkipReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxPTStartSkipReportNumber.TabIndex = 11;
            this.tbxPTStartSkipReportNumber.Text = "150";
            // 
            // lblPTStartSkipReportNumber
            // 
            this.lblPTStartSkipReportNumber.AutoSize = true;
            this.lblPTStartSkipReportNumber.Location = new System.Drawing.Point(4, 87);
            this.lblPTStartSkipReportNumber.Name = "lblPTStartSkipReportNumber";
            this.lblPTStartSkipReportNumber.Size = new System.Drawing.Size(168, 17);
            this.lblPTStartSkipReportNumber.TabIndex = 10;
            this.lblPTStartSkipReportNumber.Text = "Start Skip Report Number :";
            // 
            // tbxPTValidReportNumber
            // 
            this.tbxPTValidReportNumber.Location = new System.Drawing.Point(183, 47);
            this.tbxPTValidReportNumber.Name = "tbxPTValidReportNumber";
            this.tbxPTValidReportNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxPTValidReportNumber.TabIndex = 9;
            this.tbxPTValidReportNumber.Text = "1";
            // 
            // lblPTValidReportNumber
            // 
            this.lblPTValidReportNumber.AutoSize = true;
            this.lblPTValidReportNumber.Location = new System.Drawing.Point(4, 51);
            this.lblPTValidReportNumber.Name = "lblPTValidReportNumber";
            this.lblPTValidReportNumber.Size = new System.Drawing.Size(140, 17);
            this.lblPTValidReportNumber.TabIndex = 8;
            this.lblPTValidReportNumber.Text = "Valid Report Number :";
            // 
            // tpgLTStep
            // 
            this.tpgLTStep.BackColor = System.Drawing.SystemColors.Control;
            this.tpgLTStep.Controls.Add(this.cbxLTTP_GainCompensate);
            this.tpgLTStep.Controls.Add(this.lblLTTP_GainCompensate);
            this.tpgLTStep.Location = new System.Drawing.Point(4, 26);
            this.tpgLTStep.Margin = new System.Windows.Forms.Padding(2);
            this.tpgLTStep.Name = "tpgLTStep";
            this.tpgLTStep.Size = new System.Drawing.Size(870, 436);
            this.tpgLTStep.TabIndex = 10;
            this.tpgLTStep.Text = "Linearity Tuning";
            // 
            // cbxLTTP_GainCompensate
            // 
            this.cbxLTTP_GainCompensate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLTTP_GainCompensate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxLTTP_GainCompensate.FormattingEnabled = true;
            this.cbxLTTP_GainCompensate.Items.AddRange(new object[] {
            "Disable",
            "Tip Gain",
            "Ring Gain"});
            this.cbxLTTP_GainCompensate.Location = new System.Drawing.Point(165, 11);
            this.cbxLTTP_GainCompensate.Name = "cbxLTTP_GainCompensate";
            this.cbxLTTP_GainCompensate.Size = new System.Drawing.Size(155, 25);
            this.cbxLTTP_GainCompensate.TabIndex = 21;
            // 
            // lblLTTP_GainCompensate
            // 
            this.lblLTTP_GainCompensate.AutoSize = true;
            this.lblLTTP_GainCompensate.Location = new System.Drawing.Point(4, 14);
            this.lblLTTP_GainCompensate.Name = "lblLTTP_GainCompensate";
            this.lblLTTP_GainCompensate.Size = new System.Drawing.Size(145, 17);
            this.lblLTTP_GainCompensate.TabIndex = 20;
            this.lblLTTP_GainCompensate.Text = "TP_Gain Compensate :";
            // 
            // tpgFWParameterSetting
            // 
            this.tpgFWParameterSetting.BackColor = System.Drawing.SystemColors.Control;
            this.tpgFWParameterSetting.Controls.Add(this.tbxParameter_cActivePen_FM_P0_TH);
            this.tpgFWParameterSetting.Controls.Add(this.lblParameter_cAcitvePen_FM_P0_TH);
            this.tpgFWParameterSetting.Location = new System.Drawing.Point(4, 26);
            this.tpgFWParameterSetting.Margin = new System.Windows.Forms.Padding(2);
            this.tpgFWParameterSetting.Name = "tpgFWParameterSetting";
            this.tpgFWParameterSetting.Size = new System.Drawing.Size(870, 436);
            this.tpgFWParameterSetting.TabIndex = 8;
            this.tpgFWParameterSetting.Text = "FW Parameter Setting";
            // 
            // tbxParameter_cActivePen_FM_P0_TH
            // 
            this.tbxParameter_cActivePen_FM_P0_TH.Location = new System.Drawing.Point(173, 13);
            this.tbxParameter_cActivePen_FM_P0_TH.Name = "tbxParameter_cActivePen_FM_P0_TH";
            this.tbxParameter_cActivePen_FM_P0_TH.Size = new System.Drawing.Size(81, 25);
            this.tbxParameter_cActivePen_FM_P0_TH.TabIndex = 4;
            this.tbxParameter_cActivePen_FM_P0_TH.Text = "-1";
            // 
            // lblParameter_cAcitvePen_FM_P0_TH
            // 
            this.lblParameter_cAcitvePen_FM_P0_TH.AutoSize = true;
            this.lblParameter_cAcitvePen_FM_P0_TH.Location = new System.Drawing.Point(4, 15);
            this.lblParameter_cAcitvePen_FM_P0_TH.Name = "lblParameter_cAcitvePen_FM_P0_TH";
            this.lblParameter_cAcitvePen_FM_P0_TH.Size = new System.Drawing.Size(165, 17);
            this.lblParameter_cAcitvePen_FM_P0_TH.TabIndex = 3;
            this.lblParameter_cAcitvePen_FM_P0_TH.Text = "cActivePen_FM_P0_TH :";
            // 
            // tpgOther
            // 
            this.tpgOther.AutoScroll = true;
            this.tpgOther.BackColor = System.Drawing.SystemColors.Control;
            this.tpgOther.Controls.Add(this.cbx5TRawDataType);
            this.tpgOther.Controls.Add(this.lbl5TRawDataType);
            this.tpgOther.Controls.Add(this.cbxSetDigiGain);
            this.tpgOther.Controls.Add(this.lblSetDigiGain);
            this.tpgOther.Controls.Add(this.tbxOtherFreqNumber);
            this.tpgOther.Controls.Add(this.lblOtherFreqNumber);
            this.tpgOther.Controls.Add(this.tbxTNFreqNumber);
            this.tpgOther.Controls.Add(this.lblTNFreqNumber);
            this.tpgOther.Controls.Add(this.cbxAutoTune_P0_detect_time);
            this.tpgOther.Controls.Add(this.lblAutoTune_P0_detect_time);
            this.tpgOther.Controls.Add(this.tbxDrawLineTimeout);
            this.tpgOther.Controls.Add(this.lblDrawLineTimeout);
            this.tpgOther.Controls.Add(this.tbxRecordDataRetryCount);
            this.tpgOther.Controls.Add(this.lblRecordDataRetryCount);
            this.tpgOther.Controls.Add(this.tbxStartDelayTime);
            this.tpgOther.Controls.Add(this.lblStartDelayTime);
            this.tpgOther.Location = new System.Drawing.Point(4, 26);
            this.tpgOther.Name = "tpgOther";
            this.tpgOther.Padding = new System.Windows.Forms.Padding(3);
            this.tpgOther.Size = new System.Drawing.Size(870, 436);
            this.tpgOther.TabIndex = 1;
            this.tpgOther.Text = "Other";
            // 
            // cbx5TRawDataType
            // 
            this.cbx5TRawDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx5TRawDataType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbx5TRawDataType.FormattingEnabled = true;
            this.cbx5TRawDataType.Items.AddRange(new object[] {
            "Old Format",
            "New Format"});
            this.cbx5TRawDataType.Location = new System.Drawing.Point(496, 118);
            this.cbx5TRawDataType.Name = "cbx5TRawDataType";
            this.cbx5TRawDataType.Size = new System.Drawing.Size(136, 25);
            this.cbx5TRawDataType.TabIndex = 25;
            // 
            // lbl5TRawDataType
            // 
            this.lbl5TRawDataType.AutoSize = true;
            this.lbl5TRawDataType.Location = new System.Drawing.Point(356, 123);
            this.lbl5TRawDataType.Name = "lbl5TRawDataType";
            this.lbl5TRawDataType.Size = new System.Drawing.Size(129, 17);
            this.lbl5TRawDataType.TabIndex = 24;
            this.lbl5TRawDataType.Text = "5T Raw Data Type :";
            // 
            // cbxSetDigiGain
            // 
            this.cbxSetDigiGain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSetDigiGain.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxSetDigiGain.FormattingEnabled = true;
            this.cbxSetDigiGain.Items.AddRange(new object[] {
            "Disable",
            "Only Noise&TiltNoise Step",
            "All Step"});
            this.cbxSetDigiGain.Location = new System.Drawing.Point(447, 79);
            this.cbxSetDigiGain.Name = "cbxSetDigiGain";
            this.cbxSetDigiGain.Size = new System.Drawing.Size(186, 25);
            this.cbxSetDigiGain.TabIndex = 23;
            // 
            // lblSetDigiGain
            // 
            this.lblSetDigiGain.AutoSize = true;
            this.lblSetDigiGain.Location = new System.Drawing.Point(356, 84);
            this.lblSetDigiGain.Name = "lblSetDigiGain";
            this.lblSetDigiGain.Size = new System.Drawing.Size(89, 17);
            this.lblSetDigiGain.TabIndex = 22;
            this.lblSetDigiGain.Text = "Set DigiGain :";
            // 
            // tbxOtherFreqNumber
            // 
            this.tbxOtherFreqNumber.Location = new System.Drawing.Point(551, 45);
            this.tbxOtherFreqNumber.Name = "tbxOtherFreqNumber";
            this.tbxOtherFreqNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxOtherFreqNumber.TabIndex = 21;
            this.tbxOtherFreqNumber.Text = "5";
            // 
            // lblOtherFreqNumber
            // 
            this.lblOtherFreqNumber.AutoSize = true;
            this.lblOtherFreqNumber.Location = new System.Drawing.Point(356, 49);
            this.lblOtherFreqNumber.Name = "lblOtherFreqNumber";
            this.lblOtherFreqNumber.Size = new System.Drawing.Size(167, 17);
            this.lblOtherFreqNumber.TabIndex = 20;
            this.lblOtherFreqNumber.Text = "Other Frequency Number :";
            // 
            // tbxTNFreqNumber
            // 
            this.tbxTNFreqNumber.Location = new System.Drawing.Point(551, 11);
            this.tbxTNFreqNumber.Name = "tbxTNFreqNumber";
            this.tbxTNFreqNumber.Size = new System.Drawing.Size(81, 25);
            this.tbxTNFreqNumber.TabIndex = 19;
            this.tbxTNFreqNumber.Text = "-1";
            // 
            // lblTNFreqNumber
            // 
            this.lblTNFreqNumber.AutoSize = true;
            this.lblTNFreqNumber.Location = new System.Drawing.Point(356, 15);
            this.lblTNFreqNumber.Name = "lblTNFreqNumber";
            this.lblTNFreqNumber.Size = new System.Drawing.Size(189, 17);
            this.lblTNFreqNumber.TabIndex = 18;
            this.lblTNFreqNumber.Text = "Tilt Noise Frequency Number :";
            // 
            // cbxAutoTune_P0_detect_time
            // 
            this.cbxAutoTune_P0_detect_time.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAutoTune_P0_detect_time.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbxAutoTune_P0_detect_time.FormattingEnabled = true;
            this.cbxAutoTune_P0_detect_time.Items.AddRange(new object[] {
            "400us",
            "800us"});
            this.cbxAutoTune_P0_detect_time.Location = new System.Drawing.Point(186, 10);
            this.cbxAutoTune_P0_detect_time.Name = "cbxAutoTune_P0_detect_time";
            this.cbxAutoTune_P0_detect_time.Size = new System.Drawing.Size(103, 25);
            this.cbxAutoTune_P0_detect_time.TabIndex = 17;
            // 
            // lblAutoTune_P0_detect_time
            // 
            this.lblAutoTune_P0_detect_time.AutoSize = true;
            this.lblAutoTune_P0_detect_time.Location = new System.Drawing.Point(4, 15);
            this.lblAutoTune_P0_detect_time.Name = "lblAutoTune_P0_detect_time";
            this.lblAutoTune_P0_detect_time.Size = new System.Drawing.Size(174, 17);
            this.lblAutoTune_P0_detect_time.TabIndex = 16;
            this.lblAutoTune_P0_detect_time.Text = "AutoTune_P0_detect_time :";
            // 
            // tbxDrawLineTimeout
            // 
            this.tbxDrawLineTimeout.Location = new System.Drawing.Point(175, 117);
            this.tbxDrawLineTimeout.Name = "tbxDrawLineTimeout";
            this.tbxDrawLineTimeout.Size = new System.Drawing.Size(81, 25);
            this.tbxDrawLineTimeout.TabIndex = 9;
            this.tbxDrawLineTimeout.Text = "600";
            // 
            // lblDrawLineTimeout
            // 
            this.lblDrawLineTimeout.AutoSize = true;
            this.lblDrawLineTimeout.Location = new System.Drawing.Point(4, 121);
            this.lblDrawLineTimeout.Name = "lblDrawLineTimeout";
            this.lblDrawLineTimeout.Size = new System.Drawing.Size(164, 17);
            this.lblDrawLineTimeout.TabIndex = 8;
            this.lblDrawLineTimeout.Text = "Draw Line Timeout (sec) :";
            // 
            // tbxRecordDataRetryCount
            // 
            this.tbxRecordDataRetryCount.Location = new System.Drawing.Point(175, 81);
            this.tbxRecordDataRetryCount.Name = "tbxRecordDataRetryCount";
            this.tbxRecordDataRetryCount.Size = new System.Drawing.Size(81, 25);
            this.tbxRecordDataRetryCount.TabIndex = 7;
            this.tbxRecordDataRetryCount.Text = "3";
            // 
            // lblRecordDataRetryCount
            // 
            this.lblRecordDataRetryCount.AutoSize = true;
            this.lblRecordDataRetryCount.Location = new System.Drawing.Point(4, 84);
            this.lblRecordDataRetryCount.Name = "lblRecordDataRetryCount";
            this.lblRecordDataRetryCount.Size = new System.Drawing.Size(171, 17);
            this.lblRecordDataRetryCount.TabIndex = 6;
            this.lblRecordDataRetryCount.Text = "Record Data Retry Count : ";
            // 
            // tbxStartDelayTime
            // 
            this.tbxStartDelayTime.Location = new System.Drawing.Point(175, 45);
            this.tbxStartDelayTime.Name = "tbxStartDelayTime";
            this.tbxStartDelayTime.Size = new System.Drawing.Size(81, 25);
            this.tbxStartDelayTime.TabIndex = 2;
            this.tbxStartDelayTime.Text = "3";
            // 
            // lblStartDelayTime
            // 
            this.lblStartDelayTime.AutoSize = true;
            this.lblStartDelayTime.Location = new System.Drawing.Point(4, 49);
            this.lblStartDelayTime.Name = "lblStartDelayTime";
            this.lblStartDelayTime.Size = new System.Drawing.Size(149, 17);
            this.lblStartDelayTime.TabIndex = 1;
            this.lblStartDelayTime.Text = "Start Delay Time (sec) :";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(772, 498);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 36);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // gbxNoiseMaxValueOverWarningHB
            // 
            this.gbxNoiseMaxValueOverWarningHB.Controls.Add(this.lblNoiseMaxMinusMeanValueOverWarningStdevMagHB);
            this.gbxNoiseMaxValueOverWarningHB.Controls.Add(this.tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB);
            this.gbxNoiseMaxValueOverWarningHB.Controls.Add(this.lblNoiseMaxValueOverWarningAbsValueHB);
            this.gbxNoiseMaxValueOverWarningHB.Controls.Add(this.tbxNoiseMaxValueOverWarningAbsValueHB);
            this.gbxNoiseMaxValueOverWarningHB.Location = new System.Drawing.Point(7, 223);
            this.gbxNoiseMaxValueOverWarningHB.Name = "gbxNoiseMaxValueOverWarningHB";
            this.gbxNoiseMaxValueOverWarningHB.Size = new System.Drawing.Size(461, 99);
            this.gbxNoiseMaxValueOverWarningHB.TabIndex = 25;
            this.gbxNoiseMaxValueOverWarningHB.TabStop = false;
            this.gbxNoiseMaxValueOverWarningHB.Text = "Noise Max Value Over Warning HB";
            // 
            // lblNoiseMaxMinusMeanValueOverWarningStdevMagHB
            // 
            this.lblNoiseMaxMinusMeanValueOverWarningStdevMagHB.AutoSize = true;
            this.lblNoiseMaxMinusMeanValueOverWarningStdevMagHB.Location = new System.Drawing.Point(6, 34);
            this.lblNoiseMaxMinusMeanValueOverWarningStdevMagHB.Name = "lblNoiseMaxMinusMeanValueOverWarningStdevMagHB";
            this.lblNoiseMaxMinusMeanValueOverWarningStdevMagHB.Size = new System.Drawing.Size(351, 17);
            this.lblNoiseMaxMinusMeanValueOverWarningStdevMagHB.TabIndex = 25;
            this.lblNoiseMaxMinusMeanValueOverWarningStdevMagHB.Text = "Max-Mean Value Over Warning Stdev Magnification HB :";
            // 
            // tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB
            // 
            this.tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB.Location = new System.Drawing.Point(368, 30);
            this.tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB.Name = "tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB";
            this.tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB.Size = new System.Drawing.Size(81, 25);
            this.tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB.TabIndex = 26;
            this.tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB.Text = "10";
            // 
            // frmParameterSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 538);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tctrlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmParameterSetting";
            this.Text = "Parameter Setting";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmParameterSetting_FormClosing);
            this.tctrlMain.ResumeLayout(false);
            this.tpgNormal.ResumeLayout(false);
            this.gbxProjectInfo.ResumeLayout(false);
            this.gbxProjectInfo.PerformLayout();
            this.gbxConnectSetting.ResumeLayout(false);
            this.gbxConnectSetting.PerformLayout();
            this.gbxElanBridgeSetting.ResumeLayout(false);
            this.gbxElanBridgeSetting.PerformLayout();
            this.tpgProcess.ResumeLayout(false);
            this.gbxProcess.ResumeLayout(false);
            this.gbxProcess.PerformLayout();
            this.gbxIndependentStepSetting.ResumeLayout(false);
            this.gbxIndependentStepSetting.PerformLayout();
            this.tpgCoordinate.ResumeLayout(false);
            this.tcCoordinateSetting.ResumeLayout(false);
            this.tpgLTRobotCoordinate.ResumeLayout(false);
            this.gbxNormalCoordinate.ResumeLayout(false);
            this.gbxNormalCoordinate.PerformLayout();
            this.gbxTPGTCoordinate.ResumeLayout(false);
            this.gbxTPGTCoordinate.PerformLayout();
            this.gbxTPGTVerticalLine.ResumeLayout(false);
            this.gbxTPGTVerticalLine.PerformLayout();
            this.gbxTPGTHorizontalLine.ResumeLayout(false);
            this.gbxTPGTHorizontalLine.PerformLayout();
            this.tpgGoDrawCoordinate.ResumeLayout(false);
            this.gbxGoDrawNormalCoordinate.ResumeLayout(false);
            this.gbxGoDrawNormalCoordinate.PerformLayout();
            this.gbxGoDrawTPGTCoordinate.ResumeLayout(false);
            this.gbxGoDrawTPGTCoordinate.PerformLayout();
            this.gbxGoDrawTPGTVerticalLine.ResumeLayout(false);
            this.gbxGoDrawTPGTVerticalLine.PerformLayout();
            this.gbxGoDrawTPGTHorizontalLine.ResumeLayout(false);
            this.gbxGoDrawTPGTHorizontalLine.PerformLayout();
            this.tpgSpeed.ResumeLayout(false);
            this.gbxSpeedSetting.ResumeLayout(false);
            this.gbxSpeedSetting.PerformLayout();
            this.tpgColorPattern.ResumeLayout(false);
            this.tpgColorPattern.PerformLayout();
            this.gbxColorPattern.ResumeLayout(false);
            this.gbxColorPattern.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbxManualPattern)).EndInit();
            this.tpgGen8Command.ResumeLayout(false);
            this.gbxCommandSetting.ResumeLayout(false);
            this.gbxCommandSetting.PerformLayout();
            this.tpgOtherSetting.ResumeLayout(false);
            this.tctrlOtherSetting.ResumeLayout(false);
            this.tpgNoiseAndTNStep.ResumeLayout(false);
            this.tpgNoiseAndTNStep.PerformLayout();
            this.tabDGTStep.ResumeLayout(false);
            this.tabDGTStep.PerformLayout();
            this.tpgTPGTStep.ResumeLayout(false);
            this.tpgTPGTStep.PerformLayout();
            this.tpgDTAndPCTStep.ResumeLayout(false);
            this.tpgDTAndPCTStep.PerformLayout();
            this.tpgPCTStep.ResumeLayout(false);
            this.tpgPCTStep.PerformLayout();
            this.tpgDigitalTuningStep.ResumeLayout(false);
            this.tpgDigitalTuningStep.PerformLayout();
            this.tpgTiltTuningStep.ResumeLayout(false);
            this.tpgTiltTuningStep.PerformLayout();
            this.tpgPTStep.ResumeLayout(false);
            this.tpgPTStep.PerformLayout();
            this.tpgLTStep.ResumeLayout(false);
            this.tpgLTStep.PerformLayout();
            this.tpgFWParameterSetting.ResumeLayout(false);
            this.tpgFWParameterSetting.PerformLayout();
            this.tpgOther.ResumeLayout(false);
            this.tpgOther.PerformLayout();
            this.gbxNoiseMaxValueOverWarningHB.ResumeLayout(false);
            this.gbxNoiseMaxValueOverWarningHB.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tctrlMain;
        private System.Windows.Forms.TabPage tpgNormal;
        private System.Windows.Forms.GroupBox gbxProjectInfo;
        private System.Windows.Forms.TextBox tbxProjectName;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.TabPage tpgCoordinate;
        private System.Windows.Forms.GroupBox gbxNormalCoordinate;
        private System.Windows.Forms.Label lblHoverHeight_DT2nd;
        private System.Windows.Forms.TextBox tbxHoverHeight_DT2nd;
        private System.Windows.Forms.Label lblHoverHeight_DT1st;
        private System.Windows.Forms.TextBox tbxHoverHeight_DT1st;
        private System.Windows.Forms.Label lblContactZCoord;
        private System.Windows.Forms.Label lblEndYCoord;
        private System.Windows.Forms.Label lblEndXCoord;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.Label lblStartYCoord;
        private System.Windows.Forms.Label lblStartXCoord;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.TextBox tbxZ;
        private System.Windows.Forms.TextBox tbxY2;
        private System.Windows.Forms.TextBox tbxX2;
        private System.Windows.Forms.TextBox tbxY1;
        private System.Windows.Forms.TextBox tbxX1;
        private System.Windows.Forms.TabPage tpgColorPattern;
        private System.Windows.Forms.CheckBox chbxDisplayProgressStatus;
        private System.Windows.Forms.GroupBox gbxColorPattern;
        private System.Windows.Forms.Label lblScreenSize;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.PictureBox picbxManualPattern;
        private System.Windows.Forms.Button btnPHCKPatternOption;
        private System.Windows.Forms.RadioButton rbtnManual;
        private System.Windows.Forms.RadioButton rbtnPHCKPattern;
        private System.Windows.Forms.TextBox tbxPatternPath;
        private System.Windows.Forms.Panel pnlDisplay;
        private System.Windows.Forms.Button btnColorSelect;
        private System.Windows.Forms.Button btnPatternPath;
        private System.Windows.Forms.ComboBox cbxColorSelect;
        private System.Windows.Forms.RadioButton rbtnPattern;
        private System.Windows.Forms.RadioButton rbtnColor;
        private System.Windows.Forms.TabPage tpgOtherSetting;
        private System.Windows.Forms.TabControl tctrlOtherSetting;
        private System.Windows.Forms.TabPage tpgNoiseAndTNStep;
        private System.Windows.Forms.TextBox tbxNoiseProcessReportNumber;
        private System.Windows.Forms.Label lblNoiseProcessReportNumber;
        private System.Windows.Forms.TextBox tbxNoiseValidReportNumber;
        private System.Windows.Forms.Label lblNoiseValidReportNumber;
        private System.Windows.Forms.TextBox tbxNoiseTimeout;
        private System.Windows.Forms.Label lblNoiseTimeout;
        private System.Windows.Forms.TextBox tbxNoiseReportNumber;
        private System.Windows.Forms.Label lblNoiseReportNumber;
        private System.Windows.Forms.TabPage tpgDTAndPCTStep;
        private System.Windows.Forms.TextBox tbxTRxSTXValidReportNumber;
        private System.Windows.Forms.Label lblTRxSTXValidReportNumber;
        private System.Windows.Forms.TextBox tbxTRxSRXValidReportNumber;
        private System.Windows.Forms.Label lblTRxSRXValidReportNumber;
        private System.Windows.Forms.TextBox tbxNormalFilterTXValidReportNumber;
        private System.Windows.Forms.Label lblNormalFilterTXValidReportNumber;
        private System.Windows.Forms.TextBox tbxNormalFilterRXValidReportNumber;
        private System.Windows.Forms.Label lblNormalFilterRXValidReportNumber;
        private System.Windows.Forms.TextBox tbxNormalValidReportNumber;
        private System.Windows.Forms.Label lblNormalValidReportNumber;
        private System.Windows.Forms.TabPage tpgDigitalTuningStep;
        private System.Windows.Forms.TextBox tbxNormalContactTHRatio_TX;
        private System.Windows.Forms.Label lblNormalContactTHRatio_TX;
        private System.Windows.Forms.TextBox tbxNormalContactTHRatio_RX;
        private System.Windows.Forms.Label lblNormalContactTHRatio_RX;
        private System.Windows.Forms.TextBox tbxNormalHoverTHRatio_TX;
        private System.Windows.Forms.Label lblNormalHoverTHRatio_TX;
        private System.Windows.Forms.TextBox tbxNormalHoverTHRatio_RX;
        private System.Windows.Forms.Label lblNormalHoverTHRatio_RX;
        private System.Windows.Forms.TabPage tpgTiltTuningStep;
        private System.Windows.Forms.TextBox tbxTTTXValidReportNumber;
        private System.Windows.Forms.Label lblTTTXValidReportNumber;
        private System.Windows.Forms.TextBox tbxTTRXValidReportNumber;
        private System.Windows.Forms.Label lblTTRXValidReportNumber;
        private System.Windows.Forms.TextBox tbxTTValidTipTraceNumber;
        private System.Windows.Forms.Label lblTTValidTipTraceNumber;
        private System.Windows.Forms.TabPage tpgOther;
        private System.Windows.Forms.ComboBox cbxAutoTune_P0_detect_time;
        private System.Windows.Forms.Label lblAutoTune_P0_detect_time;
        private System.Windows.Forms.TextBox tbxDrawLineTimeout;
        private System.Windows.Forms.Label lblDrawLineTimeout;
        private System.Windows.Forms.TextBox tbxRecordDataRetryCount;
        private System.Windows.Forms.Label lblRecordDataRetryCount;
        private System.Windows.Forms.TextBox tbxStartDelayTime;
        private System.Windows.Forms.Label lblStartDelayTime;
        private System.Windows.Forms.TabPage tpgPTStep;
        private System.Windows.Forms.TextBox tbxPTEndSkipReportNumber;
        private System.Windows.Forms.Label lblPTEndSkipReportNumber;
        private System.Windows.Forms.TextBox tbxPTStartSkipReportNumber;
        private System.Windows.Forms.Label lblPTStartSkipReportNumber;
        private System.Windows.Forms.TextBox tbxPTValidReportNumber;
        private System.Windows.Forms.Label lblPTValidReportNumber;
        private System.Windows.Forms.CheckBox chbxDisplayTimeAndReportNumber;
        private System.Windows.Forms.TextBox tbxPTRecordTime;
        private System.Windows.Forms.Label lblPTRecordTime;
        private System.Windows.Forms.ComboBox cbxPTPenVersion;
        private System.Windows.Forms.Label lblPTPenVersion;
        private System.Windows.Forms.ComboBox cbxFWType;
        private System.Windows.Forms.Label lblFWType;
        private System.Windows.Forms.Label LblHoverHeight_PCT2nd;
        private System.Windows.Forms.TextBox tbxHoverHeight_PCT2nd;
        private System.Windows.Forms.Label LblHoverHeight_PCT1st;
        private System.Windows.Forms.TextBox tbxHoverHeight_PCT1st;
        private System.Windows.Forms.TabPage tpgPCTStep;
        private System.Windows.Forms.TextBox tbxPeakCheckRatio3T;
        private System.Windows.Forms.Label lblPeakCheckRatio3T;
        private System.Windows.Forms.TextBox tbxPeakCheckRatio5T;
        private System.Windows.Forms.Label lblPeakCheckRatio5T;
        private System.Windows.Forms.TextBox tbxPeakCheckRatio;
        private System.Windows.Forms.Label lblPeakCheckRatio;
        private System.Windows.Forms.TextBox tbxNormal800to400PwrRatio;
        private System.Windows.Forms.Label lblNormal800to400PwrRatio;
        private System.Windows.Forms.TextBox tbxNoiseInnerRefValueHB;
        private System.Windows.Forms.Label lblNoiseInnerRefValueHB;
        private System.Windows.Forms.TextBox tbxTNFreqNumber;
        private System.Windows.Forms.Label lblTNFreqNumber;
        private System.Windows.Forms.TextBox tbxOtherFreqNumber;
        private System.Windows.Forms.Label lblOtherFreqNumber;
        private System.Windows.Forms.TabPage tabDGTStep;
        private System.Windows.Forms.TextBox tbxDGTCompensatePower;
        private System.Windows.Forms.Label lblDGTCompensatePower;
        private System.Windows.Forms.TextBox tbxDGTTXValidReportNumber;
        private System.Windows.Forms.Label lblDGTTXValidReportNumber;
        private System.Windows.Forms.TextBox tbxDGTRXValidReportNumber;
        private System.Windows.Forms.Label lblDGTRXValidReportNumber;
        private System.Windows.Forms.ComboBox cbxSetDigiGain;
        private System.Windows.Forms.Label lblSetDigiGain;
        private System.Windows.Forms.TextBox tbxNoiseNoReportInterruptTime;
        private System.Windows.Forms.Label lblNoiseNoReportInterruptTime;
        private System.Windows.Forms.TextBox tbxDGTDigiGainScaleHB;
        private System.Windows.Forms.Label lblDGTDigiGainScaleHB;
        private System.Windows.Forms.TextBox tbxDGTDigiGainScaleLB;
        private System.Windows.Forms.Label lblDGTDigiGainScaleLB;
        private System.Windows.Forms.ComboBox cbxDTDisplayChartDetailValue;
        private System.Windows.Forms.Label lblDTDisplayChartDetailValue;
        private System.Windows.Forms.TabPage tpgProcess;
        private System.Windows.Forms.GroupBox gbxProcess;
        private System.Windows.Forms.ComboBox cbxProcessType;
        private System.Windows.Forms.Label lblProcessType;
        private System.Windows.Forms.TextBox tbxFixedPH2;
        private System.Windows.Forms.Label lblFixedPH2;
        private System.Windows.Forms.TextBox tbxFixedPH1;
        private System.Windows.Forms.Label lblFixedPH1;
        private System.Windows.Forms.Label lblNotIncludeMessage;
        private System.Windows.Forms.GroupBox gbxIndependentStepSetting;
        private System.Windows.Forms.TabPage tpgFWParameterSetting;
        private System.Windows.Forms.TextBox tbxParameter_cActivePen_FM_P0_TH;
        private System.Windows.Forms.Label lblParameter_cAcitvePen_FM_P0_TH;
        private System.Windows.Forms.TextBox tbxDTThresholdRatio_TX;
        private System.Windows.Forms.Label lblDTThresholdRatio_TX;
        private System.Windows.Forms.TextBox tbxDTThresholdRatio_RX;
        private System.Windows.Forms.Label lblDTThresholdRatio_RX;
        private System.Windows.Forms.ComboBox cbxDTSkipCompareThreshold;
        private System.Windows.Forms.Label lblDTSkipCompareThreshold;
        private System.Windows.Forms.TabPage tpgSpeed;
        private System.Windows.Forms.GroupBox gbxSpeedSetting;
        private System.Windows.Forms.TextBox tbxLTSpeed;
        private System.Windows.Forms.Label lblLTSpeed;
        private System.Windows.Forms.Label lblPCTSpeed;
        private System.Windows.Forms.TextBox tbxPCTSpeed;
        private System.Windows.Forms.Label lblDGTSpeed;
        private System.Windows.Forms.TextBox tbxDGTSpeed;
        private System.Windows.Forms.TextBox tbxTTSpeed;
        private System.Windows.Forms.Label lblTTSpeed;
        private System.Windows.Forms.Label lblDTSpeed;
        private System.Windows.Forms.TextBox tbxDTSpeed;
        private System.Windows.Forms.GroupBox gbxTPGTCoordinate;
        private System.Windows.Forms.GroupBox gbxTPGTHorizontalLine;
        private System.Windows.Forms.Label lblTPGTHorizontalStart;
        private System.Windows.Forms.TextBox tbxTPGTHorizontalStartXCoord;
        private System.Windows.Forms.Label lblTPGTHorizontalEndYCoord;
        private System.Windows.Forms.TextBox tbxTPGTHorizontalStartYCoord;
        private System.Windows.Forms.Label lblTPGTHorizontalEndXCood;
        private System.Windows.Forms.TextBox tbxTPGTHorizontalEndXCoord;
        private System.Windows.Forms.Label lblTPGTHorizontalEnd;
        private System.Windows.Forms.TextBox tbxTPGTHorizontalEndYCoord;
        private System.Windows.Forms.Label lblTPGTHorizontalStartYCoord;
        private System.Windows.Forms.Label lblTPGTHorizontalStartXCoord;
        private System.Windows.Forms.Label lblTPGTContactZCoord;
        private System.Windows.Forms.TextBox tbxTPGTContactZCoord;
        private System.Windows.Forms.GroupBox gbxTPGTVerticalLine;
        private System.Windows.Forms.Label lblTPGTVerticalStart;
        private System.Windows.Forms.TextBox tbxTPGTVerticalStartXCoord;
        private System.Windows.Forms.Label lblTPGTVerticalEndYCoord;
        private System.Windows.Forms.TextBox tbxTPGTVerticalStartYCoord;
        private System.Windows.Forms.Label lblTPGTVerticalEndXCoord;
        private System.Windows.Forms.TextBox tbxTPGTVerticalEndXCoord;
        private System.Windows.Forms.Label lblTPGTVerticalEnd;
        private System.Windows.Forms.TextBox tbxTPGTVerticalEndYCoord;
        private System.Windows.Forms.Label lblTPGTVerticalStartYCoord;
        private System.Windows.Forms.Label lblTPGTVerticalStartXCoord;
        private System.Windows.Forms.Label lblTPGTSpeed;
        private System.Windows.Forms.TextBox tbxTPGTSpeed;
        private System.Windows.Forms.TabPage tpgTPGTStep;
        private System.Windows.Forms.TextBox tbxTPGTTXValidReportNumber;
        private System.Windows.Forms.Label lblTPGTTXValidReportNumber;
        private System.Windows.Forms.TextBox tbxTPGTRXValidReportNumber;
        private System.Windows.Forms.Label lblTPGTRXValidReportNumber;
        private System.Windows.Forms.TextBox tbxTPGTRXEndPin;
        private System.Windows.Forms.Label lblTPGTRXEndPin;
        private System.Windows.Forms.TextBox tbxTPGTRXStartPin;
        private System.Windows.Forms.Label lblTPGTRXStartPin;
        private System.Windows.Forms.TextBox tbxTPGTTXEndPin;
        private System.Windows.Forms.Label lblTPGTTXEndPin;
        private System.Windows.Forms.TextBox tbxTPGTTXStartPin;
        private System.Windows.Forms.Label lblTPGTTXStartPin;
        private System.Windows.Forms.TextBox tbxTPGTGainRatio;
        private System.Windows.Forms.Label lblTPGTGainRatio;
        private System.Windows.Forms.TextBox tbxTPGTVerticalRAngle;
        private System.Windows.Forms.Label lblTPGTVerticalRAngle;
        private System.Windows.Forms.TextBox tbxTPGTHorizontalRAngle;
        private System.Windows.Forms.Label lblTPGTHorizontalRAngle;
        private System.Windows.Forms.TextBox tbxTPGTVAngle;
        private System.Windows.Forms.Label lblTPGTVAngle;
        private System.Windows.Forms.Label lblTPGTDisaplayMessage;
        private System.Windows.Forms.ComboBox cbxTPGTDisplayMessage;
        private System.Windows.Forms.TabControl tcCoordinateSetting;
        private System.Windows.Forms.TabPage tpgLTRobotCoordinate;
        private System.Windows.Forms.TabPage tpgGoDrawCoordinate;
        private System.Windows.Forms.GroupBox gbxGoDrawNormalCoordinate;
        private System.Windows.Forms.Label lblGoDrawHoverZServoValue_PCT2nd;
        private System.Windows.Forms.TextBox tbxGoDrawHoverZServoValue_PCT2nd;
        private System.Windows.Forms.Label lblGoDrawHoverZServoValue_PCT1st;
        private System.Windows.Forms.TextBox tbxGoDrawHoverZServoValue_PCT1st;
        private System.Windows.Forms.Label lblGoDrawHoverZServoValue_DT2nd;
        private System.Windows.Forms.TextBox tbxGoDrawHoverZServoValue_DT2nd;
        private System.Windows.Forms.Label lblGoDrawHoverZServoValue_DT1st;
        private System.Windows.Forms.TextBox tbxGoDrawHoverZServoValue_DT1st;
        private System.Windows.Forms.Label lblGoDrawContactZServoValue;
        private System.Windows.Forms.Label lblGoDrawEndYCoord;
        private System.Windows.Forms.Label lblGoDrawEndXCoord;
        private System.Windows.Forms.Label lblGoDrawEnd;
        private System.Windows.Forms.Label lblGoDrawStartYCoord;
        private System.Windows.Forms.Label lblGoDrawStartXCoord;
        private System.Windows.Forms.Label lblGoDrawStart;
        private System.Windows.Forms.TextBox tbxGoDrawContactZServoValue;
        private System.Windows.Forms.TextBox tbxGoDrawEndYCoord;
        private System.Windows.Forms.TextBox tbxGoDrawEndXCoord;
        private System.Windows.Forms.TextBox tbxGoDrawStartYCoord;
        private System.Windows.Forms.TextBox tbxGoDrawStartXCoord;
        private System.Windows.Forms.GroupBox gbxGoDrawTPGTCoordinate;
        private System.Windows.Forms.GroupBox gbxGoDrawTPGTVerticalLine;
        private System.Windows.Forms.Label lblGoDrawTPGTVerticalStart;
        private System.Windows.Forms.TextBox tbxGoDrawTPGTVerticalStartXCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTVerticalEndYCoord;
        private System.Windows.Forms.TextBox tbxGoDrawTPGTVerticalStartYCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTVerticalEndXCoord;
        private System.Windows.Forms.TextBox tbxGoDrawTPGTVerticalEndXCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTVerticalEnd;
        private System.Windows.Forms.TextBox tbxGoDrawTPGTVerticalEndYCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTVerticalStartYCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTVerticalStartXCoord;
        private System.Windows.Forms.GroupBox gbxGoDrawTPGTHorizontalLine;
        private System.Windows.Forms.Label lblGoDrawTPGTHorizontalStart;
        private System.Windows.Forms.TextBox tbxGoDrawTPGTHorizontalStartXCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTHorizontalEndYCoord;
        private System.Windows.Forms.TextBox tbxGoDrawTPGTHorizontalStartYCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTHorizontalEndXCoord;
        private System.Windows.Forms.TextBox tbxGoDrawTPGTHorizontalEndXCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTHorizontalEnd;
        private System.Windows.Forms.TextBox tbxGoDrawTPGTHorizontalEndYCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTHorizontalStartYCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTHorizontalStartXCoord;
        private System.Windows.Forms.Label lblGoDrawTPGTContactZServoValue;
        private System.Windows.Forms.TextBox tbxGoDrawTPGTContactZServoValue;
        private System.Windows.Forms.Label lblGoDrawTopZServoValue;
        private System.Windows.Forms.TextBox tbxGoDrawTopZServoValue;
        private System.Windows.Forms.TabPage tpgLTStep;
        private System.Windows.Forms.ComboBox cbxLTTP_GainCompensate;
        private System.Windows.Forms.Label lblLTTP_GainCompensate;
        private System.Windows.Forms.RadioButton rbtnMonitorOFF;
        private System.Windows.Forms.ComboBox cbx5TRawDataType;
        private System.Windows.Forms.Label lbl5TRawDataType;
        private System.Windows.Forms.TabPage tpgGen8Command;
        private System.Windows.Forms.GroupBox gbxCommandSetting;
        private System.Windows.Forms.ComboBox cbxCommandScriptType;
        private System.Windows.Forms.Label lblCommandScriptType;
        private System.Windows.Forms.Label lblUserDefinedPath;
        private System.Windows.Forms.TextBox tbxUserDeifnedPath;
        private System.Windows.Forms.Button btnUserDefinedSelect;
        private System.Windows.Forms.ComboBox cbxGen8FilterType;
        private System.Windows.Forms.Label lblGen8FilterType;
        private System.Windows.Forms.ComboBox cbxGen8AFEType;
        private System.Windows.Forms.Label lblGen8AFEType;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox gbxConnectSetting;
        private System.Windows.Forms.Label lblI2CAddress;
        private System.Windows.Forms.TextBox tbxI2CAddress;
        private System.Windows.Forms.Label lblVID;
        private System.Windows.Forms.TextBox tbxVID;
        private System.Windows.Forms.Label lblPID;
        private System.Windows.Forms.TextBox tbxPID;
        private System.Windows.Forms.RadioButton rbtnDisable;
        private System.Windows.Forms.ComboBox cbxDT7318TRxSSpecificReportType;
        private System.Windows.Forms.Label lblDT7318TRxSSpecificReportType;
        private System.Windows.Forms.TextBox tbxTTSlantSpeed;
        private System.Windows.Forms.Label lblTTSlantSpeed;
        private System.Windows.Forms.GroupBox gbxElanBridgeSetting;
        private System.Windows.Forms.Label lblNormalLegth;
        private System.Windows.Forms.TextBox tbxNormalLength;
        private System.Windows.Forms.ComboBox cbxVIO;
        private System.Windows.Forms.Label lblVIO;
        private System.Windows.Forms.ComboBox cbxDVDD;
        private System.Windows.Forms.Label lblDVDD;
        private System.Windows.Forms.TextBox tbxNoiseMaxValueOverWarningAbsValueHB;
        private System.Windows.Forms.Label lblNoiseMaxValueOverWarningAbsValueHB;
        private System.Windows.Forms.GroupBox gbxNoiseMaxValueOverWarningHB;
        private System.Windows.Forms.Label lblNoiseMaxMinusMeanValueOverWarningStdevMagHB;
        private System.Windows.Forms.TextBox tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB;
        /*
private System.Windows.Forms.TextBox tbxDGTDefaultScaleDigiGain_BHF_Tx;
private System.Windows.Forms.Label lblDGTDefaultScaleDigiGain_BHF_Tx;
private System.Windows.Forms.TextBox tbxDGTDefaultScaleDigiGain_BHF_Rx;
private System.Windows.Forms.Label lblDGTDefaultScaleDigiGain_BHF_Rx;
private System.Windows.Forms.TextBox tbxDGTDefaultScaleDigiGain_PTHF_Tx;
private System.Windows.Forms.Label lblDGTDefaultScaleDigiGain_PTHF_Tx;
private System.Windows.Forms.TextBox tbxDGTDefaultScaleDigiGain_PTHF_Rx;
private System.Windows.Forms.Label lblDGTDefaultScaleDigiGain_PTHF_Rx;
private System.Windows.Forms.TextBox tbxDGTDefaultScaleDigiGain_Beacon_Tx;
private System.Windows.Forms.Label lblDGTDefaultScaleDigiGain_Beacon_Tx;
private System.Windows.Forms.TextBox tbxDGTDefaultScaleDigiGain_Beacon_Rx;
private System.Windows.Forms.Label lblDGTDefaultScaleDigiGain_Beacon_Rx;
private System.Windows.Forms.TextBox tbxDGTDefaultScaleDigiGain_P0;
private System.Windows.Forms.Label lblDGTDefaultScaleDigiGain_P0;
*/
    }
}